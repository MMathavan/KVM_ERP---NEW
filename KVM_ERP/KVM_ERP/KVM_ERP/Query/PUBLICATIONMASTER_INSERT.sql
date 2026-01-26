USE [MEDIAPRO_ERP];
GO

-- Enable IDENTITY_INSERT
SET IDENTITY_INSERT [dbo].[PUBLICATIONMASTER] ON;
GO

-- Perform the insert, including PUBLNID
INSERT INTO [dbo].[PUBLICATIONMASTER]
           ([PUBLNID],
		   [PUBLNNAME],
           [PUBLNDNAME],
           [PUBLNADDR],
           [PUBLNPHN1],
           [PUBLNPHN2],
           [PUBLNPHN3],
           [PUBLNPHN4],
           [PUBLNCPRSN],
           [PUBLNCODE],
           [PUBLNWSITE],
           [PUBLNEMAIL],
           [LANGID],
           [LDGRID],
           [PUBLNTID],
           [STATEID],
           [PUBLNGSTNO],
           [PUBLNPANNO],
           [CUSRID],
           [LMUSRID],
           [DISPSTATUS],
           [PRCSDATE])
SELECT 
           [PUBLNID],
           [PUBLNNAME],
           [PUBLNDNAME],
           [PUBLNADDR],
           [PUBLNPHN1],
           [PUBLNPHN2],
           [PUBLNPHN3],
           [PUBLNPHN4],
           [PUBLNCPRSN],
           ISNULL([PUBLNCODE], 'Nil'),
           [PUBLNWSITE],
           [PUBLNEMAIL],
           [LANGID],
           [LDGRID],
           [PUBLNTID],
           0, -- STATEID hardcoded to 0
           [PUBLNGSTNO],
           [PUBLNPANNO],
           'admin',
           'admin',
           [DISPSTATUS],
           GETDATE()
FROM PUBLICATION;
GO

-- Disable IDENTITY_INSERT
SET IDENTITY_INSERT [dbo].[PUBLICATIONMASTER] OFF;
GO
