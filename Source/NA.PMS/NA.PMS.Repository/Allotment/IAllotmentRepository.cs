using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OfficeOpenXml;
using System.IO;
using System.Web.Mvc;
using NA.PMS.Model.Entities;

namespace NA.PMS.Repository
{
    public interface IAllotmentRepository
    {
        List<ApplicationFormModel> GetArchivedApplications();
        DataSourceResult GetAllApplications([DataSourceRequest]DataSourceRequest request);
        bool SaveApplicationForm(ApplicationFormModel applicationFormModel);
        DataSourceResult GetRefundDetails(DataSourceRequest req, int loginUser);
        //DataSourceResult GetAllotment(DataSourceRequest req);
        //List<AllotmentModel> GetAllotment();
        DataSourceResult GetAllotment(DataSourceRequest req);
        DataSourceResult GetAllotmentPartial(DataSourceRequest req);
        bool AddAllotment(AllotmentModel allotmentModel, int userid);
        //To Get All Applicants form Id.
        List<DDLStringList> GetAllForms(int SchemeId, int DepartmentId);
        ApplicationFormModel GetApplicantDetails(int rID);
        ApplicationFormModel GetApplicantDetails(string formno, int SchemeId, int departmentID);

        bool CheckFormNo(int schemeId, int departmentId, string formNo, int applicationId);
        bool CheckIssueDate(int schemeId, DateTime issueDate);
        ApplicationFormModel GetSchemeDates(int schemeId);
        List<DDList> GetRIDs();
        List<DDList> GetRIDsByDeptt();
        DataSourceResult GetRIDsByDeptt(DataSourceRequest Req, int Rid);
        ApplicationFormModel GetPersonalInfo(int rId);
        bool UpdatePersonalInfo(ApplicationFormModel applicationFormModel);
        DataSourceResult GetUnsuccessfulApplicants(DataSourceRequest request, int scID, int depID);
        string BulkUploadApplicationForm(int? schemeId, int? departmentId, System.Web.HttpPostedFileBase uploadExcel);
        string UploadBulkApplicationForm(int? schemeId, int? departmentId, System.Web.HttpPostedFileBase uploadExcel);
        Stream BulkDownloadApplicationForm(int? schemeId, int? departmentId);
        Stream DownloadAllotmentExcelFormat();
        //DataSourceResult GetAllotment(DataSourceRequest req);

        DataSourceResult GetAlloteeLists(DataSourceRequest sourceReq);

        UnsuccessfulApplicant GetUnsuccessfulLstDetails(int scID, int depID);
        //DataSourceResult GetAlloteeLists(DataSourceRequest sourceReq);
        bool UnsuccessfulListAction(int scID, int depID, int action, int loginUserID, string userVal);
        List<AllotteeListModel> AllotteeListView(int schemeId, int depid, DateTime allotmentDate);
        AllotteeListModel GetAllotteeListByID(int schemeId, int deptmentId, DateTime AllotmentDates);

        bool SaveRequetForApproval(int schemeId, int depid, string user, DateTime allotmentDate);
        List<DDList> GetAssineTo();
        List<ManageRequestModel> GetAllRequest();
        List<ManageRequestModel> AllotteeListForApproval(int schemeId, int depid, DateTime allotmentDate);
        bool SaveRequetForApprovalRequest(int schemeId, int depid, string comment, string status, bool isUnSuccessfullAplicants, DateTime allotmentDate);
        ManageRequestModel GetAllotteeListRequestByID(int schemeId, int deptmentId, DateTime AllotmentDates);
        List<ManageRequestModel> GetUnsuccessfullApplicants();
        //To Get All Property in Sector/Block-Property number.
        List<DDList> GetAllProperty(int SchemeId, int DepartmentId);
        DataSourceResult GetAllProperty(DataSourceRequest Req, int? SchemeId, int? DepartmentId);
        // To get the details of property on the basis of propertyid, scheme and department
        SchemePropertyTransDetail GetPropertyDetails(int rId);
        SchemePropertyTransDetail GetPropertyDetails(int propertyId, int schemeID, int departmentId);
        // To Get Allotment By Id for Edit.
        AllotmentModel GetAllotmentById(int Id);
        bool UpdateAllotment(AllotmentModel allotmentModel, int userId);
        ApplicationPayment GetApplicantPaymentDetails(string formno, int applicationId);
        ManageRequestModel GetUnsuccessfullAplicantstRequestByID(int schemeId, int deptmentId);
        List<ManageRequestModel> GetUnsuccessfullApplicantsForApproval(int schemeId, int depid);
        ChallanModel PrintViewAllotment(int propertyId, int schemeID, int departmentId);
        bool BulkUploadForAllotment(int? schemeId, int? departmentId, System.Web.HttpPostedFileBase uploadExcel);
        List<ChallanModel> GetAllotmentDetailsForBulkPrint(List<int> propIds, int schemeId, int deptId);
        // Method for updating the model object values used in calculation allotment money
        //AllotmentModel UpdateAllotmentModel(AllotmentModel allotmentModel);

        List<DDLStringList> GetEarnestMoneyBySchemeAndDeptId(int SchemeId, int DepartmentId);
        List<AllotmentLetterModel> GetAllotmentDetailsForBulkLetterPrint(List<int> rIds);

        int BulkPaymentSchedule(List<int> rIds);

        ApplicationFormModel GetApplicationFormDetailById(int applicationId);
        ChallanModel PrintViewAllotment(int propertyId, int schemeID, int departmentId, int rId);

        //To Get All form Id for compnay.
        List<DDLStringList> GetAllCompanyForms(int SchemeId, int DepartmentId);

        //To fill Director Grid on Add Company => under Manage Application.
        DataSourceResult GetDirectorDetailsByAppID(DataSourceRequest req, int applicationID);

        //To save Directors for Add company
        bool SaveDiretors(decimal directorShare, string directorName, int type, int applicationID);

        // To Add Company Details
        bool AddCompanyDetails(string data, string NewFirmName, string NewFirmProduct, int NewFirmStatus, int appId);

        // To get information from CIC.
        CICModel GetApplicantInfo(string formno, int SchemeId, int departmentID);

        string GetBulkAllotmnetLetterPrint(List<int> rIds);
        string GetBulkAllotmnetPaymentLetterPrint(List<int> rIds);

        //To Cancel Allotment request
        bool CancelAllotment(int rid);

        //To generate letter
        string Generateletter(int rid);

        List<SchemeAllotmentModel> SchemeListForAllotment();

        //To Get All Applicants form Id for Allotment process excluding alloted form number.
        List<DDLStringList> GetAllFormsForAllotment(int SchemeId, int DepartmentId);
        DataSourceResult GetAllFormsForAllotment(DataSourceRequest Req, int? SchemeId, int? DepartmentId);

        //To Check Allotment Date
        bool CheckAllotmentDate(int schemeId, int departmentId, DateTime allotmentDate);
        bool CheckUTROrDDNumber(string number, string type, int appId);

        //To Get All Applicants form Id for Allotment process.
        List<DDLStringList> GetAllFormsForView(int SchemeId, int DepartmentId);

        //Online application download and payment
        OnlineApplicationFormModel OnlineApplicationRequest(OnlineApplicationFormModel applicationFormModel);

        OnlineApplicationFormModel ViewOnlineDetails(int onlineappId);

        //Edit Online application Basic info
        OnlineApplicationFormModel EditOnlineDetails(OnlineApplicationFormModel applicationFormModel);

        List<SchemeAllotmentModel> GetSchemeListForAllotment();
        bool SaveFileDetailsToDB(UploadDetails details);
        //List<DepartmentAllotmentModel> FilterDepartmentOnScheme();
        List<ChecklistDocuments> GetApplicationChcklstDocuments(DataSourceRequest request, int applicationId);

        List<DepartmentAllotmentModel> FilterDepartmentOnScheme(int SchemeId);

        List<DDList> GetAllBanksforOnline();

        OnlineApplicationFormModel GetAreaDetails(int id);

        OnlineApplicationFormModel GetEarneshMoney(int id, int deptt, int floor);

        List<DDList> GetFloors(int schemeId, int depttID);

        OnlineApplicationDetailsTrans SaveTrasOnlineDetails(int id);

        OnlineApplicationDetailsTrans GetTrasactionDetails(string id, string paymenttype);

        OnlineApplicationDetailsTrans UpdateTrasactionDetails(FormCollection form);

        DataSourceResult GetOnlineApplications(DataSourceRequest req);        

        bool SaveCommentByApplicationID(int requestNo, string Comment, bool acceptReject);

        bool SaveCommentByAppID(int requestNo, string Comment, bool acceptReject, string MobNo, string EmailId);

        ApplicatandDetailsModel GetDetails(int applid);

        OnlineApplicationDetailsTrans SaveOtherPaymentTras(int id);
        DataSourceResult GetRIDsForApplication(DataSourceRequest Req);
        bool UpdateAddress(UpdateAddress objUpdateAddress);
        UpdateAddress GetApplicationDetailsById(int rId);
        UpdateDetails GetFormDetailsById(int rId);
        bool UpdateFormDetails(UpdateDetails ObjUpdateDetails);
        List<ApplicationDetail> GetDetailsOfAllotedProprety(int schemeId, int depid, DateTime allotmentDate);

        int PropertyAllotmentForOnlineApplicationForm(OnlineFormViewModel model);

        DataSourceResult GetApplicationFormAsDataSourceByApplicationId(DataSourceRequest request, int? applicationId);

        DataSourceResult GetPropertyDetailAsDataSourceByPropertyId(DataSourceRequest request, int? propertyId);

        DataSourceResult GetAllottedPropertyFormListById(DataSourceRequest request, AllotmentViewModel model);

        int ValidatePropertyAndApplicationForm(FormViewModel model);

        int PropertyAllotmentForApplicationForm(FormViewModel model);

        DataSourceResult GetAllottedPropertyFormListForApproval(DataSourceRequest request, AllotmentViewModel model);

        int UpdateAllottedPropertyStatus(AllotmentViewModel model);

        string ViewletterTemplate(LetterHistory _LetterHistory);

        DataSourceResult GetApplicationFormList(DataSourceRequest request, FormViewModel model);

        PropertyViewModel GetApplicantDetailsToTransferProperty(PropertyViewModel model);

        int SavePropertyTransferDetail(TransferViewModel model);

        TransferViewModel GetTransferedPropertyDetailById(TransferViewModel model);

        DataSourceResult GetAllotmentRequestByUserIdAsDataSource(DataSourceRequest request, AllotmentViewModel model);

        DataSourceResult GetAllottedPropertyListByYearAsDataSource(DataSourceRequest request, AllotmentViewModel model);
    }
}
