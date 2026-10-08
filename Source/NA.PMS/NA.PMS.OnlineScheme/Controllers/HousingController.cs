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
    public class HousingController : Controller
    {
        private IOnlineSchemeRepository _schemeRepository;
       
        public HousingController()
        {
            _schemeRepository = new OnlineSchemeRepository();
        }
        // GET: Scheme/Housing
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult SchemeInformation(SchemeFormViewModel schemeForm)
        {
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["HousingSchemeHeaderName"];
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

        [AllowAnonymous]
        public ActionResult SchemeForm(SchemeFormViewModel form)
        {
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["HousingSchemeHeaderName"];
            ViewBag.SchemeHeaderName = "House/Flat Rent Scheme";
            form = form == null ? new SchemeFormViewModel() : form;
            return View(form);
        }

        [AllowAnonymous]
        public ActionResult RentingForm(SchemeFormViewModel form)
        {
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["HousingSchemeHeaderName"];

            if (form != null && form.ApplicationFormId != null && form.ApplicationFormId > 0)
            {
                form = _schemeRepository.GetOnlineSchemeApplicationFormById(form.ApplicationFormId);
            }
            else
            {
                form = form == null ? new SchemeFormViewModel() : form;
                form.SchemeId = Convert.ToInt32(ConfigurationManager.AppSettings["HousingRentSchemeId"]);
                //ViewBag.SchemeHeaderName = "Housing Yojana code No - 2020-21 (H)(02)";
                form = _schemeRepository.GetOnlineSchemeBasicDetail(form);
                form.SchemeName = "2020-21 (H)(02)";
                form.Department = "Housing";
                form.DepartmentId = 5;
                form.ApplicantType = "Company";
            }
            return View(form);
        }

        [AllowAnonymous]
        public ActionResult SchemeDocument(SchemeFormViewModel form)
        {
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["HousingSchemeHeaderName"];
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

        [AllowAnonymous]
        public ActionResult PreviewSchemeForm(string id)
        {
            ViewBag.SchemeHeaderName = ConfigurationManager.AppSettings["HousingSchemeHeaderName"];
            if (!string.IsNullOrEmpty(id))
            {
                int ID = Convert.ToInt32(OnlineSchemeHelper.Decode(id));
                if (ID > 0)
                {
                    SchemeFormViewModel model = new SchemeFormViewModel();
                    model = _schemeRepository.GetOnlineSchemeApplicationFormById(ID);
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
                    //    //Commented on 4 oct 2017
                    //    ////User will update challan details(3 oct 2017)
                    //    //if (model.PaymentMode == "Offline")
                    //    //{
                    //    //    model.BankModel = _generalService.GetBankListBySchemeId((int)model.SchemeId);
                    //    //    return View(model);
                    //    //}
                    //    else { return RedirectToAction("ErrorPage"); }
                    //}

                    //model.BankModel = _generalService.GetBankListBySchemeId((int)model.SchemeId);
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
        public ActionResult SaveDocumentForOnlineSchemeForm(SchemeFormViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var data = _schemeRepository.SaveDocumentsForOnlineSchemeApplication(model,files);
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
        public ActionResult PrintHousingSchemeForm(SchemeFormViewModel model)
        {
            string _PrintFormContent = string.Empty;
            SchemeFormViewModel formModel = new SchemeFormViewModel();
            formModel = _schemeRepository.GetOnlineSchemeApplicationFormById(model.ApplicationFormId);
            if (formModel != null)
            {
                var templatePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["HousingSchemeFormTemplatePath"]);
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
            return RedirectToAction("PreviewSchemeForm", "Housing", new { area = "Online", id = OnlineSchemeHelper.Encode(form.ApplicationFormId.ToString()) });
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
                formchallan.TextFormFeeGST = Convert.ToDecimal(formchallan.FormModel.FormFeeGST).ToString("#,##0.00");
                formchallan.TextApplicationFee = Convert.ToDecimal(formchallan.FormModel.ApplicationFee).ToString("#,##0.00");
                formchallan.TextFormFeeSGST = Convert.ToDecimal(formchallan.FormModel.FormFeeSGST).ToString("#,##0.00");
                formchallan.TextFormFeeCGST = Convert.ToDecimal(formchallan.FormModel.FormFeeCGST).ToString("#,##0.00");
                formchallan.TextProcessingCharge = Convert.ToDecimal(formchallan.FormModel.ProcessingCharge).ToString("#,##0.00");
                formchallan.TextProcessingSGST = Convert.ToDecimal(formchallan.FormModel.ProcessingSGST).ToString("#,##0.00");
                formchallan.TextProcessingCGST = Convert.ToDecimal(formchallan.FormModel.ProcessingCGST).ToString("#,##0.00");
                formchallan.TextProcessingGST = Convert.ToDecimal(formchallan.FormModel.ProcessingChargeGST).ToString("#,##0.00");
                formchallan.TextEarnestMoney = Convert.ToDecimal(formchallan.FormModel.EarnestMoney).ToString("#,##0.00");
                formchallan.TextTotalAmountGST = Convert.ToDecimal(formchallan.FormModel.TotalAmountGST).ToString("#,##0.00");
                formchallan.TotalAmountGSTInWords = OnlineSchemeHelper.ConvertNumberIntoWords(Convert.ToInt64(formchallan.FormModel.TotalAmountGST));
                
                var templatePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["HousingTemplatePath"]) + "SchemeChallanTemplate.cshtml";
                challan = RazorParser.ParseSchemeFormChallanTemplate(formchallan, templatePath);
                //challan = RazorParser.ParseTemplate(formchallan, "ApplicationForRTGSRecieptTemplate.cshtml");
            }
            return Json(challan, JsonRequestBehavior.AllowGet);
        }
    }
}
