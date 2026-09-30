
# BarcodeDesigner Codex Task

## 项目目标

开发一个基于 JSON 配置驱动的 Windows Application。

技术要求：

- C#
- .NET Framework 4.8
- Windows Forms

目标：

根据 JSON 自动生成输入界面，并生成汽车零件追溯条码。

---

## 必须读取配置

Config:

- XinWangDaBattery_UI_Schema.json
- XinWangDaDate.json


---

## 开发原则

禁止针对单一客户硬编码。

禁止：

```csharp
if(customer=="XinWangDa")
{
}
```

所有规则必须通过：

- JSON
- Formatter Provider

实现。


---

## 开发流程

严格按照：

1. 需求分析
2. 架构设计
3. Model设计
4. 编码
5. 单元测试

执行。

不要一次性生成全部代码。


---

## 核心模块

需要实现：

- JSON Rule Loader
- Dynamic WinForms UI Builder
- Barcode Builder
- Formatter Factory
- Date Provider


---

## 扩展目标

未来支持：

- Barcode Parser
- 多供应商规则
- 标签打印
- MES追溯

