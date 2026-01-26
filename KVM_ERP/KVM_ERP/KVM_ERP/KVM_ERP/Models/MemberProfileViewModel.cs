using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    public class MemberProfileViewModel
    {
        public MemberShipMaster Member { get; set; }
        public List<MemberShipFamilyDetail> Children { get; set; }
        public List<MemberShipODetail> OrganizationDetails { get; set; }

    }
}