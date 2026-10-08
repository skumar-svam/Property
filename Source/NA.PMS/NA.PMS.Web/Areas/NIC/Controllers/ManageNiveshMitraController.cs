using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Model.NIC;
using NA.PMS.Service;
using NA.PMS.Web.Controllers;
using NA.PMS.Web.Controllers.Common;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace NA.PMS.Web.Areas.NIC.Controllers
{
    public class ManageNiveshMitraController : WebBaseController
    {
        static string servicePassalt = ConfigurationManager.AppSettings["ServicePassalt"];
        INICService _nicService;
        IGeneralService _generalService;
        public ManageNiveshMitraController(INICService nicService, IGeneralService generalService)
        {
            _nicService = nicService;
            _generalService = generalService;
        }

        // GET: NIC/ManageNiveshMitra
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NiveshMitraRequests()
        {
            return View();
        }

        public ActionResult Services()
        {
            return View();
        }

        public ActionResult AddEditServices(int? id)
        {
            return View();
        }


        public ActionResult ServiceStatus()
        {
            return View();
        }

        public ActionResult RequestDetail(int? id)
        {
            if (id != null && id > 0)
            {
                ServiceRequestVM service = _nicService.GetServiceRequestDetailById(id);
                service.ServiceModel.Comment = string.Empty;
                return View(service);
            }
            else return RedirectToAction("Index");
        }

        public JsonResult GetServiceListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var services = _generalService.GetServiceListAsDataSource(request);
            return Json(services, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNiveshMitraServiceRequests([DataSourceRequest]DataSourceRequest request)
        {
            var list = _nicService.GetNiveshMitraServiceRequests(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNiveshMitraServiceStatus([DataSourceRequest]DataSourceRequest request)
        {
            var list = _nicService.GetNiveshMitraServiceStatus(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNiveshMitraServices([DataSourceRequest]DataSourceRequest request)
        {
            var list = _nicService.GetNiveshMitraServices(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCitizenServiceDetailsbyServiceId([DataSourceRequest]DataSourceRequest request, string ServiceCode)
        {
            if (!string.IsNullOrEmpty(ServiceCode))
            {
                var filter = new FilterDescriptor { Member = "ServiceCode", Value = ServiceCode };
                request.Filters.Add(filter);
            }
            var list = _nicService.GetCitizenServiceDetailsbyServiceId(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerServiceRequestList([DataSourceRequest]DataSourceRequest request, int RequestNo)
        {
            if (RequestNo > 0)
            {
                var filter = new FilterDescriptor { Member = "Id", Value = RequestNo };
                request.Filters.Add(filter);
            }
            var list = _nicService.GetCustomerServiceRequestList_NIC(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerServiceRequestGrid([DataSourceRequest]DataSourceRequest request)
        {
            var list = _nicService.GetCustomerServiceRequestList_NIC(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetStatusMaster([DataSourceRequest]DataSourceRequest request, int? serviceId)
        {
            var user = Session["CurrentUser"] as CurrentUserDetail;
            List<int?> servicelist = new List<int?> { 12 };
            if (user != null)
            {
                var list = new List<DropdownViewModel>();
                var status = _generalService.GetStatusList();
                if (servicelist.Contains(serviceId))
                {
                    list.Add(new DropdownViewModel { Id = NAStatusId.Completed, Text = "Complete" });
                    list.Add(new DropdownViewModel { Id = NAStatusId.Forwarded, Text = "Forward" });
                    //list.Add(new DropdownViewModel { Id = NAStatusId.Rejected, Text = "Reject" });
                    list.Add(new DropdownViewModel { Id = NAStatusId.Objection, Text = "Objection" });
                    list.Add(new DropdownViewModel { Id = NAStatusId.Cancelled, Text = "Cancel" });
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
                return Json(_generalService.GetStatusMasterAsDataSource(request), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateCustomerServiceRequestStatus(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> letterfiles)
        {
            int flag = _nicService.UpdateCustomerServiceRequestStatus(model, letterfiles);
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
                    if (_nicData != null)
                    {
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
                                var _reqDetails = _nicService.GetServiceRequestDetailById(_reqNo);

                                var _docName_FullPath = _reqDetails.ServiceModel.DispatchedDocument;
                                if (!string.IsNullOrEmpty(_docName_FullPath))
                                {
                                    objWReturn_CUSID_STATUSModel.NOC_Certificate_Number = "Noida/Industry/2019/" + model.RequestId.ToString();
                                    objWReturn_CUSID_STATUSModel.NOC_URL = _docName_FullPath;
                                }
                                else
                                {
                                    objWReturn_CUSID_STATUSModel.Status_Code = Common.ServiceStatus.APPROVED;
                                }
                            }
                        }

                        NiveshMitraServices _niveshMitraServices = new NiveshMitraServices();
                        _niveshMitraServices.GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);
                    }
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
                        if (_objNewDataSet.Table.Status_Code == Common.ServiceStatus.FEE_PAID)
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
                                _WReturn_CUSID_STATUSModel.Fee_Amount = _objNewDataSet.Table.Fee_Amount;
                                _WReturn_CUSID_STATUSModel.Fee_Status = PaymentStatus_NIC.PAID;

                                _niveshMitraServices.GetWReturn_CUSID_STATUS(_WReturn_CUSID_STATUSModel);
                            }
                        }
                    }
                }
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        #region

        [HttpPost]
        public JsonResult UploadGeneratedLetterByserviceId(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> documentfiles)
        {
            int flag = _nicService.UploadGeneratedLetterByserviceId(model, documentfiles);

            var _customerServcieReq = _nicService.GetServiceRequestDetailById(model.RequestId);
            if (_customerServcieReq != null)
            {

                if (_customerServcieReq.ServiceModel.StatusId == NAStatusId.Completed)
                {
                    var serviceStatus = new ServiceStatusVM();
                    if (_customerServcieReq.ServiceModel.StatusId > 0)
                    {
                        //Get nivesh status code mappped to the customer request status
                        serviceStatus = _nicService.GetServiceStatusByCustomerRequestStatusId((int)_customerServcieReq.ServiceModel.StatusId);
                    }

                    var _nicData = _nicService.GetNiveshMitraServicesByReqId(model.RequestId);
                    WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel
                    {
                        ControlID = _nicData.NICModel.TxtControlID,
                        ApplicationID = Convert.ToString(model.RequestId),
                        ProcessIndustryID = Convert.ToString(_customerServcieReq.ServiceModel.RegistrationId),
                        UnitID = _nicData.NICModel.TxtUnitID,
                        ServiceID = _nicData.NICModel.TxtServiceID,
                        Status_Code = serviceStatus.StatusCode,
                        Fee_Status = string.Empty,
                        Fee_Amount = string.Empty,
                        passsalt = servicePassalt,
                    };

                    if (Session["CurrentUser"] != null)
                    {
                        var userdetails = (CurrentUserDetail)Session["CurrentUser"];
                        objWReturn_CUSID_STATUSModel.Remarks = "REMARKS | Certificate No Issued - User: " + userdetails.FirstName + " - Status:  " + serviceStatus.StatusName + " | ";
                    }
                    else
                    {
                        objWReturn_CUSID_STATUSModel.Remarks = "REMARKS | Certificate No Issued - Status:  " + serviceStatus.StatusName + " | ";
                    }

                    if (model.RequestId > 0)
                    {
                        int _reqNo = model.RequestId > 0 ? (int)model.RequestId : 0;
                        var _reqDetails = _nicService.GetServiceRequestDetailById(_reqNo);

                        var _docName_FullPath = _reqDetails.ServiceModel.DispatchedDocument;
                        objWReturn_CUSID_STATUSModel.NOC_Certificate_Number = "Noida/Industry/2019/" + model.RequestId.ToString();
                        objWReturn_CUSID_STATUSModel.NOC_URL = _docName_FullPath;

                        NiveshMitraServices _niveshMitraServices = new NiveshMitraServices();
                        _niveshMitraServices.GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);
                    }
                }
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}