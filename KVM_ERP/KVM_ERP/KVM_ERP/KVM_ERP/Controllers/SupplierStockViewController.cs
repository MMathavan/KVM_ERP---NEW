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

                // Split-wise production: group by TRANDID and Production Date
                var grouped = allCalcs
                    .GroupBy(c => new { Date = c.PRODDATE.Value.Date, c.TRANDID })
                    .OrderBy(g => g.Key.Date)
                    .ThenBy(g => g.Key.TRANDID)
                    .ToList();

                var productionRows = new List<object>();
                decimal runningBalance = transaction.CLIENTWGHT;

                foreach (var g in grouped)
                {
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

                var allCalcs = (from tpc in db.TransactionProductCalculations
                                join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                join m in db.MaterialMasters on td.MTRLID equals m.MTRLID
                                join tm in db.TransactionMasters on td.TRANMID equals tm.TRANMID
                                where (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                      && (m.DISPSTATUS == 0 || m.DISPSTATUS == null)
                                      && (tm.DISPSTATUS == 0 || tm.DISPSTATUS == null)
                                      && tpc.PRODDATE <= asOnDate
                                      && tm.REGSTRID == 1
                                      && tm.TRANREFID == supplierId
                                select new
                                {
                                    ProductId = m.MTRLID,
                                    ProductName = m.MTRLDESC,
                                    tpc.PCK1,
                                    tpc.PCK2,
                                    tpc.PCK3,
                                    tpc.PCK4,
                                    tpc.PCK5,
                                    tpc.PCK6,
                                    tpc.PCK7,
                                    tpc.PCK8,
                                    tpc.PCK9,
                                    tpc.PCK10,
                                    tpc.PCK11,
                                    tpc.PCK12,
                                    tpc.PCK13,
                                    tpc.PCK14,
                                    tpc.PCK15,
                                    tpc.PCK16,
                                    tpc.PCK17,
                                    PackingId = tpc.PACKMID,
                                    KgWeight = tpc.KGWGT,
                                    GradeId = tpc.GRADEID,
                                    PclrId = tpc.PCLRID,
                                    RcvdtId = tpc.RCVDTID
                                }).ToList();

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
                        TotalPCK = g.Sum(tpc =>
                            tpc.PCK1 + tpc.PCK2 + tpc.PCK3 +
                            tpc.PCK4 + tpc.PCK5 + tpc.PCK6 +
                            tpc.PCK7 + tpc.PCK8 + tpc.PCK9 +
                            tpc.PCK10 + tpc.PCK11 + tpc.PCK12 +
                            tpc.PCK13 + tpc.PCK14 + tpc.PCK15 +
                            tpc.PCK16 + tpc.PCK17)
                    })
                    .Where(x => x.TotalPCK > 0)
                    .OrderBy(x => x.ProductName)
                    .ToList();

                foreach (var product in productCalcs)
                {
                    var productRows = allCalcs
                        .Where(x => x.ProductId == product.ProductId)
                        .ToList();

                    decimal totalCases = 0;

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
                        decimal col1 = grp.Sum(r => r.PCK1);
                        decimal col2 = grp.Sum(r => r.PCK2);
                        decimal col3 = grp.Sum(r => r.PCK3);
                        decimal col4 = grp.Sum(r => r.PCK4);
                        decimal col5 = grp.Sum(r => r.PCK5);
                        decimal col6 = grp.Sum(r => r.PCK6);
                        decimal col7 = grp.Sum(r => r.PCK7);
                        decimal col8 = grp.Sum(r => r.PCK8);
                        decimal col9 = grp.Sum(r => r.PCK9);
                        decimal col10 = grp.Sum(r => r.PCK10);
                        decimal col11 = grp.Sum(r => r.PCK11);
                        decimal col12 = grp.Sum(r => r.PCK12);
                        decimal col13 = grp.Sum(r => r.PCK13);
                        decimal col14 = grp.Sum(r => r.PCK14);
                        decimal col15 = grp.Sum(r => r.PCK15);
                        decimal col16 = grp.Sum(r => r.PCK16);
                        decimal col17 = grp.Sum(r => r.PCK17);

                        decimal groupCases =
                            CalculateBoxes(col1) +
                            CalculateBoxes(col2) +
                            CalculateBoxes(col3) +
                            CalculateBoxes(col4) +
                            CalculateBoxes(col5) +
                            CalculateBoxes(col6) +
                            CalculateBoxes(col7) +
                            CalculateBoxes(col8) +
                            CalculateBoxes(col9) +
                            CalculateBoxes(col10) +
                            CalculateBoxes(col11) +
                            CalculateBoxes(col12) +
                            CalculateBoxes(col13) +
                            CalculateBoxes(col14) +
                            CalculateBoxes(col15) +
                            CalculateBoxes(col16) +
                            CalculateBoxes(col17);

                        totalCases += groupCases;
                    }

                    productTotals.Add(new object[]
                    {
                        product.ProductId,
                        product.ProductName,
                        product.TotalPCK.ToString("N2"),
                        totalCases.ToString("N0")
                    });
                }

                var bknData = (from tpc in db.TransactionProductCalculations
                               join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                               join tm in db.TransactionMasters on td.TRANMID equals tm.TRANMID
                               where (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                     && (tm.DISPSTATUS == 0 || tm.DISPSTATUS == null)
                                     && tpc.PRODDATE <= asOnDate
                                     && tm.REGSTRID == 1
                                     && tm.TRANREFID == supplierId
                                     && tpc.BKN > 0
                               select tpc.BKN).ToList();

                var bknTotal = bknData.Sum();

                if (bknTotal > 0)
                {
                    var bknCases = CalculateBoxes(bknTotal);

                    productTotals.Add(new object[]
                    {
                        -1,
                        "BKN (Broken)",
                        bknTotal.ToString("N2"),
                        bknCases.ToString("N0")
                    });
                }

                var othersData = (from tpc in db.TransactionProductCalculations
                                 join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                 join tm in db.TransactionMasters on td.TRANMID equals tm.TRANMID
                                 where (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                       && (tm.DISPSTATUS == 0 || tm.DISPSTATUS == null)
                                       && tpc.PRODDATE <= asOnDate
                                       && tm.REGSTRID == 1
                                       && tm.TRANREFID == supplierId
                                       && tpc.OTHERS > 0
                                 select tpc.OTHERS).ToList();

                var othersTotal = othersData.Sum();

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

                // Exclude records that have no slab values (all PCK1-PCK17 are null or zero) so that
                // BKN/OTHERS-only rows do not create empty packing master tables under normal products.
                allCalculations = allCalculations
                    .Where(x =>
                        x.Calculation.PCK1 > 0 ||
                        x.Calculation.PCK2 > 0 ||
                        x.Calculation.PCK3 > 0 ||
                        x.Calculation.PCK4 > 0 ||
                        x.Calculation.PCK5 > 0 ||
                        x.Calculation.PCK6 > 0 ||
                        x.Calculation.PCK7 > 0 ||
                        x.Calculation.PCK8 > 0 ||
                        x.Calculation.PCK9 > 0 ||
                        x.Calculation.PCK10 > 0 ||
                        x.Calculation.PCK11 > 0 ||
                        x.Calculation.PCK12 > 0 ||
                        x.Calculation.PCK13 > 0 ||
                        x.Calculation.PCK14 > 0 ||
                        x.Calculation.PCK15 > 0 ||
                        x.Calculation.PCK16 > 0 ||
                        x.Calculation.PCK17 > 0)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"After filtering slab records, remaining calculations for supplier {supplierId}: {allCalculations.Count}");

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

                        if (g.Key.KGWGT > 0)
                            displayName += " 6 x " + g.Key.KGWGT.ToString("0.##");

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
                            GradeId = g.Key.GRADEID
                        };
                    })
                    .OrderBy(x => x.PackingId)
                    .ThenBy(x => x.KgWeight)
                    .ThenBy(x => x.GradeId)
                    .ThenBy(x => x.PclrId)
                    .ThenBy(x => x.RcvdtId)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Found {packingMasters.Count} packing master+KGWGT combinations for supplier {supplierId}");

                foreach (var pm in packingMasters)
                {
                    System.Diagnostics.Debug.WriteLine($"  - {pm.PackingType} (PackingId: {pm.PackingId}, KgWeight: {pm.KgWeight})");

                    var previousDate = selectedDate.AddDays(-1);

                    var upToPreviousDay = allCalculations
                        .Where(x => x.PackingId == pm.PackingId
                                   && x.Calculation.KGWGT == pm.KgWeight
                                   && x.Calculation.GRADEID == pm.GradeId
                                   && x.Calculation.PCLRID == pm.PclrId
                                   && x.Calculation.RCVDTID == pm.RcvdtId
                                   && x.TranDate <= previousDate)
                        .Select(x => x.Calculation)
                        .ToList();

                    var selectedDay = allCalculations
                        .Where(x => x.PackingId == pm.PackingId
                                   && x.Calculation.KGWGT == pm.KgWeight
                                   && x.Calculation.GRADEID == pm.GradeId
                                   && x.Calculation.PCLRID == pm.PclrId
                                   && x.Calculation.RCVDTID == pm.RcvdtId
                                   && x.TranDate == selectedDate)
                        .Select(x => x.Calculation)
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"  Found {upToPreviousDay.Count} records up to previous day");
                    System.Diagnostics.Debug.WriteLine($"  Found {selectedDay.Count} records for selected day");

                    var upToPreviousData = new PackingDetailRow
                    {
                        RowType = $"Up to {previousDate:dd/MM/yyyy}",
                        PCK1 = upToPreviousDay.Sum(x => x.PCK1),
                        PCK2 = upToPreviousDay.Sum(x => x.PCK2),
                        PCK3 = upToPreviousDay.Sum(x => x.PCK3),
                        PCK4 = upToPreviousDay.Sum(x => x.PCK4),
                        PCK5 = upToPreviousDay.Sum(x => x.PCK5),
                        PCK6 = upToPreviousDay.Sum(x => x.PCK6),
                        PCK7 = upToPreviousDay.Sum(x => x.PCK7),
                        PCK8 = upToPreviousDay.Sum(x => x.PCK8),
                        PCK9 = upToPreviousDay.Sum(x => x.PCK9),
                        PCK10 = upToPreviousDay.Sum(x => x.PCK10),
                        PCK11 = upToPreviousDay.Sum(x => x.PCK11),
                        PCK12 = upToPreviousDay.Sum(x => x.PCK12),
                        PCK13 = upToPreviousDay.Sum(x => x.PCK13),
                        PCK14 = upToPreviousDay.Sum(x => x.PCK14),
                        PCK15 = upToPreviousDay.Sum(x => x.PCK15),
                        PCK16 = upToPreviousDay.Sum(x => x.PCK16),
                        PCK17 = upToPreviousDay.Sum(x => x.PCK17)
                    };
                    upToPreviousData.Total = (upToPreviousData.PCK1 ?? 0) + (upToPreviousData.PCK2 ?? 0) + (upToPreviousData.PCK3 ?? 0) +
                                            (upToPreviousData.PCK4 ?? 0) + (upToPreviousData.PCK5 ?? 0) + (upToPreviousData.PCK6 ?? 0) +
                                            (upToPreviousData.PCK7 ?? 0) + (upToPreviousData.PCK8 ?? 0) + (upToPreviousData.PCK9 ?? 0) +
                                            (upToPreviousData.PCK10 ?? 0) + (upToPreviousData.PCK11 ?? 0) + (upToPreviousData.PCK12 ?? 0) +
                                            (upToPreviousData.PCK13 ?? 0) + (upToPreviousData.PCK14 ?? 0) + (upToPreviousData.PCK15 ?? 0) +
                                            (upToPreviousData.PCK16 ?? 0) + (upToPreviousData.PCK17 ?? 0);

                    var selectedDayData = new PackingDetailRow
                    {
                        RowType = selectedDate.ToString("dd/MM/yyyy"),
                        PCK1 = selectedDay.Sum(x => x.PCK1),
                        PCK2 = selectedDay.Sum(x => x.PCK2),
                        PCK3 = selectedDay.Sum(x => x.PCK3),
                        PCK4 = selectedDay.Sum(x => x.PCK4),
                        PCK5 = selectedDay.Sum(x => x.PCK5),
                        PCK6 = selectedDay.Sum(x => x.PCK6),
                        PCK7 = selectedDay.Sum(x => x.PCK7),
                        PCK8 = selectedDay.Sum(x => x.PCK8),
                        PCK9 = selectedDay.Sum(x => x.PCK9),
                        PCK10 = selectedDay.Sum(x => x.PCK10),
                        PCK11 = selectedDay.Sum(x => x.PCK11),
                        PCK12 = selectedDay.Sum(x => x.PCK12),
                        PCK13 = selectedDay.Sum(x => x.PCK13),
                        PCK14 = selectedDay.Sum(x => x.PCK14),
                        PCK15 = selectedDay.Sum(x => x.PCK15),
                        PCK16 = selectedDay.Sum(x => x.PCK16),
                        PCK17 = selectedDay.Sum(x => x.PCK17)
                    };
                    selectedDayData.Total = (selectedDayData.PCK1 ?? 0) + (selectedDayData.PCK2 ?? 0) + (selectedDayData.PCK3 ?? 0) +
                                           (selectedDayData.PCK4 ?? 0) + (selectedDayData.PCK5 ?? 0) + (selectedDayData.PCK6 ?? 0) +
                                           (selectedDayData.PCK7 ?? 0) + (selectedDayData.PCK8 ?? 0) + (selectedDayData.PCK9 ?? 0) +
                                           (selectedDayData.PCK10 ?? 0) + (selectedDayData.PCK11 ?? 0) + (selectedDayData.PCK12 ?? 0) +
                                           (selectedDayData.PCK13 ?? 0) + (selectedDayData.PCK14 ?? 0) + (selectedDayData.PCK15 ?? 0) +
                                           (selectedDayData.PCK16 ?? 0) + (selectedDayData.PCK17 ?? 0);

                    var totalData = new PackingDetailRow
                    {
                        RowType = "TOTAL",
                        PCK1 = (upToPreviousData.PCK1 ?? 0) + (selectedDayData.PCK1 ?? 0),
                        PCK2 = (upToPreviousData.PCK2 ?? 0) + (selectedDayData.PCK2 ?? 0),
                        PCK3 = (upToPreviousData.PCK3 ?? 0) + (selectedDayData.PCK3 ?? 0),
                        PCK4 = (upToPreviousData.PCK4 ?? 0) + (selectedDayData.PCK4 ?? 0),
                        PCK5 = (upToPreviousData.PCK5 ?? 0) + (selectedDayData.PCK5 ?? 0),
                        PCK6 = (upToPreviousData.PCK6 ?? 0) + (selectedDayData.PCK6 ?? 0),
                        PCK7 = (upToPreviousData.PCK7 ?? 0) + (selectedDayData.PCK7 ?? 0),
                        PCK8 = (upToPreviousData.PCK8 ?? 0) + (selectedDayData.PCK8 ?? 0),
                        PCK9 = (upToPreviousData.PCK9 ?? 0) + (selectedDayData.PCK9 ?? 0),
                        PCK10 = (upToPreviousData.PCK10 ?? 0) + (selectedDayData.PCK10 ?? 0),
                        PCK11 = (upToPreviousData.PCK11 ?? 0) + (selectedDayData.PCK11 ?? 0),
                        PCK12 = (upToPreviousData.PCK12 ?? 0) + (selectedDayData.PCK12 ?? 0),
                        PCK13 = (upToPreviousData.PCK13 ?? 0) + (selectedDayData.PCK13 ?? 0),
                        PCK14 = (upToPreviousData.PCK14 ?? 0) + (selectedDayData.PCK14 ?? 0),
                        PCK15 = (upToPreviousData.PCK15 ?? 0) + (selectedDayData.PCK15 ?? 0),
                        PCK16 = (upToPreviousData.PCK16 ?? 0) + (selectedDayData.PCK16 ?? 0),
                        PCK17 = (upToPreviousData.PCK17 ?? 0) + (selectedDayData.PCK17 ?? 0)
                    };
                    totalData.Total = (upToPreviousData.Total ?? 0) + (selectedDayData.Total ?? 0);

                    var noOfBoxesData = new PackingDetailRow
                    {
                        RowType = "NO OF CASES",
                        PCK1 = CalculateBoxes(totalData.PCK1),
                        PCK2 = CalculateBoxes(totalData.PCK2),
                        PCK3 = CalculateBoxes(totalData.PCK3),
                        PCK4 = CalculateBoxes(totalData.PCK4),
                        PCK5 = CalculateBoxes(totalData.PCK5),
                        PCK6 = CalculateBoxes(totalData.PCK6),
                        PCK7 = CalculateBoxes(totalData.PCK7),
                        PCK8 = CalculateBoxes(totalData.PCK8),
                        PCK9 = CalculateBoxes(totalData.PCK9),
                        PCK10 = CalculateBoxes(totalData.PCK10),
                        PCK11 = CalculateBoxes(totalData.PCK11),
                        PCK12 = CalculateBoxes(totalData.PCK12),
                        PCK13 = CalculateBoxes(totalData.PCK13),
                        PCK14 = CalculateBoxes(totalData.PCK14),
                        PCK15 = CalculateBoxes(totalData.PCK15),
                        PCK16 = CalculateBoxes(totalData.PCK16),
                        PCK17 = CalculateBoxes(totalData.PCK17)
                    };
                    noOfBoxesData.Total = (noOfBoxesData.PCK1 ?? 0) + (noOfBoxesData.PCK2 ?? 0) + (noOfBoxesData.PCK3 ?? 0) +
                                         (noOfBoxesData.PCK4 ?? 0) + (noOfBoxesData.PCK5 ?? 0) + (noOfBoxesData.PCK6 ?? 0) +
                                         (noOfBoxesData.PCK7 ?? 0) + (noOfBoxesData.PCK8 ?? 0) + (noOfBoxesData.PCK9 ?? 0) +
                                         (noOfBoxesData.PCK10 ?? 0) + (noOfBoxesData.PCK11 ?? 0) + (noOfBoxesData.PCK12 ?? 0) +
                                         (noOfBoxesData.PCK13 ?? 0) + (noOfBoxesData.PCK14 ?? 0) + (noOfBoxesData.PCK15 ?? 0) +
                                         (noOfBoxesData.PCK16 ?? 0) + (noOfBoxesData.PCK17 ?? 0);

                    var columnHeaders = db.PackingTypeMasters
                        .Where(pt => pt.PACKMID == pm.PackingId
                                  && (pt.DISPSTATUS == 0 || pt.DISPSTATUS == null)
                                  && !pt.PACKTMDESC.ToUpper().Contains("BKN")
                                  && !pt.PACKTMDESC.ToUpper().Contains("BROKEN")
                                  && !pt.PACKTMDESC.ToUpper().Contains("OTHERS")
                                  && !pt.PACKTMDESC.ToUpper().Contains("OTHER"))
                        .OrderBy(pt => pt.PACKTMCODE)
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

                System.Diagnostics.Debug.WriteLine($"Loaded {allBKNData.Count} BKN records for supplier {supplierId}");

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

                        if (g.Key.KGWGT > 0)
                            displayName += " 6 x " + g.Key.KGWGT.ToString("0.##");

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
                        PCK1 = upToPreviousDay,
                        Total = upToPreviousDay
                    };

                    var selectedDayData = new PackingDetailRow
                    {
                        RowType = selectedDate.ToString("dd/MM/yyyy"),
                        PCK1 = selectedDay,
                        Total = selectedDay
                    };

                    var totalData = new PackingDetailRow
                    {
                        RowType = "TOTAL",
                        PCK1 = total,
                        Total = total
                    };

                    var noOfBoxesData = new PackingDetailRow
                    {
                        RowType = "NO OF CASES",
                        PCK1 = CalculateBoxes(total),
                        Total = CalculateBoxes(total)
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

                System.Diagnostics.Debug.WriteLine($"Loaded {allOTHERSData.Count} OTHERS records for supplier {supplierId}");

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

                        if (g.Key.KGWGT > 0)
                            displayName += " 6 x " + g.Key.KGWGT.ToString("0.##");

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
                        PCK1 = upToPreviousDay,
                        Total = upToPreviousDay
                    };

                    var selectedDayData = new PackingDetailRow
                    {
                        RowType = selectedDate.ToString("dd/MM/yyyy"),
                        PCK1 = selectedDay,
                        Total = selectedDay
                    };

                    var totalData = new PackingDetailRow
                    {
                        RowType = "TOTAL",
                        PCK1 = total,
                        Total = total
                    };

                    var noOfBoxesData = new PackingDetailRow
                    {
                        RowType = "NO OF CASES",
                        PCK1 = CalculateBoxes(total),
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
            decimal boxes = (totalValue ?? 0) / 6;
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
