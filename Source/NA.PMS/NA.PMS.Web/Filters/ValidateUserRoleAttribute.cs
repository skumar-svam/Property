
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NA.PMS.Model;
using System.Web.Security;
using NA.PMS.Service;
using MvcSiteMapProvider;

namespace NA.PMS.Web.Filters
{
    //public class ValidateUserRoleAttribute : AuthorizeAttribute
    //{
    //    public override void OnAuthorization(AuthorizationContext filterContext)
    //    {
    //        base.OnAuthorization(filterContext);
    //        if (filterContext == null || filterContext.HttpContext == null)
    //            return;
    //        HttpRequestBase request = filterContext.HttpContext.Request;
    //        if (request == null)
    //            return;

    //        //don't apply filter to child methods
    //        if (filterContext.IsChildAction)
    //            return;

    //        if (filterContext.RouteData.Values["controller"] != null &&
    //            filterContext.RouteData.Values["action"] != null &&
    //            filterContext.RouteData.Values["controller"].ToString().Trim().Length > 0 &&
    //            filterContext.RouteData.Values["action"].ToString().Trim().Length > 0)
    //        {
    //        }
    //        else
    //        {
    //            return;
    //        }

    //        UmRoleMaster role = null;
    //        if (filterContext.HttpContext.Request.IsAuthenticated)
    //        {
    //            var user = (CurrentUserDetail)HttpContext.Current.Session["CurrentUser"];
    //            if (user.MultiRole.Count != 0)
    //            {
    //                role = (UmRoleMaster)user.RoleMaster;
    //            }
    //            bool unAuthorize = false;
    //            if (role != null)
    //            {
    //                if (role.RoleType != "")
    //                {
    //                    unAuthorize = true;
    //                }
    //                if (unAuthorize)
    //                {
    //                    filterContext.Result = filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "login" })); ;
    //                }
    //            }
    //            else
    //            {
    //                filterContext.Result = filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" })); ;
    //            }
    //        }

    //        //Role role = null;

    //        //if (filterContext.HttpContext.Request.IsAuthenticated)
    //        //{
    //        //    if (HttpContext.Current.Session["Roles"] != null)
    //        //    {
    //        //        role = (Role)(HttpContext.Current.Session["Roles"]);
    //        //    }

    //        //    bool unAuthorize = false;
    //        //    if (role != null)
    //        //    {
    //        //        if (role.RoleId != Constants.AdminRoleId && role.RoleId != Constants.DeptRoleId && role.RoleId != Constants.BackOfficeRoleId)
    //        //        {
    //        //            unAuthorize = true;
    //        //        }


    //        //        if (unAuthorize)
    //        //        {
    //        //            filterContext.Result = filterContext.Result = new RedirectToRouteResult(new
    //        //                RouteValueDictionary(new { controller = "Account", action = "Login" }));
    //        //            ;
    //        //        }
    //        //    }
    //        //    else
    //        //    {
    //        //        filterContext.Result = filterContext.Result = new RedirectToRouteResult(new
    //        //            RouteValueDictionary(new { controller = "Account", action = "Login" }));
    //        //        ;
    //        //    }
    //        //}
    //    }
    //}



    public class AuthorityMemberAttribute : AuthorizeAttribute
    {
        public string View { get; set; }
        public string Master { get; set; }

        public AuthorityMemberAttribute()
        {
            View = "error";
            Master = String.Empty;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            CheckIfUserIsAuthenticated(filterContext);
        }
        private void CheckIfUserIsAuthenticated(AuthorizationContext filterContext)
        {
            // If Result is null, we're OK: the user is authenticated and authorized.
            if (filterContext.Result == null)
                return;
            // If here, you're getting an HTTP 401 status code
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (String.IsNullOrEmpty(View))
                    return;
                var result = new ViewResult { ViewName = View, MasterName = Master };
                filterContext.Result = result;
            }
        }

        // put as [AuthorizedOnly(Roles="admin", Users="DinoE")]


    }

    public class AuthorityRoleProvider : RoleProvider
    {

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }


    public class AuthoritySuperAdminAttribute : AuthorizeAttribute
    {
        ILoginService service = new LoginService();
        //Entities context = new Entities(); // my entity  
        private readonly string[] allowedroles;
        CurrentUserDetail usr = null;
        public AuthoritySuperAdminAttribute(params string[] roles)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["CurrentUser"] != null)
                    {
                        usr = HttpContext.Current.Session["CurrentUser"] as CurrentUserDetail;
                    }
                }
            }
            this.allowedroles = roles;
        }
        
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            foreach (var role in allowedroles)
            {  
                //var user = service.GetRoleMasterDetails(usr.UserID);
                
                if (usr.RoleMaster.RoleType == "SA")
                {
                    authorize = true;
                }
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }


    public class AuthorityRoleAttribute : AuthorizeAttribute
    {
        CurrentUserDetail user;
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["CurrentUser"] != null)
                    {
                        user = HttpContext.Current.Session["CurrentUser"] as CurrentUserDetail;
                    }
                }
            }

            if (filterContext == null || filterContext.HttpContext == null)
                return;
            var request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;
            var actionName = string.Empty;
            var controllerName = string.Empty;
            if (filterContext.RouteData.Values["controller"] != null && filterContext.RouteData.Values["action"] != null &&
                filterContext.RouteData.Values["controller"].ToString().Trim().Length > 0 && filterContext.RouteData.Values["action"].ToString().Trim().Length > 0)
            {
                controllerName = filterContext.RouteData.Values["controller"].ToString();
                actionName = filterContext.RouteData.Values["action"].ToString();
            }
            else
            {
                return;
            }

            var authorize = false;
            var node = SiteMaps.Current.CurrentNode;
            if (node.Controller.ToLower().Trim() == controllerName.ToLower().Trim() && node.Action.ToLower().Trim() == actionName.ToLower().Trim())
            {
                var id = Convert.ToInt32(node.Attributes["id"]);

                foreach (var men in user.MenuMaster)
                {
                    if (men.MenuId == id)
                    {
                        authorize = true;
                        //return; 
                        break;
                        //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = node.Controller, action = node.Action, area = node.Area }));
                        
                    }
                }
                //unAuthorize = true;
            }
            if (authorize == false)
            {
                var result = new ViewResult { ViewName = "SecurityError" };
                result.ViewBag.ErrorMessage = "You are not authorized to view this page. Please contact administrator!";
                filterContext.Result = result;
            }
            
          
            //if (filterContext.HttpContext.Request.IsAuthenticated)
            //if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            //{
               
            //    if (user.IsActive == true)
            //    { 
            //        //if (HttpContext.Current.Session["Roles"] != null)
            //    //{
            //    //    role = (Role)(HttpContext.Current.Session["Roles"]);
            //    //}

            //    var nodeColl = ChildrenOf(SiteMaps.Current.RootNode);
            //    var unAuthorize = false;
            //    if (user.MultiRole.Count() > 0) //if (user.UserRoleId > 0)
            //    {
            //        foreach (var node in nodeColl)
            //        {
            //            //if the controller and action is not part of XML sitemap do not use role based security
            //            if (node.Controller.ToLower().Trim() == controllerName.ToLower().Trim() &&
            //                node.Action.ToLower().Trim() == actionName.ToLower().Trim())
            //            {
            //                var id = Convert.ToInt32(node.Attributes["id"]);
            //                //if (allMenusForRole.Contains(id))
            //                //{
            //                //    unAuthorize = false;
            //                //    break;
            //                //}
            //                foreach (var men in user.MenuMaster)
            //                {
            //                    if (men.MenuId == id)
            //                    {
            //                        unAuthorize = false;
            //                        break;
            //                    }
            //                }
            //                unAuthorize = true;
            //            }
            //        }

            //        if (unAuthorize)
            //        {
            //            var result = new ViewResult { ViewName = "SecurityError" };
            //            result.ViewBag.ErrorMessage = "You are not authorized to view this page. Please contact administrator!";
            //            filterContext.Result = result;
            //        }
            //    }
            //    else
            //    {
            //        var result = new ViewResult { ViewName = "SecurityError" };
            //        result.ViewBag.ErrorMessage = "You are not authorized to view this page. Please contact administrator!";
            //        filterContext.Result = result;
            //    }
            //}
        }

        //private List<int> GetCurrentUserRoleIds(CurrentUserDetail user)
        //{
        //    IManageRoleandMenuService menuMappingService = new ManageRoleandMenuService();
        //    return menuMappingService.GetMenuIdsByRoleID(user.UserRoleId);
        //}

        public IEnumerable<ISiteMapNode> ChildrenOf(ISiteMapNode root)
        {
            yield return root;
            foreach (var c in root.ChildNodes)
                foreach (var cc in ChildrenOf(c))
                    yield return cc;
        }
    }
}