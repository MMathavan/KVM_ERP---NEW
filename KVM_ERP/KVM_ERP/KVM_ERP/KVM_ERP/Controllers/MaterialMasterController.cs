using KVM_ERP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KVM_ERP.Controllers
{
    [SessionExpire]
    public class MaterialMasterController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MaterialMaster
        [Authorize(Roles = "MaterialMasterIndex")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: MaterialMaster/Form
        [Authorize(Roles = "MaterialMasterCreate,MaterialMasterEdit")]
        public ActionResult Form(int? id)
        {
            MaterialMaster tab = new MaterialMaster();
            
            if (id != null && id > 0)
            {
                // Edit mode - get existing record
                tab = db.Database.SqlQuery<MaterialMaster>("SELECT * FROM MATERIALMASTER WHERE MTRLID = @p0", id).FirstOrDefault();
                if (tab == null)
                {
                    ViewBag.msg = "<div class='alert alert-danger'>Material not found!</div>";
                    return View(new MaterialMaster());
                }
            }
            else
            {
                // Add mode - set default values
                tab.DISPSTATUS = 0; // Enabled by default
                tab.DISPORDER = 1;
                tab.ACHEADID = 0;
                tab.SCHLID = 0;
                tab.ROLNQTY = 0;
                tab.ROLXQTY = 0;
                tab.EOQTY = 0;
                tab.MTRLPRFT = 0;
                tab.MTRLBQTY = 0;
                tab.MTRLESTATUS = 0;
                tab.MTRLBSTATUS = 0;
                tab.MTRLASTATUS = 0;
                tab.MTRLRTYPE = 0;
                tab.MTRLBRATE = 0;
                tab.MTRLCATEID = 0;
                tab.PACKMID = 0;
            }

            // Populate dropdowns
            PopulateDropdowns(tab, id);

            return View(tab);
        }

        private void PopulateDropdowns(MaterialMaster tab, int? id)
        {
            // Debug logging
            System.Diagnostics.Debug.WriteLine($"PopulateDropdowns called with ID: {id}");
            if (tab != null)
            {
                System.Diagnostics.Debug.WriteLine($"Material values - MTRLGID: {tab.MTRLGID}, MTRLTID: {tab.MTRLTID}, UNITID: {tab.UNITID}, HSNID: {tab.HSNID}, DISPSTATUS: {tab.DISPSTATUS}");
            }

            // Initialize empty dropdowns first to prevent ViewBag errors - using SelectList like PackingTypeMaster
            ViewBag.MTRLGID = new SelectList(new List<SelectListItem>(), "Value", "Text");
            ViewBag.MTRLTID = new SelectList(new List<SelectListItem>(), "Value", "Text");
            ViewBag.UNITID = new SelectList(new List<SelectListItem>(), "Value", "Text");
            ViewBag.HSNID = new SelectList(new List<SelectListItem>(), "Value", "Text");
            
            // Status dropdown using SelectList like PackingTypeMaster
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Enabled", Value = "0" },
                new SelectListItem { Text = "Disabled", Value = "1" }
            };
            ViewBag.DISPSTATUS = new SelectList(statusList, "Value", "Text", tab.DISPSTATUS.ToString());

            try
            {
                // Material Group dropdown (only enabled items) - Use Entity Framework if available
                try
                {
                    var materialGroupList = new List<SelectListItem> { new SelectListItem { Text = "-- Select Material Group --", Value = "" } };
                    
                    if (db.MaterialGroupMasters != null)
                    {
                        // Get enabled items
                        var materialGroups = db.MaterialGroupMasters.Where(m => m.DISPSTATUS == 0).OrderBy(m => m.MTRLGDESC).ToList();
                        System.Diagnostics.Debug.WriteLine($"Found {materialGroups.Count} enabled material groups");
                        
                        // If in edit mode and current selection is disabled, add it to the list
                        if (id != null && id > 0 && tab.MTRLGID > 0)
                        {
                            var currentSelection = db.MaterialGroupMasters.FirstOrDefault(m => m.MTRLGID == tab.MTRLGID);
                            if (currentSelection != null && currentSelection.DISPSTATUS != 0 && !materialGroups.Any(m => m.MTRLGID == tab.MTRLGID))
                            {
                                materialGroups.Add(currentSelection);
                                System.Diagnostics.Debug.WriteLine($"Added disabled current selection: {currentSelection.MTRLGID} ({currentSelection.MTRLGDESC})");
                            }
                        }
                        
                        foreach (var item in materialGroups)
                        {
                            bool isSelected = id != null && id > 0 && tab.MTRLGID == item.MTRLGID;
                            string displayText = item.MTRLGDESC ?? "";
                            if (item.DISPSTATUS != 0) displayText += " (Disabled)";
                            
                            System.Diagnostics.Debug.WriteLine($"Group {item.MTRLGID} ({item.MTRLGDESC}) - Selected: {isSelected}");
                            
                            materialGroupList.Add(new SelectListItem
                            {
                                Text = displayText,
                                Value = item.MTRLGID.ToString(),
                                Selected = isSelected
                            });
                        }
                    }
                    // Use SelectList with selected value like PackingTypeMaster
                    string selectedGroupId = (id != null && id > 0 && tab.MTRLGID > 0) ? tab.MTRLGID.ToString() : "";
                    ViewBag.MTRLGID = new SelectList(materialGroupList, "Value", "Text", selectedGroupId);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Material Group error: {ex.Message}");
                    // Fallback to basic dropdown if MaterialGroupMasters not available
                    ViewBag.MTRLGID = new List<SelectListItem> { new SelectListItem { Text = "-- Material Group Master not available --", Value = "" } };
                }

                // Material Type dropdown (only enabled items)
                try
                {
                    var materialTypeList = new List<SelectListItem> { new SelectListItem { Text = "-- Select Material Type --", Value = "" } };
                    
                    if (db.MaterialTypeMasters != null)
                    {
                        // Get enabled items
                        var materialTypes = db.MaterialTypeMasters.Where(m => m.DISPSTATUS == 0).OrderBy(m => m.MTRLTDESC).ToList();
                        System.Diagnostics.Debug.WriteLine($"Found {materialTypes.Count} enabled material types");
                        
                        // If in edit mode and current selection is disabled, add it to the list
                        if (id != null && id > 0 && tab.MTRLTID > 0)
                        {
                            var currentSelection = db.MaterialTypeMasters.FirstOrDefault(m => m.MTRLTID == tab.MTRLTID);
                            if (currentSelection != null && currentSelection.DISPSTATUS != 0 && !materialTypes.Any(m => m.MTRLTID == tab.MTRLTID))
                            {
                                materialTypes.Add(currentSelection);
                                System.Diagnostics.Debug.WriteLine($"Added disabled current selection: {currentSelection.MTRLTID} ({currentSelection.MTRLTDESC})");
                            }
                        }
                        
                        foreach (var item in materialTypes)
                        {
                            bool isSelected = id != null && id > 0 && tab.MTRLTID == item.MTRLTID;
                            string displayText = item.MTRLTDESC ?? "";
                            if (item.DISPSTATUS != 0) displayText += " (Disabled)";
                            
                            System.Diagnostics.Debug.WriteLine($"Type {item.MTRLTID} ({item.MTRLTDESC}) - Selected: {isSelected}");
                            
                            materialTypeList.Add(new SelectListItem
                            {
                                Text = displayText,
                                Value = item.MTRLTID.ToString(),
                                Selected = isSelected
                            });
                        }
                    }
                    // Use SelectList with selected value like PackingTypeMaster
                    string selectedTypeId = (id != null && id > 0 && tab.MTRLTID > 0) ? tab.MTRLTID.ToString() : "";
                    ViewBag.MTRLTID = new SelectList(materialTypeList, "Value", "Text", selectedTypeId);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Material Type error: {ex.Message}");
                    // Fallback to basic dropdown if MaterialTypeMasters not available
                    ViewBag.MTRLTID = new List<SelectListItem> { new SelectListItem { Text = "-- Material Type Master not available --", Value = "" } };
                }

                // Unit dropdown (only enabled items)
                try
                {
                    var unitList = new List<SelectListItem> { new SelectListItem { Text = "-- Select Unit --", Value = "" } };
                    
                    if (db.UnitMasters != null)
                    {
                        // Get enabled items
                        var units = db.UnitMasters.Where(u => u.DISPSTATUS == 0).OrderBy(u => u.UNITDESC).ToList();
                        System.Diagnostics.Debug.WriteLine($"Found {units.Count} enabled units");
                        
                        // If in edit mode and current selection is disabled, add it to the list
                        if (id != null && id > 0 && tab.UNITID > 0)
                        {
                            var currentSelection = db.UnitMasters.FirstOrDefault(u => u.UNITID == tab.UNITID);
                            if (currentSelection != null && currentSelection.DISPSTATUS != 0 && !units.Any(u => u.UNITID == tab.UNITID))
                            {
                                units.Add(currentSelection);
                                System.Diagnostics.Debug.WriteLine($"Added disabled current selection: {currentSelection.UNITID} ({currentSelection.UNITDESC})");
                            }
                        }
                        
                        foreach (var item in units)
                        {
                            bool isSelected = id != null && id > 0 && tab.UNITID == item.UNITID;
                            string displayText = item.UNITDESC ?? "";
                            if (item.DISPSTATUS != 0) displayText += " (Disabled)";
                            
                            System.Diagnostics.Debug.WriteLine($"Unit {item.UNITID} ({item.UNITDESC}) - Selected: {isSelected}");
                            
                            unitList.Add(new SelectListItem
                            {
                                Text = displayText,
                                Value = item.UNITID.ToString(),
                                Selected = isSelected
                            });
                        }
                    }
                    // Use SelectList with selected value like PackingTypeMaster
                    string selectedUnitId = (id != null && id > 0 && tab.UNITID > 0) ? tab.UNITID.ToString() : "";
                    ViewBag.UNITID = new SelectList(unitList, "Value", "Text", selectedUnitId);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Unit error: {ex.Message}");
                    // Fallback to basic dropdown if UnitMasters not available
                    ViewBag.UNITID = new List<SelectListItem> { new SelectListItem { Text = "-- Unit Master not available --", Value = "" } };
                }

                // HSN Code dropdown (only enabled items)
                try
                {
                    var hsnCodeList = new List<SelectListItem> { new SelectListItem { Text = "-- Select HSN Code --", Value = "" } };
                    
                    if (db.HSNCodeMasters != null)
                    {
                        // Get enabled items
                        var hsnCodes = db.HSNCodeMasters.Where(h => h.DISPSTATUS == 0).OrderBy(h => h.HSNDESC).ToList();
                        System.Diagnostics.Debug.WriteLine($"Found {hsnCodes.Count} enabled HSN codes");
                        
                        // If in edit mode and current selection is disabled, add it to the list
                        if (id != null && id > 0 && tab.HSNID > 0)
                        {
                            var currentSelection = db.HSNCodeMasters.FirstOrDefault(h => h.HSNID == tab.HSNID);
                            if (currentSelection != null && currentSelection.DISPSTATUS != 0 && !hsnCodes.Any(h => h.HSNID == tab.HSNID))
                            {
                                hsnCodes.Add(currentSelection);
                                System.Diagnostics.Debug.WriteLine($"Added disabled current selection: {currentSelection.HSNID} ({currentSelection.HSNDESC})");
                            }
                        }
                        
                        foreach (var item in hsnCodes)
                        {
                            bool isSelected = id != null && id > 0 && tab.HSNID == item.HSNID;
                            string displayText = item.HSNDESC ?? "";
                            if (item.DISPSTATUS != 0) displayText += " (Disabled)";
                            
                            System.Diagnostics.Debug.WriteLine($"HSN {item.HSNID} ({item.HSNDESC}) - Selected: {isSelected}");
                            
                            hsnCodeList.Add(new SelectListItem
                            {
                                Text = displayText,
                                Value = item.HSNID.ToString(),
                                Selected = isSelected
                            });
                        }
                    }
                    // Use SelectList with selected value like PackingTypeMaster
                    string selectedHsnId = (id != null && id > 0 && tab.HSNID > 0) ? tab.HSNID.ToString() : "";
                    ViewBag.HSNID = new SelectList(hsnCodeList, "Value", "Text", selectedHsnId);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"HSN Code error: {ex.Message}");
                    // Fallback to basic dropdown if HSNCodeMasters not available
                    ViewBag.HSNID = new List<SelectListItem> { new SelectListItem { Text = "-- HSN Code Master not available --", Value = "" } };
                }

                // Status dropdown already set above using SelectList
                System.Diagnostics.Debug.WriteLine($"Status dropdown - Selected: {(tab.DISPSTATUS == 0 ? "Enabled" : "Disabled")}");
            }
            catch (Exception ex)
            {
                ViewBag.msg = $"<div class='alert alert-danger'>Error loading dropdowns: {ex.Message}</div>";
                // ViewBag items are already initialized above, so the form will still work
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "MaterialMasterCreate,MaterialMasterEdit")]
        public ActionResult savedata(MaterialMaster tab)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check for duplicate code on server side (case-insensitive like PackingTypeMaster)
                    var duplicateCheck = db.Database.SqlQuery<int>(
                        @"SELECT COUNT(*) FROM MATERIALMASTER 
                          WHERE UPPER(MTRLCODE) = @p0 AND MTRLID != @p1",
                        tab.MTRLCODE.ToUpper(), tab.MTRLID
                    ).FirstOrDefault();

                    if (duplicateCheck > 0)
                    {
                        ModelState.AddModelError("MTRLCODE", "This material code is already used.");
                        ViewBag.msg = "<div class='alert alert-danger'>Material code already exists. Please use a different code.</div>";
                    }
                    else
                    {
                        // Get current user (you may need to adjust this based on your authentication)
                        string currentUser = User.Identity.Name ?? "System";

                    if (tab.MTRLID > 0)
                    {
                        // Update existing record
                        var existingRecord = db.Database.SqlQuery<MaterialMaster>(
                            "SELECT * FROM MATERIALMASTER WHERE MTRLID = @p0", tab.MTRLID).FirstOrDefault();

                        if (existingRecord != null)
                        {
                            // Preserve CUSRID, update LMUSRID
                            tab.CUSRID = existingRecord.CUSRID;
                            tab.LMUSRID = currentUser;
                            tab.PRCSDATE = DateTime.Now;

                            // Update using raw SQL to maintain data integrity
                            db.Database.ExecuteSqlCommand(@"
                                UPDATE MATERIALMASTER SET 
                                    MTRLCODE = @p0, MTRLDESC = @p1, MTRLGID = @p2, MTRLTID = @p3, 
                                    UNITID = @p4, HSNID = @p5, ACHEADID = @p6, SCHLID = @p7, 
                                    ROLNQTY = @p8, ROLXQTY = @p9, EOQTY = @p10, MTRLPRFT = @p11, 
                                    MTRLBQTY = @p12, MTRLESTATUS = @p13, MTRLBSTATUS = @p14, 
                                    MTRLASTATUS = @p15, MTRLRTYPE = @p16, MTRLBRATE = @p17, 
                                    MTRLCATEID = @p18, PACKMID = @p19, DISPORDER = @p20, 
                                    LMUSRID = @p21, DISPSTATUS = @p22, PRCSDATE = @p23 
                                WHERE MTRLID = @p24",
                                tab.MTRLCODE, tab.MTRLDESC, tab.MTRLGID, tab.MTRLTID,
                                tab.UNITID, tab.HSNID, tab.ACHEADID ?? 0, tab.SCHLID ?? 0,
                                tab.ROLNQTY ?? 0, tab.ROLXQTY ?? 0, tab.EOQTY ?? 0, tab.MTRLPRFT ?? 0,
                                tab.MTRLBQTY ?? 0, tab.MTRLESTATUS ?? 0, tab.MTRLBSTATUS ?? 0,
                                tab.MTRLASTATUS ?? 0, tab.MTRLRTYPE ?? 0, tab.MTRLBRATE,
                                tab.MTRLCATEID, tab.PACKMID, tab.DISPORDER,
                                tab.LMUSRID, tab.DISPSTATUS, tab.PRCSDATE, tab.MTRLID);

                            // Redirect to Index page after successful update
                            TempData["SuccessMessage"] = "Material updated successfully!";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        // Insert new record
                        tab.CUSRID = currentUser;
                        tab.LMUSRID = currentUser;
                        tab.PRCSDATE = DateTime.Now;

                        // Insert using raw SQL
                        db.Database.ExecuteSqlCommand(@"
                            INSERT INTO MATERIALMASTER 
                            (MTRLCODE, MTRLDESC, MTRLGID, MTRLTID, UNITID, HSNID, ACHEADID, SCHLID, 
                             ROLNQTY, ROLXQTY, EOQTY, MTRLPRFT, MTRLBQTY, MTRLESTATUS, MTRLBSTATUS, 
                             MTRLASTATUS, MTRLRTYPE, MTRLBRATE, MTRLCATEID, PACKMID, DISPORDER, 
                             CUSRID, LMUSRID, DISPSTATUS, PRCSDATE) 
                            VALUES 
                            (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, 
                             @p13, @p14, @p15, @p16, @p17, @p18, @p19, @p20, @p21, @p22, @p23, @p24)",
                            tab.MTRLCODE, tab.MTRLDESC, tab.MTRLGID, tab.MTRLTID,
                            tab.UNITID, tab.HSNID, tab.ACHEADID ?? 0, tab.SCHLID ?? 0,
                            tab.ROLNQTY ?? 0, tab.ROLXQTY ?? 0, tab.EOQTY ?? 0, tab.MTRLPRFT ?? 0,
                            tab.MTRLBQTY ?? 0, tab.MTRLESTATUS ?? 0, tab.MTRLBSTATUS ?? 0,
                            tab.MTRLASTATUS ?? 0, tab.MTRLRTYPE ?? 0, tab.MTRLBRATE,
                            tab.MTRLCATEID, tab.PACKMID, tab.DISPORDER,
                            tab.CUSRID, tab.LMUSRID, tab.DISPSTATUS, tab.PRCSDATE);

                        // Redirect to Index page after successful insert
                        TempData["SuccessMessage"] = "Material added successfully!";
                        return RedirectToAction("Index");
                    }
                    } // Close the else block for duplicate check
                }
                else
                {
                    ViewBag.msg = "<div class='alert alert-danger'>Please correct the errors and try again.</div>";
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = $"<div class='alert alert-danger'>Error: {ex.Message}</div>";
            }

            // Repopulate dropdowns
            PopulateDropdowns(tab, tab.MTRLID > 0 ? tab.MTRLID : (int?)null);
            return View("Form", tab);
        }

        // AJAX method to get data for DataTables
        public ActionResult GetAjaxData()
        {
            try
            {
                var materials = db.MaterialMasters.OrderBy(m => m.MTRLCODE).ToList();

                var result = materials.Select(m => new
                {
                    MTRLID = m.MTRLID,
                    MTRLCODE = m.MTRLCODE ?? "",
                    MTRLDESC = m.MTRLDESC ?? "",
                    GroupName = GetMaterialGroupName(m.MTRLGID),
                    TypeName = GetMaterialTypeName(m.MTRLTID),
                    UnitName = GetUnitName(m.UNITID),
                    HSNName = GetHSNCodeName(m.HSNID),
                    DISPSTATUS = m.DISPSTATUS == 0 ? "Enabled" : "Disabled",
                    StatusBadge = m.DISPSTATUS == 0 ? 
                        "<span class='badge badge-success'>Enabled</span>" : 
                        "<span class='badge badge-danger'>Disabled</span>"
                }).ToList();

                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private string GetMaterialGroupName(int mtrlgid)
        {
            try
            {
                var result = db.Database.SqlQuery<string>(
                    "SELECT MTRLGDESC FROM MATERIALGROUPMASTER WHERE MTRLGID = @p0", mtrlgid).FirstOrDefault();
                return result ?? "N/A";
            }
            catch
            {
                return "N/A";
            }
        }

        private string GetMaterialTypeName(int mtrltid)
        {
            try
            {
                var result = db.Database.SqlQuery<string>(
                    "SELECT MTRLTDESC FROM MATERIALTYPEMASTER WHERE MTRLTID = @p0", mtrltid).FirstOrDefault();
                return result ?? "N/A";
            }
            catch
            {
                return "N/A";
            }
        }

        private string GetUnitName(int unitid)
        {
            try
            {
                var result = db.Database.SqlQuery<string>(
                    "SELECT UNITDESC FROM UNITMASTER WHERE UNITID = @p0", unitid).FirstOrDefault();
                return result ?? "N/A";
            }
            catch
            {
                return "N/A";
            }
        }

        private string GetHSNCodeName(int hsnid)
        {
            try
            {
                var result = db.Database.SqlQuery<string>(
                    "SELECT HSNDESC FROM HSNCODEMASTER WHERE HSNID = @p0", hsnid).FirstOrDefault();
                return result ?? "N/A";
            }
            catch
            {
                return "N/A";
            }
        }

        // Remote validation for unique material code (case-insensitive like PackingTypeMaster)
        public JsonResult ValidateMTRLCODE(string MTRLCODE, int MTRLID = 0)
        {
            try
            {
                var exists = db.Database.SqlQuery<int>(
                    @"SELECT COUNT(*) FROM MATERIALMASTER 
                      WHERE UPPER(MTRLCODE) = @p0 AND MTRLID != @p1", 
                    MTRLCODE.ToUpper(), MTRLID).FirstOrDefault();

                return Json(exists == 0, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        // Delete method for AJAX calls
        [HttpPost]
        public ActionResult Del(int id)
        {
            try
            {
                // Check if user has delete role
                if (!User.IsInRole("MaterialMasterDelete"))
                {
                    return Json("Access Denied: You do not have permission to delete records. Please contact your administrator.");
                }
                
                var material = db.Database.SqlQuery<MaterialMaster>(
                    "SELECT * FROM MATERIALMASTER WHERE MTRLID = @p0", id).FirstOrDefault();

                if (material != null)
                {
                    db.Database.ExecuteSqlCommand("DELETE FROM MATERIALMASTER WHERE MTRLID = @p0", id);
                    return Json("Successfully deleted");
                }
                else
                {
                    return Json("Material not found");
                }
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message);
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
