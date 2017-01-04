GO

INSERT INTO [CoverMarketingStatus] ([CoverId], [MarketingStatusId], [CreatedBy],[CreatedTS],[ModifiedBy],[ModifiedTS])
SELECT	c.Id AS [CoverId]
		,0 AS [MarketingStatusId]
		,'SYSTEM' AS [CreatedBy]
		,GetDate() AS [CreatedTS]
		,'SYSTEM' AS [ModifiedBy]
		,GetDate() AS [ModifiedTS]
FROM [Cover] c
LEFT OUTER JOIN [CoverMarketingStatus] cms on cms.CoverId = c.Id
WHERE cms.MarketingStatusId IS NULL

GO

INSERT INTO [PlanMarketingStatus]([PlanId], [MarketingStatusId], [CreatedBy],[CreatedTS],[ModifiedBy],[ModifiedTS])
SELECT p.Id AS [PlanId]
		,0 AS [MarketingStatusId]
		,'SYSTEM' AS [CreatedBy]
		,GetDate() AS [CreatedTS]
		,'SYSTEM' AS [ModifiedBy]
		,GetDate() AS [ModifiedTS]
FROM [Plan] p
LEFT OUTER JOIN [PlanMarketingStatus] pms on pms.PlanId = p.Id
WHERE pms.MarketingStatusId IS NULL

GO

INSERT INTO [RiskMarketingStatus]([RiskId], [MarketingStatusId], [CreatedBy],[CreatedTS],[ModifiedBy],[ModifiedTS])
SELECT r.Id AS [RiskId]
		,0 AS [MarketingStatusId]
		,'SYSTEM' AS [CreatedBy]
		,GetDate() AS [CreatedTS]
		,'SYSTEM' AS [ModifiedBy]
		,GetDate() AS [ModifiedTS] 
FROM [Risk] r
LEFT OUTER JOIN [RiskMarketingStatus] rms on rms.RiskId = r.Id
WHERE rms.MarketingStatusId IS NULL