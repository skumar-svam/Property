using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Service;
using System;
using System.Configuration;

namespace NA.PMS.Web.Controllers.Common
{
    public class NiveshMitraServices
    {
        static string servicePassalt = ConfigurationManager.AppSettings["ServicePassalt"];
        public string GetWReturn_CUSID_STATUS(WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel)
        {
            try
            {
                objWReturn_CUSID_STATUSModel.Transaction_ID = string.Empty;
                objWReturn_CUSID_STATUSModel.Transaction_Date = string.Empty;
                objWReturn_CUSID_STATUSModel.Transaction_Date_Time = string.Empty;

                if (string.IsNullOrEmpty(objWReturn_CUSID_STATUSModel.NOC_Certificate_Number))
                {
                    objWReturn_CUSID_STATUSModel.NOC_Certificate_Number = string.Empty;
                }

                if (string.IsNullOrEmpty(objWReturn_CUSID_STATUSModel.NOC_URL))
                {
                    objWReturn_CUSID_STATUSModel.NOC_URL = string.Empty;
                }

                objWReturn_CUSID_STATUSModel.ISNOC_URL_ActiveYesNO = string.Empty;
                string path = string.Empty;
                string xmlInputData = string.Empty;
                string xmlOutputData = string.Empty;
                using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
                {
                    string result = client.WReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel.ControlID, objWReturn_CUSID_STATUSModel.UnitID, objWReturn_CUSID_STATUSModel.ServiceID, objWReturn_CUSID_STATUSModel.ProcessIndustryID, objWReturn_CUSID_STATUSModel.ApplicationID, objWReturn_CUSID_STATUSModel.Status_Code, objWReturn_CUSID_STATUSModel.Remarks, objWReturn_CUSID_STATUSModel.Fee_Amount, objWReturn_CUSID_STATUSModel.Fee_Status, objWReturn_CUSID_STATUSModel.Transaction_ID, objWReturn_CUSID_STATUSModel.Transaction_Date, objWReturn_CUSID_STATUSModel.Transaction_Date_Time, objWReturn_CUSID_STATUSModel.NOC_Certificate_Number, objWReturn_CUSID_STATUSModel.NOC_URL, objWReturn_CUSID_STATUSModel.ISNOC_URL_ActiveYesNO, servicePassalt);
                    return result;
                }
            }
            catch (Exception ex)
            {
                return "Error";
                throw ex;
            }
        }

        public string GetServiceStatus(string ServiceCode)
        {
            string _msg = string.Empty;
            if (ServiceCode == ServiceStatus.APPROVED)
            {
                _msg = "COMPLETED";
            }
            else if (ServiceCode == ServiceStatus.FORM_SUBMITTED)
            {
                _msg = "SUBMITTED";
            }
            else if (ServiceCode == ServiceStatus.FORWARDED)
            {
                _msg = "FORWARDED";
            }
            else if (ServiceCode == ServiceStatus.QUERY_OBJECTION)
            {
                _msg = "An QUERY OBJECTION";
            }
            else
            {
                _msg = "SUBMITTED";
            }
            return _msg;
        }

        public string WReturn_CUSID_ISLandPurchased(WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel)
        {
            try
            {
                string path = string.Empty;
                string xmlInputData = string.Empty;
                string xmlOutputData = string.Empty;
                using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
                {
                    string result = client.WReturn_CUSID_ISLandPurchased(objWReturn_CUSID_STATUSModel.ControlID, objWReturn_CUSID_STATUSModel.UnitID, objWReturn_CUSID_STATUSModel.ServiceID, objWReturn_CUSID_STATUSModel.ISLandPurchasedYesNO, objWReturn_CUSID_STATUSModel.passsalt);
                    return result;
                }
            }
            catch (Exception ex)
            {
                return "Error";
                throw ex;
            }
        }

        public string WReturn_CUSID_PrintApplicationURL(WReturn_CUSID_STATUSModel model)
        {
            try
            {
                string path = string.Empty;
                string xmlInputData = string.Empty;
                string xmlOutputData = string.Empty;
                using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
                {
                    string result = client.WReturn_CUSID_PrintApplicationURL(model.ControlID, model.UnitID, model.ServiceID, model.PrintApplicationURL, servicePassalt);
                    return result;
                }
            }
            catch (Exception ex)
            {
                return "Error";
                throw ex;
            }
        }

        public NewDataSet WGetUBPaymentDetails(WReturn_CUSID_STATUSModel model)
        {
            System.Data.DataSet _newDataSet = new System.Data.DataSet();
            NewDataSet objNewDataSet = new NewDataSet();
            try
            {
                string path = string.Empty;
                string xmlInputData = string.Empty;
                string xmlOutputData = string.Empty;
                using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
                {
                    _newDataSet = client.WGetUBPaymentDetails(model.ControlID, model.UnitID, model.ServiceID, servicePassalt);
                    xmlInputData = _newDataSet.GetXml();
                    Deserial objHelp = new Deserial();
                    objNewDataSet = objHelp.Deserialize<NewDataSet>(xmlInputData);
                    return objNewDataSet;
                }
            }
            catch (Exception ex)
            {
                return objNewDataSet;
                throw ex;
            }
        }
    }
}