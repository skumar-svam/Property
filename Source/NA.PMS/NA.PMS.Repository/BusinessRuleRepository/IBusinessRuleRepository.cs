using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Model;
using Kendo.Mvc.UI;

namespace NA.PMS.Repository
{
    public interface IBusinessRuleRepository
    {
        // It is used for calculating the allotment money and update the same in model
        AllotmentModel UpdateAllotmentModel(AllotmentModel allotmentModel);
        // To check whether a property us allotted or not
        bool IsPropertyAllotted(int rId, string status);
        // To check whether Allotment is paid or not
        bool IsAllotmentMoneyPaid(int rId);
       
        // To calculate Stamp Duty Amount and Duplicate Stamp value
        LeaseDeedProperty GetLeaseDeedDetails(int rid, DateTime leaseDeedDate);

        // To get the details of the property that will be used in calculations
        AllotmentModel GetPropertyAllotmentDetails(int rid);

        // Get Balance Due Till Date
        decimal GetBalanceDueTillDate(int rid, int departmentId);

        // Get Penel Interest
        decimal GetPenalInterest(int deptId);

        // Function to Get LeaseRent Percent
        double GetLeaseRentPercent(int rId, int departmentId);
         // Function to Get LeaseRent Percent
        decimal GetLeaseRentPercentForHousing(int rId, int departmentId);

        // Get Lease Rent Dues Till date
        decimal GetLeaseRentDuesTillDate(int rid, int departmentId);

        // To Get LeaseDeed due date
        DateTime GetLeaseDeedDueDate(int rId);
        // To check that CheckList generated for Lease deed or not
        bool IsCheckListGenerated(int rId);
        DateTime GetChecklistDueDate(int rId);
        // Get Possession Due Date
        int GetPossessionDurationInDays(int rId, int departmentId);
        // Get LandRatePerSQMT from DB
        decimal GetAllotmentRateByPropertyId(int propertyId);

        ChallanModel GetLeaseRentValue(int rId, int deptId, int type);
        int CalculatePreviousDues(int rId);
        bool IsRentingChargePaid(int rid);

        DataSourceResult GetPropertyPaymentDetails_Read(DataSourceRequest request);
    }
}
