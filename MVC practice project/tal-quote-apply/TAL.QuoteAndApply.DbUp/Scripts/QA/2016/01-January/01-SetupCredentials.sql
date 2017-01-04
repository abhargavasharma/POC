IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'NT AUTHORITY\NETWORK SERVICE')
BEGIN
	CREATE USER [NT AUTHORITY\NETWORK SERVICE] FOR LOGIN [NT AUTHORITY\NETWORK SERVICE] WITH DEFAULT_SCHEMA=[dbo]
	EXEC sp_addrolemember 'db_owner', 'NT AUTHORITY\NETWORK SERVICE';
END 

----FOR REPORTING TEAM TO LOOK AT THE DATA----
IF NOT EXISTS(select loginname from master.dbo.syslogins where loginname = 'mmReadonly')
BEGIN
	CREATE LOGIN [mmReadonly] WITH PASSWORD=N'fTk8Pn/Z58Zg0R2KOOgUt7a3LZDNalRy7nUIkKxxLjc=', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
END
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'mmReadonly')
BEGIN
	CREATE USER [mmReadonly] FOR LOGIN [mmReadonly] WITH DEFAULT_SCHEMA=[dbo]
	EXEC sp_addrolemember 'db_datareader', 'mmReadonly';
END 

----FOR DB TEAM TO LOOK AT THE DATA----
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'TOWER\DBA')
BEGIN
	CREATE USER [TOWER\DBA] FOR LOGIN [TOWER\DBA] WITH DEFAULT_SCHEMA=[dbo]
	EXEC sp_addrolemember 'db_datareader', 'TOWER\DBA';
END 