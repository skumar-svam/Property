using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NA.PMS.UserManagement
{
    public class NDAUserViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> UserRefId { get; set; }
        public Nullable<int> UserOptionalId { get; set; }
        public Nullable<int> UserProfileId { get; set; }
        public Nullable<int> RoleId { get; set; } //Used in Role-User Mapping
        public Nullable<int> RoleTypeId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> SubDepartmentId { get; set; }
        public Nullable<int> ActionTypeId { get; set; }
        public Nullable<int> FilterTypeId { get; set; }
        public Nullable<int> ReturnTypeId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> PropertyId { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string RoleType { get; set; }
        public string RoleName { get; set; } //Used in Role-User Mapping 
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string EmployeeCode { get; set; }
        public string UserProfile { get; set; }
        public string Department { get; set; }
        public string DepartmentsName { get; set; } // for multiple department
        public string SubDepartment { get; set; }
        public string AplicationName { get; set; }
        public string CreatedBy { get; set; }
        public string ActionType { get; set; }
        public string FilterType { get; set; }
        public string ReturnType { get; set; }
        public string Message { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }

        public string TxtPropertyId { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo { get; set; }
        public string UserIdFileType { get; set; }
        public string UserIdFileName { get; set; }
        public string PropertyFileType { get; set; }
        public string PropertyFileName { get; set; }
        public string Remarks { get; set; }
        public string UserStatus { get; set; }
        public string FieldName { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }

        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public Nullable<DateTime> LastPasswordChangeDate { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<bool> IsLocked { get; set; }
        public Nullable<bool> IsFirstTimeActivated { get; set; }
        public Nullable<bool> IsRejected { get; set; }
        public Nullable<bool> IsIdFileUploaded { get; set; }
        public Nullable<bool> IsPropertyFileUploaded { get; set; }

        public List<int> DepartmentIdList { get; set; }
        public List<string> DepartmentNameList { get; set; }
        public List<NDACheckBoxViewModel> DepartmentList { get; set; }
        public List<NDACheckBoxViewModel> SubDepartmentList { get; set; }
    }

    public class NDARoleViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> UserRefId { get; set; }
        public Nullable<int> UserOptionalId { get; set; }
        public Nullable<int> UserProfileId { get; set; }
        public Nullable<int> RoleId { get; set; } //Used in Role-User Mapping
        public Nullable<int> RoleTypeId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> SubDepartmentId { get; set; }
        public Nullable<int> ActionTypeId { get; set; }
        public Nullable<int> FilterTypeId { get; set; }
        public Nullable<int> ReturnTypeId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> MenuId { get; set; }
        public Nullable<int> MenuPathId { get; set; }
        public Nullable<int> MenuParentId { get; set; }

        public string UserName { get; set; }
        public string FullName { get; set; }
        public string RoleType { get; set; }
        public string RoleName { get; set; } //Used in Role-User Mapping 
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string EmployeeCode { get; set; }
        public string UserProfile { get; set; }
        public string Department { get; set; }
        public string DepartmentsName { get; set; } // for multiple department
        public string SubDepartment { get; set; }
        public string AplicationName { get; set; }
        
        public string ActionType { get; set; }
        public string FilterType { get; set; }
        public string ReturnType { get; set; }
        public string Message { get; set; }
        public string Password { get; set; }

        public string RoleInDepartment { get; set; }
        public string RoleDescription { get; set; }
        public string CreatedBy { get; set; }
        public string ApplicationUrl { get; set; }
        public string MenuName { get; set; }
        public string ParentMenuName { get; set; }
        public string Remarks { get; set; }
        public string UserStatus { get; set; }
        public string PropertyNo { get; set; }
        public string MenuInHtml { get; set; }

        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public Nullable<DateTime> LastPasswordChangeDate { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<bool> IsLocked { get; set; }
        public Nullable<bool> IsFirstTimeActivated { get; set; }
        public Nullable<bool> IsRejected { get; set; }
        public Nullable<bool> IsIdFileUploaded { get; set; }
        public Nullable<bool> IsPropertyFileUploaded { get; set; }

        public List<int> DepartmentIdList { get; set; }
        public List<int> ApplicationIdList { get; set; }
        public List<int> ParentMenuIdList { get; set; }

        public List<string> ParentMenuNameList { get; set; }
        public List<string> DepartmentNameList { get; set; }

        public List<NDAMenuViewModel> ParentMenuList { get; set; }
        public List<NDAMenuViewModel> AssignedMenuList { get; set; }

        public List<NDACheckBoxViewModel> DepartmentList { get; set; }
        public List<NDACheckBoxViewModel> SubDepartmentList { get; set; }
    }

    public class NDAMenuViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> MenuId { get; set; }
        public Nullable<int> MenuPathId { get; set; }
        public Nullable<int> ParentMenuId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public string MenuName { get; set; }
        public string ParentMenuName { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string ApplicationName { get; set; }
        public string RoleName { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsRead { get; set; }
        public Nullable<bool> IsWrite { get; set; }
        public Nullable<bool> IsUpdate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<bool> IsChecked { get; set; }
        
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }

        public List<NDAMenuViewModel> MenuList { get; set; }
    }

    public class NDACheckBoxViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> CheckBoxId { get; set; }
        public bool IsChecked { get; set; }
        public string CheckBoxName { get; set; }
    }
}
