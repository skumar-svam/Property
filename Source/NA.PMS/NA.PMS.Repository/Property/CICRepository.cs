using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NA.PMS.Model;
using System.Threading.Tasks;
using NA.PMS.Web.Models;
using System.Web;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Common;

namespace NA.PMS.Repository
{
    public class CICRepository : ICICRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public CICRepository()
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


        public bool SaveCIC(CICModel cicModel)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var dataResult = new Firm_Director_Master
                {
                    Rid = cicModel.Rid,
                    Director_Name = cicModel.Director_Name,
                    Director_Share = cicModel.Director_Share,
                    Type = cicModel.Type,
                    sha_type = cicModel.Share_Type,
                    Is_Active = 1,
                    Request_Date = DateTime.Now,
                    Created_By = userInfo.UserID,
                    Created_Date = DateTime.Now
                };
                dbContext.Firm_Director_Master.Add(dataResult);
                dbContext.SaveChanges();
                //For Audit trails...
                var oldObj = new Firm_Director_Master();
                GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.FirmDirectorMaster, Constants.AddCIC, dataResult.Director_Id, oldObj, dataResult, userInfo.UserID.ToString());
                //
                return true;
            }
        }

        public DataSourceResult GetDirectorDetails(DataSourceRequest req, int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int i = 0;
                var lstDirectorDetails = (from rd in dbContext.Firm_Director_Master
                                          join ty in dbContext.Common_Config on rd.Type equals ty.Id
                                          where rd.Is_Active == 1 && rd.Rid == rid
                                          select new CICModel
                                          {
                                              Director_Id = rd.Director_Id,
                                              Director_Name = rd.Director_Name,
                                              Director_Share = rd.Director_Share,
                                              TypeName = ty.Name,
                                              Share_Type = rd.sha_type,
                                              SNo = 0
                                          }).ToList();
                foreach (var item in lstDirectorDetails)
                {
                    i = i + 1;
                    item.SNo = i;
                }
                return lstDirectorDetails.ToDataSourceResult(req);
            }
        }

        //To Sumbit for Director for Approval
        public bool SubmitForDirectors(int rid, string approver, int reqRefNo, decimal ciccharge)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstAllItems = dbContext.Firm_Director_Master.Where(m => m.Rid == rid && m.Is_Active == 1);
                string lstDirectors = string.Join(",", lstAllItems.Select(x => x.Director_Id));
                var checkDirectorExist = dbContext.Director_Request_Master.FirstOrDefault(m => m.Rid == rid && m.Is_Active == 1);
                var bflag = false;
                if (checkDirectorExist != null)
                {
                    var lstDirectorRequstMaster = new Director_Request_Master();
                    if (checkDirectorExist.Status == 2 || checkDirectorExist.Status == 3)
                    {
                        checkDirectorExist.Status = Constants.InProgress;
                        checkDirectorExist.Approved_By = Convert.ToInt32(approver);
                        checkDirectorExist.Is_Active = 1;
                        checkDirectorExist.Change_Type = Constants.ChangeInDirector;
                        checkDirectorExist.CIC_Charge = ciccharge;
                        checkDirectorExist.Modified_By = userInfo.UserID;
                        checkDirectorExist.Modified_Date = DateTime.Now;
                    }
                    else
                    {
                        checkDirectorExist.Is_Active = 0;
                        lstDirectorRequstMaster = new Director_Request_Master
                        {
                            Rid = rid,
                            Request_Date = DateTime.Now,
                            Status = Constants.InProgress,
                            Director_Id = lstDirectors,
                            Approved_By = Convert.ToInt32(approver),
                            //Approved_By = (from uname in dbContext.UmUserMasters where uname.UserName.ToLower().Equals(approver.ToLower()) && uname.IsActive == true select uname.UserRefId).FirstOrDefault(),
                            Is_Active = 1,
                            Change_Type = Constants.ChangeInDirector,
                            CIC_Charge = ciccharge,
                            Created_By = userInfo.UserID,
                            Created_Date = DateTime.Now,
                            OnlineRequestNo = reqRefNo
                        };
                        dbContext.Director_Request_Master.Add(lstDirectorRequstMaster);
                    }

                    dbContext.SaveChanges();
                    //For Audit trails...
                    var oldObj = new Director_Request_Master();
                    GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.DirectorRequestMaster, Constants.ApproveCIC, lstDirectorRequstMaster.Id, oldObj, lstDirectorRequstMaster, userInfo.UserID.ToString());
                    //
                    bflag = true;
                }
                else
                {
                    var lstDirectorRequstMaster = new Director_Request_Master
                    {
                        Rid = rid,
                        Request_Date = DateTime.Now,
                        Status = Constants.InProgress,
                        Director_Id = lstDirectors,
                        Approved_By = Convert.ToInt32(approver),
                        //Approved_By = (from uname in dbContext.UmUserMasters where uname.UserName.ToLower().Equals(approver.ToLower()) && uname.IsActive == true select uname.UserRefId).FirstOrDefault(),
                        Is_Active = 1,
                        Change_Type = Constants.ChangeInDirector,
                        CIC_Charge = ciccharge,
                        Created_By = userInfo.UserID,
                        Created_Date = DateTime.Now,
                        OnlineRequestNo = reqRefNo
                    };
                    dbContext.Director_Request_Master.Add(lstDirectorRequstMaster);
                    dbContext.SaveChanges();
                    //For Audit trails...
                    var oldObj = new Director_Request_Master();
                    GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.DirectorRequestMaster, Constants.ApproveCIC, lstDirectorRequstMaster.Id, oldObj, lstDirectorRequstMaster, userInfo.UserID.ToString());
                    //
                    bflag = true;
                }
                return bflag;
            }
        }

        //To Sumbit for Firm Name
        public int SubmitForFirmName(int rid, string approver, int newFirmStatus, string oldFirmName, string newFirmName, int reqRefNo, decimal ciccharge)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var bflag = 0;
                //if (userInfo.UserID != null) { approver = Convert.ToString(userInfo.UserID); }//For Auto Approval
                var lstOldItems = dbcontext.Firm_Master.OrderByDescending(n => n.Id).FirstOrDefault(m => m.Rid == rid && m.Is_Active == 1 && m.Status == Constants.Approved);
                var lstOldRequestExists = dbcontext.Firm_Master.FirstOrDefault(m => m.Rid == rid && m.Is_Active == 1 && m.Status == Constants.InProgress);
                if (lstOldRequestExists != null)
                {
                    bflag = 2;
                }
                else if (lstOldItems != null)
                {
                    var lstFirstName = new Firm_Master
                    {
                        Rid = rid,
                        Old_Firm_Status = lstOldItems.New_Firm_Status != null ? lstOldItems.New_Firm_Status : newFirmStatus,
                        New_Firm_Status = newFirmStatus,
                        //Old_Firm_Name = lstOldItems.New_Firm_Name != null ? lstOldItems.New_Firm_Name : newFirmName,
                        Old_Firm_Name = oldFirmName,
                        New_Firm_Name = newFirmName,
                        New_Firm_Product = lstOldItems.New_Firm_Product,
                        Old_Firm_Product = lstOldItems.Old_Firm_Product,
                        Status = Constants.InProgress, // in case of hod approval - 16/7/2019
                        Is_Active = 1,
                        Request_Date = DateTime.Now,
                        Created_By = userInfo.UserID,
                        Created_Date = DateTime.Now,
                        CIC_Charge = ciccharge,
                        Change_Type = Constants.ChangeInFirmName,
                        Approved_By = Convert.ToInt32(approver),
                        //Approved_date = DateTime.Now,//In case of auto approve
                        OnlineRequestNo = reqRefNo
                    };

                    dbcontext.Firm_Master.Add(lstFirstName);
                    dbcontext.SaveChanges();
                    bflag = 1;
                    //For Audit trails...
                    var oldObj = new Firm_Master();
                    GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.FirmMaster, Constants.AddFirm, lstFirstName.Id, oldObj, lstFirstName, userInfo.UserID.ToString());
                    //
                }
                else
                {
                    var lstFirstName = new Firm_Master
                    {
                        Rid = rid,
                        New_Firm_Status = newFirmStatus,
                        Old_Firm_Status = newFirmStatus,
                        New_Firm_Name = newFirmName,
                        Old_Firm_Name = oldFirmName,
                        //Old_Firm_Name = newFirmName,
                        Status = Constants.InProgress, //in case of hod approval - 16/7/2019
                        Is_Active = 1,
                        Request_Date = DateTime.Now,
                        Created_By = userInfo.UserID,
                        Approved_By = Convert.ToInt32(approver),
                        Created_Date = DateTime.Now,
                        CIC_Charge = ciccharge,
                        Change_Type = Constants.ChangeInFirmName,
                        OnlineRequestNo = reqRefNo,
                        //Approved_date = DateTime.Now//In case of auto approve.27-04-2017
                    };

                    dbcontext.Firm_Master.Add(lstFirstName);
                    dbcontext.SaveChanges();
                    bflag = 1;
                    //For Audit trails...
                    var oldObj = new Firm_Master();
                    GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.FirmMaster, Constants.AddFirm, lstFirstName.Id, oldObj, lstFirstName, userInfo.UserID.ToString());
                    //                
                }
                return bflag;
            }
        }

        //To Sumbit for Product Name
        public int SubmitForFirmProduct(int rid, string approver, string oldFirmProduct, string newFirmProduct, int reqRefNo, decimal ciccharge)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var checkFirmExist = dbcontext.Product_Master.Where(m => m.Rid == rid && m.Is_Active == 1).ToList();
                var bflag = 0;
                if (userInfo.UserID != null) { approver = Convert.ToString(userInfo.UserID); }//For Auto Approval
                string ofirmproduct = newFirmProduct;
                if (checkFirmExist != null && checkFirmExist.Count > 0)
                {
                    checkFirmExist.ForEach(m => m.Is_Active = 0);
                    var LastCheckFirmExist = checkFirmExist.OrderByDescending(m => m.Created_Date).FirstOrDefault();
                    ofirmproduct = LastCheckFirmExist.New_Product_Name != null ? LastCheckFirmExist.New_Product_Name : newFirmProduct;
                }

                var lstProdMaster = new Product_Master
                {
                    Rid = rid,
                    New_Product_Name = newFirmProduct,
                    Old_Product_Name = !string.IsNullOrEmpty(oldFirmProduct) ? oldFirmProduct : ofirmproduct,
                    Is_Active = 1,
                    Request_Date = DateTime.Now,
                    Created_By = userInfo.UserID,
                    Created_Date = DateTime.Now,
                    Approved_By = Convert.ToInt32(approver),
                    Approved_Date = DateTime.Now,//In case of auto approve
                    OnlineRequestNo = reqRefNo
                };
                dbcontext.Product_Master.Add(lstProdMaster);
                dbcontext.SaveChanges();
                //For Audit trails...
                var oldObj = new Product_Master();
                GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.FirmMaster, Constants.AddFirmProduct, lstProdMaster.id, oldObj, lstProdMaster, userInfo.UserID.ToString());
                bflag = 1;

                return bflag;
                #region OldCodeCICProduct
                //var checkFirmExist = dbcontext.Firm_Master.FirstOrDefault(m => m.Rid == rid && m.Is_Active == 1);
                //var bflag = 0;
                //var lstOldRequestExists = dbcontext.Firm_Master.FirstOrDefault(m => m.Rid == rid && m.Is_Active == 1 && m.Status == Constants.InProgress);
                //if (lstOldRequestExists != null)
                //{
                //    bflag = 2;
                //}
                //else if (checkFirmExist != null)
                //{
                //    checkFirmExist.Is_Active = 0;
                //    var lstFirstName = new Firm_Master
                //    {
                //        Rid = checkFirmExist.Rid,
                //        New_Firm_Status = checkFirmExist.New_Firm_Status,
                //        Old_Firm_Status = checkFirmExist.Old_Firm_Status,
                //        New_Firm_Name = checkFirmExist.New_Firm_Name,
                //        Old_Firm_Name = checkFirmExist.Old_Firm_Name,
                //        Old_Firm_Product = checkFirmExist.New_Firm_Product != null ? checkFirmExist.New_Firm_Product : newFirmProduct,
                //        New_Firm_Product = newFirmProduct,
                //        Status = Constants.InProgress,
                //        Is_Active = 1,
                //        Request_Date = DateTime.Now,
                //        Created_By = userInfo.UserID,
                //        Created_Date = DateTime.Now,
                //        Approved_By = Convert.ToInt32(approver),
                //        CIC_Charge = ciccharge,
                //        Change_Type = Constants.ChangeInProduct,
                //        OnlineRequestNo = reqRefNo
                //    };
                //    dbcontext.Firm_Master.Add(lstFirstName);
                //    dbcontext.SaveChanges();
                //    //For Audit trails...
                //    var oldObj = new Firm_Master();
                //    GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.FirmMaster, Constants.AddFirmProduct, lstFirstName.Id, oldObj, lstFirstName, userInfo.UserID.ToString());
                //    //
                //    bflag = 1;
                //}
                //else
                //{
                //    var lstFirstName = new Firm_Master
                //    {
                //        Rid = rid,
                //        New_Firm_Product = newFirmProduct,
                //        Old_Firm_Product = newFirmProduct,
                //        Status = Constants.InProgress,
                //        Is_Active = 1,
                //        Request_Date = DateTime.Now,
                //        Created_By = userInfo.UserID,
                //        Created_Date = DateTime.Now,
                //        Approved_By = Convert.ToInt32(approver),
                //        CIC_Charge = ciccharge,
                //        Change_Type = Constants.ChangeInProduct,
                //        OnlineRequestNo = reqRefNo
                //    };
                //    dbcontext.Firm_Master.Add(lstFirstName);
                //    dbcontext.SaveChanges();
                //    //For Audit trails...
                //    var oldObj = new Firm_Master();
                //    GeneralRepository.CreateAuditTrail(Constants.Insert, Constants.FirmMaster, Constants.AddFirmProduct, lstFirstName.Id, oldObj, lstFirstName, userInfo.UserID.ToString());
                //    //
                //    bflag = 1;
                //}
                //return bflag;
                #endregion
            }
        }

        //Get all details for CIC
        public DataSourceResult GetAllCIC(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var lstCIC = (from rd in dbContext.Director_Request_Master
                              join ty in dbContext.Common_Config on rd.Change_Type equals ty.Id
                              join allot in dbContext.AllotmentMasters on rd.Rid equals allot.rid
                              join schpro in dbContext.SchemePropTrans on allot.propertyId equals schpro.propertyId
                              join sec in dbContext.SectorMsts on schpro.sectorId equals sec.sectorId
                              join block in dbContext.BlockMsts on schpro.blockId equals block.blockId
                              join sta in dbContext.StatusMasters on rd.Status equals sta.Id
                              where rd.Is_Active == 1 && allot.isActive == 1 && loginUserDeptt.Contains(rd.AllotmentMaster.departmentId)
                              select new CICModel
                              {
                                  Rid = rd.Rid,
                                  Director_Id = rd.Id,
                                  DepartmentName = rd.AllotmentMaster.DepartmentMst.departmentName,
                                  SectorName = sec.sectorName,
                                  BlockName = block.blockName,
                                  PropertyNo = schpro.propertyNo,
                                  PropNo = sec.sectorName + "/" + block.blockName + "-" + schpro.propertyNo,
                                  Request_Date = rd.Request_Date,
                                  Approved_Date = rd.Approved_date,
                                  AssignTo = (from uname in dbContext.UmUserMasters where uname.UserRefId == rd.Approved_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                  StatusName = sta.Status,
                                  Change_Type = rd.Change_Type
                              }).ToList();
                return lstCIC.ToDataSourceResult(req);
            }

        }

        //Get all details for FirmProductStatus
        public DataSourceResult GetAllFirmProductStatus(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var lstCIC = (from rd in dbContext.Firm_Master
                              join ty in dbContext.Common_Config on rd.Change_Type equals ty.Id
                              join allot in dbContext.AllotmentMasters on rd.Rid equals allot.rid
                              join schpro in dbContext.SchemePropTrans on allot.propertyId equals schpro.propertyId
                              join sec in dbContext.SectorMsts on schpro.sectorId equals sec.sectorId
                              join block in dbContext.BlockMsts on schpro.blockId equals block.blockId
                              join sta in dbContext.StatusMasters on rd.Status equals sta.Id
                              where rd.Is_Active == 1 && allot.isActive == 1 && loginUserDeptt.Contains(rd.AllotmentMaster.departmentId)
                              select new CICModel
                              {
                                  Director_Id = rd.Id,
                                  Rid = rd.Rid,
                                  DepartmentName = allot.DepartmentMst.departmentName,
                                  SectorName = sec.sectorName,
                                  BlockName = block.blockName,
                                  PropertyNo = schpro.propertyNo,
                                  PropNo = sec.sectorName + "/" + block.blockName + "-" + schpro.propertyNo,
                                  ChangeTypeName = ty.Name,
                                  Request_Date = rd.Request_Date,
                                  Approved_Date = rd.Approved_date,
                                  AssignTo = (from uname in dbContext.UmUserMasters where uname.UserRefId == rd.Approved_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                  Status = rd.Status.Value,
                                  Change_Type = rd.Change_Type,
                                  StatusName = sta.Status
                              }).ToList();
                return lstCIC.ToDataSourceResult(req);
            }
        }

        //Get all details for CIC by Approver
        public DataSourceResult GetAllCICByApprover(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var lstDirectors = (from rd in dbContext.Director_Request_Master
                                    join ty in dbContext.Common_Config on rd.Change_Type equals ty.Id
                                    join allot in dbContext.AllotmentMasters on rd.Rid equals allot.rid
                                    join schpro in dbContext.SchemePropTrans on allot.propertyId equals schpro.propertyId
                                    join sec in dbContext.SectorMsts on schpro.sectorId equals sec.sectorId
                                    join block in dbContext.BlockMsts on schpro.blockId equals block.blockId
                                    join sta in dbContext.StatusMasters on rd.Status equals sta.Id
                                    where rd.Is_Active == 1 && rd.Approved_By == userInfo.UserID && sta.Id == Constants.InProgress && loginUserDeptt.Contains(rd.AllotmentMaster.departmentId)
                                    select new CICModel
                                    {
                                        Rid = rd.Rid,
                                        Director_Id = rd.Id,
                                        DepartmentName = rd.AllotmentMaster.DepartmentMst.departmentName,
                                        PropNo = sec.sectorName + "/" + block.blockName + "-" + schpro.propertyNo,
                                        Request_Date = rd.Request_Date,
                                        Approved_Date = rd.Approved_date,
                                        AssignTo = (from uname in dbContext.UmUserMasters where uname.UserRefId == rd.Approved_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                        StatusName = sta.Status,
                                        Change_Type = rd.Change_Type
                                    }).ToList();
                return lstDirectors.ToDataSourceResult(req);
            }
        }

        //Get all details for FirmProductStatus by Approver
        public DataSourceResult GetAllFirmProductStatusByApprover(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var lstCIC = (from rd in dbContext.Firm_Master
                              join ty in dbContext.Common_Config on rd.Change_Type equals ty.Id
                              join allot in dbContext.AllotmentMasters on rd.Rid equals allot.rid
                              join schpro in dbContext.SchemePropTrans on allot.propertyId equals schpro.propertyId
                              join sec in dbContext.SectorMsts on schpro.sectorId equals sec.sectorId
                              join block in dbContext.BlockMsts on schpro.blockId equals block.blockId
                              where rd.Is_Active == 1 && rd.Approved_By == userInfo.UserID && rd.Status == Constants.InProgress
                              select new CICModel
                              {
                                  Director_Id = rd.Id,
                                  Rid = rd.Rid,
                                  DepartmentName = allot.DepartmentMst.departmentName,
                                  PropNo = sec.sectorName + "/" + block.blockName + "-" + schpro.propertyNo,
                                  ChangeTypeName = ty.Name,
                                  Request_Date = rd.Request_Date,
                                  Approved_Date = rd.Approved_date,
                                  Status = rd.Status.Value,
                                  StatusName = (from statusname in dbContext.StatusMasters where statusname.Id == rd.Status.Value select statusname.Status).FirstOrDefault(),
                                  AssignTo = (from uname in dbContext.UmUserMasters where uname.UserRefId == rd.Approved_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                  Change_Type = rd.Change_Type
                              }).ToList();
                return lstCIC.ToDataSourceResult(req);
            }
        }

        //To Remove Record (Soft Delete)
        public bool RemoveRecord(int dirID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var dResult = dbContext.Firm_Director_Master.FirstOrDefault(m => m.Director_Id == dirID && m.Is_Active == 1);
                var bflag = false;
                var objOld = new Firm_Director_Master
                {
                    Is_Active = dResult.Is_Active
                };
                if (dResult != null)
                {
                    dResult.Is_Active = 0;
                    dResult.Modified_By = userInfo.UserID;
                    dResult.Modified_Date = DateTime.Now;
                    dbContext.SaveChanges();
                    bflag = true;
                }
                //For Audit trails...               
                var objNew = new Firm_Director_Master
                {
                    Is_Active = dResult.Is_Active
                };
                GeneralRepository.CreateAuditTrail(Constants.Delete, Constants.FirmDirectorMaster, Constants.DeleteFirmDirectorMaster, dirID, objOld, objNew, userInfo.UserID.ToString());
                //
                return bflag;
            }
        }

        //To Get Firm old Details.
        public CICModel GetOldFirmName(int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var lstItems = dbContext.Firm_Master.FirstOrDefault(m => m.Rid == rid && m.Is_Active == 1);
                var lstItems = (from fm in dbContext.Firm_Master where fm.Rid == rid && fm.Is_Active == 1 orderby fm.Id descending select fm).FirstOrDefault();
                var lstCIC = new CICModel();
                if (lstItems != null)
                {
                    lstCIC.OldFirmName = lstItems.New_Firm_Name;
                    //lstCIC.OldFirmStatusName = (from statusname in dbContext.Common_Config where statusname.Id == lstItems.Old_Firm_Status && statusname.Is_Active == 1 select statusname.Name).FirstOrDefault();//lstItems.Old_Firm_Status;
                    lstCIC.OldFirmStatusName = (from statusname in dbContext.Common_Config where statusname.Id == lstItems.New_Firm_Status && statusname.Is_Active == 1 select statusname.Name).FirstOrDefault();
                }
                return lstCIC;
            }
        }

        //To Get Firm old Product Details.
        public CICModel GetOldFirmProduct(int rid)
        {
            using (var dbResult = new NoidaPMSEntities())
            {
                var lstItems = dbResult.Product_Master.OrderByDescending(m => m.id).FirstOrDefault(m => m.Rid == rid && m.Is_Active == 1);//dbResult.Firm_Master.FirstOrDefault(m => m.Rid == rid && m.Is_Active == 1);
                var lstCIC = new CICModel();
                if (lstItems != null)
                {
                    lstCIC.OldFirmProduct = lstItems.New_Product_Name;//lstItems.Old_Firm_Product;
                }
                return lstCIC;
            }

        }

        ////To Get All details for view.
        public CICModel GetDetailsbyDirID(int id, int type)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstCICModel = new CICModel();
                if ((type == Constants.ChangeInDirector))
                {
                    var lstDirbyID = dbContext.Director_Request_Master.FirstOrDefault(m => m.Id == id && m.Is_Active == 1);
                    if (lstDirbyID != null)
                    {
                        lstCICModel.Id = lstDirbyID.Id;
                        lstCICModel.Rid = lstDirbyID.Rid;
                        lstCICModel.Change_Type = lstDirbyID.Change_Type;
                        lstCICModel.CIC_Charge = lstDirbyID.CIC_Charge;
                        lstCICModel.Status = lstDirbyID.Status;
                        lstCICModel.StatusName = (from statusname in dbContext.StatusMasters where statusname.Id == lstDirbyID.Status select statusname.Status).FirstOrDefault();
                        lstCICModel.ReqRefNo = lstDirbyID.OnlineRequestNo != null ? (int)lstDirbyID.OnlineRequestNo : 0;
                    }
                }
                else
                {
                    var lstFirmbyID = dbContext.Firm_Master.FirstOrDefault(m => m.Id == id && m.Is_Active == 1);
                    if (lstFirmbyID != null)
                    {
                        lstCICModel.Id = lstFirmbyID.Id;
                        lstCICModel.Rid = lstFirmbyID.Rid;
                        lstCICModel.OldFirmName = lstFirmbyID.Old_Firm_Name;
                        lstCICModel.OldFirmProduct = lstFirmbyID.Old_Firm_Product;
                        lstCICModel.OldFirmStatus = lstFirmbyID.Old_Firm_Status;
                        lstCICModel.OldFirmStatusName = (from cc in dbContext.Common_Config where cc.Id == lstFirmbyID.Old_Firm_Status && cc.Is_Active == 1 select cc.Name).FirstOrDefault();//lstFirmbyID.Old_Firm_Status;
                        lstCICModel.NewFirmName = lstFirmbyID.New_Firm_Name;
                        lstCICModel.NewFirmProduct = lstFirmbyID.New_Firm_Product;
                        lstCICModel.NewFirmStatus = lstFirmbyID.New_Firm_Status;
                        lstCICModel.Change_Type = lstFirmbyID.Change_Type;
                        lstCICModel.CIC_Charge = lstFirmbyID.CIC_Charge;
                        lstCICModel.Status = lstFirmbyID.Status;
                        lstCICModel.Comment = lstFirmbyID.Comment;
                        lstCICModel.StatusName = (from statusname in dbContext.StatusMasters where statusname.Id == lstFirmbyID.Status && statusname.IsActive == true select statusname.Status).FirstOrDefault();
                        lstCICModel.ReqRefNo = lstFirmbyID.OnlineRequestNo != null ? (int)lstFirmbyID.OnlineRequestNo : 0;
                    }
                }
                return lstCICModel;
            }
        }

        //To Fill Mortgage Prev Loan Drop Down
        public CICModel GetPropertyDetailByRid(int rID)
        {
            var lst = new CICModel();
            using (var dbContext = new NoidaPMSEntities())
            {

                lst = (from allot in dbContext.AllotmentMasters
                       join applicant in dbContext.ApplicationDetails on allot.rid equals applicant.registrationId
                       join proptrans in dbContext.SchemePropTrans on new { allot.schemeId, allot.departmentId } equals new { proptrans.schemeId, proptrans.departmentId }
                       join proptype in dbContext.PropertyTypeMsts on proptrans.propertyTypeId equals proptype.propertyTypeId
                       join sec in dbContext.SectorMsts on proptrans.sectorId equals sec.sectorId
                       join block in dbContext.BlockMsts on proptrans.blockId equals block.blockId
                       where allot.rid == rID && allot.isActive == 1
                       select new CICModel
                       {
                           ApplicantName = applicant.tFirstName + " " + applicant.tMiddleName + " " + applicant.tLastName,
                           FatherName = applicant.tFatherHusbandName,
                           PropertyNumber = sec.sectorName + "/" + block.blockName + "-" + proptrans.propertyNo,
                           PropertyTypeName = proptype.propertyTypeName,
                           Gender = applicant.tGender,
                           IsFirmExists = (from firm in dbContext.Firm_Master where firm.Rid == rID && firm.Is_Active == 1 select "true").FirstOrDefault()
                       }).FirstOrDefault();
                return lst;
            }
        }

        ////To Get All details for Firm.
        public CICModel GetDetailsbyFirm(int id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstFirmbyID = dbContext.Firm_Master.FirstOrDefault(m => m.Id == id && m.Is_Active == 1);
                var lstCICModel = new CICModel();
                if (lstFirmbyID != null)
                {
                    lstCICModel.Rid = lstFirmbyID.Rid;
                    lstCICModel.OldFirmName = lstFirmbyID.Old_Firm_Name;
                    lstCICModel.OldFirmProduct = lstFirmbyID.Old_Firm_Product;
                    lstCICModel.OldFirmStatus = lstFirmbyID.Old_Firm_Status;
                    lstCICModel.NewFirmName = lstFirmbyID.New_Firm_Name;
                    lstCICModel.NewFirmProduct = lstFirmbyID.New_Firm_Product;
                    lstCICModel.NewFirmStatus = lstFirmbyID.New_Firm_Status;
                    lstCICModel.Change_Type = lstFirmbyID.Change_Type;
                    lstCICModel.CIC_Charge = lstFirmbyID.CIC_Charge;
                    lstCICModel.Status = lstFirmbyID.Status.Value;
                    lstCICModel.Comment = lstCICModel.Comment;
                }
                return lstCICModel;
            }
        }

        ////To Cancel Request
        public bool CancelCICRequest(int requestID, int typeID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var bflag = false;
                if ((typeID == Constants.ChangeInDirector))
                {
                    var dResult = dbContext.Director_Request_Master.FirstOrDefault(m => m.Id == requestID && m.Is_Active == 1);
                    if (dResult != null)
                    {
                        dResult.Status = Constants.Cancelled;
                        dResult.Modified_By = userInfo.UserID;
                        dResult.Modified_Date = DateTime.Now;
                        dbContext.SaveChanges();
                        bflag = true;
                    }
                }
                else
                {
                    var lstFirmbyID = dbContext.Firm_Master.FirstOrDefault(m => m.Id == requestID && m.Is_Active == 1);
                    if (lstFirmbyID != null)
                    {
                        lstFirmbyID.Status = Constants.Cancelled;
                        lstFirmbyID.Modified_By = userInfo.UserID;
                        lstFirmbyID.Modified_Date = DateTime.Now;
                        dbContext.SaveChanges();
                        bflag = true;
                    }
                }
                return bflag;
            }
        }

        // To Save comment of approver.
        public bool SaveCommentByRequestID(int requestNo, string Comment, bool acceptReject, int typeID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var bflag = false;
                if ((typeID == Constants.ChangeInDirector))
                {
                    var dResult = dbContext.Director_Request_Master.FirstOrDefault(m => m.Id == requestNo && m.Is_Active == 1);
                    if (dResult != null)
                    {
                        dResult.Status = acceptReject == true ? Constants.Approved : Constants.RejectedProp; ;
                        dResult.Modified_By = userInfo.UserID;
                        dResult.Modified_Date = DateTime.Now;
                        dResult.Approved_date = acceptReject == true ? DateTime.Now : DateTime.Parse("01/01/1900");
                        dResult.Comment = Comment;
                        dbContext.SaveChanges();

                        if (dResult.Status == Constants.Approved)
                        {
                            if (dResult.OnlineRequestNo != 0)
                            {
                                var deptId = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == dResult.Rid).departmentId;
                                PropertyRegistrationRepository repo = new PropertyRegistrationRepository();
                                repo.UpdateServiceRequest(dResult.OnlineRequestNo, dResult.Rid, deptId, Comment);
                            }
                        }
                        bflag = true;
                    }
                }
                else
                {
                    var lstFirmbyID = dbContext.Firm_Master.FirstOrDefault(m => m.Id == requestNo && m.Is_Active == 1);
                    if (lstFirmbyID != null)
                    {
                        lstFirmbyID.Status = acceptReject == true ? Constants.Approved : Constants.RejectedProp;
                        lstFirmbyID.Modified_By = userInfo.UserID;
                        lstFirmbyID.Modified_Date = DateTime.Now;
                        lstFirmbyID.Approved_date = acceptReject == true ? DateTime.Now : DateTime.Parse("01/01/1900");
                        lstFirmbyID.Comment = Comment;
                        dbContext.SaveChanges();

                        if (lstFirmbyID.Status == Constants.Approved)
                        {
                            var appdetails = dbContext.ApplicationDetails.FirstOrDefault(m => m.registrationId == lstFirmbyID.Rid);
                            if (appdetails != null)
                            {
                                if (lstFirmbyID.Change_Type == 2)
                                {
                                    appdetails.tFirstName = lstFirmbyID.New_Firm_Name;
                                    appdetails.tMiddleName = null;
                                    appdetails.lastName = null;
                                    dbContext.SaveChanges();
                                }
                            }
                            if (lstFirmbyID.OnlineRequestNo != 0)
                            {
                                var deptId = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == lstFirmbyID.Rid).departmentId;
                                PropertyRegistrationRepository repo = new PropertyRegistrationRepository();
                                repo.UpdateServiceRequest(lstFirmbyID.OnlineRequestNo, lstFirmbyID.Rid, deptId, Comment);
                            }
                        }
                        bflag = true;
                    }
                }
                return bflag;
            }
        }


        //To Update for Firm Name
        public bool UpdateForFirmName(int Id, int rid, string approver, int newFirmStatus, string newFirmName, decimal ciccharge)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var lstOldItems = dbcontext.Firm_Master.FirstOrDefault(m => m.Id == Id && m.Is_Active == 1);
                if (lstOldItems != null)
                {
                    var objOld = new Firm_Master
                    {
                        Rid = lstOldItems.Rid,
                        New_Firm_Name = lstOldItems.New_Firm_Name,
                        New_Firm_Status = lstOldItems.New_Firm_Status,
                        CIC_Charge = lstOldItems.CIC_Charge,
                        //Approved_By = lstOldItems.Approved_By,
                        Approved_By = Convert.ToInt32(approver),
                        Status = lstOldItems.Status,
                        Modified_By = lstOldItems.Modified_By,
                        Modified_Date = lstOldItems.Modified_Date
                    };

                    var objNew = new Firm_Master
                    {
                        Rid = rid,
                        New_Firm_Name = newFirmName,
                        New_Firm_Status = newFirmStatus,
                        CIC_Charge = lstOldItems.CIC_Charge,
                        //Approved_By = lstOldItems.Approved_By,
                        Approved_By = Convert.ToInt32(approver),
                        Status = lstOldItems.Status,
                        Modified_By = lstOldItems.Modified_By,
                        Modified_Date = lstOldItems.Modified_Date
                    };

                    lstOldItems.Rid = rid;
                    lstOldItems.New_Firm_Status = newFirmStatus;
                    lstOldItems.New_Firm_Name = newFirmName;
                    lstOldItems.Old_Firm_Name = lstOldItems.New_Firm_Name;
                    lstOldItems.Old_Firm_Status = lstOldItems.New_Firm_Status;
                    lstOldItems.CIC_Charge = ciccharge;
                    //lstOldItems.Approved_By = (from uname in dbcontext.UmUserMasters where uname.UserName.ToLower().Equals(approver.ToLower()) && uname.IsActive == true select uname.UserRefId).FirstOrDefault();
                    lstOldItems.Approved_By = Convert.ToInt32(approver);
                    lstOldItems.Status = Constants.InProgress;
                    lstOldItems.Modified_Date = DateTime.Now;
                    lstOldItems.Modified_By = userInfo.UserID;
                    dbcontext.SaveChanges();
                    GeneralRepository.CreateAuditTrail(Constants.Update, Constants.FirmMaster, Constants.UpdateFirmDirectorMaster, rid, objOld, objNew, userInfo.UserID.ToString());
                }
                return true;
            }
        }

        //To Update for Product Name
        public bool UpdateForFirmProduct(int Id, int rid, string approver, string newFirmProduct, decimal ciccharge)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var checkFirmExist = dbcontext.Firm_Master.FirstOrDefault(m => m.Id == Id && m.Is_Active == 1);
                var bflag = false;
                if (checkFirmExist != null)
                {
                    checkFirmExist.Rid = rid;
                    checkFirmExist.New_Firm_Product = newFirmProduct;
                    checkFirmExist.CIC_Charge = ciccharge;
                    checkFirmExist.Approved_By = Convert.ToInt32(approver);
                    //checkFirmExist.Approved_By = (from uname in dbcontext.UmUserMasters where uname.UserName.ToLower().Equals(approver.ToLower()) && uname.IsActive == true select uname.UserRefId).FirstOrDefault();
                    checkFirmExist.Old_Firm_Product = checkFirmExist.New_Firm_Product;
                    checkFirmExist.Status = Constants.InProgress;
                    checkFirmExist.Modified_Date = DateTime.Now;
                    checkFirmExist.Modified_By = userInfo.UserID;
                    dbcontext.SaveChanges();
                    bflag = true;
                }
                return bflag;
            }
        }

        // To Generate CIC Letter
        public string GenerateCICLetter(int Id)
        {
            string strLettter = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                var Objdeptt = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == Id);
                if (Objdeptt != null)
                {
                    strLettter = CommonMethords.GenerateLetter(Objdeptt.rid, Constants.CICTemplateID, Objdeptt.departmentId.Value, userInfo.UserID);
                }
            }
            return strLettter;
        }

        //Get all RIDs
        public DataSourceResult GetRIDsByDeptt(DataSourceRequest Req, int Rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstDeptts = (from u in dbContext.UmUserMasters
                                 join d in dbContext.UmUserDepartmentTrans on u.UserRefId equals d.UserRefId
                                 where u.UserRefId == userInfo.UserID
                                 select d.DepartmentId).ToList();
                var lst = (from f in dbContext.AllotmentMasters
                           where f.isActive == 1 && lstDeptts.Contains(f.departmentId) && f.departmentId != DepartmentOption.Housing && f.isStatus.ToLower() == Common.AllotmentStatus.Approved.ToString().ToLower()
                           orderby f.createdDate descending
                           select new DDList
                           {
                               id = f.rid,
                               text = f.rid.ToString()
                           });
                if (Rid > 0) { lst = lst.Where(m => m.id == Rid); }
                return lst.ToDataSourceResult(Req);
            }
        }


        public DataSourceResult GetRegistrationIdsByDepartment(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstDeptts = (from u in dbContext.UmUserMasters
                                 join d in dbContext.UmUserDepartmentTrans on u.UserRefId equals d.UserRefId
                                 where u.UserRefId == userInfo.UserID
                                 select d.DepartmentId).ToList();
                var lst = (from f in dbContext.AllotmentMasters
                           where f.isActive == 1 && lstDeptts.Contains(f.departmentId) && f.departmentId != DepartmentOption.Housing && f.isStatus.ToLower() == Common.AllotmentStatus.Approved.ToString().ToLower()
                           orderby f.createdDate descending
                           select new DDList
                           {
                               id = f.rid,
                               text = f.rid.ToString()
                           });
                return lst.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetDirectorAndShareholderListAsDataSource(DataSourceRequest request, CICViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "Director")
                {
                    var list = (from director in dbContext.Director_Request_Master
                                join alotment in dbContext.AllotmentMasters on director.Rid equals alotment.rid
                                join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                                join status in dbContext.StatusMasters on director.Status equals status.Id
                                where (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                                && (model.SectorId == null || property.sectorId == model.SectorId)
                                && (model.BlockId == null || property.blockId == model.BlockId)
                                && (model.CICStatusId == null || director.Status == model.CICStatusId)
                                select new CICViewModel
                                {
                                    Id = director.Id,
                                    RegistrationId = director.Rid,
                                    DirectorId = director.Id,
                                    Department = director.AllotmentMaster.DepartmentMst.departmentName,
                                    Sector = property.SectorMst.sectorName,
                                    Block = property.BlockMst.blockName,
                                    PlotNo = property.propertyNo,
                                    //PropNo = sec.sectorName + "/" + block.blockName + "-" + schpro.propertyNo,
                                    RequestDate = director.Request_Date,
                                    ApprovedDate = director.Approved_date,
                                    Approver = director.Approved_By,
                                    ApproverName = (from uname in dbContext.UmUserMasters where uname.UserRefId == director.Approved_By select uname.FirstName + (string.IsNullOrEmpty(uname.MiddleName) ? string.Empty : " " + uname.MiddleName + " ") + uname.LastName).FirstOrDefault(),
                                    CICStatus = status.Status,
                                    ChangeTypeId = director.Change_Type,
                                    IsCICActive = director.Is_Active,
                                    OnlineRequestNo = director.OnlineRequestNo
                                }).Distinct();
                    return list != null ? list.ToDataSourceResult(request) : null;
                }
                if (model.ActionType == "DirectorList")
                {
                    var list = (from director in dbContext.Firm_Director_Master
                                join config in dbContext.Common_Config on director.Type equals config.Id
                                where director.Rid == model.RegistrationId
                                select new CICViewModel
                                {
                                    Id = director.Director_Id,
                                    RegistrationId = director.Rid,
                                    DirectorId = director.Director_Id,
                                    DirectorName = director.Director_Name,
                                    DirectorShare = director.Director_Share,
                                    RequestDate = director.Request_Date,
                                    ApprovedDate = director.Approved_Date,
                                    TypeName = config.Name,
                                    IsCICActive = director.Is_Active,
                                    CICStatus = director.Is_Active == 1 ? "Active" : "InActive"
                                });
                    return list != null ? list.ToDataSourceResult(request) : null;
                }
                else return null;
            }
        }

        public DataSourceResult GetFirmAndProductNameListAsDataSource(DataSourceRequest request, CICViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from firm in dbContext.Firm_Master
                            join alotment in dbContext.AllotmentMasters on firm.Rid equals alotment.rid
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                                && (model.SectorId == null || property.sectorId == model.SectorId)
                                && (model.BlockId == null || property.blockId == model.BlockId)
                            select new CICViewModel
                            {
                                Id = firm.Id,
                                RegistrationId = firm.Rid,
                                OldFirmName = firm.Old_Firm_Name,
                                NewFirmName = firm.New_Firm_Name,
                                OldFirmStatus = firm.Old_Firm_Status != null ? dbContext.Common_Config.FirstOrDefault(d => d.Id == firm.Old_Firm_Status && d.Category == "Firm Status").Name : string.Empty,
                                NewFirmStatus = firm.Old_Firm_Status != null ? dbContext.Common_Config.FirstOrDefault(d => d.Id == firm.New_Firm_Status && d.Category == "Firm Status").Name : string.Empty,
                                IsCICActive = firm.Is_Active,
                                ChangeTypeId = firm.Change_Type,
                                ApprovedDate = firm.Approved_date,
                                RequestDate = firm.Request_Date,
                                Department = property.DepartmentMst.departmentName,
                                Sector = property.SectorMst.sectorName,
                                Block = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                CICCharge = firm.CIC_Charge,
                                CICStatus = firm.Status != null ? dbContext.StatusMasters.FirstOrDefault(s => s.Id == firm.Status).Status : string.Empty
                            });
                return list != null ? list.ToDataSourceResult(request) : null;
            }
        }
    }
}
