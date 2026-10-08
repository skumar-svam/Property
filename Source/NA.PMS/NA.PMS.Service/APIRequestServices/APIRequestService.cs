using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Repository.ApiRequest;
using Kendo.Mvc.UI;
using NA.PMS.Model;

namespace NA.PMS.Service.APIRequestServices
{
    public class ApiRequestService : IApiRequestService
    {
        private  IApiRequestRepository _apiRequestRepository = new ApiRequestRepository();

        /// <summary>
        ///  Save Service Request
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <returns></returns>
        public bool SaveServiceRequest(Model.Customer_ServiceRequest serviceRequest)
        {
            return _apiRequestRepository.SaveServiceRequest(serviceRequest);
        }


        /// <summary>
        /// Get Service Request Status by serviceId
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public int GetServiceRequestStatus(int serviceId)
        {
            return _apiRequestRepository.GetServiceRequestStatus(serviceId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <returns></returns>
        public bool UpdateServiceRequestStatus(int serId, string rType)
        {
            return _apiRequestRepository.UpdateServiceRequestStatus(serId, rType);
        }

        /// <summary>
        /// Get All Service Requests
        /// </summary>
        /// <returns></returns>
        public List<ServiceRequestModel> GetAllServiceRequests()
        {
            return _apiRequestRepository.GetAllServiceRequests();
        }


        public DataSourceResult GetCustomerServiceRequestList(DataSourceRequest request)
        {
            return _apiRequestRepository.GetCustomerServiceRequestList(request);
        }

        public DataSourceResult GetCustomerServiceRequestDetailByRequestId(int? requestId)
        {
            return _apiRequestRepository.GetCustomerServiceRequestDetailByRequestId(requestId);
        }


        public DataSourceResult GetLetterHistoryDetails(DataSourceRequest request)
        {
            return _apiRequestRepository.GetLetterHistoryDetails(request);
        }


        public LetterViewModel GetLetterByBarcode(string barcode)
        {
            return _apiRequestRepository.GetLetterByBarcode(barcode);
        }
    }
}
