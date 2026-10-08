using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NA.PMS.Model.Property
{
    public class MergeSplitPropModel
    {
        public int Id { get; set; }
        public int? Rid { get; set; }
        public int? PropertyId { get; set; }
        public string RequestNumber { get; set; }
        public string DepartmentName { get; set; }
        public string PropertyNumber { get; set; }

        public decimal Charges { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string AssignedTo { get; set; }
        public string Status { get; set; }
        public int Type { get; set; }
        public bool IsActive { get; set; }
        public int DepartmentId { get; set; }
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public int BlockId { get; set; }
        public string BlockName { get; set; }
        public List<MergeSplitPropertyGrid> ListAmalgamateProp { get; set; }
        public bool IsSelected { get; set; }

    }

    public class AddMergeSplitRequestModel
    {
        public int? Rid { get; set; }
        public int? PropertyId { get; set; }
        [DisplayName("Applicant Name")]
        public string ApplicantName { get; set; }
        public int? SchemeId { get; set; }
        [DisplayName("Scheme Name")]
        public string SchemeName { get; set; }
        //[Required(ErrorMessage = "Department is required")]
        public int DepartmentId { get; set; }
        [DisplayName("Department")]
        public string DepartmentName { get; set; }
        //[Required(ErrorMessage = "Property Type is required")]
        public int PropertyTypeId { get; set; }
        [DisplayName("Property Type")]
        public string PropertyType { get; set; }
        //[Required(ErrorMessage = "Sector is required")]
        public int SectorId { get; set; }
        [DisplayName("Sector")]
        public string SectorName { get; set; }
        //[Required(ErrorMessage = "Block is required")]
        public int BlockId { get; set; }
        [DisplayName("Block")]
        public string BlockName { get; set; }
        [DisplayName("Property Number")]
        [RegularExpression(@"^([a-zA-Z0-9 \-\\\/]+)$", ErrorMessage = "Property Number should be alphanumeric (with '-', '\' and '/' characters)")]
        [Required(ErrorMessage = "Property Number is required")]
        public string PropertyNumber { get; set; }
        [DisplayName("Total Area")]
        [Required(ErrorMessage = "Total Area is required")]
        public Decimal? TotalArea { get; set; }
        [DisplayName("Actual Area")]
        [Required(ErrorMessage = "Actual Area is required")]
        [Range(1.0, 99999999999.99, ErrorMessage = "Actual Area must be decimal(13,2) and greater than 0")]
        public Decimal? ActualArea { get; set; }
        [DisplayName("Covered Area")]
        [Range(1.0, 99999999999.99, ErrorMessage = "Covered Area must be decimal(13,2) and greater than 0")]
        public Decimal? CoveredArea { get; set; }
        [DisplayName("Location Type")]
        public string LocationType { get; set; }
        public IEnumerable<SelectListItem> SchemeCollection { get; set; }
        public IEnumerable<SelectListItem> DepartmentCollection { get; set; }
        public IEnumerable<SelectListItem> PropertyTypeCollection { get; set; }
        public IEnumerable<SelectListItem> SectorCollection { get; set; }
        public IEnumerable<SelectListItem> BlockCollection { get; set; }
        public IEnumerable<SelectListItem> LocationTypeCollection { get; set; }
        public int refId { get; set; }
        [DisplayName("Location Charges")]
        public bool LocationCharges { get; set; }
        [DisplayName("Charges")]
        [Required(ErrorMessage = "Charges is required")]
        [RegularExpression(@"[0-9]+(\.[0-9][0-9]?)?$", ErrorMessage = "Charges must be decimal.")]
        [Range(1.0, 999999999999, ErrorMessage = "Charges must be decimal(15,2) and greater than 0")]
        public decimal? Charges { get; set; }
        [DisplayName("Property Cost")]
        public Decimal? PropertyCost { get; set; }
        [DisplayName("Total Property Cost")]
        public Decimal? TotalPropertyCost { get; set; }
        public string User { get; set; }

        //public decimal Charges { get; set; }
        public int RequestId { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string AssignedTo { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public bool IsActive { get; set; }
        public string ApproveComments { get; set; }
        public string RejectComments { get; set; }
        public string Comments { get; set; }
        public List<MergeSplitPropertyGrid> ListMergeSplitPropertyGrid { get; set; }
    }

    public class MergeSplitPropertyGrid
    {
        public int Rid { get; set; }
        public int PropertyId { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string PropertyNumber { get; set; }
        public Decimal? CoveredArea { get; set; }
        public Decimal? ActualArea { get; set; }
        public Decimal? TotalArea { get; set; }
        public Decimal? TotalPropertyCost { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Modifiedby { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsSelected { get; set; }
    }
}
