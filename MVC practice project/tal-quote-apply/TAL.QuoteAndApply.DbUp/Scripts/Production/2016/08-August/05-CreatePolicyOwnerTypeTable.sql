SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PolicyOwnerType](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PolicyOwnerType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


EXEC sys.sp_addextendedproperty 'MS_Description'
	, 'Unique Id of the PolicyOwnerType' 
	, 'SCHEMA', 'dbo'
	, 'TABLE',  'PolicyOwnerType'
	, 'COLUMN', 'Id'
GO

EXEC sys.sp_addextendedproperty 'MS_Description'
	, 'Description of the PolicyOwnerType' 
	, 'SCHEMA', 'dbo'
	, 'TABLE', 'PolicyOwnerType'
	, 'COLUMN', 'Description'
GO

EXEC sys.sp_addextendedproperty 'LOOKUP'
	, 'Lookup table, do not clean up the data from this table' 
	, 'SCHEMA', 'dbo'
	, 'TABLE', 'PolicyOwnerType'
GO

EXEC sys.sp_addextendedproperty 'MS_Description'
	, 'Holds information about a PolicyOwnerType' 
	, 'SCHEMA', 'dbo'
	, 'TABLE', 'PolicyOwnerType'
GO


INSERT INTO [dbo].[PolicyOwnerType] ([Id] ,[Description]) VALUES (0, 'Ordinary');
INSERT INTO [dbo].[PolicyOwnerType] ([Id] ,[Description]) VALUES (1, 'SelfManagedSuperFund');
GO