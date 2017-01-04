UPDATE pc 
SET pc.Value = 'CreditCard,SMSF,Super'
FROM ProductConfiguration pc
LEFT OUTER JOIN ProductBrand b on b.Id = pc.ProductBrandId
WHERE [Key] = 'AvailablePaymentTypes' AND ProductKey = 'YB'

UPDATE pc 
SET pc.Value = 'Visa' 
FROM ProductConfiguration pc
LEFT OUTER JOIN ProductBrand b on b.Id = pc.ProductBrandId
WHERE [Key] = 'AvailableCreditCardTypes' AND ProductKey = 'YB'
