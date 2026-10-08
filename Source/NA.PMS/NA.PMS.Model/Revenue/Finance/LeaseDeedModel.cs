using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class LeaseDeedModel
    {
        public Nullable<int> LeaseDeedId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<DateTime> LeaseDeedDate { get; set; }
        public Nullable<DateTime> LeaseTransferDate { get; set; }
        public Nullable<decimal> LeaseRentPerYear { get; set; }
        public Nullable<decimal> PanelInterest { get; set; }
        public Nullable<DateTime> LeaseRentPaidUpto { get; set; }
        public string LastPaidUPO { get; set; }
        public Nullable<DateTime> LeaseRentRevisedDate { get; set; }
        public Nullable<decimal> BalanceAmount { get; set; }
        public Nullable<decimal> BalanceInterest { get; set; }
        public Nullable<decimal> GstInterestPart { get; set; }
        public Nullable<decimal> TotalBalance { get; set; }
        public Nullable<DateTime> BalanceDate { get; set; }
        public Nullable<decimal> RevisedRatePerYear { get; set; }

        //Property Detail
        public string PropertyNo { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string Address { get; set; }
        public string Applicant { get; set; }
        public Nullable<DateTime> RegistryDate { get; set; }

        public string IsPremiumPaid { get; set; }
        public string IsLeaseRentPaid { get; set; }
        public Nullable<DateTime> NDCDate { get; set; }
        public Nullable<DateTime> OTRChallanDate { get; set; }

        public Nullable<int> StatusId { get; set; }
        public string strStatus { get; set; }
        public Nullable<bool> IsActive { get; set; }

        public Nullable<DateTime> Approved_Date { get; set; }
        public string AssignTo { get; set; }
        public Nullable<int> Approver { get; set; }
        public string Comment { get; set; }
        public Nullable<DateTime> CommentDate { get; set; }
        public string User { get; set; }

        public Nullable<DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UserCreatedBy { get; set; }

        public string ApproverComments { get; set; }
        public int LoginUser { get; set; }
        public Nullable<int> DepartmentId { get; set; }
    }
}
