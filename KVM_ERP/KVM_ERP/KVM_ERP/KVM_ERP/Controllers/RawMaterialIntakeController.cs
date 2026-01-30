using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using KVM_ERP.Models;
using Newtonsoft.Json;

namespace KVM_ERP.Controllers
{
    [SessionExpire]
    public class RawMaterialIntakeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RawMaterialIntake
        [Authorize(Roles = "RawMaterialIntakeIndex")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: RawMaterialIntake/Form
        [Authorize(Roles = "RawMaterialIntakeCreate,RawMaterialIntakeEdit")]
        public ActionResult Form(int? id)
        {
            // Check specific role based on operation
            if (id.HasValue && id.Value > 0)
            {
                // Edit mode - require Edit role
                if (!User.IsInRole("RawMaterialIntakeEdit"))
                {
                    TempData["ErrorMessage"] = "You do not have permission to edit Raw Material Intake.";
                    return RedirectToAction("Index");
                }
                
                // VALIDATION: Check if invoice has been generated for this intake - prevent editing
                var checkInvoice = db.Database.SqlQuery<int>(
                    @"SELECT COUNT(1) 
                      FROM TRANSACTIONDETAIL invtd
                      INNER JOIN TRANSACTIONMASTER invtm ON invtd.TRANMID = invtm.TRANMID
                      WHERE invtm.REGSTRID = 2
                      AND invtd.TRANDAID IN (
                          SELECT TRANPID 
                          FROM TRANSACTION_PRODUCT_CALCULATION tpc
                          INNER JOIN TRANSACTIONDETAIL td ON tpc.TRANDID = td.TRANDID
                          WHERE td.TRANMID = @p0
                      )", 
                    id.Value).FirstOrDefault();
                
                if (checkInvoice > 0)
                {
                    TempData["ErrorMessage"] = "Cannot edit: Invoice has been generated for this Raw Material Intake. Please delete the invoice first.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Create mode - require Create role
                if (!User.IsInRole("RawMaterialIntakeCreate"))
                {
                    TempData["ErrorMessage"] = "You do not have permission to create Raw Material Intake.";
                    return RedirectToAction("Index");
                }
            }
            
            var model = new TransactionMaster();

            // Populate supplier dropdown (enabled suppliers only)
            var suppliers = db.SupplierMasters
                .Where(s => s.DISPSTATUS == 0)
                .OrderBy(s => s.CATENAME)
                .Select(s => new { s.CATEID, s.CATENAME, s.CATECODE, s.DISPSTATUS })
                .ToList();

            // Populate packing masters list for modal (include MTRLGID mapping)
            var packingMasters = db.PackingMasters
                .Where(p => p.DISPSTATUS == 0)
                .OrderBy(p => p.PACKMDESC)
                .ToList();
            ViewBag.PackingMasters = packingMasters;

            // Populate Grade dropdown (enabled only)
            var grades = db.GradeMasters
                .Where(g => g.DISPSTATUS == 0)
                .OrderBy(g => g.GRADEDESC)
                .Select(g => new SelectListItem { Text = g.GRADEDESC, Value = g.GRADEID.ToString() })
                .ToList();
            grades.Insert(0, new SelectListItem { Text = "-- Select Grade --", Value = "" });
            ViewBag.Grades = grades;

            // Populate Production Colour dropdown (enabled only)
            var colours = db.ProductionColourMasters
                .Where(c => c.DISPSTATUS == 0)
                .OrderBy(c => c.PCLRDESC)
                .Select(c => new SelectListItem { Text = c.PCLRDESC, Value = c.PCLRID.ToString() })
                .ToList();
            colours.Insert(0, new SelectListItem { Text = "-- Select Colour --", Value = "" });
            ViewBag.ProductionColours = colours;

            // Populate Received Type dropdown (enabled only)
            var receivedTypes = db.ReceivedTypeMasters
                .Where(r => r.DISPSTATUS == 0)
                .OrderBy(r => r.RCVDTDESC)
                .Select(r => new SelectListItem { Text = r.RCVDTDESC, Value = r.RCVDTID.ToString() })
                .ToList();
            receivedTypes.Insert(0, new SelectListItem { Text = "-- Select Type --", Value = "" });
            ViewBag.ReceivedTypes = receivedTypes;

            // We'll build this list after we know if we're editing to set Selected
            List<SelectListItem> supplierListItems;

            // Status list (selected set below based on model)
            Func<short, SelectList> buildStatus = (short selected) => new SelectList(new[]
            {
                new SelectListItem { Text = "Enabled", Value = "0" },
                new SelectListItem { Text = "Disabled", Value = "1" }
            }, "Value", "Text", selected.ToString());

            // Material Groups for details table
            var mgroups = db.MaterialGroupMasters
                .Where(g => g.DISPSTATUS == 0)
                .OrderBy(g => g.MTRLGDESC)
                .Select(g => new SelectListItem { Text = g.MTRLGDESC, Value = g.MTRLGID.ToString() })
                .ToList();
            ViewBag.MaterialGroups = mgroups;

            if (id != null && id > 0)
            {
                // Edit - Only load Raw Material Intake records (REGSTRID=1)
                model = db.TransactionMasters.FirstOrDefault(t => t.TRANMID == id && t.REGSTRID == 1) ?? new TransactionMaster();
                // Preselect supplier by code (preferred) or name
                var sel = suppliers.FirstOrDefault(x => x.CATECODE == model.CATECODE)
                          ?? suppliers.FirstOrDefault(x => x.CATENAME == model.CATENAME);
                string selectedSupplierId = sel != null ? sel.CATEID.ToString() : "";

                // If not found among enabled suppliers, include the disabled one so edit still shows selection
                if (string.IsNullOrEmpty(selectedSupplierId))
                {
                    var disabledSupplier = db.SupplierMasters
                        .Where(x => x.CATECODE == model.CATECODE || x.CATENAME == model.CATENAME)
                        .OrderBy(x => x.CATENAME)
                        .Select(x => new { x.CATEID, x.CATENAME, x.CATECODE, x.DISPSTATUS })
                        .FirstOrDefault();
                    if (disabledSupplier != null)
                    {
                        // append to list with (Disabled) tag
                        suppliers.Add(new { disabledSupplier.CATEID, CATENAME = disabledSupplier.CATENAME + " (Disabled)", disabledSupplier.CATECODE, disabledSupplier.DISPSTATUS });
                        selectedSupplierId = disabledSupplier.CATEID.ToString();
                    }
                }

                supplierListItems = suppliers
                    .Select(s => new SelectListItem
                    {
                        Text = s.CATENAME,
                        Value = s.CATEID.ToString(),
                        Selected = selectedSupplierId == s.CATEID.ToString()
                    })
                    .ToList();
                ViewBag.SupplierList = supplierListItems;
                ViewBag.DISPSTATUS = buildStatus(model.DISPSTATUS);

                // Load existing details for edit
                var details = db.Database.SqlQuery<DetailRow>(
                    "SELECT TRANDID, TRANMID, MTRLGID, MTRLID, MTRLNBOX, MTRLCOUNTS FROM TRANSACTIONDETAIL WHERE TRANMID = @p0 ORDER BY TRANDID",
                    model.TRANMID).ToList();
                ViewBag.DetailsJson = JsonConvert.SerializeObject(details);

                // Load existing quality check for edit
                var qualityCheck = db.TransactionQualityChecks.FirstOrDefault(q => q.TRANMID == model.TRANMID);
                ViewBag.QualityCheckJson = qualityCheck != null ? JsonConvert.SerializeObject(qualityCheck) : "null";
            }
            else
            {
                // Add: pre-fill defaults
                try
                {
                    var nextNo = db.Database.SqlQuery<int>("SELECT ISNULL(MAX(TRANMID),0)+1 FROM TRANSACTIONMASTER").FirstOrDefault();
                    model.TRANMID = nextNo;
                }
                catch { model.TRANMID = 1; }
                model.TRANDATE = DateTime.Today;
                model.DISPSTATUS = 0; // Enabled by default

                supplierListItems = suppliers
                    .Select(s => new SelectListItem
                    {
                        Text = s.CATENAME,
                        Value = s.CATEID.ToString()
                    }).ToList();
                ViewBag.SupplierList = supplierListItems;
                ViewBag.DISPSTATUS = buildStatus(model.DISPSTATUS);
            }

            // For client-side auto-fill of code from supplier, pass minimal map
            ViewBag.SupplierCodeMap = suppliers.ToDictionary(s => s.CATEID.ToString(), s => s.CATECODE ?? "");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "RawMaterialIntakeCreate,RawMaterialIntakeEdit")]
        public ActionResult savedata(TransactionMaster tab, int? SupplierId, string detailRowsJson, string qualityCheckJson)
        {
            try
            {
                // Debug logging
                System.Diagnostics.Debug.WriteLine("=== RAW MATERIAL INTAKE SAVE ===");
                System.Diagnostics.Debug.WriteLine($"TRANMID: {tab.TRANMID}");
                System.Diagnostics.Debug.WriteLine($"SupplierId: {SupplierId}");
                System.Diagnostics.Debug.WriteLine($"detailRowsJson: {detailRowsJson}");
                System.Diagnostics.Debug.WriteLine($"qualityCheckJson: {qualityCheckJson}");
                
                // Check if this is edit or create mode
                bool isEdit = tab.TRANMID > 0 && db.TransactionMasters.Any(x => x.TRANMID == tab.TRANMID);
                System.Diagnostics.Debug.WriteLine($"isEdit: {isEdit}");
                
                // Check specific role based on operation
                if (isEdit)
                {
                    if (!User.IsInRole("RawMaterialIntakeEdit"))
                    {
                        TempData["ErrorMessage"] = "You do not have permission to edit Raw Material Intake.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    if (!User.IsInRole("RawMaterialIntakeCreate"))
                    {
                        TempData["ErrorMessage"] = "You do not have permission to create Raw Material Intake.";
                        return RedirectToAction("Index");
                    }
                }
                
                var details = new List<DetailRow>();
                if (!string.IsNullOrWhiteSpace(detailRowsJson))
                {
                    details = JsonConvert.DeserializeObject<List<DetailRow>>(detailRowsJson) ?? new List<DetailRow>();
                }
                System.Diagnostics.Debug.WriteLine($"Parsed {details.Count} detail rows");

                // Parse quality check data
                TransactionQualityCheck qualityCheck = null;
                if (!string.IsNullOrWhiteSpace(qualityCheckJson))
                {
                    qualityCheck = JsonConvert.DeserializeObject<TransactionQualityCheck>(qualityCheckJson);
                }
                System.Diagnostics.Debug.WriteLine($"Quality check parsed: {(qualityCheck != null ? "YES" : "NO")}");

                if (SupplierId.HasValue && SupplierId.Value > 0)
                {
                    var sup = db.SupplierMasters.FirstOrDefault(x => x.CATEID == SupplierId.Value);
                    if (sup != null)
                    {
                        tab.CATENAME = sup.CATENAME;
                        tab.CATECODE = sup.CATECODE;
                    }
                }

                if (isEdit)
                {
                    // Update
                    var existing = db.TransactionMasters.FirstOrDefault(x => x.TRANMID == tab.TRANMID);
                    if (existing != null)
                    {
                        existing.TRANDATE = tab.TRANDATE;
                        existing.CATENAME = tab.CATENAME;
                        existing.CATECODE = tab.CATECODE;
                        existing.VECHNO = tab.VECHNO;
                        existing.CLIENTWGHT = tab.CLIENTWGHT;
                        existing.DISPSTATUS = tab.DISPSTATUS;
                        existing.LMUSRID = User?.Identity?.Name ?? existing.LMUSRID;
                        existing.PRCSDATE = DateTime.Now; // treat as last modified for simplicity
                        
                        // Update TRANREFID if supplier changed
                        if (SupplierId.HasValue && SupplierId.Value > 0)
                        {
                            existing.TRANREFID = SupplierId.Value;
                        }
                        
                        db.SaveChanges();

                        // Load existing details for this master
                        var existingDetails = db.Database.SqlQuery<DetailRow>(
                            "SELECT TRANDID, TRANMID, MTRLGID, MTRLID, MTRLNBOX, MTRLCOUNTS FROM TRANSACTIONDETAIL WHERE TRANMID = @p0",
                            existing.TRANMID).ToList();

                        var postedIds = new HashSet<int>(details.Where(x => x.TRANDID > 0).Select(x => x.TRANDID));
                        var toDelete = existingDetails.Where(x => !postedIds.Contains(x.TRANDID)).Select(x => x.TRANDID).ToList();

                        // VALIDATION: Check if invoice has been generated before allowing deletion of detail rows
                        if (toDelete.Any())
                        {
                            // Invoice links to intake through TRANPID (from TRANSACTION_PRODUCT_CALCULATION) 
                            // which is stored as TRANDAID in invoice's TRANSACTIONDETAIL
                            // Check if any of the rows being deleted are referenced in an invoice
                            var checkInvoice = db.Database.SqlQuery<int>(
                                @"SELECT COUNT(1) 
                                  FROM TRANSACTIONDETAIL invtd
                                  INNER JOIN TRANSACTIONMASTER invtm ON invtd.TRANMID = invtm.TRANMID
                                  WHERE invtm.REGSTRID = 2
                                  AND invtd.TRANDAID IN (
                                      SELECT TRANPID 
                                      FROM TRANSACTION_PRODUCT_CALCULATION
                                      WHERE TRANDID IN (" + string.Join(",", toDelete) + @")
                                  )").FirstOrDefault();
                            
                            if (checkInvoice > 0)
                            {
                                return Json(new { success = false, message = "Cannot delete product rows: Invoice has been generated for these items. Please delete the invoice first." });
                            }
                        }

                        // Delete removed rows
                        foreach (var delId in toDelete)
                        {
                            // STEP 1: Delete from TRANSACTION_PRODUCT_CALCULATION first (child table)
                            db.Database.ExecuteSqlCommand("DELETE FROM TRANSACTION_PRODUCT_CALCULATION WHERE TRANDID = @p0", delId);
                            
                            // STEP 2: Delete from TRANSACTIONDETAIL (parent table)
                            db.Database.ExecuteSqlCommand("DELETE FROM TRANSACTIONDETAIL WHERE TRANDID = @p0", delId);
                        }

                        // Upsert rows
                        foreach (var d in details)
                        {
                            if (d.MTRLGID > 0 && d.MTRLID > 0 && d.MTRLNBOX >= 0 && d.MTRLCOUNTS >= 0)
                            {
                                if (d.TRANDID > 0 && existingDetails.Any(ed => ed.TRANDID == d.TRANDID))
                                {
                                    // UPDATE: preserve CUSRID, only change values and LMUSRID/PRCSDATE
                                    db.Database.ExecuteSqlCommand(@"
                                        UPDATE TRANSACTIONDETAIL
                                        SET MTRLGID = @p1, MTRLID = @p2, MTRLNBOX = @p3, MTRLCOUNTS = @p4,
                                            LMUSRID = @p5, PRCSDATE = @p6
                                        WHERE TRANDID = @p0",
                                        d.TRANDID, d.MTRLGID, d.MTRLID, d.MTRLNBOX, d.MTRLCOUNTS,
                                        User?.Identity?.Name ?? "System", DateTime.Now);
                                }
                                else
                                {
                                    // INSERT during edit: preserve creator by using master CUSRID; set LMUSRID to current user
                                    db.Database.ExecuteSqlCommand(@"
                                        INSERT INTO TRANSACTIONDETAIL (TRANMID, MTRLGID, MTRLID, MTRLNBOX, MTRLCOUNTS, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE)
                                        VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, 0, @p7)",
                                        existing.TRANMID, d.MTRLGID, d.MTRLID, d.MTRLNBOX, d.MTRLCOUNTS,
                                        existing.CUSRID, User?.Identity?.Name ?? "System", DateTime.Now);
                                }
                            }
                        }

                        // Handle Quality Check (same pattern as details)
                        if (qualityCheck != null && qualityCheck.LABOID > 0)
                        {
                            var existingQualityCheck = db.Database.SqlQuery<int>(
                                "SELECT COUNT(*) FROM TRANSACTION_QUALITY_CHECK WHERE TRANMID = @p0", existing.TRANMID).FirstOrDefault();

                            if (existingQualityCheck > 0)
                            {
                                // UPDATE: preserve CUSRID, only change values and LMUSRID/PRCSDATE
                                db.Database.ExecuteSqlCommand(@"
                                    UPDATE TRANSACTION_QUALITY_CHECK
                                    SET LABOID = @p1, STATUS = @p2, REMARKS = @p3, DONEBY = @p4, VERIFIEDBY = @p5,
                                        LOTDATE = @p6, LMUSRID = @p7, PRCSDATE = @p8
                                    WHERE TRANMID = @p0",
                                    existing.TRANMID, qualityCheck.LABOID, qualityCheck.STATUS, qualityCheck.REMARKS?.Trim(),
                                    qualityCheck.DONEBY?.Trim(), qualityCheck.VERIFIEDBY?.Trim(), qualityCheck.LOTDATE,
                                    User?.Identity?.Name ?? "System", DateTime.Now);
                            }
                            else
                            {
                                // INSERT during edit: preserve creator by using master CUSRID; set LMUSRID to current user
                                db.Database.ExecuteSqlCommand(@"
                                    INSERT INTO TRANSACTION_QUALITY_CHECK (TRANMID, LABOID, STATUS, REMARKS, DONEBY, VERIFIEDBY, LOTDATE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE)
                                    VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, 0, @p9)",
                                    existing.TRANMID, qualityCheck.LABOID, qualityCheck.STATUS, qualityCheck.REMARKS?.Trim(),
                                    qualityCheck.DONEBY?.Trim(), qualityCheck.VERIFIEDBY?.Trim(), qualityCheck.LOTDATE,
                                    existing.CUSRID, User?.Identity?.Name ?? "System", DateTime.Now);
                            }
                        }

                        return RedirectToAction("Index");
                    }
                }

                // Insert
                tab.CUSRID = User?.Identity?.Name ?? "System";
                tab.LMUSRID = tab.CUSRID;
                tab.PRCSDATE = DateTime.Now;
                
                // Get COMPYID from session
                int compyId = Session["compyid"] != null ? Convert.ToInt32(Session["compyid"]) : 1;
                
                // Get next TRANNO (auto-increment)
                var maxTranNo = db.Database.SqlQuery<int?>("SELECT MAX(TRANNO) FROM TRANSACTIONMASTER WHERE COMPYID = @p0", compyId).FirstOrDefault();
                int nextTranNo = (maxTranNo ?? 0) + 1;
                
                // Generate TRANDNO as 000 + TRANNO
                string tranDNo = "000" + nextTranNo.ToString();
                
                // Get supplier ID for TRANREFID
                int tranRefId = SupplierId.HasValue && SupplierId.Value > 0 ? SupplierId.Value : 0;
                
                // Set new field values
                tab.COMPYID = compyId;
                tab.REGSTRID = 1; // Default value
                tab.TRANNO = nextTranNo;
                tab.TRANDNO = tranDNo;
                tab.TRANREFID = tranRefId;
                tab.TRANNAMT = 0.00m; // Default 0.00
                tab.TRANAMTWRDS = "Nil"; // Default "Nil"
                tab.TRANREFNO = "-"; // Default "-"

                // Insert master and get new TRANMID
                var newId = db.Database.SqlQuery<int>(@"
                    INSERT INTO TRANSACTIONMASTER (TRANDATE, CATENAME, CATECODE, VECHNO, CLIENTWGHT, DISPSTATUS, CUSRID, LMUSRID, PRCSDATE,
                                                   COMPYID, REGSTRID, TRANNO, TRANDNO, TRANREFID, TRANNAMT, TRANAMTWRDS, TRANREFNO)
                    VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16); 
                    SELECT CAST(SCOPE_IDENTITY() AS INT);
                ", tab.TRANDATE, tab.CATENAME ?? "", tab.CATECODE ?? "", tab.VECHNO ?? "",
                   tab.CLIENTWGHT, tab.DISPSTATUS, tab.CUSRID, tab.LMUSRID, tab.PRCSDATE,
                   tab.COMPYID, tab.REGSTRID, tab.TRANNO, tab.TRANDNO, tab.TRANREFID, tab.TRANNAMT, tab.TRANAMTWRDS, tab.TRANREFNO).FirstOrDefault();

                // Insert details
                foreach (var d in details)
                {
                    if (d.MTRLGID > 0 && d.MTRLID > 0 && d.MTRLNBOX >= 0 && d.MTRLCOUNTS >= 0)
                    {
                        db.Database.ExecuteSqlCommand(@"
                            INSERT INTO TRANSACTIONDETAIL (TRANMID, MTRLGID, MTRLID, MTRLNBOX, MTRLCOUNTS, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE)
                            VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, 0, @p7)",
                            newId, d.MTRLGID, d.MTRLID, d.MTRLNBOX, d.MTRLCOUNTS,
                            User?.Identity?.Name ?? "System", User?.Identity?.Name ?? "System", DateTime.Now);
                    }
                }

                // Insert quality check (same pattern as details)
                if (qualityCheck != null && qualityCheck.LABOID > 0)
                {
                    db.Database.ExecuteSqlCommand(@"
                        INSERT INTO TRANSACTION_QUALITY_CHECK (TRANMID, LABOID, STATUS, REMARKS, DONEBY, VERIFIEDBY, LOTDATE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE)
                        VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, 0, @p9)",
                        newId, qualityCheck.LABOID, qualityCheck.STATUS, qualityCheck.REMARKS?.Trim(),
                        qualityCheck.DONEBY?.Trim(), qualityCheck.VERIFIEDBY?.Trim(), qualityCheck.LOTDATE,
                        User?.Identity?.Name ?? "System", User?.Identity?.Name ?? "System", DateTime.Now);
                }

                System.Diagnostics.Debug.WriteLine("=== SAVE COMPLETED ===");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the full error
                System.Diagnostics.Debug.WriteLine($"Error saving Raw Material Intake: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                TempData["ErrorMessage"] = $"Error saving data: {ex.Message}";
                
                // Repopulate dropdowns on error and return to form
                var suppliers = db.SupplierMasters
                    .Where(s => s.DISPSTATUS == 0)
                    .OrderBy(s => s.CATENAME)
                    .Select(s => new { s.CATEID, s.CATENAME, s.CATECODE })
                    .ToList();
                ViewBag.SupplierList = suppliers
                    .Select(s => new SelectListItem { Text = s.CATENAME, Value = s.CATEID.ToString() })
                    .ToList();
                ViewBag.DISPSTATUS = new SelectList(new[]
                {
                    new SelectListItem { Text = "Enabled", Value = "0" },
                    new SelectListItem { Text = "Disabled", Value = "1" }
                }, "Value", "Text", tab.DISPSTATUS.ToString());
                ViewBag.SupplierCodeMap = suppliers.ToDictionary(s => s.CATEID.ToString(), s => s.CATECODE ?? "");

                return View("Form", tab);
            }
        }

        private void CalculateProductValuesFromSlabs(TransactionProductCalculation model, List<SlabDetailDto> slabDetails)
        {
            if (slabDetails == null || slabDetails.Count == 0)
            {
                CalculateProductValues(model);
                return;
            }

            model.TOPCK = slabDetails.Sum(s => s.SLABVALUE);

            bool isGradeWeightMode = model.CALCULATIONMODE == 2;
            System.Diagnostics.Debug.WriteLine($"Calculating from slabs in {(isGradeWeightMode ? "Grade Weight" : "Packing")} mode");

            if (isGradeWeightMode)
            {
                model.PCKLVALUE = 0;
                model.AVGPCKVALUE = 0;
                model.TOTALPNDS = 0;
                model.TOTALYELDCOUNTS = 0;

                model.PCKKGWGT = 0;
                model.WASTEPWGT = model.WASTEWGT;

                if (model.KGWGT > 0 && model.TOPCK > 0)
                {
                    model.PCKKGWGT = model.KGWGT * model.TOPCK;
                    model.WASTEPWGT = model.PCKKGWGT + model.WASTEWGT;
                }

                if (model.KGWGT > 0 && model.YELDPERCENT > 0 && model.TOPCK > 0)
                {
                    if (model.PCKKGWGT == 0)
                    {
                        model.PCKKGWGT = model.KGWGT * model.TOPCK;
                        model.WASTEPWGT = model.PCKKGWGT + model.WASTEWGT;
                    }

                    model.FACTORYWGT = model.WASTEPWGT / (model.YELDPERCENT / 100);
                }
                else
                {
                    model.FACTORYWGT = model.TOPCK + model.WASTEWGT;
                }

                System.Diagnostics.Debug.WriteLine($"Grade Weight calculation (dynamic slabs): Slab={model.TOPCK}, Peeled={model.WASTEWGT}, KGWGT={model.KGWGT}, YELD%={model.YELDPERCENT}, PCKKGWGT={model.PCKKGWGT}, WASTEPWGT={model.WASTEPWGT}, Factory Weight={model.FACTORYWGT}");
            }
            else if (model.TOPCK > 0)
            {
                model.PCKLVALUE = CalculatePCKLValueFromSlabs(model, slabDetails);

                model.AVGPCKVALUE = model.PCKLVALUE / model.TOPCK;

                model.TOTALPNDS = model.AVGPCKVALUE * model.PNDSVALUE;

                if (model.YELDPERCENT > 0)
                {
                    model.TOTALYELDCOUNTS = model.TOTALPNDS * (model.YELDPERCENT / 100);
                }

                model.PCKKGWGT = model.KGWGT * model.TOPCK;

                model.WASTEPWGT = model.PCKKGWGT + model.WASTEWGT;

                if (model.YELDPERCENT > 0)
                {
                    model.FACTORYWGT = model.WASTEPWGT / (model.YELDPERCENT / 100);
                }
            }
        }

        private void SaveProductSlabDetails(TransactionProductCalculation model, List<SlabDetailDto> slabDetails)
        {
            if (slabDetails == null || slabDetails.Count == 0)
            {
                return;
            }

            // Delete existing slab details for this TRANDID + PACKMID
            db.Database.ExecuteSqlCommand(
                @"DELETE FROM TRANSACTION_PRODUCT_CALCULATION WHERE TRANDID = @p0 AND PACKMID = @p1 AND PACKTMID <> 0",
                model.TRANDID, model.PACKMID);

            // Group by PACKTMID so that we store one row per packing type with aggregated slab value
            var groupedSlabs = slabDetails
                .GroupBy(s => s.PACKTMID)
                .Select(g => new { PACKTMID = g.Key, SLABVALUE = g.Sum(x => x.SLABVALUE) })
                .ToList();

            // Generate a unique TRANPID sequence for new slab rows
            var nextTranPid = GetNextTransactionProductId();

            var currentUserId = Session["CUSRID"]?.ToString();
            if (string.IsNullOrEmpty(currentUserId))
            {
                currentUserId = User?.Identity?.Name ?? "SYSTEM";
            }

            foreach (var slab in groupedSlabs)
            {
                if (slab.SLABVALUE <= 0)
                {
                    continue;
                }

                var detail = new TransactionProductCalculation
                {
                    TRANPID = nextTranPid++,
                    TRANMID = model.TRANMID,
                    TRANDID = model.TRANDID,
                    PACKMID = model.PACKMID,
                    PACKTMID = slab.PACKTMID,
                    SLABVALUE = slab.SLABVALUE,
                    // Copy all calculated and common fields from header so each slab row
                    // has the same data as the main calculation row
                    TOPCK = model.TOPCK,
                    PCKLVALUE = model.PCKLVALUE,
                    AVGPCKVALUE = model.AVGPCKVALUE,
                    PNDSVALUE = model.PNDSVALUE,
                    TOTALPNDS = model.TOTALPNDS,
                    YELDPERCENT = model.YELDPERCENT,
                    TOTALYELDCOUNTS = model.TOTALYELDCOUNTS,
                    KGWGT = model.KGWGT,
                    PCKKGWGT = model.PCKKGWGT,
                    PCKBOX = model.PCKBOX,
                    WASTEWGT = model.WASTEWGT,
                    WASTEPWGT = model.WASTEPWGT,
                    FACTORYWGT = model.FACTORYWGT,
                    FACAVGWGT = model.FACAVGWGT,
                    FACAVGCOUNT = model.FACAVGCOUNT,
                    BKN = model.BKN,
                    OTHERS = model.OTHERS,
                    GRADEID = model.GRADEID,
                    PCLRID = model.PCLRID,
                    RCVDTID = model.RCVDTID,
                    ATRANDID = model.ATRANDID,
                    CALCULATIONMODE = model.CALCULATIONMODE,
                    PRODDATE = model.PRODDATE,
                    CUSRID = currentUserId,
                    LMUSRID = currentUserId,
                    DISPSTATUS = 0,
                    PRCSDATE = DateTime.Now
                };

                db.TransactionProductCalculations.Add(detail);
            }
        }

        private void SaveProductSlabDetails(TransactionProductCalculation model)
        {
            // Delete existing slab details for this TRANDID + PACKMID
            db.Database.ExecuteSqlCommand(
                @"DELETE FROM TRANSACTION_PRODUCT_CALCULATION WHERE TRANDID = @p0 AND PACKMID = @p1 AND PACKTMID <> 0",
                model.TRANDID, model.PACKMID);

            // Load packing types for this packing master in original database order
            var packingTypes = db.PackingTypeMasters
                .Where(pt => pt.PACKMID == model.PACKMID && (pt.DISPSTATUS == 0 || pt.DISPSTATUS == null))
                .OrderBy(pt => pt.PACKTMCODE)
                .ToList();

            if (!packingTypes.Any())
            {
                return;
            }

            // Generate a unique TRANPID sequence for legacy PCK-based slab rows
            var nextTranPid = GetNextTransactionProductId();

            // Legacy PCK-based values
            var pckValues = new[]
            {
                model.PCK1, model.PCK2, model.PCK3, model.PCK4, model.PCK5,
                model.PCK6, model.PCK7, model.PCK8, model.PCK9, model.PCK10,
                model.PCK11, model.PCK12, model.PCK13, model.PCK14, model.PCK15,
                model.PCK16, model.PCK17
            };

            int pckIndex = 0;
            var currentUserId = Session["CUSRID"]?.ToString();
            if (string.IsNullOrEmpty(currentUserId))
            {
                currentUserId = User?.Identity?.Name ?? "SYSTEM";
            }

            int? bknPacktmId = null;
            int? othersPacktmId = null;

            foreach (var packingType in packingTypes)
            {
                var desc = packingType.PACKTMDESC ?? string.Empty;
                bool isBKN = IsBknLabel(desc);
                bool isOTHERS = IsOthersLabel(desc);

                if (isBKN)
                {
                    // Capture BKN PACKTMID; actual insert is delegated to stored procedure
                    bknPacktmId = packingType.PACKTMID;
                }
                else if (isOTHERS)
                {
                    // Capture OTHERS PACKTMID; actual insert is delegated to stored procedure
                    othersPacktmId = packingType.PACKTMID;
                }
                else
                {
                    // Regular slab columns use sequential PCK1..PCK17 mapping
                    if (pckIndex < pckValues.Length)
                    {
                        var value = pckValues[pckIndex];
                        if (value > 0)
                        {
                            var detail = new TransactionProductCalculation
                            {
                                TRANPID = nextTranPid++,
                                TRANMID = model.TRANMID,
                                TRANDID = model.TRANDID,
                                PACKMID = model.PACKMID,
                                PACKTMID = packingType.PACKTMID,
                                SLABVALUE = value,
                                // Full calculation fields copied from header
                                TOPCK = model.TOPCK,
                                PCKLVALUE = model.PCKLVALUE,
                                AVGPCKVALUE = model.AVGPCKVALUE,
                                PNDSVALUE = model.PNDSVALUE,
                                TOTALPNDS = model.TOTALPNDS,
                                YELDPERCENT = model.YELDPERCENT,
                                TOTALYELDCOUNTS = model.TOTALYELDCOUNTS,
                                KGWGT = model.KGWGT,
                                PCKKGWGT = model.PCKKGWGT,
                                PCKBOX = model.PCKBOX,
                                WASTEWGT = model.WASTEWGT,
                                WASTEPWGT = model.WASTEPWGT,
                                FACTORYWGT = model.FACTORYWGT,
                                FACAVGWGT = model.FACAVGWGT,
                                FACAVGCOUNT = model.FACAVGCOUNT,
                                GRADEID = model.GRADEID,
                                PCLRID = model.PCLRID,
                                RCVDTID = model.RCVDTID,
                                ATRANDID = model.ATRANDID,
                                CALCULATIONMODE = model.CALCULATIONMODE,
                                PRODDATE = model.PRODDATE,
                                CUSRID = currentUserId,
                                LMUSRID = currentUserId,
                                DISPSTATUS = 0,
                                PRCSDATE = DateTime.Now
                            };

                            db.TransactionProductCalculations.Add(detail);
                        }

                        pckIndex++;
                    }
                }
            }

            // Delegate BKN and OTHERS slab row creation to stored procedure so future
            // business rule changes can be made in SQL only.
            if ((model.BKN > 0 && bknPacktmId.HasValue) || (model.OTHERS > 0 && othersPacktmId.HasValue))
            {
                db.Database.ExecuteSqlCommand(
                    @"EXEC USP_SaveBknOthersSlabs 
                        @p0, @p1, @p2,
                        @p3, @p4,
                        @p5, @p6,
                        @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16, @p17, @p18, @p19, @p20, @p21, @p22, @p23, @p24, @p25, @p26, @p27, @p28",
                    model.TRANMID,
                    model.TRANDID,
                    model.PACKMID,
                    bknPacktmId.HasValue ? (object)bknPacktmId.Value : DBNull.Value,
                    othersPacktmId.HasValue ? (object)othersPacktmId.Value : DBNull.Value,
                    model.BKN,
                    model.OTHERS,
                    model.TOPCK,
                    model.PCKLVALUE,
                    model.AVGPCKVALUE,
                    model.PNDSVALUE,
                    model.TOTALPNDS,
                    model.YELDPERCENT,
                    model.TOTALYELDCOUNTS,
                    model.KGWGT,
                    model.PCKKGWGT,
                    model.PCKBOX,
                    model.WASTEWGT,
                    model.WASTEPWGT,
                    model.FACTORYWGT,
                    model.FACAVGWGT,
                    model.FACAVGCOUNT,
                    model.GRADEID,
                    model.PCLRID,
                    model.RCVDTID,
                    model.ATRANDID,
                    model.CALCULATIONMODE,
                    model.PRODDATE.HasValue ? (object)model.PRODDATE.Value : DBNull.Value,
                    currentUserId,
                    currentUserId
                );
            }
        }

        // JSON: Products by Material Group (enabled only)
        public JsonResult GetProducts(int groupId)
        {
            var prods = db.MaterialMasters
                .Where(m => m.MTRLGID == groupId && m.DISPSTATUS == 0)
                .OrderBy(m => m.MTRLDESC)
                .Select(m => new { id = m.MTRLID, text = m.MTRLDESC })
                .ToList();
            return Json(prods, JsonRequestBehavior.AllowGet);
        }

        // AJAX: RawMaterialIntake/GetAjaxData
        public ActionResult GetAjaxData(string fromDate = null, string toDate = null)
        {
            try
            {
                // Debug logging
                System.Diagnostics.Debug.WriteLine($"[GetAjaxData] fromDate: {fromDate}, toDate: {toDate}");
                // Build dynamic WHERE clause for date filtering
                string whereClause = "";
                var parameters = new List<object>();
                int paramIndex = 0;

                if (!string.IsNullOrEmpty(fromDate))
                {
                    DateTime fromDateTime;
                    if (DateTime.TryParse(fromDate, out fromDateTime))
                    {
                        whereClause += $" AND tm.TRANDATE >= @p{paramIndex}";
                        parameters.Add(fromDateTime.Date);
                        paramIndex++;
                    }
                }

                if (!string.IsNullOrEmpty(toDate))
                {
                    DateTime toDateTime;
                    if (DateTime.TryParse(toDate, out toDateTime))
                    {
                        whereClause += $" AND tm.TRANDATE <= @p{paramIndex}";
                        parameters.Add(toDateTime.Date.AddDays(1).AddSeconds(-1)); // Include full day
                        paramIndex++;
                    }
                }

                // Build the SQL query with optional date filtering - ONLY Raw Material Intake (REGSTRID=1)
                string sql = $@"SELECT tm.TRANMID, tm.TRANDATE, tm.TRANDNO, tm.CATENAME, tm.CATECODE, tm.VECHNO, tm.DISPSTATUS,
                             ISNULL(p.PRODUCTS,'') AS PRODUCTS
                      FROM TRANSACTIONMASTER tm
                      LEFT JOIN (
                         SELECT td.TRANMID,
                                STUFF((
                                   SELECT ', ' + m.MTRLDESC
                                   FROM TRANSACTIONDETAIL td2
                                   INNER JOIN MATERIALMASTER m ON m.MTRLID = td2.MTRLID
                                   WHERE td2.TRANMID = td.TRANMID
                                   FOR XML PATH(''), TYPE).value('.','NVARCHAR(MAX)'),1,2,'') AS PRODUCTS
                         FROM TRANSACTIONDETAIL td
                         GROUP BY td.TRANMID
                      ) p ON p.TRANMID = tm.TRANMID
                      WHERE tm.REGSTRID = 1 {whereClause}
                      ORDER BY tm.TRANDATE DESC, tm.TRANMID DESC";

                // Debug logging
                System.Diagnostics.Debug.WriteLine($"[GetAjaxData] SQL: {sql}");
                System.Diagnostics.Debug.WriteLine($"[GetAjaxData] Parameters: {string.Join(", ", parameters)}");

                // Execute query with parameters
                var rows = parameters.Count > 0 
                    ? db.Database.SqlQuery<TransactionRow>(sql, parameters.ToArray()).ToList()
                    : db.Database.SqlQuery<TransactionRow>(sql).ToList();

                System.Diagnostics.Debug.WriteLine($"[GetAjaxData] Returned {rows.Count} rows");

                var data = rows.Select((r, idx) => {
                    // Check if invoice has been generated for this intake
                    var hasInvoice = db.Database.SqlQuery<int>(
                        @"SELECT COUNT(1) 
                          FROM TRANSACTIONDETAIL invtd
                          INNER JOIN TRANSACTIONMASTER invtm ON invtd.TRANMID = invtm.TRANMID
                          WHERE invtm.REGSTRID = 2
                          AND invtd.TRANDAID IN (
                              SELECT TRANPID 
                              FROM TRANSACTION_PRODUCT_CALCULATION tpc
                              INNER JOIN TRANSACTIONDETAIL td ON tpc.TRANDID = td.TRANDID
                              WHERE td.TRANMID = @p0
                          )", 
                        r.TRANMID).FirstOrDefault() > 0;
                    
                    return new
                    {
                        TRANMID = r.TRANMID,
                        TRANDATE = r.TRANDATE.ToString("yyyy-MM-dd"), // ISO for stable ordering; format on client if needed
                        TRANDNO = r.TRANDNO ?? "0000",  // Display number
                        CATENAME = r.CATENAME ?? string.Empty,
                        CATECODE = r.CATECODE ?? string.Empty,
                        VECHNO = r.VECHNO ?? string.Empty,
                        Products = r.PRODUCTS ?? string.Empty,
                        DISPSTATUS = r.DISPSTATUS == 0 ? "Enabled" : "Disabled",
                        StatusBadge = r.DISPSTATUS == 0
                            ? "<span class='badge badge-success'>Enabled</span>"
                            : "<span class='badge badge-danger'>Disabled</span>",
                        HasInvoice = hasInvoice  // Flag to disable edit/delete if invoice generated
                    };
                }).ToList();

                return Json(new { data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Delete method for AJAX calls (match masters pattern)
        [HttpPost]
        public ActionResult Del(int id)
        {
            try
            {
                // Check if user has delete role
                if (!User.IsInRole("RawMaterialIntakeDelete"))
                {
                    return Json("Access Denied: You do not have permission to delete records. Please contact your administrator.");
                }
                
                var exists = db.Database.SqlQuery<int>("SELECT COUNT(1) FROM TRANSACTIONMASTER WHERE TRANMID = @p0 AND REGSTRID = 1", id).FirstOrDefault();
                if (exists == 0)
                {
                    return Json("Record not found");
                }
                
                // VALIDATION: Check if invoice has been generated for this intake
                // Invoice links to intake through TRANPID (from TRANSACTION_PRODUCT_CALCULATION) 
                // which is stored as TRANDAID in invoice's TRANSACTIONDETAIL
                var checkInvoice = db.Database.SqlQuery<int>(
                    @"SELECT COUNT(1) 
                      FROM TRANSACTIONDETAIL invtd
                      INNER JOIN TRANSACTIONMASTER invtm ON invtd.TRANMID = invtm.TRANMID
                      WHERE invtm.REGSTRID = 2
                      AND invtd.TRANDAID IN (
                          SELECT TRANPID 
                          FROM TRANSACTION_PRODUCT_CALCULATION tpc
                          INNER JOIN TRANSACTIONDETAIL td ON tpc.TRANDID = td.TRANDID
                          WHERE td.TRANMID = @p0
                      )", 
                    id).FirstOrDefault();
                
                if (checkInvoice > 0)
                {
                    return Json("Cannot delete: Invoice has been generated for this Raw Material Intake. Please delete the invoice first.");
                }
                
                // Delete child records first to prevent foreign key violations
                // Step 1: Delete product calculations
                db.Database.ExecuteSqlCommand(
                    @"DELETE FROM TRANSACTION_PRODUCT_CALCULATION 
                      WHERE TRANDID IN (SELECT TRANDID FROM TRANSACTIONDETAIL WHERE TRANMID = @p0)", 
                    id);
                
                // Step 2: Delete transaction details
                db.Database.ExecuteSqlCommand("DELETE FROM TRANSACTIONDETAIL WHERE TRANMID = @p0", id);
                
                // Step 3: Delete the master record
                db.Database.ExecuteSqlCommand("DELETE FROM TRANSACTIONMASTER WHERE TRANMID = @p0 AND REGSTRID = 1", id);
                
                return Json("Successfully deleted");
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message);
            }
        }

        private class TransactionRow
        {
            public int TRANMID { get; set; }
            public DateTime TRANDATE { get; set; }
            public string TRANDNO { get; set; }
            public string CATENAME { get; set; }
            public string CATECODE { get; set; }
            public string VECHNO { get; set; }
            public short DISPSTATUS { get; set; }
            public string PRODUCTS { get; set; }
        }

        private class DetailRow
        {
            public int TRANDID { get; set; }
            public int TRANMID { get; set; }
            public int MTRLGID { get; set; }
            public int MTRLID { get; set; }
            public int MTRLNBOX { get; set; }
            public int MTRLCOUNTS { get; set; }
        }

        private class SlabDetailDto
        {
            public int PACKTMID { get; set; }
            public decimal SLABVALUE { get; set; }
        }

        // Get packing type fields based on packing master
        public JsonResult GetPackingTypeFields(int packingId)
        {
            try
            {
                var packingMaster = db.PackingMasters.FirstOrDefault(p => p.PACKMID == packingId);
                if (packingMaster == null)
                {
                    return Json(new { success = false, message = "Packing master not found" }, JsonRequestBehavior.AllowGet);
                }

                // Get actual packing type masters mapped to this packing master
                var packingTypes = db.PackingTypeMasters
                    .Where(pt => pt.PACKMID == packingId && pt.DISPSTATUS == 0)
                    .OrderBy(pt => pt.PACKTMCODE)
                    .Select(pt => new { 
                        label = pt.PACKTMDESC, 
                        code = pt.PACKTMCODE,
                        id = pt.PACKTMID
                    })
                    .ToList();

                if (!packingTypes.Any())
                {
                    return Json(new { success = false, message = "No packing types found for this packing master" }, JsonRequestBehavior.AllowGet);
                }

                var fields = packingTypes.Select(pt => new { 
                    label = pt.label, 
                    value = pt.code,
                    id = pt.id
                }).ToList();

                return Json(new { success = true, fields = fields }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Get existing product calculation for specific packing master
        public JsonResult GetProductCalculation(int trandid, int packingId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Looking for calculation: TRANDID={trandid}, PACKMID={packingId}");

                // Prefer header row (PACKTMID = 0) when present, otherwise fall back
                // to the first available slab row (PACKTMID <> 0). This supports both
                // legacy data (with header) and new design (headerless).
                var calculation = db.TransactionProductCalculations
                    .Where(t => t.TRANDID == trandid && t.PACKMID == packingId)
                    .OrderBy(t => t.PACKTMID)
                    .ThenBy(t => t.TRANPID)
                    .FirstOrDefault();

                if (calculation != null)
                {
                    // Load normalized slab detail rows (one per PACKTMID <> 0) for this TRANDID + PACKMID
                    var slabDetails = db.TransactionProductCalculations
                        .Where(t => t.TRANDID == trandid && t.PACKMID == packingId && t.PACKTMID != 0)
                        .Select(t => new
                        {
                            t.PACKTMID,
                            t.SLABVALUE
                        })
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"Found calculation header: TRANPID={calculation.TRANPID}, slab rows={slabDetails.Count}");

                    // Create a custom object to ensure proper JSON serialization of dates
                    var result = new
                    {
                        success = true,
                        calculation = new
                        {
                            TRANPID = calculation.TRANPID,
                            TRANDID = calculation.TRANDID,
                            PACKMID = calculation.PACKMID,
                            PCK1 = calculation.PCK1,
                            PCK2 = calculation.PCK2,
                            PCK3 = calculation.PCK3,
                            PCK4 = calculation.PCK4,
                            PCK5 = calculation.PCK5,
                            PCK6 = calculation.PCK6,
                            PCK7 = calculation.PCK7,
                            PCK8 = calculation.PCK8,
                            PCK9 = calculation.PCK9,
                            PCK10 = calculation.PCK10,
                            PCK11 = calculation.PCK11,
                            PCK12 = calculation.PCK12,
                            PCK13 = calculation.PCK13,
                            PCK14 = calculation.PCK14,
                            PCK15 = calculation.PCK15,
                            PCK16 = calculation.PCK16,
                            PCK17 = calculation.PCK17,
                            TOPCK = calculation.TOPCK,
                            PCKLVALUE = calculation.PCKLVALUE,
                            AVGPCKVALUE = calculation.AVGPCKVALUE,
                            PNDSVALUE = calculation.PNDSVALUE,
                            TOTALPNDS = calculation.TOTALPNDS,
                            YELDPERCENT = calculation.YELDPERCENT,
                            TOTALYELDCOUNTS = calculation.TOTALYELDCOUNTS,
                            KGWGT = calculation.KGWGT,
                            PCKKGWGT = calculation.PCKKGWGT,
                            PCKBOX = calculation.PCKBOX,
                            WASTEWGT = calculation.WASTEWGT,
                            WASTEPWGT = calculation.WASTEPWGT,
                            FACTORYWGT = calculation.FACTORYWGT,
                            FACAVGWGT = calculation.FACAVGWGT,
                            FACAVGCOUNT = calculation.FACAVGCOUNT,
                            PRODDATE = calculation.PRODDATE?.ToString("yyyy-MM-dd"), // Format date as string
                            CALCULATIONMODE = calculation.CALCULATIONMODE,
                            GRADEID = calculation.GRADEID,
                            PCLRID = calculation.PCLRID,
                            RCVDTID = calculation.RCVDTID,
                            BKN = calculation.BKN,
                            OTHERS = calculation.OTHERS,
                            DISPSTATUS = calculation.DISPSTATUS,
                            CUSRID = calculation.CUSRID,
                            LMUSRID = calculation.LMUSRID,
                            PRCSDATE = calculation.PRCSDATE,
                            // Normalized slab rows so the client can repopulate dynamic slab fields
                            SlabDetails = slabDetails
                        }
                    };

                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                System.Diagnostics.Debug.WriteLine("No calculation found for specified TRANDID and PACKMID");
                return Json(new { success = false, message = "No calculation found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting calculation: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Get all product calculations for a TRANDID (batch loading for better performance)
        public JsonResult GetAllProductCalculations(int trandid)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Batch loading calculations for TRANDID={trandid}");

                // Materialize all rows (header + slab detail rows) first
                var rawCalculations = db.TransactionProductCalculations
                    .Where(t => t.TRANDID == trandid)
                    .ToList();

                // Group by TRANDID + PACKMID so we can support both legacy header rows
                // (PACKTMID = 0) and new headerless rows (only PACKTMID <> 0).
                var calculations = rawCalculations
                    .GroupBy(c => new { c.TRANDID, c.PACKMID })
                    .Select(group =>
                    {
                        // Prefer header if present, otherwise use first slab row
                        var calculation = group
                            .OrderBy(c => c.PACKTMID) // 0 first when exists
                            .ThenBy(c => c.TRANPID)
                            .FirstOrDefault();

                        if (calculation == null)
                        {
                            return null;
                        }

                        return new
                        {
                            TRANPID = calculation.TRANPID,
                            TRANDID = calculation.TRANDID,
                            PACKMID = calculation.PACKMID,
                            PCK1 = calculation.PCK1,
                            PCK2 = calculation.PCK2,
                            PCK3 = calculation.PCK3,
                            PCK4 = calculation.PCK4,
                            PCK5 = calculation.PCK5,
                            PCK6 = calculation.PCK6,
                            PCK7 = calculation.PCK7,
                            PCK8 = calculation.PCK8,
                            PCK9 = calculation.PCK9,
                            PCK10 = calculation.PCK10,
                            PCK11 = calculation.PCK11,
                            PCK12 = calculation.PCK12,
                            PCK13 = calculation.PCK13,
                            PCK14 = calculation.PCK14,
                            PCK15 = calculation.PCK15,
                            PCK16 = calculation.PCK16,
                            PCK17 = calculation.PCK17,
                            TOPCK = calculation.TOPCK,
                            PCKLVALUE = calculation.PCKLVALUE,
                            AVGPCKVALUE = calculation.AVGPCKVALUE,
                            PNDSVALUE = calculation.PNDSVALUE,
                            TOTALPNDS = calculation.TOTALPNDS,
                            YELDPERCENT = calculation.YELDPERCENT,
                            TOTALYELDCOUNTS = calculation.TOTALYELDCOUNTS,
                            KGWGT = calculation.KGWGT,
                            PCKKGWGT = calculation.PCKKGWGT,
                            PCKBOX = calculation.PCKBOX,
                            WASTEWGT = calculation.WASTEWGT,
                            WASTEPWGT = calculation.WASTEPWGT,
                            FACTORYWGT = calculation.FACTORYWGT,
                            FACAVGWGT = calculation.FACAVGWGT,
                            FACAVGCOUNT = calculation.FACAVGCOUNT,
                            // IMPORTANT: format PRODDATE as a plain date string to avoid timezone shifts on the client
                            PRODDATE = calculation.PRODDATE.HasValue
                                ? calculation.PRODDATE.Value.ToString("yyyy-MM-dd")
                                : null,
                            CALCULATIONMODE = calculation.CALCULATIONMODE,
                            GRADEID = calculation.GRADEID,
                            PCLRID = calculation.PCLRID,
                            RCVDTID = calculation.RCVDTID,
                            BKN = calculation.BKN,
                            OTHERS = calculation.OTHERS,
                            DISPSTATUS = calculation.DISPSTATUS,
                            CUSRID = calculation.CUSRID,
                            LMUSRID = calculation.LMUSRID,
                            PRCSDATE = calculation.PRCSDATE,
                            // Attach normalized slab rows for this TRANDID + PACKMID (PACKTMID <> 0)
                            SlabDetails = group
                                .Where(s => s.PACKTMID != 0)
                                .Select(s => new
                                {
                                    s.PACKTMID,
                                    s.SLABVALUE
                                })
                                .ToList()
                        };
                    })
                    .Where(x => x != null)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Found {calculations.Count} calculation headers");

                return Json(new { success = true, calculations = calculations }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error batch loading calculations: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Save product calculation
        [HttpPost]
        public JsonResult SaveProductCalculation(FormCollection form)
        {
            try
            {
                // Manually parse form data to handle empty strings properly
                var model = ParseFormToModel(form);

                // Parse dynamic slab details when provided from the client
                List<SlabDetailDto> slabDetails = null;
                var slabDetailsJson = form["SlabDetailsJson"];
                if (!string.IsNullOrWhiteSpace(slabDetailsJson))
                {
                    try
                    {
                        slabDetails = JsonConvert.DeserializeObject<List<SlabDetailDto>>(slabDetailsJson);
                        System.Diagnostics.Debug.WriteLine($"Parsed {slabDetails?.Count ?? 0} slab detail rows from SlabDetailsJson");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error parsing SlabDetailsJson: {ex.Message}");
                        slabDetails = null;
                    }
                }
                
                // Debug: Log received model data
                System.Diagnostics.Debug.WriteLine($"Received calculation for TRANDID: {model.TRANDID}, PACKMID: {model.PACKMID}");
                System.Diagnostics.Debug.WriteLine($"PCK1: {model.PCK1}, PCK2: {model.PCK2}, PCK3: {model.PCK3}");
                System.Diagnostics.Debug.WriteLine($"PNDSVALUE: {model.PNDSVALUE}, YELDPERCENT: {model.YELDPERCENT}");
                System.Diagnostics.Debug.WriteLine($"KGWGT: {model.KGWGT}, WASTEWGT: {model.WASTEWGT}");
                
                // Sanitize PCK values - ensure null values are properly handled
                SanitizePCKValues(model);
                
                // VALIDATION: Grade, Production Colour and Received Type are required for each calculation
                if (model.GRADEID <= 0 || model.PCLRID <= 0 || model.RCVDTID <= 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Validation failed - GRADEID: {model.GRADEID}, PCLRID: {model.PCLRID}, RCVDTID: {model.RCVDTID}");
                    return Json(new { success = false, message = "Please select Grade, Production Colour and Received Type for this calculation." });
                }

                // Check if all PCK values are null
                var pckValues = new[] { model.PCK1, model.PCK2, model.PCK3, model.PCK4, model.PCK5, model.PCK6, 
                                       model.PCK7, model.PCK8, model.PCK9, model.PCK10, model.PCK11, model.PCK12, 
                                       model.PCK13, model.PCK14, model.PCK15, model.PCK16, model.PCK17 };
                var nonNullPcks = pckValues.Where(p => p > 0).ToArray();
                System.Diagnostics.Debug.WriteLine($"Non-zero PCK values: {nonNullPcks.Length}");
                
                if (nonNullPcks.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Warning: All PCK values are null or zero!");
                }
                // Get TRANMID from TRANDID
                var transactionDetail = db.Database.SqlQuery<int>(
                    "SELECT TRANMID FROM TRANSACTIONDETAIL WHERE TRANDID = @p0", model.TRANDID).FirstOrDefault();
                
                if (transactionDetail == 0)
                {
                    return Json(new { success = false, message = "Transaction detail not found" });
                }

                model.TRANMID = transactionDetail;

                // Calculate derived values
                if (slabDetails != null && slabDetails.Any())
                {
                    CalculateProductValuesFromSlabs(model, slabDetails);
                }
                else
                {
                    CalculateProductValues(model);
                }

                // Check if header record exists for this specific TRANDID and PACKMID combination
                var existing = db.TransactionProductCalculations
                    .FirstOrDefault(t => t.TRANDID == model.TRANDID && t.PACKMID == model.PACKMID && t.PACKTMID == 0);
                
                if (existing != null)
                {
                    // Update existing record
                    UpdateCalculationRecord(existing, model);
                    existing.LMUSRID = User?.Identity?.Name ?? "System";
                    existing.PRCSDATE = DateTime.Now;
                }
                else
                {
                    // New design: do not create a separate header row with PACKTMID = 0.
                    // Use the in-memory model only for calculations and let slab rows
                    // (PACKTMID <> 0) carry the persisted data.

                    model.TRANPID = GetNextTransactionProductId();
                    model.CUSRID = User?.Identity?.Name ?? "System";
                    model.LMUSRID = User?.Identity?.Name ?? "System";
                    model.DISPSTATUS = 0;
                    model.PRCSDATE = DateTime.Now;
                }

                // Maintain normalized slab details table (one row per PACKTMID)
                if (slabDetails != null && slabDetails.Any())
                {
                    SaveProductSlabDetails(model, slabDetails);
                }
                else
                {
                    SaveProductSlabDetails(model);
                }

                db.SaveChanges();
                
                // Return the TRANPID for tracking (prefer header when present, otherwise any slab row)
                var savedRecord = db.TransactionProductCalculations
                    .Where(t => t.TRANDID == model.TRANDID && t.PACKMID == model.PACKMID)
                    .OrderBy(t => t.PACKTMID)
                    .ThenBy(t => t.TRANPID)
                    .FirstOrDefault();
                
                return Json(new { 
                    success = true, 
                    message = "Calculation saved successfully",
                    tranpid = savedRecord?.TRANPID ?? 0
                });
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                System.Diagnostics.Debug.WriteLine($"EntityCommandExecutionException: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return Json(new { success = false, message = "Database error: " + (ex.InnerException?.Message ?? ex.Message) });
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                // Log full inner exception chain so we can see the actual SQL/server error
                System.Diagnostics.Debug.WriteLine($"DbUpdateException: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException?.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner-Inner: {ex.InnerException?.InnerException?.Message}");

                var innerMost = ex.InnerException?.InnerException ?? ex.InnerException;
                var message = innerMost?.Message ?? ex.Message;

                return Json(new { success = false, message = "Database error: " + message });
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                System.Diagnostics.Debug.WriteLine($"Validation Error: {fullErrorMessage}");
                return Json(new { success = false, message = "Validation error: " + fullErrorMessage });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        private void CalculateProductValues(TransactionProductCalculation model)
        {
            // Calculate TOPCK (sum of all PCK fields + BKN + OTHERS)
            model.TOPCK = model.PCK1 + model.PCK2 + model.PCK3 + model.PCK4 + 
                         model.PCK5 + model.PCK6 + model.PCK7 + model.PCK8 + 
                         model.PCK9 + model.PCK10 + model.PCK11 + model.PCK12 + 
                         model.PCK13 + model.PCK14 + model.PCK15 + model.PCK16 + 
                         model.PCK17 + model.BKN + model.OTHERS;

            // Check calculation mode to determine which calculations to perform
            bool isGradeWeightMode = model.CALCULATIONMODE == 2;
            System.Diagnostics.Debug.WriteLine($"Calculating in {(isGradeWeightMode ? "Grade Weight" : "Packing")} mode");
            
            if (isGradeWeightMode)
            {
                // Grade Weight mode: Slab (TOPCK), Peeled (WASTEWGT),
                // Packing Weight with Glazing (KGWGT), Pack KG Weight (PCKKGWGT),
                // Peeled + Weight (WASTEPWGT) and Factory Weight.

                // Clear count-based calculation fields not used in Grade Weight mode
                model.PCKLVALUE = 0;
                model.AVGPCKVALUE = 0;
                model.TOTALPNDS = 0;
                model.TOTALYELDCOUNTS = 0;

                // Default values
                model.PCKKGWGT = 0;
                model.WASTEPWGT = model.WASTEWGT; // start with just Peeled

                // When packing weight with glazing and slab (TOPCK) are provided,
                // calculate Pack KG Weight and Peeled + Weight
                if (model.KGWGT > 0 && model.TOPCK > 0)
                {
                    model.PCKKGWGT = model.KGWGT * model.TOPCK;            // Pack KG Weight
                    model.WASTEPWGT = model.PCKKGWGT + model.WASTEWGT;     // Peeled + Weight
                }

                // Factory Weight calculation:
                // - If both packing weight with glazing and yield percent are provided,
                //   use Factory Weight = (Peeled + Weight) / (Yield% / 100)
                // - Otherwise, fall back to simple Slab + Peeled (TOPCK + WASTEWGT)
                if (model.KGWGT > 0 && model.YELDPERCENT > 0 && model.TOPCK > 0)
                {
                    if (model.PCKKGWGT == 0)
                    {
                        model.PCKKGWGT = model.KGWGT * model.TOPCK;
                        model.WASTEPWGT = model.PCKKGWGT + model.WASTEWGT;
                    }

                    model.FACTORYWGT = model.WASTEPWGT / (model.YELDPERCENT / 100);
                }
                else
                {
                    // Backward-compatible behaviour when new fields are not used
                    model.FACTORYWGT = model.TOPCK + model.WASTEWGT;
                }

                System.Diagnostics.Debug.WriteLine($"Grade Weight calculation: Slab={model.TOPCK}, Peeled={model.WASTEWGT}, KGWGT={model.KGWGT}, YELD%={model.YELDPERCENT}, PCKKGWGT={model.PCKKGWGT}, WASTEPWGT={model.WASTEPWGT}, Factory Weight={model.FACTORYWGT}");
            }
            else if (model.TOPCK > 0)
            {
                // Packing mode: Full calculations
                // Calculate PCKLVALUE (multiply each PCK with its corresponding value and sum)
                model.PCKLVALUE = CalculatePCKLValue(model);

                // Calculate AVGPCKVALUE
                model.AVGPCKVALUE = model.PCKLVALUE / model.TOPCK;

                // Calculate TOTALPNDS
                model.TOTALPNDS = model.AVGPCKVALUE * model.PNDSVALUE;

                // Calculate TOTALYELDCOUNTS
                if (model.YELDPERCENT > 0)
                {
                    model.TOTALYELDCOUNTS = model.TOTALPNDS * (model.YELDPERCENT / 100);
                }

                // Calculate PCKKGWGT
                model.PCKKGWGT = model.KGWGT * model.TOPCK;

                // Calculate WASTEPWGT
                model.WASTEPWGT = model.PCKKGWGT + model.WASTEWGT;

                // Calculate FACTORYWGT (Packing mode formula)
                if (model.YELDPERCENT > 0)
                {
                    model.FACTORYWGT = model.WASTEPWGT / (model.YELDPERCENT / 100);
                }
            }
        }

        private decimal CalculatePCKLValue(TransactionProductCalculation model)
        {
            decimal pcklValue = 0;
            var pckValues = new[] { model.PCK1, model.PCK2, model.PCK3, model.PCK4, model.PCK5, model.PCK6, 
                                   model.PCK7, model.PCK8, model.PCK9, model.PCK10, model.PCK11, model.PCK12, 
                                   model.PCK13, model.PCK14, model.PCK15, model.PCK16, model.PCK17 };

            // Get packing types for this packing master (excluding BKN and OTHERS)
            var packingTypes = db.PackingTypeMasters
                .Where(pt => pt.PACKMID == model.PACKMID && pt.DISPSTATUS == 0)
                .OrderBy(pt => pt.PACKTMCODE)
                .ToList();

            if (packingTypes.Any())
            {
                int pckIndex = 0;
                foreach (var packingType in packingTypes)
                {
                    var desc = packingType.PACKTMDESC ?? string.Empty;
                    bool isBKN = IsBknLabel(desc);
                    bool isOTHERS = IsOthersLabel(desc);
                    
                    if (isBKN)
                    {
                        // Process BKN field separately
                        if (model.BKN > 0)
                        {
                            decimal multiplier = ExtractMultiplierFromDescription(packingType.PACKTMDESC);
                            pcklValue += model.BKN * multiplier;
                        }
                    }
                    else if (isOTHERS)
                    {
                        // Process OTHERS field separately
                        if (model.OTHERS > 0)
                        {
                            decimal multiplier = ExtractMultiplierFromDescription(packingType.PACKTMDESC);
                            pcklValue += model.OTHERS * multiplier;
                        }
                    }
                    else
                    {
                        // Process regular PCK field
                        if (pckIndex < pckValues.Length && pckValues[pckIndex] > 0)
                        {
                            decimal multiplier = ExtractMultiplierFromDescription(packingType.PACKTMDESC);
                            pcklValue += pckValues[pckIndex] * multiplier;
                        }
                        pckIndex++;
                    }
                }
            }

            return pcklValue;
        }

        private decimal CalculatePCKLValueFromSlabs(TransactionProductCalculation model, List<SlabDetailDto> slabDetails)
        {
            if (slabDetails == null || slabDetails.Count == 0)
            {
                return 0;
            }

            var packtmIds = slabDetails.Select(s => s.PACKTMID).Distinct().ToList();

            var packingTypes = db.PackingTypeMasters
                .Where(pt => pt.PACKMID == model.PACKMID && packtmIds.Contains(pt.PACKTMID) && (pt.DISPSTATUS == 0 || pt.DISPSTATUS == null))
                .ToList();

            decimal pcklValue = 0;

            foreach (var detail in slabDetails)
            {
                if (detail.SLABVALUE <= 0)
                {
                    continue;
                }

                var packingType = packingTypes.FirstOrDefault(pt => pt.PACKTMID == detail.PACKTMID);
                if (packingType == null)
                {
                    continue;
                }

                decimal multiplier = ExtractMultiplierFromDescription(packingType.PACKTMDESC);
                pcklValue += detail.SLABVALUE * multiplier;
            }

            return pcklValue;
        }

        private static bool IsBknLabel(string label)
        {
            if (string.IsNullOrWhiteSpace(label)) return false;
            var upper = label.ToUpper().Trim();
            return upper == "BKN" || upper == "BROKEN" || upper.Contains("BKN");
        }

        private static bool IsOthersLabel(string label)
        {
            if (string.IsNullOrWhiteSpace(label)) return false;
            var upper = label.ToUpper().Trim();
            return upper == "OTHERS" || upper == "OTHER" || upper.Contains("OTHERS");
        }

        private decimal ExtractMultiplierFromDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                return 1;

            // Extract number from descriptions like "U - 5", "U - 10", "51-60", "151-200", etc.
            var numbers = System.Text.RegularExpressions.Regex.Matches(description, @"\d+")
                .Cast<System.Text.RegularExpressions.Match>()
                .Select(m => decimal.Parse(m.Value))
                .ToArray();

            if (numbers.Length == 1)
            {
                // Single number like "U - 5" -> use 5
                return numbers[0];
            }
            else if (numbers.Length == 2)
            {
                // Range like "51-60" -> use the last/highest value (60)
                return numbers[1];
            }

            // Default fallback
            return 1;
        }

        private void UpdateCalculationRecord(TransactionProductCalculation existing, TransactionProductCalculation model)
        {
            existing.PACKMID = model.PACKMID;
            existing.PCK1 = model.PCK1;
            existing.PCK2 = model.PCK2;
            existing.PCK3 = model.PCK3;
            existing.PCK4 = model.PCK4;
            existing.PCK5 = model.PCK5;
            existing.PCK6 = model.PCK6;
            existing.PCK7 = model.PCK7;
            existing.PCK8 = model.PCK8;
            existing.PCK9 = model.PCK9;
            existing.PCK10 = model.PCK10;
            existing.PCK11 = model.PCK11;
            existing.PCK12 = model.PCK12;
            existing.PCK13 = model.PCK13;
            existing.PCK14 = model.PCK14;
            existing.PCK15 = model.PCK15;
            existing.PCK16 = model.PCK16;
            existing.PCK17 = model.PCK17;
            existing.TOPCK = model.TOPCK;
            existing.PCKLVALUE = model.PCKLVALUE;
            existing.AVGPCKVALUE = model.AVGPCKVALUE;
            existing.PNDSVALUE = model.PNDSVALUE;
            existing.TOTALPNDS = model.TOTALPNDS;
            existing.YELDPERCENT = model.YELDPERCENT;
            existing.TOTALYELDCOUNTS = model.TOTALYELDCOUNTS;
            existing.KGWGT = model.KGWGT;
            existing.PCKKGWGT = model.PCKKGWGT;
            existing.PCKBOX = model.PCKBOX;
            existing.WASTEWGT = model.WASTEWGT;
            existing.WASTEPWGT = model.WASTEPWGT;
            existing.FACTORYWGT = model.FACTORYWGT;
            existing.FACAVGWGT = model.FACAVGWGT;
            existing.FACAVGCOUNT = model.FACAVGCOUNT;
            existing.PRODDATE = model.PRODDATE;
            existing.CALCULATIONMODE = model.CALCULATIONMODE;
            existing.GRADEID = model.GRADEID;
            existing.PCLRID = model.PCLRID;
            existing.RCVDTID = model.RCVDTID;
            existing.BKN = model.BKN;
            existing.OTHERS = model.OTHERS;
        }

        private TransactionProductCalculation ParseFormToModel(FormCollection form)
        {
            var model = new TransactionProductCalculation();
            
            // Parse required integer fields
            if (int.TryParse(form["TRANPID"], out int tranpid)) model.TRANPID = tranpid;
            if (int.TryParse(form["TRANDID"], out int trandid)) model.TRANDID = trandid;
            if (int.TryParse(form["TRANMID"], out int tranmid)) model.TRANMID = tranmid;
            if (int.TryParse(form["PACKMID"], out int packmid)) model.PACKMID = packmid;
            
            // Parse PCK fields - default to 0 for non-nullable decimals
            model.PCK1 = ParseNullableDecimal(form["PCK1"]) ?? 0;
            model.PCK2 = ParseNullableDecimal(form["PCK2"]) ?? 0;
            model.PCK3 = ParseNullableDecimal(form["PCK3"]) ?? 0;
            model.PCK4 = ParseNullableDecimal(form["PCK4"]) ?? 0;
            model.PCK5 = ParseNullableDecimal(form["PCK5"]) ?? 0;
            model.PCK6 = ParseNullableDecimal(form["PCK6"]) ?? 0;
            model.PCK7 = ParseNullableDecimal(form["PCK7"]) ?? 0;
            model.PCK8 = ParseNullableDecimal(form["PCK8"]) ?? 0;
            model.PCK9 = ParseNullableDecimal(form["PCK9"]) ?? 0;
            model.PCK10 = ParseNullableDecimal(form["PCK10"]) ?? 0;
            model.PCK11 = ParseNullableDecimal(form["PCK11"]) ?? 0;
            model.PCK12 = ParseNullableDecimal(form["PCK12"]) ?? 0;
            model.PCK13 = ParseNullableDecimal(form["PCK13"]) ?? 0;
            model.PCK14 = ParseNullableDecimal(form["PCK14"]) ?? 0;
            model.PCK15 = ParseNullableDecimal(form["PCK15"]) ?? 0;
            model.PCK16 = ParseNullableDecimal(form["PCK16"]) ?? 0;
            model.PCK17 = ParseNullableDecimal(form["PCK17"]) ?? 0;
            
            // Parse other decimal fields - default to 0 for non-nullable decimals
            model.PNDSVALUE = ParseNullableDecimal(form["PNDSVALUE"]) ?? 0;
            model.YELDPERCENT = ParseNullableDecimal(form["YELDPERCENT"]) ?? 0;
            model.KGWGT = ParseNullableDecimal(form["KGWGT"]) ?? 0;
            model.WASTEWGT = ParseNullableDecimal(form["WASTEWGT"]) ?? 0;
            model.FACAVGWGT = ParseNullableDecimal(form["FACAVGWGT"]) ?? 0;
            model.FACAVGCOUNT = ParseNullableDecimal(form["FACAVGCOUNT"]) ?? 0;

            // Parse PCKBOX (No of Cases) - required int, default 0 if not provided
            if (int.TryParse(form["PCKBOX"], out int pckbox))
            {
                model.PCKBOX = pckbox;
            }
            else
            {
                model.PCKBOX = 0;
            }
            
            // Parse BKN field (Broken) - default to 0 for non-nullable decimal
            model.BKN = ParseNullableDecimal(form["BKN"]) ?? 0;
            
            // Parse OTHERS field - default to 0 for non-nullable decimal
            model.OTHERS = ParseNullableDecimal(form["OTHERS"]) ?? 0;
            
            // Parse PRODDATE field
            model.PRODDATE = ParseNullableDateTime(form["PRODDATE"]);
            
            // Parse CALCULATIONMODE field - Convert frontend string to backend integer
            var calculationModeString = form["CalculationMode"] ?? "packing";
            model.CALCULATIONMODE = calculationModeString == "gradeweight" ? 2 : 1; // 1=Packing, 2=Grade Weight
            
            // Parse new dropdown fields (not required, default to 0)
            model.GRADEID = ParseNullableInt(form["GRADEID"]) ?? 0;
            model.PCLRID = ParseNullableInt(form["PCLRID"]) ?? 0;
            model.RCVDTID = ParseNullableInt(form["RCVDTID"]) ?? 0;
            
            System.Diagnostics.Debug.WriteLine($"Parsed model - TRANDID: {model.TRANDID}, PACKMID: {model.PACKMID}");
            System.Diagnostics.Debug.WriteLine($"Parsed PCK1: '{form["PCK1"]}' -> {model.PCK1}");
            System.Diagnostics.Debug.WriteLine($"Parsed PCK2: '{form["PCK2"]}' -> {model.PCK2}");
            System.Diagnostics.Debug.WriteLine($"Parsed PCK3: '{form["PCK3"]}' -> {model.PCK3}");
            System.Diagnostics.Debug.WriteLine($"Parsed BKN: '{form["BKN"]}' -> {model.BKN}");
            System.Diagnostics.Debug.WriteLine($"Parsed GRADEID: '{form["GRADEID"]}' -> {model.GRADEID}");
            System.Diagnostics.Debug.WriteLine($"Parsed PCLRID: '{form["PCLRID"]}' -> {model.PCLRID}");
            System.Diagnostics.Debug.WriteLine($"Parsed RCVDTID: '{form["RCVDTID"]}' -> {model.RCVDTID}");
            
            return model;
        }
        
        private decimal? ParseNullableDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
                
            if (decimal.TryParse(value, out decimal result))
                return result;
                
            return null;
        }
        
        private DateTime? ParseNullableDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
                
            if (DateTime.TryParse(value, out DateTime result))
                return result;
                
            return null;
        }
        
        private int? ParseNullableInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
                
            if (int.TryParse(value, out int result))
                return result;
                
            return null;
        }

        private void SanitizePCKValues(TransactionProductCalculation model)
        {
            if (model.TRANDID <= 0)
            {
                throw new ArgumentException("TRANDID is required and must be greater than 0");
            }
            
            if (model.PACKMID <= 0)
            {
                throw new ArgumentException("PACKMID is required and must be greater than 0");
            }
            
            System.Diagnostics.Debug.WriteLine("PCK values sanitized - zero values converted to null");
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

        // Quality Check Methods
        public JsonResult CheckQualityCheckTable()
        {
            try
            {
                // Test if the table exists by trying to query it
                var count = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TRANSACTION_QUALITY_CHECK'").FirstOrDefault();
                
                if (count == 0)
                {
                    return Json(new { 
                        success = false, 
                        message = "TRANSACTION_QUALITY_CHECK table does not exist. Please run the database script to create it.",
                        tableExists = false
                    }, JsonRequestBehavior.AllowGet);
                }
                
                return Json(new { 
                    success = true, 
                    message = "Table exists",
                    tableExists = true
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = ex.Message,
                    tableExists = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckTransactionExists(int tranmid)
        {
            try
            {
                // Check if TRANMID exists in TRANSACTIONMASTER
                var exists = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM TRANSACTIONMASTER WHERE TRANMID = @p0", tranmid).FirstOrDefault();
                
                if (exists > 0)
                {
                    // Get transaction details
                    var transaction = db.Database.SqlQuery<dynamic>("SELECT TRANMID, TRANDATE, CATENAME, CATECODE FROM TRANSACTIONMASTER WHERE TRANMID = @p0", tranmid).FirstOrDefault();
                    
                    return Json(new { 
                        success = true, 
                        exists = true,
                        message = $"Transaction {tranmid} exists",
                        transaction = transaction
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Get available TRANMIDs
                    var availableIds = db.Database.SqlQuery<int>("SELECT TOP 10 TRANMID FROM TRANSACTIONMASTER ORDER BY TRANMID DESC").ToList();
                    
                    return Json(new { 
                        success = false, 
                        exists = false,
                        message = $"Transaction {tranmid} does not exist",
                        availableIds = availableIds
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = $"Error checking transaction: {ex.Message}"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult TestQualityCheckInsert(int tranmid)
        {
            try
            {
                // First check if TRANMID exists
                var transactionExists = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM TRANSACTIONMASTER WHERE TRANMID = @p0", tranmid).FirstOrDefault();
                if (transactionExists == 0)
                {
                    return Json(new { 
                        success = false, 
                        message = $"Cannot insert: TRANMID {tranmid} does not exist in TRANSACTIONMASTER table"
                    }, JsonRequestBehavior.AllowGet);
                }

                // Try to insert a test record using raw SQL to bypass Entity Framework
                var sql = @"
                    INSERT INTO TRANSACTION_QUALITY_CHECK 
                    (TRANMID, LABOID, STATUS, REMARKS, DONEBY, VERIFIEDBY, LOTDATE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE)
                    VALUES 
                    (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10)";
                
                var result = db.Database.ExecuteSqlCommand(sql, 
                    tranmid,           // TRANMID
                    1,                 // LABOID (assuming lab with ID 1 exists)
                    0,                 // STATUS
                    "Test remarks",    // REMARKS
                    "Test User",       // DONEBY
                    "Test Verifier",   // VERIFIEDBY
                    DateTime.Now,      // LOTDATE
                    "System",          // CUSRID
                    "System",          // LMUSRID
                    0,                 // DISPSTATUS
                    DateTime.Now       // PRCSDATE
                );
                
                return Json(new { 
                    success = true, 
                    message = $"Test insert successful. Rows affected: {result}"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = $"Test insert failed: {ex.Message}"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetLaboratories()
        {
            try
            {
                var laboratories = db.LaboratoryMasters
                    .Where(l => l.DISPSTATUS == 0)
                    .OrderBy(l => l.LABODESC)
                    .Select(l => new { id = l.LABOID, text = l.LABODESC })
                    .ToList();
                return Json(laboratories, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetQualityCheck(int tranmid)
        {
            try
            {
                var qualityCheck = db.TransactionQualityChecks
                    .FirstOrDefault(q => q.TRANMID == tranmid);
                    
                if (qualityCheck != null)
                {
                    return Json(new { 
                        success = true, 
                        data = new {
                            TRANQID = qualityCheck.TRANQID,
                            TRANMID = qualityCheck.TRANMID,
                            LABOID = qualityCheck.LABOID,
                            STATUS = qualityCheck.STATUS,
                            REMARKS = qualityCheck.REMARKS,
                            DONEBY = qualityCheck.DONEBY,
                            VERIFIEDBY = qualityCheck.VERIFIEDBY,
                            LOTDATE = qualityCheck.LOTDATE.ToString("yyyy-MM-dd")
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                
                return Json(new { success = false, message = "No quality check found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveQualityCheckRawSQL(FormCollection form)
        {
            try
            {
                var tranmid = int.Parse(form["TRANMID"]);
                var laboid = int.Parse(form["LABOID"]);
                var status = int.Parse(form["STATUS"]);
                var remarks = form["REMARKS"]?.Trim();
                var doneby = form["DONEBY"]?.Trim();
                var verifiedby = form["VERIFIEDBY"]?.Trim();
                var lotdate = DateTime.Parse(form["LOTDATE"]);
                
                System.Diagnostics.Debug.WriteLine($"SaveQualityCheckRawSQL called with TRANMID: {tranmid}");
                
                // First check if TRANMID exists in TRANSACTIONMASTER
                var transactionExists = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM TRANSACTIONMASTER WHERE TRANMID = @p0", tranmid).FirstOrDefault();
                if (transactionExists == 0)
                {
                    var availableIds = db.Database.SqlQuery<int>("SELECT TOP 5 TRANMID FROM TRANSACTIONMASTER ORDER BY TRANMID DESC").ToList();
                    return Json(new { 
                        success = false, 
                        message = $"Transaction {tranmid} does not exist in TRANSACTIONMASTER. Available IDs: {string.Join(", ", availableIds)}"
                    });
                }
                
                // Check if record exists
                var existingCount = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM TRANSACTION_QUALITY_CHECK WHERE TRANMID = @p0", tranmid).FirstOrDefault();
                
                if (existingCount > 0)
                {
                    // Update
                    var updateSql = @"
                        UPDATE TRANSACTION_QUALITY_CHECK 
                        SET LABOID = @p1, STATUS = @p2, REMARKS = @p3, DONEBY = @p4, VERIFIEDBY = @p5, 
                            LOTDATE = @p6, LMUSRID = @p7, PRCSDATE = @p8
                        WHERE TRANMID = @p0";
                    
                    var result = db.Database.ExecuteSqlCommand(updateSql, 
                        tranmid, laboid, status, remarks, doneby, verifiedby, lotdate, 
                        User?.Identity?.Name ?? "System", DateTime.Now);
                    
                    return Json(new { success = true, message = "Quality check updated successfully" });
                }
                else
                {
                    // Insert
                    var insertSql = @"
                        INSERT INTO TRANSACTION_QUALITY_CHECK 
                        (TRANMID, LABOID, STATUS, REMARKS, DONEBY, VERIFIEDBY, LOTDATE, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE)
                        VALUES 
                        (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10)";
                    
                    var result = db.Database.ExecuteSqlCommand(insertSql, 
                        tranmid, laboid, status, remarks, doneby, verifiedby, lotdate,
                        User?.Identity?.Name ?? "System", User?.Identity?.Name ?? "System", 0, DateTime.Now);
                    
                    return Json(new { success = true, message = "Quality check saved successfully" });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveQualityCheckRawSQL Exception: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SaveQualityCheck(TransactionQualityCheck model)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"SaveQualityCheck called with TRANMID: {model.TRANMID}");
                
                if (model.TRANMID <= 0)
                {
                    return Json(new { success = false, message = "Invalid Transaction Master ID" });
                }

                // Validate required fields
                if (model.LABOID <= 0)
                {
                    return Json(new { success = false, message = "Laboratory is required" });
                }

                // Check if TRANMID exists in TRANSACTIONMASTER
                var transactionExists = db.TransactionMasters.Any(t => t.TRANMID == model.TRANMID);
                if (!transactionExists)
                {
                    return Json(new { success = false, message = $"Transaction Master with ID {model.TRANMID} does not exist" });
                }

                // Check if LABOID exists in LABORATORYMASTER
                var laboratoryExists = db.LaboratoryMasters.Any(l => l.LABOID == model.LABOID);
                if (!laboratoryExists)
                {
                    return Json(new { success = false, message = $"Laboratory with ID {model.LABOID} does not exist" });
                }

                if (string.IsNullOrWhiteSpace(model.REMARKS))
                {
                    return Json(new { success = false, message = "Remarks is required" });
                }

                if (string.IsNullOrWhiteSpace(model.DONEBY))
                {
                    return Json(new { success = false, message = "Done By is required" });
                }

                if (string.IsNullOrWhiteSpace(model.VERIFIEDBY))
                {
                    return Json(new { success = false, message = "Verified By is required" });
                }

                // Validate LOTDATE
                if (model.LOTDATE == DateTime.MinValue)
                {
                    return Json(new { success = false, message = "Date is required" });
                }

                // Validate string lengths
                if (model.DONEBY?.Length > 100)
                {
                    return Json(new { success = false, message = "Done By cannot exceed 100 characters" });
                }

                if (model.VERIFIEDBY?.Length > 100)
                {
                    return Json(new { success = false, message = "Verified By cannot exceed 100 characters" });
                }

                // Check if record exists
                var existing = db.TransactionQualityChecks
                    .FirstOrDefault(q => q.TRANMID == model.TRANMID);
                
                if (existing != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Updating existing quality check record: {existing.TRANQID}");
                    
                    // Update existing record
                    existing.LABOID = model.LABOID;
                    existing.STATUS = model.STATUS;
                    existing.REMARKS = model.REMARKS?.Trim();
                    existing.DONEBY = model.DONEBY?.Trim();
                    existing.VERIFIEDBY = model.VERIFIEDBY?.Trim();
                    existing.LOTDATE = model.LOTDATE;
                    existing.LMUSRID = User?.Identity?.Name ?? "System";
                    existing.PRCSDATE = DateTime.Now;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Creating new quality check record");
                    
                    // Create new record
                    model.CUSRID = User?.Identity?.Name ?? "System";
                    model.LMUSRID = User?.Identity?.Name ?? "System";
                    model.DISPSTATUS = 0;
                    model.PRCSDATE = DateTime.Now;
                    
                    // Trim string fields
                    model.REMARKS = model.REMARKS?.Trim();
                    model.DONEBY = model.DONEBY?.Trim();
                    model.VERIFIEDBY = model.VERIFIEDBY?.Trim();
                    
                    // Debug log the data being saved
                    System.Diagnostics.Debug.WriteLine($"Quality Check Data:");
                    System.Diagnostics.Debug.WriteLine($"  TRANMID: {model.TRANMID}");
                    System.Diagnostics.Debug.WriteLine($"  LABOID: {model.LABOID}");
                    System.Diagnostics.Debug.WriteLine($"  STATUS: {model.STATUS}");
                    System.Diagnostics.Debug.WriteLine($"  REMARKS: '{model.REMARKS}' (Length: {model.REMARKS?.Length ?? 0})");
                    System.Diagnostics.Debug.WriteLine($"  DONEBY: '{model.DONEBY}' (Length: {model.DONEBY?.Length ?? 0})");
                    System.Diagnostics.Debug.WriteLine($"  VERIFIEDBY: '{model.VERIFIEDBY}' (Length: {model.VERIFIEDBY?.Length ?? 0})");
                    System.Diagnostics.Debug.WriteLine($"  LOTDATE: {model.LOTDATE}");
                    System.Diagnostics.Debug.WriteLine($"  CUSRID: '{model.CUSRID}'");
                    System.Diagnostics.Debug.WriteLine($"  LMUSRID: '{model.LMUSRID}'");
                    System.Diagnostics.Debug.WriteLine($"  DISPSTATUS: {model.DISPSTATUS}");
                    System.Diagnostics.Debug.WriteLine($"  PRCSDATE: {model.PRCSDATE}");
                    
                    db.TransactionQualityChecks.Add(model);
                }

                db.SaveChanges();
                System.Diagnostics.Debug.WriteLine("Quality check saved successfully");
                
                return Json(new { 
                    success = true, 
                    message = "Quality check saved successfully"
                });
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                System.Diagnostics.Debug.WriteLine($"DbUpdateException: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                
                // Get the deepest inner exception
                Exception innermost = ex;
                while (innermost.InnerException != null)
                {
                    innermost = innermost.InnerException;
                    System.Diagnostics.Debug.WriteLine($"Deeper Inner Exception: {innermost.Message}");
                }
                
                var errorMessage = "Database error occurred. ";
                var innermostMessage = innermost.Message;
                
                if (innermostMessage.Contains("Invalid object name 'dbo.TRANSACTION_QUALITY_CHECK'"))
                {
                    errorMessage += "The TRANSACTION_QUALITY_CHECK table does not exist. Please run the database script to create it.";
                }
                else if (innermostMessage.Contains("foreign key constraint") || innermostMessage.Contains("FOREIGN KEY"))
                {
                    errorMessage += "Foreign key constraint violation. Please check if the referenced records exist.";
                }
                else if (innermostMessage.Contains("Cannot insert the value NULL"))
                {
                    errorMessage += "Required field is missing or null: " + innermostMessage;
                }
                else if (innermostMessage.Contains("String or binary data would be truncated"))
                {
                    errorMessage += "Data too long for field: " + innermostMessage;
                }
                else
                {
                    errorMessage += innermostMessage;
                }
                
                return Json(new { success = false, message = errorMessage });
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                System.Diagnostics.Debug.WriteLine($"Validation Error: {fullErrorMessage}");
                return Json(new { success = false, message = "Validation error: " + fullErrorMessage });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return Json(new { success = false, message = "Unexpected error: " + ex.Message });
            }
        }


        // Test method to check if controller is accessible
        public ActionResult TestPDF()
        {
            return Content("PDF Controller is working! Current time: " + DateTime.Now.ToString());
        }

        // Helper class for PDF transaction details
        public class PDFTransactionDetail
        {
            public int TRANDID { get; set; }
            public int TRANMID { get; set; }
            public int MTRLGID { get; set; }
            public int MTRLID { get; set; }
            public int MTRLNBOX { get; set; }
            public int MTRLCOUNTS { get; set; }
            public string ProductType { get; set; }
            public string Product { get; set; }
        }

        // Direct PDF Generation Methods
        [Authorize(Roles = "RawMaterialIntakeCalculationPrint")]
        public ActionResult GenerateCalculationPDF(int tranmid)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[PDF] GenerateCalculationPDF called with tranmid: {tranmid}");
                
                // Get transaction master details
                var transaction = db.TransactionMasters.FirstOrDefault(t => t.TRANMID == tranmid);
                if (transaction == null)
                {
                    return HttpNotFound("Transaction not found");
                }

                // Get transaction details with products
                var transactionDetails = db.Database.SqlQuery<PDFTransactionDetail>(@"
                    SELECT td.TRANDID, td.MTRLGID, td.MTRLID, td.MTRLNBOX, td.MTRLCOUNTS,
                           mg.MTRLGDESC as ProductType, m.MTRLDESC as Product
                    FROM TRANSACTIONDETAIL td
                    INNER JOIN MATERIALGROUPMASTER mg ON mg.MTRLGID = td.MTRLGID
                    INNER JOIN MATERIALMASTER m ON m.MTRLID = td.MTRLID
                    WHERE td.TRANMID = @p0
                    ORDER BY td.TRANDID", tranmid).ToList();

                System.Diagnostics.Debug.WriteLine($"[PDF] Found {transactionDetails.Count} transaction details");

                // Get all calculations for this transaction
                var allCalculations = db.TransactionProductCalculations
                    .Where(c => c.TRANMID == tranmid)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"[PDF] Found {allCalculations.Count} calculations");

                // Group calculations by TRANDID
                var calculationsByTrandid = allCalculations.GroupBy(c => c.TRANDID).ToList();

                // Generate HTML content for PDF
                var htmlContent = GeneratePDFContent(transaction, transactionDetails, calculationsByTrandid);

                System.Diagnostics.Debug.WriteLine($"[PDF] Generated HTML content length: {htmlContent.Length}");

                // Return HTML content as a view result for PDF generation
                ViewBag.HtmlContent = htmlContent;
                return View("PDFView");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PDF] Error generating PDF: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[PDF] Stack trace: {ex.StackTrace}");
                
                // Return an error page instead of JSON for better debugging
                ViewBag.ErrorMessage = $"Error generating PDF: {ex.Message}";
                ViewBag.HtmlContent = $@"
                    <html>
                    <head><title>PDF Generation Error</title></head>
                    <body>
                        <h1>PDF Generation Error</h1>
                        <p><strong>Error:</strong> {ex.Message}</p>
                        <p><strong>Transaction ID:</strong> {tranmid}</p>
                        <p><strong>Time:</strong> {DateTime.Now}</p>
                        <hr>
                        <p>Please check the console logs for more details.</p>
                    </body>
                    </html>";
                return View("PDFView");
            }
        }

        // Generate PDF for specific TRANDID (single row)
        [Authorize(Roles = "RawMaterialIntakeCalculationPrint")]
        public ActionResult GenerateRowCalculationPDF(int trandid)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[PDF] GenerateRowCalculationPDF called with trandid: {trandid}");
                
                // Get transaction detail for this TRANDID
                var transactionDetail = db.Database.SqlQuery<PDFTransactionDetail>(@"
                    SELECT td.TRANDID, td.TRANMID, td.MTRLGID, td.MTRLID, td.MTRLNBOX, td.MTRLCOUNTS,
                           mg.MTRLGDESC as ProductType, m.MTRLDESC as Product
                    FROM TRANSACTIONDETAIL td
                    INNER JOIN MATERIALGROUPMASTER mg ON mg.MTRLGID = td.MTRLGID
                    INNER JOIN MATERIALMASTER m ON m.MTRLID = td.MTRLID
                    WHERE td.TRANDID = @p0", trandid).FirstOrDefault();

                if (transactionDetail == null)
                {
                    return HttpNotFound("Transaction detail not found");
                }

                // Get transaction master details
                var transaction = db.TransactionMasters.FirstOrDefault(t => t.TRANMID == transactionDetail.TRANMID);
                if (transaction == null)
                {
                    return HttpNotFound("Transaction master not found");
                }

                // Get calculations for this specific TRANDID
                var calculations = db.TransactionProductCalculations
                    .Where(c => c.TRANDID == trandid)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"[PDF] Found {calculations.Count} calculations for TRANDID {trandid}");

                // Generate HTML content for PDF
                var htmlContent = GenerateRowPDFContent(transaction, transactionDetail, calculations);

                System.Diagnostics.Debug.WriteLine($"[PDF] Generated HTML content length: {htmlContent.Length}");

                // Return HTML content as a view result for PDF generation
                ViewBag.HtmlContent = htmlContent;
                return View("PDFView");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PDF] Error generating row PDF: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[PDF] Stack trace: {ex.StackTrace}");
                
                // Return an error page instead of JSON for better debugging
                ViewBag.ErrorMessage = $"Error generating PDF: {ex.Message}";
                ViewBag.HtmlContent = $@"
                    <html>
                    <head><title>PDF Generation Error</title></head>
                    <body>
                        <h1>PDF Generation Error</h1>
                        <p><strong>Error:</strong> {ex.Message}</p>
                        <p><strong>Transaction Detail ID:</strong> {trandid}</p>
                        <p><strong>Time:</strong> {DateTime.Now}</p>
                        <hr>
                        <p>Please check the console logs for more details.</p>
                    </body>
                    </html>";
                return View("PDFView");
            }
        }

        private string GenerateRowPDFContent(TransactionMaster transaction, PDFTransactionDetail transactionDetail, 
            List<TransactionProductCalculation> calculations)
        {
            // Get first calculation to extract Grade, ProductionColour, ReceivedType
            var firstCalc = calculations?.FirstOrDefault();
            string gradeName = "N/A";
            string productionColour = "N/A";
            string receivedType = "N/A";
            
            if (firstCalc != null)
            {
                try
                {
                    if (firstCalc.GRADEID > 0)
                    {
                        var grade = db.GradeMasters.FirstOrDefault(g => g.GRADEID == firstCalc.GRADEID);
                        gradeName = grade?.GRADEDESC ?? "N/A";
                    }
                }
                catch { gradeName = "N/A"; }
                
                try
                {
                    if (firstCalc.PCLRID > 0)
                    {
                        var pColour = db.ProductionColourMasters.FirstOrDefault(p => p.PCLRID == firstCalc.PCLRID);
                        productionColour = pColour?.PCLRDESC ?? "N/A";
                    }
                }
                catch { productionColour = "N/A"; }
                
                try
                {
                    if (firstCalc.RCVDTID > 0)
                    {
                        var rType = db.ReceivedTypeMasters.FirstOrDefault(r => r.RCVDTID == firstCalc.RCVDTID);
                        receivedType = rType?.RCVDTDESC ?? "N/A";
                    }
                }
                catch { receivedType = "N/A"; }
            }
            
            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Product Calculation Report - Single Row</title>
    <style>
        @page {{ margin: 0; size: auto; }}
        @media print {{ 
            body {{ margin: 1cm; }} 
            .no-print {{ display: none; }}
        }}
        * {{ -webkit-print-color-adjust: exact; print-color-adjust: exact; }}
        body {{ font-family: 'Segoe UI', Arial, sans-serif; margin: 20px; line-height: 1.6; color: #000; background-color: #f8f9fa; }}
        .header {{ text-align: center; border-bottom: 3px solid #000; padding: 20px; margin-bottom: 10px; background-color: #2c3e50; color: #fff; -webkit-print-color-adjust: exact; print-color-adjust: exact; }}
        .report-date {{ text-align: center; font-size: 13px; color: #666; margin: 10px 0 20px 0; padding: 8px; background-color: #fff; border-radius: 4px; }}
        .transaction-row {{ display: table; width: 100%; margin-bottom: 20px; border-spacing: 10px; page-break-after: always; }}
        .transaction-left {{ display: table-cell; width: 50%; vertical-align: top; padding: 15px; background-color: #fff; border: 1px solid #dee2e6; border-radius: 4px; }}
        .transaction-right {{ display: table-cell; width: 50%; vertical-align: top; padding: 15px; background-color: #fff; border: 1px solid #dee2e6; border-radius: 4px; }}
        .info-box {{ padding: 10px 0; border-bottom: 1px solid #f0f0f0; }}
        .info-box:last-child {{ border-bottom: none; }}
        .info-label {{ font-weight: 600; color: #495057; display: inline-block; min-width: 140px; }}
        .info-value {{ color: #212529; display: inline-block; }}
        .section {{ margin-bottom: 25px; page-break-inside: avoid; background-color: #fff; padding: 20px; border-radius: 4px; border: 1px solid #dee2e6; }}
        .product-header {{ background-color: #2c3e50; color: white; padding: 12px 15px; margin: 20px 0 20px 0; font-size: 15px; font-weight: 600; border-radius: 4px; page-break-before: always; }}
        .calculation-header {{ background-color: #495057; color: white; padding: 10px 15px; margin: 0 0 15px 0; font-weight: 600; font-size: 14px; }}
        .inline-row {{ display: table; width: 100%; margin: 15px 0; border-spacing: 10px; }}
        .inline-cell {{ display: table-cell; width: 33.33%; padding: 0; }}
        .inline-cell-50 {{ display: table-cell; width: 50%; padding: 0; }}
        .detail-box {{ background-color: #fff; padding: 10px 15px; border: 1px solid #dee2e6; border-radius: 4px; }}
        .detail-box-alt {{ background-color: #fff; padding: 10px 15px; border: 1px solid #dee2e6; border-radius: 4px; }}
        .detail-box-purple {{ background-color: #fff; padding: 10px 15px; border: 1px solid #dee2e6; border-radius: 4px; }}
        .detail-box strong {{ color: #495057; font-size: 12px; font-weight: 600; }}
        .detail-box, .detail-box-alt, .detail-box-purple {{ font-size: 13px; color: #212529; }}
        .horizontal-table {{ width: 100%; border-collapse: collapse; margin: 15px 0; background-color: white; border: 1px solid #dee2e6; table-layout: fixed; }}
        .horizontal-table th {{ background-color: #495057; color: white; padding: 8px 4px; text-align: center; font-size: 11px; font-weight: 600; border: 1px solid #dee2e6; word-wrap: break-word; }}
        .horizontal-table td {{ border: 1px solid #dee2e6; padding: 8px 4px; text-align: center; font-weight: 600; color: #212529; background-color: #f8f9fa; font-size: 13px; }}
        .horizontal-table td:last-child {{ background-color: #e9ecef; color: #2c3e50; font-weight: 700; }}
        .calc-columns {{ display: table; width: 100%; margin-top: 15px; border-spacing: 10px; }}
        .calc-left {{ display: table-cell; width: 50%; vertical-align: top; }}
        .calc-right {{ display: table-cell; width: 50%; vertical-align: top; }}
        .calc-box {{ background-color: #f8f9fa; padding: 15px; border-radius: 4px; margin-bottom: 15px; border: 1px solid #dee2e6; }}
        .calc-title {{ background-color: #6c757d; color: white; padding: 8px; font-weight: 600; margin: -15px -15px 12px -15px; text-align: center; font-size: 13px; }}
        .calc-row {{ padding: 8px 0; border-bottom: 1px solid #e9ecef; display: table; width: 100%; }}
        .calc-row:last-child {{ border-bottom: none; }}
        .calc-label {{ display: table-cell; font-weight: 500; color: #495057; width: 60%; font-size: 13px; }}
        .calc-value {{ display: table-cell; font-weight: 700; color: #212529; text-align: right; width: 40%; font-size: 14px; }}
        .total-row {{ background-color: #e9ecef; border-radius: 4px; padding: 10px 8px; margin-top: 8px; border: 1px solid #dee2e6; display: block; overflow: hidden; }}
        .total-row .calc-label {{ font-weight: 700; color: #2c3e50; display: table-cell; width: 60%; }}
        .total-row .calc-value {{ color: #2c3e50; font-size: 15px; display: table-cell; width: 40%; text-align: right; }}
        .no-calculations {{ text-align: center; color: #6c757d; font-style: italic; padding: 40px; background-color: #f8f9fa; border-radius: 4px; }}
        h2 {{ margin: 0; font-size: 24px; font-weight: 700; letter-spacing: 0.5px; color: #fff; }}
        h3 {{ margin: 5px 0 0 0; font-size: 15px; font-weight: 500; color: #fff; opacity: 1; }}
    </style>
</head>
<body>
    <div class='header'>
        <h2>MARINEX - Product Calculation Report</h2>
        <h3>Raw Material Intake Calculation Details</h3>
    </div>
    
    <div class='report-date'>
        <strong>Report Generated:</strong> {DateTime.Now:dd-MM-yyyy HH:mm:ss}
    </div>
    
    <div class='transaction-row'>
        <div class='transaction-left'>
            <div class='info-box'>
                <span class='info-label'>Transaction No:</span>
                <span class='info-value'>{transaction.TRANDNO ?? "N/A"}</span>
            </div>
            <div class='info-box'>
                <span class='info-label'>Transaction Date:</span>
                <span class='info-value'>{transaction.TRANDATE:dd-MM-yyyy}</span>
            </div>
        </div>
        <div class='transaction-right'>
            <div class='info-box'>
                <span class='info-label'>Supplier Name:</span>
                <span class='info-value'>{transaction.CATENAME ?? "N/A"}</span>
            </div>
            <div class='info-box'>
                <span class='info-label'>Supplier Code:</span>
                <span class='info-value'>{transaction.CATECODE ?? "N/A"}</span>
            </div>
            <div class='info-box'>
                <span class='info-label'>Vehicle No:</span>
                <span class='info-value'>{transaction.VECHNO ?? "N/A"}</span>
            </div>
            <div class='info-box'>
                <span class='info-label'>Client Weight (KG):</span>
                <span class='info-value'>{transaction.CLIENTWGHT:F3}</span>
            </div>
        </div>
    </div>
    
    <div class='product-header'>
        Product: {transactionDetail.Product} ({transactionDetail.ProductType}) - Boxes: {transactionDetail.MTRLNBOX}, Count: {transactionDetail.MTRLCOUNTS}
    </div>";

            // Add calculations for this specific product
            if (calculations != null && calculations.Any())
            {
                // Group by packing master
                var packingGroups = calculations.GroupBy(c => c.PACKMID);
                
                foreach (var packingGroup in packingGroups)
                {
                    var packingId = packingGroup.Key;
                    var calculation = packingGroup.First();
                    
                    // Get packing master name
                    var packingMaster = db.PackingMasters.FirstOrDefault(p => p.PACKMID == packingId);
                    var packingName = packingMaster?.PACKMDESC ?? $"Packing {packingId}";

                    var calculationMode = calculation.CALCULATIONMODE == 2 ? "Grade Weight Mode" : "Packing Mode";
                    
                    html += $@"
    <div class='section'>
        <div class='calculation-header'>
            {packingName} Calculation
        </div>
        
        <div class='inline-row'>
            <div class='inline-cell-50'>
                <div class='detail-box'>";
                    
                    if (calculation.PRODDATE.HasValue)
                    {
                        html += $@"
                    <strong>Production Date:</strong> {calculation.PRODDATE.Value:dd-MM-yyyy}";
                    }
                    else
                    {
                        html += @"
                    <strong>Production Date:</strong> N/A";
                    }
                    
                    html += $@"
                </div>
            </div>
            <div class='inline-cell-50'>
                <div class='detail-box-alt'>
                    <strong>Calculation Mode:</strong> {calculationMode}
                </div>
            </div>
        </div>
        
        <div class='inline-row'>
            <div class='inline-cell'>
                <div class='detail-box-purple'>
                    <strong>Grade:</strong> {gradeName}
                </div>
            </div>
            <div class='inline-cell'>
                <div class='detail-box-purple'>
                    <strong>Production Colour:</strong> {productionColour}
                </div>
            </div>
            <div class='inline-cell'>
                <div class='detail-box-purple'>
                    <strong>Received Type:</strong> {receivedType}
                </div>
            </div>
        </div>";

                    // Build packing details from either legacy header PCK fields or
                    // new headless normalized slab rows, so that PDF always reflects
                    // the latest slab values shown in the UI.
                    var nonZeroPackings = new System.Collections.Generic.List<(string label, decimal value)>();

                    bool hasHeaderRow = packingGroup.Any(c => c.PACKTMID == 0);

                    if (hasHeaderRow)
                    {
                        // Legacy mode: use header row PCK1..PCK17 mapped to packing type labels
                        var headerCalc = packingGroup
                            .Where(c => c.PACKTMID == 0)
                            .OrderBy(c => c.TRANPID)
                            .First();

                        var packingFields = GetPackingTypeFieldsForPDF(packingId);

                        var pckValues = new[]
                        {
                            headerCalc.PCK1, headerCalc.PCK2, headerCalc.PCK3, headerCalc.PCK4,
                            headerCalc.PCK5, headerCalc.PCK6, headerCalc.PCK7, headerCalc.PCK8,
                            headerCalc.PCK9, headerCalc.PCK10, headerCalc.PCK11, headerCalc.PCK12,
                            headerCalc.PCK13, headerCalc.PCK14, headerCalc.PCK15, headerCalc.PCK16,
                            headerCalc.PCK17
                        };

                        for (int i = 0; i < pckValues.Length && i < packingFields.Count; i++)
                        {
                            if (pckValues[i] > 0)
                            {
                                nonZeroPackings.Add((packingFields[i], pckValues[i]));
                            }
                        }
                    }
                    else
                    {
                        // Headless mode: derive slab values directly from normalized
                        // rows (PACKTMID <> 0) using packing type descriptions.
                        var packingTypes = db.PackingTypeMasters
                            .Where(pt => pt.PACKMID == packingId && (pt.DISPSTATUS == 0 || pt.DISPSTATUS == null))
                            .OrderBy(pt => pt.PACKTMCODE)
                            .ToList();

                        foreach (var pt in packingTypes)
                        {
                            var desc = pt.PACKTMDESC ?? string.Empty;

                            // Skip BKN / OTHERS here; they are appended explicitly below
                            if (IsBknLabel(desc) || IsOthersLabel(desc))
                            {
                                continue;
                            }

                            var slabRow = packingGroup
                                .Where(c => c.PACKTMID == pt.PACKTMID)
                                .OrderByDescending(c => c.SLABVALUE)
                                .FirstOrDefault();

                            if (slabRow != null && slabRow.SLABVALUE > 0)
                            {
                                nonZeroPackings.Add((desc, slabRow.SLABVALUE));
                            }
                        }
                    }

                    // Append BKN and OTHERS fields explicitly using their own values
                    if (calculation.BKN > 0)
                    {
                        nonZeroPackings.Add(("Bkn", calculation.BKN));
                    }
                    if (calculation.OTHERS > 0)
                    {
                        nonZeroPackings.Add(("Others", calculation.OTHERS));
                    }

                    html += @"
        <div style='margin: 15px 0;'>
            <div style='background-color: #6c757d; color: white; padding: 10px; font-weight: 600; margin-bottom: 10px; text-align: center; font-size: 13px;'>Packing Details</div>
            <table class='horizontal-table'>
                <tr>";
                    
                    // Add header row
                    foreach (var pck in nonZeroPackings)
                    {
                        html += $@"
                    <th>{pck.label}</th>";
                    }
                    html += $@"
                    <th style='background-color: #495057;'>Total</th>";
                    
                    html += @"
                </tr>
                <tr>";
                    
                    // Add value row
                    foreach (var pck in nonZeroPackings)
                    {
                        html += $@"
                    <td>{FormatDecimalForPdf(pck.value)}</td>";
                    }
                    html += $@"
                    <td style='background-color: #e9ecef; font-size: 15px; color: #2c3e50; font-weight: 700;'>{FormatDecimalForPdf(calculation.TOPCK)}</td>";
                    
                    html += @"
                </tr>
            </table>
        </div>";

                    // Display calculation results based on mode
                    if (calculation.CALCULATIONMODE == 2) // Grade Weight Mode
                    {
                        html += $@"
        <div class='calc-box'>
            <div class='calc-title'>Grade Weight Calculation</div>
            <div class='calc-row'>
                <span class='calc-label'>Slab:</span>
                <span class='calc-value'>{FormatDecimalForPdf(calculation.TOPCK)}</span>
            </div>
            <div class='calc-row'>
                <span class='calc-label'>Peeled:</span>
                <span class='calc-value'>{FormatDecimalForPdf(calculation.WASTEWGT)}</span>
            </div>
            <div class='calc-row total-row'>
                <span class='calc-label'>Factory Weight:</span>
                <span class='calc-value'>{FormatDecimalForPdf(calculation.FACTORYWGT)}</span>
            </div>
        </div>";
                    }
                    else // Packing Mode
                    {
                        html += @"
        <div class='calc-columns'>
            <div class='calc-left'>
                <div class='calc-box'>
                    <div class='calc-title'>Slab Calculation</div>
                    <div class='calc-row'>
                        <span class='calc-label'>Total PCK:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.TOPCK) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>PCK L Value:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.PCKLVALUE) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>Avg PCK Value:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.AVGPCKVALUE) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>PNDS Value:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.PNDSVALUE) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>Total PNDS:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.TOTALPNDS) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>Yield Percent:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.YELDPERCENT) + @"%" + @"</span>
                    </div>
                    <div class='total-row'>
                        <span class='calc-label'>Total Yield Counts:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.TOTALYELDCOUNTS) + @"</span>
                    </div>
                </div>
            </div>
            <div class='calc-right'>
                <div class='calc-box'>
                    <div class='calc-title'>Weight Details</div>
                    <div class='calc-row'>
                        <span class='calc-label'>KG Weight:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.KGWGT) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>No of Cases:</span>
                        <span class='calc-value'>" + calculation.PCKBOX + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>PCK KG Weight:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.PCKKGWGT) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>Waste Weight:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.WASTEWGT) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>Waste P Weight:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.WASTEPWGT) + @"</span>
                    </div>
                    <div class='calc-row total-row'>
                        <span class='calc-label'>Factory Weight:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.FACTORYWGT) + @"</span>
                    </div>
                </div>
            </div>
        </div>";
                    }

                    html += @"
        </div>
    </div>"; // End section
                }
            }
            else
            {
                html += @"
    <div class='section'>
        <div class='no-calculations'>No calculations available for this product.</div>
    </div>";
            }

            html += @"
</body>
</html>";

            return html;
        }

        private string GeneratePDFContent(TransactionMaster transaction, List<PDFTransactionDetail> transactionDetails, 
            IEnumerable<IGrouping<int, TransactionProductCalculation>> calculationsByTrandid)
        {
            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Product Calculation Report</title>
    <style>
        @page {{ margin: 0; size: auto; }}
        @media print {{ 
            body {{ margin: 1cm; }} 
            .no-print {{ display: none; }}
        }}
        * {{ -webkit-print-color-adjust: exact; print-color-adjust: exact; }}
        body {{ font-family: 'Segoe UI', Arial, sans-serif; margin: 20px; line-height: 1.6; color: #000; background-color: #f8f9fa; }}
        .header {{ text-align: center; border-bottom: 3px solid #000; padding: 20px; margin-bottom: 10px; background-color: #2c3e50; color: #fff; -webkit-print-color-adjust: exact; print-color-adjust: exact; }}
        .report-date {{ text-align: center; font-size: 13px; color: #666; margin: 10px 0 20px 0; padding: 8px; background-color: #fff; border-radius: 4px; }}
        .transaction-row {{ display: table; width: 100%; margin-bottom: 20px; border-spacing: 10px; }}
        .transaction-left {{ display: table-cell; width: 50%; vertical-align: top; padding: 15px; background-color: #fff; border: 1px solid #dee2e6; border-radius: 4px; }}
        .transaction-right {{ display: table-cell; width: 50%; vertical-align: top; padding: 15px; background-color: #fff; border: 1px solid #dee2e6; border-radius: 4px; }}
        .info-box {{ padding: 10px 0; border-bottom: 1px solid #f0f0f0; }}
        .info-box:last-child {{ border-bottom: none; }}
        .info-label {{ font-weight: 600; color: #495057; display: inline-block; min-width: 140px; }}
        .info-value {{ color: #212529; display: inline-block; }}
        .section {{ margin-bottom: 25px; page-break-inside: avoid; background-color: #fff; padding: 20px; border-radius: 4px; border: 1px solid #dee2e6; }}
        .product-header {{ background-color: #2c3e50; color: white; padding: 12px 15px; margin: -20px -20px 20px -20px; font-size: 15px; font-weight: 600; border-radius: 4px 4px 0 0; }}
        .calculation-header {{ background-color: #495057; color: white; padding: 10px 15px; margin: 0 0 15px 0; font-weight: 600; font-size: 14px; }}
        .inline-row {{ display: table; width: 100%; margin: 15px 0; border-spacing: 10px; }}
        .inline-cell {{ display: table-cell; width: 33.33%; padding: 0; }}
        .inline-cell-50 {{ display: table-cell; width: 50%; padding: 0; }}
        .detail-box {{ background-color: #fff; padding: 10px 15px; border: 1px solid #dee2e6; border-radius: 4px; }}
        .detail-box-alt {{ background-color: #fff; padding: 10px 15px; border: 1px solid #dee2e6; border-radius: 4px; }}
        .detail-box-purple {{ background-color: #fff; padding: 10px 15px; border: 1px solid #dee2e6; border-radius: 4px; }}
        .detail-box strong {{ color: #495057; font-size: 12px; font-weight: 600; }}
        .detail-box, .detail-box-alt, .detail-box-purple {{ font-size: 13px; color: #212529; }}
        .horizontal-table {{ width: 100%; border-collapse: collapse; margin: 15px 0; background-color: white; border: 1px solid #dee2e6; table-layout: fixed; }}
        .horizontal-table th {{ background-color: #495057; color: white; padding: 8px 4px; text-align: center; font-size: 11px; font-weight: 600; border: 1px solid #dee2e6; word-wrap: break-word; }}
        .horizontal-table td {{ border: 1px solid #dee2e6; padding: 8px 4px; text-align: center; font-weight: 600; color: #212529; background-color: #f8f9fa; font-size: 13px; }}
        .horizontal-table td:last-child {{ background-color: #e9ecef; color: #2c3e50; font-weight: 700; }}
        .calc-columns {{ display: table; width: 100%; margin-top: 15px; border-spacing: 10px; }}
        .calc-left {{ display: table-cell; width: 50%; vertical-align: top; }}
        .calc-right {{ display: table-cell; width: 50%; vertical-align: top; }}
        .calc-box {{ background-color: #f8f9fa; padding: 15px; border-radius: 4px; margin-bottom: 15px; border: 1px solid #dee2e6; }}
        .calc-title {{ background-color: #6c757d; color: white; padding: 8px; font-weight: 600; margin: -15px -15px 12px -15px; text-align: center; font-size: 13px; }}
        .calc-row {{ padding: 8px 0; border-bottom: 1px solid #e9ecef; display: table; width: 100%; }}
        .calc-row:last-child {{ border-bottom: none; }}
        .calc-label {{ display: table-cell; font-weight: 500; color: #495057; width: 60%; font-size: 13px; }}
        .calc-value {{ display: table-cell; font-weight: 700; color: #212529; text-align: right; width: 40%; font-size: 14px; }}
        .total-row {{ background-color: #e9ecef; border-radius: 4px; padding: 10px 8px; margin-top: 8px; border: 1px solid #dee2e6; display: block; overflow: hidden; }}
        .total-row .calc-label {{ font-weight: 700; color: #2c3e50; display: table-cell; width: 60%; }}
        .total-row .calc-value {{ color: #2c3e50; font-size: 15px; display: table-cell; width: 40%; text-align: right; }}
        .no-calculations {{ text-align: center; color: #6c757d; font-style: italic; padding: 40px; background-color: #f8f9fa; border-radius: 4px; }}
        .page-break {{ page-break-before: always; }}
        h2 {{ margin: 0; font-size: 22px; font-weight: 600; letter-spacing: 0.5px; }}
        h3 {{ margin: 5px 0 0 0; font-size: 14px; font-weight: 400; opacity: 0.9; }}
    </style>
</head>
<body>
    <div class='header'>
        <h2>MARINEX - Product Calculation Report</h2>
        <h3>Raw Material Intake Calculation Details</h3>
    </div>
    
    <div class='report-date'>
        <strong>Report Generated:</strong> {DateTime.Now:dd-MM-yyyy HH:mm:ss}
    </div>
    
    <div class='transaction-row'>
        <div class='transaction-left'>
            <div class='info-box'>
                <span class='info-label'>Transaction No:</span>
                <span class='info-value'>{transaction.TRANDNO ?? "N/A"}</span>
            </div>
            <div class='info-box'>
                <span class='info-label'>Transaction Date:</span>
                <span class='info-value'>{transaction.TRANDATE:dd-MM-yyyy}</span>
            </div>
        </div>
        <div class='transaction-right'>
            <div class='info-box'>
                <span class='info-label'>Supplier Name:</span>
                <span class='info-value'>{transaction.CATENAME ?? "N/A"}</span>
            </div>
            <div class='info-box'>
                <span class='info-label'>Supplier Code:</span>
                <span class='info-value'>{transaction.CATECODE ?? "N/A"}</span>
            </div>
            <div class='info-box'>
                <span class='info-label'>Vehicle No:</span>
                <span class='info-value'>{transaction.VECHNO ?? "N/A"}</span>
            </div>
            <div class='info-box'>
                <span class='info-label'>Client Weight (KG):</span>
                <span class='info-value'>{transaction.CLIENTWGHT:F3}</span>
            </div>
        </div>
    </div>";

            // Add calculations for each transaction detail
            foreach (var detail in transactionDetails)
            {
                var trandid = detail.TRANDID;
                var productType = detail.ProductType;
                var product = detail.Product;
                var boxes = detail.MTRLNBOX;
                var counts = detail.MTRLCOUNTS;

                html += $@"
    <div class='section'>
        <div class='product-header'>
            Product: {product} ({productType}) - Boxes: {boxes}, Count: {counts}
        </div>";

                // Get calculations for this TRANDID
                var calculations = calculationsByTrandid.FirstOrDefault(g => g.Key == trandid);
                if (calculations != null && calculations.Any())
                {
                    // Group by packing master
                    var packingGroups = calculations.GroupBy(c => c.PACKMID);
                    
                    foreach (var packingGroup in packingGroups)
                    {
                        var packingId = packingGroup.Key;
                        var calculation = packingGroup.First();
                        
                        // Get packing master name
                        var packingMaster = db.PackingMasters.FirstOrDefault(p => p.PACKMID == packingId);
                        var packingName = packingMaster?.PACKMDESC ?? $"Packing {packingId}";

                        // Get Grade, Production Colour, Received Type with error handling
                        string gradeName = "N/A";
                        string productionColour = "N/A";
                        string receivedType = "N/A";
                        
                        try
                        {
                            if (calculation.GRADEID > 0)
                            {
                                var grade = db.GradeMasters.FirstOrDefault(g => g.GRADEID == calculation.GRADEID);
                                gradeName = grade?.GRADEDESC ?? "N/A";
                            }
                        }
                        catch { gradeName = "N/A"; }
                        
                        try
                        {
                            if (calculation.PCLRID > 0)
                            {
                                var pColour = db.ProductionColourMasters.FirstOrDefault(p => p.PCLRID == calculation.PCLRID);
                                productionColour = pColour?.PCLRDESC ?? "N/A";
                            }
                        }
                        catch { productionColour = "N/A"; }
                        
                        try
                        {
                            if (calculation.RCVDTID > 0)
                            {
                                var rType = db.ReceivedTypeMasters.FirstOrDefault(r => r.RCVDTID == calculation.RCVDTID);
                                receivedType = rType?.RCVDTDESC ?? "N/A";
                            }
                        }
                        catch { receivedType = "N/A"; }
                        
                        var calculationMode = calculation.CALCULATIONMODE == 2 ? "Grade Weight Mode" : "Packing Mode";
                        
                        html += $@"
        <div class='calculation-header'>
            {packingName} Calculation
        </div>
        
        <div class='inline-row'>
            <div class='inline-cell-50'>
                <div class='detail-box'>";
                        
                        if (calculation.PRODDATE.HasValue)
                        {
                            html += $@"
                    <strong>Production Date:</strong> {calculation.PRODDATE.Value:dd-MM-yyyy}";
                        }
                        else
                        {
                            html += @"
                    <strong>Production Date:</strong> N/A";
                        }
                        
                        html += $@"
                </div>
            </div>
            <div class='inline-cell-50'>
                <div class='detail-box-alt'>
                    <strong>Calculation Mode:</strong> {calculationMode}
                </div>
            </div>
        </div>
        
        <div class='inline-row'>
            <div class='inline-cell'>
                <div class='detail-box-purple'>
                    <strong>Grade:</strong> {gradeName}
                </div>
            </div>
            <div class='inline-cell'>
                <div class='detail-box-purple'>
                    <strong>Production Colour:</strong> {productionColour}
                </div>
            </div>
            <div class='inline-cell'>
                <div class='detail-box-purple'>
                    <strong>Received Type:</strong> {receivedType}
                </div>
            </div>
        </div>";

                        // Build packing details from either legacy header PCK fields or
                        // new headless normalized slab rows, so that PDF always reflects
                        // the latest slab values shown in the UI.
                        var nonZeroPackings = new System.Collections.Generic.List<(string label, decimal value)>();

                        bool hasHeaderRow = packingGroup.Any(c => c.PACKTMID == 0);

                        if (hasHeaderRow)
                        {
                            // Legacy mode: use header row PCK1..PCK17 mapped to packing type labels
                            var headerCalc = packingGroup
                                .Where(c => c.PACKTMID == 0)
                                .OrderBy(c => c.TRANPID)
                                .First();

                            var packingFields = GetPackingTypeFieldsForPDF(packingId);

                            var pckValues = new[]
                            {
                                headerCalc.PCK1, headerCalc.PCK2, headerCalc.PCK3, headerCalc.PCK4,
                                headerCalc.PCK5, headerCalc.PCK6, headerCalc.PCK7, headerCalc.PCK8,
                                headerCalc.PCK9, headerCalc.PCK10, headerCalc.PCK11, headerCalc.PCK12,
                                headerCalc.PCK13, headerCalc.PCK14, headerCalc.PCK15, headerCalc.PCK16,
                                headerCalc.PCK17
                            };

                            for (int i = 0; i < pckValues.Length && i < packingFields.Count; i++)
                            {
                                if (pckValues[i] > 0)
                                {
                                    nonZeroPackings.Add((packingFields[i], pckValues[i]));
                                }
                            }
                        }
                        else
                        {
                            // Headless mode: derive slab values directly from normalized
                            // rows (PACKTMID <> 0) using packing type descriptions.
                            var packingTypes = db.PackingTypeMasters
                                .Where(pt => pt.PACKMID == packingId && (pt.DISPSTATUS == 0 || pt.DISPSTATUS == null))
                                .OrderBy(pt => pt.PACKTMCODE)
                                .ToList();

                            foreach (var pt in packingTypes)
                            {
                                var desc = pt.PACKTMDESC ?? string.Empty;

                                // Skip BKN / OTHERS here; they are appended explicitly below
                                if (IsBknLabel(desc) || IsOthersLabel(desc))
                                {
                                    continue;
                                }

                                var slabRow = packingGroup
                                    .Where(c => c.PACKTMID == pt.PACKTMID)
                                    .OrderByDescending(c => c.SLABVALUE)
                                    .FirstOrDefault();

                                if (slabRow != null && slabRow.SLABVALUE > 0)
                                {
                                    nonZeroPackings.Add((desc, slabRow.SLABVALUE));
                                }
                            }
                        }

                        // Append BKN and OTHERS fields explicitly using their own values
                        if (calculation.BKN > 0)
                        {
                            nonZeroPackings.Add(("Bkn", calculation.BKN));
                        }
                        if (calculation.OTHERS > 0)
                        {
                            nonZeroPackings.Add(("Others", calculation.OTHERS));
                        }

                        html += @"
        <div style='margin: 15px 0;'>
            <div style='background-color: #6c757d; color: white; padding: 10px; font-weight: 600; margin-bottom: 10px; text-align: center; font-size: 13px;'>Packing Details</div>
            <table class='horizontal-table'>
                <tr>";
                        
                        // Add header row
                        foreach (var pck in nonZeroPackings)
                        {
                            html += $@"
                    <th>{pck.label}</th>";
                        }
                        html += $@"
                    <th style='background-color: #495057;'>Total</th>";
                        
                        html += @"
                </tr>
                <tr>";
                        
                        // Add value row
                        foreach (var pck in nonZeroPackings)
                        {
                            html += $@"
                    <td>{FormatDecimalForPdf(pck.value)}</td>";
                        }
                        html += $@"
                    <td style='background-color: #e9ecef; font-size: 15px; color: #2c3e50; font-weight: 700;'>{FormatDecimalForPdf(calculation.TOPCK)}</td>";
                        
                        html += @"
                </tr>
            </table>
        </div>";

                        // Display calculation results based on mode
                        if (calculation.CALCULATIONMODE == 2) // Grade Weight Mode
                        {
                            html += $@"
        <div class='calc-box'>
            <div class='calc-title'>Grade Weight Calculation</div>
            <div class='calc-row'>
                <span class='calc-label'>Slab:</span>
                <span class='calc-value'>{FormatDecimalForPdf(calculation.TOPCK)}</span>
            </div>
            <div class='calc-row'>
                <span class='calc-label'>Peeled:</span>
                <span class='calc-value'>{FormatDecimalForPdf(calculation.WASTEWGT)}</span>
            </div>
            <div class='calc-row total-row'>
                <span class='calc-label'>Factory Weight:</span>
                <span class='calc-value'>{FormatDecimalForPdf(calculation.FACTORYWGT)}</span>
            </div>
        </div>";
                        }
                        else // Packing Mode
                        {
                            html += @"
        <div class='calc-columns'>
            <div class='calc-left'>
                <div class='calc-box'>
                    <div class='calc-title'>Slab Calculation</div>
                    <div class='calc-row'>
                        <span class='calc-label'>Total PCK:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.TOPCK) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>PCK L Value:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.PCKLVALUE) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>Avg PCK Value:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.AVGPCKVALUE) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>PNDS Value:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.PNDSVALUE) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>Total PNDS:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.TOTALPNDS) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>Yield Percent:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.YELDPERCENT) + @"%" + @"</span>
                    </div>
                    <div class='total-row'>
                        <span class='calc-label'>Total Yield Counts:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.TOTALYELDCOUNTS) + @"</span>
                    </div>
                </div>
            </div>
            <div class='calc-right'>
                <div class='calc-box'>
                    <div class='calc-title'>Weight Details</div>
                    <div class='calc-row'>
                        <span class='calc-label'>KG Weight:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.KGWGT) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>No of Cases:</span>
                        <span class='calc-value'>" + calculation.PCKBOX + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>PCK KG Weight:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.PCKKGWGT) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>Waste Weight:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.WASTEWGT) + @"</span>
                    </div>
                    <div class='calc-row'>
                        <span class='calc-label'>Waste P Weight:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.WASTEPWGT) + @"</span>
                    </div>
                    <div class='calc-row total-row'>
                        <span class='calc-label'>Factory Weight:</span>
                        <span class='calc-value'>" + FormatDecimalForPdf(calculation.FACTORYWGT) + @"</span>
                    </div>
                </div>
            </div>
        </div>";
                        }

                        html += @"
        </div>"; // End packing-section
                    }
                }
                else
                {
                    html += @"
        <div class='no-calculations'>No calculations available for this product.</div>";
                }

                html += @"
    </div>"; // End section
            }

            html += @"
</body>
</html>";

            return html;
        }

        private string FormatDecimalForPdf(decimal value)
        {
            var rounded = decimal.Round(value, 3);
            if (decimal.Truncate(rounded) == rounded)
            {
                return rounded.ToString("0");
            }
            return rounded.ToString("0.000");
        }

        private string FormatDecimalForPdf(decimal? value)
        {
            if (!value.HasValue)
            {
                return "0";
            }
            return FormatDecimalForPdf(value.Value);
        }

        private List<string> GetPackingTypeFieldsForPDF(int packingId)
        {
            try
            {
                // Get packing types in database order and filter out BKN / OTHERS
                var packingTypes = db.PackingTypeMasters
                    .Where(pt => pt.PACKMID == packingId && pt.DISPSTATUS == 0)
                    .OrderBy(pt => pt.PACKTMCODE)
                    .ToList();

                var labels = new List<string>();
                foreach (var pt in packingTypes)
                {
                    var desc = pt.PACKTMDESC ?? string.Empty;
                    if (!IsBknLabel(desc) && !IsOthersLabel(desc))
                    {
                        labels.Add(desc);
                    }
                }

                return labels.Any() ? labels : new List<string>
                {
                    "PCK1", "PCK2", "PCK3", "PCK4", "PCK5", "PCK6", "PCK7", "PCK8", "PCK9",
                    "PCK10", "PCK11", "PCK12", "PCK13", "PCK14", "PCK15", "PCK16", "PCK17"
                };
            }
            catch
            {
                return new List<string>
                {
                    "PCK1", "PCK2", "PCK3", "PCK4", "PCK5", "PCK6", "PCK7", "PCK8", "PCK9",
                    "PCK10", "PCK11", "PCK12", "PCK13", "PCK14", "PCK15", "PCK16", "PCK17"
                };
            }
        }

        // Generate Transaction PDF for Index page Print button
        [Authorize(Roles = "RawMaterialIntakePrint")]
        public ActionResult GenerateTransactionPDF(int id)
        {
            try
            {
                var htmlContent = GenerateTransactionPDFContent(id);
                
                Response.ContentType = "text/html";
                Response.AddHeader("Content-Disposition", $"inline; filename=RawMaterialIntake_{id}.html");
                
                return Content(htmlContent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GenerateTransactionPDF] Error: {ex.Message}");
                return Content($"<html><body><h3>Error generating PDF: {ex.Message}</h3></body></html>");
            }
        }

        private string GenerateTransactionPDFContent(int tranmid)
        {
            // Get transaction master data
            var transaction = db.TransactionMasters.FirstOrDefault(t => t.TRANMID == tranmid);
            if (transaction == null)
            {
                return "<html><body><h3>Transaction not found</h3></body></html>";
            }

            // Get transaction details with product information using Entity Framework
            var details = (from td in db.TransactionDetails
                          join m in db.MaterialMasters on td.MTRLID equals m.MTRLID
                          join mg in db.MaterialGroupMasters on td.MTRLGID equals mg.MTRLGID
                          where td.TRANMID == tranmid
                          select new
                          {
                              td.TRANDID,
                              td.TRANMID,
                              td.MTRLNBOX,
                              td.MTRLCOUNTS,
                              ProductType = mg.MTRLGDESC,
                              ProductName = m.MTRLDESC
                          }).OrderBy(x => x.TRANDID).ToList();

            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Raw Material Intake - {transaction.TRANMID}</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; background: #f8f9fa; }}
        .container {{ max-width: 800px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; margin-bottom: 30px; padding-bottom: 20px; border-bottom: 3px solid #007bff; }}
        .company-name {{ font-size: 28px; font-weight: bold; color: #007bff; margin-bottom: 5px; }}
        .document-title {{ font-size: 20px; color: #6c757d; margin-bottom: 10px; }}
        .transaction-info {{ background: linear-gradient(135deg, #e3f2fd 0%, #bbdefb 100%); padding: 20px; border-radius: 8px; margin-bottom: 25px; }}
        .info-row {{ display: flex; justify-content: space-between; margin-bottom: 10px; }}
        .info-label {{ font-weight: bold; color: #1976d2; }}
        .info-value {{ color: #424242; }}
        .section-title {{ font-size: 18px; font-weight: bold; color: #007bff; margin: 25px 0 15px 0; padding-bottom: 8px; border-bottom: 2px solid #e9ecef; }}
        .table {{ width: 100%; border-collapse: collapse; margin-bottom: 20px; }}
        .table th, .table td {{ padding: 12px; text-align: left; border-bottom: 1px solid #dee2e6; }}
        .table th {{ background: linear-gradient(135deg, #007bff, #0056b3); color: white; font-weight: 600; }}
        .table tbody tr:nth-child(even) {{ background-color: #f8f9fa; }}
        .table tbody tr:hover {{ background-color: #e3f2fd; }}
        .value {{ text-align: right; font-weight: 600; }}
        .total-row {{ background: linear-gradient(135deg, #28a745, #20c997) !important; color: white; font-weight: bold; }}
        .no-data {{ text-align: center; color: #6c757d; font-style: italic; padding: 20px; }}
        @media print {{ 
            body {{ background: white; }} 
            .container {{ box-shadow: none; }} 
            @page {{ margin: 1in; }}
        }}
    </style>
    <script>
        window.onload = function() {{
            window.print();
        }};
    </script>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='company-name'>MARINEX</div>
            <div class='document-title'>Raw Material Intake Report</div>
            <div style='color: #6c757d; font-size: 14px;'>Transaction ID: {transaction.TRANMID}</div>
        </div>

        <div class='transaction-info'>
            <div class='info-row'>
                <span class='info-label'>Date:</span>
                <span class='info-value'>{transaction.TRANDATE:dd/MM/yyyy}</span>
            </div>
            <div class='info-row'>
                <span class='info-label'>Supplier Name:</span>
                <span class='info-value'>{transaction.CATENAME}</span>
            </div>
            <div class='info-row'>
                <span class='info-label'>Supplier Code:</span>
                <span class='info-value'>{transaction.CATECODE}</span>
            </div>
            <div class='info-row'>
                <span class='info-label'>Vehicle No:</span>
                <span class='info-value'>{transaction.VECHNO}</span>
            </div>
            <div class='info-row'>
                <span class='info-label'>Client Weight (KG):</span>
                <span class='info-value'>{transaction.CLIENTWGHT:F2} KG</span>
            </div>
        </div>";

            if (details.Any())
            {
                html += @"
        <div class='section-title'>Product Details</div>
        <table class='table'>
            <thead>
                <tr>
                    <th>S.No</th>
                    <th>Product Type</th>
                    <th>Product Name</th>
                    <th>No of Boxes</th>
                    <th>Counts per KG</th>
                </tr>
            </thead>
            <tbody>";

                int serialNo = 1;
                int totalBoxes = 0;

                foreach (var detail in details)
                {
                    totalBoxes += detail.MTRLNBOX;

                    html += $@"
                <tr>
                    <td>{serialNo}</td>
                    <td>{detail.ProductType}</td>
                    <td>{detail.ProductName}</td>
                    <td class='value'>{detail.MTRLNBOX}</td>
                    <td class='value'>{detail.MTRLCOUNTS}</td>
                </tr>";
                    serialNo++;
                }

                html += $@"
                <tr class='total-row'>
                    <td colspan='3'><strong>Total No.of Boxes</strong></td>
                    <td colspan='2' class='value' style='text-align: center;'><strong>{totalBoxes}</strong></td>
                </tr>
            </tbody>
        </table>";
            }
            else
            {
                html += @"
        <div class='section-title'>Product Details</div>
        <div class='no-data'>No product details available for this transaction.</div>";
            }

            html += $@"
        <div style='margin-top: 40px; text-align: center; color: #6c757d; font-size: 12px;'>
            Generated on {DateTime.Now:dd/MM/yyyy HH:mm} | MARINEX ERP System
        </div>
    </div>
</body>
</html>";

            return html;
        }

        // Get total factory weight for a specific transaction detail (product row)
        [HttpGet]
        public ActionResult GetFactoryWeight(int trandid)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[GetFactoryWeight] Looking for TRANDID: {trandid}");
                
                var calculations = db.TransactionProductCalculations
                    .Where(calc => calc.TRANDID == trandid && calc.DISPSTATUS == 0)
                    .ToList();
                
                System.Diagnostics.Debug.WriteLine($"[GetFactoryWeight] Found {calculations.Count} calculations for TRANDID {trandid}");
                
                var totalFactoryWeight = calculations.Sum(calc => calc.FACTORYWGT);
                
                System.Diagnostics.Debug.WriteLine($"[GetFactoryWeight] Total factory weight: {totalFactoryWeight}");
                
                foreach (var calc in calculations)
                {
                    System.Diagnostics.Debug.WriteLine($"[GetFactoryWeight] TRANPID: {calc.TRANPID}, PACKMID: {calc.PACKMID}, FACTORYWGT: {calc.FACTORYWGT}");
                }

                return Json(new { success = true, factoryWeight = totalFactoryWeight }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetFactoryWeight] Error: {ex.Message}");
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
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
