/*
    清除 LabelBindingPDA 测试扫描记录
    仅处理 tb_ScanFlowScanRecord，按页面流程 FlowId 限制范围。
    执行前会先显示待删除数量；确认无误后再执行 DELETE。
*/

SELECT COUNT(*) AS PendingDeleteCount
FROM dbo.tb_ScanFlowScanRecord r
WHERE r.FlowId IN
(
    SELECT f.FlowId
    FROM dbo.tb_ScanFlow f
    WHERE f.FlowName = N'光束标签绑定流程'
);

BEGIN TRANSACTION;

DELETE r
FROM dbo.tb_ScanFlowScanRecord r
WHERE r.FlowId IN
(
    SELECT f.FlowId
    FROM dbo.tb_ScanFlow f
    WHERE f.FlowName = N'光束标签绑定流程'
);

SELECT @@ROWCOUNT AS DeletedCount;

COMMIT TRANSACTION;
