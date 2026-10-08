using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Model.Property;
using NA.PMS.Web.Models;

namespace NA.PMS.Repository.Property
{
    public class MergeSplitPropertyRepository : IMergeSplitPropertyRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();

        public MergeSplitPropertyRepository()
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
        // Get All Merged properties  
        public DataSourceResult GetAllMergeProperties(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from mergeProp in dbContext.Merge_Property_Master
                              join prop in dbContext.SchemePropTrans on mergeProp.Property_Id equals prop.propertyId
                              join dept in dbContext.DepartmentMsts on prop.departmentId equals dept.departmentId
                              //join sec in dbContext.SectorMsts on prop.sectorId.Value equals sec.sectorId
                              //join block in dbContext.BlockMsts on prop.blockId.Value equals block.blockId
                              //join proptype in dbContext.PropertyTypeMsts on prop.propertyTypeId equals proptype.propertyTypeId
                              join userMst in dbContext.UmUserMasters on mergeProp.Assigned_To equals userMst.UserRefId
                              join status in dbContext.StatusMasters on mergeProp.Status equals status.Id
                              where mergeProp.Is_Active == true && mergeProp.Type == Constants.Amalgamation
                              select new MergeSplitPropModel
                              {
                                  Id = mergeProp.Id,
                                  Rid = mergeProp.Rid,
                                  PropertyId = prop.propertyId,
                                  DepartmentName = dept.departmentName,
                                  PropertyNumber = prop.propertyNo,//sec.sectorName + "/" + block.blockName + "-" + prop.propertyNo,
                                  RequestDate = mergeProp.Request_Date,
                                  ApprovedDate = mergeProp.Approved_Date,
                                  AssignedTo = userMst.FirstName + " " + userMst.LastName,
                                  Status = status.Status
                              });

                var allMergedProp = new DataSourceResult();
                if (result.Any())
                    allMergedProp = result.ToDataSourceResult(request);
                return allMergedProp;
            }
        }

        // Get All Merged properties  
        public DataSourceResult GetAllSplitProperties(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from mergeProp in dbContext.Merge_Property_Master
                              join prop in dbContext.SchemePropTrans on mergeProp.Property_Id equals prop.propertyId
                              join dept in dbContext.DepartmentMsts on prop.departmentId equals dept.departmentId
                              //join sec in dbContext.SectorMsts on prop.sectorId.Value equals sec.sectorId
                              //join block in dbContext.BlockMsts on prop.blockId.Value equals block.blockId
                              //join proptype in dbContext.PropertyTypeMsts on prop.propertyTypeId equals proptype.propertyTypeId
                              join userMst in dbContext.UmUserMasters on mergeProp.Assigned_To equals userMst.UserRefId
                              join status in dbContext.StatusMasters on mergeProp.Status equals status.Id
                              where mergeProp.Is_Active == true && mergeProp.Type == Constants.Deamalgamation
                              select new MergeSplitPropModel
                              {
                                  Id = mergeProp.Id,
                                  Rid = mergeProp.Rid,
                                  PropertyId = prop.propertyId,
                                  DepartmentName = dept.departmentName,
                                  PropertyNumber = prop.propertyNo, //sec.sectorName + "/" + block.blockName + "-" + prop.propertyNo,
                                  RequestDate = mergeProp.Request_Date,
                                  ApprovedDate = mergeProp.Approved_Date,
                                  AssignedTo = userMst.FirstName + " " + userMst.LastName,
                                  Status = status.Status
                              });

                var allMergedProp = new DataSourceResult();
                if (result.Any())
                    allMergedProp = result.ToDataSourceResult(request);
                return allMergedProp;
            }
        }

        // Get All Merged Requests  
        public DataSourceResult GetAllMergeRequests(DataSourceRequest request)
        {
            var allMergedProp = new DataSourceResult();
            if (userInfo != null)
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    var result = (from mergeProp in dbContext.Merge_Property_Master
                                  join prop in dbContext.SchemePropTrans on mergeProp.Property_Id equals prop.propertyId
                                  join dept in dbContext.DepartmentMsts on prop.departmentId equals dept.departmentId
                                  join userMst in dbContext.UmUserMasters on mergeProp.Assigned_To equals userMst.UserRefId
                                  join status in dbContext.StatusMasters on mergeProp.Status equals status.Id
                                  where mergeProp.Is_Active == true && mergeProp.Type == Constants.Amalgamation
                                      //&& mergeProp.Status != Constants.InProgress 
                                        && mergeProp.Status != Constants.Cancelled
                                        && mergeProp.Assigned_To == userInfo.UserID
                                  select new MergeSplitPropModel
                                  {
                                      Id = mergeProp.Id,
                                      Rid = mergeProp.Rid,
                                      PropertyId = prop.propertyId,
                                      DepartmentName = dept.departmentName,
                                      PropertyNumber = prop.propertyNo,
                                      RequestDate = mergeProp.Request_Date,
                                      ApprovedDate = mergeProp.Approved_Date,
                                      AssignedTo = userMst.FirstName + " " + userMst.LastName,
                                      Status = status.Status
                                  });

                    if (result.Any())
                        allMergedProp = result.ToDataSourceResult(request);

                }
            }
            return allMergedProp;
        }
        // Get All Split Requests  
        public DataSourceResult GetAllSplitRequests(DataSourceRequest request)
        {
            var allMergedProp = new DataSourceResult();
            if (userInfo != null)
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    var result = (from mergeProp in dbContext.Merge_Property_Master
                                  join prop in dbContext.SchemePropTrans on mergeProp.Property_Id equals prop.propertyId
                                  join dept in dbContext.DepartmentMsts on prop.departmentId equals dept.departmentId
                                  join userMst in dbContext.UmUserMasters on mergeProp.Assigned_To equals userMst.UserRefId
                                  join status in dbContext.StatusMasters on mergeProp.Status equals status.Id
                                  where mergeProp.Is_Active == true && mergeProp.Type == Constants.Deamalgamation
                                      //&& mergeProp.Status != Constants.InProgress 
                                        && mergeProp.Status != Constants.Cancelled
                                        && mergeProp.Assigned_To == userInfo.UserID
                                  select new MergeSplitPropModel
                                  {
                                      Id = mergeProp.Id,
                                      Rid = mergeProp.Rid,
                                      PropertyId = prop.propertyId,
                                      DepartmentName = dept.departmentName,
                                      PropertyNumber = prop.propertyNo,
                                      RequestDate = mergeProp.Request_Date,
                                      ApprovedDate = mergeProp.Approved_Date,
                                      AssignedTo = userMst.FirstName + " " + userMst.LastName,
                                      Status = status.Status
                                  });

                    if (result.Any())
                        allMergedProp = result.ToDataSourceResult(request);

                }
            }
            return allMergedProp;
        }

        // Add Merge Request
        public bool AddMergeRequest(int rId)
        {

            return true;
        }

        // Add Split Request
        public bool AddSplitRequest(int rId)
        {
            return true;
        }

        // Get All RIDs forSplit or Merger
        public List<SelectListItem> GetAllRIDsForSplitMerge(int departmentId, int sectorId, int blockId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var allRIDsForSplitMerge = (from prop in dbContext.SchemePropTrans
                                            join allotMst in dbContext.AllotmentMasters on prop.propertyId equals allotMst.propertyId
                                            where allotMst.isActive == 1 && allotMst.isStatus == "Approved" && allotMst.departmentId == departmentId
                                            && prop.sectorId == sectorId && prop.blockId == blockId
                                            && prop.IsActive == true //Resolved Bug#277
                                            select new SelectListItem
                                            {
                                                Value = allotMst.rid.ToString(),
                                                Text = allotMst.rid.ToString()
                                            }).ToList();


                return allRIDsForSplitMerge;
            }
        }

        // Get All RIDs forSplit or Merger
        public DataSourceResult GetAllRIDsForSplitMerge(DataSourceRequest Req, int departmentId, int sectorId, int blockId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var allRIDsForSplitMerge = (from prop in dbContext.SchemePropTrans
                                            join allotMst in dbContext.AllotmentMasters on prop.propertyId equals allotMst.propertyId
                                            where allotMst.isActive == 1 && allotMst.isStatus == "Approved" && allotMst.departmentId == departmentId
                                            && prop.sectorId == sectorId && prop.blockId == blockId
                                            && prop.IsActive == true //Resolved Bug#277
                                            select new MergeSplitRidSelectionDDList
                                            {
                                                id = allotMst.rid,
                                                text = allotMst.rid.ToString(),
                                                BlockId = prop.blockId != null ? (int)prop.blockId : 0
                                            });
                return allRIDsForSplitMerge.ToDataSourceResult(Req);
            }
        }

        // Get All RIDs for Split
        public List<SelectListItem> GetAllRidsForSplit(int departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var allRIDsForSplit = (from mergePropMst in dbContext.Merge_Property_Master
                //                       where mergePropMst.Is_Active == true && mergePropMst.Type == Constants.Amalgamation && mergePropMst.DepartmentId == departmentId
                //                       //&& mergePropMst.SectorId == sectorId && mergePropMst.BlockId == blockId
                //                       select new SelectListItem
                //                       {
                //                           Value = mergePropMst.Rid.ToString(),
                //                           Text = mergePropMst.Rid.ToString()
                //                       }).ToList();

                var allRIDsForSplit = (from mergePropMst in dbContext.Merge_Property_Master
                                       join allotMst in dbContext.AllotmentMasters on mergePropMst.Rid equals allotMst.rid
                                       where mergePropMst.Is_Active == true && mergePropMst.Type == Constants.Amalgamation
                                       && mergePropMst.DepartmentId == departmentId && mergePropMst.Status == Constants.Approved
                                       && allotMst.isActive == 1 && allotMst.isStatus == "Approved"
                                       //&& mergePropMst.SectorId == sectorId && mergePropMst.BlockId == blockId
                                       select new SelectListItem
                                       {
                                           Value = mergePropMst.Rid.ToString(),
                                           Text = mergePropMst.Rid.ToString()
                                       }).ToList();

                return allRIDsForSplit;
            }
        }

        // Get All RIDs for Split
        public List<MergeSplitPropertyGrid> GetPropertiesToSplitByRid(int requestId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var allRIDsForSplit = (from mergePropMst in dbContext.Merge_Property_Master
                                       join mergePropTran in dbContext.Merge_Property_Trans on mergePropMst.Id equals mergePropTran.Request_Id
                                       join schPropTran in dbContext.SchemePropTrans on mergePropTran.Child_Property_Id equals schPropTran.propertyId
                                       join dept in dbContext.DepartmentMsts on schPropTran.departmentId equals dept.departmentId
                                       where mergePropMst.Is_Active == true && mergePropMst.Type == Constants.Amalgamation && mergePropMst.Id == requestId
                                       //&& mergePropTran.Request_Id == 
                                       select new MergeSplitPropertyGrid
                                       {
                                           Rid = mergePropTran.Child_Rid.Value,
                                           PropertyId = mergePropTran.Child_Property_Id.Value,
                                           DepartmentId = mergePropMst.DepartmentId.Value,
                                           DepartmentName = dept.departmentName,
                                           PropertyNumber = schPropTran.propertyNo,
                                           CoveredArea = schPropTran.coveredArea ?? 0,
                                           ActualArea = schPropTran.actualArea ?? 0,
                                           TotalArea = schPropTran.totalArea ?? 0,
                                           TotalPropertyCost = schPropTran.totalPropertyCost ?? 0,
                                       }).ToList();

                return allRIDsForSplit;
            }
        }

        // Get RequestId by  RID 
        public int GetRequestIdByRid(int rid)
        {
            var requestId = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.Merge_Property_Master.FirstOrDefault(x => x.Rid == rid && x.Is_Active == true && x.Status == Constants.Approved);
                if (result != null)
                    requestId = result.Id;

                return requestId;
            }
        }

        // Add property to merge/split
        public bool AddUpdatePropertyToMergeSplit(int rid, int propertyId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.Merge_Property_Trans.FirstOrDefault(x => x.Child_Rid == rid);
                if (result == null)
                {
                    var mergeProp = new Merge_Property_Trans();
                    mergeProp.Child_Rid = rid;
                    mergeProp.Child_Property_Id = propertyId;
                    mergeProp.Type = Constants.Amalgamation;
                    mergeProp.Created_By = userInfo.UserID;
                    mergeProp.Created_Date = DateTime.Now.Date;

                    dbContext.Merge_Property_Trans.Add(mergeProp);
                    var count = dbContext.SaveChanges();
                    if (count > 0)
                        flag = true;
                }
                else
                {
                    result.Child_Rid = rid;
                    result.Child_Property_Id = propertyId;
                    result.Type = Constants.Amalgamation;
                    result.Created_By = userInfo.UserID;
                    result.Created_Date = DateTime.Now.Date;

                    dbContext.Merge_Property_Trans.AddOrUpdate(result);
                    var count = dbContext.SaveChanges();
                    if (count > 0)
                        flag = true;
                }


                return flag;
            }
        }

        // Remove property to merge/split
        public bool RemovePropertyFromMergeSplit(int rid)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.Merge_Property_Trans.FirstOrDefault(x => x.Child_Rid == rid);
                if (result != null)
                {
                    dbContext.Merge_Property_Trans.Remove(result);
                    var count = dbContext.SaveChanges();
                    if (count > 0)
                        flag = true;
                }
            }
            return flag;

        }
        // Get Properties to merge
        public List<MergeSplitPropertyGrid> GetPropertiesToMergeByRequestId(int requestId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from mergePropTrans in dbContext.Merge_Property_Trans
                              join mergePropMast in dbContext.Merge_Property_Master on mergePropTrans.Merge_Rid equals mergePropMast.Rid
                              join schPropTran in dbContext.SchemePropTrans on mergePropTrans.Child_Property_Id equals schPropTran.propertyId
                              join allomentMaster in dbContext.AllotmentMasters on mergePropTrans.Child_Rid equals allomentMaster.rid
                              join dept in dbContext.DepartmentMsts on schPropTran.departmentId equals dept.departmentId
                              where mergePropTrans.Type == Constants.Amalgamation && mergePropMast.Id == requestId && mergePropTrans.Is_Active == true
                              && mergePropTrans.Request_Id == requestId
                              select new MergeSplitPropertyGrid
                          {
                              Rid = mergePropTrans.Child_Rid.Value,
                              PropertyId = mergePropTrans.Child_Property_Id.Value,
                              DepartmentName = dept.departmentName,
                              DepartmentId = schPropTran.departmentId.Value,
                              PropertyNumber = schPropTran.propertyNo,
                              CoveredArea = schPropTran.coveredArea ?? 0,
                              ActualArea = schPropTran.actualArea ?? 0,
                              TotalArea = schPropTran.totalArea ?? 0,
                              TotalPropertyCost = schPropTran.totalPropertyCost ?? 0

                          }).ToList();
                return result;
            }
        }

        // Get Properties to Split
        public List<MergeSplitPropertyGrid> GetPropertiesToSplitByRequestId(int requestId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from mergePropTrans in dbContext.Merge_Property_Trans
                              join mergePropMast in dbContext.Merge_Property_Master on mergePropTrans.Merge_Rid equals mergePropMast.Rid
                              join schPropTran in dbContext.SchemePropTrans on mergePropTrans.Child_Property_Id equals schPropTran.propertyId
                              join allomentMaster in dbContext.AllotmentMasters on mergePropTrans.Child_Rid equals allomentMaster.rid
                              join dept in dbContext.DepartmentMsts on schPropTran.departmentId equals dept.departmentId
                              where mergePropTrans.Type == Constants.Deamalgamation && mergePropMast.Id == requestId && mergePropTrans.Is_Active == true
                              select new MergeSplitPropertyGrid
                              {
                                  Rid = mergePropTrans.Child_Rid.Value,
                                  PropertyId = mergePropTrans.Child_Property_Id.Value,
                                  DepartmentName = dept.departmentName,
                                  DepartmentId = schPropTran.departmentId.Value,
                                  PropertyNumber = schPropTran.propertyNo,
                                  //SchemeName = sch.schemeName,
                                  //PropertyType = protype.propertyTypeName,
                                  //SectorName = sec.sectorName,
                                  // BlockName = block.blockName,
                                  CoveredArea = schPropTran.coveredArea,
                                  ActualArea = schPropTran.actualArea,
                                  TotalArea = schPropTran.totalArea,
                                  TotalPropertyCost = schPropTran.totalPropertyCost,
                                  //ApplicantName = appDetails.firstName + " " + appDetails.middleName + " " + appDetails.lastName

                              }).ToList();

                //var lstPropertiesToMerge = new DataSourceResult();
                //if (result.Any())
                //    lstPropertiesToMerge = result.ToDataSourceResult(request);
                return result;
            }
        }

        // Merge Request
        public bool MergeRequest(string rids, int userId, decimal charges, int status, int type, int? requestNo, string comment)
        {
            bool flag = false;
            var param1 = new SqlParameter
            {
                ParameterName = "Rid",
                Value = rids
            };
            var param2 = new SqlParameter
            {
                ParameterName = "Charges",
                Value = charges
            };
            var param3 = new SqlParameter
            {
                ParameterName = "Status",
                Value = status
            };
            var param4 = new SqlParameter
            {
                ParameterName = "AssignedTo",
                Value = userId
            };
            var param5 = new SqlParameter
            {
                ParameterName = "Type",
                Value = type
            };
            var param6 = new SqlParameter
            {
                ParameterName = "RequestNo",
                Value = requestNo
            };
            var param7 = new SqlParameter
            {
                ParameterName = "comment",
                Value = comment ?? string.Empty
            };
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.Database.SqlQuery<int>("exec Sp_AmalgamateProperties @Rid, @Charges, @Status, @AssignedTo, @Type, @RequestNo, @comment", param1, param2, param3, param4, param5, param6, param7).FirstOrDefault();
                //var result = dbContext.Database.SqlQuery<int>(string.Format("exec Sp_AmalgamateProperties {0}, {1}, {2}, {3}, {4}, {5}, {6}", param1, param2, param3, param4, param5, param6, param7)).FirstOrDefault();
                flag = result == 1 ? true : false;
            }
            return flag;
        }

        // Merge Request
        public bool SplitRequest(string rids, int rId, int userId, decimal charges, int status, int type, int? requestNo, string comment)
        {
            bool flag = false;
            var param1 = new SqlParameter
            {
                ParameterName = "ChildRid",
                Value = rids
            };
            var param12 = new SqlParameter
            {
                ParameterName = "ParentRid",
                Value = rId
            };

            var param2 = new SqlParameter
            {
                ParameterName = "Charges",
                Value = charges
            };
            var param3 = new SqlParameter
            {
                ParameterName = "Status",
                Value = status
            };
            var param4 = new SqlParameter
            {
                ParameterName = "AssignedTo",
                Value = userId
            };
            var param5 = new SqlParameter
            {
                ParameterName = "Type",
                Value = type
            };
            var param6 = new SqlParameter
            {
                ParameterName = "RequestNo",
                Value = requestNo
            };
            var param7 = new SqlParameter
            {
                ParameterName = "comment",
                Value = comment ?? string.Empty
            };
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.Database.SqlQuery<int>("exec Sp_DeAmalgamateProperties @ChildRid, @ParentRid, @Charges, @Status, @AssignedTo, @Type, @RequestNo, @comment", param1, param12, param2, param3, param4, param5, param6, param7).FirstOrDefault();
                flag = result == 1 ? true : false;
            }
            return flag;
        }

        // Get Split Merge Request details by request id
        public AddMergeSplitRequestModel GetMergeSplitRequestDetails(int requestId)
        {
            var dataResult = new AddMergeSplitRequestModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                dataResult = (from mergeSplitMaster in dbContext.Merge_Property_Master
                              join allomentMaster in dbContext.AllotmentMasters on mergeSplitMaster.Rid equals allomentMaster.rid
                              join proptrans in dbContext.SchemePropTrans on mergeSplitMaster.Property_Id equals proptrans.propertyId
                              join sch in dbContext.SchemeMsts on proptrans.schemeId equals sch.schemeId
                              join appDetails in dbContext.ApplicationDetails on allomentMaster.applicationId equals appDetails.applicationId
                              join deptt in dbContext.DepartmentMsts on proptrans.departmentId equals deptt.departmentId
                              join sec in dbContext.SectorMsts on proptrans.sectorId equals sec.sectorId
                              join block in dbContext.BlockMsts on proptrans.blockId equals block.blockId
                              join protype in dbContext.PropertyTypeMsts on proptrans.propertyTypeId.Value equals protype.propertyTypeId
                              join userMst in dbContext.UmUserMasters on mergeSplitMaster.Assigned_To equals userMst.UserRefId
                              where mergeSplitMaster.Id == requestId && mergeSplitMaster.Is_Active == true
                              select new AddMergeSplitRequestModel
                              {
                                  Rid = mergeSplitMaster.Rid,
                                  SchemeName = sch.schemeName,
                                  PropertyType = protype.propertyTypeName,
                                  BlockName = block.blockName,
                                  BlockId = mergeSplitMaster.BlockId.Value,
                                  PropertyCost = proptrans.propertyCost ?? 0,
                                  TotalPropertyCost = proptrans.totalPropertyCost ?? 0,
                                  CoveredArea = proptrans.coveredArea ?? 0,
                                  LocationType = (from locch in dbContext.PropertyLocationChargesTrans join loc in dbContext.LocationMsts on locch.locationId equals loc.locationId where locch.propertyId == proptrans.refId && locch.IsActive == true select loc.locationName).FirstOrDefault(),
                                  DepartmentName = deptt.departmentName,
                                  SectorName = sec.sectorName,
                                  SectorId = mergeSplitMaster.SectorId.Value,
                                  PropertyNumber = sec.sectorName + "/" + block.blockName + "-" + proptrans.propertyNo,
                                  TotalArea = proptrans.totalArea ?? 0,
                                  ActualArea = proptrans.actualArea ?? 0,
                                  PropertyId = proptrans.propertyId,
                                  SchemeId = proptrans.schemeId,
                                  DepartmentId = proptrans.departmentId.Value,
                                  ApplicantName = appDetails.tFirstName + " " + appDetails.tMiddleName + " " + appDetails.tLastName,
                                  AssignedTo = userMst.FirstName + " " + userMst.MiddleName + " " + userMst.LastName,
                                  ApprovedDate = mergeSplitMaster.Approved_Date,
                                  RequestDate = mergeSplitMaster.Request_Date,
                                  Comments = mergeSplitMaster.Comment,
                                  Status = mergeSplitMaster.Status.Value,
                                  Charges = mergeSplitMaster.Charges
                              }).FirstOrDefault();

            }
            return dataResult;
        }
        // Get PropertyDetails By RID
        public SchemePropertyTransDetail GetPropertyDetailsByRid(int rId)
        {
            var lst = new SchemePropertyTransDetail();
            using (var dbContext = new NoidaPMSEntities())
            {
                var allomentMaster = dbContext.AllotmentMasters.FirstOrDefault(id => id.rid == rId);
                if (allomentMaster != null)
                {
                    lst = (from proptrans in dbContext.SchemePropTrans
                           join sch in dbContext.SchemeMsts on proptrans.schemeId equals sch.schemeId
                           join appDetails in dbContext.ApplicationDetails on allomentMaster.applicationId equals appDetails.applicationId
                           join deptt in dbContext.DepartmentMsts on proptrans.departmentId equals deptt.departmentId
                           join sec in dbContext.SectorMsts on proptrans.sectorId equals sec.sectorId
                           join block in dbContext.BlockMsts on proptrans.blockId equals block.blockId
                           join protype in dbContext.PropertyTypeMsts on proptrans.propertyTypeId.Value equals protype.propertyTypeId
                           //join schemeCost in dbContext.SchemeCostTrans on proptrans.schemeId equals schemeCost.schemeId
                           //join schemeDeptt in dbContext.SchemeDepartmentTrans on proptrans.schemeId equals schemeDeptt.schemeId
                           //join floor in dbContext.FloorMsts on proptrans.floorId equals floor.floorId
                           where proptrans.propertyId == allomentMaster.propertyId && proptrans.schemeId == allomentMaster.schemeId && proptrans.departmentId == allomentMaster.departmentId && proptrans.IsActive == true
                           select new SchemePropertyTransDetail
                           {
                               SchemeName = sch.schemeName,
                               PropertyTypeName = protype.propertyTypeName,
                               BlockName = block.blockName,
                               //FloorName = floor.floorName,
                               PropertyCost = proptrans.propertyCost,
                               TotalPropertyCost = proptrans.totalPropertyCost,
                               //CoveredArea = proptrans.coveredArea,
                               //Since Location charges and location type could be null
                               //LocationCharges = (from locch in dbContext.PropertyLocationChargesTrans where locch.propertyId == proptrans.refId && locch.IsActive == true select locch.charges).FirstOrDefault(),
                               //LocationType = (from locch in dbContext.PropertyLocationChargesTrans join loc in dbContext.LocationMsts on locch.locationId equals loc.locationId where locch.propertyId == proptrans.refId && locch.IsActive == true select loc.locationName).FirstOrDefault(),
                               DepartmentName = deptt.departmentName,
                               SectorName = sec.sectorName,
                               PropertyNo = sec.sectorName + "/" + block.blockName + "-" + proptrans.propertyNo,
                               TotalArea = proptrans.totalArea,
                               //ActualArea = proptrans.actualArea,
                               //CivilCost = proptrans.civilCost,
                               //LandRate = proptrans.landRatePerSqmt,
                               // AllotmentDate = allomentMaster.allotmentDate,
                               //InstalmentStartDate = allomentMaster.instalmentStartDate,
                               //Frequency = schemeDeptt.frequency,
                               PropertyId = proptrans.propertyId,
                               SchemeId = proptrans.schemeId,
                               DepartmentId = proptrans.departmentId,
                               //TotalnoofInstallment = (from deptt1 in dbContext.SchemeDepartmentTrans where deptt1.schemeId == proptrans.schemeId && deptt1.departmentId == proptrans.departmentId && deptt1.IsActive.Value == true select deptt1.noOfInstallments).FirstOrDefault(),
                               ApplicantName = appDetails.tFirstName + " " + appDetails.tMiddleName + " " + appDetails.tLastName
                           }).FirstOrDefault();
                }
                return lst;
            }

        }

    }
}
