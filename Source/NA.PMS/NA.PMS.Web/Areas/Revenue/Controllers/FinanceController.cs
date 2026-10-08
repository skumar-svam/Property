using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Service;
using NA.PMS.Service.BusinessRuleEngine;
using NA.PMS.Service.TemplateParser;
using NA.PMS.Web.Controllers;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NA.PMS.Web.Areas.Revenue.Controllers
{
    public class FinanceController : WebBaseController
    {
        private IGeneralService _generalService;
        private IPaymentEngine _paymentEngine;
        private IRevenueService _revenueService;
        ITemplateParserService _templateParserService;
        public FinanceController(IGeneralService generalService, IPaymentEngine paymentEngine, IRevenueService revenueService, TemplateParserService templateParserService)
        {
            _generalService = generalService;
            _paymentEngine = paymentEngine;
            _revenueService = revenueService;
            _templateParserService = templateParserService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Manage()
        {
            LeaseRentViewModel rmodel = _revenueService.GetMiscellaneousPaymentCount();
            PaymentViewModel model = new PaymentViewModel();
            model.TotalLeaseRentCount = rmodel.TotalLeaseRentCount;
            model.TotalNDCCount = rmodel.TotalNDCCount;
            model.TotalIPCount = rmodel.TotalIPCount;
            return View(model);
        }

        public ActionResult LeaseRentReport(string actionType)
        {
            LeaseRentViewModel model = new LeaseRentViewModel();
            model.ActionType = actionType;
            return View(model);
        }

        public ActionResult Report(string financialYear)
        {
            LeaseRentViewModel model = new LeaseRentViewModel();
            model.FinancialYear = financialYear;
            return View(model);
        }

        public ActionResult PremiumReport()
        {
            return View();
        }

        public ActionResult AccountDetail()
        {
            return View();
        }

        public ActionResult AllotmentDetail()
        {
            return View();
        }

        public ActionResult PaymentDetail(int? id)
        {
            if (id != null && id > 0 && id.Value.ToString().Length == 8)
            {
                PaymentViewModel model = _revenueService.GetMasterPaymentDetailByRegistrationId(id);
                return View(model);
            }
            else return View();
        }

        public ActionResult PropertyCost()
        {
            return View();
        }

        public ActionResult ManagePropertyCost()
        {
            if (Session["AdvanceSearchAccount"] != null)
            {
                AdvanceSearchModel model = (AdvanceSearchModel)Session["AdvanceSearchAccount"];
                PropertyViewModel property = new PropertyViewModel { DepartmentId = model.DepartmentId, SectorId = model.SectorId, RegistrationId = model.RegistrationId, BlockId = model.BlockId };
                return View(property);
            }
            else
            {
                return View();
            }
        }

        public ActionResult ManageExcessArea()
        {
            return View();
        }

        public ActionResult ExcessArea()
        {
            return View();
        }

        public ActionResult ManageAccount()
        {
            return View();
        }

        public ActionResult ManageDuesDetail()
        {
            return View();
        }

        public ActionResult ManageLocationCharge()
        {
            return View();
        }

        public ActionResult LeaseRentPayment(int? Id)
        {
            LeaseRentViewModel model = new LeaseRentViewModel();
            if (Id > 0)
            {
                model = _revenueService.GetLeaseRentPaymentDetailById((int)Id);
                //objLeaseDeedModel = _revenueService.GetLeaseDeedDetailsById((int)Id);
                if (model.IsActive == true) { return RedirectToAction("ManageDuesDetail", "Finance", new { area = "Revenue" }); }
            }
            return View(model);
        }

        public ActionResult ManageDuesRequest()
        {
            return View();
        }

        public ActionResult ManageCompensation()
        {
            return View();
        }

        public ActionResult Compensation()
        {
            return View();
        }

        public ActionResult NDC()
        {
            return View();
        }

        public ActionResult LeaseRentDetail(int? Id)
        {
            var data = (CurrentUserDetail)Session["CurrentUser"];
            LeaseRentViewModel model = new LeaseRentViewModel();
            if (Id > 0 && data != null)
            {
                model = _revenueService.GetLeaseRentPaymentDetailById((int)Id);
                model.User = data.UserID.ToString();
            }
            else { return RedirectToAction("ManageDuesDetail", "Finance", new { area = "Revenue" }); }
            return View(model);
        }

        public ActionResult GetLeaseRentDuesDetail([DataSourceRequest] DataSourceRequest request, LeaseRentViewModel model)
        {
            var list = _revenueService.GetLeaseRentDuesDetail(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLeaseRentDuesRequest([DataSourceRequest] DataSourceRequest request)
        {
            var list = _revenueService.GetLeaseRentDuesRequest(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegistrationNoByDepartment([DataSourceRequest] DataSourceRequest request)
        {
            var lst = _generalService.GetRegistrationIdByDepartment(request);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetYesNoStatus()
        {
            var stats = _generalService.GetYesNoStatus();
            return Json(stats, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApplicantDetailsForNDC(string registrationId)
        {
            NDCVeiwModel details = _generalService.GetApplicantDetailsForNDC(registrationId);
            return Json(details, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyDetailsById(PropertyViewModel model)
        {
            var details = _generalService.GetPropertyDetailById(model);
            return Json(details, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyDetailByRegistrationId(int? registrationId)
        {
            var details = _generalService.GetAllottedPropertyDetailByRegistrationId(registrationId);
            return Json(details, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveLeaseRentPaymentByRegistrationId(LeaseRentViewModel model)
        {
            var data = _revenueService.SaveLeaseRentPaymentByRegistrationId(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveLeaseRentApprovalStatus(LeaseRentViewModel model)
        {
            var data = _revenueService.SaveLeaseRentApprovalStatus(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LeaseDeedResendforApproval(LeaseDeedModel ObjLeaseDeedModel)
        {
            var data = _revenueService.LeaseDeedResendforApproval(ObjLeaseDeedModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetDepartmentList([DataSourceRequest] DataSourceRequest Req)
        {
            var departments = _generalService.GetAllDepartmentList(Req);
            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyAccountDetails([DataSourceRequest] DataSourceRequest request, PaymentViewModel model)
        {
            var data = _revenueService.GetPropertyAccountDetails(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeaseRentReportDetails([DataSourceRequest] DataSourceRequest request, LeaseRentViewModel model)
        {
            var data = _revenueService.GetLeaseRentReportDetails(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyPremiumReportDetails([DataSourceRequest] DataSourceRequest request, LeaseRentViewModel model)
        {
            var data = _revenueService.GetPropertyPremiumReportDetails(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintDemandNote(LeaseRentViewModel model)
        {
            string demandletter = string.Empty;
            if (model.RegistrationId != null && model.RegistrationId > 0)
            {
                var DuesDetail = _revenueService.GetDuesAmountForDemandNoteById(new PaymentViewModel { RegistrationId = model.RegistrationId, ActionDate = model.ActionDate, ActionType = model.ActionType,FilterType = model.FilterType });
                //DuesDetail.DuesUptoDate = DateTime.Now;// DateTime.Now.AddDays(10);
                DuesDetail.Notes = Constants.GSTNoticeInHindi;
                DuesDetail.ExtraTaxNotes = DuesDetail.DepartmentId == NADepartment.Institutional ? DepartmentInHindi.InstitutionalNote : string.Empty;
                DuesDetail.DemandNotes = DuesDetail.DepartmentId == NADepartment.Institutional ? DepartmentInHindi.DemandNote : string.Empty;
                DuesDetail.DemandNoteTypeId = Constants.InstallmentAndLeaseRentDemandNoteId;
                DuesDetail.Department = DuesDetail.DepartmentId == NADepartment.Institutional ? DepartmentInHindi.Institutional : (DuesDetail.DepartmentId == NADepartment.Industrial ? DepartmentInHindi.Industry : (DuesDetail.DepartmentId == NADepartment.Commercial ? DepartmentInHindi.Commercial : (DuesDetail.DepartmentId == NADepartment.Housing ? DepartmentInHindi.Housing : (DuesDetail.DepartmentId == NADepartment.Residential ? DepartmentInHindi.Residential : string.Empty))));
                demandletter = _templateParserService.GetParsedAndSavedDemandNote(DuesDetail, "DemandNoteTemplateHindi.cshtml");
             }
            return Json(demandletter, JsonRequestBehavior.AllowGet);           
        }

        public ActionResult PrintLeaseRentDuesDemandNote(PaymentViewModel model)
        {
            string demandletter = string.Empty;
            if (model.RegistrationId != null && model.RegistrationId > 0)
            {
                var DuesDetail = _revenueService.GetLeaseRentDuesForDemandNote(model);
                if (DuesDetail.KYAStatusId == null || DuesDetail.KYAStatusId != NAStatusId.Approved)
                {
                    demandletter = "Fill Your KYA Form";
                }

                //DuesDetail.DuesUptoDate = DuesDetail.DuesUptoDate != null ? DuesDetail.DuesUptoDate.Value.AddDays(10) : DateTime.Now.AddDays(10);
                DuesDetail.Notes = Constants.GSTNoticeInHindi;
                DuesDetail.ExtraTaxNotes = DuesDetail.DepartmentId == NADepartment.Institutional ? DepartmentInHindi.InstitutionalNote : string.Empty;
                DuesDetail.DemandNotes = DuesDetail.DepartmentId == NADepartment.Institutional ? DepartmentInHindi.DemandNote : string.Empty;
                DuesDetail.DemandNoteTypeId = Constants.LeaseRentDemandNoteId;
                DuesDetail.Department = DuesDetail.DepartmentId == NADepartment.Institutional ? DepartmentInHindi.Institutional : (DuesDetail.DepartmentId == NADepartment.Industrial ? DepartmentInHindi.Industry : (DuesDetail.DepartmentId == NADepartment.Commercial ? DepartmentInHindi.Commercial : (DuesDetail.DepartmentId == NADepartment.Housing ? DepartmentInHindi.Housing : (DuesDetail.DepartmentId == NADepartment.Residential ? DepartmentInHindi.Residential : string.Empty))));
                if (DuesDetail.ActionTypeId == ReturnType.Exist)
                {
                    demandletter = _templateParserService.GetParsedAndSavedDemandNote(DuesDetail, "DemandNoteTemplateHindi.cshtml");
                }
                else
                {
                    demandletter = DuesDetail.ActionTypeId == ReturnType.Other ? "No Current Dues" : (DuesDetail.ActionTypeId == ReturnType.NotExist ? "Update Lease Rent Dues." : string.Empty);
                }
            }
            
            return Json(demandletter, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintInstallmentDuesDemandNote(PaymentViewModel model)
        {
            string demandletter = string.Empty;
            if (model.RegistrationId != null && model.RegistrationId > 0)
            {
                var DuesDetail = _revenueService.GetInstallmentDuesForDemandNote(model);
                
                //DuesDetail.DuesUptoDate = DuesDetail.DuesUptoDate != null ? DuesDetail.DuesUptoDate.Value.AddDays(10) : DateTime.Now.AddDays(10);
                DuesDetail.Notes = Constants.GSTNoticeInHindi;
                DuesDetail.ExtraTaxNotes = DuesDetail.DepartmentId == NADepartment.Institutional ? DepartmentInHindi.InstitutionalNote : string.Empty;
                DuesDetail.DemandNotes = DuesDetail.DepartmentId == NADepartment.Institutional ? DepartmentInHindi.DemandNote : string.Empty;
                DuesDetail.DemandNoteTypeId = Constants.InstallmentDemandNoteId;
                DuesDetail.Department = DuesDetail.DepartmentId == NADepartment.Institutional ? DepartmentInHindi.Institutional : (DuesDetail.DepartmentId == NADepartment.Industrial ? DepartmentInHindi.Industry : (DuesDetail.DepartmentId == NADepartment.Commercial ? DepartmentInHindi.Commercial : (DuesDetail.DepartmentId == NADepartment.Housing ? DepartmentInHindi.Housing : (DuesDetail.DepartmentId == NADepartment.Residential ? DepartmentInHindi.Residential : string.Empty))));
                if (DuesDetail.ActionTypeId == ReturnType.Exist)
                {
                    demandletter = _templateParserService.GetParsedAndSavedDemandNote(DuesDetail, "DemandNoteTemplateHindi.cshtml");
                }
                else
                {
                    demandletter = DuesDetail.ActionTypeId == ReturnType.Paid ? "No Current Dues" : (DuesDetail.ActionTypeId == ReturnType.NotExist ? "Update Lease Rent Dues." : string.Empty);
                }
                //demandletter = _templateParserService.GetParsedAndSavedDemandNote(DuesDetail, "DemandNoteTemplateHindi.cshtml");
            }
            return Json(demandletter, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateDemandNoteInHindi(PaymentViewModel model)
        {
            string demandletter = string.Empty;
            if (model.RegistrationId != null && model.RegistrationId > 0)
            {
                model.DuesUptoDate = model.DuesUptoDate != null ? model.DuesUptoDate : DateTime.Now.AddDays(10);
                model.Notes = Constants.GSTNoticeInHindi;
                model.DemandNoteTypeId = Constants.InstallmentAndLeaseRentDemandNoteId;
                model.Department = model.DepartmentId == NADepartment.Institutional ? DepartmentInHindi.Institutional : (model.DepartmentId == NADepartment.Industrial ? DepartmentInHindi.Industry : (model.DepartmentId == NADepartment.Commercial ? DepartmentInHindi.Commercial : (model.DepartmentId == NADepartment.Housing ? DepartmentInHindi.Housing : (model.DepartmentId == NADepartment.Residential ? DepartmentInHindi.Residential : string.Empty))));
                //model.Department = model.DepartmentId == NADepartment.Institutional ? DepartmentInHindi.InstitutionalUnicode : (model.DepartmentId == NADepartment.Industrial ? DepartmentInHindi.IndustryUnicode : (model.DepartmentId == NADepartment.Commercial ? DepartmentInHindi.CommercialUnicode : (model.DepartmentId == NADepartment.Housing ? DepartmentInHindi.HousingUnicode : (model.DepartmentId == NADepartment.Residential ? DepartmentInHindi.ResidentialUnicode : string.Empty))));
                //demandletter = _templateParserService.GetParsedHTML(model, "DemandNoteTemplateHindi.cshtml");
                //var data = _templateParserService.CreateCustomDemandNote(model, "DemandNoteTemplateHindi.cshtml");
                demandletter = _templateParserService.GetParsedAndSavedDemandNote(model, "DemandNoteTemplateHindi.cshtml");
            }
            return Json(demandletter, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRevenueReportDetails([DataSourceRequest] DataSourceRequest request, LeaseRentViewModel model)
        {
            var data = _revenueService.GetRevenueReportDetails(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyAccountSummery([DataSourceRequest] DataSourceRequest request, PropertyViewModel model)
        {
            var data = _revenueService.GetPropertyAccountSummery(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApproverIdList([DataSourceRequest] DataSourceRequest request)
        {
            var list = _generalService.GetApproverIdList(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegistrationIdList([DataSourceRequest] DataSourceRequest request)
        {
            var list = _generalService.GetRegistrationIdListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInstallmentPaymentScheduleById([DataSourceRequest] DataSourceRequest request, PaymentScheduleModel model)
        {
            var data = _revenueService.GetInstallmentSchedulePaymentById(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateInstallmentSchedulePaymentById([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PaymentScheduleModel> paySch)
        {
            var flag = ReturnType.None;
            try
            {
                if (paySch != null)
                {
                    foreach (var edit in paySch)
                    {
                        flag = _revenueService.UpdateInstallmentSchedulePaymentById(request, edit);
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

        public JsonResult AddInstallmentPaymentBySchedule(PaymentScheduleModel model)
        {
            int flag = _revenueService.AddInstallmentPaymentBySchedule(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveTempChallanDetail(PaymentScheduleModel model)
        {
            int flag = _revenueService.SaveTempChallanDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSavedTempChallanDetail([DataSourceRequest] DataSourceRequest request, PaymentScheduleModel model)
        {
            var list = _revenueService.GetSavedTempChallanDetail(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveTempChallanDataById(PaymentScheduleModel model)
        {
            int flag = _revenueService.RemoveTempChallanDataById(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePaymentDuesById(PaymentScheduleModel model)
        {
            PaymentScheduleModel data = _revenueService.UpdatePaymentDuesById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyCostDetailAsPerScheme(PropertyViewModel model)
        {
            PropertyViewModel data = _revenueService.GetPropertyCostDetailAsPerScheme(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePropertyCostAsPerScheme(PropertyViewModel model)
        {
            int flag = _revenueService.SavePropertyCostAsPerScheme(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFloorAreaListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            var list = _generalService.GetFloorAreaListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocationTypeListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            var list = _generalService.GetLocationTypeListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegistryTypeList(string category)
        {
            var list = _generalService.GetCommonConfigDataList(category);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyCostDetailListAsDataSource([DataSourceRequest] DataSourceRequest request, PropertyViewModel model)
        {
            var list = _revenueService.GetPropertyCostDetailListAsDataSource(request,model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveExcessAreaDetail(PropertyViewModel model)
        {
            int flag = _revenueService.SaveExcessAreaDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExcessAreaDetailListAsDataSource([DataSourceRequest] DataSourceRequest request, PropertyViewModel model)
        {
            var list = _revenueService.GetExcessAreaDetailListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocationChargedPropertyListAsDataSource([DataSourceRequest] DataSourceRequest request, PropertyViewModel model)
        {
            var list = _revenueService.GetLocationChargedPropertyListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyCompensationListAsDataSource([DataSourceRequest] DataSourceRequest request, PaymentViewModel model)
        {
            var list = _revenueService.GetPropertyCompensationListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveCompensationDetail(PaymentViewModel model)
        {
            int flag = _revenueService.SaveCompensationDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllottedPropertyCostDetailById(PaymentViewModel model)
        {
            PaymentViewModel data = _revenueService.GetAllottedPropertyCostDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyLeaseRentCostDetailById(PaymentViewModel model)
        {
            PaymentViewModel data = _revenueService.GetPropertyLeaseRentCostDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyInstallmentScheduleCostDetailById(PaymentViewModel model)
        {
            PaymentViewModel data = _revenueService.GetPropertyInstallmentScheduleCostDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyExcessAreaCostDetailById(PaymentViewModel model)
        {
            PaymentViewModel data = _revenueService.GetPropertyExcessAreaCostDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyCompensationCostDetailById(PaymentViewModel model)
        {
            PaymentViewModel data = _revenueService.GetPropertyCompensationCostDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyTransferCostDetailById(PaymentViewModel model)
        {
            PaymentViewModel data = _revenueService.GetPropertyTransferCostDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //payment with multiple head
        public JsonResult SavePaidChallanDetailByRegistrationId(PaymentViewModel model)
        {
            int flag = _revenueService.SavePaidChallanDetailByRegistrationId(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveTempPaidChallanDetail(PaymentViewModel model)
        {
            int flag = _revenueService.SaveTempPaidChallanDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSavedTempPaidChallanDetail([DataSourceRequest] DataSourceRequest request, PaymentViewModel model)
        {
            var list = _revenueService.GetSavedTempPaidChallanDetail(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveTempPaidChallanDataById(PaymentViewModel model)
        {
            int flag = _revenueService.RemoveTempPaidChallanDataById(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemovePaidChallanDataById(PaymentViewModel model)
        {
            int flag = _revenueService.RemovePaidChallanDataById(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaidAmountDetailListByIdAsDataSource([DataSourceRequest] DataSourceRequest request, PaymentViewModel model)
        {
            var list = _revenueService.GetPaidAmountDetailListByIdAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReceiptHeadIdListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            var list = _generalService.GetReceiptHeadIdListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllotmentDuesCalculationDetailAsDataSource([DataSourceRequest] DataSourceRequest request, PaymentViewModel model)
        {
            var list = _revenueService.GetAllotmentDuesCalculationDetailAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExcessAreaDuesCalculationDetailAsDataSource([DataSourceRequest] DataSourceRequest request, PaymentViewModel model)
        {
            var list = _revenueService.GetExcessAreaDuesCalculationDetailAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompensationDuesCalculationDetailAsDataSource([DataSourceRequest] DataSourceRequest request, PaymentViewModel model)
        {
            var list = _revenueService.GetCompensationDuesCalculationDetailAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferDuesCalculationDetailAsDataSource([DataSourceRequest] DataSourceRequest request, PaymentViewModel model)
        {
            var list = _revenueService.GetTransferDuesCalculationDetailAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExistingPaidChallanDetail(PaymentViewModel model)
        {
            PaymentViewModel data = _revenueService.GetExistingPaidChallanDetail(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNDCGeneratedLetterListAsDataSource([DataSourceRequest] DataSourceRequest request, PaymentViewModel model)
        {
            var data = _revenueService.GetNDCGeneratedLetterListAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveGeneratedLetterByType(LetterViewModel model)
        {
            int flag = _revenueService.SaveGeneratedLetterByType(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveRegistrationIdFromPaidChallan(PaymentViewModel model)
        {
            int flag = _revenueService.RemoveRegistrationIdFromPaidChallan(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDuesUptoDateForNDC(PaymentViewModel model)
        {
            //var data = _revenueService.GetDuesAmountForDemandNoteById(model);
            var data = _revenueService.GetDuesAmountForNDCByRegistrationId(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
    
}