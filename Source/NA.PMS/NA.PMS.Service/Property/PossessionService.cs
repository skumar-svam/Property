using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Model.CommonModel;
using NA.PMS.Model.Property;
using NA.PMS.Repository;

namespace NA.PMS.Service.Property
{
    public class PossessionService : IPossessionService
    {
        IPossessionRepository _possessionRepository;
        IBusinessRuleRepository _businessRuleRepository;
        IAllotmentEngine _allotmentEngine;

        public PossessionService(IAllotmentEngine allotmentEngine, IBusinessRuleRepository businessRuleRepository, IPossessionRepository possessionRepository)
        {
            _possessionRepository = possessionRepository;
            _businessRuleRepository = businessRuleRepository;
            _allotmentEngine = allotmentEngine;
        }

        // Get possession details 
        public PropertyPossessionModel GetPossessionDetailsByRID(int rId)
        {
            var possessionDetails = _possessionRepository.GetPossessionDetailsByRID(rId);
            if (possessionDetails != null)
            {
                var durationInDays = _businessRuleRepository.GetPossessionDurationInDays(rId, possessionDetails.DepartmentId);
                possessionDetails.PossessionDueDate = durationInDays == 0 ? DateTime.Now.Date.AddDays(15) : DateTime.Now.Date.AddDays(durationInDays);
                
                //possessionDetails.PossessionOrderDate != null
                //? possessionDetails.PossessionOrderDate.Value.AddDays(durationInDays == 0 ? 15 : durationInDays)
                //: DateTime.Now.Date.AddDays(15); //_allotmentEngine.GetPossessionDueDate(rId, possessionDetails.DepartmentId);
            }
            return possessionDetails;
        }

        // get rid details for Bulk possession order
        public PropertyPossessionModel GetBulkPossessionDetailsByRid(int rId)
        {
            var possessionDetails = _possessionRepository.GetBulkPossessionDetailsByRid(rId);
            if (possessionDetails != null)
            {
                var durationInDays = _businessRuleRepository.GetPossessionDurationInDays(rId,
                   possessionDetails.DepartmentId);
                possessionDetails.PossessionDueDate = durationInDays == 0
                    ? DateTime.Now.Date.AddDays(15)
                    : DateTime.Now.Date.AddDays(durationInDays);
                //possessionDetails.PossessionDueDate = possessionDetails.PossessionOrderDate != null
                //    ? possessionDetails.PossessionOrderDate.Value.AddDays(durationInDays == 0 ? 15 : durationInDays)
                //    : DateTime.Now.Date; //_allotmentEngine.GetPossessionDueDate(rId, possessionDetails.DepartmentId);
            }
            return possessionDetails;
        }

        // Get possession Entry details 
        public PropertyPossessionModel GetPossessionEntryDetailsByRID(int rId)
        {
            var possessionDetails = _possessionRepository.GetPossessionEntryDetailsByRID(rId);
            //if (possessionDetails != null)
            //    possessionDetails.PossessionDueDate = possessionDetails.PossessionOrderDate != null
            //        ? possessionDetails.PossessionOrderDate.Value.AddDays(
            //            _businessRuleRepository.GetPossessionDurationInDays(rId, possessionDetails.DepartmentId))
            //        : DateTime.Now.Date;//_allotmentEngine.GetPossessionDueDate(rId, possessionDetails.DepartmentId);
            return possessionDetails;
        }


        // Add possession
        public bool AddUpdatePossession(PropertyPossessionModel possessionModel)
        {
            return _possessionRepository.AddUpdatePossession(possessionModel);
        }

        // Add possession
        public bool AddUpdatePossessionEntry(PropertyPossessionModel possessionModel)
        {
            return _possessionRepository.AddUpdatePossessionEntry(possessionModel);
        }

        // Add Bulk Possession
        public bool AddBulkPossession(List<string> listRiDs)
        {
            return _possessionRepository.AddBulkPossession(listRiDs);
        }

        // Get All properties for which possession has been done 
        public DataSourceResult GetAllPossessionProperties(DataSourceRequest request)
        {
            return _possessionRepository.GetAllPossessionProperties(request);
        }
        // Get All properties for which possession Entry has been done 
        public DataSourceResult GetAllPossessionEntryProperties(DataSourceRequest request)
        {
            return _possessionRepository.GetAllPossessionEntryProperties(request);
        }

        // Get All RIDs whose Lease deed done
        public DataSourceResult GetAllRIDsLeaseDeedDone(DataSourceRequest Req)
        {
            return _possessionRepository.GetAllRIDsLeaseDeedDone(Req);
        }

        // Get properties for which possession is pending by schemeId and Department Id
        public DataSourceResult GetPossessionPropertiesBySchemeDept(DataSourceRequest request, int schemeId, int deptId)
        {
            return _possessionRepository.GetPossessionPropertiesBySchemeDept(request, schemeId, deptId);
        }

        // Get One time excess charge 
        public decimal GetOneTimeExcessCharge(decimal changedArea, int propertyId)
        {
            return _allotmentEngine.CalculateOneTimeExcessCharge(changedArea, propertyId);
        }
        // Get list of models to print possession orders
        public List<PropertyPossessionModel> GetModelToPrintPossessionOrder(List<int> rids)
        {
            return _possessionRepository.GetModelToPrintPossessionOrder(rids);
        }

        // Get All RIDs whose Lease deed done
        public DataSourceResult GetAllPossRIDsLeaseDeedDone(DataSourceRequest Req)
        {
            return _possessionRepository.GetAllPossRIDsLeaseDeedDone(Req);
        }


        public PropertyPossessionModel GetPossessionDetailsByRegistratioinId(int registrationId)
        {
            return _possessionRepository.GetPossessionDetailsByRegistratioinId(registrationId);
        }


        public DataSourceResult GetPossessionDataByApproverId(DataSourceRequest request, PropertyPossessionModel model)
        {
            return _possessionRepository.GetPossessionDataByApproverId(request, model);
        }


        public PropertyPossessionModel GetPossessionDetailsByReqNo(int id)
        {
            return _possessionRepository.GetPossessionDetailsByReqNo(id);
        }


        public int UpdatePossessionRequest(PropertyPossessionModel model)
        {
            return _possessionRepository.UpdatePossessionRequest(model);
        }
    }
}
