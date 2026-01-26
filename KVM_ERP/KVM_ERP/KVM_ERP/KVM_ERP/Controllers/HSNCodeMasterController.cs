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
    public class HSNCodeMasterController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HSNCodeMaster
        [Authorize(Roles = "HSNCodeMasterIndex")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: HSNCodeMaster/Form
        [Authorize(Roles = "HSNCodeMasterCreate,HSNCodeMasterEdit")]
        public ActionResult Form(int id = 0)
        {
            // Check permissions based on operation mode
            if (id > 0)
            {
                // Edit mode - requires HSNCodeMasterEdit role
                if (!User.IsInRole("HSNCodeMasterEdit"))
                {
                    return new HttpUnauthorizedResult();
                }
            }
            else
            {
                // Create mode - requires HSNCodeMasterCreate role
                if (!User.IsInRole("HSNCodeMasterCreate"))
                {
                    return new HttpUnauthorizedResult();
                }
            }
            
            HSNCodeMaster tab = new HSNCodeMaster();
            
            if (id == 0)
            {
                // New record
                tab.HSNID = 0;
                tab.DISPSTATUS = 0; // Default to Enabled
                tab.CGSTEXPRN = 0.00m;
                tab.SGSTEXPRN = 0.00m;
                tab.IGSTEXPRN = 0.00m;
                tab.ACGSTEXPRN = 0.00m;
                tab.ASGSTEXPRN = 0.00m;
                tab.AIGSTEXPRN = 0.00m;
                tab.TAXEXPRN = 0.00m;
            }
            else
            {
                // Edit existing record
                tab = db.HSNCodeMasters.Find(id);
                if (tab == null)
                {
                    ViewBag.msg = "<div class='alert alert-danger'>Record not found!</div>";
                    tab = new HSNCodeMaster();
                    tab.DISPSTATUS = 0;
                    tab.CGSTEXPRN = 0.00m;
                    tab.SGSTEXPRN = 0.00m;
                    tab.IGSTEXPRN = 0.00m;
                    tab.ACGSTEXPRN = 0.00m;
                    tab.ASGSTEXPRN = 0.00m;
                    tab.AIGSTEXPRN = 0.00m;
                    tab.TAXEXPRN = 0.00m;
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

        // POST: HSNCodeMaster/savedata
        [HttpPost]
        [Authorize(Roles = "HSNCodeMasterCreate,HSNCodeMasterEdit")]
        public ActionResult savedata(HSNCodeMaster tab)
        {
            try
            {
                // Check permissions based on operation mode
                if (tab.HSNID > 0)
                {
                    // Edit mode - requires HSNCodeMasterEdit role
                    if (!User.IsInRole("HSNCodeMasterEdit"))
                    {
                        TempData["ErrorMessage"] = "Access Denied: You do not have permission to edit records.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    // Create mode - requires HSNCodeMasterCreate role
                    if (!User.IsInRole("HSNCodeMasterCreate"))
                    {
                        TempData["ErrorMessage"] = "Access Denied: You do not have permission to create records.";
                        return RedirectToAction("Index");
                    }
                }
                
                if (ModelState.IsValid)
                {
                    // Check for duplicate code on server side
                    var duplicateCheck = db.Database.SqlQuery<int>(
                        @"SELECT COUNT(*) FROM HSNCODEMASTER 
                          WHERE UPPER(HSNCODE) = @p0 AND HSNID != @p1",
                        tab.HSNCODE.ToUpper(), tab.HSNID
                    ).FirstOrDefault();

                    if (duplicateCheck > 0)
                    {
                        ModelState.AddModelError("HSNCODE", "This HSN code is already used.");
                        ViewBag.msg = "<div class='alert alert-danger'>HSN code already exists. Please use a different code.</div>";
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
                        if (!string.IsNullOrEmpty(tab.HSNDESC))
                        {
                            tab.HSNDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(tab.HSNDESC.ToLower());
                        }

                        // Auto-format code to uppercase
                        if (!string.IsNullOrEmpty(tab.HSNCODE))
                        {
                            tab.HSNCODE = tab.HSNCODE.ToUpper();
                        }

                        // Calculate IGST as sum of CGST and SGST (done on frontend, but ensure it's set)
                        tab.IGSTEXPRN = tab.CGSTEXPRN + tab.SGSTEXPRN;
                        
                        // Set TAXEXPRN to IGSTEXPRN value
                        tab.TAXEXPRN = tab.IGSTEXPRN;
                        
                        // Set default values for additional GST fields
                        tab.ACGSTEXPRN = 0.00m;
                        tab.ASGSTEXPRN = 0.00m;
                        tab.AIGSTEXPRN = 0.00m;

                        if (tab.HSNID == 0)
                        {
                            // New record - CUSRID gets username, LMUSRID gets user ID (both same user, different formats)
                            System.Diagnostics.Debug.WriteLine($"Creating new record with CUSRID: {currentUserName}, LMUSRID: {currentUserName}");
                            
                            db.Database.ExecuteSqlCommand(
                                @"INSERT INTO HSNCODEMASTER (HSNDESC, HSNCODE, CGSTEXPRN, SGSTEXPRN, IGSTEXPRN, 
                                                           ACGSTEXPRN, ASGSTEXPRN, AIGSTEXPRN, TAXEXPRN, 
                                                           CUSRID, LMUSRID, DISPSTATUS, PRCSDATE) 
                                  VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12)",
                                tab.HSNDESC, tab.HSNCODE, tab.CGSTEXPRN, tab.SGSTEXPRN, tab.IGSTEXPRN,
                                tab.ACGSTEXPRN, tab.ASGSTEXPRN, tab.AIGSTEXPRN, tab.TAXEXPRN,
                                currentUserName, currentUserName, tab.DISPSTATUS, prcsdate);
                            
                            // Redirect to Index after successful save
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            // Update existing record - preserve original CUSRID, update only LMUSRID with current user
                            System.Diagnostics.Debug.WriteLine($"Updating existing record ID: {tab.HSNID}, LMUSRID: {currentUserName}");
                            
                            db.Database.ExecuteSqlCommand(
                                @"UPDATE HSNCODEMASTER 
                                  SET HSNDESC = @p0, HSNCODE = @p1, CGSTEXPRN = @p2, SGSTEXPRN = @p3, 
                                      IGSTEXPRN = @p4, ACGSTEXPRN = @p5, ASGSTEXPRN = @p6, AIGSTEXPRN = @p7, 
                                      TAXEXPRN = @p8, LMUSRID = @p9, DISPSTATUS = @p10, PRCSDATE = @p11 
                                  WHERE HSNID = @p12",
                                tab.HSNDESC, tab.HSNCODE, tab.CGSTEXPRN, tab.SGSTEXPRN, tab.IGSTEXPRN,
                                tab.ACGSTEXPRN, tab.ASGSTEXPRN, tab.AIGSTEXPRN, tab.TAXEXPRN,
                                currentUserName, tab.DISPSTATUS, prcsdate, tab.HSNID);
                            
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

        // GET: HSNCodeMaster/GetAjaxData
        [HttpGet]
        public JsonResult GetAjaxData()
        {
            try
            {
                // Get all data using raw SQL to match database schema
                var hsnCodes = db.Database.SqlQuery<HSNCodeMaster>(
                    @"SELECT HSNID, HSNDESC, HSNCODE, CGSTEXPRN, SGSTEXPRN, IGSTEXPRN, 
                             ACGSTEXPRN, ASGSTEXPRN, AIGSTEXPRN, TAXEXPRN, 
                             CUSRID, LMUSRID, DISPSTATUS, PRCSDATE 
                      FROM HSNCODEMASTER 
                      ORDER BY HSNCODE"
                ).ToList();

                var result = hsnCodes.Select(m => new
                {
                    m.HSNID,
                    m.HSNCODE,
                    m.HSNDESC,
                    m.CGSTEXPRN,
                    m.SGSTEXPRN,
                    m.IGSTEXPRN,
                    m.DISPSTATUS
                }).ToList();

                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the error details for debugging
                System.Diagnostics.Debug.WriteLine($"HSNCodeMaster GetAjaxData Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                // Return error information for debugging
                return Json(new { 
                    data = new List<object>(),
                    error = ex.Message,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: HSNCodeMaster/Del
        [HttpPost]
        public ActionResult Del(int id)
        {
            try
            {
                // Check if user has delete role
                if (!User.IsInRole("HSNCodeMasterDelete"))
                {
                    return Content("Access Denied: You do not have permission to delete records. Please contact your administrator.");
                }
                
                // Delete using raw SQL
                var rowsAffected = db.Database.ExecuteSqlCommand(
                    "DELETE FROM HSNCODEMASTER WHERE HSNID = @p0", id);
                
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

        // Remote validation for unique HSN code
        [HttpPost]
        public JsonResult ValidateHSNCODE(string HSNCODE, int HSNID = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(HSNCODE))
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }

                // Check if code already exists (excluding current record for edit)
                var existingRecord = db.Database.SqlQuery<int>(
                    @"SELECT COUNT(*) FROM HSNCODEMASTER 
                      WHERE UPPER(HSNCODE) = @p0 AND HSNID != @p1",
                    HSNCODE.ToUpper(), HSNID
                ).FirstOrDefault();

                bool isUnique = existingRecord == 0;
                
                if (isUnique)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("This HSN code is already used.", JsonRequestBehavior.AllowGet);
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
