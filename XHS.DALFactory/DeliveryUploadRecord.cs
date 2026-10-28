using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>配送单上传记录数据访问实例工厂。</summary>
    public class DeliveryUploadRecord
    {
        public static IDeliveryUploadRecord Create()
        {
            return new XHS.MSSQL.DeliveryUploadRecord();
        }
    }
}
