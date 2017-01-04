SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Policy](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[QuoteReference] [nvarchar](20) NOT NULL UNIQUE,
	[Premium] DECIMAL(18,2) NULL,
	[PolicyStatusId] INT NOT NULL FOREIGN KEY REFERENCES [PolicyStatus]([Id]),
	[PremiumFrequencyId] INT NULL FOREIGN KEY REFERENCES [PremiumFrequency]([Id]),
	[SubmittedToRaiseTS] DATETIME NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[DeclarationAgree] [bit] NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_Policy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about the policy'
    ,'user', 'dbo', 'table'
    ,'Policy'

exec sp_addextendedproperty 'MS_Description'
    ,'Primary key, auto incrementing'
    ,'user', 'dbo'
    ,'table', 'Policy'
    ,'column', 'Id'            

exec sp_addextendedproperty 'MS_Description'
    ,'Unique external identifier for the policy record'
    ,'user', 'dbo'
    ,'table', 'Policy'
    ,'column', 'QuoteReference'   

exec sp_addextendedproperty 'MS_Description'
    ,'Total premium for this policy'
    ,'user', 'dbo'
    ,'table', 'Policy'
    ,'column', 'Premium'   

exec sp_addextendedproperty 'MS_Description'
    ,'Current status of the policy, links to PolicyStatus table'
    ,'user', 'dbo'
    ,'table', 'Policy'
    ,'column', 'PolicyStatusId'  

exec sp_addextendedproperty 'MS_Description'
    ,'Premium frequency of the policy, links to PremiumFrequency table'
    ,'user', 'dbo'
    ,'table', 'Policy'
    ,'column', 'PremiumFrequencyId'  

exec sp_addextendedproperty 'MS_Description'
    ,'DateTime that the policy was raised to policy admin system'
    ,'user', 'dbo'
    ,'table', 'Policy'
    ,'column', 'SubmittedToRaiseTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'Boolean to determine if policy declaration has been agreed too'
    ,'user', 'dbo'
    ,'table', 'Policy'
    ,'column', 'DeclarationAgree'  

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'Policy'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'Policy'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'Policy'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'Policy'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'Policy'
    ,'column', 'ModifiedTS'  