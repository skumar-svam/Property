using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Model.NIC;
using NA.PMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Service
{
    public class RequestService : IRequestService
    {
        IServiceRepository _serviceRepository;
        public RequestService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public ServiceRequestViewModel SaveServiceRequestDetail(ServiceRequestViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            return _serviceRepository.SaveServiceRequestDetail(model, files);
        }


        public ServiceRequestViewModel GetServiceRequestDetailById(int? id)
        {
            return _serviceRepository.GetServiceRequestDetailById(id);
        }

        public DataSourceResult GetServiceRequestReport(DataSourceRequest request, int? departmentId, DateTime? fromDate, DateTime? toDate)
        {
            return _serviceRepository.GetServiceRequestReport(request, departmentId, fromDate, toDate);
        }

        public DataSourceResult GetServicerequestUploadedDocuments(DataSourceRequest request, int? RequestId)
        {
            return _serviceRepository.GetServicerequestUploadedDocuments(request, RequestId);
        }

        public ServiceRequestViewModel UploadServiceRequestDocuments(ServiceRequestViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            return _serviceRepository.UploadServiceRequestDocuments(model, files);
        }


        public ApplicantViewModel GetApplicantDetailsByRegistrationId(int? registrationId)
        {
            return _serviceRepository.GetApplicantDetailsByRegistrationId(registrationId);
        }

        public List<DirectorShareholderModel> SaveDirectorOrShareholders(string directorName, decimal? share, string shareType)
        {
            return _serviceRepository.SaveDirectorOrShareholders(directorName, share, shareType);
        }

        public List<ServiceCheckListModel> GetChecklistOptionsForFileUpload(int? departmentId, int? serviceId)
        {
            return _serviceRepository.GetChecklistOptionsForFileUpload(departmentId, serviceId);
        }

        public bool UpdateStatus(ServiceRequestViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            return _serviceRepository.UpdateStatus(model, files);
        }


        public ServiceRequestModel SaveServiceRequestForSamadhanDiwas(ServiceRequestModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            return _serviceRepository.SaveServiceRequestForSamadhanDiwas(model, files);
        }


        public ServiceRequestViewModel GetServiceRequestDetailForCustomer(int requestId, string mobile)
        {
            return _serviceRepository.GetServiceRequestDetailForCustomer(requestId, mobile);
        }

        public bool UpdateServiceReq(ServiceRequestModel ObjServiceReq)
        {
            return _serviceRepository.UpdateServiceReq(ObjServiceReq);
        }


        public DataSourceResult GetDirectorShareholderDataList(DataSourceRequest request)
        {
            return _serviceRepository.GetDirectorShareholderDataList(request);
        }


        public int RemoveDirectorShareholderFromList(int id)
        {
            return _serviceRepository.RemoveDirectorShareholderFromList(id);
        }


        public ServiceRequestViewModel SaveServiceRequestForSamadhanDiwas(ServiceRequestViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            return _serviceRepository.SaveServiceRequestForSamadhanDiwas(model, files);
        }


        public ServiceRequestViewModel GetServiceRequestDetailOfSamadhanDiwasById(int? id)
        {
            return _serviceRepository.GetServiceRequestDetailOfSamadhanDiwasById(id);
        }


        public string GetFileUploadHtmlForService(int? departmentId, int? serviceId)
        {
            return _serviceRepository.GetFileUploadHtmlForService(departmentId, serviceId);
        }


        public DataSourceResult GetCustomerServiceRequestList(DataSourceRequest request, ServiceViewModel model)
        {
            return _serviceRepository.GetCustomerServiceRequestList(request, model);
        }


        public DataSourceResult GetPradhikaranDiwasRequestList(DataSourceRequest request, ServiceViewModel model)
        {
            return _serviceRepository.GetPradhikaranDiwasRequestList(request, model);
        }


        public int UpdateCustomerServiceRequestStatus(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            return _serviceRepository.UpdateCustomerServiceRequestStatus(model, files);
        }


        public DataSourceResult GetGeneratedChallanList(DataSourceRequest request, ChallanViewModel model)
        {
            return _serviceRepository.GetGeneratedChallanList(request, model);
        }

        public DataSourceResult GetGeneratedChallanAmountListById(DataSourceRequest request, int? challanId)
        {
            return _serviceRepository.GetGeneratedChallanAmountListById(request, challanId);
        }


        public DataSourceResult GetGeneratedLetterList(DataSourceRequest request, LetterViewModel model)
        {
            return _serviceRepository.GetGeneratedLetterList(request, model);
        }


        public DataSourceResult GetGeneratedLetterListById(DataSourceRequest request, int? rid)
        {
            return _serviceRepository.GetGeneratedLetterListById(request, rid);
        }


        public DataSourceResult GetNotingFileDetailList(DataSourceRequest request, NotingViewModel model)
        {
            return _serviceRepository.GetNotingFileDetailList(request, model);
        }

        public DataSourceResult GetNotingFileDetailListById(DataSourceRequest request, NotingViewModel model)
        {
            return _serviceRepository.GetNotingFileDetailListById(request, model);
        }


        public int SaveNotingFileContent(NotingViewModel model)
        {
            return _serviceRepository.SaveNotingFileContent(model);
        }


        public NotingViewModel GetNotingFileDetailById(NotingViewModel model)
        {
            return _serviceRepository.GetNotingFileDetailById(model);
        }


        public DataSourceResult GetServiceRequestListForJanSuvidhaKendra(DataSourceRequest request)
        {
            return _serviceRepository.GetServiceRequestListForJanSuvidhaKendra(request);
        }


        public DataSourceResult GetPropertyListForJanSuvidhaKendra(DataSourceRequest request, PropertyViewModel model)
        {
            return _serviceRepository.GetPropertyListForJanSuvidhaKendra(request, model);
        }


        public DataSourceResult GetBankAccountDetailList(DataSourceRequest request, BankAccountViewModel model)
        {
            return _serviceRepository.GetBankAccountDetailList(request, model);
        }

        public DataSourceResult GetAuthorityServicesDetailList(DataSourceRequest request, ServiceViewModel model)
        {
            return _serviceRepository.GetAuthorityServicesDetailList(request, model);
        }


        public DataSourceResult GetPaymentReceiptHeadAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            return _serviceRepository.GetPaymentReceiptHeadAsDataSource(request, model);
        }

        public DataSourceResult GetPaymentReceiptSubHeadAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            return _serviceRepository.GetPaymentReceiptSubHeadAsDataSource(request, model);
        }


        public int ActivateBanckAccountStatus(BankAccountViewModel model)
        {
            return _serviceRepository.ActivateBanckAccountStatus(model);
        }

        public int ActivatePaymentReceiptStatus(PaymentViewModel model)
        {
            return _serviceRepository.ActivatePaymentReceiptStatus(model);
        }

        public int ActivateAuthorityServiceStatus(ServiceViewModel model)
        {
            return _serviceRepository.ActivateAuthorityServiceStatus(model);
        }


        public int SavePropertyServiceType(ServiceViewModel model)
        {
            return _serviceRepository.SavePropertyServiceType(model);
        }


        public int SavePaymentReceiptHeadDetail(PaymentViewModel model)
        {
            return _serviceRepository.SavePaymentReceiptHeadDetail(model);
        }


        public DataSourceResult GetKYASubmittedFormList(DataSourceRequest request, KYAViewModel model)
        {
            return _serviceRepository.GetKYASubmittedFormList(request, model);
        }


        public int ValidateKYAForm(KYAViewModel model)
        {
            return _serviceRepository.ValidateKYAForm(model);
        }


        public KYAViewModel GetKYADetailsById(KYAViewModel model)
        {
            return _serviceRepository.GetKYADetailsById(model);
        }


        public DataSourceResult GetDocumentListByRegistrationId(DataSourceRequest request, int? rid, int? id)
        {
            return _serviceRepository.GetDocumentListByRegistrationId(request, rid, id);
        }


        public DataSourceResult GetKYAFormListForValidation(DataSourceRequest request, KYAViewModel model)
        {
            return _serviceRepository.GetKYAFormListForValidation(request, model);
        }


        public DataSourceResult GetKYADetailStatusList(DataSourceRequest request, KYAViewModel model)
        {
            return _serviceRepository.GetKYADetailStatusList(request, model);
        }


        public List<KYAViewModel> GetKYAFormListCount(KYAViewModel model)
        {
            return _serviceRepository.GetKYAFormListCount(model);
        }


        public decimal GetAverageApprovedForm()
        {
            return _serviceRepository.GetAverageApprovedForm();
        }


        public PaymentViewModel GetDetailsForNDC(PaymentViewModel model)
        {
            return _serviceRepository.GetDetailsForNDC(model);
        }


        public DataSourceResult GetNDCGeneratedListAsDataSource(DataSourceRequest request, NDCVeiwModel model)
        {
            return _serviceRepository.GetNDCGeneratedListAsDataSource(request, model);
        }


        public DataSourceResult GetOnlineCustomerServiceRequestAsDataSource(DataSourceRequest request, ServiceViewModel model)
        {
            return _serviceRepository.GetOnlineCustomerServiceRequestAsDataSource(request, model);
        }


        public int UpdateRegistrationIdToServiceRequestById(ServiceViewModel model)
        {
            return _serviceRepository.UpdateRegistrationIdToServiceRequestById(model);
        }


        public DataSourceResult GetCustomerServiceRequestByJSKAsDataSource(DataSourceRequest request, ServiceViewModel model)
        {
            return _serviceRepository.GetCustomerServiceRequestByJSKAsDataSource(request, model);
        }


        public DataSourceResult GetServiceRequestUploadedDocumentsById(DataSourceRequest request, ServiceViewModel model)
        {
            return _serviceRepository.GetServiceRequestUploadedDocumentsById(request, model);
        }


        public int UpdateRemarksForNDCLetter(NDCVeiwModel model)
        {
            return _serviceRepository.UpdateRemarksForNDCLetter(model);
        }


        public string GetNDCLetter(NDCVeiwModel model)
        {
            return _serviceRepository.GetNDCLetter(model);
        }


        public NDCVeiwModel GetNDCDetailsById(int? Id)
        {
            return _serviceRepository.GetNDCDetailsById(Id);
        }


        public int UpdateNDCLetterDetails(NDCVeiwModel model)
        {
            return _serviceRepository.UpdateNDCLetterDetails(model);
        }

        public DataSourceResult GetCustomerServiceRequestList_NIC(DataSourceRequest request, ServiceVM model)
        {
            return _serviceRepository.GetCustomerServiceRequestList_NIC(request, model);
        }

        public int UpdateCustomerServiceRequestStatus_NIC(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            return _serviceRepository.UpdateCustomerServiceRequestStatus_NIC(model, files);
        }

        public ServiceRequestViewModel GetServiceRequestDetailForCustomer_NIC(int requestId)
        {
            return _serviceRepository.GetServiceRequestDetailForCustomer_NIC(requestId);
        }


        public DataSourceResult GetNDCListForApprovalAsDataSource(DataSourceRequest request, NDCVeiwModel model)
        {
            return _serviceRepository.GetNDCListForApprovalAsDataSource(request, model);
        }


        public int ForwardServiceRequestInBulkFormat(ServiceViewModel model)
        {
            return _serviceRepository.ForwardServiceRequestInBulkFormat(model);
        }


        public int UploadGeneratedLetterByserviceId(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> documentfiles)
        {
            return _serviceRepository.UploadGeneratedLetterByserviceId(model, documentfiles);
        }


        public string GetUploadedDocumentByServiceId(int Id, int Rid, string ActionType)
        {
            return _serviceRepository.GetUploadedDocumentByServiceId(Id,Rid,ActionType);
        }


        public DataSourceResult GetCustomerServiceRequestListByRid(DataSourceRequest request, int? rId)
        {
            return _serviceRepository.GetCustomerServiceRequestListByRid(request, rId);
        }


        public int SaveExportedDocument(string contentType, string base64, string fileName)
        {
            return _serviceRepository.SaveExportedDocument(contentType, base64, fileName);
        }


        public int SaveDocumentTypesInSession(DocumentViewModel model)
        {
            return _serviceRepository.SaveDocumentTypesInSession(model);
        }

        public DataSourceResult GetSavedDocumentTypeListByIdAsDataSource(DataSourceRequest request, DocumentViewModel model)
        {
            return _serviceRepository.GetSavedDocumentTypeListByIdAsDataSource(request, model);
        }


        public DataSourceResult GetDigitalSingedLetterHistory(DataSourceRequest request, int? rid)
        {
            return _serviceRepository.GetDigitalSingedLetterHistory(request, rid);
        }
    }
}
