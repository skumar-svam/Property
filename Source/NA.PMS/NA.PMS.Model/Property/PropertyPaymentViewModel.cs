using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Model
{
    public class PaymentViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RefId { get; set; }
        public Nullable<int> LeaseRentId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> PropertyTypeId { get; set; }  
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> AccountHeadId { get; set; }
        public Nullable<int> AccountSubHeadId { get; set; }
        public Nullable<int> PaymentTypeId { get; set; }
        public Nullable<int> PaymentSubTypeId { get; set; }
        public Nullable<int> PaymentModeId { get; set; }  
        public Nullable<int> ReceiptCode { get; set; }
        public Nullable<int> ReceiptHeadId { get; set; }
        public Nullable<int> ReceiptSubHeadId { get; set; }
        public Nullable<int> ActionTypeId { get; set; }
        public Nullable<int> FilterTypeId { get; set; }
        public Nullable<int> ReturnTypeId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> BankId { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> SubHeadCode { get; set; }
        public Nullable<int> HeadCode { get; set; }
        public Nullable<int> InstallmentNo { get; set; }
        public Nullable<int> ApproverId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> TotalLeaseRentCount { get; set; }
        public Nullable<int> TotalIPCount { get; set; }
        public Nullable<int> TotalNDCCount { get; set; }
        public Nullable<int> NoOfInstallment { get; set; }
        public Nullable<int> FrequencyId { get; set; }
        public Nullable<int> NotificationId { get; set; }
        public Nullable<int> DemandNoteTypeId { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<int> KYAStatusId { get; set; }
        public Nullable<int> ScheduleId { get; set; }
        public Nullable<int> ServieRequestId { get; set; }

        public Nullable<long> SerialNo { get; set; }
        public Nullable<long> SchemeId { get; set; }
        public Nullable<long> LocationId { get; set; }

        public string RegistrationNo { get; set; }
        public string DepartmentName { get; set; }
        public string Department { get; set; }
        public string ReceiptHeadName { get; set; }      
        public string ReceiptSubHeadName { get; set; }       
        public string PropertyRegistryId { get; set; }        
        public string PropertyId { get; set; }      
        public string SectorName { get; set; }    
        public string BlockName { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNumber { get; set; }
        public string SchemeName { get; set; }
        public string PropertyType { get; set; }
        public string Location { get; set; }
        public string ReceiptHead { get; set; }
        public string ReceiptSubHead { get; set; }

        public string PropertyNo { get { return SectorName + "/" + (BlockName==null ? string.Empty : BlockName + "-") + PlotNo; } }
        public string Applicant { get; set; }
        public string ApplicantII { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string HeadName { get; set; }
        public string SubHeadName { get; set; }
        public string UserId { get; set; }
        public string CONS_NO { get; set; }
        public string FLAG_EDIT { get; set; }
        public string LeaseRentUpto { get; set; }        
        public string PaymentStatus { get; set; }
        public string CreatedBy { get; set; }    
        public string AllotteeName { get { return FirstName + " " + (MiddleName == null ? string.Empty : MiddleName + " ") + LastName;} }
        public string DepositorName { get; set; }
        public string CorresspondentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string ApplicantAddress { get; set; }
        public string ApplicantAddressII { get; set; }
        public string DepositorAddress { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string ChallanId { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMode { get; set; }
        public string AccountHead { get; set; }
        public string AccountSubHead { get; set; }
        public string PaymentType { get; set; }
        public string PaymentSubType { get; set; }
        public string Comment { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string ActionType { get; set; }
        public string FilterType { get; set; }
        public string ReturnType { get; set; }
        public string LeaseRentDuration { get; set; }
        public string InstallmentPeriod { get; set; }
        public string InstallmentDuration { get; set; }
        public string PremiumPaidDuration { get; set; }
        public string OneTimePaidStatus { get; set; }
        public string PremiumPaidStatus { get; set; }
        public string TotalPremiumPaidStatus { get; set; }
        public string TotalInstallmentStatus { get; set; }
        public string OneTimeLeaseRentStatus { get; set; }
        public string LeaseRentStatus { get; set; }
        public string NDCStatus { get; set; }
        public string HeadStatus { get; set; }
        public string SubHeadStatus { get; set; }
        public string HtmlTemplate { get; set; }
        public string Frequency { get; set; }
        public string ScheduleType { get; set; }
        public string ToalIstallmentPaidStatus { get; set; }
        public string OneTimeLeasePaidStatus { get; set; }
        public string DemandNoteContent { get; set; }
        public string AmountInWords { get; set; }
        public string Notes { get; set; }
        public string DemandNotes { get; set; }
        public string DemandNoteType { get; set; }
        public string DemandNoteStatus { get; set; }
        //public string IsTotalPremiumPaid { get; set; }
        public string IsOneTimeLeasePaid { get; set; }
        public string IsOneTimeLease { get; set; }
        public string VoucherNo { get; set; }
        public string AllottmentNo { get; set; }
        public string StartYearInWord { get; set; }
        public string EndYearInWord { get; set; }
        public string AtiriktPratikar { get; set; }
        public string Remarks { get; set; }

        public string ExtraTaxNotes { get; set; }

        public HtmlString HtmlDemandNoteContent { get { return string.IsNullOrEmpty(DemandNoteContent) ? null : new HtmlString(DemandNoteContent); } }

        public Nullable<long> ReceiptId { get; set; }
        public Nullable<double> DuePrincipalAmount { get; set; }
        public Nullable<double> DueInterestAmount { get; set; }
        public Nullable<double> LeaseRentAmount { get; set; }

        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<decimal> BudgetAmount { get; set; }
        public Nullable<decimal> DebitAmount { get; set; }
        public Nullable<decimal> CreditAmount { get; set; }
        public Nullable<decimal> BalanceAmount { get; set; }
        public Nullable<decimal> PrincipalAmount { get; set; }
        public Nullable<decimal> InstallmentAmount { get; set; }
        public Nullable<decimal> InstallmentInterest { get; set; }
        public Nullable<decimal> RescheduledInstallment { get; set; }
        public Nullable<decimal> TotalInstallment { get; set; }
        public Nullable<decimal> DuesAmount { get; set; }
        public Nullable<decimal> InterestAmount { get; set; }
        public Nullable<decimal> TotalDuesAmount { get; set; }
        public Nullable<decimal> GSTAmount { get; set; }
        public Nullable<decimal> LeaseRentDues { get; set; }
        public Nullable<decimal> LeaseRentPremium { get; set; }
        public Nullable<decimal> LeaseRentPerAnnum { get; set; }
        public Nullable<decimal> LeaseRentInterest { get; set; }
        public Nullable<decimal> TotalLeaseRent { get; set; }
        public Nullable<decimal> PenalInterest { get; set; }
        public Nullable<decimal> PenalInterestAmount { get; set; }
        public Nullable<decimal> NormalInterest { get; set; }
        public Nullable<decimal> GST { get; set; }
        public Nullable<decimal> RevisedLeaseRent { get; set; }
        public Nullable<decimal> RevisedInstallment { get; set; }
        public Nullable<decimal> TotalBalance { get; set; }
        public Nullable<decimal> BalanceInterest { get; set; }
        public Nullable<decimal> LeaseRentBalance { get; set; }
        public Nullable<decimal> InstallmentBalance { get; set; }
        public Nullable<decimal> RevisedRate { get; set; }
        public Nullable<decimal> PreviousDues { get; set; }
        public Nullable<decimal> CurrentDues { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> OneTimeLeaseRentAmount { get; set; }
        public Nullable<decimal> OtherCharges { get; set; }
        public Nullable<decimal> AllottedArea { get; set; }
        public Nullable<decimal> TotalAllottedArea { get; set; }
        public Nullable<decimal> TotalArea { get; set; }
        public Nullable<decimal> TotalPropertyCost { get; set; }
        public Nullable<decimal> AllotmentMoneyInPercent { get; set; }
        public Nullable<decimal> AllotmentMoney { get; set; }
        public Nullable<decimal> LocationCharge { get; set; }
        public Nullable<decimal> LocationChargeRate { get; set; }
        public Nullable<decimal> AllotmentRate { get; set; }
        public Nullable<decimal> TotalAllotmentRate { get; set; }
        public Nullable<decimal> TotalPremiumAmount { get; set; }
        public Nullable<decimal> AdvanceLeaseRentAmount { get; set; }
        public Nullable<decimal> RegistrationAmount { get; set; }
        public Nullable<decimal> DepositAmount { get; set; }
        public Nullable<decimal> EarnestMoney { get; set; }
        public Nullable<decimal> BalanceAllotmentAmount { get; set; }
        public Nullable<decimal> PayableAllotmentAmount { get; set; }
        public Nullable<decimal> TransferChargeRate { get; set; }
        public Nullable<decimal> TotalExcessArea { get; set; }
        public Nullable<decimal> ExcessAreaRate { get; set; }
        public Nullable<decimal> ExcessAreaCost { get; set; }
        public Nullable<decimal> TotalExcessAreaCost { get; set; }
        public Nullable<decimal> TotalPayableAmount { get; set; }
        public Nullable<decimal> CompensationRate { get; set; }
        public Nullable<decimal> CompensationInPercent { get; set; }
        public Nullable<decimal> AreaForCompensation { get; set; }
        public Nullable<decimal> CompensationAmount { get; set; }
        public Nullable<decimal> TotalCompensationAmount { get; set; }
        public Nullable<decimal> TransferAmount { get; set; }
        public Nullable<decimal> ActualArea { get; set; }
        public Nullable<decimal> CoveredArea { get; set; }
        public Nullable<decimal> PropertyCost { get; set; }
        public Nullable<decimal> CivilCost { get; set; }
        public Nullable<decimal> LeaseRent { get; set; }
        public Nullable<decimal> AdvanceLeaseRent { get; set; }
        public Nullable<decimal> TotalAllotmentAmount { get; set; }
        public Nullable<decimal> EffectedArea { get; set; }
        public Nullable<decimal> ExtensionCharge { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsPaymentActive { get; set; }
        public Nullable<bool> IsLeaseRentPaid { get; set; }
        public Nullable<bool> IsOneTimeLeaseRentPaid { get; set; }
        public Nullable<bool> IsInstallmentPaid { get; set; }
        public Nullable<bool> IsTotalInstallmentPaid { get; set; }
        public Nullable<bool> IsNDCGenerated { get; set; }
        public Nullable<bool> IsDuesUptoDate { get; set; }
        public Nullable<bool> IsTotalPremiumPaid { get; set; }

        public Nullable<DateTime> RegistryDate { get; set; }
        public Nullable<DateTime> LeaseDeedDate { get; set; }
        public Nullable<DateTime> PossessionDate { get; set; }
        public Nullable<DateTime> FunctionalDate { get; set; }
        public Nullable<DateTime> DepositDate { get; set; }
        public Nullable<DateTime> DepositDueDate { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public Nullable<DateTime> ModifyDate { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> DateUptoPremium { get; set; }
        public Nullable<DateTime> NDCDate { get; set; }
        public Nullable<DateTime> InstallmentDueDate { get; set; }
        public Nullable<DateTime> ApprovalDate { get; set; }
        public Nullable<DateTime> BalanceUptoDate { get; set; }
        public Nullable<DateTime> DuesUptoDate { get; set; }
        public Nullable<DateTime> InstallmentDuesUptoDate { get; set; }
        public Nullable<DateTime> LeaseRentDuesUptoDate { get; set; }
        public Nullable<DateTime> PaidUptoDate { get; set; }
        public Nullable<DateTime> RevisedDate { get; set; }
        public Nullable<DateTime> ChallanDate { get; set; }
        public Nullable<DateTime> PreviousDuesDate { get; set; }
        public Nullable<DateTime> CurrentDuesDate { get; set; }
        public Nullable<DateTime> InstallmentStartDate { get; set; }
        public Nullable<DateTime> InstallmentEndDate { get; set; }
        public Nullable<DateTime> ActionDate { get; set; }
        public Nullable<DateTime> VoucherDate { get; set; }
        public Nullable<DateTime> AllotmentDate { get; set; }
        public Nullable<DateTime> TransferRequestDate { get; set; }
        public Nullable<DateTime> TransferDeedDate { get; set; }
        public Nullable<DateTime> ValidUptoDate { get; set; }
        public Nullable<DateTime> ExcessAreaDate { get; set; }
        public Nullable<DateTime> CompensationRequestDate { get; set; }

        public string EncodedRid { get { return RegistrationNo != null ? CommonHelper.Encode(RegistrationNo) : null; } } 
    }
}
