CREATE TABLE [dbo].[PolicySource](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PolicySource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

-- POPULATE DATA --
GO

INSERT INTO [PolicySource] ([Id], [Description]) VALUES (0, 'Unknown');
INSERT INTO [PolicySource] ([Id], [Description]) VALUES (1, 'SalesPortal');
INSERT INTO [PolicySource] ([Id], [Description]) VALUES (2, 'CustomerPortalHelpMeChoose');
INSERT INTO [PolicySource] ([Id], [Description]) VALUES (3, 'CustomerPortalBuildMyOwn');

GO