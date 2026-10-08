using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Service
{
    public class RevenueService : IRevenueService
    {
        IRevenueRepository _revenueRepository;
        public RevenueService(IRevenueRepository revenueRepository)
        {
            _revenueRepository = revenueRepository;
        }

        public int AddPaymentDetailTemp(string rid, int? receiptId, int? headId, int? subHeadId, int? bankId, decimal? amount)
        {
            return _revenueRepository.AddPaymentDetailTemp(rid, receiptId, headId, subHeadId, bankId, amount);
        }

        public int SaveLeaseRentPaymentByRegistrationId(LeaseRentViewModel model)
        {
            return _revenueRepository.SaveLeaseRentPaymentByRegistrationId(model);
        }

        public DataSourceResult GetLeaseRentDuesDetail(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _revenueRepository.GetLeaseRentDuesDetail(request, model);
        }

        public DataSourceResult GetLeaseRentDuesRequest(DataSourceRequest request)
        {
            return _revenueRepository.GetLeaseRentDuesRequest(request);
        }

        public LeaseRentViewModel GetLeaseRentPaymentDetailById(int Id)
        {
            return _revenueRepository.GetLeaseRentPaymentDetailById(Id);
        }

        public int SaveLeaseRentApprovalStatus(LeaseRentViewModel model)
        {
            return _revenueRepository.SaveLeaseRentApprovalStatus(model);
        }

        public int LeaseDeedResendforApproval(LeaseDeedModel ObjLeaseDeedModel)
        {
            return _revenueRepository.LeaseDeedResendforApproval(ObjLeaseDeedModel); ;
        }


        public DataSourceResult GetLeaseRentDetails(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _revenueRepository.GetLeaseRentDetails(request, model);
        }


        public LeaseRentViewModel GetPaymentLeaseRentDetailsById(int id)
        {
            return _revenueRepository.GetPaymentLeaseRentDetailsById(id);
        }


        public LeaseRentViewModel GetApplicantDetailsByRegistrationId(int rid)
        {
            return _revenueRepository.GetApplicantDetailsByRegistrationId(rid);
        }

        public int SaveLeaseRentPremium(LeaseRentViewModel model)
        {
            return _revenueRepository.SaveLeaseRentPremium(model);
        }


        public DataSourceResult GetPaymentTypeList(DataSourceRequest request)
        {
            return _revenueRepository.GetPaymentTypeList(request);
        }

        public DataSourceResult GetPaymentSubTypeList(DataSourceRequest request, int? typeId)
        {
            return _revenueRepository.GetPaymentSubTypeList(request, typeId);
        }


        public DataSourceResult GetMiscellaneousPaymentListById(DataSourceRequest request, int? rid)
        {
            return _revenueRepository.GetMiscellaneousPaymentListById(request, rid);
        }


        public PaymentViewModel GetAllottedPropertyDetailsById(int rid)
        {
            return _revenueRepository.GetAllottedPropertyDetailsById(rid);
        }


        public int SaveMiscellaneousPayment(PaymentViewModel model)
        {
            return _revenueRepository.SaveMiscellaneousPayment(model);
        }


        public DataSourceResult GetLeaseRentPaymentListById(DataSourceRequest request, int? rid)
        {
            return _revenueRepository.GetLeaseRentPaymentListById(request, rid);
        }


        public DataSourceResult GetInstallmentPaymentScheduleList(DataSourceRequest request, PaymentScheduleModel model)
        {
            return _revenueRepository.GetInstallmentPaymentScheduleList(request, model);
        }


        public DataSourceResult GetInstallmentPaymentListForApproval(DataSourceRequest request, PaymentScheduleModel model)
        {
            return _revenueRepository.GetInstallmentPaymentListForApproval(request, model);
        }


        public DataSourceResult GetMiscellaneousPaymentList(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetMiscellaneousPaymentList(request, model);
        }


        public DataSourceResult GetMultiplePaymentStatusList(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _revenueRepository.GetMultiplePaymentStatusList(request, model);
        }


        public LeaseRentViewModel GetMiscellaneousPaymentCount()
        {
            return _revenueRepository.GetMiscellaneousPaymentCount();
        }


        public int UpdateLeaseRentDuesPayment(LeaseRentViewModel model)
        {
            return _revenueRepository.UpdateLeaseRentDuesPayment(model);
        }


        public int UpdateInstallmentDuesPayment(LeaseRentViewModel model)
        {
            return _revenueRepository.UpdateInstallmentDuesPayment(model);
        }


        public LeaseRentViewModel GetLeaseRentDetailsById(LeaseRentViewModel model)
        {
            return _revenueRepository.GetLeaseRentDetailsById(model);
        }


        public LeaseRentViewModel UpdateLeaseRentDuesPaymentById(LeaseRentViewModel model)
        {
            return _revenueRepository.UpdateLeaseRentDuesPaymentById(model);
        }


        public LeaseRentViewModel GetPaymentDuesByRegistrationId(LeaseRentViewModel model)
        {
            return _revenueRepository.GetPaymentDuesByRegistrationId(model);
        }


        public DataSourceResult GetPropertyAccountDetails(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetPropertyAccountDetails(request, model);
        }


        public LeaseRentViewModel UpdateInstallmentDuesPaymentById(LeaseRentViewModel model)
        {
            return _revenueRepository.UpdateInstallmentDuesPaymentById(model);
        }


        public DataSourceResult GetLeaseRentReportDetails(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _revenueRepository.GetLeaseRentReportDetails(request, model);
        }


        public DataSourceResult GetRevenueGeneratedList(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _revenueRepository.GetRevenueGeneratedList(request, model);
        }

        public LeaseRentViewModel GetRevenueWithDefaulterList(LeaseRentViewModel model)
        {
            return _revenueRepository.GetRevenueWithDefaulterList(model);
        }


        public DataSourceResult GetPropertyPremiumReportDetails(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _revenueRepository.GetPropertyPremiumReportDetails(request, model);
        }


        public DataSourceResult GetRevenueReportDetails(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _revenueRepository.GetRevenueReportDetails(request, model);
        }

        public DataSourceResult GetLeaseRentTransDetails(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _revenueRepository.GetLeaseRentTransDetails(request, model);
        }


        public DataSourceResult GetLeaseRentPaidListById(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _revenueRepository.GetLeaseRentPaidListById(request, model);
        }


        public DataSourceResult GetPropertyPremiumScheduleList(DataSourceRequest request, PaymentScheduleModel model)
        {
            return _revenueRepository.GetPropertyPremiumScheduleList(request, model);
        }


        public DataSourceResult GetPropertyPremiumSchedulePaidListById(DataSourceRequest request, PaymentScheduleModel model)
        {
            return _revenueRepository.GetPropertyPremiumSchedulePaidListById(request, model);
        }


        public int SavePropertyPremiumScheduleByRegistrationId(PaymentScheduleModel model)
        {
            return _revenueRepository.SavePropertyPremiumScheduleByRegistrationId(model);
        }


        public PaymentScheduleModel GetPropertyPremiumScheduleInformationById(PaymentScheduleModel model)
        {
            return _revenueRepository.GetPropertyPremiumScheduleInformationById(model);
        }


        public int RemovePropertyPremiumScheduleById(PaymentScheduleModel model)
        {
            return _revenueRepository.RemovePropertyPremiumScheduleById(model);
        }


        public int UpdatePropertyPremiumSchedulePaymentById(DataSourceRequest request, PaymentScheduleModel model)
        {
            return _revenueRepository.UpdatePropertyPremiumSchedulePaymentById(request, model);
        }


        public PaymentScheduleModel SavePremiumPaymentDuesByRegistrationId(PaymentScheduleModel model)
        {
            return _revenueRepository.SavePremiumPaymentDuesByRegistrationId(model);
        }


        public int SaveInstallmentDuesPaymentById(PaymentScheduleModel model)
        {
            return _revenueRepository.SaveInstallmentDuesPaymentById(model);
        }


        public DataSourceResult GetInstallmentDuesPaidListByRegistrationId(DataSourceRequest request, PaymentScheduleModel model)
        {
            return _revenueRepository.GetInstallmentDuesPaidListByRegistrationId(request, model);
        }


        public PaymentScheduleModel GetInstallmentDuesDetailByRegistrationId(PaymentScheduleModel model)
        {
            return _revenueRepository.GetInstallmentDuesDetailByRegistrationId(model);
        }


        public int GeneratePremiumScheduleByDepartment(PaymentScheduleModel model)
        {
            return _revenueRepository.GeneratePremiumScheduleByDepartment(model);
        }

        public int RemovePremiumScheduleByDepartment(PaymentScheduleModel model)
        {
            return _revenueRepository.RemovePremiumScheduleByDepartment(model);
        }

        public int GeneratePremiumScheduleByRegistrationId(PaymentScheduleModel model)
        {
            return _revenueRepository.GeneratePremiumScheduleByRegistrationId(model);
        }


        public DataSourceResult GetPropertyAccountSummery(DataSourceRequest request, PropertyViewModel model)
        {
            return _revenueRepository.GetPropertyAccountSummery(request, model);
        }


        public LeaseRentViewModel GetMiscellaneousPaymentCountByDepartment(LeaseRentViewModel model)
        {
            return _revenueRepository.GetMiscellaneousPaymentCountByDepartment(model);
        }
        public IEnumerable<LeaseRentViewModel> GetTotalCountByDepartment(LeaseRentViewModel model)
        {
            return _revenueRepository.GetTotalCountByDepartment(model);
        }


        public DataSourceResult GetDemandNoteListAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetDemandNoteListAsDataSource(request, model);
        }


        public DataSourceResult GetDemandNoteListAsDataSourceII(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetDemandNoteListAsDataSourceII(request, model);
        }


        public int UpdateInstallmentDuesPaymentII(PaymentViewModel model)
        {
            return _revenueRepository.UpdateInstallmentDuesPaymentII(model);
        }


        public DataSourceResult GetInstallmentDuesPaymentListById(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetInstallmentDuesPaymentListById(request, model);
        }

        public DataSourceResult GetLeaserentDuesPaymentListById(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetLeaserentDuesPaymentListById(request, model);
        }


        public DataSourceResult GetInstallmentPaymentScheduleListById(DataSourceRequest request, PaymentScheduleModel model)
        {
            return _revenueRepository.GetInstallmentPaymentScheduleListById(request, model);
        }


        public int UpdateInstallmentDuesPaymentByIdII(PaymentScheduleModel model)
        {
            return _revenueRepository.UpdateInstallmentDuesPaymentByIdII(model);
        }


        public int UpdateLeaseRentDuesPaymentByIdII(LeaseRentViewModel model)
        {
            return _revenueRepository.UpdateLeaseRentDuesPaymentByIdII(model);
        }


        public DataSourceResult GetLeaseRentDuesPaymentById(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _revenueRepository.GetLeaseRentDuesPaymentById(request, model);
        }


        public int UpdateDuesPaymentDetails(PaymentViewModel model)
        {
            return _revenueRepository.UpdateDuesPaymentDetails(model);
        }


        public DataSourceResult GetRegistrationIdListFromLeaseRent(DataSourceRequest request)
        {
            return _revenueRepository.GetRegistrationIdListFromLeaseRent(request);
        }


        public LeaseRentViewModel GetLeaseRentDetailsByRegistrationId(LeaseRentViewModel model)
        {
            return _revenueRepository.GetLeaseRentDetailsByRegistrationId(model);
        }

        public int UpdateLeaseRentDetails(LeaseRentViewModel model)
        {
            return _revenueRepository.UpdateLeaseRentDetails(model);
        }


        public DataSourceResult GetLeaseRentDetailsByIdAndProcedure(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _revenueRepository.GetLeaseRentDetailsByIdAndProcedure(request, model);
        }


        public LeaseRentViewModel GetInstallmentDetailsByRegistrationId(LeaseRentViewModel model)
        {
            return _revenueRepository.GetInstallmentDetailsByRegistrationId(model);
        }

        public int UpdateInstallmentRentDetails(LeaseRentViewModel model)
        {
            return _revenueRepository.UpdateInstallmentRentDetails(model);
        }


        public DataSourceResult GetRegistrationIdListFromInstallmentPayment(DataSourceRequest request)
        {
            return _revenueRepository.GetRegistrationIdListFromInstallmentPayment(request);
        }


        public PaymentViewModel GetLeaseRentDuesForDemandNote(PaymentViewModel model)
        {
            return _revenueRepository.GetLeaseRentDuesForDemandNote(model);
        }


        public LeaseRentViewModel GetDetailsByIdForLeaseRent(LeaseRentViewModel model)
        {
            return _revenueRepository.GetDetailsByIdForLeaseRent(model);
        }


        public DataSourceResult GetLeaseRentRequestList(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _revenueRepository.GetLeaseRentRequestList(request, model);
        }


        public PaymentViewModel GetInstallmentDuesForDemandNote(PaymentViewModel model)
        {
            return _revenueRepository.GetInstallmentDuesForDemandNote(model);
        }


        public PaymentViewModel GetDuesAmountForDemandNoteById(PaymentViewModel model)
        {
            return _revenueRepository.GetDuesAmountForDemandNoteById(model);
        }

        public DataSourceResult GetPropertyPaymentDetails(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetPropertyPaymentDetails(request, model);
        }

        public int UpdateRegistrationIdByPropertyNo(PaymentViewModel model)
        {
            return _revenueRepository.UpdateRegistrationIdByPropertyNo(model);
        }

        public DataSourceResult GetReceiptDetailsById(DataSourceRequest request, long? receiptId)
        {
            return _revenueRepository.GetReceiptDetailsById(request, receiptId);
        }


        public int UpdateDuesPaymentByDepartment(PaymentViewModel model)
        {
            return _revenueRepository.UpdateDuesPaymentByDepartment(model);
        }


        public PropertyViewModel GetPropertyDetailById(PropertyViewModel model)
        {
            return _revenueRepository.GetPropertyDetailById(model);
        }


        public DataSourceResult GetPaidAmountDetails(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetPaidAmountDetails(request, model);
        }

        public int UpdateRegistrationIdByAllottmentNo(PaymentViewModel model)
        {
            return _revenueRepository.UpdateRegistrationIdByAllottmentNo(model);
        }

        public PaymentViewModel InstallmentDuesPayment(int Rid)
        {
            return _revenueRepository.InstallmentDuesPayment(Rid);
        }


        public DataSourceResult GetInstallmentSchedulePaymentById(DataSourceRequest request, PaymentScheduleModel model)
        {
            return _revenueRepository.GetInstallmentSchedulePaymentById(request, model);
        }


        public int UpdateInstallmentSchedulePaymentById(DataSourceRequest request, PaymentScheduleModel model)
        {
            return _revenueRepository.UpdateInstallmentSchedulePaymentById(request, model);
        }


        public int AddInstallmentPaymentBySchedule(PaymentScheduleModel model)
        {
            return _revenueRepository.AddInstallmentPaymentBySchedule(model);
        }


        public int SaveTempChallanDetail(PaymentScheduleModel model)
        {
            return _revenueRepository.SaveTempChallanDetail(model);
        }

        public DataSourceResult GetSavedTempChallanDetail(DataSourceRequest request, PaymentScheduleModel model)
        {
            return _revenueRepository.GetSavedTempChallanDetail(request, model);
        }


        public int RemoveTempChallanDataById(PaymentScheduleModel model)
        {
            return _revenueRepository.RemoveTempChallanDataById(model);
        }


        public PaymentScheduleModel UpdatePaymentDuesById(PaymentScheduleModel model)
        {
            return _revenueRepository.UpdatePaymentDuesById(model);
        }


        public DataSourceResult GetLeaseRentPaymentTransDetailsById(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _revenueRepository.GetLeaseRentPaymentTransDetailsById(request, model);
        }

        public PaymentViewModel CalculateInstallmentDuesById(PaymentViewModel model)
        {
            return _revenueRepository.CalculateInstallmentDuesById(model);
        }


        public int SaveInstallmentScheduleTransByDate(PaymentScheduleModel model)
        {
            return _revenueRepository.SaveInstallmentScheduleTransByDate(model);
        }


        public int IsPaymentExistForRid(PaymentViewModel model)
        {
            return _revenueRepository.IsPaymentExistForRid(model);
        }


        public int IsPaymentExistForProperty(PaymentViewModel model)
        {
            return _revenueRepository.IsPaymentExistForProperty(model);
        }


        public DataSourceResult GetReScheduledInstallmentAsDataSource(DataSourceRequest request, PaymentScheduleModel model)
        {
            return _revenueRepository.GetReScheduledInstallmentAsDataSource(request, model);
        }


        public PaymentScheduleModel GetPropertyPaymentReScheduleInformationById(PaymentScheduleModel model)
        {
            return _revenueRepository.GetPropertyPaymentReScheduleInformationById(model);
        }


        public DataSourceResult GetInstallmentScheduleTypeAsDataSource(DataSourceRequest request, PaymentScheduleModel model)
        {
            return _revenueRepository.GetInstallmentScheduleTypeAsDataSource(request, model);
        }


        public PropertyViewModel GetPropertyCostDetailAsPerScheme(PropertyViewModel model)
        {
            return _revenueRepository.GetPropertyCostDetailAsPerScheme(model);
        }


        public int SavePropertyCostAsPerScheme(PropertyViewModel model)
        {
            return _revenueRepository.SavePropertyCostAsPerScheme(model);
        }

        public DataSourceResult GetPropertyCostDetailListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            return _revenueRepository.GetPropertyCostDetailListAsDataSource(request, model);
        }


        public int SaveExcessAreaDetail(PropertyViewModel model)
        {
            return _revenueRepository.SaveExcessAreaDetail(model);
        }

        public DataSourceResult GetExcessAreaDetailListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            return _revenueRepository.GetExcessAreaDetailListAsDataSource(request, model);
        }


        public PaymentViewModel GetMasterPaymentDetailByRegistrationId(int? rid)
        {
            return _revenueRepository.GetMasterPaymentDetailByRegistrationId(rid);
        }


        public DataSourceResult GetLocationChargedPropertyListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            return _revenueRepository.GetLocationChargedPropertyListAsDataSource(request, model);
        }


        public DataSourceResult GetPropertyCompensationListAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetPropertyCompensationListAsDataSource(request, model);
        }


        public int SaveCompensationDetail(PaymentViewModel model)
        {
            return _revenueRepository.SaveCompensationDetail(model);
        }


        public PaymentViewModel GetAllottedPropertyCostDetailById(PaymentViewModel model)
        {
            return _revenueRepository.GetAllottedPropertyCostDetailById(model);
        }

        public PaymentViewModel GetPropertyLeaseRentCostDetailById(PaymentViewModel model)
        {
            return _revenueRepository.GetPropertyLeaseRentCostDetailById(model);
        }

        public PaymentViewModel GetPropertyInstallmentScheduleCostDetailById(PaymentViewModel model)
        {
            return _revenueRepository.GetPropertyInstallmentScheduleCostDetailById(model);
        }


        public PaymentViewModel GetPropertyExcessAreaCostDetailById(PaymentViewModel model)
        {
            return _revenueRepository.GetPropertyExcessAreaCostDetailById(model);
        }

        public PaymentViewModel GetPropertyCompensationCostDetailById(PaymentViewModel model)
        {
            return _revenueRepository.GetPropertyCompensationCostDetailById(model);
        }

        public PaymentViewModel GetPropertyTransferCostDetailById(PaymentViewModel model)
        {
            return _revenueRepository.GetPropertyTransferCostDetailById(model);
        }


        public int SavePaidChallanDetailByRegistrationId(PaymentViewModel model)
        {
            return _revenueRepository.SavePaidChallanDetailByRegistrationId(model);
        }

        public int SaveTempPaidChallanDetail(PaymentViewModel model)
        {
            return _revenueRepository.SaveTempPaidChallanDetail(model);
        }

        public DataSourceResult GetSavedTempPaidChallanDetail(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetSavedTempPaidChallanDetail(request, model);
        }

        public int RemoveTempPaidChallanDataById(PaymentViewModel model)
        {
            return _revenueRepository.RemoveTempPaidChallanDataById(model);
        }

        public int RemovePaidChallanDataById(PaymentViewModel model)
        {
            return _revenueRepository.RemovePaidChallanDataById(model);
        }

        public DataSourceResult GetPaidAmountDetailListByIdAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetPaidAmountDetailListByIdAsDataSource(request, model);
        }


        public DataSourceResult GetAllotmentDuesCalculationDetailAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetAllotmentDuesCalculationDetailAsDataSource(request, model);
        }

        public DataSourceResult GetExcessAreaDuesCalculationDetailAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetExcessAreaDuesCalculationDetailAsDataSource(request, model);
        }

        public DataSourceResult GetCompensationDuesCalculationDetailAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetCompensationDuesCalculationDetailAsDataSource(request, model);
        }

        public DataSourceResult GetTransferDuesCalculationDetailAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetTransferDuesCalculationDetailAsDataSource(request, model);
        }


        public PaymentViewModel GetExistingPaidChallanDetail(PaymentViewModel model)
        {
            return _revenueRepository.GetExistingPaidChallanDetail(model);
        }


        public DataSourceResult GetNDCGeneratedLetterListAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            return _revenueRepository.GetNDCGeneratedLetterListAsDataSource(request, model);
        }


        public int SaveGeneratedLetterByType(LetterViewModel model)
        {
            return _revenueRepository.SaveGeneratedLetterByType(model);
        }


        public DataSourceResult GetReceiptIdListByRId(DataSourceRequest request, int? rId)
        {
            return _revenueRepository.GetReceiptIdListByRId(request,rId);
        }


        public DataSourceResult GetReceiptHeadListByReceiptId(DataSourceRequest request, long? receiptId)
        {
            return _revenueRepository.GetReceiptHeadListByReceiptId(request, receiptId);
        }


        //public int RemoveChallanDetailsById(int? Id)
        //{
        //    return _revenueRepository.RemoveChallanDetailsById(Id);
        //}


        //public int SaveTempChallanDetailsByReceiptId(PaymentViewModel model)
        //{
        //    return _revenueRepository.SaveTempChallanDetailsByReceiptId(model);
        //}


        //public DataSourceResult GetSavedTempChallanDetailByReceiptId(DataSourceRequest request, PaymentViewModel model)
        //{
        //    return _revenueRepository.GetSavedTempChallanDetailByReceiptId(request, model);
        //}


        //public int SaveChallanDetails(PaymentViewModel model)
        //{
        //    return _revenueRepository.SaveChallanDetails(model);
        //}


        public DataSourceResult GetAlloteeListByDueDate(DataSourceRequest request, LeaseRentAndInstallmentDashboard model)
        {
            return _revenueRepository.GetAlloteeListByDueDate(request, model);
        }


        public LeaseRentAndInstallmentDashboard GetTotalCountByActionType(LeaseRentAndInstallmentDashboard model)
        {
            return _revenueRepository.GetTotalCountByActionType(model);
        }


        public DataSourceResult GetListValueByTotalCount(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _revenueRepository.GetListValueByTotalCount(request, model);
        }


        public int SaveNDCLetterDetails(NDCVeiwModel model)
        {
            return _revenueRepository.SaveNDCLetterDetails(model);
        }


        public int UpdateNDCStatus(NDCVeiwModel model)
        {
            return _revenueRepository.UpdateNDCStatus(model);
        }


        public int CheckRequestIdForNDC(ServiceViewModel model)
        {
            return _revenueRepository.CheckRequestIdForNDC(model);
        }


        public int RemoveRegistrationIdFromPaidChallan(PaymentViewModel model)
        {
            return _revenueRepository.RemoveRegistrationIdFromPaidChallan(model);
        }


        public PaymentViewModel GetDuesAmountForNDCByRegistrationId(PaymentViewModel model)
        {
            return _revenueRepository.GetDuesAmountForNDCByRegistrationId(model);
        }


        public DataSourceResult GetDefaulterList(DataSourceRequest request, DefaulterViewModel model)
        {
            return _revenueRepository.GetDefaulterList(request,model);
        }
    }
}
