-- 第二步 JHX 标签格式：6 位品号 + 4 位数字-4 位数字批号 + 2 位数量
-- 示例：26J0092608-100724 -> MaterialNo=26J009, BatchNo=2608-1007, Qty=24
UPDATE dbo.tb_LabelCodeRule
SET MatchRegex = '^(?<MaterialNo>[A-Z0-9]{6})(?<BatchNo>\d{4}-\d{4})(?<Qty>\d{2})$'
WHERE RuleId = 5
  AND RuleName = N'金鸿顺－零件批号数量条码YYMMDD'
  AND CustomerId = 'JHX'
  AND LabelType = 'CUSTOMER';

-- 第二步使用本厂标签处理器，StepCode 必须与页面历史查询约定为 FACTORY。
UPDATE dbo.tb_ScanFlowStep
SET StepCode = 'FACTORY',
    RuleName = N'金鸿顺－零件批号数量条码YYMMDD'
WHERE FlowId = 1
  AND StepNo = 2
  AND StepCode = 'JHX_LABEL'
  AND CustomerId = 'JHX';

-- 修正已经写入但因旧 StepCode 无法显示的历史本厂标签记录。
UPDATE dbo.tb_ScanFlowScanRecord
SET StepCode = 'FACTORY'
WHERE FlowId = 1
  AND StepId = 5
  AND StepCode = 'JHX_LABEL';
