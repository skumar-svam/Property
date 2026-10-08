using Kendo.Mvc.UI;
using NA.PMS.Service;
using NA.PMS.Service.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using NA.PMS.Web.Controllers;
using NA.PMS.Model;
using System.IO;
using System.Text;
using NA.PMS.Service.Property;
using NA.PMS.Service.TemplateParser;
using OfficeOpenXml;
using System.Configuration;

namespace NA.PMS.Web.Areas.Reports.Controllers
{
    public class ReportController : WebBaseController
    {
        IReportService _reportService;
        IGeneralService _generalService;
        IPropertyAllotmentService _propertyService;
        IPropertyRegistrationService _propertyRegistrationService;
        IRequestService _RequestService;
        ITemplateParserService _templateParserService;
        public ReportController(IReportService reportService, IGeneralService generalService, PropertyAllotmentService propertyService, IPropertyRegistrationService propertyRegistrationService, IRequestService RequestService, ITemplateParserService templateParserService)
        {
            _reportService = reportService;
            _generalService = generalService;
            _propertyService = propertyService;
            _propertyRegistrationService = propertyRegistrationService;
            _RequestService = RequestService;
            _templateParserService = templateParserService;
        }

        // GET: Reports/Report
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GenerateLetter()
        {
            return View();
        }

        public ActionResult PropertyReport()
        {
            return View();
        }

        public ActionResult VacantProperty()
        {
            return View();
        }

        /// <summary>
        /// Returns the View for GPA Report
        /// </summary>
        /// <returns></returns>
        public ActionResult GPAReport()
        {
            return View();
        }

        /// <summary>
        /// Returns View for Nominee Report
        /// </summary>
        /// <returns></returns>
        public ActionResult NomineeReport()
        {
            return View();
        }

        public ActionResult MortgageReport()
        {
            return View();
        }

        public ActionResult FunctionalReport(string returnurl)
        {
            return View();
        }

        public ActionResult PossessionReport(string possession)
        {
            return View();
        }

        public ActionResult CompletionReport(string returnurl)
        {
            return View();
        }

        public ActionResult PendencyReport()
        {
            return View();
        }

        public ActionResult VacantPropertyReport()
        {
            return View();
        }

        public ActionResult SamadhanDiwasReport()
        {
            return View();
        }

        public ActionResult CustomerServiceReport()
        {
            return View();
        }

        public ActionResult NoDuesCertificate()
        {
            return View();
        }

        public ActionResult ManageNDC()
        {
            return View();
        }

        public ActionResult UpdateData()
        {
            return View();
        }


        public ActionResult DashboardGraph()
        {
            int? ReqType = 1;
            int? UserDept = 1;
            string Graph = _reportService.GetDashboardGraph(ReqType, UserDept);
            DashBoardGraph objDashBoardGraph = new DashBoardGraph();
            objDashBoardGraph.Graph = Graph;
            return View("DashboardGraph", objDashBoardGraph);
        }

        /// <summary>
        /// Returns View for Transferred Property Report
        /// </summary>
        /// <returns></returns>
        public ActionResult TransferredPropertiesReport()
        {
            return View();
        }

        /// <summary>
        /// SMS Log of Monthly Dues Reminder
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageReminder()
        {
            return View();
        }

        public ActionResult ServiceReport()
        {
            ServiceViewModel service = new ServiceViewModel();
            var htmlContent = _reportService.GetServiceReportForAllDepartment();
            service.HtmlReport = htmlContent;
            return View(service);
        }

        public JsonResult GetHTMLServiceReport()
        {
            ServiceViewModel service = new ServiceViewModel();
            var htmlContent = _reportService.GetServiceReportForAllDepartment();
            service.HtmlReport = htmlContent;
            return Json(service.HtmlReport, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHTMLServiceReportLetter()
        {
            ServiceViewModel service = new ServiceViewModel();
            var htmlContent = _reportService.GetServiceReportLetter();
            service.HtmlReport = htmlContent;
            return Json(service.HtmlReport, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RedirectToDemandAndNDC(string filter, int? id)
        {
            if (id != null) Session["DNDCDepartmentId"] = id;
            if(filter == "ndc") return RedirectToAction("ManageNDC");
            else return RedirectToAction("ManageDemand", new { controller = "Payment", area = "Revenue" });
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        /// <summary>
        /// Getting all Properties Detail which are not allotted and are not InProgress
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropertyReport([DataSourceRequest] DataSourceRequest request, int? deptId, string propBank, string schemeId)
        {
            var propDetail = _reportService.GetPropertiesForReport(request, deptId, propBank, schemeId);
            return Json(propDetail, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Pdf_Export_Read([DataSourceRequest]DataSourceRequest request, int? deptId, string propBank)
        //{
        //    var propDetail = _reportService.GetPropertiesForReport(request, deptId, propBank);
        //    return Json(propDetail, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        //{
        //    var fileContents = Convert.FromBase64String(base64);

        //    return File(fileContents, contentType, fileName);
        //}
        public JsonResult GetLettersByTemplateDepartment(int? departmentId)
        {
            var lst = _generalService.GetLettersByTemplateDepartment(departmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLettersTemplateByDepartment([DataSourceRequest] DataSourceRequest request, int? departmentId)
        {
            var lst = _generalService.GetLettersTemplateByDepartment(request, departmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceRequestStatusForLetter(int? rid, int? letterId)
        {
            var lst = _generalService.GetServiceRequestStatusForLetter(rid, letterId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetServiceRequestStatus(int? requestId)
        {
            var lst = _generalService.GetServiceRequestStatus(requestId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GenerateLetterByService(int? rid, int? departmentId, int? serviceId, int? letterId, DateTime? letterDate)
        {
            var lst = _generalService.GenerateLetterByService(rid, departmentId, serviceId, letterId, letterDate);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPropertyDetailsByRid(string rid)
        {
            var lst = _generalService.GetAllotteDetailsToGenerateLetter(Convert.ToInt32(rid));
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllotteDetailsToGenerateLetter(int? rid)
        {
            var lst = _generalService.GetAllotteDetailsToGenerateLetter(rid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllLettersType()
        {
            var lst = _generalService.GetAllLettersType();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetServicesRequestListByRid(int? rid)
        {
            var lst = _generalService.GetServicesRequestListByRid(rid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetServicesByDepartment(int? departmentId)
        {
            var lst = _generalService.GetServicesByDepartment(departmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegistrationNoByDepartment([DataSourceRequest] DataSourceRequest request)
        {
            var lst = _generalService.GetRegistrationIdByDepartment(request);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentList()
        {
            var lst = _generalService.GetAllDepartments();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentListAsDataSource([DataSourceRequest] DataSourceRequest request)
        {
            var list = _generalService.GetDepartmentListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStatusMasterAsDataSource([DataSourceRequest] DataSourceRequest request)
        {
            var list = _generalService.GetStatusMasterAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentsByUser()
        {
            var lst = _generalService.GetDepartmentsByUser();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPropertyBank()
        {
            var lst = _reportService.GetPropertyBank();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSchemeListForAllotment()
        {
            List<SchemeAllotmentModel> schemeList = _propertyService.GetSchemeListForAllotment();
            return Json(schemeList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Kendo read function for GPA Report
        /// </summary>
        /// <param name="req">Kendo internal parameter</param>
        /// <param name="fromSearch">From Date</param>
        /// <param name="toSearch">To Date</param>
        /// <param name="depttId">Department ID</param>
        /// <returns></returns>
        public JsonResult GetGPAReportData([DataSourceRequest] DataSourceRequest req, DateTime? fromSearch, DateTime? toSearch, int? depttId)
        {
            var data = _reportService.GetGPAReportData(req, fromSearch, toSearch, depttId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Kendo read function for Nominee Report
        /// </summary>
        /// <param name="req">Kendo internal parameter</param>
        /// <param name="fromSearch">From Date</param>
        /// <param name="toSearch">To Date</param>
        /// <param name="depttId">Department ID</param>
        /// <returns></returns>
        public JsonResult GetNomineeReportData([DataSourceRequest] DataSourceRequest req, DateTime? fromSearch, DateTime? toSearch, int? depttId)
        {
            var data = _reportService.GetNomineeReportData(req, fromSearch, toSearch, depttId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepttsByUserId()
        {
            var lst = _generalService.GetDepartmentsByUser();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyFuctinalReport([DataSourceRequest]DataSourceRequest request, string status)
        {
            var data = _reportService.GetPropertyFuctinalReport(request, status);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFunctionalStatus()
        {
            List<DDList> data = _generalService.GetFunctionalStatus();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCompletedSchemeToSearch()
        {
            List<DDList> data = _generalService.GetCompletedSchemeToSearch();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SearchFunctionalReport([DataSourceRequest]DataSourceRequest request, string functionalDetail, string department, string scheme, string sector, DateTime? startDate, DateTime? endDate)
        {
            DataSourceResult data = _reportService.GetSearchedFunctionalReport(request, functionalDetail, department, scheme, sector, startDate, endDate);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentListForLoginUser()
        {
            List<DDList> data = _generalService.GetDepartmentListForLoginUser();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AreaChangeOnPossession()
        {
            List<DDList> data = _generalService.AreaChangeOnPossession();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PropertyPossessionStatus()
        {
            List<DDList> data = _generalService.PropertyPossessionStatus();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SearchPossessionReport([DataSourceRequest]DataSourceRequest request, string department, string scheme, string possession, string areaChange, string sector, DateTime? startDate, DateTime? endDate)
        {
            DataSourceResult data = _reportService.SearchPossessionReport(request, department, scheme, possession, areaChange, sector, startDate, endDate);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPossessionReport([DataSourceRequest]DataSourceRequest request)
        {
            DataSourceResult data = _reportService.GetPossessionReport(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchCompletionReportData([DataSourceRequest]DataSourceRequest request, string completion, string department, string scheme, string sector, DateTime? startDate, DateTime? endDate)
        {
            DataSourceResult data = _reportService.SearchCompletionReportData(request, completion, department, scheme, sector, startDate, endDate);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPropertyCompletionStatus()
        {
            List<DDList> data = _generalService.GetPropertyCompletionStatus();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMortgageReports([DataSourceRequest]DataSourceRequest request, DateTime? fromDate = null, DateTime? toDate = null, int? schemeId = null, int? departmentId = null)
        {
            var mortgageDetail = _reportService.GetMortgageReports(request, fromDate, toDate, schemeId, departmentId);
            return Json(mortgageDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferReportData([DataSourceRequest] DataSourceRequest req, DateTime? fromSearch, DateTime? toSearch, int? schemeId, int? depttId, int? transType, int? transSubType)
        {
            var data = _reportService.GetTransferReportData(req, fromSearch, toSearch, schemeId, depttId, transType, transSubType);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadLetter(int? rid, int? departmentId, int? serviceId, int? letterId, DateTime? letterDate)
        {
            var letter = string.Empty;
            if (letterId != null)
            {
                letter = _generalService.GenerateLetterByService(rid, departmentId, null, letterId, letterDate);
            }
            byte[] contents = Encoding.ASCII.GetBytes(letter);
            return File(contents, "application/msword", "AuthorityLetter.doc");
        }

        public ActionResult ShowServiceRequestMatrix()
        {
            string ServiceRequestMatrix = string.Empty; //_reportService.GetServiceRequestMatrix(null, null, DateTime.Now.AddDays(-8), DateTime.Now);
            UserWiseRequest objUserWiseRequest = new UserWiseRequest();
            objUserWiseRequest.Matrix = ServiceRequestMatrix;
            return View("ShowServiceRequestMatrix", objUserWiseRequest);
        }

        public JsonResult GetServiceRequestMatrixBySearch(int? departmentid, int? serviceId, DateTime? FromDate, DateTime? ToDate)
        {
            var data = _reportService.GetServiceRequestMatrix(departmentid, serviceId, FromDate, ToDate);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserWiseRequest_Read([DataSourceRequest] DataSourceRequest req, UserWiseRequest objUserWiseRequest)
        {
            var data = _reportService.GetUserWiseRequest(req, objUserWiseRequest);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPendencyReport_Read([DataSourceRequest] DataSourceRequest Req)
        {
            var data = _reportService.GetPendencyReport(Req);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApplicantDetailsForNDC(string registrationId)
        {
            NDCVeiwModel details = _generalService.GetApplicantDetailsForNDC(registrationId);
            return Json(details, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetYesNoStatus()
        {
            var stats = _generalService.GetYesNoStatus();
            return Json(stats, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApplicantPremiumDues(string registrationId)
        {
            var premium = _reportService.GetApplicantPremiumDuesByRegistrationId(registrationId);
            return Json(premium, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLeaseRentDate(string registrationId)
        {
            var premium = _reportService.GetLeaseRentDateByRegistrationId(registrationId);
            return Json(premium, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveDetailForNDC(NDCVeiwModel model)
        {
            var premium = _reportService.SaveDetailForNDC(model);
            TempData["ndc"] = premium;
            return RedirectToAction("ManageNDC");
            //return Json(null);
        }

        public JsonResult GetNoDuesCertificateList([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult ndclist = _reportService.GetNoDuesCertificateList(request);
            return Json(ndclist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateRegistrationIdByRequestNo(int requestNo, string registrationId)
        {
            int flag = _reportService.UpdateRegistrationIdByRequestNo(requestNo, registrationId);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateServiceRequestDetail(ServiceRequestModel model)
        {
            int flag = _reportService.UpdateServiceRequestDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllotteeDetailsByRid(string registrationId, string flag1, string flag2)
        {
            var data = _reportService.GetAllotteDetailsByRegistrationId(registrationId, flag1, flag2);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateAllotteeBasicInfo(PropertyDetailViewModel model)
        {
            var data = _reportService.UpdateAllotteeBasicInfo(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateTransferDetail(PropertyDetailViewModel model)
        {
            var data = _reportService.UpdateTransferDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferDetailsByRid(string registrationId, string flag)
        {
            var data = _reportService.GetTransferDetailsByRegistrationId(registrationId, flag);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegistrationIdList([DataSourceRequest] DataSourceRequest Request)
        {
            var allRIds = _generalService.GetRegistrationIdList(Request);
            return Json(allRIds, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailsByRegistrationId(string registrationId, string flag)
        {
            var detail = _reportService.GetMultipleDetailsByRegistrationId(registrationId, flag);
            return Json(detail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDateFieldsByRegistrationId(string registrationId, DateTime? firstDate, DateTime? secondDate, string flag)
        {
            var data = _reportService.UpdateDateFieldsByRegistrationId(registrationId, firstDate, secondDate, flag);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyIdList([DataSourceRequest] DataSourceRequest Request)
        {
            var pidList = _generalService.GetPropertyIdList(Request);
            return Json(pidList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllSectors()
        {
            var sectors = _generalService.GetAllSectors();
            return Json(sectors, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllBlocks()
        {
            var blocks = _generalService.GetAllBlocks();
            return Json(blocks, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllFloorTypes()
        {
            var blocks = _generalService.GetAllFloors();
            return Json(blocks, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMortgageType()
        {
            var mortgageType = _generalService.GetMortgageTypeList();
            return Json(mortgageType, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPreviousLoanNoc()
        {
            var mortgageLoan = _generalService.GetMortgageLoanStatus();
            //var mortgageLoan = _generalService.GetYesNoStatus();
            return Json(mortgageLoan, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatusMasterList()
        {
            var mortgageLoan = _generalService.getStatusMasterList();
            return Json(mortgageLoan, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllRegistryType()
        {
            var registry = _generalService.GetRegistry();
            return Json(registry, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyDetailsById(int? propertyId)
        {
            var data = _reportService.GetPropertyDetailsById(propertyId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePropertyDetail(PropertyDetailViewModel model)
        {
            int flag = _reportService.UpdatePropertyDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegistrationIdByRequestNo(int? requestNo)
        {
            var data = _reportService.GetRegistrationIdByRequestNo(requestNo);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceRequestDetailById(int? id)
        {
            var data = _reportService.GetServiceRequestDetailById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMortgageDetailsByRid(int? registrationId)
        {
            var mortgage = _reportService.GetMortgageDetailsByRid(registrationId);
            return Json(mortgage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateMortgageDetail(MortgageViewModel model)
        {
            var flag = _reportService.UpdateMortgageDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSectorBlock()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Sector", Value = "Sector" });
            list.Add(new SelectListItem { Text = "Block", Value = "Block" });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSectorBlockName(string type, string typeName)
        {
            var flag = _generalService.SaveSectorBlockName(type, typeName);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Getting all Vacant Properties Detail By Department Id
        /// </summary>
        /// <returns></returns>        
        public JsonResult VancantPropertyReport([DataSourceRequest] DataSourceRequest request, int? deptId)
        {
            var propDetail = _reportService.GetVacantProperties(request, deptId);
            return Json(propDetail, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetLeaseRentRevisedPercentage()
        {
            List<DropdownViewModel> objlist = new List<DropdownViewModel>();
            for (int i = 1; i <= 100; i++)
            {
                DropdownViewModel dplist = new DropdownViewModel();
                dplist.Id = i;
                dplist.Text = Convert.ToString(i);
                objlist.Add(dplist);
            }
            return Json(objlist, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetSamadhanDiwasRequestReport([DataSourceRequest]DataSourceRequest request, int? departmentId, DateTime? fromDate, DateTime? toDate)
        //{
        //    var reports = _RequestService.GetServiceRequestReport(request, departmentId, fromDate, toDate);
        //    return Json(reports, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetSamadhanDiwasRequestReport([DataSourceRequest]DataSourceRequest request, ServiceViewModel model)
        {
            var list = _RequestService.GetPradhikaranDiwasRequestList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerServiceRequestList([DataSourceRequest]DataSourceRequest request, ServiceViewModel model)
        {
            var list = _RequestService.GetCustomerServiceRequestList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetServiceListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVacantPropertyReport([DataSourceRequest]DataSourceRequest request, PropertyViewModel model)
        {
            var list = _reportService.GetVacantPropertyReport(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApplicationIdListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var list = _generalService.GetApplicationIdListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegistrationIdByApplicationIdOrFormNo(PropertyViewModel model)
        {
            var data = _reportService.GetRegistrationIdByApplicationIdOrFormNo(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateRegistrationIdByApplicationIdOrFormNo(PropertyViewModel model)
        {
            var data = _reportService.UpdateRegistrationIdByApplicationIdOrFormNo(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ServiceStatusReport()
        {
            return View();
        }

        public JsonResult GetServiceStatusReport([DataSourceRequest]DataSourceRequest request, ServiceReportDepartmentWiseVM model)
        {
            var data = _reportService.GetServiceReportDepartmentWise(model);
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerateNDCLetter(NDCVeiwModel model)
        {
            string letter = _templateParserService.GenerateNDCLetter(model, "NDCIndustryTemplate.cshtml");
            return Json(letter, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceReportsAsDataSource([DataSourceRequest]DataSourceRequest request, ServiceViewModel model)
        {
            var data = _reportService.GetServiceReportsAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceReportsForGraph(ServiceViewModel model)
        {
            var data = _reportService.GetServiceReportsForGraph(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceReportsForGraphII(ServiceViewModel model)
        {
            var data = _reportService.GetServiceReportsForGraphII(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageServices(int? id, string filterType, string department, string serviceName,DateTime? startDate, DateTime? endDate)
         {
            ServiceViewModel model = new ServiceViewModel();
            model.Id = id;
            model.ServiceStatusId = id;
            model.Department = department;
            model.ServiceName = serviceName;
            model.FilterType = filterType;
            model.RequestThrough = filterType;
            model.StartDate = startDate;
            model.EndDate = endDate;
            ServiceViewModel data = _reportService.GetMultipleTypeIdToRedirect(model);
            return View(data);
        }

        public ActionResult ManageKYA(int? id, string filterType, string department, string status, DateTime? startDate, DateTime? endDate)
        {
            ServiceViewModel model = new ServiceViewModel();
            model.Id = id;
            //model.ActionId = id;
            model.Department = department=="" ? null : department;
            model.Status = status;
            model.FilterType = filterType;
            model.StartDate = startDate;
            model.EndDate = endDate;
            ServiceViewModel data = _reportService.GetMultipleTypeIdToRedirect(model);
            KYAViewModel kya = new KYAViewModel { ActionId=model.StatusId, Department=model.Department, DepartmentId=model.DepartmentId,Status=model.Status,StatusId=model.StatusId,StartDate=model.StartDate,EndDate=model.EndDate,FilterType=model.FilterType};
            return View(kya);
        }

        public ActionResult Demand(int? id, string filterType, string department, string serviceName, DateTime? startDate, DateTime? endDate)
        {
            ServiceViewModel model = new ServiceViewModel();
            model.Id = id;
            model.ServiceStatusId = id;
            model.Department = department;
            model.ServiceName = serviceName;
            model.FilterType = filterType;
            model.RequestThrough = filterType;
            model.StartDate = startDate;
            model.EndDate = endDate;
            ServiceViewModel data = _reportService.GetMultipleTypeIdToRedirect(model);
            PaymentViewModel demand = new PaymentViewModel { DepartmentId=model.DepartmentId,Department=model.Department, StartDate=model.StartDate,EndDate=model.EndDate};
            return View(demand);
        }

        public ActionResult NDC(int? id, string filterType, string department, string serviceName, DateTime? startDate, DateTime? endDate)
        {
            ServiceViewModel model = new ServiceViewModel();
            model.Id = id;
            model.ServiceStatusId = id;
            model.Department = department;
            model.ServiceName = serviceName;
            model.FilterType = filterType;
            model.RequestThrough = filterType;
            model.StartDate = startDate;
            model.EndDate = endDate;
            ServiceViewModel data = _reportService.GetMultipleTypeIdToRedirect(model);
            NDCVeiwModel ndc = new NDCVeiwModel { DepartmentId=data.DepartmentId,Department=model.Department,StartDate=model.StartDate,EndDate=model.EndDate};
            return View(ndc);
        }

        //public ActionResult Challan(int? id, string filterType, string department, string serviceName, DateTime? startDate, DateTime? endDate)
        //{
        //    ServiceViewModel model = new ServiceViewModel();
        //    model.Id = id;
        //    model.ServiceStatusId = id;
        //    model.Department = department;
        //    model.ServiceName = serviceName;
        //    model.FilterType = filterType;
        //    model.RequestThrough = filterType;
        //    model.StartDate = startDate;
        //    model.EndDate = endDate;
        //    ServiceViewModel data = _reportService.GetMultipleTypeIdToRedirect(model);
        //    return View(data);
        //}

        public ActionResult Challan()
        {
            return View();
        }

        public JsonResult GetCustomerServiceRequestDataAsDataSource([DataSourceRequest]DataSourceRequest request, ServiceViewModel model)
        {
            var list = _reportService.GetCustomerServiceRequestDataAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RedirectServiceRequestByType(int? id, string filterType, string department)
        {
            ServiceViewModel model = new ServiceViewModel();
            model.Id = id;
            model.Department = department;
            model.FilterType = filterType;//Online, jsk
            //if (id != null) Session["ServiceStatusId"] = id;
            //if (service != null) Session["ServiceName"] = service;
            //if (filter == "Online") return RedirectToAction("Online");
            //else if (filter == "jsk") return RedirectToAction("JSKServices");
            return RedirectToAction("ManageServices", new { id=id,filterType=filterType,department=department});
        }

        public JsonResult GetMultipleTypeIdToRedirect(ServiceViewModel model)
        {
            ServiceViewModel data = _reportService.GetMultipleTypeIdToRedirect(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatusMasterListAsDataSource([DataSourceRequest]DataSourceRequest request)
        {
            var mortgageLoan = _generalService.GetStatusMasterAsDataSource(request);
            return Json(mortgageLoan, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKYAReportDataAsDataSource([DataSourceRequest]DataSourceRequest request, KYAViewModel model)
        {
            var list = _reportService.GetKYAReportDataAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChallanReportDataAsDataSource([DataSourceRequest]DataSourceRequest request, ChallanViewModel model)
        {
            var list = _reportService.GetChallanReportDataAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDemandNotesReportDataAsDataSource([DataSourceRequest]DataSourceRequest request, PaymentViewModel model)
        {
            var list = _reportService.GetDemandNotesReportDataAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNDCReportDataAsDataSource([DataSourceRequest]DataSourceRequest request, NDCVeiwModel model)
        {
            var list = _reportService.GetNDCReportDataAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetServiceTimelineReportsAsDataSource([DataSourceRequest]DataSourceRequest request, ServiceViewModel model)
        {
            var list = _reportService.GetServiceTimelineReportsAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetCitizenCharterTimelineList([DataSourceRequest]DataSourceRequest request)
        {
            var list = _reportService.GetCitizenCharterTimelineList(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSMSLogList([DataSourceRequest]DataSourceRequest request)
        {
            var list = _reportService.GetSMSLogList(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsRidExists(int? Id, int? RegistrationId, string PropertyNo)
        {
            var flag = _generalService.IsRidExists(Id, RegistrationId, PropertyNo);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendReportsInExcelFormat(PropertyViewModel model)
        {
            int flag = _reportService.SendReportsInExcelFormat(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadReportsInExcelFormat(PropertyViewModel model)
        {
            var flag = _reportService.DownloadReportsInExcelFormat(model);
            //return Json(flag, JsonRequestBehavior.AllowGet);
            var filePath = "D:\\FunctionalReportPackage.xlsx";
            var fileName = "ExcellData.xlsx";
            var mimeType = "application/vnd.ms-excel";
            return File(new FileStream(filePath, FileMode.Open),mimeType, fileName);

            //using (ExcelPackage pck = new ExcelPackage(flag)) //load the pck again
            //{
            //    {
            //var DownloadFile = flag.GetAsByteArray();
            //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; ;
            //        Response.AddHeader("content-disposition", "attachment; filename=" + Path.GetFileName(filePath));
            //        Response.AddHeader("content-lenght", DownloadFile.Length.ToString());
            //        Response.BufferOutput = true;
            //        Response.OutputStream.Write(DownloadFile, 0, DownloadFile.Length); // i get a 131kb file.. double the size
            //        Response.Flush();
            //        Response.End();
            //    }
            //}
                    //using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    //{
                    //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; ;
                    //    Response.AddHeader("content-disposition", "attachment; filename=" + Path.GetFileName(filePath));
                    //    Response.AddHeader("content-lenght", fs.Length.ToString());
                    //    Response.BufferOutput = true;
                    //    Response.OutputStream.Write(fs, 0, fs.Length); // i get a 131kb file.. double the size
                    //    Response.Flush();
                    //    Response.End();
                    //    flag.SaveAs(fs);
                    //}
                    //return Json(flag,JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);//<add key="UploadFilePath" value="~\UploadFiles\" />
            //string path = ConfigurationManager.AppSettings["LocalUploadFilePath"];
            //System.IO.File.WriteAllBytes(path + fileName, fileContents);
            int flag = _RequestService.SaveExportedDocument(contentType, base64, fileName);
            return File(fileContents, contentType, fileName);
        }
    }
}