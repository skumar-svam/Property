using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace NA.PMS.Model
{
    public class RolesModel
    {
        public int RoleId { get; set; }
        [DisplayName("Application Name")]
        //[Required(ErrorMessage = "Please select an application.")]
        public string ApplicationName { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        //[Required(ErrorMessage = "The role name field is required.")]
        //[Remote("IsRoleNameUnique", "ManageAdminRoles", AdditionalFields = "ApplicationName, ApplicationId,RoleId", HttpMethod = "HttpPost", ErrorMessage = "Role for the selected application has been already assigned.")]
        [DisplayName("Role Name")]
        [StringLength(100, ErrorMessage = "Role Name cannot be more than 100 characters.")]
        [RegularExpression("^[A-Za-z0-9 ]+$", ErrorMessage = "Name should not contain special character.")]
        public string RoleName { get; set; }
        //[Required(ErrorMessage = "The role description is required.")]
        [DisplayName("Role Description")]
        [StringLength(500, ErrorMessage = "Role Description cannot be more than 500 characters.")]
        public string RoleDescription { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [DisplayName("Modified By")]
        public string ModifiedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string RoleType { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ViewName { get; set; }

        public string RoleInDepartment { get; set; }
    }
}
