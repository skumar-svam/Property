using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.OnlineScheme
{
    public interface IDefaultSchemeRepository
    {
        List<OSDropdownViewModel> GetBankListBySchemeId(OSDropdownViewModel model);

        DataSourceResult GetSchemeListAsDataSource(DataSourceRequest request, OSDropdownViewModel model);

        DataSourceResult GetDepartmentListAsDataSource(DataSourceRequest request, OSDropdownViewModel model);

        DataSourceResult GetDropdownPropertyListForAllotmentAsDataSource(DataSourceRequest request, OSDropdownViewModel model);

        DataSourceResult GetDropdownOnlineSchemeFormIdListForAllotmentAsDataSource(DataSourceRequest request, OSDropdownViewModel model);

        DataSourceResult GetDropdownOnlineSchemeFormIdListAsDataSource(DataSourceRequest request, OSDropdownViewModel model);

        DataSourceResult GetBankListAsDataSource(DataSourceRequest request, OSDropdownViewModel model);

        DataSourceResult GetBanksBranchListAsDataSource(DataSourceRequest request, OSDropdownViewModel model);

        DataSourceResult GetPropertyTypeListAsDataSource(DataSourceRequest request, OSDropdownViewModel model);

        DataSourceResult GetFloorAreaListAsDataSource(DataSourceRequest request, OSDropdownViewModel model);

        OSLoginViewModel ChangePasswordForOnlineSchemeForm(OSLoginViewModel pmodel);

        List<OSDropdownViewModel> GetGenderTypeList(OSDropdownViewModel model);

        List<OSDropdownViewModel> GetMaritalStatusList(OSDropdownViewModel model);

        List<OSDropdownViewModel> GetCategoryList(OSDropdownViewModel model);

        List<OSDropdownViewModel> GetOccupationList(OSDropdownViewModel model);

        DataSourceResult GetSectorListAsDataSource(DataSourceRequest request, OSDropdownViewModel model);

        List<OSDropdownViewModel> GetCompanyTypeListByCategory(OSDropdownViewModel model);

        List<OSDropdownViewModel> GetDirectorTypeList(OSDropdownViewModel model);

        SchemeFormViewModel SendMessageToApplicant(SchemeFormViewModel model);

        List<OSDropdownViewModel> GetFormTypeList(OSDropdownViewModel model);

        List<OSDropdownViewModel> GetFormSubTypeList(OSDropdownViewModel model);

        List<OSDropdownViewModel> GetApplicantTypeList(OSDropdownViewModel model);

        List<OSDropdownViewModel> GetPaymentStatusList(OSDropdownViewModel model);



        OSSchemeAreaViewModel SaveSchemeAreaRange(OSSchemeAreaViewModel model);

        DataSourceResult GetSchemeAreaRangeListAsDataSource(DataSourceRequest request, OSSchemeAreaViewModel model);
    }
}
