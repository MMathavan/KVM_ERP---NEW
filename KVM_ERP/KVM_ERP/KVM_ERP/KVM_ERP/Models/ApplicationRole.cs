using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace KVM_ERP.Models
{
    //public class ApplicationRole : IdentityRole
    //{
    //    public ApplicationRole() : base() { }


    //    public ApplicationRole(string name, string description, int SDPTID) : base(name)
    //    {
    //        this.Description = description;
    //    }
    //    public virtual string Description { get; set; }
    //}
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string name, string description, int sdptId) : base(name)
        {
            this.Description = description;
            this.SDPTID = sdptId;
        }

        public string Description { get; set; }
        public int SDPTID { get; set; }

        // New properties
        public string RMenuType { get; set; }
        public string RControllerName { get; set; }
        public string RMenuIndex { get; set; }
        public short? RMenuGroupId { get; set; }
        public short? RMenuGroupOrder { get; set; }
        public string RImageClassName { get; set; }
    }
}