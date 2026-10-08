using Kendo.Mvc.UI;
using NA.PMS.OnlineScheme;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
//using Newtonsoft.Json;


namespace NA.PMS.Web.Areas.Online.Controllers
{
    [AllowAnonymous]
    public class DefaultController : Controller
    {
        IDefaultSchemeRepository _defaultRepository;
        IOnlineSchemeRepository _schemeRepository;
        public string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;

        public DefaultController()
        {
            _defaultRepository = new DefaultSchemeRepository();
            _schemeRepository = new OnlineSchemeRepository();
        }

        [AllowAnonymous]
        public ActionResult SchemeFormApiChallan()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult SchemeFormTemplateChallan()
        {
            return View();
        }

        #region online scheme industrial 2017 sector 156

        ////return true if scheme end date is less than current date.
        [AllowAnonymous]
        public bool CheckSchemeCredentials()
        {
            bool flag = false;
            //var model = _defaultRepository.GetInitialDataForScheme();
            //if (model.SchemeEndDate != null)
            //{
            //    if (DateTime.Compare((DateTime)model.SchemeEndDate, DateTime.Now) < 0)
            //    {
            //        return flag = false; // true;
            //    }
            //}
            return flag;
        }

        public void Counter()
        {
            if (Session["OnlineApplicationSession"] == null)
            {
                string SessionID = this.HttpContext.Session.SessionID;
                Session["OnlineApplicationSession"] = SessionID;
                if (this.HttpContext.Application["Totaluser"] != null && this.HttpContext.Application["Totaluser"] != "")
                {
                    this.HttpContext.Application["Totaluser"] = Convert.ToInt32(this.HttpContext.Application["Totaluser"]) + 1;
                }
            }
        }

        [AllowAnonymous]
        public ActionResult HelpOnline()
        {
            return View();
        }

        [AllowAnonymous]
        public JsonResult GetSchemeListAsDataSource([DataSourceRequest] DataSourceRequest request, OSDropdownViewModel model)
        {
            var schemes = _defaultRepository.GetSchemeListAsDataSource(request, model);
            return Json(schemes, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDepartmentListAsDataSource([DataSourceRequest] DataSourceRequest request, OSDropdownViewModel model)
        {
            var departments = _defaultRepository.GetDepartmentListAsDataSource(request, model);
            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        //[AllowAnonymous]
        //public JsonResult GetPropertyTypeListAsDataSource([DataSourceRequest] DataSourceRequest request, OSDropdownViewModel model)
        //{
        //    var departments = _defaultRepository.GetPropertyTypeListAsDataSource(request, model);
        //    return Json(departments, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetDropdownPropertyListForAllotmentAsDataSource([DataSourceRequest] DataSourceRequest request, OSDropdownViewModel model)
        {
            var departments = _defaultRepository.GetDropdownPropertyListForAllotmentAsDataSource(request, model);
            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDropdownOnlineSchemeFormIdListForAllotmentAsDataSource([DataSourceRequest] DataSourceRequest request, OSDropdownViewModel model)
        {
            var departments = _defaultRepository.GetDropdownOnlineSchemeFormIdListForAllotmentAsDataSource(request, model);
            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDropdownOnlineSchemeFormIdListAsDataSource([DataSourceRequest] DataSourceRequest request, OSDropdownViewModel model)
        {
            var formIdList = _defaultRepository.GetDropdownOnlineSchemeFormIdListAsDataSource(request, model);
            return Json(formIdList, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetBankListAsDataSource([DataSourceRequest] DataSourceRequest request, OSDropdownViewModel model)
        {
            var banks = _defaultRepository.GetBankListAsDataSource(request, model);
            return Json(banks, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetBanksBranchListAsDataSource([DataSourceRequest] DataSourceRequest request, OSDropdownViewModel model)
        {
            var branchs = _defaultRepository.GetBanksBranchListAsDataSource(request, model);
            return Json(branchs, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public JsonResult GetPropertyTypeListAsDataSource([DataSourceRequest] DataSourceRequest request, OSDropdownViewModel model)
        {
            var propertyType = _defaultRepository.GetPropertyTypeListAsDataSource(request, model);
            return Json(propertyType, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetBankListBySchemeId(OSDropdownViewModel model)
        {
            var banks = _defaultRepository.GetBankListBySchemeId(model);
            return Json(banks, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetFloorAreaListAsDataSource([DataSourceRequest] DataSourceRequest request, OSDropdownViewModel model)
        {
            var floorArea = _defaultRepository.GetFloorAreaListAsDataSource(request, model);
            return Json(floorArea, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SchemeAreaRange()
        {
            return View();
        }


        //[AllowAnonymous]
        //public JsonResult GetOnlineApplications([DataSourceRequest] DataSourceRequest request, OnlineFormViewModel modal)
        //{
        //    var applications = _defaultRepository.GetOnlineApplications(request, modal);
        //    return Json(applications, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetOnlineApplicationsForAdmin([DataSourceRequest] DataSourceRequest request, OnlineFormViewModel modal)
        //{
        //    var applications = _defaultRepository.GetOnlineApplicationsForAdmin(request, modal);
        //    return Json(applications, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetOnlineApplicationsForConsultant([DataSourceRequest] DataSourceRequest request, OnlineFormViewModel modal)
        //{
        //    var applications = _defaultRepository.GetOnlineApplicationsForConsultant(request, modal);
        //    return Json(applications, JsonRequestBehavior.AllowGet);
        //}

        [AllowAnonymous]
        public JsonResult RejectSchemeApplicationForm(string AppId)
        {
            int _AppId = 0;
            int ApplicationId = 0;
            if (int.TryParse(Convert.ToString(AppId), out _AppId))
                ApplicationId = _AppId;
            var RejectAppStatus = _schemeRepository.ActionForOnlineSchemeApplicationForm(new SchemeFormViewModel { ApplicationFormId = ApplicationId, ActionType="RejectForm" });
            return Json(RejectAppStatus, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetGenderTypeList(OSDropdownViewModel model)
        {
            var list = _defaultRepository.GetGenderTypeList(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetMaritalStatusList(OSDropdownViewModel model)
        {
            var list = _defaultRepository.GetMaritalStatusList(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetCategoryList(OSDropdownViewModel model)
        {
            var list = _defaultRepository.GetCategoryList(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetOccupationList(OSDropdownViewModel model)
        {
            var list = _defaultRepository.GetOccupationList(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetSectorListAsDataSource([DataSourceRequest]DataSourceRequest request, OSDropdownViewModel model)
        {
            var sectors = _defaultRepository.GetSectorListAsDataSource(request, model);
            return Json(sectors, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetCompanyTypeList(OSDropdownViewModel model)
        {
            var floorArea = _defaultRepository.GetCompanyTypeListByCategory(model);
            return Json(floorArea, JsonRequestBehavior.AllowGet);
        }

        //[AllowAnonymous]
        //public JsonResult GetChecklistDocumentsForOnlineApplication([DataSourceRequest]DataSourceRequest request, int? schemeId)
        //{
        //    var allDocs = _defaultRepository.GetChecklistDocumentsForOnlineApplication(request, schemeId);
        //    return Json(allDocs, JsonRequestBehavior.AllowGet);
        //}

        [AllowAnonymous]
        public JsonResult GetChecklistDocumentsAsDataSource([DataSourceRequest]DataSourceRequest request, SchemeFormViewModel model)
        {
            var doclist = _schemeRepository.GetChecklistDocumentsAsDataSource(request, model);
            return Json(doclist, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetUploadedDocumentsAsDataSource([DataSourceRequest]DataSourceRequest request, SchemeFormViewModel model)
        {
            var doclist = _schemeRepository.GetUploadedDocumentsAsDataSource(request, model);
            return Json(doclist, JsonRequestBehavior.AllowGet);
        }

        //[AllowAnonymous]
        //public JsonResult GetUploadedDocumentsForReturnForm([DataSourceRequest]DataSourceRequest request, int? formId)
        //{
        //    var allDocs = _defaultRepository.GetUploadedDocumentsForReturnForm(request, formId);
        //    return Json(allDocs, JsonRequestBehavior.AllowGet);
        //}

        

        [AllowAnonymous]
        public JsonResult ValidatePanNumber(SchemeFormViewModel model)
        {
            var form = _schemeRepository.ValidateOnlineSchemeFormByType(model);
            return Json(form, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public JsonResult ValidateSchemeApplicationForm(SchemeFormViewModel model)
        {
            var form = _schemeRepository.ValidateOnlineSchemeFormByType(model);
            return Json(form, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult RemoveDocumentFromApplicationForm(string formNo, string filename)
        {
            int flag = _schemeRepository.RemoveDocumentFromApplicationForm(formNo, filename);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SendOTPforPayment(string formId)
        {
            //List<int> otpList = _defaultRepository.SendOTPforPayment(formId);
            var data = _schemeRepository.GetOnlineSchemeApplicationFormById(Convert.ToInt32(formId));
            Session["OTPmobile"] = data.MobileNumber;
            Session["OTPemail"] = data.MobileNumber;
            //Session["OTPmobile"] = 123;
            //Session["OTPemail"] = 123;
            //List<int> otpList = new List<int>(); otpList.Add(123); otpList.Add(123);
            //return Json(otpList, JsonRequestBehavior.AllowGet);
            return Json(data.MobileNumber, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ValidateOTP(string otpMobile)
        {
            int flag = 0;
            if ((int)Session["OTPmobile"] == Convert.ToInt32(otpMobile)) flag = OSReturnTypeId.Success;
            //if ((string)Session["OTPmobile"] == (otpMobile)) { flag = ReturnType.Success; }
            else { flag = OSReturnTypeId.Failure; }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SendOTP(string mobileNo, string email)
        {
            int flag = OnlineSchemeHelper.GenerateOTP();
            Session["OTPmobile"] = flag;
            //int flag = 123;
            //Session["OTPmobile"] = flag;
            string emailMessage = string.Format(OSMessages.OnlineApplicationOTP, flag);
            string mobileMessage = string.Format(OSMessages.OnlineApplicationOTP, flag);
            if (mobileNo != null && mobileNo != "") OnlineSchemeHelper.SendSMS(mobileNo, mobileMessage);
            if (email != null && email != "") OnlineSchemeHelper.SendEmail(email, "OnlineForm", emailMessage);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SendSchemeFormSubmissionOTP(string mobileNo, string email)
        {
            int flag = OnlineSchemeHelper.GenerateOTP();
            Session["OTPmobile"] = flag;
            string emailMessage = string.Format(OSMessages.OnlineApplicationOTP, flag);
            string mobileMessage = string.Format(OSMessages.OnlineApplicationOTP, flag);
            if (mobileNo != null && mobileNo != "") OnlineSchemeHelper.SendSMS(mobileNo, mobileMessage);
            if (email != null && email != "") OnlineSchemeHelper.SendEmail(email, "OnlineForm", emailMessage);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public JsonResult GetDirectorTypeList(OSDropdownViewModel model)
        {
            var typeList = _defaultRepository.GetDirectorTypeList(model);
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDirectorsDetailAsDataSource([DataSourceRequest] DataSourceRequest request, SchemeFormViewModel model)
        {
            if (model.ApplicationFormId != null)
            {
                DataSourceResult directors = _schemeRepository.GetProposedFirmDirectorsDetailAsDataSource(request, model);
                return Json(directors, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SaveDirectorsDetail(SchemeProposedFirmViewModel model)
        {
            if (model.ApplicationFormId != null)
            {
                var dir = _schemeRepository.SaveProposedFirmDirectorsDetail(model);
                return Json(dir, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendMessageToApplicant(SchemeFormViewModel model)
        {
            var flag = _defaultRepository.SendMessageToApplicant(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region online scheme 2018 sector 157


        #region Scheme data and other methods

        [AllowAnonymous]
        public JsonResult GetFormTypeList(OSDropdownViewModel model)
        {
            var list = _defaultRepository.GetFormTypeList(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetFormSubTypeList(OSDropdownViewModel model)
        {
            var list = _defaultRepository.GetFormSubTypeList(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetApplicantTypeList(OSDropdownViewModel model)
        {
            var list = _defaultRepository.GetApplicantTypeList(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        

        #region Scheme Login ,Logout,Change password and forgot password

        [AllowAnonymous]
        public ActionResult SchemeLogin(OSLoginViewModel schemeLogin)
        {
            if (schemeLogin.TxtSchemeFormNo != null && schemeLogin.TxtPassword != null)
            {
                if (!string.IsNullOrEmpty(schemeLogin.TxtSchemeFormNo))
                {
                    int flag = 0;
                    SchemeFormViewModel onlineForm = new SchemeFormViewModel();
                    onlineForm.ApplicationFormId = Convert.ToInt32(schemeLogin.TxtSchemeFormNo);
                    onlineForm.UserPassword = schemeLogin.TxtPassword;
                    onlineForm.AppType = schemeLogin.TxtRequestType;
                    //flag = _defaultRepository.ValidateApplicationDetails(onlineForm);
                    var frm = _schemeRepository.ValidateOnlineSchemeFormByType(onlineForm);
                    if (flag == OSReturnTypeId.Exist)
                    {
                        Session["SchemeUserLoginDetails"] = null;
                        
                        var exForm = _schemeRepository.GetOnlineSchemeApplicationFormById(onlineForm.ApplicationFormId);
                        
                        var _schemeUserDetail = new NA.PMS.OnlineScheme.SchemeFormViewModel();
                        _schemeUserDetail.ApplicationFormId = exForm.ApplicationFormId;
                        _schemeUserDetail.SchemeId = exForm.SchemeId;
                        _schemeUserDetail.SchemeName = exForm.SchemeName;
                        _schemeUserDetail.DepartmentId = exForm.DepartmentId;
                        _schemeUserDetail.Department = exForm.Department;
                        _schemeUserDetail.Applicant = exForm.Applicant;
                        _schemeUserDetail.SigningAuthority = exForm.SigningAuthority;
                        _schemeUserDetail.ApplicantType = exForm.ApplicantType;
                        _schemeUserDetail.CompanyTypeId = exForm.CompanyTypeId;
                        _schemeUserDetail.CompanyType = exForm.CompanyType;
                        _schemeUserDetail.FirstName = exForm.FirstName;
                        _schemeUserDetail.MiddleName = exForm.MiddleName;
                        _schemeUserDetail.LastName = exForm.LastName;
                        _schemeUserDetail.Gender = exForm.Gender;
                        _schemeUserDetail.MaritalStatus = exForm.MaritalStatus;
                        _schemeUserDetail.DOB = exForm.DOB;
                        _schemeUserDetail.MobileNumber = exForm.MobileNumber;
                        _schemeUserDetail.Email = exForm.Email;
                        _schemeUserDetail.SchemeType = schemeLogin.TxtSchemeType;
                        _schemeUserDetail.AppType = onlineForm.AppType;

                        //Session["SchemeUserLoginDetails"] = _schemeUserDetail;

                        string _openScheme = ConfigurationManager.AppSettings["OpenScheme"];

                        if (exForm.DepartmentId == 4)
                        {
                            if (schemeLogin.TxtSchemeType == SchemeConstant.OpenEnded)
                            {
                                Session["SchemeUserLoginDetails"] = _schemeUserDetail;
                                return RedirectToAction("OpenSchemeForm", "Application", new { area = "Online" });
                            }
                            else
                            {
                                TempData["ErrorLoginMessage"] = "Please Login through Open Ended scheme.";
                                return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                            }
                        }
                        else if (exForm.DepartmentId == 5)
                        {
                            Session["SchemeUserLoginDetails"] = _schemeUserDetail;
                            return RedirectToAction("RentingForm", "Housing", new { area = "Online" });
                        }
                        else
                        {
                            TempData["ErrorLoginMessage"] = "Scheme From Other Department.";
                            return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                        }
                    }
                    else if (flag == OSReturnTypeId.UserNameNotExist)
                    {
                        TempData["ErrorLoginMessage"] = "UserName/FormID is not correct."; return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                    }
                    else if (flag == OSReturnTypeId.PasswordNotExist)
                    {
                        TempData["ErrorLoginMessage"] = "Password is not correct."; return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                    }
                    else if (flag == OSReturnTypeId.Failure)
                    {
                        TempData["ErrorLoginMessage"] = "Form not validated."; return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                    }
                }
            }
            return RedirectToAction("SchemeInformation", "Institutional", new { area = "Online" });
        }

        [AllowAnonymous]
        public ActionResult SchemeLogOut()
        {
            ClearSessionVariables();
            return RedirectToAction("SchemeInformation", "Institutional", new { area = "Online" });
        }

        private void ClearSessionVariables()
        {
            Session.Clear();
            Session["TempCompanyDirectors"] = null;
            Session["WBasicDetailsNIC"] = null;
            Session["SchemeType"] = null;
            Session["SchemeUserLoginDetails"] = null;
        }

        [AllowAnonymous]
        public JsonResult ValidateSchemeForm(SchemeFormViewModel model)
        {
            model = _schemeRepository.ValidateOnlineSchemeFormByType(model);
            if (model.ReturnTypeId == OSReturnTypeId.Success)
            {
                Session["OnlineSchemeLoginDetail"] = model;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult LogoutSchemeform(SchemeFormViewModel model)
        {
            var _SchemeUser = (SchemeFormViewModel)Session["SchemeUserLoginDetails"];
            model.DepartmentId = _SchemeUser.DepartmentId;
            model.ReturnTypeId = OSReturnTypeId.Success;
            Session["SchemeUserLoginDetails"] = null;
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public ActionResult ResetPasswordForOnlineSchemeForm(OSLoginViewModel pmodel)
        {
            if (Session["SchemeUserLoginDetails"] != null)
            {
                var _SchemeUser = (SchemeFormViewModel)Session["SchemeUserLoginDetails"];
                pmodel = _defaultRepository.ChangePasswordForOnlineSchemeForm(pmodel);
                pmodel.DepartmentId = _SchemeUser.DepartmentId;
            }
            return Json(pmodel, JsonRequestBehavior.AllowGet);
        }

       

        #endregion



        #endregion



        [AllowAnonymous]
        public JsonResult GetPaymentStatusList(OSDropdownViewModel model)
        {
            var list = _defaultRepository.GetPaymentStatusList(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult AddToAllotment(string FormId)
        {
            int _FormId = 0;
            int FId = 0;
            if (int.TryParse(Convert.ToString(FormId), out _FormId))
                FId = _FormId;
            var form = new SchemeFormViewModel { ApplicationFormId = FId};
            var data = _schemeRepository.MigrateOnlineSchemeFormDataForAllotment(form);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetApplicationFormIdListAsDataSource([DataSourceRequest] DataSourceRequest request, SchemeFormViewModel model)
        {
            var departments = _schemeRepository.GetApplicationFormIdListAsDataSource(request, model);
            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult AllotPropertyAfterDraw(SchemeFormViewModel model)
        {
            var flag = _schemeRepository.MigrateOnlineSchemeFormDataForAllotment(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ValidatePropertyForAllotment(SchemeFormViewModel model)
        {
            var flag = _schemeRepository.ValidatePropertyForAllotment(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSchemeFormProcess(SchemeFormViewModel model)
        {
            var data = _schemeRepository.SaveSchemeFormProcess(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetOnlineApplicationProcessAsDataSource([DataSourceRequest] DataSourceRequest request, SchemeFormViewModel model)
        {
            var applications = _schemeRepository.GetOnlineApplicationProcessAsDataSource(request,model);
            return Json(applications, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveOnlineApplicationProcessStatus(SchemeFormViewModel model)
        {
            var data = _schemeRepository.SaveOnlineApplicationProcessStatus(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetPropertyDetailAsDataSourceByPropertyId([DataSourceRequest] DataSourceRequest request, int? rid)
        //{
        //    var data = _defaultRepository.GetPropertyDetailAsDataSourceByPropertyId(request, rid);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetApplicationDetailAsDataSourceByApplicationId([DataSourceRequest] DataSourceRequest request, int? applicationId)
        //{
        //    var data = _defaultRepository.GetApplicationDetailAsDataSourceByApplicationId(request, applicationId);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult PropertyAllotmentForOnlineApplicationForm(OnlineFormViewModel model)
        //{
        //    int flag = _defaultRepository.PropertyAllotmentForOnlineApplicationForm(model);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult ValidatePropertyAndApplicationForm(OnlineFormViewModel model)
        //{
        //    int flag = _defaultRepository.ValidatePropertyAndApplicationForm(model);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetAllottedOnlineFormPropertyList([DataSourceRequest] DataSourceRequest request, OnlineFormViewModel model)
        //{
        //    var data = _defaultRepository.GetAllottedOnlineFormPropertyList(request, model);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult SaveAndGetOnlineSchemeFormCallan(SchemeFormViewModel model)
        {
            //var data = _schemeRepository.SaveOnlineApplicationProcessStatus(model);
            var data = _schemeRepository.SaveAndGetOnlineSchemeFormCallan(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlineSchemeformDetailById(SchemeFormViewModel model)
        {
            var data = _schemeRepository.GetOnlineSchemeApplicationFormById(model.ApplicationFormId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSchemeAreaRange(OSSchemeAreaViewModel model)
        {
            var data = _defaultRepository.SaveSchemeAreaRange(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSchemeAreaRangeListAsDataSource([DataSourceRequest] DataSourceRequest request, OSSchemeAreaViewModel model)
        {
            var data = _defaultRepository.GetSchemeAreaRangeListAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult abc()
        {
            var _tokenUrl = "https://bankapiuat.mynoida.in/token";
            var challanUrl = "https://bankapi.mynoida.in/Api/Receipt/CreateChallan";
            //Hosted web API REST Service base url  
            string Baseurl = _tokenUrl; 
            var EmpInfo = new SchemeFormChallanApiModel();

            using (var client = new HttpClient())
            {
                var parameters = new Dictionary<string, string> { { "username", "Admin" }, { "password", "Admin" }, { "grant_type", "password" } };
               
                var encodedContent = new FormUrlEncodedContent(parameters);
                //parameters.
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

               

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage Res = await client.GetAsync("api/Employee/GetAllEmployees");
                Task<HttpResponseMessage> Res = client.GetAsync(Baseurl);

                var response = client.PostAsync(Baseurl, encodedContent);

                //var requestUri = @"ManageApplications/GetUserDetailsByApplication?userName=" + userName + "&applicationId=" + applicationId + "&applicationName=" + applicationName;
                //return await _client.GetRequest<CurrentUserDetail>(baseUrl, requestUri);

                var parameters2 = new { username = "Admin", password = "Admin", grant_type = "password" };
                //var dd = JsonConvert.SerializeObject(parameters2);
                
                //var result1 = client.SendAsync(Baseurl, dd); //client.PostAsync(Baseurl, dd);
                //Checking the response is successful or not which is sent using HttpClient 
                var result = Res.Result;
                if (result.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    //var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                    var EmpResponse = result.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    //EmpInfo = JsonConvert.DeserializeObject<List<Employee>>(EmpResponse);

                }
                return Json(EmpInfo, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
