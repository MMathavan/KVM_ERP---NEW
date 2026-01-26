using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KVM_ERP.Models
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public int COMPID { get; set; }

        public int COMPYID { get; set; }
        public DateTime LDATE { get; set; }

        public int BRNCHID { get; set; }

        public string BRNCHNAME { get; set; }

        public Int16 BRNCHCTYPE { get; set; }
        //[Display(Name = "Remember me?")]
        //public bool RememberMe { get; set; }
        public bool RememberMe { get; set; }
    }
}