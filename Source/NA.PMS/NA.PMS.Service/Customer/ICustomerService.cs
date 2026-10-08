using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Service
{
    public interface ICustomerService
    {
        DataSourceResult GetPaymentReceiptScheduleListById(DataSourceRequest request, PaymentViewModel model);

        DataSourceResult GetPaymentScheduleDataListById(DataSourceRequest request, int? rid);

        DataSourceResult GetPaymentRescheduledListById(DataSourceRequest request, int? rid);

        DataSourceResult GetPaymentLedgerDataListById(DataSourceRequest request, int? rid);

        DataSourceResult GetServiceHistoryDataListById(DataSourceRequest request, ServiceViewModel model);

        DataSourceResult GetTransferHistoryDataListById(DataSourceRequest request, int? rid);

        DataSourceResult GetMortgageHistoryDataListById(DataSourceRequest request, int? rid);

        DataSourceResult GetExtensionHistoryDataListById(DataSourceRequest request, int? rid);

        string GetFileUploadHtmlForService(int? departmentId, int? serviceId);

        int RegisterCustomerDetails(NACustomer customer, IEnumerable<HttpPostedFileBase> files);

        PropertyDetail GetPropertyDetails(DtoPropertyFilter objPropertyFilter, int inumber);

        IEnumerable<DtoLegalHistory> GetLegalHistoryByRegistrationId(DtoPropertyFilter objPropertyFilter);

        IEnumerable<DtoJalDetailsPaymentHistory> GetJalDetailsPaymentHistoryByRegistrationId(DtoPropertyFilter objPropertyFilter);

        PropertyDetailViewModel GetPropertyDetailByRegistrationId(int rid);

        ServiceRequestViewModel GetPropertyDetailForServiceRequestById(int rid);

        ServiceRequestViewModel GetServiceRequestDetailById(int? id);

        ServiceRequestViewModel SaveServiceRequestDetail(ServiceRequestViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files);

        DataSourceResult GetDirectorShareholderDataList(DataSourceRequest request);

        int SaveDirectorOrShareholders(string directorName, decimal? share, string shareType);

        int RemoveDirectorShareholderFromList(int id);

        UserViewModel GetLoginUserDetails(string userName);

        bool LockUser(string userName);

        bool ValidateUser(string userName, string password);

        bool ChangePassword(string userName, string email, string newPassword);

        bool ChangePassword(string email, string newPassword);

        Role GetRoleForUser(string userName);

        IList<DtoList> LookupPropertyType();

        UserViewModel GetCustomerDetailById(int id);

        DataSourceResult GetJalPaymentDataList(DataSourceRequest request, JalViewModel model);

        DataSourceResult GetLitigationDataList(DataSourceRequest request, LitigationViewModel model);

        CustomerDetailViewModel GetCustomerDetails(int proid);

        DataSourceResult GetScannedDocumentListById(DataSourceRequest request, DocumentViewModel model);

        DataSourceResult GetGeneratedDocumentListById(DataSourceRequest request, DocumentViewModel model);

        DataSourceResult GetRegistrationIdListAsDataSource(DataSourceRequest request);

        CustomerViewModel ValidateCustomerRegistration(CustomerViewModel model);

        KYAViewModel GetPropertyDetailForKYA(KYAViewModel model);

        int SavePropertyDetailForKYA(KYAViewModel model, IEnumerable<HttpPostedFileBase> files);

        int SavePropertyDetailForKYAII(KYAViewModel model, HttpPostedFileBase idfile, HttpPostedFileBase letterfile, HttpPostedFileBase otherfile);
    }
}
