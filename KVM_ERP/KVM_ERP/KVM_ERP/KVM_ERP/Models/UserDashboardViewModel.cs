using System;
using System.Collections.Generic;

namespace KVM_ERP.Models
{
    public class UserDashboardViewModel
    {
        public MemberShipMaster Member { get; set; }
        public MemberShipPaymentDetail LatestPayment { get; set; }
        public List<MemberShipPaymentDetail> RecentPayments { get; set; }
        public List<MemberShipFamilyDetail> Children { get; set; }
        public List<MemberShipODetail> OrganizationDetails { get; set; }
        public MemberShipTypeMaster CurrentPlan { get; set; }
        public DateTime? NextRenewalDate { get; set; }
        public int DaysToRenewal { get; set; }
        public decimal TotalPaid { get; set; }
        public int TotalPayments { get; set; }
        public DateTime MemberSince { get; set; }
        public bool IsActive { get; set; }
    }
}
