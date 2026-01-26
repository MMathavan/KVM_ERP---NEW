using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System;
using System.Web.Mvc;  // This contains SelectListItem
using System.Web; // For HttpPostedFileBase

namespace KVM_ERP.Models
{
    public class AccountViewModels
    {

        public class ManageUserViewModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage =
                "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [System.ComponentModel.DataAnnotations.Compare("NewPassword",
                ErrorMessage = "The new password and confirmation password do not match.")]
            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; }

            public string NPassword { get; set; }
        }


        public class LoginViewModel
        {
            [Required]
            [Display(Name = "User name")]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
            [Display(Name = "Accounting Year")]
            public int COMPYID { get; set; }
            [Display(Name = "Company Name")]
            public int COMPID { get; set; }
            [Display(Name = "Login Date")]
            public DateTime LDATE { get; set; }
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }


        public class RegisterViewModel : IValidatableObject
        {
            [Required]
            [Display(Name = "User name")]
            public string UserName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage =
                "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            [Display(Name = "Confirm Password")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            [Display(Name = "Email Address")]
            public string Email { get; set; }

            public string NPassword { get; set; }

            [Required]
            [Display(Name = "Mobile Number")]
            [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be 10 digits")]
            [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers are allowed")]
            public string MobileNo { get; set; }

            [Required]
            [Display(Name = "Date of Birth")]
            [DataType(DataType.Date)]
            public DateTime DOB { get; set; }

            [Required(ErrorMessage = "Gender is required")]
            [StringLength(10, ErrorMessage = "Gender must be up to 10 characters")]
            [Display(Name = "Gender")]
            public string Gender { get; set; }  // Can be replaced with enum (e.g., "Male", "Female", "Other")

            // Government proof upload
            [Display(Name = "Government Proof (PDF/Image)")]
            public HttpPostedFileBase GovernmentProofFile { get; set; }

            // Saved path of the uploaded government proof
            public string GovernmentProofPath { get; set; }


            // Return a pre-poulated instance of AppliationUser:
            public ApplicationUser GetUser()
            {
                var user = new ApplicationUser()
                {
                    UserName = this.UserName,
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    Email = this.Email,
                    NPassword = this.NPassword,
                    MobileNo = this.MobileNo,   // New field
                    DOB = this.DOB,              // New field
                    Gender = this.Gender,        // New field
                    GovernmentProofPath = this.GovernmentProofPath,
                    // Provide safe defaults to satisfy non-null DB columns (if present)
                    CateTid = 0,
                    MemberID = 0
                };
                return user;
            }

            // Server-side validation to ensure DOB is strictly earlier than today
            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (DOB >= DateTime.Today)
                {
                    yield return new ValidationResult(
                        "Date of Birth must be earlier than today.",
                        new[] { nameof(DOB) }
                    );
                }
            }
        }


        public class EditUserViewModel
        {
            public EditUserViewModel() { }

            // Allow Initialization with an instance of ApplicationUser:
            public EditUserViewModel(ApplicationUser user)
            {
                this.UserName = user.UserName;
                this.FirstName = user.FirstName;
                this.LastName = user.LastName;
                this.Email = user.Email;
                this.NPassword = user.NPassword;
                this.MobileNo = user.MobileNo;    
                this.DOB = user.DOB;             
                this.Gender = user.Gender;        

            }

            [Required]
            [Display(Name = "User Name")]
            public string UserName { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Mobile Number")]
            [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be exactly 10 digits")]
            [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be exactly 10 digits")] 
            public string MobileNo { get; set; }

            [Required]
            [Display(Name = "Date of Birth")]
            [DataType(DataType.Date)]
            public DateTime DOB { get; set; }

            [Required]
            [Display(Name = "Gender")]
            public string Gender { get; set; }
    
            public string NPassword { get; set; }

        }


        // Used to display a single role with a checkbox, within a list structure:
        public class SelectRoleEditorViewModel
        {
            public SelectRoleEditorViewModel() { }

            // Update this to accept an argument of type ApplicationRole:
            public SelectRoleEditorViewModel(ApplicationRole role)
            {
                this.RoleName = role.Name;

                // Assign the new Descrption property:
                this.Description = role.Description;
            }

            public bool Selected { get; set; }

            [Required]
            public string RoleName { get; set; }

            // Add the new Description property:
            public string Description { get; set; }
        }


        //public class RoleViewModel
        //{
        //    public string RoleName { get; set; }
        //    public string Description { get; set; }
        //    [DisplayName("Department")]
        //    public int SDPTID { get; set; }

        //    public RoleViewModel() { }
        //    public RoleViewModel(ApplicationRole role)
        //    {
        //        this.RoleName = role.Name;
        //        this.Description = role.Description;
        //    }
        //}

        public class RoleViewModel
        {
            public string RoleName { get; set; }
            public string Description { get; set; }
            [DisplayName("Department")]
            public int SDPTID { get; set; }

            // New fields for Add functionality
            [DisplayName("Menu Name")]
            public string MenuName { get; set; }

            [DisplayName("Controller Name")]
            public string ControllerName { get; set; }

            [DisplayName("Menu Index")]
            public string MenuIndex { get; set; }

            [DisplayName("Menu Order")]
            public short? MenuOrder { get; set; }

            [DisplayName("Order")]
            public short? Order { get; set; }

            [DisplayName("R Image")]
            public string RImage { get; set; }

            // Dropdown options
            public List<SelectListItem> MenuOrderOptions { get; set; }
            public List<SelectListItem> RImageOptions { get; set; }

            public RoleViewModel()
            {
                MenuOrderOptions = new List<SelectListItem>();
                RImageOptions = new List<SelectListItem>();
            }

            public RoleViewModel(ApplicationRole role) : this()
            {
                this.RoleName = role.Name;
                this.Description = role.Description;
            }
        }


        //public class MenuMaster
        //{
        //    public int Id { get; set; }
        //    public string MenuOrder { get; set; } // Change to int if it's numeric
        //                                          // Add other properties as needed
        //}

        //public class RImageMaster
        //{
        //    public int Id { get; set; }
        //    public string ImageName { get; set; }
        //    // Add other properties as needed
        //}


        public class EditRoleViewModel
        {
            public string OriginalRoleName { get; set; }
            public string RoleName { get; set; }
            public string Description { get; set; }

            public EditRoleViewModel() { }
            public EditRoleViewModel(ApplicationRole role)
            {
                this.OriginalRoleName = role.Name;
                this.RoleName = role.Name;
                this.Description = role.Description;
            }
        }


        // Wrapper for SelectGroupEditorViewModel to select user group membership:
        public class SelectUserGroupsViewModel
        {
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public List<SelectGroupEditorViewModel> Groups { get; set; }

            public SelectUserGroupsViewModel()
            {
                this.Groups = new List<SelectGroupEditorViewModel>();
            }

            public SelectUserGroupsViewModel(ApplicationUser user)
                : this()
            {
                this.UserName = user.UserName;
                this.FirstName = user.FirstName;
                this.LastName = user.LastName;

                var Db = new ApplicationDbContext();

                // Add all available groups to the public list:
                var allGroups = Db.Groups;
                foreach (var role in allGroups)
                {
                    // An EditorViewModel will be used by Editor Template:
                    var rvm = new SelectGroupEditorViewModel(role);
                    this.Groups.Add(rvm);
                }

                // Set the Selected property to true where user is already a member:
                foreach (var group in user.Groups)
                {
                    var checkUserRole =
                        this.Groups.Find(r => r.GroupName == group.Group.Name);
                    checkUserRole.Selected = true;
                }
            }
        }


        // Used to display a single role group with a checkbox, within a list structure:
        public class SelectGroupEditorViewModel
        {
            public SelectGroupEditorViewModel() { }
            public SelectGroupEditorViewModel(Group group)
            {
                this.GroupName = group.Name;
                this.GroupId = group.Id;
            }

            public bool Selected { get; set; }

            [Required]
            public int GroupId { get; set; }
            public string GroupName { get; set; }
        }


        public class SelectGroupRolesViewModel
        {
            public SelectGroupRolesViewModel()
            {
                this.Roles = new List<SelectRoleEditorViewModel>();
            }


            // Enable initialization with an instance of ApplicationUser:
            public SelectGroupRolesViewModel(Group group)
                : this()
            {
                this.GroupId = group.Id;
                this.GroupName = group.Name;

                var Db = new ApplicationDbContext();

                // Add all available roles to the list of EditorViewModels:
                var allRoles = Db.Roles.OrderBy(x => x.Name);
                foreach (var role in allRoles)
                {
                    // An EditorViewModel will be used by Editor Template:
                    var rvm = new SelectRoleEditorViewModel(role);
                    this.Roles.Add(rvm);
                }

                // Set the Selected property to true for those roles for 
                // which the current user is a member:
                foreach (var groupRole in group.Roles)
                {
                    var checkGroupRole =
                        this.Roles.Find(r => r.RoleName == groupRole.Role.Name);
                    checkGroupRole.Selected = true;
                }
            }

            public int GroupId { get; set; }
            public string GroupName { get; set; }
            public List<SelectRoleEditorViewModel> Roles { get; set; }
        }


        public class UserPermissionsViewModel
        {
            public UserPermissionsViewModel()
            {
                this.Roles = new List<RoleViewModel>();
            }


            // Enable initialization with an instance of ApplicationUser:
            public UserPermissionsViewModel(ApplicationUser user)
                : this()
            {
                this.UserName = user.UserName;
                this.FirstName = user.FirstName;
                this.LastName = user.LastName;
                foreach (var role in user.Roles)
                {
                    var appRole = (ApplicationRole)role.Role;
                    var pvm = new RoleViewModel(appRole);
                    this.Roles.Add(pvm);
                }
            }

            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public List<RoleViewModel> Roles { get; set; }
        }


    }
}