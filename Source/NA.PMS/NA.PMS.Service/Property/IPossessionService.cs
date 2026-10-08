using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Model.CommonModel;
using NA.PMS.Model.Property;

namespace NA.PMS.Service.Property
{
    public interface IPossessionService
    {
        // Get possession details 
        PropertyPossessionModel GetPossessionDetailsByRID(int rId);
          // get rid details for Bulk possession order
        PropertyPossessionModel GetBulkPossessionDetailsByRid(int rId);
        // Get possession Entry details 
        PropertyPossessionModel GetPossessionEntryDetailsByRID(int rId);
        // Get All properties for which possession has been done 
        DataSourceResult GetAllPossessionProperties(DataSourceRequest request);
        // Get All properties for which possession Entry has been done 
        DataSourceResult GetAllPossessionEntryProperties(DataSourceRequest request);
        
         // Add possession
        bool AddUpdatePossession(PropertyPossessionModel possessionModel);
        // Add possession Entry
        bool AddUpdatePossessionEntry(PropertyPossessionModel possessionModel);
        // Add Bulk Possession
        bool AddBulkPossession(List<string> listRiDs);
        // Get All RIDs whose Lease deed done
        DataSourceResult GetAllRIDsLeaseDeedDone(DataSourceRequest Req);
        // Get properties for which possession is pending by schemeId and Department Id
        DataSourceResult GetPossessionPropertiesBySchemeDept(DataSourceRequest request, int schemeId, int deptId);
        // Get One time excess charge 
        decimal GetOneTimeExcessCharge(decimal changedArea, int propertyId);
        // Get list of models to print possession orders
        List<PropertyPossessionModel> GetModelToPrintPossessionOrder(List<int> rids);
        // Get All RIDs whose Lease deed done
        DataSourceResult GetAllPossRIDsLeaseDeedDone(DataSourceRequest Req);


        PropertyPossessionModel GetPossessionDetailsByRegistratioinId(int registrationId);

        DataSourceResult GetPossessionDataByApproverId(DataSourceRequest request, PropertyPossessionModel model);

        PropertyPossessionModel GetPossessionDetailsByReqNo(int id);

        int UpdatePossessionRequest(PropertyPossessionModel model);
    }
}
