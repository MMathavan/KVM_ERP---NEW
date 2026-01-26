using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    public class RenewalPopupViewModel
    {
        public int MemberID { get; set; }
        public string MemberName { get; set; }
        public int MemberNumber { get; set; }
        public string CurrentPlanName { get; set; }
        public DateTime? CurrentRenewalDate { get; set; }
        public List<SelectListItem> Plans { get; set; }
    }
}
