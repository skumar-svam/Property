
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.NICServices
{
    public static class SWPEncryption
    {
        //encrypt optional parameter in url
        public static string Encode(string encodeVal)
        {
            if (!string.IsNullOrEmpty(encodeVal) && encodeVal != "undefined")
            {
                byte[] encoded = Encoding.UTF8.GetBytes(encodeVal);
                return Convert.ToBase64String(encoded);
            }
            else return encodeVal;
        }
        //decrypt optional parameter in uirl
        public static string Decode(string decodeVal)
        {
            if (!string.IsNullOrEmpty(decodeVal) && decodeVal != "undefined")
            {
                byte[] encoded = Convert.FromBase64String(decodeVal);
                return Encoding.UTF8.GetString(encoded);
            }
            else return decodeVal;
        }
    }

    public class SWPDropdownViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string RoleName { get; set; }
        public string ActionType { get; set; }
        public string ReturnType { get; set; }
        public string FilterType { get; set; }
        public string ChallanRefId { get; set; }
        public string TransactionId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> FilterTypeId { get; set; }
        public Nullable<int> ActionTypeId { get; set; }
        public Nullable<int> ReturnTypeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> PropertTypeId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<long> ReceiptId { get; set; }
        public Nullable<long> RefId { get; set; }
        public Nullable<DateTime> FilterDate { get; set; }
    }

    public static class SWPSchemeType
    {
        public const string IndustrialPlots = "IP";
        public const string IndustrialScheme = "Industrial Scheme";
        public const string OpenEnded = "Open End Scheme";
        public const string Transport = "Transport Scheme";
        public const string Institutional = "Institutional Scheme";

        public const string IndustrialSchemeType = "IndustrialScheme";
        public const string TransportSchemeType = "TransportScheme";
        public const string IndustrialOpenScheme = "IndustrialOpenScheme";
    }

    public static class SWPConstant
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
        public const int SWPApplicationPayment = 5;

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

        public const int MoveToOSD = 1;
        public const int Scrutiny = 2;
        public const int ApprovalCEO = 3;
        public const int Draw = 4;
        public const int AllotmentProcess = 5;

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

        public const string HttpMethodPOST = "POST";
        public const string HttpMethodGET = "GET";

        public const string NICSurrenderCertificate = "SC22006";
        public const string NICNoDuesCertificate = "SC22007";
        public const string NICMutationOfLand = "SC22004";
        public const string NICDuesCalculation = "SC22008";

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

    public static class SWPReturnTypeId
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

    public static class SWPStatusId
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

    public static class SWPStatus
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

        public static class Wording
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

        public static class Form
        {
            public const string Accepted = "Accepted";
            public const string Rejected = "Rejected";
            public const string InProgress = "InProgress";
            public const string Approved = "Approved";
        }

        public static class Payment
        {
            public const string NotPaid = "Not Paid";
            public const string Paid = "Paid";

            public const string C_PAID = "PAID";
            public const string C_NOTPAID = "NOT PAID";
            public const string UB = "UB";
        }
    }
    
    public static class SWPApplication
    {
        public static int GenerateOTP()
        {
            Random random = new Random();
            int maxValue = 999999;
            int otp = random.Next(maxValue);
            return otp;
        }

        public static string GeneratePasswordForScheme()
        {
            Random random = new Random();
            int maxValue = 999999;
            string Key = "NS";
            int Pass = random.Next(maxValue);
            return Key + Pass;
            //return "Noida";
        }

        //public static string GenerateLetter(int rid, int templateId, int departmentId, int UserID)
        //{
        //    string constring = ConfigurationManager.ConnectionStrings["PIMSSqlConnection"].ConnectionString;
        //    using (SqlConnection con = new SqlConnection(constring))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("Sp_LatterPrintTemp", con))
        //        {
        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@registrationId", rid.ToString());
        //            cmd.Parameters.Add("@templateId", templateId);
        //            cmd.Parameters.Add("@departmentId", departmentId);
        //            cmd.Parameters.Add("@userId", UserID.ToString());
        //            cmd.Parameters.Add("@CommaString", SqlDbType.VarChar, 8000);
        //            cmd.Parameters["@CommaString"].Direction = ParameterDirection.Output;
        //            con.Open();
        //            cmd.ExecuteNonQuery();
        //            con.Close();
        //            return cmd.Parameters["@CommaString"].Value.ToString();
        //        }
        //    }
        //}

        public static string GenerateSHA512HashCode(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }

        public static string GenerateTransactionId()
        {
            Random rnd = new Random();
            string strHash = GenerateSHA512HashCode(rnd.ToString() + DateTime.Now);
            string txnid1 = strHash.ToString().Substring(0, 20);
            return txnid1;
        }

        //public static RoleMenuKeyModel SetRolePrmision(int menuKey)
        //{
        //    RoleMenuKeyModel objRoleMenuKey = new RoleMenuKeyModel();
        //    if (menuKey != 0)
        //    {
        //        var loginUser = (CurrentUserDetail)HttpContext.Current.Session["CurrentUser"];
        //        if (loginUser != null)
        //        {
        //            foreach (var Role in loginUser.MenuMaster)
        //            {
        //                if (Role != null && Role.MenuId == menuKey)
        //                {
        //                    objRoleMenuKey.EditMenuVal = Role.IsUpdate;
        //                    objRoleMenuKey.AddMenuVal = Role.IsWrite;
        //                    objRoleMenuKey.DeleteMenuVal = Role.Isdelete;
        //                    objRoleMenuKey.ReadOnlyMenu = Role.IsRead;
        //                }
        //            }
        //        }
        //    }
        //    return objRoleMenuKey;
        //}

        //Digit to Words by Mankaran
       
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public static String ToMD5HashForPasswordPIS(this String plainText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(plainText));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

        public static string CreatePassword()
        {
            const string PasswordCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int PasswordLength = 8;
            StringBuilder password = new StringBuilder();
            Random random = new Random();
            while (0 < PasswordLength--)
            {
                password.Append(PasswordCharacters[random.Next(PasswordCharacters.Length)]);
            }
            return password.ToString();
        }

        //send sms
        #region

        public static void SMSSend(string mobileNo, string msg)
        {
            WebClient client = new WebClient();
            string baseurl = ConfigurationManager.AppSettings["SMSsend"].ToString() + ConfigurationManager.AppSettings["SMSUsername"].ToString() + "&password=" + ConfigurationManager.AppSettings["SMSPassword"].ToString() + "&sendername=" + ConfigurationManager.AppSettings["SMSSenderID"].ToString() + "&mobileno=" + mobileNo + "&message=" + msg;
            Stream data = client.OpenRead(baseurl);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            data.Close();
            reader.Close();
        }

        public static int SendSMS(string mobileNo, string msg)
        {
            int flag = 0;
            try
            {
                WebClient client = new WebClient();
                string baseurl = ConfigurationManager.AppSettings["SMSsend"].ToString() + ConfigurationManager.AppSettings["SMSUsername"].ToString() + "&password=" + ConfigurationManager.AppSettings["SMSPassword"].ToString() + "&sendername=" + ConfigurationManager.AppSettings["SMSSenderID"].ToString() + "&mobileno=" + mobileNo + "&message=" + msg;
                Stream data = client.OpenRead(baseurl);
                StreamReader reader = new StreamReader(data);
                string s = reader.ReadToEnd();
                data.Close();
                reader.Close();
                flag = 1;
            }
            catch (Exception e)
            {

            }
            return flag;
        }

        #endregion

        // send email
        #region
        public static void SendEmail(string toAddress, string subject, string body)
        {
            Task.Factory.StartNew(() => SendMail(toAddress, subject, body, false), TaskCreationOptions.LongRunning);
        }

        //private static void SendMail(string toAddress, string subject, string body, bool priority)
        //{
        //    string serverName = ConfigurationManager.AppSettings["SmtpGmailServerName"];
        //    int port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpGmailTLSPort"]);
        //    string userName = "samestien@gmail.com"; // ConfigurationManager.AppSettings["SmtpGmailUserName"];
        //    string password = "samestien@delhi"; // ConfigurationManager.AppSettings["SmtpGmailPassword"];
        //    SmtpClient client = new SmtpClient(serverName, port);
        //    try
        //    {
        //        //MailAddress fromAddress = new MailAddress(ConfigurationManager.AppSettings["SmtpGmailFromAddress"]);
        //        MailAddress fromAddress = new MailAddress("samestien@gmail.com");
        //        MailAddress toAddressNew = new MailAddress(toAddress);

        //        var message = new MailMessage(fromAddress, toAddressNew);
        //        message.Subject = subject;
        //        message.Body = body;
        //        message.IsBodyHtml = true;
        //        message.HeadersEncoding = Encoding.UTF8;
        //        message.SubjectEncoding = Encoding.UTF8;
        //        message.BodyEncoding = Encoding.UTF8;
        //        if (priority) message.Priority = MailPriority.High;

        //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        //client.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpSsl"]);
        //        client.EnableSsl = true;
        //        client.UseDefaultCredentials = false;

        //        NetworkCredential smtpUserInfo = new NetworkCredential(userName, password);
        //        client.Credentials = smtpUserInfo;
        //        ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        //        { return true; };
        //        client.Send(message);
        //        client.Dispose();
        //        message.Dispose();
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //    finally
        //    {
        //        client.Dispose();
        //        //message.Dispose();
        //    }
        //}

        private static void SendMail(string toAddress, string subject, string body, bool priority)
        {
            string serverName = ConfigurationManager.AppSettings["SmtpServerName"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            string userName = ConfigurationManager.AppSettings["SmtpUserName"];
            string password = ConfigurationManager.AppSettings["SmtpPassword"];
            SmtpClient client = new SmtpClient(serverName, port);
            try
            {
                MailAddress fromAddress = new MailAddress(ConfigurationManager.AppSettings["SmtpFromAddress"]);
                MailAddress toAddressNew = new MailAddress(toAddress);

                var message = new MailMessage(fromAddress, toAddressNew);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                message.HeadersEncoding = Encoding.UTF8;
                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = Encoding.UTF8;
                if (priority) message.Priority = MailPriority.High;

                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpSsl"]);
                client.UseDefaultCredentials = false;

                NetworkCredential smtpUserInfo = new NetworkCredential(userName, password);
                client.Credentials = smtpUserInfo;
                ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };
                client.Send(message);
                client.Dispose();
                message.Dispose();
            }
            catch (Exception e)
            {

            }
            finally
            {
                client.Dispose();
                //message.Dispose();
            }
        }


        /// <summary>
        /// Send an e-mail message with optional attachments.
        /// </summary>
        /// <example>
        /// <code>
        /// ArrayList AttachMe = new ArrayList();
        /// AttachMe.Add("/ftp/file1.pdf");
        /// AttachMe.Add("/ftp/file2.pdf");
        /// 
        /// string result = ApplicationHelper.SendMail(
        ///		"from@domain.com",
        ///		"skumar",
        ///		"recipient1@domain.com,recipient2@domain.com",
        ///		"Message subject here",
        ///		"This is the message body.",
        ///		"plaintext",
        ///		AttachMe);
        /// </code>
        /// </example>
        /// <param name="senderAddress">From address (i.e. skumar@svam.com).</param>
        /// <param name="senderName">From name (i.e. Shatrughna).</param>
        /// <param name="recipient">To address(es), separated by commas.</param>
        /// <param name="subject">Message subject.</param>
        /// <param name="body">Message body.</param>
        /// <param name="BodyFormat">"html" or "plaintext".</param>
        /// <param name="attachments">ArrayList object with virtual paths to files.</param>
        /// <returns>"SUCCESS", or an error message.</returns>
        public static string SendMail(string senderAddress, string senderName, string recipient, string subject, string body, string BodyFormat, ArrayList attachments)
        {
            return SendMail(senderAddress, senderName, recipient, "", "", subject, body, BodyFormat, attachments);
        }
        public static string SendMail(string senderAddress, string senderName, string recipient, string ccList, string bccList, string subject, string body, string BodyFormat, ArrayList attachments)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();

            mail.From = string.IsNullOrEmpty(senderName) ? new MailAddress(senderAddress) : new MailAddress(senderAddress, senderName);
            mail.Subject = subject;
            mail.Body = body;

            foreach (string address in recipient.Split(';'))
            {
                if (address != null && address.Trim() != "")
                    mail.To.Add(address);
            }

            if (!String.IsNullOrEmpty(ccList))
                foreach (string address in ccList.Split(';'))
                {
                    if (address != null && address.Trim() != "")
                        mail.CC.Add(address);
                }

            if (!String.IsNullOrEmpty(bccList))
                foreach (string address in bccList.Split(';')) mail.Bcc.Add(address);

            string result = "SUCCESS";

            try
            {
                mail.IsBodyHtml = (BodyFormat.ToLower() == "html" ? true : false);

                if (attachments != null)
                {
                    if (attachments.Count > 0)
                    {
                        foreach (string attachment in attachments)
                        {
                            //System.Net.Mail.Attachment data = Path.IsPathRooted(attachment) == true ? new System.Net.Mail.Attachment(attachment, MediaTypeNames.Application.Octet) : new System.Net.Mail.Attachment(HttpContext.Current.Server.MapPath(attachment), MediaTypeNames.Application.Octet);
                            System.Net.Mail.Attachment data;
                            if (Path.IsPathRooted(attachment) == true)
                            {
                                data = new System.Net.Mail.Attachment(attachment, MediaTypeNames.Application.Octet);
                            }
                            else
                            {
                                data = new System.Net.Mail.Attachment(HttpContext.Current.Server.MapPath(attachment), MediaTypeNames.Application.Octet);
                            }
                            mail.Attachments.Add(data);
                        }
                    }
                }

                SmtpClient smtp = new SmtpClient();
                smtp.Send(mail);
                mail.Dispose();
            }

            catch (Exception err)
            {
                result = err.ToString();
            }

            return result;
        }

        //public static string SendEmailWithAttachedFiles(string senderAddress, string senderName, string recipient, string ccList, string bccList, string subject, string body, string BodyFormat, ArrayList attachments)
        //{
        //    string serverName = ConfigurationManager.AppSettings["SmtpServerName"];
        //    int port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
        //    string userName = ConfigurationManager.AppSettings["SmtpUserName"];
        //    string password = ConfigurationManager.AppSettings["SmtpPassword"];
        //    //SmtpClient client = new SmtpClient(serverName, port);
        //    SmtpClient client = new SmtpClient(serverName);
        //    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
        //    mail.From = string.IsNullOrEmpty(senderName) ? new MailAddress(senderAddress) : new MailAddress(senderAddress, senderName);
        //    mail.Subject = subject;
        //    mail.Body = body;

        //    foreach (string address in recipient.Split(';'))
        //    {
        //        if (address != null && address.Trim() != "")
        //            mail.To.Add(address);
        //    }

        //    if (!String.IsNullOrEmpty(ccList))
        //        foreach (string address in ccList.Split(';'))
        //        {
        //            if (address != null && address.Trim() != "")
        //                mail.CC.Add(address);
        //        }

        //    if (!String.IsNullOrEmpty(bccList))
        //        foreach (string address in bccList.Split(';')) mail.Bcc.Add(address);

        //    string result = "SUCCESS";

        //    try
        //    {
        //        mail.IsBodyHtml = (BodyFormat.ToLower() == "html" ? true : false);

        //        if (attachments != null)
        //        {
        //            if (attachments.Count > 0)
        //            {
        //                //foreach (string attachment in attachments)
        //                //{
        //                //    //System.Net.Mail.Attachment data = Path.IsPathRooted(attachment) == true ? new System.Net.Mail.Attachment(attachment, MediaTypeNames.Application.Octet) : new System.Net.Mail.Attachment(HttpContext.Current.Server.MapPath(attachment), MediaTypeNames.Application.Octet);
        //                //    System.Net.Mail.Attachment data;
        //                //    if (Path.IsPathRooted(attachment) == true)
        //                //    {
        //                //        data = new System.Net.Mail.Attachment(attachment, MediaTypeNames.Application.Octet);
        //                //    }
        //                //    else
        //                //    {
        //                //        data = new System.Net.Mail.Attachment(HttpContext.Current.Server.MapPath(attachment), MediaTypeNames.Application.Octet);
        //                //    }
        //                //    mail.Attachments.Add(data);
        //                //}
        //                foreach (var attachment in attachments)
        //                {
        //                    //System.Net.Mail.Attachment data = Path.IsPathRooted(attachment) == true ? new System.Net.Mail.Attachment(attachment, MediaTypeNames.Application.Octet) : new System.Net.Mail.Attachment(HttpContext.Current.Server.MapPath(attachment), MediaTypeNames.Application.Octet);

        //                    System.Net.Mail.Attachment data;
        //                    ExcelPackage ExcelPkg = (ExcelPackage)attachment;
        //                    if (Path.IsPathRooted(ExcelPkg.File.Name) == true)
        //                    {
        //                        data = new System.Net.Mail.Attachment(ExcelPkg.File.OpenRead(), MediaTypeNames.Application.Octet);
        //                    }
        //                    else
        //                    {
        //                        string filepath = ConfigurationManager.AppSettings["UploadFilePath"].ToString() + "FunctionalReportPackage.xlsx";
        //                        //string filepath = "D:\\FunctionalReportPackage.xlsx";
        //                        data = new System.Net.Mail.Attachment(HttpContext.Current.Server.MapPath(filepath), MediaTypeNames.Application.Octet);
        //                        //data = new System.Net.Mail.Attachment(HttpContext.Current.Server.MapPath(ExcelPkg.File.DirectoryName + ExcelPkg.File.Name), MediaTypeNames.Application.Octet);
        //                    }
        //                    mail.Attachments.Add(data);
        //                }
        //            }
        //        }
        //        client.Credentials = new NetworkCredential(userName, password);
        //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        client.Send(mail);
        //        mail.Dispose();
        //    }

        //    catch (Exception err)
        //    {
        //        result = err.ToString();
        //    }

        //    return result;
        //}

        #endregion

        // convert number into words
        #region
        public static string SingleDigitNumber(int number)
        {
            string result = string.Empty;
            if (number == 1) result = "One";
            if (number == 2) result = "Two";
            if (number == 3) result = "Three";
            if (number == 4) result = "Four";
            if (number == 5) result = "Five";
            if (number == 6) result = "Six";
            if (number == 7) result = "Seven";
            if (number == 8) result = "Eight";
            if (number == 9) result = "Nine";
            if (number == 0) result = string.Empty;
            return result;
        }

        public static string TwoDigitNumber(int number)
        {
            string result = string.Empty;
            if (number >= 0 && number <= 9)
            {
                result = SingleDigitNumber(number);
            }
            else if (number > 9 && number <= 20)
            {
                if (number == 10) result = "Ten";
                if (number == 11) result = "Eleven";
                if (number == 12) result = "Twelve";
                if (number == 13) result = "Thirteen";
                if (number == 14) result = "Forteen";
                if (number == 15) result = "Fifteen";
                if (number == 16) result = "Sixteen";
                if (number == 17) result = "Seventeen";
                if (number == 18) result = "Eighteen";
                if (number == 19) result = "Nineteen";
                if (number == 20) result = "Twenty";
            }
            else if (number > 20 && number <= 99)
            {
                if (number / 10 == 2) result = "Twenty" + " " + SingleDigitNumber(number % 10);
                if (number / 10 == 3) result = "Thirty" + " " + SingleDigitNumber(number % 10);
                if (number / 10 == 4) result = "Forty" + " " + SingleDigitNumber(number % 10);
                if (number / 10 == 5) result = "Fifty" + " " + SingleDigitNumber(number % 10);
                if (number / 10 == 6) result = "Sixty" + " " + SingleDigitNumber(number % 10);
                if (number / 10 == 7) result = "Seventy" + " " + SingleDigitNumber(number % 10);
                if (number / 10 == 8) result = "Eighty" + " " + SingleDigitNumber(number % 10);
                if (number / 10 == 9) result = "Ninety" + " " + SingleDigitNumber(number % 10);
            }
            return result;
        }

        public static string ThreeDigitNumber(int number)
        {
            string result = string.Empty;
            if (number > 0 && number <= 99)
            {
                result = TwoDigitNumber((int)number % 100);
            }
            else if (number >= 100 && number <= 999)
            {
                int hundredNumber = (int)(number / 100);
                result = SingleDigitNumber(hundredNumber) + " Hundred " + TwoDigitNumber((int)number % 100);
            }
            return result;
        }

        public static string FiveDigitNumber(int number)
        {
            string result = string.Empty;
            if (number > 0 && number <= 99)
            {
                result = TwoDigitNumber((int)number % 100);
            }
            else if (number >= 100 && number <= 999)
            {
                int hundredPlace = (int)(number / 100);
                result = SingleDigitNumber(hundredPlace) + " Hundred " + TwoDigitNumber((int)number % 100);
            }
            else if (number >= 1000 && number <= 99999)
            {
                int thousandPlace = (int)(number / 1000);
                int hundredPlace = ((int)(number % 1000) / 100);
                result = (thousandPlace == 0 ? string.Empty : (TwoDigitNumber(thousandPlace) + " Thousand ")) + (hundredPlace == 0 ? string.Empty : (SingleDigitNumber(hundredPlace) + " Hundred ")) + TwoDigitNumber((int)number % 100);
            }
            return result;
        }

        public static string ConvertNumberIntoWords(Int64 number)
        {
            string result = string.Empty;
            string numberString = number > 0 ? number.ToString() : "Zero";
            if (numberString != "Zero")
            {
                int numberLength = numberString.Length;
                if (numberLength <= 3)
                {
                    result = ThreeDigitNumber((int)number);
                }
                else if (numberLength > 3 && numberLength <= 5)
                {
                    int thousandplace = (int)number / 1000;
                    result = TwoDigitNumber(thousandplace) + " Thousand " + ThreeDigitNumber((int)number % 1000);
                }
                else if (numberLength > 5 && numberLength <= 7)
                {
                    int lakhplace = (int)number / 100000;
                    int thousandplace = ((int)number % 100000) / 1000;
                    result = TwoDigitNumber(lakhplace) + " Lakh " + (thousandplace == 0 ? string.Empty : (TwoDigitNumber(thousandplace) + " Thousand ")) + ThreeDigitNumber(((int)number % 100000) % 1000);
                }
                else if (numberLength > 7 && numberLength <= 10)
                {
                    int croreplace = (int)(number / 10000000);//for crore 
                    int lakhplace = (int)(number % 10000000) / 100000; //for lakh
                    int thousandplace = (int)(number % 100000) / 1000; // for thousand
                    result = (croreplace == 0 ? string.Empty : (ThreeDigitNumber(croreplace) + " Crore ")) + (lakhplace == 0 ? string.Empty : (TwoDigitNumber(lakhplace) + " Lakh ")) + (thousandplace == 0 ? string.Empty : (TwoDigitNumber(thousandplace) + " Thousand ")) + ThreeDigitNumber(((int)number % 100000) % 1000);
                }
                else if (numberLength > 10 && numberLength <= 12)
                {
                    int croreplace = (int)(number / 10000000);//for crore 
                    int lakhplace = (int)(number % 10000000) / 100000; //for lakh
                    int thousandplace = (int)(number % 100000) / 1000; // for thousand
                    result = (croreplace == 0 ? string.Empty : (FiveDigitNumber(croreplace) + " Crore ")) + (lakhplace == 0 ? string.Empty : (TwoDigitNumber(lakhplace) + " Lakh ")) + (thousandplace == 0 ? string.Empty : (TwoDigitNumber(thousandplace) + " Thousand ")) + ThreeDigitNumber((int)((number % 100000) % 1000));
                }
            }
            return result;
        }

        #endregion

        
    }

    public class SWPResultMessage
    {
        public SWPResultMessage()
        {
            this.ResultTypeList = new List<SWPResultMessage>();
        }
        public int ReturnType { get; set; }
        public string Message { get; set; }
        public int PrimaryKey { get; set; }
        public List<SWPResultMessage> ResultTypeList { get; set; }
    }

    public class SWPCategoryType
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

    public static class SWPDepartment
    {
        public const int Institutional = 1;
        public const int Commercial = 2;
        public const int Residential = 3;
        public const int Industrial = 4;
        public const int Housing = 5;
        public const int GroupHousing = 6;
        public const int Village = 7;

        public static class SubDepartment
        {
            public const string P = "Property";
            public const string A = "Account";
            public const string Property = "Property";
            public const string Account = "Account";
        }

        public static class Role
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
    }

    public static class SWPEnum
    {
        public enum MortgageType
        {
            [Description("Collateral")]
            Collateral = 1,
            [Description("Normal")]
            Normal = 2
        }

        public enum MortgageStatus
        {
            [Description("Yes")]
            Yes = 1,
            [Description("No")]
            No = 2
        }

        public enum ActiveStatus
        {
            No = 0,
            Yes = 1
        }

        public enum MaritalStatus
        {
            [Description("Married")]
            married,
            [Description("UnMarried")]
            unmarried
        }

        public enum FormProcess
        {
            MoveToOSD = 1,
            Scrutiny = 2,
            ApprovalCEO = 3,
            Draw = 4,
            AllotmentProcess = 5
        }
    }

    public static class SWPService
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
}
