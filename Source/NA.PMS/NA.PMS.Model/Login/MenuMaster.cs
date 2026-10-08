using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class MenuMaster
    {
        public int MenuId { get; set; }
        public Nullable<int> MenuPathId { get; set; }
        public string MenuName { get; set; }
        public Nullable<int> MenuParentId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public ApplicationMaster ApplicationMaster { get; set; }

        public Nullable<bool> IsRead { get; set; }
        public Nullable<bool> IsWrite { get; set; }
        public Nullable<bool> IsUpdate { get; set; }
        public Nullable<bool> Isdelete { get; set; }
    }
}
