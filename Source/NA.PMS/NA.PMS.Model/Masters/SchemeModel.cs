using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NA.PMS.Model
{
    public class Scheme
    {
        [DisplayName("Scheme ID")]
        public int scheId { get; set; }
        //[Remote("IsSchemeNameUnique", "Scheme", AdditionalFields = "schemeName, scheId", HttpMethod = "HttpPost", ErrorMessage = "Scheme name already exsits.")]
        [DisplayName("Scheme Name")]
        //[MaxLength(100, ErrorMessage = "Max 100 digits is required.")]
        //[RegularExpression(@"^[a-zA-Z0-9 ]*$", ErrorMessage = "Scheme name should be alphanumeric.")]
        public string schemeName { get; set; }
        [DisplayName("Scheme Type")]
        //[Required]
        public string schemeType { get; set; }
        [DisplayName("Start Date")]
        //[Required]
        public Nullable<System.DateTime> startDate { get; set; }
        [DisplayName("End Date")]
        //[Required]
        public Nullable<System.DateTime> endDate { get; set; }
        public Department Departments { get; set; }
        public Quota Quotas { get; set; }
        public Rebate Rebate { get; set; }
        public Banks Banks { get; set; }
        public CostModel costModel { get; set; }
        [DisplayName("Created By")]
        public string createdBy { get; set; }
        [DisplayName("Modified By")]
        public string modifiedBy { get; set; }
        [DisplayName("Created Date")]
        public Nullable<System.DateTime> createdDate { get; set; }
        [DisplayName("Modified Date")]
        public Nullable<System.DateTime> modifiedDate { get; set; }
        public Nullable<bool> completed { get; set; }
        public Nullable<int> IsAllotted { get; set; }
        public LandDevelopmentScheduleModel LandDevelopmentScheduleModel { get; set; }
        public bool isSchemeActive { get; set; }

        public bool isDepttExists { get; set; }
        public bool isCostExists { get; set; }
        public bool isBankExists { get; set; }
        public bool isLandExists { get; set; }

        public int? SchemeStatus { get; set; }
        public string strSchemeStatus { get; set; }
        private string encodedParam;
        public string EncodedParameter
        {
            get { return encodedParam = CommonHelper.Encode(scheId.ToString()); }           
            set { encodedParam = CommonHelper.Encode(scheId.ToString()); }
        }
        public decimal FormFee { get; set; }
    }

    public class Department
    {
        public int refId { get; set; }
        public Nullable<int> scheId { get; set; }
        //[Remote("IsDepatmnetNameUnique", "Scheme", AdditionalFields = "scheId, departmentId", HttpMethod = "HttpPost", ErrorMessage = "Department name already exsits.")]
        [DisplayName("Department")]
        [Required]
        public Nullable<int> departmentId { get; set; }
        public string departmentName { get; set; }
        [DisplayName("Normal Interest (%)")]
        [Required]
        public Nullable<double> normalInterest { get; set; }
        public string NIName { get; set; }
        [DisplayName("Penal Interest (%)")]
        [Required]
        public Nullable<double> penalInterest { get; set; }
        public string PIName { get; set; }
        [DisplayName("Frequency")]
        [Required]
        public Nullable<int> frequency { get; set; }
        public string frequencyName { get; set; }
        [DisplayName("Maximum No. of Installment")]
        //[Required (ErrorMessage ="Maximum No. of Installment is required field.")]
        [Range(1, float.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        [MaxLength(3, ErrorMessage = "Only 3 digits is required.")]
        public Nullable<int> noInstallment { get; set; }
        [DisplayName("Floor Area Ratio (%)")]
        [Required]
        [Range(1, float.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        [MaxLength(12, ErrorMessage = "Max 12 digits is required.")]
        public Nullable<double> floorArearatio { get; set; }
        [DisplayName("Allotment Money (%)")]
        [Required]
        [Range(1, float.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        [MaxLength(12, ErrorMessage = "Max 12 digits is required.")]
        public Nullable<double> allotmentmoney { get; set; }
        public string allotmentMoneyName { get; set; }
        [DisplayName("Installment Money (%)")]
        [Required]
        [Range(1, float.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        [MaxLength(12, ErrorMessage = "Max 12 digits is required.")]
        public Nullable<double> installmentmoney { get; set; }
        public string installmentMoneyName { get; set; }
        [DisplayName("Lease Rent (%)")]
        //[Required (ErrorMessage = "Lease Rent is required field.")]
        [Range(1, float.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        [MaxLength(12, ErrorMessage = "Max 12 digits is required.")]
        public Nullable<double> leaseRent { get; set; }
        public string leaseRentName { get; set; }
        public Boolean IsSelected { get; set; }
        [DisplayName("Selection Type")]
        [Required]
        public string SelectionType { get; set; }
    }

    public class CostModel
    {
        public Nullable<int> schemeId { get; set; }
        public Nullable<int> depttID { get; set; }
        public Nullable<int> propType { get; set; }
        [DisplayName("Sector")]
        //[Required]
        public Nullable<int> sector { get; set; }
        [DisplayName("Floor")]
        //[Required]
        public Nullable<int> floor { get; set; }
        public Nullable<int> blockId { get; set; }
        public int refId { get; set; }
        [Required]
        [MaxLength(12, ErrorMessage = "Max 12 digits is required.")]
        [DisplayName("Property Cost* (INR)")]
        public Nullable<decimal> propCost { get; set; }
        [Required]
        [MaxLength(12, ErrorMessage = "Max 12 digits is required.")]
        [DisplayName("Land Rate(/Sq.Mtr)")]
        public Nullable<decimal> landRate { get; set; }
        public decimal ProcessingFee { get; set; }
        public Nullable<decimal> propertyCostTotal { get; set; }
        [DisplayName("Total Property Cost* (INR)")]
        public Nullable<decimal> totPropCost { get; set; }
        [Required]
        [MaxLength(12, ErrorMessage = "Max 12 digits is required.")]
        [DisplayName("Civil Cost* (INR)")]
        public Nullable<decimal> civilCost { get; set; }
        [Required]
        [MaxLength(12, ErrorMessage = "Max 12 digits is required.")]
        [DisplayName("Allotment Money* (INR)")]
        public Nullable<decimal> allotmentMoney { get; set; }
        [Required]
        [MaxLength(12, ErrorMessage = "Max 12 digits is required.")]
        public Nullable<decimal> earnestMoney { get; set; }
        [Required]
        [MaxLength(12, ErrorMessage = "Max 12 digits is required.")]

        [DisplayName("Lease Rent* (Yearly)(INR)")]
        public Nullable<decimal> leaseRent { get; set; }
        public Nullable<bool> IsActive { get; set; }
        //public string createdBy { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        //public string modifiedBy { get; set; }
        public Nullable<System.DateTime> modifiedDate { get; set; }
        public string depttName { get; set; }
        public string propName { get; set; }
        public string sectorName { get; set; }
        public string floorName { get; set; }
        public string blockName { get; set; }
    }

    public class Quota
    {
        //[Remote("IsQuotaNameUnique", "Scheme", AdditionalFields = "schemeId, quotaDepartmentId", HttpMethod = "HttpPost", ErrorMessage = "Department name already exsits.")]
        [DisplayName("Department")]
        //[Required]
        public Nullable<int> quotaDepartmentId { get; set; }
        public string quotaDepartmentName { get; set; }
        public Nullable<int> schemeId { get; set; }
        [DisplayName("Quota Type")]
        //[Required]
        public Nullable<int> quotaId { get; set; }
        public string quotaName { get; set; }
        public int refId { get; set; }
        [DisplayName("Value")]
        //[Required]
        [MaxLength(12, ErrorMessage = "Max 12 digits is required.")]
        public Nullable<decimal> value { get; set; }
        [DisplayName("Units")]
        //[Required]
        public string unit { get; set; }
        public string unitName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string createdBy { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        public string modifiedBy { get; set; }
        public Nullable<System.DateTime> modifiedDate { get; set; }
    }

    public class Rebate
    {
        //[Remote("IsRebateNameUnique", "Scheme", AdditionalFields = "schemeId, rebatedepartment", HttpMethod = "HttpPost", ErrorMessage = "Department name already exsits.")]
        [DisplayName("Department")]
        [Required]
        public Nullable<int> rebatedepartment { get; set; }
        public string rebateDepartmentName { get; set; }
        [DisplayName("Rebate Type")]
        [Required]
        public Nullable<int> rebateId { get; set; }
        public string rebateName { get; set; }
        [DisplayName("Value")]
        //[Required]
        [MaxLength(12, ErrorMessage = "Max 12 digits is required.")]
        public Nullable<decimal> rebateValue { get; set; }
        [DisplayName("Units")]
        [Required]
        public string rebateUnitId { get; set; }
        public string rebateUnitname { get; set; }
        public int refId { get; set; }
    }

    public class Banks
    {
        [Required]
        [DisplayName("Bank Name*")]
        public int bankId { get; set; }
        [Required]
        [DisplayName("Bank Name*")]
        [RegularExpression("^([a-zA-Z0-9 .&']+)$", ErrorMessage = "Invalid Character.")]
        public string bankName { get; set; }
        [DisplayName("Branch Address*")]
        public int branchId { get; set; }
        [Required]
        [DisplayName("Branch Address*")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid Character.")]
        public string branchName { get; set; }
        public string ddlBranchName { get; set; }
        [DisplayName("Account Number*")]
        [Required]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Please enter only alphanumeric character.")]
        [StringLength(16, MinimumLength = 9, ErrorMessage = "Account number should be of 9 to 16 digits.")]
        public string accountNumber { get; set; }
        public string DdlAccountNumber { get; set; }
        public int refId { get; set; }
    }

    public class LandDevelopmentScheduleModel
    {
        public int ScheduleId { get; set; }
        [DisplayName("Department")]
        public int? LandDevelopmentDepartmentId { get; set; }
        public string LandDevelopmentDepartmentName { get; set; }
        public int? SchemeId { get; set; }
        public string SchemeName { get; set; }
        [DisplayName("Process Name")]
        public int? ProcessId { get; set; }
        public string ProcessName { get; set; }
        [DisplayName("Trigger Process")]
        public int? TriggerId { get; set; }
        public string TriggerName { get; set; }
        [DisplayName("Duration")]
        public int? Duration { get; set; }
        public string createdBy { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        public string modifiedBy { get; set; }
        public Nullable<System.DateTime> modifiedDate { get; set; }
        public int? Years { get; set; }
        public int? Months { get; set; }
        public int? Days { get; set; }
    }


    public class SchemeViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RefId { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> SchemeTypeId { get; set; }
        public Nullable<int> AllotmentId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> SchemeStatusId { get; set; }
        public Nullable<int> FrequencyId { get; set; }
        public Nullable<int> InstallmentNo { get; set; }
        public Nullable<int> TotalInstallment { get; set; }
        public Nullable<int> SelectionTypeId { get; set; }
        public Nullable<int> PropertyTypeId { get; set; }
        public Nullable<int> FloorAreaTypeId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> QuotaTypeId { get; set; }
        public Nullable<int> UnitId { get; set; }
        public Nullable<int> RebateTypeId { get; set; }
        public Nullable<int> BankId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> ScheduleId { get; set; }
        public Nullable<int> ProcessId { get; set; }
        public Nullable<int> TriggerId { get; set; }
        public Nullable<int> Years { get; set; }
        public Nullable<int> Months { get; set; }
        public Nullable<int> Days { get; set; }
        public Nullable<int> TimePeriod { get; set; }

        public string SchemeName { get; set; }
        public string SchemeType { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public string SchemeStatus { get; set; }
        public string Frequency { get; set; }
        public string SelectionType { get; set; }
        public string PropertyType { get; set; }
        public string FloorAreaType { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo { get; set; }
        public string QuotaType { get; set; }
        public string UnitName { get; set; }
        public string RebateType { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string AccountNo { get; set; }
        public string IFSCCode { get; set; }
        public string ScheduleName { get; set; }
        public string ProcessName { get; set; }
        public string TriggerName { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsCompleted { get; set; }
        public Nullable<bool> IsSchemeActive { get; set; }
        public Nullable<bool> IsDepartmentExist { get; set; }
        public Nullable<bool> IsPropertyCostExist { get; set; }
        public Nullable<bool> IsBankExist { get; set; }
        public Nullable<bool> IsScheduleExist { get; set; }
        public Nullable<bool> IsSelected { get; set; }

        public Nullable<decimal> ApplicationFormFee { get; set; }
        public Nullable<decimal> ProcessingFee { get; set; }
        public Nullable<decimal> NormalInterest { get; set; }
        public Nullable<decimal> PenalInterest { get; set; }
        public Nullable<decimal> FloorAreaRatio { get; set; }
        public Nullable<decimal> AllotmentMoney { get; set; }
        public Nullable<decimal> InstallmentMoney { get; set; }
        public Nullable<decimal> EarnestMoney { get; set; }
        public Nullable<decimal> LeaseRent { get; set; }
        public Nullable<decimal> CivilCost { get; set; }
        public Nullable<decimal> PropertyRate { get; set; }
        public Nullable<decimal> PropertyCost { get; set; }
        public Nullable<decimal> TotalPropertyCost { get; set; }
        public Nullable<decimal> QuotaValue { get; set; }
        public Nullable<decimal> RebateValue { get; set; }
        public Nullable<decimal> GST { get; set; }
        public Nullable<decimal> CGST { get; set; }
        public Nullable<decimal> SGST { get; set; }

        public Nullable<DateTime> SchemeStartDate { get; set; }
        public Nullable<DateTime> SchemeEndDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }

        public string EncodedParameter { get { return SchemeId != null ? CommonHelper.Encode(SchemeId.ToString()) : null; } }               
    }
}
