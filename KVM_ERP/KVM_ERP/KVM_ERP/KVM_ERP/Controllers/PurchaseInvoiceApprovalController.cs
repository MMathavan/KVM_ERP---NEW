using KVM_ERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KVM_ERP.Controllers
{
    [SessionExpire]
    public class PurchaseInvoiceApprovalController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();

        // GET: PurchaseInvoiceApproval
        [Authorize(Roles = "PurchaseInvoiceApprovalIndex")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Ajax data for DataTables - "Waiting for Approval" invoices
        [Authorize(Roles = "PurchaseInvoiceApprovalIndex")]
        public JsonResult GetAjaxData(JQueryDataTableParamModel param, string fromDate = null, string toDate = null, string status = "waiting")
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"PurchaseInvoiceApproval GetAjaxData called - Status: {status}, FromDate: {fromDate}, ToDate: {toDate}");
                
                // Determine status code based on parameter
                string statusCode = status == "approved" ? "PUS004" : "PUS003";
                
                // Build SQL query - Get ONLY invoices with selected/checked items based on TRANDAID
                var sql = @"SELECT DISTINCT tm.TRANMID, tm.TRANDATE, tm.TRANNO, tm.TRANDNO, tm.TRANREFNO, tm.CATENAME, 
                           ISNULL(tm.TRANNAMT, 0) as TRANNAMT,
                           tm.DISPSTATUS,
                           ISNULL(pis.PUINSTDESC, 'N/A') as StatusDescription
                           FROM TRANSACTIONMASTER tm
                           LEFT JOIN PURCHASEINVOICESTATUS pis ON tm.DISPSTATUS = pis.PUINSTID
                           INNER JOIN TRANSACTIONDETAIL td ON tm.TRANMID = td.TRANMID
                           WHERE tm.REGSTRID = 2 
                           AND pis.PUINSTCODE = @p0
                           AND td.TRANDAID IS NOT NULL 
                           AND td.TRANDAID > 0";
                
                var parameters = new List<object>();
                parameters.Add(statusCode); // @p0 for status code
                
                // Add date filters if provided
                if (!string.IsNullOrEmpty(fromDate))
                {
                    sql += " AND tm.TRANDATE >= @p" + parameters.Count;
                    parameters.Add(DateTime.Parse(fromDate));
                }
                
                if (!string.IsNullOrEmpty(toDate))
                {
                    sql += " AND tm.TRANDATE <= @p" + parameters.Count;
                    parameters.Add(DateTime.Parse(toDate).AddDays(1).AddSeconds(-1)); // Include full day
                }
                
                sql += " ORDER BY tm.TRANDATE DESC, tm.TRANNO DESC";
                
                // Get invoice data from TRANSACTIONMASTER
                var invoices = context.Database.SqlQuery<RawMaterialInvoiceViewModel>(sql, parameters.ToArray()).ToList();

                System.Diagnostics.Debug.WriteLine($"Found {invoices.Count} invoices with status: {status}");

                // Format data for DataTables
                var allInvoices = invoices.Select(i => new {
                    TRANMID = i.TRANMID,
                    TRANDATE = i.TRANDATE,
                    TRANNO = i.TRANNO,
                    TRANDNO = i.TRANDNO ?? "0000",
                    TRANREFNO = i.TRANREFNO ?? "-",
                    CATENAME = i.CATENAME ?? "",
                    TRANNAMT = i.TRANNAMT,
                    DISPSTATUS = i.DISPSTATUS,
                    StatusDescription = i.StatusDescription
                }).ToList();

                return Json(new { aaData = allInvoices }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in PurchaseInvoiceApproval GetAjaxData: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return Json(new { error = "Error loading data: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Print Invoice (Approval) - reuse the same data and template as Raw Material Invoice print
        [Authorize(Roles = "PurchaseInvoiceApprovalPrint")]
        public ActionResult Print(int id)
        {
            try
            {
                // Get invoice header (same fields as RawMaterialInvoiceController.Print)
                var invoice = context.Database.SqlQuery<InvoicePrintViewModel>(
                    @"SELECT tm.TRANMID, tm.TRANNO, tm.TRANDNO, tm.TRANREFNO, tm.TRANDATE,
                             tm.CATENAME, tm.CATECODE, tm.TRANNAMT, 
                             pis.PUINSTDESC as StatusDescription,
                             ISNULL(tm.TRANCGSTAMT, 0) as CGSTAMT,
                             ISNULL(tm.TRANSGSTAMT, 0) as SGSTAMT,
                             ISNULL(tm.TRANIGSTAMT, 0) as IGSTAMT,
                             ISNULL(tm.TRANCGSTEXPRN, 0) as CGSTPER,
                             ISNULL(tm.TRANSGSTEXPRN, 0) as SGSTPER,
                             ISNULL(tm.TRANIGSTEXPRN, 0) as IGSTPER,
                             ISNULL(tm.TRANGAMT, 0) as TRANGAMT,
                             ISNULL(tm.TRANPACKAMT, 0) as TRANPACKAMT,
                             ISNULL(tm.TRANINCAMT, 0) as TRANINCAMT
                      FROM TRANSACTIONMASTER tm
                      LEFT JOIN PURCHASEINVOICESTATUS pis ON tm.DISPSTATUS = pis.PUINSTID
                      WHERE tm.TRANMID = @p0 AND tm.REGSTRID = 2",
                    id
                ).FirstOrDefault();

                if (invoice == null)
                {
                    TempData["ErrorMessage"] = "Invoice not found";
                    return RedirectToAction("Index");
                }

                // Get invoice items (same fields as RawMaterialInvoiceController.Print)
                invoice.Items = context.Database.SqlQuery<InvoiceItemPrintViewModel>(
                    @"SELECT td.TRANDID, m.MTRLDESC as MTRLNAME, 
                             ISNULL(g.GRADEDESC, '') as GRADEDESC,
                             ISNULL(pcm.PCLRDESC, '') as PCLRDESC,
                             ISNULL(rt.RCVDTDESC, '') as RCVDTDESC,
                             td.TRANDQTY as TRANQTY, 
                             td.TRANDRATE as TRANRATE, 
                             td.TRANDAMT,
                             ISNULL(td.TRANDDISCEXPRN, 0) as PACKINGKG,
                             ISNULL(td.TRANDDISCAMT, 0) as PACKINGAMOUNT,
                             ISNULL(td.TRANDNAMT, 0) as NETAMOUNT,
                             ISNULL(td.TRANDINCAMT, 0) as INCENTIVEAMOUNT
                      FROM TRANSACTIONDETAIL td
                      INNER JOIN MATERIALMASTER m ON td.MTRLID = m.MTRLID
                      LEFT JOIN GRADEMASTER g ON td.GRADEID = g.GRADEID
                      LEFT JOIN PRODUCTIONCOLOURMASTER pcm ON td.PCLRID = pcm.PCLRID
                      LEFT JOIN RECEIVEDTYPEMASTER rt ON td.RCVDTID = rt.RCVDTID
                      WHERE td.TRANMID = @p0
                      ORDER BY td.TRANDID",
                    id
                ).ToList();

                // Get tax factors (unchanged, same model as invoice print)
                invoice.TaxFactors = context.Database.SqlQuery<TaxFactorPrintViewModel>(
                    @"SELECT tmf.TRANMFID, 
                             ISNULL(tmf.TRANCFDESC, cf.CFDESC) as CFDESC,
                             ISNULL(CAST(tmf.CFOPTN AS INT), 0) as OPTNVALUE,
                             ISNULL(tmf.DEDEXPRN, 0) as CFRATE,
                             ISNULL(tmf.DEDVALUE, 0) as CFAMT,
                             ISNULL(CAST(tmf.DEDMODE AS INT), 0) as CFMODE
                      FROM TRANSACTIONMASTERFACTOR tmf
                      INNER JOIN COSTFACTORMASTER cf ON tmf.CFID = cf.CFID
                      WHERE tmf.TRANMID = @p0
                      ORDER BY tmf.DEDORDR",
                    id
                ).ToList();

                // Reuse the same Razor view as the main Raw Material Invoice print
                return View("~/Views/RawMaterialInvoice/Print.cshtml", invoice);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading invoice for print: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading invoice: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // Note: RawMaterialInvoiceViewModel is defined in RawMaterialInvoiceController.cs
    // and is shared across both controllers in the same namespace
}
