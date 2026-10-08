using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NA.PMS.Model;
using NA.PMS.Service.APIRequestServices;
using NA.PMS.Web.Filters;

namespace NA.PMS.Web.APIControllers
{

    [AllowAnonymous]
    public class ServiceRequestController : ApiController
    {
        private readonly IApiRequestService _apiRequestService = new ApiRequestService();

        //public ServiceRequestController()
        //    : base()
        //{
        //}

        //public ServiceRequestController(IApiRequestService apiRequestService)
        //{
        //    _apiRequestService = apiRequestService;
        //}

        // GET api/<controller>
        public HttpResponseMessage Get()
        {

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "Success");
            if (Authorize(Request))
            {

                try
                {



                }
                catch
                {
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError, "There is an InternalServerError");

                }
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "Please make sure your provided username and password");
            }


            return response;
        }

        [ActionName("ServiceRequestStatus")]
        // GET api/<controller>/5
        public HttpResponseMessage GetServiceRequestStatus(int serviceId)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "Success");
            if (Authorize(Request))
            {
                try
                {
                    var result = _apiRequestService.GetServiceRequestStatus(serviceId);

                    response = Request.CreateResponse(HttpStatusCode.OK, result);
                }
                catch
                {
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError, "There is an InternalServerError");

                }
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "Please make sure your provided username and password");
            }


            return response;
        }

        [ActionName("SaveServiceRequest")]
        [HttpPost]
        // POST api/<controller>
        public HttpResponseMessage PostSaveServiceRequest(Customer_ServiceRequest serviceRequest)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "Success");
            if (Authorize(Request))
            {

                try
                {

                    var result = _apiRequestService.SaveServiceRequest(serviceRequest);

                    response = Request.CreateResponse(HttpStatusCode.OK, result);


                }
                catch
                {
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError, "There is an InternalServerError");

                }
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "Please make sure your provided username and password");
            }


            return response;
        }

        /// <summary>
        /// Authorize User
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected bool Authorize(HttpRequestMessage request)
        {
            bool result = false;
            var username = string.Empty;
            var Password = string.Empty;
            IEnumerable<string> apiHeaderValues = null;
            

            if (request.Headers.TryGetValues("UserName", out apiHeaderValues))
            {
                username = apiHeaderValues.First();
            }
            if (request.Headers.TryGetValues("Password", out apiHeaderValues))
            {
                Password = apiHeaderValues.First();
            }


            if (username == "citizen" && Password == "test#@!123")
            {
                result = true;
            }



            return result;
        }

    }
}
