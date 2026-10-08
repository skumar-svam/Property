using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using NA.PMS.Repository;
using NA.PMS.Model;

namespace NA.PMS.Service
{
    public class ManageRolesService : IManageRolesService
    {
        private IManageRolesRepository _manageRolesRepository;
        public ManageRolesService(IManageRolesRepository manageRolesRepository)
        {
            _manageRolesRepository = manageRolesRepository;
        }
        public List<RolesModel> GetAllRoles(string roletype)
        {
            return _manageRolesRepository.GetAllRoles(roletype);
        }

        public List<ApplicationModel> GetAllApplicationList(int userID)
        {
            return _manageRolesRepository.GetAllApplicationList(userID);
        }

        public bool AddRole(RolesModel roleModel, string roletype, int currentUser)
        {
            return _manageRolesRepository.AddRole(roleModel, roletype, currentUser); ;
        }

        public RolesModel GetRecordById(int id)
        {
            return _manageRolesRepository.GetRecordById(id);
        }

        public RolesModel GetRecordByApplicationId(int id)
        {
            return _manageRolesRepository.GetRecordByApplicationId(id);
        }

        public bool UpdateRecordById(RolesModel rmodel, string roletype, int currentUser)
        {
            return _manageRolesRepository.UpdateRecordById(rmodel,roletype,currentUser);
        }

        public bool DeleteRecordById(int roleID)
        {
            return _manageRolesRepository.DeleteRecordById(roleID);
        }

        public bool IsRoleNameUnique(string roleName, int rID, int appID)
        {
            return _manageRolesRepository.IsRoleNameUnique(roleName, rID, appID);
        }

        public List<ApplicationModel> GetApplicationListBySA()
        {
            return _manageRolesRepository.GetApplicationListBySA();
        }

        public bool DeActivate(int roleId, bool status, string viewName, int userID)
        {
            return _manageRolesRepository.DeActivate(roleId, status, viewName, userID);
        }

        //Get All Roles specific to User assigned applications
        public List<RolesModel> GetUserRolesToMappedByUserId(DataSourceRequest request, int currentUserId)
        {
            return _manageRolesRepository.GetUserRolesToMappedByUserId(request, currentUserId);
        }


        public bool SaveUserRole(RolesModel roleModel)
        {
            return _manageRolesRepository.SaveUserRole(roleModel);
        }
    }
}
