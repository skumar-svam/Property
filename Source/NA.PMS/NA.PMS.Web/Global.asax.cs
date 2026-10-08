using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NA.PMS.Common.Logger;
using System.Web.Http;
using System.Configuration.Assemblies;
using System.Configuration;

namespace NA.PMS.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            Application["Totaluser"] = !(string.IsNullOrEmpty(ConfigurationManager.AppSettings["Counter"].Trim())) ? Convert.ToInt32(ConfigurationManager.AppSettings["Counter"]) : 0;
            //Improve performance  : Use only the View Engines that you need
            ViewEngines.Engines.Clear();
            IViewEngine razorEngine = new RazorViewEngine { FileExtensions = new[] { "cshtml" } };
            ViewEngines.Engines.Add(razorEngine);
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            LogException(exception);
        }
        protected void LogException(Exception exc)
        {
            if (exc == null)
                return;


            //ignore 404 HTTP errors
            var httpException = exc as HttpException;
            if (httpException != null && httpException.GetHttpCode() == 404)
                return;

            try
            {
                //log
                var logger = LoggingManager.GetLogInstance();
                logger.LogError("Global Exception", exc);
            }
            catch (Exception)
            {
                //don't throw new exception if occurs
            }
        }
        protected void Session_Start()
        {
            Application.Lock();
            Application.UnLock();
        }
    }
}
