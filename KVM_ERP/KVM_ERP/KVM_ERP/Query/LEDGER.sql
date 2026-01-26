USE [SYNC]
GO
/****** Object:  Table [dbo].[LEDGER]    Script Date: 16/08/2023 17:05:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LEDGER](
	[LDGRID] [int] NULL,
	[LDGRDESC] [nvarchar](100) NULL,
	[LDGRGID] [int] NULL,
	[LDGRCODE] [nvarchar](10) NULL,
	[DISPSTATUS] [int] NULL,
	[PRCSDATE] [datetime] NULL,
	[BRNCHID] [int] NULL
) ON [PRIMARY]
GO
