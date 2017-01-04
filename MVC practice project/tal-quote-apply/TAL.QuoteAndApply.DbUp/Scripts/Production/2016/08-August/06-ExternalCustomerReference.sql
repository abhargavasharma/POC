ALTER TABLE [dbo].[Party]
ADD [ExternalCustomerReference] [NVARCHAR] (20) NULL
exec sp_addextendedproperty 'MS_Description'
		,'Uniquely identifies the External Customer Records'
		,'user', 'dbo'
		,'table', 'Party'
		,'column', 'ExternalCustomerReference'  