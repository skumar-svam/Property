using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Service;
using NA.PMS.Model;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Web.Models;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Web.Mvc.Filters;
using NA.PMS.Web.Controllers;
using NA.PMS.Common;

namespace NA.PMS.Web.Areas.Master.Controllers
{
    public class ManageRefundsController : WebBaseController
    {
        static int menuKey = (int)Common.ScreenMenuKey.ManageRefund;
        IMastersService _mastersService;
        public ManageRefundsController(IMastersService mastersService)
        {
            _mastersService = mastersService;
        }

        /// <summary>
        /// open ManageRefund page only
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageRefund()
        {
            if (menuKey != 0)
            {
                var loginUser = (CurrentUserDetail)Session["CurrentUser"];
                var setPrmision = CommonMethords.SetRolePrmision(menuKey);
                if (setPrmision != null)
                {
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

        /// <summary>
        /// Return all refud details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult GetRefundDetails([DataSourceRequest] DataSourceRequest request)
        {
            var refunds = _mastersService.GetSchemeRefundDetails(request);
            return Json(refunds, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get all schemes for dropdown in refund page
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllSchemesForRefund()
        {
            DataSourceRequest request = new DataSourceRequest();
            List<SchemeModel> scheme = _mastersService.GetAllSchemesForRefund();
            var data = scheme.ToDataSourceResult(request);
            return Json(scheme, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get all media types for dropdown in refund page
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMediaTypesForRefund()
        {

            List<MediaModel> mediaList = _mastersService.GetMediaTypesForRefund();
            return Json(mediaList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get department for department dropdown in refund page
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDepartmentForRefund()
        {
            List<DepartmentRefund> departmentList = _mastersService.GetDepartmentForRefund();
            return Json(departmentList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDepartmentOnSchemeForRefund(int schemeId)
        {
            List<DepartmentRefund> departmentList = _mastersService.GetDepartmentOnSchemeForRefund(schemeId);
            return Json(departmentList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// open addrefund page only
        /// </summary>
        /// <returns></returns>
        public ActionResult AddRefund()
        {
            var setPrmision = CommonMethords.SetRolePrmision(menuKey);
            if (setPrmision.AddMenuVal == true)
            {
                Refund rfnd = new Refund();
                return View(rfnd);
            }
            else
            {
                return Redirect("/Account/Unauthorized");
            }

            ////Uri url = Request.UrlReferrer;

            ////if (url != null)
            ////{
            //Refund rfnd = new Refund();
            //var node = SiteMaps.Current.CurrentNode;
            //if (node != null && node.ParentNode != null)
            //{
            //    SiteMaps.Current.CurrentNode.Title = "Add Refund";
            //    // node.ParentNode.Title = "Add Refund";
            //}

            //// SiteMaps.Current.CurrentNode.Title = "Add Refund";
            //return View(rfnd);
            ////}
            ////else
            ////{
            ////    return Redirect("/Account/Unauthorized");
            ////}
        }

        /// <summary>
        /// save all data of addrefund page and editrefund 
        /// </summary>
        /// <param name="refund"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveRefundForScheme(Refund refund)
        {
            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            var flag = false;
            flag = _mastersService.SaveRefundForScheme(refund, loginUser.UserName);
           
            return RedirectToAction("ManageRefund");
        }

        [HttpPost]
        public ActionResult AddRefundForScheme(int schemeId, int departmentId, string unit, decimal deduction, int interestRate, int daysAfterInterest, string deductionApplyOn, string refundDescription, int refundLockPeriod)
        {
            Refund rd = new Refund();
            rd.SchemeId = schemeId;
            rd.DepartmentId = departmentId;
            rd.Unit = unit;
            rd.Deduction = deduction;
            rd.InterestRate = interestRate;
            rd.DaysAfterInterest = daysAfterInterest;
            rd.DeductionApplyOn = deductionApplyOn;
            rd.RefundDescription = refundDescription;
            rd.RefundLockPeriod = refundLockPeriod;

            var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            var flag = false;
            flag = _mastersService.SaveRefundForScheme(rd, loginUser.UserName);
            if (flag)
            {
                return Json(flag, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("ManageRefund");
            }
            else
            {
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            //return RedirectToAction("ManageRefund");
        }

        /// <summary>
        /// Get all refund data based on refund id to editrefund page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[MvcSiteMapNode(Title = "Edit", Key = "Edit", PreservedRouteParameters = "id")]
        public ActionResult EditRefund(string id)
        {
            int ID = Convert.ToInt32(CommonHelper.Decode(id));
            var setPrmision = CommonMethords.SetRolePrmision(menuKey);
            if (setPrmision.EditMenuVal == true)
            {
                if (ID != 0)
                {
                    var node = SiteMaps.Current.CurrentNode;
                    Refund rfnd = new Refund();
                    var refnd = _mastersService.GetRefundDetailById(ID);
                    return View(refnd);
                }
                else
                {
                    return RedirectToAction("ManageRefund");
                }
            }
            else
            {
                return Redirect("/Account/Unauthorized");
            }
            //Uri url = Request.UrlReferrer;

            //if (url != null)
            //{
            //if (id != 0)
            //{

            //    var node = SiteMaps.Current.CurrentNode;
            //    //if (node != null && node.ParentNode != null)
            //    //{
            //    Refund rfnd = new Refund();
            //    var refnd = _mastersService.GetRefundDetailById(id);
            //    return View(refnd);
            //}
            //else
            //{
            //    return RedirectToAction("ManageRefund");
            //}
            //}
            //else
            //{
            //    return Redirect("/Account/Unauthorized");
            //}

        }


        /// <summary>
        /// soft removal of refund from managerefund page
        /// </summary>
        /// <param name="refundId"></param>
        /// <returns></returns>
        public ActionResult RemoveRefundForScheme(int refundId)
        {
            var flag = false;
            flag = _mastersService.RemoveRefundForScheme(refundId);
            if (flag == true)
            {
                //return RedirectToAction("ManageRefund");
                return Json(flag);
            }
            else
            {
                return Json(flag);
            }
        }

        /// <summary>
        /// show all refund details of particular refund id on viewrefund page based on refund id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public ActionResult ViewRefund(string id)
        {
            int ID = Convert.ToInt32(CommonHelper.Decode(id));
            var setPrmision = CommonMethords.SetRolePrmision(menuKey);
            if (setPrmision.ReadOnlyMenu == true)
            {
                Refund refund = _mastersService.GetRefundDetailById(ID);
                return View(refund);
            }
            else
            {
                return Redirect("/Account/Unauthorized");
            }
            //Uri url = Request.UrlReferrer;

            //if (url != null)
            //{
            //Refund refund = _mastersService.GetRefundDetailById(id);
            //var node = SiteMaps.Current.CurrentNode;
            //if (node != null && node.ParentNode != null)
            //{
            // node.ParentNode.Title = "View Refund";
            //SiteMaps.Current.CurrentNode.Title = "View Refund";
            //node.RouteValues["id"] = id;

            //node.ParentNode.RouteValues["id"] = id;

            //}
            //return View(refund);
            //}
            //else
            //{
            //    return Redirect("/Account/Unauthorized");
            //}
        }

        public ActionResult GetDeductionApplyOnForRefund()
        {
            List<DeductionApplyOnForRefund> daon = _mastersService.GetDeductionApplyOnForRefund();
            return Json(daon, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUnitTypeForRefund()
        {
            List<UnitTypeForRefund> units = _mastersService.GetUnitTypeForRefund();
            return Json(units, JsonRequestBehavior.AllowGet);
        }
    }
}