using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using NA.PMS.Model;

namespace NA.PMS.Service
{
    public interface IManageRolesService
    {
        List<RolesModel> GetAllRoles(string roletype);
        List<ApplicationModel> GetAllApplicationList(int userID);
        bool AddRole(RolesModel rmodel, string roletype, int currentUser);
        RolesModel GetRecordById(int id);
        bool UpdateRecordById(RolesModel rmodel, string roletype, int currentUser);
        bool DeleteRecordById(int roleID);
        bool IsRoleNameUnique(string roleName, int rID, int appID);
        List<ApplicationModel> GetApplicationListBySA();
        // Getting records by Application ID
        RolesModel GetRecordByApplicationId(int id);
        // Activate / Deactivate roles
        bool DeActivate(int roleId, bool status, string viewName, int userID);
        //Get All Roles specific to User assigned applications
        List<RolesModel> GetUserRolesToMappedByUserId(DataSourceRequest request, int currentUserId);

        bool SaveUserRole(RolesModel roleModel);
    }
}
