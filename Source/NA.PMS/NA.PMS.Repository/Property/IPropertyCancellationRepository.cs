using Kendo.Mvc.UI;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Repository
{
    public interface IPropertyCancellationRepository
    {
        DataSourceResult GetAllCancellationsSurrenders(DataSourceRequest req);
        bool SaveCancellation(int rId, int cancelType, string cancelReason, int? refundYesNo, int? refundType, decimal? refundAmt, int userVal, int? reqNo, string restoreReason, decimal? restoreCharges);
        DataSourceResult GetRIDsByCancellationType(DataSourceRequest Req,int id);
        List<DDList> GetRefundTypes(int rId, int depttId);
        PropertyCancellationModel GetCancellationDetail(int reqNo);
        DataSourceResult GetAllCancellationsSurrenders_Approver(DataSourceRequest req);
        bool SaveCancellationApprovalStatus(string comments, int intStatus, int ReqNo, int type);
        bool CancelRequest(int requestId);

        DataSourceResult GetCancelledPropertyListAsDataSource(DataSourceRequest request, PropertyViewModel model);

        int SaveCancellationDetail(PropertyViewModel model);

        PropertyViewModel GetCancellationDetailById(PropertyViewModel model);

        int SaveCancellationStatus(PropertyViewModel model);
    }
}
