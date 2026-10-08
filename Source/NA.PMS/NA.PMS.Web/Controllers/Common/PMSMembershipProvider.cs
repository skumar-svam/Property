
using NA.PMS.Model;
using NA.PMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace NA.PMS.Web.Controllers.Common
{
    public class PMSMembershipProvider : MembershipProvider
    {
        ILoginService _LoginService = null;

        public PMSMembershipProvider()
        {
            _LoginService = new LoginService();
        }

        #region MembershipProvider Abstract method definition

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool ChangePassword(string email, string newPassword)
        {
            return _LoginService.ChangePassword(email, newPassword);
        }

        public override bool ChangePassword(string userName, string email, string newPassword)
        {
            return _LoginService.ChangePassword(userName, email, newPassword);
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            return _LoginService.ValidateUser(username, password);
        }

        #endregion

        public Boolean ValidateUserName(string userName)
        {
            return _LoginService.ValidateUserName(userName);
        }

        public Boolean ForgotPassword(string userName, string email, string mobileNo, int otp)
        {
            return _LoginService.ForgotPassword(userName, email, mobileNo, otp);
        }

        public Boolean CheckEmailAddressExists(String emailAddress)
        {
            return _LoginService.CheckEmailAddressExists(emailAddress);
        }

        public ViewUmUserMaster GetViewUserDetails(String email)
        {
            return _LoginService.GetViewUserDetails(email);
        }

        public UmRoleMaster GetUserRoleDetails(int? UserRefId)
        {
            return _LoginService.GetUserRoleDetails(UserRefId);
        }

        public UmApplicationMaster GetUserApplicationDetails(int? applicationId)
        {
            return _LoginService.GetUserApplicationDetails(applicationId);
        }

        public UmMenuMaster GetUserMenuDetails(int menuId)
        {
            return _LoginService.GetUserMenuDetails(menuId);
        }


        public UmUserMaster GetUserDetails(string userName)
        {
            return _LoginService.GetUserDetails(userName);
        }

        public List<UmRoleMaster> GetRoleMasterDetails(int UserRefId)
        {
            return _LoginService.GetRoleMasterDetails(UserRefId);
        }

        public List<UmApplicationMaster> GetApplicationDetails(int roleId)
        {
            return _LoginService.GetApplicationDetails(roleId);
        }

        public MenuMaster GetMenuDetails(int roleId, int applicationId)
        {
            return _LoginService.GetMenuDetails(roleId, applicationId);
        }

        public bool LockUser(string userName)
        {
            return _LoginService.LockUser(userName);
        }

        public string ValidateUserNameForgetPassword(string userName)
        {
            return _LoginService.ValidateUserNameForgetPassword(userName);
        }

        public LoginUserDetail GetLoginUserDetails(string userName)
        {
            return _LoginService.GetLoginUserDetails(userName);
        }

        public List<MenuMaster> GetMenuDetailsByRoleId(int roleId)
        {
            return _LoginService.GetMenuDetailsByRoleId(roleId);
        }

        public List<ApplicationMaster> GetApplicationDetailsByUserId(int userId)
        {
            return _LoginService.GetApplicationDetailsByUserId(userId);
        }

        public List<MenuMaster> GetMenuDetailsByUserId(int userId)
        {
            return _LoginService.GetMenuDetailsByUserId(userId);
        }

        public bool ResetPasswordBySA(int uid, string newPassword)
        {
            return _LoginService.ResetPasswordBySA(uid, newPassword);
        }

        public Boolean SendOTPtoUser(string mobileNo, int OTP)
        {
            return _LoginService.SendOTPtoUser(mobileNo, OTP);
        }

        internal List<ServiceRequestModel> GetServiceRequestNotifications(int userid)
        {
            return _LoginService.GetServiceRequestNotifications(userid);
        }

        internal List<ServiceRequestModel> GetServiceRequestForApproval(int userid)
        {
            return _LoginService.GetServiceRequestForApproval(userid);
        }

        internal List<NoidaCustomerModel> GetNewRegisteredCustomerList(int userid)
        {
            return _LoginService.GetNewRegisteredCustomerList(userid);
        }

        internal List<ServiceTypeList> GetServiceRequestByServiceType(int userid)
        {
            return _LoginService.GetServiceRequestByServiceType(userid);
        }

        public Boolean CheckRegistationIDforNAcustomer(String RegistrationId)
        {
            return _LoginService.CheckRegistationIDforNAcustomer(RegistrationId);
        }

        public Boolean CheckEmailAddressForNAcustomer(String emailAddress)
        {
            return _LoginService.CheckEmailAddressForNAcustomer(emailAddress);
        }
    }
}