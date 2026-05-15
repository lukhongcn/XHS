using System.Configuration;

namespace LabelHelp.Config
{
    /// <summary>
    /// 标签打印配置。
    /// 负责从 App.config / Web.config 的 appSettings 中读取尺寸、字体、路径等配置。
    /// 所有尺寸建议统一使用 mm。
    /// </summary>
    public class LabelPrintConfig
    {
        public float LabelWidth { get; set; }
        public float LabelHeight { get; set; }
        public float A4PageWidth { get; set; }
        public float A4PageHeight { get; set; }
        public float A4MarginLeft { get; set; }
        public float A4MarginTop { get; set; }
        public int A4Columns { get; set; }
        public int A4Rows { get; set; }
        public float A4ColumnGap { get; set; }
        public float A4RowGap { get; set; }
        public string FontName { get; set; }
        public float TitleFontSize { get; set; }
        public float ValueFontSize { get; set; }
        public string TemplateFolder { get; set; }
        public string OutputFolder { get; set; }

        /// <summary>
        /// 从配置文件读取配置。如果配置不存在或格式错误，使用默认值。
        /// </summary>
        public static LabelPrintConfig Load()
        {
            return new LabelPrintConfig
            {
                LabelWidth = GetFloat("Label.Width", 100f),
                LabelHeight = GetFloat("Label.Height", 60f),
                A4PageWidth = GetFloat("Label.A4.PageWidth", 210f),
                A4PageHeight = GetFloat("Label.A4.PageHeight", 297f),
                A4MarginLeft = GetFloat("Label.A4.MarginLeft", 5f),
                A4MarginTop = GetFloat("Label.A4.MarginTop", 10f),
                A4Columns = GetInt("Label.A4.Columns", 2),
                A4Rows = GetInt("Label.A4.Rows", 4),
                A4ColumnGap = GetFloat("Label.A4.ColumnGap", 0f),
                A4RowGap = GetFloat("Label.A4.RowGap", 5f),
                FontName = GetString("Label.FontName", "Microsoft YaHei"),
                TitleFontSize = GetFloat("Label.TitleFontSize", 8f),
                ValueFontSize = GetFloat("Label.ValueFontSize", 9f),
                TemplateFolder = GetString("Label.TemplateFolder", "Templates"),
                OutputFolder = GetString("Label.OutputFolder", "Output")
            };
        }

        private static string GetString(string key, string defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
        }

        private static int GetInt(string key, int defaultValue)
        {
            int result;
            return int.TryParse(ConfigurationManager.AppSettings[key], out result) ? result : defaultValue;
        }

        private static float GetFloat(string key, float defaultValue)
        {
            float result;
            return float.TryParse(ConfigurationManager.AppSettings[key], out result) ? result : defaultValue;
        }
    }
}
