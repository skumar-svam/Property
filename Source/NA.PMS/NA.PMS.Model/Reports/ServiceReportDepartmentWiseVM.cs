using System;
using System.Collections.Generic;

namespace NA.PMS.Model
{
    public class ServiceReportDepartmentWiseVM
    {
        
        public int? DepartmentId { get; set; }
        public int? Cancelled { get; set; }
        public int? Completed { get; set; }
        public int? Initiated { get; set; }
        public int? Pending { get; set; }
        public int? InProgress { get; set; }
        public int? TotalRequest { get; set; }

        public decimal? InProgessPercentage { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string DepartmentName { get; set; }
        public string ServiceName { get; set; }
        public string RequestThrough { get; set; }

        public List<ServiceReportDepartmentWiseVM> ServiceList;
    }
}
