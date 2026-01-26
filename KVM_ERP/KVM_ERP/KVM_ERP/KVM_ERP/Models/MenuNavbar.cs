using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KVM_ERP.Models
{
    public class MenuNavbar
    {
        public int MenuGId { get; set; }
        public int MenuGIndex { get; set; }
        public string LinkText { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string action { get; set; }
        public string area { get; set; }
        public string imageClass { get; set; }
        public string activeli { get; set; }
        public bool estatus { get; set; }

        public string username { get; set; }
    }
}