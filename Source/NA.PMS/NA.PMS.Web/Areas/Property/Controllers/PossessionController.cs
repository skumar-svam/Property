using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Model.Property;
using NA.PMS.Service;
using NA.PMS.Service.Property;
using NA.PMS.Service.TemplateParser;
using NA.PMS.Web.Controllers;
using System.IO;

namespace NA.PMS.Web.Areas.Property.Controllers
{
    public class PossessionController : WebBaseController
    {

        private IPossessionService _possessionService;
        private IPropertyAllotmentService _propertyAllotmentService;
        private ITemplateParserService _templateParserService;
        private IGeneralService _generalService;

        public PossessionController(IPossessionService possessionService, IPropertyAllotmentService propertyAllotmentService, ITemplateParserService templateParserService, IGeneralService generalService)
        {
            _possessionService = possessionService;
            _propertyAllotmentService = propertyAllotmentService;
            _templateParserService = templateParserService;
            _generalService = generalService;
        }

        // GET: Property/Possession
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult ManagePossessionOrder(PropertyPossessionModel possessionModel)
        {

            return View(possessionModel);
        }

        public ActionResult ManagePossessionEntry()
        {
            return View();
        }

        public ActionResult ManagePossessionorderApproval()
        {
            return View();
        }

        public ActionResult AddPossessionOrder()
        {
            return View();
        }

        public ActionResult AddBulkPossessionOrder(PropertyPossessionModel possessionModel)
        {

            return View(possessionModel);
        }

        public ActionResult AddPossessionEntry()
        {
            return View();
        }

        public ActionResult ViewPossessionEntry(string id)
        {
            int ID = Convert.ToInt32(CommonHelper.Decode(id));
            var possessionModel = _possessionService.GetPossessionEntryDetailsByRID(ID);
            //possessionModel.AreaChange = possessionModel.AreaChange ?? "No";
            return View(possessionModel);
        }

        public ActionResult ViewPossessionOrder(string id)
        {
            int ID = Convert.ToInt32(CommonHelper.Decode(id));
            //var possessionModel = _possessionService.GetPossessionDetailsByRID(ID);
            var possessionModel = _possessionService.GetPossessionDetailsByRegistratioinId(ID);
            return View(possessionModel);
        }

        public ActionResult ViewPossessionRequestApprover(int id)
        {
            var data = new PropertyPossessionModel();
            if (id != 0)
            {
                data = _possessionService.GetPossessionDetailsByReqNo(id);
            }
            return View(data);
        }

        public ActionResult ViewBulkPossessionOrder(int id)
        {
            var possessionModel = _possessionService.GetBulkPossessionDetailsByRid(id);
            return View(possessionModel);
        }

        public JsonResult GetAllPossessionProperties([DataSourceRequest] DataSourceRequest request)
        {
            var listPossessionProperties = _possessionService.GetAllPossessionProperties(request);
            return Json(listPossessionProperties, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllPossessionEntryProperties([DataSourceRequest] DataSourceRequest request)
        {
            var listPossessionProperties = _possessionService.GetAllPossessionEntryProperties(request);
            return Json(listPossessionProperties, JsonRequestBehavior.AllowGet);
        }

        // Get all rids for Possession Order
        public JsonResult GetAllLeaseDeedDoneRIDs([DataSourceRequest] DataSourceRequest Req)
        {
            var allRIds = _possessionService.GetAllRIDsLeaseDeedDone(Req);

            return Json(allRIds, JsonRequestBehavior.AllowGet);
        }
        // Get all rids for Possession Entry 
        public JsonResult GetAllPossLeaseDeedDoneRIDs([DataSourceRequest] DataSourceRequest Req)
        {
            var allRIds = _possessionService.GetAllPossRIDsLeaseDeedDone(Req);
            return Json(allRIds, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets details by RID
        /// </summary>
        /// <param name="rId">RID</param>
        /// <returns></returns>
        public JsonResult GetDetailsByRID(int rId)
        {
            var details = new PropertyPossessionModel();
            if (rId != 0)
            {
                details = _possessionService.GetPossessionDetailsByRID(rId);
                details = details ?? new PropertyPossessionModel();
            }
            return Json(details, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets Possession Entry details by RID
        /// </summary>
        /// <param name="rId">RID</param>
        /// <returns></returns>
        public JsonResult GetPossEntryDetailsByRID(int rId)
        {
            var details = new PropertyPossessionModel();
            if (rId != 0)
            {
                details = _possessionService.GetPossessionEntryDetailsByRID(rId);
                details = details ?? new PropertyPossessionModel();
            }
            return Json(details, JsonRequestBehavior.AllowGet);
        }

        // Add Possession Order to DB
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddPossessionOrder(PropertyPossessionModel propPossessionModel)
        {
            var result = _possessionService.AddUpdatePossession(propPossessionModel);

            return RedirectToAction("ManagePossessionOrder");
        }

        // Add Possession Entry to DB
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddPossessionEntry(PropertyPossessionModel propPossessionModel)
        {
            var result = _possessionService.AddUpdatePossessionEntry(propPossessionModel);
            //if (result == false)
            //{
            //    ModelState.AddModelError("ErrorMessage", "Possession has been done for this Property..");
            //    return View("AddPossessionEntry", propPossessionModel);
            //}
            return RedirectToAction("ManagePossessionEntry");
        }

        public ActionResult GetSchemeListForAllotment()
        {
            List<SchemeAllotmentModel> schemeList = _propertyAllotmentService.GetSchemeListForAllotment();
            return Json(schemeList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilterDepartmentOnScheme(int schemeId)
        {
            List<DepartmentAllotmentModel> department = _propertyAllotmentService.FilterDepartmentOnScheme(schemeId);
            return Json(department, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPossessionPropertiesBySchemeDept([DataSourceRequest] DataSourceRequest request, int SchemeId, int DepartmentId)
        {
            var listPossessionProperties = _possessionService.GetPossessionPropertiesBySchemeDept(request, SchemeId, DepartmentId);
            return Json(listPossessionProperties, JsonRequestBehavior.AllowGet);
        }
        // Add Bulk Possession Order to DB
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddBulkPossessionOrder(List<string> rIds)
        {
            var status = false;
            if (rIds != null)
            {
                status = _possessionService.AddBulkPossession(rIds);
                //return RedirectToAction("ManagePossessionOrder");
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        //// Print Possession Orders
        //public ActionResult PrintPossessionOrder(int rid)
        //{
        //    var newLst = new List<int>();
        //    newLst.Add(rid);
        //    var listModelsToPrintPossession = _possessionService.GetModelToPrintPossessionOrder(newLst);
        //    if (listModelsToPrintPossession != null)
        //    {
        //        _templateParserService.GetParsedHTML(listModelsToPrintPossession[0], "PossessionOrder.cshtml");
        //    }
        //    return Json(false, JsonRequestBehavior.AllowGet);
        //}

        // // Print Possession Orders
        //[AllowAnonymous]
        //public ActionResult PrintBulkPossessionOrder(List<int> rIds)
        //{
        //    if (rIds != null)
        //    {
        //       var listModelsToPrintPossession = _possessionService.GetModelToPrintPossessionOrder(rIds);
        //    }
        //    return Json(false, JsonRequestBehavior.AllowGet);
        //}

        public decimal GetOneTimeExcessCharge(decimal changedArea, int propertyId)
        {
            decimal excessCharge = 0;
            excessCharge = _possessionService.GetOneTimeExcessCharge(changedArea, propertyId);
            return excessCharge;
        }

        // To Generate Mortgage letter
        public ActionResult PrintPossessionOrder(int rid, int departmentId)
        {
            var lst = _generalService.GenerateLetterFromDb(rid, Convert.ToInt32(LetterTemplate.PossessionOrder), departmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintPossessionLetter(int rid, int departmentId)
        {
            var lst = _generalService.GenerateLetterFromDb(rid, Convert.ToInt32(LetterTemplate.PossessionLetter), departmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadPossessionLetter(int rid, int departmentId, string possession)
        {
            var letter = string.Empty;
            if (possession == PossessionType.Order.ToString())
            {
                letter = _generalService.GenerateLetterFromDb(rid, Convert.ToInt32(LetterTemplate.PossessionOrder), departmentId);
            }
            if (possession == PossessionType.Entry.ToString())
            {
                letter = _generalService.GenerateLetterFromDb(rid, Convert.ToInt32(LetterTemplate.PossessionLetter), departmentId);
            }
            System.Text.StringBuilder strBody = new System.Text.StringBuilder(letter);
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Possession.doc");
            Response.ContentType = "application/vnd.ms-word ";
            Response.Charset = string.Empty;
            StringWriter sw = new StringWriter(strBody);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("ManagePossessionEntry");
        }

        public JsonResult GetPossessionDataByApproverId([DataSourceRequest]DataSourceRequest request, PropertyPossessionModel model)
        {
            var data = _possessionService.GetPossessionDataByApproverId(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdatePossessionRequest(PropertyPossessionModel model)
        {
            int flag = _possessionService.UpdatePossessionRequest(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}