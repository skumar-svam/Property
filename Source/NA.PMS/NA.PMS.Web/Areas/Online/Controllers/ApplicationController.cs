using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Model;
using NA.PMS.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Web.Controllers;
using NA.PMS.Common;
using NA.PMS.Service.TemplateParser;
using NA.PMS.Web.Models;
using System.Text;
using NA.PMS.Web.Controllers.Common;
using NReco.PdfGenerator;

namespace NA.PMS.Web.Areas.Online.Controllers
{
    public class ApplicationController : WebBaseController
    {
        IGeneralService _generalService;
        ISchemeService _schemeService;
        IMastersService _masterService;
        IOnlineService _onlineService;
        IAllotmentService _allotmentService;
        ITemplateParserService _templateParserService;
        public string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;

        public ApplicationController(IGeneralService generalService, ISchemeService schemeService, IMastersService masterService, IOnlineService onlineService,IAllotmentService allotmentService, TemplateParserService templateParserService)
        {
            _generalService = generalService;
            _schemeService = schemeService;
            _masterService = masterService;
            _onlineService = onlineService;
            _allotmentService = allotmentService;
            _templateParserService = templateParserService;
        }

        #region online scheme industrial 2017 sector 156

        public ActionResult Index()
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            OnlineFormViewModel model = new OnlineFormViewModel();
            model.UserId = loginUser.UserID;
            model.UserRoleType = loginUser.RoleMaster.RoleType;
            return View(model);
        }

        public ActionResult Manage()
        {
            return View();
        }

        public ActionResult ManageForm()
        {
            return View();
        }

        public ActionResult ManageAllotment()
        {
            return View();
        }

        public ActionResult Consultant()
        {
            return View();
        }

        public ActionResult ManageOnlineForm()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult IndustrialSchemeForm()
        {
            //OnlineFormViewModel model = _onlineService.GetIndustrialSchemeInformation();
            //model.DirectorModel = new onlineDirectorViewModel();
            //model.ProposedModel = new ProposedCompanyViewModel();
            //return View(model);
            Session["TempCompanyDirectors"] = null;
            OnlineFormViewModel model = GetSchemeData(NASchemeType.IndustriaScheme);
            model.DirectorModel = new onlineDirectorViewModel();
            model.ProposedModel = new ProposedCompanyViewModel();
            if (Session["SchemeUserLoginDetails"] != null)
            {
                CurrentSchemeUserDetail user = new CurrentSchemeUserDetail();
                user = (CurrentSchemeUserDetail)Session["SchemeUserLoginDetails"];
                var data = _onlineService.GetOnlineApplicationFormById(user.ApplicationFormId);
                //check applicant status like online payment, genearated challan or uploaded previous challan.
                if (data.IsApplicationFeePaid != true && data.IsChallanGenerated != true && data.IsPreviousChallanUploaded != true && data.PaidThroughSWP != true)
                {                  
                    return View(data);
                }
                else
                {
                    return RedirectToAction("IndustrialPreviewForm", "Application", new { area = "Online", id = CommonHelper.Encode(data.ApplicationFormId.ToString()) });
                }
            }

            if (Session["SchemeUserLoginDetails"] == null && Session["WBasicDetailsNIC"] != null)
            {
                NewDataSet nicdata = (NewDataSet)Session["WBasicDetailsNIC"];
                model.ApplicantType = "Company";
                model.Applicant = nicdata.Table.Company_Name;
                model.SigningAuthority = nicdata.Table.Occupier_Name;
                model.RefundInfaverof = nicdata.Table.Occupier_Name;
                //model.PanNumber = nicdata.Table.Occupier_PAN;
                // model.DOB = Convert.ToDateTime(nicdata.Table.Occupier_DOB);
                //model.ApplicantMaster = nicdata.Table.Occupier_Father_Mother_Name;
                model.MobileNumber = nicdata.Table.Occupier_Mobile_No;
                model.Email = nicdata.Table.Occupier_Email_ID;
                model.AnnualIncome = nicdata.Table.Annual_Turnover;
                model.CorrespondingAddress = nicdata.Table.Occupier_District_Name + " " + nicdata.Table.Occupier_Address + " " + nicdata.Table.Occupier_Pin_Code;
                model.PermanentAddress = nicdata.Table.Occupier_District_Name + " " + nicdata.Table.Occupier_Address + " " + nicdata.Table.Occupier_Pin_Code;
                return View(model);
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult IndustrialSchemeForm(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            var flag = _onlineService.SaveOnlineApplicationForm(model, files, userImage, signatureImage);
            return RedirectToAction("IndustryDetail", new { id = CommonHelper.Encode(flag.ToString()) });
        }

        [AllowAnonymous]
        public ActionResult IndustryDetail(string id)
        {
            if (!CheckSchemeCredentials())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    int ID = Convert.ToInt32(CommonHelper.Decode(id));
                    if (ID > 0)
                    {
                        OnlineFormViewModel model = new OnlineFormViewModel();
                        model = _onlineService.GetOnlineApplicationFormById(ID);
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
            else
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult IndustryDetail(OnlineFormViewModel model)
        {
            var flag = _onlineService.UpdateCompanyDetail(model);
            if (model.FormType == "Online") return RedirectToAction("IndustrialDocumentUpload", new { id = CommonHelper.Encode(flag.ToString()) });
            else return RedirectToAction("IndustrialPreviewForm", new { id = CommonHelper.Encode(flag.ToString()) });
        }

        [AllowAnonymous]
        public ActionResult IndustrialDocumentUpload(string id)
        {
            if (!CheckSchemeCredentials())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    int ID = Convert.ToInt32(CommonHelper.Decode(id));
                    if (ID > 0)
                    {
                        OnlineFormViewModel model = new OnlineFormViewModel();
                        model = _onlineService.GetOnlineApplicationFormById(ID);
                        if (model.FormType == "Online") return View(model);
                        else return RedirectToAction("IndustrialPreviewForm", new { id = model.EncryptedFormId });
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
            else
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult IndustrialDocumentUpload(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            int flag = _onlineService.UploadDocumentByFormId(model, files, userImage, signatureImage);
            if (flag != ReturnType.Failure)
            {
                TempData["Document"] = "success";
                return RedirectToAction("IndustrialPreviewForm", new { id = model.EncryptedFormId });
            }
            else
            {
                TempData["Document"] = "failure";
                return RedirectToAction("IndustrialDocumentUpload", new { id = model.EncryptedFormId });
            }
        }

        [AllowAnonymous]
        public ActionResult IndustrialPreviewForm(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int ID = Convert.ToInt32(CommonHelper.Decode(id));
                if (ID > 0)
                {
                    OnlineFormViewModel model = new OnlineFormViewModel();
                    model = _onlineService.GetOnlineApplicationFormById(ID);
                    if (!string.IsNullOrEmpty((string)Session["SchemeType"]))
                    {
                        model.SchemeType = (string)Session["SchemeType"];
                    }
                    //if scheme end date exceed from current date returns true.
                    if (CheckSchemeCredentials())
                    {
                        if (model.IsApplicationFeePaid == true)
                        {
                            model.BankModel = _generalService.GetBankListBySchemeId((int)model.SchemeId);
                            return View(model);
                        }
                        else { return RedirectToAction("ErrorPage"); }
                    }

                    model.BankModel = _generalService.GetBankListBySchemeId((int)model.SchemeId);
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

        [AllowAnonymous]
        public ActionResult RegisterForm()
        {
            //if scheme end date exceed from current date returns true.
            //if (!CheckSchemeCredentials())
            //{
            //    OnlineFormViewModel model = _onlineService.GetInitialDataForScheme();
            //    model.DirectorModel = new onlineDirectorViewModel();
            //    model.ProposedModel = new ProposedCompanyViewModel();
            //    //if (Session["FormId"] != null)
            //    //{
            //    //    Session["FormId"] = null;
            //    //    return RedirectToAction("EditForm", new { id = (int)Session["FormId"] });
            //    //}
            //    return View(model);
            //}
            //else
            //{
            //    return RedirectToAction("ErrorPage");
            //}

            OnlineFormViewModel model = _onlineService.GetInitialDataForScheme();
            model.DirectorModel = new onlineDirectorViewModel();
            model.ProposedModel = new ProposedCompanyViewModel();
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ErrorPage()
        {
            OnlineFormViewModel model = _onlineService.GetInitialDataForScheme();
            return View(model);
        }

        //return true if scheme end date is less than current date.
        [AllowAnonymous]
        public bool CheckSchemeCredentials()
        {
            bool flag = false;
            var model = _onlineService.GetInitialDataForScheme();
            if (model.SchemeEndDate != null)
            {
                if (DateTime.Compare((DateTime)model.SchemeEndDate, DateTime.Now) < 0)
                {
                    return flag = false; // true;
                }
            }
            return flag;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterForm(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            var flag = _onlineService.SaveOnlineApplicationForm(model, files, userImage, signatureImage);
            return RedirectToAction("CompanyDetail", new { id = CommonHelper.Encode(flag.ToString()) });
        }

        [AllowAnonymous]
        public ActionResult EditForm(string id)
        {
            //if scheme end date exceed from current date returns true.
            if (!CheckSchemeCredentials())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    int ID = Convert.ToInt32(CommonHelper.Decode(id));
                    if (ID > 0)
                    {
                        OnlineFormViewModel model = new OnlineFormViewModel();
                        model = _onlineService.GetOnlineApplicationFormById(ID);
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
            else
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditForm(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            var flag = _onlineService.UpdateOnlineApplicationForm(model, files, userImage, signatureImage);
            return RedirectToAction("CompanyDetail", new { id = CommonHelper.Encode(flag.ToString()) });
        }

        [AllowAnonymous]
        public ActionResult CompanyDetail(string id)
        {
            //if scheme end date exceed from current date returns true.
            if (!CheckSchemeCredentials())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    int ID = Convert.ToInt32(CommonHelper.Decode(id));
                    if (ID > 0)
                    {
                        OnlineFormViewModel model = new OnlineFormViewModel();
                        model = _onlineService.GetOnlineApplicationFormById(ID);
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
            else
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult CompanyDetail(OnlineFormViewModel model)
        {
            var flag = _onlineService.UpdateCompanyDetail(model);
            if (model.FormType == "Online") return RedirectToAction("UploadDocument", new { id = CommonHelper.Encode(flag.ToString()) });
            else return RedirectToAction("PreviewForm", new { id = CommonHelper.Encode(flag.ToString()) });
        }

        [AllowAnonymous]
        public ActionResult UploadDocument(string id)
        {
            //if scheme end date exceed from current date returns true.
            if (!CheckSchemeCredentials())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    int ID = Convert.ToInt32(CommonHelper.Decode(id));
                    if (ID > 0)
                    {
                        OnlineFormViewModel model = new OnlineFormViewModel();
                        model = _onlineService.GetOnlineApplicationFormById(ID);
                        if (model.FormType == "Online") return View(model);
                        else return RedirectToAction("PreviewForm", new { id = model.EncryptedFormId });
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
            else
            {
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UploadDocument(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            int flag = _onlineService.UploadDocumentByFormId(model, files, userImage, signatureImage);
            if (flag != ReturnType.Failure)
            {
                TempData["Document"] = "success";
                return RedirectToAction("PreviewForm", new { id = model.EncryptedFormId });
            }
            else
            {
                TempData["Document"] = "failure";
                return RedirectToAction("UploadDocument", new { id = model.EncryptedFormId });
            }
        }

        [AllowAnonymous]
        public ActionResult UpdateDocument(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int ID = Convert.ToInt32(CommonHelper.Decode(id));
                if (ID > 0)
                {
                    OnlineFormViewModel model = new OnlineFormViewModel();
                    model = _onlineService.GetOnlineApplicationFormById(ID);
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

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UpdateDocument(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int flag = _onlineService.UploadDocumentByFormId(model, files, null, null);
            if (flag != ReturnType.Failure)
            {
                TempData["Document"] = "success";
                return RedirectToAction("UploadDocumentScrutiny", new { id = model.EncryptedFormId });
            }
            else
            {
                TempData["Document"] = "failure";
                return RedirectToAction("UploadDocumentScrutiny", new { id = model.EncryptedFormId });
            }
        }

        [AllowAnonymous]
        public ActionResult UploadDocumentScrutiny(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (id == "undefined")
                {
                    CurrentSchemeUserDetail user = (CurrentSchemeUserDetail)Session["SchemeUserLoginDetails"];
                    if (Session["SchemeUserLoginDetails"] != null)
                    {
                        OnlineFormViewModel model = new OnlineFormViewModel();
                        model = _onlineService.GetOnlineApplicationFormById(user.ApplicationFormId);
                        return View(model);
                    }
                    else
                    {
                        return RedirectToAction("SchemeInformation");
                    }
                }
                else
                {
                    int ID = Convert.ToInt32(CommonHelper.Decode(id));
                    if (ID > 0)
                    {
                        OnlineFormViewModel model = new OnlineFormViewModel();
                        model = _onlineService.GetOnlineApplicationFormById(ID);
                        return View(model);
                    }
                    else
                    {
                        return RedirectToAction("SchemeInformation");
                    }
                }
                
            }
            else
            {
                return RedirectToAction("SchemeInformation");
            }
        }

        [AllowAnonymous]
        public JsonResult GetUploadedDocumentsAfterScrutiny(DataSourceRequest request, int? formId, int? checklistIdstart, int? checklistIdend)
        {
            var allDocs = _onlineService.GetUploadedDocumentsAfterScrutiny(request, formId, checklistIdstart, checklistIdend);
            return Json(allDocs, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult PreviewForm(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int ID = Convert.ToInt32(CommonHelper.Decode(id));
                if (ID > 0)
                {
                    OnlineFormViewModel model = new OnlineFormViewModel();
                    model = _onlineService.GetOnlineApplicationFormById(ID);
                    if (!string.IsNullOrEmpty((string)Session["SchemeType"]))
                    {
                        model.SchemeType = (string)Session["SchemeType"];
                    }
                    //if scheme end date exceed from current date returns true.
                    if (CheckSchemeCredentials())
                    {
                        if (model.IsApplicationFeePaid == true)
                        {
                            model.BankModel = _generalService.GetBankListBySchemeId((int)model.SchemeId);
                            return View(model);
                        }
                        //Commented on 4 oct 2017
                        ////User will update challan details(3 oct 2017)
                        //if (model.PaymentMode == "Offline")
                        //{
                        //    model.BankModel = _generalService.GetBankListBySchemeId((int)model.SchemeId);
                        //    return View(model);
                        //}
                        else { return RedirectToAction("ErrorPage"); }
                    }

                    model.BankModel = _generalService.GetBankListBySchemeId((int)model.SchemeId);
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
     
        //[AllowAnonymous]
        public ActionResult FormDetail(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int ID = Convert.ToInt32(CommonHelper.Decode(id));
                if (ID > 0)
                {
                    OnlineFormViewModel model = new OnlineFormViewModel();
                    model = _onlineService.GetOnlineApplicationFormById(ID);
                    return View(model);
                }
            }
            return RedirectToAction("RegisterForm");
        }

        [AllowAnonymous]
        public ActionResult SchemeInformation(NA.PMS.OnlineScheme.SchemeFormViewModel schemeForm)
        {
            NA.PMS.OnlineScheme.SchemeFormViewModel objOFVModel = new NA.PMS.OnlineScheme.SchemeFormViewModel();
            //When request is from NIC.clear all existing session
            if (System.Web.HttpContext.Current.Request.HttpMethod == HttpMethodType.POST)
            {
                //Clear all session variables.
                ClearSessionVariables();
            }

            if (Session["SchemeUserLoginDetails"] != null)
            {
                NA.PMS.OnlineScheme.SchemeFormViewModel _OnlineScheme = new NA.PMS.OnlineScheme.SchemeFormViewModel();
                _OnlineScheme = (NA.PMS.OnlineScheme.SchemeFormViewModel)Session["SchemeUserLoginDetails"];
                if (schemeForm.DepartmentId == 4)
                {
                    if (_OnlineScheme.SchemeType == OnlineSchemeType.OpenEnded)
                    { 
                        return RedirectToAction("OpenSchemeForm", "Application", new { area = "Online" }); 
                    }
                    else
                    {
                        return View();
                    }
                }
                if (schemeForm.DepartmentId == 5)
                {
                    return RedirectToAction("RentingForm", "Housing", new { area = "Online" }); 
                }
            }
            else
            {
                if (schemeForm.DepartmentId == 5)
                {
                    return RedirectToAction("RentingForm", "Housing", new { area = "Online" }); 
                }
                else
                {
                    return View();
                }
            }
            Counter();//Generate user session and update the counter by 1.
            return View();
        }

        //[AllowAnonymous]
        //public ActionResult SchemeInformation(WBasicDetailsPostModel nicdata)
        //{
        //    ViewBag.SchemeHeaderName = "Authority Online Scheme";
        //    OnlineFormViewModel naonlineform = new OnlineFormViewModel();
        //    //When request is from NIC.clear all existing session
        //    if (System.Web.HttpContext.Current.Request.HttpMethod == HttpMethodType.POST)
        //    {
        //        ClearSessionVariables();//Clear all session variables.
        //    }

        //    if (Session["SchemeUserLoginDetails"] != null)
        //    {
        //        CurrentSchemeUserDetail schemeUserDetail = new CurrentSchemeUserDetail();
        //        schemeUserDetail = (CurrentSchemeUserDetail)Session["SchemeUserLoginDetails"];
        //        if (schemeUserDetail.SchemeType == OnlineSchemeType.Transport)
        //        {
        //            return RedirectToAction("TransportSchemeForm", "Application", new { area = "Online" });
        //        }
        //        else
        //        {
        //            return RedirectToAction("SchemeInformation", "NoidaAuthority", new { area = "NIC", nicdata = new { TxtControlID = nicdata.TxtControlID, TxtUnitID = nicdata.TxtUnitID, TxtServiceID = nicdata.TxtServiceID, TxtProcessIndustryID = nicdata.TxtProcessIndustryID, TxtApplicationID = nicdata.TxtApplicationID, SchemeType=nicdata.SchemeType } });
        //        }

        //        //else if (schemeUserDetail.SchemeType == OnlineSchemeType.IndustrialScheme) 
        //        //{ 
        //        //    return RedirectToAction("IndustrialForm", "OnlineProperty", new { area = "NIC", @id = naonlineform.Id, @controlid = naonlineform.NICControlId, @unitid = naonlineform.NICUnitId, @serviceid = naonlineform.NICServiceId });
        //        //}
        //        //else if (schemeUserDetail.SchemeType == OnlineSchemeType.OpenEnded)
        //        //{
        //        //    //return RedirectToAction("OpenSchemeForm", "Application", new { area = "Online" }); 
        //        //    return RedirectToAction("OpenSchemeForm", "Application", new { area = "Online", @id = naonlineform.Id, @controlid = naonlineform.NICControlId, @unitid = naonlineform.NICUnitId, @serviceid = naonlineform.NICServiceId });
        //        //}
        //    }
        //    else
        //    {
        //        //Check request type (like post,get..)               
        //        if (System.Web.HttpContext.Current.Request.HttpMethod == HttpMethodType.POST)
        //        {
        //            if (nicdata.TxtControlID != null && nicdata.TxtUnitID != null && nicdata.TxtServiceID != null)
        //            {
        //               // //var nicdetail = GetWGetBasicDetails(nicdata);
        //               // var nicdetail = GetNICPostedBasicDetails(nicdata);
                            
        //               // if (!string.IsNullOrEmpty(nicdata.TxtApplicationID))
        //               // {
        //               //     nicdetail.ApplicationFormId = Convert.ToInt32(nicdata.TxtApplicationID);
        //               //     naonlineform = _onlineService.GetOpenEndedSchemeFormDataById(nicdetail);
        //               //     //naonlineform = nicdetail;
        //               //     if (naonlineform.SchemeType == OnlineSchemeType.IndustrialScheme)
        //               //     {
        //               //         return RedirectToAction("IndustrialForm", "OnlineProperty", new { area = "NIC", @id = naonlineform.Id, @controlid = naonlineform.NICControlId, @unitid = naonlineform.NICUnitId, @serviceid = naonlineform.NICServiceId });
        //               //     }
        //               //     else
        //               //     {
        //               //         return RedirectToAction("OpenSchemeForm", "Application", new { area = "Online", @id = naonlineform.Id, @controlid = naonlineform.NICControlId, @unitid = naonlineform.NICUnitId, @serviceid = naonlineform.NICServiceId });
        //               //     }
                            
        //               //}
        //               // else
        //               // {
        //               //     //TempData["ServiceMessage"] = "New Application Form."; //"Service Failure.";
        //               //     naonlineform = nicdetail;
        //               //     naonlineform.SchemeType = nicdata.SchemeType;
        //               //     naonlineform = GetNICPostedBasicDetails(nicdata);
        //               // }

        //                return RedirectToAction("SchemeInformation", "NoidaAuthority", new { area = "NIC",  TxtControlID = nicdata.TxtControlID, TxtUnitID = nicdata.TxtUnitID, TxtServiceID = nicdata.TxtServiceID, TxtProcessIndustryID = nicdata.TxtProcessIndustryID, TxtApplicationID = nicdata.TxtApplicationID, SchemeType = nicdata.SchemeType });
        //            }
        //            else
        //            {
        //                //TempData["ServiceMessage"] = "New Application Form."; //"Service Failure.";
                        
        //                naonlineform.SchemeType = nicdata.SchemeType;
        //                naonlineform = GetSchemeData(nicdata.SchemeType); //GetNICPostedBasicDetails(nicdata);
        //            }
        //        }
        //        else
        //        {
        //            naonlineform.IsFromNIC = false;
        //            naonlineform.SchemeType = nicdata.SchemeType;
        //            naonlineform = GetSchemeData(nicdata.SchemeType); // GetNICPostedBasicDetails(nicdata);
        //        }
        //        //var scheme = GetSchemeData(OnlineSchemeType.OpenEnded);
        //        //naonlineform.SchemeType = OnlineSchemeType.OpenEnded;
        //        //naonlineform.SchemeId = scheme.SchemeId;
        //        //naonlineform.SchemeName = scheme.SchemeName;
        //        //naonlineform.DepartmentId = scheme.DepartmentId;
        //        //naonlineform.Department = scheme.Department;
        //        //naonlineform.ApplicantType = "Company";               
        //    }
            
        //    Counter();//Generate user session and update the counter by 1.
        //    return View(naonlineform);
        //}

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
        public JsonResult GetSchemeList()
        {
            //var schemes = _generalService.GetSchemeList();
            //string schemeId = ConfigurationManager.AppSettings["SchemeId"];
            //return Json(schemes.Where(x => x.Id.ToString() == schemeId), JsonRequestBehavior.AllowGet);
            var schemes = _onlineService.GetSchemeList();
            return Json(schemes, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDepartmentList()
        {
            var departments = _generalService.GetDepartmentList();
            string departmentId = ConfigurationManager.AppSettings["DepartmentId"];
            return Json(departments.Where(x => x.Id.ToString() == departmentId), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDepartmentListBySchemeSearch([DataSourceRequest] DataSourceRequest Req, int? schemeId)
        {
            var departments = _generalService.GetDepartmentListByScheme(Req, schemeId);
            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyListForAllotment([DataSourceRequest] DataSourceRequest request, int? schemeId, int? departmentId)
        {
            var departments = _generalService.GetPropertyListForAllotment(request, schemeId, departmentId);
            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlineApplicationFormIdListForAllotment([DataSourceRequest] DataSourceRequest request, int? schemeId, int? departmentId)
        {
            var departments = _generalService.GetOnlineApplicationFormIdListForAllotment(request, schemeId, departmentId);
            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDepartmentListByScheme(int schemeId)
        {
            var departments = _generalService.GetDepartmentListByScheme(schemeId);
            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetBankListforOnline()
        {
            var banks = _generalService.GetBankList();
            return Json(banks, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetAllBranchsforOnline(int bankId)
        {
            var branchs = _generalService.GetAllBranchs(bankId);
            return Json(branchs, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public JsonResult GetPropertyTypeList(int schemeId, int departmentId)
        {
            //var propertyType = _generalService.GetPropertyTypeListByDepartment(departmentId);
            var propertyType = _generalService.GetPropertyTypeListForOnline(schemeId, departmentId);
            return Json(propertyType, JsonRequestBehavior.AllowGet);
        }

        //[AllowAnonymous]
        //public JsonResult GetFloorAreaList(int schemeId, int departmentId, int propertyTypeId)
        //{
        //    //var floorArea = _generalService.GetFloorAreaListByDepartment(departmentId);
        //    var floorArea = _generalService.GetFloorAreaListForOnline(schemeId,departmentId,propertyTypeId);
        //    return Json(floorArea, JsonRequestBehavior.AllowGet);
        //}

        [AllowAnonymous]
        public JsonResult GetFloorAreaList(int schemeId, int departmentId)
        {
            //var floorArea = _generalService.GetFloorAreaListByDepartment(departmentId);
            //var floorArea = _generalService.GetFloorAreaListForOnline(schemeId, departmentId);
            var floorArea = _onlineService.GetAreaRangeByDepartment(schemeId, departmentId);
            return Json(floorArea, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetFloorAreaListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            var list = _onlineService.GetFloorAreaListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetOnlineApplications([DataSourceRequest] DataSourceRequest request, OnlineFormViewModel modal)
        {
            var applications = _onlineService.GetOnlineApplications(request, modal);
            return Json(applications, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlineApplicationsForAdmin([DataSourceRequest] DataSourceRequest request, OnlineFormViewModel modal)
        {
            var applications = _onlineService.GetOnlineApplicationsForAdmin(request, modal);
            return Json(applications, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetOnlineApplicationsForConsultant([DataSourceRequest] DataSourceRequest request, OnlineFormViewModel modal)
        {
            var applications = _onlineService.GetOnlineApplicationsForConsultant(request, modal);
            return Json(applications, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult RejectApplication(string AppId)
        {
            int _AppId = 0;
            int ApplicationId = 0;
            if (int.TryParse(Convert.ToString(AppId), out _AppId))
                ApplicationId = _AppId;
            var RejectAppStatus = _onlineService.RejectApplication(ApplicationId);
            return Json(RejectAppStatus, JsonRequestBehavior.AllowGet);
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
            var floorArea = _generalService.GetMaritalStatusList();
            return Json(floorArea, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetCategoryList()
        {
            var floorArea = _generalService.GetCategoryList();
            return Json(floorArea, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetOccupationList()
        {
            var floorArea = _generalService.GetOccupationList();
            return Json(floorArea, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetSectorList()
        {
            var sectors = _generalService.getSectorsList();
            return Json(sectors, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetCompanyTypeList(string typeName)
        {
            var floorArea = _generalService.GetCompanyTypeByCategory(typeName);
            return Json(floorArea, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetChecklistDocumentsForOnlineApplication([DataSourceRequest]DataSourceRequest request, int? schemeId)
        {
            var allDocs = _onlineService.GetChecklistDocumentsForOnlineApplication(request, schemeId);
            return Json(allDocs, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetUploadedDocumentsForOnlineForm([DataSourceRequest]DataSourceRequest request, int? formId)
        {
            var allDocs = _onlineService.GetUploadedDocumentsForOnlineForm(request, formId);
            return Json(allDocs, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetUploadedDocumentsForReturnForm([DataSourceRequest]DataSourceRequest request, int? formId)
        {
            var allDocs = _onlineService.GetUploadedDocumentsForReturnForm(request, formId);
            return Json(allDocs, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetListOfChecklistDocuments(int? schemeId)
        {
            var allDocs = _onlineService.GetListOfChecklistDocuments(schemeId);
            ViewBag.uploadFilehtml = allDocs;
            return Json(allDocs, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetApplicationFeeAndCharges(int? schemeId, int? departmentId, int? propertyTypeId, int? areaTypeId)
        {
            var data = _onlineService.GetApplicationFeeAndCharges(schemeId, departmentId, propertyTypeId, areaTypeId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetOnlineApplicationFeeAndCharges(OnlineFormViewModel model)
        {
            var data = _onlineService.GetOnlineApplicationFeeAndCharges(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ValidatePANnumber(string pan, int? areaId, int? schemeId)
        {
            int flag = _onlineService.ValidatePANnumber(pan, areaId, schemeId);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ValidatePANForOnlineScheme(OnlineFormViewModel model)
        {
            int flag = _onlineService.ValidatePANForOnlineScheme(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ValidateFormNumber(string formNo)
        {
            int flag = _onlineService.ValidateFormNumber(formNo);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ValidateOnlineFormPayment(string formNo)
        {
            int flag = _onlineService.ValidateOnlineFormPayment(formNo);
            if (flag == ReturnType.Exist)
            {
                return Json(Convert.ToString(flag) + "#" + CommonHelper.Encode(formNo), JsonRequestBehavior.AllowGet);
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult RemoveDocumentFromApplicationForm(string formNo, string filename)
        {
            int flag = _onlineService.RemoveDocumentFromApplicationForm(formNo, filename);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SendOTPforPayment(string formId)
        {
            //List<int> otpList = _onlineService.SendOTPforPayment(formId);
            var data = _onlineService.GetOnlineApplicationFormById(Convert.ToInt32(formId));
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
            if ((int)Session["OTPmobile"] == Convert.ToInt32(otpMobile)) flag = ReturnType.Success;
            //if ((string)Session["OTPmobile"] == (otpMobile)) { flag = ReturnType.Success; }
            else { flag = ReturnType.Failure; }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SendOTP(string mobileNo, string email)
        {
            int flag = ApplicationHelper.GenerateOTP();
            Session["OTPmobile"] = flag;
            //int flag = 123;
            //Session["OTPmobile"] = flag;
            string emailMessage = string.Format(NAMessages.OnlineApplicationOTP, flag);
            string mobileMessage = string.Format(NAMessages.OnlineApplicationOTP, flag);
            if (mobileNo != null && mobileNo != "") ApplicationHelper.SendSMS(mobileNo, mobileMessage);
            if (email != null && email != "") ApplicationHelper.SendEmail(email, "OnlineForm", emailMessage);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult IsDocumentUploaded(int? formId)
        {
            var flag = _onlineService.IsDocumentUploaded(formId);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult PrintSchemeForm(int FormId, string SchemeType)
        {
            string ApplicationForm = string.Empty;
            OnlineFormViewModel requestModel = new OnlineFormViewModel();
            requestModel = _onlineService.GetOnlineApplicationFormById(FormId);
            if (requestModel != null)
            {
                if (SchemeType == OnlineSchemeType.Transport)
                {
                    ApplicationForm = _templateParserService.GetParsedHTML(requestModel, "SchemeTransportFormTemplate.cshtml");
                }
                else if (SchemeType == OnlineSchemeType.OpenEnded)
                {
                    ApplicationForm = _templateParserService.GetParsedHTML(requestModel, "SchemeOpenEndedFormTemplate.cshtml");
                }
                else
                {
                    ApplicationForm = _templateParserService.GetParsedHTML(requestModel, "SchemeAllotmentFormTemplate.cshtml");
                }
            }
            return Json(ApplicationForm, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDirectorTypeList()
        {
            var typeList = _generalService.GetDirectorTypeList();
            return Json(typeList, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDirectorDetails([DataSourceRequest] DataSourceRequest request, int? formId)
        {
            if (formId != null)
            {
                DataSourceResult directors = _onlineService.GetDirectorDetails(request, formId);
                return Json(directors, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SaveDirectorDetails(int? formId, string directorName, decimal? share, int? directorTypeId, string pan)
        {
            if (formId != null)
            {
                int flag = _onlineService.SaveDirectorDetails(formId, directorName, share, directorTypeId, pan);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult RemoveDirectorDetails(int? formId, int? directorId)
        {
            if (formId != null)
            {
                int flag = _onlineService.RemoveDirectorDetails(formId, directorId);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult DownloadApplicationForm(int? FormId)
        {
            string ApplicationForm = string.Empty;
            //return new Rotativa.ActionAsPdf("ApplicationFormTable", new { FormId = FormId });
            return null;
        }

        [AllowAnonymous]
        public string ApplicationFormTable(int? FormId)
        {
            string ApplicationForm = string.Empty;
            OnlineFormViewModel requestModel = new OnlineFormViewModel();
            requestModel = _onlineService.GetOnlineApplicationFormById(FormId);
            if (requestModel != null)
            {
                ApplicationForm = _templateParserService.GetParsedHTML(requestModel, "SchemeAllotmentFormTemplate.cshtml");
            }
            return ApplicationForm;
        }

        [AllowAnonymous]
        public JsonResult EncryptFormId(string formId)
        {
            string formid = CommonHelper.Encode(formId);
            return Json(formid, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendMessageInBulk(string type)
        {
            int flag = _onlineService.SendMessageInBulk(type);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region online scheme 2018 sector 157

        #region Transport Scheme

        [AllowAnonymous]
        public ActionResult TransportSchemeForm()
        {
            Session["TempCompanyDirectors"] = null;
            OnlineFormViewModel model = GetSchemeData(OnlineSchemeType.Transport);
            model.DirectorModel = new onlineDirectorViewModel();
            model.ProposedModel = new ProposedCompanyViewModel();
            if (Session["SchemeUserLoginDetails"] != null)
            {
                //CurrentSchemeUserDetail schemeUserDetail = new CurrentSchemeUserDetail();
                //schemeUserDetail = (CurrentSchemeUserDetail)Session["SchemeUserLoginDetails"];
                //var data = _onlineService.GetOnlineApplicationFormById(schemeUserDetail.ApplicationFormId);
                var _ExForm = (OnlineFormViewModel)Session["SchemeUserLoginDetails"];
                var data = _onlineService.GetOnlineApplicationFormById(_ExForm.ApplicationFormId);
                //check applicant status like online payment, genearated challan or uploaded previous challan.
                if (data.IsApplicationFeePaid != true && data.IsChallanGenerated != true && data.IsPreviousChallanUploaded != true && data.PaidThroughSWP != true)
                {
                    return View(data);
                }
                else
                {
                    return RedirectToAction("PreviewForm", "Application", new { area = "Online", id = CommonHelper.Encode(data.ApplicationFormId.ToString()) });
                }
            }

            if (Session["SchemeUserLoginDetails"] == null && Session["WBasicDetailsNIC"] != null)
            {
                NewDataSet objNewDataSet = (NewDataSet)Session["WBasicDetailsNIC"];
                model.ApplicantType = "Company";
                model.Applicant = objNewDataSet.Table.Company_Name;
                model.SigningAuthority = objNewDataSet.Table.Occupier_Name;
                model.RefundInfaverof = objNewDataSet.Table.Occupier_Name;
                //model.PanNumber = objNewDataSet.Table.Occupier_PAN;
                // model.DOB = Convert.ToDateTime(objNewDataSet.Table.Occupier_DOB);
                //model.ApplicantMaster = objNewDataSet.Table.Occupier_Father_Mother_Name;
                model.MobileNumber = objNewDataSet.Table.Occupier_Mobile_No;
                model.Email = objNewDataSet.Table.Occupier_Email_ID;
                model.AnnualIncome = objNewDataSet.Table.Annual_Turnover;
                model.CorrespondingAddress = objNewDataSet.Table.Occupier_District_Name + " " + objNewDataSet.Table.Occupier_Address + " " + objNewDataSet.Table.Occupier_Pin_Code;
                model.PermanentAddress = objNewDataSet.Table.Occupier_District_Name + " " + objNewDataSet.Table.Occupier_Address + " " + objNewDataSet.Table.Occupier_Pin_Code;
                return View(model);
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterTransportSchemeForm(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            model.SchemeType = OnlineSchemeType.Transport;
            Session["SchemeType"] = model.SchemeType;
            int flag = 0;
            if (Session["SchemeUserLoginDetails"] == null)
            {
                if (Session["WBasicDetailsNIC"] != null) { model.BasicDetailsGetModel = (NewDataSet)Session["WBasicDetailsNIC"]; }
                flag = _onlineService.SaveOnlineApplicationForm(model, files, userImage, signatureImage);
                if (flag > 0)
                {
                    if (Session["WBasicDetailsNIC"] != null)
                    {
                        string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
                        WReturn_CUSID_STATUSModel _NIC_STATUSModel = new WReturn_CUSID_STATUSModel();
                        _NIC_STATUSModel.ControlID = model.BasicDetailsGetModel.Table.Control_ID;
                        _NIC_STATUSModel.ApplicationID = Convert.ToString(flag);
                        _NIC_STATUSModel.ProcessIndustryID = Convert.ToString(flag);
                        _NIC_STATUSModel.UnitID = model.BasicDetailsGetModel.Table.Unit_Id;
                        _NIC_STATUSModel.ServiceID = model.BasicDetailsGetModel.Table.ServiceID;
                        _NIC_STATUSModel.Status_Code = ServiceStatus.SAVE_AS_DRAFT;
                        _NIC_STATUSModel.Remarks = "SAVE AS DRAFT";
                        _NIC_STATUSModel.Fee_Amount = Convert.ToString(model.TotalAmount);
                        _NIC_STATUSModel.passsalt = Passalt;
                        string message = GetWReturn_CUSID_STATUS(_NIC_STATUSModel);
                        if (message == "Failure")
                        {
                            TempData["ServiceMessage"] = "Failure";
                            return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                        }
                    }
                }
                else
                {
                    TempData["ServiceMessage"] = "Failure";
                    return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                }
            }
            else
            {
                CurrentSchemeUserDetail _SchemeUserDetail = new CurrentSchemeUserDetail();
                _SchemeUserDetail = (CurrentSchemeUserDetail)Session["SchemeUserLoginDetails"];
                if (_SchemeUserDetail.ApplicationFormId == model.ApplicationFormId)
                {
                    flag = _onlineService.UpdateOnlineApplicationForm(model, files, userImage, signatureImage);
                }
                else { RedirectToAction("TransportSchemeForm", new { id = CommonHelper.Encode(flag.ToString()) }); }
            }
            return RedirectToAction("TransportSchemeDocument", new { id = CommonHelper.Encode(flag.ToString()) });
        }


        [AllowAnonymous]
        public ActionResult TransportSchemeDocument(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int ID = Convert.ToInt32(CommonHelper.Decode(id));
                if (ID > 0)
                {
                    OnlineFormViewModel model = new OnlineFormViewModel();
                    model = _onlineService.GetOnlineApplicationFormById(ID);
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

        #endregion

        #region Scheme data and other methods

        private OnlineFormViewModel GetSchemeData(string SchemeType)
        {
            int schemeId = GetSchemeIdBySchemeType(SchemeType);
            int departmentId = ConfigurationManager.AppSettings["DepartmentId"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["DepartmentId"]) : 0;
            OnlineFormViewModel schemeInfo = new OnlineFormViewModel();
            if (SchemeType == NASchemeType.IndustriaScheme) 
                schemeInfo = _onlineService.GetSchemeInformationForOnlineApplication(new OnlineFormViewModel { SchemeId = schemeId ,DepartmentId = departmentId });
            else if (SchemeType == NASchemeType.InstitutionalScheme)
            {
                departmentId = ConfigurationManager.AppSettings["InstitutionalDepartmentId"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["InstitutionalDepartmentId"]) : 0;
                schemeInfo = _onlineService.GetInitialDataForScheme(new OnlineFormViewModel { SchemeId = schemeId, DepartmentId = departmentId });
            }
            else
                schemeInfo = _onlineService.GetInitialDataForScheme(new OnlineFormViewModel { SchemeId = schemeId ,DepartmentId=departmentId});
            return schemeInfo;
        }

        private int GetSchemeIdBySchemeType(string SchemeType)
        {
            int schemeId = 0;
            if (SchemeType == OnlineSchemeType.IndustrialPlots) { schemeId = ConfigurationManager.AppSettings["SchemeId"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["SchemeId"]) : 0; }
            else if (SchemeType == OnlineSchemeType.Transport) { schemeId = ConfigurationManager.AppSettings["TransportScheme"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["TransportScheme"]) : 0; }
            else if (SchemeType == OnlineSchemeType.OpenEnded) { schemeId = ConfigurationManager.AppSettings["OpenScheme"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["OpenScheme"]) : 0; }
            else if (SchemeType == NASchemeType.IndustriaScheme) { schemeId = ConfigurationManager.AppSettings["IndustrialSchemeId"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["IndustrialSchemeId"]) : 0; }
            else if (SchemeType == OnlineSchemeType.Institutional) { schemeId = ConfigurationManager.AppSettings["InstitutionalScheme"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["InstitutionalScheme"]) : 0; }
            return schemeId;
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
        #endregion

        #region Director

        [AllowAnonymous]
        public JsonResult SaveDirectorDetailsForOpenScheme(onlineDirectorViewModel model)
        {
            //var flag = _onlineService.SaveDirectorDetailsForOpenScheme(model);
            model = _onlineService.SaveDirectorDetailToDataBaseForOpenScheme(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SaveDirectorDetailsForOnlineSchemeInSession(onlineDirectorViewModel model)
        {
            var flag = _onlineService.SaveDirectorDetailsForOpenScheme(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDirectorDetailsForOpenScheme(DataSourceRequest request, int? id)
        {
            //var flag = _onlineService.GetDirectorDetailsForOpenScheme(request, id);
            var flag = _onlineService.GetDirectorDetailsFromDatabaseForOpenScheme(request, id);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDirectorDetailsFromSessionForOnlineScheme(DataSourceRequest request, int? id)
        {
            var flag = _onlineService.GetDirectorDetailsForOpenScheme(request, id);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult RemoveDirectorDetailsForOpenScheme(int? id)
        {
            var flag = _onlineService.RemoveDirectorDetailsForOpenScheme(id);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Open Scheme Form

        //[AllowAnonymous]
        //public ActionResult OpenSchemeForm()
        //{
        //    OnlineFormViewModel form = GetSchemeData(OnlineSchemeType.OpenEnded);
        //    form = _onlineService.GetInitialDataForScheme(form);
        //    form.DirectorModel = new onlineDirectorViewModel();
        //    form.ProposedModel = new ProposedCompanyViewModel();
        //    if (Session["SchemeUserLoginDetails"] != null)
        //    {
        //        CurrentSchemeUserDetail objCurrentSchemeUserDetail = new CurrentSchemeUserDetail();
        //        objCurrentSchemeUserDetail = (CurrentSchemeUserDetail)Session["SchemeUserLoginDetails"];
        //        var data = _onlineService.GetOnlineApplicationFormById(objCurrentSchemeUserDetail.ApplicationFormId);

        //        //check applicant status like online payment, genearated challan, uploaded previous challan & single window portal(In case of NIC).
        //        if (data.IsApplicationFeePaid != true && data.IsChallanGenerated != true && data.IsPreviousChallanUploaded != true && data.PaidThroughSWP != true)
        //        {
        //            return View(data);
        //        }
        //        else
        //        {
        //            return RedirectToAction("PreviewForm", "Application", new { area = "Online", id = CommonHelper.Encode(data.ApplicationFormId.ToString()) });
        //        }
        //    }

        //    if (Session["SchemeUserLoginDetails"] == null && Session["WBasicDetailsNIC"] != null)
        //    {
        //        NewDataSet objNewDataSet = (NewDataSet)Session["WBasicDetailsNIC"];
        //        //form.LastName = Convert.ToString(FullName[2] != null ? FullName[2] : (FullName[1] != null ? FullName[1] : string.Empty));
        //        form.ApplicantType = "Company";
        //        form.SigningAuthorityId = 1;
        //        form.Applicant = objNewDataSet.Table.Company_Name;
        //        form.SigningAuthority = objNewDataSet.Table.Occupier_Name;
        //        form.RefundInfaverof = objNewDataSet.Table.Occupier_Name;
        //        //form.PanNumber = objNewDataSet.Table.Occupier_PAN;
        //        // form.DOB = Convert.ToDateTime(objNewDataSet.Table.Occupier_DOB);
        //        //form.ApplicantMaster = objNewDataSet.Table.Occupier_Father_Mother_Name;
        //        form.MobileNumber = objNewDataSet.Table.Occupier_Mobile_No;
        //        form.Email = objNewDataSet.Table.Occupier_Email_ID;
        //        form.AnnualIncome = objNewDataSet.Table.Annual_Turnover;
        //        form.CorrespondingAddress = objNewDataSet.Table.Occupier_District_Name + " " + objNewDataSet.Table.Occupier_Address + " " + objNewDataSet.Table.Occupier_Pin_Code;
        //        form.PermanentAddress = objNewDataSet.Table.Occupier_District_Name + " " + objNewDataSet.Table.Occupier_Address + " " + objNewDataSet.Table.Occupier_Pin_Code;

        //        form.BasicDetailsGetModel = objNewDataSet;

        //        return View(form);
        //    }
        //    return View(form);
        //}

        //[HttpPost]
        [AllowAnonymous]
        //public ActionResult OpenSchemeForm(OnlineFormViewModel form)
        public ActionResult OpenSchemeForm(int? id, string controlid,string unitid,string serviceid)
        {
            OnlineFormViewModel form = GetSchemeData(OnlineSchemeType.OpenEnded);
            if (!string.IsNullOrEmpty(controlid) && !string.IsNullOrEmpty(controlid) && !string.IsNullOrEmpty(controlid))
            {
                WBasicDetailsPostModel apidata = new WBasicDetailsPostModel { TxtControlID = controlid, TxtServiceID = serviceid, TxtUnitID = unitid};
                if (id != null && id > 0)
                {
                    apidata.TxtApplicationID = id.ToString();
                    apidata.TxtProcessIndustryID = id.ToString();
                    var nicdetail = GetNICPostedBasicDetails(apidata);
                    nicdetail.ApplicationFormId = id;
                    form = _onlineService.GetOpenEndedSchemeFormDataById(nicdetail);
                }
                else
                {
                    var nicdetail = GetNICPostedBasicDetails(apidata);
                    form.NICControlId = nicdetail.NICControlId;
                    form.NICUnitId = nicdetail.NICUnitId;
                    form.NICServiceId = nicdetail.NICServiceId;
                    form.NICProcessIndustryId = nicdetail.NICProcessIndustryId;
                    form.NICApplicationId = nicdetail.NICApplicationId;
                    form.NICXmlInputTable = nicdetail.NICXmlInputTable;
                    form.BasicDetailsGetModel = nicdetail.BasicDetailsGetModel;
                    form.FlagId = nicdetail.FlagId;
                    form.IsFromNIC = nicdetail.IsFromNIC;
                    form.ApplicantType = "Company";
                }
            }
            else
            {
                //local authority user
                if (id != null && id > 0)
                {
                    form.ApplicationFormId = id;
                    form = _onlineService.GetOpenEndedSchemeFormDataById(form);
                    form.IsFromNIC = false;
                }
                else
                {
                    //form = _onlineService.GetInitialDataForScheme(form);
                    form.SchemeType = OnlineSchemeType.OpenEnded;
                    form.DirectorModel = new onlineDirectorViewModel();
                    form.ProposedModel = new ProposedCompanyViewModel();
                    if (Session["SchemeUserLoginDetails"] != null)
                    {
                        OnlineFormViewModel schemelogin = new OnlineFormViewModel();
                        schemelogin = (OnlineFormViewModel)Session["SchemeUserLoginDetails"];
                        form.ApplicationFormId = schemelogin.ApplicationFormId;
                        form = _onlineService.GetOpenEndedSchemeFormDataById(form);
                        //var data = _onlineService.GetOnlineApplicationFormById(schemelogin.ApplicationFormId);

                        //check applicant status like online payment, genearated challan, uploaded previous challan & single window portal(In case of NIC).
                        if (form.IsApplicationFeePaid != true && form.IsChallanGenerated != true && form.IsPreviousChallanUploaded != true && form.PaidThroughSWP != true)
                        {
                            return View(form);
                        }
                        else
                        {
                            return RedirectToAction("PreviewForm", "Application", new { area = "Online", id = CommonHelper.Encode(form.ApplicationFormId.ToString()) });
                        }
                    }
                }               
            }
            
            return View(form);
        }

        [AllowAnonymous]
        //public ActionResult ProjectAndRefund(OnlineFormViewModel form)
        public ActionResult ProjectAndRefund(int? id, string controlid, string unitid, string serviceid)
        {
            OnlineFormViewModel form = GetSchemeData(OnlineSchemeType.OpenEnded);
            if (!string.IsNullOrEmpty(controlid) && !string.IsNullOrEmpty(controlid) && !string.IsNullOrEmpty(controlid))
            {
                WBasicDetailsPostModel apidata = new WBasicDetailsPostModel { TxtControlID = controlid, TxtServiceID = serviceid, TxtUnitID = unitid };
                if (id != null && id > 0)
                {
                    apidata.TxtApplicationID = id.ToString();
                    apidata.TxtProcessIndustryID = id.ToString();
                    var nicdetail = GetNICPostedBasicDetails(apidata);
                    nicdetail.ApplicationFormId = id;
                    form = _onlineService.GetOpenEndedSchemeFormDataById(nicdetail);
                }
                else
                {
                    return RedirectToAction("OpenSchemeForm", "Application", new { area = "Online", @id = id, @controlid = controlid, @unitid = unitid, @serviceid = serviceid });
                }
            }
            else
            {
                //local authority user
                if (id != null && id > 0)
                {
                    form.ApplicationFormId = id;
                    form = _onlineService.GetOpenEndedSchemeFormDataById(form);
                }
                else
                {
                    return RedirectToAction("OpenSchemeForm", "Application", new { area = "Online", @id = id, @controlid = controlid, @unitid = unitid, @serviceid = serviceid });
                }
            }

            return View(form);
        }

        [AllowAnonymous]
        public ActionResult OpenSchemeDocument(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int ID = Convert.ToInt32(CommonHelper.Decode(id));
                if (ID > 0)
                {
                    OnlineFormViewModel model = new OnlineFormViewModel();
                    model = _onlineService.GetOnlineApplicationFormById(ID);
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

        //[AllowAnonymous]
        //[HttpPost]
        //public JsonResult SaveOpenSchemeFormDetail(OnlineFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        //{
        //    model.SchemeType = OnlineSchemeType.OpenEnded;
        //    Session["SchemeType"] = model.SchemeType;
        //    //model.SchemeType = OnlineSchemeType.OpenEnded;
        //    //if (Session["WBasicDetailsNIC"] != null) { model.BasicDetailsGetModel = (NewDataSet)Session["WBasicDetailsNIC"]; }
        //    var flag = _onlineService.SaveOpenSchemeFormDetail(model, userImage, signatureImage);
        //    if (!string.IsNullOrEmpty(flag))
        //    {
        //        if (Session["WBasicDetailsNIC"] != null)
        //        {
        //            //Post Request to NIC Service
        //            string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
        //            WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel();
        //            objWReturn_CUSID_STATUSModel.ControlID = model.BasicDetailsGetModel.Table.Control_ID;
        //            objWReturn_CUSID_STATUSModel.ApplicationID = Convert.ToString(CommonHelper.Decode(flag));
        //            objWReturn_CUSID_STATUSModel.ProcessIndustryID = Convert.ToString(CommonHelper.Decode(flag));
        //            objWReturn_CUSID_STATUSModel.UnitID = model.BasicDetailsGetModel.Table.Unit_Id;
        //            objWReturn_CUSID_STATUSModel.ServiceID = model.BasicDetailsGetModel.Table.ServiceID;
        //            objWReturn_CUSID_STATUSModel.Status_Code = ServiceStatus.SAVE_AS_DRAFT;
        //            objWReturn_CUSID_STATUSModel.Remarks = ServiceStatus_Text.SAVE_AS_DRAFT;
        //            objWReturn_CUSID_STATUSModel.Fee_Amount = Convert.ToString(model.TotalAmount);
        //            objWReturn_CUSID_STATUSModel.passsalt = Passalt;
        //            //Pass Object to NIC Service
        //            string message = GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);
        //            if (message == "Failure")
        //            {
        //                TempData["ServiceMessage"] = "Service Failure";
        //            }
        //        }
        //    }
        //    else
        //    {
        //        TempData["ServiceMessage"] = "Service Failure";
        //    }
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        [AllowAnonymous]
        [HttpPost]
        public JsonResult SaveOpenSchemeFormDetail(OnlineFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            model.SchemeType = OnlineSchemeType.OpenEnded;
            Session["SchemeType"] = model.SchemeType;           
            var form = _onlineService.SaveOpenEndedSchemeFormDetail(model, userImage, signatureImage);
            if (form.IsFromNIC == true && form.FormStatusId == NAStatusId.Success)
            {
                //Post Request to NIC Service
                string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
                WReturn_CUSID_STATUSModel nicReturnModel = new WReturn_CUSID_STATUSModel();
                nicReturnModel.ControlID = model.BasicDetailsGetModel.Table.Control_ID;
                nicReturnModel.ApplicationID = model.ApplicationFormId.ToString(); //Convert.ToString(CommonHelper.Decode(flag));
                //nicReturnModel.ProcessIndustryID = model.BasicDetailsGetModel.Table.ProcessIndustryID; //Convert.ToString(CommonHelper.Decode(flag));
                nicReturnModel.ProcessIndustryID = model.BasicDetailsGetModel.Table.ProcessIndustryID == null ? model.ApplicationFormId.ToString() : model.BasicDetailsGetModel.Table.ProcessIndustryID; 
                nicReturnModel.UnitID = model.BasicDetailsGetModel.Table.Unit_Id;
                nicReturnModel.ServiceID = model.BasicDetailsGetModel.Table.ServiceID == null ? model.NICServiceId : model.BasicDetailsGetModel.Table.ServiceID;
                nicReturnModel.Status_Code = ServiceStatus.FEE_PENDING; //ServiceStatus.SAVE_AS_DRAFT;
                //nicReturnModel.Fee_Status = ServiceStatus.FEE_PENDING;
                nicReturnModel.Fee_Status = PaymentStatus_NIC.UB;
                //nicReturnModel.Remarks = ServiceStatus_Text.SAVE_AS_DRAFT;
                //nicReturnModel.Fee_Amount = Convert.ToString(model.TotalAmount);
                nicReturnModel.Remarks = ServiceStatus_Text.FEE_PENDING;
                //nicReturnModel.Fee_Amount = Convert.ToString(model.FormFeeWithGST);
                nicReturnModel.Fee_Amount = "5900";
                nicReturnModel.passsalt = Passalt;
                //Pass Object to NIC Service
                string message = GetWReturn_CUSID_STATUS(nicReturnModel);
                if (message == "FAILED") //"Failure"
                {
                    TempData["ServiceMessage"] = "NIC Service Failed";
                }
                else
                {
                    TempData["ServiceMessage"] = "NIC updated successfully.";
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



        #endregion

        #region Scheme Login ,Logout,Change password and forgot password

        [AllowAnonymous]
        public ActionResult SchemeLogin(SchemeLoginModel schemeLogin)
        {
            if (schemeLogin.txtUserName != null && schemeLogin.txtPassword != null)
            {
                if (!string.IsNullOrEmpty(schemeLogin.txtUserName))
                {
                    int flag = 0;
                    OnlineFormViewModel onlineForm = new OnlineFormViewModel();
                    onlineForm.ApplicationFormId = Convert.ToInt32(schemeLogin.txtUserName);
                    onlineForm.UserPassword = schemeLogin.txtPassword;
                    onlineForm.AppType = schemeLogin.AppType;
                    flag = _onlineService.ValidateApplicationDetails(onlineForm);
                    if (flag == ReturnType.Exist)
                    {
                        Session["SchemeUserLoginDetails"] = null;
                        //OnlineFormViewModel exForm = new OnlineFormViewModel();
                        var exForm = _onlineService.GetOnlineSchemeFormById(onlineForm.ApplicationFormId);
                        //NA.PMS.OnlineScheme.SchemeFormViewModel _schemeUserDetail = null;
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
                        _schemeUserDetail.SchemeType = schemeLogin.SchemeType;
                        _schemeUserDetail.AppType = onlineForm.AppType;

                        //Session["SchemeUserLoginDetails"] = _schemeUserDetail;

                        string _openScheme = ConfigurationManager.AppSettings["OpenScheme"];
                        
                        if (exForm.DepartmentId == 4)
                        {
                            if (schemeLogin.SchemeType == OnlineSchemeType.OpenEnded)
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
                    else if (flag == ReturnType.UserNameNotExist)
                    {
                        TempData["ErrorLoginMessage"] = "UserName/FormID is not correct."; return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                    }
                    else if (flag == ReturnType.PasswordNotExist)
                    {
                        TempData["ErrorLoginMessage"] = "Password is not correct."; return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                    }
                    else if (flag == ReturnType.Failure)
                    {
                        TempData["ErrorLoginMessage"] = "Form not validated."; return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                    }
                }
            }
            return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
        }

        //[AllowAnonymous]
        //public ActionResult SchemeLogin(SchemeLoginModel schemeLogin)
        //{
        //    if (schemeLogin.txtUserName != null && schemeLogin.txtPassword != null)
        //    {
        //        if (!string.IsNullOrEmpty(schemeLogin.txtUserName))
        //        {
        //            int flag = 0;
        //            OnlineFormViewModel onlineForm = new OnlineFormViewModel();
        //            onlineForm.ApplicationFormId = Convert.ToInt32(schemeLogin.txtUserName);
        //            onlineForm.UserPassword = schemeLogin.txtPassword;
        //            onlineForm.AppType = schemeLogin.AppType;
        //            flag = _onlineService.ValidateApplicationDetails(onlineForm);
        //            if (flag == ReturnType.Exist)
        //            {
        //                Session["SchemeUserLoginDetails"] = null;
        //                OnlineFormViewModel exForm = new OnlineFormViewModel();
        //                exForm = _onlineService.GetOnlineSchemeFormById(onlineForm.ApplicationFormId);
        //                var exForm2 = _onlineService.GetOpenEndedSchemeFormDataById(new OnlineFormViewModel { ApplicationFormId = Convert.ToInt32(schemeLogin.txtUserName), UserPassword = schemeLogin.txtPassword });
                        
        //                string _openScheme = ConfigurationManager.AppSettings["OpenScheme"];
        //                string _transportScheme = ConfigurationManager.AppSettings["TransportScheme"];
        //                string _schemeId201819 = ConfigurationManager.AppSettings["IndustrialSchemeId"];
        //                string _institutionalScheme = ConfigurationManager.AppSettings["InstitutionalScheme"];

        //                int _openSchemeId = !string.IsNullOrEmpty(_openScheme) ? Convert.ToInt32(_openScheme) : 0;
        //                int _transportSchemeId = !string.IsNullOrEmpty(_transportScheme) ? Convert.ToInt32(_transportScheme) : 0;
        //                int _IndustrialSchemeId = !string.IsNullOrEmpty(_schemeId201819) ? Convert.ToInt32(_schemeId201819) : 0;
        //                int _institutionalSchemeId = !string.IsNullOrEmpty(_institutionalScheme) ? Convert.ToInt32(_institutionalScheme) : 0;
        //                if (exForm.SchemeType == OnlineSchemeType.OpenEnded && exForm.SchemeId == _openSchemeId)
        //                {
        //                    Session["SchemeUserLoginDetails"] = exForm;
        //                    return RedirectToAction("OpenSchemeForm", "Application", new { area = "Online", @id = exForm.ApplicationFormId });
        //                }
        //                else if (exForm.SchemeType == OnlineSchemeType.Transport && exForm.SchemeId == _transportSchemeId)
        //                {
        //                    Session["SchemeUserLoginDetails"] = exForm;
        //                    return RedirectToAction("TransportSchemeForm", "Application", new { area = "Online" });
        //                }
        //                else if (exForm.SchemeType == OnlineSchemeType.IndustrialScheme && exForm.SchemeId == _IndustrialSchemeId)
        //                {
        //                    Session["SchemeUserLoginDetails"] = exForm;
        //                    if (exForm.ScrutinyStatusId == 2)
        //                        return RedirectToAction("UploadDocumentScrutiny", "Application", new { area = "Online", id = exForm.EncryptedFormId });
        //                    else
        //                        return RedirectToAction("IndustrialForm", "OnlineProperty", new { area = "NIC" });
        //                }
        //                else if (exForm.SchemeType == OnlineSchemeType.OpenEnded && exForm.SchemeId == _institutionalSchemeId)
        //                {
        //                   Session["SchemeUserLoginDetails"] = exForm;
        //                   return RedirectToAction("InstitutionalSchemeForm", "Application", new { area = "Online"});
                          
        //                }
        //                else
        //                {
        //                    TempData["ErrorLoginMessage"] = "Please Login through appropriate scheme.";
        //                    return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
        //                }
        //            }
        //            else if (flag == ReturnType.UserNameNotExist)
        //            {
        //                TempData["ErrorLoginMessage"] = "UserName/FormID is not correct."; return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
        //            }
        //            else if (flag == ReturnType.PasswordNotExist)
        //            {
        //                TempData["ErrorLoginMessage"] = "Password is not correct."; return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
        //            }
        //            else if (flag == ReturnType.Failure)
        //            {
        //                TempData["ErrorLoginMessage"] = "Form not validated."; return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
        //            }
        //        }
        //    }
        //    return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
        //}

        [AllowAnonymous]
        public ActionResult SchemeLogOut()
        {
            ClearSessionVariables();
            return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
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
        public ActionResult ChangePassword()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(PasswordModel pmodel)
        {
            int flag = 0;
            var SchemeUser = (CurrentSchemeUserDetail)Session["SchemeUserLoginDetails"];
            OnlineFormViewModel objOnlineFormViewModel = new OnlineFormViewModel();
            objOnlineFormViewModel.ApplicationFormId = SchemeUser.ApplicationFormId;
            objOnlineFormViewModel.UserPassword = pmodel.OldPassword;
            flag = _onlineService.ValidateApplicationDetails(objOnlineFormViewModel);
            if (flag > 0)
            {
                if (pmodel.OldPassword.Equals(pmodel.NewPassword))
                {
                    ModelState.AddModelError("PasswordMessage", " Old and new password is same, please change new password.");
                    return View("ChangePassword", pmodel);
                }
                else
                {
                    //Match npassword = Regex.Match(pmodel.NewPassword, @"^((?=.*\d)(?=.*[A-Z])(?=.*\W).{8,255})$");
                    //if (npassword.Success)
                    //{
                    if (pmodel.NewPassword.Equals(pmodel.ConfirmNewPassword))
                    {
                        bool data = _onlineService.ChangePassword((int)SchemeUser.ApplicationFormId, SchemeUser.Email.ToString(), pmodel.NewPassword);
                        if (data == true)
                        {
                            ModelState.AddModelError("PasswordSuccessMessage", " Password has been changed successfully.");
                            return View("ChangePassword", pmodel);
                        }
                        else
                        {
                            ModelState.AddModelError("PasswordMessage", " Password is not changed.");
                            return View("ChangePassword", pmodel);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("PasswordMessage", " New password and confirm new password missmatch");
                        return View("ChangePassword", pmodel);
                    }
                    //}
                    //else
                    //{
                    //    ModelState.AddModelError("PasswordMessage", " Password must have atleast one upper case, lower case and a special character");
                    //    return View("ChangePassword", pmodel);
                    //}
                }
            }
            else
            {
                ModelState.AddModelError("PasswordMessage", "Old password is not correct.");
                return View("ChangePassword", pmodel);
            }
        }

        [AllowAnonymous]
        public JsonResult ValidateForgotPassword(OnlineFormViewModel model)
        {
            int flag = 0;
            if (model.ApplicationFormId != null && !string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.MobileNumber))
            {
                flag = _onlineService.ValidateApplicationDetailsforForgotPassword(model);
                TempData["PasswordMessage"] = flag;
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword(OnlineFormViewModel model)
        {
            return View();
        }

        #endregion



        #endregion

        #region NIC Service

        //[HttpPost]
        //private int GetWGetBasicDetails(WBasicDetailsPostModel objWBasicDetailsPostModel)
        //{
        //    int flag = ReturnType.Failed;
        //    if (objWBasicDetailsPostModel.TxtControlID != null && objWBasicDetailsPostModel.TxtUnitID != null && objWBasicDetailsPostModel.TxtServiceID != null)
        //    {
        //        Session["WBasicDetailsNIC"] = null;
        //        string path = string.Empty;
        //        string xmlInputData = string.Empty;
        //        string xmlOutputData = string.Empty;

        //        using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
        //        {
        //            string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
        //            System.Data.DataSet result = client.WGetBasicDetails(objWBasicDetailsPostModel.TxtControlID, objWBasicDetailsPostModel.TxtUnitID, objWBasicDetailsPostModel.TxtServiceID, objWBasicDetailsPostModel.TxtProcessIndustryID, Passalt);
        //            //null checked by Shatrughna on 26-May-2018
        //            if (result != null)
        //            {
        //                xmlInputData = result.GetXml();
        //                Deserial objHelp = new Deserial();
        //                NewDataSet objNewDataSet = objHelp.Deserialize<NewDataSet>(xmlInputData);
        //                if (objNewDataSet.Table != null)
        //                {
        //                    objNewDataSet.Table.ServiceID = objWBasicDetailsPostModel.TxtServiceID;
        //                    Session["WBasicDetailsNIC"] = objNewDataSet;
        //                    return flag = ReturnType.Saved;
        //                }
        //                else
        //                {
        //                    return flag = ReturnType.Failed;
        //                }
        //            }
                    
        //        }
        //    }
        //    return flag;
        //}


        [HttpPost]
        private OnlineFormViewModel GetWGetBasicDetails(WBasicDetailsPostModel apimodel)
        {
            int flag = ReturnType.Failed;
            OnlineFormViewModel nicmodel = new OnlineFormViewModel();
            if (apimodel.TxtControlID != null && apimodel.TxtUnitID != null && apimodel.TxtServiceID != null)
            {
                Session["WBasicDetailsNIC"] = null;
                string path = string.Empty;
                string xmlInputData = string.Empty;
                string xmlOutputData = string.Empty;

                using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
                {
                    string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
                    System.Data.DataSet result = client.WGetBasicDetails(apimodel.TxtControlID, apimodel.TxtUnitID, apimodel.TxtServiceID, apimodel.TxtProcessIndustryID, Passalt);
                    //null checked by Shatrughna on 26-May-2018
                    if (result != null)
                    {
                        xmlInputData = result.GetXml();
                        Deserial objHelp = new Deserial();
                        NewDataSet objNewDataSet = objHelp.Deserialize<NewDataSet>(xmlInputData);
                        if (objNewDataSet.Table != null)
                        {
                            objNewDataSet.Table.ServiceID = apimodel.TxtServiceID;
                            Session["WBasicDetailsNIC"] = objNewDataSet;
                            nicmodel.BasicDetailsGetModel = objNewDataSet;
                            nicmodel.FlagId = ReturnType.Saved;
                            nicmodel.IsFromNIC = true;
                            //return flag = ReturnType.Saved;
                            return nicmodel;
                        }
                        else
                        {
                            //return flag = ReturnType.Failed;
                            nicmodel.FlagId = ReturnType.Failed;
                            return nicmodel;
                        }
                    }
                    else
                    {
                        nicmodel.FlagId = ReturnType.NotExist;
                        return nicmodel;
                    }
                }
            }
            return nicmodel;
        }

        [HttpPost]
        private OnlineFormViewModel GetNICPostedBasicDetails(WBasicDetailsPostModel apimodel)
        {
            OnlineFormViewModel nicmodel = new OnlineFormViewModel();
            if (apimodel.TxtControlID != null && apimodel.TxtUnitID != null && apimodel.TxtServiceID != null)
            {
                Session["WBasicDetailsNIC"] = null;
                //string path = string.Empty;
                string xmlInputData = string.Empty;
                //string xmlOutputData = string.Empty;
                nicmodel.NICControlId = apimodel.TxtControlID;
                nicmodel.NICUnitId = apimodel.TxtUnitID;
                nicmodel.NICServiceId = apimodel.TxtServiceID;
                nicmodel.NICProcessIndustryId = apimodel.TxtProcessIndustryID;
                nicmodel.NICApplicationId = apimodel.TxtApplicationID;
                nicmodel.AppType = Constants.NIC;
                nicmodel.IsFromNIC = true;
                using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
                {
                    string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
                    System.Data.DataSet result = client.WGetBasicDetails(apimodel.TxtControlID, apimodel.TxtUnitID, apimodel.TxtServiceID, apimodel.TxtProcessIndustryID, Passalt);
                    //null checked by Shatrughna on 26-May-2018
                    if (result != null)
                    {
                        xmlInputData = result.GetXml();
                        nicmodel.NICXmlInputTable = xmlInputData;
                        Deserial xmlSerializer = new Deserial();
                        NewDataSet naDataSet = xmlSerializer.Deserialize<NewDataSet>(xmlInputData);
                        if (naDataSet.Table != null)
                        {
                            naDataSet.Table.ServiceID = apimodel.TxtServiceID;
                            Session["WBasicDetailsNIC"] = naDataSet;
                            nicmodel.BasicDetailsGetModel = naDataSet;
                            nicmodel.FlagId = ReturnType.Saved;
                            
                            return nicmodel;
                        }
                        else
                        {
                            nicmodel.FlagId = ReturnType.Failed;
                            return nicmodel;
                        }
                    }
                    else
                    {
                        nicmodel.FlagId = ReturnType.NotExist;
                        return nicmodel;
                    }
                }
            }
            else
            {
                nicmodel.FlagId = ReturnType.NotExist;
            }
            return nicmodel;
        }

        public string GetWReturn_CUSID_STATUS(WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel)
        {
            //objWReturn_CUSID_STATUSModel.ProcessIndustryID = string.Empty;
            //objWReturn_CUSID_STATUSModel.Fee_Status = string.Empty;
            objWReturn_CUSID_STATUSModel.Transaction_ID = string.Empty;
            objWReturn_CUSID_STATUSModel.Transaction_Date = string.Empty;
            objWReturn_CUSID_STATUSModel.Transaction_Date_Time = string.Empty;
            objWReturn_CUSID_STATUSModel.NOC_Certificate_Number = string.Empty;
            objWReturn_CUSID_STATUSModel.NOC_URL = string.Empty;
            objWReturn_CUSID_STATUSModel.ISNOC_URL_ActiveYesNO = string.Empty;
            string path = string.Empty;
            string xmlInputData = string.Empty;
            string xmlOutputData = string.Empty;
            using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
            {
                string result = client.WReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel.ControlID, objWReturn_CUSID_STATUSModel.UnitID, objWReturn_CUSID_STATUSModel.ServiceID, objWReturn_CUSID_STATUSModel.ProcessIndustryID, objWReturn_CUSID_STATUSModel.ApplicationID, objWReturn_CUSID_STATUSModel.Status_Code, objWReturn_CUSID_STATUSModel.Remarks, objWReturn_CUSID_STATUSModel.Fee_Amount, objWReturn_CUSID_STATUSModel.Fee_Status, objWReturn_CUSID_STATUSModel.Transaction_ID, objWReturn_CUSID_STATUSModel.Transaction_Date, objWReturn_CUSID_STATUSModel.Transaction_Date_Time, objWReturn_CUSID_STATUSModel.NOC_Certificate_Number, objWReturn_CUSID_STATUSModel.NOC_URL, objWReturn_CUSID_STATUSModel.ISNOC_URL_ActiveYesNO, objWReturn_CUSID_STATUSModel.passsalt);
                return result;
            }
        }

        #endregion

        [AllowAnonymous]
        public JsonResult GetPaymentStatusList()
        {
            var list = _onlineService.GetPaymentStatusList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult NICSingleWindow(WBasicDetailsPostModel model)
        {
            OnlineFormViewModel nicmodel = new OnlineFormViewModel();
            if (Session["SchemeUserLoginDetails"] != null)
            {
                CurrentSchemeUserDetail applicant = (CurrentSchemeUserDetail)Session["SchemeUserLoginDetails"];
                if (applicant.SchemeType == OnlineSchemeType.Transport) { return RedirectToAction("TransportSchemeForm", "Application", new { area = "Online" }); }
                else if (applicant.SchemeType == OnlineSchemeType.OpenEnded) { return RedirectToAction("OpenSchemeForm", "Application", new { area = "Online" }); }
            }
            else
            {
                //Check request type (like post,get..)
                if (System.Web.HttpContext.Current.Request.HttpMethod == HttpMethodType.POST)
                {
                    if (string.IsNullOrEmpty(model.TxtApplicationID))
                    {
                        if (model.TxtControlID != null && model.TxtUnitID != null && model.TxtServiceID != null)
                        {
                          nicmodel =  GetWGetBasicDetails(model);
                          nicmodel.NICControlId = model.TxtControlID;
                          nicmodel.NICUnitId = model.TxtUnitID;
                          nicmodel.NICServiceId = model.TxtServiceID;
                          nicmodel.NICProcessIndustryId = model.TxtProcessIndustryID;
                          nicmodel.NICApplicationId = model.TxtApplicationID;
                        }
                    }
                    else
                    {
                        OnlineFormViewModel formModel = new OnlineFormViewModel();
                        int AppId = 0;
                        try { AppId = Int32.Parse(model.TxtApplicationID); }
                        catch (Exception ex) { AppId = 0; }
                        if (AppId > 0)
                        {
                            formModel = _onlineService.GetOnlineSchemeFormById(AppId);
                            if (formModel != null)
                            {
                                SchemeLoginModel loginModel = new SchemeLoginModel();
                                loginModel.txtUserName = model.TxtApplicationID;
                                loginModel.txtPassword = formModel.UserPassword.ToMD5HashForPasswordPIS();
                                loginModel.SchemeType = formModel.SchemeType;
                                loginModel.AppType = Constants.AppType;
                                if (loginModel.txtUserName != null && loginModel.txtPassword != null)
                                {
                                    return SchemeLogin(loginModel);
                                }
                            }
                            else
                            {
                                TempData["ServiceMessage"] = "Application ID not exists.";
                                return View(nicmodel);
                            }
                        }
                        else
                        {
                            TempData["ServiceMessage"] = "Service Failure.";
                            return View(nicmodel);
                        }
                    }
                }
            }
            Counter();//Generate user session and update the counter by 1.
            return View(nicmodel);
        }

        [AllowAnonymous]
        public JsonResult AddToAllotment(string FormId)
        {
            int _FormId = 0;
            int FId = 0;
            if (int.TryParse(Convert.ToString(FormId), out _FormId))
                FId = _FormId;

            var data = _onlineService.SaveApplicationDetailForAllotment(FId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // use for pdf letter by passing html content or url
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult AuthorityLetter(string htmlContent, string htmlUrl)
        {
            var htmlToPdf = new HtmlToPdfConverter();
            var pdfContentType = "application/pdf";
            if (!String.IsNullOrEmpty(htmlUrl))
            {
                return File(htmlToPdf.GeneratePdfFromFile(htmlUrl, null), pdfContentType);
            }
            else
            {
                return File(htmlToPdf.GeneratePdf(htmlContent, null), pdfContentType);
            }
        }

        [AllowAnonymous]
        public ActionResult NOCLetter(string barcode)
        {
            var data = GetLetterByBarcode(barcode);
            //return View(data);
            string htmlUrl = string.Empty;
            var htmlContent = data.LetterContent;
            //var htmlContent = String.Format("<body>Hello world: {0}</body>",DateTime.Now);
            var htmlToPdf = new HtmlToPdfConverter();
            var pdfContentType = "application/pdf";
            if (!String.IsNullOrEmpty(htmlUrl))
            {
                return File(htmlToPdf.GeneratePdfFromFile(htmlUrl, null), pdfContentType);
            }
            else
            {
                return File(htmlToPdf.GeneratePdf(htmlContent, null), pdfContentType);
            }
        }

        [AllowAnonymous]
        private LetterViewModel GetLetterByBarcode(string barcode)
        {
            var data = _onlineService.GetLetterByBarcode(barcode);
            return data;
        }

        [AllowAnonymous]
        public ActionResult SchemeDraw()
        {
            return View();
        }

        [AllowAnonymous]
        public JsonResult GetApplicantListAfterDraw([DataSourceRequest] DataSourceRequest request, OnlineFormViewModel modal)
        {
            var applications = _onlineService.GetApplicantListAfterDraw(request, modal);
            return Json(applications, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetApplicantDetailForDraw([DataSourceRequest] DataSourceRequest request, OnlineFormViewModel modal)
        {
            var applications = _onlineService.GetApplicantDetailForDraw(request, modal);
            return Json(applications, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetOnlineApplicationFormIdList([DataSourceRequest] DataSourceRequest request, int? schemeId, int? departmentId)
        {
            var departments = _onlineService.GetOnlineApplicationFormIdList(request, schemeId, departmentId);
            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult AllotPropertyAfterDraw(OnlineFormViewModel model)
        {
            var flag = _onlineService.AllotPropertyAfterDraw(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult UpdateAllottedPropertyAfterDraw(int? formId, string actionType)
        {
            var flag = _onlineService.UpdateAllottedPropertyAfterDraw(formId, actionType);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ValidatePropertyForAllotment(string sector, string block, string plot)
        {
            var flag = _onlineService.ValidatePropertyForAllotment(sector, block, plot);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateApplicationProcess(OnlineFormViewModel objOnlineFormViewModel)
        {
            ResultMessage resultmessage = new ResultMessage();
            resultmessage = _onlineService.SaveApplicationProcessRequest(objOnlineFormViewModel);

            int EStatus = (int)Enum.Parse(typeof(OnlineApplicationProcess), objOnlineFormViewModel.ProcessType);
            string EnumStatus = Convert.ToString(EStatus);
            EnumStatus = (!string.IsNullOrEmpty(EnumStatus) ? (EnumStatus == strOnlineApplicationProcess.MoveToOSD ? "File Move To OSD" : (EnumStatus == strOnlineApplicationProcess.Scrutiny ? "File goes for Scrutiny" : (EnumStatus == strOnlineApplicationProcess.ApprovalCEO ? "Approval to CEO" : (EnumStatus == strOnlineApplicationProcess.Draw ? "File gor for raw" : string.Empty)))) : string.Empty) + " - " + ServiceStatus_Text.INPROCESS;

            SendRemarktoNIC(resultmessage, ServiceStatus.INPROCESS, EnumStatus);//NIC Service
            return Json(resultmessage, JsonRequestBehavior.AllowGet);
        }

        private void SendRemarktoNIC(ResultMessage resultmessage, string Process, string Remarks)
        {
            var OnlineAppDetails = resultmessage.clsResultType.Where(m => m.PrimaryKey > 0).ToList();
            if (OnlineAppDetails.Count > 0)
            {
                for (int i = 0; i < OnlineAppDetails.Count(); i++)
                {
                    if (OnlineAppDetails[i].ReturnType == 201)
                    {
                        int AppId = OnlineAppDetails[i].PrimaryKey;
                        OnlineFormViewModel objOViewModel = new OnlineFormViewModel();
                        objOViewModel.ApplicationFormId = AppId;
                        var data = _onlineService.GetNICSingleWindowData(objOViewModel);
                        if (data != null)
                        {
                            WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel();
                            objWReturn_CUSID_STATUSModel.ControlID = data.Control_ID;
                            objWReturn_CUSID_STATUSModel.ApplicationID = Convert.ToString(objOViewModel.ApplicationFormId);
                            objWReturn_CUSID_STATUSModel.ProcessIndustryID = Convert.ToString(objOViewModel.ApplicationFormId);
                            objWReturn_CUSID_STATUSModel.UnitID = data.Unit_Id;
                            objWReturn_CUSID_STATUSModel.ServiceID = data.ServiceID;
                            objWReturn_CUSID_STATUSModel.Status_Code = Process;
                            objWReturn_CUSID_STATUSModel.Remarks = Remarks;
                            objWReturn_CUSID_STATUSModel.Fee_Status = string.Empty;
                            objWReturn_CUSID_STATUSModel.Fee_Amount = string.Empty;
                            objWReturn_CUSID_STATUSModel.passsalt = Passalt;

                            NiveshMitraServices niveshmitraservices = new NiveshMitraServices();
                            string message = niveshmitraservices.GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);
                        }
                    }
                }
            }
        }

        public ActionResult OnlineApplicationDetailRequests()
        {
            return View();
        }

        public ActionResult OnlineApplicationDetailProcess()
        {
            return View();
        }

        public JsonResult GetOnlineApplicationProcess([DataSourceRequest] DataSourceRequest request)
        {
            var applications = _onlineService.GetOnlineApplicationProcess(request);
            return Json(applications, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlineApplicationProcessRequests([DataSourceRequest] DataSourceRequest request)
        {
            var applications = _onlineService.GetOnlineApplicationProcessRequests(request);
            return Json(applications, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateOnlineApplicationStatus(OnlineApplicationDetailProcess objOnlineApplicationDetailProcess)
        {
            string Status = string.Empty;
            string Remark = string.Empty;
            ResultMessage resultmessage = new ResultMessage();
            if (objOnlineApplicationDetailProcess.StatusType == 1) { Status = ServiceStatus.APPROVED; Remark = "File Approved."; } else { Status = ServiceStatus.REJECTED; Remark = "File Rejected."; }
            resultmessage = _onlineService.UpdateOnlineApplicationStatus(objOnlineApplicationDetailProcess);
            SendRemarktoNIC(resultmessage, Status, Remark);//NIC Service
            return Json(resultmessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyDetailAsDataSourceByPropertyId([DataSourceRequest] DataSourceRequest request, int? rid)
        {
            var data = _onlineService.GetPropertyDetailAsDataSourceByPropertyId(request, rid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApplicationDetailAsDataSourceByApplicationId([DataSourceRequest] DataSourceRequest request, int? applicationId)
        {
            var data = _onlineService.GetApplicationDetailAsDataSourceByApplicationId(request, applicationId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PropertyAllotmentForOnlineApplicationForm(OnlineFormViewModel model)
        {
            int flag = _allotmentService.PropertyAllotmentForOnlineApplicationForm(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidatePropertyAndApplicationForm(OnlineFormViewModel model)
        {
            int flag = _onlineService.ValidatePropertyAndApplicationForm(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllottedOnlineFormPropertyList([DataSourceRequest] DataSourceRequest request, OnlineFormViewModel model)
        {
            var data = _onlineService.GetAllottedOnlineFormPropertyList(request, model);
            return Json(data,JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDocumentListForOnlineSchemeForm([DataSourceRequest]DataSourceRequest request, OnlineFormViewModel model)
        {
            var allDocs = _onlineService.GetDocumentListForOnlineSchemeForm(request, model);
            return Json(allDocs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageCheckListDocument()
        {
            return View();
        }

        public JsonResult GetOnlineChecklistDocument([DataSourceRequest]DataSourceRequest request, OnlineDocumentViewModel model)
        {
            var list = _onlineService.GetOnlineChecklistDocument(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChecklistTypeList([DataSourceRequest]DataSourceRequest request, OnlineDocumentViewModel model)
        {
            var list = _onlineService.GetChecklistTypeList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetDepartmentListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult ValidateOESpaymentFromNIC(OnlineFormViewModel model)
        {
            var flag = false;
            string servicePassalt = ConfigurationManager.AppSettings["ServicePassalt"];
            System.Data.DataSet _newDataSet = new System.Data.DataSet();
            NewDataSet naDataSet = new NewDataSet();
            try
            {
                string path = string.Empty;
                string xmlInputData = string.Empty;
                string xmlOutputData = string.Empty;
                using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
                {
                    _newDataSet = client.WGetUBPaymentDetails(model.NICControlId, model.NICUnitId, model.NICServiceId, servicePassalt);
                    if (_newDataSet != null)
                    {
                        xmlInputData = _newDataSet.GetXml();
                        Deserial objHelp = new Deserial();
                        naDataSet = objHelp.Deserialize<NewDataSet>(xmlInputData);
                        if (naDataSet.Table != null)
                        {
                            if (naDataSet.Table.Status_Code == Common.ServiceStatus.FEE_PAID)
                            {
                                var result = _onlineService.UpdateOESFormPaymentStatus(model);
                                flag = result.NICFeeStatusId == "11" ? true : false;

                                if (flag)
                                {
                                    // fee status
                                    NiveshMitraServices _niveshMitraServices = new NiveshMitraServices();
                                    WReturn_CUSID_STATUSModel stmodel = new WReturn_CUSID_STATUSModel();
                                    stmodel.ControlID = model.NICControlId;
                                    stmodel.UnitID = model.NICUnitId;
                                    stmodel.ServiceID = model.NICServiceId;
                                    stmodel.ProcessIndustryID = model.NICProcessIndustryId; //niveshMitraDetails.ProcessIndustryID;
                                    stmodel.Status_Code = Common.ServiceStatus.SAVE_AS_DRAFT;
                                    //stmodel.Remarks = ServiceStatus_Text.FEE_PAID;
                                    //stmodel.Fee_Amount = objNewDataSet.Table.Fee_Amount;
                                    //stmodel.Fee_Status = PaymentStatus_NIC.PAID;

                                    _niveshMitraServices.GetWReturn_CUSID_STATUS(stmodel);
                                }
                            }
                        }
                    }
                    
                }
                //return flag;
            }
            catch (Exception ex)
            {
                //return objNewDataSet;
                throw ex;
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SaveProjectAndRefundDetailForOpenEndScheme(OnlineFormViewModel model)
        {
            model = _onlineService.SaveProjectAndRefundDetailForOpenEndScheme(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult BrochureSchemeForm()
        {
            Session["TempCompanyDirectors"] = null;
            OnlineFormViewModel model = GetSchemeData(OnlineSchemeType.Institutional);
            model.DirectorModel = new onlineDirectorViewModel();
            model.ProposedModel = new ProposedCompanyViewModel();
            if (Session["SchemeUserLoginDetails"] != null)
            {
                CurrentSchemeUserDetail objCurrentSchemeUserDetail = new CurrentSchemeUserDetail();
                objCurrentSchemeUserDetail = (CurrentSchemeUserDetail)Session["SchemeUserLoginDetails"];
                var data = _onlineService.GetOnlineApplicationFormById(objCurrentSchemeUserDetail.ApplicationFormId);

                //check applicant status like online payment, genearated challan or uploaded previous challan.
                if (data.IsApplicationFeePaid != true && data.IsChallanGenerated != true && data.IsPreviousChallanUploaded != true && data.PaidThroughSWP != true)
                {
                    return View(data);
                }
                else
                {
                    return RedirectToAction("BrochurePreviewForm", "Application", new { area = "Online", id = CommonHelper.Encode(data.ApplicationFormId.ToString()) });
                }
            }

            if (Session["SchemeUserLoginDetails"] == null && Session["WBasicDetailsNIC"] != null)
            {
                NewDataSet objNewDataSet = (NewDataSet)Session["WBasicDetailsNIC"];
                model.ApplicantType = "Company";
                model.Applicant = objNewDataSet.Table.Company_Name;
                model.SigningAuthority = objNewDataSet.Table.Occupier_Name;
                model.RefundInfaverof = objNewDataSet.Table.Occupier_Name;
                //model.PanNumber = objNewDataSet.Table.Occupier_PAN;
                // model.DOB = Convert.ToDateTime(objNewDataSet.Table.Occupier_DOB);
                //model.ApplicantMaster = objNewDataSet.Table.Occupier_Father_Mother_Name;
                model.MobileNumber = objNewDataSet.Table.Occupier_Mobile_No;
                model.Email = objNewDataSet.Table.Occupier_Email_ID;
                model.AnnualIncome = objNewDataSet.Table.Annual_Turnover;
                model.CorrespondingAddress = objNewDataSet.Table.Occupier_District_Name + " " + objNewDataSet.Table.Occupier_Address + " " + objNewDataSet.Table.Occupier_Pin_Code;
                model.PermanentAddress = objNewDataSet.Table.Occupier_District_Name + " " + objNewDataSet.Table.Occupier_Address + " " + objNewDataSet.Table.Occupier_Pin_Code;
                return View(model);
            }
            return View(model);
        }

        // added by MTC
        [HttpPost]
        [AllowAnonymous]
        public ActionResult BrochureSchemeForm(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            model.SchemeType = OnlineSchemeType.Institutional;
            //setting default values to download the browchure
            model.ProcessingCharge = 0;
            model.EarnestMoney = 0;
            model.ProcessingChargeGST = 0;
            //setting default values to download the browchure

            Session["SchemeType"] = model.SchemeType;
            Session["SchemeType"] = model.SchemeType;
            int flag = _onlineService.SaveOnlineApplicationForm(model, files, userImage, signatureImage);
            if (flag > 0)
            {
                return RedirectToAction("BrochurePreviewForm", "Application", new { area = "Online", id = CommonHelper.Encode(flag.ToString()) });
            }
            else
            {
                TempData["ServiceMessage"] = "Failure";
                return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
            }
        }

        // added by MTC
        [AllowAnonymous]
        public ActionResult BrochureDownloadForm(int id)
        {
            var model = _onlineService.GetOnlineApplicationFormById(id);
            return View(model);
        }

        // added by MTC
        [AllowAnonymous]
        public ActionResult InstitutionalSchemeForm()
        {


            Session["TempCompanyDirectors"] = null;
            OnlineFormViewModel model = GetSchemeData(OnlineSchemeType.OpenEnded);
            model.DirectorModel = new onlineDirectorViewModel();
            model.ProposedModel = new ProposedCompanyViewModel();
            if (Session["SchemeUserLoginDetails"] != null)
            {
                //CurrentSchemeUserDetail objCurrentSchemeUserDetail = new CurrentSchemeUserDetail();
                var _ExForm = (OnlineFormViewModel)Session["SchemeUserLoginDetails"];
                var data = _onlineService.GetOnlineApplicationFormById(_ExForm.ApplicationFormId);
              
                //var data = _onlineService.GetOnlineApplicationFormById(objCurrentSchemeUserDetail.ApplicationFormId);

                //check applicant status like online payment, genearated challan or uploaded previous challan.
                if (data.IsApplicationFeePaid != true && data.IsChallanGenerated != true && data.IsPreviousChallanUploaded != true && data.PaidThroughSWP != true)
                {
                    data.TotalAmount = (data.ApplicationFee + data.FormFeeGST) - data.TotalAmount;
                    return View(data);
                }
                else
                {
                    return RedirectToAction("PreviewForm", "Application", new { area = "Online", id = CommonHelper.Encode(data.ApplicationFormId.ToString()) });
                }
            }

            if (Session["SchemeUserLoginDetails"] == null && Session["WBasicDetailsNIC"] != null)
            {
                NewDataSet objNewDataSet = (NewDataSet)Session["WBasicDetailsNIC"];
                model.ApplicantType = "Company";
                model.Applicant = objNewDataSet.Table.Company_Name;
                model.SigningAuthority = objNewDataSet.Table.Occupier_Name;
                model.RefundInfaverof = objNewDataSet.Table.Occupier_Name;
                //model.PanNumber = objNewDataSet.Table.Occupier_PAN;
                // model.DOB = Convert.ToDateTime(objNewDataSet.Table.Occupier_DOB);
                //model.ApplicantMaster = objNewDataSet.Table.Occupier_Father_Mother_Name;
                model.MobileNumber = objNewDataSet.Table.Occupier_Mobile_No;
                model.Email = objNewDataSet.Table.Occupier_Email_ID;
                model.AnnualIncome = objNewDataSet.Table.Annual_Turnover;
                model.CorrespondingAddress = objNewDataSet.Table.Occupier_District_Name + " " + objNewDataSet.Table.Occupier_Address + " " + objNewDataSet.Table.Occupier_Pin_Code;
                model.PermanentAddress = objNewDataSet.Table.Occupier_District_Name + " " + objNewDataSet.Table.Occupier_Address + " " + objNewDataSet.Table.Occupier_Pin_Code;
                return View(model);
            }
            return View(model);
        }

        // added by MTC
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterInstitutionalSchemeForm(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            model.SchemeType = OnlineSchemeType.OpenEnded;
            Session["SchemeType"] = model.SchemeType;
            int flag = 0;
            if (Session["SchemeUserLoginDetails"] == null)
            {
                if (Session["WBasicDetailsNIC"] != null) { model.BasicDetailsGetModel = (NewDataSet)Session["WBasicDetailsNIC"]; }
                flag = _onlineService.SaveOnlineApplicationForm(model, files, userImage, signatureImage);
                if (flag > 0)
                {
                    if (Session["WBasicDetailsNIC"] != null)
                    {
                        string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
                        WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel();
                        objWReturn_CUSID_STATUSModel.ControlID = model.BasicDetailsGetModel.Table.Control_ID;
                        objWReturn_CUSID_STATUSModel.ApplicationID = Convert.ToString(flag);
                        objWReturn_CUSID_STATUSModel.ProcessIndustryID = Convert.ToString(flag);
                        objWReturn_CUSID_STATUSModel.UnitID = model.BasicDetailsGetModel.Table.Unit_Id;
                        objWReturn_CUSID_STATUSModel.ServiceID = model.BasicDetailsGetModel.Table.ServiceID;
                        objWReturn_CUSID_STATUSModel.Status_Code = ServiceStatus.SAVE_AS_DRAFT;
                        objWReturn_CUSID_STATUSModel.Remarks = "SAVE AS DRAFT";
                        objWReturn_CUSID_STATUSModel.Fee_Amount = Convert.ToString(model.TotalAmount);
                        objWReturn_CUSID_STATUSModel.passsalt = Passalt;
                        string message = GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);
                        if (message == "Failure")
                        {
                            TempData["ServiceMessage"] = "Failure";
                            return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                        }
                    }
                }
                else
                {
                    TempData["ServiceMessage"] = "Failure";
                    return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                }
            }
            else
            {
                var _ExForm = (OnlineFormViewModel)Session["SchemeUserLoginDetails"];
                var data = _onlineService.GetOnlineApplicationFormById(_ExForm.ApplicationFormId);
                if (data.ApplicationFormId == model.ApplicationFormId)
                {
                    flag = _onlineService.UpdateOnlineApplicationForm(model, files, userImage, signatureImage);
                }
                else { RedirectToAction("TransportSchemeForm", new { id = CommonHelper.Encode(flag.ToString()) }); }
            }
            return RedirectToAction("OpenSchemeDocument", new { id = CommonHelper.Encode(flag.ToString()) });
        }




        [AllowAnonymous]
        public ActionResult BrochurePreviewForm(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int ID = Convert.ToInt32(CommonHelper.Decode(id));
                if (ID > 0)
                {
                    OnlineFormViewModel model = new OnlineFormViewModel();
                    model = _onlineService.GetOnlineApplicationFormById(ID);
                    if (!string.IsNullOrEmpty((string)Session["SchemeType"]))
                    {
                        model.SchemeType = (string)Session["SchemeType"];
                    }
                    ////if scheme end date exceed from current date returns true.
                    //if (CheckSchemeCredentials())
                    //{
                    //    if (model.IsApplicationFeePaid == true)
                    //    {
                    //        model.BankModel = _generalService.GetBankListBySchemeId((int)model.SchemeId);
                    //        return View(model);
                    //    }
                    //    else { return RedirectToAction("ErrorPage"); }
                    //}

                    model.BankModel = _generalService.GetBankListBySchemeId((int)model.SchemeId);
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
    }
}
