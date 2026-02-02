using System.Web.Mvc;
using KVM_ERP.Models;

namespace KVM_ERP.Controllers
{
    [SessionExpire]
    public class OpeningStockController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: OpeningStock
        [Authorize(Roles = "OpeningStocckIndex")] // Note: role name matches AspNetRoles entry
        public ActionResult Index()
        {
            ViewBag.Title = "Opening Stock";
            return View();
        }
    }
}
