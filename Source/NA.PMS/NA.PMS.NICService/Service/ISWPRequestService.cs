using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.NICServices
{
    public interface ISWPRequestService
    {
        SWPServicesViewModel SaveServiceRequestDetail(SWPServicesViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files);

        SWPApplicantViewModel GetApplicantDetailsByRegistrationId(int? registrationId);

        string GetFileUploadHtmlForService(int? departmentId, int? serviceId);

        SWPApplicantViewModel ValidateRegistrationId(int? registrationId);

        bool SaveNiveshMitraUnit(SWPPostViewModel model);

        SWPServicesViewModel GetNiveshMitraServicesByRegistrationId(SWPPostViewModel _WBasicDetailsModel_NMS);

        SWPServicesViewModel GetNiveshMitraServicesByRequestId(int? RequestId);

        SWPApiServiceViewModel GetNiveshMitraServicesDetails(SWPPostViewModel _WBasicDetailsModel_NMS);

        SWPApiServiceViewModel ValidateNiveshMitraServicePostData(SWPPostViewModel postmodel);

        SWPServicesViewModel GetServiceRequestDetailById(int? id);

        SWPServicesViewModel ReSubmitRequest(SWPServicesViewModel model, IEnumerable<HttpPostedFileBase> files);

        DataSourceResult GetServiceRequestUploadedDocumentsById(DataSourceRequest request, SWPServiceViewModel model);

        SWPApiServiceViewModel GetNiveshMitraServicesDetailsByApplicationId(int applicationId);

        int UpdateRequestPaymentStatus(SWPServicesViewModel model);

        SWPServicesViewModel GetNiveshMitraServicesByReqId(int? RequestId);

        SWPApiServiceViewModel GetServiceStatusByCustomerRequestStatusId(int RequestStatusId);

        SWPClientRequestViewModel GetCitizenServiceDetails(int? ServiceId, int? DeptId);

        DataSourceResult GetNiveshMitraServiceRequests(DataSourceRequest request);

        DataSourceResult GetNiveshMitraServices(DataSourceRequest request, SWPApiServiceViewModel apimodel);

        DataSourceResult GetNiveshMitraServiceStatus(DataSourceRequest request, SWPApiServiceViewModel apimodel);

        DataSourceResult GetCitizenServiceDetailsbyServiceId(DataSourceRequest request);

        DataSourceResult GetCustomerServiceRequestList_NIC(DataSourceRequest request);

        int UpdateCustomerServiceRequestStatus(SWPServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files);

        int UploadGeneratedLetterByserviceId(SWPServiceViewModel model, IEnumerable<HttpPostedFileBase> documentfiles);

        List<SWPDropdownViewModel> GetServiceListByDepartmentForNIC(int departmentId);

        SWPServicesViewModel SaveNiveshMitraServiceUnit(SWPPostViewModel apimodel);

        SWPApiServiceViewModel SaveSWPServices(SWPApiServiceViewModel apimodel);

        SWPApiServiceViewModel ActivateSWPServiceStatus(SWPApiServiceViewModel apimodel);

        DataSourceResult GetServiceStatusReasonsAsDataSource(DataSourceRequest request, SWPApiServiceViewModel model);

        SWPApiServiceViewModel ValidateNiveshMitraServiceUnit(SWPApiServiceViewModel model);

        SWPApiViewModel GetSWPServiceDetailById(SWPApiViewModel model);

        SWPApiViewModel SaveNiveshMitraStatusDetail(SWPApiViewModel model);

        SWPApiViewModel SaveReasonDetailForRejection(SWPApiViewModel model);

        DataSourceResult GetSWPServiceRequestIdForDropDownAsDataSource(DataSourceRequest request, SWPDropdownViewModel model);

        SWPStatusViewModel GetSWPServiceRequestDetailById(SWPStatusViewModel swpmodel);

        DataSourceResult GetSWPRequestServiceListAsDataSource(DataSourceRequest request);
    }
}
