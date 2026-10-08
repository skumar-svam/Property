using MvcSiteMapProvider;
using NA.PMS.Model;
using NA.PMS.Service;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NA.PMS.Web.SiteMapHelper
{
    public class AuthenticatedVisibilityProvider : SiteMapNodeVisibilityProviderBase
    {
        IMenuMappingService menuMappingService { get; set; }

        public AuthenticatedVisibilityProvider()
        {
            this.menuMappingService = new MenuMappingService();
        }
        public override bool IsVisible(ISiteMapNode node, IDictionary<string, object> sourceMetadata)
        {
            //  return IsMenuActiveForUser(node); 
            // if (HttpContext.Current.Request.IsAuthenticated)
            {
                if (sourceMetadata.ContainsKey("HtmlHelper") &&
                    sourceMetadata["HtmlHelper"].ToString().Equals("MvcSiteMapProvider.Web.Html.MenuHelper"))
                {
                    if (node.Attributes.ContainsKey("visibility") && node.Attributes["visibility"] != null && (string)node.Attributes["visibility"] == "false")
                    {
                        return false;
                    }
                }
                return IsMenuActiveForUser(node);
            }
            //else
            //    return false;
        }


        public bool IsMenuActiveForUser(ISiteMapNode node)
        {
            // return true;
            List<int> allMenudIdsForRoleUser = GetMenuIdsForUserName();
            var menuId = Convert.ToInt32(node.Attributes["id"]);
            // node.Attributes["visibility"] = false;
            return allMenudIdsForRoleUser.Contains(menuId);
        }


        public List<int> GetMenuIdsForUserName()
        {
            var currentUserDetail = (CurrentUserDetail)(HttpContext.Current.Session["CurrentUser"]);
            var allMenudIdsForRoleUser = new List<int>();
            if (currentUserDetail != null)
            {
                var lstGetMenuID = new List<GetMenuID>();
                List<int> roleIds = new List<int>();
                foreach (var objMultiRole in currentUserDetail.MultiRole)
                {
                    roleIds.Add(objMultiRole.RoleId);
                }
                if (currentUserDetail.RoleId != 0)
                {
                    //var user = (CurrentUserDetail)HttpContext.Current.Session["CurrentUser"];

                    if (HttpContext.Current.Session["allMenudIdsForRoleUser"] == null)
                    {
                        lstGetMenuID = menuMappingService.GetMenuIdsByRoleID(roleIds);
                        var lstMenus = lstGetMenuID.Select(e => e.parentID).Distinct().ToList();
                        foreach (var item in lstMenus)
                        {
                            allMenudIdsForRoleUser.Add(item);
                        }
                        foreach (GetMenuID item in lstGetMenuID)
                        {
                            allMenudIdsForRoleUser.Add(item.menuID);
                        }
                        HttpContext.Current.Session["allMenudIdsForRoleUser"] = allMenudIdsForRoleUser;
                    }
                    else
                    {
                        if (HttpContext.Current.Session["allMenudIdsForRoleUser"] != null)
                            allMenudIdsForRoleUser = (List<int>)HttpContext.Current.Session["allMenudIdsForRoleUser"];
                    }
                }
            }

            return allMenudIdsForRoleUser;
        }
    }
}