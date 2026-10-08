using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.OnlineScheme.Context;

namespace NA.PMS.OnlineScheme
{
    public class DefaultSchemeRepository : IDefaultSchemeRepository
    {
        public List<OSDropdownViewModel> GetBankListBySchemeId(OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var lst = (from scheme in dbContext.SchemeBankTrans
                           where (model.SchemeId == null || scheme.schemeId == model.SchemeId) && scheme.IsActive == true
                           select new OSDropdownViewModel
                           {
                               Id = scheme.bankId.Value,
                               Text = scheme.BankMst.bankName
                           }).ToList();
                return lst;
            }
        }

        public DataSourceResult GetSchemeListAsDataSource(DataSourceRequest request, OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var lst = (from scheme in dbContext.SchemeMsts
                           where scheme.IsActive == true && (model.FilterType == null || scheme.SchemeTypeMst.modifiedBy == model.FilterType) //OSStringConstant.OnlineScheme
                           select new OSDropdownViewModel
                           {
                               Id = scheme.schemeId,
                               Text = scheme.schemeName
                           }).ToList();
                return lst.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetDepartmentListAsDataSource(DataSourceRequest request, OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var lst = (from dept in dbContext.DepartmentMsts
                           where dept.IsActive == true && (model.DepartmentId == null || dept.departmentId == model.DepartmentId)
                           select new OSDropdownViewModel
                           {
                               Id = dept.departmentId,
                               Text = dept.departmentName
                           }).ToList();
                return lst.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetDropdownPropertyListForAllotmentAsDataSource(DataSourceRequest request, OSDropdownViewModel model)
        {
            throw new NotImplementedException();
        }

        public DataSourceResult GetDropdownOnlineSchemeFormIdListForAllotmentAsDataSource(DataSourceRequest request, OSDropdownViewModel model)
        {
            throw new NotImplementedException();
        }

        public DataSourceResult GetDropdownOnlineSchemeFormIdListAsDataSource(DataSourceRequest request, OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                if (model != null)
                {
                    //var idlist = (from transaction in dbContext.OnlineApplicationDetails_trans
                    //              join details in dbContext.OnlineApplicationDetails on transaction.ServiceRefId equals details.onlineapplicationId
                    //              where (transaction.ServiceType == 1 || transaction.ServiceType == 6) && details.isActive == true //&& transaction.status == 1 
                    //              && (model.SchemeId == null || details.schemeId == model.SchemeId)
                    //              select new OSDropdownViewModel
                    //              {
                    //                  Id = details.onlineapplicationId,
                    //                  Text = details.onlineapplicationId.ToString(),
                    //                  Value = details.onlineapplicationId.ToString()
                    //              });
                    var idlist = (from details in dbContext.OnlineApplicationDetails
                                  where details.isActive == true 
                                  && (model.SchemeId == null || details.schemeId == model.SchemeId)
                                  && (model.ApplicationFormId == null || details.onlineapplicationId == model.ApplicationFormId)
                                  select new OSDropdownViewModel
                                  {
                                      Id = details.onlineapplicationId,
                                      Text = details.onlineapplicationId.ToString(),
                                      Value = details.onlineapplicationId.ToString(),
                                      SchemeId = details.schemeId
                                  });
                    //request.Filters.RemoveAt(0);
                    if (idlist != null)
                    {
                        return idlist.ToDataSourceResult(request);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public DataSourceResult GetBankListAsDataSource(DataSourceRequest request, OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var lst = (from bank in dbContext.BankMsts
                           where bank.IsActive == true && (model.BankId == null || bank.bankId == model.BankId)
                           select new OSDropdownViewModel
                           {
                               Id = bank.bankId,
                               Text = bank.bankName
                           }).ToList();
                return lst.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetBanksBranchListAsDataSource(DataSourceRequest request, OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var lst = (from bank in dbContext.BankMsts
                           join branch in dbContext.BranchMsts on bank.bankId equals branch.bankId
                           where bank.IsActive == true && branch.IsActive == true
                           && (model.BankId == null || bank.bankId == model.BankId)
                           && (model.BranchId == null || branch.branchId == model.BranchId )
                           select new OSDropdownViewModel
                           {
                               Id = bank.bankId,
                               Text = bank.bankName
                           }).ToList();
                return lst.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetPropertyTypeListAsDataSource(DataSourceRequest request, OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var lst = (from prop in dbContext.PropertyTypeMsts
                           where prop.IsActive == true 
                           && (model.DepartmentId == null || prop.departmentId == model.DepartmentId)
                           && (model.PropertTypeId == null || prop.propertyTypeId == model.PropertTypeId)
                           select new OSDropdownViewModel
                           {
                               Id = prop.propertyTypeId,
                               Text = prop.propertyTypeName,
                               DepartmentId = prop.departmentId
                           });
                return lst.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetFloorAreaListAsDataSource(DataSourceRequest request, OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                if (model.DepartmentId == OSConstant.Housing)
                {
                    var list = (from floor in dbContext.FloorMsts
                                where floor.departmentId == OSConstant.Housing && floor.IsActive == true
                                && (model.FilterType == null || floor.category == model.FilterType)
                                select new OSDropdownViewModel
                                {
                                    Id = floor.floorId,
                                    Text = floor.floorName
                                });
                    return list.ToDataSourceResult(request);
                }
                if (model.DepartmentId == OSConstant.Institutional)
                {
                    var list = (from floor in dbContext.FloorMsts
                                where floor.departmentId == OSConstant.Institutional && floor.IsActive == true
                                && (model.FilterType == null || floor.category == model.FilterType)
                                select new OSDropdownViewModel
                                {
                                    Id = floor.floorId,
                                    Text = floor.floorName
                                });
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    var schemetype = dbContext.SchemeMsts.Where(s => s.schemeId == model.SchemeId).Select(x => x.SchemeTypeMst.SchemeTypeDesc).FirstOrDefault();
                    var list = (from scheme in dbContext.SchemeCostTrans
                                where scheme.schemeId == model.SchemeId && scheme.departmentId == model.DepartmentId && scheme.IsActive == true
                                //&& scheme.FloorMst.modifiedBy == schemetype
                                select new DropdownViewModel
                                {
                                    Id = scheme.FloorMst.floorId,
                                    Text = scheme.FloorMst.floorName
                                }).Distinct();
                    return list.ToDataSourceResult(request);
                }
            }
        }

        public OSLoginViewModel ChangePasswordForOnlineSchemeForm(OSLoginViewModel pmodel)
        {
            using (var dbContext = new OSDbContext())
            {
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(f => f.onlineapplicationId == pmodel.ApplicationFormId && f.Userpassword == pmodel.Password);
                if (form != null && !string.IsNullOrEmpty(pmodel.NewPassword))
                {
                    var _password = OnlineSchemeHelper.MD5HashPassword(pmodel.NewPassword);
                    form.Userpassword = _password;
                    dbContext.SaveChanges();
                    pmodel.PasswordSuccessMessage = "Password changed successfully.";
                    pmodel.IsPasswordChanged = true;
                }
                return pmodel;
            }
        }

        public List<OSDropdownViewModel> GetGenderTypeList(OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Is_Active == 1 && config.Category.ToLower() == OSConfigCategory.GenderType.ToLower()
                           && (model.FilterTypeId == null || config.Range == model.FilterTypeId)
                           select new OSDropdownViewModel
                           {
                               Id = config.Range.Value,
                               Text = config.Name
                           }).ToList();
                return list;
            }
        }

        public List<OSDropdownViewModel> GetMaritalStatusList(OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Is_Active == 1 && config.Category.ToLower() == OSConfigCategory.MaritalStatus.ToLower()
                           && (model.FilterTypeId == null || config.Range == model.FilterTypeId)
                            select new OSDropdownViewModel
                            {
                                Id = config.Range.Value,
                                Text = config.Name
                            }).ToList();
                return list;
            }
        }

        public List<OSDropdownViewModel> GetCategoryList(OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var lst = (from prop in dbContext.QuotaMsts
                           where prop.IsActive == true
                           select new OSDropdownViewModel
                           {
                               Id = prop.quotaId,
                               Text = prop.quotaName,
                           }).ToList();
                return lst;
            }
        }

        public List<OSDropdownViewModel> GetOccupationList(OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var lst = (from f in dbContext.OccupationMsts
                           select new OSDropdownViewModel
                           {
                               Id = f.occupationId,
                               Text = f.occupation
                           }).ToList();
                return lst;
            }
        }

        public DataSourceResult GetSectorListAsDataSource(DataSourceRequest request, OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var list = (from sector in dbContext.SectorMsts
                            where sector.IsActive == true && (model.SectorId == null || sector.sectorId == model.SectorId)
                            select new OSDropdownViewModel
                            {
                                Id = sector.sectorId,
                                Text = sector.sectorName
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public List<OSDropdownViewModel> GetCompanyTypeListByCategory(OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var lst = (from config in dbContext.Common_Config
                           where config.Is_Active == 1 && config.Category.ToLower() == OSConfigCategory.Company.ToLower()
                           select new OSDropdownViewModel
                           {
                               Id = config.Id,
                               Text = config.Name
                           }).ToList();
                return lst;
            }
        }

        public List<OSDropdownViewModel> GetDirectorTypeList(OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var lst = (from cfg in dbContext.Common_Config
                           where cfg.Is_Active == 1 && cfg.Category.ToLower().Equals(OSConfigCategory.Director.ToLower())
                           select new OSDropdownViewModel
                           {
                               Id = cfg.Id,
                               Text = cfg.Name
                           }).ToList();
                return lst;
            }
        }

        public SchemeFormViewModel SendMessageToApplicant(SchemeFormViewModel model)
        {
            throw new NotImplementedException();
        }

        public List<OSDropdownViewModel> GetFormTypeList(OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Is_Active == 1 && config.Category.ToLower() == OSConfigCategory.FormType //"FormType"
                            && (model.FilterType == null || config.Name.ToLower() == model.FilterType.ToLower())
                            select new OSDropdownViewModel
                            {
                                Id = config.Id,
                                Text = config.Name
                            }).ToList();
                return list;
            }
        }

        public List<OSDropdownViewModel> GetFormSubTypeList(OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Is_Active == 1 && config.Category == model.FilterType
                            select new OSDropdownViewModel
                            {
                                Id = config.Id,
                                Text = config.Name
                            }).ToList();
                return list;
            }
        }

        public List<OSDropdownViewModel> GetApplicantTypeList(OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Is_Active == 1 && config.Category.ToLower() == OSConfigCategory.ApplicantType.ToLower() //"ApplicantType"
                            && (model.FilterTypeId == null || config.Range == model.FilterTypeId)
                            select new OSDropdownViewModel
                            {
                                Id = config.Id,
                                Text = config.Name
                            }).ToList();
                return list;
            }
        }

        public List<OSDropdownViewModel> GetPaymentStatusList(OSDropdownViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var lst = (from config in dbContext.Common_Config
                           where config.Is_Active == 1 && config.Category == OSConfigCategory.PaymentStatus
                           select new OSDropdownViewModel
                           {
                               Id = config.Id,
                               Text = config.Name
                           }).ToList();
                return lst;
            }
        }


        public OSSchemeAreaViewModel SaveSchemeAreaRange(OSSchemeAreaViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var area = dbContext.SchemeAreaMsts.FirstOrDefault(m => m.SchemeId == model.SchemeId && m.DepartmentId == model.DepartmentId && m.PropertyTypeRefId == model.PropertyTypeRefId && m.AreaRange == model.AreaRange);
                if (area != null)
                {
                    model.ReturnTypeId = OSReturnTypeId.Exist;
                }
                else
                {
                    var _schemeArea = new SchemeAreaMst();
                    _schemeArea.SchemeId = model.SchemeId;
                    _schemeArea.DepartmentId = model.DepartmentId;
                    _schemeArea.PropertyTypeRefId = model.PropertyTypeRefId;
                    _schemeArea.AreaRange = model.AreaRange;
                    _schemeArea.ApproxArea = model.ApproxArea;
                    _schemeArea.SectorRate = model.SectorRate;
                    _schemeArea.ProcessingFee = model.ProcessingFee;
                    _schemeArea.EarnestMoney = model.EarnestMoney;
                    _schemeArea.ReservedPrice = model.ReservedPrice;
                    _schemeArea.AreaForUse = model.AreaForUse;
                    _schemeArea.Category = model.Category;
                    _schemeArea.IsActive = true;
                    _schemeArea.CreatedBy = 0;
                    _schemeArea.CreatedDate = DateTime.Now;
                    dbContext.SchemeAreaMsts.Add(_schemeArea);
                    dbContext.SaveChanges();
                    model.ReturnTypeId = OSReturnTypeId.Saved;
                }
                return model;
            }
        }

        public DataSourceResult GetSchemeAreaRangeListAsDataSource(DataSourceRequest request, OSSchemeAreaViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var list = (from area in dbContext.SchemeAreaMsts
                            join dept in dbContext.DepartmentMsts on area.DepartmentId equals dept.departmentId
                            join schm in dbContext.SchemeMsts on area.SchemeId equals schm.schemeId
                            where (model.SchemeId == null || area.SchemeId == model.SchemeId)
                            && (model.DepartmentId == null || area.DepartmentId == model.DepartmentId)
                            select new OSSchemeAreaViewModel
                            {
                                Id = area.Id,
                                AreaId = area.AreaId,
                                SchemeId = area.SchemeId,
                                SchemeName = schm.schemeName,
                                DepartmentId = area.DepartmentId,
                                Department = dept.departmentName,
                                AreaForUse = area.AreaForUse,
                                PropertyTypeRefId = area.PropertyTypeRefId,
                                PropertyType = area.PropertyTypeRefId != null ? dbContext.PropertyTypeMsts.FirstOrDefault(p=>p.propertyTypeId == area.PropertyTypeRefId).propertyTypeName : string.Empty,
                                AreaRange = area.AreaRange,
                                ApproxArea = area.ApproxArea,
                                SectorRate = area.SectorRate,
                                ProcessingFee = area.ProcessingFee,
                                EarnestMoney = area.EarnestMoney,
                                ReservedPrice = area.ReservedPrice,
                                Category = area.Category,
                                IsActive = area.IsActive,
                                CreatedDate = area.CreatedDate
                            });
                if (list != null) return list.ToDataSourceResult(request);
                else return null;
            }
        }
    }
}
