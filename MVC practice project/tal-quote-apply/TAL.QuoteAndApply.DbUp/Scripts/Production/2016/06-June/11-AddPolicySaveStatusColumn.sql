

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'PolicySaveStatus')
	BEGIN
		CREATE TABLE [dbo].[PolicySaveStatus](
			[Id] [int] NOT NULL,
			[Description] [nvarchar](50) NOT NULL,
			CONSTRAINT [PK_PolicySaveStatus] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		exec sp_addextendedproperty 'MS_Description'
			,'Holds information about a PolicySaveStatus'
			,'user', 'dbo', 'table'
			,'PolicySaveStatus'

		exec sp_addextendedproperty 'MS_Description'
			,'Unique Id of the PolicySaveStatus'
			,'user', 'dbo'
			,'table', 'PolicySaveStatus'
			,'column', 'Id'  

		exec sp_addextendedproperty 'MS_Description'
			,'Description of the PolicySaveStatus'
			,'user', 'dbo'
			,'table', 'PolicySaveStatus'
			,'column', 'Description'  

		INSERT INTO [PolicySaveStatus] ([Id], [Description]) VALUES (0, 'Not Saved');
		INSERT INTO [PolicySaveStatus] ([Id], [Description]) VALUES (1, 'PersonalDetailsEntered');
		INSERT INTO [PolicySaveStatus] ([Id], [Description]) VALUES (2, 'LoginCreated');
END	


IF NOT EXISTS(
    SELECT *
    FROM sys.columns 
    WHERE Name = N'SaveStatus'
    AND Object_ID = Object_ID(N'Policy'))
BEGIN
	ALTER TABLE [dbo].[Policy] ADD 	[SaveStatus] INT NOT NULL DEFAULT(0) FOREIGN KEY REFERENCES [PolicySaveStatus]([Id]);
	
	exec sp_addextendedproperty 'MS_Description'
		,'Status of the customers save progress, links to SaveStatus table'
		,'user', 'dbo'
		,'table', 'Policy'
		,'column', 'SaveStatus'  
END

GO