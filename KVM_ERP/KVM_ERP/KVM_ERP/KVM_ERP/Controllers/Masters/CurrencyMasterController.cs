using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KVM_ERP.Models;

namespace KVM_ERP.Controllers.Masters
{
    [SessionExpire]
    public class CurrencyMasterController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        [Authorize(Roles = "CurrencyMasterIndex")]
        public ActionResult Index()
        {
            try
            {
                var currencies = context.Database.SqlQuery<CurrencyMaster>(
                    @"SELECT CURNID, CURNDESC, CURNCODE, CURNAMT, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE
                      FROM CURRENCYMASTER"
                ).ToList();
                
                return View(currencies);
            }
            catch (Exception ex)
            {
                return Content($"Error loading currencies: {ex.Message}");
            }
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("GetAjaxData called for Currency Master DataTables");
                
                // Get currencies
                var currencies = context.Database.SqlQuery<CurrencyMaster>(
                    @"SELECT CURNID, CURNDESC, CURNCODE, CURNAMT, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE
                      FROM CURRENCYMASTER"
                ).ToList();

                // Format data for DataTables
                var allCurrencies = currencies.Select(c => new {
                    CURNID = c.CURNID,
                    CURNCODE = c.CURNCODE ?? "",
                    CURNDESC = c.CURNDESC ?? "",
                    CURNAMT = c.CURNAMT,
                    DISPSTATUS = c.DISPSTATUS,
                    CUSRID = c.CUSRID ?? "",
                    LMUSRID = c.LMUSRID,
                    PRCSDATE = c.PRCSDATE
                }).ToList();

                return Json(new { aaData = allCurrencies }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAjaxData: {ex.Message}");
                return Json(new { error = "Error loading data" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "CurrencyMasterCreate,CurrencyMasterEdit")]
        public ActionResult Form(int? id)
        {
            try
            {
                CurrencyMaster tab = new CurrencyMaster();
                ViewBag.msg = "Add New Currency";

                if (id == null)
                {
                    tab.CURNID = 0;
                    tab.DISPSTATUS = 0; // Default to Enabled
                }

                if (id == -1)
                    ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";

                if (id != 0 && id != -1 && id != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Form: Loading currency with ID: {id}");
                    
                    try
                    {
                        tab = context.Database.SqlQuery<CurrencyMaster>(
                            "SELECT * FROM CURRENCYMASTER WHERE CURNID = {0}", id
                        ).FirstOrDefault();
                        
                        if (tab == null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Currency with ID {id} not found in database");
                            return HttpNotFound();
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"Edit Currency - ID: {tab.CURNID}, Description: {tab.CURNDESC}");
                        System.Diagnostics.Debug.WriteLine($"Code: {tab.CURNCODE}, Amount: {tab.CURNAMT}");
                        System.Diagnostics.Debug.WriteLine($"Status: {tab.DISPSTATUS}");
                        
                        ViewBag.msg = "Edit Currency";
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading currency {id}: {ex.Message}");
                        // Fallback to new currency if loading fails
                        tab = new CurrencyMaster
                        {
                            CURNID = 0,
                            DISPSTATUS = 0
                        };
                        ViewBag.msg = $"Error loading currency {id}. Creating new instead.";
                    }
                }

                // Create status dropdown using SelectList for proper selection
                var statusList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "0", Text = "Enabled" },
                    new SelectListItem { Value = "1", Text = "Disabled" }
                };
                ViewBag.DISPSTATUS = new SelectList(statusList, "Value", "Text", tab.DISPSTATUS.ToString());
                
                // Debug: Show what status is being selected
                System.Diagnostics.Debug.WriteLine($"Status dropdown - Current DISPSTATUS: {tab.DISPSTATUS}, Selected Value: {tab.DISPSTATUS.ToString()}");

                return View(tab);
            }
            catch (Exception ex)
            {
                return Content($"Error in Form action: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CurrencyMasterCreate,CurrencyMasterEdit")]
        public ActionResult savedata(CurrencyMaster tab)
        {
            try
            {
                var currentUserId = Session["USERID"] != null ? Convert.ToInt32(Session["USERID"]) : 1;
                var prcsdate = DateTime.Now;

                // Server-side model validation
                if (!ModelState.IsValid)
                {
                    // Rebuild status dropdown when returning the view with errors
                    var statusList = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "0", Text = "Enabled" },
                        new SelectListItem { Value = "1", Text = "Disabled" }
                    };
                    ViewBag.DISPSTATUS = new SelectList(statusList, "Value", "Text");

                    // Return the same Form view with validation errors
                    return View("Form", tab);
                }

                // Auto-format the data
                if (!string.IsNullOrEmpty(tab.CURNCODE))
                {
                    tab.CURNCODE = tab.CURNCODE.ToUpper();
                }
                if (!string.IsNullOrEmpty(tab.CURNDESC))
                {
                    tab.CURNDESC = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tab.CURNDESC.ToLower());
                }

                if (tab.CURNID == 0)
                {
                    // Create new record
                    tab.CUSRID = Session["USERNAME"]?.ToString() ?? "admin";
                    tab.LMUSRID = currentUserId;
                    tab.PRCSDATE = prcsdate;

                    context.Database.ExecuteSqlCommand(
                        @"INSERT INTO CURRENCYMASTER 
                          (CURNDESC, CURNCODE, CURNAMT, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE) 
                          VALUES 
                          ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                        tab.CURNDESC, tab.CURNCODE, tab.CURNAMT, tab.CUSRID, tab.LMUSRID, tab.DISPSTATUS, tab.PRCSDATE
                    );
                }
                else
                {
                    // Update existing record - preserve CUSRID, only update LMUSRID
                    tab.LMUSRID = currentUserId;
                    tab.PRCSDATE = prcsdate;

                    context.Database.ExecuteSqlCommand(
                        @"UPDATE CURRENCYMASTER SET 
                          CURNDESC = {1}, CURNCODE = {2}, CURNAMT = {3}, LMUSRID = {4}, 
                          DISPSTATUS = {5}, PRCSDATE = {6}
                          WHERE CURNID = {0}",
                        tab.CURNID, tab.CURNDESC, tab.CURNCODE, tab.CURNAMT, tab.LMUSRID, 
                        tab.DISPSTATUS, tab.PRCSDATE
                    );
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in savedata: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult deletedata(int id)
        {
            try
            {
                // Check if user has delete role
                if (!User.IsInRole("CurrencyMasterDelete"))
                {
                    Response.StatusCode = 403;
                    return Content("Access Denied: You do not have permission to delete records. Please contact your administrator.");
                }
                
                context.Database.ExecuteSqlCommand("DELETE FROM CURRENCYMASTER WHERE CURNID = {0}", id);
                return Content("Deleted Successfully ...");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Content("Error deleting currency: " + ex.Message);
            }
        }
    }
}
