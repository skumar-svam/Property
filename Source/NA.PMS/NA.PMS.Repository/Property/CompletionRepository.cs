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
using NoidaAuthority.PMS.Common;
using System.Net;
using System.IO;
using System.Configuration;

namespace NA.PMS.Repository
{
    public class CompletionRepository : ICompletionRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();

        public CompletionRepository()
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
        /// For reading grid data on ManageCompletion screen from DB for Requestor
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public DataSourceResult GetCompletionData(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();
                var allCompletions = (from com in dbContext.Completion_Details //group mut by mut.Rid into groups select groups.OrderByDescending(p => p.Request_No).First()
                                      join appDetail in dbContext.ApplicationDetails on com.Rid equals appDetail.registrationId
                                      join allma in dbContext.AllotmentMasters on com.Rid equals allma.rid
                                      join spt in dbContext.SchemePropTrans on allma.propertyId equals spt.propertyId
                                      join depma in dbContext.DepartmentMsts on spt.departmentId equals depma.departmentId
                                      where com.Is_Active == true && loginUserDeptt.Contains(allma.departmentId) && depma.departmentName.ToLower().Trim() != Departmentenum.Housing.ToString().ToLower().Trim()
                                      //where com.Requested_By == userInfo.UserID
                                      select new PropertyCompletionModel
                                      {
                                          ReqNo = com.Id,
                                          RId = com.Rid,
                                          DepttName = depma.departmentName,
                                          SectorName = spt.SectorMst.sectorName,
                                          BlockName = spt.BlockMst.blockName,
                                          PropertyNo = spt.propertyNo,
                                          PropNo = com.Property_Number,
                                          //ReqDate = mut.Requested_Date,
                                          CompletionDate = com.Completion_Execution_date,
                                          ApplicantName = appDetail.firstName + " " + appDetail.lastName,
                                          SchemeId = spt.schemeId,
                                          DepttId = depma.departmentId,
                                          PropertyId = spt.propertyId

                                      });
                var distinct = from allMu in allCompletions group allMu by allMu.RId into groups select groups.OrderByDescending(p => p.ReqNo).FirstOrDefault();
                return distinct.ToDataSourceResult(req);
            }
        }

        /// <summary>
        /// Used for filling RID dropdown in Completion screens (from Succ_Mut_Trans table with Transfer of type "Approved" only) on the basis of login User's Departments
        /// </summary>
        /// <returns>List of RIDs</returns>
        //public List<DDList> GetRIDsForCompletion()
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var loginUserDeptt = (from userMst in dbContext.UmUserMasters
        //                              join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
        //                              where userMst.UserRefId == userInfo.UserID
        //                              select dept.DepartmentId).ToList();
        //        var lstRId = (from pos in dbContext.PossessionDetails
        //                      //join st in dbContext.StatusMasters on pos.Status equals st.Id
        //                      join am in dbContext.AllotmentMasters on pos.Rid equals am.rid
        //                      where pos.PossessionDate != null && loginUserDeptt.Contains(am.departmentId)
        //                      orderby pos.Id descending
        //                      select new DDList
        //                      {
        //                          id = pos.Rid.Value,
        //                          text = pos.Rid.ToString()
        //                      }).ToList();
        //        var distinctRIds = lstRId.GroupBy(x => x.id).Select(y => y.First()).ToList();
        //        return distinctRIds;
        //    }
        //}

        public PropertyCompletionModel GetApplicantDetailsByRId(int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from allotment in dbContext.AllotmentMasters
                              join registry in dbContext.RegistryDetails on allotment.rid equals registry.Rid
                              join possesionDetails in dbContext.PossessionDetails on allotment.rid equals possesionDetails.Rid
                              join appDetails in dbContext.ApplicationDetails on allotment.rid equals appDetails.registrationId
                              join prop in dbContext.SchemePropTrans on allotment.propertyId equals prop.propertyId
                              join floorMst in dbContext.FloorMsts on prop.floorId equals floorMst.floorId
                              join propMaster in dbContext.PropertyTypeMsts on prop.propertyTypeId equals propMaster.propertyTypeId
                              join schemeMaster in dbContext.SchemeMsts on prop.schemeId equals schemeMaster.schemeId
                              join dept in dbContext.DepartmentMsts on prop.departmentId equals dept.departmentId
                              join sec in dbContext.SectorMsts on prop.sectorId.Value equals sec.sectorId
                              join block in dbContext.BlockMsts on prop.blockId.Value equals block.blockId
                              //join proptype in dbContext.PropertyTypeMsts on prop.propertyTypeId equals proptype.propertyTypeId
                              where allotment.rid == rId
                              select new PropertyCompletionModel
                              {
                                  RId = allotment.rid,
                                  SchemeName = schemeMaster.schemeName,
                                  SchemeId = allotment.schemeId.Value,
                                  DepttName = dept.departmentName,
                                  DepttId = dept.departmentId,
                                  PropNo = sec.sectorName + "/" + block.blockName + "-" + prop.propertyNo,
                                  ApplicantName = appDetails.firstName + " " + appDetails.lastName,
                                  Gender = appDetails.gender,
                                  FatherOrHusbandName = appDetails.fatherHusbandName,
                                  //PropertyId = prop.propertyId,
                                  PropType = propMaster.propertyTypeName,
                                  Area = prop.totalArea.Value,
                                  Floor = floorMst.floorName,
                                  //LeaseDeedDueDate = registry.RegistryDueDate.Value,
                                  LeaseDeedDate = registry.RegistryDoneDate.Value,
                                  PossessionDate = possesionDetails.PossessionDate,
                                  BuildingPlanApproved = dbContext.Building_Plan_Master.Where(x => x.Rid == rId).Select(y => y.id).FirstOrDefault() == 0 ? BoolStatus.No.ToString() : BoolStatus.Yes.ToString(),
                                  //BuildingPlanSanctionDate = dbContext.Building_Plan_Master.Where(x => x.Rid == rId).Select(y => y.Sanction_Date).FirstOrDefault(),
                                  CurrentDateCompletion = DateTime.Now,
                                  Extension = dbContext.Extension_Details.Where(x => x.Rid == rId).Select(y => y.Id).FirstOrDefault() == 0 ? BoolStatus.No.ToString() : BoolStatus.No.ToString()
                              }).FirstOrDefault();

                return result;
            }
        }

        /// <summary>
        /// Used for fething Completion details by Request No. (erstwhile was used for pickinjg details by RID)
        /// </summary>
        /// <param name="reqNo">Request No.</param>
        /// <returns></returns>
        public PropertyCompletionModel GetCompletionDetailsByReqId(int reqNo)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from compDetail in dbContext.Completion_Details
                            join am in dbContext.AllotmentMasters on compDetail.Rid equals am.rid
                            join registry in dbContext.RegistryDetails on am.rid equals registry.Rid
                            join possesionDetails in dbContext.PossessionDetails on am.rid equals possesionDetails.Rid
                            join appDet in dbContext.ApplicationDetails on am.rid equals appDet.registrationId
                            join spt in dbContext.SchemePropTrans on am.propertyId equals spt.propertyId
                            join scm in dbContext.SchemeMsts on spt.schemeId equals scm.schemeId
                            join dm in dbContext.DepartmentMsts on am.departmentId equals dm.departmentId
                            join pm in dbContext.PropertyTypeMsts on spt.propertyTypeId equals pm.propertyTypeId
                            join sm in dbContext.SectorMsts on spt.sectorId.Value equals sm.sectorId
                            join bm in dbContext.BlockMsts on spt.blockId.Value equals bm.blockId
                            join fm in dbContext.FloorMsts on spt.floorId.Value equals fm.floorId
                            //join sta in dbContext.StatusMasters on mut.Status equals sta.Id
                            where compDetail.Id == reqNo
                            select new PropertyCompletionModel
                            {
                                RId = compDetail.Rid,
                                PropNo = sm.sectorName + "/" + bm.blockName + "-" + spt.propertyNo,
                                SchemeName = scm.schemeName,
                                DepttName = dm.departmentName,
                                ApplicantName = appDet.firstName + " " + appDet.lastName,
                                Gender = appDet.gender,
                                FatherOrHusbandName = appDet.fatherHusbandName,
                                PropType = pm.propertyTypeName,
                                Area = spt.totalArea.Value,
                                Floor = fm.floorName,
                                CompletionDate = compDetail.Completion_Execution_date,
                                CompletionType = compDetail.Completion_Type,
                                PartCompletionArea = compDetail.Part_Completion_Area,
                                PartCompletionPercentage = compDetail.Part_Completion_Percent,
                                CompletionExecutionDate = compDetail.Completion_Execution_date,
                                LeaseDeedDate = registry.RegistryDoneDate.Value,
                                PossessionDate = possesionDetails.PossessionDate,
                                CompletionDueDate = compDetail.Completion_Due_Date,
                                BuildingPlanApproved = dbContext.Building_Plan_Master.Where(x => x.Rid == compDetail.Rid).Select(y => y.id).FirstOrDefault() == 0 ? BoolStatus.No.ToString() : BoolStatus.Yes.ToString(),
                                CompletionCharges = compDetail.Completion_Charge,
                                Extension = dbContext.Extension_Details.Where(x => x.Rid == compDetail.Rid).Select(y => y.Id).FirstOrDefault() == 0 ? BoolStatus.No.ToString() : BoolStatus.No.ToString()
                            }).FirstOrDefault();
                return data;
            }
        }

        /// <summary>
        /// Check for Unique Request by Registration Id. There should be only one request for single Registration Id
        /// </summary>
        /// <param name="rId"></param>
        /// <returns></returns>
        public bool CheckForUniqueCompletionRequest(int rId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var compRecord = dbContext.Completion_Details.Where(compDetail => compDetail.Rid == rId && compDetail.Is_Active == true).FirstOrDefault();
                if (compRecord == null)
                    flag = false;
                else
                    flag = true;
            }
            return flag;
        }

        /// <summary>
        /// Saves new Completion Details in the DB  
        /// </summary>

        /// <param name="rId">RID</param>
        /// <returns></returns>
        public bool SaveCompletionDetails(DateTime completionDueDate, DateTime completionExecutionDate, int rId, string completionType, string propNo, Decimal? partCompletionArea, Decimal? partCompletionPercentage, Decimal? completionCharges, string extension, string viewName)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                PropertyCompletionModel objPropertyCompletionModel = new PropertyCompletionModel();

                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == rId select appDet).FirstOrDefault();
                var newCompletion = new Completion_Details();
                newCompletion.Rid = rId;
                newCompletion.Property_Number = propNo;
                newCompletion.Completion_Due_Date = completionDueDate;
                newCompletion.Completion_Execution_date = completionExecutionDate;
                newCompletion.Extension = extension;
                newCompletion.Completion_Type = completionType;
                newCompletion.Part_Completion_Percent = partCompletionPercentage;
                newCompletion.Part_Completion_Area = partCompletionArea;
                newCompletion.Completion_Charge = completionCharges;
                newCompletion.Is_Active = true;
                newCompletion.Created_By = userInfo.UserID;
                newCompletion.Created_Date = DateTime.Now;
                dbContext.Completion_Details.Add(newCompletion);
                dbContext.SaveChanges();
                flag = true;

                // Inserting in Audit table 

                var newObjPropertyCompletionModel = new PropertyCompletionModel
                {
                    RId = rId,
                    PropNo = propNo,
                    CompletionDueDate = completionDueDate,
                    CompletionExecutionDate = completionExecutionDate,
                    Extension = extension,
                    CompletionType = completionType,
                    PartCompletionArea = partCompletionArea,
                    PartCompletionPercentage = partCompletionPercentage,
                    CompletionCharges = completionCharges,
                    IsActive = true
                };
                GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.Completion_Details, viewName, newCompletion.Id, objPropertyCompletionModel, newObjPropertyCompletionModel, userInfo.UserID.ToString());

                //Send notifications
                //Email
                var body = "Dear User, Your Completion Request has been submitted. Regards, http://mynoida.in";
                EmailHelper emailHelper = new EmailHelper();
                emailHelper.Send(newOwner.tEmail, "Completion Request Status", body);
                //SMS
                //var msg = "Dear User, Your Completion Request has been submitted. Regards, http://mynoida.in";
                var msg = NAMessages.CompletionReqSubmit;
                //SMSSend(newOwner.tMobileNumber, msg);
                ApplicationHelper.SendSMS(newOwner.tMobileNumber, msg);
            }
            return flag;
        }

        /// <summary>
        /// Used for sending SMS using SMS Gateway
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <param name="msg"></param>
        private void SMSSend(string mobileNo, string msg)
        {
            WebClient client = new WebClient();
            //string baseurl = ConfigurationManager.AppSettings["SMSsend"].ToString() + ConfigurationManager.AppSettings["SMSUsername"].ToString() + "&password=" + ConfigurationManager.AppSettings["SMSPassword"].ToString() + "&sendername=" + "NETSMS" + "&mobileno=" + mobileNo + "&message=" + msg;
            string baseurl = ConfigurationManager.AppSettings["SMSApiUrl"].ToString() + "ApiKey=" + ConfigurationManager.AppSettings["SMSApiKey"].ToString() + "&ClientId=" + ConfigurationManager.AppSettings["SMSClientId"].ToString() + "&SenderId=" + ConfigurationManager.AppSettings["SMSSenderId"].ToString() + "&Message=" + msg + "&MobileNumbers=91" + mobileNo + "&Is_Unicode=" + ConfigurationManager.AppSettings["SMSIsUnicode"].ToString() + "&Is_Flash=" + ConfigurationManager.AppSettings["SMSIsFlash"].ToString();
            Stream data = client.OpenRead(baseurl);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            data.Close();
            reader.Close();
        }

        //public PropertyCompletionModel PrintCompletionChallan(int propertyId, int schemeID, int departmentId, int rId)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var lst = (from proptrans in dbContext.SchemePropTrans
        //                   join sec in dbContext.SectorMsts on proptrans.sectorId equals sec.sectorId
        //                   join block in dbContext.BlockMsts on proptrans.blockId equals block.blockId
        //                   join protype in dbContext.PropertyTypeMsts on proptrans.propertyTypeId equals protype.propertyTypeId
        //                   join schemeCost in dbContext.SchemeCostTrans on proptrans.schemeId equals schemeCost.schemeId
        //                   join schemeDeptt in dbContext.SchemeDepartmentTrans on proptrans.schemeId equals schemeDeptt.schemeId
        //                   join allotment in dbContext.AllotmentMasters on proptrans.propertyId equals allotment.propertyId
        //                   join completionDetail in dbContext.Completion_Details on allotment.rid equals completionDetail.Rid
        //                   where proptrans.propertyId == propertyId && proptrans.schemeId == schemeID && proptrans.departmentId == departmentId && proptrans.IsActive == true
        //                   && completionDetail.Is_Active == true
        //                   select new PropertyCompletionModel
        //                   {
        //                       BlockName = block.blockName,
        //                       SectorName = sec.sectorName,
        //                       PropertyTypeName = protype.propertyTypeName,
        //                       FormNo = allotment.formNo,
        //                       RId = allotment.rid,
        //                       CompletionCharges =  completionDetail.Completion_Charge

        //                   }).FirstOrDefault();

        //        ApplicationFormModel appdetails = new ApplicationFormModel();
        //        if (lst != null)
        //        {
        //            var appresult = dbContext.ApplicationDetails.FirstOrDefault(id => id.schemeId == schemeID && id.departmentId == departmentId && id.formNo == lst.FormNo);
        //            if (appresult != null)
        //            {
        //                appdetails.FirstName = appresult.firstName + " " + appresult.middleName + "" + appresult.lastName;
        //                appdetails.CorrespondingAddress = appresult.correspondanceAdd;
        //                appdetails.Email = appresult.email;
        //                appdetails.MobileNumber = appresult.mobileNumberP1;
        //            }
        //            lst.ApplicationForm = appdetails;
        //        }
        //        return lst;
        //    }

        //}

        // Get list of models to print possession orders
        public PropertyCompletionModel GetModelToPrintCompletionRequest(int rId)
        {
            var completionmodel = new PropertyCompletionModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                completionmodel = (from allotment in dbContext.AllotmentMasters
                                   join registry in dbContext.RegistryDetails on allotment.rid equals registry.Rid
                                   join appDetails in dbContext.ApplicationDetails on allotment.rid equals appDetails.registrationId
                                   join prop in dbContext.SchemePropTrans on allotment.propertyId equals prop.propertyId
                                   join floorMst in dbContext.FloorMsts on prop.floorId equals floorMst.floorId
                                   join propMaster in dbContext.PropertyTypeMsts on prop.propertyTypeId equals
                                       propMaster.propertyTypeId
                                   join schemeMaster in dbContext.SchemeMsts on prop.schemeId equals schemeMaster.schemeId
                                   join dept in dbContext.DepartmentMsts on prop.departmentId equals dept.departmentId
                                   join sec in dbContext.SectorMsts on prop.sectorId.Value equals sec.sectorId
                                   join block in dbContext.BlockMsts on prop.blockId.Value equals block.blockId
                                   //join proptype in dbContext.PropertyTypeMsts on prop.propertyTypeId equals proptype.propertyTypeId
                                   where rId == allotment.rid
                                   select new PropertyCompletionModel
                                  {
                                      RId = allotment.rid,
                                      SchemeName = schemeMaster.schemeName,
                                      SchemeId = allotment.schemeId.Value,
                                      DepttName = dept.departmentName,
                                      DepttId = dept.departmentId,
                                      PropertyNumber = sec.sectorName + "/" + block.blockName + "-" + prop.propertyNo,
                                      ApplicantName = appDetails.firstName + " " + appDetails.lastName,
                                      Gender = appDetails.gender,
                                      FatherOrHusbandName = appDetails.fatherHusbandName,
                                      PropertyId = prop.propertyId,
                                      PropertyType = propMaster.propertyTypeName,
                                      Area = prop.totalArea.Value,
                                      FloorAreaRange = floorMst.floorName,
                                      LeaseDeedDueDate = registry.RegistryDueDate.Value,
                                      LeaseDeedExecutionDate = registry.RegistryDoneDate.Value,
                                      //PossessionDueDate = registry.RegistryDueDate.Value.AddDays(60),

                                  }).FirstOrDefault();

            }
            return completionmodel;
        }

    }
}
