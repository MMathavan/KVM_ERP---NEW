using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KVM_ERP.Models;

namespace KVM_ERP.Controllers.Masters
{
    [SessionExpire]
    public class LaboratoryMasterController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        [Authorize(Roles = "LaboratoryMasterIndex")]
        public ActionResult Index()
        {
            try
            {
                var laboratories = context.Database.SqlQuery<LaboratoryMaster>(
                    @"SELECT LABOID, LABODESC, LABOCODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE
                      FROM LABORATORYMASTER
                      ORDER BY LABOCODE"
                ).ToList();
                
                return View(laboratories);
            }
            catch (Exception ex)
            {
                return Content($"Error loading laboratories: {ex.Message}");
            }
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("GetAjaxData called for Laboratory Master DataTables");
                
                // Get laboratories
                var laboratories = context.Database.SqlQuery<LaboratoryMaster>(
                    @"SELECT LABOID, LABODESC, LABOCODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE
                      FROM LABORATORYMASTER
                      ORDER BY LABOCODE"
                ).ToList();

                // Format data for DataTables
                var allLaboratories = laboratories.Select(l => new {
                    LABOID = l.LABOID,
                    LABOCODE = l.LABOCODE ?? "",
                    LABODESC = l.LABODESC ?? "",
                    DISPSTATUS = l.DISPSTATUS,
                    CUSRID = l.CUSRID ?? "",
                    LMUSRID = l.LMUSRID ?? "",
                    PRCSDATE = l.PRCSDATE
                }).ToList();

                return Json(new { aaData = allLaboratories }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAjaxData: {ex.Message}");
                return Json(new { error = "Error loading data" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "LaboratoryMasterCreate,LaboratoryMasterEdit")]
        public ActionResult Form(int? id)
        {
            try
            {
                LaboratoryMaster tab = new LaboratoryMaster();
                ViewBag.msg = "";

                if (id == null)
                {
                    tab.LABOID = 0;
                    tab.DISPSTATUS = 0; // Default to Enabled
                }

                if (id == -1)
                    ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";

                if (id != 0 && id != -1 && id != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Form: Loading laboratory with ID: {id}");
                    
                    try
                    {
                        tab = context.Database.SqlQuery<LaboratoryMaster>(
                            "SELECT * FROM LABORATORYMASTER WHERE LABOID = {0}", id
                        ).FirstOrDefault();
                        
                        if (tab == null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Laboratory with ID {id} not found in database");
                            return HttpNotFound();
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"Edit Laboratory - ID: {tab.LABOID}, Description: {tab.LABODESC}");
                        System.Diagnostics.Debug.WriteLine($"Code: {tab.LABOCODE}, Status: {tab.DISPSTATUS}");
                        
                        ViewBag.msg = "";
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading laboratory {id}: {ex.Message}");
                        // Fallback to new laboratory if loading fails
                        tab = new LaboratoryMaster
                        {
                            LABOID = 0,
                            DISPSTATUS = 0
                        };
                        ViewBag.msg = $"Error loading laboratory {id}. Creating new instead.";
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
        [Authorize(Roles = "LaboratoryMasterCreate,LaboratoryMasterEdit")]
        public ActionResult savedata(LaboratoryMaster tab)
        {
            try
            {
                // Get current user information from session (following MaterialTypeMaster pattern)
                var currentUserName = Session["CUSRID"]?.ToString();
                
                // If session doesn't have CUSRID, try to get from User.Identity or USERNAME
                if (string.IsNullOrEmpty(currentUserName))
                {
                    currentUserName = Session["USERNAME"]?.ToString() ?? User.Identity.Name ?? "admin";
                }
                
                // Debug: Log session values
                System.Diagnostics.Debug.WriteLine($"Session CUSRID: {Session["CUSRID"]}");
                System.Diagnostics.Debug.WriteLine($"Session USERNAME: {Session["USERNAME"]}");
                System.Diagnostics.Debug.WriteLine($"Session USERID: {Session["USERID"]}");
                System.Diagnostics.Debug.WriteLine($"User.Identity.Name: {User.Identity.Name}");
                System.Diagnostics.Debug.WriteLine($"Final currentUserName: {currentUserName}");
                
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
                if (!string.IsNullOrEmpty(tab.LABOCODE))
                {
                    tab.LABOCODE = tab.LABOCODE.ToUpper();
                }
                if (!string.IsNullOrEmpty(tab.LABODESC))
                {
                    tab.LABODESC = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tab.LABODESC.ToLower());
                }

                if (tab.LABOID == 0)
                {
                    // Create new record - both CUSRID and LMUSRID get username (following MaterialTypeMaster pattern)
                    tab.CUSRID = currentUserName;
                    tab.LMUSRID = currentUserName;
                    tab.PRCSDATE = prcsdate;
                    
                    System.Diagnostics.Debug.WriteLine($"Create: CUSRID = {tab.CUSRID}, LMUSRID = {tab.LMUSRID}");

                    context.Database.ExecuteSqlCommand(
                        @"INSERT INTO LABORATORYMASTER 
                          (LABODESC, LABOCODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE) 
                          VALUES 
                          ({0}, {1}, {2}, {3}, {4}, {5})",
                        tab.LABODESC, tab.LABOCODE, tab.CUSRID, tab.LMUSRID, tab.DISPSTATUS, tab.PRCSDATE
                    );
                }
                else
                {
                    // Update existing record - preserve original CUSRID, update only LMUSRID with current username
                    tab.LMUSRID = currentUserName;
                    tab.PRCSDATE = prcsdate;
                    
                    System.Diagnostics.Debug.WriteLine($"Update: Updating LMUSRID = {tab.LMUSRID}, preserving CUSRID in database");

                    context.Database.ExecuteSqlCommand(
                        @"UPDATE LABORATORYMASTER SET 
                          LABODESC = {1}, LABOCODE = {2}, LMUSRID = {3}, 
                          DISPSTATUS = {4}, PRCSDATE = {5}
                          WHERE LABOID = {0}",
                        tab.LABOID, tab.LABODESC, tab.LABOCODE, tab.LMUSRID, 
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
                if (!User.IsInRole("LaboratoryMasterDelete"))
                {
                    Response.StatusCode = 403;
                    return Content("Access Denied: You do not have permission to delete records. Please contact your administrator.");
                }
                
                context.Database.ExecuteSqlCommand("DELETE FROM LABORATORYMASTER WHERE LABOID = {0}", id);
                return Content("Deleted Successfully ...");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Content("Error deleting laboratory: " + ex.Message);
            }
        }
    }
}
