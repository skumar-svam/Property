using Kendo.Mvc.UI;
using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Service;
using NA.PMS.Service.TemplateParser;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using NA.PMS.Web.Controllers;

namespace NA.PMS.Web.Areas.Online.Controllers
{
    public class PaymentController : WebBaseController
    {
        IGeneralService _generalService;
        ISchemeService _schemeService;
        IMastersService _masterService;
        IOnlineService _onlineService;
        ITemplateParserService _templateParserService;
        public string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
        public PaymentController(IGeneralService generalService, ISchemeService schemeService, IMastersService masterService, IOnlineService onlineService, TemplateParserService templateParserService)
        {
            _generalService = generalService;
            _schemeService = schemeService;
            _masterService = masterService;
            _onlineService = onlineService;
            _templateParserService = templateParserService;
        }

        [AllowAnonymous]
        public ActionResult Index(int? rid, int? id, string actionflag)
        {
            if (rid != null && rid > 0 && id != null && id > 0)
            {
                OnlinePaymentViewModel data = _onlineService.GetGeneratedChallanDetailById(new OnlinePaymentViewModel { RegistrationId = rid, OnlineRequestId = id, ChallanId = id, ActionType = actionflag });
                return View(data);
            }
            else
            {
                return RedirectToAction("ManageChallan", "Authority", new { area = "Member" });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(OnlinePaymentViewModel model)
        {
            _onlineService.SaveChallanOnlinePaymentTransaction(model);
            return View();
        }

        [AllowAnonymous]
        public ActionResult Receipt(FormCollection form)
        {
            var paidmodel = new OnlinePaymentViewModel();
            if (form != null)
            {
                paidmodel = _onlineService.UpdateChallanOnlinePaymentTransaction(form);
                if (paidmodel.ReturnTypeId == ReturnType.Success)
                {
                    TempData["Success"] = "Payment completed successfully";
                    return View(paidmodel);
                }
                else if (paidmodel.ReturnTypeId == ReturnType.Failed)
                {
                    TempData["FailedTraxaction"] = "Payment request failed";
                    return RedirectToAction("Index", "Payment", new { area = "Online", rid = form["udf2"], id = form["udf3"] });
                }
                else if (paidmodel.ReturnTypeId == ReturnType.Mismatch)
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
                return RedirectToAction("BankChallan", "Authority", new { area = "Member"});
            }
            //return View();
        }

        [AllowAnonymous]
        public void OnlinePayment(int id, int banktype)
        {
            OnlineFormViewModel form = _onlineService.GetOnlineApplicationFormById(id);
            OnlinePaymentViewModel payment = _onlineService.SaveOnlinePaymentTransaction(form);
            PaymentGateway gateway = new PaymentGateway();
            if (banktype == 1)//indusind
            {
                gateway.PayOnline(payment);
            }
            if (banktype == 2) //hdfc
            {
                gateway.PayOnlineHDFC(payment);
            }

            //string transactionId = PaymentGateway.GenerateTransactionId();
            //gateway.PayOnline(transactionId,payment.FirstName,payment.Amount.ToString(),payment.ProductInfo,payment.Email,payment.Mobile);
        }

        [AllowAnonymous]
        public void BrochureOnlinePayment(int id, int banktype)
        {
            OnlineFormViewModel form = _onlineService.GetOnlineApplicationFormById(id);
            OnlinePaymentViewModel payment = _onlineService.SaveOnlinePaymentTransaction(form);
            PaymentGateway gateway = new PaymentGateway();
            if (banktype == 1)//indusind
            {
                gateway.PayOnline(payment);
            }
            if (banktype == 2) //hdfc
            {
                gateway.PayOnlineHDFC(payment);
            }

            //string transactionId = PaymentGateway.GenerateTransactionId();
            //gateway.PayOnline(transactionId,payment.FirstName,payment.Amount.ToString(),payment.ProductInfo,payment.Email,payment.Mobile);
        }
        [AllowAnonymous]
        public ActionResult OnlineReceipt(FormCollection form)
        {
            var PaymentModel = new OnlinePaymentViewModel();
            try
            {
                if (form != null)
                {
                    PaymentModel = _onlineService.UpdateOnlinePaymentTransaction(form);
                }

                //string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";
                string HashSequence = ConfigurationManager.AppSettings["hashSequence"];

                if (form["status"].ToString() == "success")
                {
                    string[] paramArr = HashSequence.Split('|');
                    Array.Reverse(paramArr);
                    string paramSequence = ConfigurationManager.AppSettings["SALT"] + "|" + form["status"].ToString();
                    foreach (string param in paramArr)
                    {
                        paramSequence += "|";
                        paramSequence = paramSequence + (form[param] != null ? form[param] : "");
                    }

                    string hashSequence = PaymentGateway.GenerateSHA512HashCode(paramSequence).ToLower();
                    if (hashSequence != form["hash"])
                    {
                        TempData["MismatchTraxaction"] = "Transaction key did not match";
                        return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                        //Response.Write("Hash value did not matched");
                    }
                    else
                    {
                        //PaymentModel = _onlineService.UpdateOnlinePaymentTransaction(form);
                        TempData["Success"] = "Payment success";
                        //MTchange
                        OnlineFormViewModel applicationFormDetail = _onlineService.GetOnlineApplicationFormById(PaymentModel.ApplicationFormId);
                        if (applicationFormDetail.EarnestMoney == 0)
                        {
                            return RedirectToAction("BrochureDownloadForm", "Application", new { area = "Online", id = PaymentModel.ApplicationFormId });
                        }
                        //mTchange
                        return RedirectToAction("Acknowledgement", new { id = PaymentModel.EncryptedFormId });
                    }
                }
                else
                {
                    TempData["FailedTraxaction"] = "Payment request failed";
                    return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                    //Response.Write("Payment request failed.");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorInTraxaction"] = ex.Message;
                return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
                //Response.Write("<span style='color:red'>" + ex.Message + "</span>");
            }
            //return RedirectToAction("Acknowledgement", new { id = PaymentModel.ApplicationFormId});
        }

        [AllowAnonymous]
        public ActionResult Acknowledgement(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int ID = Convert.ToInt32(CommonHelper.Decode(id));
                if (ID > 0 && ID != null)
                {
                    OnlineFormViewModel model = new OnlineFormViewModel();
                    model = _onlineService.GetOnlineApplicationFormById(ID);
                    return View(model);
                }
            }
            return RedirectToAction("RegisterForm", "Application", new { area = "Online" });
        }

        [AllowAnonymous]
        public ActionResult DownloadApplicationForm(int appId, int departmentId)
        {
            var lst = _generalService.DownloadApplicationFormat(appId, Convert.ToInt32(LetterTemplate.ApplicationFormat), departmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }


        #region Generate Challan
        [AllowAnonymous]
        public ActionResult GenerateChallan()
        {
            return View();
        }

        //Save challan detils in db condider paramenter rid,accountHeadId,AccountSubHeadId,Amount
        [AllowAnonymous]
        public JsonResult SaveCreateChallan(int rId, int AccountHeadId, int AccountSubHeadId, decimal? Amount)
        {
            rId = rId != null ? rId : 0;
            var flag = _onlineService.SaveCreateChallan(rId, AccountHeadId, AccountSubHeadId, Amount);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        //Method for Online Application Form
        [AllowAnonymous]
        public ActionResult GeneratePaymentChallan(OnlineChallanViewModel objOnlineChallanViewModel)
        {
            //int rId, int bankId, int branchId, string DdlAccountNumber, int? deptId
            string challan = string.Empty;
            ChallanModel requestModel = _onlineService.GeneratePaymentChallan(objOnlineChallanViewModel);
            if (requestModel != null)
            {
                challan = _templateParserService.GetParsedHTML(requestModel, "PaymentChallanTemplate.cshtml");
                bool flag = _onlineService.SaveGeneratedChallan(requestModel.ChallanId, challan);
            }
            return Json(challan, JsonRequestBehavior.AllowGet);
        }

        //To get Account Number base on bankid and branchid
        [AllowAnonymous]
        public JsonResult GetAccountNumber(int bankId, int branchId)
        {
            var data = _onlineService.GetAccountNumber(bankId, branchId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //To remove challan charge deatils base on rid and challanTransID
        [AllowAnonymous]
        public JsonResult RemoveChallanChargeDetail(int rid, string headName, string subHeadName, decimal amount)
        {
            rid = rid != null ? rid : 0;
            var data = _onlineService.RemoveChallanChargeDetail(rid, headName, subHeadName, amount);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetGeneratedChallanDetails([DataSourceRequest] DataSourceRequest request, int rid)
        {
            List<OnlineChallanViewModel> dataresult = _onlineService.GetGeneratedChallanDetails(rid);
            if (dataresult == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = dataresult.ToDataSourceResult(request);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        #region
        //Method for Generate Challan for scheme
        [AllowAnonymous]
        public ActionResult GenerateChallanForScheme(OnlineFormViewModel objOnlineModel)
        {
            string challan = string.Empty;
            OnlineChallanViewModel requestModel = _onlineService.GenerateSchemeChallan(objOnlineModel);
            if (requestModel != null)
            {
                challan = _templateParserService.GetParsedHTML(requestModel, "SchemeChallanTemplate.cshtml");
                //bool flag = _onlineService.SaveGeneratedChallan(requestModel.ChallanId, challan);
            }
            return Json(challan, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetBankListforOnline(int schemeId)
        {
            var banks = _generalService.GetBankListBySchemeId(schemeId);
            return Json(banks, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetAllBranchsforOnline(int bankId)
        {
            var branchs = _generalService.GetAllBranchs(bankId);
            return Json(branchs, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetAccountNo(int schemeId, int bankId)
        {
            var AccountNo = _generalService.GetAccountBranchBySchemeIdBankId(schemeId, bankId);
            return Json(AccountNo, JsonRequestBehavior.AllowGet);
        }
        #endregion

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

                    return flag = true;
                }
            }
            return flag;
        }

        [AllowAnonymous]
        public ActionResult ErrorPage()
        {
            OnlineFormViewModel model = _onlineService.GetInitialDataForScheme();
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult UpdateOfflinePayment()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult OfflinePayment(string ApplicationFormId, string AppType)
        {
            //Commented to update challan details from admin(4 oct 2017)
            ////if scheme end date exceed from current date returns true.
            //if (!CheckSchemeCredentials())
            //{
            if (!string.IsNullOrEmpty(ApplicationFormId))
            {
                int ID = Convert.ToInt32(CommonHelper.Decode(ApplicationFormId));
                if (ID > 0)
                {
                    var objOnlinePaymentModel = _onlineService.GetOfflinePayment_Trans(ID);
                    OnlineFormViewModel objOnlineFormViewModel = new OnlineFormViewModel();
                    if (string.IsNullOrEmpty(objOnlinePaymentModel.TransactionKey))
                    {
                        if (objOnlinePaymentModel != null)
                        {
                            objOnlineFormViewModel = objOnlinePaymentModel.FormModel;
                            objOnlineFormViewModel.PaymentModel = objOnlinePaymentModel;
                            objOnlineFormViewModel.PaymentModel.TransactionId = "";
                            objOnlineFormViewModel.PaymentModel.EntryDate = null;
                            objOnlineFormViewModel.ApplicationFormId = ID;
                            objOnlineFormViewModel.AppType = AppType;
                            return View("UpdateOfflinePayment", objOnlineFormViewModel);
                        }
                    }
                }
            }
            return RedirectToAction("SchemeInformation", "Application");
            //}
            //else
            //{
            //    return RedirectToAction("ErrorPage");
            //}
        }

        [AllowAnonymous]
        public JsonResult GetApplicationFormIdForOfflinePayment([DataSourceRequest] DataSourceRequest request, int? ApplicationFormId)
        {
            var data = _onlineService.GetApplicationFormIdForOfflinePayment(request, ApplicationFormId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetChallanDetailsTrans(int? ApplicationFormId)
        {
            var data = _onlineService.GetOfflinePayment_Trans((int)ApplicationFormId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetPreviousChallanDetailsTrans(int? ApplicationFormId)
        {
            var data = _onlineService.GetPreviousChallanPayment_Trans((int)ApplicationFormId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult SubmitOfflinePayment(OnlineFormViewModel objOnlineFormViewModel, HttpPostedFileBase files)
        {
            var data = _onlineService.UpdateOfflinePayment(objOnlineFormViewModel, files);
            if (data == "2")
            {
                TempData["OfflinePayment_RTGS"] = "Transaction ID not exist or wrong";
                return RedirectToAction("OfflinePayment", "Payment", new { ApplicationFormId = CommonHelper.Encode(objOnlineFormViewModel.PaymentModel.ApplicationFormId.ToString()), area = "Online" });
            }

            if (data == "Updated Successfully")
            {
                if (objOnlineFormViewModel.AppType == Constants.AppType)
                {
                    //Post Request to NIC Service              
                    objOnlineFormViewModel.ApplicationFormId = objOnlineFormViewModel.PaymentModel.ApplicationFormId;
                    string message = PostReturn_CUSID_STATUS(objOnlineFormViewModel, ServiceStatus.FEE_PENDING, ServiceStatus_Text.FEE_PENDING);
                    if (message == "Failure")
                    {
                        TempData["ServiceMessage"] = "Service Failure";
                    }
                }
            }
            else
            {
                TempData["ServiceMessage"] = "Service Failure";
            }
            TempData["OfflinePayment_RTGS"] = data;
            return RedirectToAction("SchemeInformation", "Application", new { area = "Online" });
        }

        [AllowAnonymous]
        public ActionResult GenerateChallanRTGSForScheme(OnlineFormViewModel objOnlineModel)
        {
            string challan = string.Empty;
            OnlineChallanViewModel requestModel = _onlineService.GenerateSchemeChallan(objOnlineModel);
            var data = _onlineService.GetOnlineApplicationFormById(objOnlineModel.ApplicationFormId);
            if (requestModel != null)
            {
                if (data.DepartmentId != NADepartment.Institutional)
                {
                    requestModel.nFormFeeGST = Convert.ToDecimal(requestModel.FormModel.FormFeeGST).ToString("#,##0.00");
                    requestModel.nApplicationFee = Convert.ToDecimal(requestModel.FormModel.ApplicationFee).ToString("#,##0.00");
                    requestModel.nFormFeeSGST = Convert.ToDecimal(requestModel.FormModel.FormFeeSGST).ToString("#,##0.00");
                    requestModel.nFormFeeCGST = Convert.ToDecimal(requestModel.FormModel.FormFeeCGST).ToString("#,##0.00");
                }
                else
                {
                    requestModel.FormModel.TotalAmountGST = requestModel.FormModel.TotalAmountGST - (Convert.ToDecimal(requestModel.FormModel.FormFeeGST));
                }
                //if (data.SchemeId == 164164 || data.SchemeId == 162162) //made by vishal
                //{
                //    requestModel.nFormFeeGST = "0.00";
                //    requestModel.nApplicationFee = "0.00";
                //    requestModel.nFormFeeSGST = "0.00";
                //    requestModel.nFormFeeCGST = "0.00";
                //    requestModel.nProcessingCharge = Convert.ToDecimal(requestModel.FormModel.ProcessingCharge).ToString("#,##0.00");
                //    requestModel.nProcessingSGST = Convert.ToDecimal(requestModel.FormModel.ProcessingSGST).ToString("#,##0.00");
                //    requestModel.nProcessingCGST = Convert.ToDecimal(requestModel.FormModel.ProcessingCGST).ToString("#,##0.00");
                //    requestModel.nProcessingGST = Convert.ToDecimal(requestModel.FormModel.ProcessingChargeGST).ToString("#,##0.00");
                //    requestModel.nEarnestMoney = Convert.ToDecimal(requestModel.FormModel.EarnestMoney).ToString("#,##0.00");
                //    requestModel.nTotalAmountGST=(Convert.ToDecimal(requestModel.FormModel.TotalAmountGST) - (Convert.ToDecimal(requestModel.FormModel.FormFeeGST) + Convert.ToDecimal(requestModel.FormModel.ApplicationFee) + Convert.ToDecimal(requestModel.FormModel.FormFeeSGST) + Convert.ToDecimal(requestModel.FormModel.FormFeeCGST))).ToString("#,##0.00");
                //    //requestModel.nTotalAmountGST = Convert.ToDecimal(requestModel.FormModel.TotalAmountGST).ToString("#,##0.00");
                //    requestModel.TotalAmountGSTInWords = ApplicationHelper.Rupees(Convert.ToInt64(requestModel.nTotalAmountGST));
                //}
                //else
                //{ 
                //For Currency Format
                //requestModel.nFormFeeGST = Convert.ToDecimal(requestModel.FormModel.FormFeeGST).ToString("#,##0.00");
                //requestModel.nApplicationFee = Convert.ToDecimal(requestModel.FormModel.ApplicationFee).ToString("#,##0.00");
                //requestModel.nFormFeeSGST = Convert.ToDecimal(requestModel.FormModel.FormFeeSGST).ToString("#,##0.00");
                //requestModel.nFormFeeCGST = Convert.ToDecimal(requestModel.FormModel.FormFeeCGST).ToString("#,##0.00");
                requestModel.nProcessingCharge = Convert.ToDecimal(requestModel.FormModel.ProcessingCharge).ToString("#,##0.00");
                requestModel.nProcessingSGST = Convert.ToDecimal(requestModel.FormModel.ProcessingSGST).ToString("#,##0.00");
                requestModel.nProcessingCGST = Convert.ToDecimal(requestModel.FormModel.ProcessingCGST).ToString("#,##0.00");
                requestModel.nProcessingGST = Convert.ToDecimal(requestModel.FormModel.ProcessingChargeGST).ToString("#,##0.00");
                requestModel.nEarnestMoney = Convert.ToDecimal(requestModel.FormModel.EarnestMoney).ToString("#,##0.00");
                requestModel.nTotalAmountGST = Convert.ToDecimal(requestModel.FormModel.TotalAmountGST).ToString("#,##0.00");
                requestModel.TotalAmountGSTInWords = ApplicationHelper.Rupees(Convert.ToInt64(requestModel.FormModel.TotalAmountGST));
                //}
                challan = _templateParserService.GetParsedHTML(requestModel, "ApplicationForRTGSRecieptTemplate.cshtml");

            }
            return Json(challan, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult UploadPreviousChallan(OnlineFormViewModel objOnlineFormViewModel, HttpPostedFileBase files)
        {
            var data = _onlineService.SaveUploadPreviousChallan(objOnlineFormViewModel, files);
            if (data > 0)
            {
                if (objOnlineFormViewModel.AppType == Constants.AppType)
                {
                    //Post Request to NIC Service              
                    objOnlineFormViewModel.TotalAmount = objOnlineFormViewModel.PaymentModel.Amount;
                    string message = PostReturn_CUSID_STATUS(objOnlineFormViewModel, ServiceStatus.FEE_PENDING, ServiceStatus_Text.FEE_PENDING);
                    if (message == "Failure")
                    {
                        TempData["ServiceMessage"] = "Service Failure";
                    }
                }
            }
            else
            {
                TempData["ServiceMessage"] = "Service Failure";
            }
            return RedirectToAction("PreviewForm", "Application", new { area = "Online", id = CommonHelper.Encode(objOnlineFormViewModel.ApplicationFormId.ToString()) });
        }

        //model(Applicationid,Amount,Paytype) , Status Code,Remarks
        private string PostReturn_CUSID_STATUS(OnlineFormViewModel model, string ServiceStatusCode, string ServiceStatusRemarks)
        {
            string message = string.Empty;
            var data = _onlineService.GetNICSingleWindowData(model);
            if (data != null)
            {
                WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel();
                objWReturn_CUSID_STATUSModel.ControlID = data.Control_ID;
                objWReturn_CUSID_STATUSModel.ApplicationID = Convert.ToString(model.ApplicationFormId);
                objWReturn_CUSID_STATUSModel.ProcessIndustryID = Convert.ToString(model.ApplicationFormId);
                objWReturn_CUSID_STATUSModel.UnitID = data.Unit_Id;
                objWReturn_CUSID_STATUSModel.ServiceID = data.ServiceID;
                objWReturn_CUSID_STATUSModel.Status_Code = ServiceStatusCode;
                objWReturn_CUSID_STATUSModel.Remarks = ServiceStatusRemarks;
                objWReturn_CUSID_STATUSModel.Fee_Status = ServiceStatusRemarks;
                objWReturn_CUSID_STATUSModel.Fee_Amount = Convert.ToString(model.TotalAmount);
                objWReturn_CUSID_STATUSModel.passsalt = Passalt;

                //Update status of Single Window
                //# verify from Service
                //#update details to db
                if (Convert.ToInt32(model.PayType) == Constants.singleWindowPortalApplicationPayment)
                {
                    GetWGetUBPaymentDetails(objWReturn_CUSID_STATUSModel);
                }

                //Pass Object to NIC Service
                message = GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);
            } return message;
        }

        [AllowAnonymous]
        public JsonResult SingleWindowPortalPayment(OnlineFormViewModel objOnlineFormViewModel)
        {
            int result = ReturnType.None;
            string message = string.Empty;

            //Get data for NIC table
            var data = _onlineService.GetNICSingleWindowData(objOnlineFormViewModel);
            if (data != null)
            {
                WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel();
                objWReturn_CUSID_STATUSModel.ControlID = data.Control_ID;
                objWReturn_CUSID_STATUSModel.UnitID = data.Unit_Id;
                objWReturn_CUSID_STATUSModel.ServiceID = data.ServiceID;
                objWReturn_CUSID_STATUSModel.ProcessIndustryID = data.ProcessIndustryID;
                objWReturn_CUSID_STATUSModel.ApplicationID = Convert.ToString(objOnlineFormViewModel.ApplicationFormId);
                objWReturn_CUSID_STATUSModel.Fee_Amount = Convert.ToString(objOnlineFormViewModel.TotalAmount);
                objWReturn_CUSID_STATUSModel.Remarks = "UB";
                objWReturn_CUSID_STATUSModel.Status_Code = "12";
                objWReturn_CUSID_STATUSModel.Fee_Status = "UB";
                objWReturn_CUSID_STATUSModel.passsalt = Passalt;

                objWReturn_CUSID_STATUSModel.Transaction_ID = string.Empty;
                objWReturn_CUSID_STATUSModel.Transaction_Date = string.Empty;
                objWReturn_CUSID_STATUSModel.Transaction_Date_Time = string.Empty;
                objWReturn_CUSID_STATUSModel.NOC_Certificate_Number = string.Empty;
                objWReturn_CUSID_STATUSModel.NOC_URL = string.Empty;
                objWReturn_CUSID_STATUSModel.ISNOC_URL_ActiveYesNO = string.Empty;
                string path = string.Empty;
                string xmlInputData = string.Empty;
                string xmlOutputData = string.Empty;

                //Send Fee Status to Nic
                using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
                {
                    message = client.WReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel.ControlID, objWReturn_CUSID_STATUSModel.UnitID, objWReturn_CUSID_STATUSModel.ServiceID, objWReturn_CUSID_STATUSModel.ProcessIndustryID, objWReturn_CUSID_STATUSModel.ApplicationID, objWReturn_CUSID_STATUSModel.Status_Code, objWReturn_CUSID_STATUSModel.Remarks, objWReturn_CUSID_STATUSModel.Fee_Amount, objWReturn_CUSID_STATUSModel.Fee_Status, objWReturn_CUSID_STATUSModel.Transaction_ID, objWReturn_CUSID_STATUSModel.Transaction_Date, objWReturn_CUSID_STATUSModel.Transaction_Date_Time, objWReturn_CUSID_STATUSModel.NOC_Certificate_Number, objWReturn_CUSID_STATUSModel.NOC_URL, objWReturn_CUSID_STATUSModel.ISNOC_URL_ActiveYesNO, objWReturn_CUSID_STATUSModel.passsalt);
                }

                if (message == "SUCCESS")
                {
                    NewDataSet objNewDataSet = new NewDataSet();
                    decimal totalAmount = Convert.ToDecimal(objWReturn_CUSID_STATUSModel.Fee_Amount);
                    int statusCode = Convert.ToInt32(objWReturn_CUSID_STATUSModel.Status_Code);
                    objNewDataSet.Table = new Table();
                    objNewDataSet.Table.OnlineApplicationId = (int)objOnlineFormViewModel.ApplicationFormId;
                    objNewDataSet.Table.Fee_Amount = Convert.ToString(totalAmount);
                    objNewDataSet.Table.Status_Code = Convert.ToString(statusCode);
                    objNewDataSet.Table.Fee_Status = objWReturn_CUSID_STATUSModel.Fee_Status;
                    objNewDataSet.Table.Payment_Through = 1;
                    objNewDataSet.Table.Payment_Description = "Single Window Portal";
                    result = _onlineService.SaveNICSingleWindowPayment(objNewDataSet);
                    TempData["ServiceMessage"] = "Fee Status sent to Consolidate Payment through Single Window Portal.";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Retur Status
        public string GetWReturn_CUSID_STATUS(WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel)
        {
            objWReturn_CUSID_STATUSModel.ProcessIndustryID = string.Empty;
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

        //Get Valid Payment for Single Window Service 
        public int GetWGetUBPaymentDetails(WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel)
        {
            int flag = ReturnType.None;
            string xmlInputData = string.Empty;
            string xmlOutputData = string.Empty;
            if (!string.IsNullOrEmpty(objWReturn_CUSID_STATUSModel.ControlID) && !string.IsNullOrEmpty(objWReturn_CUSID_STATUSModel.UnitID) && !string.IsNullOrEmpty(objWReturn_CUSID_STATUSModel.ServiceID) && !string.IsNullOrEmpty(objWReturn_CUSID_STATUSModel.passsalt))
            {
                NewDataSet objNewDataSet = new NewDataSet();
                using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
                {
                    System.Data.DataSet result = client.WGetUBPaymentDetails(objWReturn_CUSID_STATUSModel.ControlID, objWReturn_CUSID_STATUSModel.UnitID, objWReturn_CUSID_STATUSModel.ServiceID, objWReturn_CUSID_STATUSModel.passsalt);
                    xmlInputData = result.GetXml();
                    Deserial objHelp = new Deserial();
                    objNewDataSet = objHelp.Deserialize<NewDataSet>(xmlInputData);
                }
                if (objNewDataSet != null)
                {
                    OnlineFormViewModel objOnlineFormViewModel = new OnlineFormViewModel();
                    objOnlineFormViewModel.PaymentModel = new OnlinePaymentViewModel();
                    objOnlineFormViewModel.ApplicationFormId = Convert.ToInt32(objWReturn_CUSID_STATUSModel.ApplicationID);
                    objOnlineFormViewModel.PaymentModel.TransactionId = objNewDataSet.Table.Transaction_ID;
                    if (objOnlineFormViewModel.ApplicationFormId > 0 && !string.IsNullOrEmpty(objOnlineFormViewModel.PaymentModel.TransactionId))
                    {
                        flag = _onlineService.UpdatePaymentTransaction_SingleWindowPortal(objOnlineFormViewModel);
                    }
                }
            }
            return flag;
        }
        #endregion


        [AllowAnonymous]
        public JsonResult UpdateChallanStatus(string AppId, string PaymentType, string Amount)
        {
            bool UpdateChallanStatus = false;
            int _AppId = 0;
            int ApplicationId = 0;
            if (int.TryParse(Convert.ToString(AppId), out _AppId))
                ApplicationId = _AppId;

            int _PaymentType = 0;
            int PType = 0;
            if (int.TryParse(Convert.ToString(PaymentType), out _PaymentType))
                PType = _PaymentType;

            int _Amount = 0;
            int Amt = 0;
            if (int.TryParse(Convert.ToString(Amount), out _Amount))
                Amt = _Amount;

            if (PType == Constants.offlineApplicationPayment || PType == Constants.offlinePrevoiusChallanApplicationPayment)
            {
                UpdateChallanStatus = _onlineService.UpdateChallanStatus(ApplicationId, PType);
            }
            else if (PType == Constants.singleWindowPortalApplicationPayment) { UpdateChallanStatus = true; }
            if (UpdateChallanStatus == true)
            {
                if (PType == Constants.offlineApplicationPayment || PType == Constants.offlinePrevoiusChallanApplicationPayment || PType == Constants.singleWindowPortalApplicationPayment)
                {
                    //Post Request to NIC Service     
                    OnlineFormViewModel objOnlineFormViewModel = new OnlineFormViewModel();
                    objOnlineFormViewModel.ApplicationFormId = ApplicationId;
                    objOnlineFormViewModel.TotalAmount = Amt;
                    objOnlineFormViewModel.PayType = Convert.ToString(PType);
                    string message = PostReturn_CUSID_STATUS(objOnlineFormViewModel, ServiceStatus.FEE_PAID, ServiceStatus_Text.FEE_PAID);
                    if (message == "Failure")
                    {
                        TempData["ServiceMessage"] = "Service Failure";
                    }
                }
            }
            return Json(UpdateChallanStatus, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateInfo()
        {
            return View();
        }

        public JsonResult GetOnlinePaidChallanIdListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            var list = _onlineService.GetOnlinePaidChallanIdListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlineTransactionIdListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            var list = _onlineService.GetOnlineTransactionIdListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlinePaymentDetailById(OnlinePaymentViewModel model)
        {
            var data = _onlineService.GetOnlinePaymentDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateOnlinePaymentDetailById(OnlinePaymentViewModel model)
        {
            int flag = _onlineService.UpdateOnlinePaymentDetailById(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}