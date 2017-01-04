
DECLARE @PreferredContactMethodFkName NVARCHAR(256)

--THE FK HAS A DYNAMIC NAME, SO WORK IT OUT
SELECT @PreferredContactMethodFkName = obj.name
FROM sys.foreign_key_columns fkc
INNER JOIN sys.objects obj
    ON obj.object_id = fkc.constraint_object_id
INNER JOIN sys.tables tab1
    ON tab1.object_id = fkc.parent_object_id
INNER JOIN sys.schemas sch
    ON tab1.schema_id = sch.schema_id
INNER JOIN sys.columns col1
    ON col1.column_id = parent_column_id AND col1.object_id = tab1.object_id
INNER JOIN sys.tables tab2
    ON tab2.object_id = fkc.referenced_object_id
INNER JOIN sys.columns col2
    ON col2.column_id = referenced_column_id AND col2.object_id = tab2.object_id
WHERE tab1.name = 'Party' AND col1.name = 'PreferredContactMethodId'

--DROP FK IF EXISTS
if exists (select 1 from sys.objects where object_id = OBJECT_ID(@PreferredContactMethodFkName) AND parent_object_id = OBJECT_ID('Party'))
BEGIN
	EXEC('ALTER TABLE [Party] DROP CONSTRAINT ' + @PreferredContactMethodFkName )
END 

--DROP COLUMN IF EXISTS
IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'Party'
                 AND COLUMN_NAME = 'PreferredContactMethodId') 
BEGIN
	ALTER TABLE [Party] DROP COLUMN [PreferredContactMethodId]
END

--DROP TABLE IF EXISTS
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'PreferredContactMethod')
BEGIN
   DROP TABLE [PreferredContactMethod]
END

