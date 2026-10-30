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
            StatusName = "等待随箱卡"
        };

        public static readonly PackingStageInfo 已完成 = new PackingStageInfo
        {
            Status = "Completed",
            StatusName = "已完成"
        };

        public static readonly PackingStageInfo 上传完成 = new PackingStageInfo
        {
            Status = "Uploaded",
            StatusName = "上传完成"
        };

        public static readonly PackingStageInfo 已解除绑定 = new PackingStageInfo
        {
            Status = "Unbound",
            StatusName = "已解除绑定"
        };

        public static readonly PackingStageInfo 未装箱 = new PackingStageInfo
        {
            Status = "Unpacked",
            StatusName = "未装箱"
        };

        /// <summary>按 Status 查找对应的中文名称。</summary>
        public static string GetStatusName(string status)
        {
            if (status == 装箱中.Status) return 装箱中.StatusName;
            if (status == 装箱完成.Status) return 装箱完成.StatusName;
            if (status == 已完成.Status) return 已完成.StatusName;
            if (status == 上传完成.Status) return 上传完成.StatusName;
            if (status == 已解除绑定.Status) return 已解除绑定.StatusName;
            if (status == 未装箱.Status) return 未装箱.StatusName;
            return string.Empty;
        }
    }
}
