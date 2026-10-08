using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using NA.PMS.Model;

namespace NA.PMS.Service
{
    public interface ISchemeService
    {
        int AddScheme(string schemeName, int schemeType, DateTime startDate, DateTime endDate, int userID, decimal formFee);
        bool AddDeptt(int departmentId, double normalInterest, double penalInterest, int frequency, int noInstallment, double allotmentmoney, double installmentmoney, double leaseRent, double floorArearatio, int schemeId, int userID, string selectionType);
        int AddQuota(int quotaDepartmentId, int quotaId, string value, string UnitId, int schemeId, int userID);
        int AddRebate(int rebatedepartment, int rebateId, string rebateValue, string rebateUnitId, int schemeId, int userID);
        bool AddCost(int depttID, int propType, int sector, int floor, int block, decimal processingFee, double earnestmoney, double propCost, double landRate, double civilCost, double totPropCost, double allotmentMoney, double leaseRent, int schemeId, int userID);
        bool AddBank(int bankId, int branchId, string branchName, int schemeId, int userID, string newaccountNumber, string newBank);
        DataSourceResult GetAllScheme(DataSourceRequest sourceReq);
        DataSourceResult GetDepttData(DataSourceRequest request, int schemeId);
        DataSourceResult GetQuotaData(DataSourceRequest request, int schemeId);
        DataSourceResult GetBankData(DataSourceRequest request, int schemeId);
        DataSourceResult GetRebateData(DataSourceRequest request, int schemeId);
        DataSourceResult GetCostData(DataSourceRequest request, int schemeId);
        Scheme GetSChemeByID(int schemeId);
        bool RemoveDepttRecord(int refId, int schemeID);
        bool RemoveQuotaRecord(int refId, int schemeID);
        bool RemoveRebRecord(int refId, int schemeID);
        bool RemoveCostRecord(int refId, int schemeID);
        bool RemoveBankRecord(int refId, int schemeID);
        bool ActivateScheme(int schemeId);
        List<DDLStringList> GetAccountNumber(int bankId, int branchId);
        bool IsSchemeNameUnique(string schemeName, int schemeid);
        bool IsDepatmnetNameUnique(int deptId, int schemeid, int flag, int type, int floorddl, int block, int sector);
        bool DeleteRecordById(int schemeid);
        bool IsBankNameUnique(int bankid, string bankName);
        bool IsAccountNumberDuplicate(int bankid, string accountNumber);
        bool UpdateScheme(int schemeId, DateTime startDate, DateTime endDate, int userID, decimal formFee);

        bool IsCostNameUnique(int deptId, int schemeid, int flag, int typesector, int floorddl, int block);
        DataSourceResult GetLandDevelopmentScheduleData(DataSourceRequest sourceReq, int schemeId);
        int AddLandDevelopmentSchedule(int departmentId, int processId, int duration, int triggerId, int schemeId, int userID);
        bool RemoveLandDevelopment(int scheduleId, int schemeID);
        bool IsLandDevelopmentUnique(int departmentID, int processId, int schemeID, int triggerId, int type);
        
        //Update Scheme Status
        bool GetStatusUpdated(int schemeID);

        SchemeViewModel GetSchemeDetailById(SchemeViewModel model);
    }
}
