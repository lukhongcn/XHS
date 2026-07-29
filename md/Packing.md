# Packing 装箱模块技术文档

## 概述

装箱模块（Packing）是 XHS 生产管理系统中的核心模块，负责 KD 件装箱扫描、零件匹配核验、异常锁定与审核解除的完整业务流程。

**页面地址**：`XHSWorkFlow/Packing.aspx?taskId={guid}`

**核心能力**：
- KD 标签扫描创建装箱任务
- 随箱码扫描绑定
- 零件标签逐件扫描核验（BOM 匹配、数量校验、重复检测）
- 基于 Token 的并发控制与异常锁定
- 主管审核解除锁定后恢复装箱

---

## 架构分层

```
Packing.aspx (页面层)
    ↓
PackingOperationService (BLL 业务事务层)  ←  QRCode (KEY_VALUE 硬编码解析)
    ↓
DALFactory (工厂层)
    ↓
IPacking* (IDAL 接口层)
    ↓
MSSQL Packing* (数据访问实现层)
    ↓
tb_Packing* (数据库表)

条码解析链路（独立于 Packing 事务，在页面层直接调用）：
FactoryBarcodeParser → LabelCodeRule (取 MatchRegex) → ILabelCodeRule → MSSQL.LabelCodeRule → tb_LabelCodeRule
                     → RegexFieldParser (Regex 命名群組提取)
                     → 手動映射 PartInfo
（LabelCodeRuleField 全鏈路未使用）
```

---

## 页面 UI 结构

### 扫描区域

| 控件 | 类型 | 说明 |
|------|------|------|
| `txt_ScanQRCode` | TextBox | 扫描输入框，回车自动提交，AutoPostBack |
| `rblScanType` | RadioButtonList | 扫描类型：KD 标签 / 零件标签 / 随箱码 |
| `txt_SupplyBatchNo` | TextBox | 供货批次号（黄色必填背景） |
| `txt_PartNo` | TextBox | 零件编号（黄色必填背景） |
| `txt_CartonNo` | TextBox | 箱号（黄色必填背景） |
| `txt_PlanQty` | TextBox | 计划数量（黄色必填背景） |
| `txt_MaterialNo` | TextBox | 物料号 |
| `txt_Qty` | TextBox | 本次数量，默认值 1 |
| `txt_Status` | TextBox | 装箱状态（只读） |
| `txt_ExceptionStatus` | TextBox | 异常状态（只读） |
| `txt_LockToken` | TextBox | 当前令牌（只读） |
| `hidPackingId` | HiddenField | 装箱记录 ID |
| `hidLockToken` | HiddenField | 当前令牌隐藏域 |

### 按钮

| 按钮 | 样式 | 说明 |
|------|------|------|
| `lnk_view` | btn5 | 浏览/刷新页面 |
| `lnkbutton_save` | btn3 | 保存/完成装箱 |

### 扫描明细

`gvScanRecords` GridView 展示字段：二维码类型、二维码、物料号、数量、扫描人、扫描时间。

### 锁定遮罩

`pnlLocked` 面板在异常锁定时以半透明遮罩覆盖页面，显示红色警告卡片，阻止所有操作。

---

## 业务流程

### 完整流程图

```
打开装箱页面
     │
     ▼
扫描 KD 标签 ──→ 解析条码（FactoryBarcodeParser）
     │             创建 PackingRecord + 生成 LockToken
     │             插入 KD 扫描记录
     │
     ├── 已有任务（TaskId）→ 直接绑定，自动切至"零件标签"模式
     ├── 异常锁定 → 冻结页面 + 显示锁定遮罩
     └── 已完成 → 只读展示
     │
     ▼
扫描随箱码（可选）──→ 验证 Token → 检查重复 → 写入 PackingQRCode
     │
     ▼
扫描零件标签 ──→ 验证 Token → 检查重复 → BOM 零件匹配
     │             检查数量是否超计划 → 累加 PackingQty
     │
     ├── 零件不匹配 → 异常锁定（PART001）
     ├── 标签重复 → 异常锁定（LABEL001）
     ├── 数量超计划 → 异常锁定（QTY001）
     └── 随箱码重复 → 异常锁定（BOX001）
     │
     ▼
完成装箱 ──→ 验证 Token → 校验数量 == 计划数量 → Status=1
     │
     ▼
异常审核（ExceptionReview.aspx）──→ 通过 → 解锁恢复 / 拒绝 → 保持锁定
```

### 扫描类型切换逻辑

| 条件 | KD 标签 | 随箱码 | 零件标签 |
|------|---------|--------|----------|
| 无活动任务 | ✅ 启用 | ❌ 禁用 | ❌ 禁用 |
| 有活动任务 | ❌ 禁用 | ✅ 启用 | ✅ 启用 |
| 已锁定/已完成 | ❌ 禁用 | ❌ 禁用 | ❌ 禁用 |

---

## 数据库表结构

### tb_PackingRecord（装箱记录主表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint (PK, IDENTITY) | 主键 |
| SupplyBatchNo | nvarchar(50) | 供货批次号 |
| PartNo | nvarchar(50) | 零件编号 |
| CartonNo | nvarchar(50) | 箱号 |
| KDQRCode | nvarchar(100) | KD 标签二维码内容 |
| PackingQRCode | nvarchar(100) | 随箱码内容 |
| TaskId | uniqueidentifier | 装箱任务唯一标识（URL 参数传递） |
| Status | int | 0=装箱中，1=已完成 |
| PlanQty | int | 计划装箱数量 |
| PackingQty | int | 已扫描数量（累加） |
| PackingUser | nvarchar(50) | 装箱完成人 |
| PackingTime | datetime | 装箱完成时间 |
| ExceptionStatus | int | 0=正常，1=异常暂停 |
| LockToken | nvarchar(100) | 并发控制令牌（Guid 32 位） |
| LockTime | datetime | 锁定时间 |
| LockUser | nvarchar(50) | 锁定操作人 |
| LockMachine | nvarchar(50) | 锁定机器 |
| ClientId | nvarchar(50) | 客户端标识 |
| MachineId | nvarchar(50) | 机器标识 |
| CreateUser | nvarchar(50) | 创建人 |
| CreateTime | datetime | 创建时间 |
| UpdateUser | nvarchar(50) | 更新人 |
| UpdateTime | datetime | 更新时间 |

### tb_PackingScanRecord（扫描明细表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint (PK, IDENTITY) | 主键 |
| PackingId | bigint (FK) | 关联装箱记录 |
| QRCodeType | nvarchar(20) | 二维码类型：KD / 随箱码 / 零件标签 |
| QRCode | nvarchar(200) | 二维码内容 |
| MaterialNo | nvarchar(50) | 物料号 |
| Qty | int | 数量 |
| ScanUser | nvarchar(50) | 扫描人 |
| ScanTime | datetime | 扫描时间 |

### tb_PackingException（异常记录表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | bigint (PK, IDENTITY) | 主键 |
| PackingId | bigint (FK) | 关联装箱记录 |
| BoxCode | nvarchar(50) | 箱号（兼容旧字段） |
| CartonNo | nvarchar(50) | 箱号 |
| ExceptionCode | nvarchar(50) | 异常代码 |
| ExceptionMessage | nvarchar(500) | 异常描述 |
| TriggerQRCode | nvarchar(200) | 触发异常的二维码 |
| LockToken | nvarchar(100) | 锁定时令牌 |
| Status | int | 0=待审核，1=已通过，2=已驳回 |
| CreateUser | nvarchar(50) | 创建人 |
| CreateTime | datetime | 创建时间 |
| AuditUser | nvarchar(50) | 审核人 |
| AuditTime | datetime | 审核时间 |
| AuditRemark | nvarchar(500) | 审核备注 |

**约束**：
- 外键 `FK_tb_PackingException_tb_PackingRecord`：PackingId → tb_PackingRecord.Id
- 过滤唯一索引 `IX_tb_PackingException_UniquePending`：同一 PackingId 最多一笔 Status=0

### tb_PackingExceptionType（异常类型基础表）

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int (PK) | 主键 |
| ExceptionCode | nvarchar(50) | 异常代码 |
| ExceptionName | nvarchar(MAX) | 异常名称 |
| IsLock | bit | 是否触发锁定 |
| Enable | bit | 是否启用 |
| CreateTime | datetime | 创建时间 |

**预置异常类型**：

| 代码 | 名称 | 触发场景 |
|------|------|----------|
| PART001 | 零件不匹配 | KD 零件号 ≠ 标签零件号 |
| QTY001 | 数量超出 | 累计扫描数量 > 计划数量 |
| BOX001 | 随箱码重复 | 随箱码已被其他箱使用 |
| LABEL001 | 零件标签重复 | 同一二维码重复扫描 |

---

## 模型层（XHS.Model）

### PackingRecordInfo

装箱记录实体，对应 `tb_PackingRecord` 表。包含 22 个属性，所有可空类型均为 `Nullable<T>`。

| 属性 | 类型 | 说明 |
|------|------|------|
| Id | long? | 主键 |
| SupplyBatchNo | string | 供货批次号 |
| PartNo | string | 零件编号 |
| CartonNo | string | 箱号 |
| KDQRCode | string | KD 标签二维码 |
| PackingQRCode | string | 随箱码 |
| TaskId | Guid? | 任务唯一标识 |
| Status | int? | 装箱状态 |
| PlanQty | int? | 计划数量 |
| PackingQty | int? | 已装箱数量 |
| PackingUser | string | 装箱完成人 |
| PackingTime | DateTime? | 装箱完成时间 |
| ExceptionStatus | int? | 异常状态 |
| LockToken | string | 锁定令牌 |
| LockTime | DateTime? | 锁定时间 |
| LockUser | string | 锁定操作人 |
| LockMachine | string | 锁定机器 |
| ClientId | string | 客户端标识 |
| MachineId | string | 机器标识 |
| CreateUser | string | 创建人 |
| CreateTime | DateTime? | 创建时间 |
| UpdateUser | string | 更新人 |
| UpdateTime | DateTime? | 更新时间 |

### PackingScanRecordInfo

扫描记录实体，对应 `tb_PackingScanRecord` 表。

| 属性 | 类型 | 说明 |
|------|------|------|
| Id | long? | 主键 |
| PackingId | long? | 装箱记录 ID |
| QRCodeType | string | 二维码类型（KD/随箱码/零件标签） |
| QRCode | string | 二维码内容 |
| MaterialNo | string | 物料号 |
| Qty | int? | 数量 |
| ScanUser | string | 扫描人 |
| ScanTime | DateTime? | 扫描时间 |

### PackingScanRecordQRCodeTypeInfo

二维码类型常量类（不映射数据表）。

| 常量 | 值 | 说明 |
|------|-----|------|
| KD | "KD" | KD 标签 |
| Packing | "随箱码" | 随箱码 |
| MaterialLabel | "零件标签" | 零件标签 |

### PackingExceptionInfo

异常记录实体，对应 `tb_PackingException` 表。

| 属性 | 类型 | 说明 |
|------|------|------|
| Id | long? | 主键 |
| PackingId | long? | 装箱记录 ID |
| BoxCode | string | 箱号（兼容） |
| CartonNo | string | 箱号 |
| ExceptionCode | string | 异常代码 |
| ExceptionMessage | string | 异常消息 |
| TriggerQRCode | string | 触发二维码 |
| LockToken | string | 锁定令牌 |
| Status | int? | 0=待审核，1=通过，2=驳回 |
| CreateUser | string | 创建人 |
| CreateTime | DateTime? | 创建时间 |
| AuditUser | string | 审核人 |
| AuditTime | DateTime? | 审核时间 |
| AuditRemark | string | 审核备注 |
| KDQRCode | string | 关联的 KD 标签（JOIN 填充） |
| KDPartNo | string | 解析出的零件编号（非数据库字段） |

### PackingExceptionTypeInfo

异常类型实体，对应 `tb_PackingExceptionType` 表。

| 属性 | 类型 | 说明 |
|------|------|------|
| Id | int | 主键 |
| ExceptionCode | string | 异常代码 |
| ExceptionName | string | 异常名称 |
| IsLock | bool | 是否触发锁定 |
| Enable | bool | 是否启用 |
| CreateTime | DateTime | 创建时间 |

### PackingDetailInfo

轻量级明细实体，用于旧版方法兼容。

| 属性 | 类型 | 说明 |
|------|------|------|
| materialBarCode | string | 物料条码 |
| materialNo | string | 物料号 |
| materialName | string | 物料名称 |
| createTime | string | 创建时间 |
| createName | string | 创建人 |

---

## 接口层（XHS.IDAL）

### IPackingRecord

装箱记录数据访问接口，定义 12 个方法：

| 方法 | 返回类型 | 说明 |
|------|----------|------|
| `GetExPackingRecords(supplyBatchNo, partNo)` | `List<PackingRecordInfo>` | 传统查询 |
| `InsertPackingRecord(List<PackingRecordInfo>)` | `ParamterInfo` | 传统批量插入 |
| `UpdatePackingRecord(List<PackingRecordInfo>)` | `ParamterInfo` | 传统批量更新 |
| `GetPackingRecordForUpdate(packingId, conn, trans)` | `PackingRecordInfo` | 事务内 UPDLOCK 读取 |
| `GetPackingRecordById(packingId, conn, trans)` | `PackingRecordInfo` | 事务内共享锁读取 |
| `GetPackingRecordByTaskId(taskId, conn, trans)` | `PackingRecordInfo` | 按 TaskId 查询 |
| `GetPackingRecordByKdQRCode(kdQRCode, conn, trans)` | `PackingRecordInfo` | 按 KD 二维码查询 |
| `InsertPackingRecord(info, conn, trans)` | `long` | 事务内插入，返回新 ID |
| `UpdatePackingRecordScan(packingId, pageToken, newQty, newToken, userName, conn, trans)` | `bool` | 事务内更新扫描数量 |
| `LockPackingRecord(packingId, pageToken, newToken, userName, machineId, conn, trans)` | `bool` | 事务内锁定 |
| `CompletePackingRecord(packingId, pageToken, newToken, userName, conn, trans)` | `bool` | 事务内完成装箱 |
| `UnlockPackingRecord(packingId, exceptionLockToken, newToken, conn, trans)` | `bool` | 事务内解锁 |
| `UpdatePackingQRCode(packingId, pageToken, qrCode, newToken, userName, conn, trans)` | `bool` | 事务内更新随箱码 |

### IPackingScanRecord

| 方法 | 返回类型 | 说明 |
|------|----------|------|
| `GetExPackingScanRecords(...)` | `List<PackingScanRecordInfo>` | 传统关联查询 |
| `GetExPackingScanRecordsByQRCodes(...)` | `List<PackingScanRecordInfo>` | 按二维码组合查询 |
| `InsertScanRecord(...)` | `bool` | 事务内插入扫描记录 |
| `GetScannedMaterialQty(packingId, conn, trans)` | `int` | 事务内汇总已扫描数量 |
| `CheckDuplicateQRCode(packingId, qrCode, conn, trans)` | `bool` | 事务内查重 |
| `CheckPackingQRCodeUsedByOther(packingId, qrCode, conn, trans)` | `bool` | 事务内检查随箱码占用 |
| `GetScanRecordsByPackingId(packingId, conn, trans)` | `List<PackingScanRecordInfo>` | 事务内查询明细 |

### IPackingException

| 方法 | 返回类型 | 说明 |
|------|----------|------|
| `GetExPackingExceptions(boxCode)` | `List<PackingExceptionInfo>` | 传统按箱号查询 |
| `SearchPackingExceptions(kdCode, status)` | `List<PackingExceptionInfo>` | 高级搜索（JOIN KDQRCode） |
| `GetPackingExceptionByLockToken(lockToken)` | `List<PackingExceptionInfo>` | 按 LockToken 查询 |
| `GetPendingException(packingId, conn, trans)` | `PackingExceptionInfo` | 事务内查询待审核异常 |
| `GetPendingExceptionByPackingId(packingId, conn, trans)` | `PackingExceptionInfo` | 只读查询待审核异常 |
| `InsertException(info, conn, trans)` | `bool` | 事务内插入异常 |
| `AuditException(id, user, remark, status, conn, trans)` | `bool` | 事务内更新审核状态 |

### IPackingExceptionType

| 方法 | 返回类型 | 说明 |
|------|----------|------|
| `GetExPackingExceptionTypes()` | `List<PackingExceptionTypeInfo>` | 查询全部异常类型 |

---

## 数据访问实现层（XHS.MSSQL）

### 事务内 SQL 实现要点

所有核心操作均在 `SqlConnection` + `SqlTransaction` 内执行，使用 `Microsoft.ApplicationBlocks.Data.SqlHelper` 进行参数化查询。

#### UPDLOCK 并发控制
```sql
-- GetPackingRecordForUpdate 使用 UPDLOCK + ROWLOCK
SELECT ... FROM tb_PackingRecord WITH (UPDLOCK, ROWLOCK) WHERE Id=@PackingId
```

#### Token CAS（Compare-And-Swap）更新
所有状态变更 SQL 均以 `LockToken=@PageToken` 作为 WHERE 条件，确保只有持有最新 Token 的页面才能执行操作：

```sql
-- 更新扫描数量
UPDATE tb_PackingRecord SET PackingQty=@PackingQty, LockToken=@NewToken, ...
WHERE Id=@PackingId AND LockToken=@PageToken AND ExceptionStatus=0 AND Status=0

-- 锁定记录
UPDATE tb_PackingRecord SET ExceptionStatus=1, LockToken=@NewToken, ...
WHERE Id=@PackingId AND LockToken=@PageToken AND ExceptionStatus=0 AND Status=0

-- 完成装箱
UPDATE tb_PackingRecord SET Status=1, PackingUser=@UserName, ...
WHERE Id=@PackingId AND LockToken=@PageToken AND ExceptionStatus=0 AND Status=0

-- 解锁记录（审核通过）
UPDATE tb_PackingRecord SET ExceptionStatus=0, LockToken=@NewToken, ...
WHERE Id=@PackingId AND ExceptionStatus=1 AND LockToken=@ExceptionLockToken
```

每条 UPDATE 受影响行数不等于 1 即判定失败。

---

## 工厂层（XHS.DALFactory）

四个工厂类通过静态方法 `Create()` 返回对应接口的 MSSQL 实现实例：

```csharp
// PackingRecord.cs
public static IPackingRecord Create() => new XHS.MSSQL.PackingRecord();

// PackingScanRecord.cs
public static IPackingScanRecord Create() => new XHS.MSSQL.PackingScanRecord();

// PackingException.cs
public static IPackingException Create() => new XHS.MSSQL.PackingException();

// PackingExceptionType.cs
public static IPackingExceptionType Create() => new XHS.MSSQL.PackingExceptionType();
```

---

## 业务逻辑层（BLL）

### PackingOperationService（核心事务服务）

`XHS.BLL.PackingOperationService` 是所有装箱操作的事务协调中心，构造函数注入三个 DAL 接口：

```csharp
packingRecordDal = XHS.DALFactory.PackingRecord.Create();
packingScanRecordDal = XHS.DALFactory.PackingScanRecord.Create();
packingExceptionDal = XHS.DALFactory.PackingException.Create();
```

#### 公共方法

| 方法 | 说明 | 事务 |
|------|------|------|
| `OpenOrCreateByKd(...)` | 扫描 KD 标签，查找已有或创建新任务 | ✅ |
| `ScanPackingQRCode(...)` | 扫描随箱码 | ✅ |
| `ScanMaterialQRCode(...)` | 扫描零件标签 | ✅ |
| `CompletePacking(...)` | 完成装箱 | ✅ |
| `LockForException(...)` | 异常锁定（外部调用入口） | ✅ |
| `ReviewException(...)` | 审核异常 | ✅ |
| `ReviewAndUnlock(...)` | 审核并解锁 | ✅ |
| `GetPackingState(long)` | 按 ID 获取装箱状态 | ❌ 只读 |
| `GetPackingState(Guid)` | 按 TaskId 获取装箱状态 | ❌ 只读 |
| `GetScanRecordsByPackingId(long)` | 获取扫描明细列表 | ❌ 只读 |

#### 异常锁定流程（LockForExceptionInternal）

```
1. UPDLOCK 读取装箱记录
2. 验证 Token 一致性
3. 检查是否已完成
4. 检查是否已锁定（防重入）
5. 检查是否存在待审核异常（防御性检查）
6. 生成新 Token
7. 更新装箱记录：ExceptionStatus=1 + 新 Token
8. 插入异常记录：Status=0（待审核）
```

#### 审核解锁流程（ReviewException）

```
1. 按 PackingId 查询待审核异常（UPDLOCK）
2. 验证异常存在且未处理
3. 验证装箱 LockToken == 异常 LockToken
4. 更新异常审核状态
5. 若通过(newStatus=1)：解锁装箱记录（ExceptionStatus=0 + 新 Token）
6. 若暂停(0)/驳回(2)：保持锁定
```

### PackingOperationResult（操作结果类）

统一的操作结果对象，包含五个工厂方法：

| 工厂方法 | Status | Success | 说明 |
|----------|--------|---------|------|
| `Ok()` | NORMAL | true | 操作成功 |
| `Completed()` | COMPLETED | true | 装箱已完成 |
| `Lock()` | LOCK | false | 异常锁定 |
| `Stale()` | STALE | false | Token 过期 |
| `Error()` | ERROR | false | 一般错误 |

属性：`Success`、`Status`、`Message`、`NewToken`、`PackingRecord`

### PackingRecord（传统 BLL）

`XHS.BLL.PackingRecord` — 遵循传统 BLL 模式，通过 `Common.Save(source)` 执行批量操作：
- `GetExPackingRecords(supplyBatchNo, partNo)` — 查询装箱记录
- `InsertPackingRecord(List)` — 批量插入
- `UpdatePackingRecord(List)` — 批量更新

### PackingScanRecord（传统 BLL）

`XHS.BLL.PackingScanRecord`：
- `GetExPackingScanRecords(supplyBatchNo, partNo, cartonNo)` — 关联查询扫描记录
- `GetExPackingScanRecordsByQRCodes(kd, packing, material)` — 按二维码组合查询
- `InsertPackingScanRecord(List)` / `UpdatePackingScanRecord(List)` — 批量增改

### PackingException（传统 BLL）

`XHS.BLL.PackingException`（与 `BLL.PackingException` 不同，此为 XHS.BLL 命名空间）：
- `GetExPackingExceptions(boxCode)` — 按箱号查询异常
- `SearchPackingExceptions(kdCode, status)` — 高级搜索，自动解析 KD 标签中的零件编号
- `GetPackingExceptionByLockToken(lockToken)` — 按令牌查询
- `InsertPackingException(List)` / `UpdatePackingException(List)` — 批量增改

### PackingExceptionType

`BLL.PackingExceptionType`（BLL 命名空间）：
- `GetExPackingExceptionTypes()` — 获取所有异常类型

---

## Token 机制详解

### 什么是 Token

Token 是一个 GUID 去掉连字符的 32 位字符串（例：`9F6E8C3A5D2E4A9CB812`），由服务端 `PackingOperationService.GenerateToken()` 生成。

### Token 生命周期

```
KD 扫描创建任务 → 生成 Token1
     │
扫描随箱码/零件 → Token1 验证通过 → 生成 Token2（页面拿到新 Token）
     │
异常触发 → TokenN 写入 tb_PackingRecord.LockToken
     │             同时写入 tb_PackingException.LockToken
     │
审核通过 → 验证异常 LockToken == 装箱 LockToken → 生成新 Token
```

### Token 防护的并发场景

1. **F5 刷新绕过**：刷新后页面从数据库重新加载 Token，不会因持有旧 Token 绕过
2. **多浏览器窗口**：各窗口持有不同 Token，只有最后一次操作生成的 Token 有效
3. **旧页面提交**：Token 不一致 → `Stale` 结果 → 操作被拒绝
4. **异常锁定后操作**：锁定后生成新 Token，旧 Token 立即失效

---

## SQL 迁移脚本

### alter_tb_PackingException.sql（v1）

使用 `sys.columns` 系统视图检查字段/约束/索引是否存在，不存在则新增：
- 新增字段：`PackingId`（bigint）、`CartonNo`（nvarchar(50)）、`TriggerQRCode`（nvarchar(200)）
- 外键：`FK_tb_PackingException_tb_PackingRecord`
- 复合索引：`IX_tb_PackingException_PackingId_Status`
- 过滤唯一索引：`IX_tb_PackingException_UniquePending`

### alter_tb_PackingException_v2.sql（v2）

同上，但使用 `COL_LENGTH()` 函数检查列，并显式设置 `SET QUOTED_IDENTIFIER ON` 以支持过滤索引创建。更适合 `sqlcmd` 方式执行。

执行方式：
```bash
sqlcmd -S . -d XHS -U sa -P MES2016mj -C -i alter_tb_PackingException_v2.sql
```

---

## 文件清单

### 页面层（XHSWorkFlow）

| 文件 | 行数 | 说明 |
|------|------|------|
| `Packing.aspx` | 57 | 前端页面，包含扫描区、GridView、锁定遮罩、JS 逻辑 |
| `Packing.aspx.cs` | 376 | 后端代码，处理扫描事件、状态绑定、Token 管理 |
| `Packing.aspx.designer.cs` | — | 设计器自动生成 |

### 业务逻辑层（BLL）

| 文件 | 说明 |
|------|------|
| `BLL/PackingOperationService.cs` | 核心事务服务（993 行），所有装箱操作的事务协调 |
| `BLL/PackingOperationResult.cs` | 统一操作结果类 |
| `BLL/PackingRecord.cs` | 传统 BLL 装箱记录 |
| `BLL/PackingScanRecord.cs` | 传统 BLL 扫描记录 |
| `BLL/PackingException.cs` | 传统 BLL 异常记录（XHS.BLL 命名空间） |
| `BLL/PackingExceptionType.cs` | 传统 BLL 异常类型（BLL 命名空间） |

### 模型层（Model）

| 文件 | 说明 |
|------|------|
| `Model/PackingRecordInfo.cs` | 装箱记录实体（22 属性） |
| `Model/PackingScanRecordInfo.cs` | 扫描记录实体（7 属性） |
| `Model/PackingScanRecordQRCodeTypeInfo.cs` | 二维码类型常量 |
| `Model/PackingExceptionInfo.cs` | 异常记录实体（16 属性） |
| `Model/PackingExceptionTypeInfo.cs` | 异常类型实体（5 属性） |
| `Model/PackingDetailInfo.cs` | 旧版兼容实体（5 属性） |

### 接口层（XHS.IDAL）

| 文件 | 方法数 | 说明 |
|------|--------|------|
| `XHS.IDAL/IPackingRecord.cs` | 13 | 装箱记录接口 |
| `XHS.IDAL/IPackingScanRecord.cs` | 9 | 扫描记录接口 |
| `XHS.IDAL/IPackingException.cs` | 9 | 异常记录接口 |
| `XHS.IDAL/IPackingExceptionType.cs` | 1 | 异常类型接口 |

### 实现层（XHS.MSSQL）

| 文件 | 说明 |
|------|------|
| `XHS.MSSQL/PackingRecord.cs` | SQL Server 装箱记录实现 |
| `XHS.MSSQL/PackingScanRecord.cs` | SQL Server 扫描记录实现 |
| `XHS.MSSQL/PackingException.cs` | SQL Server 异常记录实现 |
| `XHS.MSSQL/PackingExceptionType.cs` | SQL Server 异常类型实现 |

### 工厂层（XHS.DALFactory）

| 文件 | 说明 |
|------|------|
| `XHS.DALFactory/PackingRecord.cs` | 装箱记录工厂 |
| `XHS.DALFactory/PackingScanRecord.cs` | 扫描记录工厂 |
| `XHS.DALFactory/PackingException.cs` | 异常记录工厂 |
| `XHS.DALFactory/PackingExceptionType.cs` | 异常类型工厂 |

### SQL 脚本（Packing 模块）

| 文件 | 说明 |
|------|------|
| `.tmp/alter_tb_PackingException.sql` | 异常表增量迁移 v1 |
| `.tmp/alter_tb_PackingException_v2.sql` | 异常表增量迁移 v2（sqlcmd 优化版） |

### 条码解析引擎 — 模型层（Model）

> Packing 模块依赖 LabelCodeRule 系统进行 KD 标签和零件标签的二维码解析。实际使用的只有 `LabelCodeRuleInfo`（MatchRegex），字段映射使用手動硬編碼而非資料庫驅動。

| 文件 | 说明 |
|------|------|
| `Model/LabelCodeRuleInfo.cs` | ✅ 条码规则定义（MatchRegex、ParseType、CustomerId 等） |
| `Model/FactoryBarcodeResultInfo.cs` | ✅ 条码匹配中间结果（Success、BarcodeType、Fields） |
| `Model/FactoryBarcodeType.cs` | ✅ 条码类型枚举（Unknown/WorkOrder/Package） |
| `Model/PartInfo.cs` | ✅ 厂内条码解析输出实体（LabelBinding 使用） |
| `Model/ShippingGoodsInfo.cs` | ✅ 出货标签解析输出实体（PartNo、SupplyBatchNo、CartonNo、Quantity） |
| `Model/LabelCodeRuleFieldInfo.cs` | ❌ 未使用 — 字段映射表實體，`BuildPartInfo()` 引用但該方法從未被呼叫 |

### 条码解析引擎 — 业务逻辑层（BLL）

| 文件 | 说明 |
|------|------|
| `BLL/LabelCodeRule.cs` | ✅ `GetLabelCodeRulesByCustomerId` — 提供 MatchRegex |
| `BLL/FactoryBarcodeParser.cs` | ✅ `ParseFactoryBarcode()` — Regex 規則匹配後手動提取命名群組 |
| `BLL/RegexFieldParser.cs` | ✅ Regex 命名捕获组提取工具 |
| `BLL/QRCode.cs` | ✅ `ParseShippingGoodsInfo()` — 寫死的 KEY_VALUE 格式（PackingOperationService 使用） |
| `BLL/LabelCodeRuleField.cs` | ❌ 未使用 — 無任何外部呼叫者 |

### 条码解析引擎 — 接口层（XHS.IDAL）

| 文件 | 说明 |
|------|------|
| `XHS.IDAL/ILabelCodeRule.cs` | ✅ 条码规则数据访问接口 |
| `XHS.IDAL/ILabelCodeRuleField.cs` | ❌ 未使用 |

### 条码解析引擎 — 实现层（XHS.MSSQL）

| 文件 | 说明 |
|------|------|
| `XHS.MSSQL/LabelCodeRule.cs` | ✅ SQL Server 规则实现（表 `tb_LabelCodeRule`） |
| `XHS.MSSQL/LabelCodeRuleField.cs` | ❌ 未使用（表 `tb_LabelCodeRuleField`，資料庫有建但目前代碼不走） |

### 条码解析引擎 — 工厂层（XHS.DALFactory）

| 文件 | 说明 |
|------|------|
| `XHS.DALFactory/LabelCodeRule.cs` | ✅ 规则工厂 |
| `XHS.DALFactory/LabelCodeRuleField.cs` | ❌ 未使用 |

### SQL 脚本（LabelCodeRule 建表）

| 文件 | 说明 |
|------|------|
| `Label_Parser_Init_SQLServer_MD.sql` | 解析引擎建表脚本（tb_FieldType、tb_BatchRule、tb_LabelCodeRule、tb_LabelCodeRuleField） |
| `绑定.sql` | MatchRegex 列增量迁移 + REGEX 解析类型支持 |

---

## 依赖关系

```
Packing.aspx.cs
  ├── PackingOperationService (XHS.BLL)
  │     ├── IPackingRecord → DALFactory.PackingRecord → MSSQL.PackingRecord
  │     ├── IPackingScanRecord → DALFactory.PackingScanRecord → MSSQL.PackingScanRecord
  │     ├── IPackingException → DALFactory.PackingException → MSSQL.PackingException
  │     └── QRCode (BLL) — KEY_VALUE 格式解析（內部使用，寫死 "10#" 等鍵值）
  ├── FactoryBarcodeParser (BLL) — KD 标签/零件标签二维码解析
  │     ├── LabelCodeRule (BLL) — 规则查询（僅取 MatchRegex）
  │     │     └── ILabelCodeRule → DALFactory.LabelCodeRule → MSSQL.LabelCodeRule
  │     ├── RegexFieldParser (BLL) — Regex 捕获组提取
  │     ├── LabelCodeRuleInfo / FactoryBarcodeResultInfo (Model)
  │     └── PartInfo / ShippingGoodsInfo (Model)
  ├── PackingOperationResult (XHS.BLL) — 操作结果
  └── PackingRecordInfo / PackingScanRecordInfo / PackingExceptionInfo (XHS.Model)

（LabelCodeRuleField 全鏈路未使用：Model/BLL/IDAL/MSSQL/DALFactory 共 5 層均無外部呼叫者）
```

### 解析调用关系

| 调用位置 | 解析器 | 方法 | 说明 |
|----------|--------|------|------|
| `Packing.aspx.cs` 第 108 行 | `FactoryBarcodeParser` | `ParseShippingGoodsBarcode(code, "XHSFZKD")` | KD 标签解析 |
| `Packing.aspx.cs` 第 171 行 | `FactoryBarcodeParser` | `ParseShippingGoodsBarcode(code, "XHSFZPart")` | 零件标签解析 |
| `PackingException.cs` 第 36 行 | `FactoryBarcodeParser` | `ParseShippingGoodsBarcode(code, "XHSFZKD")` | 异常搜索解析 |
| `PackingOperationService.cs` 第 285 行 | `QRCode` | `ParseShippingGoodsInfo(code)` | 零件扫描 KEY_VALUE 解析 |
| `LabelBinding.aspx.cs` 第 72 行 | `FactoryBarcodeParser` | `ParseFactoryBarcode(code, "JHX")` | 厂内条码解析（Regex 规则匹配） |

> ⚠️ **注意**：`FactoryBarcodeParser.ParseShippingGoodsBarcode()` 方法目前未实现。`FactoryBarcodeParser` 仅实现了 `ParseFactoryBarcode()`（返回 `PartInfo`，供 LabelBinding 使用）。Packing 页面调用的 `ParseShippingGoodsBarcode()`（应返回 `ShippingGoodsInfo`）需要补充实现，或改为调用 `QRCode.ParseShippingGoodsInfo()`。

---

## 关联文档

- `XHS_裝箱掃描流程設計.md` — 装箱扫描流程设计（繁体）
- `XHS_裝箱異常停線鎖定與網頁凍結流程.md` — 异常锁定与页面冻结流程
- `XHS_裝箱異常鎖定Token實現步驟.md` — Token 实现步骤详解
- `Label_Parser_Engine_Design.md` — 标签解析引擎设计
- `XHS_装箱TaskId_KD唯一索引增量.sql` — TaskId/KD 唯一索引增量脚本

---

> 生成时间：2026-07-29
