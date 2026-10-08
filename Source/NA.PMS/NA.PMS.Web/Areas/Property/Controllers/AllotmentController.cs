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
using NA.PMS.Web.Models;
using OfficeOpenXml;
using MvcSiteMapProvider;
using System.IO;
using System.Globalization;
using NA.PMS.Common;
using NA.PMS.Web.Filters;
using System.Web.UI;
using System.Configuration;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using NoidaAuthority.PMS.Common;

namespace NA.PMS.Web.Areas.Property.Controllers
{
    public class AllotmentController : WebBaseController
    {
        IGeneralService _generalService;
        IAllotmentService _allotmentService;
        IPropertyAllotmentService _propertyService;
        IMastersService _mastersService;
        IOnlineService _onlineService;
        static int menuKey = (int)Common.ScreenMenuKey.ManageAllotment;
        const string COMPANYTYPE = "Companytype";
        public string Passalt = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServicePassalt"]) ? ConfigurationManager.AppSettings["ServicePassalt"] : string.Empty;
        public AllotmentController(IGeneralService generalService, IAllotmentService allotmentService, IPropertyAllotmentService propertyService, IMastersService mastersService, IOnlineService onlineService)
        {
            _generalService = generalService;
            _allotmentService = allotmentService;
            _propertyService = propertyService;
            _mastersService = mastersService;
            _onlineService = onlineService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApplicationForm()
        {
            return View();
        }

        public ActionResult ApproveRequest()
        {
            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }

        public ActionResult Manage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        // GET: Property/Application
        public void SetRolePrmision(int menuKey)
        {
            if (menuKey != 0)
            {
                var loginUser = (CurrentUserDetail)Session["CurrentUser"];
                if (loginUser != null)
                {
                    foreach (var Role in loginUser.MenuMaster)
                    {
                        if (Role != null && Role.MenuId == menuKey)
                        {
                            ViewBag.EditMenuVal = Role.IsUpdate;
                            ViewBag.AddMenuVal = Role.IsWrite;
                            ViewBag.DeleteMenuVal = Role.Isdelete;
                            ViewBag.ReadOnlyMenu = Role.IsRead;
                        }
                    }
                }
                else
                {
                    RedirectToAction("Login", "Account", new { area = "" });
                }
            }
            else
            {
                RedirectToAction("Login", "Account", new { area = "" });
            }
        }

        #region Application form process
        public ActionResult ArchivedApplications()
        {
            return View();
        }
        public ActionResult GetArchivedApplications([DataSourceRequest]DataSourceRequest request)
        {
            var allApplications = _allotmentService.GetArchivedApplications().ToDataSourceResult(request);
            return Json(allApplications, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageApplication()
        {
            if (menuKey != 0)
            {
                var setPrmision = CommonMethords.SetRolePrmision(menuKey);
                if (setPrmision != null)
                {

                    ViewBag.EditMenuVal = setPrmision.EditMenuVal;
                    ViewBag.AddMenuVal = setPrmision.AddMenuVal;
                    ViewBag.DeleteMenuVal = setPrmision.DeleteMenuVal;
                    ViewBag.ReadOnlyMenu = setPrmision.ReadOnlyMenu;
                    if (TempData["isAdded"] != null && Convert.ToBoolean(TempData["isAdded"]) == true)
                    {
                        ViewBag.IsApplicationAdded = true;
                    }
                    if (TempData["isUpdated"] != null && Convert.ToBoolean(TempData["isUpdated"]) == true)
                    {
                        ViewBag.IsApplicationUpdated = true;
                    }
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { area = "" });
                }
            }
            else
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            //SetRolePrmision(menuKey);
            //return View();
        }
        public ActionResult AddApplication()
        {
            //Uri url = Request.UrlReferrer;
            var node = SiteMaps.Current.CurrentNode;
            //if (url != null)
            //{

            if (node != null && node.ParentNode != null)
            {
                SiteMaps.Current.CurrentNode.Title = "Add Application";
                // node.ParentNode.Title = "Add Refund";
            }
            return View();
            //}
            //else
            //{
            //    return Redirect("/Account/Unauthorized");
            //}


        }
        public ActionResult EditApplication()
        {

            return View();
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
        public ActionResult PersonalInfo()
        {
            return View();
        }

        /// <summary>
        /// Get Data for Viewing Application Form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public ActionResult ViewApplicationForm(int id)
        //{
        //    var applicationDetail = new ApplicationFormModel();
        //    applicationDetail = _allotmentService.GetAllApplications().Where(x => x.ApplicationId == id).FirstOrDefault();
        //    applicationDetail.Age = applicationDetail.DOB != null ? (DateTime.Now).Year - (Convert.ToDateTime(applicationDetail.DOB)).Year : 0;
        //    return View("ViewApplication", applicationDetail);
        //}
        public ActionResult ViewApplicationForm(string id)
        {
            var appId = Convert.ToInt32(CommonHelper.Decode(id));
            var applicationDetail = new ApplicationFormModel();
            //applicationDetail = _allotmentService.GetAllApplications().Where(x => x.ApplicationId == id).FirstOrDefault();
            applicationDetail = _allotmentService.GetApplicationFormDetailById(appId);
            applicationDetail.Age = applicationDetail.DOB != null ? (DateTime.Now).Year - (Convert.ToDateTime(applicationDetail.DOB)).Year : 0;
            return View("ViewApplication", applicationDetail);
        }

        public ActionResult PartialViewApplication(string id)
        {
            var appId = Convert.ToInt32(CommonHelper.Decode(id));
            var applicationDetail = new ApplicationFormModel();
            //applicationDetail = _allotmentService.GetAllApplications().Where(x => x.ApplicationId == id).FirstOrDefault();
            applicationDetail = _allotmentService.GetApplicationFormDetailById(appId);
            applicationDetail.Age = applicationDetail.DOB != null ? (DateTime.Now).Year - (Convert.ToDateTime(applicationDetail.DOB)).Year : 0;
            //populate a model
            return PartialView("~/Views/Shared/_ViewApplicationForm.cshtml", applicationDetail);
        }

        public ActionResult PartialViewProperty(string id)
        {
            int refId = Convert.ToInt32(CommonHelper.Decode(id));
            var lstPropModel = new List<PropertyModel>();
            var objPropModel = new PropertyModel();
            lstPropModel = _mastersService.GetPropertyDetail();
            objPropModel = lstPropModel.Where(x => x.refId == refId).FirstOrDefault();
            return PartialView("~/Views/Shared/_ViewProperty.cshtml", objPropModel);
        }

        public ActionResult EditApplicationForm(string id)
        {
            var appId = Convert.ToInt32(CommonHelper.Decode(id));
            var setPrmision = CommonMethords.SetRolePrmision(menuKey);
            if (setPrmision.EditMenuVal == true)
            {
                var applicationDetail = new ApplicationFormModel();
                //applicationDetail = _allotmentService.GetAllApplications().Where(x => x.ApplicationId == id).FirstOrDefault();
                applicationDetail = _allotmentService.GetApplicationFormDetailById(appId);
                applicationDetail.Age = applicationDetail.DOB != null ? (DateTime.Now).Year - (Convert.ToDateTime(applicationDetail.DOB)).Year : 0;
                return View("EditApplication", applicationDetail);

            }
            else
            {
                return Redirect("/Account/Unauthorized");
            }

        }

        //public ActionResult EditApplicationForm(int id)
        //{
        //    var setPrmision = CommonMethords.SetRolePrmision(menuKey);
        //       if (setPrmision.EditMenuVal == true)
        //    {
        //         var applicationDetail = new ApplicationFormModel();
        //        applicationDetail = _allotmentService.GetAllApplications().Where(x => x.ApplicationId == id).FirstOrDefault();
        //           applicationDetail.Age = applicationDetail.DOB != null ? (DateTime.Now).Year - (Convert.ToDateTime(applicationDetail.DOB)).Year : 0;
        //        return View("EditApplication", applicationDetail);

        //    }
        //       else
        //       {
        //           return Redirect("/Account/Unauthorized");
        //       }

        //}
        /// <summary>
        /// Get all application forms
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult GetAllApplications([DataSourceRequest]DataSourceRequest request)
        {
            var allApplications = _allotmentService.GetAllApplications(request);//.ToDataSourceResult(request);
            return Json(allApplications, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Add Application Form 
        /// </summary>
        /// <param name="applicationFormModel"></param>
        /// <returns></returns>
        public ActionResult SaveApplicationForm(ApplicationFormModel applicationFormModel)
        {
            bool isAdded = _allotmentService.SaveApplicationForm(applicationFormModel);
            if (applicationFormModel.ApplicationId == 0)
                TempData["isAdded"] = true;
            else
                TempData["isUpdated"] = true;
            return RedirectToAction("ManageApplication");
        }
        /// <summary>
        /// Get Personal Information of applicant on the basis of RID
        /// </summary>
        /// <param name="rId"></param>
        /// <returns></returns>
        public JsonResult GetPersonalInfo(int rId)
        {
            var data = _allotmentService.GetPersonalInfo(rId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Update Personal Info of applicant after Allotment
        /// </summary>
        /// <param name="applicationFormModel"></param>
        /// <returns></returns>
        public ActionResult UpdatePersonalInfo(ApplicationFormModel applicationFormModel)
        {
            bool isAdded = _allotmentService.UpdatePersonalInfo(applicationFormModel);
            return RedirectToAction("ManageApplication");
        }
        /// <summary>
        /// Check Form No Duplicacy. Form No should be unique with Scheme and Department wise
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <param name="formNo"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public JsonResult CheckFormNo(int schemeId, int departmentId, string formNo, int applicationId)
        {
            var flag = false;
            flag = _allotmentService.CheckFormNo(schemeId, departmentId, formNo, applicationId);
            return Json(flag);
        }

        /// <summary>
        /// Check CheckIssue Date. It should be in between Scheme's start date and end date
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public JsonResult CheckIssueDate(int schemeId, string issueDate)
        {
            var flag = true;
            if (issueDate != "")
            {
                var issueDateinDateTime = issueDate;
                flag = _allotmentService.CheckIssueDate(schemeId, Convert.ToDateTime(issueDateinDateTime));
            }
            return Json(flag);
        }

        /// <summary>
        /// Getting Scheme Start and End Date for validating Issue Date
        /// </summary>
        /// <param name="rId"></param>
        /// <returns></returns>
        public JsonResult GetSchemeDates(int schemeId)
        {
            if (schemeId != 0)
            {
                var schemeDates = _allotmentService.GetSchemeDates(schemeId);
                return Json(schemeDates, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Getting RID's from AllotmentMaster table for updating Personal Info
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDs()
        {
            var lst = _allotmentService.GetRIDs();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns RIds on the basis of Login User's Departments
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDsByDeptt()
        {
            var lst = _allotmentService.GetRIDsByDeptt();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Getting Scheme Wise Bank
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSchemeWiseBanks(int schemeId)
        {
            var lst = _generalService.GetSchemeWiseBanks(schemeId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Getting All Banks 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllBanks()
        {
            var lst = _generalService.GetAllBanks();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Getting All Branches by bank Id
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public JsonResult GetAllBranchs(int bankId)
        {
            var lst = _generalService.GetAllBranchs(bankId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Getting Branch Scheme Wise
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public JsonResult GetSchemeWiseBranchs(int bankId, int schemeId)
        {
            var lst = _generalService.GetSchemeWiseBranchs(bankId, schemeId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Getting All genders
        /// </summary>
        /// <returns></returns>
        public JsonResult GetGenderById(string genderId)
        {
            var lst = _generalService.GetGenderById(genderId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Getting All genders
        /// </summary>
        /// <returns></returns>

        [AllowAnonymous]
        public JsonResult GetGender()
        {
            var lst = _generalService.GetGender();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetIndividualGenders()
        {
            var lst = _generalService.GetIndividualGenders();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Getting All genders excecpt company //Change in Application form
        /// </summary>
        /// <returns></returns>

        [AllowAnonymous]
        public JsonResult GetGenderOnly()
        {
            var lst = _generalService.GetGender();
            lst.RemoveAt(2);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Getting Maritial Status
        /// </summary>
        /// <returns></returns>

        [AllowAnonymous]
        public JsonResult GetMaritialStatus()
        {
            var lst = _generalService.GetMaritialStatus();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Getting Occupation
        /// </summary>
        /// <returns></returns>

        [AllowAnonymous]
        public JsonResult GetOccupation()
        {
            var lst = _generalService.GetOccupation();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Getting Religion
        /// </summary>
        /// <returns></returns>

        [AllowAnonymous]
        public JsonResult GetReligion()
        {
            var lst = _generalService.GetReligion();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Getting Category or Quota
        /// </summary>
        /// <returns></returns>

        [AllowAnonymous]
        public JsonResult GetCategory()
        {
            var lst = _generalService.GetAllQuota();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Getting Earnest Money for displaying in Amount Deposited Grid
        /// </summary>
        /// <returns></returns>

        public JsonResult GetEarnestMoneyBySchemeAndDeptId(int SchemeId, int DepartmentId)
        {
            var lst = _allotmentService.GetEarnestMoneyBySchemeAndDeptId(SchemeId, DepartmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        // Check UTR or DD Number
        public JsonResult CheckUTROrDDNumber(string number, string type, int appId)
        {
            var flag = false;
            flag = _allotmentService.CheckUTROrDDNumber(number, type, appId);
            return Json(flag);
        }
        #endregion

        #region "Manage Allotee List "
        //  <summary>
        // For execution during page load    
        // </summary>

        public ActionResult ManageAllottee()
        {
            if (menuKey != 0)
            {
                var setPrmision = CommonMethords.SetRolePrmision(menuKey);
                if (setPrmision != null)
                {

                    ViewBag.EditMenuVal = setPrmision.EditMenuVal;
                    ViewBag.AddMenuVal = setPrmision.AddMenuVal;
                    ViewBag.DeleteMenuVal = setPrmision.DeleteMenuVal;
                    ViewBag.ReadOnlyMenu = setPrmision.ReadOnlyMenu;
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { area = "" });
                }
            }
            else
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            //SetRolePrmision(menuKey);
            //return View();
        }
        //  <summary>
        // Get all allottee list    
        // </summary>
        public ActionResult GetAllotteeLists([DataSourceRequest] DataSourceRequest req)
        {


            var allAllotee = _allotmentService.GetAlloteeLists(req);

            return Json(allAllotee, JsonRequestBehavior.AllowGet);

        }
        //  <summary>
        // Get  allottee model base on scheme id and depatmnet id     
        // </summary>
        public ActionResult AllotteeListView()
        {
            Uri url = Request.UrlReferrer;
            var schemeId = Convert.ToInt32(CommonHelper.Decode(Request.QueryString["SchemeId"]));
            var deptmentId = Convert.ToInt32(CommonHelper.Decode(Request.QueryString["DepartmentId"]));
            DateTime AllotmentDate = Convert.ToDateTime(CommonHelper.Decode(Request.QueryString["AllotmentDate"]));
            var AllotmentDates = Convert.ToDateTime(AllotmentDate.ToString("MM/dd/yyyy"));
            AllotteeListModel allotteeList = new AllotteeListModel();
            allotteeList = _allotmentService.GetAllotteeListByID(schemeId, deptmentId, AllotmentDates);
            return View(allotteeList);
        }
        //  <summary>
        // Get  allottee list base on scheme id and depatmnet id     
        // </summary>
        [HttpPost]
        public ActionResult AllotteeListView([DataSourceRequest] DataSourceRequest request, int idVal, int depid, DateTime allotmentDate)
        {
            List<AllotteeListModel> allotteeList = new List<AllotteeListModel>();
            if (idVal != 0 && depid != 0)
            {
                allotteeList = _allotmentService.AllotteeListView(idVal, depid, allotmentDate);
            }
            return Json(allotteeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        //  <summary>
        // submit for approval base on scheme id , deptment id and assine to user     
        // </summary>
        public ActionResult SubmitForApproval(int schemeId, int depid, string user, DateTime allotmentDate)
        {

            bool blnSuccess = false;
            //if (schemeId != 0 && depid != 0 && user != null && applicationId != 0)
            if (schemeId != 0 && depid != 0 && user != null)
            {
                blnSuccess = _allotmentService.SaveRequetForApproval(schemeId, depid, user, allotmentDate);
            }
            return Json(blnSuccess, JsonRequestBehavior.AllowGet);
        }

        //  <summary>
        // Get  all active user    
        // </summary>
        public JsonResult GetAssineTo()
        {
            var lst = _allotmentService.GetAssineTo();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintBulkAllotmentLetter(List<int> rIds)
        {
            //var parsedHtml = _allotmentService.GetBulkAllotmnetLetterPrint(rIds);
            var parsedHtml = string.Empty;
            foreach (var r in rIds)
            {
                parsedHtml += _generalService.GenerateLetterFromDbByRId(r, Constants.BulkAllotmentLetterTemplateID);
                parsedHtml += "<div style='page-break-after: always;' ></div>";
            }
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintBulkAllotmentPaymnetSchedule(List<int> rIds)
        {
            //var parsedHtml = _allotmentService.GetBulkAllotmnetPaymentLetterPrint(rIds);
            var parsedHtml = string.Empty;
            foreach (var r in rIds)
            {
                parsedHtml += _generalService.GenerateLetterFromDbByRId(r, Constants.BulkAllotmentPaymentScheduleTemplateID);
                parsedHtml += "<div style='page-break-after: always;' ></div>";
            }
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }
        #endregion



        /// <summary>
        /// bulk upload of property application form in excel 
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <param name="uploadExcel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BulkUploadApplicationForm()
        {
            var uploadExcel = Request.Files[0];
            int schemeId = int.Parse(Request.Form["SchemeId"]);
            int departmentId = int.Parse(Request.Form["DepartmentId"]);
            //var flag = false;
            string flag = _allotmentService.BulkUploadApplicationForm(schemeId, departmentId, uploadExcel);
            //return RedirectToAction("ManageApplication");
            return Json(flag);
        }

        /// <summary>
        /// download excel with specified header for bulk upload application form
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        //[WordDocument]
        public ActionResult BulkDownloadApplicationForm(int? schemeId, int? departmentId)
        {

            //var stream = _allotmentService.BulkDownloadApplicationForm(schemeId, departmentId);
            //var memoryStream = stream as MemoryStream;
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.AddHeader("content-disposition", "attachment; filename=ApplicationFormList.xlsx");
            //Response.BinaryWrite(memoryStream.ToArray());
            //Response.Flush();
            //return RedirectToAction("ManageApplication");

            //byte[] fileBytes = System.IO.File.ReadAllBytes(@"D:\NoidaAuthority\BulkApplicationFormat.xlsx");
            //byte[] fileBytes = System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["ApplicationExcelTemplate"]);
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(ConfigurationManager.AppSettings["ApplicationExcelTemplate"]));
            string fileName = "BulkApplicationFormat.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        /// <summary>
        /// download excel format with specified header for allotment bulk upload
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadAllotmentExcelFormat()
        {
            var stream = _allotmentService.DownloadAllotmentExcelFormat();
            var memoryStream = stream as MemoryStream;
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=AllotmentList.xlsx");
            Response.BinaryWrite(memoryStream.ToArray());
            Response.Flush();
            return RedirectToAction("ManageAllotment");
        }

        //public ActionResult BulkUploadForAllotment(int? schemeId, int? departmentId, HttpPostedFileBase uploadExcel)
        public ActionResult BulkUploadForAllotment()
        {
            var uploadExcel = Request.Files[0];
            int schemeId = int.Parse(Request.Form["SchemeId"]);
            int departmentId = int.Parse(Request.Form["DepartmentId"]);

            var flag = false;
            flag = _allotmentService.BulkUploadForAllotment(schemeId, departmentId, uploadExcel);
            //return RedirectToAction("ManageAllotment");
            return Json(flag);
        }

        #region Manage Request Process

        #region SuccessfullAllotteList
        public ActionResult ManageRequests()
        {
            return View();
        }
        /// <summary>
        /// Get All Requests
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult GetAllRequest([DataSourceRequest]DataSourceRequest request)
        {
            var allottedApplicants = _allotmentService.GetAllRequest().ToDataSourceResult(request);
            return Json(allottedApplicants, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Getting data like Approver Name, Comments, Approve Date for showing in page on the basis of Scheme and Dept Id
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="deptmentId"></param>
        /// <returns></returns>
        public ActionResult ManageAllotteeRequest()
        {

            Uri url = Request.UrlReferrer;
            var schemeId = Convert.ToInt32(CommonHelper.Decode(Request.QueryString["SchemeId"]));
            var deptmentId = Convert.ToInt32(CommonHelper.Decode(Request.QueryString["DepartmentId"]));
            DateTime allotmentDate = Convert.ToDateTime(CommonHelper.Decode(Request.QueryString["AllotmentDate"]));



            //var schemeId = Convert.ToInt32(Request.QueryString["SchemeId"]);
            //var deptmentId = Convert.ToInt32(Request.QueryString["DepartmentId"]);
            //var allotmentDate = Request.QueryString["AllotmentDate"];
            ManageRequestModel allotte = new ManageRequestModel();
            //if (id != null)
            //{
            //    string[] SchemeIdAndDept = id.Split(new char[] { ',' });

            //    if (SchemeIdAndDept.Length > 0)
            //    {
            //        var schemeId = Int32.Parse(SchemeIdAndDept[0]);
            //        var deptmentId = Int32.Parse(SchemeIdAndDept[1]);
            if (allotmentDate != null)
            {

                var AllotmentDates = Convert.ToDateTime(allotmentDate.ToString("MM/dd/yyyy"));

                //string AllotmentDateSubStr = allotmentDate.Substring(4, 11);
                //string AllotmentDates = DateTime.ParseExact(AllotmentDateSubStr, "MMM dd yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                // var AllotmentDates = DateTime.ParseExact(AllotmentDateSubStr, "MMM dd yyyy", CultureInfo.InvariantCulture);
                allotte = _allotmentService.GetAllotteeListRequestByID(schemeId, deptmentId, AllotmentDates);
                allotte.AllotmentDate = AllotmentDates;
            }
            //    }
            //}
            return View(allotte);

        }
        /// <summary>
        /// Getting Aplicants list for approval on the basis of scheme and dept Id
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="depid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAllotteesForApproveReject([DataSourceRequest] DataSourceRequest request, int idVal, int depid, DateTime allotmentDate)
        {
            List<ManageRequestModel> allotteeList = new List<ManageRequestModel>();
            if (idVal != 0 && depid != 0)
            {
                //allotteeList = _allotmentService.AllotteeListView(idVal, depid);
                allotteeList = _allotmentService.AllotteeListForApproval(idVal, depid, allotmentDate);
            }
            return Json(allotteeList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Saving Post for Allotte List aplicants and unsuccessfull aplicants, also Saving isAllotted true and false for successful and Unsucessful aplicants 
        /// copying unsuccessful aplicants in UnsuccessfulApplicantListMasters table which have isAllotted column False in Aplication Detail table
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="depid"></param>
        /// <param name="comment"></param>
        /// <param name="status"></param>
        /// <param name="isUnSuccessfullAplicants"></param>
        /// <returns></returns>
        public ActionResult SaveForPost(int schemeId, int depid, string comment, string status, bool isUnSuccessfullAplicants, DateTime allotmentDate)
        {
            bool blnSuccess = false;
            if (schemeId != 0 && depid != 0 && comment != null && status != null)
            {
                blnSuccess = _allotmentService.SaveRequetForApprovalRequest(schemeId, depid, comment, status, isUnSuccessfullAplicants, allotmentDate);
                if (blnSuccess)
                {
                    List<ApplicationDetail> objApplicationDetail = new List<ApplicationDetail>();
                    objApplicationDetail = _allotmentService.GetDetailsOfAllotedProprety(schemeId, depid, allotmentDate);

                    for (int i = 0; i < objApplicationDetail.Count(); i++)
                    {
                        string FormNo = objApplicationDetail[i].formNo;
                        OnlineFormViewModel objOnlineFormViewModel = new OnlineFormViewModel();
                        objOnlineFormViewModel.ApplicationForm = FormNo;
                        int _AppId = 0;
                        if (int.TryParse(Convert.ToString(FormNo), out _AppId))
                            objOnlineFormViewModel.ApplicationFormId = _AppId;
                        if (objOnlineFormViewModel.ApplicationFormId > 0)
                        {
                            if (status.ToLower() == Common.AllotmentStatus.Approved.ToString().ToLower())
                            {
                                SendAllotmentStatus(objOnlineFormViewModel, ServiceStatus.APPROVED, ServiceStatus_Text.APPROVED);
                                IsLandPurchased(objOnlineFormViewModel, "YES");
                            }
                            else
                            {
                                SendAllotmentStatus(objOnlineFormViewModel, ServiceStatus.APPROVED, ServiceStatus_Text.APPROVED);
                            }
                        }
                    }

                }
            }
            return Json(blnSuccess, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region UnSuccessfullAllotteList

        public ActionResult UnSuccessfullAllotteList()
        {
            return View();
        }
        /// <summary>
        /// Get All Unsuccessfull Applicants List
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetAllUnsuccessfullApplicants([DataSourceRequest]DataSourceRequest request)
        {
            var allApplications = _allotmentService.GetAllUnsuccessfullApplicants().ToDataSourceResult(request);
            return Json(allApplications, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Getting data like Approver Name, Comments, Approve Date for showing in page on the basis of Scheme and Dept Id
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="deptmentId"></param>
        /// <returns></returns>
        public ActionResult ManageUnsuccessfullAllottees()
        {
            //var schemeId = Convert.ToInt32(Request.QueryString["SchemeId"]);
            //var deptmentId = Convert.ToInt32(Request.QueryString["DepartmentId"]);
            //var allotmentDate = Request.QueryString["AllotmentDate"];
            var schemeId = Convert.ToInt32(CommonHelper.Decode(Request.QueryString["SchemeId"]));
            var deptmentId = Convert.ToInt32(CommonHelper.Decode(Request.QueryString["DepartmentId"]));
            DateTime allotmentDate = new DateTime();
            if (Request.QueryString["AllotmentDate"] != "")
            {
                allotmentDate = Convert.ToDateTime(CommonHelper.Decode(Request.QueryString["AllotmentDate"]));
            }
            ManageRequestModel allotte = new ManageRequestModel();
            //if (id != null)
            //{
            //    string[] SchemeIdAndDept = id.Split(new char[] { ',' });

            //    if (SchemeIdAndDept.Length > 0)
            //    {
            //        var schemeId = Int32.Parse(SchemeIdAndDept[0]);
            //        var deptmentId = Int32.Parse(SchemeIdAndDept[1]);
            allotte = _allotmentService.GetUnsuccessfullAplicantstRequestByID(schemeId, deptmentId);
            if (allotmentDate.ToString() != null)
            {
                var AllotmentDates = Convert.ToDateTime(allotmentDate.ToString("MM/dd/yyyy"));
                //string AllotmentDateSubStr = allotmentDate.Substring(4, 11);
                //string AllotmentDates = DateTime.ParseExact(AllotmentDateSubStr, "MMM dd yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                //var AllotmentDates = DateTime.ParseExact(AllotmentDateSubStr, "MMM dd yyyy", CultureInfo.InvariantCulture);
                allotte.AllotmentDate = AllotmentDates;
            }
            //    }
            //}
            return View(allotte);

        }
        /// <summary>
        /// Getting List of Unsucessfull Applicants for Approve and Reject
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="depid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetUnsuccessfullApplicantsForApproval([DataSourceRequest] DataSourceRequest request, int idVal, int depid)
        {
            List<ManageRequestModel> unSuccessfulApplicantsforapproval = new List<ManageRequestModel>();
            if (idVal != 0 && depid != 0)
            {
                unSuccessfulApplicantsforapproval = _allotmentService.GetUnsuccessfullApplicantsForApproval(idVal, depid);
            }
            return Json(unSuccessfulApplicantsforapproval.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion End Manage Request Process

        #region Allotment Process
        public ActionResult ManageAllotment()
        {
            if (menuKey != 0)
            {
                var setPrmision = CommonMethords.SetRolePrmision(menuKey);
                if (setPrmision != null)
                {

                    ViewBag.EditMenuVal = setPrmision.EditMenuVal;
                    ViewBag.AddMenuVal = setPrmision.AddMenuVal;
                    ViewBag.DeleteMenuVal = setPrmision.DeleteMenuVal;
                    ViewBag.ReadOnlyMenu = setPrmision.ReadOnlyMenu;
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { area = "" });
                }
            }
            else
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            //SetRolePrmision(menuKey);
            //return View();
        }

        public JsonResult GetAllotment([DataSourceRequest]DataSourceRequest req)
        {
            var data = _allotmentService.GetAllotment(req);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllotmentPartial([DataSourceRequest]DataSourceRequest req)
        {
            var data = _allotmentService.GetAllotmentPartial(req);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllRequests()
        {
            var lst = _generalService.GetAllQuota();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddAllotment()
        {
            Uri url = Request.UrlReferrer;
            var setPrmision = CommonMethords.SetRolePrmision(menuKey);

            if (setPrmision.AddMenuVal == true)
            {

                AllotmentModel allotmentModel = new AllotmentModel();
                allotmentModel.PropertyAllotment = new PropertyAllotmentModel();
                //allotmentModel.ApplicationForm = new ApplicationFormModel();
                return View(allotmentModel);
            }
            else
            {
                return Redirect("/Account/Unauthorized");
            }
        }
        [HttpPost]
        public ActionResult AddAllotment(AllotmentModel allotmentModel)
        {
            bool flag = false;
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (ModelState.IsValid)
            {
                flag = _allotmentService.AddAllotment(allotmentModel, loginUser.UserID);
            }

            if (flag)
            {
                OnlineFormViewModel ObjOnlineFormViewModel = new OnlineFormViewModel();
                int _AppId = 0;
                if (int.TryParse(Convert.ToString(allotmentModel.FormNo), out _AppId))
                    ObjOnlineFormViewModel.ApplicationFormId = _AppId;
                if (ObjOnlineFormViewModel.ApplicationFormId > 0)
                {
                    SendAllotmentStatus(ObjOnlineFormViewModel, ServiceStatus.INPROCESS, ServiceStatus_Text.INPROCESS);
                }
                return RedirectToAction("ManageAllotment", "Allotment");
            }
            else
            {
                return View(allotmentModel);
            }
        }

        //Nivesh Mitra Services (NIC)
        private string SendAllotmentStatus(OnlineFormViewModel model, string ServiceStatusCode, string ServiceStatusRemarks)
        {
            string message = string.Empty;
            var data = _onlineService.GetNICSingleWindowData(model);
            if (data != null && !string.IsNullOrEmpty(data.Control_ID))
            {
                WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel();
                objWReturn_CUSID_STATUSModel.ControlID = data.Control_ID;
                objWReturn_CUSID_STATUSModel.ApplicationID = Convert.ToString(model.ApplicationFormId);
                objWReturn_CUSID_STATUSModel.ProcessIndustryID = Convert.ToString(model.ApplicationFormId);
                objWReturn_CUSID_STATUSModel.UnitID = data.Unit_Id;
                objWReturn_CUSID_STATUSModel.ServiceID = data.ServiceID;
                objWReturn_CUSID_STATUSModel.Status_Code = ServiceStatusCode;
                objWReturn_CUSID_STATUSModel.Remarks = ServiceStatusRemarks;
                objWReturn_CUSID_STATUSModel.Fee_Status = string.Empty;
                objWReturn_CUSID_STATUSModel.Fee_Amount = string.Empty;
                objWReturn_CUSID_STATUSModel.passsalt = Passalt;

                //Pass Object to NIC Service
                NA.PMS.Web.Controllers.Common.NiveshMitraServices objNIC = new Web.Controllers.Common.NiveshMitraServices();
                message = objNIC.GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);
            }
            return message;
        }

        private string IsLandPurchased(OnlineFormViewModel model, string ISLandPurchasedYesNO)
        {
            string message = string.Empty;
            var data = _onlineService.GetNICSingleWindowData(model);
            if (data != null && !string.IsNullOrEmpty(data.Control_ID))
            {
                WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel();
                objWReturn_CUSID_STATUSModel.ControlID = data.Control_ID;
                objWReturn_CUSID_STATUSModel.UnitID = data.Unit_Id;
                objWReturn_CUSID_STATUSModel.ServiceID = data.ServiceID;
                objWReturn_CUSID_STATUSModel.ISLandPurchasedYesNO = ISLandPurchasedYesNO;
                objWReturn_CUSID_STATUSModel.passsalt = Passalt;
                NA.PMS.Web.Controllers.Common.NiveshMitraServices objNIC = new Web.Controllers.Common.NiveshMitraServices();
                message = objNIC.WReturn_CUSID_ISLandPurchased(objWReturn_CUSID_STATUSModel);
            }
            return message;
        }

        public ActionResult EditAllotment(string Id)
        {
            int allotmentID = Convert.ToInt32(CommonHelper.Decode(Id));
            var setPrmision = CommonMethords.SetRolePrmision(menuKey);
            //Uri url = Request.UrlReferrer;

            if (setPrmision.EditMenuVal == true)
            {
                AllotmentModel allotmentmodel = new AllotmentModel();
                allotmentmodel.PropertyAllotment = new PropertyAllotmentModel();
                //allotmentmodel.ApplicationForm = new ApplicationFormModel();
                if (allotmentID != 0)
                {
                    allotmentmodel = _allotmentService.GetAllotmentById(allotmentID);
                }
                return View(allotmentmodel);
            }
            else
            {
                return Redirect("/Account/Unauthorized");
            }
        }

        public ActionResult ViewAllotmentdetails(string Id)
        {
            int allotmentID = Convert.ToInt32(CommonHelper.Decode(Id));
            AllotmentModel allotmentmodel = new AllotmentModel();
            allotmentmodel.PropertyAllotment = new PropertyAllotmentModel();
            if (allotmentID != 0)
            {
                allotmentmodel = _allotmentService.GetAllotmentById(allotmentID);
            }
            return View(allotmentmodel);
        }

        [HttpPost]
        public ActionResult EditAllotment(AllotmentModel allotmentModel)
        {
            bool flag = false;
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (ModelState.IsValid)
            {
                flag = _allotmentService.UpdateAllotment(allotmentModel, loginUser.UserID);
            }
            if (flag)
            {
                return RedirectToAction("ManageAllotment", "Allotment");
            }
            else
            {
                return View(allotmentModel);
            }

        }

        public ActionResult ViewAllotment(int id)
        {
            AllotmentModel allotmentModel = new AllotmentModel();
            allotmentModel.PropertyAllotment = new PropertyAllotmentModel();
            //allotmentModel.ApplicationForm = new ApplicationFormModel();
            if (id != 0)
            {
                allotmentModel = _allotmentService.GetAllotmentById(id);
            }
            return View(allotmentModel);
        }

        public JsonResult GetAllForms(int SchemeId, int DepartmentId)
        {
            var lst = _allotmentService.GetAllFormsForAllotment(SchemeId, DepartmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllFormsByDatasource([DataSourceRequest] DataSourceRequest Req, int? SchemeId, int? DepartmentId)
        {
            var lst = _allotmentService.GetAllFormsForAllotment(Req, SchemeId, DepartmentId);
            var JsonResult = Json(lst, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }

        public JsonResult GetAllFormsForView(int SchemeId, int DepartmentId)
        {
            var lst = _allotmentService.GetAllFormsForView(SchemeId, DepartmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllProperty(int SchemeId, int DepartmentId)
        {
            var lst = _allotmentService.GetAllProperty(SchemeId, DepartmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllPropertyByDataSource([DataSourceRequest] DataSourceRequest Req, int? SchemeId, int? DepartmentId)
        {
            var lst = _allotmentService.GetAllProperty(Req, SchemeId, DepartmentId);
            var JsonResult = Json(lst, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }

        public JsonResult GetApplicantDetails(int propId)
        {
            var lst = _allotmentService.GetApplicantDetails(propId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApplicantInfo(string formno, int schemeID, int departmentID)
        {
            var lst = _allotmentService.GetApplicantInfo(formno, schemeID, departmentID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApplicantDetailsForAllotment(string formno, int schemeID, int departmentID)
        {
            var lst = _allotmentService.GetApplicantDetails(formno, schemeID, departmentID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPropertyDetailsForAllotment(int propertyId, int schemeID, int departmentID)
        {
            var lst = _allotmentService.GetPropertyDetails(propertyId, schemeID, departmentID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPropertyDetails(int rID)
        {
            var lst = _allotmentService.GetPropertyDetails(rID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApplicantPaymentDetails(string formno, int applicationId)
        {
            var lst = _allotmentService.GetApplicantPaymentDetails(formno, applicationId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        #endregion

        /// <summary>
        /// Returns View for Manage Refunds
        /// </summary>
        /// <returns>View</returns>
        public ActionResult ManageRefunds()
        {
            if (menuKey != 0)
            {
                var setPrmision = CommonMethords.SetRolePrmision(menuKey);
                if (setPrmision != null)
                {

                    ViewBag.EditMenuVal = setPrmision.EditMenuVal;
                    ViewBag.AddMenuVal = setPrmision.AddMenuVal;
                    ViewBag.DeleteMenuVal = setPrmision.DeleteMenuVal;
                    ViewBag.ReadOnlyMenu = setPrmision.ReadOnlyMenu;
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { area = "" });
                }
            }
            else
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            //SetRolePrmision(menuKey);
            //return View();
        }

        /// <summary>
        /// Used for reading grid on Manage Refund View
        /// </summary>
        /// <param name="req">DataSourceRequest, in-built of Kendo</param>
        /// <returns>Gird data</returns>
        public JsonResult GetRefundDetails([DataSourceRequest] DataSourceRequest request)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser != null)
            {
                var refundLst = _allotmentService.GetRefundDetails(request, loginUser.UserID);
                return Json(refundLst, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Reads Unsuccessful Dateils such as such as status -> Approved, Rejected, Refund initiated etc. and returns View.
        /// </summary>
        /// <param name="scID">Scheme ID</param>
        /// <param name="depID">Department ID</param>
        /// <returns>View</returns>
        public ActionResult ViewUnsuccessfulList(int scID, int depID)
        {
            var objUnsuccessful = new UnsuccessfulApplicant();
            objUnsuccessful = _allotmentService.GetUnsuccessfulLstDetails(scID, depID);
            if (objUnsuccessful != null)
            {
                objUnsuccessful.SchemeId = scID;
                objUnsuccessful.DepttId = depID;
            }
            return View(objUnsuccessful);
        }

        /// <summary>
        /// Used for reading Unsuccesful List according to Scheme ID and Department ID
        /// </summary>
        /// <param name="request">Kendo internal</param>
        /// <param name="scID">Scheme ID</param>
        /// <param name="depID">Department ID</param>
        /// <returns>Grid data</returns>
        public JsonResult GetUnsuccessfulApplicants([DataSourceRequest] DataSourceRequest request, int scID, int depID)
        {
            var unsuccessfulLst = _allotmentService.GetUnsuccessfulApplicants(request, scID, depID);
            return Json(unsuccessfulLst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Used for Submitting for Approval or Initiating Refund
        /// </summary>
        /// <param name="scID">Scheme ID</param>
        /// <param name="depID">Department ID</param>
        /// <param name="action">Action = 1 => Submitting for Approval; Action = 2 => Initiating Refund</param>
        /// <param name="userVal">Optional parameter for "Approver Name (UserRefID)"</param>
        /// <returns>Flag: True -> Data updated; False -> Data not updated</returns>
        public JsonResult UnsuccessfulListAction(int scID, int depID, int action, string userVal)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            if (loginUser != null && scID != 0 && depID != 0 && action != 0)
            {
                var flag = _allotmentService.UnsuccessfulListAction(scID, depID, action, loginUser.UserID, userVal);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChallanPrint(int RegistrationId = 0, int flag = 0)
        {
            PropertyModel ChallanPrintDetails = new PropertyModel();
            return View(ChallanPrintDetails);
        }

        public ActionResult PrintViewAllotment(int propertyId, int schemeID, int departmentId)
        {
            ChallanModel challanModel = new ChallanModel();
            if (propertyId != 0)
            {
                challanModel = _allotmentService.PrintViewAllotment(propertyId, schemeID, departmentId);
            }
            return View("ChallanPrint", challanModel);
        }

        public ActionResult AddCompany()
        {
            return View();
        }

        public JsonResult GetAllCompanyForms(int SchemeId, int DepartmentId)
        {
            var lst = _allotmentService.GetAllCompanyForms(SchemeId, DepartmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDirectorDetailsByAppID(DataSourceRequest req, int applicationID)
        {
            var lst = _allotmentService.GetDirectorDetailsByAppID(req, applicationID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDiretors(decimal directorShare, string directorName, int type, int appId)
        {
            var lst = _allotmentService.SaveDiretors(directorShare, directorName, type, appId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddCompanyDetails(string data, string NewFirmName, string NewFirmProduct, int NewFirmStatus, int appId)
        {
            var lst = _allotmentService.AddCompanyDetails(data, NewFirmName, NewFirmProduct, NewFirmStatus, appId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelAllotment(int rid)
        {
            var lst = _allotmentService.CancelAllotment(rid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Generateletter(int rid)
        {
            var lst = _allotmentService.Generateletter(rid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Viewletter(LetterHistory _LetterHistory)
        {
            var lst = _allotmentService.ViewletterTemplate(_LetterHistory);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// return scheme list for dropdown
        /// </summary>
        /// <returns></returns>
        public ActionResult SchemeListForAllotment()
        {
            List<SchemeAllotmentModel> schemeList = _allotmentService.SchemeListForAllotment();
            return Json(schemeList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllottedPropertyDetails()
        {
            return View();
        }

        public JsonResult CheckAllotmentCompletedForDay(int schemeId, int departmentId, DateTime allotmentDate)
        {
            var lst = _allotmentService.CheckAllotmentDate(schemeId, departmentId, allotmentDate);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewSchemeDetails(string id)
        {
            return RedirectToAction("ViewScheme", "Scheme", new { area = "Master", id = id });
        }

        [AllowAnonymous]
        public ActionResult OnlineApplicationRequest()
        {
            return View();
        }
        //[AllowAnonymous]
        //public ActionResult OfflineApplicationRequest()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult OnlineApplicationRequest(OnlineApplicationFormModel applicationFormModel)
        //{
        //    var lstOnline = _allotmentService.OnlineApplicationRequest(applicationFormModel);
        //    //if (lstOnline.flag)
        //    //{
        //    //    return View("OnlineApplicationChecklist", applicationFormModel);
        //    //    //int otp = GenerateOTP();
        //    //    //Session["OTP"] = otp;
        //    //    //string strMsg = "Your OTP is " + otp.ToString() + " DO NOT disclose this to anyone by any means. This is for online use by you only.";
        //    //    //SMSSend(applicationFormModel.MobileNumber, strMsg);
        //    //}
        //    //TempData["MesgAdd"] = lstOnline;
        //    return View("OnlineApplicationuploads", applicationFormModel);
        //}

        //public ActionResult OnlineApplicationChecklist()
        //{
        //    return View();
        //}

        //private int GenerateOTP()
        //{
        //    Session["OTP"] = null;
        //    Random random = new Random();
        //    int maxValue = 999999;
        //    int r = random.Next(maxValue);
        //    return r;
        //}

        //private void SMSSend(string mobileNo, string msg)
        //{
        //    WebClient client = new WebClient();
        //    string baseurl = ConfigurationManager.AppSettings["SMSsend"].ToString() + ConfigurationManager.AppSettings["SMSUsername"].ToString() + "&password=" + ConfigurationManager.AppSettings["SMSPassword"].ToString() + "&sendername=" + "NETSMS" + "&mobileno=" + mobileNo + "&message=" + msg;
        //    Stream data = client.OpenRead(baseurl);
        //    StreamReader reader = new StreamReader(data);
        //    string s = reader.ReadToEnd();
        //    data.Close();
        //    reader.Close();
        //}

        //[AllowAnonymous]
        //public JsonResult VerifyOTP(string otp)
        //{
        //    var flag = false;
        //    if (Session["OTP"].ToString().Equals(otp.ToString()))
        //    {
        //        flag = true;
        //    }
        //    return Json(flag);
        //}

        //[AllowAnonymous]
        //public void GenerateOTPForOfline(string strMobileNumber)
        //{
        //    Session["OTP"] = null;
        //    int otp = GenerateOTP();
        //    Session["OTP"] = otp;
        //    string strMsg = "Your OTP is " + otp.ToString() + " DO NOT disclose this to anyone by any means. This is for online use by you only.";
        //    SMSSend(strMobileNumber, strMsg);
        //}

        //[AllowAnonymous]
        //public ActionResult OTPDetails()
        //{
        //    return View();
        //}
        //[AllowAnonymous]
        //public ActionResult ViewOnlineDetails(int id)
        //{
        //    var lstViewDetails = _allotmentService.ViewOnlineDetails(id);
        //    return View(lstViewDetails);
        //}

        //[AllowAnonymous]
        //public ActionResult EditDetails(int id)
        //{
        //    var lstViewDetails = _allotmentService.ViewOnlineDetails(id);
        //    return View(lstViewDetails);
        //}
        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult Edit(OnlineApplicationFormModel onlineApplicationFormModel)
        //{
        //    //Session["MemberName"] = onlineApplicationFormModel.FirstName + " " + onlineApplicationFormModel.MiddleName + " " + onlineApplicationFormModel.LastName;
        //    //Session["Date"] = DateTime.Now;
        //    //Session["TotalAmount"] = onlineApplicationFormModel.Amount;
        //    OnlineApplicationFormModel lstViewDetails = _allotmentService.EditOnlineDetails(onlineApplicationFormModel);
        //    //return View("ViewOnlineDetails", lstViewDetails);
        //    //return RedirectToAction("ViewOnlineDetails", lstViewDetails.ApplicationId);
        //    return RedirectToAction("ViewOnlineDetails", new { id = lstViewDetails.ApplicationId });
        //}
        //[AllowAnonymous]
        //public ActionResult PaymentDetails(int id)
        //{
        //    //SaveTrasOnlineDetails
        //    var lstViewDetails = _allotmentService.SaveTrasOnlineDetails(id);
        //    return View(lstViewDetails);
        //    //string[] hashVarsSeq;
        //    //string hash_string = string.Empty;
        //    //if (string.IsNullOrEmpty(Request.Form["txnid"])) // generating txnid
        //    //{
        //    //    Random rnd = new Random();
        //    //    string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
        //    //    txnid1 = strHash.ToString().Substring(0, 20);

        //    //}
        //}

        //[AllowAnonymous]
        //public ActionResult TrasactionDetails(string id)
        //{

        //    var obj = new HTMLClass();
        //    //var lstViewDetails = _allotmentService.GetTrasactionDetails("1-7");
        //    string action1 = string.Empty;
        //    string hash1 = string.Empty;
        //    string txnid1 = string.Empty;
        //    string hash = string.Empty;
        //    string key = string.Empty;
        //    string txnid = string.Empty;
        //    // var productinfo="sdf";


        //    string[] hashVarsSeq;
        //    string hash_string = string.Empty;


        //    if (string.IsNullOrEmpty(lstViewDetails.Txnid)) // generating txnid
        //    {
        //        Random rnd = new Random();
        //        string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
        //        txnid1 = strHash.ToString().Substring(0, 20);

        //    }
        //    else
        //    {
        //        txnid1 = lstViewDetails.Txnid;
        //    }
        //    if (string.IsNullOrEmpty(Request.Form["hash"])) // generating hash value
        //    {
        //        if (
        //            string.IsNullOrEmpty(ConfigurationManager.AppSettings["MERCHANT_KEY"]) ||
        //            string.IsNullOrEmpty(txnid1) ||
        //            string.IsNullOrEmpty(lstViewDetails.Amount.ToString()) ||
        //            string.IsNullOrEmpty(lstViewDetails.FirstName) ||
        //            string.IsNullOrEmpty(lstViewDetails.Email) ||
        //            string.IsNullOrEmpty(lstViewDetails.PhoneNumber) ||
        //            string.IsNullOrEmpty(lstViewDetails.Productinfo) ||
        //            string.IsNullOrEmpty(ConfigurationManager.AppSettings["surl"]) ||
        //            string.IsNullOrEmpty(ConfigurationManager.AppSettings["furl"])
        //            )
        //        {
        //            //error

        //            // frmError.Visible = true;
        //            // return;
        //        }

        //        else
        //        {
        //            //frmError.Visible = false;
        //            hashVarsSeq = ConfigurationManager.AppSettings["hashSequence"].Split('|'); // spliting hash sequence from config
        //            hash_string = "";
        //            foreach (string hash_var in hashVarsSeq)
        //            {
        //                if (hash_var == "key")
        //                {
        //                    hash_string = hash_string + ConfigurationManager.AppSettings["MERCHANT_KEY"];
        //                    hash_string = hash_string + '|';
        //                }
        //                else if (hash_var == "txnid")
        //                {
        //                    hash_string = hash_string + txnid1;
        //                    hash_string = hash_string + '|';
        //                }
        //                else if (hash_var == "amount")
        //                {
        //                    hash_string = hash_string + lstViewDetails.Amount.ToString();
        //                    hash_string = hash_string + '|';
        //                }
        //                else
        //                {

        //                    hash_string = hash_string + (Request.Form[hash_var] != null ? Request.Form[hash_var] : "");// isset if else
        //                    hash_string = hash_string + '|';
        //                }
        //            }

        //            hash_string += ConfigurationManager.AppSettings["SALT"];// appending SALT

        //            hash1 = Generatehash512(hash_string).ToLower();         //generating hash
        //            action1 = ConfigurationManager.AppSettings["PAYU_BASE_URL"] + "/_payment";// setting URL

        //        }


        //    }

        //    else if (!string.IsNullOrEmpty(Request.Form["hash"]))
        //    {
        //        hash1 = Request.Form["hash"];
        //        action1 = ConfigurationManager.AppSettings["PAYU_BASE_URL"] + "/_payment";

        //    }




        //    if (!string.IsNullOrEmpty(hash1))
        //    {
        //        hash = hash1;
        //        txnid = txnid1;

        //        System.Collections.Hashtable data = new System.Collections.Hashtable(); // adding values in gash table for data post
        //        data.Add("hash", hash);
        //        data.Add("txnid", txnid);
        //        data.Add("key", ConfigurationManager.AppSettings["MERCHANT_KEY"]);
        //        //string AmountForm = Convert.ToDecimal(amount.Text.Trim()).ToString("g29");// eliminating trailing zeros
        //        //amount.Text = AmountForm;
        //        data.Add("amount", lstViewDetails.Amount);
        //        data.Add("firstname", lstViewDetails.FirstName);
        //        data.Add("email", lstViewDetails.Email);
        //        data.Add("phone", lstViewDetails.PhoneNumber);
        //        data.Add("productinfo", lstViewDetails.Productinfo);
        //        data.Add("surl", ConfigurationManager.AppSettings["surl"]);
        //        data.Add("furl", ConfigurationManager.AppSettings["furl"]);
        //        data.Add("lastname", "");
        //        data.Add("curl", "");
        //        data.Add("address1", "");
        //        data.Add("address2", "");
        //        data.Add("city", "");
        //        data.Add("state", "");
        //        data.Add("country", "");
        //        data.Add("zipcode", "");
        //        data.Add("udf1", lstViewDetails.Productinfo);
        //        data.Add("udf2", lstViewDetails.Productinfo);
        //        data.Add("udf3", lstViewDetails.Productinfo);
        //        data.Add("udf4", lstViewDetails.Productinfo);
        //        data.Add("udf5", lstViewDetails.Productinfo);
        //        data.Add("pg", "");


        //        string strForm = PreparePOSTForm(action1, data);

        //        // Page.Controls.Add(new LiteralControl(strForm));

        //        obj.city = string.Empty;
        //        obj.curl = string.Empty;
        //        obj.address2 = string.Empty;
        //        obj.phone = lstViewDetails.PhoneNumber;
        //        obj.furl = ConfigurationManager.AppSettings["furl"];
        //        obj.state = string.Empty;
        //        obj.udf1 = lstViewDetails.Productinfo;
        //        obj.address1 = string.Empty;
        //        obj.amount = lstViewDetails.Amount;
        //        obj.txnid = txnid;
        //        obj.udf2 = lstViewDetails.Productinfo;
        //        obj.email = lstViewDetails.Email;
        //        obj.productinfo = lstViewDetails.Productinfo;
        //        obj.udf3 = lstViewDetails.Productinfo;
        //        obj.firstname = lstViewDetails.FirstName;
        //        obj.lastname = string.Empty;
        //        obj.zipcode = string.Empty;
        //        obj.udf4 = lstViewDetails.Productinfo;
        //        obj.pg = string.Empty;
        //        obj.country = string.Empty;
        //        obj.surl = ConfigurationManager.AppSettings["surl"];
        //        obj.hash = "2e45618dfd4caaa2bf5360473cc6f6dd0300e983036b88a1180c63c5a1d89b21c07869d33a7de150023b667b1fc1618159f8d62eb58d0580c2d86e83d169b9a5";
        //        obj.hash = hash;
        //        // obj.hash = "sha512(gtKFFx|1-7|20000.00|Online|s|sds@sdsd.com|Online|Online|Online|Online|Online||||||eCwWELxi)";
        //        obj.udf5 = lstViewDetails.Productinfo;
        //        obj.key = ConfigurationManager.AppSettings["MERCHANT_KEY"];
        //        obj.LiteralStr = strForm;
        //        //ViewBag.div = strForm;
        //    }

        //    else
        //    {
        //        //no hash

        //    }
        //    return View("TrasactionDetailsPayment", obj);


        //    //catch (Exception ex)
        //    //{
        //    //    Response.Write("<span style='color:red'>" + ex.Message + "</span>");

        //    //}


        //    //  var lstViewDetails = _allotmentService.GetTrasactionDetails(id);
        //    //  var key = ConfigurationManager.AppSettings["MERCHANT_KEY"];
        //    //  System.Collections.Hashtable data = new System.Collections.Hashtable(); // adding values in gash table for data post
        //    //  data.Add("hash", hash.Value);
        //    //  data.Add("txnid", lstViewDetails.Txnid);
        //    //  data.Add("key", key);
        //    ////  string AmountForm = Convert.ToDecimal(amount.Text.Trim()).ToString("g29");// eliminating trailing zeros
        //    //  //amount.Text = lstViewDetails.Amount;
        //    //  data.Add("amount", lstViewDetails.Amount);
        //    //  data.Add("firstname", lstViewDetails.FirstName);
        //    //  data.Add("email", lstViewDetails.Email);
        //    //  data.Add("phone", lstViewDetails.PhoneNumber);
        //    //  data.Add("productinfo", "Ind Online Form Amount");
        //    //var lstViewDetails = _allotmentService.ViewOnlineDetails(id);
        //    //return View(lstViewDetails);
        //}
        //[AllowAnonymous]
        //public ActionResult PrintTransaction(int appId)
        //{
        //    var lst = "Applicant Name: Dummy Data";
        //    return Json(lst, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public ActionResult GetSchemeListForOnline()
        //{
        //    List<SchemeAllotmentModel> schemeList = _allotmentService.GetSchemeListForAllotment();
        //    return Json(schemeList, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public ActionResult FilterDepartmentOnSchemeForOnline(int SchemeId)
        //{
        //    List<DepartmentAllotmentModel> department = _allotmentService.FilterDepartmentOnScheme(SchemeId);
        //    return Json(department, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public ActionResult OnlineApplicationUploads()
        //{
        //    return View();
        //}

        //[AllowAnonymous]
        //public JsonResult SaveFiles()
        //{
        //    var flag = false;
        //    var flagFilesSaved = false;
        //    var obj = new UploadDetails();
        //    var lst = Request["IDLst"];
        //    if (Request.Files.Count > 0)
        //    {
        //        if (Request.Files[0] != null && Request.Files[1] != null)
        //        {
        //            var filePhoto = Request.Files[0];
        //            var fileSign = Request.Files[1];

        //            var applicationID = Request["applicationID"];
        //            var details = new UploadDetails
        //            {
        //                filePhoto = new FileInfo(filePhoto.FileName).Name,
        //                fileSign = new FileInfo(fileSign.FileName).Name,
        //                //fileDoc = (Request.Files[2] != null) ? new FileInfo(Request.Files[2].FileName).Name : string.Empty,
        //                fileDoc = string.Empty,
        //                IDLst = lst,
        //                ApplicationID = Convert.ToInt32(applicationID)
        //            };

        //            if (Request.Files.Count > 2)
        //            {
        //                details.fileDoc = new FileInfo(Request.Files[2].FileName).Name;
        //            }
        //            flag = _allotmentService.SaveFileDetailsToDB(details);

        //            if (flag == true)
        //            {
        //                flagFilesSaved = true;
        //                if (filePhoto != null && filePhoto.ContentLength > 0)
        //                {
        //                    if (!Directory.Exists(Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID)))
        //                    {
        //                        Directory.CreateDirectory(Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID));
        //                    }
        //                    var fileSavePath = Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID + "/" + details.filePhoto);
        //                    filePhoto.SaveAs(fileSavePath);
        //                }
        //                if (fileSign != null && fileSign.ContentLength > 0)
        //                {
        //                    if (!Directory.Exists(Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID)))
        //                    {
        //                        Directory.CreateDirectory(Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID));
        //                    }
        //                    var fileSavePath = Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID + "/" + details.fileSign);
        //                    fileSign.SaveAs(fileSavePath);
        //                    flagFilesSaved = true;
        //                }
        //                if (Request.Files.Count > 2 && Request.Files[2] != null && Request.Files[2].ContentLength > 0)
        //                {
        //                    if (!Directory.Exists(Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID)))
        //                    {
        //                        Directory.CreateDirectory(Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID));
        //                    }
        //                    var fileSavePath = Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + applicationID + "/" + details.fileDoc);
        //                    Request.Files[2].SaveAs(fileSavePath);
        //                    flagFilesSaved = true;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var applicationID = Request["applicationID"];
        //        var details = new UploadDetails
        //        {
        //            filePhoto = string.Empty,
        //            fileSign = string.Empty,
        //            fileDoc = string.Empty,
        //            IDLst = lst,
        //            ApplicationID = Convert.ToInt32(applicationID)
        //        };
        //        flag = _allotmentService.SaveFileDetailsToDB(details);
        //        if (flag == true)
        //            flagFilesSaved = true;
        //    }
        //    if (flagFilesSaved)
        //    {
        //        var applicationID = Request["applicationID"];
        //        var getDetails = _allotmentService.GetDetails(Convert.ToInt32(applicationID));
        //        string strMsg = "Your online application request has been submit sucessfully.";
        //        SMSSend(getDetails.MobNu, strMsg);
        //        var body = "Your online application request has been submit sucessfully.";
        //        EmailHelper emailHelper = new EmailHelper();
        //        emailHelper.Send(getDetails.EmailId, "Online Application Request", body);
        //    }
        //    return Json(flagFilesSaved, JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// Reads Documents list.
        ///// Kendo's DataSourceRequest functionality has not been used as the number of documents would be less, hence its not required.
        ///// </summary>
        ///// <param name="request">Kendo grid internal parameter</param>
        ///// <returns></returns>
        //[AllowAnonymous]
        //public JsonResult GetApplicationChcklstDocuments([DataSourceRequest]DataSourceRequest request, int applicationId)
        //{
        //    var allDocs = _allotmentService.GetApplicationChcklstDocuments(request, applicationId);
        //    var data = allDocs.ToDataSourceResult(request);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        //[AllowAnonymous]
        //public ActionResult GetCompanyType()
        //{
        //    var lst = _generalService.BindDDL(COMPANYTYPE);
        //    return Json(lst, JsonRequestBehavior.AllowGet);
        //}
        //[AllowAnonymous]
        //public ActionResult GetAllBanksforOnline()
        //{
        //    var lst = _allotmentService.GetAllBanksforOnline();
        //    return Json(lst, JsonRequestBehavior.AllowGet);
        //}

        //// To get all flooors type ddl.
        //[AllowAnonymous]
        //public JsonResult GetFloors(int schemeid, int depttID)
        //{
        //    var lst = _allotmentService.GetFloors(schemeid, depttID);
        //    return Json(lst, JsonRequestBehavior.AllowGet);
        //}
        //[AllowAnonymous]
        //public JsonResult GetAreaDetails(int id)
        //{
        //    var lst = _allotmentService.GetAreaDetails(id);
        //    return Json(lst, JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public JsonResult GetEarneshMoney(int id, int deptt, int floor)
        //{
        //    var lst = _allotmentService.GetEarneshMoney(id, deptt, floor);
        //    return Json(lst, JsonRequestBehavior.AllowGet);
        //}
        //[AllowAnonymous]
        //private string PreparePOSTForm(string url, System.Collections.Hashtable data)      // post form
        //{
        //    string formID = "PostForm";
        //    //Build the form using the specified data to be posted.
        //    StringBuilder strForm = new StringBuilder();
        //    strForm.Append("<form id=\"" + formID + "\" name=\"" +
        //                   formID + "\" action=\"" + url +
        //                   "\" method=\"POST\">");

        //    foreach (System.Collections.DictionaryEntry key in data)
        //    {

        //        strForm.Append("<input type=\"hidden\" name=\"" + key.Key +
        //                       "\" value=\"" + key.Value + "\">");
        //    }


        //    strForm.Append("</form>");
        //    //Build the JavaScript which will do the Posting operation.
        //    StringBuilder strScript = new StringBuilder();
        //    strScript.Append("<script language='javascript'>");
        //    strScript.Append("var v" + formID + " = document." +
        //                     formID + ";");
        //    strScript.Append("v" + formID + ".submit();");
        //    strScript.Append("</script>");
        //    //Return the form and the script concatenated.
        //    //(The order is important, Form then JavaScript)
        //    return strForm.ToString() + strScript.ToString();
        //}
        [AllowAnonymous]
        public void OnlinePayment(int id)
        {
            //var lstViewDetails = _allotmentService.SaveTrasOnlineDetails(id);
            //return View(lstViewDetails);
            var lstViewDetails = _allotmentService.SaveTrasOnlineDetails(id);
            string firstName = lstViewDetails.FirstName;
            decimal? amount = lstViewDetails.Amount;
            string productInfo = lstViewDetails.Productinfo;
            string email = lstViewDetails.Email;
            string phone = lstViewDetails.PhoneNumber;
            string surl = ConfigurationManager.AppSettings["surl"];
            string furl = ConfigurationManager.AppSettings["furl"];
            string udf1 = lstViewDetails.Txnid;

            RemotePost myremotepost = new RemotePost();
            string key = "gtKFFx";
            string salt = "eCwWELxi";

            //posting all the parameters required for integration.

            myremotepost.Url = "https://test.payu.in/_payment";
            myremotepost.Add("key", "gtKFFx");
            string txnid = Generatetxnid();
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

        //[AllowAnonymous]
        //public ActionResult Return()
        //{
        //    var onlineApplicationDetailsTrans = new OnlineApplicationDetailsTrans();
        //    return View(onlineApplicationDetailsTrans);
        //}

        //[HttpPost]
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
                        if (lstViewDetails.Status != 4) //if Payment already paid & Page Refreshed.
                        {
                            if (lstViewDetails != null)
                            {
                                onlineApplicationDetailsTrans.StatusName = lstViewDetails.StatusName;
                                onlineApplicationDetailsTrans.FirstName = lstViewDetails.FirstName;
                                onlineApplicationDetailsTrans.Amount = lstViewDetails.Amount;
                                onlineApplicationDetailsTrans.Txnid = lstViewDetails.Txnid;
                                //onlineApplicationDetailsTrans.TrKey = lstViewDetails.Mihpayid;
                                onlineApplicationDetailsTrans.bank_ref_num = lstViewDetails.bank_ref_num;
                                onlineApplicationDetailsTrans.card_type = lstViewDetails.card_type;
                                onlineApplicationDetailsTrans.error = lstViewDetails.error;
                                onlineApplicationDetailsTrans.error_Message = lstViewDetails.error_Message;
                                onlineApplicationDetailsTrans.issuing_bank = lstViewDetails.issuing_bank;
                                onlineApplicationDetailsTrans.OnlineApplicationId = lstViewDetails.OnlineApplicationId;
                                onlineApplicationDetailsTrans.TrKey = lstViewDetails.Mihpayid;
                                onlineApplicationDetailsTrans.name_on_card = lstViewDetails.name_on_card;
                            }
                            //order_id = Request.Form["txnid"];
                            //ViewData["Message"] = "Status is successful. Hash value is matched";
                            //Response.Write("<br/>Hash value matched");
                            //Hash value did not matched
                            var model = _onlineService.GetOnlineApplicationFormById(lstViewDetails.OnlineApplicationId);
                            if (model != null)
                            {
                                MapOnlineApplicationDetails(model, onlineApplicationDetailsTrans);
                            }
                        }
                        else
                        {
                            TempData["PaymentStatus"] = "refresh";
                            return RedirectToAction("SchemeInformation", "Application", new { Area = "Online" });
                        }

                    }
                    TempData["PaymentStatus"] = "success";
                }
                else
                {
                    Response.Write("Hash value did not matched");
                    TempData["PaymentStatus"] = "error";
                    return RedirectToAction("SchemeInformation", "Application", new { Area = "Online" });
                    // osc_redirect(osc_href_link(FILENAME_CHECKOUT, 'payment' , 'SSL', null, null,true));
                }
            }
            catch (Exception ex)
            {
                Response.Write("<span style='color:red'>" + ex.Message + "</span>");
            }
            return View(onlineApplicationDetailsTrans);
            //return View("Return", onlineApplicationDetailsTrans);
            //return RedirectToAction("Return", onlineApplicationDetailsTrans);
        }

        public void MapOnlineApplicationDetails(OnlineFormViewModel model, OnlineApplicationDetailsTrans onlineApplicationDetailsTrans)
        {

            onlineApplicationDetailsTrans.objOnlineFormViewModel.Id = model.Id;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.ApplicationFormId = model.ApplicationFormId;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.SchemeId = model.SchemeId;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.SchemeName = model.SchemeName;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.DepartmentId = model.DepartmentId;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.Department = model.Department;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.Applicant = model.Applicant;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.SigningAuthority = model.SigningAuthority;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.ApplicantType = model.ApplicantType;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.CompanyTypeId = model.CompanyTypeId;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.CompanyType = model.CompanyType;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.FirstName = model.FirstName;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.MiddleName = model.MiddleName;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.LastName = model.LastName;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.Gender = model.Gender;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.MaritalStatus = model.MaritalStatus;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.DOB = model.DOB;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.FatherName = model.FatherName;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.MotherName = model.MotherName;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.OccupationId = model.OccupationId;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.Occupation = model.Occupation;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.ReligionId = model.ReligionId;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.Religion = model.Religion;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.CategoryId = model.CategoryId;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.CategoryName = model.CategoryName;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.MobileNumber = model.MobileNumber;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.PhoneNumber = model.PhoneNumber;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.FaxNumber = model.FaxNumber;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.PermanentAddress = model.PermanentAddress;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.CorrespondingAddress = model.CorrespondingAddress;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.RegisteredOffice = model.RegisteredOffice;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.Email = model.Email;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.AnnualIncome = model.AnnualIncome;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.EarnestMoney = model.EarnestMoney;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.ApplicationFee = model.ApplicationFee;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.TotalAmount = model.TotalAmount;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.PanNumber = model.PanNumber;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.ProcessingCharge = model.ProcessingCharge;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.RefundBankId = model.RefundBankId;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.RefundBank = model.RefundBank;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.RefundInfaverof = model.RefundInfaverof;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.RefundAccountNo = model.RefundAccountNo;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.AreaRangeId = model.AreaRangeId;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.AreaRange = model.AreaRange;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.SignImage = model.SignImage;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.UserImage = model.UserImage;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.PropertyTypeId = model.PropertyTypeId;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.PropertyType = model.PropertyType;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.IFSCCode = model.IFSCCode;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.BranchName = model.BranchName;
            onlineApplicationDetailsTrans.objOnlineFormViewModel.Sector = model.Sector;
            // return onlineApplicationDetailsTrans;
        }

        //public ActionResult ManageOnlineApplication()
        //{
        //    return View();
        //}

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

        #region Update Application Address
        public ActionResult UpdateAddress()
        {
            return View();
        }

        public ActionResult UpdateApplicationAddress(UpdateAddress ObjUpdateAddress)
        {
            bool isUpdated = _allotmentService.UpdateAddress(ObjUpdateAddress);
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        /// <summary>
        /// Fills RID DDL
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDsForApplication([DataSourceRequest] DataSourceRequest Req)
        {
            var RIdlst = _allotmentService.GetRIDsForApplication(Req);
            var JsonResult = Json(RIdlst, JsonRequestBehavior.AllowGet);
            return JsonResult;
        }

        /// <summary>
        /// Get Application Details of applicant on the basis of RID
        /// </summary>
        /// <param name="rId"></param>
        /// <returns></returns>
        public JsonResult GetApplicationDetailsByRid(int rId)
        {
            var data = _allotmentService.GetApplicationDetailsById(rId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Update Details
        public ActionResult UpdateDetails()
        {
            return View();
        }

        public ActionResult UpdateFormDetails(UpdateDetails ObjUpdateDetails)
        {
            bool isUpdated = _allotmentService.UpdateFormDetails(ObjUpdateDetails);
            return RedirectToAction("ManageApplication");
        }

        public JsonResult GetFormDetailsByRid(int rId)
        {
            var data = _allotmentService.GetFormDetailsById(rId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion


        public JsonResult GetSchemeList([DataSourceRequest] DataSourceRequest request)
        {
            var list = _generalService.GetSchemeList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSchemeListAsDataSource([DataSourceRequest] DataSourceRequest request)
        {
            var list = _generalService.GetSchemeListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentListBySchemeId([DataSourceRequest] DataSourceRequest request, int? schemeId)
        {
            var list = _generalService.GetDepartmentListByScheme(request, schemeId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApplicationFormListByScheme([DataSourceRequest] DataSourceRequest request, int? schemeId, int? departmentId)
        {
            var list = _generalService.GetApplicationFormListByScheme(request, schemeId, departmentId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApplicationFormListAsDataSource([DataSourceRequest] DataSourceRequest request, int? schemeId, int? departmentId)
        {
            var list = _generalService.GetApplicationFormListAsDataSource(request, schemeId, departmentId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyIdListByScheme([DataSourceRequest] DataSourceRequest request, int? schemeId, int? departmentId)
        {
            var list = _generalService.GetPropertyListForAllotment(request, schemeId, departmentId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllottedPropertyIdListAsDataSource([DataSourceRequest] DataSourceRequest request, int? schemeId, int? departmentId, int? applicationId)
        {
            var list = _generalService.GetAllottedPropertyIdListAsDataSource(request, schemeId, departmentId, applicationId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApproverIdList([DataSourceRequest] DataSourceRequest request)
        {
            var list = _generalService.GetApproverIdList(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApplicationFormAsDataSourceByApplicationId([DataSourceRequest] DataSourceRequest request, int? applicationId)
        {
            var form = _allotmentService.GetApplicationFormAsDataSourceByApplicationId(request, applicationId);
            return Json(form, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyDetailAsDataSourceByPropertyId([DataSourceRequest] DataSourceRequest request, int? propertyId)
        {
            var property = _allotmentService.GetPropertyDetailAsDataSourceByPropertyId(request, propertyId);
            return Json(property, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllottedPropertyFormListById([DataSourceRequest] DataSourceRequest request, AllotmentViewModel model)
        {
            var data = _allotmentService.GetAllottedPropertyFormListById(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllottedPropertyFormListForApproval([DataSourceRequest] DataSourceRequest request, AllotmentViewModel model)
        {
            var data = _allotmentService.GetAllottedPropertyFormListForApproval(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidatePropertyAndApplicationForm(FormViewModel model)
        {
            int flag = _allotmentService.ValidatePropertyAndApplicationForm(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PropertyAllotmentForApplicationForm(FormViewModel model)
        {
            int flag = _allotmentService.PropertyAllotmentForApplicationForm(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatusListAsDataSource([DataSourceRequest] DataSourceRequest request)
        {
            var list = _generalService.GetStatusMasterAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateAllottedPropertyStatus(AllotmentViewModel model)
        {
            int flag = _allotmentService.UpdateAllottedPropertyStatus(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApplicationFormList([DataSourceRequest]DataSourceRequest request, FormViewModel model)
        {
            var list = _allotmentService.GetApplicationFormList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMasterSearchParameterAsDataSource([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var data = _generalService.GetMasterSearchParameterAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TransferRequest()
        {
            return View();
        }

        public JsonResult GetApplicantDetailsToTransferProperty(PropertyViewModel model)
        {
            PropertyViewModel detail = _allotmentService.GetApplicantDetailsToTransferProperty(model);
            return Json(detail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePropertyTransferDetail(TransferViewModel model)
        {
            int flag = _allotmentService.SavePropertyTransferDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferedPropertyDetailById(TransferViewModel model)
        {
            TransferViewModel detail = _allotmentService.GetTransferedPropertyDetailById(model);
            return Json(detail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegistrationIdListAsDataSource([DataSourceRequest]DataSourceRequest request, DropdownViewModel model)
        {
            var list = _generalService.GetRegistrationIdListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferTypeList()
        {
            var list = _generalService.GetTransferTypeList();
            list.RemoveAt(1);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTransferSubTypeList(int transferTypeId)
        {
            var list = _generalService.GetTransferSubTypeList(transferTypeId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOccupationList()
        {
            var list = _generalService.GetOccupationList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllotteeTypeList()
        {
            var list = _generalService.GetAllotteeTypeList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllotmentRequestByUserIdAsDataSource([DataSourceRequest]DataSourceRequest request, AllotmentViewModel model)
        {
            var list = _allotmentService.GetAllotmentRequestByUserIdAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllottedPropertyListByYearAsDataSource([DataSourceRequest]DataSourceRequest request, AllotmentViewModel model)
        {
            var list = _allotmentService.GetAllottedPropertyListByYearAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllotmentYearAsDataSource([DataSourceRequest]DataSourceRequest request, DropdownViewModel model)
        {
            var list = _generalService.GetAllotmentYearAsDataSource(request,model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsersDepartmentListAsDataSource([DataSourceRequest]DataSourceRequest request, DropdownViewModel model)
        {
            var list = _generalService.GetUsersDepartmentListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}