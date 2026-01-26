using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KVM_ERP.Models;

namespace KVM_ERP.Controllers.Masters
{
    [SessionExpire]
    public class LocationMasterController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        [Authorize(Roles = "LocationMasterIndex")]
        public ActionResult Index()
        {
            // Use raw SQL to match actual database schema
            var locations = context.Database.SqlQuery<LocationMaster>(
                @"SELECT LOCTID, LOCTCODE, LOCTDESC, STATEID, CUSRID, LMUSRID, 
                         DISPSTATUS, PRCSDATE 
                  FROM LOCATIONMASTER"
            ).ToList();
            
            return View(locations);
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            try
            {
                // Get locations first
                var locations = context.Database.SqlQuery<LocationMaster>(
                    @"SELECT LOCTID, LOCTCODE, LOCTDESC, STATEID, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE 
                      FROM LOCATIONMASTER"
                ).ToList();

                // Get states for lookup using Entity Framework
                var stateLookup = context.StateMasters
                    .Select(s => new { s.STATEID, s.STATEDESC })
                    .ToDictionary(s => s.STATEID, s => s.STATEDESC ?? "");

                // Combine data
                var allLocations = locations.Select(l => new {
                    LOCTID = l.LOCTID,
                    LOCTCODE = l.LOCTCODE ?? "",
                    LOCTDESC = l.LOCTDESC ?? "",
                    STATEID = l.STATEID,
                    CUSRID = l.CUSRID ?? "",
                    LMUSRID = l.LMUSRID,
                    DISPSTATUS = l.DISPSTATUS,
                    PRCSDATE = l.PRCSDATE,
                    StateName = stateLookup.ContainsKey(l.STATEID) ? stateLookup[l.STATEID] : ""
                }).ToList();

                var totalRecords = allLocations.Count;

                // Apply search filter
                var filteredData = allLocations.AsQueryable();
                var searchValue = Request["search[value]"] ?? param.sSearch ?? "";
                
                if (!string.IsNullOrEmpty(searchValue))
                {
                    searchValue = searchValue.ToLower();
                    filteredData = filteredData.Where(l =>
                        (l.LOCTCODE ?? "").ToLower().Contains(searchValue) ||
                        (l.LOCTDESC ?? "").ToLower().Contains(searchValue) ||
                        (l.StateName ?? "").ToLower().Contains(searchValue) ||
                        l.LOCTID.ToString().Contains(searchValue)
                    );
                }

                var filteredRecords = filteredData.Count();

                // Apply sorting
                var sortColumn = Request["order[0][column]"] ?? Request["iSortCol_0"] ?? "0";
                var sortDirection = Request["order[0][dir]"] ?? Request["sSortDir_0"] ?? "asc";
                
                switch (sortColumn)
                {
                    case "0":
                        filteredData = sortDirection == "desc" 
                            ? filteredData.OrderByDescending(l => l.LOCTCODE) 
                            : filteredData.OrderBy(l => l.LOCTCODE);
                        break;
                    case "1":
                        filteredData = sortDirection == "desc" 
                            ? filteredData.OrderByDescending(l => l.LOCTDESC) 
                            : filteredData.OrderBy(l => l.LOCTDESC);
                        break;
                    case "2":
                        filteredData = sortDirection == "desc" 
                            ? filteredData.OrderByDescending(l => l.StateName) 
                            : filteredData.OrderBy(l => l.StateName);
                        break;
                    case "3":
                        filteredData = sortDirection == "desc" 
                            ? filteredData.OrderByDescending(l => l.DISPSTATUS) 
                            : filteredData.OrderBy(l => l.DISPSTATUS);
                        break;
                    default:
                        filteredData = filteredData.OrderBy(l => l.LOCTCODE);
                        break;
                }

                // Apply paging
                var start = Convert.ToInt32(Request["start"] ?? Request["iDisplayStart"] ?? "0");
                var length = Convert.ToInt32(Request["length"] ?? Request["iDisplayLength"] ?? "10");
                
                var pagedData = filteredData.Skip(start).Take(length).ToList();

                // Format data for DataTables
                var result = pagedData.Select(l => new
                {
                    LOCTCODE = l.LOCTCODE ?? "",
                    LOCTDESC = l.LOCTDESC ?? "",
                    StateName = l.StateName ?? "",
                    DISPSTATUS = l.DISPSTATUS.ToString(),
                    LOCTID = l.LOCTID
                }).ToList();

                return Json(new
                {
                    draw = Convert.ToInt32(Request["draw"] ?? param.sEcho ?? "1"),
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { 
                    draw = 1,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new object[0],
                    error = ex.Message 
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "LocationMasterCreate,LocationMasterEdit")]
        public ActionResult Form(int? id = 0)
        {
            LocationMaster tab = new LocationMaster();
            tab.LOCTID = 0;
            // Default to Enabled for create mode (cast to short to match model)
            tab.DISPSTATUS = (short)0;

            if (id == -1)
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";

            if (id != 0 && id != -1)
            {
                // Use Entity Framework to get the record
                tab = context.LocationMasters.Find(id);
                if (tab == null) return HttpNotFound();
                
                // Debug: Let's see what value we actually get from database
                System.Diagnostics.Debug.WriteLine($"Edit Mode - LOCTID: {tab.LOCTID}, DISPSTATUS: {tab.DISPSTATUS}");
            }

            // Clear any existing model state for dropdowns to ensure they work
            ModelState.Remove("DISPSTATUS");
            ModelState.Remove("STATEID");
            
            // Create status dropdown using SelectList for proper selection
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Enabled" },
                new SelectListItem { Value = "1", Text = "Disabled" }
            };
            
            ViewBag.DISPSTATUS = new SelectList(statusList, "Value", "Text", tab.DISPSTATUS.ToString());
            
            // Debug: Show what status is being selected
            System.Diagnostics.Debug.WriteLine($"Status dropdown - Current DISPSTATUS: {tab.DISPSTATUS}, Selected Value: {tab.DISPSTATUS.ToString()}");

            // Get states for dropdown using Entity Framework
            var states = context.StateMasters
                .Where(s => s.DISPSTATUS == 0)
                .OrderBy(s => s.STATEDESC)
                .Select(s => new { s.STATEID, s.STATEDESC })
                .ToList();

            // Create state dropdown using SelectList for proper selection
            var stateList = new List<SelectListItem>();
            stateList.Add(new SelectListItem { Value = "", Text = "-- Select State --" });
            
            foreach (var state in states)
            {
                stateList.Add(new SelectListItem
                {
                    Value = state.STATEID.ToString(),
                    Text = state.STATEDESC
                });
            }
            
            ViewBag.STATEID = new SelectList(stateList, "Value", "Text", tab.STATEID.ToString());
            
            // Debug: Show what state is being selected
            System.Diagnostics.Debug.WriteLine($"State dropdown - Current STATEID: {tab.STATEID}, Selected Value: {tab.STATEID.ToString()}");

            // Debug: Let's see what we're passing to the view
            System.Diagnostics.Debug.WriteLine($"Passing to View - LOCTID: {tab.LOCTID}, DISPSTATUS: {tab.DISPSTATUS}, STATEID: {tab.STATEID}");

            return View(tab);
        }

        [HttpPost]
        [Authorize(Roles = "LocationMasterCreate,LocationMasterEdit")]
        public void savedata(LocationMaster tab)
        {
            // Debug: Let's see what we receive from the form
            System.Diagnostics.Debug.WriteLine($"Received from Form - LOCTID: {tab.LOCTID}, DISPSTATUS: {tab.DISPSTATUS}, LOCTCODE: {tab.LOCTCODE}");
            
            var currentUserId = Session["USERID"] != null ? Convert.ToInt32(Session["USERID"]) : 1;
            var prcsdate = DateTime.Now;

            var s = tab.LOCTDESC ?? string.Empty;
            s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());

            if (tab.LOCTID != 0)
            {
                // Fix: For updates, preserve original CUSRID and only update LMUSRID
                var existingRecord = context.Database.SqlQuery<LocationMaster>(
                    @"SELECT LOCTID, LOCTCODE, LOCTDESC, STATEID, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE 
                      FROM LOCATIONMASTER WHERE LOCTID = @p0", tab.LOCTID).FirstOrDefault();
                
                if (existingRecord != null)
                {
                    // Update using raw SQL - preserve CUSRID, update LMUSRID
                    context.Database.ExecuteSqlCommand(
                        @"UPDATE LOCATIONMASTER 
                          SET LOCTDESC = @p0, LOCTCODE = @p1, STATEID = @p2, LMUSRID = @p3, 
                              DISPSTATUS = @p4, PRCSDATE = @p5 
                          WHERE LOCTID = @p6",
                        s, tab.LOCTCODE, tab.STATEID, currentUserId, tab.DISPSTATUS, prcsdate, tab.LOCTID);
                }
            }
            else
            {
                // For new records, set both CUSRID and LMUSRID to current user
                var cusrid = Session["CUSRID"] != null ? Session["CUSRID"].ToString() : "admin";
                
                // Insert using raw SQL
                context.Database.ExecuteSqlCommand(
                    @"INSERT INTO LOCATIONMASTER (LOCTDESC, LOCTCODE, STATEID, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE) 
                      VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6)",
                    s, tab.LOCTCODE, tab.STATEID, cusrid, currentUserId, tab.DISPSTATUS, prcsdate);
            }

            if (Request.Form.Get("continue") == null)
            {
                Response.Redirect("index");
            }
            else
            {
                Response.Redirect("Form/-1");
            }
        }

        public void Del()
        {
            // Check if user has delete role
            if (!User.IsInRole("LocationMasterDelete"))
            {
                Response.Write("Access Denied: You do not have permission to delete records. Please contact your administrator.");
                return;
            }
            
            string id = Request.Form.Get("id");
            if (string.IsNullOrWhiteSpace(id)) { Response.Write("Invalid Id"); return; }

            // Use raw SQL to match database schema
            var rec = context.Database.SqlQuery<LocationMaster>(
                @"SELECT LOCTID, LOCTCODE, LOCTDESC, STATEID, CUSRID, LMUSRID, 
                         DISPSTATUS, PRCSDATE 
                  FROM LOCATIONMASTER 
                  WHERE LOCTID = @p0", Convert.ToInt32(id)
            ).FirstOrDefault();

            if (rec == null) { Response.Write("Record not found"); return; }

            // Delete using raw SQL
            context.Database.ExecuteSqlCommand("DELETE FROM LOCATIONMASTER WHERE LOCTID = @p0", Convert.ToInt32(id));

            Response.Write("Deleted Successfully ...");
        }
    }
}
