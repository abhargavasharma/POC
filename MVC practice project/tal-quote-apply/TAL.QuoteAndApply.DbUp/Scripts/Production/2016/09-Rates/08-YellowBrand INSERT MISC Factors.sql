
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 1, 'AAA', 'IP', 1.2, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 2, 'AAA', 'IP', 1.2, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 1, 'AA+', 'IP', 1.2, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 2, 'AA+', 'IP', 1.2, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 1, 'AA', 'IP', 1.7, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 2, 'AA', 'IP', 1.7, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 1, 'A', 'IP', 1.7, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 2, 'A', 'IP', 1.7, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 1, 'BBB', 'IP', 2.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 2, 'BBB', 'IP', 2.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 1, 'BB+', 'IP', 3.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 2, 'BB+', 'IP', 3.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 1, 'BB', 'IP', 3.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 2, 'BB', 'IP', 3.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 1, 'B', 'IP', 3.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 2, 'B', 'IP', 3.5, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 1, 'SRA', 'IP', 6.0, 'SYSTEM', GETDATE())
INSERT INTO [OccupationClassFactor] ([BrandId], [GenderId], [OccupationClass],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 2, 'SRA', 'IP', 6.0, 'SYSTEM', GETDATE())

GO

INSERT INTO [SmokerFactor] ([BrandId], [Smoker],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 0, 'IP', 1, 'SYSTEM', GETDATE())
INSERT INTO [SmokerFactor] ([BrandId], [Smoker],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) VALUES(2, 1, 'IP', 1.2625, 'SYSTEM', GETDATE())

GO

--life
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 0, 249999, 'DTH', 1, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 250000, 499999, 'DTH', 0.940, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 500000, 749999, 'DTH', 0.780, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 750000, 999999, 'DTH', 0.760, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 1000000, 1999999, 'DTH', 0.680, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 2000000, 2999999, 'DTH', 0.670, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 3000000, 3999999, 'DTH', 0.660, 'SYSTEM', GETDATE())

--tpd
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 0, 249999, 'TPS', 1, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 250000, 499999, 'TPS', 0.900, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 500000, 749999, 'TPS', 0.800, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 750000, 999999, 'TPS', 0.775, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 1000000, 1499999, 'TPS', 0.775, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 1500000, 1999999, 'TPS', 0.745, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 2000000, 2999999, 'TPS', 0.700, 'SYSTEM', GETDATE())

--tpd rider
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 0, 249999, 'TPDDTH', 1, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 250000, 499999, 'TPDDTH', 0.900, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 500000, 749999, 'TPDDTH', 0.800, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 750000, 999999, 'TPDDTH', 0.775, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 1000000, 1499999, 'TPDDTH', 0.775, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 1500000, 1999999, 'TPDDTH', 0.745, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 2000000, 2999999, 'TPDDTH', 0.700, 'SYSTEM', GETDATE())

--CI
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 0, 249999, 'TRS', 1, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 250000, 499999, 'TRS', 0.950, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 500000, 749999, 'TRS', 0.900, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 750000, 3000000, 'TRS', 0.850, 'SYSTEM', GETDATE())

--CI rider
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 0, 249999, 'TRADTH', 1, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 250000, 499999, 'TRADTH', 0.950, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 500000, 3000000, 'TRADTH', 0.900, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 500000, 749999, 'TRADTH', 0.900, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 750000, 3000000, 'TRADTH', 0.850, 'SYSTEM', GETDATE())

--ip
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 0, 3999, 'IP', 1, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 4000, 6999, 'IP', 0.95, 'SYSTEM', GETDATE())
INSERT INTO [LargeSumInsuredDiscountFactor] ([BrandId], [MinSumInsured],[MaxSumInsured],[PlanCode],[Factor],[CreatedBy],[CreatedTS]) 
VALUES(2, 7000, 12500, 'IP', 0.91, 'SYSTEM', GETDATE())


GO
INSERT INTO [WaitingPeriodFactor] ([BrandId], [WaitingPeriod],[PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 2, 'IP', 1, 'SYSTEM', GETDATE())
INSERT INTO [WaitingPeriodFactor] ([BrandId], [WaitingPeriod],[PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 4, 'IP', 1, 'SYSTEM', GETDATE())
INSERT INTO [WaitingPeriodFactor] ([BrandId], [WaitingPeriod],[PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 8, 'IP', 1, 'SYSTEM', GETDATE())
INSERT INTO [WaitingPeriodFactor] ([BrandId], [WaitingPeriod],[PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 13, 'IP', 1, 'SYSTEM', GETDATE())
INSERT INTO [WaitingPeriodFactor] ([BrandId], [WaitingPeriod],[PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 26, 'IP', 0.85, 'SYSTEM', GETDATE())
INSERT INTO [WaitingPeriodFactor] ([BrandId], [WaitingPeriod],[PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 52, 'IP', 0.75, 'SYSTEM', GETDATE())
INSERT INTO [WaitingPeriodFactor] ([BrandId], [WaitingPeriod],[PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 104, 'IP', 0.7, 'SYSTEM', GETDATE())

GO

INSERT INTO [OccupationMapping] ([BrandId], [OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES(2, 'AAA', 'WC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([BrandId], [OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES(2, 'AA+', 'WC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([BrandId], [OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES(2, 'AA', 'WC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([BrandId], [OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES(2, 'A', 'WC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([BrandId], [OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES(2, 'BBB', 'BC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([BrandId], [OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES(2, 'BB+', 'BC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([BrandId], [OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES(2, 'BB', 'BC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([BrandId], [OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES(2, 'B', 'BC', 'SYSTEM', GETDATE())
INSERT INTO [OccupationMapping] ([BrandId], [OccupationClass],[OccupationGroup],[CreatedBy],[CreatedTS])  VALUES(2, 'SRA', 'BC', 'SYSTEM', GETDATE())

GO

INSERT INTO [CoverDivisionalFactor] ([BrandId], [CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES(2, 'DTHAC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([BrandId], [CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES(2, 'DTHIC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([BrandId], [CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES(2, 'TPDDTHAC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([BrandId], [CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES(2, 'TPDDTHIC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([BrandId], [CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES(2, 'TRADTHSIN', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([BrandId], [CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES(2, 'TRADTHCC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([BrandId], [CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES(2, 'TRADTHSIC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([BrandId], [CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES(2, 'TRSSIN', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([BrandId], [CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES(2, 'TRSCC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([BrandId], [CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES(2, 'TRSSIC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([BrandId], [CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES(2, 'TPSAC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([BrandId], [CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES(2, 'TPSIC', 10000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([BrandId], [CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES(2, 'IPSAC', 1000, 'SYSTEM', GETDATE())
INSERT INTO [CoverDivisionalFactor] ([BrandId], [CoverCode],[DivisionalFactor],[CreatedBy],[CreatedTS])  VALUES(2, 'IPSIC', 1000, 'SYSTEM', GETDATE())

GO

INSERT INTO [MultiPlanDiscountFactor] ([BrandId], [PlanCount],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 0, 1, 'SYSTEM', GETDATE())
INSERT INTO [MultiPlanDiscountFactor] ([BrandId], [PlanCount],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 1, 1, 'SYSTEM', GETDATE())
INSERT INTO [MultiPlanDiscountFactor] ([BrandId], [PlanCount],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 2, .950, 'SYSTEM', GETDATE())
INSERT INTO [MultiPlanDiscountFactor] ([BrandId], [PlanCount],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 3, .900, 'SYSTEM', GETDATE())
INSERT INTO [MultiPlanDiscountFactor] ([BrandId], [PlanCount],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 4, .850, 'SYSTEM', GETDATE())

GO

INSERT INTO [ModalFrequencyFactor] ([BrandId], [PremiumFrequencyId],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 1, 1, 'SYSTEM', GETDATE())
INSERT INTO [ModalFrequencyFactor] ([BrandId], [PremiumFrequencyId],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 2, 3.0, 'SYSTEM', GETDATE())
INSERT INTO [ModalFrequencyFactor] ([BrandId], [PremiumFrequencyId],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 3, 5.5, 'SYSTEM', GETDATE())
INSERT INTO [ModalFrequencyFactor] ([BrandId], [PremiumFrequencyId],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 4, 11, 'SYSTEM', GETDATE())

GO


INSERT INTO [MultiCoverDiscountFactor] ([BrandId], [PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'DTH', 'DTHAC|DTHIC', -0.1667, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([BrandId], [PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'TPDDTH', 'TPDDTHAC|TPDDTHIC', -0.1667, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([BrandId], [PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'TPS', 'TPSAC|TPSIC',  -0.1667, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([BrandId], [PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'IP', 'IPSAC|IPSIC', -0.1667, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([BrandId], [PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'TRADTH', 'TRADTHCC|TRADTHSIC|TRADTHSIN', -0.3750, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([BrandId], [PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'TRADTH', 'TRADTHCC|TRADTHSIC', -0.3750, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([BrandId], [PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'TRADTH', 'TRADTHCC|TRADTHSIN', -0.3750, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([BrandId], [PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'TRADTH', 'TRADTHSIC|TRADTHSIN', -0.3750, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([BrandId], [PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'TRS', 'TRSCC|TRSSIC|TRSSIN', -0.3750, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([BrandId], [PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'TRS', 'TRSCC|TRSSIC', -0.3750, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([BrandId], [PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'TRS', 'TRSCC|TRSSIN', -0.3750, 'SYSTEM', GETDATE())
INSERT INTO [MultiCoverDiscountFactor] ([BrandId], [PlanCode],[SelectedCoverCodes],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'TRS', 'TRSSIC|TRSSIN', -0.3750, 'SYSTEM', GETDATE())

GO

INSERT INTO [PremiumReliefFactor] ([BrandId], [Selected],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 1, 1.07, 'SYSTEM', GETDATE())
INSERT INTO [PremiumReliefFactor] ([BrandId], [Selected],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 0, 1, 'SYSTEM', GETDATE())

GO

INSERT INTO [PerMilleLoadingFactor] ([BrandId], [CoverCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'DTHASC', 0.05, 'SYSTEM', GETDATE())
INSERT INTO [PerMilleLoadingFactor] ([BrandId], [CoverCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'TPSASC', 0.05, 'SYSTEM', GETDATE())
INSERT INTO [PerMilleLoadingFactor] ([BrandId], [CoverCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'TPDDTHASC', 0.05, 'SYSTEM', GETDATE())

GO

INSERT INTO [PercentageLoadingFactor] ([BrandId], [CoverCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'IPSSC', .01, 'SYSTEM', GETDATE())

GO

INSERT INTO [IndemnityFactor] ([BrandId], [PlanCode],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'IP', 0.85, 'SYSTEM', GETDATE())

GO

INSERT INTO [IncreasingClaimsFactor] ([BrandId], [PlanCode],[BenefitPeriod],[IncreasingClaimsEnabled],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'IP', 1, 0, 1, 'SYSTEM', GETDATE())
INSERT INTO [IncreasingClaimsFactor] ([BrandId], [PlanCode],[BenefitPeriod],[IncreasingClaimsEnabled],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'IP', 2, 0, 1, 'SYSTEM', GETDATE())
INSERT INTO [IncreasingClaimsFactor] ([BrandId], [PlanCode],[BenefitPeriod],[IncreasingClaimsEnabled],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'IP', 5, 0, 1, 'SYSTEM', GETDATE())
INSERT INTO [IncreasingClaimsFactor] ([BrandId], [PlanCode],[BenefitPeriod],[IncreasingClaimsEnabled],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'IP', 1, 1, 1.02, 'SYSTEM', GETDATE())
INSERT INTO [IncreasingClaimsFactor] ([BrandId], [PlanCode],[BenefitPeriod],[IncreasingClaimsEnabled],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'IP', 2, 1, 1.03, 'SYSTEM', GETDATE())
INSERT INTO [IncreasingClaimsFactor] ([BrandId], [PlanCode],[BenefitPeriod],[IncreasingClaimsEnabled],[Factor],[CreatedBy],[CreatedTS])  VALUES(2, 'IP', 5, 1, 1.04, 'SYSTEM', GETDATE())

GO

INSERT INTO [PlanMinimumCoverAmountForMultiPlanDiscount] ([BrandId], [PlanCode],[MinimumCoverAmount],[CreatedBy],[CreatedTS])  VALUES(2, 'DTH', 100000, 'SYSTEM', GETDATE())
INSERT INTO [PlanMinimumCoverAmountForMultiPlanDiscount] ([BrandId], [PlanCode],[MinimumCoverAmount],[CreatedBy],[CreatedTS])  VALUES(2, 'TPDDTH', 100000, 'SYSTEM', GETDATE())
INSERT INTO [PlanMinimumCoverAmountForMultiPlanDiscount] ([BrandId], [PlanCode],[MinimumCoverAmount],[CreatedBy],[CreatedTS])  VALUES(2, 'TRADTH', 50000, 'SYSTEM', GETDATE())
INSERT INTO [PlanMinimumCoverAmountForMultiPlanDiscount] ([BrandId], [PlanCode],[MinimumCoverAmount],[CreatedBy],[CreatedTS])  VALUES(2, 'TPS', 100000, 'SYSTEM', GETDATE())
INSERT INTO [PlanMinimumCoverAmountForMultiPlanDiscount] ([BrandId], [PlanCode],[MinimumCoverAmount],[CreatedBy],[CreatedTS])  VALUES(2, 'TRS', 100000, 'SYSTEM', GETDATE())
INSERT INTO [PlanMinimumCoverAmountForMultiPlanDiscount] ([BrandId], [PlanCode],[MinimumCoverAmount],[CreatedBy],[CreatedTS])  VALUES(2, 'IP', 2000, 'SYSTEM', GETDATE())

GO


INSERT INTO [OccupationDefinitionTypeFactor] ([BrandId], [OccupationDefinitionTypeID],[Factor],[CreatedBy],[CreatedTS])  
VALUES(2, 1, 1.00, 'SYSTEM', GETDATE())
INSERT INTO [OccupationDefinitionTypeFactor] ([BrandId], [OccupationDefinitionTypeID],[Factor],[CreatedBy],[CreatedTS])  
VALUES(2, 2, 1.50, 'SYSTEM', GETDATE())

GO