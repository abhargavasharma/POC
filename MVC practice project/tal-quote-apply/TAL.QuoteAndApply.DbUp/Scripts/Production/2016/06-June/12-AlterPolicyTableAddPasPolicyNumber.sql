ALTER TABLE [dbo].[Policy]
ADD [PasPolicyNumber] NVARCHAR(256) NULL

exec sp_addextendedproperty 'MS_Description'
    ,'Holds the corresponding PasPolicyNumber after a submit to PAS has occurred'
    ,'user', 'dbo'
    ,'table', 'Policy'
    ,'column', 'PasPolicyNumber'  