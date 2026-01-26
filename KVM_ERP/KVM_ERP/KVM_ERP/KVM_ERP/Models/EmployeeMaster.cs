using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace KVM_ERP.Models
{
    [Table("EMPLOYEEMASTER")]
    public class EmployeeMaster
    {
        [Key]
        public int CATEID { get; set; }

        [DisplayName("Employee Type")]
        public int CATETID { get; set; } = 1; // Default value 1

        [DisplayName("Employee Name")]
        [Required(ErrorMessage = "Please enter employee name")]
        [Remote("ValidateCATENAME", "Common", AdditionalFields = "i_CATENAME", ErrorMessage = "This is already used.")]
        public string CATENAME { get; set; }

        [DisplayName("Address Line 1")]
        public string CATEADDR1 { get; set; }

        [DisplayName("Address Line 2")]
        public string CATEADDR2 { get; set; }

        [DisplayName("Address Line 3")]
        public string CATEADDR3 { get; set; }

        [DisplayName("Address Line 4")]
        public string CATEADDR4 { get; set; }

        [DisplayName("Phone 1")]
        [MaxLength(25)]
        public string CATEPHN1 { get; set; }

        [DisplayName("Phone 2")]
        [MaxLength(25)]
        public string CATEPHN2 { get; set; }

        [DisplayName("Phone 3")]
        [MaxLength(25)]
        public string CATEPHN3 { get; set; }

        [DisplayName("Phone 4")]
        [MaxLength(25)]
        public string CATEPHN4 { get; set; }

        [DisplayName("Contact Person")]
        [MaxLength(100)]
        public string CATECPNAME { get; set; }

        [DisplayName("Email")]
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string CATEEMAIL { get; set; }

        [DisplayName("Department")]
        [Required(ErrorMessage = "Please select department")]
        public int DEPTID { get; set; }

        [DisplayName("Designation")]
        [Required(ErrorMessage = "Please select designation")]
        public int DSGNID { get; set; }

        [DisplayName("Location")]
        public int? LOCTID { get; set; }

        [DisplayName("Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? CATEDOB { get; set; }

        [DisplayName("Date of Joining")]
        [DataType(DataType.Date)]
        public DateTime? CATEDOJ { get; set; }

        [DisplayName("Date of Confirmation")]
        [DataType(DataType.Date)]
        public DateTime? CATEDOC { get; set; }

        [DisplayName("Date of Retirement")]
        [DataType(DataType.Date)]
        public DateTime? CATEDOR { get; set; }

        [DisplayName("Employee Status")]
        public short CATESTATUS { get; set; }

        [DisplayName("Employee Code")]
        [Required(ErrorMessage = "Please enter employee code")]
        [MaxLength(15)]
        [Remote("ValidateCATECODE", "Common", AdditionalFields = "i_CATECODE", ErrorMessage = "This is already used.")]
        public string CATECODE { get; set; }

        // Created user id (username)
        public string CUSRID { get; set; }

        // Last modified user id (int)
        public int LMUSRID { get; set; }

        [DisplayName("Status")]
        public short DISPSTATUS { get; set; }

        [DataType(DataType.Date)]
        public DateTime PRCSDATE { get; set; }

        [DisplayName("Region")]
        public int REGNID { get; set; } = 1; // Default value 1

        [DisplayName("Employee Grade")]
        public int? EMPGRD { get; set; } // Default NULL

        [DisplayName("Annual Incentive CTC")]
        public decimal? ANNUAL_INCENTIVE_CTC { get; set; }

        [DisplayName("Employee Photo")]
        public string EMPLOYEE_PHOTO { get; set; }

        // Navigation properties (not mapped)
        [NotMapped]
        public string DepartmentName { get; set; }

        [NotMapped]
        public string DesignationName { get; set; }

        [NotMapped]
        public string LocationName { get; set; }
    }
}