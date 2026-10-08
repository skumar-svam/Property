using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Model.Property;

namespace NA.PMS.Repository.Property
{
    public interface IMergeSplitPropertyRepository
    {
        // Get All Merged properties  
        DataSourceResult GetAllMergeProperties(DataSourceRequest request);
        // Get All Split properties  
        DataSourceResult GetAllSplitProperties(DataSourceRequest request);

        // Get All Merged Requests  
        DataSourceResult GetAllMergeRequests(DataSourceRequest request);
        // Get All Split Requests  
        DataSourceResult GetAllSplitRequests(DataSourceRequest request);

        // Add Merge Request
        bool AddMergeRequest(int rId);
        // Add Split Request
        bool AddSplitRequest(int rId);
        // Get All RIDs forSplit or Merger
        List<SelectListItem> GetAllRIDsForSplitMerge(int departmentId, int sectorId, int blockId);
        DataSourceResult GetAllRIDsForSplitMerge(DataSourceRequest Req, int departmentId, int sectorId, int blockId);
        // Add property to merge/split
        bool AddUpdatePropertyToMergeSplit(int rid, int propertyId);
        // Remove property to merge/split
        bool RemovePropertyFromMergeSplit(int rid);
        // Get Properties to merge
        List<MergeSplitPropertyGrid> GetPropertiesToMergeByRequestId(int requestId);
        // Get Properties to Split
        List<MergeSplitPropertyGrid> GetPropertiesToSplitByRequestId(int requestId);

        // Merge Request
        bool MergeRequest(string rids, int userId, decimal charges, int status, int type, int? requestNo,
            string comment);
        // Split Request
        bool SplitRequest(string rids, int rId, int userId, decimal charges, int status, int type, int? requestNo,
            string comment);

        // Get Split Merge Request details by request id
        AddMergeSplitRequestModel GetMergeSplitRequestDetails(int requestId);

        // Get All RIDs for Split
        List<SelectListItem> GetAllRidsForSplit(int departmentId);
        // Get All RIDs for Split
        List<MergeSplitPropertyGrid> GetPropertiesToSplitByRid(int rid);

        // Get RequestId by  RID 
        int GetRequestIdByRid(int rid);

        // Get PropertyDetails By RID
        SchemePropertyTransDetail GetPropertyDetailsByRid(int rId);
    }
}
