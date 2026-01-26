using System;
using System.Data.Entity.Migrations;

namespace ClubMembership.Migrations
{
    public partial class AddCateDescToMemberShipMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MemberShipMaster", "CateTDesc", c => c.String(maxLength: 200));
        }

        public override void Down()
        {
            DropColumn("dbo.MemberShipMaster", "CateTDesc");
        }
    }
}
