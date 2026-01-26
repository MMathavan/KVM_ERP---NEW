using KVM_ERP.Models;
using ClubMembership.Data;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
//using ClubMembership.Helper;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KVM_ERP;

namespace KVM_ERP.Controllers.Masters
{
    [SessionExpire]
    public class AccountGroupMasterController : Controller
    {
        // GET: AccountGroupMaster
        ApplicationDbContext context = new ApplicationDbContext();
        [Authorize(Roles = "AccountGroupMasterIndex")]
        public ActionResult Index()
        {
            return View(context.accountgroupmasters.ToList());//---Loading Grid
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new ClubMembershipDBEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_SearchAccountGroup(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);
                var aaData = data.Select(d => new { ACHEADGCODE = d.ACHEADGCODE, ACHEADGDESC = d.ACHEADGDESC, DISPSTATUS = d.DISPSTATUS.ToString(), ACHEADGID = d.ACHEADGID.ToString() }).ToArray();
                return Json(new
                {
                    //sEcho = param.sEcho,
                    data = aaData
                    //iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    //iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //----------------Initializing Form-----------------------//
        [Authorize(Roles = "AccountGroupMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            AccountGroupMaster tab = new AccountGroupMaster();
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            tab.ACHEADGID = 0;

            // IMP
            if (id == -1)
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";


            if (id != 0 && id != -1)  // IMP  
            {

                tab = context.accountgroupmasters.Find(id);
                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem3 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem3);
                    selectedItem3 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem3);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
            }
            return View(tab);
        }//End of Form
         //-------------------Insert or Modify data-------------//

        [Authorize(Roles = "AccountGroupMasterCreate,AccountGroupMasterEdit")]
        public void savedata(AccountGroupMaster tab)
        {

            var s = tab.ACHEADGDESC;//...ProperCase
            s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());//end
            tab.ACHEADGDESC = s;

            tab.CUSRID = Session["CUSRID"].ToString();
            tab.LMUSRID = 1;
            tab.PRCSDATE = DateTime.Now;
            if ((tab.ACHEADGID).ToString() != "0")
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                context.accountgroupmasters.Add(tab);
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




        }//--End of Savedata
         //---------------Delete Record-------------//
        [Authorize(Roles = "AccountGroupMasterDelete")]

        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = "PROCEED";// Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                AccountGroupMaster accountgroupmasters = context.accountgroupmasters.Find(Convert.ToInt32(id));
                context.accountgroupmasters.Remove(accountgroupmasters);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);
        }//.............end delete

        [Authorize(Roles = "AccountGroupMasterEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/AccountGroupMaster/Form/" + id);
            }
        }
        //end class
    }
}