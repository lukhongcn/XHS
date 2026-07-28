-- ============================================
-- 增量迁移脚本 v2：更新 tb_PackingException 表结构
-- 适用数据库：XHS
-- 生成时间：2026-07-27
-- 执行方式：sqlcmd -S <server> -d XHS -U sa -P MES2016mj -C -i alter_tb_PackingException_v2.sql
-- ============================================

-- 1. 新增 PackingId 字段
IF COL_LENGTH(N'[dbo].[tb_PackingException]', 'PackingId') IS NULL
BEGIN
    ALTER TABLE [dbo].[tb_PackingException] ADD [PackingId] BIGINT NULL;
END;

-- 2. 新增 CartonNo 字段
IF COL_LENGTH(N'[dbo].[tb_PackingException]', 'CartonNo') IS NULL
BEGIN
    ALTER TABLE [dbo].[tb_PackingException] ADD [CartonNo] NVARCHAR(50) NULL;
END;

-- 3. 新增 TriggerQRCode 字段
IF COL_LENGTH(N'[dbo].[tb_PackingException]', 'TriggerQRCode') IS NULL
BEGIN
    ALTER TABLE [dbo].[tb_PackingException] ADD [TriggerQRCode] NVARCHAR(200) NULL;
END;

-- 4. 外键约束
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_tb_PackingException_tb_PackingRecord')
BEGIN
    ALTER TABLE [dbo].[tb_PackingException]
        ADD CONSTRAINT [FK_tb_PackingException_tb_PackingRecord]
        FOREIGN KEY ([PackingId]) REFERENCES [dbo].[tb_PackingRecord]([Id]);
END;

-- 5. PackingId + Status 复合索引
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_tb_PackingException_PackingId_Status')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_tb_PackingException_PackingId_Status]
        ON [dbo].[tb_PackingException]([PackingId], [Status]);
END;

-- 6. 同一 PackingId 最多只有一笔 Status=0 的待审核异常（需 QUOTED_IDENTIFIER ON）
SET QUOTED_IDENTIFIER ON;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_tb_PackingException_UniquePending')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_tb_PackingException_UniquePending]
        ON [dbo].[tb_PackingException]([PackingId])
        WHERE [Status] = 0;
END;
SET QUOTED_IDENTIFIER OFF;

PRINT '=== tb_PackingException migration v2 completed ===';
