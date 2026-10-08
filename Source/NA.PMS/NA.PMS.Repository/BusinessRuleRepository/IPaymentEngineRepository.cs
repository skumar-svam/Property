using Kendo.Mvc.UI;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Repository
{
    public interface IPaymentEngineRepository
    {
        DataSourceResult GetPropertyPaymentDetails_Read(DataSourceRequest request);

        PaymentViewModel SavePaymentByRegistrationId(PaymentViewModel model);

        List<DynamicDataModel> GetPropertyPaymentType();

        List<DynamicDataModel> GetPropertySubPaymentType(int receiptId);

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
