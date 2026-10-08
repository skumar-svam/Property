using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace NA.PMS.Service
{
    public class ManageUsersService : IManageUsersService
    {
        IManageUsersRepository _userRepository;
        public ManageUsersService()
        {
            _userRepository = new ManageUsersRepository();
        }
        public bool SaveUser(UsersModel user, int loginUserId)
        {
            return _userRepository.SaveUser(user, loginUserId);
        }

        public UsersModel GetUserDetailsById(int id)
        {
            return _userRepository.GetUserDetailsById(id);
        }

        public List<CheckBoxListItem> GetAllDeptts()
        {
            return _userRepository.GetAllDeptts();
        }

        public List<UsersModel> GetAllUsers(DataSourceRequest request, string roleType, int loginUserId)
        {
            return _userRepository.GetAllUsers(request, roleType, loginUserId);
        }

        public bool LockUnlockToggle(int id, bool isActive)
        {
            return _userRepository.LockUnlockToggle(id, isActive);
        }

        public bool LockUnlockRoleToggle(int id, bool isActive, int roleId, int applicationID)
        {
            return _userRepository.LockUnlockRoleToggle(id,isActive, roleId,applicationID);
        }

        public bool CheckUsernameDuplicacy(string empID, int id)
        {
            return _userRepository.CheckUsernameDuplicacy(empID, id);
        }

        public List<UsersModel> SearchUsers(DataSourceRequest request, string empID, int loginUserId)
        {
            return _userRepository.SearchUsers(request, empID, loginUserId);
        }

        public int MapUserToRole(int id, int roleID, int loginUser)
        {
            return _userRepository.MapUserToRole(id, roleID, loginUser);
        }

        public List<UsersModel> GetRemoveUsers(DataSourceRequest request, int roleID)
        {
            return _userRepository.GetRemoveUsers(request, roleID);
        }

        public bool RemoveUser(int userId, int roleId, int applicationId)
        {
            return _userRepository.RemoveUser(userId, roleId, applicationId);
        }

        public bool ChangePassword(string userName, string email, string newPassword)
        {
            return _userRepository.ChangePassword(userName, email, newPassword);
        }

        public bool CheckMobileNoDuplicacy(string mobile, int id)
        {
            return _userRepository.CheckMobileNoDuplicacy(mobile, id);
        }

        public DataSourceResult GetEntireUsers(DataSourceRequest request)
        {
            return _userRepository.GetEntireUsers(request);
        }


        public List<AdminRoleDetail> GetAdminRoleDetails(int userId)
        {
            return _userRepository.GetAdminRoleDetails(userId);
        }


        public List<UsersModel> GetApplicationUsersListForAdmin()
        {
            return _userRepository.GetApplicationUsersListForAdmin();
        }

        public List<UsersModel> GetApplicationUsersListForAdmin(List<ApplicationMaster> lstapplication)
        {
            return _userRepository.GetApplicationUsersListForAdmin(lstapplication);
        }

        public List<UsersModel> GetApplicationUsers()
        {
            return _userRepository.GetApplicationUsers();
        }
        public List<UsersModel> GetAllUsersSuper(DataSourceRequest request, string roleType, int loginUserId)
        {
            return _userRepository.GetAllUsersSuper(request, roleType, loginUserId);
        }


        public bool CheckEmailDuplicacy(string email, int refID)
        {
            return _userRepository.CheckEmailDuplicacy(email, refID);
        }


        public List<AuditActionModel> GetActionAuditTrail()
        {
            return _userRepository.GetActionAuditTrail();
        }


        public List<AuditActionModel> GetActionAuditDetailByDate(DateTime startDate, DateTime endDate)
        {
            return _userRepository.GetActionAuditDetailByDate(startDate, endDate);
        }


        public List<AuditActionModel> GetAuditTrailActionGroupByModuleName(string moduleName)
        {
            return _userRepository.GetAuditTrailActionGroupByModuleName(moduleName);
        }


        public List<DDList> GetAuditModuleName()
        {
            return _userRepository.GetAuditModuleName();
        }

        public List<DDList> GetAuditActionName(string moduleName)
        {
            return _userRepository.GetAuditActionName(moduleName);
        }

        public List<DDList> GetAuditUserName(string moduleName, string actionName)
        {
            return _userRepository.GetAuditUserName(moduleName, actionName);
        }


        public List<AuditActionModel> GetAuditTrailByAdvanceSearch(string moduleName, string actionName, string userName, DateTime? startDate, DateTime? endDate)
        {
            return _userRepository.GetAuditTrailByAdvanceSearch(moduleName, actionName,userName,startDate,endDate);
        }


        public List<DynamicDataModel> GetRidOfPropertyTransaction()
        {
            return _userRepository.GetRidOfPropertyTransaction();
        }

        public List<DynamicDataModel> GetServiceNameOfPropertyTransaction()
        {
            return _userRepository.GetServiceNameOfPropertyTransaction();
        }

        public List<DynamicDataModel> GetUserOfPropertyTransaction()
        {
            return _userRepository.GetUserOfPropertyTransaction();
        }

        public List<PropertyTransactionAudit> GetTransactionAuditHistory(int? rid, string serviceName, int? user, DateTime? startDate, DateTime? endDate)
        {
            return _userRepository.GetTransactionAuditHistory( rid, serviceName,  user,  startDate,  endDate);
        }


        public List<UsersModel> GetMappedUsersForApplication(int applicationId, int? roleId)
        {
            return _userRepository.GetMappedUsersForApplication(applicationId, roleId);
        }


        public List<UsersModel> GetUserListForSuperAdmin(int? applicationId, int? roleId)
        {
            return _userRepository.GetUserListForSuperAdmin(applicationId, roleId);
        }


        public DataSourceResult GetPropertyCustomersList(DataSourceRequest request, UserViewModel model)
        {
            return _userRepository.GetPropertyCustomersList(request, model);
        }


        public int UpdateCustomerStatus(UserViewModel model)
        {
            return _userRepository.UpdateCustomerStatus(model);
        }


        public DataSourceResult GetAuditActionDetailByAdvanceSearch(DataSourceRequest request, AuditViewModel model)
        {
            return _userRepository.GetAuditActionDetailByAdvanceSearch(request, model);
        }


        public DataSourceResult GetAuditSearchParameter(DataSourceRequest request, AuditViewModel model)
        {
            return _userRepository.GetAuditSearchParameter(request, model);
        }
        #region GetUser Keshav 14 Sep 2018
        public List<UsersModel> GetAllUser(string userName)
        {
            return _userRepository.GetAllUser().Where(x=>x.firstName !=userName).ToList();
        }
        public List<UsersModel> GetUserByName(string userName)
        {
            return _userRepository.GetAllUser().Where(x => x.firstName == userName).ToList();
        }
        #endregion
        //public List<Common.SelectList> GetAllUser()
        //{
        //    var pimsUsers = (from users in _userRepository.GetAllUser()
        //                     select new Common.SelectList
        //                     {
        //                         Text = users.fullName,
        //                         Value = users.id
        //                     }).ToList();
        //    return pimsUsers;
        //}


        public List<CheckBoxViewModel> GetDepartmentCheckBoxList()
        {
            return _userRepository.GetDepartmentCheckBoxList();
        }


        public DataSourceResult GetDepartmentListByIdAsDataSource(DataSourceRequest request, UsersModel model)
        {
            return _userRepository.GetDepartmentListByIdAsDataSource(request, model);
        }


        public DataSourceResult GetUserNameListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _userRepository.GetUserNameListAsDataSource(request, model);
        }


        public UserViewModel GetUsersDetailById(UserViewModel model)
        {
            return _userRepository.GetUsersDetailById(model);
        }


        public int SaveUserDetailById(UserViewModel model)
        {
            return _userRepository.SaveUserDetailById(model);
        }


        public int RegisterCustomerDetails(UserViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            return _userRepository.RegisterCustomerDetails(model, files);
        }
    }
}
