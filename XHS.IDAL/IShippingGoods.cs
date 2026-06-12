using System.Collections.Generic;
using Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 出货货品数据访问接口。
    /// </summary>
    public interface IShippingGoods
    {
        List<ShippingGoodsInfo> GetShippingGoods();

        List<ShippingGoodsInfo> GetShippingGoodsBySupplyBatchNo(string supplyBatchNo);

        bool InsertShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos);

        bool UpdateShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos);

        bool DeleteShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos);
    }
}
