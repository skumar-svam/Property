using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Repository;

namespace NA.PMS.Service
{
    public class AllotmentEngine : IAllotmentEngine
    {
        IBusinessRuleRepository _businessRuleRepository;

        public AllotmentEngine(IBusinessRuleRepository businessRuleRepository)
        {
            //_businessRuleRepository = new BusinessRuleRepository();
            _businessRuleRepository = businessRuleRepository;
        }

        // It is used for calculating the allotment money and update the same in model
        public AllotmentModel CalculateAllotmentMoney(AllotmentModel allotmentModel)
        {
            allotmentModel = _businessRuleRepository.UpdateAllotmentModel(allotmentModel);


            if (allotmentModel != null)
            {
                allotmentModel.TotalPropertyCost = (allotmentModel.TotalArea) * allotmentModel.LandRatePerSqMet;

                allotmentModel.AllotmentMoney = ((((allotmentModel.TotalPropertyCost) * Convert.ToDecimal(allotmentModel.AllotmentMoneyPercent) / 100) - allotmentModel.EarnestMoney));
            }

            return allotmentModel;
        }

        // To check whether a property us allotted or not
        public bool IsPropertyAllotted(int rId)
        {
            bool flag = false;
            flag = _businessRuleRepository.IsPropertyAllotted(rId, Convert.ToString(AllotmentStatus.Approved));
            return flag;
        }
        // To check whether Allotment is paid or not
        public bool IsAllotmentMoneyPaid(int rId)
        {
            bool flag = false;
            var allotmentModel = _businessRuleRepository.GetPropertyAllotmentDetails(rId);
            flag = !(allotmentModel.AllotmentMoney - allotmentModel.AmountPaid > 0);
            return flag;
        }
        // To check whether there is no dues pending or not
        public bool IsNoDues(int rId)
        {
            bool flag = false;

            return flag;
        }
        // Function to calculate LeaseRent Annualy
        public decimal CalculateLeaseRentMoneyAnnual(int rId, int departmentId)
        {
            decimal leaseRentAmount = 0;
            decimal leaseRentPercent = 0;
            var allotmentModel = _businessRuleRepository.GetPropertyAllotmentDetails(rId);
            if (departmentId == Constants.DepartmentIdForHousing)
                leaseRentPercent = _businessRuleRepository.GetLeaseRentPercentForHousing(rId, departmentId);
            else
                leaseRentPercent = Convert.ToDecimal(_businessRuleRepository.GetLeaseRentPercent(rId, departmentId));

            leaseRentAmount = Convert.ToDecimal((allotmentModel.TotalPropertyCost * leaseRentPercent) / 100);

            return leaseRentAmount;
        }

        // Function to calculate LeaseRent One Time
        public decimal CalculateLeaseRentMoneyOneTime(int rId, int departmentId)
        {
            decimal oneTimeleaseRentAmount = 0;
            oneTimeleaseRentAmount = CalculateLeaseRentMoneyAnnual(rId, departmentId) * Constants.DefineYearByAuthority;


            return oneTimeleaseRentAmount;
        }
        // To check wheather CheckList has been generated or not
        public bool IsCheckListGenerated(int rId)
        {
            bool flag = _businessRuleRepository.IsCheckListGenerated(rId);
            return flag;
        }
        // Get Lease Deed Due Date
        public DateTime GetLeaseDeedDueDate(int rId)
        {
            DateTime dtLeaseDeed = _businessRuleRepository.GetLeaseDeedDueDate(rId);

            return dtLeaseDeed;
        }

        public DateTime GetChecklistDueDate(int rId)
        {
            DateTime dtCheckLst = _businessRuleRepository.GetChecklistDueDate(rId);
            return dtCheckLst;
        }

        // Get Possession Due Date
        public DateTime GetPossessionDueDate(int rId, int departmentId)
        {
            DateTime dtCheckLst = DateTime.Now.Date.AddDays(_businessRuleRepository.GetPossessionDurationInDays(rId, departmentId));
            return dtCheckLst;
        }

        // Get One time excess charge 
        public decimal CalculateOneTimeExcessCharge(decimal changedArea, int propertyId)
        {
            var excessCharge = (changedArea * (_businessRuleRepository.GetAllotmentRateByPropertyId(propertyId)));
            return excessCharge;
        }
    }
}
