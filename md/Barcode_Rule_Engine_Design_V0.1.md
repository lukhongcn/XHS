# Barcode Rule Engine 设计说明 V0.1

## 项目目标

建立 MES 通用条码规则引擎。

目标：
- JSON 定义条码结构
- 自动生成输入页面
- 支持不同供应商日期规则
- 支持条码生成与解析

架构：

客户条码规则
↓
JSON配置
↓
输入页面
↓
BarcodeBuilder
↓
生成条码


## 核心原则

BarcodeBuilder 不关心客户规则。

禁止：

```csharp
if(customer=="XinWangDa")
{
}
```

所有变化通过 JSON 和 Provider 扩展。


## 配置结构

```
BarcodeEngine

 ├─ BarcodeRules
 │     ├─ XinWangDaBattery.json
 │     └─ CustomerA.json
 │
 ├─ DateRules
 │     ├─ XinWangDaDate.json
 │     └─ YYYYMMDD.json
 │
 └─ FormatterRegistry.json
```


## 条码规则 JSON

示例：

```json
{
  "RuleName":"XinWangDaBattery",
  "Description":"欣旺达汽车电池标签规则",
  "Length":28,
  "Fields":[
    {
      "Name":"PartNo",
      "Description":"零件号码",
      "Length":13,
      "Source":"Input"
    },
    {
      "Name":"ProductionDate",
      "Description":"生产日期",
      "Length":3,
      "Source":"Date",
      "Formatter":"XinWangDaDate"
    }
  ]
}
```


## 日期规则设计

日期规则独立：

```
DateCodeProvider

 ├─ YYYYMMDDProvider
 └─ XinWangDaDateProvider
```


接口：

```csharp
public interface IDateCodeProvider
{
    string Name { get; }

    string Encode(DateTime date);

    DateTime Decode(string code);
}
```


## 欣旺达日期规则

示例：

```
81Y = 2018-01-30
```

日期编码通过 JSON 配置：

- 年份循环
- 月份循环
- 日期字符表

不要写死代码。


## BarcodeBuilder调用流程

```
读取BarcodeRule

↓

循环Fields

↓

根据Formatter找到Provider

↓

生成字段值

↓

拼接完整条码
```


伪代码：

```csharp
foreach(var field in rule.Fields)
{
    var value = Resolve(field);

    if(field.Formatter != null)
    {
        var provider =
            factory.Get(field.Formatter);

        value =
            provider.Encode(value);
    }

    result += value;
}
```


## 自动生成输入页面

JSON中的 Description 用于页面显示。

例如：

```
零件号码:
[          ]

供应商:
[          ]

生产日期:
[2026-09-25]

版本:
[ ]

批次:
[ ]

生成条码
```


## 单元测试

验证：

```
2018-01-30 -> 81Y
```

```
G99 -> 2026-09-09
```

```
G9T -> 根据配置解析
```


## 后续扩展

支持：

- 不同汽车供应商
- 不同二维码规则
- 自动生成 MES 打印页面
- 条码解析追溯

目标：

客户提供条码规范

↓

配置 JSON

↓

自动生成维护页面

↓

自动生成和解析条码
