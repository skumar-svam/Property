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
using System.Data.Entity;
using NA.PMS.Model.Property;
using NA.PMS.Dal.DBConnection;
using Dapper;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using System.Collections;
using System.Configuration;

namespace NA.PMS.Repository.Reports
{
    public class ReportRepository : IReportRepository
    {
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public ReportRepository()
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
                            DepartmentList = dbContext.UmUserDepartmentTrans.Where(d => d.UserRefId == userInfo.UserID && d.Status == true).Select(d => d.DepartmentId).ToList();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Kendo read function for GPA Report
        /// </summary>
        /// <param name="req">Kendo internal parameter</param>
        /// <param name="fromSearch">From Date</param>
        /// <param name="toSearch">To Date</param>
        /// <param name="depttId">Department ID</param>
        /// <returns></returns>
        public DataSourceResult GetGPAReportData(DataSourceRequest req, DateTime? fromSearch, DateTime? toSearch, int? depttId)
        {
            int userid = userInfo.UserID;
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userid
                                      select dept.DepartmentId).ToList();
                var allGPA = (from gpa in dbContext.GPAs
                              join alloMas in dbContext.AllotmentMasters on gpa.Rid equals alloMas.rid
                              join spt in dbContext.SchemePropTrans on alloMas.propertyId equals spt.propertyId
                              join dept in dbContext.DepartmentMsts on alloMas.departmentId equals dept.departmentId
                              join secma in dbContext.SectorMsts on spt.sectorId equals secma.sectorId
                              join bloma in dbContext.BlockMsts on spt.blockId equals bloma.blockId
                              where gpa.Is_Active == true && alloMas.isActive == 1 && loginUserDeptt.Contains(spt.departmentId)
                              && (depttId == null || spt.departmentId == depttId)
                              && (fromSearch == null || DbFunctions.TruncateTime(fromSearch) <= DbFunctions.TruncateTime(gpa.Acceptance_date))
                              && (toSearch == null || DbFunctions.TruncateTime(toSearch) >= DbFunctions.TruncateTime(gpa.Acceptance_date))
                              select new GPAModel
                              {
                                  RId = gpa.Rid,
                                  GPAId = gpa.Id,
                                  DepttName = dept.departmentName,
                                  PropNo = secma.sectorName + "/" + bloma.blockName + "-" + spt.propertyNo,
                                  EffectiveFrom = gpa.Effcetd_From,
                                  EffectiveTo = gpa.Effected_To,
                                  IsActive = gpa.Is_Active,
                                  GPAType = Common.GPAType.GPA.ToString(),
                                  CreatedDate = gpa.Created_Date
                              });
                var rslt = (from g in allGPA orderby g.CreatedDate select g);
                return rslt.ToDataSourceResult(req);
            }
        }

        /// <summary>
        /// Kendo read function for Nominee Report
        /// </summary>
        /// <param name="req">Kendo internal parameter</param>
        /// <param name="fromSearch">From Date</param>
        /// <param name="toSearch">To Date</param>
        /// <param name="depttId">Department ID</param>
        /// <returns></returns>
        public DataSourceResult GetNomineeReportData(DataSourceRequest req, DateTime? fromSearch, DateTime? toSearch, int? depttId)
        {
            int userid = userInfo.UserID;
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userid
                                      select dept.DepartmentId).ToList();
                var nominees = (from nom in dbContext.Nominee_Details
                                join alloMas in dbContext.AllotmentMasters on nom.Rid equals alloMas.rid
                                join spt in dbContext.SchemePropTrans on alloMas.propertyId equals spt.propertyId
                                join dept in dbContext.DepartmentMsts on alloMas.departmentId equals dept.departmentId
                                join secma in dbContext.SectorMsts on spt.sectorId equals secma.sectorId
                                join bloma in dbContext.BlockMsts on spt.blockId equals bloma.blockId
                                where nom.Is_Active == 1 && alloMas.isActive == 1 && loginUserDeptt.Contains(spt.departmentId)
                                && (depttId == null || spt.departmentId == depttId)
                                && (fromSearch == null || DbFunctions.TruncateTime(fromSearch) <= DbFunctions.TruncateTime(nom.Nomination_Date))
                                && (toSearch == null || DbFunctions.TruncateTime(toSearch) >= DbFunctions.TruncateTime(nom.Nomination_Date))
                                select new GPAModel
                                {
                                    RId = nom.Rid,
                                    GPAId = nom.Id,
                                    NomineeId = nom.Id,
                                    NomineeName = nom.Nominee_Name,
                                    NominationDate = nom.Nomination_Date,
                                    RelationName = nom.Relation,
                                    GPAType = Common.GPAType.Nominee.ToString(),
                                    DepttName = dept.departmentName,
                                    PropNo = secma.sectorName + "/" + bloma.blockName + "-" + spt.propertyNo,
                                    CreatedDate = nom.Created_Date
                                });
                var distinctNominees = from allNo in nominees group allNo by allNo.RId into groups select groups.OrderByDescending(p => p.NomineeId).FirstOrDefault();
                var rslt = (from g in distinctNominees orderby g.CreatedDate select g);
                return rslt.ToDataSourceResult(req);
            }
        }

        public DataSourceResult GetTransferReportData(DataSourceRequest req, DateTime? fromSearch, DateTime? toSearch, int? schemeId, int? depttId, int? transType, int? transSubType)
        {
            int userid = userInfo.UserID;
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userid
                                      select dept.DepartmentId).ToList();
                var allTransfers = (from trans in dbContext.Succ_Mut_Trans
                                    join allma in dbContext.AllotmentMasters on trans.Rid equals allma.rid
                                    join spt in dbContext.SchemePropTrans on allma.propertyId equals spt.propertyId
                                    join depma in dbContext.DepartmentMsts on spt.departmentId equals depma.departmentId
                                    join secma in dbContext.SectorMsts on spt.sectorId equals secma.sectorId
                                    join bloma in dbContext.BlockMsts on spt.blockId equals bloma.blockId
                                    join statusMas in dbContext.StatusMasters on trans.Status equals statusMas.Id
                                    join users in dbContext.UmUserMasters on trans.Approved_By equals users.UserRefId
                                    where trans.Type.ToLower() == TransferType.T.ToString().ToLower() && loginUserDeptt.Contains(allma.departmentId) && allma.isActive == 1
                                    && (depttId == null || spt.departmentId == depttId)
                                    //&& (fromSearch == null || DbFunctions.TruncateTime(fromSearch) <= DbFunctions.TruncateTime(nom.Nomination_Date))
                                    //&& (toSearch == null || DbFunctions.TruncateTime(toSearch) >= DbFunctions.TruncateTime(nom.Nomination_Date))
                                    select new TransferModel
                                    {
                                        ReqNo = trans.Request_No,
                                        RId = trans.Rid,
                                        DepttName = depma.departmentName,
                                        PropNo = secma.sectorName + "/" + bloma.blockName + "-" + spt.propertyNo,
                                        ReqDate = trans.Requested_Date,
                                        ApprovedDate = trans.Approved_Date,
                                        AssignedTo = users.FirstName + " " + users.MiddleName + " " + users.LastName,
                                        Status = statusMas.Status
                                    });
                var distinct = from allMu in allTransfers group allMu by allMu.RId into groups select groups.OrderByDescending(p => p.ReqNo).FirstOrDefault();
                return distinct.ToDataSourceResult(req);
            }
        }

        #region PropertyReport
        public DataSourceResult GetPropertiesForReport(DataSourceRequest request, int? deptId, string propBank, string schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var allottedOrInProgressProp = dbContext.AllotmentMasters.Where(y => y.isStatus == AllotmentStatus.Approved.ToString() || y.isStatus == AllotmentStatus.InProgress.ToString()).Select(x => x.propertyId);

                var propDetail = new List<PropertyModel>();

                propDetail = (from property in dbContext.SchemePropTrans
                              where
                              property.IsActive == true && property.propertyId != null && !allottedOrInProgressProp.Contains(property.propertyId.Value)
                              && loginUserDeptt.Contains(property.departmentId)
                              && (deptId == null || property.departmentId == deptId)
                              && (schemeId == null || schemeId == "" || property.schemeId.ToString().Trim() == schemeId.Trim())
                              select new PropertyModel
                              {
                                  schemeId = property.schemeId,
                                  schemeName = property.SchemeMst.schemeName,
                                  propertyType = property.PropertyTypeMst.propertyTypeName,
                                  sectorId = property.SectorMst.sectorId,
                                  sectorName = property.SectorMst.sectorName,
                                  blockName = property.BlockMst.blockName,
                                  departmentName = property.DepartmentMst.departmentName,
                                  refId = property.refId,
                                  totalArea = property.totalArea,
                                  RegistryName = property.Registry,
                                  SchemeStatus = property.SchemeMst.Status,
                                  PropertyNo = property.propertyId,
                                  IsAllotted = dbContext.AllotmentMasters.Where(p => p.propertyId == property.propertyId && p.isStatus == AllotmentStatus.Approved.ToString()
                                                                                         ).Select(p => p.rid).FirstOrDefault()

                              }).ToList();

                if (propBank.Trim() == "1")
                {
                    propDetail = propDetail.Where(x => x.schemeId == null).ToList();
                }
                else if (propBank.Trim() == "0")
                {
                    propDetail = propDetail.Where(x => x.schemeId != null).ToList();
                }

                //propDetail = propDetail.Contains(allottedOrInProgressProp);

                return propDetail.ToDataSourceResult(request);
            }
        }
        public List<DDList> GetPropertyBank()
        {
            List<DDList> maritialStatus = new List<DDList>();
            foreach (int value in Enum.GetValues(typeof(PropertyBank)))
            {
                maritialStatus.Add(new DDList
                {
                    text = Enum.GetName(typeof(PropertyBank), value),
                    id = value
                });
            }
            return maritialStatus;
        }
        #endregion End PropertyReport

        #region Functional Report
        /// <summary>
        /// get report about property, functional/non-functional
        /// </summary>
        /// <returns></returns>
        public DataSourceResult GetPropertyFuctinalReport(DataSourceRequest request, string isFunctional)
        {
            Nullable<bool> yesorno = false;
            if (isFunctional == FunctionalStatus.Functional.ToString())
            {
                yesorno = true;
            }
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var functionalProperty = (from functional in dbContext.FunctionalDetails
                                          join status in dbContext.StatusMasters on functional.StatusId equals status.Id
                                          join allotment in dbContext.AllotmentMasters on functional.Rid equals allotment.rid
                                          //where functional.IsActive == true && allotment.isActive == 1 && loginUserDeptt.Contains(allotment.departmentId)
                                          where loginUserDeptt.Contains(allotment.departmentId) && (isFunctional == "" || functional.Functional == yesorno)
                                          select new FunctionalModel
                                          {
                                              RequestNo = functional.RequestNo,
                                              RId = functional.Rid,
                                              ApproveDate = functional.ApproveDate,
                                              CreatedDate = functional.CreatedDate,
                                              PropertyNumber = functional.PropertyNumber,
                                              Status = status.Status,
                                              FirstName = allotment.ApplicationDetail.tFirstName,
                                              MiddleName = allotment.ApplicationDetail.tMiddleName,
                                              LastName = allotment.ApplicationDetail.tLastName,
                                              DepttName = allotment.DepartmentMst.departmentName,
                                              SchemeName = allotment.SchemeMst.schemeName,
                                              IsFunctional = functional.Functional
                                          });


                return functionalProperty.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetSearchedFunctionalReport(DataSourceRequest request, string functionalDetail, string department, string scheme, string sector, DateTime? startDate, DateTime? endDate)
        {
            Nullable<bool> yesorno = false;
            int dept = 0;
            if (department != "")
            {
                dept = Convert.ToInt32(department);
            }
            if (functionalDetail == FunctionalStatus.Functional.ToString())
            {
                yesorno = true;
            }
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDepartment = (from userMaster in dbContext.UmUserMasters
                                           join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                           where userMaster.UserRefId == userInfo.UserID
                                           select departmentTrans.DepartmentId).ToList();

                var functionalProperty = (from functional in dbContext.FunctionalDetails
                                          join status in dbContext.StatusMasters on functional.StatusId equals status.Id
                                          join allotment in dbContext.AllotmentMasters on functional.Rid equals allotment.rid
                                          join property in dbContext.SchemePropTrans on allotment.propertyId equals property.propertyId
                                          where loginUserDepartment.Contains(allotment.departmentId)
                                                && (functionalDetail == "" || functional.Functional == yesorno)
                                                && (department == "" || allotment.departmentId == dept)
                                                && (scheme == "" || allotment.SchemeMst.schemeName == scheme)
                                                && (sector == "" || property.SectorMst.sectorName == sector)
                                                && (startDate == null || DbFunctions.TruncateTime(functional.ApproveDate) >= DbFunctions.TruncateTime(startDate))
                                                && (endDate == null || DbFunctions.TruncateTime(functional.ApproveDate) <= DbFunctions.TruncateTime(endDate))
                                          select new FunctionalModel
                                          {
                                              RequestNo = functional.RequestNo,
                                              RId = functional.Rid,
                                              CreatedDate = functional.CreatedDate,
                                              ApproveDate = functional.ApproveDate,
                                              FunctionalDate = functional.FunctionalDate,
                                              //PropertyNumber = functional.PropertyNumber,
                                              PropertyNumber = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo,
                                              Status = status.Status,
                                              FirstName = allotment.ApplicationDetail.tFirstName,
                                              MiddleName = allotment.ApplicationDetail.tMiddleName,
                                              LastName = allotment.ApplicationDetail.tLastName,
                                              DepttName = allotment.DepartmentMst.departmentName,
                                              SchemeName = allotment.SchemeMst.schemeName,
                                              IsFunctional = functional.Functional
                                          });


                return functionalProperty.ToDataSourceResult(request);
            }
        }
        #endregion End Functional Report region


        public DataSourceResult GetMortgageReports(DataSourceRequest request, DateTime? fromDate = null, DateTime? toDate = null, int? schemeId = null, int? departmentId = null)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var lstRId = (from mor in dbContext.MortgageDetails
                              join allot in dbContext.AllotmentMasters on mor.RID equals allot.rid
                              join prop in dbContext.SchemePropTrans on allot.propertyId equals prop.propertyId
                              join applicant in dbContext.ApplicationDetails on allot.rid equals applicant.registrationId
                              join sche in dbContext.SchemeMsts on applicant.schemeId equals sche.schemeId
                              join deptt in dbContext.DepartmentMsts on applicant.departmentId equals deptt.departmentId
                              join status in dbContext.StatusMasters on mor.StatusId equals status.Id
                              where loginUserDeptt.Contains(applicant.departmentId) && sche.Status != Constants.SchemeClosed && sche.IsActive == true
                              && (schemeId == null || allot.schemeId == schemeId)
                              && (departmentId == null || allot.departmentId == departmentId)
                              && (fromDate == null || DbFunctions.TruncateTime(mor.ApproveDate) >= DbFunctions.TruncateTime(fromDate))
                              && (toDate == null || DbFunctions.TruncateTime(mor.ApproveDate) <= DbFunctions.TruncateTime(toDate))
                              select new MortgageModel
                              {
                                  RequestNo = mor.RequestNo,
                                  RID = mor.RID,
                                  SchemeName = sche.schemeName,
                                  DepartmentName = deptt.departmentName,
                                  StatusName = status.Status,
                                  ApplicantName = applicant.tFirstName + " " + applicant.tMiddleName + " " + applicant.tLastName,
                                  MortgageDate = mor.MortgageDate,
                                  MortgageType = mor.MortgageType == "1" ? MortgageType.Collateral.ToString() : MortgageType.Normal.ToString(),
                                  ApproveDate = mor.ApproveDate,
                                  PropertyNumber = prop.SectorMst.sectorName + "/" + prop.BlockMst.blockName + "-" + prop.propertyNo
                              });
                return lstRId.ToDataSourceResult(request);
            }
        }


        public DataSourceResult SearchPossessionReport(DataSourceRequest request, string department, string scheme, string possession, string areaChange, string sector, DateTime? startDate, DateTime? endDate)
        {
            int dept = 0; bool flag = false; string YesOrNo = "";
            if (department != "")
            {
                dept = Convert.ToInt32(department);
            }
            if (possession == PossessionStatus.Ordered.ToString())
            {
                possession = "";
            }
            if (possession == PossessionStatus.Released.ToString())
            {
                flag = true;
            }
            if (possession == PossessionStatus.Due.ToString())
            {
                flag = false;
            }
            if (areaChange == AreaChange.Change.ToString() || areaChange == AreaChange.Increase.ToString() || areaChange == AreaChange.Decrease.ToString())
            {
                YesOrNo = "Yes";
            }
            if (areaChange == AreaChange.NoChange.ToString())
            {
                YesOrNo = "No";
            }
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDepartment = (from userMaster in dbContext.UmUserMasters
                                           join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                           where userMaster.UserRefId == userInfo.UserID
                                           select departmentTrans.DepartmentId).ToList();
                var possessionReport = (from possessionDetail in dbContext.PossessionDetails
                                        join allotment in dbContext.AllotmentMasters on possessionDetail.Rid equals allotment.rid
                                        join property in dbContext.SchemePropTrans on allotment.propertyId equals property.propertyId
                                        where loginUserDepartment.Contains(allotment.departmentId)
                                              && (possession == "" || possessionDetail.Possession == flag)
                                              && (areaChange == "" || possessionDetail.AreaChange == YesOrNo)
                                              && (department == "" || allotment.departmentId == dept)
                                              && (scheme == "" || allotment.SchemeMst.schemeName == scheme)
                                              && (sector == "" || property.SectorMst.sectorName == sector)
                                              && (startDate == null || DbFunctions.TruncateTime(possessionDetail.PossessionDate) >= DbFunctions.TruncateTime(startDate))
                                              && (endDate == null || DbFunctions.TruncateTime(possessionDetail.PossessionDate) <= DbFunctions.TruncateTime(endDate))
                                        select new PropertyPossessionModel
                                        {
                                            Id = possessionDetail.Id,
                                            Rid = possessionDetail.Rid,
                                            PropertyId = property.propertyId,
                                            DepartmentName = allotment.DepartmentMst.departmentName,
                                            PropertyNumber = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo,
                                            PossessionOrderDate = possessionDetail.PossessionOrderDate,
                                            PossessionDueDate = possessionDetail.PossessionDueDate,
                                            PossessionDate = possessionDetail.PossessionDate,
                                            AreaChange = possessionDetail.AreaChange,
                                            AreaChangeType = possessionDetail.AreaChangeType,
                                            FirstName = allotment.ApplicationDetail.tFirstName,
                                            MiddleName = allotment.ApplicationDetail.tMiddleName,
                                            LastName = allotment.ApplicationDetail.tLastName,
                                            Print = true
                                        });
                return possessionReport.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetPossessionReport(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from possession in dbContext.PossessionDetails
                              join allotment in dbContext.AllotmentMasters on possession.Rid equals allotment.rid
                              join property in dbContext.SchemePropTrans on allotment.propertyId equals property.propertyId
                              select new PropertyPossessionModel
                              {
                                  Id = possession.Id,
                                  Rid = possession.Rid,
                                  PropertyId = property.propertyId,
                                  DepartmentName = allotment.DepartmentMst.departmentName,
                                  PropertyNumber = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo,
                                  PossessionOrderDate = possession.PossessionOrderDate,
                                  PossessionDueDate = possession.PossessionDueDate,
                                  PossessionDate = possession.PossessionDate,
                                  AreaChange = possession.AreaChange,
                                  FirstName = allotment.ApplicationDetail.tFirstName,
                                  MiddleName = allotment.ApplicationDetail.tMiddleName,
                                  LastName = allotment.ApplicationDetail.tLastName,
                                  AreaChangeType = possession.AreaChangeType,
                                  Print = true
                              });
                return result.ToDataSourceResult(request);
            }
        }


        public DataSourceResult SearchCompletionReportData(DataSourceRequest request, string completion, string department, string scheme, string sector, DateTime? startDate, DateTime? endDate)
        {
            var departmentId = 0; var fullOrPartial = "";
            if (department != "")
            {
                departmentId = Convert.ToInt32(department);
            }
            if (completion != "")
            {
                if (completion == BuildingCompletion.Completed.ToString())
                {
                    fullOrPartial = "Full";
                }
                if (completion == BuildingCompletion.PartialCompleted.ToString())
                {
                    fullOrPartial = "Partial";
                }
                if (completion == BuildingCompletion.NotCompleted.ToString())
                {
                    fullOrPartial = "Not Completed";
                }
            }

            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDepartment = (from userMaster in dbContext.UmUserMasters
                                           join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                           where userMaster.UserRefId == userInfo.UserID
                                           select departmentTrans.DepartmentId).ToList();
                var completionData = (from completionDetail in dbContext.Completion_Details
                                      join allotment in dbContext.AllotmentMasters on completionDetail.Rid equals allotment.rid
                                      join property in dbContext.SchemePropTrans on allotment.propertyId equals property.propertyId
                                      where loginUserDepartment.Contains(allotment.departmentId)
                                                && (completion == "" || completionDetail.Completion_Type == fullOrPartial)
                                                && (department == "" || allotment.departmentId == departmentId)
                                                && (scheme == "" || allotment.SchemeMst.schemeName == scheme)
                                                && (sector == "" || property.SectorMst.sectorName == sector)
                                                && (startDate == null || DbFunctions.TruncateTime(completionDetail.Completion_Execution_date) >= DbFunctions.TruncateTime(startDate))
                                                && (endDate == null || DbFunctions.TruncateTime(completionDetail.Completion_Execution_date) <= DbFunctions.TruncateTime(endDate))
                                      select new PropertyCompletionModel
                                      {
                                          ReqNo = completionDetail.Id,
                                          RId = completionDetail.Rid,
                                          DepttName = allotment.DepartmentMst.departmentName,
                                          PropNo = completionDetail.Property_Number,
                                          CompletionDate = completionDetail.Completion_Execution_date,
                                          SchemeId = allotment.schemeId,
                                          DepttId = allotment.departmentId.Value,
                                          PropertyId = allotment.propertyId,
                                          FirstName = allotment.ApplicationDetail.tFirstName,
                                          MiddleName = allotment.ApplicationDetail.tMiddleName,
                                          LastName = allotment.ApplicationDetail.tLastName
                                      }).OrderByDescending(r => r.ReqNo);

                return completionData.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetCustomerServiceReport(DataSourceRequest request, int? departmentId, DateTime? startDate, DateTime? endDate, int? status)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDepartment = (from userMaster in dbContext.UmUserMasters
                                           join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                           where userMaster.UserRefId == userInfo.UserID
                                           select departmentTrans.DepartmentId).ToList();

                var lst = (from csr in dbContext.View_Service_report
                           where loginUserDepartment.Contains(csr.DepartmentId)
                           && (departmentId == null || csr.DepartmentId == departmentId)
                           && (status == null || csr.Request_Status == status)
                           && (startDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(startDate))
                           && (endDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(endDate))
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
                               Status = dbContext.StatusMasters.Where(s => s.Id == csr.Request_Status).Select(s => s.Status).FirstOrDefault(),
                               DepartmentName = dbContext.DepartmentMsts.Where(d => d.departmentId == csr.DepartmentId).Select(d => d.departmentName).FirstOrDefault(),
                               SubDepartment = csr.SubDepartment
                               //DepartmentName = csr.DepartmentId != 1 ? (csr.DepartmentId != 2 ? (csr.DepartmentId != 3 ? (csr.DepartmentId != 4 ? (csr.DepartmentId != 5 ? "Group Housing" : "Housing") : "Industrial") : "Residential") : "Commercial") : "Institutional"
                           }).Distinct();
                lst.GroupBy(g => g.RegistrationNo).Select(s => s.First());
                //var data = lst.ToDataSourceResult(request);
                //var data = "update edmx";
                return lst.ToDataSourceResult(request);
            }
        }

        public string GetDashboardGraph(int? ReqType, int? UserDept)
        {
            string DashboardGraph = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                DashboardGraph = dbContext.usp_getCitizenGraph(ReqType, UserDept).FirstOrDefault();
            }
            return DashboardGraph;
        }

        public string GetServiceRequestMatrix(int? departmentid, int? serviceId, DateTime? FromDate, DateTime? ToDate)
        {
            string ServiceRequestMatrix = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (FromDate == null) { FromDate = DateTime.Now.AddDays(-8); }
                if (ToDate == null) { ToDate = DateTime.Now; }
                ServiceRequestMatrix = dbContext.usp_GetServiceRequestMatrix(departmentid, serviceId, FromDate, ToDate).FirstOrDefault();
            }
            return ServiceRequestMatrix;
        }

        public DataSourceResult GetUserWiseRequest(DataSourceRequest Req, UserWiseRequest objUserWiseRequest)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (objUserWiseRequest.DepartmentId > 0 && objUserWiseRequest.ServiceId > 0 && objUserWiseRequest.RequestId > 0)
                {
                    DateTime? startDate = null;
                    DateTime? endDate = null;
                    if (!string.IsNullOrEmpty(objUserWiseRequest.FromDate))
                    {
                        startDate = Convert.ToDateTime(objUserWiseRequest.FromDate);
                    }
                    if (!string.IsNullOrEmpty(objUserWiseRequest.ToDate))
                    {
                        endDate = Convert.ToDateTime(objUserWiseRequest.ToDate);
                    }
                    var Request = (from vsr in dbContext.View_Service_report
                                   join csr in dbContext.Customer_ServiceRequest on vsr.requestNo equals csr.Id into ViewCustomer
                                   from x in ViewCustomer.DefaultIfEmpty()
                                   join um in dbContext.UmUserMasters on x.Modified_By equals um.UserRefId into ViewUser
                                   from y in ViewUser.DefaultIfEmpty()
                                   where x.IsActive == true && vsr.DepartmentId == objUserWiseRequest.DepartmentId
                             && vsr.ServiceId == objUserWiseRequest.ServiceId
                             && vsr.Request_Status == objUserWiseRequest.RequestId
                              && (startDate == null || DbFunctions.TruncateTime(vsr.Created_Date) >= DbFunctions.TruncateTime(startDate))
                              && (endDate == null || DbFunctions.TruncateTime(vsr.Created_Date) <= DbFunctions.TruncateTime(endDate))
                                   // && x.Modified_By != null
                                   group new { x, y } by new { x.DepartmentId, x.ServiceId, x.Request_Status, y.UserName, y.FirstName, y.MiddleName, y.LastName } into UserReq
                                   select new UserWiseRequest
                                   {
                                       TotalRequest = UserReq.Count(),
                                       UserName = UserReq.Key.UserName != null ? UserReq.Key.UserName : "--",
                                       Name = (UserReq.Key.FirstName + " " + (!string.IsNullOrEmpty(UserReq.Key.MiddleName) ? UserReq.Key.MiddleName + " " : "") + UserReq.Key.LastName) != string.Empty ? (UserReq.Key.FirstName + " " + (!string.IsNullOrEmpty(UserReq.Key.MiddleName) ? UserReq.Key.MiddleName + " " : "") + UserReq.Key.LastName) : "--"
                                   });
                    return Request.ToDataSourceResult(Req);
                }
                else { return null; }
            }
        }

        public DataSourceResult GetPendencyReport(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDepartment = (from userMaster in dbContext.UmUserMasters
                                           join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                           where userMaster.UserRefId == userInfo.UserID
                                           select departmentTrans.DepartmentId).ToList();

                var lst = (from vapr in dbContext.view_approval_pending_request
                           where loginUserDepartment.Contains(vapr.departmentid) && vapr.Approved_By == userInfo.UserID && vapr.Status == 5//Only show Inprogress Request
                           select new PendencyReport
                           {
                               Request_No = vapr.Request_No,
                               rid = vapr.rid,
                               PagePath = vapr.PagePath,
                               departmentid = vapr.departmentid,
                               departmentName = vapr.departmentName,
                               Sector = vapr.Sector,
                               Block = vapr.Block,
                               propertyNo = vapr.propertyNo,
                               Requested_By = vapr.Requested_By,
                               requestedname = vapr.requestedname,
                               Requested_Date = vapr.Requested_Date,
                               Status = vapr.Status,
                               Approved_By = vapr.Approved_By,
                               ApprovedName = vapr.ApprovedName,
                               Approved_Date = vapr.Approved_Date,
                               requestType = vapr.requestType,
                               strStatus = dbContext.StatusMasters.Where(s => s.Id == vapr.Status).Select(s => s.Status).FirstOrDefault()
                           });

                return lst.ToDataSourceResult(request);
            }
        }


        public CommonViewModel GetApplicantPremiumDuesByRegistrationId(string registrationId)
        {
            throw new NotImplementedException();
        }

        public CommonViewModel GetLeaseRentDateByRegistrationId(string registrationId)
        {
            throw new NotImplementedException();
        }


        public int SaveDetailForNDC(NDCVeiwModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingNDC = dbContext.PRE_FULL_PAYMENT_NDC.Where(r => r.RegistrationId.ToString() == model.RegistrationId).FirstOrDefault();
                //existingNDC.ForEach(m => m.IsActive = false);

                if (existingNDC != null)
                {
                    existingNDC.PropertyNo = model.PropertyNo;
                    existingNDC.Applicant = model.Applicant;
                    existingNDC.TotalPaidPream = model.IsPremiumPaid;
                    existingNDC.OneTimeLease = model.IsLeaseRentPaid;
                    if (model.IsLeaseRentPaid == NA.PMS.Common.NDCOptions.Id_Yes)
                    {
                        existingNDC.NDCDate = model.NDCDate;
                    }
                    if (model.IsLeaseRentPaid == NA.PMS.Common.NDCOptions.Id_No)
                    {
                        existingNDC.LeaseRentAmount = model.LeaseRentAmount;
                        existingNDC.LeaseRentUpto = model.LastYearLeaseRentPaidUpto;
                        //existingNDC.DuePrincipalAmount = model.PremiumDues;
                        //existingNDC.LeaseRentPaidDate = model.LeaseRentPaidDate;
                    }
                    existingNDC.Status = "1";
                    existingNDC.IsActive = true;

                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                else
                {
                    PRE_FULL_PAYMENT_NDC ndc = new PRE_FULL_PAYMENT_NDC();
                    ndc.RegistrationId = Convert.ToInt32(model.RegistrationId);
                    ndc.PropertyNo = model.PropertyNo;
                    ndc.Applicant = model.Applicant;
                    ndc.TotalPaidPream = model.IsPremiumPaid;
                    ndc.OneTimeLease = model.IsLeaseRentPaid;
                    //ndc.DuePrincipalAmount = model.PremiumDues;
                    ndc.LeaseRentAmount = model.LeaseRentDues;
                    ndc.Status = "1";
                    if (model.IsLeaseRentPaid == NA.PMS.Common.NDCOptions.Id_Yes)
                    {
                        ndc.NDCDate = model.NDCDate;
                    }
                    if (model.IsLeaseRentPaid == NA.PMS.Common.NDCOptions.Id_No)
                    {
                        //ndc.LeaseRentPaidDate = model.LeaseRentPaidDate;
                        //ndc.LastYearLeaseRentPaidUpto = model.LastYearLeaseRentPaidUpto;
                        //ndc.LeaseRentRevisedAfterYear = model.LeaseRentRevisedAfterYear;
                        //ndc.LeaseRentRevisedPercentage = model.LeaseRentRevisedPercentage;
                    }

                    ndc.EntryDate = DateTime.Now;
                    ndc.IsActive = true;
                    ndc.CreatedBy = userInfo.UserID.ToString();
                    ndc.CreatedOn = DateTime.Now;

                    dbContext.PRE_FULL_PAYMENT_NDC.Add(ndc);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }


                //ndc.NDCDate = DateTime.Now; 

                //Changed on 9th August 2017 

            }
            return flag;
        }

        public DataSourceResult GetNoDuesCertificateList(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var UserDepartment = (from userMaster in dbContext.UmUserMasters
                                      join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                      where userMaster.UserRefId == userInfo.UserID
                                      select departmentTrans.DepartmentId).ToList();

                var ndclist = (from ndc in dbContext.PRE_FULL_PAYMENT_NDC
                                   //join alot in dbContext.AllotmentMasters on ndc.RegistrationId equals alot.rid
                                   //where ndc.IsActive == true && UserDepartment.Contains(alot.departmentId)
                               select new NDCVeiwModel
                               {
                                   Id = ndc.Id,
                                   RegistrationId = ndc.RegistrationId.ToString(),
                                   Applicant = ndc.Applicant,
                                   EntryDate = ndc.EntryDate,
                                   NDCDate = ndc.NDCDate,
                                   Status = ndc.Status == "1" ? "Issued" : "Not Issued"
                               });
                return ndclist.ToDataSourceResult(request);
            }
        }

        public int UpdateRegistrationIdByRequestNo(int requestNo, string registrationId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var service = dbContext.Customer_ServiceRequest.Where(c => c.Id == requestNo && c.IsActive == true).FirstOrDefault();
                if (service != null)
                {
                    if (service.Registration_No == null || service.Registration_No == "")
                    {
                        service.Registration_No = registrationId;
                        dbContext.SaveChanges();
                        return ReturnType.Success;
                    }
                    else
                    {
                        return ReturnType.Exist;
                    }
                }
                else
                {
                    return ReturnType.NotExist;
                }
            }
        }

        public PropertyDetailViewModel GetAllotteDetailsByRegistrationId(string registrationId, string flag1, string flag2)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var allottee = dbContext.ApplicationDetails.Where(a => a.registrationId.ToString() == registrationId).FirstOrDefault();
                if (allottee != null)
                {
                    PropertyDetailViewModel model = new PropertyDetailViewModel();
                    if (flag1 == "OLD")
                    {
                        model.FirstName = allottee.firstName;
                        model.MiddleName = allottee.middleName;
                        model.LastName = allottee.lastName;
                        model.ApplicantType = allottee.gender;
                        model.ApplicantMaster = allottee.gender == "Company" ? allottee.signingAuthority : allottee.fatherHusbandName;
                        model.MobileNo = allottee.mobileNumberP1;
                        model.Email = allottee.email;
                        model.GenderType = allottee.gender;
                        model.CorrespondAddress = allottee.correspondanceAdd;
                        model.PermanentAddress = allottee.permanentAdd;
                    }
                    if (flag1 == "NEW")
                    {
                        model.FirstName = allottee.tFirstName;
                        model.MiddleName = allottee.tMiddleName;
                        model.LastName = allottee.tLastName;
                        model.ApplicantType = allottee.tGender;
                        model.ApplicantMaster = allottee.tGender == "Company" ? allottee.tSigningAuthority : allottee.tFatherHusbandName;
                        model.MobileNo = allottee.tMobileNumber;
                        model.Email = allottee.tEmail;
                        model.GenderType = allottee.tGender;
                        model.CorrespondAddress = allottee.tCorrespondanceAdd;
                        model.PermanentAddress = allottee.tPermanentAdd;
                        model.PAN = allottee.tPan;
                        model.GST = allottee.tGSTNo;
                    }
                    return model;
                }
                else
                {
                    return null;
                }              
            }
        }

        public int UpdateAllotteeBasicInfo(PropertyDetailViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var allottee = dbContext.ApplicationDetails.Where(a => a.registrationId == model.RegistrationId).FirstOrDefault();
                if (allottee != null)
                {
                    if (model.FlagString == "NEW")
                    {
                        allottee.tFirstName = model.FirstName;
                        allottee.tMiddleName = model.MiddleName;
                        allottee.tLastName = model.LastName;
                        allottee.tGender = model.GenderType;
                        allottee.tMobileNumber = model.MobileNo;
                        allottee.tEmail = model.Email;
                        allottee.tCorrespondanceAdd = model.CorrespondAddress;
                        allottee.tPermanentAdd = model.PermanentAddress;
                        allottee.T_Company_Name = model.ApplicantType == "Company" ? model.FirstName : null;
                        allottee.tSigningAuthority = model.ApplicantType == "Company" ? model.ApplicantMaster : null;
                        allottee.tFatherHusbandName = model.ApplicantType == "Company" ? null : model.ApplicantMaster;
                        allottee.tPan = model.PAN == null ? allottee.tPan : model.PAN;
                        allottee.tGSTNo = model.GST == null ? allottee.tGSTNo : model.GST;
                        dbContext.SaveChanges();
                    }
                    if (model.FlagString == "OLD")
                    {
                        allottee.firstName = model.FirstName;
                        allottee.middleName = model.MiddleName;
                        allottee.lastName = model.LastName;
                        allottee.gender = model.GenderType;
                        allottee.mobileNumberP2 = model.MobileNo;
                        allottee.email = model.Email;
                        allottee.correspondanceAdd = model.CorrespondAddress;
                        allottee.permanentAdd = model.PermanentAddress;
                        allottee.T_Company_Name = model.ApplicantType == "Company" ? model.FirstName : null;
                        allottee.signingAuthority = model.ApplicantType == "Company" ? model.ApplicantMaster : null;
                        allottee.fatherHusbandName = model.ApplicantType == "Company" ? null : model.ApplicantMaster;
                        dbContext.SaveChanges();
                    }
                    return ReturnType.Success;
                }
                else
                {
                    return ReturnType.NotExist;
                }
            }
        }


        public int UpdateTransferDetailById(PropertyDetailViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var allottee = dbContext.Succ_Mut_Trans.Where(r => r.Rid == model.RegistrationId).ToList();
                var reqNo = allottee.Max(m => m.Request_No);
                var transfer = dbContext.Succ_Mut_Trans.Where(r => r.Request_No == reqNo).FirstOrDefault();
                if (transfer != null)
                {
                    if (model.TransferFlag == "Transferee")
                    {
                        transfer.T_First_Name = model.FirstName;
                        transfer.T_Middle_Name = model.MiddleName;
                        transfer.T_Last_Name = model.LastName;
                        transfer.T_Mobile = model.MobileNo;
                        transfer.T_Correspondence_Add = model.CorrespondAddress;
                        transfer.T_Permanent_Add = model.PermanentAddress;
                        transfer.T_Company_Name = model.ApplicantType == Constants.Company ? model.FirstName + " " + model.MiddleName + " " + model.LastName : null;
                        transfer.T_Signing_Authority = model.ApplicantType == Constants.Company ? model.ApplicantMaster : null;
                        transfer.T_Father_Husband_Name = model.ApplicantType == Constants.Company ? null : model.ApplicantMaster;
                    }
                    if (model.TransferFlag == "Allottee")
                    {
                        transfer.Applicant_Name = model.FirstName + " " + model.MiddleName + " " + model.LastName;
                        transfer.T_Mobile = model.MobileNo;
                        transfer.Correspondance_Add = model.CorrespondAddress;
                        transfer.Permanent_Add = model.PermanentAddress;
                        transfer.Applicant_Signing_Authority = model.ApplicantType == Constants.Company ? model.ApplicantMaster : null;
                        transfer.ApplicantFather_Name = model.ApplicantType == Constants.Company ? null : model.ApplicantMaster;
                    }
                    transfer.Transfer_Charge = model.TransferCharge == null ? transfer.Transfer_Charge : model.TransferCharge;
                    transfer.Total_Transfer_Charge = transfer.Total_Transfer_Charge == null ? model.TransferCharge : transfer.Total_Transfer_Charge;
                    transfer.Transfer_Date = model.TransferDate == null ? transfer.Transfer_Date : model.TransferDate;
                    transfer.Mutation_Date = transfer.Type == "M" ? model.TransferDate : null;
                    dbContext.SaveChanges();
                    return ReturnType.Success;
                }
                else
                {
                    return ReturnType.NotExist;
                }
            }
        }


        public PropertyDetailViewModel GetTransferDetailsByRegistrationId(string registrationId, string flag)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var allottee = dbContext.Succ_Mut_Trans.Where(r => r.Rid.ToString() == registrationId).ToList();
                var reqNo = 0;
                if (allottee.Count > 0)
                {
                    reqNo = allottee.Max(m => m.Request_No);
                }
                var transfer = dbContext.Succ_Mut_Trans.Where(r => r.Request_No == reqNo).FirstOrDefault();
                if (transfer != null)
                {
                    PropertyDetailViewModel model = new PropertyDetailViewModel();
                    if (flag == "Transferee")
                    {
                        model.Id = transfer.Request_No;
                        model.FirstName = (transfer.T_First_Name == null || transfer.T_First_Name == "") ? transfer.T_Company_Name : transfer.T_First_Name;
                        model.MiddleName = transfer.T_Middle_Name;
                        model.LastName = transfer.T_Last_Name;
                        model.ApplicantType = transfer.T_Gender;
                        model.MobileNo = transfer.T_Mobile;
                        model.ApplicantMaster = transfer.T_Gender == Constants.Company ? transfer.T_Signing_Authority : transfer.T_Father_Husband_Name;
                        //model.SigningAuthority = transfer.T_Signing_Authority;
                        model.CorrespondAddress = transfer.T_Correspondence_Add;
                        model.PermanentAddress = transfer.T_Permanent_Add;
                    }
                    if (flag == "Allottee")
                    {
                        model.Id = transfer.Request_No;
                        model.FirstName = transfer.Applicant_Name;
                        //model.MiddleName = transfer.T_Middle_Name;
                        //model.LastName = transfer.T_Last_Name;
                        model.ApplicantType = transfer.Applicant_Gender;
                        model.MobileNo = transfer.T_Mobile;
                        model.ApplicantMaster = transfer.ApplicantFather_Name;
                        //model.SigningAuthority = transfer.Applicant_Signing_Authority;
                        model.CorrespondAddress = transfer.Correspondance_Add;
                        model.PermanentAddress = transfer.Permanent_Add;
                    }
                    model.TransferCharge = transfer.Transfer_Charge;
                    model.TransferDate = transfer.Transfer_Date;
                    return model;
                }
                else
                {
                    return null;
                }
            }
        }


        public PropertyDetailViewModel GetMultipleDetailsByRegistrationId(string registrationId, string flag)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var model = new PropertyDetailViewModel();
                if (flag == "Allotment")
                {
                    var allotment = dbContext.AllotmentMasters.Where(a => a.rid.ToString() == registrationId).FirstOrDefault();
                    if (allotment != null)
                    {
                        model.AllotmentDate = allotment.allotmentDate;
                        model.InstallmentStartDate = allotment.instalmentStartDate;
                    }
                    return model;
                }
                if (flag == "Possession")
                {
                    var possession = dbContext.PossessionDetails.Where(p => p.Rid.ToString() == registrationId).FirstOrDefault();
                    if (possession != null)
                    {
                        model.PossessionDate = possession.PossessionDate;
                        model.PossessionOrderDate = possession.PossessionOrderDate;
                    }
                    return model;
                }
                if (flag == "Checklist")
                {
                    var checklist = dbContext.ChecklistTrans.Where(p => p.Rid.ToString() == registrationId).FirstOrDefault();
                    if (checklist != null)
                    {
                        model.ChecklistDate = checklist.ChecklistDate;
                    }
                    return model;
                }
                if (flag == "Registry")
                {
                    var registry = dbContext.RegistryDetails.Where(p => p.Rid.ToString() == registrationId).FirstOrDefault();
                    if (registry != null)
                    {
                        model.RegistryDate = registry.RegistryDoneDate;
                        model.RegistryDueDate = registry.RegistryDueDate;
                    }
                    return model;
                }
                else
                {
                    return null;
                }
            }
        }

        public int UpdateDateFieldsByRegistrationId(string registrationId, DateTime? firstDate, DateTime? secondDate, string flag)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (flag == "Checklist")
                {
                    var checklist = dbContext.ChecklistTrans.Where(a => a.Rid.ToString() == registrationId && a.IsActive == true).FirstOrDefault();
                    if (checklist != null)
                    {
                        checklist.ChecklistDate = firstDate;
                        dbContext.SaveChanges();
                        return ReturnType.Success;
                    }
                    else return ReturnType.Failure;
                }
                if (flag == "Allotment")
                {
                    var allotment = dbContext.AllotmentMasters.Where(a => a.rid.ToString() == registrationId && a.isActive == 1).FirstOrDefault();
                    if (allotment != null)
                    {
                        allotment.allotmentDate = firstDate;
                        allotment.instalmentStartDate = secondDate;
                        dbContext.SaveChanges();
                        return ReturnType.Success;
                    }
                    else return ReturnType.Failure;
                }
                if (flag == "Registry")
                {
                    var registry = dbContext.RegistryDetails.Where(a => a.Rid.ToString() == registrationId && a.IsActive == true).FirstOrDefault();
                    if (registry != null)
                    {
                        registry.RegistryDoneDate = firstDate;
                        registry.RegistryDueDate = secondDate;
                        dbContext.SaveChanges();
                        return ReturnType.Success;
                    }
                    else return ReturnType.Failure;
                }
                if (flag == "Possession")
                {
                    var possession = dbContext.PossessionDetails.Where(p => p.Rid.ToString() == registrationId && p.IsActive == true).FirstOrDefault();
                    if (possession != null)
                    {
                        possession.PossessionOrderDate = firstDate;
                        possession.PossessionDate = secondDate;
                        dbContext.SaveChanges();
                        return ReturnType.Success;
                    }
                    else return ReturnType.Failure;
                }
                else
                {
                    return ReturnType.NotExist;
                }
            }
        }


        public SchemePropertyModel GetPropertyDetailsById(int? propertyId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from property in dbContext.SchemePropTrans
                              where property.propertyId == propertyId && property.IsActive == true
                              select new SchemePropertyModel
                              {
                                  Id = property.refId,
                                  propertyId = property.propertyId,
                                  ParentPropertyId = property.ParentPropertyId,
                                  SectorId = property.sectorId,
                                  Sector = property.SectorMst.sectorName,
                                  BlockId = property.blockId,
                                  Block = property.BlockMst.blockName,
                                  FloorId = property.floorId,
                                  Floor = property.FloorMst.floorName,
                                  PlotNo = property.propertyNo,
                                  PropertyCost = property.propertyCost,
                                  CivilCost = property.civilCost,
                                  TotalPropertyCost = property.totalPropertyCost,
                                  PropertyRate = property.landRatePerSqmt,
                                  CoveredArea = property.coveredArea,
                                  ActualArea = property.actualArea,
                                  TotalArea = property.totalArea,
                                  Registry = property.Registry,
                                  AllotmentMoney = property.allotmentMoney
                              }).FirstOrDefault();
                return detail;
            }
        }


        public int UpdatePropertyDetail(PropertyDetailViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var property = dbContext.SchemePropTrans.Where(p => p.propertyId == model.PropertyId).FirstOrDefault();
                if (property != null)
                {
                    property.ParentPropertyId = model.ParentPropertyId;
                    property.sectorId = model.SectorId;
                    property.blockId = model.BlockId;
                    property.floorId = model.FloorAreaId;
                    property.propertyNo = model.PlotNo;
                    property.landRatePerSqmt = model.PropertyRate;
                    property.propertyCost = model.PropertyCost;
                    property.civilCost = model.CivilCost;
                    property.totalPropertyCost = model.TotalPropertyCost;
                    property.coveredArea = model.CoveredArea;
                    property.actualArea = model.ActualArea;
                    property.totalArea = model.TotalArea;
                    property.Registry = model.Registry;
                    property.allotmentMoney = model.AllotmentMoney;
                    dbContext.SaveChanges();
                    return ReturnType.Success;
                }
                else
                {
                    return ReturnType.NotExist;
                }
            }
        }


        public string GetRegistrationIdByRequestNo(int? requestNo)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.Customer_ServiceRequest.Where(c => c.Id == requestNo.Value).FirstOrDefault();
                return data.Registration_No; //rid;// 
            }
        }

        public DataSourceResult GetVacantProperties(DataSourceRequest Req, int? DepartmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var UserDepartment = (from userMaster in dbContext.UmUserMasters
                                      join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                      where userMaster.UserRefId == userInfo.UserID
                                      select departmentTrans.DepartmentId).ToList();

                var detail = (from vancantProp in dbContext.VecantPropertyLists
                              where UserDepartment.Contains(vancantProp.Departmentid)
                              && (DepartmentId == null || vancantProp.Departmentid == DepartmentId)
                              select new VacantPropertyViewModel
                              {
                                  DepartmentId = vancantProp.Departmentid != null ? (int)vancantProp.Departmentid : 0,
                                  SectorId = vancantProp.sectorid != null ? (int)vancantProp.sectorid : 0,
                                  SectorName = vancantProp.SectorName,
                                  BlockName = vancantProp.BlockName,
                                  DepartmentName = vancantProp.DepartmentName,
                                  TotalArea = vancantProp.totalarea,
                                  PlotProperty = vancantProp.propertyno
                              });

                return detail.ToDataSourceResult(Req);
            }
        }


        public MortgageViewModel GetMortgageDetailsByRid(int? registrationId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var UserDepartment = (from userMaster in dbContext.UmUserMasters
                                      join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                      where userMaster.UserRefId == userInfo.UserID
                                      select departmentTrans.DepartmentId).ToList();
                MortgageViewModel detail = new MortgageViewModel();
                var mort = dbContext.MortgageDetails.FirstOrDefault(m => m.RID == registrationId && m.IsActive == true);
                if (mort != null)
                {
                    detail = (from mortgage in dbContext.MortgageDetails
                              where mortgage.RID == registrationId && mortgage.IsActive == true
                              select new MortgageViewModel
                              {
                                  RegistrationId = mortgage.RID,
                                  OnlineRequestNo = mortgage.OnlineRequestNo,
                                  MortgageDate = mortgage.MortgageDate,
                                  ValidUpto = mortgage.ValidUpto,
                                  Id = mortgage.RequestNo,
                                  BankName = mortgage.BankName,
                                  BranchAddress = mortgage.BranchAddress,
                                  MortgageType = mortgage.MortgageType,
                                  //MortgageTypeId = mortgage.MortgageType,
                                  ProcessingFee = mortgage.ProcessingFee,
                                  SanctionedAmount = mortgage.SanctionedAmount,
                                  PreviousLoanNoc = mortgage.PreviousLoanNoc,
                                  ApproveDate = mortgage.ApproveDate,
                                  MortgageStatusId = mortgage.StatusId
                              }).FirstOrDefault();
                    detail.MortgageTypeId = detail.MortgageType == null ? 0 : Convert.ToInt32(detail.MortgageType);
                    return detail;
                }
                else
                {
                    return null;
                }
            }
        }

        public int UpdateMortgageDetail(MortgageViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var mortgage = dbContext.MortgageDetails.FirstOrDefault(m => m.RequestNo == model.Id && m.IsActive == true);
                if (mortgage != null)
                {
                    mortgage.RID = model.RegistrationId;
                    mortgage.OnlineRequestNo = model.OnlineRequestNo;
                    mortgage.MortgageDate = model.MortgageDate;
                    mortgage.ValidUpto = model.ValidUpto;
                    mortgage.BankName = model.BankName;
                    mortgage.BranchAddress = model.BranchAddress;
                    mortgage.MortgageType = model.MortgageType;
                    mortgage.ProcessingFee = model.ProcessingFee;
                    mortgage.SanctionedAmount = model.SanctionedAmount;
                    mortgage.PreviousLoanNoc = model.PreviousLoanNoc;
                    mortgage.ApproveDate = model.ApproveDate;
                    mortgage.StatusId = model.MortgageStatusId;
                    dbContext.SaveChanges();
                    return ReturnType.Success;
                }
                else
                {
                    return ReturnType.Failure;
                }
            }
        }


        public int UpdateServiceRequestDetail(ServiceRequestModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == ActionType.RegistrationId && model.Id != null && model.RegistrationId != null)
                {
                    var data = dbContext.Customer_ServiceRequest.FirstOrDefault(r => r.Id == model.Id);
                    if (data != null)
                    {
                        data.Registration_No = model.RegistrationId.ToString();
                        dbContext.SaveChanges();
                        flag = ReturnType.Updated;
                    }
                }
                if (model.ActionType == ActionType.ServiceStatus && model.Id != null && model.StatusId != null)
                {
                    var data = dbContext.Customer_ServiceRequest.FirstOrDefault(r => r.Id == model.Id);
                    if (data != null)
                    {
                        data.Request_Status = model.StatusId;
                        dbContext.SaveChanges();
                        flag = ReturnType.Updated;
                    }
                }
                if (model.ActionType == ActionType.Comment && model.Id != null)
                {
                    var data = dbContext.Customer_ServiceRequest.FirstOrDefault(r => r.Id == model.Id);
                    if (data != null)
                    {
                        data.Comment = model.Comment;
                        dbContext.SaveChanges();
                        flag = ReturnType.Updated;
                    }
                }
                if (model.ActionType == ActionType.ServiceId && model.Id != null && model.ServiceId != null)
                {
                    var data = dbContext.Customer_ServiceRequest.FirstOrDefault(r => r.Id == model.Id);
                    if (data != null)
                    {
                        data.ServiceId = model.ServiceId;
                        dbContext.SaveChanges();
                        flag = ReturnType.Updated;
                    }
                }
            }
            return flag;
        }


        public ServiceRequestModel GetServiceRequestDetailById(int? id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from service in dbContext.Customer_ServiceRequest
                            where service.Id == id
                            select new ServiceRequestModel
                            {
                                Id = service.Id,
                                Registration_No = service.Registration_No,
                                StatusId = service.Request_Status,
                                Comment = service.Comment,
                                Description = service.Description,
                                Status = dbContext.StatusMasters.FirstOrDefault(r => r.Id == service.Request_Status).Status
                            }).FirstOrDefault();
                return data;
            }
        }


        public DataSourceResult GetVacantPropertyReport(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var propertyList = dbContext.AllotmentMasters.Where(y => y.isStatus == AllotmentStatus.Approved.ToString() || y.isStatus == AllotmentStatus.InProgress.ToString()).Select(x => x.propertyId);

                var list = (from property in dbContext.SchemePropTrans
                            where property.IsActive == true && property.propertyId != null && !propertyList.Contains(property.propertyId.Value)
                            && DepartmentList.Contains(property.departmentId)
                            && (model.SchemeId == null || property.schemeId == model.SchemeId)
                            && (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                            && (model.SectorId == null || property.sectorId == model.SectorId)
                            && (model.BlockId == null || property.blockId == model.BlockId)
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
                                SectorId = property.SectorMst.sectorId,
                                SectorName = property.SectorMst.sectorName,
                                BlockName = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                TotalArea = property.totalArea,
                                Registry = property.Registry,
                                SchemeStatusId = property.SchemeMst.Status,
                                SchemeStatus = property.SchemeMst.Status == 24 ? "Open" : (property.SchemeMst.Status == 25 ? "InProgress" : (property.SchemeMst.Status == 26 ? "Closed" : "Old Scheme")),
                                IsPropertyAllotted = dbContext.AllotmentMasters.Where(p => p.propertyId == property.propertyId && p.isStatus == Status.Approved).FirstOrDefault() == null ? false : true
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public PropertyViewModel GetRegistrationIdByApplicationIdOrFormNo(PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from application in dbContext.ApplicationDetails
                            where application.applicationId == model.ApplicationId
                            select new PropertyViewModel
                            {
                                ApplicationId = application.applicationId,
                                RegistrationId = application.registrationId,
                                FormNo = application.formNo
                            }).FirstOrDefault();
                return data;
            }
        }

        public int UpdateRegistrationIdByApplicationIdOrFormNo(PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                var application = dbContext.ApplicationDetails.FirstOrDefault(r => r.applicationId == model.ApplicationId);
                if (application != null)
                {
                    application.registrationId = model.RegistrationId;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                return flag;
            }

        }

        public List<ServiceReportDepartmentWiseVM> GetServiceReportDepartmentWise(ServiceReportDepartmentWiseVM serviceReport)
        {
            var data = new List<ServiceReportDepartmentWiseVM>();
            using (var dbContext = new NoidaPMSEntities())
            {
                data = (from report in dbContext.Sp_ServiceReportDepartmentWise(serviceReport.DepartmentId, serviceReport.StartDate, serviceReport.EndDate, serviceReport.RequestThrough).ToList()
                        select new ServiceReportDepartmentWiseVM
                        {
                            ServiceName = report.ServiceName,
                            Cancelled = report.Cancelled,
                            Completed = report.Completed,
                            Initiated = report.Initiated,
                            InProgress = report.InProgress,
                            Pending = report.Pending,
                            TotalRequest = report.TotalRequest,
                            InProgessPercentage = report.InProgessPercentage,
                            DepartmentName = serviceReport.DepartmentId > 0 ? dbContext.DepartmentMsts.Where(m => m.departmentId == serviceReport.DepartmentId && m.IsActive == true).FirstOrDefault().departmentName : string.Empty
                        }).ToList();
            }
            return data;
        }


        public DataSourceResult GetKYAReportsAsDataSource(DataSourceRequest request, KYAViewModel model)
        {
            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                var list = connection.Query<KYAViewModel>("SP_KYAReport", commandType: System.Data.CommandType.StoredProcedure).ToList();
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetChallanReportsAsDataSource(DataSourceRequest request, ChallanViewModel model)
        {
            throw new NotImplementedException();
        }

        public DataSourceResult GetServiceReportsAsDataSource(DataSourceRequest request, ServiceViewModel model)
        {
            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                //var list = connection.Query<ServiceViewModel>("Sp_ServiceReportDepartmentWise", new { DepartmentId = model.DepartmentId, StartDate = model.StartDate, EndDate = model.EndDate, requestthru = model.RequestThrough }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                var list = connection.Query<ServiceViewModel>("Sp_ServiceStatusReportByServiceType", new { DepartmentId = model.DepartmentId, StartDate = model.StartDate, EndDate = model.EndDate, RequestThrough = model.RequestThrough }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return list.ToDataSourceResult(request);
            }
        }

        public IEnumerable<KYAViewModel> GetKYAReportsForGraph(KYAViewModel model)
        {
            List<KYAViewModel> list = new List<KYAViewModel>();
            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                list = connection.Query<KYAViewModel>("SP_KYADashboardGraph", new { DepartmentId = model.DepartmentId, StartDate = model.StartDate, EndDate = model.EndDate, FilterType = model.FilterType }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                //return list.ToDataSourceResult(request);
            }

            
            //model.StatusDate = Convert.ToDateTime("12/14/2018");
            //if (model.StatusDate != null)
            //{
            //    var FromDate = Convert.ToDateTime(model.StatusDate.Value.ToString("MM/dd/yyyy") + " 00:00");
            //    var ToDate = Convert.ToDateTime(model.StatusDate.Value.ToString("MM/dd/yyyy") + " 23:59");
            //    using (var dbContext = new NoidaPMSEntities())
            //    {
            //        //List<KYAViewModel> list = new List<KYAViewModel>();
            //        for (int i = 1; i < 8; i++)
            //        {
            //            KYAViewModel kya = new KYAViewModel();
            //            kya.DepartmentId = i;
            //            kya.Department = dbContext.DepartmentMsts.FirstOrDefault(m => m.departmentId == i).departmentName;
            //            kya.Count = dbContext.KYADetails.Where(k => k.DepartmentId == i && k.SubmitDate >= FromDate && k.SubmitDate <= ToDate).ToList().Count;
            //            list.Add(kya);
            //        }
            //        //return list;
            //    }
            //}
            return list;
        }

        public IEnumerable<ChallanViewModel> GetChallanReportsForGraph(ChallanViewModel model)
        {
            //using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            //{
            //    var dbContext = new NoidaPMSEntities();

            //    string reportFlag = string.Empty;
            //    if (model.FilterType != "ChallanForGrid")
            //    {
            //        if (model.FilterType != "TotalChallan")
            //        {
            //            if (model.FilterType != "NotVerifiedChallan")
            //            {
            //                model.FilterType = "TotalChallan";
            //                int departmentId = dbContext.Database.SqlQuery<int>("select distinct departmentId from DepartmentMst where departmentName like '" + model.ActionType + "%'").FirstOrDefault();
            //                int bankId = dbContext.Database.SqlQuery<int>("select bankId from BankMst where bankName like '" + model.ActionType + "%'").FirstOrDefault();
            //                if (departmentId > 0)
            //                {
            //                    HttpContext.Current.Session["SPDepartment"] = model.ActionType;
            //                    model.DepartmentId = departmentId;
            //                    model.Department = model.ActionType;
            //                    model.FilterType = "ChallanByDepartment";
            //                }
            //                else if (bankId > 0)
            //                {
            //                    model.Department = (string)HttpContext.Current.Session["SPDepartment"];
            //                    model.BankName = model.ActionType;
            //                    model.FilterType = "ChallanByBank";
            //                }

            //                if (model.FilterType == "TotalChallan")
            //                {
            //                    HttpContext.Current.Session["SPDepartment"] = null;
            //                }
            //            }
                        
            //        }                   
            //    }
            //    else
            //    {
            //        model.FilterType = "ChallanForGrid";
            //    }

            //    var list = connection.Query<ChallanViewModel>("sp_ChallanServicesGraph", new { Department = model.Department, BankName = model.BankName, StartDate=model.StartDate, EndDate=model.EndDate, ReportFlag = model.FilterType }, commandType: System.Data.CommandType.StoredProcedure).ToList();

            //    return list;
            //}

            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                var dbContext = new NoidaPMSEntities();

                string reportFlag = string.Empty;
                if (model.FilterType != "ChallanForGrid")
                {
                    if (model.FilterType != "ChallanByAmount")
                    {
                        if (model.FilterType != "ChallanByStatus")
                        {
                            if (model.FilterType != "NotVerifiedChallan")
                            {
                                model.FilterType = "TotalChallan";
                                int departmentId = dbContext.Database.SqlQuery<int>("select distinct departmentId from DepartmentMst where departmentName like '" + model.ActionType + "%'").FirstOrDefault();
                                int bankId = dbContext.Database.SqlQuery<int>("select bankId from BankMst where bankName like '" + model.ActionType + "%'").FirstOrDefault();
                                if (departmentId > 0)
                                {
                                    HttpContext.Current.Session["SPDepartment"] = model.ActionType;
                                    model.DepartmentId = departmentId;
                                    model.Department = model.ActionType;
                                    model.FilterType = "ChallanByDepartment";
                                }
                                else if (bankId > 0)
                                {
                                    model.Department = (string)HttpContext.Current.Session["SPDepartment"];
                                    model.BankName = model.ActionType;
                                    model.FilterType = "ChallanByBank";
                                }

                                if (model.FilterType == "TotalChallan")
                                {
                                    HttpContext.Current.Session["SPDepartment"] = null;
                                }
                            }
                        }
                        
                    }
                }
                else
                {
                    model.FilterType = "ChallanForGrid";
                }

                var list = connection.Query<ChallanViewModel>("sp_ChallanServicesGraph", new { Department = model.Department, BankName = model.BankName, StartDate = model.StartDate, EndDate = model.EndDate, ReportFlag = model.FilterType }, commandType: System.Data.CommandType.StoredProcedure).ToList();

                return list;
            }
        }

        public IEnumerable<ServiceViewModel> GetServiceReportsForGraph(ServiceViewModel model)
        {
            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                //model.DepartmentId = 4;
                //model.StartDate = DateTime.Parse("2018-01-01"); //Convert.ToDateTime("2018-01-01");
                //model.EndDate = DateTime.Parse("2018-11-01"); //Convert.ToDateTime("2018-11-01");
                //var list = connection.Query<ServiceViewModel>("Sp_ServiceReportDepartmentWise", new { DepartmentId = model.DepartmentId, StartDate = model.StartDate, EndDate = model.EndDate }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                var list = connection.Query<ServiceViewModel>("Sp_ServiceReportDepartmentWise", new { DepartmentId = model.DepartmentId, StartDate = model.StartDate, EndDate = model.EndDate, requestthru = model.RequestThrough }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return list;
            }
        }


        public DataSourceResult GetDemandAndNDCListByDepartmentAsDataSource(DataSourceRequest request, NDCVeiwModel model)
        {
            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                //model.StartDate = model.StartDate == null ? DateTime.Parse("2018-11-01") : model.StartDate;
                //model.EndDate = model.EndDate == null ? DateTime.Now : model.EndDate;
                var list = connection.Query<NDCVeiwModel>("Sp_ServiceReportDepartmentWise", new { DepartmentId = 1, StartDate = model.StartDate, EndDate = model.EndDate, requestthru = "NDC" }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return list.ToDataSourceResult(request);
            }
        }

        public IEnumerable<NDCVeiwModel> GetNDCAndDemandReportsForGraph(NDCVeiwModel model)
        {
            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                //model.StartDate = model.StartDate == null ? DateTime.Parse("2018-11-01") : model.StartDate;
                //model.EndDate = model.EndDate == null ? DateTime.Now : model.EndDate;
                var list = connection.Query<NDCVeiwModel>("Sp_ServiceReportDepartmentWise", new { DepartmentId = 1, StartDate = model.StartDate, EndDate = model.EndDate, requestthru = "NDC" }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return list;
            }
        }


        public IEnumerable<ServiceViewModel> GetServiceReportsForGraphII(ServiceViewModel model)
        {
            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                var dbContext = new NoidaPMSEntities();
                
                string reportFlag = string.Empty;
                if (model.FilterType != "CompletedServicesByServiceName")
                {
                    if (model.FilterType != "PendingServicesByServiceName")
                    {
                        if (model.FilterType != "TotalServicesByDepartment")
                        {
                            if (model.FilterType != "PendingServices")
                            {
                                model.FilterType = "TotalServicesByDepartment";
                                int departmentId = dbContext.Database.SqlQuery<int>("select distinct departmentId from DepartmentMst where departmentName like '" + model.ActionType + "%'").FirstOrDefault();
                                int serviceId = dbContext.Database.SqlQuery<int>("select Service_Id from CitizenService_Master where ServiceName like '" + model.ActionType + "%'").FirstOrDefault();
                                if (departmentId > 0)
                                {
                                    HttpContext.Current.Session["GDepartmentId"] = departmentId;
                                    model.DepartmentId = departmentId;
                                    model.FilterType = "ServicesByDepartment";
                                }
                                else if (serviceId > 0)
                                {
                                    model.DepartmentId = (int)HttpContext.Current.Session["GDepartmentId"];
                                    model.ServiceId = serviceId;
                                    model.FilterType = "StatusOfServices";
                                }

                                if (model.FilterType == "TotalServicesByDepartment")
                                {
                                    HttpContext.Current.Session["GDepartmentId"] = null;
                                }
                            }
                        }
                    }
                }
                
                var list = connection.Query<ServiceViewModel>("sp_ServiceRequestGraph", new { DepartmentId = model.DepartmentId, ServiceId = model.ServiceId, StatusId = model.StatusId, StartDate = model.StartDate, EndDate = model.EndDate, RequestThrough = model.RequestThrough, ReportFlag = model.FilterType }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                
                return list;
            }
        }


        public DataSourceResult GetCustomerServiceRequestDataAsDataSource(DataSourceRequest request, ServiceViewModel model)
        {
            int userid = userInfo.UserID;
            
            using (var dbContext = new NoidaPMSEntities())
            {
                List<int> statuslist = new List<int>();
                if (model.ServiceStatusId == NAStatusId.InProcess)
                {
                    statuslist.Add(8);
                    statuslist.Add(5);
                    statuslist.Add(10);
                    statuslist.Add(14);
                }
                else
                {
                    if (model.ServiceStatusId != null)
                    {
                        statuslist.Add((int)model.ServiceStatusId);
                    }
                    else
                    {
                        statuslist = dbContext.StatusMasters.Select(x => x.Id).ToList();
                    }
                    //statuslist = null;
                    //statuslist = dbContext.StatusMasters.Select(x=>x.Id).ToList();
                }

                var list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && csm.Status == 1 && csr.IsActive == true
                            && (model.Id == null || csr.Id == model.Id)
                            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                            && (model.ServiceStatusId == null || statuslist.Contains(csr.Request_Status.Value))
                            //&& ((statuslist == null || statuslist.Contains(csr.Request_Status)) || (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId))
                            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                            && (model.RequestThrough == null || csr.RequestThrough == model.RequestThrough)
                            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            select new ServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                                RequestDate = csr.Created_Date,
                                CreatedDate = csr.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = csr.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = csr.ServiceType,
                                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                                Amount = csr.DuesAmount,
                                StatusId = sts.Id,
                                Status = sts.Status,
                                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                ApproverId = csr.ApproverId,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public ServiceViewModel GetMultipleTypeIdToRedirect(ServiceViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.Department != null)
                {
                    model.DepartmentId = dbContext.Database.SqlQuery<int>("select distinct departmentId from DepartmentMst where departmentName like '" + model.Department + "%'").FirstOrDefault();
                }
                if (model.ServiceName != null)
                {
                    model.ServiceId = dbContext.Database.SqlQuery<int>("select distinct service_id from CitizenService_Master where servicename like '" + model.ServiceName + "%'").FirstOrDefault();
                }
                if (model.Status != null)
                {
                    model.StatusId = dbContext.Database.SqlQuery<int>("select distinct id from StatusMaster where status like '" + model.Status + "%'").FirstOrDefault();
                }
            }
            return model;
        }


        public DataSourceResult GetKYAReportDataAsDataSource(DataSourceRequest request, KYAViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<int> statuslist = new List<int>();
                if (model.ActionId == NAStatusId.InProcess)
                {
                    statuslist.Add(8);
                    statuslist.Add(5);
                    statuslist.Add(2);
                    statuslist.Add(14);
                }
                else
                {
                    if (model.ActionId != null)
                    {
                        statuslist.Add((int)model.ActionId);
                    }
                    else
                    {
                        statuslist = dbContext.StatusMasters.Select(x => x.Id).ToList();
                    }
                }
                if (model.ActionId != NAStatusId.InProcess)
                {
                    var data = (from kya in dbContext.KYADetails.GroupBy(g => g.RId).Select(x => x.FirstOrDefault(c => c.StatusId == model.ActionId))
                                where (model.Id == null || kya.Id == model.Id)
                                && (model.DepartmentId == null || kya.DepartmentId == model.DepartmentId)
                                    //&& (model.ActionId == null || kya.StatusId == model.ActionId)
                                && (model.ActionId == null || statuslist.Contains(kya.StatusId.Value))
                                && (model.StartDate == null || DbFunctions.TruncateTime(kya.CreatedDate) >= DbFunctions.TruncateTime(model.StartDate))
                                && (model.EndDate == null || DbFunctions.TruncateTime(kya.CreatedDate) <= DbFunctions.TruncateTime(model.EndDate))
                                && DepartmentList.Contains(kya.DepartmentId)
                                //&& kya.IsActive==true
                                select new KYAViewModel
                                {
                                    Id = kya.Id,
                                    KYAuid = kya.KYAuid,
                                    RegistrationId = kya.RId,
                                    DepartmentId = kya.DepartmentId,
                                    SectorId = kya.SectorId,
                                    BlockId = kya.BlockId,
                                    PlotNo = kya.PlotNo,
                                    Department = kya.DepartmentId != null ? dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == kya.DepartmentId).departmentName : string.Empty,
                                    Sector = kya.SectorId != null ? dbContext.SectorMsts.FirstOrDefault(s => s.sectorId == kya.SectorId).sectorName : Constants.NA,
                                    Block = kya.BlockId != null ? dbContext.BlockMsts.FirstOrDefault(b => b.blockId == kya.BlockId).blockName : Constants.NA,
                                    AllotteeName = kya.AllotteeName,
                                    AllotteeType = kya.AllotteeType,
                                    CorrespondAddress = kya.CorrespondAddress,
                                    AuthorizedSignatory = kya.AllotteeType == Constants.Company ? kya.AuthorizedSignatory : kya.FatherOrHusbandName,
                                    CountryCode = kya.CountryCode,
                                    MobileNo = kya.MobileNo,
                                    SignatoryMobileNo = kya.SignatoryMobileNo,
                                    Email = kya.Email,
                                    SignatoryEmail = kya.SignatoryEmail,
                                    PhoneNo = kya.PhoneNo,
                                    GSTNo = kya.GSTNo,
                                    PAN = kya.PAN,
                                    AadharNo = kya.AadharNo,
                                    ROC = kya.ROC,
                                    AddressLine1 = kya.AddressLine1,
                                    AddressLine2 = kya.AddressLine2,
                                    AreaLocality = kya.AreaLocality,
                                    City = kya.City,
                                    State = kya.State,
                                    PinCode = kya.PinCode,
                                    KYAReferenceCode = kya.KYAReferenceCode,
                                    AllotteeIdFileType = kya.IdFileType,
                                    PlotOwnershipFileType = kya.PlotOwnershipFileType,
                                    OtherFileType = kya.OtherFileType,
                                    IsActive = kya.IsActive,
                                    StatusId = kya.StatusId,
                                    Status = kya.StatusId != null ? dbContext.StatusMasters.FirstOrDefault(s => s.Id == kya.StatusId).Status : string.Empty,
                                    SubmitDate = kya.SubmitDate,
                                    ApprovalDate = kya.ApprovalDate,
                                    CreatedDate = kya.CreatedDate,
                                    ValidatorId = kya.ValidatorId,
                                    Validator = kya.ValidatorId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ValidatorId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ValidatorId).LastName) : string.Empty,
                                    ApproverId = kya.ApproverId,
                                    Approver = kya.ApproverId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ApproverId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ApproverId).LastName) : string.Empty,
                                    Remarks = kya.Remarks,
                                    OptionalAction = kya.OptionalAction,
                                    ActionId = kya.ActionId,
                                    IsInitiated = kya.StatusId == NAStatusId.Initiated ? true : false,
                                    IsForwarded = kya.StatusId == NAStatusId.Forwarded ? true : false,
                                    IsApproved = kya.StatusId == NAStatusId.Approved ? true : false
                                });

                    return data.ToDataSourceResult(request);
                }
                else
                {
                    var data = (from kya in dbContext.KYADetails
                                where (model.Id == null || kya.Id == model.Id)
                                && (model.DepartmentId == null || kya.DepartmentId == model.DepartmentId)
                                    //&& (model.ActionId == null || kya.StatusId == model.ActionId)
                                && (model.ActionId == null || statuslist.Contains(kya.StatusId.Value))
                                && (model.StartDate == null || DbFunctions.TruncateTime(kya.CreatedDate) >= DbFunctions.TruncateTime(model.StartDate))
                                && (model.EndDate == null || DbFunctions.TruncateTime(kya.CreatedDate) <= DbFunctions.TruncateTime(model.EndDate))
                                && DepartmentList.Contains(kya.DepartmentId)
                                //&& kya.IsActive==true
                                select new KYAViewModel
                                {
                                    Id = kya.Id,
                                    KYAuid = kya.KYAuid,
                                    RegistrationId = kya.RId,
                                    DepartmentId = kya.DepartmentId,
                                    SectorId = kya.SectorId,
                                    BlockId = kya.BlockId,
                                    PlotNo = kya.PlotNo,
                                    Department = kya.DepartmentId != null ? dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == kya.DepartmentId).departmentName : string.Empty,
                                    Sector = kya.SectorId != null ? dbContext.SectorMsts.FirstOrDefault(s => s.sectorId == kya.SectorId).sectorName : Constants.NA,
                                    Block = kya.BlockId != null ? dbContext.BlockMsts.FirstOrDefault(b => b.blockId == kya.BlockId).blockName : Constants.NA,
                                    AllotteeName = kya.AllotteeName,
                                    AllotteeType = kya.AllotteeType,
                                    CorrespondAddress = kya.CorrespondAddress,
                                    AuthorizedSignatory = kya.AllotteeType == Constants.Company ? kya.AuthorizedSignatory : kya.FatherOrHusbandName,
                                    CountryCode = kya.CountryCode,
                                    MobileNo = kya.MobileNo,
                                    SignatoryMobileNo = kya.SignatoryMobileNo,
                                    Email = kya.Email,
                                    SignatoryEmail = kya.SignatoryEmail,
                                    PhoneNo = kya.PhoneNo,
                                    GSTNo = kya.GSTNo,
                                    PAN = kya.PAN,
                                    AadharNo = kya.AadharNo,
                                    ROC = kya.ROC,
                                    AddressLine1 = kya.AddressLine1,
                                    AddressLine2 = kya.AddressLine2,
                                    AreaLocality = kya.AreaLocality,
                                    City = kya.City,
                                    State = kya.State,
                                    PinCode = kya.PinCode,
                                    KYAReferenceCode = kya.KYAReferenceCode,
                                    AllotteeIdFileType = kya.IdFileType,
                                    PlotOwnershipFileType = kya.PlotOwnershipFileType,
                                    OtherFileType = kya.OtherFileType,
                                    IsActive = kya.IsActive,
                                    StatusId = kya.StatusId,
                                    Status = kya.StatusId != null ? dbContext.StatusMasters.FirstOrDefault(s => s.Id == kya.StatusId).Status : string.Empty,
                                    SubmitDate = kya.SubmitDate,
                                    ApprovalDate = kya.ApprovalDate,
                                    CreatedDate = kya.CreatedDate,
                                    ValidatorId = kya.ValidatorId,
                                    Validator = kya.ValidatorId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ValidatorId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ValidatorId).LastName) : string.Empty,
                                    ApproverId = kya.ApproverId,
                                    Approver = kya.ApproverId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ApproverId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ApproverId).LastName) : string.Empty,
                                    Remarks = kya.Remarks,
                                    OptionalAction = kya.OptionalAction,
                                    ActionId = kya.ActionId,
                                    IsInitiated = kya.StatusId == NAStatusId.Initiated ? true : false,
                                    IsForwarded = kya.StatusId == NAStatusId.Forwarded ? true : false,
                                    IsApproved = kya.StatusId == NAStatusId.Approved ? true : false
                                });

                    return data.ToDataSourceResult(request);
                }
            }
        }

        public DataSourceResult GetChallanReportDataAsDataSource(DataSourceRequest request, ChallanViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                model.BankId = userInfo.OptionalId != null ? userInfo.OptionalId : (model.BankId != null ? model.BankId : null);

                //model.IsActive = model.StatusId == 3 ? false : true;
                if (model.StatusId == 3) model.IsActive = false;
                if (model.StatusId == 1) model.IsVerified = true;
                if (model.StatusId == 2) model.IsVerified = false;

                var list = (from challan in dbContext.Challan_Master
                            join bank in dbContext.BankMsts on challan.Bank_Id equals bank.bankId
                            where (model.Id == null || challan.Id == model.Id) && (DepartmentList.Contains(challan.Department_Id))
                            && (model.DepartmentId == null || challan.Department_Id == model.DepartmentId)
                            && (model.StartDate == null || DbFunctions.TruncateTime(challan.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(challan.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            && (model.BankId == null || challan.Bank_Id == model.BankId)
                            && (model.IsVerified == null || challan.Is_Verified == model.IsVerified)
                            && (model.IsActive == null || challan.Is_Active == model.IsActive)
                            select new ChallanViewModel
                            {
                                Id = challan.Id,
                                ChallanId = challan.Challan_Id,
                                RegistrationId = challan.Rid,
                                DepartmentId = challan.Department_Id,
                                Department = challan.Department_Id != null ? dbContext.DepartmentMsts.FirstOrDefault(c => c.departmentId == challan.Department_Id).departmentName : string.Empty,
                                SectorId = challan.Sector_Id,
                                Sector = challan.Sector_Id != null ? dbContext.SectorMsts.FirstOrDefault(s => s.sectorId == challan.Sector_Id).sectorName : string.Empty,
                                BlockId = challan.Block_Id,
                                Block = challan.Block_Id != null ? dbContext.BlockMsts.FirstOrDefault(b => b.blockId == challan.Block_Id).blockName : string.Empty,
                                PlotNo = challan.Plot_No,
                                Applicant = (challan.Allottee == null && challan.Rid != null) ? dbContext.ApplicationDetails.FirstOrDefault(a => a.registrationId == challan.Rid).tFirstName : challan.Allottee,
                                CorrespondAddress = challan.Address,
                                //Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : (aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName),
                                MobileNo = challan.Mobile_No,
                                PropertyNo = (challan.Sector_Id != null ? dbContext.SectorMsts.FirstOrDefault(s => s.sectorId == challan.Sector_Id).sectorName : string.Empty) + "/" + (challan.Block_Id != null ? dbContext.BlockMsts.FirstOrDefault(b => b.blockId == challan.Block_Id).blockName : string.Empty) + "-" + challan.Plot_No,
                                //PropertyNo = property.SectorMst.sectorName + "/" + property.blockId == null ? Constants.NA : property.BlockMst.blockName + "-" + property.propertyNo,
                                GeneratedDate = challan.Generate_Date,
                                CreateDate = challan.Created_Date,
                                ChallanContent = challan.Content,
                                IsActive = challan.Is_Active,
                                IsCancelled = challan.Is_Active == true ? false : true,
                                IsVerified = challan.Is_Verified,
                                ChallanStatus = challan.Is_Active == false ? "Canceled" : (challan.Is_Verified == true ? "Verified" : "Not Verified"),
                                RequestId = challan.ServiceRequestNo,
                                ServiceRequestId = challan.ServiceRequestNo,
                                IsBankUserId = userInfo.OptionalId != null ? true : false,
                                BankId = challan.Bank_Id,
                                BankName = challan.Bank_Id == null ? string.Empty : dbContext.BankMsts.FirstOrDefault(b => b.bankId == challan.Bank_Id).bankName,
                                Amount = dbContext.Challan_Trans.Where(t => t.Challan_Master_Id == challan.Id).Sum(s => s.Amount)
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetDemandNotesReportDataAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var idlist = (from demand in dbContext.DemandNoteDetails
                              select new
                              {
                                  Id = dbContext.DemandNoteDetails.Where(m => m.RegistrationId == demand.RegistrationId).OrderByDescending(c => c.Id).FirstOrDefault().Id
                              }).ToList();
                List<int> intlist = idlist.Select(k => (int)k.Id).ToList();

                var list = (from demand in dbContext.DemandNoteDetails
                            join alotment in dbContext.AllotmentMasters on demand.RegistrationId equals alotment.rid
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where DepartmentList.Contains(demand.DepartmentId)
                              && intlist.Contains(demand.Id)
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
                return list != null ? list.ToDataSourceResult(request) : null;
            }
        }

        public DataSourceResult GetNDCReportDataAsDataSource(DataSourceRequest request, NDCVeiwModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<NDCVeiwModel> idList = new List<NDCVeiwModel>();
                idList = (from ndcmst in dbContext.NDCDetailMsts
                          select new NDCVeiwModel
                          {
                              Id = dbContext.NDCDetailMsts.Where(m => m.RegistrationId == ndcmst.RegistrationId).OrderByDescending(c => c.Id).FirstOrDefault().Id
                          }).ToList();
                List<int> intList = idList.Select(k => (int)k.Id).ToList();

                var data = (from ndc in dbContext.NDCDetailMsts
                            where intList.Contains(ndc.Id)
                            && (model.DepartmentId == null || ndc.DepartmentId == model.DepartmentId)
                            && (model.StatusId == null || ndc.StatusId == model.StatusId)
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


        public DataSourceResult GetServiceTimelineReportsAsDataSource(DataSourceRequest request, ServiceViewModel model)
        {
            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                List<ServiceViewModel> list;
                if (model.ActionType == "All")
                {
                    list = connection.Query<ServiceViewModel>("Sp_GetDepartmentRequestCountTimeLine", new { DepartmentId = model.DepartmentId, StartDate = model.StartDate, EndDate = model.EndDate, type = 1, RequestThrough = model.RequestThrough }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
                else if (model.ActionType == "DepartmentWise")
                {
                    list = connection.Query<ServiceViewModel>("Sp_GetDepartmentRequestCountTimeLine", new { DepartmentId = model.DepartmentId, StartDate = model.StartDate, EndDate = model.EndDate, type = 2, RequestThrough = model.RequestThrough }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
                else 
                {
                    list = connection.Query<ServiceViewModel>("Sp_GetServiceDetailsByServiceId", new { DepartmentId = model.DepartmentId, StartDate = model.StartDate, EndDate = model.EndDate, type = model.ActionTypeId, ServiceId = model.ServiceId, RequestThrough = model.RequestThrough }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }
                
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetCitizenCharterTimelineList(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from timeline in dbContext.CitizenService_Master
                            where timeline.Status == 1 && timeline.Deptt_Id <= 7
                            //orderby timeline.Deptt_Id ascending
                            select new ServiceViewModel
                            {
                                Id = timeline.Id,
                                DepartmentId = timeline.Deptt_Id,
                                Department = dbContext.DepartmentMsts.FirstOrDefault(m => m.departmentId == timeline.Deptt_Id).departmentName,
                                ServiceId = timeline.service_id,
                                ServiceName = timeline.ServiceName,
                                Timeline = timeline.Timeline,
                                Amount = timeline.Amount
                            }).OrderBy(o => o.DepartmentId);
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetSMSLogList(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from sms in dbContext.SMSServicesMsts
                            where sms.SMSType == "Reminder"
                            select new SMSLogViewModel
                            {
                                Id = sms.Id,
                                RegistrationId = sms.RegistrationId,
                                MobileNo = sms.MobileNo,
                                SMSType = sms.SMSType,
                                Heading = sms.Heading,
                                Message = sms.Message,
                                StatusMsg = sms.Status,
                                SenderId = sms.CreatedBy,
                                Sender = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == sms.CreatedBy).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == sms.CreatedBy).LastName,
                                SentDate = sms.SentDate,
                                CreatedBy = sms.CreatedBy,
                                CreatedDate = sms.CreatedDate,
                                Sector = null
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public int SendReportsInExcelFormat(PropertyViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var datalist = (from functional in dbContext.FunctionalDetails
                                join status in dbContext.StatusMasters on functional.StatusId equals status.Id
                                join allotment in dbContext.AllotmentMasters on functional.Rid equals allotment.rid
                                join property in dbContext.SchemePropTrans on allotment.propertyId equals property.propertyId
                                where DepartmentList.Contains(allotment.departmentId)
                                && allotment.departmentId == model.DepartmentId
                                select new FunctionalModel
                                {
                                    RequestNo = functional.RequestNo,
                                    RId = functional.Rid,
                                    CreatedDate = functional.CreatedDate,
                                    ApproveDate = functional.ApproveDate,
                                    FunctionalDate = functional.FunctionalDate,
                                    //PropertyNumber = functional.PropertyNumber,
                                    PropertyNumber = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo,
                                    Status = status.Status,
                                    FirstName = allotment.ApplicationDetail.tFirstName,
                                    MiddleName = allotment.ApplicationDetail.tMiddleName,
                                    LastName = allotment.ApplicationDetail.tLastName,
                                    DepttName = allotment.DepartmentMst.departmentName,
                                    SchemeName = allotment.SchemeMst.schemeName,
                                    IsFunctional = functional.Functional
                                }).ToList();
                if (datalist != null)
                {
                    ExcelPackage ExcelPkg = new ExcelPackage();  
                    ExcelWorksheet wsSheet1 = ExcelPkg.Workbook.Worksheets.Add("Functional Report");  
                    using(ExcelRange Rng = wsSheet1.Cells[1, 1, 1, 7]) {  
                        Rng.Value = "Functional Institutional Property (NOIDA)";  
                        Rng.Style.Font.Size = 16;  
                        Rng.Style.Font.Bold = true;  
                        Rng.Style.Font.Italic = true;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Rng.Merge = true;
                    }  
                    wsSheet1.Protection.IsProtected = false;  
                    wsSheet1.Protection.AllowSelectLockedCells = false; 
                    wsSheet1.Cells[2, 1].Value = "S.No";
                    wsSheet1.Cells[2, 2].Value = "Registration Id";
                    wsSheet1.Cells[2, 3].Value = "Department";
                    wsSheet1.Cells[2, 4].Value = "Allottee";
                    wsSheet1.Cells[2, 5].Value = "Functional date";
                    wsSheet1.Cells[2, 6].Value = "Status";
                    wsSheet1.Cells[2, 7].Value = "Is Functional";
                    wsSheet1.Cells[2, 1, 2, 7].Style.Font.Bold = true;
                    int recordIndex = 3;
                    foreach (var data in datalist)
                    {
                        wsSheet1.Cells[recordIndex, 1].Value = (recordIndex - 2).ToString();
                        wsSheet1.Cells[recordIndex, 2].Value = data.RId;
                        wsSheet1.Cells[recordIndex, 3].Value = data.DepttName;
                        wsSheet1.Cells[recordIndex, 4].Value = data.FirstName;
                        wsSheet1.Cells[recordIndex, 5].Style.Numberformat.Format = "yyyy-mm-dd";
                        wsSheet1.Cells[recordIndex, 5].Value = data.FunctionalDate;
                        wsSheet1.Cells[recordIndex, 6].Value = data.Status;
                        wsSheet1.Cells[recordIndex, 7].Value = data.IsFunctional;
                        recordIndex++;
                    }
                    wsSheet1.Column(2).AutoFit();
                    wsSheet1.Column(3).AutoFit();
                    wsSheet1.Column(4).AutoFit();
                    wsSheet1.Column(5).AutoFit();
                    wsSheet1.Column(6).AutoFit();
                    wsSheet1.Column(7).AutoFit();
                    ExcelPkg.SaveAs(new FileInfo("D:\\FunctionalReportPackage.xlsx"));
                    //ExcelPkg.SaveAs(new FileInfo("D:\\TFSProjects\\NoidaAuthority\\Source\\NA.PMS\\NA.PMS.Web\\UploadFiles\\FunctionalReportPackage.xlsx"));
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + "Test")))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + "Test"));
                    }
                    var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + "FunctionalReportPackage.xlsx");
                    var memory = new MemoryStream();
                    ExcelPkg.SaveAs(memory);

                    //File.Create(ConfigurationManager.AppSettings["UploadFilePath"].ToString() + "FunctionalReportPackage.xlsx", 1000);

                    //HttpPostedFileBase fb; 
                    //fb.SaveAs(ConfigurationManager.AppSettings["UploadFilePath"].ToString() + "FunctionalReportPackage.xlsx");
                    //ExcelPkg.SaveAs(ConfigurationManager.AppSettings["UploadFilePath"].ToString() + "FunctionalReportPackage.xlsx");
                    //using (var memoryStream = new MemoryStream())
                    //{
                    //    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //    HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=FunctionalReportPackage.xlsx");
                    //    ExcelPkg.SaveAs(memoryStream);
                    //    memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                    //    HttpContext.Current.Response.Flush();
                    //    HttpContext.Current.Response.End();
                    //}
                    ArrayList list = new ArrayList();
                    list.Add(ExcelPkg);
                    ApplicationHelper.SendEmailWithAttachedFiles("supportnoida@noidaauthorityonline.com", "NOIDA", "skumar@svam.com", "", "", "send file", "Hello", "excel", list);
                    //ApplicationHelper.SendEmailWithAttachedFiles("samestien@gmail.com", "skumar", "skumar@svam.com", "", "", "send file", "Hello", "excel", null);
                    flag = ReturnType.Success;
                }
                return flag;
            }
        }


        public ExcelPackage DownloadReportsInExcelFormat(PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var datalist = (from functional in dbContext.FunctionalDetails
                                join status in dbContext.StatusMasters on functional.StatusId equals status.Id
                                join allotment in dbContext.AllotmentMasters on functional.Rid equals allotment.rid
                                join property in dbContext.SchemePropTrans on allotment.propertyId equals property.propertyId
                                where DepartmentList.Contains(allotment.departmentId)
                                && allotment.departmentId == model.DepartmentId
                                select new FunctionalModel
                                {
                                    RequestNo = functional.RequestNo,
                                    RId = functional.Rid,
                                    CreatedDate = functional.CreatedDate,
                                    ApproveDate = functional.ApproveDate,
                                    FunctionalDate = functional.FunctionalDate,
                                    //PropertyNumber = functional.PropertyNumber,
                                    PropertyNumber = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo,
                                    Status = status.Status,
                                    FirstName = allotment.ApplicationDetail.tFirstName,
                                    MiddleName = allotment.ApplicationDetail.tMiddleName,
                                    LastName = allotment.ApplicationDetail.tLastName,
                                    DepttName = allotment.DepartmentMst.departmentName,
                                    SchemeName = allotment.SchemeMst.schemeName,
                                    IsFunctional = functional.Functional
                                }).ToList();
                if (datalist != null)
                {
                    ExcelPackage ExcelPkg = new ExcelPackage();
                    ExcelWorksheet wsSheet1 = ExcelPkg.Workbook.Worksheets.Add("Functional Report");
                    using (ExcelRange Rng = wsSheet1.Cells[1, 1, 1, 7])
                    {
                        Rng.Value = "Functional Institutional Property (NOIDA)";
                        Rng.Style.Font.Size = 16;
                        Rng.Style.Font.Bold = true;
                        Rng.Style.Font.Italic = true;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Rng.Merge = true;
                    }
                    wsSheet1.Protection.IsProtected = false;
                    wsSheet1.Protection.AllowSelectLockedCells = false;
                    wsSheet1.Cells[2, 1].Value = "S.No";
                    wsSheet1.Cells[2, 2].Value = "Registration Id";
                    wsSheet1.Cells[2, 3].Value = "Department";
                    wsSheet1.Cells[2, 4].Value = "Allottee";
                    wsSheet1.Cells[2, 5].Value = "Functional date";
                    wsSheet1.Cells[2, 6].Value = "Status";
                    wsSheet1.Cells[2, 7].Value = "Is Functional";
                    wsSheet1.Cells[2, 1, 2, 7].Style.Font.Bold = true;
                    int recordIndex = 3;
                    foreach (var data in datalist)
                    {
                        wsSheet1.Cells[recordIndex, 1].Value = (recordIndex - 2).ToString();
                        wsSheet1.Cells[recordIndex, 2].Value = data.RId;
                        wsSheet1.Cells[recordIndex, 3].Value = data.DepttName;
                        wsSheet1.Cells[recordIndex, 4].Value = data.FirstName;
                        wsSheet1.Cells[recordIndex, 5].Style.Numberformat.Format = "yyyy-mm-dd";
                        wsSheet1.Cells[recordIndex, 5].Value = data.FunctionalDate;
                        wsSheet1.Cells[recordIndex, 6].Value = data.Status;
                        wsSheet1.Cells[recordIndex, 7].Value = data.IsFunctional;
                        recordIndex++;
                    }
                    wsSheet1.Column(2).AutoFit();
                    wsSheet1.Column(3).AutoFit();
                    wsSheet1.Column(4).AutoFit();
                    wsSheet1.Column(5).AutoFit();
                    wsSheet1.Column(6).AutoFit();
                    wsSheet1.Column(7).AutoFit();
                    ExcelPkg.SaveAs(new FileInfo("D:\\FunctionalReportPackage.xlsx"));
                    //if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" +"Test")))
                    //{
                    //    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + "Test"));
                    //}
                    //var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + "FunctionalReportPackage.xlsx");

                    //File.Create(ConfigurationManager.AppSettings["UploadFilePath"].ToString() + "FunctionalReportPackage.xlsx", 1000);

                    //HttpPostedFileBase fb; 
                    //fb.SaveAs(ConfigurationManager.AppSettings["UploadFilePath"].ToString() + "FunctionalReportPackage.xlsx");
                    //ExcelPkg.SaveAs(ConfigurationManager.AppSettings["UploadFilePath"].ToString() + "FunctionalReportPackage.xlsx");
                    //using (var memoryStream = new MemoryStream())
                    //{
                    //    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //    HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=FunctionalReportPackage.xlsx");
                    //    ExcelPkg.SaveAs(memoryStream);
                    //    memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                    //    HttpContext.Current.Response.Flush();
                    //    HttpContext.Current.Response.End();
                    //}
                    //ArrayList list = new ArrayList();
                    //list.Add(ExcelPkg);
                    ApplicationHelper.SendEmailWithAttachedFiles("supportnoida@noidaauthorityonline.com", "NOIDA", "skumar@svam.com", "", "", "send file", "Hello", "excel", null);
                    //ApplicationHelper.SendEmailWithAttachedFiles("samestien@gmail.com", "skumar", "skumar@svam.com", "", "", "send file", "Hello", "excel", null);
                    return ExcelPkg;
                }
                else return null;
            }
        }


        public string GetServiceReportForAllDepartment()
        {
            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                IList<ServiceViewModel> ServiceDetails;
                ServiceDetails = connection.Query<ServiceViewModel>("Sp_ServiceReportDepartment_Report", new { DepartmentId = 1 }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                var result = ServiceDetails[0].HtmlReport;
                return result;
            }
        }


        public string GetServiceReportLetter()
        {
            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                IList<ServiceViewModel> ServiceDetails;
                ServiceDetails = connection.Query<ServiceViewModel>("Sp_ServiceReportDepartment_Report_letter", new { DepartmentId = 1 }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                var result = ServiceDetails[0].HtmlReport;
                return result;
            }
        }
    }
}
