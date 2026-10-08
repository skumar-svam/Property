using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.OnlineScheme
{
    public class OSSchemeAreaViewModel
    {
        public int Id { get; set; }
        public Nullable<int> AreaId { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> PropertyTypeRefId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<int> ActionTypeId { get; set; }
        public Nullable<int> ReturnTypeId { get; set; }
        public Nullable<int> FilterTypeId { get; set; }

        public string AreaRange { get; set; }
        public string AreaForUse { get; set; }
        public string Category { get; set; }
        public string PropertyType { get; set; }
        public string Department { get; set; }
        public string SchemeName { get; set; }
        public string ActionType { get; set; }
        public string ReturnType { get; set; }
        public string FilterType { get; set; }

        public Nullable<decimal> ApproxArea { get; set; }
        public Nullable<decimal> SectorRate { get; set; }
        public Nullable<decimal> ReservedPrice { get; set; }
        public Nullable<decimal> ProcessingFee { get; set; }
        public Nullable<decimal> EarnestMoney { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
       
    }
}
