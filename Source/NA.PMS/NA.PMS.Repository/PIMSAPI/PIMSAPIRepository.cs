using NA.PMS.Model;
using System.Collections.Generic;
using System.Linq;

namespace NA.PMS.Repository
{
    public class PIMSAPIRepository : IPIMSAPIRepository
    {
        public PIMSAPIRepository() { }

        public PropertyViewModel GetPropertyDetailsByAddress(PropertyViewModel model)
        {
            var propertyDetails = new PropertyViewModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                if (!string.IsNullOrEmpty(model.SectorName) && !string.IsNullOrEmpty(model.BlockName) && !string.IsNullOrEmpty(model.PlotNo))
                {
                    propertyDetails = (from details in dbContext.ViewAllPropertyDetails
                                       where details.sectorName.ToLower() == model.SectorName &&
                                       details.blockName.ToLower() == model.BlockName &&
                                       details.propertyNo.ToLower() == model.PlotNo
                                       select new PropertyViewModel
                                       {
                                           RegistrationId = details.rid,
                                           Applicant = details.ApplicantName,
                                           ApplicantAddress = details.tCorrespondanceAdd,
                                           BlockName = details.blockName,
                                           SectorName = details.sectorName,
                                           PlotNo = details.propertyNo,
                                           TotalArea = details.totalArea,
                                           Department = details.departmentName
                                       }).FirstOrDefault();
                }
            }
            return propertyDetails;
        }

        public List<DDList> GetApproversListByApplicationId(int logedInUserId, int applicationId)
        {
            var userList = new List<DDList>();
            using (var dbContext = new NoidaPMSEntities())
            {
                userList = (from user in dbContext.UmUserMasters
                            join role in dbContext.UmUserMasterRoles on user.UserRefId equals role.UserRefId
                            join app in dbContext.UmRoleAppTrans on role.RoleId equals app.RoleId
                            where user.IsActive == true && app.ApplicationId == applicationId && user.UserRefId != logedInUserId
                            select new DDList
                            {
                                id = user.UserRefId,
                                text = user.UserName
                            }).Distinct().ToList();
            }
            return userList;
        }


        public PropertyViewModel SaveCustomerInfo(PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var _epass = new EpassMaster
                {
                    Applicant = model.Applicant,
                    MobileNo = model.MobileNo,
                    Email = model.Email,
                    Comment = model.Comment,
                    VehicleType = model.DepartmentId == null ? null : model.DepartmentId.ToString(),
                    VehicleNo = model.TotalInstallment == null ? null : model.TotalInstallment.ToString(),
                    RCNo = model.PropertyUpdateId == null ? null : model.PropertyUpdateId.ToString()
                };

                dbContext.EpassMasters.Add(_epass);
                dbContext.SaveChanges();
                model.StatusId = 200;
            }
            return model;
        }
    }
}
