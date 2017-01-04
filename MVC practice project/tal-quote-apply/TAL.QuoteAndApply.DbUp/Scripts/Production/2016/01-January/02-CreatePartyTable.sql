SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Party](
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
	[MobileNumber] [nvarchar](10) NULL,
	[HomeNumber] [nvarchar](10) NULL,
	[EmailAddress] [nvarchar](200) NULL,
	[PreferredContactMethodId] [int] NULL FOREIGN KEY REFERENCES [PreferredContactMethod]([Id]),
	[LeadId] [int] NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_Party] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a person'
    ,'user', 'dbo', 'table'
    ,'Party'

exec sp_addextendedproperty 'MS_Description'
    ,'Primary key, auto incrementing'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'Id'

exec sp_addextendedproperty 'MS_Description'
    ,'Gender of the party, links to Gender table'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'GenderId'     

exec sp_addextendedproperty 'MS_Description'
    ,'DateOfBirth of the party'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'DateOfBirth'  

exec sp_addextendedproperty 'MS_Description'
    ,'Title of the party, links to title table'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'TitleId'  

exec sp_addextendedproperty 'MS_Description'
    ,'Firstname of the party, links to title table'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'Firstname'  

exec sp_addextendedproperty 'MS_Description'
    ,'Surname of the party, links to title table'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'Surname'  

exec sp_addextendedproperty 'MS_Description'
    ,'Address of the party'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'Address'  

exec sp_addextendedproperty 'MS_Description'
    ,'Suburb of the party'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'Suburb'  

exec sp_addextendedproperty 'MS_Description'
    ,'State of the party, links to State table'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'StateId'  

exec sp_addextendedproperty 'MS_Description'
    ,'Country of the party, links to Country table'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'CountryId'  

exec sp_addextendedproperty 'MS_Description'
    ,'Postcode of the party'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'Postcode'	
	
exec sp_addextendedproperty 'MS_Description'
    ,'MobileNumber of the party'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'MobileNumber'		
	
exec sp_addextendedproperty 'MS_Description'
    ,'HomeNumber of the party'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'HomeNumber'		
	
exec sp_addextendedproperty 'MS_Description'
    ,'EmailAddress of the party'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'EmailAddress'	
	
exec sp_addextendedproperty 'MS_Description'
    ,'LeadId of the party, links to campaign management system'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'LeadId' 	

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'Party'
    ,'column', 'ModifiedTS'  