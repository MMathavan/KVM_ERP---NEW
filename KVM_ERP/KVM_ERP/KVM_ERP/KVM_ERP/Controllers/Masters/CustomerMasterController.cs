using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KVM_ERP.Models;

namespace KVM_ERP.Controllers.Masters
{
    [SessionExpire]
    public class CustomerMasterController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        [Authorize(Roles = "CustomerMasterIndex")]
        public ActionResult Index()
        {
            try
            {
                var customers = context.Database.SqlQuery<CustomerMaster>(
                    @"SELECT CATEID, CATENAME, CATEDNAME, CATEADDR1, CATEADDR2, CATEADDR3, CATEADDR4, CATEADDR5, 
                             LOCTID, STATEID, CATEPHN1, CATEPHN2, CATEPHN3, CATEPHN4, CATEPNAME, CATEMAIL, 
                             CATE_GST_NO, CATE_PAN_NO, CATE_TAN_NO, CATE_PEST_LIC_NO, CATE_SEED_LIC_NO, 
                             CATECODE, CATE_CRDTPRD, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE, CATETID, ACHEADID,
                             CATE_BANK_NAME, CATE_BRNCH_NAME, CATE_IFSC_CODE, CATE_ACNO, CATE_IBAN_CODE, 
                             CATE_SWIFT_CODE, CATE_DSGNDESC, CATENO, CATESTYPE
                      FROM CUSTOMERMASTER"
                ).ToList();
                
                return View(customers);
            }
            catch (Exception ex)
            {
                return Content($"Error loading customers: {ex.Message}");
            }
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("GetAjaxData called for Customer Master DataTables");
                
                // Get customers first
                var customers = context.Database.SqlQuery<CustomerMaster>(
                    @"SELECT CATEID, CATENAME, CATEDNAME, CATEADDR1, CATEADDR2, CATEADDR3, CATEADDR4, CATEADDR5, 
                             LOCTID, STATEID, CATEPHN1, CATEPHN2, CATEPHN3, CATEPHN4, CATEPNAME, CATEMAIL, 
                             CATE_GST_NO, CATE_PAN_NO, CATE_TAN_NO, CATE_PEST_LIC_NO, CATE_SEED_LIC_NO, 
                             CATECODE, CATE_CRDTPRD, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE, CATETID, ACHEADID,
                             CATE_BANK_NAME, CATE_BRNCH_NAME, CATE_IFSC_CODE, CATE_ACNO, CATE_IBAN_CODE, 
                             CATE_SWIFT_CODE, CATE_DSGNDESC, CATENO, CATESTYPE
                      FROM CUSTOMERMASTER"
                ).ToList();

                // Get lookup data using Entity Framework
                var locationLookup = new Dictionary<int, string>();
                var stateLookup = new Dictionary<int, string>();
                
                try
                {
                    locationLookup = context.LocationMasters
                        .Select(l => new { l.LOCTID, l.LOCTDESC })
                        .ToDictionary(l => l.LOCTID, l => l.LOCTDESC ?? "");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Location lookup failed: {ex.Message}");
                }
                
                try
                {
                    stateLookup = context.StateMasters
                        .Select(s => new { s.STATEID, s.STATEDESC })
                        .ToDictionary(s => s.STATEID, s => s.STATEDESC ?? "");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"State lookup failed: {ex.Message}");
                }

                // Combine data with lookup names (like Employee Master)
                var allCustomers = customers.Select(c => new {
                    CATEID = c.CATEID,
                    CATECODE = c.CATECODE ?? "",
                    CATENAME = c.CATENAME ?? "",
                    CATEDNAME = c.CATEDNAME ?? "",
                    CATEMAIL = c.CATEMAIL ?? "",
                    LOCTID = c.LOCTID,
                    STATEID = c.STATEID,
                    DISPSTATUS = c.DISPSTATUS,
                    CUSRID = c.CUSRID ?? "",
                    LMUSRID = c.LMUSRID,
                    PRCSDATE = c.PRCSDATE,
                    LocationName = locationLookup.ContainsKey(c.LOCTID) ? locationLookup[c.LOCTID] : "Unknown",
                    StateName = stateLookup.ContainsKey(c.STATEID) ? stateLookup[c.STATEID] : "Unknown"
                }).ToList();

                return Json(new { aaData = allCustomers }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAjaxData: {ex.Message}");
                return Json(new { error = "Error loading data" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "CustomerMasterCreate,CustomerMasterEdit")]
        public ActionResult Form(int? id)
        {
            try
            {
                CustomerMaster tab = new CustomerMaster();
                ViewBag.msg = "Add New Customer";

                if (id == null)
                {
                    tab.CATEID = 0;
                    // Set only required default values
                    tab.CATETID = 2;
                    tab.CATENO = 1;
                    tab.CATESTYPE = 0;
                    tab.ACHEADID = 1;
                }

                if (id == -1)
                    ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";

                if (id != 0 && id != -1 && id != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Form: Loading customer with ID: {id}");
                    
                    try
                    {
                        tab = context.Database.SqlQuery<CustomerMaster>(
                            "SELECT * FROM CUSTOMERMASTER WHERE CATEID = {0}", id
                        ).FirstOrDefault();
                        
                        if (tab == null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Customer with ID {id} not found in database");
                            return HttpNotFound();
                        }
                        
                        // Debug logging - detailed customer data
                        System.Diagnostics.Debug.WriteLine($"Edit Customer - ID: {tab.CATEID}, Name: {tab.CATENAME}");
                        System.Diagnostics.Debug.WriteLine($"Location ID: {tab.LOCTID}, State ID: {tab.STATEID}");
                        System.Diagnostics.Debug.WriteLine($"Status: {tab.DISPSTATUS}");
                        System.Diagnostics.Debug.WriteLine($"LOCTID Type: {tab.LOCTID.GetType()}, STATEID Type: {tab.STATEID.GetType()}");
                        System.Diagnostics.Debug.WriteLine($"LOCTID == 0: {tab.LOCTID == 0}, STATEID == 0: {tab.STATEID == 0}");
                        
                        ViewBag.msg = "Edit Customer";
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading customer {id}: {ex.Message}");
                        // Fallback to new customer if loading fails
                        tab = new CustomerMaster
                        {
                            CATEID = 0,
                            CATETID = 2,
                            CATENO = 1,
                            CATESTYPE = 0,
                            ACHEADID = 1
                        };
                        ViewBag.msg = $"Error loading customer {id}. Creating new instead.";
                    }
                }

                // Create status dropdown using SelectList for proper selection (exactly like Employee Master)
                var statusList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "0", Text = "Enabled" },
                    new SelectListItem { Value = "1", Text = "Disabled" }
                };
                ViewBag.DISPSTATUS = new SelectList(statusList, "Value", "Text", tab.DISPSTATUS.ToString());
                
                // Debug: Show what status is being selected
                System.Diagnostics.Debug.WriteLine($"Status dropdown - Current DISPSTATUS: {tab.DISPSTATUS}, Selected Value: {tab.DISPSTATUS.ToString()}");

                // Get locations for dropdown with error handling (exactly like Employee Master)
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Loading locations for customer LOCTID: {tab.LOCTID}");
                    
                    var locations = context.LocationMasters.ToList();
                    var loctList = new List<SelectListItem>();
                    loctList.Add(new SelectListItem { Value = "", Text = "-- Select Location --" });
                    
                    // Add only enabled locations - NO disabled items at all
                    foreach (var loct in locations.Where(l => l.DISPSTATUS == 0).OrderBy(l => l.LOCTDESC))
                    {
                        loctList.Add(new SelectListItem
                        {
                            Value = loct.LOCTID.ToString(), 
                            Text = loct.LOCTDESC
                        });
                    }
                    
                    // If customer has disabled location, reset to empty selection (same as Employee Master)
                    var selectedLoctId = "";
                    if (tab.LOCTID != 0 && loctList.Any(x => x.Value == tab.LOCTID.ToString()))
                    {
                        selectedLoctId = tab.LOCTID.ToString();
                    }
                    else if (tab.LOCTID != 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Customer has disabled location ID {tab.LOCTID} - resetting to empty");
                    }
                    
                    ViewBag.LOCTID = new SelectList(loctList, "Value", "Text", selectedLoctId);
                    System.Diagnostics.Debug.WriteLine($"Location dropdown created with {loctList.Count} items, model LOCTID: {tab.LOCTID}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading locations: {ex.Message}");
                    ViewBag.LOCTID = new SelectList(new List<SelectListItem> 
                    { 
                        new SelectListItem { Value = "", Text = "-- Select Location --" }
                    }, "Value", "Text");
                }

                // Get states for dropdown with error handling (exactly like Employee Master)
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Loading states for customer STATEID: {tab.STATEID}");
                    
                    var states = context.StateMasters.ToList();
                    var stateList = new List<SelectListItem>();
                    stateList.Add(new SelectListItem { Value = "", Text = "-- Select State --" });
                    
                    // Add only enabled states - NO disabled items at all
                    foreach (var state in states.Where(s => s.DISPSTATUS == 0).OrderBy(s => s.STATEDESC))
                    {
                        stateList.Add(new SelectListItem
                        {
                            Value = state.STATEID.ToString(),
                            Text = state.STATEDESC
                        });
                    }
                    
                    // If customer has disabled state, reset to empty selection (same as Employee Master)
                    var selectedStateId = "";
                    if (tab.STATEID != 0 && stateList.Any(x => x.Value == tab.STATEID.ToString()))
                    {
                        selectedStateId = tab.STATEID.ToString();
                    }
                    else if (tab.STATEID != 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Customer has disabled state ID {tab.STATEID} - resetting to empty");
                    }
                    
                    ViewBag.STATEID = new SelectList(stateList, "Value", "Text", selectedStateId);
                    System.Diagnostics.Debug.WriteLine($"State dropdown created with {stateList.Count} items, model STATEID: {tab.STATEID}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading states: {ex.Message}");
                    ViewBag.STATEID = new SelectList(new List<SelectListItem> 
                    { 
                        new SelectListItem { Value = "", Text = "-- Select State --" }
                    }, "Value", "Text");
                }

                return View(tab);
            }
            catch (Exception ex)
            {
                return Content($"Error in Form action: {ex.Message}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CustomerMasterCreate,CustomerMasterEdit")]
        public ActionResult savedata(CustomerMaster tab)
        {
            try
            {
                var currentUserId = Session["USERID"] != null ? Convert.ToInt32(Session["USERID"]) : 1;
                var prcsdate = DateTime.Now;

                // Server-side model validation
                if (!ModelState.IsValid)
                {
                    // Rebuild dropdowns and status lists when returning the view with errors
                    // Status dropdown (exactly like Employee Master)
                    var statusList = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "0", Text = "Enabled" },
                        new SelectListItem { Value = "1", Text = "Disabled" }
                    };
                    ViewBag.DISPSTATUS = new SelectList(statusList, "Value", "Text");

                    // Location dropdown (same as Employee Master)
                    var locations = context.LocationMasters.Where(l => l.DISPSTATUS == 0).ToList();
                    var loctList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Select Location --" } };
                    foreach (var loct in locations.OrderBy(l => l.LOCTDESC))
                    {
                        loctList.Add(new SelectListItem
                        {
                            Value = loct.LOCTID.ToString(),
                            Text = loct.LOCTDESC
                        });
                    }
                    
                    ViewBag.LOCTID = new SelectList(loctList, "Value", "Text");

                    // State dropdown (same as Employee Master)
                    var states = context.StateMasters.Where(s => s.DISPSTATUS == 0).OrderBy(s => s.STATEDESC).ToList();
                    var stateList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "-- Select State --" } };
                    foreach (var state in states)
                    {
                        stateList.Add(new SelectListItem
                        {
                            Value = state.STATEID.ToString(),
                            Text = state.STATEDESC
                        });
                    }
                    
                    ViewBag.STATEID = new SelectList(stateList, "Value", "Text");

                    // Return the same Form view with validation errors
                    return View("Form", tab);
                }

                if (tab.CATEID == 0)
                {
                    // Create new record
                    tab.CUSRID = Session["USERNAME"]?.ToString() ?? "admin";
                    tab.LMUSRID = currentUserId;
                    tab.PRCSDATE = prcsdate;
                    tab.CATETID = 2;
                    tab.CATENO = 1;
                    tab.CATESTYPE = 0;
                    tab.ACHEADID = 1;

                    context.Database.ExecuteSqlCommand(
                        @"INSERT INTO CUSTOMERMASTER 
                          (CATENAME, CATEDNAME, CATEADDR1, CATEADDR2, CATEADDR3, CATEADDR4, CATEADDR5, 
                           LOCTID, STATEID, CATEPHN1, CATEPHN2, CATEPHN3, CATEPHN4, CATEPNAME, CATEMAIL, 
                           CATE_GST_NO, CATE_PAN_NO, CATE_TAN_NO, CATE_PEST_LIC_NO, CATE_SEED_LIC_NO, 
                           CATECODE, CATE_CRDTPRD, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE, CATETID, 
                           CATENO, CATESTYPE, ACHEADID, 
                           CATE_BANK_NAME, CATE_BRNCH_NAME, CATE_IFSC_CODE, CATE_ACNO, CATE_IBAN_CODE, CATE_SWIFT_CODE, CATE_DSGNDESC) 
                          VALUES 
                          ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, 
                           {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27}, {28}, {29},
                           {30}, {31}, {32}, {33}, {34}, {35}, {36})",
                        tab.CATENAME, tab.CATEDNAME, tab.CATEADDR1, tab.CATEADDR2, tab.CATEADDR3, tab.CATEADDR4, tab.CATEADDR5,
                        tab.LOCTID, tab.STATEID, tab.CATEPHN1, tab.CATEPHN2, tab.CATEPHN3, tab.CATEPHN4, tab.CATEPNAME, tab.CATEMAIL,
                        tab.CATE_GST_NO, tab.CATE_PAN_NO, tab.CATE_TAN_NO, tab.CATE_PEST_LIC_NO, tab.CATE_SEED_LIC_NO,
                        tab.CATECODE, tab.CATE_CRDTPRD, tab.CUSRID, tab.LMUSRID, tab.DISPSTATUS, tab.PRCSDATE, tab.CATETID,
                        tab.CATENO, tab.CATESTYPE, tab.ACHEADID,
                        tab.CATE_BANK_NAME, tab.CATE_BRNCH_NAME, tab.CATE_IFSC_CODE, tab.CATE_ACNO, tab.CATE_IBAN_CODE, tab.CATE_SWIFT_CODE, tab.CATE_DSGNDESC
                    );
                }
                else
                {
                    // Update existing record
                    tab.LMUSRID = currentUserId;
                    tab.PRCSDATE = prcsdate;

                    context.Database.ExecuteSqlCommand(
                        @"UPDATE CUSTOMERMASTER SET 
                          CATENAME = {1}, CATEDNAME = {2}, CATEADDR1 = {3}, CATEADDR2 = {4}, CATEADDR3 = {5}, 
                          CATEADDR4 = {6}, CATEADDR5 = {7}, LOCTID = {8}, STATEID = {9}, CATEPHN1 = {10}, 
                          CATEPHN2 = {11}, CATEPHN3 = {12}, CATEPHN4 = {13}, CATEPNAME = {14}, CATEMAIL = {15}, 
                          CATE_GST_NO = {16}, CATE_PAN_NO = {17}, CATE_TAN_NO = {18}, CATE_PEST_LIC_NO = {19}, 
                          CATE_SEED_LIC_NO = {20}, CATECODE = {21}, CATE_CRDTPRD = {22}, LMUSRID = {23}, 
                          DISPSTATUS = {24}, PRCSDATE = {25},
                          CATE_BANK_NAME = {26}, CATE_BRNCH_NAME = {27}, CATE_IFSC_CODE = {28}, CATE_ACNO = {29},
                          CATE_IBAN_CODE = {30}, CATE_SWIFT_CODE = {31}, CATE_DSGNDESC = {32}
                          WHERE CATEID = {0}",
                        tab.CATEID, tab.CATENAME, tab.CATEDNAME, tab.CATEADDR1, tab.CATEADDR2, tab.CATEADDR3,
                        tab.CATEADDR4, tab.CATEADDR5, tab.LOCTID, tab.STATEID, tab.CATEPHN1, tab.CATEPHN2,
                        tab.CATEPHN3, tab.CATEPHN4, tab.CATEPNAME, tab.CATEMAIL, tab.CATE_GST_NO, tab.CATE_PAN_NO,
                        tab.CATE_TAN_NO, tab.CATE_PEST_LIC_NO, tab.CATE_SEED_LIC_NO, tab.CATECODE, tab.CATE_CRDTPRD,
                        tab.LMUSRID, tab.DISPSTATUS, tab.PRCSDATE,
                        tab.CATE_BANK_NAME, tab.CATE_BRNCH_NAME, tab.CATE_IFSC_CODE, tab.CATE_ACNO,
                        tab.CATE_IBAN_CODE, tab.CATE_SWIFT_CODE, tab.CATE_DSGNDESC
                    );
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Optionally log ex
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult deletedata(int id)
        {
            try
            {
                // Check if user has delete role
                if (!User.IsInRole("CustomerMasterDelete"))
                {
                    Response.StatusCode = 403;
                    return Content("Access Denied: You do not have permission to delete records. Please contact your administrator.");
                }
                
                context.Database.ExecuteSqlCommand("DELETE FROM CUSTOMERMASTER WHERE CATEID = {0}", id);
                return Content("Deleted Successfully ...");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Content("Error deleting customer: " + ex.Message);
            }
        }
    }
}
