using NA.PMS.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NA.PMS.Model;
using System.IO;
using System.Net;
using NA.PMS.Common;
using NoidaAuthority.PMS.Common;

namespace NA.PMS.Web.Areas.Property.Controllers
{
    public class PaymentGatewayController : Controller
    {
        IAllotmentService _allotmentService;
        IGeneralService _generalService;
        const string COMPANYTYPE = "Companytype";
        public PaymentGatewayController(IAllotmentService allotmentService, IGeneralService generalService)
        {
            _allotmentService = allotmentService;
            _generalService = generalService;
        }

        [AllowAnonymous]
        public ActionResult OnlineApplicationRequest()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult OfflineApplicationRequest()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult OnlineApplicationRequest(OnlineApplicationFormModel applicationFormModel)
        {
            var lstOnline = _allotmentService.OnlineApplicationRequest(applicationFormModel);
            return View("OnlineApplicationuploads", applicationFormModel);
        }
        [AllowAnonymous]
        public ActionResult OnlineApplicationChecklist()
        {
            return View();
        }

        private int GenerateOTP()
        {
            Session["OTP"] = null;
            Random random = new Random();
            int maxValue = 999999;
            int r = random.Next(maxValue);
            return r;
        }

        private void SMSSend(string mobileNo, string msg)
        {
            WebClient client = new WebClient();
            string baseurl = ConfigurationManager.AppSettings["SMSsend"].ToString() + ConfigurationManager.AppSettings["SMSUsername"].ToString() + "&password=" + ConfigurationManager.AppSettings["SMSPassword"].ToString() + "&sendername=" + "NETSMS" + "&mobileno=" + mobileNo + "&message=" + msg;
            Stream data = client.OpenRead(baseurl);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            data.Close();
            reader.Close();
        }

        [AllowAnonymous]
        public JsonResult VerifyOTP(string otp)
        {
            var flag = false;
            if (Session["OTP"].ToString().Equals(otp.ToString()))
            {
                flag = true;
            }
            return Json(flag);
        }

        [AllowAnonymous]
        public void GenerateOTPForOfline(string strMobileNumber)
        {
            Session["OTP"] = null;
            int otp = GenerateOTP();
            Session["OTP"] = otp;
            //string strMsg = "Your OTP is " + otp.ToString() + " DO NOT disclose this to anyone by any means. This is for online use by you only.";
            string strMsg = string.Format(NAMessages.OTPForOffline, otp.ToString());
            SMSSend(strMobileNumber, strMsg);
        }

        [AllowAnonymous]
        public ActionResult OTPDetails()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ViewOnlineDetails(int id)
        {
            var lstViewDetails = _allotmentService.ViewOnlineDetails(id);
            return View(lstViewDetails);
        }

        [AllowAnonymous]
        public ActionResult EditDetails(int id)
        {
            var lstViewDetails = _allotmentService.ViewOnlineDetails(id);
            return View(lstViewDetails);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Edit(OnlineApplicationFormModel onlineApplicationFormModel)
        {
            OnlineApplicationFormModel lstViewDetails = _allotmentService.EditOnlineDetails(onlineApplicationFormModel);
            return RedirectToAction("ViewOnlineDetails", new { id = lstViewDetails.ApplicationId });
        }

        [AllowAnonymous]
        public ActionResult PaymentDetails(int id)
        {
            var lstViewDetails = _allotmentService.SaveTrasOnlineDetails(id);
            return View(lstViewDetails);
        }

        [AllowAnonymous]
        public ActionResult PrintTransaction(int appId)
        {
            var lst = "Applicant Name: Dummy Data";
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult GetSchemeListForOnline()
        {
            List<SchemeAllotmentModel> schemeList = _allotmentService.GetSchemeListForAllotment();
            return Json(schemeList, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult FilterDepartmentOnSchemeForOnline(int SchemeId)
        {
            List<DepartmentAllotmentModel> department = _allotmentService.FilterDepartmentOnScheme(SchemeId);
            return Json(department, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult OnlineApplicationUploads()
        {
            return View();
        }

        [AllowAnonymous]
        public JsonResult SaveFiles()
        {
            var flag = false;
            var flagFilesSaved = false;
            var obj = new UploadDetails();
            var lst = Request["IDLst"];
            if (Request.Files.Count > 0)
            {
                if (Request.Files[0] != null && Request.Files[1] != null)
                {
                    var filePhoto = Request.Files[0];
                    var fileSign = Request.Files[1];

                    var applicationID = Request["applicationID"];
                    var details = new UploadDetails
                    {
                        filePhoto = new FileInfo(filePhoto.FileName).Name,
                        fileSign = new FileInfo(fileSign.FileName).Name,
                        fileDoc = string.Empty,
                        IDLst = lst,
                        ApplicationID = Convert.ToInt32(applicationID)
                    };

                    if (Request.Files.Count > 2)
                    {
                        details.fileDoc = new FileInfo(Request.Files[2].FileName).Name;
                    }
                    flag = _allotmentService.SaveFileDetailsToDB(details);

                    if (flag == true)
                    {
                        flagFilesSaved = true;
                        if (filePhoto != null && filePhoto.ContentLength > 0)
                        {
                            if (!Directory.Exists(Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID)))
                            {
                                Directory.CreateDirectory(Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID));
                            }
                            var fileSavePath = Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID + "/" + details.filePhoto);
                            filePhoto.SaveAs(fileSavePath);
                        }
                        if (fileSign != null && fileSign.ContentLength > 0)
                        {
                            if (!Directory.Exists(Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID)))
                            {
                                Directory.CreateDirectory(Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID));
                            }
                            var fileSavePath = Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID + "/" + details.fileSign);
                            fileSign.SaveAs(fileSavePath);
                            flagFilesSaved = true;
                        }
                        if (Request.Files.Count > 2 && Request.Files[2] != null && Request.Files[2].ContentLength > 0)
                        {
                            if (!Directory.Exists(Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID)))
                            {
                                Directory.CreateDirectory(Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID));
                            }
                            var fileSavePath = Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID + "/" + details.fileDoc);
                            Request.Files[2].SaveAs(fileSavePath);
                            flagFilesSaved = true;
                        }
                    }
                }
            }
            else
            {
                var applicationID = Request["applicationID"];
                var details = new UploadDetails
                {
                    filePhoto = string.Empty,
                    fileSign = string.Empty,
                    fileDoc = string.Empty,
                    IDLst = lst,
                    ApplicationID = Convert.ToInt32(applicationID)
                };
                flag = _allotmentService.SaveFileDetailsToDB(details);
                if (flag == true)
                    flagFilesSaved = true;
            }
            if (flagFilesSaved)
            {
                var applicationID = Request["applicationID"];
                var getDetails = _allotmentService.GetDetails(Convert.ToInt32(applicationID));
                //string strMsg = "Your online application request has been submit sucessfully.";
                string strMsg = NAMessages.OnlineAppReqSuccess;
                SMSSend(getDetails.MobNu, strMsg);
                var body = "Your online application request has been submit sucessfully.";
                EmailHelper emailHelper = new EmailHelper();
                emailHelper.Send(getDetails.EmailId, "Online Application Request", body);
            }
            return Json(flagFilesSaved, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Reads Documents list.
        /// Kendo's DataSourceRequest functionality has not been used as the number of documents would be less, hence its not required.
        /// </summary>
        /// <param name="request">Kendo grid internal parameter</param>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult GetApplicationChcklstDocuments([DataSourceRequest]DataSourceRequest request, int applicationId)
        {
            var allDocs = _allotmentService.GetApplicationChcklstDocuments(request, applicationId);
            var data = allDocs.ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult GetCompanyType()
        {
            var lst = _generalService.BindDDL(COMPANYTYPE);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult GetAllBanksforOnline()
        {
            var lst = _allotmentService.GetAllBanksforOnline();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        // To get all flooors type ddl.
        [AllowAnonymous]
        public JsonResult GetFloors(int schemeid, int depttID)
        {
            var lst = _allotmentService.GetFloors(schemeid, depttID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetAreaDetails(int id)
        {
            var lst = _allotmentService.GetAreaDetails(id);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetEarneshMoney(int id, int deptt, int floor)
        {
            var lst = _allotmentService.GetEarneshMoney(id, deptt, floor);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        private string PreparePOSTForm(string url, System.Collections.Hashtable data)      // post form
        {
            string formID = "PostForm";
            //Build the form using the specified data to be posted.
            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" +
                           formID + "\" action=\"" + url +
                           "\" method=\"POST\">");

            foreach (System.Collections.DictionaryEntry key in data)
            {

                strForm.Append("<input type=\"hidden\" name=\"" + key.Key +
                               "\" value=\"" + key.Value + "\">");
            }


            strForm.Append("</form>");
            //Build the JavaScript which will do the Posting operation.
            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("var v" + formID + " = document." +
                             formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");
            //Return the form and the script concatenated.
            //(The order is important, Form then JavaScript)
            return strForm.ToString() + strScript.ToString();
        }

        [AllowAnonymous]
        public void OnlinePayment(int id)
        {
            var lstViewDetails = _allotmentService.SaveTrasOnlineDetails(id);
            string firstName = lstViewDetails.FirstName;
            decimal? amount = lstViewDetails.Amount;
            string productInfo = lstViewDetails.Productinfo;
            string email = lstViewDetails.Email;
            string phone = lstViewDetails.PhoneNumber;
            string surl = ConfigurationManager.AppSettings["surl"];
            string furl = ConfigurationManager.AppSettings["furl"];
            string purl = ConfigurationManager.AppSettings["purl"];
            string key = ConfigurationManager.AppSettings["key"];
            string salt = ConfigurationManager.AppSettings["salt"];
            string udf1 = lstViewDetails.Txnid;

            RemotePost myremotepost = new RemotePost();
            //posting all the parameters required for integration.

            string txnid = Generatetxnid();
            myremotepost.Url = purl;
            myremotepost.Add("key", "gtKFFx");
            myremotepost.Add("txnid", txnid);
            myremotepost.Add("amount", amount.ToString());
            myremotepost.Add("productinfo", productInfo);
            myremotepost.Add("firstname", firstName);
            myremotepost.Add("phone", phone);
            myremotepost.Add("email", email);
            myremotepost.Add("surl", surl);
            myremotepost.Add("furl", furl);
            myremotepost.Add("udf1", udf1);
            string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + udf1 + "||||||||||" + salt;
            string hash = Generatehash512(hashString);
            myremotepost.Add("hash", hash);
            myremotepost.Post();
        }

        [AllowAnonymous]
        public void OtherPayments()
        {
            var id = Convert.ToInt32(Request["refno"]);
            var paymentType = Request["PaymentType"].ToString();
            var lstViewDetails = _allotmentService.SaveOtherPaymentTras(id);
            string firstName = lstViewDetails.FirstName;
            decimal? amount = lstViewDetails.Amount == null ? 1000 : lstViewDetails.Amount;
            string productInfo = lstViewDetails.Productinfo;
            string email = lstViewDetails.Email;
            string phone = lstViewDetails.PhoneNumber;
            string udf1 = lstViewDetails.Txnid;
            string udf2 = paymentType;

            //Configurations
            string surl = ConfigurationManager.AppSettings["surl"];
            string furl = ConfigurationManager.AppSettings["furl"];
            string purl = ConfigurationManager.AppSettings["purl"];
            string key = ConfigurationManager.AppSettings["key"];
            string salt = ConfigurationManager.AppSettings["salt"];

            RemotePost myremotepost = new RemotePost();
            //posting all the parameters required for integration.

            string txnid = Generatetxnid();
            myremotepost.Url = purl;
            myremotepost.Add("key", "gtKFFx");
            myremotepost.Add("txnid", txnid);
            myremotepost.Add("amount", amount.ToString());
            myremotepost.Add("productinfo", productInfo);
            myremotepost.Add("firstname", firstName);
            myremotepost.Add("phone", phone);
            myremotepost.Add("email", email);
            myremotepost.Add("surl", surl);
            myremotepost.Add("furl", furl);
            myremotepost.Add("udf1", udf1);
            myremotepost.Add("udf2", udf2);
            string hashString = key + "|" + txnid + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|" + udf1 + "|" + udf2 + "|||||||||" + salt;
            string hash = Generatehash512(hashString);
            myremotepost.Add("hash", hash);
            myremotepost.Post();
        }

        [AllowAnonymous]
        public class RemotePost
        {
            private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();
            public string Url = "";
            public string Method = "post";
            public string FormName = "form1";

            public void Add(string name, string value)
            {
                Inputs.Add(name, value);
            }

            [AllowAnonymous]
            public void Post()
            {
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.Write("<html><head>");
                System.Web.HttpContext.Current.Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));
                System.Web.HttpContext.Current.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
                for (int i = 0; i < Inputs.Keys.Count; i++)
                {
                    System.Web.HttpContext.Current.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", Inputs.Keys[i], Inputs[Inputs.Keys[i]]));
                }
                System.Web.HttpContext.Current.Response.Write("</form>");
                System.Web.HttpContext.Current.Response.Write("</body></html>");
                System.Web.HttpContext.Current.Response.End();
            }
        }

        //Hash generation Algorithm
        [AllowAnonymous]
        public string Generatehash512(string text)
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

        [AllowAnonymous]
        public string Generatetxnid()
        {
            Random rnd = new Random();
            string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
            string txnid1 = strHash.ToString().Substring(0, 20);
            return txnid1;
        }

        [AllowAnonymous]
        public ActionResult Return(FormCollection form)
        {
            var onlineApplicationDetailsTrans = new OnlineApplicationDetailsTrans();
            try
            {
                string[] merc_hash_vars_seq;
                string merc_hash_string = string.Empty;
                string merc_hash = string.Empty;
                string order_id = string.Empty;
                string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";
                if (form["status"].ToString() == "success")
                {
                    merc_hash_vars_seq = hash_seq.Split('|');
                    Array.Reverse(merc_hash_vars_seq);
                    merc_hash_string = ConfigurationManager.AppSettings["SALT"] + "|" + form["status"].ToString();
                    foreach (string merc_hash_var in merc_hash_vars_seq)
                    {
                        merc_hash_string += "|";
                        merc_hash_string = merc_hash_string + (form[merc_hash_var] != null ? form[merc_hash_var] : "");
                    }
                    //Response.Write(merc_hash_string);
                    merc_hash = Generatehash512(merc_hash_string).ToLower();

                    if (merc_hash != form["hash"])
                    {
                        Response.Write("Hash value did not matched");
                    }
                    else
                    {
                        var lstViewDetails = _allotmentService.UpdateTrasactionDetails(form);
                        if (lstViewDetails != null)
                        {
                            onlineApplicationDetailsTrans.StatusName = lstViewDetails.StatusName;
                            onlineApplicationDetailsTrans.FirstName = lstViewDetails.FirstName;
                            onlineApplicationDetailsTrans.Amount = lstViewDetails.Amount;
                            onlineApplicationDetailsTrans.Txnid = lstViewDetails.Txnid;
                            onlineApplicationDetailsTrans.bank_ref_num = lstViewDetails.bank_ref_num;
                            onlineApplicationDetailsTrans.card_type = lstViewDetails.card_type;
                            onlineApplicationDetailsTrans.error = lstViewDetails.error;
                            onlineApplicationDetailsTrans.error_Message = lstViewDetails.error_Message;
                            onlineApplicationDetailsTrans.issuing_bank = lstViewDetails.issuing_bank;
                            onlineApplicationDetailsTrans.OnlineApplicationId = lstViewDetails.OnlineApplicationId;
                            onlineApplicationDetailsTrans.TrKey = lstViewDetails.Mihpayid;
                            onlineApplicationDetailsTrans.name_on_card = lstViewDetails.name_on_card;
                        }
                    }
                }
                else
                {
                    Response.Write("Hash value did not matched");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<span style='color:red'>" + ex.Message + "</span>");
            }
            return View(onlineApplicationDetailsTrans);
        }

        public ActionResult ManageOnlineApplication()
        {
            return View();
        }

        public JsonResult GetOnlineApplications([DataSourceRequest] DataSourceRequest req)
        {
            var lst = _allotmentService.GetOnlineApplications(req);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewOnlineApplication(int id)
        {
            var lstViewDetails = _allotmentService.ViewOnlineDetails(id);
            return View(lstViewDetails);
        }

        public ActionResult SaveCommentByApplicationID(int requestNo, string Comment, bool acceptReject)
        {
            var lst = _allotmentService.SaveCommentByApplicationID(requestNo, Comment, acceptReject);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveCommentByAppID(int requestNo, string Comment, bool acceptReject, string MobNo, string EmailId)
        {
            var lst = _allotmentService.SaveCommentByAppID(requestNo, Comment, acceptReject, MobNo, EmailId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult DownloadApplicationFormat(int appId, int departmentId)
        {
            var lst = _generalService.DownloadApplicationFormat(appId, Convert.ToInt32(LetterTemplate.ApplicationFormat), departmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult HelpOnline()
        {
            return View();
        }
    }
}