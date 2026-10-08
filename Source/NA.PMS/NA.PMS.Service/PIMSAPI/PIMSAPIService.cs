
using NA.PMS.Model;
using NA.PMS.Repository;
using System.Collections.Generic;

namespace NA.PMS.Service
{
    public class PIMSAPIService : IPIMSAPIService
    {
        IPIMSAPIRepository _pimsapiReporsitory;
        public PIMSAPIService()
        {
            this._pimsapiReporsitory = new PIMSAPIRepository();
        }

        public PropertyViewModel GetPropertyDetailsByAddress(PropertyViewModel model)
        {
            return _pimsapiReporsitory.GetPropertyDetailsByAddress(model);
        }

        public List<DDList> GetApproversListByApplicationId(int logedInUserId, int applicationId)
        {
            return _pimsapiReporsitory.GetApproversListByApplicationId(logedInUserId, applicationId);
        }


        public PropertyViewModel SaveCustomerInfo(PropertyViewModel model)
        {
            return _pimsapiReporsitory.SaveCustomerInfo(model);
        }
    }
}
