using Kendo.Mvc.UI;
//using NA.PMS.Common;
//using NA.PMS.Model;
using NA.PMS.NICService.NiveshMitraApiServices;
using NA.PMS.NICServices;
//using NA.PMS.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NA.PMS.Web.Areas.NIC.Controllers
{
    [AllowAnonymous]
    public class OnlinePropertyController : Controller
    {
        //public string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
        //private INICRequestService _nicService;
        //private INICFormService _nicFormService;
        //private NiveshMitraHelper _nmHelper;
        //private TemplateParserService _templateParserService;
        //public NICSuperController()
        //{
        //    _nicFormService = new NICFormService();
        //    _nicService = new NICRequestService();
        //    _nmHelper = new NiveshMitraHelper();
        //    _templateParserService = new TemplateParserService();
        //    //_onlineService = onlineService;
        //}

        //#region e-pass
        //public ActionResult DemoTest()
        //{
        //    return Content("Shatrughna u r winner!");
        //}

        //[AllowAnonymous]
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //[AllowAnonymous]
        //public ActionResult SearchEpass()
        //{
        //    return View();
        //}

        //public ActionResult ViewEpass(string passno)
        //{
        //    return View();
        //}

        //public ActionResult ManageOnlineEpass()
        //{
        //    return View();
        //}
        //public JsonResult GetAllEpassList([DataSourceRequest]DataSourceRequest request, SWPEpassViewModel model)
        //{
        //    var list = _nicFormService.GetAllEpassList(request, model);
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public ActionResult SaveEpassForm(SWPEpassViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase idfile, HttpPostedFileBase rcfile, HttpPostedFileBase dlfile)
        //{
        //    model = _nicFormService.SaveEpassFormDetail(model, userImage, idfile, rcfile, dlfile);
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public ActionResult GetEpassFormDetailById(SWPEpassViewModel model)
        //{
        //    model = _nicFormService.GetEpassFormDetailById(model);
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult ValidateEpass(SWPEpassViewModel model)
        //{
        //    int flag = _nicFormService.ValidateEpass(model);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}


        //[AllowAnonymous]
        //public JsonResult SendOTP(string mobileNo, string email)
        //{
        //    int flag = ApplicationHelper.GenerateOTP();
        //    Session["OTPmobile"] = flag;
        //    //int flag = 123;
        //    //Session["OTPmobile"] = flag;
        //    string message = string.Format(NAMessages.OnlineApplicationOTP, flag);
        //    //string mobileMessage = string.Format(NAMessages.OnlineApplicationOTP, flag);
        //    if (mobileNo != null && mobileNo != "") ApplicationHelper.SendSMS(mobileNo, message);
        //    if (email != null && email != "") ApplicationHelper.SendEmail(email, "Online E-Pass", message);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult EpassDetails(int? Id)
        //{
        //    SWPEpassViewModel model = new SWPEpassViewModel();
        //    if (Id != null)
        //    {
        //        model = _nicFormService.EpassDetails(Id);
        //    }
        //    return View(model);
        //}

        
        //#endregion


        //#region online scheme form
        //[AllowAnonymous]
        //public ActionResult SingleWindowPortal()
        //{
        //    return View();
        //}

        //[AllowAnonymous]
        //public ActionResult SchemeInformation(SWPPostViewModel nicdata)
        //{
        //    SWPFormViewModel naonlineform = new SWPFormViewModel();

        //    if (Session["SchemeUserLoginDetails"] != null)
        //    {
        //        //CurrentSchemeUserDetail schemeUserDetail = new CurrentSchemeUserDetail();
        //        //schemeUserDetail = (CurrentSchemeUserDetail)Session["SchemeUserLoginDetails"];
        //        //if (schemeUserDetail.SchemeType == OnlineSchemeType.IndustrialScheme)
        //        //{
        //        //    return RedirectToAction("IndustrialForm", "OnlineProperty", new { area = "NIC" });
        //        //}
        //        //else if (schemeUserDetail.SchemeType == OnlineSchemeType.OpenEnded)
        //        //{
        //        //    //return RedirectToAction("OpenSchemeForm", "Application", new { area = "Online" }); 
        //        //    return RedirectToAction("IndustrialOpenEndForm", "OnlineProperty", new { area = "NIC", @id = schemeUserDetail.ApplicationFormId });
        //        //}
        //    }
        //    else
        //    {
        //        //Check request type (like post,get..)               
        //        if (System.Web.HttpContext.Current.Request.HttpMethod == HttpMethodType.POST)
        //        {
        //            if (nicdata.TxtControlID != null && nicdata.TxtUnitID != null && nicdata.TxtServiceID != null)
        //            {
        //                //var nicdetail = GetWGetBasicDetails(nicdata);
        //                var nicdetail = GetNICPostedBasicDetails(nicdata);

        //                if (!string.IsNullOrEmpty(nicdata.TxtApplicationID))
        //                {
        //                    nicdetail.ApplicationFormId = Convert.ToInt32(nicdata.TxtApplicationID);
        //                    //naonlineform = _onlineService.GetOpenEndedSchemeFormDataById(nicdetail);
        //                    naonlineform = _nicFormService.GetOpenEndedSchemeFormDataById(nicdetail);
        //                    //naonlineform = nicdetail;
        //                    if (naonlineform.SchemeType == OnlineSchemeType.IndustrialScheme)
        //                    {
        //                        return RedirectToAction("IndustrialForm", "OnlineProperty", new { area = "NIC", @id = naonlineform.Id, @controlid = naonlineform.NICControlId, @unitid = naonlineform.NICUnitId, @serviceid = naonlineform.NICServiceId });
        //                    }
        //                    else
        //                    {
        //                        return RedirectToAction("IndustrialOpenEndForm", "OnlineProperty", new { area = "NIC", @id = naonlineform.Id, @controlid = naonlineform.NICControlId, @unitid = naonlineform.NICUnitId, @serviceid = naonlineform.NICServiceId });
        //                    }
        //                }
        //                else
        //                {
        //                    //TempData["ServiceMessage"] = "New Application Form."; //"Service Failure.";
        //                    naonlineform = nicdetail;
        //                    naonlineform.SchemeType = nicdata.SchemeType;
        //                    naonlineform = _nicFormService.GetSchemeBasicInfoData(naonlineform);
        //                }
        //            }
        //            else
        //            {
        //                //TempData["ServiceMessage"] = "New Application Form."; //"Service Failure.";
        //                naonlineform.SchemeType = nicdata.SchemeType;
        //                naonlineform = _nicFormService.GetSchemeBasicInfoData(naonlineform);
        //            }
        //        }
        //        else
        //        {
        //            naonlineform.IsFromNIC = false;
        //            naonlineform.SchemeType = nicdata.SchemeType;
        //            naonlineform = _nicFormService.GetSchemeBasicInfoData(naonlineform);
        //        }

        //    }

        //    //Counter();//Generate user session and update the counter by 1.
        //    return View(naonlineform);
        //}

        //[AllowAnonymous]
        //public ActionResult IndustrialSchemeInformation()
        //{
        //    return View();
        //}

        //[AllowAnonymous]
        //public ActionResult IndustrialOpenEndForm(int? id, string controlid, string unitid, string serviceid)
        //{
        //    SWPFormViewModel form = _nicFormService.GetSchemeBasicInfoData(new SWPFormViewModel { SchemeType = OnlineSchemeType.OpenEnded }); //GetSchemeBasicInfoData(OnlineSchemeType.OpenEnded);
        //    if (!string.IsNullOrEmpty(controlid) && !string.IsNullOrEmpty(controlid) && !string.IsNullOrEmpty(controlid))
        //    {
        //        SWPPostViewModel apidata = new SWPPostViewModel { TxtControlID = controlid, TxtServiceID = serviceid, TxtUnitID = unitid };
        //        if (id != null && id > 0)
        //        {
        //            apidata.TxtApplicationID = id.ToString();
        //            apidata.TxtProcessIndustryID = id.ToString();
        //            var nicdetail = GetNICPostedBasicDetails(apidata);
        //            nicdetail.ApplicationFormId = id;
        //            //form = _onlineService.GetOpenEndedSchemeFormDataById(nicdetail);
        //            form = _nicFormService.GetOpenEndedSchemeFormDataById(nicdetail);
        //        }
        //        else
        //        {
        //            //data from nic web service
        //            var nicdetail = GetNICPostedBasicDetails(apidata);
        //            form.NICControlId = nicdetail.NICControlId;
        //            form.NICUnitId = nicdetail.NICUnitId;
        //            form.NICServiceId = nicdetail.NICServiceId;
        //            form.NICProcessIndustryId = nicdetail.NICProcessIndustryId;
        //            form.NICApplicationId = nicdetail.NICApplicationId;
        //            form.NICXmlInputTable = nicdetail.NICXmlInputTable;
        //            form.BasicDetailsGetModel = nicdetail.BasicDetailsGetModel;
        //            form.FlagId = nicdetail.FlagId;
        //            form.IsFromNIC = nicdetail.IsFromNIC;
        //            form.AppType = Constants.NIC;
        //            form.ApplicantType = "Company";
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("IndustrialSchemeInformation");
        //    }

        //    return View(form);
        //}

        //[AllowAnonymous]
        //public ActionResult IndustrialForm(int? id, string controlid, string unitid, string serviceid)
        //{
        //    SWPFormViewModel form = _nicFormService.GetSchemeBasicInfoData(new SWPFormViewModel { SchemeType = OnlineSchemeType.IndustrialScheme }); //GetSchemeBasicInfoData(OnlineSchemeType.OpenEnded);
        //    if (!string.IsNullOrEmpty(controlid) && !string.IsNullOrEmpty(controlid) && !string.IsNullOrEmpty(controlid))
        //    {
        //        SWPPostViewModel apidata = new SWPPostViewModel { TxtControlID = controlid, TxtServiceID = serviceid, TxtUnitID = unitid };
        //        if (id != null && id > 0)
        //        {
        //            apidata.TxtApplicationID = id.ToString();
        //            apidata.TxtProcessIndustryID = id.ToString();
        //            var nicdetail = GetNICPostedBasicDetails(apidata);
        //            nicdetail.ApplicationFormId = id;
        //            //form = _onlineService.GetOpenEndedSchemeFormDataById(nicdetail);
        //            form = _nicFormService.GetOpenEndedSchemeFormDataById(nicdetail);
        //        }
        //        else
        //        {
        //            //data from nic web service
        //            var nicdetail = GetNICPostedBasicDetails(apidata);
        //            form.NICControlId = nicdetail.NICControlId;
        //            form.NICUnitId = nicdetail.NICUnitId;
        //            form.NICServiceId = nicdetail.NICServiceId;
        //            form.NICProcessIndustryId = nicdetail.NICProcessIndustryId;
        //            form.NICApplicationId = nicdetail.NICApplicationId;
        //            form.NICXmlInputTable = nicdetail.NICXmlInputTable;
        //            form.BasicDetailsGetModel = nicdetail.BasicDetailsGetModel;
        //            form.FlagId = nicdetail.FlagId;
        //            form.IsFromNIC = nicdetail.IsFromNIC;
        //            form.AppType = Constants.NIC;
        //            form.ApplicantType = "Company";
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("IndustrialSchemeInformation");
        //    }

        //    return View(form);
        //}

        //[AllowAnonymous]
        //public ActionResult IndustrialDocument(string id)
        //{
        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        int formId = Convert.ToInt32(CommonHelper.Decode(id));
        //        if (formId > 0)
        //        {
        //            SWPFormViewModel model = new SWPFormViewModel();
        //            //model = _onlineService.GetOnlineApplicationFormById(formId);
        //            model = _nicFormService.GetOpenEndedSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = formId });
        //            return View(model);
        //        }
        //        else
        //        {
        //            return RedirectToAction("IndustrialSchemeInformation");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("IndustrialSchemeInformation");
        //    }
        //}

        //[AllowAnonymous]
        //public ActionResult IndustrialProposedProject(int? id, string controlid, string unitid, string serviceid)
        //{
        //    //SWPFormViewModel form = GetSchemeBasicInfoData(OnlineSchemeType.OpenEnded);
        //    SWPFormViewModel form = _nicFormService.GetSchemeBasicInfoData(new SWPFormViewModel { ApplicationFormId = id });
        //    if (!string.IsNullOrEmpty(controlid) && !string.IsNullOrEmpty(controlid) && !string.IsNullOrEmpty(controlid))
        //    {
        //        SWPPostViewModel apidata = new SWPPostViewModel { TxtControlID = controlid, TxtServiceID = serviceid, TxtUnitID = unitid };
        //        if (id != null && id > 0)
        //        {
        //            apidata.TxtApplicationID = id.ToString();
        //            apidata.TxtProcessIndustryID = id.ToString();
        //            var nicdetail = GetNICPostedBasicDetails(apidata);
        //            nicdetail.ApplicationFormId = id;
        //            //form = _onlineService.GetOpenEndedSchemeFormDataById(nicdetail);
        //            form = _nicFormService.GetOpenEndedSchemeFormDataById(nicdetail);
        //        }
        //        else
        //        {
        //            if (form.SchemeType == OnlineSchemeType.IndustrialScheme)
        //            {
        //                return RedirectToAction("IndustrialForm", "OnlineProperty", new { area = "NIC", @id = id, @controlid = controlid, @unitid = unitid, @serviceid = serviceid });
        //            }
        //            else
        //            {
        //                return RedirectToAction("IndustrialOpenEndForm", "OnlineProperty", new { area = "NIC", @id = id, @controlid = controlid, @unitid = unitid, @serviceid = serviceid });
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("IndustrialSchemeInformation");
        //    }

        //    return View(form);
        //}

        //[AllowAnonymous]
        //[HttpGet]
        //public ActionResult IndustrialPreviewForm(string id)
        //{
        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        int formId = Convert.ToInt32(CommonHelper.Decode(id));
        //        if (formId > 0)
        //        {
        //            SWPFormViewModel model = new SWPFormViewModel();
        //            //model = _onlineService.GetOnlineApplicationFormById(formId);
        //            model = _nicFormService.GetOpenEndedSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = formId });
        //            if (!string.IsNullOrEmpty((string)Session["SchemeType"]))
        //            {
        //                model.SchemeType = (string)Session["SchemeType"];
        //            }
        //            //if (DateTime.Compare((DateTime)model.SchemeEndDate, DateTime.Now) >= 0)
        //            //{
        //            //    if (model.IsApplicationFeePaid == true)
        //            //    {
        //            //        model.BankModel = _nmHelper.GetBankListBySchemeId((int)model.SchemeId);
        //            //        return View(model);
        //            //    }
        //            //    else { return RedirectToAction("ErrorPage"); }
        //            //}
        //            model.BankModel = _nmHelper.GetBankListBySchemeId((int)model.SchemeId);
        //            return View(model);
        //        }
        //        else
        //        {
        //            return RedirectToAction("IndustrialSchemeInformation");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("IndustrialSchemeInformation");
        //    }
        //}


        //public ActionResult ManageSchemeForm()
        //{
        //    if (Session["CurrentUser"] != null)
        //    {
        //        var loginUser = (CurrentUserDetail)Session["CurrentUser"];
        //        SWPFormViewModel model = new SWPFormViewModel();
        //        model.UserId = loginUser.UserID;
        //        model.UserRoleType = loginUser.RoleMaster.RoleType;
        //        return View(model);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home", new { area = "" });
        //    }
        //}

        //public ActionResult SchemeFormDetail(string id)
        //{
        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        int ID = Convert.ToInt32(CommonHelper.Decode(id));
        //        if (ID > 0)
        //        {
        //            var model = _nicFormService.GetOnlineSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = ID });
        //            return View(model);
        //        }
        //    }
        //    return RedirectToAction("ManageSchemeForm");
        //}

        //public ActionResult ManageApplicationForm()
        //{
        //    return View();
        //}

        //[AllowAnonymous]
        //public ActionResult ManageChallan()
        //{
        //    return View();
        //}

        //[AllowAnonymous]
        //public ActionResult SchemeFormChallan()
        //{
        //    return View();
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult ScrutinyRequisite(string id)
        //{
        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        int ID = Convert.ToInt32(CommonHelper.Decode(id));
        //        if (ID > 0)
        //        {
        //            var model = _nicFormService.GetOnlineSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = ID });
        //            return View(model);
        //        }
        //        else
        //        {
        //            return RedirectToAction("SchemeInformation");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("SchemeInformation");
        //    }
        //}

        //[AllowAnonymous]
        //public JsonResult GetUploadedDocumentsAfterScrutiny(DataSourceRequest request, int? formId, int? checklistStartId, int? checklistEndId)
        //{
        //    var allDocs = _nicFormService.GetUploadedDocumentsAfterScrutiny(request, formId, checklistStartId, checklistEndId);
        //    return Json(allDocs, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult UpdateDocumentAfterScrutiny(SWPFormViewModel model, IEnumerable<HttpPostedFileBase> files)
        //{
        //    //int flag = _onlineService.UploadDocumentByFormId(model, files, null, null);
        //    model = _nicFormService.SaveUploadedDocument(model, files, null, null);
        //    if (model.ReturnTypeId != ReturnType.Failure)
        //    {
        //        TempData["Document"] = "success";
        //        return RedirectToAction("ScrutinyRequisite", new { id = model.EncryptedFormId });
        //    }
        //    else
        //    {
        //        TempData["Document"] = "failure";
        //        return RedirectToAction("ScrutinyRequisite", new { id = model.EncryptedFormId });
        //    }
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //public JsonResult SaveOpenSchemeFormDetail(SWPFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        //{
        //    model.SchemeType = OnlineSchemeType.OpenEnded;
        //    Session["SchemeType"] = model.SchemeType;
        //    //var form = _onlineService.SaveOpenEndedSchemeFormDetail(model, userImage, signatureImage);
        //    var form = _nicFormService.SaveOpenEndedSchemeFormDetail(model, userImage, signatureImage);
        //    if (form.IsFromNIC == true && form.FormStatusId == NAStatusId.Success)
        //    {
        //        //Post Request to NIC Service
        //        string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
        //        WReturn_CUSID_STATUSModel nicReturnModel = new WReturn_CUSID_STATUSModel();
        //        nicReturnModel.ControlID = model.BasicDetailsGetModel.Table.Control_ID;
        //        nicReturnModel.ApplicationID = model.ApplicationFormId.ToString(); //Convert.ToString(CommonHelper.Decode(flag));
        //        //nicReturnModel.ProcessIndustryID = model.BasicDetailsGetModel.Table.ProcessIndustryID; //Convert.ToString(CommonHelper.Decode(flag));
        //        nicReturnModel.ProcessIndustryID = model.BasicDetailsGetModel.Table.ProcessIndustryID == null ? model.ApplicationFormId.ToString() : model.BasicDetailsGetModel.Table.ProcessIndustryID;
        //        nicReturnModel.UnitID = model.BasicDetailsGetModel.Table.Unit_Id;
        //        nicReturnModel.ServiceID = model.BasicDetailsGetModel.Table.ServiceID == null ? model.NICServiceId : model.BasicDetailsGetModel.Table.ServiceID;
        //        nicReturnModel.Status_Code = ServiceStatus.FEE_PENDING; //ServiceStatus.SAVE_AS_DRAFT;
        //        //nicReturnModel.Fee_Status = ServiceStatus.FEE_PENDING;
        //        nicReturnModel.Fee_Status = PaymentStatus_NIC.UB;
        //        //nicReturnModel.Remarks = ServiceStatus_Text.SAVE_AS_DRAFT;
        //        //nicReturnModel.Fee_Amount = Convert.ToString(model.TotalAmount);
        //        nicReturnModel.Remarks = ServiceStatus_Text.FEE_PENDING;
        //        //nicReturnModel.Fee_Amount = Convert.ToString(model.FormFeeWithGST);
        //        nicReturnModel.Fee_Amount = "5900";
        //        nicReturnModel.passsalt = Passalt;
        //        //Pass Object to NIC Service
        //        string message = GetWReturn_CUSID_STATUS(nicReturnModel);
        //        if (message == "FAILED") //"Failure"
        //        {
        //            TempData["ServiceMessage"] = "NIC Service Failed";
        //        }
        //        else
        //        {
        //            TempData["ServiceMessage"] = "NIC updated successfully.";
        //        }
        //    }
        //    else
        //    {
        //        TempData["ServiceMessage"] = "Application form not saved by Authority.";
        //    }
        //    return Json(form, JsonRequestBehavior.AllowGet);
        //    //var encodedId = CommonHelper.Encode(form.ApplicationFormId.ToString());
        //    //return Json(encodedId, JsonRequestBehavior.AllowGet);
        //}

        //private SWPFormViewModel GetSchemeBasicInfoData(string schemeType)
        //{
        //    int schemeId = 0;
        //    int departmentId = 0;

        //    if (schemeType == OnlineSchemeType.IndustrialPlots) { schemeId = ConfigurationManager.AppSettings["SchemeId"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["SchemeId"]) : 0; }
        //    else if (schemeType == OnlineSchemeType.Transport) { schemeId = ConfigurationManager.AppSettings["TransportScheme"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["TransportScheme"]) : 0; }
        //    else if (schemeType == OnlineSchemeType.OpenEnded) { schemeId = ConfigurationManager.AppSettings["OpenScheme"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["OpenScheme"]) : 0; }
        //    else if (schemeType == NASchemeType.IndustriaScheme) { schemeId = ConfigurationManager.AppSettings["IndustrialSchemeId"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["IndustrialSchemeId"]) : 0; }

        //    departmentId = ConfigurationManager.AppSettings["DepartmentId"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["DepartmentId"]) : 0;

        //    SWPFormViewModel schemeInfo = new SWPFormViewModel();
        //    if (schemeType == NASchemeType.IndustriaScheme)
        //        schemeInfo = _nicFormService.GetSchemeInformationForOnlineApplication(new SWPFormViewModel { SchemeId = schemeId, DepartmentId = departmentId });
        //        //schemeInfo = _onlineService.GetSchemeInformationForOnlineApplication(new SWPFormViewModel { SchemeId = schemeId, DepartmentId = departmentId });
        //    else
        //        schemeInfo = _nicFormService.GetInitialDataForScheme(new SWPFormViewModel { SchemeId = schemeId, DepartmentId = departmentId });
        //        //schemeInfo = _onlineService.GetInitialDataForScheme(new SWPFormViewModel { SchemeId = schemeId, DepartmentId = departmentId });
        //    return schemeInfo;
        //}

        ////return true if scheme end date is less than current date.
        //[AllowAnonymous]
        //public bool CheckSchemeCredentials()
        //{
        //    bool flag = false;
        //    //var model = _onlineService.GetInitialDataForScheme();
        //    var model = _nicFormService.GetInitialDataForScheme();
        //    if (model.SchemeEndDate != null)
        //    {
        //        if (DateTime.Compare((DateTime)model.SchemeEndDate, DateTime.Now) < 0)
        //        {
        //            return flag = false; // true;
        //        }
        //    }
        //    return flag;
        //}

        //[AllowAnonymous]
        //public JsonResult GetFormTypeList()
        //{
        //    var list = _nmHelper.GetFormTypeList();
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetFormSubTypeList(string formtype)
        //{
        //    var list = _nmHelper.GetFormSubTypeList(formtype);
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetApplicantTypeList()
        //{
        //    var list = _nmHelper.GetApplicantTypeList();
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetCompanyTypeList(string typeName)
        //{
        //    var floorArea = _nmHelper.GetCompanyTypeByCategory(typeName);
        //    return Json(floorArea, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetFloorAreaList(int schemeId, int departmentId)
        //{
        //    //var floorArea = _onlineService.GetAreaRangeByDepartment(schemeId, departmentId);
        //    var floorArea = _nicFormService.GetAreaRangeByDepartment(schemeId, departmentId);
        //    return Json(floorArea, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetFloorAreaRangeList(DropdownViewModel model)
        //{
        //    var floorArea = _nicFormService.GetFloorAreaRangeList(model);
        //    return Json(floorArea, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetFloorAreaListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        //{
        //    var list = _nicFormService.GetFloorAreaListAsDataSource(request, model);
        //    //var list = _onlineService.GetFloorAreaListAsDataSource(request, model);
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetSectorList()
        //{
        //    var sectors = _nmHelper.getSectorsList();
        //    return Json(sectors, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetDirectorTypeList()
        //{
        //    var typeList = _nmHelper.GetDirectorTypeList();
        //    return Json(typeList, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetGenderList()
        //{
        //    var floorArea = _nmHelper.GetGenderList();
        //    return Json(floorArea, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetMaritalStatusList()
        //{
        //    var maritalstatus = _nmHelper.GetMaritalStatusList();
        //    return Json(maritalstatus, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetCategoryList()
        //{
        //    var categories = _nmHelper.GetCategoryList();
        //    return Json(categories, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetOccupationList()
        //{
        //    var occupations = _nmHelper.GetOccupationList();
        //    return Json(occupations, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult SaveProjectAndRefundDetailForOpenEndScheme(SWPFormViewModel model)
        //{
        //    //model = _onlineService.SaveProjectAndRefundDetailForOpenEndScheme(model);
        //    model = _nicFormService.SaveProposedProjectAndRefundDetail(model);
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetBankListforOnline()
        //{
        //    var banks = _nmHelper.GetBankList();
        //    return Json(banks, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetBankListBySchemeId(int schemeId)
        //{
        //    var banks = _nmHelper.GetBankListBySchemeId(schemeId);
        //    return Json(banks, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetBankAccountDetails(SWPFormViewModel model)
        //{
        //    //var AccountNo = _nmHelper.GetAccountBranchBySchemeIdBankId(schemeId, bankId);
        //    var AccountNo = _nicFormService.GetBankAccountDetails(model);
        //    return Json(AccountNo, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult SaveDirectorDetailsForOpenScheme(SWPDirectorViewModel model)
        //{
        //    model = _nicFormService.SaveDirectorDetailsForOpenScheme(model);
        //    //model = _onlineService.SaveDirectorDetailToDataBaseForOpenScheme(model);
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetDirectorDetailsAsDataSource(DataSourceRequest request, int? formId)
        //{
        //    var flag = _nicFormService.GetDirectorDetailsAsDataSourceByFormId(request, formId);
        //    //var flag = _onlineService.GetDirectorDetailsFromDatabaseForOpenScheme(request, id);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult SaveUploadedDocument(SWPFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        //{
        //    model = _nicFormService.SaveUploadedDocument(model, files, userImage, signatureImage);
        //    //int flag = _onlineService.UploadDocumentByFormId(model, files, userImage, signatureImage);
        //    if (model.ReturnTypeId != ReturnType.Failure)
        //    {
        //        TempData["Document"] = "success";
        //        return RedirectToAction("IndustrialPreviewForm", "OnlineProperty", new { area = "NIC", id = model.EncryptedFormId });
        //    }
        //    else
        //    {
        //        TempData["Document"] = "failure";
        //        return RedirectToAction("IndustrialDocument", "OnlineProperty", new { area = "NIC", id = model.EncryptedFormId });
        //    }
        //}

        //[AllowAnonymous]
        //public JsonResult RemoveDocumentFromApplicationForm(string formNo, string filename)
        //{
        //    int flag = _nicFormService.RemoveDocumentFromApplicationForm(formNo, filename);
        //    //int flag = _onlineService.RemoveDocumentFromApplicationForm(formNo, filename);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetUploadedDocumentsForOnlineForm([DataSourceRequest]DataSourceRequest request, int? formId)
        //{
        //    var allDocs = _nicFormService.GetUploadedDocumentsByFormId(request, formId);
        //    //var allDocs = _onlineService.GetUploadedDocumentsForOnlineForm(request, formId);
        //    return Json(allDocs, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]//don't use this method, use below one
        //public JsonResult GetApplicationFeeAndCharges(int? schemeId, int? departmentId, int? propertyTypeId, int? areaTypeId)
        //{
        //    //var data = _onlineService.GetApplicationFeeAndCharges(schemeId, departmentId, propertyTypeId, areaTypeId);
        //    var data = _nicFormService.GetApplicationFeeAndCharges(schemeId, departmentId, propertyTypeId, areaTypeId);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetApplicationFormFeeAndProcessingCharge(SWPFormViewModel model)
        //{
        //    var data = _nicFormService.GetApplicationFormFeeAndProcessingCharge(model);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]//don't use this method use below one
        //public JsonResult ValidatePANnumber(string pan, int? areaId, int? schemeId)
        //{
        //    //int flag = _onlineService.ValidatePANnumber(pan, areaId, schemeId);
        //    int flag = _nicFormService.ValidatePANnumber(pan, areaId, schemeId);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult ValidatePANForApplicationForm(SWPFormViewModel model)
        //{
        //    var flag = _nicFormService.ValidatePANForApplicationForm(model);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult IsProcessingFeeAndReservationMoneyPaid(string formNo)
        //{
        //    int flag = _nicFormService.GetProcessingAndReservationMoneyPaymentStatus(formNo);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public ActionResult GenerateChallanForOnlineScheme(SWPFormViewModel model)
        //{
        //    string challan = string.Empty;
        //    //OnlineChallanViewModel requestModel = _onlineService.GenerateSchemeChallan(objOnlineModel);
        //    OnlineChallanViewModel requestModel = _nicFormService.SaveOfflinePaymentTransactionForChallan(model);
        //    if (requestModel != null)
        //    {
        //        challan = _templateParserService.GetParsedHTML(requestModel, "SchemeChallanTemplate.cshtml");
        //        //challan = _templateParserService.GetParsedHTML(requestModel, "SchemeChallanTemplate.cshtml");
        //        //bool flag = _onlineService.SaveGeneratedChallan(requestModel.ChallanId, challan);
        //    }
        //    return Json(challan, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public ActionResult GenerateRTGSChallanForOnlineScheme(SWPFormViewModel model)
        //{
        //    string challan = string.Empty;
        //    //OnlineChallanViewModel requestModel = _onlineService.GenerateSchemeChallan(objOnlineModel);
        //    OnlineChallanViewModel requestModel = _nicFormService.SaveOfflinePaymentTransactionForChallan(model);
        //    //var data = _onlineService.GetOnlineApplicationFormById(objOnlineModel.ApplicationFormId);
        //    var data = _nicFormService.GetOpenEndedSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = model.ApplicationFormId });
        //    if (requestModel != null)
        //    {
        //        requestModel.nFormFeeGST = Convert.ToDecimal(requestModel.FormModel.FormFeeGST).ToString("#,##0.00");
        //        requestModel.nApplicationFee = Convert.ToDecimal(requestModel.FormModel.ApplicationFee).ToString("#,##0.00");
        //        requestModel.nFormFeeSGST = Convert.ToDecimal(requestModel.FormModel.FormFeeSGST).ToString("#,##0.00");
        //        requestModel.nFormFeeCGST = Convert.ToDecimal(requestModel.FormModel.FormFeeCGST).ToString("#,##0.00");

        //        requestModel.nProcessingCharge = Convert.ToDecimal(requestModel.FormModel.ProcessingCharge).ToString("#,##0.00");
        //        requestModel.nProcessingSGST = Convert.ToDecimal(requestModel.FormModel.ProcessingSGST).ToString("#,##0.00");
        //        requestModel.nProcessingCGST = Convert.ToDecimal(requestModel.FormModel.ProcessingCGST).ToString("#,##0.00");
        //        requestModel.nProcessingGST = Convert.ToDecimal(requestModel.FormModel.ProcessingChargeGST).ToString("#,##0.00");
        //        requestModel.nEarnestMoney = Convert.ToDecimal(requestModel.FormModel.EarnestMoney).ToString("#,##0.00");
        //        requestModel.nTotalAmountGST = Convert.ToDecimal(requestModel.FormModel.TotalAmountGST).ToString("#,##0.00");
        //        requestModel.TotalAmountGSTInWords = ApplicationHelper.Rupees(Convert.ToInt64(requestModel.FormModel.TotalAmountGST));
        //        //}
        //        challan = _templateParserService.GetParsedHTML(requestModel, "ApplicationForRTGSRecieptTemplate.cshtml");

        //    }
        //    return Json(challan, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public ActionResult PrintOnlineSchemeForm(int FormId, string SchemeType)
        //{
        //    string schemeTemplate = string.Empty;
        //    SWPFormViewModel requestModel = new SWPFormViewModel();
        //    //requestModel = _onlineService.GetOnlineApplicationFormById(FormId);
        //    requestModel = _nicFormService.GetOpenEndedSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = FormId });
        //    if (requestModel != null)
        //    {
        //        if (SchemeType == OnlineSchemeType.Transport)
        //        {
        //            schemeTemplate = _templateParserService.GetParsedHTML(requestModel, "SchemeTransportFormTemplate.cshtml");
        //        }
        //        else if (SchemeType == OnlineSchemeType.OpenEnded)
        //        {
        //            schemeTemplate = _templateParserService.GetParsedHTML(requestModel, "SchemeOpenEndedFormTemplate.cshtml");
        //        }
        //        else if (SchemeType == OnlineSchemeType.IndustrialScheme)
        //        {
        //            schemeTemplate = _templateParserService.GetParsedHTML(requestModel, "SchemeIndustrialFormTemplate.cshtml");
        //        }
        //        else
        //        {
        //            schemeTemplate = _templateParserService.GetParsedHTML(requestModel, "SchemeAllotmentFormTemplate.cshtml");
        //        }
        //    }
        //    return Json(schemeTemplate, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public ActionResult UploadOnlineSchemeFormPaidChallan(SWPFormViewModel model, HttpPostedFileBase challan)
        //{
        //    var result = _nicFormService.SaveOnlineSchemePaymentStatus(model, challan);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetOnlineSchemeListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        //{
        //    var schemes = _nicFormService.GetOnlineSchemeListAsDataSource(request, model);
        //    return Json(schemes, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetDepartmentBySchemeAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        //{
        //    var departments = _nicFormService.GetDepartmentBySchemeAsDataSource(request, model);
        //    return Json(departments, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetOnlineSchemeApplications([DataSourceRequest] DataSourceRequest request, SWPFormViewModel modal)
        //{
        //    var applications = _nicFormService.GetOnlineSchemeApplicationAsDataSource(request, modal);
        //    return Json(applications, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetOnlineSchemeFormPayment([DataSourceRequest] DataSourceRequest request, SWPFormViewModel modal)
        //{
        //    var applications = _nicFormService.GetOnlineSchemeFormPaymentAsDataSource(request, modal);
        //    return Json(applications, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetSchemeFormPaymentTransaction(SWPPaymentViewModel model)
        //{
        //    SWPPaymentViewModel payment = _nicFormService.GetSchemeFormPaymentTransaction(model);
        //    return Json(payment, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult ValidateSchemeFormChallan(SWPPaymentViewModel model)
        //{
        //    if (model.ActionType == "ValidateAll")
        //    {
        //        var form = _nicFormService.ValidateSchemeFormChallan(model);
        //        return Json(model, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        model = _nicFormService.ValidateSchemeFormChallan(model);
        //        if (model.ReturnTypeId == ReturnType.Updated)
        //        {
        //            var form = _nicFormService.GetOnlineSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = model.ApplicationFormId });

        //            //NiveshMitraApiServiceHelper _niveshMitraServices = new NiveshMitraApiServiceHelper();
        //            NICServiceApiHelper _niveshMitraServices = new NICServiceApiHelper();
        //            WReturn_CUSID_STATUSModel wsmodel = new WReturn_CUSID_STATUSModel();
        //            wsmodel.ControlID = form.NICControlId; //model.NICControlId;
        //            wsmodel.UnitID = form.NICUnitId; //model.NICUnitId;
        //            wsmodel.ServiceID = form.NICServiceId; //model.NICServiceId;
        //            wsmodel.ProcessIndustryID = form.ApplicationFormId.ToString(); //model.NICProcessIndustryId; 
        //            wsmodel.Status_Code = Common.ServiceStatus.FORM_SUBMITTED;
        //            wsmodel.Remarks = ServiceStatus_Text.FORM_SUBMITTED;

        //            _niveshMitraServices.GetWReturn_CUSID_STATUS(wsmodel);
        //        }
        //        return Json(model, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[AllowAnonymous]
        //public JsonResult ValidateOnlineSchemeForm(SWPFormViewModel model)
        //{
        //    //model = _nicFormService.ValidateOnlineSchemeForm(model);
        //    model = _nicFormService.SaveOnlineSchemeFormStatus(model);
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult SaveOnlineSchemeFormProcessRequest(SWPFormViewModel model)
        //{
        //    model = _nicFormService.SaveOnlineSchemeFormProcessRequest(model);

        //    int EStatus = model.ActionType == "Process" ? ((int)Enum.Parse(typeof(OnlineApplicationProcess), model.ProcessType)) : (Convert.ToInt32(model.ProcessType));
        //    string EnumStatus = Convert.ToString(EStatus);
        //    if (model.ActionType == "Process")
        //    {
        //        EnumStatus = (!string.IsNullOrEmpty(EnumStatus) ? (EnumStatus == strOnlineApplicationProcess.MoveToOSD ? "File Move To OSD" : (EnumStatus == strOnlineApplicationProcess.Scrutiny ? "File goes for Scrutiny" : (EnumStatus == strOnlineApplicationProcess.ApprovalCEO ? "Approval to CEO" : (EnumStatus == strOnlineApplicationProcess.Draw ? "File gor for raw" : string.Empty)))) : string.Empty) + " - " + ServiceStatus_Text.INPROCESS;
        //    }
            
        //    var OnlineAppDetails = model.ResultMessage.clsResultType.Where(m => m.PrimaryKey > 0).ToList();
        //    if (OnlineAppDetails.Count > 0 && model.ActionType == "Process")
        //    {
        //        for (int i = 0; i < OnlineAppDetails.Count(); i++)
        //        {
        //            if (OnlineAppDetails[i].ReturnType == 201)
        //            {
        //                int AppId = OnlineAppDetails[i].PrimaryKey;
        //                var data = _nicFormService.GetOnlineSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = AppId });
        //                if (data != null)
        //                {
        //                    WReturn_CUSID_STATUSModel wsmodel = new WReturn_CUSID_STATUSModel();
        //                    wsmodel.ControlID = data.NICControlId;
        //                    wsmodel.ApplicationID = data.ApplicationFormId.ToString();
        //                    wsmodel.ProcessIndustryID = data.ApplicationFormId.ToString();
        //                    wsmodel.UnitID = data.NICUnitId;
        //                    wsmodel.ServiceID = data.NICServiceId;
        //                    wsmodel.Status_Code = ServiceStatus.INPROCESS;
        //                    wsmodel.Remarks = EnumStatus;
        //                    wsmodel.Fee_Status = string.Empty;
        //                    wsmodel.Fee_Amount = string.Empty;
        //                    wsmodel.passsalt = Passalt;

        //                    //NiveshMitraApiServiceHelper niveshMitraServices = new NiveshMitraApiServiceHelper();
        //                    NICServiceApiHelper _niveshMitraServices = new NICServiceApiHelper();
        //                    string message = _niveshMitraServices.GetWReturn_CUSID_STATUS(wsmodel);
        //                }
        //            }
        //        }
        //    }
        //    if (OnlineAppDetails.Count > 0 && model.ActionType == "Status")
        //    {
        //        for (int i = 0; i < OnlineAppDetails.Count(); i++)
        //        {
        //            //if (OnlineAppDetails[i].ReturnType == 201)
        //            {
        //                int AppId = OnlineAppDetails[i].PrimaryKey;
        //                var data = _nicFormService.GetOnlineSchemeFormDataById(new SWPFormViewModel { ApplicationFormId = AppId });
        //                if (data != null)
        //                {
        //                    WReturn_CUSID_STATUSModel wsmodel = new WReturn_CUSID_STATUSModel();
        //                    wsmodel.ControlID = data.NICControlId;
        //                    wsmodel.ApplicationID = data.ApplicationFormId.ToString();
        //                    wsmodel.ProcessIndustryID = data.ApplicationFormId.ToString();
        //                    wsmodel.UnitID = data.NICUnitId;
        //                    wsmodel.ServiceID = data.NICServiceId;
        //                    wsmodel.Status_Code = ServiceStatus.QUERY_OBJECTION;
        //                    wsmodel.Remarks = "Objection"; //EnumStatus;
        //                    wsmodel.Fee_Status = string.Empty;
        //                    wsmodel.Fee_Amount = string.Empty;
        //                    wsmodel.passsalt = Passalt;

        //                    //NiveshMitraApiServiceHelper niveshMitraServices = new NiveshMitraApiServiceHelper();
        //                    NICServiceApiHelper _niveshMitraServices = new NICServiceApiHelper();
        //                    string message = _niveshMitraServices.GetWReturn_CUSID_STATUS(wsmodel);
        //                }
        //            }
        //        }
        //    }
        //    return Json(model.Message, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult SaveAllotmentDetailByOnlineSchemeFormId(SWPFormViewModel model)
        //{
        //    model = _nicFormService.SaveAllotmentDetailByOnlineSchemeFormId(model);
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// dropdown for scheme,department,formtype,subformtype,applicanttype,companytype,areatype,directortype,category,occupation,gender,maritalstatus
        ///// </summary>
        ///// <param name="request"></param>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public JsonResult GetDropDownListByTypeAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        //{
        //    var data = _nicFormService.GetDropDownListByTypeAsDataSource(request, model);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// Only for Online Payment
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="banktype"></param>
        //[AllowAnonymous]
        //public void OnlineSchemePayment(int id, int banktype)
        //{
        //    SWPPaymentViewModel payment = _nicFormService.SaveOnlinePaymentTransaction(new SWPPaymentViewModel { ApplicationFormId = id, BankId = banktype });
        //    PaymentGateway gateway = new PaymentGateway();
        //    if (banktype == 1)//indusind
        //    {
        //        gateway.PayOnline(payment);
        //    }
        //    if (banktype == 2) //hdfc
        //    {
        //        gateway.HDFCOnlinePaymentForSchemeForm(payment);
        //    }
        //}

        //[AllowAnonymous]
        //public ActionResult PaymentReceipt(FormCollection form)
        //{
        //    var paidmodel = new SWPPaymentViewModel();
        //    if (form != null)
        //    {
        //        //paidmodel = _onlineService.UpdateChallanOnlinePaymentTransaction(form);
        //        paidmodel = _nicFormService.SaveOnlinePaymentTransactionReturn(form);
        //        if (paidmodel.ReturnTypeId == ReturnType.Success)
        //        {
        //            TempData["Message"] = "Payment completed successfully";
        //            //return View(paidmodel);
        //        }
        //        else if (paidmodel.ReturnTypeId == ReturnType.Failed)
        //        {
        //            TempData["Message"] = "Payment request failed";
        //            //return RedirectToAction("Index", "Payment", new { area = "Online", rid = form["udf2"], id = form["udf3"] });
        //        }
        //        else if (paidmodel.ReturnTypeId == ReturnType.Mismatch)
        //        {
        //            TempData["Message"] = "Transaction key did not match";
        //            //return RedirectToAction("Index", "Payment", new { area = "Online", rid = form["udf2"], id = form["udf3"] });
        //        }
        //        else
        //        {
        //            TempData["Message"] = "Error in Transaction";
        //            //return RedirectToAction("BankChallan", "Authority", new { area = "Member" });
        //        }
        //        return View(paidmodel);
        //    }
        //    else
        //    {
        //        TempData["ErrorInTraxaction"] = "Error in Transaction";
        //        return RedirectToAction("SchemeInformation", "OnlineProperty", new { area = "NIC" });
        //    }
        //}

        //[AllowAnonymous]
        //public JsonResult ValidateOTP(string otpMobile)
        //{
        //    int flag = 0;
        //    if ((int)Session["OTPmobile"] == Convert.ToInt32(otpMobile)) flag = ReturnType.Success;
        //    //if ((string)Session["OTPmobile"] == (otpMobile)) { flag = ReturnType.Success; }
        //    else { flag = ReturnType.Failure; }
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        ////[AllowAnonymous]
        ////public JsonResult SendOTP(string mobileNo, string email)
        ////{
        ////    int flag = ApplicationHelper.GenerateOTP();
        ////    Session["OTPmobile"] = flag;
        ////    //int flag = 123;
        ////    //Session["OTPmobile"] = flag;
        ////    string emailMessage = string.Format(NAMessages.OnlineApplicationOTP, flag);
        ////    string mobileMessage = string.Format(NAMessages.OnlineApplicationOTP, flag);
        ////    if (mobileNo != null && mobileNo != "") ApplicationHelper.SendSMS(mobileNo, mobileMessage);
        ////    if (email != null && email != "") ApplicationHelper.SendEmail(email, "OnlineForm", emailMessage);
        ////    return Json(flag, JsonRequestBehavior.AllowGet);
        ////}

        //public JsonResult GetSchemeFormChallanAsDataSource([DataSourceRequest] DataSourceRequest request, SWPFormViewModel modal)
        //{
        //    var challan = _nicFormService.GetSchemeFormChallanAsDataSource(request, modal);
        //    return Json(challan, JsonRequestBehavior.AllowGet);
        //}

        ////[HttpPost]
        ////private SWPFormViewModel GetNICPostedBasicDetails(SWPPostViewModel apimodel)
        ////{
        ////    SWPFormViewModel nicmodel = new SWPFormViewModel();
        ////    if (apimodel.TxtControlID != null && apimodel.TxtUnitID != null && apimodel.TxtServiceID != null)
        ////    {
        ////        Session["WBasicDetailsNIC"] = null;
        ////        string xmlInputData = string.Empty;

        ////        nicmodel.NICControlId = apimodel.TxtControlID;
        ////        nicmodel.NICUnitId = apimodel.TxtUnitID;
        ////        nicmodel.NICServiceId = apimodel.TxtServiceID;
        ////        nicmodel.NICProcessIndustryId = apimodel.TxtProcessIndustryID;
        ////        nicmodel.NICApplicationId = apimodel.TxtApplicationID;
        ////        nicmodel.AppType = Constants.NIC;
        ////        nicmodel.IsFromNIC = true;
        ////        using (var client = new upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap_nic"))
        ////        {
        ////            string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
        ////            System.Data.DataSet result = client.WGetBasicDetails(apimodel.TxtControlID, apimodel.TxtUnitID, apimodel.TxtServiceID, apimodel.TxtProcessIndustryID, Passalt);
        ////            if (result != null)
        ////            {
        ////                xmlInputData = result.GetXml();
        ////                nicmodel.NICXmlInputTable = xmlInputData;
        ////                Deserial xmlSerializer = new Deserial();
        ////                NewDataSet naDataSet = xmlSerializer.Deserialize<NewDataSet>(xmlInputData);
        ////                if (naDataSet.Table != null)
        ////                {
        ////                    naDataSet.Table.ServiceID = apimodel.TxtServiceID;
        ////                    Session["WBasicDetailsNIC"] = naDataSet;
        ////                    nicmodel.BasicDetailsGetModel = naDataSet;
        ////                    nicmodel.FlagId = ReturnType.Saved;

        ////                    return nicmodel;
        ////                }
        ////                else
        ////                {
        ////                    nicmodel.FlagId = ReturnType.Failed;
        ////                    return nicmodel;
        ////                }
        ////            }
        ////            else
        ////            {
        ////                nicmodel.FlagId = ReturnType.NotExist;
        ////                return nicmodel;
        ////            }
        ////        }
        ////    }
        ////    else
        ////    {
        ////        nicmodel.FlagId = ReturnType.NotExist;
        ////    }
        ////    return nicmodel;
        ////}

        ////public string GetWReturn_CUSID_STATUS(SWPStatusViewModel apiModel)
        ////{
        ////    //objWReturn_CUSID_STATUSModel.ProcessIndustryID = string.Empty;
        ////    //objWReturn_CUSID_STATUSModel.Fee_Status = string.Empty;
        ////    apiModel.Transaction_ID = string.Empty;
        ////    apiModel.Transaction_Date = string.Empty;
        ////    apiModel.Transaction_Date_Time = string.Empty;
        ////    apiModel.NOC_Certificate_Number = string.Empty;
        ////    apiModel.NOC_URL = string.Empty;
        ////    apiModel.ISNOC_URL_ActiveYesNO = string.Empty;
        ////    string path = string.Empty;
        ////    string xmlInputData = string.Empty;
        ////    string xmlOutputData = string.Empty;
        ////    using (var client = new upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap_nic"))
        ////    {
        ////        string result = client.WReturn_CUSID_STATUS(apiModel.ControlID, apiModel.UnitID, apiModel.ServiceID, apiModel.ProcessIndustryID, apiModel.ApplicationID, apiModel.Status_Code, apiModel.Remarks, apiModel.Fee_Amount, apiModel.Fee_Status, apiModel.Transaction_ID, apiModel.Transaction_Date, apiModel.Transaction_Date_Time, apiModel.NOC_Certificate_Number, apiModel.NOC_URL, apiModel.ISNOC_URL_ActiveYesNO, apiModel.passsalt);
        ////        return result;
        ////    }
        ////}

        ////private string PostReturn_CUSID_STATUS(SWPFormViewModel model, string ServiceStatusCode, string ServiceStatusRemarks)
        ////{
        ////    string message = string.Empty;
        ////   // var data = _onlineService.GetNICSingleWindowData(model);
        ////    var data = _nicFormService.GetNICSingleWindowTableData(model);
        ////    if (data != null)
        ////    {
        ////        WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel();
        ////        objWReturn_CUSID_STATUSModel.ControlID = data.Control_ID;
        ////        objWReturn_CUSID_STATUSModel.ApplicationID = Convert.ToString(model.ApplicationFormId);
        ////        objWReturn_CUSID_STATUSModel.ProcessIndustryID = Convert.ToString(model.ApplicationFormId);
        ////        objWReturn_CUSID_STATUSModel.UnitID = data.Unit_Id;
        ////        objWReturn_CUSID_STATUSModel.ServiceID = data.ServiceID;
        ////        objWReturn_CUSID_STATUSModel.Status_Code = ServiceStatusCode;
        ////        objWReturn_CUSID_STATUSModel.Remarks = ServiceStatusRemarks;
        ////        objWReturn_CUSID_STATUSModel.Fee_Status = ServiceStatusRemarks;
        ////        objWReturn_CUSID_STATUSModel.Fee_Amount = Convert.ToString(model.TotalAmount);
        ////        objWReturn_CUSID_STATUSModel.passsalt = Passalt;

        ////        //Update status of Single Window
        ////        //# verify from Service
        ////        //#update details to db
        ////        if (Convert.ToInt32(model.PayType) == Constants.singleWindowPortalApplicationPayment)
        ////        {
        ////            //GetWGetUBPaymentDetails(objWReturn_CUSID_STATUSModel);
        ////        }

        ////        //Pass Object to NIC Service
        ////        message = GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);
        ////    } return message;
        ////}

        ////[AllowAnonymous]
        ////public JsonResult IsApplicationFormFeePaidViaNiveshMitra(SWPFormViewModel model)
        ////{
        ////    var flag = false;
        ////    string servicePassalt = ConfigurationManager.AppSettings["ServicePassalt"];
        ////    System.Data.DataSet _newDataSet = new System.Data.DataSet();
        ////    NewDataSet naDataSet = new NewDataSet();
        ////    try
        ////    {
        ////        string path = string.Empty;
        ////        string xmlInputData = string.Empty;
        ////        string xmlOutputData = string.Empty;
        ////        //using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
        ////        using (var client = new upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap_nic"))
        ////        {
        ////            _newDataSet = client.WGetUBPaymentDetails(model.NICControlId, model.NICUnitId, model.NICServiceId, servicePassalt);
        ////            if (_newDataSet != null)
        ////            {
        ////                xmlInputData = _newDataSet.GetXml();
        ////                Deserial xmlSerializer = new Deserial();
        ////                naDataSet = xmlSerializer.Deserialize<NewDataSet>(xmlInputData);
        ////                if (naDataSet.Table != null)
        ////                {
        ////                    if (naDataSet.Table.Status_Code == Common.ServiceStatus.FEE_PAID)
        ////                    {
        ////                        //var result = _onlineService.UpdateOESFormPaymentStatus(model);
        ////                        var result = _nicFormService.SaveOnlineSchemePaymentStatus(model, null);
        ////                        flag = result.NICFeeStatusId == "11" ? true : false;

        ////                        if (flag)
        ////                        {
        ////                            // fee status
        ////                            //NiveshMitraApiServiceHelper _niveshMitraServices = new NiveshMitraApiServiceHelper();
        ////                            NICServiceApiHelper _niveshMitraServices = new NICServiceApiHelper();
        ////                            WReturn_CUSID_STATUSModel stmodel = new WReturn_CUSID_STATUSModel();
        ////                            stmodel.ControlID = model.NICControlId;
        ////                            stmodel.UnitID = model.NICUnitId;
        ////                            stmodel.ServiceID = model.NICServiceId;
        ////                            stmodel.ProcessIndustryID = model.NICProcessIndustryId; //niveshMitraDetails.ProcessIndustryID;
        ////                            stmodel.Status_Code = Common.ServiceStatus.SAVE_AS_DRAFT;
        ////                            //stmodel.Remarks = ServiceStatus_Text.FEE_PAID;
        ////                            //stmodel.Fee_Amount = objNewDataSet.Table.Fee_Amount;
        ////                            //stmodel.Fee_Status = PaymentStatus_NIC.PAID;

        ////                            _niveshMitraServices.GetWReturn_CUSID_STATUS(stmodel);
        ////                        }
        ////                    }
        ////                }
        ////            }

        ////        }
        ////        //return flag;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        //return objNewDataSet;
        ////        throw ex;
        ////    }
        ////    return Json(flag, JsonRequestBehavior.AllowGet);
        ////}


        //#endregion
    }
}
