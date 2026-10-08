using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Service;
using NA.PMS.Web.Controllers;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Model;
using NA.PMS.UserManagement;

namespace NA.PMS.Web.Areas.Admin.Controllers
{
    public class OrganizationController : NoidaAdminController
    {

        //IManageUsersService _manageUsersService;
        //public OrganizationController(IManageUsersService manageUsersService)
        //{
        //    _manageUsersService = manageUsersService;
        //}

        //// GET: Admin/Organization
        //public ActionResult Index()
        //{
        //    return View();
        //}

        
        //public ActionResult ManageRoles()
        //{
        //    return View();
        //}

        //public ActionResult ManageUsers()
        //{
        //    return View();
        //}

        ///// <summary>
        ///// Used for reading Users Grid
        ///// </summary>
        ///// <param name="request">Kendo Grid internal parameter</param>
        ///// <returns></returns>
        //public ActionResult GetAllUsers([DataSourceRequest] DataSourceRequest request, int? applicationId, int? roleId)
        //{
        //    var loginUser = (CurrentUserDetail)Session["CurrentUser"];
        //    //var loginUser = new CurrentUserDetail();
        //    //loginUser.RoleType = "SA";
        //    if (loginUser != null)
        //    {
        //        //var users = _manageUsersService.GetAllUsers(request, loginUser.RoleMaster.RoleType, loginUser.UserID);
        //        List<UsersModel> users = _manageUsersService.GetUserListForSuperAdmin(applicationId, roleId);
        //        var data = users.ToDataSourceResult(request);
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// Returnd View for Add User screen.
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult AddUser()
        //{
        //    var user = new UsersModel();
        //    var lst = new List<CheckBoxListItem>();
        //    var subdepartmentlist = new List<CheckBoxViewModel> { new CheckBoxViewModel { Id = 1, CheckBoxId = 1, CheckBoxName = "Property", IsChecked = false }, new CheckBoxViewModel { Id = 2, CheckBoxId = 2, CheckBoxName = "Account", IsChecked = false } };
        //    lst = _manageUsersService.GetAllDeptts();
        //    user.depttIDs = lst;
        //    user.SubDepartmentList = subdepartmentlist;
        //    return View("AddUser", user);
        //}

        ///// <summary>
        ///// Used for adding/updating User
        ///// </summary>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public ActionResult SaveUser(UsersModel user)
        //{
        //    var loginUser = (CurrentUserDetail)Session["CurrentUser"];
        //    if (loginUser != null)
        //    {
        //        _manageUsersService.SaveUser(user, loginUser.UserID);
        //        TempData["MesgAdd"] = "true";
        //    }
        //    return RedirectToAction("ManageUsers");
        //}

        ///// <summary>
        ///// Retursn view for Edit User screen
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public ActionResult EditUser(int id)
        //{
        //    var user = _manageUsersService.GetUserDetailsById(id);
        //    var allDeptt = _manageUsersService.GetAllDeptts();
        //    var missing = allDeptt.Where(y => !user.depttIDs.Any(z => z.id == y.id)).ToList();
        //    user.depttIDs.AddRange(missing);
        //    user.depttIDs = user.depttIDs.OrderBy(x => x.id).ToList();
        //    //ViewBag.Page = "Update";
        //    //TempData["MesgEdit"] = "true";
        //    return View("EditUser", user);
        //}

        ///// <summary>
        ///// Returns view for View User screen
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public ActionResult ViewUser(int id)
        //{
        //    var user = _manageUsersService.GetUserDetailsById(id);
        //    return View("ViewUser", user);
        //}

        //public ActionResult ChangePassword()
        //{
        //    return View();
        //}

        ///// <summary>
        ///// Ajax call for Locking/Unlocking User
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="isActive"></param>
        ///// <returns></returns>
        //public JsonResult LockUnlockToggle(int id, bool isActive)
        //{
        //    var flag = false;
        //    flag = _manageUsersService.LockUnlockToggle(id, isActive);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// Checks for any existing Username that can be duplicate for the one enetered by the User
        ///// </summary>
        ///// <param name="empID"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public JsonResult CheckUsernameDuplicacy(string empID, string id)
        //{
        //    int refID = 0;
        //    if (!string.IsNullOrEmpty(id) && id.ToLower() != "undefined")
        //    {
        //        refID = Convert.ToInt32(id);
        //    }
        //    var data = !_manageUsersService.CheckUsernameDuplicacy(empID, refID);
        //    return Json(data);
        //}

        ///// <summary>
        ///// Checks for duplicacy of Mobile No. in DB
        ///// </summary>
        ///// <param name="mobile"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public JsonResult CheckMobileNoDuplicacy(string mobile, string id)
        //{
        //    int refID = 0;
        //    if (!string.IsNullOrEmpty(id) && id.ToLower() != "undefined")
        //    {
        //        refID = Convert.ToInt32(id);
        //    }
        //    var data = !_manageUsersService.CheckMobileNoDuplicacy(mobile, refID);
        //    return Json(data);
        //}

        ///// <summary>
        ///// Used for returning Map User screen View
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult MapUsers()
        //{
        //    var user = new UsersModel();
        //    user.roleId = Convert.ToInt32(Request.QueryString["rId"]);
        //    user.applicationID = Convert.ToInt32(Request.QueryString["appID"]);
        //    ViewBag.roleId = Convert.ToInt32(Request.QueryString["rId"]);
        //    ViewBag.appID = Convert.ToInt32(Request.QueryString["appID"]);
        //    return View(user);
        //}

        ///// <summary>
        ///// Grid read for searched Users from DB.
        ///// </summary>
        ///// <param name="request"></param>
        ///// <param name="empID"></param>
        ///// <returns></returns>
        //public ActionResult SearchUsers([DataSourceRequest] DataSourceRequest request, string empID)
        //{
        //    var loginUser = (CurrentUserDetail)Session["CurrentUser"];
        //    if (loginUser != null)
        //    {
        //        var users = _manageUsersService.SearchUsers(request, empID, loginUser.UserID);
        //        var data = users.ToDataSourceResult(request);
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //}

        ////public ActionResult GetAllUsersExcludingAssigned(DataSourceRequest request, int roleId)
        ////{
        ////    var users = _manageUsersService.SearchUsers(request, roleId);
        ////    var data = users.ToDataSourceResult(request);
        ////    return Json(data, JsonRequestBehavior.AllowGet);
        ////}

        ///// <summary>
        ///// Ajax Call to map User to Role.
        ///// </summary>
        ///// <param name="id">UserRefID</param>
        ///// <param name="roleID">Role ID</param>
        ///// <returns>(Json) 0 -> User is already attached to a Role (not Admin type) for the given Application; 1-> User added successfully; 2 -> User is already an Admin for the given Application</returns>
        //public JsonResult MapUserToRole(int id, int roleID)
        //{
        //    var loginUser = (CurrentUserDetail)Session["CurrentUser"];
        //    if (loginUser != null)
        //    {
        //        var result = _manageUsersService.MapUserToRole(id, roleID, loginUser.UserID);
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// Grid Read for all the Users assigned to a Role
        ///// </summary>
        ///// <param name="request"></param>
        ///// <param name="roleID"></param>
        ///// <returns></returns>
        //public ActionResult GetRemoveUsers([DataSourceRequest] DataSourceRequest request, int roleID)
        //{
        //    var users = _manageUsersService.GetRemoveUsers(request, roleID);
        //    var data = users.ToDataSourceResult(request);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// Ajax Call to remove Users associated with a Role 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public JsonResult RemoveUser(int userId, int roleId, int applicationId)
        //{
        //    var flag = false;
        //    flag = _manageUsersService.RemoveUser(userId, roleId, applicationId);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// Returns View for users grid
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult ViewAllUsers()
        //{
        //    return View();
        //}

        ///// <summary>e
        ///// Grid read function for ViewAllUsers page
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //public ActionResult ViewAllUsers_Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    var lstusers = _manageUsersService.GetEntireUsers(request);
        //    return Json(lstusers, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetAdminRoleDetails([DataSourceRequest] DataSourceRequest request, int userId)
        //{
        //    List<AdminRoleDetail> roles = _manageUsersService.GetAdminRoleDetails(userId);
        //    var obj = roles.ToDataSourceResult(request);
        //    return Json(obj, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetApplicationUsers()
        //{
        //    List<UsersModel> users = _manageUsersService.GetApplicationUsers();
        //    return Json(users, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetAllUsersSuper([DataSourceRequest] DataSourceRequest request)
        //{
        //    var loginUser = (CurrentUserDetail)Session["CurrentUser"];
        //    //var loginUser = new CurrentUserDetail();
        //    //loginUser.RoleType = "SA";
        //    if (loginUser != null)
        //    {
        //        var users = _manageUsersService.GetAllUsersSuper(request, loginUser.RoleMaster.RoleType, loginUser.UserID);

        //        var data = users.ToDataSourceResult(request);
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //}
        ///// <summary>
        ///// check email duplicacy
        ///// </summary>
        ///// <param name="email"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public JsonResult CheckEmailDuplicacy(string email, string id)
        //{
        //    int refID = 0;
        //    //  var address;
        //    if (!string.IsNullOrEmpty(id) && id.ToLower() != "undefined")
        //    {
        //        refID = Convert.ToInt32(id);
        //    }
        //    //if (email!=null)
        //    //{
        //    //    email = new MailAddress(email).Address;
        //    //}
        //    var data = !_manageUsersService.CheckEmailDuplicacy(email, refID);
        //    return Json(data);
        //}

        //public JsonResult GetDepartmentListByIdAsDataSource([DataSourceRequest] DataSourceRequest request, UsersModel model)
        //{
        //    var list = _manageUsersService.GetDepartmentListByIdAsDataSource(request, model);
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}
    }
}