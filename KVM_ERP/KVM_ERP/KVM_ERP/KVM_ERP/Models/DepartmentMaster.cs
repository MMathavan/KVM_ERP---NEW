using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("DEPARTMENTMASTER")]
    public class DepartmentMaster
    {
        [Key]
        public int DEPTID { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "Please enter description")]
        [Remote("ValidateDEPTDESC", "Common", AdditionalFields = "i_DEPTDESC", ErrorMessage = "This is already used.")]
        public string DEPTDESC { get; set; }

        [DisplayName("Code")]
        [Required(ErrorMessage = "Please enter code")]
        [MaxLength(15)]
        [Remote("ValidateDEPTCODE", "Common", AdditionalFields = "i_DEPTCODE", ErrorMessage = "This is already used.")]
        public string DEPTCODE { get; set; }

        // Created user id (username)
        public string CUSRID { get; set; }

        // Last modified user id (int)
        public int LMUSRID { get; set; }

        [DisplayName("Status")]
        public short DISPSTATUS { get; set; }

        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }
    }
}
