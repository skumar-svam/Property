using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Service.Property;
using NA.PMS.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NA.PMS.Web.Areas.Property.Controllers
{
    public class PropertyCancellationController : WebBaseController
    {
        private IPropertyCancellationService _propertyCancellationService;

        public PropertyCancellationController(IPropertyCancellationService propertyCancellationService)
        {
            _propertyCancellationService = propertyCancellationService;
        }

        /// <summary>
        /// Returns ManageCancellations View
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageCancellations()
        {
            return View();
        }

        /// <summary>
        /// Reads grid on ManageCancellations View
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public JsonResult GetAllCancellationsSurrenders([DataSourceRequest] DataSourceRequest req)
        {
            var lst = _propertyCancellationService.GetAllCancellationsSurrenders(req);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns Add Cancellation View
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCancellation()
        {
            return View();
        }

        public JsonResult SaveCancellation(int rId, int cancelType, string cancelReason, int? refundYesNo, int? refundType, decimal? refundAmt, int userVal, int? reqNo, string restoreReason, decimal? restoreCharges)
        {
            var flag = false;
            flag = _propertyCancellationService.SaveCancellation(rId, cancelType, cancelReason, refundYesNo, refundType, refundAmt, userVal, reqNo, restoreReason, restoreCharges);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRIDsByCancellationType([DataSourceRequest] DataSourceRequest Req, int type)
        {
            var lst = _propertyCancellationService.GetRIDsByCancellationType(Req, type);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRefundTypes(int rId, int depttId)
        {
            var lst = _propertyCancellationService.GetRefundTypes(rId, depttId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewCancellation(int Id)
        {
            var details = _propertyCancellationService.GetCancellationDetail(Id);
            return View(details);
        }

        public ActionResult ManageCancellations_Approver()
        {
            return View();
        }

        public JsonResult GetAllCancellationsSurrenders_Approver([DataSourceRequest] DataSourceRequest req)
        {
            var lst = _propertyCancellationService.GetAllCancellationsSurrenders_Approver(req);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewCancellation_Approver(int Id)
        {
            var details = _propertyCancellationService.GetCancellationDetail(Id);
            return View(details);
        }

        public JsonResult SaveCancellationApprovalStatus(string comments, int intStatus, int ReqNo, int type)
        {
            var rslt = _propertyCancellationService.SaveCancellationApprovalStatus(comments, intStatus, ReqNo, type);
            return Json(rslt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelRequest(int requestId)
        {
            var rslt = _propertyCancellationService.CancelRequest(requestId);
            return Json(rslt, JsonRequestBehavior.AllowGet);
        }        
    }
}