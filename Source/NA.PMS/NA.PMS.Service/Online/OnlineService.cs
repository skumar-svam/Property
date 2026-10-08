using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Service
{
    public class OnlineService : IOnlineService
    {
        IOnlineRepository _onlineRepository;
        public OnlineService(IOnlineRepository onlineRepository)
        {
            _onlineRepository = onlineRepository;
        }

        public DataSourceResult GetOnlineApplications(DataSourceRequest request, OnlineFormViewModel modal)
        {
            return _onlineRepository.GetOnlineApplications(request, modal);
        }

        public DataSourceResult GetOnlineApplicationsForAdmin(DataSourceRequest request, OnlineFormViewModel modal)
        {
            return _onlineRepository.GetOnlineApplicationsForAdmin(request, modal);
        }

        public OnlineApplicationDetailsTrans GetOnlineApplicationReceipt(OnlineApplicationDetailsTrans ObjOnlineApplicationDetailsTrans)
        {
            return _onlineRepository.GetOnlineApplicationReceipt(ObjOnlineApplicationDetailsTrans);
        }


        public DataSourceResult GetChecklistDocumentsForOnlineApplication(DataSourceRequest request, int? schemeId)
        {
            return _onlineRepository.GetChecklistDocumentsForOnlineApplication(request, schemeId);
        }


        public OnlineFormViewModel GetApplicationFeeAndCharges(int? schemeId, int? departmentId, int? propertyTypeId, int? areaTypeId)
        {
            return _onlineRepository.GetApplicationFeeAndCharges(schemeId, departmentId, propertyTypeId, areaTypeId);
        }

        public OnlineFormViewModel GetOnlineApplicationFeeAndCharges(OnlineFormViewModel model)
        {
            return _onlineRepository.GetOnlineApplicationFeeAndCharges(model);
        }


        public int SaveOnlineApplicationForm(OnlineFormViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            return _onlineRepository.SaveOnlineApplicationForm(model, files, userImage, signatureImage);
        }

        public int UpdateOnlineApplicationForm(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            return _onlineRepository.UpdateOnlineApplicationForm(model, files, userImage, signatureImage);
        }

        public OnlineFormViewModel GetOnlineApplicationFormById(int? id)
        {
            return _onlineRepository.GetOnlineApplicationFormById(id);
        }


        public string GetListOfChecklistDocuments(int? schemeId)
        {
            return _onlineRepository.GetListOfChecklistDocuments(schemeId);
        }


        public DataSourceResult GetUploadedDocumentsForOnlineForm(DataSourceRequest request, int? formId)
        {
            return _onlineRepository.GetUploadedDocumentsForOnlineForm(request, formId);
        }

        public DataSourceResult GetUploadedDocumentsForReturnForm(DataSourceRequest request, int? formId)
        {
            return _onlineRepository.GetUploadedDocumentsForReturnForm(request, formId);
        }

        public int ValidatePANnumber(string pan, int? areaId, int? schemeId)
        {
            return _onlineRepository.ValidatePANnumber(pan, areaId, schemeId);
        }


        public int ValidateFormNumber(string formNo)
        {
            return _onlineRepository.ValidateFormNumber(formNo);
        }


        public int ValidateOnlineFormPayment(string formNo)
        {
            return _onlineRepository.ValidateOnlineFormPayment(formNo);
        }


        public OnlinePaymentViewModel SaveOnlinePaymentTransaction(OnlineFormViewModel form)
        {
            return _onlineRepository.SaveOnlinePaymentTransaction(form);
        }


        public OnlinePaymentViewModel GetOnlinePaymentTransactionDetailById(string transactionId, string paymentType)
        {
            return _onlineRepository.GetOnlinePaymentTransactionDetailById(transactionId, paymentType);
        }

        public OnlinePaymentViewModel UpdateOnlinePaymentTransaction(System.Web.Mvc.FormCollection form)
        {
            return _onlineRepository.UpdateOnlinePaymentTransaction(form);
        }

        public Boolean RejectApplication(int Id)
        {
            return _onlineRepository.RejectApplication(Id);
        }


        public int RemoveDocumentFromApplicationForm(string formNo, string filename)
        {
            return _onlineRepository.RemoveDocumentFromApplicationForm(formNo, filename);
        }


        public List<int> SendOTPforPayment(string formId)
        {
            return _onlineRepository.SendOTPforPayment(formId);
        }


        public int UploadDocumentByFormId(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            return _onlineRepository.UploadDocumentByFormId(model, files, userImage, signatureImage);
        }


        public ChallanModel GeneratePaymentChallan(OnlineChallanViewModel objOnlineChallanViewModel)
        {
            return _onlineRepository.GeneratePaymentChallan(objOnlineChallanViewModel);
        }
        public int SaveCreateChallan(int? rId, int AccountHeadId, int AccountSubHeadId, decimal? Amount)
        {
            return _onlineRepository.SaveCreateChallan(rId, AccountHeadId, AccountSubHeadId, Amount);
        }
        public bool SaveGeneratedChallan(int? rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId, string parsedHTML)
        {
            return _onlineRepository.SaveGeneratedChallan(rId, bankId, branchId, DdlAccountNumber, DepttId, parsedHTML);
        }
        public bool SaveGeneratedChallan(string challanId, string challan)
        {
            return _onlineRepository.SaveGeneratedChallan(challanId, challan);
        }
        public string GetAccountNumber(int bankId, int branchId)
        {
            return _onlineRepository.GetAccountNumber(bankId, branchId);
        }

        public bool RemoveChallanChargeDetail(int? rid, string headName, string subHeadName, decimal amount)
        {
            return _onlineRepository.RemoveChallanChargeDetail(rid, headName, subHeadName, amount);
        }
        public List<OnlineChallanViewModel> GetGeneratedChallanDetails(int rid)
        {
            return _onlineRepository.GetGeneratedChallanDetails(rid);
        }

        public bool IsDocumentUploaded(int? formId)
        {
            return _onlineRepository.IsDocumentUploaded(formId);
        }

        public OnlineChallanViewModel GenerateSchemeChallan(OnlineFormViewModel objOnlineFormView)
        {
            return _onlineRepository.GenerateSchemeChallan(objOnlineFormView);
        }


        public DataSourceResult GetDirectorDetails(DataSourceRequest request, int? formId)
        {
            return _onlineRepository.GetDirectorDetails(request, formId);
        }

        public int SaveDirectorDetails(int? formId, string directorName, decimal? share, int? directorTypeId, string pan)
        {
            return _onlineRepository.SaveDirectorDetails(formId, directorName, share, directorTypeId, pan);
        }

        public int RemoveDirectorDetails(int? formId, int? directorId)
        {
            return _onlineRepository.RemoveDirectorDetails(formId, directorId);
        }


        public OnlineFormViewModel GetInitialDataForScheme()
        {
            return _onlineRepository.GetInitialDataForScheme();
        }


        public int UpdateCompanyDetail(OnlineFormViewModel model)
        {
            return _onlineRepository.UpdateCompanyDetail(model);
        }

        public OnlinePaymentViewModel GetOfflinePayment_Trans(int ApplicationFormId)
        {
            return _onlineRepository.GetOfflinePayment_Trans(ApplicationFormId);
        }

        public string UpdateOfflinePayment(OnlineFormViewModel objOnlineFormViewModel, HttpPostedFileBase files)
        {
            return _onlineRepository.UpdateOfflinePayment(objOnlineFormViewModel, files);
        }

        public DataSourceResult GetApplicationFormIdForOfflinePayment(DataSourceRequest Req, int? ApplicationFormId)
        {
            return _onlineRepository.GetApplicationFormIdForOfflinePayment(Req, ApplicationFormId);
        }

        public Boolean UpdateChallanStatus(int Id, int PaymentType)
        {
            return _onlineRepository.UpdateChallanStatus(Id, PaymentType);
        }

        public int SendMessageInBulk(string type)
        {
            return _onlineRepository.SendMessageInBulk(type);
        }


        public DataSourceResult GetUploadedDocumentsAfterScrutiny(DataSourceRequest request, int? formId, int? checklistIdstart, int? checklistIdend)
        {
            return _onlineRepository.GetUploadedDocumentsAfterScrutiny(request, formId, checklistIdstart, checklistIdend);
        }

        public int ValidateApplicationDetails(OnlineFormViewModel ObjOnlineFormViewModel)
        {
            return _onlineRepository.ValidateApplicationDetails(ObjOnlineFormViewModel);
        }

        public OnlineFormViewModel GetOnlineSchemeFormById(int? id)
        {
            return _onlineRepository.GetOnlineSchemeFormById(id);
        }

        public OnlineFormViewModel GetInitialDataForScheme(OnlineFormViewModel objOnlineFormViewModel)
        {
            return _onlineRepository.GetInitialDataForScheme(objOnlineFormViewModel);
        }


        public int SaveDirectorDetailsForOpenScheme(onlineDirectorViewModel model)
        {
            return _onlineRepository.SaveDirectorDetailsForOpenScheme(model);
        }

        public DataSourceResult GetDirectorDetailsForOpenScheme(DataSourceRequest request, int? id)
        {
            return _onlineRepository.GetDirectorDetailsForOpenScheme(request, id);
        }

        public int RemoveDirectorDetailsForOpenScheme(int? id)
        {
            return _onlineRepository.RemoveDirectorDetailsForOpenScheme(id);
        }


        public string SaveOpenSchemeFormDetail(OnlineFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            return _onlineRepository.SaveOpenSchemeFormDetail(model, userImage, signatureImage);
        }

        public bool ChangePassword(int FormId, string email, string newPassword)
        {
            return _onlineRepository.ChangePassword(FormId, email, newPassword);
        }

        public int ValidateApplicationDetailsforForgotPassword(OnlineFormViewModel ObjOnlineFormViewModel)
        {
            return _onlineRepository.ValidateApplicationDetailsforForgotPassword(ObjOnlineFormViewModel);
        }

        public List<DropdownViewModel> GetAreaRangeByDepartment(int? schemeId, int? departmentId)
        {
            return _onlineRepository.GetAreaRangeByDepartment(schemeId, departmentId);
        }

        public int SaveUploadPreviousChallan(OnlineFormViewModel model, HttpPostedFileBase docs)
        {
            return _onlineRepository.SaveUploadPreviousChallan(model, docs);
        }

        public List<DropdownViewModel> GetSchemeList()
        {
            return _onlineRepository.GetSchemeList();
        }

        public List<DropdownViewModel> GetPaymentStatusList()
        {
            return _onlineRepository.GetPaymentStatusList();
        }

        public NICsingalwindowSystem GetNICSingleWindowData(OnlineFormViewModel objOnlineApplicationDetails)
        {
            return _onlineRepository.GetNICSingleWindowData(objOnlineApplicationDetails);
        }

        public int SaveNICSingleWindowPayment(NewDataSet ObjWBasicDetailsGetModel)
        {
            return _onlineRepository.SaveNICSingleWindowPayment(ObjWBasicDetailsGetModel);
        }

        public int UpdatePaymentTransaction_SingleWindowPortal(OnlineFormViewModel Objmodel)
        {
            return _onlineRepository.UpdatePaymentTransaction_SingleWindowPortal(Objmodel);
        }

        public int SaveApplicationDetailForAllotment(int FormId)
        {
            return _onlineRepository.SaveApplicationDetailForAllotment(FormId);
        }


        public LetterViewModel GetLetterByBarcode(string barcode)
        {
            return _onlineRepository.GetLetterByBarcode(barcode);
        }


        public DataSourceResult GetApplicantListAfterDraw(DataSourceRequest request, OnlineFormViewModel modal)
        {
            return _onlineRepository.GetApplicantListAfterDraw(request, modal);
        }

        public DataSourceResult GetApplicantDetailForDraw(DataSourceRequest request, OnlineFormViewModel modal)
        {
            return _onlineRepository.GetApplicantDetailForDraw(request, modal);
        }


        public int AllotPropertyAfterDraw(OnlineFormViewModel model)
        {
            return _onlineRepository.AllotPropertyAfterDraw(model);
        }


        public DataSourceResult GetOnlineApplicationFormIdList(DataSourceRequest request, int? schemeId, int? departmentId)
        {
            return _onlineRepository.GetOnlineApplicationFormIdList(request, schemeId, departmentId);
        }


        public int UpdateAllottedPropertyAfterDraw(int? formId, string actionType)
        {
            return _onlineRepository.UpdateAllottedPropertyAfterDraw(formId, actionType);
        }


        public int ValidatePropertyForAllotment(string sector, string block, string plot)
        {
            return _onlineRepository.ValidatePropertyForAllotment(sector, block, plot);
        }

        public OnlinePaymentViewModel GetPreviousChallanPayment_Trans(int ApplicationFormId)
        {
            return _onlineRepository.GetPreviousChallanPayment_Trans(ApplicationFormId);
        }

        public ResultMessage SaveApplicationProcessRequest(OnlineFormViewModel objOnlineFormViewModel)
        {
            return _onlineRepository.SaveApplicationProcessRequest(objOnlineFormViewModel);
        }

        public DataSourceResult GetOnlineApplicationProcessRequests(DataSourceRequest request)
        {
            return _onlineRepository.GetOnlineApplicationProcessRequests(request);
        }

        public DataSourceResult GetOnlineApplicationProcess(DataSourceRequest request)
        {
            return _onlineRepository.GetOnlineApplicationProcess(request);
        }

        public ResultMessage UpdateOnlineApplicationStatus(OnlineApplicationDetailProcess objOnlineApplicationDetailProcess)
        {
            return _onlineRepository.UpdateOnlineApplicationStatus(objOnlineApplicationDetailProcess);
        }


        public DataSourceResult GetPropertyDetailAsDataSourceByPropertyId(DataSourceRequest request, int? propertyId)
        {
            return _onlineRepository.GetPropertyDetailAsDataSourceByPropertyId(request, propertyId);
        }

        public DataSourceResult GetApplicationDetailAsDataSourceByApplicationId(DataSourceRequest request, int? applicationId)
        {
            return _onlineRepository.GetApplicationDetailAsDataSourceByApplicationId(request, applicationId);
        }


        public int ValidatePropertyAndApplicationForm(OnlineFormViewModel model)
        {
            return _onlineRepository.ValidatePropertyAndApplicationForm(model);
        }


        public DataSourceResult GetAllottedOnlineFormPropertyList(DataSourceRequest request, OnlineFormViewModel model)
        {
            return _onlineRepository.GetAllottedOnlineFormPropertyList(request, model);
        }


        public DataSourceResult GetOnlineApplicationsForConsultant(DataSourceRequest request, OnlineFormViewModel modal)
        {
            return _onlineRepository.GetOnlineApplicationsForConsultant(request, modal);
        }


        public DataSourceResult GetFloorAreaListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _onlineRepository.GetFloorAreaListAsDataSource(request, model);
        }


        public OnlineFormViewModel GetIndustrialSchemeInformation()
        {
            return _onlineRepository.GetIndustrialSchemeInformation();
        }


        public DataSourceResult GetDocumentListForOnlineSchemeForm(DataSourceRequest request, OnlineFormViewModel model)
        {
            return _onlineRepository.GetDocumentListForOnlineSchemeForm(request, model);
        }


        public OnlineFormViewModel GetSchemeInformationForOnlineApplication(OnlineFormViewModel model)
        {
            return _onlineRepository.GetSchemeInformationForOnlineApplication(model);
        }


        public DataSourceResult GetOnlineChecklistDocument(DataSourceRequest request, OnlineDocumentViewModel model)
        {
            return _onlineRepository.GetOnlineChecklistDocument(request, model);
        }


        public DataSourceResult GetChecklistTypeList(DataSourceRequest request, OnlineDocumentViewModel model)
        {
            return _onlineRepository.GetChecklistTypeList(request, model);
        }


        public int ValidatePANForOnlineScheme(OnlineFormViewModel model)
        {
            return _onlineRepository.ValidatePANForOnlineScheme(model);
        }


        public OnlinePaymentViewModel GetGeneratedChallanDetailById(OnlinePaymentViewModel onlinePaymentViewModel)
        {
            return _onlineRepository.GetGeneratedChallanDetailById(onlinePaymentViewModel);
        }

        public void SaveChallanOnlinePaymentTransaction(OnlinePaymentViewModel model)
        {
            _onlineRepository.SaveChallanOnlinePaymentTransaction(model);
        }

        public OnlinePaymentViewModel UpdateChallanOnlinePaymentTransaction(System.Web.Mvc.FormCollection form)
        {
            return _onlineRepository.UpdateChallanOnlinePaymentTransaction(form);
        }


        public DataSourceResult GetOnlinePaymentDetailsAsDataSource(DataSourceRequest request, OnlinePaymentViewModel model)
        {
            return _onlineRepository.GetOnlinePaymentDetailsAsDataSource(request, model);
        }


        public OnlinePaymentViewModel GetOnlinePaymentDetailById(OnlinePaymentViewModel model)
        {
            return _onlineRepository.GetOnlinePaymentDetailById(model);
        }


        public DataSourceResult GetOnlinePaidChallanIdListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _onlineRepository.GetOnlinePaidChallanIdListAsDataSource(request, model);
        }


        public DataSourceResult GetOnlineTransactionIdListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _onlineRepository.GetOnlineTransactionIdListAsDataSource(request, model);
        }


        public int UpdateOnlinePaymentDetailById(OnlinePaymentViewModel model)
        {
            return _onlineRepository.UpdateOnlinePaymentDetailById(model);
        }


        public OnlineFormViewModel SaveOpenEndedSchemeFormDetail(OnlineFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            return _onlineRepository.SaveOpenEndedSchemeFormDetail(model, userImage, signatureImage);
        }


        public OnlineFormViewModel GetOpenEndedSchemeFormDataById(OnlineFormViewModel form)
        {
            return _onlineRepository.GetOpenEndedSchemeFormDataById(form);
        }


        public OnlineFormViewModel UpdateOESFormPaymentStatus(OnlineFormViewModel model)
        {
            return _onlineRepository.UpdateOESFormPaymentStatus(model);
        }


        public OnlineFormViewModel SaveProjectAndRefundDetailForOpenEndScheme(OnlineFormViewModel model)
        {
            return _onlineRepository.SaveProjectAndRefundDetailForOpenEndScheme(model);
        }


        public onlineDirectorViewModel SaveDirectorDetailToDataBaseForOpenScheme(onlineDirectorViewModel model)
        {
            return _onlineRepository.SaveDirectorDetailToDataBaseForOpenScheme(model);
        }


        public DataSourceResult GetDirectorDetailsFromDatabaseForOpenScheme(DataSourceRequest request, int? id)
        {
            return _onlineRepository.GetDirectorDetailsFromDatabaseForOpenScheme(request, id);
        }
    }
}