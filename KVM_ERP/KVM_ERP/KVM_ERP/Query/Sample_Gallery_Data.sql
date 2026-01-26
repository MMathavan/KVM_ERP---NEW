-- Sample Gallery Data for Testing Image Preview Functionality
-- Run this script in your database to add sample gallery items

-- First, ensure the GALLERYMASTER table exists
IF OBJECT_ID(N'[dbo].[GALLERYMASTER]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[GALLERYMASTER](
        [GalleryId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Heading] NVARCHAR(200) NOT NULL,
        [Caption] NVARCHAR(500) NULL,
        [Description] NVARCHAR(2000) NULL,
        [MainImage] NVARCHAR(500) NULL,
        [AdditionalImages] NVARCHAR(MAX) NULL,
        [Category] NVARCHAR(100) NULL,
        [Status] SMALLINT NOT NULL DEFAULT 0,
        [CreatedBy] NVARCHAR(128) NULL,
        [CreatedDate] DATETIME NOT NULL,
        [ModifiedBy] NVARCHAR(128) NULL,
        [ModifiedDate] DATETIME NULL,
        [CompanyId] INT NOT NULL DEFAULT 1
    );
END

-- Insert sample gallery items
-- Note: Update the image paths to match your actual uploaded images

-- Sample 1: Club Event
INSERT INTO [dbo].[GALLERYMASTER] 
([Heading], [Caption], [Description], [MainImage], [AdditionalImages], [Category], [Status], [CreatedBy], [CreatedDate], [CompanyId])
VALUES 
('Annual Club Meeting 2024', 
 'Members gathering for the yearly club meeting', 
 'A wonderful gathering of club members discussing future plans and celebrating achievements', 
 '~/Uploads/Gallery/503e7d5d-393c-472b-a4f6-242a88c09bd9.png', 
 '~/Uploads/Gallery/a62607a1-d3b2-4da0-b24f-8c7897b1718f.png,~/Uploads/Gallery/99bad6b7-a5f0-4e82-b61f-5cb051143be5.avif', 
 'Events', 0, 'System', GETDATE(), 1);

-- Sample 2: Member Activity
INSERT INTO [dbo].[GALLERYMASTER] 
([Heading], [Caption], [Description], [MainImage], [AdditionalImages], [Category], [Status], [CreatedBy], [CreatedDate], [CompanyId])
VALUES 
('Fitness Training Session', 
 'Members participating in group fitness activities', 
 'Weekly fitness training session with professional trainers', 
 '~/Uploads/Gallery/8c80b13f-c00d-4846-b9bd-1eba21c10d9f.png', 
 '~/Uploads/Gallery/11178dcd-169b-4d26-bbfa-a9e3dc4e5aa9.png', 
 'Activities', 0, 'System', GETDATE(), 1);

-- Sample 3: Club Facilities
INSERT INTO [dbo].[GALLERYMASTER] 
([Heading], [Caption], [Description], [MainImage], [AdditionalImages], [Category], [Status], [CreatedBy], [CreatedDate], [CompanyId])
VALUES 
('Club Swimming Pool', 
 'Our state-of-the-art swimming facility', 
 'Modern swimming pool with temperature control and safety features', 
 '~/Uploads/Gallery/081e8c64-43d8-4d7a-aad4-30bd179c88c5.png', 
 NULL, 
 'Facilities', 0, 'System', GETDATE(), 1);

-- Sample 4: Member Photos
INSERT INTO [dbo].[GALLERYMASTER] 
([Heading], [Caption], [Description], [MainImage], [AdditionalImages], [Category], [Status], [CreatedBy], [CreatedDate], [CompanyId])
VALUES 
('Member Spotlight - John Doe', 
 'Featured member of the month', 
 'John Doe has been an active member for 5 years and contributes significantly to our community', 
 '~/Uploads/Gallery/dbfab960-ceab-48e0-ad0a-54640f4d231c.avif', 
 NULL, 
 'Members', 0, 'System', GETDATE(), 1);

-- Sample 5: Special Event
INSERT INTO [dbo].[GALLERYMASTER] 
([Heading], [Caption], [Description], [MainImage], [AdditionalImages], [Category], [Status], [CreatedBy], [CreatedDate], [CompanyId])
VALUES 
('Holiday Celebration 2024', 
 'Festive decorations and member celebrations', 
 'Annual holiday party with decorations, food, and entertainment for all members', 
 '~/Uploads/Gallery/503e7d5d-393c-472b-a4f6-242a88c09bd9.png', 
 '~/Uploads/Gallery/a62607a1-d3b2-4da0-b24f-8c7897b1718f.png,~/Uploads/Gallery/8c80b13f-c00d-4846-b9bd-1eba21c10d9f.png,~/Uploads/Gallery/11178dcd-169b-4d26-bbfa-a9e3dc4e5aa9.png', 
 'Events', 0, 'System', GETDATE(), 1);

PRINT 'Sample gallery data inserted successfully!';
PRINT 'You can now view the gallery at: /Content/Gallery';
PRINT 'Make sure you are logged in to access the gallery.';
