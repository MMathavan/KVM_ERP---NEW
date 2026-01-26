using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace KVM_ERP
{
    public class SessionExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Skip checks for actions explicitly allowing anonymous access
            bool allowAnonymous = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                                   || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);

            // Skip for Account controller (Login, Register, etc.) to avoid redirect loops
            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            if (allowAnonymous || string.Equals(controllerName, "Account", StringComparison.OrdinalIgnoreCase))
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            if (HttpContext.Current.Session == null || HttpContext.Current.Session["Group"] == null || HttpContext.Current.Session["CUSRID"] == null || HttpContext.Current.Session["compyid"] == null)
            {
                //                FormsAuthentication.SignOut();
                HttpContext.Current.GetOwinContext().Authentication.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);
                filterContext.Result =
                new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "action", "Login" },
                    { "controller", "Account" },
                    { "returnUrl", filterContext.HttpContext.Request.RawUrl}
                });
                return;
            }
        }
    }
}