using Kendo.Mvc.UI;
using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Service;
using NA.PMS.Web.Controllers;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using NA.PMS.Web.Filters;

namespace NA.PMS.Web.Areas.SuperAdmin.Controllers
{
    [AuthoritySuperAdmin("SA")]
    public class ManageAdminRolesController : WebBaseController
    {
        
        IManageRolesService _manageRolesService;
        public ManageAdminRolesController(IManageRolesService manageRolesService)
        {
            _manageRolesService = manageRolesService;
        }

        public ActionResult ManageAdminRoles()
        {
            return View();
        }

        public ActionResult GetAllRoles([DataSourceRequest] DataSourceRequest req)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            var data = _manageRolesService.GetAllRoles(loginUser.RoleMaster.RoleType).ToDataSourceResult(req);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddAdminRole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAdminRole(RolesModel roleModel)
        {
            bool flag = false;
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser.RoleMaster.RoleType == null || loginUser.UserID == 0)
            {
                return View(roleModel);
            }

            if (ModelState.IsValid)
            {
                if (!_manageRolesService.IsRoleNameUnique(roleModel.RoleName, roleModel.RoleId,
                    roleModel.ApplicationId.Value))
                {                    
                    flag = _manageRolesService.AddRole(roleModel, loginUser.RoleMaster.RoleType, loginUser.UserID);
                    TempData["MesgAdd"] = flag;
                }
                else
                {
                    ModelState.AddModelError("RoleName", "Role for the selected application has been already assigned.");
                    flag = false;
                    TempData["MesgAdd"] = null;
                }

            }
            if (flag)
            {
                return RedirectToAction("ManageAdminRoles", "ManageAdminRoles");
            }
            else
            {
                return View(roleModel);
            }

        }

        public ActionResult EditAdminRole(int id)
        {
            RolesModel rmodel = new RolesModel();
            if (id != 0)
            {
                rmodel = _manageRolesService.GetRecordById(id);
            }
            return View(rmodel);
        }

        public ActionResult GetRecordById(int id)
        {
            var roleModel = new RolesModel();
            if (id != 0)
            {
                roleModel = _manageRolesService.GetRecordById(id);
            }
            return Json(roleModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRecordByApplicationId(int id)
        {
            var roleModel = new RolesModel();
            if (id != 0)
            {
                var roleModelApp = _manageRolesService.GetRecordByApplicationId(id);
                if (roleModelApp != null)
                    roleModel = roleModelApp;
            }
            return Json(roleModel, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult EditAdminRole(RolesModel rmodel)
        {
            bool flag = false;
            if (ModelState.IsValid)
            {
                flag = _manageRolesService.UpdateRecordById(rmodel, ((CurrentUserDetail)Session["CurrentUser"]).RoleMaster.RoleType, ((CurrentUserDetail)Session["CurrentUser"]).UserID);
                TempData["MesgEdit"] = flag;
            }

            if (flag)
            {
                return RedirectToAction("ManageAdminRoles", "ManageAdminRoles");
            }
            else
            {
                return View(rmodel);
            }
        }

        public ActionResult ViewAdminRole(int id)
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
            }
            return Json(lstApplication, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                _manageRolesService.DeleteRecordById(id);
            }
            return RedirectToAction("ManageAdminRoles", "ManageAdminRoles");
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult IsRoleNameUnique(string roleName, int roleId, int applicationId)
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
        // Activate/ Deactivate roles
        public Boolean DeActivateRole(int roleId, bool status, string viewName)
        {
            return _manageRolesService.DeActivate(roleId, status, viewName, ((CurrentUserDetail)Session["CurrentUser"]).UserID);
        }
    }
}