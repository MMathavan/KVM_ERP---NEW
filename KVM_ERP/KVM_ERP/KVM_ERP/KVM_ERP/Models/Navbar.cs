using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sb_admin.Models
{
    public class Navbar
    {
        public int Id { get; set; }
        public string nameOption1 { get; set; }
        public string controller1 { get; set; }
        public string action1 { get; set; }
        public string area1 { get; set; }
        public string imageClass1 { get; set; }
        public string activeli1 { get; set; }
        public bool estatus { get; set; }
    }
}