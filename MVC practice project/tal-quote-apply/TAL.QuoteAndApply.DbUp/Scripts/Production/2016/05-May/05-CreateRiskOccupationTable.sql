CREATE TABLE [dbo].[RiskOccupation](
	[Id] INT IDENTITY(1,1) NOT NULL,
	[RiskId] INT NOT NULL FOREIGN KEY REFERENCES [Risk]([Id]),
	[OccupationTitle] NVARCHAR(256) NULL,
	[OccupationAnswerId] NVARCHAR(50) NULL,
	[IndustryTitle] NVARCHAR(256) NULL,
	[IndustryAnswerId] NVARCHAR(50) NULL,
	[OccupationClass] NVARCHAR(10) NULL,
	[TpdOccupationLoading] DECIMAL(18,2) NULL,
	[IsTpdAny] BIT NULL,
	[IsTpdOwn] BIT NULL,
	[PasCode] NVARCHAR(10) NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_RiskOccupation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a Risk Occupation'
    ,'user', 'dbo', 'table'
    ,'RiskOccupation'

exec sp_addextendedproperty 'MS_Description'
    ,'Primary key, auto incrementing'
    ,'user', 'dbo'
    ,'table', 'RiskOccupation'
    ,'column', 'Id'

exec sp_addextendedproperty 'MS_Description'
    ,'Occupation Answer Code of the risk, code comes from TALUS interview'
    ,'user', 'dbo'
    ,'table', 'RiskOccupation'
    ,'column', 'OccupationAnswerId'   

exec sp_addextendedproperty 'MS_Description'
    ,'Occupation Title of the risk, title comes from TALUS interview'
    ,'user', 'dbo'
    ,'table', 'RiskOccupation'
    ,'column', 'OccupationTitle'   

exec sp_addextendedproperty 'MS_Description'
    ,'OccupationClass of the risk, class comes from TALUS interview'
    ,'user', 'dbo'
    ,'table', 'RiskOccupation'
    ,'column', 'OccupationClass'  

exec sp_addextendedproperty 'MS_Description'
    ,'Occupation Industry Code of the risk, code comes from TALUS interview'
    ,'user', 'dbo'
    ,'table', 'RiskOccupation'
    ,'column', 'IndustryAnswerId'   

exec sp_addextendedproperty 'MS_Description'
    ,'Industry Title of the risk, title comes from TALUS interview'
    ,'user', 'dbo'
    ,'table', 'RiskOccupation'
    ,'column', 'IndustryTitle'   

exec sp_addextendedproperty 'MS_Description'
    ,'TPD Specific Occupation Loading of the risk, value comes from TALUS interview'
    ,'user', 'dbo'
    ,'table', 'RiskOccupation'
    ,'column', 'TpdOccupationLoading'  

exec sp_addextendedproperty 'MS_Description'
    ,'TPD Any Available Flag for the risk, value comes from TALUS interview'
    ,'user', 'dbo'
    ,'table', 'RiskOccupation'
    ,'column', 'IsTpdAny'  

exec sp_addextendedproperty 'MS_Description'
    ,'TPD Own Available Flag for the risk, value comes from TALUS interview'
    ,'user', 'dbo'
    ,'table', 'RiskOccupation'
    ,'column', 'IsTpdOwn'  

exec sp_addextendedproperty 'MS_Description'
    ,'Pas Occupation Code of the risk, value comes from TALUS interview'
    ,'user', 'dbo'
    ,'table', 'RiskOccupation'
    ,'column', 'PasCode'  

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'RiskOccupation'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'RiskOccupation'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'RiskOccupation'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'RiskOccupation'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'RiskOccupation'
    ,'column', 'ModifiedTS'  