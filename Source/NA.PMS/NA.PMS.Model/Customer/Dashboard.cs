using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class Dashboard
    {
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> SubDepartmentId { get; set; }
        public string Property_No { get; set; }
        public string ServiceName { get; set; }
        public string DepartmentName { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantAddress { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}
