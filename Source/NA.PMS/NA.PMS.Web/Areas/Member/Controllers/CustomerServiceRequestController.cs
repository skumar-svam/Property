using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NA.PMS.Service.APIRequestServices;
using NA.PMS.Web.Models;
using NA.PMS.Service;
using NA.PMS.Service.Reports;
using NA.PMS.Model;
using NA.PMS.Web.Controllers;

namespace NA.PMS.Web.Areas.Member.Controllers
{
    public class CustomerServiceRequestController : WebBaseController
    {
        private readonly IApiRequestService _apiRequestService;
        private ICitizenRequestsService _citizenRequest;
        private IGeneralService _generalService;
        private IReportService _reportService;
        public CustomerServiceRequestController(IApiRequestService apiRequestService, ICitizenRequestsService citizenRequest, IReportService reportService ,IGeneralService generalService)
        {
            _apiRequestService = apiRequestService;
            _citizenRequest = citizenRequest;
            _generalService = generalService;
            _reportService = reportService;
        }
        
        public ActionResult ManageServiceRequest()
        {
            return View();
        }
        public ActionResult ManageEmail()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult ManageLetter()
        {
            return View();
        }

        [AllowAnonymous]
        public JsonResult GetLetterByBarcode(string barcode)
        {
            var letterHistory = _apiRequestService.GetLetterByBarcode(barcode);
            if (letterHistory == null)
            {
                letterHistory = new LetterViewModel();
                return Json(letterHistory.LetterContent="Not Available", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(letterHistory, JsonRequestBehavior.AllowGet);
            }            
        }

        public ActionResult GetLetterHistoryDetails([DataSourceRequest]DataSourceRequest request)
        {
            var letterHistory = _apiRequestService.GetLetterHistoryDetails(request);
            return Json(letterHistory, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRegistrationIdList([DataSourceRequest]DataSourceRequest request)        
        {
            var depts = _generalService.GetRegistrationIdList(request);
            return Json(depts, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApplicantDetailToSendMessage(int? registrationId)
        {
            var detail = _generalService.GetApplicantDetailToSendMessage(registrationId);
            return Json(detail, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SendMessageToApplicant(ApplicantModel model)
        {
            var flag = _generalService.SendMessageToApplicant(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentList()
        {
            var depts = _generalService.GetAllDepartments();
            return Json(depts, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceRequestStatusList()
        {
            var depts = _generalService.GetServiceRequestStatusList();
            return Json(depts, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ServiceReport([DataSourceRequest]DataSourceRequest request, int? departmentId, DateTime? startDate, DateTime? endDate, int? status)
        {
            //var reports = _citizenRequest.GetCustomerServiceReport(request);
            var reports = _reportService.GetCustomerServiceReport(request,departmentId,startDate,endDate,status);
            return View(reports);
        }

        public JsonResult GetServiceRequestReport([DataSourceRequest]DataSourceRequest request, int? departmentId, DateTime? startDate, DateTime? endDate, int? status)
        {
            var reports = _reportService.GetCustomerServiceReport(request, departmentId, startDate, endDate, status);
            return Json(reports,JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllCustomerServiceRequest([DataSourceRequest]DataSourceRequest req)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            DataSourceResult data = null;
            if (loginUser != null)
                data = _apiRequestService.GetAllServiceRequests().ToDataSourceResult(req);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AcceptRejectService(int serId, string rType)
        {
            var result = _apiRequestService.UpdateServiceRequestStatus(serId, rType);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerServiceRequestList([DataSourceRequest]DataSourceRequest request)
        {
            var data = _apiRequestService.GetCustomerServiceRequestList(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCustomerServiceRequestDetailByRequestId(int? requestId)
        {
            var data = _apiRequestService.GetCustomerServiceRequestDetailByRequestId(requestId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetResourceMessageList()
        {
            var list = _generalService.GetResourceMessageList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}