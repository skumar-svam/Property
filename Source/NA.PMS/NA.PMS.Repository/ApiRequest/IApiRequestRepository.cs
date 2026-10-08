using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Model;
using Kendo.Mvc.UI;
using NA.PMS.Model.Entities;

namespace NA.PMS.Repository.ApiRequest
{
   public interface IApiRequestRepository
   {

       bool SaveServiceRequest(Customer_ServiceRequest serviceRequest);

       int GetServiceRequestStatus(int serviceId);

       List<ServiceRequestModel> GetAllServiceRequests();

       bool UpdateServiceRequestStatus(int serId, string rType);


       DataSourceResult GetCustomerServiceRequestList(DataSourceRequest request);

       DataSourceResult GetCustomerServiceRequestDetailByRequestId(int? requestId);

       DataSourceResult GetLetterHistoryDetails(DataSourceRequest request);

       LetterViewModel GetLetterByBarcode(string barcode);
   }
}
