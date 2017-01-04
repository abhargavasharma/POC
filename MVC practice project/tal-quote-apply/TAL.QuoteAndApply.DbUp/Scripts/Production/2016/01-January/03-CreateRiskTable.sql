SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Risk](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GenderId] [int] NULL FOREIGN KEY REFERENCES [Gender]([Id]),
	[DateOfBirth] [date] NULL,
	[ResidencyId] [int] NULL,
	[SmokerStatusId] [int] NULL FOREIGN KEY REFERENCES [SmokerStatus]([Id]),
	[AnnualIncome] [bigint] NULL,
	[InterviewId] [nvarchar](50) NULL,
	[InterviewConcurrencyToken] [nvarchar](12) NULL,
	[InterviewTemplateVersion] [nvarchar](50) NULL, 
	[PartyId] [int] NOT NULL FOREIGN KEY REFERENCES [Party]([Id]),
	[PolicyId] [int] NOT NULL FOREIGN KEY REFERENCES [Policy]([Id]),
	[LprBeneficiary] [bit] NOT NULL,
	[Premium] DECIMAL(18,2) NULL,
	[MultiPlanDiscount] DECIMAL(18,2) NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_Risk] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


GO

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a Risk'
    ,'user', 'dbo', 'table'
    ,'Risk'

exec sp_addextendedproperty 'MS_Description'
    ,'Primary key, auto incrementing'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'Id'

exec sp_addextendedproperty 'MS_Description'
    ,'Gender of the risk, links to Gender table'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'GenderId'     

exec sp_addextendedproperty 'MS_Description'
    ,'DateOfBirth of the risk'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'DateOfBirth'  

exec sp_addextendedproperty 'MS_Description'
    ,'ResidencyId of the risk'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'ResidencyId'    

exec sp_addextendedproperty 'MS_Description'
    ,'SmokerStatus of the risk, links to SmokerStatus table'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'SmokerStatusId'   

exec sp_addextendedproperty 'MS_Description'
    ,'AnnualIncome of the risk, answered in TALUS interview and synced to Risk'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'AnnualIncome'   

exec sp_addextendedproperty 'MS_Description'
    ,'InterviewId of the risk, links to TALUS interview'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'InterviewId'   

exec sp_addextendedproperty 'MS_Description'
    ,'InterviewConcurrencyToken of the risk, current interview token for TALUS interview'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'InterviewConcurrencyToken'   

exec sp_addextendedproperty 'MS_Description'
    ,'InterviewTemplateVersion of the risk, current interview version for TALUS interview'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'InterviewTemplateVersion'   

exec sp_addextendedproperty 'MS_Description'
    ,'PartyId of the risk, links to Party table'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'PartyId'  

exec sp_addextendedproperty 'MS_Description'
    ,'PolicyId of the risk, links to Policy table'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'PolicyId'  

exec sp_addextendedproperty 'MS_Description'
    ,'LprBeneficiary applies for the risk'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'LprBeneficiary'  

exec sp_addextendedproperty 'MS_Description'
    ,'Current Premium for the risk'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'Premium'  

exec sp_addextendedproperty 'MS_Description'
    ,'Current MultiPlanDiscount for the risk'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'MultiPlanDiscount'  

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'ModifiedTS'  

