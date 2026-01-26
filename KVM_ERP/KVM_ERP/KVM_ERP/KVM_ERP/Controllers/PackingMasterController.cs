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
    public class PackingMasterController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PackingMaster
        [Authorize(Roles = "PackingMasterIndex")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: PackingMaster/Form
        [Authorize(Roles = "PackingMasterCreate,PackingMasterEdit")]
        public ActionResult Form(int id = 0)
        {
            PackingMaster tab = new PackingMaster();
            
            if (id == 0)
            {
                // New record
                tab.PACKMID = 0;
                tab.DISPSTATUS = 0; // Default to Enabled
                tab.PACKMNOU = 1; // Default number of units
            }
            else
            {
                // Edit existing record
                tab = db.PackingMasters.Find(id);
                if (tab == null)
                {
                    ViewBag.msg = "<div class='alert alert-danger'>Record not found!</div>";
                    tab = new PackingMaster();
                    tab.DISPSTATUS = 0;
                    tab.PACKMNOU = 1;
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

            // Populate Material Group dropdown (similar pattern as MaterialMaster)
            try
            {
                var materialGroupList = new List<SelectListItem>();

                var materialGroups = db.MaterialGroupMasters
                    .OrderBy(m => m.MTRLGDESC)
                    .ToList();

                foreach (var item in materialGroups)
                {
                    string displayText = item.MTRLGDESC ?? string.Empty;
                    if (item.DISPSTATUS != 0)
                    {
                        displayText += " (Disabled)";
                    }

                    materialGroupList.Add(new SelectListItem
                    {
                        Text = displayText,
                        Value = item.MTRLGID.ToString(),
                        Selected = (tab.MTRLGID.HasValue && tab.MTRLGID.Value == item.MTRLGID)
                    });
                }

                string selectedGroupId = tab.MTRLGID.HasValue && tab.MTRLGID.Value > 0
                    ? tab.MTRLGID.Value.ToString()
                    : string.Empty;

                ViewBag.MTRLGID = new SelectList(materialGroupList, "Value", "Text", selectedGroupId);
            }
            catch
            {
                ViewBag.MTRLGID = new List<SelectListItem>
                {
                    new SelectListItem { Text = "-- Material Group Master not available --", Value = string.Empty }
                };
            }

            return View(tab);
        }

        // POST: PackingMaster/savedata
        [HttpPost]
        [Authorize(Roles = "PackingMasterCreate,PackingMasterEdit")]
        public ActionResult savedata(PackingMaster tab)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check for duplicate code on server side
                    var duplicateCheck = db.Database.SqlQuery<int>(
                        @"SELECT COUNT(*) FROM PACKINGMASTER 
                          WHERE UPPER(PACKMCODE) = @p0 AND PACKMID != @p1",
                        tab.PACKMCODE.ToUpper(), tab.PACKMID
                    ).FirstOrDefault();

                    if (duplicateCheck > 0)
                    {
                        ModelState.AddModelError("PACKMCODE", "This packing code is already used.");
                        ViewBag.msg = "<div class='alert alert-danger'>Packing code already exists. Please use a different code.</div>";
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
                        if (!string.IsNullOrEmpty(tab.PACKMDESC))
                        {
                            tab.PACKMDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(tab.PACKMDESC.ToLower());
                        }

                        // Auto-format code to uppercase
                        if (!string.IsNullOrEmpty(tab.PACKMCODE))
                        {
                            tab.PACKMCODE = tab.PACKMCODE.ToUpper();
                        }

                        if (tab.PACKMID == 0)
                        {
                            // New record - CUSRID gets username, LMUSRID gets user ID (both same user, different formats)
                            System.Diagnostics.Debug.WriteLine($"Creating new record with CUSRID: {currentUserName}, LMUSRID: {currentUserName}");
                            
                            db.Database.ExecuteSqlCommand(
                                @"INSERT INTO PACKINGMASTER (PACKMDESC, PACKMCODE, PACKMNOU, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE, MTRLGID) 
                                  VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7)",
                                tab.PACKMDESC,
                                tab.PACKMCODE,
                                tab.PACKMNOU,
                                currentUserName,
                                currentUserName,
                                tab.DISPSTATUS,
                                prcsdate,
                                (object)tab.MTRLGID ?? DBNull.Value);
                            
                            // Redirect to Index after successful save
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            // Update existing record - preserve original CUSRID, update only LMUSRID with current user
                            System.Diagnostics.Debug.WriteLine($"Updating existing record ID: {tab.PACKMID}, LMUSRID: {currentUserName}");
                            
                            db.Database.ExecuteSqlCommand(
                                @"UPDATE PACKINGMASTER 
                                  SET PACKMDESC = @p0, PACKMCODE = @p1, PACKMNOU = @p2, LMUSRID = @p3, 
                                      DISPSTATUS = @p4, PRCSDATE = @p5, MTRLGID = @p6 
                                  WHERE PACKMID = @p7",
                                tab.PACKMDESC,
                                tab.PACKMCODE,
                                tab.PACKMNOU,
                                currentUserName,
                                tab.DISPSTATUS,
                                prcsdate,
                                (object)tab.MTRLGID ?? DBNull.Value,
                                tab.PACKMID);
                            
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

            // Rebuild Material Group dropdown (for validation errors)
            try
            {
                var materialGroupList = new List<SelectListItem>();

                var materialGroups = db.MaterialGroupMasters
                    .OrderBy(m => m.MTRLGDESC)
                    .ToList();

                foreach (var item in materialGroups)
                {
                    string displayText = item.MTRLGDESC ?? string.Empty;
                    if (item.DISPSTATUS != 0)
                    {
                        displayText += " (Disabled)";
                    }

                    materialGroupList.Add(new SelectListItem
                    {
                        Text = displayText,
                        Value = item.MTRLGID.ToString(),
                        Selected = (tab.MTRLGID.HasValue && tab.MTRLGID.Value == item.MTRLGID)
                    });
                }

                string selectedGroupId = tab.MTRLGID.HasValue && tab.MTRLGID.Value > 0
                    ? tab.MTRLGID.Value.ToString()
                    : string.Empty;

                ViewBag.MTRLGID = new SelectList(materialGroupList, "Value", "Text", selectedGroupId);
            }
            catch
            {
                ViewBag.MTRLGID = new List<SelectListItem>
                {
                    new SelectListItem { Text = "-- Material Group Master not available --", Value = string.Empty }
                };
            }

            return View("Form", tab);
        }

        // GET: PackingMaster/GetAjaxData
        [HttpGet]
        public JsonResult GetAjaxData()
        {
            try
            {
                // Get all data using raw SQL to match database schema
                var packingMasters = db.Database.SqlQuery<PackingMaster>(
                    @"SELECT PACKMID, PACKMDESC, PACKMCODE, PACKMNOU, CUSRID, LMUSRID, 
                             DISPSTATUS, PRCSDATE, MTRLGID 
                      FROM PACKINGMASTER 
                      ORDER BY PACKMCODE"
                ).ToList();

                var result = packingMasters.Select(p => new
                {
                    p.PACKMID,
                    p.PACKMCODE,
                    p.PACKMDESC,
                    p.PACKMNOU,
                    p.DISPSTATUS,
                    MaterialGroup = GetMaterialGroupName(p.MTRLGID)
                }).ToList();

                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the error details for debugging
                System.Diagnostics.Debug.WriteLine($"PackingMaster GetAjaxData Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                // Return error information for debugging
                return Json(new { 
                    data = new List<object>(),
                    error = ex.Message,
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: PackingMaster/Del
        [HttpPost]
        public ActionResult Del(int id)
        {
            try
            {
                // Check if user has delete role
                if (!User.IsInRole("PackingMasterDelete"))
                {
                    return Content("Access Denied: You do not have permission to delete records. Please contact your administrator.");
                }
                
                // Delete using raw SQL
                var rowsAffected = db.Database.ExecuteSqlCommand(
                    "DELETE FROM PACKINGMASTER WHERE PACKMID = @p0", id);
                
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

        // Remote validation for unique packing code
        [HttpPost]
        public JsonResult ValidatePACKMCODE(string PACKMCODE, int PACKMID = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(PACKMCODE))
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }

                // Check if code already exists (excluding current record for edit)
                var existingRecord = db.Database.SqlQuery<int>(
                    @"SELECT COUNT(*) FROM PACKINGMASTER 
                      WHERE UPPER(PACKMCODE) = @p0 AND PACKMID != @p1",
                    PACKMCODE.ToUpper(), PACKMID
                ).FirstOrDefault();

                bool isUnique = existingRecord == 0;
                
                if (isUnique)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("This packing code is already used.", JsonRequestBehavior.AllowGet);
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

        private string GetMaterialGroupName(int? mtrlgid)
        {
            if (!mtrlgid.HasValue || mtrlgid.Value <= 0)
            {
                return string.Empty;
            }

            try
            {
                var result = db.Database.SqlQuery<string>(
                    "SELECT MTRLGDESC FROM MATERIALGROUPMASTER WHERE MTRLGID = @p0",
                    mtrlgid.Value
                ).FirstOrDefault();

                return result ?? string.Empty;
            }
            catch
            {
                return string.Empty;
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
