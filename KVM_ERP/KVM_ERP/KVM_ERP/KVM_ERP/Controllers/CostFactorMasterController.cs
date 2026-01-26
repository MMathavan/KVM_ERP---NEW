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
    public class CostFactorMasterController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CostFactorMaster
        [Authorize(Roles = "CostFactorMasterIndex")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: CostFactorMaster/Form
        [Authorize(Roles = "CostFactorMasterCreate,CostFactorMasterEdit")]
        public ActionResult Form(int id = 0)
        {
            // Check permissions based on operation mode
            if (id > 0)
            {
                // Edit mode - requires CostFactorMasterEdit role
                if (!User.IsInRole("CostFactorMasterEdit"))
                {
                    return new HttpUnauthorizedResult();
                }
            }
            else
            {
                // Create mode - requires CostFactorMasterCreate role
                if (!User.IsInRole("CostFactorMasterCreate"))
                {
                    return new HttpUnauthorizedResult();
                }
            }
            
            CostFactorMaster tab = new CostFactorMaster();
            
            if (id == 0)
            {
                // New record
                tab.CFID = 0;
                tab.DISPSTATUS = 0; // Default to Enabled
                tab.TRANTID = 0;
                tab.ACHEADID = 0;
                tab.CFEXPR = 0.000m;
            }
            else
            {
                // Edit existing record
                tab = db.CostFactorMasters.Find(id);
                if (tab == null)
                {
                    ViewBag.msg = "<div class='alert alert-danger'>Record not found!</div>";
                    tab = new CostFactorMaster();
                    tab.DISPSTATUS = 0;
                    tab.TRANTID = 0;
                    tab.ACHEADID = 0;
                    tab.CFEXPR = 0.000m;
                }
            }

            // Populate all dropdowns
            PopulateDropdowns(tab);

            return View(tab);
        }

        private void PopulateDropdowns(CostFactorMaster model)
        {
            // Mode dropdown
            var modeList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Please Select", Selected = string.IsNullOrEmpty(model.CFMODE) },
                new SelectListItem { Value = "0", Text = "ADD" },
                new SelectListItem { Value = "1", Text = "DEDUCT" }
            };
            ViewBag.CFMODE = new SelectList(modeList, "Value", "Text", model.CFMODE);

            // Type dropdown (As)
            var typeList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "%" },
                new SelectListItem { Value = "1", Text = "Value" }
            };
            ViewBag.CFTYPE = new SelectList(typeList, "Value", "Text", model.CFTYPE.ToString());

            // Nature dropdown (On)
            var natureList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Exclusive" },
                new SelectListItem { Value = "1", Text = "Inclusive" }
            };
            ViewBag.CFNATR = new SelectList(natureList, "Value", "Text", model.CFNATR.ToString());

            // Calculated On dropdown
            var calculatedOnList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Amount" },
                new SelectListItem { Value = "1", Text = "Tax" },
                new SelectListItem { Value = "2", Text = "Excise" }
            };
            ViewBag.CFOPTN = new SelectList(calculatedOnList, "Value", "Text", model.CFOPTN.ToString());

            // Belongs to dropdown
            var belongsToList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Discount" },
                new SelectListItem { Value = "1", Text = "Packing" },
                new SelectListItem { Value = "2", Text = "Excise Duty" },
                new SelectListItem { Value = "3", Text = "Primary Cess" },
                new SelectListItem { Value = "4", Text = "Secondary Cess" },
                new SelectListItem { Value = "5", Text = "Sales Tax" },
                new SelectListItem { Value = "6", Text = "Others" },
                new SelectListItem { Value = "7", Text = "Rounding Off" },
                new SelectListItem { Value = "8", Text = "Service Charge" }
            };
            ViewBag.DORDRID = new SelectList(belongsToList, "Value", "Text", model.DORDRID.ToString());

            // Status dropdown
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Enabled" },
                new SelectListItem { Value = "1", Text = "Disabled" }
            };
            ViewBag.DISPSTATUS = new SelectList(statusList, "Value", "Text", model.DISPSTATUS.ToString());
        }

        // POST: CostFactorMaster/savedata
        [HttpPost]
        [Authorize(Roles = "CostFactorMasterCreate,CostFactorMasterEdit")]
        public ActionResult savedata(CostFactorMaster tab)
        {
            try
            {
                // Check permissions based on operation mode
                if (tab.CFID > 0)
                {
                    // Edit mode - requires CostFactorMasterEdit role
                    if (!User.IsInRole("CostFactorMasterEdit"))
                    {
                        TempData["ErrorMessage"] = "Access Denied: You do not have permission to edit records.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    // Create mode - requires CostFactorMasterCreate role
                    if (!User.IsInRole("CostFactorMasterCreate"))
                    {
                        TempData["ErrorMessage"] = "Access Denied: You do not have permission to create records.";
                        return RedirectToAction("Index");
                    }
                }
                
                if (ModelState.IsValid)
                {
                    // Get current user information from session
                    var currentUserName = Session["CUSRID"]?.ToString();
                    
                    // If session doesn't have CUSRID, try to get from User.Identity
                    if (string.IsNullOrEmpty(currentUserName))
                    {
                        currentUserName = User.Identity.Name ?? "admin";
                    }
                    
                    // Debug: Log session values
                    System.Diagnostics.Debug.WriteLine($"Session CUSRID: {Session["CUSRID"]}");
                    System.Diagnostics.Debug.WriteLine($"Final currentUserName: {currentUserName}");
                    var prcsdate = DateTime.Now;

                    // Auto-format description to title case
                    if (!string.IsNullOrEmpty(tab.CFDESC))
                    {
                        tab.CFDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(tab.CFDESC.ToLower());
                    }

                    // Set default values
                    tab.TRANTID = 0;
                    tab.ACHEADID = 0;

                    if (tab.CFID == 0)
                    {
                        // New record - CUSRID gets username, LMUSRID gets user ID (both same user, different formats)
                        System.Diagnostics.Debug.WriteLine($"Creating new record with CUSRID: {currentUserName}, LMUSRID: {currentUserName}");
                        
                        db.Database.ExecuteSqlCommand(
                            @"INSERT INTO COSTFACTORMASTER (TRANTID, CFDESC, CFMODE, CFTYPE, CFEXPR, CFNATR, CFOPTN, 
                                                          DORDRID, DISPSTATUS, PRCSDATE, ACHEADID, LMUSRID, CUSRID) 
                              VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12)",
                            tab.TRANTID, tab.CFDESC, tab.CFMODE, tab.CFTYPE, tab.CFEXPR, tab.CFNATR, tab.CFOPTN,
                            tab.DORDRID, tab.DISPSTATUS, prcsdate, tab.ACHEADID, currentUserName, currentUserName);
                        
                        // Redirect to Index after successful save
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Update existing record - preserve original CUSRID, update only LMUSRID with current user
                        System.Diagnostics.Debug.WriteLine($"Updating existing record ID: {tab.CFID}, LMUSRID: {currentUserName}");
                        
                        db.Database.ExecuteSqlCommand(
                            @"UPDATE COSTFACTORMASTER 
                              SET TRANTID = @p0, CFDESC = @p1, CFMODE = @p2, CFTYPE = @p3, CFEXPR = @p4, 
                                  CFNATR = @p5, CFOPTN = @p6, DORDRID = @p7, DISPSTATUS = @p8, 
                                  PRCSDATE = @p9, ACHEADID = @p10, LMUSRID = @p11 
                              WHERE CFID = @p12",
                            tab.TRANTID, tab.CFDESC, tab.CFMODE, tab.CFTYPE, tab.CFEXPR, tab.CFNATR, tab.CFOPTN,
                            tab.DORDRID, tab.DISPSTATUS, prcsdate, tab.ACHEADID, currentUserName, tab.CFID);
                        
                        // Redirect to Index after successful update
                        return RedirectToAction("Index");
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

            // Rebuild dropdowns for validation errors
            PopulateDropdowns(tab);

            return View("Form", tab);
        }

        // GET: CostFactorMaster/TestData - Simple test method
        [HttpGet]
        public JsonResult TestData()
        {
            try
            {
                // Simple test query to verify database connection
                var testQuery = db.Database.SqlQuery<dynamic>("SELECT COUNT(*) as RecordCount FROM COSTFACTORMASTER").FirstOrDefault();
                var recordCount = testQuery?.RecordCount ?? 0;
                
                System.Diagnostics.Debug.WriteLine($"CostFactorMaster TestData: Found {recordCount} records in database");
                
                return Json(new { 
                    success = true, 
                    recordCount = recordCount,
                    message = $"Database connection successful. Found {recordCount} records."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CostFactorMaster TestData Error: {ex.Message}");
                return Json(new { 
                    success = false, 
                    error = ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: CostFactorMaster/GetAjaxData
        [HttpGet]
        public JsonResult GetAjaxData()
        {
            try
            {
                // First try Entity Framework approach
                System.Diagnostics.Debug.WriteLine("CostFactorMaster GetAjaxData: Starting data retrieval...");
                
                var costFactors = db.CostFactorMasters.OrderBy(c => c.CFDESC).ToList();
                System.Diagnostics.Debug.WriteLine($"CostFactorMaster GetAjaxData: Retrieved {costFactors.Count} records using EF");

                // Convert to display format
                var result = costFactors.Select(m => new
                {
                    CFID = m.CFID,
                    CFDESC = m.CFDESC ?? "",
                    CFMODEText = GetModeText(m.CFMODE),
                    CFTYPEText = GetTypeText(m.CFTYPE),
                    CFEXPR = m.CFEXPR,
                    CFNATRText = GetNatureText(m.CFNATR),
                    CFOPTNText = GetCalculatedOnText(m.CFOPTN),
                    DORDRIDText = GetBelongsToText(m.DORDRID),
                    DISPSTATUS = m.DISPSTATUS
                }).ToList();

                System.Diagnostics.Debug.WriteLine($"CostFactorMaster GetAjaxData: Returning {result.Count} processed records");
                
                if (result.Count > 0)
                {
                    var firstRecord = result.First();
                    System.Diagnostics.Debug.WriteLine($"First record - CFID: {firstRecord.CFID}, CFDESC: {firstRecord.CFDESC}, Mode: {firstRecord.CFMODEText}");
                }

                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the error details for debugging
                System.Diagnostics.Debug.WriteLine($"CostFactorMaster GetAjaxData Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                // Return error information for debugging
                return Json(new { 
                    data = new List<object>(),
                    error = ex.Message,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: CostFactorMaster/Del
        [HttpPost]
        public ActionResult Del(int id)
        {
            try
            {
                // Check if user has delete role
                if (!User.IsInRole("CostFactorMasterDelete"))
                {
                    return Content("Access Denied: You do not have permission to delete records. Please contact your administrator.");
                }
                
                // Delete using raw SQL
                var rowsAffected = db.Database.ExecuteSqlCommand(
                    "DELETE FROM COSTFACTORMASTER WHERE CFID = @p0", id);
                
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

        // Helper methods for converting dropdown values to text
        private string GetModeText(string cfmode)
        {
            switch (cfmode)
            {
                case "0": return "ADD";
                case "1": return "DEDUCT";
                default: return cfmode ?? "Unknown";
            }
        }

        private string GetTypeText(short cftype)
        {
            switch (cftype)
            {
                case 0: return "%";
                case 1: return "Value";
                default: return "Unknown";
            }
        }

        private string GetNatureText(short cfnatr)
        {
            switch (cfnatr)
            {
                case 0: return "Exclusive";
                case 1: return "Inclusive";
                default: return "Unknown";
            }
        }

        private string GetCalculatedOnText(short cfoptn)
        {
            switch (cfoptn)
            {
                case 0: return "Amount";
                case 1: return "Tax";
                case 2: return "Excise";
                default: return "Unknown";
            }
        }

        private string GetBelongsToText(short dordrid)
        {
            switch (dordrid)
            {
                case 0: return "Discount";
                case 1: return "Packing";
                case 2: return "Excise Duty";
                case 3: return "Primary Cess";
                case 4: return "Secondary Cess";
                case 5: return "Sales Tax";
                case 6: return "Others";
                case 7: return "Rounding Off";
                case 8: return "Service Charge";
                default: return "Unknown";
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
