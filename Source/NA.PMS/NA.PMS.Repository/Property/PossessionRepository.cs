using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NA.PMS.Common;
using NA.PMS.Model;
using NA.PMS.Model.CommonModel;
using NA.PMS.Model.Property;
using NoidaAuthority.PMS.Common;
using System.Configuration;
using System.IO;
using System.Net;
using NA.PMS.Web.Models;
using System.Web;

namespace NA.PMS.Repository
{
    public class PossessionRepository : IPossessionRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public PossessionRepository()
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

        public PropertyPossessionModel GetPossessionDetailsByRID(int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from registry in dbContext.RegistryDetails
                              join schemeprop in dbContext.SchemePropTrans on registry.AllotmentMaster.propertyId equals schemeprop.propertyId 
                              where registry.Rid == rId
                              select new PropertyPossessionModel
                              {
                                  Rid = registry.Rid,
                                  SchemeId = schemeprop.SchemeMst.schemeId,
                                  SchemeName = schemeprop.SchemeMst.schemeName,
                                  DepartmentId = schemeprop.DepartmentMst.departmentId,
                                  DepartmentName = schemeprop.DepartmentMst.departmentName,
                                  FirstName = registry.AllotmentMaster.ApplicationDetail.firstName,
                                  MiddleName = registry.AllotmentMaster.ApplicationDetail.middleName,
                                  LastName = registry.AllotmentMaster.ApplicationDetail.lastName,
                                  FatherName = registry.AllotmentMaster.ApplicationDetail.fatherHusbandName,
                                  MotherName = registry.AllotmentMaster.ApplicationDetail.motherName,
                                  SigningAuthority = registry.AllotmentMaster.ApplicationDetail.signingAuthority,
                                  RegisteredOffice = registry.AllotmentMaster.ApplicationDetail.registeredOffice,
                                  CorrespondAdd = registry.AllotmentMaster.ApplicationDetail.correspondanceAdd,
                                  Gender = registry.AllotmentMaster.ApplicationDetail.gender,
                                  PropertyId = schemeprop.propertyId,
                                  PropertyType = schemeprop.PropertyTypeMst.propertyTypeName,
                                  PropertyNumber = schemeprop.SectorMst.sectorName + "/" + schemeprop.BlockMst.blockName + "-" + schemeprop.propertyNo,
                                  Area = schemeprop.totalArea.Value,
                                  FloorAreaRange = schemeprop.FloorMst.floorName,
                                  LeaseDeedDueDate = registry.RegistryDueDate.Value,
                                  LeaseDeedExecutionDate = registry.RegistryDoneDate.Value,
                                  AllottmentDate=registry.AllotmentMaster.allotmentDate,
                                  PossessionOrderDate = (from possession in dbContext.PossessionDetails where possession.Rid == rId select possession).FirstOrDefault().PossessionOrderDate,
                                  PossessionDueDate = (from possession in dbContext.PossessionDetails where possession.Rid == rId select possession).FirstOrDefault().PossessionDueDate,
                                  IsPossessionOrderDone = (from possession in dbContext.PossessionDetails where possession.Rid == rId && possession.IsActive == true select possession).FirstOrDefault() != null,//&& possession.StatusId==NAStatusId.Approved
                                  ReqRefNo = (from possession in dbContext.PossessionDetails where possession.Rid == rId select possession).FirstOrDefault() != null ? ((from possession in dbContext.PossessionDetails where possession.Rid == rId select possession).FirstOrDefault().OnlineRequestNo != null ? (from possession in dbContext.PossessionDetails where possession.Rid == rId select possession).FirstOrDefault().OnlineRequestNo : 0) : 0,
                              }).FirstOrDefault();
                return result;

            }
        }
        // get rid details for Bulk possession order
        public PropertyPossessionModel GetBulkPossessionDetailsByRid(int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from registry in dbContext.RegistryDetails
                              join schemeprop in dbContext.SchemePropTrans on registry.AllotmentMaster.propertyId equals schemeprop.propertyId
                              where registry.Rid == rId
                              select new PropertyPossessionModel
                              {
                                  Rid = registry.Rid,
                                  SchemeId = schemeprop.SchemeMst.schemeId,
                                  SchemeName = schemeprop.SchemeMst.schemeName,
                                  DepartmentId = schemeprop.DepartmentMst.departmentId,
                                  DepartmentName = schemeprop.DepartmentMst.departmentName,
                                  FirstName = registry.AllotmentMaster.ApplicationDetail.firstName,
                                  MiddleName = registry.AllotmentMaster.ApplicationDetail.middleName,
                                  LastName = registry.AllotmentMaster.ApplicationDetail.lastName,
                                  FatherName = registry.AllotmentMaster.ApplicationDetail.fatherHusbandName,
                                  MotherName = registry.AllotmentMaster.ApplicationDetail.motherName,
                                  SigningAuthority = registry.AllotmentMaster.ApplicationDetail.signingAuthority,
                                  RegisteredOffice = registry.AllotmentMaster.ApplicationDetail.registeredOffice,
                                  CorrespondAdd = registry.AllotmentMaster.ApplicationDetail.correspondanceAdd,
                                  Gender = registry.AllotmentMaster.ApplicationDetail.gender,
                                  PropertyId = schemeprop.propertyId,
                                  PropertyType = schemeprop.PropertyTypeMst.propertyTypeName,
                                  PropertyNumber = schemeprop.SectorMst.sectorName + "/" + schemeprop.BlockMst.blockName + "-" + schemeprop.propertyNo,
                                  Area = schemeprop.totalArea.Value,
                                  FloorAreaRange = schemeprop.FloorMst.floorName,
                                  LeaseDeedDueDate = registry.RegistryDueDate.Value,
                                  LeaseDeedExecutionDate = registry.RegistryDoneDate.Value,
                                  //PossessionOrderDate = (from possession in dbContext.PossessionDetails where possession.Rid == rId select possession).FirstOrDefault().PossessionOrderDate,
                                  //PossessionDueDate = (from possession in dbContext.PossessionDetails where possession.Rid == rId select possession).FirstOrDefault().PossessionDueDate,
                                  //IsPossessionOrderDone = (from possession in dbContext.PossessionDetails where possession.Rid == rId select possession).FirstOrDefault() != null
                              }).FirstOrDefault();
                return result;

            }
        }

        public PropertyPossessionModel GetPossessionEntryDetailsByRID(int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from registry in dbContext.RegistryDetails
                              join schemeprop in dbContext.SchemePropTrans on registry.AllotmentMaster.propertyId equals schemeprop.propertyId
                              join possession in dbContext.PossessionDetails on registry.Rid equals possession.Rid
                              where registry.Rid == rId
                              select new PropertyPossessionModel
                              {
                                  Rid = registry.Rid,
                                  SchemeId = schemeprop.SchemeMst.schemeId,
                                  SchemeName = schemeprop.SchemeMst.schemeName,
                                  DepartmentId = schemeprop.DepartmentMst.departmentId,
                                  DepartmentName = schemeprop.DepartmentMst.departmentName,
                                  FirstName = registry.AllotmentMaster.ApplicationDetail.firstName,
                                  MiddleName = registry.AllotmentMaster.ApplicationDetail.middleName,
                                  LastName = registry.AllotmentMaster.ApplicationDetail.lastName,
                                  FatherName = registry.AllotmentMaster.ApplicationDetail.fatherHusbandName,
                                  MotherName = registry.AllotmentMaster.ApplicationDetail.motherName,
                                  SigningAuthority = registry.AllotmentMaster.ApplicationDetail.signingAuthority,
                                  RegisteredOffice = registry.AllotmentMaster.ApplicationDetail.registeredOffice,
                                  CorrespondAdd = registry.AllotmentMaster.ApplicationDetail.correspondanceAdd,
                                  Gender = registry.AllotmentMaster.ApplicationDetail.gender,
                                  PropertyId = schemeprop.propertyId,
                                  PropertyType = schemeprop.PropertyTypeMst.propertyTypeName,
                                  PropertyNumber = schemeprop.SectorMst.sectorName + "/" + schemeprop.BlockMst.blockName + "-" + schemeprop.propertyNo,
                                  Area = schemeprop.totalArea.Value,
                                  North = possession.North,
                                  East = possession.East,
                                  West = possession.West,
                                  South = possession.South,
                                  PossessionDate = possession.PossessionDate,
                                  AreaChange = possession.AreaChange ?? "No",
                                  PossessionBy = possession.PossessionBy,
                                  AreaChangeType = possession.AreaChangeType,
                                  ChnagedArea = possession.ChnagedArea,
                                  OneTimeExcessCharge = possession.OneTimeExcessCharge,
                                  FloorAreaRange = schemeprop.FloorMst.floorName,
                                  LeaseDeedDueDate = registry.RegistryDueDate.Value,
                                  LeaseDeedExecutionDate = registry.RegistryDoneDate.Value,
                                  PossessionOrderDate = possession.PossessionOrderDate,
                                  PossessionDueDate = possession.PossessionDueDate,
                                  IsPossessionEntryDone = possession.PossessionDate != null ? true : false //(from poss in dbContext.PossessionDetails where possession.Rid == rId select possession).FirstOrDefault() != null
                              }).FirstOrDefault();
                return result;

            }
        }

        // Get All properties for which possession has been done 
        public DataSourceResult GetAllPossessionProperties(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var result = (from possession in dbContext.PossessionDetails
                              join allotment in dbContext.AllotmentMasters on possession.Rid equals allotment.rid
                              join prop in dbContext.SchemePropTrans on allotment.propertyId equals prop.propertyId
                              join dept in dbContext.DepartmentMsts on prop.departmentId equals dept.departmentId
                              join sec in dbContext.SectorMsts on prop.sectorId.Value equals sec.sectorId
                              join block in dbContext.BlockMsts on prop.blockId.Value equals block.blockId
                              join proptype in dbContext.PropertyTypeMsts on prop.propertyTypeId equals proptype.propertyTypeId
                              where possession.IsActive == true && loginUserDeptt.Contains(allotment.departmentId)
                              select new PropertyPossessionModel
                              {
                                  Id = possession.Id,
                                  Rid = possession.Rid,
                                  PropertyId = prop.propertyId,
                                  DepartmentName = dept.departmentName,
                                  SectorName = sec.sectorName,
                                  BlockName = block.blockName,
                                  PropertyNo = prop.propertyNo,
                                  PropertyNumber = sec.sectorName + "/" + block.blockName + "-" + prop.propertyNo,
                                  PossessionOrderDate = possession.PossessionOrderDate,
                                  PossessionDate = possession.PossessionDate,
                                  AreaChange = possession.AreaChange,
                                  Print = true,
                                  StatusId=possession.StatusId,
                                  ApproveDate=possession.ApproveDate
                              });

                var allPossessionProp = new DataSourceResult();
                if (result.Any())
                    allPossessionProp = result.ToDataSourceResult(request);
                return allPossessionProp;
            }
        }

        // Get All properties for which possession has been done 
        public DataSourceResult GetAllPossessionEntryProperties(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var result = (from possession in dbContext.PossessionDetails
                              join allotment in dbContext.AllotmentMasters on possession.Rid equals allotment.rid
                              join prop in dbContext.SchemePropTrans on allotment.propertyId equals prop.propertyId
                              join dept in dbContext.DepartmentMsts on prop.departmentId equals dept.departmentId
                              join sec in dbContext.SectorMsts on prop.sectorId.Value equals sec.sectorId
                              join block in dbContext.BlockMsts on prop.blockId.Value equals block.blockId
                              join proptype in dbContext.PropertyTypeMsts on prop.propertyTypeId equals proptype.propertyTypeId
                              where possession.IsActive == true && possession.Possession == true && loginUserDeptt.Contains(allotment.departmentId)
                              select new PropertyPossessionModel
                              {
                                  Id = possession.Id,
                                  Rid = possession.Rid,
                                  PropertyId = prop.propertyId,
                                  DepartmentName = dept.departmentName,
                                  SectorName = sec.sectorName,
                                  BlockName = block.blockName,
                                  PropertyNo = prop.propertyNo,
                                  PropertyNumber = sec.sectorName + "/" + block.blockName + "-" + prop.propertyNo,
                                  PossessionOrderDate = possession.PossessionOrderDate,
                                  PossessionDate = possession.PossessionDate,
                                  AreaChange = possession.AreaChange,
                                  Print = true

                              });

                var allPossessionProp = new DataSourceResult();
                if (result.Any())
                    allPossessionProp = result.ToDataSourceResult(request);
                return allPossessionProp;
            }
        }

        // Add possession Order
        public bool AddUpdatePossession(PropertyPossessionModel possessionModel)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.PossessionDetails.FirstOrDefault(x => x.Rid == possessionModel.Rid && x.IsActive == true);
                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == possessionModel.Rid select appDet).FirstOrDefault();//For notification details
                if (result == null)
                {
                    var propPossessionDetails = new PossessionDetail();
                    propPossessionDetails.Rid = possessionModel.Rid;
                    propPossessionDetails.PropertyId = possessionModel.PropertyId;
                    propPossessionDetails.PossessionOrderDate = possessionModel.PossessionOrderDate ?? DateTime.Now.Date;//possessionModel.PossessionOrderDate;
                    propPossessionDetails.PossessionDueDate = possessionModel.PossessionDueDate;
                    propPossessionDetails.PossessionDate = possessionModel.PossessionDate;
                    propPossessionDetails.AreaChange = possessionModel.AreaChange;
                    //propPossessionDetails.OneTimeExcessCharge = possessionModel.OneTimeExcessCharge;
                    //propPossessionDetails.ChnagedArea = possessionModel.ChnagedArea;
                    propPossessionDetails.Possession = false;//possessionModel.Possession;
                    //propPossessionDetails.Amount = possessionModel.Amount;
                    //propPossessionDetails.PenaltyAmount = possessionModel.PenaltyAmount;
                    //propPossessionDetails.North = possessionModel.North;
                    //propPossessionDetails.East = possessionModel.East;
                    //propPossessionDetails.West = possessionModel.West;
                    //propPossessionDetails.South = possessionModel.South;
                    ////propPossessionDetails.StatusId = possessionModel.StatusId;
                    //propPossessionDetails.PossessionBy = possessionModel.PossessionBy;
                    ////propPossessionDetails.Approver = possessionModel.Approver;
                    ////propPossessionDetails.ApproveDate = possessionModel.ApproveDate;
                    ////propPossessionDetailss.Comment = possessionModel.Comment;
                    ////propPossessionDetails.CommentDate = possessionModel.CommentDate;
                    //propPossessionDetails.AreaChangeType = possessionModel.AreaChangeType;
                    propPossessionDetails.IsActive = true;//possessionModel.IsActive;
                    propPossessionDetails.CreatedBy = userInfo.UserID;
                    propPossessionDetails.CreatedDate = DateTime.Now.Date; //possessionModel.CreatedDate;
                    propPossessionDetails.OnlineRequestNo = possessionModel.ReqRefNo != null ? possessionModel.ReqRefNo : 0;
                    propPossessionDetails.Approver = possessionModel.Approver;
                    if (possessionModel.Approver == null)
                    {
                        propPossessionDetails.StatusId = NAStatusId.Approved;
                    }
                    else
                    {
                        propPossessionDetails.StatusId = NAStatusId.InProgress;
                    }
                    dbContext.PossessionDetails.Add(propPossessionDetails);
                    var count = dbContext.SaveChanges();
                    flag = true;
                    //if (count > 0)
                    //{
                    //    flag = true;
                    //    //Send notifications
                    //    //Email
                    //    var body = "Hi,<br><br>Please check the property Area and collect your Possession Letter within 15 days.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                    //    EmailHelper emailHelper = new EmailHelper();
                    //    emailHelper.Send(newOwner.tEmail, "Registration Completed", body);
                    //    //SMS
                    //    //var msg = "Please check the property Area and collect your Possession Letter within 15 days.";
                    //    var msg = NAMessages.PossessionLetter;
                    //    SMSSend(newOwner.tMobileNumber, msg);
                    //}
                }

            }
            return flag;
        }

        // Add possession Entry
        public bool AddUpdatePossessionEntry(PropertyPossessionModel possessionModel)
        {
            var flag = false;
            try
            {

                using (var dbContext = new NoidaPMSEntities())
                {
                    var result = dbContext.PossessionDetails.FirstOrDefault(x => x.Rid == possessionModel.Rid && x.IsActive == true && x.StatusId==NAStatusId.Approved);
                    var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == possessionModel.Rid select appDet).FirstOrDefault();//For notification details
                    if (result == null)
                    {
                        var propPossessionDetails = new PossessionDetail();
                        propPossessionDetails.Rid = possessionModel.Rid;
                        propPossessionDetails.PropertyId = possessionModel.PropertyId;
                        propPossessionDetails.PossessionOrderDate = possessionModel.PossessionOrderDate ?? DateTime.Now.Date;//possessionModel.PossessionOrderDate;
                        propPossessionDetails.PossessionDueDate = possessionModel.PossessionDueDate;
                        propPossessionDetails.PossessionDate = possessionModel.PossessionDate;
                        propPossessionDetails.AreaChange = possessionModel.AreaChange;
                        propPossessionDetails.OneTimeExcessCharge = possessionModel.OneTimeExcessCharge;
                        propPossessionDetails.ChnagedArea = possessionModel.ChnagedArea;
                        propPossessionDetails.Possession = true;//possessionModel.Possession;
                        //propPossessionDetails.Amount = possessionModel.Amount;
                        //propPossessionDetails.PenaltyAmount = possessionModel.PenaltyAmount;
                        propPossessionDetails.North = possessionModel.North;
                        propPossessionDetails.East = possessionModel.East;
                        propPossessionDetails.West = possessionModel.West;
                        propPossessionDetails.South = possessionModel.South;
                        //propPossessionDetails.StatusId = possessionModel.StatusId;
                        propPossessionDetails.PossessionBy = possessionModel.PossessionBy;
                        //propPossessionDetails.Approver = possessionModel.Approver;
                        //propPossessionDetails.ApproveDate = possessionModel.ApproveDate;
                        //propPossessionDetailss.Comment = possessionModel.Comment;
                        //propPossessionDetails.CommentDate = possessionModel.CommentDate;
                        propPossessionDetails.AreaChangeType = possessionModel.AreaChangeType;
                        propPossessionDetails.IsActive = true;//possessionModel.IsActive;
                        propPossessionDetails.CreatedBy = userInfo.UserID;
                        propPossessionDetails.CreatedDate = DateTime.Now.Date; //possessionModel.CreatedDate;

                        dbContext.PossessionDetails.Add(propPossessionDetails);
                        var count = dbContext.SaveChanges();
                        if (count > 0)
                        {
                            flag = true;
                            //Send notifications
                            //Email
                            var body = "Hi,<br><br>Please check the property Area and collect your Possession Letter within 15 days.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                            EmailHelper emailHelper = new EmailHelper();
                            emailHelper.Send(newOwner.tEmail, "Registration Completed", body);
                            //SMS
                            //var msg = "Please check the property Area and collect your Possession Letter within 15 days.";
                            var msg = NAMessages.PossessionLetter;
                            //SMSSend(newOwner.tMobileNumber, msg);
                            ApplicationHelper.SendSMS(newOwner.tMobileNumber, msg);
                        }
                    }
                    else
                    {
                        result.Rid = possessionModel.Rid;
                        result.PropertyId = possessionModel.PropertyId;
                        result.PossessionOrderDate = DateTime.Now.Date;//possessionModel.PossessionOrderDate;
                        result.PossessionDueDate = possessionModel.PossessionDueDate;
                        result.PossessionDate = possessionModel.PossessionDate;
                        result.AreaChange = possessionModel.AreaChange;
                        result.AreaChangeType = possessionModel.AreaChangeType;
                        result.OneTimeExcessCharge = possessionModel.OneTimeExcessCharge;
                        result.ChnagedArea = possessionModel.ChnagedArea;
                        result.Possession = true;//possessionModel.Possession;
                        //result.Amount = possessionModel.Amount;
                        //result.PenaltyAmount = possessionModel.PenaltyAmount;
                        result.North = possessionModel.North;
                        result.East = possessionModel.East;
                        result.West = possessionModel.West;
                        result.South = possessionModel.South;
                        //result.StatusId = 0;
                        result.PossessionBy = possessionModel.PossessionBy;
                        //result.Approver = possessionModel.Approver;
                        //result.ApproveDate = possessionModel.ApproveDate;
                        //result.Comment = possessionModel.Comment;
                        //result.CommentDate = possessionModel.CommentDate;
                        result.IsActive = true;//possessionModel.IsActive;

                        result.Modifiedby = userInfo.UserID;
                        result.ModifiedDate = DateTime.Now.Date; //possessionModel.CreatedDate;

                        dbContext.PossessionDetails.AddOrUpdate(result);
                        var count = dbContext.SaveChanges();
                        if (count > 0)
                        {
                            flag = true;
                            //Send notifications
                            //Email
                            var body = "Hi,<br><br>Please check the property Area and collect your Possession Letter within 15 days.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                            EmailHelper emailHelper = new EmailHelper();
                            emailHelper.Send(newOwner.tEmail, "Registration Completed", body);
                            //SMS
                            //var msg = "Please check the property Area and collect your Possession Letter within 15 days.";
                            var msg = NAMessages.PossessionLetter;
                            //SMSSend(newOwner.tMobileNumber, msg);
                            ApplicationHelper.SendSMS(newOwner.tMobileNumber, msg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return flag;
        }

        // Add Bulk Possession Order
        public bool AddBulkPossession(List<string> listRiDs)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                foreach (var rid in listRiDs)
                {
                    //var ids = Convert.ToInt32(rid.Split(',')[0]);
                    var propPossessionDetails = new PossessionDetail();
                    propPossessionDetails.Rid = Convert.ToInt32(rid.Split(',')[0]);
                    propPossessionDetails.PropertyId = Convert.ToInt32(rid.Split(',')[1]);
                    propPossessionDetails.PossessionOrderDate = DateTime.Now.Date;
                    //possessionModel.PossessionOrderDate;
                    //propPossessionDetails.PossessionDueDate = possessionModel.PossessionDueDate;
                    //propPossessionDetails.PossessionDate = possessionModel.PossessionDate;
                    propPossessionDetails.Possession = false; //possessionModel.Possession;
                    propPossessionDetails.IsActive = true; //possessionModel.IsActive;
                    propPossessionDetails.CreatedBy = userInfo.UserID; //possessionModel.CreatedBy;
                    propPossessionDetails.CreatedDate = DateTime.Now.Date; //possessionModel.CreatedDate;

                    dbContext.PossessionDetails.Add(propPossessionDetails);
                }
                var count = dbContext.SaveChanges();
                if (count > 0)
                    flag = true;
                return flag;
            }
        }

        // Get All RIDs whose Lease deed done
        public DataSourceResult GetAllRIDsLeaseDeedDone(DataSourceRequest Req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var didList = (from usr in dbContext.UmUserMasters join udt in dbContext.UmUserDepartmentTrans on usr.UserRefId equals udt.UserRefId where usr.UserRefId == userInfo.UserID select udt.DepartmentId).ToList();
                //var result = dbContext.RegistryDetails.Where(x => x.IsActive == true).Select(obj => new CommonListModel { Value = obj.Rid.Value, Text = obj.Rid.ToString() }).ToList();
                var result = (from regd in dbContext.RegistryDetails
                              where didList.Contains(regd.AllotmentMaster.departmentId) && regd.AllotmentMaster.isActive == 1 && regd.IsActive == true
                              orderby regd.CreatedDate descending
                              select new CommonListModel
                              {
                                  Text = regd.Rid.ToString(),
                                  Value = regd.Rid.Value
                              });
                return result.ToDataSourceResult(Req);
            }
        }

        // Get All RIDs whose Lease deed done
        public DataSourceResult GetAllPossRIDsLeaseDeedDone(DataSourceRequest Req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var didList = (from usr in dbContext.UmUserMasters join udt in dbContext.UmUserDepartmentTrans on usr.UserRefId equals udt.UserRefId where usr.UserRefId == userInfo.UserID select udt.DepartmentId).ToList();
                //var result = dbContext.RegistryDetails.Where(x => x.IsActive == true).Select(obj => new CommonListModel { Value = obj.Rid.Value, Text = obj.Rid.ToString() }).ToList();
                var result = (from regd in dbContext.RegistryDetails
                              join possession in dbContext.PossessionDetails on regd.Rid equals possession.Rid
                              where didList.Contains(regd.AllotmentMaster.departmentId) && regd.AllotmentMaster.isActive == 1 && regd.IsActive == true && possession.IsActive==true && possession.StatusId==NAStatusId.Approved
                              orderby regd.CreatedDate descending
                              select new CommonListModel
                              {
                                  Text = regd.Rid.ToString(),
                                  Value = regd.Rid.Value
                              });
                return result.ToDataSourceResult(Req);
            }
        }

        // Get properties for which possession is pending by schemeId and Department Id
        public DataSourceResult GetPossessionPropertiesBySchemeDept(DataSourceRequest request, int schemeId, int deptId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var tempResult = from poss in dbContext.PossessionDetails select poss.Rid;
                var result = (from allotment in dbContext.AllotmentMasters //on possession.Rid equals allotment.rid
                              join registry in dbContext.RegistryDetails on allotment.rid equals registry.Rid
                              join appDetails in dbContext.ApplicationDetails on allotment.applicationId equals appDetails.applicationId
                              join prop in dbContext.SchemePropTrans on allotment.propertyId equals prop.propertyId
                              join dept in dbContext.DepartmentMsts on prop.departmentId equals dept.departmentId
                              join sec in dbContext.SectorMsts on prop.sectorId.Value equals sec.sectorId
                              join block in dbContext.BlockMsts on prop.blockId.Value equals block.blockId
                              join proptype in dbContext.PropertyTypeMsts on prop.propertyTypeId equals proptype.propertyTypeId
                              where prop.departmentId == deptId && prop.schemeId == schemeId && allotment.isActive == 1
                              && allotment.isStatus == Constants.IsApproved
                              && !tempResult.Contains(allotment.rid)
                              select new PropertyPossessionModel
                              {
                                  Id = allotment.applicationId.Value,
                                  Rid = allotment.rid,
                                  PropertyId = prop.propertyId,
                                  DepartmentName = dept.departmentName,
                                  DepartmentId = dept.departmentId,
                                  PropertyNumber = sec.sectorName + "/" + block.blockName + "-" + prop.propertyNo,
                                  FirstName = appDetails.firstName,
                                  MiddleName = appDetails.middleName,
                                  LastName = appDetails.lastName,
                                  //ApplicantName = appDetails.firstName + " " + appDetails.lastName,
                                  FatherName = appDetails.fatherHusbandName,
                                  //PossessionOrderDate = date//possession.PossessionOrderDate,
                                  //PossessionDate = possession.PossessionDate,
                                  //PossessionDueDate = possession.PossessionDueDate
                                  //AreaChange = possession.AreaChange,
                                  //Print = true
                              });

                var allPossessionProp = new DataSourceResult();
                if (result.Any())
                    allPossessionProp = result.ToDataSourceResult(request);
                return allPossessionProp;
            }
        }

        // Get list of models to print possession orders
        public List<PropertyPossessionModel> GetModelToPrintPossessionOrder(List<int> rids)
        {
            var listPossModels = new List<PropertyPossessionModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                listPossModels = (from allotment in dbContext.AllotmentMasters
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
                                  where rids.Contains(allotment.rid)
                                  select new PropertyPossessionModel
                                  {
                                      Rid = allotment.rid,
                                      SchemeName = schemeMaster.schemeName,
                                      SchemeId = allotment.schemeId.Value,
                                      DepartmentName = dept.departmentName,
                                      DepartmentId = dept.departmentId,
                                      PropertyNumber = sec.sectorName + "/" + block.blockName + "-" + prop.propertyNo,
                                      FirstName = appDetails.firstName,
                                      MiddleName = appDetails.middleName,
                                      LastName = appDetails.lastName,
                                      //ApplicantName = appDetails.firstName + " " + appDetails.lastName,
                                      Gender = appDetails.gender,
                                      FatherName = appDetails.fatherHusbandName,
                                      PropertyId = prop.propertyId,
                                      PropertyType = propMaster.propertyTypeName,
                                      Area = prop.totalArea.Value,
                                      FloorAreaRange = floorMst.floorName,
                                      LeaseDeedDueDate = registry.RegistryDueDate.Value,
                                      LeaseDeedExecutionDate = registry.RegistryDoneDate.Value,
                                      //PossessionDueDate = registry.RegistryDueDate.Value.AddDays(60),

                                  }).ToList();

            }
            return listPossModels;
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


        public PropertyPossessionModel GetPossessionDetailsByRegistratioinId(int registrationId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from possessn in dbContext.PossessionDetails
                              join property in dbContext.SchemePropTrans on possessn.AllotmentMaster.propertyId equals property.propertyId
                              where possessn.Rid == registrationId && possessn.IsActive==true
                              select new PropertyPossessionModel
                              {
                                  Rid = possessn.Rid,
                                  SchemeId = property.SchemeMst.schemeId,
                                  SchemeName = property.SchemeMst.schemeName,
                                  DepartmentId = property.DepartmentMst.departmentId,
                                  DepartmentName = property.DepartmentMst.departmentName,
                                  FirstName = possessn.AllotmentMaster.ApplicationDetail.firstName,
                                  MiddleName = possessn.AllotmentMaster.ApplicationDetail.middleName,
                                  LastName = possessn.AllotmentMaster.ApplicationDetail.lastName,
                                  FatherName = possessn.AllotmentMaster.ApplicationDetail.fatherHusbandName,
                                  MotherName = possessn.AllotmentMaster.ApplicationDetail.motherName,
                                  SigningAuthority = possessn.AllotmentMaster.ApplicationDetail.signingAuthority,
                                  RegisteredOffice = possessn.AllotmentMaster.ApplicationDetail.registeredOffice,
                                  CorrespondAdd = possessn.AllotmentMaster.ApplicationDetail.correspondanceAdd,
                                  Gender = possessn.AllotmentMaster.ApplicationDetail.gender,
                                  PropertyId = property.propertyId,
                                  PropertyType = property.PropertyTypeMst.propertyTypeName,
                                  PropertyNumber = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo,
                                  Area = property.totalArea.Value,
                                  FloorAreaRange = property.FloorMst.floorName,
                                  LeaseDeedDueDate = dbContext.RegistryDetails.FirstOrDefault(r=>r.Rid==possessn.Rid).RegistryDueDate.Value,
                                  LeaseDeedExecutionDate = dbContext.RegistryDetails.FirstOrDefault(r=>r.Rid==possessn.Rid).RegistryDoneDate.Value,
                                  PossessionOrderDate = possessn.PossessionOrderDate,
                                  PossessionDueDate = possessn.PossessionDueDate,
                                  IsPossessionOrderDone = false,
                                  ReqRefNo = possessn.OnlineRequestNo,
                                  Approver=possessn.Approver,
                                  ApproveDate=possessn.ApproveDate,
                                  IsActive=possessn.IsActive
                              }).FirstOrDefault();
                return result;

            }
        }


        public DataSourceResult GetPossessionDataByApproverId(DataSourceRequest request, PropertyPossessionModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var objPossession = (from possession in dbContext.PossessionDetails
                                     join alot in dbContext.AllotmentMasters on possession.Rid equals alot.rid
                                     join prop in dbContext.SchemePropTrans on alot.propertyId equals prop.propertyId
                                     join statusMas in dbContext.StatusMasters on possession.StatusId equals statusMas.Id
                                     where possession.IsActive == true && possession.Approver == userInfo.UserID && possession.StatusId == NAStatusId.InProgress
                                     select new PropertyPossessionModel
                                     {
                                         Id=possession.Id,
                                         Rid = possession.Rid,
                                         PropertyId = prop.propertyId,
                                         SectorName=prop.SectorMst.sectorName,
                                         BlockName=prop.BlockMst.blockName,
                                         PropertyNo=prop.propertyNo,
                                         StatusId=possession.StatusId,
                                         Status=statusMas.Status,
                                         PossessionOrderDate=possession.PossessionOrderDate,
                                         PossessionDate=possession.PossessionDate,
                                         DepartmentId=(int)alot.departmentId,
                                         DepartmentName = alot.DepartmentMst.departmentName,
                                         PropertyNumber = prop.SectorMst.sectorName + "/" + prop.BlockMst.blockName + "-" + prop.propertyNo,
                                         AreaChange = possession.AreaChange,
                                         ApproveDate = possession.ApproveDate,
                                         Approver=possession.Approver
                                     });
                return objPossession.ToDataSourceResult(request);
            }
        }


        public PropertyPossessionModel GetPossessionDetailsByReqNo(int id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from possessn in dbContext.PossessionDetails
                              join property in dbContext.SchemePropTrans on possessn.AllotmentMaster.propertyId equals property.propertyId
                              where possessn.Id == id //&& possessn.IsActive == true
                              select new PropertyPossessionModel
                              {
                                  Id=possessn.Id,
                                  Rid = possessn.Rid,
                                  SchemeId = property.SchemeMst.schemeId,
                                  SchemeName = property.SchemeMst.schemeName,
                                  DepartmentId = property.DepartmentMst.departmentId,
                                  DepartmentName = property.DepartmentMst.departmentName,
                                  FirstName = possessn.AllotmentMaster.ApplicationDetail.firstName,
                                  MiddleName = possessn.AllotmentMaster.ApplicationDetail.middleName,
                                  LastName = possessn.AllotmentMaster.ApplicationDetail.lastName,
                                  FatherName = possessn.AllotmentMaster.ApplicationDetail.fatherHusbandName,
                                  MotherName = possessn.AllotmentMaster.ApplicationDetail.motherName,
                                  SigningAuthority = possessn.AllotmentMaster.ApplicationDetail.signingAuthority,
                                  RegisteredOffice = possessn.AllotmentMaster.ApplicationDetail.registeredOffice,
                                  CorrespondAdd = possessn.AllotmentMaster.ApplicationDetail.correspondanceAdd,
                                  Gender = possessn.AllotmentMaster.ApplicationDetail.gender,
                                  PropertyId = property.propertyId,
                                  PropertyType = property.PropertyTypeMst.propertyTypeName,
                                  PropertyNumber = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo,
                                  Area = property.totalArea.Value,
                                  FloorAreaRange = property.FloorMst.floorName,
                                  LeaseDeedDueDate = dbContext.RegistryDetails.FirstOrDefault(r => r.Rid == possessn.Rid).RegistryDueDate.Value,
                                  LeaseDeedExecutionDate = dbContext.RegistryDetails.FirstOrDefault(r => r.Rid == possessn.Rid).RegistryDoneDate.Value,
                                  PossessionOrderDate = possessn.PossessionOrderDate,
                                  PossessionDueDate = possessn.PossessionDueDate,
                                  //IsPossessionOrderDone = false,
                                  ReqRefNo = possessn.OnlineRequestNo,
                                  Approver = possessn.Approver,
                                  ApproveDate = possessn.ApproveDate,
                                  IsActive = possessn.IsActive,
                                  StatusId=possessn.StatusId,
                                  Status=possessn.StatusMaster.Status,
                                  Possession=possessn.Possession
                              }).FirstOrDefault();
                return result;
            }
        }


        public int UpdatePossessionRequest(PropertyPossessionModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var possession = dbContext.PossessionDetails.FirstOrDefault(x => x.Id == model.Id && x.IsActive == true);
                if (possession != null)
                {
                    possession.StatusId = model.StatusId;
                    possession.ApproveDate = DateTime.Now;
                    possession.Comment = model.Comment;
                    if (model.StatusId == NAStatusId.Rejected)
                    {
                        possession.IsActive = false;
                        flag = ReturnType.Rejected;
                    }
                    dbContext.SaveChanges();
                    if(possession.StatusId==NAStatusId.Approved)
                    {
                        if (possession.OnlineRequestNo != 0)
                        {
                            var deptId = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == possession.Rid).departmentId;
                            PropertyRegistrationRepository repo = new PropertyRegistrationRepository();
                            repo.UpdateServiceRequest(possession.OnlineRequestNo, possession.Rid, deptId, possession.Comment);
                        }
                        flag=ReturnType.Approved;
                    }
                    
                }
                return flag = ReturnType.Approved;
            }
        }
    }
}
