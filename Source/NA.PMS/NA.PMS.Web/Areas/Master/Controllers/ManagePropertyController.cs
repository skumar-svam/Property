using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Service;
using Kendo.Mvc.Extensions;
using NA.PMS.Model;
using NA.PMS.Web.Controllers;
using MvcSiteMapProvider;
using NA.PMS.Web.Models;
using NA.PMS.Common;



namespace NA.PMS.Web.Areas.Master.Controllers
{
    public class ManagePropertyController : WebBaseController
    {
        IGeneralService _generalService;
        IMastersService _mastersService;
        static int menuKey = (int)Common.ScreenMenuKey.ManageProperty;
        //static Uri url;
        public ManagePropertyController(IMastersService mastersService, IGeneralService generalService)
        {
            _generalService = generalService;
            _mastersService = mastersService;
        }
        // GET: Property
        //public void SetRolePrmision(int menuKey)
        //{
        //    if (menuKey != 0)
        //    {
        //        var loginUser = (CurrentUserDetail)Session["CurrentUser"];
        //        if (loginUser != null)
        //        {
        //            foreach (var Role in loginUser.MenuMaster)
        //            {
        //                if (Role != null && Role.MenuId == menuKey)
        //                {
        //                    ViewBag.EditMenuVal = Role.IsUpdate;
        //                    ViewBag.AddMenuVal = Role.IsWrite;
        //                    ViewBag.DeleteMenuVal = Role.Isdelete;
        //                    ViewBag.ReadOnlyMenu = Role.IsRead;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            RedirectToAction("Login", "Account", new { area = "" });
        //        }
        //    }
        //    else
        //    {
        //        RedirectToAction("Login", "Account", new { area = "" });
        //    }
        //}

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Manage()
        {
            if (menuKey != 0)
            {
                var setPrmision = CommonMethords.SetRolePrmision(menuKey);
                if (setPrmision != null)
                {
                    //foreach (var Role in loginUser.MenuMaster)
                    //{
                    //    if (Role != null && Role.MenuId == menuKey)
                    //    {
                    ViewBag.EditMenuVal = setPrmision.EditMenuVal;
                    ViewBag.AddMenuVal = setPrmision.AddMenuVal;
                    ViewBag.DeleteMenuVal = setPrmision.DeleteMenuVal;
                    ViewBag.ReadOnlyMenu = setPrmision.ReadOnlyMenu;
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

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Document()
        {
            return View();
        }

        public ActionResult ManagePropertyRate()
        {
            return View();
        }

        public ActionResult ManagePropertyType()
        {
            return View();
        }

        public ActionResult ManageSubLeaseProperty()
        {
            return View();
        }

        public ActionResult ManagePropertyCost()
        {
            return View();
        }

       

        public JsonResult GetPropertyTypeByDepartment(int departmentId)
        {
            var list = _generalService.GetPropertyTypeByDepartment(departmentId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyTypeByDepartmentAsDataSource([DataSourceRequest]DataSourceRequest request, int departmentId)
        {
            var list = _generalService.GetPropertyTypeByDepartmentAsDataSource(request,departmentId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFloorAreaListByDepartment(int? departmentId)
        {
            if (departmentId != null)
            {
                var propertyType = _generalService.GetFloorAreaListByDepartment(Convert.ToInt32(departmentId));
                return Json(propertyType, JsonRequestBehavior.AllowGet);
            }
            else return Json(null, JsonRequestBehavior.AllowGet);
        }

        

        public JsonResult GetRegistrationIdListForSubLease([DataSourceRequest] DataSourceRequest request)
        {
            var rids = _mastersService.GetRegistrationIdListForSubLease(request);
            return Json(rids, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubLeasePropertyList([DataSourceRequest] DataSourceRequest request)
        {
            var data = _mastersService.GetSubLeasePropertyList(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubleasePropertyDetail(string id)
        {
            var data = _mastersService.GetSubleasePropertyDetail(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Getting all Properties Detail
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPropertyDetail([DataSourceRequest] DataSourceRequest request)
        {
            var propDetail = _mastersService.GetPropertyDetail_Read(request);
            return Json(propDetail, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// To check whether a valid Parent RID has been entered or not 
        /// </summary>
        /// <param name="parentPropRId">Parent RID</param>
        /// <param name="dept">Department ID</param>
        /// <returns></returns>
        public JsonResult CheckRIDValidity(int parentPropRId)
        {
            var flag = _mastersService.CheckRIDValidity(parentPropRId);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetParentPropertyDetailById(int rid)
        {
            var property = _mastersService.GetParentPropertyDetailById(rid);
            return Json(property, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save Sub-Lease data
        /// </summary>
        /// <param name="parentPropRId"></param>
        /// <param name="dept"></param>
        /// <param name="subLeasePropNo"></param>
        /// <param name="areaRate"></param>
        /// <param name="propArea"></param>
        /// <param name="propCost"></param>
        /// <returns></returns>
        public JsonResult SaveSubLease(int parentPropRId, int dept, string subLeasePropNo, decimal areaRate, decimal propArea, decimal propCost, int areaRangeId)
        {
            var flag = _mastersService.SaveSubLease(parentPropRId, dept, subLeasePropNo, areaRate, propArea, propCost, areaRangeId);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSubLeaseProperty(SubLeaseViewModel model)
        {
            int flag = _mastersService.SaveSubLeaseProperty(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveSubLease(int Id)
        {
            var flag = _mastersService.RemoveSubLease(Id);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SubLeasePlotActivation(int Id)
        {
            var flag = _mastersService.SubLeasePlotActivation(Id);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubLeaseData([DataSourceRequest] DataSourceRequest request, int rid)
        {
            var details = _mastersService.GetSubLeaseData(request, rid);
            return Json(details, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubLeasedProperty([DataSourceRequest] DataSourceRequest request, int? rid)
        {
            if (rid != null)
            {
                var details = _mastersService.GetSubLeasedProperty(request, Convert.ToInt32(rid));
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Displaying Add Property View
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            //Uri url = Request.UrlReferrer;
            //var objPropModel = new PropertyModel { schemeCollection = GetSchemes(), propertyTypeCollection = GetPropertyType(), locationTypeCollection = GetLocations() };
            //return View(objPropModel);
            var setPrmision = CommonMethords.SetRolePrmision(menuKey);
            if (setPrmision.AddMenuVal == true)
            {
                var objPropModel = new PropertyModel { schemeCollection = GetSchemes(), propertyTypeCollection = GetPropertyType(), locationTypeCollection = GetLocations() };
                return View(objPropModel);

            }
            else
            {
                return Redirect("/Account/Unauthorized");
            }



        }
        /// <summary>
        /// Displaying Edit Property View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditPropertyDetail(string id)
        {
            int refId = Convert.ToInt32(CommonHelper.Decode(id));
            var setPrmision = CommonMethords.SetRolePrmision(menuKey);
            //Constants.URL != null && 

            if (setPrmision.EditMenuVal == true)
            {
                var objPropModel = new PropertyModel();
                //objPropModel = _mastersService.GetPropertyDetail().Where(x => x.refId == refId).FirstOrDefault();
                objPropModel = _mastersService.GetPropertyDetailById(refId);//.Where(x => x.refId == refId).FirstOrDefault();
                objPropModel.schemeCollection = GetSchemes();

                var locationCollection = new List<SelectListItem>();
                var locationData = (from x in _mastersService.GetLocations()
                                    select new SelectListItem
                                    {
                                        Text = String.Format("{0}", x.locationName),
                                        Value = x.locationId.ToString()
                                    }).ToList();
                locationCollection.AddRange(locationData);
                objPropModel.locationTypeCollection = locationCollection;

                return View("Edit", objPropModel);
            }
            else
            {
                return Redirect("/Account/Unauthorized");
            }
        }
        /// <summary>
        /// Viewing Property Detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ViewPropertyDetail(string id)
        {
            int refId = Convert.ToInt32(CommonHelper.Decode(id));
            var setPrmision = CommonMethords.SetRolePrmision(menuKey);
            //Constants.URL != null && 

            if (setPrmision.ReadOnlyMenu == true)
            {
                var lstPropModel = new List<PropertyModel>();
                var objPropModel = new PropertyModel();
                lstPropModel = _mastersService.GetPropertyDetail();
                objPropModel = lstPropModel.Where(x => x.refId == refId).FirstOrDefault();
                //var user = _mastersService.GetUserDetailsById(id);
                return View("View", objPropModel);
            }
            else
            {
                return Redirect("/Account/Unauthorized");
            }
        }
        /// <summary>
        /// Soft Delete Property by Property Id from Database
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        public ActionResult Remove(int propertyId)
        {
            var flag = false;
            if (propertyId != 0)
            {
                flag = _mastersService.RemovePropertyDetail(propertyId);
            }
            return Json(flag);
        }
        /// <summary>
        /// Add or Edit Property detail
        /// </summary>
        /// <param name="propertyDetail"></param>
        /// <returns></returns>
        public ActionResult SavePropertyDetail(PropertyModel propertyDetail)
        {
            bool isAdded = _mastersService.SavePropertyDetail(propertyDetail);
            return RedirectToAction("Manage");
            //}
            //catch (Exception ex)
            //{
            //    log.Error("SaveFacilitator:- " + ex.Message);
            //    return RedirectToAction("error", "home");
            //}
        }
        /// <summary>
        /// Checking Duplicate value for Property
        /// </summary>
        /// <param name="secotrId"></param>
        /// <param name="blockId"></param>
        /// <param name="plotNo"></param>
        /// <param name="refId"></param>
        /// <returns></returns>
        public JsonResult CheckDuplicateProperty(int secotrId, int blockId, string plotNo, int refId)
        {
            var flag = false;
            flag = _mastersService.CheckDuplicateProperty(secotrId, blockId, plotNo, refId);
            return Json(flag);
        }

        #region Get Dropdown Values
        /// <summary>
        /// Getting Property Type Leasehold or Freehold
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRegistry()
        {
            var lst = _generalService.GetRegistry();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
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
        public JsonResult GetDepartment(int schemeId)
        {
            var schemeCollection = new List<SelectListItem>();
            var selectListItem = new SelectListItem { Text = "--Select--", Value = String.Empty };
            schemeCollection.Add(selectListItem);
            var data = (from x in _mastersService.GetDepartment(schemeId)
                        select new SelectListItem
                        {
                            Text = String.Format("{0}", x.departmentName),
                            Value = x.departmentId.ToString()
                        }).ToList();
            schemeCollection.AddRange(data);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentsForSubLease()
        {
            var lst = _mastersService.GetDepartmentsForSubLease();
            return Json(lst, JsonRequestBehavior.AllowGet);
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
        /// <summary>
        /// Get Sectors on the basis of SchemeId and Sector Id
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public JsonResult GetSectorBySchemeAndDeptId(int schemeId, int departmentId, int propTypeId)
        {
            var data = (from x in _mastersService.GetSectorBySchemeAndDeptId(schemeId, departmentId, propTypeId)
                        select new SelectListItem
                        {
                            Text = String.Format("{0}", x.sectorName),
                            Value = x.sectorId.ToString()
                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get Blocks on the basis of SchemeId and Sector Id
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public JsonResult GetBlockBySchemeAndDeptId(int schemeId, int departmentId, int propTypeId, int sectorId)
        {
            var data = (from x in _mastersService.GetBlockBySchemeAndDeptId(schemeId, departmentId, propTypeId, sectorId)
                        select new SelectListItem
                        {
                            Text = String.Format("{0}", x.blockName),
                            Value = x.blockId.ToString()
                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///  Get Floors on the basis of SchemeId and Sector Id 
        ///  Floors will only display in case of Housing as Department and Flat as Property Type
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public JsonResult GetFloorBySchemeAndDeptId(int schemeId, int departmentId, int propTypeId, int sectorId, int blockId)
        {
            var data = (from x in _mastersService.GetFloorBySchemeAndDeptId(schemeId, departmentId, propTypeId, sectorId, blockId)
                        select new SelectListItem
                        {
                            Text = String.Format("{0}", x.floorName),
                            Value = x.floorId.ToString()
                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get All Property Types
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SelectListItem> GetPropertyType()
        {
            var propertyTypeCollection = new List<SelectListItem>();
            var selectListItem = new SelectListItem { Text = "--Select--", Value = String.Empty };
            propertyTypeCollection.Add(selectListItem);
            var data = (from x in _mastersService.GetPropertyType()
                        select new SelectListItem
                        {
                            Text = String.Format("{0}", x.propertyType),
                            Value = x.propertyTypeId.ToString()
                        }).ToList();
            propertyTypeCollection.AddRange(data);
            return propertyTypeCollection;
        }
        /// <summary>
        /// Get All Locations
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SelectListItem> GetLocations()
        {
            var locationCollection = new List<SelectListItem>();
            var selectListItem = new SelectListItem { Text = "--Select--", Value = String.Empty };
            locationCollection.Add(selectListItem);
            var data = (from x in _mastersService.GetLocations()
                        select new SelectListItem
                        {
                            Text = String.Format("{0}", x.locationName),
                            Value = x.locationId.ToString()
                        }).ToList();
            locationCollection.AddRange(data);
            return locationCollection;
        }

        public JsonResult GetLandRate(int schemeId, int departmentId, int floorId, int blockId, int sectorId, int propertyTypeId)
        {
            var landRate = _mastersService.GetLandRate(schemeId, departmentId, floorId, blockId, sectorId, propertyTypeId);
            return Json(landRate, JsonRequestBehavior.AllowGet);
        }

        #endregion End Get Dropdown Values

        public ActionResult AddSubLeaseProperty()
        {
            return View();
        }

        #region Prperty Bank

        //public ActionResult GetPropertyDetail([DataSourceRequest] DataSourceRequest request)
        //{
        //    var propDetail = _mastersService.GetPropertyDetail().ToDataSourceResult(request);
        //    return Json(propDetail, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult AddPropertyBank()
        {
            return View();
        }
        public JsonResult GetAllDepartments()
        {
            var lst = _generalService.GetAllDepartments();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllPropTypes(int departmentId)
        {
            var lst = _generalService.GetPropTypes(departmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllSectors()
        {
            var lst = _generalService.GetSectorsByPropType();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllBlocks()
        {
            var lst = _generalService.GetAllBlocks();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFloorByDeptId(int departmentId)
        {
            var lst = _generalService.GetFloorByDeptId(departmentId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult AAA()
        //{
        //    var schemeCollection = new List<SelectListItem>();
        //    var selectListItem = new SelectListItem { Text = "--Select--", Value = String.Empty };
        //    schemeCollection.Add(selectListItem);
        //    var data = (from x in _mastersService.GetSchemes()
        //                select new SelectListItem
        //                {
        //                    Text = String.Format("{0}", x.schemeName),
        //                    Value = x.schemeId.ToString()
        //                }).ToList();
        //    schemeCollection.AddRange(data);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        /// <summary>
        /// Getting all Properties Bank Detail
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPropertyBankDetail([DataSourceRequest] DataSourceRequest request, int? schemeId, int? deptId, int? propertyTypeId, int? sectorId, int? blockId, int? floorId)
        {
            var propDetail = _mastersService.GetPropertyBankDetail(schemeId, deptId, propertyTypeId, sectorId, blockId, floorId).ToDataSourceResult(request);
            return Json(propDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPropertyBankDetailAdd([DataSourceRequest] DataSourceRequest request, int? schemeId, int? deptId, int? propertyTypeId, int? sectorId, int? blockId, int? floorId)
        {
            var propDetail = _mastersService.GetPropertyBankDetailAdd(schemeId, deptId, propertyTypeId, sectorId, blockId, floorId).ToDataSourceResult(request);
            return Json(propDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetachedPropertyDetail([DataSourceRequest] DataSourceRequest request, int? schemeId, int? deptId, int? propertyTypeId, int? sectorId, int? blockId, int? floorId)
        {
            var detachedPropertyDetail = _mastersService.GetDetachedPropertyDetail(schemeId, deptId, propertyTypeId, sectorId, blockId, floorId).ToDataSourceResult(request);
            return Json(detachedPropertyDetail, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Displaying Edit Property View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditPropertyBankDetail(int id)
        {
            var setPrmision = CommonMethords.SetRolePrmision(menuKey);
            //Constants.URL != null && 

            if (setPrmision.EditMenuVal == true)
            {
                var objPropModel = new PropertyModel();
                objPropModel = _mastersService.GetPropertyBankDetailAdd(null, null, null, null, null, null).Where(x => x.refId == id).FirstOrDefault();

                return View("EditPropertyBank", objPropModel);
            }
            else
            {
                return Redirect("/Account/Unauthorized");
            }
        }

        public ActionResult AttachProperties(List<int> rIds, string refIds, int schemeId, string registry)
        {
            if (refIds != null)
            {
                var listModelsToPrintPossession = _mastersService.AttachProperties(rIds, refIds, schemeId, registry);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DetachProperties(List<int> refIds)
        {
            if (refIds != null)
            {
                var listModelsToPrintPossession = _mastersService.DetachProperties(refIds);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Soft Delete Property by Property Id from Database
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        public ActionResult RemovePropertyBank(int propertyId)
        {
            var flag = false;
            if (propertyId != 0)
            {
                flag = _mastersService.RemovePropertyDetail(propertyId);
            }
            return Json(flag);
        }

        /// <summary>
        /// Add or Edit Property detail
        /// </summary>
        /// <param name="propertyDetail"></param>
        /// <returns></returns>
        public ActionResult SavePropertyBankDetail(PropertyModel propertyDetail)
        {
            bool isAdded = _mastersService.SavePropertyDetail(propertyDetail);
            return RedirectToAction("AddPropertyBank");
        }

        public ActionResult AddPropertyToScheme()
        {
            var setPrmision = CommonMethords.SetRolePrmision(menuKey);
            if (setPrmision.AddMenuVal == true)
            {
                var objPropModel = new PropertyModel { schemeCollection = GetSchemes(), propertyTypeCollection = GetPropertyType(), locationTypeCollection = GetLocations() };
                return View(objPropModel);

            }
            else
            {
                return Redirect("/Account/Unauthorized");
            }
        }
        #endregion

        public ActionResult Update()
        {
            return View();
        }

        public ActionResult SaveSectorBlock(SectorBlock objSectorBlock)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            objSectorBlock.createdBy = loginUser.UserID.ToString();
            bool isAdded = _mastersService.AddSectorBlock(objSectorBlock);
            TempData["MesgAdd"] = isAdded;
            return RedirectToAction("AddSectorBlock");
        }

        public JsonResult GetSectorBlock()
        {
            var lst = _mastersService.GetDDLSectorBlock();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddProject()
        {
            return View();
        }

        //Get project by ParentRid for dropdown
        public JsonResult GetProjectsByPRid(int ParentPropertyId)
        {
            var data = _mastersService.GetProjectsByParentRid(ParentPropertyId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //Save Property Projects
        public JsonResult SavePropertyProject(ProjectViewModel model)
        {
            int flag = _mastersService.SavePropertyProject(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        //get Project Details by Rid for grid
        public ActionResult GetProjectByRid([DataSourceRequest] DataSourceRequest Req, int? Rid)
        {
            if (Rid != null)
            {
                var details = _mastersService.GetProjectsByRidForGrid(Req, Convert.ToInt32(Rid));
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Fetches RIDs for Project
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRIDsForProject([DataSourceRequest] DataSourceRequest request)
        {           
            var lst = _mastersService.GetRIDsForProject(request);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        //Update Project Status
        public JsonResult UpdateProjectStatus(ProjectModel ProjectModel)
        {
            var flag = _mastersService.UpdateProjectStatus(ProjectModel);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMasterSearchParameterAsDataSource([DataSourceRequest] DataSourceRequest request, PropertyViewModel model)
        {
            var data = _mastersService.GetMasterSearchParameterAsDataSource(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMasterPropertyList([DataSourceRequest] DataSourceRequest request, PropertyViewModel model)
        {
            var data = _mastersService.GetMasterPropertyList(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMasterPropertyCostList([DataSourceRequest] DataSourceRequest request, PropertyViewModel model)
        {
            var data = _mastersService.GetMasterPropertyCostList(request, model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyRateListAsDataSource([DataSourceRequest] DataSourceRequest request, PropertyViewModel model)
        {
            var list = _mastersService.GetPropertyRateListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePropertyRateDetail(PropertyViewModel model)
        {
            int flag = _mastersService.SavePropertyRateDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemovePropertyRateDetailById(PropertyViewModel model)
        {
            int flag = _mastersService.RemovePropertyRateDetailById(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyTypeList([DataSourceRequest] DataSourceRequest request, PropertyViewModel model)
        {
            var list = _mastersService.GetPropertyTypeList(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePropertyTypeByDepartment(PropertyViewModel model)
        {
            int flag = _mastersService.SavePropertyTypeByDepartment(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeStatusOfPropertyType(PropertyViewModel model)
        {
            int flag = _mastersService.ChangeStatusOfPropertyType(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDefaultSectorListAsDataSource([DataSourceRequest] DataSourceRequest request)
        {
            var list = _generalService.GetDefaultSectorListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDefaultBlockListAsDataSource([DataSourceRequest] DataSourceRequest request)
        {
            var list = _generalService.GetDefaultBlockListAsDataSource(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyIdList([DataSourceRequest] DataSourceRequest Request)
        {
            var pidList = _generalService.GetPropertyIdList(Request);
            return Json(pidList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegistrationIdList([DataSourceRequest] DataSourceRequest Request)
        {
            var List = _generalService.GetRegistrationIdList(Request);
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllSectorsForProperty()
        {
            var sectors = _generalService.GetAllSectors();
            return Json(sectors, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVillageIdList()
        {
            var villageList = _generalService.GetVillageIdList();
            return Json(villageList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllBlocksForProperty()
        {
            var blocks = _generalService.GetAllBlocks();
            return Json(blocks, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllFloorTypes()
        {
            var blocks = _generalService.GetAllFloors();
            return Json(blocks, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyDetailsById(int? propertyId)
        {
            var data = _mastersService.GetPropertyDetailsById(propertyId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePropertyDetail(PropertyDetailViewModel model)
        {
            var flag = _mastersService.UpdatePropertyDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyCostDetailsById(int? propertyId)
        {
            var data = _mastersService.GetPropertyCostDetailsById(propertyId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePropertyCostDetail(PropertyDetailViewModel model)
        {
            var flag = _mastersService.UpdatePropertyCostDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveAllottedPropertyDetail(PropertyViewModel model)
        {
            int flag = _mastersService.SaveAllottedPropertyDetail(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyDetailAsDataSource([DataSourceRequest] DataSourceRequest request, PropertyViewModel model)
        {
            var list = _mastersService.GetPropertyDetailAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyCostDetailById(PropertyViewModel model)
        {
            PropertyViewModel data = _mastersService.GetPropertyCostDetailById(model);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SavePropertyDocuments(DocumentViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int flag = _mastersService.SavePropertyDocuments(model, files);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveDocumentsInTempSession(DocumentViewModel model, HttpPostedFileBase tempFile)
        {
            int flag = _mastersService.SaveDocumentsInTempSession(model, tempFile);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentListFromTempSessionAsDataSource([DataSourceRequest] DataSourceRequest request, DocumentViewModel model)
        {
            var list = _mastersService.GetDocumentListFromTempSessionAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentTypeListAsDataSource([DataSourceRequest] DataSourceRequest request, DropdownViewModel model)
        {
            var list = _mastersService.GetDocumentTypeListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveDocumentByIdFromTempSession(DocumentViewModel model)
        {
            int flag = _mastersService.RemoveDocumentByIdFromTempSession(model);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExistingDocumentListAsDataSource([DataSourceRequest] DataSourceRequest request, DocumentViewModel model)
        {
            var list = _mastersService.GetExistingDocumentListAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}