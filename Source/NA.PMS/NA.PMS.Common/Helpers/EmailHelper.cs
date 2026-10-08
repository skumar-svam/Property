using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NoidaAuthority.PMS.Common
{
    public class EmailHelper
    {
        public void Send(string toAddress, string subject, string body)
        {
            Task.Factory.StartNew(() => SendEmail(toAddress, subject, body, false), TaskCreationOptions.LongRunning);
        }

        private void SendEmail(string toAddress, string subject, string body, bool priority)
        {
            MailAddress fromAddress = new MailAddress(ConfigurationManager.AppSettings["SmtpFromAddress"]);
            MailAddress toAddressNew = new MailAddress(toAddress);
            string serverName = ConfigurationManager.AppSettings["SmtpServerName"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            string userName = ConfigurationManager.AppSettings["SmtpUserName"];
            string password = ConfigurationManager.AppSettings["SmtpPassword"];

            var message = new MailMessage(fromAddress, toAddressNew);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            message.HeadersEncoding = Encoding.UTF8;
            message.SubjectEncoding = Encoding.UTF8;
            message.BodyEncoding = Encoding.UTF8;
            if (priority) message.Priority = MailPriority.High;


            SmtpClient client = new SmtpClient(serverName, port);
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
    }
}
