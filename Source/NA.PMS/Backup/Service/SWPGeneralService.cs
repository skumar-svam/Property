using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NA.PMS.NICService.Context;
using System.Configuration;

namespace NA.PMS.NICServices
{
    public class SWPGeneralService : ISWPGeneralService
    {
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public SWPGeneralService()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["CurrentUser"] != null)
                    {
                        userInfo = HttpContext.Current.Session["CurrentUser"] as CurrentUserDetail;
                        using (var dbContext = new PIMSEntitiesContext())
                        {
                            DepartmentList = dbContext.UmUserDepartmentTrans.Where(u => u.UserRefId == userInfo.UserID && u.Status == true).Select(d => d.DepartmentId).ToList();
                        }
                    }
                }
            }
        }

        public DataSourceResult GetServiceListAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var list = (from services in dbContext.CitizenService_Master
                            where services.Status == 1
                            group services by services.service_id into grpservice
                            select new SWPDropdownViewModel
                            {
                                Id = grpservice.FirstOrDefault().service_id.Value,
                                Text = grpservice.FirstOrDefault().ServiceName
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public List<SWPDropdownViewModel> GetStatusList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var list = (from status in dbContext.StatusMasters
                            where status.IsActive == true
                            select new SWPDropdownViewModel
                            {
                                Text = status.Status,
                                Id = status.Id
                            }).ToList();
                return list;
            }
        }

        public DataSourceResult GetStatusMasterAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var list = (from status in dbContext.StatusMasters
                            where status.IsActive == true
                            select new SWPDropdownViewModel
                            {
                                Id = status.Id,
                                Text = status.Status
                            }).ToList();
                return list.ToDataSourceResult(request);
            }
        }

        public SWPLoginViewModel GetLoginUserDetails(int userId)
        {
            SWPLoginViewModel loginUserDetail = null;
            using (var dbContext = new PIMSEntitiesContext())
            {
                loginUserDetail = (from user in dbContext.UmUserMasters
                                   where user.UserRefId == userId
                                   select new SWPLoginViewModel
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

        public List<SWPDropdownViewModel> GetDepartmentList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from dept in dbContext.DepartmentMsts
                           where dept.IsActive == true
                           select new SWPDropdownViewModel
                           {
                               Id = dept.departmentId,
                               Text = dept.departmentName
                           }).ToList();
                return lst;
            }
        }

        public List<SWPDropdownViewModel> GetSubDepartmentList(int departmentId)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from service in dbContext.SubDepartmentMsts
                           where service.departmentId == departmentId && service.IsActive == true
                           select new SWPDropdownViewModel
                           {
                               Id = service.SubdepartmentId,
                               Text = service.SubdepartmentName
                           }).ToList();
                return lst;
            }
        }

        public List<SWPDropdownViewModel> GetFirmStatusList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from cfg in dbContext.Common_Config
                           where cfg.Is_Active == 1 && cfg.Category.ToLower().Equals(SWPCategoryType.FirmStatus.ToLower())
                           select new SWPDropdownViewModel
                           {
                               Id = cfg.Id,
                               Text = cfg.Name
                           }).ToList();
                return lst;
            }
        }

        public List<SWPDropdownViewModel> GetMortgageTypeList()
        {
            List<SWPDropdownViewModel> mortgageType = new List<SWPDropdownViewModel>();
            foreach (int value in Enum.GetValues(typeof(SWPEnum.MortgageType)))
            {
                mortgageType.Add(new SWPDropdownViewModel
                {
                    Text = Enum.GetName(typeof(SWPEnum.MortgageType), value),
                    Id = value
                });
            }
            return mortgageType;
        }

        public List<SWPDropdownViewModel> GetNOCStatusList()
        {
            List<SWPDropdownViewModel> statusList = new List<SWPDropdownViewModel>();
            foreach (int value in Enum.GetValues(typeof(SWPEnum.MortgageStatus)))
            {
                statusList.Add(new SWPDropdownViewModel
                {
                    Text = Enum.GetName(typeof(SWPEnum.MortgageStatus), value),
                    Id = value
                });
            }
            return statusList;
        }

        public List<SWPDropdownViewModel> GetGPAStatusList()
        {
            List<SWPDropdownViewModel> gender = new List<SWPDropdownViewModel>();
            foreach (int value in Enum.GetValues(typeof(SWPEnum.ActiveStatus)))
            {
                gender.Add(new SWPDropdownViewModel
                {
                    Text = Enum.GetName(typeof(SWPEnum.ActiveStatus), value),
                    Id = value
                });
            }
            return gender;
        }

        public List<SWPDropdownViewModel> GetGenderList()
        {
            List<SWPDropdownViewModel> genderList = new List<SWPDropdownViewModel>();
            genderList.Add(new SWPDropdownViewModel { Text = SWPConstant.Male });
            genderList.Add(new SWPDropdownViewModel { Text = SWPConstant.Female });
            return genderList;
        }

        public List<SWPDropdownViewModel> GetOccupationList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from f in dbContext.OccupationMsts
                           select new SWPDropdownViewModel
                           {
                               Id = f.occupationId,
                               Text = f.occupation
                           }).ToList();
                return lst;
            }
        }

        public List<SWPDropdownViewModel> GetCICRequestTypeList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from cfg in dbContext.Common_Config
                           where cfg.Is_Active == 1 && cfg.Category.ToLower().Equals(SWPCategoryType.CIC.ToLower())
                           select new SWPDropdownViewModel
                           {
                               Id = cfg.Id,
                               Text = cfg.Name
                           }).ToList();
                return lst;
            }
        }

        public List<SWPDropdownViewModel> GetTransferTypeList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from transTy in dbContext.Transfer_Type
                           where transTy.Parent_Id == null && transTy.Is_Active == true
                           select new SWPDropdownViewModel
                           {
                               Text = transTy.type,
                               Id = transTy.Id
                           }).ToList();
                return lst;
            }
        }

        public List<SWPDropdownViewModel> GetTransferSubTypeList(int transferTypeId)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from transTy in dbContext.Transfer_Type
                           where transTy.Parent_Id == transferTypeId && transTy.Is_Active == true
                           select new SWPDropdownViewModel
                           {
                               Text = transTy.type,
                               Id = transTy.Id
                           }).ToList();
                return lst;
            }
        }

        public List<SWPDropdownViewModel> GetCompanyMemberTypeList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from cfg in dbContext.Common_Config
                           where cfg.Is_Active == 1 && cfg.Category.ToLower().Equals(SWPCategoryType.Director.ToLower())
                           select new SWPDropdownViewModel
                           {
                               Id = cfg.Id,
                               Text = cfg.Name
                           }).ToList();
                return lst;
            }
        }

        public List<SWPDropdownViewModel> GetBankListBySchemeId(int schemeId)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from prop in dbContext.SchemeBankTrans
                           where prop.IsActive == true && prop.schemeId == schemeId
                           select new SWPDropdownViewModel
                           {
                               Id = prop.BankMst.bankId,
                               Text = prop.BankMst.bankName
                           }).ToList();
                return lst;
            }
        }

        public List<SWPDropdownViewModel> GetFormTypeList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Is_Active == 1 && config.Category == "FormType"
                            select new SWPDropdownViewModel
                            {
                                Id = config.Id,
                                Text = config.Name
                            }).ToList();
                return list;
            }
        }

        public List<SWPDropdownViewModel> GetFormSubTypeList(string formtype)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Is_Active == 1 && config.Category == formtype
                            select new SWPDropdownViewModel
                            {
                                Id = config.Id,
                                Text = config.Name
                            }).ToList();
                return list;
            }
        }

        public List<SWPDropdownViewModel> GetApplicantTypeList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var list = (from config in dbContext.Common_Config
                            where config.Is_Active == 1 && config.Category == "ApplicantType"
                            select new SWPDropdownViewModel
                            {
                                Id = config.Id,
                                Text = config.Name
                            }).ToList();
                return list;
            }
        }

        public List<SWPDropdownViewModel> GetCompanyTypeByCategory(string typeName)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from config in dbContext.Common_Config
                           where config.Is_Active == 1 && config.Category.ToLower() == typeName.ToLower()
                           select new SWPDropdownViewModel
                           {
                               Id = config.Id,
                               Text = config.Name
                           }).ToList();
                return lst;
            }
        }

        public List<SWPDropdownViewModel> getSectorsList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from sector in dbContext.SectorMsts
                           where sector.IsActive == true
                           select new SWPDropdownViewModel
                           {
                               Id = sector.sectorId,
                               Text = sector.sectorName
                           }).ToList();
                return lst;
            }
        }

        public List<SWPDropdownViewModel> GetDirectorTypeList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from cfg in dbContext.Common_Config
                           where cfg.Is_Active == 1 && cfg.Category.ToLower().Equals(SWPCategoryType.Director.ToLower())
                           select new SWPDropdownViewModel
                           {
                               Id = cfg.Id,
                               Text = cfg.Name
                           }).ToList();
                return lst;
            }
        }

        public List<SWPDropdownViewModel> GetMaritalStatusList()
        {
            List<SWPDropdownViewModel> maritialStatus = new List<SWPDropdownViewModel>();
            foreach (int value in Enum.GetValues(typeof(SWPEnum.MaritalStatus)))
            {
                maritialStatus.Add(new SWPDropdownViewModel
                {
                    Text = Enum.GetName(typeof(SWPEnum.MaritalStatus), value),
                    Id = value
                });
            }
            return maritialStatus;
        }

        public List<SWPDropdownViewModel> GetCategoryList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from prop in dbContext.QuotaMsts
                           where prop.IsActive == true
                           select new SWPDropdownViewModel
                           {
                               Id = prop.quotaId,
                               Text = prop.quotaName,
                           }).ToList();
                return lst;
            }
        }

        public List<SWPDropdownViewModel> GetBankList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from prop in dbContext.BankMsts
                           where prop.IsActive == true
                           select new SWPDropdownViewModel
                           {
                               Id = prop.bankId,
                               Text = prop.bankName
                           }).ToList();
                return lst;
            }
        }


        public DataSourceResult GetDropDownListAsDataSource(DataSourceRequest request, SWPDropdownViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (model.FilterType == "Scheme" && model.ReturnType == "All")
                {
                    var list = (from scheme in dbContext.SchemeMsts
                                where (model.Id == 0 || scheme.schemeId == model.Id)
                                && scheme.IsActive == true
                                select new SWPDropdownViewModel
                                {
                                    Id = scheme.schemeId,
                                    Text = scheme.schemeName,
                                    Value = scheme.schemeName
                                });
                    return list != null ? list.ToDataSourceResult(request) : null;
                }
                else if (model.FilterType == "Scheme" && model.ReturnType == "Online")
                {
                    string[] _SchemeList = ConfigurationManager.AppSettings["OnlineSchemeList"].Split(',');
                    var list = (from scheme in dbContext.SchemeMsts
                                where (model.Id == 0 || scheme.schemeId == model.Id) && _SchemeList.Contains(scheme.schemeId.ToString())
                                && scheme.IsActive == true
                                select new SWPDropdownViewModel
                                {
                                    Id = scheme.schemeId,
                                    Text = scheme.schemeName,
                                    Value = scheme.schemeName
                                });
                    return list != null ? list.ToDataSourceResult(request) : null;
                }
                else if (model.FilterType == "Department" && model.ReturnType == "All")
                {
                    var list = (from dept in dbContext.DepartmentMsts
                                where (model.Id == 0 || dept.departmentId == model.Id)
                                select new SWPDropdownViewModel
                                {
                                    Id = dept.departmentId,
                                    Text = dept.departmentName,
                                    Value = dept.departmentName
                                });
                    return list != null ? list.ToDataSourceResult(request) : null;
                }
                else if (model.FilterType == "Department" && model.ReturnType == "Scheme")
                {
                    var list = (from dept in dbContext.DepartmentMsts
                                join scheme in dbContext.SchemeDepartmentTrans on dept.departmentId equals scheme.departmentId
                                where dept.IsActive == true && scheme.schemeId == model.SchemeId
                                select new SWPDropdownViewModel
                                {
                                    Id = dept.departmentId,
                                    Text = dept.departmentName,
                                    Value = dept.departmentName,
                                    SchemeId = model.SchemeId
                                });
                    request.Filters.RemoveAt(0);
                    return list != null ? list.ToDataSourceResult(request) : null;
                }
                else if (model.FilterType == "Sector")
                {
                    var list = (from sector in dbContext.SectorMsts
                                where (model.Id == 0 || sector.sectorId == model.Id)
                                select new SWPDropdownViewModel
                                {
                                    Id = sector.sectorId,
                                    Text = sector.sectorName,
                                    Value = sector.sectorName
                                });
                    return list != null ? list.ToDataSourceResult(request) : null;
                }
                else if (model.FilterType == "Block")
                {
                    var list = (from block in dbContext.BlockMsts
                                where (model.Id == 0 || block.blockId == model.Id)
                                select new SWPDropdownViewModel
                                {
                                    Id = block.blockId,
                                    Text = block.blockName,
                                    Value = block.blockName
                                });
                    return list != null ? list.ToDataSourceResult(request) : null;
                }
                else if (model.FilterType == "ApplicationFormId" && model.ReturnType == "Allotment")
                {
                    var list = (from f in dbContext.OnlineApplicationDetails
                                join t in dbContext.OnlineApplicationDetails_trans on f.onlineapplicationId equals t.ServiceRefId
                                join n in dbContext.NICsingalwindowSystems on f.onlineapplicationId equals n.onlineapplicationId
                                where f.schemeId == model.SchemeId && t.TranStatus == 1 && t.status == 1 && n.Status_Code == 11
                                && (model.DepartmentId == null || f.departmentId == model.DepartmentId)
                                select new SWPDropdownViewModel
                                {
                                    Id = f.onlineapplicationId,
                                    Text = f.onlineapplicationId.ToString()
                                });
                    request.Filters.RemoveAt(0);
                    return list.ToDataSourceResult(request);
                }
                else if (model.FilterType == "ConstitutionType")
                {
                    var list = (from config in dbContext.Common_Config
                               where config.Is_Active == 1 && config.Category.ToLower() == model.ReturnType.ToLower()
                               && model.Id == 0 || config.Id == model.Id
                               select new SWPDropdownViewModel
                               {
                                   Id = config.Id,
                                   Text = config.Name
                               });
                    //request.Filters.RemoveAt(0);
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    return null;
                }
            }
        }


    }
}
