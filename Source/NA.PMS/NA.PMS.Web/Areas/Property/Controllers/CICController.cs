using System;
using System.Web;
using System.Linq;
using Kendo.Mvc.UI;
using NA.PMS.Service;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using NA.PMS.Web.Controllers;
using System.Collections.Generic;
using NA.PMS.Service.BusinessRuleEngine;
using NA.PMS.Model;
using NA.PMS.Common;
using NA.PMS.Web.Models;
using NA.PMS.Web.Controllers.Common;
using System.Configuration;

namespace NA.PMS.Web.Areas.Property.Controllers
{
    public class CICController : Controller
    {
        static string servicePassalt = ConfigurationManager.AppSettings["ServicePassalt"];
        private ICICService _cicService;
        private IGeneralService _generalService;
        IRequestService _requstService;
        NiveshMitraServices _niveshMitraServices;
        INICService _nicService;
        // Const variable to bind dropdown for particular type.
        const string CHANGEDDLTYPE = "CIC";
        const string TYPE = "Director";
        const string FIRMSTATUS = "Firm Status";
        const string SHARETYPE = "Share Type";
        public CICController(ICICService iCICService, IGeneralService iGeneralService, IRequestService requestService, INICService nicServices)
        {
            _cicService = iCICService;
            _generalService = iGeneralService;
            _requstService = requestService;
            _niveshMitraServices = new NiveshMitraServices();
            _nicService = nicServices;
        }

        public ActionResult Manage()
        {
            return View();
        }

        //To Manage Change in Constitution. 
        public ActionResult ManageCIC()
        {
            return View();
        }

        //To Get All CIC
        public JsonResult GetAllCIC([DataSourceRequest] DataSourceRequest req)
        {
            var lstCIC = _cicService.GetAllCIC(req);
            return Json(lstCIC, JsonRequestBehavior.AllowGet);
        }

        //To Get All Firm/Product/Status
        public JsonResult GetAllFirmProductStatus([DataSourceRequest] DataSourceRequest req)
        {
            var lstFirmProductStatus = _cicService.GetAllFirmProductStatus(req);
            return Json(lstFirmProductStatus, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// To return blank CIC View.
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCIC()
        {
            return View();
        }

        // To Save CIC for Director       
        // (int rid, decimal directorShare, int changeType, string directorName, int type, decimal cicCharge = 0)
        public ActionResult SaveCIC(CICModel cicModel)
        {
            var flag = false;
            flag = _cicService.SaveCIC(cicModel);
            return Json(flag);
        }

        // To Bind Change Type in drop down.
        public ActionResult GetChangeType()
        {
            var lstChangeType = _generalService.BindDDL(CHANGEDDLTYPE);
            return Json(lstChangeType, JsonRequestBehavior.AllowGet);
        }

        // To Bind Type in drop down.
        public ActionResult GetType()
        {
            var lstChangeType = _generalService.BindDDL(TYPE);
            return Json(lstChangeType, JsonRequestBehavior.AllowGet);
        }

        // To Firm Status in drop down.
        public ActionResult GetFirmStatus()
        {
            var lstFirmStatus = _generalService.BindDDL(FIRMSTATUS);
            return Json(lstFirmStatus, JsonRequestBehavior.AllowGet);
        }

        // To bind Share Type in drop down.
        public ActionResult GetShareType()
        {
            var lstFirmStatus = _generalService.BindDDL(SHARETYPE);
            return Json(lstFirmStatus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDirectorDetails(DataSourceRequest req, int rid)
        {
            var lstDirectorDetails = _cicService.GetDirectorDetails(req, rid);
            return Json(lstDirectorDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SubmitForDirectors(int rid, string approver, int reqRefNo, decimal ciccharge = 0)
        {
            var flag = false;
            flag = _cicService.SubmitForDirectors(rid, approver, reqRefNo, ciccharge);
            return Json(flag);
        }

        public ActionResult SubmitForFirmName(int rid, string approver, int newFirmStatus, string oldFirmName, string newFirmName, int reqRefNo, decimal ciccharge = 0)
        {
            var flag = 0;
            flag = _cicService.SubmitForFirmName(rid, approver, newFirmStatus, oldFirmName, newFirmName, reqRefNo, ciccharge);
            return Json(flag);
        }

        public ActionResult SubmitForFirmProduct(int rid, string approver, string oldFirmProduct, string newFirmProduct, int reqRefNo, decimal ciccharge = 0)
        {
            var flag = 0;
            flag = _cicService.SubmitForFirmProduct(rid, approver, oldFirmProduct, newFirmProduct, reqRefNo, ciccharge);
            return Json(flag);
        }


        //To Manage Change in Constitution for Approval. 
        public ActionResult ManageCICApprover()
        {
            return View();
        }

        //To Get All CIC by Approver
        public JsonResult GetAllCICByApprover([DataSourceRequest] DataSourceRequest req)
        {
            var lstCIC = _cicService.GetAllCICByApprover(req);
            return Json(lstCIC, JsonRequestBehavior.AllowGet);
        }

        //To Get All Firm/Product/Status by Approver
        public JsonResult GetAllFirmProductStatusByApprover([DataSourceRequest] DataSourceRequest req)
        {
            var lstFirmProductStatus = _cicService.GetAllFirmProductStatusByApprover(req);
            return Json(lstFirmProductStatus, JsonRequestBehavior.AllowGet);
        }

        //To Remove Director
        public JsonResult RemoveRecord(int dirID)
        {
            var flag = _cicService.RemoveRecord(dirID);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        //To Get Firm old Details.
        public ActionResult GetOldFirmName(int rid)
        {
            var lstOldDetails = _cicService.GetOldFirmName(rid);
            return Json(lstOldDetails, JsonRequestBehavior.AllowGet);
        }


        //To Get Firm old Products.
        public ActionResult GetOldFirmProduct(int rid)
        {
            var lstOldProduct = _cicService.GetOldFirmProduct(rid);
            return Json(lstOldProduct, JsonRequestBehavior.AllowGet);
        }

        //To View CIC Details
        public ActionResult ViewCIC()
        {
            var reqId = Convert.ToInt32(Request.QueryString["ReqID"]);
            var type = Convert.ToInt32(Request.QueryString["Type"]);
            var lstDetailsbyDirID = _cicService.GetDetailsbyDirID(reqId, type);
            return View(lstDetailsbyDirID);
        }


        public JsonResult GetPropertyDetailByRid(int rID)
        {
            var lst = _cicService.GetPropertyDetailByRid(rID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelCICRequest(int requestID, int typeID)
        {
            var flag = _cicService.CancelCICRequest(requestID, typeID);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        //To Manage Change in Constitution for Approval or Rejection.
        public ActionResult ViewCICApproval()
        {
            var reqId = Convert.ToInt32(Request.QueryString["ReqID"]);
            var type = Convert.ToInt32(Request.QueryString["Type"]);
            var lstDetailsbyDirID = _cicService.GetDetailsbyDirID(reqId, type);
            return View(lstDetailsbyDirID);
        }

        public ActionResult SaveCommentByRequestID(int requestNo, string Comment, bool acceptReject, int typeID)
        {
            var lst = _cicService.SaveCommentByRequestID(requestNo, Comment, acceptReject, typeID);
            var lstDetailsbyDirID = _cicService.GetDetailsbyDirID(requestNo, typeID);
            if (lstDetailsbyDirID.ReqRefNo != 0 && acceptReject==true)
            {
                SendNICStatus(lstDetailsbyDirID.ReqRefNo,Comment);
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public int SendNICStatus(int ReqNo, string comment)
        {
            var flag = ReturnType.None;
            CurrentUserDetail user = Session["CurrentUser"] as CurrentUserDetail;
            //CurrentUserDetail user = HttpContext.Current.Session["CurrentUser"] as CurrentUserDetail;
            var requestDetail = _requstService.GetServiceRequestDetailById(ReqNo);
            var nicDetails = _nicService.GetNiveshMitraServicesByReqId(ReqNo);
            var serviceStatus = _nicService.GetServiceStatusByCustomerRequestStatusId(NAStatusId.Approved);
            if (requestDetail.ServiceModel.RequestThrough == Constants.NIC_NiveshMitra && requestDetail.ServiceModel.StatusId == NAStatusId.Completed)
            {
                WReturn_CUSID_STATUSModel objWReturn_CUSID_STATUSModel = new WReturn_CUSID_STATUSModel
                {
                    ControlID = nicDetails.NICModel.TxtControlID,
                    ApplicationID = Convert.ToString(ReqNo),
                    ProcessIndustryID = nicDetails.NICModel.TxtProcessIndustryID,
                    UnitID = nicDetails.NICModel.TxtUnitID,
                    ServiceID = nicDetails.NICModel.TxtServiceID,
                    Status_Code = serviceStatus.StatusCode,
                    Remarks = "REMARKS | " + comment + " - User: " + user.FirstName + " - Status:  " + serviceStatus.StatusName + " | ",
                    Fee_Status = string.Empty,
                    Fee_Amount = string.Empty,
                    passsalt = servicePassalt
                };
                NiveshMitraServices _niveshMitraServices = new NiveshMitraServices();
                _niveshMitraServices.GetWReturn_CUSID_STATUS(objWReturn_CUSID_STATUSModel);
                flag = ReturnType.Saved;
            }
            return flag;
        }

        //To Update for Firm Name       
        public ActionResult UpdateForFirmName(int Id, int rid, string approver, int newFirmStatus, string newFirmName, decimal ciccharge = 0)
        {
            var flag = false;
            flag = _cicService.UpdateForFirmName(Id, rid, approver, newFirmStatus, newFirmName, ciccharge);
            return Json(flag);
        }

        //To Update for Product Name      
        public ActionResult UpdateForFirmProduct(int Id, int rid, string approver, string newFirmProduct, decimal ciccharge)
        {
            var flag = false;
            flag = _cicService.UpdateForFirmProduct(Id, rid, approver, newFirmProduct, ciccharge);
            return Json(flag);
        }

        // To Generate Mortgage letter
        public ActionResult GenerateCICLetter(int Id)
        {
            var lst = _cicService.GenerateCICLetter(Id);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRIDsByDeptt([DataSourceRequest] DataSourceRequest Req, int Rid)
        {

            var lst = _cicService.GetRIDsByDeptt(Req, Rid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRegistrationIdsByDepartment([DataSourceRequest]DataSourceRequest request)
        {
            DataSourceResult ridList = _cicService.GetRegistrationIdsByDepartment(request);
            return Json(ridList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDirectorAndShareholderListAsDataSource([DataSourceRequest]DataSourceRequest request, CICViewModel model)
        {
            DataSourceResult ridList = _cicService.GetDirectorAndShareholderListAsDataSource(request, model);
            return Json(ridList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFirmAndProductNameListAsDataSource([DataSourceRequest]DataSourceRequest request, CICViewModel model)
        {
            DataSourceResult ridList = _cicService.GetFirmAndProductNameListAsDataSource(request, model);
            return Json(ridList, JsonRequestBehavior.AllowGet);
        }
    }
}