using System.Web.Mvc;

namespace NA.PMS.Web.Areas.Online
{
    public class OnlineAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Online";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Online_default",
                "Online/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}