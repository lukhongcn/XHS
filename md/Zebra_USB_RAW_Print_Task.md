# Task: Create C# Zebra USB RAW ZPL Printer Sender

## Goal

Create a C# test program that sends an existing ZPL string to a Zebra
ZT211 printer through USB.

The Zebra Windows Driver is already installed.

Only handle:

ZPL string -\> Windows RAW Printer Queue -\> USB -\> Zebra ZT211

Do NOT generate ZPL in this task.

## Environment

-   C#
-   .NET Framework 4.7.2
-   Windows
-   Zebra ZT211
-   USB connection
-   Zebra ZPL Driver installed

## Input

The program receives a ZPL string or reads:

    output.zpl

Example:

``` csharp
string zpl = @"^XA
^FO50,50
^A0N,40,40
^FDTEST ZPL^FS
^XZ";
```

## Printer Name

Make printer name configurable:

``` csharp
string printerName = "ZDesigner ZT211-300dpi ZPL";
```

Do not hardcode it inside the printer implementation.

## Project Structure

    ZebraPrintTest

    /Printer
        IPrinter.cs
        ZebraRawPrinter.cs

    /Models
        PrintResult.cs

    Program.cs
    output.zpl

## Interface

``` csharp
public interface IPrinter
{
    PrintResult Print(string content);
}
```

## Result

``` csharp
public class PrintResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}
```

## ZebraRawPrinter

Implement Windows RAW printing using Win32 API:

-   OpenPrinter
-   ClosePrinter
-   StartDocPrinter
-   EndDocPrinter
-   StartPagePrinter
-   EndPagePrinter
-   WritePrinter

Use printer data type:

    RAW

Process:

1.  Open Windows printer
2.  Start RAW print job
3.  Send ZPL bytes
4.  Close print job
5.  Return PrintResult

## Test Program

Read:

    output.zpl

Create printer:

``` csharp
IPrinter printer =
    new ZebraRawPrinter(
        "ZDesigner ZT211-300dpi ZPL");
```

Send:

``` csharp
PrintResult result = printer.Print(zpl);
```

Console output:

    Print Success

## Test File

Example output.zpl:

``` zpl
^XA
^FO50,50
^A0N,40,40
^FDTEST ZPL^FS
^XZ
```

## Restrictions

Do NOT use:

-   System.Drawing.Printing.PrintDocument
-   PDF printing
-   Image printing
-   Zebra SDK

Use RAW ZPL printing only.

## Future

Later integrate:

MES -\> PrintService -\> ZebraRawPrinter -\> ZT211

Current goal:

C# -\> Windows Driver -\> USB -\> Zebra ZT211
