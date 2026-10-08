using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Service;
using NA.PMS.Service.Property;
using NA.PMS.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NA.PMS.Web.Areas.Property.Controllers
{
    public class DefaultController : WebBaseController
    {
        private IPropertyCancellationService _propertyCancellationService;
        private IGeneralService _generalService;
        private IMastersService _masterService;
        public DefaultController(IPropertyCancellationService propertyCancellationService, IGeneralService generalService,IMastersService masterService)
        {
            _propertyCancellationService = propertyCancellationService;
            _generalService = generalService;
            _masterService = masterService;
        }

        // GET: Property/Default
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Manage()
        {
            return View();
        }

        public ActionResult ManageRequest()
        {
            return View();
        }

        public JsonResult GetRegistrationIdListByType([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            //var lst = _propertyCancellationService.GetRIDsByCancellationType(Req, type);
            var list = _generalService.GetRegistrationIdListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApproverIdListByDepartment([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            //var list = _generalService.GetApproverIdList(request);
            var list = _generalService.GetApproverIdListByDepartment(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSchemeRefundTypeListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            var lst = _generalService.GetSchemeRefundTypeListAsDataSource(request, model);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCancelledPropertyListAsDataSource([DataSourceRequest] DataSourceRequest request, PropertyViewModel model)
        {
            var lst = _propertyCancellationService.GetCancelledPropertyListAsDataSource(request, model);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveCancellationDetail(PropertyViewModel model)
        {
            int flag = _propertyCancellationService.SaveCancellationDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCancellationDetailById(PropertyViewModel model)
        {
            var details = _propertyCancellationService.GetCancellationDetailById(model);
            return Json(details, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveCancellationStatus(PropertyViewModel model)
        {
            int flag = _propertyCancellationService.SaveCancellationStatus(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyDetailByRegistrationId(PropertyViewModel model)
        {
            var data = _generalService.GetPropertyDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSchemeRefundDetailById(PropertyViewModel model)
        {
            var data = _masterService.GetRefundDetailById(model.RefundTypeId.Value);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}