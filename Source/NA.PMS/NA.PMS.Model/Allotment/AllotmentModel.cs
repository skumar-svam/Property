using NA.PMS.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NA.PMS.Model
{
    public class PropertyApplicationForm
    {
        public int ApplicationId { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string FormNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string MarritalStatus { get; set; }
        public string FatherHusbandName { get; set; }
        public string MotherName { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string SigningAuthority { get; set; }
        public string RegisteredOffice { get; set; }
        public string CorrespondanceAdd { get; set; }
        public string PermanentAdd { get; set; }
        public string MobileNumberP1 { get; set; }
        public string MobileNumberP2 { get; set; }
        public string PhoneNumberP1 { get; set; }
        public string PhoneNumberP2 { get; set; }
        public string FaxNumberP1 { get; set; }
        public string FaxNumberP2 { get; set; }
        public string Email { get; set; }
        public Nullable<int> OccupationId { get; set; }
        public string OccupationName { get; set; }
        public Nullable<int> QuotaId { get; set; }
        public string QuotaName { get; set; }
        public Nullable<int> ReligionId { get; set; }
        public string ReligionName { get; set; }
        public string PanNumber { get; set; }
        public Nullable<decimal> AnnualIncome { get; set; }
        public string RegistrationId { get; set; }
        public string IsAllotted { get; set; }

        public string PaymentMode { get; set; }
        public Nullable<int> BankId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<decimal> AmountDeposited { get; set; }
        public string DemandDraftNo { get; set; }
        public string DemandDraftIssueBank { get; set; }
        public Nullable<System.DateTime> DemandDraftIssueDate { get; set; }
        public string Utn { get; set; }
        public Nullable<int> ChallanNo { get; set; }
        public Nullable<System.DateTime> ChallanIssueDate { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

    }

    #region Manage Application From
    public class ApplicationFormModel
    {
        public int ApplicationId { get; set; }

        [Required(ErrorMessage = "Scheme Name is required")]
        public int SchemeId { get; set; }
        public string SchemeName { get; set; }

        [Required(ErrorMessage = "Department Name is required")]
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        [Required(ErrorMessage = "Form No is required")]
        [RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "Form No must be alphanumeric.")]
        public string FormNo { get; set; }

        public int GenderId { get; set; }
        [Required(ErrorMessage = "Gender Name is required")]
        public string GenderName { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression(@"^[a-zA-Z/\& ]*$", ErrorMessage = "First Name must be alphabets only (space,/,& ) allowed.")]
        public string FirstName { get; set; }

        [RegularExpression(@"^[a-zA-Z/\& ]*$", ErrorMessage = "Middle Name must be alphabets only (space,/,& ) allowed.")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        //[RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Field must be alphabets only (space allowed).")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email.")]
        //[RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@(([a-zA-Z\-]+\.)+)([a-zA-Z]{2,4})$", ErrorMessage = "Please enter a valid e-mail address")]
        public string Email { get; set; }

        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Father's Name must be alphabets only (space allowed).")]
        [Required(ErrorMessage = "Father's Name is required")]
        public string FatherName { get; set; }

        //[Required(ErrorMessage = "Mother's Name is required")]
        //[RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Mother's Name must be alphabets only (space allowed).")]
        public string MotherName { get; set; }

        [Required(ErrorMessage = "Date Of Birth is required")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DOB { get; set; }

        public int Age { get; set; }

        public int MaritialStatusId { get; set; }

        [Required(ErrorMessage = "Marital Status is required")]
        public string MaritialStatusName { get; set; }

        [Required(ErrorMessage = "Religion is required")]
        public int? ReligionId { get; set; }
        public string ReligionName { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Occupation is required")]
        public int? OccupationId { get; set; }
        public string OccupationName { get; set; }

        [Required(ErrorMessage = "Corresponding Address is required")]
        public string CorrespondingAddress { get; set; }

        [Required(ErrorMessage = "Permanent Address is required")]
        public string PermanentAddress { get; set; }

        public string RegisteredOffice { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Authorized Signatory is required")]
        public string SigningAuthority { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "CNC must be numeric.")]
        //[Required(ErrorMessage = "CNC is required")]
        public string MobileNumberP1 { get; set; }

        //[Required(ErrorMessage = "Mobile Number is required")]
        //[Range(1000000000, 999999999999, ErrorMessage = "Mobile Number should be of 12 digits.")]
        //[RegularExpression(@"^[0-9]*$", ErrorMessage = "Mobile Number must be numeric.")]
        public string MobileNumber { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "STD must be numeric.")]
        public string PhoneNumberP1 { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Phone Number must be numeric.")]
        //[StringLength(12, MinimumLength = 8, ErrorMessage = "Phone Number should be in between 8 and 12 digits.")]
        [MaxLength(12, ErrorMessage = "Phone Number should be in between 8 and 12 digits.")]
        [MinLength(8, ErrorMessage = "Phone Number should be in between 8 and 12 digits.")]
        public string PhoneNumber { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "STD Number must be numeric.")]
        public string FaxNumberP1 { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Fax Number must be numeric.")]
        [MaxLength(12, ErrorMessage = "Fax Number should be in between 8 and 12 digits.")]
        [MinLength(8, ErrorMessage = "Fax Number should be in between 8 and 12 digits.")]
        // [StringLength(12, MinimumLength = 8, ErrorMessage = "Fax Number should be in between 8 and 12 digits.")]
        public string FaxNumber { get; set; }

        //[Required(ErrorMessage = "Pan Number is required")]
        //[RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "Pan Number must be alphanumeric.")]
        public string PanNumber { get; set; }

        //[Required(ErrorMessage = "Annual Income is required")]
        //[Range(50000, 999999999999999, ErrorMessage = "Annual Income must be decimal(15,2) and greater than 50000")]
        public Decimal? AnnualIncome { get; set; }

        public string PaymentType { get; set; }

        [Required(ErrorMessage = "Bank is required")]
        public int? BankId { get; set; }
        public string BankName { get; set; }

        //[Required(ErrorMessage = "Bank is required")]
        //public int? BankIdRTGS { get; set; }
        //public string BankNameRTGS { get; set; }

        [Required(ErrorMessage = "Branch is required")]
        public int? BranchId { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessage = "DD Number is required")]
        [RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "Demand Draft Number must be alphanumeric.")]
        public string DemandDraftNumber { get; set; }

        [Required(ErrorMessage = "UTR is required")]
        [RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "UTR Number must be alphanumeric.")]
        public string UTRNumber { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [Required(ErrorMessage = "Issue Date is required")]
        public DateTime? IssueDate { get; set; }

        //[DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        //[Required(ErrorMessage = "Issue Date is required")]
        //public DateTime? IssueDateRTGS { get; set; }

        public string IssueBank { get; set; }

        //[Required(ErrorMessage = "Amount is required")]
        [Range(1.0, 999999999999, ErrorMessage = "Amount Deposited must be decimal(15,2) and greater than 0")]
        public Decimal? Amount { get; set; }

        //[Required(ErrorMessage = "Amount is required")]
        //[Range(1.0, 999999999999, ErrorMessage = "Amount Deposited must be decimal(15,2) and greater than 0")]
        //public Decimal? AmountRTGS { get; set; }

        public DateTime? SchemeStartDate { get; set; }
        public DateTime? SchemeEndDate { get; set; }
        public DateTime? CurrentDate { get; set; }

        public PropertyAllotmentModel PropertyAllotmentModel { get; set; }

        public string IsAllotted { get; set; }

        [Required(ErrorMessage = "RID is required")]
        public string RID { get; set; }

        public Nullable<decimal> earnestMoney { get; set; }
        public Boolean RecordExistsIn { get; set; }
        public string Status { get; set; }

        [Required(ErrorMessage = "Amount Deposited is required")]
        public Decimal? AmountDepositedId { get; set; }

        //[Required(ErrorMessage = "Amount Deposited is required")]
        //public Decimal? AmountDepositedIdRTGS { get; set; }
        public int PropID { get; set; }

        public string ApplicantName
        {
            get { return FirstName + " " + MiddleName + " " + LastName; }
        }

        public int? SchemeStatus { get; set; }
        public string EncryptedAppId { get { return CommonHelper.Encode(ApplicationId.ToString()); } }
        private string encodedParam;
        public string EncodedParameter
        {
            get { return encodedParam = CommonHelper.Encode(SchemeId.ToString()); }
            set { encodedParam = CommonHelper.Encode(SchemeId.ToString()); }
        }

        public DateTime? IsAllotmentCompletedForDay { get; set; }

        public Nullable<int> OldRegistrationId { get; set; }
    }

    public class UploadDetails
    {
        public string filePhoto { get; set; }
        public string fileSign { get; set; }
        public string fileDoc { get; set; }
        public string IDLst { get; set; }
        public int ApplicationID { get; set; }
        public List<int?> Checklist { get; set; }
    }

    public class FormViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> FormId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> ExRegistrationId { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> GenderId { get; set; }
        public Nullable<int> Age { get; set; }
        public Nullable<int> MaritialStatusId { get; set; }
        public Nullable<int> ReligionId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<int> OccupationId { get; set; }
        public Nullable<int> BankId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> RefundBankId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> ApproverId { get; set; }

        public string SchemeName { get; set; }
        public string Department { get; set; }
        public string Application { get; set; }
        public string FormType { get; set; }
        public string ExpansionType { get; set; }
        public string FormNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string Applicant { get; set; }
        public string FirstApplicant { get; set; }
        public string ApplicantMaster { get; set; }
        public string ApplicantType { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string Religion { get; set; }
        public string Category { get; set; }
        public string Occupation { get; set; }
        public string FirstApplicantAdd { get; set; }
        public string CorrespondAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string RegisteredOffice { get; set; }
        public string SigningAuthority { get; set; }
        public string PaymentType { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string DDNo { get; set; }
        public string UTR { get; set; }
        public string PAN { get; set; }
        public string RefundAccountNo { get; set; }
        public string RefundInfaverOf { get; set; }
        public string AccountNo { get; set; }
        public string AreaRange { get; set; }
        public string Comment { get; set; }
        public string IssueBank { get; set; }
        public string AllotmentStatus { get; set; }
        public string FormStatus { get; set; }
        public string Status { get; set; }
        public string Approver { get; set; }
        public string ActionType { get; set; }

        public Nullable<decimal> Area { get; set; }
        public Nullable<decimal> AnnualIncome { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> EarnestMoney { get; set; }
        public Nullable<decimal> DepositedAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> ProcessingCharge { get; set; }
        public Nullable<decimal> ApplicationFee { get; set; }

        public Nullable<DateTime> DateOfBirth { get; set; }
        public Nullable<DateTime> IssuingDate { get; set; }
        public Nullable<DateTime> AllotmentDate { get; set; }
        public Nullable<DateTime> InstallmentStartDate { get; set; }
        public Nullable<DateTime> ApplicationDate { get; set; }
        public Nullable<DateTime> DepositDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<DateTime> SubmissionDate { get; set; }

        public Nullable<bool> IsRecordExist { get; set; }
        public Nullable<bool> IsFormAllotted { get; set; }
        public Nullable<bool> IsActive { get; set; }

        public string EncryptedApplicationId { get { return ApplicationId == null ? null : CommonHelper.Encode(ApplicationId.ToString()); } }
        public string EncryptedFormId { get { return FormId == null ? null : CommonHelper.Encode(ApplicationId.ToString()); } }
    }

    public class OnlineApplicationFormModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        //public string ApplicationName { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public string SchemeName { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string Department { get; set; }
        public string FormNo { get; set; }

        //for individual/company name
        public string Applicant { get; set; }
        //father/husband/signing authority name
        public string ApplicantMaster { get; set; }
        //for male/female/company type
        public string ApplicantType { get; set; }

        public int GenderId { get; set; }

        public string GenderName { get; set; }

        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "First name must be alphabets.")]
        public string FirstName { get; set; }

        [RegularExpression(@"^[a-zA-Z. ]*$", ErrorMessage = "Middle name must be alphabets.")]
        public string MiddleName { get; set; }

        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "Last name must be alphabets.")]
        public string LastName { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@(([a-zA-Z\-]+\.)+)([a-zA-Z]{2,4})$", ErrorMessage = "Please enter a valid e-mail address")]
        public string Email { get; set; }

        [RegularExpression(@"^[a-zA-Z. ]*$", ErrorMessage = "Father name must be alphabets.")]
        public string FatherName { get; set; }

        [RegularExpression(@"^[a-zA-Z. ]*$", ErrorMessage = "Mother name must be alphabets.")]
        public string MotherName { get; set; }


        public DateTime? DOB { get; set; }

        public int Age { get; set; }

        public int MaritialStatusId { get; set; }


        public string MaritialStatusName { get; set; }


        public int? ReligionId { get; set; }
        public string ReligionName { get; set; }


        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }

        public int? CompanySelType { get; set; }

        public int? OccupationId { get; set; }
        public string OccupationName { get; set; }


        public string CorrespondingAddress { get; set; }


        public string PermanentAddress { get; set; }

        public string RegisteredOffice { get; set; }


        public string SigningAuthority { get; set; }


        public string MobileNumberP1 { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Mobile Number must be numeric.")]
        public string MobileNumber { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Phone Number must be numeric.")]
        public string PhoneNumberP1 { get; set; }

        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Phone Number must be numeric.")]
        public string PhoneNumber { get; set; }


        public string FaxNumberP1 { get; set; }
        public string FaxNumber { get; set; }

        [Required(ErrorMessage = "Pan Number is required")]
        [RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "Pan Number must be alphanumeric.")]
        public string PanNumber { get; set; }

        public Decimal? AnnualIncome { get; set; }
        public string PaymentType { get; set; }
        public int? BankId { get; set; }
        public string BankName { get; set; }
        public int? BranchId { get; set; }
        public string BranchName { get; set; }
        public string DemandDraftNumber { get; set; }
        public string UTRNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public string IssueBank { get; set; }
        public Decimal Amount { get; set; }
        public DateTime? SchemeStartDate { get; set; }
        public DateTime? SchemeEndDate { get; set; }
        public DateTime? CurrentDate { get; set; }
        public PropertyAllotmentModel PropertyAllotmentModel { get; set; }
        public string IsAllotted { get; set; }
        public string RID { get; set; }
        public Nullable<decimal> earnestMoney { get; set; }
        public Boolean RecordExistsIn { get; set; }
        public string Status { get; set; }
        public Decimal? AmountDepositedId { get; set; }
        public int PropID { get; set; }
        //{ return FirstName + " " + MiddleName + " " + LastName; }
        public string ApplicantName { get; set; }
        public int? SchemeStatus { get; set; }
        //public string EncryptedAppId { get { return CommonHelper.Encode(ApplicationId.ToString()); } }
        public DateTime? IsAllotmentCompletedForDay { get; set; }
        public Decimal? ApplicationFee { get; set; }
        public bool? IsCompany { get; set; }
        public string CompanyName { get; set; }
        public bool flag { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Decimal? ProcessingFee { get; set; }

        public int? RefundBankId { get; set; }


        public string RefundInfaverof { get; set; }
        [Range(1000000000, 9999999999999999, ErrorMessage = "Account Number must be between 11 To 16 digits.")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Account Number must be numeric.")]
        public string RefundaccountNo { get; set; }
        public string Area { get; set; }
        public string AreaRange { get; set; }
        public UploadDetails FileDetails { get; set; }

        public string Comment { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string EncryptedApplicationId { get { return CommonHelper.Encode(ApplicationId.ToString()); } }
        public DateTime? AmountPaidDate { get; set; }

    }

    public partial class OnlineApplicationDetailsTrans
    {
        public OnlineFormViewModel objOnlineFormViewModel = new OnlineFormViewModel();
        public Nullable<int> OnlineApplicationId { get; set; }
        public string TrKey { get; set; }
        public string Txnid { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Productinfo { get; set; }
        public string Udf1 { get; set; }
        public string Udf2 { get; set; }
        public string Udf3 { get; set; }
        public string Udf4 { get; set; }
        public string Udf5 { get; set; }
        public string Mihpayid { get; set; }
        public string Mode { get; set; }
        public Nullable<int> TranStatus { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<int> Status { get; set; }
        public string StatusName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
        public string payment_source { get; set; }
        public string PG_Type { get; set; }
        public string bank_ref_num { get; set; }
        public string bankcode { get; set; }
        public string error { get; set; }
        public string error_Message { get; set; }
        public string name_on_card { get; set; }
        public string cardnum { get; set; }
        public string cardhash { get; set; }
        public string issuing_bank { get; set; }
        public string card_type { get; set; }
    }

    #endregion End Manage Application Form

    #region Manage Request
    public class ManageRequestModel
    {
        public string SchemeName { get; set; }
        public string Status { get; set; }
        public string ReuestedBy { get; set; }
        public string ApprovedBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ApprovedOn { get; set; }
        public int? ApplicationId { get; set; }
        public int? SchemeId { get; set; }
        public int? DepartmentId { get; set; }
        public string PropertyNo { get; set; }
        public string FormNo { get; set; }
        public string Department { get; set; }
        public string ApplicantName { get; set; }
        public string ProperyType { get; set; }
        public string Comment { get; set; }
        public string From { get; set; }
        public int RID { get; set; }
        public decimal? EarnestMoney { get; set; }
        public decimal? AllotmentMoney { get; set; }
        public decimal? Area { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> AllotmentDate { get; set; }
        public string MobileNo { get; set; }
        public Decimal? AmountDeposited { get; set; }
        public int ApplicationID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? RequestedReceived { get; set; }
        public string EncryptedSchemeId { get { return CommonHelper.Encode(SchemeId.ToString()); } }
        public string EncryptedDeptId { get { return CommonHelper.Encode(DepartmentId.ToString()); } }
        public string EncryptedAllotmentDate { get { return CommonHelper.Encode(AllotmentDate.ToString()); } }
        public string EncryptedRequestedReceived { get { return CommonHelper.Encode(RequestedReceived.ToString()); } }
        public string BankName { get; set; }
        public string PermanentAddress { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
    #endregion End Manage Request

    public class AllotmentModel
    {
        public int RID { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> KYAStatusId { get; set; }
        public Nullable<int> PropertyUpdateId { get; set; }
        public Nullable<int> DepartmentUpdateId { get; set; }
        public Nullable<int> AccountsUpdateId { get; set; }
        public Nullable<int> OrganisationUpdateId { get; set; }
        public string EncryptedSchemeId { get { return CommonHelper.Encode(SchemeId.ToString()); } }
        public string SchemeName { get; set; }
       
        public string FormNo { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public string PropertyNumber { get; set; }
        public string ApplicantName { get; set; }
        public Nullable<System.DateTime> AllotmentDate { get; set; }
        public Nullable<System.DateTime> InstalmentStartDate { get; set; }
        public Nullable<int> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public PropertyAllotmentModel PropertyAllotment { get; set; }
        public ApplicationFormModel ApplicationForm { get; set; }
        [Range(1.0, 9999999999999.99, ErrorMessage = "Total Area must be decimal(15,2) and greater than 0")]
        public Nullable<decimal> TotalArea { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public string isStatus { get; set; }
        public Nullable<int> HdnPropertyID { get; set; }
        public string HdnFormNo { get; set; }
        public string DepartmentName { get; set; }
        public decimal? AllotmentMoney { get; set; }
        public decimal? LandRatePerSqMet { get; set; }
        public double? AllotmentMoneyPercent { get; set; }
        public decimal? EarnestMoney { get; set; }
        public decimal? TotalPropertyCost { get; set; }
        public DateTime? AllotmentMoneyDueDate { get; set; }
        public string Gender { get; set; }
        public decimal? AmountPaid { get; set; }
        public DateTime? DepositDate { get; set; }
        public Nullable<bool> IsSubleased { get; set; }
        public string RegistryType { get; set; }
        public string KYAStatus { get; set; }
        public string MobileNumber { get; set; }
        public string FatherOrHusbandName { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public string PropertyNo { get; set; }
        public string CorresspondingAddress { get; set; }
        public string PermanentAddress { get; set; }
        private string encodedParam;
        public string EncodedParameter
        {
            get { return encodedParam = CommonHelper.Encode(RID.ToString()); }
            set { encodedParam = CommonHelper.Encode(RID.ToString()); }
        }
        public bool IsThisPropertyHasDocument { get; set; }
        public int? OldRID { get; set; }
        public Nullable<bool> IsPropertyValidated { get; set; }
        public Nullable<bool> IsValidatedByDepartment { get; set; }
        public Nullable<bool> IsValidatedByAccounts { get; set; }
        public Nullable<bool> IsValidatedByOrganisation { get; set; }
    }

    public class DrawSlipModel
    {
        public int ApplicationId { get; set; }
        public int SchemeId { get; set; }
        public string SchemeName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string FormNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ApplicantName
        {
            get
            {
                return FirstName + " " + MiddleName + " " + LastName;
            }
        }
        public string MobileNumber { get; set; }
        public string CorresspondAddress { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string PanNumber { get; set; }
        public string BankName { get; set; }
        public int? PropertyTypeId { get; set; }
        public string PropertyTypeName { get; set; }
    }

    public class SchemePropertyTransDetail
    {
        public int RefId { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> PropertyTypeId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> FloorId { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> Frequency { get; set; }
        public Nullable<int> ParentPropertyId { get; set; }
        public Nullable<int> SchemeTypeId { get; set; }
        public Nullable<int> TotalnoofInstallment { get; set; }
        public Nullable<int> RequestId { get; set; }

        public string SchemeName { get; set; }
        public string SchemeType { get; set; }    
        public string DepartmentName { get; set; }      
        public string PropertyTypeName { get; set; }      
        public string SectorName { get; set; }      
        public string BlockName { get; set; }       
        public string FloorName { get; set; }       
        public string PropertyNo { get; set; }        
        public string Registry { get; set; }
        public string ApplicantName { get; set; }
        public string Gender { get; set; }
        public string LocationType { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public Nullable<decimal> TotalArea { get; set; }
        public Nullable<decimal> CoveredArea { get; set; }
        public Nullable<decimal> ActualArea { get; set; }
        public Nullable<decimal> PropertyCost { get; set; }
        public Nullable<decimal> TotalPropertyCost { get; set; }
        public Nullable<decimal> AllotmentMoney { get; set; }
        public Nullable<decimal> CivilCost { get; set; }
        public Nullable<decimal> LandRate { get; set; }
        public Nullable<decimal> LocationCharges { get; set; }
        public Nullable<decimal> EarnestMoney { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Boolean RecordExistsIn { get; set; }

        //public Nullable<decimal> TotalPropertyCost { get; set; }       
        public Nullable<System.DateTime> AllotmentDate { get; set; }
        public Nullable<System.DateTime> InstalmentStartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }

    #region "Manage Allotee List "

    public class AllotteeListModel
    {
        public int SchemeId { get; set; }
        public string SchemeName { get; set; }
        public string Status { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> SubmittedDate { get; set; }
        public Nullable<System.DateTime> AllotmentDate { get; set; }

        public int RID { get; set; }
        public string PropertyNo { get; set; }
        public string FormNo { get; set; }
        public string ApplicantName { get; set; }
        public string ProperyType { get; set; }
        public decimal? EarnestMoney { get; set; }
        public decimal? AllotmentMoney { get; set; }
        public decimal? Area { get; set; }
        public string Comment { get; set; }
        [DisplayName("Assign To* :  ")]
        [Required(ErrorMessage = "Assign To is required")]
        public string User { get; set; }
        public string From { get; set; }
        public int? ApplicationId { get; set; }
        public int PropertyId { get; set; }
        public int PropRefId { get; set; }

        public string ApproverByName { get; set; }

        public string EncryptedSchemeId { get { return CommonHelper.Encode(SchemeId.ToString()); } }
        public string EncryptedDeptId { get { return CommonHelper.Encode(DepartmentId.ToString()); } }
        public string EncryptedAllotmentDate { get { return CommonHelper.Encode(AllotmentDate.ToString()); } }
        public string EncryptedAppId { get { return CommonHelper.Encode(ApplicationId.ToString()); } }
        public string EncryptedPropRefId { get { return CommonHelper.Encode(PropRefId.ToString()); } }
    }

    #endregion

    public class PropertyAllotmentModel
    {
        public int AllotmentId { get; set; }
        public Nullable<int> RID { get; set; }
        [Required(ErrorMessage = "Scheme Name is required")]
        public Nullable<int> SchemeId { get; set; }
        public string SchemeName { get; set; }
        [Required(ErrorMessage = "Department Name is required")]
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string ApplicantName { get; set; }
        public string PropertyNo { get; set; }
        public Nullable<bool> Approved { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<DateTime> AllotmentDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }

        public List<SchemeMst> SchemeList { get; set; }
        public List<UmDepartmentMaster> DepartmentList { get; set; }
    }

    public class SchemeAllotmentModel
    {
        public int schemeId { get; set; }
        public Nullable<int> schemeTypeId { get; set; }
        public string schemeName { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string createdBy { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        public string modifiedBy { get; set; }
        public Nullable<System.DateTime> modifiedDate { get; set; }
        public Nullable<bool> completed { get; set; }
    }
    public class DepartmentAllotmentModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<bool> Status { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }

    public class ApplicationPayment
    {
        public int Id { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<int> FormNo { get; set; }
        public string PaymentMode { get; set; }
        public Nullable<int> BankId { get; set; }
        public string BankName { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string BranchName { get; set; }
        public Nullable<decimal> AmountDeposited { get; set; }
        public string DdNo { get; set; }
        public string DdIssueBank { get; set; }
        public Nullable<System.DateTime> DdIssueDate { get; set; }
        public Nullable<int> Utn { get; set; }
        public Nullable<int> ChallanNo { get; set; }
        public Nullable<System.DateTime> ChallanIssueDate { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }

    public class SchemeDepartmentModel
    {
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
    }

    public class PaymentSchedule
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

        public Nullable<bool> IsInstallmentPaid { get; set; }
    }

    public class ApplicantPropertyBasicInfo
    {
        public string ApplicantName { get; set; }
        public string PropertyNo { get; set; }
    }
    public class DetailedPropertyView
    {
        public AllotmentModel AllotmentModel { get; set; }
        public ApplicationFormModel ApplicationDetail { get; set; }
        public PaymentSchedule PaymentSchedule { get; set; }
        public AlloteeBasicInfo AlloteeBasicInfo { get; set; }
        public PropertyDocument PropertyDocument { get; set; }
        public AllottedPropertyDetails AllottedPropertyDetails { get; set; }
        public PaymentLedger PaymentLedger { get; set; }
        public LetterHistory letterHistory { get; set; }
        public NotingDetailsModel Noting { get; set; }
        public bool isReschedule { get; set; }
        public TransferModel transferModel { get; set; }
        public OtherDetails OtherDetails { get; set; }
        public NDCVeiwModel NDCVeiwModel { get; set; }
        public SubLeaseViewModel SubLeaseViewModel { get; set; }
        public InstallmentDues_Payment InstallmentDuesPayment { get; set; }
        public PaymentViewModel PaymentModel { get; set; }
        public KYAViewModel KYAViewModel { get; set; }
        public ServiceViewModel ServiceModel { get; set; }
    }

    public class PaymentLedger
    {
        public int Id { get; set; }
        public Nullable<int> Rid { get; set; }
        public Nullable<System.DateTime> Entry_Date { get; set; }
        public Nullable<int> Receipt_Head_Id { get; set; }
        public Nullable<int> Receipt_Subhead_Id { get; set; }
        public string ReceiptSubheadName { get; set; }
        public Nullable<decimal> Debit_Amount { get; set; }
        public Nullable<decimal> Credit_Amount { get; set; }
        public Nullable<decimal> Balance_Amount { get; set; }
        public string Bal_Amount { get; set; }
        public Nullable<bool> Is_Active { get; set; }
        public Nullable<int> Created_By { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public Nullable<int> Modified_By { get; set; }
        public Nullable<System.DateTime> Modified_Date { get; set; }
    }

    public class LetterHistory
    {
        public int Id { get; set; }
        public Nullable<int> Department_Id { get; set; }
        public Nullable<int> User_Id { get; set; }
        public Nullable<int> Rid { get; set; }
        public string Template_Html { get; set; }
        public Nullable<System.DateTime> Generate_Date { get; set; }
        public Nullable<bool> Is_Active { get; set; }
        public Nullable<int> Created_By { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public Nullable<int> Modified_By { get; set; }
        public Nullable<System.DateTime> Modified_Date { get; set; }
        public Nullable<int> Template_Id { get; set; }
        public int Sno { get; set; }
        public string LetterName { get; set; }
    }

    public class PaymentReceipt
    {
        public int ID { get; set; }
        public Nullable<long> RECEIPT_ID { get; set; }
        public Nullable<int> DEPT_CODE { get; set; }
        public Nullable<int> RECEIPT_HEAD_ID { get; set; }
        public string RECEIPT_HEAD_Name { get; set; }
        public Nullable<int> RECEIPT_SUBHEAD_ID { get; set; }
        public string RECEIPT_SUBHEAD_Name { get; set; }
        public string CHALLAN_ID { get; set; }
        public Nullable<System.DateTime> DEPOSIT_DATE { get; set; }
        public Nullable<decimal> AMOUNT_PAID { get; set; }
        public Nullable<int> STATUS { get; set; }
        public string USERID { get; set; }
        public Nullable<System.DateTime> ENTRY_DATE { get; set; }
    }

    public class AlloteeBasicInfo
    {
        public string ApplicantName { get; set; }
        public string FatherName { get; set; }
        public string SignatoryAuthority { get; set; }
        public string RegisteredOffice { get; set; }
        public string DepartmentName { get; set; }
        public string PropertyType { get; set; }
        public string PropertyNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? AllotmentDate { get; set; }
    }

    public class AllottedPropertyDetails
    {
        public Nullable<int> RID { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public string SchemeName { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string FormNo { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public string ApplicantName { get; set; }
        public int AllotmentId { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public string PropertyNumber { get; set; }
        public Nullable<System.DateTime> AllotmentDate { get; set; }
        public Nullable<System.DateTime> AllotmentDueDate { get; set; }
        public Nullable<System.DateTime> ChecklistDate { get; set; }
        public Nullable<int> ChecklistTypeId { get; set; }
        public string ChecklistType { get; set; }
        public string DocumentForChecklist { get; set; }
        public Nullable<System.DateTime> RegistryDueDate { get; set; }
        public Nullable<System.DateTime> RegistryDoneDate { get; set; }
        public Nullable<System.DateTime> LeaseDeedDate { get; set; }
        public Nullable<System.DateTime> LeaseDeedDueDate { get; set; }
        public Nullable<System.DateTime> PossessionDate { get; set; }
        public Nullable<System.DateTime> PossessionDueDate { get; set; }
        public Nullable<System.DateTime> PossessionOrderDate { get; set; }

        public Nullable<System.DateTime> Approved_Date { get; set; }
        public Nullable<System.DateTime> Mutation_Date { get; set; }
        public Nullable<System.DateTime> FunctionalDate { get; set; }
        public Nullable<System.DateTime> RentingDate { get; set; }
        public Nullable<System.DateTime> MortgageDate { get; set; }
        public Nullable<System.DateTime> Extension_Given_Date { get; set; }
        public Nullable<int> TransferCount { get; set; }
        public Nullable<int> MortgageCount { get; set; }
        public Nullable<int> RentPermissionCount { get; set; }
        public Nullable<int> ExtensionCount { get; set; }
    }

    public class PropertyDocument
    {
        public int RID { get; set; }
        public int DeptId { get; set; }
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
    }

    public class AllotmentLetterModel
    {
        public string ApplicantName { get; set; }
        public string ApplicantAddress { get; set; }
        public string DepttName { get; set; }
        public int RId { get; set; }
        public string PropType { get; set; }
        public string PropNo { get; set; }
        public string SchemeName { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public decimal? PropRate { get; set; }
        public decimal? AllotedArea { get; set; }
        public decimal EarnestMoney { get; set; }
        public string TotalPremium { get; set; }
        public string AllotmentMode { get; set; }
        public string BalanceAllotmentMoney { get; set; }
        public DateTime? AllotmentDate { get; set; }
        public decimal AdjustedAllotmentMoney { get; set; }
        public string PaymentMode { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal AnnualLeaseRent { get; set; }
        public double? InterestRate { get; set; }
        public int NoOfInstallments { get; set; }
        public decimal AllotmentAmount { get; set; }
    }


    public class ExtensionDetails
    {
        public int Id { get; set; }
        public Nullable<int> Rid { get; set; }
        public string PropertyNo { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public Nullable<System.DateTime> Completion_DueDate { get; set; }
        public Nullable<System.DateTime> Extension_Given_Date { get; set; }
        public Nullable<System.DateTime> Extension_Due_Date { get; set; }
        public Nullable<decimal> Extension_Charge { get; set; }
        public Nullable<int> Approved_By { get; set; }
        public Nullable<System.DateTime> Approved_Date { get; set; }
        public string Comment { get; set; }
        public Nullable<bool> Is_Active { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> Created_By { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public Nullable<int> Modified_By { get; set; }
        public Nullable<System.DateTime> Modified_Date { get; set; }
        public string User { get; set; }
        public int? ScheduleActionDay { get; set; }
        public string SchemeName { get; set; }
        public string DepartmentName { get; set; }
        public string ApplicantName { get; set; }
        public string Gender { get; set; }
        public string FatherName { get; set; }
        public string PropertyType { get; set; }
        public decimal? Area { get; set; }
        public string PropertyNumber { get; set; }
        public string Floor { get; set; }
        public DateTime? Leasedeeddate { get; set; }
        public DateTime? Possessiondate { get; set; }
        public string BuildingPlan { get; set; }
        public string StatusName { get; set; }
        public string AssignTo { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public bool ExtendableFlag { get; set; }
        public bool IsSelected { get; set; }
        public Nullable<int> OnlineRequestRefNo { get; set; }
    }
    public class HTMLClass
    {
        public string LiteralStr { get; set; }
        public string city { get; set; }
        public string curl { get; set; }
        public string address2 { get; set; }
        public string phone { get; set; }
        public string furl { get; set; }
        public string state { get; set; }
        public string udf1 { get; set; }
        public string address1 { get; set; }
        public decimal? amount { get; set; }
        public string txnid { get; set; }
        public string udf2 { get; set; }
        public string email { get; set; }
        public string productinfo { get; set; }
        public string udf3 { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string zipcode { get; set; }
        public string udf4 { get; set; }
        public string pg { get; set; }
        public string country { get; set; }
        public string surl { get; set; }
        public string hash { get; set; }
        public string udf5 { get; set; }
        public string key { get; set; }
    }

    public class DemoModel
    {
        [Required]
        public string firstName { get; set; }
        [Required]
        public string amount { get; set; }
        [Required]
        public string phone { get; set; }
        [Required]
        public string prodInfo { get; set; }
        [Required]
        public string surl { get; set; }
        [Required]
        public string furl { get; set; }
        [Required]
        public string email { get; set; }



    }

    public class ApplicatandDetailsModel
    {
        public int Appld { get; set; }
        public string MobNu { get; set; }
        public string EmailId { get; set; }
    }

    public class UpdateAddress
    {
        [Required(ErrorMessage = "RID is required")]
        public string RID { get; set; }

        public string TCorrespondenceAdd { get; set; }
        public string TPermanentAdd { get; set; }
        public int ApplicationId { get; set; }
        public string Name { get; set; }

    }

    public class UpdateDetails
    {
        [Required(ErrorMessage = "RID is required")]
        public string RID { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\.\-\&\ ]*$", ErrorMessage = "First Name must be alphabets only (space allowed).")]
        public string FirstName { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\.\-\&\ ]*$", ErrorMessage = "Middle Name must be alphabets only (space allowed).")]
        public string MiddleName { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\.\-\&\ ]*$", ErrorMessage = "Field must be alphabets only (space allowed).")]
        public string LastName { get; set; }

        public Nullable<System.DateTime> LeaseDeedDate { get; set; }
        public Nullable<decimal> Area { get; set; }
        public Nullable<decimal> LandRate { get; set; }
        public Nullable<decimal> PropertyCost { get; set; }
        public Nullable<System.DateTime> PossessionDate { get; set; }
    }


    public class OtherDetails
    {
        public int? RId { get; set; }
        public List<RentModel> ObjRentModel { get; set; }
    }


    public class RemarksDetailsModel
    {
        public int? RId { get; set; }
        public string Remarks { get; set; }
    }
}
