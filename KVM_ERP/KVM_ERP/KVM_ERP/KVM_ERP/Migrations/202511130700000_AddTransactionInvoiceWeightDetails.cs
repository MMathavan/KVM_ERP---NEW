namespace KVM_ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransactionInvoiceWeightDetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TRANSACTION_INVOICE_WEIGHT_DETAILS",
                c => new
                    {
                        TRANRID = c.Int(nullable: false, identity: true),
                        TRANMID = c.Int(nullable: false),
                        PACKMID = c.Int(nullable: false),
                        PACKTMID = c.Int(nullable: false),
                        SLABVALUE = c.Decimal(nullable: false, precision: 18, scale: 3),
                        PNDSVALUE = c.Decimal(nullable: false, precision: 18, scale: 3),
                        TOTALPNDS = c.Decimal(nullable: false, precision: 18, scale: 3),
                        PACKWGT = c.Decimal(nullable: false, precision: 18, scale: 3),
                        TOTALWGHT = c.Decimal(nullable: false, precision: 18, scale: 3),
                        ONEDOLLAR = c.Decimal(nullable: false, precision: 18, scale: 3),
                        TOTALDOLVAL = c.Decimal(nullable: false, precision: 18, scale: 3),
                        WEIGHTINKGS = c.Decimal(nullable: false, precision: 18, scale: 3),
                        PERKGRATE = c.Decimal(nullable: false, precision: 18, scale: 3),
                        CUSRID = c.String(nullable: false, maxLength: 100),
                        LMUSRID = c.String(nullable: false, maxLength: 100),
                        DISPSTATUS = c.Short(nullable: false),
                        PRCSDATE = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TRANRID)
                .ForeignKey("dbo.TRANSACTIONMASTER", t => t.TRANMID)
                .ForeignKey("dbo.PACKINGMASTER", t => t.PACKMID)
                .ForeignKey("dbo.PACKINGTYPEMASTER", t => t.PACKTMID)
                .Index(t => t.TRANMID)
                .Index(t => t.PACKMID)
                .Index(t => t.PACKTMID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TRANSACTION_INVOICE_WEIGHT_DETAILS", "PACKTMID", "dbo.PACKINGTYPEMASTER");
            DropForeignKey("dbo.TRANSACTION_INVOICE_WEIGHT_DETAILS", "PACKMID", "dbo.PACKINGMASTER");
            DropForeignKey("dbo.TRANSACTION_INVOICE_WEIGHT_DETAILS", "TRANMID", "dbo.TRANSACTIONMASTER");
            DropIndex("dbo.TRANSACTION_INVOICE_WEIGHT_DETAILS", new[] { "PACKTMID" });
            DropIndex("dbo.TRANSACTION_INVOICE_WEIGHT_DETAILS", new[] { "PACKMID" });
            DropIndex("dbo.TRANSACTION_INVOICE_WEIGHT_DETAILS", new[] { "TRANMID" });
            DropTable("dbo.TRANSACTION_INVOICE_WEIGHT_DETAILS");
        }
    }
}
