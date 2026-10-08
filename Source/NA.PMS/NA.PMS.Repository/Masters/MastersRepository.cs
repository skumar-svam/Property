using Kendo.Mvc.UI;

using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Common;
using System.Data.Entity.Core.Objects;
using NA.PMS.Web.Models;
using System.Data.Entity.SqlServer;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.IO;

namespace NA.PMS.Repository
{
    public class MastersRepositiory : IMastersRepositiory
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public MastersRepositiory()
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
                            DepartmentList = dbContext.UmUserDepartmentTrans.Where(x => x.UserRefId == userInfo.UserID && x.Status == true).Select(d => d.DepartmentId).ToList();
                        }
                    }
                }
            }
        }

        #region Scheme Refunds by Shatrughna
        /// <summary>
        /// Get all schemes for refund
        /// </summary>
        /// <returns></returns>
        public List<SchemeModel> GetAllSchemesForRefund()
        {
            List<SchemeModel> schemeList = new List<SchemeModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                schemeList = (from schemes in dbContext.SchemeMsts
                              where schemes.IsActive == true //&& schemes.completed == true && schemes.Status != Constants.SchemeClosed
                              select new SchemeModel
                              {
                                  schemeId = schemes.schemeId,
                                  schemeName = schemes.schemeName,
                                  schemeTypeId = schemes.schemeTypeId,
                                  startDate = schemes.startDate,
                                  endDate = schemes.endDate,
                                  IsActive = schemes.IsActive,
                                  createdDate = schemes.createdDate,
                                  modifiedDate = schemes.modifiedDate,
                                  createdBy = schemes.createdBy
                              }).OrderByDescending(x => x.schemeId).ToList();
            }
            return schemeList;
        }
        /// <summary>
        /// Get media types for scheme refund
        /// </summary>
        /// <returns></returns>
        public List<MediaModel> GetMediaTypesForRefund()
        {
            var mediaTypeList = new List<MediaModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                mediaTypeList = (from mediaTypeMsts in dbContext.MediaTypeMsts
                                 where mediaTypeMsts.IsActive == true
                                 select new MediaModel
                                 {
                                     mediaTypeId = mediaTypeMsts.mediaTypeId,
                                     mediaType = mediaTypeMsts.mediaType
                                 }).ToList();
                return mediaTypeList;
            }
        }
        /// <summary>
        /// Get department for refund
        /// </summary>
        /// <returns></returns>
        public List<DepartmentRefund> GetDepartmentForRefund()
        {
            List<DepartmentRefund> departments = null;
            using (var dbContext = new NoidaPMSEntities())
            {
                departments = (from dept in dbContext.DepartmentMsts
                               select new DepartmentRefund
                               {
                                   DepartmentId = dept.departmentId,
                                   DepartmentName = dept.departmentName,
                                   IsActive = dept.IsActive
                               }).ToList();
            }
            return departments;
        }

        /// <summary>
        /// return department based on scheme id for dropdown  addrefund page
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public List<DepartmentRefund> GetDepartmentOnSchemeForRefund(int schemeId)
        {
            List<DepartmentRefund> departments = null;
            using (var dbContext = new NoidaPMSEntities())
            {
                departments = (from dept in dbContext.DepartmentMsts
                               join sdt in dbContext.SchemeDepartmentTrans on dept.departmentId equals sdt.departmentId
                               where sdt.schemeId == schemeId
                               select new DepartmentRefund
                               {
                                   DepartmentId = dept.departmentId,
                                   DepartmentName = dept.departmentName,
                                   IsActive = dept.IsActive
                               }).Distinct().ToList();
            }
            return departments;
        }

        /// <summary>
        /// return deduction apply on like total cost, civil cost
        /// </summary>
        /// <returns></returns>
        public List<DeductionApplyOnForRefund> GetDeductionApplyOnForRefund()
        {
            List<DeductionApplyOnForRefund> ddlist = new List<DeductionApplyOnForRefund>();
            foreach (int value in Enum.GetValues(typeof(DeductionApplyOn)))
            {
                ddlist.Add(new DeductionApplyOnForRefund
                {
                    TypeName = Enum.GetName(typeof(DeductionApplyOn), value),
                    TypeValue = Enum.GetName(typeof(DeductionApplyOn), value),
                    TypeId = value
                });
            }
            return ddlist;
        }
        /// <summary>
        /// return unit like INR for rupees and P for percent
        /// </summary>
        /// <returns></returns>
        public List<UnitTypeForRefund> GetUnitTypeForRefund()
        {
            List<UnitTypeForRefund> unitList = new List<UnitTypeForRefund>();
            foreach (var unit in Enum.GetNames(typeof(Unit)))
            {
                UnitTypeForRefund unittype = new UnitTypeForRefund();
                if (unit == "P")
                {
                    unittype.UnitName = "PER";
                    unittype.UnitValue = unit;
                    unitList.Add(unittype);
                }
                else
                {
                    unittype.UnitName = unit;
                    unittype.UnitValue = unit;
                    unitList.Add(unittype);
                }
            }
            //foreach (int value in Enum.GetValues(typeof(Unit)))
            //{
            //    unitList.Add(new UnitTypeForRefund
            //    {
            //        UnitName = Enum.GetName(typeof(Unit), value),
            //        UnitValue = Enum.GetName(typeof(Unit), value),
            //        UnitId = value
            //    });
            //}
            return unitList;
        }

        /// <summary>
        /// Get all refunds details to manage 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DataSourceResult GetSchemeRefundDetails(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //List<Refund> refundList = new List<Refund>();
                var refundList = (from refunds in dbContext.SchemeRefundTrans
                                  join schemes in dbContext.SchemeMsts on refunds.schemeId equals schemes.schemeId
                                  join dept in dbContext.DepartmentMsts on refunds.departmentId equals dept.departmentId
                                  where refunds.IsActive == true && schemes.Status != Constants.SchemeClosed
                                  select new Refund
                                  {
                                      RefundId = refunds.refundId,
                                      SchemeId = refunds.schemeId,
                                      SchemeName = schemes.schemeName,
                                      DepartmentId = dept.departmentId,
                                      DepartmentName = dept.departmentName,
                                      RefundDescription = refunds.refundDescription,
                                      Deduction = refunds.deduction.Value,
                                      DeductionApplyOn = refunds.deductionApplyOn,
                                      RefundLockPeriod = refunds.refundLockPeriod,
                                      //MediaTypeid = refunds.mediaTypeId,
                                      Unit = refunds.unit,
                                      DaysAfterInterest = refunds.daysAfterInterest,
                                      CreatedBy = refunds.createdBy,
                                      CreatedDate = refunds.createdDate,
                                      ModifiedBy = refunds.modifiedBy,
                                      ModifiedDate = refunds.modifiedDate,
                                      IsActive = refunds.IsActive,
                                      //EncodedParameter = refunds.refundId.ToString()
                                  });
                return refundList.ToDataSourceResult(request);
            }
        }
        /// <summary>
        /// Get all information of particular refund
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Refund GetRefundDetailById(int id)
        {
            Refund rfnd = null;
            using (var dbContext = new NoidaPMSEntities())
            {
                rfnd = (from refunds in dbContext.SchemeRefundTrans
                        join schemes in dbContext.SchemeMsts on refunds.schemeId equals schemes.schemeId
                        join dept in dbContext.DepartmentMsts on refunds.departmentId equals dept.departmentId
                        //join media in dbContext.MediaTypeMsts on refunds.mediaTypeId equals media.mediaTypeId
                        where refunds.refundId == id
                        select new Refund
                        {
                            RefundId = refunds.refundId,
                            SchemeId = refunds.schemeId,
                            SchemeName = schemes.schemeName,
                            DepartmentId = dept.departmentId,
                            DepartmentName = dept.departmentName,
                            //MediaTypeid = refunds.mediaTypeId,
                            //MediaTypeName = media.mediaType,
                            Deduction = refunds.deduction,
                            InterestRate = refunds.interest,
                            DeductionApplyOn = refunds.deductionApplyOn,
                            RefundDescription = refunds.refundDescription,
                            RefundLockPeriod = refunds.refundLockPeriod,
                            Unit = refunds.unit,
                            CreatedBy = refunds.createdBy,
                            CreatedDate = refunds.createdDate,
                            DaysAfterInterest = refunds.daysAfterInterest,
                            ModifiedBy = refunds.modifiedBy,
                            ModifiedDate = refunds.modifiedDate,
                            IsActive = refunds.IsActive
                        }).FirstOrDefault();

            }
            return rfnd;
        }

        /// <summary>
        /// Add and Edit refund
        /// </summary>
        /// <param name="refund"></param>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        public bool SaveRefundForScheme(Refund refund, string loginUser)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingRefund = dbContext.SchemeRefundTrans.FirstOrDefault(cond => cond.refundId == refund.RefundId);

                if (existingRefund != null)
                {
                    existingRefund.refundDescription = refund.RefundDescription;
                    existingRefund.refundLockPeriod = refund.RefundLockPeriod;
                    //existingRefund.mediaTypeId = refund.MediaTypeid;
                    existingRefund.unit = refund.Unit;
                    existingRefund.deduction = refund.Deduction;
                    existingRefund.deductionApplyOn = refund.DeductionApplyOn;
                    existingRefund.interest = refund.InterestRate;
                    existingRefund.daysAfterInterest = refund.DaysAfterInterest;
                    existingRefund.modifiedBy = loginUser;
                    existingRefund.modifiedDate = DateTime.Now;

                    dbContext.SaveChanges();
                    flag = true;
                }
                else
                {
                    SchemeRefundTran srt = new SchemeRefundTran();
                    srt.schemeId = refund.SchemeId;
                    srt.departmentId = refund.DepartmentId;
                    srt.refundDescription = refund.RefundDescription;
                    srt.refundLockPeriod = refund.RefundLockPeriod;
                    //srt.mediaTypeId = refund.MediaTypeid;
                    srt.deduction = refund.Deduction;
                    srt.unit = refund.Unit;
                    srt.deductionApplyOn = refund.DeductionApplyOn;
                    srt.interest = refund.InterestRate;
                    srt.daysAfterInterest = refund.DaysAfterInterest;
                    srt.IsActive = true;
                    srt.createdDate = DateTime.Now;
                    srt.createdBy = loginUser;

                    dbContext.SchemeRefundTrans.Add(srt);
                    dbContext.SaveChanges();
                    flag = true;
                }

            }

            return flag;
        }
        /// <summary>
        /// inactivate particular refund
        /// </summary>
        /// <param name="refundId"></param>
        /// <returns></returns>
        public bool RemoveRefundForScheme(int refundId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var refnd = dbContext.SchemeRefundTrans.FirstOrDefault(cond => cond.refundId == refundId);
                refnd.IsActive = false;
                dbContext.SaveChanges();
                flag = true;
            }
            return flag;
        }

        #endregion

        #region property

        /// <summary>
        /// Getting all Properties Detail
        /// </summary>
        /// <returns></returns>

        public List<PropertyModel> GetPropertyDetail()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var propDetail = new List<PropertyModel>();

                propDetail = (from schemePropTrans in dbContext.SchemePropTrans
                              join departmentMsts in dbContext.DepartmentMsts on schemePropTrans.departmentId equals departmentMsts.departmentId
                              join propertyTypeMsts in dbContext.PropertyTypeMsts on schemePropTrans.propertyTypeId equals propertyTypeMsts.propertyTypeId
                              join sectorMsts in dbContext.SectorMsts on schemePropTrans.sectorId equals sectorMsts.sectorId
                              join blockMsts in dbContext.BlockMsts on schemePropTrans.blockId equals blockMsts.blockId
                              join floorMsts in dbContext.FloorMsts on schemePropTrans.floorId equals floorMsts.floorId
                              join propertyLocationChargesTrans in dbContext.PropertyLocationChargesTrans on schemePropTrans.propertyId equals propertyLocationChargesTrans.propertyId
                              into prods
                              from x in prods.DefaultIfEmpty()
                              where schemePropTrans.IsActive == true && schemePropTrans.schemeId != null
                              select new PropertyModel
                              {
                                  schemeId = schemePropTrans.schemeId,
                                  schemeName = schemePropTrans.SchemeMst.schemeName,
                                  propertyTypeId = propertyTypeMsts.propertyTypeId,
                                  propertyType = propertyTypeMsts.propertyTypeName,
                                  sectorId = sectorMsts.sectorId,
                                  sectorName = sectorMsts.sectorName,
                                  blockId = blockMsts.blockId,
                                  blockName = blockMsts.blockName,
                                  departmentId = departmentMsts.departmentId,
                                  departmentName = departmentMsts.departmentName,
                                  floorId = floorMsts.floorId,
                                  floorName = floorMsts.floorName,
                                  createdBy = schemePropTrans.createdBy,
                                  lastModifiedDate = schemePropTrans.modifiedDate,
                                  plotProperty = schemePropTrans.propertyNo,
                                  refId = schemePropTrans.refId,
                                  totalArea = schemePropTrans.totalArea,
                                  coveredArea = schemePropTrans.coveredArea,
                                  actualArea = schemePropTrans.actualArea,
                                  charges = x.charges,
                                  locationId = x.locationId,
                                  RegistryName = schemePropTrans.Registry,
                                  TotalPropertyCost = schemePropTrans.totalPropertyCost,
                                  LandRatePerSqMet = schemePropTrans.landRatePerSqmt,
                                  SchemeStatus = schemePropTrans.SchemeMst.Status,
                                  locationName = dbContext.LocationMsts.Where(p => p.locationId == x.locationId).Select(p => p.locationName).FirstOrDefault(),

                                  //applicationForms.gender.ToLower() == Constants.Male ? 1 : applicationForms.gender.ToLower() == Constants.Female ? 2 : 3,

                                  LandRate = schemePropTrans.departmentId == 5 ? dbContext.SchemeCostTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId && x.IsActive == true).Select(p => p.totalPropertyCost + p.civilCost).FirstOrDefault()
                                                                               : dbContext.SchemeCostTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId && x.IsActive == true).Select(p => p.landRatePerSqmt).FirstOrDefault(),

                                  allotementMoney = dbContext.SchemeCostTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
                                                                                    && p.propertyTypeId == schemePropTrans.propertyTypeId && p.sectorId == schemePropTrans.sectorId
                                                                                    && p.floorId == schemePropTrans.floorId).Select(p => p.allotmentMoney).FirstOrDefault(),
                                  leaseRent = dbContext.SchemeCostTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
                                                                                    && p.propertyTypeId == schemePropTrans.propertyTypeId && p.sectorId == schemePropTrans.sectorId
                                                                                    && p.floorId == schemePropTrans.floorId).Select(p => p.leaseRent).FirstOrDefault(),

                                  civilCost = dbContext.SchemeCostTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
                                                                                  && p.propertyTypeId == schemePropTrans.propertyTypeId && p.sectorId == schemePropTrans.sectorId
                                                                                  && p.floorId == schemePropTrans.floorId).Select(p => p.civilCost).FirstOrDefault(),

                                  allotementMoneyPercentage = dbContext.SchemeDepartmentTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
                                                                                                                                                && p.IsActive == true).Select(p => p.allotmentMoneyPercent).FirstOrDefault(),

                                  floorAreaRatio = dbContext.SchemeDepartmentTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
                                                                                            && p.IsActive == true).Select(p => p.far).FirstOrDefault(),

                                  leaseRentPercentage = dbContext.SchemeDepartmentTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
                                                                                    && p.IsActive == true).Select(p => p.leaseRentPercent).FirstOrDefault(),

                                  //LandRatePerSqMet = dbContext.SchemeCostTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
                                  //                                                  && p.propertyTypeId == schemePropTrans.propertyTypeId && p.sectorId == schemePropTrans.sectorId).Select(p => p.landRatePerSqmt).FirstOrDefault(),

                                  PropertyCost = dbContext.SchemeCostTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
                                                                                          && p.propertyTypeId == schemePropTrans.propertyTypeId && p.sectorId == schemePropTrans.sectorId
                                                                                         ).Select(p => p.totalPropertyCost).FirstOrDefault(),

                                  IsAllotted = dbContext.AllotmentMasters.Where(p => p.propertyId == schemePropTrans.propertyId && p.isStatus == AllotmentStatus.Approved.ToString()
                                                                                         ).Select(p => p.rid).FirstOrDefault(),
                                  ProjectId = schemePropTrans.groupProjectId,//Added on 21 Nov 2017 for group housing
                                  ParentPropertyId = schemePropTrans.ParentPropertyId

                              }).OrderByDescending(x => x.schemeName).ThenByDescending(s => s.sectorName).ThenByDescending(b => b.blockName).ThenByDescending(p => p.plotProperty).ToList();

                return propDetail;
            }
        }

        public DataSourceResult GetPropertyDetail_Read(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var departmentList = (from dept in dbContext.UmDepartmentMasters
                                      join udts in dbContext.UmUserDepartmentTrans on dept.DepartmentId equals udts.DepartmentId
                                      where udts.UserRefId == userInfo.UserID
                                      select dept.DepartmentId);

                var propDetail = (from schemePropTrans in dbContext.SchemePropTrans
                                  join departmentMsts in dbContext.DepartmentMsts on schemePropTrans.departmentId equals departmentMsts.departmentId
                                  join propertyTypeMsts in dbContext.PropertyTypeMsts on schemePropTrans.propertyTypeId equals propertyTypeMsts.propertyTypeId
                                  join sectorMsts in dbContext.SectorMsts on schemePropTrans.sectorId equals sectorMsts.sectorId
                                  join blockMsts in dbContext.BlockMsts on schemePropTrans.blockId equals blockMsts.blockId
                                  join floorMsts in dbContext.FloorMsts on schemePropTrans.floorId equals floorMsts.floorId
                                  join propertyLocationChargesTrans in dbContext.PropertyLocationChargesTrans on schemePropTrans.propertyId equals propertyLocationChargesTrans.propertyId
                                  into prods
                                  from x in prods.DefaultIfEmpty()
                                  where schemePropTrans.IsActive == true && schemePropTrans.schemeId != null && departmentList.Contains(schemePropTrans.departmentId.Value)
                                  select new PropertyModel
                                  {
                                      PropertyNo = schemePropTrans.propertyId,
                                      schemeId = schemePropTrans.schemeId,
                                      schemeName = schemePropTrans.SchemeMst.schemeName,
                                      propertyTypeId = propertyTypeMsts.propertyTypeId,
                                      propertyType = propertyTypeMsts.propertyTypeName,
                                      sectorId = sectorMsts.sectorId,
                                      sectorName = sectorMsts.sectorName,
                                      blockId = blockMsts.blockId,
                                      blockName = blockMsts.blockName,
                                      departmentId = departmentMsts.departmentId,
                                      departmentName = departmentMsts.departmentName,
                                      floorId = floorMsts.floorId,
                                      floorName = floorMsts.floorName,
                                      createdBy = schemePropTrans.createdBy,
                                      lastModifiedDate = schemePropTrans.modifiedDate,
                                      plotProperty = schemePropTrans.propertyNo,
                                      refId = schemePropTrans.refId,
                                      totalArea = schemePropTrans.totalArea,
                                      coveredArea = schemePropTrans.coveredArea,
                                      actualArea = schemePropTrans.actualArea,
                                      charges = x.charges,
                                      locationId = x.locationId,
                                      locationCharges = x.charges == null ? false : true,
                                      RegistryName = schemePropTrans.Registry,
                                      PropertyCost = schemePropTrans.propertyCost,
                                      civilCost = schemePropTrans.civilCost,
                                      TotalPropertyCost = schemePropTrans.totalPropertyCost,
                                      LandRate = schemePropTrans.landRatePerSqmt,
                                      LandRatePerSqMet = schemePropTrans.landRatePerSqmt,
                                      SchemeStatus = schemePropTrans.SchemeMst.Status,
                                      locationName = dbContext.LocationMsts.Where(p => p.locationId == x.locationId).Select(p => p.locationName).FirstOrDefault(),

                                      IsAllotted = dbContext.AllotmentMasters.Where(p => p.propertyId == schemePropTrans.propertyId && p.isStatus == AllotmentStatus.Approved.ToString()
                                                                                             ).Select(p => p.rid).FirstOrDefault()

                                  });

                return propDetail.ToDataSourceResult(request);
            }
        }
        //public DataSourceResult GetPropertyDetail_Read(DataSourceRequest request)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        //var propDetail = new List<PropertyModel>();

        //        var propDetail = (from schemePropTrans in dbContext.SchemePropTrans
        //                      //join departmentMsts in dbContext.DepartmentMsts on schemePropTrans.departmentId equals departmentMsts.departmentId
        //                      join propertyTypeMsts in dbContext.PropertyTypeMsts on schemePropTrans.propertyTypeId equals propertyTypeMsts.propertyTypeId
        //                      join alotment in dbContext.AllotmentMasters on schemePropTrans.propertyId equals alotment.propertyId
        //                      //join sectorMsts in dbContext.SectorMsts on schemePropTrans.sectorId equals sectorMsts.sectorId
        //                      //join blockMsts in dbContext.BlockMsts on schemePropTrans.blockId equals blockMsts.blockId
        //                      //join floorMsts in dbContext.FloorMsts on schemePropTrans.floorId equals floorMsts.floorId
        //                      //join propertyLocationChargesTrans in dbContext.PropertyLocationChargesTrans on schemePropTrans.propertyId equals propertyLocationChargesTrans.propertyId
        //                      //into prods
        //                      //from x in prods.DefaultIfEmpty()
        //                      where schemePropTrans.IsActive == true && schemePropTrans.schemeId != null
        //                      select new PropertyModel
        //                      {
        //                          schemeId = schemePropTrans.schemeId,
        //                          schemeName = schemePropTrans.SchemeMst.schemeName,
        //                          propertyTypeId = propertyTypeMsts.propertyTypeId,
        //                          propertyType = propertyTypeMsts.propertyTypeName,
        //                          sectorId = schemePropTrans.SectorMst.sectorId,
        //                          sectorName = schemePropTrans.SectorMst.sectorName,
        //                          blockId = schemePropTrans.BlockMst.blockId,
        //                          blockName = schemePropTrans.BlockMst.blockName,
        //                          departmentId = schemePropTrans.DepartmentMst.departmentId,
        //                          departmentName = schemePropTrans.DepartmentMst.departmentName,
        //                          floorId = schemePropTrans.FloorMst.floorId,
        //                          floorName = schemePropTrans.FloorMst.floorName,
        //                          createdBy = schemePropTrans.createdBy,
        //                          lastModifiedDate = schemePropTrans.modifiedDate,
        //                          plotProperty = schemePropTrans.propertyNo,
        //                          refId = schemePropTrans.refId,
        //                          totalArea = schemePropTrans.totalArea,
        //                          coveredArea = schemePropTrans.coveredArea,
        //                          actualArea = schemePropTrans.actualArea,
        //                          //charges = x.charges,
        //                          //locationId = x.locationId,
        //                          RegistryName = schemePropTrans.Registry,
        //                          TotalPropertyCost = schemePropTrans.totalPropertyCost,
        //                          LandRatePerSqMet = schemePropTrans.landRatePerSqmt,
        //                          SchemeStatus = schemePropTrans.SchemeMst.Status,
        //                          IsAllotted = dbContext.AllotmentMasters.Where(p => p.propertyId == schemePropTrans.propertyId && p.isStatus == AllotmentStatus.Approved.ToString()
        //                                                                                 ).Select(p => p.rid).FirstOrDefault()
        //                          //locationName = dbContext.LocationMsts.Where(p => p.locationId == x.locationId).Select(p => p.locationName).FirstOrDefault(),

        //                          ////applicationForms.gender.ToLower() == Constants.Male ? 1 : applicationForms.gender.ToLower() == Constants.Female ? 2 : 3,

        //                          //LandRate = schemePropTrans.departmentId == 5 ? dbContext.SchemeCostTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
        //                          //                                                  && x.IsActive == true).Select(p => p.totalPropertyCost + p.civilCost).FirstOrDefault() : dbContext.SchemeCostTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
        //                          //                                                  && x.IsActive == true).Select(p => p.landRatePerSqmt).FirstOrDefault(),

        //                          //allotementMoney = dbContext.SchemeCostTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
        //                          //                                                  && p.propertyTypeId == schemePropTrans.propertyTypeId && p.sectorId == schemePropTrans.sectorId
        //                          //                                                  && p.floorId == schemePropTrans.floorId).Select(p => p.allotmentMoney).FirstOrDefault(),
        //                          //leaseRent = dbContext.SchemeCostTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
        //                          //                                                  && p.propertyTypeId == schemePropTrans.propertyTypeId && p.sectorId == schemePropTrans.sectorId
        //                          //                                                  && p.floorId == schemePropTrans.floorId).Select(p => p.leaseRent).FirstOrDefault(),

        //                          //civilCost = dbContext.SchemeCostTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
        //                          //                                                && p.propertyTypeId == schemePropTrans.propertyTypeId && p.sectorId == schemePropTrans.sectorId
        //                          //                                                && p.floorId == schemePropTrans.floorId).Select(p => p.civilCost).FirstOrDefault(),

        //                          //allotementMoneyPercentage = dbContext.SchemeDepartmentTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
        //                          //                                                                                                              && p.IsActive == true).Select(p => p.allotmentMoneyPercent).FirstOrDefault(),

        //                          //floorAreaRatio = dbContext.SchemeDepartmentTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
        //                          //                                                          && p.IsActive == true).Select(p => p.far).FirstOrDefault(),

        //                          //leaseRentPercentage = dbContext.SchemeDepartmentTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
        //                          //                                                  && p.IsActive == true).Select(p => p.leaseRentPercent).FirstOrDefault(),

        //                          ////LandRatePerSqMet = dbContext.SchemeCostTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
        //                          ////                                                  && p.propertyTypeId == schemePropTrans.propertyTypeId && p.sectorId == schemePropTrans.sectorId).Select(p => p.landRatePerSqmt).FirstOrDefault(),

        //                          //PropertyCost = dbContext.SchemeCostTrans.Where(p => p.schemeId == schemePropTrans.schemeId && p.departmentId == schemePropTrans.departmentId
        //                          //                                                        && p.propertyTypeId == schemePropTrans.propertyTypeId && p.sectorId == schemePropTrans.sectorId
        //                          //                                                       ).Select(p => p.totalPropertyCost).FirstOrDefault(),

        //                          //IsAllotted = dbContext.AllotmentMasters.Where(p => p.propertyId == schemePropTrans.propertyId && p.isStatus == AllotmentStatus.Approved.ToString()
        //                          //                                                      ).Select(p => p.rid).FirstOrDefault()

        //                      });//.OrderByDescending(x => x.schemeName).ThenByDescending(s => s.sectorName).ThenByDescending(b => b.blockName).ThenByDescending(p => p.plotProperty).ToList();

        //        return propDetail.ToDataSourceResult(request);
        //    }
        //}

        /// <summary>
        /// Soft Delete Property by Property Id from Database
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>

        public bool RemovePropertyDetail(int propertyId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var propertyDetail = dbContext.SchemePropTrans.Where(u => u.refId == propertyId).FirstOrDefault();
                if (propertyDetail != null)
                {
                    propertyDetail.IsActive = false;
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// Get All Schemes
        /// </summary>
        /// <returns></returns>
        public List<PropertyModel> GetSchemes()
        {
            var lstPropertyModel = new List<PropertyModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();
                lstPropertyModel = (from lstSchemes in dbContext.SchemeMsts
                                    join dpt in dbContext.SchemeDepartmentTrans on lstSchemes.schemeId equals dpt.schemeId
                                    where lstSchemes.completed == true && lstSchemes.IsActive == true && loginUserDeptt.Contains(dpt.departmentId) && lstSchemes.Status != Constants.SchemeClosed
                                    select
                                        new PropertyModel
                                        {
                                            schemeId = lstSchemes.schemeId,
                                            schemeName = lstSchemes.schemeName
                                        }).Distinct().OrderByDescending(x => x.schemeId).ToList();
                return lstPropertyModel;
            }
        }

        /// <summary>
        /// Get all Property Type
        /// </summary>
        /// <returns></returns>
        public List<PropertyModel> GetPropertyType()
        {
            var lstPropertyModel = new List<PropertyModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lstPropertyModel = (from lstPropertyType in dbContext.PropertyTypeMsts
                                    where lstPropertyType.IsActive == true
                                    select
                                        new PropertyModel
    {
        propertyTypeId = lstPropertyType.propertyTypeId,
        propertyType = lstPropertyType.propertyTypeName
    }).ToList();
                return lstPropertyModel;
            }
        }

        /// <summary>
        /// Get all Locations
        /// </summary>
        /// <returns></returns>

        public List<PropertyModel> GetLocations()
        {
            var lstPropertyModel = new List<PropertyModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lstPropertyModel = (from lstLocations in dbContext.LocationMsts
                                    select
                                        new PropertyModel
    {
        locationId = lstLocations.locationId,
        locationName = lstLocations.locationName
    }).ToList();
                return lstPropertyModel;
            }
        }

        /// <summary>
        /// Get Departments on the basis of SchemeId
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public List<PropertyModel> GetDepartment(int schemeId)
        {
            var lstPropertyModel = new List<PropertyModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lstPropertyModel = (from lstDepartment in dbContext.DepartmentMsts
                                    join schemeCostTrans in dbContext.SchemeCostTrans on lstDepartment.departmentId equals schemeCostTrans.departmentId
                                    where lstDepartment.IsActive == true && schemeCostTrans.schemeId == schemeId
                                    select
                                        new PropertyModel
                                        {
                                            departmentId = lstDepartment.departmentId,
                                            departmentName = lstDepartment.departmentName
                                        }).Distinct().ToList();
                return lstPropertyModel;
            }
        }

        public List<DDList> GetDepartmentsForSubLease()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = new List<DDList>();
                var obj1 = new DDList();
                var obj2 = new DDList();
                var obj3 = new DDList();

                obj1.text = Departmentenum.Institutional.ToString();
                obj1.id = (from dept in dbContext.DepartmentMsts where dept.departmentName.ToLower() == Departmentenum.Institutional.ToString().ToLower() select dept.departmentId).FirstOrDefault();
                lst.Add(obj1);

                obj2.text = Departmentenum.Commercial.ToString();
                obj2.id = (from dept in dbContext.DepartmentMsts where dept.departmentName.ToLower() == Departmentenum.Commercial.ToString().ToLower() select dept.departmentId).FirstOrDefault();
                lst.Add(obj2);

                obj3.text = Constants.strDeptGroupHousing;
                obj3.id = (from dept in dbContext.DepartmentMsts where dept.departmentName.ToLower() == Constants.strDeptGroupHousing.ToLower() select dept.departmentId).FirstOrDefault();
                lst.Add(obj3);
                return lst;
            }
        }

        public SubleaseModel CheckRIDValidity(int parentPropRId)
        {
            var details = new SubleaseModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                var det = (from spt in dbContext.SchemePropTrans
                           join alMa in dbContext.AllotmentMasters on spt.propertyId equals alMa.propertyId
                           join sec in dbContext.SectorMsts on spt.sectorId equals sec.sectorId
                           join blo in dbContext.BlockMsts on spt.blockId equals blo.blockId
                           join deptMas in dbContext.DepartmentMsts on spt.departmentId equals deptMas.departmentId
                           where alMa.rid == parentPropRId && alMa.isActive == 1 && spt.IsActive == true
                           select new
                           {
                               Sector = sec.sectorName,
                               Block = blo.blockName,
                               Plot = spt.propertyNo,
                               //Following used to store values in hidden fields, used for getting Area Range
                               scheme = spt.schemeId,
                               propType = spt.propertyTypeId,
                               sectorID = spt.sectorId,
                               blockId = spt.blockId,
                               areaRange = spt.floorId,
                               deptId = spt.departmentId,
                               deptName = deptMas.departmentName
                           }).FirstOrDefault();
                if (det == null)
                    details.IsRIdValid = false;
                else
                {
                    details.Sector = det.Sector;
                    details.Block = det.Block;
                    details.Plot = det.Plot;
                    details.DepaertmentName = det.deptName;
                    details.IsRIdValid = true;
                    //Following used to store values in hidden fields, used for getting Area Range
                    details.Scheme = det.scheme;
                    details.PropType = det.propType;
                    details.SectorId = det.sectorID;
                    details.BlockId = det.blockId;
                    details.AreaRange = det.areaRange;
                    details.DeptId = det.deptId;
                }
                return details;
            }
            //return obj;
        }

        public bool SaveSubLease(int parentPropRId, int dept, string subLeasePropNo, decimal areaRate, decimal propArea, decimal propCost, int areaRangeId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var parentSPT = (from spt in dbContext.SchemePropTrans
                                 join alMa in dbContext.AllotmentMasters on spt.propertyId equals alMa.propertyId
                                 where alMa.rid == parentPropRId && alMa.isActive == 1 && spt.IsActive == true
                                 select spt).FirstOrDefault();
                if (parentSPT != null)
                {
                    var newSubLease = new SchemePropTran();
                    newSubLease.schemeId = parentSPT.schemeId;
                    newSubLease.departmentId = dept;
                    newSubLease.propertyTypeId = parentSPT.propertyTypeId;
                    newSubLease.sectorId = parentSPT.sectorId;
                    newSubLease.blockId = parentSPT.blockId;
                    newSubLease.floorId = parentSPT.floorId;
                    //PropertyID 
                    newSubLease.floorId = areaRangeId;
                    newSubLease.propertyNo = subLeasePropNo;
                    newSubLease.propertyCost = propCost;
                    newSubLease.landRatePerSqmt = areaRate;
                    newSubLease.totalArea = propArea;
                    newSubLease.Registry = parentSPT.Registry;
                    newSubLease.totalPropertyCost = propCost;
                    newSubLease.allotmentMoney = Convert.ToDecimal(0.0); // As discussed with Vishal Shukla, 0.0 instead of null 
                    newSubLease.IsActive = true;
                    newSubLease.createdBy = userInfo.UserID.ToString();
                    newSubLease.createdDate = DateTime.Now;
                    newSubLease.ParentPropertyId = parentSPT.propertyId;
                    dbContext.SchemePropTrans.Add(newSubLease);
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }

        public DataSourceResult GetSubLeaseData(DataSourceRequest request, int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var parentPropId = (from par in dbContext.SchemePropTrans
                                    join alMa in dbContext.AllotmentMasters on par.propertyId equals alMa.propertyId
                                    join deptt in dbContext.DepartmentMsts on par.departmentId equals deptt.departmentId
                                    where alMa.rid == rid
                                    select new
                                    {
                                        propId = par.propertyId,
                                        deptName = deptt.departmentName
                                    }).FirstOrDefault();
                var data = (from spt in dbContext.SchemePropTrans
                            join sec in dbContext.SectorMsts on spt.sectorId equals sec.sectorId
                            join blo in dbContext.BlockMsts on spt.blockId equals blo.blockId
                            where spt.ParentPropertyId == parentPropId.propId && spt.IsActive == true
                            select new SubleaseModel
                            {
                                Id = spt.refId,
                                DepaertmentName = parentPropId.deptName,
                                Sector = sec.sectorName,
                                Block = blo.blockName,
                                SubLeaseAddNo = spt.propertyNo,
                                PropArea = spt.totalArea.Value
                            });
                return data.ToDataSourceResult(request);
            }
        }

        public bool RemoveSubLease(int Id)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var subLease = (from spt in dbContext.SchemePropTrans where spt.refId == Id select spt).FirstOrDefault();
                if (subLease != null)
                {
                    subLease.IsActive = false;
                    //dbContext.SchemePropTrans.Remove(subLease);
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// Add or Update PropertyDetail
        /// </summary>
        /// <param name="propertyDetail"></param>
        /// <returns></returns>
        public bool SavePropertyDetail(PropertyModel model)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.departmentId == NADepartment.Housing)
                {
                    var allotmentMoney = dbContext.SchemeCostTrans.Where(x => x.schemeId == model.schemeId && x.departmentId == model.departmentId && x.sectorId == model.sectorId && x.blockId == model.blockId && x.floorId == model.floorId).Select(y => y.allotmentMoney).FirstOrDefault();
                    if (allotmentMoney != null)
                        model.allotementMoney = allotmentMoney;
                }
                else
                {
                    var allotmentPercent = dbContext.SchemeDepartmentTrans.Where(x => x.schemeId == model.schemeId && x.departmentId == model.departmentId).Select(y => y.allotmentMoneyPercent).FirstOrDefault();
                    if (allotmentPercent != null && model.TotalPropertyCost != null)
                        model.allotementMoney = ((model.TotalPropertyCost * Convert.ToDecimal(allotmentPercent)) / 100);
                }
                var existingProperty = dbContext.SchemePropTrans.Where(i => i.refId == model.refId && i.IsActive == true).FirstOrDefault();
                if (existingProperty != null)
                {
                    existingProperty.schemeId = model.schemeId;
                    existingProperty.departmentId = model.departmentId;
                    existingProperty.propertyTypeId = model.propertyTypeId;
                    existingProperty.sectorId = model.sectorId;
                    existingProperty.blockId = model.blockId;
                    existingProperty.floorId = model.floorId;
                    existingProperty.coveredArea = model.coveredArea;
                    existingProperty.IsActive = true;
                    existingProperty.propertyNo = model.plotProperty;
                    existingProperty.actualArea = model.actualArea;
                    existingProperty.totalArea = model.totalArea;
                    existingProperty.Registry = model.RegistryName;
                    existingProperty.totalPropertyCost = model.TotalPropertyCost;
                    existingProperty.landRatePerSqmt = model.LandRate == null ? model.LandRatePerSqMet : model.LandRate;
                    existingProperty.allotmentMoney = model.allotementMoney;
                    existingProperty.EarnestMoney = model.EarnestMoney;
                    existingProperty.civilCost = (model.civilCost == null || model.civilCost == 0) ? existingProperty.propertyCost : model.civilCost;
                    existingProperty.propertyCost = (model.PropertyCost == null || model.PropertyCost == 0) ? existingProperty.propertyCost : model.PropertyCost;
                    existingProperty.ParentPropertyId = (model.ParentPropertyId == null || model.ParentPropertyId == 0) ? existingProperty.ParentPropertyId : model.ParentPropertyId;
                    //Added on 16Nov2017
                    existingProperty.groupProjectId = model.ProjectId;

                    existingProperty.modifiedBy = userInfo.UserID.ToString();
                    existingProperty.modifiedDate = DateTime.Now;

                    dbContext.SaveChanges();

                    int iflag = SaveLocationChargeDetails(model);
                }
                else
                {
                    var record = dbContext.SchemePropTrans.Where(x => x.schemeId == model.schemeId && x.departmentId == model.departmentId && x.sectorId == model.sectorId && x.blockId == model.blockId && x.floorId == model.floorId && x.propertyNo.Trim() == model.plotProperty.Trim()).FirstOrDefault();
                    if (record == null)
                    {
                        var schemePropTran = new SchemePropTran();
                        schemePropTran.schemeId = model.schemeId;
                        schemePropTran.departmentId = model.departmentId;
                        schemePropTran.propertyTypeId = model.propertyTypeId;
                        schemePropTran.sectorId = model.sectorId;
                        schemePropTran.blockId = model.blockId;
                        schemePropTran.floorId = model.floorId;
                        schemePropTran.coveredArea = model.coveredArea;
                        schemePropTran.IsActive = true;
                        schemePropTran.propertyNo = model.plotProperty;
                        schemePropTran.actualArea = model.actualArea;
                        schemePropTran.totalArea = model.totalArea;
                        schemePropTran.Registry = model.RegistryName;
                        schemePropTran.totalPropertyCost = model.TotalPropertyCost;
                        schemePropTran.landRatePerSqmt = model.LandRatePerSqMet;
                        schemePropTran.civilCost = model.civilCost;
                        schemePropTran.propertyCost = model.PropertyCost;
                        schemePropTran.allotmentMoney = model.allotementMoney;
                        schemePropTran.EarnestMoney = model.EarnestMoney;
                        //Added on 16Nov2017
                        schemePropTran.groupProjectId = model.ProjectId;

                        schemePropTran.createdBy = userInfo.UserID.ToString();
                        schemePropTran.createdDate = DateTime.Now;

                        dbContext.SchemePropTrans.Add(schemePropTran);
                        dbContext.SaveChanges();

                        model.refId = schemePropTran.refId;
                        ////model.PropertyId = dbContext.SchemePropTrans.Where(x => x.schemeId == model.schemeId && x.departmentId == model.departmentId && x.sectorId == model.sectorId && x.blockId == model.blockId && x.floorId == model.floorId && x.propertyNo.Trim() == model.plotProperty.Trim()).FirstOrDefault().propertyId;
                        //model.PropertyId = dbContext.SchemePropTrans.FirstOrDefault(c=>c.refId==schemePropTran.refId).propertyId;
                        ////model.PropertyNo = schemePropTran.propertyId;
                        //model.PropertyNo = schemePropTran.propertyId;
                        int iflag = SaveLocationChargeDetails(model);
                    }
                    //var latInsertedRecord = dbContext.SchemePropTrans.Where(x => x.schemeId == model.schemeId && x.departmentId == model.departmentId && x.sectorId == model.sectorId && x.blockId == model.blockId && x.floorId == model.floorId && x.propertyNo.Trim() == model.plotProperty.Trim()).FirstOrDefault();
                    //if (latInsertedRecord == null)
                    //{
                    //    dbContext.SchemePropTrans.Add(schemePropTran);
                    //    dbContext.SaveChanges();
                    //    if (model.departmentId != NADepartment.Housing)
                    //    {
                    //        int lastInsertedRefId = schemePropTran.refId;
                    //        int? lastInsertedpropertyId = dbContext.SchemePropTrans.Where(y => y.refId == lastInsertedRefId).Select(x => x.propertyId).FirstOrDefault();
                    //        if (model.locationId != 0 && model.locationId != null && model.charges != 0 && model.charges != null) // add Null check By Vishal Shukla 24-Feb-2017
                    //        {
                    //            var LocationCharge = new PropertyLocationChargesTran();
                    //            LocationCharge.locationId = model.locationId;
                    //            LocationCharge.propertyId = lastInsertedpropertyId;
                    //            LocationCharge.charges = model.charges;
                    //            LocationCharge.IsActive = true;
                    //            dbContext.PropertyLocationChargesTrans.Add(LocationCharge);
                    //            dbContext.SaveChanges();
                    //        }
                    //    }
                    //}
                    flag = true;
                }

            }
            return flag;
        }

        // shatrughna 29-11-2017
        private int SaveLocationChargeDetails(PropertyModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                //var propid = dbContext.SchemePropTrans.Where(x => x.schemeId == model.schemeId && x.departmentId == model.departmentId && x.sectorId == model.sectorId && x.blockId == model.blockId && x.floorId == model.floorId && x.propertyNo.Trim() == model.plotProperty.Trim()).FirstOrDefault().propertyId;
                model.PropertyId = dbContext.SchemePropTrans.FirstOrDefault(c => c.refId == model.refId).propertyId;
                if (model.IsLocationCharged == false)
                {
                    var existingRecord = dbContext.PropertyLocationChargesTrans.Where(x => x.propertyId == model.PropertyId && x.IsActive == true).FirstOrDefault();
                    if (existingRecord != null)
                    {
                        if (existingRecord != null)
                        {
                            dbContext.PropertyLocationChargesTrans.Remove(existingRecord);
                            dbContext.SaveChanges();
                            flag = ReturnType.Removed;
                        }
                    }

                    var locationcharge = dbContext.LocationChargeDetailMsts.FirstOrDefault(l => l.PropertyId == model.PropertyId);
                    if (locationcharge != null)
                    {
                        dbContext.LocationChargeDetailMsts.Remove(locationcharge);
                        dbContext.SaveChanges();
                        flag = ReturnType.Removed;
                    }
                }
                else
                {                    
                    var existingCharge = dbContext.PropertyLocationChargesTrans.Where(x => x.propertyId == model.PropertyId && x.IsActive == true).FirstOrDefault();
                    if (existingCharge != null && model.departmentId == NADepartment.Housing)
                    {
                        if (existingCharge != null)
                        {
                            dbContext.PropertyLocationChargesTrans.Remove(existingCharge);
                            dbContext.SaveChanges();
                            flag = ReturnType.Removed;
                        }
                    }
                    else
                    {
                        if (existingCharge != null)
                        {
                            existingCharge.locationId = model.locationId;
                            existingCharge.charges = model.charges;
                            existingCharge.IsActive = true;
                            existingCharge.modifiedBy = userInfo.UserID.ToString();
                            existingCharge.modifiedDate = DateTime.Now;
                            dbContext.SaveChanges();
                            flag = ReturnType.Updated;
                        }
                        else if (model.locationId != null && model.charges != null && model.locationId != 0 && model.charges != 0)
                        {
                            var locationCharge = new PropertyLocationChargesTran();
                            locationCharge.locationId = model.locationId;
                            locationCharge.propertyId = model.PropertyId;
                            locationCharge.charges = model.charges;
                            locationCharge.IsActive = true;
                            locationCharge.createdBy = userInfo.UserID.ToString();
                            locationCharge.createdDate = DateTime.Now;
                            dbContext.PropertyLocationChargesTrans.Add(locationCharge);
                            dbContext.SaveChanges();
                            flag = ReturnType.Saved;
                        }
                    }

                    var locationcharge = dbContext.LocationChargeDetailMsts.FirstOrDefault(l => l.PropertyId == model.PropertyId);
                    if (locationcharge != null)
                    {
                        locationcharge.LocationId = model.locationId;
                        locationcharge.LocationCharge = model.charges;
                        locationcharge.LocationChargeRate = model.LocationChargeRate;
                        locationcharge.IsActive = true;
                        locationcharge.ModifiedBy = userInfo.UserID;
                        locationcharge.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        var lcharge = new LocationChargeDetailMst();
                        lcharge.LocationId = model.locationId;
                        lcharge.PropertyId = model.PropertyId;
                        lcharge.LocationCharge = model.charges;
                        lcharge.LocationChargeRate = model.LocationChargeRate;
                        lcharge.IsActive = true;
                        lcharge.CreatedBy = userInfo.UserID;
                        lcharge.CreatedDate = DateTime.Now;
                        dbContext.LocationChargeDetailMsts.Add(lcharge);
                        dbContext.SaveChanges();
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// Get Property Type on the basis of SchemeId and Sector Id
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>

        public List<PropertyModel> GetPropertyTypeBySchemeAndDeptId(int schemeId, int departmentId)
        {
            var lstPropertyModel = new List<PropertyModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lstPropertyModel = (from schemeCostTrans in dbContext.SchemeCostTrans
                                    join propertyTypeMsts in dbContext.PropertyTypeMsts on schemeCostTrans.propertyTypeId equals propertyTypeMsts.propertyTypeId
                                    where schemeCostTrans.IsActive == true && schemeCostTrans.schemeId == schemeId && schemeCostTrans.departmentId == departmentId
                                    select
                                        new PropertyModel
                                        {
                                            propertyTypeId = propertyTypeMsts.propertyTypeId,
                                            propertyType = propertyTypeMsts.propertyTypeName
                                        }).Distinct().ToList();
                return lstPropertyModel;
            }
        }

        /// <summary>
        /// Get Sectors on the basis of SchemeId and Sector Id
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>

        public List<PropertyModel> GetSectorBySchemeAndDeptId(int schemeId, int departmentId, int propTypeId)
        {
            var lstPropertyModel = new List<PropertyModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lstPropertyModel = (from schemeCostTrans in dbContext.SchemeCostTrans
                                    join sectorMsts in dbContext.SectorMsts on schemeCostTrans.sectorId equals sectorMsts.sectorId
                                    where schemeCostTrans.IsActive == true && schemeCostTrans.schemeId == schemeId && schemeCostTrans.departmentId == departmentId && schemeCostTrans.propertyTypeId == propTypeId
                                    select
                                        new PropertyModel
                                        {
                                            sectorId = sectorMsts.sectorId,
                                            sectorName = sectorMsts.sectorName
                                        }).Distinct().ToList();
                return lstPropertyModel;
            }
        }

        /// <summary>
        /// Get Blocks on the basis of SchemeId and Sector Id
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>

        public List<PropertyModel> GetBlockBySchemeAndDeptId(int schemeId, int departmentId, int propTypeId, int sectorId)
        {
            var lstPropertyModel = new List<PropertyModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lstPropertyModel = (from schemeCostTrans in dbContext.SchemeCostTrans
                                    join blockMsts in dbContext.BlockMsts on schemeCostTrans.blockId equals blockMsts.blockId
                                    where schemeCostTrans.IsActive == true && schemeCostTrans.schemeId == schemeId && schemeCostTrans.departmentId == departmentId && schemeCostTrans.propertyTypeId == propTypeId && schemeCostTrans.sectorId == sectorId
                                    select
                                        new PropertyModel
        {
            blockId = blockMsts.blockId,
            blockName = blockMsts.blockName
        }).Distinct().ToList();
                return lstPropertyModel;
            }
        }

        /// <summary>
        ///  Get Floors on the basis of SchemeId and Sector Id 
        ///  Floors will only display in case of Housing as Department and Flat as Property Type
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>

        public List<PropertyModel> GetFloorBySchemeAndDeptId(int schemeId, int departmentId, int propTypeId, int sectorId, int blockId)
        {
            var lstPropertyModel = new List<PropertyModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lstPropertyModel = (from schemeCostTrans in dbContext.SchemeCostTrans
                                    join floorMsts in dbContext.FloorMsts on schemeCostTrans.floorId equals floorMsts.floorId
                                    where schemeCostTrans.IsActive == true && schemeCostTrans.schemeId == schemeId && schemeCostTrans.departmentId == departmentId
                                          && schemeCostTrans.propertyTypeId == propTypeId && schemeCostTrans.sectorId == sectorId && schemeCostTrans.blockId == blockId
                                    select
                                        new PropertyModel
                                        {
                                            floorId = floorMsts.floorId,
                                            floorName = floorMsts.floorName
                                        }).Distinct().ToList();
                return lstPropertyModel;
            }
        }

        /// <summary>
        /// Checking Duplicate value for Property
        /// </summary>
        /// <param name="secotrId"></param>
        /// <param name="blockId"></param>
        /// <param name="plotNo"></param>
        /// <param name="refId"></param>
        /// <returns></returns>
        //This function is no longer in use because property no. can be in any kind of format which will be difficult to find duplicay in -> by Vishal Shukla
        public bool CheckDuplicateProperty(int secotrId, int blockId, string plotNo, int refId)
        {
            var flag = false;

            //using (var dbContext = new NoidaPMSEntities())
            //{


            //    int numericPlotNo;
            //    List<string> propertyNoList;
            //    bool isNumeric = int.TryParse(plotNo, out numericPlotNo);
            //    //var propertyNoList = dbContext.Database.SqlQuery<int>("SELECT cast([propertyNo] as int) FROM [dbo].[SchemePropTrans] where propertyNo not like'%[^0-9]%'").ToList();
            //    if (refId == 0)
            //    {
            //        propertyNoList = (from Props in dbContext.SchemePropTrans
            //                          where SqlFunctions.IsNumeric(Props.propertyNo) == 1
            //                          select Props.propertyNo).ToList();
            //    }
            //    else
            //    {
            //        propertyNoList = (from Props in dbContext.SchemePropTrans
            //                          where SqlFunctions.IsNumeric(Props.propertyNo) == 1 && Props.refId != refId
            //                          select Props.propertyNo).ToList();
            //    }
            //    var propertyNoListinInt = propertyNoList.Select(s => (s)).ToList();
            //    SchemePropTran property = new SchemePropTran();
            //    if (refId == 0)
            //    {
            //        //if (deptId == 5)
            //        //    property = dbContext.SchemePropTrans.FirstOrDefault(cond => cond.departmentId == deptId && cond.propertyTypeId == propTypeId && cond.sectorId == secotrId && cond.blockId == blockId && cond.propertyNo == plotNo && cond.IsActive == true);
            //        //else
            //        if (isNumeric)
            //        {
            //            property = dbContext.SchemePropTrans.FirstOrDefault(cond => cond.sectorId == secotrId && cond.blockId == blockId && propertyNoListinInt.Contains(numericPlotNo) && cond.IsActive == true);
            //        }
            //        else
            //        {
            //            property = dbContext.SchemePropTrans.FirstOrDefault(cond => cond.sectorId == secotrId && cond.blockId == blockId && cond.propertyNo == plotNo && cond.IsActive == true);
            //        }

            //    }
            //    else
            //    {
            //        //if (deptId == 5)
            //        //    property = dbContext.SchemePropTrans.FirstOrDefault(cond => cond.departmentId == deptId && cond.propertyTypeId == propTypeId && cond.sectorId == secotrId && cond.blockId == blockId && cond.propertyNo == plotNo && cond.IsActive == true && cond.refId != refId);
            //        //else
            //        if (isNumeric)
            //        {
            //            property = dbContext.SchemePropTrans.FirstOrDefault(cond => cond.sectorId == secotrId && cond.blockId == blockId && propertyNoListinInt.Contains(numericPlotNo) && cond.IsActive == true);
            //            // property = dbContext.SchemePropTrans.FirstOrDefault(cond => cond.sectorId == secotrId && cond.blockId == blockId && cond.propertyNo == plotNo && cond.IsActive == true && cond.refId != refId);
            //        }
            //        else
            //        {
            //            property = dbContext.SchemePropTrans.FirstOrDefault(cond => cond.sectorId == secotrId && cond.blockId == blockId && cond.propertyNo == plotNo && cond.IsActive == true && cond.refId != refId);
            //        }

            //    }
            //    if (property != null)
            //        flag = true;
            //    else
            // flag = false;
            //}
            return flag;
        }


        public PropertyModel GetLandRate(int schemeId, int departmentId, int floorId, int blockId, int sectorId, int propertyTypeId)
        {
            //Decimal? landRate = 0;
            var model = new PropertyModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                var propertyCost = dbContext.SchemeCostTrans.Where(x => x.departmentId == departmentId && x.schemeId == schemeId && x.blockId == blockId && x.sectorId == sectorId && x.propertyTypeId == propertyTypeId && x.floorId == floorId && x.IsActive == true).FirstOrDefault();
                if (propertyCost != null)
                {
                    model.departmentId = departmentId;
                    model.LandRate = departmentId != NADepartment.Housing ? propertyCost.landRatePerSqmt : null;
                    model.LandRatePerSqMet = departmentId != NADepartment.Housing ? propertyCost.landRatePerSqmt : null;
                    model.civilCost = departmentId == NADepartment.Housing ? propertyCost.civilCost : null;
                    model.PropertyCost = propertyCost.propertyCost;
                    model.TotalPropertyCost = propertyCost.totalPropertyCost;


                    //if (departmentId == Convert.ToInt32(Departmentenum.Housing))
                    //{
                    //    //model.LandRate = dbContext.SchemeCostTrans.Where(x => x.departmentId == departmentId && x.schemeId == schemeId && x.IsActive == true).Select(y => y.totalPropertyCost + y.civilCost).FirstOrDefault();
                    //    model.civilCost = propertyCost.civilCost;
                    //    model.PropertyCost = propertyCost.propertyCost;
                    //    model.LandRatePerSqMet = null;
                    //    model.TotalPropertyCost = propertyCost.totalPropertyCost;
                    //    model.departmentId = departmentId;
                    //}
                    //else
                    //{
                    //    model.civilCost = null;
                    //    model.PropertyCost = null;
                    //    model.LandRatePerSqMet = propertyCost.landRatePerSqmt;
                    //    model.TotalPropertyCost = propertyCost.landRatePerSqmt;
                    //    model.departmentId = departmentId;
                    //    //model.LandRate = dbContext.SchemeCostTrans.Where(x => x.departmentId == departmentId && x.schemeId == schemeId && x.IsActive == true).Select(y => y.landRatePerSqmt).FirstOrDefault();
                    //}
                }
            }
            return model;
        }
        /// <summary>
        /// Getting all Properties Detail
        /// </summary>
        /// <returns></returns>
        public List<PropertyModel> GetPropertyBankDetailAdd(int? schemeId, int? deptId, int? propertyTypeId, int? sectorId, int? blockId, int? floorId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var propDetail = new List<PropertyModel>();
                propDetail = (from schemePropTrans in dbContext.SchemePropTrans
                              join departmentMsts in dbContext.DepartmentMsts on schemePropTrans.departmentId equals departmentMsts.departmentId
                              join propertyTypeMsts in dbContext.PropertyTypeMsts on schemePropTrans.propertyTypeId equals propertyTypeMsts.propertyTypeId
                              join sectorMsts in dbContext.SectorMsts on schemePropTrans.sectorId equals sectorMsts.sectorId
                              join blockMsts in dbContext.BlockMsts on schemePropTrans.blockId equals blockMsts.blockId
                              join floorMsts in dbContext.FloorMsts on schemePropTrans.floorId equals floorMsts.floorId
                              join propertyLocationChargesTrans in dbContext.PropertyLocationChargesTrans on schemePropTrans.refId equals propertyLocationChargesTrans.propertyId
                              into prods
                              from x in prods.DefaultIfEmpty()
                              where schemePropTrans.IsActive == true && schemePropTrans.schemeId == null //&&
                                  && (deptId == null || schemePropTrans.departmentId == deptId)
                                   && (propertyTypeId == 0 || propertyTypeId == null || schemePropTrans.propertyTypeId == propertyTypeId)
                          && (sectorId == 0 || sectorId == null || schemePropTrans.sectorId == sectorId)
                                   && (blockId == 0 || blockId == null || schemePropTrans.blockId == blockId)
                                   && (floorId == 0 || floorId == null || schemePropTrans.floorId == floorId)

                              select new PropertyModel
                              {
                                  schemeId = schemePropTrans.schemeId,
                                  schemeName = schemePropTrans.SchemeMst.schemeName,
                                  propertyTypeId = propertyTypeMsts.propertyTypeId,
                                  propertyType = propertyTypeMsts.propertyTypeName,
                                  sectorId = sectorMsts.sectorId,
                                  sectorName = sectorMsts.sectorName,
                                  blockId = blockMsts.blockId,
                                  blockName = blockMsts.blockName,
                                  departmentId = departmentMsts.departmentId,
                                  departmentName = departmentMsts.departmentName,
                                  floorId = floorMsts.floorId,
                                  floorName = floorMsts.floorName,
                                  plotProperty = schemePropTrans.propertyNo,
                                  refId = schemePropTrans.refId,
                                  totalArea = schemePropTrans.totalArea,
                                  coveredArea = schemePropTrans.coveredArea,
                                  actualArea = schemePropTrans.actualArea,
                                  RegistryName = schemePropTrans.Registry,
                                  TotalPropertyCost = schemePropTrans.totalPropertyCost,
                                  IsAllotted = dbContext.AllotmentMasters.Where(p => p.propertyId == schemePropTrans.propertyId && p.isStatus == AllotmentStatus.Approved.ToString()
                                                                                         ).Select(p => p.rid).FirstOrDefault()

                              }).OrderByDescending(x => x.refId).ToList();

                return propDetail;
            }
        }
        public List<PropertyModel> GetPropertyBankDetail(int? schemeId, int? deptId, int? propertyTypeId, int? sectorId, int? blockId, int? floorId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var propDetail = new List<PropertyModel>();
                if (schemeId == null && (deptId == null || deptId == 0) && (propertyTypeId == null || propertyTypeId == 0) && (sectorId == null || sectorId == 0) && (blockId == null || blockId == 0) && (floorId == null || floorId == 0))
                {// propDetail = null;
                }
                else
                {
                    propDetail = (from schemePropTrans in dbContext.SchemePropTrans
                                  join departmentMsts in dbContext.DepartmentMsts on schemePropTrans.departmentId equals departmentMsts.departmentId
                                  join propertyTypeMsts in dbContext.PropertyTypeMsts on schemePropTrans.propertyTypeId equals propertyTypeMsts.propertyTypeId
                                  join sectorMsts in dbContext.SectorMsts on schemePropTrans.sectorId equals sectorMsts.sectorId
                                  join blockMsts in dbContext.BlockMsts on schemePropTrans.blockId equals blockMsts.blockId
                                  join floorMsts in dbContext.FloorMsts on schemePropTrans.floorId equals floorMsts.floorId
                                  join propertyLocationChargesTrans in dbContext.PropertyLocationChargesTrans on schemePropTrans.refId equals propertyLocationChargesTrans.propertyId
                                  into prods
                                  from x in prods.DefaultIfEmpty()
                                  where schemePropTrans.IsActive == true && schemePropTrans.schemeId == null //&&
                                      // (schemeId == null || schemePropTrans.schemeId == schemeId)
                                           && (schemePropTrans.departmentId == deptId)
                                           && (schemePropTrans.propertyTypeId == propertyTypeId)
                                  && (schemePropTrans.sectorId == sectorId)
                                           && (schemePropTrans.blockId == blockId)
                                           && (schemePropTrans.floorId == floorId)

                                  select new PropertyModel
                                  {
                                      schemeId = schemePropTrans.schemeId,
                                      schemeName = schemePropTrans.SchemeMst.schemeName,
                                      propertyTypeId = propertyTypeMsts.propertyTypeId,
                                      propertyType = propertyTypeMsts.propertyTypeName,
                                      sectorId = sectorMsts.sectorId,
                                      sectorName = sectorMsts.sectorName,
                                      blockId = blockMsts.blockId,
                                      blockName = blockMsts.blockName,
                                      departmentId = departmentMsts.departmentId,
                                      departmentName = departmentMsts.departmentName,
                                      floorId = floorMsts.floorId,
                                      floorName = floorMsts.floorName,
                                      plotProperty = schemePropTrans.propertyNo,
                                      refId = schemePropTrans.refId,
                                      totalArea = schemePropTrans.totalArea,
                                      coveredArea = schemePropTrans.coveredArea,
                                      actualArea = schemePropTrans.actualArea,
                                      RegistryName = schemePropTrans.Registry,
                                      TotalPropertyCost = schemePropTrans.totalPropertyCost,
                                      IsAllotted = dbContext.AllotmentMasters.Where(p => p.propertyId == schemePropTrans.propertyId && p.isStatus == AllotmentStatus.Approved.ToString()
                                                                                             ).Select(p => p.rid).FirstOrDefault()

                                  }).OrderByDescending(x => x.schemeName).ThenByDescending(s => s.sectorName).ThenByDescending(b => b.blockName).ThenByDescending(p => p.plotProperty).ToList();
                }
                return propDetail;
            }
        }

        public List<PropertyModel> GetDetachedPropertyDetail(int? schemeId, int? deptId, int? propertyTypeId, int? sectorId, int? blockId, int? floorId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var propDetail = new List<PropertyModel>();
                if (schemeId == null && (deptId == null || deptId == 0) && (propertyTypeId == null || propertyTypeId == 0) && (sectorId == null || sectorId == 0) && (blockId == null || blockId == 0) && (floorId == null || floorId == 0))
                {// propDetail = null;
                }
                else
                {
                    propDetail = (from schemePropTrans in dbContext.SchemePropTrans
                                  join departmentMsts in dbContext.DepartmentMsts on schemePropTrans.departmentId equals departmentMsts.departmentId
                                  join propertyTypeMsts in dbContext.PropertyTypeMsts on schemePropTrans.propertyTypeId equals propertyTypeMsts.propertyTypeId
                                  join sectorMsts in dbContext.SectorMsts on schemePropTrans.sectorId equals sectorMsts.sectorId
                                  join blockMsts in dbContext.BlockMsts on schemePropTrans.blockId equals blockMsts.blockId
                                  join floorMsts in dbContext.FloorMsts on schemePropTrans.floorId equals floorMsts.floorId
                                  join allotmentMaster in dbContext.AllotmentMasters on schemePropTrans.propertyId equals allotmentMaster.propertyId //where allotmentMaster.isStatus != null
                                  into prods
                                  from fgi in
                                      (from f in prods
                                       where f.isStatus != null && f.isActive != null
                                       select f).DefaultIfEmpty()
                                  where schemePropTrans.IsActive == true && schemePropTrans.schemeId != null
                                      //&& schemePropTrans.schemeId == null //&&
                                      && (schemePropTrans.schemeId == schemeId)
                                           && (schemePropTrans.departmentId == deptId)
                                           && (schemePropTrans.propertyTypeId == propertyTypeId)
                                           && (schemePropTrans.sectorId == sectorId)
                                           && (schemePropTrans.blockId == blockId)
                                           && (schemePropTrans.floorId == floorId)
                                  select new PropertyModel
                                  {
                                      schemeId = schemePropTrans.schemeId,
                                      schemeName = schemePropTrans.SchemeMst.schemeName,
                                      propertyTypeId = propertyTypeMsts.propertyTypeId,
                                      propertyType = propertyTypeMsts.propertyTypeName,
                                      sectorId = sectorMsts.sectorId,
                                      sectorName = sectorMsts.sectorName,
                                      blockId = blockMsts.blockId,
                                      blockName = blockMsts.blockName,
                                      departmentId = departmentMsts.departmentId,
                                      departmentName = departmentMsts.departmentName,
                                      floorId = floorMsts.floorId,
                                      floorName = floorMsts.floorName,
                                      createdBy = schemePropTrans.createdBy,
                                      lastModifiedDate = schemePropTrans.modifiedDate,
                                      plotProperty = schemePropTrans.propertyNo,
                                      refId = schemePropTrans.refId,
                                      totalArea = schemePropTrans.totalArea,
                                      RegistryName = schemePropTrans.Registry,
                                      TotalPropertyCost = schemePropTrans.totalPropertyCost,
                                      isStatusAllotment = fgi.isStatus,
                                      isActiveAllotment = fgi.isActive

                                  }).OrderByDescending(x => x.schemeName).ToList();
                }
                return propDetail;
            }
        }

        public bool AttachProperties(List<int> rIds, string refIds, int schemeId, string registry)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var attachedProperties = (from objAllotteeList in dbContext.SchemePropTrans
                                          where rIds.Contains(objAllotteeList.refId)
                                          select objAllotteeList).ToList();
                SchemePropTran objSchemePropTran = new SchemePropTran();
                objSchemePropTran = attachedProperties.FirstOrDefault();
                ObjectParameter commaString = new ObjectParameter("CommaString", typeof(string));
                dbContext.Sp_PropertyBank(refIds, Convert.ToInt32(schemeId), Convert.ToInt32(objSchemePropTran.departmentId), Convert.ToInt32(objSchemePropTran.sectorId), Convert.ToInt32(objSchemePropTran.blockId), Convert.ToInt32(objSchemePropTran.propertyTypeId), Convert.ToInt32(objSchemePropTran.floorId), registry);
                //var result = commaString.Value.ToString();
                return true;
            }
        }

        public bool DetachProperties(List<int> refIds)
        {
            using (var dbContext = new NoidaPMSEntities())
            {

                var detacedProperties = (from objAllotteeList in dbContext.SchemePropTrans
                                         where refIds.Contains(objAllotteeList.refId)
                                         select objAllotteeList).ToList();

                detacedProperties.Select(ua => { ua.schemeId = null; ua.Registry = null; ua.civilCost = null; ua.propertyCost = null; ua.totalPropertyCost = null; ua.allotmentMoney = null; return ua; }).ToList();
                dbContext.SaveChanges();
                return true;
            }
        }
        #endregion

        #region Scheme Notifications
        /// <summary>
        /// Get all notification to manage
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<NotificationsModel> GetAllNotifications(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<NotificationsModel> notificationsList = new List<NotificationsModel>();
                notificationsList = (from notification in dbContext.SchemeNotifTrans
                                     join schemes in dbContext.SchemeMsts on notification.schemeId equals schemes.schemeId
                                     where notification.IsActive == true && schemes.completed == true
                                     select new NotificationsModel
                                     {
                                         notificationID = notification.notificationId,
                                         notificationName = notification.notificationName,
                                         schemeID = notification.schemeId.Value,
                                         schemeName = schemes.schemeName,
                                         createdBy = notification.createdBy,
                                         createdDate = notification.createdDate.Value,
                                         modifiedBy = notification.modifiedBy,
                                         modifiedDate = notification.modifiedDate.Value,
                                         bookletCost = notification.bookletCost.Value,
                                         notificationStart = notification.startDate.Value,
                                         notificationEnd = notification.endDate.Value,
                                         publishDate = notification.PublishDate.Value,
                                         oldNotificationID = notification.oldNotificationId,
                                         oldNotificationName = dbContext.SchemeNotifTrans.Where(x => x.notificationId == notification.oldNotificationId).Select(y => y.notificationName).FirstOrDefault(),
                                         isActive = notification.IsActive.Value,
                                         selectedMediaType = dbContext.NotificationMediaTrans.Where(p => p.notificationId == notification.notificationId && p.IsActive == true).Select(p => p.mediaTypeId).ToList(),
                                         selectedPaymentMode = dbContext.NotificationPaymentModeTrans.Where(p => p.notificationId == notification.notificationId && p.IsActive == true).Select(p => p.paymentModeId).ToList()

                                     }).OrderByDescending(x => x.notificationID).ToList();

                return notificationsList;
            }

        }

        public DataSourceResult GetAllNotifications_Read(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //List<NotificationsModel> notificationsList = new List<NotificationsModel>();
                var notificationsList = (from notification in dbContext.SchemeNotifTrans
                                         join schemes in dbContext.SchemeMsts on notification.schemeId equals schemes.schemeId
                                         where notification.IsActive == true && schemes.completed == true
                                         select new NotificationsModel
                                         {
                                             notificationID = notification.notificationId,
                                             notificationName = notification.notificationName,
                                             schemeID = notification.schemeId.Value,
                                             schemeName = schemes.schemeName,
                                             createdBy = notification.createdBy,
                                             createdDate = notification.createdDate.Value,
                                             modifiedBy = notification.modifiedBy,
                                             modifiedDate = notification.modifiedDate.Value,
                                             bookletCost = notification.bookletCost.Value,
                                             notificationStart = notification.startDate.Value,
                                             notificationEnd = notification.endDate.Value,
                                             publishDate = notification.PublishDate.Value,
                                             oldNotificationID = notification.oldNotificationId,
                                             oldNotificationName = dbContext.SchemeNotifTrans.Where(x => x.notificationId == notification.oldNotificationId).Select(y => y.notificationName).FirstOrDefault(),
                                             isActive = notification.IsActive.Value,
                                             selectedMediaType = dbContext.NotificationMediaTrans.Where(p => p.notificationId == notification.notificationId && p.IsActive == true).Select(p => p.mediaTypeId).ToList(),
                                             selectedPaymentMode = dbContext.NotificationPaymentModeTrans.Where(p => p.notificationId == notification.notificationId && p.IsActive == true).Select(p => p.paymentModeId).ToList()

                                         });//.OrderByDescending(x => x.notificationID).ToList();

                return notificationsList.ToDataSourceResult(request);
            }

        }
        /// <summary>
        /// Get all information of a particular notification
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public NotificationsModel ViewSchemeNotification(int notificationId)
        {
            NotificationsModel model = null;

            using (var dbContext = new NoidaPMSEntities())
            {

                model = (from notif in dbContext.SchemeNotifTrans
                         join schemes in dbContext.SchemeMsts on notif.schemeId equals schemes.schemeId
                         where notif.notificationId == notificationId && schemes.completed == true
                         select new NotificationsModel
                         {
                             notificationID = notif.notificationId,
                             notificationName = notif.notificationName,
                             notificationStart = notif.startDate.Value,
                             notificationEnd = notif.endDate.Value,
                             publishDate = notif.PublishDate.Value,
                             schemeID = notif.schemeId,
                             schemeName = schemes.schemeName,
                             createdDate = notif.createdDate.Value,
                             modifiedDate = notif.modifiedDate.Value,
                             createdBy = notif.createdBy,
                             modifiedBy = notif.modifiedBy,
                             isActive = notif.IsActive.Value,
                             bookletCost = notif.bookletCost.Value,
                             oldNotificationID = notif.oldNotificationId,
                             oldNotificationName = dbContext.SchemeNotifTrans.Where(x => x.notificationId == notif.oldNotificationId).Select(y => y.notificationName).FirstOrDefault(),
                             mediaList = (from nm in dbContext.NotificationMediaTrans
                                          where nm.notificationId == notificationId
                                          join m in dbContext.MediaTypeMsts on nm.mediaTypeId equals m.mediaTypeId
                                          select new MediaModel
                                          {
                                              mediaTypeId = m.mediaTypeId,
                                              mediaType = m.mediaType,
                                              IsActive = m.IsActive
                                          }).ToList(),
                             paymentModeList = (from pnt in dbContext.NotificationPaymentModeTrans
                                                where pnt.notificationId == notificationId
                                                join pm in dbContext.PaymentModeMsts on pnt.paymentModeId equals pm.paymentModeId
                                                select new PaymentModel
                                                {
                                                    paymentModeId = pm.paymentModeId,
                                                    paymentModeName = pm.paymentModeName,
                                                    IsActive = pm.IsActive
                                                }).ToList()
                         }).FirstOrDefault();
            }
            return model;
        }
        /// <summary>
        /// Get all Schemes for notification
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<SchemeModel> GetAllSchemesForNotifications(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<SchemeModel> allSchemes = new List<SchemeModel>();
                allSchemes = (from schemes in dbContext.SchemeMsts
                              where schemes.completed == true && schemes.IsActive == true && schemes.Status != Constants.SchemeClosed
                              select new SchemeModel
                              {
                                  schemeId = schemes.schemeId,
                                  schemeName = schemes.schemeName
                              }).OrderByDescending(x => x.schemeId).ToList();
                return allSchemes;
            }
        }
        /// <summary>
        /// Get media types like Times of India...
        /// </summary>
        /// <returns></returns>
        public List<MediaModel> GetMediaTypeForNotification()
        {
            var lstMediaType = new List<MediaModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lstMediaType = (from mediaTypeMsts in dbContext.MediaTypeMsts
                                where mediaTypeMsts.IsActive == true
                                select new MediaModel
                                {
                                    mediaTypeId = mediaTypeMsts.mediaTypeId,
                                    mediaType = mediaTypeMsts.mediaType
                                }).ToList();
                return lstMediaType;
            }
        }
        /// <summary>
        /// Get payment mode for notification like Cash, cheque..
        /// </summary>
        /// <returns></returns>
        public List<PaymentModel> GetPaymentModeForNotifications()
        {
            var paymentModeList = new List<PaymentModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                paymentModeList = (from paymentModeMsts in dbContext.PaymentModeMsts
                                   where paymentModeMsts.IsActive == true
                                   select new PaymentModel
                                      {
                                          paymentModeId = paymentModeMsts.paymentModeId,
                                          paymentModeName = paymentModeMsts.paymentModeName
                                      }).ToList();
                return paymentModeList;
            }
        }
        /// <summary>
        /// Add and edit notification
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public int SaveNotificationForScheme(NotificationsModel notification, string user)
        {
            var flag = 0;
            using (var dbContext = new NoidaPMSEntities())
            {

                var scheme = dbContext.SchemeMsts.Where(cond => cond.schemeId == notification.schemeID).FirstOrDefault();
                if (scheme != null)
                {
                    if (scheme.startDate > notification.notificationStart || scheme.endDate < notification.notificationEnd)
                    {
                        flag = 2;
                    }
                    else
                    {
                        var existingNotification = new SchemeNotifTran();
                        if (notification.notificationID == 0)
                        {
                            existingNotification = dbContext.SchemeNotifTrans.FirstOrDefault(cond => cond.schemeId == notification.schemeID && cond.notificationName.ToLower().Trim() == notification.notificationName.ToLower().Trim() && cond.IsActive == true);
                        }
                        else
                        {
                            existingNotification = dbContext.SchemeNotifTrans.FirstOrDefault(cond => cond.schemeId == notification.schemeID && cond.notificationName.ToLower().Trim() == notification.notificationName.ToLower().Trim() && cond.IsActive == true && cond.notificationId != notification.notificationID);
                        }
                        if (existingNotification != null)
                        {
                            flag = 3;
                        }
                        else
                        {
                            existingNotification = dbContext.SchemeNotifTrans.Where(i => i.notificationId == notification.notificationID && i.IsActive == true).FirstOrDefault();


                            if (existingNotification != null)
                            {
                                //property = dbContext.SchemePropTrans.FirstOrDefault(cond => cond.schemeId == schemeId && cond.departmentId == deptId && cond.sectorId == secotrId && cond.blockId == blockId && cond.propertyNo == plotNo && cond.IsActive == true && cond.refId != refId);
                                // Edit notification 
                                //existingNotification.schemeId = notification.schemeID;
                                existingNotification.notificationName = notification.notificationName;
                                existingNotification.startDate = notification.notificationStart;
                                existingNotification.endDate = notification.notificationEnd;
                                existingNotification.bookletCost = notification.bookletCost;
                                existingNotification.oldNotificationId = notification.oldNotificationID;
                                existingNotification.IsActive = true;
                                existingNotification.modifiedDate = DateTime.Now;
                                existingNotification.PublishDate = notification.publishDate;
                                existingNotification.modifiedBy = user;
                                dbContext.SaveChanges();
                                var notificationPaymentModeTransForInActive = dbContext.NotificationPaymentModeTrans.Where(x => x.notificationId == notification.notificationID).ToList();
                                if (notificationPaymentModeTransForInActive.Count > 0)
                                {
                                    foreach (NotificationPaymentModeTran objNotificationPaymentModeTransForInActive in notificationPaymentModeTransForInActive)
                                    {
                                        dbContext.NotificationPaymentModeTrans.Remove(objNotificationPaymentModeTransForInActive);
                                    }
                                    //dbContext.SaveChanges();
                                }
                                NotificationPaymentModeTran notificationPaymentModeTrans;
                                foreach (int paymentMode in notification.schemePaymentType)
                                {
                                    notificationPaymentModeTrans = new NotificationPaymentModeTran();
                                    notificationPaymentModeTrans.notificationId = notification.notificationID;
                                    notificationPaymentModeTrans.paymentModeId = paymentMode;
                                    notificationPaymentModeTrans.IsActive = true;
                                    notificationPaymentModeTrans.modifiedBy = user;
                                    notificationPaymentModeTrans.modifiedDate = DateTime.Now;
                                    dbContext.NotificationPaymentModeTrans.Add(notificationPaymentModeTrans);
                                }
                                //dbContext.SaveChanges();
                                var notificationMediaTranForInActive = dbContext.NotificationMediaTrans.Where(x => x.notificationId == notification.notificationID).ToList();
                                if (notificationMediaTranForInActive.Count > 0)
                                {
                                    foreach (NotificationMediaTran objNotificationMediaTranForInActiveForInActive in notificationMediaTranForInActive)
                                    {
                                        dbContext.NotificationMediaTrans.Remove(objNotificationMediaTranForInActiveForInActive);
                                    }
                                    //dbContext.SaveChanges();
                                }
                                NotificationMediaTran notificationMediaTrans;
                                foreach (int mediaType in notification.schemeMediaType)
                                {
                                    notificationMediaTrans = new NotificationMediaTran();
                                    notificationMediaTrans.notificationId = notification.notificationID;
                                    notificationMediaTrans.mediaTypeId = mediaType;
                                    notificationMediaTrans.IsActive = true;
                                    notificationMediaTrans.modifiedBy = user;
                                    notificationMediaTrans.modifiedDate = DateTime.Now;
                                    dbContext.NotificationMediaTrans.Add(notificationMediaTrans);
                                }
                                dbContext.SaveChanges();
                                flag = 1;
                            }
                            else
                            {
                                // Save notification
                                SchemeNotifTran schemeNotifTrans = new SchemeNotifTran();
                                schemeNotifTrans.schemeId = notification.schemeID;
                                schemeNotifTrans.notificationName = notification.notificationName;
                                schemeNotifTrans.startDate = notification.notificationStart;
                                schemeNotifTrans.endDate = notification.notificationEnd;
                                schemeNotifTrans.bookletCost = notification.bookletCost;
                                schemeNotifTrans.oldNotificationId = notification.oldNotificationID;
                                schemeNotifTrans.createdDate = DateTime.Now;
                                schemeNotifTrans.createdBy = user;
                                schemeNotifTrans.IsActive = true;
                                schemeNotifTrans.PublishDate = notification.publishDate;
                                dbContext.SchemeNotifTrans.Add(schemeNotifTrans);
                                dbContext.SaveChanges();

                                int insertedNotificationId = dbContext.SchemeNotifTrans.Max(u => u.notificationId); //existingNotification.notificationId;

                                NotificationPaymentModeTran notificationPaymentModeTrans;
                                foreach (int paymentMode in notification.schemePaymentType)
                                {
                                    notificationPaymentModeTrans = new NotificationPaymentModeTran();
                                    notificationPaymentModeTrans.notificationId = insertedNotificationId;
                                    notificationPaymentModeTrans.paymentModeId = paymentMode;
                                    notificationPaymentModeTrans.IsActive = true;
                                    notificationPaymentModeTrans.createdBy = user;
                                    notificationPaymentModeTrans.createdDate = DateTime.Now;
                                    dbContext.NotificationPaymentModeTrans.Add(notificationPaymentModeTrans);
                                }
                                //dbContext.SaveChanges();

                                NotificationMediaTran notificationMediaTrans;
                                foreach (int media in notification.schemeMediaType)
                                {
                                    notificationMediaTrans = new NotificationMediaTran();
                                    notificationMediaTrans.notificationId = insertedNotificationId;
                                    // notificationMediaTrans.mediaSubTypeId = mediaSubType;
                                    notificationMediaTrans.mediaTypeId = media;
                                    notificationMediaTrans.IsActive = true;
                                    notificationMediaTrans.createdBy = user;
                                    notificationMediaTrans.createdDate = DateTime.Now;
                                    dbContext.NotificationMediaTrans.Add(notificationMediaTrans);
                                }
                                dbContext.SaveChanges();
                                flag = 1;
                            }
                        }
                    }
                }
            }
            return flag;
        }

        public bool CheckNotificationPublishDate(int schemeId, DateTime notificationStart, DateTime notificationEnd, DateTime publishDate)
        {
            var flag = false;

            if (publishDate >= notificationStart && publishDate <= notificationEnd)
            {
                return flag = true;
            }
            else
            {
                return flag;
            }

            //using (var dbContext = new NoidaPMSEntities())
            //{
            //    var scheme = dbContext.SchemeMsts.Where(s=>s.schemeId==schemeId).FirstOrDefault();
            //    if (scheme != null)
            //    {
            //        return flag = true;
            //    }
            //    else
            //    {
            //        return flag;
            //    }
            //}
        }

        public bool SaveSchemeNotification(NotificationsModel notification)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                SchemeNotifTran notice = new SchemeNotifTran();
                if (notification.notificationID != 0)
                {
                    //to edit notification
                    //notice = dbContext.SchemeNotifTrans.FirstOrDefault(cond => cond.schemeId == notification.schemeID && cond.notificationName.ToLower().Trim() == notification.notificationName.ToLower().Trim() && cond.IsActive == true && cond.notificationId != notification.notificationID);
                    notice = dbContext.SchemeNotifTrans.Where(n => n.notificationId == notification.notificationID).FirstOrDefault();
                    notice.bookletCost = notification.bookletCost;
                    notice.oldNotificationId = notification.oldNotificationID;
                    notice.IsActive = true;
                    notice.modifiedDate = DateTime.Now;
                    notice.PublishDate = notification.publishDate;
                    notice.modifiedBy = userInfo.UserID.ToString();
                    dbContext.SaveChanges();
                    var notificationPaymentModeTransForInActive = dbContext.NotificationPaymentModeTrans.Where(x => x.notificationId == notification.notificationID).ToList();
                    if (notificationPaymentModeTransForInActive.Count > 0)
                    {
                        foreach (NotificationPaymentModeTran objNotificationPaymentModeTransForInActive in notificationPaymentModeTransForInActive)
                        {
                            dbContext.NotificationPaymentModeTrans.Remove(objNotificationPaymentModeTransForInActive);
                        }
                        //dbContext.SaveChanges();
                    }
                    NotificationPaymentModeTran notificationPaymentModeTrans;
                    foreach (int paymentMode in notification.schemePaymentType)
                    {
                        notificationPaymentModeTrans = new NotificationPaymentModeTran();
                        notificationPaymentModeTrans.notificationId = notification.notificationID;
                        notificationPaymentModeTrans.paymentModeId = paymentMode;
                        notificationPaymentModeTrans.IsActive = true;
                        notificationPaymentModeTrans.modifiedBy = userInfo.UserID.ToString();
                        notificationPaymentModeTrans.modifiedDate = DateTime.Now;
                        dbContext.NotificationPaymentModeTrans.Add(notificationPaymentModeTrans);
                    }
                    //dbContext.SaveChanges();
                    var notificationMediaTranForInActive = dbContext.NotificationMediaTrans.Where(x => x.notificationId == notification.notificationID).ToList();
                    if (notificationMediaTranForInActive.Count > 0)
                    {
                        foreach (NotificationMediaTran objNotificationMediaTranForInActiveForInActive in notificationMediaTranForInActive)
                        {
                            dbContext.NotificationMediaTrans.Remove(objNotificationMediaTranForInActiveForInActive);
                        }
                        //dbContext.SaveChanges();
                    }
                    NotificationMediaTran notificationMediaTrans;
                    foreach (int mediaType in notification.schemeMediaType)
                    {
                        notificationMediaTrans = new NotificationMediaTran();
                        notificationMediaTrans.notificationId = notification.notificationID;
                        notificationMediaTrans.mediaTypeId = mediaType;
                        notificationMediaTrans.IsActive = true;
                        notificationMediaTrans.modifiedBy = userInfo.UserID.ToString();
                        notificationMediaTrans.modifiedDate = DateTime.Now;
                        dbContext.NotificationMediaTrans.Add(notificationMediaTrans);
                    }
                    dbContext.SaveChanges();
                    flag = true;
                }
                else
                {
                    // addition of new notification
                    notice = dbContext.SchemeNotifTrans.FirstOrDefault(cond => cond.schemeId == notification.schemeID && cond.notificationName.ToLower().Trim() == notification.notificationName.ToLower().Trim() && cond.IsActive == true);
                    // Save notification
                    SchemeNotifTran schemeNotifTrans = new SchemeNotifTran();
                    schemeNotifTrans.schemeId = notification.schemeID;
                    schemeNotifTrans.notificationName = notification.notificationName;
                    schemeNotifTrans.startDate = notification.notificationStart;
                    schemeNotifTrans.endDate = notification.notificationEnd;
                    schemeNotifTrans.bookletCost = notification.bookletCost;
                    schemeNotifTrans.oldNotificationId = notification.oldNotificationID;
                    schemeNotifTrans.createdDate = DateTime.Now;
                    schemeNotifTrans.createdBy = userInfo.UserID.ToString();
                    schemeNotifTrans.IsActive = true;
                    schemeNotifTrans.PublishDate = notification.publishDate;
                    dbContext.SchemeNotifTrans.Add(schemeNotifTrans);
                    dbContext.SaveChanges();

                    int insertedNotificationId = dbContext.SchemeNotifTrans.Max(u => u.notificationId); //existingNotification.notificationId;

                    NotificationPaymentModeTran notificationPaymentModeTrans;
                    foreach (int paymentMode in notification.schemePaymentType)
                    {
                        notificationPaymentModeTrans = new NotificationPaymentModeTran();
                        notificationPaymentModeTrans.notificationId = insertedNotificationId;
                        notificationPaymentModeTrans.paymentModeId = paymentMode;
                        notificationPaymentModeTrans.IsActive = true;
                        notificationPaymentModeTrans.createdBy = userInfo.UserID.ToString();
                        notificationPaymentModeTrans.createdDate = DateTime.Now;
                        dbContext.NotificationPaymentModeTrans.Add(notificationPaymentModeTrans);
                    }
                    //dbContext.SaveChanges();

                    NotificationMediaTran notificationMediaTrans;
                    foreach (int media in notification.schemeMediaType)
                    {
                        notificationMediaTrans = new NotificationMediaTran();
                        notificationMediaTrans.notificationId = insertedNotificationId;
                        // notificationMediaTrans.mediaSubTypeId = mediaSubType;
                        notificationMediaTrans.mediaTypeId = media;
                        notificationMediaTrans.IsActive = true;
                        notificationMediaTrans.createdBy = userInfo.UserID.ToString();
                        notificationMediaTrans.createdDate = DateTime.Now;
                        dbContext.NotificationMediaTrans.Add(notificationMediaTrans);
                    }
                    dbContext.SaveChanges();
                    flag = true;
                }

            }
            return flag;
        }
        /// <summary>
        /// deactivate a particular notification 
        /// </summary>
        /// <param name="notificationID"></param>
        /// <returns></returns>
        public bool RemoveNotification(int notificationID)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var notification = dbContext.SchemeNotifTrans.FirstOrDefault(cond => cond.notificationId == notificationID);
                notification.IsActive = false;
                dbContext.SaveChanges();
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// Get old notification based on scheme id whose new notification is going to generate 
        /// </summary>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public List<NotificationsModel> GetOldNotifications(int schemeId, int notiId)
        {
            var lstNotiModel = new List<NotificationsModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lstNotiModel = (from lstOldNotification in dbContext.SchemeNotifTrans
                                where lstOldNotification.IsActive == true && lstOldNotification.schemeId == schemeId && lstOldNotification.notificationId != notiId
                                select
                                    new NotificationsModel
                                    {
                                        oldNotificationID = lstOldNotification.notificationId,
                                        oldNotificationName = lstOldNotification.notificationName
                                    }).ToList();
                return lstNotiModel;
            }
        }

        public bool CompareNotiStartAndEndDate(DateTime notiStartDate, DateTime notiEndDate, int schemeId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var scheme = new SchemeMst();
                scheme = dbContext.SchemeMsts.Where(cond => cond.schemeId == schemeId).FirstOrDefault();
                if (scheme != null)
                {
                    if (scheme.startDate > notiStartDate || scheme.endDate < notiEndDate)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }
            return flag;
        }

        #endregion

        public int AddCircleRate(int departmentId, int sector, decimal rate, DateTime startDate, int blockId)
        {
            var objcircleRate = new CircleRateMst();
            int result = 0;
            using (var dbcontext = new NoidaPMSEntities())
            {
                var recordExit = dbcontext.CircleRateMsts.Where(d => d.departmentId == departmentId && d.sectorId == sector && EntityFunctions.TruncateTime(d.effectiveFrom) == EntityFunctions.TruncateTime(startDate.Date) && d.blockId == blockId && d.IsActive == true).FirstOrDefault();

                var recordSameExit = dbcontext.CircleRateMsts.Where(d => d.departmentId == departmentId && d.sectorId == sector && d.blockId == blockId && d.IsActive == true && d.circleRate == rate).FirstOrDefault();
                if (recordSameExit != null)
                {
                    if (recordSameExit.effectiveTo == null)
                    {
                        result = 4;
                        return result;
                    }
                }

                var objEndDate = dbcontext.CircleRateMsts.Where(d => d.departmentId == departmentId && d.sectorId == sector && d.IsActive == true).OrderByDescending(d => d.effectiveFrom).FirstOrDefault();


                //if (objEndDate != null)
                //{

                //   var dateComapir = objEndDate.effectiveFrom < startDate;
                //    if (dateComapir==false)
                //    {
                //    result = 3;
                //    return result;
                //    }

                //}
                //var objEndDate = dbcontext.CircleRateMsts.Where(d => d.departmentId == departmentId && d.sectorId == sector && d.IsActive == true).ToList();
                //var pp = objEndDate.OrderByDescending(d => d.effectiveFrom).FirstOrDefault();

                var endDate = startDate.AddDays(-1);

                if (recordExit != null)
                {
                    result = 2;
                    return result;
                }
                if (objEndDate != null)
                {

                    var dateComapir = objEndDate.effectiveFrom < startDate;
                    if (dateComapir == false)
                    {
                        result = 3;
                        return result;
                    }
                    else
                    {

                        objcircleRate.departmentId = departmentId;
                        objcircleRate.sectorId = sector;
                        objcircleRate.circleRate = rate;
                        objcircleRate.effectiveFrom = startDate;
                        objcircleRate.IsActive = true;
                        objcircleRate.createdBy = userInfo.UserID.ToString();
                        objcircleRate.createdDate = DateTime.Now;
                        objcircleRate.blockId = blockId;
                        dbcontext.CircleRateMsts.Add(objcircleRate);
                        if (objEndDate != null)
                        {

                            objEndDate.effectiveTo = endDate;
                        }
                        dbcontext.SaveChanges();
                        result = 1;
                    }

                }
                else
                {

                    objcircleRate.departmentId = departmentId;
                    objcircleRate.sectorId = sector;
                    objcircleRate.circleRate = rate;
                    objcircleRate.effectiveFrom = startDate;
                    objcircleRate.IsActive = true;
                    objcircleRate.createdBy = userInfo.UserID.ToString();
                    objcircleRate.createdDate = DateTime.Now;
                    objcircleRate.blockId = blockId;
                    dbcontext.CircleRateMsts.Add(objcircleRate);
                    if (objEndDate != null)
                    {

                        objEndDate.effectiveTo = endDate;
                    }
                    dbcontext.SaveChanges();
                    result = 1;
                }

            }
            return result;
        }
        public DataSourceResult GetCircleRate(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var objCircleRateList = (from circleRate in dbContext.CircleRateMsts
                                         join dept in dbContext.DepartmentMsts on circleRate.departmentId equals dept.departmentId
                                         join sctor in dbContext.SectorMsts on circleRate.sectorId equals sctor.sectorId
                                         where circleRate.IsActive == true
                                         select new CircleRateModel
                                         {
                                             departmentId = circleRate.departmentId,
                                             departmentName = dept.departmentName,
                                             StartDate = circleRate.effectiveFrom,
                                             EndDate = circleRate.effectiveTo == null ? null : circleRate.effectiveTo,
                                             sector = circleRate.sectorId.Value,
                                             rate = circleRate.circleRate,
                                             sectorName = sctor.sectorName,
                                             blockName = (from uname in dbContext.BlockMsts where uname.blockId == circleRate.blockId && uname.IsActive == true select uname.blockName).FirstOrDefault(),
                                             refid = circleRate.refId
                                         }).ToList();
                return objCircleRateList.ToDataSourceResult(request);
            }


        }
        public bool RemoveCircleRate(int refId)
        {
            bool flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var objresult = dbContext.CircleRateMsts.FirstOrDefault(id => id.refId == refId);
                if (objresult != null)
                {
                    objresult.IsActive = false;
                    dbContext.SaveChanges();
                    flag = true;
                }
                return flag;
            }
        }

        public NotificationsModel GetSchemeDateForNotification(int schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var notifscheme = (from scheme in dbContext.SchemeMsts
                                   where scheme.schemeId == schemeId && scheme.IsActive == true
                                   select new NotificationsModel
                                   {
                                       notificationStart = scheme.startDate,
                                       notificationEnd = scheme.endDate
                                   }).FirstOrDefault();
                return notifscheme;
            }
        }

        //modified on 13 oct 2017 error due to nullable field
        public DataSourceResult GetSubLeasePropertyList(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var sublease = (from prop in dbContext.SchemePropTrans
                                where prop.ParentPropertyId != null
                                select new SubLeaseViewModel
                                {
                                    Id = prop.refId,
                                    PropertyId = prop.propertyId,
                                    ParentPropertyId = prop.ParentPropertyId,
                                    DepartmentId = prop.departmentId,
                                    Department = prop.DepartmentMst.departmentName,
                                    PropertyRate = prop.landRatePerSqmt,
                                    PropertyCost = prop.propertyCost,
                                    TotalCost = prop.totalPropertyCost,
                                    PropertyArea = prop.totalArea,
                                    SectorId = prop.sectorId,
                                    Sector = prop.SectorMst.sectorName,
                                    BlockId = prop.blockId,
                                    Block = prop.BlockMst.blockName,
                                    PlotNo = prop.propertyNo,
                                    SubLeaseStatus = prop.IsActive == true ? "Active" : "InActive",
                                    RegistrationId = dbContext.AllotmentMasters.Where(a => a.propertyId == prop.propertyId).Select(r => r.rid).FirstOrDefault(),
                                    ParentRegistrationId = dbContext.AllotmentMasters.Where(a => a.propertyId == prop.ParentPropertyId).Select(r => r.rid).FirstOrDefault()
                                });
                if (sublease != null)
                {
                    return sublease.ToDataSourceResult(request);
                }
                else
                {
                    return null;
                }
                //return sublease;
            }
        }


        public SubLeaseViewModel GetSubleasePropertyDetail(string id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int? propid = Convert.ToInt32(id);
                var sublease = (from prop in dbContext.SchemePropTrans
                                where prop.refId == propid //&& prop.ParentPropertyId != null
                                select new SubLeaseViewModel
                                {
                                    Id = prop.refId,
                                    PropertyId = prop.propertyId,
                                    ParentPropertyId = prop.ParentPropertyId,
                                    DepartmentId = prop.departmentId,
                                    Department = prop.DepartmentMst.departmentName,
                                    PropertyRate = prop.landRatePerSqmt,
                                    PropertyCost = prop.propertyCost,
                                    PropertyArea = prop.totalArea,
                                    Sector = prop.SectorMst.sectorName,
                                    Block = prop.BlockMst.blockName,
                                    PlotNo = prop.propertyNo,
                                    //RegistrationId = dbContext.AllotmentMasters.Where(a => a.propertyId == prop.propertyId).Select(r => r.rid).FirstOrDefault(),
                                    //Applicant = dbContext.AllotmentMasters.Where(a => a.propertyId == prop.propertyId).Select(n => n.ApplicationDetail.tFirstName).FirstOrDefault()
                                }).FirstOrDefault();
                var alotment = dbContext.AllotmentMasters.FirstOrDefault(a => a.propertyId == sublease.PropertyId && a.isActive == 1);
                if (alotment != null)
                {
                    var form = dbContext.ApplicationDetails.FirstOrDefault(c => c.registrationId == alotment.rid);
                    sublease.RegistrationId = alotment.rid;
                    sublease.Applicant = form.tGender == Constants.Company ? form.T_Company_Name : form.tFirstName + " " + form.tMiddleName + " " + form.tLastName;
                    sublease.ApplicantMaster = form.tGender == Constants.Company ? form.tSigningAuthority : form.tFatherHusbandName;
                    sublease.ApplicantAddress = form.tCorrespondanceAdd;
                    sublease.Email = form.tEmail;
                    sublease.MobileNo = form.tMobileNumber;
                }
                return sublease;
            }
        }

        #region Add Sector & Block
        public bool AddSectorBlock(SectorBlock objSectorBlock)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (objSectorBlock.MasterTypeId == 1)
                {
                    var existingSector = dbContext.SectorMsts.FirstOrDefault(cond => cond.sectorName.ToLower() == objSectorBlock.MasterTypeValue.ToLower());
                    if (existingSector == null)
                    {
                        if (!string.IsNullOrEmpty(objSectorBlock.MasterTypeValue))
                        {
                            if (objSectorBlock.MasterTypeValue.Length <= 100)
                            {
                                SectorMst objSectorMst = new SectorMst();
                                objSectorMst.sectorName = objSectorBlock.MasterTypeValue;
                                objSectorMst.createdBy = objSectorBlock.createdBy;
                                objSectorMst.createdDate = DateTime.Now;
                                objSectorMst.IsActive = true;
                                dbContext.SectorMsts.Add(objSectorMst);
                                dbContext.SaveChanges();
                                flag = true;
                            }
                        }
                    }
                }

                if (objSectorBlock.MasterTypeId == 2)
                {
                    var existingBlock = dbContext.BlockMsts.FirstOrDefault(cond => cond.blockName.ToLower() == objSectorBlock.MasterTypeValue.ToLower());
                    if (existingBlock == null)
                    {
                        if (!string.IsNullOrEmpty(objSectorBlock.MasterTypeValue))
                        {
                            if (objSectorBlock.MasterTypeValue.Length <= 10)
                            {
                                BlockMst objBlockMst = new BlockMst();
                                objBlockMst.blockName = objSectorBlock.MasterTypeValue;
                                objBlockMst.createdBy = objSectorBlock.createdBy;
                                objBlockMst.createdDate = DateTime.Now;
                                objBlockMst.IsActive = true;
                                dbContext.BlockMsts.Add(objBlockMst);
                                dbContext.SaveChanges();
                                flag = true;
                            }
                        }
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// return Sector & Block DDL
        /// </summary>
        /// <returns></returns>
        public List<MasterTypeForSectorBlock> GetDDLSectorBlock()
        {
            List<MasterTypeForSectorBlock> ddlist = new List<MasterTypeForSectorBlock>();
            foreach (int value in Enum.GetValues(typeof(DDLSectorBlock)))
            {
                ddlist.Add(new MasterTypeForSectorBlock
                {
                    MasterName = Enum.GetName(typeof(DDLSectorBlock), value),
                    MasterValue = Enum.GetName(typeof(DDLSectorBlock), value),
                    MasterId = value
                });
            }
            return ddlist;
        }

        #endregion


        public SubLeaseViewModel GetParentPropertyDetailById(int rid)
        {
            var details = new SubleaseModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                var property = (from prop in dbContext.SchemePropTrans
                                join alot in dbContext.AllotmentMasters on prop.propertyId equals alot.propertyId
                                where alot.rid == rid && alot.isActive == 1 && prop.IsActive == true
                                select new SubLeaseViewModel
                                {
                                    Id = prop.propertyId,
                                    PropertyId = prop.propertyId,
                                    SectorId = prop.sectorId,
                                    Sector = prop.SectorMst.sectorName,
                                    BlockId = prop.blockId,
                                    Block = prop.BlockMst.blockName,
                                    PlotNo = prop.propertyNo,
                                    SchemeId = prop.schemeId,
                                    PropertyTypeId = prop.propertyTypeId,
                                    AreaRangeId = prop.floorId,
                                    DepartmentId = prop.departmentId,
                                    Department = prop.DepartmentMst.departmentName,
                                    RegistrationId = alot.rid,
                                    SubLeaseStatus = prop.IsActive == true ? "Active" : "InActive",
                                }).FirstOrDefault();
                if (property == null)
                {
                    property = new SubLeaseViewModel();
                    property.IsRIDValid = false;
                }
                return property;
            }
        }


        public DataSourceResult GetSubLeasedProperty(DataSourceRequest request, int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var Allotment = dbContext.AllotmentMasters.FirstOrDefault(x => x.rid == rid);
                if (Allotment != null)
                {
                    var Property = (from prop in dbContext.SchemePropTrans
                                    where prop.ParentPropertyId == Allotment.propertyId //&& prop.IsActive == true
                                    select new SubLeaseViewModel
                                    {
                                        Id = prop.refId,
                                        PropertyId = prop.propertyId,
                                        ParentPropertyId = prop.ParentPropertyId,
                                        DepartmentId = prop.departmentId,
                                        Department = prop.DepartmentMst.departmentName,
                                        Sector = prop.SectorMst.sectorName,
                                        Block = prop.BlockMst.blockName,
                                        SubLeasePlot = prop.propertyNo,
                                        PropertyArea = prop.totalArea,
                                        IsPropertyActive = prop.IsActive,
                                        SubLeaseStatus = prop.IsActive == true ? "Active" : "InActive",
                                        RegistrationId = dbContext.AllotmentMasters.Where(a => a.propertyId == prop.propertyId).Select(x => x.rid).FirstOrDefault(),
                                        Project = prop.groupProjectId != null ? dbContext.GroupHousingProjectMsts.FirstOrDefault(m => m.Status == true && m.Id == prop.groupProjectId).ProjectName : string.Empty
                                    });
                    return Property.ToDataSourceResult(request);
                }
                else return null;
            }
        }


        public int SaveSubLeaseProperty(SubLeaseViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var property = (from prop in dbContext.SchemePropTrans
                                //join alot in dbContext.AllotmentMasters on prop.propertyId equals alot.propertyId
                                where prop.ParentPropertyId == model.ParentPropertyId && prop.sectorId == model.SectorId && prop.blockId == model.BlockId && prop.propertyNo == model.SubLeasePlot
                                select prop).FirstOrDefault();
                if (property == null)
                {
                    var newSubLease = new SchemePropTran();
                    newSubLease.schemeId = model.SchemeId;
                    newSubLease.departmentId = model.DepartmentId;
                    newSubLease.propertyTypeId = model.PropertyTypeId;
                    newSubLease.sectorId = model.SectorId;
                    newSubLease.blockId = model.BlockId;
                    newSubLease.floorId = model.AreaRangeId;
                    //PropertyID 
                    //newSubLease.floorId = model.AreaRangeId;
                    newSubLease.propertyNo = model.SubLeasePlot;
                    newSubLease.propertyCost = model.PropertyCost;
                    newSubLease.landRatePerSqmt = model.PropertyRate;
                    newSubLease.totalArea = model.PropertyArea;
                    newSubLease.Registry = "SubLease";
                    newSubLease.totalPropertyCost = model.PropertyCost;
                    newSubLease.allotmentMoney = Convert.ToDecimal(0.0); // As discussed with Vishal Shukla, 0.0 instead of null 
                    newSubLease.IsActive = true;
                    newSubLease.createdBy = userInfo.UserID.ToString();
                    newSubLease.createdDate = DateTime.Now;
                    newSubLease.ParentPropertyId = model.PropertyId;

                    //Added on 16Nov2017
                    newSubLease.groupProjectId = model.ProjectId;

                    dbContext.SchemePropTrans.Add(newSubLease);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                else
                {
                    property.schemeId = model.SchemeId;
                    property.departmentId = model.DepartmentId;
                    property.propertyTypeId = model.PropertyTypeId;
                    property.sectorId = model.SectorId;
                    property.blockId = model.BlockId;
                    property.floorId = model.AreaRangeId;
                    //PropertyID 
                    //property.floorId = model.AreaRangeId;
                    property.propertyNo = model.SubLeasePlot;
                    property.propertyCost = model.PropertyCost;
                    property.landRatePerSqmt = model.PropertyRate;
                    property.totalArea = model.PropertyArea;
                    property.Registry = "SubLease";
                    property.totalPropertyCost = model.PropertyCost;
                    property.allotmentMoney = Convert.ToDecimal(0.0);
                    property.IsActive = true;
                    property.modifiedBy = userInfo.UserID.ToString();
                    property.modifiedDate = DateTime.Now;
                    property.ParentPropertyId = model.PropertyId;

                    //Added on 16Nov2017
                    property.groupProjectId = model.ProjectId;

                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }


        public DataSourceResult GetRegistrationIdListForSubLease(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var ridlist = (from alotment in dbContext.AllotmentMasters
                               where alotment.departmentId == NADepartment.Institutional || alotment.departmentId == NADepartment.Commercial
                               && alotment.departmentId == NADepartment.GroupHousing && alotment.isActive == 1
                               select new DropdownViewModel
                               {
                                   Id = alotment.rid,
                                   Text = alotment.rid.ToString()
                               });
                return ridlist.ToDataSourceResult(request);
            }

        }


        public int SubLeasePlotActivation(int Id)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var subLease = (from spt in dbContext.SchemePropTrans where spt.refId == Id select spt).FirstOrDefault();
                if (subLease != null)
                {
                    if (subLease.IsActive == true) subLease.IsActive = false;
                    else subLease.IsActive = true;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                else flag = ReturnType.NotExist;
            }
            return flag;
        }

        #region Property Projects
        //Get project by ParentRid
        public List<DropdownViewModel> GetProjectsByParentRid(int ParentPropertyId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from project in dbContext.GroupHousingProjectMsts
                           where project.Status == true && project.PropertyId == ParentPropertyId
                           orderby project.Rid descending
                           select new DropdownViewModel
                           {
                               Id = project.Id,
                               Text = project.ProjectName
                           }).ToList();
                return lst;
            }
        }

        //Get project by Rid for grid
        public DataSourceResult GetProjectsByRidForGrid(DataSourceRequest Req, int Rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from project in dbContext.GroupHousingProjectMsts
                           where project.Status == true && project.Rid == Rid
                           orderby project.Id descending
                           select new ProjectModel
                           {
                               ParentRegistrationId = project.Rid,
                               ParentPropertyId = project.PropertyId,
                               ProjectId = project.Id,
                               Project = project.ProjectName
                           });
                return lst.ToDataSourceResult(Req);
            }
        }

        //Save Property Projects
        public int SavePropertyProject(ProjectViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var objproject = (from project in dbContext.GroupHousingProjectMsts
                                  where project.Status == true && project.Rid == model.RegistrationId && project.Id == model.ProjectId
                                  select project).FirstOrDefault();
                if (objproject == null)
                {
                    var data = dbContext.GroupHousingProjectMsts.FirstOrDefault(m => m.Status == true && m.Rid == model.RegistrationId && m.ProjectName == model.Project);
                    if (data == null)
                    {
                        var objGroupHousingProject = new GroupHousingProjectMst();
                        objGroupHousingProject.Rid = model.RegistrationId;
                        objGroupHousingProject.PropertyId = model.PropertyId;
                        objGroupHousingProject.ProjectName = model.Project;
                        objGroupHousingProject.Status = true;
                        objGroupHousingProject.CreatedBy = userInfo.UserID;
                        objGroupHousingProject.CreatedDate = DateTime.Now;
                        dbContext.GroupHousingProjectMsts.Add(objGroupHousingProject);
                        dbContext.SaveChanges();
                        flag = ReturnType.Saved;
                    }
                    else { flag = ReturnType.Exist; }
                }
                else
                {
                    if (objproject.ProjectName != model.Project)
                    {
                        objproject.Rid = model.RegistrationId;
                        objproject.PropertyId = model.PropertyId;
                        objproject.ProjectName = model.Project;
                        objproject.Status = true;
                        objproject.ModifiedBy = userInfo.UserID;
                        objproject.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Updated;
                    }
                    else { flag = ReturnType.Exist; }
                }
            }
            return flag;
        }

        /// <summary>
        /// Fetches RIDs for Add Project only Parent RID (Group Housing)
        /// </summary>
        /// <returns></returns>
        public DataSourceResult GetRIDsForProject(DataSourceRequest Request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstDeptts = (from u in dbContext.UmUserMasters
                                 join d in dbContext.UmUserDepartmentTrans on u.UserRefId equals d.UserRefId
                                 where u.UserRefId == userInfo.UserID
                                 select d.DepartmentId).ToList();
                var lst = (from f in dbContext.AllotmentMasters
                           join spt in dbContext.SchemePropTrans on f.propertyId equals spt.propertyId
                           where f.isActive == 1 && spt.ParentPropertyId == null && f.departmentId == 6 && lstDeptts.Contains(f.departmentId) && f.isStatus.ToLower() == Common.AllotmentStatus.Approved.ToString().ToLower() && spt.ParentPropertyId == null
                           orderby f.createdDate descending
                           select new DDList
                           {
                               id = f.rid,
                               text = f.rid.ToString()
                           });
                return lst.ToDataSourceResult(Request);
            }
        }

        public int UpdateProjectStatus(ProjectModel ProjectModel)
        {
            var flag = ReturnType.None;
            using (var dbcontext = new NoidaPMSEntities())
            {
                var data = dbcontext.GroupHousingProjectMsts.FirstOrDefault(m => m.Id == ProjectModel.ProjectId && m.Status == true);
                if (data != null)
                {
                    data.Status = false;
                    dbcontext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                else
                {
                    flag = ReturnType.NotExist;
                }
            }
            return flag;
        }
        #endregion


        public PropertyModel GetPropertyDetailById(int refId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from property in dbContext.SchemePropTrans
                              join propcost in dbContext.SchemeCostTrans on new { x1 = property.schemeId, x2 = property.departmentId, x3 = property.sectorId, x4 = property.propertyTypeId, x5 = property.floorId } equals new { x1 = propcost.schemeId, x2 = propcost.departmentId, x3 = propcost.sectorId, x4 = propcost.propertyTypeId, x5 = propcost.floorId }
                              into landcost
                              from cost in landcost.DefaultIfEmpty()
                              join deptcost in dbContext.SchemeDepartmentTrans on new { d1 = property.schemeId, d2 = property.departmentId } equals new { d1 = deptcost.schemeId, d2 = deptcost.departmentId }
                              into departmentCost
                              from dcost in departmentCost.DefaultIfEmpty()
                              join location in dbContext.PropertyLocationChargesTrans on property.propertyId equals location.propertyId
                              into locationTrans
                              from lcharge in locationTrans.DefaultIfEmpty()
                              where property.refId == refId && property.IsActive == true
                              select new PropertyModel
                              {
                                  refId = property.refId,
                                  schemeId = property.schemeId,
                                  schemeName = property.SchemeMst.schemeName,
                                  propertyTypeId = property.PropertyTypeMst.propertyTypeId,
                                  propertyType = property.PropertyTypeMst.propertyTypeName,
                                  sectorId = property.SectorMst.sectorId,
                                  sectorName = property.SectorMst.sectorName,
                                  blockId = property.BlockMst.blockId,
                                  blockName = property.BlockMst.blockName,
                                  departmentId = property.DepartmentMst.departmentId,
                                  departmentName = property.DepartmentMst.departmentName,
                                  floorId = property.FloorMst.floorId,
                                  floorName = property.FloorMst.floorName,
                                  createdBy = property.createdBy,
                                  lastModifiedDate = property.modifiedDate,
                                  plotProperty = property.propertyNo,
                                  totalArea = property.totalArea,
                                  coveredArea = property.coveredArea,
                                  actualArea = property.actualArea,
                                  RegistryName = property.Registry,
                                  TotalPropertyCost = property.totalPropertyCost,
                                  LandRatePerSqMet = property.landRatePerSqmt,
                                  LandRate = cost.landRatePerSqmt,
                                  SchemeStatus = property.SchemeMst.Status,
                                  ProjectId = property.groupProjectId,//Added on 21 Nov 2017 for group housing
                                  PropertyNo = property.propertyId,
                                  PropertyId = property.propertyId,
                                  ParentPropertyId = property.ParentPropertyId,

                                  IsLocationCharged = dbContext.PropertyLocationChargesTrans.FirstOrDefault(l => l.propertyId == property.propertyId) == null ? false : true,
                                  locationCharges = dbContext.PropertyLocationChargesTrans.FirstOrDefault(l => l.propertyId == property.propertyId) == null ? false : true,
                                  charges = lcharge.charges,
                                  locationId = lcharge.locationId,
                                  locationName = lcharge.LocationMst.locationName,
                                  //locationName = dbContext.LocationMsts.Where(p => p.locationId == lcharge.locationId).Select(p => p.locationName).FirstOrDefault(),


                                  allotementMoney = cost.allotmentMoney == null ? property.allotmentMoney : cost.allotmentMoney,
                                  leaseRent = cost.leaseRent,
                                  civilCost = cost.civilCost == null ? property.civilCost : cost.civilCost,
                                  PropertyCost = cost.propertyCost == null ? property.propertyCost : cost.propertyCost,

                                  allotementMoneyPercentage = dcost.allotmentMoneyPercent,
                                  floorAreaRatio = dcost.far,
                                  leaseRentPercentage = dcost.leaseRentPercent,

                                  IsAllotted = dbContext.AllotmentMasters.Where(p => p.propertyId == property.propertyId && p.isStatus == Status.Approved).Select(p => p.rid).FirstOrDefault(),
                                  RegistrationId = dbContext.AllotmentMasters.Where(p => p.propertyId == property.propertyId).Select(a => a.rid).FirstOrDefault()
                              }).FirstOrDefault();

                return detail;
            }
        }


        public DataSourceResult GetMasterSearchParameterAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "Scheme")
                {
                    var list = (from property in dbContext.SchemePropTrans
                                join scheme in dbContext.SchemeMsts on property.schemeId equals scheme.schemeId
                                where property.IsActive == true && scheme.completed == true
                                && (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                                select new DropdownViewModel
                                {
                                    Id = scheme.schemeId,
                                    Text = scheme.schemeName,
                                    DepartmentId = model.DepartmentId
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }
                else if (model.ActionType == "Department")
                {
                    var list = (from property in dbContext.SchemePropTrans
                                join department in dbContext.DepartmentMsts on property.departmentId equals department.departmentId
                                where property.IsActive == true
                                && (model.SchemeId == null || property.schemeId == model.SchemeId)
                                select new DropdownViewModel
                                {
                                    Id = department.departmentId,
                                    Text = department.departmentName,
                                    SchemeId = model.SchemeId
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }
                else if (model.ActionType == "Sector")
                {
                    var list = (from property in dbContext.SchemePropTrans
                                join sector in dbContext.SectorMsts on property.sectorId equals sector.sectorId
                                where property.IsActive == true
                                && (model.SchemeId == null || property.schemeId == model.SchemeId)
                                && (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                                select new DropdownViewModel
                                {
                                    Id = sector.sectorId,
                                    Text = sector.sectorName,
                                    SchemeId = model.SchemeId,
                                    DepartmentId = model.DepartmentId
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }
                else if (model.ActionType == "Block")
                {
                    var list = (from property in dbContext.SchemePropTrans
                                from block in dbContext.BlockMsts.Where(b => b.blockId == property.blockId).DefaultIfEmpty()
                                where property.IsActive == true
                                && (model.SchemeId == null || property.schemeId == model.SchemeId)
                                && (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                                && (model.SectorId == null || property.sectorId == model.SectorId)
                                select new DropdownViewModel
                                {
                                    Id = property.blockId == null ? 0 : block.blockId,
                                    Text = block.blockName,
                                    SchemeId = model.SchemeId,
                                    DepartmentId = model.DepartmentId,
                                    SectorId = model.SectorId
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }
                else if (model.ActionType == "Plot")
                {
                    var list = (from property in dbContext.SchemePropTrans
                                //from block in dbContext.BlockMsts.Where(b => b.blockId == property.blockId).DefaultIfEmpty()
                                where property.IsActive == true
                                && (model.SchemeId == null || property.schemeId == model.SchemeId)
                                && (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                                && (model.SectorId == null || property.sectorId == model.SectorId)
                                && (model.BlockId == null || property.blockId == model.BlockId)
                                select new DropdownViewModel
                                {
                                    Id = property.refId,
                                    Text = property.propertyNo,
                                    SchemeId = model.SchemeId,
                                    DepartmentId = model.DepartmentId,
                                    SectorId = model.SectorId,
                                    BlockId = model.BlockId
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }
                else if (model.ActionType == "RegistrationId")
                {
                    var list = (from property in dbContext.SchemePropTrans
                                from alotment in dbContext.AllotmentMasters.Where(a => a.propertyId == property.propertyId).DefaultIfEmpty()
                                where property.IsActive == true && alotment.isActive == 1
                                && (model.SchemeId == null || property.schemeId == model.SchemeId)
                                && (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                                && (model.SectorId == null || property.sectorId == model.SectorId)
                                && (model.BlockId == null || property.blockId == model.BlockId)
                                && (model.RegistrationId == null || alotment.rid == model.RegistrationId)
                                select new DropdownViewModel
                                {
                                    Id = alotment.rid,
                                    Text = alotment.rid.ToString(),
                                    SchemeId = model.SchemeId,
                                    DepartmentId = model.DepartmentId,
                                    SectorId = model.SectorId,
                                    BlockId = model.BlockId
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    return null;
                }
            }
        }

        public DataSourceResult GetMasterPropertyList(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from property in dbContext.SchemePropTrans
                            from block in dbContext.BlockMsts.Where(b => b.blockId == property.blockId).DefaultIfEmpty()
                            from propertytype in dbContext.PropertyTypeMsts.Where(t => t.propertyTypeId == property.propertyTypeId).DefaultIfEmpty()
                            from floorarea in dbContext.FloorMsts.Where(a => a.floorId == property.floorId).DefaultIfEmpty()
                            from locationcharge in dbContext.PropertyLocationChargesTrans.Where(l => l.propertyId == property.propertyId).DefaultIfEmpty()
                            from alotment in dbContext.AllotmentMasters.Where(c => c.propertyId == property.propertyId && c.isActive == 1).DefaultIfEmpty()
                            where DepartmentList.Contains(property.departmentId.Value)
                            && (model.Id == null || property.refId == model.Id)
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
                                PropertyTypeId = propertytype.propertyTypeId,
                                PropertyType = propertytype.propertyTypeName,
                                SectorId = property.sectorId,
                                SectorName = property.SectorMst.sectorName,
                                BlockId = block.blockId,
                                BlockName = block.blockName,
                                PlotNo = property.propertyNo,
                                FloorId = floorarea.floorId,
                                FloorArea = floorarea.floorName,
                                TotalArea = property.totalArea,
                                CoveredArea = property.coveredArea,
                                ActualArea = property.actualArea,
                                LocationCharge = locationcharge.charges,
                                LocationId = locationcharge.locationId,
                                IsLocationCharged = locationcharge.charges == null ? false : true,
                                Registry = property.Registry,
                                PropertyCost = property.propertyCost,
                                CivilCost = property.civilCost,
                                TotalPropertyCost = property.totalPropertyCost,
                                LandRate = property.landRatePerSqmt,
                                //SchemeStatus = property.SchemeMst.Status,
                                RegistrationId = alotment.rid,
                                Location = dbContext.LocationMsts.Where(p => p.locationId == locationcharge.locationId).Select(p => p.locationName).FirstOrDefault(),
                                //IsPropertyAllotted = dbContext.AllotmentMasters.Where(p => p.propertyId == property.propertyId && p.isStatus == AllotmentStatus.Approved.ToString()).Select(p => p.rid).FirstOrDefault()
                                IsPropertyAllotted = dbContext.AllotmentMasters.FirstOrDefault(r => r.propertyId == property.propertyId && r.isActive == Constants.Active) != null ? true : false
                            });

                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetPropertyRateListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from rate in dbContext.PropertyRateMsts
                            join department in dbContext.DepartmentMsts on rate.DepartmentId equals department.departmentId
                            //join sector in dbContext.SectorMsts on rate.SectorId equals sector.sectorId
                            where (model.Id == null || rate.Id == model.Id)
                                && (model.DepartmentId == null || rate.DepartmentId == model.DepartmentId)
                                && (model.SectorId == null || rate.SectorId == model.SectorId)
                                && (model.BlockId == null || rate.BLockId == model.BlockId)
                                && (model.PropertyTypeId == null || rate.PropertyTypeId == model.PropertyTypeId)
                            select new PropertyViewModel
                            {
                                Id = rate.Id,
                                DepartmentId = rate.DepartmentId,
                                Department = department.departmentName,
                                SchemeName = rate.SchemeName,
                                PropertyTypeId = rate.PropertyTypeId,
                                PropertyType = rate.PropertyTypeId == null ? string.Empty : dbContext.PropertyTypeMsts.FirstOrDefault(r => r.propertyTypeId == rate.PropertyTypeId).propertyTypeName,
                                AllotmentMethod = rate.AllotmentMethod,
                                AreaPhase = rate.Phase,
                                PropertyRateStartDate = rate.StartDate,
                                PropertyRateEndDate = rate.EndDate == null ? null : rate.EndDate,
                                AllotmentYear = rate.AllotmentYear,
                                AreaRange = rate.AreaRange,
                                SectorId = rate.SectorId,
                                LandRate = rate.PropertyRate,
                                AllotmentMoney = rate.PlotPremium,
                                NormalInterest = rate.NormalInterest,
                                PenalInterest = rate.PenalInterest,
                                SectorName = rate.SectorId == null ? rate.SectorName : dbContext.SectorMsts.FirstOrDefault(c=>c.sectorId==rate.SectorId).sectorName,
                                //SectorName = .sectorName,
                                BlockName = rate.BLockId == null ? "NA" : dbContext.BlockMsts.FirstOrDefault(b => b.blockId == rate.BLockId).blockName,
                                IsActive = rate.IsActive
                            }).ToList();
                return list.ToDataSourceResult(request);
            }
        }

        public int SavePropertyRateDetail(PropertyViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbcontext = new NoidaPMSEntities())
            {
                if (model.DepartmentId == NADepartment.Industrial)
                {
                    //var exRate = dbcontext.PropertyRateMsts.Where(d => d.DepartmentId == model.DepartmentId && d.PropertyTypeId == model.PropertyTypeId && d.Phase == model.AreaPhase).FirstOrDefault();
                    var exRate = dbcontext.PropertyRateMsts.Where(d => d.DepartmentId == model.DepartmentId && d.PropertyTypeId == model.PropertyTypeId && d.Phase == model.AreaPhase && DbFunctions.TruncateTime(d.StartDate) >= DbFunctions.TruncateTime(model.PropertyRateStartDate) && d.AreaRange == model.AreaRange).FirstOrDefault();
                    if (exRate != null)
                    {
                        flag = ReturnType.Exist;
                    }
                    else
                    {
                        flag = SaveAuthorityPropertyRateDetail(model);
                    }
                }
                else
                {
                    //var existingRate = dbcontext.PropertyRateMsts.Where(d => d.DepartmentId == model.DepartmentId && d.SectorId == model.SectorId && d.BLockId == model.BlockId && d.PropertyTypeId == model.PropertyTypeId && d.AreaRange == model.AreaRange && DbFunctions.TruncateTime(d.StartDate) >= DbFunctions.TruncateTime(model.PropertyRateStartDate)).FirstOrDefault();

                    //if (existingRate != null)
                    //{
                    //    if (existingRate.EndDate == null || existingRate.EndDate > model.PropertyRateStartDate)
                    //    {
                    //        flag = ReturnType.Exist;
                    //    }
                    //}
                    //else
                    //{
                    //    flag = SaveAuthorityPropertyRateDetail(model);
                    //}
                    flag = SaveAuthorityPropertyRateDetail(model);
                }
            }
            return flag;
        }

        private int SaveAuthorityPropertyRateDetail(PropertyViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var propertyRate = new PropertyRateMst();
                propertyRate.DepartmentId = model.DepartmentId;
                propertyRate.SchemeId = model.SchemeId;
                propertyRate.SectorId = model.SectorId;
                propertyRate.SectorName = model.SectorName;
                propertyRate.BLockId = model.BlockId;
                propertyRate.PropertyTypeId = model.PropertyTypeId;
                propertyRate.SchemeName = model.SchemeName;
                propertyRate.PropertyRate = model.LandRate;
                propertyRate.AllotmentYear = model.AllotmentYear;
                propertyRate.AreaRange = model.AreaRange;
                propertyRate.NormalInterest = model.NormalInterest;
                propertyRate.PenalInterest = model.PenalInterest;
                propertyRate.PlotPremium = model.AllotmentMoney;
                propertyRate.Phase = model.AreaPhase;
                propertyRate.AllotmentMethod = model.AllotmentMethod;
                propertyRate.StartDate = model.PropertyRateStartDate;
                propertyRate.EndDate = model.PropertyRateEndDate;
                propertyRate.TransferRateInPercent = model.TransferRateInPercent;
                propertyRate.TransferRateInINR = model.TransferRateInINR;
                propertyRate.IsActive = true;
                propertyRate.CreatedBy = userInfo.UserID;
                propertyRate.CreatedDate = DateTime.Now;
                dbContext.PropertyRateMsts.Add(propertyRate);
                dbContext.SaveChanges();
                flag = ReturnType.Saved;
            }
            return flag;
        }

        public int RemovePropertyRateDetailById(PropertyViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var rate = dbContext.PropertyRateMsts.FirstOrDefault(id => id.Id == model.Id);
                if (rate != null)
                {
                    rate.IsActive = false;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                return flag;
            }
        }


        public DataSourceResult GetPropertyTypeList(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from property in dbContext.PropertyTypeMsts
                            where (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                            select new PropertyViewModel
                            {
                                Id = property.propertyTypeId,
                                DepartmentId = property.departmentId,
                                Department = property.DepartmentMst.departmentName,
                                PropertyTypeId = property.propertyTypeId,
                                PropertyType = property.propertyTypeName,
                                IsActive = property.IsActive,
                                Status = property.IsActive == true ? "Active" : "InActive",
                                SchemeName = null
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public int SavePropertyTypeByDepartment(PropertyViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "Add")
                {
                    var propertyType = new PropertyTypeMst();
                    propertyType.departmentId = model.DepartmentId;
                    propertyType.propertyTypeName = model.PropertyType.ToUpper();
                    propertyType.IsActive = true;
                    propertyType.createdBy = userInfo.UserID.ToString();
                    propertyType.createdDate = DateTime.Now;
                    dbContext.PropertyTypeMsts.Add(propertyType);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                else if (model.ActionType == "Update")
                {
                    var propertyType = dbContext.PropertyTypeMsts.FirstOrDefault(r => r.propertyTypeId == model.PropertyTypeId);
                    propertyType.propertyTypeName = model.PropertyType.ToUpper();
                    propertyType.modifiedBy = userInfo.UserID.ToString();
                    propertyType.modifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }

        public int ChangeStatusOfPropertyType(PropertyViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "ChangeStatus")
                {
                    var propertyType = dbContext.PropertyTypeMsts.FirstOrDefault(r => r.propertyTypeId == model.PropertyTypeId);
                    propertyType.IsActive = propertyType.IsActive == true ? false : true;
                    propertyType.modifiedBy = userInfo.UserID.ToString();
                    propertyType.modifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                else if (model.ActionType == "Remove")
                {
                    var propertyType = dbContext.PropertyTypeMsts.FirstOrDefault(r => r.propertyTypeId == model.PropertyTypeId);
                    dbContext.PropertyTypeMsts.Remove(propertyType);
                    dbContext.SaveChanges();
                    flag = ReturnType.Removed;
                }
            }
            return flag;
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
                                  KhasraNumber = property.KhasraNumber,
                                  KhatoniNumber = property.KhatoniNumber,
                                  VillageId = property.VillageId,
                                  VillageName = dbContext.Village_Master.FirstOrDefault(m => m.Id == property.VillageId).Village_Name,
                                  ProcessingFee = property.Processingfee,
                                  GroupProjectId = property.groupProjectId,
                                  EarnestMoney = property.EarnestMoney,
                                  AllotmentMoney = property.allotmentMoney,
                                  Latitude = property.Latitude,
                                  Longitude = property.Longitude
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
                    property.EarnestMoney = model.EarnestMoney;
                    property.KhasraNumber = model.KhasraNumber;
                    property.KhatoniNumber = model.KhatoniNumber;
                    property.VillageId = model.VillageId;
                    property.Processingfee = model.ProcessingFee;
                    property.groupProjectId = model.GroupProjectId;
                    property.Latitude = model.Latitude;
                    property.Longitude = model.Longitude;

                    dbContext.SaveChanges();
                    return ReturnType.Success;
                }
                else
                {
                    return ReturnType.NotExist;
                }
            }
        }


        public int UpdatePropertyCostDetail(PropertyDetailViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var property = dbContext.SchemeCostTrans.Where(c => c.refId == model.RefId).FirstOrDefault();
                if (property != null)
                {
                    property.schemeId = model.SchemeId;
                    property.departmentId = model.DepartmentId;
                    property.sectorId = model.SectorId;
                    property.blockId = model.BlockId;
                    property.floorId = model.FloorAreaId;
                    property.propertyTypeId = model.PropertyTypeId;
                    property.landRatePerSqmt = model.PropertyRate;
                    property.propertyCost = model.PropertyCost;
                    property.civilCost = model.CivilCost;
                    property.totalPropertyCost = model.TotalPropertyCost;
                    property.allotmentMoney = model.AllotmentMoney;
                    property.earnestMoney = model.EarnestMoney;
                    property.leaseRent = model.LeaseRent;

                    dbContext.SaveChanges();
                    return ReturnType.Success;
                }
                else
                {
                    return ReturnType.NotExist;
                }
            }
        }

        public SchemePropertyModel GetPropertyCostDetailsById(int? propertyId)
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
                                  KhasraNumber = property.KhasraNumber,
                                  KhatoniNumber = property.KhatoniNumber,
                                  VillageId = property.VillageId,
                                  VillageName = dbContext.Village_Master.FirstOrDefault(m => m.Id == property.VillageId).Village_Name,
                                  ProcessingFee = property.Processingfee,
                                  GroupProjectId = property.groupProjectId,
                                  EarnestMoney = property.EarnestMoney,
                                  AllotmentMoney = property.allotmentMoney,
                                  Latitude = property.Latitude,
                                  Longitude = property.Longitude
                              }).FirstOrDefault();
                return detail;
            }
        }


        public DataSourceResult GetMasterPropertyCostList(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from property in dbContext.SchemeCostTrans
                            from block in dbContext.BlockMsts.Where(b => b.blockId == property.blockId).DefaultIfEmpty()
                            from propertytype in dbContext.PropertyTypeMsts.Where(t => t.propertyTypeId == property.propertyTypeId).DefaultIfEmpty()
                            from floorarea in dbContext.FloorMsts.Where(a => a.floorId == property.floorId).DefaultIfEmpty()
                            where DepartmentList.Contains(property.departmentId.Value)
                            && (model.Id == null || property.refId == model.Id)
                            && (model.SchemeId == null || property.schemeId == model.SchemeId)
                            && (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                            && (model.SectorId == null || property.sectorId == model.SectorId)
                            && (model.BlockId == null || property.blockId == model.BlockId)
                            select new PropertyViewModel
                            {
                                Id = property.refId,
                                //PropertyId = property.propertyId,
                                SchemeId = property.schemeId,
                                SchemeName = property.SchemeMst.schemeName,
                                DepartmentId = property.departmentId,
                                Department = property.DepartmentMst.departmentName,
                                PropertyTypeId = propertytype.propertyTypeId,
                                PropertyType = propertytype.propertyTypeName,
                                SectorId = property.sectorId,
                                SectorName = property.SectorMst.sectorName,
                                BlockId = block.blockId,
                                BlockName = block.blockName,
                                FloorId = floorarea.floorId,
                                FloorArea = floorarea.floorName,
                                PropertyCost = property.propertyCost,
                                CivilCost = property.civilCost,
                                TotalPropertyCost = property.totalPropertyCost,
                                LandRate = property.landRatePerSqmt,
                                AllotmentMoney = property.allotmentMoney,
                                EarnestMoney = property.earnestMoney,
                                LeaseRent = property.leaseRent,
                                CreatedDate = property.createdDate,
                                //SchemeStatus = property.SchemeMst.Status,

                            });

                return list.ToDataSourceResult(request);
            }
        }


        public int SaveAllottedPropertyDetail(PropertyViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.DepartmentId == NADepartment.Housing)
                {
                    var allotmentMoney = dbContext.SchemeCostTrans.Where(x => x.schemeId == model.SchemeId && x.departmentId == model.DepartmentId && x.sectorId == model.SectorId && x.blockId == model.BlockId && x.floorId == model.FloorAreaId).Select(y => y.allotmentMoney).FirstOrDefault();
                    if (allotmentMoney != null)
                        model.AllotmentMoney = allotmentMoney;
                }
                else
                {
                    var allotmentPercent = dbContext.SchemeDepartmentTrans.Where(x => x.schemeId == model.SchemeId && x.departmentId == model.DepartmentId).Select(y => y.allotmentMoneyPercent).FirstOrDefault();
                    if (allotmentPercent != null && model.TotalPropertyCost != null)
                        model.AllotmentMoney = ((model.TotalPropertyCost * Convert.ToDecimal(allotmentPercent)) / 100);
                }
                var existingProperty = dbContext.SchemePropTrans.Where(i => i.refId == model.Id && i.IsActive == true).FirstOrDefault();
                if (existingProperty != null)
                {
                    existingProperty.schemeId = model.SchemeId;
                    existingProperty.departmentId = model.DepartmentId;
                    existingProperty.propertyTypeId = model.PropertyTypeId;
                    existingProperty.floorId = model.FloorAreaId;
                    existingProperty.sectorId = model.SectorId;
                    existingProperty.blockId = model.BlockId;
                    existingProperty.propertyNo = model.PlotNo;
                    existingProperty.AllottedArea = model.AllottedArea;
                    existingProperty.coveredArea = model.CoveredArea;
                    existingProperty.actualArea = model.ActualArea;
                    existingProperty.totalArea = model.TotalArea;
                    existingProperty.Registry = model.RegistryType;
                    existingProperty.landRatePerSqmt = model.AllotmentRate;
                    existingProperty.TotalAllotmentRate = model.TotalAllotmentRate;
                    existingProperty.propertyCost = model.PropertyCost;
                    existingProperty.civilCost = model.CivilCost;
                    existingProperty.totalPropertyCost = model.TotalPropertyCost;
                    existingProperty.allotmentMoney = model.AllotmentMoney;
                    existingProperty.EarnestMoney = model.EarnestMoney;
                    existingProperty.Processingfee = model.ProcessingFee;
                    existingProperty.ParentPropertyId = (model.ParentPropertyId == null || model.ParentPropertyId == 0) ? existingProperty.ParentPropertyId : model.ParentPropertyId;
                    existingProperty.LocationId = model.LocationId;
                    existingProperty.groupProjectId = model.GroupProjectId;
                    existingProperty.IsActive = true;
                    existingProperty.modifiedBy = userInfo.UserID.ToString();
                    existingProperty.modifiedDate = DateTime.Now;

                    dbContext.SaveChanges();

                    int iflag = SaveLocationChargeForAllottedProperty(model);
                    flag = ReturnType.Updated;
                }
                else
                {
                    var record = dbContext.SchemePropTrans.Where(x => x.schemeId == model.SchemeId && x.departmentId == model.DepartmentId && x.sectorId == model.SectorId && x.blockId == model.BlockId && x.floorId == model.FloorId && x.propertyNo.Trim() == model.PlotNo.Trim()).FirstOrDefault();
                    if (record == null)
                    {
                        var schemePropTran = new SchemePropTran();
                        schemePropTran.schemeId = model.SchemeId;
                        schemePropTran.departmentId = model.DepartmentId;
                        schemePropTran.propertyTypeId = model.PropertyTypeId;
                        schemePropTran.floorId = model.FloorAreaId;
                        schemePropTran.sectorId = model.SectorId;
                        schemePropTran.blockId = model.BlockId;
                        schemePropTran.propertyNo = model.PlotNo;
                        schemePropTran.AllottedArea = model.AllottedArea;
                        schemePropTran.totalArea = model.TotalArea;
                        schemePropTran.coveredArea = model.CoveredArea;
                        schemePropTran.actualArea = model.ActualArea;
                        schemePropTran.landRatePerSqmt = model.AllotmentRate;
                        schemePropTran.TotalAllotmentRate = model.TotalAllotmentRate;
                        schemePropTran.propertyCost = model.PropertyCost;
                        schemePropTran.civilCost = model.CivilCost;
                        schemePropTran.totalPropertyCost = model.TotalPropertyCost;
                        schemePropTran.allotmentMoney = model.AllotmentMoney;
                        schemePropTran.EarnestMoney = model.EarnestMoney;
                        schemePropTran.Processingfee = model.ProcessingFee;
                        schemePropTran.Registry = model.RegistryType;
                        schemePropTran.LocationId = model.LocationId;
                        schemePropTran.groupProjectId = model.GroupProjectId;
                        schemePropTran.IsActive = true;
                        schemePropTran.createdBy = userInfo.UserID.ToString();
                        schemePropTran.createdDate = DateTime.Now;

                        dbContext.SchemePropTrans.Add(schemePropTran);
                        dbContext.SaveChanges();

                        model.PropertyId = schemePropTran.propertyId;
                        int iflag = SaveLocationChargeForAllottedProperty(model);
                        flag = ReturnType.Saved;
                    }
                    else flag = ReturnType.Exist;
                }
                return flag;
            }
        }

        private int SaveLocationChargeForAllottedProperty(PropertyViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.IsLocationCharged == false)
                {
                    var locationcharge = dbContext.LocationChargeDetailMsts.FirstOrDefault(l => l.PropertyId == model.PropertyId);
                    if (locationcharge != null)
                    {
                        dbContext.LocationChargeDetailMsts.Remove(locationcharge);
                        dbContext.SaveChanges();
                        flag = ReturnType.Removed;
                    }
                }
                else
                {
                    var locationcharge = dbContext.LocationChargeDetailMsts.FirstOrDefault(l => l.PropertyId == model.PropertyId);
                    if (locationcharge != null)
                    {
                        locationcharge.LocationId = model.LocationId;
                        locationcharge.LocationCharge = model.LocationCharge;
                        locationcharge.LocationChargeRate = model.LocationChargeRate;
                        locationcharge.IsActive = true;
                        locationcharge.ModifiedBy = userInfo.UserID;
                        locationcharge.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Updated;
                    }
                    else
                    {
                        var lcharge = new LocationChargeDetailMst();
                        lcharge.LocationId = model.LocationId;
                        lcharge.PropertyId = model.PropertyId;
                        lcharge.LocationCharge = model.LocationCharge;
                        lcharge.LocationChargeRate = model.LocationChargeRate;
                        lcharge.IsActive = true;
                        lcharge.CreatedBy = userInfo.UserID;
                        lcharge.CreatedDate = DateTime.Now;
                        dbContext.LocationChargeDetailMsts.Add(lcharge);
                        dbContext.SaveChanges();
                        flag = ReturnType.Saved;
                    }
                }
            }
            return flag;
        }

        public DataSourceResult GetPropertyDetailAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from property in dbContext.SchemePropTrans
                            where (model.Id == null || property.refId == model.Id)
                            && (model.SchemeId == null || property.schemeId == model.SchemeId)
                            && (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                            && (model.SectorId == null || property.sectorId == model.SectorId)
                            && (model.BlockId == null || property.blockId == model.BlockId)
                            && (model.PropertyTypeId == null || property.propertyTypeId == model.PropertyTypeId)
                            && (model.FloorAreaId == null || property.floorId == model.FloorAreaId)
                            select new PropertyViewModel
                            {
                                Id = property.refId,
                                SchemeId = property.schemeId,
                                DepartmentId = property.departmentId,
                                Department = property.DepartmentMst.departmentName,
                                PropertyId = property.propertyId,
                                PropertyTypeId = property.propertyTypeId,
                                PropertyType = property.PropertyTypeMst.propertyTypeName,
                                FloorAreaId = property.floorId,
                                FloorArea = property.FloorMst.floorName,
                                SectorId = property.sectorId,
                                SectorName = property.SectorMst.sectorName,
                                BlockId = property.blockId,
                                BlockName = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                AllottedArea = property.AllottedArea,
                                TotalArea = property.totalArea,
                                CoveredArea = property.coveredArea,
                                ActualArea = property.actualArea,
                                AllotmentRate = property.landRatePerSqmt,
                                TotalAllotmentRate = property.TotalAllotmentRate,
                                PropertyCost = property.propertyCost,
                                CivilCost = property.civilCost,
                                TotalPropertyCost = property.totalPropertyCost,
                                AllotmentMoney = property.allotmentMoney,
                                EarnestMoney = property.EarnestMoney,
                                ProcessingFee = property.Processingfee,
                                RegistryType = property.Registry,
                                GroupProjectId = property.groupProjectId,
                                IsActive = property.IsActive,
                                IsExcessArea = property.IsExcessArea,
                                LocationId = property.LocationId,
                                RegistrationId = dbContext.AllotmentMasters.FirstOrDefault(p => p.propertyId == property.propertyId) != null ? dbContext.AllotmentMasters.FirstOrDefault(p => p.propertyId == property.propertyId).rid : 0,
                                AllotmentDate = dbContext.AllotmentMasters.FirstOrDefault(p => p.propertyId == property.propertyId) != null ? dbContext.AllotmentMasters.FirstOrDefault(p => p.propertyId == property.propertyId).allotmentDate : null,
                                IsPropertyAllotted = dbContext.AllotmentMasters.FirstOrDefault(p => p.propertyId == property.propertyId) != null ? true : false
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public PropertyViewModel GetPropertyCostDetailById(PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from property in dbContext.SchemeCostTrans
                              from deptrans in dbContext.SchemeDepartmentTrans.Where(d => d.schemeId == property.schemeId && d.departmentId == property.departmentId).DefaultIfEmpty()
                              where property.IsActive == true && (property.schemeId == model.SchemeId && property.departmentId == model.DepartmentId && property.propertyTypeId == model.PropertyTypeId && property.sectorId == model.SectorId && property.blockId == model.BlockId && property.floorId == model.FloorAreaId)
                              select new PropertyViewModel
                              {
                                  Id = property.refId,
                                  SchemeId = property.schemeId,
                                  SchemeName = property.SchemeMst.schemeName,
                                  DepartmentId = property.departmentId,
                                  Department = property.DepartmentMst.departmentName,

                                  SectorId = property.sectorId,
                                  SectorName = property.SectorMst.sectorName,
                                  BlockId = property.blockId,
                                  BlockName = property.BlockMst.blockName,
                                  FloorId = property.floorId,
                                  FloorArea = property.FloorMst.floorName,

                                  NormalInterestII = deptrans.normalInt,
                                  PenalInterestII = deptrans.penalInt,
                                  TotalInstallment = deptrans.noOfInstallments,
                                  Frequency = deptrans.frequency,
                                  FloorAreaRatio = deptrans.far,
                                  AllotmentAmountInPercentII = deptrans.allotmentMoneyPercent,
                                  LeaseRentInPercentII = deptrans.leaseRentPercent,
                                  InstallmentAmountInPercentII = deptrans.installmentMoneyPercent,

                                  //ProcessingFee = property.Processingfee,
                                  PropertyRate = property.landRatePerSqmt,
                                  LandRate = property.landRatePerSqmt,
                                  AllotmentRate = property.landRatePerSqmt,
                                  AllotmentMoney = property.allotmentMoney,
                                  EarnestMoney = property.earnestMoney,
                                  PropertyCost = property.propertyCost,
                                  TotalPropertyCost = property.totalPropertyCost,
                                  CivilCost = property.civilCost,
                                  LeaseRent = property.leaseRent // != null ? property.LeaseRent : ((property != null && property.leaseRent != null) ? property.leaseRent : null),
                                  //AdvanceLeaseRent = property.AdvanceLeaseRent,

                              }).FirstOrDefault();
                return detail;
            }
        }


        public int SavePropertyDocuments(DocumentViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int flag = ReturnType.None;
            var fileList = (List<HttpPostedFileBase>)HttpContext.Current.Session["TempUploadDocuments"];
            var documentList = (List<DocumentViewModel>)HttpContext.Current.Session["TempDocuments"];
            if (fileList != null && documentList != null)
            {
                //FtpHandler.UploadFileRange(fileList, model.RegistrationId.ToString(), "Upload");
                for (int i = 0; i < fileList.Count; i++)
                {
                    //FtpHandler.UploadFileByName(fileList[i], documentList[i].DocumentName, model.RegistrationId.ToString(), "Documents");
                }
                flag = ReturnType.Saved;
            }
            return flag;
        }

        public int SaveDocumentsInTempSession(DocumentViewModel model, HttpPostedFileBase tempFile)
        {
            int flag = ReturnType.None;

            if (tempFile != null)
            {
                model.DocumentName = model.RegistrationId.ToString() + "-" + model.DocumentType + Path.GetExtension(tempFile.FileName);
                //FtpHandler.UploadFileByName(tempFile, model.DocumentName, model.RegistrationId.ToString(), "Documents");
                FtpHandler.UploadPropertyFile(tempFile, model.DocumentName, model.RegistrationId.ToString(), "Documents");
                flag = ReturnType.Saved;
            }
            //List<DocumentViewModel> documentList = new List<DocumentViewModel>();
            //List<HttpPostedFileBase> fileList = new List<HttpPostedFileBase>();
            //if (HttpContext.Current.Session["TempDocuments"] != null)
            //{
            //    fileList = (List<HttpPostedFileBase>)HttpContext.Current.Session["TempUploadDocuments"];
            //    fileList.Add(tempFile);
            //    HttpContext.Current.Session["TempUploadDocuments"] = fileList;

            //    documentList = (List<DocumentViewModel>)HttpContext.Current.Session["TempDocuments"];
            //    //model.Document = tempFile;
            //    model.DocumentName = model.DocumentName.Split()[0] + "-" + model.DocumentType + "." + Path.GetExtension(tempFile.FileName);
            //    documentList.Add(model);
            //    HttpContext.Current.Session["TempDocuments"] = documentList;
            //    flag = ReturnType.Saved;
            //}
            //else
            //{
            //    fileList.Add(tempFile);
            //    HttpContext.Current.Session["TempUploadDocuments"] = fileList;

            //    DocumentViewModel document = new DocumentViewModel();
            //    //document.Document = tempFile;
            //    document.DocumentName = model.DocumentName.Split()[0] + "-" + model.DocumentType + "." + Path.GetExtension(tempFile.FileName);
            //    documentList.Add(document);
            //    HttpContext.Current.Session["TempDocuments"] = documentList;
            //    flag = ReturnType.Saved;
            //}

            return flag;
        }

        public DataSourceResult GetDocumentListFromTempSessionAsDataSource(DataSourceRequest request, DocumentViewModel model)
        {
            var path = FtpHandler.GetPropertyDocumentPath(model.RegistrationId, "Documents", string.Empty, true);
            //path = path + "\\Documents";
            List<string> fileList = FtpHandler.ToListFiles(path);
            List<DocumentViewModel> documentList = new List<DocumentViewModel>();
            int i = 1;
            foreach (string file in fileList)
            {
                string str = file;
                string doctypekey = string.Empty;
                if ((str.Split('-')).Length > 1)
                {
                    doctypekey = str.Substring(0, str.Length - 4);
                    doctypekey = doctypekey.Substring(9, doctypekey.Length - 9);
                }
                string keyvalue = System.Configuration.ConfigurationManager.AppSettings[doctypekey];
                string documentname = string.IsNullOrEmpty(doctypekey) ? "Other Document" : (string.IsNullOrEmpty(keyvalue) ? "Other Documents" : keyvalue);
                DocumentViewModel document = new DocumentViewModel();
                document.Id = i;
                document.DocumentPath = FtpHandler.GetPropertyDocumentPath((int)model.RegistrationId, "Documents", file, false);
                document.DocumentName = documentname; //!(string.IsNullOrEmpty(doctypekey)) ? (!(string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[doctypekey])) ? System.Configuration.ConfigurationManager.AppSettings[doctypekey] : "Other Documents") : "Other Documents";
                document.RegistrationId = (int)model.RegistrationId;
                documentList.Add(document);
                i = i + 1;
            }
            return documentList.ToDataSourceResult(request);

            //List<DocumentViewModel> documentList = new List<DocumentViewModel>();
            //if (HttpContext.Current.Session["TempDocuments"] != null)
            //{
            //    documentList = (List<DocumentViewModel>)HttpContext.Current.Session["TempDocuments"];
            //    int i = 1;
            //    foreach (var doc in documentList)
            //    {
            //        doc.Id = i;
            //        i++;
            //    }
            //    return documentList.ToDataSourceResult(request);
            //}
            //else
            //{
            //    return documentList.ToDataSourceResult(request);
            //}  
        }

        public DataSourceResult GetDocumentTypeListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var typelist = (from document in dbContext.Document_Mst
                                select new DropdownViewModel
                                {
                                    Id = document.Id,
                                    Text = document.Document_Name, //Regex.Replace(document.Document_Name,"\s","");
                                    Value = document.Document_Key
                                });
                return typelist.ToDataSourceResult(request);
            }
        }

        public int RemoveDocumentByIdFromTempSession(DocumentViewModel model)
        {
            int flag = ReturnType.None;
            var fileList = (List<HttpPostedFileBase>)HttpContext.Current.Session["TempUploadDocuments"];
            var documentList = (List<DocumentViewModel>)HttpContext.Current.Session["TempDocuments"];
            if (fileList != null && documentList != null)
            {
                if (model.Id != null)
                {
                    int id = model.Id.Value - 1;
                    fileList.RemoveAt(id);
                    documentList.RemoveAt(id);
                    flag = ReturnType.Removed;
                }
            }
            return flag;
        }


        public DataSourceResult GetExistingDocumentListAsDataSource(DataSourceRequest request, DocumentViewModel model)
        {
            List<DocumentViewModel> documentList = new List<DocumentViewModel>();
            if (model.RegistrationId != null)
            {
                var pathI = FtpHandler.GetPropertyDocumentPath(model.RegistrationId, string.Empty, string.Empty, true);
                var pathII = FtpHandler.GetPropertyDocumentPath(model.RegistrationId, "Documents", string.Empty, true);
                List<string> fileListI = FtpHandler.ToListFiles(pathI);
                List<string> fileListII = FtpHandler.ToListFiles(pathII);
                //fileListI.AddRange(fileListII);

                int i = 1;
                foreach (string file in fileListI)
                {
                    string str = file;
                    string doctypekey = string.Empty;
                    if ((str.Split('-')).Length > 1)
                    {
                        doctypekey = str.Substring(0, str.Length - 4);
                        doctypekey = doctypekey.Substring(9, doctypekey.Length - 9);
                    }
                    string keyvalue = System.Configuration.ConfigurationManager.AppSettings[doctypekey];
                    string documentname = string.IsNullOrEmpty(doctypekey) ? "Other Document" : (string.IsNullOrEmpty(keyvalue) ? "Other Documents" : keyvalue);
                    DocumentViewModel document = new DocumentViewModel();
                    document.Id = i;
                    document.DocumentPath = FtpHandler.GetPropertyDocumentPath((int)model.RegistrationId, string.Empty, file, false);
                    document.DocumentName = documentname; //!(string.IsNullOrEmpty(doctypekey)) ? (!(string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[doctypekey])) ? System.Configuration.ConfigurationManager.AppSettings[doctypekey] : "Other Documents") : "Other Documents";
                    document.RegistrationId = (int)model.RegistrationId;
                    documentList.Add(document);
                    i = i + 1;
                }

                //foreach (string file in fileListII)
                //{
                //    string str = file;
                //    string doctypekey = string.Empty;
                //    if ((str.Split('-')).Length > 1)
                //    {
                //        doctypekey = str.Substring(0, str.Length - 4);
                //        doctypekey = doctypekey.Substring(9, doctypekey.Length - 9);
                //    }
                //    string keyvalue = System.Configuration.ConfigurationManager.AppSettings[doctypekey];
                //    string documentname = string.IsNullOrEmpty(doctypekey) ? "Other Document" : (string.IsNullOrEmpty(keyvalue) ? "Other Documents" : keyvalue);
                //    DocumentViewModel document = new DocumentViewModel();
                //    document.Id = i;
                //    document.DocumentPath = FtpHandler.GetPropertyDocumentPath((int)model.RegistrationId, "Documents", file, false);
                //    document.DocumentName = documentname; //!(string.IsNullOrEmpty(doctypekey)) ? (!(string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[doctypekey])) ? System.Configuration.ConfigurationManager.AppSettings[doctypekey] : "Other Documents") : "Other Documents";
                //    document.RegistrationId = (int)model.RegistrationId;
                //    documentList.Add(document);
                //    i = i + 1;
                //}
            }
            return documentList.ToDataSourceResult(request);
        }
    }
}
