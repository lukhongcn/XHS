ALTER TABLE dbo.tb_LabelCodeRule
ADD MatchRegex varchar(500) NULL;

ALTER TABLE dbo.tb_LabelCodeRule
DROP CONSTRAINT UQ_tb_LabelCodeRule_NameVersion;

ALTER TABLE dbo.tb_LabelCodeRule
DROP CONSTRAINT CK_tb_LabelCodeRule_ParseType;

ALTER TABLE dbo.tb_LabelCodeRule
ADD CONSTRAINT CK_tb_LabelCodeRule_ParseType CHECK ([ParseType] IN ('KEY_VALUE', 'POSITION', 'FIX_LENGTH', 'REGEX'));

ALTER TABLE dbo.tb_LabelCodeRule
ALTER COLUMN CustomerId varchar(100) NULL;

INSERT INTO dbo.tb_LabelCodeRule
(
    RuleName,
    LabelType,
    CustomerId,
    ParseType,
    Separator,
    KeySeparator,
    MatchRegex,
    [Version],
    Enabled,
    Remark
)
VALUES
(
    N'金鸿顺－零件批号数量条码',
    'CUSTOMER',
    N'1001',
    'REGEX',
    NULL,
    NULL,
    N'^(?<MaterialNo>[A-Z0-9]{6})(?<BatchNo>\d{4})(?<Qty>\d{3})$',
    1,
    1,
    N'零件号6位＋批号4位＋数量3位'
);
