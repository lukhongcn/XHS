/* 解除绑定零件历史明细，仅保存解绑产生的数据，不参与有效装箱和配送单上传。 */
IF OBJECT_ID(N'[dbo].[tb_PackingUnboundScanRecord]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[tb_PackingUnboundScanRecord]
    (
        [Id] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_tb_PackingUnboundScanRecord] PRIMARY KEY,
        [PackingExceptionId] BIGINT NOT NULL,
        [OriginalPackingId] BIGINT NULL,
        [QRCodeType] NVARCHAR(20) NULL,
        [QRCode] NVARCHAR(200) NULL,
        [MaterialNo] NVARCHAR(50) NULL,
        [Qty] INT NULL,
        [ScanUser] NVARCHAR(50) NULL,
        [ScanTime] DATETIME NULL,
        [UnbindUser] NVARCHAR(50) NULL,
        [UnbindTime] DATETIME NOT NULL CONSTRAINT [DF_tb_PackingUnboundScanRecord_UnbindTime] DEFAULT (GETDATE())
    );

    ALTER TABLE [dbo].[tb_PackingUnboundScanRecord]
        ADD CONSTRAINT [FK_tb_PackingUnboundScanRecord_tb_PackingException]
        FOREIGN KEY ([PackingExceptionId]) REFERENCES [dbo].[tb_PackingException]([Id]);

    CREATE INDEX [IX_tb_PackingUnboundScanRecord_PackingExceptionId]
        ON [dbo].[tb_PackingUnboundScanRecord]([PackingExceptionId]);
    CREATE INDEX [IX_tb_PackingUnboundScanRecord_OriginalPackingId]
        ON [dbo].[tb_PackingUnboundScanRecord]([OriginalPackingId]);
END;
