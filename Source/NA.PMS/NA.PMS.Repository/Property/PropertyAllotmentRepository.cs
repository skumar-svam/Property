using Kendo.Mvc.UI;
using NA.PMS.Common;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using NA.PMS.Web.Models;
using System.Web;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using System.Data.Entity;

namespace NA.PMS.Repository
{
    public class PropertyAllotmentRepository : IPropertyAllotmentRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public PropertyAllotmentRepository()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["CurrentUser"] != null)
                    {
                        userInfo = HttpContext.Current.Session["CurrentUser"] as CurrentUserDetail;
                        using (var dbContext = new NoidaPMSEntities())
                        {
                            DepartmentList = dbContext.UmUserDepartmentTrans.Where(u => u.UserRefId == userInfo.UserID && u.Status == true).Select(d => d.DepartmentId).ToList();
                        }
                    }
                }
            }
        }

        public List<PropertyAllotmentModel> GetPropertyAllotmentList()
        {
            return null;
        }

        public List<SchemeAllotmentModel> FilterSchemeListOnDepartment(int departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<SchemeAllotmentModel> schemeList = (from scheme in dbContext.SchemeMsts
                                                         join sdt in dbContext.SchemeDepartmentTrans on scheme.schemeId equals sdt.schemeId
                                                         where sdt.departmentId == departmentId
                                                         select new SchemeAllotmentModel
                                                         {
                                                             schemeId = scheme.schemeId,
                                                             schemeName = scheme.schemeName,
                                                             schemeTypeId = scheme.schemeTypeId,
                                                             IsActive = scheme.IsActive,
                                                             completed = scheme.completed
                                                         }).ToList();
                return schemeList;
            }
        }

        public List<DepartmentAllotmentModel> GetDepartmentListForAllotment()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<DepartmentAllotmentModel> departmentList = (from dept in dbContext.UmDepartmentMasters
                                                                 select new DepartmentAllotmentModel
                                                                 {
                                                                     DepartmentId = dept.DepartmentId,
                                                                     DepartmentName = dept.DepartmentName,
                                                                     Status = dept.Status
                                                                 }).ToList();
                return departmentList;
            }
        }

        public List<SchemeAllotmentModel> GetSchemeListForAllotment()
        {
            int userid = userInfo.UserID;
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userid
                                      select dept.DepartmentId).ToList();

                List<SchemeAllotmentModel> schemeList = new List<SchemeAllotmentModel>();
                schemeList = (from schemes in dbContext.SchemeMsts
                              join dpt in dbContext.SchemeDepartmentTrans on schemes.schemeId equals dpt.schemeId
                              where schemes.IsActive == true && schemes.completed == true && loginUserDeptt.Contains(dpt.departmentId) && schemes.Status != Constants.SchemeClosed
                              select new SchemeAllotmentModel
                              {
                                  schemeId = schemes.schemeId,
                                  schemeName = schemes.schemeName
                              }).Distinct().OrderByDescending(x => x.schemeId).ToList();
                return schemeList;
            }

        }

        public List<DepartmentAllotmentModel> FilterDepartmentOnScheme(int schemeId)
        {
            int userid = userInfo.UserID;
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userid
                                      select dept.DepartmentId).ToList();

                List<DepartmentAllotmentModel> departmentList = new List<DepartmentAllotmentModel>();
                departmentList = (from dept in dbContext.DepartmentMsts
                                  join sdt in dbContext.SchemeDepartmentTrans on dept.departmentId equals sdt.departmentId
                                  where sdt.schemeId == schemeId && sdt.IsActive == true && loginUserDeptt.Contains(sdt.departmentId)
                                  select new DepartmentAllotmentModel
                                  {
                                      DepartmentId = dept.departmentId,
                                      DepartmentName = dept.departmentName
                                  }).ToList();
                return departmentList;
            }
        }

        private NotingDetailsModel GetNotingDetails(int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {

                var notingDetails = (from notingMaster in dbContext.Noting_File_Master
                                     join dep in dbContext.DepartmentMsts on notingMaster.Department_Id equals dep.departmentId
                                     //join notings in dbContext.Noting_File_Trans on notingMaster.Id equals notings.Noting_File_Id 
                                     where notingMaster.Is_Active == true && notingMaster.Rid == rid
                                     select new NotingDetailsModel
                                     {
                                         DepartmentId = dep.departmentId,
                                         DepartmentName = dep.departmentName,
                                         Rid = notingMaster.Rid.Value,
                                         FileName = notingMaster.File_Number,
                                         FileNameNotingCreate = notingMaster.File_Number,
                                         NotingDetails = (from uname in dbContext.Noting_File_Trans where uname.Noting_File_Id == notingMaster.Id && uname.Is_Active == true select uname.Noting_Details).FirstOrDefault(),
                                         CreatedBy = (from uname in dbContext.UmUserMasters where uname.UserRefId == notingMaster.Created_By && uname.IsActive == true select uname.FirstName).FirstOrDefault(),
                                         ReceiveBy = (from uname in dbContext.UmUserMasters where uname.UserRefId == notingMaster.Receive_By && uname.IsActive == true select uname.FirstName).FirstOrDefault(),
                                         CreatedDate = notingMaster.Created_Date,
                                         ReceiveByID = notingMaster.Receive_By.Value,
                                         Receive_Date = notingMaster.Receive_Date,
                                         IsNotingDetailShow = true
                                     }).FirstOrDefault();

                //if (notingDetails.ReceiveByID == userInfo.UserID)
                //{
                //    notingDetails.IsNotingDetailShow = true;
                //}
                //else
                //{
                //    notingDetails.IsNotingDetailShow = false;
                //}
                //notingDetails.IsNotingDetailShow = true;

                return notingDetails;
            }
        }
        public DetailedPropertyView GetDetailedPropertyView(int rid)
        {
            DetailedPropertyView detailedPropertyView = new DetailedPropertyView();
            AllotmentModel alot = new AllotmentModel();
            PaymentSchedule paymentSchedule = new PaymentSchedule();
            AlloteeBasicInfo alloteeBasicInfo = new AlloteeBasicInfo();
            PropertyDocument propertyDocument = new PropertyDocument();
            PaymentLedger paymentLedger = new PaymentLedger();
            LetterHistory letterHistory = new LetterHistory();
            NotingDetailsModel noting = new NotingDetailsModel();
            TransferModel transferModel = new TransferModel();
            OtherDetails OtherDetails = new OtherDetails();
            //NDCVeiwModel objNDCVeiwModel = new NDCVeiwModel();
            NDCVeiwModel PreFullPaymentNdc = new NDCVeiwModel();
            InstallmentDues_Payment objInstallmentDuesPaymentViewModel = new InstallmentDues_Payment();
            PaymentViewModel paymentModel = new PaymentViewModel();
            //added for sub lease property - on 28 feb 2018
            SubLeaseViewModel objSubLeaseViewModel = new SubLeaseViewModel();
            ServiceViewModel objServiceViewModel = new ServiceViewModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                var applicationForm = (from applnform in dbContext.ApplicationDetails
                                       where applnform.registrationId == rid
                                       select new ApplicationFormModel
                                       {
                                           FirstName = applnform.tFirstName,
                                           MiddleName = applnform.tMiddleName,
                                           LastName = applnform.tLastName,
                                           OldRegistrationId = applnform.OldRegistrationId
                                       }).FirstOrDefault();

                alot = (from allotment in dbContext.AllotmentMasters
                        where allotment.rid == rid
                        select new AllotmentModel
                        {
                            RID = allotment.rid,
                            DepartmentId = allotment.departmentId,
                            AllotmentDate = allotment.allotmentDate,
                            OldRID = allotment.ApplicationDetail.OldRegistrationId
                        }).FirstOrDefault();

                var validatedProperty = dbContext.ValidatedPropertyDetailMsts.FirstOrDefault(r => r.RegistrationId == rid);
                if (validatedProperty != null)
                {
                    if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Assistant || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.OSD || userInfo.RoleMaster.RoleInDepartment == null)
                    {
                        alot.PropertyUpdateId = 1;
                        alot.IsPropertyValidated = validatedProperty.IsValidatedByDepartment == true ? true : false;
                    }
                    if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Accountant)
                    {
                        alot.PropertyUpdateId = 2;
                        alot.IsPropertyValidated = validatedProperty.IsValidatedByAccounts == true ? true : false;
                    }
                    if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Admin)
                    {
                        alot.PropertyUpdateId = 3;
                        alot.IsPropertyValidated = validatedProperty.IsValidatedByOrganisation == true ? true : false;
                    }
                    alot.DepartmentUpdateId = 1;
                    alot.IsValidatedByDepartment = validatedProperty.IsValidatedByDepartment == true ? true : false;
                    alot.AccountsUpdateId = 2;
                    alot.IsValidatedByAccounts = validatedProperty.IsValidatedByAccounts == true ? true : false;
                    alot.OrganisationUpdateId = 3;
                    alot.IsValidatedByOrganisation = validatedProperty.IsValidatedByOrganisation == true ? true : false;
                }
                else
                {
                    alot.PropertyUpdateId = 0;
                    alot.DepartmentUpdateId = 1;
                    alot.AccountsUpdateId = 2;
                    alot.OrganisationUpdateId = 3;
                    alot.IsPropertyValidated = false;
                    alot.IsValidatedByDepartment = false;
                    alot.IsValidatedByAccounts = false;
                    alot.IsValidatedByOrganisation = false;
                }

                var kya = dbContext.KYADetails.Where(c => c.RId == rid && c.IsActive==true).FirstOrDefault();
                if (kya != null)
                {
                    alot.KYAStatusId = kya.StatusId;
                    alot.KYAStatus = kya.StatusId != null ? dbContext.StatusMasters.FirstOrDefault(k => k.Id == kya.StatusId && k.IsActive==true).Status : string.Empty;
                }

                alloteeBasicInfo = (from allotment in dbContext.AllotmentMasters
                                    join appln in dbContext.ApplicationDetails on allotment.rid equals appln.registrationId.Value
                                    join deptt in dbContext.DepartmentMsts on appln.departmentId equals deptt.departmentId
                                    join prop in dbContext.SchemePropTrans on allotment.propertyId equals prop.propertyId
                                    join sec in dbContext.SectorMsts on prop.sectorId.Value equals sec.sectorId
                                    join block in dbContext.BlockMsts on prop.blockId.Value equals block.blockId
                                    join proptype in dbContext.PropertyTypeMsts on prop.propertyTypeId equals proptype.propertyTypeId
                                    where allotment.rid == rid
                                    select new AlloteeBasicInfo
                                    {
                                        ApplicantName = appln.tFirstName + " " + appln.tMiddleName + " " + appln.tLastName,
                                        FatherName = appln.tFatherHusbandName,
                                        SignatoryAuthority = appln.tSigningAuthority,
                                        Gender = appln.tGender,
                                        DepartmentName = deptt.departmentName,
                                        PropertyNumber = sec.sectorName + "/" + block.blockName + "-" + prop.propertyNo,
                                        PropertyType = proptype.propertyTypeName
                                    }).FirstOrDefault();

                var depttid = dbContext.AllotmentMasters.FirstOrDefault(id => id.rid == rid);
                if (depttid != null)
                {
                    var lst = dbContext.Sp_DuesCalculationTillDate(rid.ToString(), DateTime.Now, depttid.departmentId).ToList();
                    paymentSchedule.TotalDueAmount = lst.FirstOrDefault();
                    paymentSchedule.InstalmentStartDate = (from allot in dbContext.AllotmentMasters where allot.rid == alot.RID && allot.isActive == 1 && allot.isStatus == "Approved" select allot.instalmentStartDate).FirstOrDefault();
                }

                var leaserentndc = dbContext.LeaseRentPayments.FirstOrDefault(l=>l.RegistrationId==rid);
                if (leaserentndc != null)
                {
                    PreFullPaymentNdc.IsLeaseRentPaid = leaserentndc.IsOneTimeLeasePaid == true ? "Yes" : "No";
                    PreFullPaymentNdc.IsPremiumPaid = leaserentndc.IsTotalPremiumPaid == true ? "Yes" : "No";
                    PreFullPaymentNdc.NDCDate = leaserentndc.NDCDate;
                    PreFullPaymentNdc.LastYearLeaseRentPaidUpto = leaserentndc.PremiumPaidDuration;
                }
                else
                {
                    PreFullPaymentNdc.IsLeaseRentPaid = "No";
                }

                //PreFullPaymentNdc = (from preNdc in dbContext.PRE_FULL_PAYMENT_NDC
                //                     where preNdc.RegistrationId == rid && preNdc.IsActive == true
                //                     select new NDCVeiwModel
                //                     {
                //                         NDCDate = preNdc.NDCDate,
                //                         IsPremiumPaid = !string.IsNullOrEmpty(preNdc.TotalPaidPream) ? preNdc.TotalPaidPream == NA.PMS.Common.NDCOptions.Id_Yes ? NA.PMS.Common.NDCOptions.Value_Yes : NA.PMS.Common.NDCOptions.Value_No : string.Empty,
                //                         IsLeaseRentPaid = !string.IsNullOrEmpty(preNdc.OneTimeLease) ? preNdc.OneTimeLease == NA.PMS.Common.NDCOptions.Id_Yes ? NA.PMS.Common.NDCOptions.Value_Yes : NA.PMS.Common.NDCOptions.Value_No : string.Empty,
                //                         LastYearLeaseRentPaidUpto = preNdc.LeaseRentUpto
                //                     }).FirstOrDefault();                
                //if (PreFullPaymentNdc == null)
                //{
                //    PreFullPaymentNdc = new NDCVeiwModel();

                //    var newdate = Convert.ToDateTime("2018/01/01");
                //    var rent = dbContext.LeaseRentPayments.FirstOrDefault(r => r.RegistrationId == rid);
                //    if (rent != null)
                //    {
                //        if (rent.IsOneTimeLeasePaid == true)
                //        {
                //            PreFullPaymentNdc.IsLeaseRentPaid = "Yes";
                //        }
                //        else
                //        {
                //            PreFullPaymentNdc.IsLeaseRentPaid = "No";
                //        }
                //        if (rent.IsTotalPremiumPaid == true)
                //        {
                //            PreFullPaymentNdc.IsPremiumPaid = "Yes";
                //        }
                //        else
                //        {
                //            PreFullPaymentNdc.IsPremiumPaid = "No";
                //        }
                //    }
                //    else
                //    {
                //        PreFullPaymentNdc.IsLeaseRentPaid = "No";
                //    }


                //    //PreFullPaymentNdc.IsLeaseRentPaid = rent == null ? "Yes" : "No";

                //    //var dues = dbContext.InstallmentDuesPayments.FirstOrDefault(d => d.RegistrationId == rid);
                //    //if (dues != null)
                //    //{
                //    //    if (dues.IsTotalPremiumPaid == true)
                //    //    {
                //    //        PreFullPaymentNdc.IsPremiumPaid = "Yes";
                //    //    }
                //    //    else
                //    //    {
                //    //        PreFullPaymentNdc.IsPremiumPaid = "No";
                //    //    }
                //    //}
                //    //else
                //    //{
                //    //    //if (alot.AllotmentDate >= newdate)
                //    //    //{
                //    //        PreFullPaymentNdc.IsPremiumPaid = "No";
                //    //    //}
                //    //    //else
                //    //    //{
                //    //    //    PreFullPaymentNdc.IsPremiumPaid = "Yes";
                //    //    //}
                //    //}

                //    //PreFullPaymentNdc.IsPremiumPaid = dues == null ? "Yes" : "No";
                //}

                var InstallmentPaymentDues = (from installmentDues in dbContext.InstallmentDuesPayments
                                              where installmentDues.RegistrationId == rid
                                              select new InstallmentDues_Payment
                                              {
                                                  InstallmentStartDate = installmentDues.InstallmentStartDate,
                                                  InstallmentEndDate = installmentDues.InstallmentEndDate,
                                                  IsOneTimeLeasePaid = installmentDues.IsOneTimeLeasePaid,
                                                  IsTotalPremiumPaid = installmentDues.IsTotalPremiumPaid,
                                                  DuesAmount = installmentDues.DuesAmount,
                                                  DuesUptoDate = installmentDues.DuesUptoDate == null ? DateTime.Now : installmentDues.DuesUptoDate,
                                                  BalanceAmount = installmentDues.BalanceAmount,
                                                  BalanceUptoDate = installmentDues.BalanceUptoDate == null ? DateTime.Now : installmentDues.BalanceUptoDate
                                              }).FirstOrDefault();

                if (InstallmentPaymentDues != null) { objInstallmentDuesPaymentViewModel = InstallmentPaymentDues; }

                var duesList = (from dues in dbContext.temp_cal
                                where dues.rid == rid
                                select new PaymentViewModel
                                {
                                    DuesAmount = dues.DuesTill,
                                    TotalDuesAmount = dues.DuesNextWithInterest,
                                    PenalInterest = dues.TotalPanelAmount,
                                    PrincipalAmount = dues.TotalPrincipalAmount,
                                    DuesUptoDate = dues.InstallmentDueDate == null ? DateTime.Now : dues.InstallmentDueDate,
                                }).ToList();
                var ILRDues = new PaymentViewModel();
                if (duesList != null && duesList.Count > 0)
                {
                    var dues = duesList[duesList.Count - 1];
                    ILRDues.DuesAmount = dues.DuesAmount;
                    ILRDues.TotalDuesAmount = dues.TotalDuesAmount;
                    ILRDues.PenalInterest = dues.PenalInterest;
                    ILRDues.PrincipalAmount = dues.PrincipalAmount;
                    ILRDues.DuesUptoDate = dues.InstallmentDueDate == null ? DateTime.Now : dues.InstallmentDueDate;
                    //var Idues = new PaymentViewModel
                    //{
                    //    DuesAmount = dues.DuesAmount,
                    //    TotalDuesAmount = dues.TotalDuesAmount,
                    //    PenalInterest = dues.PenalInterest,
                    //    PrincipalAmount = dues.PrincipalAmount,
                    //    DuesUptoDate = dues.InstallmentDueDate == null ? DateTime.Now : dues.InstallmentDueDate,
                    //};
                    //paymentModel = Idues;
                }
                var leaserent = dbContext.LeaseRentPayments.OrderByDescending(r => r.Id).Where(r => r.RegistrationId == rid).FirstOrDefault();
                if (leaserent != null)
                {
                    ILRDues.CurrentDues = leaserent.CurrentDues;
                    ILRDues.CurrentDuesDate = leaserent.CurrentDuesDate;

                    ILRDues.LeaseRentStatus = leaserent.IsOneTimeLeasePaid == true ? "One Time Paid" : string.Empty;
                    ILRDues.IsOneTimeLeaseRentPaid = leaserent.IsOneTimeLeasePaid == true ? true : false;
                }
                

                paymentSchedule.Rid = rid;
                paymentLedger.Rid = rid;
                letterHistory.Rid = rid;
                noting.Rid = rid;
                transferModel.RId = rid;
                OtherDetails.RId = rid;
                objInstallmentDuesPaymentViewModel.RegistrationId = rid;
                paymentSchedule.IsActive = true;
                objSubLeaseViewModel.RegistrationId = rid;
                objServiceViewModel.RegistrationId = rid;
                detailedPropertyView.PropertyDocument = propertyDocument;
                detailedPropertyView.ApplicationDetail = applicationForm;
                detailedPropertyView.AllotmentModel = alot;
                detailedPropertyView.PaymentSchedule = paymentSchedule;
                detailedPropertyView.AlloteeBasicInfo = alloteeBasicInfo;
                detailedPropertyView.PaymentLedger = paymentLedger;
                detailedPropertyView.letterHistory = letterHistory;
                detailedPropertyView.transferModel = transferModel;
                detailedPropertyView.OtherDetails = OtherDetails;
                detailedPropertyView.NDCVeiwModel = PreFullPaymentNdc;
                detailedPropertyView.Noting = GetNotingDetails(rid);
                detailedPropertyView.SubLeaseViewModel = objSubLeaseViewModel;
                detailedPropertyView.ServiceModel = objServiceViewModel;
                detailedPropertyView.InstallmentDuesPayment = objInstallmentDuesPaymentViewModel;
                detailedPropertyView.PaymentModel = paymentModel;
                if (detailedPropertyView.Noting == null)
                {
                    detailedPropertyView.Noting = new NotingDetailsModel();
                }
                var reschedule = (from pay in dbContext.PaymentScheduleMasters where pay.Rid == rid && pay.ScheduleType == Constants.reschedule && pay.IsActive == true select pay).FirstOrDefault();
                if (reschedule != null)
                {
                    detailedPropertyView.isReschedule = true;
                }
                else
                {
                    detailedPropertyView.isReschedule = false;
                }
            }
            return detailedPropertyView;
        }

        public DataSourceResult GetPaymentSchedule(DataSourceRequest req, int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from payment in dbContext.PaymentScheduleTrans
                            where payment.Rid == rid && payment.IsActive == true
                            select new PaymentSchedule
                            {
                                Id = payment.Id,
                                ScheduleId = payment.ScheduleId,
                                Rid = payment.Rid,
                                InstallmentNumber = payment.InstallmentNo,
                                InstallmentDueDate = payment.InstallmentDueDate,
                                PrincipalInstallmentAmount = payment.InstallmentAmount,
                                PrincipalBalAmount = payment.BalanceAmount,
                                InterestInstallmentAmount = payment.InterestAmount,
                                TotalBalAmount = payment.InstallmentAmount + payment.InterestAmount,
                                IsInstallmentPaid = payment.IsInstallmentPaid
                            });
                return list.ToDataSourceResult(req);
            }
        }

        public bool UpdatePaymentScheduleStatus(int ScheduleId, int Rid)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.PaymentScheduleTrans.Where(m => m.Id == ScheduleId && m.Rid == Rid).FirstOrDefault();
                if (data != null)
                {
                    data.IsInstallmentPaid = data.IsInstallmentPaid == null || data.IsInstallmentPaid == false ? true : false;
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }

        public DataSourceResult GetSubLeasePropertyList(DataSourceRequest request, int Rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int AllotmentPropertyId = dbContext.AllotmentMasters.FirstOrDefault(x => x.rid == Rid).propertyId != null ? dbContext.AllotmentMasters.FirstOrDefault(x => x.rid == Rid).propertyId : 0;
                var sublease = (from prop in dbContext.SchemePropTrans
                                where prop.ParentPropertyId != null
                                && prop.ParentPropertyId == AllotmentPropertyId
                                select new SubLeaseViewModel
                                {
                                    Id = prop.refId,
                                    PropertyId = prop.propertyId,
                                    ParentPropertyId = prop.ParentPropertyId,
                                    DepartmentId = prop.departmentId,
                                    Department = prop.DepartmentMst.departmentName,
                                    PropertyRate = prop.landRatePerSqmt,
                                    PropertyCost = prop.propertyCost,
                                    TotalCost = prop.totalPropertyCost,
                                    PropertyArea = prop.totalArea,
                                    SectorId = prop.sectorId,
                                    Sector = prop.SectorMst.sectorName,
                                    BlockId = prop.blockId,
                                    Block = prop.BlockMst.blockName,
                                    PlotNo = prop.propertyNo,
                                    SubLeaseStatus = prop.IsActive == true ? "Active" : "InActive",
                                    RegistrationId = dbContext.AllotmentMasters.Where(a => a.propertyId == prop.propertyId).Select(r => r.rid).FirstOrDefault(),
                                    ParentRegistrationId = dbContext.AllotmentMasters.Where(a => a.propertyId == prop.ParentPropertyId).Select(r => r.rid).FirstOrDefault()
                                });
                if (sublease != null)
                {
                    return sublease.ToDataSourceResult(request);
                }
                else
                {
                    return null;
                }
            }
        }

        public DataSourceResult GetPaymentReschedule(DataSourceRequest request, int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var rescheduledPayment = (from pay in dbContext.PaymentScheduleMasters
                                          join payTrans in dbContext.PaymentScheduleTrans on pay.ScheduleId equals payTrans.ScheduleId
                                          where pay.Rid == rId && pay.ScheduleType == Constants.reschedule && pay.IsActive == true && payTrans.IsActive == true
                                          select new PaymentSchedule
                                          {
                                              Id = payTrans.Id,
                                              Rid = payTrans.Rid,
                                              InstallmentNumber = payTrans.InstallmentNo,
                                              InstallmentDueDate = payTrans.InstallmentDueDate,
                                              PrincipalInstallmentAmount = payTrans.InstallmentAmount,
                                              PrincipalBalAmount = payTrans.BalanceAmount,
                                              InterestInstallmentAmount = payTrans.InterestAmount,
                                              TotalBalAmount = payTrans.InstallmentAmount + payTrans.InterestAmount
                                          });
                return rescheduledPayment.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetReceiptSchedule(DataSourceRequest req, int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                string Rid = Convert.ToString(rid);
                var applicationreceipt = (from payreceipt in dbContext.ViewReceiptMasters
                                          join receipthead in dbContext.RECIEPT_HEAD on payreceipt.RECEIPT_HEAD_ID equals receipthead.RECIEPT_CODE
                                          join receiptsubhead in dbContext.RECEIPT_SUB_HEAD on payreceipt.RECEIPT_SUBHEAD_ID equals receiptsubhead.RECEIPT_SUBHEAD_ID
                                          where payreceipt.RID_NO.Equals(Rid)
                                          select new PaymentReceipt
                                          {
                                              ID = 1,
                                              RECEIPT_ID = payreceipt.RECEIPT_ID,
                                              RECEIPT_HEAD_Name = receipthead.RECIEPT_HEAD_NAME,
                                              RECEIPT_SUBHEAD_Name = receiptsubhead.RECEIPT_SUB_HEAD1,
                                              CHALLAN_ID = payreceipt.CHALLAN_ID,
                                              DEPOSIT_DATE = payreceipt.DEPOSIT_DATE,
                                              AMOUNT_PAID = payreceipt.AMOUNT_PAID
                                          });
                return applicationreceipt.ToDataSourceResult(req);
            }

        }

        /// <summary>
        /// return scheduled date after allotment of a property
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        public AllottedPropertyDetails GetScheduleDetailsForAllottedProperty(int rid)
        {
            ////AllottedPropertyDetails scheduleDetail = null;
            //using (var dbContext = new NoidaPMSEntities())
            //{
            //    var scheduleDetail = (from alt in dbContext.AllotmentMasters
            //                          join clt in dbContext.ChecklistTrans on alt.rid equals clt.Rid
            //                          join rd in dbContext.RegistryDetails on alt.rid equals rd.Rid
            //                          join pd in dbContext.PossessionDetails on alt.rid equals pd.Rid
            //                          //join clm in dbContext.ChecklistMasters on clt.ChecklistTypeId equals clm.ChecklistTypeId
            //                          //join ctm in dbContext.ChecklistTypeMasters on clt.ChecklistTypeId equals ctm.Id
            //                          where alt.rid == rid
            //                          select new AllottedPropertyDetails
            //                          {
            //                              AllotmentDate = alt.allotmentDate,
            //                              SchemeId = alt.schemeId,
            //                              DepartmentId = alt.departmentId,
            //                              ApplicationId = alt.applicationId,
            //                              FormNo = alt.formNo,
            //                              ChecklistDate = clt.ChecklistDate,
            //                              LeaseDeedDueDate = rd.RegistryDueDate,
            //                              LeaseDeedDate = rd.RegistryDoneDate,
            //                              PossessionOrderDate = pd.PossessionOrderDate,
            //                              PossessionDueDate = pd.PossessionDueDate,
            //                              PossessionDate = pd.PossessionDate
            //                          }).FirstOrDefault();
            //    return scheduleDetail;
            //}
            ////return scheduleDetail;


            using (var dbContext = new NoidaPMSEntities())
            {
                AllottedPropertyDetails objAllottedPropertyDetails = new AllottedPropertyDetails();
                objAllottedPropertyDetails =
                      dbContext.Database.SqlQuery<AllottedPropertyDetails>(
                            "select a.AllotmentDate, c.ChecklistDate, d.RegistryDueDate, d.RegistryDoneDate, e.PossessionOrderDate, e.PossessionDueDate, e.PossessionDate, f.Approved_Date, g.Mutation_Date, h.FunctionalDate, i.RentingDate, j.MortgageDate, k.Extension_Given_Date from AllotmentMaster a " +
                            "left outer join [ChecklistTrans] c on a.rid = c.rid and c.IsActive = 1 " +
                            "left outer join [RegistryDetails] d on a.rid = d.rid and d.IsActive = 1 " +
                            "left outer join [PossessionDetails] e on a.rid = e.rid  and e.IsActive = 1 " +
                            "left outer join [Succ_Mut_Trans] f on a.rid = f.Rid and (f.Type = 'T') and f.Status = 1 " +
                            "left outer join [Succ_Mut_Trans] g on a.rid = g.Rid and (g.Type = 'M') and g.Status = 1 " +
                            "left outer join [FunctionalDetails] h on a.rid = h.Rid and h.StatusId = 1 " +
                            "left outer join [RentPermissionDetails] i on a.rid = i.Rid and i.StatusId = 1 " +
                            "left outer join [MortgageDetails] j on a.rid = j.RID and j.StatusId = 1 " +
                            "left outer join [Extension_Details] k on a.rid = k.RID and k.Status = 1 " +
                            "where a.rid = " + rid + "").FirstOrDefault();

                var transfercount = (from a in dbContext.Succ_Mut_Trans
                                     join b in dbContext.AllotmentMasters on a.Rid equals b.rid into transfer
                                     from c in transfer.DefaultIfEmpty()
                                     where a.Rid == rid && a.Type == "T" && a.Status == 1 //&& a.Is_Active == true
                                     group new { c } by new { c.rid } into transferAll
                                     select new
                                     {
                                         Count = transferAll.Count(),
                                         RID = transferAll.Key.rid != null ? transferAll.Key.rid : 0
                                     }).FirstOrDefault();

                var mortgagecount = (from a in dbContext.MortgageDetails
                                     join b in dbContext.AllotmentMasters on a.RID equals b.rid into mortgage
                                     from c in mortgage.DefaultIfEmpty()
                                     where a.RID == rid && a.StatusId == 1 //&& a.IsActive == true
                                     group new { c } by new { c.rid } into mortgageAll
                                     select new
                                     {
                                         Count = mortgageAll.Count(),
                                         RID = mortgageAll.Key.rid != null ? mortgageAll.Key.rid : 0
                                     }).FirstOrDefault();

                var rentpermissioncount = (from a in dbContext.RentPermissionDetails
                                           join b in dbContext.AllotmentMasters on a.Rid equals b.rid into rentpermission
                                           from c in rentpermission.DefaultIfEmpty()
                                           where a.Rid == rid && a.StatusId == 1 //&& a.IsActive == true
                                           group new { c } by new { c.rid } into rentpermissionAll
                                           select new
                                           {
                                               Count = rentpermissionAll.Count(),
                                               RID = rentpermissionAll.Key.rid != null ? rentpermissionAll.Key.rid : 0
                                           }).FirstOrDefault();

                var extensioncount = (from a in dbContext.Extension_Details
                                      join b in dbContext.AllotmentMasters on a.Rid equals b.rid into extension
                                      from c in extension.DefaultIfEmpty()
                                      where a.Rid == rid && a.Status == 1 //&& a.Is_Active == true
                                      group new { c } by new { c.rid } into extensionAll
                                      select new
                                      {
                                          Count = extensionAll.Count(),
                                          RID = extensionAll.Key.rid != null ? extensionAll.Key.rid : 0
                                      }).FirstOrDefault();

                if (mortgagecount != null && objAllottedPropertyDetails != null) { objAllottedPropertyDetails.MortgageCount = mortgagecount.Count > 0 ? mortgagecount.Count : 0; }

                if (transfercount != null && objAllottedPropertyDetails != null) { objAllottedPropertyDetails.TransferCount = transfercount.Count > 0 ? transfercount.Count : 0; }

                if (rentpermissioncount != null && objAllottedPropertyDetails != null) { objAllottedPropertyDetails.RentPermissionCount = rentpermissioncount.Count > 0 ? rentpermissioncount.Count : 0; }

                if (extensioncount != null && objAllottedPropertyDetails != null) { objAllottedPropertyDetails.ExtensionCount = extensioncount.Count > 0 ? extensioncount.Count : 0; }

                return objAllottedPropertyDetails;
            }

        }

        public List<DDList> GetAllDocumentType()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from doclst in dbContext.Document_Mst
                           where doclst.Is_Active == true
                           select new DDList
                           {
                               id = doclst.Id,
                               text = doclst.Document_Name
                           }).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Used for reschduling the payment plan for a given RID
        /// </summary>
        /// <param name="rId">RID</param>
        /// <returns></returns>
        public bool ReschedulePayments(int rId, decimal dueAmnt)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                dbContext.Sp_PaymentReSchedule(rId, dueAmnt);
                flag = true;
            }
            return flag;
        }

        //To GetPayment Ledger
        public DataSourceResult GetPaymentLedger(DataSourceRequest request, int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<PaymentLedger> lstPay = new List<PaymentLedger>();
                var paymentLedger = dbContext.SpAccountLedger(rId).ToList();
                if (paymentLedger != null)
                {
                    if (paymentLedger != null)
                    {
                        foreach (var item in paymentLedger)
                        {
                            var lstPayLed = new PaymentLedger
                            {
                                Id = item.Id,
                                Rid = item.Rid,
                                ReceiptSubheadName = item.RECEIPT_SUB_HEAD,
                                Entry_Date = item.Entry_Date,
                                Debit_Amount = item.Debit_Amount,
                                Credit_Amount = item.Credit_Amount,
                                Balance_Amount = item.Balance_Amount
                            };
                            lstPay.Add(lstPayLed);
                        }
                    }
                }
                return lstPay.ToDataSourceResult(request);
            }
        }

        //To Get Letter History
        public DataSourceResult GetLetterHistory(DataSourceRequest request, int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {

                var paymentLedger = (from payhis in dbContext.Letter_History
                                     where payhis.Rid == rId
                                     select new LetterHistory
                                     {
                                         Id = payhis.Id,
                                         Generate_Date = payhis.Generate_Date,
                                         LetterName = (from tem in dbContext.TemplateMasters where tem.templateId == payhis.Template_Id select tem.templateName).FirstOrDefault()
                                     }).ToList();
                int i = 1;
                foreach (var items in paymentLedger)
                {
                    items.Sno = i;
                    i = i + 1;
                }
                return paymentLedger.ToDataSourceResult(request);
            }
        }
        // To Save Extension
        public bool SaveExtension(int rid, int OnlineRequestRefNo, string propertyNu, DateTime completionDueDate, DateTime extensionGivenDate, decimal extensionCharge, string user)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var PreExtension = dbContext.Extension_Details.Where(m => m.Rid == rid && m.Is_Active == true).ToList();
                if (PreExtension != null && PreExtension.Count > 0)
                {
                    PreExtension.ForEach(i => i.Is_Active = false);
                }

                var dbResult = new Extension_Details
                {
                    Rid = rid,
                    OnlineRequestNo = OnlineRequestRefNo != null ? OnlineRequestRefNo : 0,
                    Property_No = propertyNu,
                    Completion_DueDate = completionDueDate,
                    Extension_Due_Date = extensionGivenDate,
                    Extension_Given_Date = DateTime.Now,
                    Extension_Charge = extensionCharge,
                    Is_Active = true,
                    Status = Constants.InProgress,
                    //Approved_By = (from uname in dbContext.UmUserMasters where uname.UserName.ToLower().Equals(user.ToLower()) && uname.IsActive == true select uname.UserRefId).FirstOrDefault(),
                    Approved_By = Convert.ToInt32(user),
                    Created_By = userInfo.UserID,
                    Created_Date = DateTime.Now
                };
                dbContext.Extension_Details.Add(dbResult);
                dbContext.SaveChanges();
                return true;
            }
        }

        // To Get the details of property.
        public ExtensionDetails GetPropertyDetails(int rid)
        {
            var lst = new ExtensionDetails();
            using (var dbContext = new NoidaPMSEntities())
            {

                lst = (from allot in dbContext.AllotmentMasters
                       join applicant in dbContext.ApplicationDetails on allot.rid equals applicant.registrationId
                       join sche in dbContext.SchemeMsts on applicant.schemeId equals sche.schemeId
                       join deptt in dbContext.DepartmentMsts on applicant.departmentId equals deptt.departmentId
                       join proptrans in dbContext.SchemePropTrans on allot.propertyId equals proptrans.propertyId
                       join proptype in dbContext.PropertyTypeMsts on proptrans.propertyTypeId equals proptype.propertyTypeId
                       join sec in dbContext.SectorMsts on proptrans.sectorId equals sec.sectorId
                       join block in dbContext.BlockMsts on proptrans.blockId equals block.blockId
                       where allot.rid == rid && allot.isActive == 1
                       select new ExtensionDetails
                       {
                           SchemeName = sche.schemeName,
                           DepartmentName = deptt.departmentName,
                           ApplicantName = applicant.tFirstName + " " + applicant.tMiddleName + " " + applicant.tLastName,
                           Gender = applicant.tGender,
                           FatherName = applicant.tFatherHusbandName,
                           PropertyType = proptype.propertyTypeName,
                           PropertyNo = sec.sectorName + "/" + block.blockName + " - " + proptrans.propertyNo,
                           Area = proptrans.totalArea,
                           Floor = proptrans.FloorMst.floorName,
                           Leasedeeddate = (from leasedet in dbContext.RegistryDetails where leasedet.Rid == rid && leasedet.IsActive == true select leasedet.RegistryDoneDate).FirstOrDefault(),
                           Possessiondate = (from possession in dbContext.PossessionDetails where possession.Rid == rid && possession.IsActive == true select possession.PossessionDate).FirstOrDefault(),
                           BuildingPlan = (from buildplan in dbContext.Building_Plan_Master where buildplan.Rid == rid && buildplan.Is_Completed == true && buildplan.Is_Active == true select "true").FirstOrDefault(),
                           ScheduleActionDay = (from intdays in dbContext.Auto_Schedule where intdays.Is_Active == true && intdays.Scheme_Id == sche.schemeId && intdays.Department_Id == deptt.departmentId select intdays.Duration_In_Days).FirstOrDefault()
                       }).FirstOrDefault();
                var buildingplanCompletiondate = (from bplan in dbContext.Building_Plan_Master where bplan.Rid == rid && bplan.Is_Completed == true && bplan.Is_Active == true select bplan).FirstOrDefault();
                var extensionGivenDate = (from bplan in dbContext.Extension_Details where bplan.Rid == rid && bplan.Is_Active == true && bplan.Status == 1 select bplan).FirstOrDefault();
                if (lst != null)
                {
                    if (extensionGivenDate != null)
                    {
                        if (extensionGivenDate.Extension_Due_Date >= DateTime.Now.Date)
                        {
                            lst.ExtendableFlag = false;
                            //if old extension exists the completion date would be extension given date.
                            lst.Completion_DueDate = extensionGivenDate.Completion_DueDate;
                        }
                        else
                        {
                            lst.ExtendableFlag = true;
                            //if old extension exists the completion date would be extension given date.
                            lst.Completion_DueDate = extensionGivenDate.Extension_Given_Date;
                        }
                    }
                    else
                    {
                        if (buildingplanCompletiondate != null)
                            lst.Completion_DueDate = Convert.ToDateTime(buildingplanCompletiondate.Sanction_Date).AddDays(Convert.ToDouble(lst.ScheduleActionDay));
                        lst.ExtendableFlag = true;
                    }
                }
                return lst;
            }
        }

        // To Fill grid.
        public DataSourceResult GetExtensionDetails(DataSourceRequest request)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbcontext.UmUserMasters
                                      join dept in dbcontext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var lstExtension = (from ext in dbcontext.Extension_Details
                                    join alot in dbcontext.AllotmentMasters on ext.Rid equals alot.rid
                                    join prop in dbcontext.SchemePropTrans on alot.propertyId equals prop.propertyId
                                    join sta in dbcontext.StatusMasters on ext.Status equals sta.Id
                                    where ext.Is_Active == true && loginUserDeptt.Contains(ext.AllotmentMaster.departmentId.Value)
                                    select new ExtensionDetails
                                    {
                                        Id = ext.Id,
                                        Rid = ext.Rid,
                                        DepartmentName = ext.AllotmentMaster.DepartmentMst.departmentName,
                                        PropertyNumber = ext.Property_No,
                                        SectorName = prop.SectorMst.sectorName,
                                        BlockName = prop.BlockMst.blockName,
                                        PropertyNo = prop.propertyNo,
                                        Created_Date = ext.Created_Date,
                                        Approved_Date = ext.Approved_Date,
                                        Approved_By = ext.Approved_By,
                                        AssignTo = (from uname in dbcontext.UmUserMasters where uname.UserRefId == ext.Approved_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                        Status = ext.Status,
                                        StatusName = sta.Status
                                    });
                return lstExtension.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetExtensionDetailsByRid(DataSourceRequest request, int Rid)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbcontext.UmUserMasters
                                      join dept in dbcontext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var lstExtension = (from ext in dbcontext.Extension_Details
                                    join alot in dbcontext.AllotmentMasters on ext.Rid equals alot.rid
                                    join prop in dbcontext.SchemePropTrans on alot.propertyId equals prop.propertyId
                                    join sta in dbcontext.StatusMasters on ext.Status equals sta.Id
                                    where loginUserDeptt.Contains(ext.AllotmentMaster.departmentId.Value) && ext.Rid == Rid && ext.Status == 1
                                    select new ExtensionDetails
                                    {
                                        Id = ext.Id,
                                        Rid = ext.Rid,
                                        DepartmentName = ext.AllotmentMaster.DepartmentMst.departmentName,
                                        PropertyNumber = ext.Property_No,
                                        SectorName = prop.SectorMst.sectorName,
                                        BlockName = prop.BlockMst.blockName,
                                        PropertyNo = prop.propertyNo,
                                        Created_Date = ext.Created_Date,
                                        Approved_Date = ext.Approved_Date,
                                        Approved_By = ext.Approved_By,
                                        AssignTo = (from uname in dbcontext.UmUserMasters where uname.UserRefId == ext.Approved_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                        Status = ext.Status,
                                        StatusName = sta.Status
                                    });
                return lstExtension.ToDataSourceResult(request);
            }
        }

        public ExtensionDetails GetExtensionById(int id)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var lstExtension = (from ext in dbcontext.Extension_Details
                                    join sta in dbcontext.StatusMasters on ext.Status equals sta.Id
                                    where ext.Id == id && ext.Is_Active == true
                                    select new ExtensionDetails
                                    {
                                        Id = ext.Id,
                                        Rid = ext.Rid,
                                        Completion_DueDate = ext.Completion_DueDate,
                                        Extension_Given_Date = ext.Extension_Given_Date,
                                        Extension_Due_Date = ext.Extension_Due_Date,
                                        Extension_Charge = ext.Extension_Charge,
                                        Status = ext.Status,
                                        StatusName = sta.Status,
                                        Approved_Date = ext.Approved_Date,
                                        Approved_By = ext.Approved_By,
                                        Comment = ext.Comment,
                                        AssignTo = (from uname in dbcontext.UmUserMasters where uname.UserRefId == ext.Approved_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                        OnlineRequestRefNo = ext.OnlineRequestNo != null ? (int)ext.OnlineRequestNo : 0
                                    }).FirstOrDefault();
                return lstExtension;
            }
        }

        // To Fill Approval grid.
        public DataSourceResult GetExtensionApprovalDetails(DataSourceRequest request)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbcontext.UmUserMasters
                                      join dept in dbcontext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var lstExtension = (from ext in dbcontext.Extension_Details
                                    join alot in dbcontext.AllotmentMasters on ext.Rid equals alot.rid
                                    join prop in dbcontext.SchemePropTrans on alot.propertyId equals prop.propertyId
                                    join sta in dbcontext.StatusMasters on ext.Status equals sta.Id
                                    where ext.Is_Active == true && ext.Status == Constants.InProgress && ext.Approved_By == userInfo.UserID && loginUserDeptt.Contains(ext.AllotmentMaster.departmentId.Value)
                                    select new ExtensionDetails
                                    {
                                        Id = ext.Id,
                                        Rid = ext.Rid,
                                        DepartmentName = ext.AllotmentMaster.DepartmentMst.departmentName,
                                        PropertyNumber = ext.Property_No,
                                        SectorName = prop.SectorMst.sectorName,
                                        BlockName = prop.BlockMst.blockName,
                                        PropertyNo = prop.propertyNo,
                                        Created_Date = ext.Created_Date,
                                        Approved_Date = ext.Approved_Date,
                                        Approved_By = ext.Approved_By,
                                        AssignTo = (from uname in dbcontext.UmUserMasters where uname.UserRefId == ext.Approved_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                        Status = ext.Status,
                                        StatusName = sta.Status
                                    });
                return lstExtension.ToDataSourceResult(request);
            }
        }

        // To Save Comment by User at the time of approval
        public bool SaveCommentByID(int Id, string Comment, bool acceptReject)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstExtDetails = dbContext.Extension_Details.FirstOrDefault(m => m.Id == Id && m.Is_Active == true);
                var bflag = false;
                if (lstExtDetails != null)
                {
                    lstExtDetails.Status = acceptReject == true ? Constants.Approved : Constants.RejectedProp;
                    lstExtDetails.Modified_By = userInfo.UserID;
                    lstExtDetails.Modified_Date = DateTime.Now;
                    lstExtDetails.Approved_Date = acceptReject == true ? DateTime.Now : DateTime.Parse("01/01/1900");
                    lstExtDetails.Comment = Comment;
                    dbContext.SaveChanges();

                    if (lstExtDetails.Status == Constants.Approved)
                    {
                        if (lstExtDetails.OnlineRequestNo != 0)
                        {
                            var deptId = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == lstExtDetails.Rid).departmentId;
                            PropertyRegistrationRepository repo = new PropertyRegistrationRepository();
                            repo.UpdateServiceRequest(lstExtDetails.OnlineRequestNo, lstExtDetails.Rid, deptId, Comment);
                        }
                    }
                    bflag = true;
                }
                return bflag;
            }
        }

        // To Re-Sumbit for Extension or update
        public bool UpdateExtension(int Id, int rid, string propertyNu, DateTime completionDueDate, DateTime extensionGivenDate, decimal extensionCharge, string user)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstExtDetails = dbContext.Extension_Details.FirstOrDefault(m => m.Id == Id && m.Is_Active == true);
                if (lstExtDetails != null)
                {
                    lstExtDetails.Property_No = propertyNu;
                    lstExtDetails.Completion_DueDate = completionDueDate;
                    lstExtDetails.Extension_Due_Date = extensionGivenDate;
                    lstExtDetails.Extension_Charge = extensionCharge;
                    lstExtDetails.Status = Constants.InProgress;
                    //lstExtDetails.Approved_By = (from uname in dbContext.UmUserMasters where uname.UserName.ToLower().Equals(user.ToLower()) && uname.IsActive == true select uname.UserRefId).FirstOrDefault();
                    lstExtDetails.Approved_By = Convert.ToInt32(user);
                    lstExtDetails.Modified_Date = DateTime.Now;
                    lstExtDetails.Modified_By = userInfo.UserID;
                }
                dbContext.SaveChanges();
                return true;
            }
        }

        //To Cancel Extension Request
        public bool CancelExtension(int Id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstExtDetails = dbContext.Extension_Details.FirstOrDefault(m => m.Id == Id && m.Is_Active == true);
                var bflag = false;
                if (lstExtDetails != null)
                {
                    lstExtDetails.Status = Constants.Cancelled;
                    lstExtDetails.Modified_By = userInfo.UserID;
                    lstExtDetails.Modified_Date = DateTime.Now;
                    dbContext.SaveChanges();
                    bflag = true;
                }
                return bflag;
            }
        }

        //To Generate Extension Letter
        public string GenerateExtensionLetter(int rid)
        {
            string strLettter = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                var Objdeptt = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == rid);
                if (Objdeptt != null)
                {
                    GeneralRepository gen = new GeneralRepository();
                    strLettter = gen.GenerateLetterFromDbByRId(Objdeptt.rid, Constants.ExtensionTemplateID);
                }
            }
            return strLettter;
        }


        public List<DynamicDataModel> GetRegistrationIdForAdvanceSearch()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var rids = (from allotment in dbContext.AllotmentMasters
                            where allotment.isActive == 1 && allotment.isStatus == "Approved"
                            select new DynamicDataModel
                            {
                                Value = allotment.rid
                            }).ToList();
                return rids;
            }
        }

        public List<DynamicDataModel> GetDepartmentForAdvanceSearch()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<int> LandTypeList = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
                var rids = (from dept in dbContext.DepartmentMsts
                            join deptrans in dbContext.UmUserDepartmentTrans on dept.departmentId equals deptrans.DepartmentId
                            where deptrans.UserRefId == userInfo.UserID && LandTypeList.Contains(dept.departmentId)
                            select new DynamicDataModel
                            {
                                Name = dept.departmentName,
                                Value = dept.departmentId
                            }).ToList();
                return rids;
            }
        }

        public List<DynamicDataModel> GetSectorsForAdvanceSearch()
        {
            return null;
        }

        public List<DynamicDataModel> GetBlocksForAdvanceSearch()
        {
            return null;
        }

        public List<AllotmentModel> AdvanceSearchForAllottedProperty(int? rid, int? department, string sector, string block, string plot, string mobileNumber, string name, string fatherName, string motherName, string address)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from alotment in dbContext.AllotmentMasters
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            join sectr in dbContext.SectorMsts on property.sectorId equals sectr.sectorId
                            join blok in dbContext.BlockMsts on property.blockId equals blok.blockId
                            where (rid == null || alotment.rid == rid || alotment.ApplicationDetail.OldRegistrationId == rid)
                                && (department == null || alotment.departmentId == department)
                                && (sector == null || sector == "" || property.SectorMst.sectorName == sector)
                                && (block == null || block == "" || property.BlockMst.blockName == block)
                                && (plot == null || plot == "" || property.propertyNo == plot)
                                && (name == null || name == "" || alotment.ApplicationDetail.firstName == name)
                                && (fatherName == null || fatherName == "" || alotment.ApplicationDetail.fatherHusbandName == fatherName)
                                && (mobileNumber == null || mobileNumber == "" || alotment.ApplicationDetail.tMobileNumber == mobileNumber)
                                && alotment.isActive == 1 && alotment.isStatus == "Approved" && DepartmentList.Contains(alotment.departmentId)
                            select new AllotmentModel
                            {
                                RID = alotment.rid,
                                DepartmentName = property.DepartmentMst.departmentName,
                                ApplicantName = alotment.ApplicationDetail.tFirstName + " " + alotment.ApplicationDetail.tMiddleName + " " + alotment.ApplicationDetail.tLastName,
                                MobileNumber = alotment.ApplicationDetail.tMobileNumber,
                                FatherOrHusbandName = alotment.ApplicationDetail.tFatherHusbandName,
                                SectorName = property.SectorMst.sectorName,
                                BlockName = property.BlockMst.blockName,
                                PropertyNumber = property.propertyNo,
                                CorresspondingAddress = alotment.ApplicationDetail.tCorrespondanceAdd,
                                DepartmentId = alotment.departmentId,
                                SectorId = property.sectorId,
                                OldRID = alotment.ApplicationDetail.OldRegistrationId != null ? alotment.ApplicationDetail.OldRegistrationId : 0,
                                //IsSubleased =
                                //IsThisPropertyHasDocument = FtpHandler.IsDocumentAvailable(alotment.rid)
                            }).ToList();
                return list;
            }
        }

        public DataSourceResult AdvanceSearchForAllottedProperty(DataSourceRequest request, AdvanceSearchModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from alotment in dbContext.AllotmentMasters
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            join sectr in dbContext.SectorMsts on property.sectorId equals sectr.sectorId
                            join blok in dbContext.BlockMsts on property.blockId equals blok.blockId
                            where (model.RegistrationId == null || alotment.rid == model.RegistrationId || alotment.ApplicationDetail.OldRegistrationId == model.RegistrationId)
                                && (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                                && (model.Sector == null || model.Sector == "" || property.SectorMst.sectorName == model.Sector)
                                && (model.Block == null || model.Block == "" || property.BlockMst.blockName == model.Block)
                                && (model.PlotNo == null || model.PlotNo == "" || property.propertyNo == model.PlotNo)
                                && (model.Applicant == null || model.Applicant == "" || alotment.ApplicationDetail.firstName.Contains(model.Applicant))
                                && (model.ApplicantMaster == null || model.ApplicantMaster == "" || alotment.ApplicationDetail.fatherHusbandName.Contains(model.ApplicantMaster))
                                && (model.Address == null || model.Address == "" || alotment.ApplicationDetail.correspondanceAdd.Contains(model.Address))
                                && (model.MobileNo == null || model.MobileNo == "" || alotment.ApplicationDetail.tMobileNumber == model.MobileNo)
                                && alotment.isActive == 1 && alotment.isStatus == "Approved" && DepartmentList.Contains(alotment.departmentId)
                            select new AllotmentModel
                            {
                                RID = alotment.rid,
                                SchemeId = property.schemeId,
                                SchemeName = property.SchemeMst.schemeName,
                                DepartmentName = property.DepartmentMst.departmentName,
                                ApplicantName = alotment.ApplicationDetail.tFirstName + " " + alotment.ApplicationDetail.tMiddleName + " " + alotment.ApplicationDetail.tLastName,
                                MobileNumber = alotment.ApplicationDetail.tMobileNumber,
                                FatherOrHusbandName = alotment.ApplicationDetail.tFatherHusbandName,
                                SectorName = property.SectorMst.sectorName,
                                BlockName = property.BlockMst.blockName,
                                PropertyNumber = property.propertyNo,
                                CorresspondingAddress = alotment.ApplicationDetail.tCorrespondanceAdd,
                                DepartmentId = alotment.departmentId,
                                SectorId = property.sectorId,
                                PropertyId = alotment.propertyId,
                                IsSubleased = (property.ParentPropertyId == null || property.ParentPropertyId == 0) ? false : true,
                                RegistryType = (property.ParentPropertyId == null || property.ParentPropertyId == 0) ? "Lease" : "SubLease",
                                OldRID = alotment.ApplicationDetail.OldRegistrationId != null ? alotment.ApplicationDetail.OldRegistrationId : 0,
                                TotalArea=property.totalArea,
                                AllotmentDate=alotment.allotmentDate
                            });
                return list.ToDataSourceResult(request);
            }
        }

        // Get All RID from possessiondetails
        public DataSourceResult GetAllRIDs(DataSourceRequest Req, int Rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstDeptts = (from u in dbContext.UmUserMasters
                                 join d in dbContext.UmUserDepartmentTrans on u.UserRefId equals d.UserRefId
                                 where u.UserRefId == userInfo.UserID
                                 select d.DepartmentId).ToList();

                var lst = (from f in dbContext.PossessionDetails
                           join allot in dbContext.AllotmentMasters on f.Rid equals allot.rid
                           where f.Possession == true && lstDeptts.Contains(allot.departmentId) && allot.isActive == 1 && allot.DepartmentMst.departmentName.ToLower().Trim() != Departmentenum.Housing.ToString().ToLower().Trim()
                           orderby f.CreatedDate descending
                           select new DDList
                           {
                               id = f.Rid.Value,
                               text = f.Rid.Value.ToString()
                           });

                if (Rid > 0) { lst = lst.Where(m => m.id == Rid); }
                return lst.ToDataSourceResult(Req);
            }
        }


        public System.IO.Stream DownloadExcelApplicationForm(int? schemeId, int? departmentId, string formType)
        {
            List<PropertyApplicationForm> formList = new List<PropertyApplicationForm>();

            Stream stream = null;
            using (var package = new ExcelPackage(stream ?? new MemoryStream()))
            {
                package.Workbook.Properties.Company = "Noida Authority";
                package.Workbook.Properties.Title = "EPPlus Application Form";
                package.Workbook.Properties.Comments = "Application Form is uploaded as bulk in excel sheet";
                package.Workbook.Worksheets.Add("Application Form");
                package.Workbook.Worksheets.Add("Second Sheet");
                var worksheet = package.Workbook.Worksheets[1];
                worksheet.DefaultColWidth = 25;
                worksheet.Cells.Style.WrapText = true;
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                if (formType == "individual")
                {
                    string[] headerName = IndividualForm.FormHeader;
                    for (int i = 1; i <= headerName.Count(); i++)
                    {
                        worksheet.Cells[1, i].Value = headerName[i - 1];
                    }
                }
                if (formType == "company")
                {
                    string[] headerName = CompanyForm.FormHeader;
                    for (int i = 1; i <= headerName.Count(); i++)
                    {
                        worksheet.Cells[1, i].Value = headerName[i - 1];
                    }
                }
                else
                {

                }

                var worksheet2 = package.Workbook.Worksheets[2];
                worksheet2.Cells[1, 1].Value = "abc";
                worksheet2.Cells[2, 1].Value = "abc";
                worksheet2.Cells[3, 1].Value = "abc";
                //worksheet2.Cells[1, 1].Value=properties;
                //worksheet.Cells[1,1].Value = worksheet2!$A$1:$A$3;
                //worksheet.Cells[1,1].Value= Formula("worksheet2!$A$1:$A$3");
                package.Save();
                return package.Stream;
            }
        }


        public string UploadExcelApplicationForm(int schemeId, int departmentId, HttpPostedFileBase uploadExcel)
        {
            //int formFlag = 0;
            string formFlag = null;
            //var exl = new ExcelPackage(uploadExcel);
            var excel = new ExcelPackage(uploadExcel.InputStream);
            string headerFlag = excel.ValidateHeaderNameForIndividualForm();
            if (headerFlag != string.Empty)
            {
                return headerFlag;
            }
            //string fieldFlag = excel.ValidateExcelDataField();
            string fieldFlag = excel.ValidateExcelDataForIndividualForm();
            if (fieldFlag != null)
            {
                return formFlag = fieldFlag;
            }

            using (var dbContext = new NoidaPMSEntities())
            {
                //var formList = excel.ToApplicationFormModel();
                var formList = excel.ToIndividualFormModel();
                if (formList != null)
                {
                    foreach (var frm in formList)
                    {
                        //var fnumber = from app in dbContext.ApplicationDetails where app.formNo == frm.FormNo && app.schemeId == schemeId && app.departmentId == departmentId select app.formNo;
                        var fnumber = dbContext.ApplicationDetails.Where(f => f.formNo == frm.FormNo && f.schemeId == schemeId && f.departmentId == departmentId).FirstOrDefault();
                        if (fnumber != null)
                        {
                            //return formFlag = 2;
                            return formFlag = Constants.Duplicate + " " + Constants.Form + " " + fnumber.formNo;
                        }
                    }
                    foreach (var form in formList)
                    {
                        ApplicationDetail app = new ApplicationDetail();
                        app.schemeId = schemeId;
                        app.departmentId = departmentId;
                        app.formNo = form.FormNo;
                        app.firstName = form.FirstName;
                        app.middleName = form.MiddleName;
                        app.lastName = form.LastName;
                        app.gender = form.Gender.ToLower();
                        app.marritalStatus = form.MarritalStatus;
                        app.fatherHusbandName = form.FatherHusbandName;
                        app.motherName = form.MotherName;
                        app.dateOfBirth = form.DateOfBirth;
                        app.signingAuthority = form.SigningAuthority;
                        app.registeredOffice = form.RegisteredOffice;
                        app.correspondanceAdd = form.CorrespondanceAdd;
                        app.permanentAdd = form.PermanentAdd;
                        app.mobileNumberP1 = form.MobileNumberP1;
                        app.mobileNumberP2 = form.MobileNumberP2;
                        app.phoneNumberP1 = form.PhoneNumberP1;
                        app.phoneNumberP2 = form.PhoneNumberP2;
                        app.faxNumberP1 = form.FaxNumberP1;
                        app.faxNumberP2 = form.FaxNumberP2;
                        app.email = form.Email;
                        app.occupationId = form.OccupationId;
                        app.quotaId = form.QuotaId;
                        app.religionId = form.ReligionId;
                        app.pan = form.PanNumber;
                        app.annualIncome = form.AnnualIncome;
                        //app.createdBy = username;
                        app.createdBy = userInfo.UserID.ToString();
                        app.createdDate = DateTime.Now;

                        dbContext.ApplicationDetails.Add(app);
                        dbContext.SaveChanges();

                        int lastInsertedForm = dbContext.ApplicationDetails.Max(u => u.applicationId);
                        ApplicationPaymentDetail payment = new ApplicationPaymentDetail();
                        payment.applicationId = lastInsertedForm;
                        payment.formNo = form.FormNo;
                        payment.paymentMode = form.PaymentMode.ToUpper();
                        payment.bankId = form.BankId;
                        payment.branchId = form.BranchId;
                        payment.amountDeposited = form.AmountDeposited;
                        payment.ddNo = form.DemandDraftNo;
                        payment.ddIssueBank = form.DemandDraftIssueBank;
                        payment.ddIssueDate = form.DemandDraftIssueDate;
                        payment.utn = form.Utn;
                        //payment.challanNo = form.ChallanNo;
                        //payment.challanIssueDate = form.ChallanIssueDate;
                        //payment.paymentDate = form.PaymentDate;
                        //payment.createdBy = username;
                        payment.createdBy = userInfo.UserID.ToString();
                        payment.createdDate = DateTime.Now;

                        dbContext.ApplicationPaymentDetails.Add(payment);
                        dbContext.SaveChanges();
                    }
                    //return formFlag = 3;
                    return formFlag = Constants.Uploaded;
                }
                else
                {
                    //return formFlag=1;
                    return formFlag = Constants.NotNull;
                }
            }
        }

        public bool AddRemarksForProperty(RemarksDetailsModel ObjRemarks)
        {
            bool flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                string sComment = string.Empty;
                var username = userInfo.UserID.ToString() + "-" + userInfo.FirstName + " " + userInfo.MiddleName + " " + userInfo.LastName + " Dated:- " + DateTime.Now.ToString("dd-MMM-yyyy");
                username = username.Trim();
                var appDetail = dbContext.ApplicationDetails.FirstOrDefault(m => m.registrationId == ObjRemarks.RId);
                if (appDetail != null)
                {
                    if (!string.IsNullOrEmpty(ObjRemarks.Remarks))
                    {
                        string sAddComment = "Comment:" + ObjRemarks.Remarks + " " + username;
                        if (string.IsNullOrEmpty(appDetail.tCorrespondanceAdd)) { sComment = sAddComment; }
                        else { sComment = (sAddComment + "#n#" + appDetail.tCorrespondanceAdd).Replace("#n#", System.Environment.NewLine); }
                        appDetail.tCorrespondanceAdd = sComment;
                        flag = true;
                        dbContext.SaveChanges();
                    }
                }
            }
            return flag;
        }


        public DataSourceResult GetPropertyDetailListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from alotment in dbContext.AllotmentMasters
                            from property in dbContext.SchemePropTrans.Where(p => p.propertyId == alotment.propertyId).DefaultIfEmpty()
                            from aplicant in dbContext.ApplicationDetails.Where(a => a.registrationId == alotment.rid).DefaultIfEmpty()
                            where (model.RegistrationId == null || alotment.rid == model.RegistrationId)
                            && (model.SchemeId == null || property.schemeId == model.SchemeId)
                            && (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                            && (model.SectorId == null || property.sectorId == model.SectorId)
                            && (model.BlockId == null || property.blockId == model.BlockId)
                            && (model.PlotNo == null || property.propertyNo == model.PlotNo)
                            select new PropertyViewModel
                            {
                                Id = property.refId,
                                RegistrationId = alotment.rid,
                                PropertyId = property.propertyId,
                                SchemeId = property.schemeId,
                                SchemeName = property.SchemeMst.schemeName,
                                DepartmentId = property.departmentId,
                                Department = property.DepartmentMst.departmentName,
                                SectorId = property.sectorId,
                                SectorName = property.SectorMst.sectorName,
                                BlockId = property.blockId,
                                BlockName = property.blockId == null ? "NA" : property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                PropertyNo = property.SectorMst.sectorName + "/" + (property.blockId == null ? string.Empty : property.BlockMst.blockName + " - ") + property.propertyNo,
                                PropertyTypeId = property.propertyTypeId,
                                PropertyType = property.PropertyTypeMst.propertyTypeName,
                                TotalArea = property.totalArea,
                                TotalAllotmentRate = property.TotalAllotmentRate,
                                TotalPropertyCost = property.totalPropertyCost,
                                ApplicantType = aplicant.tGender,
                                FirstApplicant = aplicant.gender == Constants.Company ? aplicant.firstName : aplicant.firstName + " " + (string.IsNullOrEmpty(aplicant.middleName) ? string.Empty : aplicant.middleName + " ") + aplicant.lastName,
                                Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                ApplicantAddress = aplicant.tCorrespondanceAdd,
                                FirstApplicantAdd = aplicant.correspondanceAdd,
                                ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                AllotmentDate = alotment.allotmentDate,
                                Status = alotment.isStatus == "Approved" ? "Allotted" : "Not Allotted",
                                MobileNo = aplicant.tMobileNumber,
                                MobileNoI = aplicant.mobileNumberP2,
                                MobileNoII = aplicant.tMobileNumber,
                                Email = aplicant.tEmail,
                                IsPropertyAllotted = string.IsNullOrEmpty(aplicant.isAllotted) ? false : true,
                                IsActive = (alotment.isActive == null || alotment.isActive == 0) ? false : true,
                                IsDocumentAvailable = alotment.isDocAvailable
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult AdvanceSearchForAllottedPropertyII(DataSourceRequest request, AdvanceSearchModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from alotment in dbContext.AllotmentMasters
                            join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            join sectr in dbContext.SectorMsts on property.sectorId equals sectr.sectorId
                            join blok in dbContext.BlockMsts on property.blockId equals blok.blockId
                            where (model.RegistrationId == null || alotment.rid == model.RegistrationId)
                                && (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                                && (model.Sector == null || model.Sector == "" || property.SectorMst.sectorName == model.Sector)
                                && (model.Block == null || model.Block == "" || property.BlockMst.blockName == model.Block)
                                && (model.PlotNo == null || model.PlotNo == "" || property.propertyNo == model.PlotNo)
                                && (model.Applicant == null || model.Applicant == "" || aplicant.tFirstName.Contains(model.Applicant))
                                && (model.ApplicantMaster == null || model.ApplicantMaster == "" || aplicant.tFatherHusbandName.Contains(model.ApplicantMaster))
                                && (model.Address == null || model.Address == "" || aplicant.tCorrespondanceAdd.Contains(model.Address))
                                && (model.MobileNo == null || model.MobileNo == "" || aplicant.tMobileNumber == model.MobileNo)
                                && alotment.isActive == 1 && alotment.isStatus == "Approved" && DepartmentList.Contains(alotment.departmentId)
                            select new AllotmentViewModel
                            {
                                Id = alotment.rid,
                                RegistrationId = alotment.rid,
                                PropertyId = alotment.propertyId,
                                SchemeId = property.schemeId,
                                SchemeName = property.SchemeMst.schemeName,
                                DepartmentId = alotment.departmentId,
                                Department = property.DepartmentMst.departmentName,
                                SectorId = property.sectorId,
                                SectorName = property.SectorMst.sectorName,
                                BlockId = property.blockId,
                                BlockName = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                PropertyNo = property.SectorMst.sectorName + "/" + (property.blockId == null ? string.Empty : property.BlockMst.blockName + "-") + property.propertyNo,
                                ApplicantType = aplicant.tGender,
                                Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                MobileNo = aplicant.tMobileNumber,
                                ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                FatherOrHusbandName = aplicant.tFatherHusbandName,
                                CorresspondAddress = aplicant.tCorrespondanceAdd,
                                IsAllotted = aplicant.isAllotted == "1" ? true : false,
                                AllotmentDate = alotment.allotmentDate,
                                ApplicationId = alotment.applicationId,
                                FormNo = alotment.formNo,
                                TotalArea = property.totalArea,
                                TotalPropertyCost = property.totalPropertyCost
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public AllottedPropertyViewModel GetPropertyDetailByRegistrationId(int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var details = (from alotment in dbContext.AllotmentMasters
                               from property in dbContext.SchemePropTrans.Where(p => p.propertyId == alotment.propertyId).DefaultIfEmpty()
                               from aplicant in dbContext.ApplicationDetails.Where(a => a.registrationId == alotment.rid).DefaultIfEmpty()
                               where (alotment.rid == rid)
                               select new AllottedPropertyViewModel
                               {
                                   Property = new PropertyViewModel
                                   {
                                       Id = property.refId,
                                       RegistrationId = alotment.rid,
                                       PropertyId = property.propertyId,
                                       SchemeId = property.schemeId,
                                       SchemeName = property.SchemeMst.schemeName,
                                       DepartmentId = property.departmentId,
                                       Department = property.DepartmentMst.departmentName,
                                       SectorId = property.sectorId,
                                       SectorName = property.SectorMst.sectorName,
                                       BlockId = property.blockId,
                                       BlockName = property.blockId == null ? "NA" : property.BlockMst.blockName,
                                       PlotNo = property.propertyNo,
                                       PropertyNo = property.SectorMst.sectorName + "/" + (property.blockId == null ? string.Empty : property.BlockMst.blockName + " - ") + property.propertyNo,
                                       TotalArea = property.totalArea,
                                       TotalPropertyCost = property.totalPropertyCost,
                                       AllotmentDate = alotment.allotmentDate,
                                       Status = alotment.isStatus == "Approved" ? "Allotted" : "Not Allotted",
                                       IsPropertyAllotted = string.IsNullOrEmpty(aplicant.isAllotted) ? false : true,
                                       IsActive = (alotment.isActive == null || alotment.isActive == 0) ? false : true
                                   },
                                   Applicant = new ApplicantViewModel
                                   {
                                       ApplicantType = aplicant.tGender,
                                       Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                       ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                       CorrespondAddress = aplicant.tCorrespondanceAdd,
                                       PermanentAddress = aplicant.tGender == Constants.Company ? aplicant.tRegisteredOffice : aplicant.tPermanentAdd,
                                       MobileNo = aplicant.tMobileNumber,
                                       Email = aplicant.tEmail,
                                       MaritalStatus = aplicant.tMarritalStatus,
                                       MotherName = aplicant.tMotherName,
                                       DateOfBirth = aplicant.tDateOfBirth,
                                       PhoneNo = aplicant.tPhoneNumber,
                                       Occupation = aplicant.tOccupationId == null ? string.Empty : dbContext.OccupationMsts.FirstOrDefault(o => o.occupationId == aplicant.tOccupationId).occupation,
                                       PAN = aplicant.tPan,
                                       AnnualIncome = aplicant.tAnnualIncome
                                   }

                               }).FirstOrDefault();
                return details;
            }
        }


        public DataSourceResult GetDocumentListByRegistrationId(DataSourceRequest request, int rid)
        {
            FtpHandler ftp = new FtpHandler();
            string path = ftp.GetDocumentPath(rid, string.Empty, true);//System.Configuration.ConfigurationManager.AppSettings["RootPath"] + "\\" + rid;
            List<string> fileList = ftp.DirSearch(path);
            List<DocumentViewModel> documentList = new List<DocumentViewModel>();
            int count = 1;
            foreach (string file in fileList)
            {
                string filename = file;
                string str1 = string.Empty;
                if (filename.Contains(" "))
                {
                    filename = filename.Replace(" ", "");
                }
                if ((filename.Split('-')).Length > 1)
                {
                    str1 = filename.Substring(0, filename.Length - 4);
                    str1 = str1.Substring(9, str1.Length - 9);
                }
                DocumentViewModel document = new DocumentViewModel();
                document.DocumentPath = ftp.GetDocumentPath(rid, filename, false);
                document.DocumentName = !(string.IsNullOrEmpty(str1)) ? (!(string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[str1])) ? System.Configuration.ConfigurationManager.AppSettings[str1] : "Other Documents") : "Other Documents";
                document.RegistrationId = rid;
                document.SerialNo = count;
                documentList.Add(document);
                count++;
            }
            return documentList.ToDataSourceResult(request);
        }


        public DataSourceResult GetGeneratedLetterByRegistrationId(DataSourceRequest request, int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from letter in dbContext.Letter_History
                            join template in dbContext.TemplateMasters on new { x = letter.Department_Id, y = letter.Template_Id } equals new { x = template.departmentId, y = template.templateId }
                            where (letter.Template_Html != null && letter.Template_Html != "")
                            && letter.Rid == 10000045 //rid
                            select new DocumentViewModel
                            {
                                Id = letter.Id,
                                RegistrationId = letter.Rid,
                                DepartmentId = letter.Department_Id,
                                Department = letter.DepartmentMst.departmentName,
                                DocumentContent = letter.Template_Html,
                                Template = template.templateName,
                                DocumentName = template.templateName,
                                Barcode = letter.Barcode_Val
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetServiceRequestListByRegistrationId(DataSourceRequest request, ServiceViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from requests in dbContext.Customer_ServiceRequest
                            join service in dbContext.CitizenService_Master on new { x1 = requests.DepartmentId, x2 = requests.ServiceId } equals new { x1 = service.Deptt_Id, x2 = service.service_id }
                            where requests.Registration_No == "10000045" //model.RegistrationId.ToString()
                            && (model.Id == null || requests.Id == model.Id)
                            select new ServiceViewModel
                            {
                                Id = requests.Id,
                                RequestId = requests.Id,
                                RegistrationId = model.RegistrationId,
                                RegistrationNo = requests.Registration_No,
                                ServiceId = requests.ServiceId,
                                ServiceName = service.ServiceName,
                                DepartmentId = requests.DepartmentId,
                                Department = requests.DepartmentId == null ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == requests.DepartmentId).departmentName,
                                SubDepartment = (requests.SubDepartment == "A" || requests.SubDepartment == "Account") ? "Account" : "Property",
                                Requestor = requests.RequestorName,
                                RequestorAddress = requests.RequestorAddress,
                                Description = requests.Description,
                                Comment = requests.Comment,
                                CreatedDate = requests.Created_Date,
                                StatusId = requests.Request_Status,
                                Status = dbContext.StatusMasters.Where(s => s.Id == requests.Request_Status).Select(s => s.Status).FirstOrDefault()
                            });
                return data.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetTransferHistoryByIdAsDataSource(DataSourceRequest request, TransferViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<TransferViewModel> transferList = new List<TransferViewModel>();
                if (model.ActionType == "Transfer")
                {
                    var data = (from transfer in dbContext.Succ_Mut_Trans
                                where transfer.Rid == model.RegistrationId
                                select new TransferViewModel
                                {
                                    Id = transfer.Request_No,
                                    RequestNo = transfer.Request_No,
                                    RegistrationId = transfer.Rid,
                                    Applicant = transfer.Applicant_Name,
                                    ApplicantAddress = transfer.Correspondance_Add,
                                    Transferee = transfer.T_Gender == "Company" ? transfer.T_Company_Name : transfer.T_First_Name + " " + transfer.T_Middle_Name + " " + transfer.T_Last_Name,
                                    TransfereeAddress = transfer.T_Correspondence_Add,
                                    TransferType = transfer.Type == "M" ? "Mutation" : "Transfer",
                                    TransferDate = transfer.Transfer_Date,
                                    PropertyNo = null,
                                    IsActive = transfer.Is_Active,
                                    TransferStatus = dbContext.StatusMasters.Where(s => s.Id == transfer.Status).Select(s => s.Status).FirstOrDefault()
                                }).ToList();
                    transferList.AddRange(data);
                }
                if (model.ActionType == "GPA")
                {
                    var data = (from gpa in dbContext.GPAs
                                where gpa.Rid == model.RegistrationId
                                select new TransferViewModel
                                {
                                    Id = gpa.Id,
                                    RegistrationId = gpa.Rid,
                                    GPAHolderName = gpa.GPA_Holder_Name,
                                    GPAHolderAddress = gpa.GPA_Holder_Address,
                                    GPAEffectiveFrom = gpa.Effcetd_From,
                                    GPAEffectiveTo = gpa.Effected_To,
                                    IsActive = gpa.Is_Active,
                                    IsGPAExecuted = gpa.Registered,
                                    Relation = gpa.Relation,
                                    ApprovedDate = gpa.Acceptance_date
                                });
                    transferList.AddRange(data);
                }
                if (model.ActionType == "Nominee")
                {
                    var data = (from nominee in dbContext.Nominee_Details
                                where nominee.Rid == model.RegistrationId
                                select new TransferViewModel
                                {
                                    Id = nominee.Id,
                                    RegistrationId = nominee.Rid,
                                    Nominee = nominee.Nominee_Name,
                                    ApprovedDate = nominee.Nomination_Date,
                                    IsActive = (nominee.Is_Active == null || nominee.Is_Active == 0) ? false : true
                                });
                    transferList.AddRange(data);
                }
                return transferList.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetMortgageHistoryByIdAsDataSource(DataSourceRequest request, MortgageViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from mortgage in dbContext.MortgageDetails
                            join aplicant in dbContext.ApplicationDetails on mortgage.RID equals aplicant.registrationId
                            where mortgage.RID == model.RegistrationId
                            select new MortgageViewModel
                            {
                                Id = mortgage.RequestNo,
                                RegistrationId = mortgage.RID,
                                MortgageDate = mortgage.MortgageDate,
                                MortgageType = mortgage.MortgageType == "2" ? "Collateral" : "Normal",
                                BankName = mortgage.BankName,
                                BranchAddress = mortgage.BranchAddress,
                                MortgageStatus = mortgage.StatusMaster.Status,
                                CreatedDate = mortgage.CreatedDate,
                                ApproveDate = mortgage.ApproveDate,
                                ValidUpto = mortgage.ValidUpto,
                                ProcessingFee = mortgage.ProcessingFee,
                                SanctionedAmount = mortgage.SanctionedAmount,
                                Applicant = aplicant.tGender == Constants.Company ? aplicant.T_Company_Name : aplicant.tFirstName + " " + aplicant.tMiddleName + " " + aplicant.tLastName
                            });
                return data.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetExtensionHistoryByIdAsDataSource(DataSourceRequest request, ExtensionViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from extension in dbContext.Extension_Details
                            where extension.Rid == model.RegistrationId
                            select new ExtensionViewModel
                            {
                                Id = extension.Id,
                                RegistrationId = extension.Rid,
                                CompletionDueDate = extension.Completion_DueDate,
                                ExtensionDueDate = extension.Extension_Due_Date,
                                ExtensionGivenDate = extension.Extension_Given_Date,
                                ExtensionCharge = extension.Extension_Charge,
                                ApprovedDate = extension.Approved_Date,
                                Comment = null,
                                ExtensionStatus = dbContext.StatusMasters.Where(s => s.Id == extension.Status).Select(s => s.Status).FirstOrDefault()
                            });
                return data.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetRentingHistoryByIdAsDataSource(DataSourceRequest request, RentingViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var requests = (from rent in dbContext.RentPermissionDetails
                                join alot in dbContext.AllotmentMasters on rent.Rid equals alot.rid
                                where rent.Rid == model.RegistrationId
                                select new RentingViewModel
                                {
                                    Id = rent.RequestNo,
                                    RegistrationId = rent.Rid.Value,
                                    //SchemeName = alot.SchemeMst.schemeName,
                                    //Department = alot.DepartmentMst.departmentName,
                                    Applicant = alot.ApplicationDetail.tFirstName + " " + alot.ApplicationDetail.tMiddleName + " " + alot.ApplicationDetail.tLastName,
                                    TenantName = rent.TenantName,
                                    RequestDate = rent.RentingDate.Value,
                                    RentStatus = rent.StatusMaster.Status
                                });
                return requests.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetCICHistoryByIdAsDataSource(DataSourceRequest request, CICViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "Directors")
                {
                    var list = (from masters in dbContext.Director_Request_Master
                                join director in dbContext.Firm_Director_Master on masters.Rid equals director.Rid
                                where masters.Rid == model.RegistrationId
                                select new CICViewModel
                                {
                                    Id = masters.Id,
                                    RegistrationId = masters.Rid,
                                    RequestDate = masters.Request_Date,
                                    ApprovedDate = masters.Approved_date,
                                    IsCICActive = masters.Is_Active,
                                    CICCharge = masters.CIC_Charge,
                                    DirectorName = director.Director_Name,
                                    DirectorShare = director.Director_Share,
                                    DirectorId = director.Director_Id,
                                    TypeId = director.Type,
                                    TypeName = director.Type != null ? dbContext.Common_Config.FirstOrDefault(d => d.Id == director.Type && d.Category == "Director").Name : string.Empty,
                                    OnlineRequestNo = masters.OnlineRequestNo
                                });
                    return list != null ? list.ToDataSourceResult(request) : null;
                }
                if (model.ActionType == "FirmName")
                {
                    var list = (from firm in dbContext.Firm_Master
                                where firm.Rid == model.RegistrationId
                                select new CICViewModel
                                {
                                    Id = firm.Id,
                                    OldFirmName = firm.Old_Firm_Name,
                                    NewFirmName = firm.New_Firm_Name,
                                    OldFirmStatus = firm.Old_Firm_Status != null ? dbContext.Common_Config.FirstOrDefault(d => d.Id == firm.Old_Firm_Status && d.Category == "Firm Status").Name : string.Empty,
                                    NewFirmStatus = firm.Old_Firm_Status != null ? dbContext.Common_Config.FirstOrDefault(d => d.Id == firm.New_Firm_Status && d.Category == "Firm Status").Name : string.Empty,
                                    IsCICActive = firm.Is_Active,
                                    ChangeTypeId = firm.Change_Type,
                                    ApprovedDate = firm.Approved_date,
                                    RequestDate = firm.Request_Date
                                });
                    return list != null ? list.ToDataSourceResult(request) : null;
                }
                else
                {
                    return null;
                }
            }
        }

        public DataSourceResult GetFunctionalHistoryByIdAsDataSource(DataSourceRequest request, FunctionalViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from data in dbContext.FunctionalDetails
                            where data.Rid == model.RegistrationId
                            select new FunctionalViewModel
                            {
                                Id = data.RequestNo,
                                RequestNo = data.RequestNo,
                                RegistrationId = data.Rid,
                                PropertyNo = data.PropertyNumber,
                                IsFunctional = data.Functional,
                                FunctionalDueDate = data.FunctionalDueDate,
                                FunctionalDate = data.FunctionalDate,
                                FunctionalCharge = data.FunctionalCharge,
                                IsMeterSealed = data.MeterSeallingDocFlag,
                                IsAffidavit = data.AffidavitFlag,
                                IsRegistered = data.RegistrationCertiFlag,
                                IsNOCAccount = data.NOCAccountFlag,
                                IsActive = data.IsActive,
                                Comment = data.Comment
                            });
                return list != null ? list.ToDataSourceResult(request) : null;
            }
        }


        public DataSourceResult GetLeaseRentPaymentByIdAsDataSource(DataSourceRequest request, LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                if (model.ActionType == "LeaseRent")
                {
                    var list = (from leaserent in dbcontext.LeaseRentPayments
                                where leaserent.RegistrationId == model.RegistrationId
                                select new LeaseRentViewModel
                                {
                                    Id = leaserent.Id,
                                    RentId = leaserent.Id,
                                    RegistrationId = leaserent.RegistrationId,
                                    LeaseDeedDate = leaserent.LeaseDeedDate,
                                    LeaseRentPremium = leaserent.PremiumLeaseRent,
                                    PanelInterest = leaserent.PanelInterest,
                                    PaidUptoDate = leaserent.PremiumPaidUptoDate,
                                    PremiumPaidDuration = leaserent.PremiumPaidDuration,
                                    RevisedDate = leaserent.RevisedPremiumDate,
                                    BalanceAmount = leaserent.BalanceAmount,
                                    BalanceInterest = leaserent.BalanceInterest,
                                    GST = leaserent.GST,
                                    TotalBalance = leaserent.BalanceAmount != null ? leaserent.BalanceAmount : 0 + leaserent.BalanceInterest != null ? leaserent.BalanceInterest : 0 + leaserent.GST != null ? leaserent.GST : 0,
                                    BalanceUptoDate = leaserent.BalanceUptoDate,
                                    RevisedPremium = leaserent.RevisedRate,
                                    IsOneTimeLeaseRentPaid = leaserent.IsOneTimeLeasePaid,
                                    LeaseRentDues = leaserent.LeaseRentDues,//for lease rent dues
                                    OneTimePaidStatus = leaserent.IsOneTimeLeasePaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                                    PremiumPaidStatus = leaserent.IsTotalPremiumPaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                                    NDCDate = leaserent.NDCDate,
                                    NDCStatus = leaserent.NDCStatus,
                                    ChallanDate = leaserent.ChallanDate,
                                    StatusId = leaserent.StatusId,
                                    Status = leaserent.StatusId > 0 ? (from status in dbcontext.StatusMasters where status.Id == leaserent.StatusId select status.Status).FirstOrDefault() : string.Empty,
                                    IsActive = leaserent.IsActive,
                                    Sector = null,
                                    //Block = property.BlockMst.blockName,
                                    //PlotNo = property.propertyNo,
                                    DuesUptoDate = leaserent.DuesUptoDate
                                });
                    return list != null ? list.ToDataSourceResult(request) : null;
                }
                if (model.ActionType == "LeaseRentTrans")
                {
                    var list = (from leaserent in dbcontext.PaymentLeaseRentTrans
                                where leaserent.RentId == model.RentId
                                select new LeaseRentViewModel
                                {
                                    Id = leaserent.Id,
                                    RentId = leaserent.Id,
                                    RegistrationId = leaserent.RegistrationId,
                                    LeaseRentPremium = leaserent.LeaseRentPremium,
                                    DepositDueDate = leaserent.DepositDueDate,
                                    PaymentMode = leaserent.PaymentMode,
                                    TransactionId = leaserent.TransactionId,
                                    DuesAmount = leaserent.DuesAmount,
                                    DuesUptoDate = leaserent.DuesUptoDate,
                                    StatusId = leaserent.StatusId,
                                    Status = leaserent.StatusId > 0 ? (from status in dbcontext.StatusMasters where status.Id == leaserent.StatusId select status.Status).FirstOrDefault() : string.Empty,
                                    IsActive = leaserent.IsActive,
                                    HtmlDuesReport = leaserent.HtmlDuesTemplate,
                                    Sector = null
                                });
                    return list != null ? list.ToDataSourceResult(request) : null;
                }
                else
                {
                    return null;
                }
            }
        }

        public DataSourceResult GetPremiumDuesPaymentByIdAsDataSource(DataSourceRequest request, LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var list = (from instlmnt in dbcontext.InstallmentDuesPayments
                            where instlmnt.RegistrationId == model.RegistrationId
                            select new LeaseRentViewModel
                            {
                                Id = instlmnt.Id,
                                RegistrationId = instlmnt.RegistrationId,
                                DuesAmount = instlmnt.DuesAmount,
                                GstAmount = instlmnt.GstAmount,
                                BalanceAmount = instlmnt.BalanceAmount,
                                BalanceUptoDate = instlmnt.BalanceUptoDate,
                                DuesUptoDate = instlmnt.DuesUptoDate,
                                LeaseRentPremium = instlmnt.BalanceAmount,
                                LeaseRentDues = null
                            });
                return list != null ? list.ToDataSourceResult(request) : null;
            }
        }

        public LeaseRentViewModel GetDuesPaymentStatusByRegistrationId(LeaseRentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "Installment")
                {
                    var data = (from instlmnt in dbContext.InstallmentDuesPayments
                                where instlmnt.RegistrationId == model.RegistrationId
                                select new LeaseRentViewModel
                                {
                                    Id = instlmnt.Id,
                                    RegistrationId = instlmnt.RegistrationId,
                                    DuesAmount = instlmnt.DuesAmount,
                                    GstAmount = instlmnt.GstAmount,
                                    BalanceAmount = instlmnt.BalanceAmount,
                                    BalanceUptoDate = instlmnt.BalanceUptoDate,
                                    DuesUptoDate = instlmnt.DuesUptoDate,
                                    LeaseRentPremium = instlmnt.BalanceAmount,
                                    InstallmentStartDate = instlmnt.InstallmentStartDate,
                                    InstallmentEndDate = instlmnt.InstallmentEndDate,
                                    LeaseRentDues = null
                                }).OrderByDescending(o => o.Id).FirstOrDefault();
                    return data;
                }
                if (model.ActionType == "NDC")
                {
                    var data = (from preNdc in dbContext.PRE_FULL_PAYMENT_NDC
                                where preNdc.RegistrationId == model.RegistrationId && preNdc.IsActive == true && preNdc.Status == "1"
                                select new LeaseRentViewModel
                                {
                                    NDCDate = preNdc.NDCDate,
                                    IsTotalPremiumPaid = string.IsNullOrEmpty(preNdc.TotalPaidPream) ? false : true,
                                    PremiumPaidStatus = !string.IsNullOrEmpty(preNdc.TotalPaidPream) ? preNdc.TotalPaidPream == NA.PMS.Common.NDCOptions.Id_Yes ? NA.PMS.Common.NDCOptions.Value_Yes : NA.PMS.Common.NDCOptions.Value_No : string.Empty,
                                    IsOneTimeLeaseRentPaid = string.IsNullOrEmpty(preNdc.OneTimeLease) ? false : true,
                                    LeaseRentStatus = !string.IsNullOrEmpty(preNdc.OneTimeLease) ? preNdc.OneTimeLease == NA.PMS.Common.NDCOptions.Id_Yes ? NA.PMS.Common.NDCOptions.Value_Yes : NA.PMS.Common.NDCOptions.Value_No : string.Empty,
                                    LeaseRentDuration = preNdc.LeaseRentUpto
                                }).FirstOrDefault();
                    return data;
                }
                else
                {
                    return null;
                }
            }
        }


        public DataSourceResult GetNotingFilesByIdAsDataSource(DataSourceRequest request, NotingViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var notings = (from notingMaster in dbContext.Noting_File_Master
                               join notingfile in dbContext.Noting_File_Trans on notingMaster.Id equals notingfile.Noting_File_Id
                               where notingMaster.Rid == model.RegistrationId
                               select new NotingViewModel
                               {
                                   Id = notingMaster.Id,
                                   NotingFileId = notingMaster.Id,
                                   NotingFileNo = notingMaster.File_Number,
                                   RegistrationId = notingMaster.Rid,
                                   FileName = notingMaster.File_Number,
                                   NotingDetail = notingfile.Noting_Details,
                                   CreatedBy = (from uname in dbContext.UmUserMasters where uname.UserRefId == notingMaster.Created_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                   CreatedDate = notingMaster.Created_Date
                               });
                return notings.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetSubLeasedPropertyByIdAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from prop in dbContext.SchemePropTrans
                            where prop.ParentPropertyId == model.PropertyId
                            select new PropertyViewModel
                            {
                                Id = prop.refId,
                                PropertyId = prop.propertyId,
                                ParentPropertyId = prop.ParentPropertyId,
                                DepartmentId = prop.departmentId,
                                Department = prop.DepartmentMst.departmentName,
                                LandRate = prop.landRatePerSqmt,
                                PropertyCost = prop.propertyCost,
                                TotalPropertyCost = prop.totalPropertyCost,
                                TotalArea = prop.totalArea,
                                SectorId = prop.sectorId,
                                SectorName = prop.SectorMst.sectorName,
                                BlockId = prop.blockId,
                                BlockName = prop.BlockMst.blockName,
                                PlotNo = prop.propertyNo,
                                IsActive = prop.IsActive,
                                Status = prop.IsActive == true ? "Active" : "InActive",
                                RegistrationId = dbContext.AllotmentMasters.Where(a => a.propertyId == prop.propertyId).Select(r => r.rid).FirstOrDefault()
                            });
                return data != null ? data.ToDataSourceResult(request) : null;
            }
        }


        public int UpdateDocumentStatusOfProperty(PropertyViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "Department")
                {
                    var ridList = dbContext.AllotmentMasters.Where(r => r.departmentId == model.DepartmentId).Select(s => s.rid).ToList();
                    foreach (var rid in ridList)
                    {
                        var isdoc = FtpHandler.IsDocumentAvailable(rid);
                        if (isdoc)
                        {
                            var allotment = dbContext.AllotmentMasters.FirstOrDefault(r => r.rid == rid);
                            if (allotment != null)
                            {
                                allotment.isDocAvailable = true;
                                dbContext.SaveChanges();
                                flag = ReturnType.Updated;
                            }
                        }
                    }
                }
                if (model.ActionType == "RegistrationId")
                {
                    var allotment = dbContext.AllotmentMasters.FirstOrDefault(r => r.rid == model.RegistrationId);
                    if (allotment != null)
                    {
                        var isdoc = FtpHandler.IsDocumentAvailable(allotment.rid);
                        if (isdoc)
                        {
                            allotment.isDocAvailable = true;
                            dbContext.SaveChanges();
                            flag = ReturnType.Updated;
                        }
                    }
                }
            }
            return flag;
        }


        public DataSourceResult GetDuesCalcaluationHistoryAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var list = dbcontext.Sp_DuesCalculationHistory(model.RegistrationId.ToString(), model.ActionDate, model.PropertyTypeId).ToList();
                //var dt = Convert.ToDateTime("30-Jun-2018");
                //var list = dbcontext.Sp_DuesCalculationHistory("10001181",dt,1).ToList();
                return list.ToDataSourceResult(request);
            }
        }




        public DataSourceResult GetPropertyListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from alotment in dbContext.AllotmentMasters
                            from property in dbContext.SchemePropTrans.Where(p => p.propertyId == alotment.propertyId).DefaultIfEmpty()
                            from aplicant in dbContext.ApplicationDetails.Where(a => a.registrationId == alotment.rid).DefaultIfEmpty()
                            where (model.RegistrationId == null || alotment.rid == model.RegistrationId)
                            && (model.SchemeId == null || property.schemeId == model.SchemeId)
                            && (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                            && (model.SectorId == null || property.sectorId == model.SectorId)
                            && (model.BlockId == null || property.blockId == model.BlockId)
                            && (model.PlotNo == null || property.propertyNo == model.PlotNo)
                            && (model.Applicant == null || model.Applicant == "" || aplicant.tFirstName.Contains(model.Applicant))
                            && (model.MobileNo == null || model.MobileNo == "" || aplicant.tMobileNumber.Contains(model.MobileNo))
                            select new PropertyViewModel
                            {
                                Id = property.refId,
                                RegistrationId = alotment.rid,
                                PropertyId = property.propertyId,
                                SchemeId = property.schemeId,
                                SchemeName = property.SchemeMst.schemeName,
                                DepartmentId = property.departmentId,
                                Department = property.DepartmentMst.departmentName,
                                SectorId = property.sectorId,
                                SectorName = property.SectorMst.sectorName,
                                BlockId = property.blockId,
                                BlockName = property.blockId == null ? "NA" : property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                PropertyNo = property.SectorMst.sectorName + "/" + (property.blockId == null ? string.Empty : property.BlockMst.blockName + " - ") + property.propertyNo,
                                TotalArea = property.totalArea,
                                TotalPropertyCost = property.totalPropertyCost,
                                ApplicantType = aplicant.tGender,
                                FirstApplicant = aplicant.gender == Constants.Company ? aplicant.firstName : aplicant.firstName + " " + (string.IsNullOrEmpty(aplicant.middleName) ? string.Empty : aplicant.middleName + " ") + aplicant.lastName,
                                Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                ApplicantAddress = aplicant.tCorrespondanceAdd,
                                FirstApplicantAdd = aplicant.correspondanceAdd,
                                ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                AllotmentDate = alotment.allotmentDate,
                                Status = alotment.isStatus == "Approved" ? "Allotted" : "Not Allotted",
                                MobileNo = aplicant.tMobileNumber,
                                Email = aplicant.tEmail,
                                IsPropertyAllotted = string.IsNullOrEmpty(aplicant.isAllotted) ? false : true,
                                IsActive = (alotment.isActive == null || alotment.isActive == 0) ? false : true,
                                IsDocumentAvailable = alotment.isDocAvailable
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public KYAViewModel GetKYADetails(int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                KYAViewModel kyaDetails = new KYAViewModel();
                var details = dbContext.KYADetails.FirstOrDefault(r => r.RId == rId && r.IsActive == true);
                if (details != null)
                {
                    kyaDetails.RegistrationId = details.RId;
                    kyaDetails.AllotteeName = details.AllotteeName;
                    kyaDetails.AllotteeType = details.AllotteeType;
                    kyaDetails.KYAuid = details.KYAuid;
                    kyaDetails.KYAReferenceCode = details.KYAReferenceCode;
                    kyaDetails.SectorId = details.SectorId;
                    kyaDetails.BlockId = details.BlockId;
                    kyaDetails.Sector = details.SectorId != null ? dbContext.SectorMsts.FirstOrDefault(s => s.sectorId == details.SectorId).sectorName : Constants.NA;
                    kyaDetails.Block = details.BlockId != null ? dbContext.BlockMsts.FirstOrDefault(b => b.blockId == details.BlockId).blockName : Constants.NA;
                    kyaDetails.PlotNo = details.PlotNo;
                    kyaDetails.MobileNo = details.MobileNo;
                    kyaDetails.Email = details.Email;
                    kyaDetails.GSTNo = details.GSTNo;
                    kyaDetails.PAN = details.PAN;
                    kyaDetails.SignatoryMobileNo = details.SignatoryMobileNo;
                    kyaDetails.SignatoryEmail = details.SignatoryEmail;
                    kyaDetails.AuthorizedSignatory = details.AuthorizedSignatory;
                    kyaDetails.ActionId = details.ActionId;
                    kyaDetails.OptionalAction = details.OptionalAction;
                    kyaDetails.Remarks = details.Remarks;
                    kyaDetails.StatusId = details.StatusId;
                    kyaDetails.Status = details.StatusId != null ? dbContext.StatusMasters.FirstOrDefault(s => s.Id == details.StatusId).Status : string.Empty;
                    kyaDetails.CorrespondAddress = details.CorrespondAddress;
                    kyaDetails.ApprovalDate = details.ApprovalDate;
                    kyaDetails.CreatedDate = details.CreatedDate;
                    kyaDetails.Approver = details.ApproverId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == details.ApproverId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == details.ApproverId).LastName) : string.Empty;
                    kyaDetails.CommunicationAddress = (string.IsNullOrEmpty(details.AddressLine1) ? string.Empty : details.AddressLine1 + ",")
                        + (string.IsNullOrEmpty(details.AddressLine2) ? string.Empty : details.AddressLine2 + ",")
                        + (string.IsNullOrEmpty(details.AreaLocality) ? string.Empty : details.AreaLocality + ",")
                        + (string.IsNullOrEmpty(details.City) ? string.Empty : details.City + ",")
                        + (string.IsNullOrEmpty(details.State) ? string.Empty : details.State + ",")
                        + (string.IsNullOrEmpty(details.PinCode) ? string.Empty : "Pincode-" + details.PinCode);
                }
                return kyaDetails;
            }
        }


        public DataSourceResult GetDocumentTypeListByDepartmentId(DataSourceRequest request, int deptId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from temp in dbContext.TemplateMasters
                            where temp.departmentId == deptId
                            select new DropdownViewModel
                            {
                                Id = temp.templateId.Value,
                                Text = temp.templateName,
                                Value = temp.templateName
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public int SaveUploadedDocument(PropertyDocument model, HttpPostedFileBase docfile)
        {
            int flag = ReturnType.None;
            bool bflag = false;
            if (docfile != null)
            {
                string extension = Path.GetExtension(docfile.FileName);
                string docName = model.DocumentType;
                if (docName.Contains(" "))
                {
                    docName = docName.Replace(" ", "");
                }
                string filename = docName + extension;
                bflag = FtpHandler.UploadFileByName(docfile, filename, model.RID.ToString(), Constants.Authority);
                flag = bflag == true ? ReturnType.Saved : ReturnType.Failed;
            }
            return flag;
        }
    }
}
