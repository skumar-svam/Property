using NA.PMS.Common.Logger;
using NA.PMS.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NA.PMS.Web.Areas.Customer.Controllers
{
    [HandleLogging]
    //[Authorize]
    public class PISWebBaseController : Controller
    {

        /// <summary>
        ///     On exception
        /// </summary>
        /// <param name="filterContext">Filter context</param>
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
                LogException(filterContext.Exception);
            base.OnException(filterContext);
        }

        /// <summary>
        ///     Log exception
        /// </summary>
        /// <param name="exc">Exception</param>
        private void LogException(Exception exc)
        {
            var logger = LoggingManager.GetLogInstance();
            logger.LogError("Controller Exception", exc);
        }
    }
}