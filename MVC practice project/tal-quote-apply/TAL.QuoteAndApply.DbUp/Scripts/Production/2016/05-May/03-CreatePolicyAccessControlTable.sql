CREATE TABLE [dbo].[LastTouchedByType](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
CONSTRAINT [PK_LastTouchedByType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a LastTouchedByType'
    ,'user', 'dbo', 'table'
    ,'LastTouchedByType'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the LastTouchedByType'
    ,'user', 'dbo'
    ,'table', 'LastTouchedByType'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the LastTouchedByType'
    ,'user', 'dbo'
    ,'table', 'LastTouchedByType'
    ,'column', 'Description'  

GO

-- POPULATE DATA --
GO

INSERT INTO [LastTouchedByType] ([Id], [Description]) VALUES (0, 'Agent');
INSERT INTO [LastTouchedByType] ([Id], [Description]) VALUES (1, 'Customer');
INSERT INTO [LastTouchedByType] ([Id], [Description]) VALUES (2, 'Underwriter');

GO

CREATE TABLE [dbo].[PolicyAccessControl](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PolicyId] [int] NOT NULL FOREIGN KEY REFERENCES [Policy]([Id]),
	[LastTouchedByName] NVARCHAR(256) NOT  NULL,
	[LastTouchedByTS] DATETIME NOT NULL,
	[LastTouchedByTypeId] INT NOT NULL  FOREIGN KEY REFERENCES [LastTouchedByType]([Id]),
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_PolicyAccessControl] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a PolicyAccessControl'
    ,'user', 'dbo', 'table'
    ,'PolicyAccessControl'

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'PolicyAccessControl'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'PolicyAccessControl'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'PolicyAccessControl'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'PolicyAccessControl'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'PolicyAccessControl'
    ,'column', 'ModifiedTS'
GO