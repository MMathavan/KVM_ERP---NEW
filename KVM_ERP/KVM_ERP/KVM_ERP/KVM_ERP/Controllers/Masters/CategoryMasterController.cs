using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ClubMembership.Data;
using KVM_ERP.Models;
using System.Data.SqlClient;
using KVM_ERP;

namespace KVM_ERP.Controllers.Masters
{
	[SessionExpire]
	public class CategoryMasterController : Controller
	{
		// Note: Using ClubMembershipDBEntities to query existing table [CategoryTypeMaster]
		[Authorize(Roles = "CategoryMasterIndex")]
		public ActionResult Index()
		{
			return View();
		}

		public JsonResult GetAjaxData(JQueryDataTableParamModel param)
		{
			using (var db = new ClubMembershipDBEntities())
			{
				// Fetch up to 1000 records as per user's SQL

				var sql = @"SELECT TOP (1000)
					CateTid,
					CAST(CateTCode AS NVARCHAR(100)) AS CateTCode,
					CateTDesc,
					CAST(Dispstatus AS NVARCHAR(10)) AS Dispstatus
				FROM [CategoryTypeMaster]
				ORDER BY CateTid DESC";

				var list = db.Database.SqlQuery<CategoryTypeRow>(sql).ToList();

				// Optional client-side search filter (DataTables sends sSearch)
				var search = (param != null ? param.sSearch : null) ?? string.Empty;
				if (!string.IsNullOrWhiteSpace(search))
				{
					var term = search.Trim().ToLower();
					list = list.Where(x =>
						(x.CateTCode ?? string.Empty).ToLower().Contains(term) ||
						(x.CateTDesc ?? string.Empty).ToLower().Contains(term) ||
						(x.Dispstatus ?? string.Empty).ToLower().Contains(term)
					).ToList();
				}

				// Map to keys the existing Index view expects
				var aaData = list.Select(d => new
				{
					ACCODE = d.CateTCode,
					ACDESC = d.CateTDesc,
					DISPSTATUS = d.Dispstatus,
					ACID = d.CateTid.ToString()
				}).ToArray();

				return Json(new { data = aaData }, JsonRequestBehavior.AllowGet);
			}
		}

		[Authorize(Roles = "CategoryMasterCreate,CategoryMasterEdit")]
		public ActionResult Form(int? id)
		{
			// Populate status dropdown: 0 = Active, 1 = Inactive
			ViewBag.DISPSTATUS = new SelectList(new[]
			{
				new { Value = "0", Text = "Active" },
				new { Value = "1", Text = "Inactive" }
			}, "Value", "Text");

			var model = new CategoryMaster
			{
				DISPSTATUS = "0" // default Active
			};

			if (id.HasValue && id.Value > 0)
			{
				using (var db = new ClubMembershipDBEntities())
				{
					var sql = @"SELECT CateTid, CateTCode, CateTDesc, CAST(Dispstatus AS NVARCHAR(10)) AS Dispstatus
								FROM [CategoryTypeMaster] WHERE CateTid = @id";
					var p = new SqlParameter("@id", id.Value);
					var row = db.Database.SqlQuery<CategoryTypeRow>(sql, p).FirstOrDefault();
					if (row != null)
					{
						model.ACID = row.CateTid;
						model.ACCODE = row.CateTCode;
						model.ACDESC = row.CateTDesc;
						model.DISPSTATUS = string.IsNullOrEmpty(row.Dispstatus) ? "0" : row.Dispstatus;
					}
				}
			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "CategoryMasterCreate,CategoryMasterEdit")]
		public ActionResult SaveData(CategoryMaster model)
		{
			if (!ModelState.IsValid)
			{
				// Repopulate dropdown on validation failure
				ViewBag.DISPSTATUS = new SelectList(new[]
				{
					new { Value = "0", Text = "Active" },
					new { Value = "1", Text = "Inactive" }
				}, "Value", "Text", model.DISPSTATUS);
				return View("Form", model);
			}

			var code = (model.ACCODE ?? string.Empty).Trim();
			var desc = (model.ACDESC ?? string.Empty).Trim();
			var disp = string.IsNullOrWhiteSpace(model.DISPSTATUS) ? "0" : model.DISPSTATUS.Trim();
			var cusr = (model.CUSRID ?? string.Empty).Trim();
			var lmus = (model.LMUSRID ?? string.Empty).Trim();

			using (var db = new ClubMembershipDBEntities())
			{
				// If code is empty on create, auto-generate next numeric code padded to 3 digits
				if (model.ACID == 0 && string.IsNullOrWhiteSpace(code))
				{
					var maxSql = @"SELECT ISNULL(MAX(CAST(CateTCode AS INT)), 0) FROM [CategoryTypeMaster] WHERE ISNUMERIC(CateTCode) = 1";
					var maxVal = db.Database.SqlQuery<int>(maxSql).FirstOrDefault();
					code = (maxVal + 1).ToString("000");
				}

				// Uniqueness check for CateTCode
				if (model.ACID > 0)
				{
					var dupSql = @"SELECT COUNT(1) FROM [CategoryTypeMaster] WHERE CateTCode = @code AND CateTid <> @id";
					var dup = db.Database.SqlQuery<int>(dupSql,
						new SqlParameter("@code", code),
						new SqlParameter("@id", model.ACID)
					).FirstOrDefault();
					if (dup > 0)
					{
						ModelState.AddModelError("ACCODE", "Code already exists.");
						ViewBag.DISPSTATUS = new SelectList(new[]
						{
							new { Value = "0", Text = "Active" },
							new { Value = "1", Text = "Inactive" }
						}, "Value", "Text", model.DISPSTATUS);
						return View("Form", model);
					}
				}
				else
				{
					var dupSql = @"SELECT COUNT(1) FROM [CategoryTypeMaster] WHERE CateTCode = @code";
					var dup = db.Database.SqlQuery<int>(dupSql, new SqlParameter("@code", code)).FirstOrDefault();
					if (dup > 0)
					{
						ModelState.AddModelError("ACCODE", "Code already exists.");
						ViewBag.DISPSTATUS = new SelectList(new[]
						{
							new { Value = "0", Text = "Active" },
							new { Value = "1", Text = "Inactive" }
						}, "Value", "Text", model.DISPSTATUS);
						return View("Form", model);
					}
				}

				if (model.ACID > 0)
				{
					var usql = @"UPDATE [CategoryTypeMaster]
							SET CateTCode = @code, CateTDesc = @desc, Dispstatus = @disp, Lmusrid = @lmus
							WHERE CateTid = @id";
					var rows = db.Database.ExecuteSqlCommand(usql,
						new SqlParameter("@code", code),
						new SqlParameter("@desc", desc),
						new SqlParameter("@disp", disp),
						new SqlParameter("@lmus", (object)lmus ?? DBNull.Value),
						new SqlParameter("@id", model.ACID)
					);
					ViewBag.msg = "<div class='alert alert-success msg'>Updated successfully.</div>";
				}
				else
				{
					var isql = @"INSERT INTO [CategoryTypeMaster] (CateTCode, CateTDesc, Cusrid, Lmusrid, Dispstatus, pscrdate)
							VALUES (@code, @desc, @cusr, @lmus, @disp, GETDATE())";
					db.Database.ExecuteSqlCommand(isql,
						new SqlParameter("@code", code),
						new SqlParameter("@desc", desc),
						new SqlParameter("@cusr", (object)cusr ?? DBNull.Value),
						new SqlParameter("@lmus", (object)lmus ?? DBNull.Value),
						new SqlParameter("@disp", disp)
					);
					// On successful insert, go back to list
				}
			}

			return RedirectToAction("Index");
		}

		[Authorize(Roles = "CategoryMasterDelete")]
		public void Del()
		{
			var idStr = Request.Form.Get("id");
			int id;
			if (!int.TryParse(idStr, out id))
			{
				Response.Write("Invalid id");
				return;
			}

			using (var db = new ClubMembershipDBEntities())
			{
				var sql = "DELETE FROM [CategoryTypeMaster] WHERE CateTid = @id";
				var p = new SqlParameter("@id", id);
				var rows = db.Database.ExecuteSqlCommand(sql, p);
				if (rows > 0)
					Response.Write("Deleted Successfully ...");
				else
					Response.Write("Record not found");
			}
		}

		private class CategoryTypeRow
		{
			public int CateTid { get; set; }
			public string CateTCode { get; set; }
			public string CateTDesc { get; set; }
			public string Dispstatus { get; set; }
		}
	}
}
