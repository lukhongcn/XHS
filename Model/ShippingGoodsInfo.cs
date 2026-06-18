using System;

namespace XHS.Model
{
    /// <summary>
    /// 出货货品信息。
    /// </summary>
    public class ShippingGoodsInfo
    {
        /// <summary>
        /// 主键标识。
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 供应商代码。
        /// </summary>
        public string SupplierCode { get; set; }

        /// <summary>
        /// 零件号。
        /// </summary>
        public string PartNo { get; set; }

        /// <summary>
        /// 零件中文名称。
        /// </summary>
        public string PartChineseName { get; set; }

        /// <summary>
        /// 零件英文名称。
        /// </summary>
        public string PartEnglishName { get; set; }

        /// <summary>
        /// 数量。
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// 供货批次号。
        /// </summary>
        public string SupplyBatchNo { get; set; }

        /// <summary>
        /// 码放层数。
        /// </summary>
        public int? StackLayerCount { get; set; }

        /// <summary>
        /// 生产日期。
        /// </summary>
        public DateTime? ProductionDate { get; set; }

        /// <summary>
        /// 检验确认日期。
        /// </summary>
        public DateTime? InspectionConfirmDate { get; set; }

        /// <summary>
        /// 纸箱编号。
        /// </summary>
        public string CartonNo { get; set; }

        /// <summary>
        /// 单箱毛重，单位：g。
        /// </summary>
        public decimal? SingleBoxGrossWeight { get; set; }

        /// <summary>
        /// 标签二维码或唯一识别码。
        /// </summary>
        public string QrCode { get; set; }

        /// <summary>
        /// 外箱二维码原文。
        /// </summary>
        public string OutBoxQRCode { get; set; }

        /// <summary>
        /// 当前状态。
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 打印次数。
        /// </summary>
        public int? PrintCount { get; set; }

        /// <summary>
        /// 创建人。
        /// </summary>
        public string Creater { get; set; }

        /// <summary>
        /// 创建日期。
        /// </summary>
        public DateTime? CreatDate { get; set; }
    }
}

