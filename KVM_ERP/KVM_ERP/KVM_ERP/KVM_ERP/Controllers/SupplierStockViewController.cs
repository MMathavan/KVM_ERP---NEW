using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using KVM_ERP.Models;

namespace KVM_ERP.Controllers
{
    [SessionExpire]
    public class SupplierStockViewController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SupplierStockView
        [Authorize(Roles = "SupplierStockViewIndex")]
        public ActionResult Index()
        {
            var suppliers = (from tm in db.TransactionMasters
                             join s in db.SupplierMasters on tm.TRANREFID equals s.CATEID
                             where tm.REGSTRID == 1
                                   && (tm.DISPSTATUS == 0 || tm.DISPSTATUS == null)
                                   && (s.DISPSTATUS == 0 || s.DISPSTATUS == null)
                             select new
                             {
                                 s.CATEID,
                                 s.CATENAME,
                                 s.CATECODE
                             })
                            .Distinct()
                            .OrderBy(x => x.CATENAME)
                            .ToList();

            var supplierList = suppliers
                .Select(s => new SelectListItem
                {
                    Text = s.CATENAME,
                    Value = s.CATEID.ToString()
                })
                .ToList();

            supplierList.Insert(0, new SelectListItem { Text = "-- Select Supplier --", Value = string.Empty });

            ViewBag.SupplierList = supplierList;
            ViewBag.Title = "Supplier wise Stock View";

            return View();
        }

        [HttpGet]
        public JsonResult GetIntakeList(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var query = db.TransactionMasters.Where(t => t.REGSTRID == 1 && (t.DISPSTATUS == 0 || t.DISPSTATUS == null));

                if (fromDate.HasValue)
                {
                    var from = fromDate.Value.Date;
                    query = query.Where(t => t.TRANDATE >= from);
                }

                if (toDate.HasValue)
                {
                    var to = toDate.Value.Date;
                    query = query.Where(t => t.TRANDATE <= to);
                }

                var detailBoxes = db.TransactionDetails
                    .GroupBy(d => d.TRANMID)
                    .Select(g => new
                    {
                        TRANMID = g.Key,
                        Boxes = g.Sum(x => (int?)x.MTRLNBOX) ?? 0
                    })
                    .ToList();

                var result = query
                    .OrderBy(t => t.TRANDATE)
                    .ThenBy(t => t.TRANMID)
                    .ToList()
                    .Select(t => new object[]
                    {
                        t.TRANMID,
                        t.TRANDATE.ToString("dd-MM-yyyy"),
                        t.CATENAME ?? string.Empty,
                        t.CATECODE ?? string.Empty,
                        t.VECHNO ?? string.Empty,
                        t.CLIENTWGHT.ToString("0.000"),
                        detailBoxes.FirstOrDefault(x => x.TRANMID == t.TRANMID)?.Boxes ?? 0
                    })
                    .ToList();

                return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, data = new object[0] }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetIntakeOverview(int tranmid)
        {
            try
            {
                var transaction = db.TransactionMasters.FirstOrDefault(t => t.TRANMID == tranmid && t.REGSTRID == 1);
                if (transaction == null)
                {
                    return Json(new { success = false, message = "Transaction not found" }, JsonRequestBehavior.AllowGet);
                }

                var products = (from td in db.TransactionDetails
                                join mg in db.MaterialGroupMasters on td.MTRLGID equals mg.MTRLGID
                                join m in db.MaterialMasters on td.MTRLID equals m.MTRLID
                                where td.TRANMID == tranmid
                                select new
                                {
                                    ProductType = mg.MTRLGDESC,
                                    ProductName = m.MTRLDESC,
                                    NoOfBoxes = td.MTRLNBOX,
                                    CountsPerKg = td.MTRLCOUNTS
                                })
                                .OrderBy(x => x.ProductType)
                                .ThenBy(x => x.ProductName)
                                .ToList();

                var allCalcs = db.TransactionProductCalculations
                    .Where(c => c.TRANMID == tranmid && (c.DISPSTATUS == 0 || c.DISPSTATUS == null) && c.PRODDATE.HasValue)
                    .ToList();

                // IMPORTANT: In the new headless slab design, each logical calculation
                // (TRANDID + PACKMID) can be stored as multiple rows (one per PACKTMID
                // slab), and FACTORYWGT / TOTALYELDCOUNTS are copied to every row.
                // If we sum directly over allCalcs we will incorrectly multiply
                // weights and counts by the number of slab rows.

                // Deduplicate to a single representative row per TRANDID + PACKMID + Date
                // by preferring the header row (PACKTMID = 0) when present, otherwise the
                // first slab row. This matches RawMaterialIntake's aggregation behaviour.
                var headerCalcs = allCalcs
                    .GroupBy(c => new { c.TRANDID, c.PACKMID, Date = c.PRODDATE.Value.Date })
                    .Select(g => g
                        .OrderBy(c => c.PACKTMID) // header row first when it exists
                        .ThenBy(c => c.TRANPID)
                        .First())
                    .ToList();

                // Split-wise production: group by TRANDID and Production Date using
                // the deduplicated headerCalcs instead of raw slab rows
                var grouped = headerCalcs
                    .GroupBy(c => new { Date = c.PRODDATE.Value.Date, c.TRANDID })
                    .OrderBy(g => g.Key.Date)
                    .ThenBy(g => g.Key.TRANDID)
                    .ToList();

                var productionRows = new List<object>();
                decimal runningBalance = transaction.CLIENTWGHT;

                foreach (var g in grouped)
                {
                    // Each group now contains at most one row per TRANDID+PACKMID,
                    // so these sums reflect the true totals for that split without
                    // being multiplied by the number of slab rows.
                    decimal totalKgs = g.Sum(x => x.KGWGT);
                    decimal totalCounts = g.Sum(x => x.TOTALYELDCOUNTS);
                    decimal ourKgs = g.Sum(x => x.FACTORYWGT);
                    runningBalance -= ourKgs;

                    productionRows.Add(new
                    {
                        ProductionDate = g.Key.Date.ToString("dd-MM-yyyy"),
                        TotalKgs = totalKgs.ToString("0.000"),
                        TotalCounts = totalCounts.ToString("0.000"),
                        OurKgs = ourKgs.ToString("0.000"),
                        BalanceStock = runningBalance.ToString("0.000")
                    });
                }

                var header = new
                {
                    TransactionId = transaction.TRANMID,
                    TransactionNo = transaction.TRANDNO,
                    TransactionDate = transaction.TRANDATE.ToString("dd-MM-yyyy"),
                    SupplierName = transaction.CATENAME,
                    SupplierCode = transaction.CATECODE,
                    VehicleNo = transaction.VECHNO,
                    ClientWeight = transaction.CLIENTWGHT.ToString("0.000")
                };

                var productModels = products
                    .Select(p => new
                    {
                        ProductType = p.ProductType,
                        ProductName = p.ProductName,
                        NoOfBoxes = p.NoOfBoxes,
                        CountsPerKg = p.CountsPerKg
                    })
                    .ToList();

                return Json(new
                {
                    success = true,
                    header = header,
                    products = productModels,
                    productionRows = productionRows
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetAjaxData(int supplierId, string asOnDate)
        {
            try
            {
                if (supplierId <= 0)
                {
                    return Json(new { data = new object[0] }, JsonRequestBehavior.AllowGet);
                }

                DateTime filterDate = DateTime.Now.Date;
                if (!string.IsNullOrWhiteSpace(asOnDate))
                {
                    DateTime parsed;
                    if (DateTime.TryParse(asOnDate, out parsed))
                    {
                        filterDate = parsed.Date;
                    }
                }

                var slabTotals = GetSlabTotalsBySupplier(filterDate, supplierId);

                return Json(new { data = slabTotals }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in SupplierStockView.GetAjaxData: " + ex.Message);
                return Json(new { error = ex.Message, data = new object[0] }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetItemDetails(int supplierId, int itemId, string asOnDate)
        {
            try
            {
                DateTime filterDate = DateTime.Now;
                if (!string.IsNullOrEmpty(asOnDate))
                {
                    DateTime.TryParse(asOnDate, out filterDate);
                }

                System.Diagnostics.Debug.WriteLine($"SupplierStockView.GetItemDetails called - SupplierId: {supplierId}, ItemId: {itemId}, AsOnDate: {filterDate:yyyy-MM-dd}");

                var details = GetItemDetailBreakdownByDateRangeForSupplier(supplierId, itemId, filterDate);

                return Json(new
                {
                    success = true,
                    packingMasters = details.PackingMasters,
                    selectedDate = filterDate.ToString("dd/MM/yyyy")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in SupplierStockView.GetItemDetails: " + ex.Message);
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<object[]> GetSlabTotalsBySupplier(DateTime asOnDate, int supplierId)
        {
            try
            {
                var productTotals = new List<object[]>();

                // Load packing type metadata once to map PACKTMID -> logical slab order
                var packingTypeMeta = db.PackingTypeMasters
                    .Where(pt => (pt.DISPSTATUS == 0 || pt.DISPSTATUS == null))
                    .Select(pt => new
                    {
                        pt.PACKMID,
                        pt.PACKTMID,
                        pt.PACKTMDESC,
                        pt.PACKTMCODE
                    })
                    .ToList();

                var brokenOrOthersPacktmIds = new HashSet<int>(
                    packingTypeMeta
                        .Where(pt => pt.PACKTMDESC.ToUpper().Contains("BKN")
                                  || pt.PACKTMDESC.ToUpper().Contains("BROKEN")
                                  || pt.PACKTMDESC.ToUpper().Contains("OTHERS")
                                  || pt.PACKTMDESC.ToUpper().Contains("OTHER"))
                        .Select(pt => pt.PACKTMID)
                );

                var packingTypesByPackmid = packingTypeMeta
                    .Where(pt => !brokenOrOthersPacktmIds.Contains(pt.PACKTMID))
                    .OrderBy(pt => pt.PACKMID)
                    .ThenBy(pt => pt.PACKTMCODE)
                    .GroupBy(pt => pt.PACKMID)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Load normalized slab-based calculation data for this supplier
                var allCalcs = (from tpc in db.TransactionProductCalculations
                                join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                join m in db.MaterialMasters on td.MTRLID equals m.MTRLID
                                join tm in db.TransactionMasters on td.TRANMID equals tm.TRANMID
                                where (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                      && (m.DISPSTATUS == 0 || m.DISPSTATUS == null)
                                      && (tm.DISPSTATUS == 0 || tm.DISPSTATUS == null)
                                      && tpc.PRODDATE <= asOnDate
                                      && tpc.PACKTMID != 0
                                      && tpc.SLABVALUE > 0
                                      && tm.REGSTRID == 1
                                      && tm.TRANREFID == supplierId
                                select new
                                {
                                    ProductId = m.MTRLID,
                                    ProductName = m.MTRLDESC,
                                    PackingId = tpc.PACKMID,
                                    KgWeight = tpc.KGWGT,
                                    GradeId = tpc.GRADEID,
                                    PclrId = tpc.PCLRID,
                                    RcvdtId = tpc.RCVDTID,
                                    BoxSize = tpc.PCKBOX,
                                    PackTmId = tpc.PACKTMID,
                                    SlabValue = tpc.SLABVALUE
                                }).ToList();

                // Exclude slab rows that belong to BKN/OTHERS packing types; those are handled separately
                allCalcs = allCalcs
                    .Where(x => !brokenOrOthersPacktmIds.Contains(x.PackTmId))
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"SupplierStockView: Loaded {allCalcs.Count} slab calculation records into memory for supplier {supplierId}");

                if (!allCalcs.Any())
                {
                    return productTotals;
                }

                var productCalcs = allCalcs
                    .GroupBy(x => new { x.ProductId, x.ProductName })
                    .Select(g => new
                    {
                        ProductId = g.Key.ProductId,
                        ProductName = g.Key.ProductName,
                        TotalPCK = g.Sum(tpc => tpc.SlabValue)
                    })
                    .Where(x => x.TotalPCK > 0)
                    .OrderBy(x => x.ProductName)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"SupplierStockView: Product totals found (from slabs): {productCalcs.Count}");

                foreach (var product in productCalcs)
                {
                    var productRows = allCalcs
                        .Where(x => x.ProductId == product.ProductId)
                        .ToList();

                    decimal totalCases = 0;
                    decimal productTotalSlabs = 0;

                    // Group like detail breakdown: by packing + weight + grade + colour + received type
                    var productGroups = productRows
                        .GroupBy(x => new
                        {
                            x.PackingId,
                            x.KgWeight,
                            x.GradeId,
                            x.PclrId,
                            x.RcvdtId
                        });

                    foreach (var grp in productGroups)
                    {
                        int packBoxSize = grp.Max(r => r.BoxSize);
                        int boxSize = packBoxSize > 0 ? packBoxSize : 6;

                        if (!packingTypesByPackmid.TryGetValue(grp.Key.PackingId, out var packTypes) || packTypes.Count == 0)
                        {
                            // If we have no packing type metadata, fall back to treating all slabs as a single column
                            decimal fallbackTotalSlabs = grp.Sum(r => r.SlabValue);
                            productTotalSlabs += fallbackTotalSlabs;
                            totalCases += CalculateBoxes(fallbackTotalSlabs, boxSize);
                            continue;
                        }

                        int columnCount = packTypes.Count;
                        var slabByPacktm = grp
                            .GroupBy(r => r.PackTmId)
                            .ToDictionary(g => g.Key, g => g.Sum(x => x.SlabValue));

                        decimal[] colTotals = new decimal[columnCount];

                        for (int index = 0; index < columnCount; index++)
                        {
                            var pt = packTypes[index];
                            if (slabByPacktm.TryGetValue(pt.PACKTMID, out var val))
                            {
                                colTotals[index] = val;
                            }
                        }

                        decimal groupTotalSlabs = colTotals.Sum();

                        // NO OF CASES for this packing group = sum of boxes per column
                        decimal groupCases = 0;
                        for (int i = 0; i < columnCount; i++)
                        {
                            groupCases += CalculateBoxes(colTotals[i], boxSize);
                        }

                        productTotalSlabs += groupTotalSlabs;
                        totalCases += groupCases;
                    }

                    productTotals.Add(new object[]
                    {
                        product.ProductId,
                        product.ProductName,
                        productTotalSlabs.ToString("N2"),
                        totalCases.ToString("N0")
                    });
                }

                // BKN totals for this supplier (deduplicated by TRANDID+PACKMID)
                var allBknRows = (from tpc in db.TransactionProductCalculations
                                  join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                  join tm in db.TransactionMasters on td.TRANMID equals tm.TRANMID
                                  where (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                        && (tm.DISPSTATUS == 0 || tm.DISPSTATUS == null)
                                        && tpc.PRODDATE <= asOnDate
                                        && tm.REGSTRID == 1
                                        && tm.TRANREFID == supplierId
                                        && tpc.BKN > 0
                                  select tpc).ToList();

                var bknTotal = allBknRows
                    .GroupBy(t => new { t.TRANDID, t.PACKMID })
                    .Select(g => g
                        .OrderBy(c => c.PACKTMID)
                        .ThenBy(c => c.TRANPID)
                        .First().BKN)
                    .DefaultIfEmpty(0)
                    .Sum();

                if (bknTotal > 0)
                {
                    // For BKN (Broken) we do not track cases; always display 0 cases
                    decimal bknCases = 0;

                    productTotals.Add(new object[]
                    {
                        -1,
                        "BKN (Broken)",
                        bknTotal.ToString("N2"),
                        bknCases.ToString("N0")
                    });
                }

                // OTHERS totals for this supplier (deduplicated by TRANDID+PACKMID)
                var allOthersRows = (from tpc in db.TransactionProductCalculations
                                     join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                     join tm in db.TransactionMasters on td.TRANMID equals tm.TRANMID
                                     where (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                           && (tm.DISPSTATUS == 0 || tm.DISPSTATUS == null)
                                           && tpc.PRODDATE <= asOnDate
                                           && tm.REGSTRID == 1
                                           && tm.TRANREFID == supplierId
                                           && tpc.OTHERS > 0
                                     select tpc).ToList();

                var othersTotal = allOthersRows
                    .GroupBy(t => new { t.TRANDID, t.PACKMID })
                    .Select(g => g
                        .OrderBy(c => c.PACKTMID)
                        .ThenBy(c => c.TRANPID)
                        .First().OTHERS)
                    .DefaultIfEmpty(0)
                    .Sum();

                if (othersTotal > 0)
                {
                    var othersCases = CalculateBoxes(othersTotal);

                    productTotals.Add(new object[]
                    {
                        -2,
                        "Others(Peeled)",
                        othersTotal.ToString("N2"),
                        othersCases.ToString("N0")
                    });
                }

                return productTotals;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in GetSlabTotalsBySupplier: " + ex.Message);
                return new List<object[]>();
            }
        }

        private PackingMasterBreakdown GetItemDetailBreakdownByDateRangeForSupplier(int supplierId, int itemId, DateTime selectedDate)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine($"SupplierStockView.GetItemDetailBreakdownByDateRangeForSupplier - SupplierId: {supplierId}, ProductId: {itemId}, SelectedDate: {selectedDate:yyyy-MM-dd}");

                var breakdown = new PackingMasterBreakdown
                {
                    PackingMasters = new List<PackingMasterData>()
                };

                if (itemId == -1)
                {
                    return GetBKNDetailBreakdownByDateRangeForSupplier(supplierId, selectedDate);
                }

                if (itemId == -2)
                {
                    return GetOTHERSDetailBreakdownByDateRangeForSupplier(supplierId, selectedDate);
                }

                // Step 1: Load all calculations for this product for the given supplier
                var allCalculations = (from tpc in db.TransactionProductCalculations
                                       join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                       join tm in db.TransactionMasters on td.TRANMID equals tm.TRANMID
                                       join pm in db.PackingMasters on tpc.PACKMID equals pm.PACKMID
                                       join pclr in db.ProductionColourMasters on tpc.PCLRID equals pclr.PCLRID into pclrJoin
                                       from pclr in pclrJoin.DefaultIfEmpty()
                                       join rcvdt in db.ReceivedTypeMasters on tpc.RCVDTID equals rcvdt.RCVDTID into rcvdtJoin
                                       from rcvdt in rcvdtJoin.DefaultIfEmpty()
                                       join grade in db.GradeMasters on tpc.GRADEID equals grade.GRADEID into gradeJoin
                                       from grade in gradeJoin.DefaultIfEmpty()
                                       where td.MTRLID == itemId
                                             && (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                             && (pm.DISPSTATUS == 0 || pm.DISPSTATUS == null)
                                             && (tm.DISPSTATUS == 0 || tm.DISPSTATUS == null)
                                             && tpc.PRODDATE <= selectedDate
                                             && tm.REGSTRID == 1
                                             && tm.TRANREFID == supplierId
                                       select new
                                       {
                                           Calculation = tpc,
                                           PackingType = pm.PACKMDESC,
                                           PackingId = pm.PACKMID,
                                           TranDate = tpc.PRODDATE,
                                           ColourDesc = pclr != null ? pclr.PCLRDESC : null,
                                           ReceivedTypeDesc = rcvdt != null ? rcvdt.RCVDTDESC : null,
                                           GradeDesc = grade != null ? grade.GRADEDESC : null
                                       }).ToList();

                System.Diagnostics.Debug.WriteLine($"Loaded {allCalculations.Count} total calculation records for supplier {supplierId} (including BKN/OTHERS)");

                // Exclude records that have no slab values; keep only slab rows with PACKTMID <> 0 and SLABVALUE > 0
                allCalculations = allCalculations
                    .Where(x =>
                        x.Calculation.PACKTMID != 0 &&
                        x.Calculation.SLABVALUE > 0)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"After filtering slab records, remaining calculations for supplier {supplierId}: {allCalculations.Count}");

                // Step 2: Group in memory by PackingId + KGWGT + PCLRID + RCVDTID + GRADEID
                var packingMasters = allCalculations
                    .GroupBy(x => new
                    {
                        x.PackingId,
                        x.PackingType,
                        KGWGT = x.Calculation.KGWGT,
                        PCLRID = x.Calculation.PCLRID,
                        RCVDTID = x.Calculation.RCVDTID,
                        GRADEID = x.Calculation.GRADEID,
                        x.ColourDesc,
                        x.ReceivedTypeDesc,
                        x.GradeDesc
                    })
                    .Select(g =>
                    {
                        string displayName = g.Key.PackingType;

                        // Use per-pack No of Boxes (PCKBOX) as the pack size shown in the header
                        int packBoxSize = g.Max(x => x.Calculation.PCKBOX);
                        int boxesForDisplay = packBoxSize > 0 ? packBoxSize : 6;

                        if (g.Key.KGWGT > 0)
                            displayName += " " + boxesForDisplay + " x " + g.Key.KGWGT.ToString("0.##");

                        if (!string.IsNullOrEmpty(g.Key.GradeDesc))
                            displayName += " - " + g.Key.GradeDesc;

                        if (!string.IsNullOrEmpty(g.Key.ColourDesc))
                            displayName += " - " + g.Key.ColourDesc;

                        if (!string.IsNullOrEmpty(g.Key.ReceivedTypeDesc))
                            displayName += " - " + g.Key.ReceivedTypeDesc;

                        return new
                        {
                            PackingType = displayName,
                            PackingId = g.Key.PackingId,
                            KgWeight = g.Key.KGWGT,
                            PclrId = g.Key.PCLRID,
                            RcvdtId = g.Key.RCVDTID,
                            GradeId = g.Key.GRADEID,
                            BoxSize = boxesForDisplay
                        };
                    })
                    .OrderBy(x => x.PackingId)
                    .ThenBy(x => x.KgWeight)
                    .ThenBy(x => x.GradeId)
                    .ThenBy(x => x.PclrId)
                    .ThenBy(x => x.RcvdtId)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Found {packingMasters.Count} packing master+KGWGT combinations for supplier {supplierId}");

                // For each packing master, get data split by date ranges
                foreach (var pm in packingMasters)
                {
                    System.Diagnostics.Debug.WriteLine($"  - {pm.PackingType} (PackingId: {pm.PackingId}, KgWeight: {pm.KgWeight})");

                    var previousDate = selectedDate.AddDays(-1);

                    // Load packing types for this packing master to determine dynamic slab columns
                    var packingTypes = db.PackingTypeMasters
                        .Where(pt => pt.PACKMID == pm.PackingId
                                  && (pt.DISPSTATUS == 0 || pt.DISPSTATUS == null)
                                  && !pt.PACKTMDESC.ToUpper().Contains("BKN")
                                  && !pt.PACKTMDESC.ToUpper().Contains("BROKEN")
                                  && !pt.PACKTMDESC.ToUpper().Contains("OTHERS")
                                  && !pt.PACKTMDESC.ToUpper().Contains("OTHER"))
                        .OrderBy(pt => pt.PACKTMCODE)
                        .ToList();

                    var effectivePackingTypes = packingTypes;
                    int columnCount = effectivePackingTypes.Count;

                    if (columnCount == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"  {pm.PackingType}: No packing types found, skipping.");
                        continue;
                    }

                    // Filter from already loaded data in memory by PackingId, KGWGT, GRADEID, PCLRID and RCVDTID
                    var rowsForPacking = allCalculations
                        .Where(x => x.PackingId == pm.PackingId
                                   && x.Calculation.KGWGT == pm.KgWeight
                                   && x.Calculation.GRADEID == pm.GradeId
                                   && x.Calculation.PCLRID == pm.PclrId
                                   && x.Calculation.RCVDTID == pm.RcvdtId)
                        .ToList();

                    var upToPreviousDay = rowsForPacking
                        .Where(x => x.TranDate <= previousDate)
                        .ToList();

                    var selectedDay = rowsForPacking
                        .Where(x => x.TranDate == selectedDate)
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"  Found {upToPreviousDay.Count} records up to previous day");
                    System.Diagnostics.Debug.WriteLine($"  Found {selectedDay.Count} records for selected day");

                    var upToPrevByPacktm = upToPreviousDay
                        .GroupBy(x => x.Calculation.PACKTMID)
                        .ToDictionary(g => g.Key, g => g.Sum(r => r.Calculation.SLABVALUE));

                    var selectedByPacktm = selectedDay
                        .GroupBy(x => x.Calculation.PACKTMID)
                        .ToDictionary(g => g.Key, g => g.Sum(r => r.Calculation.SLABVALUE));

                    // Build value arrays for each row type
                    var upToPreviousValues = new List<decimal>(new decimal[columnCount]);
                    var selectedDayValues = new List<decimal>(new decimal[columnCount]);

                    for (int index = 0; index < columnCount; index++)
                    {
                        var packtmId = effectivePackingTypes[index].PACKTMID;
                        decimal upVal = upToPrevByPacktm.ContainsKey(packtmId) ? upToPrevByPacktm[packtmId] : 0;
                        decimal selVal = selectedByPacktm.ContainsKey(packtmId) ? selectedByPacktm[packtmId] : 0;

                        upToPreviousValues[index] = upVal;
                        selectedDayValues[index] = selVal;
                    }

                    var upToPreviousData = new PackingDetailRow
                    {
                        RowType = $"Up to {previousDate:dd/MM/yyyy}",
                        Values = upToPreviousValues,
                        Total = upToPreviousValues.Sum()
                    };

                    var selectedDayData = new PackingDetailRow
                    {
                        RowType = selectedDate.ToString("dd/MM/yyyy"),
                        Values = selectedDayValues,
                        Total = selectedDayValues.Sum()
                    };

                    // Calculate TOTAL row (sum of both)
                    var totalValues = new List<decimal>(columnCount);
                    for (int i = 0; i < columnCount; i++)
                    {
                        totalValues.Add(upToPreviousValues[i] + selectedDayValues[i]);
                    }

                    var totalData = new PackingDetailRow
                    {
                        RowType = "TOTAL",
                        Values = totalValues,
                        Total = totalValues.Sum()
                    };

                    // Calculate NO OF CASES row using dynamic box size per packing master
                    var boxSize = pm.BoxSize > 0 ? pm.BoxSize : 6;

                    var noOfCasesValues = new List<decimal>(columnCount);
                    for (int i = 0; i < columnCount; i++)
                    {
                        noOfCasesValues.Add(CalculateBoxes(totalValues[i], boxSize));
                    }

                    var noOfBoxesData = new PackingDetailRow
                    {
                        RowType = "NO OF CASES",
                        Values = noOfCasesValues,
                        Total = noOfCasesValues.Sum()
                    };

                    // Column headers from PACKINGTYPEMASTER
                    var columnHeaders = effectivePackingTypes
                        .Select(pt => pt.PACKTMDESC)
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"  {pm.PackingType}: Found {columnHeaders.Count} column headers");

                    var packingMasterData = new PackingMasterData
                    {
                        PackingType = pm.PackingType,
                        ColumnHeaders = columnHeaders,
                        Rows = new List<PackingDetailRow>
                        {
                            upToPreviousData,
                            selectedDayData,
                            totalData,
                            noOfBoxesData
                        }
                    };

                    breakdown.PackingMasters.Add(packingMasterData);

                    System.Diagnostics.Debug.WriteLine($"  {pm.PackingType}: UpToPrevious={upToPreviousData.Total:N2}, SelectedDay={selectedDayData.Total:N2}, Total={totalData.Total:N2}");
                }

                System.Diagnostics.Debug.WriteLine("========================================");
                return breakdown;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR in SupplierStockView.GetItemDetailBreakdownByDateRangeForSupplier: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                return new PackingMasterBreakdown
                {
                    PackingMasters = new List<PackingMasterData>()
                };
            }
        }

        private PackingMasterBreakdown GetBKNDetailBreakdownByDateRangeForSupplier(int supplierId, DateTime selectedDate)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine($"SupplierStockView.GetBKNDetailBreakdownByDateRangeForSupplier - SupplierId: {supplierId}, SelectedDate: {selectedDate:yyyy-MM-dd}");

                var breakdown = new PackingMasterBreakdown
                {
                    PackingMasters = new List<PackingMasterData>()
                };

                var allBKNData = (from tpc in db.TransactionProductCalculations
                                   join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                   join tm in db.TransactionMasters on td.TRANMID equals tm.TRANMID
                                   join pm in db.PackingMasters on tpc.PACKMID equals pm.PACKMID
                                   join m in db.MaterialMasters on td.MTRLID equals m.MTRLID
                                   join pclr in db.ProductionColourMasters on tpc.PCLRID equals pclr.PCLRID into pclrJoin
                                   from pclr in pclrJoin.DefaultIfEmpty()
                                   join rcvdt in db.ReceivedTypeMasters on tpc.RCVDTID equals rcvdt.RCVDTID into rcvdtJoin
                                   from rcvdt in rcvdtJoin.DefaultIfEmpty()
                                   join grade in db.GradeMasters on tpc.GRADEID equals grade.GRADEID into gradeJoin
                                   from grade in gradeJoin.DefaultIfEmpty()
                                   where (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                         && (pm.DISPSTATUS == 0 || pm.DISPSTATUS == null)
                                         && (tm.DISPSTATUS == 0 || tm.DISPSTATUS == null)
                                         && (m.DISPSTATUS == 0 || m.DISPSTATUS == null)
                                         && tpc.PRODDATE <= selectedDate
                                         && tm.REGSTRID == 1
                                         && tm.TRANREFID == supplierId
                                         && tpc.BKN != null && tpc.BKN > 0
                                   select new
                                   {
                                       Calculation = tpc,
                                       PackingType = pm.PACKMDESC,
                                       PackingId = pm.PACKMID,
                                       ProductName = m.MTRLDESC,
                                       TranDate = tpc.PRODDATE,
                                       ColourDesc = pclr != null ? pclr.PCLRDESC : null,
                                       ReceivedTypeDesc = rcvdt != null ? rcvdt.RCVDTDESC : null,
                                       GradeDesc = grade != null ? grade.GRADEDESC : null
                                   }).ToList();

                // Deduplicate BKN per TRANDID + PACKMID to avoid multiplying by slab rows
                allBKNData = allBKNData
                    .GroupBy(x => new { x.Calculation.TRANDID, x.Calculation.PACKMID })
                    .Select(g => g
                        .OrderBy(c => c.Calculation.PACKTMID)
                        .ThenBy(c => c.Calculation.TRANPID)
                        .First())
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Loaded {allBKNData.Count} distinct BKN calculation rows for supplier {supplierId} (grouped by TRANDID+PACKMID)");

                var bknGroups = allBKNData
                    .GroupBy(x => new
                    {
                        x.PackingId,
                        x.PackingType,
                        KGWGT = x.Calculation.KGWGT,
                        PCLRID = x.Calculation.PCLRID,
                        RCVDTID = x.Calculation.RCVDTID,
                        GRADEID = x.Calculation.GRADEID,
                        x.ProductName,
                        x.ColourDesc,
                        x.ReceivedTypeDesc,
                        x.GradeDesc
                    })
                    .Select(g =>
                    {
                        string displayName = g.Key.PackingType;

                        int packBoxSize = g.Max(x => x.Calculation.PCKBOX);
                        int boxesForDisplay = packBoxSize > 0 ? packBoxSize : 6;

                        if (g.Key.KGWGT > 0)
                            displayName += " " + boxesForDisplay + " x " + g.Key.KGWGT.ToString("0.##");

                        if (!string.IsNullOrEmpty(g.Key.ProductName))
                            displayName += " - " + g.Key.ProductName;

                        if (!string.IsNullOrEmpty(g.Key.GradeDesc))
                            displayName += " - " + g.Key.GradeDesc;

                        if (!string.IsNullOrEmpty(g.Key.ColourDesc))
                            displayName += " - " + g.Key.ColourDesc;

                        if (!string.IsNullOrEmpty(g.Key.ReceivedTypeDesc))
                            displayName += " - " + g.Key.ReceivedTypeDesc;

                        return new
                        {
                            PackingType = displayName,
                            PackingId = g.Key.PackingId,
                            KgWeight = g.Key.KGWGT,
                            PclrId = g.Key.PCLRID,
                            RcvdtId = g.Key.RCVDTID,
                            GradeId = g.Key.GRADEID,
                            ProductName = g.Key.ProductName
                        };
                    })
                    .OrderBy(x => x.PackingId)
                    .ThenBy(x => x.ProductName)
                    .ThenBy(x => x.KgWeight)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Found {bknGroups.Count} BKN group combinations for supplier {supplierId}");

                foreach (var grp in bknGroups)
                {
                    var previousDate = selectedDate.AddDays(-1);

                    var upToPreviousDay = allBKNData
                        .Where(x => x.PackingId == grp.PackingId
                                   && x.Calculation.KGWGT == grp.KgWeight
                                   && x.Calculation.GRADEID == grp.GradeId
                                   && x.Calculation.PCLRID == grp.PclrId
                                   && x.Calculation.RCVDTID == grp.RcvdtId
                                   && x.ProductName == grp.ProductName
                                   && x.TranDate <= previousDate)
                        .Sum(x => x.Calculation.BKN);

                    var selectedDay = allBKNData
                        .Where(x => x.PackingId == grp.PackingId
                                   && x.Calculation.KGWGT == grp.KgWeight
                                   && x.Calculation.GRADEID == grp.GradeId
                                   && x.Calculation.PCLRID == grp.PclrId
                                   && x.Calculation.RCVDTID == grp.RcvdtId
                                   && x.ProductName == grp.ProductName
                                   && x.TranDate == selectedDate)
                        .Sum(x => x.Calculation.BKN);

                    var total = upToPreviousDay + selectedDay;

                    var upToPreviousData = new PackingDetailRow
                    {
                        RowType = $"Up to {previousDate:dd/MM/yyyy}",
                        Values = new List<decimal> { upToPreviousDay },
                        Total = upToPreviousDay
                    };

                    var selectedDayData = new PackingDetailRow
                    {
                        RowType = selectedDate.ToString("dd/MM/yyyy"),
                        Values = new List<decimal> { selectedDay },
                        Total = selectedDay
                    };

                    var totalData = new PackingDetailRow
                    {
                        RowType = "TOTAL",
                        Values = new List<decimal> { total },
                        Total = total
                    };

                    var noOfBoxesData = new PackingDetailRow
                    {
                        RowType = "NO OF CASES",
                        Values = new List<decimal> { 0 },
                        Total = 0
                    };

                    var columnHeaders = new List<string> { "BKN (KG)" };

                    var packingMasterData = new PackingMasterData
                    {
                        PackingType = grp.PackingType,
                        ColumnHeaders = columnHeaders,
                        Rows = new List<PackingDetailRow>
                        {
                            upToPreviousData,
                            selectedDayData,
                            totalData,
                            noOfBoxesData
                        }
                    };

                    breakdown.PackingMasters.Add(packingMasterData);
                    System.Diagnostics.Debug.WriteLine($"  {grp.PackingType}: BKN Total={total:N2}");
                }

                System.Diagnostics.Debug.WriteLine("========================================");
                return breakdown;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR in SupplierStockView.GetBKNDetailBreakdownByDateRangeForSupplier: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                return new PackingMasterBreakdown
                {
                    PackingMasters = new List<PackingMasterData>()
                };
            }
        }

        private PackingMasterBreakdown GetOTHERSDetailBreakdownByDateRangeForSupplier(int supplierId, DateTime selectedDate)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine($"SupplierStockView.GetOTHERSDetailBreakdownByDateRangeForSupplier - SupplierId: {supplierId}, SelectedDate: {selectedDate:yyyy-MM-dd}");

                var breakdown = new PackingMasterBreakdown
                {
                    PackingMasters = new List<PackingMasterData>()
                };

                var allOTHERSData = (from tpc in db.TransactionProductCalculations
                                      join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                      join tm in db.TransactionMasters on td.TRANMID equals tm.TRANMID
                                      join pm in db.PackingMasters on tpc.PACKMID equals pm.PACKMID
                                      join m in db.MaterialMasters on td.MTRLID equals m.MTRLID
                                      join pclr in db.ProductionColourMasters on tpc.PCLRID equals pclr.PCLRID into pclrJoin
                                      from pclr in pclrJoin.DefaultIfEmpty()
                                      join rcvdt in db.ReceivedTypeMasters on tpc.RCVDTID equals rcvdt.RCVDTID into rcvdtJoin
                                      from rcvdt in rcvdtJoin.DefaultIfEmpty()
                                      join grade in db.GradeMasters on tpc.GRADEID equals grade.GRADEID into gradeJoin
                                      from grade in gradeJoin.DefaultIfEmpty()
                                      where (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                            && (pm.DISPSTATUS == 0 || pm.DISPSTATUS == null)
                                            && (tm.DISPSTATUS == 0 || tm.DISPSTATUS == null)
                                            && (m.DISPSTATUS == 0 || m.DISPSTATUS == null)
                                            && tpc.PRODDATE <= selectedDate
                                            && tm.REGSTRID == 1
                                            && tm.TRANREFID == supplierId
                                            && tpc.OTHERS != null && tpc.OTHERS > 0
                                      select new
                                      {
                                          Calculation = tpc,
                                          PackingType = pm.PACKMDESC,
                                          PackingId = pm.PACKMID,
                                          ProductName = m.MTRLDESC,
                                          TranDate = tpc.PRODDATE,
                                          ColourDesc = pclr != null ? pclr.PCLRDESC : null,
                                          ReceivedTypeDesc = rcvdt != null ? rcvdt.RCVDTDESC : null,
                                          GradeDesc = grade != null ? grade.GRADEDESC : null
                                      }).ToList();

                // Deduplicate OTHERS per TRANDID + PACKMID
                allOTHERSData = allOTHERSData
                    .GroupBy(x => new { x.Calculation.TRANDID, x.Calculation.PACKMID })
                    .Select(g => g
                        .OrderBy(c => c.Calculation.PACKTMID)
                        .ThenBy(c => c.Calculation.TRANPID)
                        .First())
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Loaded {allOTHERSData.Count} distinct OTHERS calculation rows for supplier {supplierId} (grouped by TRANDID+PACKMID)");

                var othersGroups = allOTHERSData
                    .GroupBy(x => new
                    {
                        x.PackingId,
                        x.PackingType,
                        KGWGT = x.Calculation.KGWGT,
                        PCLRID = x.Calculation.PCLRID,
                        RCVDTID = x.Calculation.RCVDTID,
                        GRADEID = x.Calculation.GRADEID,
                        x.ProductName,
                        x.ColourDesc,
                        x.ReceivedTypeDesc,
                        x.GradeDesc
                    })
                    .Select(g =>
                    {
                        string displayName = g.Key.PackingType;

                        int packBoxSize = g.Max(x => x.Calculation.PCKBOX);
                        int boxesForDisplay = packBoxSize > 0 ? packBoxSize : 6;

                        if (g.Key.KGWGT > 0)
                            displayName += " " + boxesForDisplay + " x " + g.Key.KGWGT.ToString("0.##");

                        if (!string.IsNullOrEmpty(g.Key.ProductName))
                            displayName += " - " + g.Key.ProductName;

                        if (!string.IsNullOrEmpty(g.Key.GradeDesc))
                            displayName += " - " + g.Key.GradeDesc;

                        if (!string.IsNullOrEmpty(g.Key.ColourDesc))
                            displayName += " - " + g.Key.ColourDesc;

                        if (!string.IsNullOrEmpty(g.Key.ReceivedTypeDesc))
                            displayName += " - " + g.Key.ReceivedTypeDesc;

                        return new
                        {
                            PackingType = displayName,
                            PackingId = g.Key.PackingId,
                            KgWeight = g.Key.KGWGT,
                            PclrId = g.Key.PCLRID,
                            RcvdtId = g.Key.RCVDTID,
                            GradeId = g.Key.GRADEID,
                            ProductName = g.Key.ProductName
                        };
                    })
                    .OrderBy(x => x.PackingId)
                    .ThenBy(x => x.ProductName)
                    .ThenBy(x => x.KgWeight)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Found {othersGroups.Count} OTHERS group combinations for supplier {supplierId}");

                foreach (var grp in othersGroups)
                {
                    var previousDate = selectedDate.AddDays(-1);

                    var upToPreviousDay = allOTHERSData
                        .Where(x => x.PackingId == grp.PackingId
                                   && x.Calculation.KGWGT == grp.KgWeight
                                   && x.Calculation.GRADEID == grp.GradeId
                                   && x.Calculation.PCLRID == grp.PclrId
                                   && x.Calculation.RCVDTID == grp.RcvdtId
                                   && x.ProductName == grp.ProductName
                                   && x.TranDate <= previousDate)
                        .Sum(x => x.Calculation.OTHERS);

                    var selectedDay = allOTHERSData
                        .Where(x => x.PackingId == grp.PackingId
                                   && x.Calculation.KGWGT == grp.KgWeight
                                   && x.Calculation.GRADEID == grp.GradeId
                                   && x.Calculation.PCLRID == grp.PclrId
                                   && x.Calculation.RCVDTID == grp.RcvdtId
                                   && x.ProductName == grp.ProductName
                                   && x.TranDate == selectedDate)
                        .Sum(x => x.Calculation.OTHERS);

                    var total = upToPreviousDay + selectedDay;

                    var upToPreviousData = new PackingDetailRow
                    {
                        RowType = $"Up to {previousDate:dd/MM/yyyy}",
                        Values = new List<decimal> { upToPreviousDay },
                        Total = upToPreviousDay
                    };

                    var selectedDayData = new PackingDetailRow
                    {
                        RowType = selectedDate.ToString("dd/MM/yyyy"),
                        Values = new List<decimal> { selectedDay },
                        Total = selectedDay
                    };

                    var totalData = new PackingDetailRow
                    {
                        RowType = "TOTAL",
                        Values = new List<decimal> { total },
                        Total = total
                    };

                    var noOfBoxesData = new PackingDetailRow
                    {
                        RowType = "NO OF CASES",
                        Values = new List<decimal> { CalculateBoxes(total) },
                        Total = CalculateBoxes(total)
                    };

                    var columnHeaders = new List<string> { "Others(Peeled) (KG)" };

                    var packingMasterData = new PackingMasterData
                    {
                        PackingType = grp.PackingType,
                        ColumnHeaders = columnHeaders,
                        Rows = new List<PackingDetailRow>
                        {
                            upToPreviousData,
                            selectedDayData,
                            totalData,
                            noOfBoxesData
                        }
                    };

                    breakdown.PackingMasters.Add(packingMasterData);
                    System.Diagnostics.Debug.WriteLine($"  {grp.PackingType}: OTHERS Total={total:N2}");
                }

                System.Diagnostics.Debug.WriteLine("========================================");
                return breakdown;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR in SupplierStockView.GetOTHERSDetailBreakdownByDateRangeForSupplier: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                return new PackingMasterBreakdown
                {
                    PackingMasters = new List<PackingMasterData>()
                };
            }
        }

        private decimal CalculateBoxes(decimal? totalValue)
        {
            return CalculateBoxes(totalValue, 6);
        }

        private decimal CalculateBoxes(decimal? totalValue, int boxSize)
        {
            if (boxSize <= 0)
            {
                boxSize = 6;
            }

            decimal boxes = (totalValue ?? 0) / boxSize;
            decimal floorValue = Math.Floor(boxes);
            return floorValue >= 1 ? floorValue : 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
