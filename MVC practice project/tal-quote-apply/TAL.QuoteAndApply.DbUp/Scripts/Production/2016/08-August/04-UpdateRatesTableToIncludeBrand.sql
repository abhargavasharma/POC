SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

----------------------------------------------------

ALTER TABLE [dbo].[OccupationClassFactor]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_OccupationClassFactor_GenderId_OccupationClass_PlanCode] ON [dbo].[OccupationClassFactor]

 GO

 CREATE UNIQUE NONCLUSTERED INDEX [IX_OccupationClassFactor_GenderId_OccupationClass_PlanCode_Brand] ON [dbo].[OccupationClassFactor]
(
	[GenderId] ASC,
	[OccupationClass] ASC,
	[PlanCode] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

----------------------------------------------------

ALTER TABLE [dbo].[SmokerFactor]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_SmokerFactor_Smoker_PlanCode] ON [dbo].[SmokerFactor]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_SmokerFactor_Smoker_PlanCode_Brand] ON [dbo].[SmokerFactor]
(
	[Smoker] ASC,
	[PlanCode] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

----------------------------------------------------

ALTER TABLE [dbo].[LargeSumInsuredDiscountFactor]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_LargeSumInsuredDiscountFactor_PlanCode_MinSumInsured_MaxSumInsured] ON [dbo].[LargeSumInsuredDiscountFactor]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_LargeSumInsuredDiscountFactor_PlanCode_Brand_MinSumInsured_MaxSumInsured] ON [dbo].[LargeSumInsuredDiscountFactor]
(
	[PlanCode] ASC,
	[BrandId] ASC,
	[MinSumInsured] ASC,
	[MaxSumInsured] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

----------------------------------------------------

ALTER TABLE [dbo].[WaitingPeriodFactor]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_WaitingPeriodFactor_PlanCode_WaitingPeriod] ON [dbo].[WaitingPeriodFactor]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_WaitingPeriodFactor_PlanCode_Brand_WaitingPeriod] ON [dbo].[WaitingPeriodFactor]
(
	[PlanCode] ASC,
	[BrandId] ASC,
	[WaitingPeriod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

----------------------------------------------------

ALTER TABLE [dbo].[OccupationMapping]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_OccupationMapping_OccupationClass] ON [dbo].[OccupationMapping]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_OccupationMapping_OccupationClass_Brand] ON [dbo].[OccupationMapping]
(
	[OccupationClass] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

----------------------------------------------------

ALTER TABLE [dbo].[CoverBaseRate]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_CoverBaseRate_Plan_Cover_Age_Gender_Smoker_PremiumType_BuyBack_Waiting_Benefit] ON [dbo].[CoverBaseRate]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_CoverBaseRate_Plan_Brand_Cover_Age_Gender_Smoker_PremiumType_BuyBack_Waiting_Benefit] ON [dbo].[CoverBaseRate]
(
	[CoverCode] ASC,
	[PlanCode] ASC,
	[Age] ASC,
	[GenderId] ASC,
	[IsSmoker] ASC,
	[OccupationGroup] ASC,
	[PremiumTypeId] ASC,
	[BuyBack] ASC,
	[WaitingPeriod] ASC,
	[BenefitPeriod] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

----------------------------------------------------

ALTER TABLE [dbo].[CoverDivisionalFactor]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_CoverDivisionalFactor_CoverCode] ON [dbo].[CoverDivisionalFactor]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_CoverDivisionalFactor_CoverCode_Brand] ON [dbo].[CoverDivisionalFactor]
(
	[CoverCode] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

---------------------------------------------------- MAY BE BROKEN ON DROPPING INDEX

ALTER TABLE [dbo].[MultiPlanDiscountFactor]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_MultiPlanDiscountFactor_PlanCount] ON [dbo].[MultiPlanDiscountFactor]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_MultiPlanDiscountFactor_PlanCount_Brand] ON [dbo].[MultiPlanDiscountFactor]
(
	[PlanCount] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

----------------------------------------------------

ALTER TABLE [dbo].[ModalFrequencyFactor]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_ModalFrequencyFactor_PremiumFrequencyId] ON [dbo].[ModalFrequencyFactor]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ModalFrequencyFactor_PremiumFrequencyId_Brand] ON [dbo].[ModalFrequencyFactor]
(
	[PremiumFrequencyId] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

----------------------------------------------------

ALTER TABLE [dbo].[MultiCoverDiscountFactor]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_MultiCoverDiscountFactor_PlanCode_SelectedCoverCodes] ON [dbo].[MultiCoverDiscountFactor]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_MultiCoverDiscountFactor_PlanCode_SelectedCoverCodes_Brand] ON [dbo].[MultiCoverDiscountFactor]
(
	[PlanCode] ASC,
	[SelectedCoverCodes] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

----------------------------------------------------

ALTER TABLE [dbo].[PremiumReliefFactor]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_PremiumReliefFactor_Selected] ON [dbo].[PremiumReliefFactor]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_PremiumReliefFactor_Selected_Brand] ON [dbo].[PremiumReliefFactor]
(
	[Selected] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

----------------------------------------------------

ALTER TABLE [dbo].[PerMilleLoadingFactor]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_PerMilleLoadingFactor_CoverCode] ON [dbo].[PerMilleLoadingFactor]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_PerMilleLoadingFactor_CoverCode_Brand] ON [dbo].[PerMilleLoadingFactor]
(
	[CoverCode] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

---------------------------------------------------- MAY HAVE ERROR DROPPING

ALTER TABLE [dbo].[PercentageLoadingFactor]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_PercentageLoadingFactor_CoverCode] ON [dbo].[PercentageLoadingFactor]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_PercentageLoadingFactor_CoverCode_Brand] ON [dbo].[PercentageLoadingFactor]
(
	[CoverCode] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

----------------------------------------------------

ALTER TABLE [dbo].[IndemnityFactor]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_IndemnityFactor_PlanCode] ON [dbo].[IndemnityFactor]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_IndemnityFactor_PlanCode_Brand] ON [dbo].[IndemnityFactor]
(
	[PlanCode] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

----------------------------------------------------

ALTER TABLE [dbo].[IncreasingClaimsFactor]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_IncreasingClaimsFactor_PlanCode_BenefitPeriod_IncreasingClaimsEnabled] ON [dbo].[IncreasingClaimsFactor]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_IncreasingClaimsFactor_PlanCode_BenefitPeriod_IncreasingClaimsEnabled_Brand] ON [dbo].[IncreasingClaimsFactor]
(
	[PlanCode] ASC,
	[BenefitPeriod] ASC,
	[IncreasingClaimsEnabled] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

----------------------------------------------------

ALTER TABLE [dbo].[DayOneAccidentBaseRate]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_DayOneAccidentBaseRate_PlanCode_CoverCode_Age_Gender_PremiumType_WaitingPeriod] ON [dbo].[DayOneAccidentBaseRate]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_DayOneAccidentBaseRate_PlanCode_CoverCode_Age_Gender_PremiumType_WaitingPeriod_Brand] ON [dbo].[DayOneAccidentBaseRate]
(
	[PlanCode] ASC,
	[CoverCode] ASC,
	[Age] ASC,
	[GenderId] ASC,
	[PremiumTypeId] ASC,
	[WaitingPeriod] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

----------------------------------------------------

ALTER TABLE [dbo].[PlanMinimumCoverAmountForMultiPlanDiscount]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_PlanMinimumCoverAmountForMultiPlanDiscount_PlanCode_MinimumCoverAmount] ON [dbo].[PlanMinimumCoverAmountForMultiPlanDiscount]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_PlanMinimumCoverAmountForMultiPlanDiscount_PlanCode_MinimumCoverAmount_Brand] ON [dbo].[PlanMinimumCoverAmountForMultiPlanDiscount]
(
	[PlanCode] ASC,
	[MinimumCoverAmount] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

----------------------------------------------------

ALTER TABLE [dbo].[OccupationDefinitionTypeFactor]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

DROP INDEX [IX_OccupationDefinitionTypeFactor_OccupationDefinitionTypeID] ON [dbo].[OccupationDefinitionTypeFactor]

 GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_OccupationDefinitionTypeFactor_OccupationDefinitionTypeID_Brand] ON [dbo].[OccupationDefinitionTypeFactor]
(
	[OccupationDefinitionTypeID] ASC,
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

--------------------------------------

exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'OccupationClassFactor'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'SmokerFactor'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'LargeSumInsuredDiscountFactor'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'WaitingPeriodFactor'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'OccupationMapping'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'CoverBaseRate'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'CoverDivisionalFactor'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'MultiPlanDiscountFactor'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'ModalFrequencyFactor'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'MultiCoverDiscountFactor'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'PremiumReliefFactor'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'PerMilleLoadingFactor'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'PercentageLoadingFactor'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'IndemnityFactor'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'IncreasingClaimsFactor'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'DayOneAccidentBaseRate'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'PlanMinimumCoverAmountForMultiPlanDiscount'
    ,'column', 'BrandId'


exec sp_addextendedproperty 'MS_Description'
    ,'The Brand for which this factor applies to'
    ,'user', 'dbo'
    ,'table', 'OccupationDefinitionTypeFactor'
    ,'column', 'BrandId'

