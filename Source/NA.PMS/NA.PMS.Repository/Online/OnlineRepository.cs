using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NA.PMS.Model;
using NA.PMS.Common;
using System.IO;
using System.Configuration;
using NoidaAuthority.PMS.Common;
using System.Transactions;
using System.Data.Entity;
using com.fss.plugin.bob;

namespace NA.PMS.Repository
{
    public class OnlineRepository : IOnlineRepository
    {
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public OnlineRepository()
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

        public DataSourceResult GetOnlineApplicationsForAdmin(DataSourceRequest request, OnlineFormViewModel modal)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                string userRole = userInfo.RoleMaster.RoleType;

                int MoveToOSD = Convert.ToInt32(strOnlineApplicationProcess.MoveToOSD);
                int Scrutiny = Convert.ToInt32(strOnlineApplicationProcess.Scrutiny);
                int Draw = Convert.ToInt32(strOnlineApplicationProcess.Draw);
                int ApprovalCEO = Convert.ToInt32(strOnlineApplicationProcess.ApprovalCEO);

                var list = (from oam in dbContext.OnlineApplicationDetails
                            from sch in dbContext.SchemeMsts.Where(s => s.schemeId == oam.schemeId).DefaultIfEmpty()
                            from deptt in dbContext.DepartmentMsts.Where(d => d.departmentId == oam.departmentId).DefaultIfEmpty()
                            where DepartmentList.Contains(oam.departmentId)
                            && (modal.Id == null || oam.onlineapplicationId == modal.Id)
                            && (modal.DepartmentId == null || oam.departmentId == modal.DepartmentId)
                            && (modal.SchemeId == null || oam.schemeId == modal.SchemeId)
                            select new OnlineFormViewModel
                            {
                                Id = oam.onlineapplicationId,
                                ApplicationFormId = oam.onlineapplicationId,
                                SchemeName = sch.schemeName,
                                Department = deptt.departmentName,
                                FirstName = string.IsNullOrEmpty(oam.CompanyName) ? oam.firstName + " " + (!string.IsNullOrEmpty(oam.middleName) ? oam.middleName + " " + oam.lastName : oam.lastName) : oam.CompanyName,
                                Gender = oam.gender,
                                DOB = oam.dateOfBirth,
                                FormStatus = oam.IsSubmited == null ? Status.Rejected : (oam.IsSubmited == true ? Status.Accepted : Status.InProgress),
                                IsDeleted = oam.isActive == null ? "" : (oam.isActive == true ? Status.Accepted : Status.Rejected),
                                TotalAmount = oam.TotalAmount,
                                SubmitDate = oam.createdDate,
                                AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == oam.area && m.IsActive == true).FirstOrDefault().floorName,
                                ApplicantType = oam.gender == "Company" ? "Company" : "Individual",
                                AmountPaidDate = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().EntryDate,
                                PaymentModeId = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType,
                                PaymentMode = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().productinfo,
                                ChallanBank = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().issuing_bank,
                                //ChallanBank = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.SchemeBankTrans.FirstOrDefault(m => m.schemeId == oam.schemeId && m.IsActive == true && m.bankId.ToString() == (dbContext.OnlineApplicationDetails_trans.FirstOrDefault(i => i.ServiceRefId == oam.onlineapplicationId && i.ServiceType == 3).GetwayName)).BankMst.bankName) : "--",
                                //PayType = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? "Online" : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? "Offline (RTGS/NEFT)" : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 4 ? "Previous Challan" : "--")),
                                //ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "Paid" : "Not Paid") : "--"),
                                ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "Paid" : "Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 4 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "PC Paid" : "PC Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 5 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "SWP Paid" : "SWP Not Paid") : "--"))),
                                StatusId = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status,
                                ApplicationFormType = !string.IsNullOrEmpty(oam.FormCategory) ? (oam.FormCategory) : "--",
                                ExpansionType = !string.IsNullOrEmpty(oam.FormCategory) ? ((oam.FormCategory == Constants.ApplicationFormTypeEx) ? oam.FormSubCategory : "--") : "--",
                                ProcessType = oam.OnlineApplicationProcessDetails.FirstOrDefault() != null ? (oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == MoveToOSD) != null ? "Move To OSD" : oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == Scrutiny) != null ? "Scrunity" : oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == ApprovalCEO) != null ? "Approval for CEO" : oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == Draw) != null ? "Draw" : string.Empty) : string.Empty
                            });

                if (modal.PayType == PaymentStatus.NotPaid)
                {
                    list = list.Where(m => m.ChallanStatus != "Not Paid" && m.ChallanStatus != "--");
                }
                if (modal.PayType == PaymentStatus.Paid)
                {
                    list = list.Where(m => m.ChallanStatus != "Not Paid" && m.ChallanStatus != "--");
                }
                return list.ToDataSourceResult(request);
            }
        }

        #region scheme industrial 2017

        public DataSourceResult GetOnlineApplications(DataSourceRequest request, OnlineFormViewModel modal)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                string userRole = userInfo.RoleMaster.RoleType;

                var loginUserDepartment = (from userMaster in dbContext.UmUserMasters
                                           join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                           where userMaster.UserRefId == userInfo.UserID
                                           select departmentTrans.DepartmentId).ToList();
                if (modal.SchemeId == null)
                {
                    modal.SchemeId = Convert.ToInt32(ConfigurationManager.AppSettings["CurrentSchemeId"]);
                }
                if (userRole == Constants.Admin || userInfo.UserID == 1222)
                {
                    int MoveToOSD = Convert.ToInt32(strOnlineApplicationProcess.MoveToOSD);
                    int Scrutiny = Convert.ToInt32(strOnlineApplicationProcess.Scrutiny);
                    int Draw = Convert.ToInt32(strOnlineApplicationProcess.Draw);
                    int ApprovalCEO = Convert.ToInt32(strOnlineApplicationProcess.ApprovalCEO);
                    var Online_trans = (from oam in dbContext.OnlineApplicationDetails
                                        from sch in dbContext.SchemeMsts.Where(s => s.schemeId == oam.schemeId).DefaultIfEmpty()
                                        from deptt in dbContext.DepartmentMsts.Where(d => d.departmentId == oam.departmentId).DefaultIfEmpty()
                                        where loginUserDepartment.Contains(oam.departmentId)
                                        && (modal.Id == null || oam.onlineapplicationId == modal.Id)
                                        && (modal.DepartmentId == null || oam.departmentId == modal.DepartmentId)
                                        && (modal.SchemeId == null || oam.schemeId == modal.SchemeId)
                                        select new OnlineFormViewModel
                                        {
                                            Id = oam.onlineapplicationId,
                                            ApplicationFormId = oam.onlineapplicationId,
                                            SchemeName = sch.schemeName,
                                            Department = deptt.departmentName,
                                            FirstName = string.IsNullOrEmpty(oam.CompanyName) ? oam.firstName + " " + (!string.IsNullOrEmpty(oam.middleName) ? oam.middleName + " " + oam.lastName : oam.lastName) : oam.CompanyName,
                                            Gender = oam.gender,
                                            DOB = oam.dateOfBirth,
                                            FormStatus = oam.IsSubmited == null ? Status.Rejected : (oam.IsSubmited == true ? Status.Accepted : Status.InProgress),
                                            IsDeleted = oam.isActive == null ? "" : (oam.isActive == true ? Status.Accepted : Status.Rejected),
                                            TotalAmount = oam.TotalAmount,
                                            SubmitDate = oam.createdDate,
                                            AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == oam.area && m.IsActive == true).FirstOrDefault().floorName,
                                            ApplicantType = oam.gender == "Company" ? "Company" : "Individual",
                                            AmountPaidDate = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().EntryDate,
                                            ChallanBank = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.SchemeBankTrans.FirstOrDefault(m => m.schemeId == oam.schemeId && m.IsActive == true && m.bankId.ToString() == (dbContext.OnlineApplicationDetails_trans.FirstOrDefault(i => i.ServiceRefId == oam.onlineapplicationId && i.ServiceType == 3).GetwayName)).BankMst.bankName) : "--",
                                            PayType = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? "Online" : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? "Offline (RTGS/NEFT)" : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 4 ? "Previous Challan" : "--")),
                                            //ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "Paid" : "Not Paid") : "--"),
                                            ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "Paid" : "Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 4 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "PC Paid" : "PC Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 5 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "SWP Paid" : "SWP Not Paid") : "--"))),
                                            ApplicationFormType = !string.IsNullOrEmpty(oam.FormCategory) ? (oam.FormCategory) : "--",
                                            ExpansionType = !string.IsNullOrEmpty(oam.FormCategory) ? ((oam.FormCategory == Constants.ApplicationFormTypeEx) ? oam.FormSubCategory : "--") : "--",
                                            ProcessType = oam.OnlineApplicationProcessDetails.FirstOrDefault() != null ? (oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == MoveToOSD) != null ? "Move To OSD" : oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == Scrutiny) != null ? "Scrunity" : oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == ApprovalCEO) != null ? "Approval for CEO" : oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == Draw) != null ? "Draw" : string.Empty) : string.Empty
                                        });

                    if (modal.PayType == "2")
                    {
                        Online_trans = Online_trans.Where(m => m.ChallanStatus != "Not Paid" && m.ChallanStatus != "--");
                    }
                    if (modal.PayType == PaymentStatus.Paid)
                    {
                        Online_trans = Online_trans.Where(m => m.ChallanStatus != "Not Paid" && m.ChallanStatus != "--");
                    }
                    return Online_trans.ToDataSourceResult(request);
                }
                else
                {
                    int? area = (userInfo.OptionalId == null || userInfo.OptionalId == 0) ? 0 : userInfo.OptionalId;
                    var Online = (from oam in dbContext.OnlineApplicationDetails
                                  join oad_t in dbContext.OnlineApplicationDetails_trans on oam.onlineapplicationId equals oad_t.ServiceRefId
                                  join sch in dbContext.SchemeMsts on oam.schemeId equals sch.schemeId
                                  join deptt in dbContext.DepartmentMsts on oam.departmentId equals deptt.departmentId
                                  from floor in dbContext.FloorMsts.Where(f => f.floorId.ToString() == oam.area).DefaultIfEmpty()
                                  where ((oam.isActive == true && oad_t.status == 1) || oad_t.ServiceType == 4)
                                  && loginUserDepartment.Contains(oam.departmentId)
                                  && (modal.Id == null || oam.onlineapplicationId == modal.Id)
                                  && (modal.DepartmentId == null || oam.departmentId == modal.DepartmentId)
                                  && (modal.SchemeId == null || oam.schemeId == modal.SchemeId)
                                  && (area == 0 || oam.area == area.ToString())
                                  select new OnlineFormViewModel
                                  {
                                      Id = oam.onlineapplicationId,
                                      ApplicationFormId = oam.onlineapplicationId,
                                      SchemeName = sch.schemeName,
                                      Department = deptt.departmentName,
                                      FirstName = string.IsNullOrEmpty(oam.CompanyName) ? oam.firstName + " " + (!string.IsNullOrEmpty(oam.middleName) ? oam.middleName + " " + oam.lastName : oam.lastName) : oam.CompanyName,
                                      Gender = oam.gender,
                                      DOB = oam.dateOfBirth,
                                      FormStatus = oam.IsSubmited == null ? Status.Rejected : (oam.IsSubmited == true ? Status.Accepted : Status.InProgress),
                                      AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == oam.area && m.IsActive == true).FirstOrDefault().floorName,
                                      TotalAmount = (decimal)oam.TotalAmount,//Amount Paid
                                      SubmitDate = oam.createdDate,
                                      AmountPaidDate = oad_t.EntryDate,
                                      ApplicantType = oam.gender == "Company" ? "Company" : "Individual",
                                      PayType = oad_t.ServiceType == 1 ? "Online" : (oad_t.ServiceType == 3 ? "Offline (RTGS/NEFT)" : "--"),
                                      ChallanStatus = oad_t.ServiceType == 3 ? (oad_t.status == 0 ? ((!string.IsNullOrEmpty(oad_t.TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (oad_t.ServiceType == 1 ? (oad_t.status == 1 ? "Paid" : "Not Paid") : "--"),
                                      ApplicationFormType = !string.IsNullOrEmpty(oam.FormCategory) ? (oam.FormCategory) : "--",
                                      ExpansionType = !string.IsNullOrEmpty(oam.FormCategory) ? ((oam.FormCategory == Constants.ApplicationFormTypeEx) ? oam.FormSubCategory : "--") : "--"
                                  });
                    return Online.ToDataSourceResult(request);
                }
            }
        }

        public OnlineApplicationDetailsTrans GetOnlineApplicationReceipt(OnlineApplicationDetailsTrans ObjOnlineApplicationDetailsTrans)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                OnlineApplicationDetailsTrans ObjTransReciept = new OnlineApplicationDetailsTrans();
                var onlineApplicationDetails = GetOnlineApplicationFormById(ObjOnlineApplicationDetailsTrans.OnlineApplicationId);
                if ((bool)onlineApplicationDetails.IsApplicationFeePaid)
                {
                    ObjTransReciept = (from oad_t in dbContext.OnlineApplicationDetails_trans
                                       where oad_t.status == 1
                                        && (ObjOnlineApplicationDetailsTrans.OnlineApplicationId == null || oad_t.ServiceRefId == ObjOnlineApplicationDetailsTrans.OnlineApplicationId)
                                       select new OnlineApplicationDetailsTrans
                                       {
                                           StatusName = oad_t.TranStatus == Constants.Success ? "Success" : "Failure",
                                           Amount = oad_t.Amount,
                                           Txnid = oad_t.txnid,
                                           bank_ref_num = oad_t.bank_ref_num,
                                           card_type = oad_t.card_type,
                                           name_on_card = oad_t.name_on_card
                                       }).FirstOrDefault();
                }
                ObjTransReciept.objOnlineFormViewModel = onlineApplicationDetails;
                return ObjTransReciept;
            }
        }

        //Reject Application
        public Boolean RejectApplication(int Id)
        {
            Boolean IsRejectedStatus = false;
            try
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    var IsRejected = dbContext.OnlineApplicationDetails.Where(us => us.onlineapplicationId == Id).FirstOrDefault();
                    if (IsRejected != null)
                    {
                        IsRejected.isActive = IsRejected.isActive != true ? true : false;
                        IsRejectedStatus = (Boolean)IsRejected.isActive;
                        dbContext.SaveChanges();
                        IsRejectedStatus = true;
                        string body = string.Empty;
                        if (IsRejectedStatus)
                        {
                            body = string.Format(NAMessages.AppRejected, IsRejected.onlineapplicationId);
                        }
                        else { body = string.Format(NAMessages.OfflineAppReqSuccess, IsRejected.onlineapplicationId); }
                        if (!string.IsNullOrEmpty(body))
                        {
                            if (!string.IsNullOrEmpty(IsRejected.email)) { ApplicationHelper.SendEmail(IsRejected.email, "Online Application Request Status", body); }
                            if (!string.IsNullOrEmpty(IsRejected.mobileNumberP2)) { ApplicationHelper.SendSMS(IsRejected.mobileNumberP2, body); }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return IsRejectedStatus;
        }

        //Update Challan Application Status
        public Boolean UpdateChallanStatus(int Id, int PayType)
        {
            Boolean Status = false;
            try
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    var AppDetails = dbContext.OnlineApplicationDetails.Where(us => us.onlineapplicationId == Id && us.isActive == true).FirstOrDefault();
                    if (AppDetails != null)
                    {
                        //check service type 
                        int ServiceType = 0;
                        if (PayType == Constants.offlinePrevoiusChallanApplicationPayment)
                        {
                            ServiceType = Constants.offlinePrevoiusChallanApplicationPayment;
                        }

                        else { ServiceType = Constants.offlineApplicationPayment; }
                        var Details_trans = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == AppDetails.onlineapplicationId && m.ServiceType == ServiceType && m.status == 0).FirstOrDefault();
                        if (Details_trans != null)
                        {
                            Details_trans.status = 1;
                            Details_trans.TranStatus = 1;
                            dbContext.SaveChanges();
                            Status = true;
                            if (Status)
                            {
                                string body = string.Empty;
                                string message = string.Empty;
                                if (PayType == Constants.offlineApplicationPayment || PayType == Constants.offlinePrevoiusChallanApplicationPayment)
                                {
                                    body = string.Format(NAMessages.OnlineApplicationChallanStatus, AppDetails.onlineapplicationId);
                                    message = "Online Application Challan Validated";
                                }

                                if (!string.IsNullOrEmpty(body))
                                {
                                    if (!string.IsNullOrEmpty(AppDetails.email)) { ApplicationHelper.SendEmail(AppDetails.email, message, body); }
                                    if (!string.IsNullOrEmpty(AppDetails.mobileNumberP2)) { ApplicationHelper.SendSMS(AppDetails.mobileNumberP2, body); }
                                }

                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Status;
        }

        public DataSourceResult GetChecklistDocumentsForOnlineApplication(DataSourceRequest request, int? schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //int i = 0;
                var docs = (from doc in dbContext.OnlineCheckListMasters
                            where doc.IsActive == true //&& doc.schemeId == schemeId
                            select new OnlineDocumentViewModel
                            {
                                Id = doc.CheckListId,
                                DocumentId = doc.CheckListId,
                                DocumentName = doc.CheckListName
                            }).ToList();
                //if (docs.Count > 0)
                //{
                //    allDocs = (from d in docs select new ChecklistDocuments { Id = d.Id, DocName = d.DocName, SNo = ++i }).ToList();
                //}
                //return allDocs.ToDataSourceResult(request);
                return docs.ToDataSourceResult(request);
            }
        }

        public OnlineFormViewModel GetApplicationFeeAndCharges(int? schemeId, int? departmentId, int? propertyTypeId, int? areaTypeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var schemecost = dbContext.SchemeCostTrans.FirstOrDefault(c => c.schemeId == schemeId && c.departmentId == departmentId && c.propertyTypeId == propertyTypeId && c.floorId == areaTypeId);
                var schemecost = dbContext.SchemeCostTrans.FirstOrDefault(c => c.schemeId == schemeId && c.departmentId == departmentId && c.floorId == areaTypeId);
                var online = dbContext.OnlineAreawithRegisandProcfees.FirstOrDefault(r => r.schemeId == schemeId);
                OnlineFormViewModel model = new OnlineFormViewModel();
                if (schemecost != null)
                {
                    model.EarnestMoney = schemecost.earnestMoney;
                }
                if (online != null)
                {
                    model.ApplicationFee = online.ApplicationFee;
                    model.ProcessingCharge = online.ProcessingCharge;
                }

                //var model = (from cost in dbContext.SchemeCostTrans
                //             join scme in dbContext.SchemeMsts on cost.schemeId equals scme.schemeId   
                //             join ofee in dbContext.OnlineAreawithRegisandProcfees on cost.schemeId equals ofee.schemeId
                //             where cost.schemeId == schemeId && cost.departmentId == departmentId && cost.propertyTypeId == propertyTypeId && cost.floorId == areaTypeId
                //             select new OnlineFormViewModel
                //             {
                //                 EarnestMoney = schemecost.earnestMoney,
                //                 ApplicationFee = ofee.ApplicationFee,
                //                 ProcessingCharge = ofee.ProcessingCharge
                //             }).FirstOrDefault();

                return model;
            }
        }

        public OnlineFormViewModel GetOnlineApplicationFeeAndCharges(OnlineFormViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var scheme = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId == model.SchemeId);
                var schemecost = dbContext.SchemeCostTrans.FirstOrDefault(c => c.schemeId == model.SchemeId && c.departmentId == model.DepartmentId && c.floorId == model.AreaRangeTypeId);
                var online = dbContext.OnlineAreawithRegisandProcfees.FirstOrDefault(r => r.schemeId == model.SchemeId);
                //OnlineFormViewModel model = new OnlineFormViewModel();
                if (schemecost != null)
                {
                    model.ApplicationFee = scheme.FormFee;
                    model.FormFeeGST = scheme.FormCGST + scheme.FormSGST;
                    model.ProcessingCharge = scheme.ProcessingFee;
                    model.ProcessingChargeGST = scheme.ProcessingCGST + scheme.ProcessingSGST;
                    model.EarnestMoney = schemecost.earnestMoney;
                }
                if (online != null)
                {
                    model.ApplicationFee = online.ApplicationFee;
                    model.ProcessingCharge = online.ProcessingCharge;
                }

                return model;
            }
        }

        public int SaveOnlineApplicationForm(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            int flag = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                //using (TransactionScope scope = new TransactionScope())
                //{
                //    try
                //    {
                        OnlineApplicationDetail application = new OnlineApplicationDetail();
                        model.Id = model.Id == null ? 0 : model.Id;
                        var detail = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == model.Id);
                        if (detail == null)
                        {
                            if (model.ApplicantType == Constants.Individual)
                            {
                                application.firstName = model.FirstName;
                                application.middleName = model.MiddleName;
                                application.lastName = model.LastName;
                                application.fatherHusbandName = model.FatherName;
                                application.motherName = model.MotherName;
                                application.gender = model.Gender;
                                application.religionId = model.ReligionId;
                                application.occupationId = model.OccupationId;
                                //application.quotaId = model.CategoryId;
                                application.quotaId = Constants.General;
                                application.marritalStatus = model.MaritalStatus;
                                application.dateOfBirth = model.DOB;
                            }
                            else
                            {
                                application.CompanyName = model.Applicant;
                                application.signingAuthority = model.SigningAuthority;
                                application.registeredOffice = model.PermanentAddress;
                                application.CompanyType = model.CompanyTypeId;
                                application.gender = Constants.Company;
                                application.fatherHusbandName = model.ApplicantMaster;
                                application.quotaId = model.SigningAuthorityId;
                            }
                            application.CompanyType = model.CompanyTypeId;
                            application.schemeId = model.SchemeId;
                            application.departmentId = model.DepartmentId;
                            application.permanentAdd = model.PermanentAddress;
                            application.correspondanceAdd = model.CorrespondingAddress;
                            application.area = model.AreaRangeId;
                            application.mobileNumberP2 = model.MobileNumber;
                            application.phoneNumberP2 = model.PhoneNumber;
                            application.faxNumberP2 = model.FaxNumber;
                            application.email = model.Email;
                            application.annualIncome = model.AnnualIncome;
                            application.ApplicationFee = model.ApplicationFee;
                            application.FormCGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.FormSGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.ProcessingCharge = model.ProcessingCharge;
                            application.ProcessingCGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.ProcessingSGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.EarnestMoney = model.EarnestMoney;
                            application.TotalAmount = model.TotalAmount;
                            application.pan = model.PanNumber;
                            application.AadharNumber = model.AadharNumber;
                            //application.faxNumberP2 = model.ApplicantGSTNumber;
                            application.GSTNO = model.ApplicantGSTNumber;
                            application.ApplicationDate = DateTime.Now;

                            application.RefundBankId = model.RefundBankId;
                            application.RefundInfaverof = model.RefundInfaverof;
                            application.RefundaccountNo = model.RefundAccountNo;

                            application.PropertyTypeID = model.PropertyTypeId;
                            application.Sector = model.Sector;
                            application.BranchName = model.BranchName;
                            application.IFSCCode = model.IFSCCode;

                            application.FormCategory = model.ApplicationFormType;
                            if (model.ApplicationFormType == "Expansion")
                            {
                                application.FormSubCategory = model.ExpansionType;
                                application.RentingDate = model.LetterDate;
                                application.RentingLetterNo = model.LetterCode;
                                application.RentingPropertyNo = model.PropertyNo;
                            }

                            application.PropertyNo = model.PropertyNo;
                            application.ExistingPropertyNo = model.ExistingProperty;
                            application.AllotmentDate = model.AllottmentDate;
                            application.DispatchDate = model.DispatchDate;

                            application.createdBy = "Online";
                            application.createdDate = DateTime.Now;
                            application.Online_offline = (model.FormType == null || model.FormType == "Online") ? "Y" : "N";
                            application.isActive = true;
                            application.IsSubmited = false;

                            string PassWord = string.Empty;
                            if (OnlineSchemeType.Transport == model.SchemeType || OnlineSchemeType.OpenEnded == model.SchemeType || NASchemeType.IndustriaScheme == model.SchemeType || model.SchemeType == "Industrial Scheme" || model.SchemeType == NASchemeType.InstitutionalScheme)
                            {
                                PassWord = ApplicationHelper.GeneratePassWordForScheme();
                                application.Userpassword = PassWord.ToMD5HashForPasswordPIS();
                                if (OnlineSchemeType.Transport == model.SchemeType)
                                {
                                    if (!string.IsNullOrEmpty(model.PreviousFormNo))
                                    {
                                        //application.Comment = model.PreviousFormNo;
                                        application.PreviousFormNo = model.PreviousFormNo;
                                    }
                                }
                            }

                            dbContext.OnlineApplicationDetails.Add(application);
                            dbContext.SaveChanges();

                            //Method to save NIC Get Values.
                            if (model.BasicDetailsGetModel != null)
                            {
                                if (!string.IsNullOrEmpty(model.BasicDetailsGetModel.Table.Control_ID))
                                {
                                    model.BasicDetailsGetModel.Table.OnlineApplicationId = application.onlineapplicationId;
                                    model.BasicDetailsGetModel.Table.departmentId = (int)application.departmentId;
                                    model.BasicDetailsGetModel.Table.SchemeId = (int)application.schemeId;
                                    var Basicdetail = dbContext.NICsingalwindowSystems.FirstOrDefault(c => c.onlineapplicationId == model.BasicDetailsGetModel.Table.OnlineApplicationId);
                                    if (Basicdetail == null)
                                    {
                                        NICsingalwindowSystem objNICsingalwindowSystem = new NICsingalwindowSystem();
                                        objNICsingalwindowSystem = MapNICsingalwindowSystemTable(model.BasicDetailsGetModel);
                                        objNICsingalwindowSystem.ServiceID = objNICsingalwindowSystem.ServiceID == null ? model.NICServiceId : objNICsingalwindowSystem.ServiceID;
                                        dbContext.NICsingalwindowSystems.Add(objNICsingalwindowSystem);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }

                            int id = application.onlineapplicationId;

                            model.Id = id;
                            model.ApplicationFormId = id;
                            SaveDocumentsForApplicationForm(model, files, userImage, signatureImage);
                            //save director details
                            if (OnlineSchemeType.Transport == model.SchemeType)
                            {
                                UpdateDirectorDetailsForOpenSchemeForm(application.onlineapplicationId);
                            }
                            //string message = string.Format(NAMessages.OnlineApplicationSubmitted, id);
                            //ApplicationHelper.SendEmail(model.Email, "Registration Form", message);

                            //ApplicationHelper.SendSMS(model.MobileNumber, message);
                            if (OnlineSchemeType.Transport == model.SchemeType || OnlineSchemeType.OpenEnded == model.SchemeType || model.SchemeType == NASchemeType.IndustriaScheme || model.SchemeType == "Industrial Scheme")
                            {
                                string mobileMessage = string.Format(NAMessages.PIS_Registration_Activation, id, PassWord);
                                string emailMessage = string.Format(NAMessages.PIS_Registration_Activation, id, PassWord);
                                if (model.MobileNumber != null && model.MobileNumber != "") ApplicationHelper.SendSMS(model.MobileNumber, mobileMessage);
                                if (model.Email != null && model.Email != "") ApplicationHelper.SendEmail(model.Email, "OnlineForm", emailMessage);
                            }
                            //HttpContext.Current.Session["OpenScheme"] = "123";

                            flag = ReturnType.Success;
                        }
                        else
                        {
                            UpdateOnlineApplicationForm(model, files, userImage, signatureImage);
                        }
                        //scope.Complete();
                //    }
                //    catch (Exception ex)
                //    {
                //        return 0;
                //        scope.Dispose();
                //    }
                //}
            }
            return model.Id.Value;
        }

        private int SaveDocumentsForApplicationForm(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            int flag = 0; model.Id = model.ApplicationFormId;
            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id)))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id));
            }

            if (files != null && files.Count() > 0)
            {
                if (model.StrDocList != null && model.StrDocList.Length > 0)
                {
                    string[] docsIds = model.StrDocList[0].Split(','); int count = 0;
                    string documents = "Documents";
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + documents)))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + documents));
                    }
                    foreach (var file in files)
                    {
                        if (file != null)
                        {
                            string extension = Path.GetExtension(file.FileName);
                            var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + documents + "/" + model.Id + "-" + docsIds[count] + extension);
                            using (var dbContext = new NoidaPMSEntities())
                            {
                                OnlineCheckLisTran objOnlineCheckLisTran = new OnlineCheckLisTran();
                                objOnlineCheckLisTran.CheckListId = Convert.ToInt32(docsIds[count]);
                                objOnlineCheckLisTran.onlineapplicationId = (int)model.Id;
                                objOnlineCheckLisTran.FileNAme = (model.Id + "-" + docsIds[count] + extension).ToString();
                                objOnlineCheckLisTran.CreatedDate = DateTime.Now.Date;
                                objOnlineCheckLisTran.CreatedBy = 0;
                                objOnlineCheckLisTran.isActive = true;
                                dbContext.OnlineCheckLisTrans.Add(objOnlineCheckLisTran);
                                dbContext.SaveChanges();
                            }
                            file.SaveAs(fileSavePath);
                            count++;
                            flag = ReturnType.Success;
                        }
                    }
                    //30-11-2017 shatrughna
                    string message = string.Format(NAMessages.UploadDocumentSuccess, model.Id);
                    if (!string.IsNullOrEmpty(model.MobileNumber)) { ApplicationHelper.SendSMS(model.MobileNumber, message); }
                }
            }
            if (userImage != null && userImage.ContentLength > 0)
            {
                string picture = "Pictures";
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture)))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture));
                }
                else
                {
                    var dirpath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture);
                    string[] fils = Directory.GetFiles(dirpath);
                    if (fils != null && fils.Count() > 0)
                    {
                        foreach (var fl in fils)
                        {
                            string path1 = dirpath + "\\" + model.Id + "-userphoto" + ".jpg";
                            if (fl == path1) { File.Delete(fl); }
                        }
                    }
                }
                string extension = Path.GetExtension(userImage.FileName);
                var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture + "/" + model.Id + "-userphoto" + extension);
                userImage.SaveAs(fileSavePath);
                flag = ReturnType.Success;
            }
            if (signatureImage != null && signatureImage.ContentLength > 0)
            {
                string picture = "Pictures";
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture)))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture));
                }
                else
                {
                    string fileExtn = "*signature";
                    var dirpath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture);
                    var fileList = new DirectoryInfo(dirpath).GetFileSystemInfos(fileExtn);
                    //string[] fils = Directory.GetFiles(dirpath);
                    if (fileList != null && fileList.Count() > 0)
                    {
                        foreach (var fl in fileList)
                        {
                            fl.Delete();
                        }
                    }
                }
                string extension = Path.GetExtension(userImage.FileName);
                var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture + "/" + model.Id + "-signature" + extension);
                signatureImage.SaveAs(fileSavePath);
                flag = ReturnType.Success;
            }

            return flag;
        }

        public OnlineFormViewModel GetOnlineApplicationFormById(int? id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                OnlineFormViewModel applicant = new OnlineFormViewModel();
                var detail = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == id && c.isActive == true);
                if (detail != null)
                {
                    if (detail.RefundBankId != null)
                    {
                        applicant = (from appl in dbContext.OnlineApplicationDetails
                                     join dept in dbContext.DepartmentMsts on appl.departmentId equals dept.departmentId
                                     join scem in dbContext.SchemeMsts on appl.schemeId equals scem.schemeId
                                     join bank in dbContext.BankMsts on appl.RefundBankId equals bank.bankId
                                     join area in dbContext.FloorMsts on appl.area equals area.floorId.ToString()
                                     where appl.onlineapplicationId == id
                                     select new OnlineFormViewModel
                                     {
                                         Id = appl.onlineapplicationId,
                                         ApplicationFormId = appl.onlineapplicationId,
                                         SchemeId = scem.schemeId,
                                         SchemeName = scem.schemeName,
                                         SchemeType = scem.SchemeTypeMst.SchemeTypeDesc,
                                         DepartmentId = dept.departmentId,
                                         Department = dept.departmentName,
                                         Applicant = appl.gender == "Company" ? appl.CompanyName : appl.firstName + " " + appl.middleName + " " + appl.lastName,
                                         SigningAuthority = appl.signingAuthority,
                                         ApplicantType = appl.gender == "Company" ? "Company" : "Individual",
                                         CompanyTypeId = appl.CompanyType,
                                         CompanyType = dbContext.Common_Config.Where(c => c.Id == appl.CompanyType).Select(c => c.Name).FirstOrDefault(),
                                         FirstName = appl.firstName,
                                         MiddleName = appl.middleName,
                                         LastName = appl.lastName,
                                         Gender = appl.gender,
                                         MaritalStatus = appl.marritalStatus,
                                         DOB = appl.dateOfBirth,
                                         FatherName = (!string.IsNullOrEmpty(appl.fatherHusbandName)) ? appl.fatherHusbandName : "NA",
                                         ApplicantMaster = (!string.IsNullOrEmpty(appl.fatherHusbandName)) ? appl.fatherHusbandName : "NA",
                                         MotherName = appl.motherName,
                                         OccupationId = appl.occupationId,
                                         Occupation = dbContext.OccupationMsts.Where(c => c.occupationId == appl.occupationId).Select(s => s.occupation).FirstOrDefault(),
                                         ReligionId = appl.religionId,
                                         Religion = dbContext.ReligionMsts.Where(r => r.religionId == appl.religionId).Select(r => r.religion).FirstOrDefault(),
                                         CategoryId = appl.quotaId,
                                         CategoryName = dbContext.QuotaMsts.Where(c => c.quotaId == appl.quotaId).Select(q => q.quotaName).FirstOrDefault(),
                                         MobileNumber = appl.mobileNumberP2,
                                         PhoneNumber = appl.phoneNumberP2,
                                         FaxNumber = appl.faxNumberP2,
                                         //ApplicantGSTNumber = appl.faxNumberP2,
                                         ApplicantGSTNumber = appl.GSTNO,
                                         PermanentAddress = appl.permanentAdd,
                                         CorrespondingAddress = appl.correspondanceAdd,
                                         RegisteredOffice = appl.registeredOffice,
                                         Email = appl.email,
                                         AnnualIncome = appl.annualIncome,
                                         EarnestMoney = appl.EarnestMoney,
                                         ApplicationFee = appl.ApplicationFee,
                                         FormFeeSGST = appl.FormSGST,
                                         FormFeeCGST = appl.FormCGST,
                                         ProcessingCharge = appl.ProcessingCharge,
                                         ProcessingCGST = appl.ProcessingCGST,
                                         ProcessingSGST = appl.ProcessingSGST,
                                         TotalAmount = appl.TotalAmount,
                                         PanNumber = appl.pan,
                                         RefundBankId = appl.RefundBankId,
                                         RefundBank = bank.bankName,
                                         BranchName = appl.BranchName,
                                         RefundInfaverof = appl.RefundInfaverof,
                                         RefundAccountNo = appl.RefundaccountNo,
                                         IFSCCode = appl.IFSCCode,
                                         AadharNumber = appl.AadharNumber,
                                         Sector = appl.Sector,
                                         PropertyNo = appl.PropertyNo,
                                         ApplicationFormType = appl.FormCategory.Trim(),
                                         ExpansionType = appl.FormSubCategory.Trim(),
                                         ExistingProperty = appl.ExistingPropertyNo,
                                         AllottmentDate = appl.AllotmentDate,
                                         LetterCode = appl.RentingLetterNo,
                                         LetterDate = appl.RentingDate,
                                         DispatchDate = appl.DispatchDate,
                                         AreaRangeId = appl.area,
                                         AreaRange = dbContext.FloorMsts.Where(f => f.floorId.ToString() == appl.area).Select(f => f.floorName).FirstOrDefault(),
                                         PropertyTypeId = appl.PropertyTypeID,
                                         PropertyType = dbContext.PropertyTypeMsts.FirstOrDefault(p => p.departmentId == appl.departmentId && p.propertyTypeId == appl.PropertyTypeID && p.IsActive == true).propertyTypeName,
                                         ChecklistHtml = appl.Documentfilename,
                                         FormType = appl.Online_offline == "Y" ? "Online" : "Offline",
                                         DirectorModel = new onlineDirectorViewModel
                                         {
                                             DirectorType = appl.gender == "Company" ? dbContext.Common_Config.Where(m => m.Id == appl.occupationId).FirstOrDefault().Name : string.Empty
                                         },
                                         ProposedModel = new ProposedCompanyViewModel
                                         {
                                             ProposedProject = appl.projectname,
                                             ImplementationTime = appl.projecttimeempl,
                                             TotalCost = appl.projectcost
                                         },
                                         //PreviousFormNo = appl.Comment != null ? appl.Comment : string.Empty,
                                         PreviousFormNo = appl.PreviousFormNo != null ? appl.PreviousFormNo : string.Empty,
                                         AppType = dbContext.NICsingalwindowSystems.Where(x => x.onlineapplicationId == id).FirstOrDefault() != null ? Constants.AppType : null
                                     }).FirstOrDefault();

                    }
                    else
                    {
                        applicant = (from appl in dbContext.OnlineApplicationDetails
                                     join dept in dbContext.DepartmentMsts on appl.departmentId equals dept.departmentId
                                     join scem in dbContext.SchemeMsts on appl.schemeId equals scem.schemeId
                                     //join bank in dbContext.BankMsts on appl.RefundBankId equals bank.bankId
                                     //join area in dbContext.FloorMsts on appl.area equals area.floorId.ToString()
                                     where appl.onlineapplicationId == id
                                     select new OnlineFormViewModel
                                     {
                                         Id = appl.onlineapplicationId,
                                         ApplicationFormId = appl.onlineapplicationId,
                                         SchemeId = scem.schemeId,
                                         SchemeName = scem.schemeName,
                                         SchemeType = scem.SchemeTypeMst.SchemeTypeDesc,
                                         DepartmentId = dept.departmentId,
                                         Department = dept.departmentName,
                                         Applicant = appl.gender == "Company" ? appl.CompanyName : appl.firstName + " " + appl.middleName + " " + appl.lastName,
                                         SigningAuthority = appl.signingAuthority,
                                         ApplicantType = appl.gender == "Company" ? "Company" : "Individual",
                                         CompanyTypeId = appl.CompanyType,
                                         CompanyType = dbContext.Common_Config.Where(c => c.Id == appl.CompanyType).Select(c => c.Name).FirstOrDefault(),
                                         FirstName = appl.firstName,
                                         MiddleName = appl.middleName,
                                         LastName = appl.lastName,
                                         Gender = appl.gender,
                                         MaritalStatus = appl.marritalStatus,
                                         DOB = appl.dateOfBirth,
                                         FatherName = (!string.IsNullOrEmpty(appl.fatherHusbandName)) ? appl.fatherHusbandName : "NA",
                                         ApplicantMaster = (!string.IsNullOrEmpty(appl.fatherHusbandName)) ? appl.fatherHusbandName : "NA",
                                         MotherName = appl.motherName,
                                         OccupationId = appl.occupationId,
                                         Occupation = dbContext.OccupationMsts.Where(c => c.occupationId == appl.occupationId).Select(s => s.occupation).FirstOrDefault(),
                                         ReligionId = appl.religionId,
                                         Religion = dbContext.ReligionMsts.Where(r => r.religionId == appl.religionId).Select(r => r.religion).FirstOrDefault(),
                                         CategoryId = appl.quotaId,
                                         CategoryName = dbContext.QuotaMsts.Where(c => c.quotaId == appl.quotaId).Select(q => q.quotaName).FirstOrDefault(),
                                         MobileNumber = appl.mobileNumberP2,
                                         PhoneNumber = appl.phoneNumberP2,
                                         FaxNumber = appl.faxNumberP2,
                                         //ApplicantGSTNumber = appl.faxNumberP2,
                                         ApplicantGSTNumber = appl.GSTNO,
                                         PermanentAddress = appl.permanentAdd,
                                         CorrespondingAddress = appl.correspondanceAdd,
                                         RegisteredOffice = appl.registeredOffice,
                                         Email = appl.email,
                                         AnnualIncome = appl.annualIncome,
                                         EarnestMoney = appl.EarnestMoney,
                                         ApplicationFee = appl.ApplicationFee,
                                         FormFeeSGST = appl.FormSGST,
                                         FormFeeCGST = appl.FormCGST,
                                         ProcessingCharge = appl.ProcessingCharge,
                                         ProcessingCGST = appl.ProcessingCGST,
                                         ProcessingSGST = appl.ProcessingSGST,
                                         TotalAmount = appl.TotalAmount,
                                         PanNumber = appl.pan,
                                         RefundBankId = appl.RefundBankId,
                                         RefundBank = "",
                                         BranchName = appl.BranchName,
                                         RefundInfaverof = appl.RefundInfaverof,
                                         RefundAccountNo = appl.RefundaccountNo,
                                         IFSCCode = appl.IFSCCode,
                                         AadharNumber = appl.AadharNumber,
                                         Sector = appl.Sector,
                                         PropertyNo = appl.PropertyNo,
                                         ApplicationFormType = appl.FormCategory.Trim(),
                                         ExpansionType = appl.FormSubCategory.Trim(),
                                         ExistingProperty = appl.ExistingPropertyNo,
                                         AllottmentDate = appl.AllotmentDate,
                                         LetterCode = appl.RentingLetterNo,
                                         LetterDate = appl.RentingDate,
                                         DispatchDate = appl.DispatchDate,
                                         AreaRangeId = appl.area,
                                         AreaRange = dbContext.FloorMsts.Where(f => f.floorId.ToString() == appl.area).Select(f => f.floorName).FirstOrDefault(),
                                         PropertyTypeId = appl.PropertyTypeID,
                                         PropertyType = dbContext.PropertyTypeMsts.FirstOrDefault(p => p.departmentId == appl.departmentId && p.propertyTypeId == appl.PropertyTypeID && p.IsActive == true).propertyTypeName,
                                         ChecklistHtml = appl.Documentfilename,
                                         FormType = appl.Online_offline == "Y" ? "Online" : "Offline",
                                         DirectorModel = new onlineDirectorViewModel
                                         {
                                             DirectorType = appl.gender == "Company" ? dbContext.Common_Config.Where(m => m.Id == appl.occupationId).FirstOrDefault().Name : string.Empty
                                         },
                                         ProposedModel = new ProposedCompanyViewModel
                                         {
                                             ProposedProject = appl.projectname,
                                             ImplementationTime = appl.projecttimeempl,
                                             TotalCost = appl.projectcost
                                         },
                                         //PreviousFormNo = appl.Comment != null ? appl.Comment : string.Empty,
                                         PreviousFormNo = appl.PreviousFormNo != null ? appl.PreviousFormNo : string.Empty,
                                         AppType = dbContext.NICsingalwindowSystems.Where(x => x.onlineapplicationId == id).FirstOrDefault() != null ? Constants.AppType : null
                                     }).FirstOrDefault();
                }

                    if (applicant.Gender.ToLower() == Constants.Company.ToLower())
                    {
                        applicant.SigningAuthorityId = applicant.CategoryId;
                        if (applicant.CategoryId != null && applicant.CategoryId > 0)
                        {
                            applicant.SignatoryStatus = dbContext.Common_Config.FirstOrDefault(x => x.Id == applicant.CategoryId).Name;
                        }
                    }

                    applicant.FormFeeGST = applicant.FormFeeCGST + applicant.FormFeeSGST;
                    applicant.FormFeeWithGST = applicant.ApplicationFee + applicant.FormFeeGST;
                    applicant.ProcessingChargeGST = applicant.ProcessingSGST + applicant.ProcessingCGST;
                    applicant.ProcessingChargeWithGST = applicant.ProcessingCharge + applicant.ProcessingChargeGST;

                    //var doctype = applicant.ChecklistHtml.Split(',');
                    if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + id + "/Pictures/")))
                    {
                        var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + id + "/Pictures/");
                        string[] files = Directory.GetFiles(filepath);
                        if (files != null)
                        {
                            if (files.Count() > 0)
                            {
                                if (files.Count() == 2)
                                {
                                    if (files[0] != null)
                                    {
                                        var uplod = files[0].Split('\\');
                                        applicant.SignImage = "/UploadFiles/" + id + "/Pictures/" + uplod[uplod.Length - 1];
                                    }
                                    if (files[1] != null)
                                    {
                                        var uplod = files[1].Split('\\');
                                        applicant.UserImage = "/UploadFiles/" + id + "/Pictures/" + uplod[uplod.Length - 1];
                                    }
                                }
                            }
                        }
                    }
                    applicant.IsApplicationFeePaid = false;
                    var payment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(p => p.ServiceRefId == detail.onlineapplicationId && p.status == 1 && p.ServiceType == 1);
                    if (payment != null)
                    {
                        applicant.IsApplicationFeePaid = true;
                        applicant.PaymentMode = "Online";
                        applicant.PaymentModel = GetOnlinePaymentDetailById(id, payment.AutoID);
                    }
                    else
                    {
                        var Offpayment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(p => p.ServiceRefId == detail.onlineapplicationId && p.ServiceType == 3);
                        if (Offpayment != null)
                        {
                            applicant.PaymentMode = "Offline";
                            applicant.IsChallanGenerated = true;
                            if (!string.IsNullOrEmpty(Offpayment.TrKey))
                            {
                                applicant.PaymentMode = "Offline_Updated";
                                if (Offpayment.status == 1 || Offpayment.TranStatus == 1)
                                {
                                    applicant.IsApplicationFeePaid = true;
                                    applicant.IsChallanGenerated = false;
                                    applicant.PaymentMode = "Offline_Validated";
                                }
                            }
                            applicant.PaymentModel = GetOnlinePaymentDetailById(id, Offpayment.AutoID);
                        }
                    }

                    applicant.PaidThroughSWP = false;
                    if (applicant.AppType == Constants.AppType)
                    {
                        //check single window portal
                        var singleWindowPortal = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(p => p.ServiceRefId == detail.onlineapplicationId && p.ServiceType == Constants.singleWindowPortalApplicationPayment);
                        if (singleWindowPortal != null)
                        {
                            applicant.PaidThroughSWP = true;
                            if (singleWindowPortal.TranStatus == 1 && singleWindowPortal.status == 1)
                            {
                                applicant.IsApplicationFeePaid = true;
                                applicant.ServiceAppPayStatus = "Paid";
                            }
                            applicant.PaymentModel = GetOnlinePaymentDetailById(id, singleWindowPortal.AutoID);
                        }
                    }

                    var flag = IsDocumentUploaded(id);
                    if (flag == true)
                    {
                        applicant.IsDocumentUploaded = true;
                        applicant.DocumentsTable = dbContext.Sp_NewSchemereturn(applicant.ApplicationFormId, applicant.SchemeId).FirstOrDefault();
                    }
                    //else applicant.IsDocumentUploaded = false;

                    if (applicant.SchemeType == OnlineSchemeType.Transport || applicant.SchemeType == OnlineSchemeType.OpenEnded)
                    {
                        var PreChallan = IsPreviousChallanUploaded(id);
                        if (PreChallan == true)
                        {
                            applicant.IsPreviousChallanUploaded = true;
                        }
                    }
                }
                return applicant;
            }
        }

        private OnlinePaymentViewModel GetOnlinePaymentDetailById(int? id, int? AutoId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from transaction in dbContext.OnlineApplicationDetails_trans
                              where transaction.ServiceRefId == id && transaction.AutoID == AutoId
                              select new OnlinePaymentViewModel
                              {
                                  Id = id,
                                  ApplicationFormId = transaction.ServiceRefId,
                                  TransactionKey = transaction.TrKey,
                                  TransactionId = transaction.txnid,
                                  Amount = transaction.Amount,
                                  ProductInfo = transaction.productinfo,
                                  Mode = transaction.mode,
                                  TransactionStatusId = transaction.TranStatus,
                                  Discount = transaction.discount,
                                  StatusId = transaction.status,
                                  EntryDate = transaction.EntryDate,
                                  //Applicant = details.gender == Constants.genderCompany ? details.CompanyName : details.firstName + " " + details.middleName + "" + details.lastName,
                                  //FirstName = details.gender == Constants.genderCompany ? details.CompanyName : details.firstName + " " + details.middleName + "" + details.lastName,
                                  //Email = details.email,
                                  //PhoneNumber = details.mobileNumberP1,
                                  Status = transaction.TranStatus == Constants.Success ? "Success" : "Failure",
                                  PaymentSource = transaction.payment_source,
                                  BankReferenceNo = transaction.bank_ref_num,
                                  BankCode = transaction.bankcode,
                                  Error = transaction.error,
                                  ErrorMessage = transaction.error_Message,
                                  NameOnCard = transaction.name_on_card,
                                  CardNumber = transaction.cardnum,
                                  CardHash = transaction.cardhash,
                                  IssuingBank = transaction.issuing_bank,
                                  CardType = transaction.card_type,
                                  Mihpayid = transaction.mihpayid,
                                  modifiydate = transaction.modifiydate,
                                  BankName = transaction.ServiceType == 3 ? dbContext.BankMsts.Where(m => m.bankId.ToString() == transaction.GetwayName && m.IsActive == true).FirstOrDefault().bankName : (transaction.ServiceType == 5 ? "Single Window Portal" : string.Empty)
                              }).FirstOrDefault();
                return detail;
            }
        }

        public string GetListOfChecklistDocuments(int? schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {

                var checklist = (from doc in dbContext.OnlineCheckListMasters
                                 where doc.IsActive == true //&& doc.schemeId == schemeId
                                 select new
                                 {
                                     Id = doc.CheckListId,
                                     DocumentName = doc.CheckListName
                                 }).ToList();
                string divMain = "";
                int count = 0;
                foreach (var item in checklist)
                {
                    divMain = divMain + "<div class='row  border-bottom'> "
                                           + "<div class='col-md-2 col-lbl'><span>" + count + "</span></div>"
                                           + "<div class='col-md-6 col-lbl'><span>" + item.DocumentName + "</span></div>"
                                           + "<div class='col-md-4 col-lbl-vl'><input type='file' class='single' name='files' /></div>"
                                      + "</div>";
                    count++;
                }
                return divMain;
            }
        }

        int configSchemeId = !(string.IsNullOrEmpty(ConfigurationManager.AppSettings["SchemeId"])) ? Convert.ToInt32(ConfigurationManager.AppSettings["SchemeId"]) : 0;
        public DataSourceResult GetUploadedDocumentsForOnlineForm(DataSourceRequest request, int? formId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<OnlineDocumentViewModel> docs = new List<OnlineDocumentViewModel>();
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(f => f.onlineapplicationId == formId);
                var documents = dbContext.OnlineCheckListMasters.ToList();
                if (form != null)
                {
                    docs = (from doc in dbContext.OnlineCheckListMasters
                            where doc.IsActive == true && doc.SchemeId == form.schemeId //configSchemeId
                            select new OnlineDocumentViewModel
                            {
                                Id = doc.CheckListId,
                                DocumentId = doc.CheckListId,
                                DocumentName = doc.CheckListName,
                                ParentDocumentType = doc.MainList
                            }).ToList();
                    if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formId + "/Documents/")))
                    {
                        var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.onlineapplicationId + "/Documents/");
                        string[] files = Directory.GetFiles(filepath);
                        if (files != null)
                        {
                            for (int x = 0; x < files.Length; x++)
                            {
                                var ufile = files[x].Split('\\');
                                var filename = ufile[ufile.Length - 1];
                                int start = (filename.LastIndexOf('-') + 1);
                                int end = filename.LastIndexOf('.');
                                int dif = end - start;
                                int nod = Convert.ToInt32(filename.Substring(start, dif));
                                docs.Where(m => m.Id == nod).FirstOrDefault().PathName = "/UploadFiles/" + formId + "/Documents/" + filename;
                                docs.Where(m => m.Id == nod).FirstOrDefault().UploadedDocument = filename;
                                //docs.ElementAt(nod - 1).PathName = "/UploadFiles/" + formId + "/Documents/" + filename;
                                // docs.ElementAt(nod - 1).UploadedDocument = filename;
                            }

                        }
                    }
                    int counter = 1;
                    docs.ForEach(x => x.Sno = counter++);
                }
                return docs.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetUploadedDocumentsAfterScrutiny(DataSourceRequest request, int? formId, int? checklistIdstart, int? checklistIdend)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<OnlineDocumentViewModel> docs = new List<OnlineDocumentViewModel>();
                if (formId == null)
                {
                    if (HttpContext.Current.Session["SchemeUserLoginDetails"] != null)
                    {
                        CurrentSchemeUserDetail loginuser = (CurrentSchemeUserDetail)HttpContext.Current.Session["SchemeUserLoginDetails"];
                        formId = loginuser.ApplicationFormId;
                    }
                    else return null;
                }
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(f => f.onlineapplicationId == formId);
                //var documents = dbContext.OnlineCheckListMasters.ToList();
                if (form != null)
                {
                    docs = (from doc in dbContext.OnlineCheckListMasters
                            where doc.IsActive == true //&& doc.SchemeId == form.schemeId // configSchemeId 
                                //&& doc.CheckListId >= checklistIdstart && doc.CheckListId <= checklistIdend
                            && doc.CheckListType == Constants.SCRUTINY
                            select new OnlineDocumentViewModel
                            {
                                Id = doc.CheckListId,
                                DocumentId = doc.CheckListId,
                                DocumentName = doc.CheckListName,
                                ParentDocumentType = doc.MainList
                            }).ToList();
                    if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formId + "/Documents/")))
                    {
                        var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.onlineapplicationId + "/Documents/");
                        string[] files = Directory.GetFiles(filepath);
                        if (files != null)
                        {
                            for (int x = 0; x < files.Length; x++)
                            {
                                var ufile = files[x].Split('\\');
                                var filename = ufile[ufile.Length - 1];
                                int start = (filename.LastIndexOf('-') + 1);
                                int end = filename.LastIndexOf('.');
                                int dif = end - start;
                                int nod = Convert.ToInt32(filename.Substring(start, dif));

                                foreach (var dc in docs)
                                {
                                    if (dc.Id == nod)
                                    {
                                        dc.PathName = "/UploadFiles/" + formId + "/Documents/" + filename ;
                                        dc.UploadedDocument = filename;
                                    }
                                    //dc.PathName = dc.Id == nod ? (dc.PathName == null ? "/UploadFiles/" + formId + "/Documents/" + filename : dc.PathName) : null;
                                    //dc.UploadedDocument = dc.Id == nod ? (dc.UploadedDocument == null ? filename : dc.UploadedDocument) : null;
                                }
                            }
                        }
                    }
                    int counter = 1;
                    docs.ForEach(x => x.Sno = counter++);
                }
                return docs.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetUploadedDocumentsForReturnForm(DataSourceRequest request, int? formId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<OnlineDocumentViewModel> docs = new List<OnlineDocumentViewModel>();
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(f => f.onlineapplicationId == formId);
                var documents = dbContext.OnlineCheckListMasters.ToList();
                if (form != null)
                {
                    docs = (from doc in dbContext.OnlineCheckListMasters
                            where doc.IsActive == true && doc.SchemeId == form.schemeId //&& doc.SchemeId == form.schemeId // configSchemeId
                            select new OnlineDocumentViewModel
                            {
                                Id = doc.CheckListId,
                                DocumentId = doc.CheckListId,
                                DocumentName = doc.CheckListName,
                                ParentDocumentType = doc.MainList
                            }).ToList();
                    if (form.ScrutinyStatusId == 2)
                    {
                        var scrutinydocs = (from doc in dbContext.OnlineCheckListMasters
                                            where doc.IsActive == true && doc.CheckListType == Constants.SCRUTINY //&& doc.SchemeId == form.schemeId // configSchemeId
                                            select new OnlineDocumentViewModel
                                            {
                                                Id = doc.CheckListId,
                                                DocumentId = doc.CheckListId,
                                                DocumentName = doc.CheckListName,
                                                ParentDocumentType = doc.MainList
                                            }).ToList();
                        docs.AddRange(scrutinydocs);
                    }
                    if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formId + "/Documents/")))
                    {
                        var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.onlineapplicationId + "/Documents/");
                        string[] files = Directory.GetFiles(filepath);
                        if (files != null)
                        {

                            for (int x = 0; x < files.Length; x++)
                            {
                                var ufile = files[x].Split('\\');
                                var filename = ufile[ufile.Length - 1];
                                int start = (filename.LastIndexOf('-') + 1);
                                int end = filename.LastIndexOf('.');
                                int dif = end - start;
                                int nod = Convert.ToInt32(filename.Substring(start, dif));
                                docs.Where(m => m.Id == nod).FirstOrDefault().PathName = "/UploadFiles/" + formId + "/Documents/" + filename;
                                docs.Where(m => m.Id == nod).FirstOrDefault().UploadedDocument = filename;
                                //docs.ElementAt(nod - 1).PathName = "/UploadFiles/" + formId + "/Documents/" + filename;
                                //docs.ElementAt(nod - 1).UploadedDocument = filename;
                            }
                        }
                    }
                    docs = docs.Where(m => !string.IsNullOrEmpty(m.PathName)).ToList();
                    int counter = 1;
                    docs.ForEach(x => x.Sno = counter++);
                    return docs.ToDataSourceResult(request);
                }
                return null;
            }
        }

        public int ValidatePANnumber(string pan, int? areaId, int? schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(o => o.isActive == true && o.pan.ToUpper() == pan.ToUpper() && o.area == areaId.ToString() && o.schemeId == schemeId);
                if (form == null) return ReturnType.NotExist;
                else return ReturnType.Exist;
            }
        }

        public int UpdateOnlineApplicationForm(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            int flag = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == model.ApplicationFormId);
                if (detail != null)
                {
                    if (model.ApplicantType == "Individual")
                    {
                        detail.firstName = model.FirstName;
                        detail.middleName = model.MiddleName;
                        detail.lastName = model.LastName;
                        detail.fatherHusbandName = model.FatherName;
                        detail.motherName = model.MotherName;
                        detail.gender = model.Gender;
                        detail.CompanyType = model.CompanyTypeId;
                        detail.religionId = model.ReligionId;
                        detail.occupationId = model.OccupationId;
                        //application.quotaId = model.CategoryId;
                        detail.quotaId = Constants.General;
                        detail.marritalStatus = model.MaritalStatus;
                        detail.dateOfBirth = model.DOB;
                    }
                    else
                    {
                        detail.CompanyName = model.Applicant;
                        detail.signingAuthority = model.SigningAuthority;
                        detail.registeredOffice = model.PermanentAddress;
                        detail.CompanyType = model.CompanyTypeId;
                        detail.gender = "Company";
                        detail.fatherHusbandName = model.ApplicantMaster;
                        detail.quotaId = model.SigningAuthorityId;
                    }
                    detail.schemeId = model.SchemeId;
                    detail.departmentId = model.DepartmentId;
                    detail.permanentAdd = model.PermanentAddress;
                    detail.correspondanceAdd = model.CorrespondingAddress;
                    detail.area = model.AreaRangeId;
                    detail.mobileNumberP2 = model.MobileNumber;
                    detail.phoneNumberP2 = model.PhoneNumber;
                    //detail.faxNumberP2 = model.ApplicantGSTNumber;
                    detail.GSTNO = model.ApplicantGSTNumber;
                    detail.email = model.Email;
                    detail.annualIncome = model.AnnualIncome;
                    detail.ApplicationFee = model.ApplicationFee;
                    detail.ProcessingCharge = model.ProcessingCharge;
                    detail.EarnestMoney = model.EarnestMoney;
                    detail.TotalAmount = model.TotalAmount;
                    detail.pan = model.PanNumber;
                    //detail.ApplicationDate = DateTime.Now;

                    detail.RefundBankId = model.RefundBankId;
                    detail.RefundInfaverof = model.RefundInfaverof;
                    detail.RefundaccountNo = model.RefundAccountNo;

                    detail.PropertyTypeID = model.PropertyTypeId;
                    detail.Sector = model.Sector;
                    detail.BranchName = model.BranchName;
                    detail.IFSCCode = model.IFSCCode;

                    //added on 19 aug 2017
                    detail.FormCGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                    detail.FormSGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                    detail.ProcessingCGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                    detail.ProcessingSGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                    detail.AadharNumber = model.AadharNumber;

                    detail.FormCategory = model.ApplicationFormType;
                    if (model.ApplicationFormType == "Expansion")
                    {
                        detail.FormSubCategory = model.ExpansionType;
                        detail.RentingDate = model.LetterDate;
                        detail.RentingLetterNo = model.LetterCode;
                        detail.RentingPropertyNo = model.PropertyNo;
                    }

                    if (model.SchemeType == OnlineSchemeType.Transport)
                    {
                        if (!string.IsNullOrEmpty(model.PreviousFormNo))
                        {
                            //detail.Comment = model.PreviousFormNo;
                            detail.PreviousFormNo = model.PreviousFormNo;
                        }
                    }

                    detail.PropertyNo = model.PropertyNo;
                    detail.ExistingPropertyNo = model.ExistingProperty;
                    detail.AllotmentDate = model.AllottmentDate;
                    detail.DispatchDate = model.DispatchDate;

                    detail.modifiedBy = "Online";
                    detail.modifiedDate = DateTime.Now;
                    detail.isActive = true;

                    dbContext.SaveChanges();

                    SaveDocumentsForApplicationForm(model, files, userImage, signatureImage);

                    //save director details
                    if (OnlineSchemeType.Transport == model.SchemeType)
                    {
                        UpdateDirectorDetailsForOpenSchemeForm(model.ApplicationFormId);
                    }

                    //var body = "Dear User, Your Application Form  has been updated successfully. We assure you of our best services always.Thanks and Regards. Noida Authority";
                    string message = string.Format(NAMessages.OnlineApplicationFormUpdate, detail.onlineapplicationId);
                    ApplicationHelper.SendEmail(model.Email, "Registration Form", message);

                    ApplicationHelper.SendSMS(model.MobileNumber, message);

                    flag = ReturnType.Success;
                }
            }
            return model.ApplicationFormId.Value;
        }

        public int ValidateFormNumber(string formNo)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int formId = Convert.ToInt32(formNo);
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(o => o.onlineapplicationId == formId && o.isActive == true);
                if (form == null) return ReturnType.NotExist;
                else return form.onlineapplicationId;
            }
        }

        public int ValidateOnlineFormPayment(string formNo)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = 0;
                int formId = Convert.ToInt32(formNo);
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(f => f.onlineapplicationId == formId);
                var payment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(o => o.ServiceRefId == formId && o.status == 1 && o.ServiceType == 1);
                var payment_off = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(o => o.ServiceRefId == formId && o.ServiceType == 3);

                if (form != null)
                {
                    if (payment == null && payment_off == null)
                    {
                        //int otpm = ApplicationHelper.GenerateOTP();
                        //int otpe = ApplicationHelper.GenerateOTP();
                        //HttpContext.Current.Session["OTPmobile"] = otpm;
                        //HttpContext.Current.Session["OTPemail"] = otpe;
                        ////string emailMessage = "Dear User,</br>This is OTP"+ otpe +" to proceed for payment of  Form No:" + form.onlineapplicationId + ".</br>Thanks and Regards,<br>Noida Authority";
                        ////string mobileMessage = "Dear User,This is OTP" + otpm + " to proceed for payment of  Form No:" + form.onlineapplicationId + ".Thanks and Regards,<br>Noida Authority";
                        //string emailMessage = string.Format(NAMessages.OnlinePaymentOTP, form.onlineapplicationId, otpm);
                        //string mobileMessage = string.Format(NAMessages.OnlinePaymentOTP, form.onlineapplicationId, otpm);
                        //ApplicationHelper.SendSMS(form.mobileNumberP2, mobileMessage);
                        //ApplicationHelper.SendEmail(form.email, "OnlineForm", emailMessage);
                        flag = ReturnType.None;
                    }
                    else flag = ReturnType.Exist;
                }
                else flag = ReturnType.NotExist;

                return flag;
            }
        }

        public OnlinePaymentViewModel SaveOnlinePaymentTransaction(OnlineFormViewModel form)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                string transid = string.Empty;
                string trxkey = ApplicationHelper.GenerateTransactionId();
                var transactionId = dbContext.OnlineApplicationDetails_trans.Where(f => f.ServiceRefId == form.ApplicationFormId).OrderByDescending(x => x.AutoID).Select(x => x.txnid).FirstOrDefault();
                if (transactionId == null)
                {
                    int brochureDepartmentId = Convert.ToInt32(ConfigurationManager.AppSettings["InstitutionalDepartmentId"]);
                    OnlineApplicationDetails_trans trans = new OnlineApplicationDetails_trans
                    {
                        ServiceRefId = form.ApplicationFormId,
                        TrKey = trxkey,
                        txnid = form.ApplicationFormId.ToString() + "-1",
                        Amount = form.TotalAmount,
                        productinfo = "Online",
                        mode = "Online",
                       // ServiceType = Constants.OnlineApplicationPayment,
                        ServiceType = form.DepartmentId != brochureDepartmentId ? Constants.OnlineApplicationPayment : Constants.BrochureApplicationPayment,
                        GetwayName = "IND",
                        status = 0,
                        EntryDate = DateTime.Now
                    };
                    dbContext.OnlineApplicationDetails_trans.Add(trans);
                    dbContext.SaveChanges();
                    transid = trans.txnid;
                }
                else
                {
                    var temp = transactionId.Split('-').Last();
                    int newTxId = Convert.ToInt32(temp) + 1;
                    OnlineApplicationDetails_trans trans = new OnlineApplicationDetails_trans
                    {
                        ServiceRefId = form.ApplicationFormId,
                        TrKey = trxkey,
                        txnid = form.ApplicationFormId.ToString() + "-" + newTxId,
                        Amount = form.TotalAmount,
                        productinfo = "Online",
                        mode = "Online",
                        ServiceType = Constants.OnlineApplicationPayment,
                        GetwayName = "IND",
                        status = 0,
                        EntryDate = DateTime.Now
                    };

                    dbContext.OnlineApplicationDetails_trans.Add(trans);
                    dbContext.SaveChanges();
                    transid = trans.txnid;
                }

                var model = (from trans in dbContext.OnlineApplicationDetails_trans
                             join appl in dbContext.OnlineApplicationDetails on trans.ServiceRefId equals appl.onlineapplicationId
                             where trans.txnid == transid
                             select new OnlinePaymentViewModel
                             {
                                 ApplicationFormId = trans.ServiceRefId,
                                 TransactionKey = trans.TrKey,
                                 TransactionId = trans.txnid,
                                 Amount = trans.Amount,
                                 ProductInfo = trans.productinfo,
                                 Mode = trans.mode,
                                 TransactionStatusId = trans.TranStatus,
                                 Discount = trans.discount,
                                 StatusId = trans.status,
                                 EntryDate = trans.EntryDate,
                                 FirstName = form.FirstName,
                                 Email = form.Email,
                                 PhoneNumber = form.MobileNumber
                             }).FirstOrDefault();
                return model;

                //return model;
            }
        }

        public OnlinePaymentViewModel GetOnlinePaymentTransactionDetailById(string transactionId, string paymentType)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var PaymentModel = new OnlinePaymentViewModel();
                if (paymentType == Constants.strOther)
                {
                    var rid = Convert.ToInt32(transactionId.Split('-')[1]);

                    var form = dbContext.ApplicationDetails.Where(model => model.registrationId == rid).FirstOrDefault();
                    var customerName = form.tFirstName + " " + form.tMiddleName + "" + form.tLastName;
                    var cutomerEmail = form.tEmail;
                    var customerPhone = form.mobileNumberP2;

                    PaymentModel = (from transaction in dbContext.OnlineApplicationDetails_trans
                                    where transaction.txnid == transactionId
                                    select new OnlinePaymentViewModel
                                    {
                                        ApplicationFormId = transaction.ServiceRefId,
                                        TransactionKey = transaction.TrKey,
                                        TransactionId = transaction.txnid,
                                        Amount = transaction.Amount,
                                        ProductInfo = transaction.productinfo,
                                        Mode = transaction.mode,
                                        TransactionStatusId = transaction.TranStatus,
                                        Discount = transaction.discount,
                                        StatusId = transaction.status,
                                        EntryDate = transaction.EntryDate,
                                        Applicant = customerName,
                                        FirstName = customerName,
                                        Email = cutomerEmail,
                                        PhoneNumber = customerPhone,
                                        Status = transaction.TranStatus == Constants.Success ? "Success" : "Failure",
                                        PaymentSource = transaction.payment_source,
                                        BankReferenceNo = transaction.bank_ref_num,
                                        BankCode = transaction.bankcode,
                                        Error = transaction.error,
                                        ErrorMessage = transaction.error_Message,
                                        NameOnCard = transaction.name_on_card,
                                        CardNumber = transaction.cardnum,
                                        CardHash = transaction.cardhash,
                                        IssuingBank = transaction.issuing_bank,
                                        CardType = transaction.card_type,
                                        Mihpayid = transaction.mihpayid
                                    }).FirstOrDefault();
                }
                else
                {
                    PaymentModel = (from transaction in dbContext.OnlineApplicationDetails_trans
                                    join details in dbContext.OnlineApplicationDetails on transaction.ServiceRefId equals details.onlineapplicationId
                                    where transaction.txnid == transactionId
                                    select new OnlinePaymentViewModel
                                    {
                                        ApplicationFormId = transaction.ServiceRefId,
                                        TransactionKey = transaction.TrKey,
                                        TransactionId = transaction.txnid,
                                        Amount = transaction.Amount,
                                        ProductInfo = transaction.productinfo,
                                        Mode = transaction.mode,
                                        TransactionStatusId = transaction.TranStatus,
                                        Discount = transaction.discount,
                                        StatusId = transaction.status,
                                        EntryDate = transaction.EntryDate,
                                        Applicant = details.gender == Constants.genderCompany ? details.CompanyName : details.firstName + " " + details.middleName + "" + details.lastName,
                                        FirstName = details.gender == Constants.genderCompany ? details.CompanyName : details.firstName + " " + details.middleName + "" + details.lastName,
                                        Email = details.email,
                                        Mobile = details.mobileNumberP2,
                                        PhoneNumber = details.phoneNumberP2,
                                        Status = transaction.TranStatus == Constants.Success ? "Success" : "Failure",
                                        PaymentSource = transaction.payment_source,
                                        BankReferenceNo = transaction.bank_ref_num,
                                        BankCode = transaction.bankcode,
                                        Error = transaction.error,
                                        ErrorMessage = transaction.error_Message,
                                        NameOnCard = transaction.name_on_card,
                                        CardNumber = transaction.cardnum,
                                        CardHash = transaction.cardhash,
                                        IssuingBank = transaction.issuing_bank,
                                        CardType = transaction.card_type,
                                        Mihpayid = transaction.mihpayid
                                    }).FirstOrDefault();
                }
                return PaymentModel;
            }
        }

        public OnlinePaymentViewModel UpdateOnlinePaymentTransaction(System.Web.Mvc.FormCollection form)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var id = form["udf1"];
                var paymenttype = form["udf2"];

                var TransactionModel = new OnlinePaymentViewModel();
                var transaction = dbContext.OnlineApplicationDetails_trans.Where(m => m.txnid == id).FirstOrDefault();
                var flag = false;
                if (paymenttype == Constants.strOther)
                {
                    if (transaction != null)
                    {
                        if (form["status"].ToString() == "success")
                        {
                            transaction.TranStatus = Constants.Success;
                            transaction.status = 1;
                        }
                        else
                        {
                            transaction.TranStatus = Constants.Faliure;
                            transaction.error = form["error"];
                            transaction.error_Message = form["error_Message"];
                            transaction.status = 0;
                        }
                        transaction.TrKey = form["txnid"].ToString();
                        transaction.mode = form["mode"].ToString();
                        transaction.mihpayid = form["mihpayid"];
                        transaction.productinfo = form["productinfo"];
                        transaction.EntryDate = DateTime.Now;
                        transaction.payment_source = form["payment_source"];
                        transaction.PG_Type = form["PG_Type"];
                        transaction.bank_ref_num = form["bank_ref_num"];
                        transaction.bankcode = form["bankcode"];
                        transaction.name_on_card = form["name_on_card"];
                        transaction.cardnum = form["cardnum"];
                        //transaction.cardhash = form["cardhash"];
                        transaction.issuing_bank = form["issuing_bank"];
                        transaction.card_type = form["card_type"];
                        dbContext.SaveChanges();

                        TransactionModel = GetOnlinePaymentTransactionDetailById(id, paymenttype);
                        if (form["status"].ToString() == "success")
                        {
                            string emailMessage = "Hi,</br></br>Application form fee has been paid against Form No:" + TransactionModel.ApplicationFormId + ".</br></br>We value your relationship with us and assure you of our best services always.</br></br>Thanks and Regards,<br>Noida Authority";
                            string mobileMessage = string.Format(NAMessages.OnlineApplicationFormFee, TransactionModel.ApplicationFormId);
                            ApplicationHelper.SendSMS(TransactionModel.Mobile, mobileMessage);
                            ApplicationHelper.SendEmail(TransactionModel.Email, "OnlineForm", emailMessage);
                        }

                        if (TransactionModel != null) TransactionModel.FormModel = GetOnlineApplicationFormById(TransactionModel.ApplicationFormId);
                    }
                }
                else
                {
                    if (transaction != null)
                    {
                        if (form["status"].ToString() == "success")
                        {
                            transaction.TranStatus = Constants.Success;
                            transaction.status = 1;
                        }
                        else
                        {
                            transaction.TranStatus = Constants.Faliure;
                            transaction.error = form["error"];
                            transaction.error_Message = form["error_Message"];
                            transaction.status = 0;
                        }
                        transaction.TrKey = form["txnid"].ToString();
                        transaction.mode = form["mode"].ToString();
                        transaction.mihpayid = form["mihpayid"];
                        transaction.productinfo = form["productinfo"];
                        transaction.EntryDate = DateTime.Now;
                        transaction.payment_source = form["payment_source"];
                        transaction.PG_Type = form["PG_Type"];
                        transaction.bank_ref_num = form["bank_ref_num"];
                        transaction.bankcode = form["bankcode"];
                        transaction.name_on_card = form["name_on_card"];
                        transaction.cardnum = form["cardnum"];
                        //transaction.cardhash = form["cardhash"];
                        transaction.issuing_bank = form["issuing_bank"];
                        transaction.card_type = form["card_type"];
                        dbContext.SaveChanges();

                        TransactionModel = GetOnlinePaymentTransactionDetailById(id, paymenttype);
                        if (form["status"].ToString() == "success")
                        {
                            string emailMessage = "Hi,</br></br>Application form fee has been paid against Form No:" + TransactionModel.ApplicationFormId + ".</br></br>We value your relationship with us and assure you of our best services always.</br></br>Thanks and Regards,<br>Noida Authority";
                            string mobileMessage = string.Format(NAMessages.OnlineApplicationFormFee, TransactionModel.ApplicationFormId);
                            ApplicationHelper.SendSMS(TransactionModel.Mobile, mobileMessage);
                            ApplicationHelper.SendEmail(TransactionModel.Email, "OnlineForm", emailMessage);
                        }
                        if (TransactionModel != null) TransactionModel.FormModel = GetOnlineApplicationFormById(TransactionModel.ApplicationFormId);
                    }
                }
                return TransactionModel;
            }
        }

        public int RemoveDocumentFromApplicationForm(string formNo, string filename)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = 0;
                if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formNo + "/Documents/")))
                {
                    var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formNo + "/Documents/");
                    string[] files = Directory.GetFiles(filepath);
                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            var flst = file.Split('\\');
                            var nfileName = flst[flst.Length - 1];
                            var wFileName = nfileName.Split('.');
                            int CheckListId = Convert.ToInt32((wFileName[0].Split('-'))[1]);
                            if (flst[flst.Length - 1] == filename)
                            {
                                OnlineCheckLisTran objOnlineCheckLisTran = new OnlineCheckLisTran();
                                objOnlineCheckLisTran = dbContext.OnlineCheckLisTrans.Where(m => m.onlineapplicationId.ToString() == formNo && m.CheckListId == CheckListId).FirstOrDefault();
                                if (objOnlineCheckLisTran != null)
                                {
                                    dbContext.OnlineCheckLisTrans.Remove(objOnlineCheckLisTran);
                                    dbContext.SaveChanges();
                                }
                                File.Delete(file);
                                flag = ReturnType.Success;
                            }
                        }
                    }
                }
                return flag;
            }
        }

        public List<int> SendOTPforPayment(string formId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<int> otplist = new List<int>();
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(o => o.onlineapplicationId.ToString() == formId);
                if (form != null)
                {
                    int otpm = ApplicationHelper.GenerateOTP();
                    int otpe = ApplicationHelper.GenerateOTP();
                    otplist.Add(otpm);
                    otplist.Add(otpe);
                    HttpContext.Current.Session["OTPmobile"] = otpm;
                    HttpContext.Current.Session["OTPemail"] = otpe;
                    string emailMessage = string.Format(NAMessages.OnlinePaymentOTP, form.onlineapplicationId, otpm);
                    string mobileMessage = string.Format(NAMessages.OnlinePaymentOTP, form.onlineapplicationId, otpm);
                    ApplicationHelper.SendSMS(form.mobileNumberP2, mobileMessage);
                    ApplicationHelper.SendEmail(form.email, "OnlineForm", emailMessage);
                }
                return otplist;
            }
        }

        public int UploadDocumentByFormId(OnlineFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            if (model.ApplicationFormId != null && model.ApplicationFormId > 0)
            {
                model.Id = model.ApplicationFormId;
                if (model.Id != null && model.Id > 0)
                {
                    int formId = model.Id.Value;
                    SaveDocumentsForApplicationForm(model, files, userImage, signatureImage);
                    return formId;
                }
            }
            return ReturnType.Failure;
        }

        #region Generate Challan For Scheme
        public OnlineChallanViewModel GenerateSchemeChallan(OnlineFormViewModel objOnlineFormView)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                OnlineChallanViewModel objSchemeChallanModel = new OnlineChallanViewModel();
                objSchemeChallanModel = SaveoffinePaymentTransaction(objOnlineFormView);
                return objSchemeChallanModel;
            }
        }

        public OnlineChallanViewModel SaveoffinePaymentTransaction(OnlineFormViewModel Objmodel)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var onlineApplication = dbContext.OnlineApplicationDetails.Where(m => m.onlineapplicationId == Objmodel.ApplicationFormId && m.isActive == true).FirstOrDefault();
                if (onlineApplication != null)
                {
                    var transaction = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(f => f.ServiceRefId == Objmodel.ApplicationFormId && f.ServiceType != 1 );
                    if (transaction == null)
                    {
                        OnlineApplicationDetails_trans trans = new OnlineApplicationDetails_trans
                        {
                            ServiceRefId = Objmodel.ApplicationFormId,
                            txnid = Objmodel.ApplicationFormId.ToString() + "T01",
                            Amount = onlineApplication.TotalAmount,
                            productinfo = "RTGS/NEFT",
                            mode = "RTGS/NEFT",
                            ServiceType = Constants.offlineApplicationPayment,
                            GetwayName = Objmodel.ChallanBankId.ToString(),
                            udf5 = Objmodel.BankIFSCCode,
                            virtualaccountno = Objmodel.VirtualAccPrefix + Objmodel.ApplicationFormId.ToString() + "T01",
                            status = 0,
                            EntryDate = DateTime.Now
                        };
                        dbContext.OnlineApplicationDetails_trans.Add(trans);
                        dbContext.SaveChanges();
                    }
                }

                var model = (from appl in dbContext.OnlineApplicationDetails
                             join trans in dbContext.OnlineApplicationDetails_trans on appl.onlineapplicationId equals trans.ServiceRefId
                             where appl.onlineapplicationId == Objmodel.ApplicationFormId && trans.ServiceType == 3
                             select new OnlineChallanViewModel
                             {
                                 FormModel = new OnlineFormViewModel
                                 {
                                     ApplicationFormId = appl.onlineapplicationId,
                                     Applicant = appl.gender != Constants.genderCompany ? appl.firstName + " " + (!string.IsNullOrEmpty(appl.middleName) ? appl.middleName + " " + appl.lastName : appl.lastName) : appl.CompanyName,
                                     Email = appl.email,
                                     MobileNumber = appl.mobileNumberP2,
                                     CorrespondingAddress = appl.correspondanceAdd,
                                     ApplicationFee = appl.ApplicationFee,
                                     ProcessingCharge = appl.ProcessingCharge,
                                     FormFeeGST = appl.ApplicationFee + appl.FormCGST + appl.FormSGST,
                                     ProcessingChargeGST = appl.ProcessingCharge + appl.ProcessingSGST + appl.ProcessingCGST,
                                     FormFeeSGST = appl.FormSGST,
                                     FormFeeCGST = appl.FormCGST,
                                     ProcessingSGST = appl.ProcessingSGST,
                                     ProcessingCGST = appl.ProcessingCGST,
                                     TotalAmount = appl.ApplicationFee + appl.ProcessingCharge + appl.EarnestMoney,
                                     EarnestMoney = appl.EarnestMoney,
                                     TotalAmountGST = (appl.ProcessingCharge + appl.ProcessingSGST + appl.ProcessingCGST) + (appl.ApplicationFee + appl.FormCGST + appl.FormSGST) + appl.EarnestMoney,
                                     ChallanBank = dbContext.BankMsts.Where(m => m.bankId.ToString() == trans.GetwayName && m.IsActive == true).FirstOrDefault().bankName + " Branch : " + dbContext.SchemeBankTrans.Where(m => m.bankId.ToString() == trans.GetwayName && m.schemeId == appl.schemeId && m.IsActive == true).FirstOrDefault().BranchMst.branchName,
                                     AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == appl.area && m.IsActive == true).FirstOrDefault().floorName,
                                     SchemeName = dbContext.SchemeMsts.Where(m => m.schemeId == appl.schemeId && m.IsActive == true && m.completed == true && m.Status != Constants.SchemeClosed).FirstOrDefault().schemeName,
                                     Department = dbContext.DepartmentMsts.Where(m => m.departmentId == appl.departmentId && m.IsActive == true).FirstOrDefault().departmentName,
                                     //In offline case BankId place in gateway name , Branch Id Place in Bank code and Account Place in Card Num Field in OnlineApplicationDetails_trans table 23 aug 2017 According to vishal Shukla Sir
                                     ChallanAccountNo = trans.cardnum,
                                     ChallanBranchName = dbContext.BankMsts.Where(m => m.bankId.ToString() == trans.GetwayName && m.IsActive == true).FirstOrDefault().bankName + " " + dbContext.BranchMsts.Where(m => m.branchId.ToString() == trans.bankcode && m.IsActive == true).FirstOrDefault().branchName,
                                     //In Case of applicant GST No saved in fax no 30 Aug 2017
                                     ApplicantGSTNumber = appl.GSTNO,
                                     BankIFSCCode = trans.udf5,
                                     VirtualAccPrefix = trans.virtualaccountno,
                                     ValidTillDate = dbContext.SchemeMsts.Where(m => m.schemeId == appl.schemeId && m.IsActive == true && m.completed == true && m.Status != Constants.SchemeClosed).FirstOrDefault().endDate
                                 },
                                 PaymentModel = new OnlinePaymentViewModel
                                 {
                                     ApplicationFormId = trans.ServiceRefId,
                                     TransactionKey = trans.TrKey,
                                     TransactionId = trans.txnid,
                                     Amount = trans.Amount,
                                     ProductInfo = trans.productinfo,
                                     Mode = trans.mode,
                                     TransactionStatusId = trans.TranStatus,
                                     Discount = trans.discount,
                                     StatusId = trans.status,
                                     EntryDate = trans.EntryDate,
                                 }
                             }).FirstOrDefault();
                return model;
            }
        }

        #endregion

        #region GenerateChallan
        public ChallanModel GeneratePaymentChallan(OnlineChallanViewModel objOnlineChallanViewModel)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var challanIdPK = 0;
                List<OnlineChallanViewModel> modelList = (List<OnlineChallanViewModel>)HttpContext.Current.Session["TempModel"];
                if (modelList != null)
                {
                    Challan_Master challanMaster = new Challan_Master();
                    challanMaster.Rid = objOnlineChallanViewModel.RegistrationId;
                    challanMaster.Bank_Id = objOnlineChallanViewModel.BankId;
                    challanMaster.Branch_Id = objOnlineChallanViewModel.BranchId;
                    challanMaster.Account_Number = objOnlineChallanViewModel.AccountNo;
                    //challanMaster.Content = challan;
                    challanMaster.Created_Date = DateTime.Now;
                    challanMaster.Generate_Date = DateTime.Now;
                    challanMaster.Created_By = userInfo.UserID;
                    challanMaster.Is_Active = true;
                    dbContext.Challan_Master.Add(challanMaster);
                    dbContext.SaveChanges();

                    var challanIdPK = dbContext.Challan_Master.Max(m => m.Id);
                    foreach (var model in modelList)
                    {
                        Challan_Trans trans = new Challan_Trans();
                        trans.Rid = objOnlineChallanViewModel.RegistrationId;
                        trans.Challan_Master_Id = challanIdPK;
                        trans.Head_Id = model.AccountHeadId;
                        trans.Subhead_Id = model.AccountSubHeadId;
                        trans.Amount = model.Amount;
                        trans.Is_Active = true;
                        trans.Created_By = userInfo.UserID;
                        trans.Created_Date = DateTime.Now;
                        dbContext.Challan_Trans.Add(trans);
                        dbContext.SaveChanges();
                    }
                    HttpContext.Current.Session["TempModel"] = null;
                    //return challan;
                    ChallanModel objChallanModel = new ChallanModel();
                    if (objOnlineChallanViewModel.RegistrationId > 0 && objOnlineChallanViewModel.RegistrationId != null)
                    {
                        objChallanModel = (from challan in dbContext.Challan_Master
                                           join alotment in dbContext.AllotmentMasters on challan.Rid equals alotment.rid
                                           join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                                           where challan.Id == challanIdPK
                                           select new ChallanModel
                                           {
                                               RID = challan.Rid.Value,
                                               ChallanId = challan.Challan_Id,
                                               SectorName = property.SectorMst.sectorName,
                                               BlockName = property.BlockMst.blockName,
                                               PropertyTypeName = property.PropertyTypeMst.propertyTypeName,
                                               PropertyNumber = property.propertyNo,
                                               BankName = challan.BranchMst.BankMst.bankName,
                                               ApplicationForm = new ApplicationFormModel
                                               {
                                                   FirstName = alotment.ApplicationDetail.tFirstName,
                                                   MiddleName = alotment.ApplicationDetail.tMiddleName,
                                                   LastName = alotment.ApplicationDetail.tLastName,
                                                   Email = alotment.ApplicationDetail.tEmail,
                                                   MobileNumber = alotment.ApplicationDetail.tMobileNumber,
                                                   PhoneNumber = alotment.ApplicationDetail.tPhoneNumber,
                                                   CorrespondingAddress = alotment.ApplicationDetail.tCorrespondanceAdd
                                               }
                                           }).FirstOrDefault();
                    }
                    else
                    {
                        objChallanModel = (from challan in dbContext.Challan_Master
                                           where challan.Id == challanIdPK
                                           select new ChallanModel
                                           {
                                               ChallanId = challan.Challan_Id,
                                               BankName = challan.BranchMst.BankMst.bankName
                                           }).FirstOrDefault();
                        objChallanModel.RID = objOnlineChallanViewModel.RegistrationId != null ? (int)objOnlineChallanViewModel.RegistrationId : 0;
                        objChallanModel.SectorName = objOnlineChallanViewModel.Sector;
                        objChallanModel.BlockName = objOnlineChallanViewModel.Block;
                        objChallanModel.PropertyTypeName = objOnlineChallanViewModel.PropertyType;
                        objChallanModel.PropertyNumber = objOnlineChallanViewModel.PropertyNo;
                        objChallanModel.ApplicationForm = new ApplicationFormModel { FirstName = objOnlineChallanViewModel.Applicant };
                    }

                    int count = 1;
                    foreach (var model in modelList)
                    {
                        if (count == 1)
                        {
                            objChallanModel.Amount1 = model.Amount;
                            objChallanModel.AccountHeadName1 = model.AccountHeadName;
                            objChallanModel.AccountSubHeadName1 = model.AccountHeadName;
                        }
                        if (count == 2)
                        {
                            objChallanModel.Amount2 = model.Amount;
                            objChallanModel.AccountHeadName2 = model.AccountHeadName;
                            objChallanModel.AccountSubHeadName2 = model.AccountHeadName;
                        }
                        if (count == 3)
                        {
                            objChallanModel.Amount3 = model.Amount;
                            objChallanModel.AccountHeadName3 = model.AccountHeadName;
                            objChallanModel.AccountSubHeadName3 = model.AccountHeadName;
                        }
                        count++;
                    }
                    return objChallanModel;
                }
                else
                {
                    return null;
                }
            }
        }

        public int SaveCreateChallan(int? rId, int AccountHeadId, int AccountSubHeadId, decimal? Amount)
        {
            int flag = 0;
            List<OnlineChallanViewModel> data = (List<OnlineChallanViewModel>)HttpContext.Current.Session["TempModel"];
            if (data == null)
            {
                List<OnlineChallanViewModel> modelList = new List<OnlineChallanViewModel>();
                OnlineChallanViewModel model = new OnlineChallanViewModel();
                model.RegistrationId = rId;
                model.AccountHeadId = AccountHeadId;
                model.AccountSubHeadId = AccountSubHeadId;
                model.Amount = Amount.Value;
                modelList.Add(model);
                HttpContext.Current.Session["TempModel"] = modelList;
            }
            else
            {
                int count = 0;
                foreach (var models in data)
                {
                    if (models.RegistrationId != rId)
                    {
                        data = null;
                        List<OnlineChallanViewModel> modelList = new List<OnlineChallanViewModel>();
                        OnlineChallanViewModel model = new OnlineChallanViewModel();
                        model.RegistrationId = rId;
                        model.AccountHeadId = AccountHeadId;
                        model.AccountSubHeadId = AccountSubHeadId;
                        model.Amount = Amount.Value;
                        modelList.Add(model);
                        HttpContext.Current.Session["TempModel"] = modelList;

                        return flag = 2;
                    }
                    else
                    {
                        count++;
                    }
                }
                if (count >= 3)
                {
                    return flag = 3;
                }
                else
                {
                    OnlineChallanViewModel model = new OnlineChallanViewModel();
                    model.RegistrationId = rId;
                    model.AccountHeadId = AccountHeadId;
                    model.AccountSubHeadId = AccountSubHeadId;
                    model.Amount = Amount.Value;
                    data.Add(model);
                    HttpContext.Current.Session["TempModel"] = data;
                }
            }
            return flag;
        }

        /// <summary>
        /// dynamically generated challan is saved in database
        /// </summary>
        /// <param name="rId"></param>
        /// <param name="bankId"></param>
        /// <param name="branchId"></param>
        /// <param name="DdlAccountNumber"></param>
        /// <param name="DepttId"></param>
        /// <param name="parsedHTML"></param>
        /// <returns></returns>
        public bool SaveGeneratedChallan(int? rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId, string parsedHTML)
        {
            var flag = false; //string flag = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                Challan_Master challanMaster = new Challan_Master();
                challanMaster.Rid = rId;
                challanMaster.Bank_Id = bankId;
                challanMaster.Branch_Id = branchId;
                challanMaster.Account_Number = DdlAccountNumber.ToString();
                challanMaster.Is_Active = true;
                challanMaster.Created_By = userInfo.UserID;
                challanMaster.Created_Date = DateTime.Now;
                challanMaster.Generate_Date = DateTime.Now;
                challanMaster.Content = parsedHTML;
                dbContext.Challan_Master.Add(challanMaster);
                dbContext.SaveChanges();

                var objExit = dbContext.Challan_Master.FirstOrDefault(m => m.Rid == rId && m.Bank_Id == bankId && m.Branch_Id == branchId && m.Is_Active == true && m.Account_Number == DdlAccountNumber);
                var objAllotteeLsts = dbContext.Challan_Trans.Where(i => i.Rid == rId && i.Is_Active == true && i.Challan_Master_Id == null).ToList();

                if (objAllotteeLsts.Count > 0)
                {
                    objAllotteeLsts.Select(ua =>
                    {
                        ua.Challan_Master_Id = objExit.Id;
                        return ua;
                    }).ToList();
                    dbContext.SaveChanges();
                }
                flag = true;
            }
            return flag;
        }

        public bool SaveGeneratedChallan(string challanId, string challan)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var challanDetail = dbContext.Challan_Master.Where(c => c.Challan_Id == challanId).FirstOrDefault();
                challanDetail.Content = challan;
                dbContext.SaveChanges();
                flag = true;
            }
            return flag;
        }

        public string GetAccountNumber(int bankId, int branchId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingRequest = (from m in dbContext.BranchMsts where m.bankId == bankId && m.branchId == branchId && m.IsActive == true select m.accountNumber).FirstOrDefault();
                return existingRequest;
            }
        }

        public bool RemoveChallanChargeDetail(int? rid, string headName, string subHeadName, decimal amount)
        {
            var flag = false;
            List<OnlineChallanViewModel> modelList = (List<OnlineChallanViewModel>)HttpContext.Current.Session["TempModel"];
            if (modelList != null)
            {
                foreach (var model in modelList)
                {
                    if (rid > 0)
                    {
                        if (model.RegistrationId == rid && model.AccountHeadName == headName && model.AccountSubHeadName == subHeadName && model.Amount == amount)
                        {
                            modelList.Remove(model);
                            return flag = true;
                        }
                    }
                    else
                    {
                        if (model.AccountHeadName == headName && model.AccountSubHeadName == subHeadName && model.Amount == amount)
                        {
                            modelList.Remove(model);
                            return flag = true;
                        }
                    }
                }
            }
            return flag;
        }

        public List<OnlineChallanViewModel> GetGeneratedChallanDetails(int rid)
        {
            List<OnlineChallanViewModel> data = (List<OnlineChallanViewModel>)HttpContext.Current.Session["TempModel"];
            if (data == null)
            {
                return null;
            }
            else
            {
                var dbContext = new NoidaPMSEntities();
                foreach (var model in data)
                {
                    model.AccountHeadName = dbContext.RECIEPT_HEAD.Where(h => h.RECIEPT_CODE == model.AccountHeadId).Select(h => h.RECIEPT_HEAD_NAME).FirstOrDefault();
                    model.AccountSubHeadName = dbContext.RECEIPT_SUB_HEAD.Where(s => s.RECEIPT_SUBHEAD_ID == model.AccountSubHeadId).Select(s => s.RECEIPT_SUB_HEAD1).FirstOrDefault();
                }
                return data;
            }
        }
        #endregion

        public bool IsDocumentUploaded(int? formId)
        {
            var flag = false;
            if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formId + "/Documents/")))
            {
                var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formId + "/Documents/");
                string[] files = Directory.GetFiles(filepath);
                if (files != null)
                {
                    if (files.Count() > 0) flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }

        public bool IsPreviousChallanUploaded(int? formId)
        {
            var flag = false;
            if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formId + "/PreviousChallan/")))
            {
                var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formId + "/PreviousChallan/");
                string[] files = Directory.GetFiles(filepath);
                if (files != null)
                {
                    if (files.Count() > 0) flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }


        public DataSourceResult GetDirectorDetails(DataSourceRequest request, int? formId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var directorList = (from directors in dbContext.online_Director_Master
                                    where directors.onlineapplicationId == formId && directors.Is_Active == 1
                                    select new onlineDirectorViewModel
                                    {
                                        Id = directors.Director_Id,
                                        DirectorId = directors.Director_Id,
                                        ApplicationFormId = directors.onlineapplicationId,
                                        DirectorName = directors.Director_Name,
                                        DirectorShare = directors.Director_Share,
                                        PAN = directors.pan_no,
                                        DirectorTypeId = directors.Type,
                                        DirectorType = dbContext.Common_Config.FirstOrDefault(d => d.Id == directors.Type && d.Category.ToLower() == "director" && d.Is_Active == 1).Name
                                    }).ToList();
                int count = 1;
                foreach (var director in directorList)
                {
                    director.Id = count;
                    count++;
                }
                return directorList.ToDataSourceResult(request);
            }

        }

        public int SaveDirectorDetails(int? formId, string directorName, decimal? share, int? directorTypeId, string pan)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                online_Director_Master director = new online_Director_Master();
                director.onlineapplicationId = formId;
                director.Director_Name = directorName;
                director.Director_Share = share;
                director.Type = directorTypeId;
                director.pan_no = pan;
                director.Is_Active = 1;
                director.Created_By = userInfo.UserID;

                dbContext.online_Director_Master.Add(director);
                dbContext.SaveChanges();
                return ReturnType.Success;
            }

        }

        public int RemoveDirectorDetails(int? formId, int? directorId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var director = dbContext.online_Director_Master.FirstOrDefault(x => x.onlineapplicationId == formId && x.Director_Id == directorId);
                if (director != null)
                {
                    dbContext.online_Director_Master.Remove(director);
                    dbContext.SaveChanges();
                    return ReturnType.Success;
                }
                else return ReturnType.Failure;
            }

        }

        public OnlineFormViewModel GetInitialDataForScheme()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                string schemeId = ConfigurationManager.AppSettings["SchemeId"];
                string departmentId = ConfigurationManager.AppSettings["DepartmentId"];
                OnlineFormViewModel model = new OnlineFormViewModel();
                var scheme = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId.ToString() == schemeId && s.IsActive == true);
                if (scheme != null)
                {
                    model.SchemeId = scheme.schemeId;
                    model.SchemeName = scheme.schemeName;
                    model.SchemeEndDate = scheme.endDate;
                    model.SchemeType = dbContext.SchemeTypeMsts.Where(x => x.schemeTypeId == scheme.schemeTypeId).Select(x => x.SchemeTypeDesc).FirstOrDefault();
                }
                var department = dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId.ToString() == departmentId);
                if (department != null)
                {
                    model.DepartmentId = department.departmentId;
                    model.Department = department.departmentName;
                }
                return model;
            }
        }

        public OnlineFormViewModel GetInitialDataForScheme(OnlineFormViewModel objOnlineFormViewModel)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                OnlineFormViewModel model = new OnlineFormViewModel();
                var scheme = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId == objOnlineFormViewModel.SchemeId && s.IsActive == true);
                if (scheme != null)
                {
                    model.SchemeId = scheme.schemeId;
                    model.SchemeName = scheme.schemeName;
                    model.SchemeEndDate = scheme.endDate;
                    model.SchemeType = dbContext.SchemeTypeMsts.Where(x => x.schemeTypeId == scheme.schemeTypeId).Select(x => x.SchemeTypeDesc).FirstOrDefault();
                    model.ApplicationFormType = "General";
                }
                var department = dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == objOnlineFormViewModel.DepartmentId);
                if (department != null)
                {
                    model.DepartmentId = department.departmentId;
                    model.Department = department.departmentName;
                }
                return model;
            }
        }

        public int UpdateCompanyDetail(OnlineFormViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(a => a.onlineapplicationId == model.ApplicationFormId && a.isActive == true);
                form.projectname = model.ProposedModel.ProposedProject;
                form.projectcost = model.ProposedModel.TotalCost.ToString();
                form.projecttimeempl = model.ProposedModel.ImplementationTime;
                form.Online_offline = model.FormType == "Online" ? "Y" : "N";
                dbContext.SaveChanges();
                return form.onlineapplicationId;
            }
        }

        public OnlinePaymentViewModel GetOfflinePayment_Trans(int ApplicationFormId)
        {
            OnlinePaymentViewModel objOnlinePaymentModel = new OnlinePaymentViewModel();
            using (var dbContext = new NoidaPMSEntities())
            {

                objOnlinePaymentModel = (from transaction in dbContext.OnlineApplicationDetails_trans
                                         join details in dbContext.OnlineApplicationDetails on transaction.ServiceRefId equals details.onlineapplicationId
                                         where transaction.ServiceType == 3 && transaction.status == 0 && details.isActive == true && details.onlineapplicationId == ApplicationFormId
                                         select new OnlinePaymentViewModel
                                         {
                                             ApplicationFormId = transaction.ServiceRefId,
                                             TransactionKey = transaction.TrKey,
                                             TransactionId = transaction.txnid,
                                             Amount = transaction.Amount,
                                             ProductInfo = transaction.productinfo,
                                             Mode = transaction.mode,
                                             TransactionStatusId = transaction.TranStatus,
                                             Discount = transaction.discount,
                                             StatusId = transaction.status,
                                             EntryDate = transaction.EntryDate,
                                             Applicant = details.gender == Constants.genderCompany ? details.CompanyName : details.firstName + " " + details.middleName + "" + details.lastName,
                                             FirstName = details.gender == Constants.genderCompany ? details.CompanyName : details.firstName + " " + details.middleName + "" + details.lastName,
                                             Email = details.email,
                                             PhoneNumber = details.mobileNumberP1,
                                             Status = transaction.TranStatus == Constants.Success ? "Success" : "Failure",
                                             PaymentSource = transaction.payment_source,
                                             BankReferenceNo = transaction.bank_ref_num,
                                             BankCode = transaction.bankcode,
                                             Error = transaction.error,
                                             ErrorMessage = transaction.error_Message,
                                             NameOnCard = transaction.name_on_card,
                                             CardNumber = transaction.cardnum,
                                             CardHash = transaction.cardhash,
                                             IssuingBank = transaction.issuing_bank,
                                             CardType = transaction.card_type,
                                             Mihpayid = transaction.mihpayid,
                                             modifiydate = transaction.modifiydate,
                                             BankName = dbContext.BankMsts.Where(m => m.bankId.ToString() == transaction.GetwayName && m.IsActive == true).FirstOrDefault().bankName,
                                             FormModel = new OnlineFormViewModel
                                             {
                                                 ApplicationFee = details.ApplicationFee,
                                                 ProcessingCharge = details.ProcessingCharge,
                                                 FormFeeGST = details.ApplicationFee + details.FormCGST + details.FormSGST,
                                                 ProcessingChargeGST = details.ProcessingCharge + details.ProcessingSGST + details.ProcessingCGST,
                                                 FormFeeSGST = details.FormSGST,
                                                 FormFeeCGST = details.FormCGST,
                                                 ProcessingSGST = details.ProcessingSGST,
                                                 ProcessingCGST = details.ProcessingCGST,
                                                 TotalAmount = details.ApplicationFee + details.ProcessingCharge + details.EarnestMoney,
                                                 EarnestMoney = details.EarnestMoney,
                                                 TotalAmountGST = (details.ProcessingCharge + details.ProcessingSGST + details.ProcessingCGST) + (details.ApplicationFee + details.FormCGST + details.FormSGST) + details.EarnestMoney,
                                             }
                                         }).FirstOrDefault();

                string filePath = GetDocumentUploadedByID((int)objOnlinePaymentModel.ApplicationFormId, "Challan");
                if (!string.IsNullOrEmpty(filePath)) { objOnlinePaymentModel.docPath = filePath; }
            }
            return objOnlinePaymentModel;
        }

        public OnlinePaymentViewModel GetPreviousChallanPayment_Trans(int ApplicationFormId)
        {
            OnlinePaymentViewModel objOnlinePaymentModel = new OnlinePaymentViewModel();
            using (var dbContext = new NoidaPMSEntities())
            {

                objOnlinePaymentModel = (from transaction in dbContext.OnlineApplicationDetails_trans
                                         join details in dbContext.OnlineApplicationDetails on transaction.ServiceRefId equals details.onlineapplicationId
                                         where transaction.ServiceType == 4 && transaction.status == 0 && details.isActive == true && details.onlineapplicationId == ApplicationFormId
                                         select new OnlinePaymentViewModel
                                         {
                                             ApplicationFormId = transaction.ServiceRefId,
                                             TransactionKey = transaction.TrKey,
                                             TransactionId = transaction.txnid,
                                             Amount = transaction.Amount,
                                             ProductInfo = transaction.productinfo,
                                             Mode = transaction.mode,
                                             TransactionStatusId = transaction.TranStatus,
                                             Discount = transaction.discount,
                                             StatusId = transaction.status,
                                             EntryDate = transaction.EntryDate,
                                             Applicant = details.gender == Constants.genderCompany ? details.CompanyName : details.firstName + " " + details.middleName + "" + details.lastName,
                                             FirstName = details.gender == Constants.genderCompany ? details.CompanyName : details.firstName + " " + details.middleName + "" + details.lastName,
                                             Email = details.email,
                                             PhoneNumber = details.mobileNumberP1,
                                             Status = transaction.TranStatus == Constants.Success ? "Success" : "Failure",
                                             PaymentSource = transaction.payment_source,
                                             BankReferenceNo = transaction.bank_ref_num,
                                             BankCode = transaction.bankcode,
                                             Error = transaction.error,
                                             ErrorMessage = transaction.error_Message,
                                             NameOnCard = transaction.name_on_card,
                                             CardNumber = transaction.cardnum,
                                             CardHash = transaction.cardhash,
                                             IssuingBank = transaction.issuing_bank,
                                             CardType = transaction.card_type,
                                             Mihpayid = transaction.mihpayid,
                                             modifiydate = transaction.modifiydate,
                                             BankName = dbContext.BankMsts.Where(m => m.bankId.ToString() == transaction.GetwayName && m.IsActive == true).FirstOrDefault().bankName,
                                             FormModel = new OnlineFormViewModel
                                             {
                                                 ApplicationFee = details.ApplicationFee,
                                                 ProcessingCharge = details.ProcessingCharge,
                                                 FormFeeGST = details.ApplicationFee + details.FormCGST + details.FormSGST,
                                                 ProcessingChargeGST = details.ProcessingCharge + details.ProcessingSGST + details.ProcessingCGST,
                                                 FormFeeSGST = details.FormSGST,
                                                 FormFeeCGST = details.FormCGST,
                                                 ProcessingSGST = details.ProcessingSGST,
                                                 ProcessingCGST = details.ProcessingCGST,
                                                 TotalAmount = details.ApplicationFee + details.ProcessingCharge + details.EarnestMoney,
                                                 EarnestMoney = details.EarnestMoney,
                                                 TotalAmountGST = (details.ProcessingCharge + details.ProcessingSGST + details.ProcessingCGST) + (details.ApplicationFee + details.FormCGST + details.FormSGST) + details.EarnestMoney,
                                             }
                                         }).FirstOrDefault();

                string filePath = GetDocumentUploadedByID((int)objOnlinePaymentModel.ApplicationFormId, "PreviousChallan");
                if (!string.IsNullOrEmpty(filePath)) { objOnlinePaymentModel.docPath = filePath; }
            }
            return objOnlinePaymentModel;
        }

        public string GetDocumentUploadedByID(int formId, string DocType)
        {
            string DocumentPath = string.Empty;
            if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formId + "/" + DocType + "/")))
            {
                var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formId + "/" + DocType + "/");
                string[] files = Directory.GetFiles(filepath);
                if (files != null)
                {
                    if (files.Count() > 0)
                    { string[] uFile = files[0].Split('\\'); string fileName = uFile[(uFile).Length - 1]; DocumentPath = "/UploadFiles/" + formId + "/" + DocType + "/" + fileName; }
                }
            }
            return DocumentPath;
        }

        public DataSourceResult GetApplicationFormIdForOfflinePayment(DataSourceRequest Req, int? ApplicationFormId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (ApplicationFormId != null && ApplicationFormId > 0)
                {
                    var ndata = (from transaction in dbContext.OnlineApplicationDetails_trans
                                 join details in dbContext.OnlineApplicationDetails on transaction.ServiceRefId equals details.onlineapplicationId
                                 where transaction.ServiceType == 3 && transaction.status == 0 && details.isActive == true && details.onlineapplicationId == ApplicationFormId
                                 select new DropDownListItem
                                 {
                                     Text = details.onlineapplicationId.ToString(),
                                     Value = details.onlineapplicationId.ToString()
                                 });
                    return ndata.ToDataSourceResult(Req);
                }
                else
                {
                    var sdata = (from transaction in dbContext.OnlineApplicationDetails_trans
                                 join details in dbContext.OnlineApplicationDetails on transaction.ServiceRefId equals details.onlineapplicationId
                                 where transaction.ServiceType == 3 && transaction.status == 0 && details.isActive == true
                                 select new DropDownListItem
                                 {
                                     Text = details.onlineapplicationId.ToString(),
                                     Value = details.onlineapplicationId.ToString()
                                 });
                    return sdata.ToDataSourceResult(Req);
                }
            }
        }

        public string UpdateOfflinePayment(OnlineFormViewModel objOnlineFormViewModel, HttpPostedFileBase files)
        {
            string bFlag = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from transaction in dbContext.OnlineApplicationDetails_trans
                            join details in dbContext.OnlineApplicationDetails on transaction.ServiceRefId equals details.onlineapplicationId
                            where transaction.ServiceType == 3 && transaction.status == 0 && details.isActive == true && details.onlineapplicationId == objOnlineFormViewModel.PaymentModel.ApplicationFormId
                            select transaction).FirstOrDefault();
                if (data != null)
                {
                    if (data.txnid == objOnlineFormViewModel.PaymentModel.TransactionId)
                    {
                        var trans = dbContext.OnlineApplicationDetails_trans.Where(m => m.AutoID == data.AutoID && m.status == 0).FirstOrDefault();
                        if (trans != null)
                        {
                            SaveChallanDocumentsForApplicationForm(objOnlineFormViewModel, files);
                            trans.TrKey = objOnlineFormViewModel.PayType == "RTGS" ? objOnlineFormViewModel.PaymentModel.Udf1 : objOnlineFormViewModel.PaymentModel.BankReferenceNo;
                            trans.modifiydate = DateTime.Now;//objOnlineFormViewModel.PaymentModel.EntryDate;
                            dbContext.SaveChanges();
                            bFlag = "Updated Successfully";
                        }
                    }
                    else
                    {
                        bFlag = "2";
                    }
                }
                else { bFlag = "challan Details (RTGS/NEFT) already updated."; }

            }
            return bFlag;
        }

        private int SaveChallanDocumentsForApplicationForm(OnlineFormViewModel model, HttpPostedFileBase docs)
        {
            int flag = 0;
            if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.PaymentModel.ApplicationFormId)))
            {
                if (docs != null && docs.ContentLength > 0)
                {
                    string challan = "Challan";
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.PaymentModel.ApplicationFormId + "/" + challan)))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.PaymentModel.ApplicationFormId + "/" + challan));
                    }
                    string extension = Path.GetExtension(docs.FileName);
                    var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.PaymentModel.ApplicationFormId + "/" + challan + "/" + model.PaymentModel.ApplicationFormId + "-" + model.PaymentModel.TransactionId + extension);
                    docs.SaveAs(fileSavePath);
                    flag = ReturnType.Success;
                }
            }
            return flag;
        }

        public int SendMessageInBulk(string type)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                string schemeId = "119119";// ConfigurationManager.AppSettings["SchemeId"];
                string departmentId = "4";// ConfigurationManager.AppSettings["DepartmentId"];
                List<int> formList = new List<int>();

                if (type == "Draw" || type == "DrawVenue")
                {
                    formList = (from aplication in dbContext.OnlineApplicationDetails
                                join transction in dbContext.OnlineApplicationDetails_trans on aplication.onlineapplicationId equals transction.ServiceRefId
                                where aplication.schemeId == 119119 && aplication.departmentId == 4 && aplication.isActive == true && transction.status == 1 && transction.TranStatus == 1 //&& aplication.PropertyTypeID == 2
                                select aplication.onlineapplicationId).ToList();
                }
                if (type == "FormUploaded")
                {
                    var date1 = Convert.ToDateTime("2018/01/01");
                    var date2 = Convert.ToDateTime("2018/01/31");
                    formList = (from aplication in dbContext.OnlineApplicationDetails
                                //join transction in dbContext.OnlineApplicationDetails_trans on aplication.onlineapplicationId equals transction.ServiceRefId
                                where aplication.schemeId == 139139 && aplication.departmentId == 4 && DbFunctions.TruncateTime(aplication.createdDate) >= DbFunctions.TruncateTime(date1) && DbFunctions.TruncateTime(aplication.createdDate) <= DbFunctions.TruncateTime(date2)
                                select aplication.onlineapplicationId).ToList();
                }
                if (type == "Other")
                {
                    formList = (from aplication in dbContext.OnlineApplicationDetails
                                where aplication.schemeId == 139139 && aplication.FormCategory == "FinalDraw"
                                select aplication.onlineapplicationId).ToList();

                    //formList = (from aplication in dbContext.OnlineApplicationDetails
                    //            join transction in dbContext.OnlineApplicationDetails_trans on aplication.onlineapplicationId equals transction.ServiceRefId
                    //            where aplication.schemeId == 119119 && aplication.departmentId == 4 && aplication.isActive == true 
                    //            && transction.status == 1 && transction.TranStatus == 1 
                    //            && aplication.Photographfilename != "1"
                    //                select aplication.onlineapplicationId).ToList();

                }

                if (formList != null)
                {
                    if (type == "Draw" || type == "DrawVenue")
                    {
                        foreach (var id in formList)
                        {
                            var form = dbContext.OnlineApplicationDetails.Where(c => c.onlineapplicationId == id).FirstOrDefault();

                            if (form != null)
                            {
                                string body = string.Empty;
                                if (type == "DrawVenue")
                                {
                                    body = string.Format(NAMessages.DrawVenueUpdate, "NOIDA/IP/2017-18/01");
                                    if (!string.IsNullOrEmpty(form.email)) { ApplicationHelper.SendEmail(form.email, "Scheme Draw Info", body); }
                                    if (!string.IsNullOrEmpty(form.mobileNumberP2)) { ApplicationHelper.SendSMS(form.mobileNumberP2, body); }
                                }
                                else
                                {
                                    body = string.Format(NAMessages.SchemeDrawMessage, "NOIDA/IP/2017-18/01", "19-01-2018");
                                    if (!string.IsNullOrEmpty(form.email)) { ApplicationHelper.SendEmail(form.email, "Scheme Draw Info", body); }
                                    if (!string.IsNullOrEmpty(form.mobileNumberP2)) { ApplicationHelper.SendSMS(form.mobileNumberP2, body); }
                                }


                                var audit = new Audit();
                                audit.TableName = "OnlineApplicationDetail";
                                audit.Type = "S";
                                //audit.FormName = form.onlineapplicationId.ToString();
                                audit.FormName = "ManageEmail";
                                audit.PrimaryKeyField = "Scheme Draw Info";
                                audit.PrimaryKeyValue = body;
                                audit.FieldName = form.mobileNumberP2;
                                audit.OldValue = form.mobileNumberP2;
                                audit.NewValue = body;
                                audit.UpdateDate = DateTime.Now;
                                audit.UserName = userInfo.UserID.ToString();
                                dbContext.Audits.Add(audit);

                                form.IsSubmited = true;

                                dbContext.SaveChanges();
                            }
                        }
                        flag = ReturnType.Success;
                    }

                    if (type == "FormUploaded")
                    {
                        foreach (var id in formList)
                        {
                            var form = dbContext.OnlineApplicationDetails.Where(c => c.onlineapplicationId == id).FirstOrDefault();
                            if (form != null)
                            {
                                string body = string.Empty;
                                string message1 = string.Empty;
                                string message2 = string.Empty;
                                message1 = string.Format(NAMessages.OnlineApplication01, "NOIDA/TN/2017-18/02", "9/3/2018");
                                //message2 = string.Format("Info2-" + NAMessages.OnlineSchemeInfo2);
                                body = message1;
                                if (!string.IsNullOrEmpty(form.email)) { ApplicationHelper.SendEmail(form.email, "Scheme Draw Info", body); }
                                if (!string.IsNullOrEmpty(form.mobileNumberP2)) { ApplicationHelper.SendSMS(form.mobileNumberP2, message1); }
                                //if (!string.IsNullOrEmpty(form.mobileNumberP2)) { ApplicationHelper.SendSMS(form.mobileNumberP2, message2); }

                                var audit = new Audit();
                                audit.TableName = "OnlineApplicationDetail";
                                audit.Type = "T";
                                audit.FormName = form.onlineapplicationId.ToString();
                                audit.PrimaryKeyField = "Scheme Draw Info";
                                audit.PrimaryKeyValue = body;
                                audit.FieldName = form.mobileNumberP2;
                                audit.OldValue = form.mobileNumberP2;
                                audit.NewValue = body;
                                audit.UpdateDate = DateTime.Now;
                                audit.UserName = userInfo.UserID.ToString();
                                dbContext.Audits.Add(audit);

                                form.IsSubmited = true;

                                dbContext.SaveChanges();
                            }
                        }
                        flag = ReturnType.Success;
                    }

                    if (type == "Other")
                    {
                        foreach (var id in formList)
                        {
                            var form = dbContext.OnlineApplicationDetails.Where(c => c.onlineapplicationId == id).FirstOrDefault();
                            if (form != null)
                            {
                                string body = string.Empty;
                                string message1 = string.Empty;
                                string message2 = string.Empty;
                                message1 = string.Format(NAMessages.OnlineTransportScheme201805, "NOIDA/TN/2017-18/02");
                                //message2 = string.Format(NAMessages.SchemeRefundInfo02,"supportnoida@noidaauthorityonline.com");
                                body = message1;
                                if (!string.IsNullOrEmpty(form.email)) { ApplicationHelper.SendEmail(form.email, "Transport Scheme Info", body); }
                                if (!string.IsNullOrEmpty(form.mobileNumberP2)) { ApplicationHelper.SendSMS(form.mobileNumberP2, message1); }
                                //if (!string.IsNullOrEmpty(form.mobileNumberP2)) { ApplicationHelper.SendSMS(form.mobileNumberP2, message2); }

                                var audit = new Audit();
                                audit.TableName = "OnlineApplicationDetail";
                                audit.Type = "T";
                                audit.FormName = "NOIDA/TN/2017-18/02";// form.onlineapplicationId.ToString();
                                audit.PrimaryKeyField = form.onlineapplicationId.ToString();// "Transport Scheme Info";
                                audit.PrimaryKeyValue = body;
                                audit.FieldName = "Transport Final Draw"; // form.mobileNumberP2;
                                audit.OldValue = form.mobileNumberP2;
                                audit.NewValue = body;
                                audit.UpdateDate = DateTime.Now;
                                audit.UserName = userInfo.UserID.ToString();
                                dbContext.Audits.Add(audit);
                                //form.IsSubmited = true;
                                dbContext.SaveChanges();
                            }
                        }
                        flag = ReturnType.Success;
                    }


                }
            }
            return flag;
        }
        #endregion

        #region Scheme 2018 - (Open Ended and Transport)

        public int ValidateApplicationDetails(OnlineFormViewModel model)
        {
            int flag = ReturnType.Failed;
            string Password = string.Empty;
            if (model.AppType == Constants.AppType)
            {
                Password = (model.UserPassword);
            }
            else
            {
                Password = (model.UserPassword).ToMD5HashForPasswordPIS();
            }

            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.OnlineApplicationDetails.FirstOrDefault(m => m.onlineapplicationId == model.ApplicationFormId);
                if (data != null)
                {
                    if (data.Userpassword == Password)
                    {
                        if (data.isActive == true) { return flag = ReturnType.Exist; }
                        else { return flag = ReturnType.Failure; }
                    }
                    else { return flag = ReturnType.PasswordNotExist; }
                }
                else { return flag = ReturnType.UserNameNotExist; }
            }
        }

        public int ValidateApplicationDetailsforForgotPassword(OnlineFormViewModel ObjOnlineFormViewModel)
        {
            int flag = ReturnType.Failed;
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.OnlineApplicationDetails.FirstOrDefault(m => m.onlineapplicationId == ObjOnlineFormViewModel.ApplicationFormId && m.email == ObjOnlineFormViewModel.Email && m.mobileNumberP2 == ObjOnlineFormViewModel.MobileNumber);
                if (data != null)
                {
                    if (data.isActive == true)
                    {
                        string PassWord = ApplicationHelper.GeneratePassWordForScheme();
                        string Message = string.Format(NAMessages.PasswordChange, PassWord);
                        data.Userpassword = PassWord.ToMD5HashForPasswordPIS();
                        dbContext.SaveChanges();
                        //send message to user/applicant
                        ApplicationHelper.SendEmail(ObjOnlineFormViewModel.Email, "Password Reset", Message);
                        ApplicationHelper.SendSMS(ObjOnlineFormViewModel.MobileNumber, Message);
                        return flag = ReturnType.Exist;
                    }
                    else { return flag = ReturnType.Failure; }
                }
                else { return flag = ReturnType.UserNameNotExist; }
            }
        }

        //Get details
        public OnlineFormViewModel GetOnlineSchemeFormById(int? id)
        {
            OnlineFormViewModel applicant = new OnlineFormViewModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == id && c.isActive == true);
                if (detail != null)
                {
                    applicant = (from appl in dbContext.OnlineApplicationDetails
                                 join dept in dbContext.DepartmentMsts on appl.departmentId equals dept.departmentId
                                 join scem in dbContext.SchemeMsts on appl.schemeId equals scem.schemeId
                                 where appl.onlineapplicationId == id
                                 select new OnlineFormViewModel
                                 {
                                     Id = appl.onlineapplicationId,
                                     ApplicationFormId = appl.onlineapplicationId,
                                     SchemeId = scem.schemeId,
                                     SchemeName = scem.schemeName,
                                     SchemeType = scem.SchemeTypeMst.SchemeTypeDesc,
                                     DepartmentId = dept.departmentId,
                                     Department = dept.departmentName,
                                     Applicant = appl.gender == "Company" ? appl.CompanyName : appl.firstName + " " + appl.middleName + " " + appl.lastName,
                                     SigningAuthority = appl.signingAuthority,
                                     ApplicantType = appl.gender == "Company" ? "Company" : "Individual",
                                     CompanyTypeId = appl.CompanyType,
                                     CompanyType = dbContext.Common_Config.Where(c => c.Id == appl.CompanyType).Select(c => c.Name).FirstOrDefault(),
                                     FirstName = appl.firstName,
                                     MiddleName = appl.middleName,
                                     LastName = appl.lastName,
                                     Gender = appl.gender,
                                     MaritalStatus = appl.marritalStatus,
                                     DOB = appl.dateOfBirth,
                                     MobileNumber = appl.mobileNumberP2,
                                     Email = appl.email,
                                     UserPassword = appl.Userpassword,
                                     ScrutinyStatusId = appl.ScrutinyStatusId,
                                     //AreaRangeId = appl.area,
                                     //AreaRange = dbContext.FloorMsts.Where(f => f.floorId.ToString() == appl.area).Select(f => f.floorName).FirstOrDefault(),
                                     //PropertyTypeId = appl.PropertyTypeID,
                                     //PropertyType = dbContext.PropertyTypeMsts.FirstOrDefault(p => p.departmentId == appl.departmentId && p.propertyTypeId == appl.PropertyTypeID && p.IsActive == true).propertyTypeName,
                                 }).FirstOrDefault();
                }
            }
            return applicant;
        }

        public int SaveDirectorDetailsForOpenScheme(onlineDirectorViewModel model)
        {
            var flag = ReturnType.None;
            List<onlineDirectorViewModel> templist = (List<onlineDirectorViewModel>)HttpContext.Current.Session["TempCompanyDirectors"];

            if (templist == null)
            {
                var directorlist = new List<onlineDirectorViewModel>();
                model.status = true;
                directorlist.Add(model);
                HttpContext.Current.Session["TempCompanyDirectors"] = directorlist;
                flag = ReturnType.Saved;
            }
            else
            {
                model.status = true;
                templist.Add(model);
                HttpContext.Current.Session["TempCompanyDirectors"] = templist;
                flag = ReturnType.Saved;
            }
            return flag;
        }

        public DataSourceResult GetDirectorDetailsForOpenScheme(DataSourceRequest request, int? id)
        {
            if (id != null && id > 0)
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    var directorlist = dbContext.online_Director_Master.Where(d => d.onlineapplicationId == id).ToList();
                    if (directorlist != null && directorlist.Count > 0)
                    {
                        var Dirlist = (from directors in dbContext.online_Director_Master
                                       where directors.onlineapplicationId == id && directors.Is_Active == 1
                                       select new onlineDirectorViewModel
                                       {
                                           Id = directors.Director_Id,
                                           DirectorId = directors.Director_Id,
                                           ApplicationFormId = directors.onlineapplicationId,
                                           DirectorName = directors.Director_Name,
                                           DirectorShare = directors.Director_Share,
                                           PAN = directors.pan_no,
                                           DirectorTypeId = directors.Type,
                                           status = directors.Is_Active == 1 ? true : false,
                                           DirectorType = dbContext.Common_Config.FirstOrDefault(d => d.Id == directors.Type && d.Category.ToLower() == "director" && d.Is_Active == 1).Name
                                       }).ToList();


                        if (HttpContext.Current.Session["TempCompanyDirectors"] != null)
                        {
                            List<onlineDirectorViewModel> Sessiondata = (List<onlineDirectorViewModel>)HttpContext.Current.Session["TempCompanyDirectors"];

                            Sessiondata = Sessiondata.Where(m => m.status == true).ToList();
                            int count = 1;
                            foreach (var director in Sessiondata)
                            {
                                director.Id = count;
                                count++;
                            }
                            return Sessiondata.ToDataSourceResult(request);
                        }
                        else
                        {
                            int count = 1;
                            foreach (var director in Dirlist)
                            {
                                director.Id = count;
                                count++;
                            }

                            HttpContext.Current.Session["TempCompanyDirectors"] = Dirlist;
                            return Dirlist.ToDataSourceResult(request);
                        }
                    }
                    else
                    {
                        var templist = (List<onlineDirectorViewModel>)HttpContext.Current.Session["TempCompanyDirectors"];
                        if (templist != null)
                        {
                            var data = templist.Where(m => m.status == true).ToList();
                            if (data != null)
                            {
                                int count = 1;
                                foreach (var director in data)
                                {
                                    director.Id = count;
                                    count++;
                                }
                                return data.ToDataSourceResult(request);
                            } return null;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            else
            {
                var templist = (List<onlineDirectorViewModel>)HttpContext.Current.Session["TempCompanyDirectors"];
                if (templist != null)
                {
                    var data = templist.Where(m => m.status == true).ToList();
                    if (data != null)
                    {
                        int count = 1;
                        foreach (var director in data)
                        {
                            director.Id = count;
                            count++;
                        }
                        return data.ToDataSourceResult(request);
                    } return null;
                }
                else
                {
                    return null;
                }
            }
        }

        public int RemoveDirectorDetailsForOpenScheme(int? id)
        {
            var flag = ReturnType.None;
            var templist = (List<onlineDirectorViewModel>)HttpContext.Current.Session["TempCompanyDirectors"];
            if (templist != null)
            {
                //templist.RemoveAt((int)id - 1);
                var data = templist.FirstOrDefault(m => m.Id == id && m.status == true);
                data.status = false;
                flag = ReturnType.Removed;
            }
            else
            {
                flag = ReturnType.NotExist;
            }
            return flag;
        }

        public string SaveOpenSchemeFormDetail(OnlineFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            string encryptedId = string.Empty;
            int nicmsg = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        OnlineApplicationDetail application = new OnlineApplicationDetail();
                        var detail = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == model.Id);
                        if (detail == null)
                        {
                            if (model.ApplicantType == "Individual")
                            {
                                application.firstName = model.FirstName;
                                application.middleName = model.MiddleName;
                                application.lastName = model.LastName;
                                application.fatherHusbandName = model.FatherName;
                                application.motherName = model.MotherName;
                                application.gender = model.Gender;
                                application.religionId = model.ReligionId;
                                application.occupationId = model.OccupationId;
                                //application.quotaId = model.CategoryId;
                                application.quotaId = Constants.General;
                                application.marritalStatus = model.MaritalStatus;
                                application.dateOfBirth = model.DOB;
                            }
                            else
                            {
                                application.CompanyName = model.Applicant;
                                application.signingAuthority = model.SigningAuthority;
                                application.registeredOffice = model.PermanentAddress;
                                application.CompanyType = model.CompanyTypeId;
                                application.gender = "Company";
                                application.fatherHusbandName = model.ApplicantMaster;
                                application.quotaId = model.SigningAuthorityId;
                            }
                            application.CompanyType = model.CompanyTypeId;
                            application.schemeId = model.SchemeId;
                            application.departmentId = model.DepartmentId;
                            application.permanentAdd = model.PermanentAddress;
                            application.correspondanceAdd = model.CorrespondingAddress;
                            application.area = model.AreaRangeId;
                            application.Phase = Convert.ToInt32(model.AreaRange);
                            application.mobileNumberP2 = model.MobileNumber;
                            application.phoneNumberP2 = model.PhoneNumber;
                            application.faxNumberP2 = model.FaxNumber;
                            application.email = model.Email;
                            application.annualIncome = model.AnnualIncome;
                            application.ApplicationFee = model.ApplicationFee;
                            application.FormCGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.FormSGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.ProcessingCharge = model.ProcessingCharge;
                            application.ProcessingCGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.ProcessingSGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.EarnestMoney = model.EarnestMoney;
                            application.TotalAmount = model.TotalAmount;
                            application.pan = model.PanNumber;
                            application.AadharNumber = model.AadharNumber;
                            //application.faxNumberP2 = model.ApplicantGSTNumber;
                            application.GSTNO = model.ApplicantGSTNumber;
                            application.ApplicationDate = DateTime.Now;

                            application.RefundBankId = model.RefundBankId;
                            application.RefundInfaverof = model.RefundInfaverof;
                            application.RefundaccountNo = model.RefundAccountNo;

                            application.PropertyTypeID = model.PropertyTypeId;
                            application.Sector = model.Sector;
                            application.BranchName = model.BranchName;
                            application.IFSCCode = model.IFSCCode;

                            application.FormCategory = model.ApplicationFormType;
                            if (model.ApplicationFormType == "Expansion")
                            {
                                application.FormSubCategory = model.ExpansionType;
                                application.RentingDate = model.LetterDate;
                                application.RentingLetterNo = model.LetterCode;
                                application.RentingPropertyNo = model.PropertyNo;
                            }

                            application.PropertyNo = model.PropertyNo;
                            application.ExistingPropertyNo = model.ExistingProperty;
                            application.AllotmentDate = model.AllottmentDate;
                            application.DispatchDate = model.DispatchDate;

                            application.createdBy = "Online";
                            application.createdDate = DateTime.Now;
                            application.isActive = true;
                            application.IsSubmited = false;

                            application.projectname = model.ProposedModel.ProposedProject;
                            application.projectcost = model.ProposedModel.TotalCost.ToString();
                            application.projecttimeempl = model.ProposedModel.ImplementationTime;
                            application.Online_offline = model.FormType == "Online" ? "Y" : "N";

                            string PassWord = string.Empty;
                            if (model.SchemeType == OnlineSchemeType.OpenEnded || model.SchemeType == OnlineSchemeType.Transport)
                            {
                                PassWord = ApplicationHelper.GeneratePassWordForScheme();
                                application.Userpassword = PassWord.ToMD5HashForPasswordPIS();
                            }

                            dbContext.OnlineApplicationDetails.Add(application);
                            dbContext.SaveChanges();

                            //Method to save NIC Get Values.
                            if (model.BasicDetailsGetModel != null)
                            {
                                if (!string.IsNullOrEmpty(model.BasicDetailsGetModel.Table.Control_ID))
                                {
                                    model.BasicDetailsGetModel.Table.OnlineApplicationId = application.onlineapplicationId;
                                    model.BasicDetailsGetModel.Table.departmentId = (int)application.departmentId;
                                    model.BasicDetailsGetModel.Table.SchemeId = (int)application.schemeId;
                                    if (ReturnType.Saved == ValidateNICSingleWindowData(application))
                                    {
                                        NICsingalwindowSystem objNICsingalwindowSystem = new NICsingalwindowSystem();
                                        objNICsingalwindowSystem = MapNICsingalwindowSystemTable(model.BasicDetailsGetModel);
                                        objNICsingalwindowSystem.ServiceID = objNICsingalwindowSystem.ServiceID == null ? model.NICServiceId : objNICsingalwindowSystem.ServiceID;
                                        dbContext.NICsingalwindowSystems.Add(objNICsingalwindowSystem);
                                        dbContext.SaveChanges();
                                        nicmsg = 1;
                                    }
                                }
                            }

                            

                            int id = application.onlineapplicationId;
                            model.Id = id;
                            model.ApplicationFormId = id;
                            SaveDocumentsForApplicationForm(model, null, userImage, signatureImage);

                            UpdateDirectorDetailsForOpenSchemeForm(id);

                            string message = string.Format(NAMessages.OnlineApplicationSubmitted, id);
                            ApplicationHelper.SendEmail(model.Email, "Registration Form", message);
                            ApplicationHelper.SendSMS(model.MobileNumber, message);



                            if (model.SchemeType == OnlineSchemeType.OpenEnded || model.SchemeType == OnlineSchemeType.Transport)
                            {
                                if (nicmsg == 0)
                                {
                                    string mobileMessage = string.Format(NAMessages.PIS_Registration_Activation, id, PassWord);
                                    string emailMessage = string.Format(NAMessages.PIS_Registration_Activation, id, PassWord);
                                    if (model.MobileNumber != null && model.MobileNumber != "") ApplicationHelper.SendSMS(model.MobileNumber, mobileMessage);
                                    if (model.Email != null && model.Email != "") ApplicationHelper.SendEmail(model.Email, "OnlineForm", emailMessage);
                                }

                            }

                            encryptedId = CommonHelper.Encode(model.Id.ToString());
                        }
                        else
                        {
                            int flag = UpdateOpenSchemeForm(model, userImage, signatureImage);
                            encryptedId = CommonHelper.Encode(model.Id.ToString());
                        }
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        return string.Empty;
                        scope.Dispose();
                    }
                }
            }
            return encryptedId;
        }

        private int ValidateNICSingleWindowData(OnlineApplicationDetail objOnlineApplicationDetails)
        {
            var flag = ReturnType.None;
            if (objOnlineApplicationDetails.onlineapplicationId != null)
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    var detail = dbContext.NICsingalwindowSystems.FirstOrDefault(c => c.onlineapplicationId == objOnlineApplicationDetails.onlineapplicationId);
                    if (detail == null)
                    {
                        return flag = ReturnType.Saved;
                    }
                    else { return flag; }
                }
            }
            return flag;
        }

        public NICsingalwindowSystem GetNICSingleWindowData(OnlineFormViewModel objOnlineApplicationDetails)
        {
            NICsingalwindowSystem detail = new NICsingalwindowSystem();
            if (objOnlineApplicationDetails.ApplicationFormId != null)
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    detail = dbContext.NICsingalwindowSystems.FirstOrDefault(c => c.onlineapplicationId == objOnlineApplicationDetails.ApplicationFormId);
                    if (detail != null)
                    {
                        return detail;
                    }
                    else { return detail; }
                }
            }
            return detail;
        }

        public int SaveNICSingleWindowPayment(NewDataSet ObjWBasicDetailsGetModel)
        {
            int flag = ReturnType.None;
            if (ObjWBasicDetailsGetModel != null)
            {
                if (ObjWBasicDetailsGetModel.Table.OnlineApplicationId != null)
                {
                    using (var dbContext = new NoidaPMSEntities())
                    {
                        var onlineApplication = dbContext.OnlineApplicationDetails.Where(m => m.onlineapplicationId == ObjWBasicDetailsGetModel.Table.OnlineApplicationId && m.isActive == true).FirstOrDefault();
                        if (onlineApplication != null)
                        {
                            var detail = dbContext.NICsingalwindowSystems.FirstOrDefault(c => c.onlineapplicationId == ObjWBasicDetailsGetModel.Table.OnlineApplicationId);
                            if (detail != null)
                            {
                                detail.Fee_Amount = Convert.ToDecimal(ObjWBasicDetailsGetModel.Table.Fee_Amount);
                                detail.Fee_Status = ObjWBasicDetailsGetModel.Table.Fee_Status;
                                detail.Status_Code = Convert.ToInt32(ObjWBasicDetailsGetModel.Table.Status_Code);
                                detail.Payment_Through = ObjWBasicDetailsGetModel.Table.Payment_Through;
                                detail.Payment_Description = ObjWBasicDetailsGetModel.Table.Payment_Description;
                                dbContext.SaveChanges();

                                //insert detail in trans 
                                var transaction = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(f => f.ServiceRefId == ObjWBasicDetailsGetModel.Table.OnlineApplicationId && f.ServiceType != Constants.singleWindowPortalApplicationPayment);
                                if (transaction == null)
                                {
                                    OnlineApplicationDetails_trans trans = new OnlineApplicationDetails_trans
                                    {
                                        ServiceRefId = ObjWBasicDetailsGetModel.Table.OnlineApplicationId,
                                        txnid = ObjWBasicDetailsGetModel.Table.OnlineApplicationId.ToString() + "-1",
                                        Amount = Convert.ToDecimal(ObjWBasicDetailsGetModel.Table.Fee_Amount),
                                        productinfo = "SWP",
                                        mode = "SWP",
                                        ServiceType = Constants.singleWindowPortalApplicationPayment,
                                        status = 0,
                                        EntryDate = DateTime.Now.Date
                                    };
                                    dbContext.OnlineApplicationDetails_trans.Add(trans);
                                    dbContext.SaveChanges();
                                }

                                flag = ReturnType.Updated;
                            }
                        }
                        else { return flag; }
                    }
                }
            }
            return flag;
        }

        private NICsingalwindowSystem MapNICsingalwindowSystemTable(NewDataSet ObjWBasicDetailsGetModel)
        {
            NICsingalwindowSystem objNICsingalwindow = new NICsingalwindowSystem();
            objNICsingalwindow.onlineapplicationId = ObjWBasicDetailsGetModel.Table.OnlineApplicationId;
            objNICsingalwindow.schemeId = ObjWBasicDetailsGetModel.Table.SchemeId;
            objNICsingalwindow.Departmentid = ObjWBasicDetailsGetModel.Table.departmentId;
            objNICsingalwindow.Control_ID = ObjWBasicDetailsGetModel.Table.Control_ID;
            objNICsingalwindow.Unit_Id = ObjWBasicDetailsGetModel.Table.Unit_Id;
            objNICsingalwindow.ServiceID = ObjWBasicDetailsGetModel.Table.ServiceID;
            objNICsingalwindow.ProcessIndustryID = Convert.ToString(ObjWBasicDetailsGetModel.Table.OnlineApplicationId);
            //below lines were commented
            objNICsingalwindow.Company_Name = ObjWBasicDetailsGetModel.Table.Company_Name;
            objNICsingalwindow.Industry_District = ObjWBasicDetailsGetModel.Table.Industry_District;
            objNICsingalwindow.Industry_District_Id = ObjWBasicDetailsGetModel.Table.Industry_District_Id;
            objNICsingalwindow.Industry_Address = ObjWBasicDetailsGetModel.Table.Industry_Address;
            objNICsingalwindow.Pin_Code = ObjWBasicDetailsGetModel.Table.Pin_Code;
            objNICsingalwindow.Occupier_Name = ObjWBasicDetailsGetModel.Table.Occupier_Name;

            objNICsingalwindow.Occupier_Email_ID = ObjWBasicDetailsGetModel.Table.Occupier_Email_ID;
            objNICsingalwindow.Occupier_Mobile_No = ObjWBasicDetailsGetModel.Table.Occupier_Mobile_No;
            objNICsingalwindow.Occupier_DOB = ObjWBasicDetailsGetModel.Table.Occupier_DOB;
            objNICsingalwindow.Occupier_Gender = ObjWBasicDetailsGetModel.Table.Occupier_Gender;
            //below lines were commented
            objNICsingalwindow.Occupier_Address = ObjWBasicDetailsGetModel.Table.Occupier_Address;
            objNICsingalwindow.Occupier_District_ID = ObjWBasicDetailsGetModel.Table.Occupier_District_ID;
            objNICsingalwindow.Occupier_District_Name = ObjWBasicDetailsGetModel.Table.Occupier_District_Name;
            objNICsingalwindow.Occupier_Pin_Code = ObjWBasicDetailsGetModel.Table.Occupier_Pin_Code;
            objNICsingalwindow.Nature_of_Activity = ObjWBasicDetailsGetModel.Table.Nature_of_Activity;
            objNICsingalwindow.Installed_Capacity = ObjWBasicDetailsGetModel.Table.Installed_Capacity;
            objNICsingalwindow.Employees = ObjWBasicDetailsGetModel.Table.Employees;
            objNICsingalwindow.Nature_of_Operation = ObjWBasicDetailsGetModel.Table.Nature_of_Operation;
            objNICsingalwindow.publicdecimalProject_Cost = Convert.ToString(ObjWBasicDetailsGetModel.Table.Project_Cost);
            objNICsingalwindow.Organization_Type_ID = ObjWBasicDetailsGetModel.Table.Organization_Type_ID;
            objNICsingalwindow.Organization_Type = ObjWBasicDetailsGetModel.Table.Organization_Type;
            objNICsingalwindow.Industry_Type_ID = ObjWBasicDetailsGetModel.Table.Industry_Type_ID;
            objNICsingalwindow.Industry_Type_Name = ObjWBasicDetailsGetModel.Table.Industry_Type_Name;
            objNICsingalwindow.Expected_date_construction = ObjWBasicDetailsGetModel.Table.Expected_date_construction;
            objNICsingalwindow.Project_Status = ObjWBasicDetailsGetModel.Table.Project_Status;
            objNICsingalwindow.Industry_Color = ObjWBasicDetailsGetModel.Table.Industry_Color;
            objNICsingalwindow.Expected_date_production = ObjWBasicDetailsGetModel.Table.Expected_date_production;
            objNICsingalwindow.Unit_Category = ObjWBasicDetailsGetModel.Table.Unit_Category;
            objNICsingalwindow.Items_Manufactured = ObjWBasicDetailsGetModel.Table.Items_Manufactured;

            objNICsingalwindow.Annual_Turnover = Convert.ToString(ObjWBasicDetailsGetModel.Table.Annual_Turnover);
            return objNICsingalwindow;
        }

        private int UpdateOpenSchemeForm(OnlineFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                //OnlineApplicationDetail application = new OnlineApplicationDetail();
                var application = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == model.Id);
                if (application != null)
                {
                    if (model.ApplicantType == "Individual")
                    {
                        application.firstName = model.FirstName;
                        application.middleName = model.MiddleName;
                        application.lastName = model.LastName;
                        application.fatherHusbandName = model.FatherName;
                        application.motherName = model.MotherName;
                        application.gender = model.Gender;
                        application.religionId = model.ReligionId;
                        application.occupationId = model.OccupationId;
                        //application.quotaId = model.CategoryId;
                        application.quotaId = Constants.General;
                        application.marritalStatus = model.MaritalStatus;
                        application.dateOfBirth = model.DOB;
                    }
                    else
                    {
                        application.CompanyName = model.Applicant;
                        application.signingAuthority = model.SigningAuthority;
                        application.registeredOffice = model.PermanentAddress;
                        application.CompanyType = model.CompanyTypeId;
                        application.gender = "Company";
                        application.fatherHusbandName = model.ApplicantMaster;
                        application.quotaId = model.SigningAuthorityId;
                    }
                    application.CompanyType = model.CompanyTypeId;
                    application.schemeId = model.SchemeId;
                    application.departmentId = model.DepartmentId;
                    application.permanentAdd = model.PermanentAddress;
                    application.correspondanceAdd = model.CorrespondingAddress;
                    application.area = model.AreaRangeId;
                    application.mobileNumberP2 = model.MobileNumber;
                    application.phoneNumberP2 = model.PhoneNumber;
                    application.faxNumberP2 = model.FaxNumber;
                    application.email = model.Email;
                    application.annualIncome = model.AnnualIncome;
                    application.ApplicationFee = model.ApplicationFee;
                    application.FormCGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                    application.FormSGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                    application.ProcessingCharge = model.ProcessingCharge;
                    application.ProcessingCGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                    application.ProcessingSGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                    application.EarnestMoney = model.EarnestMoney;
                    application.TotalAmount = model.TotalAmount;
                    application.pan = model.PanNumber;
                    application.AadharNumber = model.AadharNumber;
                    //application.faxNumberP2 = model.ApplicantGSTNumber;
                    application.GSTNO = model.ApplicantGSTNumber;
                    application.ApplicationDate = DateTime.Now;

                    application.RefundBankId = model.RefundBankId;
                    application.RefundInfaverof = model.RefundInfaverof;
                    application.RefundaccountNo = model.RefundAccountNo;

                    application.PropertyTypeID = model.PropertyTypeId;
                    application.Sector = model.Sector;
                    application.BranchName = model.BranchName;
                    application.IFSCCode = model.IFSCCode;

                    application.FormCategory = model.ApplicationFormType;
                    if (model.ApplicationFormType == "Expansion")
                    {
                        application.FormSubCategory = model.ExpansionType;
                        application.RentingDate = model.LetterDate;
                        application.RentingLetterNo = model.LetterCode;
                        application.RentingPropertyNo = model.PropertyNo;
                    }

                    application.PropertyNo = model.PropertyNo;
                    application.ExistingPropertyNo = model.ExistingProperty;
                    application.AllotmentDate = model.AllottmentDate;
                    application.DispatchDate = model.DispatchDate;

                    application.createdBy = "Online";
                    application.createdDate = DateTime.Now;
                    application.isActive = true;
                    application.IsSubmited = false;

                    application.projectname = model.ProposedModel.ProposedProject;
                    application.projectcost = model.ProposedModel.TotalCost.ToString();
                    application.projecttimeempl = model.ProposedModel.ImplementationTime;
                    application.Online_offline = model.FormType == "Online" ? "Y" : "N";
                    if (OnlineSchemeType.Transport == model.SchemeType)
                    {
                        application.PropertyNo = model.PropertyNo;
                    }

                    //dbContext.OnlineApplicationDetails.Add(application);
                    dbContext.SaveChanges();

                    //int id = application.onlineapplicationId;
                    //model.Id = id;
                    //model.ApplicationFormId = id;
                    SaveDocumentsForApplicationForm(model, null, userImage, signatureImage);

                    UpdateDirectorDetailsForOpenSchemeForm(model.ApplicationFormId);

                    //string message = string.Format(NAMessages.OnlineApplicationSubmitted, id);
                    //ApplicationHelper.SendEmail(model.Email, "Registration Form", message);

                    //ApplicationHelper.SendSMS(model.MobileNumber, message);

                    flag = ReturnType.Updated;
                }
                else
                {
                    flag = ReturnType.NotExist;
                }
            }
            return flag;
        }

        private int UpdateDirectorDetailsForOpenSchemeForm(int? id)
        {
            if (id != null && id > 0)
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    List<onlineDirectorViewModel> templist = (List<onlineDirectorViewModel>)HttpContext.Current.Session["TempCompanyDirectors"];
                    if (templist != null && templist.Count > 0)
                    {
                        var directorlist = dbContext.online_Director_Master.Where(d => d.onlineapplicationId == id).ToList();
                        if (directorlist != null && directorlist.Count > 0)
                        {
                            dbContext.online_Director_Master.RemoveRange(directorlist);
                            dbContext.SaveChanges();
                        }

                        List<online_Director_Master> directorList = new List<online_Director_Master>();
                        foreach (var director in templist)
                        {
                            if (director.status == true)
                            {
                                online_Director_Master master = new online_Director_Master();
                                master.onlineapplicationId = id; // director.ApplicationFormId;
                                master.Director_Name = director.DirectorName;
                                master.Director_Share = director.DirectorShare;
                                master.Type = director.DirectorTypeId;
                                master.pan_no = director.PAN;
                                master.Is_Active = 1;
                                master.Created_By = director.ApplicationFormId;
                                directorList.Add(master);
                            }
                        }
                        dbContext.online_Director_Master.AddRange(directorList);
                        dbContext.SaveChanges();
                        return ReturnType.Success;
                    }
                    else
                    {
                        return ReturnType.NotExist;
                    }
                }
            }
            else
            {
                return ReturnType.NotExist;
            }
        }

        public bool ChangePassword(int FormId, string email, string newPassword)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.OnlineApplicationDetails.FirstOrDefault(cond => cond.onlineapplicationId == FormId && cond.email == email);
                if (user != null)
                {
                    user.Userpassword = newPassword.ToMD5HashForPasswordPIS();
                    user.modifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    if (user.email != null)
                    {
                        var body = "Dear User,<br><br>You have successfully changed your password. Your new password is " + newPassword + ". </br></br>Regards, </br>http://mynoida.in";
                        EmailHelper emailHelper = new EmailHelper();
                        emailHelper.Send(user.email, "You have successfully changed your password.", body);
                    }

                    if (user.mobileNumberP2 != null)
                    {
                        var msg = string.Format(NAMessages.PasswordChange, newPassword);
                        ApplicationHelper.SendSMS(user.mobileNumberP2, msg);
                    }
                    flag = true;
                }
            }
            return flag;
        }

        public List<DropdownViewModel> GetAreaRangeByDepartment(int? schemeId, int? departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var schemetype = dbContext.SchemeMsts.Where(s => s.schemeId == schemeId).Select(x => x.SchemeTypeMst.SchemeTypeDesc).FirstOrDefault();
                var lst = (from scheme in dbContext.SchemeCostTrans
                           where scheme.schemeId == schemeId && scheme.departmentId == departmentId && scheme.IsActive == true
                           && scheme.FloorMst.modifiedBy == schemetype
                           select new DropdownViewModel
                           {
                               Id = scheme.FloorMst.floorId,
                               Text = scheme.FloorMst.floorName
                           }).ToList();
                return lst;
            }
        }

        public int SaveUploadPreviousChallan(OnlineFormViewModel model, HttpPostedFileBase docs)
        {
            int flag = 0;
            if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.ApplicationFormId)))
            {
                if (docs != null && docs.ContentLength > 0)
                {
                    string challan = "PreviousChallan";
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.ApplicationFormId + "/" + challan)))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.ApplicationFormId + "/" + challan));
                    }
                    string extension = Path.GetExtension(docs.FileName);
                    var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.ApplicationFormId + "/" + challan + "/" + model.ApplicationFormId + "_PreviousChallan" + extension);
                    docs.SaveAs(fileSavePath);
                    flag = SaveOffinePreviousChallanPaymentTransaction(model);
                }
            }
            return flag;
        }

        private int SaveOffinePreviousChallanPaymentTransaction(OnlineFormViewModel Objmodel)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var onlineApplication = dbContext.OnlineApplicationDetails.Where(m => m.onlineapplicationId == Objmodel.ApplicationFormId && m.isActive == true).FirstOrDefault();
                if (onlineApplication != null)
                {
                    var transaction = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(f => f.ServiceRefId == Objmodel.ApplicationFormId && f.ServiceType != 4);
                    if (transaction == null)
                    {
                        OnlineApplicationDetails_trans trans = new OnlineApplicationDetails_trans
                        {
                            ServiceRefId = Objmodel.ApplicationFormId,
                            txnid = Objmodel.ApplicationFormId.ToString() + "-1",
                            TrKey = Objmodel.PaymentModel.TransactionId,
                            Amount = Objmodel.PaymentModel.Amount,
                            productinfo = "PreviousChallan",
                            mode = "PChallan",
                            ServiceType = Constants.offlinePrevoiusChallanApplicationPayment,
                            issuing_bank = Objmodel.PaymentModel.BankName,
                            status = 0,
                            EntryDate = DateTime.Now.Date
                        };
                        dbContext.OnlineApplicationDetails_trans.Add(trans);
                        dbContext.SaveChanges();
                        return flag = ReturnType.Saved;
                    }
                }
            }
            return flag;
        }

        public int UpdatePaymentTransaction_SingleWindowPortal(OnlineFormViewModel Objmodel)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var onlineApplication = dbContext.OnlineApplicationDetails.Where(m => m.onlineapplicationId == Objmodel.ApplicationFormId && m.isActive == true).FirstOrDefault();
                if (onlineApplication != null)
                {
                    var transaction = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(f => f.ServiceRefId == Objmodel.ApplicationFormId && f.ServiceType != Constants.singleWindowPortalApplicationPayment);
                    if (transaction != null)
                    {
                        if (transaction.status == 0 && transaction.TranStatus == 0)
                        {
                            transaction.TrKey = Objmodel.PaymentModel.TransactionId;
                            transaction.issuing_bank = "Single Window Portal";
                            transaction.status = 1;
                            transaction.TranStatus = 1;
                            transaction.modifiydate = DateTime.Now.Date;
                            dbContext.SaveChanges();
                            return flag = ReturnType.Updated;
                        }
                    }
                }
            }
            return flag;
        }
        #endregion

        public List<DropdownViewModel> GetSchemeList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from scheme in dbContext.SchemeMsts
                           where scheme.Status == Constants.SchemeOpen && scheme.IsActive == true
                           && scheme.SchemeTypeMst.modifiedBy == Constants.SchemeOnline && scheme.schemeId != 184184
                           select new DropdownViewModel
                           {
                               Id = scheme.schemeId,
                               Text = scheme.schemeName
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetPaymentStatusList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from config in dbContext.Common_Config
                           where config.Is_Active == 1 && config.Category == Constants.PaymentStatus
                           select new DropdownViewModel
                           {
                               Id = config.Id,
                               Text = config.Name
                           }).ToList();
                return lst;
            }
        }

        private OnlineFormViewModel GetOnlineApplicationDetailByApplicationId(int applicationId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from onlineapp in dbContext.OnlineApplicationDetails
                            join trans in dbContext.OnlineApplicationDetails_trans on onlineapp.onlineapplicationId equals trans.ServiceRefId
                            where trans.status == 1 && trans.TranStatus == 1 && onlineapp.onlineapplicationId == applicationId
                            select new OnlineFormViewModel
                            {
                                FormNo = applicationId.ToString(),
                                ApplicationFormId = onlineapp.onlineapplicationId,
                                FirstName = onlineapp.firstName,
                                MiddleName = onlineapp.middleName,
                                LastName = onlineapp.lastName,
                                FatherName = onlineapp.fatherHusbandName,
                                MotherName = onlineapp.motherName,
                                ReligionId = onlineapp.religionId,
                                OccupationId = onlineapp.occupationId,
                                MaritalStatus = onlineapp.marritalStatus,
                                DOB = onlineapp.dateOfBirth,
                                Applicant = onlineapp.CompanyName,
                                SigningAuthority = onlineapp.signingAuthority,
                                PermanentAddress = onlineapp.permanentAdd,
                                ApplicantMaster = onlineapp.fatherHusbandName,
                                AnnualIncome = onlineapp.annualIncome,
                                PanNumber = onlineapp.pan,
                                MobileNumber = onlineapp.mobileNumberP2,
                                CorrespondingAddress = onlineapp.correspondanceAdd,
                                Email = onlineapp.email,
                                Gender = onlineapp.gender,
                                ApplicantType = onlineapp.gender,
                                SchemeId = onlineapp.schemeId,
                                DepartmentId = onlineapp.departmentId,
                                PaymentModel = new OnlinePaymentViewModel
                                {
                                    ApplicationFormId = onlineapp.onlineapplicationId,
                                    TransactionKey = trans.TrKey,
                                    TransactionId = trans.txnid,
                                    Amount = trans.Amount,
                                    GatewayName = trans.GetwayName,
                                    ProductInfo = trans.productinfo,
                                    Udf5 = trans.udf5,
                                    Mihpayid = trans.mihpayid,
                                    Mode = trans.mode,
                                    TransactionStatus = trans.TranStatus.ToString(),
                                    modifiydate = trans.modifiydate,
                                    EntryDate = trans.EntryDate,
                                    PaymentSource = trans.payment_source,
                                    PG_Type = trans.PG_Type,
                                    BankReferenceNo = trans.bank_ref_num,
                                    BankCode = trans.bankcode,
                                    VirtualAccountNo = trans.virtualaccountno
                                }
                            }).FirstOrDefault();
                return data;
            }
        }

        //transfer online application form data to application form
        public int SaveApplicationDetailForAllotment(int FormId)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (FormId > 0)
                {
                    var data = GetOnlineApplicationDetailByApplicationId(FormId);
                    if (data != null)
                    {
                        string FormNo = Convert.ToString(data.ApplicationFormId);
                        var ExApplication = dbContext.ApplicationDetails.FirstOrDefault(m => m.formNo == FormNo);
                        if (ExApplication == null)
                        {
                            ApplicationDetail applicatinForm = new ApplicationDetail();
                            if (data.ApplicantType.ToLower() == Constants.Company.ToLower())
                            {
                                applicatinForm = new ApplicationDetail
                                {
                                    firstName = data.Applicant,
                                    signingAuthority = data.SigningAuthority,
                                    registeredOffice = data.PermanentAddress,
                                    gender = data.Gender,
                                    fatherHusbandName = data.ApplicantMaster,
                                    quotaId = data.SigningAuthorityId,
                                    tFirstName = data.Applicant,
                                    T_Company_Name = data.Applicant,
                                    tAnnualIncome = data.AnnualIncome,
                                };
                            }
                            else
                            {
                                applicatinForm = new ApplicationDetail
                                {
                                    firstName = data.FirstName,
                                    middleName = data.MiddleName,
                                    lastName = data.LastName,
                                    fatherHusbandName = data.FatherName,
                                    motherName = data.MotherName,
                                    gender = data.Gender,
                                    religionId = data.ReligionId,
                                    occupationId = data.OccupationId,
                                    quotaId = Constants.General,
                                    marritalStatus = data.MaritalStatus,
                                    dateOfBirth = data.DOB,
                                    tFirstName = data.FirstName,
                                    tMiddleName = data.MiddleName,
                                    tLastName = data.LastName
                                };
                            }

                            applicatinForm.pan = data.PanNumber;
                            applicatinForm.dateOfBirth = data.DOB;
                            applicatinForm.mobileNumberP2 = data.MobileNumber;
                            applicatinForm.dateOfBirth = data.DOB;
                            applicatinForm.permanentAdd = data.PermanentAddress;
                            applicatinForm.correspondanceAdd = data.CorrespondingAddress;
                            applicatinForm.email = data.Email;

                            applicatinForm.tFatherHusbandName = data.FatherName;
                            applicatinForm.tMotherName = data.MotherName;
                            applicatinForm.tPan = data.PanNumber;
                            applicatinForm.tEmail = data.Email;
                            applicatinForm.tMobileNumber = data.MobileNumber;
                            applicatinForm.tDateOfBirth = data.DOB;

                            applicatinForm.gender = data.Gender;
                            applicatinForm.tCorrespondanceAdd = data.CorrespondingAddress;
                            applicatinForm.tPermanentAdd = data.PermanentAddress;

                            applicatinForm.schemeId = data.SchemeId;
                            applicatinForm.departmentId = data.DepartmentId;
                            applicatinForm.formNo = data.FormNo;

                            applicatinForm.createdBy = userInfo.UserID.ToString();
                            applicatinForm.createdDate = DateTime.Now;

                            dbContext.ApplicationDetails.Add(applicatinForm);
                            dbContext.SaveChanges();

                            data.Id = applicatinForm.applicationId;
                            int pflag = TransferOnlinePaymentDetail(data);
                            if (pflag == ReturnType.Paid) flag = ReturnType.Updated;
                            else flag = ReturnType.NotPaid;
                            //flag = ReturnType.Updated;
                        }
                        else
                        {
                            flag = ReturnType.Exist;
                        }
                    }
                }
            }
            return flag;
        }

        private int TransferOnlinePaymentDetail(OnlineFormViewModel model)
        {
            var flag = ReturnType.NotPaid;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingRecord = dbContext.ApplicationPaymentDetails.Where(x => x.applicationId == model.Id).FirstOrDefault();
                if (existingRecord == null)
                {
                    var payment = new ApplicationPaymentDetail();
                    payment.applicationId = model.Id;
                    payment.paymentMode = model.PaymentModel.Mode;
                    payment.formNo = model.Id.ToString();
                    payment.amountDeposited = model.PaymentModel.Amount;
                    payment.utn = model.PaymentModel.TransactionKey;
                    payment.paymentDate = model.PaymentModel.modifiydate == null ? model.PaymentModel.EntryDate : model.PaymentModel.modifiydate;
                    payment.challanIssueDate = model.PaymentModel.modifiydate != null ? model.PaymentModel.EntryDate : null;
                    //payment.challanNo = model.PaymentModel.modifiydate != null ? model.PaymentModel.TransactionId : null;
                    payment.createdBy = userInfo.UserID.ToString();
                    payment.createdDate = DateTime.Now;
                    dbContext.ApplicationPaymentDetails.Add(payment);
                    dbContext.SaveChanges();
                    flag = ReturnType.Paid;
                }
            }
            return flag;
        }

        public LetterViewModel GetLetterByBarcode(string barcode)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from lettr in dbContext.Letter_History
                              join allot in dbContext.AllotmentMasters on lettr.Rid equals allot.rid
                              join propt in dbContext.SchemePropTrans on allot.propertyId equals propt.propertyId
                              join tmplt in dbContext.TemplateMasters on (lettr.Department_Id + lettr.Template_Id) equals (tmplt.departmentId + tmplt.templateId)
                              where lettr.Barcode_Val == barcode  //orderby srvc.requestNo descending
                              select new LetterViewModel
                              {
                                  Id = lettr.Id,
                                  Rid = lettr.Rid,
                                  Applicant = allot.ApplicationDetail.tFirstName,
                                  CorrespondAddress = allot.ApplicationDetail.tCorrespondanceAdd,
                                  Sector = propt.SectorMst.sectorName,
                                  Block = propt.BlockMst.blockName,
                                  PlotNo = propt.propertyNo,
                                  DepartmentId = lettr.Department_Id,
                                  Department = allot.DepartmentMst.departmentName,
                                  LetterId = lettr.Template_Id,
                                  LetterType = tmplt.templateName,
                                  CreatedDate = lettr.Created_Date,
                                  LetterDate = lettr.Generate_Date,
                                  LetterContent = lettr.Template_Html
                              }).FirstOrDefault();
                return result;
            }
        }

        public DataSourceResult GetApplicantListAfterDraw(DataSourceRequest request, OnlineFormViewModel modal)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var DepartmentList = (from deptTrans in dbContext.UmUserDepartmentTrans where deptTrans.UserRefId == userInfo.UserID && deptTrans.Status == true select deptTrans.DepartmentId).ToList();

                var applicants = (from draw in dbContext.OnlineSchemeDraws
                                  join oam in dbContext.OnlineApplicationDetails on draw.OnlineApplicationId equals oam.onlineapplicationId
                                  from sch in dbContext.SchemeMsts.Where(s => s.schemeId == oam.schemeId).DefaultIfEmpty()
                                  from deptt in dbContext.DepartmentMsts.Where(d => d.departmentId == oam.departmentId).DefaultIfEmpty()
                                  where draw.IsActive == true //&& DepartmentList.Contains(oam.departmentId) 
                                        && (modal.DepartmentId == null || oam.departmentId == modal.DepartmentId)
                                        && (modal.SchemeId == null || oam.schemeId == modal.SchemeId)
                                  select new OnlineFormViewModel
                                  {
                                      Id = oam.onlineapplicationId,
                                      ApplicationFormId = oam.onlineapplicationId,
                                      SchemeName = sch.schemeName,
                                      Department = deptt.departmentName,
                                      FirstName = string.IsNullOrEmpty(oam.CompanyName) ? oam.firstName + " " + (!string.IsNullOrEmpty(oam.middleName) ? oam.middleName + " " + oam.lastName : oam.lastName) : oam.CompanyName,
                                      Gender = oam.gender,
                                      DOB = oam.dateOfBirth,
                                      FormStatus = oam.IsSubmited == null ? Status.Rejected : (oam.IsSubmited == true ? Status.Accepted : Status.InProgress),
                                      IsDeleted = oam.isActive == null ? "" : (oam.isActive == true ? Status.Accepted : Status.Rejected),
                                      TotalAmount = oam.TotalAmount,
                                      SubmitDate = oam.createdDate,
                                      MobileNumber = oam.mobileNumberP2,
                                      PanNumber = oam.pan,
                                      PropertyNo = draw.Sector + "/" + (!string.IsNullOrEmpty(draw.Block) ? draw.Block + "-" + draw.PlotNo : draw.PlotNo),
                                      AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == oam.area && m.IsActive == true).FirstOrDefault().floorName,
                                      ApplicantType = oam.gender == "Company" ? "Company" : "Individual",
                                      AmountPaidDate = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().EntryDate,
                                      ChallanBank = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.SchemeBankTrans.FirstOrDefault(m => m.schemeId == oam.schemeId && m.IsActive == true && m.bankId.ToString() == (dbContext.OnlineApplicationDetails_trans.FirstOrDefault(i => i.ServiceRefId == oam.onlineapplicationId && i.ServiceType == 3).GetwayName)).BankMst.bankName) : "--",
                                      PayType = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? "Online" : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? "Offline (RTGS/NEFT)" : "--"),
                                      //ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "Paid" : "Not Paid") : "--"),
                                      ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "Paid" : "Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 4 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "PC Paid" : "PC Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 5 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "SWP Paid" : "SWP Not Paid") : "--"))),
                                      ApplicationFormType = !string.IsNullOrEmpty(oam.FormCategory) ? (oam.FormCategory) : "--",
                                      ExpansionType = !string.IsNullOrEmpty(oam.FormCategory) ? ((oam.FormCategory == Constants.ApplicationFormTypeEx) ? oam.FormSubCategory : "--") : "--"
                                  });
                return applicants.ToDataSourceResult(request);
            }
            throw new NotImplementedException();
        }

        public DataSourceResult GetApplicantDetailForDraw(DataSourceRequest request, OnlineFormViewModel modal)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var DepartmentList = (from deptTrans in dbContext.UmUserDepartmentTrans where deptTrans.UserRefId == userInfo.UserID && deptTrans.Status == true select deptTrans.DepartmentId).ToList();

                var applicants = (from oam in dbContext.OnlineApplicationDetails
                                  from sch in dbContext.SchemeMsts.Where(s => s.schemeId == oam.schemeId).DefaultIfEmpty()
                                  from deptt in dbContext.DepartmentMsts.Where(d => d.departmentId == oam.departmentId).DefaultIfEmpty()
                                  where oam.departmentId == modal.DepartmentId && oam.schemeId == modal.SchemeId && oam.onlineapplicationId == modal.ApplicationFormId
                                  select new OnlineFormViewModel
                                  {
                                      Id = oam.onlineapplicationId,
                                      ApplicationFormId = oam.onlineapplicationId,
                                      SchemeName = sch.schemeName,
                                      Department = deptt.departmentName,
                                      FirstName = string.IsNullOrEmpty(oam.CompanyName) ? oam.firstName + " " + (!string.IsNullOrEmpty(oam.middleName) ? oam.middleName + " " + oam.lastName : oam.lastName) : oam.CompanyName,
                                      Gender = oam.gender,
                                      DOB = oam.dateOfBirth,
                                      FormStatus = oam.IsSubmited == null ? Status.Rejected : (oam.IsSubmited == true ? Status.Accepted : Status.InProgress),
                                      IsDeleted = oam.isActive == null ? "" : (oam.isActive == true ? Status.Accepted : Status.Rejected),
                                      TotalAmount = oam.TotalAmount,
                                      SubmitDate = oam.createdDate,
                                      CorrespondingAddress = oam.correspondanceAdd,
                                      MobileNumber = oam.mobileNumberP2,
                                      PanNumber = oam.pan,
                                      AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == oam.area && m.IsActive == true).FirstOrDefault().floorName,
                                      ApplicantType = oam.gender == "Company" ? "Company" : "Individual",
                                      AmountPaidDate = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().EntryDate,
                                      ChallanBank = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.SchemeBankTrans.FirstOrDefault(m => m.schemeId == oam.schemeId && m.IsActive == true && m.bankId.ToString() == (dbContext.OnlineApplicationDetails_trans.FirstOrDefault(i => i.ServiceRefId == oam.onlineapplicationId && i.ServiceType == 3).GetwayName)).BankMst.bankName) : "--",
                                      PayType = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? "Online" : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? "Offline (RTGS/NEFT)" : "--"),
                                      //ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "Paid" : "Not Paid") : "--"),
                                      ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "Paid" : "Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 4 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "PC Paid" : "PC Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 5 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "SWP Paid" : "SWP Not Paid") : "--"))),
                                      ApplicationFormType = !string.IsNullOrEmpty(oam.FormCategory) ? (oam.FormCategory) : "--",
                                      ExpansionType = !string.IsNullOrEmpty(oam.FormCategory) ? ((oam.FormCategory == Constants.ApplicationFormTypeEx) ? oam.FormSubCategory : "--") : "--"
                                  }).ToList();
                return applicants.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetOnlineApplicationFormIdList(DataSourceRequest request, int? schemeId, int? departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var DepartmentList = (from deptTrans in dbContext.UmUserDepartmentTrans where deptTrans.UserRefId == userInfo.UserID && deptTrans.Status == true select deptTrans.DepartmentId).ToList();
                var existingId = dbContext.OnlineSchemeDraws.Where(s => s.SchemeId == schemeId && s.DepartmentId == departmentId && s.IsActive == true).Select(s => s.OnlineApplicationId).ToList();
                var formIdList = (from application in dbContext.OnlineApplicationDetails
                                  join transction in dbContext.OnlineApplicationDetails_trans on application.onlineapplicationId equals transction.ServiceRefId
                                  where application.isActive == true && !existingId.Contains(application.onlineapplicationId)
                                  && application.schemeId == schemeId && (departmentId == null || application.departmentId == departmentId)
                                  && transction.status == 1 && transction.TranStatus == 1
                                  select new DropdownViewModel
                                  {
                                      Id = application.onlineapplicationId,
                                      Text = application.onlineapplicationId.ToString()
                                  });
                return formIdList.ToDataSourceResult(request);
            }
        }

        public int AllotPropertyAfterDraw(OnlineFormViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = dbContext.OnlineSchemeDraws.FirstOrDefault(x => x.OnlineApplicationId == model.ApplicationFormId);
                if (detail == null)
                {
                    var draw = new OnlineSchemeDraw();
                    draw.OnlineApplicationId = model.ApplicationFormId;
                    draw.SchemeId = model.SchemeId;
                    draw.DepartmentId = model.DepartmentId;
                    draw.Sector = model.Sector;
                    draw.Block = model.Block;
                    draw.PlotNo = model.PlotNo;
                    draw.Area = model.Area;
                    draw.DrawDate = model.SchemeDrawDate;
                    draw.IsActive = true;
                    draw.Status = "Allotted";
                    draw.CreatedDate = DateTime.Now;
                    draw.CreatedBy = userInfo.UserID.ToString();
                    dbContext.OnlineSchemeDraws.Add(draw);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                else
                {
                    //detail.OnlineApplicationId = model.ApplicationFormId;
                    detail.SchemeId = model.SchemeId;
                    detail.DepartmentId = model.DepartmentId;
                    detail.Sector = model.Sector;
                    detail.Block = model.Block;
                    detail.PlotNo = model.PlotNo;
                    detail.Area = model.Area;
                    detail.DrawDate = model.SchemeDrawDate;
                    detail.ModifiedDate = DateTime.Now;
                    detail.ModifiedBy = userInfo.UserID.ToString();
                    detail.IsActive = true;
                    detail.Status = "Allotted";
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }

        public int UpdateAllottedPropertyAfterDraw(int? formId, string actionType)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = dbContext.OnlineSchemeDraws.FirstOrDefault(x => x.OnlineApplicationId == formId);
                if (detail != null)
                {
                    detail.ModifiedDate = DateTime.Now;
                    detail.ModifiedBy = userInfo.UserID.ToString();
                    detail.IsActive = false;
                    detail.Status = "Canceled";
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                else
                {
                    flag = ReturnType.NotExist;
                }
            }
            return flag;
        }

        public int ValidatePropertyForAllotment(string sector, string block, string plot)
        {
            var flag = ReturnType.NotExist;
            using (var dbContext = new NoidaPMSEntities())
            {
                OnlineSchemeDraw detail = null;
                if (block != "")
                {
                    detail = dbContext.OnlineSchemeDraws.FirstOrDefault(x => x.Sector == sector && x.Block == block && x.PlotNo == plot);
                }
                else
                {
                    detail = dbContext.OnlineSchemeDraws.FirstOrDefault(x => x.Sector == sector && (x.Block == "" || x.Block == null) && x.PlotNo == plot);
                }

                if (detail != null)
                {
                    flag = ReturnType.Exist;
                }
            }
            return flag;
        }

        public ResultMessage SaveApplicationProcessRequest(OnlineFormViewModel objOnlineFormViewModel)
        {
            ResultMessage objresultmessage = new ResultMessage();
            ResultMessage resultmessagewithreturnType = new ResultMessage();
            var flag = ReturnType.None;
            string message = "<div class='row'><div class='col-md-12'>";
            using (var dbContext = new NoidaPMSEntities())
            {
                for (int i = 0; i < objOnlineFormViewModel.ApplicationIdList.Length; i++)
                {
                    int key = 0;
                    if (objOnlineFormViewModel.ApplicationIdList[i] > 0)
                    {
                        OnlineApplicationProcessDetail Process = new OnlineApplicationProcessDetail();
                        int Id = objOnlineFormViewModel.ApplicationIdList[i];
                        int EnumStatus = (int)Enum.Parse(typeof(OnlineApplicationProcess), objOnlineFormViewModel.ProcessType);

                        int ApproverUserId = 0;
                        if (!string.IsNullOrEmpty(objOnlineFormViewModel.User)) { ApproverUserId = Convert.ToInt32(objOnlineFormViewModel.User); }

                        Process = dbContext.OnlineApplicationProcessDetails.FirstOrDefault(m => m.OnlineApplicationId == Id);
                        if (Process == null)
                        {
                            int onAppId = (from onlineapp in dbContext.OnlineApplicationDetails
                                           join trans in dbContext.OnlineApplicationDetails_trans on onlineapp.onlineapplicationId equals trans.ServiceRefId
                                           where trans.status == 1 && trans.TranStatus == 1 && onlineapp.onlineapplicationId == Id
                                           select onlineapp.onlineapplicationId).FirstOrDefault();
                            if (onAppId > 0)
                            {
                                OnlineApplicationProcessDetail onlineapplicationprocessdetail = new OnlineApplicationProcessDetail();
                                onlineapplicationprocessdetail.OnlineApplicationId = onAppId;
                                onlineapplicationprocessdetail.ApplicationStatus = EnumStatus;
                                onlineapplicationprocessdetail.ProcessStatus = Constants.InProgress;
                                onlineapplicationprocessdetail.SubmittedBy = userInfo.UserID;
                                onlineapplicationprocessdetail.SubmitDate = DateTime.Now;
                                onlineapplicationprocessdetail.Approver = ApproverUserId;
                                onlineapplicationprocessdetail.CreatedBy = userInfo.UserID;
                                onlineapplicationprocessdetail.CreatedDate = DateTime.Now;
                                dbContext.OnlineApplicationProcessDetails.Add(onlineapplicationprocessdetail);
                                dbContext.SaveChanges();
                                flag = 201;
                                key = onAppId;
                                message = message + "<label>" + onAppId + " saved.</label>";
                            }
                            else { flag = ReturnType.NotExist; message = message + "<label>" + onAppId + " not saved.application id not exist.</label>"; }
                        }
                        else
                        {
                            if (Process.ProcessStatus == Constants.Approved)
                            {
                                if (EnumStatus > Process.ApplicationStatus)
                                {
                                    Process.ApplicationStatus = EnumStatus;
                                    Process.ProcessStatus = Constants.InProgress;
                                    Process.Approver = ApproverUserId;
                                    Process.SubmittedBy = userInfo.UserID;
                                    Process.SubmitDate = DateTime.Now;
                                    Process.ModifiedBy = userInfo.UserID;
                                    Process.ModifiedDate = DateTime.Now;
                                    dbContext.SaveChanges();
                                    flag = 201;
                                    key = Id;
                                    message = message + "<label>" + Id + " updated.</label>";
                                }
                                else
                                {
                                    flag = ReturnType.NotExist;
                                    message = message + "<label>" + Id + " not updated.wrong process selected.</label>";
                                }
                            }
                            else
                            {
                                flag = ReturnType.NotExist;
                                message = message + "<label>" + Id + " not updated.previous process not approved.</label>";
                            }
                        }
                    }
                    resultmessagewithreturnType.ReturnType = flag;
                    resultmessagewithreturnType.PrimaryKey = key;
                    objresultmessage.Message = objresultmessage.Message + message;
                    objresultmessage.clsResultType.Add(resultmessagewithreturnType);
                }
            }
            objresultmessage.ReturnType = 200;
            objresultmessage.Message = objresultmessage.Message + "</div></div>";
            return objresultmessage;
        }

        public DataSourceResult GetOnlineApplicationProcessRequests(DataSourceRequest request)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                int MoveToOSD = Convert.ToInt32(strOnlineApplicationProcess.MoveToOSD);
                int Scrutiny = Convert.ToInt32(strOnlineApplicationProcess.Scrutiny);
                int Draw = Convert.ToInt32(strOnlineApplicationProcess.Draw);
                int ApprovalCEO = Convert.ToInt32(strOnlineApplicationProcess.ApprovalCEO);
                var ApplicationRequest = (from appProcess in dbcontext.OnlineApplicationProcessDetails
                                          where appProcess.Approver == userInfo.UserID
                                          select new OnlineApplicationDetailProcess
                                        {
                                            OnlineApplicationDetailProcessId = appProcess.Id,
                                            OnlineApplicationId = appProcess.OnlineApplicationId,
                                            ApplicationStatus = appProcess.ApplicationStatus,
                                            strApplicationStatus = appProcess.ApplicationStatus > 0 ? (appProcess.ApplicationStatus == MoveToOSD ? "MoveToOSD" : (appProcess.ApplicationStatus == Scrutiny ? "Scrutiny" : (appProcess.ApplicationStatus == ApprovalCEO ? "ApprovalCEO" : (appProcess.ApplicationStatus == Draw ? "Draw" : string.Empty)))) : string.Empty,
                                            ProcessStatus = appProcess.ProcessStatus,
                                            strProcessStatus = appProcess.ProcessStatus > 0 ? (from status in dbcontext.StatusMasters where status.Id == appProcess.ProcessStatus select status.Status).FirstOrDefault() : string.Empty,
                                            IsActive = appProcess.IsActive,
                                            UserCreatedBy = dbcontext.UmUserMasters.Where(x => x.UserRefId == appProcess.CreatedBy).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault(),
                                            CreatedBy = appProcess.CreatedBy,
                                            CreatedDate = appProcess.CreatedDate,
                                            ApprovalDate = appProcess.ApprovalDate,
                                            AssignTo = dbcontext.UmUserMasters.Where(x => x.UserRefId == appProcess.Approver).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault(),
                                            Approver = appProcess.Approver,
                                            Comment = appProcess.Comment,
                                        });
                return ApplicationRequest.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetOnlineApplicationProcess(DataSourceRequest request)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                int MoveToOSD = Convert.ToInt32(strOnlineApplicationProcess.MoveToOSD);
                int Scrutiny = Convert.ToInt32(strOnlineApplicationProcess.Scrutiny);
                int Draw = Convert.ToInt32(strOnlineApplicationProcess.Draw);
                int ApprovalCEO = Convert.ToInt32(strOnlineApplicationProcess.ApprovalCEO);
                var ApplicationRequest = (from appProcess in dbcontext.OnlineApplicationProcessDetails
                                          select new OnlineApplicationDetailProcess
                                          {
                                              OnlineApplicationDetailProcessId = appProcess.Id,
                                              OnlineApplicationId = appProcess.OnlineApplicationId,
                                              ApplicationStatus = appProcess.ApplicationStatus,
                                              strApplicationStatus = appProcess.ApplicationStatus > 0 ? (appProcess.ApplicationStatus == MoveToOSD ? "MoveToOSD" : (appProcess.ApplicationStatus == Scrutiny ? "Scrutiny" : (appProcess.ApplicationStatus == ApprovalCEO ? "ApprovalCEO" : (appProcess.ApplicationStatus == Draw ? "Draw" : string.Empty)))) : string.Empty,
                                              ProcessStatus = appProcess.ProcessStatus,
                                              strProcessStatus = appProcess.ProcessStatus > 0 ? (from status in dbcontext.StatusMasters where status.Id == appProcess.ProcessStatus select status.Status).FirstOrDefault() : string.Empty,
                                              IsActive = appProcess.IsActive,
                                              UserCreatedBy = dbcontext.UmUserMasters.Where(x => x.UserRefId == appProcess.CreatedBy).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault(),
                                              CreatedBy = appProcess.CreatedBy,
                                              CreatedDate = appProcess.CreatedDate,
                                              ApprovalDate = appProcess.ApprovalDate,
                                              AssignTo = dbcontext.UmUserMasters.Where(x => x.UserRefId == appProcess.Approver).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault(),
                                              Approver = appProcess.Approver,
                                              Comment = appProcess.Comment,
                                          });
                return ApplicationRequest.ToDataSourceResult(request);
            }
        }

        public ResultMessage UpdateOnlineApplicationStatus(OnlineApplicationDetailProcess objOnlineApplicationDetailProcess)
        {
            var flag = ReturnType.None;
            ResultMessage resultmessage = new ResultMessage();
            ResultMessage listResult = new ResultMessage();
            using (var dbContext = new NoidaPMSEntities())
            {
                for (int i = 0; i < objOnlineApplicationDetailProcess.ApplicationProcessIdList.Length; i++)
                {
                    OnlineApplicationProcessDetail Process = new OnlineApplicationProcessDetail();
                    int Id = objOnlineApplicationDetailProcess.ApplicationProcessIdList[i];
                    Process = dbContext.OnlineApplicationProcessDetails.FirstOrDefault(m => m.Id == Id);
                    if (Process != null)
                    {
                        int MoveToOSD = Convert.ToInt32(strOnlineApplicationProcess.MoveToOSD);
                        int Scrutiny = Convert.ToInt32(strOnlineApplicationProcess.Scrutiny);
                        int Draw = Convert.ToInt32(strOnlineApplicationProcess.Draw);
                        int ApprovalCEO = Convert.ToInt32(strOnlineApplicationProcess.ApprovalCEO);
                        string Stage = Process.ApplicationStatus > 0 ? (Process.ApplicationStatus == MoveToOSD ? "MoveToOSD" : (Process.ApplicationStatus == Scrutiny ? "Scrutiny" : (Process.ApplicationStatus == ApprovalCEO ? "ApprovalCEO" : (Process.ApplicationStatus == Draw ? "Draw" : string.Empty)))) : string.Empty;
                        string Comment = "Comment: " + objOnlineApplicationDetailProcess.Comment + " by" + " " + userInfo.UserName + " (" + Stage + ") " + " on " + DateTime.Now;
                        Process.ProcessStatus = objOnlineApplicationDetailProcess.StatusType;
                        Process.Comment = Process.Comment + " \n " + Comment;
                        Process.SubmittedBy = userInfo.UserID;
                        Process.SubmitDate = DateTime.Now;
                        Process.ModifiedBy = userInfo.UserID;
                        Process.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Updated;
                        listResult.ReturnType = 201;
                        listResult.PrimaryKey = (int)Process.OnlineApplicationId;
                    }
                    resultmessage.ReturnType = 200;
                    resultmessage.clsResultType.Add(listResult);
                }
            }
            return resultmessage;
        }


        public DataSourceResult GetPropertyDetailAsDataSourceByPropertyId(DataSourceRequest request, int? propertyId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from property in dbContext.SchemePropTrans
                            where property.IsActive == true && property.propertyId == propertyId
                            select new PropertyViewModel
                            {
                                Id = property.refId,
                                PropertyId = property.propertyId,
                                SchemeId = property.schemeId,
                                SchemeName = property.SchemeMst.schemeName,
                                DepartmentId = property.departmentId,
                                Department = property.DepartmentMst.departmentName,
                                PropertyTypeId = property.propertyTypeId,
                                PropertyType = property.PropertyTypeMst.propertyTypeName,
                                //SectorId = property.sectorId,
                                //SectorName = property.SectorMst.sectorName,
                                //BlockId = property.blockId,
                                //BlockName = property.BlockMst.blockName,                               
                                FloorId = property.floorId,
                                FloorArea = property.FloorMst.floorName,
                                PropertyNo = property.SectorMst.sectorName + "/" + (property.blockId == null ? "" : property.BlockMst.blockName + "-") + property.propertyNo,
                                TotalArea = property.totalArea,
                                CoveredArea = property.coveredArea,
                                ActualArea = property.actualArea,
                                PropertyCost = property.propertyCost,
                                CivilCost = property.civilCost,
                                TotalPropertyCost = property.totalPropertyCost,
                                LandRate = property.landRatePerSqmt
                            });

                return data.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetApplicationDetailAsDataSourceByApplicationId(DataSourceRequest request, int? applicationId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from onlineapp in dbContext.OnlineApplicationDetails
                            join trans in dbContext.OnlineApplicationDetails_trans on onlineapp.onlineapplicationId equals trans.ServiceRefId
                            where trans.status == 1 && trans.TranStatus == 1 && onlineapp.onlineapplicationId == applicationId
                            select new OnlineFormViewModel
                            {
                                FormNo = applicationId.ToString(),
                                ApplicationFormId = onlineapp.onlineapplicationId,
                                FirstName = onlineapp.firstName,
                                MiddleName = onlineapp.middleName,
                                LastName = onlineapp.lastName,
                                Applicant = string.IsNullOrEmpty(onlineapp.CompanyName) ? onlineapp.firstName + " " + (!string.IsNullOrEmpty(onlineapp.middleName) ? onlineapp.middleName + " " + onlineapp.lastName : onlineapp.lastName) : onlineapp.CompanyName,
                                FatherName = onlineapp.fatherHusbandName,
                                MotherName = onlineapp.motherName,
                                ReligionId = onlineapp.religionId,
                                OccupationId = onlineapp.occupationId,
                                MaritalStatus = onlineapp.marritalStatus,
                                DOB = onlineapp.dateOfBirth,
                                AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == onlineapp.area && m.IsActive == true).FirstOrDefault().floorName,
                                SigningAuthority = onlineapp.signingAuthority,
                                PermanentAddress = onlineapp.permanentAdd,
                                ApplicantMaster = onlineapp.fatherHusbandName,
                                AnnualIncome = onlineapp.annualIncome,
                                PanNumber = onlineapp.pan,
                                MobileNumber = onlineapp.mobileNumberP2,
                                CorrespondingAddress = onlineapp.correspondanceAdd,
                                Email = onlineapp.email,
                                Gender = onlineapp.gender,
                                ApplicantType = onlineapp.gender,
                                SchemeId = onlineapp.schemeId,
                                DepartmentId = onlineapp.departmentId,
                                Department = dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == onlineapp.departmentId).departmentName,
                                TotalAmount = (decimal)onlineapp.TotalAmount,
                                ApplicationFormType = !string.IsNullOrEmpty(onlineapp.FormCategory) ? (onlineapp.FormCategory) : "--",
                                ExpansionType = !string.IsNullOrEmpty(onlineapp.FormCategory) ? ((onlineapp.FormCategory == Constants.ApplicationFormTypeEx) ? onlineapp.FormSubCategory : "--") : "--",
                                PaymentModel = new OnlinePaymentViewModel
                                {
                                    ApplicationFormId = onlineapp.onlineapplicationId,
                                    TransactionKey = trans.TrKey,
                                    TransactionId = trans.txnid,
                                    Amount = trans.Amount,
                                    GatewayName = trans.GetwayName,
                                    ProductInfo = trans.productinfo,
                                    Udf5 = trans.udf5,
                                    Mihpayid = trans.mihpayid,
                                    Mode = trans.mode,
                                    TransactionStatus = trans.TranStatus.ToString(),
                                    modifiydate = trans.modifiydate,
                                    EntryDate = trans.EntryDate,
                                    PaymentSource = trans.payment_source,
                                    PG_Type = trans.PG_Type,
                                    BankReferenceNo = trans.bank_ref_num,
                                    BankCode = trans.bankcode,
                                    VirtualAccountNo = trans.virtualaccountno
                                }
                            });
                return data.ToDataSourceResult(request);
            }
        }


        public int ValidatePropertyAndApplicationForm(OnlineFormViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == ActionType.Property)
                {
                    var property = dbContext.AllotmentMasters.FirstOrDefault(c => c.propertyId == model.PropertyId && c.isActive == NAStatusId.Active && c.isStatus == Status.Approved);
                    if (property != null) flag = ReturnType.Exist;
                    else flag = ReturnType.NotExist;
                }
                if (model.ActionType == ActionType.Application)
                {
                    var application = dbContext.ApplicationDetails.FirstOrDefault(a => a.formNo == model.ApplicationFormId.ToString() && a.isAllotted == NAStatusId.Allotted.ToString());
                    if (application != null) flag = ReturnType.Exist;
                    else flag = ReturnType.NotExist;
                }
            }
            return flag;
        }


        public DataSourceResult GetAllottedOnlineFormPropertyList(DataSourceRequest request, OnlineFormViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from alotment in dbContext.AllotmentMasters
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            join aplicant in dbContext.OnlineApplicationDetails on alotment.formNo equals aplicant.onlineapplicationId.ToString()
                            where (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                            && (model.SchemeId == null || alotment.schemeId == model.SchemeId)
                            select new OnlineFormViewModel
                            {
                                Id = aplicant.onlineapplicationId,
                                RegistrationId = alotment.rid,
                                ApplicationFormId = aplicant.onlineapplicationId,
                                SchemeName = alotment.SchemeMst.schemeName,
                                Department = alotment.DepartmentMst.departmentName,
                                FirstName = string.IsNullOrEmpty(aplicant.CompanyName) ? aplicant.firstName + " " + (!string.IsNullOrEmpty(aplicant.middleName) ? aplicant.middleName + " " + aplicant.lastName : aplicant.lastName) : aplicant.CompanyName,
                                Gender = aplicant.gender,
                                DOB = aplicant.dateOfBirth,
                                FormStatus = aplicant.IsSubmited == null ? Status.Rejected : (aplicant.IsSubmited == true ? Status.Accepted : Status.InProgress),
                                IsDeleted = aplicant.isActive == null ? "" : (aplicant.isActive == true ? Status.Accepted : Status.Rejected),
                                TotalAmount = aplicant.TotalAmount,
                                SubmitDate = aplicant.createdDate,
                                AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == aplicant.area && m.IsActive == true).FirstOrDefault().floorName,
                                ApplicantType = aplicant.gender == "Company" ? "Company" : "Individual",
                                //AmountPaidDate = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().EntryDate,
                                //ChallanBank = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.SchemeBankTrans.FirstOrDefault(m => m.schemeId == aplicant.schemeId && m.IsActive == true && m.bankId.ToString() == (dbContext.OnlineApplicationDetails_trans.FirstOrDefault(i => i.ServiceRefId == aplicant.onlineapplicationId && i.ServiceType == 3).GetwayName)).BankMst.bankName) : "--",
                                //PayType = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? "Online" : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? "Offline (RTGS/NEFT)" : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 4 ? "Previous Challan" : "--")),
                                //ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "Paid" : "Not Paid") : "--"),
                                //ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "Paid" : "Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 4 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "PC Paid" : "PC Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 5 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == aplicant.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "SWP Paid" : "SWP Not Paid") : "--"))),
                                ApplicationFormType = !string.IsNullOrEmpty(aplicant.FormCategory) ? (aplicant.FormCategory) : "--",
                                ExpansionType = !string.IsNullOrEmpty(aplicant.FormCategory) ? ((aplicant.FormCategory == Constants.ApplicationFormTypeEx) ? aplicant.FormSubCategory : "--") : "--",
                                //ProcessType = aplicant.OnlineApplicationProcessDetails.FirstOrDefault() != null ? (aplicant.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == MoveToOSD) != null ? "Move To OSD" : aplicant.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == Scrutiny) != null ? "Scrunity" : aplicant.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == ApprovalCEO) != null ? "Approval for CEO" : aplicant.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == Draw) != null ? "Draw" : string.Empty) : string.Empty
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetOnlineApplicationsForConsultant(DataSourceRequest request, OnlineFormViewModel modal)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var Online_trans = (from oam in dbContext.OnlineApplicationDetails
                                    from sch in dbContext.SchemeMsts.Where(s => s.schemeId == oam.schemeId).DefaultIfEmpty()
                                    from deptt in dbContext.DepartmentMsts.Where(d => d.departmentId == oam.departmentId).DefaultIfEmpty()
                                    where DepartmentList.Contains(oam.departmentId)
                                    && (modal.Id == null || oam.onlineapplicationId == modal.Id)
                                    && (modal.DepartmentId == null || oam.departmentId == modal.DepartmentId)
                                    && (modal.SchemeId == null || oam.schemeId == modal.SchemeId)
                                    select new OnlineFormViewModel
                                    {
                                        Id = oam.onlineapplicationId,
                                        ApplicationFormId = oam.onlineapplicationId,
                                        SchemeName = sch.schemeName,
                                        Department = deptt.departmentName,
                                        FirstName = string.IsNullOrEmpty(oam.CompanyName) ? oam.firstName + " " + (!string.IsNullOrEmpty(oam.middleName) ? oam.middleName + " " + oam.lastName : oam.lastName) : oam.CompanyName,
                                        Gender = oam.gender,
                                        DOB = oam.dateOfBirth,
                                        FormStatus = oam.IsSubmited == null ? Status.Rejected : (oam.IsSubmited == true ? Status.Accepted : Status.InProgress),
                                        IsDeleted = oam.isActive == null ? "" : (oam.isActive == true ? Status.Accepted : Status.Rejected),
                                        IsFormActive = oam.isActive,
                                        TotalAmount = oam.TotalAmount,
                                        SubmitDate = oam.createdDate,
                                        AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == oam.area && m.IsActive == true).FirstOrDefault().floorName,
                                        ApplicantType = oam.gender == Constants.Company ? Constants.Company : Constants.Individual,
                                        AmountPaidDate = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().EntryDate,
                                        ChallanBank = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.SchemeBankTrans.FirstOrDefault(m => m.schemeId == oam.schemeId && m.IsActive == true && m.bankId.ToString() == (dbContext.OnlineApplicationDetails_trans.FirstOrDefault(i => i.ServiceRefId == oam.onlineapplicationId && i.ServiceType == 3).GetwayName)).BankMst.bankName) : "--",
                                        PayType = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? "Online" : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? "Offline (RTGS/NEFT)" : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 4 ? "Previous Challan" : "--")),
                                        //ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "Paid" : "Not Paid") : "--"),
                                        ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "Paid" : "Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 4 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "PC Paid" : "PC Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 5 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "SWP Paid" : "SWP Not Paid") : "--"))),
                                        ApplicationFormType = !string.IsNullOrEmpty(oam.FormCategory) ? (oam.FormCategory) : "--",
                                        ExpansionType = !string.IsNullOrEmpty(oam.FormCategory) ? ((oam.FormCategory == Constants.ApplicationFormTypeEx) ? oam.FormSubCategory : "--") : "--",
                                        //ProcessType = oam.OnlineApplicationProcessDetails.FirstOrDefault() != null ? (oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == MoveToOSD) != null ? "Move To OSD" : oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == Scrutiny) != null ? "Scrunity" : oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == ApprovalCEO) != null ? "Approval for CEO" : oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == Draw) != null ? "Draw" : string.Empty) : string.Empty
                                    });

                if (modal.PayType == "2")
                {
                    Online_trans = Online_trans.Where(m => m.ChallanStatus != "Not Paid" && m.ChallanStatus != "--");
                }
                if (modal.PayType == PaymentStatus.Paid)
                {
                    Online_trans = Online_trans.Where(m => m.ChallanStatus != "Not Paid" && m.ChallanStatus != "--");
                }
                return Online_trans.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetFloorAreaListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var schemetype = dbContext.SchemeMsts.Where(s => s.schemeId == model.SchemeId).Select(x => x.SchemeTypeMst.SchemeTypeDesc).FirstOrDefault();
                var list = (from scheme in dbContext.SchemeCostTrans
                            where scheme.schemeId == model.SchemeId && scheme.departmentId == model.DepartmentId && scheme.IsActive == true
                            //&& scheme.FloorMst.modifiedBy == schemetype
                            select new DropdownViewModel
                            {
                                Id = scheme.FloorMst.floorId,
                                Text = scheme.FloorMst.floorName
                            }).Distinct();
                return list.ToDataSourceResult(request);
            }
        }


        public OnlineFormViewModel GetIndustrialSchemeInformation()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                string schemeId = ConfigurationManager.AppSettings["IndustrialSchemeId"];
                string departmentId = ConfigurationManager.AppSettings["IndustryDepartmentId"];
                OnlineFormViewModel model = new OnlineFormViewModel();
                var scheme = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId.ToString() == schemeId && s.IsActive == true);
                if (scheme != null)
                {
                    model.SchemeId = scheme.schemeId;
                    model.SchemeName = scheme.schemeName;
                    model.SchemeEndDate = scheme.endDate;
                    model.ApplicationFee = scheme.FormFee;
                    model.ProcessingCharge = scheme.ProcessingFee;
                    model.FormFeeGST = scheme.FormCGST + scheme.FormSGST;
                    model.ProcessingChargeGST = scheme.ProcessingCGST + scheme.ProcessingSGST;
                    model.SchemeType = dbContext.SchemeTypeMsts.Where(x => x.schemeTypeId == scheme.schemeTypeId).Select(x => x.SchemeType).FirstOrDefault();
                }
                var department = dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId.ToString() == departmentId);
                if (department != null)
                {
                    model.DepartmentId = department.departmentId;
                    model.Department = department.departmentName;
                }
                return model;
            }
        }


        public DataSourceResult GetDocumentListForOnlineSchemeForm(DataSourceRequest request, OnlineFormViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<OnlineDocumentViewModel> docs = new List<OnlineDocumentViewModel>();
                if (model.SchemeType == NASchemeType.IndustriaScheme)
                {
                    string schemeId = ConfigurationManager.AppSettings["IndustrialDocumentSchemeId"];
                    var form = dbContext.OnlineApplicationDetails.FirstOrDefault(f => f.onlineapplicationId == model.ApplicationFormId);
                    //var documents = dbContext.OnlineCheckListMasters.ToList();
                    docs = (from doc in dbContext.OnlineCheckListMasters
                            where doc.IsActive == true && doc.SchemeId.ToString() == schemeId //configSchemeId
                            select new OnlineDocumentViewModel
                            {
                                Id = doc.CheckListId,
                                DocumentId = doc.CheckListId,
                                DocumentName = doc.CheckListName,
                                ParentDocumentType = doc.MainList
                            }).ToList();
                    if (form != null)
                    {
                        if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.ApplicationFormId + "/Documents/")))
                        {
                            var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.onlineapplicationId + "/Documents/");
                            string[] files = Directory.GetFiles(filepath);
                            if (files != null)
                            {
                                for (int x = 0; x < files.Length; x++)
                                {
                                    var ufile = files[x].Split('\\');
                                    var filename = ufile[ufile.Length - 1];
                                    int start = (filename.LastIndexOf('-') + 1);
                                    int end = filename.LastIndexOf('.');
                                    int dif = end - start;
                                    int nod = Convert.ToInt32(filename.Substring(start, dif));
                                    docs.Where(m => m.Id == nod).FirstOrDefault().PathName = "/UploadFiles/" + model.ApplicationFormId + "/Documents/" + filename;
                                    docs.Where(m => m.Id == nod).FirstOrDefault().UploadedDocument = filename;
                                    //docs.ElementAt(nod - 1).PathName = "/UploadFiles/" + formId + "/Documents/" + filename;
                                    // docs.ElementAt(nod - 1).UploadedDocument = filename;
                                }
                            }
                        }
                        int counter = 1;
                        docs.ForEach(x => x.Sno = counter++);
                    }
                }
                return docs.ToDataSourceResult(request);
            }
        }


        public OnlineFormViewModel GetSchemeInformationForOnlineApplication(OnlineFormViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.DepartmentId == NADepartment.Industrial)
                {
                    string schemeId = ConfigurationManager.AppSettings["IndustrialSchemeId"];
                    string departmentId = ConfigurationManager.AppSettings["IndustryDepartmentId"];
                    //OnlineFormViewModel model = new OnlineFormViewModel();
                    var scheme = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId.ToString() == schemeId && s.IsActive == true);
                    if (scheme != null)
                    {
                        model.SchemeId = scheme.schemeId;
                        model.SchemeName = scheme.schemeName;
                        model.SchemeEndDate = scheme.endDate;
                        model.ApplicationFee = scheme.FormFee;
                        model.ProcessingCharge = scheme.ProcessingFee;
                        model.FormFeeGST = scheme.FormCGST + scheme.FormSGST;
                        model.ProcessingChargeGST = scheme.ProcessingCGST + scheme.ProcessingSGST;
                        model.SchemeType = dbContext.SchemeTypeMsts.Where(x => x.schemeTypeId == scheme.schemeTypeId).Select(x => x.SchemeType).FirstOrDefault();
                    }
                    var department = dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId.ToString() == departmentId);
                    if (department != null)
                    {
                        model.DepartmentId = department.departmentId;
                        model.Department = department.departmentName;
                    }
                }
                return model;
            }
        }


        public DataSourceResult GetOnlineChecklistDocument(DataSourceRequest request, OnlineDocumentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var docs = (from doc in dbContext.OnlineCheckListMasters
                            where (model.SchemeId == null || doc.SchemeId == model.SchemeId)
                            && (model.DepartmentId == null || doc.DepartmentId == model.DepartmentId)
                            && (model.SchemeTypeId == null || doc.SchemeTypeId == model.SchemeTypeId)
                            && (model.CheckListType == null || doc.CheckListType == model.CheckListType)
                            select new OnlineDocumentViewModel
                            {
                                Id = doc.CheckListId,
                                DocumentId = doc.CheckListId,
                                DocumentName = doc.CheckListName,
                                SchemeId = doc.SchemeId,
                                SchemeName = doc.SchemeId == null ? string.Empty : dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId == doc.SchemeId).schemeName,
                                SchemeTypeId = doc.SchemeTypeId,
                                SchemeType = doc.SchemeTypeId == null ? string.Empty : dbContext.SchemeTypeMsts.FirstOrDefault(s => s.schemeTypeId == doc.SchemeTypeId).SchemeType,
                                DepartmentId = doc.DepartmentId,
                                Department = doc.DepartmentId == null ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == doc.DepartmentId).departmentName,
                                CheckListType = doc.CheckListType,
                                IsActive = doc.IsActive,
                                Status = doc.IsActive == true ? "Active" : "InActive"
                            });
                return docs.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetChecklistTypeList(DataSourceRequest request, OnlineDocumentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var docs = (from doc in dbContext.OnlineCheckListMasters
                            where (model.SchemeId == null || doc.SchemeId == model.SchemeId)
                            && (model.DepartmentId == null || doc.DepartmentId == model.DepartmentId)
                            && (model.CheckListType == null || doc.CheckListType == model.CheckListType)
                            select new DropdownViewModel
                            {
                                Id = doc.CheckListId,
                                SchemeId = doc.SchemeId,
                                DepartmentId = doc.DepartmentId,
                                Text = doc.CheckListType,
                                IsActive = doc.IsActive
                            }).Distinct();
                return docs.ToDataSourceResult(request);
            }
        }


        public int ValidatePANForOnlineScheme(OnlineFormViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                if (model.ApplicationFormId == null)
                {
                    var form = dbContext.OnlineApplicationDetails.FirstOrDefault(o => o.isActive == true && o.pan.ToUpper() == model.PanNumber.ToUpper() && o.area == model.AreaRangeId.ToString() && o.schemeId == model.SchemeId);
                    flag = form == null ? ReturnType.NotExist : ReturnType.Exist;
                }
                else if (model.ApplicationFormId != null && model.ApplicationFormId > 0)
                {
                    var form = dbContext.OnlineApplicationDetails.FirstOrDefault(a => a.onlineapplicationId == model.ApplicationFormId);
                    if (form != null && form.pan != model.PanNumber)
                    {
                        flag = ReturnType.Mismatch;
                    }
                    else
                    {
                        flag = ReturnType.Success;
                    }
                }
                return flag;
            }
        }


        public OnlinePaymentViewModel GetGeneratedChallanDetailById(OnlinePaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                OnlinePaymentViewModel online = new OnlinePaymentViewModel();
                var challan = new Challan_Master();
                if (model.ActionType == "Challan")
                {
                    challan = dbContext.Challan_Master.FirstOrDefault(c => c.Id == model.ChallanId);
                }
                
                //var challan = dbContext.Challan_Master.Where(c => c.Rid == model.RegistrationId).OrderByDescending(o => o.Id).FirstOrDefault();
                if (challan != null)
                {
                    online.Id = challan.Id;
                    online.ChallanId = challan.Id;
                    online.ChallanRefId = challan.Challan_Id;
                    online.RegistrationId = challan.Rid;
                    online.DepartmentId = challan.Department_Id;
                    online.Department = challan.Department_Id != null ? dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == challan.Department_Id).departmentName : string.Empty;
                    online.SectorId = challan.Sector_Id;
                    online.Sector = challan.Sector_Id != null ? dbContext.SectorMsts.FirstOrDefault(c => c.sectorId == challan.Sector_Id).sectorName : string.Empty;
                    online.BlockId = challan.Block_Id;
                    online.Block = challan.Block_Id != null ? dbContext.BlockMsts.FirstOrDefault(c => c.blockId == challan.Block_Id).blockName : string.Empty;
                    online.PlotNo = challan.Plot_No;
                    online.TransactionId = challan.Id.ToString() + challan.Rid.ToString();
                    online.Applicant = challan.Allottee;
                    online.AddressI = challan.Address;
                    online.ChallanDate = challan.Created_Date;
                    online.MobileNo = challan.Mobile_No;
                    online.Email = challan.Email;
                    online.HtmlContent = challan.Content;
                    online.BankId = challan.Bank_Id;
                    online.BankName = challan.Bank_Id != null ? dbContext.BankMsts.FirstOrDefault(b => b.bankId == challan.Bank_Id).bankName : string.Empty;
                    online.BranchId = challan.Branch_Id;
                    online.AccountNo = challan.Account_Number;
                    online.ServiceName = "Challan Payment";
                    online.ProductInfo = "Challan Payment";
                    online.ActionType = model.ActionType;
                    online.OnlineRequestId = challan.ServiceRequestNo;

                    var paydetail = dbContext.Challan_Trans.Where(r => r.Challan_Master_Id == challan.Id).ToList();
                    online.Amount = (paydetail != null && paydetail.Count > 0) ? paydetail.Sum(p => p.Amount) : 0;                    
                }
                return online;
            }
        }


        public void SaveChallanOnlinePaymentTransaction(OnlinePaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //<add key="hashSequence" value="key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10"/>
                string transid = string.Empty;
                var exTransaction = dbContext.OnlineApplicationDetails_trans.Where(f => f.ServiceRefId == model.OnlineRequestId && f.ChallanId==model.ChallanId.ToString()).OrderByDescending(x => x.AutoID).FirstOrDefault();
                var transactionKey = ApplicationHelper.GenerateTransactionId();
                //model.TransactionId = transactionKey;
                model.TransactionKey = transactionKey;
                if (exTransaction == null)
                {
                    //transid = model.OnlineRequestId.ToString() + model.RegistrationId.ToString() + "-1";
                    transid = model.TransactionId + "-1";
                }
                else
                {
                    var temp = exTransaction.txnid.Split('-').Last();
                    int newTxId = Convert.ToInt32(temp) + 1;
                    //transid = model.OnlineRequestId.ToString() + model.RegistrationId.ToString() + "-" + newTxId;
                    transid = model.TransactionId + "-" + newTxId;
                }

                //var service = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == model.OnlineRequestId);
                //var processingCharge = dbContext.CitizenService_Master.FirstOrDefault(c => c.Deptt_Id == service.DepartmentId && c.service_id == service.ServiceId && c.Status == 1).Amount;
                //var gst = processingCharge != null ? processingCharge * (decimal?)0.18 : 0;
                //processingCharge = processingCharge + Math.Round(gst.Value, 2);
                OnlineApplicationDetails_trans transaction = new OnlineApplicationDetails_trans
                {
                    TrKey = transactionKey,
                    ServiceRefId = model.OnlineRequestId,
                    txnid = transid,
                    Amount = model.Amount,
                    //Amount = processingCharge,
                    productinfo = model.ProductInfo,
                    mode = "Online",
                    //ServiceType = NAConstant.OnlinePayment,
                    GetwayName = model.BankId == 67 ? "HDFC" : (model.BankId == 96 ? "BOB" : ("OTHER")),
                    status = 0,
                    EntryDate = DateTime.Now,
                    RegistrationId = model.RegistrationId,
                    OnlineRequestId = model.OnlineRequestId,
                    ChallanId = model.ChallanId.ToString(),
                    ChallanRefId = model.ChallanRefId
                };
                dbContext.OnlineApplicationDetails_trans.Add(transaction);
                dbContext.SaveChanges();

                //service.ChallanId = model.ChallanId;
                ////service.ServiceFee = model.OnlinePaymentModel.Amount;
                //service.ServiceFee = processingCharge;
                //dbContext.SaveChanges();

                model.TransactionId = transid;
                model.Amount = model.Amount;

                PaymentGateway gateway = new PaymentGateway();
                if (model.BankId == 1)//indusind
                {
                    gateway.PayOnline(model);
                }
                if (model.BankId == 67) //earlier bankid-2 for hdfc only for payment
                {
                    gateway.PayOnlineHDFC(model);
                }
                if (model.BankId == 96) //for Bank Of Baroda
                {
                    HttpContext.Current.Session["OnlineBankId"] = model.BankId;
                    gateway.PayBankOfBaroda(model);
                }
            }
        }


        public OnlinePaymentViewModel UpdateChallanOnlinePaymentTransaction(System.Web.Mvc.FormCollection form)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var TransactionModel = new OnlinePaymentViewModel();
                if (HttpContext.Current.Session["OnlineBankId"] != null)
                {
                    var bankId = (int)HttpContext.Current.Session["OnlineBankId"];
                    var bob = UpdateBankOfBarodaOnlinePayment(form);
                    HttpContext.Current.Session["OnlineBankId"] = null;
                    return bob;
                }
                else
                {
                    //string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";
                    string HashSequence = ConfigurationManager.AppSettings["hashSequence"];
                    var ServiceDetailModel = new ServiceRequestViewModel();
                    
                    if (form != null && form["status"].ToString() == "success")
                    {
                        var id = form["udf1"]; // TransactionId
                        var rid = form["udf2"]; // RegistrationId
                        var requestId = form["udf3"]; // OnlineRequestId
                        var challanId = form["udf4"]; // ChallanId
                        var serviceRequestId = !string.IsNullOrEmpty(requestId) ? Convert.ToInt32(requestId) : 0;//requestId
                        var paidAmount = Convert.ToDecimal(form["amount"]);

                        //var serviceRequest = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == serviceRequestId);
                        //var processingCharge = dbContext.CitizenService_Master.FirstOrDefault(c => c.Deptt_Id == serviceRequest.DepartmentId && c.service_id == serviceRequest.ServiceId && c.Status == 1).Amount;

                        string[] paramArr = HashSequence.Split('|');
                        Array.Reverse(paramArr);
                        string paramSequence = ConfigurationManager.AppSettings["HDFC_SALT"] + "|" + form["status"].ToString();
                        foreach (string param in paramArr)
                        {
                            paramSequence += "|";
                            paramSequence = paramSequence + (form[param] != null ? form[param] : "");
                        }

                        string hashSequence = PaymentGateway.GenerateSHA512HashCode(paramSequence).ToLower();
                        if (hashSequence == form["hash"])
                        {
                            var transaction = dbContext.OnlineApplicationDetails_trans.Where(m => m.txnid == id).FirstOrDefault();
                            if (transaction != null)
                            {
                                //transaction.TranStatus = (form["status"].ToString() == "success" && !string.IsNullOrEmpty(form["bank_ref_num"])) ? NAStatusId.Success : NAStatusId.Failed;
                                transaction.TranStatus = form["status"].ToString() == "success" ? NAStatusId.Success : NAStatusId.Failed;
                                transaction.status = form["status"].ToString() == "success" ? 1 : 0;
                                transaction.error = form["error"];
                                transaction.error_Message = form["error_Message"];
                                transaction.TrKey = form["txnid"].ToString();
                                transaction.mode = form["mode"].ToString();
                                transaction.mihpayid = form["mihpayid"];
                                transaction.productinfo = form["productinfo"];
                                transaction.EntryDate = DateTime.Now;
                                transaction.payment_source = form["payment_source"];
                                transaction.PG_Type = form["PG_Type"];
                                transaction.bank_ref_num = form["bank_ref_num"];
                                transaction.bankcode = form["bankcode"];
                                transaction.name_on_card = form["name_on_card"];
                                transaction.cardnum = form["cardnum"];
                                //transaction.cardhash = form["cardhash"];
                                transaction.issuing_bank = form["issuing_bank"];
                                transaction.card_type = form["card_type"];
                                transaction.udf1 = form["udf1"];
                                transaction.udf2 = form["udf2"];
                                transaction.udf3 = form["udf3"];
                                transaction.udf4 = form["udf4"];
                                transaction.ChallanId = form["udf4"];
                                dbContext.SaveChanges();

                                //var chalan = dbContext.Challan_Master.FirstOrDefault(c => c.Id.ToString() == challanId);

                                //TransactionModel.ChallanRefId = chalan.Challan_Id;
                                //TransactionModel.RegistrationId = chalan.Rid;
                                //TransactionModel.DepartmentId = chalan.Department_Id;
                                //TransactionModel.Department = (chalan.Department_Id != null && chalan.Department_Id != 0) ? dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == chalan.Department_Id).departmentName : string.Empty;
                                //TransactionModel.SectorId = chalan.Sector_Id;
                                //TransactionModel.Sector = (chalan.Sector_Id != null && chalan.Sector_Id != 0) ? dbContext.SectorMsts.FirstOrDefault(s => s.sectorId == chalan.Sector_Id).sectorName : string.Empty;
                                //TransactionModel.BlockId = chalan.Block_Id;
                                //TransactionModel.Block = (chalan.Block_Id != null && chalan.Block_Id != 0) ? dbContext.BlockMsts.FirstOrDefault(b => b.blockId == chalan.Block_Id).blockName : string.Empty;
                                //TransactionModel.PlotNo = chalan.Plot_No;
                                //TransactionModel.Applicant = chalan.Allottee;
                                //TransactionModel.Email = chalan.Email;
                                //TransactionModel.MobileNo = chalan.Mobile_No;
                                //TransactionModel.BankId = chalan.Bank_Id;
                                //TransactionModel.BankName = chalan.Bank_Id != null ? dbContext.BankMsts.FirstOrDefault(b => b.bankId == chalan.Bank_Id).bankName : string.Empty;
                                //TransactionModel.BranchId = chalan.Branch_Id;
                                //TransactionModel.BranchAddress = chalan.BranchMst.branchName;
                                //TransactionModel.AccountNo = chalan.Account_Number;
                                //TransactionModel.ChallanDate = chalan.Created_Date;
                                //TransactionModel.HtmlContent = chalan.Content;

                                TransactionModel = GetOnlinePaymentDetailById(new OnlinePaymentViewModel { TransactionId = id });

                                if (form["status"].ToString() == "success")
                                {
                                    var chalan = dbContext.Challan_Master.FirstOrDefault(c => c.Id.ToString() == challanId);
                                    chalan.Is_Verified = true;
                                    dbContext.SaveChanges();
                                    TransactionModel.ReturnTypeId = ReturnType.Success; //success
                                    TransactionModel.Status = form["status"].ToString();
                                    string message = string.Format(NAMessages.OnlineProcessingFee, form["productinfo"], transaction.ServiceRefId, transaction.Amount);
                                    if (!string.IsNullOrEmpty(chalan.Mobile_No)) ApplicationHelper.SendSMS(chalan.Mobile_No, message);
                                    if (!string.IsNullOrEmpty(chalan.Email)) ApplicationHelper.SendEmail(chalan.Email, "Online Request", message);
                                }
                                else
                                {
                                    TransactionModel.ReturnTypeId = ReturnType.Failed;
                                    TransactionModel.Status = form["status"].ToString();
                                }

                                //var paydetail = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(c => c.txnid == id);
                                //TransactionModel.Amount = paydetail.Amount;
                                //TransactionModel.NameOnCard = paydetail.name_on_card;
                                //TransactionModel.CardNumber = paydetail.cardnum;
                                //TransactionModel.BankReferenceNo = paydetail.bank_ref_num;
                                //TransactionModel.Mode = paydetail.mode;
                                //TransactionModel.TransactionStatus = form["status"].ToString();
                                //TransactionModel.EntryDate = paydetail.EntryDate;
                                //TransactionModel.TransactionId = paydetail.txnid;
                                ////TransactionModel.ReturnTypeId = ReturnType.Success;
                                ////TransactionModel.Status = form["status"].ToString();
                                ////return ServiceDetailModel;
                            }
                            else
                            {
                                TransactionModel.ReturnTypeId = ReturnType.Mismatch; //mismatch
                            }
                        }
                        else
                        {
                            TransactionModel.ReturnTypeId = ReturnType.Mismatch; //mismatch
                        }
                    }
                    else
                    {
                        TransactionModel.ReturnTypeId = ReturnType.Failed; // failed
                    }
                }

                //ServiceDetailModel.OnlinePaymentModel = TransactionModel;
                return TransactionModel;
            }
        }

        public OnlinePaymentViewModel UpdateBankOfBarodaOnlinePayment(System.Web.Mvc.FormCollection form)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var ServiceDetailModel = new ServiceRequestViewModel();
                var TransactionModel = new OnlinePaymentViewModel();

                string resourcePath = ConfigurationManager.AppSettings["BOB_RESOURCE_PATH"];
                string recieptURL = ConfigurationManager.AppSettings["BOB_RETURN_URL"];
                string errorURL = ConfigurationManager.AppSettings["BOB_RETURN_URL"];
                string aliasName = ConfigurationManager.AppSettings["BOB_ALIAS_NAME"];

                iPayPipe pipe = new iPayPipe();
                pipe.setAlias(aliasName);
                pipe.setResourcePath(resourcePath); //pipe.getResourcePath();
                pipe.setKeystorePath(resourcePath); 

                if (form != null)
                {
                    if (!String.IsNullOrEmpty(form["trandata"]))
                    {
                        int a = pipe.parseEncryptedRequest(form["trandata"].ToString());                        
                        var challanId = pipe.getUdf19(); // ChallanId
                        var rid = pipe.getUdf20(); // RegistrationId
                        var transid = pipe.getUdf21(); //pipe.getTransId(); // TransactionId
                        var paidAmount = Convert.ToDecimal(pipe.getAmt());
                        if (a == 0)
                        {
                            string errorText = !String.IsNullOrEmpty(HttpContext.Current.Request.Form["ErrorText"]) ? HttpContext.Current.Request.Form["ErrorText"] : !String.IsNullOrEmpty(HttpContext.Current.Request.QueryString.Get("ErrorText")) ? HttpContext.Current.Request.QueryString.Get("ErrorText") : "";
                            var result = pipe.getResult();
                            //var id = pipe.getUdf1();
                            var transaction = dbContext.OnlineApplicationDetails_trans.Where(m => m.txnid == transid).FirstOrDefault();
                            if (transaction != null)
                            {
                                //transaction.TranStatus = (form["status"].ToString() == "success" && !string.IsNullOrEmpty(form["bank_ref_num"])) ? NAStatusId.Success : NAStatusId.Failed;
                                transaction.TranStatus = a == 0 ? NAStatusId.Success : NAStatusId.Failed;
                                transaction.status = a == 0 ? 1 : 0;
                                transaction.error = errorText;// form["error"];
                                transaction.error_Message = errorText; // form["error_Message"];
                                //transaction.TrKey = pipe.getTransId();  //form["txnid"].ToString();
                                transaction.mode = "Online"; //pipe.getPmntmode(); //form["mode"].ToString();
                                transaction.mihpayid = pipe.getPaymentId(); //form["mihpayid"];
                                //transaction.productinfo = form["productinfo"];
                                transaction.EntryDate = DateTime.Now;
                                transaction.payment_source = pipe.getType(); //form["payment_source"];
                                transaction.PG_Type = "BOB"; //pipe.getPmntmode();  //form["PG_Type"];
                                transaction.bank_ref_num = pipe.getRef();  //form["bank_ref_num"];
                                transaction.bankcode = pipe.getRef(); //form["bankcode"];
                                transaction.name_on_card = pipe.getMember(); //form["name_on_card"];
                                transaction.cardnum = pipe.getCard(); //form["cardnum"];
                                //transaction.cardhash = form["cardhash"];
                                transaction.issuing_bank = "Bank Of Baroda"; //form["issuing_bank"];
                                transaction.card_type = pipe.getType(); //form["card_type"];
                                transaction.udf1 = pipe.getTransId();
                                transaction.udf2 = pipe.getTrackId();
                                transaction.ChallanId = pipe.getUdf19(); 
                                dbContext.SaveChanges();

                                //var request = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == transaction.ServiceRefId);
                                TransactionModel = GetOnlinePaymentDetailById(new OnlinePaymentViewModel { TransactionId = transid });
                                if (a == 0)
                                {
                                    //var challan = dbContext.Challan_Master.FirstOrDefault(c => c.Id.ToString() == challanId);
                                    var challan = dbContext.Challan_Master.FirstOrDefault(c => c.Id.ToString() == TransactionModel.ChallanId.ToString());
                                    if (challan != null) challan.Is_Verified = true;
                                    dbContext.SaveChanges();
                                    TransactionModel.ReturnTypeId = ReturnType.Success; //success
                                    TransactionModel.Status = "success";
                                    string message = string.Format(NAMessages.OnlineProcessingFee, "Online Payment", transaction.ServiceRefId, transaction.Amount);
                                    if (!string.IsNullOrEmpty(TransactionModel.MobileNo)) ApplicationHelper.SendSMS(TransactionModel.MobileNo, message);
                                    if (!string.IsNullOrEmpty(TransactionModel.Email)) ApplicationHelper.SendEmail(TransactionModel.Email, "Online Request", message);
                                    //form = null;
                                }
                                else
                                {
                                    TransactionModel.ReturnTypeId = ReturnType.Failed; //failed
                                    TransactionModel.Status = "failed";
                                }
                            }
                            else
                            {
                                TransactionModel.ReturnTypeId = ReturnType.Mismatch; //mismatch
                            }
                        }
                    }
                    else
                    {
                        TransactionModel.ReturnTypeId = ReturnType.Failed; // failed
                    }
                }
                else
                {
                    TransactionModel.ReturnTypeId = ReturnType.Failed; // failed
                }

                //ServiceDetailModel.OnlinePaymentModel = TransactionModel;
                return TransactionModel;
            }
            //return null;
        }


        public DataSourceResult GetOnlinePaymentDetailsAsDataSource(DataSourceRequest request, OnlinePaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities()) 
            {
                var data = (from trans in dbContext.OnlineApplicationDetails_trans
                            //join alotment in dbContext.AllotmentMasters on trans.RegistrationId equals alotment.rid
                            //join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            join chalan in dbContext.Challan_Master on trans.ChallanId equals chalan.Id.ToString()
                            where !string.IsNullOrEmpty(trans.ChallanId) && trans.RegistrationId != 10000013 //&& trans.ServiceRefId != null
                            && (model.Id == null || trans.AutoID == model.Id)
                            && (model.StartDate == null || DbFunctions.TruncateTime(trans.EntryDate) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(trans.EntryDate) <= DbFunctions.TruncateTime(model.EndDate))
                            && (model.StatusId == null || trans.status == model.StatusId)
                            select new OnlinePaymentViewModel
                            {
                                Id = trans.AutoID,
                                ApplicationFormId = null,
                                ServiceRefId = trans.ServiceRefId,
                                TransactionId = trans.txnid,
                                TransactionKey = trans.TrKey,
                                Amount = trans.Amount,
                                PaymentTypeId = trans.ServiceType,
                                GatewayName = trans.GetwayName,
                                ProductInfo = trans.productinfo,
                                Udf1 = trans.udf1,
                                Udf2 = trans.udf2,
                                Udf3 = trans.udf3,
                                Udf4 = trans.udf4,
                                Udf5 = trans.udf5,
                                Mihpayid = trans.mihpayid,
                                Mode = trans.mode,
                                TransactionStatusId = trans.TranStatus,
                                TransactionStatus = trans.TranStatus == 1 ? "Success" : "Failed",
                                StatusId = trans.status,
                                Status = trans.status != null ? dbContext.StatusMasters.FirstOrDefault(f=>f.Id==trans.status).Status : "NA",
                                EntryDate = trans.EntryDate,
                                PaymentSource = trans.payment_source,
                                PG_Type = trans.PG_Type,
                                BankReferenceNo = trans.bank_ref_num,
                                BankCode = trans.bankcode,
                                NameOnCard = trans.name_on_card,
                                CardNumber = trans.cardnum,
                                IssuingBank = trans.issuing_bank,
                                RegistrationId = trans.RegistrationId,
                                OnlineRequestId = trans.OnlineRequestId,
                                //ChallanRefId = trans.ChallanId,
                                ChallanId = trans.ChallanId != null ? dbContext.Challan_Master.FirstOrDefault(c=>c.Id.ToString()==trans.ChallanId).Id : 0,
                                ChallanRefId = trans.ChallanId != null ? dbContext.Challan_Master.FirstOrDefault(c => c.Id.ToString() == trans.ChallanId).Challan_Id : null,
                                BankId = chalan.Bank_Id,
                                BankName = dbContext.BankMsts.FirstOrDefault(b=>b.bankId==chalan.Bank_Id && b.IsActive==true).bankName,
                                SectorId = chalan.Sector_Id,
                                BlockId = chalan.Block_Id,
                                PlotNo = chalan.Plot_No,
                                Sector = chalan.Sector_Id != null ? dbContext.SectorMsts.FirstOrDefault(s=>s.sectorId==chalan.Sector_Id).sectorName : string.Empty,
                                Block = chalan.Block_Id != null ? dbContext.BlockMsts.FirstOrDefault(b=>b.blockId==chalan.Block_Id).blockName : string.Empty,
                                DepartmentId = chalan.Department_Id,
                                Department = chalan.Department_Id != null ? dbContext.DepartmentMsts.FirstOrDefault(x=>x.departmentId==chalan.Department_Id).departmentName : string.Empty
                            });
                return data != null ? data.ToDataSourceResult(request) : null;
            }
        }


        public OnlinePaymentViewModel GetOnlinePaymentDetailById(OnlinePaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from trans in dbContext.OnlineApplicationDetails_trans
                            join challan in dbContext.Challan_Master on trans.ChallanId equals challan.Id.ToString()
                            where trans.ChallanId != null
                            && (model.TransactionId == null || trans.txnid == model.TransactionId)
                            && (model.Id == null || trans.AutoID == model.Id)
                            select new OnlinePaymentViewModel
                            {
                                Id = trans.AutoID,
                                ApplicationFormId = null,
                                ServiceRefId = trans.ServiceRefId,
                                TransactionId = trans.txnid,
                                TransactionKey = trans.TrKey,
                                Amount = trans.Amount,
                                PaymentTypeId = trans.ServiceType,
                                GatewayName = trans.GetwayName,
                                ProductInfo = trans.productinfo,
                                Udf1 = trans.udf1,
                                Udf2 = trans.udf2,
                                Udf3 = trans.udf3,
                                Udf4 = trans.udf4,
                                Udf5 = trans.udf5,
                                Mihpayid = trans.mihpayid,
                                Mode = trans.mode,
                                TransactionStatusId = trans.TranStatus == 1 ? 1 : 0,
                                TransactionStatus = trans.TranStatus == 1 ? "Success" : "Failed",
                                StatusId = trans.status,
                                Status = trans.status != null ? dbContext.StatusMasters.FirstOrDefault(f => f.Id == trans.status).Status : "NA",
                                EntryDate = trans.EntryDate,
                                PaymentSource = trans.payment_source,
                                PG_Type = trans.PG_Type,
                                BankReferenceNo = trans.bank_ref_num,
                                BankCode = trans.bankcode,
                                NameOnCard = trans.name_on_card,
                                CardNumber = trans.cardnum,
                                IssuingBank = trans.issuing_bank,
                                RegistrationId = trans.RegistrationId,
                                OnlineRequestId = trans.OnlineRequestId,
                                ChallanRefId = trans.ChallanId,

                                ChallanId = challan.Id,
                                SectorId = challan.Sector_Id,
                                Sector = challan.Sector_Id != null ? dbContext.SectorMsts.FirstOrDefault(s=>s.sectorId==challan.Sector_Id).sectorName : "NA",
                                BlockId = challan.Block_Id,
                                Block = challan.Block_Id != null ? dbContext.BlockMsts.FirstOrDefault(b=>b.blockId==challan.Block_Id).blockName : "NA",
                                PlotNo = challan.Plot_No,
                                DepartmentId = challan.Department_Id,
                                Department = challan.Department_Id != null ? dbContext.DepartmentMsts.FirstOrDefault(d=>d.departmentId==challan.Department_Id).departmentName : "NA",
                                Applicant = challan.Allottee,
                                AddressI = challan.Address,
                                MobileNo = challan.Mobile_No,
                                Email = challan.Email,
                                BankId = challan.Bank_Id,
                                BankName = challan.Bank_Id != null ? dbContext.BankMsts.FirstOrDefault(c=>c.bankId==challan.Bank_Id).bankName : "NA",
                                BranchId = challan.Branch_Id,
                                BranchAddress = challan.Branch_Id != null ? dbContext.BranchMsts.FirstOrDefault(r=>r.branchId==challan.Branch_Id).branchName : "NA",
                                AccountNo = challan.Account_Number,
                                PAN = challan.PAN,
                                GSTNo = challan.GST_No,
                                ChallanDate = challan.Generate_Date,
                                HtmlContent = challan.Content,
                                IsChallanVerifed = challan.Is_Verified
                            }).FirstOrDefault();
                return data;
            }
        }


        public DataSourceResult GetOnlinePaidChallanIdListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from trans in dbContext.OnlineApplicationDetails_trans
                            where !string.IsNullOrEmpty(trans.ChallanId)
                            select new DropdownViewModel
                            {
                                Id=trans.AutoID,
                                Text = trans.ChallanId,
                                ChallanRefId = trans.ChallanId
                            }).Distinct();
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetOnlineTransactionIdListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from trans in dbContext.OnlineApplicationDetails_trans
                            where !string.IsNullOrEmpty(trans.ChallanId) && trans.ChallanId == model.ChallanRefId
                            select new DropdownViewModel
                            {
                                Id = trans.AutoID,
                                Text = trans.txnid,
                                TransactionId = trans.txnid,
                                ChallanRefId = trans.ChallanId
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public int UpdateOnlinePaymentDetailById(OnlinePaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                var payment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(c => c.AutoID == model.Id);
                if (payment != null)
                {
                    payment.ServiceRefId = model.ServiceRefId;
                    payment.PG_Type = model.PG_Type;
                    payment.bank_ref_num = model.BankReferenceNo;
                    payment.bankcode = model.BankCode;
                    payment.name_on_card = model.NameOnCard;
                    payment.cardnum = model.CardNumber;
                    payment.issuing_bank = model.IssuingBank;
                    payment.card_type = model.CardType;
                    payment.RegistrationId = model.RegistrationId != null ? model.RegistrationId : payment.RegistrationId;
                    payment.OnlineRequestId = model.OnlineRequestId;
                    payment.TranStatus = model.TransactionStatusId;
                    payment.status = model.StatusId;
                    payment.modifiydate = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                return flag;
            }
        }


        public OnlineFormViewModel SaveOpenEndedSchemeFormDetail(OnlineFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            string encryptedId = string.Empty;
            int nicmsg = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        OnlineApplicationDetail application = new OnlineApplicationDetail();
                        model.Id = model.Id == null ? 0 : model.Id;
                        var detail = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == model.Id);
                        if (detail == null)
                        {
                            if (model.ApplicantType == "Individual")
                            {
                                application.firstName = model.FirstName;
                                application.middleName = model.MiddleName;
                                application.lastName = model.LastName;
                                application.fatherHusbandName = model.FatherName;
                                application.motherName = model.MotherName;
                                application.gender = model.Gender;
                                application.religionId = model.ReligionId;
                                application.occupationId = model.OccupationId;
                                //application.quotaId = model.CategoryId;
                                application.quotaId = Constants.General;
                                application.marritalStatus = model.MaritalStatus;
                                application.dateOfBirth = model.DOB;
                            }
                            else
                            {
                                application.CompanyName = model.Applicant;
                                application.signingAuthority = model.SigningAuthority;
                                application.registeredOffice = model.PermanentAddress;
                                application.CompanyType = model.CompanyTypeId;
                                application.gender = string.IsNullOrEmpty(model.ApplicantType) ? Constants.Company : model.ApplicantType;
                                application.fatherHusbandName = model.ApplicantMaster;
                                application.quotaId = model.SigningAuthorityId;
                            }
                            application.CompanyType = model.CompanyTypeId;
                            application.schemeId = model.SchemeId;
                            application.departmentId = model.DepartmentId;
                            application.permanentAdd = model.PermanentAddress;
                            application.correspondanceAdd = model.CorrespondingAddress;
                            application.area = model.AreaRangeId;
                            //application.Phase = Convert.ToInt32(model.AreaRange);
                            application.mobileNumberP2 = model.MobileNumber;
                            application.phoneNumberP2 = model.PhoneNumber;
                            application.faxNumberP2 = model.FaxNumber;
                            application.email = model.Email;
                            application.annualIncome = model.AnnualIncome;
                            application.ApplicationFee = model.ApplicationFee;
                            application.FormCGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.FormSGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.ProcessingCharge = model.ProcessingCharge;
                            application.ProcessingCGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.ProcessingSGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.EarnestMoney = model.EarnestMoney;
                            application.TotalAmount = model.TotalAmount;
                            application.pan = model.PanNumber;
                            application.AadharNumber = model.AadharNumber;
                            //application.faxNumberP2 = model.ApplicantGSTNumber;
                            application.GSTNO = model.ApplicantGSTNumber;
                            application.ApplicationDate = DateTime.Now;

                            //application.RefundBankId = model.RefundBankId;
                            //application.RefundInfaverof = model.RefundInfaverof;
                            //application.RefundaccountNo = model.RefundAccountNo;

                            application.PropertyTypeID = model.PropertyTypeId;
                            application.Sector = model.Sector;
                            application.BranchName = model.BranchName;
                            application.IFSCCode = model.IFSCCode;

                            application.FormCategory = model.ApplicationFormType;
                            if (model.ApplicationFormType == "Expansion")
                            {
                                application.FormSubCategory = model.ExpansionType;
                                application.RentingDate = model.LetterDate;
                                application.RentingLetterNo = model.LetterCode;
                                application.RentingPropertyNo = model.PropertyNo;
                            }

                            application.PropertyNo = model.PropertyNo;
                            application.ExistingPropertyNo = model.ExistingProperty;
                            application.AllotmentDate = model.AllottmentDate;
                            application.DispatchDate = model.DispatchDate;

                            application.createdBy = "Online";
                            application.createdDate = DateTime.Now;
                            application.isActive = true;
                            application.IsSubmited = false;

                            //application.projectname = model.ProposedModel.ProposedProject;
                            //application.projectcost = model.ProposedModel.TotalCost.ToString();
                            //application.projecttimeempl = model.ProposedModel.ImplementationTime;
                            application.Online_offline = model.FormType == "Online" ? "Y" : "N";

                            string PassWord = string.Empty;
                            if (model.SchemeType == OnlineSchemeType.OpenEnded || model.SchemeType == OnlineSchemeType.Transport)
                            {
                                PassWord = ApplicationHelper.GeneratePassWordForScheme();
                                application.Userpassword = PassWord.ToMD5HashForPasswordPIS();
                            }

                            dbContext.OnlineApplicationDetails.Add(application);
                            dbContext.SaveChanges();
                            //added on 1-nov-2019
                            model.ApplicationFormId = application.onlineapplicationId;
                            //Method to save NIC Get Values.  commented on 19-Nov-2019
                            //if (model.BasicDetailsGetModel != null)
                            //{
                            //    if (model.BasicDetailsGetModel.Table != null)
                            //    {
                            //        if (!string.IsNullOrEmpty(model.BasicDetailsGetModel.Table.Control_ID))
                            //        {
                            //            model.BasicDetailsGetModel.Table.OnlineApplicationId = application.onlineapplicationId;
                            //            model.BasicDetailsGetModel.Table.departmentId = (int)application.departmentId;
                            //            model.BasicDetailsGetModel.Table.SchemeId = (int)application.schemeId;
                            //            if (ReturnType.Saved == ValidateNICSingleWindowData(application))
                            //            {
                            //                NICsingalwindowSystem nicmodel = new NICsingalwindowSystem();
                            //                nicmodel = MapNICsingalwindowSystemTable(model.BasicDetailsGetModel);
                            //                nicmodel.ServiceID = nicmodel.ServiceID == null ? model.NICServiceId : nicmodel.ServiceID;
                            //                nicmodel.Fee_Amount = model.TotalAmount;
                            //                nicmodel.Status_Code = Convert.ToInt32(ServiceStatus.FEE_PENDING);
                            //                nicmodel.Fee_Status = ServiceStatus_Text.FEE_PENDING;
                            //                dbContext.NICsingalwindowSystems.Add(nicmodel);
                            //                dbContext.SaveChanges();
                            //                model.FormStatusId = NAStatusId.Success;
                            //                nicmsg = 1;
                            //            }
                            //        }
                            //    }                                
                            //}

                            if (!string.IsNullOrEmpty(model.NICControlId))
                            {
                                Table xmlTbl = new Table();
                                xmlTbl.OnlineApplicationId = application.onlineapplicationId;
                                xmlTbl.departmentId = application.departmentId.Value;
                                xmlTbl.SchemeId = application.schemeId.Value;
                                xmlTbl.ServiceID = model.NICServiceId;
                                xmlTbl.Control_ID = model.NICControlId;
                                xmlTbl.ApplicationID = model.NICApplicationId;
                                xmlTbl.Unit_Id = model.NICUnitId;
                                xmlTbl.ProcessIndustryID = application.onlineapplicationId.ToString();
                                NewDataSet nds = new NewDataSet();
                                nds.Table = xmlTbl;

                                NICsingalwindowSystem NICdata = new NICsingalwindowSystem();
                                NICdata = MapNICsingalwindowSystemTable(nds);
                                NICdata.Fee_Amount = model.FormFeeWithGST;
                                NICdata.Status_Code = Convert.ToInt32(ServiceStatus.FEE_PENDING);
                                NICdata.Fee_Status = ServiceStatus_Text.FEE_PENDING;
                                dbContext.NICsingalwindowSystems.Add(NICdata);
                                dbContext.SaveChanges();
                                model.FormStatusId = NAStatusId.Success;
                                nicmsg = 1;
                            }

                            int id = application.onlineapplicationId;
                            model.Id = id;
                            model.ApplicationFormId = id;
                            model.NICApplicationId = Convert.ToString(id);
                            model.NICProcessIndustryId = Convert.ToString(id);
                            SaveDocumentsForApplicationForm(model, null, userImage, signatureImage);

                            UpdateDirectorDetailsForOpenSchemeForm(id);

                            string message = string.Format(NAMessages.OnlineApplicationSubmitted, id);
                            if (!string.IsNullOrEmpty(model.Email)) ApplicationHelper.SendEmail(model.Email, "Registration Form", message);
                            if (!string.IsNullOrEmpty(model.MobileNumber)) ApplicationHelper.SendSMS(model.MobileNumber, message);
                            
                            if (model.SchemeType == OnlineSchemeType.OpenEnded || model.SchemeType == OnlineSchemeType.Transport)
                            {
                                if (nicmsg == 0)
                                {
                                    string mobileMessage = string.Format(NAMessages.PIS_Registration_Activation, id, PassWord);
                                    string emailMessage = string.Format(NAMessages.PIS_Registration_Activation, id, PassWord);
                                    if (model.MobileNumber != null && model.MobileNumber != "") ApplicationHelper.SendSMS(model.MobileNumber, mobileMessage);
                                    if (model.Email != null && model.Email != "") ApplicationHelper.SendEmail(model.Email, "OnlineForm", emailMessage);
                                }
                            }
                            encryptedId = CommonHelper.Encode(model.Id.ToString());
                        }
                        else
                        {
                            int flag = UpdateOpenSchemeForm(model, userImage, signatureImage);
                            model.FormStatusId = NAStatusId.Success;
                            encryptedId = CommonHelper.Encode(model.Id.ToString());
                        }
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        //return string.Empty;
                        scope.Dispose();
                    }
                }
            }
            return model;
        }


        public OnlineFormViewModel GetOpenEndedSchemeFormDataById(OnlineFormViewModel form)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                OnlineFormViewModel applicant = new OnlineFormViewModel();
                var detail = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == form.ApplicationFormId && c.isActive == true);
                if (detail != null)
                {
                    form.Id = detail.onlineapplicationId;
                    form.ApplicationFormId = detail.onlineapplicationId;
                    form.UserPassword = "Noida"; //user password for login existing application
                    form.SchemeId = detail.schemeId;
                    form.SchemeName = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId == form.SchemeId).schemeName;
                    form.SchemeType = dbContext.SchemeMsts.FirstOrDefault(x => x.schemeId == detail.schemeId).SchemeTypeMst.SchemeTypeDesc; //OnlineSchemeType.OpenEnded; //"Open End Scheme";
                    form.DepartmentId = detail.departmentId;
                    form.Department = dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == detail.departmentId).departmentName;
                    form.Applicant = detail.gender == "Company" ? detail.CompanyName : detail.firstName + " " + (string.IsNullOrEmpty(detail.middleName) ? string.Empty : detail.middleName + " ") + detail.lastName;
                    form.ApplicantType = detail.gender == "Company" ? "Company" : "Individual";
                    form.SigningAuthority = detail.signingAuthority;
                    form.CompanyTypeId = detail.CompanyType;
                    form.CompanyType = dbContext.Common_Config.Where(c => c.Id == detail.CompanyType).Select(c => c.Name).FirstOrDefault();
                    form.FirstName = detail.firstName;
                    form.MiddleName = detail.middleName;
                    form.LastName = detail.lastName;
                    form.Gender = detail.gender;
                    form.MaritalStatus = detail.marritalStatus;
                    form.DOB = detail.dateOfBirth;
                    form.FatherName = (!string.IsNullOrEmpty(detail.fatherHusbandName)) ? detail.fatherHusbandName : "NA";
                    form.ApplicantMaster = (!string.IsNullOrEmpty(detail.fatherHusbandName)) ? detail.fatherHusbandName : "NA";
                    form.MotherName = detail.motherName;
                    form.OccupationId = detail.occupationId;
                    form.Occupation = detail.occupationId != null ? dbContext.OccupationMsts.Where(c => c.occupationId == detail.occupationId).Select(s => s.occupation).FirstOrDefault() : string.Empty;
                    form.ReligionId = detail.religionId;
                    form.Religion = detail.religionId != null ? dbContext.ReligionMsts.Where(r => r.religionId == detail.religionId).Select(r => r.religion).FirstOrDefault() : null;
                    form.CategoryId = detail.quotaId;
                    form.CategoryName = dbContext.QuotaMsts.Where(c => c.quotaId == detail.quotaId).Select(q => q.quotaName).FirstOrDefault();
                    form.MobileNumber = detail.mobileNumberP2;
                    form.PhoneNumber = detail.phoneNumberP2;
                    form.FaxNumber = detail.faxNumberP2;
                    form.ApplicantGSTNumber = detail.GSTNO;
                    form.PermanentAddress = detail.permanentAdd;
                    form.CorrespondingAddress = detail.correspondanceAdd;
                    form.ApplicationFee = detail.ApplicationFee;
                    form.FormFeeCGST = detail.FormCGST;
                    form.FormFeeSGST = detail.FormSGST;
                    form.ProcessingCharge = detail.ProcessingCharge;
                    form.ProcessingCGST = detail.ProcessingCGST;
                    form.ProcessingSGST = detail.ProcessingSGST;
                    form.TotalAmount = detail.TotalAmount;
                    form.Email = detail.email;
                    form.AnnualIncome = detail.annualIncome;
                    form.EarnestMoney = detail.EarnestMoney;
                    form.PanNumber = detail.pan;
                    form.RefundBankId = detail.RefundBankId;
                    form.RefundBank = detail.RefundBankId != null ? dbContext.BankMsts.FirstOrDefault(b=>b.bankId==detail.RefundBankId).bankName : string.Empty;
                    form.BranchName = detail.BranchName;
                    form.RefundAccountNo = detail.RefundaccountNo;
                    form.IFSCCode = detail.IFSCCode;
                    form.RefundInfaverof = detail.RefundInfaverof;
                    form.AadharNumber = detail.AadharNumber;
                    form.Sector = detail.Sector;
                    form.PropertyNo = detail.PropertyNo;
                    form.ApplicationFormType = string.IsNullOrEmpty(detail.FormCategory) ? string.Empty : detail.FormCategory.Trim();
                    form.ExpansionType = string.IsNullOrEmpty(detail.FormSubCategory) ? string.Empty : detail.FormSubCategory.Trim();
                    form.ExistingProperty = detail.ExistingPropertyNo;
                    form.AllottmentDate = detail.AllotmentDate;
                    form.LetterCode = detail.RentingLetterNo;
                    form.LetterDate = detail.RentingDate;
                    form.DispatchDate = detail.DispatchDate;
                    form.AreaRangeId = detail.area;
                    form.AreaRange = string.IsNullOrEmpty(detail.area) ? null : dbContext.FloorMsts.Where(f => f.floorId.ToString() == detail.area).Select(f => f.floorName).FirstOrDefault();
                    form.PropertyTypeId = detail.PropertyTypeID;
                    form.PropertyType = detail.PropertyTypeID != null ? dbContext.PropertyTypeMsts.FirstOrDefault(p => p.departmentId == detail.departmentId && p.propertyTypeId == detail.PropertyTypeID && p.IsActive == true).propertyTypeName : null;
                    form.ChecklistHtml = detail.Documentfilename;
                    form.FormType = detail.Online_offline == "Y" ? "Online" : "Offline";
                    form.DirectorModel = new onlineDirectorViewModel
                    {
                        DirectorType = (detail.gender == "Company" && detail.occupationId != null) ? dbContext.Common_Config.Where(m => m.Id == detail.occupationId).FirstOrDefault().Name : string.Empty
                    };
                    form.ProposedModel = new ProposedCompanyViewModel
                    {
                        ProposedProject = detail.projectname,
                        ImplementationTime = detail.projecttimeempl,
                        TotalCost = detail.projectcost
                    };
                    form.PreviousFormNo = detail.PreviousFormNo != null ? detail.PreviousFormNo : string.Empty;
                    //form.AppType = dbContext.NICsingalwindowSystems.Where(x => x.onlineapplicationId == form.ApplicationFormId).FirstOrDefault() != null ? Constants.AppType : null;

                    if (form.NICControlId != null && form.NICUnitId != null && form.NICServiceId != null)
                    {
                        var nic = dbContext.NICsingalwindowSystems.FirstOrDefault(n => n.onlineapplicationId == form.ApplicationFormId && n.Control_ID == form.NICControlId && n.Unit_Id == form.NICUnitId && n.ServiceID == form.NICServiceId);
                        if (nic != null)
                        {
                            form.NICSingleWindowModel = nic;
                            form.AppType = Constants.NIC;
                            form.IsFromNIC = true;
                        }
                        else
                        {
                            form.AppType = Constants.Authority;
                        }
                    }
                    else
                    {
                        form.IsFromNIC = true;
                        form.AppType = Constants.Authority;
                    }
                    
                    if (form.Gender.ToLower() == Constants.Company.ToLower())
                    {
                        form.SigningAuthorityId = form.CategoryId;
                        if (form.CategoryId != null && form.CategoryId > 0)
                        {
                            form.SignatoryStatus = dbContext.Common_Config.FirstOrDefault(x => x.Id == form.CategoryId).Name;
                        }
                    }

                    form.FormFeeGST = form.FormFeeCGST + form.FormFeeSGST;
                    form.FormFeeWithGST = form.ApplicationFee + form.FormFeeGST;
                    form.ProcessingChargeGST = form.ProcessingSGST + form.ProcessingCGST;
                    form.ProcessingChargeWithGST = form.ProcessingCharge + form.ProcessingChargeGST;

                    //var doctype = applicant.ChecklistHtml.Split(',');
                    if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.ApplicationFormId + "/Pictures/")))
                    {
                        var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.ApplicationFormId + "/Pictures/");
                        string[] files = Directory.GetFiles(filepath);
                        if (files != null)
                        {
                            if (files.Count() > 0)
                            {
                                if (files.Count() == 2)
                                {
                                    if (files[0] != null)
                                    {
                                        var uplod = files[0].Split('\\');
                                        form.SignImage = "/UploadFiles/" + form.ApplicationFormId + "/Pictures/" + uplod[uplod.Length - 1];
                                    }
                                    if (files[1] != null)
                                    {
                                        var uplod = files[1].Split('\\');
                                        form.UserImage = "/UploadFiles/" + form.ApplicationFormId + "/Pictures/" + uplod[uplod.Length - 1];
                                    }
                                }
                            }
                        }
                    }
                    form.IsApplicationFeePaid = false;
                    var payment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(p => p.ServiceRefId == detail.onlineapplicationId && p.status == 1 && p.ServiceType == 1);
                    if (payment != null)
                    {
                        form.IsApplicationFeePaid = true;
                        form.PaymentMode = "Online";
                        form.PaymentModel = GetOnlinePaymentDetailById(form.ApplicationFormId, payment.AutoID);
                    }
                    else
                    {
                        var Offpayment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(p => p.ServiceRefId == detail.onlineapplicationId && p.ServiceType == 3);
                        if (Offpayment != null)
                        {
                            form.PaymentMode = "Offline";
                            form.IsChallanGenerated = true;
                            if (!string.IsNullOrEmpty(Offpayment.TrKey))
                            {
                                form.PaymentMode = "Offline_Updated";
                                if (Offpayment.status == 1 || Offpayment.TranStatus == 1)
                                {
                                    form.IsApplicationFeePaid = true;
                                    form.IsChallanGenerated = false;
                                    form.PaymentMode = "Offline_Validated";
                                }
                            }
                            form.PaymentModel = GetOnlinePaymentDetailById(form.ApplicationFormId, Offpayment.AutoID);
                        }
                    }

                    form.PaidThroughSWP = false;
                    if (form.AppType == Constants.AppType)
                    {
                        //check single window portal
                        var singleWindowPortal = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(p => p.ServiceRefId == detail.onlineapplicationId && p.ServiceType == Constants.singleWindowPortalApplicationPayment);
                        if (singleWindowPortal != null)
                        {
                            form.PaidThroughSWP = true;
                            if (singleWindowPortal.TranStatus == 1 && singleWindowPortal.status == 1)
                            {
                                form.IsApplicationFeePaid = true;
                                form.ServiceAppPayStatus = "Paid";
                            }
                            form.PaymentModel = GetOnlinePaymentDetailById(form.ApplicationFormId, singleWindowPortal.AutoID);
                        }
                    }

                    var flag = IsDocumentUploaded(form.ApplicationFormId);
                    if (flag == true)
                    {
                        form.IsDocumentUploaded = true;
                        form.DocumentsTable = dbContext.Sp_NewSchemereturn(form.ApplicationFormId, form.SchemeId).FirstOrDefault();
                    }
                    //else applicant.IsDocumentUploaded = false;

                    if (form.SchemeType == OnlineSchemeType.Transport || form.SchemeType == OnlineSchemeType.OpenEnded)
                    {
                        var PreChallan = IsPreviousChallanUploaded(form.ApplicationFormId);
                        if (PreChallan == true)
                        {
                            form.IsPreviousChallanUploaded = true;
                        }
                    }
                }
                else
                {
                    form.FlagId = ReturnType.NotExist;
                }
                return form;
            }
        }


        public OnlineFormViewModel UpdateOESFormPaymentStatus(OnlineFormViewModel model)
        {
            using (var dbContext = new Model.NoidaPMSEntities())
            {
                if (model.NICApplicationId != null && model.NICControlId != null && model.NICUnitId != null && model.NICServiceId != null )
                {
                    var exService = dbContext.NICsingalwindowSystems.FirstOrDefault(c => c.onlineapplicationId.ToString() == model.NICApplicationId && c.Control_ID == model.NICControlId && c.Unit_Id == model.NICUnitId);
                    if (exService != null)
                    {
                        exService.Status_Code = Convert.ToInt32(ServiceStatus.FEE_PAID);
                        exService.Fee_Status = ServiceStatus_Text.FEE_PAID;
                        dbContext.SaveChanges();
                        model.NICFeeStatus = ServiceStatus_Text.FEE_PAID;
                        model.NICFeeStatusId = ServiceStatus.FEE_PAID;
                        //return exService.Id;
                    }
                }
                else
                {
                    var exPayment = dbContext.OnlineApplicationDetails_trans.Where(c => c.ServiceRefId == model.ApplicationFormId).OrderByDescending(o => o.AutoID).FirstOrDefault();
                    model.IsApplicationFeePaid = (exPayment != null && exPayment.status == 1) ? true : false;
                }
                return model;
            }
        }


        public OnlineFormViewModel SaveProjectAndRefundDetailForOpenEndScheme(OnlineFormViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var flag = ReturnType.Failed;
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(f => f.onlineapplicationId == model.ApplicationFormId);
                if (form != null)
                {
                    form.RefundBankId = model.RefundBankId;
                    form.BranchName = model.BranchName;
                    form.RefundInfaverof = model.RefundInfaverof;
                    form.RefundaccountNo = model.RefundAccountNo;
                    form.IFSCCode = model.IFSCCode;
                    form.projectname = model.ProposedModel.ProposedProject;
                    form.projectcost = model.ProposedModel.TotalCost.ToString();
                    form.projecttimeempl = model.ProposedModel.ImplementationTime;
                    dbContext.SaveChanges();
                    //flag = UpdateDirectorDetailsForOpenSchemeForm(model.ApplicationFormId);        
                    model.IsDirectorDetailsSaved = true;
                }
                else
                {
                    model.IsDirectorDetailsSaved = false;
                }
                return model;
            }
        }


        public onlineDirectorViewModel SaveDirectorDetailToDataBaseForOpenScheme(onlineDirectorViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "Delete")
                {
                    var director = dbContext.online_Director_Master.FirstOrDefault(d => d.Director_Id == model.DirectorId);
                    director.Is_Active = 0;
                    dbContext.SaveChanges();
                    model.ActionTypeId = ReturnType.Removed;
                }
                else
                {
                    online_Director_Master master = new online_Director_Master();
                    master.onlineapplicationId = model.ApplicationFormId;
                    master.Director_Name = model.DirectorName;
                    master.Director_Share = model.DirectorShare;
                    master.Type = model.DirectorTypeId;
                    master.pan_no = model.PAN;
                    master.Is_Active = 1;
                    master.Created_By = model.ApplicationFormId;
                    dbContext.online_Director_Master.Add(master);
                    dbContext.SaveChanges();
                    model.IsDirectorActive = 1;
                    model.ActionTypeId = ReturnType.Saved;
                }
                
                return model;
            }
        }


        public DataSourceResult GetDirectorDetailsFromDatabaseForOpenScheme(DataSourceRequest request, int? id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var directorlist = dbContext.online_Director_Master.Where(d => d.onlineapplicationId == id).ToList();
                if (directorlist != null && directorlist.Count > 0)
                {
                    var Dirlist = (from directors in dbContext.online_Director_Master
                                   where directors.onlineapplicationId == id && directors.Is_Active == 1
                                   select new onlineDirectorViewModel
                                   {
                                       Id = directors.Director_Id,
                                       DirectorId = directors.Director_Id,
                                       ApplicationFormId = directors.onlineapplicationId,
                                       DirectorName = directors.Director_Name,
                                       DirectorShare = directors.Director_Share,
                                       PAN = directors.pan_no,
                                       DirectorTypeId = directors.Type,
                                       status = directors.Is_Active == 1 ? true : false,
                                       DirectorType = dbContext.Common_Config.FirstOrDefault(d => d.Id == directors.Type && d.Category.ToLower() == "director" && d.Is_Active == 1).Name
                                   }).ToList();
                    int count = 1;
                    Dirlist.ForEach(c => c.Id = count++);
                    
                    return Dirlist.ToDataSourceResult(request);
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
