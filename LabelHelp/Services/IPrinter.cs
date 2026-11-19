using XHS.Model.ZPLLabel;

namespace LabelHelp.Services
{
    public interface IPrinter
    {
        PrintResult Print(string content);
    }
}
