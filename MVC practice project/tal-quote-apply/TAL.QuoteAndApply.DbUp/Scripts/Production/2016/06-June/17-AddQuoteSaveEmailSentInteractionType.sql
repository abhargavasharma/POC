
if not exists(select id from [dbo].[InteractionType] where id = 10)
INSERT INTO [dbo].[InteractionType] ([Id] ,[Description]) VALUES (10, 'Quote_Save_Email_Sent');
GO
