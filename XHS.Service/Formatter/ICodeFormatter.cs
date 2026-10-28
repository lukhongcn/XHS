namespace XHS.service.Formatter
{
    public interface ICodeFormatter
    {
        string Name { get; }

        string Encode(object value);

        object Decode(string code);
    }
}
