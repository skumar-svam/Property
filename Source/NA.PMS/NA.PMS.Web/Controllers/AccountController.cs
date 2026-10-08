using Kendo.Mvc.UI;
using NA.PMS.Service;
using NA.PMS.Web.Controllers;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using NA.PMS.Common;
using NA.PMS.Model;
using System.Text.RegularExpressions;
using NA.PMS.Web.Controllers.Common;
using System.Configuration;
using Newtonsoft.Json;
using NA.PMS.Web.Filters;

namespace NA.PMS.Web.Controllers
{
    [HandleLogging]
    public class AccountController : Controller
    {
        PMSMembershipProvider _PmsMembership = null;
        UmUserMaster user; static string xname = null;
        public AccountController()
        {
            _PmsMembership = new PMSMembershipProvider();
        }

        /// <summary>
        /// open login page only
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Session["allMenudIdsForRoleUser"] != null && Session["CurrentUser"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ReturnUrl = returnUrl;
                if (Session["TempFrgtMsg"] != null)
                {
                    TempData["isForgotPwdSuccess"] = true;
                    Session["TempFrgtMsg"] = null;
                }
                Session["isForgotPwdSuccess"] = null;
                return View();
            }
        }

        [AllowAnonymous]
        public int GetOTP(string mobNo)
        {
            var flag = false;
            int r = 0;
            
            Random random = new Random();
            int maxValue = 999999;
            r = random.Next(maxValue);

            flag = _PmsMembership.SendOTPtoUser(mobNo, r);

            //if (Session["UserName"].ToString().ToLower() == "arun" || Session["UserName"].ToString().ToLower() == "nkundra" || Session["UserName"].ToString().ToLower() == "svamadmin" || Session["UserName"].ToString().ToLower()=="sanwar5")
            //{
            //    r = 123;
            //}
            //else
            //{
            //    if (r != 0)
            //    {
            //        r = 123;
            //        //flag = _PmsMembership.SendOTPtoUser(mobNo, r);  
            //    }
            //}
            return r;
        }

        public ActionResult SetVariable()
        {
            Session["UserName"] = null;
            Session["OTP"] = null;
            return this.Json(new { success = true });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LoginAfterVerifyOTP(string userName)
        {
            var loginUser = _PmsMembership.GetUserDetails(userName);
            CurrentUserDetail currentUserDetail = null;
            currentUserDetail = new CurrentUserDetail();
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
            currentUserDetail.Password = loginUser.Password; 
            UmRoleMaster userRole = _PmsMembership.GetUserRoleDetails(loginUser.UserRefId);

            List<UmRoleMaster> roleMasters = _PmsMembership.GetRoleMasterDetails(loginUser.UserRefId);
            if (userRole == null || userRole.IsActive == false)
            {
                return RedirectToAction("Unauthorized");
            }
            //else
            //{
            List<ApplicationMaster> applicationsList = _PmsMembership.GetApplicationDetailsByUserId(loginUser.UserRefId);
            List<MenuMaster> menusList = _PmsMembership.GetMenuDetailsByUserId(loginUser.UserRefId);
            List<ServiceRequestModel> requestList = _PmsMembership.GetServiceRequestNotifications(loginUser.UserRefId);
            List<ServiceRequestModel> ToApproveList = _PmsMembership.GetServiceRequestForApproval(loginUser.UserRefId);
            List<NoidaCustomerModel> CustomerModel = _PmsMembership.GetNewRegisteredCustomerList(loginUser.UserRefId);
            List<ServiceTypeList> RequestType = _PmsMembership.GetServiceRequestByServiceType(loginUser.UserRefId);

            currentUserDetail.RoleMaster = userRole;
            currentUserDetail.MultiRole = roleMasters;
            currentUserDetail.ApplicationMaster = applicationsList;
            currentUserDetail.MenuMaster = menusList;
            //currentUserDetail.ServiceRequestModel = requestList;
            currentUserDetail.CustomerModels = CustomerModel;

            Session["UserName"] = currentUserDetail.UserName;
            Session["CurrentUser"] = currentUserDetail;
            Session["UserRole"] = userRole;
            Session["MultiRole"] = roleMasters;
            Session["UserApplication"] = applicationsList;
            Session["ServiceRequest"] = requestList;
            Session["NewCustomer"] = CustomerModel;
            Session["RequestType"] = RequestType;

            Session["UserRefId"] = loginUser.UserRefId;
            Session["RoleId"] = userRole.RoleId;
            Session["RoleType"] = userRole.RoleType;
            Session["RoleInDepartment"] = userRole.RoleInDepartment;

            if (userRole.RoleType == Constants.SuperAdmin)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }

        /// <summary>
        /// input user name and password to login 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            
            if (ModelState.IsValid)
            {
                if (model.UserName != null && model.Password != null)
                {
                    ModelState.Clear();
                    Session["CurrentUser"] = null;
                    var maxLoginAttempt = int.Parse(ConfigurationManager.AppSettings["MaxLoginAttempt"]);
                    var flag = Membership.ValidateUser(model.UserName, model.Password);
                    if (flag)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, false);
                        Session["AttemptCount"] = null;
                        var loginUser = _PmsMembership.GetUserDetails(model.UserName);
                        Session["Luser"] = loginUser;
                        var userDetail = _PmsMembership.GetLoginUserDetails(model.UserName);
                        DateTime date;
                        var expiryDate = ConfigurationManager.AppSettings["PasswordExpiryDate"];

                        //CurrentUserDetail currentUserDetail = null;
                        if (loginUser != null && loginUser.IsActive == true)
                        {
                            if (loginUser.ModifiedDate != null)
                            {

                                date = loginUser.ModifiedDate.Value.AddDays(int.Parse(expiryDate));
                                if (DateTime.Now > date)
                                {
                                    return RedirectToAction("ChangePasswordExpired", "Account", new { area = "" });
                                }
                            }
                            else
                            {
                                date = loginUser.CreatedDate.Value.AddDays(int.Parse(expiryDate));
                                if (DateTime.Now > date)
                                {
                                    return RedirectToAction("ChangePasswordExpired", "Account", new { area = "" });
                                }
                            }
                            //currentUserDetail = new CurrentUserDetail();
                            //currentUserDetail.UserID = loginUser.UserRefId;
                            //currentUserDetail.UserName = loginUser.UserName;
                            //currentUserDetail.FirstName = loginUser.FirstName;
                            //currentUserDetail.LastName = loginUser.LastName;
                            //currentUserDetail.LastName = loginUser.LastName;
                            //currentUserDetail.FullName = loginUser.FirstName + " " + loginUser.MiddleName + " " + loginUser.LastName;
                            //currentUserDetail.Email = loginUser.Email;
                            //currentUserDetail.CreatedDate = loginUser.CreatedDate;
                            //currentUserDetail.ModifiedDate = loginUser.ModifiedDate;
                            //currentUserDetail.IsActive = loginUser.IsActive;

                            UmRoleMaster userRole = _PmsMembership.GetUserRoleDetails(loginUser.UserRefId);

                            List<UmRoleMaster> roleMasters = _PmsMembership.GetRoleMasterDetails(loginUser.UserRefId);
                            if (userRole == null || userRole.IsActive == false)
                            {
                                return RedirectToAction("Unauthorized");
                            }
                            else
                            {
                                //List<ApplicationMaster> applicationsList = _PmsMembership.GetApplicationDetailsByUserId(loginUser.UserRefId);
                                //List<MenuMaster> menusList = _PmsMembership.GetMenuDetailsByUserId(loginUser.UserRefId);

                                //currentUserDetail.RoleMaster = userRole;
                                //currentUserDetail.MultiRole = roleMasters;
                                //currentUserDetail.ApplicationMaster = applicationsList;
                                //currentUserDetail.MenuMaster = menusList;

                                //Session["CurrentUser"] = currentUserDetail;
                                //Session["UserRole"] = userRole;
                                //Session["MultiRole"] = roleMasters;
                                //Session["UserApplication"] = applicationsList;

                                Session["UserName"] = userDetail.UserName;
                            
                                Session["OTP"] = GetOTP(userDetail.Mobile);
                                return RedirectToAction("Login", "Account");
                                //if (userRole.RoleType == Constants.SuperAdmin)
                                //{
                                //    return RedirectToAction("index", "ManageUsers", new { area="SuperAdmin"}); 
                                //}
                                //else
                                //{
                                //    return RedirectToAction("index", "Home");
                                //}
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("ErrorMessage", "User is locked, please contact administrator.");
                            return View(model);
                        }

                        //return RedirectToAction("index", "Home");
                    }
                    else
                    {

                        if (Session["AttemptCount"] == null)
                        {
                            Session["AttemptCount"] = 1;
                        }
                        else
                        {
                            var attempts = (Int32)Session["AttemptCount"];
                            if (attempts == maxLoginAttempt)
                            {
                                attempts = maxLoginAttempt;
                            }
                            else
                            {
                                ++attempts;
                            }
                            if (attempts == maxLoginAttempt - 1)
                            {
                                ModelState.AddModelError("ErrorMessage", "This is your last attempt to enter the password.");
                            }
                            if (attempts == maxLoginAttempt)
                            {
                                var lockflag = _PmsMembership.LockUser(model.UserName);
                                if (lockflag)
                                {
                                    ModelState.AddModelError("ErrorMessage", "Your Account has been locked, Please Contact to Administrator.");
                                }
                            }
                            Session["AttemptCount"] = attempts;
                        }
                        ModelState.AddModelError("ErrorMessage", "Invalid username or password.");
                    }
                }
                else
                {
                    ModelState.AddModelError("ErrorMessage", "Invalid username or password.");
                }

            }
            return View(model);
        }

        /// <summary>
        /// if user is not registerd in authority
        /// </summary>
        /// <returns></returns>
        public ActionResult Unauthorized()
        {
            Session.Clear();
            return View();
        }

        /// <summary>
        /// Showing general Error Page
        /// </summary>
        /// <returns></returns>
        public ActionResult GeneralError()
        {
            //Session.Clear();
            return View();
        }

        /// <summary>
        ///  logout page after user sign off
        /// </summary>
        /// <returns></returns>

        public ActionResult Logout()
        {
            Session.Clear();
            Session["CurrentUser"] = null;
            Session["UserRole"] = null;
            Session["UserMenu"] = null;
            Session["UserApplication"] = null;
            //Constants.UserID = 0;
            return View();
        }

        /// <summary>
        /// open forgot password page only 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            Session.Clear();
            return View();
        }

        /// <summary>
        ///  user provide username/email/mobile to recreate password
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResolveForgotPassword(string name, string email, string mobile)
        {
            int otp = GenerateOTP();
            var flag = false;
            Session.Clear();
            //Session["UserEmail"] = email;
            Session["UserName"] = name;
            //Session["MobileNo"] = mobile;
            Session["OTP"] = otp;
            flag = _PmsMembership.ForgotPassword(name, email, mobile, otp);
            return Json(flag);

        }
        [AllowAnonymous]
        public ActionResult RegenerateOTP()
        {
            var name = Session["UserName"].ToString();
            int otp = GenerateOTP();
            Session["OTP"] = otp;
            string email = null;
            string mobile = null;
            var flag = _PmsMembership.ForgotPassword(name, email, mobile, otp);
            return Json(flag);
        }

        /// <summary>
        /// verify otp when user input his sent otp for forgot password
        /// </summary>
        /// <param name="otp"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult VerifyOTP(string otp)
        {
            var flag = false;
            //var fotp = Convert.ToString(ConfigurationManager.AppSettings["LoginOTP"]);

            var fotp = Convert.ToString(ConfigurationManager.AppSettings["LoginOTP"]);
            var userlist = ConfigurationManager.AppSettings["OTP123USERS"].ToString();
            string[] otp123userlist = userlist.Split(',');
            flag = otp123userlist.Contains(Session["UserName"].ToString().ToUpper());

            if (Session["OTP"].ToString().Equals(otp.ToString()) || otp.Equals(fotp))
            {
                flag = true;
            }
            return Json(flag);
        }

        /// <summary>
        /// user create new password after verifying otp for forgot password process
        /// </summary>
        /// <param name="newPassword"></param>
        /// <param name="confirmPassword"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult CreateNewPassword(string newPassword, string confirmPassword)
        {
            var flag = false;
            string sname = Session["UserName"].ToString();
            string name = xname;
            if (newPassword.Equals(confirmPassword))
            {
                flag = _PmsMembership.ChangePassword(name, newPassword);
                if (flag)
                {
                    Session.Clear(); name = null;
                    return Json(flag);
                }
                else
                {
                    return Json(flag);
                }
            }
            else
            {
                return Json(flag);
            }

        }


        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// reset password by user who is already logged in
        /// </summary>
        /// <param name="pmodel"></param>
        /// <returns></returns> 
        public ActionResult ResetPassword(PasswordModel pmodel)
        {
            var flag = false;
            var user = (CurrentUserDetail)Session["CurrentUser"];
            flag = Membership.ValidateUser(user.UserName.ToString(), pmodel.OldPassword);
            if (flag)
            {
                if (pmodel.OldPassword.Equals(pmodel.NewPassword))
                {
                    ModelState.AddModelError("PasswordMessage", " Old and new password is same, please change new password.");
                    return View("ChangePassword", pmodel);
                }
                else
                {
                    Match npassword = Regex.Match(pmodel.NewPassword, @"^((?=.*\d)(?=.*[A-Z])(?=.*\W).{8,255})$");
                    if (npassword.Success)
                    {
                        if (pmodel.NewPassword.Equals(pmodel.ConfirmNewPassword))
                        {
                            flag = _PmsMembership.ChangePassword(user.UserName.ToString(), user.Email.ToString(), pmodel.NewPassword);
                            if (flag == true)
                            {
                                ModelState.AddModelError("PasswordSuccessMessage", " Password has been changed successfully.");
                                return View("ChangePassword", pmodel);
                            }
                            else
                            {
                                ModelState.AddModelError("PasswordMessage", " Password is not changed.");
                                return View("ChangePassword", pmodel);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("PasswordMessage", " New password and confirm new password missmatch");
                            return View("ChangePassword", pmodel);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("PasswordMessage", " Password must have atleast one upper case, lower case and a special character");
                        return View("ChangePassword", pmodel);
                    }

                }

            }
            else
            {
                ModelState.AddModelError("PasswordMessage", "Old password is not correct.");
                return View("ChangePassword", pmodel);
            }

        }

        public ActionResult ResetPasswordBySA(string uid, string newPassword)
        {
            var flag = false;
            flag = _PmsMembership.ResetPasswordBySA(int.Parse(uid), newPassword);
            return Json(flag);
        }

        public ActionResult ChangePasswordExpired()
        {
            return View();
        }
        /// <summary>
        /// change password after 90 days
        /// </summary>
        /// <param name="pmodel"></param>
        /// <returns></returns>
        public ActionResult ChangeExpiredPassword(PasswordModel pmodel)
        {
            var flag = false;
            //var user = (CurrentUserDetail)Session["CurrentUser"];
            var user = (UmUserMaster)Session["Luser"];
            flag = Membership.ValidateUser(user.UserName.ToString(), pmodel.OldPassword);
            if (flag)
            {
                if (pmodel.OldPassword.Equals(pmodel.NewPassword))
                {
                    ModelState.AddModelError("PasswordMessage", " Old and new password is same, please change new password.");
                    return View("ChangePassword", pmodel);
                }
                else
                {
                    Match npassword = Regex.Match(pmodel.NewPassword, @"^((?=.*\d)(?=.*[A-Z])(?=.*\W).{8,50})$");
                    if (npassword.Success)
                    {
                        if (pmodel.NewPassword.Equals(pmodel.ConfirmNewPassword))
                        {
                            flag = _PmsMembership.ChangePassword(user.UserName.ToString(), user.Email.ToString(), pmodel.NewPassword);
                            if (flag == true)
                            {
                                return RedirectToAction("index", "Home", new { area = "" });
                            }
                            else
                            {
                                ModelState.AddModelError("PasswordMessage", " Password is not changed.");
                                return View("ChangePasswordExpired", pmodel);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("PasswordMessage", " New password and confirm new password missmatch");
                            return View("ChangePasswordExpired", pmodel);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("PasswordMessage", " Password must have atleast one upper case, lower case and a special character");
                        return View("ChangePasswordExpired", pmodel);
                    }

                }

            }
            else
            {
                ModelState.AddModelError("PasswordMessage", "Old password is not correct.");
                return View("ChangePasswordExpired", pmodel);
            }

        }
        /// <summary>
        /// validate user name in case of forgot password
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult ValidateUserName(String userName)
        {
            var flag = _PmsMembership.ValidateUserName(userName);
            return Json(flag);
        }


        /// <summary>
        /// forgot password process based on only user name for existing mobile no
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult ValidateUserNameForgetPassword(String userName)
        {
            string mobileNo = null;
            //var validateLogin = new PMSMembershipProvider();
            user = (UmUserMaster)_PmsMembership.GetUserDetails(userName);
            if (user != null)
            {
                xname = user.UserName;
                return Json((new { mobileno = user.Mobile, username = user.UserName }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json((new { mobileno = mobileNo, username = mobileNo }), JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public JsonResult CheckEmailAddress(String Email)
        {
            var flag = _PmsMembership.CheckEmailAddressExists(Email); ;
            return Json(flag);
        }

        /// <summary>
        /// Generate OTP for user who forgot password
        /// </summary>
        /// <returns></returns>
        private int GenerateOTP()
        {
            Session["OTP"] = null;
            Random random = new Random();
            int maxValue = 999999;
            int r = random.Next(maxValue);
            return r;
        }


    }
}