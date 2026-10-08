using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Repository;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Model;

namespace NA.PMS.Service
{
    public class CICService : ICICService
    {
        ICICRepository _cicRepository;

        public CICService(ICICRepository iICICRepository)
        {
            _cicRepository = iICICRepository;
        }

        public bool SaveCIC(CICModel cicModel)
        {
            return _cicRepository.SaveCIC(cicModel);
        }

        //Get all details for Directors
        public DataSourceResult GetDirectorDetails(DataSourceRequest req, int rid)
        {
            return _cicRepository.GetDirectorDetails(req, rid);
        }

        //To Sumbit for Director for Approval
        public bool SubmitForDirectors(int rid, string approver, int reqRefNo, decimal ciccharge)
        {
            return _cicRepository.SubmitForDirectors(rid, approver, reqRefNo, ciccharge);
        }

        //To Sumbit for Firm Name
        public int SubmitForFirmName(int rid, string approver, int newFirmStatus, string oldFirmName, string newFirmName, int reqRefNo, decimal ciccharge)
        {
            return _cicRepository.SubmitForFirmName(rid, approver, newFirmStatus, oldFirmName, newFirmName, reqRefNo, ciccharge);
        }

        //To Sumbit for Product Name
        public int SubmitForFirmProduct(int rid, string approver, string oldFirmProduct, string newFirmProduct, int reqRefNo, decimal ciccharge)
        {
            return _cicRepository.SubmitForFirmProduct(rid, approver, oldFirmProduct, newFirmProduct, reqRefNo, ciccharge);
        }

        //Get all details for CIC
        public DataSourceResult GetAllCIC(DataSourceRequest req)
        {
            return _cicRepository.GetAllCIC(req);
        }

        //Get all details for FirmProductStatus
        public DataSourceResult GetAllFirmProductStatus(DataSourceRequest req)
        {
            return _cicRepository.GetAllFirmProductStatus(req);
        }

        //Get all details for CIC by Approver
        public DataSourceResult GetAllCICByApprover(DataSourceRequest req)
        {
            return _cicRepository.GetAllCICByApprover(req);
        }

        //Get all details for FirmProductStatus by Approver
        public DataSourceResult GetAllFirmProductStatusByApprover(DataSourceRequest req)
        {
            return _cicRepository.GetAllFirmProductStatusByApprover(req);
        }

        //To Remove Record (Soft Delete)
        public bool RemoveRecord(int dirID)
        {
            return _cicRepository.RemoveRecord(dirID);
        }

        //To Get Firm old Details.
        public CICModel GetOldFirmName(int rid)
        {
            return _cicRepository.GetOldFirmName(rid);
        }

        //To Get Firm old Product Details.
        public CICModel GetOldFirmProduct(int rid)
        {
            return _cicRepository.GetOldFirmProduct(rid);
        }

        ////To Get All details for view.
        public CICModel GetDetailsbyDirID(int id, int type)
        {
            return _cicRepository.GetDetailsbyDirID(id, type);
        }

        //To Fill Mortgage Prev Loan Drop Down
        public CICModel GetPropertyDetailByRid(int rID)
        {
            return _cicRepository.GetPropertyDetailByRid(rID);
        }

        ////To Get All details for Firm.
        public CICModel GetDetailsbyFirm(int id)
        {
            return _cicRepository.GetDetailsbyFirm(id);
        }

        ////To Cancel Request
        public bool CancelCICRequest(int requestID, int typeID)
        {
            return _cicRepository.CancelCICRequest(requestID, typeID);
        }

        public bool SaveCommentByRequestID(int requestNo, string Comment, bool acceptReject, int typeID)
        {
            return _cicRepository.SaveCommentByRequestID(requestNo, Comment, acceptReject, typeID);
        }

        //To Update for Firm Name
        public bool UpdateForFirmName(int Id, int rid, string approver, int newFirmStatus, string newFirmName, decimal ciccharge)
        {
            return _cicRepository.UpdateForFirmName(Id, rid, approver, newFirmStatus, newFirmName, ciccharge);
        }

        //To Update for Product Name
        public bool UpdateForFirmProduct(int Id, int rid, string approver, string newFirmProduct, decimal ciccharge)
        {
            return _cicRepository.UpdateForFirmProduct(Id, rid, approver, newFirmProduct, ciccharge);
        }

        // To Generate CIC Letter
        public string GenerateCICLetter(int Id)
        {
            return _cicRepository.GenerateCICLetter(Id);
        }

        //Get all RIDs
        public DataSourceResult GetRIDsByDeptt(DataSourceRequest Req, int Rid)
        {
            return _cicRepository.GetRIDsByDeptt(Req, Rid);
        }


        public DataSourceResult GetRegistrationIdsByDepartment(DataSourceRequest request)
        {
            return _cicRepository.GetRegistrationIdsByDepartment(request);
        }


        public DataSourceResult GetDirectorAndShareholderListAsDataSource(DataSourceRequest request, CICViewModel model)
        {
            return _cicRepository.GetDirectorAndShareholderListAsDataSource(request, model);
        }

        public DataSourceResult GetFirmAndProductNameListAsDataSource(DataSourceRequest request, CICViewModel model)
        {
            return _cicRepository.GetFirmAndProductNameListAsDataSource(request, model);
        }
    }
}
