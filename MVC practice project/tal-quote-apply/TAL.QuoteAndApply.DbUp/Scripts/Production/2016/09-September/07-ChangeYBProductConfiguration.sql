INSERT INTO [ProductConfiguration] ([ProductBrandId], [Key], [Value], [Description], [CreatedBy], [CreatedTS])
VALUES (2, 'AvailableCreditCardTypes', 'Visa,MasterCard,Amex', 'Available Credit Card Types', 'SYSTEM', GETDATE())

Update [ProductConfiguration]
set [Value] = 'CreditCard,SMSF'
where [Key] = 'AvailablePaymentTypes'
and [ProductBrandId] = 2

