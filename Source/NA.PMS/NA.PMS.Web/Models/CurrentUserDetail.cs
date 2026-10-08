
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NA.PMS.Model;

namespace NA.PMS.Web.Models
{
    public class CurrentUserDetail
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }      
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public Nullable<int> RoleId { get; set; }
        //public string RoleName { get; set; }
        public string RoleType { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public int MenuId { get; set; }

        public UmRoleMaster RoleMaster { get; set; }
        public List<UmRoleMaster> MultiRole { get; set; }
        public List<ApplicationMaster> ApplicationMaster { get; set; }
        public List<MenuMaster> MenuMaster { get; set; }
        public List<DepartmentMaster> DepartmentMaster { get; set; }
    }

}