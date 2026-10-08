
using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Model.NIC;
using NA.PMS.Repository;
using System.Collections.Generic;
using System.Web;

namespace NA.PMS.Service
{
    public class NICService : INICService
    {
        INICRepository _NICRepository = null;

        public NICService()
        {
            _NICRepository = new NICRepository();
        }

        public ApplicantVM GetApplicantDetailsByRegistrationId(int? registrationId)
        {
            return _NICRepository.GetApplicantDetailsByRegistrationId(registrationId);
        }

        public string GetFileUploadHtmlForService(int? departmentId, int? serviceId)
        {
            return _NICRepository.GetFileUploadHtmlForService(departmentId, serviceId);
        }

        public ServiceRequestVM SaveServiceRequestDetail(ServiceRequestVM model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            return _NICRepository.SaveServiceRequestDetail(model, files);
        }


        public ApplicantVM ValidateRegistrationId(int? registrationId)
        {
            return _NICRepository.ValidateRegistrationId(registrationId);
        }

        public bool SaveNiveshMitraUnit(WBasicDetailsModel_NMS model)
        {
            return _NICRepository.SaveNiveshMitraUnit(model);
        }

        public ServiceRequestVM GetNiveshMitraServicesByRegistrationId(WBasicDetailsModel_NMS _WBasicDetailsModel_NMS)
        {
            return _NICRepository.GetNiveshMitraServicesByRegistrationId(_WBasicDetailsModel_NMS);
        }

        public ServiceRequestVM GetNiveshMitraServicesByRequestId(int? RequestId)
        {
            return _NICRepository.GetNiveshMitraServicesByRequestId(RequestId);
        }

        public NiveshMitraMasterVM GetNiveshMitraServicesDetails(WBasicDetailsModel_NMS _WBasicDetailsModel_NMS)
        {
            return _NICRepository.GetNiveshMitraServicesDetails(_WBasicDetailsModel_NMS);
        }

        public ServiceRequestVM GetServiceRequestDetailById(int? id)
        {
            return _NICRepository.GetServiceRequestDetailById(id);
        }

        public ServiceRequestVM ReSubmitRequest(ServiceRequestVM model, IEnumerable<HttpPostedFileBase> files)
        {
            return _NICRepository.ReSubmitRequest(model, files);
        }

        public DataSourceResult GetServiceRequestUploadedDocumentsById(DataSourceRequest request, ServiceVM model)
        {
            return _NICRepository.GetServiceRequestUploadedDocumentsById(request, model);
        }

        public NiveshMitraMasterVM GetNiveshMitraServicesDetailsByApplicationId(int applicationId)
        {
            return _NICRepository.GetNiveshMitraServicesDetailsByApplicationId(applicationId);
        }

        public int UpdateRequestPaymentStatus(ServiceRequestVM model)
        {
            return _NICRepository.UpdateRequestPaymentStatus(model);
        }

        public ServiceRequestVM GetNiveshMitraServicesByReqId(int? RequestId)
        {
            return _NICRepository.GetNiveshMitraServicesByReqId(RequestId);
        }

        public ServiceStatusVM GetServiceStatusByCustomerRequestStatusId(int RequestStatusId)
        {
            return _NICRepository.GetServiceStatusByCustomerRequestStatusId(RequestStatusId);
        }

        public CitizenServiceRequest GetCitizenServiceDetails(int? ServiceId, int? DeptId)
        {
            return _NICRepository.GetCitizenServiceDetails(ServiceId, DeptId);
        }

        public DataSourceResult GetNiveshMitraServiceRequests(DataSourceRequest request)
        {
            return _NICRepository.GetNiveshMitraServiceRequests(request);
        }

        public DataSourceResult GetNiveshMitraServices(DataSourceRequest request)
        {
            return _NICRepository.GetNiveshMitraServices(request);
        }

        public DataSourceResult GetNiveshMitraServiceStatus(DataSourceRequest request)
        {
            return _NICRepository.GetNiveshMitraServiceStatus(request);
        }

        public DataSourceResult GetCitizenServiceDetailsbyServiceId(DataSourceRequest request)
        {
            return _NICRepository.GetCitizenServiceDetailsbyServiceId(request);
        }

        public DataSourceResult GetCustomerServiceRequestList_NIC(DataSourceRequest request)
        {
            return _NICRepository.GetCustomerServiceRequestList_NIC(request);
        }

        public int UpdateCustomerServiceRequestStatus(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            return _NICRepository.UpdateCustomerServiceRequestStatus(model, files);
        }

        public int UploadGeneratedLetterByserviceId(ServiceViewModel model, IEnumerable<HttpPostedFileBase> documentfiles)
        {
            return _NICRepository.UploadGeneratedLetterByserviceId(model, documentfiles);
        }

        public List<DropdownViewModel> GetServiceListByDepartmentForNIC(int departmentId)
        {
            return _NICRepository.GetServiceListByDepartmentForNIC(departmentId);
        }


        public ServiceRequestVM SaveNiveshMitraServiceUnit(WBasicDetailsModel_NMS apimodel)
        {
            return _NICRepository.SaveNiveshMitraServiceUnit(apimodel);
        }
    }
}
