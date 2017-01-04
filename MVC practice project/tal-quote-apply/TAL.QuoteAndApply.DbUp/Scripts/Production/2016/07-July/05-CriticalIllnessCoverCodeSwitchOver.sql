DECLARE @OldIllnessCode AS NVARCHAR(10)
DECLARE @OldInjuryCode AS NVARCHAR(10)
DECLARE @OldRiderIllnessCode AS NVARCHAR(10)
DECLARE @OldRiderInjuryCode AS NVARCHAR(10)

SET @OldIllnessCode = 'TRSSIN'
SET @OldInjuryCode = 'TRSSIC'
SET @OldRiderIllnessCode = 'TRADTHSIN'
SET @OldRiderInjuryCode = 'TRADTHSIC'

DECLARE @NewIllnessCode AS NVARCHAR(10)
DECLARE @NewInjuryCode AS NVARCHAR(10)
DECLARE @NewRiderIllnessCode AS NVARCHAR(10)
DECLARE @NewRiderInjuryCode AS NVARCHAR(10)

SET @NewIllnessCode = 'TRSSIC'
SET @NewInjuryCode = 'TRSSIN'
SET @NewRiderIllnessCode = 'TRADTHSIC'
SET @NewRiderInjuryCode = 'TRADTHSIN'

--COVER DIVISIONAL FACTOR

DROP INDEX [IX_CoverDivisionalFactor_CoverCode] ON [dbo].[CoverDivisionalFactor]

DECLARE @OriginalIllnessRecord INT
DECLARE @OriginalInjuryRecord INT
DECLARE @OriginalRiderIllnessRecord INT
DECLARE @OriginalRiderInjuryRecord INT

select @OriginalIllnessRecord = Id from CoverDivisionalFactor where CoverCode = @OldIllnessCode
select @OriginalInjuryRecord = Id from CoverDivisionalFactor where CoverCode = @OldInjuryCode
select @OriginalRiderIllnessRecord = Id from CoverDivisionalFactor where CoverCode = @OldRiderIllnessCode
select @OriginalRiderInjuryRecord = Id from CoverDivisionalFactor where CoverCode = @OldRiderInjuryCode

UPDATE CoverDivisionalFactor SET CoverCode = @NewIllnessCode WHERE Id = @OriginalIllnessRecord
UPDATE CoverDivisionalFactor SET CoverCode = @NewInjuryCode WHERE Id = @OriginalInjuryRecord
UPDATE CoverDivisionalFactor SET CoverCode = @NewRiderIllnessCode WHERE Id = @OriginalRiderIllnessRecord
UPDATE CoverDivisionalFactor SET CoverCode = @NewRiderInjuryCode WHERE Id = @OriginalRiderInjuryRecord

CREATE UNIQUE NONCLUSTERED INDEX [IX_CoverDivisionalFactor_CoverCode] ON [dbo].[CoverDivisionalFactor]
(
	[CoverCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


--COVERS
SELECT Id INTO #IllnessCovers FROM Cover WHERE Code = @OldIllnessCode
SELECT Id INTO #InjuryCovers FROM Cover WHERE Code = @OldInjuryCode
SELECT Id INTO #RiderIllnessCovers FROM Cover WHERE Code = @OldRiderIllnessCode
SELECT Id INTO #RiderInjuryCovers FROM Cover WHERE Code = @OldRiderInjuryCode

UPDATE Cover SET Code = @NewIllnessCode WHERE Id IN (
	SELECT Id FROM #IllnessCovers
)
UPDATE Cover SET Code = @NewInjuryCode WHERE Id IN (
	SELECT Id FROM #InjuryCovers
)
UPDATE Cover SET Code = @NewRiderIllnessCode WHERE Id IN (
	SELECT Id FROM #RiderIllnessCovers
)
UPDATE Cover SET Code = @NewRiderInjuryCode WHERE Id IN (
	SELECT Id FROM #RiderInjuryCovers
)

DROP TABLE #IllnessCovers
DROP TABLE #InjuryCovers
DROP TABLE #RiderIllnessCovers
DROP TABLE #RiderInjuryCovers

-- COVER BASE RATES
DROP INDEX [IX_CoverBaseRate_Plan_Cover_Age_Gender_Smoker_PremiumType_BuyBack_Waiting_Benefit] ON [dbo].[CoverBaseRate]

SELECT Id INTO #IllnessCoverRates FROM CoverBaseRate WHERE CoverCode = @OldIllnessCode
SELECT Id INTO #InjuryCoverRates FROM CoverBaseRate WHERE CoverCode = @OldInjuryCode
SELECT Id INTO #RiderIllnessCoverRates FROM CoverBaseRate WHERE CoverCode = @OldRiderIllnessCode
SELECT Id INTO #RiderInjuryCoverRates FROM CoverBaseRate WHERE CoverCode = @OldRiderInjuryCode

UPDATE CoverBaseRate SET CoverCode = @NewIllnessCode WHERE Id IN (
	SELECT Id FROM #IllnessCoverRates
)
UPDATE CoverBaseRate SET CoverCode = @NewInjuryCode WHERE Id IN (
	SELECT Id FROM #InjuryCoverRates
)
UPDATE CoverBaseRate SET CoverCode = @NewRiderIllnessCode WHERE Id IN (
	SELECT Id FROM #RiderIllnessCoverRates
)
UPDATE CoverBaseRate SET CoverCode = @NewRiderInjuryCode WHERE Id IN (
	SELECT Id FROM #RiderInjuryCoverRates
)

DROP TABLE #IllnessCoverRates
DROP TABLE #InjuryCoverRates
DROP TABLE #RiderIllnessCoverRates
DROP TABLE #RiderInjuryCoverRates

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
