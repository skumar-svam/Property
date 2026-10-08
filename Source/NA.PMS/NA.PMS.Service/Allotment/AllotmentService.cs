using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Web;
using System.IO;
using NA.PMS.Service.BusinessRuleEngine;
using NA.PMS.Repository;
using System.Web.Mvc;

namespace NA.PMS.Service
{
    public class AllotmentService : IAllotmentService
    {
        IAllotmentRepository _allotmentRepository;
        private IAllotmentEngine _allotmentEngine;
        private IPaymentEngine _paymentEngine;
        public AllotmentService(IPaymentEngine paymentEngine, IAllotmentEngine allotmentEngine, IAllotmentRepository allotmentRepository)
        {
            _allotmentRepository = allotmentRepository;
            _allotmentEngine = allotmentEngine;
            _paymentEngine = paymentEngine;
        }
        public List<ApplicationFormModel> GetArchivedApplications()
        {
            return _allotmentRepository.GetArchivedApplications();
        }
        public DataSourceResult GetAllApplications([DataSourceRequest]DataSourceRequest request)
        {
            return _allotmentRepository.GetAllApplications(request);
        }
        public bool SaveApplicationForm(ApplicationFormModel applicationFormModel)
        {
            return _allotmentRepository.SaveApplicationForm(applicationFormModel);
        }

        //public List<AllotmentModel> GetAllotment()
        public DataSourceResult GetAllotment(DataSourceRequest req)
        {
            return _allotmentRepository.GetAllotment(req);
        }
        public DataSourceResult GetAllotmentPartial(DataSourceRequest req)
        {
            return _allotmentRepository.GetAllotmentPartial(req);
        }
        // To Add new Allotment Details. 
        public bool AddAllotment(AllotmentModel allotmentModel, int userid)
        {
            // CalculateAllotmentMoney(allotmentModel) is used for calculating the allotment money and update the same in model
            return _allotmentRepository.AddAllotment(_allotmentEngine.CalculateAllotmentMoney(allotmentModel), userid);
        }
        //To Get All Applicants form Id.
        public List<DDLStringList> GetAllForms(int SchemeId, int DepartmentId)
        {
            return _allotmentRepository.GetAllForms(SchemeId, DepartmentId);
        }
        public ApplicationFormModel GetApplicantDetails(int rID)
        {
            return _allotmentRepository.GetApplicantDetails(rID);
        }

        public DataSourceResult GetAlloteeLists(DataSourceRequest sourceReq)
        {
            // return;
            return _allotmentRepository.GetAlloteeLists(sourceReq);
        }
        public List<AllotteeListModel> AllotteeListView(int schemeId, int depid, DateTime allotmentDate)
        {
            return _allotmentRepository.AllotteeListView(schemeId, depid, allotmentDate);
        }
        public AllotteeListModel GetAllotteeListByID(int schemeId, int deptmentId, DateTime AllotmentDates)
        {
            return _allotmentRepository.GetAllotteeListByID(schemeId, deptmentId, AllotmentDates);
        }

        public List<DDList> GetAssineTo()
        {
            return _allotmentRepository.GetAssineTo();
        }

        public DataSourceResult GetRefundDetails(DataSourceRequest req, int loginUser)
        {
            return _allotmentRepository.GetRefundDetails(req, loginUser);
        }

        public bool CheckIssueDate(int schemeId, DateTime issueDate)
        {
            return _allotmentRepository.CheckIssueDate(schemeId, issueDate);
        }
        public ApplicationFormModel GetSchemeDates(int schemeId)
        {
            return _allotmentRepository.GetSchemeDates(schemeId);
        }
        public bool CheckFormNo(int schemeId, int departmentId, string formNo, int applicationId)
        {
            return _allotmentRepository.CheckFormNo(schemeId, departmentId, formNo, applicationId);
        }
        public List<DDList> GetRIDs()
        {
            return _allotmentRepository.GetRIDs();
        }
        public List<DDList> GetRIDsByDeptt()
        {
            return _allotmentRepository.GetRIDsByDeptt();
        }
        public DataSourceResult GetRIDsByDeptt(DataSourceRequest Req, int Rid)
        {
            return _allotmentRepository.GetRIDsByDeptt(Req, Rid);
        }
        public ApplicationFormModel GetPersonalInfo(int rId)
        {
            return _allotmentRepository.GetPersonalInfo(rId);
        }
        public bool UpdatePersonalInfo(ApplicationFormModel applicationFormModel)
        {
            return _allotmentRepository.UpdatePersonalInfo(applicationFormModel);
        }
        public List<ManageRequestModel> GetAllRequest()
        {
            return _allotmentRepository.GetAllRequest();
        }
        public List<ManageRequestModel> AllotteeListForApproval(int schemeId, int depid, DateTime allotmentDate)
        {
            return _allotmentRepository.AllotteeListForApproval(schemeId, depid, allotmentDate);
        }

        public List<ManageRequestModel> GetUnsuccessfullApplicantsForApproval(int schemeId, int depid)
        {
            return _allotmentRepository.GetUnsuccessfullApplicantsForApproval(schemeId, depid);
        }
        public bool SaveRequetForApproval(int schemeId, int depid, string user, DateTime allotmentDate)
        {
            return _allotmentRepository.SaveRequetForApproval(schemeId, depid, user, allotmentDate);
        }
        //public bool SaveRequetForApproval(int schemeId, int depid, string user)
        // {
        //     return _allotmentRepository.SaveRequetForApproval(schemeId, depid, user);
        // }


        public string BulkUploadApplicationForm(int? schemeId, int? departmentId, HttpPostedFileBase uploadExcel)
        {
            //return _allotmentRepository.BulkUploadApplicationForm(schemeId, departmentId, uploadExcel);
            return _allotmentRepository.UploadBulkApplicationForm(schemeId, departmentId, uploadExcel);
        }
        public bool SaveRequetForApprovalRequest(int schemeId, int depid, string comment, string status, bool isUnSuccessfullAplicants, DateTime allotmentDate)
        {
            return _allotmentRepository.SaveRequetForApprovalRequest(schemeId, depid, comment, status, isUnSuccessfullAplicants, allotmentDate);
        }
        public ManageRequestModel GetAllotteeListRequestByID(int schemeId, int deptmentId, DateTime AllotmentDates)
        {
            return _allotmentRepository.GetAllotteeListRequestByID(schemeId, deptmentId, AllotmentDates);
        }
        public List<ManageRequestModel> GetAllUnsuccessfullApplicants()
        {
            return _allotmentRepository.GetUnsuccessfullApplicants();
        }
        public ManageRequestModel GetUnsuccessfullAplicantstRequestByID(int schemeId, int deptmentId)
        {
            return _allotmentRepository.GetUnsuccessfullAplicantstRequestByID(schemeId, deptmentId);
        }
        public DataSourceResult GetUnsuccessfulApplicants(DataSourceRequest request, int scID, int depID)
        {
            return _allotmentRepository.GetUnsuccessfulApplicants(request, scID, depID);
        }

        public UnsuccessfulApplicant GetUnsuccessfulLstDetails(int scID, int depID)
        {
            return _allotmentRepository.GetUnsuccessfulLstDetails(scID, depID);
        }

        public bool UnsuccessfulListAction(int scID, int depID, int action, int loginUserID, string userVal)
        {
            return _allotmentRepository.UnsuccessfulListAction(scID, depID, action, loginUserID, userVal);
        }
        //To Get All Property in Sector/Block-Property number. 
        public List<DDList> GetAllProperty(int schemeId, int departmentId)
        {
            return _allotmentRepository.GetAllProperty(schemeId, departmentId);
        }
        public DataSourceResult GetAllProperty(DataSourceRequest Req, int? schemeId, int? departmentId)
        {
            return _allotmentRepository.GetAllProperty(Req, schemeId, departmentId);
        }
        // To Get Property Details.
        public SchemePropertyTransDetail GetPropertyDetails(int rId)
        {
            return _allotmentRepository.GetPropertyDetails(rId);
        }
        // To Get Allotment By Id for Edit.
        public AllotmentModel GetAllotmentById(int Id)
        {
            return _allotmentRepository.GetAllotmentById(Id);
        }
        // To Update Allotment. 
        public bool UpdateAllotment(AllotmentModel allotmentModel, int userId)
        {

            // CalculateAllotmentMoney(allotmentModel) is used for calculating the allotment money and update the same in model
            return _allotmentRepository.UpdateAllotment(_allotmentEngine.CalculateAllotmentMoney(allotmentModel), userId);
        }

        public ApplicationPayment GetApplicantPaymentDetails(string formno, int applicationId)
        {
            return _allotmentRepository.GetApplicantPaymentDetails(formno, applicationId);
        }

        public Stream BulkDownloadApplicationForm(int? schemeId, int? departmentId)
        {
            return _allotmentRepository.BulkDownloadApplicationForm(schemeId, departmentId);
        }

        public Stream DownloadAllotmentExcelFormat()
        {
            return _allotmentRepository.DownloadAllotmentExcelFormat();
        }

        public bool BulkUploadForAllotment(int? schemeId, int? departmentId, HttpPostedFileBase uploadExcel)
        {
            return _allotmentRepository.BulkUploadForAllotment(schemeId, departmentId, uploadExcel);
        }
        public ChallanModel PrintViewAllotment(int propertyId, int schemeID, int departmentId)
        {
            //var challanValues = _allotmentRepository.PrintViewAllotment(propertyId, schemeID, departmentId);
            //challanValues.TotalDue = _paymentEngine.GetLeaseRentDuesTillDate();
            //return challanValues;
            return _allotmentRepository.PrintViewAllotment(propertyId, schemeID, departmentId);
        }

        public decimal GetLeaseRentDuesTillDate(int rId, int DepttId)
        {
            return _paymentEngine.GetLeaseRentDuesTillDate(rId, DepttId);
        }

        public List<DDLStringList> GetEarnestMoneyBySchemeAndDeptId(int SchemeId, int DepartmentId)
        {
            return _allotmentRepository.GetEarnestMoneyBySchemeAndDeptId(SchemeId, DepartmentId);
        }


        public ApplicationFormModel GetApplicantDetails(string formno, int schemeID, int departmentID)
        {
            return _allotmentRepository.GetApplicantDetails(formno, schemeID, departmentID);
        }

        public SchemePropertyTransDetail GetPropertyDetails(int propertyId, int schemeID, int departmentID)
        {
            return _allotmentRepository.GetPropertyDetails(propertyId, schemeID, departmentID);
        }

        public List<ChallanModel> GetAllotmentDetailsForBulkPrint(List<int> propIds, int schemeId, int deptId)
        {
            return _allotmentRepository.GetAllotmentDetailsForBulkPrint(propIds, schemeId, deptId);
        }

        public List<AllotmentLetterModel> GetAllotmentDetailsForBulkLetterPrint(List<int> rIds)
        {
            return _allotmentRepository.GetAllotmentDetailsForBulkLetterPrint(rIds);
        }

        //public  int BulkPaymentSchedule(List<int> rIds)
        //{
        //    return _allotmentRepository.BulkPaymentSchedule(rIds);
        //}


        public ApplicationFormModel GetApplicationFormDetailById(int applicationId)
        {
            return _allotmentRepository.GetApplicationFormDetailById(applicationId);
        }

        public ChallanModel PrintViewAllotment(int propertyId, int schemeID, int departmentId, int rId)
        {
            return _allotmentRepository.PrintViewAllotment(propertyId, schemeID, departmentId, rId);
        }

        //To Get All form Id for compnay.
        public List<DDLStringList> GetAllCompanyForms(int SchemeId, int DepartmentId)
        {
            return _allotmentRepository.GetAllCompanyForms(SchemeId, DepartmentId);
        }

        //To fill Director Grid on Add Company => under Manage Application.
        public DataSourceResult GetDirectorDetailsByAppID(DataSourceRequest req, int applicationID)
        {
            return _allotmentRepository.GetDirectorDetailsByAppID(req, applicationID);
        }

        //To save Directors for Add company
        public bool SaveDiretors(decimal directorShare, string directorName, int type, int applicationID)
        {
            return _allotmentRepository.SaveDiretors(directorShare, directorName, type, applicationID);
        }

        // To Add Company Details
        public bool AddCompanyDetails(string data, string NewFirmName, string NewFirmProduct, int NewFirmStatus, int appId)
        {
            return _allotmentRepository.AddCompanyDetails(data, NewFirmName, NewFirmProduct, NewFirmStatus, appId);
        }

        // To get information from CIC.
        public CICModel GetApplicantInfo(string formno, int SchemeId, int departmentID)
        {
            return _allotmentRepository.GetApplicantInfo(formno, SchemeId, departmentID);
        }
        public string GetBulkAllotmnetLetterPrint(List<int> rIds)
        {
            return _allotmentRepository.GetBulkAllotmnetLetterPrint(rIds);
        }
        public string GetBulkAllotmnetPaymentLetterPrint(List<int> rIds)
        {
            return _allotmentRepository.GetBulkAllotmnetPaymentLetterPrint(rIds);
        }

        //To Cancel Allotment request
        public bool CancelAllotment(int rid)
        {
            return _allotmentRepository.CancelAllotment(rid);
        }

        //To generate letter
        public string Generateletter(int rid)
        {
            return _allotmentRepository.Generateletter(rid);
        }

        public List<SchemeAllotmentModel> SchemeListForAllotment()
        {
            return _allotmentRepository.SchemeListForAllotment();
        }

        //To Get All Applicants form Id for Allotment process excluding alloted form number.
        public List<DDLStringList> GetAllFormsForAllotment(int SchemeId, int DepartmentId)
        {
            return _allotmentRepository.GetAllFormsForAllotment(SchemeId, DepartmentId);
        }

        public DataSourceResult GetAllFormsForAllotment(DataSourceRequest Req, int? SchemeId, int? DepartmentId)
        {
            return _allotmentRepository.GetAllFormsForAllotment(Req, SchemeId, DepartmentId);
        }

        public bool CheckUTROrDDNumber(string number, string type, int appId)
        {
            return _allotmentRepository.CheckUTROrDDNumber(number, type, appId);
        }

        public bool CheckAllotmentDate(int schemeId, int departmentId, DateTime allotmentDate)
        {
            return _allotmentRepository.CheckAllotmentDate(schemeId, departmentId, allotmentDate);
        }

        //To Get All Applicants form Id for Allotment process.
        public List<DDLStringList> GetAllFormsForView(int SchemeId, int DepartmentId)
        {
            return _allotmentRepository.GetAllFormsForView(SchemeId, DepartmentId);
        }

        //Online application download and payment
        public OnlineApplicationFormModel OnlineApplicationRequest(OnlineApplicationFormModel applicationFormModel)
        {
            return _allotmentRepository.OnlineApplicationRequest(applicationFormModel);
        }

        public OnlineApplicationFormModel ViewOnlineDetails(int onlineappId)
        {
            return _allotmentRepository.ViewOnlineDetails(onlineappId);
        }

        //Edit Online application Basic info
        public OnlineApplicationFormModel EditOnlineDetails(OnlineApplicationFormModel applicationFormModel)
        {
            return _allotmentRepository.EditOnlineDetails(applicationFormModel);
        }

        public List<SchemeAllotmentModel> GetSchemeListForAllotment()
        {
            return _allotmentRepository.GetSchemeListForAllotment();
        }

        public List<DepartmentAllotmentModel> FilterDepartmentOnScheme(int SchemeId)
        {
            return _allotmentRepository.FilterDepartmentOnScheme(SchemeId);
        }

        public List<DDList> GetAllBanksforOnline()
        {
            return _allotmentRepository.GetAllBanksforOnline();
        }

        public List<ChecklistDocuments> GetApplicationChcklstDocuments(DataSourceRequest request, int applicationId)
        {
            return _allotmentRepository.GetApplicationChcklstDocuments(request, applicationId);
        }

        public bool SaveFileDetailsToDB(UploadDetails details)
        {
            return _allotmentRepository.SaveFileDetailsToDB(details);
        }

        public OnlineApplicationFormModel GetAreaDetails(int id)
        {
            return _allotmentRepository.GetAreaDetails(id);
        }

        public OnlineApplicationFormModel GetEarneshMoney(int id, int deptt, int floor)
        {
            return _allotmentRepository.GetEarneshMoney(id, deptt, floor);
        }

        public List<DDList> GetFloors(int schemeId, int depttID)
        {
            return _allotmentRepository.GetFloors(schemeId, depttID);
        }

        public OnlineApplicationDetailsTrans SaveTrasOnlineDetails(int id)
        {
            return _allotmentRepository.SaveTrasOnlineDetails(id);
        }

        public OnlineApplicationDetailsTrans GetTrasactionDetails(string id, string paymenttype)
        {
            return _allotmentRepository.GetTrasactionDetails(id, paymenttype);
        }

        public OnlineApplicationDetailsTrans UpdateTrasactionDetails(FormCollection form)
        {
            return _allotmentRepository.UpdateTrasactionDetails(form);
        }

        public DataSourceResult GetOnlineApplications(DataSourceRequest req)
        {
            return _allotmentRepository.GetOnlineApplications(req);
        }

        public bool SaveCommentByApplicationID(int requestNo, string Comment, bool acceptReject)
        {
            return _allotmentRepository.SaveCommentByApplicationID(requestNo, Comment, acceptReject);
        }

        public bool SaveCommentByAppID(int requestNo, string Comment, bool acceptReject, string MobNo, string EmailId)
        {
            return _allotmentRepository.SaveCommentByAppID(requestNo, Comment, acceptReject, MobNo, EmailId);
        }

        public ApplicatandDetailsModel GetDetails(int applid)
        {
            return _allotmentRepository.GetDetails(applid);
        }

        public OnlineApplicationDetailsTrans SaveOtherPaymentTras(int id)
        {
            return _allotmentRepository.SaveOtherPaymentTras(id);
        }

        public DataSourceResult GetRIDsForApplication(DataSourceRequest Req)
        {
            return _allotmentRepository.GetRIDsForApplication(Req);
        }

        public bool UpdateAddress(UpdateAddress objUpdateAddress)
        {
            return _allotmentRepository.UpdateAddress(objUpdateAddress);
        }

        public UpdateAddress GetApplicationDetailsById(int rId)
        {
            return _allotmentRepository.GetApplicationDetailsById(rId);
        }

        public UpdateDetails GetFormDetailsById(int rId)
        {
            return _allotmentRepository.GetFormDetailsById(rId);
        }

        public bool UpdateFormDetails(UpdateDetails ObjUpdateDetails)
        {
            return _allotmentRepository.UpdateFormDetails(ObjUpdateDetails);
        }

        public List<ApplicationDetail> GetDetailsOfAllotedProprety(int schemeId, int depid, DateTime allotmentDate)
        {
            return _allotmentRepository.GetDetailsOfAllotedProprety(schemeId, depid, allotmentDate);
        }


        public int PropertyAllotmentForOnlineApplicationForm(OnlineFormViewModel model)
        {
            return _allotmentRepository.PropertyAllotmentForOnlineApplicationForm(model);
        }


        public DataSourceResult GetApplicationFormAsDataSourceByApplicationId(DataSourceRequest request, int? applicationId)
        {
            return _allotmentRepository.GetApplicationFormAsDataSourceByApplicationId(request, applicationId);
        }

        public DataSourceResult GetPropertyDetailAsDataSourceByPropertyId(DataSourceRequest request, int? propertyId)
        {
            return _allotmentRepository.GetPropertyDetailAsDataSourceByPropertyId(request, propertyId);
        }

        public DataSourceResult GetAllottedPropertyFormListById(DataSourceRequest request, AllotmentViewModel model)
        {
            return _allotmentRepository.GetAllottedPropertyFormListById(request, model);
        }

        public int ValidatePropertyAndApplicationForm(FormViewModel model)
        {
            return _allotmentRepository.ValidatePropertyAndApplicationForm(model);
        }

        public int PropertyAllotmentForApplicationForm(FormViewModel model)
        {
            return _allotmentRepository.PropertyAllotmentForApplicationForm(model);
        }


        public DataSourceResult GetAllottedPropertyFormListForApproval(DataSourceRequest request, AllotmentViewModel model)
        {
            return _allotmentRepository.GetAllottedPropertyFormListForApproval(request, model);
        }


        public int UpdateAllottedPropertyStatus(AllotmentViewModel model)
        {
            return _allotmentRepository.UpdateAllottedPropertyStatus(model);
        }

        public string ViewletterTemplate(LetterHistory _LetterHistory)
        {
            return _allotmentRepository.ViewletterTemplate(_LetterHistory);
        }


        public DataSourceResult GetApplicationFormList(DataSourceRequest request, FormViewModel model)
        {
            return _allotmentRepository.GetApplicationFormList(request, model);
        }


        public PropertyViewModel GetApplicantDetailsToTransferProperty(PropertyViewModel model)
        {
            return _allotmentRepository.GetApplicantDetailsToTransferProperty(model);
        }


        public int SavePropertyTransferDetail(TransferViewModel model)
        {
            return _allotmentRepository.SavePropertyTransferDetail(model);
        }

        public TransferViewModel GetTransferedPropertyDetailById(TransferViewModel model)
        {
            return _allotmentRepository.GetTransferedPropertyDetailById(model);
        }


        public DataSourceResult GetAllotmentRequestByUserIdAsDataSource(DataSourceRequest request, AllotmentViewModel model)
        {
            return _allotmentRepository.GetAllotmentRequestByUserIdAsDataSource(request, model);
        }


        public DataSourceResult GetAllottedPropertyListByYearAsDataSource(DataSourceRequest request, AllotmentViewModel model)
        {
            return _allotmentRepository.GetAllottedPropertyListByYearAsDataSource(request, model);
        }
    }
}
