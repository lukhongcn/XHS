using System.Collections.Generic;
using XHS.Model;

namespace XHS.IDAL
{
    /// <summary>
    /// 出货货品数据访问接口。
    /// </summary>
    public interface IShippingGoods
    {
        List<ShippingGoodsInfo> GetShippingGoods();

        List<ShippingGoodsInfo> GetShippingGoods(string partNo, string partName, string supplyBatchNo);

        List<ShippingGoodsInfo> GetShippingGoodsBySupplyBatchNo(string supplyBatchNo, string cartonNo);

        List<ShippingGoodsInfo> GetShippingGoodsByBusinessKey(string supplyBatchNo, string partNo, string cartonNo);

        ParamterInfo InsertShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos);

        ParamterInfo UpdateShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos);

        ParamterInfo DeleteShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos);

        ParamterInfo DeleteShippingGoodsByBusinessKey(List<ShippingGoodsInfo> shippingGoodsInfos);
    }
}
