using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Model;

using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Common;
using Kendo.Mvc;
using System.Web;
using NA.PMS.Web.Models;



namespace NA.PMS.Repository
{
    public class SchemeRepository : ISchemeRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();

        public SchemeRepository()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["CurrentUser"] != null)
                    {
                        userInfo = HttpContext.Current.Session["CurrentUser"] as CurrentUserDetail;
                    }
                }
            }
        }
        /// <summary>
        /// Add Scheme used to add new scheme.
        /// </summary>
        /// <param name="schemeName"></param>
        /// <param name="schemeType"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public int AddScheme(string schemeName, int schemeType, DateTime startDate, DateTime endDate, int UserID, decimal formFee)
        {
            var dataResult = new SchemeMst();
            var flag = 0;
            using (var dbcontext = new NoidaPMSEntities())
            {
                var varscheme = dbcontext.SchemeMsts.FirstOrDefault(r => r.schemeName.ToLower() == schemeName.ToLower() && r.IsActive == true);
                if (varscheme != null)
                {
                    flag = -1;
                }
                else
                {
                    dataResult.schemeName = schemeName;
                    dataResult.schemeTypeId = schemeType;
                    dataResult.startDate = startDate;
                    dataResult.endDate = endDate;
                    dataResult.IsActive = true;
                    dataResult.createdBy = UserID.ToString();
                    dataResult.createdDate = DateTime.Now;
                    dataResult.completed = false;
                    dataResult.FormFee = formFee;
                    dbcontext.SchemeMsts.Add(dataResult);
                    dbcontext.SaveChanges();
                    flag = dataResult.schemeId;
                }
            }

            return flag;
        }
        /// <summary>
        /// AddDeptt used to add department, for which scheme are being made.
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="normalInterest"></param>
        /// <param name="penalInterest"></param>
        /// <param name="frequency"></param>
        /// <param name="noInstallment"></param>
        /// <param name="allotmentmoney"></param>
        /// <param name="installmentmoney"></param>
        /// <param name="leaseRent"></param>
        /// <param name="floorArearatio"></param>
        /// <param name="schemeId"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool AddDeptt(int departmentId, double normalInterest, double penalInterest, int frequency, int noInstallment, double allotmentmoney, double installmentmoney, double leaseRent, double floorArearatio, int schemeId, int userID, string selectionType)
        {
            var datadepartment = new SchemeDepartmentTran();
            var result = false;
            using (var dbcontext = new NoidaPMSEntities())
            {
                var varscheme = dbcontext.SchemeDepartmentTrans.FirstOrDefault(r => r.departmentId == departmentId && r.schemeId == schemeId && r.IsActive == true);
                if (varscheme == null)
                {
                    datadepartment.schemeId = schemeId;
                    datadepartment.departmentId = departmentId;
                    datadepartment.normalInt = normalInterest;
                    datadepartment.penalInt = penalInterest;
                    datadepartment.frequency = frequency;
                    datadepartment.noOfInstallments = noInstallment;
                    datadepartment.far = floorArearatio;
                    datadepartment.allotmentMoneyPercent = allotmentmoney;
                    datadepartment.installmentMoneyPercent = installmentmoney;
                    datadepartment.leaseRentPercent = leaseRent;
                    datadepartment.IsActive = true;
                    datadepartment.selectionType = selectionType;
                    datadepartment.createdBy = userID.ToString();
                    datadepartment.createdDate = DateTime.Now;
                    dbcontext.SchemeDepartmentTrans.Add(datadepartment);
                    dbcontext.SaveChanges();
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }
        /// <summary>
        ///  AddQuota used to add Quota, for which scheme are being made.
        /// </summary>
        /// <param name="quotaDepartmentId"></param>
        /// <param name="quotaId"></param>
        /// <param name="value"></param>
        /// <param name="UnitId"></param>
        /// <param name="schemeId"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public int AddQuota(int quotaDepartmentId, int quotaId, string value, string UnitId, int schemeId, int userID)
        {
            var quotaObj = new SchemeQuotaTran();
            int result;
            decimal dValue = Convert.ToDecimal(value);
            using (var dbcontext = new NoidaPMSEntities())
            {
                var duplicateData = dbcontext.SchemeQuotaTrans.FirstOrDefault(r => r.departmentId == quotaDepartmentId && r.schemeId == schemeId && r.quotaId == quotaId && r.IsActive == true);
                if (duplicateData == null)
                {
                    var units = (from schemeQuota in dbcontext.SchemeQuotaTrans
                                 where schemeQuota.schemeId == schemeId && schemeQuota.departmentId == quotaDepartmentId && schemeQuota.IsActive == true
                                 select schemeQuota).ToList();
                    if (units.Count > 0)
                    {
                        var unitsExit = units[0].unit.Contains(UnitId);
                        if (unitsExit == true)
                        {
                            if (UnitId == "PER")
                            {
                                decimal sum = 0m;
                                foreach (var item in units)
                                {
                                    sum = sum + Convert.ToDecimal(item.value);
                                }
                                if ((Convert.ToDecimal(value) + sum) > 100)
                                {
                                    result = 4;
                                    return result;
                                }
                            }
                            quotaObj.schemeId = schemeId;
                            quotaObj.departmentId = quotaDepartmentId;
                            quotaObj.quotaId = quotaId;
                            quotaObj.value = Convert.ToDecimal(value);
                            quotaObj.unit = UnitId.ToString();
                            quotaObj.createdBy = userID.ToString();
                            quotaObj.createdDate = DateTime.Now;
                            quotaObj.IsActive = true;
                            dbcontext.SchemeQuotaTrans.Add(quotaObj);
                            dbcontext.SaveChanges();
                            result = 1;
                        }
                        else
                        {
                            result = 2;
                        }

                    }
                    else
                    {
                        quotaObj.schemeId = schemeId;
                        quotaObj.departmentId = quotaDepartmentId;
                        quotaObj.quotaId = quotaId;
                        quotaObj.value = Convert.ToDecimal(value);
                        quotaObj.unit = UnitId.ToString();
                        quotaObj.createdBy = userID.ToString();
                        quotaObj.createdDate = DateTime.Now;
                        quotaObj.IsActive = true;
                        dbcontext.SchemeQuotaTrans.Add(quotaObj);
                        dbcontext.SaveChanges();
                        result = 1;
                    }
                }
                else
                {
                    result = 3;
                }
            }
            return result;
        }
        /// <summary>
        /// AddRebate used to add Rebate, for which scheme are being made.
        /// </summary>
        /// <param name="rebatedepartment"></param>
        /// <param name="rebateId"></param>
        /// <param name="rebateValue"></param>
        /// <param name="rebateUnitId"></param>
        /// <param name="schemeId"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public int AddRebate(int rebatedepartment, int rebateId, string rebateValue, string rebateUnitId, int schemeId, int userID)
        {
            var rebateObj = new SchemeRebateTran();
            int result;
            using (var dbcontext = new NoidaPMSEntities())
            {
                var varRebate = dbcontext.SchemeRebateTrans.FirstOrDefault(r => r.departmentId == rebatedepartment && r.schemeId == schemeId && r.rebateId == rebateId && r.IsActive == true);
                if (varRebate == null)
                {
                    var units = (from schemeRebate in dbcontext.SchemeRebateTrans
                                 where schemeRebate.schemeId == schemeId && schemeRebate.departmentId == rebatedepartment && schemeRebate.IsActive == true
                                 select schemeRebate).ToList();
                    if (units.Count > 0)
                    {
                        var unitsExit = units[0].unit.Contains(rebateUnitId);
                        if (unitsExit == true)
                        {
                            if (rebateUnitId == "PER")
                            {
                                decimal sum = 0m;
                                foreach (var item in units)
                                {
                                    sum = sum + Convert.ToDecimal(item.value);
                                }
                                if ((Convert.ToDecimal(rebateValue) + sum) > 100)
                                {
                                    result = 4;
                                    return result;
                                }
                            }
                            rebateObj.schemeId = schemeId;
                            rebateObj.departmentId = rebatedepartment;
                            rebateObj.rebateId = rebateId;
                            rebateObj.value = Convert.ToDecimal(rebateValue);
                            rebateObj.unit = rebateUnitId.ToString();
                            rebateObj.createdBy = userID.ToString();
                            rebateObj.createdDate = DateTime.Now;
                            rebateObj.IsActive = true;
                            dbcontext.SchemeRebateTrans.Add(rebateObj);
                            dbcontext.SaveChanges();
                            result = 1;
                        }
                        else
                        {
                            result = 2;
                        }
                    }
                    else
                    {
                        rebateObj.schemeId = schemeId;
                        rebateObj.departmentId = rebatedepartment;
                        rebateObj.rebateId = rebateId;
                        rebateObj.value = Convert.ToDecimal(rebateValue);
                        rebateObj.unit = rebateUnitId.ToString();
                        rebateObj.createdBy = userID.ToString();
                        rebateObj.createdDate = DateTime.Now;
                        rebateObj.IsActive = true;
                        dbcontext.SchemeRebateTrans.Add(rebateObj);
                        dbcontext.SaveChanges();
                        result = 1;
                    }
                }
                else
                {
                    result = 3;
                }
            }
            return result;
        }
        /// <summary>
        /// AddCost used to add Cost, for scheme are being made.
        /// </summary>
        /// <param name="depttID"></param>
        /// <param name="propType"></param>
        /// <param name="sector"></param>
        /// <param name="floor"></param>
        /// <param name="block"></param>
        /// <param name="earnestmoney"></param>
        /// <param name="propCost"></param>
        /// <param name="landRate"></param>
        /// <param name="civilCost"></param>
        /// <param name="totPropCost"></param>
        /// <param name="allotmentMoney"></param>
        /// <param name="leaseRent"></param>
        /// <param name="schemeId"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool AddCost(int depttID, int propType, int sector, int floor, int block, decimal processingFee, double earnestmoney, double propCost, double landRate, double civilCost, double totPropCost, double allotmentMoney, double leaseRent, int schemeId, int userID)
        {
            var costObj = new SchemeCostTran();
            var result = false;
            using (var dbcontext = new NoidaPMSEntities())
            {
                SchemeCostTran schemeCostTran = new SchemeCostTran();
                if (depttID != (int)Common.Departmentenum.Housing)
                {
                    schemeCostTran = dbcontext.SchemeCostTrans.FirstOrDefault(r => r.departmentId == depttID && r.schemeId == schemeId && r.propertyTypeId == propType && r.IsActive == true && r.sectorId == sector && r.floorId == floor && r.blockId == block);
                }
                else
                {
                    schemeCostTran = dbcontext.SchemeCostTrans.FirstOrDefault(r => r.departmentId == depttID && r.schemeId == schemeId && r.propertyTypeId == propType && r.blockId.Value == block && r.floorId.Value == floor && r.sectorId == sector && r.IsActive == true);
                }

                if (schemeCostTran == null)
                {
                    costObj.schemeId = schemeId;
                    costObj.departmentId = depttID;
                    costObj.propertyTypeId = propType;
                    costObj.sectorId = sector;
                    costObj.floorId = floor;
                    costObj.blockId = block;
                    costObj.earnestMoney = Convert.ToDecimal(earnestmoney);
                    costObj.propertyCost = Convert.ToDecimal(propCost);
                    costObj.landRatePerSqmt = Convert.ToDecimal(landRate);
                    costObj.civilCost = Convert.ToDecimal(civilCost);
                    costObj.totalPropertyCost = Convert.ToDecimal(totPropCost);
                    costObj.allotmentMoney = Convert.ToDecimal(allotmentMoney);
                    costObj.leaseRent = Convert.ToDecimal(leaseRent);
                    //costObj
                    costObj.createdBy = userID.ToString();
                    costObj.createdDate = DateTime.Now;
                    costObj.IsActive = true;
                    dbcontext.SchemeCostTrans.Add(costObj);
                    dbcontext.SaveChanges();
                    result = true;
                }
                else
                { result = false; }
            }
            return result;
        }
        /// <summary>
        /// AddBank used to add Bank, for that particular scheme.
        /// </summary>
        /// <param name="bankId"></param>
        /// <param name="branchId"></param>
        /// <param name="branchName"></param>
        /// <param name="accountNumber"></param>
        /// <param name="schemeId"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool AddBank(int bankId, int branchId, string branchName, int schemeId, int userID, string newaccountNumber, string newBank)
        {
            var result = false;
            if (bankId == -1)
            {
                var newbankObj = new BankMst();
                using (var dbcontext = new NoidaPMSEntities())
                {
                    newbankObj.bankName = newBank;
                    newbankObj.IsActive = true;
                    newbankObj.createdBy = userID.ToString();
                    newbankObj.createdDate = DateTime.Now;
                    dbcontext.BankMsts.Add(newbankObj);
                    dbcontext.SaveChanges();
                    if (newbankObj.bankId > 0)
                    {
                        var newbranchObj = new BranchMst();
                        newbranchObj.bankId = newbankObj.bankId;
                        newbranchObj.branchName = branchName;
                        newbranchObj.accountNumber = newaccountNumber;
                        newbranchObj.IsActive = true;
                        newbranchObj.createdBy = userID.ToString();
                        newbranchObj.createdDate = DateTime.Now;
                        dbcontext.BranchMsts.Add(newbranchObj);
                        dbcontext.SaveChanges();
                        if (newbranchObj.branchId > 0)
                        {
                            var bankObj = new SchemeBankTran();
                            bankObj.schemeId = schemeId;
                            bankObj.bankId = newbankObj.bankId;
                            bankObj.branchId = newbranchObj.branchId;
                            bankObj.IsActive = true;
                            bankObj.createdBy = userID.ToString();
                            bankObj.createdDate = DateTime.Now;
                            dbcontext.SchemeBankTrans.Add(bankObj);
                            dbcontext.SaveChanges();
                            result = true;
                        }
                    }

                }
                return result;
            }
            else if (bankId != -1 && branchId == -1)
            {
                if (bankId > 0)
                {
                    using (var dbcontext = new NoidaPMSEntities())
                    {
                        var newbranchObj = new BranchMst();
                        newbranchObj.bankId = bankId;
                        newbranchObj.branchName = branchName;
                        newbranchObj.accountNumber = newaccountNumber;
                        newbranchObj.IsActive = true;
                        newbranchObj.createdBy = userID.ToString();
                        newbranchObj.createdDate = DateTime.Now;
                        dbcontext.BranchMsts.Add(newbranchObj);
                        dbcontext.SaveChanges();
                        if (newbranchObj.branchId > 0)
                        {
                            var bankObj = new SchemeBankTran();
                            bankObj.schemeId = schemeId;
                            bankObj.bankId = bankId;
                            bankObj.branchId = newbranchObj.branchId;
                            bankObj.IsActive = true;
                            bankObj.createdBy = userID.ToString();
                            bankObj.createdDate = DateTime.Now;
                            dbcontext.SchemeBankTrans.Add(bankObj);
                            dbcontext.SaveChanges();
                            result = true;
                        }
                    }
                }
                return result;
            }
            else
            {
                var bankObj = new SchemeBankTran();
                using (var dbcontext = new NoidaPMSEntities())
                {
                    bankObj.schemeId = schemeId;
                    bankObj.bankId = bankId;
                    bankObj.branchId = branchId;
                    bankObj.IsActive = true;
                    bankObj.createdBy = userID.ToString();
                    bankObj.createdDate = DateTime.Now;
                    dbcontext.SchemeBankTrans.Add(bankObj);
                    dbcontext.SaveChanges();
                    result = true;
                }
                return result;
            }
        }
        /// <summary>
        /// To get all scheme for manage page.
        /// </summary>
        /// <param name="sourceReq"></param>
        /// <returns></returns>
        public DataSourceResult GetAllScheme(DataSourceRequest sourceReq)
        {
            List<Scheme> schemeList = new List<Scheme>();

            if (sourceReq.Sorts.Count == 0)
            {
                sourceReq.Sorts.Add(new SortDescriptor("scheId",
                    System.ComponentModel.ListSortDirection.Descending));
            }

            using (var dbContext = new NoidaPMSEntities())
            {
                var lstAllScheme = (from rat in dbContext.SchemeMsts
                                    join schtype in dbContext.SchemeTypeMsts on rat.schemeTypeId equals schtype.schemeTypeId
                                    where rat.IsActive == true
                                    //orderby rat.schemeId descending
                                    select new Scheme
                                    {
                                        scheId = rat.schemeId,
                                        schemeName = rat.schemeName,
                                        schemeType = schtype.SchemeTypeDesc,
                                        startDate = rat.startDate,
                                        endDate = rat.endDate,
                                        createdDate = rat.createdDate,
                                        completed = rat.completed,
                                        IsAllotted = (from allot in dbContext.AllotmentMasters where allot.schemeId == rat.schemeId && allot.isActive == 1 select allot.isActive).FirstOrDefault(),
                                        SchemeStatus = rat.Status,
                                        strSchemeStatus = (from cc in dbContext.Common_Config where cc.Id == rat.Status && cc.Is_Active == 1 select cc.Name).FirstOrDefault()
                                    }
                    );
                return lstAllScheme.ToDataSourceResult(sourceReq);
            }
        }
        /// <summary>
        /// To fill the department grid.
        /// </summary>
        /// <param name="sourceReq"></param>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public DataSourceResult GetDepttData(DataSourceRequest sourceReq, int schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstAllDeptt = (from rat in dbContext.SchemeDepartmentTrans
                                   join depp in dbContext.DepartmentMsts on rat.departmentId equals depp.departmentId
                                   where (rat.schemeId == schemeId && rat.IsActive == true)
                                   select new Department
                                   {
                                       departmentId = rat.departmentId,
                                       departmentName = depp.departmentName,
                                       normalInterest = rat.normalInt,
                                       penalInterest = rat.penalInt,
                                       frequency = rat.frequency,
                                       frequencyName = rat.frequency == 1 ? Frequency.Yearly.ToString() : rat.frequency == 2 ? Frequency.HalfYearly.ToString() : rat.frequency == 4 ? Frequency.Quarterly.ToString() : Frequency.Monthly.ToString(),
                                       noInstallment = rat.noOfInstallments,
                                       allotmentmoney = rat.allotmentMoneyPercent,
                                       installmentmoney = rat.installmentMoneyPercent,
                                       leaseRent = rat.leaseRentPercent,
                                       floorArearatio = rat.far,
                                       scheId = rat.schemeId,
                                       SelectionType = rat.selectionType,
                                       refId = rat.refId
                                   }
                     );
                //DataSourceResult allDepartment = null;
                var allDepartment = new DataSourceResult();
                if (lstAllDeptt.Any())
                    allDepartment = lstAllDeptt.ToDataSourceResult(sourceReq);
                return allDepartment;
            }
        }
        /// <summary>
        /// To fill Quota Grid.
        /// </summary>
        /// <param name="sourceReq"></param>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public DataSourceResult GetQuotaData(DataSourceRequest sourceReq, int schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstAllDeptt = new List<Quota>();

                var lstAllQuota = (from rat in dbContext.SchemeQuotaTrans
                                   join deptt in dbContext.DepartmentMsts on rat.departmentId equals deptt.departmentId
                                   join quota in dbContext.QuotaMsts on rat.quotaId equals quota.quotaId
                                   where (rat.schemeId == schemeId && rat.IsActive == true)
                                   select new Quota
                                   {
                                       quotaDepartmentId = rat.departmentId,
                                       quotaId = rat.quotaId,
                                       value = rat.value,
                                       unit = rat.unit,
                                       unitName = rat.unit == "PER" ? "PERCENTAGE" : rat.unit,
                                       quotaDepartmentName = deptt.departmentName,
                                       quotaName = quota.quotaName,
                                       refId = rat.refId
                                   }
                    );
                return lstAllQuota.ToDataSourceResult(sourceReq);
            }
        }
        /// <summary>
        /// To Add banks for particular scheme.
        /// </summary>
        /// <param name="sourceReq"></param>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public DataSourceResult GetBankData(DataSourceRequest sourceReq, int schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstAllDeptt = new List<Banks>();

                var lstAllBank = (from rat in dbContext.SchemeBankTrans
                                  join bank in dbContext.BankMsts on rat.bankId equals bank.bankId
                                  join branch in dbContext.BranchMsts on rat.branchId equals branch.branchId
                                  where rat.schemeId == schemeId && rat.IsActive == true
                                  select new Banks
                                  {
                                      bankId = rat.bankId.Value,
                                      bankName = bank.bankName,
                                      branchName = branch.branchName,
                                      accountNumber = branch.accountNumber,
                                      refId = rat.refId
                                  }
                    );
                //DataSourceResult allBanks = null;
                var allBanks = new DataSourceResult();
                if (lstAllBank.Any())
                    allBanks = lstAllBank.ToDataSourceResult(sourceReq);
                return allBanks;
            }
        }
        /// <summary>
        /// To get all rebate date.
        /// </summary>
        /// <param name="sourceReq"></param>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public DataSourceResult GetRebateData(DataSourceRequest sourceReq, int schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstAllDeptt = new List<Rebate>();

                var lstAllRebate = (from rat in dbContext.SchemeRebateTrans
                                    join rbat in dbContext.RebateMsts on rat.rebateId equals rbat.rebateId
                                    join deptt in dbContext.DepartmentMsts on rat.departmentId equals deptt.departmentId
                                    where (rat.schemeId == schemeId && rat.IsActive == true)
                                    select new Rebate
                                    {
                                        rebatedepartment = rat.departmentId,
                                        rebateDepartmentName = deptt.departmentName,
                                        rebateValue = rat.value,
                                        rebateUnitId = rat.unit,
                                        rebateUnitname = rat.unit == "PER" ? "PERCENTAGE" : rat.unit,
                                        rebateId = rat.rebateId,
                                        rebateName = rbat.rebateName,
                                        refId = rat.refId
                                    }
                    );
                //DataSourceResult allRebates = null;
                var allRebates = new DataSourceResult();
                if (lstAllRebate.Any())
                    allRebates = lstAllRebate.ToDataSourceResult(sourceReq);
                return allRebates;
            }
        }
        /// <summary>
        /// To get all cost related data for givien scheme.
        /// </summary>
        /// <param name="sourceReq"></param>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public DataSourceResult GetCostData(DataSourceRequest sourceReq, int schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstAllDeptt = new List<CostModel>();
                var lstAllCost = (from rat in dbContext.SchemeCostTrans
                                  join proptype in dbContext.PropertyTypeMsts on rat.propertyTypeId equals proptype.propertyTypeId
                                  join floor in dbContext.FloorMsts on rat.floorId equals floor.floorId
                                  join sec in dbContext.SectorMsts on rat.sectorId equals sec.sectorId
                                  join deptt in dbContext.DepartmentMsts on rat.departmentId equals deptt.departmentId
                                  join block in dbContext.BlockMsts on rat.blockId equals block.blockId
                                  where (rat.schemeId == schemeId && rat.IsActive == true)
                                  select new CostModel
                                  {
                                      depttID = rat.departmentId,
                                      depttName = deptt.departmentName,
                                      floor = rat.floorId,
                                      floorName = floor.floorName,
                                      blockId = rat.blockId,
                                      blockName = block.blockName,
                                      earnestMoney = rat.earnestMoney,
                                      sector = rat.sectorId,
                                      sectorName = sec.sectorName,
                                      landRate = rat.landRatePerSqmt,
                                      civilCost = rat.civilCost,
                                      allotmentMoney = rat.allotmentMoney,
                                      leaseRent = rat.leaseRent,
                                      propCost = rat.departmentId == 5 ? rat.propertyCost : rat.totalPropertyCost,
                                      totPropCost = rat.totalPropertyCost,
                                      propName = proptype.propertyTypeName,
                                      propType = rat.propertyTypeId,
                                      refId = rat.refId
                                  }
                    );
                return lstAllCost.ToDataSourceResult(sourceReq);
            }
        }

        /// <summary>
        /// Fetches Scheme details from DB based on Scheme ID.
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public Scheme GetSChemeByID(int schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var schemeDetails = (from s in dbContext.SchemeMsts
                                     join m in dbContext.SchemeTypeMsts on s.schemeTypeId equals m.schemeTypeId
                                     where s.schemeId == schemeId
                                     select new Scheme
                                     {
                                         schemeType = m.schemeTypeId.ToString(),
                                         schemeName = s.schemeName,
                                         startDate = s.startDate,
                                         endDate = s.endDate,
                                         scheId = s.schemeId,
                                         IsAllotted = (from allot in dbContext.AllotmentMasters where allot.schemeId == s.schemeId && allot.isActive == 1 select allot.isActive).FirstOrDefault(),
                                         completed = s.completed.Value,
                                         SchemeStatus = s.Status,
                                         strSchemeStatus = (from cc in dbContext.Common_Config where cc.Id == s.Status && cc.Is_Active == 1 select cc.Name).FirstOrDefault(),
                                         FormFee = s.FormFee == null ? 0.0M : s.FormFee.Value
                                     }).FirstOrDefault();
                var depttDetails = (from dept in dbContext.SchemeDepartmentTrans where dept.schemeId == schemeId && dept.IsActive == true select dept).FirstOrDefault();
                var costDetails = (from co in dbContext.SchemeCostTrans where co.schemeId == schemeId && co.IsActive == true select co).FirstOrDefault();
                //var rebateDetails = (from rd in dbContext.SchemeRebateTrans where rd.schemeId == schemeId && rd.IsActive == true select rd).FirstOrDefault();
                //var quotaDetails = (from qd in dbContext.SchemeQuotaTrans where qd.schemeId == schemeId && qd.IsActive == true select qd).FirstOrDefault();
                var bankDetails = (from bd in dbContext.SchemeBankTrans where bd.schemeId == schemeId && bd.IsActive == true select bd).FirstOrDefault();
                var landSchedule = (from ls in dbContext.Auto_Schedule where ls.Scheme_Id == schemeId && ls.Is_Active == true select ls).FirstOrDefault();

                if (depttDetails != null)
                    schemeDetails.isDepttExists = true;
                else schemeDetails.isDepttExists = false;

                if (costDetails != null)
                    schemeDetails.isCostExists = true;
                else schemeDetails.isCostExists = false;

                if (bankDetails != null)
                    schemeDetails.isBankExists = true;
                else schemeDetails.isBankExists = false;

                if (landSchedule != null)
                    schemeDetails.isLandExists = true;
                else schemeDetails.isLandExists = false;

                if (depttDetails != null && costDetails != null && bankDetails != null && landSchedule != null)
                {
                    if (schemeDetails.completed.Value)
                        schemeDetails.isSchemeActive = false;
                    else
                        schemeDetails.isSchemeActive = true;
                }
                else
                {
                    schemeDetails.isSchemeActive = false;
                }
                return schemeDetails;
            }
        }

        /// <summary>
        /// Deletes SchemeMaster related Department from DB
        /// </summary>
        /// <param name="refId"></param>
        /// <returns></returns>
        public bool RemoveDepttRecord(int refId, int schemeID)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var record = dbContext.SchemeDepartmentTrans.Where(d => d.refId == refId).FirstOrDefault();
                if (record != null)
                {
                    var dependentQuota = dbContext.SchemeQuotaTrans.Where(q => q.departmentId == record.departmentId && q.schemeId == record.schemeId).FirstOrDefault();
                    var dependentRebate = dbContext.SchemeRebateTrans.Where(r => r.departmentId == record.departmentId && r.schemeId == record.schemeId).FirstOrDefault();
                    var dependentCost = dbContext.SchemeCostTrans.Where(c => c.departmentId == record.departmentId && c.schemeId == record.schemeId).FirstOrDefault();

                    if (dependentQuota == null && dependentRebate == null && dependentCost == null)
                    {
                        dbContext.SchemeDepartmentTrans.Remove(record);
                        dbContext.SaveChanges();
                        flag = true;
                    }
                    else
                    {
                        flag = false; //Can't delete Department as a dependent Quota, Rebate or Cost exists for it.
                    }
                }
                var schemeDepartmentTranExit = dbContext.SchemeDepartmentTrans.Where(d => d.schemeId == schemeID).ToList();
                if (schemeDepartmentTranExit.Count == 0)
                {
                    var schemeComitedUpdate = dbContext.SchemeMsts.Where(d => d.schemeId == schemeID).FirstOrDefault();
                    if (schemeComitedUpdate != null)
                    {
                        schemeComitedUpdate.completed = false;
                        dbContext.SaveChanges();
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// Deletes SchemeMaster related Quota from DB
        /// </summary>
        /// <param name="refId"></param>
        /// <returns></returns>
        public bool RemoveQuotaRecord(int refId, int schemeID)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var record = dbContext.SchemeQuotaTrans.Where(d => d.refId == refId).FirstOrDefault();
                if (record != null)
                {
                    dbContext.SchemeQuotaTrans.Remove(record);
                    dbContext.SaveChanges();
                    flag = true;
                }
                var schemeDepartmentTranExit = dbContext.SchemeQuotaTrans.Where(d => d.schemeId == schemeID).ToList();
                if (schemeDepartmentTranExit.Count == 0)
                {
                    var schemeComitedUpdate = dbContext.SchemeMsts.Where(d => d.schemeId == schemeID).FirstOrDefault();
                    if (schemeComitedUpdate != null)
                    {
                        schemeComitedUpdate.completed = false;
                        dbContext.SaveChanges();
                    }
                }
            }

            return flag;
        }

        /// <summary>
        /// Deletes SchemeMaster related Rebate from DB
        /// </summary>
        /// <param name="refId"></param>
        /// <returns></returns>
        public bool RemoveRebRecord(int refId, int schemeID)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var record = dbContext.SchemeRebateTrans.Where(d => d.refId == refId).FirstOrDefault();
                if (record != null)
                {
                    dbContext.SchemeRebateTrans.Remove(record);
                    dbContext.SaveChanges();
                    flag = true;
                }
                var schemeDepartmentTranExit = dbContext.SchemeRebateTrans.Where(d => d.schemeId == schemeID).ToList();
                if (schemeDepartmentTranExit.Count == 0)
                {
                    var schemeComitedUpdate = dbContext.SchemeMsts.Where(d => d.schemeId == schemeID).FirstOrDefault();
                    if (schemeComitedUpdate != null)
                    {
                        schemeComitedUpdate.completed = false;
                        dbContext.SaveChanges();
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// Deletes SchemeMaster related Cost from DB
        /// </summary>
        /// <param name="refId"></param>
        /// <returns></returns>
        public bool RemoveCostRecord(int refId, int schemeID)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var record = dbContext.SchemeCostTrans.Where(d => d.refId == refId).FirstOrDefault();
                if (record != null)
                {
                    dbContext.SchemeCostTrans.Remove(record);
                    dbContext.SaveChanges();
                    flag = true;
                }
                var schemeDepartmentTranExit = dbContext.SchemeCostTrans.Where(d => d.schemeId == schemeID).ToList();
                if (schemeDepartmentTranExit.Count == 0)
                {
                    var schemeComitedUpdate = dbContext.SchemeMsts.Where(d => d.schemeId == schemeID).FirstOrDefault();
                    if (schemeComitedUpdate != null)
                    {
                        schemeComitedUpdate.completed = false;
                        dbContext.SaveChanges();
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// Deletes SchemeMaster related Bank from DB
        /// </summary>
        /// <param name="refId"></param>
        /// <returns></returns>
        public bool RemoveBankRecord(int refId, int schemeID)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var record = dbContext.SchemeBankTrans.Where(d => d.refId == refId).FirstOrDefault();
                if (record != null)
                {
                    dbContext.SchemeBankTrans.Remove(record);
                    dbContext.SaveChanges();
                    flag = true;
                    var schemeDepartmentTranExit = dbContext.SchemeBankTrans.Where(d => d.schemeId == schemeID).ToList();
                    if (schemeDepartmentTranExit.Count == 0)
                    {
                        var schemeComitedUpdate = dbContext.SchemeMsts.Where(d => d.schemeId == schemeID).FirstOrDefault();
                        if (schemeComitedUpdate != null)
                        {
                            schemeComitedUpdate.completed = false;
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// ActivateScheme 
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public bool ActivateScheme(int schemeId)
        {
            bool flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var objresult = dbContext.SchemeMsts.FirstOrDefault(id => id.schemeId == schemeId);
                if (objresult != null)
                {
                    objresult.completed = true;
                    objresult.Status = Constants.SchemeOpen;
                    dbContext.SaveChanges();
                    flag = true;
                }
                return flag;
            }
        }
        /// <summary>
        /// To get account number for particular bank and branch.
        /// </summary>
        /// <param name="bankId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public List<DDLStringList> GetAccountNumber(int bankId, int branchId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var accountnumbers = (from s in dbContext.BranchMsts
                                      where s.bankId == bankId && s.branchId == branchId
                                      select new DDLStringList
                                      {
                                          id = s.accountNumber,
                                          text = s.accountNumber
                                      }).ToList();
                return accountnumbers;
            }
        }
        /// <summary>
        /// To check scheme duplicacy.
        /// </summary>
        /// <param name="schemeName"></param>
        /// <param name="schemeid"></param>
        /// <returns></returns>
        public bool IsSchemeNameUnique(string schemeName, int schemeid)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var varscheme = dbContext.SchemeMsts.FirstOrDefault(r => r.schemeName.ToLower() == schemeName.ToLower() && r.IsActive == true);
                if (varscheme != null)
                {
                    //var NameExit = (from sch in dbContext.SchemeMsts
                    //                where (sch.schemeName.ToLower().Trim() == schemeName.ToLower().Trim()
                    //                && sch.IsActive == true)
                    //                select sch.schemeId).FirstOrDefault();
                    //if (NameExit == 0)
                    //{
                    //    flag = false;
                    //}
                    //else { flag = true; }
                    flag = true;
                }
            }
            return flag;
        }
        /// <summary>
        /// To check Department, Quota, Rebate, Cost, Bank duplicacy.
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="schemeid"></param>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public bool IsDepatmnetNameUnique(int deptId, int schemeid, int type, int flags, int floorddl, int block, int sector)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (type == 1)
                {
                    var varscheme = dbContext.SchemeDepartmentTrans.FirstOrDefault(r => r.departmentId == deptId && r.schemeId == schemeid && r.IsActive == true);
                    if (varscheme == null)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                if (type == 2)
                {
                    var varscheme = dbContext.SchemeQuotaTrans.FirstOrDefault(r => r.departmentId == deptId && r.schemeId == schemeid && r.quotaId == flags && r.IsActive == true);
                    if (varscheme == null)
                    {
                        flag = false;

                    }
                    else
                    {
                        flag = true;
                    }
                }
                if (type == 3)
                {
                    var varscheme = dbContext.SchemeRebateTrans.FirstOrDefault(r => r.departmentId == deptId && r.schemeId == schemeid && r.rebateId == flags && r.IsActive == true);
                    if (varscheme == null)
                    {
                        flag = false;

                    }
                    else
                    {
                        flag = true;
                    }
                }
                if (type == 4)
                {
                    var varscheme = dbContext.SchemeCostTrans.FirstOrDefault(r => r.departmentId == deptId && r.schemeId == schemeid && r.propertyTypeId == flags && r.blockId.Value == block && r.floorId.Value == floorddl && r.sectorId == sector && r.IsActive == true);
                    if (varscheme == null)
                    {
                        flag = false;

                    }
                    else
                    {
                        flag = true;
                    }
                }
                if (type == 5)
                {
                    var varscheme = dbContext.SchemeBankTrans.FirstOrDefault(r => r.bankId == deptId && r.schemeId == schemeid && r.branchId == flags && r.IsActive == true);
                    if (varscheme == null)
                    {
                        flag = false;

                    }
                    else
                    {
                        flag = true;
                    }
                }

            }
            return flag;
        }

        /// <summary>
        /// To Soft delete the record by scheme id.
        /// </summary>
        /// <param name="schemeid"></param>
        /// <returns></returns>
        public bool DeleteRecordById(int schemeid)
        {
            bool flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var objresult = dbContext.SchemeMsts.FirstOrDefault(id => id.schemeId == schemeid);
                if (objresult != null)
                {
                    objresult.IsActive = false;
                    dbContext.SaveChanges();
                    flag = true;
                }
                return flag;
            }
        }

        public bool IsBankNameUnique(int bankId, string bankName)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var varBank = dbContext.BankMsts.FirstOrDefault(r => r.bankId == bankId && r.bankName == bankName && r.IsActive == true);
                if (varBank == null)
                {
                    var NameExit = (from sch in dbContext.BankMsts
                                    where (sch.bankName.ToLower().Trim() == bankName.ToLower().Trim()
                                    && sch.IsActive == true)
                                    select sch.bankId).FirstOrDefault();
                    if (NameExit == 0)
                    {
                        flag = false;
                    }
                    else { flag = true; }
                }
            }
            return flag;
        }

        public bool IsAccountNumberDuplicate(int bankId, string accountNumber)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var varBank = dbContext.BranchMsts.FirstOrDefault(r => r.accountNumber == accountNumber && r.accountNumber != "" && r.IsActive == true);
                if (varBank != null)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }

            }
            return flag;
        }

        //public bool IsbranchNameDuplicate(int bankid, string branchName)
        //{
        //    var flag = false;
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var varBank = dbContext.BranchMsts.FirstOrDefault(r => r.bankId == bankid && r.branchName == branchName && r.IsActive == true);
        //        if (varBank == null)
        //        {
        //            var NameExit = (from sch in dbContext.BranchMsts
        //                            where (sch.branchName.ToLower().Trim() == branchName.ToLower().Trim()
        //                            && sch.IsActive == true)
        //                            select sch.branchId).FirstOrDefault();
        //            if (NameExit == 0)
        //            {
        //                flag = false;
        //            }
        //            else { flag = true; }
        //        }
        //    }
        //    return flag;
        //}

        public bool UpdateScheme(int schemeId, DateTime startDate, DateTime endDate, int userID, decimal formFee)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                bool flag = false;
                //var IsPropertyAlloted = dbcontext.SchemePropTrans.FirstOrDefault(id => id.schemeId == schemeId);
                //if (IsPropertyAlloted != null)
                //{
                //    flag = false;
                //    return flag;
                //}
                //else
                //{
                var objresult = dbcontext.SchemeMsts.FirstOrDefault(id => id.schemeId == schemeId);
                if (objresult != null)
                {
                    objresult.startDate = startDate;
                    objresult.endDate = endDate;
                    objresult.modifiedBy = userID.ToString();
                    objresult.modifiedDate = DateTime.Now;
                    objresult.FormFee = formFee;
                    dbcontext.SaveChanges();
                    flag = true;
                }
                return flag;
                //}
            }

            //return dataResult.schemeId > 0 ? dataResult.schemeId : 0;
        }
        public bool IsCostNameUnique(int deptId, int schemeid, int flag, int typesector, int floorddl, int block)
        {
            bool flags = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var varscheme = dbContext.SchemeCostTrans.FirstOrDefault(r => r.departmentId == deptId && r.schemeId == schemeid && r.propertyTypeId == flag && r.IsActive == true && r.sectorId == typesector && r.floorId == floorddl && r.blockId == block);
                if (varscheme == null)
                {
                    flags = false;

                }
                else
                {
                    flags = true;
                }
                return flags;
            }
        }

        #region Auto Schedule Land Development
        /// <summary>
        /// Get Data for Auto Schedule Land Development
        /// </summary>
        /// <param name="request"></param>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public DataSourceResult GetLandDevelopmentScheduleData(DataSourceRequest sourceReq, int schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstAllDeptt = new List<LandDevelopmentScheduleModel>();

                var lstAllQuota = (from rat in dbContext.Auto_Schedule
                                   join deptt in dbContext.DepartmentMsts on rat.Department_Id equals deptt.departmentId
                                   join processData in dbContext.Process_Master on rat.Process_Id equals processData.Process_Id
                                   join scheme in dbContext.SchemeMsts on rat.Scheme_Id equals scheme.schemeId
                                   //join quota in dbContext.QuotaMsts on rat.quotaId equals quota.quotaId
                                   where (rat.Scheme_Id == schemeId && rat.Is_Active == true)
                                   select new LandDevelopmentScheduleModel
                                   {
                                       ScheduleId = rat.Schedule_Id,
                                       LandDevelopmentDepartmentId = rat.Department_Id,
                                       LandDevelopmentDepartmentName = deptt.departmentName,
                                       Duration = rat.Duration_In_Days,
                                       TriggerId = rat.Trigger_Process_Id,
                                       TriggerName = dbContext.Process_Master.Where(x => x.Process_Id == rat.Trigger_Process_Id).Select(y => y.Process_Name).FirstOrDefault(),
                                       ProcessName = processData.Process_Name,
                                       SchemeName = scheme.schemeName
                                   }
                    );
                return lstAllQuota.ToDataSourceResult(sourceReq);
            }
        }
        /// <summary>
        /// Adding Auto Schedule for Land Development
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="processId"></param>
        /// <param name="duration"></param>
        /// <param name="triggerId"></param>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public int AddLandDevelopmentSchedule(int departmentId, int processId, int duration, int triggerId, int schemeId, int userID)
        {
            var landDevelopment = new Auto_Schedule();
            var result = 0;
            using (var dbcontext = new NoidaPMSEntities())
            {
                var LandDevelopment = dbcontext.Auto_Schedule.Where(r => r.Department_Id == departmentId && r.Scheme_Id == schemeId && r.Trigger_Process_Id == triggerId && r.Process_Id == processId && r.Is_Active == true).FirstOrDefault();
                if (LandDevelopment == null)
                {
                    landDevelopment.Scheme_Id = schemeId;
                    landDevelopment.Department_Id = departmentId;
                    landDevelopment.Process_Id = processId;
                    landDevelopment.Trigger_Process_Id = triggerId;
                    landDevelopment.Duration_In_Days = duration;
                    landDevelopment.Created_Date = DateTime.Now;
                    landDevelopment.Created_By = userID;
                    landDevelopment.Is_Active = true;
                    dbcontext.Auto_Schedule.Add(landDevelopment);
                    dbcontext.SaveChanges();
                    result = 1;
                }
                else
                {
                    result = 2;
                }
            }
            return result;
        }
        /// <summary>
        /// Remove Land Development Record
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <returns></returns>
        public bool RemoveLandDevelopment(int scheduleId, int schemeID)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var record = dbContext.Auto_Schedule.Where(d => d.Schedule_Id == scheduleId).FirstOrDefault();
                if (record != null)
                {
                    dbContext.Auto_Schedule.Remove(record);
                    dbContext.SaveChanges();
                    flag = true;
                    var schemeDepartmentTranExit = dbContext.Auto_Schedule.Where(d => d.Scheme_Id == schemeID).ToList();
                    if (schemeDepartmentTranExit.Count == 0)
                    {
                        var schemeComitedUpdate = dbContext.SchemeMsts.Where(d => d.schemeId == schemeID).FirstOrDefault();
                        if (schemeComitedUpdate != null)
                        {
                            schemeComitedUpdate.completed = false;
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            return flag;
        }
        /// <summary>
        /// Validating for duplicate entery on the basis of Scheme, Department , Process Name and Trigger Process
        /// </summary>
        /// <param name="departmentID"></param>
        /// <param name="processId"></param>
        /// <param name="schemeID"></param>
        /// <param name="triggerId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsLandDevelopmentUnique(int departmentID, int processId, int schemeID, int triggerId, int type)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var varLandDevelopment = new Auto_Schedule();
                if (type == 3)
                {
                    varLandDevelopment = dbContext.Auto_Schedule.Where(r => r.Department_Id == departmentID && r.Scheme_Id == schemeID && r.Trigger_Process_Id == triggerId && r.Process_Id == processId && r.Is_Active == true).FirstOrDefault();
                    if (varLandDevelopment == null)
                    {
                        flag = false;

                    }
                    else
                    {
                        flag = true;
                    }
                }

            }
            return flag;
        }
        #endregion

        //Update Scheme Status
        public bool GetStatusUpdated(int schemeID)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                bool flag = false;
                var objresult = dbcontext.SchemeMsts.FirstOrDefault(id => id.schemeId == schemeID);
                if (objresult != null)
                {
                    objresult.Status = Constants.SchemeClosed;
                    dbcontext.SaveChanges();
                    flag = true;
                    if (flag == true)
                    {
                        // Unsuccessfull Applicants will be those Aplications which are rejected or Not sending for approval
                        var unsuccessfullAplicantsSchemeDept = (from aapDetail in dbcontext.ApplicationDetails
                                                                where aapDetail.schemeId == schemeID && (aapDetail.registrationId == null || aapDetail.isAllotted == null || aapDetail.isAllotted == "0")
                                                                select aapDetail).ToList();
                        foreach (var unsucessfulAplicant in unsuccessfullAplicantsSchemeDept)
                        {
                            var unsuccessfulApplicant = new UnsuccessfulApplicantListMaster();
                            unsuccessfulApplicant.applicationId = unsucessfulAplicant.applicationId;
                            unsuccessfulApplicant.schemeId = unsucessfulAplicant.schemeId;
                            unsuccessfulApplicant.departmentId = unsucessfulAplicant.departmentId;
                            //unsuccessfulApplicant.status = Common.AllotmentStatus.NotSubmitted.ToString();
                            unsuccessfulApplicant.isActive = true;
                            unsuccessfulApplicant.createdDate = DateTime.Now;
                            unsuccessfulApplicant.submitDate = DateTime.Now;
                            unsuccessfulApplicant.createdBy = userInfo.UserID.ToString();
                            dbcontext.UnsuccessfulApplicantListMasters.Add(unsuccessfulApplicant);
                            dbcontext.SaveChanges();
                        }
                    }
                }
                return flag;
            }
        }


        public SchemeViewModel GetSchemeDetailById(SchemeViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var SchemeInfo = (from s in dbContext.SchemeMsts
                                  join m in dbContext.SchemeTypeMsts on s.schemeTypeId equals m.schemeTypeId
                                  where s.schemeId == model.SchemeId
                                  select new SchemeViewModel
                                  {
                                      SchemeType = m.schemeTypeId.ToString(),
                                      SchemeName = s.schemeName,
                                      SchemeStartDate = s.startDate,
                                      SchemeEndDate = s.endDate,
                                      SchemeId = s.schemeId,
                                      //IsAllotted = (from allot in dbContext.AllotmentMasters where allot.schemeId == s.schemeId && allot.isActive == 1 select allot.isActive).FirstOrDefault(),
                                      IsCompleted = s.completed,
                                      StatusId = s.Status,
                                      SchemeStatus = (from cc in dbContext.Common_Config where cc.Id == s.Status && cc.Is_Active == 1 select cc.Name).FirstOrDefault(),
                                      ApplicationFormFee = s.FormFee == null ? 0.0M : s.FormFee.Value
                                  }).FirstOrDefault();
                var Department = dbContext.SchemeDepartmentTrans.FirstOrDefault(d => d.schemeId == model.SchemeId && d.IsActive == true);
                var PropertyCost = dbContext.SchemeCostTrans.FirstOrDefault(d => d.schemeId == model.SchemeId && d.IsActive == true);
                var BankList = dbContext.SchemeBankTrans.FirstOrDefault(d => d.schemeId == model.SchemeId && d.IsActive == true);
                var Schedule = dbContext.Auto_Schedule.FirstOrDefault(d => d.Scheme_Id == model.SchemeId && d.Is_Active == true);

                SchemeInfo.IsDepartmentExist = Department != null ? true : false;
                SchemeInfo.IsPropertyCostExist = PropertyCost != null ? true : false;
                SchemeInfo.IsBankExist = BankList != null ? true : false;
                SchemeInfo.IsScheduleExist = Schedule != null ? true : false;
                SchemeInfo.IsSchemeActive = (Department != null && PropertyCost != null && BankList != null && Schedule != null && SchemeInfo.IsCompleted != null && SchemeInfo.IsCompleted == true) ? true : false;
                return SchemeInfo;
            }
        }
    }
}//end of class and Namespace.
