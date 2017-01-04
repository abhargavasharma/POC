
if not exists(select id from [dbo].[InteractionType] where id = 13 OR id = 14)
INSERT INTO [dbo].[InteractionType] ([Id] ,[Description]) VALUES (13, 'Quote_Email_Sent_From_Sales_Portal');
INSERT INTO [dbo].[InteractionType] ([Id] ,[Description]) VALUES (14, 'Application_Confirmation_Email_Sent');
GO
