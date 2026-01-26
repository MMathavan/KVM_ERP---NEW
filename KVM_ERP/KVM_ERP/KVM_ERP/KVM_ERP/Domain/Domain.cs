using KVM_ERP.Models;
using ClubMembership.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace KVM_ERP
{
  
    public class MenuNavData
    {
        private readonly ApplicationDbContext context = new ApplicationDbContext();
        public IEnumerable<MenuNavbar> navbarItems()
        {
            //" + Session["CUSRID"] + "
            //var uname = HttpSessionStateBase["CUSRID"].ToString();
            var amenu = new List<MenuNavbar>();

            // Guard against missing session (e.g., timeout) to avoid NullReference
            var http = System.Web.HttpContext.Current;
            var session = http != null ? http.Session : null;
            var cusrid = session != null ? Convert.ToString(session["CUSRID"]) : null;

            if (string.IsNullOrWhiteSpace(cusrid))
            {
                // No session -> return empty menu to keep layout rendering without crash
                return amenu.ToList();
            }

            //var query = context.Database.SqlQuery<MenuRoleMaster>("selecgit statust * from MenuRoleMaster where Roles='admin'");
            var query = context.Database.SqlQuery<MenuRoleMaster>("select * from MenuRoleMaster where Roles='" + cusrid + "'");
            
            // Get current user to check role claims
            var user = http != null ? http.User : null;
            
            foreach (var data in query)
            {
                // CRITICAL FIX: Menu item visibility logic
                // Menu should ONLY show if user has Index permission (since clicking menu goes to Index action)
                // When ALL 4 permissions (Index, Create, Edit, Delete) are deselected, menu will be hidden
                string controllerName = data.ControllerName;
                string requiredIndexRole = controllerName + "Index";
                
                System.Diagnostics.Debug.WriteLine($"[Menu] Checking menu item '{data.LinkText}' (Controller: {controllerName})");
                
                // Check if user has Index permission OR has all 4 permissions deselected
                bool shouldShowMenu = false;
                
                if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
                {
                    // Check claims directly (since we added group roles as claims)
                    var claimsPrincipal = user as System.Security.Claims.ClaimsPrincipal;
                    if (claimsPrincipal != null)
                    {
                        // Debug: Show all role claims for this user
                        var roleClaims = claimsPrincipal.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToArray();
                        System.Diagnostics.Debug.WriteLine($"[Menu] User has {roleClaims.Length} role claims: {string.Join(", ", roleClaims.Take(5))}...");
                        
                        // Check all 4 permissions
                        bool hasIndex = claimsPrincipal.HasClaim(System.Security.Claims.ClaimTypes.Role, controllerName + "Index");
                        bool hasCreate = claimsPrincipal.HasClaim(System.Security.Claims.ClaimTypes.Role, controllerName + "Create");
                        bool hasEdit = claimsPrincipal.HasClaim(System.Security.Claims.ClaimTypes.Role, controllerName + "Edit");
                        bool hasDelete = claimsPrincipal.HasClaim(System.Security.Claims.ClaimTypes.Role, controllerName + "Delete");
                        
                        // Menu shows ONLY if user has Index permission
                        // BUT if ALL 4 permissions are deselected, menu should be hidden
                        bool hasAnyPermission = hasIndex || hasCreate || hasEdit || hasDelete;
                        
                        if (!hasAnyPermission)
                        {
                            // All 4 permissions deselected - hide menu completely
                            shouldShowMenu = false;
                            System.Diagnostics.Debug.WriteLine($"[Menu] All permissions deselected for '{controllerName}' - hiding menu");
                        }
                        else if (hasIndex)
                        {
                            // User has Index permission - show menu (can access the page)
                            shouldShowMenu = true;
                            System.Diagnostics.Debug.WriteLine($"[Menu] User has Index permission for '{controllerName}' - showing menu");
                        }
                        else
                        {
                            // User has some permissions but NOT Index - hide menu (clicking would redirect to login)
                            shouldShowMenu = false;
                            System.Diagnostics.Debug.WriteLine($"[Menu] User lacks Index permission for '{controllerName}' (Has: Create={hasCreate}, Edit={hasEdit}, Delete={hasDelete}) - hiding menu to prevent login redirect");
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"[Menu] Permissions for '{controllerName}': Index={hasIndex}, Create={hasCreate}, Edit={hasEdit}, Delete={hasDelete} => ShowMenu={shouldShowMenu}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[Menu] User is not ClaimsPrincipal, trying IsInRole");
                        // Fallback to IsInRole for backward compatibility
                        bool hasIndex = user.IsInRole(requiredIndexRole);
                        bool hasAnyPermission = hasIndex || 
                                              user.IsInRole(controllerName + "Create") || 
                                              user.IsInRole(controllerName + "Edit") || 
                                              user.IsInRole(controllerName + "Delete");
                        
                        // Same logic: show only if has Index, hide if all deselected or if lacks Index
                        if (!hasAnyPermission)
                        {
                            shouldShowMenu = false;
                        }
                        else if (hasIndex)
                        {
                            shouldShowMenu = true;
                        }
                        else
                        {
                            shouldShowMenu = false;
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"[Menu] IsInRole check: hasIndex={hasIndex}, hasAny={hasAnyPermission}, ShowMenu={shouldShowMenu}");
                    }
                    
                    if (!shouldShowMenu)
                    {
                        // Skip this menu item
                        System.Diagnostics.Debug.WriteLine($"[Menu] ❌ HIDING '{data.LinkText}'");
                        continue;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[Menu] ✅ SHOWING '{data.LinkText}'");
                    }
                }
                else
                {
                    // User not authenticated - skip all menu items
                    System.Diagnostics.Debug.WriteLine($"[Menu] User not authenticated - hiding all menu items");
                    continue;
                }
                
                amenu.Add(new MenuNavbar { MenuGId = Convert.ToInt32(data.MenuGId),
                                           MenuGIndex = Convert.ToInt32(data.MenuGIndex),
                                           LinkText  = data.LinkText,
                                           ActionName = data.ActionName,
                                           ControllerName = data.ControllerName,
                                           username = cusrid,// "admin",
                                           imageClass = data.ImageClassName, estatus = true });
            }

            return amenu.ToList();
        }

        public IEnumerable<User> users()
        {
            var users = new List<User>();

            var query = context.Database.SqlQuery<AspNetUser>("select * from AspNetUsers");
            foreach (var data1 in query)
            {
                users.Add(new User
                {
                    Id = data1.Id,
                    user = data1.UserName,
                    password = data1.PasswordHash,
                    estatus = true,
                    RememberMe = true
                });
            }
            return users.ToList();
        }

        public IEnumerable<Roles> roles()
        {
            var roles = new List<Roles>();
            roles.Add(new Roles { rowid = 1, idUser = 1, idMenu = 1, status = true });
            roles.Add(new Roles { rowid = 2, idUser = 1, idMenu = 2, status = true });
            roles.Add(new Roles { rowid = 3, idUser = 1, idMenu = 3, status = true });
            roles.Add(new Roles { rowid = 4, idUser = 1, idMenu = 4, status = true });
            roles.Add(new Roles { rowid = 5, idUser = 1, idMenu = 5, status = true });
            roles.Add(new Roles { rowid = 6, idUser = 1, idMenu = 6, status = true });
            roles.Add(new Roles { rowid = 7, idUser = 1, idMenu = 7, status = true });
            roles.Add(new Roles { rowid = 8, idUser = 2, idMenu = 1, status = true });
            roles.Add(new Roles { rowid = 9, idUser = 2, idMenu = 2, status = true });
            roles.Add(new Roles { rowid = 10, idUser = 2, idMenu = 3, status = true });
            roles.Add(new Roles { rowid = 11, idUser = 2, idMenu = 4, status = true });
            roles.Add(new Roles { rowid = 12, idUser = 2, idMenu = 5, status = true });
            roles.Add(new Roles { rowid = 13, idUser = 3, idMenu = 1, status = true });
            roles.Add(new Roles { rowid = 14, idUser = 3, idMenu = 2, status = true });

            return roles.ToList();
        }

        public IEnumerable<MenuNavbar> itemsPerUser(string controller, string action, string userName)
        {
            
            IEnumerable<MenuNavbar> items = navbarItems();
            //IEnumerable<Roles> rolesNav = roles();
            IEnumerable<User> usersNav = users();

            var navbar =  items.Where(p => p.ControllerName == controller && p.action == action).Select(c => { c.activeli = "active"; return c; }).ToList();

            navbar = (from nav in items
                      where nav.username == userName

                      select new MenuNavbar
                      {
                          MenuGId = nav.MenuGId,
                          MenuGIndex = nav.MenuGIndex,
                          LinkText = nav.LinkText,
                          ControllerName = nav.ControllerName,
                          ActionName = nav.ActionName,
                          imageClass = nav.imageClass,
                          estatus = nav.estatus,
                          activeli = nav.activeli
                      }).ToList();

            return navbar.ToList();
        }

    }
}