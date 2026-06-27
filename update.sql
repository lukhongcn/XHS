IF NOT EXISTS (
    SELECT 1
    FROM sys.objects
    WHERE object_id = OBJECT_ID(N'[dbo].[tb_ShippingGoods]')
      AND type = N'U'
)
BEGIN
    CREATE TABLE [dbo].[tb_ShippingGoods]
    (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [SupplierCode] NVARCHAR(50) NULL,
        [PartNo] NVARCHAR(50) NULL,
        [PartChineseName] NVARCHAR(200) NULL,
        [PartEnglishName] NVARCHAR(200) NULL,
        [Quantity] INT NULL,
        [SupplyBatchNo] NVARCHAR(100) NULL,
        [StackLayerCount] INT NULL,
        [ProductionDate] DATETIME NULL,
        [InspectionConfirmDate] DATETIME NULL,
        [CartonNo] NVARCHAR(50) NULL,
        [SingleBoxGrossWeight] DECIMAL(18, 2) NULL,
        [QrCode] NVARCHAR(200) NULL,
        [OutBoxQRCode] NVARCHAR(500) NULL,
        [Creater] NVARCHAR(50) NULL,
        [CreatDate] DATETIME NULL
    );
END;

IF COL_LENGTH('dbo.tb_ShippingGoods', 'OutBoxQRCode') IS NULL
BEGIN
    ALTER TABLE [dbo].[tb_ShippingGoods]
    ADD [OutBoxQRCode] NVARCHAR(500) NULL;
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.objects
    WHERE object_id = OBJECT_ID(N'[dbo].[tb_UnRegularTableImportField]')
      AND type = N'U'
)
BEGIN
    CREATE TABLE [dbo].[tb_UnRegularTableImportField]
    (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [TemplateCode] NVARCHAR(50) NULL,
        [ColumnName] NVARCHAR(100) NULL,
        [ExtractKeyword] NVARCHAR(100) NULL,
        [FieldProperty] NVARCHAR(100) NULL,
        [RowIndex] INT NULL,
        [ColumnIndex] INT NULL,
        [OffsetRow] INT NULL,
        [OffsetColumn] INT NULL,
        [IsRequired] BIT NULL,
        [SortNo] INT NULL,
        [Comment] NVARCHAR(500) NULL
    );
END;

IF COL_LENGTH('dbo.tb_UnRegularTableImportField', 'RowIndex') IS NULL
BEGIN
    ALTER TABLE [dbo].[tb_UnRegularTableImportField]
    ADD [RowIndex] INT NULL;
END;

IF COL_LENGTH('dbo.tb_UnRegularTableImportField', 'ColumnIndex') IS NULL
BEGIN
    ALTER TABLE [dbo].[tb_UnRegularTableImportField]
    ADD [ColumnIndex] INT NULL;
END;

IF OBJECT_ID(N'[dbo].[tb_PrintRecord]', N'U') IS NOT NULL
   AND COL_LENGTH('dbo.tb_PrintRecord', 'ReprintReasonsId') IS NULL
BEGIN
    ALTER TABLE [dbo].[tb_PrintRecord]
    ADD [ReprintReasonsId] INT NULL;
END;

IF OBJECT_ID(N'[dbo].[tb_PrintRecord]', N'U') IS NOT NULL
   AND OBJECT_ID(N'[dbo].[tb_ReprintReason]', N'U') IS NOT NULL
   AND NOT EXISTS (
       SELECT 1
       FROM sys.foreign_keys
       WHERE name = N'FK_tb_PrintRecord_tb_ReprintReason'
         AND parent_object_id = OBJECT_ID(N'[dbo].[tb_PrintRecord]')
   )
BEGIN
    ALTER TABLE [dbo].[tb_PrintRecord]
    ADD CONSTRAINT [FK_tb_PrintRecord_tb_ReprintReason]
    FOREIGN KEY ([ReprintReasonsId]) REFERENCES [dbo].[tb_ReprintReason]([Id]);
END;

GO

DELETE FROM [dbo].[tb_UnRegularTableImportField]
WHERE [TemplateCode] = N'ShippingGoods';

INSERT INTO [dbo].[tb_UnRegularTableImportField]
(
    [TemplateCode],
    [ColumnName],
    [ExtractKeyword],
    [FieldProperty],
    [RowIndex],
    [ColumnIndex],
    [OffsetRow],
    [OffsetColumn],
    [IsRequired],
    [SortNo],
    [Comment]
)
VALUES
(N'ShippingGoods', N'起始标记', N'KD专用', NULL, 0, 0, 0, 0, 1, 10, N'tb_ShippingGoods'),
(N'ShippingGoods', N'供应商代码', N'供应商代码', N'SupplierCode', 1, 0, 0, 1, 1, 20, N'tb_ShippingGoods'),
(N'ShippingGoods', N'零件号', N'零件号', N'PartNo', 2, 0, 0, 1, 1, 30, N'tb_ShippingGoods'),
(N'ShippingGoods', N'零件中文名称', N'零件中文名称', N'PartChineseName', 3, 0, 0, 1, 1, 40, N'tb_ShippingGoods'),
(N'ShippingGoods', N'英文名称', N'英文名称', N'PartEnglishName', 4, 0, 0, 1, 0, 50, N'tb_ShippingGoods'),
(N'ShippingGoods', N'数量', N'数量', N'Quantity', 5, 0, 0, 1, 1, 60, N'tb_ShippingGoods'),
(N'ShippingGoods', N'供货批次号', N'供货批次号', N'SupplyBatchNo', 6, 0, 0, 1, 1, 70, N'tb_ShippingGoods'),
(N'ShippingGoods', N'码放层数', N'码放层数', N'StackLayerCount', 7, 0, 0, 1, 0, 80, N'tb_ShippingGoods'),
(N'ShippingGoods', N'生产日期', N'生产日期', N'ProductionDate', 8, 0, 0, 1, 0, 90, N'tb_ShippingGoods'),
(N'ShippingGoods', N'检验确认日期', N'检验确认/日期', N'InspectionConfirmDate', 9, 0, 0, 1, 0, 100, N'tb_ShippingGoods'),
(N'ShippingGoods', N'纸箱编号', N'纸箱编号', N'CartonNo', 10, 0, 0, 1, 1, 110, N'tb_ShippingGoods'),
(N'ShippingGoods', N'单箱毛重', N'单箱毛重', N'SingleBoxGrossWeight', 11, 0, 0, 1, 0, 120, N'tb_ShippingGoods');

