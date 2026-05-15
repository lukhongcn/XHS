namespace Model.Label
{
    /// <summary>
    /// 标签数据模型。
    /// 只负责保存一张标签需要显示的数据，不负责绘图、PDF、打印。
    /// </summary>
    public class LabelInfo
    {
        public string SupplierName { get; set; }      // 供应商名称
        public string SupplierCode { get; set; }      // 供应商代码
        public string PartNo { get; set; }            // 零件号
        public string PartName { get; set; }          // 零件名称
        public string Qty { get; set; }               // 单包装数量
        public string LotNo { get; set; }             // 供货批次号
        public string LayerCount { get; set; }        // 码放层数
        public string ProduceDate { get; set; }       // 生产日期
        public string CheckDate { get; set; }         // 检验确认/日期
        public string MaterialCode { get; set; }      // 材料代码
        public string SerialNo { get; set; }          // 流水号
        public string QrContent { get; set; }         // 二维码内容
    }
}
