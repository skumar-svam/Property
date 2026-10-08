using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
//using NA.PMS.Common;
//using NA.PMS.Model;
//using NA.PMS.Model.NIC;
using NA.PMS.NICServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NA.PMS.NICController
{
    public class NiveshMitraServiceSuperController : Controller
    {
        //static string servicePassalt = ConfigurationManager.AppSettings["ServicePassalt"];
        //private INICRequestService _nicService;
        //private INICFormService _nicFormService;
        //private NiveshMitraHelper _nmHelper;
        //private NICServiceApiHelper _niveshMitraServices;

        //public NiveshMitraServiceSuperController()
        //{
        //    _nicFormService = new NICFormService();
        //    _nicService = new NICRequestService();
        //    _nmHelper = new NiveshMitraHelper();
        //    _niveshMitraServices = new NICServiceApiHelper();
        //    //_onlineService = onlineService;
        //}

        //#region

        //public ActionResult Error()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult NMUnitCheck(WBasicDetailsModel_NMS apidmodel = null)
        //{
        //    // For Nivesh Mitra Services
        //    if (apidmodel != null)
        //    {
        //        // to check unit already exits
        //        var data = _nicService.GetNiveshMitraServicesDetails(apidmodel);
        //        if (data != null)
        //        {
        //            apidmodel.TxtProcessIndustryID = data.ProcessIndustryID;
        //            Session["NIC_Service"] = apidmodel;
        //            //return RedirectToAction("Index");
        //            return RedirectToAction("Index", new { TxtControlID = apidmodel.TxtControlID, TxtUnitID = apidmodel.TxtUnitID, TxtServiceID = apidmodel.TxtServiceID, TxtProcessIndustryID = apidmodel.TxtProcessIndustryID });
        //        }
        //        else
        //        {
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage_NIC"] = "Only Nivesh Mitra Service allowed";
        //        return View("Error");
        //    }
        //}

        //public ActionResult Index(WBasicDetailsModel_NMS model)
        //{
        //    if (!string.IsNullOrEmpty(model.TxtControlID) && !string.IsNullOrEmpty(model.TxtUnitID) && !string.IsNullOrEmpty(model.TxtServiceID) && !string.IsNullOrEmpty(model.TxtProcessIndustryID))
        //    {
        //        //var apimodel = new WBasicDetailsModel_NMS { TxtControlID = controlId, TxtUnitID = unitId, TxtServiceID = serviceId, TxtProcessIndustryID = rid };
        //        if (Convert.ToInt32(model.TxtProcessIndustryID) > 0)
        //        {
        //            ServiceRequestVM service = _nicService.GetNiveshMitraServicesByRegistrationId(model);
        //            if (service != null)
        //            {
        //                // If already submitted the request
        //                if (!string.IsNullOrEmpty(service.NICModel.TxtApplicationID))
        //                {
        //                    var _reqId = Convert.ToInt32(service.NICModel.TxtApplicationID);
        //                    return RedirectToAction("ViewRequestDetail", new { id = _reqId });
        //                }
        //                return View(service);
        //            }
        //        }
        //    }
        //    TempData["ErrorMessage_NIC"] = "Nivesh Mitra Service Failed";
        //    return View("Error");
        //}

        //public ActionResult SaveUnit(WBasicDetailsModel_NMS objWBasicDetailsPostModel)
        //{
        //    var service = _nicService.SaveNiveshMitraUnit(objWBasicDetailsPostModel);
        //    if (service)
        //    {
        //        #region commented on 27 june 2019  [send in process to NIC service]

        //        //WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel();
        //        //objWReturn_CUSID_STATUSModel.ControlID = objWBasicDetailsPostModel.TxtControlID;
        //        //objWReturn_CUSID_STATUSModel.ApplicationID = string.Empty;
        //        //objWReturn_CUSID_STATUSModel.ProcessIndustryID = objWBasicDetailsPostModel.TxtProcessIndustryID;
        //        //objWReturn_CUSID_STATUSModel.UnitID = objWBasicDetailsPostModel.TxtUnitID;
        //        //objWReturn_CUSID_STATUSModel.ServiceID = objWBasicDetailsPostModel.TxtServiceID;
        //        //objWReturn_CUSID_STATUSModel.Status_Code = ServiceStatus.INPROCESS;
        //        //objWReturn_CUSID_STATUSModel.Remarks = ServiceStatus_Text.INPROCESS;
        //        //objWReturn_CUSID_STATUSModel.Fee_Status = string.Empty;
        //        //objWReturn_CUSID_STATUSModel.Fee_Amount = string.Empty;
        //        //objWReturn_CUSID_STATUSModel.passsalt = servicePassalt;

        //        //_niveshMitraServices.GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);

        //        #endregion

        //        Session["NIC_Service"] = objWBasicDetailsPostModel;
        //        return RedirectToAction("Index");
        //    }

        //    // In case of error
        //    TempData["ErrorMessage_NIC"] = "Service Failure";
        //    return View("Error");
        //}

        //public ActionResult SaveNiveshMitraServiceUnit(WBasicDetailsModel_NMS apimodel)
        //{
        //    var service = _nicService.SaveNiveshMitraServiceUnit(apimodel);
        //    if (service.IsServiceExist == true)
        //    {
        //        Session["NIC_Service"] = apimodel;
        //        return RedirectToAction("Index", new { TxtControlID = apimodel.TxtControlID, TxtUnitID = apimodel.TxtUnitID, TxtServiceID = apimodel.TxtServiceID, TxtProcessIndustryID = apimodel.TxtProcessIndustryID });
        //    }
        //    else
        //    {
        //        TempData["Save_ErrorMessage"] = "Provider failed to save Service.";
        //        return RedirectToAction("NMUnitCheck", new { TxtControlID = apimodel.TxtControlID, TxtUnitID = apimodel.TxtUnitID, TxtServiceID = apimodel.TxtServiceID, TxtProcessIndustryID = apimodel.TxtProcessIndustryID });
        //    }
        //}

        //[HttpPost]
        //public ActionResult SaveServiceRequest(ServiceRequestVM model, IEnumerable<HttpPostedFileBase> files)
        //{
        //    if (model != null)
        //    {
        //        var service = _nicService.SaveServiceRequestDetail(model, files);
        //        if (service != null)
        //        {
        //            WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel
        //            {
        //                ControlID = model.NICModel.TxtControlID,
        //                ApplicationID = Convert.ToString(model.ServiceModel.RequestId),
        //                ProcessIndustryID = model.NICModel.TxtProcessIndustryID,
        //                UnitID = model.NICModel.TxtUnitID,
        //                ServiceID = model.NICModel.TxtServiceID,
        //                Status_Code = ServiceStatus.FORM_SUBMITTED,
        //                Remarks = ServiceStatus_Text.FORM_SUBMITTED,
        //                Fee_Status = string.Empty,
        //                Fee_Amount = string.Empty,
        //                passsalt = servicePassalt
        //            };

        //            _niveshMitraServices.GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);

        //            var serviceDetails = _nicService.GetCitizenServiceDetails(model.ServiceModel.ServiceId, model.ServiceModel.DepartmentId);
        //            if (serviceDetails != null)
        //            {
        //                // In case of paid service
        //                if (serviceDetails.ServiceFee > 0)
        //                {
        //                    objWReturn_CUSID_STATUSModel.Status_Code = ServiceStatus.FEE_PENDING;
        //                    objWReturn_CUSID_STATUSModel.Remarks = ServiceStatus_Text.FEE_PENDING;
        //                    objWReturn_CUSID_STATUSModel.Fee_Amount = serviceDetails.ServiceFee.ToString();
        //                    objWReturn_CUSID_STATUSModel.Fee_Status = PaymentStatus_NIC.UB;

        //                    _niveshMitraServices.GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);
        //                }
        //            }
        //        }
        //        return Json(service, JsonRequestBehavior.AllowGet);
        //    }
        //    return View();
        //}

        //public JsonResult GetApplicantDetailsByRegistrationId(int? registrationId)
        //{
        //    var detail = _nicService.GetApplicantDetailsByRegistrationId(registrationId);
        //    return Json(detail, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult ReSubmitRequest(ServiceRequestVM model, IEnumerable<HttpPostedFileBase> files)
        //{
        //    if (model != null)
        //    {
        //        var service = _nicService.ReSubmitRequest(model, files);
        //        if (service != null)
        //        {
        //            WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel();
        //            objWReturn_CUSID_STATUSModel.ControlID = service.NICModel.TxtControlID;
        //            objWReturn_CUSID_STATUSModel.ApplicationID = Convert.ToString(model.OnlineRequestId);
        //            objWReturn_CUSID_STATUSModel.ProcessIndustryID = service.NICModel.TxtProcessIndustryID;
        //            objWReturn_CUSID_STATUSModel.UnitID = service.NICModel.TxtUnitID;
        //            objWReturn_CUSID_STATUSModel.ServiceID = service.NICModel.TxtServiceID;
        //            objWReturn_CUSID_STATUSModel.Status_Code = ServiceStatus.FORM_RE_SUBMITTED;
        //            objWReturn_CUSID_STATUSModel.Remarks = ServiceStatus_Text.FORM_RE_SUBMITTED;
        //            objWReturn_CUSID_STATUSModel.Fee_Status = string.Empty;
        //            objWReturn_CUSID_STATUSModel.Fee_Amount = string.Empty;
        //            objWReturn_CUSID_STATUSModel.passsalt = servicePassalt;

        //            _niveshMitraServices.GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}

        //public ActionResult ViewRequestDetail(int? id)
        //{
        //    if (id != null && id > 0)
        //    {
        //        var service = _nicService.GetServiceRequestDetailById(id);
        //        return View(service);
        //    }
        //    return null;
        //}

        //// Check Registration Id Exists and KYA done or not.
        //public JsonResult ValidateUnit(int? registrationId)
        //{
        //    var detail = _nicService.ValidateRegistrationId(registrationId);
        //    // if KYA done OTP is sent to registered Mobile no.
        //    if (detail.IsKYADone)
        //    {
        //        //detail.OTP = 123;
        //        detail.OTP = GetSentOTP(detail.Mobile, detail.Email);
        //    }
        //    return Json(detail, JsonRequestBehavior.AllowGet);
        //}

        //private int GetSentOTP(string mobileNo, string email)
        //{
        //    int _otp = ApplicationHelper.GenerateOTP();
        //    // _otp = 123;
        //    Session["NIC_UnitValidation_OTP"] = _otp;

        //    string emailMessage = string.Format(NAMessages.OnlineApplicationOTP, _otp);
        //    string mobileMessage = string.Format(NAMessages.OnlineApplicationOTP, _otp);
        //    if (mobileNo != null && mobileNo != "") ApplicationHelper.SendSMS(mobileNo, mobileMessage);
        //    if (email != null && email != "") ApplicationHelper.SendEmail(email, "Nivesh Mitra Services", emailMessage);

        //    return _otp;
        //}

        //private JsonResult SendOTP(string mobileNo, string email)
        //{
        //    int _otp = ApplicationHelper.GenerateOTP();
        //    // _otp = 123;
        //    Session["NIC_UnitValidation_OTP"] = _otp;

        //    string emailMessage = string.Format(NAMessages.OnlineApplicationOTP, _otp);
        //    string mobileMessage = string.Format(NAMessages.OnlineApplicationOTP, _otp);
        //    if (mobileNo != null && mobileNo != "") ApplicationHelper.SendSMS(mobileNo, mobileMessage);
        //    if (email != null && email != "") ApplicationHelper.SendEmail(email, "OnlineForm", emailMessage);

        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult ValidateOTP(string otpMobile)
        //{
        //    int flag = 0;
        //    if (Session["NIC_UnitValidation_OTP"] != null)
        //    {
        //        if ((int)Session["NIC_UnitValidation_OTP"] == Convert.ToInt32(otpMobile)) { flag = ReturnType.Success; }
        //    }
        //    else { flag = ReturnType.Failure; }
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        //#endregion

        //#region [Director Methods]

        //public JsonResult GetDirectorShareholderDetails([DataSourceRequest] DataSourceRequest request)
        //{
        //    if (Session["TempDirectors"] != null)
        //    {
        //        var directorList = (List<DirectorShareholderVM>)Session["TempDirectors"];
        //        return Json(directorList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public JsonResult SaveDirectorOrShareholders(string directorName, decimal? share, string shareType)
        //{
        //    var shares = SaveDirectorOrShareholder(directorName, share, shareType);
        //    return Json(shares, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult RemoveDirectorShareholderDetail(int id)
        //{
        //    var flag = false;
        //    var directorList = (List<DirectorShareholderVM>)Session["TempDirectors"];
        //    if (directorList != null)
        //    {
        //        directorList.RemoveAt(id - 1);
        //        flag = true;
        //    }
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        //#endregion

        //#region [File Checklist - Service Wise]

        //public JsonResult GetFileUploadOptionsForService(int? departmentId, int? serviceId)
        //{
        //    var checkList = _nicService.GetFileUploadHtmlForService(departmentId, serviceId);
        //    if (checkList != null) return Json(checkList, JsonRequestBehavior.AllowGet);
        //    else return Json("NotExist", JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetServiceRequestDocumentsById([DataSourceRequest]DataSourceRequest request, ServiceVM model)
        //{
        //    var reports = _nicService.GetServiceRequestUploadedDocumentsById(request, model);
        //    return Json(reports, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetFileUploadOptionsForService_Objection(int? departmentId, int? serviceId)
        //{
        //    string divMain = "";
        //    var checklists = new List<ServiceCheckListVM>();
        //    checklists.Add(new ServiceCheckListVM { ChecklistName = "File 1", ServiceId = serviceId, DepartmentId = departmentId });
        //    checklists.Add(new ServiceCheckListVM { ChecklistName = "File 2", ServiceId = serviceId, DepartmentId = departmentId });
        //    if (checklists.Count != 0)
        //    {
        //        divMain = "<table class='row table table-responsive table-bordered'>"
        //                        + "<thead><tr><th>Serial No</th>"
        //                        + "<th>Required File</th>"
        //                        + "<th>Select File <br/> <small>only pdf,jpg & jpeg</small></th>"
        //                    + "</tr></thead><tbody>";

        //        int docCounter = 1;
        //        foreach (var item in checklists)
        //        {
        //            divMain = divMain + "<tr>"
        //                                   + "<td><label>" + docCounter + "</label></td>"
        //                                   + "<td><label>" + item.ChecklistName + "</label></td>"
        //                                   + "<td style='width:40%'><input type='file' class='single' name='files' /></td>"
        //                              + "</tr>";
        //            docCounter++;
        //        }

        //        divMain = divMain + "</tbody></table>";
        //    }
        //    if (divMain != null) return Json(divMain, JsonRequestBehavior.AllowGet);
        //    else return Json("NotExist", JsonRequestBehavior.AllowGet);
        //}

        //#endregion

        //#region [Department, ServiceListByDepartment, SubDepartment, FirmStatus, MortgageType, NOCStatus, GPAStatus, Gender, Occupation, CICRequestType]

        //public JsonResult GetDepartmentList()
        //{
        //    //var department = _generalService.GetDepartmentList();
        //    var department = _nmHelper.GetDepartmentList();
        //    return Json(department, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetServiceListByDepartment(int departmentId)
        //{
        //    var services = _nicService.GetServiceListByDepartmentForNIC(departmentId);
        //    return Json(services, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetSubDepartmentList(int departmentId)
        //{
        //    //var department = _generalService.GetSubDepartmentList(departmentId);
        //    var department = _nmHelper.GetSubDepartmentList(departmentId);
        //    return Json(department, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetFirmStatusList()
        //{
        //    //var typeList = _generalService.GetFirmStatusList();
        //    var typeList = _nmHelper.GetFirmStatusList();
        //    return Json(typeList, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetMortgageTypeList()
        //{
        //    //var typeList = _generalService.GetMortgageTypeList();
        //    var typeList = _nmHelper.GetMortgageTypeList();
        //    return Json(typeList, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetNOCStatusList()
        //{
        //    //var typeList = _generalService.GetNOCStatusList();
        //    var typeList = _nmHelper.GetNOCStatusList();
        //    return Json(typeList, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetGPAStatusList()
        //{
        //    //var statusList = _generalService.GetGPAStatusList();
        //    var statusList = _nmHelper.GetGPAStatusList();
        //    return Json(statusList, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetGenderList()
        //{
        //    //var gender = _generalService.GetGenderList();
        //    var gender = _nmHelper.GetGenderList();
        //    return Json(gender, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetOccupationList()
        //{
        //    //var occupation = _generalService.GetOccupationList();
        //    var occupation = _nmHelper.GetOccupationList();
        //    return Json(occupation, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetCICRequestTypeList()
        //{
        //    //var occupation = _generalService.GetCICRequestTypeList();
        //    var occupation = _nmHelper.GetCICRequestTypeList();
        //    return Json(occupation, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetTransferTypeList()
        //{
        //    //var transferList = _generalService.GetTransferTypeList();
        //    var transferList = _nmHelper.GetTransferTypeList();
        //    return Json(transferList, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetTransferSubTypeList(int transferTypeId)
        //{
        //    //var transferSubList = _generalService.GetTransferSubTypeList(transferTypeId);
        //    var transferSubList = _nmHelper.GetTransferSubTypeList(transferTypeId);
        //    return Json(transferSubList, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetCompanyMemberTypeList()
        //{
        //    //var typeList = _generalService.GetCompanyMemberTypeList();
        //    var typeList = _nmHelper.GetCompanyMemberTypeList();
        //    return Json(typeList, JsonRequestBehavior.AllowGet);
        //}

        //#endregion

        //#region [Private Methods]

        //private List<DirectorShareholderVM> SaveDirectorOrShareholder(string directorName, decimal? share, string shareType)
        //{
        //    try
        //    {
        //        var directors = (List<DirectorShareholderVM>)Session["TempDirectors"];
        //        if (directors == null)
        //        {
        //            directors = new List<DirectorShareholderVM>();
        //            directors.Add(new DirectorShareholderVM
        //            {
        //                Id = 1,
        //                ShareType = Convert.ToInt32(shareType),
        //                ShareholderName = directorName,
        //                ShareValue = share
        //            });
        //            Session["TempDirectors"] = directors;
        //        }
        //        else
        //        {
        //            var flag = directors.Count;
        //            directors.Add(new DirectorShareholderVM
        //            {
        //                Id = flag + 1,
        //                ShareType = Convert.ToInt32(shareType),
        //                ShareholderName = directorName,
        //                ShareValue = share
        //            });
        //            Session["TempDirectors"] = directors;
        //        }
        //        return directors;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        //#endregion
    }
}
