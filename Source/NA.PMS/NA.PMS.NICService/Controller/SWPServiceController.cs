using Kendo.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.NICServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using NA.PMS.NICService.Resource;

namespace NA.PMS.NICController
{
    public class SWPServiceController : SWPSuperController
    {
        static string servicePassalt = ConfigurationManager.AppSettings["ServicePassalt"];
        private ISWPRequestService _requestService;
        private ISWPGeneralService _generalService;
        private SWPApiServiceProvider _apiService;
        PIMSAccount _account;
        AccountViewModel _userInfo;
        public SWPServiceController()
        {
            _requestService = new SWPRequestService();
            _generalService = new SWPGeneralService();
            _apiService = new SWPApiServiceProvider();
            _account = new PIMSAccount();
            _userInfo = new AccountViewModel();
        }

        #region services from customer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SingleWindowPortal()
        {
            return View();
        }

        public ActionResult SWPServices()
        {
            return View();
        }

        public ActionResult SWPServiceStatus()
        {
            return View();
        }

        public ActionResult ManageServices()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ManageReasons()
        {
            return View();
        }

        public ActionResult ServiceDetail(int? id)
        {
            if (id != null && id > 0)
            {
                SWPServicesViewModel service = _requestService.GetServiceRequestDetailById(id);
                service.ServiceModel.Comment = string.Empty;
                return View(service);
            }
            else return RedirectToAction("ManageServices");
        }

        public ActionResult ValidateProperty(SWPPostViewModel apidmodel = null)
        {
            if (!string.IsNullOrEmpty(apidmodel.TxtControlID) && !string.IsNullOrEmpty(apidmodel.TxtUnitID) && !string.IsNullOrEmpty(apidmodel.TxtServiceID))
            {
                if (!string.IsNullOrEmpty(apidmodel.TxtRequestID))
                {
                    // to check unit already exits
                    //var data = _requestService.GetNiveshMitraServicesDetails(apidmodel);
                    var data = _requestService.ValidateNiveshMitraServicePostData(apidmodel);
                    if (data != null && data.IsServiceExist == true)
                    {
                        Session["NIC_Service"] = apidmodel;
                        data.NICRequestId = apidmodel.TxtRequestID;
                        return RedirectToAction("RequestService", new { TxtControlID = data.NICControlId, TxtUnitID = data.NICUnitId, TxtServiceID = data.NICServiceId, TxtProcessIndustryID = data.NICProcessIndustryId, TxtRequestID = data.NICRequestId });
                    }
                    else
                    {
                        return View(apidmodel);
                    }
                }
                else
                {
                    TempData["ErrorMessage_NIC"] = "Request No is not posted from Nivesh Mitra Portal, contact Nivesh mitra";
                    //return View("Error");
                    return RedirectToAction("Error", new { TxtControlID = apidmodel.TxtControlID, TxtUnitID = apidmodel.TxtUnitID, TxtServiceID = apidmodel.TxtServiceID, TxtProcessIndustryID = apidmodel.TxtProcessIndustryID, TxtRequestID = apidmodel.TxtRequestID });
                }
            }
            else
            {
                TempData["ErrorMessage_NIC"] = "Services are allowed only from Nivesh Mitra Portal";
                return View("Error");
            }
        }

        public ActionResult ManageRequest()
        {
            return View();
        }

        public ActionResult RequestService(SWPPostViewModel model)
        {
            if (!string.IsNullOrEmpty(model.TxtControlID) && !string.IsNullOrEmpty(model.TxtUnitID) && !string.IsNullOrEmpty(model.TxtServiceID))
            {
                //var apimodel = new WBasicDetailsModel_NMS { TxtControlID = controlId, TxtUnitID = unitId, TxtServiceID = serviceId, TxtProcessIndustryID = rid };
                if (!string.IsNullOrEmpty(model.TxtProcessIndustryID) && Convert.ToInt32(model.TxtProcessIndustryID) > 0)
                {
                    SWPServicesViewModel service = _requestService.GetNiveshMitraServicesByRegistrationId(model);
                    if (service != null && service.IsServiceExist == true)
                    {
                        var _reqId = Convert.ToInt32(service.SWPPostModel.TxtApplicationID);
                        return RedirectToAction("RequestDetail", new { id = _reqId });
                    }
                    else
                    {
                        //service.SWPPostModel = model;
                        return View(service);
                    }
                }
                else
                {
                    SWPServicesViewModel service = new SWPServicesViewModel();
                    service.SWPPostModel = model;
                    return View(service);
                }
            }
            else
            {
                TempData["ErrorMessage_NIC"] = "Nivesh Mitra Service Failed";
                //return View("Error");
                return RedirectToAction("Error", new { TxtControlID = model.TxtControlID, TxtUnitID = model.TxtUnitID, TxtServiceID = model.TxtServiceID, TxtProcessIndustryID = model.TxtProcessIndustryID, TxtRequestID = model.TxtRequestID });
            }
        }

        public ActionResult RequestDetail(int? id)
        {
            if (id != null && id > 0)
            {
                var service = _requestService.GetServiceRequestDetailById(id);
                return View(service);
            }
            return null;
        }

        public ActionResult Error(SWPPostViewModel apidmodel = null)
        {
            return View(apidmodel);
        }

        public ActionResult UpdateInfo()
        {
            return View();
        }

        public JsonResult GetServiceListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            //var services = _generalService.GetServiceListAsDataSource(request);
            var services = _generalService.GetServiceListAsDataSource(request);
            return Json(services, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNiveshMitraServiceRequests([DataSourceRequest]DataSourceRequest request)
        {
            var list = _requestService.GetNiveshMitraServiceRequests(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNiveshMitraServiceStatus([DataSourceRequest]DataSourceRequest request, SWPApiServiceViewModel apimodel)
        {
            var list = _requestService.GetNiveshMitraServiceStatus(request, apimodel);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNiveshMitraServices([DataSourceRequest]DataSourceRequest request, SWPApiServiceViewModel apimodel)
        {
            var list = _requestService.GetNiveshMitraServices(request, apimodel);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCitizenServiceDetailsbyServiceId([DataSourceRequest]DataSourceRequest request, string ServiceCode)
        {
            if (!string.IsNullOrEmpty(ServiceCode))
            {
                var filter = new FilterDescriptor { Member = "NICServiceCode", Value = ServiceCode };
                request.Filters.Add(filter);
            }
            var list = _requestService.GetCitizenServiceDetailsbyServiceId(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerServiceRequestList([DataSourceRequest]DataSourceRequest request, int RequestNo)
        {
            if (RequestNo > 0)
            {
                var filter = new FilterDescriptor { Member = "Id", Value = RequestNo };
                request.Filters.Add(filter);
            }
            var list = _requestService.GetCustomerServiceRequestList_NIC(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerServiceRequestGrid([DataSourceRequest]DataSourceRequest request)
        {
            //var list = _requestService.GetCustomerServiceRequestList_NIC(request);
            var list = _requestService.GetSWPRequestServiceListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetStatusMaster([DataSourceRequest]DataSourceRequest request, int? serviceId, int? approverId)
        {
            var user = _account.GetLoginAccountDetails(Session["UserName"].ToString());
            List<int?> servicelist = new List<int?> { 12 };
            if (user != null)
            {
                var list = new List<SWPDropdownViewModel>();
                //var status = _generalService.GetStatusList();
                var status = _generalService.GetStatusList();
                if (servicelist.Contains(serviceId) || user.UserRefId == approverId)
                {
                    list.Add(new SWPDropdownViewModel { Id = SWPStatusId.Completed, Text = "Complete" });
                    list.Add(new SWPDropdownViewModel { Id = SWPStatusId.Forwarded, Text = "Forward" });
                    list.Add(new SWPDropdownViewModel { Id = SWPStatusId.Rejected, Text = "Reject" });
                    list.Add(new SWPDropdownViewModel { Id = SWPStatusId.Objection, Text = "Objection" });
                    list.Add(new SWPDropdownViewModel { Id = SWPStatusId.Cancelled, Text = "Cancel" });
                }
                else
                {
                    //list.Add(status[8]);
                    list.Add(status[2]);
                    //list.Add(status[3]);
                    list.Add(status[9]);
                    list.Add(status[12]);
                }
                return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                //return Json(_generalService.GetStatusMasterAsDataSource(request), JsonRequestBehavior.AllowGet);
                return Json(_generalService.GetStatusMasterAsDataSource(request), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateCustomerServiceRequestStatus(SWPServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> letterfiles)
        {
            int flag = _requestService.UpdateCustomerServiceRequestStatus(model, letterfiles);
            if (flag != SWPReturnTypeId.None)
            {
                var serviceStatus = new SWPApiServiceViewModel();
                if (model.StatusId > 0)
                {
                    //Get nivesh status code mappped to the customer request status
                    serviceStatus = _requestService.GetServiceStatusByCustomerRequestStatusId((int)model.StatusId);
                }

                if (model.RegistrationId > 0)
                {
                    //var _nicData = _requestService.GetNiveshMitraServicesByReqId(model.RequestId);
                    //_requestService.GetServiceRequestDetailById(id);
                    var _nicData = _requestService.GetServiceRequestDetailById(model.RequestId);
                    //var _nicData = _requestService.GetServiceRequestDetailById(model.RequestId);
                    if (_nicData != null)
                    {
                        SWPStatusViewModel apistatus = new SWPStatusViewModel
                        {
                            ControlId = _nicData.SWPPostModel.TxtControlID,
                            ApplicationId = Convert.ToString(model.RequestId),
                            ProcessIndustryId = Convert.ToString(model.RegistrationId),
                            UnitId = _nicData.SWPPostModel.TxtUnitID,
                            ServiceId = _nicData.SWPPostModel.TxtServiceID,
                            RequestId = _nicData.SWPPostModel.TxtRequestID,
                            DepartmentId = _nicData.ServiceModel.DepartmentId.Value,
                            Department = _nicData.ServiceModel.Department,
                            StatusCode = _nicData.ServiceModel.NICStatusCode,
                            PendancyLevel = _nicData.ServiceModel.PendencyLevel,
                            FeeStatus = string.Empty,
                            FeeAmount = string.Empty,
                            NICPassSalt = servicePassalt
                        };

                        //PIMSAccount _account = new PIMSAccount();
                        //_userInfo = _account.GetLoginAccountDetails(Session["UserName"].ToString());

                        if (Session["CurrentUser"] != null)
                        {
                            var userdetails = _account.GetLoginAccountDetails(Session["UserName"].ToString()); //(CurrentUserDetail)Session["CurrentUser"];
                            if (model.StatusId == SWPStatusId.Forwarded)
                            {
                                if (userdetails != null)
                                {
                                    var _approverId = model.ApproverId > 0 ? (int)model.ApproverId : 0;
                                    if (_approverId > 0)
                                    {
                                        //var _appDetails = _generalService.GetLoginUserDetails(_approverId);
                                        var _appDetails = _generalService.GetLoginUserDetails(_approverId);
                                        apistatus.Remarks = "REMARKS | " + model.Comment + " -  Forwarded to " + _appDetails.UserName + " by " + userdetails.UserName + " - Status: " + serviceStatus.StatusName + " | ";
                                    }
                                }
                            }
                            else
                            {
                                if (model.StatusId == SWPStatusId.Rejected || model.StatusId == SWPStatusId.Cancelled)
                                {
                                    if (userdetails != null)
                                    {
                                        apistatus.Remarks = "REASON: " + model.NICReasonText + "REMARKS | " + model.Comment + " - User: " + userdetails.FirstName + " - Status:  " + serviceStatus.StatusName + " | ";
                                        apistatus.ObjectionOrRejectionCode = model.NICReasonId;
                                    }
                                }
                                else
                                {
                                    if (userdetails != null)
                                    {
                                        apistatus.Remarks = "REMARKS | " + model.Comment + " - User: " + userdetails.FirstName + " - Status:  " + serviceStatus.StatusName + " | ";
                                    }
                                }
                            }
                        }

                        // in case of completion only
                        if (model.StatusId == SWPStatusId.Completed)
                        {
                            if (model.RequestId > 0)
                            {
                                int _reqNo = model.RequestId > 0 ? (int)model.RequestId : 0;
                                var _reqDetails = _requestService.GetServiceRequestDetailById(_reqNo);

                                var _docName_FullPath = _reqDetails.ServiceModel.DispatchedDocument;
                                if (!string.IsNullOrEmpty(_docName_FullPath))
                                {
                                    apistatus.NOCCertificateNo = "Noida/Industry/2020/" + model.RequestId.ToString();
                                    apistatus.NOCUrl = _docName_FullPath;
                                    apistatus.IsCertificateValidLifeTime = apistatus.StatusCode == SWPStatus.CERTIFICATE_NO_ISSUED ? "Yes" : string.Empty;
                                    apistatus.StatusCode = SWPStatus.APPROVED;
                                }
                                else
                                {
                                    apistatus.StatusCode = SWPStatus.APPROVED;
                                }
                            }
                        }

                        _apiService.GetSWPServiceStatus(apistatus);
                    }
                }
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePaymentStatusByRequestId(int RequestId)
        {
            var flag = false;
            var niveshMitraDetails = new SWPApiServiceViewModel();
            //SWPTableViewModel _objNewDataSet = new SWPTableViewModel();
            niveshMitraDetails = _requestService.GetNiveshMitraServicesDetailsByApplicationId(RequestId);
            if (niveshMitraDetails != null)
            {
                var apistatus = new SWPStatusViewModel
                {
                    ControlId = niveshMitraDetails.NICControlId,
                    UnitId = niveshMitraDetails.NICUnitId,
                    ServiceId = niveshMitraDetails.NICServiceId,
                    RequestId = niveshMitraDetails.NICRequestId
                };

                var _NewDataSet = _apiService.GetSWPPaymentDetails(apistatus);

                if (_NewDataSet != null && _NewDataSet.Table != null)
                {
                    if (_NewDataSet.Table.Status_Code == SWPStatus.FEE_PAID)
                    {
                        var _ServiceRequestVM = new SWPServicesViewModel
                        {
                            Id = RequestId
                        };
                        var result = _requestService.UpdateRequestPaymentStatus(_ServiceRequestVM);
                        flag = result > 0 ? true : false;

                        if (flag)
                        {
                            // fee status
                            apistatus.ProcessIndustryId = niveshMitraDetails.NICProcessIndustryId;
                            apistatus.ProcessIndustryId = niveshMitraDetails.NICApplicationId;
                            apistatus.RequestId = niveshMitraDetails.NICRequestId;
                            //apistatus.PendancyLevel = SWPStatus.Wording.FEE_PAID;
                            apistatus.PendancyLevel = SWPStatus.Wording.HOD;
                            //apistatus.StatusCode = SWPStatus.FEE_PAID;
                            //apistatus.Remarks = SWPStatus.Wording.FEE_PAID;
                            apistatus.StatusCode = SWPStatus.FORM_SUBMITTED;
                            apistatus.Remarks = SWPStatus.Wording.FORM_SUBMITTED;
                            apistatus.FeeAmount = _NewDataSet.Table.Fee_Amount;
                            apistatus.FeeStatus = SWPStatus.Payment.C_PAID;

                            _apiService.GetSWPServiceStatus(apistatus);
                        }
                    }
                }
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UploadGeneratedLetterByserviceId(SWPServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> documentfiles)
        {
            int flag = _requestService.UploadGeneratedLetterByserviceId(model, documentfiles);

            var _customerServcieReq = _requestService.GetServiceRequestDetailById(model.RequestId);
            if (_customerServcieReq != null)
            {

                if (_customerServcieReq.ServiceModel.StatusId == SWPStatusId.Completed)
                {
                    var serviceStatus = new SWPApiServiceViewModel();
                    if (_customerServcieReq.ServiceModel.StatusId > 0)
                    {
                        //Get nivesh status code mappped to the customer request status
                        serviceStatus = _requestService.GetServiceStatusByCustomerRequestStatusId((int)_customerServcieReq.ServiceModel.StatusId);
                    }

                    //var _nicData = _requestService.GetNiveshMitraServicesByReqId(model.RequestId);
                    var _nicData = _requestService.GetServiceRequestDetailById(model.RequestId);
                    SWPStatusViewModel apistatus = new SWPStatusViewModel
                    {
                        ControlId = _nicData.SWPPostModel.TxtControlID,
                        ApplicationId = Convert.ToString(model.RequestId),
                        ProcessIndustryId = Convert.ToString(_customerServcieReq.ServiceModel.RegistrationId),
                        UnitId = _nicData.SWPPostModel.TxtUnitID,
                        ServiceId = _nicData.SWPPostModel.TxtServiceID,
                        StatusCode = _nicData.ServiceModel.NICStatusCode,
                        RequestId = _nicData.SWPPostModel.TxtRequestID,
                        PendancyLevel = _nicData.ServiceModel.PendencyLevel,
                        FeeStatus = string.Empty,
                        FeeAmount = string.Empty,
                        NICPassSalt = servicePassalt,
                    };

                    if (Session["CurrentUser"] != null)
                    {
                        var userdetails = _account.GetLoginAccountDetails(Session["UserName"].ToString()); //(CurrentUserDetail)Session["CurrentUser"];
                        apistatus.Remarks = "REMARKS | Certificate No Issued - User: " + userdetails.FirstName + " - Status:  " + serviceStatus.StatusName + " | ";
                        apistatus.PendancyLevel = "REMARKS | Certificate No Issued - User: " + userdetails.FirstName + " - Status:  " + serviceStatus.StatusName + " | ";
                    }
                    else
                    {
                        apistatus.Remarks = "REMARKS | Certificate No Issued - Status:  " + serviceStatus.StatusName + " | ";
                        apistatus.PendancyLevel = "REMARKS | Certificate No Issued - Status:  " + serviceStatus.StatusName + " | ";
                    }

                    if (model.RequestId > 0)
                    {
                        int _reqNo = model.RequestId > 0 ? (int)model.RequestId : 0;
                        var _reqDetails = _requestService.GetServiceRequestDetailById(_reqNo);

                        var _docName_FullPath = _reqDetails.ServiceModel.DispatchedDocument;
                        apistatus.NOCCertificateNo = "Noida/Industry/2020/" + model.RequestId.ToString();
                        apistatus.NOCUrl = _docName_FullPath;

                        //NICServiceApiHelper _apiService = new NICServiceApiHelper();
                        //SWPApiServiceProvider _apiService = new SWPApiServiceProvider();
                        _apiService.GetSWPServiceStatus(apistatus);
                    }
                }
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region NIC SERVICE REQUEST

        public ActionResult SaveNiveshMitraServiceUnit(SWPPostViewModel apimodel)
        {
            var service = _requestService.SaveNiveshMitraServiceUnit(apimodel);
            if (service.IsServiceExist == true)
            {
                Session["NIC_Service"] = apimodel;
                return RedirectToAction("RequestService", new { TxtControlID = apimodel.TxtControlID, TxtUnitID = apimodel.TxtUnitID, TxtServiceID = apimodel.TxtServiceID, TxtProcessIndustryID = apimodel.TxtProcessIndustryID, TxtRequestID = apimodel.TxtRequestID });
            }
            else
            {
                TempData["Save_ErrorMessage"] = "Provider failed to save Service.";
                return RedirectToAction("ValidateProperty", new { TxtControlID = apimodel.TxtControlID, TxtUnitID = apimodel.TxtUnitID, TxtServiceID = apimodel.TxtServiceID, TxtProcessIndustryID = apimodel.TxtProcessIndustryID, TxtRequestID = apimodel.TxtRequestID });
            }
        }

        //[HttpPost]
        public ActionResult SaveServiceRequest(SWPServicesViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            if (model != null)
            {
                var service = _requestService.SaveServiceRequestDetail(model, files);
                if (service != null)
                {
                    var serviceDetails = _requestService.GetCitizenServiceDetails(model.ServiceModel.ServiceId, model.ServiceModel.DepartmentId);
                    //var serviceDetails = _requestService.GetSWPServiceDetailById(new SWPApiViewModel { NICServiceCode = model.ServiceModel.NICStatusCode, DepartmentId= model.ServiceModel.DepartmentId});
                    SWPStatusViewModel apistatus = new SWPStatusViewModel
                    {
                        ControlId = model.SWPPostModel.TxtControlID,
                        ApplicationId = Convert.ToString(model.ServiceModel.RequestId),
                        ProcessIndustryId = model.SWPPostModel.TxtProcessIndustryID,
                        UnitId = model.SWPPostModel.TxtUnitID,
                        ServiceId = model.SWPPostModel.TxtServiceID,
                        RequestId = string.IsNullOrEmpty(model.SWPPostModel.TxtRequestID) ? service.ServiceModel.NICRequestId : model.SWPPostModel.TxtRequestID,
                        Department = model.ServiceModel.Department,
                        DepartmentId = model.ServiceModel.DepartmentId.Value,
                        
                        NICPassSalt = servicePassalt
                    };
                    
                    if (serviceDetails != null)
                    {
                        // In case of paid service
                        if (serviceDetails.ServiceFee > 0)
                        {
                            apistatus.StatusCode = SWPStatus.FEE_PENDING;
                            apistatus.Remarks = SWPStatus.Wording.FEE_PENDING;
                            //apistatus.PendancyLevel = SWPStatus.Wording.FEE_PENDING;
                            apistatus.PendancyLevel = SWPStatus.Wording.ENTREPRENEUR;
                            apistatus.FeeAmount = serviceDetails.ServiceFee.ToString();
                            apistatus.FeeStatus = SWPStatus.Payment.UB;

                            //_apiService.GetSWPServiceStatus(apistatus);
                        }
                        else
                        {
                            apistatus.StatusCode = SWPStatus.FORM_SUBMITTED;
                            apistatus.Remarks = SWPStatus.Wording.FORM_SUBMITTED;
                            //apistatus.PendancyLevel = SWPStatus.Wording.FORM_SUBMITTED;
                            apistatus.PendancyLevel = SWPStatus.Wording.HOD;
                            apistatus.FeeAmount = string.Empty;
                            apistatus.FeeStatus = string.Empty;
                        }
                    }

                    apistatus = _apiService.GetSWPServiceStatus(apistatus);
                    service.SWPApiModel.Status = apistatus.Status;

                    return RedirectToAction("RequestDetail", new { id = model.ServiceModel.RequestId });
                }
                else
                {
                    return RedirectToAction("RequestService", new { TxtControlID = model.SWPPostModel.TxtControlID, TxtUnitID = model.SWPPostModel.TxtUnitID, TxtServiceID = model.SWPPostModel.TxtServiceID, TxtProcessIndustryID = model.SWPPostModel.TxtProcessIndustryID, TxtRequestID = model.SWPPostModel.TxtRequestID });
                    //service.SWPApiModel.Status = "FAILED";
                }
                //return Json(service, JsonRequestBehavior.AllowGet);
            }
            else
            {
                TempData["ErrorMessage_NIC"] = "Only Nivesh Mitra Services allowed";
                return View("Error");
            }
        }

        public JsonResult GetApplicantDetailsByRegistrationId(int? registrationId)
        {
            var detail = _requestService.GetApplicantDetailsByRegistrationId(registrationId);
            return Json(detail, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ReSubmitRequest(SWPServicesViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int? requestId = 0;
            if (model != null)
            {
                var service = _requestService.ReSubmitRequest(model, files);
                if (service != null)
                {
                    SWPStatusViewModel apistatus = new SWPStatusViewModel();
                    apistatus.ControlId = service.SWPPostModel.TxtControlID;
                    apistatus.ApplicationId = Convert.ToString(model.OnlineRequestId);
                    apistatus.ProcessIndustryId = service.SWPPostModel.TxtProcessIndustryID;
                    apistatus.UnitId = service.SWPPostModel.TxtUnitID;
                    apistatus.ServiceId = service.SWPPostModel.TxtServiceID;
                    apistatus.RequestId = service.SWPPostModel.TxtRequestID;
                    apistatus.StatusCode = SWPStatus.FORM_RE_SUBMITTED;
                    apistatus.Remarks = SWPStatus.Wording.FORM_RE_SUBMITTED;
                    //apistatus.PendancyLevel = SWPStatus.Wording.FORM_RE_SUBMITTED;
                    apistatus.PendancyLevel = SWPStatus.Wording.ASSISTANT;
                    apistatus.FeeStatus = string.Empty;
                    apistatus.FeeAmount = string.Empty;
                    apistatus.NICPassSalt = servicePassalt;

                    requestId = service.ServiceModel.RequestId;

                    _apiService.GetSWPServiceStatus(apistatus);
                }
            }
            //return RedirectToAction("RequestService");
            return RedirectToAction("RequestDetail", new { id = requestId });
        }

        public ActionResult ValidateNiveshMitraServiceUnit(SWPApiServiceViewModel model)
        {
            var detail = _requestService.ValidateNiveshMitraServiceUnit(model);
            if (detail.IsPropertyDifferent == false && detail.IsKYADone == true)
            {
                // if KYA done OTP is sent to registered Mobile no.
                //detail.OTP = 123;
                detail.OTP = GetSentOTP(detail.MobileNo, detail.Email);
            }
            return Json(detail, JsonRequestBehavior.AllowGet);
        }

        // Check Registration Id Exists and KYA done or not.
        public JsonResult ValidateUnit(int? registrationId)
        {
            var detail = _requestService.ValidateRegistrationId(registrationId);
            // if KYA done OTP is sent to registered Mobile no.
            if (detail.IsKYADone == true)
            {
                //detail.OTP = 123;
                detail.OTP = GetSentOTP(detail.Mobile, detail.Email);
            }
            return Json(detail, JsonRequestBehavior.AllowGet);
        }

        private int GetSentOTP(string mobileNo, string email)
        {
            int _otp = SWPApplication.GenerateOTP();
            // _otp = 123;
            Session["NIC_UnitValidation_OTP"] = _otp;

            string emailMessage = string.Format(SWPMessage.OnlineApplicationOTP, _otp);
            string mobileMessage = string.Format(SWPMessage.OnlineApplicationOTP, _otp);
            if (mobileNo != null && mobileNo != "") SWPApplication.SendSMS(mobileNo, mobileMessage);
            if (email != null && email != "") SWPApplication.SendEmail(email, "Nivesh Mitra Services", emailMessage);

            return _otp;
        }

        private JsonResult SendOTP(string mobileNo, string email)
        {
            int _otp = SWPApplication.GenerateOTP();
            // _otp = 123;
            Session["NIC_UnitValidation_OTP"] = _otp;

            string emailMessage = string.Format(SWPMessage.OnlineApplicationOTP, _otp);
            string mobileMessage = string.Format(SWPMessage.OnlineApplicationOTP, _otp);
            if (mobileNo != null && mobileNo != "") SWPApplication.SendSMS(mobileNo, mobileMessage);
            if (email != null && email != "") SWPApplication.SendEmail(email, "OnlineForm", emailMessage);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateOTP(string otpMobile)
        {
            int flag = 0;
            if (Session["NIC_UnitValidation_OTP"] != null)
            {
                if ((int)Session["NIC_UnitValidation_OTP"] == Convert.ToInt32(otpMobile)) { flag = SWPReturnTypeId.Success; }
            }
            else { flag = SWPReturnTypeId.Failure; }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [Director Methods]

        public JsonResult GetDirectorShareholderDetails([DataSourceRequest] DataSourceRequest request)
        {
            if (Session["TempDirectors"] != null)
            {
                var directorList = (List<SWPShareholderViewModel>)Session["TempDirectors"];
                return Json(directorList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveDirectorOrShareholders(string directorName, decimal? share, string shareType)
        {
            var shares = SaveDirectorOrShareholder(directorName, share, shareType);
            return Json(shares, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveDirectorShareholderDetail(int id)
        {
            var flag = false;
            var directorList = (List<SWPShareholderViewModel>)Session["TempDirectors"];
            if (directorList != null)
            {
                directorList.RemoveAt(id - 1);
                flag = true;
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [File Checklist - Service Wise]

        public JsonResult GetFileUploadOptionsForService(int? departmentId, int? serviceId)
        {
            var checkList = _requestService.GetFileUploadHtmlForService(departmentId, serviceId);
            if (checkList != null) return Json(checkList, JsonRequestBehavior.AllowGet);
            else return Json("NotExist", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceRequestDocumentsById([DataSourceRequest]DataSourceRequest request, SWPServiceViewModel model)
        {
            var reports = _requestService.GetServiceRequestUploadedDocumentsById(request, model);
            return Json(reports, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFileUploadOptionsForService_Objection(int? departmentId, int? serviceId)
        {
            string divMain = "";
            var checklists = new List<SWPCheckListViewModel>();
            checklists.Add(new SWPCheckListViewModel { ChecklistName = "File 1", ServiceId = serviceId, DepartmentId = departmentId });
            checklists.Add(new SWPCheckListViewModel { ChecklistName = "File 2", ServiceId = serviceId, DepartmentId = departmentId });
            if (checklists.Count != 0)
            {
                divMain = "<table class='row table table-responsive table-bordered'>"
                                + "<thead><tr><th>Serial No</th>"
                                + "<th>Required File</th>"
                                + "<th>Select File <br/> <small>only pdf,jpg & jpeg</small></th>"
                            + "</tr></thead><tbody>";

                int docCounter = 1;
                foreach (var item in checklists)
                {
                    divMain = divMain + "<tr>"
                                           + "<td><label>" + docCounter + "</label></td>"
                                           + "<td><label>" + item.ChecklistName + "</label></td>"
                                           + "<td style='width:40%'><input type='file' class='single' name='files' /></td>"
                                      + "</tr>";
                    docCounter++;
                }

                divMain = divMain + "</tbody></table>";
            }
            if (divMain != null) return Json(divMain, JsonRequestBehavior.AllowGet);
            else return Json("NotExist", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [Department, ServiceListByDepartment, SubDepartment, FirmStatus, MortgageType, NOCStatus, GPAStatus, Gender, Occupation, CICRequestType]

        public JsonResult GetDepartmentList()
        {
            //var department = _generalService.GetDepartmentList();
            var department = _generalService.GetDepartmentList();
            return Json(department, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceListByDepartment(int departmentId)
        {
            var services = _requestService.GetServiceListByDepartmentForNIC(departmentId);
            return Json(services, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubDepartmentList(int departmentId)
        {
            //var department = _generalService.GetSubDepartmentList(departmentId);
            var department = _generalService.GetSubDepartmentList(departmentId);
            return Json(department, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFirmStatusList()
        {
            //var typeList = _generalService.GetFirmStatusList();
            var typeList = _generalService.GetFirmStatusList();
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMortgageTypeList()
        {
            //var typeList = _generalService.GetMortgageTypeList();
            var typeList = _generalService.GetMortgageTypeList();
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNOCStatusList()
        {
            //var typeList = _generalService.GetNOCStatusList();
            var typeList = _generalService.GetNOCStatusList();
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGPAStatusList()
        {
            //var statusList = _generalService.GetGPAStatusList();
            var statusList = _generalService.GetGPAStatusList();
            return Json(statusList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGenderList()
        {
            //var gender = _generalService.GetGenderList();
            var gender = _generalService.GetGenderList();
            return Json(gender, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOccupationList()
        {
            //var occupation = _generalService.GetOccupationList();
            var occupation = _generalService.GetOccupationList();
            return Json(occupation, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCICRequestTypeList()
        {
            //var occupation = _generalService.GetCICRequestTypeList();
            var occupation = _generalService.GetCICRequestTypeList();
            return Json(occupation, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferTypeList()
        {
            //var transferList = _generalService.GetTransferTypeList();
            var transferList = _generalService.GetTransferTypeList();
            return Json(transferList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferSubTypeList(int transferTypeId)
        {
            //var transferSubList = _generalService.GetTransferSubTypeList(transferTypeId);
            var transferSubList = _generalService.GetTransferSubTypeList(transferTypeId);
            return Json(transferSubList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanyMemberTypeList()
        {
            //var typeList = _generalService.GetCompanyMemberTypeList();
            var typeList = _generalService.GetCompanyMemberTypeList();
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [Private Methods]

        private List<SWPShareholderViewModel> SaveDirectorOrShareholder(string directorName, decimal? share, string shareType)
        {
            try
            {
                var directors = (List<SWPShareholderViewModel>)Session["TempDirectors"];
                if (directors == null)
                {
                    directors = new List<SWPShareholderViewModel>();
                    directors.Add(new SWPShareholderViewModel
                    {
                        Id = 1,
                        ShareType = Convert.ToInt32(shareType),
                        ShareholderName = directorName,
                        ShareValue = share
                    });
                    Session["TempDirectors"] = directors;
                }
                else
                {
                    var flag = directors.Count;
                    directors.Add(new SWPShareholderViewModel
                    {
                        Id = flag + 1,
                        ShareType = Convert.ToInt32(shareType),
                        ShareholderName = directorName,
                        ShareValue = share
                    });
                    Session["TempDirectors"] = directors;
                }
                return directors;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        public ActionResult SaveSWPServices(SWPApiServiceViewModel apimodel)
        {
            apimodel = _requestService.SaveSWPServices(apimodel);
            return Json(apimodel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ActivateSWPServiceStatus(SWPApiServiceViewModel apimodel)
        {
            apimodel = _requestService.ActivateSWPServiceStatus(apimodel);
            return Json(apimodel, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public JsonResult GetDropDownListAsDataSource([DataSourceRequest] DataSourceRequest request, SWPDropdownViewModel model)
        {
            var list = _generalService.GetDropDownListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        
        [AllowAnonymous]
        public JsonResult GetEncryptedNicPostDetail(SWPPostViewModel model)
        {
            if (model != null)
            {
                string IsSWPTestApi = ConfigurationManager.AppSettings["IsSWPTestApi"];
                string passalt = ConfigurationManager.AppSettings["IsSWPTestApi"] == "true" ? ConfigurationManager.AppSettings["nictestpassalt"] : ConfigurationManager.AppSettings["ServicePassalt"];
                string swpBackUrl = ConfigurationManager.AppSettings["IsSWPTestApi"] == "true" ? ConfigurationManager.AppSettings["NiveshMitraPortalProd"] : ConfigurationManager.AppSettings["NiveshMitraPortalTest"]; 
      
                model.TxtControlID = string.IsNullOrEmpty(model.TxtControlID) ? string.Empty : SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", model.TxtControlID);
                model.TxtUnitID = string.IsNullOrEmpty(model.TxtUnitID) ? string.Empty : SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", model.TxtUnitID);
                model.TxtServiceID = string.IsNullOrEmpty(model.TxtServiceID) ? string.Empty : SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", model.TxtServiceID);
                model.TxtApplicationID = string.IsNullOrEmpty(model.TxtApplicationID) ? string.Empty : SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", model.TxtApplicationID);
                model.TxtProcessIndustryID = string.IsNullOrEmpty(model.TxtProcessIndustryID) ? string.Empty : SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", model.TxtProcessIndustryID);
                model.TxtDepartmentID = string.IsNullOrEmpty(model.TxtDepartmentID) ? string.Empty : SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", model.TxtDepartmentID);
                model.PassSalt = string.IsNullOrEmpty(model.PassSalt) ? SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", passalt) : SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", model.TxtDepartmentID);
                model.SWPBackUrl = swpBackUrl;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetServiceStatusReasonsAsDataSource([DataSourceRequest] DataSourceRequest request, SWPApiServiceViewModel model)
        {
            var list = _requestService.GetServiceStatusReasonsAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSWPServiceDetailById(SWPApiViewModel model)
        {
            model = _requestService.GetSWPServiceDetailById(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyStatusListAsDataSource([DataSourceRequest]DataSourceRequest request, SWPDropdownViewModel model)
        {
            return Json(_generalService.GetStatusMasterAsDataSource(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNiveshMitraStatusListAsDataSource([DataSourceRequest]DataSourceRequest request, SWPDropdownViewModel model)
        {
            var list = _generalService.GetNiveshMitraStatusListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveNiveshMitraStatusDetail(SWPApiViewModel model)
        {
            model = _requestService.SaveNiveshMitraStatusDetail(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveReasonDetailForRejection(SWPApiViewModel model)
        {
            model = _requestService.SaveReasonDetailForRejection(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSWPServiceRequestIdForDropDownAsDataSource([DataSourceRequest]DataSourceRequest request, SWPDropdownViewModel model)
        {
            var data = _requestService.GetSWPServiceRequestIdForDropDownAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSWPServiceRequestDetailById(SWPStatusViewModel swpmodel)
        {
            SWPStatusViewModel data = _requestService.GetSWPServiceRequestDetailById(swpmodel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateNiveshMitraApiStatusService(SWPStatusViewModel model)
        {
            var data = _apiService.GetSWPServiceStatus(model);
            return Json(data,JsonRequestBehavior.AllowGet);
        }
    }
}
