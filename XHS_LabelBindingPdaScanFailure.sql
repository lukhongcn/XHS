/* LabelBindingPDA 扫描失败审计表
   可重复执行；仅在对象不存在时创建，不删除或修改既有数据。 */
if object_id(N'dbo.tb_LabelBindingPdaScanFailure', N'U') is null
begin
    create table dbo.tb_LabelBindingPdaScanFailure
    (
        FailureId bigint identity(1,1) not null primary key,
        BindingTaskId nvarchar(36) null,
        CustomerRawCode nvarchar(1000) null,
        ScanRawCode nvarchar(1000) not null,
        ScanStage nvarchar(20) not null,
        FailureType nvarchar(50) not null,
        FailureReason nvarchar(1000) not null,
        ParsedBarcodeType nvarchar(50) null,
        ParsedPartNo nvarchar(100) null,
        ParsedBatchNo nvarchar(100) null,
        ParsedQty decimal(18,4) null,
        OperatorName nvarchar(100) null,
        DeviceInfo nvarchar(500) null,
        ClientIp nvarchar(64) null,
        ScanTime datetime not null constraint DF_tb_LabelBindingPdaScanFailure_ScanTime default(getdate()),
        IsProcessed bit not null constraint DF_tb_LabelBindingPdaScanFailure_IsProcessed default(0)
    );
end;
go

if not exists
(
    select 1 from sys.indexes
    where object_id = object_id(N'dbo.tb_LabelBindingPdaScanFailure')
      and name = N'IX_tb_LabelBindingPdaScanFailure_TaskTime'
)
begin
    create index IX_tb_LabelBindingPdaScanFailure_TaskTime
        on dbo.tb_LabelBindingPdaScanFailure(BindingTaskId, ScanTime desc);
end;
go
