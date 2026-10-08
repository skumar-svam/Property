using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Web.Models;
using NA.PMS.Web.Controllers;
using NA.PMS.Model.NIC;
using NA.PMS.Web.Controllers.Common;
using System.Configuration;

namespace NA.PMS.Web.Areas.Member.Controllers
{
    public class ServiceRequestController : WebBaseController
    {
        static string servicePassalt = ConfigurationManager.AppSettings["ServicePassalt"];
        static string filepath_NIC = ConfigurationManager.AppSettings["SiteRootPath_NM"];
        IGeneralService _generalService;
        IRequestService _requestService;
        INICService _nicService;
        public ServiceRequestController(IGeneralService generalService, IRequestService requestService, INICService nicService)
        {
            _generalService = generalService;
            _requestService = requestService;
            _nicService = nicService;
        }

        public ActionResult Index(int? id)
        {
            ServiceRequestViewModel model = new ServiceRequestViewModel();
            //model.ServiceModel = new ServiceViewModel();           
            //return View(model);
            if (id != null && id > 0)
            {
                model = _requestService.GetServiceRequestDetailById(id);
                return View(model);
            }
            else return View(model);
        }

        [HttpPost]
        public ActionResult Index(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            if (model != null)
            {
                ServiceRequestViewModel service = _requestService.SaveServiceRequestDetail(model, files);
                //ServiceRequestViewModel service = _requestService.SaveServiceRequestForSamadhanDiwas(model, files);
                return Json(service, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult Property()
        {
            return View();
        }

        public ActionResult Manage()
        {
            return View();
        }

        public ActionResult Online()
        {
            return View();
        }

        public ActionResult JSKServices()
        {
            return View();
        }

        public ActionResult ManageRequest()
        {
            return View();
        }

        public ActionResult PradhikaranDiwas()
        {
            return View();
        }

        public ActionResult PradhikaranDiwasRequest()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PradhikaranDiwas(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            if (model != null)
            {
                ServiceRequestViewModel service = _requestService.SaveServiceRequestForSamadhanDiwas(model, files);
                return Json(service, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        //public ActionResult SaveServiceRequestForSamadhanDiwas(ServiceRequestModel model, IEnumerable<HttpPostedFileBase> files)
        public ActionResult SaveServiceRequestForSamadhanDiwas(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var file = Request.Files[0];
            //if (model != null)
            //{
            //    ServiceRequestModel service = _requestService.SaveServiceRequestForSamadhanDiwas(model, files);
            //    return Json(service,JsonRequestBehavior.AllowGet);
            //}
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult ServiceStatus()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadDocument(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            if (model != null)
            {
                ServiceRequestViewModel service = _requestService.UploadServiceRequestDocuments(model, files);
                return RedirectToAction("RequestDetail", new { id = service.ServiceModel.RequestId });
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdateStatus(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> letterfiles)
        {
            var result = _requestService.UpdateStatus(model, letterfiles);
            return RedirectToAction("Manage", "ServiceRequest");
        }

        public ActionResult RequestDetail(int? id)
        {
            if (id != null && id > 0)
            {
                ServiceRequestViewModel service = _requestService.GetServiceRequestDetailById(id);
                return View(service);
            }
            else return RedirectToAction("Index");
        }


        //public ActionResult EditRequestDetail(string id)
        //{
        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        int ServiceId = Convert.ToInt32(CommonHelper.Decode(id));
        //        ServiceRequestViewModel service = _requestService.GetServiceRequestDetailById(ServiceId);
        //        if (service.ServiceModel.ServiceStatusId == Constants.InProgress)
        //        {
        //            return View(service);
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}

        public ActionResult RedirectServiceRequestByType(int? id, string service, string filter)
        {
            if (id != null) Session["ServiceStatusId"] = id;
            if (service != null) Session["ServiceName"] = service;
            if (filter == "Online") return RedirectToAction("Online");
            else if (filter == "jsk") return RedirectToAction("JSKServices");
            else return RedirectToAction("Manage");
        }

        public ActionResult EditRequestDetail(int? id)
        {
            if (id != null)
            {
                ServiceRequestViewModel service = _requestService.GetServiceRequestDetailById(id);
                return View(service);
            }
            else return RedirectToAction("Index");
        }

        [HttpPost]   //update service request
        public ActionResult EditRequestDetail(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            if (model != null)
            {
                ServiceRequestViewModel service = _requestService.SaveServiceRequestDetail(model, files);
                return View(service);
            }
            else return RedirectToAction("Index");
        }

        public JsonResult UpdateServiceRequest(ServiceRequestModel ObjServiceReq)
        {
            var data = _requestService.UpdateServiceReq(ObjServiceReq);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public ActionResult GetServiceRequestDetailForCustomer(int? requestId, string mobile)
        {
            if (requestId != null && requestId > 0)
            {
                ServiceRequestViewModel service = _requestService.GetServiceRequestDetailForCustomer(Convert.ToInt32(requestId), mobile);
                return Json(service, JsonRequestBehavior.AllowGet);
            }
            else return RedirectToAction("ServiceStatus");
        }

        public JsonResult GetDepartmentList()
        {
            var department = _generalService.GetDepartmentList();
            return Json(department, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsersDepartmentListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            var department = _generalService.GetUsersDepartmentListAsDataSource(request, model);
            return Json(department, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceListByDepartment(int departmentId)
        {
            var services = _generalService.GetServiceListByDepartment(departmentId);
            return Json(services, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Not Contain NIC Services
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public JsonResult GetServiceListByDepartmentForNAServices(int departmentId)
        {
            var services = _generalService.GetServiceListByDepartmentForNAServices(departmentId);
            return Json(services, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatusList()
        {
            var services = _generalService.GetServiceRequestStatusList();
            var data = services.Where(m => m.id == 5 || m.id == 9 || m.id == 11 || m.id == 12).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubDepartmentList(int departmentId)
        {
            var department = _generalService.GetSubDepartmentList(departmentId);
            return Json(department, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferTypeList()
        {
            var transferList = _generalService.GetTransferTypeList();
            return Json(transferList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferSubTypeList(int transferTypeId)
        {
            var transferSubList = _generalService.GetTransferSubTypeList(transferTypeId);
            return Json(transferSubList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGenderList()
        {
            var gender = _generalService.GetGenderList();
            return Json(gender, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOccupationList()
        {
            var occupation = _generalService.GetOccupationList();
            return Json(occupation, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCICRequestTypeList()
        {
            var occupation = _generalService.GetCICRequestTypeList();
            return Json(occupation, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanyMemberTypeList()
        {
            var typeList = _generalService.GetCompanyMemberTypeList();
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFirmStatusList()
        {
            var typeList = _generalService.GetFirmStatusList();
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMortgageTypeList()
        {
            var typeList = _generalService.GetMortgageTypeList();
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNOCStatusList()
        {
            var typeList = _generalService.GetNOCStatusList();
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGPAStatusList()
        {
            var statusList = _generalService.GetGPAStatusList();
            return Json(statusList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApplicantDetailsByRegistrationId(int? registrationId)
        {
            var detail = _requestService.GetApplicantDetailsByRegistrationId(registrationId);
            return Json(detail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDirectorOrShareholders(string directorName, decimal? share, string shareType)
        {
            var shares = _requestService.SaveDirectorOrShareholders(directorName, share, shareType);
            return Json(shares, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDirectorShareholderDetails([DataSourceRequest] DataSourceRequest request)
        {
            if (Session["TempDirectors"] != null)
            {
                var directorList = (List<DirectorShareholderModel>)Session["TempDirectors"];
                return Json(directorList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RemoveDirectorShareholderDetail(int id)
        {
            var flag = false;
            var directorList = (List<DirectorShareholderModel>)Session["TempDirectors"];
            if (directorList != null)
            {
                directorList.RemoveAt(id - 1);
                flag = true;
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetFileUploadOptionsForService(int? departmentId, int? serviceId)
        //{
        //    var checkList = _requestService.GetChecklistOptionsForFileUpload(departmentId, serviceId);
        //    string divMain = "";
        //    int docCounter = 1;
        //    foreach (var item in checkList)
        //    {
        //        divMain = divMain + "<div class='row  border-bottom'> "
        //                               + "<div class='col-md-3 col-sm-3 col-xs-12 form-group'><b>" + docCounter + "</b></div>"
        //                               + "<div class='col-md-6 col-sm-6 col-xs-12 form-group'><span>" + item.ChecklistName + "</span></div>"
        //                               + "<div class='col-md-3 col-sm-3 col-xs-12 form-group'><input type='file' class='single' name='files' /></div>"
        //                          + "</div>";
        //        docCounter++;
        //    }
        //    ViewBag.uploadFilehtml = divMain;
        //    //put in session
        //    Session["CheckList"] = checkList;
        //    return Json(divMain, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ManageServiceRequest()
        {
            ServiceRequestViewModel model = new ServiceRequestViewModel();
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            model.ServiceModel = new ServiceViewModel();
            model.ServiceModel.RoleType = loginUser.RoleMaster.RoleId == Constants.RoleIdSDOfficer ? "SDO" : "SDU";
            return View(model);
        }

        public JsonResult GetServiceRequestReport([DataSourceRequest]DataSourceRequest request, int? departmentId, DateTime? fromDate, DateTime? toDate)
        {
            string path = "http://" + System.Configuration.ConfigurationManager.AppSettings["FTPSiteRootPath"];
            var reports = _requestService.GetServiceRequestReport(request, departmentId, fromDate, toDate);
            var model = reports.Data;
            var Model = model.OfType<ServiceReportModel>().ToList();
            Model.ForEach(m => m.IsServiceHasDoc = ServiceRequestHaveDocument(m.Id));
            //Model.ForEach(m => m.UploadedDocumentName = (m.UploadedDocumentName != string.Empty) ? path + "//" + m.Id + "//" + m.UploadedDocumentName : string.Empty);
            //Model.ForEach(m => m.DispatchDocumentName = (m.DispatchDocumentName != string.Empty) ? path + "//" + m.Id + "//" + m.DispatchDocumentName : string.Empty);
            reports.Data = Model;
            return Json(reports, JsonRequestBehavior.AllowGet);
        }

        public bool ServiceRequestHaveDocument(int RequestId)
        {
            bool hasDocument = false;
            FtpHandler ObjFtpHandler = new FtpHandler();
            string path = ObjFtpHandler.GetDocumentPathForServiceRequest(RequestId, true);
            List<string> allFilesOld = ObjFtpHandler.DirSearch(path);
            if (allFilesOld.Count() > 0) { hasDocument = true; }
            return hasDocument;
        }

        public JsonResult GetServiceRequestDocuments([DataSourceRequest]DataSourceRequest request, int? RequestId)
        {
            var reports = _requestService.GetServicerequestUploadedDocuments(request, RequestId);
            return Json(reports, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceRequestDocumentsById([DataSourceRequest]DataSourceRequest request, ServiceViewModel model)
        {
            var reports = _requestService.GetServiceRequestUploadedDocumentsById(request, model);
            return Json(reports, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFileUploadOptionsForService(int? departmentId, int? serviceId)
        {
            var checkList = _requestService.GetFileUploadHtmlForService(departmentId, serviceId);
            if (checkList != null) return Json(checkList, JsonRequestBehavior.AllowGet);
            else return Json("NotExist", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerServiceRequestList([DataSourceRequest]DataSourceRequest request, ServiceViewModel model)
        {
            var list = _requestService.GetCustomerServiceRequestList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerServiceRequestListById([DataSourceRequest]DataSourceRequest request, int Id)
        {
            var list = _requestService.GetCustomerServiceRequestList(request, new ServiceViewModel { Id = Id });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPradhikaranDiwasRequestList([DataSourceRequest]DataSourceRequest request, ServiceViewModel model)
        {
            var list = _requestService.GetPradhikaranDiwasRequestList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPradhikaranDiwasRequestById([DataSourceRequest]DataSourceRequest request, int Id)
        {
            var list = _requestService.GetPradhikaranDiwasRequestList(request, new ServiceViewModel { Id = Id });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatusMasterAsDataSource([DataSourceRequest]DataSourceRequest request, int? serviceId, DateTime? RequestDate)
        {
            var user = Session["CurrentUser"] as CurrentUserDetail;
            DateTime? date = Convert.ToDateTime("2019/06/01");
            List<int?> servicelist = new List<int?> { 21, 25, 22, 5, 12, 17 };//26
            if (user != null)
            {
                var list = new List<DropdownViewModel>();
                if (user.RoleMaster.RoleInDepartment != null && (user.RoleMaster.RoleInDepartment == RoleInDepartment.Assistant || user.RoleMaster.RoleInDepartment == RoleInDepartment.Accountant))
                {
                    //if (serviceId == NAService.Other || serviceId == NAService.Query || serviceId == NAService.SubmissionOfDocument || serviceId == NAService.LeaseDeed)
                    if (servicelist.Contains(serviceId))
                    {
                        list.Add(new DropdownViewModel { Id = NAStatusId.Completed, Text = "Complete" });
                        list.Add(new DropdownViewModel { Id = NAStatusId.Forwarded, Text = "Forward" });
                        //list.Add(new DropdownViewModel { Id = NAStatusId.Rejected, Text = "Reject" });
                        list.Add(new DropdownViewModel { Id = NAStatusId.Objection, Text = "Objection" });
                        list.Add(new DropdownViewModel { Id = NAStatusId.Appointment, Text = "Appointment" });
                    }
                    else
                    {
                        if (RequestDate >= date)
                        {
                            list.Add(new DropdownViewModel { Id = NAStatusId.Objection, Text = "Objection" });
                            //list.Add(new DropdownViewModel { Id = NAStatusId.Rejected, Text = "Reject" });
                            list.Add(new DropdownViewModel { Id = NAStatusId.Forwarded, Text = "Forward" });
                            list.Add(new DropdownViewModel { Id = NAStatusId.Appointment, Text = "Appointment" });
                        }
                        else
                        {
                            list.Add(new DropdownViewModel { Id = NAStatusId.Completed, Text = "Complete" });
                            list.Add(new DropdownViewModel { Id = NAStatusId.Forwarded, Text = "Forward" });
                            //list.Add(new DropdownViewModel { Id = NAStatusId.Rejected, Text = "Reject" });
                            list.Add(new DropdownViewModel { Id = NAStatusId.Objection, Text = "Objection" });
                            list.Add(new DropdownViewModel { Id = NAStatusId.Appointment, Text = "Appointment" });
                        }
                    }
                }
                else if (user.RoleMaster.RoleInDepartment != null && (user.RoleMaster.RoleInDepartment == RoleInDepartment.OSD || user.RoleMaster.RoleInDepartment == RoleInDepartment.HODAccounts))
                {
                    list.Add(new DropdownViewModel { Id = NAStatusId.Forwarded, Text = "Forward" });
                    list.Add(new DropdownViewModel { Id = NAStatusId.Rejected, Text = "Reject" });
                    list.Add(new DropdownViewModel { Id = NAStatusId.Appointment, Text = "Appointment" });
                }
                else
                {
                    var status = _generalService.GetStatusList();
                    //list = _generalService.GetStatusList();
                    list.Add(status[8]);
                    //list.Add(status[2]);
                    //list.Add(status[3]);
                    list.Add(status[1]);
                    list.Add(status[9]);
                    list.Add(status[12]);
                    list.Add(status[18]);
                }
                return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(_generalService.GetStatusMasterAsDataSource(request), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetStatusMasterAsDataSourceTest([DataSourceRequest]DataSourceRequest request, int? serviceId, DateTime? RequestDate)
        {
            var user = Session["CurrentUser"] as CurrentUserDetail;
            DateTime? date = Convert.ToDateTime("2019/05/22");
            if (user != null)
            {
                var list = new List<DropdownViewModel>();
                if (user.RoleMaster.RoleInDepartment != null && (user.RoleMaster.RoleInDepartment == RoleInDepartment.Assistant || user.RoleMaster.RoleInDepartment == RoleInDepartment.Accountant))
                {
                    List<int?> servicelist = new List<int?> { 21, 25, 22, 5, 12, 17 };
                    //if (RequestDate >= date && (serviceId != NAService.Other || serviceId != NAService.Query || serviceId != NAService.SubmissionOfDocument || serviceId != NAService.LeaseDeed))
                    if (RequestDate >= date && servicelist.Contains(serviceId))
                    {
                        list.Add(new DropdownViewModel { Id = NAStatusId.Objection, Text = "Objection" });
                        list.Add(new DropdownViewModel { Id = NAStatusId.Rejected, Text = "Reject" });
                        list.Add(new DropdownViewModel { Id = NAStatusId.Forwarded, Text = "Forward" });
                    }
                    else
                    {
                        list.Add(new DropdownViewModel { Id = NAStatusId.Completed, Text = "Complete" });
                        list.Add(new DropdownViewModel { Id = NAStatusId.Rejected, Text = "Reject" });
                        list.Add(new DropdownViewModel { Id = NAStatusId.Objection, Text = "Objection" });
                        list.Add(new DropdownViewModel { Id = NAStatusId.Forwarded, Text = "Forward" });
                    }
                }
                else if (user.RoleMaster.RoleInDepartment != null && (user.RoleMaster.RoleInDepartment == RoleInDepartment.OSD || user.RoleMaster.RoleInDepartment == RoleInDepartment.HODAccounts))
                {
                    list.Add(new DropdownViewModel { Id = NAStatusId.Forwarded, Text = "Forward" });
                }
                else
                {
                    var status = _generalService.GetStatusList();
                    list.Add(status[8]);
                    //list.Add(status[2]);
                    //list.Add(status[3]);
                    list.Add(status[1]);
                    list.Add(status[9]);
                    list.Add(status[12]);
                }
                return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(_generalService.GetStatusMasterAsDataSource(request), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateCustomerServiceRequestStatus(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> letterfiles)
        {
            int flag = _requestService.UpdateCustomerServiceRequestStatus(model, letterfiles);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UploadGeneratedLetterByserviceId(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> documentfiles)
        {
            int flag = _requestService.UploadGeneratedLetterByserviceId(model, documentfiles);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult ForwardServiceRequestInBulkFormat(List<int> requestIdList, int statusId,int approverId,string comment)
        //{
        //    int flag = _requestService.ForwardServiceRequestInBulkFormat(new ServiceViewModel { RequestIdList=requestIdList,StatusId=statusId,ApproverId=approverId,Comment=comment});
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}      

        public JsonResult ForwardServiceRequestInBulkFormat(ServiceViewModel model)
        {
            int flag = _requestService.ForwardServiceRequestInBulkFormat(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegistrationIdListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetRegistrationIdListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetServiceListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServicesByDepartmentAsDataSource([DataSourceRequest]DataSourceRequest request, DropdownViewModel model)
        {
            var list = _generalService.GetServicesByDepartmentAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetServiceRequestListForJanSuvidhaKendra([DataSourceRequest]DataSourceRequest request)
        {
            var list = _requestService.GetServiceRequestListForJanSuvidhaKendra(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyListForJanSuvidhaKendra([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var list = _requestService.GetPropertyListForJanSuvidhaKendra(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSectorListAsDataSource([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var list = _generalService.GetSectorListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBlockListAsDataSource([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var list = _generalService.GetBlockListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPlotListAsDataSource([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var list = _generalService.GetPlotListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetDepartmentListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlineCustomerServiceRequestAsDataSource([DataSourceRequest]DataSourceRequest request, ServiceViewModel model)
        {
            var list = _requestService.GetOnlineCustomerServiceRequestAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateRegistrationIdToServiceRequestById(ServiceViewModel model)
        {
            int flag = _requestService.UpdateRegistrationIdToServiceRequestById(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerServiceRequestByJSKAsDataSource([DataSourceRequest]DataSourceRequest request, ServiceViewModel model)
        {
            var list = _requestService.GetCustomerServiceRequestByJSKAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #region NIC Methods

        public ActionResult ManageNICRequests()
        {
            return View();
        }

        public JsonResult GetCustomerServiceRequestList_NIC([DataSourceRequest]DataSourceRequest request, ServiceVM model)
        {
            var list = _requestService.GetCustomerServiceRequestList_NIC(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RequestDetailNIC(int? id)
        {
            if (id != null && id > 0)
            {
                ServiceRequestViewModel service = _requestService.GetServiceRequestDetailById(id);
                return View(service);
            }
            else return RedirectToAction("Index");
        }

        public JsonResult GetStatusMaster_NIC([DataSourceRequest]DataSourceRequest request)
        {
            var user = Session["CurrentUser"] as CurrentUserDetail;
            if (user != null)
            {
                var list = new List<DropdownViewModel>();
                //if (user.RoleMaster.RoleInDepartment != null && user.RoleMaster.RoleInDepartment == RoleInDepartment.Assistant)
                //{
                //    list.Add(new DropdownViewModel { Id = NAStatusId.Completed, Text = "Completed" });
                //    list.Add(new DropdownViewModel { Id = NAStatusId.Cancelled, Text = "Cancelled" });
                //    list.Add(new DropdownViewModel { Id = NAStatusId.Pending, Text = "Pending" });
                //}
                //else if (user.RoleMaster.RoleInDepartment != null && user.RoleMaster.RoleInDepartment == RoleInDepartment.OSD)
                //{
                //    list.Add(new DropdownViewModel { Id = NAStatusId.Forwarded, Text = "Forward for Completion" });
                //    list.Add(new DropdownViewModel { Id = NAStatusId.Forwarded, Text = "Forward for Cancellation" });
                //}
                //else
                //{
                var status = _generalService.GetStatusList();
                list.Add(status[8]);
                list.Add(status[2]);
                list.Add(status[3]);
                list.Add(status[9]);
                list.Add(status[12]);
                //   }
                return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(_generalService.GetStatusMasterAsDataSource(request), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateCustomerServiceRequestStatus_NIC(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> letterfiles)
        {
            int flag = _requestService.UpdateCustomerServiceRequestStatus_NIC(model, letterfiles);
            if (flag != ReturnType.None)
            {
                var serviceStatus = new ServiceStatusVM();
                if (model.StatusId > 0)
                {
                    //Get nivesh status code mappped to the customer request status
                    serviceStatus = _nicService.GetServiceStatusByCustomerRequestStatusId((int)model.StatusId);
                }

                if (model.RegistrationId > 0)
                {
                    var _nicData = _nicService.GetNiveshMitraServicesByReqId(model.RequestId);
                    WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel
                    {
                        ControlID = _nicData.NICModel.TxtControlID,
                        ApplicationID = Convert.ToString(model.RequestId),
                        ProcessIndustryID = Convert.ToString(model.RegistrationId),
                        UnitID = _nicData.NICModel.TxtUnitID,
                        ServiceID = _nicData.NICModel.TxtServiceID,
                        Status_Code = serviceStatus.StatusCode,
                        Fee_Status = string.Empty,
                        Fee_Amount = string.Empty,
                        passsalt = servicePassalt
                    };

                    if (Session["CurrentUser"] != null)
                    {
                        var userdetails = (CurrentUserDetail)Session["CurrentUser"];
                        if (model.StatusId == NAStatusId.Forwarded)
                        {
                            if (userdetails != null)
                            {
                                var _approverId = model.ApproverId > 0 ? (int)model.ApproverId : 0;
                                if (_approverId > 0)
                                {
                                    var _appDetails = _generalService.GetLoginUserDetails(_approverId);
                                    objWReturn_CUSID_STATUSModel.Remarks = "REMARKS | " + model.Comment + " -  Forwarded to " + _appDetails.UserName + " by " + userdetails.UserName + " - Status: " + serviceStatus.StatusName + " | ";
                                }
                            }
                        }
                        else
                        {
                            if (userdetails != null)
                            {
                                objWReturn_CUSID_STATUSModel.Remarks = "REMARKS | " + model.Comment + " - User: " + userdetails.FirstName + " - Status:  " + serviceStatus.StatusName + " | ";
                            }
                        }
                    }

                    // in case of completion only
                    if (model.StatusId == NAStatusId.Completed)
                    {
                        if (model.RequestId > 0)
                        {
                            int _reqNo = model.RequestId > 0 ? (int)model.RequestId : 0;
                            var _reqDetails = _requestService.GetServiceRequestDetailForCustomer_NIC(_reqNo);

                            var _docName_FullPath = _reqDetails.ServiceModel.DispatchedDocument;
                            objWReturn_CUSID_STATUSModel.NOC_Certificate_Number = "Noida/Industry/2019/" + model.RequestId.ToString();
                            objWReturn_CUSID_STATUSModel.NOC_URL = _docName_FullPath;
                        }
                    }

                    NiveshMitraServices _niveshMitraServices = new NiveshMitraServices();
                    _niveshMitraServices.GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);
                }
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePaymentStatusByRequestId(int RequestId)
        {
            var flag = false;
            var niveshMitraDetails = new NiveshMitraMasterVM();
            NewDataSet _objNewDataSet = new NewDataSet();
            niveshMitraDetails = _nicService.GetNiveshMitraServicesDetailsByApplicationId(RequestId);
            if (niveshMitraDetails != null)
            {
                var _WReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel
                {
                    ControlID = niveshMitraDetails.ControlID,
                    UnitID = niveshMitraDetails.UnitID,
                    ServiceID = niveshMitraDetails.ServiceID
                };

                // check payment status
                NiveshMitraServices _niveshMitraServices = new NiveshMitraServices();
                _objNewDataSet = _niveshMitraServices.WGetUBPaymentDetails(_WReturn_CUSID_STATUSModel);

                if (_objNewDataSet != null)
                {
                    if (_objNewDataSet.Table != null)
                    {
                        var _ServiceRequestVM = new ServiceRequestVM
                        {
                            Id = RequestId
                        };
                        var result = _nicService.UpdateRequestPaymentStatus(_ServiceRequestVM);
                        flag = result > 0 ? true : false;

                        if (flag)
                        {
                            // fee status
                            _WReturn_CUSID_STATUSModel.ProcessIndustryID = niveshMitraDetails.ProcessIndustryID;
                            _WReturn_CUSID_STATUSModel.ProcessIndustryID = niveshMitraDetails.ApplicationID;
                            _WReturn_CUSID_STATUSModel.Status_Code = Common.ServiceStatus.FEE_PAID;
                            _WReturn_CUSID_STATUSModel.Remarks = ServiceStatus_Text.FEE_PAID;
                            _WReturn_CUSID_STATUSModel.Fee_Amount = "5900";
                            _WReturn_CUSID_STATUSModel.Fee_Status = PaymentStatus_NIC.PAID;

                            _niveshMitraServices.GetWReturn_CUSID_STATUS(_WReturn_CUSID_STATUSModel);
                        }
                    }
                }
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public JsonResult GetApproverIdByDepartmentAsDataSource([DataSourceRequest]DataSourceRequest request, DropdownViewModel model)
        {
            var list = _generalService.GetApproverIdByDepartmentAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUploadedDocumentByServiceId(int Id, int Rid, string ActionType)
        {
            var reports = _requestService.GetUploadedDocumentByServiceId(Id, Rid, ActionType);
            return Json(reports, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerServiceRequestListByRid([DataSourceRequest]DataSourceRequest request, int? rId)
        {
            var list = _requestService.GetCustomerServiceRequestListByRid(request, rId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentTypeListAsDataSource([DataSourceRequest]DataSourceRequest request, DropdownViewModel model)
        {
            var list = _generalService.GetDocumentTypeListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDocumentTypesInSession(DocumentViewModel model)
        {
            int flag = _requestService.SaveDocumentTypesInSession(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSavedDocumentTypeListByIdAsDataSource([DataSourceRequest]DataSourceRequest request, DocumentViewModel model)
        {
            var list = _requestService.GetSavedDocumentTypeListByIdAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}