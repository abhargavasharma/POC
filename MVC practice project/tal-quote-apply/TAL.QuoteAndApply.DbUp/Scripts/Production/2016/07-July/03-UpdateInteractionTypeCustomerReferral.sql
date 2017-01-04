DECLARE @InteractionTypeId INT
SELECT @InteractionTypeId = Id
FROM InteractionType
WHERE [Description] = 'Customer_Referral'

UPDATE InteractionType
SET [Description] = 'Customer_Submit_Application_Referred'
WHERE Id = @InteractionTypeId