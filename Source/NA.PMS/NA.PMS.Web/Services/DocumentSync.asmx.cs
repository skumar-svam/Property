using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using NA.PMS.Model;
using NA.PMS.Service.AccountWS;

namespace NA.PMS.Web.Services
{
    /// <summary>
    /// Summary description for DocumentSync
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DocumentSync : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public List<PropertyDocument> GetDocumentDetails(int rid)
        {
            List<PropertyDocument> result = null;
            IAccountWSService _accountWSService = new AccountWSService();
            result = _accountWSService.GetDocumentDetails(rid);
            return result;
        }
    }
}
