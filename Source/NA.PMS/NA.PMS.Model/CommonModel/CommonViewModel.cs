using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Model
{
    public static class CommonHelper
    {
        //encrypt optional parameter in url
        public static string Encode(string encodeVal)
        {
            byte[] encoded = Encoding.UTF8.GetBytes(encodeVal);
            return Convert.ToBase64String(encoded);
        }
        //decrypt optional parameter in uirl
        public static string Decode(string decodeVal)
        {
            if (!string.IsNullOrEmpty(decodeVal) && decodeVal != "undefined")
            {
                byte[] encoded = Convert.FromBase64String(decodeVal);
                return Encoding.UTF8.GetString(encoded);
            }
            else
            {
                return decodeVal;
            }
        }

    }

    public class DynamicDataModel
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public class DynamicDateModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime NoticeDate { get; set; }
        public DateTime PublishDate { get; set; }
    }

    public class DDList
    {
        public int id { get; set; }
        public string text { get; set; }
        public Boolean RecordExistsIn { get; set; }
    }

    public class DtoList
    {
        public string value { get; set; }
        public string label { get; set; }
    }

    public class SectorDDList
    {
        public int id { get; set; }
        public string text { get; set; }
        public int DepartmentId { get; set; }
    }

    public class RidList
    {
        public int DepartmentId { get; set; }
        public int id { get; set; }
        public string text { get; set; }
    }

    public class FormAllotmentList
    {
        public int DepartmentId { get; set; }
        public string id { get; set; }
        public Boolean RecordExistsIn { get; set; }
        public string text { get; set; }
    }

    public class DDLStringList
    {
        public string id { get; set; }
        public string text { get; set; }
        public Boolean RecordExistsIn { get; set; }
    }

    public class CheckBoxListItem
    {
        public int id { get; set; }
        public string display { get; set; }
        public bool isChecked { get; set; }
    }

    public class CheckBoxViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> CheckBoxId { get; set; }
        public bool IsChecked { get; set; }
        public string CheckBoxName { get; set; }       
    }

    public class DropdownViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string RoleName { get; set; }
        public string MenuName { get; set; }
        public string ActionType { get; set; }
        public string ReturnType { get; set; }
        public string FilterType { get; set; }
        public string ChallanRefId { get; set; }
        public string TransactionId { get; set; }
        public string Application { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> MenuId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> FilterTypeId { get; set; }
        public Nullable<int> ActionTypeId { get; set; }
        public Nullable<int> ReturnTypeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> PropertTypeId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<long> ReceiptId { get; set; }
        public Nullable<long> RefId { get; set; }
        public Nullable<long> ApplicationId { get; set; }
        public Nullable<DateTime> FilterDate { get; set; }
    }
    

    public class DropdownViewModelDepartment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public int SchemeId { get; set; }
    }

    public class ChallanBankandAccountNo
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string AccountNo { get; set; }
        public string BankIFSC { get; set; }
        public string VirtualAccountPrefix { get; set; }
    }

    public class BankAccountViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> BankId { get; set; }
        public Nullable<int> BranchId { get; set; }

        public string BankName { get; set; }
        public string BranchAddress { get; set; }
        public string Department { get; set; }
        public string AccountNo { get; set; }
        public string IFSCCode { get; set; }
        public string Status { get; set; }
        public string BankStatus { get; set; }
        public string BranchStatus { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string ActionType { get; set; }
        public string FilterType { get; set; }

        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsBankActive { get; set; }
        public Nullable<bool> IsBranchActive { get; set; }
    }

    public class CommonViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> RegistrationNo { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<int> SchemeTypeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> PropertyTypeId { get; set; }
        public Nullable<int> AreaRangeId { get; set; }
        public Nullable<int> ParentMenuId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<int> MenuId { get; set; }
        public Nullable<int> Range { get; set; }
        public Nullable<int> TypeId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> OTP { get; set; }
        public Nullable<int> ActionTypeId { get; set; }
        public Nullable<int> FilterTypeId { get; set; }
        public Nullable<int> ReturnTypeId { get; set; }

        public string RegistrationId { get; set; }
        public string Application { get; set; }
        public string SchemeType { get; set; }
        public string Department { get; set; }
        public string PropertyType { get; set; }
        public string AreaRange { get; set; }
        public string AreaType { get; set; }
        public string ParentMenu { get; set; }
        public string Menu { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string StatusType { get; set; }
        public string SchemeTypeDescription { get; set; }
        //public string ActionType { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string ActionType { get; set; }
        public string FilterType { get; set; }
        public string ReturnType { get; set; }
        public Nullable<decimal> PremiumDues { get; set; }
        public Nullable<decimal> LeaseRentDues { get; set; }
        public Nullable<DateTime> LastPremiumPaidDate { get; set; }
        public Nullable<DateTime> LeseRentStartDate { get; set; }
        public Nullable<DateTime> LeaseRentEndDate { get; set; }
    }

    public class NotingViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> NotingFileId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<int> ReceiverId { get; set; }
        public Nullable<int> ApproverId { get; set; }
        public Nullable<int> StatusId { get; set; }

        public string NotingFileNo { get; set; }
        public string SchemeName { get; set; }
        public string Department { get; set; }
        public string Applicant { get; set; }
        public string Address { get; set; }
        public string PropertyNo { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string FileName { get; set; }
        public string NotingFile { get; set; }
        public string NotingDetail { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string Receiver { get; set; }
        public string Approver { get; set; }
        public string Content { get; set; }
        public string Comment { get; set; }
        public string Message { get; set; }
        public string ActionType { get; set; }

        public Nullable<DateTime> NotingDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ReceivedDate { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsAllotted { get; set; }
        public Nullable<bool> IsNotingToDisplay { get; set; }

        public HtmlString HtmlNoting { get { return string.IsNullOrEmpty(NotingDetail) ? null : new HtmlString(NotingDetail); } }
        public string EncryptedRegistrationId { get { return RegistrationId == null ? null : CommonHelper.Encode(RegistrationId.ToString()); } }
    }

    public class PaymentLedgerViewModel
    {
        public Nullable<int> Id { get; set; }
        //public Nullable<int> ReceiptId { get; set; }
        public Nullable<int> ReceiptHeadId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> ReceiptSubHeadId { get; set; }
        public Nullable<int> DepartmentId { get; set; }

        public string Applicant { get; set; }
        public string ApplicantType { get; set; }
        public string Department { get; set; }
        public string Block { get; set; }
        public string Sector { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo { get; set; }
        public string DepositerName { get; set; }
        public string BankId { get; set; }
        public string ChallanId { get; set; }

        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string GSTNo { get; set; }
        public string PAN { get; set; }
        public string CorrespondAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string ServiceName { get; set; }
        public string FloorArea { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string ReceiptHead { get; set; }
        public string ReceiptSubHead { get; set; }

        public Nullable<DateTime> DepositDate { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public Nullable<decimal> AmountPaid { get; set; }
        public Nullable<long> ReceiptId { get; set; }
    }

    public class ChallanViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RequestId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> BankId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> FloorAreaId { get; set; }
        public Nullable<int> PropertyTypeId { get; set; }
        public Nullable<int> AccountHeadId { get; set; }
        public Nullable<int> AccountSubHeadId { get; set; }
        public Nullable<int> ServiceRequestId { get; set; }
        public Nullable<int> TotalVerified { get; set; }
        public Nullable<int> TotalNotVerified { get; set; }
        public Nullable<int> TotalChallanCount { get; set; }
        public Nullable<int> ReturnTypeId { get; set; }
        public Nullable<int> EmployeeCode { get; set; }
        public Nullable<int> ReferenceTypeId { get; set; }

        public string ChallanId { get; set; }
        public string SchemeName { get; set; }
        public string Department { get; set; }
        public string FormNo { get; set; }
        public string PropertyType { get; set; }
        public string PropertyNo { get; set; }
        public string Applicant { get; set; }
        public string ApplicantType { get; set; }
        public string ApplicantMaster { get; set; }
        public string Block { get; set; }
        public string Sector { get; set; }
        public string PlotNo { get; set; }
        public string PlotUnitNo { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string GSTNo { get; set; }
        public string PAN { get; set; }
        public string CorrespondAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string ServiceName { get; set; }
        public string FloorArea { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string AccountNo { get; set; }
        public string AccountHead { get; set; }
        public string AccountSubHead { get; set; }
        public string TotalDueInWords { get; set; }
        public string ChallanContent { get; set; }
        public string ActionType { get; set; }
        public string FilterType { get; set; }
        public string ReferenceNo { get; set; }
        public string ReferenceType { get; set; }
        public string IFSCCode { get; set; }
        public string ChallanStatus { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string HtmlContent { get; set; }
        public string ReturnType { get; set; }
        public string EmployeeName { get; set; }
        public string ReceivedBank { get; set; }
        public string ReceivedOrg { get; set; }
        public string ChequeNumber { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsCancelled { get; set; }
        public Nullable<bool> IsVerified { get; set; }
        public Nullable<bool> IsBankUserId { get; set; }

        public Nullable<DateTime> AllotmentDate { get; set; }
        public Nullable<DateTime> GeneratedDate { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<DateTime> ChequeDate { get; set; }

        public Nullable<decimal> TotalArea { get; set; }
        public Nullable<decimal> AllotmentMoney { get; set; }
        public Nullable<decimal> LeaseRentAmount { get; set; }
        public Nullable<decimal> InstallmentAmount { get; set; }
        public Nullable<decimal> TotalDues { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> CompletionCharge { get; set; }
        public Nullable<decimal> TotalHeadAmount { get; set; }

        public Nullable<decimal> DepositedAmount { get; set; }
        public Nullable<decimal> NotDepositedAmount { get; set; }

        public List<decimal> AmountList { get; set; }

        public List<string> AccountHeadList { get; set; }
        public List<string> AccountSubHeadList { get; set; }
    }

    public class ChallanModel
    {
        public string ChallanId { get; set; }
        public int RID { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public string SchemeName { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string FormNo { get; set; }
        public string PropertyTypeName { get; set; }
        public string PropertyNumber { get; set; }
        public string ApplicantName { get; set; }
        public Nullable<System.DateTime> AllotmentDate { get; set; }
        public Nullable<System.DateTime> InstalmentStartDate { get; set; }
        public Nullable<int> IsActive { get; set; }
        public string BlockName { get; set; }
        public string SectorName { get; set; }
        public PropertyAllotmentModel PropertyAllotment { get; set; }
        public ApplicationFormModel ApplicationForm { get; set; }
        public Nullable<decimal> AllotmentMoney { get; set; }

        //Properties for Lease Rent Challan
        public decimal? LeaseRentAmount { get; set; }
        public decimal? Dues { get; set; }
        public decimal? TotalDue { get; set; }
        public string TotalDueInWords { get; set; }
        public Decimal? CompletionCharges { get; set; }

        public int? BankId { get; set; }
        public string BankName { get; set; }
        public int? Amount { get; set; }
        public decimal? Amount1 { get; set; }
        public decimal? Amount2 { get; set; }
        public decimal? Amount3 { get; set; }
        public int? AccountHeadId { get; set; }
        public string AccountHeadName1 { get; set; }
        public string AccountHeadName2 { get; set; }
        public string AccountHeadName3 { get; set; }
        public int? AccountSubHeadId { get; set; }
        public string AccountSubHeadName1 { get; set; }
        public string AccountSubHeadName2 { get; set; }
        public string AccountSubHeadName3 { get; set; }
        public List<string> AccountHeadName { get; set; }
        public List<string> AccountSubHeadName { get; set; }

        public int? ServiceRequestId { get; set; }
        public string ServiceName { get; set; }

        //public List<decimal> Amount { get; set; }
        public decimal? TotalAmount
        {
            get
            {
                var amount1 = Amount1; var amount2 = Amount2; var amount3 = Amount3;

                if (amount1 == null) amount1 = 0;
                if (amount2 == null) amount2 = 0;
                if (amount3 == null) amount3 = 0;
                return amount1 + amount2 + amount3;
            }
        }

        public List<BankAccountManagementModel> ModelHeadProperty { get; set; }
        public decimal? TotalHeadAmount { get; set; }
    }

    public class AdvanceSearchModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> FilterTypeId { get; set; }
        public Nullable<int> ActionTypeId { get; set; }
        public string Department { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string Applicant { get; set; }
        public string ApplicantMaster { get; set; }
        public string MotherName { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string FilterType { get; set; }
        public string ActionType { get; set; }
    }

    public class MergeSplitRidSelectionDDList
    {
        public int id { get; set; }
        public string text { get; set; }
        public int BlockId { get; set; }
    }

    public class MergeSplitBlockSelectionDDList
    {
        public int id { get; set; }
        public string text { get; set; }
        public int SectorId { get; set; }
    }

    public class ApplicantModel
    {
        public int Id { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public string RegistrationNo { get; set; }
        public string PropertyNo { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantMaster { get; set; }
        public string CompanyName { get; set; }
        public string FatherOrHusband { get; set; }
        public string SigningAuthority { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string PermanentAddress { get; set; }
        public string CorresspondingAddress { get; set; }
        public string RegisteredOffice { get; set; }
        public string Comment { get; set; }
        public string Message { get; set; }
        public string ExistingMessage { get; set; }
        public string MessageKey { get; set; }
        public string MessageValue { get; set; }

        public Nullable<decimal> TransferCharge { get; set; }

        public Nullable<DateTime> AllotmentDate { get; set; }
        public Nullable<DateTime> InstallmentStartDate { get; set; }
        public Nullable<DateTime> PossessionDate { get; set; }
        public Nullable<DateTime> PossessionOrderDate { get; set; }
        public Nullable<DateTime> ChecklistDate { get; set; }
        public Nullable<DateTime> RegistryDueDate { get; set; }
        public Nullable<DateTime> RegistryDate { get; set; }
        public Nullable<DateTime> TransferDate { get; set; }
    }

    public class SchemePropertyModel
    {
        public int Id { get; set; }
        public Nullable<int> propertyId { get; set; }
        public Nullable<int> ParentPropertyId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> PropertyTypeId { get; set; }
        public Nullable<int> GroupProjectId { get; set; }
        public Nullable<int> VillageId { get; set; }

        public string Sector { get; set; }
        public Nullable<int> SectorId { get; set; }
        public string Block { get; set; }
        public Nullable<int> BlockId { get; set; }
        public string Floor { get; set; }
        public Nullable<int> FloorId { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo { get; set; }
        public string Registry { get; set; }
        public string KhasraNumber { get; set; }
        public string KhatoniNumber { get; set; }
        public string VillageName { get; set; }

        public Nullable<decimal> PropertyCost { get; set; }
        public Nullable<decimal> CivilCost { get; set; }
        public Nullable<decimal> TotalPropertyCost { get; set; }
        public Nullable<decimal> PropertyRate { get; set; }
        public Nullable<decimal> TotalArea { get; set; }
        public Nullable<decimal> CoveredArea { get; set; }
        public Nullable<decimal> ActualArea { get; set; }
        public Nullable<decimal> ProcessingFee { get; set; }
        public Nullable<decimal> AllotmentMoney { get; set; }
        public Nullable<decimal> EarnestMoney { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }
    }

    public class DashBoardGraph
    {
        public string Graph { get; set; }
    }

    public class LetterViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> Rid { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> RequestId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> TemplateId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> LetterId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> ActionTypeId { get; set; }

        public string Department { get; set; }
        public string RequestStatus { get; set; }
        public string ServiceName { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo { get { return Sector + "/" + (string.IsNullOrEmpty(Block) ? string.Empty : (Block + "-")) + PlotNo; } }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Applicant { get; set; }
        public string ApplicantMaster { get; set; }
        public string ApplicantType { get; set; }
        public string CorrespondAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string IndividualOrCompany { get; set; }
        public string FatherOrHusbandName { get; set; }
        public string AutorizedSignatory { get; set; }
        public string LetterType { get; set; }
        public string BarcodeValue { get; set; }
        public string LetterContent { get; set; }
        public string ActionType { get; set; }
        public string CreatedBy { get; set; }
        public string Template { get; set; }

        public Nullable<DateTime> AllotmentDate { get; set; }
        public Nullable<DateTime> LetterDate { get; set; }
        public Nullable<DateTime> GeneratedDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
    }

    public class NDCVeiwModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RegistrationNo { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> BankId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> OnlineReqNo { get; set; }
        public Nullable<int> RequesterId { get; set; }
        public Nullable<int> ApproverId { get; set; }

        public string RegistrationId { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Applicant { get; set; }
        public string Address { get; set; }
        public string Department { get; set; }
        public string DepartmentInHindi { get; set; }
        //Changes on 9th august 2017
        public string IsPremiumPaid { get; set; }
        public string IsLeaseRentPaid { get; set; }
        public string Status { get; set; }
        public string ActionType { get; set; }
        public string LastYearLeaseRentPaidUpto { get; set; }

        public string BankName { get; set; }
        public string Remarks { get; set; }
        public string LeaseRentInWord { get; set; }
        public string InstallmentInWord { get; set; }
        public string InterestInWord { get; set; }
        public string ChallanId { get; set; }
        public string ChalanAmount { get; set; }
        public string OneTimeLeaseRent { get; set; }
        public string OneTimeInstallment { get; set; }

        public string InstallmentDateInWord { get; set; }
        public string InterestDateInWord { get; set; }
        public string LeaseRentDateInWord { get; set; }
        public string LetterNo { get; set; }
        public string LetterDateInWord { get; set; }
        public string Requester { get; set; }
        public string Approver { get; set; }
        public string Comment { get; set; }

        public string TemplateContent { get; set; }

        public Nullable<bool> IsTotalInstallmentPaid { get; set; }
        public Nullable<bool> IsOneTimeLeasePaid { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsLetterIssued { get; set; }
        public Nullable<bool> IsDuesExist { get; set; }

        public Nullable<DateTime> RegistryDate { get; set; }
        public Nullable<DateTime> NDCDate { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<DateTime> LeaseRentPaidDate { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> LeaseRentUptoDate { get; set; }
        public Nullable<DateTime> LeaseRentPaidUpto { get; set; }
        public Nullable<DateTime> LeaseRentInterestPaidUpto { get; set; }
        public Nullable<DateTime> InstallmentPaidUpto { get; set; }
        public Nullable<DateTime> InstallmentInterestPaidUpto { get; set; }
        public Nullable<DateTime> RequestedDate { get; set; }
        public Nullable<DateTime> ApprovalDate { get; set; }

        public Nullable<double> PremiumDues { get; set; }
        public Nullable<double> LeaseRentDues { get; set; }
        public Nullable<double> LeaseRentAmount { get; set; }
        public Nullable<double> LeaseRentRevisedAfterYear { get; set; }
        public Nullable<double> LeaseRentRevisedPercentage { get; set; }
        public Nullable<double> NDCCount { get; set; }
        public Nullable<double> DemandCount { get; set; }

        public Nullable<decimal> LeaseRent { get; set; }
        public Nullable<decimal> InstallmentPaidAmount { get; set; }
        public Nullable<decimal> InstallmentInterestPaidAmount { get; set; }
        public Nullable<decimal> LeaseRentPaidAmount { get; set; }
        public Nullable<decimal> LeaseRentInterestPaidAmount { get; set; }

        public List<double> NDCList { get; set; }
    }



    public class PropertyDetailViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RefId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> ParentPropertyId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> PropertyTypeId { get; set; }
        public Nullable<int> AreaRangeId { get; set; }
        public Nullable<int> FloorAreaId { get; set; }
        public Nullable<int> VillageId { get; set; }
        public Nullable<int> GroupProjectId { get; set; }

        public string FlagString { get; set; }
        public string RegistrationNo { get; set; }
        public string SchemeName { get; set; }
        public string Department { get; set; }
        public string PropertyNo { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string AreaRange { get; set; }
        public string FloorArea { get; set; }
        public string PropertyType { get; set; }
        public string GenderType { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Applicant { get; set; }
        public string ApplicantType { get; set; }
        public string ApplicantMaster { get; set; }
        public string ApplicantAddress { get; set; }
        public string CorrespondAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Registry { get; set; }
        public string TransferType { get; set; }
        public string TransferFlag { get; set; }
        public string KhasraNumber { get; set; }
        public string KhatoniNumber { get; set; }
        public string PAN { get; set; }
        public string GST { get; set; }

        public Nullable<decimal> PropertyCost { get; set; }
        public Nullable<decimal> CivilCost { get; set; }
        public Nullable<decimal> TotalPropertyCost { get; set; }
        public Nullable<decimal> PropertyRate { get; set; }
        public Nullable<decimal> TotalArea { get; set; }
        public Nullable<decimal> CoveredArea { get; set; }
        public Nullable<decimal> ActualArea { get; set; }
        public Nullable<decimal> ProcessingFee { get; set; }
        public Nullable<decimal> AllotmentMoney { get; set; }
        public Nullable<decimal> EarnestMoney { get; set; }
        public Nullable<decimal> TransferCharge { get; set; }
        public Nullable<decimal> TransferCost { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }
        public Nullable<decimal> LeaseRent { get; set; }

        public Nullable<DateTime> AllotmentDate { get; set; }
        public Nullable<DateTime> ChecklistDate { get; set; }
        public Nullable<DateTime> InstallmentStartDate { get; set; }
        public Nullable<DateTime> PossessionDate { get; set; }
        public Nullable<DateTime> PossessionOrderDate { get; set; }
        public Nullable<DateTime> FunctionalDate { get; set; }
        public Nullable<DateTime> RentingDate { get; set; }
        public Nullable<DateTime> RegistryDueDate { get; set; }
        public Nullable<DateTime> RegistryDate { get; set; }
        public Nullable<DateTime> TransferDate { get; set; }
        public Nullable<DateTime> MortgageDate { get; set; }
    }

    public class ResultMessage
    {
        public ResultMessage()
        {
            this.clsResultType = new List<ResultMessage>();
        }
        public int ReturnType { get; set; }
        public string Message { get; set; }
        public int PrimaryKey { get; set; }
        public List<ResultMessage> clsResultType { get; set; }
    }

    public class InstallmentDues_Payment
    {
        public int RegistrationId { get; set; }
        public DateTime? InstallmentStartDate { get; set; }
        public DateTime? InstallmentEndDate { get; set; }
        public bool? IsOneTimeLeasePaid { get; set; }
        public bool? IsTotalPremiumPaid { get; set; }
        public decimal? DuesAmount { get; set; }
        public DateTime? DuesUptoDate { get; set; }
        public decimal? BalanceAmount { get; set; }
        public DateTime? BalanceUptoDate { get; set; }
    }

}
