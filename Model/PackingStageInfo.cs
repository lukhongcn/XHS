namespace XHS.Model
{
    /// <summary>
    /// 装箱阶段信息（Status 存数据库，StatusName 用于页面显示）。
    /// </summary>
    public class PackingStageInfo
    {
        public string Status { get; set; }
        public string StatusName { get; set; }

        public static readonly PackingStageInfo 装箱中 = new PackingStageInfo
        {
            Status = "Scanning",
            StatusName = "装箱中"
        };

        public static readonly PackingStageInfo 装箱完成 = new PackingStageInfo
        {
            Status = "PackingComplete",
            StatusName = "装箱完成"
        };

        public static readonly PackingStageInfo 已完成 = new PackingStageInfo
        {
            Status = "Completed",
            StatusName = "已完成"
        };

        /// <summary>按 Status 查找对应的中文名称。</summary>
        public static string GetStatusName(string status)
        {
            if (status == 装箱中.Status) return 装箱中.StatusName;
            if (status == 装箱完成.Status) return 装箱完成.StatusName;
            if (status == 已完成.Status) return 已完成.StatusName;
            return string.Empty;
        }
    }
}
