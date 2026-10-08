using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NA.PMS.Web.Controllers
{
    public class AdministrationController : Controller
    {

        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            if (Session["allMenudIdsForRoleUser"] != null && Session["CurrentUser"] != null)
            {
                return RedirectToAction("Index", "Home", new { area=""});
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

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        if (model.UserName != null && model.Password != null)
        //        {
        //            ModelState.Clear();
        //            Session["CurrentUser"] = null;
        //            var maxLoginAttempt = int.Parse(ConfigurationManager.AppSettings["MaxLoginAttempt"]);
        //            var flag = Membership.ValidateUser(model.UserName, model.Password);
        //            if (flag)
        //            {
        //                FormsAuthentication.SetAuthCookie(model.UserName, false);
        //                Session["AttemptCount"] = null;
        //                var loginUser = _PmsMembership.GetUserDetails(model.UserName);
        //                Session["Luser"] = loginUser;
        //                var userDetail = _PmsMembership.GetLoginUserDetails(model.UserName);
        //                DateTime date;
        //                var expiryDate = ConfigurationManager.AppSettings["PasswordExpiryDate"];

        //                //CurrentUserDetail currentUserDetail = null;
        //                if (loginUser != null && loginUser.IsActive == true)
        //                {
        //                    if (loginUser.ModifiedDate != null)
        //                    {

        //                        date = loginUser.ModifiedDate.Value.AddDays(int.Parse(expiryDate));
        //                        if (DateTime.Now > date)
        //                        {
        //                            return RedirectToAction("ChangePasswordExpired", "Account", new { area = "" });
        //                        }
        //                    }
        //                    else
        //                    {
        //                        date = loginUser.CreatedDate.Value.AddDays(int.Parse(expiryDate));
        //                        if (DateTime.Now > date)
        //                        {
        //                            return RedirectToAction("ChangePasswordExpired", "Account", new { area = "" });
        //                        }
        //                    }
        //                    //currentUserDetail = new CurrentUserDetail();
        //                    //currentUserDetail.UserID = loginUser.UserRefId;
        //                    //currentUserDetail.UserName = loginUser.UserName;
        //                    //currentUserDetail.FirstName = loginUser.FirstName;
        //                    //currentUserDetail.LastName = loginUser.LastName;
        //                    //currentUserDetail.LastName = loginUser.LastName;
        //                    //currentUserDetail.FullName = loginUser.FirstName + " " + loginUser.MiddleName + " " + loginUser.LastName;
        //                    //currentUserDetail.Email = loginUser.Email;
        //                    //currentUserDetail.CreatedDate = loginUser.CreatedDate;
        //                    //currentUserDetail.ModifiedDate = loginUser.ModifiedDate;
        //                    //currentUserDetail.IsActive = loginUser.IsActive;

        //                    UmRoleMaster userRole = _PmsMembership.GetUserRoleDetails(loginUser.UserRefId);

        //                    List<UmRoleMaster> roleMasters = _PmsMembership.GetRoleMasterDetails(loginUser.UserRefId);
        //                    if (userRole == null || userRole.IsActive == false)
        //                    {
        //                        return RedirectToAction("Unauthorized");
        //                    }
        //                    else
        //                    {
        //                        //List<ApplicationMaster> applicationsList = _PmsMembership.GetApplicationDetailsByUserId(loginUser.UserRefId);
        //                        //List<MenuMaster> menusList = _PmsMembership.GetMenuDetailsByUserId(loginUser.UserRefId);

        //                        //currentUserDetail.RoleMaster = userRole;
        //                        //currentUserDetail.MultiRole = roleMasters;
        //                        //currentUserDetail.ApplicationMaster = applicationsList;
        //                        //currentUserDetail.MenuMaster = menusList;

        //                        //Session["CurrentUser"] = currentUserDetail;
        //                        //Session["UserRole"] = userRole;
        //                        //Session["MultiRole"] = roleMasters;
        //                        //Session["UserApplication"] = applicationsList;

        //                        Session["UserName"] = userDetail.UserName;

        //                        Session["OTP"] = GetOTP(userDetail.Mobile);
        //                        return RedirectToAction("Login", "Account");
        //                        //if (userRole.RoleType == Constants.SuperAdmin)
        //                        //{
        //                        //    return RedirectToAction("index", "ManageUsers", new { area="SuperAdmin"}); 
        //                        //}
        //                        //else
        //                        //{
        //                        //    return RedirectToAction("index", "Home");
        //                        //}
        //                    }
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("ErrorMessage", "User is locked, please contact administrator.");
        //                    return View(model);
        //                }

        //                //return RedirectToAction("index", "Home");
        //            }
        //            else
        //            {

        //                if (Session["AttemptCount"] == null)
        //                {
        //                    Session["AttemptCount"] = 1;
        //                }
        //                else
        //                {
        //                    var attempts = (Int32)Session["AttemptCount"];
        //                    if (attempts == maxLoginAttempt)
        //                    {
        //                        attempts = maxLoginAttempt;
        //                    }
        //                    else
        //                    {
        //                        ++attempts;
        //                    }
        //                    if (attempts == maxLoginAttempt - 1)
        //                    {
        //                        ModelState.AddModelError("ErrorMessage", "This is your last attempt to enter the password.");
        //                    }
        //                    if (attempts == maxLoginAttempt)
        //                    {
        //                        var lockflag = _PmsMembership.LockUser(model.UserName);
        //                        if (lockflag)
        //                        {
        //                            ModelState.AddModelError("ErrorMessage", "Your Account has been locked, Please Contact to Administrator.");
        //                        }
        //                    }
        //                    Session["AttemptCount"] = attempts;
        //                }
        //                ModelState.AddModelError("ErrorMessage", "Invalid username or password.");
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("ErrorMessage", "Invalid username or password.");
        //        }

        //    }
        //    return View(model);
        //}


        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult LoginAfterVerifyOTP(string userName)
        //{
        //    var loginUser = _PmsMembership.GetUserDetails(userName);
        //    CurrentUserDetail currentUserDetail = null;
        //    currentUserDetail = new CurrentUserDetail();
        //    currentUserDetail.UserID = loginUser.UserRefId;
        //    currentUserDetail.UserName = loginUser.UserName;
        //    currentUserDetail.FirstName = loginUser.FirstName;
        //    currentUserDetail.LastName = loginUser.LastName;
        //    currentUserDetail.LastName = loginUser.LastName;
        //    currentUserDetail.FullName = loginUser.FirstName + " " + loginUser.MiddleName + " " + loginUser.LastName;
        //    currentUserDetail.Email = loginUser.Email;
        //    currentUserDetail.CreatedDate = loginUser.CreatedDate;
        //    currentUserDetail.ModifiedDate = loginUser.ModifiedDate;
        //    currentUserDetail.IsActive = loginUser.IsActive;
        //    currentUserDetail.UserProfile = loginUser.UserProfile;
        //    currentUserDetail.UserProfileId = loginUser.UserProfileId;
        //    currentUserDetail.OptionalId = loginUser.OptionalId;
        //    currentUserDetail.Password = loginUser.Password;
        //    UmRoleMaster userRole = _PmsMembership.GetUserRoleDetails(loginUser.UserRefId);

        //    List<UmRoleMaster> roleMasters = _PmsMembership.GetRoleMasterDetails(loginUser.UserRefId);
        //    if (userRole == null || userRole.IsActive == false)
        //    {
        //        return RedirectToAction("Unauthorized");
        //    }
        //    //else
        //    //{
        //    List<ApplicationMaster> applicationsList = _PmsMembership.GetApplicationDetailsByUserId(loginUser.UserRefId);
        //    List<MenuMaster> menusList = _PmsMembership.GetMenuDetailsByUserId(loginUser.UserRefId);
        //    List<ServiceRequestModel> requestList = _PmsMembership.GetServiceRequestNotifications(loginUser.UserRefId);
        //    List<ServiceRequestModel> ToApproveList = _PmsMembership.GetServiceRequestForApproval(loginUser.UserRefId);
        //    List<NoidaCustomerModel> CustomerModel = _PmsMembership.GetNewRegisteredCustomerList(loginUser.UserRefId);
        //    List<ServiceTypeList> RequestType = _PmsMembership.GetServiceRequestByServiceType(loginUser.UserRefId);

        //    currentUserDetail.RoleMaster = userRole;
        //    currentUserDetail.MultiRole = roleMasters;
        //    currentUserDetail.ApplicationMaster = applicationsList;
        //    currentUserDetail.MenuMaster = menusList;
        //    //currentUserDetail.ServiceRequestModel = requestList;
        //    currentUserDetail.CustomerModels = CustomerModel;

        //    Session["UserName"] = currentUserDetail.UserName;
        //    Session["CurrentUser"] = currentUserDetail;
        //    Session["UserRole"] = userRole;
        //    Session["MultiRole"] = roleMasters;
        //    Session["UserApplication"] = applicationsList;
        //    Session["ServiceRequest"] = requestList;
        //    Session["NewCustomer"] = CustomerModel;
        //    Session["RequestType"] = RequestType;
        //    //Session["OTP"] = GetOTP(userDetail.Mobile);
        //    //return RedirectToAction("Login", "Account");
        //    if (userRole.RoleType == Constants.SuperAdmin)
        //    {
        //        return Json(false);
        //    }
        //    else
        //    {
        //        return Json(true);
        //    }
        //}
    }
}
