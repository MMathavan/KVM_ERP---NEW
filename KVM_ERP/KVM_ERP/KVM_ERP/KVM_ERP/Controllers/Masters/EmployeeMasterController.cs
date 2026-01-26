using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using KVM_ERP.Models;

namespace KVM_ERP.Controllers.Masters
{
    [SessionExpire]
    public class EmployeeMasterController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        [Authorize(Roles = "EmployeeMasterIndex")]
        public ActionResult Index()
        {
            try
            {
                var employees = context.Database.SqlQuery<EmployeeMaster>(
                    @"SELECT CATEID, CATETID, CATENAME, CATEADDR1, CATEADDR2, CATEADDR3, CATEADDR4, 
                             CATEPHN1, CATEPHN2, CATEPHN3, CATEPHN4, CATECPNAME, CATEEMAIL, 
                             DEPTID, DSGNID, LOCTID, CATEDOB, CATEDOJ, CATEDOC, CATEDOR, 
                             CATESTATUS, CATECODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE, 
                             REGNID, EMPGRD, ANNUAL_INCENTIVE_CTC, EMPLOYEE_PHOTO 
                      FROM EMPLOYEEMASTER"
                ).ToList();
                
                System.Diagnostics.Debug.WriteLine($"Index: Found {employees.Count} employees");
                foreach (var emp in employees.Take(3)) // Log first 3 employees
                {
                    System.Diagnostics.Debug.WriteLine($"Employee: ID={emp.CATEID}, Name={emp.CATENAME}, Code={emp.CATECODE}");
                }
                
                return View(employees);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Index: {ex.Message}");
                return Content($"Error loading employees: {ex.Message}");
            }
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("GetAjaxData called for DataTables");
                System.Diagnostics.Debug.WriteLine($"Param: iDisplayStart={param?.iDisplayStart}, iDisplayLength={param?.iDisplayLength}, sEcho={param?.sEcho}");
                // Get employees first
                var employees = context.Database.SqlQuery<EmployeeMaster>(
                    @"SELECT CATEID, CATETID, CATENAME, CATEADDR1, CATEADDR2, CATEADDR3, CATEADDR4, 
                             CATEPHN1, CATEPHN2, CATEPHN3, CATEPHN4, CATECPNAME, CATEEMAIL, 
                             DEPTID, DSGNID, LOCTID, CATEDOB, CATEDOJ, CATEDOC, CATEDOR, 
                             CATESTATUS, CATECODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE, 
                             REGNID, EMPGRD, ANNUAL_INCENTIVE_CTC, EMPLOYEE_PHOTO 
                      FROM EMPLOYEEMASTER"
                ).ToList();

                // Get lookup data using Entity Framework
                var departmentLookup = context.DepartmentMasters
                    .Select(d => new { d.DEPTID, d.DEPTDESC })
                    .ToDictionary(d => d.DEPTID, d => d.DEPTDESC ?? "");

                var designationLookup = context.DesignationMasters
                    .Select(d => new { d.DSGNID, d.DSGNDESC })
                    .ToDictionary(d => d.DSGNID, d => d.DSGNDESC ?? "");

                var locationLookup = context.LocationMasters
                    .Select(l => new { l.LOCTID, l.LOCTDESC })
                    .ToDictionary(l => l.LOCTID, l => l.LOCTDESC ?? "");

                // Combine data
                var allEmployees = employees.Select(e => new {
                    CATEID = e.CATEID,
                    CATECODE = e.CATECODE ?? "",
                    CATENAME = e.CATENAME ?? "",
                    CATEEMAIL = e.CATEEMAIL ?? "",
                    DEPTID = e.DEPTID,
                    DSGNID = e.DSGNID,
                    LOCTID = e.LOCTID,
                    CATESTATUS = e.CATESTATUS,
                    DISPSTATUS = e.DISPSTATUS,
                    CUSRID = e.CUSRID ?? "",
                    LMUSRID = e.LMUSRID,
                    PRCSDATE = e.PRCSDATE,
                    DepartmentName = departmentLookup.ContainsKey(e.DEPTID) ? departmentLookup[e.DEPTID] : "",
                    DesignationName = designationLookup.ContainsKey(e.DSGNID) ? designationLookup[e.DSGNID] : "",
                    LocationName = e.LOCTID.HasValue && locationLookup.ContainsKey(e.LOCTID.Value) ? locationLookup[e.LOCTID.Value] : ""
                }).ToList();

                var totalRecords = allEmployees.Count;

                // Apply search filter
                var filteredData = allEmployees.AsQueryable();
                var searchValue = Request["search[value]"] ?? param.sSearch ?? "";
                
                if (!string.IsNullOrEmpty(searchValue))
                {
                    searchValue = searchValue.ToLower();
                    filteredData = filteredData.Where(e =>
                        (e.CATECODE ?? "").ToLower().Contains(searchValue) ||
                        (e.CATENAME ?? "").ToLower().Contains(searchValue) ||
                        (e.CATEEMAIL ?? "").ToLower().Contains(searchValue) ||
                        (e.DepartmentName ?? "").ToLower().Contains(searchValue) ||
                        (e.DesignationName ?? "").ToLower().Contains(searchValue) ||
                        (e.LocationName ?? "").ToLower().Contains(searchValue) ||
                        e.CATEID.ToString().Contains(searchValue)
                    );
                }

                var filteredRecords = filteredData.Count();

                // Apply sorting
                var sortColumnIndex = Convert.ToInt32(Request["order[0][column]"] ?? "0");
                var sortDirection = Request["order[0][dir]"] ?? "asc";
                System.Diagnostics.Debug.WriteLine($"Sorting: Column={sortColumnIndex}, Direction={sortDirection}");

                var sortedData = filteredData.AsEnumerable();

                switch (sortColumnIndex)
                {
                    case 0: 
                        sortedData = sortDirection == "asc" ? sortedData.OrderBy(x => x.CATECODE) : sortedData.OrderByDescending(x => x.CATECODE);
                        break;
                    case 1: 
                        sortedData = sortDirection == "asc" ? sortedData.OrderBy(x => x.CATENAME) : sortedData.OrderByDescending(x => x.CATENAME);
                        break;
                    case 2: 
                        sortedData = sortDirection == "asc" ? sortedData.OrderBy(x => x.CATEEMAIL) : sortedData.OrderByDescending(x => x.CATEEMAIL);
                        break;
                    case 3: 
                        sortedData = sortDirection == "asc" ? sortedData.OrderBy(x => x.DepartmentName) : sortedData.OrderByDescending(x => x.DepartmentName);
                        break;
                    case 4: 
                        sortedData = sortDirection == "asc" ? sortedData.OrderBy(x => x.DesignationName) : sortedData.OrderByDescending(x => x.DesignationName);
                        break;
                    case 5: 
                        sortedData = sortDirection == "asc" ? sortedData.OrderBy(x => x.LocationName) : sortedData.OrderByDescending(x => x.LocationName);
                        break;
                    case 6: 
                        sortedData = sortDirection == "asc" ? sortedData.OrderBy(x => x.CATESTATUS) : sortedData.OrderByDescending(x => x.CATESTATUS);
                        break;
                    case 7: 
                        sortedData = sortDirection == "asc" ? sortedData.OrderBy(x => x.DISPSTATUS) : sortedData.OrderByDescending(x => x.DISPSTATUS);
                        break;
                    default: 
                        sortedData = sortDirection == "asc" ? sortedData.OrderBy(x => x.CATEID) : sortedData.OrderByDescending(x => x.CATEID);
                        break;
                }

                // Apply pagination with fallback values
                int skip = param?.iDisplayStart ?? 0;
                int take = param?.iDisplayLength ?? 25;
                System.Diagnostics.Debug.WriteLine($"Pagination: Skip={skip}, Take={take}, SortedData Count={sortedData.Count()}");
                var displayData = sortedData.Skip(skip).Take(take).ToList();
                System.Diagnostics.Debug.WriteLine($"DisplayData Count after pagination: {displayData.Count}");

                var result = Json(
                    new
                    {
                        sEcho = param?.sEcho ?? "1",
                        iTotalRecords = totalRecords,
                        iTotalDisplayRecords = filteredRecords,
                        aaData = displayData
                    }, JsonRequestBehavior.AllowGet
                );
                
                System.Diagnostics.Debug.WriteLine($"GetAjaxData returning: {totalRecords} total records, {filteredRecords} filtered records, {displayData.Count} display records");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAjaxData: {ex.Message}");
                return Json(new { error = "Error loading data" }, JsonRequestBehavior.AllowGet);
            }
        }

        // Simple AJAX method without DataTables complexity
        public JsonResult GetSimpleData()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("GetSimpleData called for Employee Master");
                
                // Try to get employees using Entity Framework first, then fallback to raw SQL
                var employees = new List<EmployeeMaster>();
                
                try
                {
                    // Try Entity Framework first
                    employees = context.EmployeeMasters.ToList();
                    System.Diagnostics.Debug.WriteLine($"Retrieved {employees.Count} employees using Entity Framework");
                }
                catch (Exception efEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Entity Framework failed: {efEx.Message}, trying raw SQL");
                    
                    try
                    {
                        // Fallback to raw SQL
                        employees = context.Database.SqlQuery<EmployeeMaster>(
                            @"SELECT CATEID, CATETID, CATENAME, CATEADDR1, CATEADDR2, CATEADDR3, CATEADDR4, 
                                     CATEPHN1, CATEPHN2, CATEPHN3, CATEPHN4, CATECPNAME, CATEEMAIL, 
                                     DEPTID, DSGNID, LOCTID, CATEDOB, CATEDOJ, CATEDOC, CATEDOR, 
                                     CATESTATUS, CATECODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE, 
                                     REGNID, EMPGRD, ANNUAL_INCENTIVE_CTC, EMPLOYEE_PHOTO 
                              FROM EMPLOYEEMASTER"
                        ).ToList();
                        System.Diagnostics.Debug.WriteLine($"Retrieved {employees.Count} employees using raw SQL");
                    }
                    catch (Exception sqlEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Raw SQL also failed: {sqlEx.Message} - returning empty list");
                        // Return empty result if table doesn't exist
                        return Json(new { 
                            success = true, 
                            data = new List<object>(),
                            count = 0,
                            message = "No employees found - table may not exist yet"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                // Get lookup data with error handling
                var departmentLookup = new Dictionary<int, string>();
                var designationLookup = new Dictionary<int, string>();
                var locationLookup = new Dictionary<int, string>();
                
                try
                {
                    departmentLookup = context.DepartmentMasters
                        .Select(d => new { d.DEPTID, d.DEPTDESC })
                        .ToDictionary(d => d.DEPTID, d => d.DEPTDESC ?? "");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Department lookup failed: {ex.Message}");
                }
                
                try
                {
                    designationLookup = context.DesignationMasters
                        .Select(d => new { d.DSGNID, d.DSGNDESC })
                        .ToDictionary(d => d.DSGNID, d => d.DSGNDESC ?? "");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Designation lookup failed: {ex.Message}");
                }
                
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

                // Combine data with lookup names
                var result = employees.Select(e => new {
                    CATEID = e.CATEID,
                    CATECODE = e.CATECODE ?? "",
                    CATENAME = e.CATENAME ?? "",
                    CATEEMAIL = e.CATEEMAIL ?? "",
                    DEPTID = e.DEPTID,
                    DSGNID = e.DSGNID,
                    LOCTID = e.LOCTID,
                    CATESTATUS = e.CATESTATUS,
                    DISPSTATUS = e.DISPSTATUS,
                    CUSRID = e.CUSRID ?? "",
                    LMUSRID = e.LMUSRID,
                    PRCSDATE = e.PRCSDATE,
                    DepartmentName = departmentLookup.ContainsKey(e.DEPTID) ? departmentLookup[e.DEPTID] : "Unknown",
                    DesignationName = designationLookup.ContainsKey(e.DSGNID) ? designationLookup[e.DSGNID] : "Unknown",
                    LocationName = e.LOCTID.HasValue && locationLookup.ContainsKey(e.LOCTID.Value) ? locationLookup[e.LOCTID.Value] : "Unknown"
                }).ToList();
                
                System.Diagnostics.Debug.WriteLine($"GetSimpleData returning {result.Count} employees with lookup names");
                
                return Json(new { 
                    success = true, 
                    data = result,
                    count = result.Count
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetSimpleData: {ex.Message}");
                return Json(new { 
                    success = false, 
                    error = ex.Message,
                    data = new List<object>(),
                    count = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // Simple test for dropdown issue
        public ActionResult TestDropdown(int id)
        {
            try
            {
                var employee = context.EmployeeMasters.Find(id);
                if (employee == null) return Content("Employee not found");
                
                var result = $"<h3>Employee {id} Dropdown Test</h3>";
                result += $"Employee: {employee.CATENAME}<br/>";
                result += $"DEPTID: {employee.DEPTID}<br/>";
                result += $"DSGNID: {employee.DSGNID}<br/>";
                result += $"LOCTID: {employee.LOCTID}<br/><br/>";
                
                // Test Department
                var dept = context.DepartmentMasters.Where(d => d.DEPTID == employee.DEPTID).FirstOrDefault();
                result += $"Department: {(dept != null ? dept.DEPTDESC : "NOT FOUND")}<br/>";
                
                // Test Designation  
                var dsgn = context.DesignationMasters.Where(d => d.DSGNID == employee.DSGNID).FirstOrDefault();
                result += $"Designation: {(dsgn != null ? dsgn.DSGNDESC : "NOT FOUND")}<br/>";
                
                // Test Location
                var loct = employee.LOCTID.HasValue ? context.LocationMasters.Where(l => l.LOCTID == employee.LOCTID.Value).FirstOrDefault() : null;
                result += $"Location: {(loct != null ? loct.LOCTDESC : "NOT FOUND")}<br/><br/>";
                
                // Test dropdown creation
                result += $"<h4>Dropdown Test:</h4>";
                
                var depts = context.DepartmentMasters.ToList();
                result += $"Total Departments: {depts.Count}<br/>";
                foreach (var d in depts.Take(3))
                {
                    bool selected = (d.DEPTID == employee.DEPTID);
                    result += $"Dept {d.DEPTID}: {d.DEPTDESC} - Selected: {selected}<br/>";
                }
                
                return Content(result);
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }
        
        // Test method to check data directly
        public ActionResult TestData(int? id = null)
        {
            try
            {
                var result = "<h3>Employee Master Test Data</h3>";
                
                if (id.HasValue)
                {
                    // Test specific employee
                    var employee = context.Database.SqlQuery<EmployeeMaster>(
                        @"SELECT CATEID, CATETID, CATENAME, CATEADDR1, CATEADDR2, CATEADDR3, CATEADDR4, 
                                 CATEPHN1, CATEPHN2, CATEPHN3, CATEPHN4, CATECPNAME, CATEEMAIL, 
                                 DEPTID, DSGNID, LOCTID, CATEDOB, CATEDOJ, CATEDOC, CATEDOR, 
                                 CATESTATUS, CATECODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE, 
                                 REGNID, EMPGRD, ANNUAL_INCENTIVE_CTC, EMPLOYEE_PHOTO 
                          FROM EMPLOYEEMASTER WHERE CATEID = {0}", id.Value
                    ).FirstOrDefault();
                    
                    if (employee != null)
                    {
                        result += $"<h4>Employee {employee.CATEID} Details:</h4>";
                        result += $"Name: {employee.CATENAME}<br/>";
                        result += $"Code: {employee.CATECODE}<br/>";
                        result += $"<strong>DEPTID: {employee.DEPTID}</strong><br/>";
                        result += $"<strong>DSGNID: {employee.DSGNID}</strong><br/>";
                        result += $"<strong>LOCTID: {employee.LOCTID}</strong><br/><br/>";
                        
                        // Check if these IDs exist in master tables
                        var dept = context.DepartmentMasters.Where(d => d.DEPTID == employee.DEPTID).FirstOrDefault();
                        var dsgn = context.DesignationMasters.Where(d => d.DSGNID == employee.DSGNID).FirstOrDefault();
                        var loct = employee.LOCTID.HasValue ? context.LocationMasters.Where(l => l.LOCTID == employee.LOCTID.Value).FirstOrDefault() : null;
                        
                        result += $"<h4>Master Table Lookups:</h4>";
                        result += $"Department: {(dept != null ? $"{dept.DEPTDESC} (Status: {dept.DISPSTATUS})" : "NOT FOUND")}<br/>";
                        result += $"Designation: {(dsgn != null ? $"{dsgn.DSGNDESC} (Status: {dsgn.DISPSTATUS})" : "NOT FOUND")}<br/>";
                        result += $"Location: {(loct != null ? $"{loct.LOCTDESC} (Status: {loct.DISPSTATUS})" : (employee.LOCTID.HasValue ? "NOT FOUND" : "NULL"))}<br/><br/>";
                    }
                    else
                    {
                        result += $"Employee with ID {id.Value} not found.<br/>";
                    }
                }
                
                // Show all employees
                var employees = context.Database.SqlQuery<EmployeeMaster>(
                    @"SELECT CATEID, CATETID, CATENAME, CATEADDR1, CATEADDR2, CATEADDR3, CATEADDR4, 
                             CATEPHN1, CATEPHN2, CATEPHN3, CATEPHN4, CATECPNAME, CATEEMAIL, 
                             DEPTID, DSGNID, LOCTID, CATEDOB, CATEDOJ, CATEDOC, CATEDOR, 
                             CATESTATUS, CATECODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE, 
                             REGNID, EMPGRD, ANNUAL_INCENTIVE_CTC, EMPLOYEE_PHOTO 
                      FROM EMPLOYEEMASTER"
                ).ToList();
                
                result += $"<h4>All Employees ({employees.Count} found):</h4>";
                foreach (var emp in employees.Take(10))
                {
                    result += $"<strong>Employee {emp.CATEID}:</strong> {emp.CATENAME} - DEPTID:{emp.DEPTID}, DSGNID:{emp.DSGNID}, LOCTID:{emp.LOCTID}<br/>";
                }
                
                // Also check master tables
                var deptCount = context.DepartmentMasters.Count();
                var dsgnCount = context.DesignationMasters.Count();
                var loctCount = context.LocationMasters.Count();
                
                result += $"<br/><h4>Master Tables:</h4>";
                result += $"Departments: {deptCount}, Designations: {dsgnCount}, Locations: {loctCount}<br/>";
                
                // Show some departments
                var depts = context.DepartmentMasters.Take(5).ToList();
                result += $"<br/><strong>Sample Departments:</strong><br/>";
                foreach (var d in depts)
                {
                    result += $"ID: {d.DEPTID}, Name: {d.DEPTDESC}, Status: {d.DISPSTATUS}<br/>";
                }
                
                result += $"<br/><strong>Usage:</strong> /EmployeeMaster/TestData/[EmployeeID] to test specific employee";
                
                return Content(result);
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}<br/><br/>Stack: {ex.StackTrace}");
            }
        }

        [Authorize(Roles = "EmployeeMasterCreate,EmployeeMasterEdit")]
        public ActionResult Form(int? id)
        {
            try
            {
                EmployeeMaster tab = new EmployeeMaster();
                ViewBag.msg = "Add New Employee";

                if (id == null)
                {
                    tab.CATEID = 0;
                    // Set default values
                    tab.CATETID = 1;
                    tab.REGNID = 1;
                    tab.EMPGRD = null;
                    tab.DISPSTATUS = (short)0; // Default to Enabled
                    tab.CATESTATUS = (short)0; // Default to On Roll
                }

                if (id == -1)
                    ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";

                if (id != 0 && id != -1 && id != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Form: Loading employee with ID: {id}");
                    
                    try
                    {
                        // Use Entity Framework Find method for better reliability
                        tab = context.EmployeeMasters.Find(id);
                        
                        if (tab == null) 
                        {
                            System.Diagnostics.Debug.WriteLine($"Employee with ID {id} not found in database");
                            return HttpNotFound();
                        }
                        
                        // Debug logging - detailed employee data
                        System.Diagnostics.Debug.WriteLine($"Edit Employee - ID: {tab.CATEID}, Name: {tab.CATENAME}");
                        System.Diagnostics.Debug.WriteLine($"Department ID: {tab.DEPTID}, Designation ID: {tab.DSGNID}, Location ID: {tab.LOCTID}");
                        System.Diagnostics.Debug.WriteLine($"Status: {tab.DISPSTATUS}, Employee Status: {tab.CATESTATUS}");
                        
                        ViewBag.msg = "Edit Employee";
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading employee {id}: {ex.Message}");
                        // Fallback to new employee if loading fails
                        tab = new EmployeeMaster
                        {
                            CATEID = 0,
                            CATETID = 1,
                            REGNID = 1,
                            EMPGRD = null,
                            DISPSTATUS = (short)0,
                            CATESTATUS = (short)0
                        };
                        ViewBag.msg = $"Error loading employee {id}. Creating new instead.";
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

                // Create employee status dropdown using SelectList for proper selection
                var empStatusList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "0", Text = "On Roll" },
                    new SelectListItem { Value = "1", Text = "Expired" },
                    new SelectListItem { Value = "2", Text = "Retired" }
                };
                ViewBag.CATESTATUS = new SelectList(empStatusList, "Value", "Text", tab.CATESTATUS.ToString());
                
                // Debug: Show what employee status is being selected
                System.Diagnostics.Debug.WriteLine($"Employee Status dropdown - Current CATESTATUS: {tab.CATESTATUS}, Selected Value: {tab.CATESTATUS.ToString()}");

                // Get departments for dropdown with error handling
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Loading departments for employee DEPTID: {tab.DEPTID}");
                    
                    var departments = context.DepartmentMasters.ToList();
                    var deptList = new List<SelectListItem>();
                    deptList.Add(new SelectListItem { Value = "", Text = "-- Select Department --" });
                    
                    // Add only enabled departments - NO disabled items at all
                    foreach (var dept in departments.Where(d => d.DISPSTATUS == 0).OrderBy(d => d.DEPTDESC))
                    {
                        deptList.Add(new SelectListItem
                        {
                            Value = dept.DEPTID.ToString(),
                            Text = dept.DEPTDESC
                        });
                    }
                    
                    // If employee has disabled department, reset to empty selection
                    var selectedDeptId = "";
                    if (tab.DEPTID != 0 && deptList.Any(x => x.Value == tab.DEPTID.ToString()))
                    {
                        selectedDeptId = tab.DEPTID.ToString();
                    }
                    else if (tab.DEPTID != 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Employee has disabled department ID {tab.DEPTID} - resetting to empty");
                    }
                    
                    ViewBag.DEPTID = new SelectList(deptList, "Value", "Text", selectedDeptId);
                    System.Diagnostics.Debug.WriteLine($"Department dropdown created with {deptList.Count} items, selected: {tab.DEPTID}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading departments: {ex.Message}");
                    ViewBag.DEPTID = new SelectList(new List<SelectListItem> 
                    { 
                        new SelectListItem { Value = "", Text = "-- Select Department --" }
                    }, "Value", "Text");
                }

                // Get designations for dropdown with error handling
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Loading designations for employee DSGNID: {tab.DSGNID}");
                    
                    var designations = context.DesignationMasters.ToList();
                    var dsgnList = new List<SelectListItem>();
                    dsgnList.Add(new SelectListItem { Value = "", Text = "-- Select Designation --" });
                    
                    // Add only enabled designations - NO disabled items at all
                    foreach (var dsgn in designations.Where(d => d.DISPSTATUS == 0).OrderBy(d => d.DSGNDESC))
                    {
                        dsgnList.Add(new SelectListItem
                        {
                            Value = dsgn.DSGNID.ToString(),
                            Text = dsgn.DSGNDESC
                        });
                    }
                    
                    // If employee has disabled designation, reset to empty selection
                    var selectedDsgnId = "";
                    if (tab.DSGNID != 0 && dsgnList.Any(x => x.Value == tab.DSGNID.ToString()))
                    {
                        selectedDsgnId = tab.DSGNID.ToString();
                    }
                    else if (tab.DSGNID != 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Employee has disabled designation ID {tab.DSGNID} - resetting to empty");
                    }
                    
                    ViewBag.DSGNID = new SelectList(dsgnList, "Value", "Text", selectedDsgnId);
                    System.Diagnostics.Debug.WriteLine($"Designation dropdown created with {dsgnList.Count} items, selected: {tab.DSGNID}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading designations: {ex.Message}");
                    ViewBag.DSGNID = new SelectList(new List<SelectListItem> 
                    { 
                        new SelectListItem { Value = "", Text = "-- Select Designation --" }
                    }, "Value", "Text");
                }

                // Get locations for dropdown with error handling
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Loading locations for employee LOCTID: {tab.LOCTID}");
                    
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
                    
                    // If employee has disabled location, reset to empty selection
                    var selectedLoctId = "";
                    if (tab.LOCTID.HasValue && loctList.Any(x => x.Value == tab.LOCTID.Value.ToString()))
                    {
                        selectedLoctId = tab.LOCTID.Value.ToString();
                    }
                    else if (tab.LOCTID.HasValue)
                    {
                        System.Diagnostics.Debug.WriteLine($"Employee has disabled location ID {tab.LOCTID.Value} - resetting to empty");
                    }
                    
                    ViewBag.LOCTID = new SelectList(loctList, "Value", "Text", selectedLoctId);
                    System.Diagnostics.Debug.WriteLine($"Location dropdown created with {loctList.Count} items, selected: {selectedLoctId}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading locations: {ex.Message}");
                    ViewBag.LOCTID = new SelectList(new List<SelectListItem> 
                    { 
                        new SelectListItem { Value = "", Text = "-- Select Location --" }
                    }, "Value", "Text");
                }

                return View(tab);
            }
            catch (Exception ex)
            {
                // If there's an error, return a detailed error message
                return Content($"Error in Form action: {ex.Message}<br/><br/>Stack Trace: {ex.StackTrace}");
            }
        }


        [HttpPost]
        [Authorize(Roles = "EmployeeMasterCreate,EmployeeMasterEdit")]
        public void savedata(EmployeeMaster tab)
        {
            try
            {
                // Debug logging
                System.Diagnostics.Debug.WriteLine($"savedata called - CATEID: {tab.CATEID}, CATENAME: {tab.CATENAME}, CATECODE: {tab.CATECODE}");
                
                var currentUserId = Session["USERID"] != null ? Convert.ToInt32(Session["USERID"]) : 1;
                var prcsdate = DateTime.Now;

                // Handle Employee Photo upload and removal
                try
                {
                    // Check if photo should be removed
                    var removePhoto = Request.Form["REMOVE_PHOTO"];
                    if (!string.IsNullOrEmpty(removePhoto) && removePhoto == "true")
                    {
                        // Delete existing photo file if it exists
                        if (!string.IsNullOrEmpty(tab.EMPLOYEE_PHOTO))
                        {
                            var existingPhotoPath = Server.MapPath("~" + tab.EMPLOYEE_PHOTO);
                            if (System.IO.File.Exists(existingPhotoPath))
                            {
                                try
                                {
                                    System.IO.File.Delete(existingPhotoPath);
                                    System.Diagnostics.Debug.WriteLine($"Deleted existing photo: {existingPhotoPath}");
                                }
                                catch (Exception delEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error deleting photo: {delEx.Message}");
                                }
                            }
                        }
                        // Clear the photo field
                        tab.EMPLOYEE_PHOTO = null;
                    }
                    else
                    {
                        // Handle new photo upload
                        var file = Request?.Files?["EMPLOYEE_PHOTO_FILE"];
                        if (file != null && file.ContentLength > 0)
                        {
                            var allowedExts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".pdf" };
                            var ext = Path.GetExtension(file.FileName);
                            if (!allowedExts.Contains(ext))
                            {
                                throw new InvalidOperationException("Invalid file type. Allowed types: .jpg, .jpeg, .png, .pdf");
                            }

                            // Validate file size (5MB max)
                            if (file.ContentLength > 5 * 1024 * 1024)
                            {
                                throw new InvalidOperationException("File size must be less than 5MB");
                            }

                            // Ensure directory exists
                            var uploadsDir = Server.MapPath("~/Uploads/EmployeePhotos");
                            if (!Directory.Exists(uploadsDir))
                            {
                                Directory.CreateDirectory(uploadsDir);
                            }

                            // Delete old photo if exists and uploading new one
                            if (!string.IsNullOrEmpty(tab.EMPLOYEE_PHOTO))
                            {
                                var oldPhotoPath = Server.MapPath("~" + tab.EMPLOYEE_PHOTO);
                                if (System.IO.File.Exists(oldPhotoPath))
                                {
                                    try
                                    {
                                        System.IO.File.Delete(oldPhotoPath);
                                        System.Diagnostics.Debug.WriteLine($"Deleted old photo: {oldPhotoPath}");
                                    }
                                    catch (Exception delEx)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Error deleting old photo: {delEx.Message}");
                                    }
                                }
                            }

                            // Generate unique filename
                            var safeName = Path.GetFileNameWithoutExtension(file.FileName);
                            var uniqueName = $"{safeName}_{DateTime.Now:yyyyMMddHHmmssfff}{ext}";
                            var fullPath = Path.Combine(uploadsDir, uniqueName);
                            file.SaveAs(fullPath);

                            // Store app-relative path for serving
                            tab.EMPLOYEE_PHOTO = $"/Uploads/EmployeePhotos/{uniqueName}";
                            System.Diagnostics.Debug.WriteLine($"Uploaded new photo: {tab.EMPLOYEE_PHOTO}");
                        }
                        // else: if no new file uploaded, keep the existing EMPLOYEE_PHOTO coming from hidden input
                    }
                }
                catch (Exception exUpload)
                {
                    // Log upload error but continue to save other data
                    System.Diagnostics.Debug.WriteLine($"Photo handling error: {exUpload.Message}");
                }

                if (tab.CATEID == 0)
            {
                // Create new record
                tab.CUSRID = Session["USERNAME"]?.ToString() ?? "admin";
                tab.LMUSRID = currentUserId;
                tab.PRCSDATE = prcsdate;
                
                // Set default values if not provided
                if (tab.CATETID == 0) tab.CATETID = 1;
                if (tab.REGNID == 0) tab.REGNID = 1;

                context.Database.ExecuteSqlCommand(
                    @"INSERT INTO EMPLOYEEMASTER 
                      (CATETID, CATENAME, CATEADDR1, CATEADDR2, CATEADDR3, CATEADDR4, 
                       CATEPHN1, CATEPHN2, CATEPHN3, CATEPHN4, CATECPNAME, CATEEMAIL, 
                       DEPTID, DSGNID, LOCTID, CATEDOB, CATEDOJ, CATEDOC, CATEDOR, 
                       CATESTATUS, CATECODE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE, 
                       REGNID, EMPGRD, ANNUAL_INCENTIVE_CTC, EMPLOYEE_PHOTO) 
                      VALUES 
                      ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, 
                       {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, 
                       {25}, {26}, {27}, {28})",
                    tab.CATETID, tab.CATENAME, tab.CATEADDR1, tab.CATEADDR2, tab.CATEADDR3, tab.CATEADDR4,
                    tab.CATEPHN1, tab.CATEPHN2, tab.CATEPHN3, tab.CATEPHN4, tab.CATECPNAME, tab.CATEEMAIL,
                    tab.DEPTID, tab.DSGNID, tab.LOCTID, tab.CATEDOB, tab.CATEDOJ, tab.CATEDOC, tab.CATEDOR,
                    tab.CATESTATUS, tab.CATECODE, tab.CUSRID, tab.LMUSRID, tab.DISPSTATUS, tab.PRCSDATE,
                    tab.REGNID, tab.EMPGRD, tab.ANNUAL_INCENTIVE_CTC, tab.EMPLOYEE_PHOTO
                );
            }
            else
            {
                // Update existing record - preserve CUSRID, update LMUSRID
                tab.LMUSRID = currentUserId;
                tab.PRCSDATE = prcsdate;

                context.Database.ExecuteSqlCommand(
                    @"UPDATE EMPLOYEEMASTER SET 
                      CATETID = {1}, CATENAME = {2}, CATEADDR1 = {3}, CATEADDR2 = {4}, CATEADDR3 = {5}, CATEADDR4 = {6}, 
                      CATEPHN1 = {7}, CATEPHN2 = {8}, CATEPHN3 = {9}, CATEPHN4 = {10}, CATECPNAME = {11}, CATEEMAIL = {12}, 
                      DEPTID = {13}, DSGNID = {14}, LOCTID = {15}, CATEDOB = {16}, CATEDOJ = {17}, CATEDOC = {18}, CATEDOR = {19}, 
                      CATESTATUS = {20}, CATECODE = {21}, LMUSRID = {22}, DISPSTATUS = {23}, PRCSDATE = {24}, 
                      REGNID = {25}, EMPGRD = {26}, ANNUAL_INCENTIVE_CTC = {27}, EMPLOYEE_PHOTO = {28} 
                      WHERE CATEID = {0}",
                    tab.CATEID, tab.CATETID, tab.CATENAME, tab.CATEADDR1, tab.CATEADDR2, tab.CATEADDR3, tab.CATEADDR4,
                    tab.CATEPHN1, tab.CATEPHN2, tab.CATEPHN3, tab.CATEPHN4, tab.CATECPNAME, tab.CATEEMAIL,
                    tab.DEPTID, tab.DSGNID, tab.LOCTID, tab.CATEDOB, tab.CATEDOJ, tab.CATEDOC, tab.CATEDOR,
                    tab.CATESTATUS, tab.CATECODE, tab.LMUSRID, tab.DISPSTATUS, tab.PRCSDATE,
                    tab.REGNID, tab.EMPGRD, tab.ANNUAL_INCENTIVE_CTC, tab.EMPLOYEE_PHOTO
                );
            }

                System.Diagnostics.Debug.WriteLine("Employee saved successfully, redirecting to Index");
                Response.Redirect("~/EmployeeMaster/Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving employee: {ex.Message}");
                // For now, still redirect to Index even if there's an error
                Response.Redirect("~/EmployeeMaster/Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "EmployeeMasterDelete")]
        public ActionResult deletedata(int id)
        {
            try
            {
                context.Database.ExecuteSqlCommand("DELETE FROM EMPLOYEEMASTER WHERE CATEID = {0}", id);
                return Content("Deleted Successfully ...");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Content("Error deleting employee: " + ex.Message);
            }
        }
    }
}
