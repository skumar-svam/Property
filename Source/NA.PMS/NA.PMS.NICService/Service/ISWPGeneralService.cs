using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.NICServices
{
    public interface ISWPGeneralService
    {
        DataSourceResult GetServiceListAsDataSource(DataSourceRequest request);

        List<SWPDropdownViewModel> GetStatusList();

        DataSourceResult GetStatusMasterAsDataSource(DataSourceRequest request);

        SWPLoginViewModel GetLoginUserDetails(int _approverId);

        List<SWPDropdownViewModel> GetDepartmentList();

        List<SWPDropdownViewModel> GetSubDepartmentList(int departmentId);

        List<SWPDropdownViewModel> GetFirmStatusList();

        List<SWPDropdownViewModel> GetMortgageTypeList();

        List<SWPDropdownViewModel> GetNOCStatusList();

        List<SWPDropdownViewModel> GetGPAStatusList();

        List<SWPDropdownViewModel> GetGenderList();

        List<SWPDropdownViewModel> GetOccupationList();

        List<SWPDropdownViewModel> GetCICRequestTypeList();

        List<SWPDropdownViewModel> GetTransferTypeList();

        List<SWPDropdownViewModel> GetTransferSubTypeList(int transferTypeId);

        List<SWPDropdownViewModel> GetCompanyMemberTypeList();

        List<SWPDropdownViewModel> GetBankListBySchemeId(int p);

        List<SWPDropdownViewModel> GetFormTypeList();

        List<SWPDropdownViewModel> GetFormSubTypeList(string formtype);

        List<SWPDropdownViewModel> GetApplicantTypeList();

        List<SWPDropdownViewModel> GetCompanyTypeByCategory(string typeName);

        List<SWPDropdownViewModel> getSectorsList();

        List<SWPDropdownViewModel> GetDirectorTypeList();

        List<SWPDropdownViewModel> GetMaritalStatusList();

        List<SWPDropdownViewModel> GetCategoryList();

        List<SWPDropdownViewModel> GetBankList();

        DataSourceResult GetDropDownListAsDataSource(DataSourceRequest request, SWPDropdownViewModel model);

        DataSourceResult GetNiveshMitraStatusListAsDataSource(DataSourceRequest request, SWPDropdownViewModel model);

        DataSourceResult GetDropDownApproverListAsDataSource(DataSourceRequest request, SWPDropdownViewModel model);
    }
}
