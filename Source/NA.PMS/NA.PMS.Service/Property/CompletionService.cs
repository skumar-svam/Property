using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Service.Property
{
    public class CompletionService : ICompletionService
    {
        ICompletionRepository _completionRepository;

        public CompletionService(ICompletionRepository completionRepository)
        {
            _completionRepository = completionRepository;
        }

        public DataSourceResult GetCompletionData(DataSourceRequest req)
        {
            return _completionRepository.GetCompletionData(req);
        }

        //public List<DDList> GetRIDsForCompletion()
        //{
        //    return _completionRepository.GetRIDsForCompletion();
        //}

        public PropertyCompletionModel GetApplicantDetailsByRId(int rId)
        {
            return _completionRepository.GetApplicantDetailsByRId(rId);
        }

        public PropertyCompletionModel GetCompletionDetailsByReqId(int reqNo)
        {
            return _completionRepository.GetCompletionDetailsByReqId(reqNo);
        }

        public bool SaveCompletionDetails(DateTime completionDueDate, DateTime completionExecutionDate, int rId, string completionType, string propNo, Decimal? partCompletionArea, Decimal? partCompletionPercentage, Decimal? completionCharges, string extension, string viewName)
        {
            return _completionRepository.SaveCompletionDetails(completionDueDate, completionExecutionDate, rId, completionType, propNo, partCompletionArea, partCompletionPercentage, completionCharges, extension, viewName);
        }

        public bool CheckForUniqueCompletionRequest(int rId)
        {
            return _completionRepository.CheckForUniqueCompletionRequest(rId);
        }

        //public PropertyCompletionModel PrintCompletionChallan(int propertyId, int schemeID, int departmentId, int rId)
        //{
        //    return _completionRepository.PrintCompletionChallan(propertyId,  schemeID,  departmentId,rId);
        //}
        public PropertyCompletionModel GetModelToPrintCompletionRequest(int rId)
        {
            return _completionRepository.GetModelToPrintCompletionRequest(rId);
        }
    }
}
