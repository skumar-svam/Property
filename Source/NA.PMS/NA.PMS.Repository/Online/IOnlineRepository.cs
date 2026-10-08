using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Repository
{
    public interface IOnlineRepository
    {
        DataSourceResult GetOnlineApplications(DataSourceRequest request, OnlineFormViewModel modal);
        DataSourceResult GetOnlineApplicationsForAdmin(DataSourceRequest request, OnlineFormViewModel modal);
        OnlineApplicationDetailsTrans GetOnlineApplicationReceipt(OnlineApplicationDetailsTrans ObjOnlineApplicationDetailsTrans);
        DataSourceResult GetChecklistDocumentsForOnlineApplication(DataSourceRequest request, int? schemeId);

        Model.OnlineFormViewModel GetApplicationFeeAndCharges(int? schemeId, int? departmentId, int? propertyTypeId, int? areaTypeId);

        OnlineFormViewModel GetOnlineApplicationFeeAndCharges(OnlineFormViewModel model);

        int SaveOnlineApplicationForm(Model.OnlineFormViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage);

        Model.OnlineFormViewModel GetOnlineApplicationFormById(int? id);

        string GetListOfChecklistDocuments(int? schemeId);

        DataSourceResult GetUploadedDocumentsForOnlineForm(DataSourceRequest request, int? formId);
        DataSourceResult GetUploadedDocumentsForReturnForm(DataSourceRequest request, int? formId);
        int ValidatePANnumber(string pan, int? areaId, int? schemeId);

        int UpdateOnlineApplicationForm(Model.OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage);

        Boolean RejectApplication(int Id);

        int ValidateFormNumber(string formNo);

        int ValidateOnlineFormPayment(string formNo);

        Model.OnlinePaymentViewModel SaveOnlinePaymentTransaction(Model.OnlineFormViewModel form);

        Model.OnlinePaymentViewModel GetOnlinePaymentTransactionDetailById(string transactionId, string paymentType);

        Model.OnlinePaymentViewModel UpdateOnlinePaymentTransaction(System.Web.Mvc.FormCollection form);

        int RemoveDocumentFromApplicationForm(string formNo, string filename);

        List<int> SendOTPforPayment(string formId);

        int UploadDocumentByFormId(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage);

        ChallanModel GeneratePaymentChallan(OnlineChallanViewModel objOnlineChallanViewModel);
        int SaveCreateChallan(int? rId, int AccountHeadId, int AccountSubHeadId, decimal? Amount);
        bool SaveGeneratedChallan(int? rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId, string parsedHTML);
        bool SaveGeneratedChallan(string challanId, string challan);
        string GetAccountNumber(int bankId, int branchId);
        bool RemoveChallanChargeDetail(int? rid, string headName, string subHeadName, decimal amount);
        List<OnlineChallanViewModel> GetGeneratedChallanDetails(int rid);

        bool IsDocumentUploaded(int? formId);
        OnlineChallanViewModel GenerateSchemeChallan(OnlineFormViewModel objOnlineFormView);

        DataSourceResult GetDirectorDetails(DataSourceRequest request, int? formId);

        int SaveDirectorDetails(int? formId, string directorName, decimal? share, int? directorTypeId, string pan);

        int RemoveDirectorDetails(int? formId, int? directorId);

        OnlineFormViewModel GetInitialDataForScheme();

        int UpdateCompanyDetail(OnlineFormViewModel model);
        OnlinePaymentViewModel GetOfflinePayment_Trans(int ApplicationFormId);
        string UpdateOfflinePayment(OnlineFormViewModel objOnlineFormViewModel, HttpPostedFileBase files);
        DataSourceResult GetApplicationFormIdForOfflinePayment(DataSourceRequest Req, int? ApplicationFormId);
        Boolean UpdateChallanStatus(int Id,int PaymentType);

        int SendMessageInBulk(string type);

        DataSourceResult GetUploadedDocumentsAfterScrutiny(DataSourceRequest request, int? formId, int? checklistIdstart, int? checklistIdend);

        int ValidateApplicationDetails(OnlineFormViewModel ObjOnlineFormViewModel);

        OnlineFormViewModel GetOnlineSchemeFormById(int? id);

        int SaveDirectorDetailsForOpenScheme(onlineDirectorViewModel model);

        DataSourceResult GetDirectorDetailsForOpenScheme(DataSourceRequest request, int? id);

        int RemoveDirectorDetailsForOpenScheme(int? id);

        string SaveOpenSchemeFormDetail(OnlineFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage);

        OnlineFormViewModel GetInitialDataForScheme(OnlineFormViewModel objOnlineFormViewModel);

        bool ChangePassword(int FormId, string email, string newPassword);

        int ValidateApplicationDetailsforForgotPassword(OnlineFormViewModel ObjOnlineFormViewModel);

        List<DropdownViewModel> GetAreaRangeByDepartment(int? schemeId, int? departmentId);

        int SaveUploadPreviousChallan(OnlineFormViewModel model, HttpPostedFileBase docs);

        List<DropdownViewModel> GetSchemeList();

        List<DropdownViewModel> GetPaymentStatusList();

        NICsingalwindowSystem GetNICSingleWindowData(OnlineFormViewModel objOnlineApplicationDetails);

        int SaveNICSingleWindowPayment(NewDataSet ObjWBasicDetailsGetModel);

        int UpdatePaymentTransaction_SingleWindowPortal(OnlineFormViewModel Objmodel);

        int SaveApplicationDetailForAllotment(int FormId);

        LetterViewModel GetLetterByBarcode(string barcode);

        DataSourceResult GetApplicantListAfterDraw(DataSourceRequest request, OnlineFormViewModel modal);

        DataSourceResult GetApplicantDetailForDraw(DataSourceRequest request, OnlineFormViewModel modal);

        int AllotPropertyAfterDraw(OnlineFormViewModel model);

        DataSourceResult GetOnlineApplicationFormIdList(DataSourceRequest request, int? schemeId, int? departmentId);

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

        OnlinePaymentViewModel UpdateChallanOnlinePaymentTransaction(System.Web.Mvc.FormCollection form);

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
