
if not exists(select id from [dbo].[InteractionType] where id = 2)
INSERT INTO [dbo].[InteractionType] ([Id] ,[Description]) VALUES (2, 'Pipeline_Status_Change');
GO
