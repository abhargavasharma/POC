SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Title](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Title] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Gender](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Gender] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[State](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[Country](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[SmokerStatus](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SmokerStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[UnderwritingStatus](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_UnderwritingStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[PremiumFrequency](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_PremiumFrequency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Policy]
ADD CONSTRAINT FK_Policy_PremiumFrequencyId  FOREIGN KEY (PremiumFrequencyId)
    REFERENCES PremiumFrequency(Id);

GO

CREATE TABLE [dbo].[PremiumType](
	[Id] [int] NOT NULL,
	[Code] [nvarchar](7) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PremiumType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Plan]
ADD CONSTRAINT FK_Plan_PremiumTypeId  FOREIGN KEY (PremiumTypeId)
    REFERENCES PremiumType(Id);

GO

CREATE TABLE [dbo].[InteractionType](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
CONSTRAINT [PK_InteractionType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[OccupationDefinitionType](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_OccupationDefinitionType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

-- POPULATE DATA --
GO

INSERT INTO [Title] ([Id], [Description]) VALUES (0, 'Unknown');
INSERT INTO [Title] ([Id], [Description]) VALUES (1, 'Dr');
INSERT INTO [Title] ([Id], [Description]) VALUES (2, 'Mr');
INSERT INTO [Title] ([Id], [Description]) VALUES (3, 'Mrs');
INSERT INTO [Title] ([Id], [Description]) VALUES (4, 'Miss');
INSERT INTO [Title] ([Id], [Description]) VALUES (5, 'Ms');

GO

INSERT INTO [State] ([Id], [Description]) VALUES (0, 'Unknown');
INSERT INTO [State] ([Id], [Description]) VALUES (1, 'ACT');
INSERT INTO [State] ([Id], [Description]) VALUES (2, 'NSW');
INSERT INTO [State] ([Id], [Description]) VALUES (3, 'NT');
INSERT INTO [State] ([Id], [Description]) VALUES (4, 'QLD');
INSERT INTO [State] ([Id], [Description]) VALUES (5, 'SA');
INSERT INTO [State] ([Id], [Description]) VALUES (6, 'TAS');
INSERT INTO [State] ([Id], [Description]) VALUES (7, 'WA');
INSERT INTO [State] ([Id], [Description]) VALUES (8, 'VIC');


GO

INSERT INTO [Gender] ([Id], [Description]) VALUES (0, 'Unknown');
INSERT INTO [Gender] ([Id], [Description]) VALUES (1, 'Male');
INSERT INTO [Gender] ([Id], [Description]) VALUES (2, 'Female');

GO

INSERT INTO [Country] ([Id], [Description]) VALUES (0, 'Unknown');
INSERT INTO [Country] ([Id], [Description]) VALUES (1, 'Australia');

GO

INSERT INTO [SmokerStatus] ([Id], [Description]) VALUES (0, 'Unknown');
INSERT INTO [SmokerStatus] ([Id], [Description]) VALUES (1, 'HaventSmoked');
INSERT INTO [SmokerStatus] ([Id], [Description]) VALUES (2, 'SmokedLessThan10');
INSERT INTO [SmokerStatus] ([Id], [Description]) VALUES (3, 'Smoked10To20');
INSERT INTO [SmokerStatus] ([Id], [Description]) VALUES (4, 'SmokedMoreThan20');

GO

INSERT INTO [UnderwritingStatus] ([Id], [Description]) VALUES (1, 'Accept');
INSERT INTO [UnderwritingStatus] ([Id], [Description]) VALUES (2, 'Decline');
INSERT INTO [UnderwritingStatus] ([Id], [Description]) VALUES (3, 'Defer');
INSERT INTO [UnderwritingStatus] ([Id], [Description]) VALUES (4, 'Incomplete');
INSERT INTO [UnderwritingStatus] ([Id], [Description]) VALUES (5, 'MoreInfo');
INSERT INTO [UnderwritingStatus] ([Id], [Description]) VALUES (6, 'Refer');

GO

Insert into [PremiumFrequency] ([Id], [Description]) Values(0, 'Unknown');
Insert into [PremiumFrequency] ([Id], [Description]) Values(1, 'Monthly');
Insert into [PremiumFrequency] ([Id], [Description]) Values(2, 'Quarterly');
Insert into [PremiumFrequency] ([Id], [Description]) Values(3, 'Half yearly');
Insert into [PremiumFrequency] ([Id], [Description]) Values(4, 'Yearly');

GO

INSERT INTO [PremiumType] ([Id], [Code], [Description]) VALUES (0, 'Unknown', 'Unknown');
INSERT INTO [PremiumType] ([Id], [Code], [Description]) VALUES (1, 'Stepped', 'Stepped');
INSERT INTO [PremiumType] ([Id], [Code], [Description]) VALUES (2, 'Level', 'Level');

GO

INSERT INTO [InteractionType] ([Id], [Description]) VALUES (1, 'Quote_Accessed');

GO

INSERT INTO [OccupationDefinitionType] ([Id], [Description]) VALUES (0, 'Unknown');
INSERT INTO [OccupationDefinitionType] ([Id], [Description]) VALUES (1, 'AnyOccupation');
INSERT INTO [OccupationDefinitionType] ([Id], [Description]) VALUES (2, 'OwnOccupation');

GO