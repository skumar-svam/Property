using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.OnlineScheme
{
    public class OSLoginViewModel
    {
        public string TxtSchemeFormNo { get; set; }
        public string TxtPassword { get; set; }
        public string TxtSchemeType { get; set; }
        public string TxtRequestType { get; set; }

        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }

        public string PasswordMessage { get; set; }
        public string PasswordSuccessMessage { get; set; }

        public Nullable<int> ApplicationFormId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> HiddenVal { get; set; }

        public Nullable<bool> IsPasswordChanged { get; set; }
    }

    public class OSDropdownViewModel
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
        public Nullable<int> ApplicationFormId { get; set; }
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
        public Nullable<int> BankId { get; set; }
        public Nullable<int> BranchId { get; set; }

        public Nullable<long> ReceiptId { get; set; }
        public Nullable<long> RefId { get; set; }
        public Nullable<DateTime> FilterDate { get; set; }
    }

    public class SchemeFormChallanApiModel
    {
        //mandatory fields
        public string Allottee { get; set; }
        public string BankCode { get; set; }
        public Nullable<int> Department_Id { get; set; }
        public string Address { get; set; }
        public string Mobile_No { get; set; }
        public string FormNumber { get; set; }
        public Nullable<int> ChallanTypeId { get; set; }
        public Nullable<int> PaymentModeId { get; set; }//1-rtgs,2-dd
        public Nullable<decimal> TotalAmount { get; set; }
        //belows are optional 
        public string Sector_Name { get; set; }
        public string BlockName { get; set; }
        public string Plot_No { get; set; }
        public string Email { get; set; }
        public string Rid { get; set; }
        public string PAN { get; set; }
        public string GST_No { get; set; }

        public List<SchemeFormChargesApiModel> ChallanChargesVM { get; set; }
    }

    public class SchemeFormChargesApiModel
    {
        public string Rid { get; set; }//optional
        public Nullable<int> HeadId { get; set; }//mandatory
        public Nullable<int> SubheadId { get; set; }//mandatory
        public Nullable<decimal> Amount { get; set; }//mandatory
    }
}
