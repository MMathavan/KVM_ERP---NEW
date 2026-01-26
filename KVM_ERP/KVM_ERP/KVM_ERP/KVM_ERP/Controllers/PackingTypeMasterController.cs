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
    public class PackingTypeMasterController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PackingTypeMaster
        [Authorize(Roles = "PackingTypeMasterIndex")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: PackingTypeMaster/Form
        [Authorize(Roles = "PackingTypeMasterCreate,PackingTypeMasterEdit")]
        public ActionResult Form(int id = 0)
        {
            PackingTypeMaster tab = new PackingTypeMaster();
            
            if (id == 0)
            {
                // New record
                tab.PACKTMID = 0;
                tab.DISPSTATUS = 0; // Default to Enabled
            }
            else
            {
                // Edit existing record
                tab = db.PackingTypeMasters.Find(id);
                if (tab == null)
                {
                    ViewBag.msg = "<div class='alert alert-danger'>Record not found!</div>";
                    tab = new PackingTypeMaster();
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

            // Populate Packing Master dropdown (only enabled items)
            var packingMasterList = db.PackingMasters
                .Where(p => p.DISPSTATUS == 0)
                .OrderBy(p => p.PACKMDESC)
                .Select(p => new { p.PACKMID, p.PACKMDESC })
                .ToList();

            var packingMasterDropdown = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Select Packing Master --" }
            };

            foreach (var item in packingMasterList)
            {
                packingMasterDropdown.Add(new SelectListItem
                {
                    Value = item.PACKMID.ToString(),
                    Text = item.PACKMDESC
                });
            }

            // Set selected value for edit mode
            var selectedPackingId = "";
            if (id != null && id > 0 && tab.PACKMID > 0)
            {
                selectedPackingId = tab.PACKMID.ToString();
            }

            ViewBag.PACKMID = new SelectList(packingMasterDropdown, "Value", "Text", selectedPackingId);

            return View(tab);
        }

        // POST: PackingTypeMaster/savedata
        [HttpPost]
        [Authorize(Roles = "PackingTypeMasterCreate,PackingTypeMasterEdit")]
        public ActionResult savedata(PackingTypeMaster tab)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check for duplicate code on server side
                    var duplicateCheck = db.Database.SqlQuery<int>(
                        @"SELECT COUNT(*) FROM PACKINGTYPEMASTER 
                          WHERE UPPER(PACKTMCODE) = @p0 AND PACKTMID != @p1",
                        tab.PACKTMCODE.ToUpper(), tab.PACKTMID
                    ).FirstOrDefault();

                    if (duplicateCheck > 0)
                    {
                        ModelState.AddModelError("PACKTMCODE", "This packing type code is already used.");
                        ViewBag.msg = "<div class='alert alert-danger'>Packing type code already exists. Please use a different code.</div>";
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
                        if (!string.IsNullOrEmpty(tab.PACKTMDESC))
                        {
                            tab.PACKTMDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(tab.PACKTMDESC.ToLower());
                        }

                        // Auto-format code to uppercase
                        if (!string.IsNullOrEmpty(tab.PACKTMCODE))
                        {
                            tab.PACKTMCODE = tab.PACKTMCODE.ToUpper();
                        }

                        if (tab.PACKTMID == 0)
                        {
                            // New record - CUSRID gets username, LMUSRID gets user ID (both same user, different formats)
                            System.Diagnostics.Debug.WriteLine($"Creating new record with CUSRID: {currentUserName}, LMUSRID: {currentUserName}");
                            
                            db.Database.ExecuteSqlCommand(
                                @"INSERT INTO PACKINGTYPEMASTER (PACKTMCODE, PACKTMDESC, PACKMID, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE) 
                                  VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6)",
                                tab.PACKTMCODE, tab.PACKTMDESC, tab.PACKMID, currentUserName, currentUserName, tab.DISPSTATUS, prcsdate);
                            
                            // Redirect to Index after successful save
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            // Update existing record - preserve original CUSRID, update only LMUSRID with current user
                            System.Diagnostics.Debug.WriteLine($"Updating existing record ID: {tab.PACKTMID}, LMUSRID: {currentUserName}");
                            
                            db.Database.ExecuteSqlCommand(
                                @"UPDATE PACKINGTYPEMASTER 
                                  SET PACKTMCODE = @p0, PACKTMDESC = @p1, PACKMID = @p2, LMUSRID = @p3, 
                                      DISPSTATUS = @p4, PRCSDATE = @p5 
                                  WHERE PACKTMID = @p6",
                                tab.PACKTMCODE, tab.PACKTMDESC, tab.PACKMID, currentUserName, tab.DISPSTATUS, prcsdate, tab.PACKTMID);
                            
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

            // Rebuild dropdowns for validation errors
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Enabled" },
                new SelectListItem { Value = "1", Text = "Disabled" }
            };

            ViewBag.DISPSTATUS = new SelectList(statusList, "Value", "Text", tab.DISPSTATUS.ToString());

            // Rebuild Packing Master dropdown
            var packingMasterList = db.PackingMasters
                .Where(p => p.DISPSTATUS == 0)
                .OrderBy(p => p.PACKMDESC)
                .Select(p => new { p.PACKMID, p.PACKMDESC })
                .ToList();

            var packingMasterDropdown = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Select Packing Master --" }
            };

            foreach (var item in packingMasterList)
            {
                packingMasterDropdown.Add(new SelectListItem
                {
                    Value = item.PACKMID.ToString(),
                    Text = item.PACKMDESC
                });
            }

            ViewBag.PACKMID = new SelectList(packingMasterDropdown, "Value", "Text", tab.PACKMID.ToString());

            return View("Form", tab);
        }

        // GET: PackingTypeMaster/GetAjaxData
        [HttpGet]
        public JsonResult GetAjaxData()
        {
            try
            {
                // Get all data using Entity Framework LINQ with join to get packing master name
                var result = (from pt in db.PackingTypeMasters
                             join pm in db.PackingMasters on pt.PACKMID equals pm.PACKMID into pmGroup
                             from pm in pmGroup.DefaultIfEmpty()
                             orderby pt.PACKTMCODE
                             select new
                             {
                                 pt.PACKTMID,
                                 pt.PACKTMCODE,
                                 pt.PACKTMDESC,
                                 pt.PACKMID,
                                 PackingMasterName = pm != null ? pm.PACKMDESC : "",
                                 pt.DISPSTATUS
                             }).ToList();

                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the error details for debugging
                System.Diagnostics.Debug.WriteLine($"PackingTypeMaster GetAjaxData Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                // Return error information for debugging
                return Json(new { 
                    data = new List<object>(),
                    error = ex.Message,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: PackingTypeMaster/Del
        [HttpPost]
        public ActionResult Del(int id)
        {
            try
            {
                // Check if user has delete role
                if (!User.IsInRole("PackingTypeMasterDelete"))
                {
                    return Content("Access Denied: You do not have permission to delete records. Please contact your administrator.");
                }
                
                // Delete using raw SQL
                var rowsAffected = db.Database.ExecuteSqlCommand(
                    "DELETE FROM PACKINGTYPEMASTER WHERE PACKTMID = @p0", id);
                
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

        // Remote validation for unique packing type code
        [HttpPost]
        public JsonResult ValidatePACKTMCODE(string PACKTMCODE, int PACKTMID = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(PACKTMCODE))
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }

                // Check if code already exists (excluding current record for edit)
                var existingRecord = db.Database.SqlQuery<int>(
                    @"SELECT COUNT(*) FROM PACKINGTYPEMASTER 
                      WHERE UPPER(PACKTMCODE) = @p0 AND PACKTMID != @p1",
                    PACKTMCODE.ToUpper(), PACKTMID
                ).FirstOrDefault();

                bool isUnique = existingRecord == 0;
                
                if (isUnique)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("This packing type code is already used.", JsonRequestBehavior.AllowGet);
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
