SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BeneficiaryRelationship](
	[Id] [int] NOT NULL,
	[PASExportValue] [nvarchar](30) NULL,
	[Description] [nvarchar](50) NULL,
	[RV] [timestamp] NULL,
	[CreatedBy] [nvarchar](256) NULL,
	[CreatedTS] [datetime] NULL,
	[ModifiedBy] [nvarchar](256) NULL,
	[ModifiedTS] [datetime] NULL,
 CONSTRAINT [PK_BeneficiaryRelationship] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a BeneficiaryRelationship'
    ,'user', 'dbo', 'table'
    ,'BeneficiaryRelationship'

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'BeneficiaryRelationship'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'BeneficiaryRelationship'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'BeneficiaryRelationship'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'BeneficiaryRelationship'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'BeneficiaryRelationship'
    ,'column', 'ModifiedTS'
GO

insert into [dbo].[BeneficiaryRelationship]
(Id, PASExportValue, Description, CreatedBy, CreatedTS)
values
(1, 'Spouse', 'Spouse', 'script', GETDATE());

GO

insert into [dbo].[BeneficiaryRelationship]
(Id, PASExportValue, Description, CreatedBy, CreatedTS)
values
(2, 'Spouse, de facto ', 'Spouse, de facto ', 'script', GETDATE());

GO

insert into [dbo].[BeneficiaryRelationship]
(Id, PASExportValue, Description, CreatedBy, CreatedTS)
values
(3, 'Child', 'Child', 'script', GETDATE());

GO

insert into [dbo].[BeneficiaryRelationship]
(Id, PASExportValue, Description, CreatedBy, CreatedTS)
values
(4, 'Child, adopted', 'Child, adopted', 'script', GETDATE());

GO

insert into [dbo].[BeneficiaryRelationship]
(Id, PASExportValue, Description, CreatedBy, CreatedTS)
values
(5, 'Child, ex-nuptial', 'Child, from a previous partner', 'script', GETDATE());

GO

insert into [dbo].[BeneficiaryRelationship]
(Id, PASExportValue, Description, CreatedBy, CreatedTS)
values
(6, 'Child, step', 'Child, step', 'script', GETDATE());

GO

insert into [dbo].[BeneficiaryRelationship]
(Id, PASExportValue, Description, CreatedBy, CreatedTS)
values
(7, 'Financially Dependant', 'Financially Dependant', 'script', GETDATE());

GO

insert into [dbo].[BeneficiaryRelationship]
(Id, PASExportValue, Description, CreatedBy, CreatedTS)
values
(8, 'Personal Representative', 'Personal Representative', 'script', GETDATE());

GO

insert into [dbo].[BeneficiaryRelationship]
(Id, PASExportValue, Description, CreatedBy, CreatedTS)
values
(9, 'Other (as per application)', 'Other (as per application)', 'script', GETDATE());

GO


ALTER TABLE Beneficiary
ADD CONSTRAINT FK_BeneficiaryRelationship_BeneficiaryRelationshipId  FOREIGN KEY (BeneficiaryRelationshipId)
    REFERENCES BeneficiaryRelationship(id);

GO