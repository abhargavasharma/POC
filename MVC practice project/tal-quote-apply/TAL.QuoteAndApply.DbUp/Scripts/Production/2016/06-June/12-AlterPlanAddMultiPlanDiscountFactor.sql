IF NOT EXISTS(
    SELECT *
    FROM sys.columns 
    WHERE Name = N'MultiPlanDiscountFactor'
    AND Object_ID = Object_ID(N'Plan'))
BEGIN
	ALTER TABLE [dbo].[Plan] ADD [MultiPlanDiscountFactor] DECIMAL(18,2) NOT NULL DEFAULT(0)
	
	exec sp_addextendedproperty 'MS_Description'
		,'Stores the discount factor used for the multi plan discount'
		,'user', 'dbo'
		,'table', 'Plan'
		,'column', 'MultiPlanDiscountFactor'  
END
