using System.Collections.Generic;
using System;
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

        List<ShippingGoodsInfo> GetShippingGoods(string partNo, string partName, string supplyBatchNo, string closeStatus);

        List<ShippingGoodsInfo> GetShippingGoodsBySupplyBatchNo(string supplyBatchNo, string cartonNo);

        List<ShippingGoodsInfo> GetShippingGoodsByBusinessKey(string supplyBatchNo, string partNo, string cartonNo);

        List<ShippingGoodsInfo> GetShippingGoodsByBusinessKey(string supplyBatchNo, string partNo, string cartonNo, string deliveryNo);

        string GetNextAutoDeliveryNo(DateTime date);

        ParamterInfo InsertShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos);

        ParamterInfo UpdateShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos);

        ParamterInfo UpdateShippingGoodsByBusinessKey(List<ShippingGoodsInfo> shippingGoodsInfos, string deliveryNo);

        ParamterInfo UpdatePrintedDeliveryNo(string supplyBatchNo, string partNo, string cartonNo, string oldDeliveryNo, string newDeliveryNo);

        ParamterInfo UpdateShippingGoodsClose(string supplyBatchNo, string partNo, string deliveryNo, string closer, DateTime closeDate);

        ParamterInfo DeleteShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos);

        ParamterInfo DeleteShippingGoodsByBusinessKey(List<ShippingGoodsInfo> shippingGoodsInfos, string deliveryNo);
        ParamterInfo DeleteShippingGoodsByBusinessKey(List<ShippingGoodsInfo> shippingGoodsInfos);
    }
}
