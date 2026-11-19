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

UPDATE dbo.tb_PartMaster
SET BatchNo = 1
WHERE CustomerAbbr = N'XWD';
GO

BEGIN TRY
    BEGIN TRANSACTION;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.tb_PartMaster
        WHERE CustomerAbbr = N'SQSD'
          AND CustomerMaterialNo = N'50040500966'
    )
    BEGIN
        UPDATE dbo.tb_PartMaster
        SET JHSPartNo = N'50040500966',
            MaterialName = N'IP5PM上盖',
            standardProcessName = N'AirtightParts',
            Remark = N'101',
            BatchNo = N''
        WHERE CustomerAbbr = N'SQSD'
          AND CustomerMaterialNo = N'50040500966';
    END
    ELSE
    BEGIN
        INSERT INTO dbo.tb_PartMaster
        (
            JHSPartNo,
            CustomerMaterialNo,
            CustomerAbbr,
            MaterialName,
            standardProcessName,
            Remark,
            BatchNo
        )
        VALUES
        (
            N'50040500966',
            N'50040500966',
            N'SQSD',
            N'IP5PM上盖',
            N'AirtightParts',
            N'101',
            N''
        );
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.tb_PartMaster
        WHERE CustomerAbbr = N'SQSD'
          AND CustomerMaterialNo = N'50160500633'
    )
    BEGIN
        UPDATE dbo.tb_PartMaster
        SET JHSPartNo = N'50160500633',
            MaterialName = N'IP5PM底护板',
            standardProcessName = N'Packing',
            Remark = N'100',
            BatchNo = N''
        WHERE CustomerAbbr = N'SQSD'
          AND CustomerMaterialNo = N'50160500633';
    END
    ELSE
    BEGIN
        INSERT INTO dbo.tb_PartMaster
        (
            JHSPartNo,
            CustomerMaterialNo,
            CustomerAbbr,
            MaterialName,
            standardProcessName,
            Remark,
            BatchNo
        )
        VALUES
        (
            N'50160500633',
            N'50160500633',
            N'SQSD',
            N'IP5PM底护板',
            N'Packing',
            N'100',
            N''
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
    JHSPartNo,
    CustomerMaterialNo,
    CustomerAbbr,
    MaterialName,
    standardProcessName,
    Remark,
    BatchNo
FROM dbo.tb_PartMaster
WHERE CustomerAbbr = N'SQSD'
  AND CustomerMaterialNo IN (N'50040500966', N'50160500633');
GO

USE XHS;
GO

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF DB_ID(N'HFAMD') IS NULL
        THROW 50001, N'数据库 HFAMD 不存在。', 1;

    IF OBJECT_ID(N'HFAMD.dbo.tb_standprocesstype', N'U') IS NULL
        THROW 50002, N'HFAMD.dbo.tb_standprocesstype 不存在。', 1;

    IF OBJECT_ID(N'dbo.tb_standprocesstype', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.tb_standprocesstype
        (
            Id            INT IDENTITY(1,1) NOT NULL,
            typeName      NVARCHAR(50) NULL,
            isProductType SMALLINT NOT NULL
                CONSTRAINT DF_tb_standprocesstype_isProductType DEFAULT (0),
            parentId      INT NOT NULL
                CONSTRAINT DF_tb_standprocesstype_parentId DEFAULT (0),

            CONSTRAINT PK_tb_standprocesstype PRIMARY KEY (Id)
        );
    END;

    SET IDENTITY_INSERT dbo.tb_standprocesstype ON;

    INSERT INTO dbo.tb_standprocesstype
    (
        Id,
        typeName,
        isProductType,
        parentId
    )
    SELECT
        s.Id,
        s.typeName,
        s.isProductType,
        s.parentId
    FROM HFAMD.dbo.tb_standprocesstype s
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.tb_standprocesstype t
        WHERE t.Id = s.Id
    );

    SET IDENTITY_INSERT dbo.tb_standprocesstype OFF;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.tb_StandProcess sp
        LEFT JOIN dbo.tb_standprocesstype st
            ON st.Id = sp.standardProcessType
        WHERE st.Id IS NULL
    )
    BEGIN
        THROW 50003, N'tb_StandProcess 中存在找不到对应类型的 standardProcessType，无法建立外键。', 1;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.foreign_keys
        WHERE name = N'FK_tb_StandProcess_standardProcessType'
          AND parent_object_id = OBJECT_ID(N'dbo.tb_StandProcess')
    )
    BEGIN
        ALTER TABLE dbo.tb_StandProcess
        ADD CONSTRAINT FK_tb_StandProcess_standardProcessType
        FOREIGN KEY (standardProcessType)
        REFERENCES dbo.tb_standprocesstype (Id);
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO

SELECT
    Id,
    typeName,
    isProductType,
    parentId
FROM dbo.tb_standprocesstype
ORDER BY Id;
GO

SELECT
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS ParentTable,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ParentColumn,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumn
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc
    ON fk.object_id = fkc.constraint_object_id
WHERE fk.name = N'FK_tb_StandProcess_standardProcessType';
GO

USE XHS;
GO

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF DB_ID(N'HFAMD') IS NULL
        THROW 50011, N'数据库 HFAMD 不存在。', 1;

    IF OBJECT_ID(N'HFAMD.dbo.tb_partprocess', N'U') IS NULL
        THROW 50012, N'HFAMD.dbo.tb_partprocess 不存在。', 1;

    IF OBJECT_ID(N'dbo.tb_partprocess', N'U') IS NULL
    BEGIN
        DECLARE @columnDefinitions NVARCHAR(MAX);
        DECLARE @createTableSql NVARCHAR(MAX);

        SELECT @columnDefinitions = STUFF
        (
            (
                SELECT
                    N',' + QUOTENAME(c.name) + N' ' +
                    CASE
                        WHEN t.name IN (N'nvarchar', N'nchar') AND c.max_length = -1
                            THEN t.name + N'(MAX)'
                        WHEN t.name IN (N'nvarchar', N'nchar')
                            THEN t.name + N'(' + CONVERT(NVARCHAR(10), c.max_length / 2) + N')'
                        WHEN t.name IN (N'varchar', N'char', N'varbinary', N'binary') AND c.max_length = -1
                            THEN t.name + N'(MAX)'
                        WHEN t.name IN (N'varchar', N'char', N'varbinary', N'binary')
                            THEN t.name + N'(' + CONVERT(NVARCHAR(10), c.max_length) + N')'
                        WHEN t.name IN (N'decimal', N'numeric')
                            THEN t.name + N'(' + CONVERT(NVARCHAR(10), c.precision) + N',' + CONVERT(NVARCHAR(10), c.scale) + N')'
                        ELSE t.name
                    END +
                    CASE
                        WHEN ic.is_identity = 1
                            THEN N' IDENTITY(' + CONVERT(NVARCHAR(50), ic.seed_value) + N',' + CONVERT(NVARCHAR(50), ic.increment_value) + N')'
                        ELSE N''
                    END +
                    CASE
                        WHEN dc.definition IS NOT NULL
                            THEN N' DEFAULT ' + dc.definition
                        ELSE N''
                    END +
                    CASE
                        WHEN c.is_nullable = 1 THEN N' NULL'
                        ELSE N' NOT NULL'
                    END
                FROM HFAMD.sys.columns c
                INNER JOIN HFAMD.sys.types t
                    ON c.user_type_id = t.user_type_id
                LEFT JOIN HFAMD.sys.identity_columns ic
                    ON c.object_id = ic.object_id
                   AND c.column_id = ic.column_id
                LEFT JOIN HFAMD.sys.default_constraints dc
                    ON c.default_object_id = dc.object_id
                WHERE c.object_id = OBJECT_ID(N'HFAMD.dbo.tb_partprocess')
                ORDER BY c.column_id
                FOR XML PATH(N''), TYPE
            ).value(N'.', N'NVARCHAR(MAX)'),
            1,
            1,
            N''
        );

        IF @columnDefinitions IS NULL OR LEN(@columnDefinitions) = 0
            THROW 50013, N'无法读取 HFAMD.dbo.tb_partprocess 的字段定义。', 1;

        SET @createTableSql =
            N'CREATE TABLE dbo.tb_partprocess (' +
            @columnDefinitions +
            N', CONSTRAINT PK_tb_partprocess PRIMARY KEY ([id]))';

        EXEC sys.sp_executesql @createTableSql;
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO

SELECT
    OBJECT_ID(N'dbo.tb_partprocess') AS TableId,
    COUNT(*) AS ColumnCount
FROM sys.columns
WHERE object_id = OBJECT_ID(N'dbo.tb_partprocess');
GO

USE XHS;
GO

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.tb_menu
        WHERE menuid = N'B09'
    )
    BEGIN
        INSERT INTO dbo.tb_menu
        (
            menuid,
            menuname,
            url,
            userlevel,
            pquery,
            padd,
            pedit,
            pdel,
            adminmenu,
            orderlist,
            menuclass,
            Id,
            parentId,
            icon,
            alturl
        )
        SELECT
            N'B09',
            N'欣旺达零件管控',
            N'PartProcessList.aspx?CustomerAbbr=XWD',
            menulevel.userlevel,
            menulevel.pquery,
            menulevel.padd,
            menulevel.pedit,
            menulevel.pdel,
            menulevel.adminmenu,
            menulevel.orderlist,
            menulevel.menuclass,
            (SELECT ISNULL(MAX(Id), 0) + 1 FROM dbo.tb_menu),
            menulevel.parentId,
            menulevel.icon,
            menulevel.alturl
        FROM dbo.tb_menu menulevel
        WHERE menulevel.menuid = N'B08';
    END
    ELSE
    BEGIN
        UPDATE dbo.tb_menu
        SET menuname = N'欣旺达零件管控',
            url = N'PartProcessList.aspx?CustomerAbbr=XWD'
        WHERE menuid = N'B09';
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO

SELECT
    menuid,
    menuname,
    url,
    parentId,
    menuclass,
    Id
FROM dbo.tb_menu
WHERE menuid = N'B09';
GO

USE XHS;
GO

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.tb_menu
        WHERE menuid = N'B10'
    )
    BEGIN
        INSERT INTO dbo.tb_menu
        (
            menuid,
            menuname,
            url,
            userlevel,
            pquery,
            padd,
            pedit,
            pdel,
            adminmenu,
            orderlist,
            menuclass,
            Id,
            parentId,
            icon,
            alturl
        )
        SELECT
            N'B10',
            N'上汽时代零件管控',
            N'PartProcessList.aspx?CustomerAbbr=SQSD',
            menulevel.userlevel,
            menulevel.pquery,
            menulevel.padd,
            menulevel.pedit,
            menulevel.pdel,
            menulevel.adminmenu,
            menulevel.orderlist,
            menulevel.menuclass,
            (SELECT ISNULL(MAX(Id), 0) + 1 FROM dbo.tb_menu),
            menulevel.parentId,
            menulevel.icon,
            menulevel.alturl
        FROM dbo.tb_menu menulevel
        WHERE menulevel.menuid = N'B08';
    END
    ELSE
    BEGIN
        UPDATE dbo.tb_menu
        SET menuname = N'上汽时代零件管控',
            url = N'PartProcessList.aspx?CustomerAbbr=SQSD'
        WHERE menuid = N'B10';
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO

SELECT
    menuid,
    menuname,
    url,
    parentId,
    menuclass,
    Id
FROM dbo.tb_menu
WHERE menuid = N'B10';
GO

USE XHS;
GO

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.tb_status', N'U') IS NULL
    BEGIN
        SELECT TOP (0) *
        INTO dbo.tb_status
        FROM HFAMD.dbo.tb_status;
    END;

    INSERT INTO dbo.tb_status
    SELECT sourceStatus.*
    FROM HFAMD.dbo.tb_status sourceStatus
    WHERE sourceStatus.StatusId IN (N'Ready', N'Pending')
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.tb_status targetStatus
          WHERE targetStatus.StatusId = sourceStatus.StatusId
      );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO

SELECT StatusId, StatusDescription, StatusColor, isSearch, fontColor, menuid
FROM dbo.tb_status
WHERE StatusId IN (N'Ready', N'Pending')
ORDER BY StatusId;
GO
