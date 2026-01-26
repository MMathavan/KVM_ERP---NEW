using CHAKRA_ERP.Models;
using CHAKRA_ERP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHAKRA_ERP.Controllers.Masters
{
    //[SessionExpire]
    public class TESTMASTERController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        // GET: TESTMASTER
    
        //[Authorize(Roles = "TESTMasterIndex")]
        public ActionResult Index()
        {
            return View(context.testmasters.ToList());//Loading Grid
        }
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CHAKRA_ERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                var data = e.pr_SearchTestMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);
                var aaData = data.Select(d => new { TESTCODE = d.TESTCODE, TESTDESC = d.TESTDESC, DISPSTATUS = d.DISPSTATUS.ToString(), TESTID = d.TESTID.ToString() }).ToArray();
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
        //[Authorize(Roles = "TestMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            TESTMASTER tab = new TESTMASTER();
            //ViewBag.ACHEADGID = new SelectList(context.accountgroupmasters, "ACHEADGID", "ACHEADGDESC");
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            tab.TESTID = 0;
            // IMP
            if (id == -1)
                ViewBag.msg = "<div class='msg'>Record Successfully Saved</div>";
            if (id != 0 && id != -1)  // IMP
            {
                tab = context.testmasters.Find(id);
                //ViewBag.ACHEADGID = new SelectList(context.accountgroupmasters, "ACHEADGID", "ACHEADGDESC", tab.ACHEADGID);
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
        public void savedata(TESTMASTER tab)
        {
            
            tab.CUSRID = Session["CUSRID"].ToString();
            tab.LMUSRID = 1;
            tab.PRCSDATE = DateTime.Now;
            var s = tab.TESTDESC;//...ProperCase
            s = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());//end
            tab.TESTDESC = s;
            if ((tab.TESTID).ToString() != "0")
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                context.testmasters.Add(tab);
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
        //[Authorize(Roles = "TESTMASTERDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            TESTMASTER testmasters = context.testmasters.Find(Convert.ToInt32(id));
            context.testmasters.Remove(testmasters);
            context.SaveChanges();
            Response.Write("Deleted Successfully ...");
        }
    }//--End of class
}
