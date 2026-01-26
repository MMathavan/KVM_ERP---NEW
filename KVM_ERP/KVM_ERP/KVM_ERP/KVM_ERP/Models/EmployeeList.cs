using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KVM_ERP.Models
{
    public class EmployeeList
    {
        public List<EmployeeMaster> masterdata { get; set; }
        public List<EmployeeLinkMaster> detaildata { get; set; }

        // public List<pr_Employoee_Master_Link_Ctrl_Assgn>
    }
}