ALTER TABLE [dbo].[Policy]
ADD [PolicySourceId] INT NOT NULL DEFAULT(0) FOREIGN KEY REFERENCES [PolicySource]([Id])


exec sp_addextendedproperty 'MS_Description'
    ,'The source that created the policy'
    ,'user', 'dbo'
    ,'table', 'Policy'
    ,'column', 'PolicySourceId'  

GO