using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KVM_ERP.Models;
using System.Data.SqlClient;
using System.Data;

namespace KVM_ERP.Controllers
{
    [SessionExpire]
    public class StockViewController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: StockView
        [Authorize(Roles = "StockViewIndex")]
        public ActionResult Index()
        {
            ViewBag.Title = "Stock View";
            
            // Test database connection
            try
            {
                var testQuery = "SELECT COUNT(*) FROM MATERIALMASTER";
                var count = db.Database.SqlQuery<int>(testQuery).FirstOrDefault();
                System.Diagnostics.Debug.WriteLine($"Database test - MATERIALMASTER count: {count}");
                ViewBag.MaterialCount = count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database test error: {ex.Message}");
                ViewBag.MaterialCount = -1;
                ViewBag.DatabaseError = ex.Message;
            }
            
            return View();
        }

        [HttpPost]
        public ActionResult GetAjaxData()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("GetAjaxData called - Calculating real slab totals");
                
                var draw = Request.Form.GetValues("draw")?.FirstOrDefault() ?? "1";

                // Parse As On Date from request
                DateTime filterDate = DateTime.Now.Date;
                try
                {
                    var asOnDateStr = Request.Form["asOnDate"] ?? Request["asOnDate"];
                    if (!string.IsNullOrWhiteSpace(asOnDateStr))
                    {
                        DateTime parsed;
                        if (DateTime.TryParse(asOnDateStr, out parsed))
                        {
                            filterDate = parsed.Date;
                        }
                    }
                }
                catch { }

                // Calculate real slab totals from TRANSACTION_PRODUCT_CALCULATION
                var slabTotals = GetSlabTotalsFromDatabase(filterDate);

                var jsonData = new
                {
                    draw = draw,
                    recordsFiltered = slabTotals.Count,
                    recordsTotal = slabTotals.Count,
                    data = slabTotals
                };

                System.Diagnostics.Debug.WriteLine($"Returning {slabTotals.Count} slab total items");
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAjaxData: {ex.Message}");
                return Json(new { 
                    error = ex.Message,
                    draw = "1",
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new object[0]
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // Helper methods to calculate number of boxes
        // Default version uses 6 as the box size (legacy behaviour)
        // Overload accepts a dynamic box size (e.g. PCKBOX per packing master)
        // and always floors to a whole number, returning 0 when less than 1.
        private decimal CalculateBoxes(decimal? totalValue)
        {
            return CalculateBoxes(totalValue, 6);
        }

        private decimal CalculateBoxes(decimal? totalValue, int boxSize)
        {
            if (boxSize <= 0)
            {
                boxSize = 6; // preserve previous behaviour for missing data
            }

            decimal boxes = (totalValue ?? 0) / boxSize;
            decimal floorValue = Math.Floor(boxes);
            
            // Only return value if >= 1, otherwise return 0
            return floorValue >= 1 ? floorValue : 0;
        }

        [HttpPost]
        public ActionResult GetItemDetails(int itemId, string asOnDate, int? calculationMode = null, int? packingId = null)
        {
            try
            {
                DateTime filterDate = DateTime.Now;
                if (!string.IsNullOrEmpty(asOnDate))
                {
                    DateTime.TryParse(asOnDate, out filterDate);
                }

                System.Diagnostics.Debug.WriteLine($"GetItemDetails called - ItemId: {itemId}, AsOnDate: {filterDate:yyyy-MM-dd}, CalculationMode: {calculationMode}, PackingId: {packingId}");

                // Get detailed breakdown split by date ranges for the specific item
                var details = GetItemDetailBreakdownByDateRange(itemId, filterDate, calculationMode, packingId);

                return Json(new
                {
                    success = true,
                    packingMasters = details.PackingMasters,
                    selectedDate = filterDate.ToString("dd/MM/yyyy")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetItemDetails: {ex.Message}");
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private ItemDetailBreakdown GetItemDetailBreakdown(int itemId, DateTime asOnDate)
        {
            try
            {
                var breakdown = new ItemDetailBreakdown
                {
                    Details = new List<ItemDetailCategory>(),
                    TotalWeight = 0
                };

                // itemId here represents the ProductId (MTRLID)
                System.Diagnostics.Debug.WriteLine($"========================================");
                System.Diagnostics.Debug.WriteLine($"Getting breakdown for ProductId: {itemId}, AsOnDate: {asOnDate:yyyy-MM-dd}");

                // First, check all calculations for this product
                var allCalcs = (from tpc in db.TransactionProductCalculations
                               join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                               join tm in db.TransactionMasters on td.TRANMID equals tm.TRANMID
                               join pm in db.PackingMasters on tpc.PACKMID equals pm.PACKMID
                               where td.MTRLID == itemId
                               select new {
                                   tpc.TRANPID,
                                   tpc.PACKMID,
                                   pm.PACKMDESC,
                                   tm.TRANDATE,
                                   tpc.DISPSTATUS,
                                   pm_DISPSTATUS = pm.DISPSTATUS,
                                   tm_DISPSTATUS = tm.DISPSTATUS
                               }).ToList();

                System.Diagnostics.Debug.WriteLine($"Total calculations for product {itemId}: {allCalcs.Count}");
                foreach (var calc in allCalcs)
                {
                    System.Diagnostics.Debug.WriteLine($"  - TRANPID: {calc.TRANPID}, Packing: {calc.PACKMDESC}, Date: {calc.TRANDATE:yyyy-MM-dd}, " +
                                                      $"TPC.DISPSTATUS: {calc.DISPSTATUS}, PM.DISPSTATUS: {calc.pm_DISPSTATUS}, TM.DISPSTATUS: {calc.tm_DISPSTATUS}");
                }

                // Get breakdown by packing master with all PCK values - SHOW ALL PACKING MASTERS
                // NOTE: Use PRODDATE from TransactionProductCalculations as the effective stock date
                var packingBreakdown = (from tpc in db.TransactionProductCalculations
                                       join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                       join tm in db.TransactionMasters on td.TRANMID equals tm.TRANMID
                                       join pm in db.PackingMasters on tpc.PACKMID equals pm.PACKMID
                                       where td.MTRLID == itemId
                                             && (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                             && (pm.DISPSTATUS == 0 || pm.DISPSTATUS == null)
                                             && (tm.DISPSTATUS == 0 || tm.DISPSTATUS == null)
                                             && tpc.PRODDATE <= asOnDate
                                       group tpc by new { pm.PACKMDESC, pm.PACKMID } into g
                                       select new {
                                           PackingType = g.Key.PACKMDESC,
                                           PackingId = g.Key.PACKMID,
                                           PCK1 = g.Sum(tpc => tpc.PCK1),
                                           PCK2 = g.Sum(tpc => tpc.PCK2),
                                           PCK3 = g.Sum(tpc => tpc.PCK3),
                                           PCK4 = g.Sum(tpc => tpc.PCK4),
                                           PCK5 = g.Sum(tpc => tpc.PCK5),
                                           PCK6 = g.Sum(tpc => tpc.PCK6),
                                           PCK7 = g.Sum(tpc => tpc.PCK7),
                                           PCK8 = g.Sum(tpc => tpc.PCK8),
                                           PCK9 = g.Sum(tpc => tpc.PCK9),
                                           PCK10 = g.Sum(tpc => tpc.PCK10),
                                           PCK11 = g.Sum(tpc => tpc.PCK11),
                                           PCK12 = g.Sum(tpc => tpc.PCK12),
                                           PCK13 = g.Sum(tpc => tpc.PCK13),
                                           PCK14 = g.Sum(tpc => tpc.PCK14),
                                           PCK15 = g.Sum(tpc => tpc.PCK15),
                                           PCK16 = g.Sum(tpc => tpc.PCK16),
                                           PCK17 = g.Sum(tpc => tpc.PCK17),
                                           Total = g.Sum(tpc => 
                                               tpc.PCK1 + tpc.PCK2 + tpc.PCK3 + 
                                               tpc.PCK4 + tpc.PCK5 + tpc.PCK6 + 
                                               tpc.PCK7 + tpc.PCK8 + tpc.PCK9 + 
                                               tpc.PCK10 + tpc.PCK11 + tpc.PCK12 + 
                                               tpc.PCK13 + tpc.PCK14 + tpc.PCK15 + 
                                               tpc.PCK16 + tpc.PCK17)
                                       })
                                       .Where(x => x.Total > 0)
                                       .OrderBy(x => x.PackingId) // Order by PackingId to keep consistent order
                                       .ToList();

                System.Diagnostics.Debug.WriteLine($"Found {packingBreakdown.Count} packing master breakdowns after filters:");
                foreach (var packing in packingBreakdown)
                {
                    System.Diagnostics.Debug.WriteLine($"  - {packing.PackingType} (ID: {packing.PackingId}): Total = {packing.Total:N2} KG");
                    System.Diagnostics.Debug.WriteLine($"    PCK Values: PCK1={packing.PCK1}, PCK2={packing.PCK2}, PCK3={packing.PCK3}, PCK4={packing.PCK4}, PCK5={packing.PCK5}");
                }

                // Store the detailed breakdown for table display
                breakdown.PackingDetails = packingBreakdown.Select(p => new {
                    p.PackingType,
                    p.PCK1, p.PCK2, p.PCK3, p.PCK4, p.PCK5, p.PCK6, p.PCK7, p.PCK8, p.PCK9,
                    p.PCK10, p.PCK11, p.PCK12, p.PCK13, p.PCK14, p.PCK15, p.PCK16, p.PCK17,
                    p.Total
                }).ToList();

                decimal totalWeight = packingBreakdown.Sum(p => p.Total);
                breakdown.TotalWeight = totalWeight;

                System.Diagnostics.Debug.WriteLine($"Total weight across all packing masters: {totalWeight:N2} KG");
                System.Diagnostics.Debug.WriteLine($"========================================");

                return breakdown;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in GetItemDetailBreakdown: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new ItemDetailBreakdown
                {
                    Details = new List<ItemDetailCategory>
                    {
                        new ItemDetailCategory
                        {
                            Category = "Error loading breakdown",
                            Weight = "0.000"
                        }
                    },
                    TotalWeight = 0,
                    PackingDetails = new List<object>()
                };
            }
        }

        private PackingMasterBreakdown GetItemDetailBreakdownByDateRange(int itemId, DateTime selectedDate, int? calculationMode, int? packingId = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"========================================");
                System.Diagnostics.Debug.WriteLine($"GetItemDetailBreakdownByDateRange - ProductId: {itemId}, SelectedDate: {selectedDate:yyyy-MM-dd}, CalculationMode: {calculationMode}");

                var breakdown = new PackingMasterBreakdown
                {
                    PackingMasters = new List<PackingMasterData>()
                };

                // Special handling for BKN (itemId = -1)
                if (itemId == -1)
                {
                    return GetBKNDetailBreakdownByDateRange(selectedDate);
                }
                
                // Special handling for OTHERS (itemId = -2)
                if (itemId == -2)
                {
                    return GetOTHERSDetailBreakdownByDateRange(selectedDate);
                }

                // Step 1: Load all calculations for this product into memory with master descriptions
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
                                      select new {
                                          Calculation = tpc,
                                          PackingType = pm.PACKMDESC,
                                          PackingId = pm.PACKMID,
                                          TranDate = tpc.PRODDATE,
                                          ColourDesc = pclr != null ? pclr.PCLRDESC : null,
                                          ReceivedTypeDesc = rcvdt != null ? rcvdt.RCVDTDESC : null,
                                          GradeDesc = grade != null ? grade.GRADEDESC : null,
                                          SupplierName = tm.CATENAME
                                      }).ToList();

                System.Diagnostics.Debug.WriteLine($"Loaded {allCalculations.Count} total calculation records (including BKN/OTHERS)");

                // If a specific packing master is requested (e.g. from Opening Stock form),
                // filter calculations down to that PACKMID before building breakdown rows.
                if (packingId.HasValue)
                {
                    int pid = packingId.Value;
                    allCalculations = allCalculations
                        .Where(x => x.PackingId == pid)
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"After filtering by PackingId={pid}, remaining calculations: {allCalculations.Count}");
                }

                // Exclude records that have no slab values (all PCK1-PCK17 are null or zero).
                // This prevents BKN/OTHERS-only rows from creating empty packing tables under products
                // because BKN and OTHERS are handled as separate virtual products (-1 and -2).
                allCalculations = allCalculations
                    .Where(x =>
                        x.Calculation.PACKTMID != 0 &&
                        x.Calculation.SLABVALUE > 0)
                    .ToList();

                // If a specific calculation mode is requested, filter by normalized mode
                if (calculationMode.HasValue)
                {
                    int modeFilter = calculationMode.Value;
                    allCalculations = allCalculations
                        .Where(x => (x.Calculation.CALCULATIONMODE == 2 ? 2 : 1) == modeFilter)
                        .ToList();
                }

                System.Diagnostics.Debug.WriteLine($"After filtering slab records, remaining calculations: {allCalculations.Count}");

                // Step 2: Group in memory by PackingId + KGWGT + PCLRID + RCVDTID + GRADEID
                var packingMasters = allCalculations
                    .GroupBy(x => new { 
                        x.PackingId, 
                        x.PackingType, 
                        KGWGT = x.Calculation.KGWGT,
                        PCLRID = x.Calculation.PCLRID,
                        RCVDTID = x.Calculation.RCVDTID,
                        GRADEID = x.Calculation.GRADEID,
                        x.ColourDesc,
                        x.ReceivedTypeDesc,
                        x.GradeDesc,
                        x.SupplierName
                    })
                    .Select(g => {
                        // Build display name
                        string displayName = g.Key.PackingType;
                        
                        // Use per-pack No of Boxes (PCKBOX) as the pack size shown in the
                        // header (e.g. "5 x 4.2"). If no PCKBOX data is present yet,
                        // fall back to 6 to preserve behaviour for old records.
                        int packBoxSize = g.Max(x => x.Calculation.PCKBOX);
                        int boxesForDisplay = packBoxSize > 0 ? packBoxSize : 6;

                        // Add KGWGT if present, prefixed by pack size
                        if (g.Key.KGWGT > 0)
                            displayName += " " + boxesForDisplay + " x " + g.Key.KGWGT.ToString("0.##");
                        
                        // Add Grade if present
                        if (!string.IsNullOrEmpty(g.Key.GradeDesc))
                            displayName += " - " + g.Key.GradeDesc;
                        
                        // Add Colour if present
                        if (!string.IsNullOrEmpty(g.Key.ColourDesc))
                            displayName += " - " + g.Key.ColourDesc;
                        
                        // Add Received Type if present
                        if (!string.IsNullOrEmpty(g.Key.ReceivedTypeDesc))
                            displayName += " - " + g.Key.ReceivedTypeDesc;

                        // Add Supplier Name if present
                        if (!string.IsNullOrEmpty(g.Key.SupplierName))
                            displayName += " - " + g.Key.SupplierName;
                        
                        return new {
                            PackingType = displayName,
                            PackingId = g.Key.PackingId,
                            KgWeight = g.Key.KGWGT,
                            PclrId = g.Key.PCLRID,
                            RcvdtId = g.Key.RCVDTID,
                            GradeId = g.Key.GRADEID,
                            BoxSize = boxesForDisplay,
                            SupplierName = g.Key.SupplierName
                        };
                    })
                    .OrderBy(x => x.PackingId)
                    .ThenBy(x => x.KgWeight)
                    .ThenBy(x => x.GradeId)
                    .ThenBy(x => x.PclrId)
                    .ThenBy(x => x.RcvdtId)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Found {packingMasters.Count} packing master+KGWGT combinations");
                foreach (var pm in packingMasters)
                {
                    System.Diagnostics.Debug.WriteLine($"  - {pm.PackingType} (PackingId: {pm.PackingId}, KgWeight: {pm.KgWeight})");
                }

                // For each packing master, get data split by date ranges
                foreach (var pm in packingMasters)
                {
                    System.Diagnostics.Debug.WriteLine($"Processing packing master: {pm.PackingType}");

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

                    // Use all available packing types as dynamic columns
                    var effectivePackingTypes = packingTypes;
                    int columnCount = effectivePackingTypes.Count;

                    if (columnCount == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"  {pm.PackingType}: No packing types found, skipping.");
                        continue;
                    }

                    // Filter from already loaded data in memory by PackingId, KGWGT, GRADEID, PCLRID, RCVDTID and Supplier
                    var rowsForPacking = allCalculations
                        .Where(x => x.PackingId == pm.PackingId 
                                   && x.Calculation.KGWGT == pm.KgWeight
                                   && x.Calculation.GRADEID == pm.GradeId
                                   && x.Calculation.PCLRID == pm.PclrId
                                   && x.Calculation.RCVDTID == pm.RcvdtId
                                   && x.SupplierName == pm.SupplierName)
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

                    // Calculate NO OF CASES row using dynamic box size (PCKBOX per packing master)
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

                    // Get column headers from PACKINGTYPEMASTER for this packing master.
                    // Use all effective packing types so headers and data columns stay perfectly aligned.
                    var columnHeaders = effectivePackingTypes
                        .Select(pt => pt.PACKTMDESC)
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"  {pm.PackingType}: Found {columnHeaders.Count} column headers");
                    foreach (var header in columnHeaders)
                    {
                        System.Diagnostics.Debug.WriteLine($"    - {header}");
                    }

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

                System.Diagnostics.Debug.WriteLine($"========================================");
                return breakdown;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in GetItemDetailBreakdownByDateRange: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new PackingMasterBreakdown
                {
                    PackingMasters = new List<PackingMasterData>()
                };
            }
        }

        private PackingMasterBreakdown GetBKNDetailBreakdownByDateRange(DateTime selectedDate)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"========================================");
                System.Diagnostics.Debug.WriteLine($"GetBKNDetailBreakdownByDateRange - SelectedDate: {selectedDate:yyyy-MM-dd}");

                var breakdown = new PackingMasterBreakdown
                {
                    PackingMasters = new List<PackingMasterData>()
                };

                // Load all BKN data from calculations (filter by PRODDATE instead of TRANDATE)
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
                                       && tpc.BKN != null && tpc.BKN > 0
                                 select new {
                                     Calculation = tpc,
                                     PackingType = pm.PACKMDESC,
                                     PackingId = pm.PACKMID,
                                     ProductName = m.MTRLDESC,
                                     TranDate = tpc.PRODDATE,
                                     ColourDesc = pclr != null ? pclr.PCLRDESC : null,
                                     ReceivedTypeDesc = rcvdt != null ? rcvdt.RCVDTDESC : null,
                                     GradeDesc = grade != null ? grade.GRADEDESC : null
                                 }).ToList();

                // IMPORTANT: BKN is stored on every slab row for a TRANDID+PACKMID combination
                // in the new headless design. To avoid multiplying BKN by the number of slab
                // rows, we must collapse to one record per TRANDID+PACKMID and use a single
                // BKN value (they are identical within the group).
                allBKNData = allBKNData
                    .GroupBy(x => new { x.Calculation.TRANDID, x.Calculation.PACKMID })
                    .Select(g => g
                        .OrderBy(c => c.Calculation.PACKTMID) // header (PACKTMID = 0) first when present
                        .ThenBy(c => c.Calculation.TRANPID)
                        .First())
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Loaded {allBKNData.Count} distinct BKN calculation rows (grouped by TRANDID+PACKMID)");

                // Group by Packing Master + KGWGT + Grade + Colour + Received Type + Product
                var bknGroups = allBKNData
                    .GroupBy(x => new {
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
                    .Select(g => {
                        // Build display name
                        string displayName = g.Key.PackingType;

                        // Use per-pack No of Boxes (PCKBOX) for the header (pack size),
                        // not a sum across slabs. If there is no PCKBOX data yet, fall
                        // back to 6 to preserve behaviour for older data.
                        int packBoxSizeBkn = g.Max(x => x.Calculation.PCKBOX);
                        int boxesForDisplay = packBoxSizeBkn > 0 ? packBoxSizeBkn : 6;

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
                        
                        return new {
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

                System.Diagnostics.Debug.WriteLine($"Found {bknGroups.Count} BKN group combinations");

                // For each group, calculate BKN totals by date
                foreach (var grp in bknGroups)
                {
                    var previousDate = selectedDate.AddDays(-1);

                    // Filter BKN data for this specific group
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

                    // Create rows for this BKN group using PCK1 to store BKN value
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

                    // Create single column header for BKN
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

                System.Diagnostics.Debug.WriteLine($"========================================");
                return breakdown;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in GetBKNDetailBreakdownByDateRange: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new PackingMasterBreakdown
                {
                    PackingMasters = new List<PackingMasterData>()
                };
            }
        }

        private PackingMasterBreakdown GetOTHERSDetailBreakdownByDateRange(DateTime selectedDate)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"========================================");
                System.Diagnostics.Debug.WriteLine($"GetOTHERSDetailBreakdownByDateRange - SelectedDate: {selectedDate:yyyy-MM-dd}");

                var breakdown = new PackingMasterBreakdown
                {
                    PackingMasters = new List<PackingMasterData>()
                };

                // Load all OTHERS data from calculations (filter by PRODDATE instead of TRANDATE)
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
                                       && tpc.OTHERS != null && tpc.OTHERS > 0
                                 select new {
                                     Calculation = tpc,
                                     PackingType = pm.PACKMDESC,
                                     PackingId = pm.PACKMID,
                                     ProductName = m.MTRLDESC,
                                     TranDate = tpc.PRODDATE,
                                     ColourDesc = pclr != null ? pclr.PCLRDESC : null,
                                     ReceivedTypeDesc = rcvdt != null ? rcvdt.RCVDTDESC : null,
                                     GradeDesc = grade != null ? grade.GRADEDESC : null
                                 }).ToList();

                // IMPORTANT: OTHERS is stored on every slab row for a TRANDID+PACKMID combination
                // in the new headless design. To avoid multiplying OTHERS by the number of slab
                // rows, we must collapse to one record per TRANDID+PACKMID and use a single
                // OTHERS value (they are identical within the group).
                allOTHERSData = allOTHERSData
                    .GroupBy(x => new { x.Calculation.TRANDID, x.Calculation.PACKMID })
                    .Select(g => g
                        .OrderBy(c => c.Calculation.PACKTMID) // header (PACKTMID = 0) first when present
                        .ThenBy(c => c.Calculation.TRANPID)
                        .First())
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Loaded {allOTHERSData.Count} distinct OTHERS calculation rows (grouped by TRANDID+PACKMID)");

                // Group by Packing Master + KGWGT + Grade + Colour + Received Type + Product
                var othersGroups = allOTHERSData
                    .GroupBy(x => new {
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
                    .Select(g => {
                        // Build display name
                        string displayName = g.Key.PackingType;

                        // Use per-pack No of Boxes (PCKBOX) for the header (pack size),
                        // not a sum across slabs. If there is no PCKBOX data yet, fall
                        // back to 6 to preserve behaviour for older data.
                        int packBoxSizeOthers = g.Max(x => x.Calculation.PCKBOX);
                        int boxesForDisplay = packBoxSizeOthers > 0 ? packBoxSizeOthers : 6;

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
                        
                        return new {
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

                System.Diagnostics.Debug.WriteLine($"Found {othersGroups.Count} OTHERS group combinations");

                // For each group, calculate OTHERS totals by date
                foreach (var grp in othersGroups)
                {
                    var previousDate = selectedDate.AddDays(-1);

                    // Filter OTHERS data for this specific group
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

                    // Create rows for this OTHERS group using PCK1 to store OTHERS value
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

                    // Create single column header for OTHERS
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

                System.Diagnostics.Debug.WriteLine($"========================================");
                return breakdown;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in GetOTHERSDetailBreakdownByDateRange: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new PackingMasterBreakdown
                {
                    PackingMasters = new List<PackingMasterData>()
                };
            }
        }

        private string GetSlabSizeLabel(int slabIndex)
        {
            var slabLabels = new Dictionary<int, string>
            {
                { 1, "U-5" },
                { 2, "U-6" },
                { 3, "U-7" },
                { 4, "U-8" },
                { 5, "U-9" },
                { 6, "U-10" },
                { 7, "U-12" },
                { 8, "U-15" },
                { 9, "U-20" },
                { 10, "U-30" },
                { 11, "U-40" },
                { 12, "U-50" },
                { 13, "U-60" },
                { 14, "16-20" },
                { 15, "21-25" },
                { 16, "26-30" },
                { 17, "31-40" }
            };

            return slabLabels.ContainsKey(slabIndex) ? slabLabels[slabIndex] : $"PCK{slabIndex}";
        }

        private List<StockViewData> GetStockData(DateTime asOnDate, string searchValue, string sortColumn, string sortDirection)
        {
            try
            {
                var stockData = new List<StockViewData>();

                // Optimized query - get materials with stock totals in single query
                var materialsWithStock = db.Database.SqlQuery<dynamic>(@"
                    SELECT TOP 100
                        m.MTRLID as ItemId,
                        m.MTRLCODE as ItemCode,
                        m.MTRLDESC as ItemDescription,
                        ISNULL(u.UNITDESC, 'KG') as Unit,
                        ISNULL(SUM(CAST(ISNULL(td.BOXES, 0) as DECIMAL(18,2))), 0) as ItemTotal
                    FROM MATERIALMASTER m
                    LEFT JOIN UNITMASTER u ON m.UNITID = u.UNITID
                    LEFT JOIN TRANSACTIONDETAILS td ON m.MTRLID = td.MTRLID
                    LEFT JOIN TRANSACTIONMASTER tm ON td.TRANMID = tm.TRANMID 
                        AND tm.TRANDATE <= @AsOnDate 
                        AND (tm.DISPSTATUS = 0 OR tm.DISPSTATUS IS NULL)
                    WHERE (m.DISPSTATUS = 0 OR m.DISPSTATUS IS NULL)
                    GROUP BY m.MTRLID, m.MTRLCODE, m.MTRLDESC, u.UNITDESC
                    ORDER BY m.MTRLDESC
                ", new SqlParameter("@AsOnDate", asOnDate)).ToList();

                System.Diagnostics.Debug.WriteLine($"Found {materialsWithStock.Count} materials with stock data");

                foreach (var material in materialsWithStock)
                {
                    try
                    {
                        decimal itemTotal = Convert.ToDecimal(material.ItemTotal ?? 0);
                        
                        // If no real transactions, add sample data for testing
                        if (itemTotal == 0)
                        {
                            itemTotal = (decimal)(new Random().NextDouble() * 100 + 10);
                        }
                        
                        stockData.Add(new StockViewData
                        {
                            ItemId = (int)material.ItemId,
                            ItemCode = material.ItemCode?.ToString() ?? "",
                            ItemDescription = material.ItemDescription?.ToString() ?? "Unknown Item",
                            ItemTotal = itemTotal,
                            Unit = material.Unit?.ToString() ?? "KG",
                            LastUpdated = DateTime.Now
                        });

                        System.Diagnostics.Debug.WriteLine($"Added item: {material.ItemDescription} with total: {itemTotal}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error processing material {material.ItemCode}: {ex.Message}");
                    }
                }

                // Apply search filter
                if (!string.IsNullOrEmpty(searchValue))
                {
                    stockData = stockData.Where(x => 
                        (x.ItemCode != null && x.ItemCode.ToLower().Contains(searchValue.ToLower())) ||
                        (x.ItemDescription != null && x.ItemDescription.ToLower().Contains(searchValue.ToLower())) ||
                        (x.Unit != null && x.Unit.ToLower().Contains(searchValue.ToLower()))
                    ).ToList();
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(sortColumn))
                {
                    switch (sortColumn.ToLower())
                    {
                        case "itemcode":
                            stockData = sortDirection == "asc" ? 
                                stockData.OrderBy(x => x.ItemCode).ToList() : 
                                stockData.OrderByDescending(x => x.ItemCode).ToList();
                            break;
                        case "itemdescription":
                            stockData = sortDirection == "asc" ? 
                                stockData.OrderBy(x => x.ItemDescription).ToList() : 
                                stockData.OrderByDescending(x => x.ItemDescription).ToList();
                            break;
                        case "itemtotal":
                            stockData = sortDirection == "asc" ? 
                                stockData.OrderBy(x => x.ItemTotal).ToList() : 
                                stockData.OrderByDescending(x => x.ItemTotal).ToList();
                            break;
                        case "unit":
                            stockData = sortDirection == "asc" ? 
                                stockData.OrderBy(x => x.Unit).ToList() : 
                                stockData.OrderByDescending(x => x.Unit).ToList();
                            break;
                        default:
                            stockData = stockData.OrderBy(x => x.ItemCode).ToList();
                            break;
                    }
                }
                else
                {
                    stockData = stockData.OrderBy(x => x.ItemCode).ToList();
                }

                return stockData;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetStockData: {ex.Message}");
                return new List<StockViewData>();
            }
        }

        private List<object[]> GetSlabTotalsFromDatabase(DateTime asOnDate)
        {
            try
            {
                var productTotals = new List<object[]>();

                System.Diagnostics.Debug.WriteLine($"Starting GetSlabTotalsFromDatabase for date: {asOnDate:yyyy-MM-dd}");

                // STEP 1: Check if TRANSACTION_PRODUCT_CALCULATION table has any data at all
                try
                {
                    var totalCalcCount = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM TRANSACTION_PRODUCT_CALCULATION").FirstOrDefault();
                    System.Diagnostics.Debug.WriteLine($"Total TRANSACTION_PRODUCT_CALCULATION records: {totalCalcCount}");
                    
                    if (totalCalcCount == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("INFO: No data in TRANSACTION_PRODUCT_CALCULATION table");
                        // Return empty list - no stock data exists yet
                        return new List<object[]>();
                    }

                    // STEP 2: Check for calculation records with slab or BKN/OTHERS values within the date range
                    var rawCalcCount = db.Database.SqlQuery<int>(@"
                        SELECT COUNT(*) 
                        FROM TRANSACTION_PRODUCT_CALCULATION tpc
                        INNER JOIN TRANSACTIONDETAIL td ON tpc.TRANDID = td.TRANDID
                        INNER JOIN TRANSACTIONMASTER tm ON td.TRANMID = tm.TRANMID
                        WHERE (tpc.DISPSTATUS = 0 OR tpc.DISPSTATUS IS NULL)
                        AND (tm.DISPSTATUS = 0 OR tm.DISPSTATUS IS NULL)
                        AND tm.REGSTRID = 1
                        AND tpc.PRODDATE <= @p0
                        AND (tpc.SLABVALUE > 0 OR tpc.BKN > 0 OR tpc.OTHERS > 0)
                    ", asOnDate).FirstOrDefault();

                    System.Diagnostics.Debug.WriteLine($"Found {rawCalcCount} calculation records with slab/BKN/OTHERS values > 0 for date {asOnDate:yyyy-MM-dd}");

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error checking calculation data: {ex.Message}");
                    // Return empty list on error - will be handled by main try-catch if serious
                    return new List<object[]>();
                }

                // STEP 3: Get products with their PCK totals
                try
                {
                    // Load packing type metadata once to map PACKTMID -> logical column order (PCK1..PCK17)
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

                    // Load slab-based calculation data into memory first to avoid EF translation issues
                    var allCalcs = (from tpc in db.TransactionProductCalculations
                                   join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                   join m in db.MaterialMasters on td.MTRLID equals m.MTRLID
                                   join tm in db.TransactionMasters on td.TRANMID equals tm.TRANMID
                                    where (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                          && (m.DISPSTATUS == 0 || m.DISPSTATUS == null)
                                          && (tm.DISPSTATUS == 0 || tm.DISPSTATUS == null)
                                          && tm.REGSTRID == 1
                                          && tpc.PRODDATE <= asOnDate
                                          && tpc.PACKTMID != 0
                                          && tpc.SLABVALUE > 0
                                   select new {
                                       ProductId = m.MTRLID,
                                       ProductName = m.MTRLDESC,
                                       PackingId = tpc.PACKMID,
                                       KgWeight = tpc.KGWGT,
                                       GradeId = tpc.GRADEID,
                                       PclrId = tpc.PCLRID,
                                       RcvdtId = tpc.RCVDTID,
                                       SupplierName = tm.CATENAME,
                                       BoxSize = tpc.PCKBOX,
                                       PackTmId = tpc.PACKTMID,
                                       SlabValue = tpc.SLABVALUE,
                                       // Normalize calculation mode so that 1 and 0 are treated as Packing, 2 as Grade Weight
                                       CalculationMode = (tpc.CALCULATIONMODE == 2 ? 2 : 1)
                                   }).ToList();

                    // Exclude slab rows that belong to BKN/OTHERS packing types; those are handled as virtual products
                    allCalcs = allCalcs
                        .Where(x => !brokenOrOthersPacktmIds.Contains(x.PackTmId))
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"Loaded {allCalcs.Count} slab calculation records into memory");

                    // Group by product and calculation mode, summing all slab values
                    var productCalcs = allCalcs
                        .GroupBy(x => new { x.ProductId, x.ProductName, x.CalculationMode })
                        .Select(g => new {
                            ProductId = g.Key.ProductId,
                            ProductName = g.Key.ProductName,
                            CalculationMode = g.Key.CalculationMode,
                            TotalPCK = g.Sum(tpc => tpc.SlabValue)
                        })
                        .Where(x => x.TotalPCK > 0)
                        .OrderBy(x => x.ProductName)
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"Product totals found (from slabs): {productCalcs.Count}");

                    foreach (var product in productCalcs)
                    {
                        // To make Cases consistent with the detail view, we must:
                        // 1) Calculate NO OF CASES per packing master and slab column
                        // 2) Sum those case counts for the product

                        var productRows = allCalcs
                            .Where(x => x.ProductId == product.ProductId && x.CalculationMode == product.CalculationMode)
                            .ToList();

                        decimal totalCases = 0;
                        decimal productTotalSlabs = 0;

                        // Group exactly like the detail breakdown: by packing + weight + grade + colour + received type + supplier
                        var productGroups = productRows
                            .GroupBy(x => new { x.PackingId, x.KgWeight, x.GradeId, x.PclrId, x.RcvdtId, x.SupplierName });

                        foreach (var grp in productGroups)
                        {
                            // Dynamic box size for this packing group, based on PCKBOX; fallback to 6 for legacy data
                            int packBoxSize = grp.Max(r => r.BoxSize);
                            int boxSize = packBoxSize > 0 ? packBoxSize : 6;

                            // Map slab values to logical columns using packing type order.
                            // We no longer cap at 17 columns; use all configured slab sizes.
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

                            // NO OF CASES for this packing group = sum of boxes per column (same as detail view)
                            decimal groupCases = 0;
                            for (int i = 0; i < columnCount; i++)
                            {
                                groupCases += CalculateBoxes(colTotals[i], boxSize);
                            }

                            productTotalSlabs += groupTotalSlabs;
                            totalCases += groupCases;
                        }

                        var displayName = product.ProductName;
                        if (product.CalculationMode == 2)
                        {
                            displayName = product.ProductName + " (Grade Weight)";
                        }

                        productTotals.Add(new object[] {
                            product.ProductId,
                            displayName,
                            productTotalSlabs.ToString("N2"),
                            totalCases.ToString("N0"),
                            product.CalculationMode
                        });

                    }

                    var allBknRows = (from tpc in db.TransactionProductCalculations
                                      join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                      join tm in db.TransactionMasters on td.TRANMID equals tm.TRANMID
                                      where (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                            && (tm.DISPSTATUS == 0 || tm.DISPSTATUS == null)
                                            && tm.REGSTRID == 1
                                            && tpc.PRODDATE <= asOnDate
                                            && tpc.BKN > 0
                                      select tpc).ToList();

                    var bknTotal = allBknRows
                        .GroupBy(t => new { t.TRANDID, t.PACKMID })
                        .Select(g => g
                            .OrderBy(c => c.PACKTMID) // header (PACKTMID = 0) first when present
                            .ThenBy(c => c.TRANPID)
                            .First().BKN)
                        .DefaultIfEmpty(0)
                        .Sum();
                    
                    if (bknTotal > 0)
                    {
                        // For BKN (Broken) we do not track cases; always display 0 cases
                        decimal bknCases = 0;

                        // Add BKN as virtual product with ID = -1
                        productTotals.Add(new object[] { 
                            -1, 
                            "BKN (Broken)", 
                            bknTotal.ToString("N2"),
                            bknCases.ToString("N0"),
                            0
                        });
                        System.Diagnostics.Debug.WriteLine($"Added: BKN (Broken) with total: {bknTotal:N2}, Cases: {bknCases:N0}");
                    }
                    
                    // STEP 5: Add OTHERS as a separate virtual product
                    // Same reasoning as BKN: OTHERS is present on each slab row for a
                    // TRANDID+PACKMID, so we must aggregate once per calculation.
                    var allOthersRows = (from tpc in db.TransactionProductCalculations
                                         join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                         join tm in db.TransactionMasters on td.TRANMID equals tm.TRANMID
                                         where (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                               && (tm.DISPSTATUS == 0 || tm.DISPSTATUS == null)
                                               && tm.REGSTRID == 1
                                               && tpc.PRODDATE <= asOnDate
                                               && tpc.OTHERS > 0
                                         select tpc).ToList();

                    var othersTotal = allOthersRows
                        .GroupBy(t => new { t.TRANDID, t.PACKMID })
                        .Select(g => g
                            .OrderBy(c => c.PACKTMID) // header (PACKTMID = 0) first when present
                            .ThenBy(c => c.TRANPID)
                            .First().OTHERS)
                        .DefaultIfEmpty(0)
                        .Sum();
                    
                    if (othersTotal > 0)
                    {
                        var othersCases = CalculateBoxes(othersTotal);

                        // Add OTHERS as virtual product with ID = -2
                        productTotals.Add(new object[] { 
                            -2, 
                            "Others(Peeled)", 
                            othersTotal.ToString("N2"),
                            othersCases.ToString("N0"),
                            0 
                        });
                        System.Diagnostics.Debug.WriteLine($"Added: Others(Peeled) with total: {othersTotal:N2}, Cases: {othersCases:N0}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in direct calculation query: {ex.Message}");
                    // Return empty list on query error - will be logged for debugging
                    return new List<object[]>();
                }

                // If we found direct calculations, return them
                if (productTotals.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"SUCCESS: Returning {productTotals.Count} transaction totals");
                    return productTotals;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("INFO: No stock data found for the selected date");
                    // Return empty list - the view will show "No data available"
                    return new List<object[]>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetSlabTotalsFromDatabase: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<object[]> 
                { 
                    new object[] { 1, $"Error: {ex.Message}", "0.00" } 
                };
            }
        }

        // Removed CalculateItemTotal method - now calculated in main query for better performance

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // Data model for Stock View
    public class StockViewData
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal ItemTotal { get; set; }
        public string Unit { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    // Data model for Item Detail Breakdown
    public class ItemDetailBreakdown
    {
        public List<ItemDetailCategory> Details { get; set; }
        public decimal TotalWeight { get; set; }
        public object PackingDetails { get; set; }
    }

    public class ItemDetailCategory
    {
        public string Category { get; set; }
        public string Weight { get; set; }
    }

    // New data models for date range breakdown
    public class PackingMasterBreakdown
    {
        public List<PackingMasterData> PackingMasters { get; set; }
    }

    public class PackingMasterData
    {
        public string PackingType { get; set; }
        public List<string> ColumnHeaders { get; set; }
        public List<PackingDetailRow> Rows { get; set; }
    }

    public class PackingDetailRow
    {
        // RowType values: "Up to DD/MM/YYYY", "DD/MM/YYYY", "TOTAL", "NO OF CASES"
        public string RowType { get; set; }

        // Dynamic slab values aligned with ColumnHeaders.
        // Each entry in Values corresponds to one slab size column.
        public List<decimal> Values { get; set; }

        // Row total (sum of Values for weight rows, sum of boxes for NO OF CASES rows)
        public decimal? Total { get; set; }
    }
}
