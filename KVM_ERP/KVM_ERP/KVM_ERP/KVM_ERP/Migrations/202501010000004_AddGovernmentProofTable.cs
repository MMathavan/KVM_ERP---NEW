using System;
using System.Data.Entity.Migrations;

namespace ClubMembership.Migrations
{
    public partial class AddGovernmentProofTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.govrmnet_proof",
                c => new
                {
                    gp_id = c.Int(nullable: false, identity: true),
                    MemberID = c.Int(nullable: false),
                    gov_path = c.String(nullable: false, maxLength: 500)
                })
                .PrimaryKey(t => t.gp_id)
                .ForeignKey("dbo.MemberShipMaster", t => t.MemberID, cascadeDelete: true)
                .Index(t => t.MemberID);
        }

        public override void Down()
        {
            DropForeignKey("dbo.govrmnet_proof", "MemberID", "dbo.MemberShipMaster");
            DropIndex("dbo.govrmnet_proof", new[] { "MemberID" });
            DropTable("dbo.govrmnet_proof");
        }
    }
}
