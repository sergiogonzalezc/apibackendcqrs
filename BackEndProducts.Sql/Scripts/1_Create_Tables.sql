
USE [BD_Products]
GO
/* truncate table dbo.BD_Products  */
/**************************** CREATION TABLES ******************************/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO

GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Product]') AND type in (N'U'))
BEGIN

CREATE TABLE [dbo].[Product](
	[ProductId] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Status] [int] NOT NULL,
	[Stock] [int] NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[Price] [numeric](10, 3) NOT NULL,
	[Discount] [numeric](10, 3) NOT NULL,
	[FinalPrice]  AS (([Price]*((100)-[Discount]))/(100)),
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

END
GO

ALTER TABLE [dbo].[Product] ADD  DEFAULT ((0)) FOR [Stock]
ALTER TABLE [dbo].[Product] ADD  DEFAULT ((0)) FOR [Price]
ALTER TABLE [dbo].[Product] ADD  DEFAULT ((0)) FOR [Discount]

GO

