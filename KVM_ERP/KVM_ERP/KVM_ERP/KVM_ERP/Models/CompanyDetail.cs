using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace KVM_ERP.Models
{
    [Table("COMPANYDETAIL")]
    public class CompanyDetail
    {
        [Key]
        public int COMPDID { get; set; }
        public int COMPID { get; set; }
        public string COMPPONAME { get; set; }
        public string COMPPOADDR { get; set; }
    }
}