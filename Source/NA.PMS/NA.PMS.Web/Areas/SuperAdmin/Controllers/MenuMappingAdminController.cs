using Kendo.Mvc.UI;
using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Service;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NA.PMS.Web.Areas.SuperAdmin.Controllers
{
    public class MenuMappingAdminController : Controller
    {
        IMenuMappingService _menuMappingService;
        public MenuMappingAdminController(IMenuMappingService menuMappingService)
        {
            _menuMappingService = menuMappingService;
        }
        //// GET: MenuMapping
        //public ActionResult Index()
        //{
        //    return View();
        //}

        /// <summary>
        /// Getting Html sting with Menus and Submenus on PageLoad
        /// </summary>
        /// <param name="rId"></param>
        /// <param name="appID"></param>
        /// <returns></returns>
        public ActionResult MenuMappingAdmin(int rId, int appID)
        {
            var objMenuMapping = new MenuMapping
            {
                htmlString = GetHtmlString(rId, appID),
                applicationId = appID,
                roleId = rId
            };
            TempData["appID"] = appID;
            TempData["rID"] = rId;
            return View(objMenuMapping);
        }

        /// <summary>
        /// Getting Html sting with Menus and Submenus on selection of Role dropdown
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="appID"></param>
        /// <returns></returns>
        public JsonResult MenuBind(int roleId, int appID)
        {
            var objMenuMapping = new MenuMapping { htmlString = GetHtmlString(roleId, appID) };
            return Json(objMenuMapping, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Getting Html for showing Menus and Submenus with Checkboxes
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="appID"></param>
        /// <returns></returns>
        private string GetHtmlString(int roleId, int appID)
        {
            var roleList = _menuMappingService.GetMenusByRoleId();

            var selectedMenus = _menuMappingService.GetSelectedMenus();
            selectedMenus = selectedMenus.Where(x => x.roleId == roleId).ToList();
            roleList = roleList.Where(x => x.applicationId == appID).ToList();

            var applications = roleList.Select(e => e.applicationId).Distinct().ToList();
            var lstMenu = roleList.Where(x => x.menuParentId == null).ToList();
            var lstSubMenu = roleList.Where(x => x.menuParentId != null).ToList();

            // Making Html for Menu Mapping

            string htmlString = string.Empty;
            htmlString = htmlString + "<hr style='border-top: 1px solid black'><table><tr><td ><b>Application(s)</b></td><td ><b>Read</b></td><td ><b>Write</b></td><td ><b>Modify</b></td><td ><b>Delete</b></td></tr></table><hr style='border-top: 1px solid black'>";
            foreach (var appId in applications)
            {
                string appName = roleList.Where(l => l.applicationId == appId).Select(l => l.applicationName).FirstOrDefault();
                htmlString = htmlString + "<fieldset><img id='imgApp' class='imgApp' src='/Content/Images/Home/Next.png' />" + appName;
                int countMenu = 1;
                foreach (var menu in lstMenu)
                {
                    if (menu.applicationId == appId)
                    {
                        var selectedMenu = selectedMenus.FirstOrDefault(x => x.menuPathId == menu.menuPathId && x.applicationId == menu.applicationId);
                        if (selectedMenu == null)
                        {
                            htmlString = htmlString + "<br><fieldset><div id='dvMenu' class='dvMenu'><fieldset><table ><tbody><tr><td ><img id='imgMenu' class='imgMenu' src='/Content/Images/Home/Next.png' />" + menu.menuName + "</td><td ><input class='chkMenuRead' id='chkRead_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "' type='checkbox' /></td><td ><input class='chkMenuWrite' id='chkWrite_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "'  type='checkbox' /></td><td ><input class='chkMenuModify' id='chkModify_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "'  type='checkbox'/></td><td ><input class='chkMenuDelete' id='chkDelete_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "' type='checkbox' /></td> </tr></tbody></table><div id='dvSubMenu' class='dvSubMenu'><table >";
                            int countSubMenu = 1;
                            foreach (var subMenu in lstSubMenu)
                            {
                                var selectedSubMenu = selectedMenus.FirstOrDefault(x => x.menuPathId == subMenu.menuPathId && x.applicationId == menu.applicationId);
                                if (selectedSubMenu == null)
                                {
                                    if (subMenu.menuParentId == menu.menuPathId)
                                    {
                                        htmlString = htmlString + "<tr><td>" + subMenu.menuName + "</td><td ><input class='chkSubMenuRead' id='chkRead_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox' /></td><td><input class='chkSubMenuWrite' id='chkWrite_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuModify' id='chkModify_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuDelete' id='chkDelete_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td></tr>";
                                        countSubMenu++;
                                    }
                                }
                                else
                                {
                                    string checkeboxCheckedSMRead = "";
                                    string checkeboxCheckedSMWrite = "";
                                    string checkeboxCheckedSMModify = "";
                                    string checkeboxCheckedSMDelete = "";
                                    if (selectedSubMenu.IsRead == true)
                                        checkeboxCheckedSMRead = "checked";
                                    if (selectedSubMenu.IsWrite == true)
                                        checkeboxCheckedSMWrite = "checked";
                                    if (selectedSubMenu.IsModify == true)
                                        checkeboxCheckedSMModify = "checked";
                                    if (selectedSubMenu.IsDelete == true)
                                        checkeboxCheckedSMDelete = "checked";
                                    if (subMenu.menuParentId == menu.menuPathId)
                                    {
                                        htmlString = htmlString + "<tr><td>" + subMenu.menuName + "</td><td><input class='chkSubMenuRead'" + checkeboxCheckedSMRead + " id='chkRead_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox' /></td><td><input class='chkSubMenuWrite'" + checkeboxCheckedSMWrite + " id='chkWrite_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuModify'" + checkeboxCheckedSMModify + " id='chkModify_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuDelete'" + checkeboxCheckedSMDelete + " id='chkDelete_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td></tr>";
                                        countSubMenu++;
                                    }
                                }
                            }
                            htmlString = htmlString + "</table></fieldset></div></fieldset>";
                            countMenu++;
                        }
                        else
                        {
                            string checkeboxCheckedRead = "";
                            string checkeboxCheckedWrite = "";
                            string checkeboxCheckedModify = "";
                            string checkeboxCheckedDelete = "";
                            if (selectedMenu.IsRead == true)
                                checkeboxCheckedRead = "checked";
                            if (selectedMenu.IsWrite == true)
                                checkeboxCheckedWrite = "checked";
                            if (selectedMenu.IsModify == true)
                                checkeboxCheckedModify = "checked";
                            if (selectedMenu.IsDelete == true)
                                checkeboxCheckedDelete = "checked";
                            htmlString = htmlString + "<br><fieldset><div id='dvMenu' class='dvMenu'><fieldset><table><tbody><tr><td><img id='imgMenu' class='imgMenu' src='/Content/Images/Home/Next.png' />" + menu.menuName + "</td><td><input class='chkMenuRead'" + checkeboxCheckedRead + " id='chkRead_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "' type='checkbox' /></td><td ><input class='chkMenuWrite'" + checkeboxCheckedWrite + " id='chkWrite_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "'  type='checkbox' /></td><td ><input class='chkMenuModify'" + checkeboxCheckedModify + " id='chkModify_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "'  type='checkbox'/></td><td ><input class='chkMenuDelete'" + checkeboxCheckedDelete + " id='chkDelete_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "' type='checkbox' /></td> </tr></tbody></table><div id='dvSubMenu' class='dvSubMenu'><table>";
                            int countSubMenu = 1;
                            foreach (var subMenu in lstSubMenu)
                            {
                                var selectedSubMenu = selectedMenus.FirstOrDefault(x => x.menuPathId == subMenu.menuPathId && x.applicationId == menu.applicationId);
                                if (selectedSubMenu == null)
                                {
                                    if (subMenu.menuParentId == menu.menuPathId)
                                    {
                                        htmlString = htmlString + "<tr><td>" + subMenu.menuName + "</td><td><input class='chkSubMenuRead' id='chkRead_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox' /></td><td><input class='chkSubMenuWrite' id='chkWrite_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'</td><td><input class='chkSubMenuModify' id='chkModify_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuDelete' id='chkDelete_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td></tr>";
                                        countSubMenu++;
                                    }
                                }
                                else
                                {
                                    string checkeboxCheckedSMRead = "";
                                    string checkeboxCheckedSMWrite = "";
                                    string checkeboxCheckedSMModify = "";
                                    string checkeboxCheckedSMDelete = "";
                                    if (selectedSubMenu.IsRead == true)
                                        checkeboxCheckedSMRead = "checked";
                                    if (selectedSubMenu.IsWrite == true)
                                        checkeboxCheckedSMWrite = "checked";
                                    if (selectedSubMenu.IsModify == true)
                                        checkeboxCheckedSMModify = "checked";
                                    if (selectedSubMenu.IsDelete == true)
                                        checkeboxCheckedSMDelete = "checked";
                                    if (subMenu.menuParentId == menu.menuPathId)
                                    {
                                        htmlString = htmlString + "<tr><td>" + subMenu.menuName + "</td><td><input class='chkSubMenuRead'" + checkeboxCheckedSMRead + " id='chkRead_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox' /></td><td><input class='chkSubMenuWrite'" + checkeboxCheckedSMWrite + " id='chkWrite_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuModify'" + checkeboxCheckedSMModify + " id='chkModify_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuDelete'" + checkeboxCheckedSMDelete + " id='chkDelete_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td></tr>";
                                        countSubMenu++;
                                    }
                                }
                            }
                            htmlString = htmlString + "</table></fieldset></div></fieldset>";
                            countMenu++;
                        }
                    }
                }
                htmlString = htmlString + "</fieldset>";
            }
            return htmlString;
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public JsonResult GetRoles(int applicationID)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser != null)
            {
                var roleList = _menuMappingService.GetRolesByApplicationId(applicationID, loginUser.RoleMaster.RoleType);
                return Json(roleList, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Used for fetching Role data according to the Application ID and Login User Role Type. 
        /// </summary>
        /// <param name="appID">Application ID</param>
        /// <param name="roleType">Role Type of the Login User</param>
        /// <returns></returns>

        public JsonResult GetRolesByApplicationId([DataSourceRequest] DataSourceRequest request)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser != null)
            {
                var roleList = _menuMappingService.GetRolesByApplicationId(Convert.ToInt32(TempData["appID"]), loginUser.RoleMaster.RoleType);
                return Json(roleList, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save Menu Mapping Read Write Permissions
        /// </summary>
        /// <param name="lstMenuMapping"></param>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveMenuMapping(string appDetail, int roleId)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            List<MenuMappingDetail> lstMenuMappingDetail = new List<MenuMappingDetail>();
            MenuMappingDetail menuMappingDetail = new MenuMappingDetail();
            string[] MenuDetail = appDetail.Split(',');
            int count = 1;
            foreach (string str in MenuDetail)
            {
                string[] subMenuDetail;
                if (str != "undefined")
                {
                    if (count == 1)
                        menuMappingDetail = new MenuMappingDetail();
                    subMenuDetail = str.Split('_');
                    if (subMenuDetail.Length == 4)
                    {
                        menuMappingDetail.menuId = Convert.ToInt32(subMenuDetail[1].Substring(3));
                        menuMappingDetail.applicationId = Convert.ToInt32(subMenuDetail[2].Substring(5));
                        if (count < 5)
                        {
                            if (subMenuDetail[0] == "chkRead")
                            {
                                if (subMenuDetail[3] == "IsCheckedTrue")
                                    menuMappingDetail.IsRead = true;
                                else
                                    menuMappingDetail.IsRead = false;
                            }
                            if (subMenuDetail[0] == "chkWrite")
                            {
                                if (subMenuDetail[3] == "IsCheckedTrue")
                                    menuMappingDetail.IsWrite = true;
                                else
                                    menuMappingDetail.IsWrite = false;
                            }
                            if (subMenuDetail[0] == "chkModify")
                            {
                                if (subMenuDetail[3] == "IsCheckedTrue")
                                    menuMappingDetail.IsModify = true;
                                else
                                    menuMappingDetail.IsModify = false;
                            }
                            if (subMenuDetail[0] == "chkDelete")
                            {
                                if (subMenuDetail[3] == "IsCheckedTrue")
                                    menuMappingDetail.IsDelete = true;
                                else
                                    menuMappingDetail.IsDelete = false;
                            }
                        }
                        count++;
                    }
                    else
                    {
                        menuMappingDetail.menuId = Convert.ToInt32(subMenuDetail[1].Substring(4));
                        menuMappingDetail.applicationId = Convert.ToInt32(subMenuDetail[3].Substring(5));
                        if (count < 5)
                        {
                            if (subMenuDetail[0] == "chkRead")
                            {
                                if (subMenuDetail[4] == "IsCheckedTrue")
                                    menuMappingDetail.IsRead = true;
                                else
                                    menuMappingDetail.IsRead = false;
                            }
                            if (subMenuDetail[0] == "chkWrite")
                            {
                                if (subMenuDetail[4] == "IsCheckedTrue")
                                    menuMappingDetail.IsWrite = true;
                                else
                                    menuMappingDetail.IsWrite = false;
                            }
                            if (subMenuDetail[0] == "chkModify")
                            {
                                if (subMenuDetail[4] == "IsCheckedTrue")
                                    menuMappingDetail.IsModify = true;
                                else
                                    menuMappingDetail.IsModify = false;
                            }
                            if (subMenuDetail[0] == "chkDelete")
                            {
                                if (subMenuDetail[4] == "IsCheckedTrue")
                                    menuMappingDetail.IsDelete = true;
                                else
                                    menuMappingDetail.IsDelete = false;
                            }
                            count++;
                        }
                    }
                    if (count > 4)
                    {
                        lstMenuMappingDetail.Add(menuMappingDetail);
                        count = 1;
                    }
                }
            }
            lstMenuMappingDetail = lstMenuMappingDetail.Where(x => x.IsRead == true || x.IsWrite == true || x.IsModify == true || x.IsDelete == true).ToList();

            bool isDataInserted = _menuMappingService.SaveMenuMapping(lstMenuMappingDetail, roleId, loginUser.UserID);
            return Json(isDataInserted);
        }
    }
}