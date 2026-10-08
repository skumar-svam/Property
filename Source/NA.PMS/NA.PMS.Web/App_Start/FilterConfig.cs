using NA.PMS.Web.Filters;
using System.Web.Mvc;

namespace NA.PMS.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new HandleLoggingAttribute());
            //filters.Add(new ActiveUserRoleAttribute());
        }
    }
}