using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Common.Extension;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Net;
using System.Configuration;
using System.IO;
using NA.PMS.Common.Helpers;
using NoidaAuthority.PMS.Common;
using NA.PMS.Common;
using NA.PMS.Web.Models;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using NA.PMS.Dal.CustomerContext;
using System.Data.Entity;

namespace NA.PMS.Repository
{
    public class ManageUsersRepository : IManageUsersRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public ManageUsersRepository()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["CurrentUser"] != null)
                    {
                        userInfo = HttpContext.Current.Session["CurrentUser"] as CurrentUserDetail;
                        using (var dbContext = new NoidaPMSEntities())
                        {
                            DepartmentList = dbContext.UmUserDepartmentTrans.Where(u => u.UserRefId == userInfo.UserID && u.Status == true).Select(d => d.DepartmentId).ToList();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Used for Adding/Updating Users
        /// </summary>
        /// <param name="user">User details</param>
        /// <param name="loginUserId">Used for CreatedBy</param>
        /// <returns></returns>
        public bool SaveUser(UsersModel user, int loginUserId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingUser = dbContext.UmUserMasters.Where(i => i.UserRefId == user.id).FirstOrDefault();
                if (existingUser != null) //Updating existing User
                {
                    existingUser.Email = user.email;
                    existingUser.FirstName = user.firstName;
                    existingUser.MiddleName = user.middleName;
                    existingUser.LastName = user.lastName;
                    existingUser.Mobile = user.mobile;
                    existingUser.ModifiedBy = loginUserId.ToString();
                    existingUser.ModifiedDate = DateTime.Now;
                    var depts = dbContext.UmUserDepartmentTrans.Where(d => d.UserRefId == user.id).ToList();
                    foreach (var item in depts)
                    {
                        dbContext.UmUserDepartmentTrans.Remove(item);
                    }
                    
                    foreach (var item in user.depttIDs)
                    {
                        if (item.isChecked == true)
                        {
                            var dept = new UmUserDepartmentTran();
                            dept.UserRefId = user.id;
                            dept.DepartmentId = item.id;
                            dept.Status = true;
                            dept.CreatedBy = loginUserId.ToString();
                            dept.CreateDate = DateTime.Now;
                            dbContext.UmUserDepartmentTrans.Add(dept);
                        }
                    }

                    var subdepts = dbContext.UmUserSubDepartmentTrans.Where(s => s.UserRefId == user.id).ToList();
                    foreach (var subd in subdepts)
                    {
                        dbContext.UmUserSubDepartmentTrans.Remove(subd);
                    }

                    foreach (var item in user.depttIDs)
                    {
                        if (item.isChecked == true)
                        {
                            foreach (var sdept in user.SubDepartmentList)
                            {
                                if (sdept.IsChecked == true)
                                {
                                    var sdeptrans = new UmUserSubDepartmentTran();
                                    sdeptrans.UserRefId = user.id;
                                    sdeptrans.DepartmentId = item.id;
                                    sdeptrans.SubDepartmentId = sdept.Id;
                                    sdeptrans.Status = true;
                                    sdeptrans.ModifiedBy = loginUserId;
                                    sdeptrans.ModifiedDate = DateTime.Now;
                                    dbContext.UmUserSubDepartmentTrans.Add(sdeptrans);
                                }
                            }
                        }
                    }
                    //foreach (var sdept in user.SubDepartmentList)
                    //{
                    //    if (sdept.IsChecked == true)
                    //    {
                    //        var sdeptrans = new UmUserSubDepartmentTran();
                    //        sdeptrans.UserRefId = user.id;
                    //        sdeptrans.SubDepartmentId = sdept.Id;
                    //        sdeptrans.Status = true;
                    //        sdeptrans.CreatedBy = loginUserId;
                    //        sdeptrans.CreatedDate = DateTime.Now;
                    //        dbContext.UmUserSubDepartmentTrans.Add(sdeptrans);
                    //    }
                    //}
                    dbContext.SaveChanges();
                }
                else //Adding new User
                {
                    var newUser = new UmUserMaster();
                    newUser.Email = user.email;
                    newUser.FirstName = user.firstName;
                    newUser.MiddleName = user.middleName;
                    newUser.LastName = user.lastName;
                    newUser.Mobile = user.mobile;
                    newUser.UserName = user.empID;
                    newUser.IsActive = true;
                    var password = CreatePassword();
                    newUser.Password = password.ToMD5HashForPassword();
                    newUser.CreatedBy = loginUserId.ToString();
                    newUser.CreatedDate = DateTime.Now;
                    dbContext.UmUserMasters.Add(newUser);
                    var datacheck = dbContext.UmUserMasters.Where(x => x.UserName == user.empID).FirstOrDefault();
                    if (datacheck == null)
                    {
                        dbContext.SaveChanges();
                        foreach (var item in user.depttIDs)
                        {
                            if (item.isChecked == true)
                            {
                                var dept = new UmUserDepartmentTran();
                                dept.UserRefId = newUser.UserRefId;
                                dept.DepartmentId = item.id;
                                dept.Status = true;
                                dept.CreatedBy = loginUserId.ToString();
                                dept.CreateDate = DateTime.Now;
                                dbContext.UmUserDepartmentTrans.Add(dept);
                            }
                        }

                        //foreach (var sdept in user.SubDepartmentList)
                        //{
                        //    if (sdept.IsChecked == true)
                        //    {
                        //        var sdeptrans = new UmUserSubDepartmentTran();
                        //        sdeptrans.UserRefId = user.id;
                        //        sdeptrans.SubDepartmentId = sdept.Id;
                        //        sdeptrans.Status = true;
                        //        sdeptrans.CreatedBy = loginUserId;
                        //        sdeptrans.CreatedDate = DateTime.Now;
                        //        dbContext.UmUserSubDepartmentTrans.Add(sdeptrans);
                        //    }
                        //}

                        foreach (var item in user.depttIDs)
                        {
                            if (item.isChecked == true)
                            {
                                foreach (var sdept in user.SubDepartmentList)
                                {
                                    if (sdept.IsChecked == true)
                                    {
                                        var sdeptrans = new UmUserSubDepartmentTran();
                                        sdeptrans.UserRefId = user.id;
                                        sdeptrans.DepartmentId = item.id;
                                        sdeptrans.SubDepartmentId = sdept.Id;
                                        sdeptrans.Status = true;
                                        sdeptrans.CreatedBy = loginUserId;
                                        sdeptrans.CreatedDate = DateTime.Now;
                                        dbContext.UmUserSubDepartmentTrans.Add(sdeptrans);
                                    }
                                }
                            }
                        }

                        dbContext.SaveChanges();
                        //send mail to new user 
                        var body = "Dear User,<br><br>You are successfully registered with mynoida.in. Your user name is: " + user.empID + " and password is " + password + ".<br><br>Regards,<br>http://mynoida.in";
                        EmailHelper emailHelper = new EmailHelper();
                        emailHelper.Send(user.email, "Registration Completed", body);

                        //send message to new user on mobile
                        //var msg = "Dear User, You are successfully registered with mynoida.in. Your user name is: " + user.empID + " and password is " + password + ". Regards,http://mynoida.in";
                        var msg = string.Format(NAMessages.SendCredentials, user.empID, password);
                        //SMSSend(user.mobile.ToString(), msg);
                        ApplicationHelper.SendSMS(user.mobile.ToString(), msg);
                        flag = true;
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// Fetches User Details from DB based on id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UsersModel GetUserDetailsById(int id)
        {
            var user = new UsersModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                user = (from u in dbContext.UmUserMasters
                        where u.UserRefId == id
                        select new UsersModel
                            {
                                email = u.Email,
                                firstName = u.FirstName,
                                middleName = u.MiddleName,
                                lastName = u.LastName,
                                mobile = u.Mobile,
                                empID = u.UserName,
                                id = id,
                                createdBy = dbContext.UmUserMasters.Where(p => p.UserRefId.ToString() == u.CreatedBy).Select(p => p.FirstName + " " + p.MiddleName + " " + p.LastName).FirstOrDefault(),
                            }).FirstOrDefault();
                user.depttIDs = (from d in dbContext.UmUserDepartmentTrans
                                 where d.UserRefId == id
                                 select new CheckBoxListItem
                                 {
                                     id = dbContext.UmDepartmentMasters.Where(depMas => depMas.DepartmentId == d.DepartmentId).Select(mas => mas.DepartmentId).FirstOrDefault(),
                                     display = dbContext.UmDepartmentMasters.Where(depMas => depMas.DepartmentId == d.DepartmentId).Select(mas => mas.DepartmentName).FirstOrDefault(),
                                     isChecked = true
                                 }).ToList();
                var subdepts = (from sdept in dbContext.UmUserSubDepartmentTrans
                                where sdept.UserRefId == id
                                select new CheckBoxViewModel
                                {
                                    Id = dbContext.DepartmentSubMsts.FirstOrDefault(d => d.SubDepartmentId == sdept.SubDepartmentId).SubDepartmentId,
                                    CheckBoxName = dbContext.DepartmentSubMsts.FirstOrDefault(d => d.SubDepartmentId == sdept.SubDepartmentId).SubDepartment,
                                    IsChecked = true
                                }).ToList();
                if (subdepts == null || subdepts.Count==0)
                {
                    user.SubDepartmentList = new List<CheckBoxViewModel> { new CheckBoxViewModel { Id = 1, CheckBoxId = 1, CheckBoxName = "Property", IsChecked = false }, new CheckBoxViewModel { Id = 2, CheckBoxId = 2, CheckBoxName = "Account", IsChecked = false } };
                }
                else
                {
                    user.SubDepartmentList = subdepts;
                }
            }

            return user;
        }

        /// <summary>
        /// Fetches all the Departments from DB.
        /// </summary>
        /// <returns></returns>
        public List<CheckBoxListItem> GetAllDeptts()
        {
            var lst = new List<CheckBoxListItem>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lst = (from d in dbContext.UmDepartmentMasters
                       where d.Status == true
                       select new CheckBoxListItem
                       {
                           id = d.DepartmentId,
                           display = d.DepartmentName,
                           isChecked = false
                       }).ToList();
            }
            return lst;
        }

        public List<UsersModel> GetAllUsers(DataSourceRequest request, string roleType, int loginUserId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var users = new List<UsersModel>();

                var innerQuery = from m in dbContext.UmUserMasterRoles join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId where r.RoleType.ToLower() == Constants.SuperAdmin.ToLower() select m.UserRefId;
                users = (from u in dbContext.UmUserMasters
                         join m in dbContext.UmUserMasterRoles on u.UserRefId equals m.UserRefId
                         join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId
                         where !innerQuery.Contains(u.UserRefId) && u.UserRefId != loginUserId //&& r.RoleType == "U"
                         orderby u.UserRefId descending//&& r.RoleType.ToLower() != Constants.SuperAdmin.ToLower()
                         select new UsersModel
                         {
                             id = u.UserRefId,
                             email = u.Email,
                             firstName = u.FirstName,
                             middleName = u.MiddleName,
                             lastName = u.LastName,
                             mobile = u.Mobile,
                             empID = u.UserName,
                             roleName = r.RoleName,
                             roleId = r.RoleId,
                             //applicationID = 
                             isActive = u.IsActive.Value,
                             lstDeptt = (from ma in dbContext.UmDepartmentMasters join d in dbContext.UmUserDepartmentTrans on ma.DepartmentId equals d.DepartmentId join us in dbContext.UmUserMasters on d.UserRefId equals us.UserRefId where us.UserRefId == u.UserRefId select ma.DepartmentName).ToList(),
                             fullName = u.FirstName + " " + u.MiddleName + " " + u.LastName
                         }).ToList();

                foreach (var us in users)
                {
                    if (us.lstDeptt.Count > 0)
                    {
                        foreach (var dept in us.lstDeptt)
                        {
                            us.strDeptts = us.strDeptts + dept + ",";
                        }
                        us.strDeptts = us.strDeptts.TrimEnd(',');
                    }
                }
                return users;
            }
        }
        /// <summary>
        /// Used for Locking/Unlocking a User
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public bool LockUnlockToggle(int id, bool isActive)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.UmUserMasters.Where(u => u.UserRefId == id).FirstOrDefault();
                if (user != null)
                {
                    if (isActive == false)
                    {
                        user.IsActive = true;
                    }
                    else
                        user.IsActive = false;
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }
        /// <summary>
        /// used for locking/unlocking role in an application
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public bool LockUnlockRoleToggle(int id, bool isActive, int roleId, int applicationID)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                // var user = dbContext.UmUserMasters.Where(u => u.UserRefId == id).FirstOrDefault();
                var user = dbContext.UmUserMasterRoles.Where(u => u.UserRefId == id && u.RoleId == roleId).FirstOrDefault();
                if (user != null)
                {
                    if (isActive == false)
                    {
                        user.isActive = true;
                    }
                    else
                        user.isActive = false;
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }
        /// <summary>
        /// Used for checking whether a given Username already exists or not
        /// </summary>
        /// <param name="empID"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckUsernameDuplicacy(string empID, int id)
        {
            var isExist = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var rec = new UmUserMaster();
                if (id != 0)
                {
                    rec = dbContext.UmUserMasters.Where(i => i.UserName.ToLower() == empID.ToLower() && i.UserRefId != id).FirstOrDefault();
                }
                else
                {
                    rec = dbContext.UmUserMasters.Where(i => i.UserName.ToLower() == empID.ToLower()).FirstOrDefault();
                }
                if (rec != null)
                {
                    isExist = true;
                }
            }
            return isExist;
        }

        /// <summary>
        /// Used for checking whether a given Mobile No. already exists or not
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckMobileNoDuplicacy(string mobile, int id)
        {
            var isExist = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var rec = new UmUserMaster();
                if (id != 0)
                {
                    rec = dbContext.UmUserMasters.Where(i => i.Mobile == mobile && i.UserRefId != id).FirstOrDefault();
                }
                else
                {
                    rec = dbContext.UmUserMasters.Where(i => i.Mobile == mobile).FirstOrDefault();
                }
                if (rec != null)
                {
                    isExist = true;
                }
            }
            return isExist;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="empID"></param>
        /// <param name="loginUserId"></param>
        /// <returns></returns>
        public List<UsersModel> SearchUsers(DataSourceRequest request, string empID, int loginUserId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var innerQuery = from m in dbContext.UmUserMasterRoles join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId where r.RoleType.ToLower() == Constants.SuperAdmin.ToLower() select m.UserRefId;
                var users = (from u in dbContext.UmUserMasters
                             //join m in dbContext.UmUserMasterRoles on u.UserRefId equals m.UserRefId
                             //join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId
                             //where !innerQuery.Contains(u.UserRefId) && u.UserName.ToLower().Contains(empID.ToLower()) && u.UserRefId != loginUserId
                             where !innerQuery.Contains(u.UserRefId) && u.UserName.ToLower() == empID.ToLower() && u.UserRefId != loginUserId
                             select new UsersModel
                             {
                                 id = u.UserRefId,
                                 email = u.Email,
                                 firstName = u.FirstName,
                                 middleName = u.MiddleName,
                                 lastName = u.LastName,
                                 fullName = u.FirstName + " " + u.MiddleName + " " + u.LastName,
                                 mobile = u.Mobile,
                                 empID = u.UserName,
                                 createdBy = u.CreatedBy,
                                 lastModified = (u.ModifiedDate == null ? u.CreatedDate : u.ModifiedDate),
                                 isActive = u.IsActive.Value,
                                 lstDeptt = (from mas in dbContext.UmDepartmentMasters join d in dbContext.UmUserDepartmentTrans on mas.DepartmentId equals d.DepartmentId join us in dbContext.UmUserMasters on d.UserRefId equals us.UserRefId where us.UserRefId == u.UserRefId select mas.DepartmentName).ToList()
                             }).ToList();
                foreach (var us in users)
                {
                    foreach (var dept in us.lstDeptt)
                    {
                        us.strDeptts = us.strDeptts + dept + ",";
                    }
                    us.strDeptts = us.strDeptts.TrimEnd(',');
                }
                return users;
            }
        }


        #region GetUserAll Keshav 14 Sep 2018
        public List<UsersModel> GetAllUser()
        {
            var pimsUsers = new List<UsersModel>();
            try
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    pimsUsers = (from u in dbContext.UmUserMasters //where u.UserName!= userName
                                 select new UsersModel
                                 {
                                     id = u.UserRefId,
                                     fullName = u.FirstName + " " + u.MiddleName + " " + u.LastName
                                     ,
                                     firstName = u.UserName

                                 }).ToList();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pimsUsers;
        }


        #endregion
        /// <summary>
        /// Used for Mapping a User to a Role
        /// </summary>
        /// <param name="id">UserRefID</param>
        /// <param name="roleID">Role ID</param>
        /// <param name="loginUser">For Created_By and/or Modified_By</param>
        /// <returns>0 -> User is already attached to a Role (not Admin type) for the given Application; 1-> User added successfully; 2 -> User is already an Admin for the given Application</returns>
        public int MapUserToRole(int id, int roleID, int loginUser)
        {
            int result = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                var appID = dbContext.UmRoleAppTrans.Where(app => app.RoleId == roleID).Select(app => app.ApplicationId).FirstOrDefault();
                if (appID != null)
                {
                    var existingRole = (from us in dbContext.UmUserMasterRoles
                                        join ro in dbContext.UmRoleAppTrans on us.RoleId equals ro.RoleId
                                        join rm in dbContext.UmRoleMasters on ro.RoleId equals rm.RoleId
                                        where ro.ApplicationId == appID && us.UserRefId == id
                                        select new
                                        {
                                            us = us,
                                            roleType = rm.RoleType
                                        }).FirstOrDefault();
                    if (existingRole != null)
                    {
                        if (existingRole.roleType.ToLower() == Constants.Admin.ToLower())
                        {
                            result = 2;
                        }
                        //result = 0; //User is already attached to a Role for the given Application.
                    }
                    else
                    {
                        var mapping = new UmUserMasterRole();
                        mapping.UserRefId = id;
                        mapping.RoleId = roleID;
                        mapping.CreatedBy = loginUser.ToString();
                        mapping.CreatedDate = DateTime.Now;
                        mapping.isActive = true;
                        dbContext.UmUserMasterRoles.Add(mapping);
                        dbContext.SaveChanges();
                        result = 1;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Grid read function for Remove Grid on UserRole Mapping page
        /// </summary>
        /// <param name="request"></param>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public List<UsersModel> GetRemoveUsers(DataSourceRequest request, int roleID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var users = (from u in dbContext.UmUserMasters
                             join r in dbContext.UmUserMasterRoles on u.UserRefId equals r.UserRefId
                             where r.RoleId == roleID
                             select new UsersModel
                             {
                                 id = u.UserRefId,
                                 email = u.Email,
                                 firstName = u.FirstName,
                                 middleName = u.MiddleName,
                                 lastName = u.LastName,
                                 mobile = u.Mobile,
                                 empID = u.UserName,
                                 createdBy = u.CreatedBy,
                                 lastModified = (u.ModifiedDate == null ? u.CreatedDate : u.ModifiedDate),
                                 isActive = u.IsActive.Value,
                                 lstDeptt = (from m in dbContext.UmDepartmentMasters join d in dbContext.UmUserDepartmentTrans on m.DepartmentId equals d.DepartmentId join us in dbContext.UmUserMasters on d.UserRefId equals us.UserRefId where us.UserRefId == u.UserRefId select m.DepartmentName).ToList()
                             }).ToList();
                foreach (var us in users)
                {
                    foreach (var dept in us.lstDeptt)
                    {
                        us.strDeptts = us.strDeptts + dept + ",";
                    }
                    us.strDeptts = us.strDeptts.TrimEnd(',');
                }
                return users;
            }
        }

        /// <summary>
        /// Removes User association with a Role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveUser(int userId, int roleId, int applicationId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var mapping = dbContext.UmUserMasterRoles.Where(u => u.UserRefId == userId && u.RoleId == roleId).FirstOrDefault();
                var rolapp = dbContext.UmRoleAppTrans.Where(r => r.RoleId == roleId && r.ApplicationId == applicationId).FirstOrDefault();
                if (mapping != null)
                {
                    dbContext.UmUserMasterRoles.Remove(mapping);
                    //dbContext.UmRoleAppTrans.Remove(rolapp);
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }

        //Private method for creating random Password
        private string CreatePassword()
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int length = 8;
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public Boolean sendOTPtoUser(string mobNo, int otp)
        {
            var result = false;
            try
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    //var phoneNo = dbContext.view_property.Where(us => us.PRDVREGISTRATION_ID.ToLower() == id.ToLower()).Select(us => us.mobile_no).FirstOrDefault();
                    //phoneNo = "8010079321";//Hard coded for Testing purposes
                    if (mobNo != null)
                    {
                        //var msg = "Dear User, Your OTP is " + otp + " DO NOT disclose this to anyone by any means.";
                        var msg = string.Format(NAMessages.OTPSend, otp);
                        //SMSSend(mobNo, msg);
                        ApplicationHelper.SendSMS(mobNo, msg);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private void SMSSend(string mobileNo, string msg)
        {
            WebClient client = new WebClient();
            //string baseurl = ConfigurationManager.AppSettings["SMSsend"].ToString() + ConfigurationManager.AppSettings["SMSUsername"].ToString() + "&password=" + ConfigurationManager.AppSettings["SMSPassword"].ToString() + "&sendername=" + "NETSMS" + "&mobileno=" + mobileNo + "&message=" + msg;
            string baseurl = ConfigurationManager.AppSettings["SMSApiUrl"].ToString() + "ApiKey=" + ConfigurationManager.AppSettings["SMSApiKey"].ToString() + "&ClientId=" + ConfigurationManager.AppSettings["SMSClientId"].ToString() + "&SenderId=" + ConfigurationManager.AppSettings["SMSSenderId"].ToString() + "&Message=" + msg + "&MobileNumbers=91" + mobileNo + "&Is_Unicode=" + ConfigurationManager.AppSettings["SMSIsUnicode"].ToString() + "&Is_Flash=" + ConfigurationManager.AppSettings["SMSIsFlash"].ToString();
            Stream data = client.OpenRead(baseurl);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            data.Close();
            reader.Close();
        }

        public bool ChangePassword(string userName, string email, string newPassword)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.UmUserMasters.FirstOrDefault(cond => cond.UserName.ToLower() == userName.ToLower() && cond.Email == email);
                if (user != null)
                {
                    var password = newPassword.ToMD5HashForPassword();
                    user.Password = password;
                    user.ModifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    if (user.Email != null)
                    {
                        var body = "Dear User, You have successfully changed your password. Your new password is " + password + " Regards, http://mynoida.in";
                        EmailHelper emailHelper = new EmailHelper();
                        emailHelper.Send(user.Email, "You have successfully changed your password.", body);
                    }

                    if (user.Mobile != null)
                    {
                        //var msg = "Dear User, You have successfully changed your password. Your new password is " + password + " Regards, http://mynoida.in";
                        var msg = string.Format(NAMessages.PasswordChange, password);
                        //SMSSend(user.Mobile, msg);
                        ApplicationHelper.SendSMS(user.Mobile, msg);
                    }

                    flag = true;
                }
            }
            return flag;
        }

        public DataSourceResult GetEntireUsers(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstusers = (from u in dbContext.ViewAllUserApplicationRoleDetails
                                where (u.RoleName != Constants.SuperAdmin)
                                select new UsersModel
                                {
                                    applicationName = u.ApplicationName,
                                    roleName = u.RoleName,
                                    id = u.UserRefId,
                                    firstName = u.FirstName,
                                    lastName = u.LastName,
                                    mobile = u.Mobile,
                                    strDeptts = u.DepartmentName,
                                    email = u.Email,
                                    empID = u.UserRefId.ToString(),
                                    isActive = u.IsActive.Value
                                }
                    );
                return lstusers.ToDataSourceResult(request);
            }
        }


        public List<AdminRoleDetail> GetAdminRoleDetails(int userId)
        {
            List<AdminRoleDetail> roleDetail = new List<AdminRoleDetail>();
            using (var dbContext = new NoidaPMSEntities())
            {
                roleDetail = (from rmt in dbContext.UmRoleAppTrans
                              join roles in dbContext.UmRoleMasters on rmt.RoleId equals roles.RoleId
                              join apps in dbContext.UmApplicationMasters on rmt.ApplicationId equals apps.ApplicationId
                              join umr in dbContext.UmUserMasterRoles on rmt.RoleId equals umr.RoleId
                              join users in dbContext.UmUserMasters on umr.UserRefId equals users.UserRefId
                              where umr.UserRefId == userId
                              select new AdminRoleDetail
                              {
                                  RoleId = roles.RoleId,
                                  RoleType = roles.RoleType,
                                  RoleName = roles.RoleName,
                                  ApplicationId = apps.ApplicationId,
                                  ApplicationName = apps.ApplicationName,
                                  createdBy = users.FirstName + " " + users.MiddleName + " " + users.LastName
                              }).ToList();
            }
            return roleDetail;
        }

        /// <summary>
        /// Get all users for admin in an application
        /// </summary>
        /// <returns></returns>
        public List<UsersModel> GetApplicationUsersListForAdmin()
        {
            List<UsersModel> usersList = new List<UsersModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                usersList = (from u in dbContext.ViewAllUserApplicationRoleDetails
                             where (u.RoleName.ToLower() != Constants.SuperAdmin.ToLower())
                             select new UsersModel
                             {
                                 applicationName = u.ApplicationName,
                                 roleName = u.RoleName,
                                 id = u.UserRefId,
                                 firstName = u.FirstName,
                                 lastName = u.LastName,
                                 mobile = u.Mobile,
                                 strDeptts = u.DepartmentName,
                                 email = u.Email,
                                 empID = u.UserName,
                                 isActive = u.IsActive.Value
                             }
                    ).ToList();
                return usersList;
            }
        }



        public List<UsersModel> GetApplicationUsersListForAdmin(List<ApplicationMaster> lstapplication)
        {
            List<UsersModel> usersList = new List<UsersModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                foreach (var appls in lstapplication)
                {
                    //List<UsersModel> usersList1 = new List<UsersModel>();
                    var applicationId = appls.ApplicationId;
                    var innerQuery = from m in dbContext.UmUserMasterRoles join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId where r.RoleType.ToLower() == Constants.SuperAdmin.ToLower() select m.UserRefId;
                    var usersList1 = (from um in dbContext.UmUserMasters
                                      join umr in dbContext.UmUserMasterRoles on um.UserRefId equals umr.UserRefId
                                      join rat in dbContext.UmRoleAppTrans on umr.RoleId equals rat.RoleId
                                      join roles in dbContext.UmRoleMasters on rat.RoleId equals roles.RoleId
                                      join apps in dbContext.UmApplicationMasters on rat.ApplicationId equals apps.ApplicationId
                                      where !innerQuery.Contains(um.UserRefId) && rat.ApplicationId == applicationId && roles.RoleType.ToLower() != Constants.Admin.ToLower()

                                      //where (rat.ApplicationId == applicationId && roles.RoleName.ToLower() != Constants.SuperAdmin.ToLower())
                                      select new UsersModel
                                      {
                                          applicationID = apps.ApplicationId,
                                          applicationName = apps.ApplicationName,
                                          roleName = roles.RoleName,
                                          roleId = roles.RoleId,
                                          id = um.UserRefId,
                                          firstName = um.FirstName,
                                          lastName = um.LastName,
                                          lstDeptt = (from udt in dbContext.UmUserDepartmentTrans join dept in dbContext.UmDepartmentMasters on udt.DepartmentId equals dept.DepartmentId where udt.UserRefId == um.UserRefId select dept.DepartmentName).ToList(),
                                          mobile = um.Mobile,
                                          email = um.Email,
                                          //empID = um.UserRefId.ToString(),
                                          empID = um.UserName,
                                          isActive = umr.isActive.Value
                                      });

                    usersList.AddRange(usersList1);
                }
                foreach (var us in usersList)
                {
                    if (us.lstDeptt.Count > 0)
                    {
                        foreach (var dept in us.lstDeptt)
                        {
                            us.strDeptts = us.strDeptts + dept + ",";
                        }
                        us.strDeptts = us.strDeptts.TrimEnd(',');
                    }
                }

                return usersList;
            }
        }






        public List<UsersModel> GetApplicationUsers()
        {
            List<UsersModel> usersList = new List<UsersModel>();
            int userid = userInfo.UserID;
            using (var dbContext = new NoidaPMSEntities())
            {
                var selectSA = from m in dbContext.UmUserMasterRoles join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId where r.RoleType.ToLower() == Constants.SuperAdmin.ToLower() select m.UserRefId;
                usersList = (from u in dbContext.UmUserMasters
                             where !selectSA.Contains(u.UserRefId) && u.UserRefId != userid
                             select new UsersModel
                             {
                                 id = u.UserRefId,
                                 empID = u.UserName
                             }).Distinct().ToList();

                return usersList;
            }
        }
        public List<UsersModel> GetAllUsersSuper(DataSourceRequest request, string roleType, int loginUserId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var users = new List<UsersModel>();

                var innerQuery = from m in dbContext.UmUserMasterRoles join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId where r.RoleType.ToLower() == Constants.SuperAdmin.ToLower() select m.UserRefId;
                users = (from u in dbContext.UmUserMasters
                         //join m in dbContext.UmUserMasterRoles on u.UserRefId equals m.UserRefId
                         //join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId
                         where !innerQuery.Contains(u.UserRefId) && u.UserRefId != loginUserId//&& r.RoleType.ToLower() != Constants.SuperAdmin.ToLower()
                         orderby u.UserRefId descending
                         select new UsersModel
                         {
                             id = u.UserRefId,
                             email = u.Email,
                             firstName = u.FirstName,
                             middleName = u.MiddleName,
                             lastName = u.LastName,
                             mobile = u.Mobile,
                             empID = u.UserName,
                             //createdBy = dbContext.UmUserMasters.Where(p => p.UserRefId.ToString() == u.CreatedBy).Select(p => p.FirstName + " " + p.MiddleName + " " + p.LastName).FirstOrDefault(),
                             //lastModified = (u.ModifiedDate == null ? u.CreatedDate : u.ModifiedDate),
                             isActive = u.IsActive.Value,
                             lstDeptt = (from ma in dbContext.UmDepartmentMasters join d in dbContext.UmUserDepartmentTrans on ma.DepartmentId equals d.DepartmentId join us in dbContext.UmUserMasters on d.UserRefId equals us.UserRefId where us.UserRefId == u.UserRefId select ma.DepartmentName).ToList(),
                             fullName = u.FirstName + " " + u.MiddleName + " " + u.LastName
                         }).ToList();

                foreach (var us in users)
                {
                    if (us.lstDeptt.Count > 0)
                    {
                        foreach (var dept in us.lstDeptt)
                        {
                            us.strDeptts = us.strDeptts + dept + ",";
                        }
                        us.strDeptts = us.strDeptts.TrimEnd(',');
                    }
                }
                return users;
            }
        }

        /// <summary>
        /// check email duplicacy
        /// </summary>
        /// <param name="email"></param>
        /// <param name="refID"></param>
        /// <returns></returns>
        public bool CheckEmailDuplicacy(string email, int refID)
        {
            var isExist = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var rec = new UmUserMaster();
                if (refID != 0)
                {
                    rec = dbContext.UmUserMasters.Where(i => i.Email == email && i.UserRefId != refID).FirstOrDefault();
                }
                else
                {
                    rec = dbContext.UmUserMasters.Where(i => i.Email == email).FirstOrDefault();
                }
                if (rec != null)
                {
                    isExist = true;
                }
            }
            return isExist;
        }


        public List<AuditActionModel> GetActionAuditTrail()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var auditDetails = (from audit in dbContext.Audits

                                    select new AuditActionModel
                                    {
                                        AuditID = audit.AuditID,
                                        FormName = audit.FormName,
                                        FieldName = audit.FieldName,
                                        ActionType = audit.Type,
                                        NewValue = audit.NewValue,
                                        OldValue = audit.OldValue,
                                        TableName = audit.TableName,
                                        UpdateDate = audit.UpdateDate,
                                        UserName = dbContext.UmUserMasters.Where(u => u.UserRefId.ToString() == audit.UserName).Select(u => u.UserName).FirstOrDefault()
                                    });
                var distinct = auditDetails.GroupBy(ad => ad.FormName).Select(y => y.FirstOrDefault()).OrderByDescending(o => o.UpdateDate).ToList();//.GroupBy(g=>g.FormName).First()
                return distinct;

            }
        }


        public List<AuditActionModel> GetActionAuditDetailByDate(DateTime startDate, DateTime endDate)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var auditDetails = (from audit in dbContext.Audits
                                    where audit.UpdateDate >= startDate && audit.UpdateDate <= endDate
                                    select new AuditActionModel
                                    {
                                        AuditID = audit.AuditID,
                                        FormName = audit.FormName,
                                        FieldName = audit.FieldName,
                                        ActionType = audit.Type,
                                        NewValue = audit.NewValue,
                                        OldValue = audit.OldValue,
                                        TableName = audit.TableName,
                                        UpdateDate = audit.UpdateDate,
                                        UserName = dbContext.UmUserMasters.Where(u => u.UserRefId.ToString() == audit.UserName).Select(u => u.UserName).FirstOrDefault()
                                    }).OrderByDescending(o => o.UpdateDate).ToList();
                return auditDetails;
            }


        }



        public List<AuditActionModel> GetAuditTrailActionGroupByModuleName(string moduleName)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var auditDetails = (from audit in dbContext.Audits
                                    where audit.FormName == moduleName
                                    select new AuditActionModel
                                    {
                                        AuditID = audit.AuditID,
                                        FormName = audit.FormName,
                                        FieldName = audit.FieldName,
                                        ActionType = audit.Type,
                                        NewValue = audit.NewValue,
                                        OldValue = audit.OldValue,
                                        TableName = audit.TableName,
                                        UpdateDate = audit.UpdateDate,
                                        UserName = dbContext.UmUserMasters.Where(u => u.UserRefId.ToString() == audit.UserName).Select(u => u.UserName).FirstOrDefault()
                                    }).OrderByDescending(o => o.UpdateDate).ToList();
                return auditDetails;
            }
        }


        public List<DDList> GetAuditModuleName()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var modules = (from module in dbContext.Audits
                               select new DDList
                               {
                                   text = module.FormName
                               }).Distinct().ToList();
                return modules;
            }
        }

        public List<DDList> GetAuditActionName(string moduleName)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var actions = (from act in dbContext.Audits
                               //where act.FormName == moduleName
                               select new DDList
                               {
                                   text = act.FieldName
                               }).Distinct().ToList();
                return actions;
            }
        }

        public List<DDList> GetAuditUserName(string moduleName, string actionName)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var usrs = (from usr in dbContext.Audits
                            //where usr.FormName == moduleName && usr.FieldName == actionName
                            select new DDList
                            {
                                id = dbContext.UmUserMasters.Where(u => u.UserRefId.ToString() == usr.UserName).Select(u => u.UserRefId).FirstOrDefault(),
                                text = dbContext.UmUserMasters.Where(u => u.UserRefId.ToString() == usr.UserName).Select(u => u.UserName).FirstOrDefault()
                            }).Distinct().ToList();
                return usrs;
            }
        }


        public List<AuditActionModel> GetAuditTrailByAdvanceSearch(string moduleName, string actionName, string userName, DateTime? startDate, DateTime? endDate)
        {

            using (var dbContext = new NoidaPMSEntities())
            {
                var searchedAudits = (from audit in dbContext.Audits
                                      where (moduleName == "" || audit.FormName == moduleName)
                                          && (actionName == "" || audit.FieldName == actionName)
                                          && (userName == "" || audit.UserName == userName)
                                          && (startDate == null || audit.UpdateDate >= startDate)
                                          && (endDate == null || audit.UpdateDate <= endDate)

                                      select new AuditActionModel
                                      {
                                          AuditID = audit.AuditID,
                                          FormName = audit.FormName,
                                          FieldName = audit.FieldName,
                                          ActionType = audit.Type,
                                          NewValue = audit.NewValue,
                                          OldValue = audit.OldValue,
                                          TableName = audit.TableName,
                                          UpdateDate = audit.UpdateDate,
                                          UserName = dbContext.UmUserMasters.Where(u => u.UserRefId.ToString() == audit.UserName).Select(u => u.UserName).FirstOrDefault()
                                      }).ToList();
                return searchedAudits;
            }

        }


        public List<DynamicDataModel> GetRidOfPropertyTransaction()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var transactions = (from trans in dbContext.PropertyTransactions
                                    select new DynamicDataModel
                                    {
                                        Value = trans.Rid.Value
                                    }).ToList();
                return transactions;
            }
        }

        public List<DynamicDataModel> GetServiceNameOfPropertyTransaction()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var transactions = (from trans in dbContext.PropertyTransactions
                                    select new DynamicDataModel
                                    {
                                        Name = trans.Service_Name
                                    }).ToList();
                return transactions;
            }
        }

        public List<DynamicDataModel> GetUserOfPropertyTransaction()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var transactions = (from trans in dbContext.PropertyTransactions
                                    join usr in dbContext.UmUserMasters on trans.User_Identity equals usr.UserRefId
                                    select new DynamicDataModel
                                    {
                                        Name = usr.UserName,
                                        Value = trans.User_Identity.Value
                                    }).ToList();
                return transactions;
            }
        }

        public List<PropertyTransactionAudit> GetTransactionAuditHistory(int? rid, string serviceName, int? user, DateTime? startDate, DateTime? endDate)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var transactions = (from audit in dbContext.PropertyTransactions
                                    where (rid == null || audit.Rid == rid)
                                        && (serviceName == "" || audit.Service_Name == serviceName)
                                        && (user == null || audit.User_Identity == user)
                                        && (startDate == null || audit.Service_Date >= startDate)
                                        && (endDate == null || audit.Service_Date <= endDate)

                                    select new PropertyTransactionAudit
                                      {
                                          Id = audit.Id,
                                          Rid = audit.Rid,
                                          ServiceName = audit.Service_Name,
                                          ServiceDate = audit.Service_Date,
                                          UserIdentity = audit.User_Identity,
                                          CreatedBy = audit.Created_By,
                                          CreatedDate = audit.Created_Date,
                                          PropertyNumber = audit.Property_Number,
                                          UserName = dbContext.UmUserMasters.Where(u => u.UserRefId == audit.User_Identity).Select(u => u.UserName).FirstOrDefault()
                                      }).ToList();
                return transactions;
            }
        }


        public List<UsersModel> GetMappedUsersForApplication(int applicationId, int? roleId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var users = new List<UsersModel>();

                var innerQuery = from m in dbContext.UmUserMasterRoles join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId where r.RoleType.ToLower() == Constants.SuperAdmin.ToLower() select m.UserRefId;
                users = (from u in dbContext.UmUserMasters
                         join m in dbContext.UmUserMasterRoles on u.UserRefId equals m.UserRefId
                         join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId
                         join ra in dbContext.UmRoleAppTrans on r.RoleId equals ra.RoleId
                         where !innerQuery.Contains(u.UserRefId) && u.UserRefId != userInfo.UserID && r.RoleType != "A"
                                && (roleId == null || ra.RoleId == roleId)
                                && (applicationId == null || ra.ApplicationId == applicationId)
                         orderby u.UserRefId descending
                         select new UsersModel
                         {
                             id = u.UserRefId,
                             email = u.Email,
                             firstName = u.FirstName,
                             middleName = u.MiddleName,
                             lastName = u.LastName,
                             mobile = u.Mobile,
                             empID = u.UserName,
                             roleName = r.RoleName,
                             roleId = r.RoleId,
                             applicationID = ra.ApplicationId.Value,
                             isActive = u.IsActive.Value,
                             lstDeptt = (from ma in dbContext.UmDepartmentMasters join d in dbContext.UmUserDepartmentTrans on ma.DepartmentId equals d.DepartmentId join us in dbContext.UmUserMasters on d.UserRefId equals us.UserRefId where us.UserRefId == u.UserRefId select ma.DepartmentName).ToList(),
                             fullName = u.FirstName + " " + u.MiddleName + " " + u.LastName
                         }).ToList();

                foreach (var us in users)
                {
                    if (us.lstDeptt.Count > 0)
                    {
                        foreach (var dept in us.lstDeptt)
                        {
                            us.strDeptts = us.strDeptts + dept + ",";
                        }
                        us.strDeptts = us.strDeptts.TrimEnd(',');
                    }
                }
                return users;
            }
        }



        public List<UsersModel> GetUserListForSuperAdmin(int? applicationId, int? roleId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var users = new List<UsersModel>();

                var innerQuery = from m in dbContext.UmUserMasterRoles join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId where r.RoleType.ToLower() == Constants.SuperAdmin.ToLower() select m.UserRefId;
                users = (from u in dbContext.UmUserMasters
                         join m in dbContext.UmUserMasterRoles on u.UserRefId equals m.UserRefId
                         join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId
                         join ra in dbContext.UmRoleAppTrans on r.RoleId equals ra.RoleId
                         where !innerQuery.Contains(u.UserRefId) && u.UserRefId != userInfo.UserID
                                && (roleId == null || ra.RoleId == roleId)
                                && (applicationId == null || ra.ApplicationId == applicationId)
                         orderby u.UserRefId descending
                         select new UsersModel
                         {
                             id = u.UserRefId,
                             email = u.Email,
                             firstName = u.FirstName,
                             middleName = u.MiddleName,
                             lastName = u.LastName,
                             mobile = u.Mobile,
                             empID = u.UserName,
                             roleName = r.RoleName,
                             roleId = r.RoleId,
                             applicationID = ra.ApplicationId.Value,
                             isActive = u.IsActive.Value,
                             lstDeptt = (from ma in dbContext.UmDepartmentMasters join d in dbContext.UmUserDepartmentTrans on ma.DepartmentId equals d.DepartmentId join us in dbContext.UmUserMasters on d.UserRefId equals us.UserRefId where us.UserRefId == u.UserRefId select ma.DepartmentName).ToList(),
                             fullName = u.FirstName + " " + u.MiddleName + " " + u.LastName
                         }).ToList();

                foreach (var us in users)
                {
                    if (us.lstDeptt.Count > 0)
                    {
                        foreach (var dept in us.lstDeptt)
                        {
                            us.strDeptts = us.strDeptts + dept + ",";
                        }
                        us.strDeptts = us.strDeptts.TrimEnd(',');
                    }
                }
                return users;
            }
        }


        public DataSourceResult GetPropertyCustomersList(DataSourceRequest request, UserViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                string filePath = ConfigurationManager.AppSettings["DocumentFilesPath"].ToString();
                var list = (from user in dbContext.CustomerMsts
                            from role in dbContext.UmRoleMasters.Where(r => r.RoleId == user.RoleId).DefaultIfEmpty()
                            where (model.IsActive == null || user.IsActive == model.IsActive)
                            && (model.UserName == null || user.UserName == model.UserName)
                            && (model.DepartmentId == null || user.DepartmentId == model.DepartmentId)
                            select new UserViewModel
                            {
                                Id = user.Id,
                                UserName = user.UserName,
                                Pasword = user.Password,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                MobileNo = user.MobileNo,
                                PropertyId = user.PropertyId,
                                CreatedOn = user.CreatedDate,
                                //CreatedBy = user.CreatedBy,
                                //ModifiedBy = user.ModifiedBy,
                                ModifiedOn = user.ModifiedDate,
                                RoleId = user.RoleId,
                                RoleName = role.RoleName,
                                LastPasswordChangeDate = user.PasswordChangeDate,
                                FullName = user.FirstName + " " + user.LastName,
                                CustomerIdFileName = user.IdFileName == null ? string.Empty : filePath + user.PropertyId + "/" + user.IdFileName,
                                CustomerIdFileType = user.IdFileType,
                                AuthorityLetter = user.PropertyFileName == null ? string.Empty : filePath + user.PropertyId + "/" + user.PropertyFileName,
                                AuthorityLetterType = user.PropertyFileType,
                                DepartmentId = user.DepartmentId,
                                Department = (user.DepartmentId == null || user.DepartmentId <= 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(x => x.departmentId == user.DepartmentId).departmentName,
                                Remarks = user.Remarks,
                                Email = user.Email,
                                StatusId = user.StatusId,
                                Status = user.StatusId == 1 ? true : false,
                                IsApproved = user.StatusId == 1 ? true : false,
                                IsActive = user.IsActive,
                                IsLocked = user.IsLocked,
                                IsFirstTimeActivated = user.IsFirstTimeActivated == null ? false : user.IsFirstTimeActivated,
                                IsRejected = (user.StatusId == null || user.StatusId != NAStatusId.Rejected) ? false : true,
                                PropertyNo = user.Sector + "/" + (string.IsNullOrEmpty(user.Block) ? string.Empty : user.Block + "-") + user.PlotNo,
                                IsIdFileUploaded = string.IsNullOrEmpty(user.IdFileName) ? false : true,
                                IsPropertyFileUploaded = string.IsNullOrEmpty(user.PropertyFileName) ? false : true,
                            });
                return list.ToDataSourceResult(request);
            }
            
        }


        public int UpdateCustomerStatus(UserViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.CustomerMsts.FirstOrDefault(u => u.UserName == model.UserName);
                if (model.ActionType == "Locked")
                {
                    if (user.IsLocked == true) user.IsLocked = false;
                    else user.IsLocked = true;
                    dbContext.SaveChanges();
                    flag = ReturnType.Locked;
                }
                if (model.ActionType == "Password")
                {
                    string newpassword = ApplicationHelper.RandomString(4);
                    string encryptedPassword = newpassword.ToMD5HashForPassword();
                    user.Password = encryptedPassword;
                    dbContext.SaveChanges();
                    string msg = string.Format(NAMessages.PIS_Registration_Activation, model.UserName, newpassword);
                    if (!string.IsNullOrEmpty(user.MobileNo)) ApplicationHelper.SendSMS(user.MobileNo, msg);
                    flag = ReturnType.Updated;
                }
                if (model.ActionType == "Active")
                {
                    if (user.IsActive == true) user.IsActive = false;
                    else user.IsActive = true;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                if (model.ActionType == "Remarks")
                {
                    user.Remarks = user.Remarks == null ? model.Remarks : user.Remarks + "  " + model.Remarks;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                if (model.ActionType == "Approval")
                {
                    //if (user.StatusId == NAStatusId.Rejected) user.StatusId = NAStatusId.Approved;
                    //else user.StatusId = NAStatusId.Rejected;
                    string newpassword = ApplicationHelper.RandomString(4);
                    string encryptedPassword = newpassword.ToMD5HashForPassword();
                    user.StatusId = NAStatusId.Approved;
                    user.Password = encryptedPassword;
                    dbContext.SaveChanges();
                    string msg = string.Format(NAMessages.PIS_Registration_Activation, model.UserName, newpassword);
                    if (!string.IsNullOrEmpty(user.MobileNo)) ApplicationHelper.SendSMS(user.MobileNo, msg);
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }


        public DataSourceResult GetAuditActionDetailByAdvanceSearch(DataSourceRequest request, AuditViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from audit in dbContext.Audits
                            where (model.AuditId == null || audit.AuditID == model.AuditId)
                                && (model.TableName == null || audit.TableName == model.TableName)
                                && (model.FormName == null || audit.FormName == model.FormName)
                                && (model.FieldName == null || audit.FieldName == model.FieldName)
                                && (model.UserId == null || audit.UserName == model.UserId)
                                && (model.StartDate == null || DbFunctions.TruncateTime(audit.UpdateDate) >= DbFunctions.TruncateTime(model.StartDate))
                                && (model.EndDate == null || DbFunctions.TruncateTime(audit.UpdateDate) <= DbFunctions.TruncateTime(model.EndDate))
                            select new AuditViewModel
                            {
                                Id = audit.AuditID,
                                AuditId = audit.AuditID,
                                TableName = audit.TableName,
                                FormName = audit.FormName,
                                KeyField = audit.PrimaryKeyField,
                                KeyValue = audit.PrimaryKeyValue,
                                FieldName = audit.FieldName,
                                ActionType = audit.Type == null ? string.Empty : (audit.Type == "I" ? "Insert" : (audit.Type == "D" ? "Delete" : (audit.Type == "U" ? "Update" : (audit.Type == "S" ? "SMS" : string.Empty)))),
                                NewValue = audit.NewValue,
                                OldValue = audit.OldValue,
                                ActionDate = audit.UpdateDate,
                                UserId = audit.UserName,
                                //UserName = dbContext.UmUserMasters.Where(u => u.UserRefId.ToString() == audit.UserName).Select(u => u.UserName).FirstOrDefault(),
                                UserName = (string.IsNullOrEmpty(audit.UserName) || audit.UserName == "0") ? string.Empty : dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId.ToString() == audit.UserName).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId.ToString() == audit.UserName).MiddleName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId.ToString() == audit.UserName).LastName,
                            });
                return data.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetAuditSearchParameter(DataSourceRequest request, AuditViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "TableName")
                {
                    var list = (from audit in dbContext.Audits
                                where (model.FormName == null || audit.FormName == model.FormName)
                                && (model.FieldName == null || audit.FieldName == model.FieldName)
                                && (model.UserId == null || audit.UserName == model.UserId)
                                select new DropdownViewModel
                                {
                                    Id = 0,
                                    Text = audit.TableName
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }
                else if (model.ActionType == "FormName")
                {
                    var list = (from audit in dbContext.Audits
                                where (model.TableName == null || audit.TableName == model.TableName)
                                && (model.FieldName == null || audit.FieldName == model.FieldName)
                                && (model.UserId == null || audit.UserName == model.UserId)
                                select new DropdownViewModel
                                {
                                    Id = 0,
                                    Text = audit.FormName
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }
                else if (model.ActionType == "FieldName")
                {
                    var list = (from audit in dbContext.Audits
                                where (model.TableName == null || audit.TableName == model.TableName)
                                && (model.FormName == null || audit.FormName == model.FormName)
                                && (model.UserId == null || audit.UserName == model.UserId)
                                select new DropdownViewModel
                                {
                                    Id = 0,
                                    Text = audit.FieldName
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }
                else if (model.ActionType == "UserName")
                {
                    var list = (from audit in dbContext.Audits
                                where (model.UserId == null || audit.UserName == model.UserId)
                                && (model.TableName == null || audit.TableName == model.TableName)
                                && (model.FormName == null || audit.FormName == model.FormName)
                                select new DropdownViewModel
                                {
                                    Id = (string.IsNullOrEmpty(audit.UserName) || audit.UserName == "0") ? 0 : dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId.ToString() == audit.UserName).UserRefId,
                                    Text = (string.IsNullOrEmpty(audit.UserName) || audit.UserName == "0") ? "NA" : (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId.ToString() == audit.UserName).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId.ToString() == audit.UserName).MiddleName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId.ToString() == audit.UserName).LastName)
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    return null;
                }
            }
        }


        public List<CheckBoxViewModel> GetDepartmentCheckBoxList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from dept in dbContext.DepartmentMsts
                            where dept.IsActive == true
                            select new CheckBoxViewModel
                           {
                               Id = dept.departmentId,
                               CheckBoxId = dept.departmentId,
                               CheckBoxName = dept.departmentName,
                               IsChecked = false
                           }).ToList();
                return list;
            }
        }


        public DataSourceResult GetDepartmentListByIdAsDataSource(DataSourceRequest request, UsersModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<UsersModel> list = new List<UsersModel>();
                var departmentIdList = dbContext.UmUserDepartmentTrans.Where(i => i.UserRefId == model.id).ToList();
                if (departmentIdList != null && departmentIdList.Count > 0)
                {
                    string department = string.Empty;
                    foreach (var dept in departmentIdList)
                    {
                        department = department + dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == dept.DepartmentId).departmentName + ",";
                    }
                    department.TrimEnd(',');
                    model.strDeptts = department;
                    list.Add(model);
                }
                return list.ToDataSourceResult(request);


                //var users = new List<UsersModel>();

                //var innerQuery = from m in dbContext.UmUserMasterRoles join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId where r.RoleType.ToLower() == Constants.SuperAdmin.ToLower() select m.UserRefId;
                //users = (from u in dbContext.UmUserMasters
                //         //join m in dbContext.UmUserMasterRoles on u.UserRefId equals m.UserRefId
                //         //join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId
                //         where !innerQuery.Contains(u.UserRefId) && u.UserRefId != loginUserId//&& r.RoleType.ToLower() != Constants.SuperAdmin.ToLower()
                //         orderby u.UserRefId descending
                //         select new UsersModel
                //         {
                //             id = u.UserRefId,
                //             email = u.Email,
                //             firstName = u.FirstName,
                //             middleName = u.MiddleName,
                //             lastName = u.LastName,
                //             mobile = u.Mobile,
                //             empID = u.UserName,
                //             //createdBy = dbContext.UmUserMasters.Where(p => p.UserRefId.ToString() == u.CreatedBy).Select(p => p.FirstName + " " + p.MiddleName + " " + p.LastName).FirstOrDefault(),
                //             //lastModified = (u.ModifiedDate == null ? u.CreatedDate : u.ModifiedDate),
                //             isActive = u.IsActive.Value,
                //             lstDeptt = (from ma in dbContext.UmDepartmentMasters join d in dbContext.UmUserDepartmentTrans on ma.DepartmentId equals d.DepartmentId join us in dbContext.UmUserMasters on d.UserRefId equals us.UserRefId where us.UserRefId == u.UserRefId select ma.DepartmentName).ToList(),
                //             fullName = u.FirstName + " " + u.MiddleName + " " + u.LastName
                //         }).ToList();

                //foreach (var us in users)
                //{
                //    if (us.lstDeptt.Count > 0)
                //    {
                //        foreach (var dept in us.lstDeptt)
                //        {
                //            us.strDeptts = us.strDeptts + dept + ",";
                //        }
                //        us.strDeptts = us.strDeptts.TrimEnd(',');
                //    }
                //}
                //return users;
            }
        }


        public DataSourceResult GetUserNameListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.FilterType == "Customer")
                {
                    var list = (from customer in dbContext.CustomerMsts
                                select new DropdownViewModel
                                {
                                    Id = customer.Id,
                                    Text = customer.UserName
                                });
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    var list = (from employee in dbContext.UmUserMasters
                                select new DropdownViewModel
                                {
                                    Id = employee.UserRefId,
                                    Text = employee.UserName
                                });
                    return list.ToDataSourceResult(request);
                }
            }
        }


        public UserViewModel GetUsersDetailById(UserViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                string filePath = ConfigurationManager.AppSettings["DocumentFilesPath"].ToString();
                if (model.FilterType == "Customer")
                {
                    var list = (from user in dbContext.CustomerMsts
                                from role in dbContext.UmRoleMasters.Where(r => r.RoleId == user.RoleId).DefaultIfEmpty()
                                where user.Id == model.Id
                                select new UserViewModel
                                {
                                    Id = user.Id,
                                    PropertyId = user.PropertyId,
                                    RegistrationId = user.RegistrationId,
                                    UserName = user.UserName,
                                    Password = user.Password,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    FullName = user.FirstName + " " + user.LastName,
                                    MobileNo = user.MobileNo,
                                    Email = user.Email,
                                    RoleId = user.RoleId,
                                    RoleName = role.RoleName,
                                    DepartmentId = user.DepartmentId,
                                    Department = (user.DepartmentId == null || user.DepartmentId <= 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(x => x.departmentId == user.DepartmentId).departmentName,
                                    Remarks = user.Remarks
                                }).FirstOrDefault();
                    return list;
                }
                else
                {
                    var list = (from user in dbContext.UmUserMasters
                                //from role in dbContext.UmRoleMasters.Where(r => r.RoleId == user.RoleId).DefaultIfEmpty()
                                where user.UserRefId == model.Id
                                select new UserViewModel
                                {
                                    Id = user.UserRefId,
                                    UserName = user.UserName,
                                    Password = user.Password,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    FullName = user.FirstName + " " + user.LastName,
                                    MobileNo = user.Mobile,
                                    Email = user.Email
                                }).FirstOrDefault();
                    return list;
                }
            }
        }


        public int SaveUserDetailById(UserViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                if (model.FilterType == "Customer")
                {
                    var customer = dbContext.CustomerMsts.FirstOrDefault(c => c.Id == model.Id);
                    if (customer != null)
                    {
                        customer.RegistrationId = model.RegistrationId;
                        customer.PropertyId = model.PropertyId;
                        customer.FirstName = model.FirstName;
                        customer.LastName = model.LastName;
                        customer.MobileNo = model.MobileNo;
                        customer.Email = model.Email;
                        customer.ModifiedDate = DateTime.Now;
                        customer.ModifiedBy = userInfo.UserID.ToString();
                        dbContext.SaveChanges();
                        flag = ReturnType.Updated;
                    }
                }
                if (model.FilterType == "Employee")
                {
                    var employee = dbContext.UmUserMasters.FirstOrDefault(c => c.UserRefId == model.Id);
                    if (employee != null)
                    {
                        employee.UserName = model.UserName;
                        employee.FirstName = model.FirstName;
                        employee.LastName = model.LastName;
                        employee.Mobile = model.MobileNo;
                        employee.Email = model.Email;
                        employee.ModifiedBy = userInfo.UserID.ToString();
                        employee.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Updated;
                    }
                }
                return flag;
            }
        }


        public int RegisterCustomerDetails(UserViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var flag = ReturnType.None;
            CustomerMst ctxCustomer = new CustomerMst();
            using (var dbContext = new NoidaPMSEntities())
            {
                if (files != null && files.Count() > 0)
                {
                    var fileList = files.ToList();
                    model.CustomerIdFileName = fileList[0] != null ? fileList[0].FileName : string.Empty;
                    model.AuthorityLetter = fileList[1] != null ? fileList[1].FileName : string.Empty;
                }

                var existingCustomer = dbContext.CustomerMsts.FirstOrDefault(us => us.UserName == model.RegistrationId.ToString());
                if (existingCustomer != null)
                {
                    //creating password
                    //var newPassword = "password123";
                    //existingCustomer.Pasword = newPassword.ToMD5HashForPasswordPIS();

                    existingCustomer.RoleId = 2;
                    existingCustomer.DepartmentId = model.DepartmentId;
                    existingCustomer.Sector = model.Sector;
                    existingCustomer.Block = model.Block;
                    existingCustomer.PlotNo = model.PlotNo;
                    existingCustomer.ModifiedDate = DateTime.Now;
                    existingCustomer.MobileNo = model.MobileNo;
                    existingCustomer.Email = model.Email;
                    existingCustomer.PropertyId = model.PropertyId.ToString();
                    existingCustomer.FirstName = model.Applicant;
                    existingCustomer.IdFileName = model.CustomerIdFileName;
                    existingCustomer.IdFileType = model.CustomerIdFileType;
                    existingCustomer.PropertyFileName = model.AuthorityLetter;
                    existingCustomer.PropertyFileType = model.AuthorityLetterType;
                    existingCustomer.SecurityQuestion = model.SecurityQuestion;
                    existingCustomer.SecurityAnswer = model.SecurityAnswer;
                    existingCustomer.IsActive = true;
                    existingCustomer.StatusId = NAStatusId.Pending;
                    existingCustomer.IsFirstTimeActivated = false;
                }
                else
                {
                    //ctxCustomer.UserId = Guid.NewGuid();
                    ctxCustomer.UserName = model.RegistrationId.ToString();
                    ctxCustomer.FirstName = model.Applicant;
                    ctxCustomer.DepartmentId = model.DepartmentId;
                    ctxCustomer.PropertyId = model.PropertyId.ToString();
                    ctxCustomer.RoleId = 2;
                    ctxCustomer.Sector = model.Sector;
                    ctxCustomer.Block = model.Block;
                    ctxCustomer.PlotNo = model.PlotNo;
                    ctxCustomer.MobileNo = model.MobileNo;
                    ctxCustomer.Email = model.Email;
                    ctxCustomer.CreatedDate = DateTime.Now;
                    ctxCustomer.CreatedBy = model.UserName;
                    ctxCustomer.IdFileName = model.CustomerIdFileName;
                    ctxCustomer.IdFileType = model.CustomerIdFileType;
                    ctxCustomer.PropertyFileName = model.AuthorityLetter;
                    ctxCustomer.PropertyFileType = model.AuthorityLetterType;
                    ctxCustomer.SecurityQuestion = model.SecurityQuestion;
                    ctxCustomer.SecurityAnswer = model.SecurityAnswer;
                    ctxCustomer.IsActive = false;
                    ctxCustomer.StatusId = NAStatusId.Initiated;
                    ctxCustomer.IsFirstTimeActivated = false;
                    //creating password
                    var newPassword = "password123";
                    ctxCustomer.Password = newPassword.ToMD5HashForPasswordPIS();

                    dbContext.CustomerMsts.Add(ctxCustomer);

                    var msg1 = NAMessages.PIS_registration_1;
                    if (model.Email != null)
                    {
                        EmailHelper emailHelper = new EmailHelper();
                        emailHelper.Send(model.Email, "Registration Completed", msg1 + "Regards, http://mynoida.in");
                    }
                    //Send SMS
                    ApplicationHelper.SendSMS(model.MobileNo, msg1);
                    //Sending mail to From Address on new Registration, as asked by Vishal Shukla to do so
                    var emailAdd = System.Configuration.ConfigurationManager.AppSettings["SmtpFromAddress"];
                    var msg2 = string.Format(NAMessages.PIS_registration_2, ctxCustomer.PropertyId);
                    if (!string.IsNullOrEmpty(emailAdd))
                    {
                        EmailHelper emailHelper = new EmailHelper();
                        emailHelper.Send(emailAdd, "Registration Completed", msg2 + "<br><br>Regards, http://mynoida.in");
                    }
                }
                dbContext.SaveChanges();
                var dflag = SaveCustomerDocument(model, files);
                flag = ReturnType.Saved;
            }
            return flag;
        }

        private int SaveCustomerDocument(UserViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int flag = ReturnType.None;
            if (files != null && files.Count() > 0)
            {
                var directoryPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FilePath"]).ToString() + model.RegistrationId;
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                foreach (var fyl in files)
                {
                    if (fyl != null)
                    {
                        fyl.SaveAs(HttpContext.Current.Server.MapPath((ConfigurationManager.AppSettings["FilePath"]) + model.RegistrationId).ToString() + "/" + fyl.FileName);
                    }
                }
                flag = ReturnType.Saved;
            }
            return flag;
        }


    }
}
