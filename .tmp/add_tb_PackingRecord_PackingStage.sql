-- ============================================
-- 增量迁移脚本：tb_PackingRecord 新增 PackingStage 字段
-- 适用数据库：XHS
-- 生成时间：2026-07-29
-- 执行方式：sqlcmd -S . -d XHS -U sa -P MES2016mj -C -i add_PackingStage.sql
-- ============================================

-- 新增 PackingStage 字段
IF COL_LENGTH(N'[dbo].[tb_PackingRecord]', 'PackingStage') IS NULL
BEGIN
    ALTER TABLE [dbo].[tb_PackingRecord] ADD [PackingStage] NVARCHAR(30) NULL;
    PRINT 'Added column: PackingStage';
END
ELSE
BEGIN
    PRINT 'Column PackingStage already exists.';
END;

-- 现有记录回填默认值
UPDATE [dbo].[tb_PackingRecord] SET [PackingStage] = 'Scanning' WHERE [PackingStage] IS NULL AND [Status] = 0;
UPDATE [dbo].[tb_PackingRecord] SET [PackingStage] = 'Completed' WHERE [PackingStage] IS NULL AND [Status] = 1;

PRINT '=== tb_PackingRecord PackingStage migration completed ===';
