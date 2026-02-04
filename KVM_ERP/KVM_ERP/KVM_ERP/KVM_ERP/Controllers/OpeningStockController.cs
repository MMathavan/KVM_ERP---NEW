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

            // Grade dropdown - enabled grades only (shared with Raw Material Intake)
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

            // Production Colour dropdown - enabled colours only (shared with Raw Material Intake)
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

            // Received Type dropdown - enabled only (shared with Raw Material Intake)
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

            // Map of ProductId (MTRLID) to Product Type (MTRLGID) for client-side filtering.
            // Use string keys so Json.Encode can serialize this safely.
            var productGroupMap = db.MaterialMasters
                .Where(m => m.DISPSTATUS == 0 || m.DISPSTATUS == null)
                .Select(m => new { m.MTRLID, m.MTRLGID })
                .ToList()
                .ToDictionary(x => x.MTRLID.ToString(), x => x.MTRLGID);

            ViewBag.ProductGroupMap = productGroupMap;

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

        // JSON: Additional filter values for Opening Stock form derived from
        // TransactionProductCalculations and related tables for the selected
        // Product Type (Material Group). This powers Received Date,
        // Packing Weight with Glazing and No of Slabs dropdowns.
        [HttpGet]
        public JsonResult GetFilterValues(int groupId, string asOnDate = null)
        {
            try
            {
                DateTime? filterDate = null;
                if (!string.IsNullOrEmpty(asOnDate))
                {
                    DateTime parsed;
                    if (DateTime.TryParse(asOnDate, out parsed))
                    {
                        filterDate = parsed.Date;
                    }
                }

                // Base query: all calculation rows for materials in the
                // selected material group, restricted by PRODDATE when
                // an As on Date is supplied.
                var baseQuery = from tpc in db.TransactionProductCalculations
                                join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                join m in db.MaterialMasters on td.MTRLID equals m.MTRLID
                                where m.MTRLGID == groupId
                                select new { tpc, m };

                baseQuery = baseQuery.Where(x => (x.tpc.DISPSTATUS == 0 || x.tpc.DISPSTATUS == null)
                                                 && (x.m.DISPSTATUS == 0 || x.m.DISPSTATUS == null));

                if (filterDate.HasValue)
                {
                    var asOn = filterDate.Value;
                    baseQuery = baseQuery.Where(x => x.tpc.PRODDATE != null && x.tpc.PRODDATE <= asOn);
                }

                var materialised = baseQuery.ToList();

                // Grades actually used for this product type
                var gradeIds = materialised
                    .Select(x => x.tpc.GRADEID)
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList();

                var grades = db.GradeMasters
                    .Where(g => gradeIds.Contains(g.GRADEID))
                    .OrderBy(g => g.GRADEDESC)
                    .Select(g => new { value = g.GRADEID, text = g.GRADEDESC })
                    .ToList();

                // Production colours used for this product type
                var colourIds = materialised
                    .Select(x => x.tpc.PCLRID)
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList();

                var colours = db.ProductionColourMasters
                    .Where(c => colourIds.Contains(c.PCLRID))
                    .OrderBy(c => c.PCLRDESC)
                    .Select(c => new { value = c.PCLRID, text = c.PCLRDESC })
                    .ToList();

                // Received dates (production dates) available for this group
                var dateValues = materialised
                    .Select(x => x.tpc.PRODDATE)
                    .Where(d => d.HasValue)
                    .Select(d => d.Value.Date)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                var receivedDates = dateValues
                    .Select(d => new
                    {
                        value = d.ToString("yyyy-MM-dd"),
                        text = d.ToString("dd/MM/yyyy")
                    })
                    .ToList();

                // Packing Weight with Glazing: distinct KGWGT values
                var packingWeights = materialised
                    .Select(x => x.tpc.KGWGT)
                    .Where(w => w > 0)
                    .Distinct()
                    .OrderBy(w => w)
                    .Select(w => new
                    {
                        value = w,
                        text = w.ToString("0.###")
                    })
                    .ToList();

                // No of Slabs: use per-pack No of Slabs/Boxes (PCKBOX) as
                // captured in Raw Material Intake instead of summing
                // SLABVALUE. This mirrors the "No of Slabs" field shown
                // below "Packing with Glazing" in the intake form.
                var slabBoxSizes = materialised
                    .Select(x => x.tpc.PCKBOX)
                    .Where(v => v > 0)
                    .Distinct()
                    .OrderBy(v => v)
                    .ToList();

                var noOfSlabs = slabBoxSizes
                    .Select(v => new
                    {
                        value = (decimal)v,
                        text = v.ToString("0")
                    })
                    .ToList();

                return Json(new
                {
                    grades,
                    colours,
                    receivedDates,
                    packingWeights,
                    noOfSlabs
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
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
