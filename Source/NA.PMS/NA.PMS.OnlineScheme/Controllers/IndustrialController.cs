using Kendo.Mvc.UI;
using NA.PMS.OnlineScheme;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NA.PMS.Web.Areas.Online.Controllers
{
    public class IndustrialController : Controller
    {
        // GET: Scheme/Industry
        private IOnlineSchemeRepository _schemeRepository;

        public IndustrialController()
        {
            _schemeRepository = new OnlineSchemeRepository();
        }

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
                    //flag = _schemeRepository.ValidateOnlineSchemeFormByType(onlineForm);
                    var exform = _schemeRepository.ValidateOnlineSchemeFormByType(onlineForm);
                    if (flag == OSReturnTypeId.Exist)
                    {
                        Session["SchemeApplicant"] = null;
                        var _schemeForm = _schemeRepository.GetOnlineSchemeApplicationFormById(onlineForm.ApplicationFormId);
                        
                        string _openScheme = ConfigurationManager.AppSettings["OpenScheme"];

                        if (schemeLogin.TxtSchemeType == SchemeConstant.OpenEnded)
                        {
                            Session["SchemeApplicant"] = _schemeForm;
                            return RedirectToAction("OpenEndSchemeForm", "Industry", new { area = "Online" });
                        }
                        else
                        {
                            TempData["ErrorLoginMessage"] = "Please Login through Open Ended scheme.";
                            return RedirectToAction("SchemeInformation", "Industry", new { area = "Online" });
                        }
                    }
                    else if (flag == OSReturnTypeId.UserNameNotExist)
                    {
                        TempData["ErrorLoginMessage"] = "UserName/FormID is not correct."; return RedirectToAction("SchemeInformation", "Industry", new { area = "Online" });
                    }
                    else if (flag == OSReturnTypeId.PasswordNotExist)
                    {
                        TempData["ErrorLoginMessage"] = "Password is not correct."; return RedirectToAction("SchemeInformation", "Industry", new { area = "Online" });
                    }
                    else if (flag == OSReturnTypeId.Failure)
                    {
                        TempData["ErrorLoginMessage"] = "Form not validated."; return RedirectToAction("SchemeInformation", "Industry", new { area = "Online" });
                    }
                }
            }
            return RedirectToAction("SchemeInformation", "Industry", new { area = "Online" });
        }

        [AllowAnonymous]
        public ActionResult SchemeInformation(SchemeFormViewModel schemeForm)
        {
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["Industrial-OES-SchemeName"];
            schemeForm = schemeForm == null ? new SchemeFormViewModel() : schemeForm;

            if (Session["SchemeApplicant"] != null)
            {
                SchemeFormViewModel _OnlineScheme = new SchemeFormViewModel();
                _OnlineScheme = (SchemeFormViewModel)Session["SchemeApplicant"];

                return RedirectToAction("RentingForm", "Housing", new { area = "Online", ApplicationFormId = _OnlineScheme.ApplicationFormId });
            }
            else
            {
                return View();
            }
        }

        public ActionResult SchemeForm(SchemeFormViewModel form)
        {
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["Industrial-IP-SchemeName"];
            ViewBag.SchemeHeaderName = "Institutional Open End Scheme";
            form = form == null ? new SchemeFormViewModel() : form;
            return View(form);
        }

        public ActionResult SchemeDocument(SchemeFormViewModel form)
        {
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["Industrial-IP-SchemeName"];
            if (form != null)
            {
                if (!string.IsNullOrEmpty(form.EncryptedFormId))
                {
                    int ID = Convert.ToInt32(OnlineSchemeHelper.Decode(form.EncryptedFormId));
                    if (ID > 0)
                    {
                        SchemeFormViewModel model = new SchemeFormViewModel();
                        model = _schemeRepository.GetOnlineSchemeApplicationFormById(ID);
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
            return View(form);
        }

        public ActionResult OpenSchemeDocument(SchemeFormViewModel form)
        {
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["Industrial-OES-SchemeName"];
            if (form != null)
            {
                if (!string.IsNullOrEmpty(form.EncryptedFormId))
                {
                    int ID = Convert.ToInt32(OnlineSchemeHelper.Decode(form.EncryptedFormId));
                    if (ID > 0)
                    {
                        SchemeFormViewModel model = new SchemeFormViewModel();
                        model = _schemeRepository.GetOnlineSchemeApplicationFormById(ID);
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
            return View(form);
        }

        public ActionResult OpenEndSchemeForm(SchemeFormViewModel form)
        {
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["Industrial-OES-SchemeName"];

            if (form != null && form.ApplicationFormId != null && form.ApplicationFormId > 0)
            {
                form = _schemeRepository.GetOnlineSchemeApplicationFormById(form.ApplicationFormId);
            }
            else
            {
                form = form == null ? new SchemeFormViewModel() : form;
                form.SchemeId = Convert.ToInt32(ConfigurationManager.AppSettings["IndustrialOpenEndSchemeId"]);
                form.DepartmentId = 4;
                form.Department = "Industry";
                form = _schemeRepository.GetOnlineSchemeBasicDetail(form);
                //form.SchemeName = "2020-21 (Institutional)(02)";
                //form.ApplicantType = "Company";
            }
            return View(form);
        }

        public ActionResult PreviewSchemeForm(SchemeFormViewModel form)
        {
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["Industrial-OES-SchemeName"];
            if (!string.IsNullOrEmpty(form.EncryptedFormId))
            {
                int ID = Convert.ToInt32(OnlineSchemeHelper.Decode(form.EncryptedFormId));
                if (ID > 0)
                {
                    SchemeFormViewModel model = new SchemeFormViewModel();
                    model = _schemeRepository.GetOnlineSchemeApplicationFormById(ID);

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
        public JsonResult GetSchemeFormFeeAndChargesByArea(SchemeFormViewModel model)
        {
            var data = _schemeRepository.GetSchemeFormFeeAndChargesByArea(model);
            return Json(data, JsonRequestBehavior.AllowGet);
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
        public JsonResult SaveOnlineSchemeApplicationForm(SchemeFormViewModel model, System.Web.HttpPostedFileBase userImage, System.Web.HttpPostedFileBase signatureImage)
        {
            if (model != null)
            {
                _schemeRepository.SaveOnlineSchemeApplicationForm(model, userImage, signatureImage);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SaveOpenEndSchemeApplicationForm(SchemeFormViewModel model, System.Web.HttpPostedFileBase userImage, System.Web.HttpPostedFileBase signatureImage)
        {
            if (model != null)
            {
                _schemeRepository.SaveOpenEndedSchemeFormDetail(model, userImage, signatureImage);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult SaveProposedFirmDirectorsDetail(SchemeProposedFirmViewModel model)
        {
            var data = _schemeRepository.SaveProposedFirmDirectorsDetail(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult GetProposedFirmDirectorsDetailAsDataSource(DataSourceRequest request, SchemeFormViewModel model)
        {
            var data = _schemeRepository.GetProposedFirmDirectorsDetailAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult SaveDocumentForOnlineSchemeForm(SchemeFormViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var data = _schemeRepository.SaveDocumentsForOnlineSchemeApplication(model, files);
            return RedirectToAction("PreviewSchemeForm", new { ApplicationFormId = data.ApplicationFormId });
            //return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDocumentListAsDataSourceForOnlineSchemeform([DataSourceRequest]DataSourceRequest request, SchemeFormViewModel model)
        {
            var data = _schemeRepository.GetChecklistDocumentsAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetUploadedDocumentsAsDataSource([DataSourceRequest]DataSourceRequest request, SchemeFormViewModel model)
        {
            var data = _schemeRepository.GetUploadedDocumentsAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public void OnlineSchemeFormPayment(SchemeFormViewModel model)
        {
            SchemeFormViewModel form = _schemeRepository.GetOnlineSchemeApplicationFormById(model.ApplicationFormId);
            form.PayType = "FormPayment";
            SchemePaymentViewModel payment = _schemeRepository.SaveOnlineSchemePaymentTransaction(form);
            OSPaymentGateway gateway = new OSPaymentGateway();
            if (model.BankId == 74)//indusind banktype == 1
            {
                gateway.PayOnline(payment);
            }
            if (model.BankId == 67) //hdfc banktype == 2
            {
                gateway.PayOnlineHDFC(payment);
            }
        }

        [AllowAnonymous]
        public ActionResult PrintIndustrialSchemeFormByTemplate(SchemeFormViewModel model)
        {
            string _PrintFormContent = string.Empty;
            SchemeFormViewModel formModel = new SchemeFormViewModel();
            formModel = _schemeRepository.GetOnlineSchemeApplicationFormById(model.ApplicationFormId);
            if (formModel != null)
            {
                var templatePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["IndustrialSchemeFormTemplatePath"]);
                _PrintFormContent = RazorParser.ParseSchemeFormTemplate(formModel, templatePath);
            }
            return Json(_PrintFormContent, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult SaveGeneratedSchemeFormChallan(SchemeFormViewModel form, HttpPostedFileBase files)
        {
            form = _schemeRepository.SaveGeneratedSchemeFormChallan(form, files);
            if (form.IsPaidChallanUploaded == true)
            {
                TempData["ServiceMessage"] = form.Message;
            }
            else
            {
                TempData["ServiceMessage"] = "Service Failure";
            }
            return RedirectToAction("PreviewSchemeForm", "Industry", new { area = "Online", id = OnlineSchemeHelper.Encode(form.ApplicationFormId.ToString()) });
        }

        [AllowAnonymous]
        public ActionResult GenerateSchemeFormChallanByTemplate(SchemeFormViewModel form)
        {
            string challan = string.Empty;
            //var _SchemeForm = _schemeRepository.SaveAndGetOnlineSchemeFormCallan(form);
            SchemeChallanViewModel formchallan = _schemeRepository.GenerateSchemeFormChallanByTemplate(form);
            //var data = _schemeRepository.GetOnlineApplicationFormById(form.ApplicationFormId);
            if (formchallan != null)
            {
                //formchallan.TextFormFeeGST = Convert.ToDecimal(formchallan.FormModel.FormFeeGST).ToString("#,##0.00");
                //formchallan.TextApplicationFee = Convert.ToDecimal(formchallan.FormModel.ApplicationFee).ToString("#,##0.00");
                //formchallan.TextFormFeeSGST = Convert.ToDecimal(formchallan.FormModel.FormFeeSGST).ToString("#,##0.00");
                //formchallan.TextFormFeeCGST = Convert.ToDecimal(formchallan.FormModel.FormFeeCGST).ToString("#,##0.00");
                var totalAmount = formchallan.FormModel.TotalAmountGST - formchallan.FormModel.FormFeeGST;
                formchallan.TextProcessingCharge = Convert.ToDecimal(formchallan.FormModel.ProcessingCharge).ToString("#,##0.00");
                formchallan.TextProcessingSGST = Convert.ToDecimal(formchallan.FormModel.ProcessingSGST).ToString("#,##0.00");
                formchallan.TextProcessingCGST = Convert.ToDecimal(formchallan.FormModel.ProcessingCGST).ToString("#,##0.00");
                formchallan.TextProcessingGST = Convert.ToDecimal(formchallan.FormModel.ProcessingChargeGST).ToString("#,##0.00");
                formchallan.TextEarnestMoney = Convert.ToDecimal(formchallan.FormModel.EarnestMoney).ToString("#,##0.00");
                formchallan.TextTotalAmountGST = Convert.ToDecimal(formchallan.FormModel.TotalAmountGST).ToString("#,##0.00");
                formchallan.TotalAmountGSTInWords = OnlineSchemeHelper.ConvertNumberIntoWords(Convert.ToInt64(formchallan.FormModel.TotalAmountGST));

                var templatePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["IndustrialSchemeChallanTemplatePath"]);
                challan = RazorParser.ParseSchemeFormChallanTemplate(formchallan, templatePath);
            }
            return Json(challan, JsonRequestBehavior.AllowGet);
        }

    }
}
