using ZebraPrintTest.Models;

namespace ZebraPrintTest.Printer
{
    public interface IPrinter
    {
        PrintResult Print(string content);
    }
}
