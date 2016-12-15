CREATE TABLE [dbo].[Table]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NULL, 
    [ParentId] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(250) NULL, 
    [URL] NVARCHAR(250) NULL
)
