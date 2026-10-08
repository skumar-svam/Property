using NA.PMS.Common.Helpers;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace NA.PMS.Web.Areas.Customer.Controllers
{
    public class PISAccountController : Controller
    {

        PISMembershipProvider _pisMembership = null;
        public PISAccountController()
        {
            _pisMembership = new PISMembershipProvider();
        }

        // GET: Customer/Account
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// open login page only
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Session["CurrentCustomer"] != null)
            {
                return RedirectToAction("Index", "Property", new { area="Customer"});
            }
            else
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(UserViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                Session["allMenudIdsForRoleUser_PIS"] = null;
                
                var flag = Membership.ValidateUser(model.UserName, model.Password);//Membership not working
                flag = _pisMembership.ValidateUser(model.UserName, model.Password);
                if (flag)
                {
                    Session["AttemptCount"] = null;
                    //var userRecord = _membership.GetUserDetails(model.UserName);
                    var customer = _pisMembership.GetLoginUserDetails(model.UserName);
                    if (customer.IsActive == true && customer.IsLocked == false)
                    {
                        var serializer = new JavaScriptSerializer();
                        string userData = serializer.Serialize(customer);

                        Session["CurrentCustomer"] = customer;
                        var authTicket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.Add(FormsAuthentication.Timeout), (model.IsRemembered==null ? false : true), userData, FormsAuthentication.FormsCookiePath);

                        string encTicket = FormsAuthentication.Encrypt(authTicket);
                        HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                        Response.Cookies.Add(faCookie);

                        //return Redirect("/Customer/Property/Index");
                        return RedirectToAction("Index", "Property", new { area = "Customer" });
                    }
                    else
                    {
                        if ((bool)customer.IsLocked)  ModelState.AddModelError("ErrorMessage", "Your Account has been locked, contact Administration.");
                        if (customer.IsActive == false)  ModelState.AddModelError("ErrorMessage", "User is not active.");
                    }
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
                        attempts++;
                        if (attempts == 4) 
                        { 
                            ModelState.AddModelError("ErrorMessage", "This is your last attempt to enter the password."); 
                        }
                        else if (attempts == 5)
                        {
                            _pisMembership.LockUser(model.UserName);
                            ModelState.AddModelError("ErrorMessage", "Your Account has been locked, Please Contact to Administration.");
                        }
                        
                        Session["AttemptCount"] = attempts;
                    }
                    ModelState.AddModelError("ErrorMessage", "Invalid username or password.");
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// input user name and password to login 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<ActionResult> Login(LoginViewModel_PIS model, string returnUrl)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        ModelState.Clear();
        //        Session["allMenudIdsForRoleUser_PIS"] = null;
        //        var _membership = new PISMembershipProvider();
        //        var flag = Membership.ValidateUser(model.UserName, model.Password);//Membership not working
        //        flag = _membership.ValidateUser(model.UserName, model.Password);
        //        if (flag)
        //        {
        //            Session["AttemptCount"] = null;
        //            var userRecord = _membership.GetUserDetails(model.UserName);

        //            if (userRecord.Status != null && ((bool)!userRecord.IsLockedOut && userRecord.Status.Value))
        //            {
        //                var currentUserDetail = new CurrentUserDetail_PIS();
        //                currentUserDetail.UserID = userRecord.UserId;
        //                currentUserDetail.FirstName = userRecord.FirstName;
        //                currentUserDetail.LastName = userRecord.LastName;
        //                currentUserDetail.Email = model.Email;
        //                currentUserDetail.RegistrationId = userRecord.PropertyId;
        //                currentUserDetail.Mobile = userRecord.MobileNo;
        //                currentUserDetail.Email = userRecord.UserEmail;

        //                currentUserDetail.FullName = String.Format("{0} {1}", userRecord.FirstName, userRecord.LastName);

        //                var serializer = new JavaScriptSerializer();
        //                string userData = serializer.Serialize(currentUserDetail);

        //                Session["CurrentUser_PIS"] = currentUserDetail;
        //                Session["UserName_PIS"] = currentUserDetail.FirstName;
        //                Session["RegistrationID_PIS"] = currentUserDetail.RegistrationId;
        //                Session["Roles_PIS"] = _pisMembership.GetRoleForUser(model.UserName);
        //                var roles = _pisMembership.GetRoleForUser(model.UserName);
        //                Session["RoleID_PIS"] = _pisMembership.GetRoleForUser(model.UserName).RoleId.ToString();
        //                Session["RoleName_PIS"] = roles.RoleName;
        //                Session["DeptId_PIS"] = userRecord.DeptId;
        //                if (userRecord.DeptId != 0)
        //                {
        //                    IList<DtoList> list = null;
        //                    list = _pisMembership.GetPropertyType();
        //                    foreach (DtoList lst in list)
        //                    {
        //                        if (lst.value == userRecord.DeptId.ToString())
        //                            Session["DeptName_PIS"] = lst.label;
        //                    }
        //                }
        //                var authTicket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.Add(FormsAuthentication.Timeout), model.RememberMe, userData, FormsAuthentication.FormsCookiePath);

        //                string encTicket = FormsAuthentication.Encrypt(authTicket);
        //                HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
        //                Response.Cookies.Add(faCookie);
        //                var customUrl = "/Customer/Property/";

        //                //if (Session["RoleID_PIS"].Equals("1") || Session["RoleID_PIS"].Equals("4") || Session["RoleID_PIS"].Equals("5"))
        //                //{
        //                //    return RedirectToLocal(returnUrl);
        //                //}
        //                //else
        //                //{
        //                    return Redirect(customUrl);
        //                //}
        //            }
        //            else
        //            {
        //                if ((bool)userRecord.IsLockedOut)
        //                    ModelState.AddModelError("ErrorMessage", "Your Account has been locked, Please Contact to Administration.");
        //                if (userRecord.Status != null && !userRecord.Status.Value)
        //                    ModelState.AddModelError("ErrorMessage", "User is inactive.");
        //            }
        //        }
        //        else
        //        {
        //            if (Session["AttemptCount"] == null)
        //            {
        //                Session["AttemptCount"] = 1;
        //            }
        //            else
        //            {
        //                var attempts = (Int32)Session["AttemptCount"];

        //                if (attempts == 5)
        //                {
        //                    attempts = 5;
        //                }
        //                else
        //                {
        //                    ++attempts;
        //                }
        //                if (attempts == 4)
        //                {
        //                    ModelState.AddModelError("ErrorMessage", "This is your last attempt to enter the password.");
        //                }
        //                if (attempts == 5)
        //                {
        //                    attempts = 5;
        //                    _pisMembership.LockUser(model.UserName);
        //                    ModelState.AddModelError("ErrorMessage", "Your Account has been locked, Please Contact to Administration.");
        //                }
        //                Session["AttemptCount"] = attempts;

        //            }
        //            ModelState.AddModelError("ErrorMessage", "Invalid username or password.");
        //        }
        //    }
        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        public ActionResult Logout()
        {
            Session["allMenudIdsForRoleUser_PIS"] = null;
            Session["UserName_PIS"] = null;
            Session["RoleName_PIS"] = null;

            Session["CurrentCustomer"] = null;
            FormsAuthentication.SignOut();
            Session.Abandon();
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);
            Session.Abandon();
            return RedirectToAction("Login", "PISAccount", new { area = "Customer" });
        }

    }
}