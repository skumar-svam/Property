using Kendo.Mvc.UI;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NA.PMS.Repository
{
    public interface IGeneralRepository
    {
        List<DDList> GetAllDepartments();
        List<DDList> GetAllDepartmentsByScheme(int schemeId);
        // Get all departments of current user
        List<SelectListItem> GetAllUserDepartments();
        // Get Sectors by Department IDs
        List<SelectListItem> GetSectorsByDeptId(int departmentId);
        // Get Blocks by Department and Sector IDs
        List<SelectListItem> GetBlocksByDeptAndSectorId(int departmentId, int sectorId);
        DataSourceResult GetAllDepartmentList(DataSourceRequest Req);
        // Get Sectors by Department IDs except from Scheme 
        List<SelectListItem> GetAllSectorsByDeptId(int departmentId);
        DataSourceResult GetAllSectorsByDeptId(DataSourceRequest Req);
        // Get Blocks by Department and Sector Ids except from Scheme 
        List<SelectListItem> GetAllBlocksByDeptAndSectorId(int departmentId, int sectorId);
        DataSourceResult GetAllBlocksByDeptAndSectorId(DataSourceRequest Req, int departmentId, int sectorId);
        List<DDList> GetPropTypes(int depttID);
        List<DDLStringList> GetAllInterestRate();
        List<DDLStringList> GetAllPenalRate();
        List<DDList> GetAllFrquency();
        List<DDList> GetAllQuota();
        List<DDList> GetAllUnits();
        List<DDList> GetRebateUnits();
        List<DDList> GetRebateTypes();
        List<DDList> GetAllBanks();
        List<DDList> GetAllBranchs(int bankId);
        List<DDList> GetFloors(int depttID);
        List<DDList> GetAllSchemeType();
        List<DDList> GetAllSectors();
        List<DDList> GetAllBlocks();
        List<DDList> GetAllFloors();
        List<DDList> GetGenderById(string genderId);
        List<DDList> GetGender();
        List<DDList> GetMaritialStatus();
        List<DDList> GetOccupation();
        List<DDList> GetReligion();
        List<DDList> GetRegistry();
        List<DDLStringList> GetAllSelectionType();
        List<DDList> GetProcessData(int departmentId);
        List<DDList> GetTriggerProcessData(int departmentId);
        List<DDList> GetIndividualGenders();
        List<DDList> GetSectorsByPropType();

        List<DDList> GetChallanOptions();

        List<DDList> GetSchemeWiseBanks(int schemeId);
        List<DDList> GetAssineToByDepttId(int depttId);
        List<DDList> GetSchemeWiseBranchs(int bankId, int schemeId);
        string GenerateLetterFromDbByRId(int rId, int templateId);
        List<DDList> GetCompletionType();
        DateTime GetCompletionDueDateByRId(int rId, int schemeId, int deptId);
        List<DDList> GetRIDs(string type);
        List<DDList> YesNoDDL();
        List<DDList> RegUnRegDDL();
        //Common method to bind ddl
        List<DDList> BindDDL(string type);
        List<DDList> GetDepartmentsByUser();
        // Method for getting content for letter to print
        string GenerateLetterFromDb(int rid, int templateId, int departmentId);
        string DownloadApplicationFormat(int appId, int templateId, int departmentId);
        bool SaveFile(HttpPostedFileBase hpf, int rId);
        List<DDList> GetAllPropTypes();
        List<DDList> GetFloorByDeptId(int deptId);

        List<DDList> GetFunctionalStatus();

        List<DDList> GetCompletedSchemeToSearch();

        List<DDList> GetDepartmentListForLoginUser();

        List<DDList> AreaChangeOnPossession();

        List<DDList> PropertyPossessionStatus();

        List<DDList> GetPropertyCompletionStatus();

        List<DDList> GetServiceRequestsByDepartment(int department);

        BankAccountManagementModel GetPropertyDetailByRid(int rid, int referenceNo);

        List<DDList> GetRegistrationIdByDepartment(int? departmentId);
        DataSourceResult GetRegistrationIdByDepartment(DataSourceRequest Req);
        List<DynamicDataModel> GetPropertyPaymentType();

        List<DynamicDataModel> GetBankNamesForPayment();

        List<DDList> GetAllLettersType();

        List<DDList> GetServicesByDepartment(int? departmentId);

        PropertyInfoModel GetAllotteDetailsToGenerateLetter(int? rid);

        string GenerateLetterByService(int? rid, int? departmentId, int? serviceId, int? letterId, DateTime? letterDate);

        List<DDList> GetServicesRequestListByRid(int? rid);

        int? GetServiceRequestStatus(int? requestId);

        int GetServiceRequestStatusForLetter(int? rid, int? letterId);

        List<DDList> GetLettersByTemplateDepartment(int? departmentId);

        List<DDList> GetServiceRequestStatusList();

        DataSourceResult GetRegistrationIdList(DataSourceRequest request);

        DataSourceResult GetPropertyIdList(DataSourceRequest request);

        ApplicantModel GetApplicantDetailToSendMessage(int? registrationId);

        bool SendMessageToApplicant(ApplicantModel model);

        NDCVeiwModel GetApplicantDetailsForNDC(string registrationId);

        List<DropdownViewModel> GetYesNoStatus();

        DataSourceResult GetLettersTemplateByDepartment(DataSourceRequest request, int? departmentId);

        List<DropdownViewModel> GetSchemeList();

        List<DropdownViewModel> GetDepartmentListByScheme(int schemeId);
        DataSourceResult GetDepartmentListByScheme(DataSourceRequest Req, int? schemeId);

        List<DropdownViewModel> GetGenderList();

        List<DropdownViewModel> GetMaritalStatusList();

        List<DropdownViewModel> GetCategoryList();

        List<DropdownViewModel> GetOccupationList();

        List<DropdownViewModel> GetCompanyTypeList();

        List<DropdownViewModel> GetBankList();

        List<DropdownViewModel> GetBankListBySchemeId(int schemeId);

        List<DropdownViewModel> GetPropertyTypeList();

        List<DropdownViewModel> GetPropertyTypeListById(int departmentId);

        List<DropdownViewModel> GetPropertyTypeListByDepartment(int departmentId);

        List<DropdownViewModel> GetFloorAreaListByDepartment(int departmentId);

        List<DropdownViewModel> GetPropertyTypeListForOnline(int schemeId, int departmentId);

        List<DropdownViewModel> GetFloorAreaListForOnline(int schemeId, int departmentId, int propertyTypeId);

        List<DropdownViewModel> GetDepartmentList();

        List<DropdownViewModel> GetServiceListByDepartment(int departmentId);

        List<DropdownViewModel> GeSubtDepartmentList(int departmentId);

        List<DropdownViewModel> GetTransferTypeList();

        List<DropdownViewModel> GetTransferSubTypeList(int transferTypeId);

        List<DropdownViewModel> GetCICRequestTypeList();

        List<DropdownViewModel> GetCompanyMemberTypeList();

        List<DropdownViewModel> GetFirmStatusList();

        List<DropdownViewModel> GetMortgageTypeList();

        List<DropdownViewModel> GetGPAStatusList();

        List<DropdownViewModel> GetNOCStatusList();

        List<DropdownViewModel> GetFloorAreaListForOnline(int schemeId, int departmentId);

        List<DropdownViewModel> GetDirectorTypeList();

        List<DropdownViewModel> GetCompanyTypeByCategory(string typeName);
        ChallanBankandAccountNo GetAccountBranchBySchemeIdBankId(int schemeId, int BankId);

        List<DropdownViewModel> getSectorsList();

        List<DropdownViewModel> getStatusMasterList();

        List<DropdownViewModel> GetMortgageLoanStatus();

        int SaveSectorBlockName(string type, string typeName);

        List<DropdownViewModel> GetResourceMessageList();

        List<DropdownViewModel> GetFormTypeList();

        List<DropdownViewModel> GetApplicantTypeList();

        List<DropdownViewModel> GetFormSubTypeList(string formtype);

        LetterViewModel GetLetterByBarcode(string barcode);

        List<DropdownViewModel> GetOnlineApplicationFormIdList(DataSourceRequest request, int? schemeId, int? departmentId);

        List<DropdownViewModel> GetApplicationList();

        List<DropdownViewModel> GetMenuList(int? parentId);

        List<DropdownViewModel> GetParentMenuList();

        List<DropdownViewModel> GetAreaRangeList(int department);

        List<DropdownViewModel> GetCommonConfigDataList(string category);

        List<DropdownViewModel> GetSchemeTypeList();

        List<DropdownViewModel> GetCommonConfigCategoryList();

        List<DropdownViewModel> GetPropertyTypeByDepartment(int departmentId);

        List<DropdownViewModel> GetStatusList();

        List<DropdownViewModel> GetStatusListByType(string type);

        int SaveApplication(CommonViewModel model);

        int SaveSchemeType(CommonViewModel model);

        int SavePropertyType(CommonViewModel model);

        int SavePropertyAreaRange(CommonViewModel model);

        int SaveApplicationMenu(CommonViewModel model);

        int SaveCommonConfigData(CommonViewModel model);

        int SaveStatusByType(CommonViewModel model);

        List<DropdownViewModel> GetStatusTypeList();

        int SaveDepartment(CommonViewModel model);

        CommonViewModel GetPropertyTypeDetailById(CommonViewModel model);

        DataSourceResult GetFrequencyList(DataSourceRequest request);

        DataSourceResult GetApproverIdList(DataSourceRequest request);

        DataSourceResult GetRegistrationIdListAsDataSource(DataSourceRequest request);

        AllotmentModel GetApplicantDetails(int? rid);

        DataSourceResult GetPropertyListForAllotment(DataSourceRequest request, int? schemeId, int? departmentId);

        DataSourceResult GetOnlineApplicationFormIdListForAllotment(DataSourceRequest request, int? schemeId, int? departmentId);

        DataSourceResult GetApplicationFormListByScheme(DataSourceRequest request, int? schemeId, int? departmentId);

        DataSourceResult GetStatusMasterAsDataSource(DataSourceRequest request);

        CommonViewModel GetCommonConfigDataById(CommonViewModel model);

        DataSourceResult GetServiceListAsDataSource(DataSourceRequest request);

        DataSourceResult GetServicesByDepartmentAsDataSource(DataSourceRequest request, DropdownViewModel model);

        DataSourceResult GetBankListAsDataSource(DataSourceRequest request);

        DataSourceResult GetBranchListOfBankAsDataSource(DataSourceRequest request, int? bankId);

        int SaveTempChallanChargeDetail(ChallanViewModel model);

        int SaveTempChallanChargeDetailII(ChallanViewModel model);

        int RemoveTempChallanChargeDetail(ChallanViewModel model);

        int RemoveTempChallanChargeDetailII(ChallanViewModel model);

        DataSourceResult GetTempChallanChargesAsDataSource(DataSourceRequest request, ChallanViewModel model);

        DataSourceResult GetTempChallanChargesAsDataSourceII(DataSourceRequest request, ChallanViewModel model);

        PropertyDetailViewModel GetAllottedPropertyDetailByRegistrationId(int? rid);

        DataSourceResult GetReceiptHeadListAsDataSource(DataSourceRequest request);

        DataSourceResult GetReceiptSubHeadListAsDataSource(DataSourceRequest request, int? id);

        DataSourceResult GetLetterTemplateListByDepartmentAsDataSource(DataSourceRequest request, int? departmentId);

        LetterViewModel GenerateAuthorizedLetterById(LetterViewModel model);

        LetterViewModel GetGeneratedLetterByBarcode(LetterViewModel model);

        int SendRegisteredMessageToApplicant(ApplicantViewModel model);

        DataSourceResult GetDepartmentListAsDataSource(DataSourceRequest request);

        DataSourceResult GetDepartmentListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        DataSourceResult GetNotingFileTypeListAsDataSource(DataSourceRequest request);

        DataSourceResult GetSchemeListAsDataSource(DataSourceRequest request);

        DataSourceResult GetApplicationFormListAsDataSource(DataSourceRequest request, int? schemeId, int? departmentId);

        DataSourceResult GetAllottedPropertyIdListAsDataSource(DataSourceRequest request, int? schemeId, int? departmentId, int? applicationId);

        DataSourceResult GetUserStatusListAsDataSource(DataSourceRequest request);

        DataSourceResult GetSectorListAsDataSource(DataSourceRequest request, PropertyViewModel model);

        DataSourceResult GetBlockListAsDataSource(DataSourceRequest request, PropertyViewModel model);

        DataSourceResult GetSectorListAsDataSourceII(DataSourceRequest request, PropertyViewModel model);

        DataSourceResult GetBlockListAsDataSourceII(DataSourceRequest request, PropertyViewModel model);

        DataSourceResult GetPlotListAsDataSource(DataSourceRequest request, PropertyViewModel model);

        DataSourceResult GetMasterSearchParameterAsDataSource(DataSourceRequest request, PropertyViewModel model);

        DataSourceResult GetSecurityQuestioinListAsDataSource(DataSourceRequest request);

        DataSourceResult GetBankAccountDetailAsDataSource(DataSourceRequest request, BankAccountViewModel model);

        int SaveBankAccountDetail(BankAccountViewModel model);

        int SaveServiceDetailById(ServiceViewModel model);

        BankAccountViewModel GetBankAccountDetailById(BankAccountViewModel model);

        ServiceViewModel GetServiceDetailById(ServiceViewModel model);

        int SavePropertyServiceType(ServiceViewModel model);

        DataSourceResult GetPropertyTypeByDepartmentAsDataSource(DataSourceRequest request, int departmentId);

        DataSourceResult GetDefaultSectorListAsDataSource(DataSourceRequest request);

        DataSourceResult GetDefaultBlockListAsDataSource(DataSourceRequest request);

        DataSourceResult GetApplicationIdListAsDataSource(DataSourceRequest request);

        DataSourceResult GetAllotteeIdTypeList(DataSourceRequest request, DropdownViewModel model);

        DataSourceResult GetOwnershipFileTypeList(DataSourceRequest request, DropdownViewModel model);

        int SendOTP(CommonViewModel model);

        int ValidateOTP(CommonViewModel model);

        PropertyViewModel GetPropertyDetailById(PropertyViewModel model);

        DataSourceResult GetOptionalReasonAsDataSource(DataSourceRequest request);

        int CheckChallanSessionDataById(ChallanViewModel model);

        List<DropdownViewModel> GetVillageIdList();

        int VerifyChallanDetailById(ChallanViewModel model);

        LetterViewModel GenerateNDCByRegistrationId(LetterViewModel model);

        DataSourceResult GetOSDApproverIdList(DataSourceRequest request);

        int IsRegistrationIdExist(PropertyViewModel model);

        DropdownViewModel GetServiceTypeDetailById(DropdownViewModel model);

        DataSourceResult GetServiceTypeListAsDataSource([DataSourceRequest]DataSourceRequest request, DropdownViewModel model);

        List<DashboardPropertyVM> GetDashboardPropertyData(int departmentId);

        KYAViewModel GetKYADetailsForRid(KYAViewModel model);

        CommonViewModel ActivateDetailsOnId(CommonViewModel model);

        DataSourceResult GetRegistrationIdListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        List<DropdownViewModel> GetAllotteeTypeList();

        DataSourceResult GetFloorAreaListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        DataSourceResult GetLocationTypeListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        DataSourceResult GetReceiptHeadIdListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        LoginUserDetail GetLoginUserDetails(int userId);

        int IsRequestIdIsExist(ServiceViewModel model);

        DataSourceResult GetSchemeRefundTypeListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        DataSourceResult GetApproverIdListByDepartment(DataSourceRequest request, DropdownViewModel model);

        int SendReminderToAllottee(int? Rid);

        int IsRidExists(int? Id, int? RegistrationId, string PropertyNo);

        int ValidateUpdatedProperty(PropertyViewModel model);

        DataSourceResult GetAllotmentYearAsDataSource(DataSourceRequest request, DropdownViewModel model);

        DataSourceResult GetUsersDepartmentListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        DataSourceResult GetStatusListByRoleAsDataSource(DataSourceRequest request, DropdownViewModel model);

        DataSourceResult GetApproverIdByDepartmentAsDataSource(DataSourceRequest request, DropdownViewModel model);

        List<DropdownViewModel> GetYearsList();

        List<DropdownViewModel> GetServiceListByDepartmentForNAServices(int departmentId);

        DataSourceResult GetDocumentTypeListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        EmployeeViewModel GetEmployeeDetailsById(EmployeeViewModel model);

        DataSourceResult GetEmployeeListAsdatasource(DataSourceRequest request);

        int RemoveTempChallanChargeDetailForEmployee(ChallanViewModel model);

        int SaveTempChallanChargeDetailForEmployee(ChallanViewModel model);

        decimal GetTotalAmount(decimal? amount);

        DataSourceResult GetTempChallanChargesForEmployee(DataSourceRequest request);

        DataSourceResult GetBankListForPaymentAsDataSource(DataSourceRequest request, DropdownViewModel model);
    }
}
