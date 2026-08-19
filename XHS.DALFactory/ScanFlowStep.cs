using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 扫描流程步骤数据访问实例工厂。
    /// </summary>
    public class ScanFlowStep
    {
        public static IScanFlowStep Create()
        {
            return new XHS.MSSQL.ScanFlowStep();
        }
    }
}
