using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Service;
using NA.PMS.Service.BusinessRuleEngine;
using NA.PMS.Service.TemplateParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Web.Models;
using NA.PMS.Web.Controllers;
using NA.PMS.Service.Reports;

namespace NA.PMS.Web.Areas.Member.Controllers
{
    public class AuthorityController : WebBaseController
    {
        IGeneralService _generalService;
        IRequestService _requestService;
        private IPaymentEngine _paymentEngine;
        private ITemplateParserService _templateParser;
        private IReportService _reportService;
        IOnlineService _onlineService;
        public AuthorityController(IGeneralService generalService, IRequestService requestService, IPaymentEngine paymentEngine, ITemplateParserService templateParser, IReportService reportService, IOnlineService onlineService)
        {
            _generalService = generalService;
            _requestService = requestService;
            _paymentEngine = paymentEngine;
            _templateParser = templateParser;
            _reportService = reportService;
            _onlineService = onlineService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Manage()
        {
            return View();
        }

        public ActionResult ManageChallan()
        {
            return View();
        }

        public ActionResult ValidateChallan()
        {
            return View();
        }

        public ActionResult ChallanDashboard()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Challan()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult BankChallan()
        {
            var userInfo = Session["CurrentUser"] as CurrentUserDetail;
            if (userInfo != null && userInfo.OptionalId != null)
            {
                var challan = new ChallanViewModel();
                challan.BankId = userInfo.OptionalId;
                return View(challan);
            }
            else return View();
        }

        public ActionResult EmployeeChallan()
        {
            return View();
        }

        public ActionResult ManageLetter()
        {
            return View();
        }

        public ActionResult Letter()
        {
            return View();
        }

        public ActionResult ManageNoting()
        {
            return View();
        }

        public ActionResult Noting()
        {
            return View();
        }

        public ActionResult ManageEmail()
        {
            return View();
        }

        public ActionResult ManageLitigation()
        {
            return View();
        }

        public ActionResult Litigation()
        {
            return View();
        }

        public ActionResult ManageBankAccount()
        {
            return View();
        }

        public ActionResult ManageServices()
        {
            return View();
        }

        public ActionResult ManagePaymentReceipt()
        {
            return View();
        }

        public ActionResult RegisterCustomer()
        {
            return View();
        }

        public ActionResult KYADashboard()
        {
            return View();
        }

        public ActionResult ManageKYA()
        {
            return View();
        }

        public ActionResult ValidateKYA()
        {
            return View();
        }

        public ActionResult NDCForm()
        {
            return View();
        }

        public ActionResult ManageNDCFormApproval()
        {
            return View();
        }

        public ActionResult ViewNDCApproval(int? Id)
        {
            NDCVeiwModel ndcDetail = new NDCVeiwModel();
            ndcDetail = _requestService.GetNDCDetailsById(Id);
            return View(ndcDetail);
        }

        [AllowAnonymous]
        public ActionResult KYAReports()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ServiceDashboard()
        {
            return View();
        }



        public ActionResult OnlinePayment()
        {
            return View();
        }

        public ActionResult EditNDCLetter(int? Id)
        {
            NDCVeiwModel ndcDetail=new NDCVeiwModel();
            ndcDetail = _requestService.GetNDCDetailsById(Id);
            return View(ndcDetail);
        }

        [AllowAnonymous]
        public ActionResult ChallanVerification()
        {
            return View();
        }

        public ActionResult KYADetail(int? id)
        {
            KYAViewModel model = new KYAViewModel();
            if (id != null && id > 0)
            {
                //KYAViewModel model = new KYAViewModel();
                model.Id = id;
                var KYADetails = _requestService.GetKYADetailsById(model);
                return View(KYADetails);
            }
            return View(model);
        }

        public ActionResult RedirectManageKYA(int? id, string status)
        {
            if (id != null) Session["KYADepartmentId"] = id;
            if (status != null) Session["KYAStatus"] = status;
            return RedirectToAction("ManageKYA");
        }

        public JsonResult GetDocumentListByRegistrationId([DataSourceRequest]DataSourceRequest request, int? rid,int? id)
        {
            var docList = _requestService.GetDocumentListByRegistrationId(request, rid,id);
            return Json(docList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKYADetailsById(KYAViewModel model)
        {
            var KYADetails = _requestService.GetKYADetailsById(model);
            return Json(KYADetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetResourceMessageList()
        {
            var list = _generalService.GetResourceMessageList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetRegistrationIdListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetRegistrationIdListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDepartmentListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetDepartmentListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetBankListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetBankListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetBranchListOfBankAsDataSource([DataSourceRequest]DataSourceRequest request, int? bankId)
        {
            var list = _generalService.GetBranchListOfBankAsDataSource(request, bankId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetAccountHeadList([DataSourceRequest]DataSourceRequest request)
        {
            //var list = _paymentEngine.GetAccountHeadList(request);
            var list = _generalService.GetReceiptHeadListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetAccountSubHeadList([DataSourceRequest]DataSourceRequest request, int? id)
        {
            //var list = _paymentEngine.GetAccountSubHeadList(request, id);
            var list = _generalService.GetReceiptSubHeadListAsDataSource(request, id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetAllottedPropertyDetailByRegistrationId(int? rid)
        {
            if (rid != null && rid != 0)
            {
                var details = _generalService.GetAllottedPropertyDetailByRegistrationId(rid);
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SaveTempChallanChargeDetail(ChallanViewModel model)
        {
            int flag = _generalService.SaveTempChallanChargeDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SaveTempChallanChargeDetailII(ChallanViewModel model)
        {
            int flag = _generalService.SaveTempChallanChargeDetailII(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult RemoveTempChallanChargeDetail(ChallanViewModel model)
        {
            int flag = _generalService.RemoveTempChallanChargeDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult RemoveTempChallanChargeDetailII(ChallanViewModel model)
        {
            int flag = _generalService.RemoveTempChallanChargeDetailII(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetTempChallanChargesAsDataSource([DataSourceRequest]DataSourceRequest request, ChallanViewModel model)
        {
            var list = _generalService.GetTempChallanChargesAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetTempChallanChargesAsDataSourceII([DataSourceRequest]DataSourceRequest request, ChallanViewModel model)
        {
            var list = _generalService.GetTempChallanChargesAsDataSourceII(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetGeneratedChallanByRegistrationId(ChallanViewModel model)
        {
            var list = _templateParser.GetGeneratedChallanByRegistrationId(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GenerateChallanForPaymentById(ChallanViewModel model)
        {
            var list = _templateParser.GenerateChallanForPaymentById(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetGeneratedChallanByPropertyNo(ChallanViewModel model)
        {
            var list = _templateParser.GetGeneratedChallanByPropertyNo(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetGeneratedChallanByChallanId(ChallanViewModel model)
        {
            var list = _templateParser.GetGeneratedChallanByChallanId(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGeneratedChallanList([DataSourceRequest]DataSourceRequest request, ChallanViewModel model)
        {
            var list = _requestService.GetGeneratedChallanList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGeneratedChallanAmountList([DataSourceRequest]DataSourceRequest request, int? challanId)
        {
            var list = _requestService.GetGeneratedChallanAmountListById(request, challanId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGeneratedLetterList([DataSourceRequest]DataSourceRequest request, LetterViewModel model)
        {
            var list = _requestService.GetGeneratedLetterList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGeneratedLetterListById([DataSourceRequest]DataSourceRequest request, int? rid)
        {
            var list = _requestService.GetGeneratedLetterListById(request, rid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLetterTemplateListByDepartmentAsDataSource([DataSourceRequest]DataSourceRequest request, int? departmentId)
        {
            var list = _generalService.GetLetterTemplateListByDepartmentAsDataSource(request, departmentId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceRequestValidationForLetter(int? rid, int? letterId)
        {
            var lst = _generalService.GetServiceRequestStatusForLetter(rid, letterId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerateAuthorizedLetterById(LetterViewModel model)
        {
            var lst = _generalService.GenerateAuthorizedLetterById(model);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGeneratedLetterByBarcode(LetterViewModel model)
        {
            var letter = _generalService.GetGeneratedLetterByBarcode(model);
            return Json(letter, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendRegisteredMessageToApplicant(ApplicantViewModel model)
        {
            var flag = _generalService.SendRegisteredMessageToApplicant(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNotingFileDetailList([DataSourceRequest]DataSourceRequest request, NotingViewModel model)
        {
            var data = _requestService.GetNotingFileDetailList(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNotingFileDetailListById([DataSourceRequest]DataSourceRequest request, NotingViewModel model)
        {
            var data = _requestService.GetNotingFileDetailListById(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApproverIdList([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetApproverIdList(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOSDApproverIdList([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetOSDApproverIdList(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNotingFileTypeList([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetNotingFileTypeListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveNotingFileContent(NotingViewModel model)
        {
            int flag = _requestService.SaveNotingFileContent(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNotingFileDetailById(NotingViewModel model)
        {
            var data = _requestService.GetNotingFileDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBankAccountDetailList([DataSourceRequest]DataSourceRequest request, BankAccountViewModel model)
        {
            var list = _requestService.GetBankAccountDetailList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBranchDetailListByBankId([DataSourceRequest]DataSourceRequest request, int? bankId)
        {
            var list = _requestService.GetBankAccountDetailList(request, new BankAccountViewModel { BankId = bankId, FilterType = "Branch" });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAuthorityServiceDetailList([DataSourceRequest]DataSourceRequest request, ServiceViewModel model)
        {
            var list = _requestService.GetAuthorityServicesDetailList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReceiptHeadListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetReceiptHeadListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReceiptSubHeadListAsDataSource([DataSourceRequest]DataSourceRequest request, int? receiptId)
        {
            var list = _generalService.GetReceiptSubHeadListAsDataSource(request, receiptId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentReceiptHeadAsDataSource([DataSourceRequest]DataSourceRequest request, PaymentViewModel model)
        {
            var list = _requestService.GetPaymentReceiptHeadAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentReceiptSubHeadAsDataSource([DataSourceRequest]DataSourceRequest request, PaymentViewModel model)
        {
            var list = _requestService.GetPaymentReceiptSubHeadAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ActivateBanckAccountStatus(BankAccountViewModel model)
        {
            int flag = _requestService.ActivateBanckAccountStatus(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ActivatePaymentReceiptStatus(PaymentViewModel model)
        {
            int flag = _requestService.ActivatePaymentReceiptStatus(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ActivateAuthorityServiceStatus(ServiceViewModel model)
        {
            int flag = _requestService.ActivateAuthorityServiceStatus(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSectorListAsDataSource([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var list = _generalService.GetSectorListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBlockListAsDataSource([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var list = _generalService.GetBlockListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetSectorListAsDataSourceII([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var list = _generalService.GetSectorListAsDataSourceII(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetBlockListAsDataSourceII([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var list = _generalService.GetBlockListAsDataSourceII(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSecurityQuestionListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetSecurityQuestioinListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetBankListAsDataSource([DataSourceRequest]DataSourceRequest request)
        //{
        //    var list = _generalService.GetBankListAsDataSource(request);
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetBranchListOfBankAsDataSource([DataSourceRequest]DataSourceRequest request, int? bankId)
        //{
        //    var list = _generalService.GetBranchListOfBankAsDataSource(request, bankId);
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetBankAccountDetailAsDataSource([DataSourceRequest]DataSourceRequest request, BankAccountViewModel model)
        {
            var list = _generalService.GetBankAccountDetailAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetBankAccountDetailById(BankAccountViewModel model)
        {
            BankAccountViewModel list = _generalService.GetBankAccountDetailById(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveBankAccountDetail(BankAccountViewModel model)
        {
            int flag = _generalService.SaveBankAccountDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceDetailById(ServiceViewModel model)
        {
            ServiceViewModel data = _generalService.GetServiceDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveServiceDetailById(ServiceViewModel model)
        {
            int data = _generalService.SaveServiceDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyServiceTypeList(int departmentId)
        {
            var list = _generalService.GetServiceListByDepartment(departmentId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceTypeListAsDataSource([DataSourceRequest]DataSourceRequest request, DropdownViewModel model)
        {
            var list = _generalService.GetServiceTypeListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePropertyServiceType(ServiceViewModel model)
        {
            int flag = _requestService.SavePropertyServiceType(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePaymentReceiptHeadDetail(PaymentViewModel model)
        {
            int flag = _requestService.SavePaymentReceiptHeadDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKYASubmittedFormList([DataSourceRequest]DataSourceRequest request, KYAViewModel model)
        {
            var list = _requestService.GetKYASubmittedFormList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetKYADetailStatusList([DataSourceRequest]DataSourceRequest request, KYAViewModel model)
        {
            var list = _requestService.GetKYADetailStatusList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKYAFormListForValidation([DataSourceRequest]DataSourceRequest request, KYAViewModel model)
        {
            var list = _requestService.GetKYAFormListForValidation(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateKYAForm(KYAViewModel model)
        {
            int flag = _requestService.ValidateKYAForm(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetKYAFormListCount(KYAViewModel model)
        {
            var list = _requestService.GetKYAFormListCount(model);
            IEnumerable<KYAViewModel> list2 = list;
            return Json(list2, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public decimal GetAverageApprovedForm()
        {
            var data = _requestService.GetAverageApprovedForm();
            return data;
        }

        public JsonResult GetStatusMasterAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            //var status = _generalService.GetStatusMasterAsDataSource(request);
            //var status = _generalService.GetStatusList();
            //var list = new List<DropdownViewModel>();
            //list.Add(status[0]);
            //list.Add(status[1]);
            //list.Add(status[3]);
            //list.Add(status[12]);
            ////status.RemoveRange(5, 5);
            //return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            var user = Session["CurrentUser"] as CurrentUserDetail;
            if (user != null)
            {
                var list = new List<DropdownViewModel>();
                if (user.RoleMaster.RoleInDepartment != null && user.RoleMaster.RoleInDepartment == RoleInDepartment.Assistant)
                {
                    list.Add(new DropdownViewModel { Id = NAStatusId.Forwarded, Text = "Forward for Approval" });
                    list.Add(new DropdownViewModel { Id = NAStatusId.Forwarded, Text = "Forward for Rejection" });
                }
                else if (user.RoleMaster.RoleInDepartment != null && user.RoleMaster.RoleInDepartment == RoleInDepartment.OSD)
                {
                    list.Add(new DropdownViewModel { Id = NAStatusId.Approved, Text = "Approved" });
                    list.Add(new DropdownViewModel { Id = NAStatusId.Rejected, Text = "Rejected" });
                }
                else
                {
                    var status = _generalService.GetStatusList();
                    //list = _generalService.GetStatusList();
                    list.Add(status[0]);
                    list.Add(status[1]);
                    list.Add(status[3]);
                    list.Add(status[12]);
                }
                return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(_generalService.GetStatusMasterAsDataSource(request), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDocumentPath(KYAViewModel model)
        {
            FtpHandler ftp = new FtpHandler();
            //string path = ftp.GetDocumentPath(model.RegistrationId, string.Empty, true);
            bool IsOldFormat = false;
            string path = FtpHandler.GetDocumentPathForKYA(model.RegistrationId.ToString(), Constants.KYA, model.Id.ToString(), string.Empty, true, IsOldFormat);
            List<string> allFiles = ftp.DirSearch(path);
            if (allFiles.Count == 0)
            {
                path = FtpHandler.GetDocumentPathForKYA(model.RegistrationId.ToString(), Constants.KYA, model.Id.ToString(), string.Empty, true, true);
                allFiles = ftp.DirSearch(path);
                IsOldFormat = allFiles.Count > 0 ? true : false;
            }
            string fileName = string.Empty;
            if (allFiles != null && allFiles.Count > 0)
            {
                if (model.ActionId == 1)
                {
                    var file = allFiles.Count > 0 ? allFiles[0] : null;
                    if (file != null) fileName = FtpHandler.GetDocumentPathForKYA(model.RegistrationId.ToString(), Constants.KYA, model.Id.ToString(), file, false, IsOldFormat);
                }
                if (model.ActionId == 2)
                {
                    var file = allFiles.Count > 1 ? allFiles[1] : null;
                    if (file != null) fileName = FtpHandler.GetDocumentPathForKYA(model.RegistrationId.ToString(), Constants.KYA, model.Id.ToString(), file, false, IsOldFormat);
                }
                if (model.ActionId == 3)
                {
                    if (allFiles.Count > 2)
                    {
                        var file = allFiles[2];
                        if (file != null) fileName = FtpHandler.GetDocumentPathForKYA(model.RegistrationId.ToString(), Constants.KYA, model.Id.ToString(), file, false, IsOldFormat);
                    }
                    else fileName = null;// string.Empty;
                }
            }
            return Json(fileName, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetMasterSearchParameterAsDataSource([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var data = _generalService.GetMasterSearchParameterAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetPropertyDetailById(PropertyViewModel model)
        {
            var data = _generalService.GetPropertyDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOptionalReasonAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var status = _generalService.GetOptionalReasonAsDataSource(request);
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult CheckChallanSessionDataById(ChallanViewModel model)
        {
            int flag = _generalService.CheckChallanSessionDataById(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult VerifyChallanDetailById(ChallanViewModel model)
        {
            int flag = _generalService.VerifyChallanDetailById(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceTypeDetailById(DropdownViewModel model)
        {
            DropdownViewModel data = _generalService.GetServiceTypeDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailsForNDC(PaymentViewModel model)
        {
            var details = _requestService.GetDetailsForNDC(model);
            return Json(details, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNDCGeneratedListAsDataSource([DataSourceRequest]DataSourceRequest request, NDCVeiwModel model)
        {
            var data = _requestService.GetNDCGeneratedListAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNDCListForApprovalAsDataSource([DataSourceRequest]DataSourceRequest request, NDCVeiwModel model)
        {
            var data = _requestService.GetNDCListForApprovalAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateRemarksForNDCLetter(NDCVeiwModel model)
        {
            var flag = _requestService.UpdateRemarksForNDCLetter(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNDCLetter(NDCVeiwModel model)
        {
            string letter = _requestService.GetNDCLetter(model);
            return Json(letter, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateNDCLetterDetails(NDCVeiwModel model)
        {
            var flag = _requestService.UpdateNDCLetterDetails(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDemandAndNDCListByDepartmentAsDataSource([DataSourceRequest]DataSourceRequest request, NDCVeiwModel model)
        {
            var data = _reportService.GetDemandAndNDCListByDepartmentAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNDCAndDemandReportsForGraph(NDCVeiwModel model)
        {
            var data = _reportService.GetNDCAndDemandReportsForGraph(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChallanReportsForGraph(ChallanViewModel model)
        {
            var data = _reportService.GetChallanReportsForGraph(model);
            if (model.FilterType == "ChallanForGrid" || model.FilterType == "ChallanByAmount")
            {
                DataSourceRequest request = new DataSourceRequest();
                return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
               return Json(data, JsonRequestBehavior.AllowGet);
        }

        #region EmployeeChallan (salaryBill)
        public JsonResult GetEmployeeListAsdatasource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetEmployeeListAsdatasource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeDetailsById(EmployeeViewModel model)
        {
            EmployeeViewModel list = _generalService.GetEmployeeDetailsById(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveTempChallanChargeDetailForEmployee(ChallanViewModel model)
        {
            int flag = _generalService.SaveTempChallanChargeDetailForEmployee(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveTempChallanChargeDetailForEmployee(ChallanViewModel model)
        {
            int flag = _generalService.RemoveTempChallanChargeDetailForEmployee(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGeneratedChallanForEmployee(ChallanViewModel model)
        {
            var list = _templateParser.GetGeneratedChallanForEmployee(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTotalAmount(decimal? amount)
        {
            decimal total = _generalService.GetTotalAmount(amount);
            return Json(total, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTempChallanChargesForEmployee([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetTempChallanChargesForEmployee(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult GetOnlinePaymentDetailsAsDataSource([DataSourceRequest]DataSourceRequest request, OnlinePaymentViewModel model)
        {
            var list = _onlineService.GetOnlinePaymentDetailsAsDataSource(request, model);
            return Json(list,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlinePaymentDetailById(OnlinePaymentViewModel model)
        {
            var data = _onlineService.GetOnlinePaymentDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetBankListForPaymentAsDataSource([DataSourceRequest]DataSourceRequest request, DropdownViewModel model)
        {
            var list = _generalService.GetBankListForPaymentAsDataSource(request,model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}