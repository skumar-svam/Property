using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Model.Property;
using NA.PMS.Service;
using NA.PMS.Service.Property;
using NA.PMS.Web.Controllers;

namespace NA.PMS.Web.Areas.Property.Controllers
{
    public class MergeSplitPropertiesController : WebBaseController
    {
        IMergeSplitPropertyService _mergeSplitPropertyService;
        IGeneralService _generalService;
        IMastersService _mastersService;
        IAllotmentService _allotmentService;

        public MergeSplitPropertiesController(IGeneralService generalService, IMergeSplitPropertyService mergeSplitPropertyService, IAllotmentService allotmentService, IMastersService mastersService)
        {
            _generalService = generalService;
            _mergeSplitPropertyService = mergeSplitPropertyService;
            _allotmentService = allotmentService;
            _mastersService = mastersService;
        }

        public ActionResult MergeProperties()
        {
            return View();
        }

        public ActionResult ManageMergeRequests()
        {
            return View();
        }

        public ActionResult SplitProperties()
        {
            return View();
        }

        public ActionResult ManageSplitRequests()
        {
            return View();
        }

        public ActionResult AddMergeRequest()
        {
            var mergeSplitProp = new AddMergeSplitRequestModel { DepartmentCollection = GetDepartment() };
            return View(mergeSplitProp);
        }

        public ActionResult AddSplitRequest()
        {
            var mergeSplitProp = new AddMergeSplitRequestModel { DepartmentCollection = GetDepartment() };
            return View(mergeSplitProp);
        }

        public ActionResult ViewMergeRequest(int id)
        {
            var mergeSplitRequestModel = new AddMergeSplitRequestModel();

            mergeSplitRequestModel = _mergeSplitPropertyService.GetMergeSplitRequestDetails(id);
            mergeSplitRequestModel.DepartmentCollection = GetDepartment();
            mergeSplitRequestModel.RequestId = id;
            //mergeSplitRequestModel.ListMergeSplitPropertyGrid =
            //    _mergeSplitPropertyService.GetPropertiesToMergeByRequestId(id);

            return View(mergeSplitRequestModel);
        }

        public ActionResult ViewApproverMergeRequest(int id)
        {
            var mergeSplitRequestModel = new AddMergeSplitRequestModel();

            mergeSplitRequestModel = _mergeSplitPropertyService.GetMergeSplitRequestDetails(id);
            mergeSplitRequestModel.DepartmentCollection = GetDepartment();
            mergeSplitRequestModel.RequestId = id;
            //mergeSplitRequestModel.ListMergeSplitPropertyGrid =
            //    _mergeSplitPropertyService.GetPropertiesToMergeByRequestId(id);

            return View(mergeSplitRequestModel);
        }

        public ActionResult EditMergeRequest(int id)
        {
            var mergeSplitRequestModel = new AddMergeSplitRequestModel();

            mergeSplitRequestModel = _mergeSplitPropertyService.GetMergeSplitRequestDetails(id);
            mergeSplitRequestModel.DepartmentCollection = GetDepartment();
            mergeSplitRequestModel.RequestId = id;
            mergeSplitRequestModel.ListMergeSplitPropertyGrid =
                _mergeSplitPropertyService.GetPropertiesToMergeByRequestId(id);
            return View(mergeSplitRequestModel);
        }

        public ActionResult ViewApproverSplitRequest(int id)
        {
            var mergeSplitRequestModel = new AddMergeSplitRequestModel();

            mergeSplitRequestModel = _mergeSplitPropertyService.GetMergeSplitRequestDetails(id);
            mergeSplitRequestModel.DepartmentCollection = GetDepartment();
            mergeSplitRequestModel.RequestId = id;
            //mergeSplitRequestModel.ListMergeSplitPropertyGrid =
            //    _mergeSplitPropertyService.GetPropertiesToMergeByRequestId(id);

            return View(mergeSplitRequestModel);
        }

        public ActionResult ViewSplitRequest(int id)
        {
            var mergeSplitRequestModel = new AddMergeSplitRequestModel();

            mergeSplitRequestModel = _mergeSplitPropertyService.GetMergeSplitRequestDetails(id);
            mergeSplitRequestModel.DepartmentCollection = GetDepartment();
            mergeSplitRequestModel.RequestId = id;
            //mergeSplitRequestModel.ListMergeSplitPropertyGrid =
            //    _mergeSplitPropertyService.GetPropertiesToMergeByRequestId(id);

            return View(mergeSplitRequestModel);
        }

        public ActionResult EditSplitRequest(int id)
        {
            var mergeSplitRequestModel = new AddMergeSplitRequestModel();

            mergeSplitRequestModel = _mergeSplitPropertyService.GetMergeSplitRequestDetails(id);
            mergeSplitRequestModel.DepartmentCollection = GetDepartment();
            mergeSplitRequestModel.RequestId = id;
            mergeSplitRequestModel.ListMergeSplitPropertyGrid =
                _mergeSplitPropertyService.GetPropertiesToMergeByRequestId(id);
            return View(mergeSplitRequestModel);
        }

        // Get all merged properties
        public JsonResult GetAllMergeProperties([DataSourceRequest] DataSourceRequest request)
        {
            var listMergeProperties = _mergeSplitPropertyService.GetAllMergeProperties(request);
            return Json(listMergeProperties, JsonRequestBehavior.AllowGet);
        }

        // Get all merged properties
        public JsonResult GetAllSplitProperties([DataSourceRequest] DataSourceRequest request)
        {
            var listSplitProperties = _mergeSplitPropertyService.GetAllSplitProperties(request);
            return Json(listSplitProperties, JsonRequestBehavior.AllowGet);
        }

        // Get all merged requests
        public JsonResult GetAllMergeRequests([DataSourceRequest] DataSourceRequest request)
        {
            var listMergeRequests = _mergeSplitPropertyService.GetAllMergeRequests(request);
            return Json(listMergeRequests, JsonRequestBehavior.AllowGet);
        }
        // Get all Split requests
        public JsonResult GetAllSplitRequests([DataSourceRequest] DataSourceRequest request)
        {
            var listMergeRequests = _mergeSplitPropertyService.GetAllSplitRequests(request);
            return Json(listMergeRequests, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddMergeRequest(MergeSplitPropModel mergeSplitPropModel)
        {
            return RedirectToAction("MergeProperties");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddSplitRequest(MergeSplitPropModel mergeSplitPropModel)
        {
            return RedirectToAction("SplitProperties");
        }

        #region Bind Dropdownn
        /// <summary>
        /// Get All Schemes
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SelectListItem> GetSchemes()
        {
            var schemeCollection = new List<SelectListItem>();
            var selectListItem = new SelectListItem { Text = "--Select--", Value = String.Empty };
            schemeCollection.Add(selectListItem);
            var data = (from x in _mastersService.GetSchemes()
                        select new SelectListItem
                        {
                            Text = String.Format("{0}", x.schemeName),
                            Value = x.schemeId.ToString()
                        }).ToList();
            schemeCollection.AddRange(data);
            return schemeCollection;
        }

        /// <summary>
        /// Get Departments on the basis of SchemeId
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetDepartment()
        {
            var deptCollection = new List<SelectListItem>();
            //var selectListItem = new SelectListItem { Text = "--Select--", Value = String.Empty };
            //deptCollection.Add(selectListItem);
            var data = _generalService.GetAllUserDepartments();
            deptCollection.AddRange(data);
            return deptCollection;
        }
        /// <summary>
        /// Get Property Type on the basis of SchemeId and Sector Id
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public JsonResult GetPropertyTypeBySchemeAndDeptId(int schemeId, int departmentId)
        {
            var data = (from x in _mastersService.GetPropertyTypeBySchemeAndDeptId(schemeId, departmentId)
                        select new SelectListItem
                        {
                            Text = String.Format("{0}", x.propertyType),
                            Value = x.propertyTypeId.ToString()
                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// Get Sectors on the basis of SchemeId and Sector Id
        public JsonResult GetSectorsByDeptId(int departmentId)
        {
            var data = _generalService.GetAllSectorsByDeptId(departmentId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// Get Sectors on the basis of SchemeId and Sector Id
        public JsonResult GetSectorsByDeptIdReq([DataSourceRequest] DataSourceRequest Req)
        {
            var data = _generalService.GetAllSectorsByDeptId(Req);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// Get Blocks on the basis of SchemeId and Sector Id
        public JsonResult GetBlocksByDeptAndSectorId(int departmentId, int sectorId)
        {
            var data = _generalService.GetAllBlocksByDeptAndSectorId(departmentId, sectorId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// Get Blocks on the basis of SchemeId and Sector Id
        public JsonResult GetBlocksByDeptAndSectorIdReq([DataSourceRequest] DataSourceRequest Req, int departmentId, int sectorId)
        {
            var data = _generalService.GetAllBlocksByDeptAndSectorId(Req, departmentId, sectorId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // Get All RIDs for Split or Merger
        public JsonResult GetAllRIDsForSplitMerge(int departmentId, int sectorId, int blockId)
        {
            var data = _mergeSplitPropertyService.GetAllRIDsForSplitMerge(departmentId, sectorId, blockId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // Get All RIDs for Split or Merger
        public JsonResult GetAllRIDsForSplitMergeReq([DataSourceRequest] DataSourceRequest Req, int departmentId, int sectorId, int blockId)
        {
            var data = _mergeSplitPropertyService.GetAllRIDsForSplitMerge(Req, departmentId, sectorId, blockId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // Get All RIDs for Split or Merge
        public JsonResult GetAllRidsForSplit(int departmentId)
        {
            var data = _mergeSplitPropertyService.GetAllRidsForSplit(departmentId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        // Get Propperty details by RID
        public JsonResult GetPropertyDetailsByRid(int rId)
        {
            var lst = _mergeSplitPropertyService.GetPropertyDetailsByRid(rId);//_allotmentService.GetPropertyDetails(rId);

            if (lst != null)
            {
                lst.ActualArea = lst.ActualArea ?? 0;
                lst.TotalArea = lst.TotalArea ?? 0;
                lst.PropertyCost = lst.PropertyCost ?? 0;
                lst.TotalPropertyCost = lst.TotalPropertyCost ?? 0;
                lst.CoveredArea = lst.CoveredArea ?? 0;
                lst.RequestId = _mergeSplitPropertyService.GetRequestIdByRid(rId);
            }

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddPropertyToMergeSplit(int rid, int propertyId)
        {
            var result = _mergeSplitPropertyService.AddUpdatePropertyToMergeSplit(rid, propertyId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // Get Properties to Merge
        public JsonResult GetPropertiesToMergeByRequestId([DataSourceRequest] DataSourceRequest request, int requestId)
        {
            var lst = _mergeSplitPropertyService.GetPropertiesToMergeByRequestId(requestId);

            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        //// Get Properties to Split
        //public JsonResult GetPropertiesToMerge([DataSourceRequest] DataSourceRequest request, MergeSplitPropertyGrid listMergeSplitPropertyGrids)
        //{
        //    //var lst = _mergeSplitPropertyService.GetPropertiesToMergeByRequestId(requestId);

        //    return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}

        // Get Properties to Split
        public JsonResult GetPropertiesToSplitByRequestId([DataSourceRequest] DataSourceRequest request, int requestId)
        {
            var lst = _mergeSplitPropertyService.GetPropertiesToSplitByRequestId(requestId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // Get Properties to Split
        public JsonResult GetPropertiesToSplitByRid([DataSourceRequest] DataSourceRequest request, int requestId)
        {
            var lst = _mergeSplitPropertyService.GetPropertiesToSplitByRid(requestId);
            return Json(lst.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemovePropertyFromMergeSplit([DataSourceRequest] DataSourceRequest request, int rid, List<MergeSplitPropertyGrid> lstMergeSplitPropertyGrids)
        {
            var item = lstMergeSplitPropertyGrids.SingleOrDefault(x => x.Rid == rid);
            lstMergeSplitPropertyGrids.Remove(item);
            //var result = _mergeSplitPropertyService.RemovePropertyFromMergeSplit(rid);
            return Json(lstMergeSplitPropertyGrids.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // Submit Merge request 
        [HttpPost]
        [AllowAnonymous]
        public JsonResult SubmitRequestForApproval(string allRids, int userId, decimal charge)
        {
            //int type = Constants.Amalgamation; // 17-Amalgamation request, 18-Deamalgamation request
            //int status = Constants.InProgress; //  1	Approved,2	Rejected,3	Cancelled,4	Pending,5	InProgress
            var flag = false;
            flag = _mergeSplitPropertyService.MergeRequest(allRids, userId, charge, Constants.InProgress, Constants.Amalgamation, null, null);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        // Submit Split request 
        [HttpPost]
        [AllowAnonymous]
        public JsonResult SubmitSplitRequestForApproval(string allRids, int rId, int userId, decimal charge)
        {
            var flag = false;
            flag = _mergeSplitPropertyService.SplitRequest(allRids, rId, userId, charge, Constants.InProgress, Constants.Deamalgamation, null, null);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        // Resubmit Merge Request
        public JsonResult ReSubmitRequestForApproval(string allRids, int userId, decimal charge)
        {
            var flag = false;
            flag = _mergeSplitPropertyService.MergeRequest(allRids, userId, charge, Constants.InProgress, Constants.Amalgamation, null, null);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        // Resubmit Merge Request
        public JsonResult ReSubmitSplitRequestForApproval(string allRids, int rId, int requestNo, int userId, decimal charge)
        {
            var flag = false;
            flag = _mergeSplitPropertyService.SplitRequest(allRids, rId, userId, charge, Constants.ReSubmitted, Constants.Deamalgamation, requestNo, null);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        // Resubmit Merge Request
        public JsonResult CancelMergeRequest(int requestNo)
        {
            var flag = false;
            flag = _mergeSplitPropertyService.MergeRequest(string.Empty, 0, 0, Constants.Cancelled, Constants.Amalgamation, requestNo, null);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        // Resubmit Merge Request
        public JsonResult CancelSplitRequest(int requestNo)
        {
            var flag = false;
            flag = _mergeSplitPropertyService.SplitRequest(string.Empty, 0, 0, 0, Constants.Cancelled, Constants.Deamalgamation, requestNo, null);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApproveMergeRequest(int requestNo, string comments)
        {
            var flag = false;
            flag = _mergeSplitPropertyService.MergeRequest(string.Empty, 0, 0, Constants.Approved, Constants.Amalgamation, requestNo, comments);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RejectMergeRequest(int requestNo, string comments)
        {
            var flag = false;
            flag = _mergeSplitPropertyService.MergeRequest(string.Empty, 0, 0, Constants.RejectedProp, Constants.Amalgamation, requestNo, comments);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApproveSplitRequest(int requestNo, string comments)
        {
            var flag = false;
            flag = _mergeSplitPropertyService.SplitRequest(string.Empty, 0, 0, 0, Constants.Approved, Constants.Deamalgamation, requestNo, comments);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RejectSplitRequest(int requestNo, string comments)
        {
            var flag = false;
            flag = _mergeSplitPropertyService.SplitRequest(string.Empty, 0, 0, 0, Constants.RejectedProp, Constants.Deamalgamation, requestNo, comments);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}