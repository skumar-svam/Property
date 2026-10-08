using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Kendo.Mvc.Extensions;
using NA.PMS.Common;
using Kendo.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using NoidaAuthority.PMS.Common;


namespace NA.PMS.Repository
{
    public class CitizenReqeustsRepository : ICitizenRequestsRepository
    {
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public CitizenReqeustsRepository()
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

        public DataSourceResult GetCitizenRequests(DataSourceRequest req)
        {
            //For sorting Kendo DataSourceResult
            if (req.Sorts.Count == 0)
            {
                req.Sorts.Add(new SortDescriptor("RefNo",
                    System.ComponentModel.ListSortDirection.Descending));
            }
            int userid = userInfo.UserID;
            using (var dbContext = new NoidaPMSEntities())
            {
                var serviceRequests = (from serv in dbContext.Customer_ServiceRequest
                                       join csm in dbContext.CitizenService_Master on serv.ServiceId.ToString() + serv.DepartmentId.ToString() equals csm.service_id.ToString() + csm.Deptt_Id.ToString()
                                       join st in dbContext.StatusMasters on serv.Request_Status equals st.Id
                                       //from alot in dbContext.AllotmentMasters.Where(a => a.rid.ToString() == (serv.Registration_No==""?"0":(serv.Registration_No)).DefaultIfEmpty() //on serv.Registration_No equals alot.rid.ToString()
                                       //from prop in dbContext.SchemePropTrans.Where(p => p.propertyId == alot.propertyId).DefaultIfEmpty() //on alot.propertyId equals prop.propertyId
                                       where DepartmentList.Contains(serv.DepartmentId) && serv.DepartmentId == csm.Deptt_Id && csm.Status == 1 && serv.IsActive == true
                                       select new CitizenRequestsModel
                                       {
                                           RId = serv.Registration_No,
                                           DepartmentId = serv.DepartmentId,
                                           Department = (serv.DepartmentId == null || serv.DepartmentId == 0) ? "" : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == serv.DepartmentId).departmentName,
                                           //SectorName = prop.SectorMst.sectorName,
                                           //BlockName = prop.BlockMst.blockName,
                                           PropertyNo = serv.Property_No != null ? serv.Property_No : "",
                                           RefNo = serv.Id,
                                           ReqDate = serv.Created_Date,
                                           SLA = csm.Timeline,
                                           ServiceId = serv.ServiceId,
                                           ServiceName = csm.ServiceName,
                                           TotalAmount = serv.DuesAmount == null ? 0 : serv.DuesAmount,
                                           //ChallanId = serv.ChallanId,
                                           DuesAmount = serv.DuesAmount,
                                           Id = serv.Id,
                                           Status = st.Status,
                                           MobileNumber = serv.MobileNumber,
                                           ApplicantName = serv.ApplicantName,
                                           Description = serv.Description,
                                           SubDepartment = serv.SubDepartment == "P" ? SubDepartment.P : SubDepartment.A
                                       });

                var rslt = serviceRequests.ToDataSourceResult(req);
                return rslt;
            }
        }



        public RequestDetails GetRequestDetailsById(int id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var details = (from det in dbContext.Customer_ServiceRequest
                               join csm in dbContext.CitizenService_Master on det.ServiceId equals csm.service_id
                               join stMaster in dbContext.StatusMasters on det.Request_Status equals stMaster.Id
                               where det.DepartmentId == csm.Deptt_Id && det.Id == id
                               select new RequestDetails
                               {
                                   RId = det.Registration_No,
                                   Id = det.Id,
                                   ServiceName = csm.ServiceName,
                                   ReqStatus = stMaster.Status,
                                   StatusId = det.Request_Status.Value,
                                   Description = det.Description,
                                   ServiceFee = det.ServiceFee,
                                   DuesAmnt = det.DuesAmount,
                                   //Comment = det.Comment,
                                   OldComment = det.Comment,
                                   PaymentStatus = (det.PaymentStatus == 1) ? Constants.yes : Constants.no,
                                   AllotteeName = (from app in dbContext.ApplicationDetails where app.registrationId.ToString() == det.Registration_No select app.tFirstName + " " + app.tMiddleName + " " + app.tLastName).FirstOrDefault(),
                                   MobileNo = det.MobileNumber,
                                   Address = det.ApplicantAddress,
                                   RequestorName = det.RequestorName
                               }).FirstOrDefault();
                return details;
            }
        }

        public DataSourceResult GetTransferServiceReq(DataSourceRequest Req, int RID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var det = (from trans in dbContext.OnlineTransferMutations
                           join ty in dbContext.Transfer_Type on trans.Transfer_Type equals ty.Id
                           join occ in dbContext.OccupationMsts on trans.T_Occupation_Id equals occ.occupationId
                           where trans.Rid == RID
                           orderby trans.TMSid descending
                           select new TransferServiceRequestModel
                           {
                               //TransType = trans.Transfer_Type,
                               //TransSubType = trans.Transfer_Sub_Type,
                               StrTransType = ty.type,
                               StrTransSybType = (from t1 in dbContext.OnlineTransferMutations join t2 in dbContext.Transfer_Type on t1.Transfer_Sub_Type equals t2.Id where t1.Rid == RID select t2.type).FirstOrDefault(),
                               RelativeName = trans.T_Father_Husband_Name,
                               MotherName = trans.T_Mother_Name,
                               MobileNo = trans.T_Mobile,
                               Name = trans.T_First_Name + " " + trans.T_Middle_Name + " " + trans.T_Last_Name,
                               CorrespondenceAdd = trans.T_Correspondence_Add,
                               PermanentAdd = trans.T_Permanent_Add,
                               PAN = trans.T_Pan,
                               StrOccupation = occ.occupation,
                               StrGender = trans.T_Gender,
                               Email = trans.T_Email
                               //desc
                           });
                return det.ToDataSourceResult(Req);
            }
        }

        public DataSourceResult GetRentServiceReq(DataSourceRequest Req, int RID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var det = (from rent in dbContext.OnlineRentPermissionDetails
                           where rent.Rid == RID
                           orderby rent.RentingSid descending
                           select new RentPermissionModel
                           {
                               TenantName = rent.TenantName,
                               RentingDate = rent.RentingDate,//.Value.Date,
                               TenantProject = rent.TenantProject,
                               RentDuration = rent.RentDurationYears
                           });
                //if (det != null)
                //    det.RentingDate = det.RentingDate.Value.Date;
                return det.ToDataSourceResult(Req);
            }
        }

        public DataSourceResult GetExtensionServiceReq(DataSourceRequest Req, int RID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var det = (from extend in dbContext.OnlineExtensionDetails
                           where extend.Rid == RID
                           orderby extend.ExtensionSid descending
                           select new ExtensionDetails
                           {
                               Extension_Due_Date = extend.ExtensionDueDate,
                               Extension_Given_Date = extend.ExtensionGivenDate
                           });
                //if (det != null)
                //    det.RentingDate = det.RentingDate.Value.Date;
                return det.ToDataSourceResult(Req);
            }
        }

        public DataSourceResult GetCICServiceReq(DataSourceRequest Req, int RID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var det = (from rent in dbContext.online
                //           where rent.Rid == RID
                //           orderby rent.RentingSid descending
                //           select new RentPermissionModel
                //           {
                //               TenantName = rent.TenantName,
                //               RentingDate = rent.RentingDate.Value.Date,
                //               TenantProject = rent.TenantProject,
                //               RentDuration = rent.RentDurationYears
                //           }).FirstOrDefault();
                var CIC = new CICModel();
                return null;
                //return det;
            }
        }

        public DataSourceResult GetMortgageServiceReq(DataSourceRequest Req, int RID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var det = (from mor in dbContext.OnlineMortgageDetails
                           where mor.RID == RID
                           select new MortgageModel
                           {
                               BankName = mor.BankName,
                               //PreviousLoanNoc = mor.PreviousLoanNoc,
                               StrPreviousLoanNoc = mor.PreviousLoanNoc == 1 ? Constants.yes : Constants.no,
                               BranchAddress = mor.BranchAddress,
                               SanctionedAmount = mor.SanctionedAmount,
                               StrMortgageType = mor.MortgageType == "1" ? Constants.collateral : Constants.normal,
                           });
                return det.ToDataSourceResult(Req);
            }
        }

        public List<PropertyDocument> GetDocumentDetails(DataSourceRequest req, int rid, int id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from docs in dbContext.ServiceRequest_Documents
                            join det in dbContext.Customer_ServiceRequest on docs.ServiceReq_No equals det.Id
                            join chk in dbContext.ServiceCheckList_Master on docs.ChkId equals chk.ChkId
                            where det.Registration_No == rid.ToString()
                            select new PropertyDocument
                            {
                                RID = rid,
                                DocumentName = docs.DocumentPath,
                                DocumentType = chk.ChkName
                            });
                return data.ToList();
            }
        }

        public bool SaveRequestDetails(int id, decimal? serviceFee, decimal? duesAmnt, string comment, int temp, string dispatchNo, DateTime? dispatchDate)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var username = userInfo.UserID.ToString() + "-" + userInfo.FirstName + " " + userInfo.MiddleName + " " + userInfo.LastName + " Dated:- " + DateTime.Now.ToString("dd-MMM-yyyy");
                username = username.Trim();
                var record = (from serv in dbContext.Customer_ServiceRequest where serv.Id == id select serv).FirstOrDefault();
                if (record != null)
                {
                    record.ServiceFee = serviceFee;
                    record.DuesAmount = duesAmnt;
                    record.DispatchNumber = dispatchNo;
                    record.Comment = record.Comment + "\n" + username + "\n" + comment;
                    record.Modified_Date = DateTime.Now;
                    record.Modified_By = userInfo.UserID;
                    record.DispatchDate = dispatchDate;
                    //record.Request_Status = paymentPending;
                    record.Request_Status = temp;
                    dbContext.SaveChanges();

                    flag = true;

                    var detail = dbContext.Customer_ServiceRequest.Where(i => i.Id == id).FirstOrDefault();
                    var stats = dbContext.StatusMasters.Where(s => s.Id == detail.Request_Status).FirstOrDefault();
                    var service = dbContext.CitizenService_Master.Where(s => s.Deptt_Id == detail.DepartmentId && s.service_id == detail.ServiceId).FirstOrDefault();
                    //on mobile
                    if (userInfo.Mobile != null)
                    {
                        //var msg = "Dear User, Your service request " + service.ServiceName + " has been " + stats.Status + " . Regards, http://mynoida.in";
                        var msg = string.Format(NAMessages.ServiceReqStatusChange, service.ServiceName, stats.Status);
                        var mobNumber = userInfo.Mobile.ToString();
                        ApplicationHelper.SMSSend(mobNumber, msg);
                    }
                    if (userInfo.Email != null)
                    {
                        var body = "Dear User, Your service request " + service.ServiceName + " has been " + stats.Status + " . Regards, http://mynoida.in";
                        EmailHelper emailHelper = new EmailHelper();
                        emailHelper.Send(userInfo.Email, "Request submitted", body);
                    }
                    if (detail.MobileNumber != null)
                    {
                        //var msg = "Dear User, Your service request " + service.ServiceName + " has been " + stats.Status + " . Regards, http://mynoida.in";
                        var msg = string.Format(NAMessages.ServiceReqStatusChange, service.ServiceName, stats.Status);
                        var mobNumber = detail.MobileNumber.ToString();
                        ApplicationHelper.SMSSend(mobNumber, msg);
                    }
                    if (detail.Email != null)
                    {
                        var body = "Dear User, Your service request " + service.ServiceName + " has been " + stats.Status + " . Regards, http://mynoida.in";
                        EmailHelper emailHelper = new EmailHelper();
                        emailHelper.Send(detail.Email, "Request submitted", body);
                    }

                }
            }
            return flag;
        }

        public void SaveFileDetails(string filePath, string fileName, int id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var record = (from serv in dbContext.Customer_ServiceRequest where serv.Id == id select serv).FirstOrDefault();
                if (record != null)
                {
                    record.DispatchDocumentName = fileName;
                    record.Modified_Date = DateTime.Now;
                    record.Modified_By = userInfo.UserID;
                    dbContext.SaveChanges();
                }
            }
        }

        public ChallanModel GenerateChallanByServices(int rId, int bankId, int branchId, string accountNumber, int? deptId, int? serviceId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var challanIdPK = 0;
                List<BankAccountManagementModel> modelList = (List<BankAccountManagementModel>)HttpContext.Current.Session["TempModel"];
                if (modelList != null)
                {
                    Challan_Master challanMaster = new Challan_Master();
                    challanMaster.Rid = rId;
                    challanMaster.Bank_Id = bankId;
                    challanMaster.Branch_Id = branchId;
                    challanMaster.Account_Number = accountNumber;
                    //challanMaster.Content = challan;
                    challanMaster.Created_Date = DateTime.Now;
                    challanMaster.Generate_Date = DateTime.Now;
                    challanMaster.Created_By = userInfo.UserID;
                    challanMaster.Is_Active = true;
                    //challanMaster.ServiceRequestNo = serviceId;
                    dbContext.Challan_Master.Add(challanMaster);
                    dbContext.SaveChanges();

                    var challanIdPK = dbContext.Challan_Master.Max(m => m.Id);
                    //var challanId = dbContext.Challan_Master.Where(c => c.Id == challanIdPK).Select(i => i.Challan_Id).FirstOrDefault();
                    foreach (var model in modelList)
                    {
                        Challan_Trans trans = new Challan_Trans();
                        trans.Rid = rId;
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
                    var challanModel = (from challan in dbContext.Challan_Master
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
                                            ServiceRequestId = serviceId,
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
                    int count = 1;
                    foreach (var model in modelList)
                    {
                        if (count == 1)
                        {
                            challanModel.Amount1 = model.Amount;
                            challanModel.AccountHeadName1 = model.AccountHeadName;
                            challanModel.AccountSubHeadName1 = model.AccountHeadName;
                        }
                        if (count == 2)
                        {
                            challanModel.Amount2 = model.Amount;
                            challanModel.AccountHeadName2 = model.AccountHeadName;
                            challanModel.AccountSubHeadName2 = model.AccountHeadName;
                        }
                        if (count == 3)
                        {
                            challanModel.Amount3 = model.Amount;
                            challanModel.AccountHeadName3 = model.AccountHeadName;
                            challanModel.AccountSubHeadName3 = model.AccountHeadName;
                        }
                        count++;
                    }

                    int servid = (int)serviceId;
                    var servicesreqid = dbContext.Customer_ServiceRequest.Where(c => c.Registration_No == rId.ToString() && c.Id == servid).FirstOrDefault();
                    if (servicesreqid != null)
                    {
                        //Customer_ServiceRequest service = new Customer_ServiceRequest();
                        //servicesreqid.Registration_No = rId.ToString();
                        //servicesreqid.Property_No = challanModel.PropertyNumber;
                        //servicesreqid.DepartmentId = deptId;
                        //servicesreqid.ServiceId = serviceId;
                        //servicesreqid.Created_Date = DateTime.Now;
                        servicesreqid.DuesAmount = challanModel.TotalAmount;
                        servicesreqid.Request_Status = 1;
                        //service.ServiceFee = 1000;
                        servicesreqid.ChallanId = challanIdPK;// Convert.ToInt32();

                        dbContext.SaveChanges();
                    }

                    return challanModel;
                }
                else
                {
                    return null;
                }
            }
        }

        public List<DDList> GetAllServices()
        {
            var lst = new List<DDList>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lst = (from csm in dbContext.CitizenService_Master
                       //where csm
                       select new DDList
                       {
                           id = csm.Id,
                           text = csm.ServiceName
                       }).ToList();
                return lst;
            }
        }

        public List<ServiceRequestDocument> GetCheckListDocumentMentsByServiceId_DepartmentId(int ServiceRequestNo)
        {
            List<ServiceRequestDocument> CheckLstDocuments;
            try
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    CheckLstDocuments = dbContext.ServiceRequest_Documents.Where(m => m.ServiceReq_No == ServiceRequestNo).Select(d => new ServiceRequestDocument()
                    {
                        ChkDocumentId = d.SrvDocId,
                        ChkDocumentName = d.DocumentPath
                    }).ToList();

                }
                return CheckLstDocuments;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public CitizenServiceRequest GetServiceRequestDetails(int id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var details = (from det in dbContext.Customer_ServiceRequest
                               join csm in dbContext.CitizenService_Master on det.ServiceId equals csm.service_id
                               join stMaster in dbContext.StatusMasters on det.Request_Status equals stMaster.Id
                               where det.DepartmentId == csm.Deptt_Id && det.Id == id
                               select new CitizenServiceRequest
                {
                    Registration_No = det.Registration_No,
                    Id = det.Id,
                    ServiceName = csm.ServiceName,
                    Status = stMaster.Status,
                    Description = det.Description,
                    ServiceFee = det.ServiceFee,
                    DuesAmnt = det.DuesAmount,
                    Comment = det.Comment,
                    PaymentStatus = (det.PaymentStatus == 1) ? Constants.yes : Constants.no,
                    ServiceRequestId = det.Id,
                    ServiceId = det.ServiceId
                }).FirstOrDefault();
                //details.MortgageDetails = new MortgageModel();
                details.transDetails = new TransferRequestModel();
                //details.CICmodel = new CICModel();
                //details.mutationDetails = new MutationModel();
                //details.RentingModel = new RentingModel();
                //details.gdaModel = new GPAModel();
                return details;
            }
        }

        //To Save Service Request
        public int SaveServiceRequest(int rID, int department, int serviceType, string description)
        {
            var requestID = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                Customer_ServiceRequest service = new Customer_ServiceRequest();
                service.Registration_No = rID.ToString();
                service.DepartmentId = department;
                service.ServiceId = serviceType;
                service.Description = description;
                service.Created_Date = DateTime.Now;
                service.Request_Status = 1;
                dbContext.Customer_ServiceRequest.Add(service);
                dbContext.SaveChanges();
                requestID = service.Id;
            }
            return requestID;
        }


        public List<DDList> GetServiceRequestStatus()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var statusList = (from stat in dbContext.StatusMasters
                                  select new DDList
                                  {
                                      id = stat.Id,
                                      text = stat.Status
                                  }).ToList();
                return statusList;
            }
        }


        public DataSourceResult GetCustomerServiceReport(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from csr in dbContext.View_Service_report
                           select new ServiceReportModel
                           {
                               Id = csr.requestNo,
                               RegistrationNo = csr.Registration_No,
                               CreatedDate = csr.Created_Date.Value,
                               Description = csr.Description,
                               ApplicantName = csr.PRDVREGIST_APPLICANT_NAME,
                               Sector = csr.SECTOR,
                               Block = csr.BLOCK,
                               PropertyNo = csr.PLDIPROPERTY_NO,
                               ServiceName = csr.ServiceName,
                               Email = csr.Email,
                               MobileNo = csr.MobileNumber,
                               DepartmentName = csr.DepartmentId != 1 ? (csr.DepartmentId != 2 ? (csr.DepartmentId != 3 ? (csr.DepartmentId != 4 ? (csr.DepartmentId != 5 ? "Group Housing" : "Housing") : "Industrial") : "Residential") : "Commercial") : "Institutional"
                           }).Distinct();
                lst.GroupBy(g => g.RegistrationNo).Select(s => s.First());
                //var data = lst.ToDataSourceResult(request);
                //var data = "update edmx";
                return lst.ToDataSourceResult(request);
            }
        }
    }
}
