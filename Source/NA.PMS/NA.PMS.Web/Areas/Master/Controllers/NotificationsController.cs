using NA.PMS.Model;
using NA.PMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Web.Controllers;

using NA.PMS.Web.Models;
using MvcSiteMapProvider;
using NA.PMS.Common;

namespace NA.PMS.Web.Areas.Master.Controllers
{
    public class NotificationsController : WebBaseController
    {
        IMastersService _MastersService;
        static int menuKey = (int)Common.ScreenMenuKey.ManageNotifications;
        public NotificationsController(IMastersService mastersService)
        {
            _MastersService = mastersService;
        }

        public ActionResult Manage()
        {
            if (menuKey != 0)
            {
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
            //if (menuKey != 0)
            //{
            //    var loginUser = (CurrentUserDetail)Session["CurrentUser"];
            //    if (loginUser != null)
            //    {
            //        foreach (var Role in loginUser.MenuMaster)
            //        {
            //            if (Role != null && Role.MenuId == menuKey)
            //            {
            //                ViewBag.EditMenuVal = Role.IsUpdate;
            //                ViewBag.AddMenuVal = Role.IsWrite;
            //                ViewBag.DeleteMenuVal = Role.Isdelete;
            //                ViewBag.ReadOnlyMenu = Role.IsRead;
            //            }
            //        }
            //        return View();
            //    }
            //    else
            //    {
            //        return RedirectToAction("Login", "Account", new { area=""});
            //    }               
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account", new {area= ""});
            //}
            //return View();
        }

        public ActionResult Add()
        {

            var setPrmision = CommonMethords.SetRolePrmision(menuKey);
            //Constants.URL != null && 

            if (setPrmision.AddMenuVal == true)
            {
                return View();

            }
            else
            {
                return Redirect("/Account/Unauthorized");
            }

            //Uri url = Request.UrlReferrer;
            //if (url != null)
            //{
                //return View();
            //}
            //else
            //{
            //    return Redirect("/Account/Unauthorized");
            //}
        }

        public ActionResult Edit(DataSourceRequest request, string id)
        {
            int notifId = Convert.ToInt32(CommonHelper.Decode(id));
            var setPrmision = CommonMethords.SetRolePrmision(menuKey);
            if (setPrmision.EditMenuVal == true)
            {
                
                var objNotificationsModel = new NotificationsModel();
                var lstNotificationsModel = _MastersService.GetAllNotifications(request);
                objNotificationsModel = lstNotificationsModel.Where(x => x.notificationID == notifId).FirstOrDefault();
                return View(objNotificationsModel);
            }
            else
            {
                return Redirect("/Account/Unauthorized");
            }
            
        }

        public ActionResult ViewNotification(string id)
        {
            int notifId = Convert.ToInt32(CommonHelper.Decode(id));
            var setPrmision = CommonMethords.SetRolePrmision(menuKey);
            if (setPrmision.ReadOnlyMenu == true)
            {
                NotificationsModel notification = _MastersService.ViewSchemeNotification(notifId);
                return View(notification);
            }
            else
            {
                return Redirect("/Account/Unauthorized");
            }
        }

        public ActionResult GetAllNotifications([DataSourceRequest] DataSourceRequest request)
        {
            //var notifications = _MastersService.GetAllNotifications(request);
            var notifications = _MastersService.GetAllNotifications_Read(request);
            //var data = new DataSourceResult();
            //data = notifications.ToDataSourceResult(request);
            return Json(notifications, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetAllSchemesForNotifications(DataSourceRequest request)
        {
            List<SchemeModel> lstSchemes = _MastersService.GetAllSchemesForNotifications(request);
            return Json(lstSchemes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMediaTypeForNotification()
        {
            List<MediaModel> mediaList = _MastersService.GetMediaTypeForNotification();
            return Json(mediaList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentModeForNotifications()
        {
            List<PaymentModel> paymentList = _MastersService.GetPaymentModeForNotifications();
            return Json(paymentList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveNotificationForScheme(NotificationsModel notification)
        {
            var flag = _MastersService.SaveSchemeNotification(notification);

            if (flag)
            {               
                TempData["MesgAdd"] = flag;
                return RedirectToAction("Manage");
            } 
            else
            {
                //TempData["MesgAdd"] = flag;
                return RedirectToAction("Add");
            }
        }

        public JsonResult SaveNotificationForScheme(int schemeId, DateTime notificationStart, DateTime notificationEnd,DateTime publishDate, List<int?> schemeMediaType, Decimal bookletCost, List<int?> schemePaymentType, int oldNotificationID, string notificationName, int notificationId)
        {
            NotificationsModel notification = new NotificationsModel();
            notification.schemeID = schemeId;
            notification.notificationStart = notificationStart;
            notification.notificationEnd = notificationEnd;
            notification.publishDate = publishDate;
            notification.schemeMediaType = schemeMediaType;
            notification.bookletCost = bookletCost;
            notification.schemePaymentType = schemePaymentType;
            notification.oldNotificationID = oldNotificationID;
            notification.notificationName = notificationName;
            notification.notificationID = notificationId;
            int flag = 0;
            var user = (CurrentUserDetail)Session["CurrentUser"];
            //if (ModelState.IsValid)
            //{
            if (user != null)
            {
                flag = _MastersService.SaveNotificationForScheme(notification, user.UserName);
            }
            //TempData["MesgAdd"] = flag;
            ////}
            //if (flag == 1)
            //{
            //    return Json(flag);
            //}
            //else if (flag == 2)
            //{
            //    TempData["MesgAdd"] = flag;
            //    if (notification.notificationID == 0)
            //        return Json(flag);
            //    else
            //    {
            //        return Json(flag);
            //    }
            //}
            //else
            //{
            //    return Json(flag);
            //}
            //return RedirectToAction("Manage");
            return Json(flag);
        }

        public JsonResult RemoveNotification(int notificationID)
        {
            var flag = false;
            flag = _MastersService.RemoveNotification(notificationID);
            return Json(flag);
        }

        public JsonResult GetOldNotifications(int schemeId, int notiId)
        {
            List<NotificationsModel> oldNotificationCollection = _MastersService.GetOldNotifications(schemeId, notiId);
            return Json(oldNotificationCollection, JsonRequestBehavior.AllowGet);
        }


        public JsonResult CompareNotiStartAndEndDate(string notiStartDate, string notiEndDate, int schemeId)
        {
            var flag = false;
            flag = _MastersService.CompareNotiStartAndEndDate(Convert.ToDateTime(notiStartDate), Convert.ToDateTime(notiEndDate), schemeId);
            return Json(flag);
        }

        public JsonResult GetSchemeDateForNotification(int schemeId)
        {
            NotificationsModel model = _MastersService.GetSchemeDateForNotification(schemeId);
            return Json(model);
        }
        public JsonResult CheckNotificationPublishDate(int schemeId, DateTime notificationStart, DateTime notificationEnd, DateTime publishDate)
        {
            bool flag = !_MastersService.CheckNotificationPublishDate(schemeId, notificationStart, notificationEnd, publishDate);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}