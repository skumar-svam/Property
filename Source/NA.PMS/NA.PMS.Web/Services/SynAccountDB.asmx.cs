using NA.PMS.Model;
using NA.PMS.Service.AccountWS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace NA.PMS.Web.Services
{
    /// <summary>
    /// Summary description for SynAccountDB
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class SynAccountDB : System.Web.Services.WebService
    {
        [WebMethod]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]       
        public string SyncReceiptMasterDB(ReceiptModel receiptDetails)
        {
            OutResultModel outResult = new OutResultModel();
            string tokenId = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["TokenId_Service"]) ? ConfigurationManager.AppSettings["TokenId_Service"] : string.Empty;

            if (!string.IsNullOrEmpty(tokenId))
            {
                IAccountWSService _accountWSService = new AccountWSService();
                if (receiptDetails != null)
                {
                    if (receiptDetails.ReciptDetailMaster != null && receiptDetails.ReciptDetailMaster.RECEIPTTRANSLIST != null)
                    {
                        if (!string.IsNullOrEmpty(receiptDetails.TokenId))
                        {
                            if (tokenId == receiptDetails.TokenId)
                            {
                                outResult = _accountWSService.SaveReceiptData(receiptDetails);
                            }
                            else
                            {
                                outResult.ErrorMessage = "Authorized key not matched.";
                            }
                        }
                        else
                        {
                            outResult.ErrorMessage = "authorization key should not be blank.";
                        }
                    }
                    else
                    {
                        outResult.ErrorMessage = "Reciept model should not be blank.";
                    }
                }
                else
                {
                    outResult.ErrorMessage = "Reciept & Transaction model should not be blank.";
                }
            }
            else
            {
                outResult.ErrorMessage = "Not Authorized.";
            }
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(outResult);
        }
    }
}
