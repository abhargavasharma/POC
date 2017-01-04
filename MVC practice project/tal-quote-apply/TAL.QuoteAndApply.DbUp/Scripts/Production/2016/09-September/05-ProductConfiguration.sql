CREATE TABLE [dbo].[ProductConfiguration](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductBrandId] [int] NOT NULL FOREIGN KEY REFERENCES [ProductBrand] ([Id]),
	[Key] NVARCHAR(256) NOT NULL,
	[Value] NVARCHAR(512) NOT NULL,
	[Description] NVARCHAR(512) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_ProductConfiguration] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

exec sp_addextendedproperty 'MS_Description'
    ,'A composite key value store of settings for the application'
    ,'user', 'dbo', 'table'
    ,'ProductConfiguration'

exec sp_addextendedproperty 'MS_Description'
    ,'Primary key, auto incrementing'
    ,'user', 'dbo'
    ,'table', 'ProductConfiguration'
    ,'column', 'Id'     
	
exec sp_addextendedproperty 'MS_Description'
    ,'The brand associated with the key'
    ,'user', 'dbo'
    ,'table', 'ProductConfiguration'
    ,'column', 'ProductBrandId'         

exec sp_addextendedproperty 'MS_Description'
    ,'THe key of the setting'
    ,'user', 'dbo'
    ,'table', 'ProductConfiguration'
    ,'column', 'Key'         

exec sp_addextendedproperty 'MS_Description'
    ,'The value of the setting'
    ,'user', 'dbo'
    ,'table', 'ProductConfiguration'
    ,'column', 'Value'         

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the given key'
    ,'user', 'dbo'
    ,'table', 'ProductConfiguration'
    ,'column', 'Description'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'ProductConfiguration'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'ProductConfiguration'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'ProductConfiguration'
    ,'column', 'ModifiedBy' 
	
	
exec sp_addextendedproperty 'MS_Description'
    ,'Row version'
    ,'user', 'dbo'
    ,'table', 'ProductConfiguration'
    ,'column', 'RV'   

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'ProductConfiguration'
    ,'column', 'ModifiedTS'  

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ProductConfiguration_ProductBrandId_Key] ON [dbo].[ProductConfiguration]
(
	[ProductBrandId] ASC,
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

INSERT INTO [ProductConfiguration] ([ProductBrandId], [Key], [Value], [Description], [CreatedBy], [CreatedTS])
VALUES (1, 'AvailablePaymentTypes', 'CreditCard,DirectDebit', 'Available Payment Types', 'SYSTEM', GETDATE())

INSERT INTO [ProductConfiguration] ([ProductBrandId], [Key], [Value], [Description], [CreatedBy], [CreatedTS])
VALUES (2, 'AvailablePaymentTypes', 'CreditCard,DirectDebit,SMSF', 'Available Payment Types', 'SYSTEM', GETDATE())

