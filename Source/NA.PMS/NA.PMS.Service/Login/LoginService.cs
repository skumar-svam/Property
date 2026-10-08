
using NA.PMS.Model;
using NA.PMS.Model.Login;
using NA.PMS.Repository;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Service
{
    public class LoginService : ILoginService
    {
        private ILoginRepository _loginRepository = null;
        IMenuMappingService menuMappingService = null;

        public LoginService()
        {
            _loginRepository = new LoginRepository();
            this.menuMappingService = new MenuMappingService();
        }

        public bool ValidateUser(string userName, string password)
        {
            return _loginRepository.ValidateUser(userName, password);
        }

        public UserTypeAndIdVM ValidateUserGetTypeAndId(string userName, string password)
        {
            //return _loginRepository.ValidateUserGetTypeAndId(userName, password);
            var userTypeAndId = new UserTypeAndIdVM();
            Web.Models.CurrentUserDetail userDetails = null;
            if (!string.IsNullOrEmpty(userName))
            {
                bool _flag = _loginRepository.ValidateUserGetTypeAndId(userName, password);
                if (_flag == true)
                {
                    userTypeAndId.IsUserExist = true;
                    var loginUser = GetUserDetails(userName);
                    if (loginUser != null)
                    {
                        userDetails = new Web.Models.CurrentUserDetail();
                        MapUserDetails(userDetails, loginUser);// Map User Details
                        if (loginUser.UserRefId > 0)
                        {
                            userTypeAndId.UserID = loginUser.UserRefId;
                            var multiRole = GetRoleMasterDetails(loginUser.UserRefId);

                            userTypeAndId.UserRoleType = multiRole.Select(x => x.RoleType).FirstOrDefault();
                        }
                    }
                }
                else
                {
                    userTypeAndId.IsUserExist = false;
                }
            }
            return userTypeAndId;
        }

        public bool ValidateUserName(string userName)
        {
            return _loginRepository.ValidateUserName(userName);
        }

        public bool ChangePassword(string email, string newPassword)
        {
            return _loginRepository.ChangePassword(email, newPassword);
        }

        public bool ChangePassword(string userName, string email, string newPassword)
        {
            return _loginRepository.ChangePassword(userName, email, newPassword);
        }

        public bool CheckEmailAddressExists(string emailAddress)
        {
            return _loginRepository.CheckEmailAddressExists(emailAddress);
        }

        public bool LockUser(string userName)
        {
            return _loginRepository.LockUser(userName);
        }

        public bool ForgotPassword(string userName, string email, string mobileNo, int otp)
        {
            return _loginRepository.ForgotPassword(userName, email, mobileNo, otp);
        }


        public ViewUmUserMaster GetViewUserDetails(String email)
        {
            return _loginRepository.GetViewUserDetails(email);
        }

        public UmRoleMaster GetUserRoleDetails(int? UserRefId)
        {
            return _loginRepository.GetUserRoleDetails(UserRefId);
        }

        public UmApplicationMaster GetUserApplicationDetails(int? applicationId)
        {
            return _loginRepository.GetUserApplicationDetails(applicationId);
        }

        public UmMenuMaster GetUserMenuDetails(int menuId)
        {
            return _loginRepository.GetUserMenuDetails(menuId);
        }

        public bool SendOTPtoUser(string mobileNo, int OTP)
        {
            return _loginRepository.SendOTPtoUser(mobileNo, OTP);
        }



        public UmUserMaster GetUserDetails(string userName)
        {
            return _loginRepository.GetUserDetails(userName);
        }


        public List<UmRoleMaster> GetRoleMasterDetails(int UserRefId)
        {
            return _loginRepository.GetRoleMasterDetails(UserRefId);
        }


        //UmRoleMaster ILoginService.GetRoleMasterDetails(int userId)
        //{
        //    throw new NotImplementedException();
        //}

        public List<UmApplicationMaster> GetApplicationDetails(int roleId)
        {
            return _loginRepository.GetApplicationDetails(roleId);
        }


        public MenuMaster GetMenuDetails(int roleId, int applicationId)
        {
            return _loginRepository.GetMenuDetails(roleId, applicationId);
        }


        public string ValidateUserNameForgetPassword(string userName)
        {
            return _loginRepository.ValidateUserNameForgetPassword(userName);
        }

        public LoginUserDetail GetLoginUserDetails(string userName)
        {
            return _loginRepository.GetLoginUserDetails(userName);
        }


        public List<MenuMaster> GetMenuDetailsByRoleId(int roleId)
        {
            return _loginRepository.GetMenuDetailsByRoleId(roleId);
        }


        public List<ApplicationMaster> GetApplicationDetailsByUserId(int userId)
        {
            return _loginRepository.GetApplicationDetailsByUserId(userId);
        }
        #region ValidateUser Keshav 14 Sep 2018
        public bool ValidateUserIsExist(string userName, string applicationName)
        {
            return _loginRepository.ValidateUserIsExist(userName, applicationName);
        }
        // Keshav 14 Sep 2018
        #endregion
        public List<MenuMaster> GetMenuDetailsByUserId(int userId)
        {
            return _loginRepository.GetMenuDetailsByUserId(userId);
        }


        public bool ResetPasswordBySA(int uid, string newPassword)
        {
            return _loginRepository.ResetPasswordBySA(uid, newPassword);
        }


        public bool IsUserisActive(int userid)
        {
            return _loginRepository.IsUserisActive(userid);
        }


        public bool IsUserisActive()
        {
            return _loginRepository.IsUserisActive();
        }


        public List<ServiceRequestModel> GetServiceRequestNotifications(int userid)
        {
            return _loginRepository.GetServiceRequestNotifications(userid);
        }


        public List<ServiceRequestModel> GetServiceRequestForApproval(int userid)
        {
            return _loginRepository.GetServiceRequestForApproval(userid);
        }


        public List<NoidaCustomerModel> GetNewRegisteredCustomerList(int userid)
        {
            return _loginRepository.GetNewRegisteredCustomerList(userid);
        }

        public List<ServiceTypeList> GetServiceRequestByServiceType(int userid)
        {
            return _loginRepository.GetServiceRequestByServiceType(userid);
        }

        public Boolean CheckRegistationIDforNAcustomer(String RegistrationId)
        {
            return _loginRepository.CheckRegistationIDforNAcustomer(RegistrationId);
        }

        public Boolean CheckEmailAddressForNAcustomer(String emailAddress)
        {
            return _loginRepository.CheckEmailAddressForNAcustomer(emailAddress);
        }

        #region Module User Data

        public Web.Models.CurrentUserDetail GetUserDetailsByApplication(string userName, int applicationId, string applicationName)
        {
            Web.Models.CurrentUserDetail userDetails = null;
            if (!string.IsNullOrEmpty(userName))
            {
                bool _flag = ValidateUserIsExist(userName, applicationName);
                if (_flag == true)
                {
                    var loginUser = GetUserDetails(userName);
                    if (loginUser != null)
                    {
                        userDetails = new Web.Models.CurrentUserDetail();
                        MapUserDetails(userDetails, loginUser);// Map User Details
                        if (loginUser.UserRefId > 0)
                        {
                            userDetails.ApplicationMaster = GetApplicationDetailsByUserId(loginUser.UserRefId);
                            userDetails.MenuMaster = GetMenuDetailsByUserId(loginUser.UserRefId);
                            //userDetails.MultiRole = GetRoleMasterDetails(loginUser.UserRefId);
                           // userDetails.RoleMaster = GetUserRoleDetails(loginUser.UserRefId);
                          
                            var multiRole = GetRoleMasterDetails(loginUser.UserRefId);

                            userDetails.RoleId = multiRole.Select(x => x.RoleId).FirstOrDefault();
                            userDetails.RoleType = multiRole.Select(x => x.RoleType).FirstOrDefault();
                            userDetails.RequestList = GetMenuIdsForUserName(userDetails, multiRole);

                           
                        }
                    }
                }
            }
            return userDetails;
        }

        public List<int> GetMenuIdsForUserName(Web.Models.CurrentUserDetail userDetails,List<UmRoleMaster> multiRole)
        {
            var currentUserDetail = userDetails;
            var allMenudIdsForRoleUser = new List<int>();
            if (currentUserDetail != null)
            {
                var lstGetMenuID = new List<GetMenuID>();
                List<int> roleIds = new List<int>();
                foreach (var objMultiRole in multiRole)
                {
                    roleIds.Add(objMultiRole.RoleId);
                }
                if (currentUserDetail.RoleId != 0)
                {
                    lstGetMenuID = menuMappingService.GetMenuIdsByRoleID(roleIds);
                    var lstMenus = lstGetMenuID.Select(e => e.parentID).Distinct().ToList();
                    foreach (var item in lstMenus)
                    {
                        allMenudIdsForRoleUser.Add(item);
                    }
                    foreach (GetMenuID item in lstGetMenuID)
                    {
                        allMenudIdsForRoleUser.Add(item.menuID);
                    }
                }
            }

            return allMenudIdsForRoleUser;
        }

        private void MapUserDetails(Web.Models.CurrentUserDetail currentUserDetail, UmUserMaster loginUser)
        {
            currentUserDetail.UserID = loginUser.UserRefId;
            currentUserDetail.UserName = loginUser.UserName;
            currentUserDetail.FirstName = loginUser.FirstName;
            currentUserDetail.LastName = loginUser.LastName;
            currentUserDetail.LastName = loginUser.LastName;
            currentUserDetail.FullName = loginUser.FirstName + " " + loginUser.MiddleName + " " + loginUser.LastName;
            currentUserDetail.Email = loginUser.Email;
            currentUserDetail.CreatedDate = loginUser.CreatedDate;
            currentUserDetail.ModifiedDate = loginUser.ModifiedDate;
            currentUserDetail.IsActive = loginUser.IsActive;
            currentUserDetail.UserProfile = loginUser.UserProfile;
            currentUserDetail.UserProfileId = loginUser.UserProfileId;
            currentUserDetail.OptionalId = loginUser.OptionalId;
        }

        #endregion


        public int SendAndValidateOTP(UserViewModel model)
        {
            return _loginRepository.SendAndValidateOTP(model);
        }
    }
}

