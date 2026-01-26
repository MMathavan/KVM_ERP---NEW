using KVM_ERP.Models;
using ClubMembership.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;

namespace KVM_ERP.Controllers.Masters
{
    public class CommonController : Controller
    {
        // GET: /Common/
        //FusionContext context = new FusionContext();
        ApplicationDbContext context = new ApplicationDbContext();


        [AllowAnonymous]


        //----------------------Department----------------------------
        public JsonResult ValidateDEPTDESC(String DEPTDESC, String i_DEPTDESC)
        {
            if (DEPTDESC.Equals(i_DEPTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select DEPTDESC from departmentmaster").ToList();
            if (d.Contains(DEPTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateDEPTCODE(String DEPTCODE, String i_DEPTCODE)
        {
            if (DEPTCODE.Equals(i_DEPTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select DEPTCODE from departmentmaster").ToList();
            if (d.Contains(DEPTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------



        //STAR
        public JsonResult ValidateLANGDESC(String LANGDESC, String i_LANGDESC)
        {
            LANGDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(LANGDESC.ToLower());
            i_LANGDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_LANGDESC.ToLower());

            if (LANGDESC.Equals(i_LANGDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select LANGDESC from LANGUAGEMASTER").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(LANGDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateLANGCODE(String LANGCODE, String i_LANGCODE)
        {
            if (LANGCODE.Equals(i_LANGCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select LANGCODE from LANGUAGEMASTER").ToList();
            if (d.Contains(LANGCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //
        public JsonResult ValidateACDESC(String ACDESC, String i_ACDESC)
        {
            ACDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(ACDESC.ToLower());
            i_ACDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_ACDESC.ToLower());

            if (ACDESC.Equals(i_ACDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select ACDESC from CATEGORYMASTER").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(ACDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateACCODE(String ACCODE, String i_ACCODE)
        {
            if (ACCODE.Equals(i_ACCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select ACCODE from CATEGORYMASTER").ToList();
            if (d.Contains(ACCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        //
        //model start
        public JsonResult ValidateEDTNDESC(String EDTNDESC, String i_EDTNDESC)
        {
            EDTNDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(EDTNDESC.ToLower());
            i_EDTNDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_EDTNDESC.ToLower());

            if (EDTNDESC.Equals(i_EDTNDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select EDTNDESC from EDITIONMASTER").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(EDTNDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateEDTNCODE(String EDTNCODE, String i_EDTNCODE)
        {
            if (EDTNCODE.Equals(i_EDTNCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select EDTNCODE from EDITIONMASTER").ToList();
            if (d.Contains(EDTNCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        //model end

        //BRAND START
        public JsonResult ValidatePOSTNDESC(String POSTNDESC, String i_POSTNDESC)
        {
            POSTNDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(POSTNDESC.ToLower());
            i_POSTNDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_POSTNDESC.ToLower());

            if (POSTNDESC.Equals(i_POSTNDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select POSTNDESC from POSITIONMASTER").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(POSTNDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidatePOSTNCODE(String POSTNCODE, String i_POSTNCODE)
        {
            if (POSTNCODE.Equals(i_POSTNCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select POSTNCODE from POSITIONMASTER").ToList();
            if (d.Contains(POSTNCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        //BRAND END
        //start
        public JsonResult ValidateMUNITDESC(String MUNITDESC, String i_MUNITDESC)
        {
            MUNITDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(MUNITDESC.ToLower());
            i_MUNITDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_MUNITDESC.ToLower());

            if (MUNITDESC.Equals(i_MUNITDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select MUNITDESC from MEDIAUNITMASTER").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(MUNITDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateMUNITCODE(String MUNITCODE, String i_MUNITCODE)
        {
            if (MUNITCODE.Equals(i_MUNITCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select MUNITCODE from MEDIAUNITMASTER").ToList();
            if (d.Contains(MUNITCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //end

        public JsonResult ValidateACHEADGDESC(String ACHEADGDESC, String i_ACHEADGDESC)
        {
            ACHEADGDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(ACHEADGDESC.ToLower());
            i_ACHEADGDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_ACHEADGDESC.ToLower());

            if (ACHEADGDESC.Equals(i_ACHEADGDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select ACHEADGDESC from accountgroupmaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(ACHEADGDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateACHEADGCODE(String ACHEADGCODE, String i_ACHEADGCODE)
        {
            if (ACHEADGCODE.Equals(i_ACHEADGCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select ACHEADGCODE from accountgroupmaster").ToList();
            if (d.Contains(ACHEADGCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //START
        public JsonResult ValidatePUBLNTDESC(String PUBLNTDESC, String i_PUBLNTDESC)
        {
            PUBLNTDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(PUBLNTDESC.ToLower());
            i_PUBLNTDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_PUBLNTDESC.ToLower());

            if (PUBLNTDESC.Equals(i_PUBLNTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select PUBLNTDESC from PUBLICATIONTYPEMASTER").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(PUBLNTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidatePUBLNTCODE(String PUBLNTCODE, String i_PUBLNTCODE)
        {
            if (PUBLNTCODE.Equals(i_PUBLNTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select PROCESSCODE from PUBLICATIONTYPEMASTER").ToList();
            if (d.Contains(PUBLNTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        //END

        public JsonResult ValidateACHEADDESC(String ACHEADDESC, String i_ACHEADDESC)
        {
            ACHEADDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(ACHEADDESC.ToLower());
            i_ACHEADDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_ACHEADDESC.ToLower());

            if (ACHEADDESC.Equals(i_ACHEADDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select ACHEADDESC from accountheadmaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(ACHEADDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateACHEADCODE(String ACHEADCODE, String i_ACHEADCODE)
        {
            if (ACHEADCODE.Equals(i_ACHEADCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select ACHEADCODE from accountheadmaster").ToList();
            if (d.Contains(ACHEADCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //--------------------------------------------------------------------------

        public JsonResult ValidateBRNCHNAME(String BRNCHNAME, String i_BRNCHNAME)
        {
            if (BRNCHNAME.Equals(i_BRNCHNAME))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select BRNCHNAME from branchmaster").ToList();
            if (d.Contains(BRNCHNAME))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateBRNCHCODE(String BRNCHCODE, String i_BRNCHCODE)
        {
            BRNCHCODE = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(BRNCHCODE.ToLower());
            i_BRNCHCODE = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_BRNCHCODE.ToLower());

            if (BRNCHCODE.Equals(i_BRNCHCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select BRNCHCODE from branchmaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(BRNCHCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateCLNTNAME(String CLNTNAME, String i_CLNTNAME)
        {
            CLNTNAME = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(CLNTNAME.ToLower());
            i_CLNTNAME = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_CLNTNAME.ToLower());

            if (CLNTNAME.Equals(i_CLNTNAME))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CLNTNAME from CLIENTMASTER").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(CLNTNAME))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateCLNTCODE(String CLNTCODE, String i_CLNTCODE)
        {
            if (CLNTCODE.Equals(i_CLNTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CLNTCODE from CLIENTMASTER").ToList();
            if (d.Contains(CLNTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //
        public JsonResult ValidateSUPLRNAME(String SUPLRNAME, String i_SUPLRNAME)
        {
            SUPLRNAME = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(SUPLRNAME.ToLower());
            i_SUPLRNAME = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_SUPLRNAME.ToLower());

            if (SUPLRNAME.Equals(i_SUPLRNAME))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CLNTNAME from SUPPLIERMASTER").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(SUPLRNAME))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateSUPLRCODE(String SUPLRCODE, String i_SUPLRCODE)
        {
            if (SUPLRCODE.Equals(i_SUPLRCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select CLNTCODE from SUPPLIERMASTER").ToList();
            if (d.Contains(SUPLRCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //
        public JsonResult ValidateSCCODE(String SCCODE, String i_SCCODE)
        {
            if (SCCODE.Equals(i_SCCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select SCCODE from servicecategorymaster").ToList();
            if (d.Contains(SCCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateSCDESC(String SCDESC, String i_SCDESC)
        {
            SCDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(SCDESC.ToLower());
            i_SCDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_SCDESC.ToLower());

            if (SCDESC.Equals(i_SCDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select SCDESC from servicecategorymaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(SCDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        //STATE
        public JsonResult ValidateSTATECODE(String STATECODE, String i_STATECODE)
        {
            if (STATECODE.Equals(i_STATECODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select STATECODE from statemaster").ToList();
            if (d.Contains(STATECODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateSTATEDESC(String STATEDESC, String i_STATEDESC)
        {
            STATEDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(STATEDESC.ToLower());
            i_STATEDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_STATEDESC.ToLower());

            if (STATEDESC.Equals(i_STATEDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select SCDESC from statemaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(STATEDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //STATE

        //BLOOD
        public JsonResult ValidateBLDGCODE(String BLDGCODE, String i_BLDGCODE)
        {
            if (BLDGCODE.Equals(i_BLDGCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select BLDGCODE from bloodgroupmaster").ToList();
            if (d.Contains(BLDGCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateBLDGDESC(String BLDGDESC, String i_BLDGDESC)
        {
            BLDGDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(BLDGDESC.ToLower());
            i_BLDGDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_BLDGDESC.ToLower());

            if (BLDGDESC.Equals(i_BLDGDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select BLDGDESC from bloodgroupmaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(BLDGDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //BLOOD

        //HSN
        public JsonResult ValidateHSNCODE(String HSNCODE, String i_HSNCODE)
        {
            if (HSNCODE.Equals(i_HSNCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select HSNCODE from HSNCodeMaster").ToList();
            if (d.Contains(HSNCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateHSNDESC(String HSNDESC, String i_HSNDESC)
        {
            HSNDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(HSNDESC.ToLower());
            i_HSNDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_HSNDESC.ToLower());

            if (HSNDESC.Equals(i_HSNDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select HSNDESC from HSNCodeMaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(HSNDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //HSN


        //START

        public JsonResult ValidateSMCODE(String SMCODE, String i_SMCODE)
        {
            if (SMCODE.Equals(i_SMCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select SMCODE from salesmanmaster").ToList();
            if (d.Contains(SMCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateSMNAME(String SMNAME, String i_SMNAME)
        {
            SMNAME = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(SMNAME.ToLower());
            i_SMNAME = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_SMNAME.ToLower());

            if (SMNAME.Equals(i_SMNAME))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select SMNAME from salesmanmaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(SMNAME))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }


        //END


        public JsonResult ValidateCURNDESC(String CURNDESC, String i_CURNDESC)
        {
            CURNDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(CURNDESC.ToLower());
            i_CURNDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_CURNDESC.ToLower());

            if (CURNDESC.Equals(i_CURNDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select CURNDESC from currencymaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(CURNDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateCURNCODE(String CURNCODE, String i_CURNCODE)
        {
            if (CURNCODE.Equals(i_CURNCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select CURNCODE from currencymaster").ToList();
            if (d.Contains(CURNCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        //

        public JsonResult ValidateOFTDESC(String OFTDESC, String i_OFTDESC)
        {
            OFTDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(OFTDESC.ToLower());
            i_OFTDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_OFTDESC.ToLower());

            if (OFTDESC.Equals(i_OFTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select OFTDESC from officetypemaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(OFTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateOFTCODE(String OFTCODE, String i_OFTCODE)
        {
            if (OFTCODE.Equals(i_OFTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select OFTCODE from officetypemaster").ToList();
            if (d.Contains(OFTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }



        //business center
        public JsonResult ValidateBCMNAME(String BCMNAME, String i_BCMNAME)
        {
            BCMNAME = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(BCMNAME.ToLower());
            i_BCMNAME = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_BCMNAME.ToLower());

            if (BCMNAME.Equals(i_BCMNAME))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select BCMNAME from businesscentermaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(BCMNAME))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateBCMCODE(String BCMCODE, String i_BCMCODE)
        {
            if (BCMCODE.Equals(i_BCMCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select BCMCODE from businesscentermaster").ToList();
            if (d.Contains(BCMCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        //-------------End----------------


        public JsonResult ValidateITEMGDESC(String ITEMGDESC, String i_ITEMGDESC)
        {
            ITEMGDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(ITEMGDESC.ToLower());
            i_ITEMGDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_ITEMGDESC.ToLower());

            if (ITEMGDESC.Equals(i_ITEMGDESC))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select ITEMGDESC from itemgroupmaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(ITEMGDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateITEMGCODE(String ITEMGCODE, String i_ITEMGCODE)
        {
            if (ITEMGCODE.Equals(i_ITEMGCODE))
                return Json(true, JsonRequestBehavior.AllowGet);

            List<String> d = context.Database.SqlQuery<String>("select ITEMGCODE from itemgroupmaster").ToList();
            if (d.Contains(ITEMGCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------


        public JsonResult ValidateCFDESC(String CFDESC, String i_CFDESC)
        {
            CFDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(CFDESC.ToLower());
            i_CFDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_CFDESC.ToLower());

            if (CFDESC.Equals(i_CFDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select CFDESC from costfactormaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(CFDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        public JsonResult ValidatePROCESSMDESC(String PROCESSMDESC, String i_PROCESSMDESC)
        {
            PROCESSMDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(PROCESSMDESC.ToLower());
            i_PROCESSMDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_PROCESSMDESC.ToLower());

            if (PROCESSMDESC.Equals(i_PROCESSMDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select PROCESSMDESC from processmaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(PROCESSMDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidatePROCESSMCODE(String PROCESSMCODE, String i_PROCESSMCODE)
        {
            if (PROCESSMCODE.Equals(i_PROCESSMCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select PROCESSMCODE from processmaster").ToList();
            if (d.Contains(PROCESSMCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //-------------End----------------
        public JsonResult ValidatePROCESSDDESC(String PROCESSDDESC, String i_PROCESSDDESC)
        {
            PROCESSDDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(PROCESSDDESC.ToLower());
            i_PROCESSDDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_PROCESSDDESC.ToLower());

            if (PROCESSDDESC.Equals(i_PROCESSDDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select PROCESSDDESC from subprocessmaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(PROCESSDDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidatePROCESSDCODE(String PROCESSDCODE, String i_PROCESSDCODE)
        {
            if (PROCESSDCODE.Equals(i_PROCESSDCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select PROCESSDCODE from subprocessmaster").ToList();
            if (d.Contains(PROCESSDCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        public JsonResult ValidateUNITDESC(String UNITDESC, String i_UNITDESC)
        {
            UNITDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(UNITDESC.ToLower());
            i_UNITDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_UNITDESC.ToLower());

            if (UNITDESC.Equals(i_UNITDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select UNITDESC from unitmaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(UNITDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateUNITCODE(String UNITCODE, String i_UNITCODE)
        {
            if (UNITCODE.Equals(i_UNITCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select UNITCODE from unitmaster").ToList();
            if (d.Contains(UNITCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        public JsonResult ValidateLOCTDESC(String LOCTDESC, String i_LOCTDESC)
        {
            LOCTDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(LOCTDESC.ToLower());
            i_LOCTDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_LOCTDESC.ToLower());


            if (LOCTDESC.Equals(i_LOCTDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select LOCTDESC from locationmaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(LOCTDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateLOCTCODE(String LOCTCODE, String i_LOCTCODE)
        {
            if (LOCTCODE.Equals(i_LOCTCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select LOCTCODE from locationmaster").ToList();
            if (d.Contains(LOCTCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------

        //REGION
        public JsonResult ValidateREGNDESC(String REGNDESC, String i_REGNDESC)
        {
            REGNDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(REGNDESC.ToLower());
            i_REGNDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_REGNDESC.ToLower());


            if (REGNDESC.Equals(i_REGNDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select REGNDESC from regionmaster").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(REGNDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ValidateREGNCODE(String REGNCODE, String i_REGNCODE)
        {
            if (REGNCODE.Equals(i_REGNCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select REGNCODE from regionmaster").ToList();
            if (d.Contains(REGNCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        // REGION END
        public JsonResult ValidateCATEVCODE(String CATEVCODE, String i_CATEVCODE)
        {
            if (CATEVCODE.Equals(i_CATEVCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select CATEVCODE from categorymaster").ToList();
            if (d.Contains(CATEVCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        //-------------End----------------
        //...ITEM DESCription
        public JsonResult ValidateITEMDESC([Bind(Prefix = "VW_ITEMMASTER_CTRL_ASSGN.ITEMDESC")]String ITEMDESC, String i_ITEMDESC)
        {
            if (ITEMDESC.Equals(i_ITEMDESC))
                return Json("ed", JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select ITEMDESC from itemmaster").ToList();
            if (d.Contains(ITEMDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json("in", JsonRequestBehavior.AllowGet);
        }//...end


        public JsonResult DupCheck(String data)
        {

            List<String> d = context.Database.SqlQuery<String>("select ITEMDESC from itemmaster").ToList();
            if (d.Contains(data))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }//...end

        public JsonResult CustomerDupCheck(String data)
        {

            List<String> d = context.Database.SqlQuery<String>("select CATENAME from customermaster").ToList();
            if (d.Contains(data))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }//...end


        //...........transaction item group and item\
        //..........material group
        //public JsonResult AutoMatGrp(string term)
        //{
        //    var result = (from r in context.itemgroupmasters
        //                  where r.ITEMGDESC.ToLower().StartsWith(term.ToLower())
        //                  //.Contains(term.ToLower())
        //                  select new { r.ITEMGDESC, r.ITEMGID }).OrderBy(x => x.ITEMGDESC).Distinct();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}//...end

        //public JsonResult AutoItem(string term)
        //{
        //    var ids = term.Split(';');
        //    var s_term = ids[0];
        //    var gid = Convert.ToInt32(ids[1]);

        //    var result = (from r in context.ItemMaster.Where(x => x.ITEMGID == gid)
        //                      // where (r.ITEMDESC.ToLower().StartsWith(s_term.ToLower()) || r.ITEMCODE.ToLower().StartsWith(s_term.ToLower()))
        //                  where (r.ITEMDESC.ToLower().Contains(s_term.ToLower()) || r.ITEMCODE.ToLower().Contains(s_term.ToLower()))
        //                  select new { r.ITEMDESC, r.ITEMID, r.ITEMCODE, r.BUNITID, r.ITEMNO }).OrderBy(x => x.ITEMDESC).Distinct();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}//......end  

        //public JsonResult AutoItemTax(string term)
        //{
        //    var ids = term.Split(';');
        //    var s_term = ids[0];
        //    var gid = Convert.ToInt32(ids[1]);

        //    var result = (from r in context.VW_ITEM_GRID_TAX_DETAIL_ASSGN.Where(x => x.ITEMGID == gid)
        //                      // where (r.ITEMDESC.ToLower().StartsWith(s_term.ToLower()) || r.ITEMCODE.ToLower().StartsWith(s_term.ToLower()))
        //                  where (r.ITEMDESC.ToLower().Contains(s_term.ToLower()) || r.ITEMCODE.ToLower().Contains(s_term.ToLower()))
        //                  select new { r.ITEMDESC, r.ITEMID, r.ITEMCODE, r.BUNITID, r.CGSTEXPRN, r.SGSTEXPRN, r.IGSTEXPRN }).OrderBy(x => x.ITEMDESC).Distinct();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}//......end  

        //..........Location dropdown
        //public JsonResult GetAgreementNo(int id)
        //{
        //    var result = context.Database.SqlQuery<AgreementMaster>("select * from AgreementMaster where AGMREFID=" + id);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}//...end

        //public JsonResult CostFactorDetail(int id)
        //{
        //    var result = (from r in context.CostFactorMaster
        //                  where r.CFID.Equals(id)
        //                  select new { r.CFID, r.CFMODE, r.CFEXPR, r.CFTYPE, r.DORDRID }).Distinct();


        //    return Json(result, JsonRequestBehavior.AllowGet);

        //}


        //..........Address
        //public JsonResult GetAddress(int id)
        //{
        //    var result = context.Database.SqlQuery<CategoryAddressDetail>("select * from CategoryAddressDetail where CATEAID=" + id);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}//...end
        //..........DeliveryAddress
        //public JsonResult GetDeliveryAddress(string id)
        //{
        //    var cid = id.Split('-');
        //    var LOCTNID = Convert.ToInt32(cid[0]);
        //    var TRANREFID = Convert.ToInt32(cid[1]);
        //    var result = context.Database.SqlQuery<CategoryAddressDetail>("select * from CategoryAddressDetail where LOCTNID=" + LOCTNID + " and CATEID=" + TRANREFID);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}//...end

        public void DuplicateCheck(string id)//......Duplicate Check for batch/serial No
        {
            var ids = id.Split('-');
            var s_term = ids[0];
            var gid = Convert.ToInt32(ids[1]);
            s_term = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s_term.ToLower());
            List<String> d = context.Database.SqlQuery<String>("select TRANDPDESC from TransactionDetailProductserial inner join TRANSACTIONDETAIL on TRANSACTIONDETAILPRODUCTSERIAL.TRANDID=TRANSACTIONDETAIL.TRANDID inner join ITEMMASTER on TRANSACTIONDETAIL.TRANDREFID=ITEMMASTER.ITEMID where ITEMID=" + gid + " ").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(s_term))
                Response.Write("Exists");
        }
        public void DuplicateCheckSER(string id)//......Duplicate Check for batch/serial No SERVICE MODULE
        {
            var ids = id.Split(';');
            var s_term = ids[0];
            var gid = Convert.ToInt32(ids[1]);
            s_term = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s_term.ToLower());
            List<String> d = context.Database.SqlQuery<String>("select TRANDPDESC from TransactionDetailProductserial inner join TRANSACTIONDETAIL on TRANSACTIONDETAILPRODUCTSERIAL.TRANDID=TRANSACTIONDETAIL.TRANDID inner join ITEMMASTER on TRANSACTIONDETAIL.TRANDREFID=ITEMMASTER.ITEMID inner join TRANSACTIONMASTER on TRANSACTIONMASTER.TRANMID=TRANSACTIONDETAIL.TRANMID where REGSTRID=41 and ITEMID=" + gid + " ").ToList();
            if (d.Contains(s_term))
            {
                d = context.Database.SqlQuery<String>("select TRANDPDESC from TransactionDetailProductserial inner join TRANSACTIONDETAIL on TRANSACTIONDETAILPRODUCTSERIAL.TRANDID=TRANSACTIONDETAIL.TRANDID inner join ITEMMASTER on TRANSACTIONDETAIL.TRANDREFID=ITEMMASTER.ITEMID inner join TRANSACTIONMASTER on TRANSACTIONMASTER.TRANMID=TRANSACTIONDETAIL.TRANMID where REGSTRID=43 and ITEMID=" + gid + " ").ToList();
                if (d.Contains(s_term)) Response.Write("");
                else Response.Write("Exists");
            }
            else Response.Write("");
            /*if (d.Count > 0)
            {
                d = context.Database.SqlQuery<String>("select TRANDPDESC from TransactionDetailProductserial inner join TRANSACTIONDETAIL on TRANSACTIONDETAILPRODUCTSERIAL.TRANDID=TRANSACTIONDETAIL.TRANDID inner join ITEMMASTER on TRANSACTIONDETAIL.TRANDREFID=ITEMMASTER.ITEMID inner join TRANSACTIONMASTER on TRANSACTIONMASTER.TRANMID=TRANSACTIONDETAIL.TRANMID where REGSTRID=43 and ITEMID=" + gid + " ").ToList();
                if (d.Count() == 0)
                {
                    Response.Write("Exists");
                }

            }
            else
            {


                d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
                if (d.Contains(s_term))
                    Response.Write("Exists");
            }*/
        }
        //public void DuplicateCheckserial(string id)//......Duplicate Check for batch/serial No
        //{
        //    var ids = id.Split('-');
        //    var s_term = ids[0];
        //    var gid = Convert.ToInt32(ids[1]);
        //    var count = 0;
        //    s_term = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s_term.ToLower());
        //    var data = context.Database.SqlQuery<TransactionProductSerial>("select * from TransactionDetailProductserial inner join TRANSACTIONDETAIL on TRANSACTIONDETAILPRODUCTSERIAL.TRANDID=TRANSACTIONDETAIL.TRANDID inner join ITEMMASTER on TRANSACTIONDETAIL.TRANDREFID=ITEMMASTER.ITEMID where ITEMID=" + gid + " ").ToList();
        //    foreach (var val in data)
        //    {
        //        var dpdesc = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(val.TRANDPDESC.ToLower());
        //        var serial = val.TRANDPSNO.Substring(3, 4);

        //        if (dpdesc == s_term)
        //        {
        //            if (Convert.ToInt32(serial) == Convert.ToInt32(ids[2]))
        //            { count++; }
        //        }
        //    }

        //    //   d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
        //    //  if (d.Contains(s_term))
        //    Response.Write(count);
        //}

        //.........salesorder and invoice
        //.............tariff head
        public JsonResult TariffNo(int id)
        {
            var Query = context.Database.SqlQuery<string>("select  ITEMGTHDESC AS ITEMGTHDESC  from ItemGroupMaster where ITEMGID=" + id).ToList();

            return Json(Query, JsonRequestBehavior.AllowGet);
        }//..end
         //.............Customer partnum
        public JsonResult PartNo(string id)
        {
            var cid = id.Split('-');
            var Query = context.Database.SqlQuery<string>("select  CPARTNO AS CPARTNO from ItemSalesDetail where ITEMID=" + Convert.ToInt32(cid[0]) + "and CUSTID=" + Convert.ToInt32(cid[1])).ToList();

            return Json(Query, JsonRequestBehavior.AllowGet);
        }//..end

        //........terms detail
        //public string GetTermsDetail(int id)
        //{

        //    string query = "select * from Terms_Condition_Detail  where TERMMID =" + id;
        //    IEnumerable<Terms_Condition_Detail> data = context.Database.SqlQuery<Terms_Condition_Detail>(query);
        //    int count = 0;

        //    string html = "";

        //    foreach (var result in data)
        //    {
        //        //  count = 
        //        html = html + "<Tr class='item-row'><td class='col-sm-1'><input type='text' value=" + (++count) + " style='border:none' class='SCOUNT' id='SCOUNT' name='SCOUNT' /></td><td class='hidden'><input type='text' class='form-control hide TRANTERMID' id='TRANTERMID' name='TRANTERMID' value='0'></td>";

        //        html = html + "<td><div class='col-md-12'><input class='form-control TRANTERMDESC' value='" + result.TERMDDESC + "' id='TRANTERMDESC' name='TRANTERMDESC' tabindex='2000'></div> </td><td><a class='btn btn-danger btn-sm' id='del_detail_tc'> <i class='fa fa-trash-o'></i></a></td></tr>";


        //    }
        //    return html;
        //    //end
        //}

        //...............GET REF NO AND DATE
        //public JsonResult GetRefNo(int id) //......group 
        //{
        //    var process = context.Database.SqlQuery<TransactionMaster>("select * from TransactionMaster where TRANMID=" + id + "");
        //    return Json(process, JsonRequestBehavior.AllowGet);
        //}//.End

        //...........server datetime
        public void GetDateTime()
        {
            string URL = "http://123.176.34.116";
            System.Net.HttpWebRequest rq2 = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
            System.Net.HttpWebResponse res2 = (System.Net.HttpWebResponse)rq2.GetResponse();
            DateTime Date = DateTime.Parse(res2.Headers["Date"]);
            Response.Write(Date);
        }


        //......................................Amend no generation

        public void SetAmend(int id)
        {
            var temp = "1";
            using (var context = new ApplicationDbContext())
            {

                var autonum = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(TRANANO)as TRANANO from TransactionMaster where TRANANO<>0 and COMPYID=" + System.Web.HttpContext.Current.Session["compyid"] + " and TRANMID=" + id).ToList();
                if (autonum[0] != null)
                {
                    temp = (autonum[0] + 1).ToString();
                }

            }
            Response.Write(temp);
        }


        public void BOMAmend(int id)
        {
            var temp = "1";
            using (var context = new ApplicationDbContext())
            {

                var autonum = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(ITEMANO)as ITEMANO from ItemMaster where ITEMANO<>0 and ITEMID=" + id).ToList();
                if (autonum[0] != null)
                {
                    temp = (autonum[0] + 1).ToString();
                }

            }
            Response.Write(temp);
        }
        public void QAmend(int id)
        {
            var temp = "1";
            using (var context = new ApplicationDbContext())
            {

                var autonum = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(ITEMQNO)as ITEMQNO from ItemMaster where ITEMQNO<>0 and ITEMID=" + id).ToList();
                if (autonum[0] != null)
                {
                    temp = (autonum[0] + 1).ToString();
                }

            }
            Response.Write(temp);
        }
        //public void DrawAmend(int id)
        //{
        //    var temp = "1";
        //    using (var context = new ApplicationDbContext())
        //    {

        //        var autonum = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(ITEMDNO)as ITEMDNO from ItemMaster where ITEMDNO<>0 and ITEMID=" + id).ToList();
        //        if (autonum[0] != null)
        //        {
        //            temp = (autonum[0] + 1).ToString();
        //        }

        //    }
        //    Response.Write(temp);
        //}

        //public void AttrAmend(int id)
        //{
        //    var temp = "1";
        //    using (var context = new ApplicationDbContext())
        //    {

        //        var autonum = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(ITEMATNO)as ITEMATNO from ItemMaster where ITEMATNO<>0 and ITEMID=" + id).ToList();
        //        if (autonum[0] != null)
        //        {
        //            temp = (autonum[0] + 1).ToString();
        //        }

        //    }
        //    Response.Write(temp);
        //}

        //..........Address


        public JsonResult GetBranchList()
        {
            int emplid = 0;
            emplid = Convert.ToInt32(Session["EMPLID"]);
            var result = (from r in context.employeelinkmasters.Where(K => K.CATEID == emplid).OrderBy(x => x.LBRNCHNAME)
                          select new { r.BRNCHID, r.LBRNCHNAME }).Distinct();
            //var result = (from r in context.branchmasters.Where(K => K.DISPSTATUS == 0 && K.BRNCHID > 1).OrderBy(x => x.BRNCHNAME)
            //              select new { r.BRNCHID, r.BRNCHNAME }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }//...end
        //public JsonResult GetLocationList()
        //{
        //    int emplid  = Convert.ToInt32(Session["EMPLID"]);

        //    var result = (from r in context.locationmasters.Where(K => K.DISPSTATUS == 0 && K.LOCTID > 1).OrderBy(x => x.LOCTDESC)
        //                  join n in context.employeelinkmasters.Where(a =>a.CATEID == emplid) on r.BRNCHID equals n.BRNCHID 
        //                  select new { r.LOCTID, r.LOCTDESC }).Distinct();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}//...end

        //public JsonResult GetCustomerList()
        //{
        //    var result = (from r in context.customermasters.Where(K => K.DISPSTATUS == 0 && K.CATEID > 1).OrderBy(x => x.CATENAME)
        //                  select new { r.CATEID, r.CATENAME }).Distinct();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}//...end


        //end class
        // DESIGNATION
        public JsonResult ValidateDSGNCODE(String DSGNCODE, String i_DSGNCODE)
        {
            if (DSGNCODE.Equals(i_DSGNCODE))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select DSGNCODE from DESIGNATIONMASTER").ToList();
            if (d.Contains(DSGNCODE))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateDSGNDESC(String DSGNDESC, String i_DSGNDESC)
        {
            DSGNDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(DSGNDESC.ToLower());
            i_DSGNDESC = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(i_DSGNDESC.ToLower());

            if (DSGNDESC.Equals(i_DSGNDESC))
                return Json(true, JsonRequestBehavior.AllowGet);
            List<String> d = context.Database.SqlQuery<String>("select DSGNDESC from DESIGNATIONMASTER").ToList();
            d = d.Select(r => System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())).ToList();
            if (d.Contains(DSGNDESC))
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
        // DESIGNATION
    }
}