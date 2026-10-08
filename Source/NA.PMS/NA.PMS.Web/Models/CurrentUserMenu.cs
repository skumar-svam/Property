using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NA.PMS.Web.Models
{
    public class CurrentUserMenu
    {
        public int MenuId { get; set; }
        public Nullable<int> MenuPathId { get; set; }
        public string MenuName { get; set; }
        public Nullable<int> MenuParentId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}