using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Service;
using NA.PMS.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Net;
using System.IO;
using System.Configuration;
using NA.PMS.Common;

namespace NA.PMS.Web.Areas.Member
{
    public class CitizenRequestsController : WebBaseController
    {
        ICitizenRequestsService _citizenRequestsService;
        IGeneralService _generalService;

        public CitizenRequestsController(ICitizenRequestsService citizenRequestsService, IGeneralService generalService)
        {
            _citizenRequestsService = citizenRequestsService;
            _generalService = generalService;
        }

        public ActionResult ManageCitizenRequests()
        {
            return View();
        }

        public JsonResult GetCitizenRequests([DataSourceRequest] DataSourceRequest req)
        {
            var data = _citizenRequestsService.GetCitizenRequests(req);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewRequestDetails(int id)
        {
            var details = _citizenRequestsService.GetRequestDetailsById(id);
            return View(details);
        }

        public ActionResult GetServiceRequestStatus()
        {
            List<DDList> statusList = _citizenRequestsService.GetServiceRequestStatus();
            return Json(statusList, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult SaveRequestDetails(int id, decimal? serviceFee, decimal? duesAmnt, string comment, int temp)
        //{
        //    var flag = _citizenRequestsService.SaveRequestDetails(id, serviceFee, duesAmnt, comment, temp);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult SaveRequestDetails()
        {
            int id = Convert.ToInt32(Request["id"]);
            decimal? serviceFee = null;
            if (!string.IsNullOrEmpty(Request["serviceFee"]))
                serviceFee = Convert.ToDecimal(Request["serviceFee"]);
            decimal? duesAmnt = null;
            if (!string.IsNullOrEmpty(Request["duesAmnt"]))
                duesAmnt = Convert.ToDecimal(Request[""]);
            string comment = Request["comment"];
            int temp = Convert.ToInt32(Request["temp"]);
            string dispatchNo = Request["dispatchNo"];
            DateTime? dispatchDate = null;
            if (!string.IsNullOrEmpty(Request["dispatchDate"]))
                dispatchDate = Convert.ToDateTime(Request["dispatchDate"]);
            string rID = Request["rID"];
            var flag = _citizenRequestsService.SaveRequestDetails(id, serviceFee, duesAmnt, comment, temp, dispatchNo, dispatchDate);
            if (flag == true)
            {
                if (Request.Files.Count > 0)
                {
                    var hpf = Request.Files[0];
                    if (hpf != null && hpf.ContentLength > 0)
                    {
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(hpf.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(hpf.ContentLength);
                        }
                        var fileName = new FileInfo(hpf.FileName).Name;
                        UploadDocument(rID, id, fileName, fileData);
                        _citizenRequestsService.SaveFileDetails(ConfigurationManager.AppSettings["CompleteDPath"] + rID + "/" + id + "/Dispatch", fileName, id);
                    }
                }
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetDocumentDetails([DataSourceRequest] DataSourceRequest req, int rid, int id)
        //{
        //    var lst = _citizenRequestsService.GetDocumentDetails(req, rid, id);
        //    string path = System.Configuration.ConfigurationManager.AppSettings["RootPath"] + "\\" + rid + "\\ServiceRequest" + "\\" +id;
        //    List<string> allFilesOld = DirSearch(path);
        //    foreach (string file in allFilesOld)
        //    {
        //        string str = file;
        //        //if (str.Contains(" "))
        //        //{
        //        //    str = str.Replace(" ", "");
        //        //    RenameFiles(file, str, path);
        //        //}
        //    }
        //    List<string> allFiles = DirSearch(path);
        //    List<PropertyDocument> lstDocumentDetail = new List<PropertyDocument>();
        //    foreach (string file in allFiles)
        //    {
        //        foreach (var item in lst)
        //        {
        //            string str = file;
        //            if (file.ToLower() == item.DocumentName.ToLower())
        //            {
        //                string str1 = string.Empty;
        //                //str1 = str.Substring(0, str.Length - 4);
        //                //str1 = str1.Substring(9, str1.Length - 9);
        //                PropertyDocument objDocumentDetail = new PropertyDocument();

        //                objDocumentDetail.DocumentPath = "ftp:\\" + System.Configuration.ConfigurationManager.AppSettings["FTPUsername"] + ":" + System.Configuration.ConfigurationManager.AppSettings["FTPPassword"] + "@" + System.Configuration.ConfigurationManager.AppSettings["SiteRootPath"] + "//" + rid + "//" + file;
        //                objDocumentDetail.DocumentName = item.DocumentType;
        //                objDocumentDetail.RID = rid;
        //                lstDocumentDetail.Add(objDocumentDetail);
        //            }
        //        }
        //    }
        //    var lstdocument = lstDocumentDetail.ToDataSourceResult(req);
        //    return Json(lstdocument, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetDocumentDetails([DataSourceRequest] DataSourceRequest req, int rid = 0, int id = 0)
        public JsonResult GetDocumentDetails([DataSourceRequest] DataSourceRequest req, int? rid, int? id)
        {
            FtpHandler ObjFtpHandler = new FtpHandler();
            //_propertyService = new PropertyService();
            //var lst = _propertyService.GetDocumentDetails(req, rid, id);
            //string path = System.Configuration.ConfigurationManager.AppSettings["RootPath"] + "\\" + rid + "\\ServiceRequest" + "\\" + id;
            //List<string> allFilesOld = DirSearch(path);
            //foreach (string file in allFilesOld)
            //{
            //    string str = file;
            //    //if (str.Contains(" "))
            //    //{
            //    //    str = str.Replace(" ", "");
            //    //    RenameFiles(file, str, path);
            //    //}
            //}
            //List<string> allFiles = DirSearch(path);
            //List<PropertyDocument> lstDocumentDetail = new List<PropertyDocument>();
            //foreach (string file in allFiles)
            //{
            //    foreach (var item in lst)
            //    {
            //        string str = file;
            //        if (file.ToLower() == item.DocumentName.ToLower())
            //        {
            //            string str1 = string.Empty;
            //            //str1 = str.Substring(0, str.Length - 4);
            //            //str1 = str1.Substring(9, str1.Length - 9);
            //            PropertyDocument objDocumentDetail = new PropertyDocument();

            //            objDocumentDetail.DocumentPath = "ftp:\\" + System.Configuration.ConfigurationManager.AppSettings["FTPUsername"] + ":" + System.Configuration.ConfigurationManager.AppSettings["FTPPassword"] + "@" + System.Configuration.ConfigurationManager.AppSettings["SiteRootPath"] + "//" + rid + "//" + file;
            //            objDocumentDetail.DocumentName = item.DocumentType;
            //            objDocumentDetail.RegistrationId = rid;
            //            lstDocumentDetail.Add(objDocumentDetail);
            //        }
            //    }
            //}
            //return Json(lstDocumentDetail, JsonRequestBehavior.AllowGet);
            //_propertyService = new PropertyService();
            //var lst = _propertyService.GetDocumentDetails(req, rid, id);
            string path = ObjFtpHandler.GetDocumentPathForCustomer(rid, id, string.Empty, true);//System.Configuration.ConfigurationManager.AppSettings["FTPRootPath"] + "\\" + rid + "\\" + id;
            //List<string> allFilesOld = DirSearch(path);
            //foreach (string file in allFilesOld)
            //{
            //    string str = file;
            //    //if (str.Contains(" "))
            //    //{
            //    //    str = str.Replace(" ", "");
            //    //    RenameFiles(file, str, path);
            //    //}
            //}
            List<string> allFiles = ObjFtpHandler.DirSearch(path);
            List<PropertyDocument> lstDocumentDetail = new List<PropertyDocument>();
            foreach (string file in allFiles)
            {
                //foreach (var item in lst)
                //{
                string str = file;
                //if (file.ToLower() == item.DocumentName.ToLower())
                //{
                string str1 = string.Empty;
                //str1 = str.Substring(0, str.Length - 4);
                //str1 = str1.Substring(9, str1.Length - 9);
                PropertyDocument objDocumentDetail = new PropertyDocument();

                //objDocumentDetail.DocumentPath = "ftp:\\" + System.Configuration.ConfigurationManager.AppSettings["FTPUsername"] + ":" + System.Configuration.ConfigurationManager.AppSettings["FTPPassword"] + "@" + System.Configuration.ConfigurationManager.AppSettings["SiteRootPath"] + "//" + rid + "//" + file;
                //objDocumentDetail.DocumentPath = "ftp:\\" + System.Configuration.ConfigurationManager.AppSettings["FTPUsername"] + ":" + System.Configuration.ConfigurationManager.AppSettings["FTPPassword"] + "@" + System.Configuration.ConfigurationManager.AppSettings["FTPSiteRootPath"] + "//" + rid + "//" + id + "//" + file;
                objDocumentDetail.DocumentPath = ObjFtpHandler.GetDocumentPathForCustomer(rid, id, file, false);// "http://" + System.Configuration.ConfigurationManager.AppSettings["FTPSiteRootPath"] + "//" + rid + "//" + id + "//" + file;
                objDocumentDetail.DocumentName = file;
                objDocumentDetail.RID = rid.Value;
                lstDocumentDetail.Add(objDocumentDetail);
                //}
                //}
            }
            var lstdocument = lstDocumentDetail.ToDataSourceResult(req);
            return Json(lstdocument, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// Used for searching Files in FTP Server.
        ///// </summary>
        ///// <param name="path"></param>
        ///// <returns></returns>
        //private List<string> DirSearch(string path)
        //{
        //    List<String> files = new List<String>();
        //    try
        //    {
        //        var request = CreateRequest(path, WebRequestMethods.Ftp.ListDirectory);
        //        using (var response = (FtpWebResponse)request.GetResponse())
        //        {
        //            using (var stream = response.GetResponseStream())
        //            {
        //                using (var reader = new StreamReader(stream, true))
        //                {
        //                    while (!reader.EndOfStream)
        //                    {
        //                        files.Add(reader.ReadLine());
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (System.Exception excpt)
        //    {
        //    }

        //    return files;
        //}

        //private FtpWebRequest CreateRequest(string uri, string method)
        //{
        //    var r = (FtpWebRequest)WebRequest.Create(uri);

        //    r.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["FTPUsername"], System.Configuration.ConfigurationManager.AppSettings["FTPPassword"]);
        //    r.Method = method;
        //    return r;
        //}

        public ActionResult CitizenChallan(string regId, string refid)
        {
            //BankAccountManagementModel model = new BankAccountManagementModel();
            var rid = Convert.ToInt32(CommonHelper.Decode(regId));
            var referenceNo = Convert.ToInt32(CommonHelper.Decode(refid));
            BankAccountManagementModel model = _generalService.GetPropertyDetailByRid(rid, referenceNo);
            //model.RId = rid;// Convert.ToInt32(CommonHelper.Decode(regId));
            //model.ReferenceNo = Convert.ToInt32(CommonHelper.Decode(refid));

            return View(model);
        }
        public ActionResult GetServiceRequestsByDepartment(int department)
        {
            List<DDList> services = _generalService.GetServiceRequestsByDepartment(department);
            return Json(services, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ServiceRequests()
        {
            var obj = new CitizenServiceRequestModel();
            obj.transReq = new TransferRequestModel();
            return View(obj);
        }

        public JsonResult GetAllServices()
        {
            var lst = _citizenRequestsService.GetAllServices();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        private void BindHtmlForUpload(int ServiceRequestId)
        {
            //ManageCitizenService objManageCitizenService = new ManageCitizenService();
            var DocumentLst = _citizenRequestsService.GetCheckListDocumentMentsByServiceId_DepartmentId(ServiceRequestId);
            string divMain = string.Empty;
            foreach (var item in DocumentLst)
            {
                divMain = divMain + "<div class='gServiceTableRow'><div class='gServiceTableCell'><span>" + item.ChkDocumentId
                                       + "</span></div><div class='gServiceTableCell'><span>" + item.ChkDocumentName
                                       + "</span></div><div class='gServiceTableCell'><input type='file' class='single' name='files' /></div></div>";
            }
            ViewBag.uploadhtml = divMain;

            //put in session
            Session["DocumentLst"] = DocumentLst;
            //return Json(divMain, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CitizenService(int? id)
        {
            var propDetails = _citizenRequestsService.GetServiceRequestDetails((int)id);
            BindHtmlForUpload((int)id);
            return View("CitizenService", propDetails);
        }

        public JsonResult SaveServiceRequest(int rID, int department, int serviceType, string description)
        {
            var lst = _citizenRequestsService.SaveServiceRequest(rID, department, serviceType, description);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetTransferServiceReq(int RID)
        //{
        //    var det = _citizenRequestsService.GetTransferServiceReq(RID);
        //    return Json(det, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetRentServiceReq(int RID)
        //{
        //    var det = _citizenRequestsService.GetRentServiceReq(RID);
        //    return Json(det, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetTransferServiceReqByRid([DataSourceRequest] DataSourceRequest Req, int RID)
        {
            var det = _citizenRequestsService.GetTransferServiceReq(Req, RID);
            return Json(det, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRentServiceReqByRid([DataSourceRequest] DataSourceRequest Req, int RID)
        {
            var det = _citizenRequestsService.GetRentServiceReq(Req, RID);
            return Json(det, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGetCICServiceReqByRid([DataSourceRequest] DataSourceRequest Req, int RID)
        {
            var det = _citizenRequestsService.GetCICServiceReq(Req, RID);
            return Json(det, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMortgageServiceReqByRid([DataSourceRequest] DataSourceRequest Req, int RID)
        {
            var det = _citizenRequestsService.GetMortgageServiceReq(Req, RID);
            return Json(det, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExtensionServiceReqByRid([DataSourceRequest] DataSourceRequest Req, int RID)
        {
            var det = _citizenRequestsService.GetExtensionServiceReq(Req, RID);
            return Json(det, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentList()
        {
            var depts = _generalService.GetAllDepartments();
            return Json(depts, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceRequestStatusList()
        {
            var depts = _generalService.GetServiceRequestStatusList();
            return Json(depts, JsonRequestBehavior.AllowGet);
        }

        private bool FTPDirectoryExists(string FtpPath, string DirectoryToCreate, string FTPUserName, string FTPUserPassword)
        {
            bool directoryExists;
            FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(new Uri(FtpPath + "/" + DirectoryToCreate));
            ftpClient.Credentials = new System.Net.NetworkCredential(FTPUserName.Normalize(), FTPUserPassword.Normalize());
            ftpClient.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;
            try
            {
                using (ftpClient.GetResponse())
                {
                    directoryExists = true;
                }
            }
            catch (WebException)
            {
                directoryExists = false;
            }
            return directoryExists;
        }

        private bool FTPDirectoryCreate(string FtpPath, string DirectoryToCreate, string FTPUserName, string FTPUserPassword)
        {
            bool IsDirectoryCreated = false;
            FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(new Uri(FtpPath + "/" + DirectoryToCreate));
            ftpClient.Credentials = new System.Net.NetworkCredential(FTPUserName.Normalize(), FTPUserPassword.Normalize());
            ftpClient.Method = System.Net.WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                using (ftpClient.GetResponse())
                {
                    IsDirectoryCreated = true;
                }
            }
            catch (WebException)
            {
                IsDirectoryCreated = false;
            }
            return IsDirectoryCreated;
        }

        private void UploadDocument(string RegistrationId, int RequestId, string filename, byte[] fileData)
        {
            //string CompleteDPath = "ftp://52.172.186.197/UploadDocuments/";
            string CompleteDPath = ConfigurationManager.AppSettings["CompleteDPath"];
            string username = ConfigurationManager.AppSettings["FTPUsernameSave"];
            string pass = ConfigurationManager.AppSettings["FTPPasswordSave"];

            #region Directory check & create
            bool IsCreatedDirectoryRequestId = false;
            bool IsCreatedDocumentIdFolder = false;
            bool IsRequestIdDirectory = FTPDirectoryExists(CompleteDPath, RegistrationId, username, pass);
            if (IsRequestIdDirectory == false)
            {
                IsCreatedDirectoryRequestId = FTPDirectoryCreate(CompleteDPath, RegistrationId, username, pass);

            }
            else
            {
                IsCreatedDirectoryRequestId = true;
            }
            if (IsCreatedDirectoryRequestId == true)
            {
                bool IsDocumentIdDirectory = FTPDirectoryExists(CompleteDPath, RegistrationId + "/" + RequestId + "/Dispatch", username, pass);
                if (IsDocumentIdDirectory == false)
                {
                    IsCreatedDocumentIdFolder = FTPDirectoryCreate(CompleteDPath, RegistrationId + "/" + RequestId + "/Dispatch", username, pass);

                }
                else
                {
                    IsCreatedDocumentIdFolder = true;
                }
            }
            #endregion

            if (IsCreatedDirectoryRequestId == true && IsCreatedDocumentIdFolder == true)
            {
                FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(new Uri(CompleteDPath + "/" + Convert.ToString(RegistrationId) + "/" + Convert.ToString(RequestId) + "/" + filename));
                ftpClient.Credentials = new System.Net.NetworkCredential(username.Normalize(), pass.Normalize());
                ftpClient.Method = System.Net.WebRequestMethods.Ftp.UploadFile;


                ftpClient.ContentLength = fileData.Length;
                byte[] buffer = new byte[4097];
                int bytes = fileData.Length;
                int total_bytes = (int)fileData.Length;
                try
                {
                    System.IO.Stream rs = ftpClient.GetRequestStream();
                    while (total_bytes > 0)
                    {
                        rs.Write(fileData, 0, bytes);
                        total_bytes = total_bytes - bytes;
                    }
                    rs.Close();
                }
                catch (Exception e)
                {
                    throw e;
                }
                FtpWebResponse uploadResponse = (FtpWebResponse)ftpClient.GetResponse();
                var value = uploadResponse.StatusDescription;
                uploadResponse.Close();
            }
        }

        //public JsonResult GetCICServiceReq(int RID)
        //{
        //    var det = _citizenRequestsService.GetCICServiceReq(RID);
        //    return Json(det, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetMortgageServiceReq(int RID)
        //{
        //    var det = _citizenRequestsService.GetMortgageServiceReq(RID);
        //    return Json(det, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetExtensionServiceReq(int RID)
        //{
        //    var det = _citizenRequestsService.GetExtensionServiceReq(RID);
        //    return Json(det, JsonRequestBehavior.AllowGet);
        //}


    }
}