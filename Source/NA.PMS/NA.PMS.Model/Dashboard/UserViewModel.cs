using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class UserViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> ActionTypeId { get; set; }
        public Nullable<int> FilterTypeId { get; set; }

        public Nullable<Guid> UserId { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string Pasword { get; set; }
        public string Applicant { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public string PropertyId { get; set; }
        public string Department { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo { get; set; }        
        public string AuthorityLetterType { get; set; }
        public string CustomerIdFileType { get; set; }
        public string CustomerIdFileName { get; set; }
        public string AuthorityLetter { get; set; }
        public string Remarks { get; set; }
        public string Comment { get; set; }
        public string ErrorMessage { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public string ActionType { get; set; }
        public string ApplicantType { get; set; }
        public string FatherName { get; set; }
        public string SigningAuthority { get; set; }
        public string PermanentAddress { get; set; }
        public string CorrespondAddress { get; set; }
        public string AadharNumber { get; set; }
        public string GSTNumber { get; set; }
        public string PANNumber { get; set; }
        public string AnnualIncome { get; set; }
        public string CaptchaInput { get; set; }
        public string RandomKeyVal { get; set; }
        public string FilterType { get; set; }

        public Nullable<bool> Status { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsLocked { get; set; }
        public Nullable<bool> IsRejected { get; set; }
        public Nullable<bool> IsRemembered { get; set; }
        public Nullable<bool> IsFirstTimeActivated { get; set; }
        public Nullable<bool> IsIdFileUploaded { get; set; }
        public Nullable<bool> IsPropertyFileUploaded { get; set; }

        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> ModifiedOn { get; set; }
        public Nullable<DateTime> LastPasswordChangeDate { get; set; }
    }
}
