SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PolicyOwner](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PolicyId] [int] FOREIGN KEY REFERENCES [Policy] ([Id]),
	[PartyId] [int] FOREIGN KEY REFERENCES [Party] ([Id]),
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_PolicyOwner] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a PolicyOwner'
    ,'user', 'dbo', 'table'
    ,'PolicyOwner'

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'PolicyOwner'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'PolicyOwner'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'PolicyOwner'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'PolicyOwner'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'PolicyOwner'
    ,'column', 'ModifiedTS'
GO