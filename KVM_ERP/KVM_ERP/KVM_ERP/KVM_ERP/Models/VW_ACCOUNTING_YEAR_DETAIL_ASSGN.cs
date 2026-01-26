using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClubMembership.Models
{
    public class VW_ACCOUNTING_YEAR_DETAIL_ASSGN
    {
        [Key]
        public int COMPYID { get; set; }
        public System.DateTime FDATE { get; set; }
        public System.DateTime TDATE { get; set; }
        public string YRDESC { get; set; }
    }
}