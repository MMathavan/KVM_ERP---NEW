using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;

namespace KVM_ERP.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
            : base()
        {
            this.Groups = new HashSet<ApplicationUserGroup>();
        }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be exactly 10 digits")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be exactly 10 digits")]
        [Display(Name = "Mobile Number")]
        public string MobileNo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        public string NPassword { get; set; }

        // Category Type ID - links to CategoryTypeMaster table
        [Display(Name = "Category Type")]
        public int? CateTid { get; set; }

        // Links AspNetUsers to MemberShipMaster.MemberID
        public int? MemberID { get; set; }

        // Stores the relative/absolute path of the uploaded Government Proof file
        [Display(Name = "Government Proof Path")]
        [NotMapped]
        public string GovernmentProofPath { get; set; }

        public virtual ICollection<ApplicationUserGroup> Groups { get; set; }
    }
}