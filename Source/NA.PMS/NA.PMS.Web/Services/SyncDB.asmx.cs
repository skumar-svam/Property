using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using NA.PMS.Service.AccountWS;
using NA.PMS.Model;
using System.Web.Script.Services;
using System.Web.Script.Serialization;


namespace NA.PMS.Web.Services
{
    /// <summary>
    /// Summary description for SyncDB
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class SyncDB : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string SyncReceiptDB(DateTime entryDate)
        {
            SyncOutPut outResult = new SyncOutPut();
            IAccountWSService _accountWSService = new AccountWSService();
            ReceiptDetails result = _accountWSService.GetReceiptDetail(entryDate);

            if (result != null)
            {
                if (result.ReciptDetailMaster != null)
                {
                    outResult.ReceiptMaster = _accountWSService.InsertReceiptMasterData(result.ReciptDetailMaster);

                }
                if (result.ReceiptAmountTrans != null)
                {
                    outResult.ReceiptTrans = _accountWSService.InsertReceiptTransData(result.ReceiptAmountTrans);
                }
            }
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(outResult);
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public void GetPendingSyncReceiptDetail(DateTime entryDate)
        {
            ReceiptDetails result = null;

            IAccountWSService _accountWSService = new AccountWSService();
            result = _accountWSService.GetReceiptDetail(entryDate);
            JavaScriptSerializer json = new JavaScriptSerializer();
           // return json.Serialize(result);

            Context.Response.ContentType = "application/json";
            Context.Response.Write(json.Serialize(result));

        }

        
    }
}
