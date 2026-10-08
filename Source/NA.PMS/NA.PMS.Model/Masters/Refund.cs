using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using NA.PMS;

namespace NA.PMS.Model
{
    public class Refund
    {
        
        [DisplayName("Refund Id")]
        public int RefundId { get; set; }

        [Required(ErrorMessage="Scheme is required.")]
        [DisplayName("Scheme Id")]
        public Nullable<int> SchemeId { get; set; }

        [DisplayName("Scheme Name")]
        public string SchemeName { get; set; }

        public int? SchemeStatus { get; set; }

        [Required(ErrorMessage="Description is required.")]
        [DisplayName("Refund Description")]
        [RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "Description must be alphanumeric.")]
        [MaxLength(200, ErrorMessage = "Description cannot be more than 200 characters.")]
        public string RefundDescription { get; set; }

        [Required(ErrorMessage = "Lock Period is required")]
        [DisplayName("Refund Lock Period(In Days)")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Lock Period must be numeric.")]
        [Range(1,99999, ErrorMessage = "Lock Period cannot be more than 5 digits.")]
        //[MaxLength(5, ErrorMessage = "Lock Period cannot be more than 5 digits.")]
        public Nullable<int> RefundLockPeriod { get; set; }

        //[Required(ErrorMessage="Media type is required.")]
        [DisplayName("Media Type")]
        public Nullable<int> MediaTypeid { get; set; }

        [DisplayName("Media")]
        public string MediaTypeName { get; set; }

        [Required(ErrorMessage="Deduction is required.")]
        [DisplayName("Deduction")]
        //[RegularExpression(@"[0-9]+(\.[0-9][0-9]?)?$", ErrorMessage = "Deduction must be decimal.")]
        public Nullable<decimal> Deduction { get; set; }

        [Required(ErrorMessage="Unit is required.")]
        [DisplayName("Unit")]
        public string Unit { get; set; }

        [Required(ErrorMessage="Deduction Apply On is required.")]
        [DisplayName("Deduction Apply On")]
        public string DeductionApplyOn { get; set; }

        [Required(ErrorMessage="Interest rate is required.")]
        [DisplayName("Interest Rate")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Interest rate must be numeric.")]
        [Range(0,99, ErrorMessage = "Interest Rate cannot be more than 3 digits.")]
        //[MaxLength(3, ErrorMessage = "Interest Rate cannot be more than 3 digits.")]
        public Nullable<int> InterestRate { get; set; }

        [Required]
        [DisplayName("Days After Interest")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Days After Interest must be number.")]
        [Range(0,99999, ErrorMessage = "Days After Interest cannot be more than 5 digits.")]
        public Nullable<int> DaysAfterInterest { get; set; }

        public Nullable<bool> IsActive { get; set; }

        [DisplayName("Created By")]
        public string CreatedBy { get; set; }

        [DisplayName("Created On")]
        public Nullable<System.DateTime> CreatedDate { get; set; }

        [DisplayName("Modified By")]
        public string ModifiedBy { get; set; }

        [DisplayName("Modified On")]
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [DisplayName("Department Id")]
        public Nullable<int> DepartmentId { get; set; }

        [DisplayName("Department")]
        public string DepartmentName { get; set; }


        private string encodedParam;
        public string EncodedParameter
        {
            get { return encodedParam = CommonHelper.Encode(RefundId.ToString()); }
            //set { encodedParam = CommonHelper.Encode(Convert.ToString(RefundId)); }
            set { encodedParam = CommonHelper.Encode(RefundId.ToString()); }
        }

        public IEnumerable<SelectListItem> SchemeCollection { get; set; }
        public IEnumerable<SelectListItem> MediaCollection { get; set; }
        public IEnumerable<SelectListItem> DepartmentCollection { get; set; }
        public IEnumerable<SelectListItem> UnitType { get; set; }
        public IEnumerable<SelectListItem> DeductionAppliedOn { get; set; }

    }

    public class SchemeRefund
    {
        public int SchemeId { get; set; }
        public Nullable<int> SchemeTypeId { get; set; }
        public string SchemeName { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> Completed { get; set; }
    }

    public class DepartmentRefund
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }

    public class DeductionApplyOnForRefund
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string TypeValue { get; set; }
    }

    public class UnitTypeForRefund
    {
        public int UnitId { get; set; }
        public string UnitValue { get; set; }
        public string UnitName { get; set; }
    }
}
