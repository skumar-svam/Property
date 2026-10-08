using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Web.Controllers.Common;
using NA.PMS.Common;
using NA.PMS.Web.Controllers;

namespace NA.PMS.Web.Areas.Customer.Controllers
{
    public class PropertyController : PISWebBaseController
    {
        private ICustomerService _customerService;
        private IGeneralService _generalService;
        private IRequestService _requestService;
        public PropertyController(ICustomerService customerService, IGeneralService generalService, IRequestService requestService)
        {
            _customerService = customerService;
            _generalService = generalService;
            _requestService = requestService;
        }

        public int GetRegistrationId()
        {
            int rid = 0;
            if (Session["CurrentCustomer"] != null)
            {
                var user = (UserViewModel)Session["CurrentCustomer"];
                rid = Convert.ToInt32(user.UserName);
            }
            return rid;
        }

        public CustomerDetailViewModel GetCustomerDetails()
        { 
            if (Session["CurrentCustomer"] != null)
            {
                var user = (UserViewModel)Session["CurrentCustomer"];
                var data = _customerService.GetCustomerDetails(Convert.ToInt32(user.UserName));
                return data;
            }
            return null;
        }

        public ActionResult Index()
        {
            int rid = GetRegistrationId();
            if (rid != 0)
            {
                var data = _customerService.GetCustomerDetails(rid);
                return View(data);
            }
            else
            {
                return RedirectToAction("Logout", "PISAccount", new { area = "Customer" });
            }            
        }

        public ActionResult KYAForm()
        {
            return View();
        }

        public ActionResult Profile(string id="10000013")
        {
            int rid = GetRegistrationId();
            if (rid != 0)
            {
                var data = _customerService.GetCustomerDetailById(rid);
                return View(data);
            }
            else
            {
                rid = Convert.ToInt32(id);
                //var data = _customerService.GetCustomerDetailById(Convert.ToInt32(id));
                return RedirectToAction("Logout", "PISAccount", new { area = "Customer" });
            }           
        }

        public ActionResult PropertyDetail(string id="10000013")//, string optString
        {
            int rid = GetRegistrationId();
            if (rid != 0)
            {
                var propertyDetail = _customerService.GetCustomerDetails(rid);
                return View(propertyDetail);
            }
            else
            {
                rid = Convert.ToInt32(id);
                return RedirectToAction("Logout", "PISAccount", new { area = "Customer" });
            }
        }

        public ActionResult PaymentHistory()
        {
            int rid = GetRegistrationId();
            if (rid != 0)
            {
                var model = _customerService.GetCustomerDetails(rid);
                return View(model);
            }
            else
            {
                return RedirectToAction("Logout", "PISAccount", new { area = "Customer" });
            }
        }

        public ActionResult NoidaJal(int? id)
        {
            int rid = GetRegistrationId();
            if (rid != 0)
            {
                var model = _customerService.GetCustomerDetails(rid);
                return View(model);
            }
            else
            {
                return RedirectToAction("Logout", "PISAccount", new { area = "Customer" });
            }
        }

        public ActionResult Litigation(int? id)
        {
            int rid = GetRegistrationId();
            if (rid != 0)
            {
                var model = _customerService.GetCustomerDetails(rid);
                return View(model);
            }
            else
            {
                return RedirectToAction("Logout", "PISAccount", new { area = "Customer" });
            }
        }

        public ActionResult Notices()
        {
            int rid = GetRegistrationId();
            if (rid != 0)
            {
                var model = _customerService.GetCustomerDetails(rid);
                return View(model);
            }
            else
            {
                return RedirectToAction("Logout", "PISAccount", new { area = "Customer" });
            }
        }

        public ActionResult ServiceHistory()
        {
            int rid = GetRegistrationId();
            if (rid != 0)
            {
                var model = _customerService.GetCustomerDetails(rid);
                return View(model);
            }
            else
            {
                return RedirectToAction("Logout", "PISAccount", new { area = "Customer" });
            }
        }

        public ActionResult ServiceRequest(string id)
        {
            int rid = GetRegistrationId();
            if (rid != 0)
            {
                var propertyDetail = _customerService.GetPropertyDetailForServiceRequestById(rid);
                return View(propertyDetail);
            }
            else
            {
                rid = !string.IsNullOrEmpty(id) ? Convert.ToInt32(CommonHelper.Decode(id)) : 10000013;
                //var propertyDetail = _customerService.GetPropertyDetailForServiceRequestById(rid);
                return RedirectToAction("Logout", "PISAccount", new { area = "Customer" });
            }           
        }

        [HttpPost]
        public ActionResult ServiceRequest(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var detail = _customerService.SaveServiceRequestDetail(model, files);
            return Json(detail,JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult PropertyHistory()
        {
            int rid = GetRegistrationId();
            if (rid != 0)
            {
                var model = _customerService.GetCustomerDetails(rid);
                return View(model);
            }
            else
            {
                return RedirectToAction("Logout", "PISAccount", new { area = "Customer" });
            }
        }

        public ActionResult Document()
        {
            int rid = GetRegistrationId();
            if (rid != 0)
            {
                var model = _customerService.GetCustomerDetails(rid);
                return View(model);
            }
            else
            {
                return RedirectToAction("Logout", "PISAccount", new { area = "Customer" });
            }
        }

        public JsonResult GetScannedDocumentListById([DataSourceRequest]DataSourceRequest request, DocumentViewModel model)
        {
            var data = _customerService.GetScannedDocumentListById(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGeneratedDocumentListById([DataSourceRequest]DataSourceRequest request, DocumentViewModel model)
        {
            var data = _customerService.GetGeneratedDocumentListById(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ServiceDetail(string id)
        {
            int sid = !string.IsNullOrEmpty(id) ? Convert.ToInt32(CommonHelper.Decode(id)) : 10000013;
            var details = _customerService.GetServiceRequestDetailById(sid);
            return View(details);
        }

        public JsonResult GetPaymentReceiptScheduleListById([DataSourceRequest]DataSourceRequest request, PaymentViewModel model)
        {
            var data = _customerService.GetPaymentReceiptScheduleListById(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentScheduleDataListById([DataSourceRequest]DataSourceRequest request, int? rid)
        {
            var data = _customerService.GetPaymentScheduleDataListById(request, rid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentRescheduledListById([DataSourceRequest]DataSourceRequest request, int? rid)
        {
            var data = _customerService.GetPaymentRescheduledListById(request, rid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentLedgerDataListById([DataSourceRequest]DataSourceRequest request, int? rid)
        {
            var data = _customerService.GetPaymentLedgerDataListById(request, rid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetServiceHistoryDataListById([DataSourceRequest]DataSourceRequest request, ServiceViewModel model)
        {
            var data = _customerService.GetServiceHistoryDataListById(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        ////To get all blocks in ddl
        //public JsonResult GetBlocks()
        //{
        //    var lst = _generalService.GetAllBlocks();
        //    return Json(lst, JsonRequestBehavior.AllowGet);
        //}

        //////To get all sector in ddl
        //public JsonResult GetSector()
        //{
        //    var lst = _generalService.getSectorsList();
        //    return Json(lst, JsonRequestBehavior.AllowGet);
        //}

        [AllowAnonymous]
        public JsonResult GetRegistrationIdListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _customerService.GetRegistrationIdListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        
        ////To get all sector in ddl
        [AllowAnonymous]
        public JsonResult GetSectorListAsDataSource([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var list = _generalService.GetSectorListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        ////To get all blocks in ddl
        [AllowAnonymous]
        public JsonResult GetBlockListAsDataSource([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var list = _generalService.GetBlockListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetSecurityQuestioinListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetSecurityQuestioinListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult RegisterCustomer()
        {
            ViewBag.Title = "Register Customer";
            var model = new NACustomer();
            model.RandomKeyVal = Common.ApplicationHelper.RandomString(7);
            model.CaptchaInput = Common.ApplicationHelper.RandomString(7);
            model.Status = true;
            return View(model);
        }

        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult RegisterCustomer(NACustomer model, IEnumerable<HttpPostedFileBase> files)
        {
            int flag = _customerService.RegisterCustomerDetails(model, files);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RegisterCustomerDetails(NACustomer model, HttpPostedFileBase upload1, HttpPostedFileBase upload2)
        {
            var result = false;
            model.Status = true;
            if (ModelState.IsValid)
            {
                if (upload1 != null && upload1.ContentLength > 0)
                {
                    if (model.CustomerIdFiletype.Equals("PANCard"))
                    {
                        model.CustomerIdFileName = upload1.FileName;
                    }
                    if (model.CustomerIdFiletype.Equals("AadhaarCard"))
                    {
                        model.CustomerIdFileName = upload1.FileName;
                    }
                }
                if (upload2 != null && upload2.ContentLength > 0)
                {
                    if (model.AuthorityLetterType.Equals("AllotmentLetter"))
                    {
                        model.AuthorityLetter = upload2.FileName;
                    }
                    if (model.AuthorityLetterType.Equals("TransferLetter"))
                    {
                        model.AuthorityLetter = upload2.FileName;
                    }
                }
                //result = _customerService.RegisterCustomerDetails(model);
                if (!Directory.Exists(Server.MapPath(ConfigurationManager.AppSettings["FilePath"]).ToString() + model.RegistrationId))
                {
                    Directory.CreateDirectory(Server.MapPath(ConfigurationManager.AppSettings["FilePath"]).ToString() + model.RegistrationId);
                    upload1.SaveAs(Server.MapPath((ConfigurationManager.AppSettings["FilePath"]) + model.RegistrationId).ToString() + "/" + upload1.FileName);
                    if (upload2 != null)
                        upload2.SaveAs(Server.MapPath((ConfigurationManager.AppSettings["FilePath"]) + model.RegistrationId).ToString() + "/" + upload2.FileName);
                }
                else
                {
                    upload1.SaveAs(Server.MapPath((ConfigurationManager.AppSettings["FilePath"]) + model.RegistrationId).ToString() + "/" + upload1.FileName);
                    if (upload2 != null)
                        upload2.SaveAs(Server.MapPath((ConfigurationManager.AppSettings["FilePath"]) + model.RegistrationId).ToString() + "/" + upload2.FileName);
                }
            }

            if (result)
            {
                TempData["isUpdated"] = true;
            }

            if (!ModelState.IsValid)
            {
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        TempData["CaptchaError"] = "CaptchaError";

                    }
                }
            }
            // Updated changes for bug #6156z
            return RedirectToAction("CustomerBackToLogin");
        }


        [AllowAnonymous]
        public JsonResult CheckRegistationIDforNAcustomer(String RegistrationId)
        {
            var flag = false;
            try
            {
                var validateRegistrationId = new PMSMembershipProvider();
                flag = validateRegistrationId.CheckRegistationIDforNAcustomer(RegistrationId);
            }
            catch (Exception ex)
            {
                //logService.LogError("Error in ChecRegistrationIdForNAcustomerRegistation", ex);
                //throw;
            }
            return Json(flag);
        }

        [AllowAnonymous]
        public JsonResult PropertyDetailJson(int id)
        {
            DtoPropertyFilter PropertyFilterObj = new DtoPropertyFilter();
            int _RegistrationId = 0;
            if (int.TryParse(Convert.ToString(id), out _RegistrationId))
                PropertyFilterObj.RegistrationId = _RegistrationId;

            PropertyDetail propDetails = _customerService.GetPropertyDetails(PropertyFilterObj, 1);

            return Json(propDetails);
        }


        [AllowAnonymous]
        public JsonResult CheckEmailForNAcustomerRegistation(String emailId)
        {
            var flag = false;
            try
            {
                var CheckEmail = new PMSMembershipProvider();
                flag = CheckEmail.CheckEmailAddressForNAcustomer(emailId);
            }
            catch (Exception ex)
            {

            }
            return Json(flag);
        }

        [AllowAnonymous]
        public JsonResult SendCustomerOTP(string mobileNo)
        {
            bool flag = false;
            int otpNo = 0;
            if (ModelState.IsValid)
            {
                otpNo = ApplicationHelper.GenerateOTP();
                string msg1 = string.Format(NAMessages.PIS_otp, otpNo);
                if (otpNo > 0 && !string.IsNullOrEmpty(msg1))
                {
                    ApplicationHelper.SendSMS(mobileNo, msg1);
                    flag = true; //otpNo = 123;
                }
            }
            return Json(new { flag = flag, OTPVal = otpNo }, JsonRequestBehavior.AllowGet);
        }

        private PropertyDetail GetCitizenPropertyDetail(int id, string optString)
        {
            DtoPropertyFilter PropertyFilterObj = new DtoPropertyFilter();
            int _RegistrationId = 0;
            if (int.TryParse(Convert.ToString(id), out _RegistrationId))
                PropertyFilterObj.RegistrationId = _RegistrationId;
            PropertyDetail propDetails = _customerService.GetPropertyDetails(PropertyFilterObj, 2);
            return propDetails;
        }

        public JsonResult GetTransferHistoryDataListById(DataSourceRequest request, int? rid)
        {
            var data = _customerService.GetTransferHistoryDataListById(request, rid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMortgageHistoryDataListById(DataSourceRequest request, int? rid)
        {
            var data = _customerService.GetMortgageHistoryDataListById(request, rid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExtensionHistoryDataListById(DataSourceRequest request, int? rid)
        {
            var data = _customerService.GetExtensionHistoryDataListById(request, rid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceListByDepartment(int departmentId)
        {
            var data = _generalService.GetServiceListByDepartment(departmentId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubDepartmentList(int departmentId)
        {
            var data = _generalService.GetSubDepartmentList(departmentId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferTypeList()
        {
            var data = _generalService.GetTransferTypeList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferSubTypeList(int transferTypeId)
        {
            var data = _generalService.GetTransferSubTypeList(transferTypeId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGenderList()
        {
            var data = _generalService.GetGenderList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOccupationList()
        {
            var data = _generalService.GetOccupationList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCICRequestTypeList()
        {
            var data = _generalService.GetCICRequestTypeList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanyMemberTypeList()
        {
            var data = _generalService.GetCompanyMemberTypeList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFirmStatusList()
        {
            var data = _generalService.GetFirmStatusList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult GetGPAStatusList()
        {
            var data = _generalService.GetGPAStatusList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMortgageTypeList()
        {
            var data = _generalService.GetMortgageTypeList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNOCStatusList()
        {
            var data = _generalService.GetNOCStatusList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFileUploadOptionsForService(int? departmentId, int? serviceId)
        {
            var checkList = _customerService.GetFileUploadHtmlForService(departmentId, serviceId);
            if (checkList != null) return Json(checkList, JsonRequestBehavior.AllowGet);
            else return Json("NotExist", JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetDirectorShareholderDataList(DataSourceRequest request)
        {
            var data = _customerService.GetDirectorShareholderDataList(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDirectorOrShareholders(string directorName, decimal? share, string shareType)
        {
            var flag = _customerService.SaveDirectorOrShareholders(directorName, share, shareType);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveDirectorShareholderFromList(int id)
        {
            var flag = _customerService.RemoveDirectorShareholderFromList(id);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLegalHistory(int id)
        {
            DtoPropertyFilter PropertyFilterObj = new DtoPropertyFilter();
            int _RegistrationId = 0;
            if (int.TryParse(Convert.ToString(id), out _RegistrationId))
                PropertyFilterObj.RegistrationId = _RegistrationId;
            IEnumerable<DtoLegalHistory> propertyLegalHistory = _customerService.GetLegalHistoryByRegistrationId(PropertyFilterObj);
            return Json(propertyLegalHistory, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJalDetailsPaymentHistory(int id)
        {
            DtoPropertyFilter PropertyFilterObj = new DtoPropertyFilter();
            int _RegistrationId = 0;
            if (int.TryParse(Convert.ToString(id), out _RegistrationId))
                PropertyFilterObj.RegistrationId = _RegistrationId;
            IEnumerable<DtoJalDetailsPaymentHistory> propertyJalPaymentHistory = _customerService.GetJalDetailsPaymentHistoryByRegistrationId(PropertyFilterObj);
            return Json(propertyJalPaymentHistory, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJalPaymentDataList([DataSourceRequest]DataSourceRequest request, JalViewModel model)
        {
            var list = _customerService.GetJalPaymentDataList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLitigationDataList([DataSourceRequest]DataSourceRequest request, LitigationViewModel model)
        {
            var list = _customerService.GetLitigationDataList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ValidateCustomerRegistration(CustomerViewModel model)
        {
            var data = _customerService.ValidateCustomerRegistration(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMasterSearchParameterAsDataSource([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var data = _generalService.GetMasterSearchParameterAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyDetailForKYA(KYAViewModel model)
        {
            var data = _customerService.GetPropertyDetailForKYA(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePropertyDetailForKYA(KYAViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int flag = _customerService.SavePropertyDetailForKYA(model, files);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePropertyDetailForKYAII(KYAViewModel model, HttpPostedFileBase idfile, HttpPostedFileBase letterfile, HttpPostedFileBase otherfile)
        {
            int flag = _customerService.SavePropertyDetailForKYAII(model, idfile,letterfile,otherfile);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllotteeIdTypeList([DataSourceRequest]DataSourceRequest request, DropdownViewModel model)
        {
            var data = _generalService.GetAllotteeIdTypeList(request,model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOwnershipFileTypeList([DataSourceRequest]DataSourceRequest request, DropdownViewModel model)
        {
            var data = _generalService.GetOwnershipFileTypeList(request,model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SendOTP(CommonViewModel model)
        {
           int flag = _generalService.SendOTP(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ValidateOTP(CommonViewModel model)
        {
            int flag = _generalService.ValidateOTP(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

       
    }
}