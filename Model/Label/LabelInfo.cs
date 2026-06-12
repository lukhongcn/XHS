namespace XHS.Model.Label
{
    /// <summary>
    /// 标签数据模型。
    /// 只负责保存一张标签需要显示的数据，不负责绘图、PDF、打印。
    /// </summary>
    public class LabelInfo
    {
        public string BaseNo { get; set; }            // 基地编号
        public string DeliveryType { get; set; }      // 平台上传类型
        public string DeliveryNo { get; set; }        // 平台配送单号
        public string SxCardSeq { get; set; }         // 平台随箱卡号
        public string PackageType { get; set; }       // 包装类型
        public string CheckTime { get; set; }         // 平台检验时间
        public string CheckUserName { get; set; }     // 平台检验人员
        public string PackingCreateTime { get; set; } // 装箱时间
        public string PackingCreateName { get; set; } // 装箱人员
        public string SupplierName { get; set; }      // 供应商名称
        public string SupplierCode { get; set; }      // 供应商代码
        public string PartNo { get; set; }            // 零件号
        public string PartName { get; set; }          // 零件名称
        public string MaterialBarCode { get; set; }   // 物料流水号/零件码
        public string Qty { get; set; }               // 单包装数量
        public string LotNo { get; set; }             // 供货批次号
        public string PackingSlipCardNo { get; set; } // 随箱卡号
        public string PackageCode { get; set; }       // 包装编号
        public string PackageName { get; set; }       // 外包装箱名
        public string LayerCount { get; set; }        // 码放层数
        public string BoxCount { get; set; }          // 箱数
        public string ProduceDate { get; set; }       // 生产日期
        public string CheckDate { get; set; }         // 到货时间
        public string CheckConfirmDate { get; set; }  // 检验确认日期
        public string MaterialCode { get; set; }      // 材料代码
        public string SerialNo { get; set; }          // 流水号
        public string QrContent { get; set; }         // 二维码内容
    }

    /// <summary>
    /// 二维码字段项。
    /// 例如：10#零件号、11#供应商代码。
    /// </summary>
    public class QRCodeInfo
    {
        public string QRNumber { get; set; }          // 二维码项号
        public string QRField { get; set; }           // 二维码字段值
    }
}
