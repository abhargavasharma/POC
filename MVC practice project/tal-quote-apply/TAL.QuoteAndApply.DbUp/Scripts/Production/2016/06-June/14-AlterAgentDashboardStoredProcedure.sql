SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[spAgentDashboardSearch]
	@agent NVARCHAR(512),
	@dateTimeFrom DATE,
	@dateTimeTo DATE,

	@PipelineStatusUnknown BIT = 0,
	@PipelineStatusInProgressPreUw BIT = 0,
	@PipelineStatusInProgressUwReferral BIT = 0,
	@PipelineStatusInProgressRecommendation BIT = 0,
	@PipelineStatusInProgressCantContact BIT = 0,
	@PipelineStatusClosedSale BIT = 0,
	@PipelineStatusClosedNoSale BIT = 0,
	@PipelineStatusClosedTriage BIT = 0,
	@PipelineStatusClosedCantContact BIT = 0,

	@PageNumber INT = 1,
    @PageSize   INT = 100

AS
BEGIN

--Set pipeline search variable
	DECLARE @DoPipelineSearch BIT = 1
	SELECT @DoPipelineSearch = 0
	WHERE  (@PipelineStatusUnknown = 0 AND @PipelineStatusInProgressPreUw = 0 AND @PipelineStatusInProgressUwReferral = 0 AND 
                     @PipelineStatusInProgressRecommendation = 0 AND @PipelineStatusInProgressCantContact = 0 AND @PipelineStatusClosedSale = 0 AND
                     @PipelineStatusClosedNoSale = 0 AND @PipelineStatusClosedTriage = 0 AND @PipelineStatusClosedCantContact = 0)

	SET NOCOUNT ON;

--Do filter on Policy Progress Type if parameter in place otherwise return all
    ;WITH FilteredPolicies AS (
    SELECT  Id as PolicyId
    FROM    Policy
    WHERE 
              (@DoPipelineSearch = 1 AND (
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
                     ))
              OR @DoPipelineSearch = 0
              
--Get the agent's last quote access interaction joined to the above filtered policies
), AgentPolicies AS (
    SELECT  DISTINCT i.PolicyId
    FROM    PolicyInteraction i
            INNER JOIN FilteredPolicies fp
                ON i.PolicyId = fp.PolicyId
    WHERE 
            i.CreatedBy = @agent
            AND
            i.InteractionTypeId = 1 -- Quote_Accessed

--Get last access of quote with time range filtering and pagination enabled
), LastAccessed AS (
        SELECT pac.PolicyId, pac.LastTouchedByTS
        FROM    PolicyAccessControl pac
                INNER JOIN AgentPolicies ap
                    ON pac.PolicyId = ap.PolicyId
              WHERE 
              @dateTimeFrom <= Convert(DATE, pac.LastTouchedByTS)
        AND
        Convert(DATE, pac.LastTouchedByTS) <= @dateTimeTo
        ORDER BY LastTouchedByTS 
        OFFSET @PageSize * (@PageNumber - 1) ROWS
        FETCH NEXT @PageSize ROWS ONLY

--Get the last pipeline interaction change for the quote
), LastPipelineStatusChange AS (
       SELECT  p.PolicyId, MAX(p.Id) AS MaxInteractionId
       FROM    PolicyInteraction p
                     INNER JOIN policy ap
                           ON p.PolicyId = ap.Id
       WHERE 
                     p.InteractionTypeId = 2 -- Pipeline_Status_Change
       GROUP BY p.PolicyId

--Get the last pipeline status change for the user
), LastPipelineStatusChangeForUser AS (
       SELECT  p.PolicyId, p.CreatedBy, p.CreatedTS
       FROM    PolicyInteraction p
              INNER JOIN LastPipelineStatusChange lastP
              ON p.Id = lastP.MaxInteractionId

--Get policy owner fullname
), Owners AS (
SELECT  ap.PolicyId, p.Firstname + ' ' + p.Surname AS OwnerFullName, po.PartyId as OwnerPartyId
FROM    AgentPolicies ap
        INNER JOIN PolicyOwner po
                ON ap.PolicyId = po.PolicyId
        INNER JOIN Party p
                ON po.PartyId = p.Id
)

--Compile final table result
SELECT  
       p.Id as PolicyId
       , p.QuoteReference
       , p.Premium
       , la.LastTouchedByTS
       , ISNULL(ps.CreatedTS, la.LastTouchedByTS) as LastPipelineStatusTS
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
       LEFT OUTER JOIN LastPipelineStatusChangeForUser ps 
                     ON ap.PolicyId = ps.PolicyId
       WHERE (@DoPipelineSearch = 0 OR (@DoPipelineSearch = 1 AND ps.CreatedBy = @agent) OR (@DoPipelineSearch = 1 AND s.Id = 0))
       ORDER BY la.LastTouchedByTS DESC 


	OPTION (RECOMPILE);

END