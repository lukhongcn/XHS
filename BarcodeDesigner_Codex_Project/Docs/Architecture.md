
# Architecture Design

## Overall

BarcodeDesigner

```
JSON Rule
    |
    v
Rule Loader
    |
    v
Dynamic UI Builder
    |
    v
Barcode Builder
    |
    v
Formatter Provider
```


## Formatter

Interface:

```csharp
public interface ICodeFormatter
{
    string Name {get;}
    string Encode(object value);
    object Decode(string code);
}
```


## Date Provider

Examples:

- XinWangDaDate
- YYYYMMDD

