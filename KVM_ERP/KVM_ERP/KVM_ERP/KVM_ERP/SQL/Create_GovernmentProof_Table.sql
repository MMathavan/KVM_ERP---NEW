-- Create govrmnet_proof table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='govrmnet_proof' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[govrmnet_proof](
        [gp_id] [int] IDENTITY(1,1) NOT NULL,
        [MemberID] [int] NOT NULL,
        [gov_path] [nvarchar](500) NOT NULL,
        CONSTRAINT [PK_govrmnet_proof] PRIMARY KEY CLUSTERED ([gp_id] ASC)
    )
    
    -- Add foreign key constraint
    ALTER TABLE [dbo].[govrmnet_proof] WITH CHECK ADD CONSTRAINT [FK_govrmnet_proof_MemberShipMaster] 
    FOREIGN KEY([MemberID]) REFERENCES [dbo].[MemberShipMaster] ([MemberID])
    
    -- Add index for better performance
    CREATE NONCLUSTERED INDEX [IX_govrmnet_proof_MemberID] ON [dbo].[govrmnet_proof] ([MemberID] ASC)
    
    PRINT 'govrmnet_proof table created successfully'
END
ELSE
BEGIN
    PRINT 'govrmnet_proof table already exists'
END
