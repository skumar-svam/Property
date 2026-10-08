using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.OnlineScheme
{
    public static class OnlineSchemeHelper
    {
        //encrypt optional parameter in url
        public static string Encode(string encodeVal)
        {
            byte[] encoded = Encoding.UTF8.GetBytes(encodeVal);
            return Convert.ToBase64String(encoded);
        }
        //decrypt optional parameter in url
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

        public static int GenerateOTP()
        {
            Random random = new Random();
            int maxValue = 999999;
            int otp = random.Next(maxValue);
            return otp;
        }

        public static string GenerateSchemeFormPassword()
        {
            Random random = new Random();
            int maxValue = 999999;
            string Key = "NS";
            int Pass = random.Next(maxValue);
            return Key + Pass;
            //return "Noida";
        }

        public static String MD5HashPassword(this String plainText)
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

        public static string GeneratePaymentTransactionId()
        {
            Random rnd = new Random();
            string _inputText = rnd.ToString() + DateTime.Now;
            byte[] message = Encoding.UTF8.GetBytes(_inputText);
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            string txnid = hex.ToString().Substring(0, 20);
            return txnid;
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

        public static void SendEmail(string toAddress, string subject, string body)
        {
            Task.Factory.StartNew(() => SendMail(toAddress, subject, body, false), TaskCreationOptions.LongRunning);
        }
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
                    int hundredplace = (int)(number % 1000); // for hundred
                    result = (croreplace == 0 ? string.Empty : (ThreeDigitNumber(croreplace) + " Crore ")) + (lakhplace == 0 ? string.Empty : (TwoDigitNumber(lakhplace) + " Lakh ")) + (thousandplace == 0 ? string.Empty : (TwoDigitNumber(thousandplace) + " Thousand ")) + ThreeDigitNumber(hundredplace);
                }
                else if (numberLength > 10 && numberLength <= 12)
                {
                    int croreplace = (int)(number / 10000000);//for crore 
                    int lakhplace = (int)(number % 10000000) / 100000; //for lakh
                    int thousandplace = (int)(number % 100000) / 1000; // for thousand
                    int hundredplace = (int)(number % 1000); // for hundred
                    result = (croreplace == 0 ? string.Empty : (FiveDigitNumber(croreplace) + " Crore ")) + (lakhplace == 0 ? string.Empty : (TwoDigitNumber(lakhplace) + " Lakh ")) + (thousandplace == 0 ? string.Empty : (TwoDigitNumber(thousandplace) + " Thousand ")) + ThreeDigitNumber(hundredplace);
                }
            }
            return result;
        }

        #endregion
    }
  
}
