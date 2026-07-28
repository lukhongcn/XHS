-- ============================================
-- 增量迁移脚本：更新 tb_PackingException 表结构
-- 适用数据库：XHS
-- 生成时间：2026-07-27
-- ============================================

-- 1. 新增字段（如不存在则添加）

-- PackingId：关联 tb_PackingRecord.Id
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[tb_PackingException]') AND name = 'PackingId'
)
BEGIN
    ALTER TABLE [dbo].[tb_PackingException] ADD [PackingId] BIGINT NULL;
    PRINT 'Added column: PackingId';
END
ELSE
BEGIN
    PRINT 'Column PackingId already exists.';
END
GO

-- CartonNo：箱号
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[tb_PackingException]') AND name = 'CartonNo'
)
BEGIN
    ALTER TABLE [dbo].[tb_PackingException] ADD [CartonNo] NVARCHAR(50) NULL;
    PRINT 'Added column: CartonNo';
END
ELSE
BEGIN
    PRINT 'Column CartonNo already exists.';
END
GO

-- TriggerQRCode：触发异常的二维码内容
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[tb_PackingException]') AND name = 'TriggerQRCode'
)
BEGIN
    ALTER TABLE [dbo].[tb_PackingException] ADD [TriggerQRCode] NVARCHAR(200) NULL;
    PRINT 'Added column: TriggerQRCode';
END
ELSE
BEGIN
    PRINT 'Column TriggerQRCode already exists.';
END
GO

-- 2. 外键约束（PackingId -> tb_PackingRecord.Id）
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_tb_PackingException_tb_PackingRecord'
)
BEGIN
    ALTER TABLE [dbo].[tb_PackingException]
        ADD CONSTRAINT [FK_tb_PackingException_tb_PackingRecord]
        FOREIGN KEY ([PackingId]) REFERENCES [dbo].[tb_PackingRecord]([Id]);
    PRINT 'Added foreign key: FK_tb_PackingException_tb_PackingRecord';
END
ELSE
BEGIN
    PRINT 'Foreign key FK_tb_PackingException_tb_PackingRecord already exists.';
END
GO

-- 3. PackingId + Status 复合索引
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes WHERE name = 'IX_tb_PackingException_PackingId_Status'
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_tb_PackingException_PackingId_Status]
        ON [dbo].[tb_PackingException]([PackingId], [Status]);
    PRINT 'Created index: IX_tb_PackingException_PackingId_Status';
END
ELSE
BEGIN
    PRINT 'Index IX_tb_PackingException_PackingId_Status already exists.';
END
GO

-- 4. 同一 PackingId 最多只有一笔 Status=0 的待审核异常（过滤唯一索引）
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes WHERE name = 'IX_tb_PackingException_UniquePending'
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_tb_PackingException_UniquePending]
        ON [dbo].[tb_PackingException]([PackingId])
        WHERE [Status] = 0;
    PRINT 'Created unique filtered index: IX_tb_PackingException_UniquePending';
END
ELSE
BEGIN
    PRINT 'Index IX_tb_PackingException_UniquePending already exists.';
END
GO

PRINT 'tb_PackingException migration completed.';
GO
