CREATE TABLE [dbo].[PolicyReferral](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PolicyId] [int] NOT NULL FOREIGN KEY REFERENCES [Policy]([Id]),
	[State] [int] NULL,
	[AssignedTo] NVARCHAR(256) NULL,
	[AssignedToTS] DATETIME NULL,
	[IsCompleted] BIT NOT NULL,
	[CompletedTS] DATETIME NULL,
	[CompletedBy] NVARCHAR(256) NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_PolicyReferral] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a PolicyReferral'
    ,'user', 'dbo', 'table'
    ,'PolicyReferral'

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'PolicyReferral'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'PolicyReferral'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'PolicyReferral'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'PolicyReferral'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'PolicyReferral'
    ,'column', 'ModifiedTS'
GO

