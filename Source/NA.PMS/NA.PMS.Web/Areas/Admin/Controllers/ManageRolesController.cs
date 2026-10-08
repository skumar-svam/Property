using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Web.Controllers;
using NA.PMS.Model;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Service;
using NA.PMS.Web.Models;
using NA.PMS.Common;


namespace NA.PMS.Web.Areas.Admin.Controllers
{
    public class ManageRolesController : WebBaseController
    {
        IManageRolesService _manageRolesService;
        public ManageRolesController(IManageRolesService manageRolesService)
        {
            _manageRolesService = manageRolesService;
        }

        public ActionResult ManageRoles()
        {
            return View();
        }

        public ActionResult GetAllRoles(DataSourceRequest req)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            List<RolesModel> data = null;
            if (loginUser != null)
                data = _manageRolesService.GetAllRoles(loginUser.RoleMaster.RoleType);//.ToDataSourceResult(req);
            return Json(data.ToDataSourceResult(req), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserRolesToMappedByUserId([DataSourceRequest]DataSourceRequest req)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            DataSourceResult data = null;
            if (loginUser != null)
                data = _manageRolesService.GetUserRolesToMappedByUserId(req, loginUser.UserID).ToDataSourceResult(req);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddRole(RolesModel roleModel)
        {
            bool flag = false;
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser.RoleMaster.RoleType == null || loginUser.UserID == 0)
            {
                return View(roleModel);
            }
            if (ModelState.IsValid)
            {
                //flag = _manageRolesService.AddRole(roleModel, loginUser.RoleMaster.RoleType, loginUser.UserID);
                flag = _manageRolesService.SaveUserRole(roleModel);
                TempData["MesgAdd"] = flag;
            }
            if (flag)
            {
                return RedirectToAction("ManageRoles", "ManageRoles");
            }
            else
            {
                return View(roleModel);
            }

        }

        public ActionResult EditRole(int id)
        {
            RolesModel rmodel = new RolesModel();
            if (id != 0)
            {
                rmodel = _manageRolesService.GetRecordById(id);
            }
            return View(rmodel);
        }
        [HttpPost]
        public ActionResult EditRole(RolesModel rmodel)
        {
            bool flag = false;
            if (ModelState.IsValid)
            {
                //flag = _manageRolesService.UpdateRecordById(rmodel, ((CurrentUserDetail)Session["CurrentUser"]).RoleMaster.RoleType, ((CurrentUserDetail)Session["CurrentUser"]).UserID);
                flag = _manageRolesService.SaveUserRole(rmodel);
                TempData["MesgEdit"] = flag;
            }
            if (flag)
            {
                return RedirectToAction("ManageRoles", "ManageRoles");
            }
            else
            {
                return View(rmodel);
            }
        }

        public ActionResult ViewDetails(int id)
        {
            RolesModel rmodel = new RolesModel();
            if (id != 0)
            {
                rmodel = _manageRolesService.GetRecordById(id);
            }
            return View(rmodel);
        }

        public JsonResult GetApplications()
        {
            var currentUserDetails = (CurrentUserDetail)Session["CurrentUser"];
            List<ApplicationModel> lstApplication = null;
            if (currentUserDetails != null)
            {
                lstApplication = currentUserDetails.RoleMaster.RoleType == Constants.SuperAdmin ? _manageRolesService.GetApplicationListBySA() : _manageRolesService.GetAllApplicationList(currentUserDetails.UserID);
                //lstApplication = _manageRolesService.GetApplicationListBySA();
            }
            return Json(lstApplication, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReadApplicationDataType()
        {
            var currentUserType = (CurrentUserDetail)Session["CurrentUser"];
            if (currentUserType != null && currentUserType.RoleMaster.RoleType == Constants.SuperAdmin)
            {
                List<ApplicationModel> lstApplication = _manageRolesService.GetApplicationListBySA();
                return Json(lstApplication, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<ApplicationModel> lstApplication = _manageRolesService.GetAllApplicationList(currentUserType.UserID);
                return Json(lstApplication, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            bool flag = false;
            if (id != 0)
            {
                flag = _manageRolesService.DeleteRecordById(id);
            }
            return RedirectToAction("ManageRoles", "ManageRoles");
        }
        // Activate/ Deactivate roles
        //public Boolean DeActivateRole(int roleId, bool status, string viewName)
        //{
        //    return _manageRolesService.DeActivate(roleId, status, viewName, ((CurrentUserDetail)Session["CurrentUser"]).UserID);
        //}

        public JsonResult DeActivateRole(int roleId, bool status, string viewN)
        {
            var flag = _manageRolesService.DeActivate(roleId, status, viewN, ((CurrentUserDetail)Session["CurrentUser"]).UserID);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult IsRoleNameUnique(string roleName, int applicationId, int roleId = 0)
        {
            bool flag = false;

            flag = _manageRolesService.IsRoleNameUnique(roleName, roleId, applicationId);

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        //[AllowAnonymous]
        //public JsonResult IsRoleNameUnique(RolesModel roleModel)
        //{
        //    bool flag = false;
        //    if (roleModel != null)
        //    {
        //        flag = _manageRolesService.IsRoleNameUnique(roleModel.RoleName, roleModel.RoleId, roleModel.ApplicationId.Value);
        //    }
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}
    }
}