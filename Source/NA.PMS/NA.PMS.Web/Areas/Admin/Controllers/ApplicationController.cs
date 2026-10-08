using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Model;
using NA.PMS.Service;
using NA.PMS.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Web.Models;
using NA.PMS.Common;
using NA.PMS.Web.Filters;

namespace NA.PMS.Web.Areas.Admin.Controllers
{
    //[AuthorityRole]
    public class ApplicationController : WebBaseController
    {
        IGeneralService _generalService;
        IManageUsersService _userService;
        IManageRolesService _roleService;
        IMenuMappingService _menuService;
        public ApplicationController(IGeneralService generalService,IManageUsersService userService, IManageRolesService roleService, IMenuMappingService menuService)
        {
            _generalService = generalService;
            _userService = userService;
            _roleService = roleService;
            _menuService = menuService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageUsers()
        {
            return View();
        }

        public ActionResult ManageCustomers()
        {
            return View();
        }

        public ActionResult UpdateInfo()
        {
            return View();
        }

        /// <summary>
        /// Get all users of applications whose admin has
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult GetApplicationUsersListForAdmin([DataSourceRequest]DataSourceRequest request)
        {
            CurrentUserDetail user = (CurrentUserDetail)Session["CurrentUser"];
            List<UsersModel> usersList = _userService.GetApplicationUsersListForAdmin();
            
            var data = usersList.ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get users of same application based on application id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult GetApplicationUsersByApplicationId([DataSourceRequest]DataSourceRequest request)
        {
            CurrentUserDetail user = (CurrentUserDetail)Session["CurrentUser"];
            List<UsersModel> usersList = new List<UsersModel>();

            //foreach (var apps in user.ApplicationMaster)
            //{
            //    usersList = _userService.GetApplicationUsersListForAdmin(apps.ApplicationId);
            //}
            usersList = _userService.GetApplicationUsersListForAdmin(user.ApplicationMaster);
            var data = usersList.ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Lock and unlock user for an application
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isActive"></param>
        /// <param name="roleId"></param>
        /// <param name="applicationID"></param>
        /// <returns></returns>
        public JsonResult LockUnlockRoleToggle(int id, bool isActive, int roleId,int applicationID)
        {
            var flag = false;
            flag = _userService.LockUnlockRoleToggle(id, isActive, roleId, applicationID);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewUser(int id)
        {
            var user = _userService.GetUserDetailsById(id);
            return View("ViewUser", user);
        }
        /// <summary>
        /// Map user(existing/new) to role of a particular application
        /// </summary>
        /// <returns></returns>
        public ActionResult MapApplicationsRole() 
        {
            var user = new UsersModel();
            user.roleId = Convert.ToInt32(Request.QueryString["rId"]);
            user.applicationID = Convert.ToInt32(Request.QueryString["appID"]);
            ViewBag.roleId = Convert.ToInt32(Request.QueryString["rId"]);
            ViewBag.appID = Convert.ToInt32(Request.QueryString["appID"]);
            return View(user);
            //return View();
        }
        /// <summary>
        /// Get all application for dropdown
        /// </summary>
        /// <returns></returns>
        public JsonResult GetApplications()
        {
            var currentUserDetails = (CurrentUserDetail)Session["CurrentUser"];
            List<ApplicationModel> lstApplication = null;
            if (currentUserDetails != null)
            {
                lstApplication = currentUserDetails.RoleMaster.RoleType == Constants.SuperAdmin ? _roleService.GetApplicationListBySA() : _roleService.GetAllApplicationList(currentUserDetails.UserID);
            }
            return Json(lstApplication, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public JsonResult GetRoles([DataSourceRequest] DataSourceRequest request, int applicationID)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser != null)
            {
                var roleList = _menuService.GetRolesByApplicationId(applicationID, loginUser.RoleMaster.RoleType);
                return Json(roleList, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetApplicationUsers()
        {
            List<UsersModel> users = _userService.GetApplicationUsers();
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Grid read for searched Users from DB.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="empID"></param>
        /// <returns></returns>
        public ActionResult SearchUsers(DataSourceRequest request, string empID)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser != null)
            {
                var users = _userService.SearchUsers(request, empID, loginUser.UserID);
                var data = users.ToDataSourceResult(request);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Used for reading Users Grid
        /// </summary>
        /// <param name="request">Kendo Grid internal parameter</param>
        /// <returns></returns>
        public ActionResult GetAllUsers([DataSourceRequest] DataSourceRequest request)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            //var loginUser = new CurrentUserDetail();
            //loginUser.RoleType = "SA";
            if (loginUser != null)
            {
                var users = _userService.GetAllUsers(request, loginUser.RoleMaster.RoleType, loginUser.UserID);

                var data = users.ToDataSourceResult(request);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Ajax Call to map User to Role.
        /// </summary>
        /// <param name="id">UserRefID</param>
        /// <param name="roleID">Role ID</param>
        /// <returns>(Json) 0 -> User is already attached to a Role (not Admin type) for the given Application; 1-> User added successfully; 2 -> User is already an Admin for the given Application</returns>
        public JsonResult MapUserToRole(int id, int roleID)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser != null)
            {
                var result = _userService.MapUserToRole(id, roleID, loginUser.UserID);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Ajax Call to remove Users associated with a Role 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult RemoveUser(int userId, int roleId, int applicationId)
        {
            var flag = false;
            flag = _userService.RemoveUser(userId, roleId, applicationId);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Grid Read for all the Users assigned to a Role
        /// </summary>
        /// <param name="request"></param>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public ActionResult GetRemoveUsers(DataSourceRequest request, int roleID)
        {
            var users = _userService.GetRemoveUsers(request, roleID);
            var data = users.ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //view page for audit trail
        public ActionResult AuditAction()
        {
            return View();
        }
        //return all logged application activity
        public JsonResult GetActionAuditTrail([DataSourceRequest]DataSourceRequest request)
        {
            List<AuditActionModel> audit = _userService.GetActionAuditTrail();
            var data = audit.ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        // return all logged application activity between two dates
        public JsonResult GetActionAuditDetailByDate([DataSourceRequest]DataSourceRequest request, DateTime startDate, DateTime endDate)
        {
            List<AuditActionModel> audit = _userService.GetActionAuditDetailByDate(startDate, endDate);
            var data = audit.ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAuditTrailActionGroupByModuleName([DataSourceRequest]DataSourceRequest request, string moduleName)
        {
            List<AuditActionModel> audit = _userService.GetAuditTrailActionGroupByModuleName(moduleName);
            var data = audit.ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAuditModuleName()
        {
            List<DDList> audit = _userService.GetAuditModuleName();
            //var data = audit.ToDataSourceResult(request);
            return Json(audit, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAuditActionName(string moduleName)
        {
            List<DDList> audit = _userService.GetAuditActionName(moduleName);
            //var data = audit.ToDataSourceResult(request);
            return Json(audit, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAuditUserName(string moduleName, string actionName)
        {
            List<DDList> audit = _userService.GetAuditUserName(moduleName,actionName);
            //var data = audit.ToDataSourceResult(request);
            return Json(audit, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAuditTrailByAdvanceSearch([DataSourceRequest]DataSourceRequest request, string moduleName, string actionName, string userName, DateTime? startDate, DateTime? endDate)
        {
            List<AuditActionModel> audit = _userService.GetAuditTrailByAdvanceSearch(moduleName, actionName, userName, startDate, endDate);
            var data = audit.ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAuditActionDetailByAdvanceSearch([DataSourceRequest]DataSourceRequest request, AuditViewModel model)
        {
            var audit = _userService.GetAuditActionDetailByAdvanceSearch(request, model);
            return Json(audit, JsonRequestBehavior.AllowGet);
        }


        //Property Transaction Audit
        public ActionResult PropertyTransactionAudit()
        {
            return View();
        }

        public JsonResult GetRidOfPropertyTransaction()
        {
            List<DynamicDataModel> data = _userService.GetRidOfPropertyTransaction();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetServiceNameOfPropertyTransaction()
        {
            List<DynamicDataModel> data = _userService.GetServiceNameOfPropertyTransaction();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserOfPropertyTransaction()
        {
            List<DynamicDataModel> data = _userService.GetUserOfPropertyTransaction();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTransactionAuditHistory([DataSourceRequest]DataSourceRequest request, int? rid, string serviceName, int? user, DateTime? startDate, DateTime? endDate)
        {
            List<PropertyTransactionAudit> transaction = _userService.GetTransactionAuditHistory(rid, serviceName,  user,  startDate,  endDate);
            var data = transaction.ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMappedUsersForApplication([DataSourceRequest]DataSourceRequest request, int applicationId, int? roleId)
        {
            List<UsersModel> users = _userService.GetMappedUsersForApplication(applicationId, roleId);
            var data = users.ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApplicationList()
        {
            var list = _generalService.GetApplicationList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMenuList(int? parentId)
        {
            var list = _generalService.GetMenuList(parentId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetParentMenuList()
        {
            var list = _generalService.GetParentMenuList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAreaRangeList(int departmentId)
        {
            var list = _generalService.GetAreaRangeList(departmentId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyTypeByDepartment(int departmentId)
        {
            var list = _generalService.GetPropertyTypeByDepartment(departmentId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCommonConfigCategoryList()
        {
            var list = _generalService.GetCommonConfigCategoryList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCommonConfigDataList(string category)
        {
            var list = _generalService.GetCommonConfigDataList(category);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatusList()
        {
            var list = _generalService.GetStatusList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserStatusListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetUserStatusListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatusListByType(string type)
        {
            var list = _generalService.GetStatusListByType(type);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSchemeTypeList()
        {
            var list = _generalService.GetSchemeTypeList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentList()
        {
            var list = _generalService.GetDepartmentList(); 
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetDepartmentListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatusTypeList()
        {
            var list = _generalService.GetStatusTypeList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveApplication(CommonViewModel model)
        {
            int flag = _generalService.SaveApplication(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSchemeType(CommonViewModel model)
        {
            int flag = _generalService.SaveSchemeType(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePropertyType(CommonViewModel model)
        {
            int flag = _generalService.SavePropertyType(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePropertyAreaRange(CommonViewModel model)
        {
            int flag = _generalService.SavePropertyAreaRange(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveApplicationMenu(CommonViewModel model)
        {
            int flag = _generalService.SaveApplicationMenu(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveStatusByType(CommonViewModel model)
        {
            int flag = _generalService.SaveStatusByType(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveCommonConfigData(CommonViewModel model)
        {
            int flag = _generalService.SaveCommonConfigData(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCommonConfigDataById(CommonViewModel model)
        {
            CommonViewModel cmodel = _generalService.GetCommonConfigDataById(model);
            return Json(cmodel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDepartment(CommonViewModel model)
        {
            int flag = _generalService.SaveDepartment(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyTypeDetailById(CommonViewModel model)
        {
            var flag = _generalService.GetPropertyTypeDetailById(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyCustomersList([DataSourceRequest]DataSourceRequest request, UserViewModel model)
        {
            var list = _userService.GetPropertyCustomersList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyCustomersListById([DataSourceRequest]DataSourceRequest request, string username)
        {
            var list = _userService.GetPropertyCustomersList(request, new UserViewModel { UserName = username});
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateCustomerStatus(UserViewModel model)
        {
            int flag = _userService.UpdateCustomerStatus(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAuditSearchParameter([DataSourceRequest]DataSourceRequest request, AuditViewModel model)
        {
            var data = _userService.GetAuditSearchParameter(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserNameListAsDataSource([DataSourceRequest]DataSourceRequest request, DropdownViewModel model)
        {
            var data = _userService.GetUserNameListAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsersDetailById(UserViewModel model)
        {
            UserViewModel user = _userService.GetUsersDetailById(model);
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveUserDetailById(UserViewModel model)
        {
            int flag = _userService.SaveUserDetailById(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDeparmentListAsDataSource([DataSourceRequest]DataSourceRequest request, DropdownViewModel model)
        {
            var list = _generalService.GetUsersDepartmentListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RegisterCustomer(UserViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int flag = _userService.RegisterCustomerDetails(model, files);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}