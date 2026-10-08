using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcSiteMapProvider;
using System.Web.Mvc;
using System.Web.Routing;
using NA.PMS.Web.Models;
using NA.PMS.Model;
using NA.PMS.Web.Controllers.Common;

namespace NA.PMS.Web.Filters
{
    public class NAAuth : System.Web.Mvc.AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //if (HttpContext.Current.Session["CurrentUser"] == null)
            //{
            //    LoadSession(HttpContext.Current.User.Identity.Name);
            //}

            var user = (CurrentUserDetail)HttpContext.Current.Session["CurrentUser"];
            if (user != null)
            {
                if (user.IsActive == false)
                {
                    HandleUnauthorizedRequest(filterContext);
                    return;
                }
            }
            base.OnAuthorization(filterContext);
            if (filterContext == null || filterContext.HttpContext == null)
                return;
            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;
            string actionName = string.Empty;
            string controllerName = string.Empty;
            if (filterContext.RouteData.Values["controller"] != null &&
                filterContext.RouteData.Values["action"] != null &&
                filterContext.RouteData.Values["controller"].ToString().Trim().Length > 0 &&
                filterContext.RouteData.Values["action"].ToString().Trim().Length > 0)
            {
                controllerName = filterContext.RouteData.Values["controller"].ToString();
                actionName = filterContext.RouteData.Values["action"].ToString();
            }
            else
            {
                return;
            }


            List<int> allMenusForRole = new List<int>();

            //if (HttpContext.Current.Session["allMenudIdsForRoleUser"] != null)
            //{
            //    allMenusForRole = (List<int>)(HttpContext.Current.Session["allMenudIdsForRoleUser"]);
            //    var x = user.MenuMaster;
            //}
            //else
            //{
            //    if (user != null)
            //    {
            //        allMenusForRole = user.MenuMaster.Select(x => x.MenuId).ToList();
            //        //IMenuMappingService menuService = new MenuMappingService();

            //        //allMenusForRole = menuService.GetallMenuIdsForRole(user.User_Role_Id);
            //    }
            //}

            //Picking up allowed Menu IDs from Session["CurrentUser"]
            if (user != null)
                allMenusForRole = user.MenuMaster.Select(x => x.MenuId).ToList();

            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                //if (HttpContext.Current.Session["Roles"] != null)
                //{
                //    role = (Role)(HttpContext.Current.Session["Roles"]);
                //}

                var nodeColl = ChildrenOf(MvcSiteMapProvider.SiteMaps.Current.RootNode);
                bool unAuthorize = false;
                //if (user!=null)
                //{
                foreach (var node in nodeColl)
                {
                    //if the controller and action is not part of XML sitemap do not use role based security
                    if (node.Controller.ToLower().Trim() == controllerName.ToLower().Trim() &&
                        node.Action.ToLower().Trim() == actionName.ToLower().Trim())
                    {

                        if (node.Attributes.Count > 0)
                        {
                            int id = Convert.ToInt32(node.Attributes["id"]);

                            if (allMenusForRole.Contains(id))
                            {
                                unAuthorize = false;
                                break;
                            }
                            else
                            {
                                unAuthorize = true;
                            }
                        }
                    }
                }
                if (unAuthorize == true)
                {
                    HandleUnauthorizedRequest(filterContext);
                }
            }
        }

        public IEnumerable<ISiteMapNode> ChildrenOf(ISiteMapNode root)
        {
            yield return root;
            foreach (var c in root.ChildNodes)
                foreach (var cc in ChildrenOf(c))
                    yield return cc;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "UnauthorizedAccess", area = "" }));
        }

        //private void LoadSession(string userName)
        //{
        //    PMSMembershipProvider _PmsMembership = new PMSMembershipProvider();
        //    var loginUser = _PmsMembership.GetUserDetails(userName);
        //    if (loginUser != null && loginUser.IsActive == true)
        //    {
        //        var currentUserDetail = new CurrentUserDetail();
        //        currentUserDetail.UserID = loginUser.UserRefId;
        //        currentUserDetail.UserName = loginUser.UserName;
        //        currentUserDetail.FirstName = loginUser.FirstName;
        //        currentUserDetail.LastName = loginUser.LastName;
        //        currentUserDetail.LastName = loginUser.LastName;
        //        currentUserDetail.FullName = loginUser.FirstName + " " + loginUser.MiddleName + " " + loginUser.LastName;
        //        currentUserDetail.Email = loginUser.Email;
        //        currentUserDetail.CreatedDate = loginUser.CreatedDate;
        //        currentUserDetail.ModifiedDate = loginUser.ModifiedDate;
        //        currentUserDetail.IsActive = loginUser.IsActive;

        //        UmRoleMaster userRole = _PmsMembership.GetUserRoleDetails(loginUser.UserRefId);
        //        List<UmRoleMaster> roleMasters = _PmsMembership.GetRoleMasterDetails(loginUser.UserRefId);
        //        List<ApplicationMaster> applicationsList = _PmsMembership.GetApplicationDetailsByUserId(loginUser.UserRefId);
        //        List<MenuMaster> menusList = _PmsMembership.GetMenuDetailsByUserId(loginUser.UserRefId);

        //        currentUserDetail.RoleMaster = userRole;
        //        currentUserDetail.MultiRole = roleMasters;
        //        currentUserDetail.ApplicationMaster = applicationsList;
        //        currentUserDetail.MenuMaster = menusList;

        //        HttpContext.Current.Session["CurrentUser"] = currentUserDetail;
        //    }
        //}
    }
}