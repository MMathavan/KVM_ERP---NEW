using ClubMembership.Data;
using KVM_ERP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KVM_ERP;

namespace KVM_ERP.Controllers
{
    public class RolesController : Controller
    {
        // GET: Roles
        private ApplicationDbContext _db = new ApplicationDbContext();
      //  FusionContext context = new FusionContext();
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new ClubMembershipDBEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                var data = e.pr_Search_Roles(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount);
                //var aaData = data.Select(d => new { ACHEADCODE = d.ACHEADCODE, ACHEADGDESC = d.ACHEADGDESC, ACHEADDESC = d.ACHEADDESC, DISPSTATUS = d.DISPSTATUS.ToString(), ACHEADID = d.ACHEADID.ToString() }).ToArray();
                // Map to DataTable format using ACTUAL column names from your stored procedure result
                var aaData = data.Select(d => new
                {
                    RoleName = d.Name ?? string.Empty,
                    Description = d.Descp ?? string.Empty,
                    Department = d.SDPTNAME ?? string.Empty,
                    MenuName = d.RMenuType ?? string.Empty,       // Using RMenuType instead of MenuName
                    ControllerName = d.RControllerName ?? string.Empty, // Using RControllerName
                    MenuIndex = d.MENUDESC ?? string.Empty,      // Using MENUDESC
                    Order = d.RMenuGroupOrder ?? 0               // Using RMenuGroupOrder
                }).ToArray();

                return Json(new
                {
                    //sEcho = param.sEcho,
                    data = aaData
                    //iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    //iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //   [Authorize(Roles = "Admin")]
        //public ActionResult Create(string message = "")
        //{
        //    ViewBag.Message = message;
        //    ViewBag.SDPTID = new SelectList(_db.softdepartmentmasters.OrderBy(x => x.SDPTNAME), "SDPTID", "SDPTNAME");
        //    return View();
        //}


        //[HttpPost]
        //// [Authorize(Roles = "Admin")]
        //public ActionResult Create([Bind(Include =
        //    "RoleName,Description,SDPTID")]AccountViewModels.RoleViewModel model)
        //{
        //    string message = "That role name has already been used";
        //    if (ModelState.IsValid)
        //    {
        //        var role = new ApplicationRole(model.RoleName, model.Description, model.SDPTID);
        //        var idManager = new IdentityManager();

        //        if (idManager.RoleExists(model.RoleName))
        //        {
        //            return View(message);
        //        }
        //        else
        //        {
        //            idManager.CreateRole(model.RoleName, model.SDPTID, model.Description);
        //            return RedirectToAction("Index", "Roles");
        //        }
        //    }
        //    return View();
        //}

        public ActionResult Create(string message = "")
        {
            ViewBag.Message = message;
            ViewBag.SDPTID = new SelectList(_db.softdepartmentmasters.OrderBy(x => x.SDPTNAME), "SDPTID", "SDPTNAME");

            var model = new AccountViewModels.RoleViewModel
            {
                MenuOrderOptions = _db.MenuMasters
                    .OrderBy(m => m.MenuDesc)
                    .AsEnumerable()
                    .Select(m => new SelectListItem
                    {
                        Value = m.MenuId.ToString(),  // Store the ID/short value
                        Text = m.MenuDesc              // Show description
                    }).ToList(),

                RImageOptions = _db.RImageMasters
                    .OrderBy(r => r.RImgCode)
                    .AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Value = r.RImgDesc,
                        Text = r.RImgDesc
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountViewModels.RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Convert the selected value to short
                if (!short.TryParse(model.MenuOrder.ToString(), out short menuOrderValue))
                {
                    ModelState.AddModelError("MenuOrder", "Invalid menu selection");
                    return View(RepopulateDropdowns(model));
                }

                var role = new ApplicationRole(model.RoleName, model.Description, model.SDPTID)
                {
                    RMenuType = model.MenuName,
                    RControllerName = model.ControllerName,
                    RMenuIndex = model.MenuIndex,
                    RMenuGroupId = menuOrderValue,  // Use the converted short value
                    RMenuGroupOrder = model.Order ?? 0,
                    RImageClassName = model.RImage
                };

                var idManager = new IdentityManager();

                if (idManager.RoleExists(model.RoleName))
                {
                    ModelState.AddModelError("RoleName", "That role name has already been used");
                }
                else
                {
                    var result = idManager.CreateRole(role.Name, role.SDPTID, role.Description);

                    var roleInDb = _db.Roles.FirstOrDefault(r => r.Name == role.Name);
                    if (roleInDb != null)
                    {
                        roleInDb.RMenuType = role.RMenuType;
                        roleInDb.RControllerName = role.RControllerName;
                        roleInDb.RMenuIndex = role.RMenuIndex;
                        roleInDb.RMenuGroupId = role.RMenuGroupId;
                        roleInDb.RMenuGroupOrder = role.RMenuGroupOrder;
                        roleInDb.RImageClassName = role.RImageClassName;
                        _db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
            }

            return View(RepopulateDropdowns(model));
        }

        private AccountViewModels.RoleViewModel RepopulateDropdowns(AccountViewModels.RoleViewModel model)
        {
            model.MenuOrderOptions = _db.MenuMasters
                .OrderBy(m => m.MenuDesc)
                .AsEnumerable()
                .Select(m => new SelectListItem
                {
                    Value = m.MenuId.ToString(),  // Store the ID/short value
                    Text = m.MenuDesc              // Show description
                }).ToList();

            model.RImageOptions = _db.RImageMasters
                .OrderBy(r => r.RImgCode)
                .AsEnumerable() // Execute query and work in memory
                .Select(r => new SelectListItem
                {
                    Value = r.RImgDesc,
                    Text = r.RImgDesc
                }).ToList();

            ViewBag.SDPTID = new SelectList(_db.softdepartmentmasters.OrderBy(x => x.SDPTNAME), "SDPTID", "SDPTNAME");
            return model;
        }


        //  [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            // It's actually the Role.Name tucked into the id param:
            var role = _db.Roles.First(r => r.Name == id);
            var roleModel = new AccountViewModels.EditRoleViewModel(role);
            return View(roleModel);
        }


        [HttpPost]
        //  [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include =
            "RoleName,OriginalRoleName,Description")] AccountViewModels.EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = _db.Roles.First(r => r.Name == model.OriginalRoleName);
                role.Name = model.RoleName;
                role.Description = model.Description;
                _db.Entry(role).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = _db.Roles.First(r => r.Name == id);
            var model = new AccountViewModels.RoleViewModel(role);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            var role = _db.Roles.First(r => r.Name == id);
            var idManager = new IdentityManager();
            idManager.DeleteRole(role.Id);
            return RedirectToAction("Index");
        }



    }
}