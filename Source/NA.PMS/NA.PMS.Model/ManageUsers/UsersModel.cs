using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Web.Mvc;


namespace NA.PMS.Model
{
    public class UsersModel
    {
        public int id { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9. ]*$", ErrorMessage = "First name must be alphanumeric.")]
        [MaxLength(100, ErrorMessage = "First Name cannot be more than 100 characters.")]
        [DisplayName("First Name")]
        public string firstName { get; set; }
        [DisplayName("Middle Name")]
        [RegularExpression(@"^[a-zA-Z0-9. ]*$", ErrorMessage = "Middle name must be alphanumeric.")]
        [MaxLength(100, ErrorMessage = "Middle Name cannot be more than 100 characters.")]
        public string middleName { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "Last Name cannot be more than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9. ]*$", ErrorMessage = "Last name must be alphanumeric.")]
        [DisplayName("Last Name")]
        public string lastName { get; set; }
        public int roleId { get; set; } //Used in Role-User Mapping
        public string roleName { get; set; } //Used in Role-User Mapping 
        [RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "Mobile Number must be numeric.")]
        [Required]
        //[StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile Number should be of 10 digits.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Mobile Number should be of 10 digits.")]
        [DisplayName("Mobile Number")]
        [Remote("CheckMobileNoDuplicacy", "ManageUsers", AdditionalFields = "mobile, id", HttpMethod = "POST", ErrorMessage = "Mobile Number already exists.")]
        public string mobile { get; set; }
        [Required]
        [MaxLength(150, ErrorMessage = "Email cannot be more than 150 characters.")]
        [DisplayName("Email")]
        //[RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail address")]
        [Remote("CheckEmailDuplicacy", "ManageUsers", AdditionalFields = "email, id", HttpMethod = "POST", ErrorMessage = "Email Id already exists.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@(([a-zA-Z\-]+\.)+)([a-zA-Z]{2,4})$", ErrorMessage = "Please enter a valid e-mail address")]
        public string email { get; set; }
        public string fullName { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Username must be alphanumeric.")]
        [MaxLength(15, ErrorMessage = "Username cannot be more than 15 characters.")]
        [DisplayName("Username")]
        [Remote("CheckUsernameDuplicacy", "ManageUsers", AdditionalFields = "empID, id", HttpMethod = "POST", ErrorMessage = "Username already exists.")]
        public string empID { get; set; }
        public bool isActive { get; set; }
        public List<CheckBoxListItem> depttIDs { get; set; }
        public string strDeptts { get; set; }
        public string createdBy { get;set;}
        public DateTime? lastModified { get; set; }
        public List<string> lstDeptt { get; set; }
        public int applicationID { get; set; }
        public string applicationName { get; set; }
        public int subdepartmentId { get; set; }
        public string subDepartment { get; set; }
        public List<CheckBoxViewModel> SubDepartmentList { get; set; }
    }

    public class AdminRoleDetail
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleType { get; set; }

        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string createdBy { get; set; }
    }


}
