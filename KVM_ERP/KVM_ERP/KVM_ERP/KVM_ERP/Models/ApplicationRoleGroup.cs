using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KVM_ERP.Models
{
    public class ApplicationRoleGroup
    {
        public virtual string RoleId { get; set; }
        public virtual int GroupId { get; set; }

        public virtual ApplicationRole Role { get; set; }
        public virtual Group Group { get; set; }
    }
}