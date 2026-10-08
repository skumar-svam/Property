using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Model;

namespace NA.PMS.Service
{
    public interface IAllotmentEngine
    {
        // It is used for calculating the allotment money and update the same in model
        AllotmentModel CalculateAllotmentMoney(AllotmentModel allotmentModel);
        // To check whether a property us allotted or not
        bool IsPropertyAllotted(int rId);
        // To check whether Allotment is paid or not
        bool IsAllotmentMoneyPaid(int rId);
        // To check whether there is no dues pending or not
        bool IsNoDues(int rId);
        // To calculate NoDues till date
        //decimal CalculateNoDuesTillDate(int rId);
        // To calculate LeaseRent Money Annual
        decimal CalculateLeaseRentMoneyAnnual(int rId, int departmentId);
        // To calculate LeaseRent Money One Time
        decimal CalculateLeaseRentMoneyOneTime(int rId, int departmentId);
        // To Get LeaseDeed due date
        DateTime GetLeaseDeedDueDate(int rId);
        // To check that CheckList generated for Lease deed or not
        bool IsCheckListGenerated(int rId);
        // Get CheckList Due Date
        DateTime GetChecklistDueDate(int rId);
        // Get Possession Due Date
        DateTime GetPossessionDueDate(int rId, int departmentId);
         // Get One time excess charge 
        decimal CalculateOneTimeExcessCharge(decimal changedArea, int propertyId);
    }
}
