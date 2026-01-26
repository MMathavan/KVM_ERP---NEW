using System;
using System.Data.Entity.Migrations;

namespace ClubMembership.Migrations
{
    public partial class AddContentManagementTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ANNOUNCEMENTMASTER",
                c => new
                {
                    AnnouncementId = c.Int(nullable: false, identity: true),
                    Heading = c.String(maxLength: 200),
                    Caption = c.String(maxLength: 500),
                    Description = c.String(maxLength: 2000),
                    MainImage = c.String(),
                    AdditionalImages = c.String(),
                    Status = c.Short(nullable: false),
                    CreatedBy = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedBy = c.String(),
                    ModifiedDate = c.DateTime(),
                    CompanyId = c.Int(nullable: false)
                })
                .PrimaryKey(t => t.AnnouncementId);

            CreateTable(
                "dbo.EVENTMASTER",
                c => new
                {
                    EventId = c.Int(nullable: false, identity: true),
                    Heading = c.String(maxLength: 200),
                    Caption = c.String(maxLength: 500),
                    Description = c.String(maxLength: 2000),
                    MainImage = c.String(),
                    AdditionalImages = c.String(),
                    EventTime = c.DateTime(nullable: false),
                    EventPlan = c.String(maxLength: 1000),
                    EventLocation = c.String(maxLength: 200),
                    Status = c.Short(nullable: false),
                    CreatedBy = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedBy = c.String(),
                    ModifiedDate = c.DateTime(),
                    CompanyId = c.Int(nullable: false)
                })
                .PrimaryKey(t => t.EventId);

            CreateTable(
                "dbo.GALLERYMASTER",
                c => new
                {
                    GalleryId = c.Int(nullable: false, identity: true),
                    Heading = c.String(maxLength: 200),
                    Caption = c.String(maxLength: 500),
                    Description = c.String(maxLength: 2000),
                    MainImage = c.String(),
                    AdditionalImages = c.String(),
                    Category = c.String(maxLength: 100),
                    Status = c.Short(nullable: false),
                    CreatedBy = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                    ModifiedBy = c.String(),
                    ModifiedDate = c.DateTime(),
                    CompanyId = c.Int(nullable: false)
                })
                .PrimaryKey(t => t.GalleryId);
        }

        public override void Down()
        {
            DropTable("dbo.GALLERYMASTER");
            DropTable("dbo.EVENTMASTER");
            DropTable("dbo.ANNOUNCEMENTMASTER");
        }
    }
}

