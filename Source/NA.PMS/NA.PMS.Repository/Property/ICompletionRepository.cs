using Kendo.Mvc.UI;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Repository
{
    public interface ICompletionRepository
    {
        DataSourceResult GetCompletionData(DataSourceRequest req);
        //List<DDList> GetRIDsForCompletion();
        PropertyCompletionModel GetApplicantDetailsByRId(int rId);
        PropertyCompletionModel GetCompletionDetailsByReqId(int reqId);
        bool SaveCompletionDetails(DateTime completionDueDate, DateTime completionExecutionDate, int rId, string completionType, string propNo, Decimal? partCompletionArea, Decimal? partCompletionPercentage, Decimal? completionCharges, string extension, string viewName);
        bool CheckForUniqueCompletionRequest(int rId);
        //PropertyCompletionModel PrintCompletionChallan(int propertyId, int schemeID, int departmentId, int rId);
        PropertyCompletionModel GetModelToPrintCompletionRequest(int rId);
    }
}
