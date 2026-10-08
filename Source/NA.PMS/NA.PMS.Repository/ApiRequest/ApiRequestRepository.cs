using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Web.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Model.Entities;

namespace NA.PMS.Repository.ApiRequest
{
    public class ApiRequestRepository : IApiRequestRepository
    {
          // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();

        public ApiRequestRepository()
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
        /// Save Service Request
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <returns></returns>
        public bool SaveServiceRequest(Customer_ServiceRequest serviceRequest)
        {
            var result = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                dbContext.Customer_ServiceRequest.Add(serviceRequest);
                if (dbContext.SaveChanges() > 0)
                    result = true;
            }
            return result;
        }


        /// <summary>
        /// Get Service Request Status by serviceid
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public int GetServiceRequestStatus(int serviceId)
        {
            var result = 0;

            using (var dbContext = new NoidaPMSEntities())
            {
             var service =   dbContext.Customer_ServiceRequest.FirstOrDefault(sr => sr.ServiceId == serviceId);
             if (service != null)
                 result =  Convert.ToInt32(service.Request_Status);
            }

            return result;
        }



        /// <summary>
        /// Update Service Request Status
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <returns></returns>
        public bool UpdateServiceRequestStatus(int serId, string rType)
        {
            var result = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var service =
                    dbContext.Customer_ServiceRequest.FirstOrDefault(sr => sr.Id == serId);
                if (service != null)
                {
                    service.Request_Status = rType == "A" ? 1 : 2;

                    if (dbContext.SaveChanges() > 0)
                        result = true;
                }
            }
            return result;
        }


        public List<ServiceRequestModel> GetAllServiceRequests()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var departmentList = (from dept in dbContext.UmDepartmentMasters
                                      join udts in dbContext.UmUserDepartmentTrans on dept.DepartmentId equals udts.DepartmentId
                                      where udts.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();


                var result = (from sReq in dbContext.Customer_ServiceRequest
                              join alot in dbContext.AllotmentMasters on sReq.Registration_No equals alot.rid.ToString()
                              join serv in dbContext.CitizenService_Master on   sReq.ServiceId equals serv.service_id
                              where serv.Deptt_Id == sReq.DepartmentId && departmentList.Contains(alot.departmentId.Value)  
                              select new ServiceRequestModel
                                {
                                    Registration_No = sReq.Registration_No,
                                    DepartmentName = alot.DepartmentMst.departmentName,
                                    ServiceName = serv.ServiceName,
                                    Request_Status = sReq.Request_Status,
                                    Id = sReq.Id,
                                    Description = sReq.Description
                                }).ToList();

                return result;
            }
        }


        public DataSourceResult GetCustomerServiceRequestList(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var departmentList = (from dept in dbContext.UmDepartmentMasters
                                      join udts in dbContext.UmUserDepartmentTrans on dept.DepartmentId equals udts.DepartmentId
                                      where udts.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var result = (from srvc in dbContext.View_Service_report
                              where departmentList.Contains(srvc.DepartmentId.Value) //orderby srvc.requestNo descending
                              select new ServiceRequestModel
                              {
                                  Id = srvc.requestNo,
                                  ApplicantName = srvc.PRDVREGIST_APPLICANT_NAME,
                                  DepartmentName = dbContext.DepartmentMsts.Where(d=>d.departmentId==srvc.DepartmentId).Select(d=>d.departmentName).FirstOrDefault(),
                                  ServiceName = srvc.ServiceName,
                                  Description = srvc.Description,
                                  Request_Status = srvc.Request_Status,
                                  Status = dbContext.StatusMasters.Where(s=>s.Id==srvc.Request_Status).Select(r=>r.Status).FirstOrDefault(),
                                  Created_Date = srvc.Created_Date
                              });
                result.OrderByDescending(o => o.Id);
                return result.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetCustomerServiceRequestDetailByRequestId(int? requestId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {               
                var result = (from srvc in dbContext.View_Service_report
                              where srvc.requestNo == requestId
                              select new ServiceRequestModel
                              {
                                  Id = srvc.requestNo,
                                  Registration_No = srvc.Registration_No,
                                  Property_No = srvc.SECTOR+"/"+srvc.BLOCK+"-"+srvc.PLDIPROPERTY_NO,
                                  Mobile = srvc.MobileNumber,
                                  Email = srvc.Email,
                                  Requestor = srvc.RequestorName,
                                  Comment = srvc.Comment,
                                  ApplicantAddress = srvc.PRDVREGIST_APPLICANT_ADDRESS
                              });
                DataSourceRequest request = new DataSourceRequest();
                return result.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetLetterHistoryDetails(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var departmentList = (from dept in dbContext.UmDepartmentMasters
                                      join udts in dbContext.UmUserDepartmentTrans on dept.DepartmentId equals udts.DepartmentId
                                      where udts.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var result = (from lettr in dbContext.Letter_History
                              join allot in dbContext.AllotmentMasters on lettr.Rid equals allot.rid
                              join tmplt in dbContext.TemplateMasters on (lettr.Department_Id+lettr.Template_Id) equals (tmplt.departmentId+tmplt.templateId)
                              where departmentList.Contains(lettr.Department_Id.Value) && lettr.Is_Active == true  //orderby srvc.requestNo descending
                              select new LetterViewModel
                              {
                                  Id = lettr.Id,
                                  Rid = lettr.Rid,
                                  Applicant = allot.ApplicationDetail.tFirstName,
                                  DepartmentId = lettr.Department_Id,
                                  Department = allot.DepartmentMst.departmentName,
                                  LetterId = lettr.Template_Id,
                                  LetterType = tmplt.templateName,
                                  CreatedDate = lettr.Created_Date,
                                  LetterDate = lettr.Generate_Date,
                              });
                result.OrderByDescending(o => o.Id);
                return result.ToDataSourceResult(request);
            }
        }


        public LetterViewModel GetLetterByBarcode(string barcode)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var rd = Convert.ToInt32(barcode);

                var departmentList = (from dept in dbContext.UmDepartmentMasters
                                      join udts in dbContext.UmUserDepartmentTrans on dept.DepartmentId equals udts.DepartmentId
                                      where udts.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var result = (from lettr in dbContext.Letter_History
                              join allot in dbContext.AllotmentMasters on lettr.Rid equals allot.rid
                              join propt in dbContext.SchemePropTrans on allot.propertyId equals propt.propertyId
                              join tmplt in dbContext.TemplateMasters on (lettr.Department_Id + lettr.Template_Id) equals (tmplt.departmentId + tmplt.templateId)
                              where lettr.Barcode_Val==barcode  //orderby srvc.requestNo descending
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
    }
}
