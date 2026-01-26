using System;
using System.Data.Entity.Migrations;

namespace ClubMembership.Migrations
{
    public partial class AddCateTidToAspNetUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CateTid", c => c.Int(nullable: true));
            
            // Add foreign key constraint to CategoryTypeMaster table
            CreateIndex("dbo.AspNetUsers", "CateTid");
            AddForeignKey("dbo.AspNetUsers", "CateTid", "dbo.CategoryTypeMaster", "CateTid");
        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "CateTid", "dbo.CategoryTypeMaster");
            DropIndex("dbo.AspNetUsers", new[] { "CateTid" });
            DropColumn("dbo.AspNetUsers", "CateTid");
        }
    }
}
