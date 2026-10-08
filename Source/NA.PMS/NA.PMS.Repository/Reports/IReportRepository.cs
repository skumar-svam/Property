using Kendo.Mvc.UI;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Repository
{
    public interface IReportRepository
    {
        DataSourceResult GetPropertiesForReport(DataSourceRequest request, int? deptId, string propBank, string schemeId);
        List<DDList> GetPropertyBank();
        // Get All Mortgage Reports
        DataSourceResult GetMortgageReports(DataSourceRequest request, DateTime? fromDate, DateTime? toDate, int? schemeId, int? departmentId);
        DataSourceResult GetGPAReportData(DataSourceRequest req, DateTime? fromSearch, DateTime? toSearch, int? depttId);
        DataSourceResult GetNomineeReportData(DataSourceRequest req, DateTime? fromSearch, DateTime? toSearch, int? depttId);
        DataSourceResult GetPropertyFuctinalReport(DataSourceRequest request, string status);
        DataSourceResult GetTransferReportData(DataSourceRequest req, DateTime? fromSearch, DateTime? toSearch, int? schemeId, int? depttId, int? transType, int? transSubType);
        DataSourceResult GetSearchedFunctionalReport(DataSourceRequest request, string functionalDetail, string department, string scheme, string sector, DateTime? startDate, DateTime? endDate);

        DataSourceResult SearchPossessionReport(DataSourceRequest request, string department, string scheme, string possession, string areaChange, string sector, DateTime? startDate, DateTime? endDate);

        DataSourceResult GetPossessionReport(DataSourceRequest request);

        DataSourceResult SearchCompletionReportData(DataSourceRequest request, string completion, string department, string scheme, string sector, DateTime? startDate, DateTime? endDate);

        DataSourceResult GetCustomerServiceReport(DataSourceRequest request, int? departmentId, DateTime? startDate, DateTime? endDate, int? status);
        string GetDashboardGraph(int? ReqType, int? UserDept);
        string GetServiceRequestMatrix(int? departmentid, int? serviceId, DateTime? FromDate, DateTime? ToDate);
        DataSourceResult GetUserWiseRequest(DataSourceRequest Req, UserWiseRequest objUserWiseRequest);
        DataSourceResult GetPendencyReport(DataSourceRequest request);

        CommonViewModel GetApplicantPremiumDuesByRegistrationId(string registrationId);

        CommonViewModel GetLeaseRentDateByRegistrationId(string registrationId);

        int SaveDetailForNDC(NDCVeiwModel model);

        DataSourceResult GetNoDuesCertificateList(DataSourceRequest request);

        int UpdateRegistrationIdByRequestNo(int requestNo, string registrationId);

        PropertyDetailViewModel GetAllotteDetailsByRegistrationId(string registrationId, string flag1, string flag2);

        int UpdateAllotteeBasicInfo(PropertyDetailViewModel model);

        int UpdateTransferDetailById(PropertyDetailViewModel model);

        PropertyDetailViewModel GetTransferDetailsByRegistrationId(string registrationId, string flag);

        PropertyDetailViewModel GetMultipleDetailsByRegistrationId(string registrationId, string flag);

        int UpdateDateFieldsByRegistrationId(string registrationId, DateTime? firstDate, DateTime? secondDate, string flag);

        SchemePropertyModel GetPropertyDetailsById(int? propertyId);

        int UpdatePropertyDetail(PropertyDetailViewModel model);

        string GetRegistrationIdByRequestNo(int? requestNo);
        DataSourceResult GetVacantProperties(DataSourceRequest Req, int? DepartmentId);

        MortgageViewModel GetMortgageDetailsByRid(int? registrationId);

        int UpdateMortgageDetail(MortgageViewModel model);

        int UpdateServiceRequestDetail(ServiceRequestModel model);

        ServiceRequestModel GetServiceRequestDetailById(int? id);

        DataSourceResult GetVacantPropertyReport(DataSourceRequest request, PropertyViewModel model);

        PropertyViewModel GetRegistrationIdByApplicationIdOrFormNo(PropertyViewModel model);

        int UpdateRegistrationIdByApplicationIdOrFormNo(PropertyViewModel model);

        List<ServiceReportDepartmentWiseVM> GetServiceReportDepartmentWise(ServiceReportDepartmentWiseVM serviceReport);

        DataSourceResult GetKYAReportsAsDataSource(DataSourceRequest request, KYAViewModel model);

        DataSourceResult GetChallanReportsAsDataSource(DataSourceRequest request, ChallanViewModel model);

        DataSourceResult GetServiceReportsAsDataSource(DataSourceRequest request, ServiceViewModel model);

        IEnumerable<KYAViewModel> GetKYAReportsForGraph(KYAViewModel model);

        IEnumerable<ChallanViewModel> GetChallanReportsForGraph(ChallanViewModel model);

        IEnumerable<ServiceViewModel> GetServiceReportsForGraph(ServiceViewModel model);

        DataSourceResult GetDemandAndNDCListByDepartmentAsDataSource(DataSourceRequest request, NDCVeiwModel model);

        IEnumerable<NDCVeiwModel> GetNDCAndDemandReportsForGraph(NDCVeiwModel model);

        IEnumerable<ServiceViewModel> GetServiceReportsForGraphII(ServiceViewModel model);

        DataSourceResult GetCustomerServiceRequestDataAsDataSource(DataSourceRequest request, ServiceViewModel model);

        ServiceViewModel GetMultipleTypeIdToRedirect(ServiceViewModel model);

        DataSourceResult GetKYAReportDataAsDataSource(DataSourceRequest request, KYAViewModel model);

        DataSourceResult GetChallanReportDataAsDataSource(DataSourceRequest request, ChallanViewModel model);

        DataSourceResult GetDemandNotesReportDataAsDataSource(DataSourceRequest request, PaymentViewModel model);

        DataSourceResult GetNDCReportDataAsDataSource(DataSourceRequest request, NDCVeiwModel model);

        DataSourceResult GetServiceTimelineReportsAsDataSource(DataSourceRequest request, ServiceViewModel model);

        DataSourceResult GetCitizenCharterTimelineList(DataSourceRequest request);

        DataSourceResult GetSMSLogList(DataSourceRequest request);

        int SendReportsInExcelFormat(PropertyViewModel model);

        OfficeOpenXml.ExcelPackage DownloadReportsInExcelFormat(PropertyViewModel model);

        string GetServiceReportForAllDepartment();

        string GetServiceReportLetter();
    }
}
