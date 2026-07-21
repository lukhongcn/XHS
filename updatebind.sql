IF OBJECT_ID(N'[dbo].[tb_LabelRule]', N'U') IS NOT NULL
   AND OBJECT_ID(N'[dbo].[tb_LabelCode]', N'U') IS NULL
BEGIN
    EXEC sp_rename N'[dbo].[tb_LabelRule]', N'tb_LabelCode';
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.objects
    WHERE object_id = OBJECT_ID(N'[dbo].[tb_LabelCodeRuleField]')
      AND type = N'U'
)
BEGIN
    CREATE TABLE [dbo].[tb_LabelCodeRuleField]
    (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [RuleId] INT NOT NULL,
        [FieldName] NVARCHAR(100) NOT NULL,
        [DataType] NVARCHAR(30) NULL,
        [KeyCode] NVARCHAR(50) NULL,
        [Position] INT NULL,
        [StartPosition] INT NULL,
        [Length] INT NULL,
        [BatchRuleId] INT NULL,
        [Required] BIT NOT NULL CONSTRAINT [DF_tb_LabelCodeRuleField_Required] DEFAULT (0),
        [SortNo] INT NOT NULL CONSTRAINT [DF_tb_LabelCodeRuleField_SortNo] DEFAULT (0)
    );
END;

GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.objects
    WHERE object_id = OBJECT_ID(N'[dbo].[tb_FieldType]')
      AND type = N'U'
)
BEGIN
    CREATE TABLE [dbo].[tb_FieldType]
    (
        [TypeCode] NVARCHAR(30) NOT NULL PRIMARY KEY,
        [TypeName] NVARCHAR(100) NOT NULL,
        [ParserClass] NVARCHAR(200) NULL
    );
END;

GO

IF OBJECT_ID(N'[dbo].[DF_tb_LabelRule_Version]', N'D') IS NOT NULL
   AND OBJECT_ID(N'[dbo].[DF_tb_LabelCode_Version]', N'D') IS NULL
BEGIN
    EXEC sp_rename N'[dbo].[DF_tb_LabelRule_Version]', N'DF_tb_LabelCode_Version', N'OBJECT';
END;

IF OBJECT_ID(N'[dbo].[DF_tb_LabelRule_Enabled]', N'D') IS NOT NULL
   AND OBJECT_ID(N'[dbo].[DF_tb_LabelCode_Enabled]', N'D') IS NULL
BEGIN
    EXEC sp_rename N'[dbo].[DF_tb_LabelRule_Enabled]', N'DF_tb_LabelCode_Enabled', N'OBJECT';
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.objects
    WHERE object_id = OBJECT_ID(N'[dbo].[tb_LabelCode]')
      AND type = N'U'
)
BEGIN
    CREATE TABLE [dbo].[tb_LabelCode]
    (
        [RuleId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [RuleName] NVARCHAR(100) NOT NULL,
        [LabelType] NVARCHAR(20) NOT NULL,
        [CustomerId] INT NULL,
        [ParseType] NVARCHAR(30) NOT NULL,
        [Separator] NVARCHAR(10) NULL,
        [KeySeparator] NVARCHAR(10) NULL,
        [Version] INT NOT NULL CONSTRAINT [DF_tb_LabelCode_Version] DEFAULT (1),
        [Enabled] BIT NOT NULL CONSTRAINT [DF_tb_LabelCode_Enabled] DEFAULT (1),
        [Remark] NVARCHAR(500) NULL
    );
END;

GO
