
using NA.PMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Common.Extension;
using System.Net;
using System.Configuration;
using System.IO;
using NA.PMS.Model;
using NoidaAuthority.PMS.Common;
using NA.PMS.Common;
using NA.PMS.Web.Models;
using System.Web;
using System.Data.SqlClient;
using NA.PMS.Dal.CustomerContext;
using NA.PMS.Model.Entities;

namespace NA.PMS.Repository
{
    public class LoginRepository : ILoginRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();

        public LoginRepository()
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
        /// validate user name and password at login time
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>

        public bool ValidateUser(string userName, string password)
        {
            var flag = false;
            var encryptedPassword = password.ToMD5HashForPassword();
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.UmUserMasters.FirstOrDefault(cond => cond.UserName == userName && cond.Password == encryptedPassword);
                if (user != null)
                {
                    flag = true;
                }
            }
            return flag;
        }

        public bool ValidateUserGetTypeAndId(string userName, string password)
        {
            bool flag = false;
            var encryptedPassword = password.ToMD5HashForPassword();
            using (var dbContext = new NoidaPMSEntities())
            {
                var pimsUsers = (from u in dbContext.UmUserMasters
                                 where u.UserName == userName && u.Password == encryptedPassword
                                 select u).FirstOrDefault();

                if (pimsUsers != null)
                { flag = true; }
            }
            return flag;
        }
        

        public bool ValidateUserName(string userName)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.UmUserMasters.FirstOrDefault(cond => cond.UserName == userName);
                if (user != null)
                {
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// Forgot password based on username/email/mobile no
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="mobileNo"></param>
        /// <param name="otp"></param>
        /// <returns></returns>

        public bool ForgotPassword(string userName, string email, string mobileNo, int otp)
        {
            var flag = false;

            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.UmUserMasters.FirstOrDefault(cond => cond.UserName == userName);
                if (user != null)
                {
                    var mobile = user.Mobile.ToString();
                    sendOTPtoUser(mobile, otp);
                    var body = "Dear User, Your One Time Password (OTP) for mynoida.in user is " + otp.ToString() + " Regards, http://mynoida.in";
                    EmailHelper emailHelper = new EmailHelper();
                    emailHelper.Send(user.Email, "OTP by Noida Authority", body);
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// change password for logged user verify email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool ChangePassword(string email, string newPassword)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.UmUserMasters.FirstOrDefault(cond => cond.UserName == email);
                if (user != null)
                {
                    user.Password = newPassword.ToMD5HashForPassword();
                    user.ModifiedDate = DateTime.Now;
                    user.ModifiedBy = userInfo.UserID.ToString();
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// change password for logged user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>

        public bool ChangePassword(string userName, string email, string newPassword)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.UmUserMasters.FirstOrDefault(cond => cond.UserName == userName && cond.Email == email);
                if (user != null)
                {
                    user.Password = newPassword.ToMD5HashForPassword();
                    user.ModifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    if (user.Email != null)
                    {
                        var body = "Dear User,<br><br>You have successfully changed your password. Your new password is " + newPassword + ". </br></br>Regards, </br>http://mynoida.in";
                        EmailHelper emailHelper = new EmailHelper();
                        emailHelper.Send(user.Email, "You have successfully changed your password.", body);
                    }

                    if (user.Mobile != null)
                    {
                        //var msg = "Dear User, You have successfully changed your password. Your new password is " + newPassword + ". Regards, http://mynoida.in";
                        var msg = string.Format(NAMessages.PasswordChange, newPassword);
                        //SMSSend(user.Mobile, msg);
                        ApplicationHelper.SendSMS(user.Mobile, msg);
                    }
                    flag = true;
                }
            }
            return flag;
        }

        public bool CheckEmailAddressExists(string emailAddress)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.UmUserMasters.FirstOrDefault(cond => cond.UserName.ToLower() == emailAddress.ToLower());
                if (user != null)
                {
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// user is locked after trying 5 times 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>

        public bool LockUser(string userName)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.UmUserMasters.FirstOrDefault(cond => cond.UserName == userName);
                if (user != null)
                {
                    user.IsActive = false;
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }

        public ViewUmUserMaster GetViewUserDetails(string userName)
        {
            ViewUmUserMaster userMaster = null;
            using (var dbContext = new NoidaPMSEntities())
            {
                //userMaster = dbContext.ViewUmUserMasters.Where(cond => cond.UserName.ToLower() == userName.ToLower());
                userMaster = dbContext.ViewUmUserMasters.FirstOrDefault(cond => cond.UserName.ToLower() == userName.ToLower());
            }
            return userMaster;
        }

        /// <summary>
        /// provide role details of logged user on userid
        /// </summary>
        /// <param name="UserRefId"></param>
        /// <returns></returns>
        public UmRoleMaster GetUserRoleDetails(int? UserRefId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                UmRoleMaster userRole = null;
                userRole = (from roles in dbContext.UmUserMasterRoles
                            join roleMaster in dbContext.UmRoleMasters on roles.RoleId equals roleMaster.RoleId
                            where roles.UserRefId == UserRefId && roles.isActive == true
                            select roleMaster).FirstOrDefault();

                return userRole;
            }
        }

        /// <summary>
        ///  provide application details of logged user
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public UmApplicationMaster GetUserApplicationDetails(int? applicationId)
        {
            UmApplicationMaster userApplication = null;
            using (var dbContext = new NoidaPMSEntities())
            {
                userApplication = dbContext.UmApplicationMasters.FirstOrDefault(cond => cond.ApplicationId == applicationId);
            }
            return userApplication;
        }

        /// <summary>
        ///  provide menu details to user
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public UmMenuMaster GetUserMenuDetails(int menuId)
        {
            UmMenuMaster userMenu = null;
            using (var dbContext = new NoidaPMSEntities())
            {
                userMenu = dbContext.UmMenuMasters.FirstOrDefault(cond => cond.MenuId == menuId);
            }
            return userMenu;
        }

        /// <summary>
        /// provide user details after logged on
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>

        public Boolean SendOTPtoUser(string mobNo, int r)
        {
            var result = false;
            try
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    if (mobNo != null)
                    {
                        //var msg = "Dear User, Your OTP is " + r + " DO NOT disclose this OTP to anyone. This is for online use by you only. Regards, http://mynoida.in";
                        var msg = string.Format(NAMessages.OTPSend, r);
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

        /// <summary>
        /// provide user details after logged on
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>

        public UmUserMaster GetUserDetails(string userName)
        {
            UmUserMaster userMaster = null;
            using (var dbContext = new NoidaPMSEntities())
            {
                userMaster = dbContext.UmUserMasters.FirstOrDefault(cond => cond.UserName == userName);

            }
            return userMaster;
        }

        public List<ServiceRequestModel> GetServiceRequestNotifications(int userid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var departmentList = (from dept in dbContext.UmDepartmentMasters
                                      join udts in dbContext.UmUserDepartmentTrans on dept.DepartmentId equals udts.DepartmentId
                                      where udts.UserRefId == userid //userInfo.UserID
                                      select dept.DepartmentId);
                //var requests = (from csr in dbContext.View_Service_report //dbContext.Customer_ServiceRequest
                //                //join csm in dbContext.CitizenService_Master on new { x = csr.ServiceId, y = csr.DepartmentId } equals new { x=csm.service_id, y=csm.Deptt_Id}
                //                where departmentList.Contains(csr.DepartmentId.Value) && csr.Request_Status.Value == Constants.Initiated
                //                select new ServiceRequestModel
                //                {
                //                    Id = csr.requestNo,
                //                    Registration_No = csr.Registration_No,
                //                    ServiceId = csr.ServiceId,
                //                    ServiceName = csr.ServiceName,
                //                    DepartmentId = csr.DepartmentId,
                //                    DepartmentName = dbContext.DepartmentMsts.Where(d => d.departmentId == csr.DepartmentId).Select(s => s.departmentName).FirstOrDefault()
                //                }).Distinct().ToList();

                var requests = (from csr in dbContext.View_Service_report //dbContext.Customer_ServiceRequest
                                //join csm in dbContext.CitizenService_Master on new { x = csr.ServiceId, y = csr.DepartmentId } equals new { x=csm.service_id, y=csm.Deptt_Id}
                                where departmentList.Contains(csr.DepartmentId.Value) && csr.Request_Status.Value == Constants.Pending
                                select new ServiceRequestModel
                                {
                                    Id = csr.requestNo,
                                    Registration_No = csr.Registration_No,
                                    ServiceId = csr.ServiceId,
                                    ServiceName = csr.ServiceName,
                                    DepartmentId = csr.DepartmentId,
                                    DepartmentName = dbContext.DepartmentMsts.Where(d => d.departmentId == csr.DepartmentId).Select(s => s.departmentName).FirstOrDefault()
                                }).Distinct().ToList();
                return requests;
            }//
        }

        public List<ServiceRequestModel> GetServiceRequestForApproval(int userid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var departmentList = (from dept in dbContext.UmDepartmentMasters
                                      join udts in dbContext.UmUserDepartmentTrans on dept.DepartmentId equals udts.DepartmentId
                                      where udts.UserRefId == userid //userInfo.UserID
                                      select dept.DepartmentId);
                var requests = (from csr in dbContext.View_Service_report
                                where departmentList.Contains(csr.DepartmentId.Value) && csr.Request_Status.Value == Constants.Pending
                                select new ServiceRequestModel
                                {
                                    Id = csr.requestNo,
                                    Registration_No = csr.Registration_No,
                                    ServiceId = csr.ServiceId,
                                    ServiceName = csr.ServiceName,
                                    DepartmentId = csr.DepartmentId,
                                    DepartmentName = dbContext.DepartmentMsts.Where(d => d.departmentId == csr.DepartmentId).Select(s => s.departmentName).FirstOrDefault()
                                }).Distinct().ToList();
                return requests;
            }
            //throw new NotImplementedException();
        }

        public List<int> GetServiceRequestCount(int userid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var departmentList = (from dept in dbContext.UmDepartmentMasters
                                      join udts in dbContext.UmUserDepartmentTrans on dept.DepartmentId equals udts.DepartmentId
                                      where udts.UserRefId == userInfo.UserID
                                      select dept.DepartmentId);

                var reqlist = (from csr in dbContext.Customer_ServiceRequest
                               where departmentList.Contains(csr.DepartmentId.Value) && (csr.Request_Status == 8 || csr.Request_Status == 5)
                               select csr.Id).ToList();
                return reqlist;
            }
        }

        public List<NoidaCustomerModel> GetNewRegisteredCustomerList(int userid)
        {
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PISSqlConnection"].ConnectionString);
            List<NoidaCustomerModel> customerList = new List<NoidaCustomerModel>();
            try
            {
                conn.Open();
                SqlCommand sqlcommand = new SqlCommand("SELECT * FROM USERS WHERE STATUS=0", conn);
                SqlDataReader dataReader = sqlcommand.ExecuteReader();

                while (dataReader.Read())
                {
                    NoidaCustomerModel model = new NoidaCustomerModel();
                    //model.UserId = Convert.ToInt32(dataReader.GetValue(0));
                    model.UserName = dataReader.GetValue(1).ToString();
                    model.FirstName = dataReader.GetValue(3).ToString();
                    model.LastName = dataReader.GetValue(4).ToString();
                    model.MobileNo = dataReader.GetValue(14).ToString();
                    model.UserEmail = dataReader.GetValue(27).ToString();
                    //model.Status = dataReader.GetValue(5).ToString().ToLower() == "true" ? true : false;
                    //model.IsLocked = null;
                    customerList.Add(model);
                }
            }
            catch (Exception ex)
            {

            }
            return customerList;
        }

        /// <summary>
        /// provide roles information to user who has multiple roles
        /// </summary>
        /// <param name="UserRefId"></param>
        /// <returns></returns>

        public List<UmRoleMaster> GetRoleMasterDetails(int UserRefId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var roless = (from roles in dbContext.UmUserMasterRoles
                              join roleMaster in dbContext.UmRoleMasters on roles.RoleId equals roleMaster.RoleId
                              where roles.UserRefId == UserRefId
                              select roleMaster).ToList();
                return roless;
            }
        }

        /// <summary>
        /// get multiple application details who has multiple role
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>

        public List<UmApplicationMaster> GetApplicationDetails(int roleId)
        {
            List<UmApplicationMaster> applications = null;
            using (var dbContext = new NoidaPMSEntities())
            {
                applications = (from apps in dbContext.UmApplicationMasters
                                join roleMasterTrans in dbContext.UmRoleMasterTrans on apps.ApplicationId equals roleMasterTrans.ApplicationId
                                where roleMasterTrans.RoleId == roleId
                                select apps).ToList();
            }
            return applications;
        }

        /// <summary>
        /// provide menu details based on user's application
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>

        public MenuMaster GetMenuDetails(int roleId, int applicationId)
        {
            MenuMaster menu = null;
            using (var dbContext = new NoidaPMSEntities())
            {
                menu = (from menus in dbContext.UmMenuMasters
                        join roleMasterTrans in dbContext.UmRoleMasterTrans on menus.MenuId equals roleMasterTrans.MenuId
                        where roleMasterTrans.RoleId == roleId
                        select new MenuMaster
                        {
                            MenuId = menus.MenuId,
                            MenuName = menus.MenuName,
                            MenuParentId = menus.MenuParentId,
                            MenuPathId = menus.MenuPathId,
                            IsActive = menus.IsActive,
                            IsRead = roleMasterTrans.IsRead,
                            IsWrite = roleMasterTrans.IsWrite,
                            IsUpdate = roleMasterTrans.IsUpdate,
                            Isdelete = roleMasterTrans.Isdelete
                        }).FirstOrDefault();
            }
            return menu;
        }

        /// <summary>
        /// provide mobile no to user who forgot password
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>

        public string ValidateUserNameForgetPassword(string userName)
        {
            string mobileNo = null;
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.UmUserMasters.FirstOrDefault(cond => cond.UserName == userName);
                if (user != null)
                {
                    mobileNo = user.Mobile.ToString();

                }
            }
            return mobileNo;
        }
        /// <summary>
        /// all details of logged user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public LoginUserDetail GetLoginUserDetails(string userName)
        {
            LoginUserDetail loginUserDetail = null;
            using (var dbContext = new NoidaPMSEntities())
            {
                loginUserDetail = (from user in dbContext.UmUserMasters
                                   where user.UserName == userName
                                   select new LoginUserDetail
                                   {
                                       UserRefId = user.UserRefId,
                                       UserName = user.UserName,
                                       FirstName = user.FirstName,
                                       LastName = user.LastName,
                                       MiddleName = user.MiddleName,
                                       IsActive = user.IsActive,
                                       Email = user.Email,
                                       Mobile = user.Mobile,
                                       DepartmentList = (from depts in dbContext.UmDepartmentMasters
                                                         join deptsTrans in dbContext.UmUserDepartmentTrans on depts.DepartmentId equals deptsTrans.DepartmentId
                                                         where deptsTrans.UserRefId == user.UserRefId
                                                         select new DepartmentMaster
                                                         {
                                                             DepartmentId = depts.DepartmentId,
                                                             DepartmentName = depts.DepartmentName,
                                                             Status = depts.Status
                                                         }).ToList(),

                                       RolesList = (from roles in dbContext.UmRoleMasters
                                                    join umr in dbContext.UmUserMasterRoles on roles.RoleId equals umr.RoleId
                                                    where umr.UserRefId == user.UserRefId
                                                    select new RoleMaster
                                                    {
                                                        RoleId = roles.RoleId,
                                                        RoleName = roles.RoleName,
                                                        RoleType = roles.RoleType,
                                                    }).ToList(),

                                       ApplicationsList = (from app in dbContext.UmApplicationMasters
                                                           join rat in dbContext.UmRoleAppTrans on app.ApplicationId equals rat.ApplicationId
                                                           join umr in dbContext.UmUserMasterRoles on rat.RoleId equals umr.RoleId
                                                           where umr.UserRefId == user.UserRefId
                                                           select new ApplicationMaster
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
                                                    select new MenuMaster
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
            return loginUserDetail;
        }

        /// <summary>
        /// Send OTP for user who forgot password
        /// </summary>
        /// <param name="mobNo"></param>
        /// <param name="otp"></param>
        /// <returns></returns>

        public Boolean sendOTPtoUser(string mobNo, int otp)
        {
            var result = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                //var phoneNo = dbContext.view_property.Where(us => us.PRDVREGISTRATION_ID.ToLower() == id.ToLower()).Select(us => us.mobile_no).FirstOrDefault();
                //phoneNo = "8010079321";//Hard coded for Testing purposes
                if (mobNo != null)
                {
                    //var msg = "Your One Time Password (OTP) for NOIDA Customer is " + otp;
                    var msg = string.Format(NAMessages.OTPSend, otp);
                    //SMSSend(mobNo, msg);
                    ApplicationHelper.SendSMS(mobNo, msg);
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Send SMS to user who forgot password
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <param name="msg"></param>

        private void SMSSend(string mobileNo, string msg)
        {
            WebClient client = new WebClient();
            //string baseurl = ConfigurationManager.AppSettings["SMSsend"].ToString() + ConfigurationManager.AppSettings["SMSUsername"].ToString() + "&password=" + ConfigurationManager.AppSettings["SMSPassword"].ToString() + "&sendername=" + ConfigurationManager.AppSettings["SMSSenderID"].ToString() + "&mobileno=" + mobileNo + "&message=" + msg;
            string baseurl = ConfigurationManager.AppSettings["SMSApiUrl"].ToString() + "ApiKey=" + ConfigurationManager.AppSettings["SMSApiKey"].ToString() + "&ClientId=" + ConfigurationManager.AppSettings["SMSClientId"].ToString() + "&SenderId=" + ConfigurationManager.AppSettings["SMSSenderId"].ToString() + "&Message=" + msg + "&MobileNumbers=91" + mobileNo + "&Is_Unicode=" + ConfigurationManager.AppSettings["SMSIsUnicode"].ToString() + "&Is_Flash=" + ConfigurationManager.AppSettings["SMSIsFlash"].ToString();
            Stream data = client.OpenRead(baseurl);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            data.Close();
            reader.Close();
        }


        public List<MenuMaster> GetMenuDetailsByRoleId(int roleId)
        {
            List<MenuMaster> menu = new List<MenuMaster>();
            using (var dbContext = new NoidaPMSEntities())
            {
                menu = (from menus in dbContext.UmMenuMasters
                        join roleMasterTrans in dbContext.UmRoleMasterTrans on menus.MenuId equals roleMasterTrans.MenuId
                        where roleMasterTrans.RoleId == roleId
                        select new MenuMaster
                        {
                            MenuId = menus.MenuId,
                            MenuName = menus.MenuName,
                            MenuParentId = menus.MenuParentId,
                            MenuPathId = menus.MenuPathId,
                            IsActive = menus.IsActive,
                            IsRead = roleMasterTrans.IsRead,
                            IsWrite = roleMasterTrans.IsWrite,
                            IsUpdate = roleMasterTrans.IsUpdate,
                            Isdelete = roleMasterTrans.Isdelete
                        }).ToList();
            }
            return menu;
        }

        /// <summary>
        /// Return application details to login user by userid
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<ApplicationMaster> GetApplicationDetailsByUserId(int userId)
        {
            List<ApplicationMaster> applicationsList = null;
            using (var dbContext = new NoidaPMSEntities())
            {
                applicationsList = (from app in dbContext.UmApplicationMasters
                                    join rat in dbContext.UmRoleAppTrans on app.ApplicationId equals rat.ApplicationId
                                    join umr in dbContext.UmUserMasterRoles on rat.RoleId equals umr.RoleId
                                    where umr.UserRefId == userId
                                    select new ApplicationMaster
                                    {
                                        ApplicationId = app.ApplicationId,
                                        ApplicationName = app.ApplicationName,
                                        ApplicationUrl = app.ApplicationUrl
                                    }).ToList();
            }
            return applicationsList;
        }

        /// <summary>
        /// Return menu details to login user by userid
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<MenuMaster> GetMenuDetailsByUserId(int userId)
        {
            List<MenuMaster> menuList = null;
            using (var dbContext = new NoidaPMSEntities())
            {
                menuList = (from menu in dbContext.UmMenuMasters
                            join rmt in dbContext.UmRoleMasterTrans on menu.MenuId equals rmt.MenuId
                            join rat in dbContext.UmUserMasterRoles on rmt.RoleId equals rat.RoleId
                            where rat.UserRefId == userId
                            select new MenuMaster
                            {
                                MenuId = menu.MenuId,
                                MenuName = menu.MenuName,
                                MenuPathId = menu.MenuPathId,
                                MenuParentId = menu.MenuParentId,
                                IsActive = menu.IsActive,
                                IsRead = rmt.IsRead,
                                IsWrite = rmt.IsWrite,
                                IsUpdate = rmt.IsUpdate,
                                Isdelete = rmt.Isdelete
                            }).ToList();
            }
            return menuList;
        }


        public bool ResetPasswordBySA(int uid, string newPassword)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.UmUserMasters.FirstOrDefault(cond => cond.UserRefId == uid);
                if (user != null)
                {
                    user.Password = newPassword.ToMD5HashForPassword();
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }


        public bool IsUserisActive(int userid)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var isActive = dbContext.UmUserMasters.Where(u => u.UserRefId == userid).Select(u => u.IsActive).FirstOrDefault();
                var isRoleActive = dbContext.UmUserMasterRoles.Where(u => u.UserRefId == userid).Select(u => u.isActive).FirstOrDefault();
                if (isActive == true && isRoleActive == true)
                {
                    flag = true;
                }
            }
            return flag;
        }


        public bool IsUserisActive()
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var isActive = dbContext.UmUserMasters.Where(u => u.UserRefId == userInfo.UserID).Select(u => u.IsActive).FirstOrDefault();
                var isRoleActive = dbContext.UmUserMasterRoles.Where(u => u.UserRefId == userInfo.UserID).Select(u => u.isActive).FirstOrDefault();
                if (isActive == true && isRoleActive == true)
                {
                    flag = true;
                }
            }
            return flag;
        }

        public List<ServiceTypeList> GetServiceRequestByServiceType(int userid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var departmentList = (from dept in dbContext.UmDepartmentMasters
                                      join udts in dbContext.UmUserDepartmentTrans on dept.DepartmentId equals udts.DepartmentId
                                      where udts.UserRefId == userid
                                      select dept.DepartmentId);
                var serviceType = (from vapr in dbContext.view_approval_pending_request
                                   where departmentList.Contains((int)vapr.departmentid) && vapr.Approved_By == userid && vapr.Status == 5//Only show Inprogress Request
                                   group new { vapr } by new { vapr.requestType } into requestTypeAll
                                   select new ServiceTypeList
                                   {
                                       ServiceType = requestTypeAll.Key.requestType,
                                       TotalCount = requestTypeAll.Count()
                                   }).ToList();
                return serviceType;
            }
        }

        #region for pis user
        public Boolean CheckRegistationIDforNAcustomer(String RegistrationId)
        {
            var flag = false;
            try
            {
                using (var dbContext = new CustomerContext())
                {
                    var customer = dbContext.Users.FirstOrDefault(cond => cond.PropertyId.ToLower() == RegistrationId.ToLower() && (cond.IsRejected == false || cond.IsRejected == null));
                    if (customer != null)
                    {
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return flag;
        }

        public Boolean CheckEmailAddressForNAcustomer(String emailAddress)
        {
            var flag = false;
            try
            {
                using (var dbContext = new CustomerContext())
                {
                    var customer = dbContext.Users.FirstOrDefault(cond => cond.UserName.ToLower() == emailAddress.ToLower() && (cond.IsRejected == false || cond.IsRejected == null));
                    if (customer != null)
                    {
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return flag;
        }
        #endregion
        #region Validate User Keshav 14 Sep 2018
        public bool ValidateUserIsExist(string username, string passwords)
        {
            bool flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var pimsUsers = (from u in dbContext.UmUserMasters
                                 where u.UserName == username && u.Password == passwords select u).FirstOrDefault();

                if (pimsUsers != null)
                { flag = true; }
            }
            return flag;

        }
        #endregion 
    

        public int SendAndValidateOTP(UserViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.MobileNo != null)
                {
                    int otp = 0;
                    var user = dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == model.Id);
                    if (user.OTPDate != null && user.OTPDate.Value.Date == DateTime.Now.Date)
                    {
                        otp = user.OTP.Value;
                    }
                    else
                    {
                        otp = ApplicationHelper.GenerateOTP();
                        user.OTP = otp;
                        user.OTPDate = DateTime.Now;
                        dbContext.SaveChanges();
                    }
                    //int r = user.OTPDate.Value.Date == DateTime.Now.Date ? user.OTP.Value : ApplicationHelper.GenerateOTP();
                    var msg = string.Format(NAMessages.OTPSend, otp);
                    NAApplication nappl = new NAApplication();
                    nappl.SaveAndSendSMS(user.UserRefId, model.MobileNo, msg, "login-otp", "login otp", user.UserRefId.ToString());
                    flag = ReturnType.Success;
                }
            }
            return flag;
        }
    }
}
