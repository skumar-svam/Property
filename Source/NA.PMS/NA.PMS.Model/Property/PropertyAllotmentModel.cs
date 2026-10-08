using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class AllottedPropertyViewModel
    {
        public PropertyViewModel Property { get; set; }
        public AllotmentViewModel Allotment { get; set; }
        public ApplicantViewModel Applicant { get; set; }
        public TransferViewModel Transfer { get; set; }
        public DocumentViewModel Document { get; set; }
        public PaymentViewModel Payment { get; set; }
    }

    public class AllotmentViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> RequestId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> ApproverId { get; set; }
        public Nullable<int> RequestorId { get; set; }
        public Nullable<int> Year { get; set; }
        
        public string SchemeName { get; set; }
        public string Department { get; set; }
        public string FormNo { get; set; }
        public string MobileNo { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo { get; set; }
        public string Applicant { get; set; }
        public string ApplicantType { get; set; }
        public string ExpansionType { get; set; }
        public string ApplicantMaster { get; set; }
        public string FatherOrHusbandName { get; set; }       
        public string CorresspondAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string RegisteredAddress { get; set; }
        public string Status { get; set; }
        public string PAN { get; set; }
        public string FormType { get; set; }
        public string ApplicantStatus { get; set; }
        public string AllotmentStatus { get; set; }
        public string AllotmentType { get; set; }
        public string ActionType { get; set; }  
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public string Gender { get; set; }
        public string Comment { get; set; }
        public string Message { get; set; }
        public string AreaRange { get; set; }
        public string Approver { get; set; }
        public string Requestor { get; set; }
        public string EncryptedSchemeId { get { return CommonHelper.Encode(SchemeId.ToString()); } }
        public string EncryptedRId { get { return RegistrationId != null ? CommonHelper.Encode(RegistrationId.ToString()) : null; } }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsAllotted { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<bool> IsDocumentAvailable { get; set; }

        public Nullable<DateTime> AllotmentDate { get; set; }
        public Nullable<DateTime> SubmissionDate { get; set; }
        public Nullable<DateTime> ApprovalDate { get; set; }
        public Nullable<DateTime> InstalmentStartDate { get; set; }       
        public Nullable<DateTime> AllotmentMoneyDueDate { get; set; }
        public Nullable<DateTime> DepositDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        
        public Nullable<decimal> TotalArea { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> AllotmentMoney { get; set; }
        public Nullable<decimal> LandRate { get; set; }
        public Nullable<decimal> AllotmentMoneyPercent { get; set; }
        public Nullable<decimal> EarnestMoney { get; set; }
        public Nullable<decimal> TotalPropertyCost { get; set; }
        public Nullable<decimal> AmountPaid { get; set; }

        //public PropertyAllotmentModel PropertyAllotment { get; set; }
        //public ApplicationFormModel ApplicationForm { get; set; }
    }

    public class FunctionalViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RequestNo { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> PropertyTypeId { get; set; }
        public Nullable<int> ApproverId { get; set; }

        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo { get; set; }
        public string FloorArea { get; set; }
        public string Status { get; set; }
        public string SchemeName { get; set; }
        public string Department { get; set; }
        public string Applicant { get; set; }
        public string ApplicantType { get; set; }
        public string PropertyType { get; set; }
        public string Approver { get; set; }
        public string Comment { get; set; }
        public string MeterSeallingFile { get; set; }
        public string AffidavitFile { get; set; }
        public string RegisteredCertificateFile { get; set; }
        public string NOCAccountFile { get; set; }
        public string FunctionalStatus { get; set; }

        public Nullable<DateTime> FunctionalDueDate { get; set; }
        public Nullable<DateTime> FunctionalDate { get; set; }
        public Nullable<DateTime> CompletionDate { get; set; }
        public Nullable<DateTime> AffidavitDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ApprovalDate { get; set; }

        public Nullable<decimal> FunctionalCharge { get; set; }
        public Nullable<decimal> TotalArea { get; set; }
        
        public Nullable<bool> IsFunctional { get; set; }
        public Nullable<bool> IsMeterSealed { get; set; }
        public Nullable<bool> IsAffidavit { get; set; }
        public Nullable<bool> IsRegistered { get; set; }
        public Nullable<bool> IsNOCAccount { get; set; }
        public Nullable<bool> IsSelected { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }


    public class RefundModel
    {
        //public int Id { get; set; }
        public int SchemeId { get; set; }
        public string SchemeName { get; set; }
        public int RefundStatusId { get; set; }
        public string RefundStatusText { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public DateTime? RefundInitiationDate { get; set; }
        public string DepttName { get; set; }
        public int DepttId { get; set; }
    }

    public class UnsuccessfulApplicant
    {
        //public int Id { get; set; }
        public int ApplicationID { get; set; }
        public string DepttName { get; set; }
        public int DepttId { get; set; }
        public string ApplicantName { get; set; }
        public string MobileNo { get; set; }
        public decimal AmountDeposited { get; set; }
        public string FormNo { get; set; }
        public int SchemeId { get; set; }
        public string Status { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public bool IsSubmitted { get; set; }
        public string CurrentStatus { get; set; }
        public string From { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? SubmittedDate { get; set; }
        public string Comment { get; set; }
        [DisplayName("Assign To*")]
        public string User { get; set; }
    }

}
