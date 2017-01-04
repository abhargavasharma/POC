SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PartyConsent](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartyId] [int] NULL FOREIGN KEY REFERENCES [Party]([Id]),
	[ExpressConsent] [bit] NOT NULL,
	[ExpressConsentUpdatedTs] DATETIME NULL,
	[DncMobile] [bit] NOT NULL,
	[DncHomeNumber] [bit] NOT NULL,
	[DncWorkNumber] [bit] NOT NULL,
	[DncEmail] [bit] NOT NULL,
	[DncPostalMail] [bit] NOT NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_PartyConsent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about consent a person has given to be contacted'
    ,'user', 'dbo', 'table'
    ,'PartyConsent'

exec sp_addextendedproperty 'MS_Description'
    ,'Primary key, auto incrementing'
    ,'user', 'dbo'
    ,'table', 'PartyConsent'
    ,'column', 'Id'

exec sp_addextendedproperty 'MS_Description'
    ,'PartyId of the party consent, links to Party table'
    ,'user', 'dbo'
    ,'table', 'PartyConsent'
    ,'column', 'PartyId'  

exec sp_addextendedproperty 'MS_Description'
    ,'Whether party has given express consent for marketing material'
    ,'user', 'dbo'
    ,'table', 'PartyConsent'
    ,'column', 'ExpressConsent'      

exec sp_addextendedproperty 'MS_Description'
    ,'Last dateTime that express consent was updated'
    ,'user', 'dbo'
    ,'table', 'PartyConsent'
    ,'column', 'ExpressConsentUpdatedTs'    

exec sp_addextendedproperty 'MS_Description'
    ,'Whether party has asked to not be contacted via mobile'
    ,'user', 'dbo'
    ,'table', 'PartyConsent'
    ,'column', 'DncMobile'  

exec sp_addextendedproperty 'MS_Description'
    ,'Whether party has asked to not be contacted via home phone'
    ,'user', 'dbo'
    ,'table', 'PartyConsent'
    ,'column', 'DncHomeNumber'  

exec sp_addextendedproperty 'MS_Description'
    ,'Whether party has asked to not be contacted via work phone'
    ,'user', 'dbo'
    ,'table', 'PartyConsent'
    ,'column', 'DncWorkNumber'  

exec sp_addextendedproperty 'MS_Description'
    ,'Whether party has asked to not be contacted via email'
    ,'user', 'dbo'
    ,'table', 'PartyConsent'
    ,'column', 'DncEmail'  

exec sp_addextendedproperty 'MS_Description'
    ,'Whether party has asked to not be contacted via postal mail'
    ,'user', 'dbo'
    ,'table', 'PartyConsent'
    ,'column', 'DncPostalMail'

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'PartyConsent'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'PartyConsent'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'PartyConsent'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'PartyConsent'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'PartyConsent'
    ,'column', 'ModifiedTS'  