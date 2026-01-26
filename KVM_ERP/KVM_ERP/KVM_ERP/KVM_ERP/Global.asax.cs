using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.IO;
using log4net;
using System.Data.Entity;
using KVM_ERP.Models;

namespace KVM_ERP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MvcApplication));
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            // Configure log4net
            ConfigureLog4Net();

            // Create new content tables if they don't exist (safe, non-destructive)
            EnsureContentTablesExist();
        }

        

        private void EnsureContentTablesExist()
        {
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    ctx.Database.ExecuteSqlCommand(@"
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ANNOUNCEMENTMASTER]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ANNOUNCEMENTMASTER](
        [AnnouncementId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Heading] NVARCHAR(200) NULL,
        [Caption] NVARCHAR(500) NULL,
        [Description] NVARCHAR(2000) NULL,
        [MainImage] NVARCHAR(MAX) NULL,
        [AdditionalImages] NVARCHAR(MAX) NULL,
        [Status] SMALLINT NOT NULL DEFAULT 0,
        [CreatedBy] NVARCHAR(256) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT(GETDATE()),
        [ModifiedBy] NVARCHAR(256) NULL,
        [ModifiedDate] DATETIME NULL,
        [CompanyId] INT NOT NULL DEFAULT 0
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EVENTMASTER]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EVENTMASTER](
        [EventId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Heading] NVARCHAR(200) NULL,
        [Caption] NVARCHAR(500) NULL,
        [Description] NVARCHAR(2000) NULL,
        [MainImage] NVARCHAR(MAX) NULL,
        [AdditionalImages] NVARCHAR(MAX) NULL,
        [EventTime] DATETIME NOT NULL,
        [EventPlan] NVARCHAR(1000) NULL,
        [EventLocation] NVARCHAR(200) NULL,
        [Status] SMALLINT NOT NULL DEFAULT 0,
        [CreatedBy] NVARCHAR(256) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT(GETDATE()),
        [ModifiedBy] NVARCHAR(256) NULL,
        [ModifiedDate] DATETIME NULL,
        [CompanyId] INT NOT NULL DEFAULT 0
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GALLERYMASTER]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[GALLERYMASTER](
        [GalleryId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Heading] NVARCHAR(200) NULL,
        [Caption] NVARCHAR(500) NULL,
        [Description] NVARCHAR(2000) NULL,
        [MainImage] NVARCHAR(MAX) NULL,
        [AdditionalImages] NVARCHAR(MAX) NULL,
        [Category] NVARCHAR(100) NULL,
        [Status] SMALLINT NOT NULL DEFAULT 0,
        [CreatedBy] NVARCHAR(256) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT(GETDATE()),
        [ModifiedBy] NVARCHAR(256) NULL,
        [ModifiedDate] DATETIME NULL,
        [CompanyId] INT NOT NULL DEFAULT 0
    );
END");
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to ensure content tables exist", ex);
            }
        }

        private void ConfigureLog4Net()
        {
            // Configure log4net using the Web.config settings
            log4net.Config.XmlConfigurator.Configure();

            // Ensure log directory exists
            try
            {
                string logDirectory = Server.MapPath("~/App_Data/Logs");
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                    log.Info($"Created log directory at: {logDirectory}");
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to configure logging directory", ex);
            }
        }
    }
}
