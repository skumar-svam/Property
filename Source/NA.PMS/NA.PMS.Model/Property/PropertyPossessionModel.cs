using NA.PMS.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model.Property
{
    public class PropertyPossessionModel
    {
        public int Id { get; set; }
        public string EncryptedId
        {
            get
            {
                return CommonHelper.Encode(Id.ToString());
            }
        }
        public int? Rid { get; set; }
        public string EncryptedRid
        {
            get
            {
                return CommonHelper.Encode(Rid.ToString());
            }
        }
        public int? PropertyId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? PossessionOrderDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? PossessionDueDate { get; set; }
        [Required(ErrorMessage="Possession Date is required")]
        [DataType(DataType.Date)]
        public DateTime? PossessionDate { get; set; }
        public string AreaChange { get; set; }
        public decimal? OneTimeExcessCharge { get; set; }
        public decimal? ChnagedArea { get; set; }
        public bool? Possession { get; set; }
        public decimal? Amount { get; set; }
        public decimal? PenaltyAmount { get; set; }
        [Required(AllowEmptyStrings=false,ErrorMessage="North field is required")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9- ]*$", ErrorMessage = "North must be alphanumeric.")]
        public string North { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "East field is required")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9- ]*$", ErrorMessage = "East must be alphanumeric.")]
        public string East { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "West field is required")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9- ]*$", ErrorMessage = "West must be alphanumeric.")]
        public string West { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "South field is required")]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9- ]*$", ErrorMessage = "South must be alphanumeric.")]
        public string South { get; set; }

        public int? StatusId { get; set; }
        public string PossessionBy { get; set; }
        public int? Approver { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string Comment { get; set; }
        public DateTime? CommentDate { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Modifiedby { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string SchemeName { get; set; }
        public int SchemeId { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ApplicantName {
            get {
                return FirstName + " " + MiddleName + " " + LastName;
            }
        }
        public string Status { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string SigningAuthority { get; set; }
        public string RegisteredOffice { get; set; }
        public string CorrespondAdd { get; set; }
        public string PropertyNumber { get; set; }
        public string PropertyType { get; set; }
        public decimal Area { get; set; }
        public string FloorAreaRange { get; set; }
        public string Gender { get; set; }
        [DataType(DataType.Date)]
        public DateTime LeaseDeedDueDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime LeaseDeedExecutionDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy MM dd}")]
        public DateTime? AllottmentDate { get; set; }
        public bool Print { get; set; }
        
        public bool IsSelected { get; set; }
        public string AreaChangeType { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public string PropertyNo { get; set; }
        public int CurrentYear
        {
            get { return DateTime.Now.Year; }
            
        }
        public string CurrentDate
        {
            get { return DateTime.Now.Date.ToShortDateString(); }

        }

        public virtual AllotmentMaster AllotmentMaster { get; set; }
        public virtual StatusMaster StatusMaster { get; set; }
        public bool IsPossessionOrderDone { get; set; }
        public bool IsPossessionEntryDone { get; set; }
        public string JavascriptToRun { get; set; }
        [DisplayName("Request Ref. No.")]
        public int? ReqRefNo { get; set; }
    }
}
