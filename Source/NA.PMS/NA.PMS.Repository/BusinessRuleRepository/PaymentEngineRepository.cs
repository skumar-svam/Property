using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Model;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Transactions;
using NA.PMS.Common;

namespace NA.PMS.Repository
{
    public class PaymentEngineRepository : IPaymentEngineRepository
    {
        private CurrentUserDetail userInfo = new CurrentUserDetail();

        public PaymentEngineRepository()
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

        public PaymentViewModel SavePaymentByRegistrationId(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var recpthed = dbContext.RECEIPT_DETAIL_MASTER.Where(r => r.RECEIPT_ID == model.ReceiptId).FirstOrDefault();
                if (recpthed == null)
                {
                    RECEIPT_DETAIL_MASTER rdm = new RECEIPT_DETAIL_MASTER();
                    rdm.RECEIPT_ID = model.ReceiptId.Value;
                    rdm.RID_NO = model.RegistrationNo;
                    rdm.ALLOTE_NAME = model.AllotteeName;
                    rdm.ADDRESS = model.CorresspondentAddress;
                    rdm.AMOUNT = model.Amount;
                    rdm.SECTOR = model.SectorName;
                    rdm.BLOCK = model.BlockName;
                    rdm.PROP_ID = model.PropertyId;
                    rdm.PROPERTY_NUMBER = model.PropertyNo;
                    rdm.DEPOSETER_NAME = model.DepositorName;
                    rdm.DEPT_ID = model.DepartmentId;
                    rdm.PROP_REG_ID = model.PropertyRegistryId;
                    rdm.ENTRY_DATE = model.EntryDate;
                    rdm.DEPOSIT_DATE = model.DepositDate;
                    rdm.STATUS = 1;
                    rdm.USERID = userInfo.UserID.ToString();
                    rdm.BANK_ID = model.BankId.ToString();
                    rdm.CHALLAN_ID = model.ChallanId;

                    dbContext.RECEIPT_DETAIL_MASTER.Add(rdm);
                    dbContext.SaveChanges();

                    RECEIPT_AMOUNT_TRANS rmt = new RECEIPT_AMOUNT_TRANS();
                    rmt.RECEIPT_ID = model.ReceiptId;
                    rmt.DEPT_CODE = model.DepartmentId;
                    rmt.RECEIPT_SUBHEAD_ID = model.ReceiptSubHeadId;
                    rmt.RECEIPT_HEAD_ID = model.ReceiptCode;
                    rmt.AMOUNT_PAID = model.Amount.Value;
                    rmt.CHALLAN_ID = model.ChallanId;
                    rmt.STATUS = 1;
                    rmt.USERID = userInfo.UserID.ToString();
                    rmt.ENTRY_DATE = model.EntryDate;
                    rmt.DEPOSIT_DATE = model.DepositDate.Value;

                    dbContext.RECEIPT_AMOUNT_TRANS.Add(rmt);
                    dbContext.SaveChanges();
                }
            }
            return model;
        }


        public List<DynamicDataModel> GetPropertyPaymentType()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var receipt = (from recp in dbContext.RECIEPT_HEAD
                               select new DynamicDataModel
                               {
                                   Name = recp.RECIEPT_HEAD_NAME,
                                   Value = recp.RECIEPT_CODE
                               }).ToList();
                return receipt;
            }
        }

        public List<DynamicDataModel> GetPropertySubPaymentType(int receiptId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var subreceipt = (from recp in dbContext.RECEIPT_SUB_HEAD
                                  where recp.RECEIPT_CODE == receiptId
                                  select new DynamicDataModel
                                  {
                                      Name = recp.RECEIPT_SUB_HEAD1,
                                      Value = recp.RECEIPT_SUBHEAD_ID
                                  }).ToList();
                return subreceipt;
            }
        }


        public PaymentViewModel SavePropertyPayment(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var recpthed = dbContext.RECEIPT_DETAIL_MASTER.Where(r => r.RECEIPT_ID == model.ReceiptId).FirstOrDefault();
                if (recpthed == null)
                {
                    RECEIPT_DETAIL_MASTER rdm = new RECEIPT_DETAIL_MASTER();
                    rdm.RECEIPT_ID = model.ReceiptId.Value;
                    rdm.RID_NO = model.RegistrationNo;
                    rdm.ALLOTE_NAME = model.AllotteeName;
                    rdm.ADDRESS = model.CorresspondentAddress;
                    rdm.AMOUNT = model.Amount;
                    rdm.SECTOR = model.SectorName;
                    rdm.BLOCK = model.BlockName;
                    rdm.PROP_ID = model.PropertyId;
                    rdm.PROPERTY_NUMBER = model.PropertyNo;
                    rdm.DEPOSETER_NAME = model.DepositorName;
                    rdm.DEPT_ID = model.DepartmentId;
                    rdm.PROP_REG_ID = model.PropertyRegistryId;
                    rdm.ENTRY_DATE = model.EntryDate;
                    rdm.DEPOSIT_DATE = model.DepositDate;
                    rdm.STATUS = 1;
                    rdm.USERID = userInfo.UserID.ToString();
                    rdm.BANK_ID = model.BankId.ToString();
                    rdm.CHALLAN_ID = model.ChallanId;

                    dbContext.RECEIPT_DETAIL_MASTER.Add(rdm);
                    dbContext.SaveChanges();

                    RECEIPT_AMOUNT_TRANS rmt = new RECEIPT_AMOUNT_TRANS();
                    rmt.RECEIPT_ID = model.ReceiptId;
                    rmt.DEPT_CODE = model.DepartmentId;
                    rmt.RECEIPT_SUBHEAD_ID = model.ReceiptSubHeadId;
                    rmt.RECEIPT_HEAD_ID = model.ReceiptCode;
                    rmt.AMOUNT_PAID = model.Amount.Value;
                    rmt.CHALLAN_ID = model.ChallanId;
                    rmt.STATUS = 1;
                    rmt.USERID = userInfo.UserID.ToString();
                    rmt.ENTRY_DATE = model.EntryDate;
                    rmt.DEPOSIT_DATE = model.DepositDate.Value;

                    dbContext.RECEIPT_AMOUNT_TRANS.Add(rmt);
                    dbContext.SaveChanges();

                    //flag = true;
                }
            }
            return model;
        }

        #region payment Schedule

        public DataSourceResult GetPaymentSchedule(DataSourceRequest req, int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var applicationForm = (from payschtans in dbContext.PaymentScheduleTrans
                                       where payschtans.Rid == rid && payschtans.IsActive == true
                                       select new PaymentScheduleModel
                                       {
                                           Id = payschtans.Id,
                                           RegistrationId = payschtans.Rid,
                                           InstallmentNo = payschtans.InstallmentNo,
                                           InstallmentDueDate = payschtans.InstallmentDueDate,
                                           InstallmentAmount = payschtans.InstallmentAmount,
                                           PrincipalAmount = payschtans.BalanceAmount,
                                           InstallmentInterest = payschtans.InterestAmount,
                                           TotalBalanceAmount = payschtans.InstallmentAmount + payschtans.InterestAmount,
                                           Applicant = string.Empty,
                                           ActionType = string.Empty
                                       });
                return applicationForm.ToDataSourceResult(req);
            }
        }

        //public PaymentScheduleModel GetPayScheduleInfo(int? Rid)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        PaymentScheduleModel applicationForm = new PaymentScheduleModel();
        //        AlloteeBasicInfo alloteeBasicInfo = new AlloteeBasicInfo();

        //        alloteeBasicInfo = (from allotment in dbContext.AllotmentMasters
        //                            join appln in dbContext.ApplicationDetails on allotment.rid equals appln.registrationId.Value
        //                            join deptt in dbContext.DepartmentMsts on appln.departmentId equals deptt.departmentId
        //                            join prop in dbContext.SchemePropTrans on allotment.propertyId equals prop.propertyId
        //                            join sec in dbContext.SectorMsts on prop.sectorId.Value equals sec.sectorId
        //                            join block in dbContext.BlockMsts on prop.blockId.Value equals block.blockId
        //                            join proptype in dbContext.PropertyTypeMsts on prop.propertyTypeId equals proptype.propertyTypeId
        //                            where allotment.rid == Rid
        //                            select new AlloteeBasicInfo
        //                            {
        //                                ApplicantName = appln.tFirstName + " " + appln.tMiddleName + " " + appln.tLastName,
        //                                FatherName = appln.tFatherHusbandName,
        //                                SignatoryAuthority = appln.tSigningAuthority,
        //                                Gender = appln.tGender,
        //                                DepartmentName = deptt.departmentName,
        //                                PropertyNumber = sec.sectorName + "/" + block.blockName + "-" + prop.propertyNo,
        //                                PropertyType = proptype.propertyTypeName,
        //                                AllotmentDate = allotment.allotmentDate
        //                            }).FirstOrDefault();

        //        var napplicationForm = (from payschmaster in dbContext.PaymentScheduleMasters
        //                                where payschmaster.Rid == Rid && payschmaster.IsActive == true
        //                                select new PaymentScheduleModel
        //                                {
        //                                    ScheduleId = payschmaster.ScheduleId,
        //                                    PrincipalAmount = payschmaster.PrincipalAmount,
        //                                    PenalInterest = payschmaster.PenalInterest != null ? (double)payschmaster.PenalInterest : 0,
        //                                    NormalInterest = payschmaster.NormalInterest != null ? (double)payschmaster.NormalInterest : 0,
        //                                    Frequency = payschmaster.FrequencyOfInstallment,
        //                                    TotalInstallment = payschmaster.NoOfInstallment,
        //                                    PeriodOfInstallment = payschmaster.PeriodOfInstallment,
        //                                    InstalmentStartDate = payschmaster.InstallmentStartDate,
        //                                }).FirstOrDefault();

        //        if (napplicationForm != null) { applicationForm = napplicationForm; }
        //        if (alloteeBasicInfo != null) { applicationForm.AlloteeBasicInfo = alloteeBasicInfo; }
        //        return applicationForm;
        //    }

        //}

        public PaymentScheduleModel GetPayScheduleInfo(int? Rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from alotment in dbContext.AllotmentMasters
                              join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId.Value
                              join dpartmnt in dbContext.DepartmentMsts on aplicant.departmentId equals dpartmnt.departmentId
                              join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                              join sector in dbContext.SectorMsts on property.sectorId.Value equals sector.sectorId
                              join block in dbContext.BlockMsts on property.blockId.Value equals block.blockId
                              join proptype in dbContext.PropertyTypeMsts on property.propertyTypeId equals proptype.propertyTypeId
                              where alotment.rid == Rid
                              select new PaymentScheduleModel
                              {
                                  Applicant = aplicant.tGender == "Company" ? aplicant.T_Company_Name : aplicant.tFirstName + " " + aplicant.tMiddleName + " " + aplicant.tLastName,
                                  ApplicantMaster = aplicant.tGender == "Company" ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                  ApplicantType = aplicant.tGender,
                                  Department = dpartmnt.departmentName,
                                  PropertyNo = sector.sectorName + "/" + block.blockName + "-" + property.propertyNo,
                                  PropertyType = proptype.propertyTypeName,
                                  AllotmentDate = alotment.allotmentDate,
                                  PropertyId = alotment.propertyId
                               }).FirstOrDefault();
                var schedule = dbContext.PaymentScheduleMasters.FirstOrDefault(p => p.Rid == Rid && p.IsActive == true);
                if (schedule != null)
                {
                    detail.ScheduleId = schedule.ScheduleId;
                    detail.PrincipalAmount = schedule.PrincipalAmount;
                    detail.PenalInterest = schedule.PenalInterest != null ? (decimal)schedule.PenalInterest : 0;
                    detail.NormalInterest = schedule.NormalInterest != null ? (decimal)schedule.NormalInterest : 0;
                    detail.Frequency = schedule.FrequencyOfInstallment;
                    detail.TotalInstallment = schedule.NoOfInstallment;
                    detail.PeriodOfInstallment = schedule.PeriodOfInstallment;
                    detail.InstallmentStartDate = schedule.InstallmentStartDate;
                    detail.ScheduleType = schedule.ScheduleType;
                    detail.OneTimeLeaseRentStatus = schedule.IsOneTimeLeaseRentPaid == true ? "Y" : "N";
                    detail.OneTimeIstallmentStatus = schedule.IsOneTimeInstallmentPaid == true ? "Y" : "N";
                }
                var installment = (from pay in dbContext.PaymentScheduleTrans where pay.Rid == Rid && pay.IsActive == true select pay).OrderByDescending(s => s.Id).FirstOrDefault();
                //var installment = dbContext.PaymentScheduleTrans.FirstOrDefault(t => t.Rid == Rid && t.IsActive == true);
                if (installment != null)
                {
                    detail.PremiumAmount = installment.InstallmentAmount;
                    detail.TotalBalanceAmount = installment.BalanceAmount;
                    detail.BalanceAmount = installment.BalanceAmount;
                    detail.PreInstallmentNo = installment.InstallmentNo;
                }
                return detail;
            }

        }

        public bool SavePaymentSchedule(PaymentScheduleModel model)
        {
            bool flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                //Procedure to create schedule. Added on 10 august 2017
                var ScheduleFlag = dbContext.Sp_NewPaymentSchedule_OutSide(model.RegistrationId, model.PrincipalAmount, model.TotalInstallment, model.Frequency, model.InstallmentStartDate, model.NormalInterest, model.PenalInterest);
                if (ScheduleFlag > 0) { flag = true; }
                return flag;
            }
        }

        public bool UpdatePaymentSchedule(PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                bool flag = false;
                var payment = dbContext.PaymentScheduleTrans.Where(m => m.Id == model.Id && m.IsActive == true).FirstOrDefault();
                if (payment != null)
                {
                    payment.InstallmentNo = model.InstallmentNo;
                    payment.InstallmentDueDate = model.InstallmentDueDate;
                    payment.InstallmentAmount = model.InstallmentAmount;
                    payment.BalanceAmount = model.BalanceAmount;
                    payment.InterestAmount = model.InstallmentInterest;
                    dbContext.SaveChanges();
                    flag = true;
                }
                return flag;
            }
        }

        public bool RemovePayScheduleInfo(int Rid, int ScheduleId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                bool flag = false;
                try
                {
                    using (var dbContext = new NoidaPMSEntities())
                    {

                        var PaySchMaster = dbContext.PaymentScheduleMasters.Where(m => m.ScheduleId == ScheduleId && m.Rid == Rid).ToList();
                        if (PaySchMaster != null)
                        {
                            int PaySchMasterGroupCount = PaySchMaster.GroupBy(m => m.ScheduleType).Count();
                            if (PaySchMasterGroupCount == 1)
                            {
                                int PaySchCount = PaySchMaster.Where(m => m.ScheduleType == "I").Count();
                                if (PaySchCount == 1)
                                {
                                    var pSchMaster = PaySchMaster.FirstOrDefault();
                                    dbContext.PaymentScheduleMasters.Remove(pSchMaster);
                                    var pSchTrans = dbContext.PaymentScheduleTrans.Where(m => m.ScheduleId == pSchMaster.ScheduleId && m.Rid == Rid && m.ScheduleType == "I").ToList();
                                    dbContext.PaymentScheduleTrans.RemoveRange(pSchTrans);
                                    dbContext.SaveChanges();
                                    flag = true;
                                }
                            }
                        }
                    }
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    throw ex;
                }
                return flag;
            }
        }
        #endregion

        public DataSourceResult GetPartialOrFullPaymentDetails(DataSourceRequest request, string departmentId, string isPremiumPaid, string isLeaseRentPaid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from payment in dbContext.PRE_FULL_PAYMENT_NDC
                              join alotment in dbContext.AllotmentMasters on payment.RegistrationId equals alotment.rid
                              join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                              where payment.Status == "1"
                              && ((departmentId == null || departmentId == "") || alotment.departmentId.ToString() == departmentId)
                              && ((isPremiumPaid == null || isPremiumPaid == "") || payment.TotalPaidPream == (isPremiumPaid == "Yes" ? "Y" : "N"))
                              && ((isLeaseRentPaid == null || isLeaseRentPaid == "") || payment.OneTimeLease == (isLeaseRentPaid == "Y" ? "Y" : "N"))
                              select new PaymentViewModel
                              {
                                  Id = payment.Id,
                                  RegistrationNo = payment.RegistrationId.ToString(),
                                  RegistrationId = payment.RegistrationId,
                                  Applicant = payment.Applicant,
                                  DuePrincipalAmount = payment.DuePrincipalAmount,
                                  DueInterestAmount = payment.DueIntrestAmount,
                                  LeaseRentAmount = payment.LeaseRentAmount,
                                  DateUptoPremium = payment.DateUptoPream,
                                  NDCDate = payment.NDCDate,
                                  LeaseRentUpto = payment.LeaseRentUpto,
                                  TotalPremiumPaidStatus = payment.TotalPaidPream,
                                  IsOneTimeLease = payment.OneTimeLease,
                                  IsPaymentActive = payment.IsActive,
                                  PaymentStatus = payment.Status,
                                  CreatedDate = payment.CreatedOn,
                                  CreatedBy = payment.CreatedBy,

                                  SectorId = property.sectorId,
                                  SectorName = property.SectorMst.sectorName,
                                  BlockId = property.blockId,
                                  BlockName = property.BlockMst.blockName,
                                  PlotNo = property.propertyNo,
                                  DepartmentId = alotment.departmentId,
                                  DepartmentName = alotment.DepartmentMst.departmentName
                              });

                return detail.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetRegistrationIdListForPayment(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var DepartmentList = dbContext.UmUserDepartmentTrans.Where(d => d.UserRefId == userInfo.UserID && d.Status == true).Select(d => d.DepartmentId).ToList();

                var ridlist = (from alotment in dbContext.AllotmentMasters
                               where alotment.isActive == 1 && alotment.isStatus.ToLower() == AllotmentStatus.Approved.ToString().ToLower() 
                               && DepartmentList.Contains(alotment.departmentId)
                               orderby alotment.rid descending
                               select new DropdownViewModel
                               {
                                  Id = alotment.rid,
                                  Text = alotment.rid.ToString()
                               });

                return ridlist.ToDataSourceResult(request);
            }
        }

        public List<DropdownViewModel> GetPaymentModeList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from mode in dbContext.PaymentModeMsts
                            where mode.IsActive == true 
                           select new DropdownViewModel
                           {
                               Id = mode.paymentModeId,
                               Text = mode.paymentModeName
                           }).ToList();
                return list;
            }
        }


        public PaymentScheduleModel SaveInstallmentPaymentSchedule(PaymentScheduleModel model)
        {
            if (model.ActionType == "InstallmentSchedule")
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    var exSchedule = dbContext.PaymentScheduleMasters.FirstOrDefault(s => s.Rid == model.RegistrationId && s.IsActive == true);
                    if (exSchedule == null)
                    {
                        PaymentScheduleMaster schedule = new PaymentScheduleMaster();
                        schedule.Rid = model.RegistrationId;
                        schedule.PropertyId = model.PropertyId;
                        schedule.PrincipalAmount = model.PrincipalAmount;
                        schedule.NoOfInstallment = model.TotalInstallment;
                        schedule.FrequencyOfInstallment = model.Frequency;
                        schedule.PeriodOfInstallment = model.PeriodOfInstallment;
                        schedule.NormalInterest = model.NormalInterest;
                        schedule.PenalInterest = model.PenalInterest;
                        schedule.InstallmentStartDate = model.InstallmentStartDate;
                        schedule.ScheduleType = model.ScheduleType;
                        schedule.IsOneTimeLeaseRentPaid = model.OneTimeLeaseRentStatus == "Y" ? true : false;
                        schedule.IsOneTimeInstallmentPaid = model.LeaseRentStatus == "Y" ? true : false;
                        schedule.IsActive = true;
                        schedule.CreatedBy = userInfo.UserID.ToString();
                        schedule.CreatedDate = DateTime.Now;
                        dbContext.PaymentScheduleMasters.Add(schedule);
                        dbContext.SaveChanges();

                        model.ScheduleId = schedule.ScheduleId;
                    }
                    else
                    {

                    }
                }
            }
            if (model.ActionType == "InstallmentPayment")
            {
               model = SaveInstallmentPamyment(model);
            }
            return model;
        }

        private PaymentScheduleModel SaveInstallmentPamyment(PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var installment = dbContext.PaymentScheduleTrans.FirstOrDefault(p => p.Rid == model.RegistrationId && p.ScheduleId == model.ScheduleId && p.InstallmentNumber == model.InstallmentNo && p.IsActive == true);
                //if (installment == null)
                //{
                    PaymentScheduleTran payment = new PaymentScheduleTran();
                    payment.ScheduleId = model.ScheduleId;
                    payment.Rid = model.RegistrationId;
                    payment.InstallmentNo = model.InstallmentNo;
                    payment.InstallmentAmount = model.InstallmentAmount;
                    payment.InterestAmount = model.InstallmentInterest;
                    payment.BalanceAmount = model.BalanceAmount;
                    payment.InstallmentStartDate = model.InstallmentStartDate;
                    payment.InstallmentEndDate = model.InstallmentEndDate;
                    payment.InstallmentDueDate = model.InstallmentDueDate;
                    payment.InstallmentPeriod = model.InstallmentPeriod;
                    payment.ScheduleType = model.ScheduleType;
                    payment.DepositDate = model.DepositDate;
                    payment.IsInstallmentPaid = true;
                    payment.PaymentMode = model.PaymentMode;
                    payment.TransactionId = model.TransactionId;
                    payment.IsActive = true;
                    payment.CreatedBy = userInfo.UserID.ToString();
                    payment.CreatedDate = DateTime.Now;
                    dbContext.PaymentScheduleTrans.Add(payment); 
                    dbContext.SaveChanges();

                    model.PaymentId = payment.Id;
                //}
            }
            return model;
        }


        public DataSourceResult GetInstallmentPaymentListByRegistrationId(DataSourceRequest request, int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (rid != 0)
                {
                    var paymentList = (from payment in dbContext.PaymentScheduleTrans
                                       where payment.Rid == rid && payment.IsActive == true
                                       select new PaymentScheduleModel
                                       {
                                           Id = payment.Id,
                                           RegistrationId = payment.Rid,
                                           InstallmentNo = payment.InstallmentNo,
                                           PremiumAmount = payment.InstallmentAmount,
                                           InstallmentInterest = payment.InterestAmount,
                                           InstallmentAmount = payment.InstallmentAmount + payment.InterestAmount,
                                           BalanceAmount = payment.BalanceAmount,
                                           TotalBalanceAmount = payment.BalanceAmount,
                                           InstallmentDueDate = payment.InstallmentDueDate,
                                           DepositDate = payment.DepositDate,    
                                           PaymentMode = payment.PaymentMode,                                                                                  
                                           Applicant = null,
                                           ActionType = null,
                                       });
                    return paymentList.ToDataSourceResult(request);
                }
                else return null;
            }
        }


        public List<DropdownViewModel> GetPaymentFrequencyList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var frequecy = (from config in dbContext.Common_Config
                                where config.Category=="Frequency" && config.Is_Active==1
                                select new DropdownViewModel
                                {
                                    Id = config.Range.Value,
                                    Text = config.Name
                                }).ToList();
                return frequecy;
            }
        }


        public DataSourceResult GetAccountHeadList(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from head in dbContext.AccountHeadMsts
                                where head.IsActive == true
                                select new DropdownViewModel
                                {
                                    Id = head.Id,
                                    Text = head.AccountHead
                                });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetAccountSubHeadList(DataSourceRequest request, int? id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from subhead in dbContext.AccountSubHeadMsts
                            where subhead.AccountHeadId==id //&& subhead.IsActive == true
                            select new
                            {
                                Id = subhead.Id,
                                Text = subhead.AccountSubHead
                            });
                request.Filters.RemoveAt(0);
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetPaymentTypeList(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var list = (from head in dbContext.PaymentTypeMsts
                //            where head.IsActive == true
                //            select new DropdownViewModel
                //            {
                //                Id = head.Id,
                //                Text = head.PaymentType
                //            });
                //return list.ToDataSourceResult(request);
                var list = (from head in dbContext.RECIEPT_HEAD
                            where head.STATUS == 1
                            select new DropdownViewModel
                            {
                                Id = head.RECIEPT_CODE,
                                Text = head.RECIEPT_HEAD_NAME
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetPaymentSubTypeList(DataSourceRequest request, int? id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var list = (from subhead in dbContext.PaymentSubTypeMsts
                //            where subhead.PaymentTypeId == id && subhead.IsActive == true
                //            select new DropdownViewModel
                //            {
                //                Id = subhead.Id,
                //                Text = subhead.PaymentSubType
                //            });
                //request.Filters.RemoveAt(0);
                //return list.ToDataSourceResult(request);
                var list = (from subhead in dbContext.RECEIPT_SUB_HEAD
                            where subhead.RECEIPT_CODE == id && subhead.STATUS == 1
                            select new DropdownViewModel
                            {
                                Id = subhead.RECEIPT_SUBHEAD_ID,
                                Text = subhead.RECEIPT_SUB_HEAD1
                            });
                request.Filters.RemoveAt(0);
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetRegistrationIdListFromPaymentSchedule(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from schedule in dbContext.PaymentScheduleMasters
                            where schedule.IsActive == true
                            select new DropdownViewModel
                            {
                                Id = schedule.Rid.Value,
                                Text = schedule.Rid.ToString()
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public LeaseRentViewModel CalculateLeaseRentPremium(LeaseRentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //check in Lease Rent Payment Details
                var leasemaster = dbContext.LeaseRentPayments.Where(r => r.RegistrationId == model.RegistrationId).OrderByDescending(d => d.Id).ToList();
                if (leasemaster != null)
                {
                    var lease = leasemaster.FirstOrDefault();
                    var applicant = dbContext.ApplicationDetails.FirstOrDefault(r => r.registrationId == lease.RegistrationId);
                    string allottee = string.Empty;
                    allottee = applicant != null ? applicant.tFirstName : string.Empty;
                    model.RentId = lease.Id;
                    string propertyno = string.Empty;
                    var property = (from alot in dbContext.AllotmentMasters
                                    join prop in dbContext.SchemePropTrans on alot.propertyId equals prop.propertyId
                                    where alot.rid == lease.RegistrationId
                                    select prop).FirstOrDefault();
                    if (property != null)
                    {
                        propertyno = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo;
                    }
                    //string dueshtml = "<style>.print-row:nth-child(2n+1){background:#d5d5d5;}</style>";
                    string dueshtml = "<div id='Report'><div class='row print-row'><div class='col-md-12 print-col' style='text-align:center'><h3>Property Lease Rent Report</h3></div></div>";
                    dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-4 print-col'><b>Allottee: " + allottee + "</b></div><div class='col-md-4 print-col' style='text-align:center'>Property: " + propertyno + "</div><div class='col-md-4 print-col' style='text-align:right'><b>Registration Id: " + lease.RegistrationId + "</b></div></div>";
                    dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>FY</div><div class='col-md-1 print-col'>PI</div><div class='col-md-2 print-col'>Lease Rent</div><div class='col-md-2 print-col'>Interest</div><div class='col-md-2 print-col'>Paid Amount</div><div class='col-md-2 print-col'>Total</div><div class='col-md-2 print-col'>Dues Upto</div></div>";
                    var leasedate = lease.TransferLeaseDate == null ? lease.LeaseDeedDate : lease.TransferLeaseDate;
                    var today = DateTime.Now;
                    //today = today.AddDays(383);
                    var nextdate = lease.PremiumPaidUptoDate == null ? DateTime.Now : lease.PremiumPaidUptoDate;
                    var paidUptoDate = lease.PremiumPaidUptoDate;
                    var revisedate = lease.RevisedPremiumDate == null ? lease.LeaseDeedDate : lease.RevisedPremiumDate;
                    decimal premiumdues = 0;
                    decimal interest = 0;
                    decimal gstamount = 0;
                    var nexttodate = today;
                    string printdate = string.Empty;

                    decimal? total_amount = 0;
                    decimal? reviserate = lease.RevisedRate == null ? 0 : lease.RevisedRate;
                    if (lease.PremiumPaidUptoDate != null)
                    {
                        var premium = lease.PremiumLeaseRent == null ? 0 : lease.PremiumLeaseRent;
                        decimal? rate = lease.PanelInterest == null ? 0 : lease.PanelInterest;
                        decimal? gstrate = lease.GST == null ? 0 : lease.GST;
                        total_amount = (decimal)premium;
                        nexttodate = paidUptoDate.Value.AddMonths(12);

                        while (today > paidUptoDate)
                        {
                            nextdate = paidUptoDate.Value.AddMonths(6);
                            decimal? htmldues = 0;
                            decimal? htmlinterest = 0;
                            decimal? paidamount = 0;
                            string prevyear = string.Empty;
                            string nextyear = string.Empty;
                            if (nextdate.Value.Year > paidUptoDate.Value.Year)
                            {
                                prevyear = paidUptoDate.Value.Year.ToString();
                                nextyear = nextdate.Value.Year.ToString();
                            }
                            else
                            {
                                prevyear = (paidUptoDate.Value.Year - 1).ToString();
                                nextyear = paidUptoDate.Value.Year.ToString();
                            }


                            if (paidUptoDate >= nexttodate)
                            {
                                nexttodate = paidUptoDate.Value.AddMonths(12);

                                if (paidUptoDate >= revisedate)
                                {
                                    total_amount = total_amount + ((decimal)premium + ((decimal)premium * reviserate) / 100);
                                    var premiumnew = (decimal)premium + ((decimal)premium * reviserate) / 100;
                                    premium = Math.Round((decimal)premium, 2);
                                }
                                else
                                {
                                    total_amount = total_amount + (decimal)premium;
                                }
                            }

                            if (today > nextdate)
                            {
                                var rentPaidList = dbContext.ViewLeaseRentChecks.OrderByDescending(r => r.DEPOSIT_DATE).FirstOrDefault(r => r.RID_NO == model.RegistrationId.ToString() && r.RECEIPT_SUBHEAD_ID == 4 && (r.DEPOSIT_DATE >= paidUptoDate && r.DEPOSIT_DATE <= nextdate));
                                if (rentPaidList != null)
                                {
                                    //var rentPaid = rentPaidList.FirstOrDefault();
                                    var depositdate = rentPaidList.DEPOSIT_DATE;
                                    var amountpaid = rentPaidList.AMOUNT_PAID;
                                    var day = paidUptoDate - depositdate;
                                    decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                                    paidamount = amountpaid;
                                    total_amount = (total_amount + rinterest) - amountpaid;
                                    day = nextdate - depositdate;
                                    rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                                    total_amount = (total_amount + rinterest);
                                    premiumdues = Math.Round((decimal)total_amount, 2);
                                    interest = Math.Round((decimal)rinterest, 2);
                                }
                                else
                                {
                                    var day = nextdate - paidUptoDate;
                                    decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                                    total_amount = (total_amount + rinterest);
                                    premiumdues = Math.Round((decimal)total_amount, 2);
                                    interest = Math.Round((decimal)rinterest, 2);
                                }
                                paidUptoDate = nextdate;
                            }
                            else
                            {
                                var rentPaidList = dbContext.ViewLeaseRentChecks.OrderByDescending(r => r.DEPOSIT_DATE).FirstOrDefault(r => r.RID_NO == model.RegistrationId.ToString() && r.RECEIPT_SUBHEAD_ID == 4 && (r.DEPOSIT_DATE >= paidUptoDate && r.DEPOSIT_DATE <= today));
                                if (rentPaidList != null)
                                {
                                    //var rentPaid = rentPaidList.FirstOrDefault();
                                    var depositdate = rentPaidList.DEPOSIT_DATE;
                                    var amountpaid = rentPaidList.AMOUNT_PAID;
                                    var day = paidUptoDate - depositdate;
                                    decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                                    total_amount = (total_amount + rinterest) - amountpaid;
                                    day = nextdate - depositdate;
                                    rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                                    total_amount = (total_amount + rinterest);
                                    premiumdues = Math.Round((decimal)total_amount, 2);
                                    interest = Math.Round((decimal)rinterest, 2);
                                }
                                else
                                {
                                    var day = paidUptoDate - nextdate;

                                    if (paidUptoDate.Value.Date >= today.Date)
                                    {
                                        day = paidUptoDate - today;
                                    }

                                    decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                                    total_amount = (total_amount + rinterest);
                                    premiumdues = Math.Round((decimal)total_amount, 2);
                                    interest = Math.Round((decimal)rinterest, 2);
                                }
                                paidUptoDate = nextdate;
                            }
                            htmldues = premium;
                            htmlinterest = interest;
                            printdate = paidUptoDate.Value.Day + "-" + paidUptoDate.Value.Month + "-" + paidUptoDate.Value.Year;
                            string fy = prevyear + "-" + nextyear;
                            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 5) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

                        }

                        //dueshtml = dueshtml + "<tr><td>Noida Authority</td></tr>";

                        var days2 = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                        var today2 = DateTime.Now.AddDays(days2 - DateTime.Now.Day);

                        model.LeaseRentPremium = premiumdues;
                        model.PremiumInterest = interest;
                        model.GstAmount = gstamount;
                        model.PaidUptoDate = paidUptoDate;
                        model.DuesUptoDate = today2;
                        dueshtml = dueshtml + "</div>";
                        model.HtmlDuesReport = dueshtml;
                    }
                    else
                    {
                        model.Comment = "Lease Rent is not paid";
                    }
                }
                else
                {
                    model.Comment = "Lease Rent is not paid";
                }
            }
            return model;
        }


        //public LeaseRentViewModel CalculateLeaseRentPremium(LeaseRentViewModel model)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        //check in Lease Rent Payment Details
        //        var leasemaster = dbContext.LeaseRentPayments.Where(r => r.RegistrationId == model.RegistrationId).OrderByDescending(d => d.Id).ToList();
        //        if (leasemaster != null)
        //        {
        //            var lease = leasemaster.FirstOrDefault();
        //            model.RentId = lease.Id;

        //            var leasedate = lease.TransferLeaseDate == null ? lease.LeaseDeedDate : lease.TransferLeaseDate;
        //            var today = DateTime.Now.Date;

        //            if (lease.PremiumPaidUptoDate != null)
        //            {
        //                if (today > lease.PremiumPaidUptoDate.Value.AddMonths(6))
        //                {
        //                    if (lease.RevisedPremiumDate != null)
        //                    {
        //                        if (today > lease.RevisedPremiumDate.Value.AddYears(10))
        //                        {
        //                            model.RevisedDate = lease.RevisedPremiumDate.Value.AddYears(10);
        //                            var newpremium = (lease.PremiumLeaseRent + (lease.PremiumLeaseRent * (50 / 100)));
        //                            model.RevisedPremium = Math.Round((decimal)newpremium, 2);

        //                            decimal amount = (decimal)lease.PremiumLeaseRent;
        //                            decimal rate = (decimal)lease.PanelInterest;
        //                            double totalamount = (double)amount * Math.Pow(1 + (double)rate / 100, 10);
        //                            var gsts = lease.GST == null ? 0 : lease.GST;
        //                            var amountforgst = (decimal)totalamount - lease.PremiumLeaseRent;
        //                            var gstamount = amountforgst * (gsts / 100);
        //                            var duesRent = Math.Round((decimal)totalamount, 2);

        //                            var extdays = today.Date - model.RevisedDate.Value.Date;
        //                            var interest = ((model.RevisedPremium * ((decimal)lease.PanelInterest / 100)) / 365) * extdays.Days;
        //                            var gst = (model.RevisedPremium * ((decimal)lease.GST / 100));
        //                            var total = (decimal)lease.RevisedPremium + interest + gst;
        //                            model.LeaseRentPremium = Math.Round((decimal)totalamount, 2) + Math.Round((decimal)total, 2);
        //                            model.PremiumInterest = Math.Round((decimal)interest,2);
        //                            model.GstAmount =  Math.Round((decimal)gstamount,2) +  Math.Round((decimal)gst,2);
        //                        }
        //                        else
        //                        {
        //                            var extdays = today.Date - lease.PremiumPaidUptoDate.Value.Date;
        //                            var interest = ((lease.PremiumLeaseRent * (lease.PanelInterest / 100)) / 365) * extdays.Days;
        //                            var gst = (lease.PremiumLeaseRent * (lease.GST / 100));
        //                            var total = lease.PremiumLeaseRent + interest + gst;
        //                            model.LeaseRentPremium = Math.Round((decimal)total, 2);
        //                            model.PremiumInterest = (decimal)interest;
        //                            model.GST = (decimal)gst;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (today > lease.LeaseDeedDate.Value.AddYears(10))
        //                        {
        //                            model.RevisedDate = lease.LeaseDeedDate.Value.AddYears(10);
        //                            var newpremium = (lease.PremiumLeaseRent + (lease.PremiumLeaseRent * (50 / 100)));
        //                            model.RevisedPremium = Math.Round((decimal)newpremium, 2);

        //                            decimal amount = (decimal)lease.PremiumLeaseRent;
        //                            decimal rate = (decimal)lease.PanelInterest;
        //                            double totalamount = (double)amount * Math.Pow(1 + (double)rate / 100, 10);
        //                            var gsts = lease.GST == null ? 0 : lease.GST;
        //                            var amountforgst = (decimal)totalamount - lease.PremiumLeaseRent;
        //                            var gstamount = amountforgst * (gsts / 100);
        //                            var duesRent = Math.Round((decimal)totalamount, 2);

        //                            var extdays = today.Date - model.RevisedDate.Value.Date;
        //                            var interest = ((model.RevisedPremium * ((decimal)lease.PanelInterest / 100)) / 365) * extdays.Days;
        //                            var gst = (model.RevisedPremium * ((decimal)lease.GST / 100));
        //                            var total = (decimal)lease.RevisedPremium + (decimal)interest + gst;

        //                            model.LeaseRentPremium = Math.Round((decimal)totalamount, 2) + Math.Round((decimal)total, 2);
        //                            model.PremiumInterest = Math.Round((decimal)interest, 2);
        //                            model.GstAmount = Math.Round((decimal)gstamount, 2) + Math.Round((decimal)gst, 2);
        //                        }
        //                        else
        //                        {
        //                            var extdays = today.Date - lease.PremiumPaidUptoDate.Value.Date;
        //                            var interest = ((lease.PremiumLeaseRent * (lease.PanelInterest / 100)) / 365) * extdays.Days;
        //                            var gst = (lease.PremiumLeaseRent * (lease.GST / 100));
        //                            var total = lease.PremiumLeaseRent + interest + gst;
        //                            model.LeaseRentPremium = Math.Round((decimal)total, 2);
        //                            model.PremiumInterest = (decimal)interest;
        //                            model.GST = (decimal)gst;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    var extdays = today.Date - lease.PremiumPaidUptoDate.Value.Date;
        //                    var interest = ((lease.PremiumLeaseRent * (lease.PanelInterest / 100)) / 365) * extdays.Days;
        //                    var gst = (lease.PremiumLeaseRent * (lease.GST / 100));
        //                    var total = lease.PremiumLeaseRent + interest + gst;
        //                    model.LeaseRentPremium = Math.Round((decimal)total, 2);
        //                    model.PremiumInterest = (decimal)interest;
        //                    model.GST = (decimal)gst;
        //                    //model.LeaseRentPremium = (decimal)lease.PremiumLeaseRent;
        //                }
        //            }
        //            else
        //            {
        //                model.Comment = "Lease Rent is not paid";
        //            }
        //        }
        //        else
        //        {
        //            model.Comment = "Lease Rent is not paid";
        //        }
        //    }
        //    return model;
        //}

        public bool CheckLeaseRentPayment(LeaseRentViewModel model)
        {
            var flag = true;
            using (var dbContext = new NoidaPMSEntities())
            {
                var paidlist = (from master in dbContext.PaymentDetailMasters
                                join trans in dbContext.PaymentDetailTrans on master.Id equals trans.PaymentId
                                where master.RegistrationId == model.RegistrationId && trans.PaymentTypeId == 4 && trans.PaymentSubTypeId == 4
                                select trans).OrderByDescending(o => o.DepositDate).ToList();
                var payment = paidlist.FirstOrDefault();
            }
            return flag;
        }


        public LeaseRentViewModel CalculateInstallmentDuesPremium(LeaseRentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.InstallmentDuesPayments.FirstOrDefault(r => r.RegistrationId == model.RegistrationId);
                if (data != null)
                {
                    if (data.BalanceUptoDate != null)
                    {
                        var balanceUptoDate = data.BalanceUptoDate;
                        decimal rate = data.PenalInterest == null ? 0 : (decimal)data.PenalInterest;

                        var today = DateTime.Now;
                        var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                        today = DateTime.Now.AddDays(days - DateTime.Now.Day);

                        var nextdate = data.BalanceUptoDate == null ? DateTime.Now : data.BalanceUptoDate;
                        decimal duesAmount = 0;
                        decimal gsamount = 0;
                        string allottee = string.Empty;
                        string propertyno = string.Empty;

                        var applicant = dbContext.ApplicationDetails.FirstOrDefault(r => r.registrationId == data.RegistrationId);
                        allottee = applicant != null ? applicant.tFirstName : string.Empty;

                        var property = (from alot in dbContext.AllotmentMasters
                                        join prop in dbContext.SchemePropTrans on alot.propertyId equals prop.propertyId
                                        where alot.rid == data.RegistrationId
                                        select prop).FirstOrDefault();
                        propertyno = property == null ? string.Empty : property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo;

                        string dueshtml = "<div id='Report'><div class='row print-row'><div class='col-md-12 print-col' style='text-align:center'><h3>Property Installment Dues Report</h3></div></div>";
                        dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-4 print-col'><b>Allottee: " + allottee + "</b></div><div class='col-md-4 print-col' style='text-align:center'>Property: " + propertyno + "</div><div class='col-md-4 print-col' style='text-align:right'><b>Registration Id: " + data.RegistrationId + "</b></div></div>";
                        dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>FY</div><div class='col-md-1 print-col'>PI</div><div class='col-md-2 print-col'>Installment</div><div class='col-md-2 print-col'>Interest</div><div class='col-md-2 print-col'>Paid Amount</div><div class='col-md-2 print-col'>Total</div><div class='col-md-2 print-col'>Dues Upto</div></div>";


                        if (balanceUptoDate.Value.Date < today.Date)
                        {
                            while (balanceUptoDate.Value.Date < today.Date)
                            {
                                var balanceAmount = data.BalanceAmount == null ? 0 : data.BalanceAmount;
                                var gst = data.GST == null ? 0 : data.GST;
                                nextdate = balanceUptoDate.Value.AddMonths(12);

                                decimal? htmldues = 0;
                                decimal? htmlbalance = 0;
                                decimal? htmlinterest = 0;
                                decimal? htmlpaidamount = 0;
                                string previousYear = string.Empty;
                                string nextYear = string.Empty;
                                string htmldate = string.Empty;
                                if (balanceUptoDate.Value.Year == today.Year)
                                {
                                    if (balanceUptoDate.Value.Month == 6 || balanceUptoDate.Value.Month == 12)
                                    {
                                        var amount = balanceAmount;
                                        var interest = (balanceAmount * (rate / 2) / 100);
                                        //var gst = data.GST == null ? 0 : data.GST;
                                        var gstinterest = ((decimal)amount - balanceAmount) * gst / 100;
                                        amount = (decimal)amount + (decimal)interest + (decimal)gstinterest;
                                        duesAmount = Math.Round((decimal)amount, 2);
                                        gsamount = Math.Round((decimal)gstinterest, 2);
                                        interest = Math.Round((decimal)interest, 2);

                                        htmldues = duesAmount;
                                        htmlbalance = balanceAmount;
                                        htmlinterest = interest;

                                        balanceUptoDate = balanceUptoDate.Value.AddMonths(6);
                                    }
                                    else
                                    {
                                        if (balanceUptoDate.Value.Month < 6)
                                        {
                                            var extdays = today.Date - balanceUptoDate.Value.Date;
                                            var interest = ((balanceAmount * (rate / 100)) / 365) * extdays.Days;
                                            //var gst = data.GST == null ? 0 : data.GST;
                                            var gstamount = (balanceAmount * (gst / 100));
                                            var total = balanceAmount + interest + gstamount;
                                            duesAmount = Math.Round((decimal)total, 2);
                                            interest = Math.Round((decimal)interest, 2);
                                            gsamount = Math.Round((decimal)gstamount, 2);

                                            htmldues = duesAmount;
                                            htmlbalance = balanceAmount;
                                            htmlinterest = interest;

                                            int mnth = 6 - balanceUptoDate.Value.Month;
                                            balanceUptoDate = balanceUptoDate.Value.AddMonths(mnth);
                                        }
                                        else if (balanceUptoDate.Value.Month > 6)
                                        {
                                            var extdays = today.Date - balanceUptoDate.Value.Date;
                                            var interest = ((balanceAmount * (rate / 100)) / 365) * extdays.Days;
                                            //var gst = data.GST == null ? 0 : data.GST;
                                            var gstamount = (balanceAmount * (gst / 100));
                                            var total = balanceAmount + interest + gstamount;
                                            duesAmount = Math.Round((decimal)total, 2);
                                            interest = Math.Round((decimal)interest, 2);
                                            gsamount = (decimal)gstamount;

                                            htmldues = duesAmount;
                                            htmlbalance = balanceAmount;
                                            htmlinterest = interest;

                                            int mnth = 12 - balanceUptoDate.Value.Month;
                                            balanceUptoDate = balanceUptoDate.Value.AddMonths(mnth);
                                        }
                                    }

                                    previousYear = (balanceUptoDate.Value.Year - 1).ToString();
                                    nextYear = balanceUptoDate.Value.Year.ToString();
                                }
                                else if (today.Year > balanceUptoDate.Value.Year)
                                {
                                    if (today > nextdate)
                                    {
                                        var interest = (balanceAmount * rate) / 100;
                                        var gstamount = (balanceAmount * (gst / 100));
                                        var total = balanceAmount + interest + gstamount;
                                        duesAmount = Math.Round((decimal)total, 2);
                                        interest = Math.Round((decimal)interest, 2);
                                        gsamount = (decimal)gstamount;

                                        htmldues = duesAmount;
                                        htmlbalance = balanceAmount;
                                        htmlinterest = interest;

                                        balanceUptoDate.Value.AddYears(1);
                                    }
                                    else
                                    {
                                        if (balanceUptoDate.Value.Month < 6)
                                        {
                                            var extdays = today.Date - balanceUptoDate.Value.Date;
                                            var interest = ((balanceAmount * (rate / 100)) / 365) * extdays.Days;
                                            //var gst = data.GST == null ? 0 : data.GST;
                                            var gstamount = (balanceAmount * (gst / 100));
                                            var total = balanceAmount + interest + gstamount;
                                            duesAmount = Math.Round((decimal)total, 2);
                                            interest = Math.Round((decimal)interest, 2);
                                            gsamount = Math.Round((decimal)gstamount, 2);

                                            htmldues = duesAmount;
                                            htmlbalance = balanceAmount;
                                            htmlinterest = interest;

                                            int mnth = 6 - balanceUptoDate.Value.Month;
                                            balanceUptoDate = balanceUptoDate.Value.AddMonths(mnth);
                                        }
                                        else if (balanceUptoDate.Value.Month > 6)
                                        {
                                            var extdays = today.Date - balanceUptoDate.Value.Date;
                                            var interest = ((balanceAmount * (rate / 100)) / 365) * extdays.Days;
                                            //var gst = data.GST == null ? 0 : data.GST;
                                            var gstamount = (balanceAmount * (gst / 100));
                                            var total = balanceAmount + interest + gstamount;
                                            duesAmount = Math.Round((decimal)total, 2);
                                            interest = Math.Round((decimal)interest, 2);
                                            gsamount = (decimal)gstamount;

                                            htmldues = duesAmount;
                                            htmlbalance = balanceAmount;
                                            htmlinterest = interest;

                                            int mnth = 12 - balanceUptoDate.Value.Month;
                                            balanceUptoDate = balanceUptoDate.Value.AddMonths(mnth);
                                        }
                                    }

                                    previousYear = balanceUptoDate.Value.Year.ToString();
                                    nextYear = nextdate.Value.Year.ToString();
                                }

                                //htmldues = balanceAmount;
                                //htmlinterest = htmlinterest;
                                htmldate = balanceUptoDate.Value.Day + "-" + balanceUptoDate.Value.Month + "-" + balanceUptoDate.Value.Year;
                                string fy = previousYear + "-" + nextYear;
                                dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmlbalance + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + htmlpaidamount + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmldate + "</div></div>";

                            }
                            var days2 = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                            var today2 = DateTime.Now.AddDays(days2 - DateTime.Now.Day);

                            model.LeaseRentPremium = duesAmount;
                            model.GstAmount = gsamount;
                            model.BalanceUptoDate = balanceUptoDate;
                            model.DuesUptoDate = today2;
                        }
                        else
                        {
                            model.LeaseRentPremium = data.BalanceAmount;
                            model.GstAmount = 0;
                            model.BalanceUptoDate = data.BalanceUptoDate;
                        }
                        dueshtml = dueshtml + "</div>";
                        model.HtmlDuesReport = dueshtml;
                    }
                }
            }
            return model;
        }
    }
}
