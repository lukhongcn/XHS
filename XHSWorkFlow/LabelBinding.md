# LabelBinding 本厂条码解码过程

## 解码流程总览

```
用户输入条码 → txt_FactoryBarcode_TextChanged
                  │
                  ▼
          FactoryBarcodeParser.ParseFactoryBarcode(rawCode, "JHX")
                  │
          ┌───────┴───────┐
          ▼               ▼
     包装码匹配        工单码匹配
   (数据库规则驱动)   (硬编码正则)
```

---

## 一、入口：`txt_FactoryBarcode_TextChanged`

**文件：** `XHSWorkFlow/LabelBinding.aspx.cs:64-89`

1. 获取并 Trim 文本框中的原始条码字符串。
2. 调用 `barcodeParser.ParseFactoryBarcode(rawCode, "JHX")` 进行解析。
3. 若返回 `null` → 显示「条码解析失败，不符合本厂条码规则」。
4. 若返回的 `PartInfo.ProcessOrderNo` 为空 → 标记为 **包装码**；否则标记为 **工单码**。
5. 将解析结果填入页面控件：
   - `txt_WorkOrderNo` ← `ProcessOrderNo`
   - `txt_FactoryPartNo` ← `JHSMaterialNo`
   - `txt_FactoryPartName` ← `MaterialName`
   - `txt_FactoryBatchNo` ← `JHSBatchNo`
6. 显示「本厂条码解析成功」。

---

## 二、核心解析器：`FactoryBarcodeParser.ParseFactoryBarcode`

**文件：** `BLL/FactoryBarcodeParser.cs:16-57`

解析分为 **两级匹配**，优先级从高到低：

### 2.1 第一级：包装码匹配（数据库规则驱动）

```
遍历数据库中 customerId="JHX" 的 LabelCodeRule 记录
   │
   ├─ 对每条规则，用其 MatchRegex 尝试匹配原始条码
   │    │
   │    └─ RegexFieldParser.TryParse(rawCode, matchRegex)
   │         │
   │         ├─ 编译正则表达式
   │         ├─ 执行 regex.Match(rawCode)
   │         ├─ 匹配成功 → 提取所有命名捕获组 → Dictionary<string,string>
   │         └─ 匹配失败 → 返回 (false, null)
   │
   ├─ 匹配成功 → 加载该规则对应的 LabelCodeRuleField 列表
   │    │
   │    └─ BuildPartInfo(rule, ruleFields, parsedValues)
   │         │
   │         ├─ 遍历每个 ruleField
   │         ├─ 用 field.KeyCode 从 parsedValues 字典中取值
   │         ├─ 若取不到值且 Required=true → 返回 null
   │         ├─ 用反射获取 PartInfo 上 field.FieldName 对应的属性类型
   │         ├─ Convert.ChangeType 转换值的类型
   │         └─ Reflector.SetProperty 设置属性值
   │
   └─ 返回构建好的 PartInfo（包装码）
```

**关键点：**
- 包装码的解析规则完全由数据库驱动，正则表达式和字段映射都可配置。
- `LabelCodeRuleInfo.MatchRegex` 定义匹配正则（含命名分组）。
- `LabelCodeRuleFieldInfo` 定义正则分组名（`KeyCode`）到 `PartInfo` 属性名（`FieldName`）的映射。
- 支持三种解析方式：`KEY_VALUE`、`POSITION`、`FIX_LENGTH`（由 `ParseType` 字段决定，但当前包装码匹配走的是正则方式，实际使用的是 `MatchRegex`）。

### 2.2 第二级：工单码匹配（硬编码正则）

仅当所有包装码规则都未命中时才尝试：

```
正则: ^(?<WorkOrderNo>\d{4}-\d{11})$
```

- 匹配格式：`4位数字 - 11位数字`（如 `1234-12345678901`）
- 匹配成功 → 创建 `PartInfo`，只设置 `ProcessOrderNo = rawCode`
- 匹配失败 → 返回 `null`

---

## 三、涉及的数据模型

| 模型类 | 作用 |
|---|---|
| `PartInfo` | 解析结果载体：`ProcessOrderNo`、`JHSMaterialNo`、`MaterialName`、`JHSBatchNo` |
| `LabelCodeRuleInfo` | 编码规则定义：`RuleId`、`CustomerId`、`MatchRegex`、`ParseType` 等 |
| `LabelCodeRuleFieldInfo` | 规则字段映射：`KeyCode`（正则组名）→ `FieldName`（PartInfo 属性名）、`Required`、`DataType` |
| `FactoryBarcodeResultInfo` | 中间匹配结果：`Success`、`BarcodeType`（Unknown/WorkOrder/Package）、`Fields` 字典 |
| `FactoryBarcodeType` | 枚举：`Unknown=0`、`WorkOrder=1`、`Package=2` |

---

## 四、数据流向图

```
原始条码字符串
      │
      ▼
RegexFieldParser.TryParse(rawCode, regex)
      │ 提取命名捕获组
      ▼
Dictionary<string, string>  (KeyCode → 捕获值)
      │
      ▼
BuildPartInfo: 遍历 LabelCodeRuleField 列表
      │ KeyCode → 查字典取值 → 类型转换 → 反射设属性
      ▼
PartInfo 对象
      │ .ProcessOrderNo
      │ .JHSMaterialNo
      │ .MaterialName
      │ .JHSBatchNo
      ▼
页面控件赋值
```

---

## 五、总结

整个解码过程的核心思路是 **可配置的正则驱动解析**：

1. 包装码通过数据库表 `tb_LabelCodeRule` + `tb_LabelCodeRuleField` 实现规则与字段映射的完全可配置化，不同的客户（`CustomerId`）可以有不同的解析规则。
2. 工单码作为一种简单的兜底方案，使用硬编码的正则 `^\d{4}-\d{11}$` 直接匹配。
3. 解析结果通过反射动态映射到 `PartInfo` 对象的对应属性上，实现了规则字段到业务模型的解耦。
