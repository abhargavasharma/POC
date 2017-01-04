SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

  ALTER TABLE [dbo].[Organisation]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]);
 
 GO

UPDATE [dbo].[Organisation] SET [BrandId] = 
       (SELECT TOP 1 Id from [dbo].ProductBrand where ProductKey = 'TAL')
where [OrganisationKey] = 'TAL'

UPDATE [dbo].[Organisation] SET [BrandId] = 
       (SELECT TOP 1 Id from [dbo].ProductBrand where ProductKey = 'YB')
where [OrganisationKey] = 'YB'

UPDATE [dbo].[Organisation] SET [BrandId] = 
       (SELECT TOP 1 Id from [dbo].ProductBrand where ProductKey = 'QA')
where [OrganisationKey] = 'QA'


GO

 exec sp_addextendedproperty 'MS_Description'
    ,'The Brand Id of the organisation'
    ,'user', 'dbo'
    ,'table', 'Organisation'
    ,'column', 'BrandId'   

GO

