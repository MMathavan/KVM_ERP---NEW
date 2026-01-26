using System;
using System.Data.Entity.Migrations;

namespace ClubMembership.Migrations
{
    public partial class AddCategoryTypeToAnnouncementMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ANNOUNCEMENTMASTER", "CategoryTypeIds", c => c.String(maxLength: 500));
            
            // Add foreign key constraint if needed
            // CreateIndex("dbo.ANNOUNCEMENTMASTER", "CategoryTypeIds");
            // AddForeignKey("dbo.ANNOUNCEMENTMASTER", "CategoryTypeIds", "dbo.CategoryTypeMaster", "CateTid");
        }

        public override void Down()
        {
            // RemoveForeignKey("dbo.ANNOUNCEMENTMASTER", "CategoryTypeIds", "dbo.CategoryTypeMaster");
            // DropIndex("dbo.ANNOUNCEMENTMASTER", new[] { "CategoryTypeIds" });
            DropColumn("dbo.ANNOUNCEMENTMASTER", "CategoryTypeIds");
        }
    }
}
