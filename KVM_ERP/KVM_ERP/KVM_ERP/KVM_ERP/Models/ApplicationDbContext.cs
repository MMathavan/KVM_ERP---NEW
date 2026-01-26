using ClubMembership.Data;
using ClubMembership.Models;
using KVM_ERP.Models;
// Add this using directive if AnnouncementMaster is in another namespace
// using ClubMembership.Entities; // <-- Uncomment and adjust if needed
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace KVM_ERP.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<MenuMaster> MenuMasters { get; set; }
        public DbSet<RImageMaster> RImageMasters { get; set; }
        public DbSet<StateMaster> StateMasters { get; set; }
        public DbSet<BloodGroupMaster> BloodGroupMasters { get; set; }
        public DbSet<RegionMaster> RegionMasters { get; set; }
        public DbSet<DesignationMaster> DesignationMasters { get; set; }
        public DbSet<DepartmentMaster> DepartmentMasters { get; set; }
        public DbSet<LocationMaster> LocationMasters { get; set; }
        public DbSet<CustomerMaster> CustomerMasters { get; set; }
        public DbSet<SupplierMaster> SupplierMasters { get; set; }
        public DbSet<CurrencyMaster> CurrencyMasters { get; set; }
        public DbSet<MaterialTypeMaster> MaterialTypeMasters { get; set; }
        public DbSet<MaterialGroupMaster> MaterialGroupMasters { get; set; }
        public DbSet<UnitMaster> UnitMasters { get; set; }
        public DbSet<HSNCodeMaster> HSNCodeMasters { get; set; }
        public DbSet<CostFactorMaster> CostFactorMasters { get; set; }
        public DbSet<PackingMaster> PackingMasters { get; set; }
        public DbSet<PackingTypeMaster> PackingTypeMasters { get; set; }
        public DbSet<MaterialMaster> MaterialMasters { get; set; }
        public DbSet<TransactionMaster> TransactionMasters { get; set; }
        public DbSet<TransactionDetail> TransactionDetails { get; set; }
        public DbSet<TransactionMasterFactor> TransactionMasterFactors { get; set; }
        public DbSet<TransactionProductCalculation> TransactionProductCalculations { get; set; }
        public DbSet<TransactionQualityCheck> TransactionQualityChecks { get; set; }
        public DbSet<LaboratoryMaster> LaboratoryMasters { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<GradeMaster> GradeMasters { get; set; }
        public DbSet<ProductionColourMaster> ProductionColourMasters { get; set; }
        public DbSet<ReceivedTypeMaster> ReceivedTypeMasters { get; set; }
        public DbSet<PurchaseInvoiceStatus> PurchaseInvoiceStatuses { get; set; }
        public DbSet<TransactionInvoiceWeightDetails> TransactionInvoiceWeightDetails { get; set; }

        new public virtual IDbSet<ApplicationRole> Roles { get; set; }
        public virtual IDbSet<Group> Groups { get; set; }
        public DbSet<CompanyMaster> companymasters { get; set; }
        public DbSet<CompanyDetail> companydetails { get; set; }
        public DbSet<TMPRPT_IDS> TMPRPT_IDS { get; set; }
        public virtual IDbSet<VW_ACCOUNTING_YEAR_DETAIL_ASSGN> VW_ACCOUNTING_YEAR_DETAIL_ASSGN { get; set; }
        public virtual IDbSet<MenuRoleMaster> MenuRoleMasters { get; set; }
        public DbSet<SoftDepartmentMaster> softdepartmentmasters { get; set; }
        public virtual IDbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual IDbSet<AccountGroupMaster> accountgroupmasters { get; set; }
        public virtual IDbSet<AccountHeadMaster> accountheadmasters { get; set; }
        public virtual IDbSet<DisplayOrderMaster> displayordermasters { get; set; }
        public DbSet<EmployeeMaster> EmployeeMasters { get; set; }
        public DbSet<EmployeeLinkMaster> employeelinkmasters { get; set; }
        // GovernmentProof module removed
        // public DbSet<GovernmentProof> GovernmentProofs { get; set; }
        public ApplicationDbContext()
         : base("Club_DefaultConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            //modelBuilder.Entity<TransactionMaster>().Property(d => d.TRANCRATE).HasPrecision(18, 4);
            
            // Configure TransactionMaster decimal precision for TRANNAMT and GST fields
            modelBuilder.Entity<TransactionMaster>().Property(d => d.TRANNAMT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionMaster>().Property(d => d.TRANCGSTAMT).HasPrecision(18, 2);
            modelBuilder.Entity<TransactionMaster>().Property(d => d.TRANSGSTAMT).HasPrecision(18, 2);
            modelBuilder.Entity<TransactionMaster>().Property(d => d.TRANIGSTAMT).HasPrecision(18, 2);
            modelBuilder.Entity<TransactionMaster>().Property(d => d.TRANCGSTEXPRN).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionMaster>().Property(d => d.TRANSGSTEXPRN).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionMaster>().Property(d => d.TRANIGSTEXPRN).HasPrecision(18, 3);
            
            // Configure CurrencyMaster decimal precision
            modelBuilder.Entity<CurrencyMaster>().Property(d => d.CURNAMT).HasPrecision(18, 2);
            
            // Configure HSNCodeMaster decimal precision for all GST fields
            modelBuilder.Entity<HSNCodeMaster>().Property(h => h.CGSTEXPRN).HasPrecision(18, 2);
            modelBuilder.Entity<HSNCodeMaster>().Property(h => h.SGSTEXPRN).HasPrecision(18, 2);
            modelBuilder.Entity<HSNCodeMaster>().Property(h => h.IGSTEXPRN).HasPrecision(18, 2);
            modelBuilder.Entity<HSNCodeMaster>().Property(h => h.ACGSTEXPRN).HasPrecision(18, 2);
            modelBuilder.Entity<HSNCodeMaster>().Property(h => h.ASGSTEXPRN).HasPrecision(18, 2);
            modelBuilder.Entity<HSNCodeMaster>().Property(h => h.AIGSTEXPRN).HasPrecision(18, 2);
            modelBuilder.Entity<HSNCodeMaster>().Property(h => h.TAXEXPRN).HasPrecision(18, 2);
            
            // Configure CostFactorMaster decimal precision for CFEXPR field
            modelBuilder.Entity<CostFactorMaster>().Property(c => c.CFEXPR).HasPrecision(18, 3);
            
            // Configure MaterialMaster decimal precision for all decimal fields
            modelBuilder.Entity<MaterialMaster>().Property(m => m.ROLNQTY).HasPrecision(18, 3);
            modelBuilder.Entity<MaterialMaster>().Property(m => m.ROLXQTY).HasPrecision(18, 3);
            modelBuilder.Entity<MaterialMaster>().Property(m => m.EOQTY).HasPrecision(18, 3);
            modelBuilder.Entity<MaterialMaster>().Property(m => m.MTRLPRFT).HasPrecision(18, 2);
            modelBuilder.Entity<MaterialMaster>().Property(m => m.MTRLBQTY).HasPrecision(18, 2);
            modelBuilder.Entity<MaterialMaster>().Property(m => m.MTRLBRATE).HasPrecision(18, 2);
            
            // Configure TransactionProductCalculation decimal precision for all decimal fields
            // PCK fields (PCK1-PCK17)
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK1).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK2).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK3).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK4).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK5).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK6).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK7).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK8).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK9).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK10).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK11).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK12).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK13).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK14).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK15).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK16).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCK17).HasPrecision(18, 3);
            
            // Calculated fields
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.TOPCK).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCKLVALUE).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.AVGPCKVALUE).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PNDSVALUE).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.TOTALPNDS).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.YELDPERCENT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.TOTALYELDCOUNTS).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.KGWGT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.PCKKGWGT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.WASTEWGT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.WASTEPWGT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.FACTORYWGT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.FACAVGWGT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.FACAVGCOUNT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.BKN).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionProductCalculation>().Property(t => t.OTHERS).HasPrecision(18, 3);

            // Configure TransactionDetail decimal precision for all decimal fields
            modelBuilder.Entity<TransactionDetail>().Property(t => t.TRANAQTY).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionDetail>().Property(t => t.TRANDQTY).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionDetail>().Property(t => t.TRANEQTY).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionDetail>().Property(t => t.TRANDRATE).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionDetail>().Property(t => t.TRANDAMT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionDetail>().Property(t => t.TRANDDISCEXPRN).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionDetail>().Property(t => t.TRANDDISCAMT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionDetail>().Property(t => t.TRANDGAMT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionDetail>().Property(t => t.TRANDCGSTEXPRN).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionDetail>().Property(t => t.TRANDSGSTEXPRN).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionDetail>().Property(t => t.TRANDIGSTEXPRN).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionDetail>().Property(t => t.TRANDCGSTAMT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionDetail>().Property(t => t.TRANDSGSTAMT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionDetail>().Property(t => t.TRANDIGSTAMT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionDetail>().Property(t => t.TRANDNAMT).HasPrecision(18, 3);
            // TRANDAID is INT type - no precision configuration needed

            // Configure TransactionMasterFactor decimal precision for all decimal fields
            modelBuilder.Entity<TransactionMasterFactor>().Property(t => t.DEDEXPRN).HasPrecision(18, 2);
            modelBuilder.Entity<TransactionMasterFactor>().Property(t => t.DEDVALUE).HasPrecision(18, 2);
            modelBuilder.Entity<TransactionMasterFactor>().Property(t => t.TRANCFCGSTEXPRN).HasPrecision(18, 2);
            modelBuilder.Entity<TransactionMasterFactor>().Property(t => t.TRANCFSGSTEXPRN).HasPrecision(18, 2);
            modelBuilder.Entity<TransactionMasterFactor>().Property(t => t.TRANCFIGSTEXPRN).HasPrecision(18, 2);
            modelBuilder.Entity<TransactionMasterFactor>().Property(t => t.TRANCFCGSTAMT).HasPrecision(18, 2);
            modelBuilder.Entity<TransactionMasterFactor>().Property(t => t.TRANCFSGSTAMT).HasPrecision(18, 2);
            modelBuilder.Entity<TransactionMasterFactor>().Property(t => t.TRANCFIGSTAMT).HasPrecision(18, 2);
            
            // Configure TransactionInvoiceWeightDetails decimal precision for all decimal fields
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.SLABVALUE).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.PNDSVALUE).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.TOTALPNDS).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.PACKWGT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.TOTALWGHT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.ONEDOLLAR).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.TOTALDOLVAL).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.TRANIDISCEXPRN).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.WASTEPWGT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.TRANIDISCAMT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.TOTALDOLDISCAMT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.WEIGHTINKGS).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.PERKGRATE).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.INCENTIVEPERCENT).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.INCENTIVEVALUE).HasPrecision(18, 3);
            modelBuilder.Entity<TransactionInvoiceWeightDetails>().Property(t => t.INCENTIVETOTALVALUE).HasPrecision(18, 3);

            // Keep this:
            modelBuilder.Entity<IdentityUser>().ToTable("AspNetUsers");

            // Change TUser to ApplicationUser everywhere else - IdentityUser and ApplicationUser essentially 'share' the AspNetUsers Table in the database:
            EntityTypeConfiguration<ApplicationUser> table =
                modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");

            table.Property((ApplicationUser u) => u.UserName).IsRequired();
            
            // Map the CateTid property to the database column
            table.Property((ApplicationUser u) => u.CateTid).HasColumnName("CateTid");

            // EF won't let us swap out IdentityUserRole for ApplicationUserRole here:
            modelBuilder.Entity<ApplicationUser>().HasMany<IdentityUserRole>((ApplicationUser u) => u.Roles);
            modelBuilder.Entity<IdentityUserRole>().HasKey((IdentityUserRole r) =>
                new { UserId = r.UserId, RoleId = r.RoleId }).ToTable("AspNetUserRoles");


            // Add the group stuff here:
            modelBuilder.Entity<ApplicationUser>().HasMany<ApplicationUserGroup>((ApplicationUser u) => u.Groups);
            modelBuilder.Entity<ApplicationUserGroup>().HasKey((ApplicationUserGroup r) => new { UserId = r.UserId, GroupId = r.GroupId }).ToTable("ApplicationUserGroups");

            // And here:
            modelBuilder.Entity<Group>().HasMany<ApplicationRoleGroup>((Group g) => g.Roles);
            modelBuilder.Entity<ApplicationRoleGroup>().HasKey((ApplicationRoleGroup gr) => new { RoleId = gr.RoleId, GroupId = gr.GroupId }).ToTable("ApplicationRoleGroups");

            // And Here:
            EntityTypeConfiguration<Group> groupsConfig = modelBuilder.Entity<Group>().ToTable("Groups");
            groupsConfig.Property((Group r) => r.Name).IsRequired();

            // Leave this alone:
            EntityTypeConfiguration<IdentityUserLogin> entityTypeConfiguration =
                modelBuilder.Entity<IdentityUserLogin>().HasKey((IdentityUserLogin l) =>
                    new { UserId = l.UserId, LoginProvider = l.LoginProvider, ProviderKey = l.ProviderKey }).ToTable("AspNetUserLogins");

            entityTypeConfiguration.HasRequired<IdentityUser>((IdentityUserLogin u) => u.User);
            EntityTypeConfiguration<IdentityUserClaim> table1 = modelBuilder.Entity<IdentityUserClaim>().ToTable("AspNetUserClaims");
            table1.HasRequired<IdentityUser>((IdentityUserClaim u) => u.User);

            // Add this, so that IdentityRole can share a table with ApplicationRole:
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");

            // Change these from IdentityRole to ApplicationRole:
            EntityTypeConfiguration<ApplicationRole> entityTypeConfiguration1 = modelBuilder.Entity<ApplicationRole>().ToTable("AspNetRoles");
            entityTypeConfiguration1.Property((ApplicationRole r) => r.Name).IsRequired();

        }

    }
}