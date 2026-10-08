using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
//using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using NA.PMS.Common;
using NA.PMS.NICService.Context;

namespace NA.PMS.NICServices
{
    public class NiveshMitraHelper
    {
        public NiveshMitraHelper()
        {

        }

        public DataSourceResult GetServiceListAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new PIMSEntitiesContext())
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

        public List<DropdownViewModel> GetStatusList()
        {
            using (var dbContext = new PIMSEntitiesContext())
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

        public DataSourceResult GetStatusMasterAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new PIMSEntitiesContext())
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

        //public LoginUserDetail GetLoginUserDetails(int userId)
        //{
        //    LoginUserDetail loginUserDetail = null;
        //    using (var dbContext = new PIMSEntitiesContext())
        //    {
        //        loginUserDetail = (from user in dbContext.UmUserMasters
        //                           where user.UserRefId == userId
        //                           select new LoginUserDetail
        //                           {
        //                               UserRefId = user.UserRefId,
        //                               UserName = user.UserName,
        //                               FirstName = user.FirstName,
        //                               LastName = user.LastName,
        //                               MiddleName = user.MiddleName,
        //                               IsActive = user.IsActive
        //                           }).FirstOrDefault();
        //    }
        //    return loginUserDetail;
        //}

        public List<DropdownViewModel> GetDepartmentList()
        {
            using (var dbContext = new PIMSEntitiesContext())
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

        public List<DropdownViewModel> GetSubDepartmentList(int departmentId)
        {
            using (var dbContext = new PIMSEntitiesContext())
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

        public List<DropdownViewModel> GetFirmStatusList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from cfg in dbContext.Common_Config
                           where cfg.Is_Active == 1 && cfg.Category.ToLower().Equals(SWPCategoryType.FirmStatus.ToLower())
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
            foreach (int value in Enum.GetValues(typeof(SWPEnum.MortgageType)))
            {
                mortgageType.Add(new DropdownViewModel
                {
                    Text = Enum.GetName(typeof(SWPEnum.MortgageType), value),
                    Id = value
                });
            }
            return mortgageType;
        }

        public List<DropdownViewModel> GetNOCStatusList()
        {
            List<DropdownViewModel> statusList = new List<DropdownViewModel>();
            foreach (int value in Enum.GetValues(typeof(SWPEnum.MortgageStatus)))
            {
                statusList.Add(new DropdownViewModel
                {
                    Text = Enum.GetName(typeof(SWPEnum.MortgageStatus), value),
                    Id = value
                });
            }
            return statusList;
        }

        public List<DropdownViewModel> GetGPAStatusList()
        {
            List<DropdownViewModel> gender = new List<DropdownViewModel>();
            foreach (int value in Enum.GetValues(typeof(SWPEnum.ActiveStatus)))
            {
                gender.Add(new DropdownViewModel
                {
                    Text = Enum.GetName(typeof(SWPEnum.ActiveStatus), value),
                    Id = value
                });
            }
            return gender;
        }

        public List<DropdownViewModel> GetGenderList()
        {
            List<DropdownViewModel> genderList = new List<DropdownViewModel>();
            genderList.Add(new DropdownViewModel { Text = SWPConstant.Male });
            genderList.Add(new DropdownViewModel { Text = SWPConstant.Female });
            return genderList;
        }

        public List<DropdownViewModel> GetOccupationList()
        {
            using (var dbContext = new PIMSEntitiesContext())
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

        public List<DropdownViewModel> GetCICRequestTypeList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from cfg in dbContext.Common_Config
                           where cfg.Is_Active == 1 && cfg.Category.ToLower().Equals(SWPCategoryType.CIC.ToLower())
                           select new DropdownViewModel
                           {
                               Id = cfg.Id,
                               Text = cfg.Name
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetTransferTypeList()
        {
            using (var dbContext = new PIMSEntitiesContext())
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
            using (var dbContext = new PIMSEntitiesContext())
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

        public List<DropdownViewModel> GetCompanyMemberTypeList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from cfg in dbContext.Common_Config
                           where cfg.Is_Active == 1 && cfg.Category.ToLower().Equals(SWPCategoryType.Director.ToLower())
                           select new DropdownViewModel
                           {
                               Id = cfg.Id,
                               Text = cfg.Name
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetBankListBySchemeId(int schemeId)
        {
            using (var dbContext = new PIMSEntitiesContext())
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

        public List<DropdownViewModel> GetFormTypeList()
        {
            using (var dbContext = new PIMSEntitiesContext())
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

        public List<DropdownViewModel> GetFormSubTypeList(string formtype)
        {
            using (var dbContext = new PIMSEntitiesContext())
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

        public List<DropdownViewModel> GetApplicantTypeList()
        {
            using (var dbContext = new PIMSEntitiesContext())
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

        public List<DropdownViewModel> GetCompanyTypeByCategory(string typeName)
        {
            using (var dbContext = new PIMSEntitiesContext())
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
            using (var dbContext = new PIMSEntitiesContext())
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

        public List<DropdownViewModel> GetDirectorTypeList()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from cfg in dbContext.Common_Config
                           where cfg.Is_Active == 1 && cfg.Category.ToLower().Equals(SWPCategoryType.Director.ToLower())
                           select new DropdownViewModel
                           {
                               Id = cfg.Id,
                               Text = cfg.Name
                           }).ToList();
                return lst;
            }
        }

        public List<DropdownViewModel> GetMaritalStatusList()
        {
            List<DropdownViewModel> maritialStatus = new List<DropdownViewModel>();
            foreach (int value in Enum.GetValues(typeof(SWPEnum.MaritalStatus)))
            {
                maritialStatus.Add(new DropdownViewModel
                {
                    Text = Enum.GetName(typeof(SWPEnum.MaritalStatus), value),
                    Id = value
                });
            }
            return maritialStatus;
        }

        public List<DropdownViewModel> GetCategoryList()
        {
            using (var dbContext = new PIMSEntitiesContext())
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

        public List<DropdownViewModel> GetBankList()
        {
            using (var dbContext = new PIMSEntitiesContext())
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


    }
}
