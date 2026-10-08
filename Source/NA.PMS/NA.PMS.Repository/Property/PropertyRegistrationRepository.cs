using Kendo.Mvc.UI;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Model.CommonModel;
using NA.PMS.Common;
using NA.PMS.Common.Extension;
using Kendo.Mvc.Extensions;
using NoidaAuthority.PMS.Common;
using System.IO;
using System.Net;
using System.Web;
using NA.PMS.Web.Models;
using System.Data.Entity.Core.Objects;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace NA.PMS.Repository
{
    public class PropertyRegistrationRepository : IPropertyRegistrationRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();

        public PropertyRegistrationRepository()
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
        NAApplication appl = new NAApplication();

        /// <summary>
        /// Grid read function of Manage Possession screen. 
        /// </summary>
        /// <param name="req">Kendo's parameter</param>
        /// <returns></returns>
        public DataSourceResult GetPossessionData(DataSourceRequest req)
        {
            var rslt = new DataSourceResult();
            return rslt;
        }

        // For getting CheckList Type
        public IEnumerable<CommonListModel> GetCheckListType()
        {
            var checklListType = new List<CommonListModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                checklListType = (from tblChkListType in dbContext.ChecklistTypeMasters
                                  select new CommonListModel
                                  {
                                      Text = tblChkListType.Type,
                                      Value = tblChkListType.Id
                                  }).ToList();
            }
            return checklListType;
        }

        // Getting Property details by RID
        public CheckList GetCheckListDetailsByRId(int rId)
        {
            var checkListDetails = new CheckList();
            return checkListDetails;
        }

        // Getting all Lease deed properties
        public List<LeaseDeedProperty> GetAllLeaseDeadProperties()
        {
            var leaseDeedPropertyList = new List<LeaseDeedProperty>();
            return leaseDeedPropertyList;
        }

        // Get lease deed property details by RID
        public LeaseDeedProperty GetLeaseDeadPropertyDetailsByRId(int rId)
        {
            var leaseDeedPropertyDetails = new LeaseDeedProperty();
            return leaseDeedPropertyDetails;
        }

        // Add/Update lease deed property details by RID ---- Add Lease deed Execution date
        public LeaseDeedProperty AddLeaseDeadPropertyDetailsByRId(LeaseDeedProperty leaseDeadProperty)
        {
            var leaseDeedPropertyDetails = new LeaseDeedProperty();
            return leaseDeedPropertyDetails;
        }

        //return rent permission list to manage 
        public DataSourceResult GetPropertyRentList(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var departmentList = (from dept in dbContext.UmDepartmentMasters
                                      join udts in dbContext.UmUserDepartmentTrans on dept.DepartmentId equals udts.DepartmentId
                                      where udts.UserRefId == userInfo.UserID
                                      select dept.DepartmentId);
                var requests = (from rent in dbContext.RentPermissionDetails
                                join alot in dbContext.AllotmentMasters on rent.Rid equals alot.rid
                                where departmentList.Contains(alot.departmentId.Value) && rent.IsActive == true
                                //where rent.CreatedBy == userInfo.UserID
                                select new RentModel
                                {
                                    RequestNo = rent.RequestNo,
                                    RID = rent.Rid.Value,
                                    SchemeName = alot.SchemeMst.schemeName,
                                    Department = alot.DepartmentMst.departmentName,
                                    ApplicantName = alot.ApplicationDetail.tFirstName + " " + alot.ApplicationDetail.tMiddleName + " " + alot.ApplicationDetail.tLastName,
                                    TenantName = rent.TenantName,
                                    RequestDate = rent.RentingDate.Value,
                                    Status = rent.StatusMaster.Status
                                });//.ToList();
                return requests.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetPropertyRentListByRid(DataSourceRequest request, int Rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var departmentList = (from dept in dbContext.UmDepartmentMasters
                                      join udts in dbContext.UmUserDepartmentTrans on dept.DepartmentId equals udts.DepartmentId
                                      where udts.UserRefId == userInfo.UserID
                                      select dept.DepartmentId);
                var requests = (from rent in dbContext.RentPermissionDetails
                                join alot in dbContext.AllotmentMasters on rent.Rid equals alot.rid
                                where departmentList.Contains(alot.departmentId.Value)
                                where rent.Rid == Rid && rent.StatusId == 1
                                select new RentModel
                                {
                                    RequestNo = rent.RequestNo,
                                    RID = rent.Rid.Value,
                                    SchemeName = alot.SchemeMst.schemeName,
                                    Department = alot.DepartmentMst.departmentName,
                                    ApplicantName = alot.ApplicationDetail.tFirstName + " " + alot.ApplicationDetail.tMiddleName + " " + alot.ApplicationDetail.tLastName,
                                    TenantName = rent.TenantName,
                                    RequestDate = rent.RentingDate.Value,
                                    Status = rent.StatusMaster.Status
                                });//.ToList();
                return requests.ToDataSourceResult(request);
            }
        }
        // Get all RIDs 
        public IEnumerable<CommonListModel> GetAllRIDsForLeaseDeed()
        {
            var allRIDsForLeaseDeed = new List<CommonListModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                allRIDsForLeaseDeed = (from tblAllotmentMst in dbContext.AllotmentMasters
                                       where tblAllotmentMst.isActive == 1
                                       orderby tblAllotmentMst.createdDate descending
                                       select new CommonListModel
                                       {
                                           // Text = Convert.ToString(tblAllotmentMst.rid),
                                           Value = tblAllotmentMst.rid
                                       }).ToList();
            }
            return allRIDsForLeaseDeed;
        }

        /// <summary>
        /// Gets details from the DB on the basis of RID
        /// </summary>
        /// <param name="rId">RID</param>
        /// <returns>Object of type DetailsByRId model with all the details</returns>
        public DetailsByRId GetDetailsByRID(int rId)
        {
            var details = new DetailsByRId();
            using (var dbContext = new NoidaPMSEntities())
            {
                int chkListTypeId = Convert.ToInt32(CheckListType.LeaseDeed);
                details = (from ad in dbContext.ApplicationDetails
                           join am in dbContext.AllotmentMasters on ad.registrationId equals am.rid
                           join spt in dbContext.SchemePropTrans on am.propertyId equals spt.propertyId
                           join scm in dbContext.SchemeMsts on spt.schemeId equals scm.schemeId
                           join dm in dbContext.DepartmentMsts on ad.departmentId equals dm.departmentId
                           join pm in dbContext.PropertyTypeMsts on spt.propertyTypeId equals pm.propertyTypeId
                           join sm in dbContext.SectorMsts on spt.sectorId equals sm.sectorId
                           join bm in dbContext.BlockMsts on spt.blockId equals bm.blockId
                           join fm in dbContext.FloorMsts on spt.floorId equals fm.floorId
                           //join chLi in dbContext.ChecklistTrans on am.rid equals chLi.Rid
                           where am.rid == rId
                           select new DetailsByRId
                           {
                               AllotteName = ad.firstName + " " + ad.middleName + " " + ad.lastName,
                               FatherName = ad.fatherHusbandName,
                               DepttName = dm.departmentName,
                               PropType = pm.propertyTypeName,
                               PropNo = sm.sectorName + "/" + bm.blockName + "-" + spt.propertyNo,
                               PropCost = spt.totalPropertyCost,
                               //AllotmentMoney = spt.allotmentMoney == null ? Constants.no : Constants.yes,
                               AllotmentMoney = spt.allotmentMoney,//Bug#117 - Resolved
                               AllotmentDate = am.allotmentDate,
                               PropTypeVal = pm.propertyTypeId,
                               SchemeId = spt.schemeId.Value,
                               PropId = am.propertyId,
                               RegistryType = spt.Registry,
                               DepttID = ad.departmentId.Value,
                               Area = spt.totalArea,
                               Floor = fm.floorName,
                               SchemeName = scm.schemeName,
                               Gender = ad.gender,
                               ChecklistDate = (from chLi in dbContext.ChecklistTrans where chLi.Rid == rId && chLi.ChecklistTypeId == chkListTypeId select chLi.ChecklistDate).FirstOrDefault()
                           }).FirstOrDefault();
                var propId = (from am in dbContext.AllotmentMasters where am.rid == rId select am.propertyId).FirstOrDefault();
                var parentRID = (from spt in dbContext.SchemePropTrans join am in dbContext.AllotmentMasters on spt.ParentPropertyId equals am.propertyId where spt.propertyId == propId select am.rid).FirstOrDefault();
                //var  = (from am in dbContext.AllotmentMasters where am.propertyId == parentPropRID select am.rid).FirstOrDefault();
                if (parentRID != null && parentRID != 0 && details != null)
                {
                    //details.IsParentLeaseDeed = ((from rd in dbContext.RegistryDetails where rd.Rid == parentRID && rd.IsActive == true select rd).FirstOrDefault()) == null ? false : true;
                    var parentLeaseDeed = (from rd in dbContext.RegistryDetails where rd.Rid == parentRID && rd.IsActive == true select rd).FirstOrDefault();
                    if (parentLeaseDeed != null)
                        details.IsParentLeaseDeed = true;
                    else
                        details.IsParentLeaseDeed = false;
                }
            }
            return details;
        }

        /// <summary>
        /// Fetches Transfer Details based on RID
        /// </summary>
        /// <param name="rId">RID</param>
        /// <returns></returns>
        public DetailsByRId GetTransferDetailsByRID(int rId)
        {
            var details = new DetailsByRId();
            using (var dbContext = new NoidaPMSEntities())
            {
                details = (from am in dbContext.AllotmentMasters
                           join appDet in dbContext.ApplicationDetails on am.rid equals appDet.registrationId
                           join spt in dbContext.SchemePropTrans on am.propertyId equals spt.propertyId
                           join scm in dbContext.SchemeMsts on spt.schemeId equals scm.schemeId
                           join dm in dbContext.DepartmentMsts on am.departmentId equals dm.departmentId
                           join pm in dbContext.PropertyTypeMsts on spt.propertyTypeId equals pm.propertyTypeId
                           join sm in dbContext.SectorMsts on spt.sectorId equals sm.sectorId
                           join bm in dbContext.BlockMsts on spt.blockId equals bm.blockId
                           join fm in dbContext.FloorMsts on spt.floorId equals fm.floorId
                           join trans in dbContext.Succ_Mut_Trans on am.rid equals trans.Rid
                           join sta in dbContext.StatusMasters on trans.Status equals sta.Id
                           join tranType in dbContext.Transfer_Type on trans.Transfer_Type equals tranType.Id
                           where trans.Rid == rId && sta.Status.ToLower() == AllotmentStatus.Approved.ToString().ToLower()
                           orderby trans.Request_No descending
                           select new DetailsByRId
                           {
                               SchemeName = scm.schemeName,
                               DepttName = dm.departmentName,
                               AllotteName = trans.Applicant_Name,
                               Gender = trans.Applicant_Gender,
                               FatherName = trans.ApplicantFather_Name,
                               PropType = pm.propertyTypeName,
                               Area = spt.totalArea,
                               Floor = fm.floorName,
                               //AllotteName = ad.firstName + " " + ad.middleName + " " + ad.lastName,
                               //FatherName = ad.fatherHusbandName,
                               //DepttName = dm.departmentName,
                               //PropType = pm.propertyTypeName,
                               PropNo = sm.sectorName + "/" + bm.blockName + "-" + spt.propertyNo,
                               //PropCost = spt.totalPropertyCost,
                               //AllotmentMoney = spt.allotmentMoney == null ? Constants.no : Constants.yes,
                               //AllotmentDate = am.allotmentDate,
                               //PropTypeVal = pm.propertyTypeId,
                               //SchemeId = spt.schemeId.Value,
                               //PropId = am.propertyId.Value,
                               //RegistryType = spt.Registry,
                               ////TODO: LeaseDeedDueDate,
                               DepttID = appDet.departmentId.Value,
                               //Area = spt.totalArea,
                               //Floor = fm.floorName,
                               //SchemeName = scm.schemeName,
                               //Gender = ad.gender
                               ReqNo = trans.Request_No,
                               TransfereeName = appDet.tFirstName + " " + appDet.tMiddleName + " " + appDet.tLastName,
                               TransfereeGender = appDet.tGender,
                               TransfereeRelationName = appDet.tFatherHusbandName,
                               TransferDate = trans.Transfer_Date,
                               //TransferType = trans.Transfer_Sub_Type
                               TransferType = tranType.type,
                               AllotmentDate = am.allotmentDate,
                               GPAEffectiveFrom = trans.GPA_Effective_From,
                               GPAEffectiveto = trans.GPA_Effective_To,
                               GPAHolderAddress = trans.GPA_Holder_Address,
                               GPAHolderName = trans.GPA_Holder_Name
                           }).FirstOrDefault();
            }
            return details;
        }

        /// <summary>
        /// Reads Documents list for Kendo grid on the basis of Property type and Checklist type.
        /// Kendo's DataSourceRequest functionality has not been used as the number of documents would be less, hence its not required.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="propTypeId">Property Type Id</param>
        /// <param name="chkLstType">Checklist Type Id</param>
        /// <returns>List of Checklist documents</returns>
        public List<ChecklistDocuments> GetChcklstDocuments(DataSourceRequest request, int propTypeId, int chkLstType)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var allDocs = new List<ChecklistDocuments>();
                int i = 0;
                var docs = (from doc in dbContext.ChecklistMasters
                            where doc.IsActive == true && doc.PropertyTypeId == propTypeId && doc.ChecklistTypeId == chkLstType
                            select new ChecklistDocuments
                            {
                                Id = doc.Id,
                                DocName = doc.DocumentName
                            }).ToList();
                if (docs.Count > 0)
                {
                    allDocs = (from d in docs select new ChecklistDocuments { Id = d.Id, DocName = d.DocName, SNo = ++i }).ToList();
                }
                return allDocs;
            }
        }

        /// <summary>
        /// Saves/Updates details of Generate Checklist in DB
        /// </summary>
        /// <param name="rId">RID</param>
        /// <param name="chkType">Checklist Type</param>
        /// <param name="chkDate">Checklist Date</param>
        /// <returns>Flag -> true - saved successfully; false - unable to save</returns>
        public bool SaveChecklistDetails(int rId, int chkType, DateTime chkDate, string viewName)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingchklstTrans = dbContext.ChecklistTrans.Where(chk => chk.Rid == rId && chk.IsActive == true && chk.ChecklistTypeId == chkType).FirstOrDefault();
                //Update existing
                if (existingchklstTrans != null)
                {
                    //Check if there is any change in the Checklist Date, only then update the record.
                    if (existingchklstTrans.ChecklistDate.Value.Date != chkDate.Date)
                    {
                        CheckList oldObjModel = new CheckList();//Old Model
                        oldObjModel.CheckListDate = existingchklstTrans.ChecklistDate;

                        existingchklstTrans.ChecklistDate = chkDate;
                        existingchklstTrans.Modifiedby = userInfo.UserID;
                        existingchklstTrans.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = true;

                        //New Model for Audit Trail
                        var newObjModel = new CheckList
                        {
                            CheckListDate = chkDate
                        };
                        GeneralRepository.CreateAuditTrail(Constants.Update, Constants.ChecklistTrans, viewName, rId, oldObjModel, newObjModel, userInfo.UserID.ToString());
                    }
                    //No updation (so that it does not come in Audit Trail as there were no changes being made) but flag is set to true so as to proceed further.
                    else
                        flag = true;
                }
                //Add new record
                else
                {
                    CheckList oldObjModel = new CheckList();

                    var chklstTrans = new ChecklistTran();
                    chklstTrans.Rid = rId;
                    chklstTrans.ChecklistTypeId = chkType;
                    chklstTrans.ChecklistDate = chkDate;
                    chklstTrans.IsActive = true;
                    chklstTrans.CreatedBy = userInfo.UserID;
                    chklstTrans.CreatedDate = DateTime.Now;
                    dbContext.ChecklistTrans.Add(chklstTrans);
                    dbContext.SaveChanges();
                    flag = true;

                    var newObjModel = new CheckList
                    {
                        RId = rId,
                        CheckListTypeId = chkType,
                        CheckListDate = chkDate,
                        IsActive = true
                    };
                    GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.ChecklistTrans, viewName, chklstTrans.Id, oldObjModel, newObjModel, userInfo.UserID.ToString());
                }
            }
            return flag;
        }

        /// <summary>
        /// Reads Lease Deed data from DB for ManageLeaseDeed screen
        /// </summary>
        /// <param name="req">Kendo's internal parameter</param>
        /// <returns></returns>
        public DataSourceResult GetLeaseDeedData(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();
                var leaseDeedData = (from rd in dbContext.RegistryDetails
                                     join am in dbContext.AllotmentMasters on rd.Rid equals am.rid
                                     join sm in dbContext.SchemeMsts on am.schemeId equals sm.schemeId
                                     join dm in dbContext.DepartmentMsts on am.departmentId equals dm.departmentId
                                     where rd.IsActive == true && loginUserDeptt.Contains(am.departmentId) && am.isActive == 1
                                     select new LeaseDeedProperty
                                     {
                                         ReqNo = rd.RequestNo,
                                         RId = rd.Rid.Value,
                                         SchemeName = sm.schemeName,
                                         DepttName = dm.departmentName,
                                         //TODO: AmountDue = ,
                                         LeaseDeedDueDate = rd.RegistryDueDate,
                                         LeaseDeedExecutionDate = rd.RegistryDoneDate,
                                         StampDutyAmount = rd.StampDutyAmount,
                                         strType = (rd.LeaseType == Constants.intSublease) ? Constants.strSublease : Constants.strLease
                                     });
                return leaseDeedData.ToDataSourceResult(req);
            }
        }

        /// <summary>
        /// Saves Lease Deed details to DB
        /// </summary>
        /// <param name="rId">RID</param>
        /// <param name="LDExecDate">Lease Deed Execution Date</param>
        /// <param name="SA">Signatory Authority</param>
        /// <param name="w1N">Witness 1 Name</param>
        /// <param name="w1A">Witness 1 Address</param>
        /// <param name="w1M">Witness 1 Mobile</param>
        /// <param name="w2N">Witness 2 Name</param>
        /// <param name="w2A">Witness 2 Address</param>
        /// <param name="w2M">Witness 2 Mobile</param>
        /// <param name="w3N">Witness 3 Name</param>
        /// <param name="w3A">Witness 3 Address</param>
        /// <param name="w3M">Witness 3 Mobile</param>
        /// <param name="w4N">Witness 4 Name</param>
        /// <param name="w4A">Witness 4 Address</param>
        /// <param name="w4M">Witness 4 Mobile</param>
        /// <param name="LDDueDate">Lease Deed Due Date</param>
        /// <param name="regType">Registry Type</param>
        /// <param name="SDAmt">Stamp Duty Amount</param>
        /// <param name="DSDAmt">Duplicate Stamp Suty Amount</param>
        /// <param name="prevDues">Previous Dues</param>
        /// <param name="balDueTD">Balance Due till Date</param>
        /// <returns>flag = true -> Successfully saved; flag = false -> Lease Deed already exists for the given RId</returns>
        public bool SaveLeaseDeedDetails(int rId, DateTime LDExecDate, string SA, string w1N, string w1A, string w1M, string w2N, string w2A, string w2M, string w3N, string w3A, string w3M, string w4N, string w4A, string w4M, DateTime LDDueDate, string regType, decimal SDAmt, decimal DSDAmt, decimal prevDues, decimal balDueTD, string viewName, int type, decimal docCharges)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingRegistry = (from r in dbContext.RegistryDetails where r.Rid == rId select r).FirstOrDefault();
                var alloteedetails = (from rd in dbContext.AllotmentMasters
                                      join appdet in dbContext.ApplicationDetails on rd.rid equals appdet.registrationId
                                      join spt in dbContext.SchemePropTrans on rd.propertyId equals spt.propertyId
                                      where rd.rid == rId
                                      select new LeaseRentViewModel
                                      {
                                          DepartmentId = appdet.departmentId,
                                          Sector = spt.SectorMst.sectorName,
                                          Block = spt.BlockMst.blockName,
                                          PlotNo = spt.propertyNo,
                                          PropertyId = spt.propertyId,
                                          Applicant = appdet.tFirstName + " " + appdet.tMiddleName + " " + appdet.tLastName
                                      }).FirstOrDefault();
                if (existingRegistry == null)
                {
                    var registry = new RegistryDetail();
                    registry.Rid = rId;
                    registry.RegistryDueDate = LDDueDate;
                    registry.RegistryDoneDate = LDExecDate;
                    registry.RegistryType = regType;
                    registry.StampDutyAmount = SDAmt;
                    registry.DuplicateStampDutyAmount = DSDAmt;
                    registry.PreviousDues = prevDues;
                    registry.BalanceDueTillDate = balDueTD;
                    registry.IsActive = true;
                    registry.CreatedBy = userInfo.UserID;
                    registry.CreatedDate = DateTime.Now;
                    registry.SignatoryAuthority = SA;
                    registry.WitnessOneName = w1N;
                    registry.WitnessOneAdd = w1A;
                    registry.WitnessOneMobile = w1M;
                    registry.WitnessTwoName = w2N;
                    registry.WitnessTwoAdd = w2A;
                    registry.WitnessTwoMobile = w2M;
                    registry.WitnessThreeName = w3N;
                    registry.WitnessThreeAdd = w3A;
                    registry.WitnessThreeMobile = w3M;
                    registry.WitnessFourName = w4N;
                    registry.WitnessFourAdd = w4A;
                    registry.WitnessFourMobile = w4M;
                    registry.LeaseType = type;
                    registry.DocumentCharges = docCharges;
                    dbContext.RegistryDetails.Add(registry);
                    dbContext.SaveChanges();
                    flag = true;

                    var oldObjModel = new LeaseDeedProperty();
                    //Generate new Model for Audit Trail
                    var newObjModel = new LeaseDeedProperty
                    {
                        RId = rId,
                        LeaseDeedDueDate = LDDueDate,
                        LeaseDeedExecutionDate = LDExecDate,
                        RegistryType = regType,
                        StampDutyAmount = SDAmt,
                        DuplicateStampValue = DSDAmt,
                        PreviousDues = prevDues,
                        BalanceDues = balDueTD,
                        IsActive = true,
                        SignatoryAuthority = SA,
                        Witness1Name = w1N,
                        Witness1Add = w1A,
                        Witness1Mob = w1M,
                        Witness2Name = w2N,
                        Witness2Add = w2A,
                        Witness2Mob = w2M,
                        Witness3Name = w3N,
                        Witness3Add = w3A,
                        Witness3Mob = w3M,
                        Witness4Name = w4N,
                        Witness4Add = w4A,
                        Witness4Mob = w4M,
                        DocCharges = docCharges
                    };
                    GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.RegistryDetails, viewName, registry.RequestNo, oldObjModel, newObjModel, userInfo.UserID.ToString());
                }
                //else -> no coding -> case for checking whether Lease Deed has already been generated for the given RId -> In this case, flag = false
            }
            return flag;
        }

        /// <summary>
        /// Fetches Leased Deed details on the basis of RID
        /// </summary>
        /// <param name="id">RID</param>
        /// <returns></returns>
        public LeaseDeedProperty GetLeaseDeedDetailsByRId(int id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var details = (from rd in dbContext.RegistryDetails
                               join allotmaster in dbContext.AllotmentMasters on rd.Rid equals allotmaster.rid
                               join appdet in dbContext.ApplicationDetails on allotmaster.formNo equals appdet.formNo
                               join dm in dbContext.DepartmentMsts on appdet.departmentId equals dm.departmentId
                               join spt in dbContext.SchemePropTrans on allotmaster.propertyId equals spt.propertyId
                               join pm in dbContext.PropertyTypeMsts on spt.propertyTypeId equals pm.propertyTypeId
                               join sm in dbContext.SectorMsts on spt.sectorId equals sm.sectorId
                               join bm in dbContext.BlockMsts on spt.blockId equals bm.blockId
                               where rd.Rid == id
                               select new LeaseDeedProperty
                               {
                                   RId = rd.Rid.Value,
                                   LeaseDeedExecutionDate = rd.RegistryDoneDate,
                                   SignatoryAuthority = rd.SignatoryAuthority,
                                   AllotteeName = appdet.firstName + " " + appdet.middleName + " " + appdet.lastName,
                                   FatherName = appdet.fatherHusbandName,
                                   DepttName = dm.departmentName,
                                   PropTypeName = pm.propertyTypeName,
                                   PropertyNumber = sm.sectorName + "/" + bm.blockName + "-" + spt.propertyNo,
                                   PropertyCost = spt.totalPropertyCost,
                                   StampDutyAmount = rd.StampDutyAmount,
                                   DuplicateStampValue = rd.DuplicateStampDutyAmount,
                                   PreviousDues = rd.PreviousDues,
                                   BalanceDues = rd.BalanceDueTillDate,
                                   RegistryType = rd.RegistryType,
                                   LeaseDeedDueDate = rd.RegistryDueDate,
                                   Witness1Name = rd.WitnessOneName,
                                   Witness1Add = rd.WitnessOneAdd,
                                   Witness1Mob = rd.WitnessOneMobile,
                                   Witness2Name = rd.WitnessTwoName,
                                   Witness2Add = rd.WitnessTwoAdd,
                                   Witness2Mob = rd.WitnessTwoMobile,
                                   Witness3Name = rd.WitnessThreeName,
                                   Witness3Add = rd.WitnessThreeAdd,
                                   Witness3Mob = rd.WitnessThreeMobile,
                                   Witness4Name = rd.WitnessFourName,
                                   Witness4Add = rd.WitnessFourAdd,
                                   Witness4Mob = rd.WitnessFourMobile,
                                   SchemeId = allotmaster.schemeId,
                                   DepttId = allotmaster.departmentId,
                                   PropId = allotmaster.propertyId,
                                   DocCharges = rd.DocumentCharges
                                   //TODO: LeaseRent = 99.99M
                               }).FirstOrDefault();
                return details;
            }
        }

        /// <summary>
        /// For reading grid data on ManageMutation screen from DB for Requestor
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public DataSourceResult GetMutationData(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();
                var allMutations = (from mut in dbContext.Succ_Mut_Trans //group mut by mut.Rid into groups select groups.OrderByDescending(p => p.Request_No).First()
                                    join allma in dbContext.AllotmentMasters on mut.Rid equals allma.rid
                                    join spt in dbContext.SchemePropTrans on allma.propertyId equals spt.propertyId
                                    join depma in dbContext.DepartmentMsts on spt.departmentId equals depma.departmentId
                                    join secma in dbContext.SectorMsts on spt.sectorId equals secma.sectorId
                                    join bloma in dbContext.BlockMsts on spt.blockId equals bloma.blockId
                                    join statusMas in dbContext.StatusMasters on mut.Status equals statusMas.Id
                                    where mut.Type.ToLower() == TransferType.M.ToString().ToLower() && loginUserDeptt.Contains(spt.departmentId) && allma.isActive == 1
                                    //&& mut.Requested_By == userInfo.UserID
                                    select new MutationModel
                                    {
                                        ReqNo = mut.Request_No,
                                        RId = mut.Rid,
                                        DepttName = depma.departmentName,
                                        SectorName = secma.sectorName,
                                        BlockName = bloma.blockName,
                                        PropertyNo = spt.propertyNo,
                                        PropNo = secma.sectorName + "/" + bloma.blockName + "-" + spt.propertyNo,
                                        ReqDate = mut.Requested_Date,
                                        MutationDate = mut.Mutation_Date,
                                        Status = statusMas.Status
                                    });
                var distinct = from allMu in allMutations group allMu by allMu.RId into groups select groups.OrderByDescending(p => p.ReqNo).FirstOrDefault();
                return distinct.ToDataSourceResult(req);
            }
        }

        /// <summary>
        /// For reading grid data on ManageMutation screen from DB for Approver
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public DataSourceResult GetMutationDataByApproverId(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();
                var allMutations = (from mut in dbContext.Succ_Mut_Trans //group mut by mut.Rid into groups select groups.OrderByDescending(p => p.Request_No).First()
                                    join allma in dbContext.AllotmentMasters on mut.Rid equals allma.rid
                                    join spt in dbContext.SchemePropTrans on allma.propertyId equals spt.propertyId
                                    join depma in dbContext.DepartmentMsts on spt.departmentId equals depma.departmentId
                                    join secma in dbContext.SectorMsts on spt.sectorId equals secma.sectorId
                                    join bloma in dbContext.BlockMsts on spt.blockId equals bloma.blockId
                                    join statusMas in dbContext.StatusMasters on mut.Status equals statusMas.Id
                                    where mut.Type.ToLower() == TransferType.M.ToString().ToLower() && mut.Approved_By == userInfo.UserID && allma.isActive == 1
                                    //&& loginUserDeptt.Contains(allma.departmentId)
                                    select new MutationModel
                                    {
                                        ReqNo = mut.Request_No,
                                        RId = mut.Rid,
                                        DepttName = depma.departmentName,
                                        SectorName = secma.sectorName,
                                        BlockName = bloma.blockName,
                                        PropertyNo = spt.propertyNo,
                                        PropNo = secma.sectorName + "/" + bloma.blockName + "-" + spt.propertyNo,
                                        ReqDate = mut.Requested_Date,
                                        MutationDate = mut.Mutation_Date,
                                        Status = statusMas.Status
                                    });
                var distinct = from allMu in allMutations group allMu by allMu.RId into groups select groups.OrderByDescending(p => p.ReqNo).FirstOrDefault();
                return distinct.ToDataSourceResult(req);
            }
        }

        /// <summary>
        /// Saves new Mutation Details in the DB  
        /// </summary>
        /// <param name="mutDate">Mutation Date</param>
        /// <param name="transDeedDate">Transfer Deed Date</param>
        /// <param name="bahiNo">Bahi No.</param>
        /// <param name="bahiZildNo">Bahi Zild No.</param>
        /// <param name="bahiPageNo">Bahi Page No.</param>
        /// <param name="SINo">SI No.</param>
        /// <param name="userVal">Approver's ID</param>
        /// <param name="ReqNo">Request No.</param>
        /// <param name="rId">RID</param>
        /// <returns>True -> Saved successfully; False -> No new Transfer exists for this RID</returns>
        public bool SaveMutationDetails(DateTime mutDate, DateTime transDeedDate, string bahiNo, string bahiZildNo, string bahiPageNo, string SINo, int userVal, int ReqNo, int rId, DateTime transDate, string transType, int? ReqRefNo)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                //var RIDByReqNo = (from smt in dbContext.Succ_Mut_Trans where smt.Request_No == ReqNo select smt.Rid).FirstOrDefault();
                var approvedStatus = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Approved.ToString().ToLower() select st.Id).FirstOrDefault();
                var existingTransfer = (from smt in dbContext.Succ_Mut_Trans where smt.Rid == rId && smt.Is_Active == true orderby smt.Request_No descending select smt).FirstOrDefault();
                //var exTransfer = (from smt in dbContext.Succ_Mut_Trans where smt.Rid == rId && smt.Is_Active == true && smt.Type=="T" orderby smt.Request_No descending select smt).FirstOrDefault();
                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == rId select appDet).FirstOrDefault();
                //var newOwnerAge = CalculateAge(newOwner.tDateOfBirth.Value, DateTime.Now);
                if (existingTransfer != null && newOwner != null)
                {
                    if (existingTransfer.Type.ToLower() == TransferType.T.ToString().ToLower() && existingTransfer.Status == approvedStatus)
                    {
                        //updated by atul yadav according to vishal shukla
                        var newMutation = new Succ_Mut_Trans();

                        newMutation.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.InProgress.ToString().ToLower() select st.Id).FirstOrDefault();
                        newMutation.Rid = rId;
                        newMutation.Property_Id = existingTransfer.Property_Id;
                        newMutation.Applicant_Name = existingTransfer.Applicant_Name;
                        newMutation.ApplicantFather_Name = existingTransfer.ApplicantFather_Name;
                        newMutation.Applicant_Signing_Authority = existingTransfer.Applicant_Signing_Authority;
                        newMutation.Applicant_Registered_Office = existingTransfer.Applicant_Registered_Office;
                        newMutation.Applicant_Gender = existingTransfer.Applicant_Gender;
                        newMutation.Applicant_Age = existingTransfer.Applicant_Age;
                        newMutation.Applicant_Occupation = existingTransfer.Applicant_Occupation;
                        newMutation.Correspondance_Add = existingTransfer.Correspondance_Add;
                        newMutation.Permanent_Add = existingTransfer.Permanent_Add;
                        newMutation.Selling_Cost = existingTransfer.Selling_Cost;
                        newMutation.Amount = existingTransfer.Amount;
                        newMutation.Transfer_Date = existingTransfer.Transfer_Date;
                        newMutation.Requested_By = userInfo.UserID;
                        newMutation.Requested_Date = DateTime.Now;
                        newMutation.Approved_By = userVal;
                        newMutation.Type = TransferType.M.ToString();
                        newMutation.Mutation_Date = mutDate;
                        newMutation.Transfer_Sub_Type = existingTransfer.Transfer_Sub_Type;
                        newMutation.TransferDeed_Date = transDeedDate;
                        newMutation.Bahi_No = bahiNo;
                        newMutation.Bahi_Zild_No = bahiZildNo;
                        newMutation.Bahi_Page_No = bahiPageNo;
                        newMutation.Bahi_Series_No = SINo;
                        newMutation.Is_Active = true;
                        newMutation.Transfer_Type = existingTransfer.Transfer_Type;
                        newMutation.T_Gender = newOwner.tGender;
                        newMutation.T_First_Name = newOwner.tFirstName;
                        newMutation.T_Middle_Name = newOwner.tMiddleName;
                        newMutation.T_Last_Name = newOwner.tLastName;
                        newMutation.T_Father_Husband_Name = newOwner.tFatherHusbandName;
                        newMutation.T_Mother_Name = newOwner.tMotherName;
                        newMutation.T_Company_Name = newOwner.T_Company_Name;
                        newMutation.T_Signing_Authority = newOwner.tSigningAuthority;
                        newMutation.T_Email = newOwner.tEmail;
                        newMutation.T_Mobile = newOwner.tMobileNumber;
                        newMutation.T_Correspondence_Add = newOwner.tCorrespondanceAdd;
                        newMutation.T_Permanent_Add = newOwner.tPermanentAdd;
                        newMutation.Applicant_Registered_Office = newOwner.tRegisteredOffice;
                        newMutation.T_Occupation_Id = newOwner.tOccupationId;
                        newMutation.T_Pan = newOwner.tPan;
                        newMutation.OnlineRequestNo = ReqRefNo;
                        newMutation.Created_By = userInfo.UserID;
                        newMutation.Created_Date = DateTime.Now;
                        newMutation.GPA_Holder_Name = existingTransfer.GPA_Holder_Name;
                        newMutation.GPA_Holder_Address = existingTransfer.GPA_Holder_Address;
                        newMutation.GPA_Effective_From = existingTransfer.GPA_Effective_From;
                        newMutation.GPA_Effective_To = existingTransfer.GPA_Effective_To;
                        dbContext.Succ_Mut_Trans.Add(newMutation);
                        dbContext.SaveChanges();
                        flag = true;
                        //Send notifications
                        //Email
                        var body = "Hi,<br><br>Your Mutation Request has been submitted.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                        EmailHelper emailHelper = new EmailHelper();
                        emailHelper.Send(newOwner.tEmail, "Mutation Request Status", body);
                        //SMS
                        var msg = NAMessages.MutationReqSubmit;
                        //SMSSend(newOwner.tMobileNumber, msg);
                        ApplicationHelper.SendSMS(newOwner.tMobileNumber, msg);

                        //create  by mankaran
                        //var newMutation = new Succ_Mut_Trans();
                        //newMutation.Mutation_Date = mutDate;
                        //newMutation.TransferDeed_Date = transDeedDate;
                        //newMutation.Bahi_No = bahiNo;
                        //newMutation.Bahi_Zild_No = bahiZildNo;
                        //newMutation.Bahi_Page_No = bahiPageNo;
                        //newMutation.Bahi_Series_No = SINo;
                        //newMutation.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Pending.ToString().ToLower() select st.Id).FirstOrDefault();
                        //newMutation.Rid = rId;
                        //newMutation.Property_Id = existingTransfer.Property_Id;
                        //newMutation.Applicant_Name = newOwner.tFirstName + " " + newOwner.tMiddleName + " " + newOwner.tLastName;
                        //newMutation.T_Father_Husband_Name = existingTransfer.T_Father_Husband_Name;
                        //newMutation.ApplicantFather_Name = existingTransfer.ApplicantFather_Name;
                        //newMutation.Applicant_Signing_Authority = newOwner.tSigningAuthority;
                        //newMutation.Applicant_Registered_Office = newOwner.tRegisteredOffice;
                        //newMutation.Applicant_Gender = newOwner.tGender;
                        ////newMutation.Applicant_Age = newOwnerAge;
                        //newMutation.Applicant_Occupation = (from ao in dbContext.OccupationMsts where ao.occupationId == newOwner.tOccupationId select ao.occupation).FirstOrDefault();
                        //newMutation.Correspondance_Add = newOwner.tCorrespondanceAdd;
                        //newMutation.Permanent_Add = newOwner.tPermanentAdd;
                        //newMutation.Approved_By = userVal;
                        ////newMutation.Selling_Cost = existingTransfer.Selling_Cost;
                        ////newMutation.Amount = existingTransfer.Amount;
                        //newMutation.Transfer_Date = existingTransfer.Transfer_Date;
                        //newMutation.Requested_By = userInfo.UserID;
                        //newMutation.Requested_Date = DateTime.Now;
                        //newMutation.Type = TransferType.M.ToString();
                        //newMutation.Transfer_Sub_Type = existingTransfer.Transfer_Sub_Type;
                        ////newMutation.Book_No = existingTransfer.Book_No;
                        ////newMutation.Book_Zild_No = existingTransfer.Book_Zild_No;
                        ////newMutation.Book_Series_No = existingTransfer.Book_Series_No;
                        //newMutation.Is_Active = true;
                        //newMutation.Created_By = userInfo.UserID;
                        //newMutation.Created_Date = DateTime.Now;
                        //newMutation.OnlineRequestNo = ReqRefNo;
                        //dbContext.Succ_Mut_Trans.Add(newMutation);
                        //dbContext.SaveChanges();
                        //flag = true;
                        ////Send notifications
                        ////Email
                        //var body = "Hi,<br><br>Your Mutation Request has been submitted.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                        //EmailHelper emailHelper = new EmailHelper();
                        //emailHelper.Send(newOwner.tEmail, "Mutation Request Status", body);
                        ////SMS
                        //var msg = NAMessages.MutationReqSubmit;
                        //SMSSend(newOwner.tMobileNumber, msg);
                    }
                    //else -> no Record exists, flag = false and send message behind that no new Transfer exists for this RID.
                }
                //record.M_Requested_By = userVal;
                //var record = (from mut in dbContext.Succ_Mut_Trans where mut.Mutation_Date == null orderby mut.Request_No descending select mut).FirstOrDefault();
                //if (record != null)
                //{
                //    record.Mutation_Date = mutDate;
                //    record.TransferDeed_Date = transDeedDate;
                //    record.Bahi_No = bahiNo;
                //    record.Bahi_Zild_No = bahiZildNo;
                //    record.Bahi_Page_No = bahiPageNo;
                //    record.Bahi_Series_No = SINo;
                //    //record.M_Requested_By = userVal;
                //    dbContext.SaveChanges();
                //    flag = true;
                //}
                //else -> no Record exists, flag = false and send message behind that no new Transfer exists for this RID.
            }
            return flag;
        }

        /// <summary>
        /// Updates Mutation Details for an existing Mutation.
        /// </summary>
        /// <param name="mutDate"></param>
        /// <param name="transDeedDate"></param>
        /// <param name="bahiNo"></param>
        /// <param name="bahiZildNo"></param>
        /// <param name="bahiPageNo"></param>
        /// <param name="SINo"></param>
        /// <param name="userVal"></param>
        /// <param name="ReqNo"></param>
        /// <param name="rId"></param>
        /// <returns></returns>
        public bool UpdateMutationDetails(DateTime mutDate, DateTime transDeedDate, string bahiNo, string bahiZildNo, string bahiPageNo, string SINo, int userVal, int ReqNo, int rId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                //var approvedStatus = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Approved.ToString().ToLower() select st.Id).FirstOrDefault();
                var existingMutation = (from smt in dbContext.Succ_Mut_Trans where smt.Rid == rId orderby smt.Request_No descending select smt).FirstOrDefault();
                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == rId select appDet).FirstOrDefault();//For notification details
                //var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == rId select appDet).FirstOrDefault();
                //var newOwnerAge = CalculateAge(newOwner.tDateOfBirth.Value, DateTime.Now);
                if (existingMutation != null)
                {
                    if (existingMutation.Type.ToLower() == TransferType.M.ToString().ToLower())
                    {
                        //var newMutation = new Succ_Mut_Trans();
                        existingMutation.Mutation_Date = mutDate;
                        existingMutation.TransferDeed_Date = transDeedDate;
                        existingMutation.Bahi_No = bahiNo;
                        existingMutation.Bahi_Zild_No = bahiZildNo;
                        existingMutation.Bahi_Page_No = bahiPageNo;
                        existingMutation.Bahi_Series_No = SINo;
                        existingMutation.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.InProgress.ToString().ToLower() select st.Id).FirstOrDefault();
                        //newMutation.Rid = rId;
                        //newMutation.Property_Id = existingTransfer.Property_Id;
                        //newMutation.Applicant_Name = newOwner.tFirstName + " " + newOwner.tMiddleName + " " + newOwner.tLastName;
                        //newMutation.ApplicantFather_Name = newOwner.tFatherHusbandName;
                        //newMutation.Applicant_Signing_Authority = newOwner.tSigningAuthority;
                        //newMutation.Applicant_Registered_Office = newOwner.tRegisteredOffice;
                        //newMutation.Applicant_Gender = newOwner.tGender;
                        //newMutation.Applicant_Age = newOwnerAge;
                        //newMutation.Applicant_Occupation = (from ao in dbContext.OccupationMsts where ao.occupationId == newOwner.tOccupationId select ao.occupation).FirstOrDefault();
                        //newMutation.Correspondance_Add = newOwner.tCorrespondanceAdd;
                        //newMutation.Permanent_Add = newOwner.tPermanentAdd;
                        existingMutation.Approved_By = userVal;
                        existingMutation.Requested_By = userInfo.UserID;
                        existingMutation.Requested_Date = DateTime.Now;
                        //newMutation.Type = TransferType.M.ToString();
                        //newMutation.Is_Active = true;
                        existingMutation.Modified_By = userInfo.UserID;
                        existingMutation.Modified_Date = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = true;
                        //Send notifications
                        //Email
                        var body = "Hi,<br><br>Your Mutation Request has been submitted.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                        EmailHelper emailHelper = new EmailHelper();
                        emailHelper.Send(newOwner.tEmail, "Mutation Request Status", body);
                        //SMS
                        var msg = NAMessages.MutationReqSubmit;
                        //SMSSend(newOwner.tMobileNumber, msg);
                        ApplicationHelper.SendSMS(newOwner.tMobileNumber, msg);
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// Used for filling RID dropdown in Mutation screens (from Succ_Mut_Trans table with Transfer of type "Approved" only) on the basis of login User's Departments
        /// </summary>
        /// <returns>List of RIDs</returns>
        public DataSourceResult GetRIDsForMutation(DataSourceRequest Req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();
                var lstRId = (from mut in dbContext.Succ_Mut_Trans
                              join st in dbContext.StatusMasters on mut.Status equals st.Id
                              join am in dbContext.AllotmentMasters on mut.Rid equals am.rid
                              where st.Status.ToLower() == AllotmentStatus.Approved.ToString().ToLower() && mut.Type.ToLower() == TransferType.T.ToString().ToLower() && loginUserDeptt.Contains(am.departmentId) && am.isActive == 1
                              orderby mut.Request_No descending
                              select new DDList
                              {
                                  id = mut.Rid.Value,
                                  text = mut.Rid.ToString()
                              });
                var distinctRIds = lstRId.GroupBy(x => x.id).Select(y => new DDList { id = y.Select(i => i.id).FirstOrDefault(), text = y.Select(i => i.text).FirstOrDefault() }).OrderByDescending(z => z.id);
                //lstRId.GroupBy(x => x.id).Select(y => y.First()).OrderByDescending(z => z.id).ToList();
                return distinctRIds.ToDataSourceResult(Req);
            }
        }

        /// <summary>
        /// To pick RIDs for Transfer Request (only those whose Lease Deed has been executed). 
        /// </summary>
        /// <returns>List of RIDs</returns>
        public DataSourceResult GetRIDsForTransfer(DataSourceRequest Req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();
                //var lstRId = (from rd in dbContext.RegistryDetails
                //              join am in dbContext.AllotmentMasters on rd.Rid equals am.rid
                //              where rd.IsActive == true && loginUserDeptt.Contains(am.departmentId)
                //              select new DDList
                //              {
                //                  id = rd.Rid.Value,
                //                  text = rd.Rid.ToString()
                //              }).ToList();
                var lstRId = (from pd in dbContext.PossessionDetails //Resolved Feature#216
                              join am in dbContext.AllotmentMasters on pd.Rid equals am.rid
                              where pd.IsActive == true && loginUserDeptt.Contains(am.departmentId) && am.isActive == 1 && pd.Possession == true
                              select new DDList
                              {
                                  id = pd.Rid.Value,
                                  text = pd.Rid.ToString()
                              });
                var distinctRIds = lstRId.GroupBy(x => x.id).Select(y => new DDList { id = y.Select(i => i.id).FirstOrDefault(), text = y.Select(i => i.text).FirstOrDefault() }).OrderByDescending(z => z.id);//.Select(y => y.First()).OrderByDescending(z => z.id).ToList();
                return distinctRIds.ToDataSourceResult(Req);
            }
        }


        /// <summary>
        /// To pick RIDs for Transfer Request (only those whose Lease Deed has been executed). 
        /// </summary>
        /// <returns>List of RIDs</returns>
        public DataSourceResult GetRIDsForTransfer(DataSourceRequest Req, int Rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();
                //var lstRId = (from rd in dbContext.RegistryDetails
                //              join am in dbContext.AllotmentMasters on rd.Rid equals am.rid
                //              where rd.IsActive == true && loginUserDeptt.Contains(am.departmentId)
                //              select new DDList
                //              {
                //                  id = rd.Rid.Value,
                //                  text = rd.Rid.ToString()
                //              }).ToList();
                var lstRId = (from pd in dbContext.PossessionDetails //Resolved Feature#216
                              join am in dbContext.AllotmentMasters on pd.Rid equals am.rid
                              where pd.IsActive == true && loginUserDeptt.Contains(am.departmentId) && am.isActive == 1 && pd.Possession == true &&
                              pd.Rid == Rid
                              select new DDList
                              {
                                  id = pd.Rid.Value,
                                  text = pd.Rid.ToString()
                              });
                var distinctRIds = lstRId.GroupBy(x => x.id).Select(y => new DDList { id = y.Select(i => i.id).FirstOrDefault(), text = y.Select(i => i.text).FirstOrDefault() }).OrderByDescending(z => z.id);//.Select(y => y.First()).OrderByDescending(z => z.id).ToList();
                return distinctRIds.ToDataSourceResult(Req);
            }
        }

        /// <summary>
        /// Used for fetching Mutation details by Request No. (erstwhile was used for picking details by RID)
        /// </summary>
        /// <param name="reqNo">Request No.</param>
        /// <returns></returns>
        public MutationModel GetMutationDetailsByRId(int reqNo)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from mut in dbContext.Succ_Mut_Trans
                            join am in dbContext.AllotmentMasters on mut.Rid equals am.rid
                            join appDet in dbContext.ApplicationDetails on am.rid equals appDet.registrationId
                            join spt in dbContext.SchemePropTrans on am.propertyId equals spt.propertyId
                            join scm in dbContext.SchemeMsts on spt.schemeId equals scm.schemeId
                            join dm in dbContext.DepartmentMsts on am.departmentId equals dm.departmentId
                            join pm in dbContext.PropertyTypeMsts on spt.propertyTypeId equals pm.propertyTypeId
                            join sm in dbContext.SectorMsts on spt.sectorId equals sm.sectorId
                            join bm in dbContext.BlockMsts on spt.blockId equals bm.blockId
                            join fm in dbContext.FloorMsts on spt.floorId equals fm.floorId
                            join sta in dbContext.StatusMasters on mut.Status equals sta.Id
                            //join tranType in dbContext.Transfer_Type on mut.Transfer_Type equals tranType.Id
                            where mut.Request_No == reqNo
                            select new MutationModel
                            {
                                RId = mut.Rid,
                                PropNo = sm.sectorName + "/" + bm.blockName + "-" + spt.propertyNo,
                                SchemeName = scm.schemeName,
                                DepttName = dm.departmentName,
                                //ApplicantName = mut.Applicant_Name, -> For fixing bug on 28th Feb. 2017
                                ApplicantName = (from trans in dbContext.Succ_Mut_Trans where trans.Rid == mut.Rid && trans.Type == TransferType.T.ToString() orderby trans.Request_No descending select trans.Applicant_Name).FirstOrDefault(),//-> For fixing bug on 28th Feb. 2017
                                ApplicantAddress = mut.Permanent_Add,
                                Gender = mut.Applicant_Gender,
                                RelationName = mut.ApplicantFather_Name,
                                PropType = pm.propertyTypeName,
                                Area = spt.totalArea,
                                Floor = fm.floorName,
                                MutationDate = mut.Mutation_Date,
                                TransferDeedDate = mut.TransferDeed_Date,
                                BahiNo = mut.Bahi_No,
                                BahiZildNo = mut.Bahi_Zild_No,
                                BahiPageNo = mut.Bahi_Page_No,
                                SINo = mut.Bahi_Series_No,
                                Status = sta.Status,
                                Comment = mut.Comment,
                                From = (from us in dbContext.UmUserMasters join mu in dbContext.Succ_Mut_Trans on us.UserRefId equals mut.Approved_By select us.FirstName + " " + us.MiddleName + " " + us.LastName).FirstOrDefault(),
                                SubmittedDate = mut.Requested_Date,
                                //TransfereeName = mut.Applicant_Name,
                                //TransfereeGender = mut.Applicant_Gender,
                                //TransfereeRelationName = mut.ApplicantFather_Name,
                                //TransferType = mut.Transfer_Sub_Type,
                                //TransferDate = mut.Transfer_Date
                                //TransfereeName = appDet.tFirstName + " " + appDet.tMiddleName + " " + appDet.tLastName, -> For fixing bug on 28th Feb. 2017
                                TransfereeName = appDet.tGender.ToLower() == Constants.Company.ToLower() ? appDet.T_Company_Name : appDet.tFirstName + " " + (!string.IsNullOrEmpty(appDet.tMiddleName) ? appDet.tMiddleName + " " : string.Empty) + appDet.tLastName, //-> For fixing bug on 28th Feb. 2017
                                TransfereeGender = appDet.tGender,
                                TransfereeRelationName = appDet.tFatherHusbandName,
                                TransferDate = mut.Transfer_Date,
                                //TransferType = tranType.type,
                                TransferType = (from m in dbContext.Succ_Mut_Trans join tranType in dbContext.Transfer_Type on m.Transfer_Type equals tranType.Id where m.Type == TransferType.T.ToString() orderby m.Request_No descending select tranType.type).FirstOrDefault(), //Resolved Bug#258
                                TransfereeAddress = appDet.tPermanentAdd,
                                ReqNo = mut.Request_No,
                                DepttId = am.departmentId.Value, //Resolved Bug#260
                                OnlineRequestRefNo = mut.OnlineRequestNo != null ? mut.OnlineRequestNo : 0
                            }).FirstOrDefault();
                return data;
            }
        }

        /// <summary>
        /// Cancels Mutation Request by Requestor
        /// </summary>
        /// <param name="reqNo">Request No.</param>
        /// <returns></returns>
        public bool CancelMutationRequest(int reqNo)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingRequest = (from mut in dbContext.Succ_Mut_Trans where mut.Request_No == reqNo select mut).FirstOrDefault();
                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == existingRequest.Rid select appDet).FirstOrDefault();//For notification details
                if (existingRequest != null)
                {
                    existingRequest.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Cancelled.ToString().ToLower() select st.Id).FirstOrDefault();
                    existingRequest.Modified_By = userInfo.UserID;
                    existingRequest.Modified_Date = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = true;
                    //Send notifications
                    //Email
                    var body = "Hi,<br><br>Your Mutation Request has been cancelled.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                    EmailHelper emailHelper = new EmailHelper();
                    emailHelper.Send(newOwner.tEmail, "Mutation Request Status", body);
                    //SMS
                    var msg = NAMessages.MutationReqCancel;
                    //SMSSend(newOwner.tMobileNumber, msg);
                    ApplicationHelper.SendSMS(newOwner.tMobileNumber, msg);
                }
            }
            return flag;
        }

        /// <summary>
        /// Saves Mutation -> Approver Status and Comments in DB
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="intStatus">= 1 for Approved; = 2 for Rejected</param>
        /// <returns></returns>
        public bool SaveApprovalStatus(string comments, int intStatus, int ReqNo)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingRequest = (from mut in dbContext.Succ_Mut_Trans where mut.Request_No == ReqNo select mut).FirstOrDefault();
                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == existingRequest.Rid select appDet).FirstOrDefault();//For notification details
                if (existingRequest != null)
                {
                    if (intStatus == 1)
                    {
                        existingRequest.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Approved.ToString().ToLower() select st.Id).FirstOrDefault();
                        //var kya = dbContext.KYADetails.FirstOrDefault(m => m.RId == existingRequest.Rid && m.IsActive == true);
                        //if (kya != null)
                        //{
                        //    kya.IsActive = false;
                        //}                       
                    }
                    else
                        existingRequest.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Rejected.ToString().ToLower() select st.Id).FirstOrDefault();
                    existingRequest.Approved_Date = DateTime.Now;
                    existingRequest.Comment = comments;
                    existingRequest.Modified_By = userInfo.UserID;
                    existingRequest.Modified_Date = DateTime.Now;
                    dbContext.SaveChanges();

                    if (intStatus == 1)
                    {
                        if (existingRequest.OnlineRequestNo != 0)
                        {
                            UpdateServiceRequest(existingRequest.OnlineRequestNo, existingRequest.Rid, newOwner.departmentId, comments);
                        }
                    }
                    flag = true;
                    //Send notifications
                    //Email
                    string body = string.Empty;
                    string msg = string.Empty;
                    if (intStatus == 1)
                    {
                        body = "Hi,<br><br>Your Mutation Request has been approved. Kindly clear all your Dues.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                        msg = "Your Mutation Request has been approved. Kindly clear all your Dues.";
                    }
                    else
                    {
                        body = "Hi,<br><br>Your Mutation Request has been rejected.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                        msg = NAMessages.MuattionReqReject;
                    }
                    EmailHelper emailHelper = new EmailHelper();
                    emailHelper.Send(newOwner.tEmail, "Mutation Request Status", body);
                    //SMS
                    //SMSSend(newOwner.tMobileNumber, msg);
                    ApplicationHelper.SendSMS(newOwner.tMobileNumber, msg);
                }
            }
            return flag;
        }

        public DateTime? GetChklstDateByChklstType(int rId, int type)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var chklstDate = (from chk in dbContext.ChecklistTrans where chk.ChecklistTypeId == type && chk.Rid == rId select chk.ChecklistDate).FirstOrDefault();
                //if (chklstDate != null)
                //    return chklstDate.ChecklistDate;
                return chklstDate;
            }
        }

        /// <summary>
        /// Reads Transfer data from DB -> for Requestor
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public DataSourceResult GetTransferData(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
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
                                    select new TransferModel
                                    {
                                        ReqNo = trans.Request_No,
                                        RId = trans.Rid,
                                        DepttName = depma.departmentName,
                                        Sector = secma.sectorName,
                                        Block = bloma.blockName,
                                        PropertyNo = spt.propertyNo,
                                        PropNo = secma.sectorName + "/" + bloma.blockName + "-" + spt.propertyNo,
                                        ReqDate = trans.Requested_Date,
                                        ApprovedDate = trans.Approved_Date,
                                        AssignedTo = users.FirstName + " " + users.MiddleName + " " + users.LastName,
                                        Status = statusMas.Status
                                    });
                var distinct = from allMu in allTransfers group allMu by allMu.RId into groups select groups.OrderByDescending(p => p.ReqNo).FirstOrDefault();
                return allTransfers.ToDataSourceResult(req);
            }
        }

        /// <summary>
        /// Reads Transfer data from DB -> for Requestor
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public DataSourceResult GetTransferData(DataSourceRequest req, int? Rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
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
                                    && trans.Rid == Rid && trans.Status == 1
                                    select new TransferModel
                                    {
                                        ReqNo = trans.Request_No,
                                        RId = trans.Rid,
                                        DepttName = depma.departmentName,
                                        Sector = secma.sectorName,
                                        Block = bloma.blockName,
                                        PropertyNo = spt.propertyNo,
                                        PropNo = secma.sectorName + "/" + bloma.blockName + "-" + spt.propertyNo,
                                        TransferDate = trans.Transfer_Date,
                                        Status = statusMas.Status,
                                        ApplicantName = trans.Applicant_Name,
                                        TransfereeFirstName = trans.T_Gender == Constants.genderCompany ? trans.T_Company_Name : trans.T_First_Name + " " + (trans.T_Middle_Name != null ? (trans.T_Middle_Name + " " + trans.T_Last_Name) : trans.T_Last_Name),
                                        TransfereeCorrespondenceAdd = trans.T_Correspondence_Add
                                    });
                //var distinct = from allMu in allTransfers group allMu by allMu.RId into groups select groups.OrderByDescending(p => p.ReqNo);
                return allTransfers.ToDataSourceResult(req);
            }
        }

        public DataSourceResult GetTransferHistory(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();
                var transHistory = (from trans in dbContext.Succ_Mut_Trans
                                    join allma in dbContext.AllotmentMasters on trans.Rid equals allma.rid
                                    join spt in dbContext.SchemePropTrans on allma.propertyId equals spt.propertyId
                                    join depma in dbContext.DepartmentMsts on spt.departmentId equals depma.departmentId
                                    join secma in dbContext.SectorMsts on spt.sectorId equals secma.sectorId
                                    join bloma in dbContext.BlockMsts on spt.blockId equals bloma.blockId
                                    join statusMas in dbContext.StatusMasters on trans.Status equals statusMas.Id
                                    join users in dbContext.UmUserMasters on trans.Approved_By equals users.UserRefId
                                    join st in dbContext.StatusMasters on trans.Status equals st.Id
                                    where trans.Type.ToLower() == TransferType.T.ToString().ToLower() && loginUserDeptt.Contains(allma.departmentId) && allma.isActive == 1
                                    && st.Status.ToLower() == AllotmentStatus.Approved.ToString().ToLower()
                                    select new TransferModel
                                    {
                                        ReqNo = trans.Request_No,
                                        RId = trans.Rid,
                                        DepttName = depma.departmentName,
                                        Sector = secma.sectorName,
                                        Block = bloma.blockName,
                                        PropertyNo = spt.propertyNo,
                                        PropNo = secma.sectorName + "/" + bloma.blockName + "-" + spt.propertyNo,
                                        ReqDate = trans.Requested_Date,
                                        ApprovedDate = trans.Approved_Date,
                                        AssignedTo = users.FirstName + " " + users.MiddleName + " " + users.LastName,
                                        Status = statusMas.Status,
                                        //ApprovedYear = (trans.Approved_Date != null) ? trans.Approved_Date.Value.Year : 0
                                        ApprovedYear = trans.Approved_Date.Value.Year
                                    });
                return transHistory.ToDataSourceResult(req);
            }
        }

        /// <summary>
        /// Reads Transfer data from DB -> for Approver
        /// </summary>
        /// <param name="req">Kendo internal</param>
        /// <returns></returns>
        public DataSourceResult GetTransferData_Approver(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                //                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                //                      where userMst.UserRefId == userInfo.UserID
                //                      select dept.DepartmentId).ToList();
                var allTransfers = (from trans in dbContext.Succ_Mut_Trans
                                    join allma in dbContext.AllotmentMasters on trans.Rid equals allma.rid
                                    join spt in dbContext.SchemePropTrans on allma.propertyId equals spt.propertyId
                                    join depma in dbContext.DepartmentMsts on spt.departmentId equals depma.departmentId
                                    join secma in dbContext.SectorMsts on spt.sectorId equals secma.sectorId
                                    join bloma in dbContext.BlockMsts on spt.blockId equals bloma.blockId
                                    join statusMas in dbContext.StatusMasters on trans.Status equals statusMas.Id
                                    join users in dbContext.UmUserMasters on trans.Approved_By equals users.UserRefId
                                    where trans.Type.ToLower() == TransferType.T.ToString().ToLower() && trans.Approved_By == userInfo.UserID && allma.isActive == 1 //loginUserDeptt.Contains(allma.departmentId)
                                    select new TransferModel
                                    {
                                        ReqNo = trans.Request_No,
                                        RId = trans.Rid,
                                        DepttName = depma.departmentName,
                                        Sector = secma.sectorName,
                                        Block = bloma.blockName,
                                        PropertyNo = spt.propertyNo,
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

        /// <summary>
        /// Fetching Allotee Details from DB. Subquery is used so that when there is no data in Mortgage table/Functional table, the query still completes 
        /// </summary>
        /// <param name="rId">RID</param>
        /// <returns></returns>
        public TransferModel GetOriginalDetailsForTransferByRID(int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var details = new TransferModel();
                var leaseDeed = (from reg in dbContext.RegistryDetails where reg.Rid == rId && reg.IsActive == true orderby reg.RequestNo descending select reg).FirstOrDefault();
                if (leaseDeed != null)
                {
                    details = (from appDet in dbContext.ApplicationDetails
                               join scMst in dbContext.SchemeMsts on appDet.schemeId equals scMst.schemeId
                               join deMst in dbContext.DepartmentMsts on appDet.departmentId equals deMst.departmentId
                               join allotMas in dbContext.AllotmentMasters on appDet.registrationId equals allotMas.rid
                               join spt in dbContext.SchemePropTrans on allotMas.propertyId equals spt.propertyId
                               join propTypeMas in dbContext.PropertyTypeMsts on spt.propertyTypeId equals propTypeMas.propertyTypeId
                               join floorMst in dbContext.FloorMsts on spt.floorId equals floorMst.floorId
                               join sector in dbContext.SectorMsts on spt.sectorId equals sector.sectorId
                               join block in dbContext.BlockMsts on spt.blockId equals block.blockId
                               //join reg in dbContext.RegistryDetails on appDet.registrationId equals reg.Rid
                               //join pos in dbContext.PossessionDetails on appDet.registrationId equals pos.Rid
                               where appDet.registrationId == rId //&& srt.departmentId == appDet.departmentId
                               select new TransferModel
                               {
                                   SchemeName = scMst.schemeName,
                                   DepttName = deMst.departmentName,
                                   ApplicantName = appDet.tFirstName + " " + appDet.tMiddleName + " " + appDet.tLastName,
                                   Gender = appDet.tGender,
                                   RelationName = appDet.tFatherHusbandName,
                                   SigningAuthorityName = appDet.tSigningAuthority,
                                   PropType = propTypeMas.propertyTypeName,
                                   Area = spt.totalArea,
                                   TransfereeCompanyName = (appDet.T_Company_Name != null && appDet.T_Company_Name != "") ? appDet.T_Company_Name : appDet.tFirstName + " " + appDet.tMiddleName + " " + appDet.tLastName,
                                   TransfereeCompanySigningAuth = appDet.tSigningAuthority,
                                   Floor = floorMst.floorName,
                                   PropMortgaged = ((from mort in dbContext.MortgageDetails where (mort.PreviousLoanNoc != MortPrevLoan.invalid || (mort.StatusId == StatusOption.Approved || mort.StatusId == StatusOption.InProgress)) && (mort.RID == rId && mort.IsActive == true) orderby mort.RequestNo descending select mort).FirstOrDefault() == null) ? Constants.no : Constants.yes,
                                   PropFunctional = ((from func in dbContext.FunctionalDetails where func.StatusId == StatusOption.Approved && func.Rid == rId && func.IsActive == true orderby func.RequestNo descending select func).FirstOrDefault() == null) ? Constants.no : Constants.yes,
                                   OneTimeLeaseRent = (from rdm in dbContext.RECEIPT_DETAIL_MASTER join rat in dbContext.RECEIPT_AMOUNT_TRANS on rdm.RECEIPT_ID equals rat.RECEIPT_ID where rdm.RID_NO == rId.ToString() && rat.RECEIPT_HEAD_ID == Constants.OTLRReceiptHead && rat.RECEIPT_SUBHEAD_ID == Constants.OTLRReceiptSubHead select rdm).FirstOrDefault() == null ? Constants.no : Constants.yes,
                                   DepttId = appDet.departmentId,
                                   PropId = allotMas.propertyId,
                                   isLeaseDeedExecuted = true,
                                   AllotmentDate = allotMas.allotmentDate,
                                   LeaseDeedDate = (from reg in dbContext.RegistryDetails where reg.IsActive == true && reg.Rid == rId select reg.RegistryDoneDate).FirstOrDefault(),//reg.RegistryDoneDate,
                                   PossessionDate = (from pos in dbContext.PossessionDetails where pos.Rid == rId && pos.IsActive == true select pos.PossessionDate).FirstOrDefault(), //Subquery because this function is being used in many places to pick data and its not necessary that possession has been given for all RIDs. //pos.PossessionDate,
                                   PropNo = sector.sectorName + "/" + block.blockName + "-" + spt.propertyNo,
                                   TransferorCorrAdd = appDet.tCorrespondanceAdd,
                                   TransferorPerAdd = appDet.tPermanentAdd
                               }).FirstOrDefault();
                }
                else //Picking up details as this method is being called in other function as well, which don't have the pre-condition of Lease Deed execution 
                {
                    details = (from appDet in dbContext.ApplicationDetails
                               join scMst in dbContext.SchemeMsts on appDet.schemeId equals scMst.schemeId
                               join deMst in dbContext.DepartmentMsts on appDet.departmentId equals deMst.departmentId
                               join allotMas in dbContext.AllotmentMasters on appDet.registrationId equals allotMas.rid
                               join spt in dbContext.SchemePropTrans on allotMas.propertyId equals spt.propertyId
                               join propTypeMas in dbContext.PropertyTypeMsts on spt.propertyTypeId equals propTypeMas.propertyTypeId
                               join floorMst in dbContext.FloorMsts on spt.floorId equals floorMst.floorId
                               join sector in dbContext.SectorMsts on spt.sectorId equals sector.sectorId
                               join block in dbContext.BlockMsts on spt.blockId equals block.blockId
                               //join reg in dbContext.RegistryDetails on appDet.registrationId equals reg.Rid
                               //join pos in dbContext.PossessionDetails on appDet.registrationId equals pos.Rid
                               where appDet.registrationId == rId
                               select new TransferModel
                               {
                                   SchemeName = scMst.schemeName,
                                   DepttName = deMst.departmentName,
                                   ApplicantName = appDet.tFirstName + " " + appDet.tMiddleName + " " + appDet.tLastName,
                                   Gender = appDet.tGender,
                                   RelationName = appDet.tFatherHusbandName,
                                   SigningAuthorityName = appDet.tSigningAuthority,
                                   PropType = propTypeMas.propertyTypeName,
                                   Area = spt.totalArea,
                                   TransfereeCompanyName = (appDet.T_Company_Name != null && appDet.T_Company_Name != "") ? appDet.T_Company_Name : appDet.tFirstName + " " + appDet.tMiddleName + " " + appDet.tLastName,
                                   TransfereeCompanySigningAuth = appDet.tSigningAuthority,
                                   Floor = floorMst.floorName,
                                   PropMortgaged = ((from mort in dbContext.MortgageDetails where (mort.PreviousLoanNoc != MortPrevLoan.invalid || (mort.StatusId == StatusOption.Approved || mort.StatusId == StatusOption.InProgress)) && (mort.RID == rId && mort.IsActive == true) orderby mort.RequestNo descending select mort).FirstOrDefault() == null) ? Constants.no : Constants.yes,
                                   PropFunctional = ((from func in dbContext.FunctionalDetails where func.StatusId == StatusOption.Approved && func.Rid == rId && func.IsActive == true orderby func.RequestNo descending select func).FirstOrDefault() == null) ? Constants.no : Constants.yes,
                                   OneTimeLeaseRent = (from rdm in dbContext.RECEIPT_DETAIL_MASTER join rat in dbContext.RECEIPT_AMOUNT_TRANS on rdm.RECEIPT_ID equals rat.RECEIPT_ID where rdm.RID_NO == rId.ToString() && rat.RECEIPT_HEAD_ID == Constants.OTLRReceiptHead && rat.RECEIPT_SUBHEAD_ID == Constants.OTLRReceiptSubHead select rdm).FirstOrDefault() == null ? Constants.no : Constants.yes,
                                   DepttId = appDet.departmentId,
                                   PropId = allotMas.propertyId,
                                   isLeaseDeedExecuted = false, //For the case when Lease Deed does not exist for the given RID.
                                   AllotmentDate = allotMas.allotmentDate,
                                   LeaseDeedDate = (from reg in dbContext.RegistryDetails where reg.IsActive == true && reg.Rid == rId select reg.RegistryDoneDate).FirstOrDefault(),//reg.RegistryDoneDate,
                                   PossessionDate = (from pos in dbContext.PossessionDetails where pos.Rid == rId && pos.IsActive == true select pos.PossessionDate).FirstOrDefault(), //Subquery because this function is being used in many places to pick data and its not necessary that possession has been given for all RIDs. //pos.PossessionDate
                                   PropNo = sector.sectorName + "/" + block.blockName + "-" + spt.propertyNo,
                                   TransferorCorrAdd = appDet.tCorrespondanceAdd,
                                   TransferorPerAdd = appDet.tPermanentAdd
                               }).FirstOrDefault();
                }
                if (details != null)
                {
                    var tempObj = (from srt in dbContext.SchemeRefundTrans
                                   join appDet in dbContext.ApplicationDetails on srt.schemeId equals appDet.schemeId
                                   where appDet.registrationId == rId && srt.departmentId == appDet.departmentId
                                   select new
                                   {
                                       tempDeductionAmnt = srt.deduction,
                                       tempUnit = srt.unit,
                                       tempDeductionApplyOn = srt.deductionApplyOn,
                                       tempRefundLockPeriod = srt.refundLockPeriod,
                                       tempInterestRate = srt.interest,
                                       tempDaysAfterInterest = srt.daysAfterInterest
                                   }).FirstOrDefault();
                    if (tempObj != null)
                    {
                        details.DeductionAmnt = tempObj.tempDeductionAmnt;
                        details.Unit = tempObj.tempUnit;
                        details.DeductionApplyOn = tempObj.tempDeductionApplyOn;
                        details.RefundLockPeriod = tempObj.tempRefundLockPeriod;
                        details.InterestRate = tempObj.tempInterestRate;
                        details.DaysAfterInterest = tempObj.tempDaysAfterInterest;
                    }

                    //If GPA exists for the given RID, only GPA type Transfer can take place
                    //var GPA = (from gpa in dbContext.GPAs where gpa.Rid == rId && EntityFunctions.TruncateTime(gpa.Effcetd_From) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(gpa.Effected_To) >= EntityFunctions.TruncateTime(DateTime.Now) && gpa.Is_Active == true select gpa).FirstOrDefault();                   
                    var GPA = (from gpa in dbContext.GPAs where gpa.Rid == rId && EntityFunctions.TruncateTime(gpa.Effcetd_From) <= EntityFunctions.TruncateTime(DateTime.Now) && gpa.Is_Active == true select gpa).FirstOrDefault();
                    if (GPA != null)
                    {
                        if (GPA.Effected_To != null)
                        {
                            if (DateTime.Compare(Convert.ToDateTime(GPA.Effected_To), DateTime.Now) >= 0)
                            {
                                details.isGPA = true;
                                details.GPAHolderName = GPA.GPA_Holder_Name;
                                details.GPAHolderAdd = GPA.GPA_Holder_Address;
                                details.GPAEffectiveFrom = GPA.Effcetd_From;
                                details.GPAEffectiveTill = GPA.Effected_To;
                            }
                            else { details.isGPA = false; }
                        }
                        else
                        {
                            details.isGPA = true;
                            details.GPAHolderName = GPA.GPA_Holder_Name;
                            details.GPAHolderAdd = GPA.GPA_Holder_Address;
                            details.GPAEffectiveFrom = GPA.Effcetd_From;
                            details.GPAEffectiveTill = DateTime.Now;
                        }
                    }
                    else
                        details.isGPA = false;

                    //Gets details of Cancellation in case of restoration
                    var tempCancellationDetails = (from rest in dbContext.Property_Cancellation_Details
                                                   where (rest.Type == Constants.intCancellation || rest.Type == Constants.intSurrender) && rest.Rid == rId && rest.Is_Active == true
                                                   orderby rest.Id descending
                                                   select new
                                                   {
                                                       tempReasonOfCancel = rest.Reason,
                                                       //tempCancellationDate = (from can in dbContext.Property_Cancellation_Details where (can.Type == Constants.intCancellation || can.Type == Constants.intSurrender) && can.Rid == rId orderby can.Id descending select can.Approve_Date).FirstOrDefault()
                                                       tempCancellationDate = rest.Approve_Date
                                                   }).FirstOrDefault();
                    if (tempCancellationDetails != null)
                    {
                        details.ReasonOfCancel = tempCancellationDetails.tempReasonOfCancel;
                        details.CancellationDate = tempCancellationDetails.tempCancellationDate;
                    }
                }
                return details;
            }
        }

        /// <summary>
        /// Same as above function used only in the case of Transfer
        /// </summary>
        /// <param name="rId">RID</param>
        /// <returns></returns>
        public TransferModel GetOriginalDetailsForTransferOnlyByRID(int rId, int reqNo)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var details = new TransferModel();
                var leaseDeed = (from reg in dbContext.RegistryDetails where reg.Rid == rId && reg.IsActive == true orderby reg.RequestNo descending select reg).FirstOrDefault();
                if (leaseDeed != null)
                {
                    details = (from appDet in dbContext.ApplicationDetails
                               join trans in dbContext.Succ_Mut_Trans on appDet.registrationId equals trans.Rid
                               join scMst in dbContext.SchemeMsts on appDet.schemeId equals scMst.schemeId
                               join deMst in dbContext.DepartmentMsts on appDet.departmentId equals deMst.departmentId
                               join allotMas in dbContext.AllotmentMasters on appDet.registrationId equals allotMas.rid
                               join spt in dbContext.SchemePropTrans on allotMas.propertyId equals spt.propertyId
                               join propTypeMas in dbContext.PropertyTypeMsts on spt.propertyTypeId equals propTypeMas.propertyTypeId
                               join floorMst in dbContext.FloorMsts on spt.floorId equals floorMst.floorId
                               join sector in dbContext.SectorMsts on spt.sectorId equals sector.sectorId
                               join block in dbContext.BlockMsts on spt.blockId equals block.blockId
                               //join reg in dbContext.RegistryDetails on appDet.registrationId equals reg.Rid
                               //join pos in dbContext.PossessionDetails on appDet.registrationId equals pos.Rid
                               where appDet.registrationId == rId && trans.Request_No == reqNo//&& srt.departmentId == appDet.departmentId
                               select new TransferModel
                               {
                                   SchemeName = scMst.schemeName,
                                   DepttName = deMst.departmentName,
                                   ApplicantName = trans.Applicant_Name,
                                   Gender = trans.Applicant_Gender,
                                   RelationName = trans.ApplicantFather_Name,
                                   SigningAuthorityName = trans.Applicant_Signing_Authority,
                                   PropType = propTypeMas.propertyTypeName,
                                   Area = spt.totalArea,
                                   TransfereeCompanyName = appDet.T_Company_Name,
                                   TransfereeCompanySigningAuth = appDet.tSigningAuthority,
                                   Floor = floorMst.floorName,
                                   PropMortgaged = ((from mort in dbContext.MortgageDetails where (mort.PreviousLoanNoc != MortPrevLoan.invalid || (mort.StatusId == StatusOption.Approved || mort.StatusId == StatusOption.InProgress)) && (mort.RID == rId && mort.IsActive == true) orderby mort.RequestNo descending select mort).FirstOrDefault() == null) ? Constants.no : Constants.yes,
                                   PropFunctional = ((from func in dbContext.FunctionalDetails where func.StatusId == StatusOption.Approved && func.Rid == rId && func.IsActive == true orderby func.RequestNo descending select func).FirstOrDefault() == null) ? Constants.no : Constants.yes,
                                   OneTimeLeaseRent = (from rdm in dbContext.RECEIPT_DETAIL_MASTER join rat in dbContext.RECEIPT_AMOUNT_TRANS on rdm.RECEIPT_ID equals rat.RECEIPT_ID where rdm.RID_NO == rId.ToString() && rat.RECEIPT_HEAD_ID == Constants.OTLRReceiptHead && rat.RECEIPT_SUBHEAD_ID == Constants.OTLRReceiptSubHead select rdm).FirstOrDefault() == null ? Constants.no : Constants.yes,
                                   DepttId = appDet.departmentId,
                                   PropId = allotMas.propertyId,
                                   isLeaseDeedExecuted = true,
                                   AllotmentDate = allotMas.allotmentDate,
                                   LeaseDeedDate = (from reg in dbContext.RegistryDetails where reg.IsActive == true && reg.Rid == rId select reg.RegistryDoneDate).FirstOrDefault(),//reg.RegistryDoneDate,
                                   PossessionDate = (from pos in dbContext.PossessionDetails where pos.Rid == rId && pos.IsActive == true select pos.PossessionDate).FirstOrDefault(), //Subquery because this function is being used in many places to pick data and its not necessary that possession has been given for all RIDs. //pos.PossessionDate,
                                   ApplicantCompany = trans.Applicant_Name,
                                   ApplicantSiAu = trans.Applicant_Signing_Authority,
                                   PropNo = sector.sectorName + "/" + block.blockName + "-" + spt.propertyNo,
                                   TransferorCorrAdd = trans.Correspondance_Add,
                                   TransferorPerAdd = trans.Permanent_Add
                               }).FirstOrDefault();
                }
                else //Picking up details as this method is being called in other function as well, which don't have the pre-condition of Lease Deed execution 
                {
                    details = (from appDet in dbContext.ApplicationDetails
                               join scMst in dbContext.SchemeMsts on appDet.schemeId equals scMst.schemeId
                               join deMst in dbContext.DepartmentMsts on appDet.departmentId equals deMst.departmentId
                               join allotMas in dbContext.AllotmentMasters on appDet.registrationId equals allotMas.rid
                               join spt in dbContext.SchemePropTrans on allotMas.propertyId equals spt.propertyId
                               join propTypeMas in dbContext.PropertyTypeMsts on spt.propertyTypeId equals propTypeMas.propertyTypeId
                               join floorMst in dbContext.FloorMsts on spt.floorId equals floorMst.floorId
                               join sector in dbContext.SectorMsts on spt.sectorId equals sector.sectorId
                               join block in dbContext.BlockMsts on spt.blockId equals block.blockId
                               //join reg in dbContext.RegistryDetails on appDet.registrationId equals reg.Rid
                               //join pos in dbContext.PossessionDetails on appDet.registrationId equals pos.Rid
                               where appDet.registrationId == rId
                               select new TransferModel
                               {
                                   SchemeName = scMst.schemeName,
                                   DepttName = deMst.departmentName,
                                   ApplicantName = appDet.tFirstName + " " + appDet.tMiddleName + " " + appDet.tLastName,
                                   Gender = appDet.tGender,
                                   RelationName = appDet.tFatherHusbandName,
                                   PropType = propTypeMas.propertyTypeName,
                                   Area = spt.totalArea,
                                   Floor = floorMst.floorName,
                                   PropMortgaged = ((from mort in dbContext.MortgageDetails where (mort.PreviousLoanNoc != MortPrevLoan.invalid || (mort.StatusId == StatusOption.Approved || mort.StatusId == StatusOption.InProgress)) && (mort.RID == rId && mort.IsActive == true) orderby mort.RequestNo descending select mort).FirstOrDefault() == null) ? Constants.no : Constants.yes,
                                   PropFunctional = ((from func in dbContext.FunctionalDetails where func.StatusId == StatusOption.Approved && func.Rid == rId && func.IsActive == true orderby func.RequestNo descending select func).FirstOrDefault() == null) ? Constants.no : Constants.yes,
                                   OneTimeLeaseRent = (from rdm in dbContext.RECEIPT_DETAIL_MASTER join rat in dbContext.RECEIPT_AMOUNT_TRANS on rdm.RECEIPT_ID equals rat.RECEIPT_ID where rdm.RID_NO == rId.ToString() && rat.RECEIPT_HEAD_ID == Constants.OTLRReceiptHead && rat.RECEIPT_SUBHEAD_ID == Constants.OTLRReceiptSubHead select rdm).FirstOrDefault() == null ? Constants.no : Constants.yes,
                                   DepttId = appDet.departmentId,
                                   PropId = allotMas.propertyId,
                                   isLeaseDeedExecuted = false, //For the case when Lease Deed does not exist for the given RID.
                                   AllotmentDate = allotMas.allotmentDate,
                                   LeaseDeedDate = (from reg in dbContext.RegistryDetails where reg.IsActive == true && reg.Rid == rId select reg.RegistryDoneDate).FirstOrDefault(),//reg.RegistryDoneDate,
                                   PossessionDate = (from pos in dbContext.PossessionDetails where pos.Rid == rId && pos.IsActive == true select pos.PossessionDate).FirstOrDefault(), //Subquery because this function is being used in many places to pick data and its not necessary that possession has been given for all RIDs. //pos.PossessionDate
                                   TransfereeCompanyName = appDet.T_Company_Name,
                                   TransfereeCompanySigningAuth = appDet.tSigningAuthority,
                                   PropNo = sector.sectorName + "/" + block.blockName + "-" + spt.propertyNo
                               }).FirstOrDefault();
                }
                if (details != null)
                {
                    var tempObj = (from srt in dbContext.SchemeRefundTrans
                                   join appDet in dbContext.ApplicationDetails on srt.schemeId equals appDet.schemeId
                                   where appDet.registrationId == rId && srt.departmentId == appDet.departmentId
                                   select new
                                   {
                                       tempDeductionAmnt = srt.deduction,
                                       tempUnit = srt.unit,
                                       tempDeductionApplyOn = srt.deductionApplyOn,
                                       tempRefundLockPeriod = srt.refundLockPeriod,
                                       tempInterestRate = srt.interest,
                                       tempDaysAfterInterest = srt.daysAfterInterest
                                   }).FirstOrDefault();
                    if (tempObj != null)
                    {
                        details.DeductionAmnt = tempObj.tempDeductionAmnt;
                        details.Unit = tempObj.tempUnit;
                        details.DeductionApplyOn = tempObj.tempDeductionApplyOn;
                        details.RefundLockPeriod = tempObj.tempRefundLockPeriod;
                        details.InterestRate = tempObj.tempInterestRate;
                        details.DaysAfterInterest = tempObj.tempDaysAfterInterest;
                    }

                    //If GPA exists for the given RID, only GPA type Transfer can take place
                    //var GPA = (from gpa in dbContext.GPAs where gpa.Rid == rId && EntityFunctions.TruncateTime(gpa.Effcetd_From) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(gpa.Effected_To) >= EntityFunctions.TruncateTime(DateTime.Now) && gpa.Is_Active == true select gpa).FirstOrDefault();
                    var GPA = (from gpa in dbContext.GPAs where gpa.Rid == rId && EntityFunctions.TruncateTime(gpa.Effcetd_From) <= EntityFunctions.TruncateTime(DateTime.Now) && gpa.Is_Active == true select gpa).FirstOrDefault();
                    if (GPA != null)
                    {
                        if (GPA.Effected_To != null)
                        {
                            //if (EntityFunctions.TruncateTime(GPA.Effected_To) >= EntityFunctions.TruncateTime(DateTime.Now))
                            if (DateTime.Compare(Convert.ToDateTime(GPA.Effected_To), DateTime.Now) >= 0)
                            {
                                details.isGPA = true;
                                details.GPAHolderName = GPA.GPA_Holder_Name;
                                details.GPAHolderAdd = GPA.GPA_Holder_Address;
                                details.GPAEffectiveFrom = GPA.Effcetd_From;
                                details.GPAEffectiveTill = GPA.Effected_To;
                            }
                            else { details.isGPA = false; }
                        }
                        else
                        {
                            details.isGPA = true;
                            details.GPAHolderName = GPA.GPA_Holder_Name;
                            details.GPAHolderAdd = GPA.GPA_Holder_Address;
                            details.GPAEffectiveFrom = GPA.Effcetd_From;
                            details.GPAEffectiveTill = DateTime.Now;
                        }
                    }
                    else
                        details.isGPA = false;

                    //Gets details of Cancellation in case of restoration
                    var tempCancellationDetails = (from rest in dbContext.Property_Cancellation_Details
                                                   where (rest.Type == Constants.intCancellation || rest.Type == Constants.intSurrender) && rest.Rid == rId && rest.Is_Active == true
                                                   orderby rest.Id descending
                                                   select new
                                                   {
                                                       tempReasonOfCancel = rest.Reason,
                                                       //tempCancellationDate = (from can in dbContext.Property_Cancellation_Details where (can.Type == Constants.intCancellation || can.Type == Constants.intSurrender) && can.Rid == rId orderby can.Id descending select can.Approve_Date).FirstOrDefault()
                                                       tempCancellationDate = rest.Approve_Date
                                                   }).FirstOrDefault();
                    if (tempCancellationDetails != null)
                    {
                        details.ReasonOfCancel = tempCancellationDetails.tempReasonOfCancel;
                        details.CancellationDate = tempCancellationDetails.tempCancellationDate;
                    }
                }
                return details;
            }
        }

        /// <summary>
        /// Will get Transfer Types from DB
        /// </summary>
        /// <returns></returns>
        public List<DDList> GetTransferTypes()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var lst = new List<DDList>();
                //var obj1 = new DDList();
                //obj1.id = 1;
                //obj1.text = "Normal";
                //lst.Add(obj1);
                //var obj2 = new DDList();
                //obj2.id = 2;
                //obj2.text = "Death Case";
                //lst.Add(obj2);
                //return lst;
                var lst = (from transTy in dbContext.Transfer_Type
                           where transTy.Parent_Id == null && transTy.Is_Active == true
                           select new DDList
                           {
                               text = transTy.type,
                               id = transTy.Id
                           }).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Will get Sub-transfer Types from DB on the basis of Transfer Type
        /// </summary>
        /// <param name="TransferType">Transfer Type ID</param>
        /// <returns></returns>
        public List<DDList> GetTransferSubTypes(int TransferType)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var lst = new List<DDList>();
                //var obj1 = new DDList();
                //obj1.id = 1;
                //obj1.text = "Normal";
                //lst.Add(obj1);
                //var obj2 = new DDList();
                //obj2.id = 2;
                //obj2.text = "Death Case";
                //lst.Add(obj2);
                //return lst;
                var lst = (from transTy in dbContext.Transfer_Type
                           where transTy.Parent_Id == TransferType && transTy.Is_Active == true
                           select new DDList
                           {
                               text = transTy.type,
                               id = transTy.Id
                           }).ToList();
                return lst;
            }
        }

        //Saves Transfer related details in DB
        public bool SaveTransferData(int rId, int propId, string applicantName, string applicantGender, string applicantRelativeName, string applicantPropType, decimal Area, string floor, int userVal, int transType, int transSubType, DateTime transDate, decimal transChargePerSqMrt, decimal totTransCharge, string transfereeGender, string transfereeCompanyName, string transfereeFrstName, string trasfereeSignAuth, string transfereeMiddleName, string transfereeLstName, string transfereeCompanyRegOfc, string TransfereeRelName, string TransfereeMotherName, string TransfereeMobile, string TransfereeEmail, string TransfereeCorrAdd, string TransfereePerAdd, string TransfereePAN, int? TransfereeOccupation, int? reqNo, string compName, string signAuth, int? OnlineReqRefNo, string transferorCorrAdd, string transferorPerAdd, decimal? CurrentPropertyRate, decimal? LocationCharge, decimal? TotalPropertyCost, decimal? AnnualLeaseRent)
        {
            var flag = false;
            if (transfereeGender == "")
            {
                transfereeGender = Constants.genderCompany;
            }
            using (var dbContext = new NoidaPMSEntities())
            {
                if (reqNo != 0 && reqNo != null) //In case of Update (when User Resubmits Transfer Request after Rejection)
                {
                    var existingRecord = (from trans in dbContext.Succ_Mut_Trans where trans.Request_No == reqNo select trans).FirstOrDefault();
                    if (existingRecord != null)
                    {
                        existingRecord.Approved_By = userVal;
                        existingRecord.Transfer_Type = transType;
                        existingRecord.Transfer_Sub_Type = transSubType;
                        existingRecord.Transfer_Date = transDate;
                        existingRecord.Transfer_Charge = transChargePerSqMrt;
                        existingRecord.Total_Transfer_Charge = totTransCharge;
                        existingRecord.Correspondance_Add = transferorCorrAdd;
                        existingRecord.Permanent_Add = transferorPerAdd;
                        //var GPA = (from gpa in dbContext.GPAs where gpa.Rid == rId && EntityFunctions.TruncateTime(gpa.Effcetd_From) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(gpa.Effected_To) >= EntityFunctions.TruncateTime(DateTime.Now) && gpa.Is_Active == true select gpa).FirstOrDefault();
                        var GPA = (from gpa in dbContext.GPAs where gpa.Rid == rId && EntityFunctions.TruncateTime(gpa.Effcetd_From) <= EntityFunctions.TruncateTime(DateTime.Now) && gpa.Is_Active == true select gpa).FirstOrDefault();
                        if (GPA != null)
                        {
                            if (GPA.Effected_To != null)
                            {//EntityFunctions.TruncateTime(GPA.Effected_To) >= EntityFunctions.TruncateTime(DateTime.Now)
                                if (DateTime.Compare(Convert.ToDateTime(GPA.Effected_To), DateTime.Now) >= 0)
                                {

                                    existingRecord.GPA_Holder_Name = GPA.GPA_Holder_Name;
                                    existingRecord.GPA_Holder_Address = GPA.GPA_Holder_Address;
                                    existingRecord.GPA_Effective_From = GPA.Effcetd_From;
                                    existingRecord.GPA_Effective_To = GPA.Effected_To;
                                }
                            }
                            else
                            {
                                existingRecord.GPA_Holder_Name = GPA.GPA_Holder_Name;
                                existingRecord.GPA_Holder_Address = GPA.GPA_Holder_Address;
                                existingRecord.GPA_Effective_From = GPA.Effcetd_From;
                                existingRecord.GPA_Effective_To = DateTime.Now;
                            }
                        }
                        existingRecord.T_Gender = transfereeGender;
                        if (transfereeGender == Constants.genderCompany)
                        {
                            existingRecord.T_Company_Name = transfereeCompanyName;
                            existingRecord.T_Signing_Authority = trasfereeSignAuth;
                            existingRecord.T_Registered_Office = transfereeCompanyRegOfc;
                            //Make Male/Female related fields null
                            existingRecord.T_First_Name = null;
                            existingRecord.T_Middle_Name = null;
                            existingRecord.T_Last_Name = null;
                            existingRecord.T_Father_Husband_Name = null;
                            existingRecord.T_Mother_Name = null;
                            existingRecord.T_Occupation_Id = null;
                        }
                        else
                        {
                            existingRecord.T_First_Name = transfereeFrstName;
                            existingRecord.T_Middle_Name = transfereeMiddleName;
                            existingRecord.T_Last_Name = transfereeLstName;
                            existingRecord.T_Father_Husband_Name = TransfereeRelName;
                            existingRecord.T_Mother_Name = TransfereeMotherName;
                            existingRecord.T_Occupation_Id = TransfereeOccupation;
                            //Make Company related details null
                            existingRecord.T_Company_Name = null;
                            existingRecord.T_Signing_Authority = null;
                            existingRecord.T_Registered_Office = null;
                        }
                        existingRecord.T_Email = TransfereeEmail;
                        existingRecord.T_Mobile = TransfereeMobile;
                        existingRecord.T_Correspondence_Add = TransfereeCorrAdd;
                        existingRecord.T_Permanent_Add = TransfereePerAdd;
                        existingRecord.T_Pan = TransfereePAN;
                        existingRecord.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.InProgress.ToString().ToLower() select st.Id).FirstOrDefault();
                        existingRecord.OnlineRequestNo = OnlineReqRefNo;
                        existingRecord.CurrentPropertyRate = CurrentPropertyRate;
                        existingRecord.LocationCharge = LocationCharge;
                        existingRecord.AnnualLeaseRent = AnnualLeaseRent;
                        existingRecord.TotalPropertyCost = TotalPropertyCost;
                        dbContext.SaveChanges();
                        flag = true;
                    }
                }
                else //Add new Transfer Requests
                {
                    var newtransfer = new Succ_Mut_Trans();
                    newtransfer.Rid = rId;
                    newtransfer.Property_Id = propId;
                    if (applicantGender.ToLower() == Constants.genderCompany.ToLower())
                    {
                        newtransfer.Applicant_Name = compName;
                    }
                    else
                        newtransfer.Applicant_Name = applicantName;
                    newtransfer.Applicant_Signing_Authority = signAuth;
                    newtransfer.ApplicantFather_Name = applicantRelativeName;
                    newtransfer.Transfer_Date = transDate;
                    newtransfer.Applicant_Gender = applicantGender;
                    newtransfer.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.InProgress.ToString().ToLower() select st.Id).FirstOrDefault();
                    newtransfer.Requested_By = userInfo.UserID;
                    newtransfer.Requested_Date = DateTime.Now;
                    newtransfer.Type = TransferType.T.ToString();
                    newtransfer.Transfer_Type = transType;
                    newtransfer.Transfer_Sub_Type = transSubType;
                    newtransfer.Correspondance_Add = transferorCorrAdd;
                    newtransfer.Permanent_Add = transferorPerAdd;
                    newtransfer.Is_Active = true;
                    newtransfer.Created_By = userInfo.UserID;
                    newtransfer.Created_Date = DateTime.Now;
                    //var GPA = (from gpa in dbContext.GPAs where gpa.Rid == rId && EntityFunctions.TruncateTime(gpa.Effcetd_From) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(gpa.Effected_To) >= EntityFunctions.TruncateTime(DateTime.Now) && gpa.Is_Active == true select gpa).FirstOrDefault();
                    var GPA = (from gpa in dbContext.GPAs where gpa.Rid == rId && EntityFunctions.TruncateTime(gpa.Effcetd_From) <= EntityFunctions.TruncateTime(DateTime.Now) && gpa.Is_Active == true select gpa).FirstOrDefault();
                    if (GPA != null)
                    {
                        //newtransfer.GPA_Holder_Name = GPA.GPA_Holder_Name;
                        //newtransfer.GPA_Holder_Address = GPA.GPA_Holder_Address;
                        //newtransfer.GPA_Effective_From = GPA.Effcetd_From;
                        //newtransfer.GPA_Effective_To = GPA.Effected_To;
                        if (GPA.Effected_To != null)
                        {//EntityFunctions.TruncateTime(GPA.Effected_To) >= EntityFunctions.TruncateTime(DateTime.Now)
                            if (DateTime.Compare(Convert.ToDateTime(GPA.Effected_To), DateTime.Now) >= 0)
                            {

                                newtransfer.GPA_Holder_Name = GPA.GPA_Holder_Name;
                                newtransfer.GPA_Holder_Address = GPA.GPA_Holder_Address;
                                newtransfer.GPA_Effective_From = GPA.Effcetd_From;
                                newtransfer.GPA_Effective_To = GPA.Effected_To;
                            }
                        }
                        else
                        {
                            newtransfer.GPA_Holder_Name = GPA.GPA_Holder_Name;
                            newtransfer.GPA_Holder_Address = GPA.GPA_Holder_Address;
                            newtransfer.GPA_Effective_From = GPA.Effcetd_From;
                            newtransfer.GPA_Effective_To = DateTime.Now;
                        }
                    }
                    newtransfer.T_First_Name = transfereeFrstName;
                    newtransfer.T_Gender = transfereeGender;
                    newtransfer.T_Middle_Name = transfereeMiddleName;
                    newtransfer.T_Last_Name = transfereeLstName;
                    newtransfer.T_Father_Husband_Name = TransfereeRelName;
                    newtransfer.T_Mother_Name = TransfereeMotherName;
                    newtransfer.T_Company_Name = transfereeCompanyName;
                    newtransfer.T_Signing_Authority = trasfereeSignAuth;
                    newtransfer.T_Email = TransfereeEmail;
                    newtransfer.T_Mobile = TransfereeMobile;
                    newtransfer.T_Correspondence_Add = TransfereeCorrAdd;
                    newtransfer.T_Permanent_Add = TransfereePerAdd;
                    newtransfer.T_Registered_Office = transfereeCompanyRegOfc;
                    newtransfer.T_Pan = TransfereePAN;
                    newtransfer.T_Occupation_Id = TransfereeOccupation;
                    newtransfer.Approved_By = userVal;
                    newtransfer.Transfer_Charge = transChargePerSqMrt;
                    newtransfer.Total_Transfer_Charge = totTransCharge;
                    newtransfer.OnlineRequestNo = OnlineReqRefNo;
                    newtransfer.CurrentPropertyRate = CurrentPropertyRate;
                    newtransfer.LocationCharge = LocationCharge;
                    newtransfer.AnnualLeaseRent = AnnualLeaseRent;
                    newtransfer.TotalPropertyCost = TotalPropertyCost;
                    dbContext.Succ_Mut_Trans.Add(newtransfer);
                    dbContext.SaveChanges();
                    flag = true;


                }
            }
            return flag;
        }

        public bool SaveTransferApprovalStatus(string comments, int intStatus, int ReqNo)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingTransReq = (from trans in dbContext.Succ_Mut_Trans where trans.Request_No == ReqNo && trans.Is_Active == true select trans).FirstOrDefault();
                if (existingTransReq != null)
                {
                    if (intStatus == 1)//For Status Approved
                    {
                        //Approve Transfer
                        existingTransReq.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Approved.ToString().ToLower() select st.Id).FirstOrDefault();
                        existingTransReq.Modified_By = userInfo.UserID;
                        existingTransReq.Modified_Date = DateTime.Now;
                        existingTransReq.Approved_Date = DateTime.Now;
                        existingTransReq.Comment = comments;
                        //Make any GPA or Nominee Inactive
                        var existingGPA = (from gpa in dbContext.GPAs where gpa.Is_Active == true && gpa.Rid == existingTransReq.Rid select gpa).FirstOrDefault();
                        if (existingGPA != null)
                        {
                            existingGPA.Is_Active = false;
                            existingGPA.Modified_By = userInfo.UserID;
                            existingGPA.Modified_date = DateTime.Now;
                        }
                        var lstNominees = (from nom in dbContext.Nominee_Details where nom.Is_Active == 1 && nom.Rid == existingTransReq.Rid select nom).ToList();
                        if (lstNominees.Any())
                        {
                            foreach (var item in lstNominees)
                            {
                                item.Is_Active = 0;
                                item.Modified_By = userInfo.UserID;
                                item.Modified_Date = DateTime.Now;
                            }
                        }
                        //Make Property Functional IsActive = false, if any
                        var functional = (from func in dbContext.FunctionalDetails where func.Rid == existingTransReq.Rid && func.IsActive == true orderby func.RequestNo descending select func).FirstOrDefault();
                        if (functional != null)
                        {
                            functional.IsActive = false;
                            functional.Modifiedby = userInfo.UserID;
                            functional.ModifiedDate = DateTime.Now;
                        }
                        //Copy new owner details from Succ_Mut_Trans table to ApplicationDetails table
                        var appDet = (from ad in dbContext.ApplicationDetails where ad.registrationId == existingTransReq.Rid select ad).FirstOrDefault();
                        if (appDet != null)
                        {
                            appDet.tGender = existingTransReq.T_Gender;
                            if (existingTransReq.T_Gender.ToLower() == Constants.genderCompany.ToLower())
                                appDet.tFirstName = existingTransReq.T_Company_Name;
                            else
                                appDet.tFirstName = existingTransReq.T_First_Name;
                            appDet.tMiddleName = existingTransReq.T_Middle_Name;
                            appDet.tLastName = existingTransReq.T_Last_Name;
                            appDet.tFatherHusbandName = existingTransReq.T_Father_Husband_Name;
                            appDet.tMotherName = existingTransReq.T_Mother_Name;
                            appDet.T_Company_Name = existingTransReq.T_Company_Name;
                            appDet.tSigningAuthority = existingTransReq.T_Signing_Authority;
                            appDet.tEmail = existingTransReq.T_Email;
                            appDet.tMobileNumber = existingTransReq.T_Mobile;
                            appDet.tCorrespondanceAdd = existingTransReq.T_Correspondence_Add;
                            appDet.tPermanentAdd = existingTransReq.T_Permanent_Add;
                            appDet.tRegisteredOffice = existingTransReq.T_Registered_Office;
                            appDet.tOccupationId = existingTransReq.T_Occupation_Id;
                            appDet.tPan = existingTransReq.T_Pan;
                            appDet.tAnnualIncome = existingTransReq.T_Annual_Income;
                            appDet.modifiedBy = userInfo.UserID.ToString();
                            appDet.modifiedDate = DateTime.Now;
                        }
                        var kya = dbContext.KYADetails.FirstOrDefault(m => m.RId == existingTransReq.Rid && m.IsActive == true);
                        if (kya != null)
                        {
                            kya.IsActive = false;
                            kya.StatusId = NAStatusId.CancelAfterTransfer;
                            kya.ModifiedBy = userInfo.UserID;
                            kya.ModifiedDate = DateTime.Now;
                        }
                        dbContext.SaveChanges();

                        if (appDet != null)
                        {
                            var exDetail = dbContext.KYADetails.Where(c => c.RId == existingTransReq.Rid && c.StatusId == NAStatusId.CancelAfterTransfer).OrderByDescending(o => o.Id).FirstOrDefault();
                            if (exDetail != null)
                            {
                                var detail = new KYADetail();
                                detail.RId = appDet.registrationId;
                                detail.DepartmentId = appDet.departmentId;
                                detail.AllotteeName = appDet.tFirstName + " " + appDet.tMiddleName + " " + appDet.tLastName;
                                detail.AllotteeType = appDet.tGender;
                                if (kya != null)
                                {
                                    detail.SectorId = kya.SectorId;
                                    detail.BlockId = kya.BlockId;
                                    detail.PlotNo = kya.PlotNo;
                                }
                                detail.FatherOrHusbandName = appDet.tFatherHusbandName;
                                detail.AuthorizedSignatory = appDet.tSigningAuthority;
                                //if (appDet.tGender != "Company")
                                //{
                                //    detail.MobileNo = appDet.tMobileNumber;
                                //    detail.Email = appDet.tEmail;
                                //}
                                //else
                                //{
                                //    detail.SignatoryMobileNo = appDet.tMobileNumber;
                                //    detail.SignatoryEmail = appDet.tEmail;
                                //}
                                detail.MobileNo = appDet.tMobileNumber;
                                detail.Email = appDet.tEmail;
                                if (appDet.tGender == "Company")
                                {
                                    detail.SignatoryMobileNo = appDet.tMobileNumber;
                                    detail.SignatoryEmail = appDet.tEmail;
                                }
                                detail.PhoneNo = appDet.tPhoneNumber;
                                detail.PAN = appDet.tPan;

                                detail.CorrespondAddress = appDet.tPermanentAdd;
                                detail.AddressLine1 = existingTransReq.T_Correspondence_Add;
                                //detail.GSTNo = existingTransReq
                                //detail.AadharNo = appDet
                                //detail.ROC = model.ROC;
                                //detail.AddressLine2 = model.AddressLine2;
                                //detail.AreaLocality = model.AreaLocality;
                                //detail.City = model.City;
                                //detail.State = model.State;
                                //detail.PinCode = model.PinCode;
                                var userdata = dbContext.CustomerMsts.FirstOrDefault(m => m.UserName == existingTransReq.Rid.ToString());
                                if (userdata != null)
                                {
                                    detail.IdFileType = userdata.IdFileType;
                                    detail.PlotOwnershipFileType = userdata.PropertyFileType;
                                }
                                detail.SubmitDate = DateTime.Now;
                                detail.ApprovalDate = DateTime.Now;
                                detail.CreatedDate = DateTime.Now;
                                detail.ApproverId = userInfo.UserID;
                                detail.StatusId = NAStatusId.Approved;
                                detail.IsActive = true;
                                detail.KYAReferenceCode = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + appDet.registrationId.ToString();
                                detail.TransferId = existingTransReq.Request_No;
                                dbContext.KYADetails.Add(detail);
                                dbContext.SaveChanges();

                                detail.KYAuid = detail.RId + "-" + detail.Id;
                                dbContext.SaveChanges();
                                if (userdata != null)
                                {
                                    userdata.FirstName = detail.AllotteeName;
                                    userdata.ModifiedBy = userInfo.UserID.ToString();
                                    userdata.ModifiedDate = DateTime.Now;
                                    userdata.IsActive = true;
                                    userdata.IsFirstTimeActivated = true;
                                    userdata.StatusId = 1;
                                    if (detail.MobileNo != null)
                                    {
                                        userdata.MobileNo = detail.MobileNo;
                                    }
                                    else
                                    {
                                        userdata.MobileNo = detail.SignatoryMobileNo;
                                    }
                                    if (detail.Email != null)
                                    {
                                        userdata.Email = detail.Email;
                                    }
                                    else
                                    {
                                        userdata.Email = detail.SignatoryEmail;
                                    }
                                    var newPassword = ApplicationHelper.CreatePassword();
                                    userdata.Password = newPassword.ToMD5HashForPassword();
                                    dbContext.SaveChanges();

                                    var msg = string.Empty;
                                    if (userdata.MobileNo != null)
                                    {
                                        msg = string.Format(NAMessages.PIS_Registration_Activation, userdata.UserName, newPassword);
                                        //if (!string.IsNullOrEmpty(userdata.MobileNo)) { ApplicationHelper.SendSMS(userdata.MobileNo, msg); }
                                        if (!string.IsNullOrEmpty(kya.MobileNo)) { appl.SaveAndSendSMS(detail.RId, userdata.MobileNo, msg, "Registration", "Registration", userInfo.UserID.ToString()); }
                                    }
                                }
                            }
                        }

                        if (existingTransReq.OnlineRequestNo != 0)
                        {
                            UpdateServiceRequest(existingTransReq.OnlineRequestNo, existingTransReq.Rid, appDet.departmentId, comments);
                        }

                        flag = true;
                    }
                    else //For Status Rejected
                    {
                        existingTransReq.Status = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Rejected.ToString().ToLower() select st.Id).FirstOrDefault();
                        existingTransReq.Modified_By = userInfo.UserID;
                        existingTransReq.Modified_Date = DateTime.Now;
                        existingTransReq.Approved_Date = DateTime.Now;
                        existingTransReq.Comment = comments;
                        dbContext.SaveChanges();
                        flag = true;
                    }
                }
            }
            return flag;
        }

        public int UpdateServiceRequest(int? requestId, int? rId, int? departmentId, string comments)
        {
            var flag = ReturnType.None;
            List<int?> statuslist = new List<int?> { 10, 2, 3, 9, 8 };
            using (var dbContext = new NoidaPMSEntities())
            {
                var service = dbContext.Customer_ServiceRequest.FirstOrDefault(m => m.Id == requestId && m.Registration_No == rId.ToString() && m.DepartmentId == departmentId);
                var status = dbContext.Customer_ServiceStatusTrans.Where(c => c.RequestRefId == requestId).OrderByDescending(c => c.Id).FirstOrDefault();
                if (service != null)
                {
                    if (!statuslist.Contains(service.Request_Status))
                    {
                        service.Request_Status = NAStatusId.Completed;
                        service.ApprovalDate = DateTime.Now;
                        service.ApproverId = userInfo.UserID;
                        service.Comment = service.Comment + "\n" + comments + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        dbContext.SaveChanges();
                        if (status != null)
                        {
                            status.StatusId = NAStatusId.Completed;
                            status.ModifiedBy = userInfo.UserID;
                            status.ModifiedDate = DateTime.Now;
                            status.Remarks = status.Remarks + "\n" + comments + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                            dbContext.SaveChanges();
                        }
                        flag = ReturnType.Updated;

                        string reqName = dbContext.StatusMasters.Where(m => m.Id == service.Request_Status).FirstOrDefault().Status;
                        var appDetails = dbContext.ApplicationDetails.Where(m => m.registrationId == rId).FirstOrDefault();
                        string message = string.Empty;
                        message = string.Format(NAMessages.SDServiceReqStatusChange, requestId, reqName);
                        if (!string.IsNullOrEmpty(appDetails.tMobileNumber)) { ApplicationHelper.SendSMS(appDetails.tMobileNumber, message); }
                        if (!string.IsNullOrEmpty(appDetails.tEmail)) { ApplicationHelper.SendEmail(appDetails.tEmail, "Online Request", message); }
                    }
                }
                return flag;
            }
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

        /// <summary>
        /// Used for getting the difference between two dates
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>No. of years</returns>
        //private int CalculateAge(DateTime startDate, DateTime endDate)
        //{
        //    return (endDate.Year - startDate.Year - 1) +
        //(((endDate.Month > startDate.Month) ||
        //((endDate.Month == startDate.Month) && (endDate.Day >= startDate.Day))) ? 1 : 0);
        //}

        public DataSourceResult GetPropFunctional(DataSourceRequest Req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var objFunctional = (from funDetails in dbContext.FunctionalDetails
                                     join alot in dbContext.AllotmentMasters on funDetails.Rid equals alot.rid
                                     join prop in dbContext.SchemePropTrans on alot.propertyId equals prop.propertyId
                                     join statusMas in dbContext.StatusMasters on funDetails.StatusId equals statusMas.Id
                                     join am in dbContext.AllotmentMasters on funDetails.Rid equals am.rid
                                     where funDetails.IsActive == true && am.isActive == 1 && loginUserDeptt.Contains(am.departmentId)
                                     select new FunctionalModel
                                     {
                                         RequestNo = funDetails.RequestNo,
                                         RId = funDetails.Rid,
                                         ApproveDate = funDetails.ApproveDate,
                                         CreatedDate = funDetails.CreatedDate,
                                         PropertyNumber = funDetails.PropertyNumber,
                                         SectorName = prop.SectorMst.sectorName,
                                         BlockName = prop.BlockMst.blockName,
                                         PropertyNo = prop.propertyNo,
                                         Status = statusMas.Status
                                     });


                return objFunctional.ToDataSourceResult(Req);
            }

        }

        public DataSourceResult GetRIDsForFunctional(DataSourceRequest Req, int Rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {

                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      && (dept.DepartmentId == (int)Departmentenum.Commercial || dept.DepartmentId == (int)Departmentenum.Industrial || dept.DepartmentId == (int)Departmentenum.Institutional)
                                      //&& Dept.Contains (dept.DepartmentId )
                                      select dept.DepartmentId).ToList();

                var objRid = (from comRid in dbContext.PossessionDetails //from comRid in dbContext.Completion_Details //changes on 17 Feb 2017
                              join am in dbContext.AllotmentMasters on comRid.Rid equals am.rid
                              where comRid.IsActive == true && am.isActive == 1 && loginUserDeptt.Contains(am.departmentId)
                              //where comRid.Is_Active == true && am.isActive == 1 && loginUserDeptt.Contains(am.departmentId)
                              //&& (am.departmentId == (int)Departmentenum.Commercial || am.departmentId == (int)Departmentenum.Industrial || am.departmentId == (int)Departmentenum.Institutional )
                              //orderby comRid.Created_Date descending
                              select new DDList
                          {
                              id = comRid.Rid.Value,
                              text = comRid.Rid.ToString()
                          });
                if (Rid > 0) { objRid = objRid.Where(m => m.id == Rid); }
                return objRid.ToDataSourceResult(Req);
            }
        }
        public int ReSubmitFunctionalDetails(int rId, DateTime? FunctionalDate, DateTime? FunctionalDueDate, bool MeterSealing, bool Affidavit, bool RegistrationCertificate, bool NDCAccount, string user, string PropertyNumber, decimal? FunctionalCharge)
        {
            int flag = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                // var objUserList = dbContext.UmUserMasters.Where(i => i.UserName == user && i.IsActive == true).FirstOrDefault();

                var existingRequest = (from mut in dbContext.FunctionalDetails where mut.Rid == rId && mut.IsActive == true select mut).FirstOrDefault();
                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == existingRequest.Rid select appDet).FirstOrDefault();//For notification details

                existingRequest.FunctionalDate = FunctionalDate;
                existingRequest.FunctionalDueDate = FunctionalDueDate;
                existingRequest.AffidavitFlag = Affidavit;
                existingRequest.MeterSeallingDocFlag = MeterSealing;
                existingRequest.NOCAccountFlag = NDCAccount;
                existingRequest.RegistrationCertiFlag = RegistrationCertificate;
                existingRequest.StatusId = (from mut in dbContext.StatusMasters where mut.Id == 5 select mut.Id).FirstOrDefault();
                existingRequest.Approver = (!string.IsNullOrEmpty(user)) ? Convert.ToInt32(user) : 0;
                existingRequest.ModifiedDate = DateTime.Now;
                existingRequest.Modifiedby = userInfo.UserID;
                existingRequest.FunctionalCharge = FunctionalCharge;
                dbContext.SaveChanges();

                flag = 1;
                var body = "Hi,<br><br>Your Functional Request has been re-submitted.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                EmailHelper emailHelper = new EmailHelper();
                emailHelper.Send(newOwner.tEmail, "Functional Request Status", body);
                //SMS
                var msg = NAMessages.FunctionalReqResubmit;
                //SMSSend(newOwner.tMobileNumber, msg);
                ApplicationHelper.SendSMS(newOwner.tMobileNumber, msg);
                return flag;
            }
        }
        public int SaveFunctionalDetails(int rId, DateTime? FunctionalDate, DateTime? FunctionalDueDate, bool MeterSealing, bool Affidavit, bool RegistrationCertificate, bool NDCAccount, string user, string PropertyNumber, decimal? FunctionalCharge, int ReqRefNo, DateTime? completionDate, DateTime? affidavitDate)
        {
            int flag = 0;
            using (var dbContext = new NoidaPMSEntities())
            {

                var existingRequest = (from mut in dbContext.FunctionalDetails where mut.Rid == rId && mut.IsActive == true select mut).ToList();
                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == rId select appDet).FirstOrDefault();//For notification details

                existingRequest.ForEach(m => m.IsActive = false);
                //var objUserList = dbContext.UmUserMasters.Where(i => i.UserName == user).FirstOrDefault();
                //if (existingRequest == null)
                //{
                FunctionalDetail objfun = new FunctionalDetail();
                objfun.Rid = rId;
                objfun.FunctionalDate = FunctionalDate;
                objfun.FunctionalDueDate = FunctionalDueDate;
                objfun.AffidavitFlag = Affidavit;
                objfun.MeterSeallingDocFlag = MeterSealing;
                objfun.NOCAccountFlag = NDCAccount;
                objfun.RegistrationCertiFlag = RegistrationCertificate;
                objfun.StatusId = (from mut in dbContext.StatusMasters where mut.Id == 5 select mut.Id).FirstOrDefault();
                objfun.Approver = (!string.IsNullOrEmpty(user)) ? Convert.ToInt32(user) : 0;
                objfun.CreatedDate = DateTime.Now;
                objfun.CreatedBy = userInfo.UserID;
                objfun.PropertyNumber = PropertyNumber;
                objfun.FunctionalCharge = FunctionalCharge;
                objfun.IsActive = true;
                objfun.Functional = false;
                objfun.OnlineRequestNo = ReqRefNo;

                objfun.ComplitionDate = completionDate;
                objfun.AffidavitDate = affidavitDate;

                dbContext.FunctionalDetails.Add(objfun);
                dbContext.SaveChanges();

                flag = 1;
                var body = "Hi,<br><br>Your Functional Request has been saved.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                EmailHelper emailHelper = new EmailHelper();
                emailHelper.Send(newOwner.tEmail, "Functional Request Status", body);
                //SMS
                var msg = NAMessages.FunctionalReqSave;
                //SMSSend(newOwner.tMobileNumber, msg);
                ApplicationHelper.SendSMS(newOwner.tMobileNumber, msg);
                //}
                //else
                //{
                //    flag = 2;
                //    //  existingRequest.Rid = rId;

                //}
                return flag;
            }

        }
        public DateTime? GetFunctionalDueDate(int rid)
        {
            return DateTime.Now;
        }
        public FunctionalModel GetFunctionalDetailsByReqNo(int reqNo)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from mut in dbContext.FunctionalDetails
                            join am in dbContext.AllotmentMasters on mut.Rid equals am.rid
                            join appDet in dbContext.ApplicationDetails on am.rid equals appDet.registrationId
                            join spt in dbContext.SchemePropTrans on am.propertyId equals spt.propertyId
                            join scm in dbContext.SchemeMsts on spt.schemeId equals scm.schemeId
                            join dm in dbContext.DepartmentMsts on am.departmentId equals dm.departmentId
                            join pm in dbContext.PropertyTypeMsts on spt.propertyTypeId equals pm.propertyTypeId
                            join sm in dbContext.SectorMsts on spt.sectorId equals sm.sectorId
                            join bm in dbContext.BlockMsts on spt.blockId equals bm.blockId
                            join fm in dbContext.FloorMsts on spt.floorId equals fm.floorId
                            join sta in dbContext.StatusMasters on mut.StatusId equals sta.Id
                            where mut.RequestNo == reqNo
                            select new FunctionalModel
                            {
                                RId = mut.Rid,
                                PropertyNumber = mut.PropertyNumber,
                                SchemeName = scm.schemeName,
                                DepttName = dm.departmentName,
                                ApplicationName = appDet.tFirstName + " " + appDet.tMiddleName + " " + appDet.tLastName,
                                Gender = appDet.tGender,
                                RelationName = appDet.tFatherHusbandName,
                                PropertyType = pm.propertyTypeName,
                                Area = spt.totalArea,
                                Floor = fm.floorName,
                                Status = sta.Status,
                                FunctionalDueDate = mut.FunctionalDueDate,
                                Affidavit = mut.AffidavitFlag.Value,
                                MeterSealing = mut.MeterSeallingDocFlag.Value,
                                NDCAccount = mut.NOCAccountFlag.Value,
                                RegistrationCertificate = mut.RegistrationCertiFlag.Value,
                                FunctionalDate = mut.FunctionalDate,
                                Comment = mut.Comment,
                                FunctionalCharge = mut.FunctionalCharge,
                                From = dbContext.UmUserMasters.Where(x => x.UserRefId == mut.Approver).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault(),
                                RequestNo = mut.RequestNo,
                                ApproveDate = mut.ApproveDate,
                                AffidavitDate = mut.ApproveDate,
                                CompletionDate = mut.ComplitionDate,
                                ReqRefNo = mut.OnlineRequestNo != null ? (int)mut.OnlineRequestNo : 0
                            }).FirstOrDefault();

                return data;
            }
        }
        public bool CancelFunctionRequest(int reqNo)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingRequest = (from mut in dbContext.FunctionalDetails where mut.RequestNo == reqNo select mut).FirstOrDefault();
                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == existingRequest.Rid select appDet).FirstOrDefault();//For notification details
                FunctionalDetail oldObjModel = new FunctionalDetail();//Old Model
                //oldObjModel.Comment  = existingRequest.Comment;
                oldObjModel.StatusId = existingRequest.StatusId;
                if (existingRequest != null)
                {
                    existingRequest.StatusId = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Cancelled.ToString().ToLower() select st.Id).FirstOrDefault();
                    existingRequest.Modifiedby = userInfo.UserID;
                    existingRequest.ModifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = true;
                    //Send notifications
                    //Email
                    var body = "Hi,<br><br>Your Functional Request has been cancelled.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                    EmailHelper emailHelper = new EmailHelper();
                    emailHelper.Send(newOwner.tEmail, "Functional Request Status", body);
                    //SMS
                    var msg = NAMessages.FunctionalReqCancel;
                    //SMSSend(newOwner.tMobileNumber, msg);
                    ApplicationHelper.SendSMS(newOwner.tMobileNumber, msg);

                    //New Model for Audit Trail
                    var newObjModel = new FunctionalDetail
                    {
                        //CheckListDate = chkDate

                        // Comment=comments ,
                        //ApproveDate = DateTime.Now
                        StatusId = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Cancelled.ToString().ToLower() select st.Id).FirstOrDefault()

                    };

                    GeneralRepository.CreateAuditTrail(Constants.Update, Constants.FunctionalDetails, "FunctionalDetails", reqNo, oldObjModel, newObjModel, userInfo.UserID.ToString());


                }
            }
            return flag;
        }
        public DataSourceResult GetFunctionalDataByApproverId(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var objFunctional = (from funDetails in dbContext.FunctionalDetails
                                     join alot in dbContext.AllotmentMasters on funDetails.Rid equals alot.rid
                                     join prop in dbContext.SchemePropTrans on alot.propertyId equals prop.propertyId
                                     join statusMas in dbContext.StatusMasters on funDetails.StatusId equals statusMas.Id
                                     where funDetails.IsActive == true && funDetails.Approver == userInfo.UserID
                                     && funDetails.StatusId == Constants.InProgress //Made change as per discussion with Vishal Shukla on 23rd Feb. 2017
                                     select new FunctionalModel
                                     {
                                         RequestNo = funDetails.RequestNo,
                                         RId = funDetails.Rid,
                                         ApproveDate = funDetails.ApproveDate,
                                         CreatedDate = funDetails.CreatedDate,
                                         PropertyNumber = funDetails.PropertyNumber,
                                         SectorName = prop.SectorMst.sectorName,
                                         BlockName = prop.BlockMst.blockName,
                                         PropertyNo = prop.propertyNo,
                                         Status = statusMas.Status
                                     });
                return objFunctional.ToDataSourceResult(req);
            }
        }
        public bool SaveFunctionalApprovalStatus(string comments, int intStatus, int ReqNo)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingRequest = (from mut in dbContext.FunctionalDetails where mut.RequestNo == ReqNo && mut.IsActive == true select mut).FirstOrDefault();
                FunctionalDetail oldObjModel = new FunctionalDetail();//Old Model
                //oldObjModel.Comment  = existingRequest.Comment;
                oldObjModel.StatusId = existingRequest.StatusId;
                //oldObjModel .ApproveDate=existingRequest.ApproveDate ;
                //New Model for Audit Trail
                FunctionalDetail newObjModel = new FunctionalDetail();

                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == existingRequest.Rid select appDet).FirstOrDefault();//For notification details
                if (existingRequest != null)
                {
                    if (intStatus == 1)
                    {
                        existingRequest.StatusId = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Approved.ToString().ToLower() select st.Id).FirstOrDefault();
                        newObjModel.StatusId = existingRequest.StatusId;
                        existingRequest.Functional = true;
                        existingRequest.ApproveDate = DateTime.Now;
                    }
                    else
                    {
                        existingRequest.StatusId = (from st in dbContext.StatusMasters where st.Status.ToLower() == AllotmentStatus.Rejected.ToString().ToLower() select st.Id).FirstOrDefault();
                        newObjModel.StatusId = existingRequest.StatusId;
                    }

                    existingRequest.Comment = comments;
                    existingRequest.Modifiedby = userInfo.UserID;
                    existingRequest.ModifiedDate = DateTime.Now;

                    dbContext.SaveChanges();

                    if (intStatus == 1)
                    {
                        if (existingRequest.OnlineRequestNo != 0)
                        {
                            UpdateServiceRequest(existingRequest.OnlineRequestNo, existingRequest.Rid, newOwner.departmentId, comments);
                        }
                    }
                    flag = true;
                    //Send notifications
                    //Email
                    string body = string.Empty;
                    string msg = string.Empty;
                    if (intStatus == 1)
                    {
                        body = "Hi,<br><br>Your Functional Request has been approved. Kindly clear all your Dues.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                        msg = NAMessages.FunctionalReqApprove;
                    }
                    else
                    {
                        body = "Hi,<br><br>Your Functional Request has been rejected.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                        msg = NAMessages.FunctionalReqReject;
                    }
                    EmailHelper emailHelper = new EmailHelper();
                    emailHelper.Send(newOwner.tEmail, "Functional Request Status", body);
                    //SMS
                    //SMSSend(newOwner.tMobileNumber, msg);
                    ApplicationHelper.SendSMS(newOwner.tMobileNumber, msg);



                    int RequestNo = ReqNo;
                    GeneralRepository.CreateAuditTrail(Constants.Update, Constants.FunctionalDetails, "FunctionalDetails", RequestNo, oldObjModel, newObjModel, userInfo.UserID.ToString());


                }
            }
            return flag;
        }
        /// <summary>
        /// Fetches Transfer Details based on RID
        /// </summary>
        /// <param name="rId">RID</param>
        /// <returns></returns>
        public FunctionalModel GetFUnctionalTransferDetailsByRID(int rId)
        {
            var details1 = new DetailsByRId();
            var details = new FunctionalModel();

            using (var dbContext = new NoidaPMSEntities())
            {
                details = (from am in dbContext.AllotmentMasters
                           join appDet in dbContext.ApplicationDetails on am.rid equals appDet.registrationId
                           join spt in dbContext.SchemePropTrans on am.propertyId equals spt.propertyId
                           join scm in dbContext.SchemeMsts on spt.schemeId equals scm.schemeId
                           join dm in dbContext.DepartmentMsts on am.departmentId equals dm.departmentId
                           join pm in dbContext.PropertyTypeMsts on spt.propertyTypeId equals pm.propertyTypeId
                           join sm in dbContext.SectorMsts on spt.sectorId equals sm.sectorId
                           join bm in dbContext.BlockMsts on spt.blockId equals bm.blockId
                           join fm in dbContext.FloorMsts on spt.floorId equals fm.floorId
                           //join trans in dbContext.Succ_Mut_Trans on am.rid equals trans.Rid
                           //join sta in dbContext.StatusMasters on trans.Status equals sta.Id
                           where am.rid == rId
                           //where sta.Status.ToLower() == AllotmentStatus.Approved.ToString().ToLower() &&//&& trans.Rid == rId
                           //orderby trans.Request_No descending
                           select new FunctionalModel
                           {
                               SchemeName = scm.schemeName,
                               DepttName = dm.departmentName,
                               ApplicationName = appDet.tFirstName + " " + appDet.tMiddleName + " " + appDet.tLastName,
                               Gender = appDet.tGender,
                               RelationName = appDet.tFatherHusbandName,
                               PropertyType = pm.propertyTypeName,
                               Area = spt.totalArea,
                               Floor = fm.floorName,
                               PropertyNumber = sm.sectorName + "/" + bm.blockName + "-" + spt.propertyNo,
                               DepttId = appDet.departmentId.Value
                               //RequestNo = trans.Request_No
                               //FunctionalDueDate = (from func in dbContext.FunctionalDetails where func.Rid == rId orderby func.RequestNo descending select func.FunctionalDueDate).FirstOrDefault()
                           }).FirstOrDefault();
            }
            //Following code commented on 20th Feb. 2017, as discussed with Vishal Shukla because Functional Due Date to be entered by the user from now on.
            //if (details != null) //null check
            //    details.FunctionalDueDate = GetFunctionalDueDate(rId);
            return details;
        }


        public ChallanModel GetChallanDetailsForFunctionalPermission(int rid, int requestNo)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from sprt in dbContext.SchemePropTrans
                           join sect in dbContext.SectorMsts on sprt.sectorId equals sect.sectorId
                           join blok in dbContext.BlockMsts on sprt.blockId equals blok.blockId
                           join pttm in dbContext.PropertyTypeMsts on sprt.propertyTypeId equals pttm.propertyTypeId
                           join scst in dbContext.SchemeCostTrans on sprt.schemeId equals scst.schemeId
                           join sdtt in dbContext.SchemeDepartmentTrans on sprt.schemeId equals sdtt.schemeId
                           join alot in dbContext.AllotmentMasters on sprt.propertyId equals alot.propertyId
                           where alot.rid == rid && sprt.IsActive == true
                           select new ChallanModel
                           {
                               BlockName = blok.blockName,
                               SectorName = sect.sectorName,
                               PropertyNumber = sprt.propertyNo,
                               PropertyTypeName = pttm.propertyTypeName,
                               FormNo = alot.formNo,
                               AllotmentMoney = sprt.allotmentMoney - scst.earnestMoney,
                               RID = alot.rid
                           }).FirstOrDefault();

                ApplicationFormModel appdetails = new ApplicationFormModel();
                if (lst != null)
                {
                    appdetails = (from rpdt in dbContext.FunctionalDetails
                                  join alot in dbContext.AllotmentMasters on rpdt.Rid equals alot.rid
                                  join appl in dbContext.ApplicationDetails on alot.applicationId equals appl.applicationId
                                  where rpdt.Rid == rid && rpdt.RequestNo == requestNo
                                  select new ApplicationFormModel
                                  {
                                      FirstName = appl.tFirstName,
                                      MiddleName = appl.tMiddleName,
                                      LastName = appl.tLastName,
                                      CorrespondingAddress = appl.tCorrespondanceAdd,
                                      Email = appl.tEmail,
                                      MobileNumber = appl.tMobileNumber,
                                      PhoneNumber = appl.tPhoneNumber
                                  }).FirstOrDefault();

                    lst.ApplicationForm = appdetails;
                }
                return lst;
            }
        }

        #region Mortgage Drop Down fill
        public DataSourceResult GetAllMortgage(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var lstRId = (from mor in dbContext.MortgageDetails
                              join allot in dbContext.AllotmentMasters on mor.RID equals allot.rid
                              join applicant in dbContext.ApplicationDetails on allot.rid equals applicant.registrationId
                              join sche in dbContext.SchemeMsts on applicant.schemeId equals sche.schemeId
                              join deptt in dbContext.DepartmentMsts on applicant.departmentId equals deptt.departmentId
                              join status in dbContext.StatusMasters on mor.StatusId equals status.Id
                              where loginUserDeptt.Contains(applicant.departmentId) && mor.IsActive == true
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
                                  ApproveDate = mor.ApproveDate
                              });
                return lstRId.ToDataSourceResult(req);
            }
        }

        public DataSourceResult GetAllMortgageByRid(DataSourceRequest req, int Rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var lstRId = (from mor in dbContext.MortgageDetails
                              join allot in dbContext.AllotmentMasters on mor.RID equals allot.rid
                              join applicant in dbContext.ApplicationDetails on allot.rid equals applicant.registrationId
                              join sche in dbContext.SchemeMsts on applicant.schemeId equals sche.schemeId
                              join deptt in dbContext.DepartmentMsts on applicant.departmentId equals deptt.departmentId
                              join status in dbContext.StatusMasters on mor.StatusId equals status.Id
                              where loginUserDeptt.Contains(applicant.departmentId) && mor.RID == Rid && mor.StatusId == 1
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
                                  ApproveDate = mor.ApproveDate
                              });
                return lstRId.ToDataSourceResult(req);
            }
        }
        /// <summary>
        /// To Get All MortgageType.
        /// </summary>
        /// <returns></returns>
        public List<DDList> GetMortgageType()
        {
            List<DDList> mortgageType = new List<DDList>();
            foreach (int value in Enum.GetValues(typeof(MortgageType)))
            {
                mortgageType.Add(new DDList
                {
                    text = Enum.GetName(typeof(MortgageType), value),
                    id = value
                });
            }
            return mortgageType;
        }
        /// <summary>
        /// To Get All MortgagePrevLoan.
        /// </summary>
        /// <returns></returns>
        public List<DDList> GetMortgagePrevLoan()
        {
            List<DDList> mortgagePrevLoan = new List<DDList>();
            foreach (int value in Enum.GetValues(typeof(MortgagePrevLoan)))
            {
                mortgagePrevLoan.Add(new DDList
                {
                    text = Enum.GetName(typeof(MortgagePrevLoan), value),
                    id = value
                });
            }
            return mortgagePrevLoan;
        }

        /// <summary>
        /// To Get Property Details @Ajax Call
        /// </summary>
        /// <param name="rID"></param>
        /// <returns></returns>
        public MortgageModel GetPropertyDetailByRid(int rID)
        {
            var lst = new MortgageModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                lst = (from allot in dbContext.AllotmentMasters
                       join applicant in dbContext.ApplicationDetails on allot.rid equals applicant.registrationId
                       join sche in dbContext.SchemeMsts on applicant.schemeId equals sche.schemeId
                       join deptt in dbContext.DepartmentMsts on applicant.departmentId equals deptt.departmentId
                       //get property conflict based on schemeid and departmentid (changed by Shatrughna)
                       //join proptrans in dbContext.SchemePropTrans on new { allot.schemeId, allot.departmentId } equals new { proptrans.schemeId, proptrans.departmentId }
                       join proptrans in dbContext.SchemePropTrans on allot.propertyId equals proptrans.propertyId
                       join proptype in dbContext.PropertyTypeMsts on proptrans.propertyTypeId equals proptype.propertyTypeId
                       join sec in dbContext.SectorMsts on proptrans.sectorId equals sec.sectorId
                       join block in dbContext.BlockMsts on proptrans.blockId equals block.blockId
                       where allot.rid == rID && allot.isActive == 1
                       select new MortgageModel
                       {
                           ApplicantName = applicant.tFirstName + " " + applicant.tMiddleName + " " + applicant.tLastName,
                           FatherName = applicant.tFatherHusbandName,
                           DepartmentName = deptt.departmentName,
                           PropertyType = proptype.propertyTypeName,
                           PropertyNumber = sec.sectorName + "/" + block.blockName + "-" + proptrans.propertyNo,
                           PropertyCost = proptrans.totalPropertyCost,
                           Functional = (from fun in dbContext.FunctionalDetails where fun.Rid == rID && fun.IsActive == true && fun.StatusId == Constants.Approved select "true").FirstOrDefault(),
                           Mortgage = (from mor in dbContext.MortgageDetails where mor.RID == rID && mor.IsActive == true && mor.StatusId == Constants.Approved && mor.PreviousLoanNoc == Constants.PreviousLoanNo select true).FirstOrDefault(),
                           DepartmentId = applicant.departmentId,
                           //PreviousBankDetail = (from bankDetail in dbContext.MortgageDetails where bankDetail.RID == rID && (bankDetail.PreviousLoanNoc == Constants.PreviousLoanYes || bankDetail.PreviousLoanNoc == Constants.InValid) && bankDetail.IsActive == true orderby bankDetail.RequestNo descending select bankDetail.BankName + " " + bankDetail.BranchAddress).FirstOrDefault(),
                           Gender = applicant.tGender,
                           AllotmentDate = allot.allotmentDate
                           //TotalDues = GetBalanceDueTillDate(rID, applicant.departmentId)
                       }).FirstOrDefault();
                //updated on 21-6-2017 by shatrughna
                var bankList = dbContext.MortgageDetails.Where(m => m.RID == rID && m.StatusId == StatusOption.Approved).ToList();
                if (bankList != null && bankList.Count > 0)
                {
                    var reqNo = bankList.Max(m => m.RequestNo);
                    var md = dbContext.MortgageDetails.Where(m => m.RequestNo == reqNo).FirstOrDefault();
                    lst.PreviousBankDetail = md.BankName + ", " + md.BranchAddress;
                }
                return lst;
            }
        }
        public string GetMortgagePreviousLoanDetails(int? requestNo, int? Rid)
        {
            string sBankDetails = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                var bankDet = dbContext.MortgageDetails.Where(m => m.RID == Rid && m.StatusId == StatusOption.Approved).OrderByDescending(m => m.RequestNo).ToList();
                if (bankDet != null && bankDet.Count > 0)
                {
                    if (requestNo > 0)
                    {
                        bankDet = bankDet.Where(c => c.RequestNo < requestNo).OrderByDescending(m => m.RequestNo).ToList();
                    }
                    string sBankName = bankDet.FirstOrDefault().BankName;
                    string sBranchAddress = bankDet.FirstOrDefault().BranchAddress;
                    sBankDetails = sBankName + ", " + sBranchAddress;
                }
            }
            return sBankDetails;
        }

        /// <summary>
        /// To add New Mortgage.
        /// </summary>
        /// <param name="mortgageDate"></param>
        /// <param name="bankName"></param>
        /// <param name="mortgageType"></param>
        /// <param name="previousLoanNoc"></param>
        /// <param name="branchAddress"></param>
        /// <param name="processingFee"></param>
        /// <param name="user"></param>
        /// <param name="rid"></param>
        /// <returns></returns>        
        public bool AddMortgage(DateTime mortgageDate, string bankName, string mortgageType, short previousLoanNoc, string branchAddress, decimal processingFee, decimal sanctionedAmount, string user, int rid, DateTime validUpto, int? reqRefNo)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //changed on 31 march 2017
                //var previousMortgage = dbContext.MortgageDetails.Where(m => m.RID == rid && m.IsActive == true && previousLoanNoc == 1).ToList();
                var previousMortgage = dbContext.MortgageDetails.Where(m => m.RID == rid && m.IsActive == true).ToList();
                if (previousMortgage != null && previousMortgage.Count > 0)
                {
                    previousMortgage.ForEach(m => m.IsActive = false);
                }

                var dataResult = new MortgageDetail
                {
                    MortgageDate = mortgageDate,
                    BankName = bankName,
                    MortgageType = mortgageType,
                    PreviousLoanNoc = previousLoanNoc,
                    BranchAddress = branchAddress,
                    ProcessingFee = processingFee,
                    SanctionedAmount = sanctionedAmount,
                    RID = rid,
                    ValidUpto = validUpto,
                    OnlineRequestNo = reqRefNo,
                    Approver = Convert.ToInt32(user),
                    //Approver = (from uname in dbContext.UmUserMasters where uname.UserName.ToLower().Equals(user.ToLower()) && uname.IsActive == true select uname.UserRefId).FirstOrDefault(),
                    StatusId = Constants.InProgress,
                    IsActive = true,
                    CreatedBy = userInfo.UserID,
                    CreatedDate = DateTime.Now
                };
                dbContext.MortgageDetails.Add(dataResult);
                dbContext.SaveChanges();
                if (user != null)
                {
                    var approver = dbContext.UmUserMasters.Where(m => m.UserName.ToLower().Equals(user.ToLower()) && m.IsActive == true).FirstOrDefault();
                    if (approver != null)
                    {
                        EmailHelper emailHelper = new EmailHelper();
                        string emailMessage = string.Empty;
                        emailMessage = "Hi,<br><br>Mortgage Permission Request has been submitted for approval of Registration id:" + rid + ".<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                        emailHelper.Send(approver.Email, "Rent Permission Request Status", emailMessage);
                    }
                }
                //For Audit trails...
                //var oldObj = new MortgageDetail();
                //GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.MortgageDetail, Constants.AddMortgageDetail, dataResult.RID, oldObj, dataResult, userInfo.UserID.ToString());
                //
                return true;
            }
        }

        /// <summary>
        /// Get All Mortgage As per Approver.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public DataSourceResult GetAllMortgageByApprover(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var lstRId = (from mor in dbContext.MortgageDetails
                              join allot in dbContext.AllotmentMasters on mor.RID equals allot.rid
                              join applicant in dbContext.ApplicationDetails on allot.rid equals applicant.registrationId
                              join sche in dbContext.SchemeMsts on applicant.schemeId equals sche.schemeId
                              join deptt in dbContext.DepartmentMsts on applicant.departmentId equals deptt.departmentId
                              join status in dbContext.StatusMasters on mor.StatusId equals status.Id
                              where mor.Approver == userInfo.UserID && mor.StatusId == Constants.InProgress && loginUserDeptt.Contains(applicant.departmentId)
                              select new MortgageModel
                              {
                                  RequestNo = mor.RequestNo,
                                  RID = mor.RID,
                                  SchemeName = sche.schemeName,
                                  DepartmentName = deptt.departmentName,
                                  StatusName = status.Status,
                                  ApplicantName = applicant.tFirstName + " " + applicant.tMiddleName + " " + applicant.tLastName,
                                  MortgageDate = mor.MortgageDate,
                                  MortgageType = mor.MortgageType == "1" ? MortgageType.Collateral.ToString() : MortgageType.Normal.ToString()
                              });
                return lstRId.ToDataSourceResult(req);
            }
        }
        /// <summary>
        /// To get Mortgage details by requested id for edit or update.
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public MortgageModel GetMortgageByRequestID(int requestId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var dataResult = (from mort in dbContext.MortgageDetails
                                  join stat in dbContext.StatusMasters on mort.StatusId equals stat.Id
                                  where mort.RequestNo == requestId
                                  select new MortgageModel
                                  {
                                      RequestNo = mort.RequestNo,
                                      RID = mort.RID,
                                      MortgageDate = mort.MortgageDate,
                                      BankName = mort.BankName,
                                      BranchAddress = mort.BranchAddress,
                                      MortgageType = mort.MortgageType,
                                      ProcessingFee = mort.ProcessingFee,
                                      SanctionedAmount = mort.SanctionedAmount,
                                      ValidUpto = mort.ValidUpto,
                                      PreviousLoanNoc = mort.PreviousLoanNoc,
                                      ApproveDate = mort.ApproveDate,
                                      StatusId = mort.StatusId,
                                      StatusName = stat.Status,
                                      OnlineRequestNo = mort.OnlineRequestNo,
                                      ApproverName = (from uname in dbContext.UmUserMasters where uname.UserRefId == mort.Approver && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                      CommentDate = mort.CommentDate,
                                      GetComment = mort.Comment,
                                      PreviousBankDetail = (from bankDetail in dbContext.MortgageDetails where (bankDetail.RID == mort.RID && (bankDetail.PreviousLoanNoc == Constants.PreviousLoanYes || bankDetail.PreviousLoanNoc == Constants.InValid)) && bankDetail.IsActive == true orderby bankDetail.RequestNo descending select bankDetail.BankName + " " + bankDetail.BranchAddress).Skip(1).FirstOrDefault()
                                  }).FirstOrDefault();
                return dataResult;
            }
        }

        // To Save comment of approver.
        public bool SaveCommentByRequestID(int requestNo, string Comment, bool acceptReject)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.MortgageDetails.FirstOrDefault(m => m.RequestNo == requestNo);
                var flag = false;
                if (result != null)
                {
                    result.Comment = Comment;
                    result.CommentDate = DateTime.Now;
                    result.ApproveDate = DateTime.Now;
                    result.StatusId = acceptReject == true ? Constants.Approved : Constants.RejectedProp;
                    dbContext.SaveChanges();

                    if (result.StatusId == Constants.Approved)
                    {
                        if (result.OnlineRequestNo != 0)
                        {
                            var deptId = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == result.RID).departmentId;
                            UpdateServiceRequest(result.OnlineRequestNo, result.RID, deptId, Comment);
                        }
                    }

                    flag = true;
                }
                return flag;
            }
        }
        /// <summary>
        /// To Update Mortgage by requested id.
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="mortgageDate"></param>
        /// <param name="bankName"></param>
        /// <param name="mortgageType"></param>
        /// <param name="previousLoanNoc"></param>
        /// <param name="branchAddress"></param>
        /// <param name="processingFee"></param>
        /// <param name="user"></param>
        /// <param name="rid"></param>
        /// <returns></returns>
        public bool UpdateMortgage(int requestID, DateTime mortgageDate, string bankName, string mortgageType, short previousLoanNoc, string branchAddress, decimal processingFee, decimal sanctionedAmount, string user, int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var dbResult = dbContext.MortgageDetails.FirstOrDefault(m => m.RequestNo == requestID);
                var flag = false;
                if (dbResult != null)
                {
                    //var objOld = new MortgageDetail
                    //{
                    //    MortgageDate = dbContext.MortgageDate,

                    //};

                    dbResult.MortgageDate = mortgageDate;
                    dbResult.BankName = bankName;
                    dbResult.MortgageType = mortgageType;
                    dbResult.PreviousLoanNoc = previousLoanNoc;
                    dbResult.BranchAddress = branchAddress;
                    dbResult.ProcessingFee = processingFee;
                    dbResult.SanctionedAmount = sanctionedAmount;
                    dbResult.RID = rid;
                    //dbResult.Approver = (from uname in dbContext.UmUserMasters where uname.UserName.ToLower().Equals(user.ToLower()) && uname.IsActive == true select uname.UserRefId).FirstOrDefault();
                    dbResult.Approver = Convert.ToInt32(user);
                    dbResult.StatusId = Constants.InProgress;
                    dbResult.IsActive = true;
                    dbResult.Modifiedby = userInfo.UserID;
                    dbResult.ModifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = true;
                }
                return flag;
            }
        }
        #region Rent Permission
        //Get RID for rent permission only industrial and institutional
        public DataSourceResult GetRIDsForRentPermission(DataSourceRequest Req, int Rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstDeptts = (from u in dbContext.UmUserMasters
                                 join d in dbContext.UmUserDepartmentTrans on u.UserRefId equals d.UserRefId
                                 where u.UserRefId == userInfo.UserID && (d.DepartmentId == DepartmentOption.Institutional || d.DepartmentId == DepartmentOption.Industrial)
                                 select d.DepartmentId).ToList();

                var lst = (from registry in dbContext.FunctionalDetails // dbContext.RegistryDetails
                           join alotment in dbContext.AllotmentMasters on registry.Rid equals alotment.rid
                           //where alot.isActive == 1 && (alot.departmentId == DepartmentOption.Institutional || alot.departmentId == DepartmentOption.Industrial) && alot.isStatus.ToLower() == AllotmentStatus.Approved.ToString().ToLower()
                           where alotment.isActive == 1 && registry.IsActive == true && lstDeptts.Contains(alotment.departmentId) && alotment.isStatus.ToLower() == AllotmentStatus.Approved.ToString().ToLower()
                           orderby registry.RequestNo descending
                           select new DDList
                           {
                               id = alotment.rid,
                               text = alotment.rid.ToString()
                           });
                if (Rid > 0) { lst = lst.Where(m => m.id == Rid); }
                return lst.ToDataSourceResult(Req);
            }
            //throw new NotImplementedException();
        }

        //get allottee details based on rid
        public RentPermissionModel GetAllotteDetailsForRentPermission(int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var alotedetails = (from alot in dbContext.AllotmentMasters
                                    join appl in dbContext.ApplicationDetails on alot.applicationId equals appl.applicationId
                                    join dept in dbContext.DepartmentMsts on alot.departmentId equals dept.departmentId
                                    join scpt in dbContext.SchemePropTrans on alot.propertyId equals scpt.propertyId
                                    join flor in dbContext.FloorMsts on scpt.floorId equals flor.floorId
                                    join prtt in dbContext.PropertyTypeMsts on scpt.propertyTypeId equals prtt.propertyTypeId
                                    join sect in dbContext.SectorMsts on scpt.sectorId equals sect.sectorId
                                    join blok in dbContext.BlockMsts on scpt.blockId equals blok.blockId
                                    where alot.rid == rid
                                    select new RentPermissionModel
                                    {
                                        DepartmentId = dept.departmentId,
                                        Department = dept.departmentName,
                                        AllotteeName = appl.tFirstName + " " + appl.tMiddleName + " " + appl.tLastName,
                                        FatherOrHusbandName = appl.tFatherHusbandName,
                                        SigningAuthority = appl.tSigningAuthority,
                                        GenderOrCompany = appl.tGender,
                                        TotalArea = scpt.totalArea.ToString(),
                                        FloorNo = flor.floorName,
                                        Sector = sect.sectorName,
                                        Block = blok.blockName,
                                        PropertyType = prtt.propertyTypeName,
                                        PropertyNo = scpt.propertyNo,
                                        IsRentPermissionRequested = false
                                        //IsRentPermissionRequested = (from permission in dbContext.RentPermissionDetails where permission.Rid == rid select permission).FirstOrDefault() != null ? (bool)((from permission in dbContext.RentPermissionDetails where permission.Rid == rid select permission).FirstOrDefault().IsActive) : false
                                        //TotalDues = businessRule.GetBalanceDueTillDate(alot.rid, dept.departmentId)                                       
                                    }).FirstOrDefault();
                var services = dbContext.Customer_ServiceRequest.Where(r => r.Registration_No == rid.ToString() && r.DepartmentId == alotedetails.DepartmentId && r.Request_Status == Constants.Initiated).FirstOrDefault();
                var functional = dbContext.FunctionalDetails.Where(f => f.Rid == rid && f.IsActive == true).FirstOrDefault();
                if (functional != null)
                {
                    alotedetails.Functional = functional.Functional;
                }
                else
                {
                    alotedetails.Functional = false;
                }

                if (services != null)
                {
                    alotedetails.ServiceRequestRefNo = services.Id;
                }
                return alotedetails;
            }

            //throw new NotImplementedException();
        }

        // save or update Rent Permission request 
        public bool SaveRentPermissionRequest(RentPermissionModel model)
        {
            var flag = false;
            var recordStatus = 0;//used only for notification updated=2 and inserted=1 
            using (var dbContext = new NoidaPMSEntities())
            {
                var approver = dbContext.UmUserMasters.Where(u => u.UserRefId == model.Approver).FirstOrDefault();
                //var allottee = (from app in dbContext.ApplicationDetails join alot in dbContext.AllotmentMasters on app.applicationId equals alot.applicationId where alot.rid == model.Rid select app).FirstOrDefault();
                var allottee = (from alot in dbContext.AllotmentMasters join app in dbContext.ApplicationDetails on alot.applicationId equals app.applicationId where alot.rid == model.Rid select app).FirstOrDefault();

                var request = dbContext.RentPermissionDetails.Where(r => r.Rid == model.Rid && r.IsActive == true).FirstOrDefault();
                // as discussed with Vishal sir multiple entries are allwed. Previous entry do false and add new entry
                if (request != null)
                {
                    if (allottee.departmentId != NADepartment.Institutional)
                    {
                        if (model.StatusId == 2 || model.StatusId == 3)
                        {
                            request.StatusId = StatusOption.InProgress;
                            request.Comment = model.Comment;
                            dbContext.SaveChanges();
                            return flag = true;
                        }
                        else
                        {
                            request.IsActive = false;
                            dbContext.SaveChanges();
                        }
                    }

                }
                {
                    if (allottee.departmentId == NADepartment.Institutional)
                    {
                        var xList = dbContext.RentPermissionDetails.Where(r => r.Rid == model.Rid && r.IsActive == true).ToList();
                        var totalArea = model.TotalArea;
                        decimal? rentedArea = model.Area;
                        foreach (var a in xList)
                        {
                            rentedArea = rentedArea + a.Area;
                        }
                        if (rentedArea > Convert.ToDecimal(totalArea))
                        {
                            flag = false;
                        }
                        else
                        {
                            RentPermissionModel oldModel = new RentPermissionModel();
                            RentPermissionDetail rent = new RentPermissionDetail();
                            rent.Rid = model.Rid;
                            rent.TenantName = model.TenantName;
                            rent.TenantProject = model.TenantProject;
                            rent.Approver = model.Approver;
                            rent.IsActive = true;
                            rent.StatusId = StatusOption.InProgress;
                            rent.RentingDate = model.RentingDate;
                            rent.RentingEndDate = model.RentingEndDate;
                            //rent.Amount = model.Amount;
                            rent.RentDurationYears = model.RentDuration;
                            rent.RentingCharge = model.RentingCharge;
                            rent.CreatedBy = userInfo.UserID;
                            rent.CreatedDate = DateTime.Now;
                            rent.OnlineRequestNo = model.ServiceRequestRefNo;
                            rent.Area = model.Area;
                            rent.Remarks = model.Remarks;
                            dbContext.RentPermissionDetails.Add(rent);
                            dbContext.SaveChanges();
                            flag = true;
                            recordStatus = 1;
                            //audit trail
                            GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.RentPermissionDetails, model.ViewName, rent.RequestNo, oldModel, model, userInfo.UserID.ToString());
                        }
                    }
                    else
                    {
                        RentPermissionModel oldModel = new RentPermissionModel();
                        RentPermissionDetail rent = new RentPermissionDetail();
                        rent.Rid = model.Rid;
                        rent.TenantName = model.TenantName;
                        rent.TenantProject = model.TenantProject;
                        rent.Approver = model.Approver;
                        rent.IsActive = true;
                        rent.StatusId = StatusOption.InProgress;
                        rent.RentingDate = model.RentingDate;
                        rent.RentingEndDate = model.RentingEndDate;
                        //rent.Amount = model.Amount;
                        rent.RentDurationYears = model.RentDuration;
                        rent.RentingCharge = model.RentingCharge;
                        rent.CreatedBy = userInfo.UserID;
                        rent.CreatedDate = DateTime.Now;
                        rent.OnlineRequestNo = model.ServiceRequestRefNo;
                        rent.Area = model.Area;
                        rent.Remarks = model.Remarks;
                        dbContext.RentPermissionDetails.Add(rent);
                        dbContext.SaveChanges();
                        flag = true;
                        recordStatus = 1;
                        //audit trail
                        GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.RentPermissionDetails, model.ViewName, rent.RequestNo, oldModel, model, userInfo.UserID.ToString());
                    }

                }
                //if (request != null)
                //{
                //    RentPermissionModel oldModel = new RentPermissionModel();//old rent object for audit
                //    oldModel.StatusId = request.StatusId;
                //    oldModel.Comment = request.Comment;

                //    request.TenantName = model.TenantName;
                //    request.TenantProject = model.TenantProject;
                //    request.Approver = model.Approver;
                //    request.RentingCharge = model.RentingCharge;
                //    request.RentDurationYears = model.RentDuration;
                //    request.RentingDate = model.RentingDate;
                //    request.RentingEndDate = model.RentingEndDate;
                //    request.Comment = model.Comment;
                //    request.OnlineRequestNo = model.ServiceRequestRefNo;
                //    request.Modifiedby = userInfo.UserID;
                //    request.ModifiedDate = DateTime.Now;
                //    request.StatusId = StatusOption.InProgress;
                //    dbContext.SaveChanges();
                //    flag = true;
                //    recordStatus = 2;
                //    //audit trail
                //    GeneralRepository.CreateAuditTrail(Constants.Update, Constants.RentPermissionDetails, model.ViewName, request.RequestNo, oldModel, model, userInfo.UserID.ToString());
                //}
                //else
                //{
                //    var exrequest = dbContext.RentPermissionDetails.Where(r => r.RequestNo == model.RequestNo && r.Rid == model.Rid).FirstOrDefault();
                //    if (exrequest == null)
                //    {
                //        RentPermissionModel oldModel = new RentPermissionModel();
                //        RentPermissionDetail rent = new RentPermissionDetail();
                //        rent.Rid = model.Rid;
                //        rent.TenantName = model.TenantName;
                //        rent.TenantProject = model.TenantProject;
                //        rent.Approver = model.Approver;
                //        rent.IsActive = true;
                //        rent.StatusId = StatusOption.InProgress;
                //        rent.RentingDate = model.RentingDate;
                //        rent.RentingEndDate = model.RentingEndDate;
                //        //rent.Amount = model.Amount;
                //        rent.RentDurationYears = model.RentDuration;
                //        rent.RentingCharge = model.RentingCharge;
                //        rent.CreatedBy = userInfo.UserID;
                //        rent.CreatedDate = DateTime.Now;
                //        rent.OnlineRequestNo = model.ServiceRequestRefNo;
                //        dbContext.RentPermissionDetails.Add(rent);
                //        dbContext.SaveChanges();
                //        flag = true;
                //        recordStatus = 1;
                //        //audit trail
                //        GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.RentPermissionDetails, model.ViewName, rent.RequestNo, oldModel, model, userInfo.UserID.ToString());
                //    }
                //}

                //Send notifications
                //Email
                string emailMessage = string.Empty;
                string mobileMessage = string.Empty;
                if (recordStatus == 1)
                {
                    emailMessage = "Hi,<br><br>Rent Permission Request has been submitted for approval of Registration id:" + model.Rid + ".<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                    mobileMessage = string.Format(NAMessages.RentPermissionReqSubmit, model.Rid);
                }
                else
                {
                    emailMessage = "Hi,<br><br>Rent Permission Request has been resubmitted for approval of Registration id:" + model.Rid + ".<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                    mobileMessage = string.Format(NAMessages.RentPermissionReqResubmit, model.Rid);
                }
                EmailHelper emailHelper = new EmailHelper();
                emailHelper.Send(approver.Email, "Rent Permission Request Status", emailMessage);
                emailHelper.Send(allottee.email, "Rent Permission Request Status", emailMessage);
                //SMS
                //SMSSend(allottee.tMobileNumber, mobileMessage);
                //SMSSend(approver.Mobile, mobileMessage);
                ApplicationHelper.SendSMS(allottee.tMobileNumber, mobileMessage);
            }
            return flag;
        }

        //Get property type for rent permission only industrial or institutional
        public List<DDList> GetPropertyTypeForRentPermission()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var propType = (from ptm in dbContext.PropertyTypeMsts
                                join dep in dbContext.DepartmentMsts on ptm.departmentId equals dep.departmentId
                                where ptm.departmentId == DepartmentOption.Institutional || ptm.departmentId == DepartmentOption.Industrial
                                select new DDList
                                {
                                    id = ptm.propertyTypeId,
                                    text = ptm.propertyTypeName
                                }).ToList();
                return propType;
            }
            //throw new NotImplementedException();
        }

        //get rent duration 5 or 10 years
        public List<DDList> GetRentDurationForRentPermission()
        {
            using (var dbContexty = new NoidaPMSEntities())
            {
                return new List<DDList>() { 
                    new DDList(){text="5",id=5},
                    new DDList(){text="10",id=10}
                };
            }
        }

        #endregion


        public List<DDList> GetApproverForRentPermission(int rid)
        {
            int userid = userInfo.UserID;
            using (var dbContext = new NoidaPMSEntities())
            {
                var usrlist = dbContext.AllotmentMasters.Where(a => a.rid == rid).FirstOrDefault();

                var deptList = (from user in dbContext.UmUserMasters
                                join dptt in dbContext.UmUserDepartmentTrans on user.UserRefId equals dptt.UserRefId
                                join dept in dbContext.DepartmentMsts on dptt.DepartmentId equals dept.departmentId
                                join rolt in dbContext.UmUserMasterRoles on user.UserRefId equals rolt.UserRefId
                                join rols in dbContext.UmRoleMasters on rolt.RoleId equals rols.RoleId
                                //where user.IsActive == true && (dept.departmentId == DepartmentOption.Institutional || dept.departmentId == DepartmentOption.Industrial) && dptt.DepartmentId != userid && rols.RoleType != Constants.SuperAdmin
                                where user.IsActive == true && user.UserRefId != userid && dept.departmentId == usrlist.departmentId && rols.RoleType != Constants.SuperAdmin
                                select new DDList
                                {
                                    id = user.UserRefId,
                                    text = user.UserName + " " + user.FirstName
                                }).Distinct().ToList();
                return deptList;
            }
        }


        public DataSourceResult GetRentRequestsList(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var departmentList = (from dept in dbContext.UmDepartmentMasters
                                      join udts in dbContext.UmUserDepartmentTrans on dept.DepartmentId equals udts.DepartmentId
                                      where udts.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();
                var requests = (from rent in dbContext.RentPermissionDetails
                                join alot in dbContext.AllotmentMasters on rent.Rid equals alot.rid
                                //where departmentList.Contains(alot.departmentId.Value) && rent.Approver==userInfo.UserID
                                where rent.Approver == userInfo.UserID && rent.IsActive == true
                                select new RentModel
                                {
                                    RequestNo = rent.RequestNo,
                                    RID = rent.Rid.Value,
                                    SchemeName = alot.SchemeMst.schemeName,
                                    Department = alot.DepartmentMst.departmentName,
                                    ApplicantName = alot.ApplicationDetail.tFirstName + " " + alot.ApplicationDetail.tMiddleName + " " + alot.ApplicationDetail.tLastName,
                                    TenantName = rent.TenantName,
                                    RequestDate = rent.RentingDate.Value,
                                    Status = rent.StatusMaster.Status
                                });//.ToList();
                return requests.ToDataSourceResult(request);
            }
        }


        public RentPermissionModel GetRentRequestByRequestNumber(int requestNo)
        {
            using (var dbContext = new NoidaPMSEntities())
            {

                var rents = (from rent in dbContext.RentPermissionDetails
                             join prop in dbContext.SchemePropTrans on rent.AllotmentMaster.propertyId equals prop.propertyId
                             where rent.RequestNo == requestNo
                             select new RentPermissionModel
                             {
                                 RequestNo = rent.RequestNo,
                                 Rid = rent.Rid,
                                 TenantName = rent.TenantName,
                                 TenantProject = rent.TenantProject,
                                 RentDuration = rent.RentDurationYears,
                                 RentingCharge = rent.RentingCharge,
                                 RentingDate = rent.RentingDate,
                                 RentingEndDate = rent.RentingEndDate,
                                 StatusId = rent.StatusId,
                                 Comment = rent.Comment,
                                 //TotalDues = 0,
                                 //TotalDues = engine.GetBalanceDueTillDate(alot.rid, dept.departmentId),
                                 //Functional = true,
                                 DepartmentId = rent.AllotmentMaster.departmentId.Value,
                                 Department = rent.AllotmentMaster.DepartmentMst.departmentName,
                                 SchemeId = rent.AllotmentMaster.schemeId,
                                 SchemeName = rent.AllotmentMaster.SchemeMst.schemeName,
                                 AllotteeName = rent.AllotmentMaster.ApplicationDetail.tFirstName + " " + rent.AllotmentMaster.ApplicationDetail.tMiddleName + " " + rent.AllotmentMaster.ApplicationDetail.tLastName,
                                 FatherOrHusbandName = rent.AllotmentMaster.ApplicationDetail.tFatherHusbandName,
                                 GenderOrCompany = rent.AllotmentMaster.ApplicationDetail.tGender,
                                 SigningAuthority = rent.AllotmentMaster.ApplicationDetail.tSigningAuthority,
                                 TotalArea = prop.totalArea.ToString(),
                                 FloorNo = prop.FloorMst.floorName,
                                 Block = prop.BlockMst.blockName,
                                 Sector = prop.SectorMst.sectorName,
                                 PropertyNo = prop.propertyNo,
                                 PropertyType = prop.PropertyTypeMst.propertyTypeName,
                                 RequestStatus = rent.StatusMaster.Status,
                                 ServiceRequestRefNo = rent.OnlineRequestNo,
                                 AssignedUser = dbContext.UmUserMasters.Where(u => u.UserRefId == rent.Approver).Select(u => u.UserName).FirstOrDefault(),
                                 CommentDate = rent.CommentDate,
                                 Area = rent.Area,
                                 Remarks = rent.Remarks
                             }).FirstOrDefault();
                var functional = dbContext.FunctionalDetails.Where(f => f.Rid == rents.Rid).FirstOrDefault();
                if (functional != null)
                {
                    rents.Functional = functional.Functional;
                }
                else
                {
                    rents.Functional = false;
                }
                return rents;

            }
        }


        //update request rent permission approved/rejected/cancelled/pending/inprogress
        public bool UpdateStatusOfRentRequest(int rid, int requestNo, string comment, string requestStatus, string viewName)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var allottee = (from app in dbContext.ApplicationDetails join alot in dbContext.AllotmentMasters on app.applicationId equals alot.applicationId where alot.rid == rid select app).FirstOrDefault();

                var request = dbContext.RentPermissionDetails.Where(r => r.Rid == rid && r.RequestNo == requestNo).FirstOrDefault();
                if (request != null)
                {
                    RentPermissionModel oldModel = new RentPermissionModel();
                    oldModel.StatusId = request.StatusId;
                    oldModel.Permission = request.Permission;

                    string emailMessage = string.Empty;
                    string mobileMessage = string.Empty;
                    EmailHelper emailHelper = new EmailHelper();
                    if (requestStatus.ToLower() == RequestStatus.Rejected.ToLower())
                    {
                        //request.StatusId = 2;
                        request.StatusId = StatusOption.Rejected;
                        request.Permission = false;
                        emailMessage = "Hi,<br><br>Rent Permission Request has been rejected of Registration id:" + rid + ".<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                        mobileMessage = string.Format(NAMessages.RentPermissionReqReject, rid);
                        emailHelper.Send(allottee.email, "Rent Permission Request Status", emailMessage);
                        //SMSSend(allottee.tMobileNumber, mobileMessage);
                        ApplicationHelper.SendSMS(allottee.tMobileNumber, mobileMessage);
                        RentPermissionModel newModel = new RentPermissionModel();
                        newModel.StatusId = StatusOption.Rejected;
                        newModel.Permission = false;
                        GeneralRepository.CreateAuditTrail(Constants.Update, Constants.RentPermissionDetails, viewName, request.RequestNo, oldModel, newModel, userInfo.UserID.ToString());
                    }
                    if (requestStatus.ToLower() == RequestStatus.Approved.ToLower())
                    {
                        //request.StatusId = 1;
                        request.StatusId = StatusOption.Approved;
                        request.Permission = true;
                        emailMessage = "Hi,<br><br>Rent Permission Request has been approved of Registration id:" + rid + ".<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                        mobileMessage = string.Format(NAMessages.RentPermissionReqApprove, rid);
                        emailHelper.Send(allottee.email, "Rent Permission Request Status", emailMessage);
                        //SMSSend(allottee.tMobileNumber, mobileMessage);
                        ApplicationHelper.SendSMS(allottee.tMobileNumber, mobileMessage);
                        RentPermissionModel newModel = new RentPermissionModel();
                        newModel.StatusId = StatusOption.Approved;
                        newModel.Permission = true;
                        GeneralRepository.CreateAuditTrail(Constants.Update, Constants.RentPermissionDetails, viewName, request.RequestNo, oldModel, newModel, userInfo.UserID.ToString());
                    }
                    if (requestStatus.ToLower() == RequestStatus.Cancelled.ToLower())
                    {
                        //request.Comment = comment;
                        request.StatusId = StatusOption.Cancelled;
                        request.Permission = false;
                        emailMessage = "Hi,<br><br>Rent Permission Request has been canceled of Registration id:" + rid + ".<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                        mobileMessage = string.Format(NAMessages.RentPermissionCancel, rid);
                        emailHelper.Send(allottee.email, "Rent Permission Request Status", emailMessage);
                        //SMSSend(allottee.tMobileNumber, mobileMessage);
                        ApplicationHelper.SendSMS(allottee.tMobileNumber, mobileMessage);
                        RentPermissionModel newModel = new RentPermissionModel();
                        newModel.StatusId = StatusOption.Cancelled;
                        newModel.Permission = false;
                        GeneralRepository.CreateAuditTrail(Constants.Update, Constants.RentPermissionDetails, viewName, request.RequestNo, oldModel, newModel, userInfo.UserID.ToString());
                    }
                    request.ApproveDate = DateTime.Now;
                    request.Comment = comment;
                    request.CommentDate = DateTime.Now;
                    dbContext.SaveChanges();

                    if (requestStatus.ToLower() == RequestStatus.Approved.ToLower())
                    {
                        if (request.OnlineRequestNo != 0)
                        {
                            UpdateServiceRequest(request.OnlineRequestNo, request.Rid, allottee.departmentId, comment);
                        }
                    }
                    flag = true;
                }
            }
            return flag;
        }

        //return challan details to generate rent permission challan for applicant
        public ChallanModel GetChallanDetailsForRentPermission(int rid, int requestNo)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var challanDetail = (from rent in dbContext.RentPermissionDetails
                                     join alot in dbContext.AllotmentMasters on rent.Rid equals alot.rid
                                     join scpt in dbContext.SchemePropTrans on alot.propertyId equals scpt.propertyId
                                     where rent.Rid == rid && rent.RequestNo == requestNo
                                     select new ChallanModel
                                     {
                                         RID = alot.rid,
                                         FormNo = alot.formNo,
                                         SectorName = scpt.SectorMst.sectorName,
                                         BlockName = scpt.BlockMst.blockName,
                                         PropertyTypeName = scpt.PropertyTypeMst.propertyTypeName,
                                         PropertyNumber = scpt.propertyNo,
                                         AllotmentMoney = scpt.allotmentMoney,
                                         ApplicationForm = new ApplicationFormModel
                                         {
                                             FirstName = alot.ApplicationDetail.tFirstName,
                                             MiddleName = alot.ApplicationDetail.tMiddleName,
                                             LastName = alot.ApplicationDetail.tLastName,
                                             CorrespondingAddress = alot.ApplicationDetail.tCorrespondanceAdd,
                                             MobileNumber = alot.ApplicationDetail.tMobileNumber,
                                             PhoneNumber = alot.ApplicationDetail.tPhoneNumber,
                                             Email = alot.ApplicationDetail.tEmail
                                         },
                                     }).FirstOrDefault();
                return challanDetail;
            }

        }

        public DataSourceResult GetDemandLetterDataByFilter(DataSourceRequest req, int? deptt, int? sector, int? block, int type)
        {
            //TODO:
            using (var dbContext = new NoidaPMSEntities())
            {
                var rslt = dbContext.Sp_PaymentDemand(type, deptt, sector, block).ToList();
                var m = from r in rslt
                        select
                            new DemandLetterModel
                            {
                                RId = r.Rid,
                                ApplicantName = r.Applicant_Name,
                                DepttName = r.Department,
                                SectorName = r.Sector,
                                Blockname = r.Block,
                                PropNo = r.Property_Number
                            };
                return m.ToDataSourceResult(req);
            }
        }

        public List<DDList> GetDemandLetterTypes()
        {
            var lst = new List<DDList>();
            var obj1 = new DDList();
            obj1.id = Constants.intPremium;
            obj1.text = Constants.premium;
            lst.Add(obj1);
            var obj2 = new DDList();
            obj2.id = Constants.intLeaseRent;
            obj2.text = Constants.leaseRent;
            lst.Add(obj2);
            return lst;
        }

        public RentPermissionModel GetModelToPrintRentPermissionLetter(int requestNo, int rid)
        {
            return GetRentRequestByRequestNumber(requestNo);
        }

        public List<RentPermissionModel> GetModelListToPrintRentPermissionLetter(List<int> requestNoList, List<int> ridList)
        {
            var modelList = new List<RentPermissionModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                modelList = (from rent in dbContext.RentPermissionDetails
                             join alot in dbContext.AllotmentMasters on rent.Rid equals alot.rid
                             join appl in dbContext.ApplicationDetails on alot.applicationId equals appl.applicationId
                             join dept in dbContext.DepartmentMsts on alot.departmentId equals dept.departmentId
                             join schm in dbContext.SchemeMsts on alot.schemeId equals schm.schemeId
                             join scpt in dbContext.SchemePropTrans on alot.propertyId equals scpt.propertyId
                             join flor in dbContext.FloorMsts on scpt.floorId equals flor.floorId
                             join prtt in dbContext.PropertyTypeMsts on scpt.propertyTypeId equals prtt.propertyTypeId
                             join sect in dbContext.SectorMsts on scpt.sectorId equals sect.sectorId
                             join blok in dbContext.BlockMsts on scpt.blockId equals blok.blockId
                             where requestNoList.Contains(rent.RequestNo)
                             select new RentPermissionModel
                             {
                                 RequestNo = rent.RequestNo,
                                 Rid = rent.Rid,
                                 TenantName = rent.TenantName,
                                 TenantProject = rent.TenantProject,
                                 RentDuration = rent.RentDurationYears,
                                 RentingCharge = rent.RentingCharge,
                                 RentingDate = rent.RentingDate,
                                 RentingEndDate = rent.RentingEndDate,
                                 StatusId = rent.StatusId,
                                 Comment = rent.Comment,
                                 TotalDues = 0,
                                 Functional = true,
                                 Department = dept.departmentName,
                                 AllotteeName = appl.firstName + " " + appl.middleName + " " + appl.lastName,
                                 FatherOrHusbandName = appl.fatherHusbandName,
                                 GenderOrCompany = appl.gender,
                                 TotalArea = scpt.totalArea.ToString(),
                                 FloorNo = flor.floorName,
                                 Block = blok.blockName,
                                 Sector = sect.sectorName,
                                 PropertyNo = scpt.propertyNo,
                                 PropertyType = prtt.propertyTypeName,

                                 RequestStatus = dbContext.StatusMasters.Where(s => s.Id == rent.StatusId).Select(s => s.Status).FirstOrDefault()
                             }).ToList();

            }
            return modelList;
        }

        /// <summary>
        /// To cancel Mortgage request.
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool CancelMortgageRequest(int requestID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.MortgageDetails.FirstOrDefault(m => m.RequestNo == requestID);
                var flag = false;
                if (result != null)
                {
                    result.StatusId = Constants.Cancelled;
                    result.ModifiedDate = DateTime.Now;
                    result.Modifiedby = userInfo.UserID;
                    dbContext.SaveChanges();
                    flag = true;
                }
                return flag;
            }
        }

        // To Invalida Mortgate
        public bool InValidMortgageRequestID(int requestNo, string Comment)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.MortgageDetails.FirstOrDefault(m => m.RequestNo == requestNo);
                var flag = false;
                if (result != null)
                {
                    result.Comment = Comment;
                    result.CommentDate = DateTime.Now;
                    result.PreviousLoanNoc = Constants.InValid;
                    dbContext.SaveChanges();
                    flag = true;
                }
                return flag;
            }
        }


        public string GenerateMortgageLetter(int rid)
        {
            string strLettter = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                var Objdeptt = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == rid);
                if (Objdeptt != null)
                {
                    //strLettter = CommonMethords.GenerateLetter(Objdeptt.rid, Constants.MortgageTemplateID, Objdeptt.departmentId.Value, userInfo.UserID);
                    strLettter = GenerateLetterFromDb(Objdeptt.rid, Constants.MortgageTemplateID, Objdeptt.departmentId.Value);
                }
            }
            return strLettter;
        }

        //To Generate Completion letter
        public string GenerateCompletionLetter(int rid)
        {
            string strLettter = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                var Objdeptt = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == rid);
                if (Objdeptt != null)
                {
                    strLettter = CommonMethords.GenerateLetter(Objdeptt.rid, Constants.CompletionTemplateID, Objdeptt.departmentId.Value, userInfo.UserID);
                }
            }
            return strLettter;
        }
        #endregion

        public string GenerateDemandLetters(int rId)
        {
            string strLettter = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                var Objdeptt = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == rId);
                if (Objdeptt != null)
                {
                    strLettter = CommonMethords.GenerateLetter(Objdeptt.rid, Constants.DemandLetterTemplate, Objdeptt.departmentId.Value, userInfo.UserID);
                    //strLettter = GenerateLetterFromDb(Objdeptt.rid, Constants.DemandLetterTemplate, Objdeptt.departmentId.Value); 
                }
            }
            return strLettter;
        }

        private string GenerateLetterFromDb(int registrationId, int templateId, int departmentId)
        {
            string strContent = string.Empty;
            if (userInfo != null)
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    ObjectParameter commaString = new ObjectParameter("CommaString", typeof(string));
                    dbContext.Sp_LatterPrintTemp(registrationId.ToString(), templateId, departmentId, userInfo.UserID.ToString(), null, commaString);
                    strContent = commaString.Value.ToString();
                }
            }

            return strContent;
        }

        public TransferModel GetTransfereeDetailByReqID(int Id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var allTransfers = (from trans in dbContext.Succ_Mut_Trans
                                    join allma in dbContext.AllotmentMasters on trans.Rid equals allma.rid
                                    join spt in dbContext.SchemePropTrans on allma.propertyId equals spt.propertyId
                                    join depma in dbContext.DepartmentMsts on spt.departmentId equals depma.departmentId
                                    join secma in dbContext.SectorMsts on spt.sectorId equals secma.sectorId
                                    join bloma in dbContext.BlockMsts on spt.blockId equals bloma.blockId
                                    join statusMas in dbContext.StatusMasters on trans.Status equals statusMas.Id
                                    join users in dbContext.UmUserMasters on trans.Approved_By equals users.UserRefId
                                    join reg in dbContext.RegistryDetails on allma.rid equals reg.Rid
                                    join pos in dbContext.PossessionDetails on trans.Rid equals pos.Rid
                                    where trans.Request_No == Id && loginUserDeptt.Contains(allma.departmentId)
                                    select new TransferModel
                                    {
                                        ReqNo = trans.Request_No,
                                        RId = trans.Rid,
                                        TransfereeGenderName = trans.T_Gender,
                                        TransfereeFirstName = trans.T_First_Name,
                                        TransfereeMiddleName = trans.T_Middle_Name,
                                        TransfereeLastName = trans.T_Last_Name,
                                        TransfereeRelationName = trans.T_Father_Husband_Name,
                                        TransfereeMotherName = trans.T_Mother_Name,
                                        TransfereeEmail = trans.T_Email,
                                        TransfereeCorrespondenceAdd = trans.T_Correspondence_Add,
                                        TransfereePermanentAdd = trans.T_Permanent_Add,
                                        TransfereePAN = trans.T_Pan,
                                        TransfereeOccupationId = trans.T_Occupation_Id,
                                        TransferType = trans.Transfer_Type.Value,
                                        TransferSubType = trans.Transfer_Sub_Type.Value,
                                        GPAHolderName = trans.GPA_Holder_Name,
                                        GPAHolderAdd = trans.GPA_Holder_Address,
                                        GPAEffectiveFrom = trans.GPA_Effective_From,
                                        GPAEffectiveTill = trans.GPA_Effective_To,
                                        TransferDate = trans.Transfer_Date,
                                        TransferChargePerSqMtr = trans.Transfer_Charge,
                                        TotalTransferCharge = trans.Total_Transfer_Charge,
                                        Status = statusMas.Status,
                                        Comment = trans.Comment,
                                        SubmittedDate = trans.Approved_Date,
                                        From = (from us in dbContext.UmUserMasters join tr in dbContext.Succ_Mut_Trans on us.UserRefId equals tr.Approved_By where tr.Request_No == Id select us.FirstName + " " + us.MiddleName + " " + us.LastName).FirstOrDefault(),
                                        TransfereeMobileNo = trans.T_Mobile,
                                        DepttId = spt.departmentId,
                                        LeaseDeedDate = reg.RegistryDoneDate,
                                        PossessionDate = pos.PossessionDate,
                                        isGPA = (trans.Transfer_Type == Constants.GPATransType) ? true : false,
                                        TransfereeCompanyEmail = trans.T_Email,
                                        TransfereeCompanyPAN = trans.T_Pan,
                                        TransfereeCompanyName = trans.T_Company_Name,
                                        TransfereeCompanyRegOff = trans.T_Correspondence_Add,
                                        TransfereeCompanySigningAuth = trans.T_Signing_Authority,
                                        PropNo = secma.sectorName + "/" + bloma.blockName + "-" + spt.propertyNo,
                                        TypeOfTransferee = (trans.T_Gender == Constants.genderCompany) ? 1 : 2, //1 for comapny and 2 for individual
                                        ReqRefNo = trans.OnlineRequestNo != null ? (int)trans.OnlineRequestNo : 0,
                                        CurrentPropertyRate = trans.CurrentPropertyRate,
                                        LocationCharge = trans.LocationCharge,
                                        AnnualLeaseRent = trans.AnnualLeaseRent,
                                        TotalPropertyCost = trans.TotalPropertyCost
                                    }).FirstOrDefault();
                //if (allTransfers != null)
                //{
                //    //If GPA exists for the given RID, only GPA type Transfer can take place
                //    var GPA = (from gpa in dbContext.GPAs where gpa.Rid == allTransfers.RId && EntityFunctions.TruncateTime(gpa.Effcetd_From) <= EntityFunctions.TruncateTime(DateTime.Now) && EntityFunctions.TruncateTime(gpa.Effected_To) >= EntityFunctions.TruncateTime(DateTime.Now) && gpa.Is_Active == true select gpa).FirstOrDefault();
                //    if (GPA != null)
                //        allTransfers.isGPA = true;
                //    else
                //        allTransfers.isGPA = false;
                //}
                return allTransfers;
            }
        }

        /// <summary>
        /// To cancel Transfer request.
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool CancelTransferRequest(int requestID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = dbContext.Succ_Mut_Trans.FirstOrDefault(m => m.Request_No == requestID);
                var flag = false;
                if (result != null)
                {
                    result.Status = Constants.Cancelled;
                    result.Modified_Date = DateTime.Now;
                    result.Modified_By = userInfo.UserID;
                    dbContext.SaveChanges();
                    flag = true;
                }
                return flag;
            }
        }
        #region Building Plan

        public DataSourceResult GetBuildingPlan(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var departmentList = (from dept in dbContext.UmDepartmentMasters
                                      join udts in dbContext.UmUserDepartmentTrans on dept.DepartmentId equals udts.DepartmentId
                                      where udts.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var buildingPlanDetail = (from buildingPlanMaster in dbContext.Building_Plan_Master
                                          join allotment in dbContext.AllotmentMasters on buildingPlanMaster.Rid equals allotment.rid
                                          join prop in dbContext.SchemePropTrans on allotment.propertyId equals prop.propertyId
                                          join dept in dbContext.DepartmentMsts on prop.departmentId equals dept.departmentId
                                          join sector in dbContext.SectorMsts on prop.sectorId equals sector.sectorId
                                          join block in dbContext.BlockMsts on prop.blockId equals block.blockId
                                          where buildingPlanMaster.Is_Active == true && departmentList.Contains(allotment.departmentId.Value)
                                          select new BuildingPlanModel
                                          {
                                              RId = buildingPlanMaster.Rid,
                                              BuildingPlanFileNo = buildingPlanMaster.BP_File_Name,
                                              DateOfSubmission = buildingPlanMaster.Submission_Date,
                                              MapRevised = buildingPlanMaster.Map_Revised,
                                              DepttName = dept.departmentName,
                                              SectorName = sector.sectorName,
                                              BlockName = block.blockName,
                                              PropertyNo = prop.propertyNo,
                                              PropNo = sector.sectorName + "/" + block.blockName + "-" + prop.propertyNo,
                                          }).ToList();

                return buildingPlanDetail.ToDataSourceResult(request);
            }
        }

        public BuildingPlanModel GetBuildingPlanDetailsByRId(int rId)
        {
            BuildingPlanModel objBuildingPlan = new BuildingPlanModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                objBuildingPlan = (from posDetail in dbContext.PossessionDetails
                                   join allma in dbContext.AllotmentMasters on posDetail.Rid equals allma.rid
                                   join appDetails in dbContext.ApplicationDetails on allma.rid equals appDetails.registrationId
                                   join prop in dbContext.SchemePropTrans on allma.propertyId equals prop.propertyId
                                   join sector in dbContext.SectorMsts on prop.sectorId equals sector.sectorId
                                   join block in dbContext.BlockMsts on prop.blockId equals block.blockId
                                   join floorMst in dbContext.FloorMsts on prop.floorId equals floorMst.floorId
                                   join propMaster in dbContext.PropertyTypeMsts on prop.propertyTypeId equals propMaster.propertyTypeId
                                   join buildingPlan in dbContext.Building_Plan_Master on allma.rid equals buildingPlan.Rid
                                          into bp
                                   from p in bp.DefaultIfEmpty()
                                   where posDetail.Rid == rId && loginUserDeptt.Contains(allma.departmentId)
                                   select new BuildingPlanModel
                                   {
                                       RId = rId,
                                       Location_East = posDetail.East,
                                       Location_West = posDetail.West,
                                       Location_North = posDetail.North,
                                       Location_South = posDetail.South,
                                       PropertyType = propMaster.propertyTypeName,
                                       AllotteeName = appDetails.firstName + " " + appDetails.lastName,
                                       BuildingPlanFileNo = p.BP_File_Name,
                                       DateOfSubmission = p.Submission_Date,
                                       DateOfSanction = p.Sanction_Date,
                                       PurchasableFar = p.Purchasable_Far_Percent,
                                       PloatArea = p.Plot_Area,
                                       CoveredArea = p.Covered_Area,
                                       FloorAreasRation = p.FAR,
                                       ArchitectRegNo = p.Architect_Reg_No,
                                       NameOfArchitect = p.Architect_Name,
                                       PropertyUse = p.Property_Use,
                                       PropNo = sector.sectorName + "/" + block.blockName + "-" + prop.propertyNo,
                                       MapReleased = p.Map_Released,
                                       NumberOfStories = p.Max_No_Stories.Value,
                                       MapRevised = p.Map_Revised,
                                       SanctionPlanValidity = p.Sanctioned_Plan_Validity
                                   }).FirstOrDefault();
                //objBuildingPlan.RId = rId;
                return objBuildingPlan;
            }
        }
        public DataSourceResult GetBuildingDetail(DataSourceRequest request, int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var buildingPlanDetail = (from buildingPlanTrans in dbContext.Building_Plan_Trans
                                          where buildingPlanTrans.Rid == rId && buildingPlanTrans.Is_Active == true && buildingPlanTrans.Record_Type == 1
                                          select new BuildingPlanModel
                                          {
                                              RId = buildingPlanTrans.Rid,
                                              NameOfBuilding = buildingPlanTrans.Tower_Name,
                                              TotalCoveredAreaAllFloor = buildingPlanTrans.All_Covered_Area,
                                              TotalCoveredAreaGroundFloor = buildingPlanTrans.GF_Covered_Area,
                                              BuildingHeight = buildingPlanTrans.Building_Height,
                                              NoOfStories = buildingPlanTrans.No_Of_stories,
                                              RequestNo = buildingPlanTrans.id
                                          }).ToList();

                return buildingPlanDetail.ToDataSourceResult(request);
            }
        }
        public DataSourceResult GetChargesDemanded(DataSourceRequest request, int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var buildingPlanDetail = (from buildingPlanTrans in dbContext.Building_Plan_Trans
                                          //join rdm in dbContext.RECEIPT_DETAIL_MASTER on buildingPlanTrans.Rid.ToString() equals rdm.RID_NO
                                          //join rat in dbContext.RECEIPT_AMOUNT_TRANS on buildingPlanTrans.Receipt_Head equals rat.RECEIPT_HEAD_ID
                                          join rsh in dbContext.RECEIPT_SUB_HEAD on buildingPlanTrans.Receipt_Sub_Head equals rsh.RECEIPT_SUBHEAD_ID
                                          where buildingPlanTrans.Rid == rId && buildingPlanTrans.Is_Active == true && buildingPlanTrans.Record_Type == 2 //&& buildingPlanTrans.Receipt_Sub_Head == rat.RECEIPT_SUBHEAD_ID && rdm.RECEIPT_ID == rat.RECEIPT_ID && buildingPlanTrans.Amount == rat.AMOUNT_PAID
                                          select new BuildingPlanModel
                                          {
                                              ChargesName = rsh.RECEIPT_SUB_HEAD1,//rsh.RECEIPT_SUB_HEAD1,//rsh.RECEIPT_SUB_HEAD//"Test Charges",//ToDo
                                              FeeAmount = buildingPlanTrans.Amount,
                                              Status = "Paid",//rat.ID == 0 ? "Not Paid" : "Paid",//ToDo
                                              RequestNo = buildingPlanTrans.id
                                          }).ToList();

                return buildingPlanDetail.ToDataSourceResult(request);
            }
        }
        public DataSourceResult GetNOCDetail(DataSourceRequest request, int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var buildingPlanDetail = (from buildingPlanTrans in dbContext.Building_Plan_Trans
                                          join noc in dbContext.NOC_MASTER on buildingPlanTrans.Noc_Type equals noc.id
                                          where buildingPlanTrans.Rid == rId && buildingPlanTrans.Is_Active == true && buildingPlanTrans.Record_Type == 3
                                          select new BuildingPlanModel
                                          {
                                              RId = buildingPlanTrans.Rid,
                                              NOC = noc.NOC_Type,
                                              RequestNo = buildingPlanTrans.id,
                                              SubmitStatus = buildingPlanTrans.Is_Submit
                                          }).ToList();

                return buildingPlanDetail.ToDataSourceResult(request);
            }
        }

        public BuildingPlanModel GetBuildingPlanByRId(int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var buildingPlanDetail = (from posDetail in dbContext.PossessionDetails
                                          join allma in dbContext.AllotmentMasters on posDetail.Rid equals allma.rid
                                          join appDetails in dbContext.ApplicationDetails on allma.rid equals appDetails.registrationId
                                          join prop in dbContext.SchemePropTrans on allma.propertyId equals prop.propertyId
                                          join scmTr in dbContext.SchemeDepartmentTrans on allma.schemeId equals scmTr.schemeId
                                          join scmTr1 in dbContext.SchemeDepartmentTrans on allma.departmentId equals scmTr1.departmentId
                                          join sm in dbContext.SectorMsts on prop.sectorId equals sm.sectorId
                                          join bm in dbContext.BlockMsts on prop.blockId equals bm.blockId
                                          join floorMst in dbContext.FloorMsts on prop.floorId equals floorMst.floorId
                                          join propMaster in dbContext.PropertyTypeMsts on prop.propertyTypeId equals propMaster.propertyTypeId
                                          join buildingPlan in dbContext.Building_Plan_Master on allma.rid equals buildingPlan.Rid
                                          into bp
                                          from p in bp.DefaultIfEmpty()
                                          where posDetail.Rid == rId && loginUserDeptt.Contains(allma.departmentId)
                                          select new BuildingPlanModel
                                          {
                                              Location_East = posDetail.East,
                                              Location_West = posDetail.West,
                                              Location_North = posDetail.North,
                                              Location_South = posDetail.South,
                                              PropertyType = propMaster.propertyTypeName,
                                              AllotteeName = appDetails.tFirstName + " " + appDetails.tLastName,
                                              BuildingPlanFileNo = p.BP_File_Name,
                                              PloatArea = prop.totalArea,
                                              PropertyNumber = sm.sectorName + "/" + bm.blockName + "-" + prop.propertyNo,
                                              CoveredArea = prop.coveredArea,
                                              FloorAreasRation = scmTr.far
                                          }).FirstOrDefault();

                return buildingPlanDetail;
            }
        }
        public int SaveBuildingPlan1(int rId, string BuildingPlanFileNo, int? PropertyUse, DateTime? DateOfSubmission, DateTime? DateOfSanction, decimal? PurchasableFar, DateTime? SanctionPlanValidity, string MapReleased, string MapRevised, decimal? PloatArea, decimal? CoveredArea, double? FloorAreasRation, int NumberOfStories, string ArchitectRegNo, string NameOfArchitect)
        {
            int flag = 0;
            using (var dbContext = new NoidaPMSEntities())
            {

                var existingRId = (from bpm in dbContext.Building_Plan_Master where bpm.Rid == rId && bpm.Is_Active == true select bpm).FirstOrDefault();
                var existingBpfn = (from bpm in dbContext.Building_Plan_Master where bpm.BP_File_Name == BuildingPlanFileNo && bpm.Is_Active == true select bpm).FirstOrDefault();
                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == rId select appDet).FirstOrDefault();//For notification details
                if (existingBpfn != null)
                {
                    flag = 3;
                    return flag;
                }
                //  var objUserList = dbContext.UmUserMasters.Where(i => i.UserName == user).FirstOrDefault();
                if (existingRId == null && existingBpfn == null)
                {
                    Building_Plan_Master objfun = new Building_Plan_Master();
                    objfun.Rid = rId;
                    objfun.BP_File_Name = BuildingPlanFileNo;
                    objfun.Property_Use = PropertyUse;
                    objfun.Submission_Date = DateOfSubmission;
                    objfun.Sanction_Date = DateOfSanction;
                    objfun.Purchasable_Far_Percent = PurchasableFar;
                    objfun.Sanctioned_Plan_Validity = SanctionPlanValidity;
                    //objfun.StatusId = (from mut in dbContext.StatusMasters where mut.Id == 5 select mut.Id).FirstOrDefault();
                    objfun.Map_Released = MapReleased;
                    objfun.Map_Revised = MapRevised;
                    objfun.Plot_Area = PloatArea;
                    objfun.Covered_Area = CoveredArea;
                    objfun.FAR = FloorAreasRation;
                    objfun.Max_No_Stories = NumberOfStories;
                    objfun.Architect_Reg_No = ArchitectRegNo;
                    objfun.Architect_Name = NameOfArchitect;


                    objfun.Is_Completed = false;
                    objfun.Created_Date = DateTime.Now;
                    objfun.Created_By = userInfo.UserID;
                    objfun.Is_Active = true;
                    dbContext.Building_Plan_Master.Add(objfun);
                    dbContext.SaveChanges();

                    flag = 1;
                    //var body = "Hi,<br><br>Your Functional Request has been saved.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                    //EmailHelper emailHelper = new EmailHelper();
                    //emailHelper.Send(newOwner.tEmail, "Functional Request Status", body);
                    ////SMS
                    //var msg = "Your Functional Request has been saved.";
                    //SMSSend(newOwner.tMobileNumber, msg);
                }
                else
                {
                    flag = 2;
                    //  existingRequest.Rid = rId;

                }
                return flag;
            }
            //  return _propertyRegistrationRepository.SaveBuildingPlan1(rId, BuildingPlanFileNo, PropertyUse, DateOfSubmission, DateOfSanction, PurchasableFar, SanctionPlanValidity, MapReleased, MapRevised, PloatArea, CoveredArea, FloorAreasRation, NumberOfStories, ArchitectRegNo, NameOfArchitect);
        }
        //Get property type for rent permission only industrial or institutional
        public List<DDList> GetChargesType()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var propType = (from ptm in dbContext.RECEIPT_SUB_HEAD
                                where ptm.STATUS == 1
                                select new DDList
                                {
                                    id = ptm.RECEIPT_SUBHEAD_ID,
                                    text = ptm.RECEIPT_SUB_HEAD1
                                }).ToList();
                return propType;
            }
            //throw new NotImplementedException();
        }
        //Get property type for rent permission only industrial or institutional
        public List<DDList> GetNOCType()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var propType = (from ptm in dbContext.NOC_MASTER
                                where ptm.Is_Active == true
                                select new DDList
                                {
                                    id = ptm.id,
                                    text = ptm.NOC_Type
                                }).ToList();
                return propType;
            }
            //throw new NotImplementedException();
        }

        public int SaveBuildingDetails(int rId, string NameOfTower, decimal? CoveredAreaMultiFloors, decimal? BuildingHeight, decimal? CoveredAreaGroundFloor, int? NumberOfStories2)
        {
            int flag = 0;
            using (var dbContext = new NoidaPMSEntities())
            {

                var existingRId = (from bpm in dbContext.Building_Plan_Master where bpm.Rid == rId && bpm.Is_Active == true select bpm).FirstOrDefault();
                // var existingBpfn = (from bpm in dbContext.Building_Plan_Master where bpm.Rid == existingRId.BP_File_Name  && bpm.Is_Active == true select bpm).FirstOrDefault();
                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == rId select appDet).FirstOrDefault();//For notification details

                var existingBuildingDetails = (from bpm in dbContext.Building_Plan_Trans where bpm.Rid == rId && bpm.Record_Type == 1 && bpm.Tower_Name == NameOfTower && bpm.Is_Active == true select bpm).FirstOrDefault();
                //  var objUserList = dbContext.UmUserMasters.Where(i => i.UserName == user).FirstOrDefault();
                if (NumberOfStories2 > existingRId.Max_No_Stories)
                {
                    flag = 3;
                    return flag;
                }
                if (existingBuildingDetails == null)
                {
                    Building_Plan_Trans objfun = new Building_Plan_Trans();
                    objfun.Rid = rId;
                    objfun.BPid = existingRId.id;
                    objfun.Tower_Name = NameOfTower;
                    objfun.All_Covered_Area = CoveredAreaMultiFloors;
                    objfun.Building_Height = BuildingHeight;
                    objfun.GF_Covered_Area = CoveredAreaGroundFloor;
                    objfun.No_Of_stories = NumberOfStories2;
                    objfun.Record_Type = 1;
                    objfun.Created_Date = DateTime.Now;
                    objfun.Created_By = userInfo.UserID;
                    objfun.Is_Active = true;
                    dbContext.Building_Plan_Trans.Add(objfun);
                    dbContext.SaveChanges();

                    flag = 1;

                }
                else
                {
                    flag = 2;

                }
                return flag;
            }
        }
        public int SaveBuildingCharge(int rId, int Charges, decimal? FeeAmount)
        {
            int flag = 0;
            using (var dbContext = new NoidaPMSEntities())
            {

                var existingRId = (from bpm in dbContext.Building_Plan_Master where bpm.Rid == rId && bpm.Is_Active == true select bpm).FirstOrDefault();
                // var existingBpfn = (from bpm in dbContext.Building_Plan_Master where bpm.Rid == existingRId.BP_File_Name  && bpm.Is_Active == true select bpm).FirstOrDefault();
                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == rId select appDet).FirstOrDefault();//For notification details

                var existingBuildingDetails = (from bpm in dbContext.Building_Plan_Trans where bpm.Rid == rId && bpm.Record_Type == 2 && bpm.Receipt_Sub_Head == Charges && bpm.Is_Active == true select bpm).FirstOrDefault();
                //  var objUserList = dbContext.UmUserMasters.Where(i => i.UserName == user).FirstOrDefault();
                if (existingBuildingDetails == null)
                {
                    Building_Plan_Trans objfun = new Building_Plan_Trans();
                    objfun.Rid = rId;
                    objfun.BPid = existingRId.id;
                    objfun.Receipt_Sub_Head = Charges;
                    objfun.Amount = FeeAmount;
                    objfun.Record_Type = 2;

                    objfun.Created_Date = DateTime.Now;
                    objfun.Created_By = userInfo.UserID;
                    objfun.Is_Active = true;
                    dbContext.Building_Plan_Trans.Add(objfun);
                    dbContext.SaveChanges();

                    flag = 1;

                }
                else
                {
                    flag = 2;

                }
                return flag;
            }
        }

        public int SaveNocType(int rId, int NocType, string Submitted)
        {
            int flag = 0;
            using (var dbContext = new NoidaPMSEntities())
            {

                var existingRId = (from bpm in dbContext.Building_Plan_Master where bpm.Rid == rId && bpm.Is_Active == true select bpm).FirstOrDefault();
                // var existingBpfn = (from bpm in dbContext.Building_Plan_Master where bpm.Rid == existingRId.BP_File_Name  && bpm.Is_Active == true select bpm).FirstOrDefault();
                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == rId select appDet).FirstOrDefault();//For notification details

                var existingBuildingDetails = (from bpm in dbContext.Building_Plan_Trans where bpm.Rid == rId && bpm.Record_Type == 3 && bpm.Noc_Type == NocType && bpm.Is_Active == true select bpm).FirstOrDefault();
                //  var objUserList = dbContext.UmUserMasters.Where(i => i.UserName == user).FirstOrDefault();
                if (existingBuildingDetails == null)
                {
                    Building_Plan_Trans objfun = new Building_Plan_Trans();
                    objfun.Rid = rId;
                    objfun.BPid = existingRId.id;
                    objfun.Noc_Type = NocType;
                    objfun.Is_Submit = Submitted;
                    objfun.Record_Type = 3;

                    objfun.Created_Date = DateTime.Now;
                    objfun.Created_By = userInfo.UserID;
                    objfun.Is_Active = true;
                    dbContext.Building_Plan_Trans.Add(objfun);
                    dbContext.SaveChanges();

                    flag = 1;

                }
                else
                {
                    flag = 2;

                }
                return flag;
            }
        }

        public bool RemoveBuildingDetails(int RequestNo, int rId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var refnd = dbContext.Building_Plan_Trans.FirstOrDefault(cond => cond.id == RequestNo && cond.Rid == rId && cond.Is_Active == true);
                if (refnd != null)
                {

                    refnd.Is_Active = false;
                    refnd.Modified_By = userInfo.UserID;
                    refnd.Modified_date = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = true;
                }
                var schemeDepartmentTranExit = dbContext.Building_Plan_Trans.Where(d => d.Rid == rId && d.Record_Type == 1 && d.Is_Active == true).ToList();
                if (schemeDepartmentTranExit.Count == 0)
                {
                    var schemeComitedUpdate = dbContext.Building_Plan_Master.Where(d => d.Rid == rId && d.Is_Active == true).FirstOrDefault();
                    if (schemeComitedUpdate != null)
                    {
                        schemeComitedUpdate.Is_Completed = false;
                        dbContext.SaveChanges();
                    }
                }
            }
            return flag;
        }

        public bool RemoveBPCharges(int RequestNo, int rId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var refnd = dbContext.Building_Plan_Trans.FirstOrDefault(cond => cond.id == RequestNo && cond.Rid == rId && cond.Is_Active == true);
                refnd.Is_Active = false;
                refnd.Modified_By = userInfo.UserID;
                refnd.Modified_date = DateTime.Now;
                dbContext.SaveChanges();
                flag = true;

                var schemeDepartmentTranExit = dbContext.Building_Plan_Trans.Where(d => d.Rid == rId && d.Record_Type == 2 && d.Is_Active == true).ToList();
                if (schemeDepartmentTranExit.Count == 0)
                {
                    var schemeComitedUpdate = dbContext.Building_Plan_Master.Where(d => d.Rid == rId && d.Is_Active == true).FirstOrDefault();
                    if (schemeComitedUpdate != null)
                    {
                        schemeComitedUpdate.Is_Completed = false;
                        dbContext.SaveChanges();
                    }
                }
            }
            return flag;
        }
        public bool RemoveNocType(int RequestNo, int rId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var refnd = dbContext.Building_Plan_Trans.FirstOrDefault(cond => cond.id == RequestNo && cond.Rid == rId && cond.Is_Active == true);
                refnd.Is_Active = false;
                refnd.Modified_By = userInfo.UserID;
                refnd.Modified_date = DateTime.Now;
                dbContext.SaveChanges();
                flag = true;
                var schemeDepartmentTranExit = dbContext.Building_Plan_Trans.Where(d => d.Rid == rId && d.Record_Type == 3 && d.Is_Active == true).ToList();
                if (schemeDepartmentTranExit.Count == 0)
                {
                    var schemeComitedUpdate = dbContext.Building_Plan_Master.Where(d => d.Rid == rId && d.Is_Active == true).FirstOrDefault();
                    if (schemeComitedUpdate != null)
                    {
                        schemeComitedUpdate.Is_Completed = false;
                        dbContext.SaveChanges();
                    }
                }
            }
            return flag;
        }
        public int SubmitBuildingPlan(int rId)
        {
            int flag = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                var objBPM = dbContext.Building_Plan_Master.FirstOrDefault(cond => cond.Rid == rId && cond.Is_Active == true);
                var objBPM1 = dbContext.Building_Plan_Trans.Where(cond => cond.Rid == rId && cond.Is_Active == true && cond.Record_Type == 1).ToList();
                var objBPM2 = dbContext.Building_Plan_Trans.Where(cond => cond.Rid == rId && cond.Is_Active == true && cond.Record_Type == 2).ToList(); ;
                var objBPM3 = dbContext.Building_Plan_Trans.Where(cond => cond.Rid == rId && cond.Is_Active == true && cond.Record_Type == 3).ToList(); ;
                //var exitTran=dbContext .Building_Plan_Trans .Where ()
                // var refnd = dbContext.Building_Plan_Trans.FirstOrDefault(cond => cond.id == RequestNo && cond.Rid == rId);
                if (objBPM != null && objBPM1.Count > 0 && objBPM2.Count > 0 && objBPM3.Count > 0)
                {
                    objBPM.Is_Active = true;
                    objBPM.Modified_By = userInfo.UserID;
                    objBPM.Modified_date = DateTime.Now;
                    objBPM.Is_Completed = true;
                    dbContext.SaveChanges();
                    flag = 1;
                }
            }
            return flag;
        }
        //To update buliding plan
        public int UpdateBuildingPlan1(int rId, string BuildingPlanFileNo, int? PropertyUse, DateTime? DateOfSubmission, DateTime? DateOfSanction, decimal? PurchasableFar, DateTime? SanctionPlanValidity, string MapReleased, string MapRevised, decimal? PloatArea, decimal? CoveredArea, double? FloorAreasRation, int NumberOfStories, string ArchitectRegNo, string NameOfArchitect)
        {
            int flag = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                var resultBP = dbContext.Building_Plan_Master.FirstOrDefault(m => m.Rid == rId);
                var existingBpfn = (from bpm in dbContext.Building_Plan_Master where bpm.BP_File_Name == BuildingPlanFileNo && bpm.Rid != rId && bpm.Is_Active == true select bpm).FirstOrDefault();
                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.registrationId == rId select appDet).FirstOrDefault();//For notification details
                if (existingBpfn != null)
                {
                    flag = 3;
                    return flag;
                }
                if (resultBP != null)
                {
                    resultBP.BP_File_Name = BuildingPlanFileNo;
                    resultBP.Property_Use = PropertyUse;
                    resultBP.Submission_Date = DateOfSubmission;
                    resultBP.Sanction_Date = DateOfSanction;
                    resultBP.Purchasable_Far_Percent = PurchasableFar;
                    resultBP.Sanctioned_Plan_Validity = SanctionPlanValidity;
                    resultBP.Map_Released = MapReleased;
                    resultBP.Map_Revised = MapRevised;
                    resultBP.Plot_Area = PloatArea;
                    resultBP.Covered_Area = CoveredArea;
                    resultBP.FAR = FloorAreasRation;
                    resultBP.Max_No_Stories = NumberOfStories;
                    resultBP.Architect_Reg_No = ArchitectRegNo;
                    resultBP.Architect_Name = NameOfArchitect;
                    resultBP.Is_Completed = false;
                    resultBP.Created_Date = DateTime.Now;
                    resultBP.Created_By = userInfo.UserID;
                    resultBP.Is_Active = true;
                    dbContext.SaveChanges();
                    flag = 1;
                }
                return flag;
            }
        }

        //Get •	Property Use
        public List<DDList> GetPropertyUse()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var propType = (from ptm in dbContext.PropertyTypeMsts
                                where ptm.IsActive == true
                                select new DDList
                                {
                                    id = ptm.propertyTypeId,
                                    text = ptm.propertyTypeName
                                }).ToList();
                return propType;
            }
            //throw new NotImplementedException();
        }

        #endregion

        #region GPA

        public DataSourceResult GetAllGPAData(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var allGPA = (from gpa in dbContext.GPAs
                              join alloMas in dbContext.AllotmentMasters on gpa.Rid equals alloMas.rid
                              join spt in dbContext.SchemePropTrans on alloMas.propertyId equals spt.propertyId
                              join dept in dbContext.DepartmentMsts on alloMas.departmentId equals dept.departmentId
                              join secma in dbContext.SectorMsts on spt.sectorId equals secma.sectorId
                              join bloma in dbContext.BlockMsts on spt.blockId equals bloma.blockId
                              where gpa.Is_Active == true && loginUserDeptt.Contains(alloMas.departmentId) && alloMas.isActive == 1
                              select new GPAModel
                              {
                                  RId = gpa.Rid,
                                  GPAId = gpa.Id,
                                  DepttName = dept.departmentName,
                                  SectorName = secma.sectorName,
                                  BlockName = bloma.blockName,
                                  PropertyNo = spt.propertyNo,
                                  PropNo = secma.sectorName + "/" + bloma.blockName + "-" + spt.propertyNo,
                                  EffectiveFrom = gpa.Effcetd_From,
                                  EffectiveTo = gpa.Effected_To,
                                  IsActive = gpa.Is_Active,
                                  GPAType = Common.GPAType.GPA.ToString(),
                                  CreatedDate = gpa.Created_Date
                              });
                var nominees = (from nom in dbContext.Nominee_Details
                                join alloMas in dbContext.AllotmentMasters on nom.Rid equals alloMas.rid
                                join spt in dbContext.SchemePropTrans on alloMas.propertyId equals spt.propertyId
                                join dept in dbContext.DepartmentMsts on alloMas.departmentId equals dept.departmentId
                                join secma in dbContext.SectorMsts on spt.sectorId equals secma.sectorId
                                join bloma in dbContext.BlockMsts on spt.blockId equals bloma.blockId
                                where nom.Is_Active == 1 && alloMas.isActive == 1 && loginUserDeptt.Contains(alloMas.departmentId)
                                select new GPAModel
                                {
                                    RId = nom.Rid,
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
                //var rslt = allGPA.Union(distinctNominees);
                var rslt = allGPA.AsEnumerable().Concat(distinctNominees);
                rslt = (from g in rslt orderby g.CreatedDate select g);
                return rslt.ToDataSourceResult(req);
            }
        }

        /// <summary>
        /// To Activate/Deactivate GPA record
        /// </summary>
        /// <param name="id">GPA ID</param>
        /// <param name="isActive">Current status</param>
        /// <returns></returns>
        public bool ActivateDeactivateToggle(int id, bool isActive)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var GPARecord = (from gpa in dbContext.GPAs where gpa.Id == id select gpa).FirstOrDefault();
                if (GPARecord != null)
                {
                    if (isActive == false)
                    {
                        GPARecord.Is_Active = true;
                    }
                    else
                        GPARecord.Is_Active = false;
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// Used for saving GPA details in DB
        /// </summary>
        /// <param name="GPAMod">Model object with GPA details</param>
        /// <returns></returns>
        public bool SaveGPANominee(GPAModel GPAMod)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                GPAMod.GPARegistered = 1;//always select gpa registered to yes - 14 July 2017
                if (GPAMod.GPAId != 0)
                {
                    if (GPAMod.hdnGPAType.ToLower() == Common.GPAType.GPA.ToString().ToLower())
                    {
                        var existingGPA = (from g in dbContext.GPAs where g.Id == GPAMod.GPAId select g).FirstOrDefault();
                        if (existingGPA != null)
                        {
                            existingGPA.GPA_Holder_Name = GPAMod.GPAHolderName;
                            existingGPA.GPA_Holder_Address = GPAMod.GPAHolderAdd;
                            existingGPA.Relation = GPAMod.RelationName;
                            existingGPA.Application_Date = GPAMod.ApplicationDate;
                            existingGPA.Acceptance_date = GPAMod.AcceptanceDate;
                            existingGPA.Effcetd_From = GPAMod.EffectiveFrom;
                            existingGPA.Effected_To = GPAMod.EffectiveTo;
                            existingGPA.Registered = GPAMod.GPARegistered == 1 ? true : false;
                            if (GPAMod.GPARegistered == MortPrevLoan.yes)
                            {
                                existingGPA.Registration_No = GPAMod.GPARegisteredNo;
                            }

                            //fields added on  14 July 2017                     
                            existingGPA.BahiNo = GPAMod.BahiNo;
                            existingGPA.BahiZildNo = GPAMod.BahiZildNo;
                            existingGPA.BahiPageNo = GPAMod.BahiPageNo;
                            existingGPA.BahiSINo = GPAMod.SINo;
                            existingGPA.GPASOA = GPAMod.GPASOA != null ? (GPAMod.GPASOA == 1 ? @NA.PMS.Common.GPAOption.GPASOAYes.ToString() : @NA.PMS.Common.GPAOption.GPASOANo.ToString()) : string.Empty;

                            if (GPAMod.GPASOA == 1)
                            {
                                existingGPA.AgreementType = GPAMod.AgreementType != null ? (GPAMod.AgreementType == 1 ? @NA.PMS.Common.GPAOption.AgreementTypeReg.ToString() : @NA.PMS.Common.GPAOption.AgreementTypeUnReg.ToString()) : string.Empty;
                                if (GPAMod.AgreementType == 1)
                                {
                                    existingGPA.RegistrationDate = GPAMod.RegistrationDate;
                                    existingGPA.SOABahiNo = GPAMod.SOABahiNo;
                                    existingGPA.SOABahiZildNo = GPAMod.SOABahiZildNo;
                                    existingGPA.SOABahiPageNo = GPAMod.SOABahiPageNo;
                                    existingGPA.SOASINo = GPAMod.SOASINo;
                                }
                                existingGPA.AdvertismentInNewsPaper = GPAMod.AdvertismentInNewsPaper;
                                existingGPA.PublishedDate = GPAMod.PublishedDate;
                            }
                            existingGPA.Modified_By = userInfo.UserID;
                            existingGPA.Modified_date = DateTime.Now;
                            dbContext.SaveChanges();
                            flag = true;
                        }
                    }
                }
                else //Add new GPA
                {
                    if (GPAMod.GPAType.ToLower() == Common.GPAType.GPA.ToString().ToLower())
                    {
                        var existingGPA = (from g in dbContext.GPAs where g.Rid == GPAMod.RId && g.Is_Active == true select g).FirstOrDefault();
                        if (existingGPA != null)
                        {
                            existingGPA.Is_Active = false;
                            existingGPA.Modified_By = userInfo.UserID;
                            existingGPA.Modified_date = DateTime.Now;
                            //dbContext.SaveChanges();
                        }
                        var newGPA = new GPA();
                        newGPA.GPA_Holder_Name = GPAMod.GPAHolderName;
                        newGPA.GPA_Holder_Address = GPAMod.GPAHolderAdd;
                        newGPA.Relation = GPAMod.RelationName;
                        newGPA.Application_Date = GPAMod.ApplicationDate;
                        newGPA.Acceptance_date = GPAMod.AcceptanceDate;
                        newGPA.Effcetd_From = GPAMod.EffectiveFrom;
                        newGPA.Effected_To = GPAMod.EffectiveTo;
                        newGPA.Registered = GPAMod.GPARegistered == 1 ? true : false;
                        newGPA.Rid = GPAMod.RId;
                        if (GPAMod.GPARegistered == 1)
                        {
                            newGPA.Registration_No = GPAMod.GPARegisteredNo;
                        }
                        newGPA.Created_By = userInfo.UserID;
                        newGPA.Created_Date = DateTime.Now;
                        //newGPA.Rid = GPAMod.hdnRId;
                        newGPA.Is_Active = true;

                        //fields added on  14 July 2017      
                        newGPA.BahiNo = GPAMod.BahiNo;
                        newGPA.BahiZildNo = GPAMod.BahiZildNo;
                        newGPA.BahiPageNo = GPAMod.BahiPageNo;
                        newGPA.BahiSINo = GPAMod.SINo;
                        newGPA.GPASOA = GPAMod.GPASOA != null ? (GPAMod.GPASOA == 1 ? @NA.PMS.Common.GPAOption.GPASOAYes.ToString() : @NA.PMS.Common.GPAOption.GPASOANo.ToString()) : string.Empty;
                        if (GPAMod.GPASOA == 1)
                        {
                            newGPA.AgreementType = GPAMod.AgreementType != null ? (GPAMod.AgreementType == 1 ? @NA.PMS.Common.GPAOption.AgreementTypeReg.ToString() : @NA.PMS.Common.GPAOption.AgreementTypeUnReg.ToString()) : string.Empty;
                            if (GPAMod.AgreementType == 1)
                            {
                                newGPA.RegistrationDate = GPAMod.RegistrationDate;
                                newGPA.SOABahiNo = GPAMod.SOABahiNo;
                                newGPA.SOABahiZildNo = GPAMod.SOABahiZildNo;
                                newGPA.SOABahiPageNo = GPAMod.SOABahiPageNo;
                                newGPA.SOASINo = GPAMod.SOASINo;
                            }
                            if (GPAMod.AgreementType == 2)
                            {
                                newGPA.AdvertismentInNewsPaper = GPAMod.AdvertismentInNewsPaper;
                                newGPA.PublishedDate = GPAMod.PublishedDate;
                            }
                        }

                        dbContext.GPAs.Add(newGPA);
                        dbContext.SaveChanges();
                        flag = true;
                    }
                }
                return flag;
            }
        }

        /// <summary>
        /// DDL read
        /// </summary>
        /// <returns></returns>
        public List<DDLStringList> GetGPAType(string company)
        {
            var lst = new List<DDLStringList>();
            using (var dbContext = new NoidaPMSEntities())
            {
                int rid = Convert.ToInt32(company);
                var detail = (from allotment in dbContext.AllotmentMasters where allotment.rid == rid select allotment.ApplicationDetail.tGender).FirstOrDefault();
                if (detail != null)
                {
                    if (detail.ToLower() == Constants.Company.ToLower())
                    {
                        var opt1 = new DDLStringList();
                        opt1.text = Common.GPAType.GPA.ToString();
                        opt1.id = Common.GPAType.GPA.ToString();
                        lst.Add(opt1);
                        return lst;
                    }
                    else
                    {
                        var opt1 = new DDLStringList();
                        opt1.text = Common.GPAType.GPA.ToString();
                        opt1.id = Common.GPAType.GPA.ToString();
                        var opt2 = new DDLStringList();
                        opt2.text = Common.GPAType.Nominee.ToString();
                        opt2.id = Common.GPAType.Nominee.ToString();
                        lst.Add(opt1);
                        lst.Add(opt2);
                        return lst;
                    }
                }
                else
                {
                    return lst = null;
                }

            }
            //var lst = new List<DDLStringList>();
            //var opt1 = new DDLStringList();
            //opt1.text = Common.GPAType.GPA.ToString();
            //opt1.id = Common.GPAType.GPA.ToString();
            //var opt2 = new DDLStringList();
            //opt2.text = Common.GPAType.Nominee.ToString();
            //opt2.id = Common.GPAType.Nominee.ToString();
            //lst.Add(opt1);
            //lst.Add(opt2);
            //return lst;
        }

        public DataSourceResult GetNomineeData(int rid, DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var allNominees = new List<GPAModel>();
                int i = 0;
                var nominees = (from nom in dbContext.Nominee_Details
                                join am in dbContext.AllotmentMasters on nom.Rid equals am.rid
                                where nom.Is_Active == 1 && nom.Rid == rid && am.isActive == 1
                                select new GPAModel
                                {
                                    NomineeId = nom.Id,
                                    NomineeName = nom.Nominee_Name,
                                    NomineeRelation = nom.Relation,
                                    NominationDate = nom.Nomination_Date
                                }).ToList();
                if (nominees.Count > 0)
                {
                    allNominees = (from n in nominees select new GPAModel { NomineeId = n.NomineeId, NomineeName = n.NomineeName, NomineeRelation = n.NomineeRelation, NominationDate = n.NominationDate, SNo = ++i }).ToList();
                }
                return allNominees.ToDataSourceResult(req);
            }
        }

        public bool AddNominee(int rid, string nomName, string relation, DateTime nomDate)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var newNom = new Nominee_Details();
                newNom.Rid = rid;
                newNom.Nominee_Name = nomName;
                newNom.Nomination_Date = nomDate;
                newNom.Relation = relation;
                newNom.Is_Active = 1;
                newNom.Created_By = userInfo.UserID;
                newNom.Created_Date = DateTime.Now;
                dbContext.Nominee_Details.Add(newNom);
                dbContext.SaveChanges();
                flag = true;
            }
            return flag;
        }

        public bool RemoveNominee(int id)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var nominee = (from nom in dbContext.Nominee_Details where nom.Id == id select nom).FirstOrDefault();
                if (nominee != null)
                {
                    nominee.Is_Active = 0;
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }

        public GPAModel GetGPADetailsByRId(int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingGPA = (from gpa in dbContext.GPAs
                                   where gpa.Rid == rId && gpa.Is_Active == true
                                   select new GPAModel
                                   {
                                       RId = rId,
                                       GPAHolderName = gpa.GPA_Holder_Name,
                                       GPAHolderAdd = gpa.GPA_Holder_Address,
                                       EffectiveFrom = gpa.Effcetd_From,
                                       EffectiveTo = gpa.Effected_To,
                                       RelationName = gpa.Relation,
                                       ApplicationDate = gpa.Application_Date,
                                       AcceptanceDate = gpa.Acceptance_date,
                                       GPARegistered = gpa.Registered == true ? 1 : 2,
                                       GPARegisteredNo = gpa.Registration_No,
                                       GPAType = Common.GPAType.GPA.ToString(),
                                       GPAId = gpa.Id,
                                       //hdnRId = rId,
                                       hdnGPAType = Common.GPAType.GPA.ToString(),
                                       //fields added on  14 July 2017                     
                                       BahiNo = gpa.BahiNo,
                                       BahiZildNo = gpa.BahiZildNo,
                                       BahiPageNo = gpa.BahiPageNo,
                                       SINo = gpa.BahiSINo,
                                       GPASOA = !string.IsNullOrEmpty(gpa.GPASOA) ? (gpa.GPASOA == @NA.PMS.Common.GPAOption.GPASOAYes.ToString() ? 1 : 2) : 0,
                                       AgreementType = !string.IsNullOrEmpty(gpa.AgreementType) ? (gpa.AgreementType == @NA.PMS.Common.GPAOption.AgreementTypeReg.ToString() ? 1 : 2) : 0,
                                       RegistrationDate = gpa.RegistrationDate,
                                       SOABahiNo = gpa.SOABahiNo,
                                       SOABahiZildNo = gpa.SOABahiZildNo,
                                       SOABahiPageNo = gpa.SOABahiPageNo,
                                       SOASINo = gpa.SOASINo,
                                       AdvertismentInNewsPaper = gpa.AdvertismentInNewsPaper,
                                       PublishedDate = gpa.PublishedDate,
                                   }).FirstOrDefault();
                return existingGPA;
            }
        }

        #endregion

        public List<DynamicDataModel> GetAllottedPropertyRidList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var ridlist = (from alot in dbContext.AllotmentMasters
                               where (alot.isStatus == Status.Approved || alot.isStatus == Status.InProgress) && loginUserDeptt.Contains(alot.departmentId)
                               orderby alot.createdDate descending
                               select new DynamicDataModel
                               {
                                   Name = alot.rid.ToString(),
                                   Value = alot.rid
                               }).ToList();
                return ridlist;
            }
        }

        public DataSourceResult GetAllottedPropertyRidList(DataSourceRequest Req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var ridlist = (from alot in dbContext.AllotmentMasters
                               where (alot.isStatus == Status.Approved || alot.isStatus == Status.InProgress) && loginUserDeptt.Contains(alot.departmentId)
                               orderby alot.createdDate descending
                               select new DynamicDataModel
                               {
                                   Name = alot.rid.ToString(),
                                   Value = alot.rid
                               });
                return ridlist.ToDataSourceResult(Req);
            }
        }

        public AllottedPropertyPaymentModel GetAllottedPropertyDetail(int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var allottee = (from alottment in dbContext.AllotmentMasters
                                join schemprop in dbContext.SchemePropTrans on alottment.propertyId equals schemprop.propertyId
                                where alottment.rid == rid
                                select new AllottedPropertyPaymentModel
                                {
                                    RID_NO = alottment.rid.ToString(),
                                    PROPERTY_ID = alottment.propertyId.ToString(),
                                    FIRST_NAME = alottment.ApplicationDetail.firstName,
                                    MIDDLE_NAME = alottment.ApplicationDetail.middleName,
                                    LAST_NAME = alottment.ApplicationDetail.lastName,
                                    SECTOR_ID = schemprop.sectorId.Value,
                                    SECTOR = schemprop.SectorMst.sectorName,
                                    BLOCK_ID = schemprop.blockId.Value,
                                    BLOCK = schemprop.BlockMst.blockName,
                                    PROPERTY_NUMBER = schemprop.propertyNo,
                                    DEPARTMENT_ID = schemprop.departmentId,
                                    DEPARTMENT_NAME = schemprop.DepartmentMst.departmentName
                                }).FirstOrDefault();
                return allottee;
            }
        }

        public List<DynamicDataModel> GetPropertyPaymentType()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var receipt = (from recp in dbContext.RECIEPT_HEAD
                               select new DynamicDataModel
                               {
                                   Name = recp.RECIEPT_HEAD_NAME,
                                   Value = recp.RECIEPT_CODE
                               }).ToList();
                return receipt;
            }
        }

        public List<DynamicDataModel> GetPropertySubPaymentType(int receiptId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var subreceipt = (from recp in dbContext.RECEIPT_SUB_HEAD
                                  where recp.RECEIPT_CODE == receiptId
                                  select new DynamicDataModel
                                  {
                                      Name = recp.RECEIPT_SUB_HEAD1,
                                      Value = recp.RECEIPT_SUBHEAD_ID
                                  }).ToList();
                return subreceipt;
            }
        }

        public bool AddAllottedPropertyPayment(AllottedPropertyPaymentModel model)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var recpthed = dbContext.RECEIPT_DETAIL_MASTER.Where(r => r.RECEIPT_ID == model.RECEIPT_ID).FirstOrDefault();
                if (recpthed == null)
                {
                    RECEIPT_DETAIL_MASTER rdm = new RECEIPT_DETAIL_MASTER();
                    rdm.RECEIPT_ID = model.RECEIPT_ID;
                    rdm.RID_NO = model.RID.ToString();
                    rdm.ALLOTE_NAME = model.ALLOTEE_NAME;
                    rdm.ADDRESS = model.ADDRESS;
                    rdm.AMOUNT = model.AMOUNT;
                    rdm.SECTOR = model.SECTOR;
                    rdm.BLOCK = model.BLOCK;
                    rdm.PROP_ID = model.PROPERTY_ID;
                    rdm.PROPERTY_NUMBER = model.PROPERTY_NUMBER;
                    rdm.DEPOSETER_NAME = model.DEPOSITOR_NAME;
                    rdm.DEPT_ID = model.DEPARTMENT_ID;
                    rdm.PROP_REG_ID = model.PROPERTY_REGESTRY_ID;
                    rdm.ENTRY_DATE = model.ENTRY_DATE;
                    rdm.DEPOSIT_DATE = model.DEPOSIT_DATE;
                    rdm.STATUS = 1;
                    rdm.USERID = userInfo.UserID.ToString();
                    rdm.BANK_ID = model.BANK_ID;
                    rdm.CHALLAN_ID = model.CHALLAN_ID;

                    dbContext.RECEIPT_DETAIL_MASTER.Add(rdm);
                    dbContext.SaveChanges();

                    RECEIPT_AMOUNT_TRANS rmt = new RECEIPT_AMOUNT_TRANS();
                    rmt.RECEIPT_ID = model.RECEIPT_ID;
                    rmt.DEPT_CODE = model.DEPARTMENT_ID;
                    rmt.RECEIPT_SUBHEAD_ID = model.RECEIPT_SUBHEAD_ID;
                    rmt.RECEIPT_HEAD_ID = model.RECIEPT_CODE;
                    rmt.AMOUNT_PAID = model.AMOUNT.Value;
                    rmt.CHALLAN_ID = model.CHALLAN_ID;
                    rmt.STATUS = 1;
                    rmt.USERID = userInfo.UserID.ToString();
                    rmt.ENTRY_DATE = model.ENTRY_DATE;
                    rmt.DEPOSIT_DATE = model.DEPOSIT_DATE.Value;

                    dbContext.RECEIPT_AMOUNT_TRANS.Add(rmt);
                    dbContext.SaveChanges();

                    flag = true;
                }
            }
            return flag;
        }

        public List<AllottedPropertyPaymentModel> GetPropertyPaymentDetails()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var paymentList = (from payment in dbContext.RECEIPT_DETAIL_MASTER
                                   join alotment in dbContext.AllotmentMasters on payment.RID_NO equals alotment.rid.ToString()
                                   join schemeprop in dbContext.SchemePropTrans on alotment.propertyId equals schemeprop.propertyId
                                   select new AllottedPropertyPaymentModel
                                   {
                                       RECEIPT_ID = payment.RECEIPT_ID,
                                       RID_NO = payment.RID_NO,
                                       DEPARTMENT_NAME = alotment.DepartmentMst.departmentName,
                                       FIRST_NAME = alotment.ApplicationDetail.firstName,
                                       MIDDLE_NAME = alotment.ApplicationDetail.middleName,
                                       LAST_NAME = alotment.ApplicationDetail.lastName,
                                       PROPERTY_NUMBER = schemeprop.SectorMst.sectorName + "/" + schemeprop.BlockMst.blockName + "-" + schemeprop.propertyNo,
                                       AMOUNT = payment.AMOUNT,
                                       DEPOSIT_DATE = payment.DEPOSIT_DATE,
                                       ENTRY_DATE = payment.ENTRY_DATE
                                   }).ToList();
                return paymentList;
            }
        }
        //To Generate Generate Functional Certificater
        public string GenerateFunctionalCertificater(int rid)
        {
            string strLettter = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                var Objdeptt = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == rid && m.isActive == 1);
                if (Objdeptt != null)
                {
                    strLettter = CommonMethords.GenerateLetter(Objdeptt.rid, Constants.FunctionalCertificatID, Objdeptt.departmentId.Value, userInfo.UserID);
                }
            }
            return strLettter;


        }



        public List<ChallanModel> GetModelListToPrintRentPermissionChallan(List<int> requestNoList, List<int> ridList)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var challanDetail = (from rent in dbContext.RentPermissionDetails
                                     join alot in dbContext.AllotmentMasters on rent.Rid equals alot.rid
                                     join scpt in dbContext.SchemePropTrans on alot.propertyId equals scpt.propertyId
                                     where requestNoList.Contains(rent.RequestNo)
                                     select new ChallanModel
                                     {
                                         RID = alot.rid,
                                         FormNo = alot.formNo,
                                         SectorName = scpt.SectorMst.sectorName,
                                         BlockName = scpt.BlockMst.blockName,
                                         PropertyTypeName = scpt.PropertyTypeMst.propertyTypeName,
                                         PropertyNumber = scpt.propertyNo,
                                         AllotmentMoney = scpt.allotmentMoney,
                                         ApplicationForm = new ApplicationFormModel
                                         {
                                             FirstName = alot.ApplicationDetail.firstName,
                                             MiddleName = alot.ApplicationDetail.middleName,
                                             LastName = alot.ApplicationDetail.lastName,
                                             CorrespondingAddress = alot.ApplicationDetail.correspondanceAdd,
                                             MobileNumber = alot.ApplicationDetail.mobileNumberP2,
                                             PhoneNumber = alot.ApplicationDetail.phoneNumberP2,
                                             Email = alot.ApplicationDetail.email
                                         },
                                     }).ToList();
                return challanDetail;
            }
        }
        public List<ChallanModel> GetModelListToPrintFunctionalChallan(List<int> requestNoList, List<int> ridList)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var challanDetail = (from rent in dbContext.FunctionalDetails
                                     join alot in dbContext.AllotmentMasters on rent.Rid equals alot.rid
                                     join scpt in dbContext.SchemePropTrans on alot.propertyId equals scpt.propertyId
                                     where requestNoList.Contains(rent.RequestNo)
                                     select new ChallanModel
                                     {
                                         RID = alot.rid,
                                         FormNo = alot.formNo,
                                         SectorName = scpt.SectorMst.sectorName,
                                         BlockName = scpt.BlockMst.blockName,
                                         PropertyTypeName = scpt.PropertyTypeMst.propertyTypeName,
                                         PropertyNumber = scpt.propertyNo,
                                         AllotmentMoney = scpt.allotmentMoney,
                                         ApplicationForm = new ApplicationFormModel
                                         {
                                             FirstName = alot.ApplicationDetail.firstName,
                                             MiddleName = alot.ApplicationDetail.middleName,
                                             LastName = alot.ApplicationDetail.lastName,
                                             CorrespondingAddress = alot.ApplicationDetail.correspondanceAdd,
                                             MobileNumber = alot.ApplicationDetail.mobileNumberP2,
                                             PhoneNumber = alot.ApplicationDetail.phoneNumberP2,
                                             Email = alot.ApplicationDetail.email
                                         },
                                     }).ToList();
                return challanDetail;
            }
        }
        public List<InterviewDetailsModel> GetSchemeList()
        {
            int userid = userInfo.UserID;
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userid
                                      select dept.DepartmentId).ToList();

                List<InterviewDetailsModel> schemeList = new List<InterviewDetailsModel>();
                schemeList = (from schemes in dbContext.SchemeMsts
                              join dpt in dbContext.SchemeDepartmentTrans on schemes.schemeId equals dpt.schemeId
                              where schemes.IsActive == true && schemes.completed == true && loginUserDeptt.Contains(dpt.departmentId)
                              select new InterviewDetailsModel
                              {
                                  SchemeId = schemes.schemeId,
                                  SchemeName = schemes.schemeName
                              }).Distinct().ToList();
                return schemeList;
            }

        }
        public List<InterviewDetailsModel> FilterDepartmentOnScheme(int schemeId)
        {
            int userid = userInfo.UserID;
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userid
                                      select dept.DepartmentId).ToList();

                List<InterviewDetailsModel> departmentList = new List<InterviewDetailsModel>();
                departmentList = (from dept in dbContext.DepartmentMsts
                                  join sdt in dbContext.SchemeDepartmentTrans on dept.departmentId equals sdt.departmentId
                                  where sdt.schemeId == schemeId && sdt.selectionType == "OpenEnded" && loginUserDeptt.Contains(sdt.departmentId)
                                  select new InterviewDetailsModel
                                  {
                                      DepartmentId = dept.departmentId,
                                      DepartmentName = dept.departmentName
                                  }).ToList();
                return departmentList;
            }
        }

        /// <summary>
        /// To get the details of Applicant details on the basis of formno, scheme and department.
        /// </summary>
        /// <param name="formno"></param>
        /// <param name="SchemeId"></param>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        public InterviewDetailsModel GetApplicantDetailsForInterview(string formno, int SchemeId, int departmentID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var application = dbContext.ApplicationDetails.Where(a => a.formNo == formno && a.schemeId == SchemeId && a.departmentId == departmentID).FirstOrDefault();

                var applicant = (from appl in dbContext.ApplicationDetails
                                 //join scot in dbContext.SchemeCostTrans on new { x1 = appl.schemeId, x2 = appl.departmentId } equals new { x1 = scot.schemeId, x2 = scot.departmentId }
                                 where appl.formNo == formno && appl.schemeId == SchemeId && appl.departmentId == departmentID
                                 select new InterviewDetailsModel
                                 {
                                     ApplicationId = appl.applicationId,
                                     FormNo = appl.formNo,
                                     Gender = appl.gender,
                                     ApplicationName = appl.firstName + " " + appl.middleName + " " + appl.lastName,
                                     RelationName = appl.fatherHusbandName,
                                     Address = appl.permanentAdd
                                 }).FirstOrDefault();
                return applicant;
            }
        }
        /// <summary>
        /// Add and Edit Application Form
        /// </summary>
        /// <param name="applicationFormModel"></param>
        /// <param name="applicationFormId"></param>
        /// <returns></returns>

        public int SaveInterviewForm(int ApplicationId, string InterviewDetails, DateTime InterviewDate, int SchemeId, int DepartmentId, string FormNo)
        {
            var flag = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingForm = dbContext.Interview_Details.Where(i => i.Application_Id == ApplicationId && i.Is_Active == true).FirstOrDefault();
                var objScm = dbContext.SchemeMsts.Where(i => i.schemeId == SchemeId && i.IsActive == true).FirstOrDefault();

                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.applicationId == ApplicationId select appDet).FirstOrDefault();
                if (InterviewDate > objScm.endDate || InterviewDate < objScm.startDate)
                {
                    flag = 3;
                    return flag;
                }
                if (existingForm == null)
                {
                    Interview_Details objInterview = new Interview_Details();
                    objInterview.Application_Id = ApplicationId;
                    objInterview.Interview_Date = InterviewDate;
                    objInterview.Interview_Details1 = InterviewDetails;
                    objInterview.Created_By = userInfo.UserID;
                    objInterview.Form_No = FormNo;
                    objInterview.Created_Date = DateTime.Now;
                    objInterview.Is_Active = true;
                    dbContext.Interview_Details.Add(objInterview);
                    dbContext.SaveChanges();
                    flag = 1;
                    string body = string.Empty;
                    string msg = string.Empty;


                    body = "Hi,<br><br>Your Interview Request has been saved.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                    //msg = "Your Interview Request has been saved.";
                    msg = NAMessages.InterviewReqSave;
                    EmailHelper emailHelper = new EmailHelper();
                    emailHelper.Send(newOwner.tEmail, "Interview details saved", body);
                    //SMS
                    //SMSSend(newOwner.tMobileNumber, msg);
                    ApplicationHelper.SendSMS(newOwner.tMobileNumber, msg);
                }
                else
                {
                    flag = 2;
                }

            }

            return flag;
        }

        public InterviewDetailsModel GetInterviewDetailsById(int id)
        {
            //InterviewDetailsModel objInterview = new InterviewDetailsModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                var objInterview = (from funDetails in dbContext.Interview_Details
                                    join am in dbContext.ApplicationDetails on funDetails.Application_Id equals am.applicationId
                                    join dep in dbContext.DepartmentMsts on am.departmentId equals dep.departmentId
                                    join scm in dbContext.SchemeMsts on am.schemeId equals scm.schemeId
                                    where funDetails.Id == id && funDetails.Is_Active == true
                                    select new InterviewDetailsModel
                                    {

                                        ApplicationId = am.applicationId,
                                        SchemeId = scm.schemeId,
                                        SchemeName = scm.schemeName,
                                        DepartmentId = dep.departmentId,
                                        DepartmentName = dep.departmentName,
                                        InterviewDate = funDetails.Interview_Date,
                                        InterviewDetails = funDetails.Interview_Details1,
                                        Id = funDetails.Id,
                                        FormNo = funDetails.Form_No,
                                        Gender = am.gender,
                                        ApplicationName = am.firstName + " " + am.middleName + " " + am.lastName,
                                        RelationName = am.fatherHusbandName,
                                        Address = am.permanentAdd
                                    }).FirstOrDefault();
                return objInterview;


            }
        }
        public DataSourceResult GetInterviewDetails(DataSourceRequest sourceReq)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var objFunctional = (from funDetails in dbContext.Interview_Details
                                     join am in dbContext.ApplicationDetails on funDetails.Application_Id equals am.applicationId
                                     join dep in dbContext.DepartmentMsts on am.departmentId equals dep.departmentId
                                     join scm in dbContext.SchemeMsts on am.schemeId equals scm.schemeId
                                     where funDetails.Is_Active == true && loginUserDeptt.Contains(am.departmentId)
                                     select new InterviewDetailsModel
                                     {
                                         ApplicationId = am.applicationId,
                                         SchemeId = scm.schemeId,
                                         SchemeName = scm.schemeName,
                                         DepartmentId = dep.departmentId,
                                         DepartmentName = dep.departmentName,
                                         InterviewDate = funDetails.Interview_Date,
                                         Id = funDetails.Id,
                                         IsAllotted = am.isAllotted,
                                         FormNo = funDetails.Form_No

                                     });


                return objFunctional.ToDataSourceResult(sourceReq);
            }
        }
        public int UpdateInterviewDetails(int ApplicationId, string InterviewDetails, DateTime InterviewDate, int Id, int SchemeId)
        {
            int flag = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingInterView = (from bpm in dbContext.Interview_Details where bpm.Id == Id && bpm.Is_Active == true select bpm).FirstOrDefault();
                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.applicationId == existingInterView.Application_Id select appDet).FirstOrDefault();
                var objScm = dbContext.SchemeMsts.Where(i => i.schemeId == SchemeId && i.IsActive == true).FirstOrDefault();
                Interview_Details oldObjModel = new Interview_Details();//Old Model
                oldObjModel.Interview_Details1 = existingInterView.Interview_Details1;
                if (InterviewDate > objScm.endDate || InterviewDate < objScm.startDate)
                {
                    flag = 3;
                    return flag;
                }
                if (existingInterView != null)
                {
                    existingInterView.Interview_Details1 = InterviewDetails;
                    existingInterView.Interview_Date = InterviewDate;
                    existingInterView.Modified_By = userInfo.UserID;
                    existingInterView.Modified_Date = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = 1;

                    string body = "Hi,<br><br>Your Interview Request has been updated.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                    //string msg = "Your Interview Request has been updated.";
                    string msg = NAMessages.InterviewReqUpdate;
                    EmailHelper emailHelper = new EmailHelper();
                    emailHelper.Send(newOwner.tEmail, "Interview details updated", body);
                    //SMS
                    //SMSSend(newOwner.tMobileNumber, msg);
                    ApplicationHelper.SendSMS(newOwner.tMobileNumber, msg);
                    Interview_Details newObjModel = new Interview_Details();//Old Model
                    newObjModel.Interview_Details1 = existingInterView.Interview_Details1;
                    GeneralRepository.CreateAuditTrail(Constants.Update, Constants.InterviewDetails, Constants.InterviewDetails, Id, oldObjModel, newObjModel, userInfo.UserID.ToString());
                }
                return flag;
            }

            //            int lastInsertedApplicationFormId = dbContext.ApplicationDetails.Max(u => u.applicationId);
            //            var payment = new ApplicationPaymentDetail();
            //            payment.applicationId = lastInsertedApplicationFormId;
            //            payment.paymentMode = formModel.PaymentType;
            //            payment.formNo = formModel.FormNo.ToString();
            //            payment.amountDeposited = formModel.AmountDepositedId;
            //            payment.bankId = formModel.BankId;
            //            if (formModel.DemandDraftNumber == null && formModel.UTRNumber != null)
            //            {
            //                payment.utn = formModel.UTRNumber;
            //                //payment.branchId = Constants.BranchIdOther;
            //            }
            //            else
            //            {
            //                payment.ddNo = formModel.DemandDraftNumber;
            //                payment.ddIssueBank = formModel.IssueBank;
            //                payment.branchId = formModel.BranchId;
            //            }
            //            payment.ddIssueDate = formModel.IssueDate;
            //            payment.createdBy = userInfo.UserID.ToString();
            //            payment.createdDate = DateTime.Now;
            //            dbContext.ApplicationPaymentDetails.Add(payment);
            //            dbContext.SaveChanges();
            //            flag = true;
            //        }
            //    }
            //}
            return flag;
        }
        public bool RemoveInterviewDetails(int rId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var refnd = dbContext.Interview_Details.FirstOrDefault(cond => cond.Id == rId && cond.Is_Active == true);
                var newOwner = (from appDet in dbContext.ApplicationDetails where appDet.applicationId == refnd.Application_Id select appDet).FirstOrDefault();
                Interview_Details oldObjModel = new Interview_Details();//Old Model
                oldObjModel.Is_Active = refnd.Is_Active;
                if (refnd != null)
                {

                    refnd.Is_Active = false;
                    refnd.Modified_By = userInfo.UserID;
                    refnd.Modified_Date = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = true;
                    string body = "Hi,<br><br>Your Interview Request has been removed.<br><br>We value your relationship with us and assure you of our best services always.<br><br>Thanks and Regards,<br>Noida Authority";
                    //string msg = "Your Interview Request has been removed.";
                    string msg = NAMessages.InterviewReqRemove;
                    EmailHelper emailHelper = new EmailHelper();
                    emailHelper.Send(newOwner.tEmail, "Interview details removed", body);
                    //SMS
                    //SMSSend(newOwner.tMobileNumber, msg);
                    ApplicationHelper.SendSMS(newOwner.tMobileNumber, msg);
                    Interview_Details newObjModel = new Interview_Details();//Old Model
                    newObjModel.Is_Active = refnd.Is_Active;

                    GeneralRepository.CreateAuditTrail(Constants.Update, Constants.InterviewDetails, Constants.InterviewDetails, rId, oldObjModel, newObjModel, userInfo.UserID.ToString());
                }

            }
            return flag;
        }
        public List<DDList> GetRIDsForNoting()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var objRid = (from comRid in dbContext.AllotmentMasters
                              where comRid.isActive == 1 && loginUserDeptt.Contains(comRid.departmentId)
                              select new DDList
                              {
                                  id = comRid.rid,
                                  text = comRid.rid.ToString()
                              }).ToList();

                return objRid;
            }
        }
        public List<DDList> GetDeptmentForNoting()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var objRid = (from comRid in dbContext.DepartmentMsts
                              where comRid.IsActive == true && loginUserDeptt.Contains(comRid.departmentId)
                              select new DDList
                              {
                                  id = comRid.departmentId,
                                  text = comRid.departmentName
                              }).ToList();

                return objRid;
            }
        }
        public int AddNotingFile(int Rid, string FileName, string DepartmentName, int? deptId)
        {
            int flag = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                var objNoting = dbContext.Noting_File_Master.FirstOrDefault(cond => cond.Rid == Rid && cond.Is_Active == true);
                if (objNoting == null)
                {
                    Noting_File_Master noting = new Noting_File_Master();
                    noting.File_Number = FileName;
                    noting.Rid = Rid;
                    noting.Department_Id = deptId;
                    noting.Created_By = userInfo.UserID;
                    noting.Created_Date = DateTime.Now;
                    noting.Is_Active = true;
                    noting.Receive_By = userInfo.UserID;
                    noting.Receive_Date = DateTime.Now;
                    dbContext.Noting_File_Master.Add(noting);
                    dbContext.SaveChanges();
                    flag = 1;
                }
                else
                {
                    flag = 2;
                }
                return flag;
            }
        }

        public List<DrawSlipModel> GetApplicationFormDetailsForDrawSlip(int schemeId, int departmentId, List<string> nameList)
        {
            using (var dbContext = new NoidaPMSEntities())
            {

                var drawSlipList = (from applicant in dbContext.ApplicationDetails
                                    join payment in dbContext.ApplicationPaymentDetails on applicant.applicationId equals payment.applicationId
                                    //join schemecost in dbContext.SchemeCostTrans on new { applicant.schemeId, applicant.departmentId } equals new { schemecost.schemeId,schemecost.departmentId}
                                    where applicant.schemeId == schemeId && applicant.departmentId == departmentId
                                    select new DrawSlipModel
                                    {
                                        ApplicationId = applicant.applicationId,
                                        SchemeId = applicant.schemeId.Value,
                                        SchemeName = applicant.SchemeMst.schemeName,
                                        DepartmentId = applicant.departmentId.Value,
                                        DepartmentName = applicant.DepartmentMst.departmentName,
                                        PropertyTypeId = dbContext.SchemeCostTrans.Where(x => x.schemeId == applicant.schemeId && x.departmentId == applicant.departmentId && x.earnestMoney == payment.amountDeposited).Select(s => s.propertyTypeId).FirstOrDefault(),
                                        PropertyTypeName = dbContext.SchemeCostTrans.Where(x => x.schemeId == applicant.schemeId && x.departmentId == applicant.departmentId && x.earnestMoney == payment.amountDeposited).Select(s => s.PropertyTypeMst.propertyTypeName).FirstOrDefault(),

                                        BankName = payment.bankId == null ? payment.ddIssueBank : payment.BankMst.bankName //dbContext.ApplicationPaymentDetails.Where(y=> y.applicationId == applicant.applicationId).Select(x=> x.BankMst.bankName).ToString()
                                        //FormNumber = applicant.formNo,
                                        //FirstName = applicant.firstName,
                                        //MiddleName = applicant.middleName,
                                        //LastName = applicant.lastName,
                                        //FatherName = applicant.fatherHusbandName,
                                        //MotherName = applicant.motherName,
                                        //MobileNumber = applicant.mobileNumberP2,
                                        //PanNumber = applicant.pan
                                    }).ToList();
                foreach (var draw in drawSlipList)
                {
                    foreach (var lst in nameList)
                    {
                        if (lst == "chkApplicant")
                        {
                            draw.FirstName = dbContext.ApplicationDetails.Where(ad => ad.applicationId == draw.ApplicationId).Select(d => d.firstName).FirstOrDefault();
                            draw.MiddleName = dbContext.ApplicationDetails.Where(ad => ad.applicationId == draw.ApplicationId).Select(d => d.middleName).FirstOrDefault();
                            draw.LastName = dbContext.ApplicationDetails.Where(ad => ad.applicationId == draw.ApplicationId).Select(d => d.lastName).FirstOrDefault();
                        }
                        if (lst == "chkFormNo")
                        {
                            draw.FormNumber = dbContext.ApplicationDetails.Where(ad => ad.applicationId == draw.ApplicationId).Select(d => d.formNo).FirstOrDefault();
                        }
                        if (lst == "chkMobile")
                        {
                            draw.MobileNumber = dbContext.ApplicationDetails.Where(ad => ad.applicationId == draw.ApplicationId).Select(d => d.mobileNumberP2).FirstOrDefault();
                        }
                        if (lst == "chkFatherName")
                        {
                            draw.FatherName = dbContext.ApplicationDetails.Where(ad => ad.applicationId == draw.ApplicationId).Select(d => d.fatherHusbandName).FirstOrDefault();
                        }
                        if (lst == "chkMotherName")
                        {
                            draw.MotherName = dbContext.ApplicationDetails.Where(ad => ad.applicationId == draw.ApplicationId).Select(d => d.motherName).FirstOrDefault();
                        }
                        if (lst == "chkPan")
                        {
                            draw.PanNumber = dbContext.ApplicationDetails.Where(ad => ad.applicationId == draw.ApplicationId).Select(d => d.pan).FirstOrDefault();
                        }
                    }
                }
                return drawSlipList;
            }
        }



        public NotingDetailsModel GetNotingDeptmentByRId(int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var objNoting = (from posDetail in dbContext.AllotmentMasters
                                 join dept in dbContext.DepartmentMsts on posDetail.departmentId equals dept.departmentId
                                 where posDetail.rid == rId && posDetail.isActive == 1
                                 select new NotingDetailsModel
                                 {
                                     Rid = posDetail.rid,
                                     DepartmentId = posDetail.departmentId,
                                     DepartmentName = dept.departmentName
                                 }).FirstOrDefault();



                return objNoting;
            }
        }
        public DataSourceResult GetNotingDetails(DataSourceRequest sourceReq, int? Rid, int? DepartmentId, string FileName)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //NotingDetailsModel objFunctional1 = new NotingDetailsModel();
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var notings = (from notingMaster in dbContext.Noting_File_Master
                               join dep in dbContext.DepartmentMsts on notingMaster.Department_Id equals dep.departmentId
                               where notingMaster.Is_Active == true
                                  && (Rid == null || notingMaster.Rid == Rid)
                                  && (DepartmentId == null || notingMaster.Department_Id == DepartmentId)
                                  && (FileName == "" || notingMaster.File_Number == FileName)
                                   //|| (notingMaster.Rid ==Rid || notingMaster .Department_Id ==DepartmentId || notingMaster .File_Number ==FileName )
                                  && loginUserDeptt.Contains(notingMaster.Department_Id)
                               select new NotingDetailsModel
                               {
                                   DepartmentId = dep.departmentId,
                                   DepartmentName = dep.departmentName,
                                   Rid = notingMaster.Rid.Value,
                                   FileName = notingMaster.File_Number,
                                   FileNameNotingCreate = notingMaster.File_Number,
                                   CreatedBy = (from uname in dbContext.UmUserMasters where uname.UserRefId == notingMaster.Created_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                   ReceiveBy = (from uname in dbContext.UmUserMasters where uname.UserRefId == notingMaster.Receive_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                   CreatedDate = notingMaster.Created_Date,
                                   Receive_Date = notingMaster.Receive_Date

                               }).ToList();


                return notings.ToDataSourceResult(sourceReq);
            }
        }
        public NotingDetailsModel ViewNotingDetails(int reqNo)
        {
            using (var dbContext = new NoidaPMSEntities())
            {

                var objFunctional = (from notingMaster in dbContext.Noting_File_Master
                                     join dep in dbContext.DepartmentMsts on notingMaster.Department_Id equals dep.departmentId
                                     //join notings in dbContext.Noting_File_Trans on notingMaster.Id equals notings.Noting_File_Id 
                                     where notingMaster.Is_Active == true && notingMaster.Rid == reqNo
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
                                         Receive_Date = notingMaster.Receive_Date

                                     }).FirstOrDefault();

                if (objFunctional.ReceiveByID == userInfo.UserID)
                {
                    objFunctional.IsNotingDetailShow = true;
                }
                else
                {
                    objFunctional.IsNotingDetailShow = false;
                }


                return objFunctional;
            }
        }
        public string AddNotingsDetails(int Rid, string AddNotingDetails)
        {
            string result = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                var noting = dbContext.Noting_File_Master.FirstOrDefault(cond => cond.Rid == Rid && cond.Is_Active == true);

                var objUser = dbContext.UmUserMasters.FirstOrDefault(cond => cond.UserRefId == userInfo.UserID);
                if (noting != null)
                {
                    var notingFile = dbContext.Noting_File_Trans.FirstOrDefault(cond => cond.Noting_File_Id == noting.Id && cond.Is_Active == true);
                    if (notingFile != null)
                    {
                        string notes = notingFile.Noting_Details;
                        notingFile.Noting_Details = notes + "<br>" + AddNotingDetails + "<br>" + userInfo.FirstName + " " + userInfo.MiddleName + " " + userInfo.LastName + "<br>" + DateTime.Now.ToString() + "<br>";
                        notingFile.Is_Active = true;
                        notingFile.Note_By = userInfo.UserID;
                        notingFile.Noting_Date = DateTime.Now;
                        notingFile.Modified_By = userInfo.UserID;
                        notingFile.Modified_Date = DateTime.Now;

                        dbContext.SaveChanges();
                        result = notingFile.Noting_Details;
                    }
                    else
                    {
                        Noting_File_Trans notingfile = new Noting_File_Trans();
                        notingfile.Noting_File_Id = noting.Id;
                        notingfile.Noting_Details = AddNotingDetails + "<br>" + userInfo.FirstName + " " + userInfo.MiddleName + " " + userInfo.LastName + "<br>" + DateTime.Now.ToString();
                        notingfile.Created_By = userInfo.UserID;
                        notingfile.Created_Date = DateTime.Now;
                        notingfile.Is_Active = true;
                        notingfile.Note_By = userInfo.UserID;
                        notingfile.Noting_Date = DateTime.Now;

                        dbContext.Noting_File_Trans.Add(notingfile);
                        dbContext.SaveChanges();

                        result = notingfile.Noting_Details;
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
        }
        public int SubmitForMoveNoting(int Rid, string user)
        {
            int flag = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                var noting = dbContext.Noting_File_Master.FirstOrDefault(m => m.Rid == Rid && m.Is_Active == true);
                Noting_File_Master oldObjModel = new Noting_File_Master();//Old Model
                //oldObjModel.Comment  = existingRequest.Comment;
                oldObjModel.Receive_By = noting.Receive_By;
                if (noting != null)
                {
                    //noting.Receive_By = (from uname in dbContext.UmUserMasters where uname.UserName == user && uname.IsActive == true select uname.UserRefId).FirstOrDefault();
                    noting.Receive_By = Convert.ToInt32(user);
                    noting.Receive_Date = DateTime.Now;
                    noting.Modified_By = userInfo.UserID;
                    noting.Modified_Date = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = 1;

                    //oldObjModel .ApproveDate=existingRequest.ApproveDate ;
                    //New Model for Audit Trail
                    Noting_File_Master newObjModel = new Noting_File_Master();
                    newObjModel.Receive_By = noting.Receive_By;

                    int RequestNo = Rid;
                    GeneralRepository.CreateAuditTrail(Constants.Update, Constants.NotingDetails, "NotingDetails", RequestNo, oldObjModel, newObjModel, userInfo.UserID.ToString());
                }
                return flag;
            }


        }
        public List<DDList> GetAllAccountHead()
        {
            using (var dbContext = new NoidaPMSEntities())
            {

                var lst = (from transTy in dbContext.RECIEPT_HEAD
                           where transTy.STATUS == 1
                           select new DDList
                           {
                               text = transTy.RECIEPT_HEAD_NAME,
                               id = transTy.RECIEPT_CODE
                           }).ToList();
                return lst;
            }
        }
        public List<DDList> GetAccountSubHead(int AccountHeadId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from subhead in dbContext.RECEIPT_SUB_HEAD
                           where subhead.STATUS == 1 && subhead.RECEIPT_CODE == AccountHeadId
                           select new DDList
                               {
                                   text = subhead.RECEIPT_SUB_HEAD1,
                                   id = subhead.RECEIPT_SUBHEAD_ID
                               }).ToList();
                return lst;
            }
        }

        public int SaveCreateChallan(int rId, int AccountHeadId, int AccountSubHeadId, decimal? Amount)
        {
            int flag = 0;
            List<BankAccountManagementModel> data = (List<BankAccountManagementModel>)HttpContext.Current.Session["TempModel"];
            if (data == null)
            {
                List<BankAccountManagementModel> modelList = new List<BankAccountManagementModel>();
                BankAccountManagementModel model = new BankAccountManagementModel();
                model.RId = rId;
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
                    if (models.RId != rId)
                    {
                        data = null;
                        List<BankAccountManagementModel> modelList = new List<BankAccountManagementModel>();
                        BankAccountManagementModel model = new BankAccountManagementModel();
                        model.RId = rId;
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
                //commented so that user can add multiple head.6/11/17

                //if (count >= 3)
                //{
                //    return flag = 3;
                //}
                //else
                //{
                //    BankAccountManagementModel model = new BankAccountManagementModel();
                //    model.RId = rId;
                //    model.AccountHeadId = AccountHeadId;
                //    model.AccountSubHeadId = AccountSubHeadId;
                //    model.Amount = Amount.Value;
                //    data.Add(model);
                //    HttpContext.Current.Session["TempModel"] = data;
                //}

                BankAccountManagementModel objmodel = new BankAccountManagementModel();
                objmodel.RId = rId;
                objmodel.AccountHeadId = AccountHeadId;
                objmodel.AccountSubHeadId = AccountSubHeadId;
                objmodel.Amount = Amount.Value;
                data.Add(objmodel);
                HttpContext.Current.Session["TempModel"] = data;
            }

            //using (var dbContext = new NoidaPMSEntities())
            //{
            //    var objExit = dbContext.Challan_Trans.FirstOrDefault(m => m.Rid == rId && m.Head_Id == AccountHeadId && m.Subhead_Id == AccountSubHeadId && m.Is_Active == true && m.Challan_Master_Id == null);
            //    if (objExit == null)
            //    {
            //        Challan_Trans objChallanMaster = new Challan_Trans();
            //        objChallanMaster.Rid = rId;
            //        objChallanMaster.Head_Id = AccountHeadId;
            //        objChallanMaster.Subhead_Id = AccountSubHeadId;
            //        objChallanMaster.Amount = Amount;
            //        objChallanMaster.Is_Active = true;
            //        objChallanMaster.Created_By = userInfo.UserID;
            //        objChallanMaster.Created_Date = DateTime.Now;
            //        dbContext.Challan_Trans.Add(objChallanMaster);
            //        dbContext.SaveChanges();
            //        flag = 1;
            //    }
            //    else
            //    {
            //        flag = 2;
            //    }

            //}
            return flag;
        }

        public string SaveGenerateChallan(int rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId)
        {
            string flag = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                //var objExit = dbContext.Challan_Trans.FirstOrDefault(m => m.Rid == rId && m.Head_Id == AccountHeadId && m.Subhead_Id == AccountSubHeadId);
                //if (objExit == null)
                //{
                Challan_Master objChallanMaster = new Challan_Master();
                objChallanMaster.Rid = rId;
                objChallanMaster.Bank_Id = bankId;
                objChallanMaster.Branch_Id = branchId;
                objChallanMaster.Account_Number = DdlAccountNumber.ToString();
                objChallanMaster.Is_Active = true;
                objChallanMaster.Created_By = userInfo.UserID;
                objChallanMaster.Created_Date = DateTime.Now;
                objChallanMaster.Generate_Date = DateTime.Now;
                dbContext.Challan_Master.Add(objChallanMaster);
                dbContext.SaveChanges();

                var objExit = dbContext.Challan_Master.FirstOrDefault(m => m.Rid == rId && m.Bank_Id == bankId && m.Branch_Id == branchId && m.Is_Active == true && m.Account_Number == DdlAccountNumber);
                var objAllotteeLsts = dbContext.Challan_Trans.Where(i => i.Rid == rId && i.Is_Active == true && i.Challan_Master_Id == null).ToList();

                if (objAllotteeLsts.Count > 0)
                {
                    //var objUserList = dbContext.UmUserMasters.Where(i => i.UserName == user).FirstOrDefault();
                    objAllotteeLsts.Select(ua =>
                    {
                        ua.Challan_Master_Id = objExit.Id;
                        //ua.submitDate = DateTime.Now;
                        //ua.submittedBy = userid;
                        //ua.approverBy = objUserList.UserRefId.ToString();
                        //ua.isSubmitted = "1";
                        return ua;
                    }).ToList();
                    dbContext.SaveChanges();
                }
                flag = "1";
                //flag = CommonMethords.GenerateLetter(rId, 2, 5, userInfo.UserID);
                //flag = 
            }
            return flag;
        }

        public bool RemoveChallanChargeDetail(int ChallanTransID, int rId)
        {
            bool flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                Challan_Master challan = dbContext.Challan_Master.FirstOrDefault(m => m.Id == ChallanTransID && m.Is_Active == true);
                Challan_Trans challanTrans = dbContext.Challan_Trans.FirstOrDefault(c => c.Challan_Master_Id == ChallanTransID && c.Is_Active == true);
                if (challan != null && challanTrans != null)
                {
                    Challan_Master challanMaster = new Challan_Master();
                    challan.Modified_By = userInfo.UserID;
                    challan.Modified_Date = DateTime.Now;
                    challan.Is_Active = false;
                    dbContext.SaveChanges();

                    Challan_Trans challanT = new Challan_Trans();
                    challanTrans.Modified_By = userInfo.UserID;
                    challanTrans.Modified_Date = DateTime.Now;
                    challanTrans.Is_Active = false;
                    dbContext.SaveChanges();
                    flag = true;
                    GeneralRepository.CreateAuditTrail(Constants.Update, Constants.AccountChallanChargeDetail, Constants.AccountChallanChargeDetail, ChallanTransID, challan, challanMaster, userInfo.UserID.ToString());
                    GeneralRepository.CreateAuditTrail(Constants.Update, Constants.AccountChallanChargeDetail, Constants.AccountChallanChargeDetail, ChallanTransID, challanTrans, challanT, userInfo.UserID.ToString());
                }
            }
            return flag;
        }
        public DataSourceResult GetAccountChargeDetails(DataSourceRequest request, int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var objAccount = (from rd in dbContext.Challan_Trans
                                  join receipthead in dbContext.RECIEPT_HEAD on rd.Head_Id equals receipthead.RECIEPT_CODE
                                  join receiptsubhead in dbContext.RECEIPT_SUB_HEAD on rd.Subhead_Id equals receiptsubhead.RECEIPT_SUBHEAD_ID
                                  where rd.Is_Active == true && rd.Rid == rId && rd.Challan_Master_Id == null
                                  select new BankAccountManagementModel
                                  {
                                      RId = rd.Rid,
                                      AccountHeadName = receipthead.RECIEPT_HEAD_NAME,
                                      AccountSubHeadName = receiptsubhead.RECEIPT_SUB_HEAD1,
                                      ChallanTransID = rd.Id,
                                      Amount = rd.Amount.Value

                                  }).ToList();
                return objAccount.ToDataSourceResult(request);
            }
        }
        public DataSourceResult GetManageAccountDetails(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var objAccount = (from rd in dbContext.Challan_Master
                                  join am in dbContext.BankMsts on rd.Bank_Id equals am.bankId
                                  join sm in dbContext.BranchMsts on rd.Branch_Id equals sm.branchId
                                  join aa in dbContext.AllotmentMasters on rd.Rid equals aa.rid
                                  join dpt in dbContext.DepartmentMsts on aa.departmentId equals dpt.departmentId
                                  // where rd.Is_Active == true
                                  //where rd.Is_Active  == true && rd.Rid=rId
                                  select new BankAccountManagementModel
                                  {
                                      RId = rd.Rid,
                                      BankName = am.bankName,
                                      BranchName = sm.branchName,
                                      ChallanNo = rd.Challan_Id,
                                      ChallanDate = rd.Created_Date,
                                      DepttName = dpt.departmentName,
                                      IsActive = rd.Is_Active

                                  }).ToList();
                return objAccount.ToDataSourceResult(request);
            }
        }
        public string GetAccountNumber(int bankId, int branchId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingRequest = (from m in dbContext.BranchMsts where m.bankId == bankId && m.branchId == branchId && m.IsActive == true select m.accountNumber).FirstOrDefault();
                // var accountnumbers =dbContext .BranchMsts .Where (m=>m.bankId ==bankId && m.branchId ==branchId && m.IsActive ==true SelectionType. )
                //var accountnumbers = (from s in dbContext.BranchMsts 
                //                      where s.bankId == bankId && s.branchId == branchId
                //                      select new DDLStringList
                //                      {
                //                          id = s.accountNumber,
                //                          text = s.accountNumber
                //                      }).ToList();
                return existingRequest;
            }
        }
        public List<DDList> GetRIDsForManageAccountDetails()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var objRid = (from comRid in dbContext.AllotmentMasters
                              where comRid.isActive == 1 && comRid.isStatus.ToLower() == AllotmentStatus.Approved.ToString().ToLower() && loginUserDeptt.Contains(comRid.departmentId)
                              orderby comRid.rid descending
                              select new DDList
                              {
                                  id = comRid.rid,
                                  text = comRid.rid.ToString()
                              }).ToList();

                return objRid;
            }
        }
        public DataSourceResult GetRIDsForManageAccountDetailsByDataSource(DataSourceRequest Req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var objRid = (from comRid in dbContext.AllotmentMasters
                              where comRid.isActive == 1 && comRid.isStatus.ToLower() == AllotmentStatus.Approved.ToString().ToLower() && loginUserDeptt.Contains(comRid.departmentId)
                              orderby comRid.rid descending
                              select new DDList
                              {
                                  id = comRid.rid,
                                  text = comRid.rid.ToString()
                              });

                return objRid.ToDataSourceResult(Req);
            }
        }
        /// <summary>
        /// Activate / Deactivate Roles
        /// </summary>
        /// <param name="ChallanNo">ChallanNo ID</param>
        /// <param name="status">Status</param>
        /// <returns></returns>
        public bool DeActivate(string ChallanNo, bool status, string viewName)
        {
            bool flag = false;
            using (var context = new NoidaPMSEntities())
            {
                var effectedRec = 0;
                var tblChallan = context.Challan_Master.FirstOrDefault(c => c.Challan_Id == ChallanNo);
                Challan_Master oldObjModel = new Challan_Master();//Old Model
                oldObjModel.Is_Active = tblChallan.Is_Active;
                if (tblChallan != null)
                {
                    tblChallan.Is_Active = !status;
                    tblChallan.Modified_By = userInfo.UserID;
                    tblChallan.Modified_Date = DateTime.Now;

                    context.SaveChanges();
                    flag = true;
                    Challan_Master oldObjModel1 = new Challan_Master();//Old Model
                    oldObjModel1.Is_Active = tblChallan.Is_Active;

                }
                return flag;
            }
        }
        public string ChallanGenerate(string ChallanId, int rid)
        {
            string strLettter = string.Empty;
            var challan = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                //var Objdeptt = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == rid && m.isActive == 1);
                //if (Objdeptt != null)
                //{
                //    strLettter = CommonMethords.GenerateLetter(Objdeptt.rid, Constants.BankChallanGenerate, Objdeptt.departmentId.Value, userInfo.UserID);
                //}
                challan = dbContext.Challan_Master.Where(m => m.Challan_Id == ChallanId && m.Rid == rid).Select(m => m.Content).FirstOrDefault();

            }
            //return strLettter;
            return challan;
        }


        // return bank names for payment
        public List<DynamicDataModel> GetBankNamesForPayment()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var bankNames = (from bank in dbContext.BankMsts
                                 where bank.IsActive == true
                                 select new DynamicDataModel
                                 {
                                     Name = bank.bankName,
                                     Value = bank.bankId
                                 }).ToList();
                return bankNames;
            }
        }

        /// <summary>
        /// Fetches RIDs for Lease Deed
        /// </summary>
        /// <returns></returns>
        public DataSourceResult GetRIDsForLeaseDeed(DataSourceRequest Request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstDeptts = (from u in dbContext.UmUserMasters
                                 join d in dbContext.UmUserDepartmentTrans on u.UserRefId equals d.UserRefId
                                 where u.UserRefId == userInfo.UserID
                                 select d.DepartmentId).ToList();
                var lst = (from f in dbContext.AllotmentMasters
                           join spt in dbContext.SchemePropTrans on f.propertyId equals spt.propertyId
                           where f.isActive == 1 && lstDeptts.Contains(f.departmentId) && f.isStatus.ToLower() == Common.AllotmentStatus.Approved.ToString().ToLower() && spt.ParentPropertyId == null
                           orderby f.createdDate descending
                           select new DDList
                           {
                               id = f.rid,
                               text = f.rid.ToString()
                           });
                return lst.ToDataSourceResult(Request);
            }
        }

        /// <summary>
        /// Fetches RIDs for Sublease Deed
        /// </summary>
        /// <returns></returns>
        public List<DDList> GetRIDsForSubleaseDeed()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstDeptts = (from u in dbContext.UmUserMasters
                                 join d in dbContext.UmUserDepartmentTrans on u.UserRefId equals d.UserRefId
                                 where u.UserRefId == userInfo.UserID
                                 select d.DepartmentId).ToList();
                var lst = (from f in dbContext.AllotmentMasters
                           join spt in dbContext.SchemePropTrans on f.propertyId equals spt.propertyId
                           where f.isActive == 1 && lstDeptts.Contains(f.departmentId) && f.isStatus.ToLower() == Common.AllotmentStatus.Approved.ToString().ToLower() && spt.ParentPropertyId != null
                           orderby f.createdDate descending
                           select new DDList
                           {
                               id = f.rid,
                               text = f.rid.ToString()
                           }).ToList();
                return lst;
            }
        }

        public List<ChallanModel> GetModelListToPrintExtensionChallan(List<int> requestNoList, List<int> ridList)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var challanDetail = (from ext in dbContext.Extension_Details
                                     join alot in dbContext.AllotmentMasters on ext.Rid equals alot.rid
                                     join scpt in dbContext.SchemePropTrans on alot.propertyId equals scpt.propertyId
                                     where requestNoList.Contains(ext.Id)
                                     select new ChallanModel
                                     {
                                         RID = alot.rid,
                                         FormNo = alot.formNo,
                                         SectorName = scpt.SectorMst.sectorName,
                                         BlockName = scpt.BlockMst.blockName,
                                         PropertyTypeName = scpt.PropertyTypeMst.propertyTypeName,
                                         PropertyNumber = scpt.propertyNo,
                                         AllotmentMoney = scpt.allotmentMoney,
                                         ApplicationForm = new ApplicationFormModel
                                         {
                                             FirstName = alot.ApplicationDetail.tFirstName,
                                             MiddleName = alot.ApplicationDetail.tMiddleName,
                                             LastName = alot.ApplicationDetail.tLastName,
                                             CorrespondingAddress = alot.ApplicationDetail.tCorrespondanceAdd,
                                             MobileNumber = alot.ApplicationDetail.tMobileNumber,
                                             PhoneNumber = alot.ApplicationDetail.tPhoneNumber,
                                             Email = alot.ApplicationDetail.tEmail
                                         },
                                     }).ToList();
                return challanDetail;
            }
        }

        //generate letter for rent permission of industrial/institutional type
        public string GenerateRentPermissionLetter(int requestNo, int rid)
        {
            string strLettter = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                var property = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == rid);
                if (property != null)
                {
                    //strLettter = CommonMethords.GenerateLetter(Objdeptt.rid, Constants.MortgageTemplateID, Objdeptt.departmentId.Value, userInfo.UserID);
                    strLettter = GenerateLetterFromDb(property.rid, Constants.RentPermissionTemplateID, property.departmentId.Value);
                }
            }
            return strLettter;
        }

        public List<ChallanModel> GetModelListToPrintCICChallan(int idList, int directorID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var challanDetail = (from ext in dbContext.Director_Request_Master
                                     join alot in dbContext.AllotmentMasters on ext.Rid equals alot.rid
                                     join scpt in dbContext.SchemePropTrans on alot.propertyId equals scpt.propertyId
                                     where ext.Rid.Value == idList && ext.Id == directorID && ext.Is_Active == 1
                                     select new ChallanModel
                                     {
                                         RID = alot.rid,
                                         FormNo = alot.formNo,
                                         SectorName = scpt.SectorMst.sectorName,
                                         BlockName = scpt.BlockMst.blockName,
                                         PropertyTypeName = scpt.PropertyTypeMst.propertyTypeName,
                                         PropertyNumber = scpt.propertyNo,
                                         AllotmentMoney = scpt.allotmentMoney,
                                         ApplicationForm = new ApplicationFormModel
                                         {
                                             FirstName = alot.ApplicationDetail.tFirstName,
                                             MiddleName = alot.ApplicationDetail.tMiddleName,
                                             LastName = alot.ApplicationDetail.tLastName,
                                             CorrespondingAddress = alot.ApplicationDetail.tCorrespondanceAdd,
                                             MobileNumber = alot.ApplicationDetail.tMobileNumber,
                                             PhoneNumber = alot.ApplicationDetail.tPhoneNumber,
                                             Email = alot.ApplicationDetail.tEmail
                                         },
                                     }).ToList();
                return challanDetail;
            }
        }

        public List<ChallanModel> GetModelListToPrintCICFirmnProductChallan(int idList, int directorID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var challanDetail = (from ext in dbContext.Firm_Master
                                     join alot in dbContext.AllotmentMasters on ext.Rid equals alot.rid
                                     join scpt in dbContext.SchemePropTrans on alot.propertyId equals scpt.propertyId
                                     where ext.Rid.Value == idList && ext.Id == directorID && ext.Is_Active == 1
                                     select new ChallanModel
                                     {
                                         RID = alot.rid,
                                         FormNo = alot.formNo,
                                         SectorName = scpt.SectorMst.sectorName,
                                         BlockName = scpt.BlockMst.blockName,
                                         PropertyTypeName = scpt.PropertyTypeMst.propertyTypeName,
                                         PropertyNumber = scpt.propertyNo,
                                         AllotmentMoney = scpt.allotmentMoney,
                                         ApplicationForm = new ApplicationFormModel
                                         {
                                             FirstName = alot.ApplicationDetail.tFirstName,
                                             MiddleName = alot.ApplicationDetail.tMiddleName,
                                             LastName = alot.ApplicationDetail.tLastName,
                                             CorrespondingAddress = alot.ApplicationDetail.tCorrespondanceAdd,
                                             MobileNumber = alot.ApplicationDetail.tMobileNumber,
                                             PhoneNumber = alot.ApplicationDetail.tPhoneNumber,
                                             Email = alot.ApplicationDetail.tEmail
                                         },
                                     }).ToList();
                return challanDetail;
            }
        }


        public FunctionalModel GetFunctionalDetailByRid(int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from funtional in dbContext.FunctionalDetails
                            join allotment in dbContext.AllotmentMasters on funtional.Rid equals allotment.rid
                            join property in dbContext.SchemePropTrans on allotment.propertyId equals property.propertyId
                            join status in dbContext.StatusMasters on funtional.StatusId equals status.Id
                            where funtional.Rid == rid
                            select new FunctionalModel
                            {
                                RId = funtional.Rid,
                                PropertyNumber = funtional.PropertyNumber,
                                SchemeName = property.SchemeMst.schemeName,
                                DepttName = property.DepartmentMst.departmentName,
                                ApplicationName = allotment.ApplicationDetail.tFirstName + " " + allotment.ApplicationDetail.tMiddleName + " " + allotment.ApplicationDetail.tLastName,
                                Gender = allotment.ApplicationDetail.tGender,
                                RelationName = allotment.ApplicationDetail.tFatherHusbandName,
                                PropertyType = property.PropertyTypeMst.propertyTypeName,
                                Area = property.totalArea,
                                //Floor = fm.floorName,
                                Status = status.Status,
                                FunctionalDueDate = funtional.FunctionalDueDate,
                                Affidavit = funtional.AffidavitFlag.Value,
                                MeterSealing = funtional.MeterSeallingDocFlag.Value,
                                NDCAccount = funtional.NOCAccountFlag.Value,
                                RegistrationCertificate = funtional.RegistrationCertiFlag.Value,
                                FunctionalDate = funtional.FunctionalDate,
                                Comment = funtional.Comment,
                                FunctionalCharge = funtional.FunctionalCharge,
                                From = dbContext.UmUserMasters.Where(x => x.UserRefId == funtional.Approver).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault(),
                                RequestNo = funtional.RequestNo,
                                ApproveDate = funtional.ApproveDate
                            }).FirstOrDefault();

                return data;
            }
            //throw new NotImplementedException();
        }


        public List<DDList> GetRidToSearchNoting()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var objRid = (from noting in dbContext.Noting_File_Master
                              where noting.Is_Active == true && loginUserDeptt.Contains(noting.Department_Id)
                              select new DDList
                              {
                                  id = noting.Rid.Value,
                                  text = noting.Rid.ToString()
                              }).ToList();

                return objRid;
            }
        }


        public ChallanModel GenerateChallan(int rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId)
        {
            List<BankAccountManagementModel> modelList = (List<BankAccountManagementModel>)HttpContext.Current.Session["TempModel"];
            if (modelList != null)
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    var challanDetail = (from alot in dbContext.AllotmentMasters
                                         join scpt in dbContext.SchemePropTrans on alot.propertyId equals scpt.propertyId
                                         where alot.rid == rId
                                         select new ChallanModel
                                         {
                                             RID = alot.rid,
                                             FormNo = alot.formNo,
                                             SectorName = scpt.SectorMst.sectorName,
                                             BlockName = scpt.BlockMst.blockName,
                                             PropertyTypeName = scpt.PropertyTypeMst.propertyTypeName,
                                             PropertyNumber = scpt.propertyNo,
                                             AllotmentMoney = scpt.allotmentMoney,

                                             ApplicationForm = new ApplicationFormModel
                                             {
                                                 FirstName = alot.ApplicationDetail.tFirstName,
                                                 MiddleName = alot.ApplicationDetail.tMiddleName,
                                                 LastName = alot.ApplicationDetail.tLastName,
                                                 CorrespondingAddress = alot.ApplicationDetail.tCorrespondanceAdd,
                                                 MobileNumber = alot.ApplicationDetail.tMobileNumber,
                                                 PhoneNumber = alot.ApplicationDetail.tPhoneNumber,
                                                 Email = alot.ApplicationDetail.tEmail
                                             },

                                             BankName = dbContext.BankMsts.Where(b => b.bankId == bankId).Select(m => m.bankName).FirstOrDefault(),
                                         }).FirstOrDefault();

                    int count = 1;
                    foreach (var model in modelList)
                    {
                        if (count == 1)
                        {
                            challanDetail.Amount1 = model.Amount;
                            challanDetail.AccountHeadName1 = model.AccountHeadName;
                            challanDetail.AccountSubHeadName1 = model.AccountHeadName;
                        }
                        if (count == 2)
                        {
                            challanDetail.Amount2 = model.Amount;
                            challanDetail.AccountHeadName2 = model.AccountHeadName;
                            challanDetail.AccountSubHeadName2 = model.AccountHeadName;
                        }
                        if (count == 3)
                        {
                            challanDetail.Amount3 = model.Amount;
                            challanDetail.AccountHeadName3 = model.AccountHeadName;
                            challanDetail.AccountSubHeadName3 = model.AccountHeadName;
                        }
                        count++;
                    }
                    return challanDetail;
                }
            }
            else
            {
                return null;
            }
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
        public bool SaveGeneratedChallan(int rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId, string parsedHTML)
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
                    //var objUserList = dbContext.UmUserMasters.Where(i => i.UserName == user).FirstOrDefault();
                    objAllotteeLsts.Select(ua =>
                    {
                        ua.Challan_Master_Id = objExit.Id;
                        //ua.submitDate = DateTime.Now;
                        //ua.submittedBy = userid;
                        //ua.approverBy = objUserList.UserRefId.ToString();
                        //ua.isSubmitted = "1";
                        return ua;
                    }).ToList();
                    dbContext.SaveChanges();
                }
                //flag = "1";
                //flag = CommonMethords.GenerateLetter(rId, 2, 5, userInfo.UserID);
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

        public List<BankAccountManagementModel> GetGeneratedChallanDetails(int rid)
        {
            List<BankAccountManagementModel> data = (List<BankAccountManagementModel>)HttpContext.Current.Session["TempModel"];
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
            //using (var dbContext = new NoidaPMSEntities())
            //{
            //    var challanDetails = (from challan in dbContext.Challan_Master
            //                      join challanTrans in dbContext.Challan_Trans on challan.Id equals challanTrans.Challan_Master_Id
            //                      join receiptHead in dbContext.RECIEPT_HEAD on challanTrans.Head_Id equals receiptHead.RECIEPT_CODE
            //                      join receiptSubHead in dbContext.RECEIPT_SUB_HEAD on challanTrans.Subhead_Id equals receiptSubHead.RECEIPT_SUBHEAD_ID
            //                      where challan.Is_Active == true && challan.Rid == rid // && challan.Challan_Master_Id == null
            //                      select new BankAccountManagementModel
            //                      {
            //                          ChallanId = challan.Id,
            //                          RId = challan.Rid,
            //                          AccountHeadName = receiptHead.RECIEPT_HEAD_NAME,
            //                          AccountSubHeadName = receiptSubHead.RECEIPT_SUB_HEAD1,
            //                          ChallanTransID = challanTrans.Id,
            //                          Amount = challanTrans.Amount.Value,
            //                          CreatedDate = challan.Created_Date,
            //                          BankName = challan.BranchMst.BankMst.bankName
            //                      }).ToList();
            //    return challanDetails; //.ToDataSourceResult(request);
            //}
        }


        public ChallanModel GeneratePaymentChallan(int rid, int bankId, int branchId, string accountNumber, int? deptId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var challanIdPK = 0;
                List<BankAccountManagementModel> modelList = (List<BankAccountManagementModel>)HttpContext.Current.Session["TempModel"];
                if (modelList != null)
                {
                    Challan_Master challanMaster = new Challan_Master();
                    challanMaster.Rid = rid;
                    challanMaster.Bank_Id = bankId;
                    challanMaster.Branch_Id = branchId;
                    challanMaster.Account_Number = accountNumber;
                    //challanMaster.Content = challan;
                    challanMaster.Created_Date = DateTime.Now;
                    challanMaster.Generate_Date = DateTime.Now;
                    challanMaster.Created_By = userInfo.UserID;
                    challanMaster.Is_Active = true;
                    dbContext.Challan_Master.Add(challanMaster);
                    dbContext.SaveChanges();

                    var challanIdPK = dbContext.Challan_Master.Max(m => m.Id);
                    //var challanId = dbContext.Challan_Master.Where(c => c.Id == challanIdPK).Select(i => i.Challan_Id).FirstOrDefault();
                    foreach (var model in modelList)
                    {
                        Challan_Trans trans = new Challan_Trans();
                        trans.Rid = rid;
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
                    challanModel.TotalHeadAmount = 0;
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
                        challanModel.TotalHeadAmount = challanModel.TotalHeadAmount + model.Amount;
                        count++;
                    }
                    challanModel.ModelHeadProperty = modelList;
                    return challanModel;
                }
                else
                {
                    return null;
                }
            }
        }



        public bool RemoveChallanChargeDetail(int rid, string headName, string subHeadName, decimal amount)
        {
            var flag = false;
            List<BankAccountManagementModel> modelList = (List<BankAccountManagementModel>)HttpContext.Current.Session["TempModel"];
            if (modelList != null)
            {
                foreach (var model in modelList)
                {
                    if (model.RId == rid && model.AccountHeadName == headName && model.AccountSubHeadName == subHeadName && model.Amount == amount)
                    {
                        modelList.Remove(model);
                        return flag = true;
                    }
                }
            }
            return flag;
        }


        public string SaveGeneratedChallanByRid(int rid, int bankId, int branchId, string accountNumber, string challan)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<BankAccountManagementModel> modelList = (List<BankAccountManagementModel>)HttpContext.Current.Session["TempModel"];
                if (modelList != null)
                {
                    Challan_Master challanMaster = new Challan_Master();
                    challanMaster.Rid = rid;
                    challanMaster.Bank_Id = bankId;
                    challanMaster.Branch_Id = branchId;
                    challanMaster.Account_Number = accountNumber;
                    challanMaster.Content = challan;
                    challanMaster.Created_Date = DateTime.Now;
                    challanMaster.Generate_Date = DateTime.Now;
                    challanMaster.Created_By = userInfo.UserID;
                    challanMaster.Is_Active = true;
                    dbContext.Challan_Master.Add(challanMaster);
                    dbContext.SaveChanges();

                    //var lastChallan = dbContext.Challan_Master.Where(c => c.Rid == rid).FirstOrDefault();
                    var challanId = dbContext.Challan_Master.Max(m => m.Id);

                    foreach (var model in modelList)
                    {
                        Challan_Trans trans = new Challan_Trans();
                        trans.Rid = rid;
                        trans.Challan_Master_Id = challanId;
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
                    return challan;
                }
                else
                {
                    return null;
                }
            }
        }




        public int UpdateNotingForAllottedProperty(int Rid, string user, string notingDetails)
        {
            int result = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                var noting = dbContext.Noting_File_Master.Where(n => n.Rid == Rid).FirstOrDefault();

                if (noting != null)
                {
                    var exnotes = dbContext.Noting_File_Trans.Where(n => n.Noting_File_Id == noting.Id).FirstOrDefault();
                    if (exnotes != null)
                    {
                        string noteContent = exnotes.Noting_Details;
                        exnotes.Noting_Details = noteContent + "<br>" + notingDetails + "<br>" + userInfo.FirstName + " " + userInfo.MiddleName + " " + userInfo.LastName + "<br>" + DateTime.Now.ToString() + "<br>";
                        exnotes.Note_By = userInfo.UserID;
                        exnotes.Noting_Date = DateTime.Now;
                        exnotes.Is_Active = true;
                        exnotes.Modified_By = userInfo.UserID;
                        exnotes.Modified_Date = DateTime.Now;
                        dbContext.SaveChanges();
                        result = 2;
                    }
                    else
                    {
                        Noting_File_Trans notes = new Noting_File_Trans();
                        notes.Noting_File_Id = noting.Id;
                        notes.Noting_Details = notingDetails + "<br>" + userInfo.FirstName + " " + userInfo.MiddleName + " " + userInfo.LastName + "<br>" + DateTime.Now.ToString() + "<br>";
                        notes.Note_By = userInfo.UserID;
                        notes.Noting_Date = DateTime.Now;
                        notes.Is_Active = true;
                        notes.Created_By = userInfo.UserID;
                        notes.Created_Date = DateTime.Now;
                        dbContext.Noting_File_Trans.Add(notes);
                        dbContext.SaveChanges();
                        result = 1;
                    }
                }
            }
            return result;
        }

        public bool SaveGeneratedChallanForServiceRequest(string challanId, string challan)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var challanDetail = dbContext.Challan_Master.Where(c => c.Challan_Id == challanId).FirstOrDefault();
                challanDetail.Content = challan;
                dbContext.SaveChanges();

                var serviceRequest = dbContext.Customer_ServiceRequest.Where(s => s.Registration_No == challanDetail.Rid.ToString()).FirstOrDefault();
                //serviceRequest.ChallanId = Convert.ToInt32(challanId);
                ////dbContext.SaveChanges();
                //var sflag = SaveServiceRequestId(challanId, challanDetail.Rid.ToString());
                var sflag = SaveServiceRequestId(challanDetail.Id.ToString(), challanDetail.Rid.ToString());
                var rflag = SaveChallanIdToReqeustService(challanId, serviceRequest.ServiceId);
                //challanDetail.ServiceRequestNo = serviceRequest.ServiceId;
                //dbContext.SaveChanges();
                flag = true;
            }
            return flag;
        }

        private bool SaveServiceRequestId(string challanId, string rid)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var serviceRequest = dbContext.Customer_ServiceRequest.Where(s => s.Registration_No == rid).FirstOrDefault();
                serviceRequest.ChallanId = Convert.ToInt32(challanId);
                dbContext.SaveChanges();
                flag = true;
            }
            return flag;
        }

        private bool SaveChallanIdToReqeustService(string challanId, int? serviceId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var challanDetail = dbContext.Challan_Master.Where(c => c.Challan_Id == challanId).FirstOrDefault();
                challanDetail.ServiceRequestNo = serviceId;
                dbContext.SaveChanges();
                flag = true;
            }
            return flag;
        }


        public DataSourceResult GetRIDsForFunctional_Read(DataSourceRequest Req, int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {

                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      && (dept.DepartmentId == (int)Departmentenum.Commercial || dept.DepartmentId == (int)Departmentenum.Industrial || dept.DepartmentId == (int)Departmentenum.Institutional)
                                      //&& Dept.Contains (dept.DepartmentId )
                                      select dept.DepartmentId).ToList();

                var objRid = (from comRid in dbContext.PossessionDetails
                              join am in dbContext.AllotmentMasters on comRid.Rid equals am.rid
                              where comRid.IsActive == true && am.isActive == 1 && loginUserDeptt.Contains(am.departmentId) && (rid == null || am.rid == rid)
                              select new DDList
                              {
                                  id = comRid.Rid.Value,
                                  text = comRid.Rid.ToString()
                              });

                return objRid.ToDataSourceResult(Req);
            }
        }


        public DataSourceResult GetRentPermissionByRid(DataSourceRequest request, int Rid)
        {
            if (Rid != 0)
            {


                using (var dbContext = new NoidaPMSEntities())
                {
                    var rentrequest = (from rent in dbContext.RentPermissionDetails
                                       where rent.Rid == Rid && rent.IsActive == true
                                       select new RentdetailShowModel
                                    {
                                        RequestNo = rent.RequestNo,
                                        TenantName = rent.TenantName,
                                        TenantProject = rent.TenantProject,
                                        RentingDate = rent.RentingDate,
                                        RentDuration = rent.RentDurationYears,
                                        Area = rent.Area,
                                        RequestStatus = rent.StatusMaster.Status
                                    });
                    return rentrequest.ToDataSourceResult(request);
                }
            }
            else
            {
                return null;
            }
        }


        public int RemoveRentRequestDetailsById(RentPermissionModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var flag = ReturnType.None;
                var data = dbContext.RentPermissionDetails.FirstOrDefault(r => r.RequestNo == model.RequestNo && r.IsActive == true);
                if (data != null)
                {
                    data.IsActive = false;
                    dbContext.SaveChanges();
                    flag = ReturnType.Removed;
                }
                return flag;
            }
        }
    }
}

