CREATE TABLE [dbo].[RaisePolicyAuditType](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_RaisePolicyAuditType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a RaisePolicyAuditType'
    ,'user', 'dbo', 'table'
    ,'RaisePolicyAuditType'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the RaisePolicyAuditType'
    ,'user', 'dbo'
    ,'table', 'RaisePolicyAuditType'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the RaisePolicyAuditType'
    ,'user', 'dbo'
    ,'table', 'RaisePolicyAuditType'
    ,'column', 'Description'  

GO

INSERT INTO [RaisePolicyAuditType] ([Id], [Description]) VALUES (0, 'Unknown');
INSERT INTO [RaisePolicyAuditType] ([Id], [Description]) VALUES (1, 'Submit');
INSERT INTO [RaisePolicyAuditType] ([Id], [Description]) VALUES (2, 'Feedback');

GO

ALTER TABLE [dbo].[RaisePolicySubmissionAudit]
ADD [RaisePolicyAuditTypeId] INT NOT NULL DEFAULT(1) FOREIGN KEY REFERENCES [RaisePolicyAuditType]([Id])


exec sp_addextendedproperty 'MS_Description'
    ,'The pipeline status of the policy when in progress'
    ,'user', 'dbo'
    ,'table', 'RaisePolicySubmissionAudit'
    ,'column', 'RaisePolicyAuditTypeId'  
	
GO