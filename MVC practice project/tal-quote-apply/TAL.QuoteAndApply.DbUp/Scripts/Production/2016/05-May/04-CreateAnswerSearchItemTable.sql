CREATE TABLE [dbo].[AnswerSearchItem](
	[Id] INT IDENTITY(1,1) NOT NULL,
	[IndexKey] NVARCHAR(256) NOT NULL,
	[ResponseId] NVARCHAR(50) NOT NULL,
	[Text] NVARCHAR(256) NOT NULL,
	[HelpText] NVARCHAR(256) NULL,
	[Tags] NVARCHAR(512) NULL,
	[ParentId] NVARCHAR(50) NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_AnswerSearchItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX_AnswerSearchItem_IndexKey] ON [dbo].[AnswerSearchItem]
(
	[IndexKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about answers in talus we want to perform an in memory index search on'
    ,'user', 'dbo', 'table'
    ,'AnswerSearchItem'

exec sp_addextendedproperty 'MS_Description'
    ,'Primary key, auto incrementing'
    ,'user', 'dbo'
    ,'table', 'AnswerSearchItem'
    ,'column', 'Id'

exec sp_addextendedproperty 'MS_Description'
    ,'An index key comprised on the template version and question hash from talus'
    ,'user', 'dbo'
    ,'table', 'AnswerSearchItem'
    ,'column', 'IndexKey'

exec sp_addextendedproperty 'MS_Description'
    ,'The answer id of a question from talus'
    ,'user', 'dbo'
    ,'table', 'AnswerSearchItem'
    ,'column', 'ResponseId'

exec sp_addextendedproperty 'MS_Description'
    ,'The string value of the answer from talus'
    ,'user', 'dbo'
    ,'table', 'AnswerSearchItem'
    ,'column', 'Text'

exec sp_addextendedproperty 'MS_Description'
    ,'The additional help text of the answer from talus'
    ,'user', 'dbo'
    ,'table', 'AnswerSearchItem'
    ,'column', 'HelpText'

exec sp_addextendedproperty 'MS_Description'
    ,'A comma separated list of tags related to the answer'
    ,'user', 'dbo'
    ,'table', 'AnswerSearchItem'
    ,'column', 'Tags'

exec sp_addextendedproperty 'MS_Description'
    ,'A response id of a parent quetion answer'
    ,'user', 'dbo'
    ,'table', 'AnswerSearchItem'
    ,'column', 'ParentId'

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'AnswerSearchItem'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'AnswerSearchItem'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'AnswerSearchItem'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'AnswerSearchItem'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'AnswerSearchItem'
    ,'column', 'ModifiedTS'  