using Kendo.Mvc.UI;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NA.PMS.Service
{
    public interface IOnlineService
    {
        DataSourceResult GetOnlineApplications(DataSourceRequest request, OnlineFormViewModel modal);
        DataSourceResult GetOnlineApplicationsForAdmin(DataSourceRequest request, OnlineFormViewModel modal);
        DataSourceResult GetUploadedDocumentsForOnlineForm(DataSourceRequest request, int? formId);
        DataSourceResult GetUploadedDocumentsForReturnForm(DataSourceRequest request, int? formId);
        DataSourceResult GetChecklistDocumentsForOnlineApplication(DataSourceRequest request, int? schemeId);
        DataSourceResult GetApplicationFormIdForOfflinePayment(DataSourceRequest Req, int? ApplicationFormId);
        DataSourceResult GetUploadedDocumentsAfterScrutiny(DataSourceRequest request, int? formId, int? checklistIdstart, int? checklistIdend);
        DataSourceResult GetDirectorDetailsForOpenScheme(DataSourceRequest request, int? id);
        DataSourceResult GetDirectorDetails(DataSourceRequest request, int? formId);
        DataSourceResult GetApplicantListAfterDraw(DataSourceRequest request, OnlineFormViewModel modal);
        DataSourceResult GetApplicantDetailForDraw(DataSourceRequest request, OnlineFormViewModel modal);
        DataSourceResult GetOnlineApplicationFormIdList(DataSourceRequest request, int? schemeId, int? departmentId);

        OnlineApplicationDetailsTrans GetOnlineApplicationReceipt(OnlineApplicationDetailsTrans ObjOnlineApplicationDetailsTrans);
        OnlineFormViewModel GetApplicationFeeAndCharges(int? schemeId, int? departmentId, int? propertyTypeId, int? areaTypeId);
        OnlineFormViewModel GetOnlineApplicationFeeAndCharges(OnlineFormViewModel model);
        OnlineFormViewModel GetOnlineApplicationFormById(int? id);
        OnlinePaymentViewModel SaveOnlinePaymentTransaction(OnlineFormViewModel form);
        OnlinePaymentViewModel GetOnlinePaymentTransactionDetailById(string transactionId, string paymentType);
        OnlinePaymentViewModel UpdateOnlinePaymentTransaction(FormCollection form);
        ChallanModel GeneratePaymentChallan(OnlineChallanViewModel objOnlineChallanViewModel);
        OnlineChallanViewModel GenerateSchemeChallan(OnlineFormViewModel objOnlineFormView);
        OnlineFormViewModel GetInitialDataForScheme(OnlineFormViewModel objOnlineFormViewModel);
        OnlineFormViewModel GetInitialDataForScheme();
        OnlinePaymentViewModel GetOfflinePayment_Trans(int ApplicationFormId);
        OnlineFormViewModel GetOnlineSchemeFormById(int? id);

        int SaveOnlineApplicationForm(OnlineFormViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage);
        int UpdateOnlineApplicationForm(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage);
        int ValidatePANnumber(string pan, int? areaId, int? schemeId);
        int ValidateFormNumber(string formNo);
        int UploadDocumentByFormId(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage);
        int SaveDirectorDetails(int? formId, string directorName, decimal? share, int? directorTypeId, string pan);
        int SendMessageInBulk(string type);
        int ValidateApplicationDetails(OnlineFormViewModel ObjOnlineFormViewModel);
        int SaveDirectorDetailsForOpenScheme(onlineDirectorViewModel model);
        int RemoveDirectorDetailsForOpenScheme(int? id);
        int ValidateApplicationDetailsforForgotPassword(OnlineFormViewModel ObjOnlineFormViewModel);
        int UpdateCompanyDetail(OnlineFormViewModel model);
        int RemoveDocumentFromApplicationForm(string formNo, string filename);
        int SaveCreateChallan(int? rId, int AccountHeadId, int AccountSubHeadId, decimal? Amount);
        int ValidateOnlineFormPayment(string formNo);
        int RemoveDirectorDetails(int? formId, int? directorId);
        int AllotPropertyAfterDraw(OnlineFormViewModel model);

        string GetListOfChecklistDocuments(int? schemeId);
        string GetAccountNumber(int bankId, int branchId);
        string UpdateOfflinePayment(OnlineFormViewModel objOnlineFormViewModel, HttpPostedFileBase files);
        string SaveOpenSchemeFormDetail(OnlineFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage);

        List<DropdownViewModel> GetAreaRangeByDepartment(int? schemeId, int? departmentId);
        List<int> SendOTPforPayment(string formId);
        List<OnlineChallanViewModel> GetGeneratedChallanDetails(int rid);

        Boolean RejectApplication(int Id);
        Boolean UpdateChallanStatus(int Id, int PaymentType);

        bool SaveGeneratedChallan(int? rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId, string parsedHTML);
        bool SaveGeneratedChallan(string challanId, string challan);
        bool RemoveChallanChargeDetail(int? rid, string headName, string subHeadName, decimal amount);
        bool ChangePassword(int FormId, string email, string newPassword);
        bool IsDocumentUploaded(int? formId);
        int SaveUploadPreviousChallan(OnlineFormViewModel model, HttpPostedFileBase docs);

        List<DropdownViewModel> GetSchemeList();

        List<DropdownViewModel> GetPaymentStatusList();
        NICsingalwindowSystem GetNICSingleWindowData(OnlineFormViewModel objOnlineApplicationDetails);
        int SaveNICSingleWindowPayment(NewDataSet ObjWBasicDetailsGetModel);
        int UpdatePaymentTransaction_SingleWindowPortal(OnlineFormViewModel Objmodel);
        int SaveApplicationDetailForAllotment(int FormId);

        LetterViewModel GetLetterByBarcode(string barcode);

        int UpdateAllottedPropertyAfterDraw(int? formId, string actionType);

        int ValidatePropertyForAllotment(string sector, string block, string plot);

        OnlinePaymentViewModel GetPreviousChallanPayment_Trans(int ApplicationFormId);

        ResultMessage SaveApplicationProcessRequest(OnlineFormViewModel objOnlineFormViewModel);

        DataSourceResult GetOnlineApplicationProcessRequests(DataSourceRequest request);

        DataSourceResult GetOnlineApplicationProcess(DataSourceRequest request);

        ResultMessage UpdateOnlineApplicationStatus(OnlineApplicationDetailProcess objOnlineApplicationDetailProcess);

        DataSourceResult GetPropertyDetailAsDataSourceByPropertyId(DataSourceRequest request, int? propertyId);

        DataSourceResult GetApplicationDetailAsDataSourceByApplicationId(DataSourceRequest request, int? applicationId);

        int ValidatePropertyAndApplicationForm(OnlineFormViewModel model);

        DataSourceResult GetAllottedOnlineFormPropertyList(DataSourceRequest request, OnlineFormViewModel model);

        DataSourceResult GetOnlineApplicationsForConsultant(DataSourceRequest request, OnlineFormViewModel modal);

        DataSourceResult GetFloorAreaListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        OnlineFormViewModel GetIndustrialSchemeInformation();

        DataSourceResult GetDocumentListForOnlineSchemeForm(DataSourceRequest request, OnlineFormViewModel model);

        OnlineFormViewModel GetSchemeInformationForOnlineApplication(OnlineFormViewModel model);

        DataSourceResult GetOnlineChecklistDocument(DataSourceRequest request, OnlineDocumentViewModel model);

        DataSourceResult GetChecklistTypeList(DataSourceRequest request, OnlineDocumentViewModel model);

        int ValidatePANForOnlineScheme(OnlineFormViewModel model);

        OnlinePaymentViewModel GetGeneratedChallanDetailById(OnlinePaymentViewModel onlinePaymentViewModel);

        void SaveChallanOnlinePaymentTransaction(OnlinePaymentViewModel model);

        OnlinePaymentViewModel UpdateChallanOnlinePaymentTransaction(FormCollection form);

        DataSourceResult GetOnlinePaymentDetailsAsDataSource(DataSourceRequest request, OnlinePaymentViewModel model);

        OnlinePaymentViewModel GetOnlinePaymentDetailById(OnlinePaymentViewModel model);

        DataSourceResult GetOnlinePaidChallanIdListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        DataSourceResult GetOnlineTransactionIdListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        int UpdateOnlinePaymentDetailById(OnlinePaymentViewModel model);

        OnlineFormViewModel SaveOpenEndedSchemeFormDetail(OnlineFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage);

        OnlineFormViewModel GetOpenEndedSchemeFormDataById(OnlineFormViewModel form);

        OnlineFormViewModel UpdateOESFormPaymentStatus(OnlineFormViewModel model);

        OnlineFormViewModel SaveProjectAndRefundDetailForOpenEndScheme(OnlineFormViewModel model);

        onlineDirectorViewModel SaveDirectorDetailToDataBaseForOpenScheme(onlineDirectorViewModel model);

        DataSourceResult GetDirectorDetailsFromDatabaseForOpenScheme(DataSourceRequest request, int? id);

       
    }
}
