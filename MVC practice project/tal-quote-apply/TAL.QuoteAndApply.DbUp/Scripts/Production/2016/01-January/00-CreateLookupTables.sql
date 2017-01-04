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

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a Title'
    ,'user', 'dbo', 'table'
    ,'Title'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the Title'
    ,'user', 'dbo'
    ,'table', 'Title'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the Title'
    ,'user', 'dbo'
    ,'table', 'Title'
    ,'column', 'Description'  

GO

CREATE TABLE [dbo].[Gender](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Gender] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a Gender'
    ,'user', 'dbo', 'table'
    ,'Gender'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the Gender'
    ,'user', 'dbo'
    ,'table', 'Gender'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the Gender'
    ,'user', 'dbo'
    ,'table', 'Gender'
    ,'column', 'Description'  

GO

CREATE TABLE [dbo].[State](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a State'
    ,'user', 'dbo', 'table'
    ,'State'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the State'
    ,'user', 'dbo'
    ,'table', 'State'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the State'
    ,'user', 'dbo'
    ,'table', 'State'
    ,'column', 'Description'  

GO


CREATE TABLE [dbo].[Country](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a Country'
    ,'user', 'dbo', 'table'
    ,'Country'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the Country'
    ,'user', 'dbo'
    ,'table', 'Country'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the Country'
    ,'user', 'dbo'
    ,'table', 'Country'
    ,'column', 'Description'  

GO


CREATE TABLE [dbo].[SmokerStatus](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SmokerStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a SmokerStatus'
    ,'user', 'dbo', 'table'
    ,'SmokerStatus'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the SmokerStatus'
    ,'user', 'dbo'
    ,'table', 'SmokerStatus'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the SmokerStatus'
    ,'user', 'dbo'
    ,'table', 'SmokerStatus'
    ,'column', 'Description'  

GO


CREATE TABLE [dbo].[PreferredContactMethod](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PreferredContactMethod] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a PreferredContactMethod'
    ,'user', 'dbo', 'table'
    ,'PreferredContactMethod'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the PreferredContactMethod'
    ,'user', 'dbo'
    ,'table', 'PreferredContactMethod'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the PreferredContactMethod'
    ,'user', 'dbo'
    ,'table', 'PreferredContactMethod'
    ,'column', 'Description'  

GO

CREATE TABLE [dbo].[UnderwritingStatus](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_UnderwritingStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a UnderwritingStatus'
    ,'user', 'dbo', 'table'
    ,'UnderwritingStatus'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the UnderwritingStatus'
    ,'user', 'dbo'
    ,'table', 'UnderwritingStatus'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the PreferredContactMethod'
    ,'user', 'dbo'
    ,'table', 'UnderwritingStatus'
    ,'column', 'Description'  

GO

CREATE TABLE [dbo].[PremiumFrequency](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NULL,
 CONSTRAINT [PK_PremiumFrequency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a PremiumFrequency'
    ,'user', 'dbo', 'table'
    ,'PremiumFrequency'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the PremiumFrequency'
    ,'user', 'dbo'
    ,'table', 'PremiumFrequency'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the PremiumFrequency'
    ,'user', 'dbo'
    ,'table', 'PremiumFrequency'
    ,'column', 'Description'  

GO

CREATE TABLE [dbo].[PremiumType](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PremiumType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a PremiumType'
    ,'user', 'dbo', 'table'
    ,'PremiumType'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the PremiumType'
    ,'user', 'dbo'
    ,'table', 'PremiumType'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the PremiumType'
    ,'user', 'dbo'
    ,'table', 'PremiumType'
    ,'column', 'Description'  

GO


CREATE TABLE [dbo].[InteractionType](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
CONSTRAINT [PK_InteractionType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a InteractionType'
    ,'user', 'dbo', 'table'
    ,'InteractionType'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the InteractionType'
    ,'user', 'dbo'
    ,'table', 'InteractionType'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the InteractionType'
    ,'user', 'dbo'
    ,'table', 'InteractionType'
    ,'column', 'Description'  

GO

CREATE TABLE [dbo].[OccupationDefinitionType](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_OccupationDefinitionType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a OccupationDefinitionType'
    ,'user', 'dbo', 'table'
    ,'OccupationDefinitionType'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the OccupationDefinitionType'
    ,'user', 'dbo'
    ,'table', 'OccupationDefinitionType'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the OccupationDefinitionType'
    ,'user', 'dbo'
    ,'table', 'OccupationDefinitionType'
    ,'column', 'Description'  

GO

CREATE TABLE [dbo].[PolicyStatus](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PolicyStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a PolicyStatus'
    ,'user', 'dbo', 'table'
    ,'PolicyStatus'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the PolicyStatus'
    ,'user', 'dbo'
    ,'table', 'PolicyStatus'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the PolicyStatus'
    ,'user', 'dbo'
    ,'table', 'PolicyStatus'
    ,'column', 'Description'  

GO


-- POPULATE DATA --

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

INSERT INTO [PreferredContactMethod] ([Id], [Description]) VALUES (0, 'Unknown');
INSERT INTO [PreferredContactMethod] ([Id], [Description]) VALUES (1, 'Phone');
INSERT INTO [PreferredContactMethod] ([Id], [Description]) VALUES (2, 'SMS');
INSERT INTO [PreferredContactMethod] ([Id], [Description]) VALUES (3, 'Email');

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

INSERT INTO [PremiumType] ([Id],[Description]) VALUES (0, 'Unknown');
INSERT INTO [PremiumType] ([Id],[Description]) VALUES (1, 'Stepped');
INSERT INTO [PremiumType] ([Id],[Description]) VALUES (2, 'Level');

GO

INSERT INTO [InteractionType] ([Id], [Description]) VALUES (1, 'Quote_Accessed');
INSERT INTO [InteractionType] ([Id], [Description]) VALUES (2, 'Pipeline_Status_Change');

GO

INSERT INTO [OccupationDefinitionType] ([Id], [Description]) VALUES (0, 'Unknown');
INSERT INTO [OccupationDefinitionType] ([Id], [Description]) VALUES (1, 'AnyOccupation');
INSERT INTO [OccupationDefinitionType] ([Id], [Description]) VALUES (2, 'OwnOccupation');

GO

INSERT INTO [PolicyStatus] ([Id], [Description]) VALUES (1, 'Incomplete');
INSERT INTO [PolicyStatus] ([Id], [Description]) VALUES (2, 'ReferredToUnderwriter');
INSERT INTO [PolicyStatus] ([Id], [Description]) VALUES (3, 'ReadyForInforce');
INSERT INTO [PolicyStatus] ([Id], [Description]) VALUES (4, 'RaisedToPolicyAdminSystem');

GO
