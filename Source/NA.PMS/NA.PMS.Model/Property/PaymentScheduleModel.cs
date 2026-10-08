using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NA.PMS.Model
{
    public class PaymentScheduleModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RefId { get; set; }
        public Nullable<int> ScheduleId { get; set; }
        public Nullable<int> PaymentId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> Frequency { get; set; }
        public Nullable<int> TotalInstallment { get; set; }
        public Nullable<int> InstallmentNo { get; set; }
        public Nullable<int> PreInstallmentNo { get; set; }
        public Nullable<int> PeriodOfInstallment { get; set; }
        public Nullable<int> PaymentModeId { get; set; }
        public Nullable<int> ReceiptId { get; set; }
        public Nullable<int> ReceiptHeadId { get; set; }
        public Nullable<int> ReceiptSubHeadId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> ReturnTypeId { get; set; }
        public Nullable<int> ApproverId { get; set; }
        public Nullable<int> BankId { get; set; }

        public Nullable<long> ChallanRefId { get; set; }
        
        public Nullable<decimal> InstallmentAmount { get; set; }
        public Nullable<decimal> InstallmentInterest { get; set; }
        public Nullable<decimal> PrincipalAmount { get; set; }
        public Nullable<decimal> PremiumAmount { get; set; }
        public Nullable<decimal> BalanceAmount { get; set; }
        public Nullable<decimal> BalanceInterest { get; set; }
        public Nullable<decimal> TotalDueAmount { get; set; }
        public Nullable<decimal> TotalBalanceAmount { get; set; }
        public Nullable<decimal> PenalInterest { get; set; }
        public Nullable<decimal> NormalInterest { get; set; }
        public Nullable<decimal> PenaltyAmount { get; set; }
        public Nullable<decimal> GST { get; set; }
        public Nullable<decimal> TotalArea { get; set; }
        public Nullable<decimal> GSTAmount { get; set; }
        public Nullable<decimal> DuesAmount { get; set; }
        public Nullable<decimal> DepositAmount { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<decimal> PreviousDues { get; set; }
        public Nullable<decimal> CurrentDues { get; set; }
        public Nullable<decimal> CurrentBalance { get; set; }
        public Nullable<decimal> TotalPropertyCost { get; set; }
        public Nullable<decimal> AllotmentMoney { get; set; }
        public Nullable<decimal> LeaseRentPerAnnum { get; set; }
        public Nullable<decimal> RentRevisedDuration { get; set; }

        public string PaymentMode { get; set; }
        public string TransactionId { get; set; }
        public string ActionType { get; set; }
        public string Applicant { get; set; }
        public string ApplicantMaster { get; set; }
        public string ApplicantType { get; set; }
        public string CorresspondAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string Department { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string PropertyType { get; set; }
        public string PropertyNo { get; set; }
        public string NIName { get; set; }
        public string PIName { get; set; }
        public string FrequencyName { get; set; }
        public string InstallmentPeriod { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string ScheduleType { get; set; }
        public string PaymentType { get; set; }
        public string PaymentStatus { get; set; }
        public string LeaseRentStatus { get; set; }
        public string OneTimeLeaseRentStatus { get; set; }
        public string OneTimeIstallmentStatus { get; set; }
        public string TotalInstallmentStatus { get; set; }
        public string TotalPremiumPaidStatus { get; set; }
        public string TotalLeaseRentStatus { get; set; }
        public string InstallmentStatus { get; set; }
        public string NDCStatus { get; set; }
        public string Comment { get; set; }
        public string ReturnType { get; set; }
        public string HtmlTemplate { get; set; }
        public string Approver { get; set; }
        public string ReceiptHead { get; set; }
        public string ReceiptSubHead { get; set; }
        public string BankName { get; set; }

        public Nullable<DateTime> InstallmentStartDate { get; set; }
        public Nullable<DateTime> InstallmentEndDate { get; set; }
        public Nullable<DateTime> InstallmentDueDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<DateTime> AllotmentDate { get; set; }
        public Nullable<DateTime> DepositDate { get; set; }
        public Nullable<DateTime> DuesUptoDate { get; set; }
        public Nullable<DateTime> NDCDate { get; set; }
        public Nullable<DateTime> BalanceUptoDate { get; set; }
        public Nullable<DateTime> PreviousDuesDate { get; set; }
        public Nullable<DateTime> CurrentDuesDate { get; set; }
        public Nullable<DateTime> LeaseDeedDate { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsInstallmentDeposited { get; set; }
        public Nullable<bool> IsAmountDeposited { get; set; }
        public Nullable<bool> IsOneTimeLeaseRentPaid { get; set; }
        public Nullable<bool> IsOneTimeInstallment { get; set; }
        public Nullable<bool> IsInstallmentPaid { get; set; }
        public Nullable<bool> IsTotalPremiumPaid { get; set; }
        public Nullable<bool> IsNDCGenerated { get; set; }
        public Nullable<bool> IsDuesUptoDate { get; set; }
               
        public AlloteeBasicInfo AlloteeBasicInfo = new AlloteeBasicInfo();       
    }


    public class LeaseRentViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RefId { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> RentId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> ApproverId { get; set; }
        public Nullable<int> LoginUserId { get; set; }
        public Nullable<int> PaymentModeId { get; set; }
        public Nullable<int> OldRId { get; set; }
        /// <summary>
        ///  for new requirement 
        /// </summary>
        public Nullable<int> TotalLeaseRentCount { get; set; }
        public Nullable<int> TotalNDCCount { get; set; }
        public Nullable<int> TotalIPCount { get; set; }
        public Nullable<int> TotalInstallmentCount { get; set; }
        public Nullable<int> TotalPropertyCount { get; set; }
        public Nullable<int> TotalFarmHousePropertyCount { get; set; }
        public Nullable<int> TotalFileNotUpdatedCount { get; set; }
        public Nullable<int> ChallanId { get; set; }
        public Nullable<int> RevisedRateAfterYear { get; set; }

        public Nullable<int> DefaulterCount { get; set; }
        public Nullable<int> ActionTypeId { get; set; }
        public Nullable<int> ReceiptId { get; set; }
        public Nullable<int> ReceiptSubHeadId { get; set; }
        public Nullable<int> ReturnTypeId { get; set; }
        public Nullable<int> NotificationId { get; set; }
        public Nullable<int> DemandNoteTypeId { get; set; }
        public Nullable<int> ReportTypeId { get; set; }  // 1 for summary 0 for details

        public Nullable<decimal> LeaseRentPremium { get; set; }
        public Nullable<decimal> LeaseRentPerAnnum { get; set; }
        public Nullable<decimal> RevisedLeaseRentPerAnnum { get; set; }
        public Nullable<decimal> LeaseRentInterest { get; set; }
        public Nullable<decimal> NormalInterest { get; set; }
        public Nullable<decimal> PanelInterest { get; set; }
        public Nullable<decimal> TotalPenalInterest { get; set; }
        public Nullable<decimal> TotalPanelInterest { get; set; }
        public Nullable<decimal> PrincipalAmount { get; set; }
        public Nullable<decimal> BalanceAmount { get; set; }
        public Nullable<decimal> BalanceInterest { get; set; }
        public Nullable<decimal> GST { get; set; }
        public Nullable<decimal> GstAmount { get; set; }
        public Nullable<decimal> TotalBalance { get; set; }        
        public Nullable<decimal> RevisedPremium { get; set; }
        public Nullable<decimal> RevisedRate { get; set; }
        public Nullable<decimal> PremiumInterest { get; set; }
        public Nullable<decimal> TotalLeaseRent { get; set; }
        public Nullable<decimal> TotalPremium { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> LeaseRentDues { get; set; }
        public Nullable<decimal> DuesAmount { get; set; }
        public Nullable<decimal> TotalDuesAmount { get; set; }
        public Nullable<decimal> DefaulterAmount { get; set; }
        public Nullable<decimal> TotalArear { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<decimal> DepositAmount { get; set; }
        public Nullable<decimal> PreviousDues { get; set; }
        public Nullable<decimal> CurrentDues { get; set; }
        public Nullable<decimal> DuesInterest { get; set; }
        public Nullable<decimal> OneTimeLeaseRentAmount { get; set; }
        public Nullable<decimal> InstallmentAmount { get; set; }
        public Nullable<decimal> InstallmentInterest { get; set; }
        public Nullable<decimal> OtherCharges { get; set; }
        public Nullable<decimal> PreviousBalance { get; set; }

        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo { get; set; }
        public string Department { get; set; }  
        public string Address { get; set; }
        public string Applicant { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string NDCStatus { get; set; }
        public string PaymentMode { get; set; }
        public string TransactionId { get; set; }
        public string Approver { get; set; }
        public string LastPaidUPO { get; set; }
        public string PremiumPaidDuration { get; set; }
        public string LeaseRentDuration { get; set; }
        public string FinancialYear { get; set; }
        public string Comment { get; set; }
        public string User { get; set; }
        public string CreatedBy { get; set; }
        public string UserCreatedBy { get; set; }
        public string ApproverComments { get; set; }
        public string OneTimePaidStatus { get; set; }
        public string OneTimeLeasePaidStatus { get; set; }
        public string PremiumPaidStatus { get; set; }
        public string LeaseRentStatus { get; set; }
        public string TotalPremiumPaidStatus { get; set; }
        public string OneTimePremiumPaidStatus { get; set; }
        public string InstallmentPaidStatus { get; set; }
        public string OneTimeInstallmentPaidStatus { get; set; }
        public string ActionType { get; set; }
        public string FilterType { get; set; }
        public string HtmlDuesReport { get; set; }
        public string ReceiptSubHead { get; set; }
        public string AmountInWords { get; set; }
        public string Notes { get; set; }
        public string PremiumBalance { get; set; }
        public string SchemeName { get; set; }

        public Nullable<DateTime> RegistryDate { get; set; }
        public Nullable<DateTime> ApprovalDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> PaidUptoDate { get; set; }
        public Nullable<DateTime> DuesUptoDate { get; set; }
        public Nullable<DateTime> CommentDate { get; set; }
        public Nullable<DateTime> RevisedDate { get; set; }
        public Nullable<DateTime> LeaseDeedDate { get; set; }
        public Nullable<DateTime> TransferLeaseDate { get; set; }
        public Nullable<DateTime> BalanceUptoDate { get; set; }
        public Nullable<DateTime> NDCDate { get; set; }
        public Nullable<DateTime> ChallanDate { get; set; }
        public Nullable<DateTime> DepositDate { get; set; }
        public Nullable<DateTime> DepositDueDate { get; set; }
        public Nullable<DateTime> InstallmentStartDate { get; set; }
        public Nullable<DateTime> InstallmentEndDate { get; set; }
        public Nullable<DateTime> PreviousDuesDate { get; set; }
        public Nullable<DateTime> CurrentDuesDate { get; set; }
        public Nullable<DateTime> ActionDate { get; set; }
     
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDuesUptoDate { get; set; }
        public Nullable<bool> IsLeaseRentPaid { get; set; }
        public Nullable<bool> IsTotalLeaseRentPaid { get; set; }
        public Nullable<bool> IsOneTimeLeaseRentPaid { get; set; } 
        public Nullable<bool> IsTotalInstallmentPaid { get; set; }
        public Nullable<bool> IsOneTimeInstallmentPaid { get; set; }
        public Nullable<bool> IsTotalPremiumPaid { get; set; }
        public Nullable<bool> IsNDCGenerated { get; set; }

        public string ApplicantName { get; set; }
        public string departmentName { get; set; }
        public string sDuesUptoDate { get; set; }
        

    }


    public class PayScheduleGrid
    {
        public int Id { get; set; }
        public Nullable<int> ScheduleId { get; set; }
        public Nullable<int> Rid { get; set; }
        public Nullable<int> InstallmentNumber { get; set; }
        public Nullable<decimal> PrincipalInstallmentAmount { get; set; }
        public Nullable<decimal> InterestInstallmentAmount { get; set; }
        public Nullable<System.DateTime> InstallmentDueDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> DepositeFlag { get; set; }
        public Nullable<decimal> PrincipalBalAmount { get; set; }
        public Nullable<System.Decimal> TotalDueAmount { get; set; }
        public Nullable<decimal> TotalBalAmount { get; set; }
        public List<decimal?> lstTotalDueAmount { get; set; }
        public Nullable<System.DateTime> InstalmentStartDate { get; set; }
    }

    public class DefaulterViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> InstallmentNumber { get; set; }
        public Nullable<int> CreatedBy { get; set; }

        public Nullable<decimal> InstallmentAmount { get; set; }
        public Nullable<decimal> LeaserentDuesAmount { get; set; }
        public Nullable<decimal> PenalInstallmentInterestAmount { get; set; }
        public Nullable<decimal> LeaserentInterestAmount { get; set; }
        public Nullable<decimal> InstallmentDuesAmount { get; set; }
        public Nullable<decimal> TotalLeaserentDues { get; set; }
        public Nullable<decimal> TotalInstallmentDues { get; set; }
        public Nullable<decimal> TotalDues { get; set; }
        public Nullable<decimal> AreaRange { get; set; }

        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> CalculationDate { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<DateTime> AllotmentDate { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public string LeaserentPaidUpto { get; set; }
        public string InstallmentPaidUpto { get; set; }
        public string Department { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo
        {
            get
            {
                return Sector + "/" + Block + "-" + PlotNo;
            }
        }
    }

    public class LeaseRentAndInstallmentDashboard
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> TotalLeaseRentCount { get; set; }
        public Nullable<int> TotalInstallmentCount { get; set; }
        public Nullable<int> StatusId { get; set; }

        public Nullable<DateTime> LeaseDeedDate { get; set; }
        public Nullable<DateTime> leaserentduedate { get; set; } //currentdate
        public Nullable<DateTime> RevisedPremiumDate { get; set; }
        public Nullable<DateTime> CurrentDuesDate { get; set; } // leaserentdue
        public Nullable<DateTime> PremiumPaidUptoDate { get; set; } // leaserent upto
        public Nullable<DateTime> CurrentDate { get; set; } // leaserent upto

        public Nullable<decimal> PremiumLeaseRent { get; set; }
        public Nullable<decimal> RevisedPremium { get; set; } // 
        public Nullable<decimal> CurrentDues { get; set; } // leaserent dues

        //public string PropertyNo { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string ActionType { get; set; }
        public string Department { get; set; }
        public string KYAStatus { get; set; }
        public string Month { get; set; }
        public string PropertyNo
        {
            get
            {
                return Sector + "/" + Block + "-" + PlotNo;
            }
        }

        public Nullable<bool> IsKYAVerified { get; set; }

        public IEnumerable<SelectListItem> Months
        {
            get
            {
                return DateTimeFormatInfo
                       .InvariantInfo
                       .MonthNames
                       .Select((monthName, index) => new SelectListItem
                       {
                           Value = (index + 1).ToString(),
                           Text = monthName
                       });
            }
        }
    }

}
