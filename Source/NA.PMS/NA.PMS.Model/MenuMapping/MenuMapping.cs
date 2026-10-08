using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class MenuMapping
    {
        public int? menuPathId { get; set; }
        public string menuName { get; set; }
        public int menuParentId { get; set; }
        public int applicationId { get; set; }
        public bool isActive { get; set; }
        public string createdBy { get; set; }
        public DateTime createdDate { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }
        public string htmlString { get; set; }
        public int roleId { get; set; }
    }

    public class Roles
    {
        public int roleId { get; set; }
        public string roleName { get; set; }
        public string roleDescription { get; set; }
        public string roleType { get; set; }
    }

    public class MenuMappingDetail : MenuMapping
    {
        public string applicationName { get; set; }
        public int menuId { get; set; }
        public Nullable<int> menuPathId { get; set; }
        public Nullable<int> menuParentId { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsWrite { get; set; }
        public bool? IsModify { get; set; }
        public bool? IsDelete { get; set; }
        public int roleId { get; set; }
    }

    public class GetMenuID
    {
        public int menuID { get; set; }
        public int parentID { get; set; }
    }

   
}
