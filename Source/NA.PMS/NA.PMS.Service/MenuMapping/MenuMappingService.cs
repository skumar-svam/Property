using NA.PMS.Model;
using NA.PMS.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Service
{
    public class MenuMappingService : IMenuMappingService   
    {
        IMenuMappingRepository menuMappingRepository = null;

        /// <summary>
        /// Cunstructor
        /// </summary>
        public MenuMappingService()
        {
            menuMappingRepository = new MenuMappingRepository();
        }

        public IEnumerable<Roles> GetRoles(string roleType)
        {
            return menuMappingRepository.GetRoles(roleType);
        }

        public IEnumerable<Roles> GetRolesByApplicationId(int appID, string roleType)
        {
            return menuMappingRepository.GetRolesByApplicationId(appID, roleType);
        }

        public IEnumerable<MenuMappingDetail> GetMenusByRoleId()
        {
            return menuMappingRepository.GetMenusByRoleId();
        }

        public IEnumerable<MenuMappingDetail> GetSelectedMenus()
        {
            return menuMappingRepository.GetSelectedMenus();
        }

        public bool SaveMenuMapping(IEnumerable<MenuMappingDetail> lstMenuMapping, int roleId, int userId)
        {
            return menuMappingRepository.SaveMenuMapping(lstMenuMapping, roleId, userId);
        }

        public List<GetMenuID> GetMenuIdsByRoleID(List<int> roleIds)
        {
            return menuMappingRepository.GetMenuIdsByRoleID(roleIds);
        }


    }
}
