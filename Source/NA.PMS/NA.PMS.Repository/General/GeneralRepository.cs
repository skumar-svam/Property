using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Model.CommonModel;
using NA.PMS.Common;
using KellermanSoftware.CompareNetObjects;
using NA.PMS.Web.Models;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NoidaAuthority.PMS.Common;
using System.Resources;
using System.Collections;
using System.Globalization;

namespace NA.PMS.Repository
{
    public class GeneralRepository : IGeneralRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public GeneralRepository()
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
        public List<DDList> GetAllDepartments()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDepartment = (from userMaster in dbContext.UmUserMasters
                                           join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                           where userMaster.UserRefId == userInfo.UserID
                                           select departmentTrans.DepartmentId).ToList();

                var lst = (from deptt in dbContext.DepartmentMsts
                           where deptt.IsActive == true && loginUserDepartment.Contains(deptt.departmentId)
                           select new DDList
                           {
                               id = deptt.departmentId,
                               text = deptt.departmentName
                           }).ToList();
                return lst;
            }
        }
        // Get all departments of current user
        public List<SelectListItem> GetAllUserDepartments()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      join deptMst in dbContext.DepartmentMsts on dept.DepartmentId equals deptMst.departmentId
                                      where userMst.UserRefId == userInfo.UserID && (deptMst.departmentId == DepartmentOption.Commercial
                                      || deptMst.departmentId == DepartmentOption.Institutional
                                      || deptMst.departmentId == DepartmentOption.Industrial)
                                      select new SelectListItem
                                      {
                                          Value = deptMst.departmentId.ToString(),
                                          Text = deptMst.departmentName
                                      }).ToList();

                return loginUserDeptt;
            }
        }
        // Get Sectors by Department IDs
        public List<SelectListItem> GetSectorsByDeptId(int departmentId)
        {
            var lstSectors = new List<SelectListItem>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lstSectors = (from schemeCostTrans in dbContext.SchemeCostTrans
                              join sectorMsts in dbContext.SectorMsts on schemeCostTrans.sectorId equals sectorMsts.sectorId
                              where schemeCostTrans.IsActive == true && schemeCostTrans.departmentId == departmentId
                              select new SelectListItem
                              {
                                  Value = sectorMsts.sectorId.ToString(),
                                  Text = sectorMsts.sectorName
                              }).Distinct().ToList();
                return lstSectors;
            }
        }
        // Get Blocks by Department and Sector Ids
        public List<SelectListItem> GetBlocksByDeptAndSectorId(int departmentId, int sectorId)
        {
            var lstBlocks = new List<SelectListItem>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lstBlocks = (from schemeCostTrans in dbContext.SchemeCostTrans
                             join blockMsts in dbContext.BlockMsts on schemeCostTrans.blockId equals blockMsts.blockId
                             where schemeCostTrans.IsActive == true && schemeCostTrans.departmentId == departmentId && schemeCostTrans.sectorId == sectorId
                             select new SelectListItem
                             {
                                 Value = blockMsts.blockId.ToString(),
                                 Text = blockMsts.blockName
                             }).Distinct().ToList();
                return lstBlocks;
            }
        }

        // Get Sectors by Department IDs except from Scheme 
        public List<SelectListItem> GetAllSectorsByDeptId(int departmentId)
        {
            var lstSectors = new List<SelectListItem>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lstSectors = (from schemePropTrans in dbContext.SchemePropTrans
                              join allotMst in dbContext.AllotmentMasters on schemePropTrans.propertyId equals allotMst.propertyId
                              join sectorMsts in dbContext.SectorMsts on schemePropTrans.sectorId equals sectorMsts.sectorId
                              where schemePropTrans.IsActive == true && schemePropTrans.departmentId == departmentId
                              select new SelectListItem
                              {
                                  Value = sectorMsts.sectorId.ToString(),
                                  Text = sectorMsts.sectorName
                              }).Distinct().ToList();
                return lstSectors;
            }
        }

        // Get Sectors by Department IDs except from Scheme 
        public DataSourceResult GetAllSectorsByDeptId(DataSourceRequest Req)
        {
            // var lstSectors = new List<SelectListItem>();
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstSectors = (from schemePropTrans in dbContext.SchemePropTrans
                                  join allotMst in dbContext.AllotmentMasters on schemePropTrans.propertyId equals allotMst.propertyId
                                  join sectorMsts in dbContext.SectorMsts on schemePropTrans.sectorId equals sectorMsts.sectorId
                                  where schemePropTrans.IsActive == true //&& schemePropTrans.departmentId == departmentId
                                  select new SectorDDList
                                  {
                                      id = sectorMsts.sectorId,
                                      text = sectorMsts.sectorName,
                                      DepartmentId = schemePropTrans.departmentId != null ? (int)schemePropTrans.departmentId : 0
                                  }).Distinct();
                return lstSectors.ToDataSourceResult(Req);
            }
        }

        // Get Blocks by Department and Sector Ids except from Scheme 
        public List<SelectListItem> GetAllBlocksByDeptAndSectorId(int departmentId, int sectorId)
        {
            var lstBlocks = new List<SelectListItem>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lstBlocks = (from schemePropTrans in dbContext.SchemePropTrans
                             join allotMst in dbContext.AllotmentMasters on schemePropTrans.propertyId equals allotMst.propertyId
                             join blockMsts in dbContext.BlockMsts on schemePropTrans.blockId equals blockMsts.blockId
                             where schemePropTrans.IsActive == true && schemePropTrans.departmentId == departmentId && schemePropTrans.sectorId == sectorId
                             select new SelectListItem
                             {
                                 Value = blockMsts.blockId.ToString(),
                                 Text = blockMsts.blockName
                             }).Distinct().ToList();
                return lstBlocks;
            }
        }

        // Get Blocks by Department and Sector Ids except from Scheme 
        public DataSourceResult GetAllBlocksByDeptAndSectorId(DataSourceRequest Req, int departmentId, int sectorId)
        {
            //var lstBlocks = new List<SelectListItem>();
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstBlocks = (from schemePropTrans in dbContext.SchemePropTrans
                                 join allotMst in dbContext.AllotmentMasters on schemePropTrans.propertyId equals allotMst.propertyId
                                 join blockMsts in dbContext.BlockMsts on schemePropTrans.blockId equals blockMsts.blockId
                                 where schemePropTrans.IsActive == true && schemePropTrans.departmentId == departmentId && schemePropTrans.sectorId == sectorId
                                 select new MergeSplitBlockSelectionDDList
                                 {
                                     id = blockMsts.blockId,
                                     text = blockMsts.blockName,
                                     SectorId = schemePropTrans.sectorId != null ? (int)schemePropTrans.sectorId : 0
                                 }).Distinct();
                return lstBlocks.ToDataSourceResult(Req);
            }
        }

        public List<DDList> GetAllDepartmentsByScheme(int schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from depttran in dbContext.SchemeDepartmentTrans
                           join deptt in dbContext.DepartmentMsts on depttran.departmentId equals deptt.departmentId
                           where deptt.IsActive == true && depttran.schemeId == schemeId
                           select new DDList
                           {
                               id = deptt.departmentId,
                               text = deptt.departmentName
                           }).ToList();
                return lst;
            }
        }

        public List<DDList> GetPropTypes(int depttID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.PropertyTypeMsts
                           where prop.IsActive == true && prop.departmentId == depttID
                           select new DDList
                           {
                               id = prop.propertyTypeId,
                               text = prop.propertyTypeName
                           }).ToList();
                return lst;
            }
        }

        public List<DDLStringList> GetAllInterestRate()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.InterestMsts
                           where prop.isActive == true
                           select new DDLStringList
                           {
                               id = prop.normalInterest.ToString(),
                               text = prop.normalInterest.ToString()
                           }).Distinct().ToList();
                return lst;
            }
        }
        public List<DDLStringList> GetAllPenalRate()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.InterestMsts
                           where prop.isActive == true
                           select new DDLStringList
                           {
                               id = prop.penalInterest.ToString(),
                               text = prop.penalInterest.ToString()
                           }).Distinct().ToList();
                return lst;
            }
        }
        public List<DDList> GetAllFrquency()
        {
            List<DDList> frquency = new List<DDList>();
            foreach (int value in Enum.GetValues(typeof(Frequency)))
            {
                frquency.Add(new DDList
                {
                    text = Enum.GetName(typeof(Frequency), value),
                    id = value
                });
            }
            return frquency;
        }

        public List<DDList> GetAllQuota()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.QuotaMsts
                           where prop.IsActive == true
                           select new DDList
                           {
                               id = prop.quotaId,
                               text = prop.quotaName,
                           }).ToList();
                return lst;
            }
        }

        public List<DDList> GetAllUnits()
        {
            List<DDList> unit = new List<DDList>();
            //var unit1 = new DDList();
            //unit1.text = QuotaUnits.fix;
            //unit1.id = QuotaUnits.fixVal;
            //unit.Add(unit1);
            //var unit2 = new DDList();
            //unit2.text = QuotaUnits.per;
            //unit2.id = QuotaUnits.perVal;
            //unit.Add(unit2);
            foreach (int value in Enum.GetValues(typeof(QuotUnit)))
            {
                unit.Add(new DDList
                {
                    text = Enum.GetName(typeof(QuotUnit), value),
                    id = value
                });
            }
            return unit;
        }

        public List<DDList> GetRebateUnits()
        {
            List<DDList> unit = new List<DDList>();
            foreach (int value in Enum.GetValues(typeof(RebateUnit)))
            {
                unit.Add(new DDList
                {
                    text = Enum.GetName(typeof(RebateUnit), value),
                    id = value
                });
            }
            return unit;
        }

        public List<DDList> GetRebateTypes()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.RebateMsts
                           where prop.IsActive == true
                           select new DDList
                           {
                               id = prop.rebateId,
                               text = prop.rebateName,
                           }).ToList();
                return lst;
            }
        }
        public List<DDList> GetSchemeWiseBanks(int schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.BankMsts
                           join sbt in dbContext.SchemeBankTrans on prop.bankId equals sbt.bankId
                           where prop.IsActive == true && sbt.schemeId == schemeId
                           select new DDList
                           {
                               id = prop.bankId,
                               text = prop.bankName
                           }).ToList();
                var otherBank = new DDList();
                //otherBank.id = -1;
                //otherBank.text = Constants.Other;
                //lst.Add(otherBank);
                return lst;
            }
        }
        public List<DDList> GetAllBanks()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.BankMsts
                           where prop.IsActive == true
                           select new DDList
                           {
                               id = prop.bankId,
                               text = prop.bankName
                           }).ToList();
                var otherBank = new DDList();
                otherBank.id = -1;
                otherBank.text = Constants.Other;
                lst.Add(otherBank);
                return lst;
            }
        }
        public List<DDList> GetSchemeWiseBranchs(int bankId, int schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.BranchMsts
                           join sbt in dbContext.SchemeBankTrans on prop.branchId equals sbt.branchId
                           where prop.IsActive == true && prop.bankId == bankId && sbt.schemeId == schemeId
                           select new DDList
                           {
                               id = prop.branchId,
                               text = prop.branchName
                           }).Distinct().ToList();
                //var distinct = from l in lst group l by l.id into groups select groups.OrderBy(p => p.id).FirstOrDefault();
                //if (bankId != -1)
                //{
                //    var otherBranch = new DDList();
                //    otherBranch.id = -1;
                //    otherBranch.text = Constants.Other;
                //    lst.Add(otherBranch);
                //}
                return lst;
            }
        }
        public List<DDList> GetAllBranchs(int bankId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.BranchMsts
                           where prop.IsActive == true && prop.bankId == bankId
                           select new DDList
                           {
                               id = prop.branchId,
                               text = prop.branchName
                           }).ToList();
                if (bankId != -1)
                {
                    var otherBranch = new DDList();
                    otherBranch.id = -1;
                    otherBranch.text = Constants.Other;
                    lst.Add(otherBranch);
                }
                return lst;
            }
        }

        public List<DDList> GetSectorsByPropType()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from sec in dbContext.SectorMsts
                           where sec.IsActive == true
                           orderby sec.sectorName
                           select new DDList
                           {
                               id = sec.sectorId,
                               text = sec.sectorName
                           }).ToList();
                return lst;
            }
        }

        public List<DDList> GetFloors(int depttID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from f in dbContext.FloorMsts
                           where f.IsActive == true && f.departmentId == depttID
                           select new DDList
                           {
                               id = f.floorId,
                               text = f.floorName
                           }).ToList();
                return lst;
            }
        }

        public List<DDList> GetAllSchemeType()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from f in dbContext.SchemeTypeMsts
                           where f.IsActive == true
                           select new DDList
                           {
                               id = f.schemeTypeId,
                               text = f.SchemeTypeDesc
                           }).ToList();
                return lst;
            }
        }


        public List<DDList> GetAllSectors()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from f in dbContext.SectorMsts
                           where f.IsActive == true
                           select new DDList
                           {
                               id = f.sectorId,
                               text = f.sectorName
                           }).ToList();
                return lst;
            }
        }

        public List<DDList> GetAllBlocks()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from f in dbContext.BlockMsts
                           where f.IsActive == true
                           select new DDList
                           {
                               id = f.blockId,
                               text = f.blockName
                           }).ToList();
                return lst;
            }
        }

        public List<DDList> GetAllFloors()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from f in dbContext.FloorMsts
                           where f.IsActive == true
                           select new DDList
                           {
                               id = f.floorId,
                               text = f.floorName
                           }).ToList();
                return lst;
            }
        }

        public List<DDList> GetGenderById(string genderName)
        {
            List<DDList> gender = new List<DDList>();
            //foreach (int value in Enum.GetValues(typeof(Gender)))
            //{
            //    gender.Add(new DDList
            //    {
            //        text = Enum.GetName(typeof(Gender), value),
            //        id = value
            //    });
            //}
            if (genderName.Trim().ToLower() != Constants.Company.Trim().ToLower())
            {
                gender.Add(new DDList { text = Constants.Male });
                gender.Add(new DDList { text = Constants.Female });
            }
            else
            {
                gender.Add(new DDList { text = Constants.Company });
            }
            return gender;
        }

        public List<DDList> GetGender()
        {
            List<DDList> gender = new List<DDList>();
            foreach (int value in Enum.GetValues(typeof(Gender)))
            {
                gender.Add(new DDList
                {
                    text = Enum.GetName(typeof(Gender), value),
                    id = value
                });
            }
            return gender;
        }

        public List<DDList> GetIndividualGenders()
        {
            List<DDList> gender = new List<DDList>();
            var obj1 = new DDList();
            var obj2 = new DDList();
            obj1.id = Convert.ToInt32(Gender.Male);
            obj1.text = Constants.Male;
            obj2.id = Convert.ToInt32(Gender.Female);
            obj2.text = Constants.Female;
            gender.Add(obj1);
            gender.Add(obj2);
            return gender;
        }

        public List<DDList> GetMaritialStatus()
        {
            List<DDList> maritialStatus = new List<DDList>();
            foreach (int value in Enum.GetValues(typeof(MaritialStatus)))
            {
                maritialStatus.Add(new DDList
                {
                    text = Enum.GetName(typeof(MaritialStatus), value),
                    id = value
                });
            }
            return maritialStatus;
        }

        public List<DDList> GetOccupation()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from f in dbContext.OccupationMsts
                           select new DDList
                           {
                               id = f.occupationId,
                               text = f.occupation
                           }).ToList();
                return lst;
            }
        }

        public List<DDList> GetReligion()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from f in dbContext.ReligionMsts
                           select new DDList
                           {
                               id = f.religionId,
                               text = f.religion
                           }).ToList();
                return lst;
            }
        }

        public List<DDList> GetRegistry()
        {
            List<DDList> registry = new List<DDList>();
            foreach (int value in Enum.GetValues(typeof(Registry)))
            {
                registry.Add(new DDList
                {
                    text = Enum.GetName(typeof(Registry), value),
                    id = value
                });
            }
            return registry;
        }

        public List<DDLStringList> GetAllSelectionType()
        {
            List<DDLStringList> selectiontype = new List<DDLStringList>();
            foreach (int value in Enum.GetValues(typeof(SelectionType)))
            {
                selectiontype.Add(new DDLStringList
                {
                    text = Enum.GetName(typeof(SelectionType), value),
                    id = Enum.GetName(typeof(SelectionType), value)
                });
            }
            return selectiontype;
        }
        /// <summary>
        /// CreateAuditTrail method is used to enter object diffrences in Audit table.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableName"></param>
        /// <param name="viewName"></param>
        /// <param name="pkey"></param>
        /// <param name="oldObject"></param>
        /// <param name="newObject"></param>
        /// <param name="userID"></param>
        public static void CreateAuditTrail(string type, string tableName, string viewName, int pkey, Object oldObject, Object newObject, string userID)
        {
            ComparisonResult compResult = AuditingEngine.CreateAuditMaster(type, tableName, viewName, pkey, oldObject, newObject, userID);
            List<Audit> DeltaList = new List<Audit>();
            using (var dbContext = new NoidaPMSEntities())
            {
                try
                {
                    foreach (var change in compResult.Differences)
                    {
                        Audit delta = new Audit();
                        if (change.PropertyName.Substring(0, 1) == ".")
                            delta.FieldName = change.PropertyName.Substring(1, change.PropertyName.Length - 1);
                        if (delta.FieldName != Constants.ViewName)
                        {
                            delta.Type = type;
                            delta.TableName = tableName;
                            delta.PrimaryKeyValue = pkey.ToString();
                            delta.FormName = viewName;
                            delta.OldValue = change.Object1Value;
                            delta.NewValue = change.Object2Value;
                            delta.UpdateDate = DateTime.Now;
                            delta.UserName = userID;
                            dbContext.Audits.Add(delta);
                        }
                    }
                    dbContext.SaveChanges();
                }
                catch { }
            }

        }

        /// <summary>
        /// Fetches Chanllan Options
        /// </summary>
        /// <returns></returns>
        public List<DDList> GetChallanOptions()
        {
            var lst = new List<DDList>();
            var opt1 = new DDList();
            var opt2 = new DDList();
            opt1.id = ChallanOptions.oneTimeId;
            opt1.text = ChallanOptions.oneTime;
            opt2.id = ChallanOptions.annualId;
            opt2.text = ChallanOptions.annual;
            lst.Add(opt1);
            lst.Add(opt2);
            return lst;
        }

        /// <summary>
        /// Bind Process Name
        /// </summary>
        /// <returns></returns>
        public List<DDList> GetProcessData(int departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from process in dbContext.Process_Master
                           where process.Is_Active == true && process.Department_Id == departmentId
                           select new DDList
                           {
                               id = process.Process_Id,
                               text = process.Process_Name,
                           }).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Bind Trigger Process
        /// </summary>
        /// <returns></returns>
        public List<DDList> GetTriggerProcessData(int departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from process in dbContext.Process_Master
                           where process.Is_Active == true && process.Department_Id == departmentId
                           select new DDList
                           {
                               id = process.Process_Id,
                               text = process.Process_Name,
                           }).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Returns Approvers list filtered by data's Department ID
        /// </summary>
        /// <param name="depttId"></param>
        /// <returns></returns>
        public List<DDList> GetAssineToByDepttId(int depttId)
        {
            int userid = userInfo.UserID;
            using (var dbContext = new NoidaPMSEntities())
            {
                //var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                //                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                //                      where userMst.UserRefId == userid
                //                      select dept.DepartmentId).ToList();
                //var userList = (from userView in dbContext.ViewUmUserMasters
                //                join dept in dbContext.UmUserDepartmentTrans on userView.UserRefId equals dept.UserRefId
                //                where userView.IsActive == true && dept.UserRefId != userid && userView.RoleType != Constants.SuperAdmin && dept.DepartmentId == depttId
                //                select new DDList
                //                {
                //                    id = userView.UserRefId,
                //                    text = userView.UserName
                //                }).Distinct().ToList();
                //Bug#176
                var userList = (from user in dbContext.UmUserMasters
                                join dept in dbContext.UmUserDepartmentTrans on user.UserRefId equals dept.UserRefId
                                where user.IsActive == true && dept.DepartmentId == depttId && user.UserRefId != userInfo.UserID
                                select new DDList
                                {
                                    id = user.UserRefId,
                                    //text = user.FirstName + " " + user.MiddleName + " " + user.LastName
                                    text = user.UserName + " " + user.FirstName
                                }).Distinct().ToList();
                return userList;
            }
        }

        /// <summary>
        /// Get Completion Type for Completion Request
        /// </summary>
        /// <returns></returns>
        public List<DDList> GetCompletionType()
        {
            List<DDList> completionType = new List<DDList>();
            foreach (int value in Enum.GetValues(typeof(CompletionType)))
            {
                completionType.Add(new DDList
                {
                    text = Enum.GetName(typeof(CompletionType), value),
                    id = value
                });
            }
            return completionType;
        }

        public DateTime GetCompletionDueDateByRId(int rId, int schemeId, int deptId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var completionDueDate = DateTime.Now;
                var extension = dbContext.Extension_Details.Where(x => x.Rid == rId && x.Is_Active == true && x.Status == 1).FirstOrDefault(); // ToDo Check for Status == Approved (== 4) 
                if (extension == null)
                {
                    //completionDueDate = Convert.ToDateTime(dbContext.Extension_Details.Where(x => x.Rid == rId && x.Is_Active == true).Select(y => y.Extension_Due_Date).Max());
                    var autoScheduleDays = (from intdays in dbContext.Auto_Schedule where intdays.Is_Active == true && intdays.Scheme_Id == schemeId && intdays.Department_Id == deptId && intdays.Process_Id == Constants.CompletionId && intdays.Trigger_Process_Id == Constants.PossessionId select intdays.Duration_In_Days).FirstOrDefault();
                    //var sanctionDate = (from buildingPlan in dbContext.Building_Plan_Master where buildingPlan.Is_Active == true && buildingPlan.Rid == rId select buildingPlan.Sanction_Date).FirstOrDefault();
                    var possessionDate = (from possessionDetail in dbContext.PossessionDetails where possessionDetail.IsActive == true && possessionDetail.Rid == rId select possessionDetail.PossessionDate).FirstOrDefault();
                    if (autoScheduleDays == null)
                    {
                        // Completion Due Date = Possion Date + 2 years
                        completionDueDate = Convert.ToDateTime(possessionDate).AddDays(Convert.ToDouble(730));
                    }
                    else
                    {
                        // Completion Due Date = Possion Date + No Of Days in Auto Schedule 
                        completionDueDate = Convert.ToDateTime(possessionDate).AddDays(Convert.ToDouble(autoScheduleDays));
                    }
                    //completionDueDate = Convert.ToDateTime(dbContext.RegistryDetails.Where(x => x.Rid == rId && x.IsActive == true).Select(y => y.RegistryDueDate).FirstOrDefault());
                }
                else
                {
                    completionDueDate = Convert.ToDateTime(dbContext.Extension_Details.Where(x => x.Rid == rId && x.Is_Active == true).Select(y => y.Extension_Due_Date).Max());
                }
                return completionDueDate;
            }
        }

        /// <summary>
        /// Used for filling RID dropdown 
        /// </summary>
        /// <returns>List of RIDs</returns>
        public List<DDList> GetRIDs(string type)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var distinctRIds = new List<DDList>();
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();
                if (type.ToLower() == RIdType.completion.ToString().ToLower())
                {
                    var lstRId = (//from bpm in dbContext.Building_Plan_Master
                                  from possessioDetail in dbContext.PossessionDetails
                                  join am in dbContext.AllotmentMasters on possessioDetail.Rid equals am.rid
                                  join department in dbContext.DepartmentMsts on am.departmentId equals department.departmentId
                                  where possessioDetail.IsActive == true && am.isActive == 1 && am.isStatus == AllotmentStatus.Approved.ToString() && loginUserDeptt.Contains(am.departmentId) && department.departmentName.ToLower().Trim() != Departmentenum.Housing.ToString().ToLower().Trim()
                                  orderby possessioDetail.Id descending
                                  select new DDList
                                  {
                                      id = possessioDetail.Rid.Value,
                                      text = possessioDetail.Rid.ToString()
                                  }).ToList();
                    distinctRIds = lstRId.GroupBy(x => x.id).Select(y => y.First()).ToList();
                }
                else if (type.ToLower() == RIdType.buildingPlan.ToString().ToLower())
                {
                    var lstRId = (from possessionDetail in dbContext.PossessionDetails
                                  join am in dbContext.AllotmentMasters on possessionDetail.Rid equals am.rid
                                  //join st in dbContext.StatusMasters on possessionDetail.StatusId equals st.Id
                                  where possessionDetail.IsActive == true && possessionDetail.Possession == true && loginUserDeptt.Contains(am.departmentId) && am.departmentId != (int)Departmentenum.Housing
                                  select new DDList
                                  {
                                      id = am.rid,
                                      text = am.rid.ToString()
                                  }).ToList();
                    distinctRIds = lstRId.GroupBy(x => x.id).Select(y => y.First()).ToList();
                }
                return distinctRIds;
            }
        }

        public List<DDList> YesNoDDL()
        {
            var lst = new List<DDList>();
            var opt1 = new DDList();
            opt1.text = Constants.yes;
            opt1.id = MortPrevLoan.yes;
            var opt2 = new DDList();
            opt2.text = Constants.no;
            opt2.id = MortPrevLoan.no;
            lst.Add(opt1);
            lst.Add(opt2);
            return lst;
        }

        public List<DDList> RegUnRegDDL()
        {
            var lst = new List<DDList>();
            var opt1 = new DDList();
            opt1.text = RegUnReg.Registered;
            opt1.id = RegUnReg.RegisteredId;
            var opt2 = new DDList();
            opt2.text = RegUnReg.UnRegistered;
            opt2.id = RegUnReg.UnRegisteredId;
            lst.Add(opt1);
            lst.Add(opt2);
            return lst;
        }

        //Common method to bind ddl
        public List<DDList> BindDDL(string type)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from ddlBind in dbContext.Common_Config
                           where ddlBind.Is_Active == 1 && ddlBind.Category.ToLower().Equals(type.ToLower())
                           select new DDList
                           {
                               id = ddlBind.Id,
                               text = ddlBind.Name
                           }).ToList();
                return lst;
            }
        }

        public List<DDList> GetDepartmentsByUser()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var userDep = (from deptt in dbContext.DepartmentMsts
                               join depTrans in dbContext.UmUserDepartmentTrans on deptt.departmentId equals depTrans.DepartmentId
                               where depTrans.UserRefId == userInfo.UserID
                               select new DDList
                               {
                                   id = deptt.departmentId,
                                   text = deptt.departmentName
                               }).ToList();
                return userDep;
            }
        }

        // Method for getting content for letter to print
        public string GenerateLetterFromDb(int registrationId, int templateId, int departmentId)
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


        public string DownloadApplicationFormat(int appId, int templateId, int departmentId)
        {
            string strContent = string.Empty;
            if (userInfo != null)
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    ObjectParameter commaString = new ObjectParameter("CommaString", typeof(string));
                    dbContext.Sp_LatterPrintTemp(appId.ToString(), templateId, departmentId, userInfo.UserID.ToString(), null, commaString);
                    strContent = commaString.Value.ToString();
                }
            }

            return strContent;
        }

        /// <summary>
        /// Used for getting content for letter print, based on RID and template to print. Fetches Department ID from the DB itself
        /// </summary>
        /// <param name="rId">RID</param>
        /// <param name="templateId">Template ID to print</param>
        /// <returns></returns>
        public string GenerateLetterFromDbByRId(int rId, int templateId)
        {
            string strContent = string.Empty;
            if (userInfo != null)
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    var depttId = (from am in dbContext.AllotmentMasters where am.rid == rId select am.departmentId).FirstOrDefault();
                    if (depttId != 0)
                    {
                        ObjectParameter commaString = new ObjectParameter("CommaString", typeof(string));
                        dbContext.Sp_LatterPrintTemp(rId.ToString(), templateId, depttId, userInfo.UserID.ToString(), null, commaString);
                        strContent = commaString.Value.ToString();
                    }
                }
            }

            return strContent;
        }

        /// <summary>
        /// Common method used to save uploaded file in the location specified in Web.Config 
        /// </summary>
        /// <param name="hpf">File</param>
        /// <param name="rId">RID</param>
        /// <returns></returns>
        public bool SaveFile(HttpPostedFileBase hpf, int rId)
        {
            var flag = false;
            if (hpf != null && hpf.ContentLength > 0)
            {
                var fileName = new FileInfo(hpf.FileName).Name;
                if (!Directory.Exists(ConfigurationManager.AppSettings["UploadFilePath"] + rId))
                {
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["UploadFilePath"] + rId);
                }
                var fileSavePath = ConfigurationManager.AppSettings["UploadFilePath"] + rId + "/" + fileName;
                hpf.SaveAs(fileSavePath);
                flag = true;
            }
            return flag;
        }

        public List<DDList> GetAllPropTypes()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from propType in dbContext.PropertyTypeMsts
                           where propType.IsActive == true
                           select new DDList
                           {
                               id = propType.propertyTypeId,
                               text = propType.propertyTypeName
                           }).ToList();
                return lst;
            }
        }
        public List<DDList> GetFloorByDeptId(int deptId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from floor in dbContext.FloorMsts
                           where floor.IsActive == true && floor.departmentId == deptId
                           select new DDList
                           {
                               id = floor.floorId,
                               text = floor.floorName
                           }).ToList();
                return lst;
            }
        }

        public List<DDList> GetFunctionalStatus()
        {
            List<DDList> functionalStatus = new List<DDList>();
            foreach (var status in Enum.GetNames(typeof(FunctionalStatus)))
            {
                functionalStatus.Add(new DDList { text = status });
            }
            return functionalStatus;
        }



        public List<DDList> GetCompletedSchemeToSearch()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDepartment = (from userMaster in dbContext.UmUserMasters
                                           join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                           where userMaster.UserRefId == userInfo.UserID
                                           select departmentTrans.DepartmentId).ToList();
                var schemeMaster = (from scheme in dbContext.SchemeMsts
                                    join schemeProp in dbContext.SchemePropTrans on scheme.schemeId equals schemeProp.schemeId
                                    where scheme.completed.Value == true && loginUserDepartment.Contains(schemeProp.departmentId)
                                    select new DDList
                                    {
                                        id = scheme.schemeId,
                                        text = scheme.schemeName
                                    }).Distinct().ToList();
                return schemeMaster;
            }
        }


        public List<DDList> GetDepartmentListForLoginUser()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDepartment = (from userMaster in dbContext.UmUserMasters
                                           join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                           where userMaster.UserRefId == userInfo.UserID
                                           select departmentTrans.DepartmentId).ToList();
                var departmentList = (from department in dbContext.DepartmentMsts
                                      where loginUserDepartment.Contains(department.departmentId)
                                      select new DDList
                                      {
                                          id = department.departmentId,
                                          text = department.departmentName
                                      }).ToList();
                return departmentList;
            }
        }


        public List<DDList> AreaChangeOnPossession()
        {
            List<DDList> areaChange = new List<DDList>();
            foreach (var status in Enum.GetNames(typeof(AreaChange)))
            {
                areaChange.Add(new DDList { text = status });
            }
            return areaChange;
        }

        public List<DDList> PropertyPossessionStatus()
        {
            List<DDList> possessionStatus = new List<DDList>();
            foreach (var status in Enum.GetNames(typeof(PossessionStatus)))
            {
                possessionStatus.Add(new DDList { text = status });
            }
            return possessionStatus;
        }


        public List<DDList> GetPropertyCompletionStatus()
        {
            List<DDList> completionStatus = new List<DDList>();
            foreach (var status in Enum.GetNames(typeof(BuildingCompletion)))
            {
                completionStatus.Add(new DDList { text = status });
            }
            return completionStatus;
        }


        public List<DDList> GetServiceRequestsByDepartment(int department)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDepartment = (from userMaster in dbContext.UmUserMasters
                                           join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                           where userMaster.UserRefId == userInfo.UserID
                                           select departmentTrans.DepartmentId).ToList();
                var serviceList = (from services in dbContext.CitizenService_Master
                                   where loginUserDepartment.Contains(services.Deptt_Id)
                                   select new DDList
                                   {
                                       id = services.Deptt_Id.Value,
                                       text = services.ServiceName
                                   }).ToList();
                return serviceList;
            }
        }


        public BankAccountManagementModel GetPropertyDetailByRid(int rid, int referenceNo)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var details = (from allotment in dbContext.AllotmentMasters
                               join application in dbContext.ApplicationDetails on allotment.rid equals application.registrationId
                               join propertyTrans in dbContext.SchemePropTrans on allotment.propertyId equals propertyTrans.propertyId
                               where allotment.rid == rid
                               select new BankAccountManagementModel
                               {
                                   RId = rid,
                                   ReferenceNo = referenceNo,
                                   SchemeName = propertyTrans.SchemeMst.schemeName,
                                   DepttName = allotment.DepartmentMst.departmentName,
                                   ApplicationName = application.tFirstName + " " + application.tMiddleName + " " + application.tLastName,
                                   Gender = application.tGender,
                                   RelationName = application.tFatherHusbandName,
                                   PropertyType = propertyTrans.PropertyTypeMst.propertyTypeName,
                                   Area = propertyTrans.totalArea,
                                   Floor = propertyTrans.FloorMst.floorName,
                                   PropertyNumber = propertyTrans.SectorMst.sectorName + "/" + propertyTrans.BlockMst.blockName + "-" + propertyTrans.propertyNo,
                                   DepttId = allotment.departmentId.Value
                                   //RequestNo = trans.Request_No
                               }).FirstOrDefault();
                return details;
            }
        }


        public List<DDList> GetRegistrationIdByDepartment(int? departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var ridList = (from alot in dbContext.AllotmentMasters
                               where alot.departmentId == departmentId
                               select new DDList
                               {
                                   id = alot.rid,
                                   text = alot.rid.ToString()
                               }).Distinct().ToList();
                return ridList;
            }
        }

        public DataSourceResult GetRegistrationIdByDepartment(DataSourceRequest Req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var UserDepartment = (from userMaster in dbContext.UmUserMasters
                                      join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                      where userMaster.UserRefId == userInfo.UserID
                                      select departmentTrans.DepartmentId).ToList();

                var ridList = (from alot in dbContext.AllotmentMasters
                               where UserDepartment.Contains(alot.departmentId)
                               select new RidList
                               {
                                   id = alot.rid,
                                   text = alot.rid.ToString(),
                                   DepartmentId = alot.departmentId != null ? (int)alot.departmentId : 0
                               }).Distinct();
                return ridList.ToDataSourceResult(Req);
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


        public List<DDList> GetAllLettersType()
        {
            List<DDList> letterList = new List<DDList>();
            foreach (int value in Enum.GetValues(typeof(LetterTypes)))
            {
                letterList.Add(new DDList
                {
                    text = Enum.GetName(typeof(LetterTypes), value),
                    id = value
                });
            }
            return letterList;
        }

        public List<DDList> GetServicesByDepartment(int? departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var serviceList = (from service in dbContext.CitizenService_Master
                                   where service.Deptt_Id == departmentId
                                   select new DDList
                                   {
                                       text = service.ServiceName,
                                       id = service.service_id.Value
                                   }).ToList();
                return serviceList;
            }
        }


        public PropertyInfoModel GetAllotteDetailsToGenerateLetter(int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var allotment = dbContext.AllotmentMasters.Where(a => a.rid == rid).FirstOrDefault();
                if (allotment != null)
                {
                    var allotteedetails = (from alot in dbContext.AllotmentMasters
                                           join appl in dbContext.ApplicationDetails on alot.applicationId equals appl.applicationId
                                           join scpt in dbContext.SchemePropTrans on alot.propertyId equals scpt.propertyId
                                           where alot.rid == rid
                                           select new PropertyInfoModel
                                           {
                                               Rid = rid,
                                               DepartmentId = alot.departmentId,
                                               DepartmentName = alot.DepartmentMst.departmentName,
                                               FirstName = appl.tFirstName,
                                               MiddleName = appl.tMiddleName,
                                               LastName = appl.tLastName,
                                               ApplicantName = appl.tFirstName + " " + appl.tMiddleName + " " + appl.tLastName,
                                               MobileNo = appl.tMobileNumber,
                                               Email = appl.tEmail,
                                               CorresspondentAddress = appl.tCorrespondanceAdd,
                                               PermanentAddress = appl.tPermanentAdd,
                                               IndividualOrCompany = appl.tGender,
                                               AutorizedSignatory = appl.tSigningAuthority,
                                               FatherOrHusbandName = appl.tFatherHusbandName,
                                               Sector = scpt.SectorMst.sectorName,
                                               Block = scpt.BlockMst.blockName,
                                               PlotNo = scpt.propertyNo,
                                               KYAStatusId = dbContext.KYADetails.OrderByDescending(o => o.Id).FirstOrDefault(m => m.RId == rid && m.IsActive == true).StatusId,
                                           }).FirstOrDefault();
                    if (allotteedetails.KYAStatusId != null)
                    {
                        allotteedetails.KYAStatus = dbContext.StatusMasters.FirstOrDefault(m => m.Id == allotteedetails.KYAStatusId && m.IsActive == true).Status;
                    }
                    //var kya = dbContext.KYADetails.Where(k => k.RId == rid).OrderByDescending(o => o.Id).FirstOrDefault();
                    //if (kya != null)
                    //{
                    //    if (kya.IsActive == false && kya.StatusId == NAStatusId.CancelAfterTransfer)
                    //    {
                    //        allotteedetails.KYAStatusId = NAStatusId.Approved;
                    //    }
                    //    else
                    //    {
                    //        allotteedetails.KYAStatusId = kya.StatusId;
                    //    }
                    //}
                    return allotteedetails;
                }
                else
                {
                    return null;
                }
            }
        }


        public string GenerateLetterByService(int? rid, int? departmentId, int? serviceId, int? letterId, DateTime? letterDate)
        {
            string strContent = string.Empty;
            if (userInfo != null)
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    var flag = dbContext.usp_getServiceCheck(rid, letterId).FirstOrDefault();
                    if (flag.Equals(1))
                    {
                        ObjectParameter commaString = new ObjectParameter("CommaString", typeof(string));
                        //dbContext.Sp_LatterPrintTemp_new(rid.ToString(), letterId, departmentId, userInfo.UserID.ToString(), letterDate.Value.Date, commaString);
                        dbContext.Sp_LatterPrintTemp(rid.ToString(), letterId, departmentId, userInfo.UserID.ToString(), letterDate.Value.Date, commaString);
                        strContent = commaString.Value.ToString();
                    }
                    else
                    {
                        strContent = flag.ToString();
                    }
                }
            }
            return strContent;
        }


        public List<DDList> GetServicesRequestListByRid(int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var serviceList = (from request in dbContext.Customer_ServiceRequest
                                   join service in dbContext.CitizenService_Master on new { x = request.DepartmentId, y = request.ServiceId } equals new { x = service.Deptt_Id, y = service.service_id }
                                   where request.Registration_No == rid.ToString()
                                   select new DDList
                                   {
                                       text = service.ServiceName,
                                       //text = EntityFunctions.TruncateTime(request.Created_Date).ToString(),
                                       //text = DbFunctions.TruncateTime(request.Created_Date).ToString(),
                                       id = request.Id
                                   }).Distinct().ToList();
                return serviceList;
            }
        }


        public int? GetServiceRequestStatus(int? requestId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int? serviceList = dbContext.Customer_ServiceRequest.Where(c => c.Id == requestId).Select(x => x.Request_Status).FirstOrDefault();
                return serviceList;
            }
        }


        public int GetServiceRequestStatusForLetter(int? rid, int? letterId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var flag = dbContext.usp_getServiceCheck(rid, letterId).FirstOrDefault();
                return Convert.ToInt32(flag);
            }
        }


        public List<DDList> GetLettersByTemplateDepartment(int? departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var letters = (from template in dbContext.TemplateMasters
                               where template.departmentId == departmentId // && template.templateId == templateId
                               select new DDList
                               {
                                   id = template.templateId.Value,
                                   text = template.templateName
                               }).ToList();
                return letters;
            }
        }



        public List<DDList> GetServiceRequestStatusList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var status = (from stats in dbContext.StatusMasters
                              where stats.IsActive == true
                              select new DDList
                              {
                                  id = stats.Id,
                                  text = stats.Status
                              }).ToList();
                return status;
            }
        }


        public DataSourceResult GetRegistrationIdList(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDepartment = (from userMaster in dbContext.UmUserMasters
                                           join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                           where userMaster.UserRefId == userInfo.UserID
                                           select departmentTrans.DepartmentId).ToList();

                var rid = (from alot in dbContext.AllotmentMasters
                           where loginUserDepartment.Contains(alot.departmentId) && alot.isActive == 1
                           select new DDList
                           {
                               id = alot.rid,
                               text = alot.rid.ToString()
                           });
                return rid.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetPropertyIdList(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDepartment = (from userMaster in dbContext.UmUserMasters
                                           join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                           where userMaster.UserRefId == userInfo.UserID
                                           select departmentTrans.DepartmentId).ToList();

                var rid = (from property in dbContext.SchemePropTrans
                           where loginUserDepartment.Contains(property.departmentId)
                           select new DDList
                           {
                               id = property.propertyId.Value,
                               text = property.propertyId.ToString()
                           });
                return rid.ToDataSourceResult(request);
            }
        }


        public ApplicantModel GetApplicantDetailToSendMessage(int? registrationId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                ApplicantModel model = new ApplicantModel();
                var applicant = dbContext.ApplicationDetails.FirstOrDefault(r => r.registrationId == registrationId);
                if (applicant != null)
                {
                    model = (from appl in dbContext.ApplicationDetails
                             where appl.registrationId == registrationId
                             select new ApplicantModel
                             {
                                 RegistrationId = appl.registrationId,
                                 FirstName = appl.tFirstName,
                                 MiddleName = appl.tMiddleName,
                                 LastName = appl.tLastName,
                                 MobileNo = appl.tMobileNumber,
                                 Email = appl.tEmail,
                                 CorresspondingAddress = appl.tCorrespondanceAdd,
                                 ApplicantName = appl.tGender == Constants.Company ? appl.T_Company_Name : appl.tFirstName + " " + appl.tMiddleName + " " + appl.tLastName,
                                 ApplicantMaster = appl.tGender == Constants.Company ? appl.tSigningAuthority : appl.tFatherHusbandName
                             }).FirstOrDefault();
                }
                return model;
            }
        }


        public bool SendMessageToApplicant(ApplicantModel model)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var applicant = dbContext.AllotmentMasters.Where(a => a.rid == model.RegistrationId).FirstOrDefault();
                if (applicant != null)
                {
                    ApplicationHelper.SMSSend(model.MobileNo, model.MessageValue);
                    if (applicant.ApplicationDetail.tMobileNumber != null)
                    {
                        ApplicationHelper.SMSSend(applicant.ApplicationDetail.tMobileNumber, model.MessageValue);
                        flag = true;
                    }

                    if (applicant.ApplicationDetail.tEmail != null)
                    {
                        string body = "Dear User," + model.Message + ".Regards, http://mynoida.in";
                        EmailHelper emailHelper = new EmailHelper();
                        emailHelper.Send(applicant.ApplicationDetail.tEmail, "Request submitted", body);
                        flag = true;
                    }
                }
                else
                {
                    ApplicationHelper.SMSSend(model.MobileNo, model.MessageValue);

                    string body = "Dear User," + model.Message + ".Regards, http://mynoida.in";
                    EmailHelper emailHelper = new EmailHelper();
                    emailHelper.Send(applicant.ApplicationDetail.tEmail, "Request submitted", body);
                    flag = true;
                }
            }
            return flag;
        }




        public NDCVeiwModel GetApplicantDetailsForNDC(string registrationId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int rId = Convert.ToInt32(registrationId);
                var alotee = dbContext.ApplicationDetails.Where(a => a.registrationId == rId).FirstOrDefault();

                var usr = (from alot in dbContext.AllotmentMasters
                           join prop in dbContext.SchemePropTrans on alot.propertyId equals prop.propertyId
                           join ndc in dbContext.PRE_FULL_PAYMENT_NDC on alot.rid equals ndc.RegistrationId
                           where alot.rid == rId
                           select new NDCVeiwModel
                           {
                               RegistrationId = alot.rid.ToString(),
                               FirstName = alot.ApplicationDetail.tFirstName,
                               MiddleName = alot.ApplicationDetail.tMiddleName,
                               LastName = alot.ApplicationDetail.tLastName,
                               Mobile = alot.ApplicationDetail.tMobileNumber,
                               Email = alot.ApplicationDetail.tEmail,
                               Sector = prop.SectorMst.sectorName,
                               Block = prop.BlockMst.blockName,
                               PlotNo = prop.propertyNo,
                               Address = alot.ApplicationDetail.tCorrespondanceAdd,
                               PropertyNo = prop.SectorMst.sectorName + "/" + prop.BlockMst.blockName + "-" + prop.propertyNo,
                               RegistryDate = alot.RegistryDetails.Where(m => m.Rid == rId).FirstOrDefault() != null ? alot.RegistryDetails.Where(m => m.Rid == rId).FirstOrDefault().RegistryDoneDate : null,

                               LeaseRentAmount = ndc.LeaseRentAmount,
                               LastYearLeaseRentPaidUpto = ndc.LeaseRentUpto,
                               NDCDate = ndc.NDCDate

                           }).FirstOrDefault();
                //return usr;
                if (usr != null)
                {
                    if (alotee.tGender.ToLower() == Constants.Company.ToLower())
                    {
                        usr.Applicant = !string.IsNullOrEmpty(alotee.T_Company_Name) ? alotee.T_Company_Name : alotee.tFirstName;
                    }
                    else
                    {
                        usr.Applicant = alotee.tFirstName + " " + (string.IsNullOrEmpty(alotee.tMiddleName) ? string.Empty : alotee.tMiddleName + " ") + alotee.tLastName;
                    }
                }

                return usr;
            }
        }

        public List<DropdownViewModel> GetYesNoStatus()
        {
            List<DropdownViewModel> list = new List<DropdownViewModel>();
            list.Add(new DropdownViewModel { Text = "Yes", Value = "Y" });
            list.Add(new DropdownViewModel { Text = "No", Value = "N" });
            return list;
        }

        public List<DropdownViewModel> GetMortgageLoanStatus()
        {
            List<DropdownViewModel> LoanStatus = new List<DropdownViewModel>();
            foreach (int value in Enum.GetValues(typeof(MortgagePrevLoan)))
            {
                LoanStatus.Add(new DropdownViewModel
                {
                    Text = Enum.GetName(typeof(MortgagePrevLoan), value),
                    Id = value
                });
            }
            return LoanStatus;
        }


        public DataSourceResult GetLettersTemplateByDepartment(DataSourceRequest request, int? departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var letters = (from template in dbContext.TemplateMasters
                               where template.departmentId == departmentId // && template.templateId == templateId
                               select new SectorDDList
                               {
                                   id = template.templateId.Value,
                                   text = template.templateName,
                                   DepartmentId = (int)template.departmentId
                               }).ToList();
                return letters.ToDataSourceResult(request);
            }
        }


        public List<DropdownViewModel> GetSchemeList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var schemeList = (from scheme in dbContext.SchemeMsts
                                  where scheme.IsActive == true && scheme.completed == true && scheme.Status != Constants.SchemeClosed
                                  select new DropdownViewModel
                                  {
                                      Id = scheme.schemeId,
                                      Text = scheme.schemeName
                                  }).ToList();
                return schemeList;
            }
        }

        public List<DropdownViewModel> GetDepartmentListByScheme(int schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var department = (from deptt in dbContext.DepartmentMsts
                                  join deptrans in dbContext.SchemeDepartmentTrans on deptt.departmentId equals deptrans.departmentId
                                  where deptt.IsActive == true && deptrans.schemeId == schemeId
                                  select new DropdownViewModel
                                  {
                                      Id = deptt.departmentId,
                                      Text = deptt.departmentName
                                  }).ToList();
                return department;
            }
        }

        public DataSourceResult GetDepartmentListByScheme(DataSourceRequest request, int? schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var department = (from deptt in dbContext.DepartmentMsts
                                  join deptrans in dbContext.SchemeDepartmentTrans on deptt.departmentId equals deptrans.departmentId
                                  where deptt.IsActive == true && deptrans.IsActive == true
                                  && DepartmentList.Contains(deptt.departmentId)
                                  && (schemeId == null || deptrans.schemeId == schemeId)
                                  select new DropdownViewModelDepartment
                                  {
                                      Id = deptt.departmentId,
                                      Text = deptt.departmentName,
                                      SchemeId = deptrans.schemeId != null ? (int)deptrans.schemeId : 0,
                                      Value = string.Empty
                                  });
                return department.ToDataSourceResult(request);
            }
        }

        public List<DropdownViewModel> GetGenderList()
        {
            List<DropdownViewModel> genderList = new List<DropdownViewModel>();
            genderList.Add(new DropdownViewModel { Text = Constants.Male });
            genderList.Add(new DropdownViewModel { Text = Constants.Female });
            return genderList;
        }

        public List<DropdownViewModel> GetMaritalStatusList()
        {
            List<DropdownViewModel> maritialStatus = new List<DropdownViewModel>();
            foreach (int value in Enum.GetValues(typeof(MaritialStatus)))
            {
                maritialStatus.Add(new DropdownViewModel
                {
                    Text = Enum.GetName(typeof(MaritialStatus), value),
                    Id = value
                });
            }
            return maritialStatus;
        }

        public List<DropdownViewModel> GetCategoryList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.QuotaMsts
                           where prop.IsActive == true
                           select new DropdownViewModel
                           {
                               Id = prop.quotaId,
                               Text = prop.quotaName,
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetOccupationList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from f in dbContext.OccupationMsts
                           select new DropdownViewModel
                           {
                               Id = f.occupationId,
                               Text = f.occupation
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetCompanyTypeList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from config in dbContext.Common_Config
                           where config.Is_Active == 1 && config.Category.ToLower() == "companyType".ToLower()
                           select new DropdownViewModel
                           {
                               Id = config.Id,
                               Text = config.Name
                           }).ToList();
                return lst;
            }
        }


        public List<DropdownViewModel> GetBankList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.BankMsts
                           where prop.IsActive == true
                           select new DropdownViewModel
                           {
                               Id = prop.bankId,
                               Text = prop.bankName
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetBankListBySchemeId(int schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.SchemeBankTrans
                           where prop.IsActive == true && prop.schemeId == schemeId
                           select new DropdownViewModel
                           {
                               Id = prop.BankMst.bankId,
                               Text = prop.BankMst.bankName
                           }).ToList();
                return lst;
            }
        }

        public ChallanBankandAccountNo GetAccountBranchBySchemeIdBankId(int schemeId, int BankId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.SchemeBankTrans
                           where prop.IsActive == true && prop.schemeId == schemeId && prop.bankId == BankId
                           select new ChallanBankandAccountNo
                           {
                               BranchId = prop.BranchMst.branchId,
                               BranchName = prop.BranchMst.branchName,
                               AccountNo = prop.accountnumber,
                               BankIFSC = prop.IFSCCode,
                               VirtualAccountPrefix = prop.virtualAccountprefix
                           }).FirstOrDefault();
                return lst;
            }
        }

        public List<DropdownViewModel> GetPropertyTypeList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.PropertyTypeMsts
                           where prop.IsActive == true
                           select new DropdownViewModel
                           {
                               Id = prop.propertyTypeId,
                               Text = prop.propertyTypeName
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetPropertyTypeListById(int departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.PropertyTypeMsts
                           where prop.IsActive == true && prop.departmentId == departmentId
                           select new DropdownViewModel
                           {
                               Id = prop.propertyTypeId,
                               Text = prop.propertyTypeName
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetPropertyTypeListByDepartment(int departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.PropertyTypeMsts
                           where prop.IsActive == true && prop.departmentId == departmentId
                           select new DropdownViewModel
                           {
                               Id = prop.propertyTypeId,
                               Text = prop.propertyTypeName
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetFloorAreaListByDepartment(int departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from f in dbContext.FloorMsts
                           where f.IsActive == true && f.departmentId == departmentId
                           select new DropdownViewModel
                           {
                               Id = f.floorId,
                               Text = f.floorName
                           }).ToList();
                return lst;
            }
        }


        public List<DropdownViewModel> GetPropertyTypeListForOnline(int schemeId, int departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from scheme in dbContext.SchemeCostTrans
                           //join prop in dbContext.PropertyTypeMsts on scheme.departmentId equals prop.departmentId
                           where scheme.schemeId == schemeId && scheme.departmentId == departmentId && scheme.IsActive == true
                           select new DropdownViewModel
                           {
                               Id = scheme.PropertyTypeMst.propertyTypeId,
                               Text = scheme.PropertyTypeMst.propertyTypeName
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetFloorAreaListForOnline(int schemeId, int departmentId, int propertyTypeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from scheme in dbContext.SchemeCostTrans
                           //join floor in dbContext.FloorMsts on scheme.departmentId equals floor.departmentId
                           where scheme.schemeId == schemeId && scheme.departmentId == departmentId && scheme.propertyTypeId == propertyTypeId && scheme.IsActive == true
                           select new DropdownViewModel
                           {
                               Id = scheme.FloorMst.floorId,
                               Text = scheme.FloorMst.floorName
                           }).ToList();
                return lst;
            }
        }

        //public List<DropdownViewModel> GetAreaList()
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        List<DropdownViewModel> list = new List<DropdownViewModel>();

        //        for (int i = 5; i <= 75; )
        //        {
        //            DropdownViewModel floor = new DropdownViewModel();
        //            floor.Id = i;
        //            floor.Text = i.ToString();
        //            i = i + 5;
        //            list.Add(floor);
        //        }
        //        return list;
        //    }
        //}


        public List<DropdownViewModel> GetDepartmentList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from dept in dbContext.DepartmentMsts
                           where dept.IsActive == true
                           select new DropdownViewModel
                           {
                               Id = dept.departmentId,
                               Text = dept.departmentName
                           }).ToList();
                return lst;
            }
        }

        public DataSourceResult GetAllDepartmentList(DataSourceRequest Req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from dept in dbContext.DepartmentMsts
                           where dept.IsActive == true
                           select new DropdownViewModel
                           {
                               Id = dept.departmentId,
                               Text = dept.departmentName
                           });
                return lst.ToDataSourceResult(Req);
            }
        }

        public List<DropdownViewModel> GetServiceListByDepartment(int departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from service in dbContext.CitizenService_Master
                           where service.Deptt_Id == departmentId && service.Status == 1
                           select new DropdownViewModel
                           {
                               Id = service.service_id.Value,
                               ServiceId = service.service_id,
                               Text = service.ServiceName
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetServiceListByDepartmentForNAServices(int departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (departmentId == NADepartment.Industrial)
                {
                    //List<int?> NICServiceIdList = new List<int?>(){ 1, 2,4, 6, 10, 12, 13,17, 19,22 };
                    var niclist = (from service in dbContext.CitizenService_Master
                                   join trans in dbContext.ServiceTrans on service.Id equals trans.CitizenServiceId
                                   where service.Deptt_Id == departmentId && service.Status == 1
                                   select new DropdownViewModel
                                   {
                                       Id = service.service_id.Value,
                                       ServiceId = service.service_id,
                                       Text = service.ServiceName
                                   }).ToList();

                    var lstInt = niclist.Select(c => c.ServiceId).ToList();
                    var lst = (from service in dbContext.CitizenService_Master //not contain NIC service list
                               where service.Deptt_Id == departmentId && service.Status == 1 && !lstInt.Contains(service.service_id)
                               select new DropdownViewModel
                               {
                                   Id = service.service_id.Value,
                                   ServiceId = service.service_id,
                                   Text = service.ServiceName
                               }).ToList();
                    return lst;
                }
                else
                {
                    var lst = (from service in dbContext.CitizenService_Master
                               where service.Deptt_Id == departmentId && service.Status == 1
                               select new DropdownViewModel
                               {
                                   Id = service.service_id.Value,
                                   ServiceId = service.service_id,
                                   Text = service.ServiceName
                               }).ToList();
                    return lst;
                }
            }
        }

        public List<DropdownViewModel> GeSubtDepartmentList(int departmentId)
        {
            //List<DropdownViewModel> list = new List<DropdownViewModel>();
            //list.Add(new DropdownViewModel { Text = "Property" });
            //list.Add(new DropdownViewModel { Text = "Accounts" });
            //return list;

            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from service in dbContext.SubDepartmentMsts
                           where service.departmentId == departmentId && service.IsActive == true
                           select new DropdownViewModel
                           {
                               Id = service.SubdepartmentId,
                               Text = service.SubdepartmentName
                           }).ToList();
                return lst;
            }
        }


        public List<DropdownViewModel> GetTransferTypeList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from transTy in dbContext.Transfer_Type
                           where transTy.Parent_Id == null && transTy.Is_Active == true
                           select new DropdownViewModel
                           {
                               Text = transTy.type,
                               Id = transTy.Id
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetTransferSubTypeList(int transferTypeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from transTy in dbContext.Transfer_Type
                           where transTy.Parent_Id == transferTypeId && transTy.Is_Active == true
                           select new DropdownViewModel
                           {
                               Text = transTy.type,
                               Id = transTy.Id
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetCICRequestTypeList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from cfg in dbContext.Common_Config
                           where cfg.Is_Active == 1 && cfg.Category.ToLower().Equals(CategoryType.CIC.ToLower())
                           select new DropdownViewModel
                           {
                               Id = cfg.Id,
                               Text = cfg.Name
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetCompanyMemberTypeList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from cfg in dbContext.Common_Config
                           where cfg.Is_Active == 1 && cfg.Category.ToLower().Equals(CategoryType.Director.ToLower())
                           select new DropdownViewModel
                           {
                               Id = cfg.Id,
                               Text = cfg.Name
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetFirmStatusList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from cfg in dbContext.Common_Config
                           where cfg.Is_Active == 1 && cfg.Category.ToLower().Equals(CategoryType.FirmStatus.ToLower())
                           select new DropdownViewModel
                           {
                               Id = cfg.Id,
                               Text = cfg.Name
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetMortgageTypeList()
        {
            List<DropdownViewModel> mortgageType = new List<DropdownViewModel>();
            foreach (int value in Enum.GetValues(typeof(MortgageType)))
            {
                mortgageType.Add(new DropdownViewModel
                {
                    Text = Enum.GetName(typeof(MortgageType), value),
                    Id = value
                });
            }
            return mortgageType;
        }

        public List<DropdownViewModel> GetGPAStatusList()
        {
            List<DropdownViewModel> gender = new List<DropdownViewModel>();
            foreach (int value in Enum.GetValues(typeof(EnumStatusType)))
            {
                gender.Add(new DropdownViewModel
                {
                    Text = Enum.GetName(typeof(EnumStatusType), value),
                    Id = value
                });
            }
            return gender;
        }

        public List<DropdownViewModel> GetNOCStatusList()
        {
            List<DropdownViewModel> statusList = new List<DropdownViewModel>();
            foreach (int value in Enum.GetValues(typeof(MortgagePrevLoan)))
            {
                statusList.Add(new DropdownViewModel
                {
                    Text = Enum.GetName(typeof(MortgagePrevLoan), value),
                    Id = value
                });
            }
            return statusList;
        }


        public List<DropdownViewModel> GetFloorAreaListForOnline(int schemeId, int departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (departmentId == 18)
                {
                    List<DropdownViewModel> list = new List<DropdownViewModel>();

                    for (int i = 5; i <= 75; )
                    {
                        DropdownViewModel floor = new DropdownViewModel();
                        floor.Id = 1029; // i; for testing server 1029, production server 1040
                        floor.Text = i.ToString();
                        i = i + 5;
                        list.Add(floor);
                    }
                    return list;
                }
                if (departmentId == 19)
                {
                    List<DropdownViewModel> list = new List<DropdownViewModel>();

                    DropdownViewModel floor = new DropdownViewModel();
                    floor.Id = 1030; //for testing server 1030, production server 1041
                    floor.Text = "120-150";
                    list.Add(floor);
                    return list;
                }
                else
                {
                    var lst = (from scheme in dbContext.SchemeCostTrans
                               where scheme.schemeId == schemeId && scheme.departmentId == departmentId && scheme.IsActive == true
                               select new DropdownViewModel
                               {
                                   Id = scheme.FloorMst.floorId,
                                   Text = scheme.FloorMst.floorName
                               }).ToList();


                    var lst1 = (from l in lst
                                group l by new { l.Id, l.Text } into u
                                select new DropdownViewModel
                                {
                                    Id = u.Key.Id,
                                    Text = u.Key.Text
                                }).ToList();

                    return lst1;
                }


            }
        }


        public List<DropdownViewModel> GetDirectorTypeList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from cfg in dbContext.Common_Config
                           where cfg.Is_Active == 1 && cfg.Category.ToLower().Equals(CategoryType.Director.ToLower())
                           select new DropdownViewModel
                           {
                               Id = cfg.Id,
                               Text = cfg.Name
                           }).ToList();
                return lst;
            }
        }


        public List<DropdownViewModel> GetCompanyTypeByCategory(string typeName)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from config in dbContext.Common_Config
                           where config.Is_Active == 1 && config.Category.ToLower() == typeName.ToLower()
                           select new DropdownViewModel
                           {
                               Id = config.Id,
                               Text = config.Name
                           }).ToList();
                return lst;
            }
        }


        public List<DropdownViewModel> getSectorsList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from sector in dbContext.SectorMsts
                           where sector.IsActive == true
                           select new DropdownViewModel
                           {
                               Id = sector.sectorId,
                               Text = sector.sectorName
                           }).ToList();
                return lst;
            }
        }


        public List<DropdownViewModel> getStatusMasterList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from status in dbContext.StatusMasters
                           where status.IsActive == true
                           select new DropdownViewModel
                           {
                               Id = status.Id,
                               Text = status.Status
                           }).ToList();
                return lst;
            }
        }



        public int SaveSectorBlockName(string type, string typeName)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var flag = ReturnType.None;
                if (type.ToLower() == "sector")
                {
                    var sector = dbContext.SectorMsts.FirstOrDefault(x => x.sectorName.ToLower() == typeName.ToLower());
                    if (sector == null)
                    {
                        SectorMst sect = new SectorMst();
                        sect.sectorName = typeName;
                        sect.IsActive = true;
                        sect.createdBy = userInfo.UserID.ToString();
                        sect.createdDate = DateTime.Now;
                        dbContext.SectorMsts.Add(sect);
                        dbContext.SaveChanges();
                        flag = ReturnType.Saved;
                    }
                    else { flag = ReturnType.Exist; }
                }
                if (type.ToLower() == "block")
                {
                    var block = dbContext.BlockMsts.FirstOrDefault(x => x.blockName.ToLower() == typeName.ToLower());
                    if (block == null)
                    {
                        BlockMst blok = new BlockMst();
                        blok.blockName = typeName;
                        blok.IsActive = true;
                        blok.createdBy = userInfo.UserID.ToString();
                        blok.createdDate = DateTime.Now;
                        dbContext.BlockMsts.Add(blok);
                        dbContext.SaveChanges();
                        flag = ReturnType.Saved;
                    }
                    else { flag = ReturnType.Exist; }
                }
                return flag;
            }
        }


        public List<DropdownViewModel> GetResourceMessageList()
        {
            List<DropdownViewModel> messageList = new List<DropdownViewModel>();
            //ResourceSet resourceSet = MyResourceClass.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            //ResourceSet resourceSet = NAMessages.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            //foreach (DictionaryEntry entry in resourceSet)
            //{
            //    string resourceKey = entry.Key.ToString();
            //    object resource = entry.Value;
            //}

            ResourceSet resourceSet = NAMessages.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                var message = new DropdownViewModel();
                message.Text = entry.Key.ToString();
                message.Value = (string)entry.Value;
                messageList.Add(message);
            }

            return messageList;
        }


        public List<DropdownViewModel> GetFormTypeList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Is_Active == 1 && config.Category == "FormType"
                            select new DropdownViewModel
                            {
                                Id = config.Id,
                                Text = config.Name
                            }).ToList();
                return list;
            }
        }

        public List<DropdownViewModel> GetApplicantTypeList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Is_Active == 1 && config.Category == "ApplicantType"
                            select new DropdownViewModel
                            {
                                Id = config.Id,
                                Text = config.Name
                            }).ToList();
                return list;
            }
        }

        public List<DropdownViewModel> GetFormSubTypeList(string formtype)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Is_Active == 1 && config.Category == formtype
                            select new DropdownViewModel
                            {
                                Id = config.Id,
                                Text = config.Name
                            }).ToList();
                return list;
            }
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


        public List<DropdownViewModel> GetOnlineApplicationFormIdList(DataSourceRequest request, int? schemeId, int? departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var DepartmentList = (from deptTrans in dbContext.UmUserDepartmentTrans where deptTrans.UserRefId == userInfo.UserID && deptTrans.Status == true select deptTrans.DepartmentId).ToList();
                var existingIdList = dbContext.OnlineSchemeDraws.Where(s => s.SchemeId == schemeId && s.DepartmentId == departmentId).Select(o => o.OnlineApplicationId).ToList();
                var formIdList = (from application in dbContext.OnlineApplicationDetails
                                  where application.schemeId == schemeId && application.departmentId == departmentId && application.isActive == true && !existingIdList.Contains(application.onlineapplicationId)
                                  select new DropdownViewModel
                                  {
                                      Id = application.onlineapplicationId,
                                      Text = application.onlineapplicationId.ToString()
                                  }).ToList();
                return formIdList;//.ToDataSourceResult(request);
            }
        }


        public List<DropdownViewModel> GetApplicationList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from appl in dbContext.UmApplicationMasters
                            where appl.IsActive == true
                            select new DropdownViewModel
                            {
                                Text = appl.ApplicationName,
                                Id = appl.ApplicationId
                            }).ToList();
                return list;
            }
        }

        public List<DropdownViewModel> GetMenuList(int? parentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from menu in dbContext.UmMenuMasters
                            where menu.IsActive == true && (parentId == null || menu.MenuParentId == parentId)
                            select new DropdownViewModel
                            {
                                Text = menu.MenuName,
                                Id = menu.MenuId
                            }).ToList();
                return list;
            }
        }

        public List<DropdownViewModel> GetParentMenuList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from menu in dbContext.UmMenuMasters
                            where menu.IsActive == true //&& menu.MenuParentId == null
                            select new DropdownViewModel
                            {
                                Text = menu.MenuName,
                                Id = menu.MenuId
                            }).ToList();
                return list;
            }
        }

        public List<DropdownViewModel> GetAreaRangeList(int department)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from floor in dbContext.FloorMsts
                            where floor.IsActive == true && floor.departmentId == department
                            select new DropdownViewModel
                            {
                                Text = floor.floorName,
                                Id = floor.floorId
                            }).ToList();
                return list;
            }
        }

        public List<DropdownViewModel> GetCommonConfigCategoryList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Is_Active == 1
                            select new DropdownViewModel
                            {
                                Text = config.Category,
                                Id = config.Id
                            }).ToList();

                List<DropdownViewModel> categorylist = list.GroupBy(x => x.Text).Select(y => y.First()).ToList();
                return categorylist;
            }
        }

        public List<DropdownViewModel> GetCommonConfigDataList(string category)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Is_Active == 1 && config.Category == category
                            select new DropdownViewModel
                            {
                                Text = config.Name,
                                Id = config.Id
                            }).ToList();
                return list;
            }
        }

        public List<DropdownViewModel> GetSchemeTypeList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from scheme in dbContext.SchemeTypeMsts
                            where scheme.IsActive == true
                            select new DropdownViewModel
                            {
                                Text = scheme.SchemeTypeDesc,
                                Id = scheme.schemeTypeId
                            }).ToList();
                return list;
            }
        }

        public List<DropdownViewModel> GetPropertyTypeByDepartment(int departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from property in dbContext.PropertyTypeMsts
                            where property.IsActive == true && property.departmentId == departmentId
                            select new DropdownViewModel
                            {
                                Text = property.propertyTypeName,
                                Id = property.propertyTypeId
                            }).ToList();
                return list;
            }
        }


        public List<DropdownViewModel> GetStatusList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from status in dbContext.StatusMasters
                            where status.IsActive == true
                            select new DropdownViewModel
                            {
                                Text = status.Status,
                                Id = status.Id
                            }).ToList();
                return list;
            }
        }

        public List<DropdownViewModel> GetStatusListByType(string type)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from status in dbContext.StatusMasters
                            where status.IsActive == true && status.StatusType == type
                            select new DropdownViewModel
                            {
                                Text = status.Status,
                                Id = status.Id
                            }).ToList();
                return list;
            }
        }

        public List<DropdownViewModel> GetStatusTypeList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from status in dbContext.StatusMasters
                            where status.IsActive == true
                            select new DropdownViewModel
                            {
                                Text = status.Status,
                                Value = status.StatusType
                            }).ToList();

                List<DropdownViewModel> statuslist = list.GroupBy(x => x.Value).Select(y => y.First()).ToList();
                return statuslist;
            }
        }

        public int SaveApplication(CommonViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var app = dbContext.UmApplicationMasters.FirstOrDefault(a => a.ApplicationName.ToLower() == model.Application.ToLower());
                if (app == null)
                {
                    if (model.ActionType == "Add")
                    {
                        var applist = dbContext.UmApplicationMasters.ToList().OrderByDescending(x => x.ApplicationId);
                        var appl = applist.FirstOrDefault();
                        UmApplicationMaster application = new UmApplicationMaster();
                        application.ApplicationId = appl.ApplicationId + 1;
                        application.ApplicationName = model.Application;
                        application.IsActive = true;
                        application.CreatedBy = userInfo.UserID.ToString();
                        application.CreatedDate = DateTime.Now;
                        dbContext.UmApplicationMasters.Add(application);
                        dbContext.SaveChanges();
                        flag = ReturnType.Saved;
                    }
                    else if (model.ActionType == "Update")
                    {
                        var exapp = dbContext.UmApplicationMasters.FirstOrDefault(a => a.ApplicationId == model.ApplicationId && a.IsActive == true);
                        exapp.ApplicationName = model.Application;
                        exapp.ModifiedBy = userInfo.UserID.ToString();
                        exapp.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Updated;
                    }
                }
            }
            return flag;
        }


        public int SaveSchemeType(CommonViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var scheme = dbContext.SchemeTypeMsts.FirstOrDefault(s => s.SchemeTypeDesc.ToLower() == model.SchemeTypeDescription.ToLower());
                if (scheme == null)
                {
                    if (model.ActionType == "Add")
                    {
                        SchemeTypeMst schemetype = new SchemeTypeMst();
                        schemetype.SchemeTypeDesc = model.SchemeTypeDescription;
                        schemetype.SchemeType = model.SchemeType;
                        schemetype.IsActive = true;
                        schemetype.createdBy = userInfo.UserID.ToString();
                        schemetype.createdDate = DateTime.Now;
                        dbContext.SchemeTypeMsts.Add(schemetype);
                        dbContext.SaveChanges();
                        flag = ReturnType.Saved;
                    }
                    else if (model.ActionType == "Update")
                    {
                        var exSchemetype = dbContext.SchemeTypeMsts.FirstOrDefault(a => a.schemeTypeId == model.SchemeTypeId && a.IsActive == true);
                        exSchemetype.SchemeTypeDesc = model.SchemeTypeDescription;
                        exSchemetype.SchemeType = model.SchemeType;
                        exSchemetype.modifiedBy = userInfo.UserID.ToString();
                        exSchemetype.modifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Updated;
                    }
                }
            }
            return flag;
        }

        public int SavePropertyType(CommonViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var propertyType = dbContext.PropertyTypeMsts.FirstOrDefault(a => a.departmentId == model.DepartmentId && a.propertyTypeName.ToUpper() == model.PropertyType.ToUpper() && a.IsActive == true);
                if (propertyType == null)
                {
                    if (model.ActionType == "Add")
                    {
                        PropertyTypeMst property = new PropertyTypeMst();
                        property.propertyTypeName = model.PropertyType.ToUpper();
                        property.departmentId = model.DepartmentId;
                        property.TypeId = model.PropertyTypeId;
                        property.IsActive = true;
                        property.createdBy = userInfo.UserID.ToString();
                        property.createdDate = DateTime.Now;
                        dbContext.PropertyTypeMsts.Add(property);
                        dbContext.SaveChanges();
                        flag = ReturnType.Saved;
                    }
                    else if (model.ActionType == "Update")
                    {
                        var exproperty = dbContext.PropertyTypeMsts.FirstOrDefault(a => a.propertyTypeId == model.Id && a.departmentId == model.DepartmentId && a.IsActive == true);
                        exproperty.propertyTypeName = model.PropertyType.ToUpper();
                        exproperty.TypeId = model.PropertyTypeId == null ? exproperty.TypeId : model.PropertyTypeId;
                        exproperty.modifiedBy = userInfo.UserID.ToString();
                        exproperty.modifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Updated;
                    }
                }
            }
            return flag;
        }

        public int SavePropertyAreaRange(CommonViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "Add")
                {
                    FloorMst area = new FloorMst();
                    area.floorName = model.AreaRange;
                    area.category = model.AreaType;
                    area.departmentId = model.DepartmentId;
                    area.IsActive = true;
                    area.createdBy = userInfo.UserID.ToString();
                    area.createdDate = DateTime.Now;
                    dbContext.FloorMsts.Add(area);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                else if (model.ActionType == "Update")
                {
                    var exArea = dbContext.FloorMsts.FirstOrDefault(a => a.floorId == model.AreaRangeId && a.departmentId == model.DepartmentId && a.IsActive == true);
                    exArea.floorName = model.AreaRange;
                    exArea.category = model.AreaType;
                    exArea.modifiedBy = userInfo.UserID.ToString();
                    exArea.modifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }

        public int SaveApplicationMenu(CommonViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "Add")
                {
                    UmMenuMaster menu = new UmMenuMaster();
                    menu.MenuName = model.Menu;
                    menu.MenuParentId = model.ParentMenuId;
                    menu.ApplicationId = model.ApplicationId;
                    menu.IsActive = true;
                    menu.CreatedBy = userInfo.UserID.ToString();
                    menu.CreatedDate = DateTime.Now;
                    dbContext.UmMenuMasters.Add(menu);
                    dbContext.SaveChanges();

                    int id = menu.MenuId;
                    var newMenu = dbContext.UmMenuMasters.FirstOrDefault(m => m.MenuId == id && m.IsActive == true);
                    newMenu.MenuPathId = id;
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                else if (model.ActionType == "UpdateParentMenu")
                {
                    var exMenu = dbContext.UmMenuMasters.FirstOrDefault(a => a.MenuId == model.ParentMenuId && a.ApplicationId == model.ApplicationId && a.IsActive == true);
                    exMenu.MenuName = model.Menu;
                    //exMenu.MenuParentId = model.ParentMenuId;
                    exMenu.ModifiedBy = userInfo.UserID.ToString();
                    exMenu.ModifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                else if (model.ActionType == "Update")
                {
                    var exMenu = dbContext.UmMenuMasters.FirstOrDefault(a => a.MenuId == model.MenuId && a.ApplicationId == model.ApplicationId && a.IsActive == true);
                    exMenu.MenuName = model.Menu;
                    exMenu.MenuParentId = model.ParentMenuId;
                    exMenu.ModifiedBy = userInfo.UserID.ToString();
                    exMenu.ModifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }

        public int SaveCommonConfigData(CommonViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "New-Category")
                {
                    Common_Config config = new Common_Config();
                    config.Name = model.Name;
                    config.Category = model.Category;
                    config.Range = model.Range;
                    config.Is_Active = 1;
                    config.Created_By = userInfo.UserID;
                    config.Created_Date = DateTime.Now;
                    dbContext.Common_Config.Add(config);
                    dbContext.SaveChanges();

                    flag = ReturnType.Saved;
                }
                if (model.ActionType == "Update-Category")
                {
                    var configList = dbContext.Common_Config.Where(a => a.Category == model.Category && a.Is_Active == 1).ToList();
                    configList.Each(x => x.Category = model.Category);
                    //foreach (var list in configList)
                    //{
                    //    list.Category = model.Category;
                    //    list.Modified_By = userInfo.UserID;
                    //    list.Modified_Date = DateTime.Now;
                    //}

                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                if (model.ActionType == "Add-Name")
                {
                    Common_Config config = new Common_Config();
                    config.Name = model.Name;
                    config.Category = model.Category;
                    config.Range = model.Range;
                    config.Is_Active = 1;
                    config.Created_By = userInfo.UserID;
                    config.Created_Date = DateTime.Now;
                    dbContext.Common_Config.Add(config);
                    dbContext.SaveChanges();

                    flag = ReturnType.Saved;
                }
                if (model.ActionType == "Update-Name")
                {
                    var exConfig = dbContext.Common_Config.FirstOrDefault(a => a.Id == model.Id && a.Is_Active == 1);
                    exConfig.Name = model.Name;
                    //exConfig.Category = model.Category;
                    exConfig.Range = model.Range;
                    exConfig.Modified_By = userInfo.UserID;
                    exConfig.Modified_Date = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }

        public int SaveStatusByType(CommonViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "Add" || model.ActionType == "NewStatus")
                {
                    StatusMaster status = new StatusMaster();
                    status.Status = model.Status;
                    status.StatusType = model.StatusType;
                    status.IsActive = true;
                    status.CreatedBy = userInfo.UserID;
                    status.CreatedDate = DateTime.Now;
                    dbContext.StatusMasters.Add(status);
                    dbContext.SaveChanges();

                    flag = ReturnType.Saved;
                }
                else if (model.ActionType == "Update")
                {
                    var exStatus = dbContext.StatusMasters.FirstOrDefault(a => a.Id == model.StatusId && a.IsActive == true);
                    exStatus.Status = model.Status;
                    exStatus.StatusType = model.StatusType;
                    exStatus.Modifiedby = userInfo.UserID;
                    exStatus.ModifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }

        public int SaveDepartment(CommonViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var dept = dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentName == model.Department);
                if (dept == null)
                {
                    if (model.ActionType == "Add")
                    {
                        DepartmentMst department = new DepartmentMst();
                        department.departmentName = model.Department;
                        department.IsActive = true;
                        department.createdBy = userInfo.UserID.ToString();
                        department.createdDate = DateTime.Now;
                        dbContext.DepartmentMsts.Add(department);
                        dbContext.SaveChanges();

                        int id = department.departmentId;
                        UmDepartmentMaster userdept = new UmDepartmentMaster();
                        userdept.DepartmentId = id;
                        userdept.DepartmentName = model.Department;
                        userdept.Status = true;
                        userdept.CreateDate = DateTime.Now;
                        userdept.CreatedBy = userInfo.UserID.ToString();
                        dbContext.UmDepartmentMasters.Add(userdept);
                        dbContext.SaveChanges();

                        flag = ReturnType.Saved;
                    }
                    else if (model.ActionType == "Update")
                    {
                        var exStatus = dbContext.DepartmentMsts.FirstOrDefault(a => a.departmentId == model.DepartmentId);
                        exStatus.departmentName = model.Department;
                        exStatus.IsActive = true;
                        exStatus.modifiedBy = userInfo.UserID.ToString();
                        exStatus.modifiedDate = DateTime.Now;
                        dbContext.SaveChanges();

                        var userdept = dbContext.UmDepartmentMasters.FirstOrDefault(d => d.DepartmentId == model.DepartmentId);
                        userdept.DepartmentName = model.Department;
                        userdept.Status = true;
                        userdept.ModifiedBy = userInfo.UserID.ToString();
                        userdept.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Updated;
                    }
                }
            }
            return flag;
        }


        public CommonViewModel GetPropertyTypeDetailById(CommonViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var exPropertyType = dbContext.PropertyTypeMsts.FirstOrDefault(a => a.propertyTypeId == model.Id && a.departmentId == model.DepartmentId && a.IsActive == true);
                if (exPropertyType != null)
                {
                    model.PropertyTypeId = exPropertyType.propertyTypeId;
                    model.TypeId = exPropertyType.TypeId;
                    model.PropertyType = exPropertyType.propertyTypeName;
                }
                return model;
            }
        }


        public DataSourceResult GetFrequencyList(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Is_Active == 1 && config.Category == "Frequency"
                            select new DropdownViewModel
                            {
                                Text = config.Name,
                                Value = config.Name,
                                Id = config.Range.Value
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetApproverIdList(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from user in dbContext.UmUserMasters
                            join dept in dbContext.UmUserDepartmentTrans on user.UserRefId equals dept.UserRefId
                            join role in dbContext.UmUserMasterRoles on user.UserRefId equals role.UserRefId
                            where user.IsActive == true && DepartmentList.Contains(dept.DepartmentId) && user.UserRefId != userInfo.UserID && role.isActive == true
                            select new DropdownViewModel
                            {
                                Id = user.UserRefId,
                                Text = user.UserName + "-" + user.FirstName
                            }).Distinct();
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetRegistrationIdListAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var ridlist = (from alotment in dbContext.AllotmentMasters
                               //where DepartmentList.Contains(alotment.departmentId) && alotment.isActive == 1
                               where alotment.isActive == 1
                               select new DropdownViewModel
                               {
                                   Id = alotment.rid,
                                   Text = alotment.rid.ToString()
                               });
                return ridlist.ToDataSourceResult(request);
            }
        }

        public AllotmentModel GetApplicantDetails(int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var ApplicantDetails = (from alotment in dbContext.AllotmentMasters
                                        join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                                        join sectr in dbContext.SectorMsts on property.sectorId equals sectr.sectorId
                                        join blok in dbContext.BlockMsts on property.blockId equals blok.blockId
                                        where (rid == null || alotment.rid == rid)
                                            && alotment.isActive == 1 && alotment.isStatus == "Approved"
                                        select new AllotmentModel
                                        {
                                            RID = alotment.rid,
                                            DepartmentName = property.DepartmentMst.departmentName,
                                            ApplicantName = alotment.ApplicationDetail.tFirstName + " " + alotment.ApplicationDetail.tMiddleName + " " + alotment.ApplicationDetail.tLastName,
                                            MobileNumber = alotment.ApplicationDetail.tMobileNumber,
                                            FatherOrHusbandName = alotment.ApplicationDetail.tFatherHusbandName,
                                            SectorName = property.SectorMst.sectorName,
                                            BlockName = property.BlockMst.blockName,
                                            PropertyNumber = property.propertyNo,
                                            CorresspondingAddress = alotment.ApplicationDetail.tCorrespondanceAdd,
                                            DepartmentId = alotment.departmentId
                                        }).FirstOrDefault();
                return ApplicantDetails;
            }
        }


        public DataSourceResult GetPropertyListForAllotment(DataSourceRequest request, int? schemeId, int? departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var AllottedProperty = (from alotment in dbContext.AllotmentMasters
                                        join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                                        where alotment.schemeId == schemeId && alotment.departmentId == departmentId
                                           && alotment.isActive == 1 //&& DepartmentList.Contains(alotment.departmentId)
                                        select alotment.propertyId);

                var ridlist = (from property in dbContext.SchemePropTrans
                               where property.schemeId == schemeId && property.departmentId == departmentId
                                  && property.IsActive == true && !AllottedProperty.Contains(property.propertyId.Value)
                                  && DepartmentList.Contains(property.departmentId)
                               select new DropdownViewModel
                               {
                                   Id = property.propertyId.Value,
                                   Text = property.SectorMst.sectorName + "/" + (property.blockId == null ? "" : property.BlockMst.blockName) + "-" + property.propertyNo,
                                   SchemeId = schemeId,
                                   DepartmentId = departmentId
                               });
                //request.Filters.RemoveAt(0);
                return ridlist.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetOnlineApplicationFormIdListForAllotment(DataSourceRequest request, int? schemeId, int? departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var ridlist = (from aplication in dbContext.OnlineApplicationDetails
                               join transction in dbContext.OnlineApplicationDetails_trans on aplication.onlineapplicationId equals transction.ServiceRefId
                               where aplication.schemeId == schemeId && aplication.departmentId == departmentId
                                  && aplication.isActive == true && DepartmentList.Contains(aplication.departmentId) && transction.status == 1 && transction.TranStatus == 1 //&& aplication.PropertyTypeID == 2
                               select new DropdownViewModel
                               {
                                   Id = aplication.onlineapplicationId,
                                   Text = aplication.onlineapplicationId.ToString(),
                                   SchemeId = schemeId,
                                   DepartmentId = departmentId
                               });
                //request.Filters.RemoveAt(0);
                return ridlist.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetApplicationFormListByScheme(DataSourceRequest request, int? schemeId, int? departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var applist = (from aplication in dbContext.ApplicationDetails
                               join transction in dbContext.ApplicationPaymentDetails on aplication.applicationId equals transction.applicationId
                               where aplication.schemeId == schemeId && aplication.departmentId == departmentId
                               && DepartmentList.Contains(aplication.departmentId) && (aplication.isAllotted == null || aplication.isAllotted != "1")
                               select new DropdownViewModel
                               {
                                   Id = aplication.applicationId,
                                   Text = aplication.applicationId.ToString(),
                                   Value = aplication.formNo,
                                   SchemeId = schemeId,
                                   DepartmentId = departmentId
                               });
                //request.Filters.RemoveAt(0);
                return applist.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetStatusMasterAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from status in dbContext.StatusMasters
                            where status.IsActive == true
                            select new DropdownViewModel
                            {
                                Id = status.Id,
                                Text = status.Status
                            }).ToList();
                return list.ToDataSourceResult(request);
            }
        }


        public CommonViewModel GetCommonConfigDataById(CommonViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "category")
                {
                    model = (from config in dbContext.Common_Config
                             where config.Category == model.Category && config.Is_Active == 1
                             select new CommonViewModel
                             {
                                 Id = config.Id,
                                 Category = config.Category
                             }).FirstOrDefault();
                }
                if (model.ActionType == "name")
                {
                    model = (from config in dbContext.Common_Config
                             where config.Id == model.Id && config.Is_Active == 1
                             select new CommonViewModel
                             {
                                 Id = config.Id,
                                 Name = config.Name,
                                 Range = config.Range
                             }).FirstOrDefault();
                }

                return model;
            }
        }


        public DataSourceResult GetServiceListAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from services in dbContext.CitizenService_Master
                            where services.Status == 1
                            group services by services.service_id into grpservice
                            select new DropdownViewModel
                            {
                                Id = grpservice.FirstOrDefault().service_id.Value,
                                Text = grpservice.FirstOrDefault().ServiceName
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetServicesByDepartmentAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.DepartmentId != null && model.DepartmentId > 0)
                {
                    var list = (from services in dbContext.CitizenService_Master
                                where services.Deptt_Id == model.DepartmentId && services.Status == 1
                                select new DropdownViewModel
                                {
                                    Id = services.service_id.Value,
                                    Text = services.ServiceName
                                });
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    var list = (from services in dbContext.CitizenService_Master
                                where services.Status == 1
                                group services by services.service_id into grpservice
                                select new DropdownViewModel
                                {
                                    Id = grpservice.FirstOrDefault().service_id.Value,
                                    Text = grpservice.FirstOrDefault().ServiceName
                                });
                    return list.ToDataSourceResult(request);
                }
            }
        }


        public DataSourceResult GetBankListAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var bankId = userInfo.OptionalId != null ? userInfo.OptionalId : null;
                var list = (from bank in dbContext.BankMsts
                            where (bankId == null || bank.bankId == bankId) && bank.IsActive == true
                            select new DropdownViewModel
                            {
                                Text = bank.bankName,
                                Id = bank.bankId
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetBranchListOfBankAsDataSource(DataSourceRequest request, int? bankId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from branch in dbContext.BranchMsts
                            where branch.bankId == bankId && branch.IsActive == true
                            select new DropdownViewModel
                            {
                                Text = branch.branchName,
                                Id = branch.branchId
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public int SaveTempChallanChargeDetail(ChallanViewModel model)
        {
            int flag = ReturnType.None;
            List<ChallanViewModel> dataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempChallanModel"];
            if (dataList == null || dataList.Count == 0)
            {
                List<ChallanViewModel> modelList = new List<ChallanViewModel>();
                modelList.Add(model);
                HttpContext.Current.Session["TempChallanModel"] = modelList;
                flag = ReturnType.Saved;
            }
            else
            {
                int count = 0;
                foreach (var data in dataList)
                {
                    if (data.RegistrationId != model.RegistrationId)
                    {
                        dataList = new List<ChallanViewModel>();
                        List<ChallanViewModel> modelList = new List<ChallanViewModel>();
                        modelList.Add(model);
                        HttpContext.Current.Session["TempChallanModel"] = modelList;
                    }
                    else
                    {
                        count++;
                    }
                }
                dataList.Add(model);
                HttpContext.Current.Session["TempChallanModel"] = dataList;
                return flag = ReturnType.Saved;
            }
            return flag;
        }

        public int SaveTempChallanChargeDetailII(ChallanViewModel model)
        {
            int flag = ReturnType.None;
            List<ChallanViewModel> dataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempChallanModelII"];
            if (dataList == null || dataList.Count == 0)
            {
                List<ChallanViewModel> modelList = new List<ChallanViewModel>();
                model.Id = 1;
                modelList.Add(model);
                HttpContext.Current.Session["TempChallanModelII"] = modelList;
                flag = ReturnType.Saved;
            }
            else
            {
                int id = dataList.Count;
                model.Id = id + 1;
                dataList.Add(model);
                HttpContext.Current.Session["TempChallanModelII"] = dataList;
                return flag = ReturnType.Saved;
            }
            return flag;
        }

        public int RemoveTempChallanChargeDetail(ChallanViewModel model)
        {
            var flag = ReturnType.None;
            List<ChallanViewModel> dataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempChallanModel"];
            if (dataList != null)
            {
                if (dataList.Count > 0)
                {
                    var rmodel = dataList.Where(x => x.RegistrationId == model.RegistrationId && x.AccountHead == model.AccountHead && x.AccountSubHead == model.AccountSubHead && x.Amount == model.Amount).FirstOrDefault();
                    dataList.Remove(rmodel);
                    flag = ReturnType.Removed;
                }
            }
            return flag;
        }

        public int RemoveTempChallanChargeDetailII(ChallanViewModel model)
        {
            var flag = ReturnType.None;
            List<ChallanViewModel> dataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempChallanModelII"];
            if (dataList != null)
            {
                if (dataList.Count > 0)
                {
                    var rmodel = dataList.Where(x => x.AccountHead == model.AccountHead && x.AccountSubHead == model.AccountSubHead && x.Amount == model.Amount).FirstOrDefault();
                    dataList.Remove(rmodel);
                    flag = ReturnType.Removed;
                }
            }
            return flag;
        }

        public DataSourceResult GetTempChallanChargesAsDataSource(DataSourceRequest request, ChallanViewModel model)
        {
            List<ChallanViewModel> tempdata = (List<ChallanViewModel>)HttpContext.Current.Session["TempChallanModel"];
            //if (tempdata == null || tempdata.Count == 0)
            //{
            //    return null;
            //}
            //else
            //{
            //    if (tempdata.FirstOrDefault().RegistrationId == model.RegistrationId)
            //    {
            //        return tempdata.ToDataSourceResult(request);
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}
            return tempdata != null ? tempdata.ToDataSourceResult(request) : null;
        }

        public DataSourceResult GetTempChallanChargesAsDataSourceII(DataSourceRequest request, ChallanViewModel model)
        {
            List<ChallanViewModel> tempdata = (List<ChallanViewModel>)HttpContext.Current.Session["TempChallanModelII"];
            if (tempdata != null && tempdata.Count > 0)
            {
                for (int id = 0; id < tempdata.Count; id++) tempdata[id].Id = id;
            }
            return tempdata != null ? tempdata.ToDataSourceResult(request) : null;
        }


        public PropertyDetailViewModel GetAllottedPropertyDetailByRegistrationId(int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from alotment in dbContext.AllotmentMasters
                              join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                              join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                              where alotment.rid == rid
                              select new PropertyDetailViewModel
                              {
                                  RegistrationId = alotment.rid,
                                  PropertyId = alotment.propertyId,
                                  SchemeId = alotment.schemeId,
                                  SchemeName = alotment.SchemeMst.schemeName,
                                  DepartmentId = alotment.departmentId,
                                  Department = alotment.DepartmentMst.departmentName,
                                  SectorId = property.sectorId,
                                  Sector = property.sectorId == null ? string.Empty : property.SectorMst.sectorName,
                                  BlockId = property.blockId,
                                  Block = property.blockId == null ? string.Empty : property.BlockMst.blockName,
                                  PlotNo = property.propertyNo,
                                  Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                  ApplicantType = aplicant.tGender,
                                  ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                  CorrespondAddress = aplicant.tCorrespondanceAdd,
                                  PermanentAddress = aplicant.tPermanentAdd,
                                  MobileNo = aplicant.tMobileNumber,
                                  Email = aplicant.tEmail,
                                  PropertyTypeId = property.propertyTypeId,
                                  PropertyType = property.PropertyTypeMst.propertyTypeName,
                                  TotalArea = property.totalArea,
                                  FloorArea = property.FloorMst.floorName,
                                  PropertyNo = property.SectorMst.sectorName + "/" + property.blockId == null ? "NA" : property.BlockMst.blockName + "-" + property.propertyNo,
                                  AllotmentDate = alotment.allotmentDate,
                                  RegistryDate = dbContext.RegistryDetails.FirstOrDefault(r => r.Rid == rid).RegistryDoneDate
                              }).FirstOrDefault();
                return detail;
            }
        }

        public DataSourceResult GetReceiptHeadListAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from receipt in dbContext.RECIEPT_HEAD
                            where receipt.STATUS == 1
                            select new DropdownViewModel
                            {
                                Text = receipt.RECIEPT_HEAD_NAME,
                                Id = receipt.RECIEPT_CODE
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetReceiptSubHeadListAsDataSource(DataSourceRequest request, int? id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from receipt in dbContext.RECEIPT_SUB_HEAD
                            where receipt.RECEIPT_CODE == id && receipt.STATUS == 1
                            select new DropdownViewModel
                            {
                                Text = receipt.RECEIPT_SUB_HEAD1,
                                Id = receipt.RECEIPT_SUBHEAD_ID
                            });
                //request.Filters.RemoveAt(0);
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetLetterTemplateListByDepartmentAsDataSource(DataSourceRequest request, int? departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var letters = (from template in dbContext.TemplateMasters
                               where template.departmentId == departmentId // && template.templateId == templateId
                               select new DropdownViewModel
                               {
                                   Id = template.templateId.Value,
                                   Text = template.templateName,
                                   DepartmentId = (int)template.departmentId
                               });
                return letters.ToDataSourceResult(request);
            }
        }


        public LetterViewModel GenerateAuthorizedLetterById(LetterViewModel model)
        {
            string strContent = string.Empty;
            if (userInfo != null)
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    var flag = dbContext.usp_getServiceCheck(model.RegistrationId, model.TemplateId).FirstOrDefault();
                    if (flag.Equals(1))
                    {
                        ObjectParameter commaString = new ObjectParameter("CommaString", typeof(string));
                        dbContext.Sp_LatterPrintTemp(model.RegistrationId.ToString(), model.TemplateId, model.DepartmentId, userInfo.UserID.ToString(), model.LetterDate.Value.Date, commaString);
                        //strContent = commaString.Value.ToString();
                        model.LetterContent = commaString.Value.ToString();
                    }
                    else
                    {
                        model.LetterContent = flag.ToString();
                    }
                }
            }
            return model;
        }


        public LetterViewModel GetGeneratedLetterByBarcode(LetterViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                model.LetterContent = dbContext.Letter_History.FirstOrDefault(l => l.Barcode_Val == model.BarcodeValue).Template_Html;
            }
            return model;
        }


        public int SendRegisteredMessageToApplicant(ApplicantViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == Constants.Individual)
                {
                    ApplicationHelper.SMSSend(model.MobileNo, model.MessageContent);
                    ApplicationHelper.SendEmail(model.Email, "Noida Authority", model.MessageContent);
                    flag = ReturnType.Success;
                }
                else
                {
                    var formList = (from aplication in dbContext.OnlineApplicationDetails
                                    where aplication.schemeId == 119119 && aplication.departmentId == 4 && aplication.Photographfilename != "1"
                                    select aplication.onlineapplicationId).ToList();
                    if (formList != null)
                    {
                        foreach (var id in formList)
                        {
                            var form = dbContext.OnlineApplicationDetails.Where(c => c.onlineapplicationId == id).FirstOrDefault();
                            if (form != null)
                            {
                                if (!string.IsNullOrEmpty(form.email)) { ApplicationHelper.SendEmail(form.email, "Online Scheme", model.MessageContent); }
                                if (!string.IsNullOrEmpty(form.mobileNumberP2)) { ApplicationHelper.SendSMS(form.mobileNumberP2, model.MessageContent); }
                                if (!string.IsNullOrEmpty(form.mobileNumberP2)) { ApplicationHelper.SendSMS(form.mobileNumberP2, model.MessageContent); }

                                var audit = new Audit();
                                audit.TableName = "OnlineApplicationDetail";
                                audit.Type = "T";
                                audit.FormName = form.onlineapplicationId.ToString();
                                audit.PrimaryKeyField = "Online Scheme";
                                audit.PrimaryKeyValue = model.MessageContent;
                                audit.FieldName = form.mobileNumberP2;
                                audit.OldValue = form.mobileNumberP2;
                                audit.NewValue = model.MessageContent;
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


        public DataSourceResult GetDepartmentListAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from dept in dbContext.DepartmentMsts
                            where dept.IsActive == true
                            select new DropdownViewModel
                            {
                                Id = dept.departmentId,
                                Text = dept.departmentName
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetDepartmentListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.FilterType == "Property")
                {
                    List<int> LandTypeList = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
                    var list = (from dept in dbContext.DepartmentMsts
                                where dept.IsActive == true && LandTypeList.Contains(dept.departmentId)
                                select new DropdownViewModel
                                {
                                    Id = dept.departmentId,
                                    Text = dept.departmentName
                                });
                    return list.ToDataSourceResult(request);
                }
                else if (model.FilterType == "UsersDepartment")
                {
                    var list = (from dept in dbContext.DepartmentMsts
                                where dept.IsActive == true && DepartmentList.Contains(dept.departmentId)
                                select new DropdownViewModel
                                {
                                    Id = dept.departmentId,
                                    Text = dept.departmentName
                                });
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    var list = (from dept in dbContext.DepartmentMsts
                                where dept.IsActive == true
                                select new DropdownViewModel
                                {
                                    Id = dept.departmentId,
                                    Text = dept.departmentName
                                });
                    return list.ToDataSourceResult(request);
                }
            }
        }


        public DataSourceResult GetNotingFileTypeListAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Category == Constants.Noting && config.Is_Active == 1
                            select new DropdownViewModel
                            {
                                Id = config.Id,
                                Text = config.Name,
                                Value = config.Name
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetSchemeListAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from scheme in dbContext.SchemeMsts
                            join depart in dbContext.SchemeDepartmentTrans on scheme.schemeId equals depart.schemeId
                            where scheme.IsActive == true && scheme.completed == true && scheme.Status != Constants.SchemeClosed && depart.IsActive == true
                            && DepartmentList.Contains(depart.departmentId)
                            select new DropdownViewModel
                            {
                                Id = scheme.schemeId,
                                Text = scheme.schemeName
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetApplicationFormListAsDataSource(DataSourceRequest request, int? schemeId, int? departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var AllottedProperty = dbContext.AllotmentMasters.Where(a => a.schemeId == schemeId && a.departmentId == departmentId && a.isActive == Constants.Active).Select(x => x.rid).ToList();
                var applist = (from aplication in dbContext.ApplicationDetails
                               join transction in dbContext.ApplicationPaymentDetails on aplication.applicationId equals transction.applicationId
                               where aplication.schemeId == schemeId && aplication.departmentId == departmentId
                               && (aplication.isAllotted == null || aplication.isAllotted != "1")
                               //&& DepartmentList.Contains(aplication.departmentId) //&& !AllottedProperty.Contains(aplication.registrationId.Value)
                               select new DropdownViewModel
                               {
                                   Id = aplication.applicationId,
                                   Text = aplication.applicationId.ToString(),
                                   Value = aplication.formNo,
                                   SchemeId = schemeId,
                                   DepartmentId = departmentId
                               });
                //request.Filters.RemoveAt(0);
                return applist.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetAllottedPropertyIdListAsDataSource(DataSourceRequest request, int? schemeId, int? departmentId, int? applicationId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from alotment in dbContext.AllotmentMasters
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where property.schemeId == schemeId && property.departmentId == departmentId
                               && (applicationId == null || alotment.applicationId == applicationId)
                               && property.IsActive == true && alotment.isActive == Constants.Active
                               && DepartmentList.Contains(property.departmentId)
                            select new DropdownViewModel
                            {
                                Id = property.propertyId.Value,
                                Text = property.SectorMst.sectorName + "/" + (property.blockId == null ? "" : property.BlockMst.blockName + "-") + property.propertyNo,
                                SchemeId = schemeId,
                                DepartmentId = departmentId
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetUserStatusListAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Category == "User Status" && config.Is_Active == 1
                            select new DropdownViewModel
                            {
                                Id = config.Range.Value,
                                Text = config.Name,
                                IsActive = (config.Range == 0 || config.Range == null) ? false : true
                            });

                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetSectorListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from property in dbContext.SchemePropTrans
                            //join alotment in dbContext.AllotmentMasters on property.propertyId equals alotment.propertyId
                            join sector in dbContext.SectorMsts on property.sectorId equals sector.sectorId
                            where property.IsActive == true && (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                            select new DropdownViewModel
                            {
                                Id = sector.sectorId,
                                Text = sector.sectorName,
                                DepartmentId = model.DepartmentId
                            }).Distinct();
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetBlockListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from property in dbContext.SchemePropTrans
                            from block in dbContext.BlockMsts.Where(b => b.blockId == property.blockId).DefaultIfEmpty()
                            where property.IsActive == true
                            && (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                            && property.sectorId == model.SectorId
                            select new DropdownViewModel
                            {
                                Id = block.blockId,
                                Text = block.blockName,
                                DepartmentId = model.DepartmentId,
                                SectorId = model.SectorId
                            }).Distinct();
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetSectorListAsDataSourceII(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from sector in dbContext.SectorMsts
                            select new DropdownViewModel
                            {
                                Id = sector.sectorId,
                                Text = sector.sectorName
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetBlockListAsDataSourceII(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from block in dbContext.BlockMsts
                            select new DropdownViewModel
                            {
                                Id = block.blockId,
                                Text = block.blockName
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetPlotListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from property in dbContext.SchemePropTrans
                            where property.IsActive == true
                            && (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                            && (model.SectorId == null || property.sectorId == model.SectorId)
                            && (model.BlockId == null || property.blockId == model.BlockId)
                            select new DropdownViewModel
                            {
                                Value = property.propertyNo,
                                Text = property.propertyNo
                            }).Distinct().ToList();
                return list.ToDataSourceResult(request);
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
                                    Text = scheme.schemeName
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
                else if (model.ActionType == "ApplicationForm")
                {
                    var list = (from forms in dbContext.ApplicationDetails
                                where (model.SchemeId == null || forms.schemeId == model.SchemeId)
                                && (model.DepartmentId == null || forms.departmentId == model.DepartmentId)
                                select new DropdownViewModel
                                {
                                    Id = forms.applicationId == null ? 0 : forms.applicationId,
                                    Text = forms.formNo,
                                    SchemeId = model.SchemeId,
                                    DepartmentId = model.DepartmentId
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }
                else if (model.ActionType == "RegistrationId")
                {
                    var list = (from alotment in dbContext.AllotmentMasters
                                from property in dbContext.SchemePropTrans.Where(p => p.propertyId == alotment.propertyId).DefaultIfEmpty()
                                where (model.SchemeId == null || alotment.schemeId == model.SchemeId)
                                && (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                                && (model.RegistrationId == null || alotment.rid == model.RegistrationId)
                                && (model.SectorId == null || property.sectorId == model.SectorId)
                                && (model.BlockId == null || property.blockId == model.BlockId)
                                && (model.PlotNo == null || property.propertyNo == model.PlotNo)
                                select new DropdownViewModel
                                {
                                    Id = alotment.rid,
                                    Text = alotment.rid.ToString(),
                                    SchemeId = model.SchemeId,
                                    DepartmentId = model.DepartmentId,
                                    SectorId = model.SectorId,
                                    BlockId = model.BlockId,
                                    Value = model.PlotNo
                                });
                    return list.ToDataSourceResult(request);
                }
                else if (model.ActionType == "Plot")
                {
                    var list = (from property in dbContext.SchemePropTrans
                                where property.IsActive == true //&& property.sectorId == model.SectorId
                                && (model.SchemeId == null || property.schemeId == model.SchemeId)
                                && (model.DepartmentId == null || property.departmentId == model.DepartmentId)
                                && (model.SectorId == null || property.sectorId == model.SectorId)
                                && (model.BlockId == null || property.blockId == model.BlockId)
                                select new DropdownViewModel
                                {
                                    Id = property.refId,
                                    Text = property.propertyNo,
                                    Value = property.propertyNo,
                                    SchemeId = model.SchemeId,
                                    DepartmentId = model.DepartmentId,
                                    SectorId = model.SectorId,
                                    BlockId = model.BlockId
                                });
                    return list.ToDataSourceResult(request);
                }
                else if (model.ActionType == "PropertyCost")
                {
                    var list = (from cost in dbContext.SchemeCostTrans
                                from propertyType in dbContext.PropertyTypeMsts.Where(b => b.propertyTypeId == cost.propertyTypeId).DefaultIfEmpty()
                                where cost.IsActive == true
                                && (model.SchemeId == null || cost.schemeId == model.SchemeId)
                                && (model.DepartmentId == null || cost.departmentId == model.DepartmentId)
                                && (model.SectorId == null || cost.sectorId == model.SectorId)
                                && (model.PropertyTypeId == null || cost.propertyTypeId == model.PropertyTypeId)
                                select new DropdownViewModel
                                {
                                    Id = cost.refId,
                                    Text = cost.refId.ToString(),
                                    SchemeId = model.SchemeId,
                                    DepartmentId = model.DepartmentId,
                                    SectorId = model.SectorId,
                                    PropertTypeId = model.PropertyTypeId
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    return null;
                }
            }
        }

        public DataSourceResult GetSecurityQuestioinListAsDataSource(DataSourceRequest request)
        {
            List<DropdownViewModel> messageList = new List<DropdownViewModel>();
            ResourceSet resourceSet = SecurityQuestion.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                var message = new DropdownViewModel();
                message.Text = entry.Key.ToString();
                message.Value = (string)entry.Value;
                messageList.Add(message);
            }
            return messageList.ToDataSourceResult(request);
        }


        public DataSourceResult GetBankAccountDetailAsDataSource(DataSourceRequest request, BankAccountViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.FilterType == "Bank")
                {
                    var list = (from bank in dbContext.BankMsts
                                select new DropdownViewModel
                                {
                                    Id = bank.bankId,
                                    Text = bank.bankName
                                });
                    return list.ToDataSourceResult(request);
                }
                else if (model.FilterType == "Branch")
                {
                    var list = (from branch in dbContext.BranchMsts
                                where branch.bankId == model.BankId
                                select new DropdownViewModel
                                {
                                    Id = branch.branchId,
                                    Text = branch.branchName
                                });
                    request.Filters.RemoveAt(0);
                    return list.ToDataSourceResult(request);
                }
                else if (model.FilterType == "AccountNo")
                {
                    var list = (from acno in dbContext.BranchMsts
                                where acno.bankId == model.BankId && acno.branchId == model.BranchId
                                select new DropdownViewModel
                                {
                                    Id = acno.branchId,
                                    Text = acno.accountNumber
                                });
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    return null;
                }
            }
        }

        public int SaveBankAccountDetail(BankAccountViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "AddBank")
                {
                    BankMst bank = new BankMst();
                    bank.bankName = model.BankName;
                    bank.createdBy = userInfo.UserID.ToString();
                    bank.createdDate = DateTime.Now;
                    bank.IsActive = true;
                    dbContext.BankMsts.Add(bank);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                else if (model.ActionType == "UpdateBank")
                {
                    var bank = dbContext.BankMsts.FirstOrDefault(b => b.bankId == model.BankId);
                    bank.bankName = model.BankName;
                    //bank.IsActive = true;
                    bank.modifiedBy = userInfo.UserID.ToString();
                    bank.modifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                else if (model.ActionType == "AddBranch")
                {
                    var branch = new BranchMst();
                    branch.bankId = model.BankId;
                    branch.branchName = model.BranchAddress;
                    //branch.IsActive = true;
                    branch.createdBy = userInfo.UserID.ToString();
                    branch.createdDate = DateTime.Now;
                    dbContext.BranchMsts.Add(branch);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                else if (model.ActionType == "UpdateBranch")
                {
                    var branch = dbContext.BranchMsts.FirstOrDefault(b => b.branchId == model.BranchId);
                    branch.bankId = model.BankId;
                    branch.branchName = model.BranchAddress;
                    //branch.IsActive = true;
                    branch.modifiedBy = userInfo.UserID.ToString();
                    branch.modifiedDate = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                else if (model.ActionType == "UpdateAccountNo")
                {
                    var branch = dbContext.BranchMsts.FirstOrDefault(b => b.bankId == model.BankId && b.branchId == model.BranchId);
                    branch.accountNumber = model.AccountNo;
                    //branch.IsActive = true;
                    branch.modifiedDate = DateTime.Now;
                    branch.modifiedBy = userInfo.UserID.ToString();
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                else if (model.ActionType == "UpdateIFSCCode")
                {
                    var branch = dbContext.BranchMsts.FirstOrDefault(b => b.bankId == model.BankId && b.branchId == model.BranchId);
                    branch.IFSCcode = model.IFSCCode;
                    //branch.IsActive = true;
                    branch.modifiedDate = DateTime.Now;
                    branch.modifiedBy = userInfo.UserID.ToString();
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }

        public int SaveServiceDetailById(ServiceViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "AddService")
                {
                    var service = new CitizenService_Master();
                    service.Deptt_Id = model.DepartmentId;
                    service.ServiceName = model.ServiceName;
                    service.Status = Constants.Active;
                    dbContext.CitizenService_Master.Add(service);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                else if (model.ActionType == "UpdateService")
                {
                    var service = dbContext.CitizenService_Master.FirstOrDefault(c => c.Deptt_Id == model.DepartmentId && c.service_id == model.ServiceId);
                    service.ServiceName = model.ServiceName;
                    service.Status = Constants.Active;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }

        public BankAccountViewModel GetBankAccountDetailById(BankAccountViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.FilterType == "Bank")
                {
                    var data = (from bank in dbContext.BankMsts
                                where bank.bankId == model.BankId
                                select new BankAccountViewModel
                                {
                                    FilterType = "Bank",
                                    BankId = bank.bankId,
                                    BankName = bank.bankName
                                }).FirstOrDefault();
                    return data;
                }
                else if (model.FilterType == "Branch")
                {
                    var data = (from branch in dbContext.BranchMsts
                                where branch.bankId == model.BankId && branch.branchId == model.BranchId
                                select new BankAccountViewModel
                                {
                                    FilterType = "Branch",
                                    BankId = branch.bankId,
                                    BranchId = branch.branchId,
                                    BranchAddress = branch.branchName,
                                    AccountNo = branch.accountNumber,
                                    IFSCCode = branch.IFSCcode
                                }).FirstOrDefault();
                    return data;
                }
                else if (model.FilterType == "AccountNo")
                {
                    var data = (from branch in dbContext.BranchMsts
                                where branch.bankId == model.BankId && branch.branchId == model.BranchId
                                select new BankAccountViewModel
                                {
                                    FilterType = "AccountNo",
                                    BankId = branch.bankId,
                                    BranchId = branch.branchId,
                                    BranchAddress = branch.branchName,
                                    AccountNo = branch.accountNumber,
                                    IFSCCode = branch.IFSCcode
                                }).FirstOrDefault();
                    return data;
                }
                else
                {
                    return null;
                }
            }
        }

        public ServiceViewModel GetServiceDetailById(ServiceViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from service in dbContext.CitizenService_Master
                            where service.Deptt_Id == model.DepartmentId && service.service_id == model.ServiceId
                            select new ServiceViewModel
                            {
                                Id = service.service_id,
                                ServiceId = service.service_id,
                                ServiceName = service.ServiceName,
                                DepartmentId = service.Deptt_Id
                            }).FirstOrDefault();
                return data;
            }
        }


        public int SavePropertyServiceType(ServiceViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "AddService")
                {
                    var service = new CitizenService_Master();
                    service.Deptt_Id = model.DepartmentId;
                    service.ServiceName = model.ServiceName.ToUpper();
                    service.Status = Constants.Active;
                    dbContext.CitizenService_Master.Add(service);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                else if (model.ActionType == "UpdateService")
                {
                    var service = dbContext.CitizenService_Master.FirstOrDefault(r => r.Deptt_Id == model.DepartmentId && r.service_id == model.ServiceId);
                    service.ServiceName = model.ServiceName.ToUpper();
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }


        public DataSourceResult GetPropertyTypeByDepartmentAsDataSource(DataSourceRequest request, int departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from property in dbContext.PropertyTypeMsts
                            where property.IsActive == true && property.departmentId == departmentId
                            select new DropdownViewModel
                            {
                                Text = property.propertyTypeName,
                                Id = property.propertyTypeId
                            });
                request.Filters.RemoveAt(0);
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetDefaultSectorListAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from sector in dbContext.SectorMsts
                            where sector.IsActive == true
                            select new DropdownViewModel
                            {
                                Id = sector.sectorId,
                                Text = sector.sectorName
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetDefaultBlockListAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from block in dbContext.BlockMsts
                            where block.IsActive == true
                            select new DropdownViewModel
                            {
                                Id = block.blockId,
                                Text = block.blockName
                            });
                return list.ToDataSourceResult(request);
            }
        }
        public DataSourceResult GetApplicationIdListAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from application in dbContext.ApplicationDetails
                            where application.isAllotted == "1"
                            select new DropdownViewModel
                            {
                                Id = application.applicationId,
                                Text = application.formNo
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetAllotteeIdTypeList(DataSourceRequest request, DropdownViewModel model)
        {
            List<DropdownViewModel> list = new List<DropdownViewModel>();
            if (model.ActionType != Constants.Company)
            {
                list.Add(new DropdownViewModel { Text = "PAN Card", Value = "PANCard" });
                list.Add(new DropdownViewModel { Text = "Aadhar Card", Value = "AadharCard" });
                list.Add(new DropdownViewModel { Text = "Voter Id Card", Value = "VoterIdCard" });
                list.Add(new DropdownViewModel { Text = "Passport", Value = "Passport" });
                list.Add(new DropdownViewModel { Text = "Driving Licence", Value = "DrivingLicence" });
            }
            else
            {
                list.Add(new DropdownViewModel { Text = "Company PAN Card", Value = "PANCard" });
                list.Add(new DropdownViewModel { Text = "GST Registration No", Value = "RegistrationNo" });
                list.Add(new DropdownViewModel { Text = "ROC Certificate", Value = "ROC" });
            }
            return list.ToDataSourceResult(request);
        }

        public DataSourceResult GetOwnershipFileTypeList(DataSourceRequest request, DropdownViewModel model)
        {
            List<DropdownViewModel> list = new List<DropdownViewModel>();
            if (model.ActionType != Constants.Company)
            {
                list.Add(new DropdownViewModel { Text = "Allotment Letter", Value = "AllotmentLetter" });
                list.Add(new DropdownViewModel { Text = "Transfer Letter", Value = "TransferLetter" });
                list.Add(new DropdownViewModel { Text = "Mutation Letter", Value = "MutationLetter" });
            }
            else
            {
                list.Add(new DropdownViewModel { Text = "Allotment Letter", Value = "AllotmentLetter" });
                list.Add(new DropdownViewModel { Text = "Transfer Letter", Value = "TransferLetter" });
                list.Add(new DropdownViewModel { Text = "CIC Letter", Value = "CICLetter" });
            }
            return list.ToDataSourceResult(request);
        }


        public int SendOTP(CommonViewModel model)
        {
            int flag = ReturnType.None;
            if (model.ActionType == "KYA")
            {
                flag = ApplicationHelper.GenerateOTP();
                //flag = 123;
                HttpContext.Current.Session["OTPmobile"] = flag;
                string message = string.Format(NAMessages.KYAOTP, flag);
                //string message = string.Format(NAMessages.OnlineApplicationOTP, flag);
                if (!string.IsNullOrEmpty(model.MobileNo)) ApplicationHelper.SendSMS(model.MobileNo, message);
                if (!string.IsNullOrEmpty(model.Email)) ApplicationHelper.SendEmail(model.Email, "KYA Form", message);
            }
            return flag;
        }

        public int ValidateOTP(CommonViewModel model)
        {
            int flag = 0;
            if ((int)HttpContext.Current.Session["OTPmobile"] == Convert.ToInt32(model.OTP)) flag = ReturnType.Success;
            //if ((string)Session["OTPmobile"] == (otpMobile)) { flag = ReturnType.Success; }
            else { flag = ReturnType.Failure; }

            return flag;
        }


        public PropertyViewModel GetPropertyDetailById(PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == Constants.Registered)
                {
                    model = GetPropertyDetailByRegistrationId(model.RegistrationId);
                }
                else if (model.ActionType == Constants.UnRegistered)
                {
                    model = GetPropertyDetailByPropertyNo(model);
                }
            }
            return model;
        }

        private PropertyViewModel GetPropertyDetailByPropertyNo(PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from property in dbContext.SchemePropTrans
                              from alotment in dbContext.AllotmentMasters.Where(a => a.propertyId == property.propertyId).DefaultIfEmpty()
                              from aplicant in dbContext.ApplicationDetails.Where(r => r.registrationId == alotment.rid).DefaultIfEmpty()
                              where property.sectorId == model.SectorId
                              && (model.BlockId == null || property.blockId == model.BlockId)
                              && property.propertyNo == model.PlotNo
                              select new PropertyViewModel
                              {
                                  RegistrationId = alotment.rid,
                                  PropertyId = alotment.propertyId,
                                  SchemeId = alotment.schemeId,
                                  SchemeName = alotment.SchemeMst.schemeName,
                                  DepartmentId = alotment.departmentId,
                                  Department = alotment.DepartmentMst.departmentName,
                                  SectorId = property.sectorId,
                                  SectorName = property.sectorId == null ? string.Empty : property.SectorMst.sectorName,
                                  BlockId = property.blockId,
                                  BlockName = property.blockId == null ? string.Empty : property.BlockMst.blockName,
                                  PlotNo = property.propertyNo,
                                  Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                  ApplicantType = aplicant.tGender,
                                  ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                  CorrespondAddress = aplicant.tCorrespondanceAdd,
                                  PermanentAddress = aplicant.tPermanentAdd,
                                  MobileNo = aplicant.tMobileNumber,
                                  Email = aplicant.tEmail,
                                  PAN = aplicant.tPan,
                                  GSTNo = aplicant.tGSTNo,
                                  PropertyTypeId = property.propertyTypeId,
                                  PropertyType = property.PropertyTypeMst.propertyTypeName,
                                  TotalArea = property.totalArea,
                                  FloorArea = property.FloorMst.floorName,
                                  PropertyNo = property.SectorMst.sectorName + "/" + property.blockId == null ? "NA" : property.BlockMst.blockName + "-" + property.propertyNo,
                                  AllotmentDate = alotment.allotmentDate,
                                  RegistryDate = dbContext.RegistryDetails.FirstOrDefault(r => r.Rid == model.RegistrationId).RegistryDoneDate
                              }).FirstOrDefault();
                return detail;
            }
        }

        private PropertyViewModel GetPropertyDetailByRegistrationId(int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var detail = (from alotment in dbContext.AllotmentMasters
                              join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                              join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                              where alotment.rid == rid
                              select new PropertyViewModel
                              {
                                  RegistrationId = alotment.rid,
                                  PropertyId = alotment.propertyId,
                                  SchemeId = alotment.schemeId,
                                  SchemeName = alotment.SchemeMst.schemeName,
                                  DepartmentId = alotment.departmentId,
                                  Department = alotment.DepartmentMst.departmentName,
                                  SectorId = property.sectorId,
                                  SectorName = property.sectorId == null ? string.Empty : property.SectorMst.sectorName,
                                  BlockId = property.blockId,
                                  BlockName = property.blockId == null ? string.Empty : property.BlockMst.blockName,
                                  PlotNo = property.propertyNo,
                                  //PlotUnitNo = property.
                                  Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                  ApplicantType = aplicant.tGender,
                                  ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                  CorrespondAddress = aplicant.tCorrespondanceAdd,
                                  PermanentAddress = aplicant.tPermanentAdd,
                                  ApplicantAddressI = aplicant.tCorrespondanceAdd,
                                  ApplicantAddressII = aplicant.tCorrespondanceAdd,
                                  MobileNo = aplicant.tMobileNumber,
                                  Email = aplicant.tEmail,
                                  PAN = aplicant.tPan,
                                  GSTNo = aplicant.tGSTNo,
                                  PropertyTypeId = property.propertyTypeId,
                                  PropertyType = property.PropertyTypeMst.propertyTypeName,
                                  TotalArea = property.totalArea,
                                  FloorArea = property.FloorMst.floorName,
                                  LandRate = property.landRatePerSqmt,
                                  TotalAllotmentRate = property.TotalAllotmentRate,
                                  PropertyNo = property.SectorMst.sectorName + "/" + property.blockId == null ? "NA" : property.BlockMst.blockName + "-" + property.propertyNo,
                                  AllotmentDate = alotment.allotmentDate,
                                  AllotmentMoney = property.allotmentMoney,
                                  PropertyCost = property.propertyCost,
                                  TotalPropertyCost = property.totalPropertyCost,
                                  RegistryDate = dbContext.RegistryDetails.FirstOrDefault(r => r.Rid == rid).RegistryDoneDate
                              }).FirstOrDefault();
                if (detail != null)
                {
                    var functional = dbContext.FunctionalDetails.Where(f => f.Rid == detail.RegistrationId).ToList().OrderByDescending(o => o.RequestNo).FirstOrDefault();
                    detail.IsPropertyFunctional = functional != null ? true : false;
                    var mortgage = dbContext.MortgageDetails.Where(m => m.RID == detail.RegistrationId).ToList().OrderByDescending(o => o.RequestNo).FirstOrDefault();
                    detail.IsMortgaged = mortgage != null ? true : false;
                    var leaserent = dbContext.LeaseRentPayments.FirstOrDefault(l => l.RegistrationId == detail.RegistrationId && l.IsOneTimeLeasePaid == true);
                    detail.IsOneTimeLeasePaid = leaserent != null ? true : false;
                    detail.IsTotalPremiumPaid = false;
                }
                return detail;
            }
        }


        public DataSourceResult GetOptionalReasonAsDataSource(DataSourceRequest request)
        {
            List<DropdownViewModel> dataList = new List<DropdownViewModel>();
            dataList.Add(new DropdownViewModel { Id = 1, Text = "Mismatch from file Information" });
            dataList.Add(new DropdownViewModel { Id = 2, Text = "Id/Letter attached document not clear" });
            dataList.Add(new DropdownViewModel { Id = 3, Text = "Incorret document attached" });
            dataList.Add(new DropdownViewModel { Id = 4, Text = "Others" });
            return dataList.ToDataSourceResult(request);
        }


        public int CheckChallanSessionDataById(ChallanViewModel model)
        {
            int flag = ReturnType.None;
            List<ChallanViewModel> tempdata = (List<ChallanViewModel>)HttpContext.Current.Session["TempChallanModel"];
            if (tempdata == null || tempdata.Count == 0)
            {
                return flag;
            }
            else
            {
                if (tempdata.FirstOrDefault().RegistrationId == model.RegistrationId)
                {
                    return flag = ReturnType.Exist;
                }
                else
                {
                    return flag = ReturnType.NotExist;
                }
            }
        }


        public List<DropdownViewModel> GetVillageIdList()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from vill in dbContext.Village_Master
                           where vill.Is_Active == true
                           select new DropdownViewModel
                           {
                               Id = vill.Id,
                               Text = vill.Village_Name
                           }).ToList();
                return lst;
            }
        }



        public int VerifyChallanDetailById(ChallanViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var challan = dbContext.Challan_Master.Where(c => c.Id == model.Id).FirstOrDefault();
                if (model.ActionType == Constants.Verify)
                {
                    if (challan.Is_Verified == true)
                    {
                        flag = ReturnType.Exist;
                    }
                    else if (challan.Is_Active == false)
                    {
                        flag = ReturnType.Cancelled;
                    }
                    else
                    {
                        challan.Is_Verified = true;
                        dbContext.SaveChanges();
                        flag = ReturnType.Updated;
                    }
                }
                else if (model.ActionType == Constants.Cancel)
                {
                    if (challan.Is_Verified == true)
                    {
                        flag = ReturnType.Verified;
                    }
                    else
                    {
                        challan.Is_Active = false;
                        dbContext.SaveChanges();
                        flag = ReturnType.Cancelled;
                    }
                }
                return flag;
            }
        }


        public LetterViewModel GenerateNDCByRegistrationId(LetterViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                model.DepartmentId = dbContext.AllotmentMasters.FirstOrDefault(r => r.rid == model.RegistrationId).departmentId;
                //model.LetterId = dbContext.TemplateMasters.FirstOrDefault(r => r.departmentId == model.DepartmentId && r.templateId == Constants.NDCTemplateId).id;
                string letterContent = string.Empty;
                ObjectParameter commaString = new ObjectParameter("CommaString", typeof(string));
                dbContext.Sp_LatterPrintTemp(model.RegistrationId.ToString(), Constants.NDCTemplateId, model.DepartmentId, userInfo.UserID.ToString(), DateTime.Now.Date, commaString);
                letterContent = commaString.Value.ToString();
                model.LetterContent = commaString.Value.ToString();
                return model;
            }
        }


        public DataSourceResult GetOSDApproverIdList(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from user in dbContext.UmUserMasters
                            join dept in dbContext.UmUserDepartmentTrans on user.UserRefId equals dept.UserRefId
                            join role in dbContext.UmUserMasterRoles on user.UserRefId equals role.UserRefId
                            where user.IsActive == true && DepartmentList.Contains(dept.DepartmentId) && user.UserRefId != userInfo.UserID
                            && role.UmRoleMaster.RoleInDepartment == RoleInDepartment.OSD
                            select new DropdownViewModel
                            {
                                Id = user.UserRefId,
                                Text = user.UserName + "-" + user.FirstName
                            }).Distinct();
                return list.ToDataSourceResult(request);
            }
        }


        public int IsRegistrationIdExist(PropertyViewModel model)
        {
            int flag = ReturnType.NotExist;
            using (var dbContext = new NoidaPMSEntities())
            {
                var isRidExist = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == model.RegistrationId);
                if (isRidExist != null)
                {
                    flag = ReturnType.Exist;
                }
                else
                {
                    flag = ReturnType.NotExist;
                }
                return flag;
            }
        }


        public DropdownViewModel GetServiceTypeDetailById(DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var service = (from servc in dbContext.CitizenService_Master
                               where servc.Id == model.Id
                               select new DropdownViewModel
                               {
                                   Id = servc.Id,
                                   ServiceId = servc.service_id,
                                   Text = servc.ServiceName,
                                   Status = servc.Status == 1 ? "Active" : "InActive"
                               }).FirstOrDefault();
                return service;
            }
        }

        public DataSourceResult GetServiceTypeListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var service = (from servc in dbContext.CitizenService_Master
                               where servc.Deptt_Id == model.DepartmentId && (model.RefId == null || servc.Id == model.RefId)
                               select new DropdownViewModel
                               {
                                   Id = servc.Id,
                                   DepartmentId = model.DepartmentId,
                                   ServiceId = servc.service_id,
                                   Text = servc.ServiceName
                               });
                return service.ToDataSourceResult(request);
            }
        }

        // Get Dashboard [Property] data by Department IDs
        public List<DashboardPropertyVM> GetDashboardPropertyData(int departmentId)
        {
            var dashboardDetailsLst = new List<DashboardPropertyVM>();
            using (var dbContext = new NoidaPMSEntities())
            {
                dashboardDetailsLst = (from dashboardPropertyData in dbContext.Sp_DashboardPropertyData(departmentId).ToList()
                                       select new DashboardPropertyVM
                                       {
                                           DepartmentId = dashboardPropertyData.DepartmentId,
                                           DepartmentName = dashboardPropertyData.DepartmentName,
                                           FullPaid = dashboardPropertyData.FullPaid,
                                           AllottedProperty = dashboardPropertyData.AllottedProperty
                                       }).ToList();
                return dashboardDetailsLst;
            }
        }


        public KYAViewModel GetKYADetailsForRid(KYAViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var details = dbContext.KYADetails.FirstOrDefault(m => m.RId == model.RegistrationId);
                if (details != null)
                {
                    model.StatusId = details.StatusId;
                    model.Status = dbContext.StatusMasters.FirstOrDefault(m => m.Id == details.StatusId).Status;
                }
            }
            return model;
        }


        public CommonViewModel ActivateDetailsOnId(CommonViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "KYA")
                {
                    var kya = dbContext.KYADetails.FirstOrDefault(m => m.RId.ToString() == model.RegistrationId);
                    if (kya != null)
                    {
                        kya.IsActive = kya.IsActive == true ? false : true;
                        dbContext.SaveChanges();
                        model.ReturnTypeId = ReturnType.Updated;
                    }
                }
                if (model.ActionType == "Property")
                {
                    var property = dbContext.SchemePropTrans.FirstOrDefault(p => p.propertyId == model.PropertyId);
                    property.IsActive = property.IsActive == true ? false : true;
                    dbContext.SaveChanges();
                    model.ReturnTypeId = ReturnType.Updated;
                }
                if (model.ActionType == "Allotment")
                {
                    var allotment = dbContext.AllotmentMasters.FirstOrDefault(p => p.rid == model.RegistrationNo);
                    allotment.isActive = allotment.isActive == 1 ? 0 : 1;
                    dbContext.SaveChanges();
                    model.ReturnTypeId = ReturnType.Updated;
                }
            }
            return model;
        }


        public DataSourceResult GetRegistrationIdListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.FilterType == "Transfer")
                {
                    var list = (from possession in dbContext.PossessionDetails
                                join allotment in dbContext.AllotmentMasters on possession.Rid equals allotment.rid
                                where possession.IsActive == true && DepartmentList.Contains(allotment.departmentId) && allotment.isActive == 1 && possession.Possession == true &&
                                possession.Rid == model.RegistrationId
                                select new DropdownViewModel
                                {
                                    Id = possession.Rid.Value,
                                    Text = possession.Rid.ToString()
                                });
                    var distinctRIdList = list.GroupBy(x => x.Id).Select(y => new DropdownViewModel { Id = y.Select(i => i.Id).FirstOrDefault(), Text = y.Select(i => i.Text).FirstOrDefault() }).OrderByDescending(z => z.Id);
                    return distinctRIdList.ToDataSourceResult(request);
                }
                if (model.FilterType == "Restoration")
                {
                    var ridlist = (from can in dbContext.Property_Cancellation_Details
                                   join app in dbContext.ApplicationDetails on can.Rid equals app.registrationId
                                   where can.Is_Active == true && DepartmentList.Contains(app.departmentId) && can.Status == NAStatusId.Approved && (can.Type == Constants.PropertySurrenderId || can.Type == Constants.PropertyCancellationId)
                                   select new DropdownViewModel
                                   {
                                       Id = can.Rid.Value,
                                       Text = can.Rid.ToString()
                                   });
                    return ridlist.ToDataSourceResult(request);
                }
                else
                {
                    var ridlist = (from alotment in dbContext.AllotmentMasters
                                   where DepartmentList.Contains(alotment.departmentId) && alotment.isActive == 1
                                   select new DropdownViewModel
                                   {
                                       Id = alotment.rid,
                                       Text = alotment.rid.ToString()
                                   });
                    return ridlist.ToDataSourceResult(request);
                }

            }
        }


        public List<DropdownViewModel> GetAllotteeTypeList()
        {
            List<DropdownViewModel> genderList = new List<DropdownViewModel>();
            genderList.Add(new DropdownViewModel { Text = Constants.Male });
            genderList.Add(new DropdownViewModel { Text = Constants.Female });
            genderList.Add(new DropdownViewModel { Text = Constants.Company });
            return genderList;
        }


        public DataSourceResult GetFloorAreaListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var floorList = (from floor in dbContext.FloorMsts
                                 where floor.departmentId == model.DepartmentId
                                 select new DropdownViewModel
                                 {
                                     Id = floor.floorId,
                                     Text = floor.floorName
                                 });
                return floorList.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetLocationTypeListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from location in dbContext.LocationMsts
                            //where location.departmentId == model.DepartmentId
                            select new DropdownViewModel
                            {
                                Id = location.locationId,
                                Text = location.locationName
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetReceiptHeadIdListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from receipt in dbContext.RECEIPT_DETAIL_MASTER
                            where receipt.RID_NO == model.RegistrationId.ToString()
                            select new DropdownViewModel
                            {
                                Id = 0,
                                ReceiptId = receipt.RECEIPT_ID,
                                Text = receipt.RECEIPT_ID.ToString()
                            });
                request.Filters.RemoveAt(0);
                return list.ToDataSourceResult(request);
            }
        }

        #region User Details For Nic Nivesh Mitra

        public LoginUserDetail GetLoginUserDetails(int userId)
        {
            LoginUserDetail loginUserDetail = null;
            using (var dbContext = new NoidaPMSEntities())
            {
                loginUserDetail = (from user in dbContext.UmUserMasters
                                   where user.UserRefId == userId
                                   select new LoginUserDetail
                                   {
                                       UserRefId = user.UserRefId,
                                       UserName = user.UserName,
                                       FirstName = user.FirstName,
                                       LastName = user.LastName,
                                       MiddleName = user.MiddleName,
                                       IsActive = user.IsActive
                                   }).FirstOrDefault();
            }
            return loginUserDetail;
        }

        #endregion


        public int IsRequestIdIsExistTest(ServiceViewModel model)
        {
            int flag = ReturnType.NotExist;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.RequestId != 0 && model.RequestId != null)
                {
                    var requestDetail = dbContext.Customer_ServiceRequest.FirstOrDefault(m => m.Id == model.RequestId && m.Registration_No == model.RegistrationId.ToString() && (m.ServiceId == model.ServiceId || m.ServiceId == NAService.Other) && m.IsActive == true);//&& m.Request_Status != NAStatusId.Completed
                    if (requestDetail != null)
                    {
                        flag = ReturnType.Exist;
                    }
                    else
                    {
                        flag = ReturnType.NotExist;
                    }
                }
                else
                {
                    flag = ReturnType.Exist;
                }

                return flag;
            }
        }
        public int IsRequestIdIsExist(ServiceViewModel model)
        {
            int flag = ReturnType.NotExist;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.RequestId != 0 && model.RequestId != null)
                {
                    var requestDetail = dbContext.Customer_ServiceRequest.FirstOrDefault(m => m.Id == model.RequestId && m.Registration_No == model.RegistrationId.ToString() && (m.ServiceId == model.ServiceId || m.ServiceId == NAService.Other) && m.IsActive == true);//&& m.Request_Status != NAStatusId.Completed
                    if (requestDetail != null)
                    {
                        if (requestDetail.Request_Status == NAStatusId.Completed)
                        {
                            flag = NAStatusId.Completed;
                        }
                        else if (requestDetail.Request_Status == NAStatusId.Initiated)
                        {
                            flag = NAStatusId.Initiated;
                        }
                        else if (requestDetail.Request_Status == NAStatusId.Objection)
                        {
                            flag = NAStatusId.Objection;
                        }
                        else if (requestDetail.Request_Status == NAStatusId.Rejected)
                        {
                            flag = NAStatusId.Rejected;
                        }
                        else if (requestDetail.Request_Status == NAStatusId.Cancelled)
                        {
                            flag = NAStatusId.Cancelled;
                        }
                        else if (requestDetail.Request_Status == NAStatusId.Withdrawn)
                        {
                            flag = NAStatusId.Withdrawn;
                        }
                        else
                        {
                            flag = ReturnType.Exist;
                        }
                    }
                    else
                    {
                        flag = ReturnType.NotExist;
                    }
                }
                else
                {
                    flag = ReturnType.Exist;
                }

                return flag;
            }
        }

        public DataSourceResult GetSchemeRefundTypeListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from refund in dbContext.SchemeRefundTrans
                           where refund.departmentId == model.DepartmentId && refund.schemeId == model.SchemeId && refund.IsActive == true
                           select new DropdownViewModel
                           {
                               Id = refund.refundId,
                               Text = refund.refundDescription
                           });
                return lst.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetApproverIdListByDepartment(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from user in dbContext.UmUserMasters
                            join dept in dbContext.UmUserDepartmentTrans on user.UserRefId equals dept.UserRefId
                            where user.IsActive == true && dept.DepartmentId == model.DepartmentId && user.UserRefId != userInfo.UserID
                            select new DropdownViewModel
                            {
                                Id = user.UserRefId,
                                Text = user.UserName + "-" + user.FirstName
                            }).Distinct();
                return list.ToDataSourceResult(request);
            }
        }


        public int SendReminderToAllottee(int? Rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var flag = ReturnType.None;
                if (Rid != null)
                {
                    var kya = dbContext.KYADetails.FirstOrDefault(m => m.RId == Rid && m.IsActive == true && m.StatusId == NAStatusId.Approved);
                    if (kya != null)
                    {
                        var sector = dbContext.SectorMsts.FirstOrDefault(m => m.sectorId == kya.SectorId).sectorName;
                        var block = dbContext.BlockMsts.FirstOrDefault(m => m.blockId == kya.BlockId).blockName;
                        var propertyNo = sector + "/" + block + "-" + kya.PlotNo;
                        //if (kya.AllotteeType == "Company")
                        //{
                        //    string message = string.Format(NAMessages.DuesPaymentReminder, propertyNo);                        
                        //    if (!string.IsNullOrEmpty(kya.SignatoryMobileNo)) { ApplicationHelper.SendSMS(kya.MobileNo, message); }
                        //    if (!string.IsNullOrEmpty(kya.SignatoryEmail)) { ApplicationHelper.SendEmail(kya.Email, "Reminder", message); }
                        //    flag = ReturnType.Success;
                        //}
                        //else
                        //{
                        //    string message = string.Format(NAMessages.DuesPaymentReminder, propertyNo); 
                        //    if (!string.IsNullOrEmpty(kya.MobileNo)) { ApplicationHelper.SendSMS(kya.MobileNo, message); }
                        //    if (!string.IsNullOrEmpty(kya.Email)) { ApplicationHelper.SendEmail(kya.Email, "Reminder", message); }
                        //    flag = ReturnType.Success;
                        //}

                        NAApplication appl = new NAApplication();
                        if (kya.AllotteeType == "Company")
                        {
                            string message = string.Format(NAMessages.DuesPaymentReminder, propertyNo);
                            if (!string.IsNullOrEmpty(kya.SignatoryMobileNo)) { appl.SaveAndSendSMS(kya.RId, kya.MobileNo, message, "Reminder", "Dues Payment", userInfo.UserID.ToString()); }
                            if (!string.IsNullOrEmpty(kya.SignatoryEmail)) { ApplicationHelper.SendEmail(kya.Email, "Reminder", message); }
                            flag = ReturnType.Success;
                        }
                        else
                        {
                            string message = string.Format(NAMessages.DuesPaymentReminder, propertyNo);
                            if (!string.IsNullOrEmpty(kya.MobileNo)) { appl.SaveAndSendSMS(kya.RId, kya.MobileNo, message, "Reminder", "Dues Payment", userInfo.UserID.ToString()); }
                            if (!string.IsNullOrEmpty(kya.Email)) { ApplicationHelper.SendEmail(kya.Email, "Reminder", message); }
                            flag = ReturnType.Success;
                        }
                    }
                }
                return flag;
            }
        }


        public int IsRidExists(int? Id, int? RegistrationId, string PropertyNo)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                //var isRidExist = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == RegistrationId);
                var details = (from allot in dbContext.AllotmentMasters
                               join scheme in dbContext.SchemePropTrans on allot.propertyId equals scheme.propertyId
                               where allot.rid == RegistrationId
                               select new ServiceViewModel
                               {
                                   SectorId = scheme.sectorId,
                                   Sector = scheme.SectorMst.sectorName,
                                   BlockId = scheme.blockId,
                                   Block = scheme.BlockMst.blockName,
                                   PlotNo = scheme.propertyNo
                               }).FirstOrDefault();
                var prop = details.Sector + "/" + details.Block + "-" + details.PlotNo;
                if (prop != null && PropertyNo != null)
                {
                    if (prop == PropertyNo)
                    {
                        flag = ReturnType.Exist;
                    }
                    else
                    {
                        flag = ReturnType.Mismatch;
                    }
                }
                else
                {
                    flag = ReturnType.NotExist;
                }
                return flag;
            }
        }


        public int ValidateUpdatedProperty(PropertyViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var alotment = dbContext.AllotmentMasters.FirstOrDefault(r => r.rid == model.RegistrationId);
                var property = dbContext.SchemePropTrans.FirstOrDefault(p => p.propertyId == alotment.propertyId);
                var vprop = dbContext.ValidatedPropertyDetailMsts.FirstOrDefault(v => v.RegistrationId == model.RegistrationId);
                if (vprop != null)
                {
                    if (model.PropertyUpdateId == 1)
                    {
                        vprop.IsValidatedByDepartment = true;
                        vprop.DepartmentValidatorId = userInfo.UserID;
                        vprop.DepartmentValidationDate = DateTime.Now;
                        vprop.UpdationCount = vprop.UpdationCount + 1;
                        vprop.ModifiedBy = userInfo.UserID;
                        vprop.ModifiedDate = DateTime.Now;
                    }
                    if (model.PropertyUpdateId == 2)
                    {
                        vprop.IsValidatedByAccounts = true;
                        vprop.AccountsValidatorId = userInfo.UserID;
                        vprop.AccountsValidationDate = DateTime.Now;
                        vprop.UpdationCount = vprop.UpdationCount + 1;
                        vprop.ModifiedBy = userInfo.UserID;
                        vprop.ModifiedDate = DateTime.Now;
                    }
                    if (model.PropertyUpdateId == 3)
                    {
                        vprop.IsValidatedByOrganisation = true;
                        vprop.OrganisationValidatorId = userInfo.UserID;
                        vprop.OrganisationValidationDate = DateTime.Now;
                        vprop.UpdationCount = vprop.UpdationCount + 1;
                        vprop.ModifiedBy = userInfo.UserID;
                        vprop.ModifiedDate = DateTime.Now;
                    }
                }
                else
                {
                    var validprop = new ValidatedPropertyDetailMst()
                    {
                        RegistrationId = model.RegistrationId,
                        DepartmentId = alotment.departmentId,
                        SectorId = property.sectorId,
                        BlockId = property.blockId,
                        PlotNo = property.propertyNo,
                        UpdationCount = 1,
                        IsActive = true,
                        StatusId = NAStatusId.Approved,
                        CreatedBy = userInfo.UserID,
                        CreatedDate = DateTime.Now
                    };
                    if (model.PropertyUpdateId == 1)
                    {
                        validprop.IsValidatedByDepartment = true;
                        validprop.DepartmentValidatorId = userInfo.UserID;
                        validprop.DepartmentValidationDate = DateTime.Now;
                    }
                    if (model.PropertyUpdateId == 2)
                    {
                        validprop.IsValidatedByAccounts = true;
                        validprop.AccountsValidatorId = userInfo.UserID;
                        validprop.AccountsValidationDate = DateTime.Now;
                    }
                    if (model.PropertyUpdateId == 3)
                    {
                        validprop.IsValidatedByOrganisation = true;
                        validprop.OrganisationValidatorId = userInfo.UserID;
                        validprop.OrganisationValidationDate = DateTime.Now;
                    }
                    dbContext.ValidatedPropertyDetailMsts.Add(validprop);
                }
                dbContext.SaveChanges();
                flag = ReturnType.Updated;
            }
            return flag;
        }


        public DataSourceResult GetAllotmentYearAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            List<DropdownViewModel> list = new List<DropdownViewModel>();
            for (int i = 1975; i <= DateTime.Now.Year; i++)
            {
                list.Add(new DropdownViewModel { Id = i, Text = i.ToString() });
            }
            return list.ToDataSourceResult(request);
        }


        public DataSourceResult GetUsersDepartmentListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from dept in dbContext.DepartmentMsts
                            where dept.IsActive == true && DepartmentList.Contains(dept.departmentId)
                            select new DropdownViewModel
                            {
                                Id = dept.departmentId,
                                Text = dept.departmentName
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetStatusListByRoleAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = HttpContext.Current.Session["CurrentUser"] as CurrentUserDetail;
                DateTime? date = Convert.ToDateTime("2019/05/22");
                if (user != null)
                {
                    var list = new List<DropdownViewModel>();
                    if (user.RoleMaster.RoleInDepartment != null && user.RoleMaster.RoleInDepartment == RoleInDepartment.Assistant)
                    {
                        if (model.ServiceId == NAService.Other || model.ServiceId == NAService.Query || model.ServiceId == NAService.SubmissionOfDocument)
                        {
                            list.Add(new DropdownViewModel { Id = NAStatusId.Completed, Text = "Completed" });
                            list.Add(new DropdownViewModel { Id = NAStatusId.Cancelled, Text = "Cancelled" });
                            //list.Add(new DropdownViewModel { Id = NAStatusId.Pending, Text = "Pending" });
                            list.Add(new DropdownViewModel { Id = NAStatusId.Objection, Text = "Objection" });
                        }
                        else
                        {
                            if (model.FilterDate >= date)
                            {
                                list.Add(new DropdownViewModel { Id = NAStatusId.Objection, Text = "Objection" });
                            }
                            else
                            {
                                list.Add(new DropdownViewModel { Id = NAStatusId.Completed, Text = "Completed" });
                                list.Add(new DropdownViewModel { Id = NAStatusId.Cancelled, Text = "Cancelled" });
                                //list.Add(new DropdownViewModel { Id = NAStatusId.Pending, Text = "Pending" });
                                list.Add(new DropdownViewModel { Id = NAStatusId.Objection, Text = "Objection" });
                            }
                        }
                    }
                    else if (user.RoleMaster.RoleInDepartment != null && user.RoleMaster.RoleInDepartment == RoleInDepartment.OSD)
                    {
                        list.Add(new DropdownViewModel { Id = NAStatusId.Forwarded, Text = "Forward for Completion" });
                        list.Add(new DropdownViewModel { Id = NAStatusId.Forwarded, Text = "Forward for Cancellation" });
                    }
                    else
                    {
                        List<int> StatusIdList = new List<int>();
                        StatusIdList.Add(2); StatusIdList.Add(8); StatusIdList.Add(12);
                        list = (from status in dbContext.StatusMasters
                                where status.IsActive == true && StatusIdList.Contains(status.Id)
                                select new DropdownViewModel
                                {
                                    Id = status.Id,
                                    Text = status.Status
                                }).ToList();
                    }
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    var list = (from status in dbContext.StatusMasters
                                where status.IsActive == true
                                select new DropdownViewModel
                                {
                                    Id = status.Id,
                                    Text = status.Status
                                }).ToList();
                    return list.ToDataSourceResult(request);
                }
            }
        }


        public DataSourceResult GetApproverIdByDepartmentAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.RoleName == RoleInDepartment.Assistant)
                {
                    var list = (from urole in dbContext.UmUserMasterRoles
                                join role in dbContext.UmRoleMasters on urole.RoleId equals role.RoleId
                                join user in dbContext.UmUserMasters on urole.UserRefId equals user.UserRefId
                                join dept in dbContext.UmUserDepartmentTrans on user.UserRefId equals dept.UserRefId
                                where user.IsActive == true && DepartmentList.Contains(dept.DepartmentId)
                                && (role.RoleInDepartment == RoleInDepartment.Assistant || role.RoleInDepartment == RoleInDepartment.Accountant)
                                && user.UserRefId != userInfo.UserID
                                select new DropdownViewModel
                                {
                                    Id = user.UserRefId,
                                    Text = user.UserName + "-" + user.FirstName
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }
                if (model.RoleName == RoleInDepartment.OSD)
                {
                    var list = (from urole in dbContext.UmUserMasterRoles
                                join role in dbContext.UmRoleMasters on urole.RoleId equals role.RoleId
                                join user in dbContext.UmUserMasters on urole.UserRefId equals user.UserRefId
                                join dept in dbContext.UmUserDepartmentTrans on user.UserRefId equals dept.UserRefId
                                where user.IsActive == true && DepartmentList.Contains(dept.DepartmentId)
                                && role.RoleInDepartment == RoleInDepartment.OSD && user.UserRefId != userInfo.UserID
                                select new DropdownViewModel
                                {
                                    Id = user.UserRefId,
                                    Text = user.UserName + "-" + user.FirstName
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    var list = (from user in dbContext.UmUserMasters
                                join dept in dbContext.UmUserDepartmentTrans on user.UserRefId equals dept.UserRefId
                                where user.IsActive == true && DepartmentList.Contains(dept.DepartmentId) && user.UserRefId != userInfo.UserID
                                select new DropdownViewModel
                                {
                                    Id = user.UserRefId,
                                    Text = user.UserName + "-" + user.FirstName
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }

            }
        }


        public List<DropdownViewModel> GetYearsList()
        {
            List<DropdownViewModel> list = new List<DropdownViewModel>();
            for (var i = 10; i < 35; i++)
            {
                list.Add(new DropdownViewModel { Text = i.ToString(), Id = i });
            }
            return list;
        }


        public DataSourceResult GetDocumentTypeListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from docs in dbContext.ServiceCheckList_Master
                            where docs.dept_id == model.DepartmentId && docs.service_id == model.ServiceId
                            select new DropdownViewModel
                            {
                                DepartmentId = model.DepartmentId,
                                ServiceId = model.ServiceId,
                                Id = docs.Checklist_Ref.Value,
                                Text = docs.ChkName
                            });
                return list.ToDataSourceResult(request);
            }
        }
        #region Employee Challan
        public DataSourceResult GetEmployeeListAsdatasource(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from emp in dbContext.EmployeeMsts
                            where emp.IsActive==true
                            select new DropdownViewModel
                            {
                                Id = emp.Id,
                                Text = emp.EmployeeCode.ToString()
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public EmployeeViewModel GetEmployeeDetailsById(EmployeeViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var empData = dbContext.EmployeeMsts.FirstOrDefault(m => m.EmployeeCode == model.EmployeeCode && m.IsActive == true);
                if (empData != null)
                {
                    model.Id = empData.Id;
                    model.EmployeeCode = empData.EmployeeCode;
                    model.EmployeeName = empData.EmployeeName;
                    model.CreatedBy = empData.CreatedBy;
                    model.CreatedDate = empData.CreatedDate;
                }
                //var details = (from emp in dbContext.EmployeeMsts
                //         where emp.EmployeeCode==model.EmployeeCode
                //            select new EmployeeViewModel
                //         {
                //             Id=emp.Id,
                //             EmployeeCode=emp.EmployeeCode,
                //             EmployeeName=emp.EmployeeName,
                //             CreatedBy=emp.CreatedBy,
                //             CreatedDate=emp.CreatedDate
                //         }).FirstOrDefault();
            }
            return model;
        }


        public int RemoveTempChallanChargeDetailForEmployee(ChallanViewModel model)
        {
            var flag = ReturnType.None;
            List<ChallanViewModel> dataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempEmpChallan"];//TempChallanModelII
            if (dataList != null)
            {
                if (dataList.Count > 0)
                {
                    var rmodel = dataList.Where(x => x.AccountHead == model.AccountHead && x.AccountSubHead == model.AccountSubHead && x.Amount == model.Amount && x.EmployeeCode == model.EmployeeCode && x.EmployeeName == model.EmployeeName).FirstOrDefault();
                    dataList.Remove(rmodel);
                    flag = ReturnType.Removed;
                }
            }
            return flag;
        }


        public int SaveTempChallanChargeDetailForEmployee(ChallanViewModel model)
        {
            int flag = ReturnType.None;
            List<ChallanViewModel> dataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempEmpChallan"];//TempChallanModelII
            if (dataList == null || dataList.Count == 0)
            {
                List<ChallanViewModel> modelList = new List<ChallanViewModel>();
                model.Id = 1;
                modelList.Add(model);
                HttpContext.Current.Session["TempEmpChallan"] = modelList;
                flag = ReturnType.Saved;
            }
            else
            {
                int id = dataList.Count;
                model.Id = id + 1;
                dataList.Add(model);
                HttpContext.Current.Session["TempEmpChallan"] = dataList;
                flag = ReturnType.Saved;
            }
            using (var dbContext = new NoidaPMSEntities())
            {
                var emp = dbContext.EmployeeMsts.FirstOrDefault(m => m.EmployeeCode == model.EmployeeCode);
                if (emp == null)
                {
                    var empMst = new EmployeeMst
                    {
                        EmployeeCode = model.EmployeeCode,
                        EmployeeName = model.EmployeeName,
                        CreatedBy = userInfo.UserID,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };
                    dbContext.EmployeeMsts.Add(empMst);
                    dbContext.SaveChanges();
                }
            }
            return flag;
        }


        public decimal GetTotalAmount(decimal? amount)
        {
            decimal totalAmount = 0;
            totalAmount = (decimal)(totalAmount + amount);
            List<ChallanViewModel> dataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempEmpChallan"];
            if (dataList != null)
            {
                if (dataList.Count > 0)
                {
                    foreach (var data in dataList)
                    {
                        totalAmount = (decimal)(totalAmount + data.Amount);
                    }
                }
            }
            return totalAmount;
        }
        

        public DataSourceResult GetTempChallanChargesForEmployee(DataSourceRequest request)
        {
            List<ChallanViewModel> tempdata = (List<ChallanViewModel>)HttpContext.Current.Session["TempEmpChallan"];
            if (tempdata != null && tempdata.Count > 0)
            {
                for (int id = 0; id < tempdata.Count; id++) tempdata[id].Id = id;
            }
            return tempdata != null ? tempdata.ToDataSourceResult(request) : null;
        }
        #endregion


        public DataSourceResult GetBankListForPaymentAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.FilterType == "Online")
                {
                    var bankList = new List<int>{67,96};
                    var list = (from bank in dbContext.BankMsts
                                where bankList.Contains(bank.bankId) && bank.IsActive == true
                                select new DropdownViewModel
                                {
                                    Text = bank.bankName,
                                    Id = bank.bankId
                                });
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    var bankId = userInfo.OptionalId != null ? userInfo.OptionalId : null;
                    var list = (from bank in dbContext.BankMsts
                                where (bankId == null || bank.bankId == bankId) && bank.IsActive == true
                                select new DropdownViewModel
                                {
                                    Text = bank.bankName,
                                    Id = bank.bankId
                                });
                    return list.ToDataSourceResult(request);
                }
            }
        }


    }
}
