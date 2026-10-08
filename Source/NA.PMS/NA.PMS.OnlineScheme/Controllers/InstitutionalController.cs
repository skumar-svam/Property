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
    public class InstitutionalController : Controller
    {
         private IOnlineSchemeRepository _schemeRepository;

         public InstitutionalController()
         {
            _schemeRepository = new OnlineSchemeRepository();
         }

        // GET: Scheme/Institutional
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult SchemeInformation(SchemeFormViewModel schemeForm)
        {
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["InstitutionalSchemeHeaderName"];
            schemeForm = schemeForm == null ? new SchemeFormViewModel() : schemeForm;

            if (Session["SchemeUserLoginDetails"] != null)
            {
                SchemeFormViewModel _OnlineScheme = new SchemeFormViewModel();
                _OnlineScheme = (SchemeFormViewModel)Session["SchemeUserLoginDetails"];

                return RedirectToAction("RentingForm", "Housing", new { area = "Online", ApplicationFormId = _OnlineScheme.ApplicationFormId });
            }
            else
            {
                return View();
            }
        }

        public ActionResult SchemeForm(SchemeFormViewModel form)
        {
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["InstitutionalSchemeHeaderName"];
            ViewBag.SchemeHeaderName = "Institutional Plot Scheme";
            form = form == null ? new SchemeFormViewModel() : form;
            return View(form);
        }

        public ActionResult SchemeDocument(SchemeFormViewModel form)
        {
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["InstitutionalSchemeHeaderName"];
            if (form != null)
            {
                if (!string.IsNullOrEmpty(form.EncryptedFormId))
                {
                    int ID = Convert.ToInt32(OnlineSchemeHelper.Decode(form.EncryptedFormId));
                    if (ID > 0)
                    {
                        SchemeFormViewModel model = new SchemeFormViewModel();
                        model = _schemeRepository.GetOnlineSchemeApplicationFormById(ID);
                        //if (model.FormType == "Online") return View(model);
                        //else return RedirectToAction("PreviewForm", new { id = model.EncryptedFormId });
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
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["InstitutionalSchemeHeaderName"];

            if (form != null && form.ApplicationFormId != null && form.ApplicationFormId > 0)
            {
                form = _schemeRepository.GetOnlineSchemeApplicationFormById(form.ApplicationFormId);
            }
            else
            {
                form = form == null ? new SchemeFormViewModel() : form;
                form.SchemeId = Convert.ToInt32(ConfigurationManager.AppSettings["InstitutionalOpenEndSchemeId"]);
                form.DepartmentId = 1;
                form.Department = "Institutional";
                form = _schemeRepository.GetOnlineSchemeBasicDetail(form);
            }
            return View(form);
        }

        public ActionResult PreviewSchemeForm(SchemeFormViewModel form)
        {
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["InstitutionalSchemeHeaderName"];
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
        public ActionResult PrintInstitutionalSchemeFormByTemplate(SchemeFormViewModel model)
        {
            string _PrintFormContent = string.Empty;
            SchemeFormViewModel formModel = new SchemeFormViewModel();
            formModel = _schemeRepository.GetOnlineSchemeApplicationFormById(model.ApplicationFormId);
            if (formModel != null)
            {
                var templatePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["InstitutionalSchemeFormTemplatePath"]);
                _PrintFormContent = RazorParser.ParseSchemeFormTemplate(formModel, templatePath);
                //_PrintFormContent = RazorParser.ParseTemplate(formModel, "SchemeAllotmentFormTemplate.cshtml");
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
            return RedirectToAction("PreviewSchemeForm", "Institutional", new { area = "Online", id = OnlineSchemeHelper.Encode(form.ApplicationFormId.ToString()) });
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

                var templatePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["InstitutionalTemplatePath"]);
                challan = RazorParser.ParseSchemeFormChallanTemplate(formchallan, templatePath);
                //challan = RazorParser.ParseTemplate(formchallan, "ApplicationForRTGSRecieptTemplate.cshtml");
            }
            return Json(challan, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult SaveSchemeFormPaidChallan(SchemeFormViewModel form, HttpPostedFileBase files)
        {
            form = _schemeRepository.SaveSchemeFormPaidChallanDetail(form, files);
            return Json(form, JsonRequestBehavior.AllowGet);
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
    }
}
