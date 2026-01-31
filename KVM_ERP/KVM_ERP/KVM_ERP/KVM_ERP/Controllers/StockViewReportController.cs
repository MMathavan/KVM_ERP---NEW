using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using KVM_ERP.Models;
using ClosedXML.Excel;
using System.IO;

namespace KVM_ERP.Controllers
{
    [SessionExpire]
    public class StockViewReportController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: StockViewReport
        [Authorize(Roles = "StockViewReportIndex")]
        public ActionResult Index()
        {
            ViewBag.Title = "Stock View Report";
            return View();
        }

        [HttpPost]
        public JsonResult GetStockData(string fromDate, string toDate, string tab = "HL")
        {
            try
            {
                DateTime from = DateTime.Parse(fromDate);
                DateTime to = DateTime.Parse(toDate);

                System.Diagnostics.Debug.WriteLine($"GetStockData called - From: {from:yyyy-MM-dd}, To: {to:yyyy-MM-dd}, Tab: {tab}");

                // Get stock data from TRANSACTION_PRODUCT_CALCULATION table
                var stockData = GetStockViewReportData(from, to, tab);

                return Json(new { success = true, data = stockData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetStockData: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ExportToExcel(string fromDate, string toDate)
        {
            try
            {
                DateTime from = DateTime.Parse(fromDate);
                DateTime to = DateTime.Parse(toDate);

                // Get all stock data
                var allStockData = GetStockViewReportData(from, to, "ALL");

                using (var workbook = new XLWorkbook())
                {
                    // Create tabs: Overall, Head On, Head Less, etc.
                    var receivedTypes = allStockData
                        .Select(x => string.IsNullOrWhiteSpace(x.ReceivedType) ? "Unknown" : x.ReceivedType)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();
                    
                    // Tab 1: Overall (all data grouped by packing type)
                    CreateExcelSheetGrouped(workbook, "Overall", allStockData, to);
                    
                    // Tab 2+: Individual ReceivedTypes
                    foreach (var receivedType in receivedTypes)
                    {
                        var filteredData = allStockData.Where(x => 
                            (string.IsNullOrWhiteSpace(x.ReceivedType) ? "Unknown" : x.ReceivedType) == receivedType
                        ).ToList();
                        
                        if (filteredData.Any())
                        {
                            // Ensure sheet name is valid (max 31 chars, no special chars)
                            string sheetName = receivedType;
                            if (string.IsNullOrWhiteSpace(sheetName))
                            {
                                sheetName = "Unknown";
                            }
                            // Remove invalid characters for Excel sheet names
                            sheetName = sheetName.Replace("/", "-").Replace("\\", "-").Replace("?", "").Replace("*", "").Replace("[", "").Replace("]", "");
                            // Limit to 31 characters (Excel limit)
                            if (sheetName.Length > 31)
                            {
                                sheetName = sheetName.Substring(0, 31);
                            }
                            
                            CreateExcelSheet(workbook, sheetName, filteredData, to);
                        }
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"StockViewReport_{to:yyyyMMdd}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting to Excel: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        private void CreateExcelSheetGrouped(XLWorkbook workbook, string sheetName, List<StockViewReportData> stockData, DateTime toDate)
        {
            if (stockData == null || !stockData.Any())
                return;

            var worksheet = workbook.Worksheets.Add(sheetName);
            
            // Group data by packing type
            var groupedByPackingType = stockData
                .GroupBy(x => new { x.PackingMasterId, x.ReceivedType })
                .OrderBy(g => g.Key.ReceivedType)
                .ToList();

            int row = 1;
            
            // Add main header
            worksheet.Cell(row, 1).Value = $"STOCK AS ON {toDate:dd/MM/yyyy}";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 1).Style.Font.FontSize = 14;
            worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(row, 1, row, 20).Merge();
            row += 2;

            // Process each packing type group
            foreach (var group in groupedByPackingType)
            {
                var groupData = group.ToList();
                var columnHeaders = groupData.First().ColumnHeaders ?? new List<string>();
                int totalColumns = columnHeaders.Count + 2;

                // Add packing type header
                worksheet.Cell(row, 1).Value = $"=== {group.Key.ReceivedType} ===";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 1).Style.Font.FontSize = 12;
                worksheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(100, 149, 237); // Cornflower Blue
                worksheet.Cell(row, 1).Style.Font.FontColor = XLColor.White;
                worksheet.Range(row, 1, row, totalColumns).Merge();
                worksheet.Range(row, 1, row, totalColumns).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                row++;

                // Add column headers
                worksheet.Cell(row, 1).Value = "PARTICULARS";
                worksheet.Cell(row, totalColumns).Value = "TOTAL NO. OF SLABS";
                worksheet.Range(row, 1, row + 1, 1).Merge();
                worksheet.Range(row, totalColumns, row + 1, totalColumns).Merge();
                
                row++;
                for (int i = 0; i < columnHeaders.Count; i++)
                {
                    var cell = worksheet.Cell(row, i + 2);
                    cell.Style.NumberFormat.Format = "@"; // Set format to text
                    cell.Value = columnHeaders[i];
                }
                
                // Style headers
                worksheet.Range(row - 1, 1, row, totalColumns).Style.Font.Bold = true;
                worksheet.Range(row - 1, 1, row, totalColumns).Style.Fill.BackgroundColor = XLColor.LightGray;
                worksheet.Range(row - 1, 1, row, totalColumns).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(row - 1, 1, row, totalColumns).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(row - 1, 1, row, totalColumns).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                row++;

                // Add data rows for this group
                int itemNumber = 1;

                // Track overall totals across all products in this packing type group
                var groupOverallTotalData = new decimal[columnHeaders.Count];
                decimal groupOverallTotalSlabs = 0;

                // Group by Variety within the Packing Type group
                var groupedByVariety = groupData
                    .GroupBy(x => x.Variety ?? "Unknown")
                    .OrderBy(v => v.Key)
                    .ToList();

                foreach (var varietyGroup in groupedByVariety)
                {
                    var varietyTotalData = new decimal[columnHeaders.Count];
                    decimal varietyTotalSlabs = 0;
                    string currentVariety = varietyGroup.Key;

                    foreach (var item in varietyGroup)
                    {
                        var openingData = item.OpeningData ?? new List<decimal>();
                        var productionData = item.ProductionData ?? new List<decimal>();
                        var totalData = item.TotalData ?? new List<decimal>();
                        
                        // Product Name Row
                        worksheet.Cell(row, 1).Value = $"{itemNumber}. {item.ProductName}";
                        worksheet.Range(row, 1, row, totalColumns).Merge();
                        worksheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(row, 1).Style.Font.Bold = true;
                        row++;

                        // OPENING STOCK Row
                        worksheet.Cell(row, 1).Value = "OPENING STOCK";
                        for (int i = 0; i < openingData.Count; i++)
                        {
                            worksheet.Cell(row, i + 2).Value = openingData[i];
                        }
                        worksheet.Cell(row, totalColumns).Value = item.OpeningTotalSlabs;
                        row++;

                        // PRODUCTION Row
                        worksheet.Cell(row, 1).Value = "PRODUCTION";
                        for (int i = 0; i < productionData.Count; i++)
                        {
                            worksheet.Cell(row, i + 2).Value = productionData[i];
                        }
                        worksheet.Cell(row, totalColumns).Value = item.ProductionTotalSlabs;
                        row++;

                        // TOTAL Row for this product
                        worksheet.Cell(row, 1).Value = "TOTAL";
                        for (int i = 0; i < totalData.Count; i++)
                        {
                            worksheet.Cell(row, i + 2).Value = totalData[i];
                        }
                        worksheet.Cell(row, totalColumns).Value = item.TotalSlabs;
                        worksheet.Range(row, 1, row, totalColumns).Style.Font.Bold = true;
                        row++;

                        // Accumulate variety and overall totals
                        for (int i = 0; i < totalData.Count && i < groupOverallTotalData.Length; i++)
                        {
                            varietyTotalData[i] += totalData[i];
                            groupOverallTotalData[i] += totalData[i];
                        }
                        varietyTotalSlabs += item.TotalSlabs;
                        groupOverallTotalSlabs += item.TotalSlabs;

                        row++;
                        itemNumber++;
                    }

                    // Add VARIETY TOTAL row
                    worksheet.Cell(row, 1).Value = $"{currentVariety.ToUpper()} TOTAL";
                    for (int i = 0; i < varietyTotalData.Length; i++)
                    {
                        worksheet.Cell(row, i + 2).Value = varietyTotalData[i];
                    }
                    worksheet.Cell(row, totalColumns).Value = varietyTotalSlabs;
                    worksheet.Range(row, 1, row, totalColumns).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 235, 156); // Light Yellow/Amber
                    worksheet.Range(row, 1, row, totalColumns).Style.Font.Bold = true;
                    worksheet.Range(row, 1, row, totalColumns).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    row += 2;
                }

                // Add OVERALL TOTAL row for this packing type group (sum of all products in the group)
                worksheet.Cell(row, 1).Value = "OVERALL TOTAL";
                for (int i = 0; i < groupOverallTotalData.Length; i++)
                {
                    worksheet.Cell(row, i + 2).Value = groupOverallTotalData[i];
                }
                worksheet.Cell(row, totalColumns).Value = groupOverallTotalSlabs;
                worksheet.Range(row, 1, row, totalColumns).Style.Fill.BackgroundColor = XLColor.FromArgb(212, 237, 218);
                worksheet.Range(row, 1, row, totalColumns).Style.Font.Bold = true;
                row++;

                // Add spacing between groups
                row += 2;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();
        }

        private void CreateExcelSheet(XLWorkbook workbook, string sheetName, List<StockViewReportData> stockData, DateTime toDate)
        {
            if (stockData == null || !stockData.Any())
                return;

            var worksheet = workbook.Worksheets.Add(sheetName);
            
            // Get column headers - search for the first item that actually has headers to avoid empty headers in some tabs
            var columnHeadersItem = stockData.FirstOrDefault(x => x.ColumnHeaders != null && x.ColumnHeaders.Any()) ?? stockData.First();
            var columnHeaders = columnHeadersItem.ColumnHeaders ?? new List<string>();
            int totalColumns = columnHeaders.Count + 2; // +2 for Particulars and Total

            // Add main header - "STOCK AS ON dd/MM/yyyy"
            int row = 1;
            worksheet.Cell(row, 1).Value = $"STOCK AS ON {toDate:dd/MM/yyyy}";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(row, 1, row, totalColumns).Merge();
                    
            // Add column headers - Row 1
            row++;
            worksheet.Cell(row, 1).Value = "PARTICULARS";
            worksheet.Cell(row, totalColumns).Value = "TOTAL NO. OF SLABS";
            
            // Merge PARTICULARS cell from row 2 to row 3
            worksheet.Range(row, 1, row + 1, 1).Merge();
            // Merge TOTAL NO. OF SLABS cell from row 2 to row 3
            worksheet.Range(row, totalColumns, row + 1, totalColumns).Merge();
            
            // Add dynamic column headers - Row 2 (Size columns from PackingTypeMaster)
            row++;
            for (int i = 0; i < columnHeaders.Count; i++)
            {
                var cell = worksheet.Cell(row, i + 2);
                cell.Style.NumberFormat.Format = "@"; // Set format to text
                cell.Value = columnHeaders[i];
            }
            
            // Style headers
            worksheet.Range(2, 1, 3, totalColumns).Style.Font.Bold = true;
            worksheet.Range(2, 1, 3, totalColumns).Style.Fill.BackgroundColor = XLColor.LightGray;
            worksheet.Range(2, 1, 3, totalColumns).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range(2, 1, 3, totalColumns).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range(2, 1, 3, totalColumns).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Add data rows
            row++; // Move to row 4 for data
            int itemNumber = 1;

            // Track overall totals across all products on this sheet
            var sheetOverallTotalData = new decimal[columnHeaders.Count];
            decimal sheetOverallTotalSlabs = 0;

            // Group by Variety within the sheet
            var groupedByVariety = stockData
                .GroupBy(x => x.Variety ?? "Unknown")
                .OrderBy(v => v.Key)
                .ToList();

            foreach (var varietyGroup in groupedByVariety)
            {
                var varietyTotalData = new decimal[columnHeaders.Count];
                decimal varietyTotalSlabs = 0;
                string currentVariety = varietyGroup.Key;

                foreach (var item in varietyGroup)
                {
                    var openingData = item.OpeningData ?? new List<decimal>();
                    var productionData = item.ProductionData ?? new List<decimal>();
                    var totalData = item.TotalData ?? new List<decimal>();
                    
                    // Product Name Row
                    worksheet.Cell(row, 1).Value = $"{itemNumber}. {item.ProductName}";
                    worksheet.Range(row, 1, row, totalColumns).Merge();
                    worksheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    row++;

                    // OPENING STOCK Row
                    worksheet.Cell(row, 1).Value = "OPENING STOCK";
                    for (int i = 0; i < openingData.Count; i++)
                    {
                        worksheet.Cell(row, i + 2).Value = openingData[i];
                    }
                    worksheet.Cell(row, totalColumns).Value = item.OpeningTotalSlabs;
                    row++;

                    // PRODUCTION Row
                    worksheet.Cell(row, 1).Value = "PRODUCTION";
                    for (int i = 0; i < productionData.Count; i++)
                    {
                        worksheet.Cell(row, i + 2).Value = productionData[i];
                    }
                    worksheet.Cell(row, totalColumns).Value = item.ProductionTotalSlabs;
                    row++;

                    // TOTAL Row for this product
                    worksheet.Cell(row, 1).Value = "TOTAL";
                    for (int i = 0; i < totalData.Count; i++)
                    {
                        worksheet.Cell(row, i + 2).Value = totalData[i];
                    }
                    worksheet.Cell(row, totalColumns).Value = item.TotalSlabs;
                    worksheet.Range(row, 1, row, totalColumns).Style.Font.Bold = true;
                    row++;

                    // Accumulate variety and overall totals
                    for (int i = 0; i < totalData.Count && i < sheetOverallTotalData.Length; i++)
                    {
                        varietyTotalData[i] += totalData[i];
                        sheetOverallTotalData[i] += totalData[i];
                    }
                    varietyTotalSlabs += item.TotalSlabs;
                    sheetOverallTotalSlabs += item.TotalSlabs;

                    row++;
                    itemNumber++;
                }

                // Add VARIETY TOTAL row
                worksheet.Cell(row, 1).Value = $"{currentVariety.ToUpper()} TOTAL";
                for (int i = 0; i < varietyTotalData.Length; i++)
                {
                    worksheet.Cell(row, i + 2).Value = varietyTotalData[i];
                }
                worksheet.Cell(row, totalColumns).Value = varietyTotalSlabs;
                worksheet.Range(row, 1, row, totalColumns).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 235, 156); // Light Yellow/Amber
                worksheet.Range(row, 1, row, totalColumns).Style.Font.Bold = true;
                worksheet.Range(row, 1, row, totalColumns).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                row += 2;
            }

            // Add OVERALL TOTAL row (sum of all products on this sheet)
            worksheet.Cell(row, 1).Value = "OVERALL TOTAL";
            for (int i = 0; i < sheetOverallTotalData.Length; i++)
            {
                worksheet.Cell(row, i + 2).Value = sheetOverallTotalData[i];
            }
            worksheet.Cell(row, totalColumns).Value = sheetOverallTotalSlabs;
            worksheet.Range(row, 1, row, totalColumns).Style.Fill.BackgroundColor = XLColor.FromArgb(212, 237, 218); // Light Green
            worksheet.Range(row, 1, row, totalColumns).Style.Font.Bold = true;
            row++;

            // Add borders to all data cells
            worksheet.Range(2, 1, row - 1, totalColumns).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range(2, 1, row - 1, totalColumns).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            
            // Center align all data cells
            worksheet.Range(4, 2, row - 1, totalColumns).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();
        }

        private List<StockViewReportData> GetStockViewReportData(DateTime fromDate, DateTime toDate, string tab)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"GetStockViewReportData - From: {fromDate:yyyy-MM-dd}, To: {toDate:yyyy-MM-dd}, Tab: {tab}");
                System.Diagnostics.Debug.WriteLine($"Opening Stock: Data BEFORE {fromDate:yyyy-MM-dd}");
                System.Diagnostics.Debug.WriteLine($"Production: Data FROM {fromDate:yyyy-MM-dd} TO {toDate:yyyy-MM-dd}");

                var stockData = new List<StockViewReportData>();

                // Call stored procedure to get stock data with proper date filtering
                // Opening Stock: Data BEFORE fromDate (DateCategory = 0)
                // Production: Data FROM fromDate TO toDate (DateCategory = 1)
                var allData = db.Database.SqlQuery<StockDataDTO>(
                    "EXEC pr_GetStockViewReportData @FromDate, @ToDate",
                    new System.Data.SqlClient.SqlParameter("@FromDate", fromDate),
                    new System.Data.SqlClient.SqlParameter("@ToDate", toDate)
                ).ToList();

                System.Diagnostics.Debug.WriteLine($"Stored procedure returned {allData.Count} records");

                // Group by packing master, product, calculation mode and supplier so that
                // each supplier/mode combination gets its own row
                var groupedByPackingAndProduct = allData
                    .GroupBy(x => new
                    {
                        x.PackingMasterId,
                        x.PackingMasterName,
                        x.ProductId,
                        x.ProductName,
                        x.KGWGT,
                        x.GradeName,
                        x.ColorName,
                        x.ReceivedTypeName,
                        x.SupplierName,
                        x.CALCULATIONMODE
                    })
                    .ToList();

                // Pre-compute size metadata (PackIndex + SizeName) per packing master so that
                // all products under the same packing type share the same column headers
                var headersByPacking = allData
                    .GroupBy(x => x.PackingMasterId)
                    .ToDictionary(
                        g => g.Key,
                        g => g
                            .GroupBy(x => new { x.PackIndex, x.SizeName })
                            .OrderBy(h => h.Key.PackIndex)
                            .Select(h => new { h.Key.PackIndex, h.Key.SizeName })
                            .ToList()
                    );

                foreach (var productGroup in groupedByPackingAndProduct)
                {
                    // Determine size metadata for this packing master
                    if (!headersByPacking.TryGetValue(productGroup.Key.PackingMasterId, out var sizeMeta) || sizeMeta.Count == 0)
                    {
                        continue;
                    }

                    var columnHeaders = new List<string>();
                    var openingDataArray = new List<decimal>();
                    var productionDataArray = new List<decimal>();
                    var totalDataArray = new List<decimal>();

                    foreach (var sizeInfo in sizeMeta)
                    {
                        var desc = sizeInfo.SizeName ?? string.Empty;
                        var upper = desc.ToUpper().Trim();

                        // Exclude BKN/BROKEN and OTHERS from slab headers – they are handled separately
                        if (upper == "BKN" || upper == "BROKEN" || upper.Contains("BKN"))
                            continue;

                        if (upper == "OTHERS" || upper == "OTHER" || upper.Contains("OTHERS"))
                            continue;

                        columnHeaders.Add(desc);

                        long packIndex = sizeInfo.PackIndex;
                        var sizeRows = productGroup.Where(x => x.PackIndex == packIndex);

                        // DateCategory 0 = Opening Stock (before fromDate)
                        // DateCategory 1 = Production (fromDate to toDate)
                        decimal openingSum = sizeRows
                            .Where(x => x.DateCategory == 0)
                            .Sum(x => x.SlabValue);

                        decimal productionSum = sizeRows
                            .Where(x => x.DateCategory == 1)
                            .Sum(x => x.SlabValue);

                        openingDataArray.Add(openingSum);
                        productionDataArray.Add(productionSum);
                        totalDataArray.Add(openingSum + productionSum);
                    }

                    // Calculate totals across all slab columns
                    decimal openingTotal = openingDataArray.Sum();
                    decimal productionTotal = productionDataArray.Sum();
                    decimal total = totalDataArray.Sum();

                    // If there are no slabs at all for this product/packing combination,
                    // skip it. These zero-slab groups typically come from BKN/OTHERS-only
                    // calculations which are already handled separately as virtual products
                    // at the bottom of this method.
                    if (openingTotal == 0 && productionTotal == 0 && total == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Skipping product group {productGroup.Key.ProductName} (PACKMID {productGroup.Key.PackingMasterId}) because all slab totals are zero.");
                        continue;
                    }

                    // Determine if this group is Grade Weight mode
                    bool isGradeWeightMode = productGroup.Key.CALCULATIONMODE == 2;
                    string baseReceivedType = string.IsNullOrWhiteSpace(productGroup.Key.PackingMasterName)
                        ? "Unknown"
                        : productGroup.Key.PackingMasterName;
                    string receivedTypeLabel = isGradeWeightMode
                        ? baseReceivedType + " (Grade Weight)"
                        : baseReceivedType;

                    // Determine pack size (PCKBOX) for display; fall back to 6 if not available
                    int packBoxSize = productGroup.Max(x => x.PCKBOX);
                    int displayBoxSize = packBoxSize > 0 ? packBoxSize : 6;

                    // Create StockViewReportData item with dynamic pack size
                    var item = new StockViewReportData
                    {
                        ProductName = $"{productGroup.Key.ProductName} {displayBoxSize} x {productGroup.Key.KGWGT}" +
                                        (!string.IsNullOrEmpty(productGroup.Key.GradeName) ? $" - {productGroup.Key.GradeName}" : "") +
                                        (!string.IsNullOrEmpty(productGroup.Key.ColorName) ? $" - {productGroup.Key.ColorName}" : "") +
                                        (!string.IsNullOrEmpty(productGroup.Key.ReceivedTypeName) ? $" - {productGroup.Key.ReceivedTypeName}" : "") +
                                        (!string.IsNullOrEmpty(productGroup.Key.SupplierName) ? $" - {productGroup.Key.SupplierName}" : ""),
                        Variety = productGroup.Key.ProductName,
                        ReceivedType = receivedTypeLabel,
                        PackingMasterId = productGroup.Key.PackingMasterId,
                        KGWGT = productGroup.Key.KGWGT,
                        // Dynamic columns
                        ColumnHeaders = columnHeaders,
                        OpeningData = openingDataArray,
                        OpeningTotalSlabs = openingTotal,
                        ProductionData = productionDataArray,
                        ProductionTotalSlabs = productionTotal,
                        TotalData = totalDataArray,
                        TotalSlabs = total
                    };

                    stockData.Add(item);
                }

                // Add separate summary sections for BKN and Others (like Stock View virtual products)
                try
                {
                    // Common query for BKN and OTHERS with date and status filters
                    // NOTE: Use PRODDATE from TransactionProductCalculations as the effective production date.
                    // In the new headless slab design, BKN and OTHERS are copied to each slab row for a
                    // TRANDID+PACKMID combination. To avoid multiplying these values by the number of slab
                    // rows, we collapse to one representative row per TRANDID+PACKMID when aggregating.
                    var baseCalcQuery = from tpc in db.TransactionProductCalculations
                                        join td in db.TransactionDetails on tpc.TRANDID equals td.TRANDID
                                        join m in db.MaterialMasters on td.MTRLID equals m.MTRLID
                                        join tm in db.TransactionMasters on td.TRANMID equals tm.TRANMID
                                        where (tpc.DISPSTATUS == 0 || tpc.DISPSTATUS == null)
                                              && (m.DISPSTATUS == 0 || m.DISPSTATUS == null)
                                              && (tm.DISPSTATUS == 0 || tm.DISPSTATUS == null)
                                              && tpc.PRODDATE <= toDate
                                        select new
                                        {
                                            TRANDATE = tpc.PRODDATE,
                                            tpc.TRANDID,
                                            tpc.PACKMID,
                                            tpc.PACKTMID,
                                            tpc.TRANPID,
                                            tpc.BKN,
                                            tpc.OTHERS
                                        };

                    // BKN aggregation with de-duplication per TRANDID+PACKMID
                    var bknRaw = baseCalcQuery.Where(x => x.BKN > 0).ToList();
                    if (bknRaw.Any())
                    {
                        var bknData = bknRaw
                            .GroupBy(x => new { x.TRANDID, x.PACKMID })
                            .Select(g =>
                            {
                                var first = g
                                    .OrderBy(c => c.PACKTMID) // header (PACKTMID = 0) first when present
                                    .ThenBy(c => c.TRANPID)
                                    .First();
                                return new
                                {
                                    TRANDATE = first.TRANDATE,
                                    BKN = first.BKN
                                };
                            })
                            .ToList();

                        decimal openingBkn = bknData.Where(x => x.TRANDATE < fromDate).Sum(x => x.BKN);
                        decimal productionBkn = bknData.Where(x => x.TRANDATE >= fromDate && x.TRANDATE <= toDate).Sum(x => x.BKN);
                        decimal totalBkn = openingBkn + productionBkn;

                        if (totalBkn > 0)
                        {
                            var bknItem = new StockViewReportData
                            {
                                ProductName = "BKN (Broken)",
                                ReceivedType = "BKN (Broken)",
                                PackingMasterId = -1,
                                ColumnHeaders = new List<string> { "BKN (KG)" },
                                OpeningData = new List<decimal> { openingBkn },
                                OpeningTotalSlabs = openingBkn,
                                ProductionData = new List<decimal> { productionBkn },
                                ProductionTotalSlabs = productionBkn,
                                TotalData = new List<decimal> { totalBkn },
                                TotalSlabs = totalBkn
                            };

                            stockData.Add(bknItem);
                        }
                    }

                    // OTHERS aggregation with de-duplication per TRANDID+PACKMID
                    var othersRaw = baseCalcQuery.Where(x => x.OTHERS > 0).ToList();
                    if (othersRaw.Any())
                    {
                        var othersData = othersRaw
                            .GroupBy(x => new { x.TRANDID, x.PACKMID })
                            .Select(g =>
                            {
                                var first = g
                                    .OrderBy(c => c.PACKTMID) // header (PACKTMID = 0) first when present
                                    .ThenBy(c => c.TRANPID)
                                    .First();
                                return new
                                {
                                    TRANDATE = first.TRANDATE,
                                    OTHERS = first.OTHERS
                                };
                            })
                            .ToList();

                        decimal openingOthers = othersData.Where(x => x.TRANDATE < fromDate).Sum(x => x.OTHERS);
                        decimal productionOthers = othersData.Where(x => x.TRANDATE >= fromDate && x.TRANDATE <= toDate).Sum(x => x.OTHERS);
                        decimal totalOthers = openingOthers + productionOthers;

                        if (totalOthers > 0)
                        {
                            var othersItem = new StockViewReportData
                            {
                                ProductName = "Others(Peeled)",
                                ReceivedType = "Others(Peeled)",
                                PackingMasterId = -2,
                                ColumnHeaders = new List<string> { "Others(Peeled) (KG)" },
                                OpeningData = new List<decimal> { openingOthers },
                                OpeningTotalSlabs = openingOthers,
                                ProductionData = new List<decimal> { productionOthers },
                                ProductionTotalSlabs = productionOthers,
                                TotalData = new List<decimal> { totalOthers },
                                TotalSlabs = totalOthers
                            };

                            stockData.Add(othersItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error adding BKN/Others summary rows: {ex.Message}");
                }

                System.Diagnostics.Debug.WriteLine($"Combined into {stockData.Count} products (including BKN/Others summaries if any)");
                stockData = stockData.OrderBy(x => x.ReceivedType).ThenBy(x => x.ProductName).ToList();

                return stockData;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetStockViewReportData: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<StockViewReportData>();
            }
        }

        // Model for Stock View Report Data with Dynamic Columns
        public class StockViewReportData
        {
            public string ProductName { get; set; }
            public string Variety { get; set; }
            public string ReceivedType { get; set; }
            public int PackingMasterId { get; set; }
            // Grade weight (KGWGT) - used only for grouping / display on the client
            public decimal KGWGT { get; set; }
            // Dynamic column headers for this packing type
            public List<string> ColumnHeaders { get; set; }
            // Opening Stock (before fromDate) - Dynamic data
            public List<decimal> OpeningData { get; set; }
            public decimal OpeningTotalSlabs { get; set; }
            // Production (fromDate to toDate) - Dynamic data
            public List<decimal> ProductionData { get; set; }
            public decimal ProductionTotalSlabs { get; set; }
            // Total (Opening + Production) - Dynamic data
            public List<decimal> TotalData { get; set; }
            public decimal TotalSlabs { get; set; }
        }

        // DTO for Stored Procedure pr_GetStockViewReportData
        public class StockDataDTO
        {
            public DateTime TRANDATE { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int PackingMasterId { get; set; }
            public string PackingMasterName { get; set; }
            public decimal KGWGT { get; set; }
            public int PCKBOX { get; set; }
            public string GradeName { get; set; }
            public string ColorName { get; set; }
            public string ReceivedTypeName { get; set; }
            public string SupplierName { get; set; }
            // Calculation mode from TRANSACTION_PRODUCT_CALCULATION (1 = Packing, 2 = Grade Weight)
            public int CALCULATIONMODE { get; set; }
            // Dynamic size metadata from PackingTypes CTE
            public long PackIndex { get; set; }
            public string SizeName { get; set; }
            // Date category: 0 = Opening (before fromDate), 1 = Production (fromDate to toDate)
            public int DateCategory { get; set; }
            // Slab count for this specific size row
            public decimal SlabValue { get; set; }
        }
    }
}
