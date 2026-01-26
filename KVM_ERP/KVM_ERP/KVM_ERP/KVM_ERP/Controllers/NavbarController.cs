using ClubMembership.Data;
using KVM_ERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KVM_ERP;

namespace KVM_ERP.Controllers
{
    public class NavbarController : Controller
    {
        // GET: Navbar
        public ActionResult Navbar(string controller, string action)
        {
            // Always render navbar; the partial will tailor items by user/session/roles
            var isAuthenticated = Request.IsAuthenticated;
            var data = new MenuNavData();
            var userName = isAuthenticated ? User.Identity.Name : string.Empty;
            var navbar = data.itemsPerUser(controller, action, userName);
            return PartialView("_navbar", navbar);
        }
    }
}