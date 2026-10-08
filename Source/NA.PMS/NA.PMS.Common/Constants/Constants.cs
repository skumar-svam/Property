namespace NA.PMS.Common
{
    public static class Constants
    {
        public static int UserID;

        public const int AdminRoleId = 1;
        public const int SuperAdminRoleId = 1001;
        public const int SvamAdmin = 1133;
        public const int individual = 1;
        public const int company = 2;
        public const int BranchIdOther = 5;
        public const int DepartmentIdForHousing = 5;
        public const int DefineYearByAuthority = 11;

        public const int Approved = 1;
        public const int RejectedProp = 2;
        public const int Cancelled = 3;
        public const int Pending = 4;
        public const int InProgress = 5;
        public const int NotSubmitted = 6;
        public const int ReSubmitted = 0;
        public const int Initiated = 8;
        // In Case we are going to invalid this entry.
        public const int InValid = 3;
        public const int PreviousLoanNo = 2;
        //Used for checking if One Time Lease Rent has been paid or not
        public const int OTLRReceiptHead = 8;
        public const int OTLRReceiptSubHead = 97;
        public const int PreviousLoanYes = 1;
        public const int MortgageTemplateID = 5;
        public const int CompletionTemplateID = 16;
        public const int FunctionalCertificatID = 6;
        public const int DemandLetterTemplate = 3;
        public const int CICTemplateID = 4;
        public const int ExtensionTemplateID = 17;
        public const int BankChallanGenerate = 15;
        public const int BulkAllotmentLetterTemplateID = 1;
        public const int BulkAllotmentPaymentScheduleTemplateID = 13;
        public const int transDeedTemplateId = 2;
        public const int leaseDeedTemplateId = 18;
        public const int RentPermissionTemplateID = 14;

        public const int ChangeInDirector = 1;
        public const int ChangeInFirmName = 2;
        public const int ChangeInFirmStatus = 3;
        public const int ChangeInProduct = 4;
        // Amalgamation of Properties
        public const int Amalgamation = 17;
        public const int Deamalgamation = 18;

        public const int intCancellation = 19;//From Common_Config table, which will remain unchanged when deployed on Production
        public const int intSurrender = 20;//From Common_Config table, which will remain unchanged when deployed on Production
        public const int intRestoration = 21;//From Common_Config table, which will remain unchanged when deployed on Production

        public const int PropertyCancellationId = 19;
        public const int PropertySurrenderId = 20;
        public const int PropertyRestorationId = 21;

        public const int intPremium = 1;
        public const int intLeaseRent = 2;
        public const int GPATransType = 2;
        public const int intLease = 22;
        public const int intSublease = 23;
        public const int SchemeOpen = 24;
        public const int SchemeProgress = 25;
        public const int SchemeClosed = 26;
        public const int CompletionId = 9;
        public const int PossessionId = 6;
        public const int General = 5;
        public const int Success = 1;
        public const int Faliure = -1;
        public const int OnlineApplicationPayment = 1;
        public const int OnlineOtherPayments = 2;
        public const int offlineApplicationPayment = 3;
        public const int offlinePrevoiusChallanApplicationPayment = 4;
        public const int singleWindowPortalApplicationPayment = 5;
        public const int BrochureApplicationPayment = 6;
        public const int RoleIdSDUser = 2197;
        public const int RoleIdSDOfficer = 2198;
        public const int Draw_Winner = 1;
        public const int Active = 1;
        public const int NABlockId = 14;
        public const int NASectorId = 211;
        public const int NDCTemplateId = 65;

        public const int InstallmentDemandNoteId = 1;
        public const int LeaseRentDemandNoteId = 2;
        public const int InstallmentAndLeaseRentDemandNoteId = 3;

        public const int CancellationId = 19;
        public const int SurrenderId = 20;
        public const int RestorationId = 21;

        public const string JSK = "JSK";
        public const string Online = "Online";
        public const string NIC_NiveshMitra = "NIC Nivesh Mitra";

        public const string SuperAdmin = "SA";
        public const string Admin = "A";
        public const string User = "U";
        public const string Consultant = "Consultant";
        public const string Male = "Male";
        public const string Female = "Female";
        public const string Company = "Company";
        public const string Individual = "Individual";
        public const string Married = "married";
        public const string DD = "DD";
        public const string RTGS = "RTGS";
        public const string Other = "Other";

        public const string Rejected = "Rejected";
        public const string Allotted = "Allotted";
        public const string PendingPayment = "Pending Payment";
        public const string Duplicate = "duplicate";
        public const string Uploaded = "uploaded";
        public const string NotNull = "notnull";
        public const string Form = "form";
        public const string collateral = "Collateral";
        public const string normal = "Normal";
        public const string genderCompany = "Company";
        public const string genderIndividual = "Individual";
        public const string strDeptGroupHousing = "Group Housing";
        public const string KYA = "KYA";
        public const string Authority = "Authority";
        public const string Mismatch = "Mismatch";
        public const string Installment = "Installment";
        public const string Leaserent = "Leaserent";
        public const string PremiumSchedule = "Premium Schedule";
        public const string DemandNote = "DemandNote";
        public const string NoidaAuthorityGSTNo = "09AAALN0120A1ZV";

        public const string yes = "Yes";
        public const string Y = "Y";
        public const string no = "No";
        public const string NA = "N/A";
        public const string SectorNotAvailable = "NA";
        public const string BlockNotAvailable = "NA";
        public const string IsApproved = "Approved";
        public const string cancellation = "Cancellation";
        public const string surrender = "Surrender";
        public const string restoration = "Restoration";
        public const string premium = "Premium";
        public const string leaseRent = "Lease Rent";
        public const string reschedule = "R";//Used in DB for Rescheduled Payments
        public const string strLease = "Lease Deed";
        public const string strSublease = "Sublease Deed";
        public const string SchemeOnline = "Online";
        public const string SchemeOffline = "Offline";
        public const string strOther = "Other";
        public const string Registered = "Registered";
        public const string UnRegistered = "UnRegistered";
        public const string PradhikaranDiwas = "PradhikaranDiwas";
        public const string JSKService = "O";
        public const string SDService = "P";
        public const string CustomerService = "C";
        public const string ApplicationFormTypeG = "General";
        public const string ApplicationFormTypeS = "Startup";
        public const string ApplicationFormTypeEx = "Expansion";
        public const string RoleIdSDU = "SDU";
        public const string RoleIdSDO = "SDO";
        public const string Transfer = "T";
        public const string Mutation = "M";
        public const string rejectedMailSubject_PIS = "Noida Authority Form Rejection";
        public const string deactivatedMailSubject_PIS = "Noida Authority Account Deactivation Rejection";
        public const string PaymentStatus = "Payment Status";
        public const string AppType = "NIC";
        public const string NIC = "NIC";
        public const string Verify = "Verify";
        public const string Cancel = "Cancel";
        public const string ReqComplete = "_Complete";
        public const string ReqObjection = "_Objection";

        public const string SCRUTINY = "SCRUTINY";

        public const string GSTNoticeInHindi = "कृपया भूखंड के विरुद्ध जी. एस. टी. की देयता आवंटी द्वारा स्वतः Reverse Charge Mechanism से जमा कराना होगा | प्राधिकरण का जी. एस. टी. नं. : 09AAALN0120A1ZV है |";


        #region History Creation Constants...
        public const string Insert = "I";
        public const string Update = "U";
        public const string Delete = "D";
        public const string ViewName = "ViewName";
        public const string UmRoleAppTrans = "UmRoleAppTrans";
        public const string ChecklistTrans = "ChecklistTrans";
        public const string RegistryDetails = "RegistryDetails";
        public const string Completion_Details = "Completion_Details";
        public const string RentPermissionDetails = "RentPermissionDetails";
        public const string FunctionalDetails = "FunctionalDetails";
        public const string AccountChallanChargeDetail = "ChallanChargeDetail";
        public const string InterviewDetails = "InterviewDetails";

        public const string FirmDirectorMaster = "Firm_Director_Master";
        public const string DirectorRequestMaster = "Director_Request_Master";
        public const string FirmMaster = "Firm_Master";
        public const string MortgageDetail = "MortgageDetail";


        public const string AddCIC = "AddCIC";
        public const string ApproveCIC = "ApproveCIC";
        public const string AddFirm = "AddFirm";
        public const string AddFirmProduct = "AddFirmProduct";
        public const string DeleteFirmDirectorMaster = "Firm_Director_Master";
        public const string UpdateFirmDirectorMaster = "Firm_Director_Master";
        public const string AddMortgageDetail = "AddMortgageDetail";

        public const string NotingDetails = "NotingDetails";
        public const string Noting = "Noting";
        #endregion

        public const bool IsActive = true;

        public static readonly string[] AllotmentHeader = { "Form No", "Property No", "Allotment Date", "Installment StartDate" };

        //PIS Constants
        public enum Roles
        {
            Administrator,
            Customer
        }

    }

    public static class AccountReceipt
    {
        public const int OldInstallment = 1;
        public const int NormalInstallment = 1;
    }

    public static class PaymentStatus
    {
        public const string NotPaid = "Not Paid";
        public const string Paid = "Paid";
    }

    public static class SchemeType
    {
        public const string Online = "Online";
        public const string Offline = "Offline";
    }

    public static class ActiveStatus
    {
        public const int Yes = 1;
        public const int No = 0;
    }

    public static class ActionType
    {
        public const string Allotment = "Allotment";
        public const string Property = "Property";
        public const string Application = "Application";
        public const string Online = "Online";
        public const string RegistrationId = "RegistrationId";
        public const string ServiceStatus = "ServiceStatus";
        public const string Comment = "Comment";
        public const string ServiceId = "RegistrationId";
        public const string Delete = "Delete";
        public const string Save = "Save";
    }

    public static class ReturnType
    {
        public const int None = 0;
        public const int Approved = 1;
        public const int Success = 1;
        public const int Failure = 2;
        public const int Exist = 3;
        public const int NotExist = 4;
        public const int Saved = 5;
        public const int Updated = 6;
        public const int Removed = 7;
        public const int Failed = 8;
        public const int UserNameNotExist = 8;
        public const int PasswordNotExist = 8;
        public const int NotPaid = 9;
        public const int Paid = 10;
        public const int Allotted = 11;
        public const int NotAllotted = 12;
        public const int Locked = 13;
        public const int Rejected = 14;
        public const int Resend = 15;
        public const int Forwarded = 16;
        public const int Validated = 17;
        public const int Initiated = 18;
        public const int NotAvailable = 19;
        public const int Other = 20;
        public const int Cancelled = 21;
        public const int Mismatch = 22;
        public const int Verified = 23;
        public const int Rescheduled = 24;
        public const int Completed = 25;
        public const int Pending = 26;
        public const int NotRegistered = 27;
        public const int Appointment = 28;
        public const int Deleted = 29;
        public const int NoAction = 30;
    }

    public static class OnlineSchemeType
    {
        public const string IndustrialPlots = "IP";
        public const string IndustrialScheme = "Industrial Scheme";
        public const string OpenEnded = "Open End Scheme";
        public const string Transport = "Transport Scheme";
        public const string Institutional = "Institutional Scheme";
    }


    public static class HttpMethodType
    {
        public const string POST = "POST";
        public const string GET = "GET";
    }

    public static class NADepartment
    {
        public const int Institutional = 1;
        public const int Commercial = 2;
        public const int Residential = 3;
        public const int Industrial = 4;
        public const int Housing = 5;
        public const int GroupHousing = 6;
        public const int Village = 7;
    }

    public static class NASchemeType
    {
        public const string IndustriaScheme = "IndustrialScheme";
        public const string TransportScheme = "TransportScheme";
        public const string IndustriaOpenScheme = "IndustrialOpenScheme";
        public const string IndustrialPlots = "IP";
        public const string OpenEnded = "Open End Scheme";
        public const string Transport = "Transport Scheme";
        public const string InstitutionalScheme = "Institutional Scheme";
    }

    public static class DepartmentInHindi
    {
        public const string Institutional = "संस्थागत";
        public const string Industry = "औद्योगिक";
        public const string Housing = "भवन";
        public const string Commercial = "वाणिज्य";
        public const string Residential = "आवासीय";
        public const string GroupHousing = "समूह आवास";
        public const string Village = "ग्रामीण आवासीय";

        public const string InstitutionalNote = "उपरोक्त सूचना पत्र के अतिरिक्त, सम्बंधित भूखंड मे ६४.७०% अतिरिक्त प्रतिकर की जो देयता होगी वो आबंटी  को अतिरिक्त प्रतिकर की धनराशि ब्याज के साथ जमा करनी होगी |";
        public const string DemandNote = "यह पत्र कंप्यूटर द्वारा तैयार किया गया है। यह केवल जानकारी के लिए है। आप किसी भी कानूनी कार्यवाही के लिए इस पत्र का उपयोग नहीं कर सकते। यदि इस पत्र के अनुसार कोई विसंगति है, तो आपको नोएडा प्राधिकरण में आना होगा।";

        public const string IndustryUnicode = "&#2324;&#2342;&#2381;&#2351;&#2379;&#2327;&#2367;&#2325;";
        public const string InstitutionalUnicode = "&#2360;&#2306;&#2360;&#2381;&#2341;&#2366;&#2327;&#2340;";
        public const string HousingUnicode = "&#2349;&#2357;&#2344;";
        public const string CommercialUnicode = "&#2357;&#2381;&#2351;&#2366;&#2357;&#2360;&#2366;&#2351;&#2367;&#2325;";
        public const string ResidentialUnicode = "&#2310;&#2357;&#2366;&#2360;&#2368;&#2351;";
        public const string GroupHousingUnicode = "&#2360;&#2366;&#2350;&#2370;&#2361;&#2367;&#2325;";
    }

    public static class RoleInDepartment
    {
        public const string Admin = "Admin";
        public const string OSD = "OSD";
        public const string HOD = "HOD";
        public const string Manager = "Manager";
        public const string Assistant = "Assistant";
        public const string Accountant = "Accountant";
        public const string Property = "Property";        
        public const string HODAccounts = "AccountsOfficer";
    }

    public static class NAService
    {
        public const int Transfer = 1;
        public const int Rent = 2;
        public const int CIC = 3;
        public const int Mortgage = 4;
        public const int LeaseDeed = 5;
        public const int Extension = 6;
        public const int Amalgmation = 7;
        public const int Deamalgmation = 7;
        public const int GPA = 8;
        public const int Mutation = 9;
        public const int Functional = 10;
        public const int CoAllottee = 11;
        public const int DuesCalculation = 12;
        public const int NDC = 13;
        public const int Other = 21;
        public const int SubmissionOfDocument = 22;
        public const int Query = 25;


        public const int ChangeInDirector = 1;
        public const int ChangeInFirmName = 2;
        public const int ChangeInFirmStatus = 3;
        public const int ChangeInProduct = 4;
    }

    public static class NAStatusId
    {
        public const int Approved = 1;
        public const int Rejected = 2;
        public const int Cancelled = 3;
        public const int Pending = 4;
        public const int InProgress = 5;
        public const int Accepted = 6;
        public const int PendingPayment = 7;
        public const int Initiated = 8;
        public const int Completed = 9;
        public const int Objection = 10;
        public const int Validated = 11;
        public const int CompleteOnSpot = 12;
        public const int Forwarded = 14;
        public const int CancelAfterTransfer = 15;
        public const int Closed = 16;
        public const int InProcess = 17;
        public const int Resubmitted = 18;
        public const int Withdrawn = 19;
        public const int Appointment = 20;

        public const int Paid = 20;
        public const int NotPaid = 21;

        public const int Active = 1;
        public const int Submitted = 1;
        public const int NotActive = 0;
        public const int NotSubmitted = 0;
        public const int Allotted = 1;

        public const int Success = 1;
        public const int Failed = 0;
    }

    public static class QuotaUnits
    {
        public const string fix = "FIX";
        public const int fixVal = 1;
        public const string per = "PER";
        public const int perVal = 2;
    }

    public static class ChallanOptions
    {
        public const string oneTime = "One Time Lease Rent";
        public const string annual = "Annual Lease Rent";
        public const int oneTimeId = 1;
        public const int annualId = 2;
    }

    public static class PaymentType
    {
        public const int Rent = 14;
        public const int RentingCharge = 82;
    }

    public static class RequestStatus
    {
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Cancelled = "Cancelled";
        public const string Pending = "Pending";
        public const string IsProgress = "IsProgress";
        public const string Accepted = "Accepted";
        public const string Completed = "Completed";
        public const string Initiated = "Initiated";
        public const string Validated = "Validated";
    }

    public static class ProcessesMaster
    {
        public const string Checklist = "Checklist";
        public const string LeaseDeed = "Lease Deed";
    }

    public static class GPAOption
    {
        public const string GPASOAYes = "Y";
        public const string GPASOANo = "N";
        public const string AgreementTypeReg = "R";
        public const string AgreementTypeUnReg = "U";
    }

    public static class StatusOption
    {
        public const int Approved = 1;
        public const int Rejected = 2;
        public const int Cancelled = 3;
        public const int Pending = 4;
        public const int InProgress = 5;
    }

    public static class DepartmentOption
    {
        public const int Institutional = 1;
        public const int Commercial = 2;
        public const int Residential = 3;
        public const int Industrial = 4;
        public const int Housing = 5;
        public const int GroupHousing = 6;
    }

    public static class MortPrevLoan
    {
        public const int yes = 1;
        public const int no = 2;
        public const int invalid = 3;
    }

    public static class NDCOptions
    {
        public const string Id_Yes = "Y";
        public const string Id_No = "N";
        public const string Value_Yes = "Yes";
        public const string Value_No = "No";
    }

    public static class Religion
    {
        public const int hindu = 1;
    }

    public static class MaritalStatus
    {
        public const string Married = "married";
        public const string Unmarried = "unmarried";
        public const string Yes = "yes";
        public const string No = "no";
    }

    public static class Status
    {
        public const string Accepted = "Accepted";
        public const string Rejected = "Rejected";
        public const string InProgress = "InProgress";
        public const string Approved = "Approved";
    }

    public static class SubDepartment
    {
        public const string P = "Property";
        public const string A = "Account";
        public const string Property = "Property";
        public const string Account = "Account";
    }

    public static class RegUnReg
    {
        public const string Registered = "Registered";
        public const int RegisteredId = 1;
        public const string UnRegistered = "UnRegistered";
        public const int UnRegisteredId = 2;
        public const string invalid = "invalid";
        public const int invalidId = 3;
    }

    public class CategoryType
    {
        public const string CIC = "CIC";
        public const string GPA = "GPA";
        public const string Merge = "Merge";
        public const string Scheme = "Scheme";
        public const string Lease = "Lease";
        public const string Status = "Status";
        public const string FirmStatus = "Firm Status";
        public const string Director = "Director";
        public const string Cancellation = "Cancellation";
        public const string Application = "Application";
        public const string CompanyType = "CompanyType";
    }

    public enum LetterTypes
    {
        Allotment = 1,
        Transfer = 2,
        TransferPOA = 3,
        Mutation = 4,
        Mortgage = 5,
        Functional = 6,
        Amalgmation = 7,
        GPA = 8,
        Possession = 12,
        Rent = 14,
        CIC = 21,
        LeaseDead = 10,
        Extension = 17,

    }

    public static class ExcelApplicationForm
    {
        public static string[] ApplicationFormHeader =
        {
            "FormNo",
            "FirstName",
            "MiddleName",
            "LastName",
            "Gender",
            "MarritalStatus",
            "FatherHusbandName",
            "MotherName",
            "DateOfBirth",
            "SigningAuthority",
            "RegisteredOffice",
            "CorrespondanceAdd",
            "PermanentAdd",
            //"MobileNumberP1",
            //"MobileNumberP2",
            //"PhoneNumberP1",
            //"PhoneNumberP2",
            //"FaxNumberP1",
            //"FaxNumberP2",
            "MobileNumber",
            "PhoneNumber",
            "FaxNumber",
            "Email",
            "OccupationId",
            "QuotaId",
            "ReligionId",
            "PanNumber",
            "AnnualIncome",

            "PaymentMode",
            "BankId",
            "BranchId",
            "AmountDeposited",
            "DDNo",
            "UTN",
            "IssueBank",
            "IssueDate"
            //"PaymentDate"  
        };
    }

    public static class NewExcelApplicationForm
    {
        public static string[] ApplicationFormHeader =
        {
            "Form No",
            "First Name",
            "Middle Name",
            "Last Name",
            "Gender",
            "Marrital Status",
            "Father/Husband Name",
            "Mother Name",
            "Date Of Birth",
            "Authorized Signatory",
            "Registered Office",
            "Correspondance Addr",
            "Permanent Addr",
            "Mobile Number",
            "Phone Number",
            "Fax Number",
            "Email",
            "Occupation",
            "Quota(GEN/OBC/SC/ST)",
            "Religion",
            "Pan Number",
            "Annual Income",

            "Payment Mode(DD/RTGS)",
            "DD No",
            "UTN(RTGS)",
            "Bank Name",
            "Bank Id",
            "Deposit Amount",
            "Issue Date"
        };
    }

    public static class IndividualForm
    {
        public static string[] FormHeader =
        {
            "Form No",
            "First Name",
            "Middle Name",
            "Last Name",
            "Gender",
            "Marrital Status",
            "Father/Husband Name",
            "Mother Name",
            "Date Of Birth",
            "Correspondance Address",
            "Permanent Address",
            "Mobile Number",
            "Phone Number",
            //"Fax Number",
            "Email",
            "Occupation",
            "Quota(GEN/OBC/SC/ST)",
            "Religion",
            "Pan Number",
            "Annual Income",

            "Payment Mode(DD/RTGS)",
            "DD No",
            "UTN(RTGS)",
            "Bank Name",
            "Bank Id",
            "Deposit Amount",
            "Issue Date"
        };
    }

    public static class CompanyForm
    {
        public static string[] FormHeader =
        {
            "Form No",
            "Company Name",
            "Authorized Signatory",
            "Registered Office",
            "Correspondance Address",
            "Permanent Address",
            "Mobile Number",
            "Phone Number",
            "Fax Number",
            "Email",
            "Pan Number",
            "Annual Income",

            "Payment Mode(DD/RTGS)",
            "DD No",
            "UTN(RTGS)",
            "Bank Name",
            "Bank Id",
            "Deposit Amount",
            "Issue Date"
        };
    }

    public static class OnlineSchemeChallan
    {
        public const string ApplicationFee = "Application Fee";
        public const string ProcessingCharge = "Processing Charge";
        public const string EarnestMoney = "Earnest Money";
        public const string SGST = "SGST(9%)";
        public const string CGST = "SGST(9%)";
    }

    public static class strOnlineApplicationProcess
    {
        public const string MoveToOSD = "1";
        public const string Scrutiny = "2";
        public const string ApprovalCEO = "3";
        public const string Draw = "4";
        public const string AllotmentProcess = "5";
    }

    public enum OnlineApplicationProcess
    {
        MoveToOSD = 1,
        Scrutiny = 2,
        ApprovalCEO = 3,
        Draw = 4,
        AllotmentProcess = 5
    }

    public static class CICDirectorShareType
    {
        public const string Percentage = "Percentage";
        public const string Nos = "Nos";
    }

    #region Define Project  & List keshav 09 Sep 2018
    public static class Application
    {
        public const string PMS = "PMS"; // Property Information Management System [PIMS]
        public const string UserManagement = "UserManagement";
        public const string GeneralAdministration = "General Administration";
        public const string ProjectManagementSystem = "Project Management System";
        public const string FinancialManagementSystem = "Financial Management System";
        public const string PayRollManagementSystem = "PAYROLL";
    }
    public class SelectList
    {
        public int Value { get; set; }
        public string Text { get; set; }
        public string SValue { get; set; }
    }
    #endregion

    #region Constants For NIC Nivesh Mitra

    // For NIC status code
    public static class ServiceStatus
    {
        public const string INPROCESS = "01";
        public const string PENDING = "02";
        public const string VIEWED = "03";
        public const string VERIFIED = "04";
        public const string FORWARDED = "05";
        public const string APPROVED = "06";
        public const string REJECTED = "07";
        public const string QUERY_OBJECTION = "08";
        public const string PUT_FOR_FURTHER_REVIEW = "09";
        public const string SAVE_AS_DRAFT = "10";
        public const string FEE_PAID = "11";
        public const string FEE_PENDING = "12";
        public const string FORM_SUBMITTED = "13";
        public const string FORM_RE_SUBMITTED = "14";
        public const string CERTIFICATE_NO_ISSUED = "15";
    }

    // For NIC status code text
    public static class ServiceStatus_Text
    {
        public const string INPROCESS = "IN PROCESS";
        public const string PENDING = "PENDING";
        public const string VIEWED = "VIEWED";
        public const string VERIFIED = "VERIFIED";
        public const string FORWARDED = "FORWARDED";
        public const string APPROVED = "APPROVED";
        public const string REJECTED = "REJECTED";
        public const string QUERY_OBJECTION = "QUERY OBJECTION";
        public const string PUT_FOR_FURTHER_REVIEW = "PUT FOR FURTHER_REVIEW";
        public const string SAVE_AS_DRAFT = "SAVE AS DRAFT";
        public const string FEE_PAID = "FEE PAID";
        public const string FEE_PENDING = "FEE PENDING";
        public const string FORM_SUBMITTED = "FORM SUBMITTED";
        public const string FORM_RE_SUBMITTED = "FORM RE SUBMITTED";
        public const string CERTIFICATE_NO_ISSUED = "CERTIFICATE NO ISSUED";
    }

    //
    public static class ServiceIds_NIC
    {
        public const string SurrenderCertificate = "SC22006";
        public const string NoDuesCertificate = "SC22007";
        public const string MutationOfLand = "SC22004";
        public const string CalculationOfDues = "SC22008";
    }

    public static class PaymentStatus_NIC
    {
        public const string PAID = "PAID";
        public const string NOTPAID = "NOT PAID";
        public const string UB = "UB";
    }

    #endregion

}
