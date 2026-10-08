using NA.PMS.OnlineScheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NA.PMS.Web.Areas.Online.Controllers
{
    public class SchemePaymentController : Controller
    {
        // GET: Online/SchemePayment

        private IOnlineSchemeRepository _schemeRepository;
        public SchemePaymentController()
        {
            _schemeRepository = new OnlineSchemeRepository();
        }

        [AllowAnonymous]
        public ActionResult Receipt(FormCollection form)
        {
            var paidmodel = new SchemePaymentViewModel();
            if (form != null)
            {
                //paidmodel = _schemeRepository.UpdateChallanOnlinePaymentTransaction(form);
                paidmodel = _schemeRepository.SaveOnlineSchemePaymentTransaction(form);
                if (paidmodel.ReturnTypeId == OSReturnTypeId.Success)
                {
                    TempData["Success"] = "Payment completed successfully";
                    return View(paidmodel);
                }
                else if (paidmodel.ReturnTypeId == OSReturnTypeId.Failed)
                {
                    TempData["FailedTraxaction"] = "Payment request failed";
                    return RedirectToAction("Index", "Payment", new { area = "Online", rid = form["udf2"], id = form["udf3"] });
                }
                else if (paidmodel.ReturnTypeId == OSReturnTypeId.Mismatch)
                {
                    TempData["MismatchTraxaction"] = "Transaction key did not match";
                    return RedirectToAction("Index", "Payment", new { area = "Online", rid = form["udf2"], id = form["udf3"] });
                }
                else
                {
                    TempData["ErrorInTraxaction"] = "Error in Transaction";
                    //return RedirectToAction("Index", "Payment", new { area = "Online", rid = form["udf2"], id = form["udf3"] });
                    return RedirectToAction("BankChallan", "Authority", new { area = "Member" });
                }
            }
            else
            {
                TempData["ErrorInTraxaction"] = "Error in Transaction";
                return RedirectToAction("BankChallan", "Authority", new { area = "Member" });
            }
            //return View();
        }

        [AllowAnonymous]
        public ActionResult FormChallan(SchemeFormViewModel model)
        {
            if (model.ApplicationFormId != null && model.ApplicationFormId > 0)
            {
                int ID = Convert.ToInt32(OnlineSchemeHelper.Decode(model.EncryptedFormId));
                if (ID > 0)
                {
                    //var objOnlinePaymentModel = _onlineService.GetOfflinePayment_Trans(ID);
                    var _payment = _schemeRepository.GetOnlineSchemePaymentTransactionDetailById(new SchemePaymentViewModel { ApplicationFormId = ID });
                    SchemeFormViewModel _form = new SchemeFormViewModel();
                    if (_payment != null)
                    {
                        if (string.IsNullOrEmpty(_payment.TransactionKey))
                        {
                            _form = _payment.SchemeForm;
                            _form.SchemePayment = _payment;
                            _form.SchemePayment.TransactionId = "";
                            _form.SchemePayment.EntryDate = null;
                            _form.ApplicationFormId = ID;
                            //_form.AppType = AppType;
                            return View(_form);
                        }
                    }
                    
                }
            }
            return RedirectToAction("SchemeInformation", "Application");
        }

        [AllowAnonymous]
        public ActionResult SaveSchemeFormPaidChallan(SchemeFormViewModel form, HttpPostedFileBase files)
        {
            form = _schemeRepository.SaveSchemeFormPaidChallanDetail(form, files);
            return Json(form, JsonRequestBehavior.AllowGet);
        }


    }
}
