CREATE VIEW vwAgentDashboardSearch
AS

  SELECT [PolicyInteraction].Id, [PolicyInteraction].PolicyId, 
  [PolicyInteraction].CreatedTS as LastInteractionTs, Policy.Premium, concat(Party.Firstname, ' ', Party.Surname) as FullName, 
  Policy.QuoteReference, Party.Id as OwnerPartyId, 
  [PolicyInteraction].CreatedBy, [PolicyInteraction].CreatedTS, [PolicyInteraction].ModifiedBy, [PolicyInteraction].ModifiedTS, [PolicyInteraction].RV
  FROM [PolicyInteraction] WITH(NOLOCK)
  INNER JOIN (
	  SELECT MAX(ID) As MaxInteractionId, PolicyId, CreatedBy
	  FROM [PolicyInteraction] WITH(NOLOCK)
	  GROUP BY PolicyId,CreatedBy
  ) MaxPolicyInteractionForAgent 
	ON MaxPolicyInteractionForAgent.MaxInteractionId = [PolicyInteraction].Id
	INNER JOIN Policy WITH(NOLOCK)
		ON Policy.Id = [PolicyInteraction].PolicyId
	INNER JOIN PolicyOwner WITH(NOLOCK)
		ON PolicyOwner.PolicyId = Policy.Id
	INNER JOIN Party WITH(NOLOCK)
		ON PolicyOwner.PartyId = Party.Id


