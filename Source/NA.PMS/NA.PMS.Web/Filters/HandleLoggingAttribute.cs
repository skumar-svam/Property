using System.Web.Mvc;
using NA.PMS.Common.Logger;
using NA.PMS.Common.Extension;
using System;
using System.Web;
using System.Web.Routing;
using NA.PMS.Service;
using NA.PMS.Web.Models;

namespace NA.PMS.Web.Filters
{
    public class HandleLoggingAttribute : ActionFilterAttribute
    {
        protected ILogService Logger = LoggingManager.GetLogInstance();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Logger.IsInfoEnabled)
            {
                var actionDescriptor = filterContext.ActionDescriptor;
                string controllerName = actionDescriptor.ControllerDescriptor.ControllerName;
                string actionName = actionDescriptor.ActionName;
                string userName = filterContext.HttpContext.User.Identity.Name;
                
                var message = "Executing Action: {0} on Controller: {1} For User : {2}.".FormatWith(actionName, controllerName, userName);
                if (filterContext.RouteData.Values["id"] != null)
                {
                    string routeId = filterContext.RouteData.Values["id"].ToString();
                    message = "Executing Action: {0} on Controller: {1}  For User : {2} Given Route: {3}.".FormatWith( actionName, controllerName, userName, routeId);
                }

                Logger.LogInformation(message);
                base.OnActionExecuting(filterContext);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            if (Logger.IsInfoEnabled)
            {
                var actionDescriptor = filterContext.ActionDescriptor;
                string controllerName = actionDescriptor.ControllerDescriptor.ControllerName;
                string actionName = actionDescriptor.ActionName;
                string userName = filterContext.HttpContext.User.Identity.Name;

                var message = "Executed Action: {0} on Controller: {1} For User : {2}.".FormatWith(actionName, controllerName, userName);
                if (filterContext.RouteData.Values["id"] != null)
                {
                    string routeId = filterContext.RouteData.Values["id"].ToString();
                    message = "Executed Action: {0} on Controller: {1} For User : {2} Given Route: {3}.".FormatWith( actionName, controllerName, userName, routeId);
                }

                Logger.LogInformation(message);
            }
        }

        

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();
            base.OnResultExecuting(filterContext);
        }
    }

}