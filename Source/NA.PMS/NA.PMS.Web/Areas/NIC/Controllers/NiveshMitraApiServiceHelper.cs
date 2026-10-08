using NA.PMS.Common;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace NA.PMS.Web.Areas.NIC.Controllers
{
    public class NiveshMitraApiServiceHelper
    {
        static string servicePassalt = ConfigurationManager.AppSettings["ServicePassalt"];
        public string GetWReturn_CUSID_STATUS(WReturn_CUSID_STATUSModel apimodel)
        {
            try
            {
                apimodel.Transaction_ID = string.Empty;
                apimodel.Transaction_Date = string.Empty;
                apimodel.Transaction_Date_Time = string.Empty;

                if (string.IsNullOrEmpty(apimodel.NOC_Certificate_Number))
                {
                    apimodel.NOC_Certificate_Number = string.Empty;
                }

                if (string.IsNullOrEmpty(apimodel.NOC_URL))
                {
                    apimodel.NOC_URL = string.Empty;
                }

                apimodel.ISNOC_URL_ActiveYesNO = string.Empty;
                string path = string.Empty;
                string xmlInputData = string.Empty;
                string xmlOutputData = string.Empty;
                using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
                {
                    string result = client.WReturn_CUSID_STATUS(apimodel.ControlID, apimodel.UnitID, apimodel.ServiceID, apimodel.ProcessIndustryID, apimodel.ApplicationID, apimodel.Status_Code, apimodel.Remarks, apimodel.Fee_Amount, apimodel.Fee_Status, apimodel.Transaction_ID, apimodel.Transaction_Date, apimodel.Transaction_Date_Time, apimodel.NOC_Certificate_Number, apimodel.NOC_URL, apimodel.ISNOC_URL_ActiveYesNO, servicePassalt);
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

        public string WReturn_CUSID_ISLandPurchased(WReturn_CUSID_STATUSModel apimodel)
        {
            try
            {
                string path = string.Empty;
                string xmlInputData = string.Empty;
                string xmlOutputData = string.Empty;
                using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
                {
                    string result = client.WReturn_CUSID_ISLandPurchased(apimodel.ControlID, apimodel.UnitID, apimodel.ServiceID, apimodel.ISLandPurchasedYesNO, apimodel.passsalt);
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

        public string GetAndUpdateServiceStatus(NICServiceStatusViewModel apimodel)
        {
            try
            {
                apimodel.TransactionId = string.Empty;
                apimodel.TransactionDate = string.Empty;
                apimodel.TransactionDateTime = string.Empty;
                apimodel.NOCCertificateNo = string.IsNullOrEmpty(apimodel.NOCCertificateNo) ? string.Empty : apimodel.NOCCertificateNo;
                apimodel.NOCUrl = string.IsNullOrEmpty(apimodel.NOCUrl) ? string.Empty : apimodel.NOCUrl;
                apimodel.NOCUrlActiveStatus = string.Empty;
                string path = string.Empty;
                string xmlInputData = string.Empty;
                string xmlOutputData = string.Empty;
                using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
                {
                    string result = client.WReturn_CUSID_STATUS(apimodel.ControlId, apimodel.UnitId, apimodel.ServiceId, apimodel.ProcessIndustryId, apimodel.ApplicationId, apimodel.StatusCode, apimodel.Remarks, apimodel.FeeAmount, apimodel.FeeStatus, apimodel.TransactionId, apimodel.TransactionDate, apimodel.TransactionDateTime, apimodel.NOCCertificateNo, apimodel.NOCUrl, apimodel.NOCUrlActiveStatus, servicePassalt);
                    return result;
                }
            }
            catch (Exception ex)
            {
                return "Error";
                throw ex;
            }
        }

        public OnlineFormViewModel GetNICPostedBasicDetails(NICBasicDetailViewModel apimodel)
        {
            OnlineFormViewModel nicmodel = new OnlineFormViewModel();
            if (apimodel.TxtControlID != null && apimodel.TxtUnitID != null && apimodel.TxtServiceID != null)
            {
                HttpContext.Current.Session["WBasicDetailsNIC"] = null;
                string xmlInputData = string.Empty;

                nicmodel.NICControlId = apimodel.TxtControlID;
                nicmodel.NICUnitId = apimodel.TxtUnitID;
                nicmodel.NICServiceId = apimodel.TxtServiceID;
                nicmodel.NICProcessIndustryId = apimodel.TxtProcessIndustryID;
                nicmodel.NICApplicationId = apimodel.TxtApplicationID;
                nicmodel.AppType = Constants.NIC;
                nicmodel.IsFromNIC = true;
                using (var client = new SingleWindowServiceReferenceNIC.upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap"))
                {
                    string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
                    System.Data.DataSet result = client.WGetBasicDetails(apimodel.TxtControlID, apimodel.TxtUnitID, apimodel.TxtServiceID, apimodel.TxtProcessIndustryID, Passalt);
                    if (result != null)
                    {
                        xmlInputData = result.GetXml();
                        nicmodel.NICXmlInputTable = xmlInputData;
                        NICXmlDeserializer xmlSerializer = new NICXmlDeserializer();
                        NICInfoDetailViewModel naDataSet = xmlSerializer.Deserialize<NICInfoDetailViewModel>(xmlInputData);
                        if (naDataSet.Table != null)
                        {
                            naDataSet.Table.ServiceID = apimodel.TxtServiceID;
                            HttpContext.Current.Session["WBasicDetailsNIC"] = naDataSet;
                            nicmodel.NICInfoDetailViewModel = naDataSet;
                            nicmodel.FlagId = ReturnType.Saved;

                            return nicmodel;
                        }
                        else
                        {
                            nicmodel.FlagId = ReturnType.Failed;
                            return nicmodel;
                        }
                    }
                    else
                    {
                        nicmodel.FlagId = ReturnType.NotExist;
                        return nicmodel;
                    }
                }
            }
            else
            {
                nicmodel.FlagId = ReturnType.NotExist;
            }
            return nicmodel;
        }

        
    }
}