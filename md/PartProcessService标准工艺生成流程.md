# PartProcessService：标准工艺生成零件工序流程

## 1. 功能说明

`PartProcessService` 位于：

```text
D:\project\xhs\BLL\PartProcessService.cs
```

它用于根据标准工艺，为已经生成的零件创建对应的零件工序，并写入：

```text
tb_partprocess
```

当前服务是业务整合层，不直接拼接或执行 SQL，主要调用以下已有 DLL 方法：

```text
WorkFlow.BLL.Standard.StandProcess.GetStandProcessByName
ModuleWorkFlow.BLL.PartProcess.getPartProcessInfo
ModuleWorkFlow.BLL.PartProcess.AddPartProcess
```

## 2. 整体调用链

```text
XHS 页面或后台代码
        |
        v
XHS.BLL.PartProcessService.CreateByStandard
        |
        v
StandProcess.GetStandProcessByName(标准工艺名称)
        |
        v
取得标准工艺中的工序和工时
        |
        v
按 PartCount 生成 PartNo_Id
        |
        v
组装 PartProcessInfo
        |
        v
检查工序是否已存在
        |
        v
PartProcess.AddPartProcess
        |
        v
tb_partprocess
```

## 3. 标准工艺来源

标准工艺通过 `WorkFlow.BLL.Standard.dll` 获取：

```csharp
StandProcess standProcess = new StandProcess();

IList standardProcesses =
    standProcess.GetStandProcessByName(standardProcessName);
```

返回的每个 `StandProcessInfo` 主要使用以下字段：

| 字段 | 用途 |
|---|---|
| `ProcessId` | 工序编号，写入 `tb_partprocess.ProcessId` |
| `ListOrder` | 工序顺序，写入 `ProcessOrder` |
| `ProcessNeedMinutes` | 工序需要分钟数 |
| `CustomerProcessId` | 客户工序编号，当前旧接口未写入 |

如果标准工艺不存在或没有工序，服务会抛出异常：

```text
未找到标准工艺：标准工艺名称
```

## 4. 零件工序生成规则

假设：

```text
PartNo    = A001
PartCount = 3
标准工艺包含：CNC、QC
```

则会生成：

```text
A001-1 / CNC
A001-2 / CNC
A001-3 / CNC
A001-1 / QC
A001-2 / QC
A001-3 / QC
```

生成规则如下：

```csharp
for (int i = 1; i <= partInfo.PartCount; i++)
{
    string partNoId = partInfo.PartNo + "-" + i;
}
```

第一道工序状态为 `Ready`，后续工序状态为 `Pending`：

```csharp
processInfo.StatusId =
    standardInfo.ListOrder == 1
        ? "Ready"
        : "Pending";
```

## 5. 写入字段

服务组装的 `PartProcessInfo` 字段如下：

| `PartProcessInfo` 字段 | 数据来源或默认值 |
|---|---|
| `ModuleId` | 已生成零件的模块编号 |
| `PartNo` | 零件编号 |
| `PartNo_Id` | `PartNo + "-" + 序号` |
| `ProcessId` | 标准工艺的 `ProcessId` |
| `ProcessOrder` | 标准工艺的 `ListOrder` |
| `ProcessNeedMinutes` | 标准工艺的 `ProcessNeedMinutes` |
| `StatusId` | 第一工序 `Ready`，其余 `Pending` |
| `NeedProduct` | `0` |
| `Priority` | 零件优先级，为 0 时使用 `100` |

随后调用：

```csharp
new ModuleWorkFlow.BLL.PartProcess()
    .AddPartProcess(processInfo);
```

该方法最终将数据写入 `tb_partprocess`。

## 6. XHS 中的调用方式

```csharp
using ModuleWorkFlow.Model;
using XHS.BLL;

PartInfo partInfo =
    new ModuleWorkFlow.BLL.Part()
        .getPartInfo(moduleId, partNo);

if (partInfo == null)
{
    throw new Exception("零件不存在");
}

int createdCount =
    new PartProcessService()
        .CreateByStandard(
            partInfo,
            standardProcessName);
```

`createdCount` 表示本次实际创建的工序数量：

```text
零件数量 × 标准工艺工序数量
```

## 7. 重复数据检查

服务默认会检查相同的：

```text
ModuleId + PartNo_Id + ProcessOrder
```

如果已存在，则停止生成并抛出异常，避免重复插入：

```csharp
partProcess.getPartProcessInfo(
    processInfo.ModuleId,
    processInfo.PartNo_Id,
    processInfo.ProcessOrder);
```

如果确实需要跳过重复检查，可以传入：

```csharp
new PartProcessService()
    .CreateByStandard(
        partInfo,
        standardProcessName,
        false);
```

一般不建议关闭重复检查。

## 8. 当前程序集引用

`XHS.BLL.csproj` 已引用标准工艺程序集：

```text
WorkFlow.BLL.Standard.dll
WorkFlow.DALFactory.Standard.dll
WorkFlow.IDAL.Standard.dll
WorkFlow.Model.Standard.dll
WorkFlow.MsSQL.Standard.dll
```

这些程序集来自：

```text
D:\project\new_xuxiang2018_zhengqin_twNJ\bin
```

运行 XHS 时，相关 DLL 必须能够被复制或部署到 XHS 网站的 `bin` 目录。

## 9. 注意事项

1. 必须先创建零件主数据，再调用 `CreateByStandard`。
2. `PartCount` 必须大于 0。
3. `standardProcessName` 必须是标准工艺名称，不是单个工序编号。
4. 第一工序使用 `Ready`，否则生产流程无法正常启动。
5. 当前实现逐条调用 `AddPartProcess`，不是调用空实现的 `StandProductPart.AddStandPartProcess`。
6. 当前实现发生中途异常时，已经成功写入的前面工序不会自动回滚；如果以后需要整体回滚，应增加事务接口。
7. 当前项目已完成 `XHS.BLL` 编译验证，代码编译结果为 0 个错误。
