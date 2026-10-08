
using System.Collections.Generic;

namespace NA.PMS.Model
{
    public class NMServicesVM
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string ServiceCode { get; set; }
        public int? Status { get; set; }
        public System.DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public System.DateTime? ModifiedDate { get; set; }

        public int? CitizenServiceId { get; set; }

        public int? DeptId { get; set; }
        public int? ServiceId { get; set; }
        public string CitizenServiceName { get; set; }
        public int? Timeline { get; set; }
        public decimal? Amount { get; set; }
        public int? CitizenServiceStatus { get; set; }

        public string DepartmentName { get; set; }
        public List<int> CitizenServiceIds { get; set; }
    }
}
