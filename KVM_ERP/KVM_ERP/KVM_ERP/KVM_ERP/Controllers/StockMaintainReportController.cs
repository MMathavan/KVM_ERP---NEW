using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using KVM_ERP.Models;
using ClosedXML.Excel;

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
            try
            {
                DateTime from = DateTime.Parse(fromDate);
                DateTime to = DateTime.Parse(toDate);

                var rows = ExecuteStockMaintainProc(from, to);
                return Json(new { success = true, data = rows }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
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

                var rows = ExecuteStockMaintainProc(from, to);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Stock Maintain");

                    var allColumns = rows
                        .SelectMany(r => r.Keys)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    if (!allColumns.Any())
                    {
                        worksheet.Cell(1, 1).Value = "No data";
                    }
                    else
                    {
                        worksheet.Cell(1, 1).Value = $"STOCK MAINTAIN REPORT {from:dd/MM/yyyy} - {to:dd/MM/yyyy}";
                        worksheet.Cell(1, 1).Style.Font.Bold = true;
                        worksheet.Range(1, 1, 1, allColumns.Count).Merge();
                        worksheet.Range(1, 1, 1, allColumns.Count).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        for (int c = 0; c < allColumns.Count; c++)
                        {
                            worksheet.Cell(2, c + 1).Value = allColumns[c];
                            worksheet.Cell(2, c + 1).Style.Font.Bold = true;
                            worksheet.Cell(2, c + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                        }

                        int rowIndex = 3;
                        foreach (var row in rows)
                        {
                            for (int c = 0; c < allColumns.Count; c++)
                            {
                                var key = allColumns[c];
                                row.TryGetValue(key, out var val);
                                worksheet.Cell(rowIndex, c + 1).Value = val ?? string.Empty;
                            }
                            rowIndex++;
                        }

                        worksheet.Columns().AdjustToContents();
                        worksheet.Range(2, 1, rows.Count + 2, allColumns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Range(2, 1, rows.Count + 2, allColumns.Count).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"StockMaintainReport_{to:yyyyMMdd}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting to Excel: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        private List<Dictionary<string, object>> ExecuteStockMaintainProc(DateTime fromDate, DateTime toDate)
        {
            var result = new List<Dictionary<string, object>>();

            var connStr = db.Database.Connection.ConnectionString;
            using (var conn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("PR_STOCK_ITEM_PACKING_WISE", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@FROMDATE", fromDate));
                cmd.Parameters.Add(new SqlParameter("@TODATE", toDate));

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    do
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var colName = reader.GetName(i);
                                object value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                row[colName] = value;
                            }
                            result.Add(row);
                        }
                    }
                    while (reader.NextResult());
                }
            }

            return result;
        }
    }
}
