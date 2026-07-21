/*
    Label Parser Engine - SQL Server 初始化腳本（依 Label_Parser_Engine_Design.md）

    相容：SQL Server 2008 R2+

    核心範圍：
      1. 標籤規則
      2. 標籤欄位規則
      3. 欄位類型
      4. 批次規則
      5. 欄位名稱映射

    支援解析方式：
      - KEY_VALUE
      - POSITION
      - FIX_LENGTH

    不包含：
      - 掃描記錄
      - 入庫、裝箱、出貨
      - 追溯關聯
      - 物料級業務規則

    注意：
      - 本腳本以設計文件第 4 節的正式表名為準：
        tb_LabelCodeRule、tb_LabelCodeRuleField。
      - 腳本可重複執行；已存在的表與初始化資料不會重複建立。
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    /* ================================================================
       1. 欄位類型：tb_FieldType
       ================================================================ */
    IF OBJECT_ID(N'dbo.tb_FieldType', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.tb_FieldType
        (
            TypeCode    varchar(30)   NOT NULL,
            TypeName    nvarchar(50)  NOT NULL,
            ParserClass varchar(200)  NULL,
            CONSTRAINT PK_tb_FieldType PRIMARY KEY (TypeCode)
        );
    END;

    /* ================================================================
       2. 批次規則：tb_BatchRule
       ================================================================ */
    IF OBJECT_ID(N'dbo.tb_BatchRule', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.tb_BatchRule
        (
            BatchRuleId int IDENTITY(1,1) NOT NULL,
            RuleName    nvarchar(100) NOT NULL,
            [Format]    varchar(100)  NOT NULL,
            [Length]    int           NOT NULL,
            Remark      nvarchar(500) NULL,
            CONSTRAINT PK_tb_BatchRule PRIMARY KEY (BatchRuleId),
            CONSTRAINT UQ_tb_BatchRule_RuleName UNIQUE (RuleName),
            CONSTRAINT CK_tb_BatchRule_Length CHECK ([Length] > 0)
        );
    END;

    /* ================================================================
       3. 標籤規則主表：tb_LabelCodeRule
       ================================================================ */
    IF OBJECT_ID(N'dbo.tb_LabelCodeRule', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.tb_LabelCodeRule
        (
            RuleId       int IDENTITY(1,1) NOT NULL,
            RuleName     nvarchar(150) NOT NULL,
            LabelType    varchar(20)   NOT NULL,  -- CUSTOMER / MES
            CustomerId   int           NULL,
            ParseType    varchar(20)   NOT NULL,  -- KEY_VALUE / POSITION / FIX_LENGTH
            Separator    nvarchar(20)  NULL,
            KeySeparator nvarchar(20)  NULL,
            [Version]    int           NOT NULL CONSTRAINT DF_tb_LabelCodeRule_Version DEFAULT (1),
            Enabled      bit           NOT NULL CONSTRAINT DF_tb_LabelCodeRule_Enabled DEFAULT (1),
            Remark       nvarchar(1000) NULL,
            CONSTRAINT PK_tb_LabelCodeRule PRIMARY KEY (RuleId),
            CONSTRAINT UQ_tb_LabelCodeRule_NameVersion UNIQUE (RuleName, [Version]),
            CONSTRAINT CK_tb_LabelCodeRule_LabelType
                CHECK (LabelType IN ('CUSTOMER', 'MES')),
            CONSTRAINT CK_tb_LabelCodeRule_ParseType
                CHECK (ParseType IN ('KEY_VALUE', 'POSITION', 'FIX_LENGTH')),
            CONSTRAINT CK_tb_LabelCodeRule_Version
                CHECK ([Version] > 0)
        );
    END;

    /* ================================================================
       4. 標籤欄位解析規則：tb_LabelCodeRuleField
       ================================================================ */
    IF OBJECT_ID(N'dbo.tb_LabelCodeRuleField', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.tb_LabelCodeRuleField
        (
            Id            int IDENTITY(1,1) NOT NULL,
            RuleId        int          NOT NULL,
            FieldName     varchar(80)  NOT NULL,
            DataType      varchar(30)  NOT NULL,
            KeyCode       varchar(80)  NULL,
            [Position]    int          NULL,
            StartPosition int          NULL,
            [Length]      int          NULL,
            BatchRuleId   int          NULL,
            Required      bit          NOT NULL CONSTRAINT DF_tb_LabelCodeRuleField_Required DEFAULT (0),
            SortNo        int          NOT NULL CONSTRAINT DF_tb_LabelCodeRuleField_SortNo DEFAULT (0),
            CONSTRAINT PK_tb_LabelCodeRuleField PRIMARY KEY (Id),
            CONSTRAINT UQ_tb_LabelCodeRuleField_RuleField UNIQUE (RuleId, FieldName),
            CONSTRAINT FK_tb_LabelCodeRuleField_Rule
                FOREIGN KEY (RuleId) REFERENCES dbo.tb_LabelCodeRule(RuleId),
            CONSTRAINT FK_tb_LabelCodeRuleField_FieldType
                FOREIGN KEY (DataType) REFERENCES dbo.tb_FieldType(TypeCode),
            CONSTRAINT FK_tb_LabelCodeRuleField_BatchRule
                FOREIGN KEY (BatchRuleId) REFERENCES dbo.tb_BatchRule(BatchRuleId),
            CONSTRAINT CK_tb_LabelCodeRuleField_Position
                CHECK ([Position] IS NULL OR [Position] > 0),
            CONSTRAINT CK_tb_LabelCodeRuleField_StartPosition
                CHECK (StartPosition IS NULL OR StartPosition > 0),
            CONSTRAINT CK_tb_LabelCodeRuleField_Length
                CHECK ([Length] IS NULL OR [Length] > 0),
            CONSTRAINT CK_tb_LabelCodeRuleField_SortNo
                CHECK (SortNo >= 0)
        );
    END;

    /* ================================================================
       5. 初始化欄位類型
       ================================================================ */
    IF NOT EXISTS (SELECT 1 FROM dbo.tb_FieldType WHERE TypeCode = 'STRING')
        INSERT dbo.tb_FieldType(TypeCode, TypeName, ParserClass)
        VALUES ('STRING', N'文字', 'StringFieldParser');

    IF NOT EXISTS (SELECT 1 FROM dbo.tb_FieldType WHERE TypeCode = 'NUMBER')
        INSERT dbo.tb_FieldType(TypeCode, TypeName, ParserClass)
        VALUES ('NUMBER', N'整數', 'NumberFieldParser');

    IF NOT EXISTS (SELECT 1 FROM dbo.tb_FieldType WHERE TypeCode = 'DECIMAL')
        INSERT dbo.tb_FieldType(TypeCode, TypeName, ParserClass)
        VALUES ('DECIMAL', N'數量／小數', 'DecimalFieldParser');

    IF NOT EXISTS (SELECT 1 FROM dbo.tb_FieldType WHERE TypeCode = 'DATE')
        INSERT dbo.tb_FieldType(TypeCode, TypeName, ParserClass)
        VALUES ('DATE', N'日期', 'DateFieldParser');

    IF NOT EXISTS (SELECT 1 FROM dbo.tb_FieldType WHERE TypeCode = 'BATCH')
        INSERT dbo.tb_FieldType(TypeCode, TypeName, ParserClass)
        VALUES ('BATCH', N'批次', 'BatchFieldParser');

    /* ================================================================
       7. 初始化批次規則
       ================================================================ */
    IF NOT EXISTS (SELECT 1 FROM dbo.tb_BatchRule WHERE RuleName = N'年月日')
        INSERT dbo.tb_BatchRule(RuleName, [Format], [Length], Remark)
        VALUES (N'年月日', 'YYMMDD', 6, N'例如 260602 解析為 2026-06-02');

    IF NOT EXISTS (SELECT 1 FROM dbo.tb_BatchRule WHERE RuleName = N'年週')
        INSERT dbo.tb_BatchRule(RuleName, [Format], [Length], Remark)
        VALUES (N'年週', 'YYWW', 4, N'例如 2625 解析為 2026 年第 25 週');

    /* ================================================================
       8. 建立三種解析方式的停用範例
          實際使用前請填入 CustomerId、確認欄位並將 Enabled 改為 1。
       ================================================================ */
    IF NOT EXISTS
    (
        SELECT 1
          FROM dbo.tb_LabelCodeRule
         WHERE RuleName = N'範例－KEY_VALUE 編號型'
           AND [Version] = 1
    )
    BEGIN
        INSERT dbo.tb_LabelCodeRule
        (
            RuleName, LabelType, CustomerId, ParseType,
            Separator, KeySeparator, [Version], Enabled, Remark
        )
        VALUES
        (
            N'範例－KEY_VALUE 編號型', 'CUSTOMER', NULL, 'KEY_VALUE',
            N'$', N'#', 1, 0,
            N'範例：10#202004112AA$11#3051$17#60$；啟用前請設定 CustomerId。'
        );
    END;

    IF NOT EXISTS
    (
        SELECT 1
          FROM dbo.tb_LabelCodeRule
         WHERE RuleName = N'範例－POSITION 位置型'
           AND [Version] = 1
    )
    BEGIN
        INSERT dbo.tb_LabelCodeRule
        (
            RuleName, LabelType, CustomerId, ParseType,
            Separator, KeySeparator, [Version], Enabled, Remark
        )
        VALUES
        (
            N'範例－POSITION 位置型', 'CUSTOMER', NULL, 'POSITION',
            N'$', NULL, 1, 0,
            N'範例：ABC001$3051$60；啟用前請設定 CustomerId。'
        );
    END;

    IF NOT EXISTS
    (
        SELECT 1
          FROM dbo.tb_LabelCodeRule
         WHERE RuleName = N'範例－FIX_LENGTH 固定長度型'
           AND [Version] = 1
    )
    BEGIN
        INSERT dbo.tb_LabelCodeRule
        (
            RuleName, LabelType, CustomerId, ParseType,
            Separator, KeySeparator, [Version], Enabled, Remark
        )
        VALUES
        (
            N'範例－FIX_LENGTH 固定長度型', 'MES', NULL, 'FIX_LENGTH',
            NULL, NULL, 1, 0,
            N'範例：26J033260530500。'
        );
    END;

    /* ================================================================
       9. 初始化範例欄位規則
       ================================================================ */
    DECLARE @KeyValueRuleId int;
    DECLARE @PositionRuleId int;
    DECLARE @FixLengthRuleId int;
    DECLARE @YYMMDDRuleId int;

    SELECT @KeyValueRuleId = RuleId
      FROM dbo.tb_LabelCodeRule
     WHERE RuleName = N'範例－KEY_VALUE 編號型'
       AND [Version] = 1;

    SELECT @PositionRuleId = RuleId
      FROM dbo.tb_LabelCodeRule
     WHERE RuleName = N'範例－POSITION 位置型'
       AND [Version] = 1;

    SELECT @FixLengthRuleId = RuleId
      FROM dbo.tb_LabelCodeRule
     WHERE RuleName = N'範例－FIX_LENGTH 固定長度型'
       AND [Version] = 1;

    SELECT @YYMMDDRuleId = BatchRuleId
      FROM dbo.tb_BatchRule
     WHERE RuleName = N'年月日';

    /* KEY_VALUE：10=MaterialNo、11=CustomerPartNo、17=Qty */
    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.tb_LabelCodeRuleField
         WHERE RuleId = @KeyValueRuleId AND FieldName = 'MaterialNo'
    )
        INSERT dbo.tb_LabelCodeRuleField
        (RuleId, FieldName, DataType, KeyCode, Required, SortNo)
        VALUES (@KeyValueRuleId, 'MaterialNo', 'STRING', '10', 1, 10);

    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.tb_LabelCodeRuleField
         WHERE RuleId = @KeyValueRuleId AND FieldName = 'CustomerPartNo'
    )
        INSERT dbo.tb_LabelCodeRuleField
        (RuleId, FieldName, DataType, KeyCode, Required, SortNo)
        VALUES (@KeyValueRuleId, 'CustomerPartNo', 'STRING', '11', 0, 20);

    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.tb_LabelCodeRuleField
         WHERE RuleId = @KeyValueRuleId AND FieldName = 'Qty'
    )
        INSERT dbo.tb_LabelCodeRuleField
        (RuleId, FieldName, DataType, KeyCode, Required, SortNo)
        VALUES (@KeyValueRuleId, 'Qty', 'DECIMAL', '17', 1, 30);

    /* POSITION：第 1/2/3 段 */
    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.tb_LabelCodeRuleField
         WHERE RuleId = @PositionRuleId AND FieldName = 'MaterialNo'
    )
        INSERT dbo.tb_LabelCodeRuleField
        (RuleId, FieldName, DataType, [Position], Required, SortNo)
        VALUES (@PositionRuleId, 'MaterialNo', 'STRING', 1, 1, 10);

    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.tb_LabelCodeRuleField
         WHERE RuleId = @PositionRuleId AND FieldName = 'CustomerPartNo'
    )
        INSERT dbo.tb_LabelCodeRuleField
        (RuleId, FieldName, DataType, [Position], Required, SortNo)
        VALUES (@PositionRuleId, 'CustomerPartNo', 'STRING', 2, 0, 20);

    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.tb_LabelCodeRuleField
         WHERE RuleId = @PositionRuleId AND FieldName = 'Qty'
    )
        INSERT dbo.tb_LabelCodeRuleField
        (RuleId, FieldName, DataType, [Position], Required, SortNo)
        VALUES (@PositionRuleId, 'Qty', 'DECIMAL', 3, 1, 30);

    /* FIX_LENGTH：1-5 物料、6-11 批次、12-14 數量 */
    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.tb_LabelCodeRuleField
         WHERE RuleId = @FixLengthRuleId AND FieldName = 'MaterialNo'
    )
        INSERT dbo.tb_LabelCodeRuleField
        (RuleId, FieldName, DataType, StartPosition, [Length], Required, SortNo)
        VALUES (@FixLengthRuleId, 'MaterialNo', 'STRING', 1, 5, 1, 10);

    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.tb_LabelCodeRuleField
         WHERE RuleId = @FixLengthRuleId AND FieldName = 'BatchNo'
    )
        INSERT dbo.tb_LabelCodeRuleField
        (RuleId, FieldName, DataType, StartPosition, [Length], BatchRuleId, Required, SortNo)
        VALUES (@FixLengthRuleId, 'BatchNo', 'BATCH', 6, 6, @YYMMDDRuleId, 1, 20);

    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.tb_LabelCodeRuleField
         WHERE RuleId = @FixLengthRuleId AND FieldName = 'Qty'
    )
        INSERT dbo.tb_LabelCodeRuleField
        (RuleId, FieldName, DataType, StartPosition, [Length], Required, SortNo)
        VALUES (@FixLengthRuleId, 'Qty', 'DECIMAL', 12, 3, 1, 30);

    COMMIT TRANSACTION;

    /* 初始化結果 */
    SELECT N'欄位類型' AS Item, COUNT(*) AS Qty FROM dbo.tb_FieldType
    UNION ALL
    SELECT N'批次規則', COUNT(*) FROM dbo.tb_BatchRule
    UNION ALL
    SELECT N'標籤規則', COUNT(*) FROM dbo.tb_LabelCodeRule
    UNION ALL
    SELECT N'標籤欄位規則', COUNT(*) FROM dbo.tb_LabelCodeRuleField
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrorMessage nvarchar(4000);
    DECLARE @ErrorSeverity int;
    DECLARE @ErrorState int;

    SELECT
        @ErrorMessage = ERROR_MESSAGE(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE();

    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;
