using Kendo.Mvc.UI;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Repository
{
    public interface IRevenueRepository
    {
        int AddPaymentDetailTemp(string rid, int? receiptId, int? headId, int? subHeadId, int? bankId, decimal? amount);

        int SaveLeaseRentPaymentByRegistrationId(LeaseRentViewModel model);

        DataSourceResult GetLeaseRentDuesDetail(DataSourceRequest request, LeaseRentViewModel model);

        DataSourceResult GetLeaseRentDuesRequest(DataSourceRequest request);

        LeaseRentViewModel GetLeaseRentPaymentDetailById(int Id);

        int SaveLeaseRentApprovalStatus(LeaseRentViewModel model);

        int LeaseDeedResendforApproval(LeaseDeedModel ObjLeaseDeedModel);

        DataSourceResult GetLeaseRentDetails(DataSourceRequest request, LeaseRentViewModel model);

        LeaseRentViewModel GetPaymentLeaseRentDetailsById(int id);

        LeaseRentViewModel GetApplicantDetailsByRegistrationId(int rid);

        int SaveLeaseRentPremium(LeaseRentViewModel model);

        DataSourceResult GetPaymentTypeList(DataSourceRequest request);

        DataSourceResult GetPaymentSubTypeList(DataSourceRequest request, int? typeId);

        DataSourceResult GetMiscellaneousPaymentListById(DataSourceRequest request, int? rid);

        PaymentViewModel GetAllottedPropertyDetailsById(int rid);

        int SaveMiscellaneousPayment(PaymentViewModel model);

        DataSourceResult GetLeaseRentPaymentListById(DataSourceRequest request, int? rid);

        DataSourceResult GetInstallmentPaymentScheduleList(DataSourceRequest request, PaymentScheduleModel model);

        DataSourceResult GetInstallmentPaymentListForApproval(DataSourceRequest request, PaymentScheduleModel model);

        DataSourceResult GetMiscellaneousPaymentList(DataSourceRequest request, PaymentViewModel model);

        DataSourceResult GetMultiplePaymentStatusList(DataSourceRequest request, LeaseRentViewModel model);

        LeaseRentViewModel GetMiscellaneousPaymentCount();

        int UpdateLeaseRentDuesPayment(LeaseRentViewModel model);

        int UpdateInstallmentDuesPayment(LeaseRentViewModel model);

        LeaseRentViewModel GetLeaseRentDetailsById(LeaseRentViewModel model);

        LeaseRentViewModel UpdateLeaseRentDuesPaymentById(LeaseRentViewModel model);

        LeaseRentViewModel GetPaymentDuesByRegistrationId(LeaseRentViewModel model);

        DataSourceResult GetPropertyAccountDetails(DataSourceRequest request, PaymentViewModel model);

        LeaseRentViewModel UpdateInstallmentDuesPaymentById(LeaseRentViewModel model);

        DataSourceResult GetLeaseRentReportDetails(DataSourceRequest request, LeaseRentViewModel model);

        DataSourceResult GetRevenueGeneratedList(DataSourceRequest request, LeaseRentViewModel model);

        LeaseRentViewModel GetRevenueWithDefaulterList(LeaseRentViewModel model);

        DataSourceResult GetPropertyPremiumReportDetails(DataSourceRequest request, LeaseRentViewModel model);

        DataSourceResult GetRevenueReportDetails(DataSourceRequest request, LeaseRentViewModel model);

        DataSourceResult GetLeaseRentTransDetails(DataSourceRequest request, LeaseRentViewModel model);

        DataSourceResult GetLeaseRentPaidListById(DataSourceRequest request, LeaseRentViewModel model);

        DataSourceResult GetPropertyPremiumScheduleList(DataSourceRequest request, PaymentScheduleModel model);

        DataSourceResult GetPropertyPremiumSchedulePaidListById(DataSourceRequest request, PaymentScheduleModel model);

        int SavePropertyPremiumScheduleByRegistrationId(PaymentScheduleModel model);

        PaymentScheduleModel GetPropertyPremiumScheduleInformationById(PaymentScheduleModel model);

        int RemovePropertyPremiumScheduleById(PaymentScheduleModel model);

        int UpdatePropertyPremiumSchedulePaymentById(DataSourceRequest request, PaymentScheduleModel model);

        PaymentScheduleModel SavePremiumPaymentDuesByRegistrationId(PaymentScheduleModel model);

        int SaveInstallmentDuesPaymentById(PaymentScheduleModel model);

        DataSourceResult GetInstallmentDuesPaidListByRegistrationId(DataSourceRequest request, PaymentScheduleModel model);

        PaymentScheduleModel GetInstallmentDuesDetailByRegistrationId(PaymentScheduleModel model);

        int GeneratePremiumScheduleByDepartment(PaymentScheduleModel model);

        int RemovePremiumScheduleByDepartment(PaymentScheduleModel model);

        int GeneratePremiumScheduleByRegistrationId(PaymentScheduleModel model);

        DataSourceResult GetPropertyAccountSummery(DataSourceRequest request, PropertyViewModel model);

        LeaseRentViewModel GetMiscellaneousPaymentCountByDepartment(LeaseRentViewModel model);
        IEnumerable<LeaseRentViewModel> GetTotalCountByDepartment(LeaseRentViewModel model);

        DataSourceResult GetDemandNoteListAsDataSource(DataSourceRequest request, PaymentViewModel model);

        DataSourceResult GetDemandNoteListAsDataSourceII(DataSourceRequest request, PaymentViewModel model);

        int UpdateInstallmentDuesPaymentII(PaymentViewModel model);

        DataSourceResult GetInstallmentDuesPaymentListById(DataSourceRequest request, PaymentViewModel model);

        DataSourceResult GetLeaserentDuesPaymentListById(DataSourceRequest request, PaymentViewModel model);

        DataSourceResult GetInstallmentPaymentScheduleListById(DataSourceRequest request, PaymentScheduleModel model);

        int UpdateInstallmentDuesPaymentByIdII(PaymentScheduleModel model);

        int UpdateLeaseRentDuesPaymentByIdII(LeaseRentViewModel model);

        DataSourceResult GetLeaseRentDuesPaymentById(DataSourceRequest request, LeaseRentViewModel model);

        int UpdateDuesPaymentDetails(PaymentViewModel model);

        DataSourceResult GetLeaseRentDetailsByIdAndProcedure(DataSourceRequest request, LeaseRentViewModel model);

        DataSourceResult GetRegistrationIdListFromLeaseRent(DataSourceRequest request);

        LeaseRentViewModel GetLeaseRentDetailsByRegistrationId(LeaseRentViewModel model);

        int UpdateLeaseRentDetails(LeaseRentViewModel model);

        LeaseRentViewModel GetInstallmentDetailsByRegistrationId(LeaseRentViewModel model);

        int UpdateInstallmentRentDetails(LeaseRentViewModel model);

        DataSourceResult GetRegistrationIdListFromInstallmentPayment(DataSourceRequest request);

        PaymentViewModel GetLeaseRentDuesForDemandNote(PaymentViewModel model);

        LeaseRentViewModel GetDetailsByIdForLeaseRent(LeaseRentViewModel model);

        DataSourceResult GetLeaseRentRequestList(DataSourceRequest request, LeaseRentViewModel model);

        PaymentViewModel GetInstallmentDuesForDemandNote(PaymentViewModel model);

        PaymentViewModel GetDuesAmountForDemandNoteById(PaymentViewModel model);

        DataSourceResult GetPropertyPaymentDetails(DataSourceRequest request, PaymentViewModel model);

        int UpdateRegistrationIdByPropertyNo(PaymentViewModel model);

        DataSourceResult GetReceiptDetailsById(DataSourceRequest request, long? receiptId);

        int UpdateDuesPaymentByDepartment(PaymentViewModel model);

        PropertyViewModel GetPropertyDetailById(PropertyViewModel model);

        DataSourceResult GetPaidAmountDetails(DataSourceRequest request, PaymentViewModel model);

        int UpdateRegistrationIdByAllottmentNo(PaymentViewModel model);

        PaymentViewModel InstallmentDuesPayment(int Rid);

        DataSourceResult GetInstallmentSchedulePaymentById(DataSourceRequest request, PaymentScheduleModel model);

        int UpdateInstallmentSchedulePaymentById(DataSourceRequest request, PaymentScheduleModel model);

        int AddInstallmentPaymentBySchedule(PaymentScheduleModel model);

        int SaveTempChallanDetail(PaymentScheduleModel model);

        DataSourceResult GetSavedTempChallanDetail(DataSourceRequest request, PaymentScheduleModel model);

        int RemoveTempChallanDataById(PaymentScheduleModel model);

        PaymentScheduleModel UpdatePaymentDuesById(PaymentScheduleModel model);

        DataSourceResult GetLeaseRentPaymentTransDetailsById(DataSourceRequest request, LeaseRentViewModel model);

        PaymentViewModel CalculateInstallmentDuesById(PaymentViewModel model);

        int SaveInstallmentScheduleTransByDate(PaymentScheduleModel model);

        int IsPaymentExistForRid(PaymentViewModel model);

        int IsPaymentExistForProperty(PaymentViewModel model);

        DataSourceResult GetReScheduledInstallmentAsDataSource(DataSourceRequest request, PaymentScheduleModel model);

        PaymentScheduleModel GetPropertyPaymentReScheduleInformationById(PaymentScheduleModel model);

        DataSourceResult GetInstallmentScheduleTypeAsDataSource(DataSourceRequest request, PaymentScheduleModel model);

        PropertyViewModel GetPropertyCostDetailAsPerScheme(PropertyViewModel model);

        int SavePropertyCostAsPerScheme(PropertyViewModel model);

        DataSourceResult GetPropertyCostDetailListAsDataSource(DataSourceRequest request, PropertyViewModel model);

        int SaveExcessAreaDetail(PropertyViewModel model);

        DataSourceResult GetExcessAreaDetailListAsDataSource(DataSourceRequest request, PropertyViewModel model);

        PaymentViewModel GetMasterPaymentDetailByRegistrationId(int? rid);

        DataSourceResult GetLocationChargedPropertyListAsDataSource(DataSourceRequest request, PropertyViewModel model);

        DataSourceResult GetPropertyCompensationListAsDataSource(DataSourceRequest request, PaymentViewModel model);

        int SaveCompensationDetail(PaymentViewModel model);

        PaymentViewModel GetAllottedPropertyCostDetailById(PaymentViewModel model);

        PaymentViewModel GetPropertyLeaseRentCostDetailById(PaymentViewModel model);

        PaymentViewModel GetPropertyInstallmentScheduleCostDetailById(PaymentViewModel model);

        PaymentViewModel GetPropertyExcessAreaCostDetailById(PaymentViewModel model);

        PaymentViewModel GetPropertyCompensationCostDetailById(PaymentViewModel model);

        PaymentViewModel GetPropertyTransferCostDetailById(PaymentViewModel model);

        int SavePaidChallanDetailByRegistrationId(PaymentViewModel model);

        int SaveTempPaidChallanDetail(PaymentViewModel model);

        DataSourceResult GetSavedTempPaidChallanDetail(DataSourceRequest request, PaymentViewModel model);

        int RemoveTempPaidChallanDataById(PaymentViewModel model);

        int RemovePaidChallanDataById(PaymentViewModel model);

        DataSourceResult GetPaidAmountDetailListByIdAsDataSource(DataSourceRequest request, PaymentViewModel model);

        DataSourceResult GetAllotmentDuesCalculationDetailAsDataSource(DataSourceRequest request, PaymentViewModel model);

        DataSourceResult GetExcessAreaDuesCalculationDetailAsDataSource(DataSourceRequest request, PaymentViewModel model);

        DataSourceResult GetCompensationDuesCalculationDetailAsDataSource(DataSourceRequest request, PaymentViewModel model);

        DataSourceResult GetTransferDuesCalculationDetailAsDataSource(DataSourceRequest request, PaymentViewModel model);

        PaymentViewModel GetExistingPaidChallanDetail(PaymentViewModel model);

        DataSourceResult GetNDCGeneratedLetterListAsDataSource(DataSourceRequest request, PaymentViewModel model);

        int SaveGeneratedLetterByType(LetterViewModel model);

        DataSourceResult GetReceiptIdListByRId(DataSourceRequest request, int? rId);

        DataSourceResult GetReceiptHeadListByReceiptId(DataSourceRequest request, long? receiptId);

        //int RemoveChallanDetailsById(int? Id);

        //int SaveTempChallanDetailsByReceiptId(PaymentViewModel model);

        //DataSourceResult GetSavedTempChallanDetailByReceiptId(DataSourceRequest request, PaymentViewModel model);

        //int SaveChallanDetails(PaymentViewModel model);

        DataSourceResult GetAlloteeListByDueDate(DataSourceRequest request, LeaseRentAndInstallmentDashboard model);

        LeaseRentAndInstallmentDashboard GetTotalCountByActionType(LeaseRentAndInstallmentDashboard model);

        DataSourceResult GetListValueByTotalCount(DataSourceRequest request, LeaseRentViewModel model);

        int SaveNDCLetterDetails(NDCVeiwModel model);

        int UpdateNDCStatus(NDCVeiwModel model);

        int CheckRequestIdForNDC(ServiceViewModel model);

        int RemoveRegistrationIdFromPaidChallan(PaymentViewModel model);

        PaymentViewModel GetDuesAmountForNDCByRegistrationId(PaymentViewModel model);

        DataSourceResult GetDefaulterList(DataSourceRequest request, DefaulterViewModel model);
    }
}
