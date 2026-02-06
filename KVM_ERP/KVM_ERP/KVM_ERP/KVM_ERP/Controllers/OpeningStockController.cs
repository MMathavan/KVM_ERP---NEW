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

        // GET: OpeningStock/Form/{id}
        // When id is provided, load existing Opening Stock (REGSTRID = 11)
        // and pass a JSON-ready model for client-side population.
        [Authorize(Roles = "OpeningStockCreate,OpeningStockEdit")]
        public ActionResult Form(int? id)
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

            ViewBag.ExistingOpeningJson = "";

            if (id.HasValue && id.Value > 0)
            {
                var tranMid = id.Value;

                var header = db.TransactionMasters
                    .FirstOrDefault(t => t.TRANMID == tranMid && t.REGSTRID == 11);

                if (header != null)
                {
                    model = header;

                    // Load details and calculations to reconstruct the client model
                    var details = db.TransactionDetails
                        .Where(d => d.TRANMID == tranMid)
                        .ToList();

                    var trandIds = details.Select(d => d.TRANDID).ToList();

                    var calcs = db.TransactionProductCalculations
                        .Where(c => trandIds.Contains(c.TRANDID))
                        .ToList();

                    // Derive shared header fields from the first detail/calculation row
                    int productTypeId = details.FirstOrDefault()?.MTRLGID ?? 0;
                    int gradeId = details.FirstOrDefault()?.GRADEID ?? 0;
                    int productionColourId = details.FirstOrDefault()?.PCLRID ?? 0;
                    int receivedTypeId = details.FirstOrDefault()?.RCVDTID ?? 0;

                    int packingId = calcs.FirstOrDefault()?.PACKMID ?? 0;
                    decimal packingWeight = calcs.FirstOrDefault()?.KGWGT ?? 0m;
                    int noOfSlabs = calcs.FirstOrDefault()?.PCKBOX ?? 0;

                    var items = details
                        .Select(d => new OpeningStockItemModel
                        {
                            ItemId = d.MTRLID,
                            Slabs = calcs
                                .Where(c => c.TRANDID == d.TRANDID)
                                .Select(c => new OpeningStockSlabModel
                                {
                                    PackingTypeId = c.PACKTMID,
                                    Value = c.SLABVALUE
                                })
                                .ToList()
                        })
                        .Where(i => i.Slabs != null && i.Slabs.Any())
                        .ToList();

                    var existingModel = new OpeningStockSaveModel
                    {
                        OpeningId = tranMid,
                        AsOnDate = header.TRANDATE.ToString("yyyy-MM-dd"),
                        ProductTypeId = productTypeId,
                        PackingId = packingId,
                        GradeId = gradeId,
                        ProductionColourId = productionColourId,
                        ReceivedTypeId = receivedTypeId,
                        PackingWeight = packingWeight,
                        NoOfSlabs = noOfSlabs,
                        Items = items
                    };

                    ViewBag.ExistingOpeningJson = Newtonsoft.Json.JsonConvert.SerializeObject(existingModel);
                }
            }

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

                // Return ONE row per Opening Stock (TRANMID) for Index view.
                // Aggregate Products and their NoOfSlabs into comma-separated strings.
                string sql = @";WITH ItemTotals AS (
                                    SELECT
                                        tm.TRANMID,
                                        tm.TRANDATE,
                                        mg.MTRLGDESC AS ProductType,
                                        pm.PACKMDESC AS PackingMaster,
                                        m.MTRLDESC AS Product,
                                        CAST(ISNULL(SUM(tpc.SLABVALUE), 0) AS DECIMAL(18,2)) AS NoOfSlabs
                                    FROM TRANSACTIONMASTER tm
                                    INNER JOIN TRANSACTIONDETAIL td ON td.TRANMID = tm.TRANMID
                                    INNER JOIN MATERIALGROUPMASTER mg ON mg.MTRLGID = td.MTRLGID
                                    INNER JOIN MATERIALMASTER m ON m.MTRLID = td.MTRLID
                                    LEFT JOIN TRANSACTION_PRODUCT_CALCULATION tpc ON tpc.TRANDID = td.TRANDID
                                    LEFT JOIN PACKINGMASTER pm ON pm.PACKMID = tpc.PACKMID
                                    WHERE tm.REGSTRID = 11
                                      AND (tm.DISPSTATUS = 0 OR tm.DISPSTATUS IS NULL)
                                      AND (mg.DISPSTATUS = 0 OR mg.DISPSTATUS IS NULL)
                                      AND (m.DISPSTATUS = 0 OR m.DISPSTATUS IS NULL)" +
                               whereClause +
                               @"
                                    GROUP BY tm.TRANMID, tm.TRANDATE, mg.MTRLGDESC, pm.PACKMDESC, m.MTRLDESC
                                )
                                SELECT
                                    it.TRANMID,
                                    MAX(it.TRANDATE) AS TRANDATE,
                                    MAX(it.ProductType) AS ProductType,
                                    MAX(it.PackingMaster) AS PackingMaster,
                                    STUFF((
                                        SELECT ', ' + it2.Product
                                        FROM ItemTotals it2
                                        WHERE it2.TRANMID = it.TRANMID
                                        ORDER BY it2.Product
                                        FOR XML PATH(''), TYPE
                                    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Product,
                                    STUFF((
                                        SELECT ', ' + CAST(CAST(it2.NoOfSlabs AS INT) AS NVARCHAR(50))
                                        FROM ItemTotals it2
                                        WHERE it2.TRANMID = it.TRANMID
                                        ORDER BY it2.Product
                                        FOR XML PATH(''), TYPE
                                    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS NoOfSlabs
                                FROM ItemTotals it
                                GROUP BY it.TRANMID
                                ORDER BY MAX(it.TRANDATE) DESC, it.TRANMID DESC";

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
                    NoOfSlabs = r.NoOfSlabs ?? string.Empty
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

        [HttpPost]
        [Authorize(Roles = "OpeningStockCreate,OpeningStockEdit")]
        public JsonResult Save(OpeningStockSaveModel model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new { success = false, message = "Invalid request data." });
                }

                if (string.IsNullOrWhiteSpace(model.AsOnDate))
                {
                    return Json(new { success = false, message = "Date is required." });
                }

                DateTime trandate;
                if (!DateTime.TryParse(model.AsOnDate, out trandate))
                {
                    return Json(new { success = false, message = "Invalid date format." });
                }

                if (model.ProductTypeId <= 0 ||
                    model.PackingId <= 0 ||
                    model.GradeId <= 0 ||
                    model.ProductionColourId <= 0 ||
                    model.ReceivedTypeId <= 0)
                {
                    return Json(new { success = false, message = "Product Type, Packing, Grade, Colour and Received Type are required." });
                }

                if (model.PackingWeight <= 0 || model.NoOfSlabs <= 0)
                {
                    return Json(new { success = false, message = "Packing Weight and No of Slabs must be greater than zero." });
                }

                if (model.Items == null || !model.Items.Any())
                {
                    return Json(new { success = false, message = "No items to save." });
                }

                var validItems = model.Items
                    .Where(i => i != null && i.ItemId > 0 && i.Slabs != null && i.Slabs.Any(s => s != null && s.PackingTypeId > 0 && s.Value > 0))
                    .ToList();

                if (!validItems.Any())
                {
                    return Json(new { success = false, message = "Please enter at least one item with slab values." });
                }

                int compyId = Session["CompyId"] != null ? Convert.ToInt32(Session["CompyId"]) : 1;
                int regstrId = 11;
                string currentUser = User?.Identity?.Name ?? "System";

                int tranMId;
                int tranNo;
                string tranDNo;

                if (model.OpeningId.HasValue && model.OpeningId.Value > 0)
                {
                    tranMId = model.OpeningId.Value;
                    var existing = db.Database.SqlQuery<ExistingOpeningData>(@"
                        SELECT TRANNO, TRANDNO 
                        FROM TRANSACTIONMASTER 
                        WHERE TRANMID = @p0 AND REGSTRID = 11",
                        tranMId).FirstOrDefault();

                    if (existing == null)
                    {
                        return Json(new { success = false, message = "Opening Stock record not found for editing." });
                    }

                    tranNo = existing.TRANNO;
                    tranDNo = existing.TRANDNO;

                    var updateSql = @"
                        UPDATE TRANSACTIONMASTER SET
                            TRANDATE = @p0,
                            CATENAME = @p1,
                            CATECODE = @p2,
                            VECHNO = @p3,
                            DISPSTATUS = @p4,
                            LMUSRID = @p5,
                            PRCSDATE = @p6
                        WHERE TRANMID = @p7 AND REGSTRID = 11";

                    db.Database.ExecuteSqlCommand(updateSql,
                        trandate,
                        "OPENING STOCK",
                        "OPENING",
                        "-",
                        0,
                        currentUser,
                        DateTime.Now,
                        tranMId);

                    db.Database.ExecuteSqlCommand(@"
                        DELETE FROM TRANSACTION_PRODUCT_CALCULATION 
                        WHERE TRANDID IN (SELECT TRANDID FROM TRANSACTIONDETAIL WHERE TRANMID = @p0)",
                        tranMId);

                    db.Database.ExecuteSqlCommand(
                        "DELETE FROM TRANSACTIONDETAIL WHERE TRANMID = @p0",
                        tranMId);
                }
                else
                {
                    var maxTranNo = db.Database.SqlQuery<int?>(@"
                        SELECT MAX(TRANNO) 
                        FROM TRANSACTIONMASTER 
                        WHERE COMPYID = @p0 AND REGSTRID = @p1",
                        compyId, regstrId).FirstOrDefault();

                    tranNo = (maxTranNo ?? 0) + 1;
                    tranDNo = tranNo.ToString("D4");

                    var insertSql = @"
                        INSERT INTO TRANSACTIONMASTER (
                            TRANDATE, CATENAME, CATECODE, VECHNO, DISPSTATUS,
                            CUSRID, LMUSRID, PRCSDATE, CLIENTWGHT, COMPYID,
                            REGSTRID, TRANNO, TRANDNO, TRANREFID, TRANNAMT,
                            TRANAMTWRDS, TRANREFNO,
                            TRANCGSTAMT, TRANSGSTAMT, TRANIGSTAMT,
                            TRANCGSTEXPRN, TRANSGSTEXPRN, TRANIGSTEXPRN,
                            TRANGAMT, TRANPACKAMT, TRANINCAMT
                        ) VALUES (
                            @p0, @p1, @p2, @p3, @p4,
                            @p5, @p6, @p7, @p8, @p9,
                            @p10, @p11, @p12, @p13, @p14,
                            @p15, @p16,
                            @p17, @p18, @p19,
                            @p20, @p21, @p22,
                            @p23, @p24, @p25
                        );
                        SELECT CAST(SCOPE_IDENTITY() as int)";

                    tranMId = db.Database.SqlQuery<int>(insertSql,
                        trandate,              // @p0 TRANDATE
                        "OPENING STOCK",       // @p1 CATENAME
                        "OPENING",             // @p2 CATECODE
                        "-",                   // @p3 VECHNO
                        0,                     // @p4 DISPSTATUS
                        currentUser,           // @p5 CUSRID
                        currentUser,           // @p6 LMUSRID
                        DateTime.Now,          // @p7 PRCSDATE
                        0m,                    // @p8 CLIENTWGHT
                        compyId,               // @p9 COMPYID
                        regstrId,              // @p10 REGSTRID
                        tranNo,                // @p11 TRANNO
                        tranDNo,               // @p12 TRANDNO
                        0,                     // @p13 TRANREFID
                        0m,                    // @p14 TRANNAMT
                        string.Empty,          // @p15 TRANAMTWRDS
                        string.Empty,          // @p16 TRANREFNO
                        0m,                    // @p17 TRANCGSTAMT
                        0m,                    // @p18 TRANSGSTAMT
                        0m,                    // @p19 TRANIGSTAMT
                        0m,                    // @p20 TRANCGSTEXPRN
                        0m,                    // @p21 TRANSGSTEXPRN
                        0m,                    // @p22 TRANIGSTEXPRN
                        0m,                    // @p23 TRANGAMT
                        0m,                    // @p24 TRANPACKAMT
                        0m                     // @p25 TRANINCAMT
                    ).FirstOrDefault();
                }

                foreach (var item in validItems)
                {
                    var material = db.MaterialMasters
                        .FirstOrDefault(m => m.MTRLID == item.ItemId);
                    int hsnId = material != null ? material.HSNID : 0;

                    var detailSql = @"
                        INSERT INTO TRANSACTIONDETAIL (
                            TRANMID, MTRLGID, MTRLID, MTRLNBOX, MTRLCOUNTS,
                            GRADEID, PCLRID, RCVDTID, HSNID,
                            TRANAQTY, TRANDQTY, TRANEQTY, TRANDRATE, TRANDAMT,
                            TRANDDISCEXPRN, TRANDDISCAMT, TRANDGAMT,
                            TRANDCGSTEXPRN, TRANDSGSTEXPRN, TRANDIGSTEXPRN,
                            TRANDCGSTAMT, TRANDSGSTAMT, TRANDIGSTAMT, TRANDNAMT, TRANDAID,
                            CUSRID, LMUSRID, DISPSTATUS, PRCSDATE, TRANDINCAMT
                        )
                        OUTPUT INSERTED.TRANDID
                        VALUES (
                            @p0, @p1, @p2, @p3, @p4,
                            @p5, @p6, @p7, @p8,
                            @p9, @p10, @p11, @p12, @p13,
                            @p14, @p15, @p16,
                            @p17, @p18, @p19,
                            @p20, @p21, @p22, @p23, @p24,
                            @p25, @p26, @p27, @p28, @p29
                        )";

                    int trandId = db.Database.SqlQuery<int>(detailSql,
                        tranMId,                    // @p0 TRANMID
                        model.ProductTypeId,        // @p1 MTRLGID
                        item.ItemId,                // @p2 MTRLID
                        0,                          // @p3 MTRLNBOX
                        0,                          // @p4 MTRLCOUNTS
                        model.GradeId,              // @p5 GRADEID
                        model.ProductionColourId,   // @p6 PCLRID
                        model.ReceivedTypeId,       // @p7 RCVDTID
                        hsnId,                      // @p8 HSNID
                        0m,                         // @p9 TRANAQTY
                        0m,                         // @p10 TRANDQTY
                        0m,                         // @p11 TRANEQTY
                        0m,                         // @p12 TRANDRATE
                        0m,                         // @p13 TRANDAMT
                        0m,                         // @p14 TRANDDISCEXPRN
                        0m,                         // @p15 TRANDDISCAMT
                        0m,                         // @p16 TRANDGAMT
                        0m,                         // @p17 TRANDCGSTEXPRN
                        0m,                         // @p18 TRANDSGSTEXPRN
                        0m,                         // @p19 TRANDIGSTEXPRN
                        0m,                         // @p20 TRANDCGSTAMT
                        0m,                         // @p21 TRANDSGSTAMT
                        0m,                         // @p22 TRANDIGSTAMT
                        0m,                         // @p23 TRANDNAMT
                        0,                          // @p24 TRANDAID
                        currentUser,                // @p25 CUSRID
                        currentUser,                // @p26 LMUSRID
                        0,                          // @p27 DISPSTATUS
                        DateTime.Now,               // @p28 PRCSDATE
                        0m                          // @p29 TRANDINCAMT
                    ).FirstOrDefault();

                    var slabList = item.Slabs
                        .Where(s => s != null && s.PackingTypeId > 0 && s.Value > 0)
                        .ToList();

                    foreach (var slab in slabList)
                    {
                        int tranPid = GetNextTransactionProductId();

                        var calcSql = @"
                            INSERT INTO TRANSACTION_PRODUCT_CALCULATION (
                                TRANPID, TRANMID, TRANDID,
                                PACKMID, PACKTMID,
                                KGWGT, PCKBOX,
                                DISPSTATUS, CUSRID, LMUSRID, PRCSDATE, PRODDATE,
                                GRADEID, PCLRID, RCVDTID, SLABVALUE
                            ) VALUES (
                                @p0, @p1, @p2,
                                @p3, @p4,
                                @p5, @p6,
                                @p7, @p8, @p9, @p10, @p11,
                                @p12, @p13, @p14, @p15
                            )";

                        db.Database.ExecuteSqlCommand(calcSql,
                            tranPid,                   // @p0 TRANPID
                            tranMId,                   // @p1 TRANMID
                            trandId,                   // @p2 TRANDID
                            model.PackingId,           // @p3 PACKMID
                            slab.PackingTypeId,        // @p4 PACKTMID
                            model.PackingWeight,       // @p5 KGWGT
                            model.NoOfSlabs,           // @p6 PCKBOX
                            0,                         // @p7 DISPSTATUS
                            currentUser,               // @p8 CUSRID
                            currentUser,               // @p9 LMUSRID
                            DateTime.Now,              // @p10 PRCSDATE
                            trandate,                  // @p11 PRODDATE
                            model.GradeId,             // @p12 GRADEID
                            model.ProductionColourId,  // @p13 PCLRID
                            model.ReceivedTypeId,      // @p14 RCVDTID
                            slab.Value                 // @p15 SLABVALUE
                        );
                    }
                }

                return Json(new
                {
                    success = true,
                    message = "Opening Stock saved successfully.",
                    tranMId,
                    tranNo,
                    tranDNo
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving Opening Stock: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Del(int id)
        {
            try
            {
                if (!User.IsInRole("OpeningStockDelete"))
                {
                    return Json("Access Denied: You do not have permission to delete Opening Stock records. Please contact your administrator.");
                }

                var exists = db.Database.SqlQuery<int>(
                    "SELECT COUNT(1) FROM TRANSACTIONMASTER WHERE TRANMID = @p0 AND REGSTRID = 11",
                    id).FirstOrDefault();

                if (exists == 0)
                {
                    return Json("Record not found");
                }

                db.Database.ExecuteSqlCommand(@"
                    DELETE FROM TRANSACTION_PRODUCT_CALCULATION
                    WHERE TRANDID IN (SELECT TRANDID FROM TRANSACTIONDETAIL WHERE TRANMID = @p0)",
                    id);

                db.Database.ExecuteSqlCommand(
                    "DELETE FROM TRANSACTIONDETAIL WHERE TRANMID = @p0",
                    id);

                db.Database.ExecuteSqlCommand(
                    "DELETE FROM TRANSACTIONMASTER WHERE TRANMID = @p0 AND REGSTRID = 11",
                    id);

                return Json("Successfully deleted");
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message);
            }
        }

        private int GetNextTransactionProductId()
        {
            var maxLocal = db.TransactionProductCalculations.Local.Any()
                ? db.TransactionProductCalculations.Local.Max(t => t.TRANPID)
                : 0;

            var maxDb = db.TransactionProductCalculations.Any()
                ? db.TransactionProductCalculations.Max(t => t.TRANPID)
                : 0;

            var max = Math.Max(maxLocal, maxDb);
            return max + 1;
        }

        private class OpeningStockRow
        {
            public int TRANMID { get; set; }
            public DateTime TRANDATE { get; set; }
            public string ProductType { get; set; }
            public string Product { get; set; }
            public string PackingMaster { get; set; }
            public string NoOfSlabs { get; set; }
        }

        public class OpeningStockSaveModel
        {
            public int? OpeningId { get; set; }
            public string AsOnDate { get; set; }

            public int ProductTypeId { get; set; }
            public int PackingId { get; set; }
            public int GradeId { get; set; }
            public int ProductionColourId { get; set; }
            public int ReceivedTypeId { get; set; }

            public decimal PackingWeight { get; set; }
            public int NoOfSlabs { get; set; }

            public List<OpeningStockItemModel> Items { get; set; }
        }

        public class OpeningStockItemModel
        {
            public int ItemId { get; set; }
            public List<OpeningStockSlabModel> Slabs { get; set; }
        }

        public class OpeningStockSlabModel
        {
            public int PackingTypeId { get; set; }
            public decimal Value { get; set; }
        }

        private class ExistingOpeningData
        {
            public int TRANNO { get; set; }
            public string TRANDNO { get; set; }
        }
    }
}
