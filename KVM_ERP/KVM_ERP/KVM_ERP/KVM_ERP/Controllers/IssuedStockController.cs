using System;
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
            return View();
        }
    }
}
