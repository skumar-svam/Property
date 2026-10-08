using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace NA.PMS.Model.NIC
{
    public class ServiceRequestVM
    {
        public int? Id { get; set; }
        public int? ServiceId { get; set; }
        public int? OnlineRequestId { get; set; }
        public int? RegistrationId { get; set; }
        public string RegistrationType { get; set; }

        public string TxtControlID { get; set; }
        public string TxtUnitID { get; set; }
        public string TxtServiceID { get; set; }
        public string TxtProcessIndustryID { get; set; }
        public string TxtApplicationID { get; set; }

        public int Status { get; set; }
        public Nullable<Boolean> IsServiceExist { get; set; }

        public ServiceVM ServiceModel { get; set; }
        public PropertyVM PropertyModel { get; set; }
        public ApplicantVM ApplicantModel { get; set; }
        public TransferVM TransferModel { get; set; }
        public RentingVM RentModel { get; set; }
        public TransferVM MutationModel { get; set; }
        public GPAVM GPAModel { get; set; }
        public ExtensionVM ExtensionModel { get; set; }
        public MortgageVM MortgageModel { get; set; }
        public CICVM CICModel { get; set; }

        public WBasicDetailsModel_NMS NICModel { get; set; } 
    }

    public class ServiceVM
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RId { get; set; }
        public Nullable<int> RequestId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> SubDepartmentId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> ServiceStatusId { get; set; }
        public Nullable<int> RequestorId { get; set; }
        public Nullable<int> ApproverId { get; set; }
        public Nullable<int> ValidatorId { get; set; }
        public Nullable<int> Timeline { get; set; }
        public Nullable<int> ActionTypeId { get; set; }
        public Nullable<int> RoleTypeId { get; set; }

        public Nullable<int> Completed { get; set; }
        public Nullable<int> Initiated { get; set; }
        public Nullable<int> Pending { get; set; }
        public Nullable<int> Cancelled { get; set; }
        public Nullable<int> InProgress { get; set; }

        public Nullable<int> TotalCompleted { get; set; }
        public Nullable<int> TotalInitiated { get; set; }
        public Nullable<int> TotalPending { get; set; }
        public Nullable<int> TotalCancelled { get; set; }
        public Nullable<int> TotalInProgress { get; set; }
        public Nullable<int> TotalRequest { get; set; }

        public string Department { get; set; }
        public string SchemeName { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo { get; set; }
        public string RegistrationNo { get; set; }
        public string RegistrationType { get; set; }
        public string SubDepartment { get; set; }
        public string ServiceName { get; set; }
        public string ServiceType { get; set; }
        public string Status { get; set; }
        public string ServiceStatus { get; set; }
        public string RequestStatus { get; set; }
        public string Description { get; set; }
        public string PendencyLevel { get; set; }
        public string Comment { get; set; }
        public string RequestComment { get; set; }
        public string RoleType { get; set; }
        public string Applicant { get; set; }
        public string ApplicantMaster { get; set; }
        public string ApplicantAddress { get; set; }
        public string RequestorAddress { get; set; }
        public string Requestor { get; set; }
        public string Approver { get; set; }
        public string Validator { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string ActionType { get; set; }
        public string UploadedDocument { get; set; }
        public string DispatchedDocument { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsCancelled { get; set; }

        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> DuesAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }

        public Nullable<decimal> PercentInProgress { get; set; }

        public Nullable<DateTime> RequestDate { get; set; }
        public Nullable<DateTime> CompletionDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<DateTime> ValidationDate { get; set; }
        public Nullable<DateTime> ApprovalDate { get; set; }

        public Nullable<bool> IsUploaded { get; set; }

        public HttpPostedFileBase Document { get; set; }
        public int? PaymentStatus { get; set; }
        public string StatusMessage { get; set; }
        public string EncryptedId { get { return RequestId != null ? CommonHelper.Encode(RequestId.ToString()) : null; } }
    }

    public class PropertyVM
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> ParentPropertyId { get; set; }
        public Nullable<int> PropertyTypeId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> SchemeStatusId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> FloorAreaId { get; set; }
        public Nullable<int> RegistryId { get; set; }
        public Nullable<int> VillageId { get; set; }
        public Nullable<int> TotalInstallment { get; set; }
        public Nullable<int> Frequency { get; set; }
        public Nullable<int> LocationId { get; set; }
        public Nullable<int> FloorId { get; set; }
        public Nullable<int> ExcessAreaRefId { get; set; }
        public Nullable<int> GroupProjectId { get; set; }

        public string SchemeName { get; set; }
        public string SchemeStatus { get; set; }
        public string Department { get; set; }
        public string PropertyNo { get; set; }
        public string PropertyType { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public string PlotNo { get; set; }
        public string PlotUnitNo { get; set; }
        public string Status { get; set; }
        public string FloorArea { get; set; }
        public string FormNo { get; set; }
        public string Applicant { get; set; }
        public string ApplicantType { get; set; }
        public string ApplicantMaster { get; set; }
        public string ApplicantAddress { get; set; }
        public string ApplicantAddressII { get; set; }
        public string FirstApplicant { get; set; }
        public string FirstApplicantAdd { get; set; }
        public string CorrespondAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string LocationType { get; set; }
        public string Registry { get; set; }
        public string RegistryType { get; set; }
        public string Village { get; set; }
        public string KhasraNumber { get; set; }
        public string KhatoniNumber { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string ActionType { get; set; }
        public string AreaRange { get; set; }
        public string AllotmentYear { get; set; }
        public string AreaPhase { get; set; }
        public string AllotmentMethod { get; set; }
        public string MortgageStatus { get; set; }
        public string FunctionalStatus { get; set; }
        public string OneTimeLeaseStatus { get; set; }
        public string GPAHolderName { get; set; }
        public string GPAHolderAddress { get; set; }

        public Nullable<Decimal> ActualArea { get; set; }
        public Nullable<Decimal> CoveredArea { get; set; }
        public Nullable<Decimal> TotalArea { get; set; }
        public Nullable<Decimal> ExcessArea { get; set; }
        public Nullable<Decimal> AllottedArea { get; set; }
        public Nullable<Decimal> CivilCost { get; set; }
        public Nullable<Decimal> PropertyCost { get; set; }
        public Nullable<Decimal> TotalPropertyCost { get; set; }
        public Nullable<Decimal> ExcessAreaCost { get; set; }
        public Nullable<Decimal> TotalExcessAreaCost { get; set; }
        public Nullable<Decimal> PayableAmount { get; set; }
        public Nullable<Decimal> TotalPayableAmount { get; set; }
        public Nullable<Decimal> AllotmentMoney { get; set; }
        public Nullable<Decimal> EarnestMoney { get; set; }
        public Nullable<Decimal> LeaseRent { get; set; }
        public Nullable<Decimal> OneTimeLeaseRent { get; set; }
        public Nullable<Decimal> AdvanceLeaseRent { get; set; }
        public Nullable<Decimal> LandRate { get; set; }
        public Nullable<Decimal> AllotmentRate { get; set; }
        public Nullable<Decimal> TotalAllotmentRate { get; set; }
        public Nullable<Decimal> ExcessAreaAllotmentRate { get; set; }
        public Nullable<Decimal> LocationChargeRate { get; set; }
        public Nullable<Decimal> FinalAllotmentRate { get; set; }
        public Nullable<Decimal> LocationCharge { get; set; }
        public Nullable<Decimal> ExcessAreaCharge { get; set; }
        public Nullable<Decimal> ExcessAreaLocationCharge { get; set; }
        public Nullable<Decimal> ProcessingFee { get; set; }
        public Nullable<Decimal> NormalInterest { get; set; }
        public Nullable<Decimal> PenalInterest { get; set; }
        public Nullable<Decimal> PenalInterestAmount { get; set; }

        public Nullable<Double> FloorAreaRatio { get; set; }

        public Nullable<DateTime> AllotmentDate { get; set; }
        public Nullable<DateTime> PossessionDate { get; set; }
        public Nullable<DateTime> RegistryDate { get; set; }
        public Nullable<DateTime> CompletionDate { get; set; }
        public Nullable<DateTime> FunctionalDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<DateTime> SchemeStartDate { get; set; }
        public Nullable<DateTime> SchemeEndDate { get; set; }
        public Nullable<DateTime> PropertyRateStartDate { get; set; }
        public Nullable<DateTime> PropertyRateEndDate { get; set; }
        public Nullable<DateTime> ActionDate { get; set; }
        public Nullable<DateTime> GPAEffectiveFromDate { get; set; }
        public Nullable<DateTime> GPAEffectiveTillDate { get; set; }
        public Nullable<DateTime> ExcessAreaDate { get; set; }
        public Nullable<DateTime> ExcessAreaValidDate { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDocumentAvailable { get; set; }
        public Nullable<bool> IsNDCIssued { get; set; }
        public Nullable<bool> IsLocationCharged { get; set; }
        public Nullable<bool> IsSelected { get; set; }
        public Nullable<bool> IsPropertyActive { get; set; }
        public Nullable<bool> IsPropertyAllotted { get; set; }
        public Nullable<bool> IsPropertyExists { get; set; }
        public Nullable<bool> IsPropertyFunctional { get; set; }
        public Nullable<bool> IsLeaseDeedExecuted { get; set; }
        public Nullable<bool> IsPropertyHasGPA { get; set; }
        public Nullable<bool> IsGPAEffective { get; set; }
        public Nullable<bool> IsCancelledOrSurrendered { get; set; }
        public Nullable<bool> IsExcessArea { get; set; }

    }

    public class ApplicantVM
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> RequestNo { get; set; }
        public Nullable<int> ReferenceNo { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> ParentPropertyId { get; set; }
        public Nullable<int> PropertyTypeId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> PreviousDues { get; set; }
        public Nullable<int> TransferTypeId { get; set; }
        public Nullable<int> OTP { get; set; }

        public string SchemeName { get; set; }
        public string Department { get; set; }
        public string Applicant { get; set; }
        public string ApplicantType { get; set; }
        public string ApplicantMaster { get; set; }
        public string Guardian { get; set; }
        public string MotherName { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo { get; set; }
        public string Mobile { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string MaritalStatus { get; set; }
        public string RegistryType { get; set; }
        public string TransfereeName { get; set; }
        public string TransfereeType { get; set; }
        public string TransfereeRelation { get; set; }
        public string TransferType { get; set; }
        public string PermanentAddress { get; set; }
        public string CorrespondAddress { get; set; }
        public string PropertyType { get; set; }
        public string FloorArea { get; set; }
        public string ActionType { get; set; }
        public string ResourceMessage { get; set; }
        public string MessageKey { get; set; }
        public string MessageValue { get; set; }
        public string MessageContent { get; set; }
        public string Occupation { get; set; }
        public string PAN { get; set; }

        public Nullable<decimal> Area { get; set; }
        public Nullable<decimal> PropertyCost { get; set; }
        public Nullable<decimal> AllotmentMoney { get; set; }
        public Nullable<decimal> StampDutyAmount { get; set; }
        public Nullable<decimal> DuplicateStampValue { get; set; }
        public Nullable<decimal> BalanceDues { get; set; }
        public Nullable<decimal> AnnualIncome { get; set; }

        public Nullable<DateTime> LeasedeedDueDate { get; set; }
        public Nullable<DateTime> TransferDate { get; set; }
        public Nullable<DateTime> AllotmentDate { get; set; }
        public Nullable<DateTime> ChecklistDuedate { get; set; }
        public Nullable<DateTime> ChecklistDate { get; set; }
        public Nullable<DateTime> DateOfBirth { get; set; }

        public Nullable<bool> IsLeasedeed { get; set; }
        public Nullable<bool> IsSubLeasedeed { get; set; }

        public bool IsKYADone { get; set; }
    }

    public class TransferVM
    {
        public int? Id { get; set; }
        public int? SectorId { get; set; }
        public int? BlockId { get; set; }
        public int? RequestNo { get; set; }
        public int? ReferenceId { get; set; }
        public int? DepartmentId { get; set; }
        public int? RegistrationId { get; set; }
        public int? OnlineRequestNo { get; set; }
        public int? PropertyId { get; set; }
        public int? OccupationId { get; set; }
        public int? RequestedBy { get; set; }
        public int? ApprovedBy { get; set; }
        public int? TransfereeTypeId { get; set; }
        public int? TransferTypeId { get; set; }
        //public int?t> TransfereeTypeId { get; set; }
        public int? TransferSubTypeId { get; set; }
        public int? ApproverId { get; set; }
        public int? StatusId { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public string PropertyNo { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string Department { get; set; }
        public string Applicant { get; set; }
        public string ApplicantMaster { get; set; }
        public string ApplicantType { get; set; }
        public string ApplicantAddress { get; set; }
        public string ApplicantAddressII { get; set; }
        public string CompanyName { get; set; }
        public string SigningAuthority { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string FatherOrHusbandName { get; set; }
        public string MotherName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string MobileNo { get; set; }
        public string CorrespondenceAdd { get; set; }
        public string PermanentAdd { get; set; }
        public string RegisteredOffice { get; set; }
        public string Occupation { get; set; }
        public string PAN { get; set; }
        public string Transferee { get; set; }
        public string TransfereeType { get; set; }
        public string TransfereeMaster { get; set; }
        public string TransfereeMother { get; set; }
        public string TransfereeAddress { get; set; }
        public string TransfereeAddressII { get; set; }
        public string TransferType { get; set; }
        public string TransferSubType { get; set; }
        public string TypeOfTransferee { get; set; }
        public string BahiNo { get; set; }
        public string BahiZildNo { get; set; }
        public string BahiPageNo { get; set; }
        public string BahiSeriesNo { get; set; }
        public string BookNo { get; set; }
        public string BookZildNo { get; set; }
        public string BookSeriesNo { get; set; }
        public string GPAHolderName { get; set; }
        public string GPAHolderAddress { get; set; }
        public string Comment { get; set; }
        public string ProjectName { get; set; }
        public string ActionType { get; set; }
        public string Relation { get; set; }
        public string Nominee { get; set; }
        public string Status { get; set; }
        public string TransferStatus { get; set; }
        public string MortgageStatus { get; set; }
        public string FunctionalStatus { get; set; }
        public string OneTimeLeaseStatus { get; set; }
        public string TransferThrough { get; set; }
        public string TransferorType { get; set; }
        public string Approver { get; set; }
        public string AadhaarNo { get; set; }

        public Nullable<decimal> AnnualIncome { get; set; }
        public Nullable<decimal> SellingCost { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> TransferRate { get; set; }
        public Nullable<decimal> TransferCharge { get; set; }
        public Nullable<decimal> TotalTransferCharge { get; set; }
        public Nullable<decimal> BalanceDues { get; set; }
        public Nullable<decimal> TotalArea { get; set; }

        public Nullable<DateTime> TransferDate { get; set; }
        public Nullable<DateTime> MutationDate { get; set; }
        public Nullable<DateTime> TransferdeedDate { get; set; }
        public Nullable<DateTime> RequestedDate { get; set; }
        public Nullable<DateTime> ApprovedDate { get; set; }
        public Nullable<DateTime> CommentDate { get; set; }
        public Nullable<DateTime> GPAEffectiveFrom { get; set; }
        public Nullable<DateTime> GPAEffectiveTo { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<DateTime> LeaseDeedDate { get; set; }
        public Nullable<DateTime> PossessionDate { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsLeasedeedExecuted { get; set; }
        public Nullable<bool> IsGPAExecuted { get; set; }
        public Nullable<bool> IsGPAEffective { get; set; }
        public Nullable<bool> IsCancelledOrSurrendered { get; set; }
    }

    public class GPAVM
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> SNo { get; set; }
        public Nullable<int> NomineeId { get; set; }
        public Nullable<int> GPAId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> OnlineRequestId { get; set; }

        public string GPAType { get; set; }
        public string GPAHolderName { get; set; }
        public string RelationName { get; set; }
        public string GPAHolderAdd { get; set; }
        public string GPARegisteredNo { get; set; }
        public string NomineeName { get; set; }
        public string NomineeRelation { get; set; }

        public Nullable<bool> IsGPAActive { get; set; }
        public Nullable<bool> IsGPARegistered { get; set; }

        public Nullable<DateTime> EffectiveFrom { get; set; }
        public Nullable<DateTime> EffectiveTo { get; set; }
        public Nullable<DateTime> ApplicationDate { get; set; }
        public Nullable<DateTime> AcceptanceDate { get; set; }
        public Nullable<DateTime> NominationDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> AllotmentDate { get; set; }
        public Nullable<DateTime> FromSearch { get; set; }
        public Nullable<DateTime> ToSearch { get; set; }

        public PropertyVM PropertyModel { get; set; }
        public ServiceRequestVM ServiceModel { get; set; }
    }


    public class RentingVM
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RentId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> OnlineRequestId { get; set; }
        public Nullable<int> RentStatusId { get; set; }
        public Nullable<int> Approver { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> Modifiedby { get; set; }

        [RegularExpression(@"^[0-9 ]*$", ErrorMessage = "Rent Duration must be numeric.")]
        [Range(0, 99, ErrorMessage = "Rent Duration must be less than 100")]
        public Nullable<int> RentDuration { get; set; }

        [MaxLength(60, ErrorMessage = "Tenant Name cannot be more than 60 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9. ]*$", ErrorMessage = "Tenant name must be alphanumeric.")]
        public string TenantName { get; set; }

        [MaxLength(100, ErrorMessage = "Tenant project cannot be more than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9. ]*$", ErrorMessage = "Tenant project must be alphanumeric.")]
        public string TenantProject { get; set; }

        public string RentStatus { get; set; }
        public string Comment { get; set; }
        public string ViewName { get; set; }
        public string Applicant { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime RequestDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> RentingDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> RentingEndDate { get; set; }

        public Nullable<DateTime> ApproveDate { get; set; }
        public Nullable<DateTime> CommentDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }

        [RegularExpression(@"^[0-9. ]*$", ErrorMessage = "Renting charge must be decimal.")]
        [Range(0, 999999999999.99, ErrorMessage = "Renting Charge must be in decimal max [12,2]")]
        public Nullable<decimal> RentingCharge { get; set; }

        public Nullable<decimal> Amount { get; set; }

        public Nullable<bool> Permission { get; set; }
        public Nullable<bool> IsRentActive { get; set; }
        public Nullable<bool> IsRentingChargePaid { get; set; }
        public Nullable<bool> IsRentPermissionRequested { get; set; }
        public Nullable<bool> IsPropertyFunctional { get; set; }

        public PropertyVM PropertyModel { get; set; }
        public ServiceRequestVM ServiceModel { get; set; }
    }


    public class ExtensionVM
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> OnlineRequestNo { get; set; }
        public Nullable<DateTime> CompletionDueDate { get; set; }
        public Nullable<DateTime> ExtensionGivenDate { get; set; }
        public Nullable<DateTime> ExtensionDueDate { get; set; }
        public Nullable<decimal> ExtensionCharge { get; set; }
        public string Comment { get; set; }
        public Nullable<int> ExtensionStatusId { get; set; }
        public string ExtensionStatus { get; set; }
        public string BuildingPlan { get; set; }

        public Nullable<int> ScheduleActionDay { get; set; }

        public string AssignTo { get; set; }
        public string Approver { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<DateTime> ApprovedDate { get; set; }

        public bool IsSelected { get; set; }
        public Nullable<bool> IsExtendable { get; set; }
        public Nullable<bool> IsExtensionActive { get; set; }

        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }

        public PropertyVM PropertyModel { get; set; }
        public ServiceRequestVM ServiceModel { get; set; }
    }

    public class MortgageVM
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> OnlineRequestNo { get; set; }
        public Nullable<int> MortgageTypeId { get; set; }
        public Nullable<int> MortgageStatusId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> Modifiedby { get; set; }
        public Nullable<int> Approver { get; set; }
        public Nullable<int> ApproverId { get; set; }

        public string Applicant { get; set; }
        public string ApplicantAddress { get; set; }
        public string BankName { get; set; }
        public string PreviousBankDetail { get; set; }
        public string BranchAddress { get; set; }
        public string Functional { get; set; }
        public string MortgageType { get; set; }
        public string MortgageStatus { get; set; }
        public string ApproverName { get; set; }
        public string Comment { get; set; }

        public Nullable<decimal> ProcessingFee { get; set; }
        public Nullable<decimal> SanctionedAmount { get; set; }
        public Nullable<decimal> TotalDues { get; set; }

        public Nullable<short> PreviousLoanNoc { get; set; }

        public Nullable<bool> IsMortgaged { get; set; }
        public Nullable<bool> IsMortgageActive { get; set; }

        public Nullable<DateTime> MortgageDate { get; set; }
        public Nullable<DateTime> ApproveDate { get; set; }
        public Nullable<DateTime> CommentDate { get; set; }
        public Nullable<DateTime> ValidUpto { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<DateTime> MortgageStartDate { get; set; }
        public Nullable<DateTime> MortgageEndDate { get; set; }

        public PropertyVM PropertyModel { get; set; }
        public ServiceRequestVM ServiceModel { get; set; }



    }

    public class CICVM
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> OnlineRequestNo { get; set; }
        public Nullable<int> DirectorId { get; set; }
        public Nullable<int> TypeId { get; set; }
        public Nullable<int> ChangeTypeId { get; set; }
        public Nullable<int> OldFirmStatusId { get; set; }
        public Nullable<int> NewFirmStatusId { get; set; }
        public Nullable<int> SerialNo { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<int> Approver { get; set; }
        public Nullable<int> CICStatusId { get; set; }
        public Nullable<int> FormId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<int> IsCICActive { get; set; }

        public string DirectorName { get; set; }
        public string TypeName { get; set; }
        public string ChangeTypeName { get; set; }
        public string OldFirmName { get; set; }
        public string NewFirmName { get; set; }
        public string OldFirmStatus { get; set; }
        public string NewFirmStatus { get; set; }
        public string OldFirmProduct { get; set; }
        public string NewFirmProduct { get; set; }
        public string Comment { get; set; }
        public string AssignTo { get; set; }
        public string CICStatus { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string FirmExists { get; set; }
        public string ActionType { get; set; }
        public string Department { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string ApproverName { get; set; }

        public Nullable<decimal> DirectorShare { get; set; }
        public Nullable<decimal> CICCharge { get; set; }

        public Nullable<DateTime> RequestDate { get; set; }
        public Nullable<DateTime> ApprovedDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }

        public Nullable<bool> IsFirmExists { get; set; }

        //public PropertyVM PropertyModel { get; set; }
        //public ServiceRequestVM ServiceModel { get; set; }
        //public List<DirectorShareholderVM> DirectorsModel { get; set; }
    }

    public class DirectorShareholderVM
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> ShareType { get; set; }
        public string ShareholderName { get; set; }
        public Nullable<decimal> ShareValue { get; set; }
        public string ShareTypeName
        {
            get
            {
                if (ShareType == 10)
                {
                    return "Driector";
                }
                if (ShareType == 11)
                {
                    return "Shareholder";
                }
                if (ShareType == 12)
                {
                    return "Driector/Shareholder";
                }
                else
                {
                    return "NA";
                }
            }
        }
    }
    public class ServiceCheckListVM
    {
        public int Id { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> ChecklistRefNo { get; set; }
        public string Department { get; set; }
        public string ServiceName { get; set; }
        public string ChecklistName { get; set; }
        public Nullable<bool> Status { get; set; }
    }

    public class DocumentVM
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> SerialNo { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> TemplateId { get; set; }

        public string Department { get; set; }
        public string ServiceName { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public string DocumentPath { get; set; }
        public string ScannedDocument { get; set; }
        public string GeneratedDocument { get; set; }
        public string UploadedDocument { get; set; }
        public string DocumentContent { get; set; }
        public string Template { get; set; }
        public string Status { get; set; }
        public string ActionType { get; set; }
        public string Barcode { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsScanned { get; set; }
        public Nullable<bool> IsUploaded { get; set; }
        public Nullable<bool> IsGenerated { get; set; }
        public Nullable<bool> IsDocumentExist { get; set; }
    }
}
