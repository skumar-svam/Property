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
        static string IsSWPTestApi = ConfigurationManager.AppSettings["IsSWPTestApi"];
        static string servicePassalt = ConfigurationManager.AppSettings["IsSWPTestApi"] == "true" ? ConfigurationManager.AppSettings["nictestpassalt"] : ConfigurationManager.AppSettings["ServicePassalt"];
        static string SWPAPI = ConfigurationManager.AppSettings["IsSWPTestApi"] == "true" ? ConfigurationManager.AppSettings["SWPTestApiHttpBinding"] : ConfigurationManager.AppSettings["SWPApiHttpBinding"];
       
        public SWPStatusViewModel GetSWPServiceStatus(SWPStatusViewModel apimodel)
        {
            try
            {
                apimodel.ProcessIndustryId = string.IsNullOrEmpty(apimodel.ProcessIndustryId) ? string.Empty : apimodel.ProcessIndustryId;
                apimodel.ApplicationId = string.IsNullOrEmpty(apimodel.ApplicationId) ? "Other" : apimodel.ApplicationId;
                apimodel.FeeAmount = string.IsNullOrEmpty(apimodel.FeeAmount) ? string.Empty : apimodel.FeeAmount;
                apimodel.FeeStatus = string.IsNullOrEmpty(apimodel.FeeStatus) ? "Other" : apimodel.FeeStatus;
                apimodel.TransactionId = string.IsNullOrEmpty(apimodel.TransactionId) ? string.Empty : apimodel.TransactionId;
                apimodel.TransactionDate = string.IsNullOrEmpty(apimodel.TransactionDate) ? string.Empty : apimodel.TransactionDate;
                apimodel.TransactionDateTime = string.IsNullOrEmpty(apimodel.TransactionDateTime) ? string.Empty : apimodel.TransactionDateTime;
                apimodel.PendancyLevel = string.IsNullOrEmpty(apimodel.PendancyLevel) ? "Other" : apimodel.PendancyLevel;
                apimodel.Remarks = string.IsNullOrEmpty(apimodel.Remarks) ? string.Empty : apimodel.Remarks;

                apimodel.NOCCertificateNo = string.IsNullOrEmpty(apimodel.NOCCertificateNo) ? string.Empty : apimodel.NOCCertificateNo;
                apimodel.NOCUrl = string.IsNullOrEmpty(apimodel.NOCUrl) ? string.Empty : apimodel.NOCUrl;
                apimodel.NOCUrlActiveStatus = string.IsNullOrEmpty(apimodel.NOCUrlActiveStatus) ? string.Empty : apimodel.NOCUrlActiveStatus;
                apimodel.ObjectionOrRejectionCode = string.IsNullOrEmpty(apimodel.ObjectionOrRejectionCode) ? string.Empty : apimodel.ObjectionOrRejectionCode;
                apimodel.IsCertificateValidLifeTime = string.IsNullOrEmpty(apimodel.IsCertificateValidLifeTime) ? string.Empty : apimodel.IsCertificateValidLifeTime;
                apimodel.CertificateExpiryDate = string.IsNullOrEmpty(apimodel.CertificateExpiryDate) ? string.Empty : apimodel.CertificateExpiryDate;
                
                string D1 = apimodel.DepartmentId == 1 ? apimodel.Department : string.Empty;
                string D2 = apimodel.DepartmentId == 2 ? apimodel.Department : string.Empty;
                string D3 = apimodel.DepartmentId == 3 ? apimodel.Department : string.Empty;
                string D4 = apimodel.DepartmentId == 4 ? apimodel.Department : string.Empty;
                string D5 = apimodel.DepartmentId == 5 ? apimodel.Department : string.Empty;
                string D6 = apimodel.DepartmentId == 6 ? apimodel.Department : string.Empty;
                string D7 = apimodel.DepartmentId == 7 ? apimodel.Department : string.Empty;
                string path = string.Empty;
                string xmlInputData = string.Empty;
                string xmlOutputData = string.Empty;
                
                if (IsSWPTestApi == "true")
                {
                    using (var client = new NA.PMS.NICService.NiveshMitraApiTestService.upswp_niveshmitraservicesSoapClient(SWPAPI))
                    {
                        string result = client.WReturn_CUSID_STATUS(apimodel.ControlId, apimodel.UnitId, apimodel.ServiceId, apimodel.ProcessIndustryId,
                            apimodel.ApplicationId, apimodel.StatusCode, apimodel.Remarks,apimodel.PendancyLevel, apimodel.FeeAmount, apimodel.FeeStatus, apimodel.TransactionId,
                            apimodel.TransactionDate, apimodel.TransactionDateTime, apimodel.NOCCertificateNo, apimodel.NOCUrl, apimodel.NOCUrlActiveStatus,
                            servicePassalt, apimodel.RequestId, apimodel.ObjectionOrRejectionCode, apimodel.IsCertificateValidLifeTime, apimodel.CertificateExpiryDate,
                            D1, D2, D3, D4, D5, D6, D7);
                        apimodel.Status = result;
                        return apimodel;
                    }
                }
                else
                {
                    //using (var client = new upswp_niveshmitraservicesSoapClient(SWPAPI))
                    //{
                    //    string result = client.WReturn_CUSID_STATUS(apimodel.ControlId, apimodel.UnitId, apimodel.ServiceId, apimodel.ProcessIndustryId, apimodel.ApplicationId, apimodel.StatusCode, apimodel.Remarks, apimodel.FeeAmount, apimodel.FeeStatus, apimodel.TransactionId, apimodel.TransactionDate, apimodel.TransactionDateTime, apimodel.NOCCertificateNo, apimodel.NOCUrl, apimodel.NOCUrlActiveStatus, servicePassalt);
                    //    apimodel.Status = result;
                    //    return apimodel;
                    //}
                    using (var client = new NA.PMS.NICService.NiveshMitraApiServices.upswp_niveshmitraservicesSoapClient(SWPAPI))
                    {
                        string result = client.WReturn_CUSID_STATUS(apimodel.ControlId, apimodel.UnitId, apimodel.ServiceId, apimodel.ProcessIndustryId,
                            apimodel.ApplicationId, apimodel.StatusCode, apimodel.Remarks, apimodel.PendancyLevel, apimodel.FeeAmount, apimodel.FeeStatus, apimodel.TransactionId,
                            apimodel.TransactionDate, apimodel.TransactionDateTime, apimodel.NOCCertificateNo, apimodel.NOCUrl, apimodel.NOCUrlActiveStatus,
                            servicePassalt, apimodel.RequestId, apimodel.ObjectionOrRejectionCode, apimodel.IsCertificateValidLifeTime, apimodel.CertificateExpiryDate,
                            D1, D2, D3, D4, D5, D6, D7);
                        apimodel.Status = result;
                        return apimodel;
                    }
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

        public NewDataSet GetSWPPaymentDetails(SWPStatusViewModel model)
        {
            System.Data.DataSet _newDataSet = new System.Data.DataSet();
            NewDataSet swpTabel = new NewDataSet();
            try
            {
                string path = string.Empty;
                string xmlInputData = string.Empty;
                string xmlOutputData = string.Empty;
                if (IsSWPTestApi == "true")
                {
                    using (var client = new NA.PMS.NICService.NiveshMitraApiTestService.upswp_niveshmitraservicesSoapClient(SWPAPI))
                    {
                        _newDataSet = client.WGetUBPaymentDetails(model.ControlId, model.UnitId, model.ServiceId, servicePassalt,model.RequestId);
                        xmlInputData = _newDataSet.GetXml();
                        SWPXmlDeserializer objHelp = new SWPXmlDeserializer();
                        swpTabel = objHelp.Deserialize<NewDataSet>(xmlInputData);
                        return swpTabel;
                    }
                }
                else
                {
                    //using (var client = new upswp_niveshmitraservicesSoapClient(SWPAPI))
                    //{
                    //    _newDataSet = client.WGetUBPaymentDetails(model.ControlId, model.UnitId, model.ServiceId, servicePassalt);
                    //    xmlInputData = _newDataSet.GetXml();
                    //    SWPXmlDeserializer objHelp = new SWPXmlDeserializer();
                    //    swpTabel = objHelp.Deserialize<NewDataSet>(xmlInputData);
                    //    return swpTabel;
                    //}
                    using (var client = new NA.PMS.NICService.NiveshMitraApiServices.upswp_niveshmitraservicesSoapClient(SWPAPI))
                    {
                        _newDataSet = client.WGetUBPaymentDetails(model.ControlId, model.UnitId, model.ServiceId, servicePassalt, model.RequestId);
                        xmlInputData = _newDataSet.GetXml();
                        SWPXmlDeserializer objHelp = new SWPXmlDeserializer();
                        swpTabel = objHelp.Deserialize<NewDataSet>(xmlInputData);
                        return swpTabel;
                    }
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
                nicmodel.NICRequestId = apimodel.TxtRequestID;
                nicmodel.AppType = SWPConstant.NIC;
                nicmodel.IsFromNIC = true;
                //using (var client = new upswp_niveshmitraservicesSoapClient("upswp_niveshmitraservicesSoap_nic"))

                if (IsSWPTestApi == "true")
                {
                    using (var client = new NA.PMS.NICService.NiveshMitraApiTestService.upswp_niveshmitraservicesSoapClient(SWPAPI))
                    {
                        //string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
                        System.Data.DataSet result = client.WGetBasicDetails(apimodel.TxtControlID, apimodel.TxtUnitID, apimodel.TxtServiceID, apimodel.TxtProcessIndustryID, servicePassalt);
                        if (result != null)
                        {
                            xmlInputData = result.GetXml();
                            nicmodel.NICXmlInputTable = xmlInputData;
                            SWPXmlDeserializer xmlSerializer = new SWPXmlDeserializer();
                            NewDataSet naDataSet = xmlSerializer.Deserialize<NewDataSet>(xmlInputData);
                            if (naDataSet.Table != null)
                            {
                                naDataSet.Table.ServiceID = apimodel.TxtServiceID;
                                HttpContext.Current.Session["WBasicDetailsNIC"] = naDataSet;
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
                    //using (var client = new upswp_niveshmitraservicesSoapClient(SWPAPI))
                    //{
                    //    //string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
                    //    System.Data.DataSet result = client.WGetBasicDetails(apimodel.TxtControlID, apimodel.TxtUnitID, apimodel.TxtServiceID, apimodel.TxtProcessIndustryID, servicePassalt);
                    //    if (result != null)
                    //    {
                    //        xmlInputData = result.GetXml();
                    //        nicmodel.NICXmlInputTable = xmlInputData;
                    //        SWPXmlDeserializer xmlSerializer = new SWPXmlDeserializer();
                    //        //SWPTableViewModel naDataSet = xmlSerializer.Deserialize<SWPTableViewModel>(xmlInputData);
                    //        NewDataSet naDataSet = xmlSerializer.Deserialize<NewDataSet>(xmlInputData);
                    //        if (naDataSet.Table != null)
                    //        {
                    //            naDataSet.Table.ServiceID = apimodel.TxtServiceID;
                    //            HttpContext.Current.Session["WBasicDetailsNIC"] = naDataSet;
                    //            //nicmodel.SWPApiBasicModel = naDataSet; //bind swptabledata
                    //            nicmodel.SWPApiXMLData = naDataSet; //bind swptabledata
                    //            nicmodel.FlagId = SWPReturnTypeId.Success;
                    //            return nicmodel;
                    //        }
                    //        else
                    //        {
                    //            nicmodel.FlagId = SWPReturnTypeId.Failed;
                    //            return nicmodel;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        nicmodel.FlagId = SWPReturnTypeId.NotExist;
                    //        return nicmodel;
                    //    }
                    //}
                    using (var client = new NA.PMS.NICService.NiveshMitraApiServices.upswp_niveshmitraservicesSoapClient(SWPAPI))
                    {
                        //string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
                        System.Data.DataSet result = client.WGetBasicDetails(apimodel.TxtControlID, apimodel.TxtUnitID, apimodel.TxtServiceID, apimodel.TxtProcessIndustryID, servicePassalt);
                        if (result != null)
                        {
                            xmlInputData = result.GetXml();
                            nicmodel.NICXmlInputTable = xmlInputData;
                            SWPXmlDeserializer xmlSerializer = new SWPXmlDeserializer();
                            NewDataSet naDataSet = xmlSerializer.Deserialize<NewDataSet>(xmlInputData);
                            if (naDataSet.Table != null)
                            {
                                naDataSet.Table.ServiceID = apimodel.TxtServiceID;
                                HttpContext.Current.Session["WBasicDetailsNIC"] = naDataSet;
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
            }
            else
            {
                nicmodel.FlagId = SWPReturnTypeId.NotExist;
            }
            return nicmodel;
        }

        public SWPStatusViewModel GetSWPSchemeFormPaymentDetail(SWPStatusViewModel model)
        {
            System.Data.DataSet _newDataSet = new System.Data.DataSet();
            NewDataSet swpTabel = new NewDataSet();
            try
            {
                string path = string.Empty;
                string xmlInputData = string.Empty;
                string xmlOutputData = string.Empty;
                if (IsSWPTestApi == "true")
                {
                    using (var client = new NA.PMS.NICService.NiveshMitraApiTestService.upswp_niveshmitraservicesSoapClient(SWPAPI))
                    {
                        _newDataSet = client.WGetUBPaymentDetails(model.ControlId, model.UnitId, model.ServiceId, servicePassalt, model.RequestId);
                        xmlInputData = _newDataSet != null ? _newDataSet.GetXml() : string.Empty;
                        SWPXmlDeserializer _xmlDesializer = new SWPXmlDeserializer();
                        swpTabel = !string.IsNullOrEmpty(xmlInputData) ? _xmlDesializer.Deserialize<NewDataSet>(xmlInputData) : null;
                        model.NewDataSet = swpTabel;
                        return model;
                    }
                }
                else
                {
                    //using (var client = new upswp_niveshmitraservicesSoapClient(SWPAPI))
                    //{
                    //    _newDataSet = client.WGetUBPaymentDetails(model.ControlId, model.UnitId, model.ServiceId, servicePassalt);
                    //    xmlInputData = _newDataSet != null ? _newDataSet.GetXml() : string.Empty;
                    //    SWPXmlDeserializer _xmlDesializer = new SWPXmlDeserializer();
                    //    swpTabel = !string.IsNullOrEmpty(xmlInputData) ? _xmlDesializer.Deserialize<NewDataSet>(xmlInputData) : null;
                    //    model.NewDataSet = swpTabel;
                    //    return model;
                    //}
                    using (var client = new NA.PMS.NICService.NiveshMitraApiServices.upswp_niveshmitraservicesSoapClient(SWPAPI))
                    {
                        _newDataSet = client.WGetUBPaymentDetails(model.ControlId, model.UnitId, model.ServiceId, servicePassalt, model.RequestId);
                        xmlInputData = _newDataSet != null ? _newDataSet.GetXml() : string.Empty;
                        SWPXmlDeserializer _xmlDesializer = new SWPXmlDeserializer();
                        swpTabel = !string.IsNullOrEmpty(xmlInputData) ? _xmlDesializer.Deserialize<NewDataSet>(xmlInputData) : null;
                        model.NewDataSet = swpTabel;
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessage = ex.ToString();
                return model;
                throw ex;
            }
        }

        public SWPPostViewModel GetNiveshMitraPostBackForm(SWPPostViewModel model)
        {
            string IsSWPTestApi = ConfigurationManager.AppSettings["IsSWPTestApi"];
            string passalt = ConfigurationManager.AppSettings["IsSWPTestApi"] == "true" ? ConfigurationManager.AppSettings["nictestpassalt"] : ConfigurationManager.AppSettings["ServicePassalt"];
            string swpBackUrl = ConfigurationManager.AppSettings["IsSWPTestApi"] == "true" ? ConfigurationManager.AppSettings["NiveshMitraPortalTest"] : ConfigurationManager.AppSettings["NiveshMitraPortalProd"];

            model.TxtControlID = string.IsNullOrEmpty(model.TxtControlID) ? string.Empty : SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", model.TxtControlID);
            model.TxtUnitID = string.IsNullOrEmpty(model.TxtUnitID) ? string.Empty : SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", model.TxtUnitID);
            model.TxtServiceID = string.IsNullOrEmpty(model.TxtServiceID) ? string.Empty : SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", model.TxtServiceID);
            model.TxtApplicationID = string.IsNullOrEmpty(model.TxtApplicationID) ? string.Empty : SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", model.TxtApplicationID);
            model.TxtProcessIndustryID = string.IsNullOrEmpty(model.TxtProcessIndustryID) ? string.Empty : SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", model.TxtProcessIndustryID);
            model.TxtDepartmentID = string.IsNullOrEmpty(model.TxtDepartmentID) ? string.Empty : SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", model.TxtDepartmentID);
            model.PassSalt = string.IsNullOrEmpty(model.PassSalt) ? SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", passalt) : SWPEncryption.EncryptString("ABCDEFGHIJKLMNOP", model.TxtDepartmentID);
            model.SWPBackUrl = swpBackUrl;

            string formcontent = "<form action='" + swpBackUrl+"' " + "method='post' id='NicPostBackForm'><div class='col-md-12'>"+
                "<input type='text' class='form-control' name='Dept_Code' id='TxtDept_Code' value='" + model.TxtDepartmentID + "' />" +
                "<input type='text' class='form-control' name='ControlID' id='TxtControlID' value='" + model.TxtControlID + "' />" +
                "<input type='text' class='form-control' name='UnitID' id='TxtUnitID' value='" + model.TxtUnitID + "' />" +
                "<input type='text' class='form-control' name='ServiceID' id='TxtServiceID' value='" + model.TxtServiceID + "' />" +
                "<input type='text' class='form-control' name='PassSalt' id='TxtPassSalt' value='" + model.PassSalt + "' />" +
                "<input type='submit' class='btn btn-default' name='btn-nicback-submit' id='btn-nicback-submit' value='submit'/>" +
                "</div></form>";

            model.HtmlBackForm = formcontent;
            return model;
        }
    }
}
