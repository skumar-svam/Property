using NA.PMS.Model;
using System.Collections.Generic;

namespace NA.PMS.Repository
{
    public interface IPIMSAPIRepository 
    {
        PropertyViewModel GetPropertyDetailsByAddress(PropertyViewModel model);

        List<DDList> GetApproversListByApplicationId(int logedInUserId, int applicationId);

        PropertyViewModel SaveCustomerInfo(PropertyViewModel model);
    }
}
