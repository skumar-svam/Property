using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using NA.PMS.Model;
using NA.PMS.Common;
using System.Configuration;
using NA.PMS.Web.Models;
using System.Web;

namespace NA.PMS.Repository
{
    public class MenuMappingRepository : IMenuMappingRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();

        public MenuMappingRepository()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["CurrentUser"] != null)
                    {
                        userInfo = HttpContext.Current.Session["CurrentUser"] as CurrentUserDetail;
                    }
                }
            }
        }
        /// <summary>
        /// Get all roles
        /// </summary>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public IEnumerable<Roles> GetRoles(string roleType)
        {
            var result = new List<Roles>();
            using (var dbContext = new NoidaPMSEntities())
            {
                if (roleType.ToLower() == Constants.SuperAdmin.ToLower())
                {
                    result = (from lstRoles in dbContext.UmRoleMasters.ToList()
                              where lstRoles.RoleType.ToLower() == Constants.Admin.ToLower() && lstRoles.IsActive == true
                              select new Roles
                              {
                                  roleId = lstRoles.RoleId,
                                  roleName = lstRoles.RoleName,
                                  roleDescription = lstRoles.RoleDescription,
                                  roleType = lstRoles.RoleType
                              }).ToList();
                }
                else if (roleType.ToLower() == Constants.Admin.ToLower())
                {
                    result = (from lstRoles in dbContext.UmRoleMasters.ToList()
                              where lstRoles.RoleType.ToLower() != Constants.Admin.ToLower() && lstRoles.RoleType.ToLower() != Constants.SuperAdmin.ToLower()
                              && lstRoles.IsActive == true
                              select new Roles
                              {
                                  roleId = lstRoles.RoleId,
                                  roleName = lstRoles.RoleName,
                                  roleDescription = lstRoles.RoleDescription,
                                  roleType = lstRoles.RoleType
                              }).ToList();
                }
            }
            return result;
        }

        /// <summary>
        /// Used for fetching Role data according to the Application ID and Login User Role Type. 
        /// </summary>
        /// <param name="appID">Application ID</param>
        /// <param name="roleType">Role Type of the Login User</param>
        /// <returns></returns>
        public IEnumerable<Roles> GetRolesByApplicationId(int appID, string roleType)
        {
            var result = new List<Roles>();
            try
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    if (roleType.ToLower() == Constants.SuperAdmin.ToLower())
                    {
                        //result = (from lstRoles in dbContext.UmRoleMasters
                        //          join lstRolesTrans in dbContext.UmRoleAppTrans on lstRoles.RoleId equals lstRolesTrans.RoleId
                        //          where lstRolesTrans.ApplicationId == appID
                        //          && lstRoles.RoleType.ToLower() == Constants.Admin.ToLower()
                        //          && lstRoles.IsActive == true
                        //          select new Roles
                        //          {
                        //              roleId = lstRoles.RoleId,
                        //              roleName = lstRoles.RoleName,
                        //              roleDescription = lstRoles.RoleDescription,
                        //              roleType = lstRoles.RoleType
                        //          }).ToList();
                        result = (from r in dbContext.UmRoleMasters
                                  join ra in dbContext.UmRoleAppTrans on r.RoleId equals ra.RoleId
                                  join a in dbContext.UmApplicationMasters on ra.ApplicationId equals a.ApplicationId
                                  where r.IsActive == true && a.ApplicationId == appID && r.RoleType.ToLower() == Constants.Admin.ToLower()
                                  select new Roles
                                  {
                                      roleId = r.RoleId,
                                      roleName = r.RoleName,
                                      roleDescription = r.RoleDescription,
                                      roleType = r.RoleType
                                  }).ToList();
                    }
                    else if (roleType.ToLower() == Constants.Admin.ToLower())
                    {
                        //result = (from lstRoles in dbContext.UmRoleMasters
                        //          join lstRolesTrans in dbContext.UmRoleAppTrans on lstRoles.RoleId equals lstRolesTrans.RoleId
                        //          where lstRolesTrans.ApplicationId == appID
                        //          && lstRoles.RoleType.ToLower() != Constants.Admin.ToLower() && lstRoles.RoleType.ToLower() != Constants.SuperAdmin.ToLower()
                        //          && lstRoles.IsActive == true
                        //          select new Roles
                        //          {
                        //              roleId = lstRoles.RoleId,
                        //              roleName = lstRoles.RoleName,
                        //              roleDescription = lstRoles.RoleDescription,
                        //              roleType = lstRoles.RoleType
                        //          }).ToList();
                        result = (from r in dbContext.UmRoleMasters
                                  join ra in dbContext.UmRoleAppTrans on r.RoleId equals ra.RoleId
                                  join a in dbContext.UmApplicationMasters on ra.ApplicationId equals a.ApplicationId
                                  where r.IsActive == true && a.ApplicationId == appID && r.RoleType.ToLower() == Constants.User.ToLower()
                                  select new Roles
                                  {
                                      roleId = r.RoleId,
                                      roleName = r.RoleName,
                                      roleDescription = r.RoleDescription,
                                      roleType = r.RoleType
                                  }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Get all Menus 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MenuMappingDetail> GetMenusByRoleId()
        {
            var result = new List<MenuMappingDetail>();
            try
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    result = (from viewApplicationMenuMasters in dbContext.ViewApplicationMenuMasters.ToList()
                              where viewApplicationMenuMasters.MenuId != Convert.ToInt32(ConfigurationManager.AppSettings["UserManagementMenuId"]) && viewApplicationMenuMasters.MenuParentId != Convert.ToInt32(ConfigurationManager.AppSettings["UserManagementMenuId"])
                              select new MenuMappingDetail
                              {
                                  applicationId = viewApplicationMenuMasters.ApplicationId,
                                  menuId = viewApplicationMenuMasters.MenuId,
                                  applicationName = viewApplicationMenuMasters.ApplicationName,
                                  menuName = viewApplicationMenuMasters.MenuName,
                                  menuParentId = viewApplicationMenuMasters.MenuParentId,
                                  menuPathId = viewApplicationMenuMasters.MenuPathId
                              }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Get all selected menus which was selected at the time of menu mapping on the basis of applicationId and RoleId
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MenuMappingDetail> GetSelectedMenus()
        {
            var result = new List<MenuMappingDetail>();
            try
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    result = (from viewApplicationMenuMasters in dbContext.UmRoleMasterTrans.ToList()
                              select new MenuMappingDetail
                              {
                                  applicationId = viewApplicationMenuMasters.ApplicationId,
                                  menuPathId = viewApplicationMenuMasters.MenuId,
                                  roleId = viewApplicationMenuMasters.RoleId,
                                  IsRead = viewApplicationMenuMasters.IsRead,
                                  IsWrite = viewApplicationMenuMasters.IsWrite,
                                  IsModify = viewApplicationMenuMasters.IsUpdate,
                                  IsDelete = viewApplicationMenuMasters.Isdelete
                              }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Save Menu Mapping Read Write Permissions
        /// </summary>
        /// <param name="lstMenuMapping"></param>
        /// <param name="roleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool SaveMenuMapping(IEnumerable<MenuMappingDetail> lstMenuMapping, int roleId, int userId)
        {
            var isDataInserted = false;
            using (var dbContext = new NoidaPMSEntities())
            {                
                var roleTrans = dbContext.UmRoleMasterTrans.Where(x => x.RoleId == roleId).ToList();
                if (roleTrans.Count > 0)
                {
                    foreach (UmRoleMasterTran objUmRoleMasterTran in roleTrans)
                    {
                        dbContext.UmRoleMasterTrans.Remove(objUmRoleMasterTran);
                    }
                    dbContext.SaveChanges();
                }
                var objresult = dbContext.UmRoleMasters.FirstOrDefault(id => id.RoleId == roleId);
                if (objresult.RoleType.Trim().ToUpper() == Constants.Admin)
                {
                    Int32 userManagement = Convert.ToInt32(ConfigurationManager.AppSettings["UserManagementApplicationID"]);
                    Int32 manageUserRoles = Convert.ToInt32(ConfigurationManager.AppSettings["ManageUserRoles"]);
                    Int32 manageAdminUsers = Convert.ToInt32(ConfigurationManager.AppSettings["ManageAdminUsers"]);
                    var umRoleMasterTran = new UmRoleMasterTran
                    {
                        RoleId = roleId,
                        ApplicationId = userManagement,
                        MenuId = manageUserRoles,
                        IsRead = true,
                        IsWrite = true,
                        Isdelete = true,
                        IsUpdate = true,
                        CreatedBy = userId.ToString(),
                        CreatedDate = DateTime.Now
                    };

                    var umRoleMasterTran1 = new UmRoleMasterTran
                    {
                        RoleId = roleId,
                        ApplicationId = userManagement,
                        MenuId = manageAdminUsers,
                        IsRead = true,
                        IsWrite = true,
                        Isdelete = true,
                        IsUpdate = true,
                        CreatedBy = userId.ToString(),
                        CreatedDate = DateTime.Now
                    };
                    //end 
                    dbContext.UmRoleMasterTrans.Add(umRoleMasterTran);
                    dbContext.UmRoleMasterTrans.Add(umRoleMasterTran1);
                }
                foreach (var MenuMappingDetail in lstMenuMapping)
                {
                    var roleMasterTrans = new UmRoleMasterTran();
                    roleMasterTrans.RoleId = roleId;
                    roleMasterTrans.ApplicationId = MenuMappingDetail.applicationId;
                    roleMasterTrans.MenuId = MenuMappingDetail.menuId;
                    roleMasterTrans.IsRead = MenuMappingDetail.IsRead;
                    roleMasterTrans.IsWrite = MenuMappingDetail.IsWrite;
                    roleMasterTrans.IsUpdate = MenuMappingDetail.IsModify;
                    roleMasterTrans.Isdelete = MenuMappingDetail.IsDelete;
                    roleMasterTrans.CreatedDate = DateTime.Now;
                    roleMasterTrans.CreatedBy = userId.ToString();
                    roleMasterTrans.ModifiedBy = userId.ToString();
                    roleMasterTrans.ModifiedDate = DateTime.Now;
                    dbContext.UmRoleMasterTrans.Add(roleMasterTrans);
                }
                dbContext.SaveChanges();
                isDataInserted = true;
            }
            return isDataInserted;
        }

        /// <summary>
        /// Getting Menus and Submenus by RoleIds
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>

        public List<GetMenuID> GetMenuIdsByRoleID(List<int> roleIds)
        {
            var menuIds = new List<GetMenuID>();
            using (var context = new NoidaPMSEntities())
            {
                menuIds = (from umRoleMasterTrans in context.UmRoleMasterTrans
                           join dd in context.UmMenuMasters on umRoleMasterTrans.MenuId equals dd.MenuId
                           where roleIds.Contains(umRoleMasterTrans.RoleId)
                            && dd.MenuParentId != null
                           select new GetMenuID
                           {
                               menuID = umRoleMasterTrans.MenuId,
                               parentID = dd.MenuParentId.Value
                           }).ToList();
            }
            return menuIds;
        }

        
    }
}