using NA.PMS.Model;
using NA.PMS.Model.CommonModel;
using NA.PMS.Web.Models;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
//using System.Web.Mail;

namespace NA.PMS.Common
{
    public static class ApplicationHelper
    {
        public static int GenerateOTP()
        {
            Random random = new Random();
            int maxValue = 999999;
            int otp = random.Next(maxValue);
            return otp;
        }

        public static string GeneratePassWordForScheme()
        {
            Random random = new Random();
            int maxValue = 999999;
            string Key = "NS";
            int Pass = random.Next(maxValue);
            return Key + Pass;
            //return "Noida";
        }

        public static string GenerateLetter(int rid, int templateId, int departmentId, int UserID)
        {
            string constring = ConfigurationManager.ConnectionStrings["PIMSSqlConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_LatterPrintTemp", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@registrationId", rid.ToString());
                    cmd.Parameters.Add("@templateId", templateId);
                    cmd.Parameters.Add("@departmentId", departmentId);
                    cmd.Parameters.Add("@userId", UserID.ToString());
                    cmd.Parameters.Add("@CommaString", SqlDbType.VarChar, 8000);
                    cmd.Parameters["@CommaString"].Direction = ParameterDirection.Output;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    return cmd.Parameters["@CommaString"].Value.ToString();
                }
            }
        }

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

        public static RoleMenuKeyModel SetRolePrmision(int menuKey)
        {
            RoleMenuKeyModel objRoleMenuKey = new RoleMenuKeyModel();
            if (menuKey != 0)
            {
                var loginUser = (CurrentUserDetail)HttpContext.Current.Session["CurrentUser"];
                if (loginUser != null)
                {
                    foreach (var Role in loginUser.MenuMaster)
                    {
                        if (Role != null && Role.MenuId == menuKey)
                        {
                            objRoleMenuKey.EditMenuVal = Role.IsUpdate;
                            objRoleMenuKey.AddMenuVal = Role.IsWrite;
                            objRoleMenuKey.DeleteMenuVal = Role.Isdelete;
                            objRoleMenuKey.ReadOnlyMenu = Role.IsRead;
                        }
                    }
                }
            }
            return objRoleMenuKey;
        }

        //Digit to Words by Mankaran
        public static string Rupees(Int64 rup)
        {
            string result = "";
            Int64 res;
            if ((rup / 10000000) > 0)
            {
                res = rup / 10000000;
                rup = rup % 10000000;
                result = result + ' ' + RupeesToWords(res) + " Crore";
            }
            if ((rup / 100000) > 0)
            {
                res = rup / 100000;
                rup = rup % 100000;
                result = result + ' ' + RupeesToWords(res) + " Lakh";
            }
            if ((rup / 1000) > 0)
            {
                res = rup / 1000;
                rup = rup % 1000;
                result = result + ' ' + RupeesToWords(res) + " Thousand";
            }
            if ((rup / 100) > 0)
            {
                res = rup / 100;
                rup = rup % 100;
                result = result + ' ' + RupeesToWords(res) + " Hundred";
            }
            if ((rup % 10) >= 0)
            {
                res = rup % 100;
                result = result + " " + RupeesToWords(res);
            }
            result = result + ' ' + " Rupees only";
            return result;
        }
        //Digit to Words by Mankaran
        public static string RupeesToWords(Int64 rup)
        {
            string result = "";
            if ((rup >= 1) && (rup <= 10))
            {
                if ((rup % 10) == 1) result = "One";
                if ((rup % 10) == 2) result = "Two";
                if ((rup % 10) == 3) result = "Three";
                if ((rup % 10) == 4) result = "Four";
                if ((rup % 10) == 5) result = "Five";
                if ((rup % 10) == 6) result = "Six";
                if ((rup % 10) == 7) result = "Seven";
                if ((rup % 10) == 8) result = "Eight";
                if ((rup % 10) == 9) result = "Nine";
                if ((rup % 10) == 0) result = "Ten";
            }
            if (rup > 9 && rup < 20)
            {
                if (rup == 11) result = "Eleven";
                if (rup == 12) result = "Twelve";
                if (rup == 13) result = "Thirteen";
                if (rup == 14) result = "Forteen";
                if (rup == 15) result = "Fifteen";
                if (rup == 16) result = "Sixteen";
                if (rup == 17) result = "Seventeen";
                if (rup == 18) result = "Eighteen";
                if (rup == 19) result = "Nineteen";
            }
            if (rup > 20 && (rup / 10) == 2 && (rup % 10) == 0) result = "Twenty";
            if (rup > 20 && (rup / 10) == 3 && (rup % 10) == 0) result = "Thirty";
            if (rup > 20 && (rup / 10) == 4 && (rup % 10) == 0) result = "Forty";
            if (rup > 20 && (rup / 10) == 5 && (rup % 10) == 0) result = "Fifty";
            if (rup > 20 && (rup / 10) == 6 && (rup % 10) == 0) result = "Sixty";
            if (rup > 20 && (rup / 10) == 7 && (rup % 10) == 0) result = "Seventy";
            if (rup > 20 && (rup / 10) == 8 && (rup % 10) == 0) result = "Eighty";
            if (rup > 20 && (rup / 10) == 9 && (rup % 10) == 0) result = "Ninty";

            if (rup > 20 && (rup / 10) == 2 && (rup % 10) != 0)
            {
                if ((rup % 10) == 1) result = "Twenty One";
                if ((rup % 10) == 2) result = "Twenty Two";
                if ((rup % 10) == 3) result = "Twenty Three";
                if ((rup % 10) == 4) result = "Twenty Four";
                if ((rup % 10) == 5) result = "Twenty Five";
                if ((rup % 10) == 6) result = "Twenty Six";
                if ((rup % 10) == 7) result = "Twenty Seven";
                if ((rup % 10) == 8) result = "Twenty Eight";
                if ((rup % 10) == 9) result = "Twenty Nine";
            }
            if (rup > 20 && (rup / 10) == 3 && (rup % 10) != 0)
            {
                if ((rup % 10) == 1) result = "Thirty One";
                if ((rup % 10) == 2) result = "Thirty Two";
                if ((rup % 10) == 3) result = "Thirty Three";
                if ((rup % 10) == 4) result = "Thirty Four";
                if ((rup % 10) == 5) result = "Thirty Five";
                if ((rup % 10) == 6) result = "Thirty Six";
                if ((rup % 10) == 7) result = "Thirty Seven";
                if ((rup % 10) == 8) result = "Thirty Eight";
                if ((rup % 10) == 9) result = "Thirty Nine";
            }
            if (rup > 20 && (rup / 10) == 4 && (rup % 10) != 0)
            {
                if ((rup % 10) == 1) result = "Forty One";
                if ((rup % 10) == 2) result = "Forty Two";
                if ((rup % 10) == 3) result = "Forty Three";
                if ((rup % 10) == 4) result = "Forty Four";
                if ((rup % 10) == 5) result = "Forty Five";
                if ((rup % 10) == 6) result = "Forty Six";
                if ((rup % 10) == 7) result = "Forty Seven";
                if ((rup % 10) == 8) result = "Forty Eight";
                if ((rup % 10) == 9) result = "Forty Nine";
            }
            if (rup > 20 && (rup / 10) == 5 && (rup % 10) != 0)
            {
                if ((rup % 10) == 1) result = "Fifty One";
                if ((rup % 10) == 2) result = "Fifty Two";
                if ((rup % 10) == 3) result = "Fifty Three";
                if ((rup % 10) == 4) result = "Fifty Four";
                if ((rup % 10) == 5) result = "Fifty Five";
                if ((rup % 10) == 6) result = "Fifty Six";
                if ((rup % 10) == 7) result = "Fifty Seven";
                if ((rup % 10) == 8) result = "Fifty Eight";
                if ((rup % 10) == 9) result = "Fifty Nine";
            }
            if (rup > 20 && (rup / 10) == 6 && (rup % 10) != 0)
            {
                if ((rup % 10) == 1) result = "Sixty One";
                if ((rup % 10) == 2) result = "Sixty Two";
                if ((rup % 10) == 3) result = "Sixty Three";
                if ((rup % 10) == 4) result = "Sixty Four";
                if ((rup % 10) == 5) result = "Sixty Five";
                if ((rup % 10) == 6) result = "Sixty Six";
                if ((rup % 10) == 7) result = "Sixty Seven";
                if ((rup % 10) == 8) result = "Sixty Eight";
                if ((rup % 10) == 9) result = "Sixty Nine";
            }
            if (rup > 20 && (rup / 10) == 7 && (rup % 10) != 0)
            {
                if ((rup % 10) == 1) result = "Seventy One";
                if ((rup % 10) == 2) result = "Seventy Two";
                if ((rup % 10) == 3) result = "Seventy Three";
                if ((rup % 10) == 4) result = "Seventy Four";
                if ((rup % 10) == 5) result = "Seventy Five";
                if ((rup % 10) == 6) result = "Seventy Six";
                if ((rup % 10) == 7) result = "Seventy Seven";
                if ((rup % 10) == 8) result = "Seventy Eight";
                if ((rup % 10) == 9) result = "Seventy Nine";
            }
            if (rup > 20 && (rup / 10) == 8 && (rup % 10) != 0)
            {
                if ((rup % 10) == 1) result = "Eighty One";
                if ((rup % 10) == 2) result = "Eighty Two";
                if ((rup % 10) == 3) result = "Eighty Three";
                if ((rup % 10) == 4) result = "Eighty Four";
                if ((rup % 10) == 5) result = "Eighty Five";
                if ((rup % 10) == 6) result = "Eighty Six";
                if ((rup % 10) == 7) result = "Eighty Seven";
                if ((rup % 10) == 8) result = "Eighty Eight";
                if ((rup % 10) == 9) result = "Eighty Nine";
            }
            if (rup > 20 && (rup / 10) == 9 && (rup % 10) != 0)
            {
                if ((rup % 10) == 1) result = "Ninty One";
                if ((rup % 10) == 2) result = "Ninty Two";
                if ((rup % 10) == 3) result = "Ninty Three";
                if ((rup % 10) == 4) result = "Ninty Four";
                if ((rup % 10) == 5) result = "Ninty Five";
                if ((rup % 10) == 6) result = "Ninty Six";
                if ((rup % 10) == 7) result = "Ninty Seven";
                if ((rup % 10) == 8) result = "Ninty Eight";
                if ((rup % 10) == 9) result = "Ninty Nine";
            }
            return result;
        }

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
            //string baseurl = ConfigurationManager.AppSettings["SMSsend"].ToString() + ConfigurationManager.AppSettings["SMSUsername"].ToString() + "&password=" + ConfigurationManager.AppSettings["SMSPassword"].ToString() + "&sendername=" + ConfigurationManager.AppSettings["SMSSenderID"].ToString() + "&mobileno=" + mobileNo + "&message=" + msg;
            string baseurl = ConfigurationManager.AppSettings["SMSApiUrl"].ToString() + "ApiKey=" + ConfigurationManager.AppSettings["SMSApiKey"].ToString() + "&ClientId=" + ConfigurationManager.AppSettings["SMSClientId"].ToString() + "&SenderId=" + ConfigurationManager.AppSettings["SMSSenderId"].ToString() + "&Message=" + msg + "&MobileNumbers=91" + mobileNo + "&Is_Unicode=" + ConfigurationManager.AppSettings["SMSIsUnicode"].ToString() + "&Is_Flash=" + ConfigurationManager.AppSettings["SMSIsFlash"].ToString();
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
                //string baseurl = ConfigurationManager.AppSettings["SMSsend"].ToString() + ConfigurationManager.AppSettings["SMSUsername"].ToString() + "&password=" + ConfigurationManager.AppSettings["SMSPassword"].ToString() + "&sendername=" + ConfigurationManager.AppSettings["SMSSenderID"].ToString() + "&mobileno=" + mobileNo + "&message=" + msg;
                string baseurl = ConfigurationManager.AppSettings["SMSApiUrl"].ToString() + "ApiKey=" + ConfigurationManager.AppSettings["SMSApiKey"].ToString() + "&ClientId=" + ConfigurationManager.AppSettings["SMSClientId"].ToString() + "&SenderId=" + ConfigurationManager.AppSettings["SMSSenderId"].ToString() + "&Message=" + msg + "&MobileNumbers=91" + mobileNo + "&Is_Unicode=" + ConfigurationManager.AppSettings["SMSIsUnicode"].ToString() + "&Is_Flash=" + ConfigurationManager.AppSettings["SMSIsFlash"].ToString();
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

        public static string SendEmailWithAttachedFiles(string senderAddress, string senderName, string recipient, string ccList, string bccList, string subject, string body, string BodyFormat, ArrayList attachments)
        {
            string serverName = ConfigurationManager.AppSettings["SmtpServerName"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            string userName = ConfigurationManager.AppSettings["SmtpUserName"];
            string password = ConfigurationManager.AppSettings["SmtpPassword"];
            //SmtpClient client = new SmtpClient(serverName, port);
            SmtpClient client = new SmtpClient(serverName);
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
                        //foreach (string attachment in attachments)
                        //{
                        //    //System.Net.Mail.Attachment data = Path.IsPathRooted(attachment) == true ? new System.Net.Mail.Attachment(attachment, MediaTypeNames.Application.Octet) : new System.Net.Mail.Attachment(HttpContext.Current.Server.MapPath(attachment), MediaTypeNames.Application.Octet);
                        //    System.Net.Mail.Attachment data;
                        //    if (Path.IsPathRooted(attachment) == true)
                        //    {
                        //        data = new System.Net.Mail.Attachment(attachment, MediaTypeNames.Application.Octet);
                        //    }
                        //    else
                        //    {
                        //        data = new System.Net.Mail.Attachment(HttpContext.Current.Server.MapPath(attachment), MediaTypeNames.Application.Octet);
                        //    }
                        //    mail.Attachments.Add(data);
                        //}
                        foreach (var attachment in attachments)
                        {
                            //System.Net.Mail.Attachment data = Path.IsPathRooted(attachment) == true ? new System.Net.Mail.Attachment(attachment, MediaTypeNames.Application.Octet) : new System.Net.Mail.Attachment(HttpContext.Current.Server.MapPath(attachment), MediaTypeNames.Application.Octet);

                            System.Net.Mail.Attachment data;
                            ExcelPackage ExcelPkg = (ExcelPackage)attachment;
                            if (Path.IsPathRooted(ExcelPkg.File.Name) == true)
                            {
                                data = new System.Net.Mail.Attachment(ExcelPkg.File.OpenRead(), MediaTypeNames.Application.Octet);
                            }
                            else
                            {
                                string filepath = ConfigurationManager.AppSettings["UploadFilePath"].ToString() + "FunctionalReportPackage.xlsx";
                                //string filepath = "D:\\FunctionalReportPackage.xlsx";
                                data = new System.Net.Mail.Attachment(HttpContext.Current.Server.MapPath(filepath), MediaTypeNames.Application.Octet);
                                //data = new System.Net.Mail.Attachment(HttpContext.Current.Server.MapPath(ExcelPkg.File.DirectoryName + ExcelPkg.File.Name), MediaTypeNames.Application.Octet);
                            }
                            mail.Attachments.Add(data);
                        }
                    }
                }
                client.Credentials = new NetworkCredential(userName, password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
                mail.Dispose();
            }

            catch (Exception err)
            {
                result = err.ToString();
            }

            return result;
        }

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
}
