using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NA.PMS.Service;
using NA.PMS.Web.Areas.Customer.Controllers;
using NA.PMS.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NA.PMS.Web.Areas.Admin.Controllers
{
    public class DashboardController : PISWebBaseController
    {
        IGraphService _graphService;
        public DashboardController(IGraphService graphService)
        {
            _graphService = graphService;
        }

        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            if (Session["RoleId_PIS"].ToString().Trim() == "5")
            {
                return RedirectToAction("ManageUsers", "Dashboard");
            }
            else
            {
                return RedirectToAction("AdminDashboard", "Dashboard");
            }
        }

        public ActionResult AdminDashboard()
        {
            return View();
        }

        public ActionResult ManageUsers()
        {
            return View();
        }

        [AjaxHandleException]
        public ActionResult ManageUsersAjax([DataSourceRequest] DataSourceRequest request)
        {
            var userList = _graphService.GetUsers();
            userList = userList.Where(ul => ul.RoleName.ToLower() == NA.PMS.Common.Constants.Roles.Administrator.ToString().ToLower()).OrderBy(ul => ul.Status);
            DataSourceResult result = userList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AjaxHandleException]
        public ActionResult ManageCustomersAjax([DataSourceRequest] DataSourceRequest request)
        {
            var userList = _graphService.GetUsers();
            userList = userList.Where(ul => ul.RoleName.ToLower() == NA.PMS.Common.Constants.Roles.Customer.ToString().ToLower()).OrderByDescending(ul => ul.CreatedOn);
            DataSourceResult result = userList.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendPassword(string email, string mobileNo, string userId)
        {
            var flag = false;
            if (!string.IsNullOrEmpty(userId))
            {
                flag = _graphService.SendPassword(email, mobileNo, userId);
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LockUnLockCustomer(string email)
        {
            var flag = false;
            flag = _graphService.LockUnLockCustomer(email);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ResetPassword(string email)
        {
            var flag = false;
            flag = _graphService.ResetPassword(email);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeactivateUser(string email)
        {
            var flag = false;
            flag = _graphService.DeactivateUser(email);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RejectCustomer(string email, string mobileNo, string remarks)
        {
            var flag = false;
            flag = _graphService.RejectCustomer(email, mobileNo, remarks);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}