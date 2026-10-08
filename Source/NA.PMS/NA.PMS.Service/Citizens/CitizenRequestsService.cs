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
    public class CitizenRequestsService : ICitizenRequestsService
    {
        ICitizenRequestsRepository _citizenRequestsRepo;

        public CitizenRequestsService()
        {
            _citizenRequestsRepo = new CitizenReqeustsRepository();
        }

        public DataSourceResult GetCitizenRequests(DataSourceRequest req)
        {
            return _citizenRequestsRepo.GetCitizenRequests(req);
        }

        public RequestDetails GetRequestDetailsById(int id)
        {
            return _citizenRequestsRepo.GetRequestDetailsById(id);
        }

        public List<PropertyDocument> GetDocumentDetails(DataSourceRequest req, int rid, int id)
        {
            return _citizenRequestsRepo.GetDocumentDetails(req, rid, id);
        }

        public bool SaveRequestDetails(int id, decimal? serviceFee, decimal? duesAmnt, string comment, int temp, string dispatchNo, DateTime? dispatchDate)
        {
            return _citizenRequestsRepo.SaveRequestDetails(id, serviceFee, duesAmnt, comment, temp, dispatchNo, dispatchDate);
        }

        public void SaveFileDetails(string filePath, string fileName, int id)
        {
            _citizenRequestsRepo.SaveFileDetails(filePath, fileName, id);
        }

        public List<DDList> GetAllServices()
        {
            return _citizenRequestsRepo.GetAllServices();
        }

        public ChallanModel GenerateChallanByServices(int rId, int bankId, int branchId, string DdlAccountNumber, int? deptId, int? serviceId)
        {
            return _citizenRequestsRepo.GenerateChallanByServices(rId, bankId, branchId, DdlAccountNumber, deptId, serviceId);
        }

        public List<ServiceRequestDocument> GetCheckListDocumentMentsByServiceId_DepartmentId(int ServiceRequestNo)
        {
            return _citizenRequestsRepo.GetCheckListDocumentMentsByServiceId_DepartmentId(ServiceRequestNo);
        }

        public CitizenServiceRequest GetServiceRequestDetails(int id)
        {
            return _citizenRequestsRepo.GetServiceRequestDetails(id);
        }

        //To Save Service Request
        public int SaveServiceRequest(int rID, int department, int serviceType, string description)
        {
            return _citizenRequestsRepo.SaveServiceRequest(rID, department, serviceType, description);
        }

        public DataSourceResult GetTransferServiceReq(DataSourceRequest Req, int RID)
        {
            return _citizenRequestsRepo.GetTransferServiceReq(Req, RID);
        }

        public DataSourceResult GetRentServiceReq(DataSourceRequest Req, int RID)
        {
            return _citizenRequestsRepo.GetRentServiceReq(Req, RID);
        }

        public List<DDList> GetServiceRequestStatus()
        {
            return _citizenRequestsRepo.GetServiceRequestStatus();
        }

        public DataSourceResult GetCICServiceReq(DataSourceRequest Req, int RID)
        {
            return _citizenRequestsRepo.GetCICServiceReq(Req, RID);
        }

        public DataSourceResult GetCustomerServiceReport(DataSourceRequest request)
        {
            return _citizenRequestsRepo.GetCustomerServiceReport(request);
        }

        public DataSourceResult GetMortgageServiceReq(DataSourceRequest Req, int RID)
        {
            return _citizenRequestsRepo.GetMortgageServiceReq(Req, RID);
        }

        public DataSourceResult GetExtensionServiceReq(DataSourceRequest Req, int RID)
        {
            return _citizenRequestsRepo.GetExtensionServiceReq(Req, RID);
        }
    }
}
