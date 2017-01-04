CREATE TABLE [dbo].[PolicyProgress](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PolicyProgress] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a PolicyProgress'
    ,'user', 'dbo', 'table'
    ,'PolicyProgress'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the PolicyProgress'
    ,'user', 'dbo'
    ,'table', 'PolicyProgress'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the PolicyProgress'
    ,'user', 'dbo'
    ,'table', 'PolicyProgress'
    ,'column', 'Description'  

GO

INSERT INTO [PolicyProgress] ([Id], [Description]) VALUES (0, 'Unknown');
INSERT INTO [PolicyProgress] ([Id], [Description]) VALUES (1, 'InProgressPreUw');
INSERT INTO [PolicyProgress] ([Id], [Description]) VALUES (2, 'InProgressUwReferral');
INSERT INTO [PolicyProgress] ([Id], [Description]) VALUES (3, 'InProgressRecommendation');
INSERT INTO [PolicyProgress] ([Id], [Description]) VALUES (4, 'InProgressCantContact');
INSERT INTO [PolicyProgress] ([Id], [Description]) VALUES (5, 'ClosedSale');
INSERT INTO [PolicyProgress] ([Id], [Description]) VALUES (6, 'ClosedNoSale');
INSERT INTO [PolicyProgress] ([Id], [Description]) VALUES (7, 'ClosedTriage');
INSERT INTO [PolicyProgress] ([Id], [Description]) VALUES (8, 'ClosedCantContact');

GO