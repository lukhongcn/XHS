# LabelBindingPDA 改造计划

## 当前目标

将 `LabelBindingPDA.aspx` 改为由数据库流程驱动：

1. 使用 `tb_ScanFlow` 找到“光束标签绑定流程”。
2. 使用 `tb_ScanFlowStep` 决定扫描顺序。
3. 使用 `tb_LabelCodeRule` 解析条码。
4. 工单与本厂标签作为一个重复组，每组各扫描一次，完成一组后才能开始下一组。
5. PDA 顶部扫码 TextBox 位置固定，所有步骤共用同一个 TextBox；只切换 Label 和下方显示内容。

## 已完成的数据库配置

### tb_ScanFlow

已在本地和 `192.168.102.25` 建立：

```text
FlowId     = 1
FlowCode   = LABEL_BINDING
FlowName   = 光束标签绑定流程
CustomerId = FZXHS
Enabled    = 1
```

`tb_ScanFlowStep.FlowId` 已建立外键关联到 `tb_ScanFlow.FlowId`。

### tb_ScanFlowStep

已增加字段：

```text
RepeatScope
RepeatGroupCode
MaxScanPerCycle
```

当前流程：

| StepNo | StepCode | LabelType | RepeatScope | RepeatGroupCode | MaxScanPerCycle |
|---:|---|---|---|---|---:|
| 1 | CUSTOMER | GSCustomer | NONE | NULL | NULL |
| 2 | WORKORDER | GSWorkOrder | GROUP | WORKORDER_FACTORY | 1 |
| 3 | FACTORY | GSFactory | GROUP | WORKORDER_FACTORY | 1 |

结束条件仍为：

```text
EndControlId        = hidBoundQty
EndCompareControlId = hidCustomerQty
```

### tb_LabelCodeRule

已配置：

```text
Customer                     / FZXHS / GSCustomer
光束－上方工单码              / FZXHS / GSWorkOrder
光束－本厂标签－YYMMDD        / FZXHS / GSFactory
```

## 页面显示设计

顶部扫码输入框固定不动，例如：

```text
txtBarcode
```

所有步骤都使用同一个输入框，扫描完成后清空并重新 Focus，不切换 TextBox。

只根据当前步骤切换：

```text
当前步骤 Label
扫描提示 Label
下方内容 Panel / MultiView
```

建议在 `tb_ScanFlowStep` 增加：

```sql
ViewCode varchar(50)
DisplayTitle nvarchar(100)
PromptText nvarchar(200)
```

配置示例：

| StepCode | ViewCode | DisplayTitle | PromptText |
|---|---|---|---|
| CUSTOMER | CUSTOMER | 客户标签 | 请扫描客户标签 |
| WORKORDER | WORKORDER | 上方工单码 | 请扫描上方工单码 |
| FACTORY | FACTORY | 本厂标签 | 请扫描本厂标签 |

页面下方建议使用三个 Panel：

```text
pnlCustomer
pnlWorkOrder
pnlFactory
```

当前步骤变化时只显示对应 Panel，顶部扫码框不移动。

## 明天的开发顺序

1. 给 `tb_ScanFlowStep` 增加 `ViewCode`、`DisplayTitle`、`PromptText`。
2. 更新 Model、MSSQL、BLL，使步骤查询包含这些字段。
3. 修改 `LabelBindingPDA.aspx`，增加固定扫码框、当前步骤 Label 及三个内容 Panel。
4. 修改 `LabelBindingPDA.aspx.cs`：
   - 按 `FlowName = N'光束标签绑定流程'` 查询流程；
   - 按 `StepNo` 读取步骤；
   - 根据 `RuleName + CustomerId + LabelType` 读取解析规则；
   - 解析完成后切换下一步骤；
   - 每次 PostBack 后恢复 `txtBarcode.Focus()`。
5. 增加重复组状态控制：
   - `WORKORDER_FACTORY` 每组工单一次、本厂标签一次；
   - 两步完成后开始下一组；
   - 单独重复扫描同一步骤时提示错误。
6. 使用以下数据进行测试：

```text
客户标签：P2026061705592/CAQPL/5401113XNY02A03&120&EA&260602
上方工单：5104-20260702020
本厂标签：26J019|260602|120
```

## 重要注意

- 不要再把 `FlowId = 1` 写死在页面代码中。
- 顶部扫码 TextBox 不能随着步骤切换位置。
- 页面只切换 Label 和下方内容区域。
- `update.sql` 已保存目前数据库结构及流程配置。
