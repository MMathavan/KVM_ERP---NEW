-- Add CateTDesc column to MemberShipMaster table
-- This script adds the CateTDesc column to store category descriptions

-- Check if column already exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'MemberShipMaster' 
               AND COLUMN_NAME = 'CateTDesc')
BEGIN
    -- Add the CateTDesc column (stores category description from CategoryTypeMaster)
    ALTER TABLE [dbo].[MemberShipMaster] 
    ADD [CateTDesc] NVARCHAR(200) NULL;
    
    PRINT 'CateTDesc column added successfully to MemberShipMaster table';
END
ELSE
BEGIN
    PRINT 'CateTDesc column already exists in MemberShipMaster table';
END

-- Verify the column was added
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'MemberShipMaster' 
AND COLUMN_NAME = 'CateTDesc';
