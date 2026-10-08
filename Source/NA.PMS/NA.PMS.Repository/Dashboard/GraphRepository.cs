using NA.PMS.Common;
using NA.PMS.Dal.CustomerContext;
using NA.PMS.Model;
using NoidaAuthority.PMS.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Repository
{
    public class GraphRepository : IGraphRepository
    {
        public IEnumerable<UserViewModel> GetUsers()
        {
            var result = new List<UserViewModel>();
            try
            {
                using (var dbContext = new CustomerContext())
                {
                    result = (from lstUsers in dbContext.Users.ToList()
                              join lstRoles in dbContext.Roles.ToList() on lstUsers.RoleId equals lstRoles.RoleId
                              select new UserViewModel
                              {
                                  UserId = lstUsers.UserId,
                                  UserName = lstUsers.UserName,
                                  Pasword = lstUsers.Pasword,
                                  FirstName = lstUsers.FirstName,
                                  LastName = lstUsers.LastName,
                                  MobileNo = lstUsers.MobileNo,
                                  Status = lstUsers.Status,
                                  PropertyId = lstUsers.PropertyId,
                                  IsLocked = lstUsers.IsLockedOut,
                                  CreatedOn = lstUsers.CreatedOn,
                                  CreatedBy = lstUsers.CreatedBy,
                                  ModifiedBy = lstUsers.ModifiedBy,
                                  ModifiedOn = lstUsers.ModifiedOn,
                                  RoleId = lstUsers.RoleId,
                                  RoleName = lstRoles.RoleName,
                                  LastPasswordChangeDate = lstUsers.LastPasswordChangeDate,
                                  FullName = lstUsers.FirstName + " " + lstUsers.LastName,
                                  CustomerIdFileName = lstUsers.CustomerIdFileName == null ? string.Empty : ConfigurationManager.AppSettings["DocumentFilesPath"].ToString() + lstUsers.PropertyId + "/" + lstUsers.CustomerIdFileName,
                                  CustomerIdFileType = lstUsers.CustomerIdFileType,
                                  AuthorityLetter = lstUsers.CustomerLetterFileName == null ? string.Empty : ConfigurationManager.AppSettings["DocumentFilesPath"].ToString() + lstUsers.PropertyId + "/" + lstUsers.CustomerLetterFileName,
                                  AuthorityLetterType = lstUsers.CustomerLetterType,
                                  DepartmentId = lstUsers.DeptId,
                                  IsRejected = lstUsers.IsRejected,
                                  Remarks = lstUsers.Remarks,
                                  IsFirstTimeActivated = lstUsers.IsFirstTimeActivated,
                                  Email = lstUsers.UserEmail
                              }).OrderBy(x => x.UserName).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Boolean SendPassword(string email, string mobileNo, string UserId)
        {
            var flag = false;
            try
            {
                using (var dbContext = new CustomerContext())
                {
                    var newPassword = CreatePassword();
                    var existingUser = dbContext.Users.Where(us => us.UserName.ToLower() == UserId.ToLower()).FirstOrDefault();
                    existingUser.Pasword = newPassword.ToMD5HashForPasswordPIS();
                    existingUser.Status = true;
                    if (existingUser.IsFirstTimeActivated == false || existingUser.IsFirstTimeActivated == null)
                        existingUser.IsFirstTimeActivated = true;
                    dbContext.SaveChanges();
                    //Send Email
                    var body = string.Format(NAMessages.PIS_Registration_Activation, existingUser.UserName, newPassword); //"Dear User, You are successfully registered with mynoida.in. Your user name is " + existingUser.UserName + " and password is " + newPassword + " Regards, http://mynoida.in";
                    if (email != "")
                    {
                        EmailHelper emailHelper = new EmailHelper();
                        emailHelper.Send(email, "Generated Password", body);
                    }
                    //Send SMS
                    var msg = string.Format(NAMessages.PIS_Registration_Activation, existingUser.UserName, newPassword);// "Dear User, You are successfully registered with mynoida.in. Your user name is " + existingUser.UserName + " and password is " + newPassword + " Regards, http://mynoida.in";
                    Common.ApplicationHelper.SendSMS(mobileNo, msg);

                    flag = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return flag;
        }

        //Lock/UnLock Customer
        public Boolean LockUnLockCustomer(string email)
        {
            Boolean IsLockedStatus = false;
            try
            {
                using (var dbContext = new CustomerContext())
                {
                    var IsLocked = dbContext.Users.Where(us => us.UserName.ToLower().Trim() == email.ToLower().Trim()).FirstOrDefault();
                    if (IsLocked != null)
                    {
                        IsLocked.IsLockedOut = IsLocked.IsLockedOut != true ? true : false;
                        IsLockedStatus = (bool)IsLocked.IsLockedOut;
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return IsLockedStatus;
        }

        public Boolean ResetPassword(string emailID)
        {
            try
            {
                var flag = false;
                using (var dbContext = new CustomerContext())
                {
                    var existingUser = dbContext.Users.Where(us => us.UserName.ToLower() == emailID.ToLower() && us.Status == true).FirstOrDefault();
                    if (existingUser != null)
                    {
                        var newPassword = CreatePassword();
                        existingUser.Pasword = newPassword.ToMD5HashForPasswordPIS();
                        dbContext.SaveChanges();
                        flag = true;
                        //Send Email
                        if (existingUser.UserEmail != null && existingUser.UserEmail != "")
                        {
                            var body = string.Format(NAMessages.PIS_PasswordChanges_Email, newPassword);// "Dear User,<br><br>You have successfully changed your password. Your new password is " + newPassword + ". DO NOT disclose this to anyone by any means.<br>Regards, http://mynoida.in";
                            EmailHelper emailHelper = new EmailHelper();
                            emailHelper.Send(existingUser.UserEmail, "New Generated Password", body);
                        }
                        var msg = string.Format(NAMessages.PIS_PasswordChanges_SMS, newPassword);//"Dear User, You have successfully changed your password. Your new password is" + newPassword + "Regards, http://mynoida.in";
                        Common.ApplicationHelper.SMSSend(existingUser.MobileNo, msg);
                    }
                    else
                    {
                        flag = false; //Case when Email ID does not exist in our DB (User does not exist or is not Active).
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Boolean DeactivateUser(string email)
        {
            var flag = false;
            try
            {
                using (var dbContext = new CustomerContext())
                {
                    var existingUser = dbContext.Users.Where(us => us.UserName.ToLower().Trim() == email.ToLower().Trim()).FirstOrDefault();
                    if (existingUser != null)
                    {
                        existingUser.Status = false;
                        dbContext.SaveChanges();
                        if (existingUser.UserEmail != null)
                        {
                            EmailHelper emailHelper = new EmailHelper();
                            emailHelper.Send(existingUser.UserEmail,Constants.deactivatedMailSubject_PIS, NAMessages.PIS_Account_Deactivated_Email);
                        }
                        //Send SMS
                        var msg = NAMessages.PIS_Account_Deactivated_Email;//"Dear User, Your account has been deactivated. Regards, http://mynoida.in";
                        Common.ApplicationHelper.SMSSend(existingUser.MobileNo, msg);
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

        public Boolean RejectCustomer(string email, string mobileNo, string remarks)
        {
            var flag = false;
            try
            {
                using (var dbContext = new CustomerContext())
                {
                    var existingUser = dbContext.Users.Where(us => us.UserName.ToLower().Trim() == email.ToLower().Trim()).FirstOrDefault();
                    if (existingUser != null)
                    {
                        existingUser.IsRejected = true;
                        existingUser.Remarks = remarks;
                        dbContext.SaveChanges();
                        if (existingUser.UserEmail != null)
                        {
                            EmailHelper emailHelper = new EmailHelper();
                            emailHelper.Send(existingUser.UserEmail, Constants.rejectedMailSubject_PIS, string.Format(NAMessages.PIS_App_Rejected_Email, remarks));
                        }
                        //Send SMS
                        var msg = string.Format(NAMessages.PIS_App_Rejected_SMS, remarks);// "Dear User, Due to " + remarks + " Your Application has been rejected. Regards, http://mynoida.in";
                        Common.ApplicationHelper.SMSSend(mobileNo, msg);
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
    }
}
