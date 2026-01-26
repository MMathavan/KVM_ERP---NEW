using KVM_ERP.Models;
using ClubMembership.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using KVM_ERP;

namespace KVM_ERP.Controllers.Masters
{
    [SessionExpire]
    public class AccountHeadMasterController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        // GET: /AccountHeadMaster/
        [Authorize(Roles = "AccountHeadMasterIndex")]
        public ActionResult Index()
        {
            return View(context.accountheadmasters.ToList());//Loading Grid
        }
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new ClubMembershipDBEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                var data = e.pr_SearchAccountHeadMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);
                var aaData = data.Select(d => new { ACHEADCODE = d.ACHEADCODE, ACHEADGDESC = d.ACHEADGDESC, ACHEADDESC = d.ACHEADDESC, DISPSTATUS = d.DISPSTATUS.ToString(), ACHEADID = d.ACHEADID.ToString() }).ToArray();
                return Json(new
                {
                    //sEcho = param.sEcho,
                    data = aaData
                    //iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    //iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //-------------Initializing Form-------------//
        [Authorize(Roles = "AccountHeadMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            AccountHeadMaster tab = new AccountHeadMaster();
            ViewBag.ACHEADGID = new SelectList(context.accountgroupmasters, "ACHEADGID", "ACHEADGDESC");
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            tab.ACHEADID = 0;
            // IMP
            if (id == -1)
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";
            if (id != 0 && id != -1)  // IMP
            {
                tab = context.accountheadmasters.Find(id);
                ViewBag.ACHEADGID = new SelectList(context.accountgroupmasters, "ACHEADGID", "ACHEADGDESC", tab.ACHEADGID);
                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItem1 = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem1);
                    selectedItem1 = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem1);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
            }
            return View(tab);
        }//--End of Form
         //-----------------Imsert or Modify data------------------//
        [Authorize(Roles = "AccountHeadMasterCreate,AccountHeadMasterEdit")]
        public void savedata(AccountHeadMaster tab)
        {
            tab.CUSRID = Session["CUSRID"].ToString();
            tab.LMUSRID = 1;
            tab.PRCSDATE = DateTime.Now;
            var s = tab.ACHEADDESC;//...ProperCase
            s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());//end
            tab.ACHEADDESC = s;
            if ((tab.ACHEADID).ToString() != "0")
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                context.accountheadmasters.Add(tab);
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

        }//---------End  
        //-----------------------------Delete Record---//
        [Authorize(Roles = "AccountHeadMasterDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            AccountHeadMaster accountheadmasters = context.accountheadmasters.Find(Convert.ToInt32(id));
            context.accountheadmasters.Remove(accountheadmasters);
            context.SaveChanges();
            Response.Write("Deleted Successfully ...");
        }

        [Authorize(Roles = "AccountHeadMasterEdit")]
        public void Edit(int? id = 0)
        {
            if (id > 0)
            {
                var strPath = ConfigurationManager.AppSettings["BaseURL"];

                Response.Redirect("" + strPath + "/AccountHeadMaster/Form/" + id);
            }
        }
    }//--End of class
}