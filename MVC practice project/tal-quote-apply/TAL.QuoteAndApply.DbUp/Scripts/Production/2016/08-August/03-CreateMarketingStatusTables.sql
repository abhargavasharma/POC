SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MarketingStatus](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MarketingStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [MarketingStatus] ([Id], [Description]) VALUES (0, 'Unknown');
INSERT INTO [MarketingStatus] ([Id], [Description]) VALUES (1, 'Lead');
INSERT INTO [MarketingStatus] ([Id], [Description]) VALUES (2, 'Accept');
INSERT INTO [MarketingStatus] ([Id], [Description]) VALUES (3, 'Refer');
INSERT INTO [MarketingStatus] ([Id], [Description]) VALUES (4, 'Off');
INSERT INTO [MarketingStatus] ([Id], [Description]) VALUES (5, 'Ineligible');
INSERT INTO [MarketingStatus] ([Id], [Description]) VALUES (6, 'Decline');

GO

CREATE TABLE [dbo].[RiskMarketingStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RiskId] INT NOT NULL FOREIGN KEY REFERENCES [Risk]([Id]),
	[MarketingStatusId] INT NOT NULL FOREIGN KEY REFERENCES [MarketingStatus]([Id]),
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_RiskMarketingStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about the Risk Marketing Status'
    ,'user', 'dbo', 'table'
    ,'RiskMarketingStatus'

exec sp_addextendedproperty 'MS_Description'
    ,'Primary key, auto incrementing'
    ,'user', 'dbo'
    ,'table', 'RiskMarketingStatus'
    ,'column', 'Id'            

exec sp_addextendedproperty 'MS_Description'
    ,'Unique external identifier for the risk record'
    ,'user', 'dbo'
    ,'table', 'RiskMarketingStatus'
    ,'column', 'RiskId'   

exec sp_addextendedproperty 'MS_Description'
    ,'Total premium for this policy'
    ,'user', 'dbo'
    ,'table', 'RiskMarketingStatus'
    ,'column', 'MarketingStatusId'  

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'RiskMarketingStatus'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'RiskMarketingStatus'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'RiskMarketingStatus'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'RiskMarketingStatus'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'RiskMarketingStatus'
    ,'column', 'ModifiedTS'  

GO

CREATE TABLE [dbo].[PlanMarketingStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlanId] INT NOT NULL FOREIGN KEY REFERENCES [Plan]([Id]),
	[MarketingStatusId] INT NOT NULL FOREIGN KEY REFERENCES [MarketingStatus]([Id]),
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_PlanMarketingStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about the Plan Marketing Status'
    ,'user', 'dbo', 'table'
    ,'PlanMarketingStatus'

exec sp_addextendedproperty 'MS_Description'
    ,'Primary key, auto incrementing'
    ,'user', 'dbo'
    ,'table', 'PlanMarketingStatus'
    ,'column', 'Id'            

exec sp_addextendedproperty 'MS_Description'
    ,'Unique external identifier for the plan record'
    ,'user', 'dbo'
    ,'table', 'PlanMarketingStatus'
    ,'column', 'PlanId'   

exec sp_addextendedproperty 'MS_Description'
    ,'Total premium for this policy'
    ,'user', 'dbo'
    ,'table', 'PlanMarketingStatus'
    ,'column', 'MarketingStatusId'  

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'PlanMarketingStatus'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'PlanMarketingStatus'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'PlanMarketingStatus'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'PlanMarketingStatus'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'PlanMarketingStatus'
    ,'column', 'ModifiedTS'  

	GO

CREATE TABLE [dbo].[CoverMarketingStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CoverId] INT NOT NULL FOREIGN KEY REFERENCES [Cover]([Id]),
	[MarketingStatusId] INT NOT NULL FOREIGN KEY REFERENCES [MarketingStatus]([Id]),
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_CoverMarketingStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about the Cover Marketing Status'
    ,'user', 'dbo', 'table'
    ,'CoverMarketingStatus'

exec sp_addextendedproperty 'MS_Description'
    ,'Primary key, auto incrementing'
    ,'user', 'dbo'
    ,'table', 'CoverMarketingStatus'
    ,'column', 'Id'            

exec sp_addextendedproperty 'MS_Description'
    ,'Unique external identifier for the cover record'
    ,'user', 'dbo'
    ,'table', 'CoverMarketingStatus'
    ,'column', 'CoverId'   

exec sp_addextendedproperty 'MS_Description'
    ,'Total premium for this policy'
    ,'user', 'dbo'
    ,'table', 'CoverMarketingStatus'
    ,'column', 'MarketingStatusId'   

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'CoverMarketingStatus'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'CoverMarketingStatus'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'CoverMarketingStatus'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'CoverMarketingStatus'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'CoverMarketingStatus'
    ,'column', 'ModifiedTS' 