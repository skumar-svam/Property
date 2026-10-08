using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Model;
using Kendo.Mvc.UI;

namespace NA.PMS.Service.BusinessRuleEngine
{
    public interface IPaymentEngine
    {
        // To calculate stampduty amount
        LeaseDeedProperty CalculateStampDutyAmount(int rId, DateTime leaseDeesDate);
        // To calculate NoDues till date
        int CalculatePreviousDues(int rId);

        // Get Balance Due Till Date
        decimal GetBalanceDueTillDate(int rid, int departmentId);

        // Get Lease Rent Dues Till Date
        decimal GetLeaseRentDuesTillDate(int rid, int departmentId);
        ChallanModel GetLeaseRentValue(int rId, int deptId, int type);

        // Total Premium Balance Amount = Total Cost of property – (allotment money + earnest money)
        // Principal Amount= Total Premium Balance Amount/No. of Installments
        // First Installment Interest Calculation = [Total Premium Balance Amount * (Rate of Interest/Frequency)]/100
        // Premium Balance Amount = Premium Balance Amount - First Installment Principal Amount
        // Second Installment Interest Calculation = [Balance Premium Amount *(Rate of interest/Frequency)]/100
        // Premium Balance Amount = Premium Balance Amount - Second Installment Principal Amount
        // Nth Installment Interest Calculation = [Balance Premium Amount *(Rate of Interest/Frequency)]/100
        // Premium Balance Amount = Premium Balance Amount - nth Installment Principal Amount

        // Rate of Interest, r = (N.I. / (12/f))
        // F=frequency;=1(yearly),f=2(halfyearly),f=4(quarterly),f=12(monthly),  N.I. = Normal Interest, n = Number of installment

        // Factor, F = [POW ((1+r/100), n) * r/100] / [POW ((1+r/100), n)-1]

        // Installment Amount = Balance Amount * Factor  

        // Penal Interest = P*(1+r/100) ^ n  ==> Where, P = Principal, r = rate of interest, n = frequency


        bool IsRentingChargePaid(int rid);

        DataSourceResult GetPropertyPaymentDetails_Read(DataSourceRequest request);

        PaymentViewModel SavePaymentByRegistrationId(PaymentViewModel model);

        List<DynamicDataModel> GetPropertySubPaymentType(int receiptId);

        List<DynamicDataModel> GetPropertyPaymentType();

        PaymentViewModel SavePropertyPayment(PaymentViewModel model);
        DataSourceResult GetPaymentSchedule(DataSourceRequest req, int rid);
        PaymentScheduleModel GetPayScheduleInfo(int? Rid);
        bool SavePaymentSchedule(PaymentScheduleModel objPaymentScheduleModel);
        bool UpdatePaymentSchedule(PaymentScheduleModel objPayScheduleGrid);
        bool RemovePayScheduleInfo(int Rid, int ScheduleId);
        DataSourceResult GetPartialOrFullPaymentDetails(DataSourceRequest request, string departmentId, string isPremiumPaid, string isLeaseRentPaid);

        DataSourceResult GetRegistrationIdListForPayment(DataSourceRequest request);

        List<DropdownViewModel> GetPaymentModeList();

        PaymentScheduleModel SaveInstallmentPaymentSchedule(PaymentScheduleModel model);

        DataSourceResult GetInstallmentPaymentListByRegistrationId(DataSourceRequest request, int rid);

        List<DropdownViewModel> GetPaymentFrequencyList();

        DataSourceResult GetAccountHeadList(DataSourceRequest request);

        DataSourceResult GetAccountSubHeadList(DataSourceRequest request, int? id);

        DataSourceResult GetPaymentTypeList(DataSourceRequest request);

        DataSourceResult GetPaymentSubTypeList(DataSourceRequest request, int? id);

        DataSourceResult GetRegistrationIdListFromPaymentSchedule(DataSourceRequest request);

        LeaseRentViewModel CalculateLeaseRentPremium(LeaseRentViewModel model);

        LeaseRentViewModel CalculateInstallmentDuesPremium(LeaseRentViewModel model);
    }
}

