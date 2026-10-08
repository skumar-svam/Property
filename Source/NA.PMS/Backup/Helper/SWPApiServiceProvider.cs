using NA.PMS.NICService.NiveshMitraApiServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.NICServices
{
    public class SWPApiServiceProvider
    {
        static string servicePassalt = ConfigurationManager.AppSettings["ServicePassalt"];
        //public string GetWReturn_CUSID_STATUS(SWPStatusViewModel apimodel)
        //{
        //    try
        //    {
        //        apimodel.TransactionId = string.Empty;
        //        apimodel.TransactionDate = string.Empty;
        //        apimodel.TransactionDateTime = string.Empty;

        //        apimodel.NOCCertificateNo = string.IsNullOrEmpty(apimodel.NOCCertificateNo) ? string.Empty : apimodel.NOCCertificateNo;
        //        apimodel.NOCUrl = string.IsNullOrEmpty(apimodel.NOCUrl) ? string.Empty : apimodel.NOCUrl;
        //        apimodel.NOCUrlActiveStatus = string.Empty;
        //        string path = string.Empty;
        //        string xmlInputData = string.Empty;
        //        string xmlOutputData = string.Empty;
        //        using (var client = new upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap_nic"))
        //        {
        //            string result = client.WReturn_CUSID_STATUS(apimodel.ControlId, apimodel.UnitId, apimodel.ServiceId, apimodel.ProcessIndustryId, apimodel.ApplicationId, apimodel.StatusCode, apimodel.Remarks, apimodel.FeeAmount, apimodel.FeeStatus, apimodel.TransactionId, apimodel.TransactionDate, apimodel.TransactionDateTime, apimodel.NOCCertificateNo, apimodel.NOCUrl, apimodel.NOCUrlActiveStatus, servicePassalt);
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error";
        //        throw ex;
        //    }
        //}

        //public SWPTableViewModel WGetUBPaymentDetails(SWPStatusViewModel model)
        //{
        //    System.Data.DataSet _newDataSet = new System.Data.DataSet();
        //    SWPTableViewModel objNewDataSet = new SWPTableViewModel();
        //    try
        //    {
        //        string path = string.Empty;
        //        string xmlInputData = string.Empty;
        //        string xmlOutputData = string.Empty;
        //        using (var client = new upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap_nic"))
        //        {
        //            _newDataSet = client.WGetUBPaymentDetails(model.ControlId, model.UnitId, model.ServiceId, servicePassalt);
        //            xmlInputData = _newDataSet.GetXml();
        //            SWPXmlDeserializer objHelp = new SWPXmlDeserializer();
        //            objNewDataSet = objHelp.Deserialize<SWPTableViewModel>(xmlInputData);
        //            return objNewDataSet;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return objNewDataSet;
        //        throw ex;
        //    }
        //}

        public SWPStatusViewModel GetSWPServiceStatus(SWPStatusViewModel apimodel)
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
                using (var client = new upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap_nic"))
                {
                    string result = client.WReturn_CUSID_STATUS(apimodel.ControlId, apimodel.UnitId, apimodel.ServiceId, apimodel.ProcessIndustryId, apimodel.ApplicationId, apimodel.StatusCode, apimodel.Remarks, apimodel.FeeAmount, apimodel.FeeStatus, apimodel.TransactionId, apimodel.TransactionDate, apimodel.TransactionDateTime, apimodel.NOCCertificateNo, apimodel.NOCUrl, apimodel.NOCUrlActiveStatus, servicePassalt);
                    //return result;
                    apimodel.Status = result;
                    return apimodel;
                }
            }
            catch (Exception ex)
            {
                apimodel.ErrorMessage = ex.ToString();
                return apimodel;
                //return "Error";
                throw ex;
            }
        }

        public SWPTableViewModel GetSWPPaymentDetails(SWPStatusViewModel model)
        {
            System.Data.DataSet _newDataSet = new System.Data.DataSet();
            SWPTableViewModel swpTabel = new SWPTableViewModel();
            try
            {
                string path = string.Empty;
                string xmlInputData = string.Empty;
                string xmlOutputData = string.Empty;
                using (var client = new upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap_nic"))
                {
                    _newDataSet = client.WGetUBPaymentDetails(model.ControlId, model.UnitId, model.ServiceId, servicePassalt);
                    xmlInputData = _newDataSet.GetXml();
                    SWPXmlDeserializer objHelp = new SWPXmlDeserializer();
                    swpTabel = objHelp.Deserialize<SWPTableViewModel>(xmlInputData);
                    return swpTabel;
                }
            }
            catch (Exception ex)
            {
                //swpTabel.Error = ex.ToString();
                return swpTabel;
                throw ex;
            }
        }

        public SWPFormViewModel GetApiPostedBasicDetails(SWPPostViewModel apimodel)
        {
            SWPFormViewModel nicmodel = new SWPFormViewModel();
            if (apimodel.TxtControlID != null && apimodel.TxtUnitID != null && apimodel.TxtServiceID != null)
            {
                HttpContext.Current.Session["WBasicDetailsNIC"] = null;
                string xmlInputData = string.Empty;

                nicmodel.NICControlId = apimodel.TxtControlID;
                nicmodel.NICUnitId = apimodel.TxtUnitID;
                nicmodel.NICServiceId = apimodel.TxtServiceID;
                nicmodel.NICProcessIndustryId = apimodel.TxtProcessIndustryID;
                nicmodel.NICApplicationId = apimodel.TxtApplicationID;
                nicmodel.AppType = SWPConstant.NIC;
                nicmodel.IsFromNIC = true;
                using (var client = new upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap_nic"))
                {
                    string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
                    System.Data.DataSet result = client.WGetBasicDetails(apimodel.TxtControlID, apimodel.TxtUnitID, apimodel.TxtServiceID, apimodel.TxtProcessIndustryID, Passalt);
                    if (result != null)
                    {
                        xmlInputData = result.GetXml();
                        nicmodel.NICXmlInputTable = xmlInputData;
                        SWPXmlDeserializer xmlSerializer = new SWPXmlDeserializer();
                        //SWPTableViewModel naDataSet = xmlSerializer.Deserialize<SWPTableViewModel>(xmlInputData);
                        NewDataSet naDataSet = xmlSerializer.Deserialize<NewDataSet>(xmlInputData);
                        if (naDataSet.Table != null)
                        {
                            naDataSet.Table.ServiceID = apimodel.TxtServiceID;
                            HttpContext.Current.Session["WBasicDetailsNIC"] = naDataSet;
                            //nicmodel.SWPApiBasicModel = naDataSet; //bind swptabledata
                            nicmodel.SWPApiXMLData = naDataSet; //bind swptabledata
                            nicmodel.FlagId = SWPReturnTypeId.Success;
                            return nicmodel;
                        }
                        else
                        {
                            nicmodel.FlagId = SWPReturnTypeId.Failed;
                            return nicmodel;
                        }
                    }
                    else
                    {
                        nicmodel.FlagId = SWPReturnTypeId.NotExist;
                        return nicmodel;
                    }
                }
            }
            else
            {
                nicmodel.FlagId = SWPReturnTypeId.NotExist;
            }
            return nicmodel;
        }
    }
}
