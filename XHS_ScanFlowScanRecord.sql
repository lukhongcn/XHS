/*
    扫描流程绑定记录表
    每条记录对应一次流程步骤的成功扫描。
    FlowId 关联 tb_ScanFlow.FlowId，ScanName 保存流程名称。
*/
if object_id(N'dbo.tb_LabelBindingFactory', N'U') is not null
begin
    drop table dbo.tb_LabelBindingFactory;
end;
go

if object_id(N'dbo.tb_LabelBindingCustomer', N'U') is not null
begin
    drop table dbo.tb_LabelBindingCustomer;
end;
go

if object_id(N'dbo.tb_ScanFlowScanRecord', N'U') is null
begin
    create table dbo.tb_ScanFlowScanRecord
    (
        RecordId bigint identity(1,1) not null primary key,
        FlowId int not null,
        ScanName nvarchar(100) not null,
        BindingTaskId nvarchar(36) not null,
        StepId int null,
        StepCode varchar(50) not null,
        SeqNo int null,
        ScanContent nvarchar(1000) not null,
        Status nvarchar(20) not null constraint DF_tb_ScanFlowScanRecord_Status default(N'已完成'),
        ScanUser nvarchar(100) null,
        DeviceInfo nvarchar(500) null,
        ClientIp nvarchar(64) null,
        ScanTime datetime not null constraint DF_tb_ScanFlowScanRecord_ScanTime default(getdate()),
        constraint FK_tb_ScanFlowScanRecord_ScanFlow foreign key (FlowId) references dbo.tb_ScanFlow(FlowId)
    );
end;
go

if not exists
(
    select 1 from sys.indexes
    where object_id = object_id(N'dbo.tb_ScanFlowScanRecord')
      and name = N'IX_tb_ScanFlowScanRecord_FlowBarcode'
)
begin
    create index IX_tb_ScanFlowScanRecord_FlowBarcode
        on dbo.tb_ScanFlowScanRecord(FlowId, StepCode, Status);
end;
go

if not exists
(
    select 1 from sys.indexes
    where object_id = object_id(N'dbo.tb_ScanFlowScanRecord')
      and name = N'IX_tb_ScanFlowScanRecord_TaskTime'
)
begin
    create index IX_tb_ScanFlowScanRecord_TaskTime
        on dbo.tb_ScanFlowScanRecord(BindingTaskId, ScanTime desc);
end;
go
