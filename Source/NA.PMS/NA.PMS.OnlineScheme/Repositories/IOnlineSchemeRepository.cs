using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.OnlineScheme
{
    public interface IOnlineSchemeRepository
    {
        DataSourceResult GetOnlineSchemeApplications(DataSourceRequest request, SchemeFormViewModel modal);
        
        DataSourceResult GetDocumentTypeChecklistForOnlineSchemeApplication(DataSourceRequest request, SchemeFormViewModel modal);

        SchemeFormViewModel SaveOnlineSchemeApplicationForm(SchemeFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage);

        SchemeFormViewModel SaveDocumentsForOnlineSchemeApplication(SchemeFormViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files);

        SchemeFormViewModel GetOnlineSchemeApplicationFormById(int? id);

        DataSourceResult GetChecklistDocumentsAsDataSource(DataSourceRequest request, SchemeFormViewModel model);

        DataSourceResult GetUploadedDocumentsAsDataSource(DataSourceRequest request, SchemeFormViewModel model);

        SchemePaymentViewModel SaveOnlineSchemePaymentTransaction(SchemeFormViewModel form);

        SchemePaymentViewModel GetOnlineSchemePaymentTransactionDetailById(SchemePaymentViewModel paymentModel);

        SchemeFormViewModel GetOnlineSchemeBasicDetail(SchemeFormViewModel modal);

        SchemeFormViewModel ValidateOnlineSchemeFormByType(SchemeFormViewModel modal);

        SchemeFormViewModel ActionForOnlineSchemeApplicationForm(SchemeFormViewModel modal);

        SchemePaymentViewModel SaveOnlineSchemePaymentTransaction(System.Web.Mvc.FormCollection form);

        int RemoveDocumentFromApplicationForm(string formNo, string filename);

        SchemeFormViewModel GetSchemeFormFeeAndChargesByArea(SchemeFormViewModel model);

        //DataSourceResult GetOnlineSchemeApplicationsForAdmin(DataSourceRequest request, SchemeFormViewModel modal);
        //OnlineApplicationDetailsTrans GetOnlineApplicationReceipt(OnlineApplicationDetailsTrans ObjOnlineApplicationDetailsTrans);

        //SchemeFormViewModel GetApplicationFeeAndCharges(int? schemeId, int? departmentId, int? propertyTypeId, int? areaTypeId);

       

        //SchemeFormViewModel SaveOnlineSchemeApplicationForm(SchemeFormViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage);
        
        

        //string GetListOfChecklistDocuments(int? schemeId);

       

        //DataSourceResult GetUploadedDocumentsForReturnForm(DataSourceRequest request, int? formId);
        //int ValidatePANnumber(string pan, int? areaId, int? schemeId);

        //int UpdateOnlineApplicationForm(SchemeFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage);

        //Boolean RejectApplication(int Id);

        //int ValidateFormNumber(string formNo);

        //int ValidateOnlineFormPayment(string formNo);

       
        //SchemePaymentViewModel GetOnlinePaymentTransactionDetailById(string transactionId, string paymentType);
       

        //SchemePaymentViewModel UpdateOnlinePaymentTransaction(System.Web.Mvc.FormCollection form);

       

        //List<int> SendOTPforPayment(string formId);

        //int UploadDocumentByFormId(SchemeFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage);

        //SchemeChallanViewModel GeneratePaymentChallan(SchemeChallanViewModel objOnlineChallanViewModel);
        //int SaveCreateChallan(int? rId, int AccountHeadId, int AccountSubHeadId, decimal? Amount);
        //bool SaveGeneratedChallan(int? rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId, string parsedHTML);
        //bool SaveGeneratedChallan(string challanId, string challan);
        //string GetAccountNumber(int bankId, int branchId);
        //bool RemoveChallanChargeDetail(int? rid, string headName, string subHeadName, decimal amount);
        //List<SchemeChallanViewModel> GetGeneratedChallanDetails(int rid);

        //bool IsDocumentUploaded(int? formId);
        //SchemeChallanViewModel GenerateSchemeChallan(SchemeFormViewModel objOnlineFormView);

        //DataSourceResult GetDirectorDetails(DataSourceRequest request, int? formId);

        //int SaveDirectorDetails(int? formId, string directorName, decimal? share, int? directorTypeId, string pan);

        //int RemoveDirectorDetails(int? formId, int? directorId);

        //SchemeFormViewModel GetInitialDataForScheme();

        //int UpdateCompanyDetail(SchemeFormViewModel model);
        //SchemePaymentViewModel GetOfflinePayment_Trans(int ApplicationFormId);
        //string UpdateOfflinePayment(SchemeFormViewModel objSchemeFormViewModel, HttpPostedFileBase files);
        //DataSourceResult GetApplicationFormIdForOfflinePayment(DataSourceRequest Req, int? ApplicationFormId);
        //Boolean UpdateChallanStatus(int Id, int PaymentType);

        //int SendMessageInBulk(string type);

        //DataSourceResult GetUploadedDocumentsAfterScrutiny(DataSourceRequest request, int? formId, int? checklistIdstart, int? checklistIdend);

        //int ValidateApplicationDetails(SchemeFormViewModel ObjOnlineFormViewModel);

        //SchemeFormViewModel GetOnlineSchemeFormById(int? id);

        //int SaveDirectorDetailsForOpenScheme(SchemeProposedFirmViewModel model);

        //DataSourceResult GetDirectorDetailsForOpenScheme(DataSourceRequest request, int? id);

        //int RemoveDirectorDetailsForOpenScheme(int? id);

        //string SaveOpenSchemeFormDetail(SchemeFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage);

        //SchemeFormViewModel GetInitialDataForScheme(SchemeFormViewModel objOnlineFormViewModel);

        ////bool ChangePassword(int FormId, string email, string newPassword);

        //int ValidateApplicationDetailsforForgotPassword(SchemeFormViewModel ObjSchemeFormViewModel);

        ////List<DropdownViewModel> GetAreaRangeByDepartment(int? schemeId, int? departmentId);

        //int SaveUploadPreviousChallan(SchemeFormViewModel model, HttpPostedFileBase docs);

        //List<DropdownViewModel> GetSchemeList();

        //List<DropdownViewModel> GetPaymentStatusList();

        //NICsingalwindowSystem GetNICSingleWindowData(SchemeFormViewModel objOnlineApplicationDetails);

        //int SaveNICSingleWindowPayment(NewDataSet ObjWBasicDetailsGetModel);

        //int UpdatePaymentTransaction_SingleWindowPortal(SchemeFormViewModel Objmodel);

        //int SaveApplicationDetailForAllotment(int FormId);

        //LetterViewModel GetLetterByBarcode(string barcode);

        //DataSourceResult GetApplicantListAfterDraw(DataSourceRequest request, SchemeFormViewModel modal);

        //DataSourceResult GetApplicantDetailForDraw(DataSourceRequest request, SchemeFormViewModel modal);

        //int AllotPropertyAfterDraw(SchemeFormViewModel model);

        //DataSourceResult GetOnlineApplicationFormIdList(DataSourceRequest request, int? schemeId, int? departmentId);

        //int UpdateAllottedPropertyAfterDraw(int? formId, string actionType);

        //int ValidatePropertyForAllotment(string sector, string block, string plot);

        //SchemePaymentViewModel GetPreviousChallanPayment_Trans(int ApplicationFormId);

        ////ResultMessage SaveApplicationProcessRequest(SchemeFormViewModel objSchemeFormViewModel);

        //DataSourceResult GetOnlineApplicationProcessRequests(DataSourceRequest request);

        //DataSourceResult GetOnlineApplicationProcess(DataSourceRequest request);

        //ResultMessage UpdateOnlineApplicationStatus(OnlineApplicationDetailProcess objOnlineApplicationDetailProcess);

        //DataSourceResult GetPropertyDetailAsDataSourceByPropertyId(DataSourceRequest request, int? propertyId);

        //DataSourceResult GetApplicationDetailAsDataSourceByApplicationId(DataSourceRequest request, int? applicationId);

        //int ValidatePropertyAndApplicationForm(SchemeFormViewModel model);

        //DataSourceResult GetAllottedOnlineFormPropertyList(DataSourceRequest request, SchemeFormViewModel model);

        //DataSourceResult GetOnlineApplicationsForConsultant(DataSourceRequest request, SchemeFormViewModel modal);

        //DataSourceResult GetFloorAreaListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        //SchemeFormViewModel GetIndustrialSchemeInformation();

        //DataSourceResult GetDocumentListForOnlineSchemeForm(DataSourceRequest request, SchemeFormViewModel model);

        //SchemeFormViewModel GetSchemeInformationForOnlineApplication(SchemeFormViewModel model);

        //DataSourceResult GetOnlineChecklistDocument(DataSourceRequest request, SchemeDocumentViewModel model);

        //DataSourceResult GetChecklistTypeList(DataSourceRequest request, SchemeDocumentViewModel model);

        //int ValidatePANForOnlineScheme(SchemeFormViewModel model);

        //SchemePaymentViewModel GetGeneratedChallanDetailById(SchemePaymentViewModel onlinePaymentViewModel);

        //void SaveChallanOnlinePaymentTransaction(SchemePaymentViewModel model);

        //SchemePaymentViewModel UpdateChallanOnlinePaymentTransaction(System.Web.Mvc.FormCollection form);

        //DataSourceResult GetOnlinePaymentDetailsAsDataSource(DataSourceRequest request, SchemePaymentViewModel model);

        //SchemePaymentViewModel GetOnlinePaymentDetailById(SchemePaymentViewModel model);

        //DataSourceResult GetOnlinePaidChallanIdListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        //DataSourceResult GetOnlineTransactionIdListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        //int UpdateOnlinePaymentDetailById(SchemePaymentViewModel model);

        SchemeFormViewModel SaveOpenEndedSchemeFormDetail(SchemeFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage);

        SchemeFormViewModel SaveDocumentsForOpenEndScheme(SchemeFormViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files);

        SchemeFormViewModel GetOpenEndedSchemeFormDataById(SchemeFormViewModel form);

        SchemeFormViewModel UpdateOpenEndSchemeFormPaymentStatus(SchemeFormViewModel model);

        SchemeFormViewModel SaveProjectAndRefundDetailForOpenEndScheme(SchemeFormViewModel model);

        //SchemeProposedFirmViewModel SaveDirectorDetailToDataBaseForOpenScheme(SchemeProposedFirmViewModel model);

        //DataSourceResult GetDirectorDetailsFromDatabaseForOpenScheme(DataSourceRequest request, int? id);

        SchemeProposedFirmViewModel SaveProposedFirmDetailForOpenEndScheme(SchemeProposedFirmViewModel model);

        DataSourceResult GetProposedFirmDetailsForOpenEndScheme(DataSourceRequest request, SchemeProposedFirmViewModel model);



        DataSourceResult GetProposedFirmDirectorsDetailAsDataSource(DataSourceRequest request, SchemeFormViewModel model);

        SchemeProposedFirmViewModel SaveProposedFirmDirectorsDetail(SchemeProposedFirmViewModel model);

        SchemeFormViewModel MigrateOnlineSchemeFormDataForAllotment(SchemeFormViewModel form);

        DataSourceResult GetApplicationFormIdListAsDataSource(DataSourceRequest request, SchemeFormViewModel model);

        SchemeFormViewModel ValidatePropertyForAllotment(SchemeFormViewModel model);

        SchemeFormViewModel SaveSchemeFormProcess(SchemeFormViewModel model);

        DataSourceResult GetOnlineApplicationProcessAsDataSource(DataSourceRequest request, SchemeFormViewModel model);

        SchemeFormViewModel SaveOnlineApplicationProcessStatus(SchemeFormViewModel model);

        SchemeFormViewModel SaveAndGetOnlineSchemeFormCallan(SchemeFormViewModel model);

        SchemeFormViewModel SaveGeneratedSchemeFormChallan(SchemeFormViewModel form, HttpPostedFileBase files);

        SchemeChallanViewModel GenerateSchemeFormChallanByTemplate(SchemeFormViewModel form);

        SchemeFormViewModel SaveSchemeFormPaidChallanDetail(SchemeFormViewModel form, HttpPostedFileBase files);
    }
}
