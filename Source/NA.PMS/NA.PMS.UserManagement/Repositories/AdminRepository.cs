using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Model;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NA.PMS.Common;
using NA.PMS.Common.Extension;
using System.Configuration;
using System.IO;
using System.Resources;
using System.Globalization;
using System.Collections;

namespace NA.PMS.UserManagement
{
    public class AdminRepository : IAdminRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public AdminRepository()
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

        public DataSourceResult GetUsersAsDataSource(DataSourceRequest request, NDAUserViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var users = (from u in dbContext.UmUserMasters
                             join m in dbContext.UmUserMasterRoles on u.UserRefId equals m.UserRefId
                             join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId
                             where u.UserRefId != userInfo.UserID && (model.UserRefId == null || u.UserRefId == model.UserRefId)
                             orderby u.UserRefId descending
                             select new NDAUserViewModel
                             {
                                 Id = u.UserRefId,
                                 UserRefId = u.UserRefId,
                                 UserName = u.UserName,
                                 FirstName = u.FirstName,
                                 MiddleName = u.MiddleName,
                                 LastName = u.LastName,
                                 Email = u.Email,
                                 MobileNo = u.Mobile,
                                 RoleName = r.RoleName,
                                 RoleId = r.RoleId,
                                 IsActive = u.IsActive,
                                 UserOptionalId = u.OptionalId,
                                 UserProfile = u.UserProfile,
                                 UserProfileId = u.UserProfileId,
                                 DepartmentNameList = (from ma in dbContext.UmDepartmentMasters join d in dbContext.UmUserDepartmentTrans on ma.DepartmentId equals d.DepartmentId join us in dbContext.UmUserMasters on d.UserRefId equals us.UserRefId where us.UserRefId == u.UserRefId select ma.DepartmentName).ToList(),
                                 DepartmentIdList = (from ma in dbContext.UmDepartmentMasters join d in dbContext.UmUserDepartmentTrans on ma.DepartmentId equals d.DepartmentId join us in dbContext.UmUserMasters on d.UserRefId equals us.UserRefId where us.UserRefId == u.UserRefId select ma.DepartmentId).ToList(),
                                 FullName = u.FirstName + " " + ((u.MiddleName == null || u.MiddleName == "") ? u.LastName : (u.MiddleName + " " + u.LastName))
                             }).ToList();

                foreach (var us in users)
                {
                    if (us.DepartmentNameList.Count > 0)
                    {
                        foreach (var dept in us.DepartmentNameList)
                        {
                            us.DepartmentsName = us.DepartmentsName + dept + ",";
                        }
                        us.DepartmentsName = us.DepartmentsName.TrimEnd(',');
                    }
                }
                return users.ToDataSourceResult(request);
            }
        }

        public List<NDACheckBoxViewModel> GetDepartmentList()
        {
            var lst = new List<NDACheckBoxViewModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                lst = (from d in dbContext.UmDepartmentMasters
                       where d.Status == true
                       select new NDACheckBoxViewModel
                       {
                           Id = d.DepartmentId,
                           CheckBoxName = d.DepartmentName,
                           IsChecked = false
                       }).ToList();
            }
            return lst;
        }

        public NDAUserViewModel SaveAuthorityUserDetail(NDAUserViewModel user)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingUser = dbContext.UmUserMasters.Where(i => i.UserRefId == user.Id).FirstOrDefault();
                if (existingUser != null) //Updating existing User
                {
                    existingUser.Email = user.Email;
                    existingUser.FirstName = user.FirstName;
                    existingUser.MiddleName = user.MiddleName;
                    existingUser.LastName = user.LastName;
                    existingUser.UserProfileId = user.UserProfileId;
                    existingUser.UserProfile = user.UserProfile;
                    existingUser.OptionalId = user.UserOptionalId;
                    existingUser.Mobile = user.MobileNo;
                    existingUser.ModifiedBy = userInfo.UserID.ToString();
                    existingUser.ModifiedDate = DateTime.Now;
                    var depts = dbContext.UmUserDepartmentTrans.Where(d => d.UserRefId == user.Id).ToList();
                    foreach (var item in depts)
                    {
                        dbContext.UmUserDepartmentTrans.Remove(item);
                    }

                    foreach (var item in user.DepartmentList)
                    {
                        if (item.IsChecked == true)
                        {
                            var dept = new UmUserDepartmentTran();
                            dept.UserRefId = user.Id;
                            dept.DepartmentId = item.Id;
                            dept.Status = true;
                            dept.CreatedBy = userInfo.UserID.ToString();
                            dept.CreateDate = DateTime.Now;
                            dbContext.UmUserDepartmentTrans.Add(dept);
                        }
                    }

                    var subdepts = dbContext.UmUserSubDepartmentTrans.Where(s => s.UserRefId == user.Id).ToList();
                    foreach (var subd in subdepts)
                    {
                        dbContext.UmUserSubDepartmentTrans.Remove(subd);
                    }

                    foreach (var item in user.DepartmentList)
                    {
                        if (item.IsChecked == true)
                        {
                            foreach (var sdept in user.SubDepartmentList)
                            {
                                if (sdept.IsChecked == true)
                                {
                                    var sdeptrans = new UmUserSubDepartmentTran();
                                    sdeptrans.UserRefId = user.Id;
                                    sdeptrans.DepartmentId = item.Id;
                                    sdeptrans.SubDepartmentId = sdept.Id;
                                    sdeptrans.Status = true;
                                    sdeptrans.ModifiedBy = userInfo.UserID;
                                    sdeptrans.ModifiedDate = DateTime.Now;
                                    dbContext.UmUserSubDepartmentTrans.Add(sdeptrans);
                                }
                            }
                        }
                    }
                    dbContext.SaveChanges();
                }
                else //Adding new User
                {
                    var newUser = new UmUserMaster();
                    newUser.Email = user.Email;
                    newUser.FirstName = user.FirstName;
                    newUser.MiddleName = user.MiddleName;
                    newUser.LastName = user.LastName;
                    newUser.Mobile = user.MobileNo;
                    newUser.UserName = user.UserName;
                    newUser.UserProfileId = user.UserProfileId;
                    newUser.UserProfile = user.UserProfile;
                    newUser.OptionalId = user.UserOptionalId;
                    newUser.IsActive = true;
                    var password = CreatePassword();
                    newUser.Password = password.ToMD5HashForPassword();
                    newUser.CreatedBy = userInfo.UserID.ToString();
                    newUser.CreatedDate = DateTime.Now;
                    dbContext.UmUserMasters.Add(newUser);
                    var datacheck = dbContext.UmUserMasters.Where(x => x.UserName == user.UserName).FirstOrDefault();
                    if (datacheck == null)
                    {
                        dbContext.SaveChanges();
                        foreach (var item in user.DepartmentList)
                        {
                            if (item.IsChecked == true)
                            {
                                var dept = new UmUserDepartmentTran();
                                dept.UserRefId = newUser.UserRefId;
                                dept.DepartmentId = item.Id;
                                dept.Status = true;
                                dept.CreatedBy = userInfo.UserID.ToString();
                                dept.CreateDate = DateTime.Now;
                                dbContext.UmUserDepartmentTrans.Add(dept);
                            }
                        }

                        foreach (var item in user.DepartmentList)
                        {
                            if (item.IsChecked == true)
                            {
                                foreach (var sdept in user.SubDepartmentList)
                                {
                                    if (sdept.IsChecked == true)
                                    {
                                        var sdeptrans = new UmUserSubDepartmentTran();
                                        sdeptrans.UserRefId = user.Id;
                                        sdeptrans.DepartmentId = item.Id;
                                        sdeptrans.SubDepartmentId = sdept.Id;
                                        sdeptrans.Status = true;
                                        sdeptrans.CreatedBy = userInfo.UserID;
                                        sdeptrans.CreatedDate = DateTime.Now;
                                        dbContext.UmUserSubDepartmentTrans.Add(sdeptrans);
                                    }
                                }
                            }
                        }

                        dbContext.SaveChanges();
                        ////send mail to new user 
                        //var body = "Dear User,<br><br>You are successfully registered with mynoida.in. Your user name is: " + user.UserName + " and password is " + password + ".<br><br>Regards,<br>http://mynoida.in";
                        //EmailHelper emailHelper = new EmailHelper();
                        //emailHelper.Send(user.email, "Registration Completed", body);

                        //send message to new user on mobile
                        //var msg = "Dear User, You are successfully registered with mynoida.in. Your user name is: " + user.empID + " and password is " + password + ". Regards,http://mynoida.in";
                        var msg = string.Format(NAMessages.SendCredentials, user.UserName, password);
                        //SMSSend(user.MobileNo.ToString(), msg);
                        ApplicationHelper.SendSMS(user.MobileNo, msg);
                        flag = true;
                    }
                }
            }
            return user;
        }

        private string CreatePassword()
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int length = 8;
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public NDAUserViewModel GetAuthorityUserDetailById(NDAUserViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == model.UserRefId);
                model.Id = user.UserRefId;
                model.UserName = user.UserName;
                model.FirstName = user.FirstName;
                model.MiddleName = user.MiddleName;
                model.LastName = user.LastName;
                model.MobileNo = user.Mobile;
                model.Email = user.Email;
                model.UserOptionalId = user.OptionalId;
                model.UserProfile = user.UserProfile;
                model.UserProfileId = user.UserProfileId;

                model.DepartmentList = (from d in dbContext.UmUserDepartmentTrans
                                        where d.UserRefId == model.UserProfileId
                                        select new NDACheckBoxViewModel
                                        {
                                            Id = dbContext.UmDepartmentMasters.Where(depMas => depMas.DepartmentId == d.DepartmentId).Select(mas => mas.DepartmentId).FirstOrDefault(),
                                            CheckBoxName = dbContext.UmDepartmentMasters.Where(depMas => depMas.DepartmentId == d.DepartmentId).Select(mas => mas.DepartmentName).FirstOrDefault(),
                                            IsChecked = true
                                        }).ToList();

                var subdepts = (from sdept in dbContext.UmUserSubDepartmentTrans
                                where sdept.UserRefId == model.UserProfileId
                                select new NDACheckBoxViewModel
                                {
                                    Id = dbContext.DepartmentSubMsts.FirstOrDefault(d => d.SubDepartmentId == sdept.SubDepartmentId).SubDepartmentId,
                                    CheckBoxName = dbContext.DepartmentSubMsts.FirstOrDefault(d => d.SubDepartmentId == sdept.SubDepartmentId).SubDepartment,
                                    IsChecked = true
                                }).ToList();
                if (subdepts == null || subdepts.Count == 0)
                {
                    model.SubDepartmentList = new List<NDACheckBoxViewModel> { 
                        new NDACheckBoxViewModel { Id = 1, CheckBoxId = 1, CheckBoxName = "Property", IsChecked = false }, 
                        new NDACheckBoxViewModel { Id = 2, CheckBoxId = 2, CheckBoxName = "Account", IsChecked = false } 
                    };
                }
                else
                {
                    model.SubDepartmentList = subdepts;
                }
            }

            return model;
        }

        public NDAUserViewModel ValidateUserDetail(NDAUserViewModel model)
        {

            using (var dbContext = new NoidaPMSEntities())
            {
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "validateusername")
                {
                    var rec = new UmUserMaster();
                    if (model.UserRefId != 0)
                    {
                        rec = dbContext.UmUserMasters.Where(i => i.UserName.ToLower() == model.UserName.ToLower() && i.UserRefId != model.UserRefId).FirstOrDefault();
                    }
                    else
                    {
                        rec = dbContext.UmUserMasters.Where(i => i.UserName.ToLower() == model.UserName.ToLower()).FirstOrDefault();
                    }
                    if (rec != null)
                    {
                        model.ReturnTypeId = ReturnType.Exist;
                    }
                }
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "validatemobile")
                {
                    var rec = new UmUserMaster();
                    if (model.UserRefId != 0)
                    {
                        rec = dbContext.UmUserMasters.Where(i => i.Mobile == model.MobileNo && i.UserRefId != model.UserRefId).FirstOrDefault();
                    }
                    else
                    {
                        rec = dbContext.UmUserMasters.Where(i => i.Mobile == model.MobileNo).FirstOrDefault();
                    }
                    if (rec != null)
                    {
                        model.ReturnTypeId = ReturnType.Exist;
                    }
                }
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "validateemail")
                {
                    var rec = new UmUserMaster();
                    if (model.UserRefId != 0)
                    {
                        rec = dbContext.UmUserMasters.Where(i => i.Email == model.Email && i.UserRefId != model.UserRefId).FirstOrDefault();
                    }
                    else
                    {
                        rec = dbContext.UmUserMasters.Where(i => i.Email == model.Email).FirstOrDefault();
                    }
                    if (rec != null)
                    {
                        model.ReturnTypeId = ReturnType.Exist;
                    }
                }
            }
            return model;
        }

        public DataSourceResult GetDepartmentListByIdAsDataSource(DataSourceRequest request, NDAUserViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<NDAUserViewModel> list = new List<NDAUserViewModel>();
                var departmentIdList = dbContext.UmUserDepartmentTrans.Where(i => i.UserRefId == model.UserRefId).ToList();
                if (departmentIdList != null && departmentIdList.Count > 0)
                {
                    string department = string.Empty;
                    foreach (var dept in departmentIdList)
                    {
                        department = department + dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == dept.DepartmentId).departmentName + ",";
                    }
                    department.TrimEnd(',');
                    model.DepartmentsName = department;
                    list.Add(model);
                }
                return list.ToDataSourceResult(request);
            }
        }

        public NDAUserViewModel MapUserToRoleAndApplication(NDAUserViewModel model)
        {
            int result = 0;
            using (var dbContext = new NoidaPMSEntities())
            {
                var appID = dbContext.UmRoleAppTrans.Where(app => app.RoleId == model.RoleId).Select(app => app.ApplicationId).FirstOrDefault();
                if (appID != null)
                {
                    var existingRole = (from us in dbContext.UmUserMasterRoles
                                        join ro in dbContext.UmRoleAppTrans on us.RoleId equals ro.RoleId
                                        join rm in dbContext.UmRoleMasters on ro.RoleId equals rm.RoleId
                                        where ro.ApplicationId == appID && us.UserRefId == model.UserRefId
                                        select new
                                        {
                                            us = us,
                                            roleType = rm.RoleType
                                        }).FirstOrDefault();
                    if (existingRole != null)
                    {
                        if (existingRole.roleType.ToLower() == Constants.Admin.ToLower())
                        {
                            result = 2;
                        }
                        //result = 0; //User is already attached to a Role for the given Application.
                    }
                    else
                    {
                        var mapping = new UmUserMasterRole();
                        mapping.UserRefId = model.UserRefId;
                        mapping.RoleId = model.RoleId;
                        mapping.CreatedBy = userInfo.UserID.ToString();
                        mapping.CreatedDate = DateTime.Now;
                        mapping.isActive = true;
                        dbContext.UmUserMasterRoles.Add(mapping);
                        dbContext.SaveChanges();
                        result = 1;
                    }
                }
            }
            return model;
        }


        public DataSourceResult GetCustomerListAsDataSource(DataSourceRequest request, NDAUserViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                string filePath = ConfigurationManager.AppSettings["DocumentFilesPath"].ToString();
                var list = (from user in dbContext.CustomerMsts
                            from role in dbContext.UmRoleMasters.Where(r => r.RoleId == user.RoleId).DefaultIfEmpty()
                            where (model.IsActive == null || user.IsActive == model.IsActive)
                            && (model.UserName == null || user.UserName == model.UserName)
                            && (model.DepartmentId == null || user.DepartmentId == model.DepartmentId)
                            select new NDAUserViewModel
                            {
                                Id = user.Id,
                                UserName = user.UserName,
                                Password = user.Password,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                MobileNo = user.MobileNo,
                                TxtPropertyId = user.PropertyId,
                                CreatedDate = user.CreatedDate,
                                ModifiedDate = user.ModifiedDate,
                                RoleId = user.RoleId,
                                RoleName = role.RoleName,
                                LastPasswordChangeDate = user.PasswordChangeDate,
                                FullName = user.FirstName + " " + user.LastName,
                                UserIdFileName = user.IdFileName == null ? string.Empty : filePath + user.PropertyId + "/" + user.IdFileName,
                                UserIdFileType = user.IdFileType,
                                PropertyFileName = user.PropertyFileName == null ? string.Empty : filePath + user.PropertyId + "/" + user.PropertyFileName,
                                PropertyFileType = user.PropertyFileType,
                                DepartmentId = user.DepartmentId,
                                Department = (user.DepartmentId == null || user.DepartmentId <= 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(x => x.departmentId == user.DepartmentId).departmentName,
                                Remarks = user.Remarks,
                                Email = user.Email,
                                StatusId = user.StatusId,
                                //Status = user.StatusId == 1 ? true : false,
                                IsApproved = user.StatusId == 1 ? true : false,
                                IsActive = user.IsActive,
                                IsLocked = user.IsLocked,
                                IsFirstTimeActivated = user.IsFirstTimeActivated == null ? false : user.IsFirstTimeActivated,
                                IsRejected = (user.StatusId == null || user.StatusId != NAStatusId.Rejected) ? false : true,
                                PropertyNo = user.Sector + "/" + (string.IsNullOrEmpty(user.Block) ? string.Empty : user.Block + "-") + user.PlotNo,
                                IsIdFileUploaded = string.IsNullOrEmpty(user.IdFileName) ? false : true,
                                IsPropertyFileUploaded = string.IsNullOrEmpty(user.PropertyFileName) ? false : true,
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetRoleListAsDataSource(DataSourceRequest request, NDARoleViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var roles = (from rol in dbContext.UmRoleMasters
                             select new NDARoleViewModel
                             {
                                 Id = rol.RoleId,
                                 RoleId = rol.RoleId,
                                 RoleName = rol.RoleName,
                                 RoleType = rol.RoleType,
                                 RoleInDepartment = rol.RoleInDepartment,
                                 RoleDescription = rol.RoleDescription,
                                 CreatedBy = rol.CreatedBy,
                                 CreatedDate = rol.CreatedDate,
                                 IsActive = rol.IsActive,
                                 UserName = string.Empty
                             });
                return roles.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetApplicationListAsDataSource(DataSourceRequest request, NDARoleViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var apps = (from app in dbContext.UmApplicationMasters
                            select new NDARoleViewModel
                            {
                                Id = app.ApplicationId,
                                ApplicationId = app.ApplicationId,
                                AplicationName = app.ApplicationName,
                                ApplicationUrl = app.ApplicationUrl,
                                IsActive = app.IsActive,
                                CreatedBy = app.CreatedBy,
                                CreatedDate = app.CreatedDate,
                                UserName = string.Empty
                            });
                return apps.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetMenuListAsDataSource(DataSourceRequest request, NDARoleViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var menulist = (from menu in dbContext.UmMenuMasters
                                select new NDARoleViewModel
                                {
                                    Id = menu.MenuId,
                                    MenuId = menu.MenuId,
                                    MenuPathId = menu.MenuPathId,
                                    MenuName = menu.MenuName,
                                    MenuParentId = menu.MenuParentId,
                                    ParentMenuName = menu.MenuParentId == null ? "Parent Menu" : dbContext.UmMenuMasters.FirstOrDefault(f => f.MenuId == menu.MenuParentId).MenuName,
                                    IsActive = menu.IsActive,
                                    UserName = string.Empty,
                                    ApplicationId = menu.ApplicationId,
                                    AplicationName = menu.ApplicationId != null ? dbContext.UmApplicationMasters.FirstOrDefault(a=>a.ApplicationId==menu.ApplicationId).ApplicationName : string.Empty
                                });
                return menulist.ToDataSourceResult(request);
            }
        }


        public NDAUserViewModel SaveAuthorityCustomer(NDAUserViewModel model, HttpPostedFileBase userIdFile, HttpPostedFileBase propertyFile)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (userIdFile != null && userIdFile.ContentLength > 0)
                {
                    model.UserIdFileName = userIdFile.FileName;
                }
                if (propertyFile != null && propertyFile.ContentLength > 0)
                {
                    model.PropertyFileName = propertyFile.FileName;
                }

                var existingCustomer = dbContext.CustomerMsts.FirstOrDefault(us => us.UserName == model.UserName);
                if (existingCustomer != null)
                {
                    //creating password
                    //var newPassword = "password123";
                    //existingCustomer.Pasword = newPassword.ToMD5HashForPasswordPIS();

                    existingCustomer.RoleId = 2;
                    existingCustomer.DepartmentId = model.DepartmentId;
                    existingCustomer.Sector = model.SectorName;
                    existingCustomer.Block = model.BlockName;
                    existingCustomer.PlotNo = model.PlotNo;
                    existingCustomer.ModifiedDate = DateTime.Now;
                    existingCustomer.MobileNo = model.MobileNo;
                    existingCustomer.Email = model.Email;
                    existingCustomer.PropertyId = model.PropertyId.ToString();
                    existingCustomer.FirstName = model.FirstName;
                    existingCustomer.IdFileName = model.UserIdFileName;
                    existingCustomer.IdFileType = model.UserIdFileType;
                    existingCustomer.PropertyFileName = model.PropertyFileName;
                    existingCustomer.PropertyFileType = model.PropertyFileType;
                    existingCustomer.SecurityQuestion = model.SecurityQuestion;
                    existingCustomer.SecurityAnswer = model.SecurityAnswer;
                    existingCustomer.IsActive = true;
                    existingCustomer.StatusId = NAStatusId.Pending;
                    existingCustomer.IsFirstTimeActivated = false;
                    existingCustomer.Remarks = model.Remarks;

                    model.ReturnTypeId = ReturnType.Updated;
                }
                else
                {
                    CustomerMst ctxCustomer = new CustomerMst();
                    ctxCustomer.RegistrationId = model.RegistrationId;
                    ctxCustomer.UserName = model.UserName;
                    ctxCustomer.FirstName = model.FullName;
                    ctxCustomer.DepartmentId = model.DepartmentId;
                    ctxCustomer.PropertyId = model.PropertyId.ToString();
                    ctxCustomer.RoleId = 2;
                    ctxCustomer.Sector = model.SectorName;
                    ctxCustomer.Block = model.BlockName;
                    ctxCustomer.PlotNo = model.PlotNo;
                    ctxCustomer.MobileNo = model.MobileNo;
                    ctxCustomer.Email = model.Email;
                    ctxCustomer.CreatedDate = DateTime.Now;
                    ctxCustomer.CreatedBy = model.UserName;
                    ctxCustomer.IdFileName = model.UserIdFileName;
                    ctxCustomer.IdFileType = model.UserIdFileType;
                    ctxCustomer.PropertyFileName = model.PropertyFileName;
                    ctxCustomer.PropertyFileType = model.PropertyFileType;
                    ctxCustomer.SecurityQuestion = model.SecurityQuestion;
                    ctxCustomer.SecurityAnswer = model.SecurityAnswer;
                    ctxCustomer.IsActive = false;
                    ctxCustomer.StatusId = NAStatusId.Initiated;
                    ctxCustomer.IsFirstTimeActivated = false;
                    ctxCustomer.Remarks = model.Remarks;
                    //creating password
                    var newPassword = "Noida" + DateTime.Now.Year;
                    ctxCustomer.Password = newPassword.ToMD5HashForPasswordPIS();

                    dbContext.CustomerMsts.Add(ctxCustomer);

                    var msg1 = NAMessages.PIS_registration_1;
                    ApplicationHelper.SendSMS(model.MobileNo, msg1);

                    var msg2 = string.Format(NAMessages.PIS_registration_2, ctxCustomer.PropertyId);
                }
                dbContext.SaveChanges();
                var dflag = SaveCustomerDocument(model, userIdFile, propertyFile);
                model.ReturnTypeId = ReturnType.Saved;
            }
            return model;
        }

        private int SaveCustomerDocument(NDAUserViewModel model, HttpPostedFileBase userIdFile, HttpPostedFileBase propertyFile)
        {
            int flag = ReturnType.None;
            var directoryPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FilePath"]).ToString() + model.UserName;
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            if (userIdFile != null && userIdFile.ContentLength > 0)
            {
                userIdFile.SaveAs(HttpContext.Current.Server.MapPath((ConfigurationManager.AppSettings["FilePath"]) + model.UserName).ToString() + "/" + userIdFile.FileName);
                flag = ReturnType.Saved;
            }
            if (propertyFile != null && propertyFile.ContentLength > 0)
            {
                propertyFile.SaveAs(HttpContext.Current.Server.MapPath((ConfigurationManager.AppSettings["FilePath"]) + model.UserName).ToString() + "/" + userIdFile.FileName);
                flag = ReturnType.Saved;
            }
            return flag;
        }


        public NDARoleViewModel SaveAuthorityRole(NDARoleViewModel model)
        {
            throw new NotImplementedException();
        }

        public NDARoleViewModel SaveAuthorityApplication(NDARoleViewModel model)
        {
            throw new NotImplementedException();
        }

        public NDARoleViewModel SaveMenuByApplication(NDARoleViewModel model)
        {
            throw new NotImplementedException();
        }

        public NDAUserViewModel SaveEmployeeInfoByActionType(NDAUserViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "lockunlock")
                {
                    var user = dbContext.UmUserMasters.Where(u => u.UserRefId == model.UserRefId).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.IsActive == false)
                        {
                            user.IsActive = true;
                        }
                        else
                            user.IsActive = false;
                        dbContext.SaveChanges();
                        model.ReturnTypeId = ReturnType.Success;
                    }
                }
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "removeuser")
                {
                    var mapping = dbContext.UmUserMasterRoles.Where(u => u.UserRefId == model.UserRefId && u.RoleId == model.RoleId).FirstOrDefault();
                    var rolapp = dbContext.UmRoleAppTrans.Where(r => r.RoleId == model.RoleId && r.ApplicationId == model.ApplicationId).FirstOrDefault();
                    if (mapping != null)
                    {
                        dbContext.UmUserMasterRoles.Remove(mapping);
                        //dbContext.UmRoleAppTrans.Remove(rolapp);
                        dbContext.SaveChanges();
                        //flag = true;
                        model.ReturnTypeId = ReturnType.Success;
                    }
                }
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "changepassword")
                {
                    var user = dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == model.UserRefId);
                    if (user != null)
                    {
                        //var password = model.Password.ToMD5HashForPassword();
                        user.Password = model.NewPassword.ToMD5HashForPassword();
                        user.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();

                        if (user.Mobile != null)
                        {
                            //var msg = "Dear User, You have successfully changed your password. Your new password is " + password + " Regards, http://mynoida.in";
                            var msg = string.Format(NAMessages.PasswordChange, user.Password);
                            ApplicationHelper.SendSMS(user.Mobile, msg);
                        }
                        model.ReturnTypeId = ReturnType.Success;
                    }
                }
            }
            return model;
        }


        public NDAUserViewModel SaveCustomerInfoByActionType(NDAUserViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "lockunlock")
                {
                    var user = dbContext.CustomerMsts.Where(u => u.UserName == model.UserName).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.IsLocked == false)
                        {
                            user.IsLocked = true;
                        }
                        else
                            user.IsLocked = false;
                        dbContext.SaveChanges();
                        model.ReturnTypeId = ReturnType.Success;
                    }
                }
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "activestatus")
                {
                    var user = dbContext.CustomerMsts.Where(u => u.UserName == model.UserName).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.IsActive == false)
                        {
                            user.IsActive = true;
                        }
                        else
                            user.IsActive = false;
                        dbContext.SaveChanges();
                        model.ReturnTypeId = ReturnType.Success;
                    }
                }
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "saveremarks")
                {
                    var user = dbContext.CustomerMsts.Where(u => u.UserName == model.UserName).FirstOrDefault();
                    user.Remarks = user.Remarks + model.Remarks;
                    dbContext.SaveChanges();
                    model.ReturnTypeId = ReturnType.Success;
                }
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "approvecustomer")
                {
                    var customer = dbContext.CustomerMsts.Where(i => i.UserName.ToLower() == model.UserName.ToLower()).FirstOrDefault();
                    customer.StatusId = NAStatusId.Approved;
                    dbContext.SaveChanges();
                    model.ReturnTypeId = ReturnType.Success;
                }
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "resetpassword")
                {
                    var customer = dbContext.CustomerMsts.Where(i => i.UserName.ToLower() == model.UserName.ToLower()).FirstOrDefault();
                    if (customer != null)
                    {
                        customer.Password = model.NewPassword.ToMD5HashForPassword();
                        customer.PasswordChangeDate = DateTime.Now;
                        dbContext.SaveChanges();
                        if (customer.MobileNo != null)
                        {
                            var msg = string.Format(NAMessages.PasswordChange, model.NewPassword);
                            ApplicationHelper.SendSMS(customer.MobileNo, msg);
                        }
                        model.ReturnTypeId = ReturnType.Success;
                    }
                }
            }
            return model;
        }


        public DataSourceResult GetApplicationRoleDetailByIdAsDataSource(DataSourceRequest request, NDARoleViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from rmt in dbContext.UmRoleAppTrans
                            join roles in dbContext.UmRoleMasters on rmt.RoleId equals roles.RoleId
                            join apps in dbContext.UmApplicationMasters on rmt.ApplicationId equals apps.ApplicationId
                            join umr in dbContext.UmUserMasterRoles on rmt.RoleId equals umr.RoleId
                            join users in dbContext.UmUserMasters on umr.UserRefId equals users.UserRefId
                            where umr.UserRefId == model.UserRefId
                            select new NDARoleViewModel
                              {
                                  RoleId = roles.RoleId,
                                  RoleType = roles.RoleType,
                                  RoleName = roles.RoleName,
                                  ApplicationId = apps.ApplicationId,
                                  AplicationName = apps.ApplicationName,
                                  CreatedBy = users.FirstName + " " + users.MiddleName + " " + users.LastName,
                                  UserName = users.UserName
                              });
                return list != null ? list.ToDataSourceResult(request) : null;
            }
        }

        public DataSourceResult GetMasterDataListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "department")
                {
                    var list = (from dept in dbContext.DepartmentMsts
                                where dept.IsActive == true && (model.DepartmentId == null || dept.departmentId == model.DepartmentId)
                                select new DropdownViewModel
                                {
                                    Id = dept.departmentId,
                                    Text = dept.departmentName,
                                    Value = dept.departmentName
                                });
                    return list.ToDataSourceResult(request);
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "application")
                {
                    var list = (from apps in dbContext.UmApplicationMasters
                                where apps.IsActive == true && (model.RefId == null || apps.ApplicationId == model.RefId)
                                select new DropdownViewModel
                                {
                                    Id = apps.ApplicationId,
                                    Text = apps.ApplicationName,
                                    Value = apps.ApplicationName
                                });
                    return list.ToDataSourceResult(request);
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "rid")
                {
                    var ridlist = (from alotment in dbContext.AllotmentMasters
                                   where alotment.isActive == 1 && (model.RegistrationId == null || alotment.rid == model.RegistrationId)
                                   select new DropdownViewModel
                                   {
                                       Id = alotment.rid,
                                       Text = alotment.rid.ToString(),
                                       Value = alotment.rid.ToString()
                                   });
                    return ridlist.ToDataSourceResult(request);
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "sector")
                {
                    var list = (from sector in dbContext.SectorMsts
                                where sector.IsActive == true && (model.SectorId == null || sector.sectorId == model.SectorId)
                                select new DropdownViewModel
                                {
                                    Id = sector.sectorId,
                                    Text = sector.sectorName,
                                    Value = sector.sectorName,
                                    SectorId = sector.sectorId
                                });
                    return list.ToDataSourceResult(request);
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "block")
                {
                    var list = (from block in dbContext.BlockMsts
                                where block.IsActive == true && (model.BlockId == null || block.blockId == model.BlockId)
                                select new DropdownViewModel
                                {
                                    Id = block.blockId,
                                    Text = block.blockName,
                                    Value = block.blockName,
                                    BlockId = block.blockId
                                });
                    return list.ToDataSourceResult(request);
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "status")
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
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "securityquestion")
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
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "rolesbyapplication")
                {
                    var list = (from r in dbContext.UmRoleMasters
                                join ra in dbContext.UmRoleAppTrans on r.RoleId equals ra.RoleId
                                join a in dbContext.UmApplicationMasters on ra.ApplicationId equals a.ApplicationId
                                where r.IsActive == true && a.ApplicationId == model.ApplicationId
                                select new DropdownViewModel
                                {
                                    Id = r.RoleId,
                                    Text = r.RoleName,
                                    ApplicationId = a.ApplicationId
                                });
                    return list.ToDataSourceResult(request);
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "username")
                {
                    var list = (from u in dbContext.UmUserMasters
                                //where !selectSA.Contains(u.UserRefId)
                                select new DropdownViewModel
                                     {
                                         Id = u.UserRefId,
                                         Text = u.UserName
                                     });
                    return list.ToDataSourceResult(request);
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "parentmenu")
                {
                    var list = (from u in dbContext.UmMenuMasters
                                where u.MenuParentId == null
                                select new DropdownViewModel
                                {
                                    Id = u.MenuId,
                                    Text = u.MenuName
                                });
                    return list.ToDataSourceResult(request);
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "menu")
                {
                    var list = (from u in dbContext.UmMenuMasters
                                where u.MenuParentId == model.MenuId
                                select new DropdownViewModel
                                {
                                    Id = u.MenuId,
                                    Text = u.MenuName
                                });
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    return null;
                }
            }
        }

        public NDAUserViewModel ValidateCustomerRegistration(NDAUserViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var exuser = dbContext.CustomerMsts.FirstOrDefault(u => u.UserName == model.RegistrationId.ToString());
                if (exuser == null)
                {
                    var data = dbContext.AllotmentMasters.FirstOrDefault(c => c.rid == model.RegistrationId && c.isActive == Constants.Active);
                    if (data != null)
                    {
                        var customer = (from alotment in dbContext.AllotmentMasters
                                        join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                                        join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                                        where alotment.rid == model.RegistrationId
                                        select new NDAUserViewModel
                                        {
                                            RegistrationId = alotment.rid,
                                            UserName = alotment.rid.ToString(),
                                            DepartmentId = alotment.departmentId,
                                            Department = alotment.DepartmentMst.departmentName,
                                            PropertyId = alotment.propertyId,
                                            FullName = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : (aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName),
                                            MobileNo = aplicant.tMobileNumber,
                                            Email = aplicant.tEmail,
                                            SectorId = property.sectorId,
                                            SectorName = property.SectorMst.sectorName,
                                            BlockId = property.blockId,
                                            BlockName = (property.blockId == null || property.blockId == 0) ? "NA" : property.BlockMst.blockName,
                                            PlotNo = property.propertyNo
                                        }).FirstOrDefault();
                        return customer;
                    }
                    else
                    {
                        model.ReturnTypeId = ReturnType.NotExist;
                        return model;
                    }
                }
                else
                {
                    model.ReturnTypeId = ReturnType.Exist;
                    return model;
                }
            }
        }

        public NDAUserViewModel ValidateEmployeeInfoByActionType(NDAUserViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "validateusername")
                {
                    var rec = new UmUserMaster();
                    if (model.UserRefId != 0)
                    {
                        rec = dbContext.UmUserMasters.Where(i => i.UserName.ToLower() == model.UserName.ToLower() && i.UserRefId != model.UserRefId).FirstOrDefault();
                    }
                    else
                    {
                        rec = dbContext.UmUserMasters.Where(i => i.UserName.ToLower() == model.UserName.ToLower()).FirstOrDefault();
                    }
                    if (rec != null)
                    {
                        model.ReturnTypeId = ReturnType.Exist;
                    }
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "validatemobilenumber")
                {
                    var rec = new UmUserMaster();
                    if (model.UserRefId != 0)
                    {
                        rec = dbContext.UmUserMasters.Where(i => i.Mobile == model.MobileNo && i.UserRefId != model.UserRefId).FirstOrDefault();
                    }
                    else
                    {
                        rec = dbContext.UmUserMasters.Where(i => i.Mobile == model.MobileNo).FirstOrDefault();
                    }
                    if (rec != null)
                    {
                        model.ReturnTypeId = ReturnType.Exist;
                    }
                }
                return model;
            }
        }

        public NDAUserViewModel GetCustomerDetailById(NDAUserViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                string filePath = ConfigurationManager.AppSettings["DocumentFilesPath"].ToString();
                var data = (from user in dbContext.CustomerMsts
                            from role in dbContext.UmRoleMasters.Where(r => r.RoleId == user.RoleId).DefaultIfEmpty()
                            where user.UserName == model.UserName
                            select new NDAUserViewModel
                            {
                                Id = user.Id,
                                UserName = user.UserName,
                                Password = user.Password,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                MobileNo = user.MobileNo,
                                TxtPropertyId = user.PropertyId,
                                CreatedDate = user.CreatedDate,
                                ModifiedDate = user.ModifiedDate,
                                RoleId = user.RoleId,
                                RoleName = role.RoleName,
                                LastPasswordChangeDate = user.PasswordChangeDate,
                                FullName = user.FirstName + " " + user.LastName,
                                UserIdFileName = user.IdFileName == null ? string.Empty : filePath + user.PropertyId + "/" + user.IdFileName,
                                UserIdFileType = user.IdFileType,
                                PropertyFileName = user.PropertyFileName == null ? string.Empty : filePath + user.PropertyId + "/" + user.PropertyFileName,
                                PropertyFileType = user.PropertyFileType,
                                DepartmentId = user.DepartmentId,
                                Department = (user.DepartmentId == null || user.DepartmentId <= 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(x => x.departmentId == user.DepartmentId).departmentName,
                                Remarks = user.Remarks,
                                Email = user.Email,
                                StatusId = user.StatusId,
                                IsApproved = user.StatusId == 1 ? true : false,
                                IsActive = user.IsActive,
                                IsLocked = user.IsLocked,
                                IsFirstTimeActivated = user.IsFirstTimeActivated == null ? false : user.IsFirstTimeActivated,
                                IsRejected = (user.StatusId == null || user.StatusId != NAStatusId.Rejected) ? false : true,
                                PropertyNo = user.Sector + "/" + (string.IsNullOrEmpty(user.Block) ? string.Empty : user.Block + "-") + user.PlotNo,
                                IsIdFileUploaded = string.IsNullOrEmpty(user.IdFileName) ? false : true,
                                IsPropertyFileUploaded = string.IsNullOrEmpty(user.PropertyFileName) ? false : true,
                                SecurityQuestion = user.SecurityQuestion,
                                SecurityAnswer = user.SecurityAnswer,
                                SectorName = user.Sector,
                                SectorId = !string.IsNullOrEmpty(user.Sector) ? dbContext.SectorMsts.FirstOrDefault(s => s.sectorName.Trim() == user.Sector.Trim()).sectorId : dbContext.SectorMsts.FirstOrDefault(s => s.sectorName.Trim() == "NA").sectorId,
                                BlockName = user.Block,
                                BlockId = !string.IsNullOrEmpty(user.Block) ? dbContext.BlockMsts.FirstOrDefault(s => s.blockName.Trim() == user.Block.Trim()).blockId : dbContext.BlockMsts.FirstOrDefault(s => s.blockName.Trim() == "NA").blockId,
                                PlotNo = user.PlotNo
                            }).FirstOrDefault();
                return data;
            }
        }


        public DataSourceResult GetMappedUsersListByApplicationRoleAsDataSource(DataSourceRequest request, NDAUserViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var users = (from u in dbContext.UmUserMasters
                             join m in dbContext.UmUserMasterRoles on u.UserRefId equals m.UserRefId
                             join r in dbContext.UmRoleMasters on m.RoleId equals r.RoleId
                             join ra in dbContext.UmRoleAppTrans on r.RoleId equals ra.RoleId
                             where (model.RoleId == null || ra.RoleId == model.RoleId)
                                    && (model.ApplicationId == null || ra.ApplicationId == model.ApplicationId)
                             orderby u.UserRefId descending
                             select new NDAUserViewModel
                            {
                                Id = u.UserRefId,
                                UserRefId = u.UserRefId,
                                UserName = u.UserName,
                                Email = u.Email,
                                FirstName = u.FirstName,
                                MiddleName = u.MiddleName,
                                LastName = u.LastName,
                                MobileNo = u.Mobile,
                                EmployeeCode = u.UserName,
                                RoleName = r.RoleName,
                                RoleId = r.RoleId,
                                ApplicationId = ra.ApplicationId.Value,
                                IsActive = u.IsActive.Value,
                                DepartmentNameList = (from ma in dbContext.UmDepartmentMasters join d in dbContext.UmUserDepartmentTrans on ma.DepartmentId equals d.DepartmentId join us in dbContext.UmUserMasters on d.UserRefId equals us.UserRefId where us.UserRefId == u.UserRefId select ma.DepartmentName).ToList(),
                                FullName = u.FirstName + " " + (!string.IsNullOrEmpty(u.MiddleName) ? (u.MiddleName + " " + u.LastName) : u.LastName)
                            }).ToList();

                foreach (var us in users)
                {
                    if (us.DepartmentNameList.Count > 0)
                    {
                        foreach (var dept in us.DepartmentNameList)
                        {
                            us.DepartmentsName = us.DepartmentsName + dept + ",";
                        }
                        us.DepartmentsName = us.DepartmentsName.TrimEnd(',');
                    }
                }
                return users.ToDataSourceResult(request);
            }
        }


        public NDARoleViewModel GetMenuListByApplicationRoleAsHtmlContent(NDARoleViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var approle = dbContext.UmRoleAppTrans.FirstOrDefault(r => r.ApplicationId == model.ApplicationId && r.RoleId == model.RoleId);
                if (approle != null)
                {
                    model.ParentMenuList = GetParentMenuListByApplicationRole(model);
                    if (model.ParentMenuList != null)
                    {
                        foreach (var parent in model.ParentMenuList)
                        {
                            var menuList = GetMenuListByParentMenuId(parent);
                            parent.MenuList = menuList;
                        }

                        model.MenuInHtml = GetMenuListByApplicationRoleAsString(model);
                    }

                    model.AssignedMenuList = GetAssignedMenuListByApplicationRole(model);
                }

                return model;
            }
        }

        private List<NDAMenuViewModel> GetAssignedMenuListByApplicationRole(NDARoleViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from rmt in dbContext.UmRoleMasterTrans
                            join mnu in dbContext.UmMenuMasters on rmt.MenuId equals mnu.MenuId
                            where rmt.ApplicationId == model.ApplicationId && rmt.RoleId == model.RoleId && mnu.MenuParentId == null
                            select new NDAMenuViewModel
                            {
                                Id = rmt.MenuId,
                                MenuId = rmt.MenuId,
                                ParentMenuId = mnu.MenuParentId,
                                ParentMenuName = mnu.MenuName,
                                //ApplicationId = app.ApplicationId,
                                //ApplicationName = app.ApplicationName,
                                //RoleId = rol.RoleId,
                                //RoleName = rol.RoleName,
                                IsChecked = false,
                                IsRead = rmt.IsRead,
                                IsWrite = rmt.IsWrite,
                                IsUpdate = rmt.IsUpdate,
                                IsDelete = rmt.Isdelete,
                                IsActive = mnu.IsActive
                            }).ToList();
                foreach (var menu in list)
                {
                    var menuList = GetMenuListByParentMenuId(menu);
                    menu.MenuList = menuList;
                }

                return list;
            }
        }

        private List<NDAMenuViewModel> GetParentMenuListByApplicationRole(NDARoleViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from menu in dbContext.UmMenuMasters
                            where menu.ApplicationId == model.ApplicationId && menu.MenuParentId == null
                            select new NDAMenuViewModel
                            {
                                Id = menu.MenuId,
                                MenuId = menu.MenuId,
                                MenuPathId = menu.MenuPathId,
                                MenuName = menu.MenuName,
                                //ParentMenuId = menu.MenuId,
                                //ParentMenuName = menu.MenuName,
                                IsChecked = false,
                                IsRead = false,
                                IsWrite = false,
                                IsUpdate = false,
                                IsDelete = false,
                                IsActive = menu.IsActive
                            }).ToList();
                return list;
            }
        }

        private List<NDAMenuViewModel> GetMenuListByParentMenuId(NDAMenuViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from menu in dbContext.UmMenuMasters
                            where menu.MenuParentId == model.MenuId
                            select new NDAMenuViewModel
                            {
                                Id = menu.MenuId,
                                MenuId = menu.MenuId,
                                MenuPathId = menu.MenuPathId,
                                MenuName = menu.MenuName,
                                ParentMenuId = menu.MenuParentId,
                                ParentMenuName = menu.MenuParentId != null ? dbContext.UmMenuMasters.FirstOrDefault(m=>m.MenuId == menu.MenuParentId).MenuName : null,
                                IsChecked = false,
                                IsRead = false,
                                IsWrite = false,
                                IsUpdate = false,
                                IsDelete = false,
                                IsActive = menu.IsActive
                            }).ToList();
                return list;
            }
        }

        private string GetMenuListByApplicationRoleAsString(NDARoleViewModel model)
        {
            //var roleList = _menuMappingService.GetMenusByRoleId();

            //var selectedMenus = _menuMappingService.GetSelectedMenus();
            //selectedMenus = selectedMenus.Where(x => x.roleId == roleId).ToList();
            //roleList = roleList.Where(x => x.applicationId == appID).ToList();

            //var applications = roleList.Select(e => e.applicationId).Distinct().ToList();
            //var lstMenu = roleList.Where(x => x.menuParentId == null).ToList();
            //var lstSubMenu = roleList.Where(x => x.menuParentId != null).ToList();

            //// Making Html for Menu Mapping

            string htmlString = string.Empty;
            string formBoxString = string.Empty;
            
            //htmlString = htmlString + "<h2 class='col-md-12 blck-heading'><table><tr><td><b>Application(s)</b></td><td><b>Read</b></td><td><b>Write</b></td><td><b>Modify</b></td><td><b>Delete</b></td></tr></table></h2>";
            htmlString = htmlString + "<div class='form-bx'>";
            htmlString = htmlString + "<div class='row'>"+
                "<div class='col-md-4 col-sm-3 col-xs-12 form-group'><label><b>Parent Menu</b></label></div>" +
                "<div class='col-md-2 col-sm-3 col-xs-12 form-group'><span><b>Read</b></span></div>" +
                "<div class='col-md-2 col-sm-3 col-xs-12 form-group'><span><b>Write</b></span></div>" +
                "<div class='col-md-2 col-sm-3 col-xs-12 form-group'><span><b>Modify</b></span></div>" +
                "<div class='col-md-2 col-sm-3 col-xs-12 form-group'><span><b>Delete</b></span></div>" +
                "</div>";
            foreach (var parent in model.ParentMenuList)
            {
                htmlString = htmlString + "<div class='row'>" + 
                    "<div class='col-md-4 col-sm-3 col-xs-12 form-group'><label>" + parent.ParentMenuName + "</label></div>" +
                    "<div class='col-md-2 col-sm-3 col-xs-12 form-group'><input type='checkbox' class='pr-checkbox' name='pr-" + parent.MenuId + "' value='" + parent.IsRead + "' checked='"+parent.IsChecked + "'/></div>" +
                    "<div class='col-md-2 col-sm-3 col-xs-12 form-group'><input type='checkbox' class='pr-checkbox' name='pw-" + parent.MenuId + "' value='" + parent.IsWrite + "' checked='" + parent.IsChecked + "'/></div>" +
                    "<div class='col-md-2 col-sm-3 col-xs-12 form-group'><input type='checkbox' class='pr-checkbox' name='pu-" + parent.MenuId + "' value='" + parent.IsUpdate + "' checked='" + parent.IsChecked + "'/></div>" +
                    "<div class='col-md-2 col-sm-3 col-xs-12 form-group'><input type='checkbox' class='pr-checkbox' name='pd-" + parent.MenuId + "' value='" + parent.IsDelete + "' checked='" + parent.IsChecked + "'/></div>" + 
                    "</div>";
                htmlString = htmlString + "<div class='col-md-12'>";
                foreach (var menu in parent.MenuList)
                {
                    htmlString = htmlString + "<div class='row' style='padding-left:30px;'>" + 
                        "<div class='col-md-4 col-sm-3 col-xs-12 form-group'><label>" + menu.MenuName + "</label></div>" +
                        "<div class='col-md-2 col-sm-3 col-xs-12 form-group'><input type='checkbox' class='m-checkbox' name='r-" + menu.MenuId + "' value='" + menu.IsRead + "'/></div>" +
                        "<div class='col-md-2 col-sm-3 col-xs-12 form-group'><input type='checkbox' class='m-checkbox' name='w-" + menu.MenuId + "' value='" + menu.IsWrite + "'/></div>" +
                        "<div class='col-md-2 col-sm-3 col-xs-12 form-group'><input type='checkbox' class='m-checkbox' name='u-" + menu.MenuId + "' value='" + menu.IsUpdate + "'/></div>" +
                        "<div class='col-md-2 col-sm-3 col-xs-12 form-group'><input type='checkbox' class='m-checkbox' name='d-" + menu.MenuId + "' value='" + menu.IsDelete + "'/></div>" +
                        "</div>";
                }
                htmlString = htmlString + "</div>";
            }
            htmlString = htmlString + "</div>";

            return htmlString;

            //foreach (var appId in applications)
            //{
            //    string appName = roleList.Where(l => l.applicationId == appId).Select(l => l.applicationName).FirstOrDefault();
            //    formBoxString = formBoxString + "<fieldset><span id='imgApp' class='imgApp menu-map-icn-minus'>" + appName + "</span>";
            //    int countMenu = 1;
            //    foreach (var menu in lstMenu)
            //    {
            //        if (menu.applicationId == appId)
            //        {
            //            var selectedMenu = selectedMenus.Where(x => x.menuPathId == menu.menuPathId && x.applicationId == menu.applicationId).FirstOrDefault();
            //            if (selectedMenu == null)
            //            {
            //                formBoxString = formBoxString + "<br><fieldset><div id='dvMenu' class='dvMenu'><fieldset><table><tbody><tr><td><span id='imgMenu' class='imgMenu submenu-map-icn-minus'>" + menu.menuName + "</span></td><td ><input class='chkMenuRead' id='chkRead_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "' type='checkbox' /></td><td ><input class='chkMenuWrite' id='chkWrite_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "'  type='checkbox' /></td><td ><input class='chkMenuModify' id='chkModify_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "'  type='checkbox'/></td><td ><input class='chkMenuDelete' id='chkDelete_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "' type='checkbox' /></td> </tr></tbody></table><div id='dvSubMenu' class='dvSubMenu'><table>";
            //                int countSubMenu = 1;
            //                foreach (var subMenu in lstSubMenu)
            //                {
            //                    var selectedSubMenu = selectedMenus.Where(x => x.menuPathId == subMenu.menuPathId && x.applicationId == menu.applicationId).FirstOrDefault();
            //                    if (selectedSubMenu == null)
            //                    {
            //                        if (subMenu.menuParentId == menu.menuPathId)
            //                        {
            //                            formBoxString = formBoxString + "<tr><td>" + subMenu.menuName + "</td><td><input class='chkSubMenuRead' id='chkRead_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox' /></td><td><input class='chkSubMenuWrite' id='chkWrite_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuModify' id='chkModify_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuDelete' id='chkDelete_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td></tr>";
            //                            countSubMenu++;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        string checkeboxCheckedSMRead = "";
            //                        string checkeboxCheckedSMWrite = "";
            //                        string checkeboxCheckedSMModify = "";
            //                        string checkeboxCheckedSMDelete = "";
            //                        if (selectedSubMenu.IsRead == true)
            //                            checkeboxCheckedSMRead = "checked";
            //                        if (selectedSubMenu.IsWrite == true)
            //                            checkeboxCheckedSMWrite = "checked";
            //                        if (selectedSubMenu.IsModify == true)
            //                            checkeboxCheckedSMModify = "checked";
            //                        if (selectedSubMenu.IsDelete == true)
            //                            checkeboxCheckedSMDelete = "checked";
            //                        if (subMenu.menuParentId == menu.menuPathId)
            //                        {
            //                            formBoxString = formBoxString + "<tr><td>" + subMenu.menuName + "</td><td><input class='chkSubMenuRead'" + checkeboxCheckedSMRead + " id='chkRead_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox' /></td><td><input class='chkSubMenuWrite'" + checkeboxCheckedSMWrite + " id='chkWrite_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuModify'" + checkeboxCheckedSMModify + " id='chkModify_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuDelete'" + checkeboxCheckedSMDelete + " id='chkDelete_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td></tr>";
            //                            countSubMenu++;
            //                        }
            //                    }
            //                }
            //                formBoxString = formBoxString + "</table></fieldset></div></fieldset>";
            //                countMenu++;
            //            }
            //            else
            //            {
            //                string checkeboxCheckedRead = "";
            //                string checkeboxCheckedWrite = "";
            //                string checkeboxCheckedModify = "";
            //                string checkeboxCheckedDelete = "";
            //                if (selectedMenu.IsRead == true)
            //                    checkeboxCheckedRead = "checked";
            //                if (selectedMenu.IsWrite == true)
            //                    checkeboxCheckedWrite = "checked";
            //                if (selectedMenu.IsModify == true)
            //                    checkeboxCheckedModify = "checked";
            //                if (selectedMenu.IsDelete == true)
            //                    checkeboxCheckedDelete = "checked";

            //                formBoxString = formBoxString + "<br><fieldset><div id='dvMenu' class='dvMenu'><fieldset><table><tbody><tr><td><span id='imgMenu' class='imgMenu submenu-map-icn-minus'>" + menu.menuName + "</span></td><td ><input class='chkMenuRead'" + checkeboxCheckedRead + " id='chkRead_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "' type='checkbox' /></td><td ><input class='chkMenuWrite'" + checkeboxCheckedWrite + " id='chkWrite_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "'  type='checkbox' /></td><td ><input class='chkMenuModify'" + checkeboxCheckedModify + " id='chkModify_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "'  type='checkbox'/></td><td ><input class='chkMenuDelete'" + checkeboxCheckedDelete + " id='chkDelete_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "' type='checkbox' /></td> </tr></tbody></table><div id='dvSubMenu' class='dvSubMenu'><table >";

            //                int countSubMenu = 1;
            //                foreach (var subMenu in lstSubMenu)
            //                {
            //                    var selectedSubMenu = selectedMenus.Where(x => x.menuPathId == subMenu.menuPathId && x.applicationId == menu.applicationId).FirstOrDefault();
            //                    if (selectedSubMenu == null)
            //                    {
            //                        if (subMenu.menuParentId == menu.menuPathId)
            //                        {
            //                            formBoxString = formBoxString + "<tr><td>" + subMenu.menuName + "</td><td><input class='chkSubMenuRead' id='chkRead_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox' /></td><td><input class='chkSubMenuWrite' id='chkWrite_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuModify' id='chkModify_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'</td><td><input class='chkSubMenuDelete' id='chkDelete_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td></tr>";
            //                            countSubMenu++;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        string checkeboxCheckedSMRead = "";
            //                        string checkeboxCheckedSMWrite = "";
            //                        string checkeboxCheckedSMModify = "";
            //                        string checkeboxCheckedSMDelete = "";
            //                        if (selectedSubMenu.IsRead == true)
            //                            checkeboxCheckedSMRead = "checked";
            //                        if (selectedSubMenu.IsWrite == true)
            //                            checkeboxCheckedSMWrite = "checked";
            //                        if (selectedSubMenu.IsModify == true)
            //                            checkeboxCheckedSMModify = "checked";
            //                        if (selectedSubMenu.IsDelete == true)
            //                            checkeboxCheckedSMDelete = "checked";
            //                        if (subMenu.menuParentId == menu.menuPathId)
            //                        {
            //                            formBoxString = formBoxString + "<tr><td>" + subMenu.menuName + "</td><td><input class='chkSubMenuRead'" + checkeboxCheckedSMRead + " id='chkRead_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox' /></td><td><input class='chkSubMenuWrite'" + checkeboxCheckedSMWrite + " id='chkWrite_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuModify'" + checkeboxCheckedSMModify + " id='chkModify_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuDelete'" + checkeboxCheckedSMDelete + " id='chkDelete_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td></tr>";
            //                            countSubMenu++;
            //                        }
            //                    }
            //                }
            //                formBoxString = formBoxString + "</table></fieldset></div></fieldset>";
            //                countMenu++;
            //            }
            //        }
            //    }
            //    formBoxString = formBoxString + "</fieldset>";
            //}
            //return htmlString + formBoxString + "</div>";
        }

        //private string GetMenuListByApplicationRoleAsHtmlContent(NDARoleViewModel model)
        //{
        //    var roleList = _menuMappingService.GetMenusByRoleId();

        //    var selectedMenus = _menuMappingService.GetSelectedMenus();
        //    selectedMenus = selectedMenus.Where(x => x.roleId == roleId).ToList();
        //    roleList = roleList.Where(x => x.applicationId == appID).ToList();

        //    var applications = roleList.Select(e => e.applicationId).Distinct().ToList();
        //    var lstMenu = roleList.Where(x => x.menuParentId == null).ToList();
        //    var lstSubMenu = roleList.Where(x => x.menuParentId != null).ToList();

        //    // Making Html for Menu Mapping

        //    string htmlString = string.Empty;
        //    string formBoxString = string.Empty;
        //    //htmlString = htmlString + "<hr style='border-top: 1px solid black'><table><tr><td><b>Application(s)</b></td><td><b>Read</b></td><td><b>Write</b></td><td><b>Modify</b></td><td><b>Delete</b></td></tr></table><hr style='border-bottom: 1px solid black'>";
        //    htmlString = htmlString + "<h2 class='col-md-12 blck-heading'><table><tr><td><b>Application(s)</b></td><td><b>Read</b></td><td><b>Write</b></td><td><b>Modify</b></td><td><b>Delete</b></td></tr></table></h2>";
        //    htmlString = htmlString + "<div class='form-bx'>";

        //    foreach (var appId in applications)
        //    {
        //        string appName = roleList.Where(l => l.applicationId == appId).Select(l => l.applicationName).FirstOrDefault();
        //        formBoxString = formBoxString + "<fieldset><span id='imgApp' class='imgApp menu-map-icn-minus'>" + appName + "</span>";
        //        int countMenu = 1;
        //        foreach (var menu in lstMenu)
        //        {
        //            if (menu.applicationId == appId)
        //            {
        //                var selectedMenu = selectedMenus.Where(x => x.menuPathId == menu.menuPathId && x.applicationId == menu.applicationId).FirstOrDefault();
        //                if (selectedMenu == null)
        //                {
        //                    formBoxString = formBoxString + "<br><fieldset><div id='dvMenu' class='dvMenu'><fieldset><table><tbody><tr><td><span id='imgMenu' class='imgMenu submenu-map-icn-minus'>" + menu.menuName + "</span></td><td ><input class='chkMenuRead' id='chkRead_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "' type='checkbox' /></td><td ><input class='chkMenuWrite' id='chkWrite_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "'  type='checkbox' /></td><td ><input class='chkMenuModify' id='chkModify_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "'  type='checkbox'/></td><td ><input class='chkMenuDelete' id='chkDelete_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "' type='checkbox' /></td> </tr></tbody></table><div id='dvSubMenu' class='dvSubMenu'><table>";
        //                    int countSubMenu = 1;
        //                    foreach (var subMenu in lstSubMenu)
        //                    {
        //                        var selectedSubMenu = selectedMenus.Where(x => x.menuPathId == subMenu.menuPathId && x.applicationId == menu.applicationId).FirstOrDefault();
        //                        if (selectedSubMenu == null)
        //                        {
        //                            if (subMenu.menuParentId == menu.menuPathId)
        //                            {
        //                                formBoxString = formBoxString + "<tr><td>" + subMenu.menuName + "</td><td><input class='chkSubMenuRead' id='chkRead_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox' /></td><td><input class='chkSubMenuWrite' id='chkWrite_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuModify' id='chkModify_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuDelete' id='chkDelete_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td></tr>";
        //                                countSubMenu++;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            string checkeboxCheckedSMRead = "";
        //                            string checkeboxCheckedSMWrite = "";
        //                            string checkeboxCheckedSMModify = "";
        //                            string checkeboxCheckedSMDelete = "";
        //                            if (selectedSubMenu.IsRead == true)
        //                                checkeboxCheckedSMRead = "checked";
        //                            if (selectedSubMenu.IsWrite == true)
        //                                checkeboxCheckedSMWrite = "checked";
        //                            if (selectedSubMenu.IsModify == true)
        //                                checkeboxCheckedSMModify = "checked";
        //                            if (selectedSubMenu.IsDelete == true)
        //                                checkeboxCheckedSMDelete = "checked";
        //                            if (subMenu.menuParentId == menu.menuPathId)
        //                            {
        //                                formBoxString = formBoxString + "<tr><td>" + subMenu.menuName + "</td><td><input class='chkSubMenuRead'" + checkeboxCheckedSMRead + " id='chkRead_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox' /></td><td><input class='chkSubMenuWrite'" + checkeboxCheckedSMWrite + " id='chkWrite_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuModify'" + checkeboxCheckedSMModify + " id='chkModify_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuDelete'" + checkeboxCheckedSMDelete + " id='chkDelete_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td></tr>";
        //                                countSubMenu++;
        //                            }
        //                        }
        //                    }
        //                    formBoxString = formBoxString + "</table></fieldset></div></fieldset>";
        //                    countMenu++;
        //                }
        //                else
        //                {
        //                    string checkeboxCheckedRead = "";
        //                    string checkeboxCheckedWrite = "";
        //                    string checkeboxCheckedModify = "";
        //                    string checkeboxCheckedDelete = "";
        //                    if (selectedMenu.IsRead == true)
        //                        checkeboxCheckedRead = "checked";
        //                    if (selectedMenu.IsWrite == true)
        //                        checkeboxCheckedWrite = "checked";
        //                    if (selectedMenu.IsModify == true)
        //                        checkeboxCheckedModify = "checked";
        //                    if (selectedMenu.IsDelete == true)
        //                        checkeboxCheckedDelete = "checked";

        //                    formBoxString = formBoxString + "<br><fieldset><div id='dvMenu' class='dvMenu'><fieldset><table><tbody><tr><td><span id='imgMenu' class='imgMenu submenu-map-icn-minus'>" + menu.menuName + "</span></td><td ><input class='chkMenuRead'" + checkeboxCheckedRead + " id='chkRead_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "' type='checkbox' /></td><td ><input class='chkMenuWrite'" + checkeboxCheckedWrite + " id='chkWrite_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "'  type='checkbox' /></td><td ><input class='chkMenuModify'" + checkeboxCheckedModify + " id='chkModify_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "'  type='checkbox'/></td><td ><input class='chkMenuDelete'" + checkeboxCheckedDelete + " id='chkDelete_MID" + menu.menuPathId + "_AppId" + menu.applicationId + "' type='checkbox' /></td> </tr></tbody></table><div id='dvSubMenu' class='dvSubMenu'><table >";

        //                    int countSubMenu = 1;
        //                    foreach (var subMenu in lstSubMenu)
        //                    {
        //                        var selectedSubMenu = selectedMenus.Where(x => x.menuPathId == subMenu.menuPathId && x.applicationId == menu.applicationId).FirstOrDefault();
        //                        if (selectedSubMenu == null)
        //                        {
        //                            if (subMenu.menuParentId == menu.menuPathId)
        //                            {
        //                                formBoxString = formBoxString + "<tr><td>" + subMenu.menuName + "</td><td><input class='chkSubMenuRead' id='chkRead_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox' /></td><td><input class='chkSubMenuWrite' id='chkWrite_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuModify' id='chkModify_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'</td><td><input class='chkSubMenuDelete' id='chkDelete_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td></tr>";
        //                                countSubMenu++;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            string checkeboxCheckedSMRead = "";
        //                            string checkeboxCheckedSMWrite = "";
        //                            string checkeboxCheckedSMModify = "";
        //                            string checkeboxCheckedSMDelete = "";
        //                            if (selectedSubMenu.IsRead == true)
        //                                checkeboxCheckedSMRead = "checked";
        //                            if (selectedSubMenu.IsWrite == true)
        //                                checkeboxCheckedSMWrite = "checked";
        //                            if (selectedSubMenu.IsModify == true)
        //                                checkeboxCheckedSMModify = "checked";
        //                            if (selectedSubMenu.IsDelete == true)
        //                                checkeboxCheckedSMDelete = "checked";
        //                            if (subMenu.menuParentId == menu.menuPathId)
        //                            {
        //                                formBoxString = formBoxString + "<tr><td>" + subMenu.menuName + "</td><td><input class='chkSubMenuRead'" + checkeboxCheckedSMRead + " id='chkRead_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox' /></td><td><input class='chkSubMenuWrite'" + checkeboxCheckedSMWrite + " id='chkWrite_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuModify'" + checkeboxCheckedSMModify + " id='chkModify_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td><td><input class='chkSubMenuDelete'" + checkeboxCheckedSMDelete + " id='chkDelete_SMID" + subMenu.menuPathId + "_MID" + subMenu.menuParentId + "_AppId" + subMenu.applicationId + "' type='checkbox'/></td></tr>";
        //                                countSubMenu++;
        //                            }
        //                        }
        //                    }
        //                    formBoxString = formBoxString + "</table></fieldset></div></fieldset>";
        //                    countMenu++;
        //                }
        //            }
        //        }
        //        formBoxString = formBoxString + "</fieldset>";
        //    }
        //    return htmlString + formBoxString + "</div>";
        //}

        public NDARoleViewModel SaveApplicationRoleMenu(NDARoleViewModel model)
        {
            throw new NotImplementedException();
        }

        //public bool SaveMenuMapping(IEnumerable<MenuMappingDetail> lstMenuMapping, int roleId, int userId)
        //{
        //    var isDataInserted = false;
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var roleTrans = dbContext.UmRoleMasterTrans.Where(x => x.RoleId == roleId).ToList();
        //        if (roleTrans.Count > 0)
        //        {
        //            foreach (UmRoleMasterTran objUmRoleMasterTran in roleTrans)
        //            {
        //                dbContext.UmRoleMasterTrans.Remove(objUmRoleMasterTran);
        //            }
        //            dbContext.SaveChanges();
        //        }
        //        var objresult = dbContext.UmRoleMasters.FirstOrDefault(id => id.RoleId == roleId);
        //        if (objresult.RoleType.Trim().ToUpper() == Constants.Admin)
        //        {
        //            Int32 userManagement = Convert.ToInt32(ConfigurationManager.AppSettings["UserManagementApplicationID"]);
        //            Int32 manageUserRoles = Convert.ToInt32(ConfigurationManager.AppSettings["ManageUserRoles"]);
        //            Int32 manageAdminUsers = Convert.ToInt32(ConfigurationManager.AppSettings["ManageAdminUsers"]);
        //            var umRoleMasterTran = new UmRoleMasterTran
        //            {
        //                RoleId = roleId,
        //                ApplicationId = userManagement,
        //                MenuId = manageUserRoles,
        //                IsRead = true,
        //                IsWrite = true,
        //                Isdelete = true,
        //                IsUpdate = true,
        //                CreatedBy = userId.ToString(),
        //                CreatedDate = DateTime.Now
        //            };

        //            var umRoleMasterTran1 = new UmRoleMasterTran
        //            {
        //                RoleId = roleId,
        //                ApplicationId = userManagement,
        //                MenuId = manageAdminUsers,
        //                IsRead = true,
        //                IsWrite = true,
        //                Isdelete = true,
        //                IsUpdate = true,
        //                CreatedBy = userId.ToString(),
        //                CreatedDate = DateTime.Now
        //            };
        //            //end 
        //            dbContext.UmRoleMasterTrans.Add(umRoleMasterTran);
        //            dbContext.UmRoleMasterTrans.Add(umRoleMasterTran1);
        //        }
        //        foreach (var MenuMappingDetail in lstMenuMapping)
        //        {
        //            var roleMasterTrans = new UmRoleMasterTran();
        //            roleMasterTrans.RoleId = roleId;
        //            roleMasterTrans.ApplicationId = MenuMappingDetail.applicationId;
        //            roleMasterTrans.MenuId = MenuMappingDetail.menuId;
        //            roleMasterTrans.IsRead = MenuMappingDetail.IsRead;
        //            roleMasterTrans.IsWrite = MenuMappingDetail.IsWrite;
        //            roleMasterTrans.IsUpdate = MenuMappingDetail.IsModify;
        //            roleMasterTrans.Isdelete = MenuMappingDetail.IsDelete;
        //            roleMasterTrans.CreatedDate = DateTime.Now;
        //            roleMasterTrans.CreatedBy = userId.ToString();
        //            roleMasterTrans.ModifiedBy = userId.ToString();
        //            roleMasterTrans.ModifiedDate = DateTime.Now;
        //            dbContext.UmRoleMasterTrans.Add(roleMasterTrans);
        //        }
        //        dbContext.SaveChanges();
        //        isDataInserted = true;
        //    }
        //    return isDataInserted;
        //}


        public NDAMasterDataViewModel GetMasterDataInfoByType(NDAMasterDataViewModel model)
        {
            if (!string.IsNullOrEmpty(model.MasterData))
            {
                switch (model.MasterData)
                {
                    case "Application":
                        model = GetApplicationInfo(model);
                        break;
                    case "Role":
                        model = GetRoleInfo(model);
                        break;
                    case "Menu":
                        model = GetApplicationMenuInfo(model);
                        break;
                    case "Department":
                        model = GetDepartmentInfo(model);
                        break;
                    case "Status":
                        model = GetStatusInfo(model);
                        break;
                    case "Sector":
                        model = GetSectorInfo(model);
                        break;
                    case "Block":
                        model = GetBlockInfo(model);
                        break;
                    case "PropertyType":
                        model = GetPropertyTypeInfo(model);
                        break;
                    case "SchemeType":
                        model = GetSchemeTypeInfo(model);
                        break;
                    case "AreaRange":
                        model = GetAreaRangeInfo(model);
                        break;
                    case "CommonConfig":
                        model = GetCommonConfigInfo(model);
                        break;
                    case "OnlineScheme":
                        model = GetOnlineSchemeInfo(model);
                        break;
                }
            }
            return model;
        }

        private NDAMasterDataViewModel GetOnlineSchemeInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "onlinescheme")
                {
                    var data = dbContext.SchemeMsts.FirstOrDefault(a => a.schemeId == model.SchemeId);
                    if (data != null)
                    {
                        model.SchemeId = data.schemeId;
                        model.SchemeName = data.schemeName;
                        model.StartDate = data.startDate;
                        model.EndDate = data.endDate;
                        model.SchemeFormFee = data.FormFee;
                        model.ProcessingFee = data.ProcessingFee;
                        model.ReturnTypeId = ReturnType.Success;
                    }
                }
                return model;
            }
        }

        private NDAMasterDataViewModel GetApplicationInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "application")
                {
                    var app = dbContext.UmApplicationMasters.FirstOrDefault(a => a.ApplicationId == model.ApplicationId);
                    if (app != null)
                    {
                        model.ApplicationId = app.ApplicationId;
                        model.ApplicationName = app.ApplicationName;
                        model.ApplicationUrl = app.ApplicationUrl;
                        model.IsActive = app.IsActive;

                        model.ReturnTypeId = ReturnType.Success;
                    }
                }
                return model;
            }
        }

        private NDAMasterDataViewModel GetRoleInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "role")
                {
                    var rol = dbContext.UmRoleMasters.FirstOrDefault(r => r.RoleId == model.RoleId);
                    if (rol != null)
                    {
                        model.RoleId = rol.RoleId;
                        model.RoleName = rol.RoleName;
                        model.RoleInDepartment = rol.RoleInDepartment;
                        model.RoleType = rol.RoleType;
                        model.RoleDescription = rol.RoleDescription;
                        model.IsActive = rol.IsActive;

                        model.ReturnTypeId = ReturnType.Success;
                        
                    }
                }
                return model;
            }
        }

        private NDAMasterDataViewModel GetApplicationMenuInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "menu")
                {
                    var data = dbContext.UmMenuMasters.FirstOrDefault(m => m.MenuId == model.MenuId);
                    if (data != null)
                    {
                        model.MenuId = data.MenuId;
                        model.MenuName = data.MenuName;
                        model.MenuParentId = data.MenuParentId;
                        model.MenuPathId = data.MenuPathId;

                        model.ReturnTypeId = ReturnType.Success;
                    }
                }
                return model;
            }
        }

        private NDAMasterDataViewModel GetDepartmentInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "department")
                {
                    var dept = dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == model.DepartmentId);
                    if (dept != null)
                    {
                        model.DepartmentId = dept.departmentId;
                        model.Department = dept.departmentName;
                        model.IsActive = dept.IsActive;

                        model.ReturnTypeId = ReturnType.Success;
                       
                    }
                }
                return model;
            }
        }

        private NDAMasterDataViewModel GetStatusInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.StatusMasters.FirstOrDefault(d => d.Id == model.StatusId);
                if (data != null)
                {
                    model.StatusId = data.Id;
                    model.StatusName = data.Status;
                    model.IsActive = data.IsActive;

                    model.ReturnTypeId = ReturnType.Success;

                }
                return model;
            }
        }

        private NDAMasterDataViewModel GetSectorInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.SectorMsts.FirstOrDefault(d => d.sectorId == model.SectorId);
                if (data != null)
                {
                    model.SectorId = data.sectorId;
                    model.SectorName = data.sectorName;
                    model.IsActive = data.IsActive;

                    model.ReturnTypeId = ReturnType.Success;

                }
                return model;
            }
        }

        private NDAMasterDataViewModel GetBlockInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.BlockMsts.FirstOrDefault(d => d.blockId == model.BlockId);
                if (data != null)
                {
                    model.BlockId = data.blockId;
                    model.BlockName = data.blockName;
                    model.IsActive = data.IsActive;

                    model.ReturnTypeId = ReturnType.Success;

                }
                return model;
            }
            throw new NotImplementedException();
        }

        private NDAMasterDataViewModel GetPropertyTypeInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.PropertyTypeMsts.FirstOrDefault(d => d.propertyTypeId == model.PropertyTypeId);
                if (data != null)
                {
                    model.PropertyTypeId = data.propertyTypeId;
                    model.PropertyType = data.propertyTypeName;
                    model.IsActive = data.IsActive;
                    model.DepartmentId = data.departmentId;
                    model.Department = data.departmentId != null ? dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == data.departmentId).departmentName : string.Empty;
                    model.ReturnTypeId = ReturnType.Success;

                }
                return model;
            }
        }

        private NDAMasterDataViewModel GetSchemeTypeInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.SchemeTypeMsts.FirstOrDefault(d => d.schemeTypeId == model.SchemeTypeId);
                if (data != null)
                {
                    model.SchemeTypeId = data.schemeTypeId;
                    model.SchemeType = data.SchemeType;
                    model.IsActive = data.IsActive;
                    model.SchemeTypeDescription = data.SchemeTypeDesc;
                    model.SchemePetitionType = data.modifiedBy;
                    model.ReturnTypeId = ReturnType.Success;
                }
                return model;
            }
        }

        private NDAMasterDataViewModel GetAreaRangeInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.FloorMsts.FirstOrDefault(d => d.floorId == model.AreaRangeId);
                if (data != null)
                {
                    model.AreaRangeId = data.floorId;
                    model.AreaRange = data.floorName;
                    model.IsActive = data.IsActive;
                    model.DepartmentId = data.departmentId;
                    model.Department = data.departmentId != null ? dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == data.departmentId).departmentName : string.Empty;
                    model.ReturnTypeId = ReturnType.Success;
                }
                return model;
            }
        }

        private NDAMasterDataViewModel GetCommonConfigInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.Common_Config.FirstOrDefault(d => d.Id == model.ConfigId);
                if (data != null)
                {
                    model.ConfigId = data.Id;
                    model.ConfigName = data.Name;
                    model.IsActive = data.Is_Active == 1 ? true : false;
                    model.ConfigRangeId = data.Range;
                    model.Category = data.Category;
                    model.ReturnTypeId = ReturnType.Success;
                }
                return model;
            }
        }


        public NDAMasterDataViewModel SaveMasterDataInfoByType(NDAMasterDataViewModel model)
        {
            var flag = ReturnType.None;

            if (!string.IsNullOrEmpty(model.MasterData))
            {
                switch (model.MasterData)
                {
                    case "Application":
                        flag = SaveApplicationInfo(model);
                        break;
                    case "Role":
                        flag = SaveRoleInfo(model);
                        break;
                    case "Menu":
                        flag = SaveApplicationMenuInfo(model);
                        break;
                    case "Department":
                        flag = SaveDepartmentInfo(model);
                        break;
                    case "Status":
                        flag = SaveStatusInfo(model);
                        break;
                    case "Sector":
                        flag = SaveSectorInfo(model);
                        break;
                    case "Block":
                        flag = SaveBlockInfo(model);
                        break;
                    case "PropertyType":
                        flag = SavePropertyTypeInfo(model);
                        break;
                    case "SchemeType":
                        flag = SaveSchemeTypeInfo(model);
                        break;
                    case "AreaRange":
                        flag = SaveAreaRangeInfo(model);
                        break;
                    case "CommonConfig":
                        flag = SaveCommonConfigInfo(model);
                        break;
                    case "OnlineScheme":
                        flag = SaveOnlineSchemeInfo(model);
                        break;
                }
                model.ReturnTypeId = flag;
            }
            return model;
        }

        private int SaveOnlineSchemeInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "save")
                {
                    var data = new SchemeMst();
                    decimal d1 = (model.SchemeFormFee != null && model.SchemeFormFee > 0) ? (decimal.Multiply(Convert.ToDecimal(model.SchemeFormFee.Value), Convert.ToDecimal(0.09))) : 0;
                    decimal d2 = (model.ProcessingFee != null && model.ProcessingFee > 0) ? (decimal.Multiply(Convert.ToDecimal(model.ProcessingFee.Value), Convert.ToDecimal(0.09))) : 0;
                        
                    data.schemeName = model.SchemeName;
                    data.startDate = model.StartDate;
                    data.endDate = model.EndDate;
                    data.FormFee = model.SchemeFormFee;
                    data.FormCGST = d1;
                    data.FormSGST = d1;
                    data.ProcessingFee = model.ProcessingFee;
                    data.ProcessingCGST = d2;
                    data.ProcessingSGST = d2;
                    data.createdDate = DateTime.Now;

                    dbContext.SchemeMsts.Add(data);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "update")
                {
                    var data = dbContext.SchemeMsts.FirstOrDefault(a => a.schemeId == model.SchemeId);
                    if (data != null)
                    {
                        decimal d1 = (model.SchemeFormFee != null && model.SchemeFormFee > 0) ? (decimal.Multiply(Convert.ToDecimal(model.SchemeFormFee.Value), Convert.ToDecimal(0.09))) : 0;
                        decimal d2 = (model.ProcessingFee != null && model.ProcessingFee > 0) ? (decimal.Multiply(Convert.ToDecimal(model.ProcessingFee.Value), Convert.ToDecimal(0.09))) : 0;
                        data.schemeName = model.SchemeName;
                        data.startDate = model.StartDate;
                        data.endDate = model.EndDate;
                        data.FormFee = model.SchemeFormFee;
                        data.FormCGST = d1;
                        data.FormSGST = d1;
                        data.ProcessingFee = model.ProcessingFee;
                        data.ProcessingCGST = d2;
                        data.ProcessingSGST = d2;
                        data.modifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "status")
                {
                    var data = dbContext.SchemeMsts.FirstOrDefault(a => a.schemeId == model.SchemeId);
                    if (data != null)
                    {
                        data.IsActive = data.IsActive == true ? false : true;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }

                return flag;
            }
        }

        private int SaveCommonConfigInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "save")
                {
                    var data = new Common_Config();
                    data.Name = model.ConfigName;
                    data.Category = model.Category;
                    data.Range = model.ConfigRangeId;
                    data.Is_Active = 1;
                    data.Created_Date = DateTime.Now;

                    dbContext.Common_Config.Add(data);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "update")
                {
                    var data = dbContext.Common_Config.FirstOrDefault(a => a.Id == model.ConfigId);
                    if (data != null)
                    {
                        data.Name = model.ConfigName;
                        data.Category = model.Category;
                        data.Range = model.ConfigRangeId;
                        //data.IsActive = model.IsActive;
                        data.Modified_Date = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "status")
                {
                    var data = dbContext.Common_Config.FirstOrDefault(a => a.Id == model.ConfigId);
                    if (data != null)
                    {
                        data.Is_Active = data.Is_Active == 1 ? 0 : 1;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }

                return flag;
            }
        }

        private int SaveAreaRangeInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "save")
                {
                    var data = new FloorMst();
                    data.floorName = model.AreaRange;
                    data.departmentId = model.DepartmentId;
                    data.IsActive = true;
                    data.createdDate = DateTime.Now;

                    dbContext.FloorMsts.Add(data);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "update")
                {
                    var data = dbContext.FloorMsts.FirstOrDefault(a => a.floorId == model.AreaRangeId);
                    if (data != null)
                    {
                        data.floorName = model.AreaRange;
                        data.departmentId = model.DepartmentId;
                        //data.IsActive = model.IsActive;
                        data.modifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "status")
                {
                    var data = dbContext.FloorMsts.FirstOrDefault(a => a.floorId == model.AreaRangeId);
                    if (data != null)
                    {
                        data.IsActive = data.IsActive == true ? false : true;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }

                return flag;
            }
        }

        private int SaveSchemeTypeInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "save")
                {
                    var data = new SchemeTypeMst();
                    data.SchemeType = model.SchemeType;
                    data.SchemeTypeDesc = model.SchemeTypeDescription;
                    data.modifiedBy = model.SchemePetitionType;//for online or offline
                    data.IsActive = true;
                    data.createdDate = DateTime.Now;

                    dbContext.SchemeTypeMsts.Add(data);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "update")
                {
                    var data = dbContext.SchemeTypeMsts.FirstOrDefault(a => a.schemeTypeId == model.SchemeTypeId);
                    if (data != null)
                    {
                        data.SchemeType = model.SchemeType;
                        data.SchemeTypeDesc = model.SchemeTypeDescription;
                        data.modifiedBy = model.SchemePetitionType;//for online or offline
                        //data.IsActive = model.IsActive;
                        data.modifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "status")
                {
                    var data = dbContext.SchemeTypeMsts.FirstOrDefault(a => a.schemeTypeId == model.SchemeTypeId);
                    if (data != null)
                    {
                        data.IsActive = data.IsActive == true ? false : true;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }

                return flag;
            }
        }

        private int SavePropertyTypeInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "save")
                {
                    var data = new PropertyTypeMst();
                    data.propertyTypeName = model.PropertyType;
                    data.departmentId = model.DepartmentId;
                    data.IsActive = true;
                    data.createdDate = DateTime.Now;

                    dbContext.PropertyTypeMsts.Add(data);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "update")
                {
                    var data = dbContext.PropertyTypeMsts.FirstOrDefault(a => a.propertyTypeId == model.PropertyTypeId);
                    if (data != null)
                    {
                        data.propertyTypeName = model.PropertyType;
                        //data.IsActive = model.IsActive;
                        data.modifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "status")
                {
                    var data = dbContext.PropertyTypeMsts.FirstOrDefault(a => a.propertyTypeId == model.PropertyTypeId);
                    if (data != null)
                    {
                        data.IsActive = data.IsActive == true ? false : true;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }

                return flag;
            }
        }

        private int SaveBlockInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "save")
                {
                    var data = new BlockMst();
                    data.blockName = model.BlockName;
                    data.IsActive = true;
                    data.createdDate = DateTime.Now;

                    dbContext.BlockMsts.Add(data);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "update")
                {
                    var data = dbContext.BlockMsts.FirstOrDefault(a => a.blockId == model.BlockId);
                    if (data != null)
                    {
                        data.blockName = model.BlockName;
                        //data.IsActive = model.IsActive;
                        data.modifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "status")
                {
                    var data = dbContext.BlockMsts.FirstOrDefault(a => a.blockId == model.BlockId);
                    if (data != null)
                    {
                        data.IsActive = data.IsActive == true ? false : true;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }

                return flag;
            }
        }

        private int SaveSectorInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "save")
                {
                    var data = new SectorMst();
                    data.sectorName = model.SectorName;
                    data.IsActive = true;
                    data.createdDate = DateTime.Now;

                    dbContext.SectorMsts.Add(data);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "update")
                {
                    var data = dbContext.SectorMsts.FirstOrDefault(a => a.sectorId == model.SectorId);
                    if (data != null)
                    {
                        data.sectorName = model.SectorName;
                        //data.IsActive = model.IsActive;
                        data.modifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "status")
                {
                    var data = dbContext.SectorMsts.FirstOrDefault(a => a.sectorId == model.SectorId);
                    if (data != null)
                    {
                        data.IsActive = data.IsActive == true ? false : true;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }

                return flag;
            }
        }

        private int SaveStatusInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "save")
                {
                    var data = new StatusMaster();
                    data.Status = model.StatusName;
                    data.IsActive = true;
                    data.CreatedDate = DateTime.Now;

                    dbContext.StatusMasters.Add(data);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "update")
                {
                    var data = dbContext.StatusMasters.FirstOrDefault(a => a.Id == model.StatusId);
                    if (data != null)
                    {
                        data.Status = model.StatusName;
                        //data.IsActive = model.IsActive;
                        data.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "status")
                {
                    var data = dbContext.StatusMasters.FirstOrDefault(a => a.Id == model.StatusId);
                    if (data != null)
                    {
                        data.IsActive = data.IsActive == true ? false : true;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }

                return flag;
            }
        }

        private int SaveDepartmentInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "save")
                {
                    var data = new DepartmentMst();
                    data.departmentName = model.ApplicationName;
                    data.IsActive = true;
                    data.createdDate = DateTime.Now;

                    dbContext.DepartmentMsts.Add(data);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "update")
                {
                    var data = dbContext.DepartmentMsts.FirstOrDefault(a => a.departmentId == model.DepartmentId);
                    if (data != null)
                    {
                        data.departmentName = model.ApplicationName;
                        //data.IsActive = model.IsActive;
                        data.modifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "status")
                {
                    var data = dbContext.DepartmentMsts.FirstOrDefault(a => a.departmentId == model.DepartmentId);
                    if (data != null)
                    {
                        data.IsActive = data.IsActive == true ? false : true;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }

                return flag;
            }
        }

        private int SaveApplicationMenuInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "save")
                {
                    var data = new UmMenuMaster();
                    data.MenuName = model.MenuName;
                    data.MenuParentId = model.MenuParentId;
                    data.ApplicationId = model.ApplicationId;
                    data.IsActive = true;
                    data.CreatedDate = DateTime.Now;

                    dbContext.UmMenuMasters.Add(data);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "update")
                {
                    var data = dbContext.UmMenuMasters.FirstOrDefault(a => a.MenuId == model.MenuId);
                    if (data != null)
                    {
                        data.MenuName = model.MenuName;
                        data.MenuParentId = model.MenuParentId;
                        data.ApplicationId = model.ApplicationId;
                        //data.IsActive = model.IsActive;
                        data.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "status")
                {
                    var data = dbContext.UmMenuMasters.FirstOrDefault(a => a.MenuId == model.MenuId);
                    if (data != null)
                    {
                        data.IsActive = data.IsActive == true ? false : true;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }

                return flag;
            }
        }

        private int SaveRoleInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "save")
                {
                    var data = new UmRoleMaster();
                    data.RoleName = model.RoleName;
                    data.RoleDescription = model.RoleDescription;
                    data.RoleInDepartment = model.RoleInDepartment;
                    data.RoleType = model.RoleType;
                    data.IsActive = true;
                    data.CreatedDate = DateTime.Now;

                    dbContext.UmRoleMasters.Add(data);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "update")
                {
                    var data = dbContext.UmRoleMasters.FirstOrDefault(a => a.RoleId == model.RoleId);
                    if (data != null)
                    {
                        data.RoleName = model.RoleName;
                        data.RoleDescription = model.RoleDescription;
                        data.RoleInDepartment = model.RoleInDepartment;
                        data.RoleType = model.RoleType;
                        //data.IsActive = model.IsActive;
                        data.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "status")
                {
                    var data = dbContext.UmRoleMasters.FirstOrDefault(a => a.RoleId == model.RoleId);
                    if (data != null)
                    {
                        data.IsActive = data.IsActive == true ? false : true;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }

                return flag;
            }
        }

        private int SaveApplicationInfo(NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "save")
                {
                    var data = new UmApplicationMaster();
                    data.ApplicationName = model.ApplicationName;
                    data.ApplicationUrl = model.ApplicationUrl;
                    data.IsActive = true;
                    data.CreatedDate = DateTime.Now;

                    dbContext.UmApplicationMasters.Add(data);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "update")
                {
                    var data = dbContext.UmApplicationMasters.FirstOrDefault(a => a.ApplicationId == model.ApplicationId);
                    if (data != null)
                    {
                        data.ApplicationName = model.ApplicationName;
                        data.ApplicationUrl = model.ApplicationUrl;
                        //data.IsActive = model.IsActive;
                        data.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }
                else if (!string.IsNullOrEmpty(model.ActionType) && model.ActionType.ToLower() == "status")
                {
                    var data = dbContext.UmApplicationMasters.FirstOrDefault(a => a.ApplicationId == model.ApplicationId);
                    if (data != null)
                    {
                        data.IsActive = data.IsActive == true ? false : true;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                }

                return flag;
            }
        }


        public DataSourceResult GetDepartmentListAsDataSource(DataSourceRequest request, NDARoleViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from dept in dbContext.DepartmentMsts
                            select new NDARoleViewModel
                            {
                                Id= dept.departmentId,
                                DepartmentId = dept.departmentId,
                                Department = dept.departmentName,
                                IsActive = dept.IsActive,
                                CreatedBy = dept.createdBy,
                                CreatedDate = dept.createdDate,
                                UserName = string.Empty
                            });
                if (list != null) return list.ToDataSourceResult(request);
                else return null;
            }
        }

        public DataSourceResult GetStatusListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from status in dbContext.StatusMasters
                            select new NDAMasterDataViewModel
                            {
                                Id = status.Id,
                                StatusId = status.Id,
                                StatusName = status.Status,
                                IsActive = status.IsActive,
                                CreatedBy = status.CreatedBy.ToString(),
                                CreatedDate = status.CreatedDate,
                                UserName = string.Empty
                            });
                if (list != null) return list.ToDataSourceResult(request);
                else return null;
            }
        }

        public DataSourceResult GetSectorAndBlockListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (!string.IsNullOrEmpty(model.MasterData) && model.MasterData == "Sector")
                {
                    var list = (from data in dbContext.SectorMsts
                                select new NDAMasterDataViewModel
                                {
                                    Id = data.sectorId,
                                    SectorId = data.sectorId,
                                    SectorName = data.sectorName,
                                    IsActive = data.IsActive,
                                    CreatedBy = data.createdBy.ToString(),
                                    CreatedDate = data.createdDate,
                                    UserName = string.Empty
                                });
                    if (list != null) return list.ToDataSourceResult(request);
                    else return null;
                }
                else if (!string.IsNullOrEmpty(model.MasterData) && model.MasterData == "Block")
                {
                    var list = (from data in dbContext.BlockMsts
                                select new NDAMasterDataViewModel
                                {
                                    Id = data.blockId,
                                    BlockId = data.blockId,
                                    BlockName = data.blockName,
                                    IsActive = data.IsActive,
                                    CreatedBy = data.createdBy.ToString(),
                                    CreatedDate = data.createdDate,
                                    UserName = string.Empty
                                });
                    if (list != null) return list.ToDataSourceResult(request);
                    else return null;
                }
                else
                {
                    return null;
                }
            }
            
        }

        public DataSourceResult GetPropertyTypeListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from data in dbContext.PropertyTypeMsts
                            select new NDAMasterDataViewModel
                            {
                                Id = data.propertyTypeId,
                                PropertyTypeId = data.propertyTypeId,
                                PropertyType = data.propertyTypeName,
                                DepartmentId = data.departmentId,
                                Department = data.departmentId != null ? dbContext.DepartmentMsts.FirstOrDefault(d=>d.departmentId==data.departmentId).departmentName : string.Empty,
                                IsActive = data.IsActive,
                                CreatedBy = data.createdBy.ToString(),
                                CreatedDate = data.createdDate,
                                UserName = string.Empty
                            });
                if (list != null) return list.ToDataSourceResult(request);
                else return null;
            }
        }

        public DataSourceResult GetSchemeTypeListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from data in dbContext.SchemeTypeMsts
                            select new NDAMasterDataViewModel
                            {
                                Id = data.schemeTypeId,
                                SchemeTypeId = data.schemeTypeId,
                                SchemeType = data.SchemeType,
                                SchemePetitionType = data.modifiedBy == "Online" ? data.modifiedBy : "Offline",
                                SchemeTypeDescription = data.SchemeTypeDesc,
                                IsActive = data.IsActive,
                                CreatedBy = data.createdBy.ToString(),
                                CreatedDate = data.createdDate,
                                UserName = string.Empty
                            });
                if (list != null) return list.ToDataSourceResult(request);
                else return null;
            }
        }

        public DataSourceResult GetAreaRangeListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from data in dbContext.FloorMsts
                            select new NDAMasterDataViewModel
                            {
                                Id = data.floorId,
                                AreaRangeId = data.floorId,
                                AreaRange = data.floorName,
                                DepartmentId = data.departmentId,
                                Department = data.departmentId != null ? dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == data.departmentId).departmentName : string.Empty,
                                IsActive = data.IsActive,
                                CreatedBy = data.createdBy.ToString(),
                                CreatedDate = data.createdDate,
                                UserName = string.Empty,
                                Category = data.category
                            });
                if (list != null) return list.ToDataSourceResult(request);
                else return null;
            }
        }

        public DataSourceResult GetCommonConfigurationListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from data in dbContext.Common_Config
                            select new NDAMasterDataViewModel
                            {
                                Id = data.Id,
                                ConfigId = data.Id,
                                ConfigName = data.Name,
                                IsActive = data.Is_Active == 1? true : false,
                                CreatedBy = data.Created_By.ToString(),
                                CreatedDate = data.Created_Date,
                                UserName = string.Empty,
                                Category = data.Category
                            });
                if (list != null) return list.ToDataSourceResult(request);
                else return null;
            }
        }


        public DataSourceResult GetOnlineSchemeListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from data in dbContext.SchemeMsts
                            join styp in dbContext.SchemeTypeMsts on data.schemeTypeId equals styp.schemeTypeId
                            where styp.modifiedBy == "Online" // && data.Status == 24
                            select new NDAMasterDataViewModel
                            {
                                Id = data.schemeId,
                                SchemeId = data.schemeId,
                                SchemeName = data.schemeName,
                                IsActive = data.IsActive,
                                CreatedBy = data.createdBy,
                                CreatedDate = data.createdDate,
                                StartDate = data.startDate,
                                EndDate = data.endDate,
                                SchemeFormFee = data.FormFee,
                                ProcessingFee = data.ProcessingFee,
                                StatusId = data.Status,
                                StatusName = data.Status == 24 ? "Completed" : "Not Completed",
                                UserName = string.Empty,
                            });
                if (list != null) return list.ToDataSourceResult(request);
                else return null;
            }
        }


        public DataSourceResult GetMasterDataDropDownListAsDataSource(DataSourceRequest request, NDAMasterDataViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (!string.IsNullOrEmpty(model.MasterData) && model.MasterData.ToLower() == "application")
                {
                    var list = (from data in dbContext.UmApplicationMasters
                                select new DropdownViewModel
                                {
                                    Id = data.ApplicationId,
                                    Text = data.ApplicationName
                                });
                    if (list != null) return list.ToDataSourceResult(request);
                    else return null;
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "department")
                {
                    var list = (from data in dbContext.DepartmentMsts
                                select new DropdownViewModel
                                {
                                    Id = data.departmentId,
                                    Text = data.departmentName
                                });
                    if (list != null) return list.ToDataSourceResult(request);
                    else return null;
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "role")
                {
                    var list = (from data in dbContext.UmRoleMasters
                                select new DropdownViewModel
                                {
                                    Id = data.RoleId,
                                    Text = data.RoleName
                                });
                    if (list != null) return list.ToDataSourceResult(request);
                    else return null;
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "menu")
                {
                    var list = (from data in dbContext.UmMenuMasters
                                where (model.MenuId == null || data.MenuParentId == model.MenuId)
                                select new DropdownViewModel
                                {
                                    Id = data.MenuId,
                                    Text = data.MenuName
                                });
                    if (list != null) return list.ToDataSourceResult(request);
                    else return null;
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "parentmenu")
                {
                    var list = (from data in dbContext.UmMenuMasters
                                where data.MenuParentId == null
                                select new DropdownViewModel
                                {
                                    Id = data.MenuId,
                                    Text = data.MenuName
                                });
                    if (list != null) return list.ToDataSourceResult(request);
                    else return null;
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "sector")
                {
                    var list = (from data in dbContext.SectorMsts
                                select new DropdownViewModel
                                {
                                    Id = data.sectorId,
                                    Text = data.sectorName
                                });
                    if (list != null) return list.ToDataSourceResult(request);
                    else return null;
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "block")
                {
                    var list = (from data in dbContext.BlockMsts
                                select new DropdownViewModel
                                {
                                    Id = data.blockId,
                                    Text = data.blockName
                                });
                    if (list != null) return list.ToDataSourceResult(request);
                    else return null;
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "status")
                {
                    var list = (from data in dbContext.StatusMasters
                                select new DropdownViewModel
                                {
                                    Id = data.Id,
                                    Text = data.Status
                                });
                    if (list != null) return list.ToDataSourceResult(request);
                    else return null;
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "propertytype")
                {
                    var list = (from data in dbContext.PropertyTypeMsts
                                select new DropdownViewModel
                                {
                                    Id = data.propertyTypeId,
                                    Text = data.propertyTypeName,
                                    DepartmentId = data.departmentId
                                });
                    if (list != null) return list.ToDataSourceResult(request);
                    else return null;
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "arearange")
                {
                    var list = (from data in dbContext.FloorMsts
                                select new DropdownViewModel
                                {
                                    Id = data.floorId,
                                    Text = data.floorName,
                                    DepartmentId = data.departmentId
                                });
                    if (list != null) return list.ToDataSourceResult(request);
                    else return null;
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "schemetype")
                {
                    var list = (from data in dbContext.SchemeTypeMsts
                                where data.modifiedBy == "Online"
                                select new DropdownViewModel
                                {
                                    Id = data.schemeTypeId,
                                    Text = data.SchemeTypeDesc
                                });
                    if (list != null) return list.ToDataSourceResult(request);
                    else return null;
                }
                else if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "onlinescheme")
                {
                    var list = (from data in dbContext.SchemeMsts
                                join trans in dbContext.SchemeTypeMsts on data.schemeTypeId equals trans.schemeTypeId
                                where trans.modifiedBy == "Online"
                                select new DropdownViewModel
                                {
                                    Id = data.schemeId,
                                    Text = data.schemeName
                                });
                    if (list != null) return list.ToDataSourceResult(request);
                    else return null;
                }
                else if (!string.IsNullOrEmpty(model.MasterData) && model.MasterData.ToLower() == "commonconfig")
                {
                    if (!string.IsNullOrEmpty(model.FilterType) && model.FilterType.ToLower() == "category")
                    {
                        var list = (from data in dbContext.Common_Config
                                    where data.Is_Active == 1
                                    select new DropdownViewModel
                                    {
                                        Text = data.Category,
                                        Value = data.Category,
                                        Id = data.Id
                                    }).GroupBy(x => x.Text).Select(y => y.FirstOrDefault());
                        if (list != null) return list.ToDataSourceResult(request);
                        else return null;
                    }
                    else
                    {
                        var list = (from data in dbContext.Common_Config
                                    where (model.Category == null || data.Category == model.Category)
                                    select new DropdownViewModel
                                    {
                                        Id = data.Id,
                                        Text = data.Name,
                                        Value = data.Category
                                    });
                        if (list != null) return list.ToDataSourceResult(request);
                        else return null;
                    }
                    
                }
                else
                {
                    return null;
                }
            }
        }
    }
}