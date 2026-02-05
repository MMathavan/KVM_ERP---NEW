using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using KVM_ERP.Models;

namespace KVM_ERP.Controllers
{
    [SessionExpire]
    public class IssuedStockController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: IssuedStock
        [Authorize(Roles = "IssuedStockIndex")]
        public ActionResult Index()
        {
            ViewBag.Title = "Issued Stock";
            return View();
        }

        // GET: IssuedStock/Form/{id}
        // Basic stub for create/edit screen. Extend with your business logic as needed.
        [Authorize(Roles = "IssuedStockCreate,IssuedStockEdit")]
        public ActionResult Form(int? id)
        {
            ViewBag.Title = id.HasValue ? "Edit Issued Stock" : "Add Issued Stock";

            var model = new TransactionMaster
            {
                TRANDATE = DateTime.Today
            };

            // Reuse the same master data as Opening Stock form
            var materialGroups = db.MaterialGroupMasters
                .Where(g => g.DISPSTATUS == 0 || g.DISPSTATUS == null)
                .OrderBy(g => g.MTRLGDESC)
                .Select(g => new SelectListItem
                {
                    Text = g.MTRLGDESC,
                    Value = g.MTRLGID.ToString()
                })
                .ToList();

            ViewBag.MaterialGroups = materialGroups;

            var grades = db.GradeMasters
                .Where(g => g.DISPSTATUS == 0 || g.DISPSTATUS == null)
                .OrderBy(g => g.GRADEDESC)
                .Select(g => new SelectListItem
                {
                    Text = g.GRADEDESC,
                    Value = g.GRADEID.ToString()
                })
                .ToList();

            ViewBag.Grades = grades;

            var colours = db.ProductionColourMasters
                .Where(c => c.DISPSTATUS == 0 || c.DISPSTATUS == null)
                .OrderBy(c => c.PCLRDESC)
                .Select(c => new SelectListItem
                {
                    Text = c.PCLRDESC,
                    Value = c.PCLRID.ToString()
                })
                .ToList();

            ViewBag.ProductionColours = colours;

            var receivedTypes = db.ReceivedTypeMasters
                .Where(r => r.DISPSTATUS == 0 || r.DISPSTATUS == null)
                .OrderBy(r => r.RCVDTDESC)
                .Select(r => new SelectListItem
                {
                    Text = r.RCVDTDESC,
                    Value = r.RCVDTID.ToString()
                })
                .ToList();

            ViewBag.ReceivedTypes = receivedTypes;

            var productGroupMap = db.MaterialMasters
                .Where(m => m.DISPSTATUS == 0 || m.DISPSTATUS == null)
                .Select(m => new { m.MTRLID, m.MTRLGID })
                .ToList()
                .ToDictionary(x => x.MTRLID.ToString(), x => x.MTRLGID);

            ViewBag.ProductGroupMap = productGroupMap;

            // For now Issued Stock does not preload existing records; edit behavior can be added later.
            ViewBag.ExistingOpeningJson = "";

            return View(model);
        }

        // Stub for Issued Stock list grid. Replace SQL and mapping when business rules are finalized.
        [Authorize(Roles = "IssuedStockIndex")]
        public ActionResult GetAjaxData(string fromDate = null, string toDate = null)
        {
            var empty = new List<object>();
            return Json(new { data = empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "IssuedStockCreate,IssuedStockEdit")]
        public JsonResult Save(object model)
        {
            return Json(new
            {
                success = false,
                message = "Issued Stock save is not implemented yet. UI design is complete; backend logic can be added once business rules are finalized."
            });
        }
    }
}
