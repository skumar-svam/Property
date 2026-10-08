using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Model.NIC;
using System.Collections.Generic;
using System.Web;

namespace NA.PMS.Repository
{
    public interface INICRepository
    {
        ServiceRequestVM SaveServiceRequestDetail(ServiceRequestVM model, IEnumerable<System.Web.HttpPostedFileBase> files);

        ApplicantVM GetApplicantDetailsByRegistrationId(int? registrationId);

        string GetFileUploadHtmlForService(int? departmentId, int? serviceId);

        ApplicantVM ValidateRegistrationId(int? registrationId);

        bool SaveNiveshMitraUnit(WBasicDetailsModel_NMS model);

        ServiceRequestVM GetNiveshMitraServicesByRegistrationId(WBasicDetailsModel_NMS _WBasicDetailsModel_NMS);

        ServiceRequestVM GetNiveshMitraServicesByRequestId(int? RequestId);

        NiveshMitraMasterVM GetNiveshMitraServicesDetails(WBasicDetailsModel_NMS _WBasicDetailsModel_NMS);

        ServiceRequestVM GetServiceRequestDetailById(int? id);

        ServiceRequestVM ReSubmitRequest(ServiceRequestVM model, IEnumerable<HttpPostedFileBase> files);

        DataSourceResult GetServiceRequestUploadedDocumentsById(DataSourceRequest request, ServiceVM model);

        NiveshMitraMasterVM GetNiveshMitraServicesDetailsByApplicationId(int applicationId);

        int UpdateRequestPaymentStatus(ServiceRequestVM model);

        ServiceRequestVM GetNiveshMitraServicesByReqId(int? RequestId);

        ServiceStatusVM GetServiceStatusByCustomerRequestStatusId(int RequestStatusId);

        CitizenServiceRequest GetCitizenServiceDetails(int? ServiceId, int? DeptId);

        DataSourceResult GetNiveshMitraServiceRequests(DataSourceRequest request);

        DataSourceResult GetNiveshMitraServices(DataSourceRequest request);

        DataSourceResult GetNiveshMitraServiceStatus(DataSourceRequest request);

        DataSourceResult GetCitizenServiceDetailsbyServiceId(DataSourceRequest request);

        DataSourceResult GetCustomerServiceRequestList_NIC(DataSourceRequest request);

        int UpdateCustomerServiceRequestStatus(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files);

        int UploadGeneratedLetterByserviceId(ServiceViewModel model, IEnumerable<HttpPostedFileBase> documentfiles);

        List<DropdownViewModel> GetServiceListByDepartmentForNIC(int departmentId);

        ServiceRequestVM SaveNiveshMitraServiceUnit(WBasicDetailsModel_NMS apimodel);
    }
}
