namespace ClubMembership.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ACCOUNTGROUPMASTER",
                c => new
                    {
                        ACHEADGID = c.Int(nullable: false, identity: true),
                        ACHEADGDESC = c.String(nullable: false),
                        ACHEADGCODE = c.String(nullable: false),
                        CUSRID = c.String(),
                        LMUSRID = c.Int(nullable: false),
                        DISPSTATUS = c.Short(nullable: false),
                        PRCSDATE = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ACHEADGID);
            
            CreateTable(
                "dbo.ACCOUNTHEADMASTER",
                c => new
                    {
                        ACHEADID = c.Int(nullable: false, identity: true),
                        ACHEADGID = c.Int(nullable: false),
                        ACHEADDESC = c.String(nullable: false),
                        ACHEADCODE = c.String(nullable: false),
                        CUSRID = c.String(),
                        LMUSRID = c.Int(nullable: false),
                        DISPSTATUS = c.Short(nullable: false),
                        PRCSDATE = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ACHEADID);
            
            CreateTable(
                "dbo.AspNetUsers1",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Discriminator = c.String(),
                        NPassword = c.String(),
                        BrnchId = c.Int(nullable: false),
                        UBrnchName = c.String(),
                        DBrnchId = c.Int(nullable: false),
                        DeptName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.COMPANYDETAIL",
                c => new
                    {
                        COMPDID = c.Int(nullable: false, identity: true),
                        COMPID = c.Int(nullable: false),
                        COMPPONAME = c.String(),
                        COMPPOADDR = c.String(),
                    })
                .PrimaryKey(t => t.COMPDID);
            
            CreateTable(
                "dbo.COMPANYMASTER",
                c => new
                    {
                        COMPID = c.Int(nullable: false, identity: true),
                        COMPNAME = c.String(),
                        COMPDNAME = c.String(),
                        COMPADDR = c.String(),
                        COMPPHNID = c.String(),
                        COMPPHN1 = c.String(),
                        COMPPHN2 = c.String(),
                        COMPPHN3 = c.String(),
                        COMPPHN4 = c.String(),
                        COMPCPRSN = c.String(),
                        COMPMAIL = c.String(),
                        COMPCODE = c.String(),
                        CUSRID = c.String(),
                        LMUSRID = c.Int(nullable: false),
                        DISPSTATUS = c.Short(nullable: false),
                        PRCSDATE = c.DateTime(nullable: false),
                        STATEID = c.Int(nullable: false),
                        COMPGSTNO = c.String(),
                        COMPPANNO = c.String(),
                        COMPTANNO = c.String(),
                    })
                .PrimaryKey(t => t.COMPID);
            
            CreateTable(
                "dbo.DISPLAYORDERMASTER",
                c => new
                    {
                        DORDRID = c.Int(nullable: false, identity: true),
                        DORDRDESC = c.String(nullable: false),
                        DORDORDR = c.Short(nullable: false),
                        DISPSTATUS = c.Short(nullable: false),
                        PRCSDATE = c.DateTime(),
                    })
                .PrimaryKey(t => t.DORDRID);
            
            CreateTable(
                "dbo.EmployeeLinkMaster",
                c => new
                    {
                        CATELID = c.Int(nullable: false, identity: true),
                        CATEID = c.Int(nullable: false),
                        BRNCHID = c.Int(nullable: false),
                        LBRNCHNAME = c.String(),
                        LCATEID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CATELID);
            
            CreateTable(
                "dbo.EMPLOYEEMASTER",
                c => new
                    {
                        CATEID = c.Int(nullable: false, identity: true),
                        CATENAME = c.String(nullable: false),
                        CATECODE = c.String(),
                        CATEADDR1 = c.String(),
                        CATEADDR2 = c.String(),
                        CATEADDR3 = c.String(),
                        CATEADDR4 = c.String(),
                        CATEPHN1 = c.String(),
                        CATEPHN2 = c.String(),
                        CATEPHN3 = c.String(),
                        CATEPHN4 = c.String(),
                        CATEPNAME = c.String(),
                        CATEMAIL = c.String(),
                        CATEDOJ = c.DateTime(),
                        CATENO = c.Int(nullable: false),
                        CATEGTYPE = c.Short(nullable: false),
                        LOCTID = c.Int(nullable: false),
                        CATEAUTRNO = c.String(),
                        CATEDRVLSNO = c.String(),
                        DEPTID = c.Int(nullable: false),
                        DSGNID = c.Int(nullable: false),
                        STATEID = c.Int(nullable: false),
                        CATE_DSGNDESC = c.String(),
                        CUSRID = c.String(),
                        LMUSRID = c.Int(nullable: false),
                        DISPSTATUS = c.Short(nullable: false),
                        PRCSDATE = c.DateTime(nullable: false),
                        BRNCHID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CATEID);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApplicationRoleGroups",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.GroupId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        SDPTID = c.Int(),
                        MenuName = c.String(),
                        ControllerName = c.String(),
                        MenuIndex = c.String(),
                        MenuOrder = c.String(),
                        Order = c.String(),
                        RImage = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MENUMASTER",
                c => new
                    {
                        MENUID = c.Int(nullable: false, identity: true),
                        MENUDESC = c.String(nullable: false, maxLength: 100),
                        MENUCODE = c.String(nullable: false, maxLength: 15),
                        CUSRID = c.String(nullable: false, maxLength: 100),
                        LMUSRID = c.String(nullable: false, maxLength: 100),
                        DISPSTATUS = c.Short(nullable: false),
                        PRCSDATE = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MENUID);
            
            CreateTable(
                "dbo.MenuRoleMaster",
                c => new
                    {
                        MenuId = c.Int(nullable: false, identity: true),
                        LinkText = c.String(),
                        ActionName = c.String(),
                        ControllerName = c.String(),
                        Roles = c.String(),
                        MenuGId = c.Short(nullable: false),
                        MenuGIndex = c.Short(nullable: false),
                        ImageClassName = c.String(),
                    })
                .PrimaryKey(t => t.MenuId);
            
            CreateTable(
                "dbo.RIMAGEMASTER",
                c => new
                    {
                        RIMGID = c.Int(nullable: false, identity: true),
                        RIMGDESC = c.String(nullable: false, maxLength: 100),
                        RIMGCODE = c.String(nullable: false, maxLength: 15),
                        CUSRID = c.String(nullable: false, maxLength: 100),
                        LMUSRID = c.String(nullable: false, maxLength: 100),
                        DISPSTATUS = c.Short(nullable: false),
                        PRCSDATE = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RIMGID);
            
            CreateTable(
                "dbo.SOFT_DEPARTMENTMASTER",
                c => new
                    {
                        SDPTID = c.Int(nullable: false, identity: true),
                        SDPTNAME = c.String(nullable: false),
                        OPTNSTR = c.String(nullable: false),
                        DISPSTATUS = c.Short(nullable: false),
                        PRCSDATE = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SDPTID);
            
            CreateTable(
                "dbo.TMPRPT_IDS",
                c => new
                    {
                        KUSRID = c.String(nullable: false, maxLength: 128),
                        OPTNSTR = c.String(),
                        RPTID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.KUSRID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        BrnchId = c.Int(),
                        UBrnchName = c.String(),
                        DBrnchId = c.Int(),
                        DeptName = c.String(),
                        NPassword = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.ApplicationUserGroups",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.GroupId })
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.VW_ACCOUNTING_YEAR_DETAIL_ASSGN",
                c => new
                    {
                        COMPYID = c.Int(nullable: false, identity: true),
                        FDATE = c.DateTime(nullable: false),
                        TDATE = c.DateTime(nullable: false),
                        YRDESC = c.String(),
                    })
                .PrimaryKey(t => t.COMPYID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUserGroups", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserGroups", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationRoleGroups", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.ApplicationRoleGroups", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.ApplicationUserGroups", new[] { "GroupId" });
            DropIndex("dbo.ApplicationUserGroups", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.ApplicationRoleGroups", new[] { "GroupId" });
            DropIndex("dbo.ApplicationRoleGroups", new[] { "RoleId" });
            DropTable("dbo.VW_ACCOUNTING_YEAR_DETAIL_ASSGN");
            DropTable("dbo.ApplicationUserGroups");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.TMPRPT_IDS");
            DropTable("dbo.SOFT_DEPARTMENTMASTER");
            DropTable("dbo.RIMAGEMASTER");
            DropTable("dbo.MenuRoleMaster");
            DropTable("dbo.MENUMASTER");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ApplicationRoleGroups");
            DropTable("dbo.Groups");
            DropTable("dbo.EMPLOYEEMASTER");
            DropTable("dbo.EmployeeLinkMaster");
            DropTable("dbo.DISPLAYORDERMASTER");
            DropTable("dbo.COMPANYMASTER");
            DropTable("dbo.COMPANYDETAIL");
            DropTable("dbo.AspNetUsers1");
            DropTable("dbo.ACCOUNTHEADMASTER");
            DropTable("dbo.ACCOUNTGROUPMASTER");
        }
    }
}
