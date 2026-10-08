using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Repository
{
    public class NAApplication
    {
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public NAApplication()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["CurrentUser"] != null)
                    {
                        userInfo = HttpContext.Current.Session["CurrentUser"] as CurrentUserDetail;
                        using (var dbContext = new NoidaPMSEntities())
                        {
                            DepartmentList = dbContext.UmUserDepartmentTrans.Where(u => u.UserRefId == userInfo.UserID && u.Status == true).Select(d => d.DepartmentId).ToList();
                        }
                    }
                }
            }
        }

        public string SendSMS(string mobileNo, string msg)
        {
            string message = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                WebClient client = new WebClient();
                //string baseurl = ConfigurationManager.AppSettings["SMSsend"].ToString() + ConfigurationManager.AppSettings["SMSUsername"].ToString() + "&password=" + ConfigurationManager.AppSettings["SMSPassword"].ToString() + "&sendername=" + ConfigurationManager.AppSettings["SMSSenderID"].ToString() + "&mobileno=" + mobileNo + "&message=" + msg;
                string baseurl = ConfigurationManager.AppSettings["SMSApiUrl"].ToString() + "ApiKey=" + ConfigurationManager.AppSettings["SMSApiKey"].ToString() + "&ClientId=" + ConfigurationManager.AppSettings["SMSClientId"].ToString() + "&SenderId=" + ConfigurationManager.AppSettings["SMSSenderId"].ToString() + "&Message=" + msg + "&MobileNumbers=91" + mobileNo + "&Is_Unicode=" + ConfigurationManager.AppSettings["SMSIsUnicode"].ToString() + "&Is_Flash=" + ConfigurationManager.AppSettings["SMSIsFlash"].ToString();
                Stream data = client.OpenRead(baseurl);
                StreamReader reader = new StreamReader(data);
                message = reader.ReadToEnd();
                data.Close();
                reader.Close();
            }

            //try
            //{
            //    WebClient client = new WebClient();
            //    string baseurl = ConfigurationManager.AppSettings["SMSsend"].ToString() + ConfigurationManager.AppSettings["SMSUsername"].ToString() + "&password=" + ConfigurationManager.AppSettings["SMSPassword"].ToString() + "&sendername=" + ConfigurationManager.AppSettings["SMSSenderID"].ToString() + "&mobileno=" + mobileNo + "&message=" + msg;
            //    Stream data = client.OpenRead(baseurl);
            //    StreamReader reader = new StreamReader(data);
            //    string s = reader.ReadToEnd();
            //    data.Close();
            //    reader.Close();
            //    flag = 1;
            //}
            //catch (Exception e)
            //{

            //}
            return message;
        }

        public int SaveAndSendSMS(int? rid, string mobileNo, string message, string smstype, string heading, string senderId)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                string sms_status = string.Empty;
                WebClient client = new WebClient();
                //string baseurl = ConfigurationManager.AppSettings["SMSsend"].ToString() + ConfigurationManager.AppSettings["SMSUsername"].ToString() + "&password=" + ConfigurationManager.AppSettings["SMSPassword"].ToString() + "&sendername=" + ConfigurationManager.AppSettings["SMSSenderID"].ToString() + "&mobileno=" + mobileNo + "&message=" + message;
                string baseurl = ConfigurationManager.AppSettings["SMSApiUrl"].ToString() + "ApiKey=" + ConfigurationManager.AppSettings["SMSApiKey"].ToString() + "&ClientId=" + ConfigurationManager.AppSettings["SMSClientId"].ToString() + "&SenderId=" + ConfigurationManager.AppSettings["SMSSenderId"].ToString() + "&Message=" + message + "&MobileNumbers=91" + mobileNo + "&Is_Unicode=" + ConfigurationManager.AppSettings["SMSIsUnicode"].ToString() + "&Is_Flash=" + ConfigurationManager.AppSettings["SMSIsFlash"].ToString();
                Stream data = client.OpenRead(baseurl);
                StreamReader reader = new StreamReader(data);
                sms_status = reader.ReadToEnd();
                data.Close();
                reader.Close();

                SMSServicesMst sms = new SMSServicesMst();
                sms.RegistrationId = rid;
                sms.MobileNo = mobileNo;
                sms.SMSType = smstype;
                sms.Heading = heading;
                sms.Message = message;
                sms.Status = sms_status;
                sms.SenderId = senderId;
                sms.CreatedBy = userInfo.UserID;
                sms.CreatedDate = DateTime.Now;
                sms.SentDate = DateTime.Now;
                dbContext.SMSServicesMsts.Add(sms);
                dbContext.SaveChanges();
                flag = ReturnType.Success;
            }
            return flag;
        }


        public int Audit(char type, string tableName, string formName, string keyField, string keyValue, string fieldName, string oldValue, string newValue)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var audit = new Audit();
                audit.Type = type.ToString();
                audit.TableName = tableName;
                audit.FormName = formName;
                audit.PrimaryKeyField = keyField;
                audit.PrimaryKeyValue = keyValue;
                audit.FieldName = fieldName;
                audit.OldValue = oldValue;
                audit.NewValue = newValue;
                audit.UpdateDate = DateTime.Now;
                audit.UserName = userInfo.UserID.ToString();
                dbContext.Audits.Add(audit);
                dbContext.SaveChanges();
                flag = ReturnType.Success;
            }
            return flag;
        }
    }


}
