using Kendo.Mvc.UI;
using NA.PMS.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Model;
using NA.PMS.Service.Property;
using Kendo.Mvc.Extensions;
using NA.PMS.Service;
using NA.PMS.Service.BusinessRuleEngine;
using System.IO;
using System.Text;
using System.Configuration;
using NA.PMS.Common;
using Microsoft.Office.Interop.Word;
using NA.PMS.Web.Filters;
using NA.PMS.Web.Controllers.Common;
using NA.PMS.Web.Models;

namespace NA.PMS.Web.Areas.Property.Controllers
{
    public class PropertyRegistrationController : WebBaseController
    {
        static string servicePassalt = ConfigurationManager.AppSettings["ServicePassalt"];
        private IPropertyRegistrationService _propertyRegistrationService;
        private IGeneralService _generalService;
        private IAllotmentService _allotmentService;
        private IPaymentEngine _paymentEngine;
        private ISchemeService _schemeService;
        private IRequestService _requstService;
        NiveshMitraServices _niveshMitraServices;
        INICService _nicService;
        public PropertyRegistrationController(IPropertyRegistrationService propertyRegistrationService, ISchemeService schemeService, IGeneralService generalService, IAllotmentService allotmentService, IPaymentEngine paymentEngine, IRequestService requestService,INICService nicServices)
        {
            _propertyRegistrationService = propertyRegistrationService;
            _generalService = generalService;
            _allotmentService = allotmentService;
            _paymentEngine = paymentEngine;
            _schemeService = schemeService;
            _requstService = requestService;
            _niveshMitraServices = new NiveshMitraServices();
            _nicService = nicServices;
        }

        /// <summary>
        /// Returns View for ManageLeaseDeed screen
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageLeaseDeed()
        {
            return View();
        }

        /// <summary>
        /// Reads Lease Deed data from DB for ManageLeaseDeed screen
        /// </summary>
        /// <param name="req">Kendo's internal parameter</param>
        /// <returns></returns>
        public JsonResult GetLeaseDeedData([DataSourceRequest] DataSourceRequest req)
        {
            var LeaseDeedLst = _propertyRegistrationService.GetLeaseDeedData(req);
            return Json(LeaseDeedLst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns View for Add Lease Deed screen.
        /// </summary>
        /// <returns></returns>
        public ActionResult AddLeaseDeed()
        {
            var obj = new LeaseDeedProperty();
            obj.Type = Constants.intLease;
            return View(obj);
        }

        /// <summary>
        /// Returns View for Add Sublease Deed screen.
        /// </summary>
        /// <returns></returns>
        public ActionResult AddSubleaseDeed()
        {
            var obj = new LeaseDeedProperty();
            obj.Type = Constants.intSublease;
            return View(obj);
        }

        /// <summary>
        /// Returns View for GenerateChecklist screen
        /// </summary>
        /// <returns></returns>
        public ActionResult GenerateCheckList()
        {
            return View();
        }

        /// <summary>
        /// Fetches RIDs on the basis of Departments allowed from DB.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDsByDeptt([DataSourceRequest] DataSourceRequest Req, int Rid)
        {
            var lst = _allotmentService.GetRIDsByDeptt(Req, Rid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fetches RIDs for Lease Deed
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDsForLeaseDeed([DataSourceRequest] DataSourceRequest request)
        {
            //var lst = _propertyRegistrationService.GetRIDsForLeaseDeed();
            var lst = _propertyRegistrationService.GetRIDsForLeaseDeed(request);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fetches RIDs for Sublease Deed
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDsForSubleaseDeed()
        {
            var lst = _propertyRegistrationService.GetRIDsForSubleaseDeed();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Generates view for Lease Rent screen
        /// </summary>
        /// <returns></returns>
        public ActionResult LeaseRent()
        {
            return View();
        }

        /// <summary>
        /// Returns View for ManagePossession screen
        /// </summary>
        /// <returns>View</returns>
        public ActionResult ManagePossession()
        {
            return View();
        }

        public JsonResult GetPossessionData([DataSourceRequest] DataSourceRequest req)
        {
            var possessionLst = _propertyRegistrationService.GetPossessionData(req);
            return Json(possessionLst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RequestPossessionOrder()
        {
            return View();
        }

        /// <summary>
        /// Gets details from the DB on the basis of RID
        /// </summary>
        /// <param name="rId">RID</param>
        /// <returns></returns>
        public JsonResult GetDetailsByRID(int rId)
        {
            if (rId != 0)
            {
                var details = _propertyRegistrationService.GetDetailsByRID(rId);
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets Stamp Duty related details from DB on the basis of Lease Deed Execution Date
        /// </summary>
        /// <param name="LDExecDate">Lease Deed Execution Date</param>
        /// <returns></returns>
        public JsonResult GetStampDetailsByLeaseExcutionDate(int rId, DateTime LDExecDate)
        {
            if (LDExecDate != null && rId != 0)
            {
                var details = _propertyRegistrationService.GetStampDetailsByLeaseExcutionDate(rId, LDExecDate);
                if (details != null)
                    return Json(details, JsonRequestBehavior.AllowGet);
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Reads Checklist Types for DDL
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCheckListType()
        {
            var checkListType = _propertyRegistrationService.GetCheckListType();
            return Json(checkListType, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Manage Rent Permission for Department user
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageRentPermission()
        {
            return View();
        }
        /// <summary>
        /// return rent permission requests list 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResult GetPropertyRentList([DataSourceRequest]DataSourceRequest request)
        {
            var rentList = _propertyRegistrationService.GetPropertyRentList(request);
            //var data = rentList.ToDataSourceResult(request);
            return Json(rentList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// return rent permission requests list 
        /// </summary>
        /// <param name="request"></param>
        /// /// <param name="Rid"></param>
        /// <returns></returns>
        public JsonResult GetPropertyRentListByRid([DataSourceRequest]DataSourceRequest request, int Rid)
        {
            var rentList = _propertyRegistrationService.GetPropertyRentListByRid(request, Rid);
            //var data = rentList.ToDataSourceResult(request);
            return Json(rentList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// return requested rent lists for approver
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResult GetRentRequestsList([DataSourceRequest]DataSourceRequest request)
        {
            var rentList = _propertyRegistrationService.GetRentRequestsList(request);
            // var data = rentList.ToDataSourceResult(request);
            return Json(rentList, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// add rent request permission
        /// </summary>
        /// <returns></returns>
        public ActionResult AddRentRequest(string id)
        {
            RentPermissionModel model = new RentPermissionModel();
            if (id != null)
            {
                model = GetRentRequestDetailForServices(Convert.ToInt32(id));
                return View(model);
            }
            else
            {
                return View();
            }
        }

        private RentPermissionModel GetRentRequestDetailForServices(int rid)
        {
            RentPermissionModel allotteeDetails = _propertyRegistrationService.GetAllotteDetailsForRentPermission(rid);
            decimal dues = _paymentEngine.GetBalanceDueTillDate(rid, allotteeDetails.DepartmentId);
            allotteeDetails.TotalDues = dues;
            allotteeDetails.Rid = rid;
            return allotteeDetails;
        }

        /// <summary>
        /// view and edit rent request permission
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditRentRequest(string id)
        {
            int ID = Convert.ToInt32(CommonHelper.Decode(id));

            RentPermissionModel request = _propertyRegistrationService.GetRentRequestByRequestNumber(ID);
            decimal dues = _paymentEngine.GetBalanceDueTillDate((int)request.Rid, request.DepartmentId);
            request.TotalDues = dues;
            return View(request);
        }
        public ActionResult ManagePayment()
        {
            return View();
        }
        /// <summary>
        /// view rent request list for approver
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageRentRequest()
        {
            return View();
        }
        /// <summary>
        /// return rent permission information for approver to approve/reject
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RentRequest(string id)
        {
            int ID = Convert.ToInt32(CommonHelper.Decode(id));
            RentPermissionModel request = _propertyRegistrationService.GetRentRequestByRequestNumber(ID);
            decimal dues = _paymentEngine.GetBalanceDueTillDate((int)request.Rid, request.DepartmentId);
            request.TotalDues = dues;
            return View(request);
        }
        /// <summary>
        /// return all registration id of industry/institutional
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRIDsForRentPermission([DataSourceRequest] DataSourceRequest Req, int Rid)
        {
            var rids = _propertyRegistrationService.GetRIDsForRentPermission(Req, Rid);
            return Json(rids, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// return allottee details for rent permission
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        public JsonResult GetAllotteDetailsForRentPermission(int rid)
        {
            RentPermissionModel allotteeDetails = _propertyRegistrationService.GetAllotteDetailsForRentPermission(rid);
            //IPaymentEngine engine = new PaymentEngine();
            decimal dues = _paymentEngine.GetBalanceDueTillDate(rid, allotteeDetails.DepartmentId);
            allotteeDetails.TotalDues = dues;
            return Json(allotteeDetails);
        }

        public JsonResult IsRentingChargePaid(int rid)
        {
            var flag = false;
            flag = _paymentEngine.IsRentingChargePaid(rid);
            return Json(flag);
        }
        public JsonResult GetAllottedPropertyRidList()
        {
            List<DynamicDataModel> ridlist = _propertyRegistrationService.GetAllottedPropertyRidList();
            var JsonResult = Json(ridlist, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }

        public JsonResult GetAllottedPropertyRidListReq([DataSourceRequest] DataSourceRequest Req)
        {
            var ridlist = _propertyRegistrationService.GetAllottedPropertyRidList(Req);
            var JsonResult = Json(ridlist, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = int.MaxValue;
            return JsonResult;
        }

        public JsonResult GetAllottedPropertyDetail(int rid)
        {
            AllottedPropertyPaymentModel lst = _propertyRegistrationService.GetAllottedPropertyDetail(rid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPropertyPaymentType()
        {
            List<DynamicDataModel> ptlist = _propertyRegistrationService.GetPropertyPaymentType();
            return Json(ptlist, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPropertySubPaymentType(int receiptId)
        {
            List<DynamicDataModel> pstlist = _propertyRegistrationService.GetPropertySubPaymentType(receiptId);
            return Json(pstlist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddAllottedPropertyPayment(AllottedPropertyPaymentModel model)
        {
            bool flag = _propertyRegistrationService.AddAllottedPropertyPayment(model);
            //TempData["MesgAdd"] = flag.ToString();
            TempData["MesgAdd"] = flag;
            return RedirectToAction("ManagePayment");
            //return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPropertyPaymentDetails([DataSourceRequest]DataSourceRequest request)
        {
            var rentList = _propertyRegistrationService.GetPropertyPaymentDetails();
            var data = rentList.ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintDrawSlipForAuthority(int schemeId, int departmentId, List<string> nameList)
        {
            string parsedHTML = string.Empty;
            var requestModel = _propertyRegistrationService.GetApplicationFormDetailsForDrawSlip(schemeId, departmentId, nameList);

            return RedirectToAction("DrawSlipTemplate", requestModel);
        }

        public ActionResult DrawSlipTemplate(int schemeId, int departmentId, List<string> nameList)
        {
            string parsedHTML = string.Empty;
            var requestModel = _propertyRegistrationService.GetApplicationFormDetailsForDrawSlip(schemeId, departmentId, nameList);
            return View(requestModel);
        }

        /// <summary>
        /// return rent duration for rent permisssion
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRentDurationForRentPermission()
        {
            List<DDList> duration = _propertyRegistrationService.GetRentDurationForRentPermission();
            return Json(duration, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// update status of rent requests by user/approver
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="requestNo"></param>
        /// <param name="comment"></param>
        /// <param name="requestStatus"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public JsonResult UpdateStatusOfRentRequest(int rid, int requestNo, string comment, string requestStatus, string viewName)
        {
            var flag = false;
            flag = _propertyRegistrationService.UpdateStatusOfRentRequest(rid, requestNo, comment, requestStatus, viewName);
            RentPermissionModel request = _propertyRegistrationService.GetRentRequestByRequestNumber(requestNo);
            if (request.ServiceRequestRefNo != 0 && requestStatus == "Approved")
            {
                SendNICStatus((int)request.ServiceRequestRefNo,comment);
            }
            return Json(flag);
        }
        /// <summary>
        /// save rent permission information 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveRentPermissionRequest(RentPermissionModel model)
        {
            //var flag = false;
            //flag = _propertyRegistrationService.SaveRentPermissionRequest(model);
            //TempData["MesgAdd"] = flag;
            //if (flag)
            //{
            //    return RedirectToAction("ManageRentPermission");
            //}
            //else
            //{
            //    return RedirectToAction("AddRentRequest");
            //}
            var flag = _propertyRegistrationService.SaveRentPermissionRequest(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// return property type for rent permission
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPropertyTypeForRentPermission()
        {
            List<DDList> lst = _propertyRegistrationService.GetPropertyTypeForRentPermission();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// get approver for rent permission particular department
        /// </summary>
        /// <returns></returns>
        public JsonResult GetApproverForRentPermission([DataSourceRequest]DataSourceRequest request, int rid)
        {
            List<DDList> lst = _propertyRegistrationService.GetApproverForRentPermission(rid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fetches RIDs for Lease Deed from DB
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllRIDsForLeaseDeed()
        {
            var allRIDsForLeaseDeed = _propertyRegistrationService.GetAllRIDsForLeaseDeed();
            return Json(allRIDsForLeaseDeed, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SavePOReq(int rId, string userVal)
        {
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Reads Documents list for Kendo grid on the basis of Property type and Checklist type.
        /// Kendo's DataSourceRequest functionality has not been used as the number of documents would be less, hence its not required.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="propTypeId"></param>
        /// <param name="chkLstType"></param>
        /// <returns></returns>
        public JsonResult GetChcklstDocuments([DataSourceRequest]DataSourceRequest request, int propTypeId, int chkLstType)
        {
            var allDocs = _propertyRegistrationService.GetChcklstDocuments(request, propTypeId, chkLstType);
            var data = allDocs.ToDataSourceResult(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves/Updates details of Generate Checklist in DB
        /// </summary>
        /// <param name="rId">RID</param>
        /// <param name="chkType">Checklist Type</param>
        /// <param name="chkDate">Checklist Date</param>
        /// <returns></returns>
        public JsonResult SaveChecklistDetails(int rId, int chkType, DateTime chkDate, string viewName)
        {
            if (rId != 0 && chkType != 0 && chkDate != null)
            {
                var flag = _propertyRegistrationService.SaveChecklistDetails(rId, chkType, chkDate, viewName);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves Lease Deed details to DB
        /// </summary>
        /// <param name="rId">RID</param>
        /// <param name="LDExecDate">Lease Deed Execution Date</param>
        /// <param name="SA">Signatory Authority</param>
        /// <param name="w1N">Witness 1 Name</param>
        /// <param name="w1A">Witness 1 Address</param>
        /// <param name="w1M">Witness 1 Mobile</param>
        /// <param name="w2N">Witness 2 Name</param>
        /// <param name="w2A">Witness 2 Address</param>
        /// <param name="w2M">Witness 2 Mobile</param>
        /// <param name="w3N">Witness 3 Name</param>
        /// <param name="w3A">Witness 3 Address</param>
        /// <param name="w3M">Witness 3 Mobile</param>
        /// <param name="w4N">Witness 4 Name</param>
        /// <param name="w4A">Witness 4 Address</param>
        /// <param name="w4M">Witness 4 Mobile</param>
        /// <param name="LDDueDate">Lease Deed Due Date</param>
        /// <param name="regType">Registry Type</param>
        /// <param name="SDAmt">Stamp Duty Amount</param>
        /// <param name="DSDAmt">Duplicate Stamp Suty Amount</param>
        /// <param name="prevDues">Previous Dues</param>
        /// <param name="balDueTD">Balance Due till Date</param>
        /// <param name="viewName">View Name for Audit Trail</param>
        /// <param name="LeaseRent">LeaseRent</param>
        /// <param name="RevisedAfterYear">RevisedAfterYear</param>
        /// <param name="RevisedRate">RevisedRate</param>
        /// <returns></returns>
        public JsonResult SaveLeaseDeedDetails(int rId, DateTime LDExecDate, string SA, string w1N, string w1A, string w1M, string w2N, string w2A, string w2M, string w3N, string w3A, string w3M, string w4N, string w4A, string w4M, DateTime LDDueDate, string regType, decimal SDAmt, decimal DSDAmt, decimal prevDues, decimal balDueTD, string viewName, int type, decimal docCharges)
        {
            var flag = _propertyRegistrationService.SaveLeaseDeedDetails(rId, LDExecDate, SA, w1N, w1A, w1M, w2N, w2A, w2M, w3N, w3A, w3M, w4N, w4A, w4M, LDDueDate, regType, SDAmt, DSDAmt, prevDues, balDueTD, viewName, type, docCharges);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// View Lease Deed screen's View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ViewLeaseDeed(int id)
        {
            var leaseDeedDetails = _propertyRegistrationService.GetLeaseDeedDetailsByRId(id);
            if (leaseDeedDetails != null)
            {
                var tempObj = _paymentEngine.GetLeaseRentValue(leaseDeedDetails.RId, leaseDeedDetails.DepttId.Value, ChallanOptions.annualId);
                leaseDeedDetails.LeaseRent = tempObj.LeaseRentAmount;
            }
            return View(leaseDeedDetails);
        }

        /// <summary>
        /// Fetches Challan Options
        /// </summary>
        /// <returns></returns>
        public JsonResult GetChallanOptions()
        {
            var challanoptions = _generalService.GetChallanOptions();
            return Json(challanoptions, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// For printing Lease Deed Challan 
        /// </summary>
        /// <param name="id">RID</param>
        /// <returns></returns>
        public ActionResult PrintLeaseRentChallan(int id)
        {
            ChallanModel challanModel = new ChallanModel();
            var details = _propertyRegistrationService.GetDetailsByRID(id);
            challanModel = _allotmentService.PrintViewAllotment(details.PropId, details.SchemeId, details.DepttID);
            //challanModel.LeaseRentAmount = 12312.12M;
            //challanModel.Dues = 1200.00M;
            //challanModel.TotalDue = _allotmentService.GetLeaseRentDuesTillDate(id, details.DepttID);
            return View(challanModel);
        }

        /// <summary>
        /// Returns ManageMutation screen View
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageMutation()
        {
            return View();
        }

        /// <summary>
        /// For reading grid data on ManageMutation screen from DB for Requestor
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public JsonResult GetMutationData([DataSourceRequest] DataSourceRequest req)
        {
            var mutationLst = _propertyRegistrationService.GetMutationData(req);
            return Json(mutationLst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// For reading grid data on ManageMutation screen from DB for Approver
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public JsonResult GetMutationDataByApproverId([DataSourceRequest] DataSourceRequest req)
        {
            var mutationLst = _propertyRegistrationService.GetMutationDataByApproverId(req);
            return Json(mutationLst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add Mutation screen's View
        /// </summary>
        /// <returns></returns>
        public ActionResult AddMutationRequest()
        {
            return View();
        }

        public MutationModel GetMutationFiles(MutationModel data)
        {
            DirectoryInfo meterSealingDirectory = null;
            string meterSealingPath = ConfigurationManager.AppSettings["UploadFilePath"] + data.RId + "/" + ConfigurationManager.AppSettings["fileTransDeedPath"];
            meterSealingDirectory = new DirectoryInfo(meterSealingPath);
            if (Directory.Exists(ConfigurationManager.AppSettings["UploadFilePath"] + data.RId + "/" + ConfigurationManager.AppSettings["fileTransDeedPath"]))
                data.TransDeedFile = ConfigurationManager.AppSettings["UploadFileSitePath"] + data.RId + "/" + ConfigurationManager.AppSettings["fileTransDeedPath"] + "//" + meterSealingDirectory.GetFiles()[0].Name;
            return data;
        }

        /// <summary>
        /// Fetches data for View Mutation screen
        /// </summary>
        /// <param name="id">RID</param>
        /// <returns></returns>
        public ActionResult ViewMutationRequest(int id)
        {
            var data = new MutationModel();
            if (id != 0)
            {
                data = _propertyRegistrationService.GetMutationDetailsByRId(id);
                data = GetMutationFiles(data);
            }
            return View(data);
        }

        /// <summary>
        /// Returns View for Manage Mutation screen for Approver
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageMutationApprover()
        {
            return View();
        }

        /// <summary>
        /// Grid read for Manage Mutation screen for Approver
        /// </summary>
        /// <param name="id">RID</param>
        /// <returns></returns>
        public ActionResult ViewMutationRequestApprover(int id)
        {
            var data = new MutationModel();
            if (id != 0)
            {
                data = _propertyRegistrationService.GetMutationDetailsByRId(id);
                data = GetMutationFiles(data);
            }
            return View(data);
        }

        /// <summary>
        /// Saves Mutation Details from AddMutationRequest screen to DB
        /// </summary>
        /// <param name="mutDate">Mutation Date</param>
        /// <param name="transDeedDate">Transfer Deed Date</param>
        /// <param name="bahiNo">Bahi No</param>
        /// <param name="bahiZildNo">Bahi Zild No</param>
        /// <param name="bahiPageNo">Bahi Page No</param>
        /// <param name="SINo">SI No</param>
        /// <param name="userVal">Approver ID</param>
        /// <returns></returns>
        //public JsonResult SaveMutationDetails(DateTime mutDate, DateTime transDeedDate, string bahiNo, string bahiZildNo, string bahiPageNo, string SINo, int userVal, int ReqNo, int rId, DateTime transDate, string transType)
        //{
        //    var flag = _propertyRegistrationService.SaveMutationDetails(mutDate, transDeedDate, bahiNo, bahiZildNo, bahiPageNo, SINo, userVal, ReqNo, rId, transDate, transType);
        //    return Json(flag, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult SaveMutationDetails()
        {
            DateTime mutDate = Convert.ToDateTime(Request["mutDate"]);
            DateTime transDeedDate = Convert.ToDateTime(Request["transDeedDate"]);
            string bahiNo = Request["bahiNo"];
            string bahiZildNo = Request["bahiZildNo"];
            string bahiPageNo = Request["bahiPageNo"];
            string SINo = Request["SINo"];
            int userVal = Convert.ToInt32(Request["userVal"]);
            int ReqNo = Convert.ToInt32(Request["ReqNo"]);
            int rId = Convert.ToInt32(Request["rId"]);
            DateTime transDate = Convert.ToDateTime(Request["transDate"]);
            string transType = Request["transType"];
            int ReqRefNo = Request["RequestReferenceNo"] != null ? Convert.ToInt32(Request["RequestReferenceNo"]) : 0;
            var flag = _propertyRegistrationService.SaveMutationDetails(mutDate, transDeedDate, bahiNo, bahiZildNo, bahiPageNo, SINo, userVal, ReqNo, rId, transDate, transType, ReqRefNo);
            if (flag == true)
            {
                //Save Lease Deed doc
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    string fileDBPath = null;
                    var hpf = Request.Files[0];
                    if (hpf != null && hpf.ContentLength > 0)
                    {
                        var folderName = Request.Files.AllKeys[i];
                        var fileName = new FileInfo(hpf.FileName).Name;
                        fileName = fileName.Replace(" ", "");
                        fileDBPath = fileName;
                        if (!Directory.Exists(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName))
                        {
                            Directory.CreateDirectory(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName);
                        }
                        else
                        {
                            foreach (FileInfo file in (Directory.CreateDirectory(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName).GetFiles()))
                            {
                                file.Delete();
                            }
                        }
                        var fileSavePath = ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName + "/" + fileName;
                        hpf.SaveAs(fileSavePath);
                    }
                }
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Updates Mutation details for an existing Mutation
        /// </summary>
        /// <param name="mutDate"></param>
        /// <param name="transDeedDate"></param>
        /// <param name="bahiNo"></param>
        /// <param name="bahiZildNo"></param>
        /// <param name="bahiPageNo"></param>
        /// <param name="SINo"></param>
        /// <param name="userVal"></param>
        /// <param name="ReqNo"></param>
        /// <param name="rId"></param>
        /// <returns></returns>
        public JsonResult UpdateMutationDetails()
        {
            DateTime mutDate = Convert.ToDateTime(Request["mutDate"]);
            DateTime transDeedDate = Convert.ToDateTime(Request["transDeedDate"]);
            string bahiNo = Request["bahiNo"];
            string bahiZildNo = Request["bahiZildNo"];
            string bahiPageNo = Request["bahiPageNo"];
            string SINo = Request["SINo"];
            int userVal = Convert.ToInt32(Request["userVal"]);
            int ReqNo = Convert.ToInt32(Request["ReqNo"]);
            int rId = Convert.ToInt32(Request["rId"]);

            var flag = _propertyRegistrationService.UpdateMutationDetails(mutDate, transDeedDate, bahiNo, bahiZildNo, bahiPageNo, SINo, userVal, ReqNo, rId);
            if (flag == true)
            {
                if (Request.Files.Count > 0)
                {
                    if (Directory.Exists(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + ConfigurationManager.AppSettings["fileTransDeedPath"]))
                    {
                        foreach (FileInfo file in (Directory.CreateDirectory(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + ConfigurationManager.AppSettings["fileTransDeedPath"]).GetFiles()))
                        {
                            file.Delete();
                        }
                        Directory.Delete(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + ConfigurationManager.AppSettings["fileTransDeedPath"]);
                    }
                }

                string fileDBPath = null;

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var hpf = Request.Files[i];
                    if (hpf != null && hpf.ContentLength > 0)
                    {
                        var folderName = Request.Files.AllKeys[i];
                        var fileName = new FileInfo(hpf.FileName).Name;
                        fileName = fileName.Replace(" ", "");
                        fileDBPath = fileName;
                        if (!Directory.Exists(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName))
                        {
                            Directory.CreateDirectory(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName);
                        }
                        else
                        {
                            foreach (FileInfo file in (Directory.CreateDirectory(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName).GetFiles()))
                            {
                                file.Delete();
                            }
                        }
                        var fileSavePath = ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName + "/" + fileName;
                        hpf.SaveAs(fileSavePath);

                    }
                }
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fills RID DDL according to BRs and validations applied in Mutation 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDsForMutation([DataSourceRequest] DataSourceRequest Req)
        {
            var RIdlst = _propertyRegistrationService.GetRIDsForMutation(Req);
            return Json(RIdlst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fetches Transfer Details by RID
        /// </summary>
        /// <param name="rId"></param>
        /// <returns></returns>
        public JsonResult GetTransferDetailsByRID(int rId)
        {
            if (rId != 0)
            {
                var details = _propertyRegistrationService.GetTransferDetailsByRID(rId);
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Sets Mutation Status to "Cancel"
        /// </summary>
        /// <param name="reqNo"></param>
        /// <returns></returns>
        public JsonResult CancelMutationRequest(int reqNo)
        {
            if (reqNo != 0)
            {
                var flag = _propertyRegistrationService.CancelMutationRequest(reqNo);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Checks for Checklist generation
        /// </summary>
        /// <param name="rId"></param>
        /// <returns></returns>
        public JsonResult IsChecklistGenerated(int rId)
        {
            if (rId != 0)
            {
                var flag = _propertyRegistrationService.IsChecklistGenerated(rId);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves Mutation -> Approver Status and Comments in DB
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="intStatus">= 1 for Approved; = 2 for Rejected</param>
        /// <returns></returns>
        public JsonResult SaveApprovalStatus(string comments, int intStatus, int ReqNo)
        {
            if (intStatus != 0)
            {
                var flag = _propertyRegistrationService.SaveApprovalStatus(comments, intStatus, ReqNo);
                var data = _propertyRegistrationService.GetMutationDetailsByRId(ReqNo);
                if (intStatus == 1 && data.OnlineRequestRefNo != 0)
                {
                    SendNICStatus((int)data.OnlineRequestRefNo,comments);
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns Users for assigning Requests
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAssineTo()
        {
            var lst = _allotmentService.GetAssineTo();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns Users for assigning Requests by data's Department ID
        /// </summary>
        /// <param name="depttId">Department ID</param>
        /// <returns></returns>
        public JsonResult GetAssineToByDepttId(int depttId)
        {
            var lst = _generalService.GetAssineToByDepttId(depttId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChklstDateByChklstType(int rId, int type)
        {
            var isNull = false;
            var chklstDate = _propertyRegistrationService.GetChklstDateByChklstType(rId, type);
            if (chklstDate == null)
                isNull = true;
            return Json(new { date = chklstDate, isN = isNull }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Return to Manage Mortage Screen.
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageMortgage()
        {
            return View();
        }
        /// <summary>
        /// To Get All Mortgage Records to fill Grid.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public JsonResult GetAllMortgage([DataSourceRequest]DataSourceRequest req)
        {
            var mortgageLst = _propertyRegistrationService.GetAllMortgage(req);
            return Json(mortgageLst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllMortgageByRid([DataSourceRequest]DataSourceRequest req, int Rid)
        {
            var mortgageLst = _propertyRegistrationService.GetAllMortgageByRid(req, Rid);
            return Json(mortgageLst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// To return blank Mortgage View.
        /// </summary>
        /// <returns></returns>
        public ActionResult AddMortgage()
        {
            return View();
        }
        /// <summary>
        /// To Fill MortgageType Drop Down.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMortgageType()
        {
            var lstMortgageType = _propertyRegistrationService.GetMortgageType();
            return Json(lstMortgageType, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// To fill Previous Loan Noc Drop down.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPreviousLoanNoc()
        {
            var lstPreviousLoanNoc = _propertyRegistrationService.GetMortgagePrevLoan();
            return Json(lstPreviousLoanNoc, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyDetailByRid(int rID)
        {
            var lst = _propertyRegistrationService.GetPropertyDetailByRid(rID);
            //IPaymentEngine _paymentEngine = new PaymentEngine();            
            var totalDueBal = _paymentEngine.GetBalanceDueTillDate(rID, lst.DepartmentId.Value);
            lst.TotalDues = totalDueBal;
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// To Save or Add new Mortgate.
        /// </summary>
        /// <param name="mortgageModel"></param>
        /// <returns></returns>       
        public ActionResult SaveMortgage(DateTime mortgageDate, string bankName, string mortgageType, short previousLoanNoc, string branchAddress, decimal processingFee, decimal sanctionedAmount, string user, int rid, DateTime validUpto, int? reqRefNo)
        {
            var lst = _propertyRegistrationService.AddMortgage(mortgageDate, bankName, mortgageType, previousLoanNoc, branchAddress, processingFee, sanctionedAmount, user, rid, validUpto, reqRefNo);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Screen is used to for Approval.
        /// </summary>
        /// <returns></returns>
        public ActionResult ApprovalMortgage()
        {
            return View();
        }

        public JsonResult GetAllMortgageByApprover([DataSourceRequest]DataSourceRequest req)
        {
            var mortgageByApproverLst = _propertyRegistrationService.GetAllMortgageByApprover(req);
            return Json(mortgageByApproverLst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get Mortgate by Request id to Approve or reject property.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MortgageRequestApproval(string id)
        {
            int mortgageID = Convert.ToInt32(CommonHelper.Decode(id));
            MortgageModel mortgageModel = new MortgageModel();
            if (mortgageID != 0)
            {
                mortgageModel = _propertyRegistrationService.GetMortgageByRequestID(mortgageID);
            }
            return View(mortgageModel);
        }

        public JsonResult GetMortgagePreviousLoan(int? requestNo, int? Rid)
        {
            var data = _propertyRegistrationService.GetMortgagePreviousLoanDetails(requestNo, Rid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// // To Save comment of approver.
        /// </summary>
        /// <param name="requestNo"></param>
        /// <param name="Comment"></param>
        /// <returns></returns>
        public ActionResult SaveCommentByRequestID(int requestNo, string Comment, bool acceptReject)
        {
            var lst = _propertyRegistrationService.SaveCommentByRequestID(requestNo, Comment, acceptReject);
            var mortgageModel = _propertyRegistrationService.GetMortgageByRequestID(requestNo);
            if (acceptReject == true && mortgageModel.OnlineRequestNo != 0)
            {
                SendNICStatus((int)mortgageModel.OnlineRequestNo, Comment);
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MortgageRequestStatus(string id)
        {
            int requestID = Convert.ToInt32(CommonHelper.Decode(id));
            MortgageModel mortgageModel = new MortgageModel();
            if (requestID != 0)
            {
                mortgageModel = _propertyRegistrationService.GetMortgageByRequestID(requestID);
            }
            return View(mortgageModel);
        }
        /// <summary>
        /// To Re-Sumbit Mortgage request or Update request.
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="mortgageDate"></param>
        /// <param name="bankName"></param>
        /// <param name="mortgageType"></param>
        /// <param name="previousLoanNoc"></param>
        /// <param name="branchAddress"></param>
        /// <param name="processingFee"></param>
        /// <param name="user"></param>
        /// <param name="rid"></param>
        /// <returns></returns>
        public ActionResult UpdateMortgage(int requestID, DateTime mortgageDate, string bankName, string mortgageType, short previousLoanNoc, string branchAddress, decimal processingFee, decimal sanctionedAmount, string user, int rid)
        {
            var lst = _propertyRegistrationService.UpdateMortgage(requestID, mortgageDate, bankName, mortgageType, previousLoanNoc, branchAddress, processingFee, sanctionedAmount, user, rid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// To Cancel Request by ID.
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public ActionResult CancelMortgageRequest(int requestID)
        {
            var lst = _propertyRegistrationService.CancelMortgageRequest(requestID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        // To Invalida Mortgate
        public ActionResult InValidMortgageRequest(int requestNo, string Comment)
        {
            var lst = _propertyRegistrationService.InValidMortgageRequestID(requestNo, Comment);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        // To Generate Mortgage letter
        public ActionResult GenerateMortgageLetter(int rid)
        {
            var lst = _propertyRegistrationService.GenerateMortgageLetter(rid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        // To Generate Rent Permission letter
        public ActionResult GenerateRentPermissionLetter(int requestNo, int rid)
        {
            var lst = _propertyRegistrationService.GenerateRentPermissionLetter(requestNo, rid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        // To Generate Completion letter
        public ActionResult GenerateCompletionLetter(int rid, int deptId)
        {
            var lst = _generalService.GenerateLetterFromDb(rid, Convert.ToInt32(LetterTemplate.CompletionLetter), deptId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GenerateDemandLetters(List<int> rIds)
        {
            string content = string.Empty;
            foreach (var item in rIds)
            {
                content += _propertyRegistrationService.GenerateDemandLetters(item);
                content += "<div style='page-break-after: always;' ></div>";
            }
            return Json(content, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateDemandLetter()
        {
            return View();
        }

        public JsonResult GetDemandLetterDataByFilter([DataSourceRequest] DataSourceRequest req, int? deptt, int? sector, int? block, int type)
        {
            var data = _propertyRegistrationService.GetDemandLetterDataByFilter(req, deptt, sector, block, type);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBlocks()
        {
            var lst = _generalService.GetAllBlocks();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSectorsByPropType()
        {
            var lst = _generalService.GetSectorsByPropType();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        #region "Manage Allotee List "

        /// <summary>
        /// Returns View for ManageLeaseDeed screen
        /// </summary>
        /// <returns></returns>

        public ActionResult ManageFunctional()
        {
            return View();
        }
        //  <summary>
        // Get all funtional property list
        // </summary>
        public ActionResult GetPropFunctional([DataSourceRequest] DataSourceRequest req)
        {
            var objFunctional = _propertyRegistrationService.GetPropFunctional(req);

            return Json(objFunctional, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Fetches data for View Mutation screen
        /// </summary>
        /// <param name="id">RID</param>
        /// <returns></returns>
        public ActionResult ViewFunctionalDetails(int id)
        {
            var data = new FunctionalModel();
            if (id != 0)
            {
                data = _propertyRegistrationService.GetFunctionalDetailsByReqNo(id);
                data = GetFiles(data);
            }
            return View(data);
        }

        //public ActionResult ViewFunctionalDetails(int id)
        //{
        //    var FunctionalDetails = _propertyRegistrationService.GetFunctionalDetailsByReqNo(id);
        //    return View(FunctionalDetails);
        //}

        /// <summary>
        /// Add Mutation screen's View
        /// </summary>
        /// <returns></returns>
        public ActionResult AddFunctionalDetails()
        {
            return View();
        }

        /// <summary>
        /// Saves Mutation Details from AddMutationRequest screen to DB
        /// </summary>
        /// <param name="mutDate">Mutation Date</param>
        /// <param name="transDeedDate">Transfer Deed Date</param>
        /// <param name="bahiNo">Bahi No</param>
        /// <param name="bahiZildNo">Bahi Zild No</param>
        /// <param name="bahiPageNo">Bahi Page No</param>
        /// <param name="SINo">SI No</param>
        /// <param name="userVal">Approver ID</param>
        /// <returns></returns>
        //public JsonResult SaveFunctionalDetails(int rId, bool MeterSealing, bool Affidavit, bool RegistrationCertificate, bool NDCAccount, string user, string PropertyNumber, DateTime? FunctionalDate, decimal? FunctionalCharge, DateTime? FunctionalDueDate)
        public JsonResult SaveFunctionalDetails()
        {
            //user.User_Role_Id = Convert.ToInt32(Request["userRoleID"]);
            int rId = Convert.ToInt32(Request["rId"]);
            bool MeterSealing = Convert.ToBoolean(Request["MeterSealing"]);
            bool Affidavit = Convert.ToBoolean(Request["Affidavit"]);
            bool RegistrationCertificate = Convert.ToBoolean(Request["RegistrationCertificate"]);
            bool NDCAccount = Convert.ToBoolean(Request["NDCAccount"]);
            string user = Request["user"];
            string PropertyNumber = Request["PropertyNumber"];
            DateTime? FunctionalDueDate = Convert.ToDateTime(Request["FunctionalDueDate"]);
            Decimal FunctionalCharge;
            DateTime? FunctionalDate = null;
            Nullable<DateTime> CompletionDate = null;
            Nullable<DateTime> AffidavitDate = null;
            if (Request["FunctionalDate"] != "")
            {
                FunctionalDate = Convert.ToDateTime(Request["FunctionalDate"]);
            }
            if (Request["CompletionDate"] != "")
            {
                CompletionDate = Convert.ToDateTime(Request["CompletionDate"]);
            }
            if (Request["AffidavitDate"] != "")
            {
                AffidavitDate = Convert.ToDateTime(Request["AffidavitDate"]);
            }
            if (Request["FunctionalCharge"] != "")
            {
                FunctionalCharge = Convert.ToDecimal(Request["FunctionalCharge"]);
            }
            else
            {
                FunctionalCharge = 0;
            }

            int ReqRefNo = 0;
            if (Request["ReqRefNo"] != "")
            {
                ReqRefNo = Convert.ToInt32(Request["ReqRefNo"]);
            }


            var flag = _propertyRegistrationService.SaveFunctionalDetails(rId, FunctionalDate, FunctionalDueDate, MeterSealing, Affidavit, RegistrationCertificate, NDCAccount, user, PropertyNumber, FunctionalCharge, ReqRefNo, CompletionDate, AffidavitDate);
            if (flag == 1)
            {
                string fileDBPath = null;

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var hpf = Request.Files[i];
                    if (hpf != null && hpf.ContentLength > 0)
                    {
                        var folderName = Request.Files.AllKeys[i];
                        var fileName = new FileInfo(hpf.FileName).Name;
                        fileName = fileName.Replace(" ", "");
                        fileDBPath = fileName;
                        if (!Directory.Exists(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName))
                        {
                            Directory.CreateDirectory(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName);
                        }
                        else
                        {
                            foreach (FileInfo file in (Directory.CreateDirectory(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName).GetFiles()))
                            {
                                file.Delete();
                            }
                        }
                        var fileSavePath = ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName + "/" + fileName;
                        hpf.SaveAs(fileSavePath);

                    }
                }
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult SaveFunctionalDetails(int rId, bool MeterSealing, bool Affidavit, bool RegistrationCertificate, bool NDCAccount, string user, string PropertyNumber, DateTime? FunctionalDate, decimal? FunctionalCharge, DateTime? FunctionalDueDate)
        //public JsonResult ReSubmitFunctionalDetails(int rId, DateTime? FunctionalDate, DateTime? FunctionalDueDate, bool MeterSealing, bool Affidavit, bool RegistrationCertificate, bool NDCAccount, string user, string PropertyNumber, decimal? FunctionalCharge)
        public JsonResult ReSubmitFunctionalDetails()
        {
            int rId = Convert.ToInt32(Request["rId"]);
            bool MeterSealing = Convert.ToBoolean(Request["MeterSealing"]);
            bool Affidavit = Convert.ToBoolean(Request["Affidavit"]);
            bool RegistrationCertificate = Convert.ToBoolean(Request["RegistrationCertificate"]);
            bool NDCAccount = Convert.ToBoolean(Request["NDCAccount"]);
            string user = Request["user"];
            string PropertyNumber = Request["PropertyNumber"];
            DateTime? FunctionalDueDate = Convert.ToDateTime(Request["FunctionalDueDate"]);
            Decimal FunctionalCharge;
            DateTime? FunctionalDate = null;
            if (Request["FunctionalDate"] != "")
            {
                FunctionalDate = Convert.ToDateTime(Request["FunctionalDate"]);
            }
            if (Request["FunctionalCharge"] != "")
            {
                FunctionalCharge = Convert.ToDecimal(Request["FunctionalCharge"]);
            }
            else
            {
                FunctionalCharge = 0;
            }


            var flag = _propertyRegistrationService.ReSubmitFunctionalDetails(rId, FunctionalDate, FunctionalDueDate, MeterSealing, Affidavit, RegistrationCertificate, NDCAccount, user, PropertyNumber, FunctionalCharge);
            if (flag == 1)
            {
                if (MeterSealing == false)
                {
                    if (Directory.Exists(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + ConfigurationManager.AppSettings["fileMeterSealingPath"]))
                    {
                        foreach (FileInfo file in (Directory.CreateDirectory(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + ConfigurationManager.AppSettings["fileMeterSealingPath"]).GetFiles()))
                        {
                            file.Delete();
                        }
                        Directory.Delete(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + ConfigurationManager.AppSettings["fileMeterSealingPath"]);
                    }
                }
                if (Affidavit == false)
                {
                    if (Directory.Exists(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + ConfigurationManager.AppSettings["fileAffidavitPath"]))
                    {
                        foreach (FileInfo file in (Directory.CreateDirectory(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + ConfigurationManager.AppSettings["fileAffidavitPath"]).GetFiles()))
                        {
                            file.Delete();
                        }
                        Directory.Delete(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + ConfigurationManager.AppSettings["fileAffidavitPath"]);
                    }
                }
                if (RegistrationCertificate == false)
                {
                    if (Directory.Exists(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + ConfigurationManager.AppSettings["fileRegistrationCertificatePath"]))
                    {
                        foreach (FileInfo file in (Directory.CreateDirectory(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + ConfigurationManager.AppSettings["fileRegistrationCertificatePath"]).GetFiles()))
                        {
                            file.Delete();
                        }
                        Directory.Delete(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + ConfigurationManager.AppSettings["fileRegistrationCertificatePath"]);
                    }
                }
                if (NDCAccount == false)
                {
                    if (Directory.Exists(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + ConfigurationManager.AppSettings["fileNDCAccountPath"]))
                    {
                        foreach (FileInfo file in (Directory.CreateDirectory(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + ConfigurationManager.AppSettings["fileNDCAccountPath"]).GetFiles()))
                        {
                            file.Delete();
                        }
                        Directory.Delete(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + ConfigurationManager.AppSettings["fileNDCAccountPath"]);
                    }
                }
                string fileDBPath = null;

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var hpf = Request.Files[i];
                    if (hpf != null && hpf.ContentLength > 0)
                    {
                        var folderName = Request.Files.AllKeys[i];
                        var fileName = new FileInfo(hpf.FileName).Name;
                        fileName = fileName.Replace(" ", "");
                        fileDBPath = fileName;
                        if (!Directory.Exists(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName))
                        {
                            Directory.CreateDirectory(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName);
                        }
                        else
                        {
                            foreach (FileInfo file in (Directory.CreateDirectory(ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName).GetFiles()))
                            {
                                file.Delete();
                            }
                        }
                        var fileSavePath = ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + folderName + "/" + fileName;
                        hpf.SaveAs(fileSavePath);

                    }
                }
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Fills RID DDL according to BRs and validations applied in Mutation 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDsForFunctional([DataSourceRequest] DataSourceRequest Req, int Rid)
        {
            var RIdlst = _propertyRegistrationService.GetRIDsForFunctional(Req, Rid);
            return Json(RIdlst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRIDsForFunctional_Read([DataSourceRequest] DataSourceRequest Req, int? rid)
        {
            var RIdlst = _propertyRegistrationService.GetRIDsForFunctional_Read(Req, rid);
            return Json(RIdlst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Sets Functional Status to "Cancel"
        /// </summary>
        /// <param name="reqNo"></param>
        /// <returns></returns>
        public JsonResult CancelFunctionRequest(int reqNo)
        {
            if (reqNo != 0)
            {
                var flag = _propertyRegistrationService.CancelFunctionRequest(reqNo);
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns View for Manage Functional screen for Approver
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageFunctionalApprover()
        {
            return View();
        }
        /// <summary>
        /// For reading grid data on Functional screen from DB for Approver
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public JsonResult GetFunctionalDataByApproverId([DataSourceRequest] DataSourceRequest req)
        {
            var mutationLst = _propertyRegistrationService.GetFunctionalDataByApproverId(req);
            return Json(mutationLst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Grid read for Manage Mutation screen for Approver
        /// </summary>
        /// <param name="id">RID</param>
        /// <returns></returns>
        public ActionResult ViewFunctionalRequestApprover(int id)
        {
            var data = new FunctionalModel();
            if (id != 0)
            {
                data = _propertyRegistrationService.GetFunctionalDetailsByReqNo(id);
                data = GetFiles(data);
            }
            return View(data);
        }

        public FunctionalModel GetFiles(FunctionalModel data)
        {
            DirectoryInfo meterSealingDirectory = null;
            string meterSealingPath = ConfigurationManager.AppSettings["UploadFilePath"] + data.RId + "/" + ConfigurationManager.AppSettings["fileMeterSealingPath"];
            meterSealingDirectory = new DirectoryInfo(meterSealingPath);
            if (Directory.Exists(ConfigurationManager.AppSettings["UploadFilePath"] + data.RId + "/" + ConfigurationManager.AppSettings["fileMeterSealingPath"]))
                data.MeterSealingFile = ConfigurationManager.AppSettings["UploadFileSitePath"] + data.RId + "/" + ConfigurationManager.AppSettings["fileMeterSealingPath"] + "//" + meterSealingDirectory.GetFiles()[0].Name;

            DirectoryInfo registrationCertificateDirectory = null;
            string registrationCertificatePath = ConfigurationManager.AppSettings["UploadFilePath"] + data.RId + "/" + ConfigurationManager.AppSettings["fileRegistrationCertificatePath"];
            registrationCertificateDirectory = new DirectoryInfo(registrationCertificatePath);
            if (Directory.Exists(ConfigurationManager.AppSettings["UploadFilePath"] + data.RId + "/" + ConfigurationManager.AppSettings["fileRegistrationCertificatePath"]))
                data.RegistrationCertificateFile = ConfigurationManager.AppSettings["UploadFileSitePath"] + data.RId + "/" + ConfigurationManager.AppSettings["fileRegistrationCertificatePath"] + "//" + registrationCertificateDirectory.GetFiles()[0].Name;

            DirectoryInfo affidebitDirectory = null;
            string affidebitPath = ConfigurationManager.AppSettings["UploadFilePath"] + data.RId + "/" + ConfigurationManager.AppSettings["fileAffidavitPath"];
            affidebitDirectory = new DirectoryInfo(affidebitPath);
            if (Directory.Exists(ConfigurationManager.AppSettings["UploadFilePath"] + data.RId + "/" + ConfigurationManager.AppSettings["fileAffidavitPath"]))
                data.AffidebitFile = ConfigurationManager.AppSettings["UploadFileSitePath"] + data.RId + "/" + ConfigurationManager.AppSettings["fileAffidavitPath"] + "//" + affidebitDirectory.GetFiles()[0].Name;

            DirectoryInfo nDCAccountDirectory = null;
            string nDCAccountPath = ConfigurationManager.AppSettings["UploadFilePath"] + data.RId + "/" + ConfigurationManager.AppSettings["fileNDCAccountPath"];
            nDCAccountDirectory = new DirectoryInfo(nDCAccountPath);
            if (Directory.Exists(ConfigurationManager.AppSettings["UploadFilePath"] + data.RId + "/" + ConfigurationManager.AppSettings["fileNDCAccountPath"]))
                data.NDCAccountFile = ConfigurationManager.AppSettings["UploadFileSitePath"] + data.RId + "/" + ConfigurationManager.AppSettings["fileNDCAccountPath"] + "//" + nDCAccountDirectory.GetFiles()[0].Name;

            return data;
        }

        /// <summary>
        /// Saves Functional -> Approver Status and Comments in DB
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="intStatus">= 1 for Approved; = 2 for Rejected</param>
        /// <returns></returns>
        public JsonResult SaveFunctionalApprovalStatus(string comments, int intStatus, int ReqNo)
        {
            if (intStatus != 0)
            {
                var flag = _propertyRegistrationService.SaveFunctionalApprovalStatus(comments, intStatus, ReqNo);
                var data = _propertyRegistrationService.GetFunctionalDetailsByReqNo(ReqNo);
                if (intStatus == 1 && data.ReqRefNo != 0)
                {
                    SendNICStatus(data.ReqRefNo, comments);
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Fetches Transfer Details by RID
        /// </summary>
        /// <param name="rId"></param>
        /// <returns></returns>
        public JsonResult GetFunctionalTransferDetailsByRID(int rId)
        {
            if (rId != 0)
            {
                var details = _propertyRegistrationService.GetFUnctionalTransferDetailsByRID(rId);
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        // To Generate Functional Certificater
        public JsonResult GenerateFunctionalCertificater(int rid)
        {
            //var lst = _propertyRegistrationService.GenerateFunctionalCertificater(rid);
            var lst = _generalService.GenerateLetterFromDbByRId(rid, Constants.FunctionalCertificatID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Transfer functionalities
        /// <summary>
        /// Returns View for ManageTransfers screen 
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageTransfers()
        {
            return View();
        }

        /// <summary>
        /// Reads data for Kendo grid on ManageTransfers screen -> for Requestor
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public JsonResult GetTransferData([DataSourceRequest] DataSourceRequest req)
        {
            var transferLst = _propertyRegistrationService.GetTransferData(req);
            return Json(transferLst, JsonRequestBehavior.AllowGet);
        }

        // <summary>
        /// Reads data for Kendo grid on Transfer Hostory
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public JsonResult GetTransferHistoryDetail([DataSourceRequest] DataSourceRequest req, int? Rid)
        {
            var transferLst = _propertyRegistrationService.GetTransferData(req, Rid);
            return Json(transferLst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransferHistory([DataSourceRequest] DataSourceRequest req)
        {
            var lst = _propertyRegistrationService.GetTransferHistory(req);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TransferHistory()
        {
            return View();
        }

        /// <summary>
        /// Returns View for adding new Transfer Request
        /// </summary>
        /// <returns></returns>
        public ActionResult AddTransferRequest()
        {
            var obj = new TransferModel();
            return View(obj);
        }

        /// <summary>
        /// Fetches details of old owner and Property from DB
        /// </summary>
        /// <param name="rId">RID</param>
        /// <returns></returns>
        public JsonResult GetOriginalDetailsForTransferByRID(int rId)
        {
            if (rId != 0)
            {
                var details = _propertyRegistrationService.GetOriginalDetailsForTransferByRID(rId);
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Same as above function, only for Transfer
        /// </summary>
        /// <param name="rId"></param>
        /// <param name="reqNo"></param>
        /// <returns></returns>
        public JsonResult GetOriginalDetailsForTransferOnlyByRID(int rId, int reqNo)
        {
            if (rId != 0 && reqNo != 0)
            {
                var details = _propertyRegistrationService.GetOriginalDetailsForTransferOnlyByRID(rId, reqNo);
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fills RID DDL for Transfer Request (only those whose Lease Deed has already been executed).
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDsForTransfer([DataSourceRequest] DataSourceRequest Req)
        {
            var RIdlst = _propertyRegistrationService.GetRIDsForTransfer(Req);
            return Json(RIdlst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fills RID DDL for Transfer Request (only those whose Lease Deed has already been executed).
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDsForTransferForView([DataSourceRequest] DataSourceRequest Req, int Rid)
        {
            var RIdlst = _propertyRegistrationService.GetRIDsForTransfer(Req, Rid);
            return Json(RIdlst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Will get Transfer Types from DB
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTransferTypes()
        {
            var lst = _propertyRegistrationService.GetTransferTypes();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Will get Sub-transfer Types from DB on the basis of Transfer Type
        /// </summary>
        /// <param name="TransferType">Transfer Type ID</param>
        /// <returns></returns>
        public JsonResult GetTransferSubTypes(int TransferType)
        {
            var lst = _propertyRegistrationService.GetTransferSubTypes(TransferType);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransfereeDetailByReqID(int Id)
        {
            var result = _propertyRegistrationService.GetTransfereeDetailByReqID(Id);
            return View(result);
        }

        //Saves Transfer related details in DB
        public JsonResult SaveTransferData(int rId, int propId, string applicantName, string applicantGender, string applicantRelativeName, string applicantPropType, decimal Area, string floor, int userVal, int transType, int transSubType, DateTime transDate, decimal transChargePerSqMrt, decimal totTransCharge, string transfereeGender, string transfereeCompanyName, string transfereeFrstName, string trasfereeSignAuth, string transfereeMiddleName, string transfereeLstName, string transfereeCompanyRegOfc, string TransfereeRelName, string TransfereeMotherName, string TransfereeMobile, string TransfereeEmail, string TransfereeCorrAdd, string TransfereePerAdd, string TransfereePAN, int? TransfereeOccupation, int? reqNo, string compName, string signAuth, int? OnlineReqRefNo, string transferorCorrAdd, string transferorPerAdd, decimal? CurrentPropertyRate, decimal? LocationCharge, decimal? TotalPropertyCost, decimal? AnnualLeaseRent)
        {
            var rslt = _propertyRegistrationService.SaveTransferData(rId, propId, applicantName, applicantGender, applicantRelativeName, applicantPropType, Area, floor, userVal, transType, transSubType, transDate, transChargePerSqMrt, totTransCharge, transfereeGender, transfereeCompanyName, transfereeFrstName, trasfereeSignAuth, transfereeMiddleName, transfereeLstName, transfereeCompanyRegOfc, TransfereeRelName, TransfereeMotherName, TransfereeMobile, TransfereeEmail, TransfereeCorrAdd, TransfereePerAdd, TransfereePAN, TransfereeOccupation, reqNo, compName, signAuth, OnlineReqRefNo, transferorCorrAdd, transferorPerAdd, CurrentPropertyRate, LocationCharge, TotalPropertyCost, AnnualLeaseRent);
            return Json(rslt, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns View for ManageTransfers screen -> for Approver
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageTransfers_Approver()
        {
            return View();
        }

        /// <summary>
        /// Reads data for Kendo grid on ManageTransfers screen -> for Approver
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public JsonResult GetTransferData_Approver([DataSourceRequest] DataSourceRequest req)
        {
            var transferLst = _propertyRegistrationService.GetTransferData_Approver(req);
            return Json(transferLst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns View for Approver to view Transfer details
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult ViewTransferDetails_Approver(int Id)
        {
            var transDetails = _propertyRegistrationService.GetTransfereeDetailByReqID(Id);
            return View(transDetails);
        }
         
        /// <summary>
        /// Saves Approval Status to DB
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="intStatus"></param>
        /// <param name="ReqNo"></param>
        /// <returns></returns>
        public JsonResult SaveTransferApprovalStatus(string comments, int intStatus, int ReqNo)
        {
            var rslt = _propertyRegistrationService.SaveTransferApprovalStatus(comments, intStatus, ReqNo);
            var transDetails = _propertyRegistrationService.GetTransfereeDetailByReqID(ReqNo);
            if (intStatus == 1 && transDetails.ReqRefNo != 0)
            {
                SendNICStatus(transDetails.ReqRefNo,comments);
            }
            return Json(rslt, JsonRequestBehavior.AllowGet);
        }


        public int SendNICStatus(int ReqNo,string comment)
        {
            var flag = ReturnType.None;
            CurrentUserDetail user = Session["CurrentUser"] as CurrentUserDetail;
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

        /// <summary>
        /// Cancels Transfer Request
        /// </summary>
        /// <param name="requestID">Reqeust Number</param>
        /// <returns></returns>
        public ActionResult CancelTransferRequest(int requestID)
        {
            var lst = _propertyRegistrationService.CancelTransferRequest(requestID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "Manage Building Plan"

        /// <summary>
        /// Returns View for ManageLeaseDeed screen
        /// </summary>
        /// <returns></returns>

        public ActionResult ManageBuildingPlan()
        {
            return View();
        }
        //  <summary>
        // Get all funtional property list
        // </summary>
        public ActionResult GetBuildingPlan([DataSourceRequest] DataSourceRequest request)
        {
            var dataresult = _propertyRegistrationService.GetBuildingPlan(request);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Add Building screen's
        /// </summary>
        /// <returns></returns>
        public ActionResult AddBuildingPlan()
        {
            return View();
        }
        public JsonResult GetBuildingPlanByRId(int rId)
        {
            var buildingPlanDetail = _propertyRegistrationService.GetBuildingPlanByRId(rId);
            return Json(buildingPlanDetail, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveBuildingPlan1(int rId, string BuildingPlanFileNo, int? PropertyUse, DateTime? DateOfSubmission, DateTime? DateOfSanction, decimal? PurchasableFar, DateTime? SanctionPlanValidity, string MapReleased, string MapRevised, decimal? PloatArea, decimal? CoveredArea, double? FloorAreasRation, int NumberOfStories, string ArchitectRegNo, string NameOfArchitect)
        {
            //var flag = null;
            var flag = _propertyRegistrationService.SaveBuildingPlan1(rId, BuildingPlanFileNo, PropertyUse, DateOfSubmission, DateOfSanction, PurchasableFar, SanctionPlanValidity, MapReleased, MapRevised, PloatArea, CoveredArea, FloorAreasRation, NumberOfStories, ArchitectRegNo, NameOfArchitect);

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewBuildingPlan(int id)
        {
            BuildingPlanModel detailedBuildingPlan = new BuildingPlanModel();
            //if (rId != 0)
            //{
            detailedBuildingPlan = _propertyRegistrationService.GetBuildingPlanDetailsByRId(id);
            //}
            return View(detailedBuildingPlan);

        }
        /// <summary>
        /// Fills RID DDL according to BRs and validations applied in Building Plan 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDs(string type)
        {
            var RIdlst = _generalService.GetRIDs(type);
            return Json(RIdlst, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetBuildingPlanDetailsByRId(int rId)
        //{
        //    DetailedBuildingPlan detailedBuildingPlan = new DetailedBuildingPlan();
        //    if (rId != 0)
        //    {
        //        detailedBuildingPlan = _propertyRegistrationService.GetBuildingPlanDetailsByRId(76);
        //    }
        //    return View(detailedBuildingPlan);

        //    //var buildingPlanDetail = _propertyRegistrationService.GetBuildingPlanDetailsByRId(76);
        //    //return Json(buildingPlanDetail, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetBuildingDetail([DataSourceRequest] DataSourceRequest request, int rid)
        {
            var dataresult = _propertyRegistrationService.GetBuildingDetail(request, rid);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetChargesDemanded([DataSourceRequest] DataSourceRequest request, int rid)
        {
            var dataresult = _propertyRegistrationService.GetChargesDemanded(request, rid);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNOCDetail([DataSourceRequest] DataSourceRequest request, int rid)
        {
            var dataresult = _propertyRegistrationService.GetNOCDetail(request, rid);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// return property type for rent permission
        /// </summary>
        /// <returns></returns>
        public JsonResult GetChargesType()
        {
            List<DDList> lst = _propertyRegistrationService.GetChargesType();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNOCType()
        {
            List<DDList> lst = _propertyRegistrationService.GetNOCType();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyUse()
        {
            List<DDList> lst = _propertyRegistrationService.GetPropertyUse();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveBuildingDetails(int rId, string NameOfTower, decimal? CoveredAreaMultiFloors, decimal? BuildingHeight, decimal? CoveredAreaGroundFloor, int? NumberOfStories2)
        {
            //var flag = null;
            var flag = _propertyRegistrationService.SaveBuildingDetails(rId, NameOfTower, CoveredAreaMultiFloors, BuildingHeight, CoveredAreaGroundFloor, NumberOfStories2);

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveBuildingChange(int rId, string NameOfTower, decimal? CoveredAreaMultiFloors, decimal? BuildingHeight, decimal? CoveredAreaGroundFloor, int? NumberOfStories2)
        {
            //var flag = null;
            var flag = _propertyRegistrationService.SaveBuildingDetails(rId, NameOfTower, CoveredAreaMultiFloors, BuildingHeight, CoveredAreaGroundFloor, NumberOfStories2);

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveBuildingCharge(int rId, int Charges, decimal? FeeAmount)
        {
            var flag = _propertyRegistrationService.SaveBuildingCharge(rId, Charges, FeeAmount);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveNocType(int rId, int NocType, string Submitted)
        {
            var flag = _propertyRegistrationService.SaveNocType(rId, NocType, Submitted);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        //  To Remove cost record by id.
        public JsonResult RemoveBuildingDetails(int RequestNo, int rId)
        {
            var data = _propertyRegistrationService.RemoveBuildingDetails(RequestNo, rId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoveNocType(int RequestNo, int rId)
        {
            var data = _propertyRegistrationService.RemoveNocType(RequestNo, rId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoveBPCharges(int RequestNo, int rId)
        {
            var data = _propertyRegistrationService.RemoveBPCharges(RequestNo, rId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Edit(int id)
        {
            BuildingPlanModel detailedBuildingPlan = new BuildingPlanModel();
            detailedBuildingPlan = _propertyRegistrationService.GetBuildingPlanDetailsByRId(id);
            return View(detailedBuildingPlan);
        }

        public JsonResult UpdateBuildingPlan1(int rId, string BuildingPlanFileNo, int? PropertyUse, DateTime? DateOfSubmission, DateTime? DateOfSanction, decimal? PurchasableFar, DateTime? SanctionPlanValidity, string MapReleased, string MapRevised, decimal? PloatArea, decimal? CoveredArea, double? FloorAreasRation, int NumberOfStories, string ArchitectRegNo, string NameOfArchitect)
        {
            var flag = _propertyRegistrationService.UpdateBuildingPlan1(rId, BuildingPlanFileNo, PropertyUse, DateOfSubmission, DateOfSanction, PurchasableFar, SanctionPlanValidity, MapReleased, MapRevised, PloatArea, CoveredArea, FloorAreasRation, NumberOfStories, ArchitectRegNo, NameOfArchitect);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SubmitBuildingPlan(int rId)
        {
            var flag = _propertyRegistrationService.SubmitBuildingPlan(rId);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GPA Module

        /// <summary>
        /// Returns View for ManageGPA page
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageGPA()
        {
            return View();
        }

        /// <summary>
        /// Reads grid data on ManageGPA screen
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public JsonResult GetAllGPAData([DataSourceRequest] DataSourceRequest req)
        {
            var GPALst = _propertyRegistrationService.GetAllGPAData(req);
            return Json(GPALst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// To Activate/Deactivate GPA record
        /// </summary>
        /// <param name="id">GPA ID</param>
        /// <param name="isActive">Current status</param>
        /// <returns></returns>
        public JsonResult ActivateDeactivateToggle(int id, bool isActive)
        {
            var flag = _propertyRegistrationService.ActivateDeactivateToggle(id, isActive);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns View for AddGPA screen
        /// </summary>
        /// <returns></returns>
        public ActionResult AddGPA()
        {
            return View();
        }

        /// <summary>
        /// Used for saving GPA details in DB
        /// </summary>
        /// <param name="GPA">Model object with GPA details</param>
        /// <returns></returns>
        public ActionResult SaveGPANominee(GPAModel GPA)
        {
            var flag = _propertyRegistrationService.SaveGPANominee(GPA);
            return RedirectToAction("ManageGPA");
        }

        /// <summary>
        /// Dropdown Read
        /// </summary>
        /// <returns></returns>
        public JsonResult GetGPAType(string company)
        {
            var lst = _propertyRegistrationService.GetGPAType(company);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Reads Nominee grid data based on RID value
        /// </summary>
        /// <param name="rid">RID</param>
        /// <returns></returns>
        public JsonResult GetNomineeData(int rid, [DataSourceRequest] DataSourceRequest req)
        {
            var nominees = _propertyRegistrationService.GetNomineeData(rid, req);
            return Json(nominees, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddNominee(int rid, string nomName, string relation, DateTime nomDate)
        {
            var flag = _propertyRegistrationService.AddNominee(rid, nomName, relation, nomDate);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveNominee(int id)
        {
            var flag = _propertyRegistrationService.RemoveNominee(id);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditGPA()
        {
            var rId = Convert.ToInt32(Request.QueryString["RId"]);
            var type = Request.QueryString["Type"];
            var obj = new GPAModel();
            if (type.ToLower() == Common.GPAType.GPA.ToString().ToLower())
            {
                obj = _propertyRegistrationService.GetGPADetailsByRId(rId);
            }
            else
            {
                obj.RId = rId;
                obj.GPAType = Common.GPAType.Nominee.ToString();
            }
            return View(obj);
        }

        #endregion
        #region Gather Interview details Module
        /// <summary>
        /// ManageLeaseDeed screen
        /// </summary>
        /// <returns></returns>

        public ActionResult ManageInterviewDetails()
        {
            return View();
        }
        //  <summary>
        // Get all funtional property list
        // </summary>
        public ActionResult GetInterviewDetails([DataSourceRequest] DataSourceRequest req)
        {
            var objFunctional = _propertyRegistrationService.GetInterviewDetails(req);

            return Json(objFunctional, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult AddInterviewdetails()
        {
            var obj = new InterviewDetailsModel();
            return View(obj);
        }
        public ActionResult SaveInterviewdetails(int ApplicationId, string InterviewDetails, DateTime InterviewDate, int SchemeId, int DepartmentId, string FormNo)
        {
            string InterviewDetailsHtml = HttpUtility.HtmlDecode(InterviewDetails);
            int isAdded = _propertyRegistrationService.SaveInterviewForm(ApplicationId, InterviewDetailsHtml, InterviewDate, SchemeId, DepartmentId, FormNo);
            return Json(isAdded, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditInterviewDetails(int id)
        {
            if (id != 0)
            {
                var data = _propertyRegistrationService.GetInterviewDetailsById(id);
                return View(data);
            }

            //  return View(data);
            //InterviewDetailsModel objID = new InterviewDetailsModel();
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewInterviewDetails(int id)
        {
            if (id != 0)
            {
                var data = _propertyRegistrationService.GetInterviewDetailsById(id);
                return View(data);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateInterviewDetails(string InterviewDetails, DateTime InterviewDate, int Id, int ApplicationId, int SchemeId)
        {
            string InterviewDetailsHtml = HttpUtility.HtmlDecode(InterviewDetails);
            int isEdit = _propertyRegistrationService.UpdateInterviewDetails(ApplicationId, InterviewDetailsHtml, InterviewDate, Id, SchemeId);
            return Json(isEdit, JsonRequestBehavior.AllowGet);

        }
        //  To Remove cost record by id.
        public JsonResult RemoveInterviewDetails(int rId)
        {

            var data = _propertyRegistrationService.RemoveInterviewDetails(rId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult UploadDocumnetDoc()
        //{

        //    string file = string.Empty;
        //    var uploadExcel = Request.Files[0];

        //    string fileSavePath = "";
        //    for (int i = 0; i < Request.Files.Count; i++)
        //    {
        //        var hpf = Request.Files[i];
        //        if (hpf != null && hpf.ContentLength > 0)
        //        {
        //            var folderName = Request.Files.AllKeys[i];
        //            var fileName = new FileInfo(hpf.FileName).Name;
        //            fileName = fileName.Replace(" ", "");
        //            // fileDBPath = fileName;

        //            fileSavePath = Path.Combine(Server.MapPath("~/UploadFiles"), fileName);
        //            //file.SaveAs(path);
        //            hpf.SaveAs(fileSavePath);
        //            Application application = new Application();

        //            // Open a doc file.
        //            Document document = application.Documents.Open(fileSavePath);

        //            String read = string.Empty;
        //            List<string> data = new List<string>();

        //            for (int i1 = 0; i1 < document.Paragraphs.Count; i1++)
        //            {
        //                string temp = document.Paragraphs[i1 + 1].Range.Text.Trim();
        //                if (temp != string.Empty)
        //                    data.Add(temp);
        //            }

        //            foreach (var item in data)
        //            {
        //                file += item;
        //            }
        //            //hpf.de(fileSavePath);
        //            document.Close();

        //            System.IO.File.Delete(fileSavePath);


        //        }
        //    }

        //    return Json(file, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult UploadDocumnetDoc(HttpPostedFileBase uploadExcel)
        {
            string file = string.Empty;
            if (Request.Files.Count != 0)
            {
                var uploadFile = Request.Files[0];

                var fileName = uploadFile.FileName;
                var fpath = Server.MapPath(ConfigurationManager.AppSettings["UploadFilePath"]) + fileName;
                if (System.IO.File.Exists(fpath))
                {
                    System.IO.File.Delete(fpath);
                }
                HttpPostedFileBase hfb = uploadFile;
                hfb.SaveAs(fpath);
                Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();

                StringBuilder text = new StringBuilder();

                object miss = System.Reflection.Missing.Value;
                //object path = @"C:\Users\skumar\Desktop\Akarsh.docx";
                object path = fpath;
                object readOnly = true;
                Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);

                for (int i = 0; i < docs.Paragraphs.Count; i++)
                {
                    text.Append(" \r\n " + docs.Paragraphs[i + 1].Range.Text.ToString());
                }

                StringWriter sw = new StringWriter(text);
                return Json(text.ToString(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("null", JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// return scheme list for dropdown
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSchemeList()
        {
            var schemeList = _propertyRegistrationService.GetSchemeList();
            return Json(schemeList, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// return department based on scheme for dropdown
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public ActionResult FilterDepartmentOnScheme(int schemeId)
        {
            var department = _propertyRegistrationService.FilterDepartmentOnScheme(schemeId);
            return Json(department, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewGPA()
        {
            var rId = Convert.ToInt32(Request.QueryString["RId"]);
            var type = Request.QueryString["Type"];
            var obj = new GPAModel();
            if (type.ToLower() == Common.GPAType.GPA.ToString().ToLower())
            {
                obj = _propertyRegistrationService.GetGPADetailsByRId(rId);
            }
            else
            {
                obj.RId = rId;
                obj.GPAType = Common.GPAType.Nominee.ToString();
            }
            return View(obj);
        }

        public JsonResult GetAllForms(int SchemeId, int DepartmentId)
        {
            var lst = _allotmentService.GetAllForms(SchemeId, DepartmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApplicantDetailsForInterview(string formno, int schemeID, int departmentID)
        {
            var lst = _propertyRegistrationService.GetApplicantDetailsForInterview(formno, schemeID, departmentID);
            // var lst = _propertyRegistrationService.GetApplicantDetails (formno, schemeID, departmentID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// Returns DDL with "Yes" and "No" options
        /// </summary>
        /// <returns></returns>
        public JsonResult YesNoDDL()
        {
            var lst = _generalService.YesNoDDL();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns DDL with "Registered" and "UnRegistered" options
        /// </summary>
        /// <returns></returns>
        public JsonResult RegUnRegDDL()
        {
            var lst = _generalService.RegUnRegDDL();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        #region 'Noting Details'
        /// <summary>
        /// Noting details screen
        /// </summary>
        /// <returns></returns>

        public ActionResult ManageNotings()
        {
            return View();
        }
        /// <summary>
        /// Fetches RIDs on the basis of Departments allowed from DB.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDsForNoting()
        {
            var RIdlst = _propertyRegistrationService.GetRIDsForNoting();
            return Json(RIdlst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRidToSearchNoting()
        {
            var RidList = _propertyRegistrationService.GetRidToSearchNoting();
            return Json(RidList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fetches Deptmnets details on the basis of Departments allowed from DB.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDeptmentForNoting()
        {
            var RIdlst = _propertyRegistrationService.GetDeptmentForNoting();
            return Json(RIdlst, JsonRequestBehavior.AllowGet);
        }
        //  <summary>
        // submit AddNotingFile 
        // </summary>
        public ActionResult AddNotingFile(int Rid, string FileName, string DepartmentName, int? deptId)
        {

            int blnSuccess = 0;

            blnSuccess = _propertyRegistrationService.AddNotingFile(Rid, FileName, DepartmentName, deptId);

            return Json(blnSuccess, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SubmitForMoveNoting(int Rid, string user)
        {
            int blnSuccess = 0;

            blnSuccess = _propertyRegistrationService.SubmitForMoveNoting(Rid, user);

            return Json(blnSuccess, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateNotingForAllottedProperty(int Rid, string user, string notingDetails)
        {
            string notingDetailsHtml = string.Empty;
            if (!string.IsNullOrEmpty(notingDetails))
                notingDetailsHtml = HttpUtility.HtmlDecode(notingDetails);
            int result = _propertyRegistrationService.UpdateNotingForAllottedProperty(Rid, user, notingDetailsHtml);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //  <summary>
        // submit AddNotingFile 
        // </summary>
        public ActionResult AddNotingsDetails(int rid, string addNotingDetails)
        {
            string AddNotingDetailsHtml = String.Empty;
            if (!string.IsNullOrEmpty(addNotingDetails))
                AddNotingDetailsHtml = HttpUtility.HtmlDecode(addNotingDetails);
            var isAdded = _propertyRegistrationService.AddNotingsDetails(rid, AddNotingDetailsHtml);
            return Json(isAdded, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNotingDeptmentByRId(int rId)
        {
            var buildingPlanDetail = _propertyRegistrationService.GetNotingDeptmentByRId(rId);
            return Json(buildingPlanDetail, JsonRequestBehavior.AllowGet);
        }
        //  <summary>
        // Get all Noting list
        // </summary>
        public ActionResult GetNotingDetails([DataSourceRequest] DataSourceRequest req, int? Rid, int? DepartmentId, string FileName)
        {
            var objFunctional = _propertyRegistrationService.GetNotingDetails(req, Rid, DepartmentId, FileName);

            return Json(objFunctional, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Fetches data for View noting screen
        /// </summary>
        /// <param name="id">RID</param>
        /// <returns></returns>
        public ActionResult ViewNotingDetails(string id)
        {
            int Id = Convert.ToInt32(CommonHelper.Decode(id));
            var data = new NotingDetailsModel();
            if (Id != 0)
            {
                data = _propertyRegistrationService.ViewNotingDetails(Id);
            }
            return View(data);
        }
        #endregion

        public JsonResult GetDepartmentsByUser()
        {
            var lst = _generalService.GetDepartmentsByUser();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDemandLetterTypes()
        {
            var lst = _propertyRegistrationService.GetDemandLetterTypes();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #region "Manage Bank Account"
        public ActionResult ManageBankAccount()
        {
            return View();
        }
        public ActionResult CreateChallan()
        {
            return View();
        }
        //To get all banks in ddl
        public JsonResult GetAllBanks()
        {
            var lst = _generalService.GetAllBanks();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //To get all branches in ddl
        public JsonResult GetAllBranchs(int bankId)
        {
            var lst = _generalService.GetAllBranchs(bankId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //To get Account Number base on bankid and branchid
        public JsonResult GetAccountNumber(int bankId, int branchId)
        {
            var data = _propertyRegistrationService.GetAccountNumber(bankId, branchId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //To get all branches in ddl
        public JsonResult GetAllAccountHead()
        {
            var lst = _propertyRegistrationService.GetAllAccountHead();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //To get all Account Sub Head base on accountId
        public JsonResult GetAccountSubHead(int AccountHeadId)
        {
            var lst = _propertyRegistrationService.GetAccountSubHead(AccountHeadId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //Save challan detils in db condider paramenter rid,accountHeadId,AccountSubHeadId,Amount
        public JsonResult SaveCreateChallan(int rId, int AccountHeadId, int AccountSubHeadId, decimal? Amount)
        {
            var flag = _propertyRegistrationService.SaveCreateChallan(rId, AccountHeadId, AccountSubHeadId, Amount);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        //To remove challan charge deatils base on rid and challanTransID
        //public JsonResult RemoveChallanChargeDetail(int ChallanTransID, int rId)
        //{
        //    var data = _propertyRegistrationService.RemoveChallanChargeDetail(ChallanTransID, rId);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult RemoveChallanChargeDetail(int rid, string headName, string subHeadName, decimal amount)
        {
            var data = _propertyRegistrationService.RemoveChallanChargeDetail(rid, headName, subHeadName, amount);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //To get all account charge deatils 
        public JsonResult GetAccountChargeDetails([DataSourceRequest] DataSourceRequest request, int rId)
        {
            var dataresult = _propertyRegistrationService.GetAccountChargeDetails(request, rId);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGeneratedChallanDetails([DataSourceRequest] DataSourceRequest request, int rid)
        {
            //List<BankAccountManagementModel> sessionData = (List<BankAccountManagementModel>)Session["TempModel"];
            ////List<BankAccountManagementModel> modelList = new List<BankAccountManagementModel>();
            ////DataSourceRequest requestSource = new DataSourceRequest();
            //var data = sessionData.ToDataSourceResult(request);
            //return Json(data, JsonRequestBehavior.AllowGet);


            List<BankAccountManagementModel> dataresult = _propertyRegistrationService.GetGeneratedChallanDetails(rid);
            if (dataresult == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = dataresult.ToDataSourceResult(request);
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }
        //To save generate challan deatils base on rid and bankid,accountNumber
        public JsonResult SaveGenerateChallan(int rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId)
        {
            var flag = _propertyRegistrationService.SaveGenerateChallan(rId, bankId, branchId, DdlAccountNumber, DepttId);
            flag = _generalService.GenerateLetterFromDbByRId(rId, 2);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult GeneratePaymentChallan(int rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId, int? amount, int? accountHeadId, int? accountSubHeadId)
        //{
        //    string parsedHTML = string.Empty;
        //    //ChallanModel requestModel = _propertyRegistrationService.GenerateChallan(rId, bankId, branchId, DdlAccountNumber, DepttId, amount, accountHeadId, accountSubHeadId);
        //    ChallanModel requestModel = _propertyRegistrationService.GeneratePaymentChallan(rId, bankId, branchId, DdlAccountNumber, DepttId, amount, accountHeadId, accountSubHeadId);
        //    if (requestModel != null)
        //    {
        //        //parsedHTML = _templateParserService.GetParsedHTML(requestModel, "ChallanTemplate.cshtml");
        //        bool flag = _propertyRegistrationService.SaveGeneratedChallan(rId, bankId, branchId, DdlAccountNumber, DepttId, parsedHTML);
        //    }
        //    return Json(parsedHTML, JsonRequestBehavior.AllowGet);
        //} 

        //get Manage account details.
        public JsonResult GetManageAccountDetails([DataSourceRequest] DataSourceRequest request)
        {
            var dataresult = _propertyRegistrationService.GetManageAccountDetails(request);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Fills RID DDL 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDsForManageAccountDetails()
        {
            var RIdlst = _propertyRegistrationService.GetRIDsForManageAccountDetails();
            return Json(RIdlst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fills RID DDL Datasource
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDsForManageAccountDetailsByDataSource([DataSourceRequest] DataSourceRequest Req)
        {
            var RIdlst = _propertyRegistrationService.GetRIDsForManageAccountDetailsByDataSource(Req);
            return Json(RIdlst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeActivateRole(string ChallanNo, bool status, string viewN)
        {
            var flag = _propertyRegistrationService.DeActivate(ChallanNo, status, viewN);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        // To Generate Challan Generate
        public ActionResult ChallanGenerate(string ChallanNo, int rid)
        {
            var lst = _propertyRegistrationService.ChallanGenerate(ChallanNo, rid);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //get bank names for payment
        public ActionResult GetBankNamesForPayment([DataSourceRequest] DataSourceRequest request)
        {
            List<DynamicDataModel> banks = _propertyRegistrationService.GetBankNamesForPayment();
            var data = banks.ToDataSourceResult(request);
            return Json(banks, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Used to print Transfer Deed
        /// </summary>
        /// <param name="rId">RID</param>
        /// <returns></returns>
        public ActionResult PrintTransferDeedByRId(int rId)
        {
            string content = string.Empty;
            if (rId != 0)
            {
                content = _generalService.GenerateLetterFromDbByRId(rId, Constants.transDeedTemplateId);
            }
            return Json(content, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Used to print Lease Deed
        /// </summary>
        /// <param name="rId"></param>
        /// <returns></returns>
        public ActionResult PrintLeaseDeedByRId(int rId)
        {
            string content = string.Empty;
            if (rId != 0)
            {
                content = _generalService.GenerateLetterFromDbByRId(rId, Constants.leaseDeedTemplateId);
            }
            return Json(content, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidateRentingStartDate(int rid, DateTime rentStartDate)
        {
            FunctionalModel functinal = _propertyRegistrationService.GetFunctionalDetailByRid(rid);
            return Json(functinal.FunctionalDate.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadPaymentChallan(int rid, string challanId)
        {
            //var lst = _generalService.GenerateLetterFromDb(rid, Convert.ToInt32(LetterTemplate.PossessionLetter), departmentId);
            var lst = _propertyRegistrationService.ChallanGenerate(challanId, rid);

            System.Text.StringBuilder strBody = new System.Text.StringBuilder(lst);

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Challan.doc");
            Response.ContentType = "application/vnd.ms-word ";
            Response.Charset = string.Empty;

            StringWriter sw = new StringWriter(strBody);

            //HtmlTextWriter htw = new HtmlTextWriter(sw);
            //stream.RenderControl(htw); 
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("ManageBankAccount");
        }

        public JsonResult GetRentPermissionByRid([DataSourceRequest]DataSourceRequest request, int Rid)
        {
            var rentDetails = _propertyRegistrationService.GetRentPermissionByRid(request, Rid);
            return Json(rentDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveRentRequestDetailsById(RentPermissionModel model)
        {
            int flag = _propertyRegistrationService.RemoveRentRequestDetailsById(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKYADetailsForRid(KYAViewModel model)
        {
            var data = _generalService.GetKYADetailsForRid(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsRequestIdIsExist(ServiceViewModel model)
        {
            var flag = _generalService.IsRequestIdIsExist(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}