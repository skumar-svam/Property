using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Model;
using NA.PMS.Service;
using NA.PMS.Service.BusinessRuleEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Common;
using NA.PMS.Service.TemplateParser;
using NA.PMS.Web.Controllers;

namespace NA.PMS.Web.Areas.Revenue.Controllers
{
    public class PaymentController : WebBaseController
    {
        private IGeneralService _generalService;
        private IPaymentEngine _paymentEngine;
        private IRevenueService _revenueService;
        private ITemplateParserService _templateParser;
        public PaymentController(IGeneralService generalService, IPaymentEngine paymentEngine, IRevenueService revenueService, ITemplateParserService templateParser)
        {
            _generalService = generalService;
            _paymentEngine = paymentEngine;
            _revenueService = revenueService;
            _templateParser = templateParser;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PaymentList()
        {
            return View();
        }

        public ActionResult PremiumPaid()
        {
            return View();
        }

        public ActionResult Payment(string id)
        {
            if (id != null)
            {
                var ridNo = CommonHelper.Decode(id);
            }
            return View();
        }

        public ActionResult PaymentDetail()
        {
            return View();
        }

        public ActionResult UpdateInfo()
        {
            return View();
        }

        public ActionResult ManageInstallment()
        {
            return View();
        }

        public ActionResult ManageLeaseRent()
        {
            return View();
        }

        public ActionResult Installment()
        {
            return View();
        }

        public ActionResult InstallmentDues()
        {
            return View();
        }

        public ActionResult ManageSchedule()
        {
            return View();
        }

        public ActionResult PremiumSchedule()
        {
            return View();
        }

        public ActionResult Premium()
        {
            return View();
        }

        public ActionResult InstallmentSchedule()
        {
            return View();
        }

        public ActionResult InstallmentReSchedule()
        {
            return View();
        }

        public ActionResult LeaseRent(int? id)
        {
            LeaseRentViewModel model = new LeaseRentViewModel();
            if (id != null && id > 0)
            {
                model = _revenueService.GetPaymentLeaseRentDetailsById((int)id);
                if (model.IsActive == true) { return RedirectToAction("ManageLeaseRent", "Payment", new { area = "Revenue" }); }
            }
            return View(model);
        }

        public ActionResult InstallmentRequest()
        {
            return View();
        }

        public ActionResult LeaseRentRequest()
        {
            return View();
        }

        public ActionResult Miscellaneous()
        {
            return View();
        }

        public ActionResult Manage()
        {
            LeaseRentViewModel model = _revenueService.GetMiscellaneousPaymentCount();
            return View(model);
        }

        public ActionResult ManageLeaseRentDashboard()
        {
            LeaseRentViewModel model = new LeaseRentViewModel();
            return View(model);
        }

        public ActionResult ManageScheduleDues()
        {
            //LeaseRentAndInstallmentDashboard model = _revenueService.GetTotalCountByActionType();
            return View();
        }

        public ActionResult Calculator()
        {
            return View();
        }

        public ActionResult ManageDemandNote()
        {
            return View();
        }

        public ActionResult ManageDemand()
        {
            return View();
        }

        public ActionResult DemandNote()
        {
            PaymentViewModel model = new PaymentViewModel();
            model.DemandNoteContent = "<table><tr><td colspan='3' style='text-align:center'><h2>New Okhla Industrial Development Authority</h2></td></tr></table>";
            return View();
        }

        public ActionResult Update()
        {
            return View();
        }

        public ActionResult ManageLedger()
        {
            return View();
        }

        public ActionResult UpdateChallan()
        {
            return View();
        }

        public ActionResult ManageDefaulterList()
        {
            return View();
        }

        public JsonResult GetPartialOrFullPaymentDetails([DataSourceRequest]DataSourceRequest request, string departmentId, string isPremiumPaid, string isLeaseRentPaid)
        {
            DataSourceResult detail = _paymentEngine.GetPartialOrFullPaymentDetails(request, departmentId, isPremiumPaid, isLeaseRentPaid);
            return Json(detail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyPaymentDetails_Read([DataSourceRequest]DataSourceRequest request)
        {
            var paymentDetails = _paymentEngine.GetPropertyPaymentDetails_Read(request);
            //var data = rentList.ToDataSourceResult(request);
            return Json(paymentDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyPaymentDetails([DataSourceRequest]DataSourceRequest request, PaymentViewModel model)
        {
            var paymentDetails = _revenueService.GetPropertyPaymentDetails(request, model);
            return Json(paymentDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateRegistrationIdByPropertyNo(PaymentViewModel model)
        {
            var flag = _revenueService.UpdateRegistrationIdByPropertyNo(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReceiptDetailsById([DataSourceRequest]DataSourceRequest request, long? receiptId)
        {
            var list = _revenueService.GetReceiptDetailsById(request, receiptId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SavePaymentByRegistrationId(PaymentViewModel model)
        {
            PaymentViewModel data = _paymentEngine.SavePaymentByRegistrationId(model);
            return RedirectToAction("Index");
        }

        public ActionResult GetYesNoStatus()
        {
            var deptList = _generalService.GetYesNoStatus();
            return Json(deptList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentFrequencyList()
        {
            var list = _paymentEngine.GetPaymentFrequencyList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDepartmentList()
        {
            //var deptList = _generalService.GetAllDepartments();
            var deptList = _generalService.GetDepartmentList();
            return Json(deptList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegistrationIdByDepartment(int? departmentId)
        {
            var ridList = _generalService.GetRegistrationIdByDepartment(departmentId);
            return Json(ridList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBankNamesForPayment([DataSourceRequest] DataSourceRequest request)
        {
            //List<DynamicDataModel> banks = _generalService.GetBankNamesForPayment();
            //var data = banks.ToDataSourceResult(request);
            var banks = _generalService.GetBankList();
            return Json(banks.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveMiscellaneousPayment(PaymentViewModel model)
        {
            //PaymentViewModel data = _paymentEngine.SavePropertyPayment(model);
            int flag = _revenueService.SaveMiscellaneousPayment(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddPaymentDetailTemp(string rid, int? receiptId, int? headId, int? subHeadId, int? bankId, decimal? amount)
        {
            int flag = _revenueService.AddPaymentDetailTemp(rid, receiptId, headId, subHeadId, bankId, amount);
            return null;
        }

        public JsonResult RemovePaymentDetailTempById(string id)
        {
            List<PaymentViewModel> TempModel = (List<PaymentViewModel>)Session["PaymentModel"];
            if (TempModel != null)
            {

            }
            return null;
        }

        public JsonResult GetAccountHeadList([DataSourceRequest]DataSourceRequest request)
        {
            var list = _paymentEngine.GetAccountHeadList(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAccountSubHeadList([DataSourceRequest]DataSourceRequest request, int? id)
        {
            var list = _paymentEngine.GetAccountSubHeadList(request, id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentTypeList([DataSourceRequest]DataSourceRequest request)
        {
            var list = _paymentEngine.GetPaymentTypeList(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPaymentSubTypeList([DataSourceRequest]DataSourceRequest request, int? id)
        {
            var list = _paymentEngine.GetPaymentSubTypeList(request, id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegistrationIdListFromPaymentSchedule([DataSourceRequest]DataSourceRequest request)
        {
            var list = _paymentEngine.GetRegistrationIdListFromPaymentSchedule(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFrequencyList([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetFrequencyList(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentModeList([DataSourceRequest]DataSourceRequest request)
        {
            var list = _paymentEngine.GetPaymentModeList();
            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLeaseRentDetails([DataSourceRequest] DataSourceRequest request, LeaseRentViewModel model)
        {
            var list = _revenueService.GetLeaseRentDetails(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLeaseRentPaidListById([DataSourceRequest] DataSourceRequest request, LeaseRentViewModel model)
        {
            var list = _revenueService.GetLeaseRentPaidListById(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListValueByTotalCount([DataSourceRequest] DataSourceRequest request, LeaseRentViewModel model)
        {
            var list = _revenueService.GetListValueByTotalCount(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMultiplePaymentStatusList([DataSourceRequest] DataSourceRequest request, LeaseRentViewModel model)
        {
            var list = _revenueService.GetMultiplePaymentStatusList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegistrationIdList([DataSourceRequest] DataSourceRequest request)
        {
            //var list = _generalService.GetRegistrationIdList(request);
            var list = _generalService.GetRegistrationIdListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegistrationIdListFromLeaseRent([DataSourceRequest] DataSourceRequest request)
        {
            var list = _revenueService.GetRegistrationIdListFromLeaseRent(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegistrationIdListFromInstallmentPayment([DataSourceRequest] DataSourceRequest request)
        {
            var list = _revenueService.GetRegistrationIdListFromInstallmentPayment(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApproverIdList([DataSourceRequest] DataSourceRequest request)
        {
            var list = _generalService.GetApproverIdList(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApplicantDetailsByRegistrationId(int rid)
        {
            var list = _revenueService.GetApplicantDetailsByRegistrationId(rid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailsByIdForLeaseRent(LeaseRentViewModel model)
        {
            var list = _revenueService.GetDetailsByIdForLeaseRent(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveLeaseRentPremium(LeaseRentViewModel model)
        {
            int flag = _revenueService.SaveLeaseRentPremium(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMiscellaneousPaymentListById([DataSourceRequest] DataSourceRequest request, int? rid)
        {
            var list = _revenueService.GetMiscellaneousPaymentListById(request, rid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllottedPropertyDetailsById(int? rid)
        {
            var list = _revenueService.GetAllottedPropertyDetailsById(Convert.ToInt32(rid));
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatusList([DataSourceRequest] DataSourceRequest request)
        {
            var list = _generalService.GetStatusList();
            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeaseRentPaymentListById([DataSourceRequest] DataSourceRequest request, int? rid)
        {
            var list = _revenueService.GetLeaseRentPaymentListById(request, rid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInstallmentPaymentScheduleList([DataSourceRequest] DataSourceRequest request, PaymentScheduleModel model)
        {
            var list = _revenueService.GetInstallmentPaymentScheduleList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInstallmentPaymentListForApproval([DataSourceRequest] DataSourceRequest request, PaymentScheduleModel model)
        {
            var list = _revenueService.GetInstallmentPaymentListForApproval(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMiscellaneousPaymentList([DataSourceRequest] DataSourceRequest request, PaymentViewModel model)
        {
            var list = _revenueService.GetMiscellaneousPaymentList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CalculateLeaseRentPremium(LeaseRentViewModel model)
        {
            var leasedetail = _paymentEngine.CalculateLeaseRentPremium(model);
            return Json(leasedetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateLeaseRentDuesPayment(LeaseRentViewModel model)
        {
            var leasedetail = _revenueService.UpdateLeaseRentDuesPayment(model);
            return Json(leasedetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateInstallmentDuesPayment(LeaseRentViewModel model)
        {
            var leasedetail = _revenueService.UpdateInstallmentDuesPayment(model);
            return Json(leasedetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeaseRentDetailsById(LeaseRentViewModel model)
        {
            var leasedetail = _revenueService.GetLeaseRentDetailsById(model);
            return Json(leasedetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeaseRentDetailsByIdAndProcedure([DataSourceRequest]DataSourceRequest request, LeaseRentViewModel model)
        {
            var leasedetail = _revenueService.GetLeaseRentDetailsByIdAndProcedure(request, model);
            return Json(leasedetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateLeaseRentDuesPaymentById(LeaseRentViewModel model)
        {
            var leasedetail = _revenueService.UpdateLeaseRentDuesPaymentById(model);
            return Json(leasedetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InstallmentDuesPayment(LeaseRentViewModel model)
        {
            var installmentduespayment = _revenueService.InstallmentDuesPayment((int)model.RegistrationId);
            return Json(installmentduespayment, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentDuesByRegistrationId(LeaseRentViewModel model)
        {
            var leasedetail = _revenueService.GetPaymentDuesByRegistrationId(model);
            return Json(leasedetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateInstallmentDuesPaymentById(LeaseRentViewModel model)
        {
            var leasedetail = _revenueService.UpdateInstallmentDuesPaymentById(model);
            return Json(leasedetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRevenueGeneratedList([DataSourceRequest] DataSourceRequest request, LeaseRentViewModel model)
        {
            var list = _revenueService.GetRevenueGeneratedList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRevenueWithDefaulterList(LeaseRentViewModel model)
        {
            var list = _revenueService.GetRevenueWithDefaulterList(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLeaseRentTransDetails([DataSourceRequest] DataSourceRequest request, LeaseRentViewModel model)
        {
            var list = _revenueService.GetLeaseRentTransDetails(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyPremiumScheduleList([DataSourceRequest] DataSourceRequest request, PaymentScheduleModel model)
        {
            var list = _revenueService.GetPropertyPremiumScheduleList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyPremiumSchedulePaidListById([DataSourceRequest] DataSourceRequest request, PaymentScheduleModel model)
        {
            var list = _revenueService.GetPropertyPremiumSchedulePaidListById(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePropertyPremiumScheduleByRegistrationId(PaymentScheduleModel model)
        {
            int flag = _revenueService.SavePropertyPremiumScheduleByRegistrationId(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyPremiumScheduleInformationById(PaymentScheduleModel model)
        {
            PaymentScheduleModel info = _revenueService.GetPropertyPremiumScheduleInformationById(model);
            return Json(info, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemovePropertyPremiumScheduleById(PaymentScheduleModel model)
        {
            int flag = _revenueService.RemovePropertyPremiumScheduleById(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdatePropertyPremiumSchedulePaymentById([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PaymentScheduleModel> paySch)
        {
            var flag = ReturnType.None;
            try
            {
                if (paySch != null)
                {
                    foreach (var edit in paySch)
                    {
                        flag = _revenueService.UpdatePropertyPremiumSchedulePaymentById(request, edit);
                        if (flag != ReturnType.Updated)
                        {
                            ModelState.AddModelError("PaymentSchedule_Update", "Record Not Saved");
                        }
                    }
                }
                return Json(new[] { paySch }.ToDataSourceResult(request, ModelState));
            }
            catch (Exception ex)
            {
                //return RedirectToAction("error", "home");
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SavePremiumPaymentDuesByRegistrationId(PaymentScheduleModel model)
        {
            if (model.RegistrationId != null && model.RegistrationId != 0)
            {
                var data = _revenueService.SavePremiumPaymentDuesByRegistrationId(model);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(ReturnType.None, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveInstallmentDuesPaymentById(PaymentScheduleModel model)
        {
            int flag = _revenueService.SaveInstallmentDuesPaymentById(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult SaveInstallmentDuesPaymentById(PaymentScheduleModel model)
        //{
        //    int flag = _revenueService.SaveInstallmentDuesPaymentById(model);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetInstallmentDuesPaidListByRegistrationId([DataSourceRequest] DataSourceRequest request, PaymentScheduleModel model)
        {
            var list = _revenueService.GetInstallmentDuesPaidListByRegistrationId(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInstallmentDuesDetailByRegistrationId(PaymentScheduleModel model)
        {
            PaymentScheduleModel data = _revenueService.GetInstallmentDuesDetailByRegistrationId(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GeneratePremiumScheduleByDepartment(PaymentScheduleModel model)
        {
            int flag = _revenueService.GeneratePremiumScheduleByDepartment(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemovePremiumScheduleByDepartment(PaymentScheduleModel model)
        {
            int flag = _revenueService.RemovePremiumScheduleByDepartment(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GeneratePremiumScheduleByRegistrationId(PaymentScheduleModel model)
        {
            int flag = _revenueService.GeneratePremiumScheduleByRegistrationId(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMiscellaneousPaymentCountByDepartment(LeaseRentViewModel model)
        {
            var data = _revenueService.GetMiscellaneousPaymentCountByDepartment(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTotalCountByDepartment(LeaseRentViewModel model)
        {
            var data = _revenueService.GetTotalCountByDepartment(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetTotalCountByActionType(LeaseRentAndInstallmentDashboard model)
        {
            var data = _revenueService.GetTotalCountByActionType(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDemandNoteListAsDataSource([DataSourceRequest] DataSourceRequest request, PaymentViewModel model)
        {
            var data = _revenueService.GetDemandNoteListAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //for CEO
        public JsonResult GetDemandNoteListAsDataSourceII([DataSourceRequest] DataSourceRequest request, PaymentViewModel model)
        {
            var data = _revenueService.GetDemandNoteListAsDataSourceII(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateInstallmentDuesPaymentII(PaymentViewModel model)
        {
            int flag = _revenueService.UpdateInstallmentDuesPaymentII(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInstallmentDuesPaymentListById(DataSourceRequest request, PaymentViewModel model)
        {
            var flag = _revenueService.GetInstallmentDuesPaymentListById(request, model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeaserentDuesPaymentListById(DataSourceRequest request, PaymentViewModel model)
        {
            var flag = _revenueService.GetLeaserentDuesPaymentListById(request, model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInstallmentPaymentScheduleListById([DataSourceRequest] DataSourceRequest request, PaymentScheduleModel model)
        {
            var list = _revenueService.GetInstallmentPaymentScheduleListById(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateInstallmentDuesPaymentByIdII(PaymentScheduleModel model)
        {
            var leasedetail = _revenueService.UpdateInstallmentDuesPaymentByIdII(model);
            return Json(leasedetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateLeaseRentDuesPaymentByIdII(LeaseRentViewModel model)
        {
            var leasedetail = _revenueService.UpdateLeaseRentDuesPaymentByIdII(model);
            return Json(leasedetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeaseRentDuesPaymentById([DataSourceRequest] DataSourceRequest request, LeaseRentViewModel model)
        {
            var leasedetail = _revenueService.GetLeaseRentDuesPaymentById(request, model);
            return Json(leasedetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDuesPaymentDetails(PaymentViewModel model)
        {
            int flag = _revenueService.UpdateDuesPaymentDetails(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExportKendoEditorToPdf(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            return File(fileContents, contentType, fileName);
        }

        public JsonResult CreateCustomDemandNote(PaymentViewModel model)
        {
            model.DemandNoteTypeId = Constants.InstallmentAndLeaseRentDemandNoteId;
            var data = _templateParser.CreateCustomDemandNote(model, "DemandNoteTemplateEng.cshtml");
            //var data = _templateParser.CreateCustomDemandNoteInHindi(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeaseRentDetailsByRegistrationId(LeaseRentViewModel model)
        {
            var data = _revenueService.GetLeaseRentDetailsByRegistrationId(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateLeaseRentDetails(LeaseRentViewModel model)
        {
            int flag = _revenueService.UpdateLeaseRentDetails(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInstallmentDetailsByRegistrationId(LeaseRentViewModel model)
        {
            var data = _revenueService.GetInstallmentDetailsByRegistrationId(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateInstallmentDetails(LeaseRentViewModel model)
        {
            int flag = _revenueService.UpdateInstallmentRentDetails(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeaseRentDuesForDemandNote(PaymentViewModel model)
        {
            var leaserent = _revenueService.GetLeaseRentDuesForDemandNote(model);
            return Json(leaserent, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInstallmentDuesForDemandNote(PaymentViewModel model)
        {
            var leaserent = _revenueService.GetInstallmentDuesForDemandNote(model);
            return Json(leaserent, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDuesAmountForDemandNoteById(LeaseRentViewModel model)
        {
            var DuesDetail = _revenueService.GetDuesAmountForDemandNoteById(new PaymentViewModel { RegistrationId = model.RegistrationId });
            return Json(DuesDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetYearsForOneTimeLeaseRent([DataSourceRequest] DataSourceRequest request)
        {
            List<DropdownViewModel> list = new List<DropdownViewModel>();
            for (int i = 1; i <= 12; i++)
            {
                var model = new DropdownViewModel { Id = i, Text = i.ToString(), Value = i.ToString() };
                list.Add(model);
            }
            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeaseRentRequestList([DataSourceRequest] DataSourceRequest request, LeaseRentViewModel model)
        {
            var list = _revenueService.GetLeaseRentRequestList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDuesPaymentByDepartment(PaymentViewModel model)
        {
            int flag = _revenueService.UpdateDuesPaymentByDepartment(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyDetailById(PropertyViewModel model)
        {
            var data = _revenueService.GetPropertyDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsRegistrationIdExist(PropertyViewModel model)
        {
            var flag = _generalService.IsRegistrationIdExist(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaidAmountDetails([DataSourceRequest]DataSourceRequest request, PaymentViewModel model)
        {
            var paymentDetails = _revenueService.GetPaidAmountDetails(request, model);
            return Json(paymentDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateRegistrationIdByAllottmentNo(PaymentViewModel model)
        {
            var flag = _revenueService.UpdateRegistrationIdByAllottmentNo(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLeaseRentPaymentTransDetailsById([DataSourceRequest] DataSourceRequest request, LeaseRentViewModel model)
        {
            var list = _revenueService.GetLeaseRentPaymentTransDetailsById(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CalculateInstallmentDuesById(PaymentViewModel model)
        {
            var dues = _revenueService.CalculateInstallmentDuesById(model);
            return Json(dues, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveInstallmentScheduleTransByDate(PaymentScheduleModel model)
        {
            int flag = _revenueService.SaveInstallmentScheduleTransByDate(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsPaymentExistForRid(PaymentViewModel model)
        {
            var flag = _revenueService.IsPaymentExistForRid(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsPaymentExistForProperty(PaymentViewModel model)
        {
            var flag = _revenueService.IsPaymentExistForProperty(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGeneratedLedgerByRId(PaymentViewModel model)
        {
            var list = _templateParser.GetGeneratedLedgerByRId(model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReScheduledInstallmentAsDataSource([DataSourceRequest] DataSourceRequest request, PaymentScheduleModel model)
        {
            var list = _revenueService.GetReScheduledInstallmentAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyPaymentReScheduleInformationById(PaymentScheduleModel model)
        {
            PaymentScheduleModel info = _revenueService.GetPropertyPaymentReScheduleInformationById(model);
            return Json(info, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInstallmentScheduleTypeAsDataSource([DataSourceRequest] DataSourceRequest request, PaymentScheduleModel model)
        {
            var list = _revenueService.GetInstallmentScheduleTypeAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReceiptIdListByRId([DataSourceRequest]DataSourceRequest request, int? rId)
        {
            var list = _revenueService.GetReceiptIdListByRId(request, rId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReceiptHeadListByReceiptId([DataSourceRequest]DataSourceRequest request, long? receiptId)
        {
            var list = _revenueService.GetReceiptHeadListByReceiptId(request, receiptId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAlloteeListByDueDate([DataSourceRequest] DataSourceRequest request, LeaseRentAndInstallmentDashboard model)
        {
            var list = _revenueService.GetAlloteeListByDueDate(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }        

        //public JsonResult RemoveChallanDetailsById(int? Id)
        //{
        //    int flag = _revenueService.RemoveChallanDetailsById(Id);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult SaveTempChallanDetailsByReceiptId(PaymentViewModel model)
        //{
        //    int flag = _revenueService.SaveTempChallanDetailsByReceiptId(model);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetSavedTempChallanDetailByReceiptId([DataSourceRequest] DataSourceRequest request, PaymentViewModel model)
        //{
        //    var list = _revenueService.GetSavedTempChallanDetailByReceiptId(request, model);
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult SaveChallanDetails(PaymentViewModel model)
        //{
        //    int flag = _revenueService.SaveChallanDetails(model);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult SendReminderToAllottee(int? Rid)
        {
            var flag = _generalService.SendReminderToAllottee(Rid);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveNDCLetterDetails(NDCVeiwModel model)
        {
            int flag = _revenueService.SaveNDCLetterDetails(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateNDCStatus(NDCVeiwModel model)
        {
            int flag = _revenueService.UpdateNDCStatus(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckRequestIdForNDC(ServiceViewModel model)
        {
            var flag = _revenueService.CheckRequestIdForNDC(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetYearsList()
        {
            var list = _generalService.GetYearsList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDefaulterList([DataSourceRequest] DataSourceRequest request, DefaulterViewModel model)
        {
            var list = _revenueService.GetDefaulterList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}