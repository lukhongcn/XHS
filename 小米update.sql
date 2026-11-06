USE XHS;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @StartOrder SMALLINT;

    SELECT @StartOrder = ISNULL(MAX(ListOrder), 0)
    FROM dbo.tb_process WITH (UPDLOCK, HOLDLOCK);

    IF EXISTS
    (
        SELECT 1
        FROM dbo.tb_process
        WHERE ProcessId IN (N'QMJC', N'QMZJ', N'ZX')
    )
    BEGIN
        THROW 50001, N'QMJC、QMZJ 或 ZX 已存在，请先检查。', 1;
    END;

    INSERT INTO dbo.tb_process
    (
        ProcessId,
        ProcessName,
        ListOrder,
        NeedSchedule,
        CustomerProcessId,
        CustomerProcessName,
        CustomerListOrder,
        HoursPerDay,
        MachineList,
        multiprocess,
        scanshow,
        pricetype,
        isNeedQC,
        comment,
        price,
        processgroupid,
        stat,
        startdirectly,
        addClick,
        designgroup,
        isEndProductEnd,
        IsDesign,
        jobgroup,
        jobgroupname,
        finishdirectly,
        EndDirectly,
        nocombine,
        needCostCaculate,
        costColumnName,
        costColumnOrder,
        dealPartScan
    )
    VALUES
    (
        N'QMJC', N'气密检查', @StartOrder + 10, NULL,
        N'QMJC', N'气密检查', @StartOrder + 10, 8,
        NULL, 0, NULL, N'', NULL, NULL, NULL, NULL,
        0, 1, 1, N'design', 0, NULL, N'QA', N'QA',
        0, 1, 0, 1, NULL, NULL, NULL
    ),
    (
        N'QMZJ', N'气密重检判定', @StartOrder + 20, NULL,
        N'QMZJ', N'气密重检判定', @StartOrder + 20, 8,
        NULL, 0, NULL, N'QC', NULL, NULL, NULL, NULL,
        0, 1, 1, N'design', 0, NULL, N'QA', N'QA',
        0, 1, 0, 1, NULL, NULL, NULL
    ),
    (
        N'ZX', N'装箱', @StartOrder + 30, NULL,
        N'ZX', N'装箱', @StartOrder + 30, 8,
        NULL, 0, NULL, N'', NULL, NULL, NULL, NULL,
        0, 1, 1, N'design', 1, NULL, N'A9', N'A9',
        1, 1, 0, 1, NULL, NULL, NULL
    );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO

SELECT
    ProcessId,
    ProcessName,
    ListOrder,
    PriceType,
    CustomerProcessId,
    CustomerProcessName
FROM dbo.tb_process
WHERE ProcessId IN (N'QMJC', N'QMZJ', N'ZX')
ORDER BY ListOrder;
GO

USE XHS;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.tb_StandProcess', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.tb_StandProcess
        (
            Id                   INT IDENTITY(1,1) NOT NULL,
            listOrder            INT NULL,
            processId            NVARCHAR(100) NULL,
            processNeedMinutes   INT NULL,
            standardProcessType  INT NOT NULL,
            standardProcessName  NVARCHAR(100) NULL,
            processno            NVARCHAR(400) NULL,
            StatusId             NVARCHAR(100) NOT NULL CONSTRAINT DF_tb_StandProcess_StatusId DEFAULT (N'Pending'),
            partno               NVARCHAR(100) NULL,
            partCount            INT NULL,
            scheduleDay          INT NOT NULL CONSTRAINT DF_tb_StandProcess_scheduleDay DEFAULT (-1),
            customerProcessId    NVARCHAR(100) NULL,
            supplyId             INT NULL,
            ProcessComment       NVARCHAR(MAX) NULL,
            QCPercent            DECIMAL(5,2) NULL,
            assemblyFixture      NVARCHAR(400) NULL,
            assemblyComment      NVARCHAR(400) NULL,
            CONSTRAINT PK_tb_StandProcess PRIMARY KEY (Id)
        );
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.tb_StandProcess
        WHERE standardProcessName = N'小米'
          AND processId IN (N'QMJC', N'QMZJ', N'ZX')
    )
    BEGIN
        INSERT INTO dbo.tb_StandProcess
        (
            listOrder,
            processId,
            processNeedMinutes,
            standardProcessType,
            standardProcessName,
            processno,
            StatusId,
            partno,
            partCount,
            scheduleDay,
            customerProcessId,
            supplyId,
            ProcessComment,
            QCPercent,
            assemblyFixture,
            assemblyComment
        )
        VALUES
        (
            1, N'QMJC', 0, 4, N'小米', CONVERT(NVARCHAR(400), NEWID()),
            N'Ready', NULL, 0, -1, N'QMJC', NULL, NULL, NULL, NULL, NULL
        ),
        (
            2, N'QMZJ', 0, 4, N'小米', CONVERT(NVARCHAR(400), NEWID()),
            N'Pending', NULL, 0, -1, N'QMZJ', NULL, NULL, NULL, NULL, NULL
        ),
        (
            3, N'ZX', 0, 4, N'小米', CONVERT(NVARCHAR(400), NEWID()),
            N'Pending', NULL, 0, -1, N'ZX', NULL, NULL, NULL, NULL, NULL
        );
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO

SELECT
    Id,
    listOrder,
    processId,
    processNeedMinutes,
    standardProcessType,
    standardProcessName,
    StatusId,
    customerProcessId
FROM dbo.tb_StandProcess
WHERE standardProcessName = N'小米'
ORDER BY listOrder;
GO

USE XHS;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.tb_StandProcess
        WHERE standardProcessName = N'AirtightParts'
    )
    BEGIN
        INSERT INTO dbo.tb_StandProcess
        (
            listOrder,
            processId,
            processNeedMinutes,
            standardProcessType,
            standardProcessName,
            processno,
            StatusId,
            partno,
            partCount,
            scheduleDay,
            customerProcessId
        )
        VALUES
        (
            1, N'QMJC', 0, 4, N'AirtightParts', CONVERT(NVARCHAR(400), NEWID()),
            N'Ready', NULL, 0, -1, N'QMJC'
        ),
        (
            2, N'ZX', 0, 4, N'AirtightParts', CONVERT(NVARCHAR(400), NEWID()),
            N'Pending', NULL, 0, -1, N'ZX'
        );
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.tb_StandProcess
        WHERE standardProcessName = N'Packing'
    )
    BEGIN
        INSERT INTO dbo.tb_StandProcess
        (
            listOrder,
            processId,
            processNeedMinutes,
            standardProcessType,
            standardProcessName,
            processno,
            StatusId,
            partno,
            partCount,
            scheduleDay,
            customerProcessId
        )
        VALUES
        (
            1, N'ZX', 0, 4, N'Packing', CONVERT(NVARCHAR(400), NEWID()),
            N'Ready', NULL, 0, -1, N'ZX'
        );
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO

SELECT
    standardProcessName,
    listOrder,
    processId,
    StatusId
FROM dbo.tb_StandProcess
WHERE standardProcessName IN (N'AirtightParts', N'Packing')
ORDER BY standardProcessName, listOrder;
GO

DELETE FROM dbo.tb_StandProcess
WHERE standardProcessName = N'小米';
GO

IF COL_LENGTH(N'dbo.tb_PartMaster', N'standardProcessName') IS NULL
BEGIN
    ALTER TABLE dbo.tb_PartMaster
    ADD standardProcessName NVARCHAR(100) NULL;
END;
GO

UPDATE dbo.tb_PartMaster
SET standardProcessName = N'AirtightParts'
WHERE CustomerAbbr = N'XWD';
GO

IF COL_LENGTH(N'dbo.tb_PartMaster', N'BatchNo') IS NULL
BEGIN
    ALTER TABLE dbo.tb_PartMaster
    ADD BatchNo NVARCHAR(50) NULL;
END;
GO
