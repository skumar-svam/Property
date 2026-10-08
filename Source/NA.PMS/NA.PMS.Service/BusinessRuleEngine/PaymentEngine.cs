using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Model;
using NA.PMS.Repository;
using Kendo.Mvc.UI;

namespace NA.PMS.Service.BusinessRuleEngine
{
    public class PaymentEngine : IPaymentEngine
    {
        IBusinessRuleRepository _businessRuleRepository;
        IPaymentEngineRepository _paymentRepository;
        public PaymentEngine(IBusinessRuleRepository businessRuleRepository, IPaymentEngineRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
            _businessRuleRepository = businessRuleRepository;
        }

        // To calculate NoDues till date
        public int CalculatePreviousDues(int rId)
        {
            //decimal previousDues = 0;//For the case when no Details exist for this rId
            //var allotmentModel = _businessRuleRepository.GetPropertyAllotmentDetails(rId);
            //if (allotmentModel != null)
            //{
            //    var allotmentMoneyPaid = allotmentModel.AllotmentMoney - allotmentModel.AmountPaid;
            //    var defaultedDays = (DateTime.Now.Date.AddDays(10) - Convert.ToDateTime(allotmentModel.AllotmentMoneyDueDate).Date.AddDays(30));
            //    var penalInterest = _businessRuleRepository.GetPenalInterest(Convert.ToInt32(allotmentModel.DepartmentId));

            //    previousDues = (((((Convert.ToDecimal(allotmentMoneyPaid) * penalInterest) / 100) / 365) * defaultedDays.Days) + Convert.ToDecimal(allotmentMoneyPaid));
            //}
            var previousDues = _businessRuleRepository.CalculatePreviousDues(rId);
            return previousDues;
        }

        // To calculate stampduty amount
        public LeaseDeedProperty CalculateStampDutyAmount(int rId, DateTime leaseDeesDate)
        {
            var leaseDeedProperty = new LeaseDeedProperty();
            leaseDeedProperty = _businessRuleRepository.GetLeaseDeedDetails(rId, leaseDeesDate);
            var allotmentModel = _businessRuleRepository.GetPropertyAllotmentDetails(rId);
            if (leaseDeedProperty != null && allotmentModel != null)
            {
                leaseDeedProperty.StampDutyAmount = ((leaseDeedProperty.StampDutyPercent * allotmentModel.TotalPropertyCost) / 100);
            }
            return leaseDeedProperty;
        }

        // Get Balance Due Till Date
        public decimal GetBalanceDueTillDate(int rid, int departmentId)
        {
            decimal noDuesAmount = 0;
            noDuesAmount = _businessRuleRepository.GetBalanceDueTillDate(rid, departmentId);
            return noDuesAmount;
        }

        // Get Lease Rent Dues Till Date
        public decimal GetLeaseRentDuesTillDate(int rid, int departmentId)
        {
            decimal noDuesAmount = 0;
            noDuesAmount = _businessRuleRepository.GetBalanceDueTillDate(rid, departmentId);
            return noDuesAmount;
        }

        public ChallanModel GetLeaseRentValue(int rId, int deptId, int type)
        {
            return _businessRuleRepository.GetLeaseRentValue(rId, deptId, type);
        }

        public bool IsRentingChargePaid(int rid)
        {
            return _businessRuleRepository.IsRentingChargePaid(rid);
        }

        public DataSourceResult GetPropertyPaymentDetails_Read(DataSourceRequest request)
        {
            return _paymentRepository.GetPropertyPaymentDetails_Read(request);
        }

        public PaymentViewModel SavePaymentByRegistrationId(PaymentViewModel model)
        {
            return _paymentRepository.SavePaymentByRegistrationId(model);
        }

        public List<DynamicDataModel> GetPropertyPaymentType()
        {
            return _paymentRepository.GetPropertyPaymentType();
        }

        public List<DynamicDataModel> GetPropertySubPaymentType(int receiptId)
        {
            return _paymentRepository.GetPropertySubPaymentType(receiptId);
        }

        public PaymentViewModel SavePropertyPayment(PaymentViewModel model)
        {
            return _paymentRepository.SavePropertyPayment(model);
        }

        public DataSourceResult GetPaymentSchedule(DataSourceRequest req, int rid)
        {
            return _paymentRepository.GetPaymentSchedule(req, rid);
        }

        public PaymentScheduleModel GetPayScheduleInfo(int? Rid)
        {
            return _paymentRepository.GetPayScheduleInfo(Rid);
        }

        public bool SavePaymentSchedule(PaymentScheduleModel objPaymentScheduleModel)
        {
            return _paymentRepository.SavePaymentSchedule(objPaymentScheduleModel);
        }

        public bool UpdatePaymentSchedule(PaymentScheduleModel objPayScheduleGrid)
        {
            return _paymentRepository.UpdatePaymentSchedule(objPayScheduleGrid);
        }

        public bool RemovePayScheduleInfo(int Rid, int ScheduleId)
        {
            return _paymentRepository.RemovePayScheduleInfo(Rid, ScheduleId);
        }


        public DataSourceResult GetPartialOrFullPaymentDetails(DataSourceRequest request, string departmentId, string isPremiumPaid, string isLeaseRentPaid)
        {
            return _paymentRepository.GetPartialOrFullPaymentDetails(request, departmentId, isPremiumPaid, isLeaseRentPaid);
        }


        public DataSourceResult GetRegistrationIdListForPayment(DataSourceRequest request)
        {
            return _paymentRepository.GetRegistrationIdListForPayment(request);
        }

        public List<DropdownViewModel> GetPaymentModeList()
        {
            return _paymentRepository.GetPaymentModeList();
        }


        public PaymentScheduleModel SaveInstallmentPaymentSchedule(PaymentScheduleModel model)
        {
            return _paymentRepository.SaveInstallmentPaymentSchedule(model);
        }


        public DataSourceResult GetInstallmentPaymentListByRegistrationId(DataSourceRequest request, int rid)
        {
            return _paymentRepository.GetInstallmentPaymentListByRegistrationId(request, rid);
        }


        public List<DropdownViewModel> GetPaymentFrequencyList()
        {
            return _paymentRepository.GetPaymentFrequencyList();
        }


        public DataSourceResult GetAccountHeadList(DataSourceRequest request)
        {
            return _paymentRepository.GetAccountHeadList(request);
        }

        public DataSourceResult GetAccountSubHeadList(DataSourceRequest request, int? id)
        {
            return _paymentRepository.GetAccountSubHeadList(request,id);
        }

        public DataSourceResult GetPaymentTypeList(DataSourceRequest request)
        {
            return _paymentRepository.GetPaymentTypeList(request);
        }

        public DataSourceResult GetPaymentSubTypeList(DataSourceRequest request, int? id)
        {
            return _paymentRepository.GetPaymentSubTypeList(request, id);
        }


        public DataSourceResult GetRegistrationIdListFromPaymentSchedule(DataSourceRequest request)
        {
            return _paymentRepository.GetRegistrationIdListFromPaymentSchedule(request);
        }


        public LeaseRentViewModel CalculateLeaseRentPremium(LeaseRentViewModel model)
        {
            return _paymentRepository.CalculateLeaseRentPremium(model);
        }


        public LeaseRentViewModel CalculateInstallmentDuesPremium(LeaseRentViewModel model)
        {
            return _paymentRepository.CalculateInstallmentDuesPremium(model);
        }
    }
}
