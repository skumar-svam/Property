using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Model;
using NA.PMS.Common;
using System.Web;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.Infrastructure.Implementation;
using Kendo.Mvc.UI;
using System.Configuration;
using System.Web.Mvc;
using NA.PMS.Web.Models;

namespace NA.PMS.Repository
{
    public class ManageRolesRepository : IManageRolesRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();

        public ManageRolesRepository()
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
        public List<RolesModel> GetAllRoles(string roleType)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                roleType = roleType == Constants.SuperAdmin ? Constants.Admin : Constants.User;

                var lstRoles = (from rat in dbContext.UmRoleAppTrans
                                join am in dbContext.UmApplicationMasters on rat.ApplicationId equals am.ApplicationId
                                join rm in dbContext.UmRoleMasters on rat.RoleId equals rm.RoleId
                                where (rm.RoleId != Constants.SuperAdminRoleId && rm.RoleType == roleType)
                                select new RolesModel
                                {
                                    RoleId = rm.RoleId,
                                    RoleName = rm.RoleName,
                                    ApplicationId = am.ApplicationId,
                                    ApplicationName = am.ApplicationName,
                                    IsActive = rm.IsActive,
                                    RoleDescription = rm.RoleDescription
                                }
                    ).ToList();
                return lstRoles;
            }
        }

        public List<ApplicationModel> GetAllApplicationList(int userID)
        {
            var dataResult = new List<ApplicationModel>();
            var UMAppId = Convert.ToInt32(ConfigurationManager.AppSettings["UserManagementApplicationID"]);
            using (var dbContext = new NoidaPMSEntities())
            {
                dataResult = (from r1 in dbContext.UmApplicationMasters.ToList()
                              where r1.ApplicationId != UMAppId
                              select new ApplicationModel
                             {
                                 ApplicationId = r1.ApplicationId,
                                 ApplicationName = r1.ApplicationName
                             }).ToList();

                var dataResult1 = (from r1 in dbContext.ViewUmUserMasters
                                   where r1.UserRefId == userID && r1.ApplicationId != UMAppId
                                   select new ApplicationModel
                                   {
                                       ApplicationId = r1.ApplicationId.Value
                                   }).Distinct().ToList();
                var matchingAppID = dataResult.Where(y => dataResult1.Any(z => z.ApplicationId == y.ApplicationId)).ToList();
                return matchingAppID;
                //return dataResult;
            }
        }

        public bool AddRole(RolesModel roleModel, string roleType, int currentUser)
        {
            bool result = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (roleType != Constants.SuperAdmin)
                {
                    var dataResult = new UmRoleMaster
                    {
                        RoleName = roleModel.RoleName,
                        RoleDescription = roleModel.RoleDescription,
                        CreatedBy = currentUser.ToString(),
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        RoleType = Constants.User
                    };
                    dbContext.UmRoleMasters.Add(dataResult);
                    var i = dbContext.UmRoleMasters.FirstOrDefault(x => x.RoleName.Trim() == roleModel.RoleName.Trim());
                    if (i == null)
                    {
                        dbContext.SaveChanges();
                    }
                    if (dataResult.RoleId > 0)
                    {
                        var umRoleAppTran = new UmRoleAppTran
                        {
                            RoleId = dataResult.RoleId,
                            ApplicationId = roleModel.ApplicationId,
                            CreatedBy = currentUser.ToString(),
                            CreatedDate = DateTime.Now
                        };
                        dbContext.UmRoleAppTrans.Add(umRoleAppTran);
                        dbContext.SaveChanges();
                    }
                    RolesModel rolesModel1 = new RolesModel();
                    GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.UmRoleAppTrans, roleModel.ViewName, dataResult.RoleId, rolesModel1, roleModel, currentUser.ToString());
                    result = true;
                }
                else if (roleType == Constants.SuperAdmin)
                {
                    var objresult = dbContext.UmRoleMasters.FirstOrDefault(id => id.RoleId == roleModel.RoleId);
                    if (objresult != null)
                    {
                        objresult.RoleName = roleModel.RoleName;
                        objresult.RoleDescription = roleModel.RoleDescription;
                        objresult.IsActive = true;
                        //objresult.RoleType = roleType.ToString();
                        objresult.ModifiedBy = currentUser.ToString();
                        objresult.ModifiedDate = DateTime.Now;
                        if (roleModel.RoleId > 0)
                        {
                            var objRoleAppTran = dbContext.UmRoleAppTrans.FirstOrDefault(id => id.RoleId == roleModel.RoleId);
                            objRoleAppTran.RoleId = roleModel.RoleId;
                            objRoleAppTran.ApplicationId = roleModel.ApplicationId;//Convert.ToInt32(rmodel.applications);
                            objRoleAppTran.ModifiedBy = currentUser.ToString();
                            objRoleAppTran.ModifiedDate = DateTime.Now;
                        }
                        dbContext.SaveChanges();
                        result = true;
                        GeneralRepository.CreateAuditTrail(Constants.Update, Constants.UmRoleAppTrans, roleModel.ViewName, roleModel.RoleId, objresult, roleModel, currentUser.ToString());
                    }
                    else
                    {
                        Int32 userManagement = Convert.ToInt32(ConfigurationManager.AppSettings["UserManagementApplicationID"]);
                        Int32 manageUserRoles = Convert.ToInt32(ConfigurationManager.AppSettings["ManageUserRoles"]);
                        Int32 manageAdminUsers = Convert.ToInt32(ConfigurationManager.AppSettings["ManageAdminUsers"]);

                        var dataResult = new UmRoleMaster
                        {
                            RoleName = roleModel.RoleName,
                            RoleDescription = roleModel.RoleDescription,
                            CreatedBy = currentUser.ToString(),
                            CreatedDate = DateTime.Now,
                            IsActive = true,
                            RoleType = Constants.Admin
                        };
                        dbContext.UmRoleMasters.Add(dataResult);
                        var i = dbContext.UmRoleMasters.FirstOrDefault(x => x.RoleName.Trim() == roleModel.RoleName.Trim());
                        if (i == null)
                        {
                            dbContext.SaveChanges();
                        }
                        // dbContext.SaveChanges();
                        if (dataResult.RoleId > 0)
                        {
                            var umRoleAppTran = new UmRoleAppTran
                            {
                                RoleId = dataResult.RoleId,
                                ApplicationId = roleModel.ApplicationId,
                                CreatedBy = currentUser.ToString(),
                                CreatedDate = DateTime.Now
                            };
                            dbContext.UmRoleAppTrans.Add(umRoleAppTran);

                            //start
                            // When a admin role created by super admin, by default two menus will show for this new admin. 
                            var umRoleMasterTran = new UmRoleMasterTran
                            {
                                RoleId = dataResult.RoleId,
                                ApplicationId = userManagement,
                                MenuId = manageUserRoles,
                                IsRead = true,
                                IsWrite = true,
                                Isdelete = true,
                                IsUpdate = true,
                                CreatedBy = currentUser.ToString(),
                                CreatedDate = DateTime.Now
                            };

                            var umRoleMasterTran1 = new UmRoleMasterTran
                            {
                                RoleId = dataResult.RoleId,
                                ApplicationId = userManagement,
                                MenuId = manageAdminUsers,
                                IsRead = true,
                                IsWrite = true,
                                Isdelete = true,
                                IsUpdate = true,
                                CreatedBy = currentUser.ToString(),
                                CreatedDate = DateTime.Now
                            };
                            //end 
                            dbContext.UmRoleMasterTrans.Add(umRoleMasterTran);
                            dbContext.UmRoleMasterTrans.Add(umRoleMasterTran1);
                            dbContext.SaveChanges();
                        }
                        result = true;
                        GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.UmRoleAppTrans, roleModel.ViewName, roleModel.RoleId, objresult, roleModel, currentUser.ToString());
                    }
                }
            }
            return result;
        }

        public RolesModel GetRecordById(int roleId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var dataResult = (from rat in dbContext.UmRoleAppTrans
                                  join am in dbContext.UmApplicationMasters on rat.ApplicationId equals am.ApplicationId
                                  join rm in dbContext.UmRoleMasters on rat.RoleId equals rm.RoleId
                                  where (rm.RoleId == roleId)
                                  select new RolesModel
                                  {
                                      RoleId = rm.RoleId,
                                      RoleName = rm.RoleName,
                                      RoleInDepartment = rm.RoleInDepartment,
                                      RoleDescription = rm.RoleDescription,
                                      ApplicationId = am.ApplicationId,
                                      ApplicationName = am.ApplicationName
                                  }
                    ).FirstOrDefault();
                return dataResult;
            }
        }
        /// <summary>
        /// Getting record by Application Id
        /// </summary>
        /// <param name="applicationId">Application Id</param>
        /// <returns></returns>
        public RolesModel GetRecordByApplicationId(int applicationId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var dataResult = (from rat in dbContext.UmRoleAppTrans
                                  join am in dbContext.UmApplicationMasters on rat.ApplicationId equals am.ApplicationId
                                  join rm in dbContext.UmRoleMasters on rat.RoleId equals rm.RoleId
                                  where (am.ApplicationId == applicationId)
                                  select new RolesModel
                                  {
                                      RoleId = rm.RoleId,
                                      RoleName = rm.RoleName,
                                      RoleDescription = rm.RoleDescription,
                                      ApplicationId = am.ApplicationId,
                                      ApplicationName = am.ApplicationName
                                  }
                    ).FirstOrDefault();

                return dataResult;
            }
        }

        public bool UpdateRecordById(RolesModel rmodel, string roleType, int currentUser)
        {
            bool result = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var objresult = dbContext.UmRoleMasters.FirstOrDefault(id => id.RoleId == rmodel.RoleId);
                if (objresult != null)
                {
                    var objOld = new RolesModel
                    {
                        RoleId = objresult.RoleId,
                        RoleName = objresult.RoleName,
                        RoleDescription = objresult.RoleDescription,
                        ApplicationId = rmodel.ApplicationId,
                        ApplicationName = rmodel.ApplicationName
                    };
                    objresult.RoleName = rmodel.RoleName;
                    objresult.RoleDescription = rmodel.RoleDescription;
                    objresult.IsActive = true;
                    objresult.ModifiedBy = currentUser.ToString();
                    objresult.ModifiedDate = DateTime.Now;
                    if (rmodel.RoleId > 0)
                    {
                        var objRoleAppTran = dbContext.UmRoleAppTrans.FirstOrDefault(id => id.RoleId == rmodel.RoleId);
                        objRoleAppTran.RoleId = rmodel.RoleId;
                        objRoleAppTran.ApplicationId = rmodel.ApplicationId;
                        objRoleAppTran.ModifiedBy = currentUser.ToString();
                        objRoleAppTran.ModifiedDate = DateTime.Now;
                    }
                    dbContext.SaveChanges();
                    result = true;
                    GeneralRepository.CreateAuditTrail(Constants.Update, Constants.UmRoleAppTrans, rmodel.ViewName, rmodel.RoleId, objOld, rmodel, currentUser.ToString());
                }

            }
            return result;
        }

        public bool DeleteRecordById(int roleID)
        {
            bool flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var objresult = dbContext.UmRoleMasters.FirstOrDefault(id => id.RoleId == roleID);
                if (objresult != null)
                {
                    objresult.IsActive = false;
                    dbContext.SaveChanges();
                    flag = true;
                }
                return flag;
            }
        }

        public bool IsRoleNameUnique(string roleName, int rID, int appID)
        {
            Boolean flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var varRoles = dbContext.UmRoleMasters.FirstOrDefault(r => r.RoleName == roleName && r.IsActive == true && r.RoleId != rID);
                if (varRoles == null)
                {
                    var NameExit = (from app in dbContext.UmRoleMasters
                                    join trans in dbContext.UmRoleAppTrans on app.RoleId equals trans.RoleId
                                    where (app.RoleName.ToLower().Trim() == roleName.ToLower().Trim()
                                    && trans.ApplicationId == appID
                                    && app.IsActive == true
                                    && app.RoleId != rID)
                                    select app.RoleId).FirstOrDefault();
                    if (NameExit == 0)
                    {
                        flag = false;
                    }
                    else { flag = true; }
                }
                else { flag = true; }
            }
            return flag;
        }

        public List<ApplicationModel> GetApplicationListBySA()
        {
            var UMAppId = Convert.ToInt32(ConfigurationManager.AppSettings["UserManagementApplicationID"]);
            using (var dbContext = new NoidaPMSEntities())
            {
                var dataResult = (from am in dbContext.UmApplicationMasters
                                  where am.ApplicationId != UMAppId
                                  select new ApplicationModel
                                  {
                                      ApplicationId = am.ApplicationId,
                                      ApplicationName = am.ApplicationName
                                  }
                    ).ToList();
                return dataResult;
            }
        }

        /// <summary>
        /// Activate / Deactivate Roles
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <param name="status">Status</param>
        /// <returns></returns>
        public bool DeActivate(int roleId, bool status, string viewName, int userID)
        {
            using (var context = new NoidaPMSEntities())
            {
                var effectedRec = 0;
                var tblRole = context.UmRoleMasters.FirstOrDefault(c => c.RoleId == roleId);
                if (tblRole != null && tblRole.RoleId != 0)
                {
                    var objOld = new RolesModel
                    {
                        IsActive = tblRole.IsActive
                    };
                    tblRole.IsActive = !status;
                    var objNew = new RolesModel
                    {
                        IsActive = tblRole.IsActive
                    };
                    effectedRec = context.SaveChanges();
                    if (effectedRec > 0)
                        GeneralRepository.CreateAuditTrail(Constants.Delete, Constants.UmRoleAppTrans, viewName, roleId, objOld, objNew, userID.ToString());
                }
                return effectedRec > 0;
            }
        }

        //Get All Roles specific to User assigned applications
        public List<RolesModel> GetUserRolesToMappedByUserId(DataSourceRequest request, int currentUserId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstRoles =
                    dbContext.Database.SqlQuery<RolesModel>(
                        "SELECT A.RoleId, A.RoleName, A.IsActive, B.ApplicationId, C.ApplicationName, A.RoleType FROM  UmRoleMaster AS A INNER JOIN " +
                        " UmRoleAppTrans AS B ON A.RoleId = B.RoleId INNER JOIN " +
                        " UmApplicationMaster AS C ON B.ApplicationId = C.ApplicationId WHERE (C.IsActive = 1) AND (A.RoleType = 'U') " +
                        " AND (C.ApplicationId IN (SELECT ApplicationId FROM dbo.ViewRoleWiseApplication WHERE (UserRefId = " + currentUserId + ") AND (RoleType = 'A')))"
                        ).ToList<RolesModel>();


                return lstRoles;
            }

        }


        public bool SaveUserRole(RolesModel roleModel)
        {
            bool result = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var role = dbContext.UmRoleMasters.FirstOrDefault(r => r.RoleName.Trim() == roleModel.RoleName.Trim());
                if (role == null)
                {
                    var rolemaster = new UmRoleMaster
                    {
                        RoleName = roleModel.RoleName,
                        RoleInDepartment = roleModel.RoleInDepartment,
                        RoleDescription = roleModel.RoleDescription,
                        CreatedBy = userInfo.UserID.ToString(),
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        RoleType = Constants.User
                    };
                    dbContext.UmRoleMasters.Add(rolemaster);
                    dbContext.SaveChanges();
                    if (rolemaster.RoleId > 0)
                    {
                        var roleAppTrans = new UmRoleAppTran
                        {
                            RoleId = rolemaster.RoleId,
                            ApplicationId = roleModel.ApplicationId,
                            CreatedBy = userInfo.UserID.ToString(),
                            CreatedDate = DateTime.Now
                        };
                        dbContext.UmRoleAppTrans.Add(roleAppTrans);
                        dbContext.SaveChanges();
                    }

                    RolesModel rolesModel1 = new RolesModel();
                    GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.UmRoleAppTrans, roleModel.ViewName, rolemaster.RoleId, rolesModel1, roleModel, userInfo.UserID.ToString());
                    result = true;
                }
                else
                {
                        role.RoleName = roleModel.RoleName;
                        role.RoleInDepartment = roleModel.RoleInDepartment;
                        role.RoleDescription = roleModel.RoleDescription;
                        role.ModifiedBy = userInfo.UserID.ToString();
                        role.ModifiedDate = DateTime.Now;
                        role.IsActive = true;
                        role.RoleType = Constants.User;
                        dbContext.SaveChanges();
                        result = true;
                }
            }
            return result;
        }
    }
} //end of namespace and class.
