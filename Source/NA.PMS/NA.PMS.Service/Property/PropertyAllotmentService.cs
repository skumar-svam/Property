using Kendo.Mvc.UI;

using NA.PMS.Model;
using NA.PMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Service
{
    public class PropertyAllotmentService : IPropertyAllotmentService
    {
        IPropertyAllotmentRepository _propertyRepository;
        public PropertyAllotmentService(IPropertyAllotmentRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }
        public PropertyAllotmentService()
        {
            _propertyRepository = new PropertyAllotmentRepository();
        }

        public List<PropertyAllotmentModel> GetPropertyAllotmentList()
        {
            return _propertyRepository.GetPropertyAllotmentList();
        }


        public List<SchemeAllotmentModel> FilterSchemeListOnDepartment(int departmentId)
        {
            return _propertyRepository.FilterSchemeListOnDepartment(departmentId);
        }

        public List<DepartmentAllotmentModel> GetDepartmentListForAllotment()
        {
            return _propertyRepository.GetDepartmentListForAllotment();
        }


        public List<SchemeAllotmentModel> GetSchemeListForAllotment()
        {
            return _propertyRepository.GetSchemeListForAllotment();
        }

        public List<DepartmentAllotmentModel> FilterDepartmentOnScheme(int schemeId)
        {
            return _propertyRepository.FilterDepartmentOnScheme(schemeId);
        }


        public AllottedPropertyDetails GetScheduleDetailsForAllottedProperty(int rid)
        {
            return _propertyRepository.GetScheduleDetailsForAllottedProperty(rid);
        }

        public DetailedPropertyView GetDetailedPropertyView(int rid)
        {
            return _propertyRepository.GetDetailedPropertyView(rid);
        }

        public DataSourceResult GetPaymentSchedule(DataSourceRequest req, int rid)
        {
            return _propertyRepository.GetPaymentSchedule(req, rid);
        }

        public DataSourceResult GetPaymentReschedule(DataSourceRequest request, int rId)
        {
            return _propertyRepository.GetPaymentReschedule(request, rId);
        }

        public DataSourceResult GetReceiptSchedule(DataSourceRequest req, int rid)
        {
            return _propertyRepository.GetReceiptSchedule(req, rid);
        }

        public List<DDList> GetAllDocumentType()
        {
            return _propertyRepository.GetAllDocumentType();
        }
        public bool ReschedulePayments(int rId, decimal dueAmnt)
        {
            return _propertyRepository.ReschedulePayments(rId, dueAmnt);
        }
        //To GetPayment Ledger
        public DataSourceResult GetPaymentLedger(DataSourceRequest request, int rId)
        {
            return _propertyRepository.GetPaymentLedger(request, rId);
        }

        //To Get Letter History
        public DataSourceResult GetLetterHistory(DataSourceRequest req, int rid)
        {
            return _propertyRepository.GetLetterHistory(req, rid);
        }

        // To Save Extension
        public bool SaveExtension(int rid, int OnlineRequestRefNo, string propertyNu, DateTime completionDueDate, DateTime extensionGivenDate, decimal extensionCharge, string user)
        {
            return _propertyRepository.SaveExtension(rid, OnlineRequestRefNo, propertyNu, completionDueDate, extensionGivenDate, extensionCharge, user);
        }

        // To Get the details of property.
        public ExtensionDetails GetPropertyDetails(int rid)
        {
            return _propertyRepository.GetPropertyDetails(rid);
        }

        // To Fill grid.
        public DataSourceResult GetExtensionDetails(DataSourceRequest request)
        {
            return _propertyRepository.GetExtensionDetails(request);
        }

        public DataSourceResult GetExtensionDetailsByRid(DataSourceRequest request, int Rid)
        {
            return _propertyRepository.GetExtensionDetailsByRid(request, Rid);
        }

        public ExtensionDetails GetExtensionById(int id)
        {
            return _propertyRepository.GetExtensionById(id);
        }

        public DataSourceResult GetExtensionApprovalDetails(DataSourceRequest request)
        {
            return _propertyRepository.GetExtensionApprovalDetails(request);
        }

        // To Save Comment by User at the time of approval
        public bool SaveCommentByID(int Id, string Comment, bool acceptReject)
        {
            return _propertyRepository.SaveCommentByID(Id, Comment, acceptReject);
        }

        // To Re-Sumbit for Extension or update
        public bool UpdateExtension(int Id, int rid, string propertyNu, DateTime completionDueDate, DateTime extensionGivenDate, decimal extensionCharge, string user)
        {
            return _propertyRepository.UpdateExtension(Id, rid, propertyNu, completionDueDate, extensionGivenDate, extensionCharge, user);
        }

        //To Cancel Extension Request
        public bool CancelExtension(int Id)
        {
            return _propertyRepository.CancelExtension(Id);
        }

        //To Generate Extension Letter
        public string GenerateExtensionLetter(int rid)
        {
            return _propertyRepository.GenerateExtensionLetter(rid);
        }


        public List<DynamicDataModel> GetRegistrationIdForAdvanceSearch()
        {
            return _propertyRepository.GetRegistrationIdForAdvanceSearch();
        }

        public List<DynamicDataModel> GetDepartmentForAdvanceSearch()
        {
            return _propertyRepository.GetDepartmentForAdvanceSearch();
        }

        public List<DynamicDataModel> GetSectorsForAdvanceSearch()
        {
            return _propertyRepository.GetSectorsForAdvanceSearch();
        }

        public List<DynamicDataModel> GetBlocksForAdvanceSearch()
        {
            return _propertyRepository.GetBlocksForAdvanceSearch();
        }

        public List<AllotmentModel> AdvanceSearchForAllottedProperty(int? rid, int? department, string sector, string block, string plot, string mobileNumber, string name, string fatherName, string motherName, string address)
        {
            return _propertyRepository.AdvanceSearchForAllottedProperty(rid, department, sector, block, plot, mobileNumber, name, fatherName, motherName, address);
        }

        public DataSourceResult AdvanceSearchForAllottedProperty(DataSourceRequest Req, AdvanceSearchModel objAdvanceSearchModel)
        {
            return _propertyRepository.AdvanceSearchForAllottedProperty(Req, objAdvanceSearchModel);
        }
        // Get All RID from possessiondetails
        public DataSourceResult GetAllRIDs(DataSourceRequest Req, int Rid)
        {
            return _propertyRepository.GetAllRIDs(Req, Rid);
        }


        public System.IO.Stream DownloadExcelApplicationForm(int? schemeId, int? departmentId, string formType)
        {
            return _propertyRepository.DownloadExcelApplicationForm(schemeId, departmentId, formType);
        }


        public string UploadExcelApplicationForm(int schemeId, int departmentId, System.Web.HttpPostedFileBase uploadExcel)
        {
            return _propertyRepository.UploadExcelApplicationForm(schemeId, departmentId, uploadExcel);
        }

        public bool AddRemarksForProperty(RemarksDetailsModel ObjRemarks)
        {
            return _propertyRepository.AddRemarksForProperty(ObjRemarks);
        }

        public DataSourceResult GetSubLeasePropertyList(DataSourceRequest request, int Rid)
        {
            return _propertyRepository.GetSubLeasePropertyList(request, Rid);
        }


        public DataSourceResult GetPropertyDetailListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            return _propertyRepository.GetPropertyDetailListAsDataSource(request, model);
        }


        public DataSourceResult AdvanceSearchForAllottedPropertyII(DataSourceRequest request, AdvanceSearchModel model)
        {
            return _propertyRepository.AdvanceSearchForAllottedPropertyII(request, model);
        }


        public AllottedPropertyViewModel GetPropertyDetailByRegistrationId(int rid)
        {
            return _propertyRepository.GetPropertyDetailByRegistrationId(rid);
        }


        public DataSourceResult GetDocumentListByRegistrationId(DataSourceRequest request, int rid)
        {
            return _propertyRepository.GetDocumentListByRegistrationId(request, rid);
        }


        public DataSourceResult GetGeneratedLetterByRegistrationId(DataSourceRequest request, int rid)
        {
            return _propertyRepository.GetGeneratedLetterByRegistrationId(request, rid);
        }


        public DataSourceResult GetServiceRequestListByRegistrationId(DataSourceRequest request, ServiceViewModel model)
        {
            return _propertyRepository.GetServiceRequestListByRegistrationId(request, model);
        }


        public DataSourceResult GetTransferHistoryByIdAsDataSource(DataSourceRequest request, TransferViewModel model)
        {
            return _propertyRepository.GetTransferHistoryByIdAsDataSource(request, model);
        }

        public DataSourceResult GetMortgageHistoryByIdAsDataSource(DataSourceRequest request, MortgageViewModel model)
        {
            return _propertyRepository.GetMortgageHistoryByIdAsDataSource(request, model);
        }

        public DataSourceResult GetExtensionHistoryByIdAsDataSource(DataSourceRequest request, ExtensionViewModel model)
        {
            return _propertyRepository.GetExtensionHistoryByIdAsDataSource(request, model);
        }

        public DataSourceResult GetRentingHistoryByIdAsDataSource(DataSourceRequest request, RentingViewModel model)
        {
            return _propertyRepository.GetRentingHistoryByIdAsDataSource(request, model);
        }


        public DataSourceResult GetCICHistoryByIdAsDataSource(DataSourceRequest request, CICViewModel model)
        {
            return _propertyRepository.GetCICHistoryByIdAsDataSource(request, model);
        }

        public DataSourceResult GetFunctionalHistoryByIdAsDataSource(DataSourceRequest request, FunctionalViewModel model)
        {
            return _propertyRepository.GetFunctionalHistoryByIdAsDataSource(request, model);
        }


        public DataSourceResult GetLeaseRentPaymentByIdAsDataSource(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _propertyRepository.GetLeaseRentPaymentByIdAsDataSource(request, model);
        }

        public DataSourceResult GetPremiumDuesPaymentByIdAsDataSource(DataSourceRequest request, LeaseRentViewModel model)
        {
            return _propertyRepository.GetPremiumDuesPaymentByIdAsDataSource(request, model);
        }

        public LeaseRentViewModel GetDuesPaymentStatusByRegistrationId(LeaseRentViewModel model)
        {
            return _propertyRepository.GetDuesPaymentStatusByRegistrationId(model);
        }


        public DataSourceResult GetNotingFilesByIdAsDataSource(DataSourceRequest request, NotingViewModel model)
        {
            return _propertyRepository.GetNotingFilesByIdAsDataSource(request, model);
        }


        public DataSourceResult GetSubLeasedPropertyByIdAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            return _propertyRepository.GetSubLeasedPropertyByIdAsDataSource(request, model);
        }


        public int UpdateDocumentStatusOfProperty(PropertyViewModel model)
        {
            return _propertyRepository.UpdateDocumentStatusOfProperty(model);
        }


        public DataSourceResult GetDuesCalcaluationHistoryAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            return _propertyRepository.GetDuesCalcaluationHistoryAsDataSource(request, model);
        }


        public DataSourceResult GetPropertyListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            return _propertyRepository.GetPropertyListAsDataSource(request, model);
        }


        public KYAViewModel GetKYADetails(int rId)
        {
            return _propertyRepository.GetKYADetails(rId);
        }


        public DataSourceResult GetDocumentTypeListByDepartmentId(DataSourceRequest request, int deptId)
        {
            return _propertyRepository.GetDocumentTypeListByDepartmentId(request, deptId);
        }


        public int SaveUploadedDocument(PropertyDocument model, System.Web.HttpPostedFileBase docfile)
        {
            return _propertyRepository.SaveUploadedDocument(model, docfile);
        }

        public bool UpdatePaymentScheduleStatus(int ScheduleId, int Rid)
        {
            return _propertyRepository.UpdatePaymentScheduleStatus(ScheduleId, Rid);
        }
    }
}
