IF (NOT EXISTS (SELECT 1 FROM sys.views WHERE name = 'vwAgentDashboardSearch'))
BEGIN
    EXECUTE('CREATE VIEW vwAgentDashboardSearch as SELECT 1 as vADS');
END;

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[vwAgentDashboardSearch]
AS

  SELECT [PolicyInteraction].Id, [PolicyInteraction].PolicyId, 
  [PolicyInteraction].CreatedTS as LastQuoteAccessTs, Policy.Premium, concat(Party.Firstname, ' ', Party.Surname) as FullName, 
  Policy.QuoteReference, Party.Id as OwnerPartyId, Policy.PolicyProgressId as Progress, MaxPolicyProgressInteractionForAgent.MaxCreatedDate as ProgressUpdateTs,
  [PolicyInteraction].CreatedBy, [PolicyInteraction].CreatedTS, [PolicyInteraction].ModifiedBy, [PolicyInteraction].ModifiedTS, [PolicyInteraction].RV
  FROM [PolicyInteraction] WITH(NOLOCK)
  INNER JOIN (
	  SELECT MAX(ID) As MaxInteractionId, PolicyId, CreatedBy
	  FROM [PolicyInteraction] WITH(NOLOCK)
	  WHERE InteractionTypeId = 1
	  GROUP BY PolicyId,CreatedBy
  ) MaxPolicyInteractionForAgent 
	ON MaxPolicyInteractionForAgent.MaxInteractionId = [PolicyInteraction].Id
	INNER JOIN Policy WITH(NOLOCK)
		ON Policy.Id = [PolicyInteraction].PolicyId
	INNER JOIN PolicyOwner WITH(NOLOCK)
		ON PolicyOwner.PolicyId = Policy.Id
	INNER JOIN Party WITH(NOLOCK)
		ON PolicyOwner.PartyId = Party.Id


	LEFT OUTER JOIN (
	  SELECT PolicyId, CreatedBy, max(CreatedTS) as MaxCreatedDate
	  FROM [PolicyInteraction] WITH(NOLOCK)
	  WHERE InteractionTypeId = 2
	  GROUP BY PolicyId,CreatedBy
  ) MaxPolicyProgressInteractionForAgent 
	ON MaxPolicyProgressInteractionForAgent.PolicyId = Policy.Id
GO