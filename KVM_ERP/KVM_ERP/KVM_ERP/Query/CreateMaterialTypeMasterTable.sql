-- Create Material Type Master Table
-- Run this script in your SQL Server database to create the MATERIALTYPEMASTER table

USE [YourDatabaseName] -- Replace with your actual database name
GO

-- Check if table exists and drop if it does (optional - remove if you want to preserve existing data)
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MATERIALTYPEMASTER')
BEGIN
    PRINT 'Table MATERIALTYPEMASTER already exists. Skipping creation.'
END
ELSE
BEGIN
    CREATE TABLE [dbo].[MATERIALTYPEMASTER](
        [MTRLTID] [int] IDENTITY(1,1) NOT NULL,
        [MTRLTDESC] [varchar](100) NOT NULL,
        [MTRLTCODE] [varchar](15) NOT NULL,
        [CUSRID] [varchar](100) NOT NULL,
        [LMUSRID] [varchar](100) NULL,
        [DISPSTATUS] [smallint] NOT NULL,
        [PRCSDATE] [datetime] NOT NULL,
        [DISPORDER] [int] NOT NULL,
     CONSTRAINT [PK_MATERIALTYPEMASTER] PRIMARY KEY CLUSTERED 
    (
        [MTRLTID] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

    PRINT 'Table MATERIALTYPEMASTER created successfully.'
    
    -- Add unique constraint on MTRLTCODE
    ALTER TABLE [dbo].[MATERIALTYPEMASTER] ADD CONSTRAINT [UC_MATERIALTYPEMASTER_CODE] UNIQUE ([MTRLTCODE])
    
    PRINT 'Unique constraint added on MTRLTCODE.'
    
    -- Insert some sample data (optional)
    INSERT INTO [dbo].[MATERIALTYPEMASTER] 
    ([MTRLTDESC], [MTRLTCODE], [CUSRID], [LMUSRID], [DISPSTATUS], [PRCSDATE], [DISPORDER])
    VALUES 
    ('Raw Materials', 'RAW', 'SYSTEM', 'SYSTEM', 0, GETDATE(), 1),
    ('Finished Goods', 'FG', 'SYSTEM', 'SYSTEM', 0, GETDATE(), 2),
    ('Work In Progress', 'WIP', 'SYSTEM', 'SYSTEM', 0, GETDATE(), 3),
    ('Consumables', 'CONS', 'SYSTEM', 'SYSTEM', 0, GETDATE(), 4),
    ('Spare Parts', 'SPARE', 'SYSTEM', 'SYSTEM', 0, GETDATE(), 5)
    
    PRINT 'Sample data inserted successfully.'
END
GO
