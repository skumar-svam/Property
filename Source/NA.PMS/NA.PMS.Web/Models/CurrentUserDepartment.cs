using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NA.PMS.Web.Models
{
    public class CurrentUserDepartment
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<bool> Status { get; set; }
    }
}