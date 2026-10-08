using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NA.PMS.Web.Models
{
    public class CurrentUserApplication
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationUrl { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public int MenuId { get; set; }
        public List<CurrentUserMenu> UserMenus { get; set; }
    }
}