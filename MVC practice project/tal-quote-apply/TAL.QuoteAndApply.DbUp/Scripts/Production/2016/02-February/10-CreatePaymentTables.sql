SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PaymentType](
	[Id] [int] NOT NULL,
	[Description] NVARCHAR(50) NULL
 CONSTRAINT [PK_PaymentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a PaymentType'
    ,'user', 'dbo', 'table'
    ,'PaymentType'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the PaymentType'
    ,'user', 'dbo'
    ,'table', 'PaymentType'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the PaymentType'
    ,'user', 'dbo'
    ,'table', 'PaymentType'
    ,'column', 'Description'  

GO

INSERT INTO [PaymentType] ([Id], [Description]) VALUES(0, 'Unkown')
INSERT INTO [PaymentType] ([Id], [Description]) VALUES(1, 'CreditCard')
INSERT INTO [PaymentType] ([Id], [Description]) VALUES(2, 'DirectDebit')
INSERT INTO [PaymentType] ([Id], [Description]) VALUES(3, 'SuperAnnuation')

GO

/* -- LINKING TABLES -- */

CREATE TABLE [dbo].[PolicyPayment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PolicyId] [int] FOREIGN KEY REFERENCES [Policy] ([Id]),
	[PaymentTypeId] [int] FOREIGN KEY REFERENCES [PaymentType]([Id]),
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_PolicyPayment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a PolicyPayment'
    ,'user', 'dbo', 'table'
    ,'PolicyPayment'

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'PolicyPayment'
    ,'column', 'RV'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'PolicyPayment'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'PolicyPayment'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'PolicyPayment'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'PolicyPayment'
    ,'column', 'ModifiedTS'  


GO

CREATE TABLE [dbo].[CreditCardPayment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreditCardTypeId] [int] NOT NULL,
	[NameOnCard] NVARCHAR(255) NULL,
	[CardNumber] NVARCHAR(255) NULL,
	[ExpiryMonth] NVARCHAR(2) NULL,
	[ExpiryYear] NVARCHAR(2) NULL,
	[Token] NVARCHAR(255) NULL,
	[PolicyPaymentId] INT FOREIGN KEY REFERENCES [PolicyPayment]([Id]),
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_CreditCardPayment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a CreditCardPayment'
    ,'user', 'dbo', 'table'
    ,'CreditCardPayment'

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'CreditCardPayment'
    ,'column', 'RV'  


exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'CreditCardPayment'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'CreditCardPayment'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'CreditCardPayment'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'CreditCardPayment'
    ,'column', 'ModifiedTS'  


GO

CREATE TABLE [dbo].[CreditCardType](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_CreditCardType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a CreditCardType'
    ,'user', 'dbo', 'table'
    ,'CreditCardType'

exec sp_addextendedproperty 'MS_Description'
    ,'Unique Id of the CreditCardType'
    ,'user', 'dbo'
    ,'table', 'CreditCardType'
    ,'column', 'Id'  

exec sp_addextendedproperty 'MS_Description'
    ,'Description of the CreditCardType'
    ,'user', 'dbo'
    ,'table', 'CreditCardType'
    ,'column', 'Description'  

GO

INSERT INTO [CreditCardType] ([Id], [Description]) VALUES(0, 'Unkown')
INSERT INTO [CreditCardType] ([Id], [Description]) VALUES(1, 'Visa')
INSERT INTO [CreditCardType] ([Id], [Description]) VALUES(2, 'MasterCard')
INSERT INTO [CreditCardType] ([Id], [Description]) VALUES(3, 'Amex')

GO

CREATE TABLE [dbo].[DirectDebitPayment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccountName] NVARCHAR(255) NULL,
	[BSBNumber] NVARCHAR(25) NULL,
	[AccountNumber] NVARCHAR(50) NULL,
	[PolicyPaymentId] INT FOREIGN KEY REFERENCES [PolicyPayment]([Id]),
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_DirectDebitPayment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a DirectDebitPayment'
    ,'user', 'dbo', 'table'
    ,'DirectDebitPayment'

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'DirectDebitPayment'
    ,'column', 'RV'  


exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'DirectDebitPayment'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'DirectDebitPayment'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'DirectDebitPayment'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'DirectDebitPayment'
    ,'column', 'ModifiedTS'  


GO

CREATE TABLE [dbo].[SuperAnnuationPayment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FundName] NVARCHAR(255) NULL,
	[FundABN] NVARCHAR(25) NULL,
	[FundUSI] NVARCHAR(255) NULL,
	[FundProduct] NVARCHAR(255) NULL,
	[MembershipNumber] NVARCHAR(50) NULL,
	[TaxFileNumber] NVARCHAR(50) NULL,
	[PolicyPaymentId] INT FOREIGN KEY REFERENCES [PolicyPayment]([Id]),
	[RV] [timestamp] NOT NULL,
	[CreatedBy] NVARCHAR(256) NOT NULL,
	[CreatedTS] DATETIME NOT NULL,
	[ModifiedBy] NVARCHAR(256) NULL,
	[ModifiedTS] DATETIME NULL,
 CONSTRAINT [PK_SuperAnnuationPayment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

exec sp_addextendedproperty 'MS_Description'
    ,'Holds information about a SuperAnnuationPayment'
    ,'user', 'dbo', 'table'
    ,'SuperAnnuationPayment'

exec sp_addextendedproperty 'MS_Description'
    ,'Row version time stamp'
    ,'user', 'dbo'
    ,'table', 'SuperAnnuationPayment'
    ,'column', 'RV'  


exec sp_addextendedproperty 'MS_Description'
    ,'The user the record was created by'
    ,'user', 'dbo'
    ,'table', 'SuperAnnuationPayment'
    ,'column', 'CreatedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was created'
    ,'user', 'dbo'
    ,'table', 'SuperAnnuationPayment'
    ,'column', 'CreatedTS'  

exec sp_addextendedproperty 'MS_Description'
    ,'The user that last modified the record'
    ,'user', 'dbo'
    ,'table', 'SuperAnnuationPayment'
    ,'column', 'ModifiedBy'  

exec sp_addextendedproperty 'MS_Description'
    ,'The date time the record was last modified'
    ,'user', 'dbo'
    ,'table', 'SuperAnnuationPayment'
    ,'column', 'ModifiedTS'  


GO


