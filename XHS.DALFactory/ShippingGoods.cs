using XHS.IDAL;

namespace XHS.DALFactory
{
    /// <summary>
    /// 出货货品数据访问实例工厂。
    /// </summary>
    public class ShippingGoods
    {
        public static IShippingGoods Create()
        {
            return new XHS.MSSQL.ShippingGoods();
        }
    }
}
