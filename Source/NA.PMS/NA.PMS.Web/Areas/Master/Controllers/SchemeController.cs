using System;
using System.Linq;
using System.Web;
using NA.PMS.Model;
using Kendo.Mvc.UI;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Collections.Generic;
using NA.PMS.Service;
using NA.PMS.Web.Controllers;
using NA.PMS.Web.Models;
using MvcSiteMapProvider;

namespace NA.PMS.Web.Areas.Master.Controllers
{
    public class SchemeController : WebBaseController
    {
        IGeneralService _generalService;
        ISchemeService _schemeService;

        //static int menuKey = Int32.Parse(SiteMaps.Current.CurrentNode.Key);
        static int menuKey = (int)Common.ScreenMenuKey.ManageScheme;
        const string SCHEMESTATUS = "Scheme";
        public SchemeController(IGeneralService generalService, ISchemeService schemeService)
        {
            _generalService = generalService;
            _schemeService = schemeService;
        }

        public ActionResult Index(string id)
        {
            if (id != null)
            {
                int schemeId = Convert.ToInt32(CommonHelper.Decode(id));
                SchemeViewModel data = _schemeService.GetSchemeDetailById(new SchemeViewModel { SchemeId = schemeId });
                return View(data);
            }
            else
              return View();
        }

        // GET: Scheme
        public ActionResult Manage()
        {
            if (menuKey != 0)
            {
                var loginUser = (CurrentUserDetail)Session["CurrentUser"];
                if (loginUser != null)
                {
                    foreach (var Role in loginUser.MenuMaster)
                    {
                        if (Role != null && Role.MenuId == menuKey)
                        {
                            ViewBag.EditMenuVal = Role.IsUpdate;
                            ViewBag.AddMenuVal = Role.IsWrite;
                            ViewBag.DeleteMenuVal = Role.Isdelete;
                            ViewBag.ReadOnlyMenu = Role.IsRead;
                        }
                    }
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { area = "" });
                }
            }
            else
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
        }
        //Get All Schemes
        public ActionResult GetAllScheme([DataSourceRequest] DataSourceRequest req)
        {
            var dataByScheme = _schemeService.GetAllScheme(req);
            return Json(dataByScheme, JsonRequestBehavior.AllowGet);
        }
        //To created new screen.
        public ActionResult CreateScheme()
        {
            Scheme scheme = new Scheme();
            scheme.Departments = new Department();
            scheme.Quotas = new Quota();
            scheme.Rebate = new Rebate();
            scheme.Banks = new Banks();
            scheme.costModel = new CostModel();
            scheme.LandDevelopmentScheduleModel = new LandDevelopmentScheduleModel();
            var node = SiteMaps.Current.CurrentNode;
            if (node != null && node.ParentNode != null)
            {
                node.Title = "Create Scheme";
            }
            return View(scheme);
        }
        // Edit Scheme
        public ActionResult EditScheme(string Id)
        {
            int schemeID = Convert.ToInt32(CommonHelper.Decode(Id));
            var existingScheme = new Scheme();
            existingScheme = _schemeService.GetSChemeByID(schemeID);
            existingScheme.Departments = new Department();
            existingScheme.Quotas = new Quota();
            existingScheme.Rebate = new Rebate();
            existingScheme.Banks = new Banks();
            existingScheme.costModel = new CostModel();
            existingScheme.LandDevelopmentScheduleModel = new LandDevelopmentScheduleModel();
            return View(existingScheme);
        }
        // View Scheme
        public ActionResult ViewScheme(string Id)
        {
            int schemeID = Convert.ToInt32(CommonHelper.Decode(Id));
            if (menuKey != 0)
            {
                var loginUser = (CurrentUserDetail)Session["CurrentUser"];
                if (loginUser != null)
                {
                    foreach (var Role in loginUser.MenuMaster)
                    {
                        if (Role != null && Role.MenuId == menuKey)
                        {
                            ViewBag.EditMenuVal = Role.IsUpdate;
                            //ViewBag.AddMenuVal = Role.IsWrite;
                            //ViewBag.DeleteMenuVal = Role.Isdelete;
                            //ViewBag.ReadOnlyMenu = Role.IsRead;
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { area = "" });
                }
            }
            var existingScheme = new Scheme();
            existingScheme = _schemeService.GetSChemeByID(schemeID);
            existingScheme.Departments = new Department();
            existingScheme.Quotas = new Quota();
            existingScheme.Rebate = new Rebate();
            existingScheme.Banks = new Banks();
            existingScheme.costModel = new CostModel();
            existingScheme.LandDevelopmentScheduleModel = new LandDevelopmentScheduleModel();
            ViewBag.view = "Yes";
            return View(existingScheme);
        }
        //Delete Scheme
        [HttpPost]
        public ActionResult Delete(int scheId)
        {
            bool flag = false;
            if (scheId != 0)
            {
                flag = _schemeService.DeleteRecordById(scheId);
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        //To populate Cost Partial View.
        public ActionResult CostPartial()
        {
            return View();
        }
        //To bind Cost Partial view Grid
        public JsonResult GetAllCosts([DataSourceRequest] DataSourceRequest request)
        {
            var data = new DataSourceResult();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //To bind Department Partial view Grid
        public JsonResult GetAllDepartments()
        {
            var lst = _generalService.GetAllDepartments();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        ////To bind Department by scheme Partial view Grid
        public JsonResult GetAllDepartmentsByScheme(int schemeId)
        {
            var lst = _generalService.GetAllDepartmentsByScheme(schemeId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //To bind All interest rate.
        public JsonResult GetAllInterestRate()
        {
            var lst = _generalService.GetAllInterestRate();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //To bind All Penal rate.
        public JsonResult GetAllPenalRate()
        {
            var lst = _generalService.GetAllPenalRate();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //To bind All Frquency.
        public JsonResult GetAllFrquency()
        {
            var lst = _generalService.GetAllFrquency();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        // To bind Quota grid
        public JsonResult GetAllQuota()
        {
            var lst = _generalService.GetAllQuota();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //To bind unit dropdown.
        public JsonResult GetAllUnits()
        {
            var lst = _generalService.GetAllUnits();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //to bind rebate unit ddl.
        public JsonResult GetRebateUnits()
        {
            var lst = _generalService.GetRebateUnits();
            return Json(lst, JsonRequestBehavior.AllowGet);

        }
        //To Bind rebate type ddl
        public JsonResult GetRebateTypes()
        {
            var lst = _generalService.GetRebateTypes();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //To Bind PropTypes type ddl
        public JsonResult GetPropTypes(int depttID)
        {
            var lst = _generalService.GetPropTypes(depttID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllData()
        {
            var data = new DataSourceResult();
            return Json(data, JsonRequestBehavior.AllowGet);
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
        //To get all Sectors by prop type in ddl
        public JsonResult GetSectorsByPropType()
        {
            var lst = _generalService.GetSectorsByPropType();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        // To get all flooors type ddl.
        public JsonResult GetFloors(int depttID = 0)
        {
            var lst = _generalService.GetFloors(depttID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //To get all scheme type in ddl
        public JsonResult GetAllSchemeType()
        {
            var lst = _generalService.GetAllSchemeType();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        ////To get all blocks in ddl
        public JsonResult GetBlocks()
        {
            var lst = _generalService.GetAllBlocks();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //  To Add new scheme
        public JsonResult AddScheme(string schemeName, int schemeType, DateTime startDate, DateTime endDate, decimal formFee)
        {
            var flag = 0;
            var schemeID = _schemeService.AddScheme(schemeName, schemeType, startDate, endDate, ((CurrentUserDetail)Session["CurrentUser"]).UserID, formFee);
            if (schemeID != -1 && schemeID != 0)
            {
                flag = 1;
            }
            else
            {
                flag = schemeID;
            }
            return Json((new { flag = flag, scheme = schemeID }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateScheme(int schemeId, DateTime startDate, DateTime endDate, decimal formFee)
        {
            var schemeID = schemeId;
            var flag = _schemeService.UpdateScheme(schemeId, startDate, endDate, ((CurrentUserDetail)Session["CurrentUser"]).UserID, formFee);
            return Json((new { flag = flag, scheme = schemeID }), JsonRequestBehavior.AllowGet);
        }

        //  To Add new Department
        public JsonResult AddDeptt(int departmentId, double normalInterest, double penalInterest, int frequency, int noInstallment, int schemeId, string selectionType, double allotmentmoney = 0D, double installmentmoney = 0D, double leaseRent = 0D, double floorArearatio = 0D)
        {
            var flag = false;
            flag = _schemeService.AddDeptt(departmentId, normalInterest, penalInterest, frequency, noInstallment, allotmentmoney,
                installmentmoney, leaseRent, floorArearatio, schemeId, ((CurrentUserDetail)Session["CurrentUser"]).UserID, selectionType);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        //  To Add new Quota
        public JsonResult AddQuota(int quotaDepartmentId, int quotaId, string value, string UnitId, int schemeId)
        {
            int flag = 0;
            flag = _schemeService.AddQuota(quotaDepartmentId, quotaId, value, UnitId, schemeId, ((CurrentUserDetail)Session["CurrentUser"]).UserID);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        //  To Add new AddRebate
        public JsonResult AddRebate(int rebatedepartment, int rebateId, string rebateValue, string rebateUnitId, int schemeId)
        {
            int flag = 0;
            flag = _schemeService.AddRebate(rebatedepartment, rebateId, rebateValue, rebateUnitId, schemeId, ((CurrentUserDetail)Session["CurrentUser"]).UserID);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        //  To Add new AddCost
        public JsonResult AddCost(int depttID, int propType, int sector, int schemeId, int floor, int block, decimal processingFee, double landRate = 0D, double earnestmoney = 0D, double propCost = 0D, double civilCost = 0D, double totPropCost = 0D, double allotmentMoney = 0D, double leaseRent = 0D)
        {
            var flag = false;
            flag = _schemeService.AddCost(depttID, propType, sector, floor, block, processingFee, earnestmoney, propCost, landRate, civilCost, totPropCost, allotmentMoney, leaseRent, schemeId, ((CurrentUserDetail)Session["CurrentUser"]).UserID);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        //  To Add new AddBanks
        public JsonResult AddBank(int bankId, string branchName, int schemeId, string newaccountNumber, string newBank, string DdlAccountNumber, int branchId = 0)
        {
            var flag = false;
            if (bankId != -1 && branchId != -1 && branchId != 0)
            {
                newaccountNumber = DdlAccountNumber;
            }

            flag = _schemeService.AddBank(bankId, branchId, branchName, schemeId, ((CurrentUserDetail)Session["CurrentUser"]).UserID, newaccountNumber, newBank);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        //  To Activate scheme.
        public JsonResult ActivateScheme(int schemeId)
        {
            var flag = false;
            flag = _schemeService.ActivateScheme(schemeId);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        //  To get all deptt data
        public ActionResult GetDepttData([DataSourceRequest] DataSourceRequest request, int schemeId)
        {
            var data = _schemeService.GetDepttData(request, schemeId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //  To get all quota data
        public ActionResult GetQuotaData([DataSourceRequest] DataSourceRequest request, int schemeId)
        {
            var data = _schemeService.GetQuotaData(request, schemeId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //  To get all bank data
        public ActionResult GetBankData([DataSourceRequest] DataSourceRequest request, int schemeId)
        {
            var data = _schemeService.GetBankData(request, schemeId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //  To get all reabte data
        public ActionResult GetRebateData([DataSourceRequest] DataSourceRequest request, int schemeId)
        {
            var data = _schemeService.GetRebateData(request, schemeId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //  To get all cost data
        public ActionResult GetCostData([DataSourceRequest] DataSourceRequest request, int schemeId)
        {
            var data = _schemeService.GetCostData(request, schemeId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //  To get all selection type for ddl
        public JsonResult GetAllSelectionType()
        {
            var lst = _generalService.GetAllSelectionType();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //  To remove deptt record by id.
        public JsonResult RemoveDepttRecord(int refId, int schemeID)
        {
            var data = _schemeService.RemoveDepttRecord(refId, schemeID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //  To Remove Quota Recort record by id.
        public JsonResult RemoveQuotaRecord(int refId, int schemeID)
        {
            var data = _schemeService.RemoveQuotaRecord(refId, schemeID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //  To Remove Quota reb record by id.
        public JsonResult RemoveRebRecord(int refId, int schemeID)
        {
            var data = _schemeService.RemoveRebRecord(refId, schemeID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //  To Remove cost record by id.
        public JsonResult RemoveCostRecord(int refId, int schemeID)
        {
            var data = _schemeService.RemoveCostRecord(refId, schemeID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //  To Remove banks record by id.
        public JsonResult RemoveBankRecord(int refId, int schemeID)
        {
            var data = _schemeService.RemoveBankRecord(refId, schemeID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //  To Remove account number record by id.
        public JsonResult GetAccountNumber(int bankId, int branchId)
        {
            var data = _schemeService.GetAccountNumber(bankId, branchId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        ////  To cheque uniqeness of scheme name
        [AllowAnonymous]
        public JsonResult IsSchemeNameUnique(string SchemeName, int scheid)
        {
            bool flag = false;
            if (SchemeName != null)
            {
                flag = _schemeService.IsSchemeNameUnique(SchemeName, scheid);
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        ////  To cheque department name of scheme name
        public JsonResult IsDepatmnetNameUnique(int departmentID, int schemeID)
        {
            int type = 1;
            bool flag = false;
            if (departmentID != 0 && schemeID != 0)
            {
                flag = _schemeService.IsDepatmnetNameUnique(departmentID, schemeID, 0, type, 0, 0, 0);
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        ////  To cheque unique quota name of scheme name
        [AllowAnonymous]
        public JsonResult IsQuotaNameUnique(int departmentID, int schemeID, int quotaId)
        {
            int type = 2;
            bool flag = false;
            if (departmentID != 0 && schemeID != 0)
            {

                flag = _schemeService.IsDepatmnetNameUnique(departmentID, schemeID, type, quotaId, 0, 0, 0);
            }


            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        ////  To cheque unique rebate name of scheme name
        [AllowAnonymous]
        public JsonResult IsRebateNameUnique(int departmentID, int schemeID, int rebateId)
        {
            int type = 3;
            bool flag = false;
            if (departmentID != 0 && schemeID != 0)
            {

                flag = _schemeService.IsDepatmnetNameUnique(departmentID, schemeID, type, rebateId, 0, 0, 0);
            }


            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        ////  To cheque cost name quota name of scheme name
        [AllowAnonymous]
        public JsonResult IsCostNameUnique(int departmentID, int schemeID, int propTypes, int sector, int floorddl, int block)
        {
            int type = 4;
            bool flag = false;
            if (departmentID != 0 && schemeID != 0)
            {
                if (departmentID != (int)Common.Departmentenum.Housing)
                {
                    flag = _schemeService.IsCostNameUnique(departmentID, schemeID, propTypes, sector, floorddl, block);
                }
                else
                {
                    flag = _schemeService.IsDepatmnetNameUnique(departmentID, schemeID, type, propTypes, floorddl, block, sector);
                }

            }


            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        ////  To cheque bankName name of scheme name
        public JsonResult IsBankNameUnique(int departmentID, int schemeID, int branchIds = 0)
        {
            int type = 5;
            bool flag = false;
            if (departmentID != 0 && schemeID != 0)
            {

                flag = _schemeService.IsDepatmnetNameUnique(departmentID, schemeID, type, branchIds, 0, 0, 0);
            }


            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        ////  To cheque bankName unique for bank master entry.
        [AllowAnonymous]
        public JsonResult IsBankNameDuplicate(string bankName, int bankID = 0)
        {
            bool flag = false;
            if (bankName != null)
            {
                flag = _schemeService.IsBankNameUnique(bankID, bankName);
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        ////  To cheque Account Number.
        [AllowAnonymous]
        public JsonResult IsAccountNumberDuplicate(string accountNumber, int bankID)
        {
            bool flag = false;
            if (!string.IsNullOrEmpty(accountNumber))
            {
                flag = _schemeService.IsAccountNumberDuplicate(bankID, accountNumber);
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        #region Auto Schedule Land Development

        public ActionResult ManageLandDevelopment()
        {
            return View();
        }
        /// <summary>
        /// Get Data for Auto Schedule Land Development
        /// </summary>
        /// <param name="request"></param>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public ActionResult GetLandDevelopmentScheduleData([DataSourceRequest] DataSourceRequest request, int schemeId)
        {
            var data = _schemeService.GetLandDevelopmentScheduleData(request, schemeId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Bind Process Name
        /// </summary>
        /// <returns></returns>
        public JsonResult GetProcessData(int departmentId)
        {
            var lst = _generalService.GetProcessData(departmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Bind Trigger Process
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTriggerProcessData(int departmentId)
        {
            var lst = _generalService.GetTriggerProcessData(departmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Adding Auto Schedule for Land Development
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="processId"></param>
        /// <param name="duration"></param>
        /// <param name="triggerId"></param>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public JsonResult AddLandDevelopmentSchedule(int departmentId, int processId, int years, int months, int days, int triggerId, int schemeId)
        {
            int duration = (years * 365) + (months * 30) + (days);
            int flag = _schemeService.AddLandDevelopmentSchedule(departmentId, processId, duration, triggerId, schemeId, ((CurrentUserDetail)Session["CurrentUser"]).UserID);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Remove Land Development Record
        /// </summary>
        /// <param name="refId"></param>
        /// <returns></returns>
        public JsonResult RemoveLandDevelopment(int refId, int schemeID)
        {
            var data = _schemeService.RemoveLandDevelopment(refId, schemeID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Validating for duplicate entery on the basis of Scheme, Department , Process Name and Trigger Process
        /// </summary>
        /// <param name="departmentID"></param>
        /// <param name="processId"></param>
        /// <param name="schemeID"></param>
        /// <param name="triggerId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult IsLandDevelopmentUnique(int departmentID, int processId, int schemeID, int triggerId)
        {
            int type = 3;
            bool flag = false;
            if (departmentID != 0 && schemeID != 0)
            {

                flag = _schemeService.IsLandDevelopmentUnique(departmentID, processId, schemeID, triggerId, type);
            }


            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        //  To get all selection type for ddl
        public JsonResult GetAllStatus()
        {
            var lst = _generalService.BindDDL(SCHEMESTATUS);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetStatusUpdated(int schemeID)
        {
            var lst = _schemeService.GetStatusUpdated(schemeID);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}