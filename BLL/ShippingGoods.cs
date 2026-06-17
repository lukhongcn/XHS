using System.Collections.Generic;
using XHS.Model;
using ModuleWorkFlow.business;
using XHS.IDAL;
using System.Collections;

namespace BLL
{
    /// <summary>
    /// 出货货品业务层。
    /// </summary>
    public class ShippingGoods
    {
        private readonly IShippingGoods dal;

        public ShippingGoods()
        {
            dal = XHS.DALFactory.ShippingGoods.Create();
        }

        public List<ShippingGoodsInfo> GetShippingGoods()
        {
            return dal.GetShippingGoods();
        }

        public List<ShippingGoodsInfo> GetShippingGoods(string partNo, string partName, string supplyBatchNo)
        {
            return dal.GetShippingGoods(partNo, partName, supplyBatchNo);
        }

        public List<ShippingGoodsInfo> GetShippingGoodsBySupplyBatchNo(string supplyBatchNo, string cartonNo)
        {
            return dal.GetShippingGoodsBySupplyBatchNo(supplyBatchNo, cartonNo);
        }

        public string InsertShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            NormalizeShippingGoods(shippingGoodsInfos);
            ParamterInfo paramterInfo = dal.InsertShippingGoods(shippingGoodsInfos);
            IList source = new ArrayList();
            source.Add(paramterInfo);

            return Common.Save(source) ? string.Empty : "保存失败。";
        }

        public ParamterInfo UpdateShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            NormalizeShippingGoods(shippingGoodsInfos);
            return dal.UpdateShippingGoods(shippingGoodsInfos);
        }

        public ParamterInfo DeleteShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            return dal.DeleteShippingGoods(shippingGoodsInfos);
        }

        private static void NormalizeShippingGoods(List<ShippingGoodsInfo> shippingGoodsInfos)
        {
            if (shippingGoodsInfos == null)
            {
                return;
            }

            foreach (ShippingGoodsInfo shippingGoodsInfo in shippingGoodsInfos)
            {
                if (shippingGoodsInfo == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(shippingGoodsInfo.Status))
                {
                    shippingGoodsInfo.Status = ShippingGoodsStatusInfo.UnPrinted;
                }

                if (!shippingGoodsInfo.PrintCount.HasValue)
                {
                    shippingGoodsInfo.PrintCount = 0;
                }
            }
        }
    }
}
