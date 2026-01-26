using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KVM_ERP.Models;

namespace KVM_ERP.Controllers.Masters
{
    [SessionExpire]
    public class DepartmentMasterController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        [Authorize(Roles = "DepartmentMasterIndex")]
        public ActionResult Index()
        {
            // Use raw SQL to match actual database schema
            var departments = context.Database.SqlQuery<DepartmentMaster>(
                @"SELECT DEPTID, DEPTCODE, DEPTDESC, CUSRID, LMUSRID, 
                         DISPSTATUS, PRCSDATE 
                  FROM DEPARTMENTMASTER"
            ).ToList();
            
            return View(departments);
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            try
            {
                // Get all data first (for simplicity, can be optimized later)
                var allDepartments = context.Database.SqlQuery<DepartmentMaster>(
                    @"SELECT DEPTID, DEPTCODE, DEPTDESC, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE 
                      FROM DEPARTMENTMASTER"
                ).ToList();

                var totalRecords = allDepartments.Count;

                // Apply search filter
                var filteredData = allDepartments.AsQueryable();
                var searchValue = Request["search[value]"] ?? param.sSearch ?? "";
                
                if (!string.IsNullOrEmpty(searchValue))
                {
                    searchValue = searchValue.ToLower();
                    filteredData = filteredData.Where(d =>
                        (d.DEPTCODE ?? "").ToLower().Contains(searchValue) ||
                        (d.DEPTDESC ?? "").ToLower().Contains(searchValue) ||
                        d.DEPTID.ToString().Contains(searchValue)
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
                            ? filteredData.OrderByDescending(d => d.DEPTCODE) 
                            : filteredData.OrderBy(d => d.DEPTCODE);
                        break;
                    case "1":
                        filteredData = sortDirection == "desc" 
                            ? filteredData.OrderByDescending(d => d.DEPTDESC) 
                            : filteredData.OrderBy(d => d.DEPTDESC);
                        break;
                    case "2":
                        filteredData = sortDirection == "desc" 
                            ? filteredData.OrderByDescending(d => d.DISPSTATUS) 
                            : filteredData.OrderBy(d => d.DISPSTATUS);
                        break;
                    default:
                        filteredData = filteredData.OrderBy(d => d.DEPTCODE);
                        break;
                }

                // Apply paging
                var start = Convert.ToInt32(Request["start"] ?? Request["iDisplayStart"] ?? "0");
                var length = Convert.ToInt32(Request["length"] ?? Request["iDisplayLength"] ?? "10");
                
                var pagedData = filteredData.Skip(start).Take(length).ToList();

                // Format data for DataTables
                var result = pagedData.Select(d => new
                {
                    DEPTCODE = d.DEPTCODE ?? "",
                    DEPTDESC = d.DEPTDESC ?? "",
                    DISPSTATUS = d.DISPSTATUS.ToString(),
                    DEPTID = d.DEPTID
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

        [Authorize(Roles = "DepartmentMasterCreate,DepartmentMasterEdit")]
        public ActionResult Form(int? id = 0)
        {
            DepartmentMaster tab = new DepartmentMaster();
            tab.DEPTID = 0;
            // Default to Enabled for create mode (cast to short to match model)
            tab.DISPSTATUS = (short)0;

            if (id == -1)
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";

            if (id != 0 && id != -1)
            {
                // Use Entity Framework to get the record
                tab = context.DepartmentMasters.Find(id);
                if (tab == null) return HttpNotFound();
                
                // Debug: Let's see what value we actually get from database
                System.Diagnostics.Debug.WriteLine($"Edit Mode - DEPTID: {tab.DEPTID}, DISPSTATUS: {tab.DISPSTATUS}");
            }

            // Clear any existing model state for DISPSTATUS to ensure dropdown works
            ModelState.Remove("DISPSTATUS");
            
            // Create status dropdown using SelectList for proper selection
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Enabled" },
                new SelectListItem { Value = "1", Text = "Disabled" }
            };
            
            ViewBag.DISPSTATUS = new SelectList(statusList, "Value", "Text", tab.DISPSTATUS.ToString());
            
            // Debug: Show what status is being selected
            System.Diagnostics.Debug.WriteLine($"Status dropdown - Current DISPSTATUS: {tab.DISPSTATUS}, Selected Value: {tab.DISPSTATUS.ToString()}");

            // Debug: Let's see what we're passing to the view
            System.Diagnostics.Debug.WriteLine($"Passing to View - DEPTID: {tab.DEPTID}, DISPSTATUS: {tab.DISPSTATUS}");

            return View(tab);
        }

        [HttpPost]
        [Authorize(Roles = "DepartmentMasterCreate,DepartmentMasterEdit")]
        public void savedata(DepartmentMaster tab)
        {
            // Debug: Let's see what we receive from the form
            System.Diagnostics.Debug.WriteLine($"Received from Form - DEPTID: {tab.DEPTID}, DISPSTATUS: {tab.DISPSTATUS}, DEPTCODE: {tab.DEPTCODE}");
            
            var currentUserId = Session["USERID"] != null ? Convert.ToInt32(Session["USERID"]) : 1;
            var prcsdate = DateTime.Now;

            var s = tab.DEPTDESC ?? string.Empty;
            s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());

            if (tab.DEPTID != 0)
            {
                // Fix: For updates, preserve original CUSRID and only update LMUSRID
                var existingRecord = context.Database.SqlQuery<DepartmentMaster>(
                    @"SELECT DEPTID, DEPTCODE, DEPTDESC, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE 
                      FROM DEPARTMENTMASTER WHERE DEPTID = @p0", tab.DEPTID).FirstOrDefault();
                
                if (existingRecord != null)
                {
                    // Update using raw SQL - preserve CUSRID, update LMUSRID
                    context.Database.ExecuteSqlCommand(
                        @"UPDATE DEPARTMENTMASTER 
                          SET DEPTDESC = @p0, DEPTCODE = @p1, LMUSRID = @p2, 
                              DISPSTATUS = @p3, PRCSDATE = @p4 
                          WHERE DEPTID = @p5",
                        s, tab.DEPTCODE, currentUserId, tab.DISPSTATUS, prcsdate, tab.DEPTID);
                }
            }
            else
            {
                // For new records, set both CUSRID and LMUSRID to current user
                var cusrid = Session["CUSRID"] != null ? Session["CUSRID"].ToString() : "admin";
                
                // Insert using raw SQL
                context.Database.ExecuteSqlCommand(
                    @"INSERT INTO DEPARTMENTMASTER (DEPTDESC, DEPTCODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE) 
                      VALUES (@p0, @p1, @p2, @p3, @p4, @p5)",
                    s, tab.DEPTCODE, cusrid, currentUserId, tab.DISPSTATUS, prcsdate);
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
            if (!User.IsInRole("DepartmentMasterDelete"))
            {
                Response.Write("Access Denied: You do not have permission to delete records. Please contact your administrator.");
                return;
            }
            
            string id = Request.Form.Get("id");
            if (string.IsNullOrWhiteSpace(id)) { Response.Write("Invalid Id"); return; }

            // Use raw SQL to match database schema
            var rec = context.Database.SqlQuery<DepartmentMaster>(
                @"SELECT DEPTID, DEPTCODE, DEPTDESC, CUSRID, LMUSRID, 
                         DISPSTATUS, PRCSDATE 
                  FROM DEPARTMENTMASTER 
                  WHERE DEPTID = @p0", Convert.ToInt32(id)
            ).FirstOrDefault();

            if (rec == null) { Response.Write("Record not found"); return; }

            // Delete using raw SQL
            context.Database.ExecuteSqlCommand("DELETE FROM DEPARTMENTMASTER WHERE DEPTID = @p0", Convert.ToInt32(id));

            Response.Write("Deleted Successfully ...");
        }
    }
}
