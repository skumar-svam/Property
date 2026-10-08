using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class LoginUserDetail
    {
        public int UserRefId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        //public string Password { get; set; }
        public string Comments { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        //public RoleMaster RoleMaster { get; set; }
        public List<DepartmentMaster> DepartmentList { get; set; }
        public List<RoleMaster> RolesList { get; set; }
        public List<ApplicationMaster> ApplicationsList { get; set; }
        public List<MenuMaster> MenusList { get; set; }
    }


    public class RoleMaster
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleType { get; set; }
        public string RoleDescription { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public List<ApplicationMaster> ApplicationsList { get; set; }
    }

    public class ApplicationMaster
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationUrl { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public List<MenuMaster> MenusList { get; set; }
    }

    //public class MenuMaster
    //{
    //    public int MenuId { get; set; }
    //    public Nullable<int> MenuPathId { get; set; }
    //    public string MenuName { get; set; }
    //    public Nullable<int> MenuParentId { get; set; }
    //    public Nullable<int> ApplicationId { get; set; }
    //    public Nullable<bool> IsActive { get; set; }
    //    public string CreatedBy { get; set; }
    //    public Nullable<System.DateTime> CreatedDate { get; set; }
    //    public string ModifiedBy { get; set; }
    //    public Nullable<System.DateTime> ModifiedDate { get; set; }

    //    public ApplicationMaster ApplicationMaster { get; set; }
    //}

    public class DepartmentMaster
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<bool> Status { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }

    public class NoidaCustomerModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public string UserEmail { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> IsLocked { get; set; }
    }
}
