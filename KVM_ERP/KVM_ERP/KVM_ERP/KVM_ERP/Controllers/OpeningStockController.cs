using System;
using System.Collections.Generic;
using System.Linq;
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

        // GET: OpeningStock/Form
        // Simple form used when clicking "Add New" from the Opening Stock index page.
        // Provides: As on Date, Product Type, Product, Packing dropdowns.
        [Authorize(Roles = "OpeningStockCreate,OpeningStockEdit")]
        public ActionResult Form()
        {
            var model = new TransactionMaster
            {
                TRANDATE = System.DateTime.Today
            };

            // Product Type (Material Group) dropdown - enabled groups only
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

            return View(model);
        }

        public ActionResult GetAjaxData(string fromDate = null, string toDate = null)
        {
            try
            {
                string whereClause = string.Empty;
                var parameters = new List<object>();
                int paramIndex = 0;

                if (!string.IsNullOrEmpty(fromDate))
                {
                    DateTime fromDateTime;
                    if (DateTime.TryParse(fromDate, out fromDateTime))
                    {
                        whereClause += " AND tm.TRANDATE >= @p" + paramIndex;
                        parameters.Add(fromDateTime.Date);
                        paramIndex++;
                    }
                }

                if (!string.IsNullOrEmpty(toDate))
                {
                    DateTime toDateTime;
                    if (DateTime.TryParse(toDate, out toDateTime))
                    {
                        whereClause += " AND tm.TRANDATE <= @p" + paramIndex;
                        parameters.Add(toDateTime.Date.AddDays(1).AddSeconds(-1));
                        paramIndex++;
                    }
                }

                string sql = @"SELECT 
                                    tm.TRANMID,
                                    tm.TRANDATE,
                                    mg.MTRLGDESC AS ProductType,
                                    m.MTRLDESC AS Product,
                                    pm.PACKMDESC AS PackingMaster,
                                    ISNULL(SUM(tpc.SLABVALUE), 0) AS NoOfSlabs
                                FROM TRANSACTIONMASTER tm
                                INNER JOIN TRANSACTIONDETAIL td ON td.TRANMID = tm.TRANMID
                                INNER JOIN MATERIALGROUPMASTER mg ON mg.MTRLGID = td.MTRLGID
                                INNER JOIN MATERIALMASTER m ON m.MTRLID = td.MTRLID
                                LEFT JOIN PACKINGMASTER pm ON pm.PACKMID = m.PACKMID
                                LEFT JOIN TRANSACTION_PRODUCT_CALCULATION tpc ON tpc.TRANDID = td.TRANDID
                                WHERE tm.REGSTRID = 3
                                  AND (tm.DISPSTATUS = 0 OR tm.DISPSTATUS IS NULL)
                                  AND (mg.DISPSTATUS = 0 OR mg.DISPSTATUS IS NULL)
                                  AND (m.DISPSTATUS = 0 OR m.DISPSTATUS IS NULL)" +
                               whereClause +
                               @" GROUP BY tm.TRANMID, tm.TRANDATE, mg.MTRLGDESC, m.MTRLDESC, pm.PACKMDESC
                                  ORDER BY tm.TRANDATE DESC, tm.TRANMID DESC";

                var rows = parameters.Count > 0
                    ? db.Database.SqlQuery<OpeningStockRow>(sql, parameters.ToArray()).ToList()
                    : db.Database.SqlQuery<OpeningStockRow>(sql).ToList();

                var data = rows.Select((r, index) => new
                {
                    TRANMID = r.TRANMID,
                    TRANDATE = r.TRANDATE.ToString("yyyy-MM-dd"),
                    ProductType = r.ProductType ?? string.Empty,
                    Product = r.Product ?? string.Empty,
                    PackingMaster = r.PackingMaster ?? string.Empty,
                    NoOfSlabs = r.NoOfSlabs
                }).ToList();

                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // JSON: Products by Material Group (enabled only) for Opening Stock form
        [HttpGet]
        public JsonResult GetProducts(int groupId)
        {
            var prods = db.MaterialMasters
                .Where(m => m.MTRLGID == groupId && (m.DISPSTATUS == 0 || m.DISPSTATUS == null))
                .OrderBy(m => m.MTRLDESC)
                .Select(m => new { id = m.MTRLID, text = m.MTRLDESC })
                .ToList();

            return Json(prods, JsonRequestBehavior.AllowGet);
        }

        // JSON: Packing masters filtered by Product Type (Material Group)
        [HttpGet]
        public JsonResult GetPackingMasters(int groupId)
        {
            var packs = db.PackingMasters
                .Where(p => (p.DISPSTATUS == 0 || p.DISPSTATUS == null)
                            && (p.MTRLGID == groupId || p.MTRLGID == null))
                .OrderBy(p => p.PACKMDESC)
                .Select(p => new { id = p.PACKMID, text = p.PACKMDESC })
                .ToList();

            return Json(packs, JsonRequestBehavior.AllowGet);
        }

        private class OpeningStockRow
        {
            public int TRANMID { get; set; }
            public DateTime TRANDATE { get; set; }
            public string ProductType { get; set; }
            public string Product { get; set; }
            public string PackingMaster { get; set; }
            public decimal NoOfSlabs { get; set; }
        }
    }
}
