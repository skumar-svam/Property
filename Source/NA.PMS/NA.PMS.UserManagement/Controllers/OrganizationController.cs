using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using NA.PMS.Model;
using System.Web;
using NA.PMS.UserManagement;

namespace NA.PMS.Web.Areas.Admin.Controllers
{
    public class OrganizationController : Controller
    {
        IAdminRepository _adminRepository;
        public OrganizationController()
        {
            _adminRepository = new AdminRepository();
        }

        // GET: Admin/Organization
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ManageRoles()
        {
            return View();
        }

        public ActionResult ManageEmployee()
        {
            return View();
        }

        public ActionResult ManageCustomer()
        {
            return View();
        }

        public ActionResult ManageApplication()
        {
            return View();
        }

        public ActionResult ManageMenu()
        {
            return View();
        }

        public ActionResult ManageMasterData()
        {
            return View();
        }

        public ActionResult ManageDepartment()
        {
            return View();
        }

        public ActionResult ManageStatusMaster()
        {
            return View();
        }

        public ActionResult ManageSectorAndBlock()
        {
            return View();
        }

        public ActionResult ManagePropertyType()
        {
            return View();
        }

        public ActionResult ManageAreaRange()
        {
            return View();
        }

        public ActionResult ManageSchemeType()
        {
            return View();
        }

        public ActionResult ManageOnlineScheme()
        {
            return View();
        }

        public ActionResult ManageCommonConfig()
        {
            return View();
        }

        public ActionResult Employee(NDAUserViewModel model)
        {
            if (model != null && model.UserRefId != null && model.UserRefId > 0)
            {
                model = _adminRepository.GetAuthorityUserDetailById(model);
                return View(model);
            }
            else
            {
                var user = new NDAUserViewModel();
                var lst = new List<NDACheckBoxViewModel>();
                var subdepartmentlist = new List<NDACheckBoxViewModel> {
                new NDACheckBoxViewModel { Id = 1, CheckBoxId = 1, CheckBoxName = "Property", IsChecked = false }, 
                new NDACheckBoxViewModel { Id = 2, CheckBoxId = 2, CheckBoxName = "Account", IsChecked = false } 
            };
                lst = _adminRepository.GetDepartmentList();
                user.DepartmentList = lst;
                user.SubDepartmentList = subdepartmentlist;
                return View(user);
            }
        }

        public ActionResult Customer(NDAUserViewModel model)
        {
            if (model != null && !string.IsNullOrEmpty(model.UserName))
            {
                var user = _adminRepository.GetCustomerDetailById(model);
                return View(user);
            }
            else
            {
                return View();
            }
        }

        public ActionResult RoleUsersMapping(NDAUserViewModel model)
        {
            if (model != null)
            {
                model.RoleId = Convert.ToInt32(Request.QueryString["RoleId"]);
                model.ApplicationId = Convert.ToInt32(Request.QueryString["ApplicationId"]);
                return View(model);
            }
            else
            {
                return View();
            }
        }

        public ActionResult RoleMenuMapping(NDARoleViewModel model)
        {
            if (model != null)
            {
                return View(model);
            }
            else
            {
                return View();
            }
            //Model.MenuMapping objMenuMapping = new Model.MenuMapping();
            ////objMenuMapping.htmlString = GetHtmlString(rId, appID);
            //objMenuMapping.applicationId = appID;
            //objMenuMapping.roleId = rId;
            //TempData["appID"] = appID;
            //TempData["rID"] = rId;
            //return View(objMenuMapping);
        }

        public JsonResult GetMenuListByApplicationRoleAsHtmlContent(NDARoleViewModel model)
        {
            model = _adminRepository.GetMenuListByApplicationRoleAsHtmlContent(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUsersAsDataSource([DataSourceRequest] DataSourceRequest request, NDAUserViewModel model)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            //var loginUser = new CurrentUserDetail();
            //loginUser.RoleType = "SA";
            if (loginUser != null)
            {
                DataSourceResult users = _adminRepository.GetUsersAsDataSource(request, model);
                //List<UsersModel> users = _adminRepository.GetUserListForSuperAdmin(applicationId, roleId);
                //var data = users.ToDataSourceResult(request);
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomerListAsDataSource([DataSourceRequest] DataSourceRequest request, NDAUserViewModel model)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            //var loginUser = new CurrentUserDetail();
            //loginUser.RoleType = "SA";
            if (loginUser != null)
            {
                DataSourceResult users = _adminRepository.GetCustomerListAsDataSource(request, model);
                //List<UsersModel> users = _adminRepository.GetUserListForSuperAdmin(applicationId, roleId);
                //var data = users.ToDataSourceResult(request);
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRoleListAsDataSource([DataSourceRequest] DataSourceRequest request, NDARoleViewModel model)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser != null)
            {
                DataSourceResult users = _adminRepository.GetRoleListAsDataSource(request, model);
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetApplicationListAsDataSource([DataSourceRequest] DataSourceRequest request, NDARoleViewModel model)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser != null)
            {
                DataSourceResult users = _adminRepository.GetApplicationListAsDataSource(request, model);
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMenuListAsDataSource([DataSourceRequest] DataSourceRequest request, NDARoleViewModel model)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser != null)
            {
                DataSourceResult users = _adminRepository.GetMenuListAsDataSource(request, model);
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveAuthorityEmployee(NDAUserViewModel user)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser != null)
            {
                user = _adminRepository.SaveAuthorityUserDetail(user);
            }
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveEmployeeInfoByActionType(NDAUserViewModel user)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser != null)
            {
                user = _adminRepository.SaveEmployeeInfoByActionType(user);
            }
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveAuthorityCustomer(NDAUserViewModel user, HttpPostedFileBase userIdFile, HttpPostedFileBase propertyFile)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser != null)
            {
                user = _adminRepository.SaveAuthorityCustomer(user, userIdFile, propertyFile);
            }
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveCustomerInfoByActionType(NDAUserViewModel user)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser != null)
            {
                user = _adminRepository.SaveCustomerInfoByActionType(user);
            }
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateUserDetail(NDAUserViewModel model)
        {
            model = _adminRepository.ValidateUserDetail(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MapUserToRoleAndApplication(NDAUserViewModel model)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser != null)
            {
                model = _adminRepository.MapUserToRoleAndApplication(model);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentListByIdAsDataSource([DataSourceRequest] DataSourceRequest request, NDAUserViewModel model)
        {
            DataSourceResult list = _adminRepository.GetDepartmentListByIdAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApplicationRoleDetailByIdAsDataSource([DataSourceRequest] DataSourceRequest request, NDARoleViewModel model)
        {
            DataSourceResult list = _adminRepository.GetApplicationRoleDetailByIdAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMasterDataListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            DataSourceResult list = _adminRepository.GetMasterDataListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegistrationIdListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            DataSourceResult list = _adminRepository.GetMasterDataListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSectorListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            DataSourceResult list = _adminRepository.GetMasterDataListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBlockListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            DataSourceResult list = _adminRepository.GetMasterDataListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSecurityQuestionListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            DataSourceResult list = _adminRepository.GetMasterDataListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateCustomerRegistration(NDAUserViewModel model)
        {
            model = _adminRepository.ValidateCustomerRegistration(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMappedUsersListByApplicationRoleAsDataSource([DataSourceRequest] DataSourceRequest request, NDAUserViewModel model)
        {
            DataSourceResult list = _adminRepository.GetMappedUsersListByApplicationRoleAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveApplicationRoleMenu(NDARoleViewModel model)
        {
            model = _adminRepository.SaveApplicationRoleMenu(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMasterDataInfoByType(NDAMasterDataViewModel model)
        {
            model = _adminRepository.GetMasterDataInfoByType(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveMasterDataInfoByType(NDAMasterDataViewModel model)
        {
            model = _adminRepository.SaveMasterDataInfoByType(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentListAsDataSource([DataSourceRequest] DataSourceRequest request, NDARoleViewModel model)
        {
            DataSourceResult list = _adminRepository.GetDepartmentListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatusListAsDataSource([DataSourceRequest] DataSourceRequest request, NDAMasterDataViewModel model)
        {
            DataSourceResult list = _adminRepository.GetStatusListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSectorAndBlockListAsDataSource([DataSourceRequest] DataSourceRequest request, NDAMasterDataViewModel model)
        {
            DataSourceResult list = _adminRepository.GetSectorAndBlockListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyTypeListAsDataSource([DataSourceRequest] DataSourceRequest request, NDAMasterDataViewModel model)
        {
            DataSourceResult list = _adminRepository.GetPropertyTypeListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSchemeTypeListAsDataSource([DataSourceRequest] DataSourceRequest request, NDAMasterDataViewModel model)
        {
            DataSourceResult list = _adminRepository.GetSchemeTypeListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAreaRangeListAsDataSource([DataSourceRequest] DataSourceRequest request, NDAMasterDataViewModel model)
        {
            DataSourceResult list = _adminRepository.GetAreaRangeListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCommonConfigurationListAsDataSource([DataSourceRequest] DataSourceRequest request, NDAMasterDataViewModel model)
        {
            DataSourceResult list = _adminRepository.GetCommonConfigurationListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlineSchemeListAsDataSource([DataSourceRequest] DataSourceRequest request, NDAMasterDataViewModel model)
        {
            DataSourceResult list = _adminRepository.GetOnlineSchemeListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMasterDataDropDownListAsDataSource([DataSourceRequest] DataSourceRequest request, NDAMasterDataViewModel model)
        {
            DataSourceResult list = _adminRepository.GetMasterDataDropDownListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
