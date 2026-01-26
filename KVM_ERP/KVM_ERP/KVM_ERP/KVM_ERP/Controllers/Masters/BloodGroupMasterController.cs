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
    [SessionExpire]
    public class BloodGroupMasterController : Controller
    {
        // GET: StateMaster
        ApplicationDbContext context = new ApplicationDbContext();

        [Authorize(Roles = "BloodGroupMasterIndex")]
        public ActionResult Index()
        {
            return View(context.BloodGroupMasters.ToList());//Loading Grid
        }
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            try
            {
                using (var e = new ClubMembershipDBEntities())
                {
                    var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                    var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                    var data = e.pr_SearchBloodGroupMaster(
                        param.sSearch,
                        Convert.ToInt32(Request["iSortCol_0"]),
                        Request["sSortDir_0"],
                        param.iDisplayStart,
                        param.iDisplayLength,
                        totalRowsCount,
                        filteredRowsCount
                    ).ToList();

                    return Json(new
                    {
                        // DataTables 1.10+ expects this structure
                        draw = param.sEcho,
                        recordsTotal = Convert.ToInt32(totalRowsCount.Value),
                        recordsFiltered = Convert.ToInt32(filteredRowsCount.Value),
                        data = data.Select(d => new
                        {
                            BLDGCODE = d.BLDGCODE,
                            BLDGDESC = d.BLDGDESC,
                            DISPSTATUS = d.DISPSTATUS.ToString(),
                            BLDGID = d.BLDGID.ToString() // Removed .ToArray()
                        })
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Log error
                return Json(new { error = ex.Message });
            }
        }
        //----------------------Initializing Form--------------------------//
        [Authorize(Roles = "BloodGroupMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            BloodGroupMaster tab = new BloodGroupMaster();
            tab.BLDGID = 0;

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;

            // IMP
            if (id == -1)
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";
            if (id != 0 && id != -1)  // IMP
            {
                tab = context.BloodGroupMasters.Find(id);

                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }

            }
            return View(tab);
        }//End of Form

        //--------------------------Insert or Modify data------------------------//
        [HttpPost]
        [Authorize(Roles = "BloodGroupMasterCreate,BloodGroupMasterEdit")]
        public void savedata(BloodGroupMaster tab)
        {
            tab.CUSRID = Session["CUSRID"].ToString();
            tab.LMUSRID = "1";
            tab.PRCSDATE = DateTime.Now;
            var s = tab.BLDGDESC;//...ProperCase
            s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());//end
            tab.BLDGDESC = s;
            if ((tab.BLDGID).ToString() != "0")
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                context.BloodGroupMasters.Add(tab);
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
        [Authorize(Roles = "BloodGroupMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            //    String fld = Request.Form.Get("fld");
            //    String temp = Delete_fun.delete_check1(fld, id);
            //    if (temp.Equals("PROCEED"))
            //    {
            BloodGroupMaster BloodGroupMasters = context.BloodGroupMasters.Find(Convert.ToInt32(id));
            context.BloodGroupMasters.Remove(BloodGroupMasters);
            context.SaveChanges();
            Response.Write("Deleted Successfully ...");
        }


    }
}