using System.Collections.Generic;
using Model;

namespace BLL
{
    /// <summary>
    /// 出货货品业务层。
    /// </summary>
    public class ShippingGoods
    {
        public List<ShippingGoodsInfo> GetShippingGoods()
        {
            return XHS.DALFactory.ShippingGoods.Create().GetShippingGoods();
        }

        public List<ShippingGoodsInfo> GetShippingGoodsBySupplyBatchNo(string supplyBatchNo)
        {
            return XHS.DALFactory.ShippingGoods.Create().GetShippingGoodsBySupplyBatchNo(supplyBatchNo);
        }

        public bool InsertShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            return XHS.DALFactory.ShippingGoods.Create().InsertShippingGoods(shippingGoodsInfos);
        }

        public bool UpdateShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            return XHS.DALFactory.ShippingGoods.Create().UpdateShippingGoods(shippingGoodsInfos);
        }

        public bool DeleteShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            return XHS.DALFactory.ShippingGoods.Create().DeleteShippingGoods(shippingGoodsInfos);
        }
    }
}
