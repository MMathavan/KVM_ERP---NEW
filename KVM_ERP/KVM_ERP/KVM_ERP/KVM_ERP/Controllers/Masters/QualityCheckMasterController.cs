using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KVM_ERP.Models;

namespace KVM_ERP.Controllers.Masters
{
    [SessionExpire]
    public class QualityCheckMasterController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        [Authorize(Roles = "QualityCheckMasterIndex")]
        public ActionResult Index()
        {
            try
            {
                var qualityChecks = context.Database.SqlQuery<QualityCheckMaster>(
                    @"SELECT QUALIID, QUALIDESC, QUALICODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE
                      FROM QUALITYCHECKMASTER
                      ORDER BY QUALICODE"
                ).ToList();
                
                return View(qualityChecks);
            }
            catch (Exception ex)
            {
                return Content($"Error loading quality checks: {ex.Message}");
            }
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("GetAjaxData called for Quality Check Master DataTables");
                
                // Get quality checks
                var qualityChecks = context.Database.SqlQuery<QualityCheckMaster>(
                    @"SELECT QUALIID, QUALIDESC, QUALICODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE
                      FROM QUALITYCHECKMASTER
                      ORDER BY QUALICODE"
                ).ToList();

                // Format data for DataTables
                var allQualityChecks = qualityChecks.Select(q => new {
                    QUALIID = q.QUALIID,
                    QUALICODE = q.QUALICODE ?? "",
                    QUALIDESC = q.QUALIDESC ?? "",
                    DISPSTATUS = q.DISPSTATUS,
                    CUSRID = q.CUSRID ?? "",
                    LMUSRID = q.LMUSRID ?? "",
                    PRCSDATE = q.PRCSDATE
                }).ToList();

                return Json(new { aaData = allQualityChecks }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAjaxData: {ex.Message}");
                return Json(new { error = "Error loading data" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "QualityCheckMasterCreate,QualityCheckMasterEdit")]
        public ActionResult Form(int? id)
        {
            try
            {
                QualityCheckMaster tab = new QualityCheckMaster();
                ViewBag.msg = "Add New Quality Check";

                if (id == null)
                {
                    tab.QUALIID = 0;
                    tab.DISPSTATUS = 0; // Default to Enabled
                }

                if (id == -1)
                    ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";

                if (id != 0 && id != -1 && id != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Form: Loading quality check with ID: {id}");
                    
                    try
                    {
                        tab = context.Database.SqlQuery<QualityCheckMaster>(
                            "SELECT * FROM QUALITYCHECKMASTER WHERE QUALIID = {0}", id
                        ).FirstOrDefault();
                        
                        if (tab == null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Quality check with ID {id} not found in database");
                            return HttpNotFound();
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"Edit Quality Check - ID: {tab.QUALIID}, Description: {tab.QUALIDESC}");
                        System.Diagnostics.Debug.WriteLine($"Code: {tab.QUALICODE}, Status: {tab.DISPSTATUS}");
                        
                        ViewBag.msg = "Edit Quality Check";
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading quality check {id}: {ex.Message}");
                        // Fallback to new quality check if loading fails
                        tab = new QualityCheckMaster
                        {
                            QUALIID = 0,
                            DISPSTATUS = 0
                        };
                        ViewBag.msg = $"Error loading quality check {id}. Creating new instead.";
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
        [Authorize(Roles = "QualityCheckMasterCreate,QualityCheckMasterEdit")]
        public ActionResult savedata(QualityCheckMaster tab)
        {
            try
            {
                var currentUsername = Session["USERNAME"]?.ToString() ?? "admin";
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
                if (!string.IsNullOrEmpty(tab.QUALICODE))
                {
                    tab.QUALICODE = tab.QUALICODE.ToUpper();
                }
                if (!string.IsNullOrEmpty(tab.QUALIDESC))
                {
                    tab.QUALIDESC = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tab.QUALIDESC.ToLower());
                }

                if (tab.QUALIID == 0)
                {
                    // Create new record
                    tab.CUSRID = currentUsername;
                    tab.LMUSRID = currentUsername;
                    tab.PRCSDATE = prcsdate;

                    context.Database.ExecuteSqlCommand(
                        @"INSERT INTO QUALITYCHECKMASTER 
                          (QUALIDESC, QUALICODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE) 
                          VALUES 
                          ({0}, {1}, {2}, {3}, {4}, {5})",
                        tab.QUALIDESC, tab.QUALICODE, tab.CUSRID, tab.LMUSRID, tab.DISPSTATUS, tab.PRCSDATE
                    );
                }
                else
                {
                    // Update existing record - preserve CUSRID, only update LMUSRID
                    tab.LMUSRID = currentUsername;
                    tab.PRCSDATE = prcsdate;

                    context.Database.ExecuteSqlCommand(
                        @"UPDATE QUALITYCHECKMASTER SET 
                          QUALIDESC = {1}, QUALICODE = {2}, LMUSRID = {3}, 
                          DISPSTATUS = {4}, PRCSDATE = {5}
                          WHERE QUALIID = {0}",
                        tab.QUALIID, tab.QUALIDESC, tab.QUALICODE, tab.LMUSRID, 
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
        [Authorize(Roles = "QualityCheckMasterDelete")]
        public ActionResult deletedata(int id)
        {
            try
            {
                context.Database.ExecuteSqlCommand("DELETE FROM QUALITYCHECKMASTER WHERE QUALIID = {0}", id);
                return Content("Deleted Successfully ...");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Content("Error deleting quality check: " + ex.Message);
            }
        }
    }
}
