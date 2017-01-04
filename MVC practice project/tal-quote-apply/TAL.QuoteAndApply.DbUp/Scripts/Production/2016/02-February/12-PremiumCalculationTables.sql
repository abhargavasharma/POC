SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OccupationClassFactor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GenderId] INT NOT NULL FOREIGN KEY REFERENCES [Gender] ([Id]),
	[OccupationClass] NVARCHAR(20) NOT NULL,
	[PlanCode] NVARCHAR(10) NOT NULL,
	[Factor] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_OccupationClassFactor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_OccupationClassFactor_GenderId_OccupationClass_PlanCode] ON [dbo].[OccupationClassFactor]
(
	[GenderId] ASC,
	[OccupationClass] ASC,
	[PlanCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(1, 'AAA', 'IP', 1.2, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 'AAA', 'IP', 1.2, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(1, 'AA+', 'IP', 1.2, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 'AA+', 'IP', 1.2, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(1, 'AA', 'IP', 1.7, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 'AA', 'IP', 1.7, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(1, 'A', 'IP', 1.7, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 'A', 'IP', 1.7, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(1, 'BBB', 'IP', 2.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 'BBB', 'IP', 2.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(1, 'BB+', 'IP', 3.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 'BB+', 'IP', 3.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(1, 'BB', 'IP', 3.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 'BB', 'IP', 3.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(1, 'B', 'IP', 3.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 'B', 'IP', 3.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(1, 'SRA', 'IP', 6.0, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 'SRA', 'IP', 6.0, 'SYSTEM', GETDATE())

GO

CREATE TABLE [dbo].[SmokerFactor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Smoker] BIT,
	[PlanCode] NVARCHAR(10) NOT NULL,
	[Factor] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_SmokerFactor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_SmokerFactor_Smoker_PlanCode] ON [dbo].[SmokerFactor]
(
	[Smoker] ASC,
	[PlanCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


GO
INSERT INTO [SmokerFactor] ([Smoker],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(0, 'IP', 1, 'SYSTEM', GETDATE())
INSERT INTO [SmokerFactor] ([Smoker],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(1, 'IP', 1.2625, 'SYSTEM', GETDATE())

GO

CREATE TABLE [dbo].[LargeSumInsuredDiscountFactor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MinSumInsured] DECIMAL(18,2) NOT NULL,
	[MaxSumInsured] DECIMAL(18,2) NOT NULL,
	[PlanCode] NVARCHAR(10) NOT NULL,
	[Factor] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_LargeSumInsuredDiscountFactor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_LargeSumInsuredDiscountFactor_PlanCode_MinSumInsured_MaxSumInsured] ON [dbo].[LargeSumInsuredDiscountFactor]
(
	[PlanCode] ASC,
	[MinSumInsured] ASC,
	[MaxSumInsured] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

--life
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(0, 249999, 'DTH', 1, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(250000, 499999, 'DTH', 0.940, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(500000, 749999, 'DTH', 0.780, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(750000, 999999, 'DTH', 0.760, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(1000000, 1999999, 'DTH', 0.680, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2000000, 2999999, 'DTH', 0.670, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(3000000, 3999999, 'DTH', 0.660, 'SYSTEM', GETDATE())

--tpd
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(0, 249999, 'TPS', 1, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(250000, 499999, 'TPS', 0.900, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(500000, 749999, 'TPS', 0.800, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(750000, 999999, 'TPS', 0.775, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(1000000, 1499999, 'TPS', 0.775, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(1500000, 1999999, 'TPS', 0.745, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2000000, 2999999, 'TPS', 0.700, 'SYSTEM', GETDATE())

--tpd rider
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(0, 249999, 'TPDDTH', 1, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(250000, 499999, 'TPDDTH', 0.900, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(500000, 749999, 'TPDDTH', 0.800, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(750000, 999999, 'TPDDTH', 0.775, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(1000000, 1499999, 'TPDDTH', 0.775, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(1500000, 1999999, 'TPDDTH', 0.745, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2000000, 2999999, 'TPDDTH', 0.700, 'SYSTEM', GETDATE())

--CI
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(0, 249999, 'TRS', 1, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(250000, 499999, 'TRS', 0.950, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(500000, 749999, 'TRS', 0.900, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(750000, 3000000, 'TRS', 0.850, 'SYSTEM', GETDATE())

--CI rider
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(0, 249999, 'TRADTH', 1, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(250000, 499999, 'TRADTH', 0.950, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(500000, 3000000, 'TRADTH', 0.900, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(500000, 749999, 'TRADTH', 0.900, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(750000, 3000000, 'TRADTH', 0.850, 'SYSTEM', GETDATE())

--ip
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(0, 3999, 'IP', 1, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(4000, 6999, 'IP', 0.95, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(7000, 12500, 'IP', 0.91, 'SYSTEM', GETDATE())

GO

CREATE TABLE [dbo].[WaitingPeriodFactor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WaitingPeriod] INT NOT NULL,
	[PlanCode] NVARCHAR(10) NOT NULL,
	[Factor] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_WaitingPeriodFactor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_WaitingPeriodFactor_PlanCode_WaitingPeriod] ON [dbo].[WaitingPeriodFactor]
(
	[PlanCode] ASC,
	[WaitingPeriod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


GO
INSERT INTO [WaitingPeriodFactor] ([WaitingPeriod],[PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'IP', 1, 'SYSTEM', GETDATE())
INSERT INTO [WaitingPeriodFactor] ([WaitingPeriod],[PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(4, 'IP', 1, 'SYSTEM', GETDATE())
INSERT INTO [WaitingPeriodFactor] ([WaitingPeriod],[PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(8, 'IP', 1, 'SYSTEM', GETDATE())
INSERT INTO [WaitingPeriodFactor] ([WaitingPeriod],[PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(13, 'IP', 1, 'SYSTEM', GETDATE())
INSERT INTO [WaitingPeriodFactor] ([WaitingPeriod],[PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(26, 'IP', 0.85, 'SYSTEM', GETDATE())
INSERT INTO [WaitingPeriodFactor] ([WaitingPeriod],[PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(52, 'IP', 0.75, 'SYSTEM', GETDATE())
INSERT INTO [WaitingPeriodFactor] ([WaitingPeriod],[PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(104, 'IP', 0.7, 'SYSTEM', GETDATE())

GO

CREATE TABLE [dbo].[OccupationMapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OccupationClass] NVARCHAR(20) NOT NULL,
	[OccupationGroup] NVARCHAR(128) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_OccupationMapping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_OccupationMapping_OccupationClass] ON [dbo].[OccupationMapping]
(
	[OccupationClass] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


GO

INSERT INTO [OccupationMapping] ([OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES('AAA', 'WC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES('AA+', 'WC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES('AA', 'WC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES('A', 'WC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES('BBB', 'BC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES('BB+', 'BC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES('BB', 'BC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES('B', 'BC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES('SRA', 'BC', 'SYSTEM', GETDATE())

GO


CREATE TABLE [dbo].[CoverBaseRate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlanCode] NVARCHAR(10) NOT NULL,
	[CoverCode] NVARCHAR(10) NOT NULL,
	[Age] INT NOT NULL,
	[GenderId] INT NOT NULL  FOREIGN KEY REFERENCES [Gender] ([Id]),
	[IsSmoker] BIT NULL,
	[PremiumTypeId] INT NOT NULL  FOREIGN KEY REFERENCES [PremiumType] ([Id]),
	[OccupationGroup] NVARCHAR(128) NULL,
	[BenefitPeriod] INT NULL,
	[WaitingPeriod] INT NULL,
	[BuyBack] BIT NULL,
	[BaseRate] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_CoverBaseRate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_CoverBaseRate_Plan_Cover_Age_Gender_Smoker_PremiumType_BuyBack_Waiting_Benefit] ON [dbo].[CoverBaseRate]
(
	[PlanCode] ASC,
	[CoverCode] ASC,
	[Age] ASC,
	[GenderId] ASC,
	[IsSmoker] ASC,
	[OccupationGroup] ASC,
	[PremiumTypeId] ASC,
	[BuyBack] ASC,
	[WaitingPeriod] ASC,
	[BenefitPeriod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


GO


CREATE TABLE [dbo].[CoverDivisionalFactor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CoverCode] NVARCHAR(10) NOT NULL,
	[DivisionalFactor] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_CoverDivisionalFactor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_CoverDivisionalFactor_CoverCode] ON [dbo].[CoverDivisionalFactor]
(
	[CoverCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

INSERT INTO [CoverDivisionalFactor] ([CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES('DTHAC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES('DTHIC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES('TPDDTHAC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES('TPDDTHIC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES('TRADTHSIN', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES('TRADTHCC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES('TRADTHSIC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES('TRSSIN', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES('TRSCC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES('TRSSIC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES('TPSAC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES('TPSIC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES('IPSAC', 1000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES('IPSIC', 1000, 'SYSTEM', GETDATE())
GO

CREATE TABLE [dbo].[MultiPlanDiscountFactor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlanCount] INT NOT NULL,
	[Factor] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_MultiPlanDiscountFactor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_MultiPlanDiscountFactor_PlanCount] ON [dbo].[MultiPlanDiscountFactor]
(
	[PlanCount] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


GO

INSERT INTO [MultiPlanDiscountFactor] ([PlanCount],[Factor],[CreatedBy],[CreatedTS])  VALUES(0, 1, 'SYSTEM', GETDATE())
INSERT INTO [MultiPlanDiscountFactor] ([PlanCount],[Factor],[CreatedBy],[CreatedTS])  VALUES(1, 1, 'SYSTEM', GETDATE())
INSERT INTO [MultiPlanDiscountFactor] ([PlanCount],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, .950, 'SYSTEM', GETDATE())
INSERT INTO [MultiPlanDiscountFactor] ([PlanCount],[Factor],[CreatedBy],[CreatedTS])  VALUES(3, .900, 'SYSTEM', GETDATE())
INSERT INTO [MultiPlanDiscountFactor] ([PlanCount],[Factor],[CreatedBy],[CreatedTS])  VALUES(4, .850, 'SYSTEM', GETDATE())
GO

CREATE TABLE [dbo].[ModalFrequencyFactor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PremiumFrequencyId] INT NOT NULL FOREIGN KEY REFERENCES [PremiumFrequency]([Id]),
	[Factor] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_ModalFrequencyFactor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ModalFrequencyFactor_PremiumFrequencyId] ON [dbo].[ModalFrequencyFactor]
(
	[PremiumFrequencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

INSERT INTO [ModalFrequencyFactor] ([PremiumFrequencyId],[Factor],[CreatedBy],[CreatedTS])  VALUES(1, 1, 'SYSTEM', GETDATE())
INSERT INTO [ModalFrequencyFactor] ([PremiumFrequencyId],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 3.0, 'SYSTEM', GETDATE())
INSERT INTO [ModalFrequencyFactor] ([PremiumFrequencyId],[Factor],[CreatedBy],[CreatedTS])  VALUES(3, 5.5, 'SYSTEM', GETDATE())
INSERT INTO [ModalFrequencyFactor] ([PremiumFrequencyId],[Factor],[CreatedBy],[CreatedTS])  VALUES(4, 11, 'SYSTEM', GETDATE())

GO
PRINT('MultiCoverDiscountFactor') 

CREATE TABLE [dbo].[MultiCoverDiscountFactor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlanCode] NVARCHAR(10),
	[SelectedCoverCodes] NVARCHAR(30),
	[Factor] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_MultiCoverDiscountFactor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_MultiCoverDiscountFactor_PlanCode_SelectedCoverCodes] ON [dbo].[MultiCoverDiscountFactor]
(
	[PlanCode] ASC,
	[SelectedCoverCodes] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

INSERT INTO [MultiCoverDiscountFactor] ([PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES('DTH', 'DTHAC|DTHIC', -0.1667, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES('TPDDTH', 'TPDDTHAC|TPDDTHIC', -0.1667, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES('TPS', 'TPSAC|TPSIC',  -0.1667, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES('IP', 'IPSAC|IPSIC', -0.1667, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES('TRADTH', 'TRADTHCC|TRADTHSIC|TRADTHSIN', -0.3750, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES('TRADTH', 'TRADTHCC|TRADTHSIC', -0.3750, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES('TRADTH', 'TRADTHCC|TRADTHSIN', -0.3750, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES('TRADTH', 'TRADTHSIC|TRADTHSIN', -0.3750, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES('TRS', 'TRSCC|TRSSIC|TRSSIN', -0.3750, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES('TRS', 'TRSCC|TRSSIC', -0.3750, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES('TRS', 'TRSCC|TRSSIN', -0.3750, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES('TRS', 'TRSSIC|TRSSIN', -0.3750, 'SYSTEM', GETDATE())

GO

CREATE TABLE [dbo].[PremiumReliefFactor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Selected] BIT NOT NULL,
	[Factor] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_PremiumReliefFactor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_PremiumReliefFactor_Selected] ON [dbo].[PremiumReliefFactor]
(
	[Selected] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

INSERT INTO [PremiumReliefFactor] ([Selected],[Factor],[CreatedBy],[CreatedTS])  VALUES(1, 1.07, 'SYSTEM', GETDATE())
INSERT INTO [PremiumReliefFactor] ([Selected],[Factor],[CreatedBy],[CreatedTS])  VALUES(0, 1, 'SYSTEM', GETDATE())

GO

CREATE TABLE [dbo].[PerMilleLoadingFactor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CoverCode] NVARCHAR(10),
	[Factor] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_PerMilleLoadingFactor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_PerMilleLoadingFactor_CoverCode] ON [dbo].[PerMilleLoadingFactor]
(
	[CoverCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO


INSERT INTO [PerMilleLoadingFactor] ([CoverCode],[Factor],[CreatedBy],[CreatedTS])  VALUES('DTHASC', 0.05, 'SYSTEM', GETDATE())
INSERT INTO [PerMilleLoadingFactor] ([CoverCode],[Factor],[CreatedBy],[CreatedTS])  VALUES('TPSASC', 0.05, 'SYSTEM', GETDATE())
INSERT INTO [PerMilleLoadingFactor] ([CoverCode],[Factor],[CreatedBy],[CreatedTS])  VALUES('TPDDTHASC', 0.05, 'SYSTEM', GETDATE())

GO

CREATE TABLE [dbo].[PercentageLoadingFactor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CoverCode] NVARCHAR(10),
	[Factor] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_PercentageLoadingFactor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_PercentageLoadingFactor_CoverCode] ON [dbo].[PercentageLoadingFactor]
(
	[CoverCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

INSERT INTO [PercentageLoadingFactor] ([CoverCode],[Factor],[CreatedBy],[CreatedTS])  VALUES('IPSSC', .01, 'SYSTEM', GETDATE())

GO

CREATE TABLE [dbo].[IndemnityFactor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlanCode] NVARCHAR(10),
	[Factor] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_IndemnityFactor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_IndemnityFactor_PlanCode] ON [dbo].[IndemnityFactor]
(
	[PlanCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

INSERT INTO [IndemnityFactor] ([PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES('IP', 0.85, 'SYSTEM', GETDATE())

GO

CREATE TABLE [dbo].[IncreasingClaimsFactor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlanCode] NVARCHAR(10),
	[BenefitPeriod] INT NOT NULL,
	[IncreasingClaimsEnabled] BIT NOT NULL,
	[Factor] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_IncreasingClaimsFactor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_IncreasingClaimsFactor_PlanCode_BenefitPeriod_IncreasingClaimsEnabled] ON [dbo].[IncreasingClaimsFactor]
(
	[PlanCode] ASC,
	[BenefitPeriod] ASC,
	[IncreasingClaimsEnabled] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

INSERT INTO [IncreasingClaimsFactor] ([PlanCode],[BenefitPeriod],[IncreasingClaimsEnabled],[Factor],[CreatedBy],[CreatedTS])  VALUES('IP', 1, 0, 1, 'SYSTEM', GETDATE())
INSERT INTO [IncreasingClaimsFactor] ([PlanCode],[BenefitPeriod],[IncreasingClaimsEnabled],[Factor],[CreatedBy],[CreatedTS])  VALUES('IP', 2, 0, 1, 'SYSTEM', GETDATE())
INSERT INTO [IncreasingClaimsFactor] ([PlanCode],[BenefitPeriod],[IncreasingClaimsEnabled],[Factor],[CreatedBy],[CreatedTS])  VALUES('IP', 5, 0, 1, 'SYSTEM', GETDATE())
INSERT INTO [IncreasingClaimsFactor] ([PlanCode],[BenefitPeriod],[IncreasingClaimsEnabled],[Factor],[CreatedBy],[CreatedTS])  VALUES('IP', 1, 1, 1.02, 'SYSTEM', GETDATE())
INSERT INTO [IncreasingClaimsFactor] ([PlanCode],[BenefitPeriod],[IncreasingClaimsEnabled],[Factor],[CreatedBy],[CreatedTS])  VALUES('IP', 2, 1, 1.03, 'SYSTEM', GETDATE())
INSERT INTO [IncreasingClaimsFactor] ([PlanCode],[BenefitPeriod],[IncreasingClaimsEnabled],[Factor],[CreatedBy],[CreatedTS])  VALUES('IP', 5, 1, 1.04, 'SYSTEM', GETDATE())

GO

CREATE TABLE [dbo].[DayOneAccidentBaseRate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlanCode] NVARCHAR(10) NOT NULL,
	[CoverCode] NVARCHAR(10) NOT NULL,
	[Age] INT NOT NULL,
	[GenderId] INT NOT NULL  FOREIGN KEY REFERENCES [Gender] ([Id]),
	[PremiumTypeId] INT NOT NULL  FOREIGN KEY REFERENCES [PremiumType] ([Id]),
	[WaitingPeriod] INT NULL,
	[BaseRate] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_DayOneAccidentBaseRate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_DayOneAccidentBaseRate_PlanCode_CoverCode_Age_Gender_PremiumType_WaitingPeriod] ON [dbo].[DayOneAccidentBaseRate]
(
	[PlanCode] ASC,
	[CoverCode] ASC,
	[Age] ASC,
	[GenderId] ASC,
	[PremiumTypeId] ASC,
	[WaitingPeriod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

CREATE TABLE [dbo].[PlanMinimumCoverAmountForMultiPlanDiscount](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlanCode] NVARCHAR(10),
	[MinimumCoverAmount] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_PlanMinimumCoverAmountForMultiPlanDiscount] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about PlanMinimumCoverAmountForMultiPlanDiscount'
    ,'user', 'dbo', 'table'
    ,'PlanMinimumCoverAmountForMultiPlanDiscount'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the PlanMinimumCoverAmountForMultiPlanDiscount'
    ,'user', 'dbo'
    ,'table', 'PlanMinimumCoverAmountForMultiPlanDiscount'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'PlanCode of the PlanMinimumCoverAmountForMultiPlanDiscount'
    ,'user', 'dbo'
    ,'table', 'PlanMinimumCoverAmountForMultiPlanDiscount'
    ,'column', 'PlanCode'  

exec sp_addextendedproperty 'MS_Description'
    ,'MinimumCoverAmount of the PlanMinimumCoverAmountForMultiPlanDiscount'
    ,'user', 'dbo'
    ,'table', 'PlanMinimumCoverAmountForMultiPlanDiscount'
    ,'column', 'MinimumCoverAmount'
	  
exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'PlanMinimumCoverAmountForMultiPlanDiscount'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'PlanMinimumCoverAmountForMultiPlanDiscount'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'PlanMinimumCoverAmountForMultiPlanDiscount'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'PlanMinimumCoverAmountForMultiPlanDiscount'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'PlanMinimumCoverAmountForMultiPlanDiscount'
    ,'column', 'ModifiedTS'  
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_PlanMinimumCoverAmountForMultiPlanDiscount_PlanCode_MinimumCoverAmount] ON [dbo].[PlanMinimumCoverAmountForMultiPlanDiscount]
(
	[PlanCode] ASC,
	[MinimumCoverAmount] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

INSERT INTO [PlanMinimumCoverAmountForMultiPlanDiscount] ([PlanCode],[MinimumCoverAmount],[CreatedBy],[CreatedTS])  VALUES('DTH', 100000, 'SYSTEM', GETDATE())
INSERT INTO [PlanMinimumCoverAmountForMultiPlanDiscount] ([PlanCode],[MinimumCoverAmount],[CreatedBy],[CreatedTS])  VALUES('TPDDTH', 100000, 'SYSTEM', GETDATE())
INSERT INTO [PlanMinimumCoverAmountForMultiPlanDiscount] ([PlanCode],[MinimumCoverAmount],[CreatedBy],[CreatedTS])  VALUES('TRADTH', 50000, 'SYSTEM', GETDATE())
INSERT INTO [PlanMinimumCoverAmountForMultiPlanDiscount] ([PlanCode],[MinimumCoverAmount],[CreatedBy],[CreatedTS])  VALUES('TPS', 100000, 'SYSTEM', GETDATE())
INSERT INTO [PlanMinimumCoverAmountForMultiPlanDiscount] ([PlanCode],[MinimumCoverAmount],[CreatedBy],[CreatedTS])  VALUES('TRS', 100000, 'SYSTEM', GETDATE())
INSERT INTO [PlanMinimumCoverAmountForMultiPlanDiscount] ([PlanCode],[MinimumCoverAmount],[CreatedBy],[CreatedTS])  VALUES('IP', 2000, 'SYSTEM', GETDATE())

GO

CREATE TABLE [dbo].[OccupationDefinitionTypeFactor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OccupationDefinitionTypeID] INT NOT NULL FOREIGN KEY REFERENCES [OccupationDefinitionType] ([Id]),
	[Factor] DECIMAL(18,4) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_OccupationDefinitionTypeFactor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about PlanMinimumCoverAmountForMultiPlanDiscount'
    ,'user', 'dbo', 'table'
    ,'OccupationDefinitionTypeFactor'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the OccupationDefinitionTypeFactor'
    ,'user', 'dbo'
    ,'table', 'OccupationDefinitionTypeFactor'
    ,'column', 'Id'
	  
exec sp_addextendedproperty 'MS_Description'
    ,'OccupationDefinitionType to look up the factor, Linked to OccupationDefinitionType table'
    ,'user', 'dbo'
    ,'table', 'OccupationDefinitionTypeFactor'
    ,'column', 'OccupationDefinitionTypeID' 

exec sp_addextendedproperty 'MS_Description'
    ,'Factor of the OccupationDefinitionTypeFactor'
    ,'user', 'dbo'
    ,'table', 'OccupationDefinitionTypeFactor'
    ,'column', 'Factor'  	 

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'OccupationDefinitionTypeFactor'
    ,'column', 'RV' 

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'OccupationDefinitionTypeFactor'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'OccupationDefinitionTypeFactor'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'OccupationDefinitionTypeFactor'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'OccupationDefinitionTypeFactor'
    ,'column', 'ModifiedTS'
	
GO
	
CREATE UNIQUE NONCLUSTERED INDEX [IX_OccupationDefinitionTypeFactor_OccupationDefinitionTypeID] ON [dbo].[OccupationDefinitionTypeFactor]
(
	[OccupationDefinitionTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

INSERT INTO [OccupationDefinitionTypeFactor] ([OccupationDefinitionTypeID],[Factor],[CreatedBy],[CreatedTS])  
VALUES(1, 1.00, 'SYSTEM', GETDATE())
INSERT INTO [OccupationDefinitionTypeFactor] ([OccupationDefinitionTypeID],[Factor],[CreatedBy],[CreatedTS])  
VALUES(2, 1.50, 'SYSTEM', GETDATE())

GO
  