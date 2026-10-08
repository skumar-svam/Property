using Kendo.Mvc.UI;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.UserManagement
{
    public interface IAdminRepository
    {
        DataSourceResult GetUsersAsDataSource(DataSourceRequest request, NDAUserViewModel model);

        List<NDACheckBoxViewModel> GetDepartmentList();

        NDAUserViewModel SaveAuthorityUserDetail(NDAUserViewModel user);

        NDAUserViewModel GetAuthorityUserDetailById(NDAUserViewModel model);

        NDAUserViewModel ValidateUserDetail(NDAUserViewModel model);

        DataSourceResult GetDepartmentListByIdAsDataSource(DataSourceRequest request, NDAUserViewModel model);

        NDAUserViewModel MapUserToRoleAndApplication(NDAUserViewModel model);

        DataSourceResult GetCustomerListAsDataSource(DataSourceRequest request, NDAUserViewModel model);

        NDAUserViewModel GetCustomerDetailById(NDAUserViewModel model);

        DataSourceResult GetRoleListAsDataSource(DataSourceRequest request, NDARoleViewModel model);

        DataSourceResult GetApplicationListAsDataSource(DataSourceRequest request, NDARoleViewModel model);

        DataSourceResult GetMenuListAsDataSource(DataSourceRequest request, NDARoleViewModel model);

        NDAUserViewModel SaveAuthorityCustomer(NDAUserViewModel customer, HttpPostedFileBase userIdFile, HttpPostedFileBase propertyFile);

        NDARoleViewModel SaveAuthorityRole(NDARoleViewModel model);

        NDARoleViewModel SaveAuthorityApplication(NDARoleViewModel model);

        NDARoleViewModel SaveMenuByApplication(NDARoleViewModel model);

        NDAUserViewModel SaveEmployeeInfoByActionType(NDAUserViewModel user);

        NDAUserViewModel SaveCustomerInfoByActionType(NDAUserViewModel user);

        DataSourceResult GetApplicationRoleDetailByIdAsDataSource(DataSourceRequest request, NDARoleViewModel model);

        DataSourceResult GetMasterDataListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        NDAUserViewModel ValidateCustomerRegistration(NDAUserViewModel model);

        NDAUserViewModel ValidateEmployeeInfoByActionType(NDAUserViewModel model);

        DataSourceResult GetMappedUsersListByApplicationRoleAsDataSource(DataSourceRequest request, NDAUserViewModel model);

        NDARoleViewModel GetMenuListByApplicationRoleAsHtmlContent(NDARoleViewModel model);

        NDARoleViewModel SaveApplicationRoleMenu(NDARoleViewModel model);

        NDAMasterDataViewModel GetMasterDataInfoByType(NDAMasterDataViewModel model);

        NDAMasterDataViewModel SaveMasterDataInfoByType(NDAMasterDataViewModel model);

        DataSourceResult GetDepartmentListAsDataSource(DataSourceRequest request, NDARoleViewModel model);

        DataSourceResult GetStatusListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model);

        DataSourceResult GetSectorAndBlockListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model);

        DataSourceResult GetPropertyTypeListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model);

        DataSourceResult GetSchemeTypeListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model);

        DataSourceResult GetAreaRangeListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model);

        DataSourceResult GetCommonConfigurationListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model);

        DataSourceResult GetOnlineSchemeListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model);

        DataSourceResult GetMasterDataDropDownListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model);
    }
}
