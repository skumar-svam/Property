using NA.PMS.NICService.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.NICServices
{
    public class AccountViewModel
    {
        public int UserRefId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string Comments { get; set; }
        public string UserProfile { get; set; }
        public Nullable<int> UserProfileId { get; set; }
        public Nullable<int> OptionalId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> OTP { get; set; }
        public Nullable<System.DateTime> OTPDate { get; set; }

        public NARoleMaster RoleMaster { get; set; }
        public List<int?> DepartmentIdList { get; set; }
        public List<NADepartmentMst> DepartmentList { get; set; }
        public List<NARoleMaster> RolesList { get; set; }
        public List<NAApplicationMst> ApplicationsList { get; set; }
        public List<NAActionMenu> MenusList { get; set; }
    }

    public class NARoleMaster
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleType { get; set; }
        public string RoleInDepartment { get; set; }
        public string RoleDescription { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
    public class NADepartmentMst
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<bool> Status { get; set; }
    }
    public class NAApplicationMst
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationUrl { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
    public class NAActionMenu
    {
        public int MenuId { get; set; }
        public Nullable<int> MenuPathId { get; set; }
        public string MenuName { get; set; }
        public Nullable<int> MenuParentId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsRead { get; set; }
        public Nullable<bool> IsWrite { get; set; }
        public Nullable<bool> IsUpdate { get; set; }
        public Nullable<bool> Isdelete { get; set; }
    }

    public class DropdownViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string RoleName { get; set; }
        public string ActionType { get; set; }
        public string ReturnType { get; set; }
        public string FilterType { get; set; }
        public string ChallanRefId { get; set; }
        public string TransactionId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> FilterTypeId { get; set; }
        public Nullable<int> ActionTypeId { get; set; }
        public Nullable<int> ReturnTypeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> PropertTypeId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<long> ReceiptId { get; set; }
        public Nullable<long> RefId { get; set; }
        public Nullable<DateTime> FilterDate { get; set; }
    }

    public class CurrentUserDetail
    {
        public int UserID { get; set; }
        public Nullable<int> KYAStatusId { get; set; }

        public int MenuId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<int> OptionalId { get; set; }
        public Nullable<int> UserProfileId { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public string RoleType { get; set; }
        //public string RoleName { get; set; }       
        public string UserProfile { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public UmRoleMaster RoleMaster { get; set; }
        public List<UmRoleMaster> MultiRole { get; set; }
        //public List<ApplicationMaster> ApplicationMaster { get; set; }
        //public List<MenuMaster> MenuMaster { get; set; }
        //public List<DepartmentMaster> DepartmentMaster { get; set; }
        //public List<CitizenServiceRequestModel> ServiceRequestModel { get; set; }
        //public List<NoidaCustomerModel> CustomerModels { get; set; }
        public List<int> RequestList { get; set; }
    }
}
