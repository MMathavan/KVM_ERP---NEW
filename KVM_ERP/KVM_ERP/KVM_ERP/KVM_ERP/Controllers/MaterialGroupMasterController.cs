using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using KVM_ERP.Models;

namespace KVM_ERP.Controllers
{
    [SessionExpire]
    public class MaterialGroupMasterController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MaterialGroupMaster
        [Authorize(Roles = "MaterialGroupMasterIndex")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: MaterialGroupMaster/Form
        [Authorize(Roles = "MaterialGroupMasterCreate,MaterialGroupMasterEdit")]
        public ActionResult Form(int id = 0)
        {
            MaterialGroupMaster tab = new MaterialGroupMaster();
            
            if (id == 0)
            {
                // New record
                tab.MTRLGID = 0;
                tab.DISPSTATUS = 0; // Default to Enabled
            }
            else
            {
                // Edit existing record
                tab = db.MaterialGroupMasters.Find(id);
                if (tab == null)
                {
                    ViewBag.msg = "<div class='alert alert-danger'>Record not found!</div>";
                    tab = new MaterialGroupMaster();
                    tab.DISPSTATUS = 0;
                }
            }

            // Populate status dropdown with proper selection
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Enabled" },
                new SelectListItem { Value = "1", Text = "Disabled" }
            };

            // Use SelectList with selected value for proper binding
            ViewBag.DISPSTATUS = new SelectList(statusList, "Value", "Text", tab.DISPSTATUS.ToString());

            return View(tab);
        }

        // POST: MaterialGroupMaster/savedata
        [HttpPost]
        [Authorize(Roles = "MaterialGroupMasterCreate,MaterialGroupMasterEdit")]
        public ActionResult savedata(MaterialGroupMaster tab)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check for duplicate code on server side
                    var duplicateCheck = db.Database.SqlQuery<int>(
                        @"SELECT COUNT(*) FROM MATERIALGROUPMASTER 
                          WHERE UPPER(MTRLGCODE) = @p0 AND MTRLGID != @p1",
                        tab.MTRLGCODE.ToUpper(), tab.MTRLGID
                    ).FirstOrDefault();

                    if (duplicateCheck > 0)
                    {
                        ModelState.AddModelError("MTRLGCODE", "This material group code is already used.");
                        ViewBag.msg = "<div class='alert alert-danger'>Material group code already exists. Please use a different code.</div>";
                    }
                    else
                    {
                        // Get current user information from session (using correct session keys)
                        var currentUserName = Session["CUSRID"]?.ToString();
                        
                        // If session doesn't have CUSRID, try to get from User.Identity
                        if (string.IsNullOrEmpty(currentUserName))
                        {
                            currentUserName = User.Identity.Name ?? "admin";
                        }
                        
                        // Debug: Log session values
                        System.Diagnostics.Debug.WriteLine($"Session CUSRID: {Session["CUSRID"]}");
                        System.Diagnostics.Debug.WriteLine($"Session Group: {Session["Group"]}");
                        System.Diagnostics.Debug.WriteLine($"User.Identity.Name: {User.Identity.Name}");
                        System.Diagnostics.Debug.WriteLine($"Final currentUserName: {currentUserName}");
                        var prcsdate = DateTime.Now;

                        // Auto-format description to title case
                        if (!string.IsNullOrEmpty(tab.MTRLGDESC))
                        {
                            tab.MTRLGDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(tab.MTRLGDESC.ToLower());
                        }

                        // Auto-format code to uppercase
                        if (!string.IsNullOrEmpty(tab.MTRLGCODE))
                        {
                            tab.MTRLGCODE = tab.MTRLGCODE.ToUpper();
                        }

                        if (tab.MTRLGID == 0)
                        {
                            // New record - CUSRID gets username, LMUSRID gets user ID (both same user, different formats)
                            System.Diagnostics.Debug.WriteLine($"Creating new record with CUSRID: {currentUserName}, LMUSRID: {currentUserName}");
                            
                            db.Database.ExecuteSqlCommand(
                                @"INSERT INTO MATERIALGROUPMASTER (MTRLGDESC, MTRLGCODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE) 
                                  VALUES (@p0, @p1, @p2, @p3, @p4, @p5)",
                                tab.MTRLGDESC, tab.MTRLGCODE, currentUserName, currentUserName, tab.DISPSTATUS, prcsdate);
                            
                            // Redirect to Index after successful save
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            // Update existing record - preserve original CUSRID, update only LMUSRID with current user
                            System.Diagnostics.Debug.WriteLine($"Updating existing record ID: {tab.MTRLGID}, LMUSRID: {currentUserName}");
                            
                            db.Database.ExecuteSqlCommand(
                                @"UPDATE MATERIALGROUPMASTER 
                                  SET MTRLGDESC = @p0, MTRLGCODE = @p1, LMUSRID = @p2, 
                                      DISPSTATUS = @p3, PRCSDATE = @p4 
                                  WHERE MTRLGID = @p5",
                                tab.MTRLGDESC, tab.MTRLGCODE, currentUserName, tab.DISPSTATUS, prcsdate, tab.MTRLGID);
                            
                            // Redirect to Index after successful update
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    ViewBag.msg = "<div class='alert alert-danger'>Please fix the validation errors.</div>";
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = "<div class='alert alert-danger'>Error: " + ex.Message + "</div>";
            }

            // Rebuild dropdown for status (for validation errors)
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Enabled" },
                new SelectListItem { Value = "1", Text = "Disabled" }
            };

            ViewBag.DISPSTATUS = new SelectList(statusList, "Value", "Text", tab.DISPSTATUS.ToString());

            return View("Form", tab);
        }

        // GET: MaterialGroupMaster/GetAjaxData
        [HttpGet]
        public JsonResult GetAjaxData()
        {
            try
            {
                // Get all data using raw SQL to match database schema
                var materialGroups = db.Database.SqlQuery<MaterialGroupMaster>(
                    @"SELECT MTRLGID, MTRLGDESC, MTRLGCODE, CUSRID, LMUSRID, 
                             DISPSTATUS, PRCSDATE 
                      FROM MATERIALGROUPMASTER 
                      ORDER BY MTRLGCODE"
                ).ToList();

                var result = materialGroups.Select(m => new
                {
                    m.MTRLGID,
                    m.MTRLGCODE,
                    m.MTRLGDESC,
                    m.DISPSTATUS
                }).ToList();

                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the error details for debugging
                System.Diagnostics.Debug.WriteLine($"MaterialGroupMaster GetAjaxData Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                // Return error information for debugging
                return Json(new { 
                    data = new List<object>(),
                    error = ex.Message,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: MaterialGroupMaster/Del
        [HttpPost]
        public ActionResult Del(int id)
        {
            try
            {
                // Check if user has delete role
                if (!User.IsInRole("MaterialGroupMasterDelete"))
                {
                    return Content("Access Denied: You do not have permission to delete records. Please contact your administrator.");
                }
                
                // Delete using raw SQL
                var rowsAffected = db.Database.ExecuteSqlCommand(
                    "DELETE FROM MATERIALGROUPMASTER WHERE MTRLGID = @p0", id);
                
                if (rowsAffected > 0)
                {
                    return Content("Successfully deleted");
                }
                else
                {
                    return Content("Record not found");
                }
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }

        // Remote validation for unique material group code
        [HttpPost]
        public JsonResult ValidateMTRLGCODE(string MTRLGCODE, int MTRLGID = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(MTRLGCODE))
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }

                // Check if code already exists (excluding current record for edit)
                var existingRecord = db.Database.SqlQuery<int>(
                    @"SELECT COUNT(*) FROM MATERIALGROUPMASTER 
                      WHERE UPPER(MTRLGCODE) = @p0 AND MTRLGID != @p1",
                    MTRLGCODE.ToUpper(), MTRLGID
                ).FirstOrDefault();

                bool isUnique = existingRecord == 0;
                
                if (isUnique)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("This material group code is already used.", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                System.Diagnostics.Debug.WriteLine($"Validation Error: {ex.Message}");
                // If there's an error, allow the validation to pass
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
