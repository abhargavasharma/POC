USE MASTER; 
declare @defaultDataPath varChar(4000);
EXEC xp_instance_regread 'HKEY_LOCAL_MACHINE', 'Software\Microsoft\MSSQLServer\MSSQLServer\', 'DefaultData', @defaultDataPath OUTPUT;
IF (@defaultDataPath IS NULL)
begin
    EXEC xp_instance_regread 'HKEY_LOCAL_MACHINE', 'Software\Microsoft\MSSQLServer\Setup\', 'SQLDataRoot', @defaultDataPath OUTPUT
    IF (@defaultDataPath IS NOT NULL) SET @defaultDataPath = @defaultDataPath + '\Data'
end;

DECLARE @dataPath NVARCHAR(MAX) = @defaultDataPath + '\TALQuoteAndApply.mdf';
DECLARE @logPath NVARCHAR(MAX) = @defaultDataPath + '\TALQuoteAndApply_log.ldf';

IF EXISTS(SELECT TOP 1 1 FROM sys.sysdatabases WHERE Name = 'TALQuoteAndApply') 
BEGIN
    PRINT 'DB Exists, using local data and log file locations'
    SELECT @dataPath = [filename] FROM sys.sysdatabases WHERE Name = 'TALQuoteAndApply'
    SET @logPath = REPLACE(@dataPath, 'TALQuoteAndApply.mdf', 'TALQuoteAndApply_log.ldf')
    ALTER DATABASE TALQuoteAndApply SET SINGLE_USER WITH ROLLBACK IMMEDIATE; 
END
RESTORE DATABASE TALQuoteAndApply FROM DISK=N'\\itstorage\Deployment\DB\TALQuoteAndApply\tal_quote_apply_backup.bak' 
WITH REPLACE
, MOVE N'TALQuoteAndApply' TO @dataPath
, MOVE N'TALQuoteAndApply_log' TO @logPath; 

ALTER DATABASE TALQuoteAndApply SET MULTI_USER;


SET @dataPath = @defaultDataPath + '\TALQuoteAndApply_search.mdf';
SET @logPath = @defaultDataPath + '\TALQuoteAndApply_search_log.ldf';

IF EXISTS(SELECT TOP 1 1 FROM sys.sysdatabases WHERE Name = 'TALQuoteAndApply_search') 
BEGIN
    PRINT 'DB Exists, using local data and log file locations'
    SELECT @dataPath = [filename] FROM sys.sysdatabases WHERE Name = 'TALQuoteAndApply_search'
    SET @logPath = REPLACE(@dataPath, 'TALQuoteAndApply_search.mdf', 'TALQuoteAndApply_search_log.ldf')
    ALTER DATABASE [TALQuoteAndApply_search] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; 
END
RESTORE DATABASE [TALQuoteAndApply_search] FROM DISK=N'\\itstorage\Deployment\DB\TALQuoteAndApply\TALQuoteAndApply_Search.bak' 
WITH FILE=1, REPLACE
, MOVE N'TALQuoteAndApply_search' TO @dataPath
, MOVE N'TALQuoteAndApply_search_log' TO @logPath; 

ALTER DATABASE [TALQuoteAndApply_search] SET MULTI_USER;


SET @dataPath = @defaultDataPath + '\TALQuoteAndApply_SalesPortal.mdf';
SET @logPath = @defaultDataPath + '\TALQuoteAndApply_SalesPortal.ldf';

IF EXISTS(SELECT TOP 1 1 FROM sys.sysdatabases WHERE Name = 'TALQuoteAndApply_SalesPortal') 
BEGIN
    PRINT 'DB Exists, using local data and log file locations'
    SELECT @dataPath = [filename] FROM sys.sysdatabases WHERE Name = 'TALQuoteAndApply_SalesPortal'
    SET @logPath = REPLACE(@dataPath, 'TALQuoteAndApply_SalesPortal.mdf', 'TALQuoteAndApply_SalesPortal_log.ldf')
    ALTER DATABASE [TALQuoteAndApply_SalesPortal] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; 
END
RESTORE DATABASE [TALQuoteAndApply_SalesPortal] FROM DISK=N'\\itstorage\Deployment\DB\TALQuoteAndApply\TALQuoteAndApply_SalesPortal.bak' 
WITH FILE=1, REPLACE
, MOVE N'TALQuoteAndApply_SalesPortal' TO @dataPath
, MOVE N'TALQuoteAndApply_SalesPortal_Log' TO @logPath; 

ALTER DATABASE [TALQuoteAndApply_SalesPortal] SET MULTI_USER;