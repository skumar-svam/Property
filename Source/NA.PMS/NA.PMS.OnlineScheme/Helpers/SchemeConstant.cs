using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.OnlineScheme
{
    public class SchemeConstant
    {
        public const string IndustrialPlots = "IP";
        public const string IndustrialScheme = "Industrial Scheme";
        public const string OpenEnded = "Open End Scheme";
        public const string Transport = "Transport Scheme";
        public const string Institutional = "Institutional Scheme";

        //public const string IndustriaScheme = "IndustrialScheme";
        //public const string TransportScheme = "TransportScheme";
        //public const string IndustriaOpenScheme = "IndustrialOpenScheme";
        //public const string IndustrialPlots = "IP";
        //public const string OpenEnded = "Open End Scheme";
        //public const string Transport = "Transport Scheme";
        //public const string InstitutionalScheme = "Institutional Scheme";
    }

    public static class OSConstant
    {
        public const int Institutional = 1;
        public const int Commercial = 2;
        public const int Residential = 3;
        public const int Industrial = 4;
        public const int Housing = 5;
        public const int GroupHousing = 6;
        public const int Village = 7;

        public const int AdminRoleId = 1;
        public const int SuperAdminRoleId = 1001;
        public const int SvamAdmin = 1133;
        public const int Individual = 1;
        public const int Company = 2;

        public const int General = 5;

        public const int Success = 1;
        public const int Faliure = -1;

        public const int SchemeOpen = 24;
        public const int SchemeInProgress = 25;
        public const int SchemeClosed = 26;

        public const int SchemeFormPayment = 1;
        public const int ReservationMoneyOnlinePayment = 2;
        public const int ReservationMoneyChallanPayment = 3;
        public const int PreviousSchemeChallanPayment = 4;
        public const int NICSchemeFormPayment = 5;
        public const int SchemeBrochurePayment = 6;

        //public const int OnlineApplicationPayment = 1;
        //public const int OnlineOtherPayments = 2;
        //public const int offlineApplicationPayment = 3;
        //public const int offlinePrevoiusChallanApplicationPayment = 4;
        //public const int singleWindowPortalApplicationPayment = 5;
        //public const int BrochureApplicationPayment = 6;

        public const int MoveToOSD = 1;
        public const int Scrutiny = 2;
        public const int ApprovalCEO = 3;
        public const int Draw = 4;
        public const int AllotmentProcess = 5;
    }

    public static class OSStringConstant
    {
        public const string SuperAdmin = "SA";
        public const string Admin = "A";
        public const string User = "U";
        public const string Consultant = "Consultant";
        public const string Company = "Company";
        public const string Individual = "Individual";

        public const string NIC = "NIC";
        public const string Authority = "Authority";
        public const string SCRUTINY = "SCRUTINY";

        public const string DD = "DD";
        public const string RTGS = "RTGS";
        public const string Other = "Other";

        public const string OnlinePayment = "Online";
        public const string OfflinePayment = "Offline";
        public const string OtherPayment = "Other"; 

        public const string NotPaid = "Not Paid";
        public const string Paid = "Paid";

        public const string MoveToOSD = "1";
        public const string Scrutiny = "2";
        public const string ApprovalCEO = "3";
        public const string Draw = "4";
        public const string AllotmentProcess = "5";

        public const string GeneralForm = "General";
        public const string StartupForm = "Startup";
        public const string ExpansionForm = "Expansion";

        public const string Accepted = "Accepted";
        public const string Rejected = "Rejected";
        public const string InProgress = "InProgress";
        public const string Approved = "Approved";

        public const string OnlineScheme = "Online";
    }

    public class OSConfigCategory
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
        public const string Company = "Company";

        public const string GenderType = "GenderType";
        public const string MaritalStatus = "MaritalStatus";
        public const string FormType = "FormType";
        public const string ApplicantType = "ApplicantType";
        public const string PaymentStatus = "Payment Status";

        public const string General = "General";
        public const string Startup = "Startup";
        public const string Expansion = "Expansion";
    }

    public static class OSStatusId
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

    public static class OSReturnTypeId
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

    // For NIC status code
    public static class NICStatusCode
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
    public static class NICTextStatus
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
}
