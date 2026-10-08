
using NA.PMS.Model;
using NA.PMS.Service;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NA.PMS.API.Controllers
{
    [Authorize]
    public class PropertyController : ApiController
    {
        IPIMSAPIService _ipimsapiService = null;
        public PropertyController()
        {
            this._ipimsapiService = new PIMSAPIService();
        }

        [HttpGet]
        [ActionName("GetPropertyDetailsByAddress")]
        [Route("api/PropertyController/GetPropertyDetailsByAddress")]
        public HttpResponseMessage GetPropertyDetailsByAddress(string sectorName, string blockName, string PlotNo)
        {
            var model = new PropertyViewModel();
            model.SectorName = sectorName;
            model.BlockName = blockName;
            model.PlotNo = PlotNo;
            var propertyDetails = _ipimsapiService.GetPropertyDetailsByAddress(model);
            return Request.CreateResponse(HttpStatusCode.OK, propertyDetails);
        }

        [HttpPost]
        [ActionName("SaveCustomerInfo")]
        [Route("api/Property/SaveCustomerInfo")]
        public HttpResponseMessage SaveCustomerInfo(string Applicant, string MobileNo, string Email, string Comment, int? DepartmentId, int? TotalInstallment, int? PropertyUpdateId)
        {
            var model = new PropertyViewModel();
            model.Applicant = Applicant;
            model.MobileNo = MobileNo;
            model.Email = Email;
            model.Comment = Applicant;
            model.DepartmentId = DepartmentId;
            model.TotalInstallment = TotalInstallment;
            model.PropertyUpdateId = PropertyUpdateId;
            var propertyDetails = _ipimsapiService.SaveCustomerInfo(model);
            return Request.CreateResponse(HttpStatusCode.OK, propertyDetails);
        }
    }
}
