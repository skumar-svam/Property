using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Service;
using NA.PMS.Service.Property;
using NA.PMS.Model;

namespace NA.PMS.Web.Areas.Property.Controllers
{
    public class CompletionController : Controller
    {
        private ICompletionService _completionService;
        private IGeneralService _generalService;
        //private IAllotmentService _allotmentService;

        public CompletionController(ICompletionService completionService, IGeneralService generalService)
        {
            _completionService = completionService;
            _generalService = generalService;
        }
        // GET: Property/Completion
        public ActionResult ManageCompletion()
        {
            return View();
        }

        /// <summary>
        /// For reading grid data on ManageCompletion screen from DB for Requestor
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public JsonResult GetCompletionData([DataSourceRequest] DataSourceRequest req)
        {
            var completionLst = _completionService.GetCompletionData(req);
            return Json(completionLst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add Completion screen's View
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCompletionRequest()
        {
            return View();
        }

        /// <summary>
        /// Fills RID DDL according to BRs and validations applied in Completion 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDs(string type)
        {
            var RIdlst = _generalService.GetRIDs(type);
            return Json(RIdlst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fills RID DDL according to BRs and validations applied in Completion 
        /// </summary>
        /// <returns></returns>
        //public JsonResult GetRIDsForCompletion()
        //{
        //    var RIdlst = _completionService.GetRIDsForCompletion();
        //    return Json(RIdlst, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// Fetches Applicant Details by RID
        /// </summary>
        /// <param name="rId"></param>
        /// <returns></returns>
        public JsonResult GetApplicantDetailsByRId(int rId)
        {
            if (rId != 0)
            {
                var details = _completionService.GetApplicantDetailsByRId(rId);
                if (details != null)
                {
                    details.CompletionDueDate = _generalService.GetCompletionDueDateByRId(rId, Convert.ToInt32(details.SchemeId), Convert.ToInt32(details.DepttId));
                    details.CompletionExecutionMinDate = Convert.ToDateTime(details.PossessionDate).AddDays(Convert.ToDouble(1));
                }
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fetches data for View Complete screen
        /// </summary>
        /// <param name="id">ReqId</param>
        /// <returns></returns>
        public ActionResult ViewCompletionRequest(int id)
        {
            var data = new PropertyCompletionModel();
            if (id != 0)
            {
                data = _completionService.GetCompletionDetailsByReqId(id);
            }
            return View(data);
        }

        /// <summary>
        /// Check for Unique Request by Registration Id. There should be only one request for single Registration Id
        /// </summary>
        /// <param name="rId"></param>
        /// <returns></returns>
        public JsonResult CheckForUniqueCompletionRequest(int rId)
        {
            var flag = _completionService.CheckForUniqueCompletionRequest(rId);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Saving Request for Completion in DB
        /// </summary>
        /// <param name="completionDueDate"></param>
        /// <param name="completionExecutionDate"></param>
        /// <param name="rId"></param>
        /// <param name="completionType"></param>
        /// <param name="propNo"></param>
        /// <param name="partCompletionArea"></param>
        /// <param name="partCompletionPercentage"></param>
        /// <param name="completionCharges"></param>
        /// <returns></returns>
        public JsonResult SaveCompletionDetails(DateTime completionDueDate, DateTime completionExecutionDate, int rId, string completionType, string propNo, Decimal? partCompletionArea, Decimal? partCompletionPercentage, Decimal? completionCharges, string extension, string viewName)
        {
            var flag = _completionService.SaveCompletionDetails(completionDueDate, completionExecutionDate, rId, completionType, propNo, partCompletionArea, partCompletionPercentage, completionCharges, extension, viewName);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Getting Completion Type Partial and Full
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCompletionType()
        {
            var lst = _generalService.GetCompletionType();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
    }
}