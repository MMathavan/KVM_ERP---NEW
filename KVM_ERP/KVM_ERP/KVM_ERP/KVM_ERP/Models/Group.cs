using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KVM_ERP.Models
{
    public class Group
    {
        public Group()
        {
        }


        public Group(string name) : this()
        {
            Name = name;
        }


        [Key]
        [Required]
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
        private ICollection<ApplicationRoleGroup> _roles;
        public virtual ICollection<ApplicationRoleGroup> Roles
        {
            get { return _roles ?? (_roles = new List<ApplicationRoleGroup>()); }
            protected set { _roles = value; }
        }
    }
}