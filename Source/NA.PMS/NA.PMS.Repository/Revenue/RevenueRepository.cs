using Kendo.Mvc.UI;
using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Kendo.Mvc.Extensions;
using System.Data.Entity;
using NA.PMS.Dal;
using NA.PMS.Model.Entities;
using NA.PMS.Dal.DBConnection;
using Dapper;

namespace NA.PMS.Repository
{
    public class RevenueRepository : IRevenueRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public RevenueRepository()
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

        public int AddPaymentDetailTemp(string rid, int? receiptId, int? headId, int? subHeadId, int? bankId, decimal? amount)
        {
            List<PaymentViewModel> TempModel = (List<PaymentViewModel>)HttpContext.Current.Session["PaymentModel"];
            if (TempModel == null)
            {
                TempModel = new List<PaymentViewModel>();
                TempModel.Add(new PaymentViewModel { RegistrationNo = rid, ReceiptId = receiptId, ReceiptHeadId = headId, ReceiptSubHeadId = subHeadId, Amount = amount, BankId = bankId, ReceiptCode = 0 });
            }
            else
            {
                int count = TempModel.Count;
                TempModel.Add(new PaymentViewModel { RegistrationNo = rid, ReceiptId = receiptId, ReceiptHeadId = headId, ReceiptSubHeadId = subHeadId, Amount = amount, BankId = bankId, ReceiptCode = count });
            }
            HttpContext.Current.Session["PaymentModel"] = TempModel;
            return ReturnType.Success;
        }

        #region Lease Deed Date

        public int SaveLeaseRentPaymentByRegistrationId(LeaseRentViewModel model)
        {
            int flag = ReturnType.None;

            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.RentId > 0)
                {
                    var existingRent = dbContext.LeaseRentPayments.FirstOrDefault(r => r.Id == model.RentId);
                    //In case of Edit
                    if (existingRent != null)
                    {
                        flag = UpdateLeaseRentPaymentById(model);
                    }
                    else { flag = ReturnType.NotExist; }
                }
                else
                {
                    var IsLeaseDeed = dbContext.LeaseRentPayments.FirstOrDefault(r => r.RegistrationId == model.RegistrationId);
                    //In case of new entry
                    if (IsLeaseDeed == null)
                    {
                        LeaseRentPayment leaserent = new LeaseRentPayment();
                        //leaserent = MapLeaseModel(LeaseDeed, ObjLeaseDeedModel);//Map data
                        leaserent.RegistrationId = model.RegistrationId;
                        leaserent.DepartmentId = model.DepartmentId;
                        leaserent.PropertyId = model.PropertyId;
                        leaserent.Sector = model.Sector;
                        leaserent.Block = model.Block;
                        leaserent.PlotNo = model.PlotNo;
                        leaserent.Applicant = model.Applicant;
                        leaserent.LeaseDeedDate = model.LeaseDeedDate;
                        leaserent.TransferLeaseDate = model.TransferLeaseDate;
                        leaserent.PremiumLeaseRent = model.LeaseRentPremium;
                        leaserent.RevisedPremium = model.RevisedPremium;
                        leaserent.RevisedPremiumDate = model.RevisedDate;
                        leaserent.PanelInterest = model.PanelInterest;
                        leaserent.RevisedRate = model.RevisedRate;
                        leaserent.GST = model.GstAmount;
                        //leaserent.GST = model.GST;
                        leaserent.TotalPremium = model.TotalPremium;
                        leaserent.ChallanDate = model.ChallanDate;
                        leaserent.NDCDate = model.NDCDate;
                        //leaserent.NDCStatus = model.NDCStatus;
                        leaserent.DepositDate = model.DepositDate;
                        leaserent.IsOneTimeLeasePaid = model.OneTimePaidStatus == "Y" ? true : false;
                        leaserent.IsTotalPremiumPaid = model.TotalPremiumPaidStatus == "Y" ? true : false;
                        leaserent.PremiumPaidDuration = model.PremiumPaidDuration;
                        leaserent.PremiumPaidUptoDate = model.PaidUptoDate;
                        leaserent.IsOneTimeInstallmentPaid = model.IsOneTimeInstallmentPaid;
                        leaserent.IsTotalInstallmentPaid = model.TotalPremiumPaidStatus == "Y" ? true : false;
                        leaserent.PaymentMode = model.PaymentMode;
                        leaserent.TransactionId = model.TransactionId;
                        leaserent.BalanceAmount = model.BalanceAmount;
                        leaserent.BalanceInterest = model.BalanceInterest;
                        leaserent.BalanceUptoDate = model.BalanceUptoDate;
                        leaserent.ReviseRateAfterYear = model.RevisedRateAfterYear;
                        leaserent.PreviousBalance = model.PreviousBalance;
                        leaserent.ChallanId = model.ChallanId;
                        leaserent.EntryDate = DateTime.Now;
                        leaserent.StatusId = NAStatusId.InProgress;
                        leaserent.IsActive = true;
                        leaserent.CreatedBy = userInfo.UserID.ToString();
                        leaserent.CreatedOn = DateTime.Now;
                        dbContext.LeaseRentPayments.Add(leaserent);
                        dbContext.SaveChanges();

                        //if (model.OneTimePaidStatus == Constants.Y && model.ChallanDate != null)
                        //{
                        var ExNDC = dbContext.PRE_FULL_PAYMENT_NDC.FirstOrDefault(n => n.RegistrationId == model.RegistrationId && n.IsActive == true);
                        if (ExNDC == null)
                        {
                            var ndc = new PRE_FULL_PAYMENT_NDC();
                            ndc.RegistrationId = model.RegistrationId;
                            ndc.PropertyNo = model.Sector + "/" + model.Block + "-" + model.PlotNo;
                            ndc.TotalPaidPream = model.TotalPremiumPaidStatus;
                            ndc.OneTimeLease = model.OneTimeLeasePaidStatus;
                            ndc.Status = NAStatusId.Initiated.ToString();
                            ndc.IsActive = true;
                            ndc.NDCDate = model.NDCDate;
                            ndc.EntryDate = DateTime.Now;
                            ndc.CreatedBy = userInfo.UserID.ToString();
                            ndc.CreatedOn = DateTime.Now;
                            dbContext.PRE_FULL_PAYMENT_NDC.Add(ndc);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            ExNDC.TotalPaidPream = model.TotalPremiumPaidStatus;
                            ExNDC.OneTimeLease = model.OneTimeLeasePaidStatus;
                            ExNDC.Status = NAStatusId.Initiated.ToString();
                            ExNDC.IsActive = true;
                            ExNDC.NDCDate = model.NDCDate;
                            dbContext.SaveChanges();
                        }
                        //}
                        flag = ReturnType.Saved;
                    }
                    else { flag = ReturnType.Exist; }
                }

            }
            return flag;
        }

        private int UpdateLeaseRentPaymentById(LeaseRentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingRent = dbContext.LeaseRentPayments.FirstOrDefault(r => r.Id == model.RentId);
                if (existingRent != null)
                {
                    existingRent.RegistrationId = model.RegistrationId;
                    existingRent.DepartmentId = model.DepartmentId;
                    existingRent.PropertyId = model.PropertyId;
                    existingRent.Sector = model.Sector;
                    existingRent.Block = model.Block;
                    existingRent.PlotNo = model.PlotNo;
                    existingRent.Applicant = model.Applicant;
                    existingRent.LeaseDeedDate = model.LeaseDeedDate;
                    existingRent.TransferLeaseDate = model.TransferLeaseDate;
                    existingRent.PremiumLeaseRent = model.LeaseRentPremium;
                    existingRent.RevisedPremium = model.RevisedPremium;
                    existingRent.RevisedPremiumDate = model.RevisedDate;
                    existingRent.PanelInterest = model.PanelInterest;
                    existingRent.RevisedRate = model.RevisedRate;
                    existingRent.GST = model.GstAmount;
                    //existingRent.GST = model.GST;
                    existingRent.TotalPremium = model.TotalPremium;
                    existingRent.ChallanDate = model.ChallanDate;
                    existingRent.NDCDate = model.NDCDate;
                    //existingRent.NDCStatus = model.NDCStatus;
                    existingRent.DepositDate = model.DepositDate;
                    existingRent.IsOneTimeLeasePaid = model.IsOneTimeLeaseRentPaid;
                    existingRent.IsTotalPremiumPaid = model.IsLeaseRentPaid;
                    existingRent.PremiumPaidDuration = model.PremiumPaidDuration;
                    existingRent.PremiumPaidUptoDate = model.PaidUptoDate;
                    existingRent.IsOneTimeInstallmentPaid = model.IsOneTimeInstallmentPaid;
                    existingRent.IsTotalInstallmentPaid = model.IsTotalInstallmentPaid;
                    existingRent.PaymentMode = model.PaymentMode;
                    existingRent.TransactionId = model.TransactionId;
                    existingRent.BalanceAmount = model.BalanceAmount;
                    existingRent.BalanceInterest = model.BalanceInterest;
                    existingRent.BalanceUptoDate = model.BalanceUptoDate;
                    existingRent.EntryDate = DateTime.Now;
                    existingRent.StatusId = NAStatusId.InProgress;
                    existingRent.IsActive = false;
                    //existingRent.CreatedBy = userInfo.UserID.ToString();
                    //existingRent.CreatedOn = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }

        private LeaseRentPayment MapLeaseModel(LeaseRentPayment ndc, LeaseDeedModel ObjLeaseDeedModel)
        {
            ndc.RegistrationId = Convert.ToInt32(ObjLeaseDeedModel.RegistrationId);

            ndc.Applicant = ObjLeaseDeedModel.Applicant;
            ndc.Sector = ObjLeaseDeedModel.Sector;
            ndc.Block = ObjLeaseDeedModel.Block;
            ndc.PlotNo = ObjLeaseDeedModel.PlotNo;

            ndc.IsTotalPremiumPaid = ObjLeaseDeedModel.IsPremiumPaid == NA.PMS.Common.NDCOptions.Id_Yes ? true : false;
            ndc.IsOneTimeLeasePaid = ObjLeaseDeedModel.IsLeaseRentPaid == NA.PMS.Common.NDCOptions.Id_Yes ? true : false;

            //one time lease rent paid is yes
            if (ObjLeaseDeedModel.IsLeaseRentPaid == NA.PMS.Common.NDCOptions.Id_Yes)
            {
                ndc.NDCDate = ObjLeaseDeedModel.NDCDate;
                ndc.ChallanDate = ObjLeaseDeedModel.OTRChallanDate;
            }

            //one time lease rent paid is no
            if (ObjLeaseDeedModel.IsLeaseRentPaid == NA.PMS.Common.NDCOptions.Id_No)
            {
                ndc.LeaseDeedDate = ObjLeaseDeedModel.LeaseDeedDate;
                ndc.TransferLeaseDate = ObjLeaseDeedModel.LeaseTransferDate;
                ndc.PremiumLeaseRent = ObjLeaseDeedModel.LeaseRentPerYear;
                ndc.PanelInterest = ObjLeaseDeedModel.PanelInterest;
                ndc.PremiumPaidUptoDate = ObjLeaseDeedModel.LeaseRentPaidUpto;
                ndc.PremiumPaidDuration = ObjLeaseDeedModel.LastPaidUPO;
                ndc.RevisedPremiumDate = ObjLeaseDeedModel.LeaseRentRevisedDate;
                ndc.BalanceAmount = ObjLeaseDeedModel.BalanceAmount;
                ndc.BalanceInterest = ObjLeaseDeedModel.BalanceInterest;
                ndc.GST = ObjLeaseDeedModel.GstInterestPart;
                ndc.BalanceUptoDate = ObjLeaseDeedModel.BalanceDate;
                ndc.RevisedRate = ObjLeaseDeedModel.RevisedRatePerYear;
            }
            ndc.Approver = (!string.IsNullOrEmpty(ObjLeaseDeedModel.User)) ? Convert.ToInt32(ObjLeaseDeedModel.User) : 0;
            return ndc;
        }

        public DataSourceResult GetLeaseRentDuesDetail(DataSourceRequest request, LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                bool IsOTLRP = model.IsLeaseRentPaid == true ? true : false;
                var list = (from leaserent in dbcontext.LeaseRentPayments
                            join alotment in dbcontext.AllotmentMasters on leaserent.RegistrationId equals alotment.rid
                            join property in dbcontext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where (model.DepartmentId == null || alotment.departmentId == model.DepartmentId) && leaserent.IsOneTimeLeasePaid == model.IsLeaseRentPaid
                            select new LeaseRentViewModel
                            {
                                Id = leaserent.Id,
                                RentId = leaserent.Id,
                                RegistrationId = leaserent.RegistrationId,
                                PropertyNo = ((leaserent.Sector != null ? leaserent.Sector : "--") + "/" + (leaserent.Block != null ? leaserent.Block : "--") + "/" + (leaserent.PlotNo != null ? leaserent.PlotNo : "--")),
                                LeaseDeedDate = leaserent.LeaseDeedDate,
                                LeaseRentPremium = leaserent.PremiumLeaseRent != null ? leaserent.PremiumLeaseRent : 0,
                                PanelInterest = leaserent.PanelInterest != null ? leaserent.PanelInterest : 0,
                                PaidUptoDate = leaserent.PremiumPaidUptoDate,
                                LastPaidUPO = leaserent.PremiumPaidDuration,
                                RevisedDate = leaserent.RevisedPremiumDate,
                                BalanceAmount = leaserent.BalanceAmount != null ? leaserent.BalanceAmount : 0,
                                BalanceInterest = leaserent.BalanceInterest != null ? leaserent.BalanceInterest : 0,
                                GstAmount = leaserent.GST != null ? leaserent.GST : 0,
                                TotalBalance = leaserent.BalanceAmount != null ? leaserent.BalanceAmount : 0 + leaserent.BalanceInterest != null ? leaserent.BalanceInterest : 0 + leaserent.GST != null ? leaserent.GST : 0,
                                BalanceUptoDate = leaserent.BalanceUptoDate,
                                RevisedRate = leaserent.RevisedRate,
                                IsLeaseRentPaid = leaserent.IsOneTimeLeasePaid,
                                PremiumPaidStatus = leaserent.IsOneTimeLeasePaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                                IsTotalLeaseRentPaid = leaserent.IsTotalPremiumPaid,
                                TotalPremiumPaidStatus = leaserent.IsTotalPremiumPaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                                NDCDate = leaserent.NDCDate,
                                ChallanDate = leaserent.ChallanDate,
                                StatusId = leaserent.StatusId,
                                Status = leaserent.StatusId > 0 ? (from status in dbcontext.StatusMasters where status.Id == leaserent.StatusId select status.Status).FirstOrDefault() : string.Empty,
                                IsActive = leaserent.IsActive,
                            }).ToList();
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetLeaseRentDuesRequest(DataSourceRequest request)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var list = (from leasedeed in dbcontext.LeaseRentPayments
                            where leasedeed.Approver == userInfo.UserID
                            select new LeaseRentViewModel
                            {
                                Id = leasedeed.Id,
                                RentId = leasedeed.Id,
                                RegistrationId = leasedeed.RegistrationId,
                                Sector = leasedeed.Sector,
                                Block = leasedeed.Block,
                                PlotNo = leasedeed.PlotNo,
                                LeaseDeedDate = leasedeed.LeaseDeedDate,
                                LeaseRentPremium = leasedeed.PremiumLeaseRent,
                                PanelInterest = leasedeed.PanelInterest,
                                PaidUptoDate = leasedeed.PremiumPaidUptoDate,
                                LastPaidUPO = leasedeed.PremiumPaidDuration,
                                RevisedDate = leasedeed.RevisedPremiumDate,
                                BalanceAmount = leasedeed.BalanceAmount,
                                BalanceInterest = leasedeed.BalanceInterest,
                                GstAmount = leasedeed.GST,
                                BalanceUptoDate = leasedeed.BalanceUptoDate,
                                RevisedRate = leasedeed.RevisedRate,
                                IsOneTimeLeaseRentPaid = leasedeed.IsOneTimeLeasePaid,
                                OneTimePaidStatus = leasedeed.IsOneTimeLeasePaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                                IsOneTimeInstallmentPaid = leasedeed.IsTotalPremiumPaid,
                                OneTimePremiumPaidStatus = leasedeed.IsTotalPremiumPaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                                NDCDate = leasedeed.NDCDate,
                                ChallanDate = leasedeed.ChallanDate,
                                StatusId = leasedeed.StatusId,
                                Status = leasedeed.StatusId > 0 ? (from status in dbcontext.StatusMasters where status.Id == leasedeed.StatusId select status.Status).FirstOrDefault() : string.Empty,
                                IsActive = leasedeed.IsActive,
                                UserCreatedBy = dbcontext.UmUserMasters.Where(x => x.UserRefId.ToString() == leasedeed.CreatedBy).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault(),
                                CreatedBy = leasedeed.CreatedBy,
                                CreatedDate = leasedeed.CreatedOn,
                                ApprovalDate = leasedeed.ApprovalDate,
                                Approver = dbcontext.UmUserMasters.Where(x => x.UserRefId == leasedeed.Approver).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault(),
                                ApproverId = leasedeed.Approver,
                                CommentDate = leasedeed.CommentDate
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public LeaseRentViewModel GetLeaseRentPaymentDetailById(int Id)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var detail = (from leasedeed in dbcontext.LeaseRentPayments
                              where leasedeed.Id == Id
                              select new LeaseRentViewModel
                              {
                                  Id = leasedeed.Id,
                                  RentId = leasedeed.Id,
                                  RegistrationId = leasedeed.RegistrationId,
                                  Applicant = leasedeed.Applicant,
                                  Sector = leasedeed.Sector,
                                  Block = leasedeed.Block,
                                  PlotNo = leasedeed.PlotNo,
                                  LeaseDeedDate = leasedeed.LeaseDeedDate,
                                  TransferLeaseDate = leasedeed.TransferLeaseDate,
                                  LeaseRentPremium = leasedeed.PremiumLeaseRent,
                                  PanelInterest = leasedeed.PanelInterest,
                                  PaidUptoDate = leasedeed.PremiumPaidUptoDate,
                                  LastPaidUPO = leasedeed.PremiumPaidDuration,
                                  RevisedDate = leasedeed.RevisedPremiumDate,
                                  BalanceAmount = leasedeed.BalanceAmount,
                                  BalanceInterest = leasedeed.BalanceInterest,
                                  GstAmount = leasedeed.GST,
                                  BalanceUptoDate = leasedeed.BalanceUptoDate,
                                  RevisedRate = leasedeed.RevisedRate,
                                  IsLeaseRentPaid = leasedeed.IsOneTimeLeasePaid,
                                  PremiumPaidStatus = leasedeed.IsOneTimeLeasePaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                                  IsTotalInstallmentPaid = leasedeed.IsTotalPremiumPaid,
                                  TotalPremiumPaidStatus = leasedeed.IsTotalPremiumPaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                                  NDCDate = leasedeed.NDCDate,
                                  ChallanDate = leasedeed.ChallanDate,
                                  StatusId = leasedeed.StatusId,
                                  ApproverId = leasedeed.Approver,
                                  Approver = leasedeed.Approver > 0 ? dbcontext.UmUserMasters.Where(x => x.UserRefId == leasedeed.Approver).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault() : string.Empty,
                                  IsActive = leasedeed.IsActive,
                                  Comment = leasedeed.Comment,
                                  ApprovalDate = leasedeed.ApprovalDate,
                                  Status = leasedeed.StatusId > 0 ? (from status in dbcontext.StatusMasters where status.Id == leasedeed.StatusId select status.Status).FirstOrDefault() : string.Empty,
                              }).FirstOrDefault();
                return detail;
            }
        }

        public int SaveLeaseRentApprovalStatus(LeaseRentViewModel model)
        {
            var Flag = ReturnType.None;
            using (var dbcontext = new NoidaPMSEntities())
            {
                var data = dbcontext.LeaseRentPayments.FirstOrDefault(m => m.Id == model.Id);
                if (model.ActionType == "ForwardForApproval")
                {
                    data.StatusId = NAStatusId.InProgress;//In Progress
                    data.Approver = model.ApproverId;
                    dbcontext.SaveChanges();
                    Flag = ReturnType.Forwarded;
                }
                else
                {
                    string Comment = "Comments: " + model.ApproverComments + " by " + " " + userInfo.UserName + " on " + DateTime.Now;
                    data.StatusId = model.StatusId;
                    if (model.StatusId == NAStatusId.Approved)
                    {
                        data.IsActive = true;
                        data.ApprovalDate = DateTime.Now;
                    }

                    data.Comment = string.IsNullOrEmpty(model.ApproverComments) ? Comment : data.Comment + "\n " + Comment;
                    data.CommentDate = DateTime.Now;
                    dbcontext.SaveChanges();
                    Flag = ReturnType.Updated;
                }
            }
            return Flag;
        }

        public int LeaseDeedResendforApproval(LeaseDeedModel ObjLeaseDeedModel)
        {
            var Flag = ReturnType.None;
            using (var dbcontext = new NoidaPMSEntities())
            {
                var data = dbcontext.LeaseRentPayments.FirstOrDefault(m => m.Id == ObjLeaseDeedModel.LeaseDeedId);
                if (data != null)
                {
                    data.StatusId = (from status in dbcontext.StatusMasters where status.Id == Constants.InProgress select status.Id).FirstOrDefault();//In Progress
                    data.Approver = (!string.IsNullOrEmpty(ObjLeaseDeedModel.User)) ? Convert.ToInt32(ObjLeaseDeedModel.User) : 0;
                    dbcontext.SaveChanges();
                    Flag = ReturnType.Updated;
                }
            }
            return Flag;
        }

        #endregion



        public DataSourceResult GetLeaseRentDetails(DataSourceRequest request, LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                bool IsOTLRP = model.PremiumPaidStatus == NA.PMS.Common.NDCOptions.Id_Yes ? true : false;
                var list = (from rentmaster in dbcontext.LeaseRentPayments
                            //join alotment in dbcontext.AllotmentMasters on rentmaster.RegistrationId equals alotment.rid
                            //join property in dbcontext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where model.DepartmentId == null || rentmaster.DepartmentId == model.DepartmentId //&& leaserent.IsOneTimeLeasePaid == IsOTLRP
                            select new LeaseRentViewModel
                            {
                                Id = rentmaster.Id,
                                RentId = rentmaster.Id,
                                RegistrationId = rentmaster.RegistrationId,
                                Sector = rentmaster.Sector,
                                Block = rentmaster.Block,
                                PlotNo = rentmaster.PlotNo,
                                //PropertyNo = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "/" + property.propertyNo,
                                LeaseDeedDate = rentmaster.LeaseDeedDate,
                                TransferLeaseDate = rentmaster.TransferLeaseDate,
                                LeaseRentPremium = rentmaster.PremiumLeaseRent,
                                RevisedPremium = rentmaster.RevisedPremium,
                                RevisedDate = rentmaster.RevisedPremiumDate,
                                PanelInterest = rentmaster.PanelInterest,
                                NDCDate = rentmaster.NDCDate,
                                IsOneTimeLeaseRentPaid = rentmaster.IsOneTimeLeasePaid,
                                IsTotalLeaseRentPaid = rentmaster.IsTotalPremiumPaid,
                                IsLeaseRentPaid = rentmaster.IsOneTimeLeasePaid,
                                LeaseRentStatus = rentmaster.IsOneTimeLeasePaid == true ? "Paid" : "Not Paid",
                                PremiumPaidDuration = rentmaster.PremiumPaidDuration,
                                PaidUptoDate = rentmaster.PremiumPaidUptoDate,
                                IsTotalInstallmentPaid = rentmaster.IsTotalInstallmentPaid,
                                IsOneTimeInstallmentPaid = rentmaster.IsOneTimeInstallmentPaid,
                                IsNDCGenerated = (rentmaster.IsOneTimeLeasePaid == true && rentmaster.IsTotalInstallmentPaid == true) ? true : false,
                                PremiumPaidStatus = (rentmaster.IsOneTimeInstallmentPaid == true || rentmaster.IsTotalInstallmentPaid == true) ? "Paid" : "Not Paid",
                                BalanceAmount = rentmaster.BalanceAmount,
                                BalanceInterest = rentmaster.BalanceInterest,
                                BalanceUptoDate = rentmaster.BalanceUptoDate,
                                LeaseRentDues = rentmaster.LeaseRentDues,
                                DuesUptoDate = rentmaster.DuesUptoDate,
                                TotalBalance = rentmaster.BalanceAmount,
                                PreviousDues = rentmaster.PreviousDues,
                                PreviousDuesDate = rentmaster.PreviousDuesDate,
                                CurrentDues = rentmaster.CurrentDues,
                                CurrentDuesDate = rentmaster.CurrentDuesDate,
                                ChallanDate = rentmaster.ChallanDate,
                                StatusId = rentmaster.StatusId,
                                Status = (rentmaster.StatusId != null || rentmaster.StatusId > 0) ? (from status in dbcontext.StatusMasters where status.Id == rentmaster.StatusId select status.Status).FirstOrDefault() : string.Empty,
                                IsActive = rentmaster.IsActive
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public LeaseRentViewModel GetPaymentLeaseRentDetailsById(int rid)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                LeaseRentViewModel model = new LeaseRentViewModel();
                model = (from rentmaster in dbcontext.LeaseRentPayments
                         join leaserent in dbcontext.PaymentLeaseRentTrans on rentmaster.Id equals leaserent.RentId
                         join alotment in dbcontext.AllotmentMasters on rentmaster.RegistrationId equals alotment.rid
                         //join property in dbcontext.SchemePropTrans on alotment.propertyId equals property.propertyId
                         where rentmaster.RegistrationId == rid
                         select new LeaseRentViewModel
                         {
                             Id = rentmaster.Id,
                             RentId = leaserent.RentId,
                             RegistrationId = rentmaster.RegistrationId,
                             Applicant = rentmaster.Applicant,
                             Sector = rentmaster.Sector,
                             Block = rentmaster.Block,
                             PlotNo = rentmaster.PlotNo,
                             LeaseDeedDate = rentmaster.LeaseDeedDate,
                             TransferLeaseDate = rentmaster.TransferLeaseDate,
                             LeaseRentPremium = leaserent.LeaseRentPremium,
                             //PanelInterest = leaserent.PenalInterest,
                             //PaidUptoDate = leaserent.PremiumPaidUptoDate,
                             //PremiumPaidDuration = leaserent.PremiumPaidDuration,
                             //RevisedDate = leaserent.RevisedPremiumDate,
                             //BalanceAmount = leaserent.BalanceAmount,
                             //BalanceInterest = leaserent.BalanceInterest,
                             //GST = leaserent.GST,
                             //BalanceUptoDate = leaserent.BalanceUptoDate,
                             //RevisedPremium = leaserent.RevisedRate,
                             IsOneTimeLeaseRentPaid = rentmaster.IsOneTimeLeasePaid,
                             IsTotalLeaseRentPaid = rentmaster.IsTotalPremiumPaid,
                             OneTimePaidStatus = rentmaster.IsOneTimeLeasePaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                             PremiumPaidStatus = rentmaster.IsTotalPremiumPaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                             NDCDate = rentmaster.NDCDate,
                             //ChallanDate = leaserent.ChallanDate,
                             StatusId = leaserent.StatusId,
                             ApproverId = leaserent.Approver,
                             Approver = leaserent.Approver > 0 ? dbcontext.UmUserMasters.Where(x => x.UserRefId == leaserent.Approver).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault() : string.Empty,
                             IsActive = leaserent.IsActive,
                             Comment = leaserent.Comment,
                             ApprovalDate = leaserent.ApprovalDate,
                             Status = leaserent.StatusId > 0 ? (from status in dbcontext.StatusMasters where status.Id == leaserent.StatusId select status.Status).FirstOrDefault() : string.Empty,
                             HtmlDuesReport = leaserent.HtmlDuesTemplate
                         }).FirstOrDefault();
                return model;
            }
        }


        public LeaseRentViewModel GetApplicantDetailsByRegistrationId(int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var usr = (from alotment in dbContext.AllotmentMasters
                           join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                           join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                           where alotment.rid == rid
                           select new LeaseRentViewModel
                           {
                               RegistrationId = alotment.rid,
                               DepartmentId = alotment.departmentId,
                               PropertyId = alotment.propertyId,
                               Applicant = aplicant.tGender.ToLower() == Constants.Company.ToLower() ? aplicant.T_Company_Name : aplicant.tFirstName + " " + ((aplicant.tMiddleName == null) ? "" : aplicant.tMiddleName + " ") + aplicant.tLastName,
                               MobileNo = aplicant.tMobileNumber,
                               Email = aplicant.tEmail,
                               Sector = property.SectorMst.sectorName,
                               Block = property.BlockMst.blockName,
                               PlotNo = property.propertyNo,
                               Address = aplicant.tCorrespondanceAdd,
                               PropertyNo = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo,
                               RegistryDate = alotment.RegistryDetails.Where(m => m.Rid == rid).FirstOrDefault() != null ? alotment.RegistryDetails.Where(m => m.Rid == rid).FirstOrDefault().RegistryDoneDate : null

                           }).FirstOrDefault();
                var leaserent = dbContext.LeaseRentPayments.FirstOrDefault(l => l.RegistrationId == rid);
                if (leaserent != null)
                {
                    usr.LeaseRentPerAnnum = leaserent.PremiumLeaseRent;
                    usr.RevisedPremium = leaserent.RevisedPremium;
                    usr.RevisedDate = leaserent.RevisedPremiumDate;
                    usr.RevisedRate = leaserent.RevisedRate;
                    usr.PanelInterest = leaserent.PanelInterest;
                    usr.IsOneTimeLeaseRentPaid = leaserent.IsOneTimeLeasePaid;
                    usr.IsTotalInstallmentPaid = leaserent.IsTotalInstallmentPaid;
                    usr.PremiumPaidDuration = leaserent.PremiumPaidDuration;
                    usr.BalanceAmount = leaserent.BalanceAmount;
                    usr.BalanceInterest = leaserent.BalanceInterest;
                    usr.CurrentDues = leaserent.CurrentDues;
                    usr.CurrentDuesDate = leaserent.CurrentDuesDate;
                    usr.LeaseRentDues = leaserent.LeaseRentDues;
                    usr.DuesUptoDate = leaserent.DuesUptoDate;
                }
                return usr;
            }
        }

        public int SaveLeaseRentPremium(LeaseRentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var masterdata = dbContext.LeaseRentPayments.FirstOrDefault(m => m.RegistrationId == model.RegistrationId);
                if (masterdata == null)
                {
                    LeaseRentPayment rentmaster = new LeaseRentPayment();
                    rentmaster.RegistrationId = model.RegistrationId;
                    rentmaster.DepartmentId = model.DepartmentId;
                    rentmaster.PropertyId = model.PropertyId;
                    rentmaster.Applicant = model.Applicant;
                    rentmaster.Sector = model.Sector;
                    rentmaster.Block = model.Block;
                    rentmaster.PlotNo = model.PlotNo;
                    rentmaster.LeaseDeedDate = model.LeaseDeedDate;
                    rentmaster.TransferLeaseDate = model.TransferLeaseDate;
                    rentmaster.PremiumLeaseRent = model.LeaseRentPerAnnum; // model.LeaseRentPremium;
                    rentmaster.PanelInterest = model.PanelInterest;
                    rentmaster.RevisedRate = model.RevisedRate;
                    rentmaster.RevisedPremium = model.RevisedLeaseRentPerAnnum; // model.RevisedPremium;
                    rentmaster.RevisedPremiumDate = model.RevisedDate;
                    rentmaster.PremiumPaidDuration = model.PremiumPaidDuration;
                    rentmaster.PremiumPaidUptoDate = model.PaidUptoDate;
                    rentmaster.GST = model.GST;
                    rentmaster.NDCDate = model.NDCDate;
                    rentmaster.ChallanDate = model.ChallanDate;
                    rentmaster.BalanceAmount = model.BalanceAmount;
                    rentmaster.StatusId = model.StatusId != null ? model.StatusId : StatusOption.InProgress;
                    rentmaster.NDCStatus = model.NDCDate != null ? Status.Approved : Status.InProgress;
                    rentmaster.IsTotalInstallmentPaid = model.TotalPremiumPaidStatus == "Y" ? true : false;
                    rentmaster.IsOneTimeLeasePaid = model.OneTimeLeasePaidStatus == "Y" ? true : false;
                    rentmaster.TotalPremium = model.OneTimeLeasePaidStatus == "Y" ? model.TotalLeaseRent : (model.LeaseRentPremium + (model.GST == null ? 0 : model.GST));
                    rentmaster.IsActive = true;
                    rentmaster.Comment = model.Comment;
                    rentmaster.CreatedOn = DateTime.Now;
                    rentmaster.CreatedBy = userInfo.UserID.ToString();
                    dbContext.LeaseRentPayments.Add(rentmaster);
                    dbContext.SaveChanges();

                    //model.Id = rentmaster.Id;
                    //model.RentId = rentmaster.Id;

                    //flag = SaveMultipleLeaseRentPremium(model);

                    flag = ReturnType.Saved;
                }
                else
                {
                    //model.RentId = masterdata.Id;
                    //flag = SaveMultipleLeaseRentPremium(model);
                    flag = UpdateLeaseRentPremium(model);
                }
            }
            return flag;
        }

        public int UpdateLeaseRentPremium(LeaseRentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var rentmaster = dbContext.LeaseRentPayments.FirstOrDefault(m => m.RegistrationId == model.RegistrationId);
                //rentmaster.RegistrationId = model.RegistrationId;
                rentmaster.DepartmentId = model.DepartmentId;
                rentmaster.PropertyId = model.PropertyId;
                rentmaster.Applicant = model.Applicant;
                rentmaster.Sector = model.Sector;
                rentmaster.Block = model.Block;
                rentmaster.PlotNo = model.PlotNo;
                rentmaster.LeaseDeedDate = model.LeaseDeedDate;
                rentmaster.TransferLeaseDate = model.TransferLeaseDate;
                rentmaster.PremiumLeaseRent = model.LeaseRentPerAnnum; // model.LeaseRentPremium;
                rentmaster.PanelInterest = model.PanelInterest;
                rentmaster.RevisedRate = model.RevisedRate;
                rentmaster.RevisedPremium = model.RevisedLeaseRentPerAnnum; // model.RevisedPremium;
                rentmaster.RevisedPremiumDate = model.RevisedDate;
                rentmaster.PremiumPaidDuration = model.PremiumPaidDuration;
                rentmaster.PremiumPaidUptoDate = model.PaidUptoDate;
                rentmaster.GST = model.GST;
                rentmaster.NDCDate = model.NDCDate;
                rentmaster.ChallanDate = model.ChallanDate;
                rentmaster.BalanceAmount = model.BalanceAmount;
                rentmaster.StatusId = model.StatusId != null ? model.StatusId : StatusOption.InProgress;
                rentmaster.NDCStatus = model.NDCDate != null ? Status.Approved : Status.InProgress;
                rentmaster.IsTotalInstallmentPaid = model.TotalPremiumPaidStatus == "Y" ? true : false;
                rentmaster.IsOneTimeLeasePaid = model.OneTimeLeasePaidStatus == "Y" ? true : false;
                rentmaster.TotalPremium = model.OneTimeLeasePaidStatus == "Y" ? model.TotalLeaseRent : (model.LeaseRentPremium + (model.GST == null ? 0 : model.GST));
                rentmaster.IsActive = true;
                rentmaster.Comment = model.Comment;
                rentmaster.CreatedOn = DateTime.Now;
                rentmaster.CreatedBy = userInfo.UserID.ToString();
                dbContext.SaveChanges();
                flag = ReturnType.Updated;
            }
            return flag;
        }

        private int SaveMultipleLeaseRentPremium(LeaseRentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                LeaseRentPaymentTran leaserent = new LeaseRentPaymentTran();
                leaserent.RefId = model.RentId;
                leaserent.RegistrationId = model.RegistrationId;
                leaserent.LeaseRentPremium = model.LeaseRentPremium;
                leaserent.PenalInterest = model.PanelInterest;
                leaserent.GST = model.GST;
                leaserent.NormalInterest = model.PremiumInterest;
                leaserent.DuesAmount = model.TotalPremium;
                leaserent.DepositDueDate = model.DepositDueDate;
                leaserent.PaidAmount = model.PaidAmount;
                leaserent.DuesUptoDate = model.PaidUptoDate;
                leaserent.DepositDate = model.DepositDate;
                leaserent.PaymentMode = model.PaymentMode;
                leaserent.TransactionId = model.TransactionId;
                leaserent.BalanceAmount = model.BalanceAmount;
                leaserent.ApproverId = model.ApproverId;
                leaserent.ApprovalDate = model.ApprovalDate;
                leaserent.StatusId = model.StatusId;
                leaserent.IsActive = true;
                leaserent.Comment = model.Comment;
                leaserent.CreatedDate = DateTime.Now;
                leaserent.CreatedBy = userInfo.UserID;
                dbContext.LeaseRentPaymentTrans.Add(leaserent);
                dbContext.SaveChanges();

                flag = ReturnType.Saved;
            }
            return flag;
        }


        public DataSourceResult GetPaymentTypeList(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from payment in dbContext.PaymentTypeMsts
                            where payment.IsActive == true
                            select new DropdownViewModel
                            {
                                Id = payment.Id,
                                Text = payment.PaymentType
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetPaymentSubTypeList(DataSourceRequest request, int? typeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from payment in dbContext.PaymentSubTypeMsts
                            where payment.IsActive == true && payment.PaymentTypeId == typeId
                            select new DropdownViewModel
                            {
                                Id = payment.Id,
                                Text = payment.PaymentSubType
                            });
                request.Filters.RemoveAt(0);
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetMiscellaneousPaymentListById(DataSourceRequest request, int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var list = (from detail in dbContext.PaymentDetailMasters
                //            join trans in dbContext.PaymentDetailTrans on detail.Id equals trans.PaymentId
                //            from type in dbContext.PaymentTypeMsts.Where(p => p.Id == trans.PaymentTypeId).DefaultIfEmpty()
                //            from subtype in dbContext.PaymentSubTypeMsts.Where(p => p.Id == trans.PaymentSubTypeId).DefaultIfEmpty()
                //            where detail.RegistrationId == rid
                //            select new PaymentViewModel
                //            {
                //                Id = detail.Id,
                //                RegistrationId = detail.RegistrationId,
                //                RegistrationNo = detail.RegistrationId.ToString(),

                //                PropertyId = detail.PropertyId.ToString(),
                //                Applicant = detail.Applicant,
                //                DepositorName = detail.Depositor,
                //                DepositorAddress = detail.Address,
                //                DepositDate = detail.DepositDate,
                //                Amount = detail.Amount,
                //                PaymentTypeId = trans.PaymentTypeId,
                //                PaymentSubTypeId = trans.PaymentSubTypeId,
                //                PaymentType = type.PaymentType,
                //                PaymentSubType = subtype.PaymentSubType,
                //                BankName = trans.BankName,
                //                BranchName = trans.BranchName,
                //                TransactionId = trans.TransactionId,
                //                PaymentMode = trans.PaymentMode,
                //                ApprovalDate = trans.ApprovalDate
                //            });
                //return list.ToDataSourceResult(request);

                var list = (from detail in dbContext.RECEIPT_DETAIL_MASTER
                            join trans in dbContext.RECEIPT_AMOUNT_TRANS on detail.RECEIPT_ID equals trans.RECEIPT_ID
                            from alotment in dbContext.AllotmentMasters.Where(a => a.rid.ToString() == detail.RID_NO).DefaultIfEmpty()
                            from aplication in dbContext.ApplicationDetails.Where(a => a.registrationId.ToString() == detail.RID_NO).DefaultIfEmpty()
                            join schemeprop in dbContext.SchemePropTrans on alotment.propertyId equals schemeprop.propertyId
                            from type in dbContext.RECIEPT_HEAD.Where(p => p.RECIEPT_CODE == trans.RECEIPT_HEAD_ID).DefaultIfEmpty()
                            from subtype in dbContext.RECEIPT_SUB_HEAD.Where(p => p.RECEIPT_SUBHEAD_ID == trans.RECEIPT_SUBHEAD_ID).DefaultIfEmpty()
                            where detail.RID_NO == rid.ToString()
                            select new PaymentViewModel
                            {
                                //Id = detail.RECEIPT_ID,
                                //RegistrationId = detail.RegistrationId,
                                //RegistrationNo = detail.RID_NO,

                                //PropertyId = detail.PROP_ID,
                                //Applicant = detail.Applicant,
                                //DepositorName = detail.Depositor,
                                //DepositorAddress = detail.Address,
                                //DepositDate = detail.DepositDate,
                                //Amount = detail.Amount,
                                //PaymentTypeId = trans.PaymentTypeId,
                                //PaymentSubTypeId = trans.PaymentSubTypeId,
                                //PaymentType = type.PaymentType,
                                //PaymentSubType = subtype.PaymentSubType,
                                //BankName = trans.BankName,
                                //BranchName = trans.BranchName,
                                //TransactionId = trans.TransactionId,
                                //PaymentMode = trans.PaymentMode,
                                //ApprovalDate = trans.ApprovalDate

                                ReceiptId = detail.RECEIPT_ID,
                                RegistrationNo = detail.RID_NO,
                                DepartmentName = alotment.DepartmentMst.departmentName,
                                FirstName = aplication.tFirstName,
                                MiddleName = aplication.tMiddleName,
                                LastName = aplication.tLastName,
                                SectorName = schemeprop.SectorMst.sectorName,
                                BlockName = schemeprop.BlockMst.blockName,
                                PlotNo = schemeprop.propertyNo,
                                //PropertyNo = schemeprop.SectorMst.sectorName + "/" + schemeprop.BlockMst.blockName + "-" + schemeprop.propertyNo,
                                Amount = detail.AMOUNT,
                                DepositDate = detail.DEPOSIT_DATE,
                                EntryDate = detail.ENTRY_DATE

                            });
                return list.ToDataSourceResult(request);
            }
        }


        public PaymentViewModel GetAllottedPropertyDetailsById(int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var allottee = (from alotment in dbContext.AllotmentMasters
                                join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                                join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                                where alotment.rid == rid
                                select new PaymentViewModel
                                {
                                    RegistrationId = alotment.rid,
                                    PropertyId = alotment.propertyId.ToString(),
                                    FirstName = aplicant.tFirstName,
                                    MiddleName = aplicant.tMiddleName,
                                    LastName = aplicant.tLastName,
                                    Applicant = aplicant.tGender.ToLower() == Constants.Company.ToLower() ? aplicant.T_Company_Name : aplicant.tFirstName + (aplicant.tMiddleName == null ? "" : " " + aplicant.tMiddleName) + " " + aplicant.tLastName,
                                    RegistryDate = alotment.allotmentDate,
                                    SectorId = property.sectorId.Value,
                                    SectorName = property.SectorMst.sectorName,
                                    BlockId = property.blockId.Value,
                                    BlockName = property.BlockMst.blockName,
                                    PlotNo = property.propertyNo,
                                    DepartmentId = property.departmentId,
                                    DepartmentName = property.DepartmentMst.departmentName,
                                    MobileNo = aplicant.tMobileNumber,
                                    Email = aplicant.tEmail,
                                    ApplicantAddress = aplicant.tCorrespondanceAdd
                                }).FirstOrDefault();
                return allottee;
            }
        }


        public int SaveMiscellaneousPayment(PaymentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var payment = dbContext.PaymentDetailMasters.FirstOrDefault(p => p.Id == model.Id && p.IsActive == true);
                if (payment == null)
                {
                    PaymentDetailMaster master = new PaymentDetailMaster();
                    master.RegistrationId = model.RegistrationId;
                    master.DepartmentId = model.DepartmentId;
                    master.PropertyId = Convert.ToInt32(model.PropertyId);
                    master.Applicant = model.Applicant;
                    master.Sector = model.SectorName;
                    master.Block = model.BlockName;
                    master.PlotNo = model.PlotNo;
                    master.PropertyNo = model.PropertyNo;
                    master.Depositor = model.DepositorName;
                    master.Address = model.DepositorAddress;
                    master.Amount = model.Amount;
                    master.DepositDate = model.DepositDate;
                    master.Status = model.Status;
                    master.IsActive = true;
                    master.CreatedBy = userInfo.UserID;
                    master.CreatedDate = DateTime.Now;
                    dbContext.PaymentDetailMasters.Add(master);
                    dbContext.SaveChanges();

                    model.Id = master.Id;
                    PaymentDetailTran trans = new PaymentDetailTran();
                    trans.PaymentId = model.Id;
                    trans.PaymentTypeId = model.PaymentTypeId;
                    trans.PaymentSubTypeId = model.PaymentSubTypeId;
                    trans.Amount = model.Amount;
                    trans.BankName = model.BankName;
                    trans.BranchName = model.BranchName;
                    trans.TransactionId = model.TransactionId;
                    trans.PaymentMode = model.PaymentMode;
                    trans.DepositDate = model.DepositDate;
                    trans.Status = model.Status;
                    trans.IsActive = true;
                    trans.ApproverId = model.ApproverId;
                    trans.ApprovalDate = model.ApprovalDate;
                    trans.Comment = model.Comment;
                    trans.CreatedBy = userInfo.UserID;
                    trans.CreatedDate = DateTime.Now;
                    dbContext.PaymentDetailTrans.Add(trans);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                else
                {
                    flag = ReturnType.Exist;
                }
            }
            return flag;
        }


        public DataSourceResult GetLeaseRentPaymentListById(DataSourceRequest request, int? rid)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                if (rid != null)
                {
                    var rentlist = (from rentmaster in dbcontext.LeaseRentPayments
                                    join leaserent in dbcontext.LeaseRentPaymentTrans on rentmaster.Id equals leaserent.RefId
                                    where rentmaster.RegistrationId == rid
                                    select new LeaseRentViewModel
                                    {
                                        Id = rentmaster.Id,
                                        RegistrationId = rentmaster.RegistrationId,
                                        Applicant = rentmaster.Applicant,
                                        Sector = rentmaster.Sector,
                                        Block = rentmaster.Block,
                                        PlotNo = rentmaster.PlotNo,
                                        LeaseDeedDate = rentmaster.LeaseDeedDate,
                                        TransferLeaseDate = rentmaster.TransferLeaseDate,
                                        IsOneTimeLeaseRentPaid = rentmaster.IsOneTimeLeasePaid,
                                        OneTimePaidStatus = rentmaster.IsOneTimeLeasePaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                                        InstallmentPaidStatus = (rentmaster.IsTotalInstallmentPaid == null || rentmaster.IsTotalInstallmentPaid == true) ? "Paid" : "Not Paid",
                                        NDCDate = rentmaster.NDCDate,
                                        LeaseRentPremium = leaserent.LeaseRentPremium,
                                        StatusId = leaserent.StatusId,
                                        ApproverId = leaserent.ApproverId,
                                        Approver = leaserent.ApproverId > 0 ? dbcontext.UmUserMasters.Where(x => x.UserRefId == leaserent.ApproverId).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault() : string.Empty,
                                        IsActive = leaserent.IsActive,
                                        Comment = leaserent.Comment,
                                        ApprovalDate = leaserent.ApprovalDate,
                                        Status = leaserent.StatusId > 0 ? (from status in dbcontext.StatusMasters where status.Id == leaserent.StatusId select status.Status).FirstOrDefault() : string.Empty,
                                    });
                    return rentlist.ToDataSourceResult(request);
                }
                else
                {
                    var list = new List<LeaseRentViewModel>();
                    return list.ToDataSourceResult(request);
                }
            }
        }


        public DataSourceResult GetInstallmentPaymentScheduleList(DataSourceRequest request, PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from detail in dbContext.InstallmentDuesPayments
                            //join aplicant in dbContext.ApplicationDetails on detail.Rid equals aplicant.registrationId
                            where (model.DepartmentId == null || detail.DepartmentId == model.DepartmentId)
                            select new PaymentScheduleModel
                            {
                                Id = detail.Id,
                                RegistrationId = detail.RegistrationId,
                                DepartmentId = detail.DepartmentId,
                                PropertyId = detail.PropertyId,
                                InstallmentStartDate = detail.InstallmentStartDate,
                                InstallmentEndDate = detail.InstallmentEndDate,
                                PenalInterest = detail.PenalInterest,
                                GST = detail.GST,
                                NDCDate = detail.NDCDate,
                                NDCStatus = detail.NDCStatus,
                                DuesAmount = detail.DuesAmount,
                                GSTAmount = detail.GstAmount,
                                DuesUptoDate = detail.DuesUptoDate,
                                BalanceInterest = detail.BalanceInterest,
                                BalanceAmount = detail.BalanceAmount,
                                BalanceUptoDate = detail.BalanceUptoDate,
                                PreviousDues = detail.PreviousDues,
                                PreviousDuesDate = detail.PreviousDuesDate,
                                CurrentDues = detail.CurrentDues,
                                CurrentDuesDate = detail.CurrentDuesDate,
                                PaymentMode = detail.PaymentMode,
                                TransactionId = detail.TransactionId,
                                IsOneTimeLeaseRentPaid = detail.IsOneTimeLeasePaid,
                                IsTotalPremiumPaid = detail.IsTotalPremiumPaid,
                                IsNDCGenerated = (detail.IsOneTimeLeasePaid == true && detail.IsTotalPremiumPaid == true) ? true : false,
                                IsActive = detail.IsActive,
                                TotalInstallmentStatus = (detail.IsTotalPremiumPaid == null || detail.IsTotalPremiumPaid == true) ? "Paid" : "Not Paid",
                                OneTimeLeaseRentStatus = (detail.IsOneTimeLeasePaid == null || detail.IsOneTimeLeasePaid == true) ? "Paid" : "Not Paid",
                                ActionType = null
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetInstallmentPaymentListForApproval(DataSourceRequest request, PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from schedule in dbContext.InstallmentDuesPayments
                            join instalment in dbContext.InstallmentDuesPaymentTrans on schedule.Id equals instalment.RefId
                            join aplicant in dbContext.ApplicationDetails on schedule.RegistrationId equals aplicant.registrationId
                            where instalment.IsActive == true //&& DepartmentList.Contains(schedule.DepartmentId)
                            select new PaymentScheduleModel
                            {
                                Id = schedule.Id,
                                RegistrationId = schedule.RegistrationId,
                                PropertyId = schedule.PropertyId,
                                //PrincipalAmount = schedule.PreviousDues,
                                Applicant = aplicant.tGender == Constants.Company ? aplicant.T_Company_Name : aplicant.tFirstName + (aplicant.tMiddleName == null ? "" : " " + aplicant.tMiddleName + " " + aplicant.tLastName),
                                //TotalInstallment = schedule.NoOfInstallment,
                                Frequency = schedule.Frequency,
                                //PeriodOfInstallment = schedule.PeriodOfInstallment,
                                InstallmentStartDate = schedule.InstallmentStartDate,
                                //NormalInterest = schedule.NormalInterest,
                                PenalInterest = schedule.PenalInterest,
                                //ScheduleType = schedule.ScheduleType,
                                IsOneTimeLeaseRentPaid = schedule.IsOneTimeLeasePaid,
                                IsInstallmentDeposited = schedule.IsTotalPremiumPaid,
                                IsActive = schedule.IsActive,
                                OneTimeIstallmentStatus = (schedule.IsTotalPremiumPaid == null || schedule.IsTotalPremiumPaid == true) ? "Paid" : "Not Paid",
                                OneTimeLeaseRentStatus = (schedule.IsOneTimeLeasePaid == null || schedule.IsOneTimeLeasePaid == true) ? "Paid" : "Not Paid",

                                //InstallmentNo = instalment.InstallmentNo,
                                //InstallmentAmount = instalment.InstallmentAmount,
                                //InstallmentDueDate = instalment.InstallmentDueDate,
                                //InstallmentEndDate = instalment.InstallmentEndDate,
                                PaymentMode = instalment.PaymentMode,
                                TransactionId = instalment.TransactionId,
                                //IsOneTimeInstallment = instalment.IsOneTimeInstallment,
                                //IsInstallmentPaid = instalment.IsInstallmentPaid,
                                //InstallmentPeriod = instalment.InstallmentPeriod,
                                //InstallmentStatus = (instalment.IsInstallmentPaid == null || instalment.IsInstallmentPaid == true) ? "Approved" : "In Progress",
                                ActionType = null
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetMiscellaneousPaymentList(DataSourceRequest request, PaymentViewModel model)
        {
            //using (var dbContext = new NoidaPMSEntities())
            //{
            //    var list = (from detail in dbContext.PaymentDetailMasters
            //                join trans in dbContext.PaymentDetailTrans on detail.Id equals trans.PaymentId
            //                from department in dbContext.DepartmentMsts.Where(d => d.departmentId == detail.DepartmentId).DefaultIfEmpty()
            //                from type in dbContext.PaymentTypeMsts.Where(p => p.Id == trans.PaymentTypeId).DefaultIfEmpty()
            //                from subtype in dbContext.PaymentSubTypeMsts.Where(p => p.Id == trans.PaymentSubTypeId).DefaultIfEmpty()
            //                where (model.RegistrationId == null || detail.RegistrationId == model.RegistrationId)
            //                && (model.DepartmentId == null || detail.DepartmentId == model.DepartmentId)
            //                && (model.PaymentTypeId == null || trans.PaymentTypeId == model.PaymentTypeId)
            //                && (model.PaymentSubTypeId == null || trans.PaymentSubTypeId == model.PaymentSubTypeId)
            //                select new PaymentViewModel
            //                {
            //                    Id = detail.Id,
            //                    RegistrationId = detail.RegistrationId,
            //                    RegistrationNo = detail.RegistrationId.ToString(),
            //                    DepartmentName = department.departmentName,
            //                    PropertyId = detail.PropertyId.ToString(),
            //                    Applicant = detail.Applicant,
            //                    DepositorName = detail.Depositor,
            //                    DepositorAddress = detail.Address,
            //                    DepositDate = detail.DepositDate,
            //                    Amount = detail.Amount,
            //                    PaymentTypeId = trans.PaymentTypeId,
            //                    PaymentSubTypeId = trans.PaymentSubTypeId,
            //                    PaymentType = type.PaymentType,
            //                    PaymentSubType = subtype.PaymentSubType,
            //                    BankName = trans.BankName,
            //                    BranchName = trans.BranchName,
            //                    TransactionId = trans.TransactionId,
            //                    PaymentMode = trans.PaymentMode,
            //                    ApprovalDate = trans.ApprovalDate
            //                });
            //    return list.ToDataSourceResult(request);
            //}

            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from detail in dbContext.RECEIPT_DETAIL_MASTER
                            join trans in dbContext.RECEIPT_AMOUNT_TRANS on detail.RECEIPT_ID equals trans.RECEIPT_ID
                            from type in dbContext.RECIEPT_HEAD.Where(p => p.RECIEPT_CODE == trans.RECEIPT_HEAD_ID).DefaultIfEmpty()
                            from subtype in dbContext.RECEIPT_SUB_HEAD.Where(p => p.RECEIPT_SUBHEAD_ID == trans.RECEIPT_SUBHEAD_ID).DefaultIfEmpty()
                            where (model.RegistrationId == null || detail.RID_NO == model.RegistrationId.ToString())
                            && (model.DepartmentId == null || detail.DEPT_ID == model.DepartmentId)
                            && (model.PaymentTypeId == null || trans.RECEIPT_HEAD_ID == model.PaymentTypeId)
                            && (model.PaymentSubTypeId == null || trans.RECEIPT_SUBHEAD_ID == model.PaymentSubTypeId)
                            select new PaymentViewModel
                            {
                                Id = 0,
                                //RegistrationId = detail.RegistrationId,
                                RegistrationNo = detail.RID_NO,
                                DepartmentName = detail.DEPT_ID == null ? " " : dbContext.DepartmentMsts.FirstOrDefault(x => x.departmentId == detail.DEPT_ID).departmentName,
                                //PropertyId = detail.PropertyId.ToString(),
                                Applicant = detail.ALLOTE_NAME,
                                DepositorName = detail.DEPOSETER_NAME,
                                DepositorAddress = detail.ADDRESS,
                                DepositDate = detail.DEPOSIT_DATE,
                                Amount = detail.AMOUNT,
                                PaymentTypeId = trans.RECEIPT_HEAD_ID,
                                PaymentSubTypeId = trans.RECEIPT_SUBHEAD_ID,
                                PaymentType = type.RECIEPT_HEAD_NAME,
                                PaymentSubType = subtype.RECEIPT_SUB_HEAD1,
                                //BankName = trans,
                                //BranchName = trans.BranchName,
                                //TransactionId = trans.TransactionId,
                                //PaymentMode = trans.PaymentMode,
                                //ApprovalDate = trans.ApprovalDate
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetMultiplePaymentStatusList(DataSourceRequest request, LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                bool IsOTLRP = model.PremiumPaidStatus == NA.PMS.Common.NDCOptions.Id_Yes ? true : false;

                //if (model.ActionType == "InstallmentDues")
                //{
                //    var list = (from instlmnt in dbcontext.InstallmentDuesPayments
                //                join alotment in dbcontext.AllotmentMasters on instlmnt.RegistrationId equals alotment.rid
                //                join property in dbcontext.SchemePropTrans on alotment.propertyId equals property.propertyId
                //                where (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                //                  && (model.IsOneTimeLeaseRentPaid == null || instlmnt.IsOneTimeLeasePaid == model.IsOneTimeLeaseRentPaid)
                //                  && (model.IsTotalInstallmentPaid == null || instlmnt.IsTotalPremiumPaid == model.IsTotalInstallmentPaid)
                //                select new LeaseRentViewModel
                //                {
                //                    Id = instlmnt.Id,
                //                    RegistrationId = instlmnt.RegistrationId,
                //                    DuesAmount = instlmnt.DuesAmount,
                //                    GstAmount = instlmnt.GstAmount,
                //                    BalanceAmount = instlmnt.BalanceAmount,
                //                    BalanceUptoDate = instlmnt.BalanceUptoDate,
                //                    Sector = property.SectorMst.sectorName,
                //                    Block = property.BlockMst.blockName,
                //                    PlotNo = property.propertyNo,
                //                    DuesUptoDate = instlmnt.DuesUptoDate,
                //                    LeaseRentPremium = instlmnt.BalanceAmount,
                //                    CurrentDues = instlmnt.CurrentDues,
                //                    CurrentDuesDate = instlmnt.CurrentDuesDate,
                //                    LeaseRentDues = null
                //                });
                //    return list.ToDataSourceResult(request);
                //}
                if (model.ActionType == "InstallmentDues")
                {
                    var list = (from instlmnt in dbcontext.viewInstutionalPBalances
                                join alotment in dbcontext.AllotmentMasters on instlmnt.RegistrationId equals alotment.rid
                                join property in dbcontext.SchemePropTrans on alotment.propertyId equals property.propertyId
                                where (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                                  && (model.IsOneTimeLeaseRentPaid == null || instlmnt.IsOneTimeLeasePaid == model.IsOneTimeLeaseRentPaid)
                                  && (model.IsTotalInstallmentPaid == null || instlmnt.IsTotalPremiumPaid == model.IsTotalInstallmentPaid)
                                select new LeaseRentViewModel
                                {
                                    Id = instlmnt.Id,
                                    RegistrationId = instlmnt.RegistrationId,
                                    PremiumBalance = instlmnt.PremiumBalance,
                                    GstAmount = instlmnt.GST,
                                    BalanceAmount = instlmnt.BalanceAmount,
                                    BalanceUptoDate = instlmnt.BalanceUptoDate,
                                    Sector = property.SectorMst.sectorName,
                                    Block = property.BlockMst.blockName,
                                    PlotNo = property.propertyNo,
                                    PropertyNo = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "/" + property.propertyNo,
                                    DuesUptoDate = instlmnt.DuesUptoDate,
                                    LeaseRentPremium = instlmnt.BalanceAmount,
                                    CurrentDues = instlmnt.CurrentDues,
                                    CurrentDuesDate = instlmnt.CurrentDuesDate,
                                    IsDuesUptoDate = (instlmnt.DuesUptoDate == null || DbFunctions.TruncateTime(instlmnt.DuesUptoDate) < DbFunctions.TruncateTime(DateTime.Now)) ? false : true
                                    //LeaseRentDues = null
                                });
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    var list = (from leaserent in dbcontext.LeaseRentPayments
                                join alotment in dbcontext.AllotmentMasters on leaserent.RegistrationId equals alotment.rid
                                join property in dbcontext.SchemePropTrans on alotment.propertyId equals property.propertyId
                                where (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                                && (model.RegistrationId == null || leaserent.RegistrationId == model.RegistrationId)
                                 && (model.IsOneTimeLeaseRentPaid == null || leaserent.IsOneTimeLeasePaid == model.IsOneTimeLeaseRentPaid)
                                 && (model.IsTotalInstallmentPaid == null || leaserent.IsTotalPremiumPaid == model.IsTotalInstallmentPaid)
                                select new LeaseRentViewModel
                                {
                                    Id = leaserent.Id,
                                    RentId = leaserent.Id,
                                    DepartmentId = alotment.departmentId,
                                    RegistrationId = leaserent.RegistrationId,
                                    PropertyNo = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "/" + property.propertyNo,
                                    LeaseDeedDate = leaserent.LeaseDeedDate,
                                    LeaseRentPremium = leaserent.PremiumLeaseRent,
                                    PanelInterest = leaserent.PanelInterest,
                                    PaidUptoDate = leaserent.PremiumPaidUptoDate,
                                    PremiumPaidDuration = leaserent.PremiumPaidDuration,
                                    RevisedDate = leaserent.RevisedPremiumDate,
                                    RevisedPremium = leaserent.RevisedRate,
                                    BalanceAmount = leaserent.BalanceAmount,
                                    BalanceUptoDate = leaserent.BalanceUptoDate,
                                    BalanceInterest = leaserent.BalanceInterest,
                                    GST = leaserent.GST,
                                    TotalBalance = leaserent.BalanceAmount != null ? leaserent.BalanceAmount : 0 + leaserent.BalanceInterest != null ? leaserent.BalanceInterest : 0 + leaserent.GST != null ? leaserent.GST : 0,
                                    IsOneTimeLeaseRentPaid = leaserent.IsOneTimeLeasePaid,
                                    OneTimePaidStatus = leaserent.IsOneTimeLeasePaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                                    PremiumPaidStatus = leaserent.IsTotalPremiumPaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                                    NDCDate = leaserent.NDCDate,
                                    NDCStatus = leaserent.NDCStatus,
                                    ChallanDate = leaserent.ChallanDate,
                                    StatusId = leaserent.StatusId,
                                    Status = leaserent.StatusId > 0 ? (from status in dbcontext.StatusMasters where status.Id == leaserent.StatusId select status.Status).FirstOrDefault() : string.Empty,
                                    IsActive = leaserent.IsActive,
                                    Sector = property.SectorMst.sectorName,
                                    Block = property.BlockMst.blockName,
                                    PlotNo = property.propertyNo,
                                    LeaseRentDues = leaserent.LeaseRentDues,//for lease rent dues
                                    DuesUptoDate = leaserent.DuesUptoDate,
                                    CurrentDues = leaserent.CurrentDues,
                                    DuesInterest = leaserent.DuesInterest,
                                    //TotalAmount = leaserent.TotalDuesAmount,
                                    CurrentDuesDate = leaserent.CurrentDuesDate,
                                    PreviousDuesDate = leaserent.PreviousDuesDate,
                                    IsDuesUptoDate = (leaserent.DuesUptoDate == null || DbFunctions.TruncateTime(leaserent.DuesUptoDate) < DbFunctions.TruncateTime(DateTime.Now)) ? false : true
                                });
                    var datalist = list.ToDataSourceResult(request);
                    return datalist;
                }
            }
        }

        public LeaseRentViewModel GetMiscellaneousPaymentCount()
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                LeaseRentViewModel model = new LeaseRentViewModel();
                model.TotalLeaseRentCount = dbcontext.LeaseRentPayments.Where(r => r.IsOneTimeLeasePaid == false).Count();//&& r.IsTotalPremiumPaid == true
                model.TotalNDCCount = dbcontext.LeaseRentPayments.Where(r => r.IsOneTimeLeasePaid == true && r.IsTotalPremiumPaid == true).Count();
                //model.TotalNDCCount = dbcontext.PRE_FULL_PAYMENT_NDC.Where(r => r.IsActive == true).Count();
                //model.TotalIPCount = dbcontext.LeaseRentPayments.Where(r => r.IsTotalPremiumPaid == false && r.IsOneTimeLeasePaid == false).Count();
                model.TotalIPCount = dbcontext.InstallmentDuesPayments.Where(r => r.IsTotalPremiumPaid == false).Count();
                model.DefaulterCount = dbcontext.LeaseRentPayments.Where(r => r.IsOneTimeLeasePaid == false && r.IsTotalPremiumPaid == false).Count();
                return model;
            }
        }

        public int UpdateLeaseRentDuesPayment(LeaseRentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbcontext = new NoidaPMSEntities())
            {
                if (model.DepartmentId != null && model.DepartmentId != 0)
                {
                    //var datalist = dbcontext.LeaseRentPayments.Where(r => r.IsOneTimeLeasePaid == false);
                    //var datalist = (from leaserent in dbcontext.LeaseRentPayments
                    //                where leaserent.IsOneTimeLeasePaid == false
                    //                && (DbFunctions.TruncateTime(leaserent.DuesUptoDate) != DbFunctions.TruncateTime(DateTime.Now))
                    //                && (leaserent.DepartmentId == model.DepartmentId)
                    //                select leaserent);
                    var datalist = (from leaserent in dbcontext.LeaseRentPayments
                                    where leaserent.IsOneTimeLeasePaid == false
                                        //&& (DbFunctions.TruncateTime(leaserent.DuesUptoDate) != DbFunctions.TruncateTime(DateTime.Now))
                                    && (leaserent.DepartmentId == model.DepartmentId)
                                    select leaserent);
                    foreach (var list in datalist)
                    {
                        model.RegistrationId = list.RegistrationId;
                        model.ActionType = "LeaseRent";
                        var data = UpdateLeaseRentDuesPaymentById(model);
                        //var newdata = CalculateLeaseRentPremium(model);
                        //list.RevisedPremium = newdata.LeaseRentPremium;
                        //list.LeaseRentDues = newdata.DuesAmount;
                        //list.DuesUptoDate = newdata.DuesUptoDate;
                        //list.RevisedPremiumDate = newdata.RevisedDate;
                        //list.HtmlDuesTemplate = newdata.HtmlDuesReport;
                        //dbcontext.SaveChanges();
                        //flag = ReturnType.Success;
                    }
                    //dbcontext.SaveChanges();
                    flag = ReturnType.Success;
                }

                model.TotalLeaseRentCount = dbcontext.LeaseRentPayments.Where(r => r.RegistrationId == model.RegistrationId && r.IsTotalPremiumPaid == true).Count();
                model.TotalNDCCount = dbcontext.LeaseRentPayments.Where(r => r.RegistrationId == model.RegistrationId && r.NDCStatus == "Approved" && r.IsActive == true).Count();
            }
            return flag;
        }

        public int UpdateInstallmentDuesPayment(LeaseRentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbcontext = new NoidaPMSEntities())
            {
                if (model.DepartmentId != null && model.DepartmentId != 0)
                {
                    var datalist = dbcontext.InstallmentDuesPayments.Where(r => r.IsTotalPremiumPaid == false && r.DepartmentId == model.DepartmentId);
                    foreach (var list in datalist)
                    {
                        model.RegistrationId = list.RegistrationId;
                        var newdata = CalculateInstallmentDues(model);
                        list.DuesAmount = newdata.LeaseRentPremium;
                        list.GstAmount = newdata.GstAmount;
                        list.DuesUptoDate = newdata.DuesUptoDate;
                    }
                    dbcontext.SaveChanges();
                    flag = ReturnType.Success;
                }
            }
            return flag;
        }

        //private LeaseRentViewModel CalculateLeaseRentPremium(LeaseRentViewModel model)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        //check in Lease Rent Payment Details
        //        var leasemaster = dbContext.LeaseRentPayments.Where(r => r.RegistrationId == model.RegistrationId).OrderByDescending(d => d.Id).ToList();
        //        if (leasemaster != null)
        //        {
        //            var lease = leasemaster.FirstOrDefault();
        //            var applicant = dbContext.ApplicationDetails.FirstOrDefault(r => r.registrationId == lease.RegistrationId);
        //            string allottee = string.Empty;
        //            allottee = applicant != null ? applicant.tFirstName : string.Empty;
        //            model.RentId = lease.Id;
        //            string propertyno = string.Empty;
        //            var property = (from alot in dbContext.AllotmentMasters
        //                            join prop in dbContext.SchemePropTrans on alot.propertyId equals prop.propertyId
        //                            where alot.rid == lease.RegistrationId
        //                            select prop).FirstOrDefault();
        //            if (property != null)
        //            {
        //                propertyno = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo;
        //            }
        //            //string dueshtml = "<style>.print-row:nth-child(2n+1){background:#d5d5d5;}</style>";
        //            string dueshtml = "<table id='Report'><tr><td colspan='7' style='text-align:center'><h3>Property Lease Rent Report</h3></td></tr>";
        //            dueshtml = dueshtml + "<tr><td colspan='3'><b>Allottee: " + allottee + "</b></td><td colspan='2' style='text-align:center'>Property: " + propertyno + "</td><td colspan='2' style='text-align:right'><b>Registration Id: " + lease.RegistrationId + "</b></td></tr>";
        //            dueshtml = dueshtml + "<tr><td>FY</td><td>PI</td><td>Lease Rent</td><td>Interest</td><td>Paid Amount</td><td>Total</td><td>Dues Upto</td></tr>";
        //            var leasedate = lease.TransferLeaseDate == null ? lease.LeaseDeedDate : lease.TransferLeaseDate;
        //            var today = DateTime.Now;
        //            //today = today.AddDays(383);
        //            var nextdate = lease.PremiumPaidUptoDate == null ? DateTime.Now : lease.PremiumPaidUptoDate;
        //            var paidUptoDate = lease.PremiumPaidUptoDate;
        //            var revisedate = lease.RevisedPremiumDate == null ? lease.LeaseDeedDate : lease.RevisedPremiumDate;
        //            decimal premiumdues = 0;
        //            decimal interest = 0;
        //            decimal gstamount = 0;
        //            var nexttodate = today;
        //            string printdate = string.Empty;

        //            decimal? total_amount = 0;
        //            decimal? reviserate = lease.RevisedRate == null ? 0 : lease.RevisedRate;
        //            if (lease.PremiumPaidUptoDate != null)
        //            {
        //                var premium = lease.PremiumLeaseRent == null ? 0 : lease.PremiumLeaseRent;
        //                decimal? rate = lease.PanelInterest == null ? 0 : lease.PanelInterest;
        //                decimal? gstrate = lease.GST == null ? 0 : lease.GST;
        //                total_amount = (decimal)premium;
        //                nexttodate = paidUptoDate.Value.AddMonths(12);

        //                while (today > paidUptoDate)
        //                {
        //                    nextdate = paidUptoDate.Value.AddMonths(6);
        //                    decimal? htmldues = 0;
        //                    decimal? htmlinterest = 0;
        //                    decimal? paidamount = 0;
        //                    string prevyear = string.Empty;
        //                    string nextyear = string.Empty;
        //                    if (nextdate.Value.Year > paidUptoDate.Value.Year)
        //                    {
        //                        prevyear = paidUptoDate.Value.Year.ToString();
        //                        nextyear = nextdate.Value.Year.ToString();
        //                    }
        //                    else
        //                    {
        //                        prevyear = (paidUptoDate.Value.Year - 1).ToString();
        //                        nextyear = paidUptoDate.Value.Year.ToString();
        //                    }


        //                    if (paidUptoDate >= nexttodate)
        //                    {
        //                        nexttodate = paidUptoDate.Value.AddMonths(12);

        //                        if (paidUptoDate >= revisedate)
        //                        {
        //                            total_amount = total_amount + ((decimal)premium + ((decimal)premium * reviserate) / 100);
        //                            var premiumnew = (decimal)premium + ((decimal)premium * reviserate) / 100;
        //                            premium = Math.Round((decimal)premium, 2);
        //                        }
        //                        else
        //                        {
        //                            total_amount = total_amount + (decimal)premium;
        //                        }
        //                    }

        //                    if (today > nextdate)
        //                    {
        //                        var rentPaidList = dbContext.ViewLeaseRentChecks.OrderByDescending(r => r.DEPOSIT_DATE).FirstOrDefault(r => r.RID_NO == model.RegistrationId.ToString() && r.RECEIPT_SUBHEAD_ID == 4 && (r.DEPOSIT_DATE >= paidUptoDate && r.DEPOSIT_DATE <= nextdate));
        //                        if (rentPaidList != null)
        //                        {
        //                            //var rentPaid = rentPaidList.FirstOrDefault();
        //                            var depositdate = rentPaidList.DEPOSIT_DATE;
        //                            var amountpaid = rentPaidList.AMOUNT_PAID;
        //                            var day = paidUptoDate - depositdate;
        //                            decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
        //                            paidamount = amountpaid;
        //                            total_amount = (total_amount + rinterest) - amountpaid;
        //                            day = nextdate - depositdate;
        //                            rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
        //                            total_amount = (total_amount + rinterest);
        //                            premiumdues = Math.Round((decimal)total_amount, 2);
        //                            interest = Math.Round((decimal)rinterest, 2);
        //                        }
        //                        else
        //                        {
        //                            var day = nextdate - paidUptoDate;
        //                            decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
        //                            total_amount = (total_amount + rinterest);
        //                            premiumdues = Math.Round((decimal)total_amount, 2);
        //                            interest = Math.Round((decimal)rinterest, 2);
        //                        }
        //                        paidUptoDate = nextdate;
        //                    }
        //                    else
        //                    {
        //                        var rentPaidList = dbContext.ViewLeaseRentChecks.OrderByDescending(r => r.DEPOSIT_DATE).FirstOrDefault(r => r.RID_NO == model.RegistrationId.ToString() && r.RECEIPT_SUBHEAD_ID == 4 && (r.DEPOSIT_DATE >= paidUptoDate && r.DEPOSIT_DATE <= today));
        //                        if (rentPaidList != null)
        //                        {
        //                            //var rentPaid = rentPaidList.FirstOrDefault();
        //                            var depositdate = rentPaidList.DEPOSIT_DATE;
        //                            var amountpaid = rentPaidList.AMOUNT_PAID;
        //                            var day = paidUptoDate - depositdate;
        //                            decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
        //                            total_amount = (total_amount + rinterest) - amountpaid;
        //                            day = nextdate - depositdate;
        //                            rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
        //                            total_amount = (total_amount + rinterest);
        //                            premiumdues = Math.Round((decimal)total_amount, 2);
        //                            interest = Math.Round((decimal)rinterest, 2);
        //                        }
        //                        else
        //                        {
        //                            var day = paidUptoDate - nextdate;

        //                            if (paidUptoDate.Value.Date >= today.Date)
        //                            {
        //                                day = paidUptoDate - today;
        //                            }

        //                            decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
        //                            total_amount = (total_amount + rinterest);
        //                            premiumdues = Math.Round((decimal)total_amount, 2);
        //                            interest = Math.Round((decimal)rinterest, 2);
        //                        }
        //                        paidUptoDate = nextdate;
        //                    }
        //                    htmldues = premium;
        //                    htmlinterest = interest;
        //                    printdate = paidUptoDate.Value.Day + "-" + paidUptoDate.Value.Month + "-" + paidUptoDate.Value.Year;
        //                    string fy = prevyear + "-" + nextyear;
        //                    dueshtml = dueshtml + "<tr><td>" + fy + "</td><td>" + rate + "</td><td>" + htmldues + "</td><td>" + htmlinterest + "</td><td>" + paidamount + "</td><td>" + Math.Round((decimal)total_amount, 5) + "</td><td>" + printdate + "</td></tr>";

        //                }

        //                //dueshtml = dueshtml + "<tr><td>Noida Authority</td></tr>";

        //                var days2 = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        //                var today2 = DateTime.Now.AddDays(days2 - DateTime.Now.Day);

        //                model.LeaseRentPremium = premiumdues;
        //                model.PremiumInterest = interest;
        //                model.GstAmount = gstamount;
        //                model.PaidUptoDate = paidUptoDate;
        //                model.DuesUptoDate = today2;
        //                dueshtml = dueshtml + "</table>";
        //                model.HtmlDuesReport = dueshtml;
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

        private LeaseRentViewModel CalculateLeaseRentPremium(LeaseRentViewModel model)
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
                        if (revisedate.Value.Date < today.Date)
                        {
                            revisedate = revisedate.Value.AddYears(10);
                        }

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
                                    premium = Math.Round((decimal)premiumnew, 2);
                                    revisedate = revisedate.Value.AddYears(10);
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
                                    decimal? rinterest = (((total_amount * rate) / 100) / 365) * (day.Value.Days + 1);
                                    paidamount = amountpaid;
                                    total_amount = (total_amount + rinterest) - amountpaid;
                                    day = nextdate - depositdate;
                                    rinterest = (((total_amount * rate) / 100) / 365) * (day.Value.Days + 1);
                                    total_amount = (total_amount + rinterest);
                                    premiumdues = Math.Round((decimal)total_amount, 2);
                                    interest = Math.Round((decimal)rinterest, 2);
                                }
                                else
                                {
                                    var day = nextdate - paidUptoDate;
                                    decimal? rinterest = (((total_amount * rate) / 100) / 365) * (day.Value.Days + 1);
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
                                    decimal? rinterest = (((total_amount * rate) / 100) / 365) * (day.Value.Days + 1);
                                    total_amount = (total_amount + rinterest) - amountpaid;
                                    day = nextdate - depositdate;
                                    rinterest = (((total_amount * rate) / 100) / 365) * (day.Value.Days + 1);
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

                                    decimal? rinterest = (((total_amount * rate) / 100) / 365) * (day.Value.Days + 1);
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
                            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 2) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

                        }

                        //dueshtml = dueshtml + "<tr><td>Noida Authority</td></tr>";

                        var days2 = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                        var today2 = DateTime.Now.AddDays(days2 - DateTime.Now.Day);

                        model.LeaseRentPremium = premium;
                        model.DuesAmount = premiumdues;
                        model.PremiumInterest = interest;
                        model.GstAmount = gstamount;
                        model.PaidUptoDate = paidUptoDate;
                        model.RevisedDate = revisedate;
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

        private List<LeaseRentViewModel> CalculateLeaseRentDues(LeaseRentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<LeaseRentViewModel> rentList = new List<LeaseRentViewModel>();
                //check in Lease Rent Payment Details
                var leasemaster = dbContext.LeaseRentPayments.Where(r => r.RegistrationId == model.RegistrationId).OrderByDescending(d => d.Id).ToList();
                var registry = dbContext.RegistryDetails.FirstOrDefault(r => r.Rid == model.RegistrationId);
                if (leasemaster != null && leasemaster.Count > 0)
                {
                    string allottee = string.Empty;
                    string propertyno = string.Empty;
                    string dueshtml = string.Empty;
                    string printdate = string.Empty;

                    decimal premiumdues = 0;
                    decimal interest = 0;
                    decimal gstamount = 0;
                    decimal? total_amount = 0;

                    decimal? T_amount = 0;
                    decimal? T_interest = 0;

                    var today = DateTime.Now;
                    var nexttodate = today;

                    var lease = leasemaster.FirstOrDefault();
                    model.RentId = lease.Id;
                    var applicant = dbContext.ApplicationDetails.FirstOrDefault(r => r.registrationId == lease.RegistrationId);
                    var property = (from alot in dbContext.AllotmentMasters
                                    join prop in dbContext.SchemePropTrans on alot.propertyId equals prop.propertyId
                                    where alot.rid == lease.RegistrationId
                                    select prop).FirstOrDefault();

                    allottee = applicant != null ? applicant.tFirstName : string.Empty;

                    if (property != null)
                    {
                        propertyno = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo;
                    }
                    //string dueshtml = "<style>.print-row:nth-child(2n+1){background:#d5d5d5;}</style>";
                    dueshtml = "<div id='Report'><div class='row print-row'><div class='col-md-12 print-col' style='text-align:center'><h3>Property Lease Rent Report</h3></div></div>";
                    dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-4 print-col'><b>Allottee: " + allottee + "</b></div><div class='col-md-4 print-col' style='text-align:center'>Property: " + propertyno + "</div><div class='col-md-4 print-col' style='text-align:right'><b>Registration Id: " + lease.RegistrationId + "</b></div></div>";
                    dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>FY</div><div class='col-md-1 print-col'>PI</div><div class='col-md-2 print-col'>Lease Rent</div><div class='col-md-2 print-col'>Interest</div><div class='col-md-2 print-col'>Paid Amount</div><div class='col-md-2 print-col'>Total</div><div class='col-md-2 print-col'>Dues Upto</div></div>";

                    var leaseDeedDate = lease.TransferLeaseDate == null ? lease.LeaseDeedDate : lease.TransferLeaseDate;
                    var nextdate = lease.PremiumPaidUptoDate == null ? DateTime.Now : lease.PremiumPaidUptoDate;
                    var paidUptoDate = lease.PremiumPaidUptoDate;
                    var nextDueDate = leaseDeedDate.Value.AddYears(1);
                    var prevDueDate = leaseDeedDate.Value.AddYears(1);
                    var revisedate = lease.RevisedPremiumDate == null ? lease.LeaseDeedDate : lease.RevisedPremiumDate;
                    decimal? reviserate = lease.RevisedRate == null ? 0 : lease.RevisedRate;



                    //var rentDueDate = leaseDeedDate.Value.AddYears(1);

                    if (lease.PremiumPaidUptoDate != null)
                    {
                        var premium = lease.PremiumLeaseRent == null ? 0 : lease.PremiumLeaseRent;
                        decimal? rate = lease.PanelInterest == null ? 0 : lease.PanelInterest;
                        decimal? gstrate = lease.GST == null ? 0 : lease.GST;
                        total_amount = (decimal)premium;
                        var paidDueDate = DateTime.Now;

                        decimal? htmldues = 0;
                        decimal? htmlinterest = 0;
                        decimal? paidamount = 0;
                        string prevyear = string.Empty;
                        string nextyear = string.Empty;

                        var paidYear = lease.PremiumPaidUptoDate.Value.Year;
                        var leaseYear = leaseDeedDate.Value.Year;
                        if (paidYear != leaseYear)
                        {
                            var diffYear = paidYear - leaseYear;
                            nextDueDate = leaseDeedDate.Value.AddYears(diffYear);
                            paidDueDate = leaseDeedDate.Value.AddYears(diffYear);
                        }

                        if (paidDueDate.Date > paidUptoDate.Value.Date)
                        {
                            var day = paidDueDate - paidUptoDate;
                            decimal? rinterest = (((total_amount * rate) / 100) / 365) * (day.Value.Days + 1);
                            total_amount = (total_amount + rinterest);
                            premiumdues = Math.Round((decimal)total_amount, 2);
                            interest = Math.Round((decimal)rinterest, 2);

                            nextDueDate = nextDueDate.AddYears(1);

                            htmldues = premium;
                            htmlinterest = interest;
                            printdate = nextDueDate.Day + "-" + nextDueDate.Month + "-" + nextDueDate.Year;
                            string fy = prevyear + "-" + nextyear;
                            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 2) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

                            LeaseRentViewModel rent = new LeaseRentViewModel();
                            rent.LeaseRentPremium = premium;
                            rent.RevisedPremium = premium;
                            rent.DuesAmount = premiumdues;
                            rent.PremiumInterest = interest;
                            rent.GstAmount = gstamount;
                            rent.PaidUptoDate = paidUptoDate;
                            rent.RevisedDate = revisedate;
                            rent.TotalPremium = total_amount;
                            rent.DuesUptoDate = nextDueDate;
                            //rentModel.DuesUptoDate = paidUptoDate;
                            //var duesdate = nextDueDate.Value.AddMonths(12);
                            rent.DepositDueDate = nextDueDate;
                            rent.PremiumPaidDuration = fy;
                            var dueshtml2 = dueshtml + "</div>";
                            rent.HtmlDuesReport = dueshtml2;

                            rentList.Add(rent);
                        }
                        else if (paidDueDate.Date < paidUptoDate.Value.Date)
                        {
                            var nextPaidDueDate = paidDueDate.AddYears(1);
                            var day = nextPaidDueDate - paidUptoDate;
                            decimal? rinterest = (((total_amount * rate) / 100) / 365) * (day.Value.Days + 1);
                            total_amount = (total_amount + rinterest);
                            premiumdues = Math.Round((decimal)total_amount, 2);
                            interest = Math.Round((decimal)rinterest, 2);

                            nextDueDate = nextPaidDueDate;

                            htmldues = premium;
                            htmlinterest = interest;
                            printdate = nextDueDate.Day + "-" + nextDueDate.Month + "-" + nextDueDate.Year;
                            string fy = prevyear + "-" + nextyear;
                            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 2) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

                            LeaseRentViewModel rent = new LeaseRentViewModel();
                            rent.LeaseRentPremium = premium;
                            rent.RevisedPremium = premium;
                            rent.DuesAmount = premiumdues;
                            rent.PremiumInterest = interest;
                            rent.GstAmount = gstamount;
                            rent.PaidUptoDate = paidUptoDate;
                            rent.RevisedDate = revisedate;
                            rent.TotalPremium = total_amount;
                            rent.DuesUptoDate = nextDueDate;
                            //rentModel.DuesUptoDate = paidUptoDate;
                            //var duesdate = nextDueDate.Value.AddMonths(12);
                            rent.DepositDueDate = nextDueDate;
                            rent.PremiumPaidDuration = fy;
                            var dueshtml2 = dueshtml + "</div>";
                            rent.HtmlDuesReport = dueshtml2;

                            rentList.Add(rent);
                        }
                        int count = 0;
                        while (today.Date > nextDueDate.Date)
                        {
                            LeaseRentViewModel rentModel = new LeaseRentViewModel();

                            if (nextDueDate >= revisedate)
                            {
                                total_amount = total_amount + ((decimal)premium + ((decimal)premium * reviserate) / 100);
                                var premiumnew = (decimal)premium + ((decimal)premium * reviserate) / 100;
                                premium = Math.Round((decimal)premiumnew, 2);
                                revisedate = revisedate.Value.AddYears(10);
                            }
                            else
                            {
                                decimal? rinterest = (((total_amount * rate) / 100));
                                total_amount = (total_amount + rinterest);
                                premiumdues = Math.Round((decimal)total_amount, 2);
                                interest = Math.Round((decimal)rinterest, 2);
                            }

                            //rentModel.LeaseRentPremium = premium;
                            //rentModel.RevisedPremium = premium;
                            //rentModel.RevisedDate = revisedate;
                            //rentModel.TotalPremium = total_amount;
                            //rentModel.DuesUptoDate = nextDueDate;

                            nextDueDate = nextDueDate.AddYears(1);

                            //if (today.Date < nextDueDate.Date && count < 1)
                            //{
                            //    today = today.AddYears(1);
                            //    count++;
                            //}

                            htmldues = premium;
                            htmlinterest = interest;
                            printdate = nextDueDate.Day + "-" + nextDueDate.Month + "-" + nextDueDate.Year;
                            string fy = prevyear + "-" + nextyear;
                            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 2) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

                            rentModel.LeaseRentPremium = premium;
                            rentModel.RevisedPremium = premium;
                            rentModel.DuesAmount = premiumdues;
                            rentModel.PremiumInterest = interest;
                            rentModel.GstAmount = gstamount;
                            rentModel.PaidUptoDate = paidUptoDate;
                            rentModel.RevisedDate = revisedate;
                            rentModel.TotalPremium = total_amount;
                            rentModel.DuesUptoDate = nextDueDate;
                            //rentModel.DuesUptoDate = paidUptoDate;
                            //var duesdate = nextDueDate.Value.AddMonths(12);
                            rentModel.DepositDueDate = nextDueDate;
                            rentModel.PremiumPaidDuration = fy;
                            var dueshtml2 = dueshtml + "</div>";
                            rentModel.HtmlDuesReport = dueshtml2;

                            rentList.Add(rentModel);
                        }

                        //nexttodate = paidUptoDate.Value.AddMonths(12);
                        //nexttodate = nextDueDate.AddMonths(12);

                        //if (revisedate.Value.Date < today.Date)
                        //{
                        //    revisedate = revisedate.Value.AddYears(10);
                        //}

                        //var next2date = nextDueDate;
                        //int count = 2;

                        //while (today > paidUptoDate)
                        //{
                        //    LeaseRentViewModel rentModel = new LeaseRentViewModel();
                        //    //rentModel.RegistrationId = model.RegistrationId;
                        //    //rentModel.DepartmentId = property.departmentId;
                        //    var p2date = paidUptoDate;
                        //    nextdate = paidUptoDate.Value.AddMonths(6);
                        //    var nextFYdate = paidUptoDate.Value.AddMonths(12);

                        //    //var next2date = nextDueDate;

                        //    //nextdate = next2date.AddMonths(12);
                        //    decimal? htmldues = 0;
                        //    decimal? htmlinterest = 0;
                        //    decimal? paidamount = 0;
                        //    string prevyear = string.Empty;
                        //    string nextyear = string.Empty;
                        //    if (nextdate.Value.Year > paidUptoDate.Value.Year)
                        //    {
                        //        prevyear = paidUptoDate.Value.Year.ToString();
                        //        nextyear = nextdate.Value.Year.ToString();
                        //    }
                        //    else
                        //    {
                        //        prevyear = (paidUptoDate.Value.Year - 1).ToString();
                        //        nextyear = paidUptoDate.Value.Year.ToString();
                        //    }


                        //    if (paidUptoDate >= nexttodate)
                        //    {
                        //        nexttodate = paidUptoDate.Value.AddMonths(12);

                        //        if (paidUptoDate >= revisedate)
                        //        {
                        //            total_amount = total_amount + ((decimal)premium + ((decimal)premium * reviserate) / 100);
                        //            var premiumnew = (decimal)premium + ((decimal)premium * reviserate) / 100;
                        //            premium = Math.Round((decimal)premiumnew, 2);
                        //            revisedate = revisedate.Value.AddYears(10);

                        //            T_amount = total_amount;

                        //        }
                        //        else
                        //        {
                        //            total_amount = total_amount + (decimal)premium;
                        //            rentModel.TotalPremium = total_amount;
                        //            T_amount = total_amount;
                        //        }

                        //        rentModel.LeaseRentPremium = premium;
                        //        rentModel.RevisedPremium = premium;
                        //        rentModel.RevisedDate = revisedate;
                        //        rentModel.TotalPremium = total_amount;
                        //    }

                        //    if (today > nextdate)
                        //    {
                        //        var rentPaidList = dbContext.ViewLeaseRentChecks.OrderByDescending(r => r.DEPOSIT_DATE).FirstOrDefault(r => r.RID_NO == model.RegistrationId.ToString() && r.RECEIPT_SUBHEAD_ID == 4 && (r.DEPOSIT_DATE >= paidUptoDate && r.DEPOSIT_DATE <= nextdate));
                        //        if (rentPaidList != null)
                        //        {
                        //            //var rentPaid = rentPaidList.FirstOrDefault();
                        //            var depositdate = rentPaidList.DEPOSIT_DATE;
                        //            var amountpaid = rentPaidList.AMOUNT_PAID;
                        //            var day = paidUptoDate - depositdate;
                        //            decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                        //            paidamount = amountpaid;
                        //            total_amount = (total_amount + rinterest) - amountpaid;
                        //            day = nextdate - depositdate;
                        //            rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                        //            total_amount = (total_amount + rinterest);
                        //            premiumdues = Math.Round((decimal)total_amount, 2);
                        //            interest = Math.Round((decimal)rinterest, 2);
                        //        }
                        //        else
                        //        {
                        //            var day = nextdate - paidUptoDate;
                        //            decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                        //            total_amount = (total_amount + rinterest);
                        //            premiumdues = Math.Round((decimal)total_amount, 2);
                        //            interest = Math.Round((decimal)rinterest, 2);
                        //        }
                        //        var pd1 = nextDueDate.AddYears(1);
                        //        nextDueDate = pd1;
                        //        paidUptoDate = nextdate;

                        //        //if (paidUptoDate.Value.Date == nextdate.Value.Date)
                        //        //{
                        //        //    paidUptoDate = nextdate.Value.AddYears(1);
                        //        //}

                        //        rentModel.TotalPremium = total_amount;
                        //        rentModel.PremiumInterest = interest;
                        //        rentModel.DuesAmount = premiumdues;
                        //    }
                        //    else
                        //    {
                        //        var rentPaidList = dbContext.ViewLeaseRentChecks.OrderByDescending(r => r.DEPOSIT_DATE).FirstOrDefault(r => r.RID_NO == model.RegistrationId.ToString() && r.RECEIPT_SUBHEAD_ID == 4 && (r.DEPOSIT_DATE >= paidUptoDate && r.DEPOSIT_DATE <= today));
                        //        if (rentPaidList != null)
                        //        {
                        //            //var rentPaid = rentPaidList.FirstOrDefault();
                        //            var depositdate = rentPaidList.DEPOSIT_DATE;
                        //            var amountpaid = rentPaidList.AMOUNT_PAID;
                        //            var day = paidUptoDate - depositdate;
                        //            decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                        //            total_amount = (total_amount + rinterest) - amountpaid;
                        //            day = nextdate - depositdate;
                        //            rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                        //            total_amount = (total_amount + rinterest);
                        //            premiumdues = Math.Round((decimal)total_amount, 2);
                        //            interest = Math.Round((decimal)rinterest, 2);
                        //        }
                        //        else
                        //        {
                        //            var day = paidUptoDate - nextdate;

                        //            if (paidUptoDate.Value.Date >= today.Date)
                        //            {
                        //                day = paidUptoDate - today;
                        //            }

                        //            decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                        //            total_amount = (total_amount + rinterest);
                        //            premiumdues = Math.Round((decimal)total_amount, 2);
                        //            interest = Math.Round((decimal)rinterest, 2);
                        //        }
                        //        p2date = paidUptoDate;
                        //        paidUptoDate = nextdate;
                        //        //if (paidUptoDate.Value.Date == nextdate.Value.Date)
                        //        //{
                        //        //    paidUptoDate = nextdate.Value.AddYears(1);
                        //        //}
                        //    }
                        //    if (nextFYdate < nextdate)
                        //    {
                        //        htmldues = premium;
                        //        htmlinterest = interest;
                        //        printdate = paidUptoDate.Value.Day + "-" + paidUptoDate.Value.Month + "-" + paidUptoDate.Value.Year;
                        //        string fy = prevyear + "-" + nextyear;
                        //        dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 2) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

                        //        rentModel.LeaseRentPremium = premium;
                        //        rentModel.DuesAmount = premiumdues;
                        //        rentModel.PremiumInterest = interest;
                        //        rentModel.GstAmount = gstamount;
                        //        rentModel.PaidUptoDate = paidUptoDate;
                        //        rentModel.RevisedDate = revisedate;
                        //        rentModel.DuesUptoDate = paidUptoDate;
                        //        var duesdate = paidUptoDate.Value.AddMonths(12);
                        //        rentModel.DepositDueDate = duesdate;
                        //        rentModel.PremiumPaidDuration = fy;
                        //        var dueshtml2 = dueshtml + "</div>";
                        //        rentModel.HtmlDuesReport = dueshtml2;

                        //        rentList.Add(rentModel);
                        //    }                            
                        //}

                        //dueshtml = dueshtml + "<tr><td>Noida Authority</td></tr>";

                        //var days2 = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                        //var today2 = DateTime.Now.AddDays(days2 - DateTime.Now.Day);

                        //model.LeaseRentPremium = premium;
                        //model.DuesAmount = premiumdues;
                        //model.PremiumInterest = interest;
                        //model.GstAmount = gstamount;
                        //model.PaidUptoDate = paidUptoDate;
                        //model.RevisedDate = revisedate;
                        //model.DuesUptoDate = today2;
                        //dueshtml = dueshtml + "</div>";
                        //model.HtmlDuesReport = dueshtml;

                        //var ct = rentList.Count;
                        //var lstRent = rentList.ElementAt(ct - 1);
                        //lstRent.DuesUptoDate = today2;
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

                return rentList;
            }
        }

        private List<LeaseRentViewModel> CalculateLeaseRentDuesII(LeaseRentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<LeaseRentViewModel> rentList = new List<LeaseRentViewModel>();
                //check in Lease Rent Payment Details
                var lease = dbContext.LeaseRentPayments.Where(r => r.RegistrationId == model.RegistrationId).OrderByDescending(d => d.Id).FirstOrDefault();
                var registry = dbContext.RegistryDetails.FirstOrDefault(r => r.Rid == model.RegistrationId);
                if (lease != null)
                {
                    string allottee = string.Empty;
                    string propertyno = string.Empty;
                    string dueshtml = string.Empty;
                    string printdate = string.Empty;

                    model.RentId = lease.Id;
                    var applicant = dbContext.ApplicationDetails.FirstOrDefault(r => r.registrationId == lease.RegistrationId);
                    var property = (from alot in dbContext.AllotmentMasters
                                    join prop in dbContext.SchemePropTrans on alot.propertyId equals prop.propertyId
                                    where alot.rid == lease.RegistrationId
                                    select prop).FirstOrDefault();

                    allottee = applicant != null ? applicant.tFirstName : string.Empty;

                    if (property != null)
                    {
                        propertyno = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo;
                    }
                    //string dueshtml = "<style>.print-row:nth-child(2n+1){background:#d5d5d5;}</style>";
                    dueshtml = "<div id='Report'><div class='row print-row'><div class='col-md-12 print-col' style='text-align:center'><h3>Property Lease Rent Report</h3></div></div>";
                    dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-4 print-col'><b>Allottee: " + allottee + "</b></div><div class='col-md-4 print-col' style='text-align:center'>Property: " + propertyno + "</div><div class='col-md-4 print-col' style='text-align:right'><b>Registration Id: " + lease.RegistrationId + "</b></div></div>";
                    dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>FY</div><div class='col-md-1 print-col'>PI</div><div class='col-md-2 print-col'>Lease Rent</div><div class='col-md-2 print-col'>Interest</div><div class='col-md-2 print-col'>Paid Amount</div><div class='col-md-2 print-col'>Total</div><div class='col-md-2 print-col'>Dues Upto</div></div>";

                    var leaseDeedDate = lease.TransferLeaseDate == null ? lease.LeaseDeedDate : lease.TransferLeaseDate;
                    var nextdate = lease.PremiumPaidUptoDate == null ? DateTime.Now : lease.PremiumPaidUptoDate;
                    var paidUptoDate = lease.PremiumPaidUptoDate == null ? leaseDeedDate : lease.PremiumPaidUptoDate;
                    var nextDueDate = leaseDeedDate.Value.AddYears(1);
                    var prevDueDate = leaseDeedDate.Value.AddYears(1);
                    var revisedate = lease.RevisedPremiumDate == null ? lease.LeaseDeedDate.Value.AddYears(10) : lease.RevisedPremiumDate;
                    decimal? reviserate = lease.RevisedRate == null ? 50 : lease.RevisedRate;
                    decimal premiumdues = 0;
                    decimal interest = 0;
                    decimal gstamount = 0;
                    decimal? total_amount = 0;

                    decimal? T_amount = 0;
                    decimal? T_interest = 0;

                    var today = DateTime.Now;
                    var nexttodate = today;
                    //var rentDueDate = leaseDeedDate.Value.AddYears(1);

                    if (lease.PremiumPaidUptoDate != null)
                    {
                        var premium = lease.PremiumLeaseRent == null ? 0 : lease.PremiumLeaseRent;
                        decimal? rate = lease.PanelInterest == null ? 0 : lease.PanelInterest;
                        decimal? gstrate = lease.GST == null ? 0 : lease.GST;
                        total_amount = (decimal)premium;
                        var paidDueDate = DateTime.Now;

                        decimal? htmldues = 0;
                        decimal? htmlinterest = 0;
                        decimal? paidamount = 0;
                        string prevyear = string.Empty;
                        string nextyear = string.Empty;

                        var paidYear = lease.PremiumPaidUptoDate.Value.Year;
                        var leaseYear = leaseDeedDate.Value.Year;
                        if (paidYear != leaseYear)
                        {
                            var diffYear = paidYear - leaseYear;
                            nextDueDate = leaseDeedDate.Value.AddYears(diffYear);
                            paidDueDate = leaseDeedDate.Value.AddYears(diffYear);
                        }

                        if (paidDueDate.Date > paidUptoDate.Value.Date)
                        {
                            var day = paidDueDate - paidUptoDate;
                            decimal? rinterest = (((total_amount * rate) / 100) / 365) * (day.Value.Days + 1);
                            total_amount = (total_amount + rinterest);
                            premiumdues = Math.Round((decimal)total_amount, 2);
                            interest = Math.Round((decimal)rinterest, 2);

                            nextDueDate = nextDueDate.AddYears(1);

                            htmldues = premium;
                            htmlinterest = interest;
                            printdate = nextDueDate.Day + "-" + nextDueDate.Month + "-" + nextDueDate.Year;
                            string fy = prevyear + "-" + nextyear;
                            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 2) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

                            LeaseRentViewModel rent = new LeaseRentViewModel();
                            rent.LeaseRentPremium = premium;
                            rent.RevisedPremium = premium;
                            rent.DuesAmount = premiumdues;
                            rent.PremiumInterest = interest;
                            rent.GstAmount = gstamount;
                            rent.PaidUptoDate = paidUptoDate;
                            rent.RevisedDate = revisedate;
                            rent.TotalPremium = total_amount;
                            rent.DuesUptoDate = nextDueDate;
                            //rentModel.DuesUptoDate = paidUptoDate;
                            //var duesdate = nextDueDate.Value.AddMonths(12);
                            rent.DepositDueDate = nextDueDate;
                            rent.PremiumPaidDuration = fy;
                            var dueshtml2 = dueshtml + "</div>";
                            rent.HtmlDuesReport = dueshtml2;

                            rentList.Add(rent);
                        }
                        else if (paidDueDate.Date < paidUptoDate.Value.Date)
                        {
                            var nextPaidDueDate = paidDueDate.AddYears(1);
                            var day = nextPaidDueDate - paidUptoDate;
                            decimal? rinterest = (((total_amount * rate) / 100) / 365) * (day.Value.Days + 1);
                            total_amount = (total_amount + rinterest);
                            premiumdues = Math.Round((decimal)total_amount, 2);
                            interest = Math.Round((decimal)rinterest, 2);

                            nextDueDate = nextPaidDueDate;

                            htmldues = premium;
                            htmlinterest = interest;
                            printdate = nextDueDate.Day + "-" + nextDueDate.Month + "-" + nextDueDate.Year;
                            string fy = prevyear + "-" + nextyear;
                            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 2) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

                            LeaseRentViewModel rent = new LeaseRentViewModel();
                            rent.LeaseRentPremium = premium;
                            rent.RevisedPremium = premium;
                            rent.DuesAmount = premiumdues;
                            rent.PremiumInterest = interest;
                            rent.GstAmount = gstamount;
                            rent.PaidUptoDate = paidUptoDate;
                            rent.RevisedDate = revisedate;
                            rent.TotalPremium = total_amount;
                            rent.DuesUptoDate = nextDueDate;
                            //rentModel.DuesUptoDate = paidUptoDate;
                            //var duesdate = nextDueDate.Value.AddMonths(12);
                            rent.DepositDueDate = nextDueDate;
                            rent.PremiumPaidDuration = fy;
                            var dueshtml2 = dueshtml + "</div>";
                            rent.HtmlDuesReport = dueshtml2;

                            rentList.Add(rent);
                        }
                        int count = 0;
                        while (today.Date > nextDueDate.Date)
                        {
                            LeaseRentViewModel rentModel = new LeaseRentViewModel();

                            if (nextDueDate >= revisedate)
                            {
                                total_amount = total_amount + ((decimal)premium + ((decimal)premium * reviserate) / 100);
                                var premiumnew = (decimal)premium + ((decimal)premium * reviserate) / 100;
                                premium = Math.Round((decimal)premiumnew, 2);
                                revisedate = revisedate.Value.AddYears(10);
                            }
                            else
                            {
                                decimal? rinterest = (((total_amount * rate) / 100));
                                total_amount = (total_amount + rinterest);
                                premiumdues = Math.Round((decimal)total_amount, 2);
                                interest = Math.Round((decimal)rinterest, 2);
                            }

                            nextDueDate = nextDueDate.AddYears(1);

                            htmldues = premium;
                            htmlinterest = interest;
                            printdate = nextDueDate.Day + "-" + nextDueDate.Month + "-" + nextDueDate.Year;
                            string fy = prevyear + "-" + nextyear;
                            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 2) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

                            rentModel.LeaseRentPremium = premium;
                            rentModel.RevisedPremium = premium;
                            rentModel.DuesAmount = premiumdues;
                            rentModel.PremiumInterest = interest;
                            rentModel.GstAmount = gstamount;
                            rentModel.PaidUptoDate = paidUptoDate;
                            rentModel.RevisedDate = revisedate;
                            rentModel.TotalPremium = total_amount;
                            rentModel.DuesUptoDate = nextDueDate;
                            //rentModel.DuesUptoDate = paidUptoDate;
                            //var duesdate = nextDueDate.Value.AddMonths(12);
                            rentModel.DepositDueDate = nextDueDate;
                            rentModel.PremiumPaidDuration = fy;
                            var dueshtml2 = dueshtml + "</div>";
                            rentModel.HtmlDuesReport = dueshtml2;

                            rentList.Add(rentModel);
                        }

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

                return rentList;
            }
        }

        private List<LeaseRentViewModel> CheckLeaseRentDuesPayment(LeaseRentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var rentPaidList = dbContext.ViewLeaseRentChecks.OrderByDescending(r => r.DEPOSIT_DATE).FirstOrDefault(r => r.RID_NO == model.RegistrationId.ToString() && r.RECEIPT_SUBHEAD_ID == 4 && (r.DEPOSIT_DATE >= model.PaidUptoDate && r.DEPOSIT_DATE <= DateTime.Now));
                if (rentPaidList != null)
                {
                    //var rentPaid = rentPaidList.FirstOrDefault();
                    var depositdate = rentPaidList.DEPOSIT_DATE;
                    var amountpaid = rentPaidList.AMOUNT_PAID;
                    //var day = paidUptoDate - depositdate;
                    //decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                    //total_amount = (total_amount + rinterest) - amountpaid;
                    //day = nextdate - depositdate;
                    //rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                    //total_amount = (total_amount + rinterest);
                    //premiumdues = Math.Round((decimal)total_amount, 2);
                    //interest = Math.Round((decimal)rinterest, 2);
                }
            }
            return null;
        }

        //private List<LeaseRentViewModel> CalculateLeaseRentDues(LeaseRentViewModel model)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        List<LeaseRentViewModel> rentList = new List<LeaseRentViewModel>();
        //        //check in Lease Rent Payment Details
        //        var leasemaster = dbContext.LeaseRentPayments.Where(r => r.RegistrationId == model.RegistrationId).OrderByDescending(d => d.Id).ToList();
        //        var registry = dbContext.RegistryDetails.FirstOrDefault(r => r.Rid == model.RegistrationId);
        //        if (leasemaster != null && leasemaster.Count > 0)
        //        {
        //            string allottee = string.Empty;
        //            string propertyno = string.Empty;
        //            string dueshtml = string.Empty;
        //            string printdate = string.Empty;

        //            decimal premiumdues = 0;
        //            decimal interest = 0;
        //            decimal gstamount = 0;
        //            decimal? total_amount = 0;

        //            var today = DateTime.Now;
        //            var nexttodate = today;

        //            var lease = leasemaster.FirstOrDefault();
        //            model.RentId = lease.Id;
        //            var applicant = dbContext.ApplicationDetails.FirstOrDefault(r => r.registrationId == lease.RegistrationId);
        //            var property = (from alot in dbContext.AllotmentMasters
        //                            join prop in dbContext.SchemePropTrans on alot.propertyId equals prop.propertyId
        //                            where alot.rid == lease.RegistrationId
        //                            select prop).FirstOrDefault();

        //            allottee = applicant != null ? applicant.tFirstName : string.Empty;

        //            if (property != null)
        //            {
        //                propertyno = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo;
        //            }
        //            //string dueshtml = "<style>.print-row:nth-child(2n+1){background:#d5d5d5;}</style>";
        //            dueshtml = "<div id='Report'><div class='row print-row'><div class='col-md-12 print-col' style='text-align:center'><h3>Property Lease Rent Report</h3></div></div>";
        //            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-4 print-col'><b>Allottee: " + allottee + "</b></div><div class='col-md-4 print-col' style='text-align:center'>Property: " + propertyno + "</div><div class='col-md-4 print-col' style='text-align:right'><b>Registration Id: " + lease.RegistrationId + "</b></div></div>";
        //            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>FY</div><div class='col-md-1 print-col'>PI</div><div class='col-md-2 print-col'>Lease Rent</div><div class='col-md-2 print-col'>Interest</div><div class='col-md-2 print-col'>Paid Amount</div><div class='col-md-2 print-col'>Total</div><div class='col-md-2 print-col'>Dues Upto</div></div>";

        //            var leaseDeedDate = lease.TransferLeaseDate == null ? lease.LeaseDeedDate : lease.TransferLeaseDate;
        //            var nextdate = lease.PremiumPaidUptoDate == null ? DateTime.Now : lease.PremiumPaidUptoDate;
        //            var paidUptoDate = lease.PremiumPaidUptoDate;
        //            var nextDueDate = lease.PremiumPaidUptoDate.Value.AddYears(1);
        //            var revisedate = lease.RevisedPremiumDate == null ? lease.LeaseDeedDate : lease.RevisedPremiumDate;                   
        //            decimal? reviserate = lease.RevisedRate == null ? 0 : lease.RevisedRate;

        //            var paidYear = lease.PremiumPaidUptoDate.Value.Year;
        //            var leaseYear = leaseDeedDate.Value.Year;
        //            if (paidYear != leaseYear)
        //            {
        //                 var diffYear = paidYear - leaseYear;
        //                 nextDueDate = lease.LeaseDeedDate.Value.AddYears(diffYear);
        //                 if (DbFunctions.TruncateTime(lease.PremiumPaidUptoDate) < DbFunctions.TruncateTime(nextDueDate))
        //                 {

        //                 }
        //            }

        //            var rentDueDate = leaseDeedDate.Value.AddYears(1);

        //            if (lease.PremiumPaidUptoDate != null)
        //            {
        //                var premium = lease.PremiumLeaseRent == null ? 0 : lease.PremiumLeaseRent;
        //                decimal? rate = lease.PanelInterest == null ? 0 : lease.PanelInterest;
        //                decimal? gstrate = lease.GST == null ? 0 : lease.GST;
        //                total_amount = (decimal)premium;
        //                nexttodate = paidUptoDate.Value.AddMonths(12);

        //                if (revisedate.Value.Date < today.Date)
        //                {
        //                    revisedate = revisedate.Value.AddYears(10);
        //                }

        //                while (today > paidUptoDate)
        //                {
        //                    LeaseRentViewModel rentModel = new LeaseRentViewModel();
        //                    //rentModel.RegistrationId = model.RegistrationId;
        //                    //rentModel.DepartmentId = property.departmentId;

        //                    nextdate = paidUptoDate.Value.AddMonths(6);
        //                    decimal? htmldues = 0;
        //                    decimal? htmlinterest = 0;
        //                    decimal? paidamount = 0;
        //                    string prevyear = string.Empty;
        //                    string nextyear = string.Empty;
        //                    if (nextdate.Value.Year > paidUptoDate.Value.Year)
        //                    {
        //                        prevyear = paidUptoDate.Value.Year.ToString();
        //                        nextyear = nextdate.Value.Year.ToString();
        //                    }
        //                    else
        //                    {
        //                        prevyear = (paidUptoDate.Value.Year - 1).ToString();
        //                        nextyear = paidUptoDate.Value.Year.ToString();
        //                    }


        //                    if (paidUptoDate >= nexttodate)
        //                    {
        //                        nexttodate = paidUptoDate.Value.AddMonths(12);

        //                        if (paidUptoDate >= revisedate)
        //                        {
        //                            total_amount = total_amount + ((decimal)premium + ((decimal)premium * reviserate) / 100);
        //                            var premiumnew = (decimal)premium + ((decimal)premium * reviserate) / 100;
        //                            premium = Math.Round((decimal)premiumnew, 2);
        //                            revisedate = revisedate.Value.AddYears(10);                                    
        //                        }
        //                        else
        //                        {
        //                            total_amount = total_amount + (decimal)premium;
        //                            rentModel.TotalPremium = total_amount;
        //                        }

        //                        rentModel.LeaseRentPremium = premium;
        //                        rentModel.RevisedPremium = premium;
        //                        rentModel.RevisedDate = revisedate;
        //                        rentModel.TotalPremium = total_amount;
        //                    }

        //                    if (today > nextdate)
        //                    {
        //                        var rentPaidList = dbContext.ViewLeaseRentChecks.OrderByDescending(r => r.DEPOSIT_DATE).FirstOrDefault(r => r.RID_NO == model.RegistrationId.ToString() && r.RECEIPT_SUBHEAD_ID == 4 && (r.DEPOSIT_DATE >= paidUptoDate && r.DEPOSIT_DATE <= nextdate));
        //                        if (rentPaidList != null)
        //                        {
        //                            //var rentPaid = rentPaidList.FirstOrDefault();
        //                            var depositdate = rentPaidList.DEPOSIT_DATE;
        //                            var amountpaid = rentPaidList.AMOUNT_PAID;
        //                            var day = paidUptoDate - depositdate;
        //                            decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
        //                            paidamount = amountpaid;
        //                            total_amount = (total_amount + rinterest) - amountpaid;
        //                            day = nextdate - depositdate;
        //                            rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
        //                            total_amount = (total_amount + rinterest);
        //                            premiumdues = Math.Round((decimal)total_amount, 2);
        //                            interest = Math.Round((decimal)rinterest, 2);                                   
        //                        }
        //                        else
        //                        {
        //                            var day = nextdate - paidUptoDate;
        //                            decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
        //                            total_amount = (total_amount + rinterest);
        //                            premiumdues = Math.Round((decimal)total_amount, 2);
        //                            interest = Math.Round((decimal)rinterest, 2);
        //                        }
        //                        paidUptoDate = nextdate;

        //                        rentModel.TotalPremium = total_amount;
        //                        rentModel.PremiumInterest = interest;
        //                        rentModel.DuesAmount = premiumdues;
        //                    }
        //                    else
        //                    {
        //                        var rentPaidList = dbContext.ViewLeaseRentChecks.OrderByDescending(r => r.DEPOSIT_DATE).FirstOrDefault(r => r.RID_NO == model.RegistrationId.ToString() && r.RECEIPT_SUBHEAD_ID == 4 && (r.DEPOSIT_DATE >= paidUptoDate && r.DEPOSIT_DATE <= today));
        //                        if (rentPaidList != null)
        //                        {
        //                            //var rentPaid = rentPaidList.FirstOrDefault();
        //                            var depositdate = rentPaidList.DEPOSIT_DATE;
        //                            var amountpaid = rentPaidList.AMOUNT_PAID;
        //                            var day = paidUptoDate - depositdate;
        //                            decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
        //                            total_amount = (total_amount + rinterest) - amountpaid;
        //                            day = nextdate - depositdate;
        //                            rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
        //                            total_amount = (total_amount + rinterest);
        //                            premiumdues = Math.Round((decimal)total_amount, 2);
        //                            interest = Math.Round((decimal)rinterest, 2);
        //                        }
        //                        else
        //                        {
        //                            var day = paidUptoDate - nextdate;

        //                            if (paidUptoDate.Value.Date >= today.Date)
        //                            {
        //                                day = paidUptoDate - today;
        //                            }

        //                            decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
        //                            total_amount = (total_amount + rinterest);
        //                            premiumdues = Math.Round((decimal)total_amount, 2);
        //                            interest = Math.Round((decimal)rinterest, 2);
        //                        }
        //                        paidUptoDate = nextdate;
        //                    }
        //                    htmldues = premium;
        //                    htmlinterest = interest;
        //                    printdate = paidUptoDate.Value.Day + "-" + paidUptoDate.Value.Month + "-" + paidUptoDate.Value.Year;
        //                    string fy = prevyear + "-" + nextyear;
        //                    dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 2) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

        //                    rentModel.LeaseRentPremium = premium;
        //                    rentModel.DuesAmount = premiumdues;
        //                    rentModel.PremiumInterest = interest;
        //                    rentModel.GstAmount = gstamount;
        //                    rentModel.PaidUptoDate = paidUptoDate;
        //                    rentModel.RevisedDate = revisedate;
        //                    rentModel.DuesUptoDate = paidUptoDate;
        //                    var duesdate = paidUptoDate.Value.AddMonths(12);
        //                    rentModel.DepositDueDate = duesdate;
        //                    rentModel.PremiumPaidDuration = fy;
        //                    var dueshtml2 = dueshtml + "</div>";
        //                    rentModel.HtmlDuesReport = dueshtml2;

        //                    rentList.Add(rentModel);
        //                }

        //                //dueshtml = dueshtml + "<tr><td>Noida Authority</td></tr>";

        //                var days2 = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        //                var today2 = DateTime.Now.AddDays(days2 - DateTime.Now.Day);

        //                model.LeaseRentPremium = premium;
        //                model.DuesAmount = premiumdues;
        //                model.PremiumInterest = interest;
        //                model.GstAmount = gstamount;
        //                model.PaidUptoDate = paidUptoDate;
        //                model.RevisedDate = revisedate;
        //                model.DuesUptoDate = today2;
        //                dueshtml = dueshtml + "</div>";
        //                model.HtmlDuesReport = dueshtml;

        //                var ct = rentList.Count;
        //                var lstRent = rentList.ElementAt(ct-1);
        //                lstRent.DuesUptoDate = today2;
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

        //        return rentList;
        //    }           
        //}

        private LeaseRentViewModel CalculateInstallmentDues(LeaseRentViewModel model)
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
                        var predate = data.BalanceUptoDate;
                        var today = DateTime.Now;
                        var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                        today = DateTime.Now.AddDays(days - DateTime.Now.Day);

                        var nextdate = data.BalanceUptoDate == null ? DateTime.Now : data.BalanceUptoDate;
                        decimal duesAmount = 0;
                        decimal gsamount = 0;
                        decimal totalInterest = 0;
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
                        dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-2 print-col'>FY</div><div class='col-md-1 print-col'>PI</div><div class='col-md-2 print-col'>Amount</div><div class='col-md-2 print-col'>Interest</div><div class='col-md-2 print-col'>Paid Amount</div><div class='col-md-2 print-col'>Total</div><div class='col-md-1 print-col'>Dues Upto</div></div>";


                        if (balanceUptoDate.Value.Date < today.Date)
                        {
                            var balanceAmount = data.BalanceAmount == null ? 0 : data.BalanceAmount;
                            var gst = data.GST == null ? 0 : data.GST;
                            while (balanceUptoDate.Value.Date < today.Date)
                            {
                                //var balanceAmount = data.BalanceAmount == null ? 0 : data.BalanceAmount;
                                //var gst = data.GST == null ? 0 : data.GST;
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
                                        //var amount = balanceAmount;
                                        var interest = (balanceAmount * (rate / 2) / 100);
                                        var amount = balanceAmount + interest;
                                        var gstinterest = (decimal)amount * gst / 100;
                                        amount = (decimal)amount + (decimal)interest + (decimal)gstinterest;

                                        duesAmount = Math.Round((decimal)amount, 2);
                                        gsamount = Math.Round((decimal)gstinterest, 2);
                                        interest = Math.Round((decimal)interest, 2);

                                        htmldues = duesAmount;
                                        htmlbalance = Math.Round((decimal)balanceAmount, 2);
                                        htmlinterest = interest;

                                        balanceAmount = amount;
                                        predate = balanceUptoDate;
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
                                            htmlbalance = Math.Round((decimal)balanceAmount, 2);
                                            htmlinterest = interest;

                                            balanceAmount = total;
                                            predate = balanceUptoDate;
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
                                            htmlbalance = Math.Round((decimal)balanceAmount, 2);
                                            htmlinterest = interest;

                                            balanceAmount = total;
                                            predate = balanceUptoDate;
                                            int mnth = 12 - balanceUptoDate.Value.Month;
                                            balanceUptoDate = balanceUptoDate.Value.AddMonths(mnth);
                                        }
                                    }

                                    //previousYear = (balanceUptoDate.Value.Year - 1).ToString();
                                    //nextYear = balanceUptoDate.Value.Year.ToString();
                                    predate = predate.Value.AddDays(1);
                                    previousYear = predate.Value.Day + "/" + predate.Value.Month + "-/" + predate.Value.Year;
                                    nextYear = balanceUptoDate.Value.Day + "/" + balanceUptoDate.Value.Month + "/" + balanceUptoDate.Value.Year;
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
                                        htmlbalance = Math.Round((decimal)balanceAmount, 2);
                                        htmlinterest = interest;

                                        balanceAmount = total;
                                        predate = balanceUptoDate;
                                        balanceUptoDate = balanceUptoDate.Value.AddYears(1);
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
                                            htmlbalance = Math.Round((decimal)balanceAmount, 2);
                                            htmlinterest = interest;

                                            balanceAmount = total;
                                            predate = balanceUptoDate;
                                            int mnth = 6 - balanceUptoDate.Value.Month;
                                            balanceUptoDate = mnth == 0 ? today : balanceUptoDate.Value.AddMonths(mnth);
                                            //balanceUptoDate = balanceUptoDate.Value.AddMonths(mnth);
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
                                            htmlbalance = Math.Round((decimal)balanceAmount, 2);
                                            htmlinterest = interest;

                                            balanceAmount = total;
                                            predate = balanceUptoDate;
                                            int mnth = 12 - balanceUptoDate.Value.Month;
                                            balanceUptoDate = mnth == 0 ? today : balanceUptoDate.Value.AddMonths(mnth);
                                            //balanceUptoDate = balanceUptoDate.Value.AddMonths(mnth);
                                        }
                                    }

                                    //previousYear = balanceUptoDate.Value.Year.ToString();
                                    //nextYear = nextdate.Value.Year.ToString();
                                    predate = predate.Value.AddDays(1);
                                    previousYear = predate.Value.Day + "/" + predate.Value.Month + "/" + predate.Value.Year;
                                    nextYear = nextdate.Value.Day + "/" + nextdate.Value.Month + "/" + nextdate.Value.Year;
                                }

                                //htmldues = balanceAmount;
                                //htmlinterest = htmlinterest;
                                htmldate = balanceUptoDate.Value.Day + "-" + balanceUptoDate.Value.Month + "-" + balanceUptoDate.Value.Year;
                                string fy = previousYear + "-" + nextYear;
                                dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-2 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmlbalance + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + htmlpaidamount + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-1 print-col'>" + htmldate + "</div></div>";

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

        private LeaseRentViewModel CalculateInstallmentDuesII(LeaseRentViewModel model)
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

                        var toNextDate = data.BalanceUptoDate == null ? DateTime.Now : data.BalanceUptoDate;
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
                                toNextDate = balanceUptoDate.Value.AddMonths(12);
                                nextdate = balanceUptoDate.Value.AddMonths(6);

                                decimal? htmldues = 0;
                                decimal? htmlbalance = 0;
                                decimal? htmlinterest = 0;
                                decimal? htmlpaidamount = 0;
                                string previousYear = string.Empty;
                                string nextYear = string.Empty;
                                string htmldate = string.Empty;

                                if (today > toNextDate)
                                {
                                    var dues = GetInstallmentDuesByDate((int)model.RegistrationId, (decimal)balanceAmount, (decimal)rate, (decimal)gst, (DateTime)balanceUptoDate, (DateTime)toNextDate, "yearly");
                                    var payment = GetPaidInstallmentDetail((int)model.RegistrationId, (DateTime)balanceUptoDate, (DateTime)toNextDate);
                                    if (payment.PaidAmount != null)
                                    {
                                        duesAmount = (decimal)dues.BalanceAmount - (decimal)payment.PaidAmount;
                                    }
                                    else
                                    {
                                        var duess = GetInstallmentDuesByDate((int)model.RegistrationId, (decimal)balanceAmount, (decimal)rate, (decimal)gst, (DateTime)balanceUptoDate, (DateTime)toNextDate, "yearly");
                                    }
                                    balanceUptoDate = balanceUptoDate.Value.AddMonths(12);
                                }
                                else
                                {
                                    if (today > nextdate)
                                    {
                                        var dues = GetInstallmentDuesByDate((int)model.RegistrationId, (decimal)balanceAmount, (decimal)rate / 2, (decimal)gst, (DateTime)balanceUptoDate, (DateTime)nextdate, "yearly");
                                        var payment = GetPaidInstallmentDetail((int)model.RegistrationId, (DateTime)balanceUptoDate, (DateTime)nextdate);
                                        balanceUptoDate = balanceUptoDate.Value.AddMonths(6);
                                    }
                                    else
                                    {
                                        var dues = GetInstallmentDuesByDate((int)model.RegistrationId, (decimal)balanceAmount, (decimal)rate, (decimal)gst, (DateTime)balanceUptoDate, DateTime.Now.Date, "perday");
                                        var payment = GetPaidInstallmentDetail((int)model.RegistrationId, (DateTime)balanceUptoDate, DateTime.Now.Date);
                                        balanceUptoDate = today;
                                    }

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
                    }
                }
            }
            return model;
        }

        private List<PaymentViewModel> CalculateInstallmentDuesIII(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<PaymentViewModel> installmentDuesList = new List<PaymentViewModel>();
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

                        var toNextDate = data.BalanceUptoDate == null ? DateTime.Now : data.BalanceUptoDate;
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
                                PaymentViewModel payModel = new PaymentViewModel();
                                var balanceAmount = data.BalanceAmount == null ? 0 : data.BalanceAmount;
                                var gst = data.GST == null ? 0 : data.GST;
                                toNextDate = balanceUptoDate.Value.AddMonths(12);
                                nextdate = balanceUptoDate.Value.AddMonths(6);

                                decimal? htmldues = 0;
                                decimal? htmlbalance = 0;
                                decimal? htmlinterest = 0;
                                decimal? htmlpaidamount = 0;
                                string previousYear = string.Empty;
                                string nextYear = string.Empty;
                                string htmldate = string.Empty;

                                if (today > toNextDate)
                                {
                                    var dues = GetInstallmentDuesByDate((int)model.RegistrationId, (decimal)balanceAmount, (decimal)rate, (decimal)gst, (DateTime)balanceUptoDate, (DateTime)toNextDate, "yearly");
                                    var payment = GetPaidInstallmentDetail((int)model.RegistrationId, (DateTime)balanceUptoDate, (DateTime)toNextDate);
                                    //if (payment.PaidAmount != null)
                                    //{
                                    //    duesAmount = (decimal)dues.BalanceAmount - (decimal)payment.PaidAmount;                               
                                    //}
                                    //else
                                    //{
                                    //    var duess = GetInstallmentDuesByDate((int)model.RegistrationId, (decimal)balanceAmount, (decimal)rate, (decimal)gst, (DateTime)balanceUptoDate, (DateTime)toNextDate, "yearly");
                                    //    duesAmount = (decimal)duess.BalanceAmount;
                                    //}
                                    //duesAmount = payment.PaidAmount != null ? ((decimal)dues.BalanceAmount - (decimal)payment.PaidAmount) : (decimal)dues.BalanceAmount; 
                                    duesAmount = payment.PaidAmount != null ? ((decimal)dues.DuesAmount - (decimal)payment.PaidAmount) : (decimal)dues.BalanceAmount;
                                    payModel.DuesAmount = duesAmount;
                                    payModel.InterestAmount = dues.InterestAmount;
                                    payModel.GSTAmount = dues.GSTAmount;
                                    payModel.BalanceInterest = dues.BalanceInterest;
                                    payModel.DuesUptoDate = toNextDate;
                                    balanceUptoDate = balanceUptoDate.Value.AddMonths(12);
                                }
                                else
                                {
                                    if (today > nextdate)
                                    {
                                        var dues = GetInstallmentDuesByDate((int)model.RegistrationId, (decimal)balanceAmount, (decimal)rate / 2, (decimal)gst, (DateTime)balanceUptoDate, (DateTime)nextdate, "yearly");
                                        var payment = GetPaidInstallmentDetail((int)model.RegistrationId, (DateTime)balanceUptoDate, (DateTime)nextdate);

                                        duesAmount = payment.PaidAmount != null ? ((decimal)dues.DuesAmount - (decimal)payment.PaidAmount) : (decimal)dues.BalanceAmount;
                                        payModel.DuesAmount = duesAmount;
                                        payModel.InterestAmount = dues.InterestAmount;
                                        payModel.GSTAmount = dues.GSTAmount;
                                        payModel.BalanceInterest = dues.BalanceInterest;
                                        payModel.DuesUptoDate = nextdate;
                                        balanceUptoDate = balanceUptoDate.Value.AddMonths(6);
                                    }
                                    else
                                    {
                                        var dues = GetInstallmentDuesByDate((int)model.RegistrationId, (decimal)balanceAmount, (decimal)rate, (decimal)gst, (DateTime)balanceUptoDate, DateTime.Now.Date, "perday");
                                        var payment = GetPaidInstallmentDetail((int)model.RegistrationId, (DateTime)balanceUptoDate, DateTime.Now.Date);

                                        duesAmount = payment.PaidAmount != null ? ((decimal)dues.DuesAmount - (decimal)payment.PaidAmount) : (decimal)dues.BalanceAmount;
                                        payModel.DuesAmount = duesAmount;
                                        payModel.InterestAmount = dues.InterestAmount;
                                        payModel.GSTAmount = dues.GSTAmount;
                                        payModel.BalanceInterest = dues.BalanceInterest;
                                        payModel.DuesUptoDate = nextdate;
                                        balanceUptoDate = today;
                                    }

                                }
                                htmldate = balanceUptoDate.Value.Day + "-" + balanceUptoDate.Value.Month + "-" + balanceUptoDate.Value.Year;
                                string fy = previousYear + "-" + nextYear;
                                dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmlbalance + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + htmlpaidamount + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmldate + "</div></div>";

                                payModel.HtmlTemplate = dueshtml;

                                installmentDuesList.Add(payModel);
                            }
                            //var days2 = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                            //var today2 = DateTime.Now.AddDays(days2 - DateTime.Now.Day);

                            //model.LeaseRentPremium = duesAmount;
                            //model.GSTAmount = gsamount;
                            //model.BalanceUptoDate = balanceUptoDate;
                            //model.DuesUptoDate = today2;
                        }
                        else
                        {
                            //model.LeaseRentPremium = data.BalanceAmount;
                            //model.GSTAmount = 0;
                            //model.BalanceUptoDate = data.BalanceUptoDate;
                        }
                    }
                }
                return installmentDuesList;
            }
        }

        private List<PaymentViewModel> CalculateInstallmentDuesIV(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<PaymentViewModel> installmentDuesList = new List<PaymentViewModel>();
                var ScheduleDetail = (from schedule in dbContext.PaymentScheduleMasters
                                      join trans in dbContext.PaymentScheduleTrans on schedule.ScheduleId equals trans.ScheduleId into details
                                      where schedule.Rid == model.RegistrationId
                                      select details).ToList();

                var PaymentDetail = dbContext.PaymentReceiptMsts.Where(r => r.RegistrationId == model.RegistrationId).ToList();
                //decimal? totalbalance = 0;
                //decimal? penalty = 0;
                decimal? totaldues = 0;
                List<int?> HeadList = new List<int?>();
                List<int?> SubHeadList = new List<int?>();
                HeadList.Add(1);
                HeadList.Add(2);
                SubHeadList.Add(1);
                SubHeadList.Add(2);
                SubHeadList.Add(187);
                SubHeadList.Add(191);
                SubHeadList.Add(192);
                SubHeadList.Add(186);
                SubHeadList.Add(166);
                SubHeadList.Add(162);
                if (ScheduleDetail != null && ScheduleDetail.Count > 0 && PaymentDetail != null && PaymentDetail.Count > 0)
                {
                    var scheduleTransList = ScheduleDetail[0].ToList();
                    //var receiptlist = PaymentDetail[0].ToList();
                    for (int i = 0; i < scheduleTransList.Count; i++)
                    {
                        if (scheduleTransList[i].InstallmentDueDate != null)
                        {
                            DateTime? predate = null;
                            DateTime? postdate = null;
                            decimal? paid = 0;
                            decimal? dues = 0;
                            //decimal? currentbalance = 0;
                            decimal? currentdues = 0;
                            if (i == 0)
                            {
                                predate = scheduleTransList[i].InstallmentDueDate;
                                dues = scheduleTransList[i].InstallmentAmount + scheduleTransList[i].InterestAmount;
                                //paid = receiptlist.Where(w => w.DepositDate.Value.Date <= predate.Value.Date).Sum(s => s.DepositAmount);
                                var payreceipt = (from receipt in dbContext.PaymentReceiptMsts
                                                  join trans in dbContext.PaymentReceiptTrans on receipt.Id equals trans.ReceiptRefId
                                                  where receipt.RegistrationId == model.RegistrationId && DbFunctions.TruncateTime(trans.DepositDate) <= DbFunctions.TruncateTime(predate) && HeadList.Contains(trans.ReceiptHeadId) && SubHeadList.Contains(trans.ReceiptSubHeadId)
                                                  select trans).ToList();
                                if (payreceipt.Count > 0)
                                {
                                    paid = payreceipt.Sum(s => s.DepositAmount);
                                }
                                currentdues = dues - paid;
                                int scheduleTransId = scheduleTransList[i].Id;
                                var scheduletrans = dbContext.PaymentScheduleTrans.FirstOrDefault(t => t.Id == scheduleTransId);
                                scheduletrans.CurrentDues = currentdues;
                                scheduletrans.IsInstallmentPaid = (currentdues == 0 || currentdues < 0) ? true : false;
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                predate = scheduleTransList[i - 1].InstallmentDueDate;
                                postdate = scheduleTransList[i].InstallmentDueDate;
                                decimal? previousdues = scheduleTransList[i - 1].CurrentDues;
                                dues = scheduleTransList[i].InstallmentAmount + scheduleTransList[i].InterestAmount;
                                var payreceipt = (from receipt in dbContext.PaymentReceiptMsts
                                                  join trans in dbContext.PaymentReceiptTrans on receipt.Id equals trans.ReceiptRefId
                                                  where receipt.RegistrationId == model.RegistrationId && DbFunctions.TruncateTime(trans.DepositDate) > DbFunctions.TruncateTime(predate) && DbFunctions.TruncateTime(trans.DepositDate) <= DbFunctions.TruncateTime(postdate) && HeadList.Contains(trans.ReceiptHeadId) && SubHeadList.Contains(trans.ReceiptSubHeadId)
                                                  select trans).ToList();
                                if (payreceipt.Count > 0)
                                {
                                    paid = payreceipt.Sum(s => s.DepositAmount);
                                }
                                decimal? previousPenalty = previousdues > 0 ? (previousdues * 7) / 100 : 0;
                                previousdues = previousdues + previousPenalty;
                                currentdues = ((dues + previousdues) - paid);
                                int preScheduleTransId = scheduleTransList[i - 1].Id;
                                int scheduleTransId = scheduleTransList[i].Id;
                                var preScheduleTrans = dbContext.PaymentScheduleTrans.FirstOrDefault(t => t.Id == preScheduleTransId);
                                var scheduletrans = dbContext.PaymentScheduleTrans.FirstOrDefault(t => t.Id == scheduleTransId);
                                if (previousPenalty > 0) preScheduleTrans.PenalInterest = previousPenalty;
                                scheduletrans.CurrentDues = currentdues;
                                scheduletrans.IsInstallmentPaid = (currentdues == 0 || currentdues < 0) ? true : false;
                                dbContext.SaveChanges();

                                if (currentdues == 0 || currentdues < 0)
                                {
                                    for (int j = i - 1; j >= 0; j--)
                                    {
                                        int tId = scheduleTransList[j].Id;
                                        var sTrans = dbContext.PaymentScheduleTrans.FirstOrDefault(t => t.Id == tId);
                                        sTrans.IsInstallmentPaid = true;
                                        dbContext.SaveChanges();
                                    }
                                }

                                totaldues = (i == scheduleTransList.Count - 1) ? currentdues : 0;
                            }
                            //var pen = scheduleTransList[i].PaymentScheduleMaster.PenalInterest;
                        }
                    }
                }

                return installmentDuesList;
            }
        }

        private PaymentViewModel GetPaidInstallmentDetail(int rid, DateTime prevDate, DateTime nextDate)
        {
            PaymentViewModel payment = new PaymentViewModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                decimal paidamount = 0;
                DateTime? paiddate = DateTime.Now;
                var paidlist = (from master in dbContext.RECEIPT_DETAIL_MASTER
                                join trans in dbContext.RECEIPT_AMOUNT_TRANS on master.RECEIPT_ID equals trans.RECEIPT_ID
                                where master.RID_NO == rid.ToString()
                                && master.RECEIPT_ID == AccountReceipt.OldInstallment && trans.RECEIPT_SUBHEAD_ID == AccountReceipt.NormalInstallment
                                && DbFunctions.TruncateTime(master.DEPOSIT_DATE) >= DbFunctions.TruncateTime(prevDate)
                                && DbFunctions.TruncateTime(master.DEPOSIT_DATE) <= DbFunctions.TruncateTime(nextDate)
                                select trans);
                if (paidlist != null)
                {
                    foreach (var list in paidlist)
                    {
                        paidamount = paidamount + (decimal)list.AMOUNT_PAID;
                        paiddate = list.DEPOSIT_DATE;
                    }
                    payment.PaidAmount = paidamount;
                    payment.PaidUptoDate = paiddate;
                    payment.DepositDate = paiddate;
                }
                else payment.PaidAmount = 0;
            }
            return payment;
        }

        private PaymentViewModel GetInstallmentDuesByDate(int rid, decimal balance, decimal rate, decimal gst, DateTime prevDate, DateTime nextDate, string option)
        {
            PaymentViewModel payment = new PaymentViewModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                decimal gstamount = 0;
                decimal interest = 0;
                decimal total = 0;
                if (option == "yearly")
                {
                    interest = (balance * rate) / 100;
                    gstamount = (balance * (gst / 100));
                    total = balance + interest + gstamount;
                }
                else
                {
                    var extdays = nextDate.Date - prevDate.Date;
                    interest = ((balance * (rate / 100)) / 365) * extdays.Days;
                    gstamount = (balance * (gst / 100));
                    total = balance + interest + gstamount;
                }
                payment.DuesAmount = Math.Round((decimal)total, 2);
                payment.BalanceInterest = Math.Round((decimal)interest, 2);
                payment.InterestAmount = Math.Round((decimal)interest, 2);
                payment.GSTAmount = Math.Round((decimal)gstamount, 2);
            }
            return payment;
        }


        //private LeaseRentViewModel CalculateInstallmentDues(LeaseRentViewModel model)
        //{
        //    using (var dbcontext = new NoidaPMSEntities())
        //    {
        //        var data = dbcontext.InstallmentDuesPayments.FirstOrDefault(r => r.RegistrationId == model.RegistrationId);
        //        if (data != null)
        //        {
        //            var today = DateTime.Now;
        //            var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        //            today = DateTime.Now.AddDays(days - DateTime.Now.Day);

        //            if (data.BalanceUptoDate != null)
        //            {
        //                var balanceUptoDate = data.BalanceUptoDate;
        //                decimal duesAmount = 0;
        //                decimal gsamount = 0;
        //                decimal rate = data.PenalInterest == null ? 0 : (decimal)data.PenalInterest;

        //                if (balanceUptoDate < today)
        //                {
        //                    while (balanceUptoDate < today)
        //                    {
        //                        var balanceAmount = data.BalanceAmount == null ? 0 : data.BalanceAmount;
        //                        if (balanceUptoDate.Value.Year == today.Year)
        //                        {
        //                            if (balanceUptoDate.Value.Month == 6 || balanceUptoDate.Value.Month == 12)
        //                            {

        //                                var amount = balanceAmount + (balanceAmount * (rate / 2) / 100);
        //                                var gst = data.GST == null ? 0 : data.GST;
        //                                var gstinterest = ((decimal)amount - balanceAmount) * gst / 100;
        //                                duesAmount = (decimal)amount;
        //                                gsamount = (decimal)gstinterest;
        //                                balanceUptoDate = balanceUptoDate.Value.AddMonths(6);
        //                            }
        //                            else
        //                            {
        //                                if (balanceUptoDate.Value.Month < 6)
        //                                {
        //                                    var extdays = today.Date - balanceUptoDate.Value.Date;
        //                                    var interest = ((balanceAmount * (rate / 100)) / 365) * extdays.Days;
        //                                    var gst = data.GST == null ? 0 : data.GST;
        //                                    var gstamount = (balanceAmount * (gst / 100));
        //                                    var total = balanceAmount + interest + gstamount;
        //                                    duesAmount = Math.Round((decimal)total, 2);
        //                                    gsamount = (decimal)gstamount;

        //                                    int mnth = 6 - balanceUptoDate.Value.Month;
        //                                    balanceUptoDate = balanceUptoDate.Value.AddMonths(mnth);
        //                                }
        //                                else if (balanceUptoDate.Value.Month > 6)
        //                                {
        //                                    var extdays = today.Date - balanceUptoDate.Value.Date;
        //                                    var interest = ((balanceAmount * (rate / 100)) / 365) * extdays.Days;
        //                                    var gst = data.GST == null ? 0 : data.GST;
        //                                    var gstamount = (balanceAmount * (gst / 100));
        //                                    var total = balanceAmount + interest + gstamount;
        //                                    duesAmount = Math.Round((decimal)total, 2);
        //                                    gsamount = (decimal)gstamount;

        //                                    int mnth = 12 - balanceUptoDate.Value.Month;
        //                                    balanceUptoDate = balanceUptoDate.Value.AddMonths(mnth);
        //                                }
        //                            }
        //                        }
        //                        else if (today.Year > balanceUptoDate.Value.Year)
        //                        {
        //                            int totalYears = today.Year - balanceUptoDate.Value.Year;
        //                            int cpyear = totalYears * 2;
        //                            int totalMonths = today.Month;
        //                            if (today.Month > 6) cpyear = cpyear + 2;
        //                            if (today.Month < 6) cpyear = cpyear + 1;
        //                            decimal amount = (decimal)balanceAmount;
        //                            decimal rates = (decimal)rate / 2;
        //                            double totalamount = (double)amount * Math.Pow(1 + (double)rates / 100, (double)cpyear);
        //                            var gst = data.GST == null ? 0 : data.GST;
        //                            var amountforgst = (decimal)totalamount - balanceAmount;
        //                            var gstamount = amountforgst * (gst / 100);
        //                            duesAmount = Math.Round((decimal)totalamount, 2);
        //                            gsamount = Math.Round((decimal)gstamount, 2);
        //                            balanceUptoDate = balanceUptoDate.Value.AddMonths(cpyear * 6);
        //                        }

        //                    }
        //                    var days2 = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        //                    var today2 = DateTime.Now.AddDays(days2 - DateTime.Now.Day);

        //                    model.LeaseRentPremium = duesAmount;
        //                    model.GstAmount = gsamount;
        //                    model.BalanceUptoDate = balanceUptoDate;
        //                    model.DuesUptoDate = today2;
        //                }
        //                else
        //                {
        //                    model.LeaseRentPremium = data.BalanceAmount;
        //                    model.GstAmount = 0;
        //                    model.BalanceUptoDate = data.BalanceUptoDate;
        //                }
        //            }
        //        }
        //    }
        //    return model;
        //}

        public LeaseRentViewModel GetLeaseRentDetailsById(LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var detail = (from leaserent in dbcontext.LeaseRentPayments
                              join allotment in dbcontext.AllotmentMasters on leaserent.RegistrationId equals allotment.rid
                              join applicant in dbcontext.ApplicationDetails on leaserent.RegistrationId equals applicant.registrationId
                              join property in dbcontext.SchemePropTrans on allotment.propertyId equals property.propertyId
                              where leaserent.Id == model.Id
                              select new LeaseRentViewModel
                              {
                                  Id = leaserent.Id,
                                  RegistrationId = leaserent.RegistrationId,
                                  LeaseRentPremium = leaserent.PremiumLeaseRent,
                                  HtmlDuesReport = leaserent.HtmlDuesTemplate,
                                  Applicant = applicant.tGender == "Company" ? applicant.T_Company_Name : applicant.tFirstName + (applicant.tMiddleName == null ? "" : " " + applicant.tMiddleName) + " " + applicant.tLastName,
                                  PropertyNo = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo
                              }).FirstOrDefault();
                return detail;
            }
        }


        //public LeaseRentViewModel UpdateLeaseRentDuesPaymentById(LeaseRentViewModel model)
        //{
        //    using (var dbcontext = new NoidaPMSEntities())
        //    {
        //        var flag = ReturnType.None;
        //        // in future should be changed on Id
        //        if (model.RegistrationId != null && model.RegistrationId != 0)
        //        {
        //            if (model.ActionType == "LeaseRent")
        //            {
        //                var leaserent = dbcontext.LeaseRentPayments.OrderByDescending(r => r.Id).Where(r => r.RegistrationId == model.RegistrationId).FirstOrDefault();
        //                if (leaserent != null)
        //                {
        //                    var rentList = CalculateLeaseRentDues(model);
        //                    var list = dbcontext.PaymentLeaseRentTrans.Where(r => r.RegistrationId == model.RegistrationId).ToList();
        //                    if (list != null) dbcontext.PaymentLeaseRentTrans.RemoveRange(list);

        //                    Nullable<Decimal> totalDues = 0;
        //                    for (int i = 0; i < rentList.Count; i++)
        //                    {
        //                        var duesrent = rentList.ElementAt(i);
        //                        totalDues = totalDues + duesrent.DuesAmount;
        //                        if (i == rentList.Count - 1)
        //                        {
        //                            var rent = new PaymentLeaseRentTran();
        //                            rent.RegistrationId = model.RegistrationId;
        //                            rent.RentId = leaserent.Id;
        //                            rent.LeaseRentPremium = duesrent.LeaseRentPremium;
        //                            var duedate = duesrent.DepositDueDate.Value.AddDays(-1);
        //                            rent.DepositDueDate = duedate;
        //                            rent.StatusId = NAStatusId.Initiated;
        //                            rent.IsActive = true;
        //                            rent.DuesAmount = duesrent.DuesAmount;
        //                            rent.DuesUptoDate = duesrent.DuesUptoDate;
        //                            rent.HtmlDuesTemplate = duesrent.HtmlDuesReport;
        //                            rent.CreatedDate = DateTime.Now;
        //                            rent.CreatedBy = userInfo.UserID.ToString();
        //                            dbcontext.PaymentLeaseRentTrans.Add(rent);
        //                            dbcontext.SaveChanges();

        //                            leaserent.PremiumLeaseRent = duesrent.LeaseRentPremium;
        //                            //leaserent.LeaseRentDues = duesrent.DuesAmount;
        //                            leaserent.LeaseRentDues = totalDues;
        //                            leaserent.DuesUptoDate = duesrent.DuesUptoDate;

        //                            leaserent.PreviousDues = leaserent.CurrentDues;
        //                            leaserent.PreviousDuesDate = leaserent.CurrentDuesDate;
        //                            leaserent.CurrentDues = totalDues;
        //                            leaserent.CurrentDuesDate = duesrent.DuesUptoDate;
        //                            leaserent.HtmlDuesTemplate = duesrent.HtmlDuesReport;
        //                            dbcontext.SaveChanges();
        //                            model.HtmlDuesReport = duesrent.HtmlDuesReport;
        //                        }
        //                        else
        //                        {                                   
        //                            var rent = new PaymentLeaseRentTran();
        //                            rent.RegistrationId = model.RegistrationId;
        //                            rent.RentId = leaserent.Id;
        //                            rent.LeaseRentPremium = duesrent.LeaseRentPremium;
        //                            var duedate = duesrent.DepositDueDate.Value.AddDays(-1);
        //                            rent.DepositDueDate = duedate;
        //                            rent.StatusId = NAStatusId.Initiated;
        //                            rent.IsActive = true;
        //                            rent.DuesAmount = duesrent.DuesAmount;
        //                            rent.DuesUptoDate = duesrent.DuesUptoDate;
        //                            rent.HtmlDuesTemplate = duesrent.HtmlDuesReport;
        //                            rent.CreatedDate = DateTime.Now;
        //                            rent.CreatedBy = userInfo.UserID.ToString();
        //                            dbcontext.PaymentLeaseRentTrans.Add(rent);
        //                            dbcontext.SaveChanges();
        //                        }
        //                    }

        //                    flag = ReturnType.Updated;
        //                }
        //            }
        //            else if (model.ActionType == "Installment")
        //            {
        //                var installment = dbcontext.InstallmentDuesPayments.OrderByDescending(r => r.Id).Where(r => r.RegistrationId == model.RegistrationId).FirstOrDefault();
        //                if (installment != null)
        //                {
        //                    var duespremium = CalculateInstallmentDuesII(model);
        //                    installment.DuesAmount = duespremium.LeaseRentPremium;
        //                    installment.GstAmount = duespremium.GstAmount;
        //                    installment.DuesUptoDate = duespremium.DuesUptoDate;
        //                    installment.HtmlDuesTemplate = duespremium.HtmlDuesReport;
        //                    dbcontext.SaveChanges();
        //                    flag = ReturnType.Updated;
        //                }
        //            }
        //            model.ActionTypeId = flag;
        //            model.ReturnTypeId = flag;
        //        }
        //    }
        //    return model;
        //}


        public LeaseRentViewModel UpdateLeaseRentDuesPaymentById(LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var flag = ReturnType.None;
                // in future should be changed on Id
                if (model.RegistrationId != null && model.RegistrationId != 0)
                {
                    if (model.ActionType == "LeaseRent")
                    {
                        var leaserent = dbcontext.LeaseRentPayments.OrderByDescending(r => r.Id).Where(r => r.RegistrationId == model.RegistrationId).FirstOrDefault();
                        if (leaserent != null)
                        {
                            var today = DateTime.Now;
                            var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                            today = DateTime.Now.AddDays(days - DateTime.Now.Day);

                            // change on 14 Nov 2018 (date seleceted from grid)
                            if (model.CurrentDuesDate != null) { today = (DateTime)model.CurrentDuesDate; }

                            List<Sp_LeaseRentDuesCalculationTillDate_Result> rentList = dbcontext.Sp_LeaseRentDuesCalculationTillDate(model.RegistrationId.ToString(), today, model.DepartmentId).ToList();
                            var exList = dbcontext.PaymentLeaseRentTrans.Where(r => r.RegistrationId == model.RegistrationId).ToList();
                            if (exList != null) dbcontext.PaymentLeaseRentTrans.RemoveRange(exList);

                            decimal? totalDues = 0;
                            for (int i = 0; i < rentList.Count; i++)
                            {
                                var duesrent = rentList.ElementAt(i);
                                totalDues = totalDues + duesrent.totalamount;
                                if (i == rentList.Count - 1)
                                {
                                    var rent = new PaymentLeaseRentTran();
                                    rent.RegistrationId = model.RegistrationId;
                                    rent.RentId = leaserent.Id;
                                    rent.LeaseRentPremium = duesrent.leaserent;
                                    rent.DepositDueDate = duesrent.duedate;
                                    rent.StatusId = NAStatusId.Initiated;
                                    rent.IsActive = true;
                                    rent.DuesAmount = duesrent.totalamount;
                                    rent.DuesUptoDate = duesrent.duedate;
                                    rent.CreatedDate = DateTime.Now;
                                    rent.CreatedBy = userInfo.UserID.ToString();
                                    dbcontext.PaymentLeaseRentTrans.Add(rent);
                                    dbcontext.SaveChanges();

                                    leaserent.PreviousDues = leaserent.CurrentDues;
                                    leaserent.PreviousDuesDate = leaserent.CurrentDuesDate;
                                    leaserent.CurrentDues = duesrent.Total_dues; //totalDues;
                                    leaserent.DuesInterest = duesrent.totalpanelamount;
                                    leaserent.LeaseRentDues = duesrent.totalamount;
                                    leaserent.TotalDuesAmount = duesrent.Total_dues;
                                    leaserent.CurrentDuesDate = duesrent.duedate;
                                    dbcontext.SaveChanges();
                                }
                                else
                                {
                                    var rent = new PaymentLeaseRentTran();
                                    rent.RegistrationId = model.RegistrationId;
                                    rent.RentId = leaserent.Id;
                                    rent.LeaseRentPremium = duesrent.leaserent;
                                    //var duedate = duesrent.DepositDate.Value.AddDays(-1);
                                    //rent.DepositDueDate = duedate;
                                    rent.DepositDueDate = duesrent.duedate;
                                    rent.StatusId = NAStatusId.Initiated;
                                    rent.IsActive = true;
                                    rent.DuesAmount = duesrent.totalamount;
                                    rent.DuesUptoDate = duesrent.duedate;
                                    //rent.HtmlDuesTemplate = duesrent.HtmlDuesReport;
                                    rent.CreatedDate = DateTime.Now;
                                    rent.CreatedBy = userInfo.UserID.ToString();
                                    dbcontext.PaymentLeaseRentTrans.Add(rent);
                                    dbcontext.SaveChanges();
                                }
                            }

                            if (rentList.Count == 0)
                            {
                                leaserent.PreviousDues = leaserent.CurrentDues;
                                leaserent.PreviousDuesDate = leaserent.CurrentDuesDate;
                                leaserent.CurrentDues = 0; //totalDues;
                                leaserent.DuesInterest = 0;
                                leaserent.LeaseRentDues = 0;
                                leaserent.TotalDuesAmount = 0;
                                leaserent.CurrentDuesDate = today;
                                dbcontext.SaveChanges();
                            }

                            flag = ReturnType.Updated;
                        }
                    }
                    else if (model.ActionType == "Installment")
                    {
                        var installment = dbcontext.InstallmentDuesPayments.OrderByDescending(r => r.Id).Where(r => r.RegistrationId == model.RegistrationId).FirstOrDefault();
                        if (installment != null)
                        {
                            var duespremium = CalculateInstallmentDuesII(model);
                            installment.DuesAmount = duespremium.LeaseRentPremium;
                            installment.GstAmount = duespremium.GstAmount;
                            installment.DuesUptoDate = duespremium.DuesUptoDate;
                            installment.HtmlDuesTemplate = duespremium.HtmlDuesReport;
                            dbcontext.SaveChanges();
                            flag = ReturnType.Updated;
                        }
                    }
                    model.ActionTypeId = flag;
                    model.ReturnTypeId = flag;
                }
            }
            return model;
        }

        public PaymentViewModel InstallmentDuesPayment(int Rid)
        {
            var paymentviewmodel = new PaymentViewModel();
            using (var dbcontext = new NoidaPMSEntities())
            {
                var duesList = (from dues in dbcontext.temp_cal
                                where dues.rid == Rid
                                select new PaymentViewModel
                                {
                                    DuesAmount = dues.DuesTill,
                                    TotalDuesAmount = dues.DuesNextWithInterest,
                                    PenalInterest = dues.TotalPanelAmount,
                                    PrincipalAmount = dues.TotalPrincipalAmount,
                                    DuesUptoDate = dues.InstallmentDueDate == null ? DateTime.Now : dues.InstallmentDueDate,
                                }).ToList();
                var Installment = new PaymentViewModel();
                if (duesList != null && duesList.Count > 0)
                {
                    var dues = duesList[duesList.Count - 1];
                    Installment = new PaymentViewModel
                    {
                        DuesAmount = dues.DuesAmount,
                        TotalDuesAmount = dues.TotalDuesAmount,
                        PenalInterest = dues.PenalInterest,
                        PrincipalAmount = dues.PrincipalAmount,
                        DuesUptoDate = dues.DuesUptoDate == null ? DateTime.Now : dues.DuesUptoDate,
                    };
                    paymentviewmodel = Installment;
                }
                else
                {
                    Installment = new PaymentViewModel
                    {
                        PremiumPaidStatus = "Full Paid",
                        IsInstallmentPaid = true
                    };
                    paymentviewmodel = Installment;
                }

                //var dueshistory = dbcontext.PaymentDuesCalculationMsts.Where(r => r.RegistrationId == Rid);
                //if (dueshistory != null)
                //{
                //    dueshistory.Each(d => d.IsActive = false);
                //    var duescal = new PaymentDuesCalculationMst();
                //    duescal.RegistrationId = Rid;
                //    duescal.TotalPremiumDues = Installment.TotalDuesAmount;
                //    duescal.TotalPremiumInsterest = Installment.PenalInterest;
                //    //duescal.
                //}
            }
            return paymentviewmodel;
        }

        public LeaseRentViewModel GetPaymentDuesByRegistrationId(LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                if (model.ActionType == "LeaseRent")
                {
                    var rentdues = dbcontext.LeaseRentPayments.OrderByDescending(r => r.Id).Where(r => r.RegistrationId == model.RegistrationId).FirstOrDefault();
                    if (rentdues != null)
                    {
                        if (rentdues.HtmlDuesTemplate == null)
                        {
                            var duesrent = CalculateLeaseRentPremium(model);
                            rentdues.LeaseRentDues = duesrent.LeaseRentPremium;
                            rentdues.DuesUptoDate = duesrent.DuesUptoDate;
                            rentdues.HtmlDuesTemplate = duesrent.HtmlDuesReport;
                            model.HtmlDuesReport = duesrent.HtmlDuesReport;
                            model.ActionType = "LeaseRent";
                        }
                        else
                        {
                            var detail = (from leaserent in dbcontext.LeaseRentPayments
                                          join allotment in dbcontext.AllotmentMasters on leaserent.RegistrationId equals allotment.rid
                                          join applicant in dbcontext.ApplicationDetails on leaserent.RegistrationId equals applicant.registrationId
                                          join property in dbcontext.SchemePropTrans on allotment.propertyId equals property.propertyId
                                          where leaserent.RegistrationId == model.RegistrationId
                                          select new LeaseRentViewModel
                                          {
                                              Id = leaserent.Id,
                                              RegistrationId = leaserent.RegistrationId,
                                              LeaseRentPremium = leaserent.PremiumLeaseRent,
                                              HtmlDuesReport = leaserent.HtmlDuesTemplate,
                                              Applicant = applicant.tGender == "Company" ? applicant.T_Company_Name : applicant.tFirstName + (applicant.tMiddleName == null ? "" : " " + applicant.tMiddleName) + " " + applicant.tLastName,
                                              PropertyNo = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo
                                          }).FirstOrDefault();

                            model = detail;
                            model.ActionType = "LeaseRent";
                        }
                    }

                    //return detail;
                }
                else if (model.ActionType == "Installment")
                {
                    var installmentdues = dbcontext.InstallmentDuesPayments.OrderByDescending(r => r.Id).Where(r => r.RegistrationId == model.RegistrationId).FirstOrDefault();
                    if (installmentdues != null)
                    {
                        if (installmentdues.HtmlDuesTemplate == null)
                        {
                            var duesrent = CalculateInstallmentDues(model);
                            installmentdues.DuesAmount = duesrent.LeaseRentPremium;
                            installmentdues.DuesUptoDate = duesrent.DuesUptoDate;
                            installmentdues.HtmlDuesTemplate = duesrent.HtmlDuesReport;
                            model.HtmlDuesReport = duesrent.HtmlDuesReport;
                            model.ActionType = "Installment";
                        }
                        else
                        {
                            var detail = (from instalment in dbcontext.InstallmentDuesPayments
                                          join allotment in dbcontext.AllotmentMasters on instalment.RegistrationId equals allotment.rid
                                          join applicant in dbcontext.ApplicationDetails on instalment.RegistrationId equals applicant.registrationId
                                          join property in dbcontext.SchemePropTrans on allotment.propertyId equals property.propertyId
                                          where instalment.RegistrationId == model.RegistrationId
                                          select new LeaseRentViewModel
                                          {
                                              Id = instalment.Id,
                                              RegistrationId = instalment.RegistrationId,
                                              LeaseRentPremium = instalment.DuesAmount,
                                              HtmlDuesReport = instalment.HtmlDuesTemplate,
                                              Applicant = applicant.tGender == "Company" ? applicant.T_Company_Name : applicant.tFirstName + (applicant.tMiddleName == null ? "" : " " + applicant.tMiddleName) + " " + applicant.tLastName,
                                              PropertyNo = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo
                                          }).FirstOrDefault();
                            model = detail;
                            model.ActionType = "LeaseRent";
                        }
                    }
                }
                return model;
            }
        }


        public DataSourceResult GetPropertyAccountDetails(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                bool IsOTLRP = model.PaymentStatus == NA.PMS.Common.NDCOptions.Id_Yes ? true : false;

                var list = (from alotment in dbcontext.AllotmentMasters
                            from leaserent in dbcontext.LeaseRentPayments.Where(r => r.RegistrationId == alotment.rid).DefaultIfEmpty()
                            from instalmnt in dbcontext.InstallmentDuesPayments.Where(r => r.RegistrationId == alotment.rid).DefaultIfEmpty()
                            join property in dbcontext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where (leaserent.RegistrationId != null || instalmnt.RegistrationId != null)
                              && (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                              && (model.IsOneTimeLeaseRentPaid == null || instalmnt.IsOneTimeLeasePaid == model.IsOneTimeLeaseRentPaid)
                              && (model.IsTotalInstallmentPaid == null || instalmnt.IsTotalPremiumPaid == model.IsTotalInstallmentPaid)
                            select new PaymentViewModel
                            {
                                Id = instalmnt.Id,
                                RegistrationId = alotment.rid,
                                RegistrationNo = alotment.rid.ToString(),
                                SectorName = property.SectorMst.sectorName,
                                BlockName = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                DepartmentId = alotment.departmentId,
                                Department = alotment.DepartmentMst.departmentName,
                                DuesAmount = instalmnt.DuesAmount,
                                GSTAmount = instalmnt.GstAmount,
                                InstallmentAmount = instalmnt.BalanceAmount,
                                BalanceUptoDate = instalmnt.BalanceUptoDate,
                                InstallmentDuesUptoDate = instalmnt.DuesUptoDate,

                                LeaseRentDues = leaserent.LeaseRentDues,
                                LeaseRentId = leaserent.Id,
                                LeaseDeedDate = leaserent.LeaseDeedDate,
                                LeaseRentPremium = leaserent.PremiumLeaseRent,
                                PenalInterest = leaserent.PanelInterest,
                                PaidUptoDate = leaserent.PremiumPaidUptoDate,
                                LeaseRentDuration = leaserent.PremiumPaidDuration,
                                RevisedDate = leaserent.RevisedPremiumDate,
                                LeaseRentBalance = leaserent.BalanceAmount,
                                BalanceInterest = leaserent.BalanceInterest,
                                NDCDate = leaserent.NDCDate,
                                LeaseRentDuesUptoDate = leaserent.DuesUptoDate
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public LeaseRentViewModel UpdateInstallmentDuesPaymentById(LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var flag = ReturnType.None;
                // in future should be changed on Id
                if (model.RegistrationId != null && model.RegistrationId != 0)
                {
                    if (model.ActionType == "LeaseRent")
                    {
                        var leaserent = dbcontext.LeaseRentPayments.OrderByDescending(r => r.Id).Where(r => r.RegistrationId == model.RegistrationId).FirstOrDefault();
                        if (leaserent != null)
                        {
                            var duesrent = CalculateLeaseRentPremium(model);
                            //leaserent.LeaseRentDues = duesrent.LeaseRentPremium;
                            leaserent.PreviousDues = leaserent.CurrentDues;
                            leaserent.PreviousDuesDate = leaserent.CurrentDuesDate;
                            leaserent.CurrentDues = duesrent.LeaseRentPremium;
                            leaserent.CurrentDuesDate = duesrent.DuesUptoDate;
                            //leaserent.DuesUptoDate = duesrent.DuesUptoDate;
                            leaserent.HtmlDuesTemplate = duesrent.HtmlDuesReport;
                            dbcontext.SaveChanges();
                            model.HtmlDuesReport = duesrent.HtmlDuesReport;
                            flag = ReturnType.Success;
                        }
                    }
                    else if (model.ActionType == "Installment")
                    {
                        var installment = dbcontext.viewInstutionalPBalances.OrderByDescending(r => r.Id).Where(r => r.RegistrationId == model.RegistrationId).FirstOrDefault();
                        if (installment != null)
                        {
                            //var duespremium = CalculateInstallmentDues(model);
                            var duespremium = CalculateInstallmentDues(model);
                            //installment.DuesAmount = duespremium.LeaseRentPremium;
                            installment.PreviousDues = installment.CurrentDues;
                            installment.PreviousDuesDate = installment.CurrentDuesDate;
                            installment.CurrentDues = duespremium.LeaseRentPremium;
                            installment.CurrentDuesDate = duespremium.DuesUptoDate;// DateTime.Now;
                            installment.GST = duespremium.GstAmount;
                            //installment.DuesUptoDate = duespremium.DuesUptoDate;
                            installment.HtmlDuesTemplate = duespremium.HtmlDuesReport;
                            dbcontext.SaveChanges();
                            flag = ReturnType.Success;
                        }
                    }
                    model.ActionTypeId = flag;
                }
            }
            return model;
        }


        public DataSourceResult GetLeaseRentReportDetails(DataSourceRequest request, LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                //bool IsOTLRP = model.PremiumPaidStatus == NA.PMS.Common.NDCOptions.Id_Yes ? true : false;
                //var list = (from leaserent in dbcontext.LeaseRentPayments
                //            join alotment in dbcontext.AllotmentMasters on leaserent.RegistrationId equals alotment.rid
                //            join property in dbcontext.SchemePropTrans on alotment.propertyId equals property.propertyId
                //            where model.DepartmentId == null || alotment.departmentId == model.DepartmentId //&& leaserent.IsOneTimeLeasePaid == IsOTLRP
                //            select new LeaseRentViewModel
                //            {
                //                Id = leaserent.Id,
                //                RentId = leaserent.Id,
                //                Department = alotment.DepartmentMst.departmentName,
                //                RegistrationId = leaserent.RegistrationId,
                //                PropertyNo = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "/" + property.propertyNo,
                //                LeaseDeedDate = leaserent.LeaseDeedDate,
                //                LeaseRentPremium = leaserent.PremiumLeaseRent,
                //                //PremiumInterest = leaserent.PremiumInterest,
                //                PanelInterest = leaserent.PanelInterest,
                //                PaidUptoDate = leaserent.PremiumPaidUptoDate,
                //                PremiumPaidDuration = leaserent.PremiumPaidDuration,
                //                RevisedDate = leaserent.RevisedPremiumDate,
                //                BalanceAmount = leaserent.BalanceAmount,
                //                BalanceInterest = leaserent.BalanceInterest,
                //                GST = leaserent.GST,
                //                TotalBalance = leaserent.BalanceAmount != null ? leaserent.BalanceAmount : 0 + leaserent.BalanceInterest != null ? leaserent.BalanceInterest : 0 + leaserent.GST != null ? leaserent.GST : 0,
                //                BalanceUptoDate = leaserent.BalanceUptoDate,
                //                RevisedPremium = leaserent.RevisedRate,
                //                IsOneTimeLeaseRentPaid = leaserent.IsOneTimeLeasePaid,
                //                IsTotalLeaseRentPaid = leaserent.IsTotalPremiumPaid,
                //                OneTimePaidStatus = leaserent.IsOneTimeLeasePaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                //                PremiumPaidStatus = leaserent.IsTotalPremiumPaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                //                NDCDate = leaserent.NDCDate,
                //                ChallanDate = leaserent.ChallanDate,
                //                StatusId = leaserent.StatusId,
                //                Status = leaserent.StatusId > 0 ? (from status in dbcontext.StatusMasters where status.Id == leaserent.StatusId select status.Status).FirstOrDefault() : string.Empty,
                //                IsActive = leaserent.IsActive,
                //                Sector = null
                //            });
                //return list.ToDataSourceResult(request);

                var list = dbcontext.Sp_AccountReport_Receipt_list(model.FinancialYear, model.DepartmentId, model.ActionType).ToList();
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetRevenueGeneratedList(DataSourceRequest request, LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                bool IsOTLRP = model.PremiumPaidStatus == NA.PMS.Common.NDCOptions.Id_Yes ? true : false;
                var prevdate = Convert.ToDateTime("2017-04-01");
                var nextdate = Convert.ToDateTime("2018-03-31");

                //var aclist = dbcontext.Sp_AccountReport_list(model.ActionType, model.DepartmentId).ToList();
                //var rclist = dbcontext.Sp_AccountReport_Receipt_list(model.ActionType, model.DepartmentId, "Lease Rent").ToList();

                var result = dbcontext.Sp_AccountReport(model.ActionType, model.DepartmentId).ToList();
                List<LeaseRentViewModel> model2 = new List<LeaseRentViewModel>();
                if (result != null)
                {
                    foreach (var rs in result)
                    {
                        LeaseRentViewModel model3 = new LeaseRentViewModel();
                        model3.PremiumPaidDuration = rs.F_Y;
                        model3.LeaseRentPremium = rs.LeaseRentPremium;
                        model3.PaidAmount = rs.ReciveAmount;
                        model3.ReceiptSubHead = rs.AccountHead;
                        model3.TotalArear = rs.LeaseRentArear;
                        model3.LeaseRentDues = rs.leaserentdues;
                        model2.Add(model3);
                    }
                }

                return model2.ToDataSourceResult(request);
                //return null;
            }
        }


        public LeaseRentViewModel GetRevenueWithDefaulterList(LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                bool IsOTLRP = model.PremiumPaidStatus == NA.PMS.Common.NDCOptions.Id_Yes ? true : false;
                var prevdate = Convert.ToDateTime("2017-04-01");
                var nextdate = Convert.ToDateTime("2018-03-31");
                var list = (from leaserent in dbcontext.LeaseRentPayments
                            join leasetrans in dbcontext.PaymentLeaseRentTrans on leaserent.Id equals leasetrans.RentId
                            join alotment in dbcontext.AllotmentMasters on leaserent.RegistrationId equals alotment.rid
                            join property in dbcontext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                            && leaserent.IsOneTimeLeasePaid == false
                            && DbFunctions.TruncateTime(leasetrans.DepositDueDate) >= DbFunctions.TruncateTime(prevdate)
                            && DbFunctions.TruncateTime(leasetrans.DepositDueDate) <= DbFunctions.TruncateTime(nextdate)
                            select new LeaseRentViewModel
                            {
                                LeaseRentPremium = leasetrans.LeaseRentPremium,
                                LeaseRentDues = leaserent.LeaseRentDues
                            });

                var sum1 = list.Sum(r => r.LeaseRentPremium);

                decimal? totalpremium = list.Sum(r => r.LeaseRentPremium);
                decimal? totaldues = list.Sum(r => r.LeaseRentDues);
                decimal? defaulterdues = 0;
                decimal? arears = totaldues - totalpremium;

                model.DuesAmount = totaldues;
                model.TotalPremium = totalpremium;
                model.TotalArear = arears;
                model.DefaulterAmount = defaulterdues;
            }
            return model;
        }


        public DataSourceResult GetPropertyPremiumReportDetails(DataSourceRequest request, LeaseRentViewModel model)
        {
            throw new NotImplementedException();
        }


        public DataSourceResult GetRevenueReportDetails(DataSourceRequest request, LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                //bool IsOTLRP = model.PremiumPaidStatus == NA.PMS.Common.NDCOptions.Id_Yes ? true : false;
                //var list = (from leaserent in dbcontext.LeaseRentPayments
                //            join alotment in dbcontext.AllotmentMasters on leaserent.RegistrationId equals alotment.rid
                //            join property in dbcontext.SchemePropTrans on alotment.propertyId equals property.propertyId
                //            where model.DepartmentId == null || alotment.departmentId == model.DepartmentId //&& leaserent.IsOneTimeLeasePaid == IsOTLRP
                //            select new LeaseRentViewModel
                //            {
                //                Id = leaserent.Id,
                //                RentId = leaserent.Id,
                //                Department = alotment.DepartmentMst.departmentName,
                //                RegistrationId = leaserent.RegistrationId,
                //                PropertyNo = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "/" + property.propertyNo,
                //                LeaseDeedDate = leaserent.LeaseDeedDate,
                //                LeaseRentPremium = leaserent.PremiumLeaseRent,
                //                //PremiumInterest = leaserent.PremiumInterest,
                //                PanelInterest = leaserent.PanelInterest,
                //                PaidUptoDate = leaserent.PremiumPaidUptoDate,
                //                PremiumPaidDuration = leaserent.PremiumPaidDuration,
                //                RevisedDate = leaserent.RevisedPremiumDate,
                //                BalanceAmount = leaserent.BalanceAmount,
                //                BalanceInterest = leaserent.BalanceInterest,
                //                GST = leaserent.GST,
                //                TotalBalance = leaserent.BalanceAmount != null ? leaserent.BalanceAmount : 0 + leaserent.BalanceInterest != null ? leaserent.BalanceInterest : 0 + leaserent.GST != null ? leaserent.GST : 0,
                //                BalanceUptoDate = leaserent.BalanceUptoDate,
                //                RevisedPremium = leaserent.RevisedRate,
                //                IsOneTimeLeaseRentPaid = leaserent.IsOneTimeLeasePaid,
                //                IsTotalLeaseRentPaid = leaserent.IsTotalPremiumPaid,
                //                OneTimePaidStatus = leaserent.IsOneTimeLeasePaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                //                PremiumPaidStatus = leaserent.IsTotalPremiumPaid == true ? NA.PMS.Common.NDCOptions.Id_Yes : NA.PMS.Common.NDCOptions.Id_No,
                //                NDCDate = leaserent.NDCDate,
                //                ChallanDate = leaserent.ChallanDate,
                //                StatusId = leaserent.StatusId,
                //                Status = leaserent.StatusId > 0 ? (from status in dbcontext.StatusMasters where status.Id == leaserent.StatusId select status.Status).FirstOrDefault() : string.Empty,
                //                IsActive = leaserent.IsActive,
                //                Sector = null
                //            });
                //return list.ToDataSourceResult(request);

                var list = dbcontext.Sp_AccountReport_list(model.FinancialYear, model.DepartmentId).ToList();
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetLeaseRentTransDetails(DataSourceRequest request, LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var list = (from leaserent in dbcontext.PaymentLeaseRentTrans
                            join alotment in dbcontext.AllotmentMasters on leaserent.RegistrationId equals alotment.rid
                            join property in dbcontext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where (model.RegistrationId == null || alotment.rid == model.RegistrationId)
                             && (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                             && (model.RentId == null || leaserent.RentId == model.RentId)
                            select new LeaseRentViewModel
                            {
                                Id = leaserent.Id,
                                RegistrationId = leaserent.RegistrationId,
                                LeaseRentPremium = leaserent.LeaseRentPremium,
                                DepositDate = leaserent.DepositDueDate,
                                Sector = null
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetLeaseRentPaidListById(DataSourceRequest request, LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                bool IsOTLRP = model.PremiumPaidStatus == NA.PMS.Common.NDCOptions.Id_Yes ? true : false;
                var list = (from rentmaster in dbcontext.LeaseRentPayments
                            join leaserent in dbcontext.PaymentLeaseRentTrans on rentmaster.Id equals leaserent.RentId
                            join alotment in dbcontext.AllotmentMasters on rentmaster.RegistrationId equals alotment.rid
                            //join property in dbcontext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where rentmaster.Id == model.Id
                            select new LeaseRentViewModel
                            {
                                Id = rentmaster.Id,
                                RentId = rentmaster.Id,
                                RegistrationId = rentmaster.RegistrationId,
                                Sector = rentmaster.Sector,
                                Block = rentmaster.Block,
                                PlotNo = rentmaster.PlotNo,
                                //PropertyNo = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "/" + property.propertyNo,
                                LeaseDeedDate = rentmaster.LeaseDeedDate,
                                TransferLeaseDate = rentmaster.TransferLeaseDate,
                                LeaseRentPremium = leaserent.LeaseRentPremium,
                                DepositDueDate = leaserent.DepositDueDate,

                                Comment = leaserent.Comment,
                                DuesUptoDate = leaserent.DuesUptoDate,
                                DuesAmount = leaserent.DuesAmount,
                                TransactionId = leaserent.TransactionId,
                                PaymentMode = leaserent.PaymentMode,
                                ApprovalDate = leaserent.ApprovalDate,
                                ApproverId = leaserent.Approver,
                                StatusId = leaserent.StatusId,
                                Status = (leaserent.StatusId != null || leaserent.StatusId > 0) ? (from status in dbcontext.StatusMasters where status.Id == leaserent.StatusId select status.Status).FirstOrDefault() : string.Empty,
                                IsActive = leaserent.IsActive,

                                CreatedDate = leaserent.CreatedDate,
                                CreatedBy = leaserent.CreatedBy
                            }).Distinct();
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetPropertyPremiumScheduleList(DataSourceRequest request, PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from detail in dbContext.PaymentScheduleMasters
                            join aplicant in dbContext.ApplicationDetails on detail.Rid equals aplicant.registrationId
                            where detail.IsActive == true
                            && (model.DepartmentId == null || detail.DepartmentId == model.DepartmentId)
                             && (model.RegistrationId == null || detail.Rid == model.RegistrationId)
                            select new PaymentScheduleModel
                            {
                                Id = detail.ScheduleId,
                                ScheduleId = detail.ScheduleId,
                                RegistrationId = detail.Rid,
                                PropertyId = detail.PropertyId,
                                PrincipalAmount = detail.PrincipalAmount,
                                Applicant = aplicant.tGender == Constants.Company ? aplicant.T_Company_Name : aplicant.tFirstName + (aplicant.tMiddleName == null ? "" : " " + aplicant.tMiddleName + " " + aplicant.tLastName),
                                TotalInstallment = detail.NoOfInstallment,
                                Frequency = detail.FrequencyOfInstallment,
                                FrequencyName = detail.FrequencyOfInstallment == 2 ? "Half Yearly" : (detail.FrequencyOfInstallment == 3 ? "Quarterly" : (detail.FrequencyOfInstallment == 12 ? "Monthly" : "Annual")),
                                PeriodOfInstallment = detail.PeriodOfInstallment,
                                InstallmentStartDate = detail.InstallmentStartDate,
                                NormalInterest = detail.NormalInterest,
                                PenalInterest = detail.PenalInterest,
                                ScheduleType = detail.ScheduleType,
                                IsOneTimeLeaseRentPaid = detail.IsOneTimeLeaseRentPaid,
                                IsInstallmentDeposited = detail.IsOneTimeInstallmentPaid,
                                IsActive = detail.IsActive,
                                OneTimeIstallmentStatus = (detail.IsOneTimeInstallmentPaid == null || detail.IsOneTimeInstallmentPaid == true) ? "Paid" : "Not Paid",
                                OneTimeLeaseRentStatus = (detail.IsOneTimeLeaseRentPaid == null || detail.IsOneTimeLeaseRentPaid == true) ? "Paid" : "Not Paid",
                                PaymentMode = null,
                            }).Distinct();
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetPropertyPremiumSchedulePaidListById(DataSourceRequest request, PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from detail in dbContext.PaymentScheduleMasters
                            join trans in dbContext.PaymentScheduleTrans on detail.ScheduleId equals trans.ScheduleId
                            join aplicant in dbContext.ApplicationDetails on detail.Rid equals aplicant.registrationId
                            where detail.IsActive == true && detail.ScheduleId == model.ScheduleId
                            select new PaymentScheduleModel
                            {
                                Id = trans.Id,
                                ScheduleId = detail.ScheduleId,
                                RegistrationId = detail.Rid,
                                InstallmentNo = trans.InstallmentNo,
                                InstallmentAmount = trans.InstallmentAmount,
                                InstallmentInterest = trans.InterestAmount,
                                InstallmentDueDate = trans.InstallmentDueDate,
                                InstallmentPeriod = trans.InstallmentPeriod,
                                BalanceAmount = trans.BalanceAmount,
                                IsInstallmentPaid = trans.IsInstallmentPaid,
                                IsActive = trans.IsActive,
                                TotalDueAmount = trans.DuesAmount,
                                DuesUptoDate = trans.DuesUptoDate,
                                HtmlTemplate = trans.HtmlDuesTemplate,
                                DepositDate = trans.DepositDate,
                                PaymentMode = trans.PaymentMode,
                                TransactionId = trans.TransactionId,
                                ActionType = null
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public int SavePropertyPremiumScheduleByRegistrationId(PaymentScheduleModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "Reschedule")
                {
                    System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection();
                    var rflag = connection.Query("dbo.Sp_NewPaymentSchedule_OutSide_new", new { rid = model.RegistrationId, PrincipalAmount = model.PrincipalAmount, noOfInstallments = model.TotalInstallment, FrequencyOfInstallment = model.Frequency, instalmentStartDate = model.InstallmentStartDate, NormalInterest = model.NormalInterest, PenalInterest = model.PenalInterest, ScheduleType = model.ScheduleType }, commandType: System.Data.CommandType.StoredProcedure);
                    //dbContext.Sp_NewPaymentSchedule_OutSide(model.RegistrationId, model.PrincipalAmount, model.TotalInstallment, model.Frequency, model.InstallmentStartDate, model.NormalInterest, model.PenalInterest);
                    //var rflag = dbContext.Sp_PaymentReSchedule(model.RegistrationId, model.PrincipalAmount);
                    //if (rflag > 0) { flag = ReturnType.Rescheduled; }
                    flag = ReturnType.Rescheduled;
                }
                else
                {
                    var ScheduleFlag = dbContext.Sp_NewPaymentSchedule_OutSide(model.RegistrationId, model.PrincipalAmount, model.TotalInstallment, model.Frequency, model.InstallmentStartDate, model.NormalInterest, model.PenalInterest);
                    if (ScheduleFlag > 0) { flag = ReturnType.Saved; }
                }
            }
            return flag;
        }


        public PaymentScheduleModel GetPropertyPremiumScheduleInformationById(PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from alotment in dbContext.AllotmentMasters
                              join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId.Value
                              join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                              join proptype in dbContext.PropertyTypeMsts on property.propertyTypeId equals proptype.propertyTypeId
                              where alotment.rid == model.RegistrationId
                              select new PaymentScheduleModel
                              {
                                  Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                  ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                  ApplicantType = aplicant.tGender,
                                  DepartmentId = alotment.departmentId,
                                  Department = alotment.DepartmentMst.departmentName,
                                  PropertyNo = property.SectorMst.sectorName + "/" + ((property.blockId == null || property.blockId <= 0) ? string.Empty : property.BlockMst.blockName + "-") + property.propertyNo,
                                  PropertyType = proptype.propertyTypeName,
                                  AllotmentDate = alotment.allotmentDate,
                                  PropertyId = alotment.propertyId
                              }).FirstOrDefault();
                var schedule = dbContext.PaymentScheduleMasters.FirstOrDefault(p => p.Rid == model.RegistrationId && p.IsActive == true && p.ScheduleType == "I");
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
                var installment = (from pay in dbContext.PaymentScheduleTrans where pay.Rid == model.RegistrationId && pay.IsActive == true select pay).OrderByDescending(s => s.Id).FirstOrDefault();
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


        public int RemovePropertyPremiumScheduleById(PaymentScheduleModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "Reschedule")
                {
                    var scheduleMst = dbContext.PaymentScheduleMasters.FirstOrDefault(f => f.ScheduleId == model.ScheduleId);
                    if (scheduleMst != null)
                    {
                        var scheduleTrans = dbContext.PaymentScheduleTrans.Where(t => t.ScheduleId == model.ScheduleId).ToList();
                        if (scheduleTrans != null)
                        {
                            dbContext.PaymentScheduleTrans.RemoveRange(scheduleTrans);
                        }
                        dbContext.PaymentScheduleMasters.Remove(scheduleMst);
                        dbContext.SaveChanges();
                        flag = ReturnType.Removed;
                    }
                }
                else
                {
                    var scheduleList = dbContext.PaymentScheduleMasters.Where(p => p.Rid == model.RegistrationId).ToList();
                    if (scheduleList != null && scheduleList.Count > 0)
                    {
                        foreach (var schedule in scheduleList)
                        {
                            if (schedule != null)
                            {
                                var installmentList = dbContext.PaymentScheduleTrans.Where(r => r.ScheduleId == schedule.ScheduleId).ToList();
                                if (installmentList != null && installmentList.Count > 0)
                                {
                                    dbContext.PaymentScheduleTrans.RemoveRange(installmentList);
                                }
                            }
                        }
                        dbContext.PaymentScheduleMasters.RemoveRange(scheduleList);
                        dbContext.SaveChanges();
                        flag = ReturnType.Removed;
                    }
                }

            }

            //using (var dbContext = new NoidaPMSEntities())
            //{
            //    var schedule = dbContext.PaymentScheduleMasters.Where(p => p.ScheduleId==model.ScheduleId).FirstOrDefault();
            //    if (schedule != null)
            //    {
            //        var installmentList = dbContext.PaymentScheduleTrans.Where(r => r.ScheduleId == schedule.ScheduleId).ToList();
            //        if (installmentList != null && installmentList.Count > 0)
            //        {
            //            dbContext.PaymentScheduleTrans.RemoveRange(installmentList);
            //        }
            //        dbContext.PaymentScheduleMasters.Remove(schedule);
            //        dbContext.SaveChanges();
            //        flag = ReturnType.Removed;
            //    }
            //}
            return flag;
        }


        public int UpdatePropertyPremiumSchedulePaymentById(DataSourceRequest request, PaymentScheduleModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var payment = dbContext.PaymentScheduleTrans.Where(m => m.Id == model.Id && m.IsActive == true).FirstOrDefault();
                if (payment != null)
                {
                    payment.InstallmentNo = model.InstallmentNo;
                    payment.InstallmentDueDate = model.InstallmentDueDate;
                    payment.InstallmentAmount = model.InstallmentAmount;
                    payment.BalanceAmount = model.BalanceAmount;
                    payment.InterestAmount = model.InstallmentInterest;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }


        public PaymentScheduleModel SavePremiumPaymentDuesByRegistrationId(PaymentScheduleModel model)
        {
            if (model.ActionType == "InstallmentPayment")
            {
                using (var dbContext = new NoidaPMSEntities())
                {
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
                    model.ReturnTypeId = ReturnType.Saved;
                }
            }
            if (model.ActionType == "InstallmentSchedule")
            {
                model = SavePremiumPaymentScheduleByRegistrationId(model);
            }
            if (model.ActionType == "DefaultSchedule")
            {
                model = GenerateDefaultPremiumScheduleByRegistrationId(model);
            }
            return model;
        }

        private PaymentScheduleModel GenerateDefaultPremiumScheduleByRegistrationId(PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var schedule = dbContext.PaymentScheduleMasters.FirstOrDefault(r => r.Rid == model.RegistrationId);
                if (schedule == null)
                {
                    var allotment = dbContext.AllotmentMasters.Where(r => r.rid == model.RegistrationId && r.isActive == 1).FirstOrDefault();
                    var property = dbContext.SchemePropTrans.FirstOrDefault(p => p.propertyId == allotment.propertyId);
                    if (property.landRatePerSqmt != null && property.landRatePerSqmt != 0 && property.totalPropertyCost != null && property.totalPropertyCost != 0)
                    {
                        model.RegistrationId = allotment.rid;
                        model.PrincipalAmount = property.totalPropertyCost;
                        model.InstallmentStartDate = allotment.instalmentStartDate;
                        model.Frequency = 2;
                        model.TotalInstallment = 12;
                        model.NormalInterest = 10;
                        model.PenalInterest = 13;
                        var ScheduleFlag = dbContext.Sp_NewPaymentSchedule_OutSide(model.RegistrationId, model.PrincipalAmount, model.TotalInstallment, model.Frequency, model.InstallmentStartDate, model.NormalInterest, model.PenalInterest);
                        if (ScheduleFlag > 0)
                        {
                            model.ReturnTypeId = ReturnType.Saved;
                        }
                    }
                }
                return model;
            }
        }

        private PaymentScheduleModel SavePremiumPaymentScheduleByRegistrationId(PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var exSchedule = dbContext.PaymentScheduleMasters.FirstOrDefault(s => s.Rid == model.RegistrationId && s.IsActive == true);
                if (exSchedule == null)
                {
                    PaymentScheduleMaster schedule = new PaymentScheduleMaster();
                    schedule.Rid = model.RegistrationId;
                    schedule.DepartmentId = model.DepartmentId;
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
                    model.ReturnTypeId = ReturnType.Saved;
                    return model;
                }
                else
                {
                    exSchedule.PropertyId = model.PropertyId;
                    exSchedule.PrincipalAmount = model.PrincipalAmount;
                    exSchedule.NoOfInstallment = model.TotalInstallment;
                    exSchedule.FrequencyOfInstallment = model.Frequency;
                    exSchedule.PeriodOfInstallment = model.PeriodOfInstallment;
                    exSchedule.NormalInterest = model.NormalInterest;
                    exSchedule.PenalInterest = model.PenalInterest;
                    exSchedule.InstallmentStartDate = model.InstallmentStartDate;
                    exSchedule.ScheduleType = model.ScheduleType;
                    exSchedule.IsOneTimeLeaseRentPaid = model.OneTimeLeaseRentStatus == "Y" ? true : false;
                    exSchedule.IsOneTimeInstallmentPaid = model.LeaseRentStatus == "Y" ? true : false;
                    exSchedule.IsActive = true;
                    exSchedule.ModifiedBy = userInfo.UserID.ToString();
                    exSchedule.ModifiedDate = DateTime.Now;
                    dbContext.SaveChanges();

                    model.ScheduleId = exSchedule.ScheduleId;
                    model.ReturnTypeId = ReturnType.Updated;
                    return model;
                }
            }
        }


        public int SaveInstallmentDuesPaymentById(PaymentScheduleModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var dues = dbContext.InstallmentDuesPayments.FirstOrDefault(c => c.RegistrationId == model.RegistrationId);
                if (dues == null)
                {
                    InstallmentDuesPayment payment = new InstallmentDuesPayment();
                    payment.RegistrationId = model.RegistrationId;
                    payment.DepartmentId = model.DepartmentId;
                    payment.PropertyId = model.PropertyId;
                    payment.Applicant = model.Applicant;
                    payment.Sector = model.Sector;
                    payment.Block = model.Block;
                    payment.PlotNo = model.PlotNo;
                    payment.InstallmentAmount = model.InstallmentAmount;
                    payment.InstallmentStartDate = model.InstallmentStartDate;
                    payment.InstallmentEndDate = model.InstallmentEndDate;
                    payment.PenalInterest = model.PenalInterest;
                    payment.GST = model.GST;
                    payment.IsOneTimeLeasePaid = model.OneTimeLeaseRentStatus == "Y" ? true : false;
                    payment.IsTotalPremiumPaid = model.TotalPremiumPaidStatus == "Y" ? true : false; ;
                    payment.DuesAmount = model.DuesAmount;
                    payment.DuesUptoDate = model.DuesUptoDate;
                    payment.BalanceAmount = model.BalanceAmount;
                    payment.EntryDate = DateTime.Now;
                    payment.StatusId = NAStatusId.InProgress;
                    payment.IsActive = true;
                    payment.CreatedBy = userInfo.UserID.ToString();
                    payment.CreatedOn = DateTime.Now;
                    dbContext.InstallmentDuesPayments.Add(payment);
                    dbContext.SaveChanges();

                    model.PaymentId = payment.Id;
                    model.ReturnTypeId = ReturnType.Saved;
                    flag = ReturnType.Saved;
                }
                else
                {
                    dues.DepartmentId = model.DepartmentId;
                    dues.PropertyId = model.PropertyId;
                    dues.Applicant = model.Applicant;
                    dues.Sector = model.Sector;
                    dues.Block = model.Block;
                    dues.PlotNo = model.PlotNo;
                    dues.InstallmentAmount = model.InstallmentAmount;
                    dues.InstallmentStartDate = model.InstallmentStartDate;
                    dues.InstallmentEndDate = model.InstallmentEndDate;
                    dues.PenalInterest = model.PenalInterest;
                    dues.GST = model.GST;
                    dues.IsOneTimeLeasePaid = model.OneTimeLeaseRentStatus == "Y" ? true : false;
                    dues.IsTotalPremiumPaid = model.TotalPremiumPaidStatus == "Y" ? true : false;
                    dues.DuesAmount = model.DuesAmount;
                    dues.DuesUptoDate = model.DuesUptoDate;
                    dues.BalanceAmount = model.BalanceAmount;
                    dues.EntryDate = DateTime.Now;
                    dues.StatusId = NAStatusId.InProgress;
                    dues.IsActive = true;
                    dues.CreatedBy = userInfo.UserID.ToString();
                    dues.CreatedOn = DateTime.Now;

                    dbContext.SaveChanges();

                    model.ReturnTypeId = ReturnType.Updated;
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }


        public DataSourceResult GetInstallmentDuesPaidListByRegistrationId(DataSourceRequest request, PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from detail in dbContext.InstallmentDuesPayments
                            //join aplicant in dbContext.ApplicationDetails on detail.Rid equals aplicant.registrationId
                            where detail.RegistrationId == model.RegistrationId
                            select new PaymentScheduleModel
                            {
                                Id = detail.Id,
                                RegistrationId = detail.RegistrationId,
                                DepartmentId = detail.DepartmentId,
                                PropertyId = detail.PropertyId,
                                InstallmentAmount = detail.InstallmentAmount,
                                InstallmentStartDate = detail.InstallmentStartDate,
                                InstallmentEndDate = detail.InstallmentEndDate,
                                PenalInterest = detail.PenalInterest,
                                GST = detail.GST,
                                NDCDate = detail.NDCDate,
                                NDCStatus = detail.NDCStatus,
                                DuesAmount = detail.DuesAmount,
                                GSTAmount = detail.GstAmount,
                                DuesUptoDate = detail.DuesUptoDate,
                                BalanceInterest = detail.BalanceInterest,
                                BalanceAmount = detail.BalanceAmount,
                                BalanceUptoDate = detail.BalanceUptoDate,
                                PaymentMode = detail.PaymentMode,
                                TransactionId = detail.TransactionId,
                                IsOneTimeLeaseRentPaid = detail.IsOneTimeLeasePaid,
                                IsTotalPremiumPaid = detail.IsTotalPremiumPaid,
                                IsActive = detail.IsActive,
                                TotalPremiumPaidStatus = (detail.IsTotalPremiumPaid == null || detail.IsTotalPremiumPaid == true) ? "Paid" : "Not Paid",
                                OneTimeLeaseRentStatus = (detail.IsOneTimeLeasePaid == null || detail.IsOneTimeLeasePaid == true) ? "Paid" : "Not Paid",
                                ActionType = null
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public PaymentScheduleModel GetInstallmentDuesDetailByRegistrationId(PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from alotment in dbContext.AllotmentMasters
                              join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId.Value
                              join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                              join proptype in dbContext.PropertyTypeMsts on property.propertyTypeId equals proptype.propertyTypeId
                              where alotment.rid == model.RegistrationId
                              select new PaymentScheduleModel
                              {
                                  Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                  ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                  ApplicantType = aplicant.tGender,
                                  DepartmentId = alotment.departmentId,
                                  Department = alotment.DepartmentMst.departmentName,
                                  SectorId = property.sectorId,
                                  Sector = property.SectorMst.sectorName,
                                  BlockId = property.blockId,
                                  Block = (property.blockId == null || property.blockId <= 0) ? Constants.NA : property.BlockMst.blockName,
                                  PlotNo = property.propertyNo,
                                  PropertyNo = property.SectorMst.sectorName + "/" + ((property.blockId == null || property.blockId <= 0) ? string.Empty : property.BlockMst.blockName + "-") + property.propertyNo,
                                  PropertyType = proptype.propertyTypeName,
                                  AllotmentDate = alotment.allotmentDate,
                                  PropertyId = alotment.propertyId
                              }).FirstOrDefault();
                var dues = (from paydues in dbContext.InstallmentDuesPayments where paydues.RegistrationId == model.RegistrationId select paydues).OrderByDescending(s => s.Id).FirstOrDefault();
                if (dues != null)
                {
                    detail.Id = dues.Id;
                    detail.InstallmentAmount = dues.InstallmentAmount;
                    detail.IsOneTimeLeaseRentPaid = dues.IsOneTimeLeasePaid;
                    detail.IsTotalPremiumPaid = dues.IsTotalPremiumPaid;
                    detail.OneTimeLeaseRentStatus = (dues.IsOneTimeLeasePaid == null || dues.IsOneTimeLeasePaid == false) ? "N" : "Y";
                    detail.TotalPremiumPaidStatus = (dues.IsTotalPremiumPaid == null || dues.IsTotalPremiumPaid == false) ? "N" : "Y";
                    detail.GST = dues.GST;
                    detail.PenalInterest = dues.PenalInterest;
                    detail.DuesAmount = dues.DuesAmount;
                    detail.InstallmentStartDate = dues.InstallmentStartDate;
                    detail.InstallmentEndDate = dues.InstallmentEndDate;
                    detail.DuesUptoDate = dues.DuesUptoDate;
                    detail.BalanceAmount = dues.BalanceAmount;
                }
                return detail;
            }
        }


        public int GeneratePremiumScheduleByDepartment(PaymentScheduleModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var allotmentList = dbContext.AllotmentMasters.Where(r => r.departmentId == model.DepartmentId && r.isActive == 1).ToList();
                foreach (var allotment in allotmentList)
                {
                    var schedule = dbContext.PaymentScheduleMasters.FirstOrDefault(r => r.Rid == allotment.rid);
                    if (schedule == null)
                    {
                        var property = dbContext.SchemePropTrans.FirstOrDefault(p => p.propertyId == allotment.propertyId);
                        if (property.landRatePerSqmt != null && property.landRatePerSqmt != 0 && property.totalPropertyCost != null && property.totalPropertyCost != 0)
                        {
                            model.RegistrationId = allotment.rid;
                            model.PrincipalAmount = property.totalPropertyCost;
                            model.InstallmentStartDate = allotment.instalmentStartDate;
                            model.Frequency = 2;
                            model.TotalInstallment = 12;
                            model.NormalInterest = 10;
                            model.PenalInterest = 13;
                            var ScheduleFlag = dbContext.Sp_NewPaymentSchedule_OutSide(model.RegistrationId, model.PrincipalAmount, model.TotalInstallment, model.Frequency, model.InstallmentStartDate, model.NormalInterest, model.PenalInterest);
                            if (ScheduleFlag > 0) { flag = ReturnType.Saved; }
                        }
                    }
                }
            }
            return flag;
        }

        public int RemovePremiumScheduleByDepartment(PaymentScheduleModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var allotmentList = dbContext.AllotmentMasters.Where(r => r.departmentId == model.DepartmentId && r.isActive == 1).ToList();
                foreach (var allotment in allotmentList)
                {
                    var scheduleList = dbContext.PaymentScheduleMasters.Where(r => r.Rid == allotment.rid).ToList();
                    if (scheduleList != null && scheduleList.Count > 1)
                    {
                        foreach (var schedule in scheduleList)
                        {
                            model.RegistrationId = schedule.Rid;
                            var stat = RemovePropertyPremiumScheduleById(model);
                            flag = ReturnType.Removed;
                        }
                    }
                }
            }
            return flag;
        }

        public int GeneratePremiumScheduleByRegistrationId(PaymentScheduleModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var allotment = dbContext.AllotmentMasters.FirstOrDefault(r => r.departmentId == model.DepartmentId && r.isActive == 1);
                if (allotment != null)
                {
                    var schedule = dbContext.PaymentScheduleMasters.FirstOrDefault(r => r.Rid == allotment.rid);
                    if (schedule == null)
                    {
                        var property = dbContext.SchemePropTrans.FirstOrDefault(p => p.propertyId == allotment.propertyId);
                        model.RegistrationId = allotment.rid;
                        model.PrincipalAmount = property.totalPropertyCost;
                        model.InstallmentStartDate = allotment.instalmentStartDate;
                        model.Frequency = 2;
                        model.TotalInstallment = 12;
                        model.NormalInterest = 10;
                        model.PenalInterest = 13;
                        var ScheduleFlag = dbContext.Sp_NewPaymentSchedule_OutSide(model.RegistrationId, model.PrincipalAmount, model.TotalInstallment, model.Frequency, model.InstallmentStartDate, model.NormalInterest, model.PenalInterest);
                        if (ScheduleFlag > 0) { flag = ReturnType.Saved; }
                    }
                }
                else flag = ReturnType.Exist;
            }
            return flag;
        }


        public DataSourceResult GetPropertyAccountSummery(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from alotment in dbContext.AllotmentMasters
                            from property in dbContext.SchemePropTrans.Where(p => p.propertyId == alotment.propertyId).DefaultIfEmpty()
                            from aplicant in dbContext.ApplicationDetails.Where(a => a.registrationId == alotment.rid).DefaultIfEmpty()
                            where alotment.isActive == 1
                            && (model.RegistrationId == null || alotment.rid == model.RegistrationId)
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
                                BlockName = property.blockId == null ? string.Empty : property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                PropertyNo = property.SectorMst.sectorName + "/" + (property.blockId == null ? string.Empty : property.BlockMst.blockName + " - ") + property.propertyNo,
                                ActualArea = property.actualArea,
                                CoveredArea = property.coveredArea,
                                TotalArea = property.totalArea,
                                CivilCost = property.civilCost,
                                PropertyCost = property.propertyCost,
                                TotalPropertyCost = property.totalPropertyCost,
                                AllotmentMoney = property.allotmentMoney,
                                LandRate = property.landRatePerSqmt,
                                ApplicantType = aplicant.tGender,
                                FirstApplicant = aplicant.gender == Constants.Company ? aplicant.firstName : aplicant.firstName + " " + (string.IsNullOrEmpty(aplicant.middleName) ? string.Empty : aplicant.middleName + " ") + aplicant.lastName,
                                Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                ApplicantAddress = aplicant.tCorrespondanceAdd,
                                FirstApplicantAdd = aplicant.correspondanceAdd,
                                ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                AllotmentDate = alotment.allotmentDate,
                                RegistryDate = dbContext.RegistryDetails.FirstOrDefault(r => r.Rid == alotment.rid) != null ? dbContext.RegistryDetails.FirstOrDefault(r => r.Rid == alotment.rid).RegistryDoneDate : null,
                                Status = alotment.isStatus == "Approved" ? "Allotted" : "Not Allotted",
                                MobileNo = aplicant.tMobileNumber,
                                Email = aplicant.tEmail,
                                IsPropertyAllotted = string.IsNullOrEmpty(aplicant.isAllotted) ? false : true,
                                IsActive = (alotment.isActive == null || alotment.isActive == 0) ? false : true
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public LeaseRentViewModel GetMiscellaneousPaymentCountByDepartment(LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                //LeaseRentViewModel model = new LeaseRentViewModel();

                model.TotalLeaseRentCount = dbcontext.LeaseRentPayments.Where(r => r.IsOneTimeLeasePaid == false && (model.DepartmentId == null || r.DepartmentId == model.DepartmentId)).Count();//&& r.IsTotalPremiumPaid == true
                model.TotalNDCCount = dbcontext.LeaseRentPayments.Where(r => r.IsOneTimeLeasePaid == true && r.IsTotalPremiumPaid == true && (model.DepartmentId == null || r.DepartmentId == model.DepartmentId)).Count();
                model.TotalIPCount = dbcontext.LeaseRentPayments.Where(r => r.IsTotalPremiumPaid == false && r.IsOneTimeLeasePaid == false && (model.DepartmentId == null || r.DepartmentId == model.DepartmentId)).Count();
                //model.TotalIPCount = dbcontext.InstallmentDuesPayments.Where(r => r.IsTotalPremiumPaid == false && (model.DepartmentId == null || r.DepartmentId == model.DepartmentId)).Count();
                model.DefaulterCount = dbcontext.LeaseRentPayments.Where(r => r.IsOneTimeLeasePaid == false && r.IsTotalPremiumPaid == false && (model.DepartmentId == null || r.DepartmentId == model.DepartmentId)).Count();
                return model;
            }
        }


        public IEnumerable<LeaseRentViewModel> GetTotalCountByDepartment(LeaseRentViewModel model)
        {
            IEnumerable<LeaseRentViewModel> departmentSummaryCount;
            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {

                departmentSummaryCount = connection.Query<LeaseRentViewModel>("Sp_ManageaccountDetailDepartmentwise", new { departmentId = model.DepartmentId, type = 2 }, commandType: System.Data.CommandType.StoredProcedure);

            }
            return departmentSummaryCount;

        }


        public DataSourceResult GetDemandNoteListAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (HttpContext.Current.Session["DNDCDepartmentId"] != null) model.DepartmentId = (int)HttpContext.Current.Session["DNDCDepartmentId"];
                var list = (from demand in dbContext.DemandNoteDetails
                            join alotment in dbContext.AllotmentMasters on demand.RegistrationId equals alotment.rid
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where DepartmentList.Contains(demand.DepartmentId)
                              && (model.Id == null || demand.Id == model.Id)
                              && (model.RegistrationId == null || demand.RegistrationId == model.RegistrationId)
                              && (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                              && (model.DemandNoteTypeId == null || demand.DemandNoteTypeId == model.DemandNoteTypeId)
                              && (model.StartDate == null || DbFunctions.TruncateTime(demand.CreatedDate) >= DbFunctions.TruncateTime(model.StartDate))
                              && (model.EndDate == null || DbFunctions.TruncateTime(demand.CreatedDate) <= DbFunctions.TruncateTime(model.EndDate))
                            select new PaymentViewModel
                            {
                                Id = demand.Id,
                                RegistrationId = demand.RegistrationId,
                                RegistrationNo = demand.RegistrationId.ToString(),
                                PropertyId = demand.PropertyId.ToString(),
                                DepartmentId = demand.DepartmentId,
                                Department = demand.DepartmentId == null ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == demand.DepartmentId).departmentName,
                                SectorName = property.SectorMst.sectorName,
                                BlockName = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                InstallmentAmount = demand.InstallmentDues,
                                InstallmentInterest = demand.InstallmentDuesInterest,
                                LeaseRentDues = demand.LeaseRentDues,
                                LeaseRentInterest = demand.LeaseRentDuesInterest,
                                OtherCharges = demand.OtherCharges,
                                TotalDuesAmount = demand.TotalDuesAmount,
                                DemandNoteTypeId = demand.DemandNoteTypeId,
                                DemandNoteType = demand.DemandNoteTypeId == null ? string.Empty : (demand.DemandNoteTypeId == Constants.InstallmentDemandNoteId ? "Installment" : (demand.DemandNoteTypeId == Constants.LeaseRentDemandNoteId ? "Lease Rent" : (demand.DemandNoteTypeId == Constants.InstallmentAndLeaseRentDemandNoteId ? "Installment&Leaserent" : string.Empty))),
                                DemandNoteContent = demand.DemandNoteTemplate,
                                StatusId = demand.StatusId,
                                DemandNoteStatus = demand.StatusId == null ? string.Empty : dbContext.StatusMasters.FirstOrDefault(s => s.Id == demand.StatusId).Status,
                                IsActive = demand.IsActive,
                                CreatedDate = demand.CreatedDate
                            });
                HttpContext.Current.Session["DNDCDepartmentId"] = null;
                return list != null ? list.ToDataSourceResult(request) : null;
            }
        }

        public DataSourceResult GetDemandNoteListAsDataSourceII(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (HttpContext.Current.Session["DNDCDepartmentId"] != null) model.DepartmentId = (int)HttpContext.Current.Session["DNDCDepartmentId"];
                var list = (from demand in dbContext.DemandNoteDetails.GroupBy(g => g.RegistrationId).Select(x => x.FirstOrDefault())
                            join alotment in dbContext.AllotmentMasters on demand.RegistrationId equals alotment.rid
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where DepartmentList.Contains(demand.DepartmentId)
                              && (model.Id == null || demand.Id == model.Id)
                              && (model.RegistrationId == null || demand.RegistrationId == model.RegistrationId)
                              && (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                              && (model.DemandNoteTypeId == null || demand.DemandNoteTypeId == model.DemandNoteTypeId)
                              && (model.StartDate == null || DbFunctions.TruncateTime(demand.CreatedDate) >= DbFunctions.TruncateTime(model.StartDate))
                              && (model.EndDate == null || DbFunctions.TruncateTime(demand.CreatedDate) <= DbFunctions.TruncateTime(model.EndDate))
                            select new PaymentViewModel
                            {
                                Id = demand.Id,
                                RegistrationId = demand.RegistrationId,
                                RegistrationNo = demand.RegistrationId.ToString(),
                                PropertyId = demand.PropertyId.ToString(),
                                DepartmentId = demand.DepartmentId,
                                Department = demand.DepartmentId == null ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == demand.DepartmentId).departmentName,
                                SectorName = property.SectorMst.sectorName,
                                BlockName = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                InstallmentAmount = demand.InstallmentDues,
                                InstallmentInterest = demand.InstallmentDuesInterest,
                                LeaseRentDues = demand.LeaseRentDues,
                                LeaseRentInterest = demand.LeaseRentDuesInterest,
                                OtherCharges = demand.OtherCharges,
                                TotalDuesAmount = demand.TotalDuesAmount,
                                DemandNoteTypeId = demand.DemandNoteTypeId,
                                DemandNoteType = demand.DemandNoteTypeId == null ? string.Empty : (demand.DemandNoteTypeId == Constants.InstallmentDemandNoteId ? "Installment" : (demand.DemandNoteTypeId == Constants.LeaseRentDemandNoteId ? "Lease Rent" : (demand.DemandNoteTypeId == Constants.InstallmentAndLeaseRentDemandNoteId ? "Installment&Leaserent" : string.Empty))),
                                DemandNoteContent = demand.DemandNoteTemplate,
                                StatusId = demand.StatusId,
                                DemandNoteStatus = demand.StatusId == null ? string.Empty : dbContext.StatusMasters.FirstOrDefault(s => s.Id == demand.StatusId).Status,
                                IsActive = demand.IsActive,
                                CreatedDate = demand.CreatedDate
                            }).Distinct();
                HttpContext.Current.Session["DNDCDepartmentId"] = null;
                return list != null ? list.ToDataSourceResult(request) : null;
            }
        }


        public int UpdateInstallmentDuesPaymentII(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var flag = ReturnType.None;
                // in future should be changed on Id
                if (model.RegistrationId != null && model.RegistrationId != 0)
                {
                    if (model.ActionType == "Installment")
                    {
                        var installment = dbContext.InstallmentDuesPayments.OrderByDescending(r => r.Id).Where(r => r.RegistrationId == model.RegistrationId).FirstOrDefault();
                        if (installment != null)
                        {
                            var duesList = CalculateInstallmentDuesIII(model);
                            if (duesList != null && duesList.Count > 0)
                            {
                                var installmentList = dbContext.InstallmentDuesPaymentTrans.Where(m => m.RegistrationId == model.RegistrationId).ToList();
                                if (installmentList != null) dbContext.InstallmentDuesPaymentTrans.RemoveRange(installmentList);

                                Nullable<Decimal> totalDues = 0;
                                for (int i = 0; i < duesList.Count; i++)
                                {
                                    var duesPayment = duesList.ElementAt(i);
                                    totalDues = totalDues + duesPayment.DuesAmount;
                                    if (i == duesList.Count - 1)
                                    {
                                        var payment = new InstallmentDuesPaymentTran();
                                        payment.RefId = installment.Id;
                                        payment.RegistrationId = model.RegistrationId;
                                        payment.Installment = duesPayment.InstallmentAmount;
                                        payment.DuesAmount = duesPayment.DuesAmount;
                                        payment.NormalInterest = duesPayment.InterestAmount;
                                        payment.PenalInterest = duesPayment.PenalInterest;
                                        payment.GST = duesPayment.GSTAmount;
                                        payment.DuesUptoDate = duesPayment.DuesUptoDate;
                                        var duedate = duesPayment.DuesUptoDate.Value.AddDays(-1);
                                        payment.DepositDueDate = duedate;
                                        payment.HtmlDuesTemplate = duesPayment.HtmlTemplate;
                                        payment.StatusId = NAStatusId.Initiated;
                                        payment.IsActive = true;
                                        payment.CreatedDate = DateTime.Now;
                                        payment.CreatedBy = userInfo.UserID;
                                        dbContext.InstallmentDuesPaymentTrans.Add(payment);
                                        dbContext.SaveChanges();

                                        installment.PreviousDues = installment.CurrentDues;
                                        installment.PreviousDuesDate = installment.PreviousDuesDate;
                                        installment.CurrentDues = duesPayment.DuesAmount;
                                        installment.CurrentDuesDate = duesPayment.DuesUptoDate;

                                        //leaserent.LeaseRentDues = duesrent.DuesAmount;
                                        installment.DuesAmount = totalDues;
                                        installment.DuesUptoDate = duesPayment.DuesUptoDate;

                                        installment.PreviousDues = duesPayment.CurrentDues;
                                        //installment.PreviousDuesDate = duesPayment.CurrentDuesDate;
                                        installment.CurrentDues = totalDues;
                                        installment.CurrentDuesDate = duesPayment.DuesUptoDate;
                                        installment.HtmlDuesTemplate = duesPayment.HtmlTemplate;
                                        dbContext.SaveChanges();
                                        flag = ReturnType.Updated;
                                    }
                                    else
                                    {
                                        var payment = new InstallmentDuesPaymentTran();
                                        payment.RefId = installment.Id;
                                        payment.RegistrationId = model.RegistrationId;
                                        payment.Installment = duesPayment.InstallmentAmount;
                                        payment.DuesAmount = duesPayment.DuesAmount;
                                        payment.NormalInterest = duesPayment.InterestAmount;
                                        payment.PenalInterest = duesPayment.PenalInterest;
                                        payment.GST = duesPayment.GSTAmount;
                                        payment.DuesUptoDate = duesPayment.DuesUptoDate;
                                        var duedate = duesPayment.DuesUptoDate.Value.AddDays(-1);
                                        payment.DepositDueDate = duedate;
                                        payment.HtmlDuesTemplate = duesPayment.HtmlTemplate;
                                        payment.StatusId = NAStatusId.Initiated;
                                        payment.IsActive = true;
                                        payment.CreatedDate = DateTime.Now;
                                        payment.CreatedBy = userInfo.UserID;
                                        dbContext.InstallmentDuesPaymentTrans.Add(payment);
                                        dbContext.SaveChanges();
                                        flag = ReturnType.Updated;
                                    }
                                }
                            }
                        }
                    }
                }
                return flag;
            }
        }


        public DataSourceResult GetInstallmentDuesPaymentListById(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from installment in dbContext.InstallmentDuesPaymentTrans
                            where (model.RefId == null || installment.RefId == model.RefId)
                            select new PaymentViewModel
                            {
                                Id = installment.Id,
                                RegistrationId = installment.RegistrationId,
                                RegistrationNo = installment.RegistrationId.ToString(),
                                DuesAmount = installment.DuesAmount,
                                GSTAmount = installment.GST,

                                InstallmentAmount = installment.Installment,
                                InterestAmount = installment.NormalInterest,
                                PenalInterest = installment.PenalInterest,
                                InstallmentDueDate = installment.DepositDueDate,
                                InstallmentDuesUptoDate = installment.DuesUptoDate,
                                PaidAmount = installment.PaidAmount,
                                DepositDate = installment.DepositDate,
                                HtmlTemplate = installment.HtmlDuesTemplate,
                                PaymentStatus = installment.StatusId != null ? (dbContext.StatusMasters.FirstOrDefault(m => m.Id == installment.StatusId).Status) : string.Empty,
                                IsDuesUptoDate = (installment.DuesUptoDate == null || DbFunctions.TruncateTime(installment.DuesUptoDate) < DbFunctions.TruncateTime(DateTime.Now)) ? false : true
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetLeaserentDuesPaymentListById(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from leaserent in dbContext.PaymentLeaseRentTrans
                            where leaserent.RentId == model.RefId
                            select new PaymentViewModel
                            {
                                Id = leaserent.Id,
                                RegistrationId = leaserent.RegistrationId,
                                RegistrationNo = leaserent.RegistrationId.ToString(),
                                LeaseRentPremium = leaserent.LeaseRentPremium,
                                DepositDueDate = leaserent.DepositDueDate,
                                DuesUptoDate = leaserent.DuesUptoDate,
                                DuesAmount = leaserent.DuesAmount,
                                HtmlTemplate = leaserent.HtmlDuesTemplate,
                                PaymentStatus = leaserent.StatusId != null ? (dbContext.StatusMasters.FirstOrDefault(m => m.Id == leaserent.StatusId).Status) : string.Empty,
                                IsDuesUptoDate = (leaserent.DuesUptoDate == null || DbFunctions.TruncateTime(leaserent.DuesUptoDate) < DbFunctions.TruncateTime(DateTime.Now)) ? false : true
                            });
                var datalist = list.ToDataSourceResult(request);
                return datalist;
            }
        }


        public DataSourceResult GetInstallmentPaymentScheduleListById(DataSourceRequest request, PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from installment in dbContext.InstallmentDuesPaymentTrans
                            where (model.RefId == null || installment.RefId == model.RefId)
                            select new PaymentScheduleModel
                            {
                                Id = installment.Id,
                                RegistrationId = installment.RegistrationId,
                                DuesAmount = installment.DuesAmount,
                                GSTAmount = installment.GST,
                                PaymentMode = installment.PaymentMode,
                                TransactionId = installment.TransactionId,
                                InstallmentAmount = installment.Installment,
                                NormalInterest = installment.NormalInterest,
                                PenalInterest = installment.PenalInterest,
                                InstallmentDueDate = installment.DepositDueDate,
                                DuesUptoDate = installment.DuesUptoDate,
                                PaidAmount = installment.PaidAmount,
                                DepositDate = installment.DepositDate,
                                HtmlTemplate = installment.HtmlDuesTemplate,
                                PaymentStatus = installment.StatusId != null ? (dbContext.StatusMasters.FirstOrDefault(m => m.Id == installment.StatusId).Status) : string.Empty,
                                IsDuesUptoDate = (installment.DuesUptoDate == null || DbFunctions.TruncateTime(installment.DuesUptoDate) < DbFunctions.TruncateTime(DateTime.Now)) ? false : true
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public int UpdateInstallmentDuesPaymentByIdII(PaymentScheduleModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.RegistrationId != null && model.RegistrationId != 0)
                {
                    var installment = dbContext.InstallmentDuesPayments.OrderByDescending(r => r.Id).Where(r => r.RegistrationId == model.RegistrationId && r.IsTotalPremiumPaid == false).FirstOrDefault();
                    if (installment != null)
                    {
                        var duesList = CalculateInstallmentDuesPaymentById(model);

                        if (duesList != null && duesList.Count > 0)
                        {
                            var propertyInfo = (from alotment in dbContext.AllotmentMasters
                                                join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                                                where alotment.rid == model.RegistrationId
                                                select property).FirstOrDefault();

                            var installmentList = dbContext.InstallmentDuesPaymentTrans.Where(m => m.RegistrationId == model.RegistrationId).ToList();
                            if (installmentList != null) dbContext.InstallmentDuesPaymentTrans.RemoveRange(installmentList);

                            Nullable<Decimal> totalDues = 0;
                            for (int i = 0; i < duesList.Count; i++)
                            {
                                var duesPayment = duesList.ElementAt(i);
                                totalDues = totalDues + duesPayment.DuesAmount;
                                if (i == duesList.Count - 1)
                                {
                                    var payment = new InstallmentDuesPaymentTran();
                                    payment.RefId = installment.Id;
                                    payment.RegistrationId = model.RegistrationId;
                                    //payment.Installment = duesPayment.InstallmentAmount;
                                    payment.Installment = installment.InstallmentAmount;
                                    payment.DuesAmount = duesPayment.DuesAmount;
                                    payment.NormalInterest = duesPayment.NormalInterest;
                                    payment.PenalInterest = duesPayment.PenalInterest;
                                    payment.GST = duesPayment.GSTAmount;
                                    payment.DuesUptoDate = duesPayment.DuesUptoDate;
                                    var duedate = duesPayment.DuesUptoDate.Value.AddDays(-1);
                                    payment.DepositDueDate = duedate;
                                    payment.HtmlDuesTemplate = duesPayment.HtmlTemplate;
                                    payment.StatusId = NAStatusId.Initiated;
                                    payment.IsActive = true;
                                    payment.CreatedDate = DateTime.Now;
                                    payment.CreatedBy = userInfo.UserID;
                                    dbContext.InstallmentDuesPaymentTrans.Add(payment);
                                    dbContext.SaveChanges();

                                    installment.DepartmentId = propertyInfo.departmentId;
                                    installment.PropertyId = propertyInfo.propertyId;
                                    installment.Sector = propertyInfo.SectorMst.sectorName;
                                    installment.Block = propertyInfo.BlockMst.blockName;
                                    installment.PlotNo = propertyInfo.propertyNo;

                                    installment.PreviousDues = installment.CurrentDues;
                                    installment.PreviousDuesDate = installment.CurrentDuesDate;
                                    installment.CurrentDues = totalDues; // duesPayment.DuesAmount;
                                    installment.CurrentDuesDate = duesPayment.DuesUptoDate;
                                    //installment.DuesAmount = totalDues;
                                    installment.DuesUptoDate = duesPayment.DuesUptoDate;
                                    installment.HtmlDuesTemplate = duesPayment.HtmlTemplate;
                                    dbContext.SaveChanges();
                                    flag = ReturnType.Updated;
                                }
                                else
                                {
                                    var payment = new InstallmentDuesPaymentTran();
                                    payment.RefId = installment.Id;
                                    payment.RegistrationId = model.RegistrationId;
                                    //payment.Installment = duesPayment.InstallmentAmount;
                                    payment.Installment = installment.InstallmentAmount;
                                    payment.DuesAmount = duesPayment.DuesAmount;
                                    payment.NormalInterest = duesPayment.NormalInterest;
                                    payment.PenalInterest = duesPayment.PenalInterest;
                                    payment.GST = duesPayment.GSTAmount;
                                    payment.DuesUptoDate = duesPayment.DuesUptoDate;
                                    var duedate = duesPayment.DuesUptoDate.Value.AddDays(-1);
                                    payment.DepositDueDate = duedate;
                                    payment.HtmlDuesTemplate = duesPayment.HtmlTemplate;
                                    payment.StatusId = NAStatusId.Initiated;
                                    payment.IsActive = true;
                                    payment.CreatedDate = DateTime.Now;
                                    payment.CreatedBy = userInfo.UserID;
                                    dbContext.InstallmentDuesPaymentTrans.Add(payment);
                                    dbContext.SaveChanges();
                                    flag = ReturnType.Updated;
                                }
                            }
                        }
                    }
                }
                return flag;
            }
        }

        private List<PaymentScheduleModel> CalculateInstallmentDuesPaymentById(PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<PaymentScheduleModel> installmentDuesList = new List<PaymentScheduleModel>();
                var data = dbContext.InstallmentDuesPayments.FirstOrDefault(r => r.RegistrationId == model.RegistrationId && r.IsTotalPremiumPaid == false);
                if (data != null)
                {
                    if (data.BalanceUptoDate != null)
                    {
                        var balanceUptoDate = data.BalanceUptoDate;
                        decimal rate = data.PenalInterest == null ? 0 : (decimal)data.PenalInterest;

                        var today = DateTime.Now;
                        var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                        today = DateTime.Now.AddDays(days - DateTime.Now.Day);

                        var toNextDate = data.BalanceUptoDate == null ? DateTime.Now : data.BalanceUptoDate;
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
                                PaymentScheduleModel payModel = new PaymentScheduleModel();
                                var balanceAmount = data.BalanceAmount == null ? 0 : data.BalanceAmount;
                                var gst = data.GST == null ? 0 : data.GST;
                                toNextDate = balanceUptoDate.Value.AddMonths(12);
                                nextdate = balanceUptoDate.Value.AddMonths(6);

                                decimal? htmldues = 0;
                                decimal? htmlbalance = 0;
                                decimal? htmlinterest = 0;
                                decimal? htmlpaidamount = 0;
                                string previousYear = string.Empty;
                                string nextYear = string.Empty;
                                string htmldate = string.Empty;

                                if (today > toNextDate)
                                {
                                    var dues = GetInstallmentDuesByDate((int)model.RegistrationId, (decimal)balanceAmount, (decimal)rate, (decimal)gst, (DateTime)balanceUptoDate, (DateTime)toNextDate, "yearly");
                                    var payment = GetPaidInstallmentDetail((int)model.RegistrationId, (DateTime)balanceUptoDate, (DateTime)toNextDate);

                                    duesAmount = payment.PaidAmount != null ? ((decimal)dues.DuesAmount - (decimal)payment.PaidAmount) : (decimal)dues.BalanceAmount;
                                    payModel.DuesAmount = duesAmount;
                                    payModel.NormalInterest = dues.InterestAmount;
                                    payModel.PenalInterest = dues.InterestAmount;
                                    payModel.GSTAmount = dues.GSTAmount;
                                    payModel.BalanceInterest = dues.BalanceInterest;
                                    payModel.DuesUptoDate = toNextDate;
                                    balanceUptoDate = balanceUptoDate.Value.AddMonths(12);
                                }
                                else
                                {
                                    if (today > nextdate)
                                    {
                                        var dues = GetInstallmentDuesByDate((int)model.RegistrationId, (decimal)balanceAmount, (decimal)rate / 2, (decimal)gst, (DateTime)balanceUptoDate, (DateTime)nextdate, "yearly");
                                        var payment = GetPaidInstallmentDetail((int)model.RegistrationId, (DateTime)balanceUptoDate, (DateTime)nextdate);

                                        duesAmount = payment.PaidAmount != null ? ((decimal)dues.DuesAmount - (decimal)payment.PaidAmount) : (decimal)dues.BalanceAmount;
                                        payModel.DuesAmount = duesAmount;
                                        payModel.NormalInterest = dues.InterestAmount;
                                        payModel.PenalInterest = dues.InterestAmount;
                                        payModel.GSTAmount = dues.GSTAmount;
                                        payModel.BalanceInterest = dues.BalanceInterest;
                                        payModel.DuesUptoDate = nextdate;
                                        balanceUptoDate = balanceUptoDate.Value.AddMonths(6);
                                    }
                                    else
                                    {
                                        var dues = GetInstallmentDuesByDate((int)model.RegistrationId, (decimal)balanceAmount, (decimal)rate, (decimal)gst, (DateTime)balanceUptoDate, DateTime.Now.Date, "perday");
                                        var payment = GetPaidInstallmentDetail((int)model.RegistrationId, (DateTime)balanceUptoDate, DateTime.Now.Date);

                                        duesAmount = payment.PaidAmount != null ? ((decimal)dues.DuesAmount - (decimal)payment.PaidAmount) : (decimal)dues.BalanceAmount;
                                        payModel.DuesAmount = duesAmount;
                                        payModel.NormalInterest = dues.InterestAmount;
                                        payModel.PenalInterest = dues.InterestAmount;
                                        payModel.GSTAmount = dues.GSTAmount;
                                        payModel.BalanceInterest = dues.BalanceInterest;
                                        payModel.DuesUptoDate = nextdate;
                                        balanceUptoDate = today;
                                    }

                                }
                                htmldate = balanceUptoDate.Value.Day + "-" + balanceUptoDate.Value.Month + "-" + balanceUptoDate.Value.Year;
                                string fy = previousYear + "-" + nextYear;
                                dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmlbalance + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + htmlpaidamount + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmldate + "</div></div>";

                                payModel.HtmlTemplate = dueshtml;

                                installmentDuesList.Add(payModel);
                            }
                        }
                        else
                        {
                            //model.LeaseRentPremium = data.BalanceAmount;
                            //model.GSTAmount = 0;
                            //model.BalanceUptoDate = data.BalanceUptoDate;
                        }
                    }
                }
                return installmentDuesList;
            }
        }


        public int UpdateLeaseRentDuesPaymentByIdII(LeaseRentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbcontext = new NoidaPMSEntities())
            {
                if (model.RegistrationId != null && model.RegistrationId != 0)
                {
                    var leaserent = dbcontext.LeaseRentPayments.OrderByDescending(r => r.Id).Where(r => r.RegistrationId == model.RegistrationId && r.IsOneTimeLeasePaid == false).FirstOrDefault();
                    if (leaserent != null)
                    {
                        var propertyInfo = (from alotment in dbcontext.AllotmentMasters
                                            join property in dbcontext.SchemePropTrans on alotment.propertyId equals property.propertyId
                                            where alotment.rid == leaserent.RegistrationId
                                            select property).FirstOrDefault();

                        var rentList = CalculateLeaseRentDuesPaymentById(model);
                        var list = dbcontext.LeaseRentPaymentTrans.Where(r => r.RegistrationId == model.RegistrationId).ToList();
                        if (list != null) dbcontext.LeaseRentPaymentTrans.RemoveRange(list);

                        Nullable<Decimal> totalDues = 0;
                        for (int i = 0; i < rentList.Count; i++)
                        {
                            var duesrent = rentList.ElementAt(i);
                            totalDues = totalDues + duesrent.DuesAmount;
                            if (i == rentList.Count - 1)
                            {
                                var rent = new LeaseRentPaymentTran();
                                rent.RegistrationId = model.RegistrationId;
                                rent.RefId = leaserent.Id;
                                rent.LeaseRentPremium = duesrent.LeaseRentPremium;
                                rent.NormalInterest = duesrent.NormalInterest;
                                rent.PenalInterest = duesrent.PanelInterest;
                                var duedate = duesrent.DepositDueDate.Value.AddDays(-1);
                                rent.DepositDueDate = duedate;
                                rent.StatusId = NAStatusId.Initiated;
                                rent.IsActive = true;
                                rent.DuesAmount = duesrent.DuesAmount;
                                rent.BalanceAmount = duesrent.BalanceAmount;
                                //rent.NormalInterest = duesrent.NormalInterest;
                                rent.DuesUptoDate = duesrent.DuesUptoDate;
                                rent.HtmlTemplate = duesrent.HtmlDuesReport;
                                rent.CreatedDate = DateTime.Now;
                                rent.CreatedBy = userInfo.UserID;
                                dbcontext.LeaseRentPaymentTrans.Add(rent);
                                dbcontext.SaveChanges();

                                //leaserent.PremiumLeaseRent = duesrent.LeaseRentPremium;
                                //leaserent.Applicant = dbcontext.ApplicationDetails.Where(r=>r.registrationId==model.RegistrationId).Select(r=>r.tGender==Constants.Company ? (string.IsNullOrEmpty(r.T_Company_Name) ? r.tFirstName : r.T_Company_Name) : (r.tFirstName + (string.IsNullOrEmpty(r.tMiddleName) ? string.Empty : r.tMiddleName) + r.tLastName)).ToString();
                                leaserent.Sector = propertyInfo.SectorMst.sectorName;
                                leaserent.Block = propertyInfo.BlockMst.blockName;
                                leaserent.PlotNo = propertyInfo.propertyNo;
                                //leaserent.LeaseRentDues = totalDues;
                                //leaserent.DuesUptoDate = duesrent.DuesUptoDate;

                                leaserent.PreviousDues = leaserent.CurrentDues;
                                leaserent.PreviousDuesDate = leaserent.CurrentDuesDate;
                                leaserent.CurrentDues = totalDues;
                                leaserent.CurrentDuesDate = duesrent.DuesUptoDate;
                                leaserent.HtmlDuesTemplate = duesrent.HtmlDuesReport;
                                dbcontext.SaveChanges();
                                model.HtmlDuesReport = duesrent.HtmlDuesReport;
                            }
                            else
                            {
                                var rent = new LeaseRentPaymentTran();
                                rent.RegistrationId = model.RegistrationId;
                                rent.RefId = leaserent.Id;
                                rent.LeaseRentPremium = duesrent.LeaseRentPremium;
                                rent.NormalInterest = duesrent.NormalInterest;
                                rent.PenalInterest = duesrent.PanelInterest;
                                var duedate = duesrent.DepositDueDate.Value.AddDays(-1);
                                rent.DepositDueDate = duedate;
                                rent.StatusId = NAStatusId.Initiated;
                                rent.IsActive = true;
                                rent.DuesAmount = duesrent.DuesAmount;
                                rent.BalanceAmount = duesrent.BalanceAmount;
                                rent.DuesUptoDate = duesrent.DuesUptoDate;
                                rent.HtmlTemplate = duesrent.HtmlDuesReport;
                                rent.CreatedDate = DateTime.Now;
                                rent.CreatedBy = userInfo.UserID;
                                dbcontext.LeaseRentPaymentTrans.Add(rent);
                                dbcontext.SaveChanges();
                            }
                        }

                        flag = ReturnType.Updated;
                    }
                    else
                    {
                        flag = ReturnType.Paid;
                    }
                }
            }
            return flag;
        }

        private List<LeaseRentViewModel> CalculateLeaseRentDuesPaymentById(LeaseRentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<LeaseRentViewModel> rentList = new List<LeaseRentViewModel>();
                //check in Lease Rent Payment Details
                var leasemaster = dbContext.LeaseRentPayments.Where(r => r.RegistrationId == model.RegistrationId).OrderByDescending(d => d.Id).ToList();
                var registry = dbContext.RegistryDetails.FirstOrDefault(r => r.Rid == model.RegistrationId);
                if (leasemaster != null && leasemaster.Count > 0)
                {
                    string allottee = string.Empty;
                    string propertyno = string.Empty;
                    string dueshtml = string.Empty;
                    string printdate = string.Empty;

                    decimal premiumdues = 0;
                    decimal interest = 0;
                    decimal gstamount = 0;
                    decimal? total_amount = 0;

                    var today = DateTime.Now;
                    var nexttodate = today;

                    var lease = leasemaster.FirstOrDefault();
                    model.RentId = lease.Id;
                    var applicant = dbContext.ApplicationDetails.FirstOrDefault(r => r.registrationId == lease.RegistrationId);
                    var property = (from alot in dbContext.AllotmentMasters
                                    join prop in dbContext.SchemePropTrans on alot.propertyId equals prop.propertyId
                                    where alot.rid == lease.RegistrationId
                                    select prop).FirstOrDefault();

                    allottee = applicant != null ? applicant.tFirstName : string.Empty;

                    if (property != null)
                    {
                        propertyno = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo;
                    }
                    //string dueshtml = "<style>.print-row:nth-child(2n+1){background:#d5d5d5;}</style>";
                    dueshtml = "<div id='Report'><div class='row print-row'><div class='col-md-12 print-col' style='text-align:center'><h3>Property Lease Rent Report</h3></div></div>";
                    dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-4 print-col'><b>Allottee: " + allottee + "</b></div><div class='col-md-4 print-col' style='text-align:center'>Property: " + propertyno + "</div><div class='col-md-4 print-col' style='text-align:right'><b>Registration Id: " + lease.RegistrationId + "</b></div></div>";
                    dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>FY</div><div class='col-md-1 print-col'>PI</div><div class='col-md-2 print-col'>Lease Rent</div><div class='col-md-2 print-col'>Interest</div><div class='col-md-2 print-col'>Paid Amount</div><div class='col-md-2 print-col'>Total</div><div class='col-md-2 print-col'>Dues Upto</div></div>";

                    var leaseDeedDate = lease.TransferLeaseDate == null ? lease.LeaseDeedDate : lease.TransferLeaseDate;
                    var nextdate = lease.PremiumPaidUptoDate == null ? DateTime.Now : lease.PremiumPaidUptoDate;
                    var paidUptoDate = lease.PremiumPaidUptoDate != null ? lease.PremiumPaidUptoDate : DateTime.Now;
                    //var nextDueDate = leaseDeedDate != null ? leaseDeedDate.Value.AddYears(1) : lease.LeaseDeedDate;
                    //var prevDueDate = leaseDeedDate != null ? leaseDeedDate.Value.AddYears(1) : DateTime.Now;
                    var nextDueDate = leaseDeedDate.Value.AddYears(1);
                    var prevDueDate = leaseDeedDate.Value.AddYears(1);
                    var revisedate = lease.RevisedPremiumDate == null ? lease.LeaseDeedDate : lease.RevisedPremiumDate;
                    decimal? reviserate = lease.RevisedRate == null ? 0 : lease.RevisedRate;

                    if (lease.PremiumPaidUptoDate != null)
                    {
                        var premium = lease.RevisedPremium == null ? lease.PremiumLeaseRent : lease.RevisedPremium;
                        decimal? rate = lease.PanelInterest == null ? 0 : lease.PanelInterest;
                        decimal? gstrate = lease.GST == null ? 0 : lease.GST;
                        total_amount = (decimal)premium;
                        var paidDueDate = DateTime.Now;

                        decimal? htmldues = 0;
                        decimal? htmlinterest = 0;
                        decimal? paidamount = 0;
                        string prevyear = string.Empty;
                        string nextyear = string.Empty;

                        var paidYear = lease.PremiumPaidUptoDate.Value.Year;
                        var leaseYear = leaseDeedDate.Value.Year;
                        if (paidYear != leaseYear)
                        {
                            var diffYear = paidYear - leaseYear;
                            nextDueDate = leaseDeedDate.Value.AddYears(diffYear);
                            paidDueDate = leaseDeedDate.Value.AddYears(diffYear);
                        }

                        if (paidDueDate.Date > paidUptoDate.Value.Date)
                        {
                            if (revisedate.Value.Date >= paidDueDate.Date)
                            {
                                var day = paidDueDate - paidUptoDate;
                                decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                                total_amount = (total_amount + rinterest);

                                model.DepositDate = paidUptoDate;
                                model.DepositDueDate = paidDueDate;
                                var paidAmount = CalculatePaidLeaseRentAmountById(model);
                                total_amount = total_amount - paidAmount.PaidAmount;

                                premiumdues = Math.Round((decimal)total_amount, 2);
                                interest = Math.Round((decimal)rinterest, 2);
                            }
                            else
                            {
                                // from paiduptodate to revised date
                                var day1 = revisedate - paidUptoDate;
                                decimal? rinterest1 = (((total_amount * rate) / 100) / 365) * day1.Value.Days;
                                total_amount = (total_amount + rinterest1);

                                model.DepositDate = paidUptoDate;
                                model.DepositDueDate = revisedate;
                                var paidAmount1 = CalculatePaidLeaseRentAmountById(model);
                                total_amount = total_amount - paidAmount1.PaidAmount;

                                var premiumdues1 = Math.Round((decimal)total_amount, 2);
                                var interest1 = Math.Round((decimal)rinterest1, 2);

                                //from revised date to paidduedate
                                var premiumnew = (decimal)premium + (((decimal)premium * reviserate) / 100);
                                var day2 = paidDueDate - revisedate;
                                premium = premiumnew;
                                total_amount = premiumnew;
                                decimal? rinterest2 = (((total_amount * rate) / 100) / 365) * day2.Value.Days;
                                total_amount = (total_amount + rinterest2);

                                model.DepositDate = paidUptoDate;
                                model.DepositDueDate = paidDueDate;
                                var paidAmount2 = CalculatePaidLeaseRentAmountById(model);
                                total_amount = total_amount - paidAmount2.PaidAmount;

                                var premiumdues2 = Math.Round((decimal)total_amount, 2);
                                var interest2 = Math.Round((decimal)rinterest2, 2);

                                premiumdues = Math.Round((decimal)premiumdues1, 2) + Math.Round((decimal)premiumdues2, 2);
                                interest = Math.Round((decimal)interest1, 2) + Math.Round((decimal)interest2, 2);
                            }

                            nextDueDate = nextDueDate.AddYears(1);

                            htmldues = premium;
                            htmlinterest = interest;
                            printdate = nextDueDate.Day + "-" + nextDueDate.Month + "-" + nextDueDate.Year;
                            string fy = prevyear + "-" + nextyear;
                            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 2) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

                            LeaseRentViewModel rent = new LeaseRentViewModel();
                            rent.LeaseRentPremium = premium;
                            rent.RevisedPremium = premium;
                            rent.DuesAmount = premiumdues;
                            rent.BalanceAmount = premiumdues;
                            rent.PremiumInterest = interest;
                            rent.NormalInterest = interest;
                            rent.PanelInterest = interest;
                            rent.GstAmount = gstamount;
                            rent.PaidUptoDate = paidUptoDate;
                            rent.RevisedDate = revisedate;
                            rent.TotalPremium = total_amount;
                            rent.DuesUptoDate = nextDueDate;
                            //rentModel.DuesUptoDate = paidUptoDate;
                            //var duesdate = nextDueDate.Value.AddMonths(12);
                            rent.DepositDueDate = nextDueDate;
                            rent.PremiumPaidDuration = fy;
                            var dueshtml2 = dueshtml + "</div>";
                            rent.HtmlDuesReport = dueshtml2;

                            rentList.Add(rent);
                        }
                        else if (paidDueDate.Date < paidUptoDate.Value.Date)
                        {
                            var nextPaidDueDate = paidDueDate.AddYears(1);
                            if (revisedate.Value.Date >= nextPaidDueDate.Date)
                            {
                                var day = nextPaidDueDate - paidUptoDate;
                                decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                                total_amount = (total_amount + rinterest);

                                model.DepositDate = paidUptoDate;
                                model.DepositDueDate = nextPaidDueDate;
                                var paidAmount = CalculatePaidLeaseRentAmountById(model);
                                total_amount = total_amount - model.PaidAmount;

                                premiumdues = Math.Round((decimal)total_amount, 2);
                                interest = Math.Round((decimal)rinterest, 2);


                            }
                            else
                            {
                                // from paiduptodate to revised date
                                var day1 = revisedate - paidUptoDate;
                                decimal? rinterest1 = (((total_amount * rate) / 100) / 365) * day1.Value.Days;
                                total_amount = (total_amount + rinterest1);

                                model.DepositDate = paidUptoDate;
                                model.DepositDueDate = revisedate;
                                var paidAmount1 = CalculatePaidLeaseRentAmountById(model);
                                total_amount = total_amount - paidAmount1.PaidAmount;

                                var premiumdues1 = Math.Round((decimal)total_amount, 2);
                                var interest1 = Math.Round((decimal)rinterest1, 2);

                                //from revised date to paidduedate
                                var premiumnew = (decimal)premium + (((decimal)premium * reviserate) / 100);
                                var day2 = nextPaidDueDate - revisedate;
                                premium = premiumnew;
                                total_amount = premiumnew;
                                decimal? rinterest2 = (((total_amount * rate) / 100) / 365) * day2.Value.Days;
                                total_amount = (total_amount + rinterest2);

                                model.DepositDate = revisedate;
                                model.DepositDueDate = nextPaidDueDate;
                                var paidAmount2 = CalculatePaidLeaseRentAmountById(model);
                                total_amount = total_amount - paidAmount2.PaidAmount;

                                var premiumdues2 = Math.Round((decimal)total_amount, 2);
                                var interest2 = Math.Round((decimal)rinterest2, 2);

                                premiumdues = Math.Round((decimal)premiumdues1, 2) + Math.Round((decimal)premiumdues2, 2);
                                interest = Math.Round((decimal)interest1, 2) + Math.Round((decimal)interest2, 2);
                            }

                            nextDueDate = nextPaidDueDate;

                            htmldues = premium;
                            htmlinterest = interest;
                            printdate = nextDueDate.Day + "-" + nextDueDate.Month + "-" + nextDueDate.Year;
                            string fy = prevyear + "-" + nextyear;
                            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 2) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

                            LeaseRentViewModel rent = new LeaseRentViewModel();
                            rent.LeaseRentPremium = premium;
                            rent.RevisedPremium = premium;
                            rent.DuesAmount = premiumdues;
                            rent.BalanceAmount = premiumdues;
                            rent.PremiumInterest = interest;
                            rent.PanelInterest = interest;
                            rent.NormalInterest = interest;
                            rent.GstAmount = gstamount;
                            rent.PaidUptoDate = paidUptoDate;
                            rent.RevisedDate = revisedate;
                            rent.TotalPremium = total_amount;
                            rent.DuesUptoDate = nextDueDate;
                            //rentModel.DuesUptoDate = paidUptoDate;
                            //var duesdate = nextDueDate.Value.AddMonths(12);
                            rent.DepositDueDate = nextDueDate;
                            rent.PremiumPaidDuration = fy;
                            var dueshtml2 = dueshtml + "</div>";
                            rent.HtmlDuesReport = dueshtml2;

                            rentList.Add(rent);
                        }
                        //int count = 0;
                        while (today.Date > nextDueDate.Date)
                        {
                            LeaseRentViewModel rentModel = new LeaseRentViewModel();
                            var next_nextDueDate = nextDueDate.AddYears(1);

                            if (today.Date > next_nextDueDate.Date)
                            {
                                if (revisedate.Value.Date >= nextDueDate.Date)
                                {
                                    decimal? rinterest = (((total_amount * rate) / 100));
                                    total_amount = (total_amount + rinterest);

                                    model.DepositDate = nextDueDate;
                                    model.DepositDueDate = next_nextDueDate;
                                    var paidAmount = CalculatePaidLeaseRentAmountById(model);
                                    total_amount = total_amount - model.PaidAmount;

                                    premiumdues = Math.Round((decimal)total_amount, 2);
                                    interest = Math.Round((decimal)rinterest, 2);
                                }
                                else
                                {
                                    // from paiduptodate to revised date
                                    var day1 = nextDueDate - revisedate;
                                    decimal? rinterest1 = (((total_amount * rate) / 100) / 365) * day1.Value.Days;
                                    total_amount = (total_amount + rinterest1);

                                    model.DepositDate = nextDueDate;
                                    model.DepositDueDate = revisedate;
                                    var paidAmount1 = CalculatePaidLeaseRentAmountById(model);
                                    total_amount = total_amount - paidAmount1.PaidAmount;

                                    var premiumdues1 = Math.Round((decimal)total_amount, 2);
                                    var interest1 = Math.Round((decimal)rinterest1, 2);

                                    //from revised date to paidduedate
                                    var premiumnew = (decimal)premium + (((decimal)premium * reviserate) / 100);
                                    var day2 = next_nextDueDate - revisedate;
                                    premium = premiumnew;
                                    total_amount = premiumnew;
                                    decimal? rinterest2 = (((total_amount * rate) / 100) / 365) * day2.Value.Days;
                                    total_amount = (total_amount + rinterest2);

                                    model.DepositDate = revisedate;
                                    model.DepositDueDate = next_nextDueDate;
                                    var paidAmount2 = CalculatePaidLeaseRentAmountById(model);
                                    total_amount = total_amount - paidAmount2.PaidAmount;

                                    var premiumdues2 = Math.Round((decimal)total_amount, 2);
                                    var interest2 = Math.Round((decimal)rinterest2, 2);

                                    premiumdues = Math.Round((decimal)premiumdues1, 2) + Math.Round((decimal)premiumdues2, 2);
                                    interest = Math.Round((decimal)interest1, 2) + Math.Round((decimal)interest2, 2);
                                }

                            }
                            else
                            {
                                //today.Date < nextDueDate.Date
                                if (revisedate.Value.Date >= nextDueDate.Date)
                                {
                                    //var nextPaidDueDate = paidDueDate.AddYears(1);
                                    var day = today - nextDueDate;
                                    decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Days;
                                    total_amount = (total_amount + rinterest);

                                    model.DepositDate = nextDueDate;
                                    model.DepositDueDate = today;
                                    var paidAmount = CalculatePaidLeaseRentAmountById(model);
                                    total_amount = total_amount - model.PaidAmount;

                                    premiumdues = Math.Round((decimal)total_amount, 2);
                                    interest = Math.Round((decimal)rinterest, 2);
                                }
                                else
                                {
                                    if (revisedate.Value.Date >= today.Date)
                                    {
                                        //var nextPaidDueDate = paidDueDate.AddYears(1);
                                        var day = today - nextDueDate;
                                        decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Days;
                                        total_amount = (total_amount + rinterest);

                                        model.DepositDate = nextDueDate;
                                        model.DepositDueDate = today;
                                        var paidAmount = CalculatePaidLeaseRentAmountById(model);
                                        total_amount = total_amount - model.PaidAmount;

                                        premiumdues = Math.Round((decimal)total_amount, 2);
                                        interest = Math.Round((decimal)rinterest, 2);
                                    }
                                    else
                                    {
                                        var day1 = nextDueDate - revisedate;
                                        decimal? rinterest1 = (((total_amount * rate) / 100) / 365) * day1.Value.Days;
                                        total_amount = (total_amount + rinterest1);

                                        model.DepositDate = nextDueDate;
                                        model.DepositDueDate = revisedate;
                                        var paidAmount1 = CalculatePaidLeaseRentAmountById(model);
                                        total_amount = total_amount - paidAmount1.PaidAmount;

                                        var premiumdues1 = Math.Round((decimal)total_amount, 2);
                                        var interest1 = Math.Round((decimal)rinterest1, 2);

                                        //from revised date to paidduedate
                                        var premiumnew = (decimal)premium + (((decimal)premium * reviserate) / 100);
                                        var day2 = next_nextDueDate - revisedate;
                                        premium = premiumnew;
                                        total_amount = premiumnew;
                                        decimal? rinterest2 = (((total_amount * rate) / 100) / 365) * day2.Value.Days;
                                        total_amount = (total_amount + rinterest2);

                                        model.DepositDate = revisedate;
                                        model.DepositDueDate = next_nextDueDate;
                                        var paidAmount2 = CalculatePaidLeaseRentAmountById(model);
                                        total_amount = total_amount - paidAmount2.PaidAmount;

                                        var premiumdues2 = Math.Round((decimal)total_amount, 2);
                                        var interest2 = Math.Round((decimal)rinterest2, 2);

                                        premiumdues = Math.Round((decimal)premiumdues1, 2) + Math.Round((decimal)premiumdues2, 2);
                                        interest = Math.Round((decimal)interest1, 2) + Math.Round((decimal)interest2, 2);
                                    }
                                }

                            }

                            nextDueDate = next_nextDueDate;

                            htmldues = premium;
                            htmlinterest = interest;
                            printdate = nextDueDate.Day + "-" + nextDueDate.Month + "-" + nextDueDate.Year;
                            string fy = prevyear + "-" + nextyear;
                            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 2) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

                            rentModel.LeaseRentPremium = premium;
                            rentModel.RevisedPremium = premium;
                            rentModel.DuesAmount = premiumdues;
                            rentModel.BalanceAmount = premiumdues;
                            rentModel.PremiumInterest = interest;
                            rentModel.NormalInterest = interest;
                            rentModel.PanelInterest = interest;
                            rentModel.GstAmount = gstamount;
                            rentModel.PaidUptoDate = paidUptoDate;
                            rentModel.RevisedDate = revisedate;
                            rentModel.TotalPremium = total_amount;
                            rentModel.DuesUptoDate = nextDueDate;
                            //rentModel.DuesUptoDate = paidUptoDate;
                            //var duesdate = nextDueDate.Value.AddMonths(12);
                            rentModel.DepositDueDate = nextDueDate;
                            rentModel.PremiumPaidDuration = fy;
                            var dueshtml2 = dueshtml + "</div>";
                            rentModel.HtmlDuesReport = dueshtml2;

                            rentList.Add(rentModel);
                        }
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

                return rentList;
            }
        }

        private List<LeaseRentViewModel> CalculateLeaseRentDuesPaymentByIdII(LeaseRentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<LeaseRentViewModel> rentList = new List<LeaseRentViewModel>();
                //check in Lease Rent Payment Details
                var leasemaster = dbContext.LeaseRentPayments.Where(r => r.RegistrationId == model.RegistrationId).OrderByDescending(d => d.Id).ToList();
                var registry = dbContext.RegistryDetails.FirstOrDefault(r => r.Rid == model.RegistrationId);
                if (leasemaster != null && leasemaster.Count > 0)
                {
                    string allottee = string.Empty;
                    string propertyno = string.Empty;
                    string dueshtml = string.Empty;
                    string printdate = string.Empty;

                    decimal premiumdues = 0;
                    decimal interest = 0;
                    decimal gstamount = 0;
                    decimal? total_amount = 0;

                    var today = DateTime.Now;
                    var nexttodate = today;

                    var lease = leasemaster.FirstOrDefault();
                    model.RentId = lease.Id;
                    var applicant = dbContext.ApplicationDetails.FirstOrDefault(r => r.registrationId == lease.RegistrationId);
                    var property = (from alot in dbContext.AllotmentMasters
                                    join prop in dbContext.SchemePropTrans on alot.propertyId equals prop.propertyId
                                    where alot.rid == lease.RegistrationId
                                    select prop).FirstOrDefault();

                    allottee = applicant != null ? applicant.tFirstName : string.Empty;

                    if (property != null)
                    {
                        propertyno = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo;
                    }
                    //string dueshtml = "<style>.print-row:nth-child(2n+1){background:#d5d5d5;}</style>";
                    dueshtml = "<div id='Report'><div class='row print-row'><div class='col-md-12 print-col' style='text-align:center'><h3>Property Lease Rent Report</h3></div></div>";
                    dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-4 print-col'><b>Allottee: " + allottee + "</b></div><div class='col-md-4 print-col' style='text-align:center'>Property: " + propertyno + "</div><div class='col-md-4 print-col' style='text-align:right'><b>Registration Id: " + lease.RegistrationId + "</b></div></div>";
                    dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>FY</div><div class='col-md-1 print-col'>PI</div><div class='col-md-2 print-col'>Lease Rent</div><div class='col-md-2 print-col'>Interest</div><div class='col-md-2 print-col'>Paid Amount</div><div class='col-md-2 print-col'>Total</div><div class='col-md-2 print-col'>Dues Upto</div></div>";

                    var leaseDeedDate = lease.TransferLeaseDate == null ? lease.LeaseDeedDate : lease.TransferLeaseDate;
                    var nextdate = lease.PremiumPaidUptoDate == null ? DateTime.Now : lease.PremiumPaidUptoDate;
                    var paidUptoDate = lease.PremiumPaidUptoDate != null ? lease.PremiumPaidUptoDate : DateTime.Now;
                    //var nextDueDate = leaseDeedDate != null ? leaseDeedDate.Value.AddYears(1) : lease.LeaseDeedDate;
                    //var prevDueDate = leaseDeedDate != null ? leaseDeedDate.Value.AddYears(1) : DateTime.Now;
                    var nextDueDate = leaseDeedDate.Value.AddYears(1);
                    var prevDueDate = leaseDeedDate.Value.AddYears(1);
                    var revisedate = lease.RevisedPremiumDate == null ? lease.LeaseDeedDate : lease.RevisedPremiumDate;
                    decimal? reviserate = lease.RevisedRate == null ? 0 : lease.RevisedRate;

                    if (lease.PremiumPaidUptoDate != null)
                    {
                        var premium = lease.RevisedPremium == null ? lease.PremiumLeaseRent : lease.RevisedPremium;
                        decimal? rate = lease.PanelInterest == null ? 0 : lease.PanelInterest;
                        decimal? gstrate = lease.GST == null ? 0 : lease.GST;
                        total_amount = (decimal)premium;
                        var paidDueDate = DateTime.Now;

                        decimal? htmldues = 0;
                        decimal? htmlinterest = 0;
                        decimal? paidamount = 0;
                        string prevyear = string.Empty;
                        string nextyear = string.Empty;

                        var paidYear = lease.PremiumPaidUptoDate.Value.Year;
                        var leaseYear = leaseDeedDate.Value.Year;
                        if (paidYear != leaseYear)
                        {
                            var diffYear = paidYear - leaseYear;
                            nextDueDate = leaseDeedDate.Value.AddYears(diffYear);
                            paidDueDate = leaseDeedDate.Value.AddYears(diffYear);
                        }

                        if (paidDueDate.Date > paidUptoDate.Value.Date)
                        {
                            if (revisedate.Value.Date >= paidDueDate.Date)
                            {
                                var day = paidDueDate - paidUptoDate;
                                decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                                total_amount = (total_amount + rinterest);

                                model.DepositDate = paidUptoDate;
                                model.DepositDueDate = paidDueDate;
                                var paidAmount = CalculatePaidLeaseRentAmountById(model);
                                total_amount = total_amount - paidAmount.PaidAmount;

                                premiumdues = Math.Round((decimal)total_amount, 2);
                                interest = Math.Round((decimal)rinterest, 2);
                            }
                            else
                            {
                                // from paiduptodate to revised date
                                var day1 = revisedate - paidUptoDate;
                                decimal? rinterest1 = (((total_amount * rate) / 100) / 365) * day1.Value.Days;
                                total_amount = (total_amount + rinterest1);

                                model.DepositDate = paidUptoDate;
                                model.DepositDueDate = revisedate;
                                var paidAmount1 = CalculatePaidLeaseRentAmountById(model);
                                total_amount = total_amount - paidAmount1.PaidAmount;

                                var premiumdues1 = Math.Round((decimal)total_amount, 2);
                                var interest1 = Math.Round((decimal)rinterest1, 2);

                                //from revised date to paidduedate
                                var premiumnew = (decimal)premium + (((decimal)premium * reviserate) / 100);
                                var day2 = paidDueDate - revisedate;
                                premium = premiumnew;
                                total_amount = premiumnew;
                                decimal? rinterest2 = (((total_amount * rate) / 100) / 365) * day2.Value.Days;
                                total_amount = (total_amount + rinterest2);

                                model.DepositDate = paidUptoDate;
                                model.DepositDueDate = paidDueDate;
                                var paidAmount2 = CalculatePaidLeaseRentAmountById(model);
                                total_amount = total_amount - paidAmount2.PaidAmount;

                                var premiumdues2 = Math.Round((decimal)total_amount, 2);
                                var interest2 = Math.Round((decimal)rinterest2, 2);

                                premiumdues = Math.Round((decimal)premiumdues1, 2) + Math.Round((decimal)premiumdues2, 2);
                                interest = Math.Round((decimal)interest1, 2) + Math.Round((decimal)interest2, 2);
                            }

                            nextDueDate = nextDueDate.AddYears(1);

                            htmldues = premium;
                            htmlinterest = interest;
                            printdate = nextDueDate.Day + "-" + nextDueDate.Month + "-" + nextDueDate.Year;
                            string fy = prevyear + "-" + nextyear;
                            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 2) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

                            LeaseRentViewModel rent = new LeaseRentViewModel();
                            rent.LeaseRentPremium = premium;
                            rent.RevisedPremium = premium;
                            rent.DuesAmount = premiumdues;
                            rent.BalanceAmount = premiumdues;
                            rent.PremiumInterest = interest;
                            rent.NormalInterest = interest;
                            rent.PanelInterest = interest;
                            rent.GstAmount = gstamount;
                            rent.PaidUptoDate = paidUptoDate;
                            rent.RevisedDate = revisedate;
                            rent.TotalPremium = total_amount;
                            rent.DuesUptoDate = nextDueDate;
                            //rentModel.DuesUptoDate = paidUptoDate;
                            //var duesdate = nextDueDate.Value.AddMonths(12);
                            rent.DepositDueDate = nextDueDate;
                            rent.PremiumPaidDuration = fy;
                            var dueshtml2 = dueshtml + "</div>";
                            rent.HtmlDuesReport = dueshtml2;

                            rentList.Add(rent);
                        }
                        else if (paidDueDate.Date < paidUptoDate.Value.Date)
                        {
                            var nextPaidDueDate = paidDueDate.AddYears(1);
                            if (revisedate.Value.Date >= nextPaidDueDate.Date)
                            {
                                var day = nextPaidDueDate - paidUptoDate;
                                decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Value.Days;
                                total_amount = (total_amount + rinterest);

                                model.DepositDate = paidUptoDate;
                                model.DepositDueDate = nextPaidDueDate;
                                var paidAmount = CalculatePaidLeaseRentAmountById(model);
                                total_amount = total_amount - model.PaidAmount;

                                premiumdues = Math.Round((decimal)total_amount, 2);
                                interest = Math.Round((decimal)rinterest, 2);


                            }
                            else
                            {
                                // from paiduptodate to revised date
                                var day1 = revisedate - paidUptoDate;
                                decimal? rinterest1 = (((total_amount * rate) / 100) / 365) * day1.Value.Days;
                                total_amount = (total_amount + rinterest1);

                                model.DepositDate = paidUptoDate;
                                model.DepositDueDate = revisedate;
                                var paidAmount1 = CalculatePaidLeaseRentAmountById(model);
                                total_amount = total_amount - paidAmount1.PaidAmount;

                                var premiumdues1 = Math.Round((decimal)total_amount, 2);
                                var interest1 = Math.Round((decimal)rinterest1, 2);

                                //from revised date to paidduedate
                                var premiumnew = (decimal)premium + (((decimal)premium * reviserate) / 100);
                                var day2 = nextPaidDueDate - revisedate;
                                premium = premiumnew;
                                total_amount = premiumnew;
                                decimal? rinterest2 = (((total_amount * rate) / 100) / 365) * day2.Value.Days;
                                total_amount = (total_amount + rinterest2);

                                model.DepositDate = revisedate;
                                model.DepositDueDate = nextPaidDueDate;
                                var paidAmount2 = CalculatePaidLeaseRentAmountById(model);
                                total_amount = total_amount - paidAmount2.PaidAmount;

                                var premiumdues2 = Math.Round((decimal)total_amount, 2);
                                var interest2 = Math.Round((decimal)rinterest2, 2);

                                premiumdues = Math.Round((decimal)premiumdues1, 2) + Math.Round((decimal)premiumdues2, 2);
                                interest = Math.Round((decimal)interest1, 2) + Math.Round((decimal)interest2, 2);
                            }

                            nextDueDate = nextPaidDueDate;

                            htmldues = premium;
                            htmlinterest = interest;
                            printdate = nextDueDate.Day + "-" + nextDueDate.Month + "-" + nextDueDate.Year;
                            string fy = prevyear + "-" + nextyear;
                            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 2) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

                            LeaseRentViewModel rent = new LeaseRentViewModel();
                            rent.LeaseRentPremium = premium;
                            rent.RevisedPremium = premium;
                            rent.DuesAmount = premiumdues;
                            rent.BalanceAmount = premiumdues;
                            rent.PremiumInterest = interest;
                            rent.PanelInterest = interest;
                            rent.NormalInterest = interest;
                            rent.GstAmount = gstamount;
                            rent.PaidUptoDate = paidUptoDate;
                            rent.RevisedDate = revisedate;
                            rent.TotalPremium = total_amount;
                            rent.DuesUptoDate = nextDueDate;
                            //rentModel.DuesUptoDate = paidUptoDate;
                            //var duesdate = nextDueDate.Value.AddMonths(12);
                            rent.DepositDueDate = nextDueDate;
                            rent.PremiumPaidDuration = fy;
                            var dueshtml2 = dueshtml + "</div>";
                            rent.HtmlDuesReport = dueshtml2;

                            rentList.Add(rent);
                        }
                        //int count = 0;
                        while (today.Date > nextDueDate.Date)
                        {
                            LeaseRentViewModel rentModel = new LeaseRentViewModel();
                            var next_nextDueDate = nextDueDate.AddYears(1);

                            if (today.Date > next_nextDueDate.Date)
                            {
                                if (revisedate.Value.Date >= nextDueDate.Date)
                                {
                                    decimal? rinterest = (((total_amount * rate) / 100));
                                    total_amount = (total_amount + rinterest);

                                    model.DepositDate = nextDueDate;
                                    model.DepositDueDate = next_nextDueDate;
                                    var paidAmount = CalculatePaidLeaseRentAmountById(model);
                                    total_amount = total_amount - model.PaidAmount;

                                    premiumdues = Math.Round((decimal)total_amount, 2);
                                    interest = Math.Round((decimal)rinterest, 2);
                                }
                                else
                                {
                                    // from paiduptodate to revised date
                                    var day1 = nextDueDate - revisedate;
                                    decimal? rinterest1 = (((total_amount * rate) / 100) / 365) * day1.Value.Days;
                                    total_amount = (total_amount + rinterest1);

                                    model.DepositDate = nextDueDate;
                                    model.DepositDueDate = revisedate;
                                    var paidAmount1 = CalculatePaidLeaseRentAmountById(model);
                                    total_amount = total_amount - paidAmount1.PaidAmount;

                                    var premiumdues1 = Math.Round((decimal)total_amount, 2);
                                    var interest1 = Math.Round((decimal)rinterest1, 2);

                                    //from revised date to paidduedate
                                    var premiumnew = (decimal)premium + (((decimal)premium * reviserate) / 100);
                                    var day2 = next_nextDueDate - revisedate;
                                    premium = premiumnew;
                                    total_amount = premiumnew;
                                    decimal? rinterest2 = (((total_amount * rate) / 100) / 365) * day2.Value.Days;
                                    total_amount = (total_amount + rinterest2);

                                    model.DepositDate = revisedate;
                                    model.DepositDueDate = next_nextDueDate;
                                    var paidAmount2 = CalculatePaidLeaseRentAmountById(model);
                                    total_amount = total_amount - paidAmount2.PaidAmount;

                                    var premiumdues2 = Math.Round((decimal)total_amount, 2);
                                    var interest2 = Math.Round((decimal)rinterest2, 2);

                                    premiumdues = Math.Round((decimal)premiumdues1, 2) + Math.Round((decimal)premiumdues2, 2);
                                    interest = Math.Round((decimal)interest1, 2) + Math.Round((decimal)interest2, 2);
                                }

                            }
                            else
                            {
                                //today.Date < nextDueDate.Date
                                if (revisedate.Value.Date >= nextDueDate.Date)
                                {
                                    //var nextPaidDueDate = paidDueDate.AddYears(1);
                                    var day = today - nextDueDate;
                                    decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Days;
                                    total_amount = (total_amount + rinterest);

                                    model.DepositDate = nextDueDate;
                                    model.DepositDueDate = today;
                                    var paidAmount = CalculatePaidLeaseRentAmountById(model);
                                    total_amount = total_amount - model.PaidAmount;

                                    premiumdues = Math.Round((decimal)total_amount, 2);
                                    interest = Math.Round((decimal)rinterest, 2);
                                }
                                else
                                {
                                    if (revisedate.Value.Date >= today.Date)
                                    {
                                        //var nextPaidDueDate = paidDueDate.AddYears(1);
                                        var day = today - nextDueDate;
                                        decimal? rinterest = (((total_amount * rate) / 100) / 365) * day.Days;
                                        total_amount = (total_amount + rinterest);

                                        model.DepositDate = nextDueDate;
                                        model.DepositDueDate = today;
                                        var paidAmount = CalculatePaidLeaseRentAmountById(model);
                                        total_amount = total_amount - model.PaidAmount;

                                        premiumdues = Math.Round((decimal)total_amount, 2);
                                        interest = Math.Round((decimal)rinterest, 2);
                                    }
                                    else
                                    {
                                        var day1 = nextDueDate - revisedate;
                                        decimal? rinterest1 = (((total_amount * rate) / 100) / 365) * day1.Value.Days;
                                        total_amount = (total_amount + rinterest1);

                                        model.DepositDate = nextDueDate;
                                        model.DepositDueDate = revisedate;
                                        var paidAmount1 = CalculatePaidLeaseRentAmountById(model);
                                        total_amount = total_amount - paidAmount1.PaidAmount;

                                        var premiumdues1 = Math.Round((decimal)total_amount, 2);
                                        var interest1 = Math.Round((decimal)rinterest1, 2);

                                        //from revised date to paidduedate
                                        var premiumnew = (decimal)premium + (((decimal)premium * reviserate) / 100);
                                        var day2 = next_nextDueDate - revisedate;
                                        premium = premiumnew;
                                        total_amount = premiumnew;
                                        decimal? rinterest2 = (((total_amount * rate) / 100) / 365) * day2.Value.Days;
                                        total_amount = (total_amount + rinterest2);

                                        model.DepositDate = revisedate;
                                        model.DepositDueDate = next_nextDueDate;
                                        var paidAmount2 = CalculatePaidLeaseRentAmountById(model);
                                        total_amount = total_amount - paidAmount2.PaidAmount;

                                        var premiumdues2 = Math.Round((decimal)total_amount, 2);
                                        var interest2 = Math.Round((decimal)rinterest2, 2);

                                        premiumdues = Math.Round((decimal)premiumdues1, 2) + Math.Round((decimal)premiumdues2, 2);
                                        interest = Math.Round((decimal)interest1, 2) + Math.Round((decimal)interest2, 2);
                                    }
                                }

                            }

                            nextDueDate = next_nextDueDate;

                            htmldues = premium;
                            htmlinterest = interest;
                            printdate = nextDueDate.Day + "-" + nextDueDate.Month + "-" + nextDueDate.Year;
                            string fy = prevyear + "-" + nextyear;
                            dueshtml = dueshtml + "<div class='row print-row'><div class='col-md-1 print-col'>" + fy + "</div><div class='col-md-1 print-col'>" + rate + "</div><div class='col-md-2 print-col'>" + htmldues + "</div><div class='col-md-2 print-col'>" + htmlinterest + "</div><div class='col-md-2 print-col'>" + paidamount + "</div><div class='col-md-2 print-col'>" + Math.Round((decimal)total_amount, 2) + "</div><div class='col-md-2 print-col'>" + printdate + "</div></div>";

                            rentModel.LeaseRentPremium = premium;
                            rentModel.RevisedPremium = premium;
                            rentModel.DuesAmount = premiumdues;
                            rentModel.BalanceAmount = premiumdues;
                            rentModel.PremiumInterest = interest;
                            rentModel.NormalInterest = interest;
                            rentModel.PanelInterest = interest;
                            rentModel.GstAmount = gstamount;
                            rentModel.PaidUptoDate = paidUptoDate;
                            rentModel.RevisedDate = revisedate;
                            rentModel.TotalPremium = total_amount;
                            rentModel.DuesUptoDate = nextDueDate;
                            //rentModel.DuesUptoDate = paidUptoDate;
                            //var duesdate = nextDueDate.Value.AddMonths(12);
                            rentModel.DepositDueDate = nextDueDate;
                            rentModel.PremiumPaidDuration = fy;
                            var dueshtml2 = dueshtml + "</div>";
                            rentModel.HtmlDuesReport = dueshtml2;

                            rentList.Add(rentModel);
                        }
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

                return rentList;
            }
        }

        private LeaseRentViewModel CalculatePaidLeaseRentAmountById(LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var paymentList = dbcontext.ViewReceiptMasters.Where(r => r.RID_NO == model.RegistrationId.ToString() && r.DEPOSIT_DATE > model.DepositDate && r.DEPOSIT_DATE < model.DepositDueDate && (r.RECEIPT_HEAD_ID == 8 || r.RECEIPT_HEAD_ID == 30)).ToList();
                model.PaidAmount = (paymentList != null && paymentList.Count > 0) ? paymentList.Sum(p => p.AMOUNT_PAID) : 0;
                //if (paymentList != null && paymentList.Count > 0)
                //{
                //    var amount = paymentList.Sum(p => p.AMOUNT_PAID);
                //    model.PaidAmount = amount;
                //}
            }
            return model;
        }


        public DataSourceResult GetLeaseRentDuesPaymentById(DataSourceRequest request, LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var list = (from leaserent in dbcontext.LeaseRentPaymentTrans
                            where leaserent.RefId == model.RefId
                            select new LeaseRentViewModel
                            {
                                Id = leaserent.Id,
                                RefId = leaserent.RefId,
                                RegistrationId = leaserent.RegistrationId,
                                LeaseRentPremium = leaserent.LeaseRentPremium,
                                NormalInterest = leaserent.NormalInterest,
                                PanelInterest = leaserent.PenalInterest,
                                GST = leaserent.GST,
                                DuesAmount = leaserent.DuesAmount,
                                DuesUptoDate = leaserent.DuesUptoDate,
                                DepositDueDate = leaserent.DepositDueDate,
                                PaidAmount = leaserent.PaidAmount,
                                DepositDate = leaserent.DepositDate,
                                BalanceAmount = leaserent.BalanceAmount,
                                HtmlDuesReport = leaserent.HtmlTemplate,
                                Comment = leaserent.Comment,
                                PaymentMode = leaserent.PaymentMode,
                                TransactionId = leaserent.TransactionId,
                                ApprovalDate = leaserent.ApprovalDate,
                                ApproverId = leaserent.ApproverId,
                                StatusId = leaserent.StatusId,
                                Status = (leaserent.StatusId != null || leaserent.StatusId > 0) ? (from status in dbcontext.StatusMasters where status.Id == leaserent.StatusId select status.Status).FirstOrDefault() : string.Empty,
                                IsActive = leaserent.IsActive,
                                CreatedDate = leaserent.CreatedDate,
                                CreatedBy = leaserent.CreatedBy.ToString(),
                                Sector = null
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public int UpdateDuesPaymentDetails(PaymentViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == Constants.Installment)
                {
                    var propertyList = dbContext.InstallmentDuesPayments.Where(i => i.DepartmentId == model.DepartmentId && i.IsTotalPremiumPaid == false).ToList();
                    if (propertyList != null && propertyList.Count > 0)
                    {
                        foreach (var property in propertyList)
                        {
                            flag = UpdateInstallmentDuesPaymentByIdII(new PaymentScheduleModel() { RegistrationId = property.RegistrationId });
                        }
                    }
                    else
                    {
                        flag = ReturnType.NotAvailable;
                    }
                }
                if (model.ActionType == Constants.Leaserent)
                {
                    var leaserent = dbContext.LeaseRentPayments.Where(r => r.DepartmentId == model.DepartmentId && r.IsOneTimeLeasePaid == false).ToList();
                    if (leaserent != null && leaserent.Count > 0)
                    {
                        foreach (var rent in leaserent)
                        {
                            flag = UpdateLeaseRentDuesPaymentByIdII(new LeaseRentViewModel() { RegistrationId = rent.RegistrationId });
                        }
                    }
                    else
                    {
                        flag = ReturnType.NotAvailable;
                    }
                }
                return flag;
            }
        }


        public DataSourceResult GetRegistrationIdListFromLeaseRent(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var ridlist = (from lease in dbContext.LeaseRentPayments
                               select new DropdownViewModel
                               {
                                   Id = lease.RegistrationId.Value,
                                   Text = lease.RegistrationId.ToString()
                               });
                return ridlist.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetRegistrationIdListFromInstallmentPayment(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var ridlist = (from installment in dbContext.InstallmentDuesPayments
                               select new DropdownViewModel
                               {
                                   Id = installment.RegistrationId.Value,
                                   Text = installment.RegistrationId.ToString()
                               });
                return ridlist.ToDataSourceResult(request);
            }
        }


        public LeaseRentViewModel GetLeaseRentDetailsByRegistrationId(LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var leaseDetails = dbcontext.LeaseRentPayments.FirstOrDefault(m => m.RegistrationId == model.RegistrationId);

                model.Id = leaseDetails.Id;
                model.RegistrationId = leaseDetails.RegistrationId;
                model.PropertyId = leaseDetails.PropertyId;
                model.Applicant = leaseDetails.Applicant;
                model.TotalPremiumPaidStatus = leaseDetails.IsTotalInstallmentPaid == true ? "Y" : "N";
                model.OneTimeLeasePaidStatus = leaseDetails.IsOneTimeLeasePaid == true ? "Y" : "N";
                model.LeaseRentPremium = leaseDetails.PremiumLeaseRent;
                model.LeaseDeedDate = leaseDetails.LeaseDeedDate;
                model.TransferLeaseDate = leaseDetails.TransferLeaseDate;
                model.RevisedDate = leaseDetails.RevisedPremiumDate;
                model.PremiumPaidDuration = leaseDetails.PremiumPaidDuration;
                model.PaidUptoDate = leaseDetails.PremiumPaidUptoDate;
                model.DepartmentId = leaseDetails.DepartmentId;
                //model.Department = leaseDetails.DepartmentId==null? string.Empty:dbcontext.DepartmentMsts.FirstOrDefault(m => m.departmentId == leaseDetails.DepartmentId).departmentName;
                model.BalanceAmount = leaseDetails.BalanceAmount;
                model.BalanceInterest = leaseDetails.BalanceInterest;
                model.BalanceUptoDate = leaseDetails.BalanceUptoDate;
                model.GstAmount = leaseDetails.GST;
                model.TotalPremium = leaseDetails.TotalPremium;
                model.DepositDate = leaseDetails.DepositDate;
                model.RevisedRate = leaseDetails.RevisedRate;
                model.NDCDate = leaseDetails.NDCDate;
                model.NDCStatus = leaseDetails.NDCStatus;
                model.LeaseRentDues = leaseDetails.LeaseRentDues;
                model.DuesUptoDate = leaseDetails.DuesUptoDate;
                model.PreviousDues = leaseDetails.PreviousDues;
                model.PreviousDuesDate = leaseDetails.PreviousDuesDate;
                model.CurrentDues = leaseDetails.CurrentDues;
                model.CurrentDuesDate = leaseDetails.CurrentDuesDate;
                model.PanelInterest = leaseDetails.PanelInterest;
                model.RevisedPremium = leaseDetails.RevisedPremium;
                model.ChallanDate = leaseDetails.ChallanDate;
                model.ChallanId = leaseDetails.ChallanId;
                model.PreviousBalance = leaseDetails.PreviousBalance;
                model.RevisedRateAfterYear = leaseDetails.ReviseRateAfterYear;
                return model;
            }
        }

        public int UpdateLeaseRentDetails(LeaseRentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbcontext = new NoidaPMSEntities())
            {
                var data = dbcontext.LeaseRentPayments.FirstOrDefault(m => m.Id == model.Id);
                if (data != null)
                {
                    data.PropertyId = model.PropertyId;
                    data.DepartmentId = model.DepartmentId;// dbcontext.DepartmentMsts.FirstOrDefault(m => m.departmentName == model.Department).departmentId;
                    data.Applicant = model.Applicant;
                    data.IsTotalInstallmentPaid = model.TotalPremiumPaidStatus == "Y" ? true : false;
                    data.IsTotalPremiumPaid = model.TotalPremiumPaidStatus == "Y" ? true : false;
                    data.IsOneTimeLeasePaid = model.OneTimeLeasePaidStatus == "Y" ? true : false;
                    data.PremiumLeaseRent = model.LeaseRentPremium;
                    data.LeaseDeedDate = model.LeaseDeedDate;
                    data.TransferLeaseDate = model.TransferLeaseDate;
                    data.RevisedPremiumDate = model.RevisedDate;
                    data.PremiumPaidDuration = model.PremiumPaidDuration;
                    data.PremiumPaidUptoDate = model.PaidUptoDate;
                    data.BalanceAmount = model.BalanceAmount;
                    data.BalanceInterest = model.BalanceInterest;
                    data.TotalPremium = model.TotalPremium;
                    data.DepositDate = model.DepositDate;
                    data.LeaseRentDues = model.LeaseRentDues;
                    data.DuesUptoDate = model.DuesUptoDate;
                    data.GST = model.GstAmount;
                    data.BalanceUptoDate = model.BalanceUptoDate;
                    data.NDCDate = model.NDCDate;
                    data.ChallanDate = model.ChallanDate;
                    data.RevisedRate = model.RevisedRate;
                    data.PanelInterest = model.PanelInterest;
                    data.ReviseRateAfterYear = model.RevisedRateAfterYear;
                    data.PreviousBalance = model.PreviousBalance;
                    data.ChallanId = model.ChallanId;

                    dbcontext.SaveChanges();
                    //if (model.OneTimeLeasePaidStatus == Constants.Y && model.ChallanDate != null)
                    //{
                    var ExNDC = dbcontext.PRE_FULL_PAYMENT_NDC.FirstOrDefault(n => n.RegistrationId == model.RegistrationId && n.IsActive == true);
                    if (ExNDC == null)
                    {
                        var ndc = new PRE_FULL_PAYMENT_NDC();
                        ndc.RegistrationId = model.RegistrationId;
                        ndc.Applicant = model.Applicant;
                        ndc.PropertyNo = model.PropertyNo;
                        ndc.TotalPaidPream = model.TotalPremiumPaidStatus;
                        ndc.OneTimeLease = model.OneTimeLeasePaidStatus;
                        ndc.Status = NAStatusId.Initiated.ToString();
                        ndc.IsActive = true;
                        ndc.NDCDate = model.NDCDate;
                        ndc.EntryDate = DateTime.Now;
                        ndc.CreatedBy = userInfo.UserID.ToString();
                        ndc.CreatedOn = DateTime.Now;
                        dbcontext.PRE_FULL_PAYMENT_NDC.Add(ndc);
                        dbcontext.SaveChanges();
                    }
                    else
                    {
                        ExNDC.TotalPaidPream = model.TotalPremiumPaidStatus;
                        ExNDC.OneTimeLease = model.OneTimeLeasePaidStatus;
                        ExNDC.Status = NAStatusId.Initiated.ToString();
                        ExNDC.IsActive = true;
                        ExNDC.NDCDate = model.NDCDate;
                        dbcontext.SaveChanges();
                    }
                    //}

                    flag = ReturnType.Updated;
                }


            }
            return flag;
        }


        public LeaseRentViewModel GetInstallmentDetailsByRegistrationId(LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var installmentDetails = dbcontext.InstallmentDuesPayments.FirstOrDefault(m => m.RegistrationId == model.RegistrationId);

                model.Id = installmentDetails.Id;
                model.RegistrationId = installmentDetails.RegistrationId;
                model.PropertyId = installmentDetails.PropertyId;
                model.Applicant = installmentDetails.Applicant;
                model.TotalPremiumPaidStatus = installmentDetails.IsTotalPremiumPaid == true ? "Y" : "N";
                model.OneTimeLeasePaidStatus = installmentDetails.IsOneTimeLeasePaid == true ? "Y" : "N";
                model.DepartmentId = installmentDetails.DepartmentId;
                model.BalanceAmount = installmentDetails.BalanceAmount;
                model.BalanceInterest = installmentDetails.BalanceInterest;
                model.DuesUptoDate = installmentDetails.DuesUptoDate;
                model.BalanceUptoDate = installmentDetails.BalanceUptoDate;
                model.InstallmentStartDate = installmentDetails.InstallmentStartDate;
                model.InstallmentEndDate = installmentDetails.InstallmentEndDate;
            }
            return model;
        }

        public int UpdateInstallmentRentDetails(LeaseRentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbcontext = new NoidaPMSEntities())
            {
                var data = dbcontext.InstallmentDuesPayments.FirstOrDefault(m => m.Id == model.Id);
                if (data != null)
                {
                    data.PropertyId = model.PropertyId;
                    data.DepartmentId = model.DepartmentId;// dbcontext.DepartmentMsts.FirstOrDefault(m => m.departmentName == model.Department).departmentId;
                    data.Applicant = model.Applicant;
                    data.IsTotalPremiumPaid = model.TotalPremiumPaidStatus == "Y" ? true : false;
                    data.IsOneTimeLeasePaid = model.OneTimeLeasePaidStatus == "Y" ? true : false;
                    data.BalanceAmount = model.BalanceAmount;
                    data.BalanceInterest = model.BalanceInterest;
                    data.DuesUptoDate = model.DuesUptoDate;
                    data.BalanceUptoDate = model.BalanceUptoDate;
                    data.InstallmentEndDate = model.InstallmentEndDate;
                    data.InstallmentStartDate = model.InstallmentStartDate;

                    dbcontext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }


        public DataSourceResult GetLeaseRentDetailsByIdAndProcedure(DataSourceRequest request, LeaseRentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var today = DateTime.Now;
                var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                model.CurrentDuesDate = model.CurrentDuesDate != null ? model.CurrentDuesDate : DateTime.Now.AddDays(days - DateTime.Now.Day);
                //today = model.CurrentDuesDate != null ? model.CurrentDuesDate : DateTime.Now.AddDays(days - DateTime.Now.Day);

                List<Sp_LeaseRentDuesCalculationTillDate_Result> rentList = dbContext.Sp_LeaseRentDuesCalculationTillDate(model.RegistrationId.ToString(), model.CurrentDuesDate, model.DepartmentId).ToList();
                if (rentList != null && rentList.Count > 0)
                {
                    List<LeaseRentViewModel> leaserentData = new List<LeaseRentViewModel>();
                    foreach (var rent in rentList)
                    {
                        LeaseRentViewModel lease = new LeaseRentViewModel();
                        lease.LeaseRentPremium = rent.leaserent;
                        lease.DuesAmount = rent.Total_dues;
                        lease.CurrentDuesDate = rent.duedate;
                        lease.DepositDate = rent.DepositDate;
                        lease.DepositAmount = rent.depositamount;
                        lease.PanelInterest = rent.penlatintrest;
                        lease.TotalAmount = rent.totalamount;
                        lease.TotalPenalInterest = rent.totalpanelamount;
                        lease.TotalDuesAmount = rent.Total_dues;
                        leaserentData.Add(lease);
                    }
                    return leaserentData.ToDataSourceResult(request);
                }
                else
                {
                    return null;
                }
            }
        }



        public PaymentViewModel GetLeaseRentDuesForDemandNote(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from alotment in dbContext.AllotmentMasters
                            join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where alotment.rid == model.RegistrationId
                            select new PaymentViewModel
                            {
                                RegistrationId = alotment.rid,
                                DepartmentId = alotment.departmentId,
                                Department = alotment.DepartmentMst.departmentName,
                                PropertyId = alotment.propertyId.ToString(),

                                Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : (aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName),
                                ApplicantAddress = aplicant.tCorrespondanceAdd,
                                MobileNo = aplicant.tMobileNumber,
                                Email = aplicant.tEmail,

                                SectorId = property.sectorId,
                                SectorName = property.SectorMst.sectorName,
                                BlockId = property.blockId,
                                BlockName = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                            }).FirstOrDefault();

                if (data != null)
                {
                    var lease = dbContext.LeaseRentPayments.FirstOrDefault(c => c.RegistrationId == model.RegistrationId);
                    if (lease != null)
                    {
                        if (lease.IsOneTimeLeasePaid.Value == true)
                        {
                            data.IsOneTimeLeasePaid = "True";
                            data.ActionTypeId = ReturnType.Paid;
                        }
                        else
                        {
                            var today = DateTime.Now;
                            var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                            today = DateTime.Now.AddDays(days - DateTime.Now.Day);
                            today = model.CurrentDuesDate != null ? (DateTime)model.CurrentDuesDate : (lease.DuesUptoDate != null ? lease.DuesUptoDate.Value : today);
                            List<Sp_LeaseRentDuesCalculationTillDate_Result> rentList = dbContext.Sp_LeaseRentDuesCalculationTillDate(data.RegistrationId.ToString(), today, data.DepartmentId).ToList();
                            if (rentList != null && rentList.Count > 0)
                            {
                                var count = rentList.Count - 1;
                                decimal? onetime = 0;
                                if (model.ActionType == "OneTimeLeaseRent")
                                {
                                    data.LeaseRentDues = rentList[count].totalamount;
                                    data.LeaseRentInterest = rentList[count].penlatintrest;
                                    data.LeaseRentPerAnnum = count == 0 ? rentList[count].leaserent : (rentList[count].leaserent == 0 ? rentList[count - 1].leaserent : rentList[count].leaserent);
                                    onetime = data.LeaseRentPerAnnum * 15;
                                    data.OneTimeLeaseRentAmount = onetime;
                                    data.TotalDuesAmount = onetime > 0 ? (onetime + rentList[count].Total_dues) : rentList[count].Total_dues;
                                    data.DuesUptoDate = today;
                                    data.ActionTypeId = ReturnType.Exist;
                                }
                                else
                                {
                                    //data.TotalAmount = rentList[count].totalamount;
                                    //data.PenalInterest = rentList[count].penlatintrest;                                
                                    //data.LeaseRentPerAnnum = rentList[count].leaserent;
                                    //data.LeaseRentDues = rentList[count].totalamount;
                                    //data.LeaseRentInterest = rentList[count].penlatintrest;
                                    data.LeaseRentDues = rentList[count].totalamount < 0 ? 0 : rentList[count].totalamount;
                                    data.LeaseRentInterest = rentList[count].totalamount < 0 ? rentList[count].penlatintrest + rentList[count].totalamount : rentList[count].penlatintrest;
                                    data.TotalDuesAmount = onetime > 0 ? (onetime + rentList[count].Total_dues) : rentList[count].Total_dues;
                                    data.DuesUptoDate = today;
                                    //if (lease.LeaseDeedDate != null && lease.LeaseDeedDate.Value.Month >= rentList[rentList.Count - 1].duedate.Value.Month)
                                    if (lease.LeaseDeedDate != null && lease.LeaseDeedDate >= rentList[rentList.Count - 1].duedate)
                                    {
                                        int preyear1 = rentList[0].duedate.Value.Year;
                                        int preyear2 = rentList[0].duedate.Value.AddYears(1).Year;
                                        int postyear1 = rentList[rentList.Count - 2].duedate.Value.AddYears(-1).Year;
                                        int postyear2 = rentList[rentList.Count - 2].duedate.Value.Year;
                                        if (preyear1 == postyear1)
                                        {
                                            data.StartYearInWord = preyear1.ToString();
                                            data.EndYearInWord = preyear2.ToString();
                                        }
                                        else
                                        {
                                            data.StartYearInWord = preyear1 + "-" + preyear2;
                                            data.EndYearInWord = postyear1 + "-" + postyear2;
                                        }
                                    }
                                    else
                                    {
                                        int preyear1 = rentList[0].duedate.Value.Year;
                                        int preyear2 = rentList[0].duedate.Value.AddYears(1).Year;
                                        int postyear1 = rentList[rentList.Count - 2].duedate.Value.Year;
                                        int postyear2 = rentList[rentList.Count - 2].duedate.Value.AddYears(1).Year;
                                        if (preyear1 == postyear1)
                                        {
                                            data.StartYearInWord = preyear1.ToString();
                                            data.EndYearInWord = preyear2.ToString();
                                        }
                                        else
                                        {
                                            data.StartYearInWord = preyear1 + "-" + preyear2;
                                            data.EndYearInWord = postyear1 + "-" + postyear2;
                                        }
                                    }
                                    

                                    if (model.ActionType == "Leaserent-Compensation")
                                    {
                                        var compensation = dbContext.PropertyCompensationMsts.FirstOrDefault(c => c.PropertyId.ToString() == data.PropertyId);
                                        if (compensation != null)
                                        {
                                            data.CompensationAmount = compensation.TotalCompensationAmount;
                                            data.TotalDuesAmount = data.TotalDuesAmount + compensation.TotalCompensationAmount;
                                        }
                                    }

                                    Int64 totalAmount = (Int64)Math.Ceiling(data.TotalDuesAmount.Value);
                                    data.AmountInWords = string.IsNullOrEmpty(ApplicationHelper.ConvertNumberIntoWords(totalAmount)) ? string.Empty : (ApplicationHelper.ConvertNumberIntoWords(totalAmount) + " Only");

                                    data.ActionType = "Lease Rent Dues Payment";
                                    data.DemandNoteContent = "You are informed that last date for dues against above mentioned property is " + DateTime.Now.AddDays(10).ToString("dd/MM/yyyy") + ". " + " Kindly pay your dues amount by any bank generated challan in noida.";
                                    data.ActionTypeId = ReturnType.Exist;
                                }

                            }
                            else
                            {
                                if (rentList.Count == 0 && model.ActionType == "OneTimeLeaseRent")
                                {
                                    //data.LeaseRentDues = 0;
                                    //data.LeaseRentInterest = 0;
                                    data.LeaseRentPerAnnum = lease.RevisedPremium == null ? lease.PremiumLeaseRent : lease.RevisedPremium;
                                    decimal? onetime = data.LeaseRentPerAnnum * 15;
                                    data.OneTimeLeaseRentAmount = onetime;
                                    data.TotalDuesAmount = onetime;
                                    data.DuesUptoDate = today;
                                    data.ActionTypeId = ReturnType.Exist;
                                }
                                else
                                {
                                    data.ActionTypeId = ReturnType.Other;
                                }

                            }

                        }
                    }
                    else
                    {
                        data.ActionTypeId = ReturnType.NotExist;
                    }

                    var kya = dbContext.KYADetails.Where(k => k.RId == model.RegistrationId).OrderByDescending(o => o.Id).FirstOrDefault();
                    if (kya != null)
                    {
                        data.KYAStatusId = kya.StatusId;
                    }

                    return data;
                }
                else
                {
                    model.ActionTypeId = ReturnType.NotAllotted;
                    return model;
                }
            }
        }


        public LeaseRentViewModel GetDetailsByIdForLeaseRent(LeaseRentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var usr = (from alotment in dbContext.AllotmentMasters
                           join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                           join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                           where alotment.rid == model.RegistrationId
                           select new LeaseRentViewModel
                           {
                               RegistrationId = alotment.rid,
                               DepartmentId = alotment.departmentId,
                               PropertyId = alotment.propertyId,
                               Applicant = aplicant.tGender.ToLower() == Constants.Company.ToLower() ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : (aplicant.tFirstName + " " + ((aplicant.tMiddleName == null) ? "" : aplicant.tMiddleName + " ") + aplicant.tLastName),
                               MobileNo = aplicant.tMobileNumber,
                               Email = aplicant.tEmail,
                               Sector = property.SectorMst.sectorName,
                               Block = property.BlockMst.blockName,
                               PlotNo = property.propertyNo,
                               Address = aplicant.tCorrespondanceAdd,
                               PropertyNo = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo,
                               RegistryDate = alotment.RegistryDetails.Where(m => m.Rid == alotment.rid).FirstOrDefault() != null ? alotment.RegistryDetails.Where(m => m.Rid == alotment.rid).FirstOrDefault().RegistryDoneDate : null

                           }).FirstOrDefault();
                var leaserent = dbContext.LeaseRentPayments.FirstOrDefault(l => l.RegistrationId == model.RegistrationId);
                if (leaserent != null)
                {
                    usr.LeaseRentPerAnnum = leaserent.PremiumLeaseRent;
                    usr.RevisedLeaseRentPerAnnum = leaserent.RevisedPremium;
                    usr.RevisedPremium = leaserent.RevisedPremium;
                    usr.RevisedDate = leaserent.RevisedPremiumDate;
                    usr.RevisedRate = leaserent.RevisedRate;
                    usr.PanelInterest = leaserent.PanelInterest;
                    usr.IsOneTimeLeaseRentPaid = leaserent.IsOneTimeLeasePaid;
                    usr.IsTotalInstallmentPaid = leaserent.IsTotalInstallmentPaid;
                    usr.PremiumPaidDuration = leaserent.PremiumPaidDuration;
                    usr.BalanceAmount = leaserent.BalanceAmount;
                    usr.BalanceInterest = leaserent.BalanceInterest;
                    usr.CurrentDues = leaserent.CurrentDues;
                    usr.CurrentDuesDate = leaserent.CurrentDuesDate;
                    usr.LeaseRentDues = leaserent.LeaseRentDues;
                    usr.DuesUptoDate = leaserent.DuesUptoDate;
                    usr.TransferLeaseDate = leaserent.TransferLeaseDate;
                    usr.PaidUptoDate = leaserent.PremiumPaidUptoDate;
                    usr.ReturnTypeId = ReturnType.Exist;
                }
                else
                {
                    if (usr != null)
                    {
                        usr.ReturnTypeId = ReturnType.NotExist;
                    }
                    else
                    {
                        usr = new LeaseRentViewModel { ReturnTypeId = ReturnType.NotAllotted };
                    }
                }
                return usr;
            }
        }


        public DataSourceResult GetLeaseRentRequestList(DataSourceRequest request, LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                bool IsOTLRP = model.PremiumPaidStatus == NA.PMS.Common.NDCOptions.Id_Yes ? true : false;
                var list = (from rentmaster in dbcontext.LeaseRentPayments
                            //join alotment in dbcontext.AllotmentMasters on rentmaster.RegistrationId equals alotment.rid
                            //join property in dbcontext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where (model.DepartmentId == null || rentmaster.DepartmentId == model.DepartmentId) && (rentmaster.Approver == userInfo.UserID)
                            select new LeaseRentViewModel
                            {
                                Id = rentmaster.Id,
                                RentId = rentmaster.Id,
                                RegistrationId = rentmaster.RegistrationId,
                                Sector = rentmaster.Sector,
                                Block = rentmaster.Block,
                                PlotNo = rentmaster.PlotNo,
                                //PropertyNo = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "/" + property.propertyNo,
                                LeaseDeedDate = rentmaster.LeaseDeedDate,
                                TransferLeaseDate = rentmaster.TransferLeaseDate,
                                LeaseRentPremium = rentmaster.PremiumLeaseRent,
                                RevisedPremium = rentmaster.RevisedPremium,
                                RevisedDate = rentmaster.RevisedPremiumDate,
                                PanelInterest = rentmaster.PanelInterest,
                                NDCDate = rentmaster.NDCDate,
                                IsOneTimeLeaseRentPaid = rentmaster.IsOneTimeLeasePaid,
                                IsTotalLeaseRentPaid = rentmaster.IsTotalPremiumPaid,
                                IsLeaseRentPaid = rentmaster.IsOneTimeLeasePaid,
                                LeaseRentStatus = rentmaster.IsOneTimeLeasePaid == true ? "Paid" : "Not Paid",
                                PremiumPaidDuration = rentmaster.PremiumPaidDuration,
                                PaidUptoDate = rentmaster.PremiumPaidUptoDate,
                                IsTotalInstallmentPaid = rentmaster.IsTotalInstallmentPaid,
                                IsOneTimeInstallmentPaid = rentmaster.IsOneTimeInstallmentPaid,
                                IsNDCGenerated = (rentmaster.IsOneTimeLeasePaid == true && rentmaster.IsTotalInstallmentPaid == true) ? true : false,
                                PremiumPaidStatus = (rentmaster.IsOneTimeInstallmentPaid == true || rentmaster.IsTotalInstallmentPaid == true) ? "Paid" : "Not Paid",
                                BalanceAmount = rentmaster.BalanceAmount,
                                BalanceInterest = rentmaster.BalanceInterest,
                                BalanceUptoDate = rentmaster.BalanceUptoDate,
                                LeaseRentDues = rentmaster.LeaseRentDues,
                                DuesUptoDate = rentmaster.DuesUptoDate,
                                TotalBalance = rentmaster.BalanceAmount,
                                PreviousDues = rentmaster.PreviousDues,
                                PreviousDuesDate = rentmaster.PreviousDuesDate,
                                CurrentDues = rentmaster.CurrentDues,
                                CurrentDuesDate = rentmaster.CurrentDuesDate,
                                ChallanDate = rentmaster.ChallanDate,
                                StatusId = rentmaster.StatusId,
                                Status = (rentmaster.StatusId != null || rentmaster.StatusId > 0) ? (from status in dbcontext.StatusMasters where status.Id == rentmaster.StatusId select status.Status).FirstOrDefault() : string.Empty,
                                IsActive = rentmaster.IsActive
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public PaymentViewModel GetInstallmentDuesForDemandNote(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from alotment in dbContext.AllotmentMasters
                            join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where alotment.rid == model.RegistrationId
                            select new PaymentViewModel
                            {
                                RegistrationId = alotment.rid,
                                DepartmentId = alotment.departmentId,
                                Department = alotment.DepartmentMst.departmentName,
                                PropertyId = alotment.propertyId.ToString(),

                                Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : (aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName),
                                ApplicantAddress = aplicant.tCorrespondanceAdd,
                                MobileNo = aplicant.tMobileNumber,
                                Email = aplicant.tEmail,

                                SectorId = property.sectorId,
                                SectorName = property.SectorMst.sectorName,
                                BlockId = property.blockId,
                                BlockName = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,

                            }).FirstOrDefault();
                if (data != null)
                {
                    decimal? totaldues = 0;
                    if (model.FilterType == "Compensation")
                    {
                        var compensation = GetCompensationDuesById(data);
                        data.TotalDuesAmount = compensation.TotalCompensationAmount;
                        data.DuesUptoDate = model.ActionDate;
                        totaldues = totaldues + compensation.TotalCompensationAmount;

                        Int64 totalAmount = (Int64)Math.Ceiling(data.TotalDuesAmount.Value);
                        data.AmountInWords = string.IsNullOrEmpty(ApplicationHelper.ConvertNumberIntoWords(totalAmount)) ? string.Empty : (ApplicationHelper.ConvertNumberIntoWords(totalAmount) + " Only");
                        return data;
                    }
                    else
                    {
                        DateTime? duesdate = model.ActionDate == null ? DateTime.Now : model.ActionDate;
                        var list = dbContext.Sp_DuesCalculationHistory(model.RegistrationId.ToString(), duesdate, data.DepartmentId).ToList();
                        if (list != null && list.Count > 0)
                        {
                            var currentDues = list[list.Count - 1];
                            data.InstallmentAmount = currentDues.TotalPrincipalAmount;
                            data.InstallmentInterest = currentDues.PenalAmount;
                            data.PrincipalAmount = currentDues.TotalPrincipalAmount;
                            data.TotalDuesAmount = currentDues.DuesNextWithInterest;
                            data.DuesUptoDate = currentDues.InstallmentDueDate == null ? DateTime.Now : currentDues.InstallmentDueDate;
                            data.ActionType = "Installment Dues Payment";
                            data.DemandNoteContent = "You are informed that last date for dues against above mentioned property is " + DateTime.Now.ToString("dd/MM/yyyy") + ". " + " Kindly pay your dues amount by any bank generated challan in noida.";
                            data.ActionTypeId = ReturnType.Exist;

                            Int64 totalAmount = (Int64)Math.Ceiling(data.TotalDuesAmount.Value);
                            data.AmountInWords = string.IsNullOrEmpty(ApplicationHelper.ConvertNumberIntoWords(totalAmount)) ? string.Empty : (ApplicationHelper.ConvertNumberIntoWords(totalAmount) + " Only");
                        }
                        else
                        {
                            data.ActionTypeId = ReturnType.Paid;
                        }
                    }
                    
                    return data;
                }
                else
                {
                    model.ActionTypeId = ReturnType.NotAllotted;
                    return model;
                }
            }
        }


        //public PaymentViewModel GetDuesAmountForDemandNoteById(PaymentViewModel model)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var data = (from alotment in dbContext.AllotmentMasters
        //                    join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
        //                    join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
        //                    where alotment.rid == model.RegistrationId
        //                    select new PaymentViewModel
        //                    {
        //                        RegistrationId = alotment.rid,
        //                        DepartmentId = alotment.departmentId,
        //                        Department = alotment.DepartmentMst.departmentName,
        //                        PropertyId = alotment.propertyId.ToString(),

        //                        Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : (aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName),
        //                        ApplicantAddress = aplicant.tCorrespondanceAdd,
        //                        MobileNo = aplicant.tMobileNumber,
        //                        Email = aplicant.tEmail,

        //                        SectorId = property.sectorId,
        //                        SectorName = property.SectorMst.sectorName,
        //                        BlockId = property.blockId,
        //                        BlockName = property.BlockMst.blockName,
        //                        PlotNo = property.propertyNo,

        //                    }).FirstOrDefault();
        //        if (data != null)
        //        {
        //            decimal? totaldues = 0;
        //            if (model.FilterType == "Compensation")
        //            {
        //                var compensation = GetCompensationDuesById(data);
        //                data.TotalDuesAmount = compensation.TotalCompensationAmount;
        //                data.DuesUptoDate = model.ActionDate;
        //                totaldues = totaldues + compensation.TotalCompensationAmount;

        //                Int64 totalAmount = (Int64)Math.Ceiling(data.TotalDuesAmount.Value);
        //                data.AmountInWords = string.IsNullOrEmpty(ApplicationHelper.ConvertNumberIntoWords(totalAmount)) ? string.Empty : (ApplicationHelper.ConvertNumberIntoWords(totalAmount) + " Only");
        //                return data;
        //            }
        //            else
        //            {
        //                var list = dbContext.Sp_DuesCalculationHistory(model.RegistrationId.ToString(), model.ActionDate, model.PropertyTypeId).ToList();
        //                //var list = dbContext.Sp_DuesCalculationHistory(model.RegistrationId.ToString(), DateTime.Now, data.DepartmentId).ToList();
        //                if (list != null && list.Count > 0)
        //                {
        //                    var currentDues = list[list.Count - 1];
        //                    //data.DuesAmount = currentDues.DuesTill;
        //                    totaldues = currentDues.DuesNextWithInterest;
        //                    data.InstallmentAmount = currentDues.TotalPrincipalAmount;
        //                    data.InstallmentInterest = currentDues.PenalAmount;
        //                    //data.InstallmentInterest = currentDues.TotalPanelAmount;
        //                    data.PrincipalAmount = currentDues.TotalPrincipalAmount;
        //                    data.TotalInstallment = currentDues.DuesNextWithInterest;
        //                    data.DuesUptoDate = model.ActionDate;// currentDues.InstallmentDueDate == null ? DateTime.Now : currentDues.InstallmentDueDate;

        //                    data.ActionTypeId = ReturnType.Exist;
        //                }

        //                var today = DateTime.Now;
        //                var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        //                today = DateTime.Now.AddDays(days - DateTime.Now.Day);

        //                //List<Sp_LeaseRentDuesCalculationTillDate_Result> rentList = dbContext.Sp_LeaseRentDuesCalculationTillDate(data.RegistrationId.ToString(), today, data.DepartmentId).ToList();
        //                List<Sp_LeaseRentDuesCalculationTillDate_Result> rentList = dbContext.Sp_LeaseRentDuesCalculationTillDate(data.RegistrationId.ToString(), model.ActionDate, data.DepartmentId).ToList();
        //                if (rentList != null && rentList.Count > 1)
        //                {
        //                    var count = rentList.Count - 1;
        //                    data.LeaseRentPerAnnum = rentList[count].leaserent == 0 ? rentList[count - 1].leaserent : rentList[count].leaserent;
        //                    data.LeaseRentDues = rentList[count].totalamount;
        //                    data.LeaseRentInterest = rentList[count].penlatintrest;
        //                    //data.LeaseRentInterest = rentList[count].penlatintrest;
        //                    //data.TotalDuesAmount = rentList[count].Total_dues;
        //                    data.TotalLeaseRent = rentList[count].Total_dues;
        //                    totaldues = totaldues + rentList[count].Total_dues;

        //                    int preyear1 = rentList[0].duedate.Value.Year;
        //                    int preyear2 = rentList[0].duedate.Value.AddYears(1).Year;
        //                    int postyear1 = rentList[rentList.Count - 2].duedate.Value.Year;
        //                    int postyear2 = rentList[rentList.Count - 2].duedate.Value.AddYears(1).Year;
        //                    if (preyear1 == postyear1)
        //                    {
        //                        data.StartYearInWord = preyear1.ToString();
        //                        data.EndYearInWord = preyear2.ToString();
        //                    }
        //                    else
        //                    {
        //                        data.StartYearInWord = preyear1 + "-" + preyear2;
        //                        data.EndYearInWord = postyear1 + "-" + postyear2;
        //                    }
        //                    data.ActionTypeId = ReturnType.Exist;
        //                }

        //                var block = (string.IsNullOrEmpty(data.BlockName) || data.BlockName == Constants.NA || data.BlockName == Constants.BlockNotAvailable) ? string.Empty : (" Block-" + data.BlockName);
        //                data.ActionType = "Leaserent/Installment Dues Payment against Property Sector-" + data.SectorName + block + " Plot-" + data.PlotNo;
        //                data.DemandNoteContent = "You are informed that last date for dues against above mentioned property is " + DateTime.Now.AddDays(10).ToString("dd/MM/yyyy") + ". " + " Kindly pay your dues amount by any bank generated challan in noida.";
        //                data.TotalDuesAmount = totaldues;

        //                Int64 totalAmount = (Int64)Math.Ceiling(data.TotalDuesAmount.Value);
        //                data.AmountInWords = string.IsNullOrEmpty(ApplicationHelper.ConvertNumberIntoWords(totalAmount)) ? string.Empty : (ApplicationHelper.ConvertNumberIntoWords(totalAmount) + " Only");

        //                return data;
        //            }
        //        }
        //        else
        //        {
        //            model.ActionTypeId = ReturnType.NotAllotted;
        //            return model;
        //        }
        //    }
        //}

        public PaymentViewModel GetDuesAmountForDemandNoteById(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from alotment in dbContext.AllotmentMasters
                            join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where alotment.rid == model.RegistrationId
                            select new PaymentViewModel
                            {
                                RegistrationId = alotment.rid,
                                DepartmentId = alotment.departmentId,
                                Department = alotment.DepartmentMst.departmentName,
                                PropertyId = alotment.propertyId.ToString(),

                                Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : (aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName),
                                ApplicantAddress = aplicant.tCorrespondanceAdd,
                                MobileNo = aplicant.tMobileNumber,
                                Email = aplicant.tEmail,

                                SectorId = property.sectorId,
                                SectorName = property.SectorMst.sectorName,
                                BlockId = property.blockId,
                                BlockName = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,

                            }).FirstOrDefault();
                if (data != null)
                {
                    //decimal? totaldues = 0;
                    data.ActionDate = model.ActionDate == null ? DateTime.Now : model.ActionDate;

                    if (model.FilterType == "LeaseRent")
                    {
                        data = GetLeaseRentDuesById(data);
                    }
                    if (model.FilterType == "OneTimeLeaseRent")
                    {
                        data = GetLeaseRentDuesById(data);
                        if (data.ActionTypeId != ReturnType.Paid)
                        {
                            data.OneTimeLeaseRentAmount = data.LeaseRentPerAnnum * 15;
                            data.TotalDuesAmount = data.TotalLeaseRent + data.OneTimeLeaseRentAmount;
                        }
                    }
                    if (model.FilterType == "Installment")
                    {
                        data = GetInstallmentDuesById(data);
                    }
                    if (model.FilterType == "Leaserent-Installment")
                    {
                        var premium = GetInstallmentDuesById(data);
                        if (premium.IsTotalInstallmentPaid != true)
                        {
                            var installment = premium.InstallmentAmount;
                            
                            data.InstallmentAmount = (premium.InstallmentAmount < 0 || premium.InstallmentAmount == null) ? 0 : premium.InstallmentAmount;
                            var interest = premium.InstallmentAmount == null ? null : (installment < 0 ? (installment + (premium.InstallmentInterest == null ? 0 : premium.InstallmentInterest)) : premium.InstallmentInterest);
                            //data.InstallmentInterest = (premium.InstallmentInterest < 0 || premium.InstallmentInterest == null) ? 0 : premium.InstallmentInterest;
                            data.InstallmentInterest = interest;
                            data.PrincipalAmount = (premium.PrincipalAmount < 0 || premium.PrincipalAmount == null) ? 0 : premium.PrincipalAmount;
                            data.TotalInstallment = (premium.TotalInstallment < 0 || premium.TotalInstallment == null) ? 0 : premium.TotalInstallment;
                        }
                        else
                        {
                            data.TotalInstallment = 0;
                        }
                        var leaseRent = GetLeaseRentDuesById(data);
                        if (leaseRent.ActionTypeId != ReturnType.Paid)
                        {
                            data.LeaseRentPerAnnum = (leaseRent.LeaseRentPerAnnum < 0 || leaseRent.LeaseRentPerAnnum == null) ? 0 : leaseRent.LeaseRentPerAnnum;
                            data.LeaseRentDues = (leaseRent.LeaseRentDues < 0 || leaseRent.LeaseRentDues == null) ? 0 : leaseRent.LeaseRentDues;
                            data.LeaseRentInterest = (leaseRent.LeaseRentInterest < 0 || leaseRent.LeaseRentInterest == null) ? 0 : leaseRent.LeaseRentInterest;
                            data.TotalLeaseRent = (leaseRent.TotalLeaseRent == null || leaseRent.TotalLeaseRent < 0) ? 0 : leaseRent.TotalLeaseRent;
                        }
                        else
                        {
                            data.TotalLeaseRent = 0;
                        }

                        data.TotalDuesAmount = premium.TotalInstallment + leaseRent.TotalLeaseRent;
                    }
                    if (model.FilterType == "Compensation")
                    {
                        data = GetCompensationDuesById(data);
                        data.DuesUptoDate = data.ActionDate != null ? data.ActionDate : DateTime.Now;
                        data.TotalDuesAmount = (decimal)Math.Ceiling(data.TotalCompensationAmount.Value);
                    }
                    if (model.FilterType == "Leaserent-Compensation")
                    {
                        var leaseRent = GetLeaseRentDuesById(data);
                        if (leaseRent.ActionTypeId != ReturnType.Paid)
                        {
                            data.LeaseRentPerAnnum = leaseRent.LeaseRentPerAnnum;
                            data.LeaseRentDues = leaseRent.LeaseRentDues;
                            data.LeaseRentInterest = leaseRent.LeaseRentInterest;
                            data.TotalLeaseRent = leaseRent.TotalLeaseRent == null ? 0 : leaseRent.TotalLeaseRent;
                        }
                        else
                        {
                            data.TotalLeaseRent = 0;
                        }

                        var compensation = GetCompensationDuesById(data);
                        data.TotalCompensationAmount = compensation.TotalCompensationAmount;
                        data.TotalDuesAmount = leaseRent.TotalLeaseRent + compensation.TotalCompensationAmount;
                    }
                    if (model.FilterType == "Installment-Compensation")
                    {
                        var premium = GetInstallmentDuesById(data);
                        if (premium.IsTotalInstallmentPaid != true)
                        {
                            data.InstallmentAmount = premium.InstallmentAmount;
                            data.InstallmentInterest = premium.InstallmentInterest;
                            data.PrincipalAmount = premium.PrincipalAmount;
                            data.TotalInstallment = premium.TotalInstallment == null ? 0 : premium.TotalInstallment;
                        }
                        else
                        {
                            data.TotalInstallment = 0;
                        }

                        var compensation = GetCompensationDuesById(data);
                        data.TotalCompensationAmount = compensation.TotalCompensationAmount;

                        data.TotalDuesAmount = data.TotalInstallment + compensation.TotalCompensationAmount;
                    }
                    if (model.FilterType == "Installment-Leaserent-Compensation")
                    {
                        var premium = GetInstallmentDuesById(data);
                        if (premium.IsTotalInstallmentPaid != true)
                        {
                            data.InstallmentAmount = premium.InstallmentAmount;
                            data.InstallmentInterest = premium.InstallmentInterest;
                            data.PrincipalAmount = premium.PrincipalAmount;
                            data.TotalInstallment = premium.TotalInstallment == null ? 0 : premium.TotalInstallment;
                        }
                        else
                        {
                            data.TotalInstallment = 0;
                        }
                        var leaseRent = GetLeaseRentDuesById(data);
                        if (leaseRent.ActionTypeId != ReturnType.Paid)
                        {
                            data.LeaseRentPerAnnum = leaseRent.LeaseRentPerAnnum;
                            data.LeaseRentDues = leaseRent.LeaseRentDues;
                            data.LeaseRentInterest = leaseRent.LeaseRentInterest;
                            data.TotalLeaseRent = leaseRent.TotalLeaseRent == null ? 0 : leaseRent.TotalLeaseRent;
                        }
                        else
                        {
                            data.TotalLeaseRent = 0;
                        }

                        var compensation = GetCompensationDuesById(data);
                        data.TotalCompensationAmount = compensation.TotalCompensationAmount;

                        data.TotalDuesAmount = premium.TotalInstallment + leaseRent.TotalLeaseRent + compensation.TotalCompensationAmount;
                    }
                    data.TotalDuesAmount = data.TotalDuesAmount == null ? 0 : data.TotalDuesAmount;
                    Int64 totalAmount = (Int64)Math.Ceiling(data.TotalDuesAmount.Value);
                    data.AmountInWords = string.IsNullOrEmpty(ApplicationHelper.ConvertNumberIntoWords(totalAmount)) ? string.Empty : (ApplicationHelper.ConvertNumberIntoWords(totalAmount) + " Only");
                    //return data;
                    var block = (string.IsNullOrEmpty(data.BlockName) || data.BlockName == Constants.NA || data.BlockName == Constants.BlockNotAvailable) ? string.Empty : (" Block-" + data.BlockName);
                    data.ActionType = "Leaserent/Installment Dues Payment against Property Sector-" + data.SectorName + block + " Plot-" + data.PlotNo;
                    data.DemandNoteContent = "You are informed that last date for dues against above mentioned property is " + DateTime.Now.AddDays(10).ToString("dd/MM/yyyy") + ". " + " Kindly pay your dues amount by any bank generated challan in noida.";
                    
                    return data;
                }
                else
                {
                    model.ActionTypeId = ReturnType.NotAllotted;
                    return model;
                }
            }
        }

        private PaymentViewModel GetInstallmentDuesById(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.LeaseRentPayments.FirstOrDefault(c=>c.RegistrationId==model.RegistrationId);
                //if (data.IsTotalPremiumPaid == true || data.IsTotalInstallmentPaid == true || data.IsOneTimeInstallmentPaid == true)
                if (data != null && data.IsTotalPremiumPaid == true)
                {
                    model.IsTotalInstallmentPaid = true;
                    model.ActionTypeId = ReturnType.Paid;
                }
                else
                {
                    var list = dbContext.Sp_DuesCalculationHistory(model.RegistrationId.ToString(), model.ActionDate, model.PropertyTypeId).ToList();
                    if (list != null && list.Count > 0)
                    {
                        var currentDues = list[list.Count - 1];
                        model.InstallmentAmount = currentDues.TotalPrincipalAmount;
                        //model.InstallmentInterest = currentDues.PenalAmount;
                        model.InstallmentInterest = currentDues.PenalAmount;
                        //model.InstallmentInterest = currentDues.TotalPanelAmount;
                        model.PrincipalAmount = currentDues.TotalPrincipalAmount;
                        model.TotalInstallment = (Int64)Math.Ceiling(currentDues.DuesNextWithInterest.Value);
                        model.TotalDuesAmount = currentDues.DuesNextWithInterest;
                        model.DuesUptoDate = model.ActionDate;
                        model.ActionTypeId = ReturnType.Exist;
                    }
                }               
                return model;
            }
        }

        private PaymentViewModel GetLeaseRentDuesById(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lease = dbContext.LeaseRentPayments.FirstOrDefault(c => c.RegistrationId == model.RegistrationId);
                if (lease != null)
                {
                    if (lease.IsOneTimeLeasePaid.Value == true)
                    {
                        model.IsOneTimeLeasePaid = "True";
                        model.ActionTypeId = ReturnType.Paid;
                    }
                    else
                    {
                        List<Sp_LeaseRentDuesCalculationTillDate_Result> rentList = dbContext.Sp_LeaseRentDuesCalculationTillDate(model.RegistrationId.ToString(), model.ActionDate, model.DepartmentId).ToList();
                        if (rentList != null && rentList.Count > 1)
                        {
                            var count = rentList.Count - 1;
                            model.LeaseRentPerAnnum = rentList[count].leaserent == 0 ? rentList[count - 1].leaserent : rentList[count].leaserent;
                            model.LeaseRentDues = rentList[count].totalamount;
                            model.LeaseRentInterest = rentList[count].penlatintrest;
                            model.TotalLeaseRent = rentList[count].Total_dues;
                            model.TotalDuesAmount = rentList[count].Total_dues;
                            model.DuesUptoDate = model.ActionDate == null ? DateTime.Now : model.ActionDate;
                            int preyear1 = rentList[0].duedate.Value.Year;
                            int preyear2 = rentList[0].duedate.Value.AddYears(1).Year;
                            int postyear1 = rentList[rentList.Count - 2].duedate.Value.Year;
                            int postyear2 = rentList[rentList.Count - 2].duedate.Value.AddYears(1).Year;
                            if (preyear1 == postyear1)
                            {
                                model.StartYearInWord = preyear1.ToString();
                                model.EndYearInWord = preyear2.ToString();
                            }
                            else
                            {
                                model.StartYearInWord = preyear1 + "-" + preyear2;
                                model.EndYearInWord = postyear1 + "-" + postyear2;
                            }
                            model.ActionTypeId = ReturnType.Exist;
                        }
                    }
                }
                return model;
            }
        }

        private PaymentViewModel GetCompensationDuesById(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var compensation = dbContext.PropertyCompensationMsts.FirstOrDefault(c => c.PropertyId.ToString() == model.PropertyId);
                if (compensation != null)
                {
                    model.TotalCompensationAmount = compensation.TotalCompensationAmount;
                    model.CompensationAmount = compensation.TotalCompensationAmount;
                    model.DuesUptoDate = model.ActionDate;
                }
                else
                {
                    model.CompensationAmount = 0;
                    model.TotalCompensationAmount = 0;
                }
                return model;
            }
        }

        public DataSourceResult GetPropertyPaymentDetails(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.SectorName == null && model.BlockName == null && model.PlotNo == null && model.RegistrationNo == null)
                {
                    List<PaymentViewModel> list = new List<PaymentViewModel>();
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    var list = (from detail in dbContext.RECEIPT_DETAIL_MASTER
                                where (model.SectorName == null || detail.SECTOR == model.SectorName)
                                && (model.BlockName == null || detail.BLOCK == model.BlockName)
                                && (model.PlotNo == null || detail.PROP_ID == model.PlotNo)
                                && (model.RegistrationNo == null || detail.RID_NO == model.RegistrationNo)
                                && detail.STATUS == 1
                                select new PaymentViewModel
                                {
                                    //Id = 0,
                                    //RegistrationId = Convert.ToInt32(detail.RID_NO),
                                    ReceiptId = detail.RECEIPT_ID,
                                    RegistrationNo = detail.RID_NO,
                                    DepartmentName = detail.DEPT_ID == null ? " " : dbContext.DepartmentMsts.FirstOrDefault(x => x.departmentId == detail.DEPT_ID).departmentName,
                                    Applicant = detail.ALLOTE_NAME,
                                    DepositorName = detail.DEPOSETER_NAME,
                                    DepositorAddress = detail.ADDRESS,
                                    DepositDate = detail.DEPOSIT_DATE,
                                    Amount = detail.AMOUNT,
                                    SectorName = detail.SECTOR,
                                    BlockName = detail.BLOCK,
                                    PlotNo = detail.PROP_ID
                                });
                    return list.ToDataSourceResult(request);
                }
            }
        }


        public int UpdateRegistrationIdByPropertyNo(PaymentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var registrionId = dbContext.RECEIPT_DETAIL_MASTER.Where(m => m.RID_NO == model.RegistrationNo).FirstOrDefault();
                var details = dbContext.RECEIPT_DETAIL_MASTER.Where(m => m.SECTOR == model.SectorName && m.BLOCK == model.BlockName && m.PROP_ID == model.PlotNo).ToList();
                var oldRid = dbContext.ApplicationDetails.FirstOrDefault(m => m.registrationId.ToString() == model.RegistrationNo);
                //if (registrionId != null)
                //{
                //    var detail = details.FirstOrDefault();
                //    if (detail != null)
                //    {
                //        if (registrionId.SECTOR == detail.SECTOR && registrionId.BLOCK == detail.BLOCK && registrionId.PROP_ID == detail.PROP_ID)
                //        {
                //            if (details != null && details.Count > 0)
                //            {
                //                string rid = string.Empty;
                //                foreach (var data in details)
                //                {
                //                    if (data.RID_NO != null)
                //                    {
                //                        rid = data.RID_NO;
                //                    }
                //                }
                //                if (string.IsNullOrEmpty(rid) || rid == model.RegistrationNo)
                //                {
                //                    //details.Each(e => e.RID_NO = model.RegistrationNo);
                //                    foreach (var data in details)
                //                    {
                //                        data.RID_NO = model.RegistrationNo;
                //                        data.MODIFIED_BY = userInfo.UserID;
                //                        data.MODIFY_DATE = System.DateTime.Now;
                //                    }
                //                    dbContext.SaveChanges();
                //                    flag = ReturnType.Updated;
                //                }
                //                else
                //                {
                //                    flag = ReturnType.Mismatch;
                //                }
                //            }
                //        }
                //        else
                //        {
                //            flag = ReturnType.Mismatch;
                //        }
                //    }
                //}
                //else
                //{
                if (details != null && details.Count > 0)
                {
                    string rid = string.Empty;
                    foreach (var data in details)
                    {
                        if (data.RID_NO != null)
                        {
                            rid = data.RID_NO;
                        }
                    }
                    if (string.IsNullOrEmpty(rid) || rid == model.RegistrationNo || rid == oldRid.OldRegistrationId.ToString())
                    {
                        foreach (var data in details)
                        {
                            data.RID_NO = model.RegistrationNo;
                            data.MODIFIED_BY = userInfo.UserID;
                            data.MODIFY_DATE = System.DateTime.Now;
                        }
                        dbContext.SaveChanges();
                        flag = ReturnType.Updated;
                    }
                    else
                    {
                        flag = ReturnType.Mismatch;
                    }
                }
                //}
            }
            return flag;
        }


        public DataSourceResult GetReceiptDetailsById(DataSourceRequest request, long? receiptId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from receipt in dbContext.RECEIPT_DETAIL_MASTER
                            join receiptTrans in dbContext.RECEIPT_AMOUNT_TRANS on receipt.RECEIPT_ID equals receiptTrans.RECEIPT_ID
                            where receipt.RECEIPT_ID == receiptId
                            select new PaymentViewModel
                            {
                                Id = receiptTrans.id,
                                ReceiptId = receipt.RECEIPT_ID,
                                Amount = receiptTrans.AMOUNT_PAID,
                                ReceiptHeadName = receiptTrans.RECEIPT_HEAD_ID == null ? string.Empty : dbContext.RECIEPT_HEAD.FirstOrDefault(x => x.RECIEPT_CODE == receiptTrans.RECEIPT_HEAD_ID && x.STATUS == 1).RECIEPT_HEAD_NAME,
                                RegistrationNo = null,
                                ReceiptSubHeadName = receiptTrans.RECEIPT_SUBHEAD_ID == null ? string.Empty : dbContext.RECEIPT_SUB_HEAD.FirstOrDefault(x => x.RECEIPT_CODE == receiptTrans.RECEIPT_HEAD_ID && x.RECEIPT_SUBHEAD_ID == receiptTrans.RECEIPT_SUBHEAD_ID && x.STATUS == 1).RECEIPT_SUB_HEAD1
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public int UpdateDuesPaymentByDepartment(PaymentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbcontext = new NoidaPMSEntities())
            {
                if (model.DepartmentId != null && model.DepartmentId != 0)
                {
                    var datalist = (from leaserent in dbcontext.LeaseRentPayments
                                    where (leaserent.IsOneTimeLeasePaid == false || leaserent.IsOneTimeLeasePaid == null)
                                        //&& (DbFunctions.TruncateTime(leaserent.DuesUptoDate) != DbFunctions.TruncateTime(DateTime.Now))
                                    && (leaserent.DepartmentId == model.DepartmentId)
                                    select leaserent);
                    if (datalist != null)
                    {
                        var today = DateTime.Now;
                        var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                        today = DateTime.Now.AddDays(days - DateTime.Now.Day);
                        today = model.CurrentDuesDate != null ? (DateTime)model.CurrentDuesDate : today;
                        //if (model.CurrentDuesDate != null) { today = (DateTime)model.CurrentDuesDate; }
                        foreach (var list in datalist)
                        {
                            List<Sp_LeaseRentDuesCalculationTillDate_Result> rentList = dbcontext.Sp_LeaseRentDuesCalculationTillDate(list.RegistrationId.ToString(), today, model.DepartmentId).ToList();
                            //var exList = dbcontext.LeaseRentPaymentTrans.Where(r => r.RegistrationId == model.RegistrationId).ToList();
                            //if (exList != null) dbcontext.LeaseRentPaymentTrans.RemoveRange(exList);
                            var plrtlist = dbcontext.PaymentLeaseRentTrans.Where(r => r.RegistrationId == model.RegistrationId).ToList();
                            if (plrtlist != null) dbcontext.PaymentLeaseRentTrans.RemoveRange(plrtlist);
                            Nullable<Decimal> totalDues = 0;
                            for (int i = 0; i < rentList.Count; i++)
                            {
                                var duesrent = rentList.ElementAt(i);
                                totalDues = totalDues + duesrent.totalamount;
                                if (i == rentList.Count - 1)
                                {
                                    //var rent = new LeaseRentPaymentTran();
                                    //rent.RegistrationId = list.RegistrationId;
                                    //rent.RefId = list.Id;
                                    //rent.LeaseRentPremium = duesrent.leaserent;
                                    //rent.DepositDueDate = duesrent.duedate;
                                    //rent.StatusId = NAStatusId.Initiated;
                                    //rent.IsActive = true;
                                    //rent.DuesAmount = duesrent.totalamount;
                                    //rent.DuesUptoDate = duesrent.duedate;
                                    //rent.CreatedDate = DateTime.Now;
                                    //rent.CreatedBy = userInfo.UserID;
                                    //dbcontext.LeaseRentPaymentTrans.Add(rent);
                                    //dbcontext.SaveChanges();

                                    //list.PreviousDues = list.CurrentDues;
                                    //list.PreviousDuesDate = list.CurrentDuesDate;
                                    //list.CurrentDues = totalDues;
                                    //list.CurrentDuesDate = duesrent.duedate;
                                    //dbcontext.SaveChanges();

                                    var rentII = new PaymentLeaseRentTran();
                                    rentII.RegistrationId = list.RegistrationId;
                                    rentII.RentId = list.Id;
                                    rentII.LeaseRentPremium = duesrent.leaserent;
                                    rentII.DepositDueDate = duesrent.duedate;
                                    rentII.StatusId = NAStatusId.Initiated;
                                    rentII.IsActive = true;
                                    rentII.DuesAmount = duesrent.totalamount;
                                    rentII.DuesUptoDate = duesrent.duedate;
                                    rentII.CreatedDate = DateTime.Now;
                                    rentII.CreatedBy = userInfo.UserID.ToString();
                                    dbcontext.PaymentLeaseRentTrans.Add(rentII);
                                    //dbcontext.SaveChanges();

                                    list.PreviousDues = list.CurrentDues;
                                    list.PreviousDuesDate = list.CurrentDuesDate;
                                    list.CurrentDues = duesrent.Total_dues;
                                    list.DuesInterest = duesrent.totalpanelamount;
                                    list.LeaseRentDues = duesrent.totalamount;
                                    list.TotalDuesAmount = duesrent.Total_dues;
                                    list.CurrentDuesDate = duesrent.duedate;
                                    dbcontext.SaveChanges();
                                }
                                else
                                {
                                    //var rent = new LeaseRentPaymentTran();
                                    //rent.RegistrationId = list.RegistrationId;
                                    //rent.RefId = list.Id;
                                    //rent.LeaseRentPremium = duesrent.leaserent;
                                    //rent.DepositDueDate = duesrent.duedate;
                                    //rent.StatusId = NAStatusId.Initiated;
                                    //rent.IsActive = true;
                                    //rent.DuesAmount = duesrent.totalamount;
                                    //rent.DuesUptoDate = duesrent.duedate;
                                    ////rent.HtmlDuesTemplate = duesrent.HtmlDuesReport;
                                    //rent.CreatedDate = DateTime.Now;
                                    //rent.CreatedBy = userInfo.UserID;
                                    //dbcontext.LeaseRentPaymentTrans.Add(rent);
                                    //dbcontext.SaveChanges();

                                    var plrt = new PaymentLeaseRentTran();
                                    plrt.RegistrationId = list.RegistrationId;
                                    plrt.RentId = list.Id;
                                    plrt.LeaseRentPremium = duesrent.leaserent;
                                    plrt.DepositDueDate = duesrent.duedate;
                                    plrt.StatusId = NAStatusId.Initiated;
                                    plrt.IsActive = true;
                                    plrt.DuesAmount = duesrent.totalamount;
                                    plrt.DuesUptoDate = duesrent.duedate;
                                    //rent.HtmlDuesTemplate = duesrent.HtmlDuesReport;
                                    plrt.CreatedDate = DateTime.Now;
                                    plrt.CreatedBy = userInfo.UserID.ToString();
                                    dbcontext.PaymentLeaseRentTrans.Add(plrt);
                                    dbcontext.SaveChanges();
                                }
                                //dbcontext.SaveChanges();
                            }

                            if (rentList.Count == 0)
                            {
                                list.PreviousDues = list.CurrentDues;
                                list.PreviousDuesDate = list.CurrentDuesDate;
                                list.CurrentDues = 0;
                                list.DuesInterest = 0;
                                list.LeaseRentDues = 0;
                                list.TotalDuesAmount = 0;
                                list.CurrentDuesDate = today;
                                dbcontext.SaveChanges();
                            }
                        }
                        flag = ReturnType.Success;
                    }
                }
            }
            return flag;
        }

        public PropertyViewModel GetPropertyDetailById(PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from property in dbContext.SchemePropTrans
                              from alotment in dbContext.AllotmentMasters.Where(a => a.propertyId == property.propertyId).DefaultIfEmpty()
                              from aplicant in dbContext.ApplicationDetails.Where(r => r.registrationId == alotment.rid).DefaultIfEmpty()
                              where property.SectorMst.sectorName == model.SectorName
                              && (model.BlockName == null || property.BlockMst.blockName == model.BlockName)
                              && property.propertyNo == model.PlotNo
                              select new PropertyViewModel
                              {
                                  RegistrationId = alotment.rid
                              }).FirstOrDefault();
                return detail;
            }
        }


        public DataSourceResult GetPaidAmountDetails(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.AllottmentNo == null && model.RegistrationNo == null)
                {
                    List<PaymentViewModel> list = new List<PaymentViewModel>();
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    var list = (from detail in dbContext.RECEIPTBEFORE2009
                                where (model.AllottmentNo == null || detail.ALLOTNO == model.AllottmentNo)
                                && (model.RegistrationNo == null || detail.RID_NO == model.RegistrationNo)

                                select new PaymentViewModel
                                {
                                    //SerialNo = detail.SLNO,
                                    VoucherNo = detail.V_NO,
                                    AllottmentNo = detail.ALLOTNO,
                                    ChallanId = detail.CHALANNO,
                                    Applicant = detail.REF2,
                                    RegistrationNo = detail.RID_NO,
                                    VoucherDate = detail.V_DAT,
                                    Amount = detail.AMT,
                                    SectorName = detail.Sector,
                                    BlockName = detail.Block,
                                    PlotNo = detail.PlotNo
                                });
                    return list.ToDataSourceResult(request);
                }
            }
        }

        public int UpdateRegistrationIdByAllottmentNo(PaymentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var registrionId = dbContext.RECEIPTBEFORE2009.Where(m => m.RID_NO == model.RegistrationNo).FirstOrDefault();
                var details = dbContext.RECEIPTBEFORE2009.Where(m => m.ALLOTNO == model.AllottmentNo).ToList();
                if (registrionId != null)
                {
                    var detail = details.FirstOrDefault();
                    if (detail != null)
                    {
                        if (registrionId.ALLOTNO == detail.ALLOTNO)
                        {
                            if (details != null && details.Count > 0)
                            {
                                string rid = string.Empty;
                                foreach (var data in details)
                                {
                                    if (data.RID_NO != null)
                                    {
                                        rid = data.RID_NO;
                                    }
                                }
                                if (string.IsNullOrEmpty(rid) || rid == model.RegistrationNo)
                                {
                                    details.Each(e => e.RID_NO = model.RegistrationNo);
                                    dbContext.SaveChanges();
                                    flag = ReturnType.Updated;
                                }
                                else
                                {
                                    flag = ReturnType.Mismatch;
                                }
                            }
                        }
                        else
                        {
                            flag = ReturnType.Mismatch;
                        }
                    }
                }
                else
                {
                    if (details != null && details.Count > 0)
                    {
                        string rid = string.Empty;
                        foreach (var data in details)
                        {
                            if (data.RID_NO != null)
                            {
                                rid = data.RID_NO;
                            }
                        }
                        if (string.IsNullOrEmpty(rid) || rid == model.RegistrationNo)
                        {
                            details.Each(e => e.RID_NO = model.RegistrationNo);
                            //foreach (var data in details)
                            //{
                            //    data.RID_NO = model.RegistrationNo;
                            //}
                            dbContext.SaveChanges();
                            flag = ReturnType.Updated;
                        }
                        else
                        {
                            flag = ReturnType.Mismatch;
                        }
                    }
                }
            }
            return flag;
        }


        public DataSourceResult GetInstallmentSchedulePaymentById(DataSourceRequest request, PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.RegistrationId != null)
                {
                    var list = (from detail in dbContext.PaymentScheduleMasters
                                join trans in dbContext.PaymentScheduleTrans on detail.ScheduleId equals trans.ScheduleId
                                from receipt in dbContext.PaymentReceiptMsts.Where(r => r.InstallmentRefId == trans.Id).DefaultIfEmpty()
                                from rtrans in dbContext.PaymentReceiptTrans.Where(r => r.ReceiptRefId == receipt.Id).DefaultIfEmpty()
                                where detail.IsActive == true && detail.Rid == model.RegistrationId && trans.InstallmentDueDate != null
                                select new PaymentScheduleModel
                                {
                                    Id = trans.Id,
                                    ScheduleId = detail.ScheduleId,
                                    RegistrationId = detail.Rid,
                                    PenalInterest = detail.PenalInterest,
                                    InstallmentNo = trans.InstallmentNo,
                                    InstallmentStartDate = trans.InstallmentStartDate,
                                    InstallmentEndDate = trans.InstallmentEndDate,
                                    InstallmentAmount = trans.InstallmentAmount,
                                    InstallmentInterest = trans.InterestAmount,
                                    PenaltyAmount = trans.PenalInterest,
                                    InstallmentDueDate = trans.InstallmentDueDate,
                                    InstallmentPeriod = trans.InstallmentPeriod,
                                    BalanceAmount = trans.BalanceAmount,
                                    IsInstallmentPaid = trans.IsInstallmentPaid,
                                    InstallmentStatus = (trans.IsOneTimeInstallment != null && trans.IsOneTimeInstallment == true) ? "Paid" : ((trans.IsInstallmentPaid == null || trans.IsInstallmentPaid == false) ? "Not Paid" : "Paid"),
                                    IsActive = trans.IsActive,
                                    TotalDueAmount = trans.DuesAmount,
                                    DuesUptoDate = trans.DuesUptoDate,
                                    HtmlTemplate = trans.HtmlDuesTemplate,
                                    CurrentDues = trans.CurrentDues,
                                    CurrentBalance = trans.CurrentBalance,
                                    DepositAmount = rtrans.DepositAmount,
                                    DepositDate = rtrans.DepositDate,
                                    PaymentMode = "Challan",
                                    ChallanRefId = rtrans.ChallanId,
                                    ActionType = rtrans.ChallanId.ToString()
                                });
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    List<PaymentScheduleModel> list = new List<PaymentScheduleModel>();
                    return list.ToDataSourceResult(request);
                }
            }
        }


        public int UpdateInstallmentSchedulePaymentById(DataSourceRequest request, PaymentScheduleModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var payment = dbContext.PaymentScheduleTrans.Where(m => m.Id == model.Id && m.IsActive == true).FirstOrDefault();
                if (payment != null)
                {
                    //payment.InstallmentNo = model.InstallmentNo;
                    //payment.InstallmentDueDate = model.InstallmentDueDate;
                    //payment.InstallmentAmount = model.InstallmentAmount;
                    //payment.BalanceAmount = model.BalanceAmount;
                    //payment.InterestAmount = model.InstallmentInterest;

                    payment.PenalInterest = model.PenaltyAmount;
                    payment.PaymentMode = model.PaymentMode;
                    payment.DepositAmount = model.DepositAmount;
                    payment.DepositDate = model.DepositDate;
                    payment.TransactionId = model.TransactionId;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }

        //public int AddInstallmentPaymentBySchedule(PaymentScheduleModel model)
        //{
        //    var flag = ReturnType.None;
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        int index = 0;
        //        int Totalcount = 0;
        //        int scheduleId = 0;
        //        var paymentList = dbContext.PaymentScheduleTrans.Where(c => c.Rid == model.RegistrationId && c.InstallmentDueDate != null).ToList();
        //        if (paymentList != null)
        //        {
        //            Totalcount = dbContext.PaymentScheduleTrans.Where(c => c.Rid == model.RegistrationId).ToList().Count;
        //            for (int i = 0; i < paymentList.Count; i++)
        //            {
        //                scheduleId = paymentList[0].ScheduleId.Value;
        //                var paymentDate = model.DepositDate.Value.Date;// DbFunctions.TruncateTime(model.DepositDate);
        //                var preDueDate = paymentList[i].InstallmentDueDate.Value.Date;// DbFunctions.TruncateTime(paymentList[i].InstallmentDueDate);
        //                var postDueDate = paymentList.Count == (i + 1) ? null : ((i + 1) > paymentList.Count ? null : paymentList[i + 1].InstallmentDueDate);
        //                if (postDueDate != null)
        //                {
        //                    if (paymentDate < preDueDate)
        //                    {
        //                        index = paymentList[i].Id;
        //                        break;
        //                    }
        //                    if (paymentDate > preDueDate && paymentDate < postDueDate.Value.Date)
        //                    {
        //                        index = paymentList[i + 1].Id;
        //                        break;
        //                    }
        //                }
        //                else
        //                {
        //                    index = paymentList[paymentList.Count - 1].Id; ;
        //                }
        //            }

        //            var payment = dbContext.PaymentScheduleTrans.FirstOrDefault(c => c.Id == index);
        //            var PropertyMst = payment != null ? (from alotment in dbContext.AllotmentMasters join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId select property).FirstOrDefault() : null;
        //            int? InstallmentNo = null;
        //            int? InstallmentRefId = null;
        //            int? ScheduleId = null;
        //            if (payment != null)
        //            {
        //                InstallmentNo = payment.InstallmentNo;
        //                InstallmentRefId = payment.Id;
        //                ScheduleId = payment.ScheduleId;
        //            }
        //            InstallmentPaymentReceiptMst receipt = new InstallmentPaymentReceiptMst();
        //            receipt.RegistrationId = model.RegistrationId;
        //            receipt.DepartmentId = model.DepartmentId;
        //            receipt.SectorId = PropertyMst != null ? PropertyMst.sectorId : model.SectorId;
        //            receipt.BlockId = PropertyMst != null ? PropertyMst.blockId : model.BlockId;
        //            receipt.PlotNo = PropertyMst != null ? PropertyMst.propertyNo : model.PlotNo;
        //            receipt.ScheduleId = ScheduleId;
        //            receipt.InstallmentNo = InstallmentNo;// payment.InstallmentNo;
        //            receipt.InstallmentRefId = InstallmentRefId;// payment.Id; // use for ref installment
        //            receipt.ChallanId = Convert.ToInt64(model.TransactionId);
        //            receipt.DepositAmount = model.DepositAmount;
        //            receipt.DepositDate = model.DepositDate;
        //            receipt.BankId = model.BankId;
        //            receipt.ReceiptHeadId = model.ReceiptHeadId;
        //            receipt.ReceiptSubHeadId = model.ReceiptSubHeadId;
        //            receipt.StatusId = NAStatusId.Accepted;
        //            receipt.IsActive = true;
        //            receipt.CreatedBy = userInfo.UserID;
        //            receipt.CreatedDate = DateTime.Now;
        //            dbContext.InstallmentPaymentReceiptMsts.Add(receipt);
        //            dbContext.SaveChanges();
        //            flag = ReturnType.Updated;
        //        }
        //    }
        //    return flag;
        //}

        public int AddInstallmentPaymentBySchedule(PaymentScheduleModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                int index = 0;
                int Totalcount = 0;
                int scheduleId = 0;
                var paymentList = dbContext.PaymentScheduleTrans.Where(c => c.Rid == model.RegistrationId && c.InstallmentDueDate != null).ToList();
                if (paymentList != null)
                {
                    Totalcount = dbContext.PaymentScheduleTrans.Where(c => c.Rid == model.RegistrationId).ToList().Count;
                    for (int i = 0; i < paymentList.Count; i++)
                    {
                        scheduleId = paymentList[0].ScheduleId.Value;
                        var paymentDate = model.DepositDate.Value.Date;// DbFunctions.TruncateTime(model.DepositDate);
                        var preDueDate = paymentList[i].InstallmentDueDate.Value.Date;// DbFunctions.TruncateTime(paymentList[i].InstallmentDueDate);
                        var postDueDate = paymentList.Count == (i + 1) ? null : ((i + 1) > paymentList.Count ? null : paymentList[i + 1].InstallmentDueDate);
                        if (postDueDate != null)
                        {
                            if (paymentDate < preDueDate)
                            {
                                index = paymentList[i].Id;
                                break;
                            }
                            if (paymentDate > preDueDate && paymentDate < postDueDate.Value.Date)
                            {
                                index = paymentList[i + 1].Id;
                                break;
                            }
                        }
                        else
                        {
                            index = paymentList[paymentList.Count - 1].Id; ;
                        }
                    }

                    var payment = dbContext.PaymentScheduleTrans.FirstOrDefault(c => c.Id == index);
                    var PropertyMst = payment != null ? (from alotment in dbContext.AllotmentMasters join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId select property).FirstOrDefault() : null;
                    int? InstallmentNo = null;
                    int? InstallmentRefId = null;
                    int? ScheduleId = null;
                    if (payment != null)
                    {
                        InstallmentNo = payment.InstallmentNo;
                        InstallmentRefId = payment.Id;
                        ScheduleId = payment.ScheduleId;
                    }
                    var tempchallan = (List<PaymentScheduleModel>)HttpContext.Current.Session["TempInstallment"];
                    if (tempchallan != null)
                    {
                        PaymentReceiptMst receipt = new PaymentReceiptMst();
                        receipt.RegistrationId = model.RegistrationId;
                        receipt.DepartmentId = model.DepartmentId;
                        receipt.SectorId = PropertyMst != null ? PropertyMst.sectorId : model.SectorId;
                        receipt.BlockId = PropertyMst != null ? PropertyMst.blockId : model.BlockId;
                        receipt.PlotNo = PropertyMst != null ? PropertyMst.propertyNo : model.PlotNo;
                        receipt.ScheduleId = ScheduleId;
                        receipt.InstallmentNo = InstallmentNo;// payment.InstallmentNo;
                        receipt.InstallmentRefId = InstallmentRefId;// payment.Id; // use for ref installment
                        receipt.ChallanId = Convert.ToInt64(model.TransactionId);
                        receipt.DepositAmount = model.DepositAmount;
                        receipt.DepositDate = model.DepositDate;
                        receipt.BankId = model.BankId;
                        //receipt.ReceiptHeadId = model.ReceiptHeadId;
                        //receipt.ReceiptSubHeadId = model.ReceiptSubHeadId;
                        receipt.StatusId = NAStatusId.Accepted;
                        receipt.IsActive = true;
                        receipt.CreatedBy = userInfo.UserID;
                        receipt.CreatedDate = DateTime.Now;
                        dbContext.PaymentReceiptMsts.Add(receipt);
                        dbContext.SaveChanges();

                        foreach (var challan in tempchallan)
                        {
                            PaymentReceiptTran trans = new PaymentReceiptTran();
                            trans.ReceiptRefId = receipt.Id;
                            trans.ReceiptHeadId = challan.ReceiptHeadId;
                            trans.ReceiptSubHeadId = challan.ReceiptSubHeadId;
                            trans.DepositAmount = challan.DepositAmount;
                            trans.DepositDate = model.DepositDate;
                            trans.ChallanId = receipt.ChallanId;
                            trans.StatusId = NAStatusId.Accepted;
                            trans.IsActive = true;
                            trans.CreatedBy = userInfo.UserID;
                            trans.CreatedDate = DateTime.Now;
                            dbContext.PaymentReceiptTrans.Add(trans);
                            dbContext.SaveChanges();
                        }
                        HttpContext.Current.Session["TempInstallment"] = null;
                        flag = ReturnType.Updated;
                    }
                }
            }
            return flag;
        }

        public int SaveTempChallanDetail(PaymentScheduleModel model)
        {
            int flag = ReturnType.None;
            var dbContext = new NoidaPMSEntities();
            var tempchallan = (List<PaymentScheduleModel>)HttpContext.Current.Session["TempInstallment"];
            if (tempchallan != null)
            {
                var rid = tempchallan.FirstOrDefault().RegistrationId;
                if (rid == model.RegistrationId)
                {
                    PaymentScheduleModel challan = new PaymentScheduleModel();
                    challan.RefId = tempchallan.ToList().Count + 1;
                    challan.ReceiptHeadId = model.ReceiptHeadId;
                    challan.ReceiptHead = model.ReceiptHead; //dbContext.RECIEPT_HEAD.FirstOrDefault(r => r.RECIEPT_CODE == model.ReceiptHeadId).RECIEPT_HEAD_NAME;
                    challan.ReceiptSubHeadId = model.ReceiptSubHeadId;
                    challan.ReceiptSubHead = model.ReceiptSubHead;// dbContext.RECEIPT_SUB_HEAD.FirstOrDefault(r => r.RECEIPT_SUBHEAD_ID == model.ReceiptSubHeadId).RECEIPT_SUB_HEAD1;
                    challan.ScheduleId = model.ScheduleId;
                    challan.RegistrationId = model.RegistrationId;
                    challan.DepositAmount = model.DepositAmount;
                    tempchallan.Add(challan);
                    HttpContext.Current.Session["TempInstallment"] = tempchallan;
                    flag = ReturnType.Saved;
                }
                else
                {
                    flag = ReturnType.Mismatch;
                }
            }
            else
            {
                List<PaymentScheduleModel> challanList = new List<PaymentScheduleModel>();
                PaymentScheduleModel challan = new PaymentScheduleModel();
                challan.RefId = 1;
                challan.ReceiptHeadId = model.ReceiptHeadId;
                challan.ReceiptHead = model.ReceiptHead; //dbContext.RECIEPT_HEAD.FirstOrDefault(r => r.RECIEPT_CODE == model.ReceiptHeadId).RECIEPT_HEAD_NAME;
                challan.ReceiptSubHeadId = model.ReceiptSubHeadId;
                challan.ReceiptSubHead = model.ReceiptSubHead; //dbContext.RECEIPT_SUB_HEAD.FirstOrDefault(r => r.RECEIPT_SUBHEAD_ID == model.ReceiptSubHeadId).RECEIPT_SUB_HEAD1;
                challan.ScheduleId = model.ScheduleId;
                challan.RegistrationId = model.RegistrationId;
                challan.DepositAmount = model.DepositAmount;
                challanList.Add(challan);
                HttpContext.Current.Session["TempInstallment"] = challanList;
                flag = ReturnType.Saved;
            }
            return flag;
        }

        public DataSourceResult GetSavedTempChallanDetail(DataSourceRequest request, PaymentScheduleModel model)
        {
            var tempchallan = (List<PaymentScheduleModel>)HttpContext.Current.Session["TempInstallment"];
            if (tempchallan != null)
            {
                List<PaymentScheduleModel> list = new List<PaymentScheduleModel>();
                for (int i = 0; i < tempchallan.Count; i++)
                {
                    PaymentScheduleModel challan = new PaymentScheduleModel();
                    challan.RefId = i + 1;
                    challan.ReceiptHeadId = tempchallan[i].ReceiptHeadId;
                    challan.ReceiptHead = tempchallan[i].ReceiptHead;
                    challan.ReceiptSubHeadId = tempchallan[i].ReceiptSubHeadId;
                    challan.ReceiptSubHead = tempchallan[i].ReceiptSubHead;
                    challan.ScheduleId = tempchallan[i].ScheduleId;
                    challan.RegistrationId = tempchallan[i].RegistrationId;
                    challan.DepositAmount = tempchallan[i].DepositAmount;
                    list.Add(challan);
                }
                //return tempchallan.ToDataSourceResult(request);
                return list.ToDataSourceResult(request);
            }
            else
            {
                return null;
            }
        }


        public int RemoveTempChallanDataById(PaymentScheduleModel model)
        {
            int flag = ReturnType.None;
            var tempchallan = (List<PaymentScheduleModel>)HttpContext.Current.Session["TempInstallment"];
            if (tempchallan != null)
            {
                tempchallan.RemoveAt(model.RefId.Value - 1);
                flag = ReturnType.Removed;
            }
            return flag;
        }


        public PaymentScheduleModel UpdatePaymentDuesById(PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<PaymentViewModel> payList = CalculateInstallmentDuesIV(new PaymentViewModel { RegistrationId = model.RegistrationId });
            }

            model.ReturnTypeId = ReturnType.Updated;
            return model;
        }


        public DataSourceResult GetLeaseRentPaymentTransDetailsById(DataSourceRequest request, LeaseRentViewModel model)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var list = (from lease in dbcontext.LeaseRentPayments
                            join leaserent in dbcontext.PaymentLeaseRentTrans on lease.Id equals leaserent.RentId
                            join alotment in dbcontext.AllotmentMasters on lease.RegistrationId equals alotment.rid
                            join property in dbcontext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where lease.Id == model.Id && lease.IsActive == true
                            select new LeaseRentViewModel
                            {
                                Id = leaserent.Id,
                                RentId = leaserent.RentId,
                                RegistrationId = leaserent.RegistrationId,
                                LeaseRentPremium = leaserent.LeaseRentPremium,
                                DepositDate = leaserent.DepositDueDate,
                                Sector = null
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public PaymentViewModel CalculateInstallmentDuesById(PaymentViewModel model)
        {
            PaymentViewModel paymentviewmodel = new PaymentViewModel();
            using (var dbcontext = new NoidaPMSEntities())
            {
                var propertyTypeId = dbcontext.AllotmentMasters.FirstOrDefault(a => a.rid == model.RegistrationId).departmentId;
                model.PropertyTypeId = propertyTypeId;
                var InstallmentDuesList = dbcontext.Sp_DuesCalculationHistory(model.RegistrationId.ToString(), model.ActionDate, model.PropertyTypeId).ToList();

                var duesList = (from dues in dbcontext.temp_cal
                                where dues.rid == model.RegistrationId
                                select new PaymentViewModel
                                {
                                    DuesAmount = dues.DuesTill,
                                    TotalDuesAmount = dues.DuesNextWithInterest,
                                    PenalInterest = dues.TotalPanelAmount,
                                    PrincipalAmount = dues.TotalPrincipalAmount,
                                    DuesUptoDate = model.ActionDate // dues.InstallmentDueDate == null ? DateTime.Now : dues.InstallmentDueDate,
                                }).ToList();
                var Installment = new PaymentViewModel();
                if (duesList != null && duesList.Count > 0)
                {
                    var dues = duesList[duesList.Count - 1];
                    Installment = new PaymentViewModel
                    {
                        DuesAmount = dues.DuesAmount,
                        TotalDuesAmount = (dues.TotalDuesAmount < 0 || dues.TotalDuesAmount == null) ? 0 : dues.TotalDuesAmount,
                        PenalInterest = dues.PenalInterest,
                        PrincipalAmount = dues.PrincipalAmount,
                        DuesUptoDate = dues.DuesUptoDate == null ? DateTime.Now : dues.DuesUptoDate,
                    };
                    paymentviewmodel = Installment;
                }
                else
                {
                    Installment = new PaymentViewModel
                    {
                        PremiumPaidStatus = "Full Paid",
                        IsInstallmentPaid = true
                    };
                    paymentviewmodel = Installment;
                }

                var lease = UpdateLeaseRentDuesPaymentById(new LeaseRentViewModel { RegistrationId = model.RegistrationId, CurrentDuesDate = model.CurrentDuesDate, DepartmentId = model.DepartmentId, ActionType = model.ActionType });
                if (lease != null)
                {
                    var leaserent = dbcontext.LeaseRentPayments.OrderByDescending(r => r.Id).Where(r => r.RegistrationId == model.RegistrationId).FirstOrDefault();
                    if (leaserent != null)
                    {
                        Installment.CurrentDues = (leaserent.CurrentDues < 0 || leaserent.CurrentDues == null) ? 0 : leaserent.CurrentDues;
                        Installment.CurrentDuesDate = leaserent.CurrentDuesDate;
                        Installment.LeaseRentStatus = leaserent.IsOneTimeLeasePaid == true ? "One Time Paid" : string.Empty;
                        Installment.IsOneTimeLeaseRentPaid = leaserent.IsOneTimeLeasePaid == true ? true : false;
                    }
                }
                //else
                //{
                //    var leaserent = dbcontext.LeaseRentPayments.OrderByDescending(r => r.Id).Where(r => r.RegistrationId == model.RegistrationId).FirstOrDefault();
                //    Installment.LeaseRentStatus = leaserent.IsOneTimeLeasePaid == true ? "One Time Paid" : string.Empty;
                //    Installment.IsOneTimeLeaseRentPaid = true;
                //}
            }
            return paymentviewmodel;
        }


        public int SaveInstallmentScheduleTransByDate(PaymentScheduleModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                int index = 0;
                int Totalcount = 0;
                int scheduleId = 0;
                List<PaymentScheduleTran> paymentList = new List<PaymentScheduleTran>();
                if (model.ScheduleType != null && model.ScheduleType != "")
                {
                    paymentList = dbContext.PaymentScheduleTrans.Where(c => c.Rid == model.RegistrationId && c.InstallmentDueDate != null && c.ScheduleType == model.ScheduleType).ToList();
                }
                else
                {
                    paymentList = dbContext.PaymentScheduleTrans.Where(c => c.Rid == model.RegistrationId && c.InstallmentDueDate != null).ToList();
                }

                if (paymentList != null && paymentList.Count > 0)
                {
                    //Totalcount = dbContext.PaymentScheduleTrans.Where(c => c.Rid == model.RegistrationId).ToList().Count;
                    Totalcount = paymentList.Count;
                    for (int i = 0; i < paymentList.Count; i++)
                    {
                        scheduleId = paymentList[0].ScheduleId.Value;
                        var dueDate = model.InstallmentDueDate.Value.Date;
                        var preDueDate = paymentList[i].InstallmentDueDate.Value.Date;
                        var postDueDate = paymentList.Count == (i + 1) ? null : ((i + 1) > paymentList.Count ? null : paymentList[i + 1].InstallmentDueDate);
                        if (postDueDate != null)
                        {
                            if (dueDate < preDueDate)
                            {
                                index = paymentList[i].Id;
                                break;
                            }
                            if (dueDate > preDueDate && dueDate < postDueDate.Value.Date)
                            {
                                index = paymentList[i + 1].Id;
                                break;
                            }
                        }
                        else
                        {
                            index = paymentList[paymentList.Count - 1].Id; ;
                        }
                    }

                    var payment = dbContext.PaymentScheduleTrans.FirstOrDefault(c => c.Id == index);
                    //var PropertyMst = payment != null ? (from alotment in dbContext.AllotmentMasters join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId select property).FirstOrDefault() : null;
                    int? InstallmentNo = null;
                    int? InstallmentRefId = null;
                    int? ScheduleId = null;
                    if (payment != null)
                    {
                        InstallmentNo = payment.InstallmentNo;
                        InstallmentRefId = payment.Id;
                        ScheduleId = payment.ScheduleId;

                        PaymentScheduleTran trans = new PaymentScheduleTran();
                        trans.ScheduleId = ScheduleId;
                        trans.Rid = model.RegistrationId;
                        trans.InstallmentNo = InstallmentNo;
                        trans.InstallmentAmount = model.InstallmentAmount;
                        trans.InterestAmount = model.InstallmentInterest;
                        trans.InstallmentStartDate = model.InstallmentStartDate;
                        trans.InstallmentEndDate = model.InstallmentEndDate;
                        trans.InstallmentDueDate = model.InstallmentDueDate;
                        trans.PenalInterest = model.PenalInterest;
                        trans.BalanceAmount = model.BalanceAmount;
                        trans.ScheduleType = "I";
                        trans.IsActive = true;
                        trans.CreatedDate = DateTime.Now;
                        trans.CreatedBy = userInfo.UserID.ToString();
                        dbContext.PaymentScheduleTrans.Add(trans);
                        dbContext.SaveChanges();

                        //var postTransList = dbContext.PaymentScheduleTrans.Where(r => r.Rid == model.RegistrationId && r.ScheduleType == payment.ScheduleType && DbFunctions.TruncateTime(r.InstallmentDueDate) > DbFunctions.TruncateTime(model.InstallmentDueDate)).ToList();
                        var postTransList = dbContext.PaymentScheduleTrans.Where(r => r.Rid == model.RegistrationId && r.ScheduleType == payment.ScheduleType).ToList().OrderBy(o => o.InstallmentDueDate);
                        if (postTransList != null)
                        {
                            //for (int i = 0; i < postTransList.Count; i++)
                            //{
                            //    var INo = postTransList[i].InstallmentNo;
                            //    var preINo = i==0 ? -1 : postTransList[i-1].InstallmentNo;
                            //    var postINo = i== postTransList.Count - 1 ? postTransList.Count : postTransList[i+1].InstallmentNo;
                            //    if (i == 0)
                            //    {
                            //        postTransList[i].InstallmentNo = INo + 1;
                            //    }
                            //    else
                            //    {
                            //        if (postINo == INo + 1)
                            //        {
                            //            postTransList[i].InstallmentNo = INo + 1;
                            //        }
                            //        else
                            //        {
                            //            postTransList[i].InstallmentNo = preINo + 1;
                            //        }
                            //    }                               
                            //}
                            int i = 1;
                            foreach (var installment in postTransList)
                            {
                                //installment.InstallmentNo = installment.InstallmentNo + 1;
                                installment.InstallmentNo = i;
                                i = i + 1;
                            }
                            dbContext.SaveChanges();
                            flag = ReturnType.Updated;
                        }
                    }

                }
            }
            return flag;
        }


        public int IsPaymentExistForRid(PaymentViewModel model)
        {
            int flag = ReturnType.NotExist;
            using (var dbContext = new NoidaPMSEntities())
            {
                var isRidExist = dbContext.RECEIPT_DETAIL_MASTER.FirstOrDefault(m => m.RID_NO == model.RegistrationNo);
                if (isRidExist != null)
                {
                    flag = ReturnType.Exist;
                }
                else
                {
                    flag = ReturnType.NotExist;
                }
                return flag;
            }
        }


        public int IsPaymentExistForProperty(PaymentViewModel model)
        {
            int flag = ReturnType.NotExist;
            using (var dbContext = new NoidaPMSEntities())
            {
                var isExist = dbContext.RECEIPT_DETAIL_MASTER.FirstOrDefault(m => m.PROPERTY_NUMBER == model.PropertyNo);
                if (isExist != null)
                {
                    flag = ReturnType.Exist;
                }
                else
                {
                    flag = ReturnType.NotExist;
                }
                return flag;
            }
        }


        public DataSourceResult GetReScheduledInstallmentAsDataSource(DataSourceRequest request, PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from pay in dbContext.PaymentScheduleMasters
                            join payTrans in dbContext.PaymentScheduleTrans on pay.ScheduleId equals payTrans.ScheduleId
                            where pay.Rid == model.RegistrationId && pay.ScheduleType == model.ScheduleType
                            select new PaymentScheduleModel
                            {
                                Id = payTrans.Id,
                                RegistrationId = payTrans.Rid,
                                InstallmentNo = payTrans.InstallmentNo,
                                InstallmentDueDate = payTrans.InstallmentDueDate,
                                InstallmentAmount = payTrans.InstallmentAmount,
                                BalanceAmount = payTrans.BalanceAmount,
                                InstallmentInterest = payTrans.InterestAmount,
                                TotalBalanceAmount = payTrans.InstallmentAmount + payTrans.InterestAmount,
                                ScheduleType = pay.ScheduleType
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public PaymentScheduleModel GetPropertyPaymentReScheduleInformationById(PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from alotment in dbContext.AllotmentMasters
                              join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId.Value
                              join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                              join proptype in dbContext.PropertyTypeMsts on property.propertyTypeId equals proptype.propertyTypeId
                              where alotment.rid == model.RegistrationId
                              select new PaymentScheduleModel
                              {
                                  Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                  ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                  ApplicantType = aplicant.tGender,
                                  DepartmentId = alotment.departmentId,
                                  Department = alotment.DepartmentMst.departmentName,
                                  PropertyNo = property.SectorMst.sectorName + "/" + ((property.blockId == null || property.blockId <= 0) ? string.Empty : property.BlockMst.blockName + "-") + property.propertyNo,
                                  PropertyType = proptype.propertyTypeName,
                                  AllotmentDate = alotment.allotmentDate,
                                  PropertyId = alotment.propertyId
                              }).FirstOrDefault();
                //var schedulelist = dbContext.PaymentScheduleMasters.Where(r => r.Rid == model.RegistrationId && r.ScheduleType != "I").ToList().OrderByDescending(o=>o.ScheduleId);
                var schedule = dbContext.PaymentScheduleMasters.FirstOrDefault(p => p.Rid == model.RegistrationId && p.ScheduleType == model.ScheduleType);
                //var schedule = schedulelist.FirstOrDefault();
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
                //var installment = (from pay in dbContext.PaymentScheduleTrans where pay.Rid == model.RegistrationId && pay.IsActive == true && pay.ScheduleType!="I" select pay).OrderByDescending(s => s.Id).FirstOrDefault();
                //var installment = dbContext.PaymentScheduleTrans.FirstOrDefault(t => t.Rid == Rid && t.IsActive == true);
                //if (installment != null)
                //{
                //    detail.PremiumAmount = installment.InstallmentAmount;
                //    detail.TotalBalanceAmount = installment.BalanceAmount;
                //    detail.BalanceAmount = installment.BalanceAmount;
                //    detail.PreInstallmentNo = installment.InstallmentNo;
                //}
                return detail;
            }
        }


        public DataSourceResult GetInstallmentScheduleTypeAsDataSource(DataSourceRequest request, PaymentScheduleModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var scheduleType = dbContext.PaymentScheduleMasters.Where(r => r.Rid == model.RegistrationId).Select(s => s.ScheduleType).ToList();
                var list = (from schedule in dbContext.PaymentScheduleMasters
                            where schedule.Rid == model.RegistrationId
                            select new DropdownViewModel
                            {
                                Text = schedule.ScheduleType,
                                Value = schedule.ScheduleType
                            }).ToList();
                if (request.Filters.Count > 0)
                {
                    request.Filters.RemoveAt(0);
                }

                if (list != null) return list.ToDataSourceResult(request);
                else return null;
            }
        }


        public PropertyViewModel GetPropertyCostDetailAsPerScheme(PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from alotment in dbContext.AllotmentMasters
                              join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                              join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                              from cost in dbContext.SchemeCostTrans.Where(x => x.schemeId == property.schemeId && x.departmentId == property.departmentId && x.propertyTypeId == property.propertyTypeId && x.sectorId == property.sectorId && x.blockId == property.blockId && x.floorId == property.floorId).DefaultIfEmpty()
                              from loccharg in dbContext.LocationChargeDetailMsts.Where(l => l.PropertyId == property.propertyId).DefaultIfEmpty()
                              where alotment.rid == model.RegistrationId
                              select new PropertyViewModel
                              {
                                  RegistrationId = alotment.rid,
                                  PropertyId = property.propertyId,
                                  DepartmentId = property.departmentId,
                                  Department = property.DepartmentMst.departmentName,
                                  SectorId = property.sectorId,
                                  SectorName = property.SectorMst.sectorName,
                                  BlockId = property.blockId,
                                  BlockName = property.BlockMst.blockName,
                                  PlotNo = property.propertyNo,
                                  PropertyNo = property.SectorMst.sectorName + "/" + (property.blockId == null ? string.Empty : property.BlockMst.blockName + "-") + property.propertyNo,
                                  PropertyTypeId = property.propertyTypeId,
                                  PropertyType = property.PropertyTypeMst.propertyTypeName,
                                  FloorAreaId = property.floorId,
                                  FloorArea = property.FloorMst.floorName,
                                  TotalArea = property.totalArea,
                                  ActualArea = property.actualArea,
                                  CoveredArea = property.coveredArea,
                                  TotalAllotmentRate = (property.TotalAllotmentRate == null || property.TotalAllotmentRate <= 0) ? property.landRatePerSqmt : property.TotalAllotmentRate,
                                  LandRate = property.landRatePerSqmt != null ? property.landRatePerSqmt : ((cost != null && cost.landRatePerSqmt != null) ? cost.landRatePerSqmt : null),
                                  AllotmentMoney = property.allotmentMoney != null ? property.allotmentMoney : ((cost != null && cost.allotmentMoney != null) ? cost.allotmentMoney : null),
                                  EarnestMoney = property.EarnestMoney != null ? property.EarnestMoney : ((cost != null && cost.earnestMoney != null) ? cost.earnestMoney : null),
                                  PropertyCost = property.propertyCost != null ? property.propertyCost : ((cost != null && cost.propertyCost != null) ? cost.propertyCost : null),
                                  TotalPropertyCost = property.totalPropertyCost != null ? property.totalPropertyCost : ((cost != null && cost.totalPropertyCost != null) ? cost.totalPropertyCost : null),
                                  CivilCost = property.civilCost != null ? property.civilCost : ((cost != null && cost.civilCost != null) ? cost.civilCost : null),
                                  LeaseRent = property.LeaseRent != null ? property.LeaseRent : ((cost != null && cost.leaseRent != null) ? cost.leaseRent : null),
                                  ExcessAreaRefId = property.ExcessAreaRefId,
                                  AdvanceLeaseRent = property.AdvanceLeaseRent,
                                  LocationId = loccharg.LocationId,
                                  Location = loccharg.LocationMst.locationName,
                                  LocationCharge = loccharg.LocationCharge,
                                  LocationChargeRate = loccharg.LocationChargeRate,
                                  Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                  ApplicantAddress = aplicant.tCorrespondanceAdd,
                                  AllotmentDate = alotment.allotmentDate,
                                  RegistryType = property.Registry == "L" ? "Lease" : (property.Registry == "Leasehold" ? "Lease" : (property.Registry == "Lease" ? "Lease" : null))
                              }).FirstOrDefault();
                if (detail != null)
                {
                    var excessArea = (from excessAreaDetail in dbContext.ExcessAreaDetailMsts.Where(c => c.PropertyId == detail.PropertyId).DefaultIfEmpty() select excessAreaDetail).FirstOrDefault();
                    if (excessArea != null)
                    {
                        detail.AllottedArea = excessArea.AllottedArea;
                        detail.ExcessArea = excessArea.ExcessArea;
                        detail.TotalArea = excessArea.TotalArea;
                        detail.ExcessAreaDate = excessArea.ExcessAreaDate;
                        detail.ExcessAreaValidDate = excessArea.ExcessAreaValidDate;
                        detail.ExcessAreaCost = excessArea.TotalExcessPremium;
                        detail.TotalExcessAreaCost = excessArea.TotalExcessAreaCost;
                        detail.LandRate = excessArea.ExcessAreaAllotmentRate;
                        detail.ExcessAreaLocationCharge = excessArea.LocationCharge;
                        detail.LocationCharge = excessArea.LocationCharge;
                        detail.OneTimeLeaseRent = excessArea.OneTimeLeaseRent;
                        detail.IsExcessArea = true;
                    }

                    var compensation = dbContext.PropertyCompensationMsts.Where(c => c.PropertyId == detail.PropertyId).ToList().OrderByDescending(o => o.Id).FirstOrDefault();
                    detail.IsCompensation = compensation != null ? true : false;

                }

                return detail;
            }
        }

        public int SavePropertyCostAsPerScheme(PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                var property = dbContext.SchemePropTrans.FirstOrDefault(x => x.propertyId == model.PropertyId);
                if (property != null)
                {
                    property.totalArea = model.TotalArea == null ? property.totalArea : model.TotalArea;
                    property.actualArea = model.ActualArea == null ? property.actualArea : model.ActualArea;
                    property.coveredArea = model.CoveredArea == null ? property.coveredArea : model.CoveredArea;
                    property.landRatePerSqmt = model.AllotmentRate;
                    property.TotalAllotmentRate = model.TotalAllotmentRate;
                    property.allotmentMoney = model.AllotmentMoney;
                    property.EarnestMoney = model.EarnestMoney;
                    property.Processingfee = model.ProcessingFee;
                    property.propertyCost = model.PropertyCost;
                    property.totalPropertyCost = model.TotalPropertyCost;
                    property.civilCost = model.CivilCost == null ? property.civilCost : model.CivilCost;
                    property.LeaseRent = model.LeaseRent;
                    property.AdvanceLeaseRent = model.AdvanceLeaseRent;
                    property.LocationId = model.LocationId == null ? null : model.LocationId;
                    if (model.LocationId != null)
                    {
                        var plc = dbContext.LocationChargeDetailMsts.FirstOrDefault(l => l.PropertyId == model.PropertyId);
                        if (plc != null)
                        {
                            plc.PropertyId = model.PropertyId;
                            plc.LocationId = model.LocationId;
                            plc.LocationCharge = model.LocationCharge;
                            plc.LocationChargeRate = model.LocationChargeRate;
                            plc.IsActive = true;
                            plc.ModifiedBy = userInfo.UserID;
                            plc.ModifiedDate = DateTime.Now;
                        }
                        else
                        {
                            LocationChargeDetailMst location = new LocationChargeDetailMst();
                            location.PropertyId = model.PropertyId;
                            location.LocationId = model.LocationId;
                            location.LocationCharge = model.LocationCharge;
                            location.LocationChargeRate = model.LocationChargeRate;
                            location.IsActive = true;
                            location.CreatedBy = userInfo.UserID;
                            location.CreatedDate = DateTime.Now;
                            dbContext.LocationChargeDetailMsts.Add(location);
                        }
                    }
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                return flag;
            }
        }


        public DataSourceResult GetPropertyCostDetailListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from alotment in dbContext.AllotmentMasters
                              join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                              join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                              from cost in dbContext.SchemeCostTrans.Where(x => x.schemeId == property.schemeId && x.departmentId == property.departmentId && x.propertyTypeId == property.propertyTypeId && x.sectorId == property.sectorId && x.blockId == property.blockId && x.floorId == property.floorId).DefaultIfEmpty()
                              from loccharg in dbContext.LocationChargeDetailMsts.Where(l => l.PropertyId == property.propertyId).DefaultIfEmpty()
                              where DepartmentList.Contains(property.departmentId.Value)
                              && (model.Id == null || property.refId == model.Id)
                              && (model.RegistrationId == null || alotment.rid == model.RegistrationId)
                              && (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                              && (model.SectorId == null || property.sectorId == model.SectorId)
                              && (model.BlockId == null || property.blockId == model.BlockId)
                              && alotment.isActive == 1
                              select new PropertyViewModel
                              {
                                  Id = property.refId,
                                  RegistrationId = alotment.rid,
                                  PropertyId = property.propertyId,
                                  DepartmentId = property.departmentId,
                                  Department = property.DepartmentMst.departmentName,
                                  SchemeId = property.schemeId,
                                  SchemeName = property.SchemeMst.schemeName,
                                  SectorId = property.sectorId,
                                  SectorName = property.SectorMst.sectorName,
                                  BlockId = property.blockId,
                                  BlockName = property.BlockMst.blockName,
                                  PlotNo = property.propertyNo,
                                  PropertyNo = property.SectorMst.sectorName + "/" + (property.blockId == null ? string.Empty : property.BlockMst.blockName + "-") + property.propertyNo,
                                  PropertyTypeId = property.propertyTypeId,
                                  PropertyType = property.PropertyTypeMst.propertyTypeName,
                                  FloorAreaId = property.floorId,
                                  FloorArea = property.FloorMst.floorName,
                                  AllottedArea = property.AllottedArea,
                                  TotalArea = property.totalArea,
                                  ActualArea = property.actualArea,
                                  CoveredArea = property.coveredArea,
                                  AllotmentRate = property.landRatePerSqmt,
                                  TotalAllotmentRate = property.TotalAllotmentRate,
                                  AllotmentDate = alotment.allotmentDate,
                                  LandRate = property.landRatePerSqmt != null ? property.landRatePerSqmt : ((cost != null && cost.landRatePerSqmt != null) ? cost.landRatePerSqmt : null),
                                  AllotmentMoney = property.allotmentMoney != null ? property.allotmentMoney : ((cost != null && cost.allotmentMoney != null) ? cost.allotmentMoney : null),
                                  EarnestMoney = property.EarnestMoney != null ? property.EarnestMoney : ((cost != null && cost.earnestMoney != null) ? cost.earnestMoney : null),
                                  PropertyCost = property.propertyCost != null ? property.propertyCost : ((cost != null && cost.propertyCost != null) ? cost.propertyCost : null),
                                  TotalPropertyCost = property.totalPropertyCost != null ? property.totalPropertyCost : ((cost != null && cost.totalPropertyCost != null) ? cost.totalPropertyCost : null),
                                  CivilCost = property.civilCost != null ? property.civilCost : ((cost != null && cost.civilCost != null) ? cost.civilCost : null),
                                  LeaseRent = property.LeaseRent != null ? property.LeaseRent : ((cost != null && cost.leaseRent != null) ? cost.leaseRent : null),
                                  AdvanceLeaseRent = property.AdvanceLeaseRent,
                                  LocationId = loccharg.LocationId,
                                  Location = loccharg.LocationMst.locationName,
                                  LocationChargeRate = loccharg.LocationChargeRate,
                                  LocationCharge = loccharg.LocationCharge,
                                  Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                  ApplicantAddress = aplicant.tCorrespondanceAdd
                              });
                return detail.ToDataSourceResult(request);
            }

        }


        public int SaveExcessAreaDetail(PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                int excessRefId = 0;
                var exExcessArea = dbContext.ExcessAreaDetailMsts.FirstOrDefault(e => e.PropertyId == model.PropertyId);
                if (exExcessArea != null)
                {
                    excessRefId = exExcessArea.Id;
                    exExcessArea.ExcessAreaDate = model.ExcessAreaDate;
                    exExcessArea.AllottedArea = model.AllottedArea;
                    exExcessArea.ExcessArea = model.ExcessArea;
                    exExcessArea.TotalArea = model.TotalArea;
                    exExcessArea.ExcessAreaAllotmentRate = model.ExcessAreaAllotmentRate;
                    exExcessArea.LocationCharge = model.ExcessAreaLocationCharge;
                    exExcessArea.ExcessCharge = model.ExcessAreaCharge;
                    exExcessArea.TotalExcessPremium = model.TotalPropertyCost;
                    exExcessArea.OneTimeLeaseRent = model.OneTimeLeaseRent;
                    exExcessArea.TotalExcessAreaCost = model.TotalExcessAreaCost;
                    exExcessArea.ExcessAreaValidDate = model.ExcessAreaValidDate;
                    exExcessArea.PenalInterest = model.PenalInterest;
                    exExcessArea.PenalAmount = model.PenalInterestAmount;
                    exExcessArea.TotalPayableAmount = model.TotalPayableAmount;
                    exExcessArea.IsActive = true;
                    exExcessArea.ModifiedBy = userInfo.UserID;
                    exExcessArea.ModifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                else
                {
                    ExcessAreaDetailMst excessArea = new ExcessAreaDetailMst();
                    excessArea.PropertyId = model.PropertyId;
                    excessArea.ExcessAreaDate = model.ExcessAreaDate;
                    excessArea.AllottedArea = model.AllottedArea;
                    excessArea.ExcessArea = model.ExcessArea;
                    excessArea.TotalArea = model.TotalArea;
                    excessArea.ExcessAreaAllotmentRate = model.ExcessAreaAllotmentRate;
                    excessArea.LocationCharge = model.ExcessAreaLocationCharge;
                    excessArea.ExcessCharge = model.ExcessAreaCharge;
                    excessArea.TotalExcessPremium = model.TotalPropertyCost;
                    excessArea.OneTimeLeaseRent = model.OneTimeLeaseRent;
                    excessArea.TotalExcessAreaCost = model.TotalExcessAreaCost;
                    excessArea.ExcessAreaValidDate = model.ExcessAreaValidDate;
                    excessArea.PenalInterest = model.PenalInterest;
                    excessArea.PenalAmount = model.PenalInterestAmount;
                    excessArea.TotalPayableAmount = model.TotalPayableAmount;
                    excessArea.IsActive = true;
                    excessArea.CreatedBy = userInfo.UserID;
                    excessArea.CreatedDate = DateTime.Now;
                    dbContext.ExcessAreaDetailMsts.Add(excessArea);
                    dbContext.SaveChanges();

                    excessRefId = excessArea.Id;
                    flag = ReturnType.Saved;
                }
                var propertydetails = dbContext.SchemePropTrans.Where(m => m.propertyId == model.PropertyId).FirstOrDefault();
                propertydetails.totalArea = model.TotalArea;
                propertydetails.IsExcessArea = true;
                propertydetails.ExcessAreaRefId = excessRefId;
                dbContext.SaveChanges();
                return flag;
            }
        }

        public DataSourceResult GetExcessAreaDetailListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from exsarea in dbContext.ExcessAreaDetailMsts
                            from alotment in dbContext.AllotmentMasters.Where(a => a.propertyId == exsarea.PropertyId).DefaultIfEmpty()
                            select new PropertyViewModel
                            {
                                Id = exsarea.Id,
                                AllottedArea = exsarea.AllottedArea,
                                ExcessArea = exsarea.ExcessArea,
                                TotalArea = exsarea.TotalArea,
                                ExcessAreaDate = exsarea.ExcessAreaDate,
                                ExcessAreaValidDate = exsarea.ExcessAreaValidDate,
                                ExcessAreaCost = exsarea.TotalExcessPremium,
                                TotalExcessAreaCost = exsarea.TotalExcessAreaCost,
                                LandRate = exsarea.ExcessAreaAllotmentRate,
                                ExcessAreaLocationCharge = exsarea.LocationCharge,
                                LocationCharge = exsarea.LocationCharge,
                                OneTimeLeaseRent = exsarea.OneTimeLeaseRent,
                                IsActive = exsarea.IsActive,
                                PenalInterest = exsarea.PenalInterest,
                                PenalInterestAmount = exsarea.PenalAmount,
                                RegistrationId = alotment.rid,
                                AllotmentDate = alotment.allotmentDate
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public PaymentViewModel GetMasterPaymentDetailByRegistrationId(int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from alotment in dbContext.AllotmentMasters
                              join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                              join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                              from cost in dbContext.SchemeCostTrans.Where(x => x.schemeId == property.schemeId && x.departmentId == property.departmentId && x.propertyTypeId == property.propertyTypeId && x.sectorId == property.sectorId && x.blockId == property.blockId && x.floorId == property.floorId).DefaultIfEmpty()
                              from loccharg in dbContext.LocationChargeDetailMsts.Where(l => l.PropertyId == property.propertyId).DefaultIfEmpty()
                              where alotment.rid == rid
                              && alotment.isActive == 1
                              select new PaymentViewModel
                              {
                                  Id = property.refId,
                                  RegistrationId = alotment.rid,
                                  PropertyId = property.propertyId.ToString(),

                                  DepartmentId = property.departmentId,
                                  Department = property.DepartmentMst.departmentName,
                                  SchemeId = property.schemeId,
                                  SchemeName = property.SchemeMst.schemeName,
                                  SectorId = property.sectorId,
                                  SectorName = property.SectorMst.sectorName,
                                  BlockId = property.blockId,
                                  BlockName = property.BlockMst.blockName,
                                  PlotNo = property.propertyNo,
                                  //PropertyNo = property.SectorMst.sectorName + "/" + (property.blockId == null ? string.Empty : property.BlockMst.blockName + "-") + property.propertyNo,
                                  PropertyTypeId = property.propertyTypeId,
                                  PropertyType = property.PropertyTypeMst.propertyTypeName,
                                  AllottedArea = property.AllottedArea,
                                  TotalArea = property.totalArea,
                                  ActualArea = property.actualArea,
                                  CoveredArea = property.coveredArea,
                                  TotalAllotmentRate = property.TotalAllotmentRate,
                                  AllotmentDate = alotment.allotmentDate,
                                  AllotmentRate = property.landRatePerSqmt != null ? property.landRatePerSqmt : ((cost != null && cost.landRatePerSqmt != null) ? cost.landRatePerSqmt : null),
                                  AllotmentMoney = property.allotmentMoney != null ? property.allotmentMoney : ((cost != null && cost.allotmentMoney != null) ? cost.allotmentMoney : null),
                                  EarnestMoney = property.EarnestMoney != null ? property.EarnestMoney : ((cost != null && cost.earnestMoney != null) ? cost.earnestMoney : null),
                                  PropertyCost = property.propertyCost != null ? property.propertyCost : ((cost != null && cost.propertyCost != null) ? cost.propertyCost : null),
                                  TotalPropertyCost = property.totalPropertyCost != null ? property.totalPropertyCost : ((cost != null && cost.totalPropertyCost != null) ? cost.totalPropertyCost : null),
                                  CivilCost = property.civilCost != null ? property.civilCost : ((cost != null && cost.civilCost != null) ? cost.civilCost : null),
                                  LeaseRent = property.LeaseRent != null ? property.LeaseRent : ((cost != null && cost.leaseRent != null) ? cost.leaseRent : null),
                                  AdvanceLeaseRent = property.AdvanceLeaseRent,
                                  LocationId = loccharg.LocationId,
                                  Location = loccharg.LocationMst.locationName,
                                  LocationCharge = loccharg.LocationCharge,
                                  Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                  ApplicantAddress = aplicant.tCorrespondanceAdd,
                                  InstallmentStartDate = alotment.instalmentStartDate,
                              }).FirstOrDefault();
                if (detail != null)
                {
                    var schedule = dbContext.PaymentScheduleMasters.FirstOrDefault(s => s.Rid == detail.RegistrationId);
                    if (schedule != null)
                    {
                        detail.BalanceAmount = schedule.PrincipalAmount;
                        detail.TotalInstallment = schedule.NoOfInstallment;
                        detail.ScheduleType = schedule.ScheduleType;
                        detail.Frequency = schedule.FrequencyOfInstallment.ToString();
                        detail.InstallmentStartDate = schedule.InstallmentStartDate;
                        detail.NormalInterest = schedule.NormalInterest;
                        detail.PenalInterest = schedule.PenalInterest;
                    }
                    var leaserent = dbContext.LeaseRentPayments.FirstOrDefault(l => l.RegistrationId == detail.RegistrationId);
                    if (leaserent != null)
                    {
                        detail.LeaseDeedDate = leaserent.LeaseDeedDate;
                        detail.FrequencyId = 1;
                        detail.RevisedLeaseRent = leaserent.RevisedPremium;
                        detail.LeaseRentAmount = leaserent.PremiumLeaseRent != null ? (double)leaserent.PremiumLeaseRent : 0;
                        detail.TotalPayableAmount = leaserent.CurrentDues;
                    }
                    var transfer = dbContext.Succ_Mut_Trans.Where(t => t.Rid == detail.RegistrationId).ToList().OrderByDescending(o => o.Request_No).FirstOrDefault();
                    detail.TransferDeedDate = transfer != null ? transfer.Transfer_Date : null;
                    var possession = dbContext.PossessionDetails.Where(p => p.Rid == detail.RegistrationId).FirstOrDefault();
                    detail.PossessionDate = possession != null ? possession.PossessionDate : null;
                    var leasedeed = dbContext.RegistryDetails.FirstOrDefault(l => l.Rid == detail.RegistrationId);
                    detail.LeaseDeedDate = leasedeed != null ? leasedeed.RegistryDoneDate : null;
                    var functional = dbContext.FunctionalDetails.FirstOrDefault(f => f.Rid == detail.RegistrationId);
                    detail.FunctionalDate = functional != null ? functional.FunctionalDate : null;
                }
                return detail;
            }
        }


        public DataSourceResult GetLocationChargedPropertyListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from chargedetail in dbContext.LocationChargeDetailMsts
                            from location in dbContext.LocationMsts.Where(l => l.locationId == chargedetail.LocationId).DefaultIfEmpty()
                            from property in dbContext.SchemePropTrans.Where(p => p.propertyId == chargedetail.PropertyId).DefaultIfEmpty()
                            select new PropertyViewModel
                            {
                                Id = chargedetail.Id,
                                LocationId = chargedetail.LocationId,
                                Location = chargedetail.LocationId != null ? location.locationName : null,
                                PropertyId = chargedetail.PropertyId,
                                LocationChargeRate = chargedetail.LocationChargeRate,
                                LocationCharge = chargedetail.LocationCharge,
                                DepartmentId = property != null ? property.departmentId : null,
                                Department = property != null ? property.DepartmentMst.departmentName : null,
                                SectorId = property != null ? property.sectorId : null,
                                SectorName = property != null ? property.SectorMst.sectorName : null,
                                BlockId = property != null ? property.blockId : null,
                                BlockName = property != null ? property.BlockMst.blockName : null,
                                PlotNo = property != null ? property.propertyNo : null,
                                RegistrationId = chargedetail.PropertyId != null ? dbContext.AllotmentMasters.FirstOrDefault(a => a.propertyId == chargedetail.PropertyId).rid : 0,
                                AllotmentDate = chargedetail.PropertyId != null ? dbContext.AllotmentMasters.FirstOrDefault(a => a.propertyId == chargedetail.PropertyId).allotmentDate : null,
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetPropertyCompensationListAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from compstion in dbContext.PropertyCompensationMsts
                            from property in dbContext.SchemePropTrans.Where(p => p.propertyId == compstion.PropertyId).DefaultIfEmpty()
                            select new PaymentViewModel
                            {
                                Id = compstion.Id,
                                PropertyId = compstion.PropertyId.ToString(),
                                AreaForCompensation = compstion.EffectedArea,
                                CompensationRate = compstion.CompensationRate,
                                CompensationAmount = compstion.CompensationAmount,
                                PenalInterest = compstion.PenalInterest,
                                TotalCompensationAmount = compstion.TotalCompensationAmount,
                                CompensationRequestDate = compstion.CompensationRequestDate,
                                IsActive = compstion.IsActive,
                                CreatedDate = compstion.CreatedDate,
                                DepartmentId = property != null ? property.departmentId : null,
                                Department = property != null ? property.DepartmentMst.departmentName : null,
                                SectorId = property != null ? property.sectorId : null,
                                SectorName = property != null ? property.SectorMst.sectorName : null,
                                BlockId = property != null ? property.blockId : null,
                                BlockName = property != null ? property.BlockMst.blockName : null,
                                PlotNo = property != null ? property.propertyNo : null,
                                RegistrationId = compstion.PropertyId != null ? dbContext.AllotmentMasters.FirstOrDefault(a => a.propertyId == compstion.PropertyId).rid : 0,
                                AllotmentDate = compstion.PropertyId != null ? dbContext.AllotmentMasters.FirstOrDefault(a => a.propertyId == compstion.PropertyId).allotmentDate : null,
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public int SaveCompensationDetail(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                var exCompensation = dbContext.PropertyCompensationMsts.FirstOrDefault(e => e.Id == model.Id);
                if (exCompensation != null)
                {
                    exCompensation.PropertyId = Convert.ToInt32(model.PropertyId);
                    exCompensation.DepartmentId = model.DepartmentId;
                    exCompensation.SectorId = model.SectorId;
                    exCompensation.BlockId = model.BlockId;
                    exCompensation.EffectedArea = model.AreaForCompensation;
                    exCompensation.CompensationRequestDate = model.CompensationRequestDate;
                    exCompensation.CompensationRate = model.CompensationRate;
                    exCompensation.CompensationInPercent = model.CompensationInPercent;
                    exCompensation.CompensationAmount = model.CompensationAmount;
                    exCompensation.PenalInterest = model.PenalInterest;
                    exCompensation.InterestAmount = model.InterestAmount;
                    exCompensation.TotalCompensationAmount = model.TotalCompensationAmount;
                    exCompensation.IsActive = true;
                    exCompensation.ModifiedBy = userInfo.UserID;
                    exCompensation.ModifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                else
                {
                    PropertyCompensationMst compensation = new PropertyCompensationMst();
                    compensation.PropertyId = Convert.ToInt32(model.PropertyId);
                    compensation.DepartmentId = model.DepartmentId;
                    compensation.SectorId = model.SectorId;
                    compensation.BlockId = model.BlockId;
                    compensation.EffectedArea = model.AreaForCompensation;
                    compensation.CompensationRequestDate = model.CompensationRequestDate;
                    compensation.CompensationRate = model.CompensationRate;
                    compensation.CompensationInPercent = model.CompensationInPercent;
                    compensation.CompensationAmount = model.CompensationAmount;
                    compensation.PenalInterest = model.PenalInterest;
                    compensation.InterestAmount = model.InterestAmount;
                    compensation.TotalCompensationAmount = model.TotalCompensationAmount;
                    compensation.IsActive = true;
                    compensation.IsActive = true;
                    compensation.CreatedBy = userInfo.UserID;
                    compensation.CreatedDate = DateTime.Now;
                    dbContext.PropertyCompensationMsts.Add(compensation);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                return flag;
            }
        }


        public PaymentViewModel GetAllottedPropertyCostDetailById(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from alotment in dbContext.AllotmentMasters
                              join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                              join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                              from cost in dbContext.SchemeCostTrans.Where(x => x.schemeId == property.schemeId && x.departmentId == property.departmentId && x.propertyTypeId == property.propertyTypeId && x.sectorId == property.sectorId && x.blockId == property.blockId && x.floorId == property.floorId).DefaultIfEmpty()
                              from loccharg in dbContext.LocationChargeDetailMsts.Where(l => l.PropertyId == property.propertyId).DefaultIfEmpty()
                              where alotment.rid == model.RegistrationId
                              && alotment.isActive == 1
                              select new PaymentViewModel
                              {
                                  Id = property.refId,
                                  RegistrationId = alotment.rid,
                                  PropertyId = property.propertyId.ToString(),

                                  DepartmentId = property.departmentId,
                                  Department = property.DepartmentMst.departmentName,
                                  SchemeId = property.schemeId,
                                  SchemeName = property.SchemeMst.schemeName,
                                  SectorId = property.sectorId,
                                  SectorName = property.SectorMst.sectorName,
                                  BlockId = property.blockId,
                                  BlockName = property.BlockMst.blockName,
                                  PlotNo = property.propertyNo,
                                  //PropertyNo = property.SectorMst.sectorName + "/" + (property.blockId == null ? string.Empty : property.BlockMst.blockName + "-") + property.propertyNo,
                                  PropertyTypeId = property.propertyTypeId,
                                  PropertyType = property.PropertyTypeMst.propertyTypeName,
                                  AllottedArea = property.AllottedArea,
                                  TotalArea = property.totalArea,
                                  ActualArea = property.actualArea,
                                  CoveredArea = property.coveredArea,
                                  TotalAllotmentRate = property.TotalAllotmentRate,
                                  AllotmentDate = alotment.allotmentDate,
                                  AllotmentRate = property.landRatePerSqmt != null ? property.landRatePerSqmt : ((cost != null && cost.landRatePerSqmt != null) ? cost.landRatePerSqmt : null),
                                  AllotmentMoney = property.allotmentMoney != null ? property.allotmentMoney : ((cost != null && cost.allotmentMoney != null) ? cost.allotmentMoney : null),
                                  EarnestMoney = property.EarnestMoney != null ? property.EarnestMoney : ((cost != null && cost.earnestMoney != null) ? cost.earnestMoney : null),
                                  PropertyCost = property.propertyCost != null ? property.propertyCost : ((cost != null && cost.propertyCost != null) ? cost.propertyCost : null),
                                  TotalPropertyCost = property.totalPropertyCost != null ? property.totalPropertyCost : ((cost != null && cost.totalPropertyCost != null) ? cost.totalPropertyCost : null),
                                  CivilCost = property.civilCost != null ? property.civilCost : ((cost != null && cost.civilCost != null) ? cost.civilCost : null),
                                  LeaseRent = property.LeaseRent != null ? property.LeaseRent : ((cost != null && cost.leaseRent != null) ? cost.leaseRent : null),
                                  AdvanceLeaseRent = property.AdvanceLeaseRent,
                                  LocationId = loccharg.LocationId,
                                  Location = loccharg.LocationMst.locationName,
                                  LocationCharge = loccharg.LocationCharge,
                                  Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                  ApplicantAddress = aplicant.tCorrespondanceAdd,
                                  InstallmentStartDate = alotment.instalmentStartDate,
                              }).FirstOrDefault();
                return detail;
            }
        }

        public PaymentViewModel GetPropertyLeaseRentCostDetailById(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                PaymentViewModel data = new PaymentViewModel();
                var leaserent = dbContext.LeaseRentPayments.FirstOrDefault(l => l.RegistrationId == model.RegistrationId);
                if (leaserent != null)
                {
                    data.LeaseDeedDate = leaserent.LeaseDeedDate;
                    data.FrequencyId = 1;
                    data.RevisedLeaseRent = leaserent.RevisedPremium;
                    data.LeaseRentAmount = leaserent.PremiumLeaseRent != null ? (double)leaserent.PremiumLeaseRent : 0;
                    data.TotalPayableAmount = leaserent.CurrentDues;
                    data.PenalInterest = leaserent.PanelInterest;
                    data.PenalInterestAmount = leaserent.BalanceInterest;
                    data.CurrentDuesDate = leaserent.CurrentDuesDate;
                    data.TotalPayableAmount = leaserent.CurrentDues;
                }
                return data;
            }
        }

        public PaymentViewModel GetPropertyInstallmentScheduleCostDetailById(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                PaymentViewModel data = new PaymentViewModel();
                var schedule = dbContext.PaymentScheduleMasters.FirstOrDefault(s => s.Rid == model.RegistrationId);
                if (schedule != null)
                {
                    data.BalanceAmount = schedule.PrincipalAmount;
                    data.TotalInstallment = schedule.NoOfInstallment;
                    data.ScheduleType = schedule.ScheduleType;
                    data.Frequency = schedule.FrequencyOfInstallment.ToString();
                    data.InstallmentStartDate = schedule.InstallmentStartDate;
                    data.NormalInterest = schedule.NormalInterest;
                    data.PenalInterest = schedule.PenalInterest;
                }
                return data;
            }
        }


        public PaymentViewModel GetPropertyExcessAreaCostDetailById(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                PaymentViewModel data = new PaymentViewModel();
                var excessArea = dbContext.ExcessAreaDetailMsts.FirstOrDefault(s => s.PropertyId.ToString() == model.PropertyId);
                if (excessArea != null)
                {
                    data.Id = excessArea.Id;
                    data.AllottedArea = excessArea.AllottedArea;
                    data.TotalExcessArea = excessArea.ExcessArea;
                    data.TotalArea = excessArea.TotalArea;
                    data.ExcessAreaRate = excessArea.ExcessAreaAllotmentRate;
                    data.LocationCharge = excessArea.LocationCharge;
                    data.ExcessAreaCost = excessArea.ExcessCharge;
                    data.TotalPremiumAmount = excessArea.TotalExcessPremium;
                    data.TotalExcessAreaCost = excessArea.TotalExcessAreaCost;
                    data.OneTimeLeaseRentAmount = excessArea.OneTimeLeaseRent;
                    data.ExcessAreaDate = excessArea.ExcessAreaDate;
                    data.ValidUptoDate = excessArea.ExcessAreaValidDate;
                    data.PenalInterest = excessArea.PenalInterest;
                    data.PenalInterestAmount = excessArea.PenalAmount;
                    data.TotalPayableAmount = excessArea.TotalPayableAmount;
                    data.IsActive = excessArea.IsActive;
                }
                return data;
            }
        }

        public PaymentViewModel GetPropertyCompensationCostDetailById(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                PaymentViewModel data = new PaymentViewModel();
                var compensation = dbContext.PropertyCompensationMsts.FirstOrDefault(s => s.PropertyId.ToString() == model.PropertyId);
                if (compensation != null)
                {
                    data.Id = compensation.Id;
                    data.EffectedArea = compensation.EffectedArea;
                    data.AreaForCompensation = compensation.EffectedArea;
                    data.CompensationRate = compensation.CompensationRate;
                    data.CompensationInPercent = compensation.CompensationInPercent;
                    data.CompensationAmount = compensation.CompensationAmount;
                    data.PenalInterest = compensation.PenalInterest;
                    data.InterestAmount = compensation.InterestAmount;
                    data.TotalCompensationAmount = compensation.TotalCompensationAmount;
                    data.CompensationRequestDate = compensation.CompensationRequestDate;
                    data.IsActive = compensation.IsActive;
                }
                return data;
            }
        }

        public PaymentViewModel GetPropertyTransferCostDetailById(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                PaymentViewModel data = new PaymentViewModel();
                var transfer = dbContext.Succ_Mut_Trans.Where(s => s.Rid == model.RegistrationId).OrderByDescending(o => o.Request_No).FirstOrDefault();
                if (transfer != null)
                {
                    data.Id = transfer.Request_No;
                    data.Applicant = transfer.T_Gender == Constants.Company ? transfer.T_Company_Name : transfer.T_First_Name;
                    data.ApplicantAddress = transfer.T_Correspondence_Add;
                    data.TransferChargeRate = transfer.Transfer_Charge;
                    data.TransferAmount = transfer.Total_Transfer_Charge;
                }
                return data;
            }
        }


        public int SavePaidChallanDetailByRegistrationId(PaymentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var tempchallan = (List<PaymentViewModel>)HttpContext.Current.Session["TempPaidChallan"];
                if (tempchallan != null)
                {
                    var receiptMaster = dbContext.RECEIPT_DETAIL_MASTER.Where(r => r.RECEIPT_ID == model.ReceiptId).FirstOrDefault();
                    if (receiptMaster == null)
                    {
                        decimal? totalAmount = 0;
                        RECEIPT_DETAIL_MASTER rdm = new RECEIPT_DETAIL_MASTER();
                        rdm.RECEIPT_ID = (long)model.ReceiptId;
                        rdm.RID_NO = model.RegistrationId.ToString();
                        rdm.ALLOTE_NAME = model.Applicant;
                        rdm.ADDRESS = model.ApplicantAddress;
                        //rdm.AMOUNT = model.Amount; //updated
                        rdm.AMOUNT = model.TotalAmount;
                        rdm.SECTOR = model.SectorName;
                        rdm.BLOCK = model.BlockName;
                        rdm.PROP_ID = model.PropertyId;
                        rdm.PROPERTY_NUMBER = model.PropertyNo;
                        rdm.DEPOSETER_NAME = model.DepositorName;
                        rdm.DEPT_ID = model.DepartmentId;
                        //rdm.PROP_REG_ID = model.PROPERTY_REGESTRY_ID;
                        rdm.ENTRY_DATE = model.EntryDate;
                        rdm.DEPOSIT_DATE = model.DepositDate;
                        rdm.STATUS = 1;
                        rdm.USERID = userInfo.UserID.ToString();
                        rdm.BANK_ID = model.BankId.ToString();
                        rdm.CHALLAN_ID = model.ChallanId;

                        dbContext.RECEIPT_DETAIL_MASTER.Add(rdm);
                        dbContext.SaveChanges();

                        long? receiptId = rdm.RECEIPT_ID;

                        List<RECEIPT_AMOUNT_TRANS> reciptList = new List<RECEIPT_AMOUNT_TRANS>();
                        foreach (var challan in tempchallan)
                        {
                            RECEIPT_AMOUNT_TRANS rmt = new RECEIPT_AMOUNT_TRANS();
                            rmt.RECEIPT_ID = receiptId;
                            rmt.DEPT_CODE = model.DepartmentId;
                            rmt.RECEIPT_SUBHEAD_ID = challan.ReceiptSubHeadId;
                            rmt.RECEIPT_HEAD_ID = challan.ReceiptHeadId;
                            rmt.AMOUNT_PAID = challan.Amount.Value;
                            totalAmount = totalAmount + challan.Amount;
                            //rmt.CHALLAN_ID =challan.ChallanId;
                            rmt.CHALLAN_ID = model.ChallanId;
                            rmt.STATUS = 1;
                            rmt.USERID = userInfo.UserID.ToString();
                            rmt.ENTRY_DATE = model.EntryDate;
                            rmt.DEPOSIT_DATE = model.DepositDate.Value;

                            reciptList.Add(rmt);
                        }
                        //var arm = dbContext.RECEIPT_DETAIL_MASTER.Where(r => r.RECEIPT_ID == receiptId).FirstOrDefault();
                        //arm.AMOUNT = totalAmount;
                        dbContext.RECEIPT_AMOUNT_TRANS.AddRange(reciptList);
                        dbContext.SaveChanges();
                        flag = ReturnType.Saved;
                        HttpContext.Current.Session["TempPaidChallan"] = null;
                    }
                    else
                    {
                        receiptMaster.AMOUNT = model.TotalAmount;
                        receiptMaster.MODIFIED_BY = userInfo.UserID;
                        receiptMaster.MODIFY_DATE = DateTime.Now;
                        var receiptTrans = dbContext.RECEIPT_AMOUNT_TRANS.Where(r => r.RECEIPT_ID == receiptMaster.RECEIPT_ID).ToList();
                        dbContext.RECEIPT_AMOUNT_TRANS.RemoveRange(receiptTrans);
                        List<RECEIPT_AMOUNT_TRANS> reciptList = new List<RECEIPT_AMOUNT_TRANS>();
                        foreach (var challan in tempchallan)
                        {
                            RECEIPT_AMOUNT_TRANS rmt = new RECEIPT_AMOUNT_TRANS();
                            rmt.RECEIPT_ID = receiptMaster.RECEIPT_ID;
                            rmt.DEPT_CODE = model.DepartmentId;
                            rmt.RECEIPT_SUBHEAD_ID = challan.ReceiptSubHeadId;
                            rmt.RECEIPT_HEAD_ID = challan.ReceiptHeadId;
                            rmt.AMOUNT_PAID = challan.Amount.Value;
                            rmt.CHALLAN_ID = model.ChallanId; //challan.ChallanId;
                            rmt.STATUS = 1;
                            rmt.USERID = userInfo.UserID.ToString();
                            rmt.ENTRY_DATE = model.EntryDate;
                            rmt.DEPOSIT_DATE = model.DepositDate.Value;

                            reciptList.Add(rmt);
                        }

                        dbContext.RECEIPT_AMOUNT_TRANS.AddRange(reciptList);
                        dbContext.SaveChanges();
                        flag = ReturnType.Saved;
                        HttpContext.Current.Session["TempPaidChallan"] = null;
                    }
                }
            }
            return flag;
        }

        public int SaveTempPaidChallanDetail(PaymentViewModel model)
        {
            int flag = ReturnType.None;
            var dbContext = new NoidaPMSEntities();
            var tempchallan = (List<PaymentViewModel>)HttpContext.Current.Session["TempPaidChallan"];
            if (tempchallan != null && tempchallan.Count > 0)
            {
                var rid = tempchallan.FirstOrDefault().RegistrationId;
                if (rid == model.RegistrationId)
                {
                    PaymentViewModel challan = new PaymentViewModel();
                    challan.SerialNo = tempchallan.ToList().Count + 1;
                    challan.ReceiptHeadId = model.ReceiptHeadId;
                    challan.ReceiptHead = model.ReceiptHead; //dbContext.RECIEPT_HEAD.FirstOrDefault(r => r.RECIEPT_CODE == model.ReceiptHeadId).RECIEPT_HEAD_NAME;
                    challan.ReceiptSubHeadId = model.ReceiptSubHeadId;
                    challan.ReceiptSubHead = model.ReceiptSubHead;// dbContext.RECEIPT_SUB_HEAD.FirstOrDefault(r => r.RECEIPT_SUBHEAD_ID == model.ReceiptSubHeadId).RECEIPT_SUB_HEAD1;
                    //challan.ScheduleId = model.ScheduleId;
                    challan.RegistrationId = model.RegistrationId;
                    challan.Amount = model.Amount;
                    tempchallan.Add(challan);
                    HttpContext.Current.Session["TempInstallment"] = tempchallan;
                    flag = ReturnType.Saved;
                }
                else
                {
                    flag = ReturnType.Mismatch;
                }
            }
            else
            {
                List<PaymentViewModel> challanList = new List<PaymentViewModel>();
                PaymentViewModel challan = new PaymentViewModel();
                challan.SerialNo = 1;
                challan.ReceiptHeadId = model.ReceiptHeadId;
                challan.ReceiptHead = model.ReceiptHead; //dbContext.RECIEPT_HEAD.FirstOrDefault(r => r.RECIEPT_CODE == model.ReceiptHeadId).RECIEPT_HEAD_NAME;
                challan.ReceiptSubHeadId = model.ReceiptSubHeadId;
                challan.ReceiptSubHead = model.ReceiptSubHead; //dbContext.RECEIPT_SUB_HEAD.FirstOrDefault(r => r.RECEIPT_SUBHEAD_ID == model.ReceiptSubHeadId).RECEIPT_SUB_HEAD1;
                //challan.ScheduleId = model.ScheduleId;
                challan.RegistrationId = model.RegistrationId;
                challan.Amount = model.Amount;
                challanList.Add(challan);
                HttpContext.Current.Session["TempPaidChallan"] = challanList;
                flag = ReturnType.Saved;
            }
            return flag;
        }

        public DataSourceResult GetSavedTempPaidChallanDetail(DataSourceRequest request, PaymentViewModel model)
        {
            if (model.ActionType == "PaidChallan")
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    var challan = (from master in dbContext.RECEIPT_DETAIL_MASTER
                                   join trans in dbContext.RECEIPT_AMOUNT_TRANS on master.RECEIPT_ID equals trans.RECEIPT_ID
                                   from head in dbContext.RECIEPT_HEAD.Where(p => p.RECIEPT_CODE == trans.RECEIPT_HEAD_ID).DefaultIfEmpty()
                                   from subhead in dbContext.RECEIPT_SUB_HEAD.Where(p => p.RECEIPT_SUBHEAD_ID == trans.RECEIPT_SUBHEAD_ID).DefaultIfEmpty()
                                   where master.RECEIPT_ID == model.ReceiptId
                                   select new PaymentViewModel
                                   {
                                       SerialNo = trans.id,
                                       ReceiptHeadId = trans.RECEIPT_HEAD_ID,
                                       ReceiptSubHeadId = trans.RECEIPT_SUBHEAD_ID,
                                       Amount = trans.AMOUNT_PAID,
                                       RegistrationId = model.RegistrationId,
                                       ReceiptHead = head.RECIEPT_HEAD_NAME,
                                       ReceiptSubHead = subhead.RECEIPT_SUB_HEAD1,
                                       DepartmentId = master.DEPT_ID
                                   }).ToList();
                    HttpContext.Current.Session["TempPaidChallan"] = challan;
                    return challan.ToDataSourceResult(request);
                }
            }
            else
            {
                var tempchallan = (List<PaymentViewModel>)HttpContext.Current.Session["TempPaidChallan"];
                if (tempchallan != null)
                {
                    List<PaymentViewModel> list = new List<PaymentViewModel>();
                    for (int i = 0; i < tempchallan.Count; i++)
                    {
                        PaymentViewModel challan = new PaymentViewModel();
                        challan.SerialNo = i + 1;
                        challan.ReceiptHeadId = tempchallan[i].ReceiptHeadId;
                        challan.ReceiptHead = tempchallan[i].ReceiptHead;
                        challan.ReceiptSubHeadId = tempchallan[i].ReceiptSubHeadId;
                        challan.ReceiptSubHead = tempchallan[i].ReceiptSubHead;
                        //challan.ScheduleId = tempchallan[i].ScheduleId;
                        challan.RegistrationId = tempchallan[i].RegistrationId;
                        challan.Amount = tempchallan[i].Amount;
                        list.Add(challan);
                    }
                    //return tempchallan.ToDataSourceResult(request);
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    var list = new List<PaymentViewModel>();
                    return list.ToDataSourceResult(request);
                }
            }

        }

        public int RemoveTempPaidChallanDataById(PaymentViewModel model)
        {
            int flag = ReturnType.None;
            var tempchallan = (List<PaymentViewModel>)HttpContext.Current.Session["TempPaidChallan"];
            if (tempchallan != null)
            {
                if (model.ActionType == "RemoveAll")
                {
                    //.RemoveAt((int)model.SerialNo.Value - 1);
                    HttpContext.Current.Session["TempPaidChallan"] = null;
                    flag = ReturnType.Removed;
                }
                else
                {
                    tempchallan.RemoveAt((int)model.SerialNo.Value - 1);
                    flag = ReturnType.Removed;
                }

            }
            return flag;
        }

        public int RemovePaidChallanDataById(PaymentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "ReceiptMaster")
                {
                    var receiptDetail = dbContext.RECEIPT_DETAIL_MASTER.Where(r => r.RECEIPT_ID == model.ReceiptId).FirstOrDefault();
                    var receiptTransList = dbContext.RECEIPT_AMOUNT_TRANS.Where(r => r.RECEIPT_ID == model.ReceiptId).ToList();
                    dbContext.RECEIPT_AMOUNT_TRANS.RemoveRange(receiptTransList);
                    dbContext.RECEIPT_DETAIL_MASTER.Remove(receiptDetail);
                    dbContext.SaveChanges();
                    flag = ReturnType.Removed;
                }
                if (model.ActionType == "ReceiptTrans")
                {
                    var receiptTrans = dbContext.RECEIPT_AMOUNT_TRANS.Where(r => r.id == model.Id).FirstOrDefault();
                    dbContext.RECEIPT_AMOUNT_TRANS.Remove(receiptTrans);
                    dbContext.SaveChanges();
                    flag = ReturnType.Removed;
                }
            }
            return flag;
        }

        public DataSourceResult GetPaidAmountDetailListByIdAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.RegistrationId != null)
                {
                    var list = (from detail in dbContext.RECEIPT_DETAIL_MASTER
                                join trans in dbContext.RECEIPT_AMOUNT_TRANS on detail.RECEIPT_ID equals trans.RECEIPT_ID
                                from alotment in dbContext.AllotmentMasters.Where(a => a.rid.ToString() == detail.RID_NO).DefaultIfEmpty()
                                from type in dbContext.RECIEPT_HEAD.Where(p => p.RECIEPT_CODE == trans.RECEIPT_HEAD_ID).DefaultIfEmpty()
                                from subtype in dbContext.RECEIPT_SUB_HEAD.Where(p => p.RECEIPT_SUBHEAD_ID == trans.RECEIPT_SUBHEAD_ID).DefaultIfEmpty()
                                where detail.RID_NO == model.RegistrationId.ToString()
                                select new PaymentViewModel
                                {
                                    Id = trans.id,
                                    ReceiptId = detail.RECEIPT_ID,
                                    RegistrationNo = detail.RID_NO,
                                    DepartmentName = alotment.DepartmentMst.departmentName,
                                    ReceiptHeadId = trans.RECEIPT_HEAD_ID,
                                    ReceiptHead = type.RECIEPT_HEAD_NAME,
                                    ReceiptSubHeadId = trans.RECEIPT_SUBHEAD_ID,
                                    ReceiptSubHead = subtype.RECEIPT_SUB_HEAD1,
                                    Amount = trans.AMOUNT_PAID,
                                    ChallanId = detail.CHALLAN_ID,
                                    DepositDate = detail.DEPOSIT_DATE,
                                    EntryDate = detail.ENTRY_DATE
                                });
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    var list = new List<PaymentViewModel>();
                    return list.ToDataSourceResult(request);
                }
            }
        }


        public DataSourceResult GetAllotmentDuesCalculationDetailAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from rdm in dbContext.RECEIPT_DETAIL_MASTER
                            join rat in dbContext.RECEIPT_AMOUNT_TRANS on rdm.RECEIPT_ID equals rat.RECEIPT_ID
                            from head in dbContext.RECIEPT_HEAD.Where(h => h.RECIEPT_CODE == rat.RECEIPT_HEAD_ID).DefaultIfEmpty()
                            from subhead in dbContext.RECEIPT_SUB_HEAD.Where(s => s.RECEIPT_SUBHEAD_ID == rat.RECEIPT_SUBHEAD_ID).DefaultIfEmpty()
                            where (rdm.RID_NO == model.RegistrationId.ToString() && rat.RECEIPT_HEAD_ID == 2)
                            select new PaymentViewModel
                            {
                                Id = rat.id,
                                RegistrationNo = rdm.RID_NO,
                                TotalAmount = rdm.AMOUNT,
                                ReceiptId = rdm.RECEIPT_ID,
                                ChallanId = rdm.CHALLAN_ID,
                                ReceiptHeadId = rat.RECEIPT_HEAD_ID,
                                ReceiptHeadName = head.RECIEPT_HEAD_NAME,
                                ReceiptSubHeadId = rat.RECEIPT_SUBHEAD_ID,
                                ReceiptSubHeadName = subhead.RECEIPT_SUB_HEAD1,
                                DepositDate = rat.DEPOSIT_DATE,
                                Amount = rat.AMOUNT_PAID
                            }).ToList();
                return data.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetExcessAreaDuesCalculationDetailAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from rdm in dbContext.RECEIPT_DETAIL_MASTER
                            join rat in dbContext.RECEIPT_AMOUNT_TRANS on rdm.RECEIPT_ID equals rat.RECEIPT_ID
                            from head in dbContext.RECIEPT_HEAD.Where(h => h.RECIEPT_CODE == rat.RECEIPT_HEAD_ID).DefaultIfEmpty()
                            from subhead in dbContext.RECEIPT_SUB_HEAD.Where(s => s.RECEIPT_SUBHEAD_ID == rat.RECEIPT_SUBHEAD_ID).DefaultIfEmpty()
                            where (rdm.RID_NO == model.RegistrationId.ToString() && rat.RECEIPT_HEAD_ID == 2 && rat.RECEIPT_SUBHEAD_ID == 186)
                            select new PaymentViewModel
                            {
                                Id = rat.id,
                                RegistrationNo = rdm.RID_NO,
                                TotalAmount = rdm.AMOUNT,
                                ReceiptId = rdm.RECEIPT_ID,
                                ChallanId = rdm.CHALLAN_ID,
                                ReceiptHeadId = rat.RECEIPT_HEAD_ID,
                                ReceiptHeadName = head.RECIEPT_HEAD_NAME,
                                ReceiptSubHeadId = rat.RECEIPT_SUBHEAD_ID,
                                ReceiptSubHeadName = subhead.RECEIPT_SUB_HEAD1,
                                DepositDate = rat.DEPOSIT_DATE,
                                Amount = rat.AMOUNT_PAID
                            }).ToList();
                return data.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetCompensationDuesCalculationDetailAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from rdm in dbContext.RECEIPT_DETAIL_MASTER
                            join rat in dbContext.RECEIPT_AMOUNT_TRANS on rdm.RECEIPT_ID equals rat.RECEIPT_ID
                            from head in dbContext.RECIEPT_HEAD.Where(h => h.RECIEPT_CODE == rat.RECEIPT_HEAD_ID).DefaultIfEmpty()
                            from subhead in dbContext.RECEIPT_SUB_HEAD.Where(s => s.RECEIPT_SUBHEAD_ID == rat.RECEIPT_SUBHEAD_ID).DefaultIfEmpty()
                            where (rdm.RID_NO == model.RegistrationId.ToString() && rat.RECEIPT_HEAD_ID == 144)
                            select new PaymentViewModel
                            {
                                Id = rat.id,
                                RegistrationNo = rdm.RID_NO,
                                TotalAmount = rdm.AMOUNT,
                                ReceiptId = rdm.RECEIPT_ID,
                                ChallanId = rdm.CHALLAN_ID,
                                ReceiptHeadId = rat.RECEIPT_HEAD_ID,
                                ReceiptHeadName = head.RECIEPT_HEAD_NAME,
                                ReceiptSubHeadId = rat.RECEIPT_SUBHEAD_ID,
                                ReceiptSubHeadName = subhead.RECEIPT_SUB_HEAD1,
                                DepositDate = rat.DEPOSIT_DATE,
                                Amount = rat.AMOUNT_PAID
                            }).ToList();
                return data.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetTransferDuesCalculationDetailAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from rdm in dbContext.RECEIPT_DETAIL_MASTER
                            join rat in dbContext.RECEIPT_AMOUNT_TRANS on rdm.RECEIPT_ID equals rat.RECEIPT_ID
                            from head in dbContext.RECIEPT_HEAD.Where(h => h.RECIEPT_CODE == rat.RECEIPT_HEAD_ID).DefaultIfEmpty()
                            from subhead in dbContext.RECEIPT_SUB_HEAD.Where(s => s.RECEIPT_SUBHEAD_ID == rat.RECEIPT_SUBHEAD_ID).DefaultIfEmpty()
                            where (rdm.RID_NO == model.RegistrationId.ToString() && rat.RECEIPT_HEAD_ID == 52 && rat.RECEIPT_SUBHEAD_ID == 11)
                            select new PaymentViewModel
                            {
                                Id = rat.id,
                                RegistrationNo = rdm.RID_NO,
                                TotalAmount = rdm.AMOUNT,
                                ReceiptId = rdm.RECEIPT_ID,
                                ChallanId = rdm.CHALLAN_ID,
                                ReceiptHeadId = rat.RECEIPT_HEAD_ID,
                                ReceiptHeadName = head.RECIEPT_HEAD_NAME,
                                ReceiptSubHeadId = rat.RECEIPT_SUBHEAD_ID,
                                ReceiptSubHeadName = subhead.RECEIPT_SUB_HEAD1,
                                DepositDate = rat.DEPOSIT_DATE,
                                Amount = rat.AMOUNT_PAID
                            }).ToList();
                return data.ToDataSourceResult(request);
            }
        }


        public PaymentViewModel GetExistingPaidChallanDetail(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var challan = (from master in dbContext.RECEIPT_DETAIL_MASTER
                               where master.RECEIPT_ID == model.ReceiptId
                               select new PaymentViewModel
                               {
                                   ReceiptId = model.ReceiptId,
                                   ChallanId = master.CHALLAN_ID,
                                   TotalAmount = master.AMOUNT,
                                   DepositDate = master.DEPOSIT_DATE,
                                   EntryDate = master.ENTRY_DATE,
                                   DepositorName = master.DEPOSETER_NAME,
                                   DepositorAddress = master.ADDRESS
                               }).FirstOrDefault();
                return challan;
            }
        }


        public DataSourceResult GetNDCGeneratedLetterListAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<NDCVeiwModel> idList = new List<NDCVeiwModel>();
                idList = (from ndcmst in dbContext.NDCDetailMsts
                          where (model.RegistrationId == null || ndcmst.RegistrationId == model.RegistrationId)
                          select new NDCVeiwModel
                          {
                              Id = dbContext.NDCDetailMsts.Where(m => m.RegistrationId == ndcmst.RegistrationId).OrderByDescending(c => c.Id).FirstOrDefault().Id
                          }).ToList();
                List<int> intList = idList.Select(k => (int)k.Id).ToList();

                var data = (from ndc in dbContext.NDCDetailMsts
                            where (model.RegistrationId == null || ndc.RegistrationId == model.RegistrationId) && intList.Contains(ndc.Id)
                            select new NDCVeiwModel
                            {
                                Id = ndc.Id,
                                RegistrationId = ndc.RegistrationId.ToString(),
                                RegistrationNo = ndc.RegistrationId,
                                Applicant = ndc.Applicant,
                                Sector = ndc.Sector,
                                Block = ndc.Block,
                                PlotNo = ndc.PlotNo,
                                //Department = ndc.DepartmentId == 1 ? DepartmentInHindi.Institutional : (ndc.DepartmentId == 2 ? DepartmentInHindi.Commercial : (ndc.DepartmentId == 3 ? DepartmentInHindi.Residential : (ndc.DepartmentId == 4 ? DepartmentInHindi.Industry : (ndc.DepartmentId == 5 ? DepartmentInHindi.Housing : (ndc.DepartmentId == 6 ? DepartmentInHindi.GroupHousing : DepartmentInHindi.Residential))))),
                                Department = ndc.Department,
                                InstallmentDateInWord = ndc.InstallmentDateInWord,
                                InterestDateInWord = ndc.InterestDateInWord,
                                LeaseRentDateInWord = ndc.LeaseRentDateInWord,
                                IsTotalInstallmentPaid = ndc.IsTotalInstallmentPaid,
                                IsOneTimeLeasePaid = ndc.IsOneTimeLeasePaid,
                                NDCDate = ndc.NDCDate,
                                LetterNo = ndc.LetterNo,
                                LetterDateInWord = ndc.LetterDateInWord,
                                ChallanId = ndc.ChallanDetail,
                                ChalanAmount = ndc.ChallanAmount,
                                StatusId = ndc.StatusId,
                                Status = ndc.StatusId != null ? dbContext.StatusMasters.FirstOrDefault(s => s.Id == ndc.StatusId).Status : string.Empty,
                                TemplateContent = ndc.NDCTemplate,
                                CreatedDate = ndc.CreatedDate,
                                IsActive = ndc.IsActive,
                                Remarks = ndc.Remarks,
                                IsLetterIssued = ndc.IsLetterIssued,
                                ActionType = ndc.LetterType
                            });
                return data.ToDataSourceResult(request);
            }
        }


        public int SaveGeneratedLetterByType(LetterViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.LetterType == "DemandNote")
                {
                    var data = dbContext.DemandNoteDetails.Where(r => r.RegistrationId == model.RegistrationId).ToList().OrderByDescending(o => o.Id).FirstOrDefault();
                    data.DemandNoteTemplate = model.LetterContent;
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
            }
            return flag;
        }


        public DataSourceResult GetReceiptIdListByRId(DataSourceRequest request, int? rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from receipt in dbContext.RECEIPT_DETAIL_MASTER
                            where receipt.RID_NO == rId.ToString() && receipt.STATUS == 1
                            select new DropdownViewModel
                            {
                                Id = 0,
                                ReceiptId = receipt.RECEIPT_ID
                                //ReceiptCode = receipt.RECEIPT_ID
                            });
                request.Filters.RemoveAt(0);
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetReceiptHeadListByReceiptId(DataSourceRequest request, long? receiptId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from trans in dbContext.RECEIPT_AMOUNT_TRANS
                            where trans.RECEIPT_ID == receiptId && trans.STATUS == 1
                            select new DropdownViewModel
                            {
                                Id = (int)trans.RECEIPT_HEAD_ID,
                                Text = trans.RECIEPT_HEAD.RECIEPT_HEAD_NAME,
                                ReceiptId = null
                            });
                return list.ToDataSourceResult(request);
            }
        }


        //public int RemoveChallanDetailsById(int? Id)
        //{
        //    var flag = ReturnType.None;
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var challan = dbContext.RECEIPT_AMOUNT_TRANS.Where(m => m.ID == Id).FirstOrDefault();
        //        if (challan != null)
        //        {
        //            dbContext.RECEIPT_AMOUNT_TRANS.Remove(challan);
        //            dbContext.SaveChanges();
        //            flag = ReturnType.Removed;
        //        }
        //    }
        //    return flag;
        //}


        //public int SaveTempChallanDetailsByReceiptId(PaymentViewModel model)
        //{
        //    int flag = ReturnType.None;
        //    var dbContext = new NoidaPMSEntities();
        //    var tempchallan = (List<PaymentViewModel>)HttpContext.Current.Session["TempChallanByReceiptId"];
        //    if (tempchallan != null)
        //    {
        //        var rid = tempchallan.FirstOrDefault().RegistrationId;
        //        var receiptid = tempchallan.FirstOrDefault().ReceiptId;
        //        if (rid == model.RegistrationId && receiptid != null)
        //        {
        //            PaymentViewModel challan = new PaymentViewModel();
        //            challan.RefId = tempchallan.ToList().Count + 1;
        //            challan.ReceiptHeadId = model.ReceiptHeadId;
        //            challan.ReceiptHeadName = model.ReceiptHeadName; //dbContext.RECIEPT_HEAD.FirstOrDefault(r => r.RECIEPT_CODE == model.ReceiptHeadId).RECIEPT_HEAD_NAME;
        //            challan.ReceiptSubHeadId = model.ReceiptSubHeadId;
        //            challan.ReceiptSubHeadName = model.ReceiptSubHeadName;// dbContext.RECEIPT_SUB_HEAD.FirstOrDefault(r => r.RECEIPT_SUBHEAD_ID == model.ReceiptSubHeadId).RECEIPT_SUB_HEAD1;
        //            challan.RegistrationId = model.RegistrationId;
        //            challan.ReceiptId = model.ReceiptId;
        //            challan.DepositAmount = model.DepositAmount;
        //            challan.DepositDate = model.DepositDate;
        //            challan.DepartmentId = model.DepartmentId;
        //            tempchallan.Add(challan);
        //            HttpContext.Current.Session["TempChallanByReceiptId"] = tempchallan;
        //            flag = ReturnType.Saved;
        //        }
        //        else
        //        {
        //            flag = ReturnType.Mismatch;
        //        }
        //    }
        //    else
        //    {
        //        List<PaymentViewModel> challanList = new List<PaymentViewModel>();
        //        PaymentViewModel challan = new PaymentViewModel();
        //        challan.RefId = 1;
        //        challan.ReceiptHeadId = model.ReceiptHeadId;
        //        challan.ReceiptHeadName = model.ReceiptHeadName; //dbContext.RECIEPT_HEAD.FirstOrDefault(r => r.RECIEPT_CODE == model.ReceiptHeadId).RECIEPT_HEAD_NAME;
        //        challan.ReceiptSubHeadId = model.ReceiptSubHeadId;
        //        challan.ReceiptSubHeadName = model.ReceiptSubHeadName; //dbContext.RECEIPT_SUB_HEAD.FirstOrDefault(r => r.RECEIPT_SUBHEAD_ID == model.ReceiptSubHeadId).RECEIPT_SUB_HEAD1;
        //        challan.RegistrationId = model.RegistrationId;
        //        challan.DepositAmount = model.DepositAmount;
        //        challan.DepositDate = model.DepositDate;
        //        challan.DepartmentId = model.DepartmentId;
        //        challan.ReceiptId = model.ReceiptId;
        //        challanList.Add(challan);
        //        HttpContext.Current.Session["TempChallanByReceiptId"] = challanList;
        //        flag = ReturnType.Saved;
        //    }
        //    return flag;
        //}


        //public DataSourceResult GetSavedTempChallanDetailByReceiptId(DataSourceRequest request, PaymentViewModel model)
        //{
        //    var tempchallan = (List<PaymentViewModel>)HttpContext.Current.Session["TempChallanByReceiptId"];
        //    if (tempchallan != null)
        //    {
        //        List<PaymentViewModel> list = new List<PaymentViewModel>();
        //        for (int i = 0; i < tempchallan.Count; i++)
        //        {
        //            PaymentViewModel challan = new PaymentViewModel();
        //            challan.RefId = i + 1;
        //            challan.ReceiptHeadId = tempchallan[i].ReceiptHeadId;
        //            challan.ReceiptHeadName = tempchallan[i].ReceiptHeadName;
        //            challan.ReceiptSubHeadId = tempchallan[i].ReceiptSubHeadId;
        //            challan.ReceiptSubHeadName = tempchallan[i].ReceiptSubHeadName;
        //            challan.RegistrationId = tempchallan[i].RegistrationId;
        //            challan.DepositAmount = tempchallan[i].DepositAmount;
        //            challan.DepositDate = model.DepositDate;
        //            challan.DepartmentId = model.DepartmentId;
        //            challan.ReceiptId = model.ReceiptId;
        //            list.Add(challan);
        //        }
        //        //return tempchallan.ToDataSourceResult(request);
        //        return list.ToDataSourceResult(request);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}


        //public int SaveChallanDetails(PaymentViewModel model)
        //{
        //    var flag = ReturnType.None;
        //    decimal? TotalAmount = 0;
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var tempchallan = (List<PaymentViewModel>)HttpContext.Current.Session["TempChallanByReceiptId"];
        //        var receiptdetails = dbContext.RECEIPT_DETAIL_MASTER.Where(m => m.RID_NO == model.RegistrationId.ToString() && m.RECEIPT_ID == model.ReceiptId);
        //        if (receiptdetails != null)
        //        {
        //            foreach (var challan in tempchallan)
        //            {
        //                RECEIPT_AMOUNT_TRANS trans = new RECEIPT_AMOUNT_TRANS();
        //                trans.RECEIPT_ID = model.ReceiptId;
        //                trans.RECEIPT_HEAD_ID = challan.ReceiptHeadId;
        //                trans.RECEIPT_SUBHEAD_ID = challan.ReceiptSubHeadId;
        //                trans.AMOUNT_PAID = challan.DepositAmount;
        //                trans.DEPOSIT_DATE = challan.DepositDate;
        //                trans.CHALLAN_ID = challan.ChallanId;
        //                trans.DEPT_CODE = challan.DepartmentId;
        //                trans.STATUS = NAStatusId.Approved;
        //                trans.USERID = userInfo.UserID.ToString();
        //                trans.ENTRY_DATE = DateTime.Now;                     
        //                dbContext.RECEIPT_AMOUNT_TRANS.Add(trans);
        //                dbContext.SaveChanges();
        //            }
        //            HttpContext.Current.Session["TempChallanByReceiptId"] = null;
        //            flag = ReturnType.Updated;
        //        }
        //        else
        //        {
        //            if (tempchallan != null)
        //            {
        //                RECEIPT_DETAIL_MASTER receipt = new RECEIPT_DETAIL_MASTER();
        //                receipt.RECEIPT_ID = (long)model.ReceiptId;
        //                receipt.RID_NO = model.RegistrationId.ToString();
        //                receipt.ALLOTE_NAME = model.AllotteeName;
        //                receipt.ADDRESS = model.CorresspondentAddress;
        //                //receipt.AMOUNT = model.DepositAmount;
        //                receipt.SECTOR = model.SectorName;
        //                receipt.BLOCK = model.BlockName;
        //                receipt.PROP_ID = model.PlotNo;
        //                receipt.PROPERTY_NUMBER = model.PropertyNumber;
        //                receipt.DEPOSETER_NAME = model.DepositorName;
        //                receipt.DEPT_ID = model.DepartmentId;
        //                receipt.PROP_REG_ID = model.PropertyRegistryId;
        //                receipt.ENTRY_DATE = model.EntryDate;
        //                receipt.DEPOSIT_DATE = model.DepositDate;
        //                receipt.STATUS = 1;
        //                receipt.USERID = userInfo.UserID.ToString();
        //                receipt.BANK_ID = model.BankId.ToString();
        //                receipt.CHALLAN_ID = model.ChallanId;
        //                dbContext.RECEIPT_DETAIL_MASTER.Add(receipt);
        //                dbContext.SaveChanges();                        

        //                foreach (var challan in tempchallan)
        //                {
        //                    RECEIPT_AMOUNT_TRANS trans = new RECEIPT_AMOUNT_TRANS();
        //                    trans.RECEIPT_ID = model.ReceiptId;
        //                    trans.RECEIPT_HEAD_ID = challan.ReceiptHeadId;
        //                    trans.RECEIPT_SUBHEAD_ID = challan.ReceiptSubHeadId;
        //                    trans.AMOUNT_PAID = challan.DepositAmount;
        //                    trans.DEPOSIT_DATE = challan.DepositDate;
        //                    trans.CHALLAN_ID = challan.ChallanId;
        //                    trans.DEPT_CODE = challan.DepartmentId;
        //                    trans.STATUS = NAStatusId.Approved;
        //                    trans.USERID = userInfo.UserID.ToString();
        //                    trans.ENTRY_DATE = DateTime.Now;
        //                    dbContext.RECEIPT_AMOUNT_TRANS.Add(trans);
        //                    dbContext.SaveChanges();
        //                    TotalAmount = TotalAmount + trans.AMOUNT_PAID;
        //                }
        //                receipt.AMOUNT = TotalAmount;
        //                dbContext.SaveChanges();

        //                HttpContext.Current.Session["TempInstallment"] = null;
        //                flag = ReturnType.Saved;
        //            }
        //        }
        //    }
        //    return flag;
        //}


        public DataSourceResult GetAlloteeListByDueDate(DataSourceRequest request, LeaseRentAndInstallmentDashboard model)
        {
            List<LeaseRentAndInstallmentDashboard> list;
            //using (var dbcontext = new NoidaPMSEntities())
            //{

            //    if (model.ActionType == "LeaseRent")
            //    {
            //        var listtemp = dbcontext.Sp_ManageDuuesDetailDepartmentwise(model.CurrentDuesDate, model.DepartmentId, 2);

            //        list = (from inst in listtemp
            //                select new LeaseRentViewModel
            //                    {
            //                        RegistrationId = inst.RegistrationId
            //                    });
            //    }
            //    else
            //    {
            //        //list = dbcontext.Sp_ManageDuuesDetailDepartmentwise(model.CurrentDuesDate, model.DepartmentId, 4);
            //    }
            //}
            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                if (model.ActionType == "LeaseRent")
                {
                    list = connection.Query<LeaseRentAndInstallmentDashboard>("Sp_ManageDuuesDetailDepartmentwise", new { Date = model.CurrentDate, departmentId = model.DepartmentId, type = 2 }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
                else if (model.ActionType == "Installment")
                {
                    list = connection.Query<LeaseRentAndInstallmentDashboard>("Sp_ManageDuuesDetailDepartmentwise", new { Date = model.CurrentDate, departmentId = model.DepartmentId, type = 4 }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
                else
                {
                    list = connection.Query<LeaseRentAndInstallmentDashboard>("Sp_ManageDuuesDetailDepartmentwise", new { Date = model.CurrentDate, departmentId = model.DepartmentId, type = 6 }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }

            }
            return list.ToDataSourceResult(request);
        }


        public LeaseRentAndInstallmentDashboard GetTotalCountByActionType(LeaseRentAndInstallmentDashboard model)
        {
            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                if (model.ActionType == "LeaseRent")
                {
                    //model.TotalLeaseRentCount = connection.Query int?("Sp_ManageDuuesDetailDepartmentwise", new { Date = model.CurrentDate, departmentId = model.DepartmentId, type = 1 }, commandType: System.Data.CommandType.StoredProcedure);
                }
                else
                {
                    //model.TotalInstallmentCount = connection.Query<LeaseRentAndInstallmentDashboard>("Sp_ManageDuuesDetailDepartmentwise", new { Date = model.CurrentDate, departmentId = model.DepartmentId, type = 3 }, commandType: System.Data.CommandType.StoredProcedure);
                }
            }
            return model;
        }


        public DataSourceResult GetListValueByTotalCount(DataSourceRequest request, LeaseRentViewModel model)
        {

            List<LeaseRentViewModel> list = new List<LeaseRentViewModel>();

            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                if (model.ActionType == "LeaseRent")
                {
                    list = connection.Query<LeaseRentViewModel>("Sp_GetListValueByTotalCount", new { departmentId = model.DepartmentId, reportTypeId = 1 }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
                else if (model.ActionType == "LeaseRent_Premium")
                {
                    list = connection.Query<LeaseRentViewModel>("Sp_GetListValueByTotalCount", new { departmentId = model.DepartmentId, reportTypeId = 2 }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
                else if (model.ActionType == "Installment")
                {
                    list = connection.Query<LeaseRentViewModel>("Sp_GetListValueByTotalCount", new { departmentId = model.DepartmentId, reportTypeId = 3 }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
                else if (model.ActionType == "NDC")
                {
                    list = connection.Query<LeaseRentViewModel>("Sp_GetListValueByTotalCount", new { departmentId = model.DepartmentId, reportTypeId = 4 }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
                else if (model.ActionType == "AllProperty")
                {
                    list = connection.Query<LeaseRentViewModel>("Sp_GetListValueByTotalCount", new { departmentId = model.DepartmentId, reportTypeId = 5 }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
                else if (model.ActionType == "FHProperty")
                {
                    list = connection.Query<LeaseRentViewModel>("Sp_GetListValueByTotalCount", new { departmentId = model.DepartmentId, reportTypeId = 6 }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
                else if (model.ActionType == "NotYetUpdate")
                {
                    list = connection.Query<LeaseRentViewModel>("Sp_GetListValueByTotalCount", new { departmentId = model.DepartmentId, reportTypeId = 7 }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }

            }
            return list.ToDataSourceResult(request);

        }


        public int SaveNDCLetterDetails(NDCVeiwModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var flag = ReturnType.None;
                //var todaysNDC = dbContext.NDCDetailMsts.Where(n => n.RegistrationId.ToString() == model.RegistrationId && n.CreatedDate.Value.Date == DateTime.Now.Date).FirstOrDefault();
                var todaysNDC = (from ndcdetail in dbContext.NDCDetailMsts
                                 where ndcdetail.RegistrationId.ToString() == model.RegistrationId
                                 && DbFunctions.TruncateTime(ndcdetail.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now)
                                 && ndcdetail.IsActive == true
                                 select ndcdetail).FirstOrDefault();
                if (todaysNDC != null)
                {
                    todaysNDC.DepartmentId = model.DepartmentId;
                    todaysNDC.Department = model.ActionType != "Hindi" ? model.Department : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == model.DepartmentId).departmentName;
                    todaysNDC.Sector = model.Sector;
                    todaysNDC.Block = model.Block;
                    todaysNDC.PlotNo = model.PlotNo;
                    todaysNDC.Applicant = model.Applicant;
                    todaysNDC.InstallmentDuesAmount = model.InstallmentPaidAmount;
                    todaysNDC.InstallmentDateInWord = model.InstallmentDateInWord;
                    todaysNDC.InstallmentStatus = model.InstallmentInWord;
                    todaysNDC.InterestDuesAmount = model.InstallmentInterestPaidAmount;
                    todaysNDC.InterestDateInWord = model.InterestDateInWord;
                    todaysNDC.InterestDueStatus = model.InterestInWord;
                    todaysNDC.InstallmentPaidUptoDate = model.InstallmentPaidUpto;
                    todaysNDC.LeaseRentAmount = model.LeaseRentPaidAmount;
                    todaysNDC.LeaseRentDateInWord = model.LeaseRentDateInWord;
                    todaysNDC.LeaseRentStatus = model.LeaseRentInWord;
                    todaysNDC.LeaseRentPaidUptoDate = model.LeaseRentPaidUpto;
                    todaysNDC.ChallanDetail = model.ChallanId;
                    todaysNDC.ChallanAmount = model.ChalanAmount;
                    todaysNDC.LetterNo = model.LetterNo;
                    todaysNDC.BankName = model.BankName;
                    todaysNDC.IsTotalInstallmentPaid = model.IsTotalInstallmentPaid;
                    todaysNDC.IsOneTimeLeasePaid = model.IsOneTimeLeasePaid;
                    todaysNDC.NDCDate = model.NDCDate;
                    //todaysNDC.NDCTemplate = ndcletter;
                    todaysNDC.Remarks = model.Remarks;                 
                    //todaysNDC.LetterType = model.ActionType;
                    //todaysNDC.Address = model.Address;
                    todaysNDC.IsActive = true;
                    //todaysNDC.StatusId = NAStatusId.InProgress;
                    todaysNDC.ValidatedDate = DateTime.Now;
                    todaysNDC.ValidatorId = userInfo.UserID;
                    todaysNDC.ApproverId = model.ApproverId;
                    todaysNDC.ServiceRequestId = model.OnlineReqNo;
                    dbContext.SaveChanges();

                    flag = ReturnType.Updated;
                }
                else
                {
                    var exNDC = dbContext.NDCDetailMsts.Where(n => n.RegistrationId.ToString() == model.RegistrationId).ToList();
                    if (exNDC != null)
                    {
                        exNDC.ForEach(f => f.IsActive = false);
                    }
                    NDCDetailMst ndc = new NDCDetailMst();
                    ndc.RegistrationId = Convert.ToInt32(model.RegistrationId);
                    ndc.DepartmentId = model.DepartmentId;
                    ndc.Department = model.ActionType != "Hindi" ? model.Department : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == model.DepartmentId).departmentName;
                    ndc.Sector = model.Sector;
                    ndc.Block = model.Block;
                    ndc.PlotNo = model.PlotNo;
                    ndc.Applicant = model.Applicant;
                    ndc.InstallmentDuesAmount = model.InstallmentPaidAmount;
                    ndc.InstallmentDateInWord = model.InstallmentDateInWord;
                    ndc.InstallmentStatus = model.InstallmentInWord;
                    ndc.InterestDuesAmount = model.InstallmentInterestPaidAmount;
                    ndc.InterestDateInWord = model.InterestDateInWord;
                    ndc.InterestDueStatus = model.InterestInWord;
                    ndc.InstallmentPaidUptoDate = model.InstallmentPaidUpto;
                    ndc.LeaseRentAmount = model.LeaseRentPaidAmount;
                    ndc.LeaseRentDateInWord = model.LeaseRentDateInWord;
                    ndc.LeaseRentStatus = model.LeaseRentInWord;
                    ndc.LeaseRentPaidUptoDate = model.LeaseRentPaidUpto;
                    ndc.ChallanDetail = model.ChallanId;
                    ndc.ChallanAmount = model.ChalanAmount;
                    ndc.LetterNo = model.LetterNo;
                    ndc.BankName = model.BankName;
                    ndc.IsTotalInstallmentPaid = model.IsTotalInstallmentPaid;
                    ndc.IsOneTimeLeasePaid = model.IsOneTimeLeasePaid;
                    ndc.NDCDate = model.NDCDate;
                    //ndc.NDCTemplate = ndcletter;
                    ndc.Remarks = model.Remarks;
                    ndc.LetterType = model.ActionType;
                    ndc.ApplicantAddress = model.Address;
                    ndc.IsActive = true;
                    ndc.StatusId = NAStatusId.InProgress;
                    ndc.CreatedBy = userInfo.UserID;
                    ndc.CreatedDate = DateTime.Now;
                    ndc.ValidatedDate = DateTime.Now;
                    ndc.ValidatorId = userInfo.UserID;
                    ndc.ApproverId = model.ApproverId;
                    ndc.ServiceRequestId = model.OnlineReqNo;
                    
                    dbContext.NDCDetailMsts.Add(ndc);
                    dbContext.SaveChanges();

                    flag = ReturnType.Saved;
                }
                return flag;
            }
        }


        public int UpdateNDCStatus(NDCVeiwModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var ndc = dbContext.NDCDetailMsts.FirstOrDefault(n => n.Id == model.Id);
                if (ndc != null)
                {
                    ndc.StatusId = model.StatusId;
                    ndc.ApprovalDate = DateTime.Now;
                    ndc.Comment = ndc.Comment +" "+ model.Comment+" by "+userInfo.FirstName+" "+userInfo.LastName;
                    dbContext.SaveChanges();
                    model.Requester = dbContext.UmUserMasters.FirstOrDefault(m=>m.UserRefId==ndc.CreatedBy).FirstName+" "+dbContext.UmUserMasters.FirstOrDefault(m=>m.UserRefId==ndc.CreatedBy).LastName;
                    if (model.StatusId == 1)
                    {
                        if (ndc.ServiceRequestId != null && ndc.ServiceRequestId > 0)
                        {
                            var service = dbContext.Customer_ServiceRequest.FirstOrDefault(m => m.Id == ndc.ServiceRequestId && m.Registration_No == ndc.RegistrationId.ToString() && m.DepartmentId == ndc.DepartmentId);
                            if (service != null)
                            {
                                service.Request_Status = NAStatusId.Completed;
                                service.ApprovalDate = DateTime.Now;
                                service.ApproverId = userInfo.UserID;
                                service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + "."+ "Completed By "+model.Requester;
                                dbContext.SaveChanges();
                            }
                        }
                    }
                    flag = ReturnType.Updated;
                }
                return flag;
            }
        }


        public int CheckRequestIdForNDC(ServiceViewModel model)
        {
            int flag = ReturnType.NotExist;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.RequestId != 0 && model.RequestId != null)
                {
                    var ndcDetail = dbContext.NDCDetailMsts.FirstOrDefault(m => m.ServiceRequestId == model.RequestId && m.IsActive == true);
                    if (ndcDetail != null)
                    {
                        flag = ReturnType.Exist;
                    }
                    else
                    {
                        flag = ReturnType.NotExist;
                    }
                }

                return flag;
            }
        }


        public int RemoveRegistrationIdFromPaidChallan(PaymentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var challan = dbContext.RECEIPT_DETAIL_MASTER.FirstOrDefault(r => r.RECEIPT_ID == model.ReceiptId);
                if (challan != null)
                {
                    challan.RID_NO = null;
                    dbContext.SaveChanges();
                    HttpContext.Current.Session["TempPaidChallan"] = null;
                    flag = ReturnType.Removed;
                }
            }
            return flag;
        }


        public PaymentViewModel GetDuesAmountForNDCByRegistrationId(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from alotment in dbContext.AllotmentMasters
                            join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where alotment.rid == model.RegistrationId
                            select new PaymentViewModel
                            {
                                RegistrationId = alotment.rid,
                                DepartmentId = alotment.departmentId,
                                Department = alotment.DepartmentMst.departmentName,
                                PropertyId = alotment.propertyId.ToString(),

                                Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : (aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName),
                                ApplicantAddress = aplicant.tCorrespondanceAdd,
                                MobileNo = aplicant.tMobileNumber,
                                Email = aplicant.tEmail,

                                SectorId = property.sectorId,
                                SectorName = property.SectorMst.sectorName,
                                BlockId = property.blockId,
                                BlockName = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,

                            }).FirstOrDefault();
                if (data != null)
                {
                    //decimal? totaldues = 0;
                    data.ActionDate = model.ActionDate == null ? DateTime.Now : model.ActionDate;

                    if (model.FilterType == "Leaserent-Installment")
                    {
                        var premium = GetInstallmentDuesById(data);
                        if (premium.IsTotalInstallmentPaid != true)
                        {
                            data.InstallmentAmount = (premium.InstallmentAmount <= 0 || premium.InstallmentAmount == null) ? null : premium.InstallmentAmount;
                            var interest = premium.InstallmentInterest == null ? null : (premium.InstallmentAmount < 0 ? (premium.InstallmentAmount + premium.InstallmentInterest) : premium.InstallmentInterest);
                            //data.InstallmentInterest = (premium.InstallmentInterest <= 0 || premium.InstallmentInterest == null) ? null : premium.InstallmentInterest;
                            data.InstallmentInterest = interest;
                            data.PrincipalAmount = premium.PrincipalAmount;
                            data.DuesUptoDate = data.ActionDate;
                            data.TotalInstallment = (premium.TotalInstallment == null || premium.TotalInstallment < 0) ? 0 : premium.TotalInstallment;
                        }
                        else
                        {
                            data.ToalIstallmentPaidStatus = premium.IsTotalInstallmentPaid == true ? "All" : "";
                            data.IsTotalInstallmentPaid = premium.IsTotalInstallmentPaid;
                            data.TotalInstallment = 0;
                        }
                        var leaseRent = GetLeaseRentDuesById(data);
                        if (leaseRent.ActionTypeId != ReturnType.Paid)
                        {
                            data.LeaseRentPerAnnum = leaseRent.LeaseRentPerAnnum;
                            data.LeaseRentDues = leaseRent.LeaseRentDues;
                            data.LeaseRentInterest = leaseRent.LeaseRentInterest;
                            data.DuesUptoDate = data.ActionDate;
                            data.TotalLeaseRent = (leaseRent.TotalLeaseRent == null || leaseRent.TotalLeaseRent < 0) ? 0 : leaseRent.TotalLeaseRent;
                        }
                        else
                        {
                            data.IsOneTimeLeasePaid = leaseRent.IsOneTimeLeasePaid;
                            data.TotalLeaseRent = 0;
                        }

                        data.TotalDuesAmount = premium.TotalInstallment + leaseRent.TotalLeaseRent;
                        data.Remarks = "Installment/Leaserent Dues as per term & conditions";
                    }
                    
                    //data.TotalDuesAmount = data.TotalDuesAmount == null ? 0 : data.TotalDuesAmount;
                    //Int64 totalAmount = (Int64)Math.Ceiling(data.TotalDuesAmount.Value);
                    //data.AmountInWords = string.IsNullOrEmpty(ApplicationHelper.ConvertNumberIntoWords(totalAmount)) ? string.Empty : (ApplicationHelper.ConvertNumberIntoWords(totalAmount) + " Only");
                    ////return data;
                    //var block = (string.IsNullOrEmpty(data.BlockName) || data.BlockName == Constants.NA || data.BlockName == Constants.BlockNotAvailable) ? string.Empty : (" Block-" + data.BlockName);
                    //data.ActionType = "Leaserent/Installment Dues Payment against Property Sector-" + data.SectorName + block + " Plot-" + data.PlotNo;
                    //data.DemandNoteContent = "You are informed that last date for dues against above mentioned property is " + DateTime.Now.AddDays(10).ToString("dd/MM/yyyy") + ". " + " Kindly pay your dues amount by any bank generated challan in noida.";

                    return data;
                }
                else
                {
                    model.ActionTypeId = ReturnType.NotAllotted;
                    return model;
                }
            }
        }


        public DataSourceResult GetDefaulterList(DataSourceRequest request, DefaulterViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var yr = model.StartDate.Value.Year;
                var mnth = model.StartDate.Value.Month;
                var days = DateTime.DaysInMonth(yr, mnth);
                model.EndDate = Convert.ToDateTime(days +model.Month);

                var data = (from def in dbContext.AllDefaulterLists
                            join allot in dbContext.ViewAllPropertyDetails on def.RegistrationId equals allot.rid
                            where (model.DepartmentId == null || def.DepartmentId == model.DepartmentId)
                                && (model.StartDate == null || (def.CalculationDate <= model.EndDate && def.CalculationDate >= model.StartDate))
                                && def.Is_Active == true && DepartmentList.Contains(def.DepartmentId)
                            select new DefaulterViewModel
                            {
                                Id = def.Id,
                                RegistrationId = def.RegistrationId,
                                DepartmentId=def.DepartmentId,
                                Department=dbContext.DepartmentMsts.FirstOrDefault(m=>m.departmentId==model.DepartmentId).departmentName,
                                LeaserentPaidUpto=def.LeaserentPaidupto,
                                LeaserentInterestAmount = def.LeaserentInterestAmount == null ? 0 : def.LeaserentInterestAmount,
                                LeaserentDuesAmount = def.LeaserentDuesAmount == null ? 0 : def.LeaserentDuesAmount,
                                InstallmentAmount = def.InstallmentAmount == null ? 0 : def.InstallmentAmount,
                                InstallmentDuesAmount = def.InstallmentDuesAmount == null ? 0 : def.InstallmentDuesAmount,
                                InstallmentNumber=def.NoOfInstallment,
                                InstallmentPaidUpto = def.InstPaidUpto,
                                PenalInstallmentInterestAmount = def.PenalInstallmentIntrestAmount == null ? 0 : def.PenalInstallmentIntrestAmount,
                                CalculationDate=def.CalculationDate,
                                IsActive=def.Is_Active,
                                CreatedDate=def.CreatedDate,
                                CreatedBy=def.CreatedBy,
                                AllotmentDate=dbContext.AllotmentMasters.FirstOrDefault(m=>m.rid==def.RegistrationId).allotmentDate,
                                AreaRange=allot.totalArea,
                                Sector=allot.sectorName,
                                Block = allot.blockName,
                                PlotNo = allot.propertyNo,
                                TotalInstallmentDues = (def.InstallmentAmount == null ? 0 : def.InstallmentAmount) + (def.PenalInstallmentIntrestAmount == null ? 0 : def.PenalInstallmentIntrestAmount),
                                TotalLeaserentDues = (def.LeaserentDuesAmount == null ? 0 : def.LeaserentDuesAmount) + (def.LeaserentInterestAmount == null ? 0 : def.LeaserentInterestAmount),
                                TotalDues = (def.LeaserentDuesAmount == null ? 0 : def.LeaserentDuesAmount) + (def.LeaserentInterestAmount == null ? 0 : def.LeaserentInterestAmount) + (def.InstallmentAmount == null ? 0 : def.InstallmentAmount) + (def.PenalInstallmentIntrestAmount == null ? 0 : def.PenalInstallmentIntrestAmount)
                            });
                return data.ToDataSourceResult(request);
            }
        }
    }
}
