using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Repository;
using Kendo.Mvc.UI;
using NA.PMS.Model;

namespace NA.PMS.Service
{
    public class SchemeService : ISchemeService
    {
        private ISchemeRepository _schemeRepository;
        public SchemeService(ISchemeRepository schemeRepository)
        {
            _schemeRepository = schemeRepository;
        }
        public int AddScheme(string schemeName, int schemeType, DateTime startDate, DateTime endDate, int userID, decimal formFee)
        {
            return _schemeRepository.AddScheme(schemeName, schemeType, startDate, endDate, userID, formFee);
        }
        public bool AddDeptt(int departmentId, double normalInterest, double penalInterest, int frequency, int noInstallment, double allotmentmoney, double installmentmoney, double leaseRent, double floorArearatio, int schemeId, int userID, string selectionType)
        {
            return _schemeRepository.AddDeptt(departmentId, normalInterest, penalInterest, frequency, noInstallment, allotmentmoney, installmentmoney, leaseRent, floorArearatio, schemeId, userID, selectionType);
        }
        public int AddQuota(int quotaDepartmentId, int quotaId, string value, string UnitId, int schemeId, int userID)
        {
            return _schemeRepository.AddQuota(quotaDepartmentId, quotaId, value, UnitId, schemeId, userID);
        }
        public int AddRebate(int rebatedepartment, int rebateId, string rebateValue, string rebateUnitId, int schemeId, int userID)
        {
            return _schemeRepository.AddRebate(rebatedepartment, rebateId, rebateValue, rebateUnitId, schemeId, userID);
        }
        public bool AddCost(int depttID, int propType, int sector, int floor, int block, decimal processingFee, double earnestmoney, double propCost, double landRate, double civilCost, double totPropCost, double allotmentMoney, double leaseRent, int schemeId, int userID)
        {
            return _schemeRepository.AddCost(depttID, propType, sector, floor, block,processingFee, earnestmoney, propCost, landRate, civilCost, totPropCost, allotmentMoney, leaseRent, schemeId, userID);
        }
        public bool AddBank(int bankId, int branchId, string branchName, int schemeId, int userID, string newaccountNumber, string newBank)
        {
            return _schemeRepository.AddBank(bankId, branchId, branchName, schemeId, userID, newaccountNumber, newBank);
        }
        public DataSourceResult GetAllScheme(DataSourceRequest sourceReq)
        {
            return _schemeRepository.GetAllScheme(sourceReq);
        }
        public DataSourceResult GetDepttData(DataSourceRequest request, int schemeId)
        {
            return _schemeRepository.GetDepttData(request, schemeId);
        }
        //
        public DataSourceResult GetQuotaData(DataSourceRequest request, int schemeId)
        {
            return _schemeRepository.GetQuotaData(request, schemeId);
        }
        public DataSourceResult GetBankData(DataSourceRequest request, int schemeId)
        {
            return _schemeRepository.GetBankData(request, schemeId);
        }
        public DataSourceResult GetRebateData(DataSourceRequest request, int schemeId)
        {
            return _schemeRepository.GetRebateData(request, schemeId);
        }
        public DataSourceResult GetCostData(DataSourceRequest request, int schemeId)
        {
            return _schemeRepository.GetCostData(request, schemeId);
        }
        public Scheme GetSChemeByID(int schemeId)
        {
            return _schemeRepository.GetSChemeByID(schemeId);
        }
        public bool RemoveDepttRecord(int refId, int schemeID)
        {
            return _schemeRepository.RemoveDepttRecord(refId, schemeID);
        }
        public bool RemoveQuotaRecord(int refId, int schemeID)
        {
            return _schemeRepository.RemoveQuotaRecord(refId, schemeID);
        }

        public bool RemoveRebRecord(int refId, int schemeID)
        {
            return _schemeRepository.RemoveRebRecord(refId, schemeID);
        }

        public bool RemoveCostRecord(int refId, int schemeID)
        {
            return _schemeRepository.RemoveCostRecord(refId, schemeID);
        }

        public bool RemoveBankRecord(int refId, int schemeID)
        {
            return _schemeRepository.RemoveBankRecord(refId, schemeID);
        }

        public bool ActivateScheme(int schemeId)
        {
            return _schemeRepository.ActivateScheme(schemeId);
        }

        public List<DDLStringList> GetAccountNumber(int bankId, int branchId)
        {
            return _schemeRepository.GetAccountNumber(bankId, branchId);
        }

        public bool IsSchemeNameUnique(string schemeName, int schemeid)
        {
            return _schemeRepository.IsSchemeNameUnique(schemeName, schemeid);
        }
        public bool IsDepatmnetNameUnique(int deptId, int schemeid, int flag, int type, int floorddl, int block, int sector)
        {
            return _schemeRepository.IsDepatmnetNameUnique(deptId, schemeid, flag, type, floorddl, block, sector);
        }

        public bool DeleteRecordById(int schemeid)
        {
            return _schemeRepository.DeleteRecordById(schemeid);
        }

        public bool IsBankNameUnique(int bankid, string bankName)
        {
            return _schemeRepository.IsBankNameUnique(bankid, bankName);
        }

        public bool IsAccountNumberDuplicate(int bankid, string accountNumber)
        {
            return _schemeRepository.IsAccountNumberDuplicate(bankid, accountNumber);
        }

        public bool UpdateScheme(int schemeId, DateTime startDate, DateTime endDate, int userID, decimal formFee)
        {
            return _schemeRepository.UpdateScheme(schemeId, startDate, endDate, userID, formFee);
        }
        //public bool IsbranchNameDuplicate(int bankid, string branchName)
        //{
        //    return _schemeRepository.IsbranchNameDuplicate(bankid, branchName);
        //}
        public bool IsCostNameUnique(int deptId, int schemeid, int flag, int typesector, int floorddl, int block)
        {
            return _schemeRepository.IsCostNameUnique(deptId, schemeid, flag, typesector, floorddl, block);
        }
        public DataSourceResult GetLandDevelopmentScheduleData(DataSourceRequest request, int schemeId)
        {
            return _schemeRepository.GetLandDevelopmentScheduleData(request, schemeId);
        }
        public int AddLandDevelopmentSchedule(int departmentId, int processId, int duration, int triggerId, int schemeId, int userID)
        {
            return _schemeRepository.AddLandDevelopmentSchedule(departmentId, processId, duration, triggerId, schemeId, userID);
        }

        public bool RemoveLandDevelopment(int scheduleId, int schemeID)
        {
            return _schemeRepository.RemoveLandDevelopment(scheduleId, schemeID);
        }
        public bool IsLandDevelopmentUnique(int departmentID, int processId, int schemeID, int triggerId, int type)
        {
            return _schemeRepository.IsLandDevelopmentUnique(departmentID, processId, schemeID, triggerId, type);
        }

        //Update Scheme Status
        public bool GetStatusUpdated(int schemeID)
        {
            return _schemeRepository.GetStatusUpdated(schemeID);
        }



        public SchemeViewModel GetSchemeDetailById(SchemeViewModel model)
        {
            return _schemeRepository.GetSchemeDetailById(model);
        }
    }
}
