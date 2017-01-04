ALTER VIEW vwPolicySearch
AS

SELECT 
Policy.Id As PolicyId, Policy.RV, Policy.CreatedBy, Policy.CreatedTS, Policy.ModifiedBy, Policy.ModifiedTS,
Policy.QuoteReference, Policy.Premium, Party.FirstName, Party.Surname, 
Party.DateOfBirth, Party.StateId, Party.MobileNumber, Party.HomeNumber, Party.EmailAddress, Party.LeadId, Party.ExternalCustomerReference, Policy.BrandId
FROM Policy WITH(NOLOCK)
INNER JOIN PolicyOwner WITH(NOLOCK)
	ON Policy.Id = PolicyOwner.PolicyId
INNER JOIN Party WITH(NOLOCK)
	ON PolicyOwner.PartyId = Party.Id