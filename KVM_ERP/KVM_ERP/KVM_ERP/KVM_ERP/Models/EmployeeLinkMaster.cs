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
    [Table("EmployeeLinkMaster")]
    public class EmployeeLinkMaster
    {
        [Key]
        public int CATELID { get; set; }
        public int CATEID { get; set; }

        public int BRNCHID { get; set; }
        public string LBRNCHNAME { get; set; }
        public int LCATEID { get; set; }
    }
}