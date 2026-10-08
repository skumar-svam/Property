using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Model
{
    public class PossessionModel
    {
        public int Id { get; set; }
        public int RequestNo { get; set; }
        public int RId { get; set; }
        public string PropNo { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime PossessionDate { get; set; }
        public string AreaChange { get; set; }
        public string Status { get; set; }
    }

    public class CheckList
    {
        public int RId { get; set; }
        public string AllotteeName { get; set; }
        public string PropertyNumber { get; set; }
        public decimal? PropertyCost { get; set; }
        public decimal? AllotmentMoney { get; set; }
        public DateTime? AllotmentDate { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int PropertyTypeId { get; set; }
        public string PropertyTypeName { get; set; }
        public int CheckListTypeId { get; set; }
        public string CheckListTypeName { get; set; }
        public DateTime? CheckListDate { get; set; }
        public string FatherName { get; set; }
        public DateTime ChecklistDuedate { get; set; }
        public string ViewName { get; set; }
        public bool IsActive { get; set; }
    }

    public class LeaseDeedProperty
    {
        public int? ReqNo { get; set; }
        public int RId { get; set; }
        public string AllotteeName { get; set; }
        public string PropertyNumber { get; set; }
        public decimal? PropertyCost { get; set; }
        public decimal? PreviousDues { get; set; }
        public decimal? BalanceDues { get; set; }
        public int StampDutyTypeId { get; set; }
        public string StampDutyTypeName { get; set; }
        public decimal? StampDutyAmount { get; set; }
        public decimal? StampDutyPercent { get; set; }
        public decimal? DuplicateStampValue { get; set; }
        public decimal? TotalDue { get; set; }
        public string RegistryType { get; set; }
        public DateTime? LeaseDeedDueDate { get; set; }
        public DateTime? LeaseDeedExecutionDate { get; set; }
        public string SchemeName { get; set; }
        public string DepttName { get; set; }
        public string LeaseDeedStatus { get; set; }
        public decimal AmountDue { get; set; }
        public string FatherName { get; set; }
        public string PropTypeName { get; set; }
        public string SignatoryAuthority { get; set; }
        public string Witness1Name { get; set; }
        public string Witness2Name { get; set; }
        public string Witness3Name { get; set; }
        public string Witness4Name { get; set; }
        public string Witness1Add { get; set; }
        public string Witness2Add { get; set; }
        public string Witness3Add { get; set; }
        public string Witness4Add { get; set; }
        public string Witness1Mob { get; set; }
        public string Witness2Mob { get; set; }
        public string Witness3Mob { get; set; }
        public string Witness4Mob { get; set; }
        public decimal? TotPropCost { get; set; }
        public string ChallanOption { get; set; }
        public double? LeaseRentPercent { get; set; }
        public decimal? LeaseRentPercentForHousing { get; set; }
        public string ViewName { get; set; }
        public bool IsActive { get; set; }
        public int? DepttId { get; set; }
        public int? SchemeId { get; set; }
        public int? PropId { get; set; }
        public DateTime AllotmentDate { get; set; }
        public DateTime ChckLstDate { get; set; }
        public decimal? LeaseRent { get; set; }
        public int Type { get; set; }
        public string strType { get; set; }
        public decimal? DocCharges { get; set; }
        public DateTime? LeasePaidUptoDate { get; set; }
        public DateTime? RevisedDate { get; set; }
        public int RevisedAfterYear { get; set; }
        public decimal? RevisedRate { get; set; }
    }


    public class RentModel
    {
        public int? RentRequestId { get; set; }
        public int RequestNo { get; set; }
        public int RID { get; set; }
        public string SchemeName { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
        public string ApplicantName { get; set; }
        public string TenantName { get; set; }
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }

        //private string EncryptedString;
        public string EncryptedRequestNo
        {
            get { return CommonHelper.Encode(RequestNo.ToString()); }
            //set { EncryptedString = CommonHelper.Encode(RequestNo.ToString()); }
        }
    }

     public class RentdetailShowModel
    {
         [DisplayName("Registration Id")]
         public Nullable<int> Rid { get; set; }
        public Nullable<int> RequestNo { get; set; }
        public string TenantName { get; set; }
        public string TenantProject { get; set; }
          public Nullable<System.DateTime> RentingDate { get; set; }
        public Nullable<int> RentDuration { get; set; }
        public Nullable<decimal> Area { get; set; }
        public string RequestStatus { get; set; }
      
        }
    public class RentPermissionModel
    {
        public Nullable<int> RequestNo { get; set; }
        public Nullable<int> ServiceRequestRefNo { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> Approver { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> SchemeTypeId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> Modifiedby { get; set; }
        public int DepartmentId { get; set; }

        [DisplayName("Registration Id")]
        public Nullable<int> Rid { get; set; }

        [Required]
        [DisplayName("Rent Duration")]
        [RegularExpression(@"^[0-9 ]*$", ErrorMessage = "Rent Duration must be numeric.")]
        [Range(0, 99, ErrorMessage = "Rent Duration must be less than 100")]
        public Nullable<int> RentDuration { get; set; }

        [Required(ErrorMessage = "Approver is required")]
        public int UserId { get; set; }

        [Required]
        [DisplayName("Tenant Name")]
        [MaxLength(60, ErrorMessage = "Tenant Name cannot be more than 60 characters.")]
        //[RegularExpression(@"^[a-zA-Z0-9. ]*$", ErrorMessage = "Tenant name must be alphanumeric.")]
        public string TenantName { get; set; }

        [DisplayName("Department")]
        public string Department { get; set; }

        [Required]
        [DisplayName("Tenant Project")]
        [MaxLength(100, ErrorMessage = "Tenant project cannot be more than 100 characters.")]
        //[RegularExpression(@"^[a-zA-Z0-9. ]*$", ErrorMessage = "Tenant project must be alphanumeric.")]
        public string TenantProject { get; set; }
        public string Comment { get; set; }
        public string PropertyType { get; set; }

        [DisplayName("Allottee Name")]
        public string AllotteeName { get; set; }
        [DisplayName("Sector")]
        public string Sector { get; set; }
        [DisplayName("Block")]
        public string Block { get; set; }
        [DisplayName("Property Number")]
        public string PropertyNo { get; set; }

        public string FatherOrHusbandName { get; set; }
        public string SigningAuthority { get; set; }
        public string GenderOrCompany { get; set; }
        public string TotalArea { get; set; }
        public string FloorNo { get; set; }
        public string RequestStatus { get; set; }
        public string AssignedUser { get; set; }
        public string SchemeName { get; set; }
        public string ViewName { get; set; }
        public string Remarks { get; set; }

        [DisplayName("Request Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime RequestDate { get; set; }

        [Required]
        [DisplayName("Renting Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> RentingDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> RentingEndDate { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public Nullable<System.DateTime> CommentDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        [Required]
        [DisplayName("Renting Charge")]
        [RegularExpression(@"^[0-9. ]*$", ErrorMessage = "Renting charge must be decimal.")]
        [Range(0, 999999999999.99, ErrorMessage = "Renting Charge must be in decimal max [12,2]")]
        public Nullable<decimal> RentingCharge { get; set; }

        [DisplayName("Property Type")]
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Area { get; set; }      
                     
        [DisplayName("Total Dues")]
        public decimal TotalDues { get; set; }

        [DisplayName("Functional")]
        public bool? Functional { get; set; }
        public bool IsSelected { get; set; }
        public Nullable<bool> Permission { get; set; }
        public bool IsRentingChargePaid { get; set; }
        public bool IsRentPermissionRequested { get; set; }
        public Nullable<bool> IsActive { get; set; }

        public string YesOrNo
        {
            get
            {
                if (Functional == true) { return "Yes"; }
                else { return "No"; }
            }
        }

        public ServiceRequestModel ServiceRequestModel { get; set; }
    }

    public class PossessionOrderModel
    {
        public int RId { get; set; }
        public string PropNo { get; set; }
        public string SchemeName { get; set; }
        public string Deptt { get; set; }
        public string ApplicantName { get; set; }
        public string Gender { get; set; }
        public int RelativeName { get; set; }
        public string PropType { get; set; }
        public decimal Area { get; set; }
        public string Floor { get; set; }
        public DateTime PossessionDate { get; set; }
        public string ApproverName { get; set; }
        public int ApproverId { get; set; }
        public string User { get; set; }
        public string ReqStatus { get; set; }
    }

    public class ChecklistDocuments
    {
        public int SNo { get; set; }
        public string DocName { get; set; }
        public int Id { get; set; }
        public bool IsSelected { get; set; }
    }

    //Common class to be used for fetching deatils by RID. Please add other properties in this class if any other additional details are required.
    public class DetailsByRId
    {
        public string AllotteName { get; set; }
        public string FatherName { get; set; }
        public string DepttName { get; set; }
        public string PropType { get; set; }
        public string PropNo { get; set; }
        public decimal? PropCost { get; set; }
        public decimal? AllotmentMoney { get; set; }
        public DateTime? AllotmentDate { get; set; }
        public int PropTypeVal { get; set; }
        public decimal? StampDutyAmount { get; set; }
        public decimal? DuplicateStampValue { get; set; }
        public int PreviousDues { get; set; }
        public decimal? BalanceDues { get; set; }
        public string RegistryType { get; set; }
        public DateTime? LeaseDeedDueDate { get; set; }
        public int DepttID { get; set; }
        public int SchemeId { get; set; }
        public int PropId { get; set; }
        public string SchemeName { get; set; }
        public string Gender { get; set; }
        public decimal? Area { get; set; }
        public string TransfereeName { get; set; }
        public string TransfereeGender { get; set; }
        public string TransfereeRelationName { get; set; }
        public DateTime? TransferDate { get; set; }
        public string TransferType { get; set; }
        public int TransferTypeId { get; set; }
        public string Floor { get; set; }
        public int ReqNo { get; set; }
        public DateTime ChecklistDuedate { get; set; }
        public DateTime? ChecklistDate { get; set; }
        public bool IsParentLeaseDeed { get; set; }
        public DateTime? GPAEffectiveFrom { get; set; }
        public DateTime? GPAEffectiveto { get; set; }
        public string GPAHolderName { get; set; }
        public string GPAHolderAddress { get; set; }
    }

    public class MutationModel
    {
        //Applicant Details
        public int ReqNo { get; set; }
        public int? RId { get; set; }
        public int DepttId { get; set; }
        public string DepttName { get; set; }
        public string PropNo { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public string PropertyNo { get; set; }
        public DateTime? ReqDate { get; set; }
        public DateTime? MutationDate { get; set; }
        public string Status { get; set; }
        public string SchemeName { get; set; }
        public string ApplicantName { get; set; }
        public string Gender { get; set; }
        public string RelationName { get; set; }
        public string PropType { get; set; }
        public int PropTypeId { get; set; }
        public decimal? Area { get; set; }
        public string Floor { get; set; }
        public int FloorId { get; set; }
        public string ApplicantAddress { get; set; }
        //Transfer Details
        public string TransfereeName { get; set; }
        public string TransfereeGender { get; set; }
        public string TransfereeRelationName { get; set; }
        public DateTime? TransferDate { get; set; }
        public string TransferType { get; set; }
        public int TransferTypeId { get; set; }
        public string TransfereeAddress { get; set; }
        //Mutation Details
        public DateTime? TransferDeedDate { get; set; }
        public string BahiNo { get; set; }
        public string BahiZildNo { get; set; }
        public string BahiPageNo { get; set; }
        public string SINo { get; set; }
        public int AssignTo { get; set; }
        //TODO: File Name 
        public string User { get; set; }
        public string From { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string Comment { get; set; }
        public string ApproverComments { get; set; }
        public string TransDeedFile { get; set; }
        public Nullable<int> OnlineRequestRefNo { get; set; }
        public DateTime? GPAEffectiveFrom { get; set; }
        public DateTime? GPAEffectiveto { get; set; }
        public string GPAHolderName { get; set; }
        public string GPAHolderAddress { get; set; }

    }
    public class FunctionalModel
    {
        public int RequestNo { get; set; }
        public int? RId { get; set; }
        public int DepttId { get; set; }

        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public string PropertyNo { get; set; }
        public string PropertyNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string Status { get; set; }

        public string SchemeName { get; set; }
        public string DepttName { get; set; }
        public string ApplicationName { get; set; }


        public string Gender { get; set; }
        public string PropertyType { get; set; }
        public decimal? Area { get; set; }
        public string Floor { get; set; }
        public DateTime? FunctionalDueDate { get; set; }
        public DateTime? FunctionalDate { get; set; }
        public Nullable<DateTime> CompletionDate { get; set; }
        public Nullable<DateTime> AffidavitDate { get; set; }
        public string RelationName { get; set; }

        public bool MeterSealing { get; set; }

        public bool Affidavit { get; set; }
        public bool RegistrationCertificate { get; set; }
        public bool NDCAccount { get; set; }
        public string User { get; set; }
        public string Comment { get; set; }
        public string From { get; set; }
        public string ApproverComments { get; set; }
        public decimal? FunctionalCharge { get; set; }
        public bool IsSelected { get; set; }
        public string MeterSealingFile { get; set; }
        public string AffidebitFile { get; set; }
        public string RegistrationCertificateFile { get; set; }
        public string NDCAccountFile { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string ApplicantName
        {
            get
            {
                return FirstName + " " + MiddleName + " " + LastName;
            }
        }
        public Nullable<bool> IsFunctional { get; set; }
        public string FunctionalStatus
        {
            get
            {
                if (IsFunctional == true) { return "Functional"; }
                else return "Not Functional";
            }
        }

        [DisplayName("Request Ref. No.")]
        public int ReqRefNo { get; set; }
    }

    public class BuildingPlanModel
    {
        public int RequestNo { get; set; }
        public int? RId { get; set; }
        public int DepttId { get; set; }
        public string PropNo { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public string PropertyNo { get; set; }

        public string PropertyNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string Status { get; set; }

        public string SchemeName { get; set; }
        public string DepttName { get; set; }
        public string ApplicationName { get; set; }

        public string AllotteeName { get; set; }
        public string PropertyType { get; set; }
        public string Location_East { get; set; }
        public string Location_West { get; set; }
        public string Location_South { get; set; }
        public string Location_North { get; set; }
        //[RegularExpression(@"^([a-zA-Z0-9 \-\\]+)$", ErrorMessage = "Property No should be alphanumeric (with '-', '\' and '/' characters)")] 
        [RegularExpression(@"^[a-zA-Z0-9. ]*$", ErrorMessage = "Tenant name must be alphanumeric.")]
        public string BuildingPlanFileNo { get; set; }
        public int? PropertyUse { get; set; }

        public DateTime? DateOfSubmission { get; set; }
        public DateTime? DateOfSanction { get; set; }
        public decimal? PurchasableFar { get; set; }
        public DateTime? SanctionPlanValidity { get; set; }

        public string MapReleased { get; set; }

        public string MapRevised { get; set; }
        public decimal? PloatArea { get; set; }
        public decimal? CoveredArea { get; set; }
        public double? FloorAreasRation { get; set; }

        public int NumberOfStories { get; set; }

        public string ArchitectRegNo { get; set; }

        public string NameOfArchitect { get; set; }

        public string NameOfTower { get; set; }

        public decimal? CoveredAreaMultiFloors { get; set; }

        public decimal? BuildingHeight { get; set; }

        public decimal? CoveredAreaGroundFloor { get; set; }
        public int? NumberOfStories2 { get; set; }

        public int? Charges { get; set; }

        public decimal? FeeAmount { get; set; }

        public int? NocType { get; set; }

        public string Submitted { get; set; }

        public string NOC { get; set; }
        public string SubmitStatus { get; set; }
        public string NameOfBuilding { get; set; }
        public Decimal? TotalCoveredAreaAllFloor { get; set; }
        public Decimal? TotalCoveredAreaGroundFloor { get; set; }
        //public Decimal? BuildingHeight { get; set; }
        public int? NoOfStories { get; set; }
        public string ChargesName { get; set; }

    }

    public class MortgageModel
    {
        public int RequestNo { get; set; }
        public Nullable<int> RID { get; set; }
        public Nullable<bool> Mortgage { get; set; }
        public Nullable<System.DateTime> MortgageDate { get; set; }
        [DisplayName("Bank Name")]
        public string BankName { get; set; }
        [DisplayName("Branch Address")]
        public string BranchAddress { get; set; }
        [DisplayName("Processing Fee")]
        public Nullable<decimal> ProcessingFee { get; set; }
        [DisplayName("Sanctioned Amount")]
        public Nullable<decimal> SanctionedAmount { get; set; }
        public string MortgageType { get; set; }
        [DisplayName("Valid Upto")]
        public Nullable<System.DateTime> ValidUpto { get; set; }
        [DisplayName("Previous Loan Noc")]
        public Nullable<short> PreviousLoanNoc { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> Approver { get; set; }
        public string ApproverName { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public string Comment { get; set; }
        public Nullable<System.DateTime> CommentDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> Modifiedby { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string StatusName { get; set; }
        public string SchemeName { get; set; }
        public string ApplicantName { get; set; }
        public string FatherName { get; set; }
        public string PropertyNumber { get; set; }
        public Nullable<decimal> TotalDues { get; set; }
        public string PropertyType { get; set; }
        public Nullable<decimal> PropertyCost { get; set; }
        public string Functional { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string GetComment { get; set; }
        public string PreviousBankDetail { get; set; }
        public string User { get; set; }
        public string Gender { get; set; }
        public DateTime? AllotmentDate { get; set; }
        public int? SchemeId { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string StrPreviousLoanNoc { get; set; }
        public string StrMortgageType { get; set; }
        public Nullable<int> OnlineRequestNo { get; set; }
        private string encodedParam;
        public string EncodedParameter
        {
            get { return encodedParam = CommonHelper.Encode(RequestNo.ToString()); }
            set { encodedParam = CommonHelper.Encode(RequestNo.ToString()); }
        }
    }

    public class TransferModel
    {
        public int ReqNo { get; set; }
        public int? RId { get; set; }
        public string DepttName { get; set; }
        public string PropNo { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PropertyNo { get; set; }
        public DateTime? ReqDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string AssignedTo { get; set; }
        public string Status { get; set; }
        public string Floor { get; set; }
        public string User { get; set; }
        public int? DepttId { get; set; }
        public int? PropId { get; set; }
        public string ApproverComments { get; set; }
        public string From { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string Comment { get; set; }
        public bool isLeaseDeedExecuted { get; set; }
        public decimal BalanceDues { get; set; }
        public bool isGPA { get; set; }

        public int ApprovedYear { get; set; } //For Transfer History

        //Allotee Details
        public string SchemeName { get; set; }
        public string ApplicantName { get; set; }
        public string Gender { get; set; }
        public string RelationName { get; set; }
        public string SigningAuthorityName { get; set; }
        public string PropType { get; set; }
        public decimal? Area { get; set; }
        public string FloorMst { get; set; }
        public string PropMortgaged { get; set; }
        public string PropFunctional { get; set; }
        public string OneTimeLeaseRent { get; set; }
        public string ApplicantCompany { get; set; }
        public string ApplicantSiAu { get; set; }

        //Add Transferee
        public int TransfereeGenderId { get; set; }
        public string TransfereeGenderName { get; set; }
        public string TransfereeFirstName { get; set; }
        public string TransfereeMiddleName { get; set; }
        public string TransfereeLastName { get; set; }
        public string TransfereeRelationName { get; set; }
        public string TransfereeMotherName { get; set; }
        public string TransfereeEmail { get; set; }
        public string TransfereeCorrespondenceAdd { get; set; }
        public string TransfereePermanentAdd { get; set; }
        public string TransfereeMobileNo { get; set; }
        public int? TransfereeOccupationId { get; set; }
        public string TransfereePAN { get; set; }
        public string TransfereeCompanyName { get; set; }
        public string TransfereeCompanySigningAuth { get; set; }
        public string TransfereeCompanyRegOff { get; set; }
        public string TransfereeCompanyEmail { get; set; }
        public string TransfereeCompanyPAN { get; set; }
        public int? TypeOfTransferee { get; set; }

        //Transfer & Documents
        public Nullable<int> TransferType { get; set; }
        public Nullable<int> TransferSubType { get; set; }
        public Nullable<DateTime> TransferDate { get; set; }
        public Nullable<decimal> TransferChargePerSqMtr { get; set; }
        public Nullable<decimal> TotalTransferCharge { get; set; }

        public Nullable<decimal> CurrentPropertyRate { get; set; }
        public Nullable<decimal> LocationCharge { get; set; }
        public Nullable<decimal> TotalPropertyCost { get; set; }
        public Nullable<decimal> AnnualLeaseRent { get; set; }

        //GPA Fields
        public string GPAHolderName { get; set; }
        public string GPAHolderAdd { get; set; }
        public Nullable<DateTime> GPAEffectiveFrom { get; set; }
        public Nullable<DateTime> GPAEffectiveTill { get; set; }

        //Fields added for Refund Details used in Cancellation/Surrender functionality
        public decimal? DeductionAmnt { get; set; }
        public string Unit { get; set; }
        public string DeductionApplyOn { get; set; }
        public int? RefundLockPeriod { get; set; }
        public int? InterestRate { get; set; }
        public int? DaysAfterInterest { get; set; }
        public DateTime? AllotmentDate { get; set; }
        public DateTime? LeaseDeedDate { get; set; }
        public DateTime? PossessionDate { get; set; }
        public string ReasonOfCancel { get; set; }
        public DateTime? CancellationDate { get; set; }

        //For report
        public DateTime? FromSearch { get; set; }
        public DateTime? ToSearch { get; set; }
        public int SchemeId { get; set; }

        [DisplayName("Request Reference No.")]
        public int ReqRefNo { get; set; }

        public string TransferorCorrAdd { get; set; }
        public string TransferorPerAdd { get; set; }
    }

    public class GPAModel
    {
        public int? RId { get; set; }
        public int GPAId { get; set; }
        public string DepttName { get; set; }
        public int DepttId { get; set; }
        public string PropNo { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public string PropertyNo { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string RelationName { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public DateTime? AcceptanceDate { get; set; }
        public bool? IsActive { get; set; }
        public string GPAHolderName { get; set; }
        public string GPAHolderAdd { get; set; }
        public int? GPARegistered { get; set; }
        public string GPARegisteredNo { get; set; }
        public string GPAType { get; set; }
        public string NomineeName { get; set; }
        public string NomineeRelation { get; set; }
        public DateTime? NominationDate { get; set; }
        public int SNo { get; set; }
        public int NomineeId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int hdnRId { get; set; } //Used when DDL is disabled in EditGPA
        public string hdnGPAType { get; set; } //Used when DDL is disabled in EditGPA
        public DateTime? AllotmentDate { get; set; }
        public DateTime? FromSearch { get; set; }
        public DateTime? ToSearch { get; set; }

        public string BahiNo { get; set; }
        public string BahiZildNo { get; set; }
        public string BahiPageNo { get; set; }
        public string SINo { get; set; }

        public int? GPASOA { get; set; } //Sale of Agreement(SOA)
        public int? AgreementType { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string SOABahiNo { get; set; }
        public string SOABahiZildNo { get; set; }
        public string SOABahiPageNo { get; set; }
        public string SOASINo { get; set; }

        public string AdvertismentInNewsPaper { get; set; }
        public DateTime? PublishedDate { get; set; }
    }
    public class InterviewDetailsModel
    {
        public int? SchemeId { get; set; }
        public int Id { get; set; }
        public int? DepartmentId { get; set; }
        public string SchemeName { get; set; }
        public string InterviewDetailsBaseOnAppID { get; set; }
        public string FormNo { get; set; }
        public string DepartmentName { get; set; }
        public string ApplicationName { get; set; }
        public string Gender { get; set; }
        public string RelationName { get; set; }
        public string Address { get; set; }
        public int ApplicationId { get; set; }
        public DateTime? InterviewDate { get; set; }
        public string InterviewDetails { get; set; }
        public string IsAllotted { get; set; }
    }

    public class NotingDetailsModel
    {
        public int? SchemeId { get; set; }
        public int Id { get; set; }
        public int Rid { get; set; }
        public int RidNotingCreate { get; set; }
        public int? DepartmentId { get; set; }
        public string SchemeName { get; set; }
        public string FileName { get; set; }
        public string FileNameNotingCreate { get; set; }
        public string DepartmentName { get; set; }
        public int ApplicationId { get; set; }
        public DateTime? NotingDate { get; set; }
        public string NotingDetail { get; set; }
        public string IsAllotted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ReceiveBy { get; set; }
        public DateTime? Receive_Date { get; set; }
        public string NotingDetails { get; set; }
        public string AddNotingDetails { get; set; }
        public string User { get; set; }
        public int ReceiveByID { get; set; }
        public bool IsNotingDetailShow { get; set; }
        public HtmlString ViewComments { get { return new HtmlString(NotingDetails); } }
        public string EncodedRid { get { return CommonHelper.Encode(Rid.ToString()); } }
    }

    public class DemandLetterModel
    {
        public int Type { get; set; }
        public int DepttId { get; set; }
        public int Sector { get; set; }
        public int Block { get; set; }
        public int RId { get; set; }
        public string ApplicantName { get; set; }
        public string DepttName { get; set; }
        public string SectorName { get; set; }
        public string Blockname { get; set; }
        public string PropNo { get; set; }
        public bool IsSelected { get; set; }
    }

    public class BankAccountManagementModel
    {

        public int? RId { get; set; }
        public int DepttId { get; set; }

        public string PropertyNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string Status { get; set; }

        public string SchemeName { get; set; }
        public string DepttName { get; set; }
        public string ApplicationName { get; set; }


        public string Gender { get; set; }
        public string PropertyType { get; set; }
        public decimal? Area { get; set; }
        public string Floor { get; set; }
        public DateTime? FunctionalDueDate { get; set; }
        public DateTime? FunctionalDate { get; set; }
        public string RelationName { get; set; }


        public bool? IsActive { get; set; }

        public int bankId { get; set; }
        public int branchId { get; set; }
        public string DdlAccountNumber { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }

        public int ChallanId { get; set; }
        public int ChallanTransID { get; set; }
        public string ChallanNo { get; set; }
        public DateTime? ChallanDate { get; set; }

        public int AccountHeadId { get; set; }
        public int AccountSubHeadId { get; set; }
        public decimal Amount { get; set; }
        public string AccountHeadName { get; set; }
        public string AccountSubHeadName { get; set; }

        public int? ServiceRequestId { get; set; }
        public string ServiceRequestName { get; set; }

        public int? ReferenceNo { get; set; }
    }

    public class CitizenServiceRequestModel
    {
        public int RId { get; set; }
        public int departmentId { get; set; }
        public int ServiceType { get; set; }
        public string Description { get; set; }
        public TransferRequestModel transReq { get; set; }
        public string Documents { get; set; }
    }

    public class TransferRequestModel
    {
        public int TransType { get; set; }
        public int TransSubType { get; set; }
        public DateTime? TransDate { get; set; }
        public decimal TransCharge { get; set; }
        public int Gender { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string RelativeName { get; set; }
        public string MotherName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string CorrespondenceAdd { get; set; }
        public string PermanentAdd { get; set; }
        public string PAN { get; set; }
        public int? Occupation { get; set; }
        public string TransfereeCompanySigningAuth { get; set; }
        public string TransfereeCompanyName { get; set; }
        public string TransfereeCompanyRegOff { get; set; }
        public int? TypeOfTransferee { get; set; }
        public string StrTransType { get; set; }
        public string StrTransSybType { get; set; }
        public string Name { get; set; }
        public string StrOccupation { get; set; }
        public string StrGender { get; set; }
    }

    public class ServiceRequestDocument
    {
        public int ChkDocumentId { get; set; }
        public string DocumentPath { get; set; }
        public DateTime Uploaded_Date { get; set; }
        public int Uploaded_By { get; set; }
        public string ChkDocumentName { get; set; }
    }

    public class CitizenServiceRequest
    {
        public string Registration_No { get; set; }
        public string Property_No { get; set; }
        public int DepartmentId { get; set; }
        public int? ServiceId { get; set; }
        public DateTime Created_Date { get; set; }
        public int Created_By { get; set; }
        public DateTime Modified_Date { get; set; }
        public int Modified_By { get; set; }
        public int Request_Status { get; set; }
        public string Description { get; set; }
        public int AmountTobePaid { get; set; }
        public DateTime? LastDateofPayment { get; set; }
        public string Comment { get; set; }
        public int ServiceRequestId { get; set; }
        public List<ServiceRequestDocument> DocumentList { get; set; }
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public decimal? ServiceFee { get; set; }
        public decimal? DuesAmnt { get; set; }
        public string PaymentStatus { get; set; }
        public string Status { get; set; }
        public TransferRequestModel transDetails { get; set; }
    }

    public class TransferServiceRequestModel
    {
        public string RelativeName { get; set; }
        public string MotherName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string CorrespondenceAdd { get; set; }
        public string PermanentAdd { get; set; }
        public string StrTransType { get; set; }
        public string StrTransSybType { get; set; }
        public string Name { get; set; }
        public string StrOccupation { get; set; }
        public string StrGender { get; set; }
        public string PAN { get; set; }
    }
}
