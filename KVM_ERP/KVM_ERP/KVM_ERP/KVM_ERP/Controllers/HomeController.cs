using ClubMembership.Data;
using KVM_ERP.Filters;
using KVM_ERP.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using log4net;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KVM_ERP.Controllers
{
    // Model for Event Interest Statistics
    public class EventInterestStat
    {
        public string EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string EventLocation { get; set; }
        public int InterestedCount { get; set; }
        public int NotInterestedCount { get; set; }
        public int TotalResponses { get; set; }
    }

    // Simple DTOs for Notifications sidebar
    public class SimpleMember
    {
        public int MemberID { get; set; }
        public string Member_Name { get; set; }
        public string Member_Photo_Path { get; set; }
    }

    public class SpouseBirthday
    {
        public int MemberID { get; set; }
        public string Member_Name { get; set; }
        public string Spouse_Name { get; set; }
    }

    public class AnniversaryItem
    {
        public int MemberID { get; set; }
        public string Member_Name { get; set; }
        public string Spouse_Name { get; set; }
        public DateTime Date_Of_Marriage { get; set; }
    }

    // [AuthActionFilter]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        //private static readonly ILog log = LogManager.GetLogger(typeof(MembersController));

        public HomeController()
        {
            _db = new ApplicationDbContext();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendWish(int memberId, string type, string recipient, string note)
        {
            // Module removed
            return new HttpStatusCodeResult(204);
        }

        private string BuildWishEmailHtml(string title, string message, string senderFirstName, string recipientName, string note)
        {
            // Lightweight, email-client friendly HTML with inline styles
            return $@"<!DOCTYPE html>
<html>
  <head>
    <meta charset='utf-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1' />
    <title>{System.Net.WebUtility.HtmlEncode(title)}</title>
  </head>
  <body style='margin:0;padding:0;background:#f6f7fb;'>
    <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='background:#f6f7fb;padding:24px 12px;'>
      <tr>
        <td align='center'>
          <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='max-width:560px;background:#ffffff;border-radius:12px;box-shadow:0 4px 16px rgba(0,0,0,0.06);overflow:hidden;'>
            <tr>
              <td style='background:#0d6efd;height:6px;'></td>
            </tr>
            <tr>
              <td style='padding:24px 24px 8px 24px;font-family:Segoe UI,Roboto,Arial,sans-serif;'>
                <h2 style='margin:0 0 10px 0;color:#0d6efd;font-size:22px;font-weight:700;'>{System.Net.WebUtility.HtmlEncode(title)}</h2>
                <p style='margin:0;color:#6c757d;font-size:14px;'>A warm message from a fellow club member</p>
              </td>
            </tr>
            <tr>
              <td style='padding:8px 24px 24px 24px;font-family:Segoe UI,Roboto,Arial,sans-serif;color:#212529;'>
                <p style='font-size:16px;line-height:1.6;margin:0 0 12px 0;'>Dear {System.Net.WebUtility.HtmlEncode(recipientName)},</p>
                <p style='font-size:16px;line-height:1.6;margin:0 0 12px 0;'>{System.Net.WebUtility.HtmlEncode(message)}</p>
                {(string.IsNullOrWhiteSpace(note) ? "" : ("<div style='background:#f8f9fa;border:1px solid #eef2f4;border-radius:8px;padding:12px 14px;margin:0 0 12px 0;'><div style='color:#6c757d;font-size:12px;margin-bottom:6px;'>Personal message</div><div style='font-size:15px;line-height:1.6;color:#212529;'>" + System.Net.WebUtility.HtmlEncode(note) + "</div></div>"))}
                <p style='font-size:16px;line-height:1.6;margin:0 0 12px 0;'>Best regards,<br/><strong>{System.Net.WebUtility.HtmlEncode(senderFirstName)}</strong></p>
              </td>
            </tr>
            <tr>
              <td style='padding:14px 24px 22px 24px;font-family:Segoe UI,Roboto,Arial,sans-serif;border-top:1px solid #eef2f4;color:#6c757d;font-size:12px;'>
                <div>Sent via Club Membership Portal</div>
              </td>
            </tr>
          </table>
          <div style='color:#adb5bd;font-size:12px;margin-top:10px;font-family:Segoe UI,Roboto,Arial,sans-serif;'>
            &copy; {DateTime.Now:yyyy} Club Membership
          </div>
        </td>
      </tr>
    </table>
  </body>
</html>";
        }

        private bool TrySendEmail(string to, string subject, string body, string senderDisplayName, out string error)
        {
            error = null;
            try
            {
                // Ensure TLS 1.2 for Gmail/modern SMTP
                try { System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12; } catch { }

                // Uses SMTP settings from Web.config <system.net><mailSettings><smtp>
                using (var mail = new System.Net.Mail.MailMessage())
                {
                    mail.To.Add(to);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true; // allow basic formatting

                    // Respect Web.config <system.net><mailSettings><smtp from="...">. If not present, use DEFAULT_FROM_EMAIL.
                    if (mail.From == null)
                    {
                        var fallbackFrom = System.Configuration.ConfigurationManager.AppSettings["DEFAULT_FROM_EMAIL"] ?? "support@fusiontec.com";
                        mail.From = new System.Net.Mail.MailAddress(fallbackFrom, string.IsNullOrWhiteSpace(senderDisplayName) ? "Club Membership" : senderDisplayName);
                    }

                    using (var smtp = new System.Net.Mail.SmtpClient())
                    {
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (System.Net.Mail.SmtpException smtpEx)
            {
                var code = smtpEx.StatusCode;
                var msg = smtpEx.Message;
                var inner = smtpEx.InnerException != null ? (": " + smtpEx.InnerException.Message) : string.Empty;
                error = $"SMTP error ({code}): {msg}{inner}";
                return false;
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException != null ? (": " + ex.InnerException.Message) : string.Empty;
                error = ex.Message + inner;
                return false;
            }
        }

        public ActionResult AdminDashboard()
        {
            // Dashboard accessible to all users (Admin and regular users)
            try
            {
                var statsDict = new Dictionary<string, DashboardStat>();

                System.Diagnostics.Debug.WriteLine("=== Dashboard Data Loading Started ===");

                // 1. SHRIMP TYPES - Different material types (Active only)
                var shrimpTypesCount = _db.MaterialMasters
                    .Where(m => m.DISPSTATUS == 0 || m.DISPSTATUS == null)
                    .Select(m => m.MTRLID)
                    .Distinct()
                    .Count();
                statsDict["ShrimpTypes"] = new DashboardStat { StatType = "ShrimpTypes", TotalCount = shrimpTypesCount, Details = "" };
                System.Diagnostics.Debug.WriteLine($"✓ Shrimp Types (Active Materials): {shrimpTypesCount}");

                // 2. TOTAL INVOICES (REGSTRID = 2 means Raw Material Invoice)
                var totalInvoices = _db.TransactionMasters
                    .Where(tm => tm.REGSTRID == 2)
                    .Count();
                statsDict["TotalInvoices"] = new DashboardStat { StatType = "TotalInvoices", TotalCount = totalInvoices, Details = "" };
                System.Diagnostics.Debug.WriteLine($"✓ Total Invoices (REGSTRID=2): {totalInvoices}");

                // DEBUG: Show all status codes with counts
                var statusBreakdown = (from tm in _db.TransactionMasters
                                      join pis in _db.PurchaseInvoiceStatuses on tm.DISPSTATUS equals pis.PUINSTID into pisJoin
                                      from pis in pisJoin.DefaultIfEmpty()
                                      where tm.REGSTRID == 2
                                      group tm by new { StatusCode = pis != null ? pis.PUINSTCODE : "NULL", StatusDesc = pis != null ? pis.PUINSTDESC : "No Status" } into g
                                      select new { g.Key.StatusCode, g.Key.StatusDesc, Count = g.Count() })
                                      .ToList();
                
                System.Diagnostics.Debug.WriteLine("--- Invoice Status Breakdown ---");
                foreach (var status in statusBreakdown)
                {
                    System.Diagnostics.Debug.WriteLine($"  {status.StatusCode} ({status.StatusDesc}): {status.Count} invoices");
                }

                // 3. INVOICES WAITING APPROVAL - Only invoices with PUS003 (Waiting for Approval) status
                var waitingApprovalCount = (from tm in _db.TransactionMasters
                                           join pis in _db.PurchaseInvoiceStatuses on tm.DISPSTATUS equals pis.PUINSTID
                                           where tm.REGSTRID == 2
                                               && pis.PUINSTCODE == "PUS003"  // Waiting for Approval status
                                           select tm.TRANMID).Count();
                
                statsDict["InvoicesWaitingApproval"] = new DashboardStat { StatType = "InvoicesWaitingApproval", TotalCount = waitingApprovalCount, Details = "" };
                System.Diagnostics.Debug.WriteLine($"✓ Invoices Waiting Approval (PUS003): {waitingApprovalCount}");

                // 4. INVOICES APPROVED - Only invoices with PUS004 (Approved) status
                var approvedInvoicesCount = (from tm in _db.TransactionMasters
                                            join pis in _db.PurchaseInvoiceStatuses on tm.DISPSTATUS equals pis.PUINSTID
                                            where tm.REGSTRID == 2
                                                && pis.PUINSTCODE == "PUS004"  // Approved status
                                            select tm.TRANMID).Count();
                statsDict["InvoicesApproved"] = new DashboardStat { StatType = "InvoicesApproved", TotalCount = approvedInvoicesCount, Details = "" };
                System.Diagnostics.Debug.WriteLine($"✓ Invoices Approved (PUS004): {approvedInvoicesCount}");

                // 5. RAW MATERIAL INTAKE (REGSTRID = 1)
                var rawMaterialIntake = _db.TransactionMasters
                    .Where(tm => tm.REGSTRID == 1)
                    .Count();
                statsDict["RawMaterialIntake"] = new DashboardStat { StatType = "RawMaterialIntake", TotalCount = rawMaterialIntake, Details = "" };
                System.Diagnostics.Debug.WriteLine($"✓ Raw Material Intake (REGSTRID=1): {rawMaterialIntake}");

                // 6. TOTAL SUPPLIERS (Active only)
                var totalSuppliers = _db.SupplierMasters
                    .Where(s => s.DISPSTATUS == 0 || s.DISPSTATUS == null)
                    .Count();
                statsDict["TotalSuppliers"] = new DashboardStat { StatType = "TotalSuppliers", TotalCount = totalSuppliers, Details = "" };
                System.Diagnostics.Debug.WriteLine($"✓ Total Suppliers (Active): {totalSuppliers}");

                // 7. TOTAL CUSTOMERS (Active only)
                var totalCustomers = _db.CustomerMasters
                    .Where(c => c.DISPSTATUS == 0 || c.DISPSTATUS == null)
                    .Count();
                statsDict["TotalCustomers"] = new DashboardStat { StatType = "TotalCustomers", TotalCount = totalCustomers, Details = "" };
                System.Diagnostics.Debug.WriteLine($"✓ Total Customers (Active): {totalCustomers}");

                // 8. THIS MONTH INVOICES (Current month/year, REGSTRID=2)
                var thisMonthInvoices = _db.TransactionMasters
                    .Where(tm => tm.REGSTRID == 2
                        && tm.TRANDATE.Month == DateTime.Now.Month
                        && tm.TRANDATE.Year == DateTime.Now.Year)
                    .Count();
                statsDict["ThisMonthInvoices"] = new DashboardStat { StatType = "ThisMonthInvoices", TotalCount = thisMonthInvoices, Details = "" };
                System.Diagnostics.Debug.WriteLine($"✓ This Month Invoices: {thisMonthInvoices}");

                // 9. TODAY'S TRANSACTIONS (All types)
                var todayTransactions = _db.TransactionMasters
                    .Where(tm => DbFunctions.TruncateTime(tm.TRANDATE) == DbFunctions.TruncateTime(DateTime.Now))
                    .Count();
                statsDict["TodayTransactions"] = new DashboardStat { StatType = "TodayTransactions", TotalCount = todayTransactions, Details = "" };
                System.Diagnostics.Debug.WriteLine($"✓ Today's Transactions: {todayTransactions}");

                ViewBag.DashboardStats = statsDict;

                // SHRIMP TYPES BY RECEIVED TYPE (for chart) - Active transactions only
                var shrimpByType = (from tpc in _db.TransactionProductCalculations
                                   join td in _db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                   join tm in _db.TransactionMasters on td.TRANMID equals tm.TRANMID
                                   join rt in _db.ReceivedTypeMasters on tpc.RCVDTID equals rt.RCVDTID into rtLeft
                                   from rt in rtLeft.DefaultIfEmpty()
                                   where (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                       && (td.DISPSTATUS == 0 || td.DISPSTATUS == null)
                                       && tm.REGSTRID == 1  // Raw Material Intake only
                                   group tpc by new { ReceivedType = rt != null ? rt.RCVDTDESC : "Unknown" } into g
                                   select new ShrimpByTypeDTO
                                   {
                                       ReceivedType = g.Key.ReceivedType,
                                       Count = g.Count()
                                   })
                                   .OrderByDescending(x => x.Count)
                                   .ToList();
                ViewBag.ShrimpByType = shrimpByType;
                System.Diagnostics.Debug.WriteLine($"✓ Shrimp By Type: {shrimpByType.Count} types found");

                // MONTHLY INVOICE TREND (Last 6 months, REGSTRID=2 only)
                var sixMonthsAgo = DateTime.Now.AddMonths(-6);
                var monthlyInvoicesRaw = (from tm in _db.TransactionMasters
                                         where tm.REGSTRID == 2  // Only invoices
                                             && tm.TRANDATE >= sixMonthsAgo
                                         group tm by new { tm.TRANDATE.Month, tm.TRANDATE.Year } into g
                                         orderby g.Key.Year, g.Key.Month
                                         select new 
                                         {
                                             Month = g.Key.Month,
                                             Year = g.Key.Year,
                                             InvoiceCount = g.Count()
                                         })
                                         .ToList();
                
                // Convert month number to name in memory (not in SQL)
                var monthlyInvoices = monthlyInvoicesRaw.Select(m => new MonthlyInvoiceDTO
                {
                    MonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m.Month),
                    InvoiceCount = m.InvoiceCount
                }).ToList();
                
                ViewBag.MonthlyInvoices = monthlyInvoices;
                System.Diagnostics.Debug.WriteLine($"✓ Monthly Invoices: {monthlyInvoices.Count} months with data");

                // TOP 5 SHRIMP TYPES BY QUANTITY (Active records only)
                var topShrimpTypes = (from tpc in _db.TransactionProductCalculations
                                     join td in _db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                     join tm in _db.TransactionMasters on td.TRANMID equals tm.TRANMID
                                     join m in _db.MaterialMasters on td.MTRLID equals m.MTRLID
                                     where (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                         && (td.DISPSTATUS == 0 || td.DISPSTATUS == null)
                                         && (m.DISPSTATUS == 0 || m.DISPSTATUS == null)
                                         && tm.REGSTRID == 1  // Raw Material Intake
                                     group tpc by m.MTRLDESC into g
                                     select new TopShrimpTypeDTO
                                     {
                                         ShrimpType = g.Key,
                                         Transactions = g.Count(),
                                         TotalQuantity = g.Sum(x =>
                                             x.PCK1 + x.PCK2 + x.PCK3 + x.PCK4 + x.PCK5 +
                                             x.PCK6 + x.PCK7 + x.PCK8 + x.PCK9 + x.PCK10 +
                                             x.PCK11 + x.PCK12 + x.PCK13 + x.PCK14 + x.PCK15 +
                                             x.PCK16 + x.PCK17)
                                     })
                                     .OrderByDescending(x => x.TotalQuantity)
                                     .Take(5)
                                     .ToList();
                ViewBag.TopShrimpTypes = topShrimpTypes;
                System.Diagnostics.Debug.WriteLine($"✓ Top Shrimp Types: {topShrimpTypes.Count} items");

                System.Diagnostics.Debug.WriteLine("=== Dashboard Data Loading Completed Successfully ===");

                System.Diagnostics.Debug.WriteLine($"Dashboard stats loaded successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR loading dashboard stats: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                
                ViewBag.DashboardStats = new Dictionary<string, DashboardStat>();
                ViewBag.ShrimpByType = new List<ShrimpByTypeDTO>();
                ViewBag.MonthlyInvoices = new List<MonthlyInvoiceDTO>();
                ViewBag.TopShrimpTypes = new List<TopShrimpTypeDTO>();
                ViewBag.ErrorMessage = ex.Message;
            }

            return View();
        }

        public ActionResult Index()
        {
            // Show the same dashboard for all users (Admin or regular users)
            return RedirectToAction("AdminDashboard");
        }

        [HttpGet]
        public ActionResult RenewalPopup(int memberId)
        {
            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitRenewal(RenewalSubmitRequest request)
        {
            Response.StatusCode = 404;
            return Json(new { success = false, message = "Not available" });
        }

        [HttpGet]
        public ActionResult Notifications()
        {
            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptNotification(int eventId)
        {
            return new HttpStatusCodeResult(204);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeclineNotification(int eventId)
        {
            return new HttpStatusCodeResult(204);
        }

        [HttpGet]
        public ActionResult UserDashboard()
        {
            return HttpNotFound();
        }
    }

    // Dashboard DTOs
    public class DashboardStat
    {
        public string StatType { get; set; }
        public int TotalCount { get; set; }
        public string Details { get; set; }
    }

    public class ShrimpByTypeDTO
    {
        public string ReceivedType { get; set; }
        public int Count { get; set; }
    }

    public class MonthlyInvoiceDTO
    {
        public string MonthName { get; set; }
        public int InvoiceCount { get; set; }
    }

    public class TopShrimpTypeDTO
    {
        public string ShrimpType { get; set; }
        public int Transactions { get; set; }
        public decimal TotalQuantity { get; set; }
    }
}