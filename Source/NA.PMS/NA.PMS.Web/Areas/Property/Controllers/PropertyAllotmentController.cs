using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Model;
using NA.PMS.Web.Controllers;
using NA.PMS.Web.Services;
using System.IO;
using System.Net;
using System.Configuration;
using System.Text;
using NoidaAuthority.PMS.Common;
using NA.PMS.Common;
using NA.PMS.Web.Models;
using NA.PMS.Web.Controllers.Common;


namespace NA.PMS.Web.Areas.Property.Controllers
{
    public class PropertyAllotmentController : WebBaseController
    {
        static string servicePassalt = ConfigurationManager.AppSettings["ServicePassalt"];
        IPropertyAllotmentService _propertyService;
        IAllotmentService _allotmentService;
        IGeneralService _generalService;
        IRequestService _requstService;
        NiveshMitraServices _niveshMitraServices;
        INICService _nicService;
        public PropertyAllotmentController(IPropertyAllotmentService propertyService, IAllotmentService allotmentService, IGeneralService generalService, IRequestService requestService,INICService nicServices)
        {
            _propertyService = propertyService;
            _allotmentService = allotmentService;
            _generalService = generalService;
            _requstService = requestService;
            _niveshMitraServices = new NiveshMitraServices();
            _nicService = nicServices;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Open manage allotment page
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageAllotment()
        {
            return View();
        }

        //public ActionResult ManageLeaseRent()
        //{
        //    return View();
        //}

        /// <summary>
        /// Add allotment details
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAllotmentDetail()
        {
            return View();
        }
        /// <summary>
        /// View particular alloted property
        /// </summary>
        /// <returns></returns>
        public ActionResult PropertyDetail(int id)
        {
            AllottedPropertyViewModel model = new AllottedPropertyViewModel();
            if (id != 0)
            {
                //model = _propertyService.GetDetailedPropertyView(rid);
                model = _propertyService.GetPropertyDetailByRegistrationId(id);
            }
            return View(model);
        }
        /// <summary>
        /// Get all alloted property list
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPropertyAllotmentList()
        {
            DataSourceRequest request = new DataSourceRequest();
            List<PropertyAllotmentModel> allotmentList = _propertyService.GetPropertyAllotmentList();
            var data = allotmentList.ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// return scheme list based on department for dropdown
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public ActionResult FilterSchemeListOnDepartment(int departmentId)
        {
            DataSourceRequest request = new DataSourceRequest();
            List<SchemeAllotmentModel> schemeList = _propertyService.FilterSchemeListOnDepartment(departmentId);
            var data = schemeList.ToDataSourceResult(request);
            return Json(schemeList, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// return departments for dropdown
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDepartmentListForAllotment()
        {
            DataSourceRequest request = new DataSourceRequest();
            List<DepartmentAllotmentModel> departmentList = _propertyService.GetDepartmentListForAllotment();
            var data = departmentList.ToDataSourceResult(request);
            return Json(departmentList, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// return scheme list for dropdown
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSchemeListForAllotment()
        {
            List<SchemeAllotmentModel> schemeList = _propertyService.GetSchemeListForAllotment();
            return Json(schemeList, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// return department based on scheme for dropdown
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public ActionResult FilterDepartmentOnScheme(int schemeId)
        {
            List<DepartmentAllotmentModel> department = _propertyService.FilterDepartmentOnScheme(schemeId);
            return Json(department, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PropertyDetails(int Id)
        {
            DetailedPropertyView detailedPropertyView = new DetailedPropertyView();
            if (Id != 0)
            {
                detailedPropertyView = _propertyService.GetDetailedPropertyView(Id);
            }
            return View(detailedPropertyView);
        }

        public JsonResult GetKYADetails(int rId)
        {
            KYAViewModel data = _propertyService.GetKYADetails(rId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubLeasedProperty([DataSourceRequest] DataSourceRequest request, int? rid)
        {
            if (rid != null)
            {
                var details = _propertyService.GetSubLeasePropertyList(request, Convert.ToInt32(rid));
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetPaymentSchedule([DataSourceRequest] DataSourceRequest request, int rid)
        {
            var dataresult = _propertyService.GetPaymentSchedule(request, rid);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePaymentScheduleStatus(int ScheduleId, int Rid)
        {
            var dataresult = _propertyService.UpdatePaymentScheduleStatus(ScheduleId, Rid);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentReschedule([DataSourceRequest] DataSourceRequest request, int rId)
        {
            var data = _propertyService.GetPaymentReschedule(request, rId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReceiptSchedule([DataSourceRequest] DataSourceRequest request, int rid)
        {
            var dataresult = _propertyService.GetReceiptSchedule(request, rid);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// return scheduled date of property after allotment
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        public JsonResult GetScheduleDetailsForAllottedProperty(int rid)
        {
            AllottedPropertyDetails scheduleDetail = _propertyService.GetScheduleDetailsForAllottedProperty(rid);
            return Json(scheduleDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDocumentDetails([DataSourceRequest] DataSourceRequest request, int rid, int? oldRid)
        {
            FtpHandler ObjFtpHandler = new FtpHandler();
            //rid = 10000022;
            // var obj = new DocumentSync();
            //  var lstdocument = obj.GetDocumentDetails(rid).ToDataSourceResult(request);
            string path = string.Empty;
            int? passrid = 0;
            if (oldRid != null)
            {
                path = ObjFtpHandler.GetDocumentPath((int)oldRid, string.Empty, true);
                passrid = oldRid;
                if (path == null)
                {
                    path = ObjFtpHandler.GetDocumentPath(rid, string.Empty, true);
                    passrid = rid;
                }
            }
            else
            {
                path = ObjFtpHandler.GetDocumentPath(rid, string.Empty, true);
                passrid = rid;
            }
            //path = ObjFtpHandler.GetDocumentPath(rid, string.Empty, true);//System.Configuration.ConfigurationManager.AppSettings["RootPath"] + "\\" + rid;
            List<string> allFilesOld = ObjFtpHandler.DirSearch(path);
            foreach (string file in allFilesOld)
            {
                string str = file;
                if (str.Contains(" "))
                {
                    str = str.Replace(" ", "");
                    ObjFtpHandler.RenameFiles(file, str, path);
                }
            }
            List<string> allFiles = ObjFtpHandler.DirSearch(path);
            List<PropertyDocument> lstDocumentDetail = new List<PropertyDocument>();
            foreach (string file in allFiles)
            {
                string str = file;
                string str1 = string.Empty;
                if ((str.Split('-')).Length > 1)
                {
                    str1 = str.Substring(0, str.Length - 4);
                    str1 = str1.Substring(9, str1.Length - 9);
                }
                PropertyDocument objDocumentDetail = new PropertyDocument();
                // objDocumentDetail.DocumentPath = "ftp:\\" + System.Configuration.ConfigurationManager.AppSettings["FTPUsername"] + ":" + System.Configuration.ConfigurationManager.AppSettings["FTPPassword"] + "@" + System.Configuration.ConfigurationManager.AppSettings["SiteRootPath"] + "//" + rid + "//" + file;
                objDocumentDetail.DocumentPath = ObjFtpHandler.GetDocumentPath((int)passrid, file, false);// "http://" + System.Configuration.ConfigurationManager.AppSettings["SiteRootPath"] + "//" + rid + "//" + file;
                objDocumentDetail.DocumentName = !(string.IsNullOrEmpty(str1)) ? (!(string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[str1])) ? System.Configuration.ConfigurationManager.AppSettings[str1] : "Other Documents") : "Other Documents";
                //objDocumentDetail.RID = rid;
                objDocumentDetail.RID = (int)passrid;
                lstDocumentDetail.Add(objDocumentDetail);
            }
            var lstdocument = lstDocumentDetail.ToDataSourceResult(request);
            return Json(lstdocument, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentListByRegistrationId([DataSourceRequest] DataSourceRequest request, int rid)
        {
            var list = _propertyService.GetDocumentListByRegistrationId(request, rid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentTypeListByDepartmentId([DataSourceRequest] DataSourceRequest request, int deptId)
        {
            var list = _propertyService.GetDocumentTypeListByDepartmentId(request, deptId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveUploadedDocument(PropertyDocument model, HttpPostedFileBase docfile)
        {
            var flag = _propertyService.SaveUploadedDocument(model, docfile);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGeneratedLetterByRegistrationId([DataSourceRequest] DataSourceRequest request, int rid)
        {
            var list = _propertyService.GetGeneratedLetterByRegistrationId(request, rid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllDocumentType()
        {
            List<DDList> schemeList = _propertyService.GetAllDocumentType();
            return Json(schemeList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPaymentLedger([DataSourceRequest] DataSourceRequest request, int rid)
        {
            var dataresult = _propertyService.GetPaymentLedger(request, rid);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLetterHistory([DataSourceRequest] DataSourceRequest request, int rid)
        {
            var dataresult = _propertyService.GetLetterHistory(request, rid);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDigitalSingedLetterHistory([DataSourceRequest] DataSourceRequest request, int? rid)
        {
            var dataresult = _requstService.GetDigitalSingedLetterHistory(request, rid);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Used for reschduling the payment plan for a given RID
        /// </summary>
        /// <param name="rId">RID</param>
        /// <returns></returns>
        public JsonResult ReschedulePayments(int rId, decimal dueAmnt)
        {
            if (dueAmnt != 0)
            {
                var flag = _propertyService.ReschedulePayments(rId, dueAmnt);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            else return Json(false, JsonRequestBehavior.AllowGet);
        }

        #region Extension...

        /// <summary>
        /// To Go on Manage Extension page
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageExtension()
        {
            return View();
        }

        /// <summary>
        /// Add Extension details
        /// </summary>
        /// <returns></returns>
        public ActionResult AddExtension()
        {
            ExtensionDetails objExtensionDetails = new ExtensionDetails();
            return View(objExtensionDetails);
        }

        public ActionResult GetExtensionDetails([DataSourceRequest] DataSourceRequest request)
        {
            var lstPropertyDetails = _propertyService.GetExtensionDetails(request);
            return Json(lstPropertyDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExtensionDetailsByRid([DataSourceRequest] DataSourceRequest request, int Rid)
        {
            var lstPropertyDetails = _propertyService.GetExtensionDetailsByRid(request, Rid);
            return Json(lstPropertyDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveExtension(int rid, int OnlineRequestRefNo, string propertyNu, DateTime completionDueDate, DateTime extensionGivenDate, decimal extensionCharge, string user)
        {
            var dataresult = _propertyService.SaveExtension(rid, OnlineRequestRefNo, propertyNu, completionDueDate, extensionGivenDate, extensionCharge, user);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPropertyDetails(int rid)
        {
            var lstPropertyDetails = _propertyService.GetPropertyDetails(rid);
            return Json(lstPropertyDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewExtension(int Id)
        {
            var lstPropertyDetails = _propertyService.GetExtensionById(Id);
            return View(lstPropertyDetails);
        }

        /// <summary>
        /// To Go on Manage Extension for Approval
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageExtensionApproval()
        {
            return View();
        }

        public ActionResult GetExtensionApprovalDetails(DataSourceRequest request)
        {
            var lstPropertyDetails = _propertyService.GetExtensionApprovalDetails(request);
            return Json(lstPropertyDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewExtensionApproval(int Id)
        {
            var lstPropertyDetails = _propertyService.GetExtensionById(Id);
            return View(lstPropertyDetails);
        }

        public ActionResult SaveCommentByID(int Id, string Comment, bool acceptReject)
        {
            var lst = _propertyService.SaveCommentByID(Id, Comment, acceptReject);
            var lstPropertyDetails = _propertyService.GetExtensionById(Id);
            if (acceptReject == true && lstPropertyDetails.OnlineRequestRefNo !=0)
            {
                SendNICStatus((int)lstPropertyDetails.OnlineRequestRefNo,Comment);
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public int SendNICStatus(int ReqNo, string comment)
        {
            var flag = ReturnType.None;
            CurrentUserDetail user = Session["CurrentUser"] as CurrentUserDetail;
            //CurrentUserDetail user = HttpContext.Current.Session["CurrentUser"] as CurrentUserDetail;
            var requestDetail = _requstService.GetServiceRequestDetailById(ReqNo);
            var nicDetails = _nicService.GetNiveshMitraServicesByReqId(ReqNo);
            var serviceStatus = _nicService.GetServiceStatusByCustomerRequestStatusId(NAStatusId.Approved);
            if (requestDetail.ServiceModel.RequestThrough == Constants.NIC_NiveshMitra && requestDetail.ServiceModel.StatusId == NAStatusId.Completed)
            {
                WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel
                {
                    ControlID = nicDetails.NICModel.TxtControlID,
                    ApplicationID = Convert.ToString(ReqNo),
                    ProcessIndustryID = nicDetails.NICModel.TxtProcessIndustryID,
                    UnitID = nicDetails.NICModel.TxtUnitID,
                    ServiceID = nicDetails.NICModel.TxtServiceID,
                    Status_Code = serviceStatus.StatusCode,
                    Remarks = "REMARKS | " + comment + " - User: " + user.FirstName + " - Status:  " + serviceStatus.StatusName + " | ",
                    Fee_Status = string.Empty,
                    Fee_Amount = string.Empty,
                    passsalt = servicePassalt
                };
                NiveshMitraServices _niveshMitraServices = new NiveshMitraServices();
                _niveshMitraServices.GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);
                flag = ReturnType.Saved;
            }
            return flag;
        }

        public ActionResult UpdateExtension(int Id, int rid, string propertyNu, DateTime completionDueDate, DateTime extensionGivenDate, decimal extensionCharge, string user)
        {
            var dataresult = _propertyService.UpdateExtension(Id, rid, propertyNu, completionDueDate, extensionGivenDate, extensionCharge, user);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CancelExtension(int Id)
        {
            var lst = _propertyService.CancelExtension(Id);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        // To Generate Mortgage letter
        public ActionResult GenerateExtensionLetter(int rid)
        {
            var lst = _propertyService.GenerateExtensionLetter(rid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllRIDs([DataSourceRequest] DataSourceRequest Req, int Rid)
        {
            var lstPropertyDetails = _propertyService.GetAllRIDs(Req, Rid);
            return Json(lstPropertyDetails, JsonRequestBehavior.AllowGet);
        }

        #endregion

        //public ActionResult AllotmentMaster( int? rid, int? department, string sector, string block, string plot, string mobileNumber, string name, string fatherName, string motherName, string address)
        public ActionResult AllotmentMaster()
        {
            //ViewBag["rid"] = rid; 
            //int? dept = department;
            //TempData["department"] = dept;
            //AdvanceSearchModel search = new AdvanceSearchModel();
            //search.Rid = rid; search.DepartmentId = department; search.SectorName = sector; search.BlockName = block; search.PlotNumber = plot; search.MobileNumber = mobileNumber;
            //search.ApplicantName = name; search.FatherOrHusbandName = fatherName; search.MotherName = motherName;
            //Session["filtersearch"] = search;
            //return View(search);
            AdvanceSearchModel model = (AdvanceSearchModel)Session["AdvanceSearch"];
            return View(model);
        }

        public JsonResult GetRegistrationIdForAdvanceSearch()
        {
            List<DynamicDataModel> data = _propertyService.GetRegistrationIdForAdvanceSearch();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentForAdvanceSearch()
        {
            List<DynamicDataModel> data = _propertyService.GetDepartmentForAdvanceSearch();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSectorsForAdvanceSearch()
        {
            List<DynamicDataModel> data = _propertyService.GetSectorsForAdvanceSearch();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBlocksForAdvanceSearch()
        {
            List<DynamicDataModel> data = _propertyService.GetBlocksForAdvanceSearch();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public bool PropertyHaveDocument(int rid, int oldRid)
        {
            bool hasDocument = false;
            FtpHandler ObjFtpHandler = new FtpHandler();
            string path = string.Empty;
            if (oldRid != 0)
            {
                path = ObjFtpHandler.GetDocumentPath(oldRid, string.Empty, true);
                if (path == null)
                {
                    path = ObjFtpHandler.GetDocumentPath(rid, string.Empty, true);
                }
            }
            else { path = ObjFtpHandler.GetDocumentPath(rid, string.Empty, true); }

            // string path = ObjFtpHandler.GetDocumentPath(rid, string.Empty, true);
            List<string> allFilesOld = ObjFtpHandler.DirSearch(path);
            if (allFilesOld.Count() > 0) { hasDocument = true; }
            return hasDocument;
        }

        public JsonResult AdvanceSearchForAllottedProperty([DataSourceRequest]DataSourceRequest req, AdvanceSearchModel objAdvanceSearchModel)
        {
            AdvanceSearchModel search;

            if (Session["AdvanceSearch"] != null)
            {
                search = (AdvanceSearchModel)Session["AdvanceSearch"];
                Session["AdvanceSearch"] = null;
            }

            var data = _propertyService.AdvanceSearchForAllottedProperty(req, objAdvanceSearchModel);
            var model = data.Data;
            var Model = model.OfType<AllotmentModel>().ToList();
            Model.ForEach(m => m.IsThisPropertyHasDocument = PropertyHaveDocument(m.RID, (int)m.OldRID));
            data.Data = Model;
            var JsonResult = Json(data, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
            //return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AdvanceSearchForAllottedPropertyII([DataSourceRequest]DataSourceRequest request, AdvanceSearchModel model)
        {
            AdvanceSearchModel search;

            if (Session["AdvanceSearch"] != null)
            {
                search = (AdvanceSearchModel)Session["AdvanceSearch"];
                Session["AdvanceSearch"] = null;
            }

            var data = _propertyService.AdvanceSearchForAllottedPropertyII(request, model);
            //var list = data.Data;
            //var Model = list.OfType<AllotmentViewModel>().ToList();
            //Model.ForEach(m => m.IsDocumentAvailable = PropertyHaveDocument(m.RegistrationId.Value));
            //data.Data = Model;
            //var JsonResult = Json(data, JsonRequestBehavior.AllowGet);
            //JsonResult.MaxJsonLength = int.MaxValue;
            //return JsonResult;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckDataForAdvanceSearch(int? rid, int? department, string sector, string block, string plot, string mobileNumber, string name, string fatherName, string motherName, string address)
        {

            var transaction = _propertyService.AdvanceSearchForAllottedProperty(rid, department, sector, block, plot, mobileNumber, name, fatherName, motherName, address);
            //var data = transaction.ToDataSourceResult(request);
            var flag = false;
            if (transaction != null)
            {
                flag = true;
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(flag, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult DownloadExcelApplicationForm(int? schemeId, int? departmentId, string formType)
        {
            Stream stream = _propertyService.DownloadExcelApplicationForm(schemeId, departmentId, formType);
            var memoryStream = stream as MemoryStream;
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=ApplicationFormList.xlsx");
            Response.BinaryWrite(memoryStream.ToArray());
            Response.Flush();
            return RedirectToAction("ManageApplication");

            //byte[] fileBytes = System.IO.File.ReadAllBytes(@"D:\NoidaAuthority\BulkApplicationFormat.xlsx");
            //byte[] fileBytes = System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["ApplicationExcelTemplate"]);

            //byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(ConfigurationManager.AppSettings["ApplicationExcelTemplate"]));
            //string fileName = "BulkApplicationFormat.xlsx";
            //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        public ActionResult BulkUploadApplicationForm()
        {
            var uploadExcel = Request.Files[0];
            int schemeId = int.Parse(Request.Form["SchemeId"]);
            int departmentId = int.Parse(Request.Form["DepartmentId"]);

            //var flag = false;
            string flag = _propertyService.UploadExcelApplicationForm(schemeId, departmentId, uploadExcel);
            //return RedirectToAction("ManageApplication");
            return Json(flag);
        }

        public ActionResult DownloadNotingDetails(string content)
        {
            string strContent = string.Empty;
            StringWriter sw = new StringWriter();
            sw.Write(content);
            //System.Data.Entity.Core.Objects.ObjectParameter param = new System.Data.Entity.Core.Objects.ObjectParameter(content, typeof(string));
            //strContent = param.Value.ToString();
            //StringBuilder sb = new StringBuilder(content);
            //DocumentModel.Load("Document.html").Save("Document.docx");
            byte[] fileBytes = System.IO.File.ReadAllBytes(sw.ToString());
            string fileName = "Noting.docx";
            //string fileName = "Noting.html";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            //return File(content, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            //return File("Noting.docx", "application/docx", content);

            //System.Text.StringBuilder strBody = new System.Text.StringBuilder(content);
            //Response.ClearContent();
            //Response.Buffer = true;
            //Response.AddHeader("content-disposition", "attachment; filename=Possession.doc");
            //Response.ContentType = "application/vnd.ms-word ";
            //Response.Charset = string.Empty;
            //StringWriter sw = new StringWriter(strBody);
            //Response.Output.Write(sw.ToString());
            //Response.Flush();
            //Response.End();
            //return RedirectToAction("ManageApplication");
            //return File("Noting.html", "application/html", content);
        }


        //public ActionResult Pdf_Export()
        //{
        //    return View();
        //}

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        //Save Property Remarks
        public JsonResult AddPropertyRemarks(RemarksDetailsModel objRemarks)
        {
            var data = _propertyService.AddRemarksForProperty(objRemarks);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchParentRid(SubLeaseViewModel objSubLeaseViewModel)
        {
            var flag = false;
            var SearchLst = _propertyService.AdvanceSearchForAllottedProperty(null, null, objSubLeaseViewModel.Sector, objSubLeaseViewModel.Block, objSubLeaseViewModel.PlotNo, null, null, null, null, null);
            if (SearchLst != null)
            {
                flag = true;
                return Json(SearchLst.FirstOrDefault(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetMasterSearchParameterAsDataSource([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var data = _generalService.GetMasterSearchParameterAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyDetailListAsDataSource([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var data = _propertyService.GetPropertyDetailListAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceRequestListByRegistrationId([DataSourceRequest]DataSourceRequest request, ServiceViewModel model)
        {
            var data = _propertyService.GetServiceRequestListByRegistrationId(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferHistoryByIdAsDataSource([DataSourceRequest]DataSourceRequest request, TransferViewModel model)
        {
            var data = _propertyService.GetTransferHistoryByIdAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMortgageHistoryByIdAsDataSource([DataSourceRequest]DataSourceRequest request, MortgageViewModel model)
        {
            var data = _propertyService.GetMortgageHistoryByIdAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExtensionHistoryByIdAsDataSource([DataSourceRequest]DataSourceRequest request, ExtensionViewModel model)
        {
            var data = _propertyService.GetExtensionHistoryByIdAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRentingHistoryByIdAsDataSource([DataSourceRequest]DataSourceRequest request, RentingViewModel model)
        {
            var data = _propertyService.GetRentingHistoryByIdAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCICHistoryByIdAsDataSource([DataSourceRequest]DataSourceRequest request, CICViewModel model)
        {
            var data = _propertyService.GetCICHistoryByIdAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFunctionalHistoryByIdAsDataSource([DataSourceRequest]DataSourceRequest request, FunctionalViewModel model)
        {
            var data = _propertyService.GetFunctionalHistoryByIdAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLeaseRentPaymentByIdAsDataSource([DataSourceRequest]DataSourceRequest request, LeaseRentViewModel model)
        {
            var data = _propertyService.GetLeaseRentPaymentByIdAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPremiumDuesPaymentByIdAsDataSource([DataSourceRequest]DataSourceRequest request, LeaseRentViewModel model)
        {
            var data = _propertyService.GetPremiumDuesPaymentByIdAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDuesPaymentStatusByRegistrationId(LeaseRentViewModel model)
        {
            var data = _propertyService.GetDuesPaymentStatusByRegistrationId(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNotingFilesByIdAsDataSource([DataSourceRequest]DataSourceRequest request, NotingViewModel model)
        {
            var data = _propertyService.GetNotingFilesByIdAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubLeasedPropertyByIdAsDataSource([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var data = _propertyService.GetSubLeasedPropertyByIdAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDocumentStatusOfProperty(PropertyViewModel model)
        {
            int flag = _propertyService.UpdateDocumentStatusOfProperty(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDuesCalcaluationHistoryAsDataSource([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var data = _propertyService.GetDuesCalcaluationHistoryAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyListAsDataSource([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var data = _propertyService.GetPropertyListAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerateNDCByRegistrationId(LetterViewModel model)
        {
            LetterViewModel letter = _generalService.GenerateNDCByRegistrationId(model);
            return Json(letter, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentListAsDataSource([DataSourceRequest]DataSourceRequest request, DropdownViewModel model)
        {
            var list = _generalService.GetDepartmentListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateUpdatedProperty(PropertyViewModel model)
        {
            int flag = _generalService.ValidateUpdatedProperty(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}