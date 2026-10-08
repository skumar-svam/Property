using Kendo.Mvc.UI;
//using NA.PMS.Common;
using NA.PMS.NICService.NiveshMitraApiServices;
using NA.PMS.NICService.Resource;
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
    
    public class SWPFormController : SWPSuperController
    {
        public string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
        private ISWPFormService _swpFromService;
        private ISWPGeneralService _generalService;
        private TemplateParserService _templateParserService;
        private SWPApiServiceProvider _apiService;  
        private PIMSAccount _account;
        public SWPFormController()
        {
            _swpFromService = new SWPFormService();
            _generalService = new SWPGeneralService();
            _templateParserService = new TemplateParserService();
            _apiService = new SWPApiServiceProvider();
            _account = new PIMSAccount();
        }

        #region Noida Covid-19 e-pass
        public ActionResult DemoTest()
        {
            return Content("Shatrughna u r winner!");
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult SearchEpass()
        {
            return View();
        }

        public ActionResult ViewEpass(string passno)
        {
            return View();
        }

        public ActionResult ManageOnlineEpass()
        {
            return View();
        }
        public JsonResult GetAllEpassList([DataSourceRequest]DataSourceRequest request, SWPEpassViewModel model)
        {
            var list = _swpFromService.GetAllEpassList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult SaveEpassForm(SWPEpassViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase idfile, HttpPostedFileBase rcfile, HttpPostedFileBase dlfile)
        {
            model = _swpFromService.SaveEpassFormDetail(model, userImage, idfile, rcfile, dlfile);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult GetEpassFormDetailById(SWPEpassViewModel model)
        {
            model = _swpFromService.GetEpassFormDetailById(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ValidateEpass(SWPEpassViewModel model)
        {
            int flag = _swpFromService.ValidateEpass(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public JsonResult SendOTP(string mobileNo, string email)
        {
            int flag = SWPApplication.GenerateOTP();
            Session["OTPmobile"] = flag;
            //int flag = 123;
            //Session["OTPmobile"] = flag;
            string message = string.Format(SWPMessage.OnlineApplicationOTP, flag);
            //string mobileMessage = string.Format(NAMessages.OnlineApplicationOTP, flag);
            if (mobileNo != null && mobileNo != "") SWPApplication.SendSMS(mobileNo, message);
            if (email != null && email != "") SWPApplication.SendEmail(email, "Online E-Pass", message);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EpassDetails(int? Id)
        {
            SWPEpassViewModel model = new SWPEpassViewModel();
            if (Id != null)
            {
                model = _swpFromService.EpassDetails(Id);
            }
            return View(model);
        }
        #endregion

        [AllowAnonymous]
        public ActionResult Dashboard()
        {
            return View();
        }

        #region online scheme form

        [AllowAnonymous]
        public ActionResult SchemeLogin(SWPLoginViewModel loginModel)
        {
            if (loginModel.UserName != null && loginModel.Password != null)
            {
                if (!string.IsNullOrEmpty(loginModel.UserName))
                {
                    int flag = 0;
                    SWPFormViewModel onlineForm = new SWPFormViewModel();
                    onlineForm.ApplicationFormId = Convert.ToInt32(loginModel.UserName);
                    onlineForm.UserPassword = loginModel.Password;
                    onlineForm.AppType = loginModel.FormType; //AppType
                   
                    flag = _swpFromService.ValidateApplicationDetails(onlineForm);
                    if (flag == SWPReturnTypeId.Exist)
                    {
                        Session["SchemeUserLoginDetails"] = null;
                        var _exform = _swpFromService.GetOnlineSchemeFormDataById(onlineForm);

                        //Session["SchemeUserLoginDetails"] = _exform;

                        string _openScheme = ConfigurationManager.AppSettings["OpenScheme"];

                        if (_exform.DepartmentId == 4)
                        {
                            if (loginModel.SchemeType == SWPSchemeType.OpenEnded)
                            {
                                Session["SchemeUserLoginDetails"] = _exform;
                                return RedirectToAction("IndustrialOpenEndForm", "NoidaAuthority", new { area = "Online" });
                            }
                            else
                            {
                                TempData["ErrorLoginMessage"] = "Please Login through Open Ended scheme.";
                                return RedirectToAction("OnlineScheme", "NoidaAuthority", new { area = "Online" });
                            }
                        }
                        else
                        {
                            TempData["ErrorLoginMessage"] = "Scheme From Other Department.";
                            return RedirectToAction("OnlineScheme", "NoidaAuthority", new { area = "Online" });
                        }
                    }
                    else if (flag == SWPReturnTypeId.UserNameNotExist)
                    {
                        TempData["ErrorLoginMessage"] = "UserName/FormID is not correct."; return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                    }
                    else if (flag == SWPReturnTypeId.PasswordNotExist)
                    {
                        TempData["ErrorLoginMessage"] = "Password is not correct."; return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                    }
                    else if (flag == SWPReturnTypeId.Failure)
                    {
                        TempData["ErrorLoginMessage"] = "Form not validated."; return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                    }
                }
            }
            return RedirectToAction("OnlineScheme", "NoidaAuthority", new { area = "Online" });
        }

        public JsonResult ForgotPasswordAction(SWPLoginViewModel model)
        {
            model = _swpFromService.ForgotPasswordAction(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult SingleWindowPortal()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult SchemeChallanPayment(string EncryptedFormId)// public ActionResult SchemeChallanPayment(SWPFormViewModel model)
        {
            if (!string.IsNullOrEmpty(EncryptedFormId))
            {
                int ID = Convert.ToInt32(SWPEncryption.Decode(EncryptedFormId));
                if (ID > 0)
                {
                    var _ChallanPayment = _swpFromService.GetSchemeFormPaymentTransaction(new SWPPaymentViewModel { ApplicationFormId = ID});//.GetOfflinePayment_Trans(ID);
                    SWPFormViewModel _SchemeForm = new SWPFormViewModel();
                    if (string.IsNullOrEmpty(_ChallanPayment.TransactionKey))
                    {
                        if (_ChallanPayment != null)
                        {
                            _SchemeForm = _ChallanPayment.FormModel;
                            _SchemeForm.PaymentModel = _ChallanPayment;
                            _SchemeForm.PaymentModel.TransactionId = string.IsNullOrEmpty(_ChallanPayment.TransactionId) ? "" : _ChallanPayment.TransactionId;
                            _SchemeForm.PaymentModel.EntryDate = null;
                            _SchemeForm.ApplicationFormId = ID;
                            _SchemeForm.AppType = "NIC"; //modelAppType;
                            return View(_SchemeForm);
                            //return View("IndustrialPreviewForm", new { id = _SchemeForm.EncryptedFormId});
                        }
                    }
                }
            }
            //return RedirectToAction("SchemeInformation", "NoidaAuthority");
            return View("IndustrialPreviewForm", new { id = EncryptedFormId });
        }

        [AllowAnonymous]
        public ActionResult SaveSchemeFormPaidChallan(SWPFormViewModel model, HttpPostedFileBase files)
        {
            var data = _swpFromService.SaveSchemeFormPaidChallanDetail(model, files);//.UpdateOfflinePayment(objOnlineFormViewModel, files);
            //if (data == "2")
            //{
            //    TempData["OfflinePayment_RTGS"] = "Transaction ID not exist or wrong";
            //    return RedirectToAction("OfflinePayment", "Payment", new { ApplicationFormId = SWPEncryption.Encode(objOnlineFormViewModel.PaymentModel.ApplicationFormId.ToString()), area = "Online" });
            //}

            if (data.ReturnTypeId == SWPReturnTypeId.Updated)//"Updated Successfully"
            {
                model.ApplicationFormId = model.PaymentModel.ApplicationFormId;
                //var form = _swpFromService.GetOpenEndedSchemeFormDataById(nicdetail);
                var form = _swpFromService.GetOpenEndedSchemeFormDetailsById(new SWPFormViewModel { ApplicationFormId = model.ApplicationFormId });
                //form.SWPApiStatusModel.PendancyLevel = SWPStatus.Wording.FORM_SUBMITTED;
                //form.SWPApiStatusModel.FeeStatus = SWPStatus.FEE_PAID;
                //var swpapi = new SWPApiServiceProvider();
                //swpapi.GetSWPServiceStatus(form.SWPApiStatusModel);
            }
            //else
            //{
            //    TempData["ServiceMessage"] = "Service Failure";
            //}
            //TempData["OfflinePayment_RTGS"] = data;
            //return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult OnlineScheme(SWPPostViewModel nicdata)
        {
            SWPFormViewModel naonlineform = new SWPFormViewModel();

            if (nicdata.TxtControlID != null && nicdata.TxtUnitID != null && nicdata.TxtServiceID != null)
            {
                var nicdetail = _apiService.GetApiPostedBasicDetails(nicdata);
                //var nicdetail = GetNICPostedBasicDetails(nicdata);

                if (!string.IsNullOrEmpty(nicdata.TxtApplicationID))
                {
                    nicdetail.ApplicationFormId = Convert.ToInt32(nicdata.TxtApplicationID);
                    //naonlineform = _onlineService.GetOpenEndedSchemeFormDataById(nicdetail);
                    naonlineform = _swpFromService.GetOpenEndedSchemeFormDataById(nicdetail);
                    //naonlineform = nicdetail;
                    if (naonlineform.SchemeType == SWPSchemeType.IndustrialScheme)
                    {
                        return RedirectToAction("IndustrialForm", "NoidaAuthority", new { area = "NIC", @id = naonlineform.Id, @controlid = naonlineform.NICControlId, @unitid = naonlineform.NICUnitId, @serviceid = naonlineform.NICServiceId, @requestid=naonlineform.NICRequestId });
                    }
                    else
                    {
                        return RedirectToAction("IndustrialOpenEndForm", "NoidaAuthority", new { area = "NIC", @id = naonlineform.Id, @controlid = naonlineform.NICControlId, @unitid = naonlineform.NICUnitId, @serviceid = naonlineform.NICServiceId, @requestid = naonlineform.NICRequestId });
                    }
                }
                else
                {
                    //TempData["ServiceMessage"] = "New Application Form."; //"Service Failure.";
                    naonlineform = nicdetail;
                    naonlineform.SchemeType = nicdata.SchemeType;
                    naonlineform = _swpFromService.GetSchemeBasicInfoData(naonlineform);
                }
            }
            else
            {
                naonlineform.NICControlId = nicdata.TxtControlID;
                naonlineform.NICUnitId = nicdata.TxtUnitID;
                naonlineform.NICServiceId = nicdata.TxtServiceID;
                naonlineform.NICApplicationId = nicdata.TxtApplicationID;
                naonlineform.NICProcessIndustryId = nicdata.TxtProcessIndustryID;
                naonlineform.NICRequestId = nicdata.TxtRequestID;
                naonlineform.SchemeType = nicdata.SchemeType;
                naonlineform = _swpFromService.GetSchemeBasicInfoData(naonlineform);
            }
            //Counter();//Generate user session and update the counter by 1.
            return View(naonlineform);
        }

        [AllowAnonymous]
        public ActionResult SchemeInformation(SWPPostViewModel nicdata)
        {
            SWPFormViewModel naonlineform = new SWPFormViewModel();

            if (nicdata.TxtControlID != null && nicdata.TxtUnitID != null && nicdata.TxtServiceID != null)
            {
                var nicdetail = _apiService.GetApiPostedBasicDetails(nicdata);
                //var nicdetail = GetNICPostedBasicDetails(nicdata);

                if (!string.IsNullOrEmpty(nicdata.TxtApplicationID))
                {
                    nicdetail.ApplicationFormId = Convert.ToInt32(nicdata.TxtApplicationID);
                    //naonlineform = _onlineService.GetOpenEndedSchemeFormDataById(nicdetail);
                    naonlineform = _swpFromService.GetOpenEndedSchemeFormDataById(nicdetail);
                    //naonlineform = nicdetail;
                    if (naonlineform.SchemeType == SWPSchemeType.IndustrialScheme)
                    {
                        return RedirectToAction("IndustrialForm", "NoidaAuthority", new { area = "NIC", @id = naonlineform.Id, @controlid = naonlineform.NICControlId, @unitid = naonlineform.NICUnitId, @serviceid = naonlineform.NICServiceId });
                    }
                    else
                    {
                        return RedirectToAction("IndustrialOpenEndForm", "NoidaAuthority", new { area = "NIC", @id = naonlineform.Id, @controlid = naonlineform.NICControlId, @unitid = naonlineform.NICUnitId, @serviceid = naonlineform.NICServiceId });
                    }
                }
                else
                {
                    //TempData["ServiceMessage"] = "New Application Form."; //"Service Failure.";
                    naonlineform = nicdetail;
                    naonlineform.SchemeType = nicdata.SchemeType;
                    naonlineform = _swpFromService.GetSchemeBasicInfoData(naonlineform);
                }
            }
            else
            {
                naonlineform.NICControlId = nicdata.TxtControlID;
                naonlineform.NICUnitId = nicdata.TxtUnitID;
                naonlineform.NICServiceId = nicdata.TxtServiceID;
                naonlineform.NICApplicationId = nicdata.TxtApplicationID;
                naonlineform.NICProcessIndustryId = nicdata.TxtProcessIndustryID;
                naonlineform.SchemeType = nicdata.SchemeType;
                naonlineform = _swpFromService.GetSchemeBasicInfoData(naonlineform);
            }
            //Counter();//Generate user session and update the counter by 1.
            return View(naonlineform);
        }

        [AllowAnonymous]
        public ActionResult IndustrialSchemeInformation()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult RegisterSchemeForm()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult SchemeAllotment(string id)
        {
            if (id != null && Convert.ToInt32(id) > 0)
            {
                int? formId = Convert.ToInt32(id);
                var model = _swpFromService.GetOnlineSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = formId });
                return View(model);
            }
            else
            {
                return View();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult SaveSchemeFormDetail(SWPFormViewModel form, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            return Json(form,JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult IndustrialOpenEndForm(int? id, string controlid, string unitid, string serviceid, string requestid)
        {
            SWPFormViewModel form = _swpFromService.GetSchemeBasicInfoData(new SWPFormViewModel { SchemeType = SWPSchemeType.OpenEnded }); //GetSchemeBasicInfoData(OnlineSchemeType.OpenEnded);
            if (!string.IsNullOrEmpty(controlid) && !string.IsNullOrEmpty(unitid) && !string.IsNullOrEmpty(serviceid))
            {
                SWPPostViewModel apidata = new SWPPostViewModel { TxtControlID = controlid, TxtServiceID = serviceid, TxtUnitID = unitid, TxtRequestID = requestid };
                if (id != null && id > 0)
                {
                    apidata.TxtApplicationID = id.ToString();
                    apidata.TxtProcessIndustryID = id.ToString();
                    var nicdetail = _apiService.GetApiPostedBasicDetails(apidata);
                    nicdetail.ApplicationFormId = id;
                    //form = _onlineService.GetOpenEndedSchemeFormDataById(nicdetail);
                    form = _swpFromService.GetOpenEndedSchemeFormDataById(nicdetail);
                }
                else
                {
                    //data from nic web service
                    var nicdetail = _apiService.GetApiPostedBasicDetails(apidata);
                    form.NICControlId = nicdetail.NICControlId;
                    form.NICUnitId = nicdetail.NICUnitId;
                    form.NICServiceId = nicdetail.NICServiceId;
                    form.NICProcessIndustryId = nicdetail.NICProcessIndustryId;
                    form.NICApplicationId = nicdetail.NICApplicationId;
                    form.NICRequestId = nicdetail.NICRequestId;
                    form.NICXmlInputTable = nicdetail.NICXmlInputTable;
                    form.SWPApiBasicModel = nicdetail.SWPApiBasicModel;
                    form.FlagId = nicdetail.FlagId;
                    form.IsFromNIC = nicdetail.IsFromNIC;
                    form.AppType = SWPConstant.NIC;
                    form.ApplicantType = "Company";
                }
            }
            else
            {
                return RedirectToAction("IndustrialSchemeInformation");
            }

            return View(form);
        }

        [AllowAnonymous]
        public ActionResult IndustrialForm(int? id, string controlid, string unitid, string serviceid, string requestid)
        {
            SWPFormViewModel form = _swpFromService.GetSchemeBasicInfoData(new SWPFormViewModel { SchemeType = SWPSchemeType.IndustrialScheme }); //GetSchemeBasicInfoData(OnlineSchemeType.OpenEnded);
            if (!string.IsNullOrEmpty(controlid) && !string.IsNullOrEmpty(controlid) && !string.IsNullOrEmpty(controlid))
            {
                SWPPostViewModel apidata = new SWPPostViewModel { TxtControlID = controlid, TxtServiceID = serviceid, TxtUnitID = unitid, TxtRequestID = requestid };
                if (id != null && id > 0)
                {
                    apidata.TxtApplicationID = id.ToString();
                    apidata.TxtProcessIndustryID = id.ToString();
                    var nicdetail = _apiService.GetApiPostedBasicDetails(apidata);
                    nicdetail.ApplicationFormId = id;
                    //form = _onlineService.GetOpenEndedSchemeFormDataById(nicdetail);
                    form = _swpFromService.GetOpenEndedSchemeFormDataById(nicdetail);
                }
                else
                {
                    //data from nic web service
                    var nicdetail = _apiService.GetApiPostedBasicDetails(apidata);
                    form.NICControlId = nicdetail.NICControlId;
                    form.NICUnitId = nicdetail.NICUnitId;
                    form.NICServiceId = nicdetail.NICServiceId;
                    form.NICProcessIndustryId = nicdetail.NICProcessIndustryId;
                    form.NICApplicationId = nicdetail.NICApplicationId;
                    form.NICRequestId = nicdetail.NICRequestId;
                    form.NICXmlInputTable = nicdetail.NICXmlInputTable;
                    form.SWPApiBasicModel = nicdetail.SWPApiBasicModel;
                    form.FlagId = nicdetail.FlagId;
                    form.IsFromNIC = nicdetail.IsFromNIC;
                    form.AppType = SWPConstant.NIC;
                    form.ApplicantType = "Company";
                }
            }
            else
            {
                return RedirectToAction("IndustrialSchemeInformation");
            }

            return View(form);
        }

        [AllowAnonymous]
        public ActionResult IndustrialDocument(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int formId = Convert.ToInt32(SWPEncryption.Decode(id));
                if (formId > 0)
                {
                    SWPFormViewModel model = new SWPFormViewModel();
                    //model = _onlineService.GetOnlineApplicationFormById(formId);
                    model = _swpFromService.GetOpenEndedSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = formId });
                    return View(model);
                }
                else
                {
                    return RedirectToAction("IndustrialSchemeInformation");
                }
            }
            else
            {
                return RedirectToAction("IndustrialSchemeInformation");
            }
        }

        [AllowAnonymous]
        public ActionResult IndustrialProposedProject(int? id, string controlid, string unitid, string serviceid)
        {
            //SWPFormViewModel form = GetSchemeBasicInfoData(OnlineSchemeType.OpenEnded);
            SWPFormViewModel form = _swpFromService.GetSchemeBasicInfoData(new SWPFormViewModel { ApplicationFormId = id });
            if (!string.IsNullOrEmpty(controlid) && !string.IsNullOrEmpty(controlid) && !string.IsNullOrEmpty(controlid))
            {
                SWPPostViewModel apidata = new SWPPostViewModel { TxtControlID = controlid, TxtServiceID = serviceid, TxtUnitID = unitid };
                if (id != null && id > 0)
                {
                    apidata.TxtApplicationID = id.ToString();
                    apidata.TxtProcessIndustryID = id.ToString();
                    var nicdetail = _apiService.GetApiPostedBasicDetails(apidata);
                    nicdetail.ApplicationFormId = id;
                    //form = _onlineService.GetOpenEndedSchemeFormDataById(nicdetail);
                    form = _swpFromService.GetOpenEndedSchemeFormDataById(nicdetail);
                }
                else
                {
                    if (form.SchemeType == SWPSchemeType.IndustrialScheme)
                    {
                        return RedirectToAction("IndustrialForm", "OnlineProperty", new { area = "NIC", @id = id, @controlid = controlid, @unitid = unitid, @serviceid = serviceid });
                    }
                    else
                    {
                        return RedirectToAction("IndustrialOpenEndForm", "OnlineProperty", new { area = "NIC", @id = id, @controlid = controlid, @unitid = unitid, @serviceid = serviceid });
                    }
                }
            }
            else
            {
                return RedirectToAction("IndustrialSchemeInformation");
            }

            return View(form);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult IndustrialPreviewForm(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int formId = Convert.ToInt32(SWPEncryption.Decode(id));
                if (formId > 0)
                {
                    SWPFormViewModel model = new SWPFormViewModel();
                    //model = _onlineService.GetOnlineApplicationFormById(formId);
                    model = _swpFromService.GetOpenEndedSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = formId });
                    if (!string.IsNullOrEmpty((string)Session["SchemeType"]))
                    {
                        model.SchemeType = (string)Session["SchemeType"];
                    }
                    //if (DateTime.Compare((DateTime)model.SchemeEndDate, DateTime.Now) >= 0)
                    //{
                    //    if (model.IsApplicationFeePaid == true)
                    //    {
                    //        model.BankModel = _generalService.GetBankListBySchemeId((int)model.SchemeId);
                    //        return View(model);
                    //    }
                    //    else { return RedirectToAction("ErrorPage"); }
                    //}
                    model.BankList = _generalService.GetBankListBySchemeId((int)model.SchemeId);
                    return View(model);
                }
                else
                {
                    return RedirectToAction("IndustrialSchemeInformation");
                }
            }
            else
            {
                return RedirectToAction("IndustrialSchemeInformation");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ScrutinyRequiredDocument(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int formId = Convert.ToInt32(SWPEncryption.Decode(id));
                if (formId > 0)
                {
                    var model = _swpFromService.GetOnlineSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = formId });
                    return View(model);
                }
                else
                {
                    return RedirectToAction("SchemeInformation");
                }
            }
            else
            {
                return RedirectToAction("SchemeInformation");
            }
        }


        public ActionResult ManageSchemeForm()
        {
            if (Session["CurrentUser"] != null)
            {
                //var loginUser = (CurrentUserDetail)Session["CurrentUser"];
                var loginUser = _account.GetLoginAccountDetails(Session["UserName"].ToString());
                SWPFormViewModel model = new SWPFormViewModel();
                model.UserId = loginUser.UserRefId;
                model.UserRoleType = loginUser.RoleMaster.RoleType;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        public ActionResult AccountableSchemeForm()
        {
            if (Session["CurrentUser"] != null)
            {
                //var loginUser = (CurrentUserDetail)Session["CurrentUser"];
                var loginUser = _account.GetLoginAccountDetails(Session["UserName"].ToString());
                SWPFormViewModel model = new SWPFormViewModel();
                model.UserId = loginUser.UserRefId;
                model.UserRoleType = loginUser.RoleMaster.RoleType;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        public ActionResult ManageScrutinization()
        {
            if (Session["CurrentUser"] != null)
            {
                //var loginUser = (CurrentUserDetail)Session["CurrentUser"];
                var loginUser = _account.GetLoginAccountDetails(Session["UserName"].ToString());
                SWPFormViewModel model = new SWPFormViewModel();
                model.UserId = loginUser.UserRefId;
                model.UserRoleType = loginUser.RoleMaster.RoleType;
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        public ActionResult SchemeFormDetail(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int ID = Convert.ToInt32(SWPEncryption.Decode(id));
                if (ID > 0)
                {
                    var model = _swpFromService.GetOnlineSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = ID });
                    return View(model);
                }
            }
            return RedirectToAction("ManageSchemeForm");
        }

        public ActionResult ScrutinizeSchemeForm(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int ID = Convert.ToInt32(SWPEncryption.Decode(id));
                if (ID > 0)
                {
                    var model = _swpFromService.GetOnlineSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = ID });
                    return View(model);
                }
            }
            return RedirectToAction("ManageSchemeForm");
        }

        public ActionResult ManageApplicationForm()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ManageChallan()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult SchemeFormChallan()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public JsonResult SaveOpenSchemeFormDetail(SWPFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            //model.SchemeType = SWPSchemeType.OpenEnded;
            Session["SchemeType"] = model.SchemeType;
            //var form = _onlineService.SaveOpenEndedSchemeFormDetail(model, userImage, signatureImage);
            var form = _swpFromService.SaveOpenEndedSchemeFormDetail(model, userImage, signatureImage);
            if (form.IsFromNIC == true && form.FormStatusId == SWPStatusId.Success)
            {
                //Post Request to NIC Service
                string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
                
                //SWPStatusViewModel nicReturnModel = new SWPStatusViewModel();
                //nicReturnModel.ControlId = model.SWPApiBasicModel.Table.Control_ID;
                //nicReturnModel.ApplicationId = model.ApplicationFormId.ToString(); //Convert.ToString(CommonHelper.Decode(flag));
                ////nicReturnModel.ProcessIndustryID = model.BasicDetailsGetModel.Table.ProcessIndustryID; //Convert.ToString(CommonHelper.Decode(flag));
                //nicReturnModel.ProcessIndustryId = model.SWPApiBasicModel.Table.ProcessIndustryID == null ? model.ApplicationFormId.ToString() : model.SWPApiBasicModel.Table.ProcessIndustryID;
                //nicReturnModel.UnitId = model.SWPApiBasicModel.Table.Unit_Id;
                //nicReturnModel.ServiceId = model.SWPApiBasicModel.Table.ServiceID == null ? model.NICServiceId : model.SWPApiBasicModel.Table.ServiceID;
                //nicReturnModel.StatusCode = SWPStatus.FEE_PENDING; //ServiceStatus.SAVE_AS_DRAFT;
                ////nicReturnModel.Fee_Status = ServiceStatus.FEE_PENDING;
                //nicReturnModel.FeeStatus = SWPStatus.Payment.UB;
                ////nicReturnModel.Remarks = ServiceStatus_Text.SAVE_AS_DRAFT;
                ////nicReturnModel.Fee_Amount = Convert.ToString(model.TotalAmount);
                //nicReturnModel.Remarks = SWPStatus.Wording.FEE_PENDING;
                ////nicReturnModel.Fee_Amount = Convert.ToString(model.FormFeeWithGST);
                //nicReturnModel.FeeAmount = "5900";
                //nicReturnModel.NICPassSalt = Passalt;
                //var stmodel = _apiService.GetSWPServiceStatus(nicReturnModel);

                form.SWPApiStatusModel.NICPassSalt = Passalt;
                var stmodel = _apiService.GetSWPServiceStatus(form.SWPApiStatusModel);

                string message = stmodel.Status;
                if (message == "FAILED") //"Failure"
                {
                    TempData["ServiceMessage"] = "Nivesh Mitra Service Failed";
                }
                else
                {
                    TempData["ServiceMessage"] = "Nivesh Mitra Service updated successfully.";
                }
            }
            else
            {
                TempData["ServiceMessage"] = "Application form not saved by Authority.";
            }
            return Json(form, JsonRequestBehavior.AllowGet);
            //var encodedId = CommonHelper.Encode(form.ApplicationFormId.ToString());
            //return Json(encodedId, JsonRequestBehavior.AllowGet);
        }

        private SWPFormViewModel GetSchemeBasicInfoData(string schemeType)
        {
            int schemeId = 0;
            int departmentId = 0;

            if (schemeType == SWPSchemeType.IndustrialPlots) { schemeId = ConfigurationManager.AppSettings["SchemeId"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["SchemeId"]) : 0; }
            else if (schemeType == SWPSchemeType.Transport) { schemeId = ConfigurationManager.AppSettings["TransportScheme"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["TransportScheme"]) : 0; }
            else if (schemeType == SWPSchemeType.OpenEnded) { schemeId = ConfigurationManager.AppSettings["OpenScheme"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["OpenScheme"]) : 0; }
            else if (schemeType == SWPSchemeType.IndustrialSchemeType) { schemeId = ConfigurationManager.AppSettings["IndustrialSchemeId"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["IndustrialSchemeId"]) : 0; }

            departmentId = ConfigurationManager.AppSettings["DepartmentId"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["DepartmentId"]) : 0;

            SWPFormViewModel schemeInfo = new SWPFormViewModel();
            if (schemeType == SWPSchemeType.IndustrialSchemeType)
                schemeInfo = _swpFromService.GetSchemeInformationForOnlineApplication(new SWPFormViewModel { SchemeId = schemeId, DepartmentId = departmentId });
            //schemeInfo = _onlineService.GetSchemeInformationForOnlineApplication(new SWPFormViewModel { SchemeId = schemeId, DepartmentId = departmentId });
            else
                schemeInfo = _swpFromService.GetInitialDataForScheme(new SWPFormViewModel { SchemeId = schemeId, DepartmentId = departmentId });
            //schemeInfo = _onlineService.GetInitialDataForScheme(new SWPFormViewModel { SchemeId = schemeId, DepartmentId = departmentId });
            return schemeInfo;
        }

        //return true if scheme end date is less than current date.
        [AllowAnonymous]
        public bool CheckSchemeCredentials()
        {
            bool flag = false;
            //var model = _onlineService.GetInitialDataForScheme();
            var model = _swpFromService.GetInitialDataForScheme();
            if (model.SchemeEndDate != null)
            {
                if (DateTime.Compare((DateTime)model.SchemeEndDate, DateTime.Now) < 0)
                {
                    return flag = false; // true;
                }
            }
            return flag;
        }

        [AllowAnonymous]
        public JsonResult GetFormTypeList()
        {
            var list = _generalService.GetFormTypeList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetFormSubTypeList(string formtype)
        {
            var list = _generalService.GetFormSubTypeList(formtype);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetApplicantTypeList()
        {
            var list = _generalService.GetApplicantTypeList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetCompanyTypeList(string typeName)
        {
            var floorArea = _generalService.GetCompanyTypeByCategory(typeName);
            return Json(floorArea, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetFloorAreaList(int schemeId, int departmentId)
        {
            //var floorArea = _onlineService.GetAreaRangeByDepartment(schemeId, departmentId);
            var floorArea = _swpFromService.GetAreaRangeByDepartment(schemeId, departmentId);
            return Json(floorArea, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetFloorAreaRangeList(SWPDropdownViewModel model)
        {
            var floorArea = _swpFromService.GetFloorAreaRangeList(model);
            return Json(floorArea, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetFloorAreaListAsDataSource([DataSourceRequest] DataSourceRequest request, SWPDropdownViewModel model)
        {
            var list = _swpFromService.GetFloorAreaListAsDataSource(request, model);
            //var list = _onlineService.GetFloorAreaListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetSectorList()
        {
            var sectors = _generalService.getSectorsList();
            return Json(sectors, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDirectorTypeList()
        {
            var typeList = _generalService.GetDirectorTypeList();
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetGenderList()
        {
            var floorArea = _generalService.GetGenderList();
            return Json(floorArea, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetMaritalStatusList()
        {
            var maritalstatus = _generalService.GetMaritalStatusList();
            return Json(maritalstatus, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetCategoryList()
        {
            var categories = _generalService.GetCategoryList();
            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetOccupationList()
        {
            var occupations = _generalService.GetOccupationList();
            return Json(occupations, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SaveProjectAndRefundDetailForOpenEndScheme(SWPFormViewModel model)
        {
            //model = _onlineService.SaveProjectAndRefundDetailForOpenEndScheme(model);
            model = _swpFromService.SaveProposedProjectAndRefundDetail(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetBankListforOnline()
        {
            var banks = _generalService.GetBankList();
            return Json(banks, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetBankListBySchemeId(int schemeId)
        {
            var banks = _generalService.GetBankListBySchemeId(schemeId);
            return Json(banks, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetBankAccountDetails(SWPFormViewModel model)
        {
            //var AccountNo = _generalService.GetAccountBranchBySchemeIdBankId(schemeId, bankId);
            var AccountNo = _swpFromService.GetBankAccountDetails(model);
            return Json(AccountNo, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SaveDirectorDetailsForOpenScheme(SWPDirectorViewModel model)
        {
            model = _swpFromService.SaveDirectorDetailsForOpenScheme(model);
            //model = _onlineService.SaveDirectorDetailToDataBaseForOpenScheme(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDirectorDetailsAsDataSource(DataSourceRequest request, int? formId)
        {
            var flag = _swpFromService.GetDirectorDetailsAsDataSourceByFormId(request, formId);
            //var flag = _onlineService.GetDirectorDetailsFromDatabaseForOpenScheme(request, id);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SaveUploadedDocument(SWPFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            model = _swpFromService.SaveUploadedDocument(model, files, userImage, signatureImage);
            //int flag = _onlineService.UploadDocumentByFormId(model, files, userImage, signatureImage);
            if (model.ReturnTypeId != SWPReturnTypeId.Failure)
            {
                TempData["Document"] = "success";
                return RedirectToAction("IndustrialPreviewForm", "NoidaAuthority", new { area = "NIC", id = model.EncryptedFormId });
            }
            else
            {
                TempData["Document"] = "failure";
                return RedirectToAction("IndustrialDocument", "NoidaAuthority", new { area = "NIC", id = model.EncryptedFormId });
            }
        }

        [AllowAnonymous]
        public JsonResult RemoveDocumentFromApplicationForm(string formNo, string filename)
        {
            int flag = _swpFromService.RemoveDocumentFromApplicationForm(formNo, filename);
            //int flag = _onlineService.RemoveDocumentFromApplicationForm(formNo, filename);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetChecklistDocumentForOnlineForm([DataSourceRequest]DataSourceRequest request, SWPFormViewModel model)
        {
            var allDocs = _swpFromService.GetChecklistDocumentForOnlineForm(request, model);
            return Json(allDocs, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetUploadedDocumentsForOnlineForm([DataSourceRequest]DataSourceRequest request, int? formId)
        {
            var allDocs = _swpFromService.GetUploadedDocumentsByFormId(request, formId);
            //var allDocs = _onlineService.GetUploadedDocumentsForOnlineForm(request, formId);
            return Json(allDocs, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]//don't use this method, use below one
        public JsonResult GetApplicationFeeAndCharges(int? schemeId, int? departmentId, int? propertyTypeId, int? areaTypeId)
        {
            //var data = _onlineService.GetApplicationFeeAndCharges(schemeId, departmentId, propertyTypeId, areaTypeId);
            var data = _swpFromService.GetApplicationFeeAndCharges(schemeId, departmentId, propertyTypeId, areaTypeId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetApplicationFormFeeAndProcessingCharge(SWPFormViewModel model)
        {
            var data = _swpFromService.GetApplicationFormFeeAndProcessingCharge(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]//don't use this method use below one
        public JsonResult ValidatePANnumber(string pan, int? areaId, int? schemeId)
        {
            //int flag = _onlineService.ValidatePANnumber(pan, areaId, schemeId);
            int flag = _swpFromService.ValidatePANnumber(pan, areaId, schemeId);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ValidateFormInputFieldByType(SWPFormViewModel model)
        {
            var flag = _swpFromService.ValidateFormInputFieldByType(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult IsProcessingFeeAndReservationMoneyPaid(string formNo)
        {
            int flag = _swpFromService.GetProcessingAndReservationMoneyPaymentStatus(formNo);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult GenerateChallanForOnlineScheme(SWPFormViewModel model)
        {
            string challan = string.Empty;
            //OnlineChallanViewModel requestModel = _onlineService.GenerateSchemeChallan(objOnlineModel);
            SWPChallanViewModel requestModel = _swpFromService.SaveOfflinePaymentTransactionForChallan(model);
            if (requestModel != null)
            {
                challan = _templateParserService.GetParsedHTML(requestModel, "SchemeChallanTemplate.cshtml");
                //challan = _templateParserService.GetParsedHTML(requestModel, "SchemeChallanTemplate.cshtml");
                //bool flag = _onlineService.SaveGeneratedChallan(requestModel.ChallanId, challan);
            }
            return Json(challan, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult GenerateRTGSChallanForOnlineScheme(SWPFormViewModel model)
        {
            string challan = string.Empty;
            //OnlineChallanViewModel requestModel = _onlineService.GenerateSchemeChallan(objOnlineModel);
            SWPChallanViewModel requestModel = _swpFromService.SaveOfflinePaymentTransactionForChallan(model);
            //var data = _onlineService.GetOnlineApplicationFormById(objOnlineModel.ApplicationFormId);
            var data = _swpFromService.GetOpenEndedSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = model.ApplicationFormId });
            if (requestModel != null)
            {
                requestModel.nFormFeeGST = Convert.ToDecimal(requestModel.FormModel.FormFeeGST).ToString("#,##0.00");
                requestModel.nApplicationFee = Convert.ToDecimal(requestModel.FormModel.ApplicationFee).ToString("#,##0.00");
                requestModel.nFormFeeSGST = Convert.ToDecimal(requestModel.FormModel.FormFeeSGST).ToString("#,##0.00");
                requestModel.nFormFeeCGST = Convert.ToDecimal(requestModel.FormModel.FormFeeCGST).ToString("#,##0.00");

                requestModel.nProcessingCharge = Convert.ToDecimal(requestModel.FormModel.ProcessingCharge).ToString("#,##0.00");
                requestModel.nProcessingSGST = Convert.ToDecimal(requestModel.FormModel.ProcessingSGST).ToString("#,##0.00");
                requestModel.nProcessingCGST = Convert.ToDecimal(requestModel.FormModel.ProcessingCGST).ToString("#,##0.00");
                requestModel.nProcessingGST = Convert.ToDecimal(requestModel.FormModel.ProcessingChargeGST).ToString("#,##0.00");
                requestModel.nEarnestMoney = Convert.ToDecimal(requestModel.FormModel.EarnestMoney).ToString("#,##0.00");
                requestModel.nTotalAmountGST = Convert.ToDecimal(requestModel.FormModel.TotalAmountGST).ToString("#,##0.00");
                requestModel.TotalAmountGSTInWords = SWPApplication.ConvertNumberIntoWords(Convert.ToInt64(requestModel.FormModel.TotalAmountGST));
                
                //challan = _templateParserService.GetParsedHTML(requestModel, "ApplicationForRTGSRecieptTemplate.cshtml");
                challan = _templateParserService.GetParsedHTML(requestModel, "SchemeFormChallanTemplate.cshtml");
            }
            return Json(challan, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult PrintOnlineSchemeForm(int FormId, string SchemeType)
        {
            string schemeTemplate = string.Empty;
            SWPFormViewModel requestModel = new SWPFormViewModel();
            //requestModel = _onlineService.GetOnlineApplicationFormById(FormId);
            requestModel = _swpFromService.GetOpenEndedSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = FormId });
            if (requestModel != null)
            {
                if (SchemeType == SWPSchemeType.Transport)
                {
                    schemeTemplate = _templateParserService.GetParsedHTML(requestModel, "SchemeTransportFormTemplate.cshtml");
                }
                else if (SchemeType == SWPSchemeType.OpenEnded)
                {
                    //schemeTemplate = _templateParserService.GetParsedHTML(requestModel, "SchemeOpenEndedFormTemplate.cshtml");
                    schemeTemplate = _templateParserService.GetParsedHTML(requestModel, "OpenEndSchemeFormTemplate.cshtml");
                }
                else if (SchemeType == SWPSchemeType.IndustrialScheme)
                {
                    //schemeTemplate = _templateParserService.GetParsedHTML(requestModel, "SchemeIndustrialFormTemplate.cshtml");
                    schemeTemplate = _templateParserService.GetParsedHTML(requestModel, "IndustrialSchemeFormTemplate.cshtml");
                }
                else
                {
                    schemeTemplate = _templateParserService.GetParsedHTML(requestModel, "SchemeAllotmentFormTemplate.cshtml");
                }
            }
            return Json(schemeTemplate, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult UploadOnlineSchemeFormPaidChallan(SWPFormViewModel model, HttpPostedFileBase challan)
        {
            var result = _swpFromService.SaveOnlineSchemePaymentStatus(model, challan);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetOnlineSchemeListAsDataSource([DataSourceRequest] DataSourceRequest request, SWPDropdownViewModel model)
        {
            var schemes = _swpFromService.GetOnlineSchemeListAsDataSource(request, model);
            return Json(schemes, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDepartmentBySchemeAsDataSource([DataSourceRequest] DataSourceRequest request, SWPDropdownViewModel model)
        {
            var departments = _swpFromService.GetDepartmentBySchemeAsDataSource(request, model);
            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetOnlineSchemeApplications([DataSourceRequest] DataSourceRequest request, SWPFormViewModel modal)
        {
            var applications = _swpFromService.GetOnlineSchemeApplicationAsDataSource(request, modal);
            return Json(applications, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetOnlineSchemeFormPayment([DataSourceRequest] DataSourceRequest request, SWPFormViewModel modal)
        {
            var applications = _swpFromService.GetOnlineSchemeFormPaymentAsDataSource(request, modal);
            return Json(applications, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetSchemeFormPaymentTransaction(SWPPaymentViewModel model)
        {
            SWPPaymentViewModel payment = _swpFromService.GetSchemeFormPaymentTransaction(model);
            return Json(payment, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ValidateSchemeFormChallan(SWPPaymentViewModel model)
        {
            if (model.ActionType == "ValidateAll")
            {
                var form = _swpFromService.ValidateSchemeFormChallan(model);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                model = _swpFromService.ValidateSchemeFormChallan(model);
                if (model.ReturnTypeId == SWPReturnTypeId.Updated)
                {
                    var form = _swpFromService.GetOnlineSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = model.ApplicationFormId });

                    //NICServiceApiHelper _niveshMitraServices = new NICServiceApiHelper();
                    //SWPApiServiceProvider _niveshMitraServices = new SWPApiServiceProvider();
                    SWPStatusViewModel wsmodel = new SWPStatusViewModel();
                    wsmodel.ControlId = form.NICControlId; //model.NICControlId;
                    wsmodel.UnitId = form.NICUnitId; //model.NICUnitId;
                    wsmodel.ServiceId = form.NICServiceId; //model.NICServiceId;
                    wsmodel.ProcessIndustryId = form.ApplicationFormId.ToString(); //model.NICProcessIndustryId; 
                    wsmodel.StatusCode = SWPStatus.FORM_SUBMITTED;
                    wsmodel.Remarks = SWPStatus.Wording.FORM_SUBMITTED;

                    //_niveshMitraServices.GetWReturn_CUSID_STATUS(wsmodel);
                    var stmodel = _apiService.GetSWPServiceStatus(wsmodel);
                    model.Status = stmodel.Status;
                }
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public JsonResult ValidateOnlineSchemeForm(SWPFormViewModel model)
        {
            //model = _swpFromService.ValidateOnlineSchemeForm(model);
            model = _swpFromService.SaveOnlineSchemeFormStatus(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveOnlineSchemeFormProcessRequest(SWPFormViewModel model)
        {
            string remarks = model.Message;
            model = _swpFromService.SaveOnlineSchemeFormProcessRequest(model);

            string nicStatusCode = string.Empty;
            string nicStatus = string.Empty;
            string processStatus = string.Empty;
            if (model.ProcessType == "Process")
            {
                processStatus = ((model.ProcessTypeId != 0) ? (model.ProcessTypeId == SWPConstant.OSDApproval ? "File Move To OSD" : (model.ProcessTypeId == SWPConstant.Scrutiny ? "File goes for Scrutiny" : (model.ProcessTypeId == SWPConstant.CEOApproval ? "Approval to CEO" : (model.ProcessTypeId == SWPConstant.Draw ? "File gor for draw" : string.Empty)))) : string.Empty) + " - " + SWPStatus.Wording.INPROCESS;
            }
            if (model.ProcessType == "Status")
            {
                nicStatus = model.FormStatusId == 1 ? "Approved" : (model.FormStatusId == 2 ? "Rejected" : (model.FormStatusId == 3 ? "Cancelled" : (model.FormStatusId == 4 ? "Pending" : (model.FormStatusId == 10 ? "Objection" : (model.FormStatusId == 11 ? "Validated" : (model.FormStatusId == 18 ? "Resubmitted" : " "))))));
                nicStatusCode = model.FormStatusId == 1 ? SWPStatus.APPROVED : (model.FormStatusId == 2 ? SWPStatus.REJECTED : (model.FormStatusId == 3 ? SWPStatus.REJECTED : (model.FormStatusId == 4 ? SWPStatus.PENDING : (model.FormStatusId == 10 ? SWPStatus.QUERY_OBJECTION : (model.FormStatusId == 11 ? SWPStatus.VERIFIED : (model.FormStatusId == 18 ? SWPStatus.FORM_RE_SUBMITTED : ""))))));
            }
            
            var OnlineAppDetails = model.ResultMessage.ResultTypeList.Where(m => m.PrimaryKey > 0).ToList();
            if (OnlineAppDetails.Count > 0)
            {
                //for (int i = 0; i < OnlineAppDetails.Count(); i++)
                //{
                //    if (OnlineAppDetails[i].ReturnType == SWPReturnTypeId.Saved)
                //    {
                //        int AppId = OnlineAppDetails[i].PrimaryKey;
                //        var data = _swpFromService.GetOnlineSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = AppId });
                //        if (data != null)
                //        {
                //            SWPStatusViewModel wsmodel = new SWPStatusViewModel();
                //            wsmodel.ControlId = data.NICControlId;
                //            wsmodel.ApplicationId = data.ApplicationFormId.ToString();
                //            wsmodel.ProcessIndustryId = data.ApplicationFormId.ToString();
                //            wsmodel.UnitId = data.NICUnitId;
                //            wsmodel.ServiceId = data.NICServiceId;
                //            wsmodel.RequestId = data.NICRequestId;
                //            wsmodel.ObjectionOrRejectionCode = model.NICReasonId;
                //            wsmodel.PendancyLevel = model.NICReasonText;
                //            wsmodel.NICReasonId = model.NICReasonId;
                //            wsmodel.NICReasonText = model.NICReasonText;
                //            wsmodel.StatusCode = model.ProcessType == "Process" ? SWPStatus.INPROCESS : nicStatusCode;
                //            wsmodel.Remarks = model.ProcessType == "Process" ? processStatus : (nicStatus + ": " + remarks);
                //            wsmodel.FeeStatus = string.Empty;
                //            wsmodel.FeeAmount = string.Empty;
                //            wsmodel.PendancyLevel = nicStatus;
                //            wsmodel.NICPassSalt = Passalt;

                //            wsmodel = _apiService.GetSWPServiceStatus(wsmodel);
                //        }
                //    }
                //}
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveAllotmentDetailByOnlineSchemeFormId(SWPFormViewModel model)
        {
            model = _swpFromService.SaveAllotmentDetailByOnlineSchemeFormId(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// dropdown for scheme,department,formtype,subformtype,applicanttype,companytype,areatype,directortype,category,occupation,gender,maritalstatus
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult GetDropDownListByTypeAsDataSource([DataSourceRequest] DataSourceRequest request, SWPDropdownViewModel model)
        {
            var data = _swpFromService.GetDropDownListByTypeAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Only for Online Payment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="banktype"></param>
        [AllowAnonymous]
        public void OnlineSchemePayment(int id, int banktype)
        {
            SWPPaymentViewModel payment = _swpFromService.SaveOnlinePaymentTransaction(new SWPPaymentViewModel { ApplicationFormId = id, BankId = banktype });
            SWPPaymentGateway gateway = new SWPPaymentGateway();
            if (banktype == 1)//indusind
            {
                //gateway.PayOnline(payment);
                gateway.SWPPayOnlineINDUSIND(payment);
            }
            if (banktype == 2) //hdfc
            {
                //gateway.HDFCOnlinePaymentForSchemeForm(payment);
                gateway.SWPPayOnlineHDFC(payment);
            }
        }

        [AllowAnonymous]
        public ActionResult PaymentReceipt(FormCollection form)
        {
            var paidmodel = new SWPPaymentViewModel();
            if (form != null)
            {
                //paidmodel = _onlineService.UpdateChallanOnlinePaymentTransaction(form);
                paidmodel = _swpFromService.SaveOnlinePaymentTransactionReturn(form);
                if (paidmodel.ReturnTypeId == SWPReturnTypeId.Success)
                {
                    TempData["Message"] = "Payment completed successfully";
                    //return View(paidmodel);
                }
                else if (paidmodel.ReturnTypeId == SWPReturnTypeId.Failed)
                {
                    TempData["Message"] = "Payment request failed";
                    //return RedirectToAction("Index", "Payment", new { area = "Online", rid = form["udf2"], id = form["udf3"] });
                }
                else if (paidmodel.ReturnTypeId == SWPReturnTypeId.Mismatch)
                {
                    TempData["Message"] = "Transaction key did not match";
                    //return RedirectToAction("Index", "Payment", new { area = "Online", rid = form["udf2"], id = form["udf3"] });
                }
                else
                {
                    TempData["Message"] = "Error in Transaction";
                    //return RedirectToAction("BankChallan", "Authority", new { area = "Member" });
                }
                return View(paidmodel);
            }
            else
            {
                TempData["ErrorInTraxaction"] = "Error in Transaction";
                return RedirectToAction("SchemeInformation", "OnlineProperty", new { area = "NIC" });
            }
        }

        [AllowAnonymous]
        public JsonResult ValidateOTP(string otpMobile)
        {
            int flag = 0;
            if ((int)Session["OTPmobile"] == Convert.ToInt32(otpMobile)) flag = SWPReturnTypeId.Success;
            else { flag = SWPReturnTypeId.Failure; }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SendSchemeFormOTP(string mobileNo, string email)
        {
            int flag = SWPApplication.GenerateOTP();
            Session["OTPmobile"] = flag;
            //int flag = 123;
            //Session["OTPmobile"] = flag;
            string message = string.Format(SWPMessage.OnlineApplicationOTP, flag);
            if (mobileNo != null && mobileNo != "") SWPApplication.SendSMS(mobileNo, message);
            if (email != null && email != "") SWPApplication.SendEmail(email, "Scheme Form", message);
            return Json(flag, JsonRequestBehavior.AllowGet);;
        }

        public JsonResult GetSchemeFormChallanAsDataSource([DataSourceRequest] DataSourceRequest request, SWPFormViewModel modal)
        {
            var challan = _swpFromService.GetSchemeFormChallanAsDataSource(request, modal);
            return Json(challan, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //private SWPFormViewModel GetNICPostedBasicDetails(SWPPostViewModel apimodel)
        //{
        //    SWPFormViewModel nicmodel = new SWPFormViewModel();
        //    if (apimodel.TxtControlID != null && apimodel.TxtUnitID != null && apimodel.TxtServiceID != null)
        //    {
        //        Session["WBasicDetailsNIC"] = null;
        //        string xmlInputData = string.Empty;

        //        nicmodel.NICControlId = apimodel.TxtControlID;
        //        nicmodel.NICUnitId = apimodel.TxtUnitID;
        //        nicmodel.NICServiceId = apimodel.TxtServiceID;
        //        nicmodel.NICProcessIndustryId = apimodel.TxtProcessIndustryID;
        //        nicmodel.NICApplicationId = apimodel.TxtApplicationID;
        //        nicmodel.AppType = SWPConstant.NIC;
        //        nicmodel.IsFromNIC = true;
        //        using (var client = new upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap_nic"))
        //        {
        //            string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
        //            System.Data.DataSet result = client.WGetBasicDetails(apimodel.TxtControlID, apimodel.TxtUnitID, apimodel.TxtServiceID, apimodel.TxtProcessIndustryID, Passalt);
        //            if (result != null)
        //            {
        //                xmlInputData = result.GetXml();
        //                nicmodel.NICXmlInputTable = xmlInputData;
        //                SWPXmlDeserializer xmlSerializer = new SWPXmlDeserializer();
        //                SWPTableViewModel naDataSet = xmlSerializer.Deserialize<SWPTableViewModel>(xmlInputData);
        //                if (naDataSet.Table != null)
        //                {
        //                    naDataSet.Table.ServiceID = apimodel.TxtServiceID;
        //                    Session["WBasicDetailsNIC"] = naDataSet;
        //                    nicmodel.SWPApiBasicModel = naDataSet; //bind swptabledata
        //                    nicmodel.FlagId = SWPReturnTypeId.Saved;

        //                    return nicmodel;
        //                }
        //                else
        //                {
        //                    nicmodel.FlagId = SWPReturnTypeId.Failed;
        //                    return nicmodel;
        //                }
        //            }
        //            else
        //            {
        //                nicmodel.FlagId = SWPReturnTypeId.NotExist;
        //                return nicmodel;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        nicmodel.FlagId = SWPReturnTypeId.NotExist;
        //    }
        //    return nicmodel;
        //}

        //public string GetWReturn_CUSID_STATUS(SWPStatusViewModel apiModel)
        //{
        //    //objWReturn_CUSID_STATUSModel.ProcessIndustryID = string.Empty;
        //    //objWReturn_CUSID_STATUSModel.Fee_Status = string.Empty;
        //    apiModel.TransactionId = string.Empty;
        //    apiModel.TransactionDate = string.Empty;
        //    apiModel.TransactionDateTime = string.Empty;
        //    apiModel.NOCCertificateNo = string.Empty;
        //    apiModel.NOCUrl = string.Empty;
        //    apiModel.NOCUrlActiveStatus = string.Empty;
        //    string path = string.Empty;
        //    string xmlInputData = string.Empty;
        //    string xmlOutputData = string.Empty;
        //    using (var client = new upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap_nic"))
        //    {
        //        string result = client.WReturn_CUSID_STATUS(apiModel.ControlId, apiModel.UnitId, apiModel.ServiceId, apiModel.ProcessIndustryId, apiModel.ApplicationId, apiModel.StatusCode, apiModel.Remarks, apiModel.FeeAmount, apiModel.FeeStatus, apiModel.TransactionId, apiModel.TransactionDate, apiModel.TransactionDateTime, apiModel.NOCCertificateNo, apiModel.NOCUrl, apiModel.NOCUrlActiveStatus, apiModel.NICPassSalt);
        //        return result;
        //    }
        //}

        //private string PostReturn_CUSID_STATUS(SWPFormViewModel model, string ServiceStatusCode, string ServiceStatusRemarks)
        //{
        //    string message = string.Empty;
        //    // var data = _onlineService.GetNICSingleWindowData(model);
        //    var data = _swpFromService.GetNICSingleWindowTableData(model);
        //    if (data != null)
        //    {
        //        SWPStatusViewModel objWReturn_CUSID_STATUSModel = new SWPStatusViewModel();
        //        objWReturn_CUSID_STATUSModel.ControlId = data.ControlId;
        //        objWReturn_CUSID_STATUSModel.ApplicationId = Convert.ToString(model.ApplicationFormId);
        //        objWReturn_CUSID_STATUSModel.ProcessIndustryId = Convert.ToString(model.ApplicationFormId);
        //        objWReturn_CUSID_STATUSModel.UnitId = data.UnitId;
        //        objWReturn_CUSID_STATUSModel.ServiceId = data.ServiceId;
        //        objWReturn_CUSID_STATUSModel.StatusCode = ServiceStatusCode;
        //        objWReturn_CUSID_STATUSModel.Remarks = ServiceStatusRemarks;
        //        objWReturn_CUSID_STATUSModel.FeeStatus = ServiceStatusRemarks;
        //        objWReturn_CUSID_STATUSModel.FeeAmount = Convert.ToString(model.TotalAmount);
        //        objWReturn_CUSID_STATUSModel.NICPassSalt = Passalt;

        //        //Update status of Single Window
        //        //# verify from Service
        //        //#update details to db
        //        if (Convert.ToInt32(model.PayType) == SWPConstant.singleWindowPortalApplicationPayment)
        //        {
        //            //GetWGetUBPaymentDetails(objWReturn_CUSID_STATUSModel);
        //        }

        //        //Pass Object to NIC Service
        //        message = GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);
        //    } return message;
        //}

        //validate payment for scheme form
        [AllowAnonymous]
        public JsonResult IsApplicationFormFeePaidViaNiveshMitra(SWPFormViewModel model)
        {
            var flag = false;
            var apiStatusModel = _apiService.GetSWPSchemeFormPaymentDetail(new SWPStatusViewModel {ControlId=model.NICControlId,UnitId=model.NICUnitId,ServiceId=model.NICServiceId,RequestId=model.NICRequestId });
            if (apiStatusModel != null && apiStatusModel.NewDataSet != null)
            {
                if (apiStatusModel.NewDataSet.Table.Status_Code == SWPStatus.FEE_PAID)
                {
                    var result = _swpFromService.SaveOnlineSchemePaymentStatus(model, null);
                    flag = result.NICFeeStatusId == "11" ? true : false;
                }

                if (flag)
                {
                    SWPStatusViewModel stmodel = new SWPStatusViewModel();
                    stmodel.ControlId = model.NICControlId;
                    stmodel.UnitId = model.NICUnitId;
                    stmodel.ServiceId = model.NICServiceId;
                    stmodel.ProcessIndustryId = model.NICProcessIndustryId;
                    stmodel.RequestId = model.NICRequestId;
                    stmodel.StatusCode = SWPStatus.INPROCESS; //SWPStatus.FORM_SUBMITTED;
                    stmodel = _apiService.GetSWPServiceStatus(stmodel);
                }
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        //[AllowAnonymous]
        //public JsonResult IsApplicationFormFeePaidViaNiveshMitra(SWPFormViewModel model)
        //{
        //    var flag = false;
        //    string servicePassalt = ConfigurationManager.AppSettings["ServicePassalt"];
        //    System.Data.DataSet _newDataSet = new System.Data.DataSet();
        //    NewDataSet naDataSet = new NewDataSet();
        //    try
        //    {
        //        string path = string.Empty;
        //        string xmlInputData = string.Empty;
        //        string xmlOutputData = string.Empty;
        //        //using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
        //        using (var client = new upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap_nic"))
        //        {
        //            _newDataSet = client.WGetUBPaymentDetails(model.NICControlId, model.NICUnitId, model.NICServiceId, servicePassalt);
        //            if (_newDataSet != null)
        //            {
        //                xmlInputData = _newDataSet.GetXml();
        //                SWPXmlDeserializer xmlSerializer = new SWPXmlDeserializer();
        //                naDataSet = xmlSerializer.Deserialize<NewDataSet>(xmlInputData);
        //                if (naDataSet.Table != null)
        //                {
        //                    if (naDataSet.Table.Status_Code == SWPStatus.FEE_PAID)
        //                    {
        //                        //var result = _onlineService.UpdateOESFormPaymentStatus(model);
        //                        var result = _swpFromService.SaveOnlineSchemePaymentStatus(model, null);
        //                        flag = result.NICFeeStatusId == "11" ? true : false;

        //                        if (flag)
        //                        {
        //                            // fee status
        //                            // NICServiceApiHelper _niveshMitraServices = new NICServiceApiHelper();
        //                            //SWPApiServiceProvider _niveshMitraServices = new SWPApiServiceProvider();
        //                            SWPStatusViewModel stmodel = new SWPStatusViewModel();
        //                            stmodel.ControlId = model.NICControlId;
        //                            stmodel.UnitId = model.NICUnitId;
        //                            stmodel.ServiceId = model.NICServiceId;
        //                            stmodel.ProcessIndustryId = model.NICProcessIndustryId; //niveshMitraDetails.ProcessIndustryID;
        //                            stmodel.StatusCode = SWPStatus.SAVE_AS_DRAFT;
        //                            //stmodel.Remarks = ServiceStatus_Text.FEE_PAID;
        //                            //stmodel.Fee_Amount = objNewDataSet.Table.Fee_Amount;
        //                            //stmodel.Fee_Status = PaymentStatus_NIC.PAID;
        //                            stmodel = _apiService.GetSWPServiceStatus(stmodel);
        //                            //_niveshMitraServices.GetWReturn_CUSID_STATUS(stmodel);
        //                        }
        //                    }
        //                }
        //            }

        //        }
        //        //return flag;
        //    }
        //    catch (Exception ex)
        //    {
        //        //return objNewDataSet;
        //        throw ex;
        //    }
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}


        #endregion

        [AllowAnonymous]
        public JsonResult GetDropDownListAsDataSource([DataSourceRequest] DataSourceRequest request, SWPDropdownViewModel model)
        {
            var list = _generalService.GetDropDownListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

         [HttpPost]
         [AllowAnonymous]
         public ActionResult SaveAllottedPropertyDocument(SWPFormViewModel model, HttpPostedFileBase allotmentLetter, HttpPostedFileBase authorityLetter)
         {
             model = _swpFromService.SaveAllottedPropertyDocument(model, allotmentLetter, authorityLetter);
             if (model.IsAllotmentLetterSaved == true)
             {
                 var form = _swpFromService.GetOnlineSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = model.ApplicationFormId });
                 string servicePassalt = ConfigurationManager.AppSettings["ServicePassalt"];
                 var documentPath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.ApplicationFormId + "/Allotment/" + model.ApplicationFormId + "-Allotment.pdf");
                 SWPStatusViewModel apistatus = new SWPStatusViewModel
                 {
                     ControlId = form.NICControlId,
                     ApplicationId = form.NICApplicationId,
                     ProcessIndustryId = form.NICProcessIndustryId,
                     UnitId = form.NICUnitId,
                     ServiceId = form.NICServiceId,
                     StatusCode = SWPStatus.APPROVED,
                     FeeStatus = string.Empty,
                     FeeAmount = string.Empty,
                     NICPassSalt = servicePassalt,
                     NOCCertificateNo = "NOIDA/"+form.Department+"/"+DateTime.Now.Year+"/"+model.ApplicationFormId,
                     NOCUrl = documentPath,
                     Remarks = "Allotment Letter issued against your Scheme Form"
                 };
                 _apiService.GetSWPServiceStatus(apistatus);
             }
             return Json(model, JsonRequestBehavior.AllowGet);
         }

        [AllowAnonymous]
         public JsonResult GetApplicationFormDetailByFormId(SWPFormViewModel model)
         {
             model = _swpFromService.GetOnlineSchemeFormDataById(model);
             return Json(model, JsonRequestBehavior.AllowGet);
         }

        [AllowAnonymous]
        public JsonResult GetUploadedDocumentsAfterScrutiny(DataSourceRequest request, int? formId, int? checklistStartId, int? checklistEndId)
        {
            var allDocs = _swpFromService.GetUploadedDocumentsAfterScrutiny(request, formId, checklistStartId, checklistEndId);
            return Json(allDocs, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult UpdateDocumentAfterScrutiny(SWPFormViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            //int flag = _onlineService.UploadDocumentByFormId(model, files, null, null);
            model = _swpFromService.SaveUploadedDocument(model, files, null, null);
            if (model.ReturnTypeId != SWPReturnTypeId.Failure)
            {
                var form = _swpFromService.GetOnlineSchemeFormDataById(model);
                string servicePassalt = ConfigurationManager.AppSettings["ServicePassalt"];
                SWPStatusViewModel apistatus = new SWPStatusViewModel
                {
                    ControlId = form.NICControlId,
                    ApplicationId = form.NICApplicationId,
                    ProcessIndustryId = form.NICProcessIndustryId,
                    UnitId = form.NICUnitId,
                    ServiceId = form.NICServiceId,
                    RequestId = string.Empty,
                    StatusCode = SWPStatus.FORM_RE_SUBMITTED,
                    PendancyLevel = SWPStatus.Wording.FORM_RE_SUBMITTED,
                    Remarks = "Scheme Form is resubmitted, let it being scrutinized"
                };
                _apiService.GetSWPServiceStatus(apistatus);
                TempData["Document"] = "success";
                return RedirectToAction("ScrutinyRequiredDocument", new { id = model.EncryptedFormId });
            }
            else
            {
                TempData["Document"] = "failure";
                return RedirectToAction("ScrutinyRequiredDocument", new { id = model.EncryptedFormId });
            }
        }

        [AllowAnonymous]
        public JsonResult GetNiveshMitraPostBackForm(SWPPostViewModel model)
        {
            model = model != null ? _apiService.GetNiveshMitraPostBackForm(model) : null;
            return Json(model, JsonRequestBehavior.AllowGet);
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
        public JsonResult SaveAndGetOnlineSchemeFormApiCallan(SWPFormViewModel model)
        {
            var challan = _swpFromService.SaveAndGetOnlineSchemeFormApiCallan(model);
            return Json(challan, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetSchemeFormListForScrutinyAsDataSource([DataSourceRequest] DataSourceRequest request, SWPFormViewModel model)
        {
            var list = _swpFromService.GetSchemeFormListForScrutinyAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccountableSchemeFormListAsDataSource([DataSourceRequest] DataSourceRequest request, SWPFormViewModel model)
        {
            var list = _swpFromService.GetAccountableSchemeFormListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSchemeFormProcessStatus(SWPFormViewModel model)
        {
            model = _swpFromService.SaveSchemeFormProcessStatus(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDropDownApproverListAsDataSource([DataSourceRequest] DataSourceRequest request, SWPDropdownViewModel model)
        {
            var schemes = _generalService.GetDropDownApproverListAsDataSource(request, model);
            return Json(schemes, JsonRequestBehavior.AllowGet);
        }
    }
}
