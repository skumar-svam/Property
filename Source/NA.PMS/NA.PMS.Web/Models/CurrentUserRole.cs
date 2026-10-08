using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NA.PMS.Web.Models
{
    public class CurrentUserRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string RoleType { get; set; }
    }
}