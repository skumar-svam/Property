using System.Web.Mvc;

namespace NA.PMS.Web.Areas.NIC
{
    // National Informatics Centre [NIC]
    public class NICAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "NIC";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "NIC_default",
                "NIC/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}