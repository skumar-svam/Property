
using NA.PMS.Model;
using System.Collections.Generic;

namespace NA.PMS.Service
{
    public interface IPIMSAPIService
    {
        PropertyViewModel GetPropertyDetailsByAddress(PropertyViewModel model);

        List<DDList> GetApproversListByApplicationId(int logedInUserId, int applicationId);

        PropertyViewModel SaveCustomerInfo(PropertyViewModel model);
    }
}
