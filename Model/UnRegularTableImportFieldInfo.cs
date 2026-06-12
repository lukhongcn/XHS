namespace XHS.Model
{
    /// <summary>
    /// 不规则表格导入字段配置。
    /// </summary>
    public class UnRegularTableImportFieldInfo
    {
        /// <summary>
        /// 导入模板编码。
        /// </summary>
        public string TemplateCode { get; set; }

        /// <summary>
        /// 栏位名称。
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Excel 中用于定位字段的提取关键字。
        /// </summary>
        public string ExtractKeyword { get; set; }

        /// <summary>
        /// 目标 Info 类中的属性名。
        /// </summary>
        public string FieldProperty { get; set; }

        /// <summary>
        /// 字段关键字相对记录起始单元格的行序号。
        /// </summary>
        public int? RowIndex { get; set; }

        /// <summary>
        /// 字段关键字相对记录起始单元格的列序号。
        /// </summary>
        public int? ColumnIndex { get; set; }

        /// <summary>
        /// 值相对关键字所在单元格的行偏移。
        /// </summary>
        public int? OffsetRow { get; set; }

        /// <summary>
        /// 值相对关键字所在单元格的列偏移。
        /// </summary>
        public int? OffsetColumn { get; set; }

        /// <summary>
        /// 是否必填。
        /// </summary>
        public bool? IsRequired { get; set; }

        /// <summary>
        /// 排序号。
        /// </summary>
        public int? SortNo { get; set; }

        /// <summary>
        /// 说明，用于标识该配置对应的业务表或用途。
        /// </summary>
        public string Comment { get; set; }
    }
}
