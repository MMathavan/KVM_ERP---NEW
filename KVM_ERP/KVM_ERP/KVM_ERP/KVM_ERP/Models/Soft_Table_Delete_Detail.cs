using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ClubMembership.Models
{
    [Table("SOFT_TABLE_DELETE_DETAIL")]
    public class Soft_Table_Delete_Detail
    {
        [Key]
        public int TABDID { get; set; }
        public string OPTNSTR { get; set; }
        public string TABNAME { get; set; }
        public string PFLDNAME { get; set; }
        public string DCONDTNSTR { get; set; }
        public string DISPDESC { get; set; }
    }
}