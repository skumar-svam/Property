using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Model.CommonModel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using NA.PMS.Web.Models;
using System.Web;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;

namespace NA.PMS.Repository
{
    public class BusinessRuleRepository : IBusinessRuleRepository
    {
        // Getting UserID from Session

        private CurrentUserDetail userInfo = new CurrentUserDetail();

        public BusinessRuleRepository()
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

        // Method for updating the model object values used in calculation allotment money
        public AllotmentModel UpdateAllotmentModel(AllotmentModel allotmentModel)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var objschmeCostTrans = dbContext.SchemeCostTrans.FirstOrDefault(id => id.schemeId == allotmentModel.SchemeId && id.departmentId == allotmentModel.DepartmentId);
                var objschmedepttTrans = dbContext.SchemeDepartmentTrans.FirstOrDefault(id => id.schemeId == allotmentModel.SchemeId && id.departmentId == allotmentModel.DepartmentId);
                if (objschmeCostTrans != null)
                {
                    allotmentModel.LandRatePerSqMet = objschmeCostTrans.landRatePerSqmt;
                    allotmentModel.EarnestMoney = objschmeCostTrans.earnestMoney;

                }
                if (objschmedepttTrans != null)
                    allotmentModel.AllotmentMoneyPercent = objschmedepttTrans.allotmentMoneyPercent;

            }
            return allotmentModel;
        }


        // To check whether a property us allotted or not
        public bool IsPropertyAllotted(int rId, string status)
        {
            bool flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.AllotmentMasters.FirstOrDefault(x => x.isStatus == status && x.rid == rId);
                if (result != null)
                    flag = true;
            }
            return flag;
        }

        // To check whether Allotment is paid or not
        public bool IsAllotmentMoneyPaid(int rId)
        {
            bool flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.AllotmentMasters.FirstOrDefault(x => x.rid == rId);
                if (result != null)
                    flag = true;
            }
            return flag;
        }

        // To get the details of the property that will be used in calculations
        public AllotmentModel GetPropertyAllotmentDetails(int rid)
        {
            //decimal? totalPropertyCost = 0;
            var allotmentModel = new AllotmentModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                allotmentModel = (from tblAllotMaster in dbContext.AllotmentMasters
                                  join tblAppDetails in dbContext.ApplicationDetails
                                        on tblAllotMaster.applicationId equals tblAppDetails.applicationId
                                  join tblSchemePropTran in dbContext.SchemePropTrans
                                        on tblAllotMaster.propertyId equals tblSchemePropTran.propertyId
                                  //  join viewAllotMoneyChk in dbContext.ViewAllotmentMoneyChecks on tblAllotMaster.rid.ToString() equals viewAllotMoneyChk.RID_NO -> Done by Vishal Shukla on 15th Feb 2017 to remove validations and easily enter file data
                                  where tblAllotMaster.rid == rid
                                  select new AllotmentModel
                                  {
                                      RID = rid,
                                      Gender = tblAppDetails.gender,
                                      AllotmentDate = tblAllotMaster.allotmentDate,
                                      AllotmentMoneyDueDate = tblAllotMaster.allotmentDate,//Convert.ToDateTime(tblAllotMaster.allotmentDate).Date.AddDays(30),
                                      AllotmentMoney = tblSchemePropTran.allotmentMoney,
                                      TotalPropertyCost = tblSchemePropTran.totalPropertyCost,
                                      AmountPaid = 0,//viewAllotMoneyChk.AMOUNT_PAID, -> Done by Vishal Shukla on 15th Feb 2017 to remove validations and easily enter file data
                                      DepositDate = null,// viewAllotMoneyChk.DEPOSIT_DATE,-> Done by Vishal Shukla on 15th Feb 2017 to remove validations and easily enter file data
                                      DepartmentId = tblAllotMaster.departmentId
                                  }
                    ).FirstOrDefault();
            }

            return allotmentModel;
        }

        // To calculate Stamp Duty Amount and Duplicate Stamp value
        public LeaseDeedProperty GetLeaseDeedDetails(int rid, DateTime leaseDeedDate)
        {
            var leaseDeedProperty = new LeaseDeedProperty();
            using (var dbContext = new NoidaPMSEntities())
            {
                leaseDeedProperty = (from tblAllotMaster in dbContext.AllotmentMasters
                                     join tblAppDetails in dbContext.ApplicationDetails on tblAllotMaster.applicationId equals
                                         tblAppDetails.applicationId
                                     join tblStampDutyMaster in dbContext.StampDutyMasters on tblAppDetails.gender equals
                                         tblStampDutyMaster.Type
                                     where
                                         tblStampDutyMaster.EffectedFrom < leaseDeedDate && tblStampDutyMaster.EffectedTo > leaseDeedDate
                                         && tblAllotMaster.rid == rid
                                     select new LeaseDeedProperty
                                     {
                                         StampDutyPercent = tblStampDutyMaster.StampDutyPercent,
                                         DuplicateStampValue = tblStampDutyMaster.DuplicateStampDutyValINR
                                     }
                    ).FirstOrDefault();
            }

            return leaseDeedProperty;
        }

        // Get Balance Due Till Date
        public decimal GetBalanceDueTillDate(int rid, int departmentId)
        {
            var dt = DateTime.Now.Date;
            decimal balance = 0;
            var param1 = new SqlParameter
            {
                ParameterName = "RID",
                Value = rid
            };
            var param2 = new SqlParameter
            {
                ParameterName = "c",
                Value = dt
            };
            var param3 = new SqlParameter
            {
                ParameterName = "landusetype",
                Value = departmentId
            };

            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.Database.SqlQuery<decimal>("exec Sp_DuesCalculationTillDate @RID, @c, @landusetype", param1, param2, param3).FirstOrDefault();
                balance = result;
            }
            return balance;
        }

        // Get Penel Interest
        public decimal GetPenalInterest(int deptId)
        {
            decimal balance = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.InterestMsts.FirstOrDefault(x => x.departmentId == deptId);
                if (result != null)
                    balance = Convert.ToDecimal(result.penalInterest);
            }
            return balance;
        }

        // Function to Get LeaseRent Percent for Housing only
        public decimal GetLeaseRentPercentForHousing(int rId, int departmentId)
        {
            decimal leaseRentPercent = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from tblAllotMaster in dbContext.AllotmentMasters
                              join tblScmPropTran in dbContext.SchemePropTrans on tblAllotMaster.propertyId equals tblScmPropTran.propertyId
                              join tblScmCostTran in dbContext.SchemeCostTrans on tblScmPropTran.schemeId equals tblScmCostTran.schemeId
                              where tblAllotMaster.isActive == 1 && tblAllotMaster.isStatus == AllotmentStatus.Approved.ToString()
                                    && tblAllotMaster.departmentId == Constants.DepartmentIdForHousing && tblScmPropTran.departmentId == tblScmCostTran.departmentId && tblScmPropTran.sectorId == tblScmCostTran.sectorId && tblScmPropTran.blockId == tblScmCostTran.blockId && tblScmPropTran.propertyTypeId == tblScmCostTran.propertyTypeId && tblScmPropTran.departmentId == tblScmCostTran.floorId &&
                                    tblAllotMaster.departmentId == departmentId
                              select new LeaseDeedProperty
                              {
                                  LeaseRentPercentForHousing = tblScmCostTran.leaseRent
                              }).FirstOrDefault();
                if (result != null)
                    leaseRentPercent = result.LeaseRentPercentForHousing.Value;
            }
            return leaseRentPercent;
        }

        // Function to Get LeaseRent Percent
        public double GetLeaseRentPercent(int rId, int departmentId)
        {
            double leaseRentPercent = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from tblAllotMaster in dbContext.AllotmentMasters
                              join tblScmDeptTran in dbContext.SchemeDepartmentTrans
                                  on tblAllotMaster.schemeId equals tblScmDeptTran.schemeId
                              where tblAllotMaster.isActive == 1 && tblAllotMaster.isStatus == AllotmentStatus.Approved.ToString()
                                    && tblAllotMaster.departmentId != Constants.DepartmentIdForHousing &&
                                    tblAllotMaster.departmentId == departmentId
                              select new LeaseDeedProperty
                              {
                                  LeaseRentPercent = tblScmDeptTran.leaseRentPercent
                              }).FirstOrDefault();
                if (result != null)
                    leaseRentPercent = result.LeaseRentPercent.Value;
            }
            return leaseRentPercent;
        }

        // To check that CheckList generated for Lease deed or not
        public bool IsCheckListGenerated(int rId)
        {
            int chkListTypeId = Convert.ToInt32(CheckListType.LeaseDeed);
            bool flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.ChecklistTrans.FirstOrDefault(x => x.Rid == rId && x.ChecklistTypeId == chkListTypeId);
                if (result != null)
                    flag = true;
            }
            return flag;
        }

        // To Get LeaseDeed date
        public DateTime GetLeaseDeedDueDate(int rId)
        {
            DateTime dtLeaseDeedDue = DateTime.Now;
            int chkListTypeId = Convert.ToInt32(CheckListType.LeaseDeed);
            using (var dbContext = new NoidaPMSEntities())
            {
                var leaseDeedProcessId = (from proMas in dbContext.Process_Master where proMas.Process_Name.ToLower() == ProcessesMaster.LeaseDeed.ToLower() select proMas.Process_Id).FirstOrDefault();
                var leaseDeedScheduleDays = (from auto in dbContext.Auto_Schedule
                                             join pro in dbContext.Process_Master on auto.Process_Id equals pro.Process_Id
                                             join allMas in dbContext.AllotmentMasters on auto.Scheme_Id equals allMas.schemeId
                                             join spt in dbContext.SchemePropTrans on allMas.departmentId equals spt.departmentId
                                             where allMas.rid == rId && auto.Is_Active == true && auto.Process_Id == leaseDeedProcessId
                                             select auto.Duration_In_Days).FirstOrDefault();
                var result = dbContext.ChecklistTrans.FirstOrDefault(x => x.Rid == rId && x.ChecklistTypeId == chkListTypeId);
                if (result != null && leaseDeedScheduleDays != 0)
                    dtLeaseDeedDue = result.ChecklistDate.Value.AddDays(Convert.ToDouble(leaseDeedScheduleDays));
            }
            return dtLeaseDeedDue;
        }

        public DateTime GetChecklistDueDate(int rId)
        {
            DateTime dtChkLstDue = DateTime.Now;
            using (var dbContext = new NoidaPMSEntities())
            {
                var chckLstProcessId = (from proMas in dbContext.Process_Master where proMas.Process_Name.ToLower() == ProcessesMaster.Checklist.ToLower() select proMas.Process_Id).FirstOrDefault();
                var chkLstScheduleDays = (from auto in dbContext.Auto_Schedule
                                          join pro in dbContext.Process_Master on auto.Process_Id equals pro.Process_Id
                                          join allMas in dbContext.AllotmentMasters on auto.Scheme_Id equals allMas.schemeId
                                          join spt in dbContext.SchemePropTrans on allMas.departmentId equals spt.departmentId
                                          where allMas.rid == rId && auto.Is_Active == true && auto.Process_Id == chckLstProcessId
                                          select auto.Duration_In_Days).FirstOrDefault();
                var result = dbContext.AllotmentMasters.FirstOrDefault(x => x.rid == rId);
                if (result != null && chkLstScheduleDays != 0)
                    dtChkLstDue = result.allotmentDate.Value.AddDays(Convert.ToDouble(chkLstScheduleDays));
            }
            return dtChkLstDue;
        }

        // Get Lease Rent Dues Till date
        public decimal GetLeaseRentDuesTillDate(int rid, int departmentId)
        {
            var dt = DateTime.Now.Date;
            decimal balance = 0;
            var param1 = new SqlParameter
            {
                ParameterName = "RID",
                Value = rid
            };
            var param2 = new SqlParameter
            {
                ParameterName = "c",
                Value = dt
            };
            var param3 = new SqlParameter
            {
                ParameterName = "landusetype",
                Value = departmentId
            };

            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.Database.SqlQuery<decimal>("exec Sp_LeaseRentDuesCalculationTillDate @RID, @c, @landusetype", param1, param2, param3).FirstOrDefault();
                balance = result;
            }
            return balance;
        }

        // Get Possession Due Date
        public int GetPossessionDurationInDays(int rId, int departmentId)
        {
            int processIdPossession = Convert.ToInt32(ProcessType.Possession);
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from allotMaster in dbContext.AllotmentMasters
                              join autoSchedule in dbContext.Auto_Schedule on allotMaster.schemeId equals autoSchedule.Scheme_Id
                              join processMst in dbContext.Process_Master on autoSchedule.Process_Id equals processMst.Process_Id
                              where allotMaster.rid == rId && allotMaster.departmentId == departmentId && autoSchedule.Process_Id == processIdPossession
                              select new AutoSchedule
                              {
                                  DurationInDays = autoSchedule.Duration_In_Days
                              }).FirstOrDefault();
                var durationInDays = 0;
                if (result != null)
                    durationInDays = result.DurationInDays.Value;

                return durationInDays;
            }
        }

        // Get LandRatePerSQMT from DB
        public decimal GetAllotmentRateByPropertyId(int propertyId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from schProp in dbContext.SchemePropTrans where schProp.propertyId == propertyId select schProp.landRatePerSqmt).FirstOrDefault();//dbContext.SchemePropTrans.Where(x => x.propertyId == propertyId).Select(x => new {  x.landRatePerSqmt}).FirstOrDefault();}
                decimal landRate = 0;
                if (result != null)
                    landRate = result.Value;
                return landRate;
            }
        }

        /// <summary>
        /// Used for getting Lease Rent Challan related values -> Lease Rent, Dues and Total Dues
        /// </summary>
        /// <param name="rId">RId</param>
        /// <param name="type">Type</param>
        /// <returns>ChallanModel</returns>
        public ChallanModel GetLeaseRentValue(int rId, int deptId, int type)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var challan = new ChallanModel();
                var housingDepttId = (from deptMas in dbContext.DepartmentMsts where deptMas.departmentName.ToLower() == Common.Departmentenum.Housing.ToString().ToLower() select deptMas.departmentId).FirstOrDefault();
                if (deptId == housingDepttId)
                {
                    challan = (from allotMas in dbContext.AllotmentMasters
                               join spt in dbContext.SchemePropTrans on allotMas.propertyId equals spt.propertyId
                               join sct in dbContext.SchemeCostTrans on allotMas.schemeId equals sct.schemeId
                               join regDet in dbContext.RegistryDetails on allotMas.rid equals regDet.Rid
                               where spt.departmentId == deptId && allotMas.rid == rId
                               && spt.departmentId == sct.departmentId && spt.propertyTypeId == sct.propertyTypeId && spt.floorId == sct.floorId && spt.sectorId == sct.sectorId && spt.blockId == sct.blockId
                               && spt.IsActive == true && sct.IsActive == true && regDet.IsActive == true
                               select new ChallanModel
                               {
                                   LeaseRentAmount = sct.leaseRent
                                   //du
                               }).FirstOrDefault();
                }
                else
                {
                    //Dummy variable used so that claculations which are not allowed in LINQ can be done later.
                    var tempchallan = (from allotMas in dbContext.AllotmentMasters
                                       join spt in dbContext.SchemePropTrans on allotMas.propertyId equals spt.propertyId
                                       //join sct in dbContext.SchemeCostTrans on allotMas.schemeId equals sct.schemeId
                                       join scDeptTrans in dbContext.SchemeDepartmentTrans on allotMas.schemeId equals scDeptTrans.schemeId
                                       join regDet in dbContext.RegistryDetails on allotMas.rid equals regDet.Rid
                                       where spt.departmentId == deptId && allotMas.rid == rId
                                       && allotMas.departmentId == scDeptTrans.departmentId
                                       && spt.IsActive == true && scDeptTrans.IsActive == true && regDet.IsActive == true
                                       select new
                                       {
                                           leaseRentPercent = scDeptTrans.leaseRentPercent,
                                           totPropCost = spt.totalPropertyCost
                                           //LeaseRentAmount = (scDeptTrans.leaseRentPercent * spt.totalPropertyCost)/100
                                           //du
                                       }).FirstOrDefault();
                    if (tempchallan != null)
                        challan.LeaseRentAmount = ((decimal)tempchallan.leaseRentPercent * tempchallan.totPropCost) / 100;//Convert Double to Decimal for multiplication
                }
                challan.TotalDue = GetLeaseRentDuesTillDate(rId, deptId);
                //In case of One time Lease Rent
                if (type == 1)
                {
                    challan.LeaseRentAmount = challan.LeaseRentAmount + (challan.LeaseRentAmount * 11);
                    challan.Dues = challan.TotalDue;
                    challan.TotalDue = challan.LeaseRentAmount + challan.TotalDue;
                }
                else
                {
                    //challan.Dues = challan.TotalDue;
                    challan.Dues = challan.TotalDue - challan.LeaseRentAmount;
                    //challan.Dues = challan.TotalDue - (challan.LeaseRentAmount = ((decimal)tempchallan.leaseRentPercent * tempchallan.totPropCost) / 100);
                }
                return challan;
            }
        }

        //validate renting charge at time of approval
        public bool IsRentingChargePaid(int rid)
        {
            var flag = false;  
            //using (var dbContext = new NoidaPMSEntities())
            //{
            //    var rent = dbContext.RentPermissionDetails.Where(m => m.Rid == rid && m.IsActive==true).Select(m => m.RentingCharge).FirstOrDefault();
            //    var rentingCharge = (from rdt in dbContext.RECEIPT_DETAIL_MASTER
            //                         join rmt in dbContext.RECEIPT_AMOUNT_TRANS on rdt.RECEIPT_ID equals rmt.RECEIPT_ID
            //                         where rdt.RID_NO == rid.ToString() && (rmt.RECEIPT_HEAD_ID == PaymentType.Rent && rmt.RECEIPT_SUBHEAD_ID == PaymentType.RentingCharge)
            //                         select rmt.AMOUNT_PAID).FirstOrDefault();
            //    if (rentingCharge == null)
            //    {
            //        rentingCharge = 0;
            //    }
            //    if (rentingCharge >= rent)
            //    {
            //        return flag = true;
            //    }
            //    else
            //    {
            //        return flag;
            //    }
            //}
            using (var dbContext = new NoidaPMSEntities())
            {
                var renting = dbContext.RentPermissionDetails.Where(m => m.Rid == rid && m.IsActive == true).FirstOrDefault();
                var paymentList = (from rdt in dbContext.RECEIPT_DETAIL_MASTER
                                     join rmt in dbContext.RECEIPT_AMOUNT_TRANS on rdt.RECEIPT_ID equals rmt.RECEIPT_ID
                                     where rdt.RID_NO == rid.ToString() && (rmt.RECEIPT_HEAD_ID == PaymentType.Rent && rmt.RECEIPT_SUBHEAD_ID == PaymentType.RentingCharge) && (DbFunctions.TruncateTime(rdt.ENTRY_DATE) >= DbFunctions.TruncateTime(renting.RentingDate))
                                     select rmt.AMOUNT_PAID).ToList();
                decimal? totalPaid = 0;
                if (paymentList != null)
                {
                    foreach (var amt in paymentList)
                    {
                        totalPaid = totalPaid + amt;
                    }
                    if (totalPaid >= renting.RentingCharge)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = false;
                }                
            }
            return flag;
        }

        public int CalculatePreviousDues(int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var param = new SqlParameter
                {
                    ParameterName = "Rid",
                    Value = rId
                };
                //var previousDues = dbContext.AllotmentMoneyDue(rId);
                var previousDues = dbContext.Database.SqlQuery<int>("exec AllotmentMoneyDue @Rid", param).FirstOrDefault();
                //var prev = Convert.ToDecimal(previousDues);
                return previousDues;
                //decimal previousDues = 0.1M;
                //return previousDues;
            }
        }


        public DataSourceResult GetPropertyPaymentDetails_Read(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var paymentList = (from payment in dbContext.RECEIPT_DETAIL_MASTER
                                   join alotment in dbContext.AllotmentMasters on payment.RID_NO equals alotment.rid.ToString()
                                   join schemeprop in dbContext.SchemePropTrans on alotment.propertyId equals schemeprop.propertyId
                                   select new PaymentViewModel
                                   {
                                       ReceiptId = payment.RECEIPT_ID,
                                       RegistrationNo = payment.RID_NO,
                                       DepartmentName = alotment.DepartmentMst.departmentName,
                                       FirstName = alotment.ApplicationDetail.firstName,
                                       MiddleName = alotment.ApplicationDetail.middleName,
                                       LastName = alotment.ApplicationDetail.lastName,
                                       SectorName = schemeprop.SectorMst.sectorName,
                                       BlockName = schemeprop.BlockMst.blockName,
                                       PlotNo = schemeprop.propertyNo,
                                       //PropertyNo = schemeprop.SectorMst.sectorName + "/" + schemeprop.BlockMst.blockName + "-" + schemeprop.propertyNo,
                                       Amount = payment.AMOUNT,
                                       DepositDate = payment.DEPOSIT_DATE,
                                       EntryDate = payment.ENTRY_DATE
                                   });
                return paymentList.ToDataSourceResult(request);
            }
        }
    }
}
