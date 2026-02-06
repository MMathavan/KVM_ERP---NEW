using System;
using System.Web.Mvc;
using KVM_ERP.Models;

namespace KVM_ERP.Controllers
{
    [SessionExpire]
    public class StockMaintainReportController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: StockMaintainReport
        [Authorize(Roles = "StockMaintainReportIndex")]
        public ActionResult Index()
        {
            ViewBag.Title = "Stock Maintain Report";
            return View();
        }

        [HttpPost]
        public JsonResult GetStockData(string fromDate, string toDate)
        {
            return Json(new { success = true, data = new object[] { } });
        }
    }
}
