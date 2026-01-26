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
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "RawMaterialIntakeReportIndex")]
        public ActionResult RawMaterialsImport()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "RawMaterialIntakeReportIndex")]
        public JsonResult RawMaterialsImportData(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                // Filter only Raw Material Intake (REGSTRID = 1)
                var query = db.TransactionMasters.Where(t => t.REGSTRID == 1);
                if (fromDate.HasValue)
                {
                    query = query.Where(t => t.TRANDATE >= fromDate.Value);
                }
                if (toDate.HasValue)
                {
                    query = query.Where(t => t.TRANDATE <= toDate.Value);
                }

                // fetch details to compute total boxes per transaction
                var details = db.TransactionDetails
                    .GroupBy(d => d.TRANMID)
                    .Select(g => new { TRANMID = g.Key, Boxes = g.Sum(x => (int?)x.MTRLNBOX) ?? 0 })
                    .ToList();

                var data = query
                    .OrderBy(t => t.TRANDATE)
                    .ToList()
                    .Select(t => new {
                        Date = t.TRANDATE.ToString("yyyy-MM-dd"),
                        SupplierCode = t.CATECODE,
                        SupplierName = t.CATENAME,
                        VehicleNo = t.VECHNO,
                        ClientWeight = t.CLIENTWGHT,
                        NoOfBoxes = details.FirstOrDefault(x => x.TRANMID == t.TRANMID)?.Boxes ?? 0,
                        Sno = t.TRANMID
                    })
                    .ToList();

                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Authorize(Roles = "RawMaterialIntakeReportIndex")]
        public ActionResult RawMaterialsImportExcel(DateTime? fromDate, DateTime? toDate)
        {
            // Filter only Raw Material Intake (REGSTRID = 1)
            var query = db.TransactionMasters.Where(t => t.REGSTRID == 1);
            if (fromDate.HasValue) query = query.Where(t => t.TRANDATE >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(t => t.TRANDATE <= toDate.Value);

            var detailMap = db.TransactionDetails
                .GroupBy(d => d.TRANMID)
                .Select(g => new { TRANMID = g.Key, Boxes = g.Sum(x => (int?)x.MTRLNBOX) ?? 0 })
                .ToList();

            var rows = query
                .OrderBy(t => t.TRANDATE)
                .ToList()
                .Select(t => new {
                    Date = t.TRANDATE,
                    SupplierCode = t.CATECODE,
                    SupplierName = t.CATENAME,
                    VehicleNo = t.VECHNO,
                    ClientWeight = t.CLIENTWGHT,
                    NoOfBoxes = detailMap.FirstOrDefault(x => x.TRANMID == t.TRANMID)?.Boxes ?? 0
                })
                .ToList();

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("RawMaterialsImport");
                // Report Title
                ws.Cell(1, 1).Value = "MARINEX";
                ws.Cell(1, 1).Style.Font.Bold = true;
                ws.Cell(1, 1).Style.Font.FontSize = 16;

                // Subtitle with date range
                var fromTxt = fromDate.HasValue ? fromDate.Value.ToString("dd/MM/yyyy") : "";
                var toTxt = toDate.HasValue ? toDate.Value.ToString("dd/MM/yyyy") : "";
                ws.Cell(2, 1).Value = $"Datewise Raw Materials Import Details (From: {fromTxt} To: {toTxt})";
                ws.Cell(2, 1).Style.Font.Bold = true;
                ws.Cell(2, 1).Style.Font.FontSize = 12;

                // Table Header
                ws.Cell(4, 1).Value = "Sno";
                ws.Cell(4, 2).Value = "Date";
                ws.Cell(4, 3).Value = "Supplier Code";
                ws.Cell(4, 4).Value = "Supplier Name";
                ws.Cell(4, 5).Value = "Vehicle No";
                ws.Cell(4, 6).Value = "Client Weight";
                ws.Cell(4, 7).Value = "No of Boxes";
                ws.Range(4,1,4,7).Style.Font.Bold = true;

                int r = 5;
                int sno = 1;
                foreach (var row in rows)
                {
                    ws.Cell(r, 1).Value = sno++;
                    ws.Cell(r, 2).Value = row.Date;
                    ws.Cell(r, 2).Style.DateFormat.Format = "dd/MM/yyyy";
                    ws.Cell(r, 3).Value = row.SupplierCode;
                    ws.Cell(r, 4).Value = row.SupplierName;
                    ws.Cell(r, 5).Value = row.VehicleNo;
                    ws.Cell(r, 6).Value = row.ClientWeight;
                    ws.Cell(r, 7).Value = row.NoOfBoxes;
                    r++;
                }

                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    stream.Position = 0;
                    var fileName = $"RawMaterialsImport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
    }
}


