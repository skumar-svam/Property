using NA.PMS.NICService.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.NICServices
{
    public class SWPEpassViewModel
    {
        public Nullable<int> Id { get; set; }
        public string EpassNo { get; set; }
        public string EpassType { get; set; }
        public string Applicant { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string LocalPlace { get; set; }
        public string PIN { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string Purpose { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNo { get; set; }
        public string RCNo { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public string PhotoPath { get; set; }
        public string UserIdPath { get; set; }
        public string RCPath { get; set; }
        public string LicencePath { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<DateTime> ApprovalDate { get; set; }
        public string Approver { get; set; }
        public string Comment { get; set; }
        public string EPDI { get; set; }
        public string EPDII { get; set; }
        public string EPDIII { get; set; }
        public string EPDIV { get; set; }
        public string EPDV { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsAuthorityEmployee { get; set; }
        public string ApplicantImage { get; set; }
        public Nullable<bool> IsDeclarationChecked { get; set; }
        public string Status { get; set; }
        public Nullable<DateTime> ValidTillDate { get; set; }
        public Nullable<int> ActionTypeId { get; set; }
    }

    //public class OnlineFormViewModel
    //{
    //    public Nullable<int> Id { get; set; }
    //    public Nullable<int> FlagId { get; set; }
    //    public Nullable<int> UserId { get; set; }
    //    public Nullable<int> FormTypeId { get; set; }
    //    public Nullable<int> ApplicationFormId { get; set; }
    //    public Nullable<int> RegistrationId { get; set; }
    //    public Nullable<int> SchemeId { get; set; }
    //    public Nullable<int> DepartmentId { get; set; }
    //    public Nullable<int> PropertyId { get; set; }
    //    public Nullable<int> PropertyTypeId { get; set; }
    //    public Nullable<int> ApplicantTypeId { get; set; }
    //    public Nullable<int> GenderId { get; set; }
    //    public Nullable<int> Age { get; set; }
    //    public Nullable<int> MaritalStatusId { get; set; }
    //    public Nullable<int> ReligionId { get; set; }
    //    public Nullable<int> CategoryId { get; set; }
    //    public Nullable<int> OccupationId { get; set; }
    //    public Nullable<int> CompanyTypeId { get; set; }
    //    public Nullable<int> SigningAuthorityId { get; set; }
    //    public Nullable<int> SchemeStatus { get; set; }
    //    public Nullable<int> RefundBankId { get; set; }
    //    public Nullable<int> FormStatusId { get; set; }
    //    public Nullable<int> BankId { get; set; }
    //    public Nullable<int> BranchId { get; set; }
    //    public Nullable<int> ChallanBankId { get; set; }
    //    public Nullable<int> ChallanBranchId { get; set; }
    //    public Nullable<int> PaymentModeId { get; set; }
    //    public Nullable<int> StatusId { get; set; }
    //    public Nullable<int> AreaRangeTypeId { get; set; }
    //    public Nullable<int> ScrutinyStatusId { get; set; }
    //    public Nullable<int> ReturnTypeId { get; set; }

    //    public string NICControlId { get; set; }
    //    public string NICUnitId { get; set; }
    //    public string NICServiceId { get; set; }
    //    public string NICProcessIndustryId { get; set; }
    //    public string NICApplicationId { get; set; }
    //    public string NICFeeStatus { get; set; }
    //    public string NICFeeStatusId { get; set; }
    //    public string NICXmlInputTable { get; set; }
    //    public string NICXmlOutputTable { get; set; }

    //    public string Department { get; set; }
    //    public string FormNo { get; set; }
    //    public string PropertyType { get; set; }
    //    public string SchemeName { get; set; }
    //    public string ApplicationFormType { get; set; }
    //    public string ExpansionType { get; set; }
    //    public string EncryptedFormId { get { return ApplicationFormId != null ? SWPEncryption.Encode(ApplicationFormId.ToString()) : null; } }
    //    public string ApplicationForm { get; set; }
    //    //for individual/company name
    //    public string Applicant { get; set; }
    //    //father/husband/signing authority name
    //    public string ApplicantMaster { get; set; }
    //    //for male/female/company type
    //    public string ApplicantType { get; set; }
    //    public string Gender { get; set; }
    //    public string MaritalStatus { get; set; }
    //    public string Religion { get; set; }
    //    public string CategoryName { get; set; }
    //    public string Occupation { get; set; }
    //    public string CompanyType { get; set; }
    //    public string RegisteredOffice { get; set; }
    //    public string SigningAuthority { get; set; }
    //    public string SignatoryStatus { get; set; }
    //    public string CorrespondingAddress { get; set; }
    //    public string PermanentAddress { get; set; }
    //    public string BranchName { get; set; }
    //    public string Sector { get; set; }
    //    public string Block { get; set; }
    //    public string PlotNo { get; set; }
    //    public string PropertyNo { get; set; }
    //    public string ApplicantGSTNumber { get; set; }
    //    public string RefundBank { get; set; }
    //    public string RefundInfaverof { get; set; }
    //    public string Area { get; set; }
    //    public string AreaRange { get; set; }
    //    public string AreaRangeId { get; set; }
    //    public string PaymentMode { get; set; }
    //    public string DocumentsTable { get; set; }
    //    public string FormStatus { get; set; }
    //    public string Comment { get; set; }
    //    public string PaymentTypeId { get; set; }
    //    public string AadharNumber { get; set; }
    //    public string ChecklistHtml { get; set; }
    //    public string UserImage { get; set; }
    //    public string SignImage { get; set; }
    //    public string ChallanBank { get; set; }
    //    public string ChallanBranchName { get; set; }
    //    public string IsDeleted { get; set; }
    //    public string UserRoleType { get; set; }
    //    public string IsPaid { get; set; }
    //    public string PayType { get; set; }//for rtgs,other      
    //    public string BankIFSCCode { get; set; }
    //    public string VirtualAccPrefix { get; set; }
    //    public string ChallanStatus { get; set; }
    //    public string FormType { get; set; }
    //    public string UserPassword { get; set; }
    //    public string SchemeType { get; set; }
    //    public string ErrorMessage { get; set; }
    //    public string PreviousFormNo { get; set; }
    //    public string AppType { get; set; }
    //    public string ServiceAppPayStatus { get; set; }
    //    public string ProcessType { get; set; }
    //    public string User { get; set; }
    //    public string LetterCode { get; set; }
    //    public string ExistingProperty { get; set; }
    //    public string ActionType { get; set; }
    //    public string PaymentStatus { get; set; }
    //    public string VirtualAccountPrefix { get; set; }
    //    public string AccountNo { get; set; }
    //    public string ReturnType { get; set; }
    //    public string Message { get; set; }
    //    public string TransactionId { get; set; }

    //    [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "First name mMust be alphabets.")]
    //    public string FirstName { get; set; }

    //    [RegularExpression(@"^[a-zA-Z. ]*$", ErrorMessage = "Middle name must be alphabets.")]
    //    public string MiddleName { get; set; }

    //    [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "Last name must be alphabets.")]
    //    public string LastName { get; set; }

    //    [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@(([a-zA-Z\-]+\.)+)([a-zA-Z]{2,4})$", ErrorMessage = "Please enter a valid e-mail address")]
    //    public string Email { get; set; }

    //    [RegularExpression(@"^[a-zA-Z. ]*$", ErrorMessage = "Father name must be alphabets.")]
    //    public string FatherName { get; set; }

    //    [RegularExpression(@"^[a-zA-Z. ]*$", ErrorMessage = "Mother name must be alphabets.")]
    //    public string MotherName { get; set; }

    //    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Mobile Number must be numeric.")]
    //    public string MobileNumber { get; set; }

    //    [RegularExpression(@"^[0-9- ]*$", ErrorMessage = "Phone Number must be numeric.")]
    //    public string PhoneNumber { get; set; }

    //    [RegularExpression(@"^[0-9- ]*$", ErrorMessage = "Phone Number must be numeric.")]
    //    public string FaxNumber { get; set; }

    //    [Required(ErrorMessage = "Pan Number is required")]
    //    [RegularExpression(@"^[A-Z0-9]*$", ErrorMessage = "PAN must be in capital alphanumeric.")]
    //    public string PanNumber { get; set; }

    //    //[RegularExpression(@"^[^\s]{4}\d{7}$", ErrorMessage = "IFSC Code must be alphanumeric.")]
    //    [RegularExpression(@"^[0-9A-Z]*$", ErrorMessage = "IFSC Code must be alphanumeric.")]
    //    public string IFSCCode { get; set; }

    //    //[Range(1000000000, 9999999999999999, ErrorMessage = "Account Number must be between 11 To 16 digits.")]
    //    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Account Number must be numeric.")]
    //    public string RefundAccountNo { get; set; }

    //    [Range(1000000000, 9999999999999999, ErrorMessage = "Account Number must be between 11 To 16 digits.")]
    //    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Account Number must be numeric.")]
    //    public string ChallanAccountNo { get; set; }

    //    public Nullable<Decimal> AnnualIncome { get; set; }
    //    public Nullable<decimal> EarnestMoney { get; set; }
    //    public Nullable<decimal> ApplicationFee { get; set; }
    //    public Nullable<decimal> TotalAmount { get; set; }
    //    public Nullable<decimal> ProcessingCharge { get; set; }
    //    public Nullable<decimal> FormFeeGST { get; set; }
    //    public Nullable<decimal> ProcessingChargeGST { get; set; }
    //    public Nullable<decimal> FormFeeSGST { get; set; }
    //    public Nullable<decimal> FormFeeCGST { get; set; }
    //    public Nullable<decimal> ProcessingSGST { get; set; }
    //    public Nullable<decimal> ProcessingCGST { get; set; }
    //    public Nullable<decimal> TotalAmountGST { get; set; }
    //    public Nullable<decimal> FormFeeWithGST { get; set; }
    //    public Nullable<decimal> ProcessingChargeWithGST { get; set; }

    //    public Nullable<bool> IsApplicationFeePaid { get; set; }
    //    public Nullable<bool> IsRegistrationFeePaid { get; set; }
    //    public Nullable<bool> IsSchemeActive { get; set; }
    //    public Nullable<bool> IsFormActive { get; set; }
    //    public Nullable<bool> IsDocumentUploaded { get; set; }
    //    public Nullable<bool> IsFormSubmited { get; set; }
    //    public Nullable<bool> IsPreviousChallanUploaded { get; set; }
    //    public Nullable<bool> IsPaidChallanUploaded { get; set; }
    //    public Nullable<bool> IsFromNIC { get; set; }
    //    public Nullable<bool> IsDirectorDetailsSaved { get; set; }
    //    public Nullable<bool> IsProposedProjectSaved { get; set; }
    //    public Nullable<bool> IsPropertyAllotted { get; set; }
    //    public Nullable<bool> IsDrawSucceeded { get; set; }
    //    public Nullable<bool> IsFormRejected { get; set; }
    //    public bool IsChallanGenerated { get; set; }
    //    public bool PaidThroughSWP { get; set; }//Paid through single window portal

    //    public Nullable<DateTime> DOB { get; set; }
    //    public Nullable<DateTime> SchemeStartDate { get; set; }
    //    public Nullable<DateTime> SchemeEndDate { get; set; }
    //    public Nullable<DateTime> FormSubmissionDate { get; set; }
    //    public Nullable<DateTime> CommentDate { get; set; }
    //    public Nullable<DateTime> SubmitDate { get; set; }
    //    public Nullable<DateTime> ApprovalDate { get; set; }
    //    public Nullable<DateTime> AmountPaidDate { get; set; }
    //    public Nullable<DateTime> DispatchDate { get; set; }
    //    public Nullable<DateTime> LetterDate { get; set; }
    //    public Nullable<DateTime> ValidTillDate { get; set; }
    //    public Nullable<DateTime> SchemeDrawDate { get; set; }
    //    public Nullable<DateTime> AllottmentDate { get; set; }

    //    public OnlinePaymentViewModel PaymentModel { get; set; }
    //    public OnlineDirectorViewModel DirectorModel { get; set; }
    //    public ProposedCompanyViewModel ProposedModel { get; set; }
    //    public NICsingalwindowSystem NICSingleWindowModel { get; set; }
    //    public ResultMessage ResultMessage { get; set; }
    //    //public NewDataSet BasicDetailsGetModel { get; set; }

    //    //public NICInfoDetailViewModel NICInfoDetailViewModel { get; set; }
    //    //public List<DocumentDetail> DocDetails;
    //    //public List<DropdownViewModel> BankModel { get; set; }

    //    public int[] ApplicationIdList { get; set; }
    //    public Nullable<int>[] DocList { get; set; }
    //    public string[] StrDocList { get; set; }

    //    public HttpPostedFileBase UserImageFile { get; set; }
    //    public HttpPostedFileBase UserSignatureFile { get; set; }
    //    public IEnumerable<HttpPostedFileBase> UploadedFileList { get; set; }
    //}

    //public class OnlinePaymentViewModel
    //{
    //    public Nullable<int> Id { get; set; }
    //    public Nullable<int> BankId { get; set; }
    //    public Nullable<int> BranchId { get; set; }
    //    public Nullable<int> DepartmentId { get; set; }
    //    public Nullable<int> RegistrationId { get; set; }
    //    public Nullable<int> OnlineRequestId { get; set; }
    //    public Nullable<int> ApplicationFormId { get; set; }
    //    public Nullable<int> TransactionStatusId { get; set; }
    //    public Nullable<int> StatusId { get; set; }
    //    public Nullable<int> ChallanId { get; set; }
    //    public Nullable<int> ActionTypeId { get; set; }
    //    public Nullable<int> ReturnTypeId { get; set; }
    //    public Nullable<int> ServiceId { get; set; }
    //    public Nullable<int> AccountHeadId { get; set; }
    //    public Nullable<int> AccountSubHeadId { get; set; }
    //    public Nullable<int> PaymentTypeId { get; set; }
    //    public Nullable<int> SectorId { get; set; }
    //    public Nullable<int> BlockId { get; set; }
    //    public Nullable<int> ServiceRefId { get; set; }

    //    public Nullable<decimal> Amount { get; set; }
    //    public Nullable<decimal> Discount { get; set; }

    //    public string TransactionKey { get; set; }
    //    public string EncryptedFormId { get { return ApplicationFormId == null ? null : SWPEncryption.Encode(ApplicationFormId.ToString()); } }
    //    public string TransactionId { get; set; }
    //    public string ProductInfo { get; set; }
    //    public string Udf1 { get; set; }
    //    public string Udf2 { get; set; }
    //    public string Udf3 { get; set; }
    //    public string Udf4 { get; set; }
    //    public string Udf5 { get; set; }
    //    public string Mihpayid { get; set; }
    //    public string Mode { get; set; }
    //    public string TransactionStatus { get; set; }
    //    public string Status { get; set; }
    //    public string Applicant { get; set; }
    //    public string FirstName { get; set; }
    //    public string Email { get; set; }
    //    public string Mobile { get; set; }
    //    public string MobileNo { get; set; }
    //    public string PhoneNumber { get; set; }
    //    public string PaymentSource { get; set; }
    //    public string PG_Type { get; set; }
    //    public string GatewayName { get; set; }
    //    public string BankReferenceNo { get; set; }
    //    public string BankName { get; set; }
    //    public string BranchAddress { get; set; }
    //    public string AccountNo { get; set; }
    //    public string VirtualAccountNo { get; set; }
    //    public string BankCode { get; set; }
    //    public string Error { get; set; }
    //    public string ErrorMessage { get; set; }
    //    public string NameOnCard { get; set; }
    //    public string CardNumber { get; set; }
    //    public string CardHash { get; set; }
    //    public string IssuingBank { get; set; }
    //    public string CardType { get; set; }
    //    public string docPath { get; set; }
    //    public string ServiceName { get; set; }
    //    public string ActionType { get; set; }
    //    public string ReturnType { get; set; }
    //    public string Department { get; set; }
    //    public string AccountHead { get; set; }
    //    public string AccountSubHead { get; set; }
    //    public string PaymentType { get; set; }
    //    public string City { get; set; }
    //    public string State { get; set; }
    //    public string Country { get; set; }
    //    public string Zipcode { get; set; }
    //    public string Pincode { get; set; }
    //    public string AddressI { get; set; }
    //    public string AddressII { get; set; }
    //    public string Sector { get; set; }
    //    public string Block { get; set; }
    //    public string PlotNo { get; set; }
    //    public string HtmlContent { get; set; }
    //    public string ChallanRefId { get; set; }
    //    public string PAN { get; set; }
    //    public string GSTNo { get; set; }

    //    public Nullable<bool> IsOnlinePaid { get; set; }
    //    public Nullable<bool> IsOfflinePaid { get; set; }
    //    public Nullable<bool> IsChallanVerifed { get; set; }

    //    public Nullable<DateTime> modifiydate { get; set; }
    //    public Nullable<DateTime> EntryDate { get; set; }
    //    public Nullable<DateTime> ChallanDate { get; set; }
    //    public Nullable<DateTime> StartDate { get; set; }
    //    public Nullable<DateTime> EndDate { get; set; }

    //    public OnlineFormViewModel FormModel { get; set; }
    //}

    //public class OnlineDirectorViewModel
    //{
    //    public Nullable<int> Id { get; set; }
    //    public Nullable<int> ApplicationFormId { get; set; }
    //    public Nullable<int> DirectorId { get; set; }
    //    public Nullable<int> DirectorTypeId { get; set; }
    //    public Nullable<int> IsDirectorActive { get; set; }
    //    public Nullable<int> CreatedBy { get; set; }
    //    public Nullable<int> ActionTypeId { get; set; }
    //    public string DirectorName { get; set; }
    //    public string DirectorType { get; set; }
    //    public string PAN { get; set; }
    //    public string ActionType { get; set; }
    //    public Nullable<decimal> DirectorShare { get; set; }

    //    public bool status { get; set; }
    //}

    //public class OnlineDocumentViewModel
    //{
    //    public Nullable<int> Id { get; set; }
    //    public Nullable<int> Sno { get; set; }
    //    public Nullable<int> SchemeId { get; set; }
    //    public Nullable<int> SchemeTypeId { get; set; }
    //    public Nullable<int> DepartmentId { get; set; }
    //    public Nullable<int> PropertyTypeId { get; set; }
    //    public Nullable<int> ApplicationFormId { get; set; }
    //    public Nullable<int> DocumentId { get; set; }
    //    public Nullable<int> FilterTypeId { get; set; }

    //    public string DocumentName { get; set; }
    //    public string DocumentType { get; set; }
    //    public string ParentDocumentType { get; set; }
    //    public string UploadedDocument { get; set; }
    //    public string PathName { get; set; }
    //    public string SchemeName { get; set; }
    //    public string Department { get; set; }
    //    public string PropertyType { get; set; }
    //    public string FilterType { get; set; }
    //    public string SchemeType { get; set; }
    //    public string CheckListType { get; set; }
    //    public string Status { get; set; }
    //    public Nullable<bool> IsActive { get; set; }
    //    public bool IsSelected { get; set; }
    //}

    //public class ProposedCompanyViewModel
    //{
    //    public Nullable<int> Id { get; set; }
    //    public Nullable<int> ApplicationFormId { get; set; }
    //    public Nullable<int> EmployementGeneration { get; set; }
    //    public Nullable<decimal> FundingFromOwnSource { get; set; }
    //    public Nullable<decimal> FundingFromOtherSource { get; set; }
    //    public string ProposedProject { get; set; }
    //    public string AppliedArea { get; set; }
    //    public string PowerRequired { get; set; }
    //    public string TotalCost { get; set; }
    //    public string ImplementationTime { get; set; }
    //}

    //public class OnlineBankViewModel
    //{
    //    public Nullable<int> Id { get; set; }
    //    public Nullable<int> BankId { get; set; }
    //    public Nullable<int> BranchId { get; set; }
    //    public Nullable<int> PaymentTypeId { get; set; }
    //    public string BankName { get; set; }
    //    public string BranchName { get; set; }
    //    public string AccountNumber { get; set; }
    //    public string AccountHolder { get; set; }
    //    public string DDNumber { get; set; }
    //    public string UTRNumber { get; set; }
    //    public string IssueBank { get; set; }
    //    public string PaymentType { get; set; }
    //    public Nullable<DateTime> IssueDate { get; set; }
    //    public Nullable<Decimal> Amount { get; set; }
    //}

    //public class ResultMessage
    //{
    //    public ResultMessage()
    //    {
    //        this.clsResultType = new List<ResultMessage>();
    //    }
    //    public int ReturnType { get; set; }
    //    public string Message { get; set; }
    //    public int PrimaryKey { get; set; }
    //    public List<ResultMessage> clsResultType { get; set; }
    //}

    //public class DropdownViewModel
    //{
    //    public int Id { get; set; }
    //    public string Text { get; set; }
    //    public string Value { get; set; }
    //    public string Status { get; set; }
    //    public string Message { get; set; }
    //    public string RoleName { get; set; }
    //    public string ActionType { get; set; }
    //    public string ReturnType { get; set; }
    //    public string FilterType { get; set; }
    //    public string ChallanRefId { get; set; }
    //    public string TransactionId { get; set; }
    //    public Nullable<bool> IsActive { get; set; }
    //    public Nullable<int> SchemeId { get; set; }
    //    public Nullable<int> SectorId { get; set; }
    //    public Nullable<int> BlockId { get; set; }
    //    public Nullable<int> RoleId { get; set; }
    //    public Nullable<int> ServiceId { get; set; }
    //    public Nullable<int> FilterTypeId { get; set; }
    //    public Nullable<int> ActionTypeId { get; set; }
    //    public Nullable<int> ReturnTypeId { get; set; }
    //    public Nullable<int> DepartmentId { get; set; }
    //    public Nullable<int> PropertTypeId { get; set; }
    //    public Nullable<int> RegistrationId { get; set; }
    //    public Nullable<long> ReceiptId { get; set; }
    //    public Nullable<long> RefId { get; set; }
    //    public Nullable<DateTime> FilterDate { get; set; }
    //}
}
