SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductBrand](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductKey] [nvarchar](30) NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_ProductBrand] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a the different brands supported by Tal Consumer'
    ,'user', 'dbo', 'table'
    ,'ProductBrand'

exec sp_addextendedproperty 'MS_Description'
    ,'The Id of the Brand'
    ,'user', 'dbo'
    ,'table', 'ProductBrand'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'The Brand Code'
    ,'user', 'dbo'
    ,'table', 'ProductBrand'
    ,'column', 'ProductKey'   

exec sp_addextendedproperty 'MS_Description'
    ,'The Brand Code'
    ,'user', 'dbo'
    ,'table', 'ProductBrand'
    ,'column', 'Description'

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'ProductBrand'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'ProductBrand'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'ProductBrand'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'ProductBrand'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'ProductBrand'
    ,'column', 'ModifiedTS'

GO

INSERT INTO [ProductBrand] ([ProductKey], [Description], [CreatedBy], [CreatedTS]) VALUES ('TAL', 'TAL Cover Builder', 'SYSTEM', GETDATE())

GO