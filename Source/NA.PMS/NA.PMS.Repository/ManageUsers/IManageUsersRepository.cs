using Kendo.Mvc.UI;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Repository
{
    public interface IManageUsersRepository
    {
        bool SaveUser(UsersModel user, int loginUserId);
        UsersModel GetUserDetailsById(int id);
        List<CheckBoxListItem> GetAllDeptts();
        List<UsersModel> GetAllUsers(DataSourceRequest request, string roleType, int loginUserId);
        bool LockUnlockToggle(int id, bool isActive);
        bool CheckUsernameDuplicacy(string empID, int id);
        bool CheckMobileNoDuplicacy(string mobile, int id);
        List<UsersModel> SearchUsers(DataSourceRequest request, string empID, int loginUserId);
        int MapUserToRole(int id, int roleID, int loginUser);
        List<UsersModel> GetRemoveUsers(DataSourceRequest request, int roleID);
        bool RemoveUser(int userId, int roleId, int applicationId);
        Boolean sendOTPtoUser(string mobNo, int otp);
        bool ChangePassword(string userName, string email, string newPassword);
        DataSourceResult GetEntireUsers(DataSourceRequest request);

        List<AdminRoleDetail> GetAdminRoleDetails(int userId);

        List<UsersModel> GetApplicationUsersListForAdmin();
        List<UsersModel> GetApplicationUsersListForAdmin(List<ApplicationMaster> lstapplication);

        bool LockUnlockRoleToggle(int id, bool isActive, int roleId, int applicationID);

        List<UsersModel> GetApplicationUsers();
        List<UsersModel> GetAllUsersSuper(DataSourceRequest request, string roleType, int loginUserId);

        bool CheckEmailDuplicacy(string email, int refID);

        List<AuditActionModel> GetActionAuditTrail();

        List<AuditActionModel> GetActionAuditDetailByDate(DateTime startDate, DateTime endDate);

        List<AuditActionModel> GetAuditTrailActionGroupByModuleName(string moduleName);

        List<DDList> GetAuditModuleName();

        List<DDList> GetAuditActionName(string moduleName);

        List<DDList> GetAuditUserName(string moduleName, string actionName);

        List<AuditActionModel> GetAuditTrailByAdvanceSearch(string moduleName, string actionName, string userName, DateTime? startDate, DateTime? endDate);

        List<DynamicDataModel> GetRidOfPropertyTransaction();

        List<DynamicDataModel> GetServiceNameOfPropertyTransaction();

        List<DynamicDataModel> GetUserOfPropertyTransaction();

        List<PropertyTransactionAudit> GetTransactionAuditHistory(int? rid, string serviceName, int? user, DateTime? startDate, DateTime? endDate);

        List<UsersModel> GetMappedUsersForApplication(int applicationId, int? roleId);

        List<UsersModel> GetUserListForSuperAdmin(int? applicationId, int? roleId);

        DataSourceResult GetPropertyCustomersList(DataSourceRequest request, UserViewModel model);

        int UpdateCustomerStatus(UserViewModel model);

        DataSourceResult GetAuditActionDetailByAdvanceSearch(DataSourceRequest request, AuditViewModel model);

        DataSourceResult GetAuditSearchParameter(DataSourceRequest request, AuditViewModel model);
        
        List<UsersModel> GetAllUser(); //  Keshav 14 Sep 2018



        List<CheckBoxViewModel> GetDepartmentCheckBoxList();

        DataSourceResult GetDepartmentListByIdAsDataSource(DataSourceRequest request, UsersModel model);

        DataSourceResult GetUserNameListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        UserViewModel GetUsersDetailById(UserViewModel model);

        int SaveUserDetailById(UserViewModel model);

        int RegisterCustomerDetails(UserViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files);
    }
}
