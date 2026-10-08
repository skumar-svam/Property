using Kendo.Mvc.UI;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using System.IO;

namespace NA.PMS.Service
{
    public interface IPropertyAllotmentService
    {
        List<PropertyAllotmentModel> GetPropertyAllotmentList();

        List<SchemeAllotmentModel> FilterSchemeListOnDepartment(int departmentId);

        List<DepartmentAllotmentModel> GetDepartmentListForAllotment();

        List<SchemeAllotmentModel> GetSchemeListForAllotment();

        List<DepartmentAllotmentModel> FilterDepartmentOnScheme(int schemeId);

        DetailedPropertyView GetDetailedPropertyView(int rid);

        DataSourceResult GetPaymentSchedule(DataSourceRequest req, int rid);
        DataSourceResult GetPaymentReschedule(DataSourceRequest request, int rId);
        DataSourceResult GetReceiptSchedule(DataSourceRequest req, int rid);

        AllottedPropertyDetails GetScheduleDetailsForAllottedProperty(int rid);

        List<DDList> GetAllDocumentType();

        DataSourceResult GetPaymentLedger(DataSourceRequest req, int rid);

        DataSourceResult GetLetterHistory(DataSourceRequest req, int rid);
        bool ReschedulePayments(int rId, decimal dueAmnt);
        // To Save Extension
        bool SaveExtension(int rid, int OnlineRequestRefNo, string propertyNu, DateTime completionDueDate, DateTime extensionGivenDate, decimal extensionCharge, string user);

        // To Get the details of property.
        ExtensionDetails GetPropertyDetails(int rid);

        // To Fill grid.
        DataSourceResult GetExtensionDetails(DataSourceRequest request);
        DataSourceResult GetExtensionDetailsByRid(DataSourceRequest request, int Rid);
        //To View Details by rid.
        ExtensionDetails GetExtensionById(int id);

        // To Fill Approval grid.
        DataSourceResult GetExtensionApprovalDetails(DataSourceRequest request);

        // To Save Comment by User at the time of approval
        bool SaveCommentByID(int Id, string Comment, bool acceptReject);

        // To Re-Sumbit for Extension or update
        bool UpdateExtension(int Id, int rid, string propertyNu, DateTime completionDueDate, DateTime extensionGivenDate, decimal extensionCharge, string user);

        //To Cancel Extension Request
        bool CancelExtension(int Id);

        //To Generate Extension Letter
        string GenerateExtensionLetter(int rid);

        List<DynamicDataModel> GetRegistrationIdForAdvanceSearch();

        List<DynamicDataModel> GetDepartmentForAdvanceSearch();

        List<DynamicDataModel> GetSectorsForAdvanceSearch();

        List<DynamicDataModel> GetBlocksForAdvanceSearch();

        List<AllotmentModel> AdvanceSearchForAllottedProperty(int? rid, int? department, string sector, string block, string plot, string mobileNumber, string name, string fatherName, string motherName, string address);
        DataSourceResult AdvanceSearchForAllottedProperty(DataSourceRequest Req, AdvanceSearchModel objAdvanceSearchModel);

        // Get All RID from possessiondetails
        DataSourceResult GetAllRIDs(DataSourceRequest Req, int Rid);

        Stream DownloadExcelApplicationForm(int? schemeId, int? departmentId, string formType);

        string UploadExcelApplicationForm(int schemeId, int departmentId, System.Web.HttpPostedFileBase uploadExcel);
        bool AddRemarksForProperty(RemarksDetailsModel ObjRemarks);
        DataSourceResult GetSubLeasePropertyList(DataSourceRequest request, int Rid);

        DataSourceResult GetPropertyDetailListAsDataSource(DataSourceRequest request, PropertyViewModel model);

        DataSourceResult AdvanceSearchForAllottedPropertyII(DataSourceRequest request, AdvanceSearchModel model);

        AllottedPropertyViewModel GetPropertyDetailByRegistrationId(int rid);

        DataSourceResult GetDocumentListByRegistrationId(DataSourceRequest request, int rid);

        DataSourceResult GetGeneratedLetterByRegistrationId(DataSourceRequest request, int rid);

        DataSourceResult GetServiceRequestListByRegistrationId(DataSourceRequest request, ServiceViewModel model);

        DataSourceResult GetTransferHistoryByIdAsDataSource(DataSourceRequest request, TransferViewModel model);

        DataSourceResult GetMortgageHistoryByIdAsDataSource(DataSourceRequest request, MortgageViewModel model);

        DataSourceResult GetExtensionHistoryByIdAsDataSource(DataSourceRequest request, ExtensionViewModel model);

        DataSourceResult GetRentingHistoryByIdAsDataSource(DataSourceRequest request, RentingViewModel model);

        DataSourceResult GetCICHistoryByIdAsDataSource(DataSourceRequest request, CICViewModel model);

        DataSourceResult GetFunctionalHistoryByIdAsDataSource(DataSourceRequest request, FunctionalViewModel model);

        DataSourceResult GetLeaseRentPaymentByIdAsDataSource(DataSourceRequest request, LeaseRentViewModel model);

        DataSourceResult GetPremiumDuesPaymentByIdAsDataSource(DataSourceRequest request, LeaseRentViewModel model);

        LeaseRentViewModel GetDuesPaymentStatusByRegistrationId(LeaseRentViewModel model);

        DataSourceResult GetNotingFilesByIdAsDataSource(DataSourceRequest request, NotingViewModel model);

        DataSourceResult GetSubLeasedPropertyByIdAsDataSource(DataSourceRequest request, PropertyViewModel model);

        int UpdateDocumentStatusOfProperty(PropertyViewModel model);

        DataSourceResult GetDuesCalcaluationHistoryAsDataSource(DataSourceRequest request, PropertyViewModel model);

        DataSourceResult GetPropertyListAsDataSource(DataSourceRequest request, PropertyViewModel model);

        KYAViewModel GetKYADetails(int rId);

        DataSourceResult GetDocumentTypeListByDepartmentId(DataSourceRequest request, int deptId);

        int SaveUploadedDocument(PropertyDocument model, System.Web.HttpPostedFileBase docfile);

        bool UpdatePaymentScheduleStatus(int ScheduleId, int Rid);
    }
}
