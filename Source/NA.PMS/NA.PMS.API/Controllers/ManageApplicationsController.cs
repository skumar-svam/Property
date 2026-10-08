using NA.PMS.Model.Login;
using NA.PMS.Service;
using NA.PMS.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NA.PMS.API.Controllers
{
    public class ManageApplicationsController : ApiController
    {
        ILoginService _loginService = null;
        IManageUsersService _manageUsersService = null;
        IPIMSAPIService _ipimsapiService = null;
        public ManageApplicationsController()
        {
            this._loginService = new LoginService();
            this._manageUsersService = new ManageUsersService();
            this._ipimsapiService = new PIMSAPIService();
        }

        [HttpGet]
        [ActionName("ValidateUser")]
        [Route("api/ManageApplications/ValidateUser")]
        public HttpResponseMessage ValidateUser(string userName, string password)
        {
            var getUserDetails = false;
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                getUserDetails = _loginService.ValidateUser(userName, password);
                return Request.CreateResponse(HttpStatusCode.OK, getUserDetails);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, getUserDetails);
        }

        [HttpGet]
        [ActionName("ValidateUserGetTypeAndId")]
        [Route("api/ManageApplications/ValidateUserGetTypeAndId")]
        public HttpResponseMessage ValidateUserGetTypeAndId(string userName, string password)
        {
            var getUserDetails = new UserTypeAndIdVM();
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                getUserDetails = _loginService.ValidateUserGetTypeAndId(userName, password);
                return Request.CreateResponse(HttpStatusCode.OK, getUserDetails);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, getUserDetails);
        }

        [HttpGet]
        [ActionName("GetUserDetailsByApplication")]
        [Route("api/ManageApplications/GetUserDetailsByApplication")]
        public HttpResponseMessage GetUserDetailsByApplication(string userName, int applicationId, string applicationName)
        {
            var getUserDetails = new CurrentUserDetail();
            if (!string.IsNullOrEmpty(userName) && applicationId > 0)
            {
                getUserDetails = _loginService.GetUserDetailsByApplication(userName, applicationId, applicationName);
                if (getUserDetails != null)
                {
                    // Here can redirect user to login page of main page.
                    return Request.CreateResponse(HttpStatusCode.OK, getUserDetails);
                }
                return Request.CreateResponse(HttpStatusCode.NoContent, getUserDetails);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, getUserDetails);

        }

        [HttpGet]
        [ActionName("GetAllUser")]
        [Route("api/ManageApplications/GetAllUser")]
        public HttpResponseMessage GetAllUser(string userName)
        {
            var getUserDetails = new Common.SelectList();
            var userModel = new List<Model.UsersModel>();

            userModel = _manageUsersService.GetAllUser(userName);

            if (userModel != null)
            {
                var pimsUsers = (from users in userModel
                                 select new Common.SelectList
                                 {
                                     Text = users.fullName,
                                     Value = users.id
                                 }).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, pimsUsers);
            }
            return Request.CreateResponse(HttpStatusCode.NoContent, getUserDetails);
        }

        [HttpGet]
        [ActionName("GetAllSelectiveUser")]
        [Route("api/ManageApplications/GetAllSelectiveUser")]
        public HttpResponseMessage GetAllSelectiveUser(string userName)
        {
            var getUserDetails = new Common.SelectList();
            var userModel = new List<Model.UsersModel>();

            userModel = _manageUsersService.GetUserByName(userName);

            if (userModel != null)
            {
                var pimsUsers = (from users in userModel
                                 select new Common.SelectList
                                 {
                                     Text = users.fullName,
                                     Value = users.id
                                 }).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, pimsUsers);
            }
            return Request.CreateResponse(HttpStatusCode.NoContent, getUserDetails);
        }

        [HttpGet]
        [ActionName("GetApproversListByApplicationId")]
        [Route("api/ManageApplications/GetApproversListByApplicationId")]
        public HttpResponseMessage GetApproversListByApplicationId(int logedInUserId, int applicationId)
        {
            var getUsers = _ipimsapiService.GetApproversListByApplicationId(logedInUserId, applicationId);
            return Request.CreateResponse(HttpStatusCode.OK, getUsers);
        }
    }
}
