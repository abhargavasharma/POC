ALTER TABLE [dbo].[Policy]
ADD [PolicyProgressId] INT NOT NULL DEFAULT(0) FOREIGN KEY REFERENCES [PolicyProgress]([Id])


exec sp_addextendedproperty 'MS_Description'
    ,'The pipeline status of the policy when in progress'
    ,'user', 'dbo'
    ,'table', 'Policy'
    ,'column', 'PolicyProgressId'  