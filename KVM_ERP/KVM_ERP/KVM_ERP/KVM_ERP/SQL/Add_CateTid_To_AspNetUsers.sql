-- Add CateTid column to AspNetUsers table
-- This script adds the CateTid column to link users with category types

-- Check if column already exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'AspNetUsers' 
               AND COLUMN_NAME = 'CateTid')
BEGIN
    -- Add the CateTid column (foreign key to CategoryTypeMaster)
    ALTER TABLE [dbo].[AspNetUsers] 
    ADD [CateTid] INT NULL;
    
    PRINT 'CateTid column added successfully to AspNetUsers table';
    
    -- Add foreign key constraint to CategoryTypeMaster table
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CategoryTypeMaster')
    BEGIN
        ALTER TABLE [dbo].[AspNetUsers]
        ADD CONSTRAINT FK_AspNetUsers_CategoryTypeMaster 
        FOREIGN KEY (CateTid) REFERENCES CategoryTypeMaster(CateTid);
        
        PRINT 'Foreign key constraint added successfully';
    END
    ELSE
    BEGIN
        PRINT 'CategoryTypeMaster table not found - foreign key constraint not added';
    END
END
ELSE
BEGIN
    PRINT 'CateTid column already exists in AspNetUsers table';
END

-- Verify the column was added
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'AspNetUsers' 
AND COLUMN_NAME = 'CateTid';
