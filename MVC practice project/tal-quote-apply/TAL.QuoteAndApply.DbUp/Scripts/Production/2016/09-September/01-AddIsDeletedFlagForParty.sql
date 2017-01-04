ALTER TABLE [dbo].[Party]
ADD [IsDeleted] BIT NOT NULL DEFAULT(0)

EXEC sp_addextendedproperty 'MS_Description'
    ,'Shows if the party was deleted or not'
    ,'SCHEMA', 'dbo'
    ,'TABLE', 'Party'
    ,'COLUMN', 'IsDeleted'  