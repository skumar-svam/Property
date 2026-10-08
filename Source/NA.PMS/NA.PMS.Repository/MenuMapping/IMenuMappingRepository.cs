using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NA.PMS.Model;

namespace NA.PMS.Repository
{
    public interface IMenuMappingRepository
    {
        IEnumerable<Roles> GetRoles(string roleType);
        IEnumerable<Roles> GetRolesByApplicationId(int appID, string roleType);
        IEnumerable<MenuMappingDetail> GetMenusByRoleId();
        IEnumerable<MenuMappingDetail> GetSelectedMenus();
        bool SaveMenuMapping(IEnumerable<MenuMappingDetail> lstMenuMapping, int roleId, int userId);
        List<GetMenuID> GetMenuIdsByRoleID(List<int> roleIds);
    }
}
