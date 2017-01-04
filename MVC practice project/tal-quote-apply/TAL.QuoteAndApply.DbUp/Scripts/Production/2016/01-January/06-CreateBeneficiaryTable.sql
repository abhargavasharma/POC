SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Beneficiary](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GenderId] [int] NULL FOREIGN KEY REFERENCES [Gender]([Id]),
	[DateOfBirth] [date] NULL,
	[TitleId] [int] NULL FOREIGN KEY REFERENCES [Title]([Id]),
	[Firstname] [nvarchar](200) NULL,
	[Surname] [nvarchar](200) NULL,
	[Address] [nvarchar](200) NULL,
	[Suburb] [nvarchar](200) NULL,
	[StateId] [int] NULL FOREIGN KEY REFERENCES [State]([Id]),
	[CountryId] [int] NULL FOREIGN KEY REFERENCES [Country]([Id]),
	[Postcode] [nvarchar](4) NULL,
	[PhoneNumber] [nvarchar](10) NULL,
	[EmailAddress] [nvarchar](200) NULL,
	[BeneficiaryRelationshipId] [int] NULL,
	[Share] [int] NULL,
	[RV] [timestamp] NOT NULL,
	[RiskId] [int] NOT NULL FOREIGN KEY REFERENCES [Risk]([Id]),
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_Beneficiary] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a Beneficiary'
    ,'user', 'dbo', 'table'
    ,'Beneficiary'

exec sp_addextendedproperty 'MS_Description'
    ,'Primary key, auto incrementing'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'Id'

exec sp_addextendedproperty 'MS_Description'
    ,'Gender of the Beneficiary, links to Gender table'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'GenderId'     

exec sp_addextendedproperty 'MS_Description'
    ,'DateOfBirth of the Beneficiary'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'DateOfBirth'  

exec sp_addextendedproperty 'MS_Description'
    ,'Title of the Beneficiary, links to title table'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'TitleId'  

exec sp_addextendedproperty 'MS_Description'
    ,'Firstname of the Beneficiary, links to title table'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'Firstname'  

exec sp_addextendedproperty 'MS_Description'
    ,'Surname of the Beneficiary, links to title table'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'Surname'  

exec sp_addextendedproperty 'MS_Description'
    ,'Address of the Beneficiary'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'Address'  

exec sp_addextendedproperty 'MS_Description'
    ,'Suburb of the Beneficiary'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'Suburb'  

exec sp_addextendedproperty 'MS_Description'
    ,'State of the Beneficiary, links to State table'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'StateId'  

exec sp_addextendedproperty 'MS_Description'
    ,'Country of the Beneficiary, links to Country table'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'CountryId'  

exec sp_addextendedproperty 'MS_Description'
    ,'Postcode of the Beneficiary'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'Postcode'	
	
exec sp_addextendedproperty 'MS_Description'
    ,'PhoneNumber of the Beneficiary'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'PhoneNumber'			
	
exec sp_addextendedproperty 'MS_Description'
    ,'EmailAddress of the Beneficiary'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'EmailAddress'	

exec sp_addextendedproperty 'MS_Description'
    ,'BeneficiaryRelationship of the Beneficiary, links to BeneficiaryRelationship table'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'BeneficiaryRelationshipId'	

exec sp_addextendedproperty 'MS_Description'
    ,'Percentage share this beneficiary recieves'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'Share'	

exec sp_addextendedproperty 'MS_Description'
    ,'Risk this beneficiary belongs too, links to the risk table'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'RiskId'	

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'Beneficiary'
    ,'column', 'ModifiedTS'  

