CREATE TABLE [dbo].[LoadingType](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_LoadingType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a LoadingType'
    ,'user', 'dbo', 'table'
    ,'LoadingType'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the LoadingType'
    ,'user', 'dbo'
    ,'table', 'LoadingType'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the LoadingType'
    ,'user', 'dbo'
    ,'table', 'LoadingType'
    ,'column', 'Description'  

GO

INSERT INTO [LoadingType] ([Id], [Description]) VALUES (0, 'Unknown');
INSERT INTO [LoadingType] ([Id], [Description]) VALUES (1, 'Variable');
INSERT INTO [LoadingType] ([Id], [Description]) VALUES (2, 'Fixed');
INSERT INTO [LoadingType] ([Id], [Description]) VALUES (3, 'PerMille');

GO

CREATE TABLE [dbo].[CoverLoading](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CoverId] [int] NOT NULL FOREIGN KEY REFERENCES [Cover]([Id]),
	[LoadingTypeId] [int] NOT NULL FOREIGN KEY REFERENCES [LoadingType]([Id]),
	[Loading] DECIMAL(18,2) NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_CoverLoading] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a CoverLoading'
    ,'user', 'dbo', 'table'
    ,'CoverLoading'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique identifier of the CoverLoading'
    ,'user', 'dbo'
    ,'table', 'CoverLoading'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Cover the loading is linked to. References Cover table'
    ,'user', 'dbo'
    ,'table', 'CoverLoading'
    ,'column', 'CoverId'  

exec sp_addextendedproperty 'MS_Description'
    ,'Loading Type the loading is linked to. References LoadingType table'
    ,'user', 'dbo'
    ,'table', 'CoverLoading'
    ,'column', 'LoadingTypeId'  

exec sp_addextendedproperty 'MS_Description'
    ,'Loading amount'
    ,'user', 'dbo'
    ,'table', 'CoverLoading'
    ,'column', 'Loading'  

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'CoverLoading'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'CoverLoading'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'CoverLoading'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'CoverLoading'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'CoverLoading'
    ,'column', 'ModifiedTS'
GO