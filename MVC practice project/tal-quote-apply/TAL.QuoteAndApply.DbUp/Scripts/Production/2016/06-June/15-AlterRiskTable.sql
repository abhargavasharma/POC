ALTER TABLE [dbo].[Risk]
ADD [InterviewStatusId] [int] NOT NULL DEFAULT (0) WITH VALUES

GO

exec sp_addextendedproperty 'MS_Description'
    ,'Determine the result of the of the interview for the risk'
    ,'user', 'dbo'
    ,'table', 'Risk'
    ,'column', 'InterviewStatusId' 

GO

CREATE TABLE [dbo].[InterviewStatus](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_InterviewStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [InterviewStatus] ([Id], [Description]) VALUES (0, 'Unknown');
INSERT INTO [InterviewStatus] ([Id], [Description]) VALUES (1, 'Incomplete');
INSERT INTO [InterviewStatus] ([Id], [Description]) VALUES (2, 'Complete');
INSERT INTO [InterviewStatus] ([Id], [Description]) VALUES (3, 'Referred');
