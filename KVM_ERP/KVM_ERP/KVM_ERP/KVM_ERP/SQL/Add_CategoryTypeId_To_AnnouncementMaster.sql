-- Add CategoryTypeIds column to ANNOUNCEMENTMASTER table
-- This script adds the CategoryTypeIds column to link announcements with multiple category types

-- Check if column already exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'ANNOUNCEMENTMASTER' 
               AND COLUMN_NAME = 'CategoryTypeIds')
BEGIN
    -- Add the CategoryTypeIds column (comma-separated list of category IDs)
    ALTER TABLE [dbo].[ANNOUNCEMENTMASTER] 
    ADD [CategoryTypeIds] NVARCHAR(500) NULL;
    
    PRINT 'CategoryTypeIds column added successfully to ANNOUNCEMENTMASTER table';
    
    -- Note: Foreign key constraints are not suitable for comma-separated values
    -- The application handles the validation and relationship logic
END
ELSE
BEGIN
    PRINT 'CategoryTypeIds column already exists in ANNOUNCEMENTMASTER table';
END

-- Verify the column was added
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'ANNOUNCEMENTMASTER' 
AND COLUMN_NAME = 'CategoryTypeIds';
