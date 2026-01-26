using ClubMembership.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KVM_ERP.Models
{
    public class Company
    {
        public List<KVM_ERP.Models.CompanyMaster> masterdata { get; set; }
        public List<KVM_ERP.Models.CompanyDetail> detaildata { get; set; }
        //public List<pr_CompanyDetail_Flx_Assgn_Result> queryresultdata { get; set; }
    }
}