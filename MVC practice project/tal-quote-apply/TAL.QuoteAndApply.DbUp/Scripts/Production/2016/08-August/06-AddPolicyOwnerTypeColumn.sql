ALTER TABLE [dbo].[PolicyOwner]
ADD [PolicyOwnerTypeId] INT NOT NULL DEFAULT(0) FOREIGN KEY REFERENCES [PolicyOwnerType]([Id])

ALTER TABLE [dbo].[PolicyOwner]
ADD [FundName] [nvarchar](MAX)

EXEC sp_addextendedproperty 'MS_Description'
    ,'The owner type of the policy'
    ,'SCHEMA', 'dbo'
    ,'TABLE', 'PolicyOwner'
    ,'COLUMN', 'PolicyOwnerTypeId'  

EXEC sp_addextendedproperty 'MS_Description'
	, 'Fund name of the self managed super fund if the owner is SMSF' 
	, 'SCHEMA', 'dbo'
	, 'TABLE', 'PolicyOwner'
	, 'COLUMN', 'FundName'