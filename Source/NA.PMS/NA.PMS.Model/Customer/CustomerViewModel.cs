using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class CustomerDetailViewModel
    {
        public CustomerViewModel CustomerModel { get; set; }
        public PropertyViewModel PropertyModel { get; set; }
    }

    public class CustomerViewModel
    {
        public System.Guid CustomerId { get; set; }

        public Nullable<int> Id { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> ParentPropertyId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> PropertyTypeId { get; set; }
        public Nullable<int> AreaRangeId { get; set; }
        public Nullable<int> FloorAreaId { get; set; }

        public string Department { get; set; }
        public string UserName { get; set; }
        public string FlagString { get; set; }
        public string RegistrationNo { get; set; }
        public string PropertyNo { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string AreaRange { get; set; }
        public string FloorArea { get; set; }
        public string PropertyType { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Applicant { get; set; }
        public string ApplicantType { get; set; }
        public string ApplicantMaster { get; set; }
        public string MotherName { get; set; }
        public string ApplicantAddress { get; set; }
        public string CorrespondAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string AadharNo { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        public string GSTNo { get; set; }
        public string PanNo { get; set; }
        public string Email { get; set; }
        public string MaritalStatus { get; set; }
        public string Religion { get; set; }
        public string Category { get; set; }
        public string Occupation { get; set; }
        public string Registry { get; set; }
        public string TransferType { get; set; }
        public string TransferFlag { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsLocked { get; set; }
        public Nullable<bool> IsRejected { get; set; }
        public Nullable<bool> IsExist { get; set; }
        public Nullable<bool> IsFirstTimeActivated { get; set; }

        public Nullable<decimal> PropertyCost { get; set; }
        public Nullable<decimal> CivilCost { get; set; }
        public Nullable<decimal> TotalPropertyCost { get; set; }
        public Nullable<decimal> PropertyRate { get; set; }
        public Nullable<decimal> TotalArea { get; set; }
        public Nullable<decimal> CoveredArea { get; set; }
        public Nullable<decimal> ActualArea { get; set; }
        public Nullable<decimal> ProcessingFee { get; set; }
        public Nullable<decimal> AllotmentMoney { get; set; }
        public Nullable<decimal> TransferCharge { get; set; }
        public Nullable<decimal> TransferCost { get; set; }
        public Nullable<decimal> AnnualIncome { get; set; }

        public Nullable<DateTime> DateOfBirth { get; set; }
        public Nullable<DateTime> AllotmentDate { get; set; }
        public Nullable<DateTime> ChecklistDate { get; set; }
        public Nullable<DateTime> InstallmentStartDate { get; set; }
        public Nullable<DateTime> PossessionDate { get; set; }
        public Nullable<DateTime> PossessionOrderDate { get; set; }
        public Nullable<DateTime> FunctionalDate { get; set; }
        public Nullable<DateTime> RentingDate { get; set; }
        public Nullable<DateTime> RegistryDueDate { get; set; }
        public Nullable<DateTime> RegistryDate { get; set; }
        public Nullable<DateTime> TransferDate { get; set; }
        public Nullable<DateTime> MortgageDate { get; set; }
    }

    public class EmployeeViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> EmployeeCode { get; set; }
        public Nullable<int> CreatedBy { get; set; }

        public string EmployeeName { get; set; }

        public Nullable<DateTime> CreatedDate { get; set; }

        public Nullable<bool> IsActive { get; set; }
    }

    public class LitigationViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> RegistrationId { get; set; } 
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> SerialNo { get; set; }
        public Nullable<int> Year { get; set; }

        public string Department { get; set; }
        public string RegistrationNo { get; set; }
        public string ActionType { get; set; } 
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Applicant { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string CaseNo { get; set; }
        public string CaseFiledStatus { get; set; }
        public string Court { get; set; }
        public string Party { get; set; }
        public string Advocate { get; set; }
        public string AdvocateDetail { get; set; }
        public string StayOrder { get; set; }
        public string LegalFiled { get; set; }
        public string NarrativeEyes { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
        public string Remarks { get; set; }
        public string Description { get; set; }

        public Nullable<DateTime> CaseFiledDate { get; set; }
        public Nullable<DateTime> LegalDate { get; set; }
    }

    public class JalViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> SerialNo { get; set; }
        public Nullable<int> DEV_TYPE { get; set; }

        public string Department { get; set; }
        public string RegistrationNo { get; set; }
        public string ActionType { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Applicant { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string ReceiptNo { get; set; }
        public string DIS_CD { get; set; } 
        public string RemovedConnection { get; set; } //RMC
        public string T_FEE { get; set; }
        //public string CessCharges { get; set; } //cess chargesCSS
        public string BANK_CD { get; set; }
        public string BR_NM { get; set; }
        public string RIV_BIL_FR { get; set; }
        public string MATCH { get; set; }
        public string KEY_NO { get; set; }
        public string ERR { get; set; }
        public string UPD { get; set; }
        public string DUP { get; set; }
        public string MST { get; set; }
        public string REV_CON_DT { get; set; }
        public string A { get; set; }
        public string REB { get; set; }
        public string MAN_LED { get; set; }

        public Nullable<decimal> BillAmount { get; set; }
        public Nullable<decimal> Surcharge { get; set; }
        public Nullable<decimal> Cesscharge { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<decimal> Arrear { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public Nullable<decimal> NOC { get; set; }
        public Nullable<decimal> SECU { get; set; }
        public Nullable<decimal> LastBillPaid { get; set; } //BIL_PAD_OLD

        public Nullable<DateTime> BillStartDate { get; set; }
        public Nullable<DateTime> BillEndDate { get; set; }
        public Nullable<DateTime> PaymentDate { get; set; }
        public Nullable<DateTime> BillDueDate { get; set; }
        public Nullable<DateTime> ERL_DT { get; set; }
        public Nullable<DateTime> LastBillStartDate { get; set; } //BIL_FR_OLD
        public Nullable<DateTime> LastBillEndDate { get; set; } //BIL_TO_OLD
    }
}
