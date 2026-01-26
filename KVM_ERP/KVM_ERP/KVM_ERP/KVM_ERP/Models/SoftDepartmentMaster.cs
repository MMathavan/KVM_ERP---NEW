using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ClubMembership.Models
{
    [Table("SOFT_DEPARTMENTMASTER")]
    public class SoftDepartmentMaster
    {
        [Key]
        public int SDPTID { get; set; }
        [DisplayName("Name")]
        [Required(ErrorMessage = "field is required")]
        public string SDPTNAME { get; set; }
        [DisplayName("Option String")]
        [Required(ErrorMessage = "field is required")]
        public string OPTNSTR { get; set; }
        [DisplayName("Status")]
        [Required(ErrorMessage = "field is required")]
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}