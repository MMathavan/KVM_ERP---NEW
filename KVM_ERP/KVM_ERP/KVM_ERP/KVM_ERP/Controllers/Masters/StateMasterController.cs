using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.Diagnostics;
using KVM_ERP.Models;
using ClubMembership.Data;
using System.Configuration;
using KVM_ERP;

namespace KVM_ERP.Controllers.Masters
{
    [KVM_ERP.SessionExpire]
    public class StateMasterController : Controller
    {
        // GET: StateMaster
        ApplicationDbContext context = new ApplicationDbContext();

        [Authorize(Roles = "StateMasterIndex")]
        public ActionResult Index()
        {
            return View(context.StateMasters.ToList());//Loading Grid
        }
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new ClubMembershipDBEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                var data = e.pr_SearchStateMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);

                //var aaData = data.Select(d => new string[] { d.STATECODE, d.STATEDESC, d.STATETYPE, d.DISPSTATUS, d.STATEID.ToString() }).ToArray();
                var aaData = data.Select(d => new { STATECODE = d.STATECODE, STATEDESC = d.STATEDESC, STATETYPE = d.STATETYPE ?? "0", DISPSTATUS = d.DISPSTATUS.ToString(), STATEID = d.STATEID.ToString() }).ToArray();
                return Json(new
                {
                    //sEcho = param.sEcho,
                    data = aaData
                    //iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    //iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //----------------------Initializing Form--------------------------//
        [Authorize(Roles = "StateMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            StateMaster tab = new StateMaster();
            tab.STATEID = 0;

            // Define dropdown lists at method level so they're accessible everywhere
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Enabled" },
                new SelectListItem { Value = "1", Text = "Disabled" }
            };

            var stateTypeList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Local" },
                new SelectListItem { Value = "1", Text = "InterState" }
            };

            ViewBag.REGNID = new SelectList(context.RegionMasters, "REGNID", "REGNDESC");

            // IMP
            if (id == -1)
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";
            
            if (id != null && id > 0)  // Edit mode (same condition as CustomerMaster fix)
            {
                tab = context.StateMasters.Find(id);

                // CRITICAL: Clear ModelState for both dropdowns to ensure proper selection in edit mode
                ModelState.Remove("DISPSTATUS");
                ModelState.Remove("STATETYPE");

                // Debug: Show what values are being read from database
                System.Diagnostics.Debug.WriteLine($"StateMaster Edit - Raw values from DB:");
                System.Diagnostics.Debug.WriteLine($"  DISPSTATUS: '{tab.DISPSTATUS}' (Type: {tab.DISPSTATUS.GetType()})");
                System.Diagnostics.Debug.WriteLine($"  STATETYPE: '{tab.STATETYPE}' (Type: {tab.STATETYPE.GetType()})");
                
                // Status dropdown selection
                var selectedStatusValue = tab.DISPSTATUS.ToString();
                var selectedStateTypeValue = tab.STATETYPE.ToString();
                
                // Handle case where STATETYPE might be stored as text instead of numeric
                if (selectedStateTypeValue == "Local" || selectedStateTypeValue == "local")
                    selectedStateTypeValue = "0";
                else if (selectedStateTypeValue == "InterState" || selectedStateTypeValue == "Interstate" || selectedStateTypeValue == "Inter State")
                    selectedStateTypeValue = "1";
                else if (string.IsNullOrEmpty(selectedStateTypeValue) || selectedStateTypeValue == "0")
                    selectedStateTypeValue = "0"; // Default to Local
                else if (selectedStateTypeValue == "1")
                    selectedStateTypeValue = "1"; // Keep as Interstate
                
                System.Diagnostics.Debug.WriteLine($"  Selected values: DISPSTATUS='{selectedStatusValue}', STATETYPE='{selectedStateTypeValue}' (Original: {tab.STATETYPE})");
                
                // Create dropdowns using SelectList for proper selection
                ViewBag.DISPSTATUS = new SelectList(statusList, "Value", "Text", selectedStatusValue);
                ViewBag.STATETYPE = new SelectList(stateTypeList, "Value", "Text", selectedStateTypeValue);
                
                // Debug: Show what values are being selected
                System.Diagnostics.Debug.WriteLine($"Dropdowns - DISPSTATUS: {selectedStatusValue}, STATETYPE: {selectedStateTypeValue}");
            }
            else  // New record mode
            {
                // Set dropdowns with default values
                ViewBag.DISPSTATUS = new SelectList(statusList, "Value", "Text", "0");
                ViewBag.STATETYPE = new SelectList(stateTypeList, "Value", "Text", "0");
            }
            return View(tab);
        }//End of Form

        //--------------------------Insert or Modify data------------------------//
        [HttpPost]
        [Authorize(Roles = "StateMasterCreate,StateMasterEdit")]
        public void savedata(StateMaster tab)
        {
            tab.CUSRID = Session["CUSRID"].ToString();
            tab.LMUSRID = 1;
            tab.PRCSDATE = DateTime.Now;
            var s = tab.STATEDESC;//...ProperCase
            s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());//end
            tab.STATEDESC = s;
            if ((tab.STATEID).ToString() != "0")
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                context.StateMasters.Add(tab);
                context.SaveChanges();
            }

            // IMP
            if (Request.Form.Get("continue") == null)
            {
                Response.Redirect("index");
            }
            else
            {
                Response.Redirect("Form/-1");
            }
        }

        //------------------------Delete Record----------//
        [HttpPost]
        public ActionResult deletedata(int id)
        {
            try
            {
                // Check if user has delete role
                if (!User.IsInRole("StateMasterDelete"))
                {
                    Response.StatusCode = 403;
                    return Content("Access Denied: You do not have permission to delete records. Please contact your administrator.");
                }
                
                context.Database.ExecuteSqlCommand("DELETE FROM STATEMASTER WHERE STATEID = {0}", id);
                return Content("Deleted Successfully ...");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Content("Error deleting state: " + ex.Message);
            }
        }

    }
}
