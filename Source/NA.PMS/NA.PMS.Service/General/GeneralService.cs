using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using NA.PMS.Repository;
using NA.PMS.Model;
using System.Web;
using Kendo.Mvc.UI;

namespace NA.PMS.Service
{
    public class GeneralService : IGeneralService
    {
        private IGeneralRepository _generalRepository;
        public GeneralService(IGeneralRepository generalRepository)
        {
            _generalRepository = generalRepository;
        }

        public List<DDList> GetAllDepartments()
        {
            return _generalRepository.GetAllDepartments();
        }
        // Get all departments of current user
        public List<SelectListItem> GetAllUserDepartments()
        {
            return _generalRepository.GetAllUserDepartments();
        }

        // Get Sectors by Department IDs
        public List<SelectListItem> GetSectorsByDeptId(int departmentId)
        {
            return _generalRepository.GetSectorsByDeptId(departmentId);
        }

        public DataSourceResult GetAllDepartmentList(DataSourceRequest Req)
        {
            return _generalRepository.GetAllDepartmentList(Req);
        }

        // Get Blocks by Department and Sector IDs
        public List<SelectListItem> GetBlocksByDeptAndSectorId(int departmentId, int sectorId)
        {
            return _generalRepository.GetBlocksByDeptAndSectorId(departmentId, sectorId);
        }

        public List<DDList> GetAllDepartmentsByScheme(int schemeId)
        {
            return _generalRepository.GetAllDepartmentsByScheme(schemeId);
        }
        public List<DDList> GetPropTypes(int depttID)
        {
            return _generalRepository.GetPropTypes(depttID);
        }

        public List<DDLStringList> GetAllInterestRate()
        {
            return _generalRepository.GetAllInterestRate();
        }

        public List<DDLStringList> GetAllPenalRate()
        {
            return _generalRepository.GetAllPenalRate();
        }

        public List<DDList> GetAllFrquency()
        {
            return _generalRepository.GetAllFrquency();
        }

        public List<DDList> GetAllQuota()
        {
            return _generalRepository.GetAllQuota();
        }
        public List<DDList> GetAllUnits()
        {
            return _generalRepository.GetAllUnits();
        }

        public List<DDList> GetRebateUnits()
        {
            return _generalRepository.GetRebateUnits();
        }
        public List<DDList> GetRebateTypes()
        {
            return _generalRepository.GetRebateTypes();
        }
        public List<DDList> GetAllBanks()
        {
            return _generalRepository.GetAllBanks();
        }
        public List<DDList> GetAllBranchs(int bankId)
        {
            return _generalRepository.GetAllBranchs(bankId);
        }


        public List<DDList> GetSectorsByPropType()
        {
            return _generalRepository.GetSectorsByPropType();
        }

        public List<DDList> GetFloors(int depttID)
        {
            return _generalRepository.GetFloors(depttID);
        }

        public List<DDList> GetAllSchemeType()
        {
            return _generalRepository.GetAllSchemeType();
        }

        public List<DDList> GetAllSectors()
        {
            return _generalRepository.GetAllSectors();
        }

        public List<DDList> GetAllBlocks()
        {
            return _generalRepository.GetAllBlocks();
        }

        public List<DDList> GetAllFloors()
        {
            return _generalRepository.GetAllFloors();
        }

        public List<DDList> GetGenderById(string genderId)
        {
            return _generalRepository.GetGenderById(genderId);
        }
        public List<DDList> GetGender()
        {
            return _generalRepository.GetGender();
        }

        public List<DDList> GetIndividualGenders()
        {
            return _generalRepository.GetIndividualGenders();
        }

        public List<DDList> GetMaritialStatus()
        {
            return _generalRepository.GetMaritialStatus();
        }

        public List<DDList> GetOccupation()
        {
            return _generalRepository.GetOccupation();
        }

        public List<DDList> GetReligion()
        {
            return _generalRepository.GetReligion();
        }

        public List<DDList> GetRegistry()
        {
            return _generalRepository.GetRegistry();
        }

        public List<DDLStringList> GetAllSelectionType()
        {
            return _generalRepository.GetAllSelectionType();
        }

        public List<DDList> GetChallanOptions()
        {
            return _generalRepository.GetChallanOptions();
        }

        public List<DDList> GetProcessData(int departmentId)
        {
            return _generalRepository.GetProcessData(departmentId);
        }

        public List<DDList> GetTriggerProcessData(int departmentId)
        {
            return _generalRepository.GetTriggerProcessData(departmentId);
        }


        public List<DDList> GetSchemeWiseBanks(int schemeId)
        {
            return _generalRepository.GetSchemeWiseBanks(schemeId);
        }
        public List<DDList> GetSchemeWiseBranchs(int bankId, int schemeId)
        {
            return _generalRepository.GetSchemeWiseBranchs(bankId, schemeId);
        }

        public List<DDList> GetAssineToByDepttId(int depttId)
        {
            return _generalRepository.GetAssineToByDepttId(depttId);
        }

        public List<DDList> GetCompletionType()
        {
            return _generalRepository.GetCompletionType();
        }

        public DateTime GetCompletionDueDateByRId(int rId, int schemeId, int deptId)
        {
            return _generalRepository.GetCompletionDueDateByRId(rId, schemeId, deptId);
        }
        public List<DDList> GetRIDs(string type)
        {
            return _generalRepository.GetRIDs(type);
        }

        //Common method to bind ddl
        public List<DDList> BindDDL(string type)
        {
            return _generalRepository.BindDDL(type);
        }
        public List<DDList> YesNoDDL()
        {
            return _generalRepository.YesNoDDL();
        }
        public List<DDList> RegUnRegDDL()
        {
            return _generalRepository.RegUnRegDDL();
        }

        public List<DDList> GetDepartmentsByUser()
        {
            return _generalRepository.GetDepartmentsByUser();
        }

        // Get Sectors by Department IDs except from Scheme 
        public List<SelectListItem> GetAllSectorsByDeptId(int departmentId)
        {
            return _generalRepository.GetAllSectorsByDeptId(departmentId);
        }

        // Get Sectors by Department IDs except from Scheme 
        public DataSourceResult GetAllSectorsByDeptId(DataSourceRequest Req)
        {
            return _generalRepository.GetAllSectorsByDeptId(Req);
        }

        // Get Blocks by Department and Sector Ids except from Scheme 
        public List<SelectListItem> GetAllBlocksByDeptAndSectorId(int departmentId, int sectorId)
        {
            return _generalRepository.GetAllBlocksByDeptAndSectorId(departmentId, sectorId);
        }

        // Get Blocks by Department and Sector Ids except from Scheme 
        public DataSourceResult GetAllBlocksByDeptAndSectorId(DataSourceRequest Req, int departmentId, int sectorId)
        {
            return _generalRepository.GetAllBlocksByDeptAndSectorId(Req, departmentId, sectorId);
        }

        // Method for getting content for letter to print
        public string GenerateLetterFromDb(int rid, int templateId, int departmentId)
        {
            return _generalRepository.GenerateLetterFromDb(rid, templateId, departmentId);
        }

        public string DownloadApplicationFormat(int appId, int templateId, int departmentId)
        {
            return _generalRepository.DownloadApplicationFormat(appId, templateId, departmentId);
        }

        public bool SaveFile(HttpPostedFileBase hpf, int rId)
        {
            return _generalRepository.SaveFile(hpf, rId);
        }
        public List<DDList> GetAllPropTypes()
        {
            return _generalRepository.GetAllPropTypes();
        }
        public List<DDList> GetFloorByDeptId(int deptId)
        {
            return _generalRepository.GetFloorByDeptId(deptId);
        }

        public string GenerateLetterFromDbByRId(int rId, int templateId)
        {
            return _generalRepository.GenerateLetterFromDbByRId(rId, templateId);
        }


        public List<DDList> GetFunctionalStatus()
        {
            return _generalRepository.GetFunctionalStatus();
        }


        public List<DDList> GetCompletedSchemeToSearch()
        {
            return _generalRepository.GetCompletedSchemeToSearch();
        }


        public List<DDList> GetDepartmentListForLoginUser()
        {
            return _generalRepository.GetDepartmentListForLoginUser();
        }


        public List<DDList> AreaChangeOnPossession()
        {
            return _generalRepository.AreaChangeOnPossession();
        }

        public List<DDList> PropertyPossessionStatus()
        {
            return _generalRepository.PropertyPossessionStatus();
        }


        public List<DDList> GetPropertyCompletionStatus()
        {
            return _generalRepository.GetPropertyCompletionStatus();
        }


        public List<DDList> GetServiceRequestsByDepartment(int department)
        {
            return _generalRepository.GetServiceRequestsByDepartment(department);
        }


        public BankAccountManagementModel GetPropertyDetailByRid(int rid, int referenceNo)
        {
            return _generalRepository.GetPropertyDetailByRid(rid, referenceNo);
        }


        public List<DDList> GetRegistrationIdByDepartment(int? departmentId)
        {
            return _generalRepository.GetRegistrationIdByDepartment(departmentId);
        }


        public DataSourceResult GetRegistrationIdByDepartment(DataSourceRequest Req)
        {
            return _generalRepository.GetRegistrationIdByDepartment(Req);
        }

        public List<DynamicDataModel> GetPropertyPaymentType()
        {
            return _generalRepository.GetPropertyPaymentType();
        }


        public List<DynamicDataModel> GetBankNamesForPayment()
        {
            return _generalRepository.GetBankNamesForPayment();
        }


        public List<DDList> GetAllLettersType()
        {
            return _generalRepository.GetAllLettersType();
        }

        public List<DDList> GetServicesByDepartment(int? departmentId)
        {
            return _generalRepository.GetServicesByDepartment(departmentId);
        }


        public PropertyInfoModel GetAllotteDetailsToGenerateLetter(int? rid)
        {
            return _generalRepository.GetAllotteDetailsToGenerateLetter(rid);
        }

        public string GenerateLetterByService(int? rid, int? departmentId, int? serviceId, int? letterId, DateTime? letterDate)
        {
            return _generalRepository.GenerateLetterByService(rid, departmentId, serviceId, letterId, letterDate);
        }

        public List<DDList> GetServicesRequestListByRid(int? rid)
        {
            return _generalRepository.GetServicesRequestListByRid(rid);
        }


        public int? GetServiceRequestStatus(int? requestId)
        {
            return _generalRepository.GetServiceRequestStatus(requestId);
        }


        public int GetServiceRequestStatusForLetter(int? rid, int? letterId)
        {
            return _generalRepository.GetServiceRequestStatusForLetter(rid, letterId);
        }


        public List<DDList> GetLettersByTemplateDepartment(int? departmentId)
        {
            return _generalRepository.GetLettersByTemplateDepartment(departmentId);
        }

        public DataSourceResult GetLettersTemplateByDepartment(DataSourceRequest request, int? departmentId)
        {
            return _generalRepository.GetLettersTemplateByDepartment(request, departmentId);
        }


        public List<DDList> GetServiceRequestStatusList()
        {
            return _generalRepository.GetServiceRequestStatusList();
        }


        public DataSourceResult GetRegistrationIdList(DataSourceRequest request)
        {
            return _generalRepository.GetRegistrationIdList(request);
        }

        public DataSourceResult GetPropertyIdList(DataSourceRequest request)
        {
            return _generalRepository.GetPropertyIdList(request);
        }

        public ApplicantModel GetApplicantDetailToSendMessage(int? registrationId)
        {
            return _generalRepository.GetApplicantDetailToSendMessage(registrationId);
        }


        public bool SendMessageToApplicant(ApplicantModel model)
        {
            return _generalRepository.SendMessageToApplicant(model);
        }


        public NDCVeiwModel GetApplicantDetailsForNDC(string registrationId)
        {
            return _generalRepository.GetApplicantDetailsForNDC(registrationId);
        }


        public List<DropdownViewModel> GetYesNoStatus()
        {
            return _generalRepository.GetYesNoStatus();
        }

        public List<DropdownViewModel> GetSchemeList()
        {
            return _generalRepository.GetSchemeList();
        }

        public List<DropdownViewModel> GetDepartmentListByScheme(int schemeId)
        {
            return _generalRepository.GetDepartmentListByScheme(schemeId);
        }

        public DataSourceResult GetDepartmentListByScheme(DataSourceRequest Req, int? schemeId)
        {
            return _generalRepository.GetDepartmentListByScheme(Req, schemeId);
        }

        public List<DropdownViewModel> GetGenderList()
        {
            return _generalRepository.GetGenderList();
        }

        public List<DropdownViewModel> GetMaritalStatusList()
        {
            return _generalRepository.GetMaritalStatusList();
        }

        public List<DropdownViewModel> GetCategoryList()
        {
            return _generalRepository.GetCategoryList();
        }

        public List<DropdownViewModel> GetOccupationList()
        {
            return _generalRepository.GetOccupationList();
        }

        public List<DropdownViewModel> GetCompanyTypeList()
        {
            return _generalRepository.GetCompanyTypeList();
        }


        public List<DropdownViewModel> GetBankList()
        {
            return _generalRepository.GetBankList();
        }

        public List<DropdownViewModel> GetBankListBySchemeId(int schemeId)
        {
            return _generalRepository.GetBankListBySchemeId(schemeId);
        }

        public List<DropdownViewModel> GetPropertyTypeList()
        {
            return _generalRepository.GetPropertyTypeList();
        }

        public List<DropdownViewModel> GetPropertyTypeListById(int departmentId)
        {
            return _generalRepository.GetPropertyTypeListById(departmentId);
        }

        public List<DropdownViewModel> GetPropertyTypeListByDepartment(int departmentId)
        {
            return _generalRepository.GetPropertyTypeListByDepartment(departmentId);
        }

        public List<DropdownViewModel> GetFloorAreaListByDepartment(int departmentId)
        {
            return _generalRepository.GetFloorAreaListByDepartment(departmentId);
        }


        public List<DropdownViewModel> GetPropertyTypeListForOnline(int schemeId, int departmentId)
        {
            return _generalRepository.GetPropertyTypeListForOnline(schemeId, departmentId);
        }

        public List<DropdownViewModel> GetFloorAreaListForOnline(int schemeId, int departmentId, int propertyTypeId)
        {
            return _generalRepository.GetFloorAreaListForOnline(schemeId, departmentId, propertyTypeId);
        }


        public List<DropdownViewModel> GetDepartmentList()
        {
            return _generalRepository.GetDepartmentList();
        }

        public List<DropdownViewModel> GetServiceListByDepartment(int departmentId)
        {
            return _generalRepository.GetServiceListByDepartment(departmentId);
        }

        public DataSourceResult GetServiceTypeListAsDataSource([DataSourceRequest]DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetServiceTypeListAsDataSource(request, model);
        }

        public List<DropdownViewModel> GetSubDepartmentList(int departmentId)
        {
            return _generalRepository.GeSubtDepartmentList(departmentId);
        }

        public List<DropdownViewModel> GetTransferTypeList()
        {
            return _generalRepository.GetTransferTypeList();
        }

        public List<DropdownViewModel> GetTransferSubTypeList(int transferTypeId)
        {
            return _generalRepository.GetTransferSubTypeList(transferTypeId);
        }

        public List<DropdownViewModel> GetCICRequestTypeList()
        {
            return _generalRepository.GetCICRequestTypeList();
        }

        public List<DropdownViewModel> GetCompanyMemberTypeList()
        {
            return _generalRepository.GetCompanyMemberTypeList();
        }

        public List<DropdownViewModel> GetFirmStatusList()
        {
            return _generalRepository.GetFirmStatusList();
        }

        public List<DropdownViewModel> GetMortgageTypeList()
        {
            return _generalRepository.GetMortgageTypeList();
        }

        public List<DropdownViewModel> GetGPAStatusList()
        {
            return _generalRepository.GetGPAStatusList();
        }

        public List<DropdownViewModel> GetNOCStatusList()
        {
            return _generalRepository.GetNOCStatusList();
        }


        public List<DropdownViewModel> GetFloorAreaListForOnline(int schemeId, int departmentId)
        {
            return _generalRepository.GetFloorAreaListForOnline(schemeId, departmentId);
        }


        public List<DropdownViewModel> GetDirectorTypeList()
        {
            return _generalRepository.GetDirectorTypeList();
        }


        public List<DropdownViewModel> GetCompanyTypeByCategory(string typeName)
        {
            return _generalRepository.GetCompanyTypeByCategory(typeName);
        }

        public ChallanBankandAccountNo GetAccountBranchBySchemeIdBankId(int schemeId, int BankId)
        {
            return _generalRepository.GetAccountBranchBySchemeIdBankId(schemeId, BankId);
        }


        public List<DropdownViewModel> getSectorsList()
        {
            return _generalRepository.getSectorsList();
        }


        public List<DropdownViewModel> getStatusMasterList()
        {
            return _generalRepository.getStatusMasterList();
        }


        public List<DropdownViewModel> GetMortgageLoanStatus()
        {
            return _generalRepository.GetMortgageLoanStatus();
        }


        public int SaveSectorBlockName(string type, string typeName)
        {
            return _generalRepository.SaveSectorBlockName(type, typeName);
        }


        public List<DropdownViewModel> GetResourceMessageList()
        {
            return _generalRepository.GetResourceMessageList();
        }


        public List<DropdownViewModel> GetFormTypeList()
        {
            return _generalRepository.GetFormTypeList();
        }

        public List<DropdownViewModel> GetApplicantTypeList()
        {
            return _generalRepository.GetApplicantTypeList();
        }

        public List<DropdownViewModel> GetFormSubTypeList(string formtype)
        {
            return _generalRepository.GetFormSubTypeList(formtype);
        }


        public LetterViewModel GetLetterByBarcode(string barcode)
        {
            return _generalRepository.GetLetterByBarcode(barcode);
        }


        public List<DropdownViewModel> GetOnlineApplicationFormIdList(DataSourceRequest request, int? schemeId, int? departmentId)
        {
            return _generalRepository.GetOnlineApplicationFormIdList(request, schemeId, departmentId);
        }


        public List<DropdownViewModel> GetApplicationList()
        {
            return _generalRepository.GetApplicationList();
        }

        public List<DropdownViewModel> GetMenuList(int? parentId)
        {
            return _generalRepository.GetMenuList(parentId);
        }

        public List<DropdownViewModel> GetParentMenuList()
        {
            return _generalRepository.GetParentMenuList();
        }

        public List<DropdownViewModel> GetAreaRangeList(int department)
        {
            return _generalRepository.GetAreaRangeList(department);
        }

        public List<DropdownViewModel> GetCommonConfigDataList(string category)
        {
            return _generalRepository.GetCommonConfigDataList(category);
        }

        public List<DropdownViewModel> GetSchemeTypeList()
        {
            return _generalRepository.GetSchemeTypeList();
        }

        public List<DropdownViewModel> GetCommonConfigCategoryList()
        {
            return _generalRepository.GetCommonConfigCategoryList();
        }

        public List<DropdownViewModel> GetPropertyTypeByDepartment(int departmentId)
        {
            return _generalRepository.GetPropertyTypeByDepartment(departmentId);
        }


        public List<DropdownViewModel> GetStatusList()
        {
            return _generalRepository.GetStatusList();
        }

        public List<DropdownViewModel> GetStatusListByType(string type)
        {
            return _generalRepository.GetStatusListByType(type);
        }

        public List<DropdownViewModel> GetStatusTypeList()
        {
            return _generalRepository.GetStatusTypeList();
        }

        public int SaveApplication(CommonViewModel model)
        {
            return _generalRepository.SaveApplication(model);
        }

        public int SaveSchemeType(CommonViewModel model)
        {
            return _generalRepository.SaveSchemeType(model);
        }

        public int SavePropertyType(CommonViewModel model)
        {
            return _generalRepository.SavePropertyType(model);
        }

        public int SavePropertyAreaRange(CommonViewModel model)
        {
            return _generalRepository.SavePropertyAreaRange(model);
        }

        public int SaveApplicationMenu(CommonViewModel model)
        {
            return _generalRepository.SaveApplicationMenu(model);
        }

        public int SaveStatusByType(CommonViewModel model)
        {
            return _generalRepository.SaveStatusByType(model);
        }

        public int SaveCommonConfigData(CommonViewModel model)
        {
            return _generalRepository.SaveCommonConfigData(model);
        }

        public int SaveDepartment(CommonViewModel model)
        {
            return _generalRepository.SaveDepartment(model);
        }


        public CommonViewModel GetPropertyTypeDetailById(CommonViewModel model)
        {
            return _generalRepository.GetPropertyTypeDetailById(model);
        }


        public DataSourceResult GetFrequencyList(DataSourceRequest request)
        {
            return _generalRepository.GetFrequencyList(request);
        }


        public DataSourceResult GetApproverIdList(DataSourceRequest request)
        {
            return _generalRepository.GetApproverIdList(request);
        }


        public DataSourceResult GetRegistrationIdListAsDataSource(DataSourceRequest request)
        {
            return _generalRepository.GetRegistrationIdListAsDataSource(request);
        }

        public AllotmentModel GetApplicantDetails(int? rid)
        {
            return _generalRepository.GetApplicantDetails(rid);
        }


        public DataSourceResult GetPropertyListForAllotment(DataSourceRequest request, int? schemeId, int? departmentId)
        {
            return _generalRepository.GetPropertyListForAllotment(request, schemeId, departmentId);
        }

        public DataSourceResult GetOnlineApplicationFormIdListForAllotment(DataSourceRequest request, int? schemeId, int? departmentId)
        {
            return _generalRepository.GetOnlineApplicationFormIdListForAllotment(request, schemeId, departmentId);
        }

        public DataSourceResult GetApplicationFormListByScheme(DataSourceRequest request, int? schemeId, int? departmentId)
        {
            return _generalRepository.GetApplicationFormListByScheme(request, schemeId, departmentId);
        }


        public DataSourceResult GetStatusMasterAsDataSource(DataSourceRequest request)
        {
            return _generalRepository.GetStatusMasterAsDataSource(request);
        }


        public CommonViewModel GetCommonConfigDataById(CommonViewModel model)
        {
            return _generalRepository.GetCommonConfigDataById(model);
        }


        public DataSourceResult GetServiceListAsDataSource(DataSourceRequest request)
        {
            return _generalRepository.GetServiceListAsDataSource(request);
        }

        public DataSourceResult GetServicesByDepartmentAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetServicesByDepartmentAsDataSource(request, model);
        }


        public DataSourceResult GetBankListAsDataSource(DataSourceRequest request)
        {
            return _generalRepository.GetBankListAsDataSource(request);
        }

        public DataSourceResult GetBranchListOfBankAsDataSource(DataSourceRequest request, int? bankId)
        {
            return _generalRepository.GetBranchListOfBankAsDataSource(request, bankId);
        }

        public int SaveTempChallanChargeDetail(ChallanViewModel model)
        {
            return _generalRepository.SaveTempChallanChargeDetail(model);
        }

        public int SaveTempChallanChargeDetailII(ChallanViewModel model)
        {
            return _generalRepository.SaveTempChallanChargeDetailII(model);
        }

        public int RemoveTempChallanChargeDetail(ChallanViewModel model)
        {
            return _generalRepository.RemoveTempChallanChargeDetail(model);
        }

        public int RemoveTempChallanChargeDetailII(ChallanViewModel model)
        {
            return _generalRepository.RemoveTempChallanChargeDetailII(model);
        }

        public DataSourceResult GetTempChallanChargesAsDataSource(DataSourceRequest request, ChallanViewModel model)
        {
            return _generalRepository.GetTempChallanChargesAsDataSource(request, model);
        }

        public DataSourceResult GetTempChallanChargesAsDataSourceII(DataSourceRequest request, ChallanViewModel model)
        {
            return _generalRepository.GetTempChallanChargesAsDataSourceII(request, model);
        }

        public PropertyDetailViewModel GetAllottedPropertyDetailByRegistrationId(int? rid)
        {
            return _generalRepository.GetAllottedPropertyDetailByRegistrationId(rid);
        }


        public DataSourceResult GetReceiptHeadListAsDataSource(DataSourceRequest request)
        {
            return _generalRepository.GetReceiptHeadListAsDataSource(request);
        }

        public DataSourceResult GetReceiptSubHeadListAsDataSource(DataSourceRequest request, int? id)
        {
            return _generalRepository.GetReceiptSubHeadListAsDataSource(request, id);
        }


        public DataSourceResult GetLetterTemplateListByDepartmentAsDataSource(DataSourceRequest request, int? departmentId)
        {
            return _generalRepository.GetLetterTemplateListByDepartmentAsDataSource(request, departmentId);
        }


        public LetterViewModel GenerateAuthorizedLetterById(LetterViewModel model)
        {
            return _generalRepository.GenerateAuthorizedLetterById(model);
        }


        public LetterViewModel GetGeneratedLetterByBarcode(LetterViewModel model)
        {
            return _generalRepository.GetGeneratedLetterByBarcode(model);
        }


        public int SendRegisteredMessageToApplicant(ApplicantViewModel model)
        {
            return _generalRepository.SendRegisteredMessageToApplicant(model);
        }


        public DataSourceResult GetDepartmentListAsDataSource(DataSourceRequest request)
        {
            return _generalRepository.GetDepartmentListAsDataSource(request);
        }

        public DataSourceResult GetDepartmentListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetDepartmentListAsDataSource(request, model);
        }


        public DataSourceResult GetNotingFileTypeListAsDataSource(DataSourceRequest request)
        {
            return _generalRepository.GetNotingFileTypeListAsDataSource(request);
        }

        public DataSourceResult GetSchemeListAsDataSource(DataSourceRequest request)
        {
            return _generalRepository.GetSchemeListAsDataSource(request);
        }


        public DataSourceResult GetApplicationFormListAsDataSource(DataSourceRequest request, int? schemeId, int? departmentId)
        {
            return _generalRepository.GetApplicationFormListAsDataSource(request, schemeId, departmentId);
        }


        public DataSourceResult GetAllottedPropertyIdListAsDataSource(DataSourceRequest request, int? schemeId, int? departmentId, int? applicationId)
        {
            return _generalRepository.GetAllottedPropertyIdListAsDataSource(request, schemeId, departmentId, applicationId);
        }


        public DataSourceResult GetUserStatusListAsDataSource(DataSourceRequest request)
        {
            return _generalRepository.GetUserStatusListAsDataSource(request);
        }


        public DataSourceResult GetSectorListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            return _generalRepository.GetSectorListAsDataSource(request, model);
        }

        public DataSourceResult GetBlockListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            return _generalRepository.GetBlockListAsDataSource(request, model);
        }

        public DataSourceResult GetSectorListAsDataSourceII(DataSourceRequest request, PropertyViewModel model)
        {
            return _generalRepository.GetSectorListAsDataSourceII(request, model);
        }

        public DataSourceResult GetBlockListAsDataSourceII(DataSourceRequest request, PropertyViewModel model)
        {
            return _generalRepository.GetBlockListAsDataSourceII(request, model);
        }

        public DataSourceResult GetPlotListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            return _generalRepository.GetPlotListAsDataSource(request, model);
        }


        public DataSourceResult GetMasterSearchParameterAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            return _generalRepository.GetMasterSearchParameterAsDataSource(request, model);
        }


        public DataSourceResult GetSecurityQuestioinListAsDataSource(DataSourceRequest request)
        {
            return _generalRepository.GetSecurityQuestioinListAsDataSource(request);
        }


        public DataSourceResult GetBankAccountDetailAsDataSource(DataSourceRequest request, BankAccountViewModel model)
        {
            return _generalRepository.GetBankAccountDetailAsDataSource(request, model);
        }

        public int SaveBankAccountDetail(BankAccountViewModel model)
        {
            return _generalRepository.SaveBankAccountDetail(model);
        }

        public int SaveServiceDetailById(ServiceViewModel model)
        {
            return _generalRepository.SaveServiceDetailById(model);
        }

        public BankAccountViewModel GetBankAccountDetailById(BankAccountViewModel model)
        {
            return _generalRepository.GetBankAccountDetailById(model);
        }

        public ServiceViewModel GetServiceDetailById(ServiceViewModel model)
        {
            return _generalRepository.GetServiceDetailById(model);
        }


        public int SavePropertyServiceType(ServiceViewModel model)
        {
            return _generalRepository.SavePropertyServiceType(model);
        }


        public DataSourceResult GetPropertyTypeByDepartmentAsDataSource(DataSourceRequest request, int departmentId)
        {
            return _generalRepository.GetPropertyTypeByDepartmentAsDataSource(request, departmentId);
        }


        public DataSourceResult GetDefaultSectorListAsDataSource(DataSourceRequest request)
        {
            return _generalRepository.GetDefaultSectorListAsDataSource(request);
        }

        public DataSourceResult GetDefaultBlockListAsDataSource(DataSourceRequest request)
        {
            return _generalRepository.GetDefaultBlockListAsDataSource(request);
        }


        public DataSourceResult GetApplicationIdListAsDataSource(DataSourceRequest request)
        {
            return _generalRepository.GetApplicationIdListAsDataSource(request);
        }


        public DataSourceResult GetAllotteeIdTypeList(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetAllotteeIdTypeList(request, model);
        }

        public DataSourceResult GetOwnershipFileTypeList(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetOwnershipFileTypeList(request, model);
        }


        public int SendOTP(CommonViewModel model)
        {
            return _generalRepository.SendOTP(model);
        }

        public int ValidateOTP(CommonViewModel model)
        {
            return _generalRepository.ValidateOTP(model);
        }


        public PropertyViewModel GetPropertyDetailById(PropertyViewModel model)
        {
            return _generalRepository.GetPropertyDetailById(model);
        }


        public DataSourceResult GetOptionalReasonAsDataSource(DataSourceRequest request)
        {
            return _generalRepository.GetOptionalReasonAsDataSource(request);
        }


        public int CheckChallanSessionDataById(ChallanViewModel model)
        {
            return _generalRepository.CheckChallanSessionDataById(model);
        }


        public List<DropdownViewModel> GetVillageIdList()
        {
            return _generalRepository.GetVillageIdList();
        }



        public int VerifyChallanDetailById(ChallanViewModel model)
        {
            return _generalRepository.VerifyChallanDetailById(model);
        }


        public LetterViewModel GenerateNDCByRegistrationId(LetterViewModel model)
        {
            return _generalRepository.GenerateNDCByRegistrationId(model);
        }


        public DataSourceResult GetOSDApproverIdList(DataSourceRequest request)
        {
            return _generalRepository.GetOSDApproverIdList(request);
        }


        public int IsRegistrationIdExist(PropertyViewModel model)
        {
            return _generalRepository.IsRegistrationIdExist(model);
        }


        public DropdownViewModel GetServiceTypeDetailById(DropdownViewModel model)
        {
            return _generalRepository.GetServiceTypeDetailById(model);
        }

        public List<DashboardPropertyVM> GetDashboardPropertyData(int departmentId)
        {
            return _generalRepository.GetDashboardPropertyData(departmentId);
        }


        public KYAViewModel GetKYADetailsForRid(KYAViewModel model)
        {
            return _generalRepository.GetKYADetailsForRid(model);
        }


        public CommonViewModel ActivateDetailsOnId(CommonViewModel model)
        {
            return _generalRepository.ActivateDetailsOnId(model);
        }


        public DataSourceResult GetRegistrationIdListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetRegistrationIdListAsDataSource(request, model);
        }


        public List<DropdownViewModel> GetAllotteeTypeList()
        {
            return _generalRepository.GetAllotteeTypeList();
        }


        public DataSourceResult GetFloorAreaListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetFloorAreaListAsDataSource(request, model);
        }

        public DataSourceResult GetLocationTypeListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetLocationTypeListAsDataSource(request, model);
        }

        public LoginUserDetail GetLoginUserDetails(int userId)
        {
            return _generalRepository.GetLoginUserDetails(userId);
        }


        public DataSourceResult GetReceiptHeadIdListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetReceiptHeadIdListAsDataSource(request, model);
        }


        public int IsRequestIdIsExist(ServiceViewModel model)
        {
            return _generalRepository.IsRequestIdIsExist(model);
        }


        public DataSourceResult GetSchemeRefundTypeListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetSchemeRefundTypeListAsDataSource(request, model);
        }


        public DataSourceResult GetApproverIdListByDepartment(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetApproverIdListByDepartment(request, model);
        }


        public int SendReminderToAllottee(int? Rid)
        {
            return _generalRepository.SendReminderToAllottee(Rid);
        }


        public int IsRidExists(int? Id, int? RegistrationId, string PropertyNo)
        {
            return _generalRepository.IsRidExists(Id, RegistrationId, PropertyNo);
        }


        public int ValidateUpdatedProperty(PropertyViewModel model)
        {
            return _generalRepository.ValidateUpdatedProperty(model);
        }


        public DataSourceResult GetAllotmentYearAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetAllotmentYearAsDataSource(request, model);
        }


        public DataSourceResult GetUsersDepartmentListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetUsersDepartmentListAsDataSource(request, model);
        }

        public DataSourceResult GetStatusListByRoleAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetStatusListByRoleAsDataSource(request, model);
        }


        public DataSourceResult GetApproverIdByDepartmentAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetApproverIdByDepartmentAsDataSource(request, model);
        }


        public List<DropdownViewModel> GetYearsList()
        {
            return _generalRepository.GetYearsList();
        }


        public List<DropdownViewModel> GetServiceListByDepartmentForNAServices(int departmentId)
        {
            return _generalRepository.GetServiceListByDepartmentForNAServices(departmentId);
        }


        public DataSourceResult GetDocumentTypeListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetDocumentTypeListAsDataSource(request, model);
        }


        public EmployeeViewModel GetEmployeeDetailsById(EmployeeViewModel model)
        {
            return _generalRepository.GetEmployeeDetailsById(model);
        }


        public DataSourceResult GetEmployeeListAsdatasource(DataSourceRequest request)
        {
            return _generalRepository.GetEmployeeListAsdatasource(request);
        }


        public int RemoveTempChallanChargeDetailForEmployee(ChallanViewModel model)
        {
            return _generalRepository.RemoveTempChallanChargeDetailForEmployee(model);
        }


        public int SaveTempChallanChargeDetailForEmployee(ChallanViewModel model)
        {
            return _generalRepository.SaveTempChallanChargeDetailForEmployee(model);
        }


        public decimal GetTotalAmount(decimal? amount)
        {
            return _generalRepository.GetTotalAmount(amount);
        }


        public DataSourceResult GetTempChallanChargesForEmployee(DataSourceRequest request)
        {
            return _generalRepository.GetTempChallanChargesForEmployee(request);
        }


        public DataSourceResult GetBankListForPaymentAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _generalRepository.GetBankListForPaymentAsDataSource(request, model);
        }
    }
}
