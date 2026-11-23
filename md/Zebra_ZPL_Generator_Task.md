# Task: Create Zebra JSON Template Based ZPL Generator

## Goal

Create a C# console test program.

The program reads:

1. Label Template JSON
2. List<string> print data

Then generates Zebra ZPL commands.

Target printer:

- Zebra ZT211CN
- ZPL language
- 203 DPI

This task only generates ZPL.

DO NOT connect printer.
DO NOT send TCP commands.

---

# Input

## Template

Read:

```
/Templates/SmallQRCode30x30.json
```

The template defines:

- label size
- object position
- QR Code settings
- text settings

Do not hardcode label size or object coordinates in C#.

---

## Print Data

The Generate method input must be:

```csharp
List<string> values
```

Example:

```csharp
List<string> values = new List<string>()
{
    "501300660292120287G9T8100471",
    "501300660292120",
    "287G9T8100471"
};
```

Mapping:

Index 0:
- QRCode value

Index 1:
- First text line

Index 2:
- Second text line

---

# JSON Mapping Rule

The JSON objects array uses fieldIndex.

Example:

```json
{
  "type":"qrcode",
  "fieldIndex":0
}
```

means:

```
values[0]
```

Example:

```json
{
  "type":"text",
  "fieldIndex":1
}
```

means:

```
values[1]
```

---

# Output Model

Create:

```csharp
public class ZplResult
{
    public bool Success { get; set; }

    public string Zpl { get; set; }

    public string ErrorMessage { get; set; }
}
```

---

# Generator

Create:

```csharp
public class ZplGenerator
{
    public ZplResult Generate(
        string templateFile,
        List<string> values)
    {

    }
}
```

The method should:

1. Load JSON template
2. Validate input data
3. Generate ZPL
4. Return ZplResult

---

# ZPL Requirements

Generated ZPL must contain:

Start:

```
^XA
```

End:

```
^XZ
```

---

# Supported Objects

First version supports:

## QR Code

JSON:

```json
{
 "type":"qrcode"
}
```

Generate:

```
^FO{x},{y}
^BQN,2,{module}
^FDQA,{value}^FS
```

---

## Text

JSON:

```json
{
 "type":"text"
}
```

Generate:

```
^FO{x},{y}
^A0N,{height},{width}
^FD{value}^FS
```

---

# Project Structure

Create:

```
ZebraLabelTest

/Templates
    SmallQRCode30x30.json

/Models
    LabelTemplate.cs
    LabelObject.cs
    ZplResult.cs

/Engine
    ZplGenerator.cs

Program.cs

output.zpl
```

---

# Console Test

Create Program.cs.

Test data:

```csharp
List<string> values = new List<string>()
{
    "501300660292120287G9T8100471",
    "501300660292120",
    "287G9T8100471"
};
```

Run:

```
Load Template Success

Generate ZPL Success

Save output.zpl Success
```

---

# Output File

Create:

```
output.zpl
```

The file should contain the generated ZPL.

The output file can later be sent directly to Zebra printer.

---

# Future Extension

Keep the design extensible.

Later support:

- barcode
- image
- line
- rectangle
- rotation
- TCP 9100 printing
- printer status checking

But this task only generates ZPL.
