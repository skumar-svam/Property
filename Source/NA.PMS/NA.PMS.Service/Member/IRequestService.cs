using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Model.NIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Service
{
    public interface IRequestService
    {
        DataSourceResult GetServiceRequestReport(DataSourceRequest request, int? departmentId, DateTime? fromDate, DateTime? toDate);
        DataSourceResult GetServicerequestUploadedDocuments(DataSourceRequest request, int? RequestId);
        DataSourceResult GetDirectorShareholderDataList(DataSourceRequest request);

        List<DirectorShareholderModel> SaveDirectorOrShareholders(string directorName, decimal? share, string shareType);
        List<ServiceCheckListModel> GetChecklistOptionsForFileUpload(int? departmentId, int? serviceId);

        ServiceRequestViewModel SaveServiceRequestDetail( ServiceRequestViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files);
        ServiceRequestViewModel SaveServiceRequestForSamadhanDiwas(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files);
        ServiceRequestModel SaveServiceRequestForSamadhanDiwas(ServiceRequestModel model, IEnumerable<System.Web.HttpPostedFileBase> files);
        ServiceRequestViewModel UploadServiceRequestDocuments(ServiceRequestViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files);
        ServiceRequestViewModel GetServiceRequestDetailById(int? id);
        ServiceRequestViewModel GetServiceRequestDetailOfSamadhanDiwasById(int? id);
        
        ServiceRequestViewModel GetServiceRequestDetailForCustomer(int requestId, string mobile);
        
        ApplicantViewModel GetApplicantDetailsByRegistrationId(int? registrationId);
       
        bool UpdateStatus(ServiceRequestViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files);
     
        bool UpdateServiceReq(ServiceRequestModel ObjServiceReq);

        int RemoveDirectorShareholderFromList(int id);

        string GetFileUploadHtmlForService(int? departmentId, int? serviceId);

        DataSourceResult GetCustomerServiceRequestList(DataSourceRequest request, ServiceViewModel model);

        DataSourceResult GetPradhikaranDiwasRequestList(DataSourceRequest request, ServiceViewModel model);

        int UpdateCustomerServiceRequestStatus(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files);

        DataSourceResult GetGeneratedChallanList(DataSourceRequest request, ChallanViewModel model);

        DataSourceResult GetGeneratedChallanAmountListById(DataSourceRequest request, int? challanId);

        DataSourceResult GetGeneratedLetterList(DataSourceRequest request, LetterViewModel model);

        DataSourceResult GetGeneratedLetterListById(DataSourceRequest request, int? rid);

        DataSourceResult GetNotingFileDetailList(DataSourceRequest request, NotingViewModel model);

        DataSourceResult GetNotingFileDetailListById(DataSourceRequest request, NotingViewModel model);

        int SaveNotingFileContent(NotingViewModel model);

        NotingViewModel GetNotingFileDetailById(NotingViewModel model);

        DataSourceResult GetServiceRequestListForJanSuvidhaKendra(DataSourceRequest request);

        DataSourceResult GetPropertyListForJanSuvidhaKendra(DataSourceRequest request, PropertyViewModel model);

        DataSourceResult GetBankAccountDetailList(DataSourceRequest request, BankAccountViewModel model);

        DataSourceResult GetAuthorityServicesDetailList(DataSourceRequest request, ServiceViewModel model);

        DataSourceResult GetPaymentReceiptHeadAsDataSource(DataSourceRequest request, PaymentViewModel model);

        DataSourceResult GetPaymentReceiptSubHeadAsDataSource(DataSourceRequest request, PaymentViewModel model);

        int ActivateBanckAccountStatus(BankAccountViewModel model);

        int ActivatePaymentReceiptStatus(PaymentViewModel model);

        int ActivateAuthorityServiceStatus(ServiceViewModel model);

        int SavePropertyServiceType(ServiceViewModel model);

        int SavePaymentReceiptHeadDetail(PaymentViewModel model);

        DataSourceResult GetKYASubmittedFormList(DataSourceRequest request, KYAViewModel model);

        int ValidateKYAForm(KYAViewModel model);

        KYAViewModel GetKYADetailsById(KYAViewModel model);

        DataSourceResult GetDocumentListByRegistrationId(DataSourceRequest request, int? rid,int? id);

        DataSourceResult GetKYAFormListForValidation(DataSourceRequest request, KYAViewModel model);

        DataSourceResult GetKYADetailStatusList(DataSourceRequest request, KYAViewModel model);

        List<KYAViewModel> GetKYAFormListCount(KYAViewModel model);

        decimal GetAverageApprovedForm();

        PaymentViewModel GetDetailsForNDC(PaymentViewModel model);

        DataSourceResult GetNDCGeneratedListAsDataSource(DataSourceRequest request, NDCVeiwModel model);

        DataSourceResult GetOnlineCustomerServiceRequestAsDataSource(DataSourceRequest request, ServiceViewModel model);

        int UpdateRegistrationIdToServiceRequestById(ServiceViewModel model);

        DataSourceResult GetCustomerServiceRequestByJSKAsDataSource(DataSourceRequest request, ServiceViewModel model);

        DataSourceResult GetServiceRequestUploadedDocumentsById(DataSourceRequest request, ServiceViewModel model);

        int UpdateRemarksForNDCLetter(NDCVeiwModel model);

        string GetNDCLetter(NDCVeiwModel model);

        NDCVeiwModel GetNDCDetailsById(int? Id);

        int UpdateNDCLetterDetails(NDCVeiwModel model);

        DataSourceResult GetCustomerServiceRequestList_NIC(DataSourceRequest request, ServiceVM model);

        int UpdateCustomerServiceRequestStatus_NIC(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files);

        ServiceRequestViewModel GetServiceRequestDetailForCustomer_NIC(int requestId);

        DataSourceResult GetNDCListForApprovalAsDataSource(DataSourceRequest request, NDCVeiwModel model);

        int ForwardServiceRequestInBulkFormat(ServiceViewModel model);

        int UploadGeneratedLetterByserviceId(ServiceViewModel model, IEnumerable<HttpPostedFileBase> documentfiles);

        string GetUploadedDocumentByServiceId(int Id, int Rid, string ActionType);

        DataSourceResult GetCustomerServiceRequestListByRid(DataSourceRequest request, int? rId);

        int SaveExportedDocument(string contentType, string base64, string fileName);

        int SaveDocumentTypesInSession(DocumentViewModel model);

        DataSourceResult GetSavedDocumentTypeListByIdAsDataSource(DataSourceRequest request, DocumentViewModel model);

        DataSourceResult GetDigitalSingedLetterHistory(DataSourceRequest request, int? rid);
    }
}
