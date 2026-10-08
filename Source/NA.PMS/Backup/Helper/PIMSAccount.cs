using NA.PMS.NICService.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.NICServices
{
    public class PIMSAccount
    {
        public AccountViewModel GetLoginAccountDetails(string userName)
        {
            AccountViewModel _LoginAccount = null;
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["CurrentUser"] != null)
                    {
                        using (var dbContext = new PIMSEntitiesContext())
                        {
                            _LoginAccount = (from user in dbContext.UmUserMasters
                                             where user.UserName == userName
                                             select new AccountViewModel
                                             {
                                                 UserRefId = user.UserRefId,
                                                 UserName = user.UserName,
                                                 FirstName = user.FirstName,
                                                 LastName = user.LastName,
                                                 MiddleName = user.MiddleName,
                                                 IsActive = user.IsActive,
                                                 Email = user.Email,
                                                 Mobile = user.Mobile,
                                                 DepartmentIdList = dbContext.UmUserDepartmentTrans.Where(u => u.UserRefId == user.UserRefId && u.Status == true).Select(d => d.DepartmentId).ToList(),
                                                 DepartmentList = (from depts in dbContext.UmDepartmentMasters
                                                                   join deptsTrans in dbContext.UmUserDepartmentTrans on depts.DepartmentId equals deptsTrans.DepartmentId
                                                                   where deptsTrans.UserRefId == user.UserRefId
                                                                   select new NADepartmentMst
                                                                   {
                                                                       DepartmentId = depts.DepartmentId,
                                                                       DepartmentName = depts.DepartmentName,
                                                                       Status = depts.Status
                                                                   }).ToList(),

                                                 RolesList = (from roles in dbContext.UmRoleMasters
                                                              join umr in dbContext.UmUserMasterRoles on roles.RoleId equals umr.RoleId
                                                              where umr.UserRefId == user.UserRefId
                                                              select new NARoleMaster
                                                              {
                                                                  RoleId = roles.RoleId,
                                                                  RoleName = roles.RoleName,
                                                                  RoleType = roles.RoleType,
                                                              }).ToList(),

                                                 ApplicationsList = (from app in dbContext.UmApplicationMasters
                                                                     join rat in dbContext.UmRoleAppTrans on app.ApplicationId equals rat.ApplicationId
                                                                     join umr in dbContext.UmUserMasterRoles on rat.RoleId equals umr.RoleId
                                                                     where umr.UserRefId == user.UserRefId
                                                                     select new NAApplicationMst
                                                                     {
                                                                         ApplicationId = app.ApplicationId,
                                                                         ApplicationName = app.ApplicationName,
                                                                         ApplicationUrl = app.ApplicationUrl,
                                                                         IsActive = app.IsActive
                                                                     }).ToList(),

                                                 MenusList = (from menu in dbContext.UmMenuMasters
                                                              join rmt in dbContext.UmRoleMasterTrans on menu.MenuId equals rmt.MenuId
                                                              join umr in dbContext.UmUserMasterRoles on rmt.RoleId equals umr.RoleId
                                                              where umr.UserRefId == user.UserRefId
                                                              select new NAActionMenu
                                                              {
                                                                  MenuId = menu.MenuId,
                                                                  MenuName = menu.MenuName,
                                                                  MenuParentId = menu.MenuParentId,
                                                                  MenuPathId = menu.MenuPathId,
                                                                  IsActive = menu.IsActive,
                                                                  IsRead = rmt.IsRead,
                                                                  IsWrite = rmt.IsWrite,
                                                                  IsUpdate = rmt.IsUpdate,
                                                                  Isdelete = rmt.Isdelete
                                                              }).ToList()

                                             }).FirstOrDefault();

                        }
                    }
                }
            }



            return _LoginAccount;
        }
    }


}
