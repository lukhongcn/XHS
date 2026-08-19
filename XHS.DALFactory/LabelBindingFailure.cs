using XHS.IDAL;

namespace XHS.DALFactory
{
    public static class LabelBindingFailure
    {
        public static ILabelBindingFailure Create()
        {
            return new XHS.MSSQL.LabelBindingFailure();
        }
    }
}
