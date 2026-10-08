using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NA.PMS.Model
{
    public class PropertyModel
    {
        public bool? IsSelected { get; set; }
        [Required(ErrorMessage = "Scheme Name is required")]
        public int? schemeId { get; set; }
        public string schemeName { get; set; }
        [Required(ErrorMessage = "Department is required")]
        public int departmentId { get; set; }
        public string departmentName { get; set; }
        [Required(ErrorMessage = "Property Type is required")]
        public int propertyTypeId { get; set; }
        public string propertyType { get; set; }
        [Required(ErrorMessage = "Sector is required")]
        public int sectorId { get; set; }
        public string sectorName { get; set; }
        [Required(ErrorMessage = "Block is required")]
        public int blockId { get; set; }
        public string blockName { get; set; }
        [RegularExpression(@"^([a-zA-Z0-9 \-\\\/]+)$", ErrorMessage = "Property No should be alphanumeric (with '-', '\' and '/' characters)")]
        [Required(ErrorMessage = "Property No is required")]
        public string plotProperty { get; set; }
        public string createdBy { get; set; }
        public DateTime? lastModifiedDate { get; set; }
        [Required(ErrorMessage = "Total Area is required")]
        //[Range(1.0, 9999999999999.99, ErrorMessage = "Total Area must be decimal(15,2) and greater than 0")]
        public Decimal? totalArea { get; set; }
        [Required(ErrorMessage = "Actual Area is required")]
        //[RegularExpression(@"[0-9]+(\.[0-9][0-9]?)?$", ErrorMessage = "Actual Area must be decimal.")]
        [Range(1.0, 99999999999.99, ErrorMessage = "Actual Area must be of 11 digits with two decimal")]
        public Decimal? actualArea { get; set; }
        //[RegularExpression(@"[0-9]+(\.[0-9][0-9]?)?$", ErrorMessage = "Covered Area must be decimal.")]
        [Range(1.0, 99999999999.99, ErrorMessage = "Covered Area must be of 11 digits with two decimal")]
        public Decimal? coveredArea { get; set; }
        public Decimal? civilCost { get; set; }
        public Double? floorAreaRatio { get; set; }
        public string locationType { get; set; }
        [Required(ErrorMessage = "Floor/Area Range is required")]
        public int floorId { get; set; }
        public string floorName { get; set; }
        [Required(ErrorMessage = "Location is required")]
        public int? locationId { get; set; }
        public string locationName { get; set; }
        public IEnumerable<SelectListItem> schemeCollection { get; set; }
        public IEnumerable<SelectListItem> departmentCollection { get; set; }
        public IEnumerable<SelectListItem> propertyTypeCollection { get; set; }
        public IEnumerable<SelectListItem> sectorCollection { get; set; }
        public IEnumerable<SelectListItem> blockCollection { get; set; }
        public IEnumerable<SelectListItem> locationTypeCollection { get; set; }
        public IEnumerable<SelectListItem> floorCollection { get; set; }
        public int refId { get; set; }
        public bool IsLocationCharged { get; set; }
        public bool locationCharges { get; set; }
        [DisplayName("Charges")]
        [Required(ErrorMessage = "Charges is required")]
        [RegularExpression(@"[0-9]+(\.[0-9][0-9]?)?$", ErrorMessage = "Charges must be decimal.")]
        [Range(1.0, 9999999999999.99, ErrorMessage = "Charges must be of 12 digits with two decimal")]
        public Decimal? charges { get; set; }
        public Decimal? allotementMoney { get; set; }
        public Decimal? leaseRent { get; set; }
        public Double? allotementMoneyPercentage { get; set; }
        public Double? leaseRentPercentage { get; set; }
        [Required(ErrorMessage = "Registry is required")]
        public string RegistryName { get; set; }
        public Decimal? LandRate { get; set; }
        public Decimal? PropertyCost { get; set; }
        public Decimal? TotalPropertyCost { get; set; }
        public Decimal? LandRatePerSqMet { get; set; }
        public Decimal? LocationChargeRate { get; set; }
        public int IsAllotted { get; set; }
        public int? isActiveAllotment { get; set; }
        public string isStatusAllotment { get; set; }
        public int? SchemeStatus { get; set; }
       
        public string EncryptedRefId 
        {
            get { return CommonHelper.Encode(refId.ToString()); }
        }        
        //public int isParentProp { get; set; }
        //public int? ParentPropRId { get; set; }

        //Added on 16Nov2017
        public Nullable<int> ProjectId { get; set; }
        public string Project { get; set; }
        public int? PropertyNo { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> ParentPropertyId { get; set; }
        public Nullable<decimal> EarnestMoney { get; set; }
        public Nullable<Decimal> Latitude { get; set; }
        public Nullable<Decimal> Longitude { get; set; }
        public Nullable<int> RegistrationId { get; set; }
    }

    public class SubleaseModel
    {
        public int Id { get; set; }
        public int? DeptId { get; set; }
        public int RID { get; set; }
        public string AllotteeName { get; set; }
        public string SubLeaseAddNo { get; set; }
        public decimal PropArea { get; set; }
        public decimal AreaRate { get; set; }
        public bool IsRIdValid { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string Plot { get; set; }
        public decimal PropCost { get; set; }
        public string DepaertmentName { get; set; }
        public int? AreaRange { get; set; }
        public int? Scheme { get; set; }
        public int? SectorId { get; set; }
        public int? BlockId { get; set; }
        public int? PropType { get; set; }
        public int PropertyId { get; set; }
        public int ParentPropertyId { get; set; }
        public decimal? DocCharges { get; set; }
    }

    public class SubLeaseViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> ParentRegistrationId { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> ParentPropertyId { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> AreaRangeId { get; set; }
        public Nullable<int> PropertyTypeId { get; set; }
        public string Department { get; set; }      
        public string Applicant { get; set; }
        public string ApplicantMaster { get; set; }
        public string ApplicantAddress { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string SubLeasePlot { get; set; }
        public string SubLeaseStatus { get; set; }       
        public string AreaRange { get; set; }
        public string PropertyType { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public Nullable<decimal> TotalCost { get; set; }
        public Nullable<decimal> PropertyCost { get; set; }
        public Nullable<decimal> PropertyArea { get; set; }
        public Nullable<decimal> PropertyRate { get; set; }       
        public Nullable<decimal> Charges { get; set; }
        public Nullable<bool> IsPropertyActive { get; set; }
        public Nullable<bool> IsRIDValid { get; set; }

        //Added on 16Nov2017
        public Nullable<int> ProjectId { get; set; }
        public string Project { get; set; }


        //For Search Parent RID
        public int SearchSectorId { get; set; }
        public int SearchBlockId { get; set; }
        public string SearchPlotNo { get; set; }
    }

    public class ProjectViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> PropertyId { get; set; }      
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string Department { get; set; }       
        public Nullable<bool> IsRIDValid { get; set; }
        public bool Status { get; set; }

        //Added on 16Nov2017
        public Nullable<int> ProjectId { get; set; }
        public string Project { get; set; }
    }

    public class ProjectModel
    {
        public Nullable<int> ParentRegistrationId { get; set; }
        public Nullable<int> ParentPropertyId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string Project { get; set; }
        public bool Status { get; set; }
    }
    
}
