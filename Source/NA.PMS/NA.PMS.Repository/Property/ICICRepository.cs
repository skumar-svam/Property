using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Model;


namespace NA.PMS.Repository
{
    public interface ICICRepository
    {
        // To Save CIC
        bool SaveCIC(CICModel cicModel);

        //Get all details for Directors
        DataSourceResult GetDirectorDetails(DataSourceRequest req, int rid);

        //To Sumbit for Director for Approval
        bool SubmitForDirectors(int rid, string approver, int reqRefNo, decimal ciccharge);

        //To Sumbit for Firm Name
        int SubmitForFirmName(int rid, string approver, int newFirmStatus, string oldFirmName, string newFirmName, int reqRefNo, decimal ciccharge);

        //To Sumbit for Product Name
        int SubmitForFirmProduct(int rid, string approver, string oldFirmProduct, string newFirmProduct, int reqRefNo, decimal ciccharge);

        //Get all details for CIC
        DataSourceResult GetAllCIC(DataSourceRequest req);

        //Get all details for FirmProductStatus
        DataSourceResult GetAllFirmProductStatus(DataSourceRequest req);

        //Get all details for CIC by Approver
        DataSourceResult GetAllCICByApprover(DataSourceRequest req);

        //Get all details for FirmProductStatus by Approver
        DataSourceResult GetAllFirmProductStatusByApprover(DataSourceRequest req);

        //To Remove Record (Soft Delete)
        bool RemoveRecord(int dirID);

        //To Get Firm old Details.
        CICModel GetOldFirmName(int rid);

        //To Get Firm old Product Details.
        CICModel GetOldFirmProduct(int rid);

        ////To Get All details for view.
        CICModel GetDetailsbyDirID(int id, int type);

        ////To Get Property details for view.
        CICModel GetPropertyDetailByRid(int rID);

        ////To Get All details for Firm.
        CICModel GetDetailsbyFirm(int id);

        ////To Cancel Request
        bool CancelCICRequest(int requestID, int typeID);

        bool SaveCommentByRequestID(int requestNo, string Comment, bool acceptReject, int typeID);

        //To Update for Firm Name
        bool UpdateForFirmName(int Id, int rid, string approver, int newFirmStatus, string newFirmName, decimal ciccharge);

        //To Update for Product Name
        bool UpdateForFirmProduct(int Id, int rid, string approver, string newFirmProduct, decimal ciccharge);

        // To Generate CIC Letter
        string GenerateCICLetter(int Id);

        //Get all RIDs
        DataSourceResult GetRIDsByDeptt(DataSourceRequest Req, int Rid);

        DataSourceResult GetRegistrationIdsByDepartment(DataSourceRequest request);

        DataSourceResult GetDirectorAndShareholderListAsDataSource(DataSourceRequest request, CICViewModel model);

        DataSourceResult GetFirmAndProductNameListAsDataSource(DataSourceRequest request, CICViewModel model);
    }
}
