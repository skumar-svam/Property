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

namespace NA.PMS.Repository
{
    public class PropertyCancellationRepository : IPropertyCancellationRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public PropertyCancellationRepository()
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

        /// <summary>
        /// Reads Cancellation/Surrender grid's data
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public DataSourceResult GetAllCancellationsSurrenders(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();
                var lst = (from can in dbContext.Property_Cancellation_Details
                           join appDet in dbContext.ApplicationDetails on can.Rid equals appDet.registrationId
                           join deptt in dbContext.DepartmentMsts on appDet.departmentId equals deptt.departmentId
                           join us in dbContext.UmUserMasters on can.Assigned_To equals us.UserRefId
                           join st in dbContext.StatusMasters on can.Status equals st.Id
                           join am in dbContext.AllotmentMasters on can.Rid equals am.rid
                           where can.Is_Active == true && loginUserDeptt.Contains(am.departmentId)//&& am.isActive == 1 -> Resolved Bug#234
                           select new PropertyCancellationModel
                       {
                           ReqNo = can.Id,
                           RId = can.Rid,
                           DepttName = deptt.departmentName,
                           AssignedTo = us.FirstName + " " + us.MiddleName + " " + us.LastName,
                           ReqDate = can.Request_Date,
                           ApprovedDate = can.Approve_Date,
                           Status = st.Status
                       });
                return lst.ToDataSourceResult(req);
            }
        }

        public bool SaveCancellation(int rId, int cancelType, string cancelReason, int? refundYesNo, int? refundType, decimal? refundAmt, int userVal, int? reqNo, string restoreReason, decimal? restoreCharges)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (reqNo != null && reqNo != 0)//Update existing
                {
                    var existingRecord = (from can in dbContext.Property_Cancellation_Details where can.Id == reqNo && can.Is_Active == true select can).FirstOrDefault();
                    if (existingRecord != null)
                    {
                        existingRecord.Reason = cancelReason;
                        existingRecord.Refund = refundYesNo;
                        if (refundYesNo == MortPrevLoan.yes)
                        {
                            existingRecord.Refund_Id = refundType;
                            existingRecord.Refund_Amount = refundAmt;
                        }
                        existingRecord.Assigned_To = userVal;
                        existingRecord.Modified_By = userInfo.UserID;
                        existingRecord.Modified_Date = DateTime.Now;
                        existingRecord.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.InProgress.ToString().ToLower() select st.Id).FirstOrDefault();
                        dbContext.SaveChanges();
                        flag = true;
                    }
                }
                else//Add new
                {
                    if (cancelType == Constants.intSurrender || cancelType == Constants.intCancellation)
                    {
                        var newRecord = new Property_Cancellation_Details();
                        newRecord.Rid = rId;
                        newRecord.Type = cancelType;
                        newRecord.Reason = cancelReason;
                        newRecord.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.InProgress.ToString().ToLower() select st.Id).FirstOrDefault();
                        newRecord.Refund = refundYesNo;
                        newRecord.Requested_By = userInfo.UserID;
                        if (refundYesNo == MortPrevLoan.yes)
                        {
                            newRecord.Refund_Id = refundType;
                            newRecord.Refund_Amount = refundAmt;
                        }
                        newRecord.Request_Date = DateTime.Now;
                        newRecord.Assigned_To = userVal;
                        newRecord.Is_Active = true;
                        newRecord.Created_By = userInfo.UserID;
                        newRecord.Created_Date = DateTime.Now;
                        dbContext.Property_Cancellation_Details.Add(newRecord);
                        dbContext.SaveChanges();
                        flag = true;
                    }
                    else//Case for Restoration
                    {
                        var existingCancellation = (from can in dbContext.Property_Cancellation_Details where can.Is_Active == true && can.Rid == rId && (can.Type == Constants.intCancellation || can.Type == Constants.intSurrender) select can).FirstOrDefault();
                        if (existingCancellation != null)
                        {
                            if (existingCancellation.Refund == MortPrevLoan.no)//Only those properties can be restored in which, no Refund has been initiated
                            {
                                var newRecord = new Property_Cancellation_Details();
                                newRecord.Rid = rId;
                                newRecord.Type = cancelType;
                                newRecord.Reason = restoreReason;
                                newRecord.Restoration_Charge = restoreCharges;
                                newRecord.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.InProgress.ToString().ToLower() select st.Id).FirstOrDefault();
                                newRecord.Requested_By = userInfo.UserID;
                                newRecord.Request_Date = DateTime.Now;
                                newRecord.Assigned_To = userVal;
                                newRecord.Is_Active = true;
                                newRecord.Created_By = userInfo.UserID;
                                newRecord.Created_Date = DateTime.Now;
                                dbContext.Property_Cancellation_Details.Add(newRecord);
                                dbContext.SaveChanges();
                                flag = true;
                            }
                            else//Refund initiated for the property, can't be restored
                            {
                                //Do nothing, as flag is already false
                            }
                        }
                    }
                }
            }
            return flag;
        }

        public DataSourceResult GetRIDsByCancellationType(DataSourceRequest Req, int id)
        {
            IQueryable<DDList> lst;
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstDeptts = (from u in dbContext.UmUserMasters
                                 join d in dbContext.UmUserDepartmentTrans on u.UserRefId equals d.UserRefId
                                 where u.UserRefId == userInfo.UserID
                                 select d.DepartmentId).ToList();
                if (id == Constants.intCancellation || id == Constants.intSurrender)
                {
                    lst = (from f in dbContext.AllotmentMasters
                           where f.isActive == 1 && lstDeptts.Contains(f.departmentId) && f.isStatus.ToLower() == Common.AllotmentStatus.Approved.ToString().ToLower()
                           select new DDList
                           {
                               id = f.rid,
                               text = f.rid.ToString()
                           });
                }
                else
                {
                    //var cancelledSurrenderedId = (from mst in dbContext.Common_Config where mst.Category.ToLower() == Constants.cancellation.ToLower() && mst.Is_Active == 1 && (mst.Name.ToLower() == Constants.cancellation.ToLower() || mst.Name.ToLower() == Constants.surrender.ToLower()) select mst.Id).ToList();
                    var approvedStatusId = (from st in dbContext.StatusMasters where st.Status.ToLower() == Common.AllotmentStatus.Approved.ToString().ToLower() select st.Id).FirstOrDefault();
                    lst = (from f in dbContext.Property_Cancellation_Details
                           join appDet in dbContext.ApplicationDetails on f.Rid equals appDet.registrationId
                           where f.Is_Active == true && lstDeptts.Contains(appDet.departmentId) && f.Status == approvedStatusId && (f.Type == Constants.intSurrender || f.Type == Constants.intCancellation)//cancelledSurrenderedId.Contains(f.Type.Value)
                           select new DDList
                           {
                               id = f.Rid.Value,
                               text = f.Rid.ToString()
                           });
                }
                return lst.ToDataSourceResult(Req);
            }
        }

        public List<DDList> GetRefundTypes(int rId, int depttId)
        {
            var lst = new List<DDList>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lst = (from refund in dbContext.SchemeRefundTrans
                       join appDet in dbContext.ApplicationDetails on refund.schemeId equals appDet.schemeId
                       where refund.departmentId == depttId && refund.IsActive == true && appDet.registrationId == rId
                       select new DDList
                       {
                           id = refund.refundId,
                           text = refund.refundDescription
                       }).ToList();
                return lst;
            }
        }

        public PropertyCancellationModel GetCancellationDetail(int reqNo)
        {
            var details = new PropertyCancellationModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                details = (from can in dbContext.Property_Cancellation_Details
                           join st in dbContext.StatusMasters on can.Status equals st.Id
                           join us in dbContext.UmUserMasters on can.Assigned_To equals us.UserRefId
                           where can.Id == reqNo && can.Is_Active == true
                           select new PropertyCancellationModel
                           {
                               ReqNo = reqNo,
                               Status = st.Status,
                               RefundType = can.Refund_Id,
                               RefundYesNo = can.Refund,
                               RefundAmt = can.Refund_Amount,
                               RId = can.Rid,
                               TypeOfCancel = can.Type,
                               ReasonOfCancel = (can.Type == Constants.intCancellation || can.Type == Constants.intSurrender) ? can.Reason : "",
                               From = us.FirstName + " " + us.MiddleName + " " + us.LastName,
                               SubmittedDate = can.Approve_Date,
                               Comment = can.Comment,
                               RestorationReason = (can.Type == Constants.intRestoration) ? can.Reason : "",
                               RestorationCharges = can.Restoration_Charge
                           }).FirstOrDefault();
            }
            return details;
        }

        public DataSourceResult GetAllCancellationsSurrenders_Approver(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from can in dbContext.Property_Cancellation_Details
                           join appDet in dbContext.ApplicationDetails on can.Rid equals appDet.registrationId
                           join deptt in dbContext.DepartmentMsts on appDet.departmentId equals deptt.departmentId
                           join us in dbContext.UmUserMasters on can.Assigned_To equals us.UserRefId
                           join st in dbContext.StatusMasters on can.Status equals st.Id
                           join am in dbContext.AllotmentMasters on can.Rid equals am.rid
                           where can.Is_Active == true && can.Assigned_To == userInfo.UserID //&& am.isActive == 1 -> Resolved Bug#234
                           select new PropertyCancellationModel
                           {
                               ReqNo = can.Id,
                               RId = can.Rid,
                               DepttName = deptt.departmentName,
                               AssignedTo = us.FirstName + " " + us.MiddleName + " " + us.LastName,
                               ReqDate = can.Request_Date,
                               ApprovedDate = can.Approve_Date,
                               Status = st.Status
                           });
                return lst.ToDataSourceResult(req);
            }
        }

        public bool SaveCancellationApprovalStatus(string comments, int intStatus, int ReqNo, int type)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingRecord = (from can in dbContext.Property_Cancellation_Details where can.Id == ReqNo && can.Is_Active == true select can).FirstOrDefault();
                if (existingRecord != null)
                {
                    if (intStatus == 1)//For Status Approved
                    {
                        if (type == Constants.intRestoration)
                        {
                            existingRecord.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Approved.ToString().ToLower() select st.Id).FirstOrDefault();
                            existingRecord.Modified_By = userInfo.UserID;
                            existingRecord.Modified_Date = DateTime.Now;
                            existingRecord.Approve_Date = DateTime.Now;
                            existingRecord.Comment = comments;
                            //Change Allotment Master IsActive to 1 upon Restoration
                            var allotment = (from allot in dbContext.AllotmentMasters where allot.rid == existingRecord.Rid && allot.isActive == 0 select allot).FirstOrDefault();
                            if (allotment != null)
                            {
                                allotment.isActive = 1;
                                allotment.modifiedBy = userInfo.UserID.ToString();
                                allotment.modifiedDate = DateTime.Now;
                            }
                            dbContext.SaveChanges();
                            flag = true;
                        }
                        else
                        {
                            existingRecord.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Approved.ToString().ToLower() select st.Id).FirstOrDefault();
                            existingRecord.Modified_By = userInfo.UserID;
                            existingRecord.Modified_Date = DateTime.Now;
                            existingRecord.Approve_Date = DateTime.Now;
                            existingRecord.Comment = comments;
                            //Change Allotment Master IsActive to 0 upon Cancellation/Surrender
                            var allotment = (from allot in dbContext.AllotmentMasters where allot.rid == existingRecord.Rid && allot.isActive == 1 select allot).FirstOrDefault();
                            if (allotment != null)
                            {
                                allotment.isActive = 0;
                                allotment.modifiedBy = userInfo.UserID.ToString();
                                allotment.modifiedDate = DateTime.Now;
                            }
                            dbContext.SaveChanges();
                            flag = true;
                        }
                    }
                    else //For Status Rejected
                    {
                        existingRecord.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Rejected.ToString().ToLower() select st.Id).FirstOrDefault();
                        existingRecord.Modified_By = userInfo.UserID;
                        existingRecord.Modified_Date = DateTime.Now;
                        existingRecord.Approve_Date = DateTime.Now;
                        existingRecord.Comment = comments;
                        dbContext.SaveChanges();
                        flag = true;
                    }
                }
            }
            return flag;
        }

        public bool CancelRequest(int requestId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingRecord = (from can in dbContext.Property_Cancellation_Details where can.Id == requestId && can.Is_Active == true select can).FirstOrDefault();
                if (existingRecord != null)
                {
                    existingRecord.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Cancelled.ToString().ToLower() select st.Id).FirstOrDefault();
                    existingRecord.Modified_By = userInfo.UserID;
                    existingRecord.Modified_Date = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }


        public DataSourceResult GetCancelledPropertyListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from can in dbContext.Property_Cancellation_Details
                           join appDet in dbContext.ApplicationDetails on can.Rid equals appDet.registrationId
                           join deptt in dbContext.DepartmentMsts on appDet.departmentId equals deptt.departmentId
                           join us in dbContext.UmUserMasters on can.Assigned_To equals us.UserRefId
                           join st in dbContext.StatusMasters on can.Status equals st.Id
                           join am in dbContext.AllotmentMasters on can.Rid equals am.rid
                           where can.Is_Active == true && DepartmentList.Contains(am.departmentId) && (model.Id == null || can.Id == model.Id)
                           select new PropertyViewModel
                           {
                               Id = can.Id,
                               RequestId = can.Id,
                               RegistrationId = can.Rid,
                               SchemeName = appDet.SchemeMst.schemeName,
                               Department = deptt.departmentName, 
                               DefaulterId = can.Refund,
                               SchemeRefundId = can.Refund_Id,
                               RefundTypeId = can.Type,
                               RefundType = (can.Type == Constants.PropertyCancellationId ? "Cancellation" : (can.Type == Constants.PropertySurrenderId ? "Surrender" : "Restoration")),
                               RefundAmount = can.Refund_Amount,
                               RestorationCharge = can.Restoration_Charge,
                               Comment = can.Reason,
                               ApproverId = can.Assigned_To,
                               Approver = us.FirstName + " " + (string.IsNullOrEmpty(us.MiddleName) ? string.Empty : us.MiddleName + " ") + us.LastName,
                               RequestDate = can.Request_Date,
                               ApprovalDate = can.Approve_Date,
                               Status = st.Status,
                               IsApproved = can.Status == NAStatusId.Approved ? true : false
                           });
                return lst.ToDataSourceResult(request);
            }
        }

        public int SaveCancellationDetail(PropertyViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.Id != null)//Update existing
                {
                    var existingRecord = (from can in dbContext.Property_Cancellation_Details where can.Id == model.Id && can.Is_Active == true select can).FirstOrDefault();
                    if (existingRecord != null)
                    {
                        existingRecord.Type = model.DefaulterId;
                        existingRecord.Request_Date = model.ActionDate;
                        existingRecord.Refund = model.RefundTypeId;
                        existingRecord.Refund_Id = model.SchemeRefundId;
                        existingRecord.Refund_Amount = model.DefaulterId != Constants.PropertyRestorationId ? model.RefundAmount : null;
                        existingRecord.Restoration_Charge = model.DefaulterId == Constants.PropertyRestorationId ? model.RefundAmount : null;
                        existingRecord.Reason = model.Comment;
                        existingRecord.Assigned_To = model.ApproverId;
                        existingRecord.Modified_By = userInfo.UserID;
                        existingRecord.Modified_Date = DateTime.Now;
                        existingRecord.Status = NAStatusId.InProgress;
                        dbContext.SaveChanges();
                        flag = ReturnType.Saved;
                    }
                }
                else//Add new
                {
                    var newRecord = new Property_Cancellation_Details();
                    newRecord.Rid = model.RegistrationId;
                    newRecord.Type = model.DefaulterId;
                    newRecord.Reason = model.Comment;
                    newRecord.Status = NAStatusId.Initiated;
                    newRecord.Refund = model.RefundTypeId;
                    newRecord.Refund_Id = model.SchemeRefundId;
                    newRecord.Refund_Amount = model.DefaulterId != Constants.PropertyRestorationId ? model.RefundAmount : null;
                    newRecord.Restoration_Charge = model.DefaulterId==Constants.PropertyRestorationId ? model.RefundAmount : null;
                    newRecord.Requested_By = userInfo.UserID;
                    newRecord.Request_Date = model.ActionDate;
                    newRecord.Assigned_To = model.ApproverId;
                    newRecord.Is_Active = true;
                    newRecord.Created_By = userInfo.UserID;
                    newRecord.Created_Date = DateTime.Now;
                    dbContext.Property_Cancellation_Details.Add(newRecord);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
            }
            return flag;
        }

        public PropertyViewModel GetCancellationDetailById(PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var details = (from can in dbContext.Property_Cancellation_Details
                               join st in dbContext.StatusMasters on can.Status equals st.Id
                               join us in dbContext.UmUserMasters on can.Assigned_To equals us.UserRefId
                               where can.Id == model.Id && can.Is_Active == true
                               select new PropertyViewModel
                               {
                                   Id = can.Id,
                                   RequestId = can.Id,
                                   RegistrationId = can.Rid,
                                   DefaulterId = can.Type,
                                   Status = st.Status,
                                   SchemeRefundId = can.Refund_Id,
                                   RefundTypeId = can.Refund,
                                   RefundAmount = can.Type == Constants.PropertyRestorationId ? can.Restoration_Charge : can.Refund_Amount,
                                   Comment = can.Reason,
                                   Approver = us.FirstName + " " + (string.IsNullOrEmpty(us.MiddleName) ? string.Empty : us.MiddleName + " ") + us.LastName,
                                   ActionDate = can.Request_Date,
                                   RequestDate = can.Approve_Date
                               }).FirstOrDefault();
                return details;
            }
        }

        public int SaveCancellationStatus(PropertyViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingRecord = dbContext.Property_Cancellation_Details.FirstOrDefault(c=>c.Id == model.Id && c.Is_Active == true);
                
                if (existingRecord != null)
                {
                    existingRecord.Status = model.StatusId;
                    existingRecord.Comment = existingRecord.Comment + "\n" + model.Comment + " :" + userInfo.FirstName + (string.IsNullOrEmpty(userInfo.MiddleName) ? string.Empty : userInfo.MiddleName + " ") + userInfo.LastName;
                    existingRecord.Modified_By = userInfo.UserID;
                    existingRecord.Modified_Date = DateTime.Now;
                    existingRecord.Approve_Date = DateTime.Now;
                    var allotment = dbContext.AllotmentMasters.FirstOrDefault(a => a.rid == existingRecord.Rid);
                    allotment.isActive = model.DefaulterId == Constants.PropertyRestorationId ? 1 : 0;
                    
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
            }
            return flag;
        }
    }
}
