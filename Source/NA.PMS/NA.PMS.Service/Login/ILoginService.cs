
using NA.PMS.Model;
using NA.PMS.Model.Entities;
using NA.PMS.Model.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Service
{
    public interface ILoginService
    {
        Boolean ValidateUser(String userName, String password);

        Boolean ChangePassword(String email, String newPassword);

        bool ChangePassword(string userName, string email, string newPassword);

        Boolean CheckEmailAddressExists(String emailAddress);

        Boolean LockUser(String userName);

        bool ForgotPassword(string userName, string email, string mobileNo, int otp);

        bool ValidateUserName(string userName);

        ViewUmUserMaster GetViewUserDetails(String email);

        UmRoleMaster GetUserRoleDetails(int? UserRefId);

        UmApplicationMaster GetUserApplicationDetails(int? applicationId);

        UmMenuMaster GetUserMenuDetails(int menuId);


        UmUserMaster GetUserDetails(string userName);

        List<UmRoleMaster> GetRoleMasterDetails(int UserRefId);

        List<UmApplicationMaster> GetApplicationDetails(int roleId);

        MenuMaster GetMenuDetails(int roleId, int applicationId);
        //bool LockUser(string userName);


        string ValidateUserNameForgetPassword(string userName);

        LoginUserDetail GetLoginUserDetails(string userName);

        List<MenuMaster> GetMenuDetailsByRoleId(int roleId);

        List<ApplicationMaster> GetApplicationDetailsByUserId(int userId);

        List<MenuMaster> GetMenuDetailsByUserId(int userId);

        bool ResetPasswordBySA(int uid, string newPassword);

        bool IsUserisActive(int userid);

        bool IsUserisActive();

        bool SendOTPtoUser(string mobileNo, int OTP);

        List<ServiceRequestModel> GetServiceRequestNotifications(int userid);

        List<ServiceRequestModel> GetServiceRequestForApproval(int userid);

        List<NoidaCustomerModel> GetNewRegisteredCustomerList(int userid);

        List<ServiceTypeList> GetServiceRequestByServiceType(int userid);

        Boolean CheckRegistationIDforNAcustomer(String RegistrationId);
        Boolean CheckEmailAddressForNAcustomer(String emailAddress);
        Web.Models.CurrentUserDetail GetUserDetailsByApplication(string userName, int applicationId, string applicationName);
        bool ValidateUserIsExist(string userName, string applicationName);

        UserTypeAndIdVM ValidateUserGetTypeAndId(string userName, string password);

        int SendAndValidateOTP(UserViewModel model);
    }
}
