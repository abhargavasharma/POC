SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Organisation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrganisationKey] [nvarchar](30) NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_Organisation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a the different brands supported by Tal Consumer'
    ,'user', 'dbo', 'table'
    ,'Organisation'

exec sp_addextendedproperty 'MS_Description'
    ,'The Id of the Brand'
    ,'user', 'dbo'
    ,'table', 'Organisation'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'The Organisation Code'
    ,'user', 'dbo'
    ,'table', 'Organisation'
    ,'column', 'OrganisationKey'   

exec sp_addextendedproperty 'MS_Description'
    ,'A field to provide a friendly text version of this organisation'
    ,'user', 'dbo'
    ,'table', 'Organisation'
    ,'column', 'Description'

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'Organisation'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'Organisation'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'Organisation'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'Organisation'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'Organisation'
    ,'column', 'ModifiedTS'

GO

INSERT INTO Organisation ([OrganisationKey], [Description], [CreatedBy], [CreatedTS]) VALUES ('TAL', 'TAL Direct', 'SYSTEM', GETDATE())
INSERT INTO Organisation ([OrganisationKey], [Description], [CreatedBy], [CreatedTS]) VALUES ('YB', 'Yellow Brand Direct', 'SYSTEM', GETDATE())

GO

ALTER TABLE [dbo].[Policy]
	ADD [BrandId] [int] NOT NULL DEFAULT 1,
		[OrganisationId] [int] NOT NULL DEFAULT 1,
	FOREIGN KEY([BrandId]) REFERENCES [ProductBrand]([Id]),
	FOREIGN KEY([OrganisationId]) REFERENCES [Organisation]([Id]);
 
 GO

 INSERT INTO [ProductBrand] ([ProductKey], [Description], [CreatedBy], [CreatedTS]) VALUES ('YB', 'Test - Yellow Brand', 'SYSTEM', GETDATE())
