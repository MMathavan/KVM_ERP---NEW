using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KVM_ERP.Models
{
    [Table("MenuRoleMaster")]
    public class MenuRoleMaster
    {
        [Key]

        public int MenuId { get; set; }
        public string LinkText { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string Roles { get; set; }
        public short MenuGId { get; set; }
        public short MenuGIndex { get; set; }
        public string ImageClassName { get; set; }
    }
}