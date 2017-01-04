SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spAgentDashboardSearch]
	@agent NVARCHAR(512),
	@dateTimeFrom DATE,
	@dateTimeTo DATE,

	@PipelineStatusUnknown BIT = 1,
	@PipelineStatusInProgressPreUw BIT = 1,
	@PipelineStatusInProgressUwReferral BIT = 1,
	@PipelineStatusInProgressRecommendation BIT = 1,
	@PipelineStatusInProgressCantContact BIT = 1,
	@PipelineStatusClosedSale BIT = 1,
	@PipelineStatusClosedNoSale BIT = 1,
	@PipelineStatusClosedTriage BIT = 1,
	@PipelineStatusClosedCantContact BIT = 1,

	@PageNumber INT = 1,
    @PageSize   INT = 100

AS
BEGIN
	SET NOCOUNT ON;

	--date range should chnage to query PolicyAccessed

    ;WITH FilteredPolicies AS (
		SELECT  Id as PolicyId
		FROM    Policy
		WHERE 
			(@PipelineStatusUnknown = 1 AND PolicyProgressId = 0)
			OR
			(@PipelineStatusInProgressPreUw = 1 AND PolicyProgressId = 1)
			OR
			(@PipelineStatusInProgressUwReferral = 1 AND PolicyProgressId = 2)
			OR
			(@PipelineStatusInProgressRecommendation = 1 AND PolicyProgressId = 3)
			OR
			(@PipelineStatusInProgressCantContact = 1 AND PolicyProgressId = 4)
			OR
			(@PipelineStatusClosedSale = 1 AND PolicyProgressId = 5)
			OR
			(@PipelineStatusClosedNoSale = 1 AND PolicyProgressId = 6)
			OR
			(@PipelineStatusClosedTriage = 1 AND PolicyProgressId = 7)
			OR
			(@PipelineStatusClosedCantContact = 1 AND PolicyProgressId = 8)

			-- and I did the pipeline change ??

			-- if no pipeline selected :
				-- currently none, but needs to be all

	), AgentPolicies AS (
		SELECT  DISTINCT i.PolicyId
		FROM    PolicyInteraction i
			INNER JOIN FilteredPolicies fp
				ON i.PolicyId = fp.PolicyId
		WHERE 
			i.CreatedBy = @agent
			AND
			@dateTimeFrom <= Convert(DATE, i.CreatedTS)
			AND
			Convert(DATE, i.CreatedTS) <= @dateTimeTo
			AND
			i.InteractionTypeId = 1 -- Quote_Accessed
	), LastAccessed AS (
		SELECT  p.PolicyId, MAX(CreatedTS) as LastAccessedTS
		FROM    PolicyInteraction p
			INNER JOIN AgentPolicies ap
				ON p.PolicyId = ap.PolicyId
		WHERE 
			p.InteractionTypeId = 1 -- Quote_Accessed
		GROUP BY p.PolicyId
		ORDER BY MAX(CreatedTS) -- grab Greg if you get to order by anything else besides LastAccessedTS
		OFFSET @PageSize * (@PageNumber - 1) ROWS
		FETCH NEXT @PageSize ROWS ONLY
	), PipelineStatus AS (
		SELECT  p.PolicyId, MAX(CreatedTS) AS LastPipelineStatusTS
		FROM    PolicyInteraction p
			INNER JOIN AgentPolicies ap
				ON p.PolicyId = ap.PolicyId
		WHERE 
			p.InteractionTypeId = 2 -- Pipeline_Status_Change
		GROUP BY p.PolicyId
	), Owners AS (
	SELECT  ap.PolicyId, p.Firstname + ' ' + p.Surname AS OwnerFullName, po.PartyId as OwnerPartyId
	FROM    AgentPolicies ap
		INNER JOIN PolicyOwner po
			ON ap.PolicyId = po.PolicyId
		INNER JOIN Party p
			ON po.PartyId = p.Id
	)
	SELECT  
		p.Id as PolicyId
	, p.QuoteReference
	, p.Premium
	, la.LastAccessedTS
	, ISNULL(ps.LastPipelineStatusTS, p.CreatedTS) as LastPipelineStatusTS
	, s.Description as LastPipelineStatusDescription
	, o.OwnerFullName
	, o.OwnerPartyId
	FROM    AgentPolicies ap
		INNER JOIN LastAccessed la
			ON ap.PolicyId = la.PolicyId
		INNER JOIN Policy p
			ON ap.PolicyId = p.Id
		INNER JOIN PolicyProgress s
			ON p.PolicyProgressId = s.Id
		INNER JOIN Owners o
			ON ap.PolicyId = o.PolicyId
		LEFT JOIN PipelineStatus ps	
			ON ap.PolicyId = ps.PolicyId
	ORDER BY la.LastAccessedTS DESC 
	OPTION (RECOMPILE);

END