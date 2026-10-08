using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Model.Property;
using NA.PMS.Repository.Property;

namespace NA.PMS.Service.Property
{
    public class MergeSplitPropertyService : IMergeSplitPropertyService
    {
        IMergeSplitPropertyRepository _mergeSplitPropertyRepository;

        public MergeSplitPropertyService(IMergeSplitPropertyRepository mergeSplitPropertyRepository)
        {
            _mergeSplitPropertyRepository = mergeSplitPropertyRepository;
        }

        // Get All Merged properties  
        public DataSourceResult GetAllMergeProperties(DataSourceRequest request)
        {
            var mergeAllProp = _mergeSplitPropertyRepository.GetAllMergeProperties(request);
            return mergeAllProp;
        }


        // Get All Split properties  
        public DataSourceResult GetAllSplitProperties(DataSourceRequest request)
        {
            var splitAllProp = _mergeSplitPropertyRepository.GetAllSplitProperties(request);
            return splitAllProp;
        }

        // Get All Merged Requests  
        public DataSourceResult GetAllMergeRequests(DataSourceRequest request)
        {
            var splitAllProp = _mergeSplitPropertyRepository.GetAllMergeRequests(request);
            return splitAllProp;
        }
        // Get All Split Requests  
        public DataSourceResult GetAllSplitRequests(DataSourceRequest request)
        {
            var splitAllProp = _mergeSplitPropertyRepository.GetAllSplitRequests(request);
            return splitAllProp;
        }

        // Add Merge Request
        public bool AddMergeRequest(int rId)
        {
            var result = _mergeSplitPropertyRepository.AddMergeRequest(rId);
            return result;
        }

        // Add Merge Request
        public bool AddSplitRequest(int rId)
        {
            var result = _mergeSplitPropertyRepository.AddSplitRequest(rId);
            return result;
        }

        // Get All RIDs forSplit or Merger
        public List<SelectListItem> GetAllRIDsForSplitMerge(int departmentId, int sectorId, int blockId)
        {
            var result = _mergeSplitPropertyRepository.GetAllRIDsForSplitMerge(departmentId, sectorId, blockId);
            return result;
        }

        public DataSourceResult GetAllRIDsForSplitMerge(DataSourceRequest Req, int departmentId, int sectorId, int blockId)
        {
            var result = _mergeSplitPropertyRepository.GetAllRIDsForSplitMerge(Req, departmentId, sectorId, blockId);
            return result;
        }

        // Add property to merge/split
        public bool AddUpdatePropertyToMergeSplit(int rid, int propertyId)
        {
            var result = _mergeSplitPropertyRepository.AddUpdatePropertyToMergeSplit(rid, propertyId);
            return result;
        }

        // Remove property to merge/split
        public bool RemovePropertyFromMergeSplit(int rid)
        {
            var result = _mergeSplitPropertyRepository.RemovePropertyFromMergeSplit(rid);
            return result;
        }

        // Get Properties to merge
        public List<MergeSplitPropertyGrid> GetPropertiesToMergeByRequestId(int requestId)
        {
            var result = _mergeSplitPropertyRepository.GetPropertiesToMergeByRequestId(requestId);
            return result;
        }

        // Get Properties to Split
        public List<MergeSplitPropertyGrid> GetPropertiesToSplitByRequestId(int requestId)
        {
            var result = _mergeSplitPropertyRepository.GetPropertiesToSplitByRequestId(requestId);
            return result;
        }

        // Merge Request
        public bool MergeRequest(string rids, int userId, decimal charges, int status, int type, int? requestNo,
            string comment)
        {
            var result = _mergeSplitPropertyRepository.MergeRequest(rids, userId, charges, status, type, requestNo ?? 0,
                comment);
            return result;
        }

        // Split Request
        public bool SplitRequest(string rids, int rId, int userId, decimal charges, int status, int type, int? requestNo,
            string comment)
        {
            var result = _mergeSplitPropertyRepository.SplitRequest(rids, rId, userId, charges, status, type, requestNo ?? 0,
                comment);
            return result;
        }

        // Get Split Merge Request details by request id
        public AddMergeSplitRequestModel GetMergeSplitRequestDetails(int requestId)
        {
            var result = _mergeSplitPropertyRepository.GetMergeSplitRequestDetails(requestId);
            return result;
        }

        // Get All RIDs for Split
        public List<SelectListItem> GetAllRidsForSplit(int departmentId)
        {
            var result = _mergeSplitPropertyRepository.GetAllRidsForSplit(departmentId);
            return result;
        }

        // Get All RIDs for Split
        public List<MergeSplitPropertyGrid> GetPropertiesToSplitByRid(int rid)
        {
            var result = _mergeSplitPropertyRepository.GetPropertiesToSplitByRid(rid);
            return result;
        }

        // Get RequestId by  RID 
        public int GetRequestIdByRid(int rid)
        {
            var result = _mergeSplitPropertyRepository.GetRequestIdByRid(rid);
            return result;
        }


        // Get PropertyDetails By RID
        public SchemePropertyTransDetail GetPropertyDetailsByRid(int rId)
        {
            var result = _mergeSplitPropertyRepository.GetPropertyDetailsByRid(rId);
            return result;
        }

    }
}
