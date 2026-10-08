using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Repository;
using NA.PMS.Repository.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Service.Reports
{
    public class ReportService : IReportService
    {
        IReportRepository _reportRepository;

        public ReportService()
        {
            _reportRepository = new ReportRepository();
        }
        public DataSourceResult GetPropertiesForReport(DataSourceRequest request, int? deptId, string propBank, string schemeId)
        {
            return _reportRepository.GetPropertiesForReport(request, deptId, propBank, schemeId);
        }
        public List<DDList> GetPropertyBank()
        {
            return _reportRepository.GetPropertyBank();
        }

        public DataSourceResult GetGPAReportData(DataSourceRequest req, DateTime? fromSearch, DateTime? toSearch, int? depttId)
        {
            return _reportRepository.GetGPAReportData(req, fromSearch, toSearch, depttId);
        }

        public DataSourceResult GetNomineeReportData(DataSourceRequest req, DateTime? fromSearch, DateTime? toSearch, int? depttId)
        {
            return _reportRepository.GetNomineeReportData(req, fromSearch, toSearch, depttId);
        }

        public DataSourceResult GetTransferReportData(DataSourceRequest req, DateTime? fromSearch, DateTime? toSearch, int? schemeId, int? depttId, int? transType, int? transSubType)
        {
            return _reportRepository.GetTransferReportData(req, fromSearch, toSearch, schemeId, depttId, transType, transSubType);
        }

        public DataSourceResult GetPropertyFuctinalReport(DataSourceRequest request, string status)
        {
            return _reportRepository.GetPropertyFuctinalReport(request, status);
        }

        // Get All Mortgage Reports
        public DataSourceResult GetMortgageReports(DataSourceRequest request, DateTime? fromDate, DateTime? toDate, int? schemeId, int? departmentId)
        {
            return _reportRepository.GetMortgageReports(request, fromDate, toDate, schemeId, departmentId);
        }


        public DataSourceResult GetSearchedFunctionalReport(DataSourceRequest request, string functionalDetail, string department, string scheme, string sector, DateTime? startDate, DateTime? endDate)
        {
            return _reportRepository.GetSearchedFunctionalReport(request, functionalDetail, department, scheme, sector, startDate, endDate);
        }


        public DataSourceResult SearchPossessionReport(DataSourceRequest request, string department, string scheme, string possession, string areaChange, string sector, DateTime? startDate, DateTime? endDate)
        {
            return _reportRepository.SearchPossessionReport(request, department, scheme, possession, areaChange, sector, startDate, endDate);
        }

        public DataSourceResult GetPossessionReport(DataSourceRequest request)
        {
            return _reportRepository.GetPossessionReport(request);
        }


        public DataSourceResult SearchCompletionReportData(DataSourceRequest request, string completion, string department, string scheme, string sector, DateTime? startDate, DateTime? endDate)
        {
            return _reportRepository.SearchCompletionReportData(request, completion, department, scheme, sector, startDate, endDate);
        }


        public DataSourceResult GetCustomerServiceReport(DataSourceRequest request, int? departmentId, DateTime? startDate, DateTime? endDate, int? status)
        {
            return _reportRepository.GetCustomerServiceReport(request, departmentId, startDate, endDate, status);
        }

        public string GetDashboardGraph(int? ReqType, int? UserDept)
        {
            return _reportRepository.GetDashboardGraph(ReqType, UserDept);
        }
        public string GetServiceRequestMatrix(int? departmentid, int? serviceId, DateTime? FromDate, DateTime? ToDate)
        {
            return _reportRepository.GetServiceRequestMatrix(departmentid, serviceId, FromDate, ToDate);
        }
        public DataSourceResult GetUserWiseRequest(DataSourceRequest Req, UserWiseRequest objUserWiseRequest)
        {
            return _reportRepository.GetUserWiseRequest(Req, objUserWiseRequest);
        }

        public DataSourceResult GetPendencyReport(DataSourceRequest request)
        {
            return _reportRepository.GetPendencyReport(request);
        }


        public CommonViewModel GetApplicantPremiumDuesByRegistrationId(string registrationId)
        {
            return _reportRepository.GetApplicantPremiumDuesByRegistrationId(registrationId);
        }

        public CommonViewModel GetLeaseRentDateByRegistrationId(string registrationId)
        {
            return _reportRepository.GetLeaseRentDateByRegistrationId(registrationId);
        }


        public int SaveDetailForNDC(NDCVeiwModel model)
        {
            return _reportRepository.SaveDetailForNDC(model);
        }


        public DataSourceResult GetNoDuesCertificateList(DataSourceRequest request)
        {
            return _reportRepository.GetNoDuesCertificateList(request);
        }


        public int UpdateRegistrationIdByRequestNo(int requestNo, string registrationId)
        {
            return _reportRepository.UpdateRegistrationIdByRequestNo(requestNo, registrationId);
        }


        public PropertyDetailViewModel GetAllotteDetailsByRegistrationId(string registrationId, string flag1, string flag2)
        {
            return _reportRepository.GetAllotteDetailsByRegistrationId(registrationId, flag1, flag2);
        }


        public int UpdateAllotteeBasicInfo(PropertyDetailViewModel model)
        {
            return _reportRepository.UpdateAllotteeBasicInfo(model);
        }

        public int UpdateTransferDetailById(PropertyDetailViewModel model)
        {
            return _reportRepository.UpdateTransferDetailById(model);
        }


        public PropertyDetailViewModel GetTransferDetailsByRegistrationId(string registrationId, string flag)
        {
            return _reportRepository.GetTransferDetailsByRegistrationId(registrationId, flag);
        }


        public PropertyDetailViewModel GetMultipleDetailsByRegistrationId(string registrationId, string flag)
        {
            return _reportRepository.GetMultipleDetailsByRegistrationId(registrationId, flag);
        }


        public int UpdateDateFieldsByRegistrationId(string registrationId, DateTime? firstDate, DateTime? secondDate, string flag)
        {
            return _reportRepository.UpdateDateFieldsByRegistrationId(registrationId, firstDate, secondDate, flag);
        }


        public SchemePropertyModel GetPropertyDetailsById(int? propertyId)
        {
            return _reportRepository.GetPropertyDetailsById(propertyId);
        }


        public int UpdatePropertyDetail(PropertyDetailViewModel model)
        {
            return _reportRepository.UpdatePropertyDetail(model);
        }


        public string GetRegistrationIdByRequestNo(int? requestNo)
        {
            return _reportRepository.GetRegistrationIdByRequestNo(requestNo);
        }

        public DataSourceResult GetVacantProperties(DataSourceRequest Req, int? DepartmentId)
        {
            return _reportRepository.GetVacantProperties(Req, DepartmentId);
        }


        public MortgageViewModel GetMortgageDetailsByRid(int? registrationId)
        {
            return _reportRepository.GetMortgageDetailsByRid(registrationId);
        }


        public int UpdateMortgageDetail(MortgageViewModel model)
        {
            return _reportRepository.UpdateMortgageDetail(model);
        }


        public int UpdateServiceRequestDetail(ServiceRequestModel model)
        {
            return _reportRepository.UpdateServiceRequestDetail(model);
        }


        public ServiceRequestModel GetServiceRequestDetailById(int? id)
        {
            return _reportRepository.GetServiceRequestDetailById(id);
        }


        public DataSourceResult GetVacantPropertyReport(DataSourceRequest request, PropertyViewModel model)
        {
            return _reportRepository.GetVacantPropertyReport(request, model);
        }


        public PropertyViewModel GetRegistrationIdByApplicationIdOrFormNo(PropertyViewModel model)
        {
            return _reportRepository.GetRegistrationIdByApplicationIdOrFormNo(model);
        }

        public int UpdateRegistrationIdByApplicationIdOrFormNo(PropertyViewModel model)
        {
            return _reportRepository.UpdateRegistrationIdByApplicationIdOrFormNo(model);
        }

        public List<ServiceReportDepartmentWiseVM> GetServiceReportDepartmentWise(ServiceReportDepartmentWiseVM serviceReport)
        {
            return _reportRepository.GetServiceReportDepartmentWise(serviceReport);
        }


        public DataSourceResult GetKYAReportsAsDataSource(DataSourceRequest request, KYAViewModel model)
        {
            return _reportRepository.GetKYAReportsAsDataSource(request, model);
        }

        public DataSourceResult GetChallanReportsAsDataSource(DataSourceRequest request, ChallanViewModel model)
        {
            return _reportRepository.GetChallanReportsAsDataSource(request, model);
        }

        public DataSourceResult GetServiceReportsAsDataSource(DataSourceRequest request, ServiceViewModel model)
        {
            return _reportRepository.GetServiceReportsAsDataSource(request, model);
        }

        public IEnumerable<KYAViewModel> GetKYAReportsForGraph(KYAViewModel model)
        {
            return _reportRepository.GetKYAReportsForGraph(model);
        }

        public IEnumerable<ChallanViewModel> GetChallanReportsForGraph(ChallanViewModel model)
        {
            return _reportRepository.GetChallanReportsForGraph(model);
        }

        public IEnumerable<ServiceViewModel> GetServiceReportsForGraph(ServiceViewModel model)
        {
            return _reportRepository.GetServiceReportsForGraph(model);
        }



        public DataSourceResult GetDemandAndNDCListByDepartmentAsDataSource(DataSourceRequest request, NDCVeiwModel model)
        {
            return _reportRepository.GetDemandAndNDCListByDepartmentAsDataSource(request, model);
        }

        public IEnumerable<NDCVeiwModel> GetNDCAndDemandReportsForGraph(NDCVeiwModel model)
        {
            return _reportRepository.GetNDCAndDemandReportsForGraph(model);
        }


        public IEnumerable<ServiceViewModel> GetServiceReportsForGraphII(ServiceViewModel model)
        {
            return _reportRepository.GetServiceReportsForGraphII(model);
        }


        public DataSourceResult GetCustomerServiceRequestDataAsDataSource(DataSourceRequest request, ServiceViewModel model)
        {
            return _reportRepository.GetCustomerServiceRequestDataAsDataSource(request, model);
        }


        public ServiceViewModel GetMultipleTypeIdToRedirect(ServiceViewModel model)
        {
            return _reportRepository.GetMultipleTypeIdToRedirect(model);
        }


        public DataSourceResult GetKYAReportDataAsDataSource(DataSourceRequest request, KYAViewModel model)
        {
            return _reportRepository.GetKYAReportDataAsDataSource(request, model);
        }

        public DataSourceResult GetChallanReportDataAsDataSource(DataSourceRequest request, ChallanViewModel model)
        {
            return _reportRepository.GetChallanReportDataAsDataSource(request, model);
        }

        public DataSourceResult GetDemandNotesReportDataAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            return _reportRepository.GetDemandNotesReportDataAsDataSource(request, model);
        }

        public DataSourceResult GetNDCReportDataAsDataSource(DataSourceRequest request, NDCVeiwModel model)
        {
            return _reportRepository.GetNDCReportDataAsDataSource(request, model);
        }


        public DataSourceResult GetServiceTimelineReportsAsDataSource(DataSourceRequest request, ServiceViewModel model)
        {
            return _reportRepository.GetServiceTimelineReportsAsDataSource(request, model);
        }


        public DataSourceResult GetCitizenCharterTimelineList(DataSourceRequest request)
        {
            return _reportRepository.GetCitizenCharterTimelineList(request);
        }


        public DataSourceResult GetSMSLogList(DataSourceRequest request)
        {
            return _reportRepository.GetSMSLogList(request);
        }


        public int SendReportsInExcelFormat(PropertyViewModel model)
        {
            return _reportRepository.SendReportsInExcelFormat(model);
        }


        public OfficeOpenXml.ExcelPackage DownloadReportsInExcelFormat(PropertyViewModel model)
        {
            return _reportRepository.DownloadReportsInExcelFormat(model);
        }


        public string GetServiceReportForAllDepartment()
        {
            return _reportRepository.GetServiceReportForAllDepartment();
        }


        public string GetServiceReportLetter()
        {
            return _reportRepository.GetServiceReportLetter();
        }
    }
}
