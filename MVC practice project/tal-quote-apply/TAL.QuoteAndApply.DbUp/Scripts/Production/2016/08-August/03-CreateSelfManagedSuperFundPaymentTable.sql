/****** Object:  Table [dbo].[SelfManagedSuperFundPayment]    Script Date: 18/08/2016 12:54:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SelfManagedSuperFundPayment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccountName] [nvarchar](255) NULL,
	[BSBNumber] [nvarchar](25) NULL,
	[AccountNumber] [nvarchar](50) NULL,
	[PolicyPaymentId] [int] NULL,
	[RV] [timestamp] NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[CreatedTS] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](256) NULL,
	[ModifiedTS] [datetime] NULL,
 CONSTRAINT [PK_SelfManagedSuperFundPayment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SelfManagedSuperFundPayment]  WITH CHECK ADD FOREIGN KEY([PolicyPaymentId])
REFERENCES [dbo].[PolicyPayment] ([Id])
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Row version time stamp' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SelfManagedSuperFundPayment', @level2type=N'COLUMN',@level2name=N'RV'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The user the record was created by' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SelfManagedSuperFundPayment', @level2type=N'COLUMN',@level2name=N'CreatedBy'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The date time the record was created' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SelfManagedSuperFundPayment', @level2type=N'COLUMN',@level2name=N'CreatedTS'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The user that last modified the record' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SelfManagedSuperFundPayment', @level2type=N'COLUMN',@level2name=N'ModifiedBy'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The date time the record was last modified' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SelfManagedSuperFundPayment', @level2type=N'COLUMN',@level2name=N'ModifiedTS'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Holds information about a SelfManagedSuperFundPayment' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SelfManagedSuperFundPayment'
GO


