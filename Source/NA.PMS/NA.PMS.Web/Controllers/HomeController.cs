using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Web.Controllers;
using NA.PMS.Service;
using NA.PMS.Web.Filters;
using NA.PMS.Model;
using NA.PMS.Web.Models;
using NA.PMS.Common;
using System.Configuration;

namespace NA.PMS.Web.Controllers
{
    public class HomeController : Controller
    {

        IManageUsersService _manageUsersService;
        IGeneralService _generalservice;
        CurrentUserDetail currentUserDetail = null; // Keshav
        public HomeController(IManageUsersService manageUsersService, IGeneralService generalservice)
        {
            _manageUsersService = manageUsersService;
            _generalservice = generalservice;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region GetRecord User Keshav 14 Sep 2018
        public JsonResult GetRecord(string Application)
        {
            int ApplicationId = 0;
            // CurrentUserDetail currentUserDetail = null;
            currentUserDetail = (CurrentUserDetail)Session["CurrentUser"];

            var ApplicationID = currentUserDetail.ApplicationMaster.ToList().Where(i => i.ApplicationName == Application).SingleOrDefault(); //currentUserDetail.ApplicationMaster[0].ApplicationId
            if (ApplicationID != null)
            {
                ApplicationId = ApplicationID.ApplicationId;
            }
            if (ApplicationID != null)
            {
                ApplicationId = ApplicationID.ApplicationId;
            }
            //ViewBag.ApplicationID = ApplicationId;
            //ViewBag.UserName = currentUserDetail.UserName;
            var result = new { applicationID = ApplicationID, userName = currentUserDetail.UserName };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LMS()
        {

            string returnUrl = string.Empty;
            returnUrl = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["LMSurl"]) ? ConfigurationManager.AppSettings["LMSurl"] : string.Empty;
            int ApplicationId = 0;
            //CurrentUserDetail currentUserDetail = null;
            currentUserDetail = (CurrentUserDetail)Session["CurrentUser"];
            var ApplicationID = currentUserDetail.ApplicationMaster.ToList().Where(i => i.ApplicationName == "General Administration").SingleOrDefault(); //currentUserDetail.ApplicationMaster[0].ApplicationId


            if (ApplicationID != null)
            {
                ApplicationId = ApplicationID.ApplicationId;
                // returnUrl = "http://52.172.156.142:5051/?Uname=" + currentUserDetail.UserName + "&ApplicationID=" + ApplicationId;
                returnUrl = returnUrl + "?Uname=" + currentUserDetail.UserName + " & ApplicationID=" + ApplicationId;
                return Redirect(returnUrl);
            }
            else
            {

                return RedirectToAction("Index");
            }
        }

        public ActionResult PS()
        {
            string returnUrl = string.Empty;
            returnUrl = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["PMSurl"]) ? ConfigurationManager.AppSettings["PMSurl"] : string.Empty;


            int ApplicationId = 0;
            //CurrentUserDetail currentUserDetail = null;
            currentUserDetail = (CurrentUserDetail)Session["CurrentUser"];
            var ApplicationID = currentUserDetail.ApplicationMaster.ToList().Where(i => i.ApplicationName == "Property Management System").SingleOrDefault(); //currentUserDetail.ApplicationMaster[0].ApplicationId


            if (ApplicationID != null)
            {
                ApplicationId = ApplicationID.ApplicationId;
                // returnUrl = "http://192.168.3.36:4040/?Uname=" + currentUserDetail.UserName + "&ApplicationID=" + ApplicationId;
                returnUrl = returnUrl + "?Uname=" + currentUserDetail.UserName + " & ApplicationID=" + ApplicationId;
                return Redirect(returnUrl);
            }
            else
            {

                return RedirectToAction("Index");
            }
        }

        public ActionResult MainScreen()
        {
            bool Flag = false;
            currentUserDetail = (CurrentUserDetail)Session["CurrentUser"];
            var iDforPMS = currentUserDetail.ApplicationMaster.ToList().Where(i => i.ApplicationName == Application.PMS).ToList(); //currentUserDetail.ApplicationMaster[0].ApplicationId
            var ids = currentUserDetail.ApplicationMaster.ToList().Where(i => i.ApplicationName != Application.PMS).ToList(); //currentUserDetail.ApplicationMaster[0].ApplicationId

            if (iDforPMS.Count != 0 && ids.Count == 0)
            {
                Flag = true;
            }
            List<ApplicationMaster> applicationsList;
            applicationsList = (List<ApplicationMaster>)Session["UserApplication"];
            ViewBag.MenuData = applicationsList.ToList();
            var userName = currentUserDetail.UserName;
            //var encryptedPassword = NA.PMS.Common.Extension.StringExtensions.ToMD5HashForPassword(userName);
            var encryptedPassword = NA.PMS.Common.Extension.StringExtensions.encrypt(userName);
            ViewBag.UserName = encryptedPassword;
            ViewBag.Password = currentUserDetail.Password;
            if (Flag == true)
            {
                if (currentUserDetail.RoleMaster.RoleInDepartment == RoleInDepartment.Accountant)
                {
                    return RedirectToAction("Index", new { controller="Finance", area="Revenue"});
                }
                else
                {
                    return RedirectToAction("Index");
                }
               
            }
            else
            {
                return View();
            }
        }
        #endregion
        public ActionResult UnauthorizedAccess()
        {
            return View();
        }

        public ActionResult ForAllotmentMaster(AdvanceSearchModel model)
        {
            if (model.FilterType == "Accountant")
            {
                Session["AdvanceSearchAccount"] = model;
                return RedirectToAction("ManagePropertyCost", "Finance", new { area = "Revenue" });
            }
            else
            {
                Session["AdvanceSearch"] = model;
                return RedirectToAction("AllotmentMaster", "PropertyAllotment", new { area = "Property" });
            }
        }

        public ActionResult Dashboard()
        {
            var data = _generalservice.GetDashboardPropertyData(0);
            return View(data);
        }
    }
}