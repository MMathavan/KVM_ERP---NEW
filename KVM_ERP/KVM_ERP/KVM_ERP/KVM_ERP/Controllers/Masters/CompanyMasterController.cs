using KVM_ERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClubMembership.Data;
//using ClubMembership.Helper;
using KVM_ERP;

namespace KVM_ERP.Controllers.Masters
{
    [KVM_ERP.SessionExpire]
    public class CompanyMasterController : Controller
    {
        // GET: CompanyMaster
        ApplicationDbContext context = new ApplicationDbContext();
        ClubMembershipDBEntities db = new ClubMembershipDBEntities();

        [Authorize(Roles = "CompanyMasterIndex")]
        public ActionResult Index()
        {
            return View(context.companymasters.ToList());//---Loading Grid
        }

        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new ClubMembershipDBEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_SearchCompanyMaster(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);
                var aaData = data.Select(d => new { COMPCODE = d.COMPCODE, COMPNAME = d.COMPNAME, COMPADDR = d.COMPADDR, COMPPHN1 = d.COMPPHN1, COMPCPRSN = d.COMPCPRSN, COMPMAIL = d.COMPMAIL, COMPID = d.COMPID.ToString() }).ToArray();
                return Json(new
                {
                    //sEcho = param.sEcho,
                    data = aaData
                    //iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    //iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize(Roles = "CompanyMasterEdit")]
        public void Edit(int id)
        {
            Response.Redirect(@Url.Action("NForm", "CompanyMaster") + "/" + id);
        }

        [Authorize(Roles = "CompanyMasterCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            CompanyMaster tab = new CompanyMaster();
            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            tab.COMPID = 0;
            if (id != 0)
            {
                tab = context.companymasters.Find(id);//...........find id
                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItemDSP = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItemDSP);
                    selectedItemDSP = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItemDSP);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
            }
            return View(tab);
        }


        public ActionResult NForm(int? id = 0)
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }

            // Check authorization based on operation type
            if (id == 0 || id == null)
            {
                // ADD operation - require Create permission ONLY
                if (!User.IsInRole("CompanyMasterCreate"))
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                // EDIT operation - require Edit permission ONLY
                if (!User.IsInRole("CompanyMasterEdit"))
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            CompanyMaster tab = new CompanyMaster();

            Company vm = new Company();
            tab = new CompanyMaster();

            List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
            SelectListItem selectedItem = new SelectListItem { Text = "Disabled", Value = "1", Selected = false };
            selectedDISPSTATUS.Add(selectedItem);
            selectedItem = new SelectListItem { Text = "Enabled", Value = "0", Selected = true };
            selectedDISPSTATUS.Add(selectedItem);
            ViewBag.DISPSTATUS = selectedDISPSTATUS;
            tab.COMPID = 0;
            if (id != 0)
            {
                tab = context.companymasters.Find(id);//...........find id
                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) == 1)
                {
                    SelectListItem selectedItemDSP = new SelectListItem { Text = "Disabled", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItemDSP);
                    selectedItemDSP = new SelectListItem { Text = "Enabled", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItemDSP);
                    ViewBag.DISPSTATUS = selectedDISPSTATUS1;
                }
                vm.masterdata = context.companymasters.Where(det => det.COMPID == id).ToList();
                vm.detaildata = context.companydetails.Where(det => det.COMPID == id).ToList();
                //vm.queryresultdata = db.pr_CompanyDetail_Flx_Assgn(id).ToList();

            }
            return View(vm);
        }

        public void savedata(CompanyMaster tab)
        {
            // Check authorization based on operation type
            if (tab.COMPID == 0)
            {
                // ADD operation - require Create permission
                if (!User.IsInRole("CompanyMasterCreate"))
                {
                    Response.Redirect("/Account/Login");
                    return;
                }
            }
            else
            {
                // EDIT operation - require Edit permission
                if (!User.IsInRole("CompanyMasterEdit"))
                {
                    Response.Redirect("/Account/Login");
                    return;
                }
            }

            //if (Session["CUSRID"] != null)
            //    tab.CUSRID = Session["CUSRID"].ToString();
            //else tab.CUSRID = "0";
            tab.LMUSRID = 1;
            tab.PRCSDATE = DateTime.Now;
            if ((tab.COMPID).ToString() != "0")
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                context.companymasters.Add(tab);
                context.SaveChanges();
            }
            Response.Redirect("Index");
        }//..........end

        public string Del(int id)
        {
            // Check DELETE permission
            if (!User.IsInRole("CompanyMasterDelete"))
            {
                return "Access Denied: You do not have permission to delete records. Please contact your administrator.";
            }

            try
            {
                var company = context.companymasters.Find(id);
                if (company != null)
                {
                    context.companymasters.Remove(company);
                    context.SaveChanges();
                    return "Deleted Successfully ...";
                }
                return "Record not found";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Delete error: {ex.Message}");
                return "Error deleting record: " + ex.Message;
            }
        }

        public void nsavedata(FormCollection myfrm)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        CompanyMaster companymaster = new CompanyMaster();
                        CompanyDetail companydetail = new CompanyDetail();
                        Int32 COMPID = Convert.ToInt32(myfrm["masterdata[0].COMPID"]);

                        // Check authorization based on operation type
                        if (COMPID == 0)
                        {
                            // ADD operation - require Create permission
                            if (!User.IsInRole("CompanyMasterCreate"))
                            {
                                Response.Redirect("/Account/Login");
                                return;
                            }
                        }
                        else
                        {
                            // EDIT operation - require Edit permission
                            if (!User.IsInRole("CompanyMasterEdit"))
                            {
                                Response.Redirect("/Account/Login");
                                return;
                            }
                        }
                        Int32 COMPDID = 0;
                        string DELIDS = "";
                        if (COMPID != 0)
                        {
                            companymaster = context.companymasters.Find(COMPID);
                        }
                        companymaster.COMPID = COMPID;
                        companymaster.COMPNAME = myfrm["masterdata[0].COMPNAME"].ToString();
                        companymaster.COMPDNAME = myfrm["masterdata[0].COMPDNAME"].ToString();
                        companymaster.COMPADDR = myfrm["masterdata[0].COMPADDR"].ToString();
                        companymaster.COMPPHNID = "0001";
                        companymaster.COMPPHN1 = myfrm["masterdata[0].COMPPHN1"].ToString();
                        companymaster.COMPPHN2 = myfrm["masterdata[0].COMPPHN2"].ToString();
                        companymaster.COMPPHN3 = myfrm["masterdata[0].COMPPHN3"].ToString();
                        companymaster.COMPPHN4 = myfrm["masterdata[0].COMPPHN4"].ToString();
                        companymaster.COMPMAIL = myfrm["masterdata[0].COMPMAIL"].ToString();
                        companymaster.COMPCPRSN = myfrm["masterdata[0].COMPCPRSN"].ToString();
                       // companymaster.COMPGSTNO = myfrm["masterdata[0].COMPGSTNO"].ToString();
                        companymaster.COMPCODE = myfrm["masterdata[0].COMPCODE"].ToString();
                      //  companymaster.COMPPANNO = myfrm["masterdata[0].COMPPANNO"].ToString();
                      //  companymaster.COMPTANNO = myfrm["masterdata[0].COMPTANNO"].ToString();
                      //  companymaster.STATEID = 0;
                        companymaster.COMPID = Convert.ToInt32(myfrm["masterdata[0].COMPID"]);

                        //if (Session["CUSRID"] != null)
                        //    companymaster.CUSRID = Session["CUSRID"].ToString();

                        companymaster.LMUSRID = 1;
                        companymaster.DISPSTATUS = Convert.ToInt16(myfrm["DISPSTATUS"]);
                        companymaster.PRCSDATE = DateTime.Now;
                        //branchmaster.BRNCHID = Convert.ToInt32(myfrm["masterdata[0].BRNCHID"]);

                        if (COMPID == 0)
                        {
                            companymaster.COMPID = 0;
                            context.companymasters.Add(companymaster);
                            context.SaveChanges();
                            Response.Write("Save<hr>");
                            COMPID = companymaster.COMPID;
                        }

                        else
                        {
                            context.Entry(companymaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            Response.Write("Update <hr>");
                        }//........end
                        //---------------detail data getting form form------------//
                        //string[] A_COMPDID = myfrm.GetValues("COMPDID");
                        //string[] A_COMPPONAME = myfrm.GetValues("COMPPONAME");
                        //string[] A_COMPPOADDR = myfrm.GetValues("COMPPOADDR");
                        //int row = 0;
                        ////--------detail insert and update-------//
                        //foreach (var S_TRANDID in A_COMPDID)
                        //{
                        //    COMPDID = Convert.ToInt32(S_TRANDID);
                        //    if (COMPDID != 0)
                        //    {
                        //        companydetail = context.companydetails.Find(COMPDID);

                        //    }
                        //    companydetail.COMPID = COMPID;
                        //    companydetail.COMPPONAME = A_COMPPONAME[row].ToString();
                        //    companydetail.COMPPOADDR = A_COMPPOADDR[row].ToString();
                        //    if (Convert.ToInt32(COMPDID) == 0)
                        //    {
                        //        context.companydetails.Add(companydetail);
                        //        context.SaveChanges();
                        //        COMPDID = companydetail.COMPDID;
                        //    }
                        //    else
                        //    {
                        //        companydetail.COMPDID = COMPDID;
                        //        context.Entry(companydetail).State = System.Data.Entity.EntityState.Modified;
                        //        context.SaveChanges();
                        //    }//..............end
                        //    //------record ids for delete----------//
                        //    DELIDS = DELIDS + "," + COMPDID.ToString();
                        //    row++;
                        //}
                        ////-------delete transaction master factor-------//
                        ////Transaction Type Master-------//
                        //context.Database.ExecuteSqlCommand("DELETE FROM companydetail  WHERE COMPID=" + COMPID + " and  COMPDID NOT IN(" + DELIDS.Substring(1) + ")");
                        //Response.Write(DELIDS.Substring(1));
                        Response.Redirect("Index");
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback(); Response.Redirect("/Error/SavepointErr");
                    }
                }
            }
        }






    }
}