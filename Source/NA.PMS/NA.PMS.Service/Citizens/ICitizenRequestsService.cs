using Kendo.Mvc.UI;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Service
{
    public interface ICitizenRequestsService
    {
        DataSourceResult GetCitizenRequests(DataSourceRequest req);
        RequestDetails GetRequestDetailsById(int id);
        List<PropertyDocument> GetDocumentDetails(DataSourceRequest req, int rid, int id);
        bool SaveRequestDetails(int id, decimal? serviceFee, decimal? duesAmnt, string comment, int temp, string dispatchNo, DateTime? dispatchDate);
        List<DDList> GetAllServices();
        ChallanModel GenerateChallanByServices(int rId, int bankId, int branchId, string DdlAccountNumber, int? deptId, int? serviceId);
        List<ServiceRequestDocument> GetCheckListDocumentMentsByServiceId_DepartmentId(int ServiceRequestNo);
        CitizenServiceRequest GetServiceRequestDetails(int id);
        void SaveFileDetails(string filePath, string fileName, int id);
        //To Save Service Request
        int SaveServiceRequest(int rID, int department, int serviceType, string description);
        DataSourceResult GetTransferServiceReq(DataSourceRequest Req, int RID);
        DataSourceResult GetRentServiceReq(DataSourceRequest Req, int RID);
        DataSourceResult GetCICServiceReq(DataSourceRequest Req, int RID);
        List<DDList> GetServiceRequestStatus();
        DataSourceResult GetMortgageServiceReq(DataSourceRequest Req, int RID);
        DataSourceResult GetCustomerServiceReport(DataSourceRequest request);
        DataSourceResult GetExtensionServiceReq(DataSourceRequest Req, int RID);
    }
}
