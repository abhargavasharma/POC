SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Plan](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PolicyId] [int] NOT NULL FOREIGN KEY REFERENCES [Policy]([Id]),
	[RiskId] [int] NOT NULL FOREIGN KEY REFERENCES [Risk]([Id]),
	[ParentPlanId] [int] NULL FOREIGN KEY REFERENCES [Plan]([Id]),
	[Code] [nvarchar](10) NOT NULL,
	[CoverAmount] DECIMAL(18,2) NULL,
	[Selected] [bit] NULL,
	[Premium] DECIMAL(18,2) NULL,
	[MultiPlanDiscount] DECIMAL(18,2) NULL,
	[MultiCoverDiscount] DECIMAL(18,2) NULL,
	[PremiumTypeId] INT NULL FOREIGN KEY REFERENCES [PremiumType]([Id]),
	[LinkedToCpi] [bit] NULL,
	[PremiumHoliday] [bit] NULL,
	[WaitingPeriod] [int] NULL,
	[BenefitPeriod] [int] NULL,
	[OccupationDefinition] [int] NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_Plan] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a Plan'
    ,'user', 'dbo', 'table'
    ,'Plan'

exec sp_addextendedproperty 'MS_Description'
    ,'Primary key, auto incrementing'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'Id'

exec sp_addextendedproperty 'MS_Description'
    ,'PolicyId links to the policy table'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'PolicyId'

exec sp_addextendedproperty 'MS_Description'
    ,'RiskId links to the risk table'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'RiskId'

exec sp_addextendedproperty 'MS_Description'
    ,'ParentPlanId links to this table, set if this plan is a rider'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'ParentPlanId'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique code for this plan'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'Code'

exec sp_addextendedproperty 'MS_Description'
    ,'CoverAmount selected for the plan'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'CoverAmount'

exec sp_addextendedproperty 'MS_Description'
    ,'True/False for if this plan has been selected'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'Selected'

exec sp_addextendedproperty 'MS_Description'
    ,'Current premium of the plan'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'Premium'

exec sp_addextendedproperty 'MS_Description'
    ,'Current MultiPlanDiscount of the plan'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'MultiPlanDiscount'

exec sp_addextendedproperty 'MS_Description'
    ,'Current MultiCoverDiscount of the plan'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'MultiCoverDiscount'

exec sp_addextendedproperty 'MS_Description'
    ,'PremiumType of the plan, links to premium type table'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'PremiumTypeId'

exec sp_addextendedproperty 'MS_Description'
    ,'True/False if plan is linked to CPI'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'LinkedToCpi'

exec sp_addextendedproperty 'MS_Description'
    ,'True/False if plan is has premium holiday enabled'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'PremiumHoliday'

exec sp_addextendedproperty 'MS_Description'
    ,'WaitingPeriod of the plan'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'WaitingPeriod'

exec sp_addextendedproperty 'MS_Description'
    ,'BenefitPeriod of the plan'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'BenefitPeriod'

exec sp_addextendedproperty 'MS_Description'
    ,'OccupationDefinition of the plan'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'OccupationDefinition'

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'Plan'
    ,'column', 'ModifiedTS'  
