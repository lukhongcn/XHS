# PLCResult 处理改造记录

日期：2026-11-18

## 一、数据库修改

`tb_PLCResult` 新增字段：

```sql
IF COL_LENGTH('dbo.tb_PLCResult', 'deal') IS NULL
BEGIN
    ALTER TABLE dbo.tb_PLCResult
    ADD deal int NOT NULL
        CONSTRAINT DF_tb_PLCResult_deal DEFAULT (0);
END;
```

字段含义：

- `deal=0`：未处理
- `deal=1`：已处理

数据库修改已执行并确认成功。

## 二、PLCResult 处理线程

新增 `PLCResultProcessService`，S7client 启动后台线程，每 5 秒处理一条 `deal=0` 且时间最新的 PLCResult。

处理规则：

1. 根据 `ProcessNo` 找到对应的 `PartNo_Id`。
2. 根据 `PartNo_Id` 查询该零件的全部 `PartProcessInfo`。
3. 当前工序为 `Ready` 时改为 `Implementation`。
4. `result=1` 时，下一工序改为 `Ready`。
5. `result<>1` 时，原下一工序顺延，并新增一条：
   - `ProcessId=QMJCPD`
   - `ProcessOrder=当前工序+1`
   - `StatusId=Ready`
6. 调用 `ModuleWorkFlow.business.PartProcess.UpdatePartProcessFromModifyEdit(source)` 更新。
7. 处理完成后将 PLCResult 的 `deal` 更新为 `1`。

线程日志统一使用 `PLC_RESULT_PROCESS` 前缀，并显示在 S7client 下方的 PLCResult 日志区域。

## 三、查询兼容处理

按 `PartNo_Id` 查询全部工序时，`tb_process` 和 `tb_status` 已从 `INNER JOIN` 改为 `LEFT JOIN`，避免因工序定义或状态定义缺失而过滤掉 `tb_partprocess` 本身存在的记录。

## 四、扫码及扫码历史

扫码后增加 `ScanFlowScanRecord` 记录：

- `scanname=QMJC`
- `taskid=qrcode`
- `scancontent=qrcode`
- 检测通过状态：`已完成`
- 检测未通过状态：`未通过`

扫码时读取检测结果和检测数值，并写入 `tb_PLCResult`。

PLC 结果常量：

```csharp
0 = 异常
1 = 合格
2 = 不合格
```

## 五、配置及部署

S7client 配置新增：

```xml
<add key="ScanFlowCode" value="QMJC" />
<add key="ScanStepCode" value="QMJC" />
<add key="PLCResultProcessIntervalMs" value="5000" />
```

本次编译后的 DLL 已复制到：

- `S7Development/S7client/bin/Debug`
- `S7Development/S7client/bin/Release`

涉及主要程序集：

- `XHS.BLL.dll`
- `XHS.MSSQL.dll`
- `ModuleWorkFlow.Model.dll`

其中 `ModuleWorkFlow.Model.dll` 版本为 `1.0.9560.38373`，用于解决程序集加载错误。

## 六、当前测试数据

当前数据库中测试 PLCResult：

- `ProcessNo=11`
- `PartProcessId=11`
- `PartNo_Id=500405009661012613281GBH0003`
- `ModuleId=SQSD`
- `ProcessId=QMJC`
- `StatusId=Ready`

之前日志提示“未找到 PLCResult 对应的工序：11”，原因是查询工序时使用了内连接，`QMJC` 缺少对应关联记录导致被过滤。已改为左连接，明天启动程序后继续验证实际推进和 `deal=1` 更新结果。

