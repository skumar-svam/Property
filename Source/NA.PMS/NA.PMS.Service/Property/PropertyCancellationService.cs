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
    public class PropertyCancellationService : IPropertyCancellationService
    {
        IPropertyCancellationRepository _propertyCancellationRepository;

        public PropertyCancellationService(IPropertyCancellationRepository propertyCancellationRepository)
        {
            _propertyCancellationRepository = propertyCancellationRepository;
        }

        public DataSourceResult GetAllCancellationsSurrenders(DataSourceRequest req)
        {
            return _propertyCancellationRepository.GetAllCancellationsSurrenders(req);
        }

        public bool SaveCancellation(int rId, int cancelType, string cancelReason, int? refundYesNo, int? refundType, decimal? refundAmt, int userVal, int? reqNo, string restoreReason, decimal? restoreCharges)
        {
            return _propertyCancellationRepository.SaveCancellation(rId, cancelType, cancelReason, refundYesNo, refundType, refundAmt, userVal, reqNo, restoreReason, restoreCharges);
        }

        public DataSourceResult GetRIDsByCancellationType(DataSourceRequest Req,int id)
        {
            return _propertyCancellationRepository.GetRIDsByCancellationType(Req,id);
        }

        public List<DDList> GetRefundTypes(int rId, int depttId)
        {
            return _propertyCancellationRepository.GetRefundTypes(rId, depttId);
        }

        public PropertyCancellationModel GetCancellationDetail(int reqNo)
        {
            return _propertyCancellationRepository.GetCancellationDetail(reqNo);
        }

        public DataSourceResult GetAllCancellationsSurrenders_Approver(DataSourceRequest req)
        {
            return _propertyCancellationRepository.GetAllCancellationsSurrenders_Approver(req);
        }

        public bool SaveCancellationApprovalStatus(string comments, int intStatus, int ReqNo, int type)
        {
            return _propertyCancellationRepository.SaveCancellationApprovalStatus(comments, intStatus, ReqNo, type);
        }

        public bool CancelRequest(int requestId)
        {
            return _propertyCancellationRepository.CancelRequest(requestId);
        }


        public DataSourceResult GetCancelledPropertyListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            return _propertyCancellationRepository.GetCancelledPropertyListAsDataSource(request,model);
        }

        public int SaveCancellationDetail(PropertyViewModel model)
        {
            return _propertyCancellationRepository.SaveCancellationDetail(model);
        }

        public PropertyViewModel GetCancellationDetailById(PropertyViewModel model)
        {
            return _propertyCancellationRepository.GetCancellationDetailById(model);
        }

        public int SaveCancellationStatus(PropertyViewModel model)
        {
            return _propertyCancellationRepository.SaveCancellationStatus(model);
        }
    }
}
