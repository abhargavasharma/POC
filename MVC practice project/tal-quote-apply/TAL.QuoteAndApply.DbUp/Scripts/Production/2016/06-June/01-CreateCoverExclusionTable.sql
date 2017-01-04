CREATE TABLE [dbo].[CoverExclusion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CoverId] [int] NOT NULL FOREIGN KEY REFERENCES [Cover]([Id]),
	[Text] NVARCHAR(MAX) NULL,
	[Name] NVARCHAR(MAX) NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_CoverExclusion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a CoverExclusion'
    ,'user', 'dbo', 'table'
    ,'CoverExclusion'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique identifier of the CoverExclusion'
    ,'user', 'dbo'
    ,'table', 'CoverExclusion'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Cover the exclusion is linked to. References Cover table'
    ,'user', 'dbo'
    ,'table', 'CoverExclusion'
    ,'column', 'CoverId'  

exec sp_addextendedproperty 'MS_Description'
    ,'Text of the exclusion'
    ,'user', 'dbo'
    ,'table', 'CoverExclusion'
    ,'column', 'Text'  

exec sp_addextendedproperty 'MS_Description'
    ,'Name of the exclusion'
    ,'user', 'dbo'
    ,'table', 'CoverExclusion'
    ,'column', 'Name'  

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'CoverExclusion'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'CoverExclusion'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'CoverExclusion'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'CoverExclusion'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'CoverExclusion'
    ,'column', 'ModifiedTS'
GO