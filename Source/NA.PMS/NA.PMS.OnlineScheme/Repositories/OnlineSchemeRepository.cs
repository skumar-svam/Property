using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using NA.PMS.Model;
using System.Web;

using System.Configuration;
using System.IO;
using System.Transactions;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using NA.PMS.OnlineScheme.Context;

namespace NA.PMS.OnlineScheme
{
    public class OnlineSchemeRepository : IOnlineSchemeRepository
    {
        //private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public OnlineSchemeRepository()
        {
            //if (HttpContext.Current != null)
            //{
            //    if (HttpContext.Current.Session != null)
            //    {
            //        if (HttpContext.Current.Session["CurrentUser"] != null)
            //        {
            //            userInfo = HttpContext.Current.Session["CurrentUser"] as CurrentUserDetail;
            //            using (var dbContext = new OSDbContext())
            //            {
            //                DepartmentList = dbContext.UmUserDepartmentTrans.Where(u => u.UserRefId == userInfo.UserID && u.Status == true).Select(d => d.DepartmentId).ToList();
            //            }
            //        }
            //    }
            //}
        }

        public DataSourceResult GetOnlineSchemeApplications(DataSourceRequest request, SchemeFormViewModel modal)
        {
            using (var dbContext = new OSDbContext())
            {
                string userRole = "A"; //userInfo.RoleMaster.RoleType;

                var loginUserDepartment = (from userMaster in dbContext.UmUserMasters
                                           join departmentTrans in dbContext.UmUserDepartmentTrans on userMaster.UserRefId equals departmentTrans.UserRefId
                                           where userMaster.UserRefId == 1133 //userInfo.UserID
                                           select departmentTrans.DepartmentId).ToList();
                if (modal.SchemeId == null)
                {
                    modal.SchemeId = Convert.ToInt32(ConfigurationManager.AppSettings["CurrentSchemeId"]);
                }
                if (userRole == OSStringConstant.Admin ) //|| userInfo.UserID == 1222)
                {
                    int MoveToOSD = Convert.ToInt32(OSConstant.MoveToOSD);
                    int Scrutiny = Convert.ToInt32(OSConstant.Scrutiny);
                    int Draw = Convert.ToInt32(OSConstant.Draw);
                    int ApprovalCEO = Convert.ToInt32(OSConstant.ApprovalCEO);

                    var Online_trans = (from oam in dbContext.OnlineApplicationDetails
                                        from sch in dbContext.SchemeMsts.Where(s => s.schemeId == oam.schemeId).DefaultIfEmpty()
                                        from deptt in dbContext.DepartmentMsts.Where(d => d.departmentId == oam.departmentId).DefaultIfEmpty()
                                        where loginUserDepartment.Contains(oam.departmentId)
                                        && (modal.Id == null || oam.onlineapplicationId == modal.Id)
                                        && (modal.DepartmentId == null || oam.departmentId == modal.DepartmentId)
                                        && (modal.SchemeId == null || oam.schemeId == modal.SchemeId)
                                        select new SchemeFormViewModel
                                        {
                                            Id = oam.onlineapplicationId,
                                            ApplicationFormId = oam.onlineapplicationId,
                                            SchemeName = sch.schemeName,
                                            Department = deptt.departmentName,
                                            FirstName = string.IsNullOrEmpty(oam.CompanyName) ? oam.firstName + " " + (!string.IsNullOrEmpty(oam.middleName) ? oam.middleName + " " + oam.lastName : oam.lastName) : oam.CompanyName,
                                            Gender = oam.gender,
                                            DOB = oam.dateOfBirth,
                                            FormStatus = oam.IsSubmited == null ? OSStringConstant.Rejected : (oam.IsSubmited == true ? OSStringConstant.Accepted : OSStringConstant.InProgress),
                                            IsDeleted = oam.isActive == null ? "" : (oam.isActive == true ? OSStringConstant.Accepted : OSStringConstant.Rejected),
                                            TotalAmount = oam.TotalAmount,
                                            SubmitDate = oam.createdDate,
                                            AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == oam.area && m.IsActive == true).FirstOrDefault().floorName,
                                            ApplicantType = oam.gender == "Company" ? "Company" : "Individual",
                                            AmountPaidDate = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().EntryDate,
                                            ChallanBank = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.SchemeBankTrans.FirstOrDefault(m => m.schemeId == oam.schemeId && m.IsActive == true && m.bankId.ToString() == (dbContext.OnlineApplicationDetails_trans.FirstOrDefault(i => i.ServiceRefId == oam.onlineapplicationId && i.ServiceType == 3).GetwayName)).BankMst.bankName) : "--",
                                            PayType = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? "Online" : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? "Offline (RTGS/NEFT)" : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 4 ? "Previous Challan" : "--")),
                                            //ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "Paid" : "Not Paid") : "--"),
                                            ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "Paid" : "Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 4 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "PC Paid" : "PC Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 5 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "SWP Paid" : "SWP Not Paid") : "--"))),
                                            ApplicationFormType = !string.IsNullOrEmpty(oam.FormCategory) ? (oam.FormCategory) : "--",
                                            ExpansionType = !string.IsNullOrEmpty(oam.FormCategory) ? ((oam.FormCategory == OSStringConstant.ExpansionForm) ? oam.FormSubCategory : "--") : "--",
                                            ProcessType = oam.OnlineApplicationProcessDetails.FirstOrDefault() != null ? (oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == MoveToOSD) != null ? "Move To OSD" : oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == Scrutiny) != null ? "Scrunity" : oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == ApprovalCEO) != null ? "Approval for CEO" : oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == Draw) != null ? "Draw" : string.Empty) : string.Empty
                                        });

                    if (modal.PayType == "2")
                    {
                        Online_trans = Online_trans.Where(m => m.ChallanStatus != "Not Paid" && m.ChallanStatus != "--");
                    }
                    if (modal.PayType == OSStringConstant.Paid)
                    {
                        Online_trans = Online_trans.Where(m => m.ChallanStatus != "Not Paid" && m.ChallanStatus != "--");
                    }
                    return Online_trans.ToDataSourceResult(request);
                }
                else
                {
                    //int? area = (userInfo.OptionalId == null || userInfo.OptionalId == 0) ? 0 : userInfo.OptionalId;
                    int? area = 0;
                    var Online = (from oam in dbContext.OnlineApplicationDetails
                                  join oad_t in dbContext.OnlineApplicationDetails_trans on oam.onlineapplicationId equals oad_t.ServiceRefId
                                  join sch in dbContext.SchemeMsts on oam.schemeId equals sch.schemeId
                                  join deptt in dbContext.DepartmentMsts on oam.departmentId equals deptt.departmentId
                                  from floor in dbContext.FloorMsts.Where(f => f.floorId.ToString() == oam.area).DefaultIfEmpty()
                                  where ((oam.isActive == true && oad_t.status == 1) || oad_t.ServiceType == 4)
                                  && loginUserDepartment.Contains(oam.departmentId)
                                  && (modal.Id == null || oam.onlineapplicationId == modal.Id)
                                  && (modal.DepartmentId == null || oam.departmentId == modal.DepartmentId)
                                  && (modal.SchemeId == null || oam.schemeId == modal.SchemeId)
                                  && (area == 0 || oam.area == area.ToString())
                                  select new SchemeFormViewModel
                                  {
                                      Id = oam.onlineapplicationId,
                                      ApplicationFormId = oam.onlineapplicationId,
                                      SchemeName = sch.schemeName,
                                      Department = deptt.departmentName,
                                      FirstName = string.IsNullOrEmpty(oam.CompanyName) ? oam.firstName + " " + (!string.IsNullOrEmpty(oam.middleName) ? oam.middleName + " " + oam.lastName : oam.lastName) : oam.CompanyName,
                                      Gender = oam.gender,
                                      DOB = oam.dateOfBirth,
                                      FormStatus = oam.IsSubmited == null ? OSStringConstant.Rejected : (oam.IsSubmited == true ? OSStringConstant.Accepted : OSStringConstant.InProgress),
                                      AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == oam.area && m.IsActive == true).FirstOrDefault().floorName,
                                      TotalAmount = (decimal)oam.TotalAmount,//Amount Paid
                                      SubmitDate = oam.createdDate,
                                      AmountPaidDate = oad_t.EntryDate,
                                      ApplicantType = oam.gender == "Company" ? "Company" : "Individual",
                                      PayType = oad_t.ServiceType == 1 ? "Online" : (oad_t.ServiceType == 3 ? "Offline (RTGS/NEFT)" : "--"),
                                      ChallanStatus = oad_t.ServiceType == 3 ? (oad_t.status == 0 ? ((!string.IsNullOrEmpty(oad_t.TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (oad_t.ServiceType == 1 ? (oad_t.status == 1 ? "Paid" : "Not Paid") : "--"),
                                      ApplicationFormType = !string.IsNullOrEmpty(oam.FormCategory) ? (oam.FormCategory) : "--",
                                      ExpansionType = !string.IsNullOrEmpty(oam.FormCategory) ? ((oam.FormCategory == OSStringConstant.ExpansionForm) ? oam.FormSubCategory : "--") : "--"
                                  });
                    return Online.ToDataSourceResult(request);
                }
            }
        }

        public DataSourceResult GetDocumentTypeChecklistForOnlineSchemeApplication(DataSourceRequest request, SchemeFormViewModel modal)
        {
            using (var dbContext = new OSDbContext())
            {
                var docs = (from doc in dbContext.OnlineCheckListMasters
                            where doc.IsActive == true && doc.DepartmentId == modal.DepartmentId
                            select new SchemeDocumentViewModel
                            {
                                Id = doc.CheckListId,
                                DocumentId = doc.CheckListId,
                                DocumentName = doc.CheckListName
                            }).ToList();
                return docs.ToDataSourceResult(request);
            }
        }

        public SchemeFormViewModel SaveOnlineSchemeApplicationForm(SchemeFormViewModel model, System.Web.HttpPostedFileBase userImage, System.Web.HttpPostedFileBase signatureImage)
        {
            int flag = 0;
            using (var dbContext = new OSDbContext())
            {
                OnlineApplicationDetail application = new OnlineApplicationDetail();
                model.Id = model.Id == null ? 0 : model.Id;
                var detail = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == model.Id);
                if (detail == null)
                {
                    if (model.ApplicantType == OSStringConstant.Individual)
                    {
                        application.firstName = model.FirstName;
                        application.middleName = model.MiddleName;
                        application.lastName = model.LastName;
                        application.fatherHusbandName = model.FatherName;
                        application.motherName = model.MotherName;
                        application.gender = model.Gender;
                        application.religionId = model.ReligionId;
                        application.occupationId = model.OccupationId;
                        application.quotaId = OSConstant.General;
                        application.marritalStatus = model.MaritalStatus;
                        application.dateOfBirth = model.DOB;
                    }
                    else
                    {
                        application.CompanyName = model.Applicant;
                        application.signingAuthority = model.SigningAuthority;
                        application.registeredOffice = model.PermanentAddress;
                        application.CompanyType = model.CompanyTypeId;
                        application.gender = OSStringConstant.Company;
                        application.fatherHusbandName = model.ApplicantMaster;
                        application.quotaId = model.SigningAuthorityId;
                    }
                    application.CompanyType = model.CompanyTypeId;
                    application.schemeId = model.SchemeId;
                    application.departmentId = model.DepartmentId;
                    application.permanentAdd = model.PermanentAddress;
                    application.correspondanceAdd = model.CorrespondingAddress;
                    application.area = model.AreaRangeId;
                    application.mobileNumberP2 = model.MobileNumber;
                    application.phoneNumberP2 = model.PhoneNumber;
                    application.faxNumberP2 = model.FaxNumber;
                    application.email = model.Email;
                    application.annualIncome = model.AnnualIncome;
                    application.ApplicationFee = model.ApplicationFee;
                    application.FormCGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                    application.FormSGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                    application.ProcessingCharge = model.ProcessingCharge;
                    application.ProcessingCGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                    application.ProcessingSGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                    application.EarnestMoney = model.EarnestMoney;
                    application.TotalAmount = model.TotalAmount;
                    application.pan = model.PanNumber;
                    application.AadharNumber = model.AadharNumber;
                    //application.faxNumberP2 = model.ApplicantGSTNumber;
                    application.GSTNO = model.ApplicantGSTNumber;
                    application.ApplicationDate = DateTime.Now;

                    application.RefundBankId = model.RefundBankId;
                    application.RefundInfaverof = model.RefundInfaverof;
                    application.RefundaccountNo = model.RefundAccountNo;

                    application.PropertyTypeID = model.PropertyTypeId;
                    application.Sector = model.Sector;
                    application.BranchName = model.BranchName;
                    application.IFSCCode = model.IFSCCode;

                    application.FormCategory = model.ApplicationFormType;
                    if (model.ApplicationFormType == "Expansion")
                    {
                        application.FormSubCategory = model.ExpansionType;
                        application.RentingDate = model.LetterDate;
                        application.RentingLetterNo = model.LetterCode;
                        application.RentingPropertyNo = model.PropertyNo;
                    }

                    application.PropertyNo = model.PropertyNo;
                    application.ExistingPropertyNo = model.ExistingProperty;
                    application.AllotmentDate = model.AllottmentDate;
                    application.DispatchDate = model.DispatchDate;

                    application.createdBy = "Online";
                    application.createdDate = DateTime.Now;
                    application.Online_offline = (model.FormType == null || model.FormType == "Online") ? "Y" : "N";
                    application.isActive = true;
                    application.IsSubmited = false;

                    string PassWord = OnlineSchemeHelper.GenerateSchemeFormPassword();
                    application.Userpassword = PassWord.MD5HashPassword();
                    application.PreviousFormNo = model.PreviousFormNo;

                    if (SchemeConstant.Transport == model.SchemeType || SchemeConstant.OpenEnded == model.SchemeType || SchemeConstant.IndustrialScheme == model.SchemeType || model.SchemeType == "Industrial Scheme" || model.SchemeType == SchemeConstant.Institutional)
                    {
                        PassWord = OnlineSchemeHelper.GenerateSchemeFormPassword();
                        //application.Userpassword = PassWord.ToMD5HashForPasswordPIS();
                        application.Userpassword = PassWord.MD5HashPassword();
                        if (SchemeConstant.Transport == model.SchemeType)
                        {
                            if (!string.IsNullOrEmpty(model.PreviousFormNo))
                            {
                                //application.Comment = model.PreviousFormNo;
                                application.PreviousFormNo = model.PreviousFormNo;
                            }
                        }
                    }

                    if (model.DepartmentId == OSConstant.Housing)
                    {
                        application.RentingPropertyNo = model.EPFRegistrationNo;
                        application.ExistingPropertyNo = model.ESIRegistrationNo;
                    }

                    dbContext.OnlineApplicationDetails.Add(application);
                    dbContext.SaveChanges();

                    //Method to save NIC Get Values.
                    if (model.NICNewDataSetModel != null)
                    {
                        if (!string.IsNullOrEmpty(model.NICNewDataSetModel.Table.Control_ID))
                        {
                            model.NICNewDataSetModel.Table.OnlineApplicationId = application.onlineapplicationId;
                            model.NICNewDataSetModel.Table.departmentId = (int)application.departmentId;
                            model.NICNewDataSetModel.Table.SchemeId = (int)application.schemeId;
                            var Basicdetail = dbContext.NICsingalwindowSystems.FirstOrDefault(c => c.onlineapplicationId == model.NICNewDataSetModel.Table.OnlineApplicationId);
                            if (Basicdetail == null)
                            {
                                NICsingalwindowSystem nicSWSSystem = new NICsingalwindowSystem();
                                nicSWSSystem = MapNICSingalWindowSystemTable(model.NICNewDataSetModel);
                                nicSWSSystem.ServiceID = nicSWSSystem.ServiceID == null ? model.NICServiceId : nicSWSSystem.ServiceID;
                                dbContext.NICsingalwindowSystems.Add(nicSWSSystem);
                                dbContext.SaveChanges();
                            }
                        }
                    }

                    int id = application.onlineapplicationId;

                    model.Id = id;
                    model.ApplicationFormId = id;

                    var md = SaveImagesForOnlineSchemeApplication(model, userImage, signatureImage);

                    if (model.SchemeProposedFirm != null)
                    {
                        //UpdateDirectorDetailsForOpenSchemeForm(application.onlineapplicationId);
                    }

                    string mobileMessage = string.Format(OSMessages.PIS_Registration_Activation, id, PassWord);
                    string emailMessage = string.Format(OSMessages.PIS_Registration_Activation, id, PassWord);
                    if (model.MobileNumber != null && model.MobileNumber != "") OnlineSchemeHelper.SendSMS(model.MobileNumber, mobileMessage);
                    if (model.Email != null && model.Email != "") OnlineSchemeHelper.SendEmail(model.Email, "OnlineForm", emailMessage);

                    flag = OSReturnTypeId.Success;
                }
                else
                {
                    var upd = UpdateOnlineSchemeApplicationForm(model, userImage, signatureImage);
                }
            }
            //return model.Id.Value;
            return model;
        }

        private SchemeFormViewModel UpdateOnlineSchemeApplicationForm(SchemeFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            using (var dbContext = new OSDbContext())
            {
                var detail = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == model.ApplicationFormId);
                if (detail != null)
                {
                    if (model.ApplicantType == "Individual")
                    {
                        detail.firstName = model.FirstName;
                        detail.middleName = model.MiddleName;
                        detail.lastName = model.LastName;
                        detail.fatherHusbandName = model.FatherName;
                        detail.motherName = model.MotherName;
                        detail.gender = model.Gender;
                        detail.CompanyType = model.CompanyTypeId;
                        detail.religionId = model.ReligionId;
                        detail.occupationId = model.OccupationId;
                        detail.quotaId = OSConstant.General;
                        detail.marritalStatus = model.MaritalStatus;
                        detail.dateOfBirth = model.DOB;
                    }
                    else
                    {
                        detail.CompanyName = model.Applicant;
                        detail.signingAuthority = model.SigningAuthority;
                        detail.registeredOffice = model.PermanentAddress;
                        detail.CompanyType = model.CompanyTypeId;
                        detail.gender = "Company";
                        detail.fatherHusbandName = model.ApplicantMaster;
                        detail.quotaId = model.SigningAuthorityId;
                    }
                    detail.schemeId = model.SchemeId;
                    detail.departmentId = model.DepartmentId;
                    detail.permanentAdd = model.PermanentAddress;
                    detail.correspondanceAdd = model.CorrespondingAddress;
                    detail.area = model.AreaRangeId;
                    detail.mobileNumberP2 = model.MobileNumber;
                    detail.phoneNumberP2 = model.PhoneNumber;
                    detail.GSTNO = model.ApplicantGSTNumber;
                    detail.email = model.Email;
                    detail.annualIncome = model.AnnualIncome;
                    detail.ApplicationFee = model.ApplicationFee;
                    detail.ProcessingCharge = model.ProcessingCharge;
                    detail.EarnestMoney = model.EarnestMoney;
                    detail.TotalAmount = model.TotalAmount;
                    detail.pan = model.PanNumber;

                    detail.RefundBankId = model.RefundBankId;
                    detail.RefundInfaverof = model.RefundInfaverof;
                    detail.RefundaccountNo = model.RefundAccountNo;

                    detail.PropertyTypeID = model.PropertyTypeId;
                    detail.Sector = model.Sector;
                    detail.BranchName = model.BranchName;
                    detail.IFSCCode = model.IFSCCode;

                    //added on 19 aug 2017
                    detail.FormCGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                    detail.FormSGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                    detail.ProcessingCGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                    detail.ProcessingSGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                    detail.AadharNumber = model.AadharNumber;

                    detail.FormCategory = model.ApplicationFormType;
                    if (model.ApplicationFormType == "Expansion")
                    {
                        detail.FormSubCategory = model.ExpansionType;
                        detail.RentingDate = model.LetterDate;
                        detail.RentingLetterNo = model.LetterCode;
                        detail.RentingPropertyNo = model.PropertyNo;
                    }

                    detail.PreviousFormNo = model.PreviousFormNo;
                    
                    detail.PropertyNo = model.PropertyNo;
                    detail.ExistingPropertyNo = model.ExistingProperty;
                    detail.AllotmentDate = model.AllottmentDate;
                    detail.DispatchDate = model.DispatchDate;

                    detail.modifiedBy = "Online";
                    detail.modifiedDate = DateTime.Now;
                    detail.isActive = true;

                    if (model.DepartmentId == OSConstant.Housing)
                    {
                        detail.RentingPropertyNo = model.EPFRegistrationNo;
                        detail.ExistingPropertyNo = model.ESIRegistrationNo;
                    }

                    dbContext.SaveChanges();

                    //var body = "Dear User, Your Application Form  has been updated successfully. We assure you of our best services always.Thanks and Regards. Noida Authority";
                    string message = string.Format(OSMessages.OnlineApplicationFormUpdate, detail.onlineapplicationId);
                    OnlineSchemeHelper.SendEmail(model.Email, "Registration Form", message);

                    OnlineSchemeHelper.SendSMS(model.MobileNumber, message);
                }
            }
            return model;
        }

        private int SaveDocumentsForApplicationForm(SchemeFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            int flag = 0; model.Id = model.ApplicationFormId;
            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id)))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id));
            }

            if (files != null && files.Count() > 0)
            {
                if (model.StrDocList != null && model.StrDocList.Length > 0)
                {
                    string[] docsIds = model.StrDocList[0].Split(','); int count = 0;
                    string documents = "Documents";
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + documents)))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + documents));
                    }
                    foreach (var file in files)
                    {
                        if (file != null)
                        {
                            string extension = Path.GetExtension(file.FileName);
                            var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + documents + "/" + model.Id + "-" + docsIds[count] + extension);
                            using (var dbContext = new OSDbContext())
                            {
                                OnlineCheckLisTran checklist = new OnlineCheckLisTran();
                                checklist.CheckListId = Convert.ToInt32(docsIds[count]);
                                checklist.onlineapplicationId = (int)model.Id;
                                checklist.FileNAme = (model.Id + "-" + docsIds[count] + extension).ToString();
                                checklist.CreatedDate = DateTime.Now.Date;
                                checklist.CreatedBy = 0;
                                checklist.isActive = true;
                                dbContext.OnlineCheckLisTrans.Add(checklist);
                                dbContext.SaveChanges();
                            }
                            file.SaveAs(fileSavePath);
                            count++;
                            flag = OSReturnTypeId.Success;
                        }
                    }
                    //30-11-2017 shatrughna
                    string message = string.Format(OSMessages.UploadDocumentSuccess, model.Id);
                    if (!string.IsNullOrEmpty(model.MobileNumber)) { OnlineSchemeHelper.SendSMS(model.MobileNumber, message); }
                }
            }
            if (userImage != null && userImage.ContentLength > 0)
            {
                string picture = "Pictures";
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture)))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture));
                }
                else
                {
                    var dirpath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture);
                    string[] fils = Directory.GetFiles(dirpath);
                    if (fils != null && fils.Count() > 0)
                    {
                        foreach (var fl in fils)
                        {
                            string path1 = dirpath + "\\" + model.Id + "-userphoto" + ".jpg";
                            if (fl == path1) { File.Delete(fl); }
                        }
                    }
                }
                string extension = Path.GetExtension(userImage.FileName);
                var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture + "/" + model.Id + "-userphoto" + extension);
                userImage.SaveAs(fileSavePath);
                flag = OSReturnTypeId.Success;
            }
            if (signatureImage != null && signatureImage.ContentLength > 0)
            {
                string picture = "Pictures";
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture)))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture));
                }
                else
                {
                    string fileExtn = "*signature";
                    var dirpath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture);
                    var fileList = new DirectoryInfo(dirpath).GetFileSystemInfos(fileExtn);
                    //string[] fils = Directory.GetFiles(dirpath);
                    if (fileList != null && fileList.Count() > 0)
                    {
                        foreach (var fl in fileList)
                        {
                            fl.Delete();
                        }
                    }
                }
                string extension = Path.GetExtension(userImage.FileName);
                var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture + "/" + model.Id + "-signature" + extension);
                signatureImage.SaveAs(fileSavePath);
                flag = OSReturnTypeId.Success;
            }

            return flag;
        }

        private NICsingalwindowSystem MapNICSingalWindowSystemTable(NewDataSet nicDataSet)
        {
            NICsingalwindowSystem nicSWS = new NICsingalwindowSystem();
            nicSWS.onlineapplicationId = nicDataSet.Table.OnlineApplicationId;
            nicSWS.schemeId = nicDataSet.Table.SchemeId;
            nicSWS.Departmentid = nicDataSet.Table.departmentId;
            nicSWS.Control_ID = nicDataSet.Table.Control_ID;
            nicSWS.Unit_Id = nicDataSet.Table.Unit_Id;
            nicSWS.ServiceID = nicDataSet.Table.ServiceID;
            nicSWS.ProcessIndustryID = Convert.ToString(nicDataSet.Table.OnlineApplicationId);
            //below lines were commented
            nicSWS.Company_Name = nicDataSet.Table.Company_Name;
            nicSWS.Industry_District = nicDataSet.Table.Industry_District;
            nicSWS.Industry_District_Id = nicDataSet.Table.Industry_District_Id;
            nicSWS.Industry_Address = nicDataSet.Table.Industry_Address;
            nicSWS.Pin_Code = nicDataSet.Table.Pin_Code;
            nicSWS.Occupier_Name = nicDataSet.Table.Occupier_Name;

            nicSWS.Occupier_Email_ID = nicDataSet.Table.Occupier_Email_ID;
            nicSWS.Occupier_Mobile_No = nicDataSet.Table.Occupier_Mobile_No;
            nicSWS.Occupier_DOB = nicDataSet.Table.Occupier_DOB;
            nicSWS.Occupier_Gender = nicDataSet.Table.Occupier_Gender;
            //below lines were commented
            nicSWS.Occupier_Address = nicDataSet.Table.Occupier_Address;
            nicSWS.Occupier_District_ID = nicDataSet.Table.Occupier_District_ID;
            nicSWS.Occupier_District_Name = nicDataSet.Table.Occupier_District_Name;
            nicSWS.Occupier_Pin_Code = nicDataSet.Table.Occupier_Pin_Code;
            nicSWS.Nature_of_Activity = nicDataSet.Table.Nature_of_Activity;
            nicSWS.Installed_Capacity = nicDataSet.Table.Installed_Capacity;
            nicSWS.Employees = nicDataSet.Table.Employees;
            nicSWS.Nature_of_Operation = nicDataSet.Table.Nature_of_Operation;
            nicSWS.publicdecimalProject_Cost = Convert.ToString(nicDataSet.Table.Project_Cost);
            nicSWS.Organization_Type_ID = nicDataSet.Table.Organization_Type_ID;
            nicSWS.Organization_Type = nicDataSet.Table.Organization_Type;
            nicSWS.Industry_Type_ID = nicDataSet.Table.Industry_Type_ID;
            nicSWS.Industry_Type_Name = nicDataSet.Table.Industry_Type_Name;
            nicSWS.Expected_date_construction = nicDataSet.Table.Expected_date_construction;
            nicSWS.Project_Status = nicDataSet.Table.Project_Status;
            nicSWS.Industry_Color = nicDataSet.Table.Industry_Color;
            nicSWS.Expected_date_production = nicDataSet.Table.Expected_date_production;
            nicSWS.Unit_Category = nicDataSet.Table.Unit_Category;
            nicSWS.Items_Manufactured = nicDataSet.Table.Items_Manufactured;

            nicSWS.Annual_Turnover = Convert.ToString(nicDataSet.Table.Annual_Turnover);
            return nicSWS;
        }

        public SchemeFormViewModel SaveDocumentsForOnlineSchemeApplication(SchemeFormViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            int flag = 0; model.Id = model.ApplicationFormId;
            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id)))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id));
            }
            if (files != null && files.Count() > 0)
            {
                if (model.StrDocList != null && model.StrDocList.Length > 0)
                {
                    string[] docsIds = model.StrDocList[0].Split(','); int count = 0;
                    string documents = "Documents";
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + documents)))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + documents));
                    }
                    foreach (var file in files)
                    {
                        if (file != null)
                        {
                            string extension = Path.GetExtension(file.FileName);
                            var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + documents + "/" + model.Id + "-" + docsIds[count] + extension);
                            using (var dbContext = new OSDbContext())
                            {
                                OnlineCheckLisTran checklist = new OnlineCheckLisTran();
                                checklist.CheckListId = Convert.ToInt32(docsIds[count]);
                                checklist.onlineapplicationId = (int)model.Id;
                                checklist.FileNAme = (model.Id + "-" + docsIds[count] + extension).ToString();
                                checklist.CreatedDate = DateTime.Now.Date;
                                checklist.CreatedBy = 0;
                                checklist.isActive = true;
                                dbContext.OnlineCheckLisTrans.Add(checklist);
                                dbContext.SaveChanges();
                            }
                            file.SaveAs(fileSavePath);
                            count++;
                            flag = OSReturnTypeId.Success;
                        }
                    }
                    model.IsDocumentUploaded = true;
                    string message = string.Format(OSMessages.UploadDocumentSuccess, model.Id);
                    if (!string.IsNullOrEmpty(model.MobileNumber)) { OnlineSchemeHelper.SendSMS(model.MobileNumber, message); }
                }
            }
            return model;
        }

        public SchemeFormViewModel SaveImagesForOnlineSchemeApplication(SchemeFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            int flag = 0; model.Id = model.ApplicationFormId;
            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id)))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id));
            }
            if (userImage != null && userImage.ContentLength > 0)
            {
                string picture = "Pictures";
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture)))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture));
                }
                else
                {
                    var dirpath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture);
                    string[] fils = Directory.GetFiles(dirpath);
                    if (fils != null && fils.Count() > 0)
                    {
                        foreach (var fl in fils)
                        {
                            string path1 = dirpath + "\\" + model.Id + "-userphoto" + ".jpg";
                            if (fl == path1) { File.Delete(fl); }
                        }
                    }
                }
                string extension = Path.GetExtension(userImage.FileName);
                var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture + "/" + model.Id + "-userphoto" + extension);
                userImage.SaveAs(fileSavePath);
                flag = OSReturnTypeId.Success;
            }
            if (signatureImage != null && signatureImage.ContentLength > 0)
            {
                string picture = "Pictures";
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture)))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture));
                }
                else
                {
                    string fileExtn = "*signature";
                    var dirpath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture);
                    var fileList = new DirectoryInfo(dirpath).GetFileSystemInfos(fileExtn);
                    //string[] fils = Directory.GetFiles(dirpath);
                    if (fileList != null && fileList.Count() > 0)
                    {
                        foreach (var fl in fileList)
                        {
                            fl.Delete();
                        }
                    }
                }
                string extension = Path.GetExtension(userImage.FileName);
                var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + picture + "/" + model.Id + "-signature" + extension);
                signatureImage.SaveAs(fileSavePath);
                flag = OSReturnTypeId.Success;
            }
            return model;
        }

        public SchemeFormViewModel GetOnlineSchemeApplicationFormById(int? id)
        {
            using (var dbContext = new OSDbContext())
            {
                SchemeFormViewModel applicant = new SchemeFormViewModel();
                var detail = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == id && c.isActive == true);
                if (detail != null)
                {
                    if (detail.RefundBankId != null)
                    {
                        applicant = (from appl in dbContext.OnlineApplicationDetails
                                     join dept in dbContext.DepartmentMsts on appl.departmentId equals dept.departmentId
                                     join scem in dbContext.SchemeMsts on appl.schemeId equals scem.schemeId
                                     join bank in dbContext.BankMsts on appl.RefundBankId equals bank.bankId
                                     join area in dbContext.FloorMsts on appl.area equals area.floorId.ToString()
                                     where appl.onlineapplicationId == id
                                     select new SchemeFormViewModel
                                     {
                                         Id = appl.onlineapplicationId,
                                         ApplicationFormId = appl.onlineapplicationId,
                                         SchemeId = scem.schemeId,
                                         SchemeName = scem.schemeName,
                                         SchemeType = scem.SchemeTypeMst.SchemeTypeDesc,
                                         SchemeTypeId = scem.schemeTypeId,
                                         DepartmentId = dept.departmentId,
                                         Department = dept.departmentName,
                                         Applicant = appl.gender == "Company" ? appl.CompanyName : appl.firstName + " " + appl.middleName + " " + appl.lastName,
                                         SigningAuthority = appl.signingAuthority,
                                         ApplicantType = appl.gender == "Company" ? "Company" : "Individual",
                                         CompanyTypeId = appl.CompanyType,
                                         CompanyType = dbContext.Common_Config.Where(c => c.Id == appl.CompanyType).Select(c => c.Name).FirstOrDefault(),
                                         FirstName = appl.firstName,
                                         MiddleName = appl.middleName,
                                         LastName = appl.lastName,
                                         Gender = appl.gender,
                                         MaritalStatus = appl.marritalStatus,
                                         DOB = appl.dateOfBirth,
                                         FatherName = (!string.IsNullOrEmpty(appl.fatherHusbandName)) ? appl.fatherHusbandName : "NA",
                                         ApplicantMaster = (!string.IsNullOrEmpty(appl.fatherHusbandName)) ? appl.fatherHusbandName : "NA",
                                         MotherName = appl.motherName,
                                         OccupationId = appl.occupationId,
                                         Occupation = dbContext.OccupationMsts.Where(c => c.occupationId == appl.occupationId).Select(s => s.occupation).FirstOrDefault(),
                                         ReligionId = appl.religionId,
                                         Religion = dbContext.ReligionMsts.Where(r => r.religionId == appl.religionId).Select(r => r.religion).FirstOrDefault(),
                                         CategoryId = appl.quotaId,
                                         CategoryName = dbContext.QuotaMsts.Where(c => c.quotaId == appl.quotaId).Select(q => q.quotaName).FirstOrDefault(),
                                         MobileNumber = appl.mobileNumberP2,
                                         PhoneNumber = appl.phoneNumberP2,
                                         FaxNumber = appl.faxNumberP2,
                                         ApplicantGSTNumber = appl.GSTNO,
                                         PermanentAddress = appl.permanentAdd,
                                         CorrespondingAddress = appl.correspondanceAdd,
                                         RegisteredOffice = appl.registeredOffice,
                                         Email = appl.email,
                                         AnnualIncome = appl.annualIncome,
                                         EarnestMoney = appl.EarnestMoney,
                                         ApplicationFee = appl.ApplicationFee,
                                         FormFeeSGST = appl.FormSGST,
                                         FormFeeCGST = appl.FormCGST,
                                         ProcessingCharge = appl.ProcessingCharge,
                                         ProcessingCGST = appl.ProcessingCGST,
                                         ProcessingSGST = appl.ProcessingSGST,
                                         TotalAmount = appl.TotalAmount,
                                         PanNumber = appl.pan,
                                         RefundBankId = appl.RefundBankId,
                                         RefundBank = bank.bankName,
                                         BranchName = appl.BranchName,
                                         RefundInfaverof = appl.RefundInfaverof,
                                         RefundAccountNo = appl.RefundaccountNo,
                                         IFSCCode = appl.IFSCCode,
                                         AadharNumber = appl.AadharNumber,
                                         Sector = appl.Sector,
                                         PropertyNo = appl.PropertyNo,
                                         ApplicationFormType = appl.FormCategory.Trim(),
                                         ExpansionType = appl.FormSubCategory.Trim(),
                                         ExistingProperty = appl.ExistingPropertyNo,
                                         AllottmentDate = appl.AllotmentDate,
                                         LetterCode = appl.RentingLetterNo,
                                         LetterDate = appl.RentingDate,
                                         DispatchDate = appl.DispatchDate,
                                         AreaRangeId = appl.area,
                                         AreaRange = dbContext.FloorMsts.Where(f => f.floorId.ToString() == appl.area).Select(f => f.floorName).FirstOrDefault(),
                                         PropertyTypeId = appl.PropertyTypeID,
                                         PropertyType = dbContext.PropertyTypeMsts.FirstOrDefault(p => p.departmentId == appl.departmentId && p.propertyTypeId == appl.PropertyTypeID && p.IsActive == true).propertyTypeName,
                                         ChecklistHtml = appl.Documentfilename,
                                         FormType = appl.Online_offline == "Y" ? "Online" : "Offline",

                                         EPFRegistrationNo = appl.RentingPropertyNo,
                                         ESIRegistrationNo = appl.ExistingPropertyNo,

                                         //DirectorModel = new onlineDirectorViewModel
                                         //{
                                         //    DirectorType = appl.gender == "Company" ? dbContext.Common_Config.Where(m => m.Id == appl.occupationId).FirstOrDefault().Name : string.Empty
                                         //},
                                         //ProposedModel = new ProposedCompanyViewModel
                                         //{
                                         //    ProposedProject = appl.projectname,
                                         //    ImplementationTime = appl.projecttimeempl,
                                         //    TotalCost = appl.projectcost
                                         //},
                                         ////PreviousFormNo = appl.Comment != null ? appl.Comment : string.Empty,
                                         SchemeProposedFirm = new SchemeProposedFirmViewModel{
                                             DirectorType = appl.gender == "Company" ? dbContext.Common_Config.Where(m => m.Id == appl.occupationId).FirstOrDefault().Name : string.Empty,
                                             ProposedProject = appl.projectname,
                                             ImplementationTime = appl.projecttimeempl,
                                             TotalCost = appl.projectcost
                                         },
                                         PreviousFormNo = appl.PreviousFormNo != null ? appl.PreviousFormNo : string.Empty,
                                         AppType = dbContext.NICsingalwindowSystems.Where(x => x.onlineapplicationId == id).FirstOrDefault() != null ? OSStringConstant.NIC : null
                                     }).FirstOrDefault();
                    }
                    else
                    {
                        applicant = (from appl in dbContext.OnlineApplicationDetails
                                     join dept in dbContext.DepartmentMsts on appl.departmentId equals dept.departmentId
                                     join scem in dbContext.SchemeMsts on appl.schemeId equals scem.schemeId
                                     //join bank in dbContext.BankMsts on appl.RefundBankId equals bank.bankId
                                     //join area in dbContext.FloorMsts on appl.area equals area.floorId.ToString()
                                     where appl.onlineapplicationId == id
                                     select new SchemeFormViewModel
                                     {
                                         Id = appl.onlineapplicationId,
                                         ApplicationFormId = appl.onlineapplicationId,
                                         SchemeId = scem.schemeId,
                                         SchemeTypeId = scem.schemeTypeId,
                                         SchemeName = scem.schemeName,
                                         SchemeType = scem.SchemeTypeMst.SchemeTypeDesc,
                                         DepartmentId = dept.departmentId,
                                         Department = dept.departmentName,
                                         Applicant = appl.gender == "Company" ? appl.CompanyName : appl.firstName + " " + appl.middleName + " " + appl.lastName,
                                         SigningAuthority = appl.signingAuthority,
                                         ApplicantType = appl.gender == "Company" ? "Company" : "Individual",
                                         CompanyTypeId = appl.CompanyType,
                                         CompanyType = dbContext.Common_Config.Where(c => c.Id == appl.CompanyType).Select(c => c.Name).FirstOrDefault(),
                                         FirstName = appl.firstName,
                                         MiddleName = appl.middleName,
                                         LastName = appl.lastName,
                                         Gender = appl.gender,
                                         MaritalStatus = appl.marritalStatus,
                                         DOB = appl.dateOfBirth,
                                         FatherName = (!string.IsNullOrEmpty(appl.fatherHusbandName)) ? appl.fatherHusbandName : "NA",
                                         ApplicantMaster = (!string.IsNullOrEmpty(appl.fatherHusbandName)) ? appl.fatherHusbandName : "NA",
                                         MotherName = appl.motherName,
                                         OccupationId = appl.occupationId,
                                         Occupation = dbContext.OccupationMsts.Where(c => c.occupationId == appl.occupationId).Select(s => s.occupation).FirstOrDefault(),
                                         ReligionId = appl.religionId,
                                         Religion = dbContext.ReligionMsts.Where(r => r.religionId == appl.religionId).Select(r => r.religion).FirstOrDefault(),
                                         CategoryId = appl.quotaId,
                                         CategoryName = dbContext.QuotaMsts.Where(c => c.quotaId == appl.quotaId).Select(q => q.quotaName).FirstOrDefault(),
                                         MobileNumber = appl.mobileNumberP2,
                                         PhoneNumber = appl.phoneNumberP2,
                                         FaxNumber = appl.faxNumberP2,
                                         ApplicantGSTNumber = appl.GSTNO,
                                         PermanentAddress = appl.permanentAdd,
                                         CorrespondingAddress = appl.correspondanceAdd,
                                         RegisteredOffice = appl.registeredOffice,
                                         Email = appl.email,
                                         AnnualIncome = appl.annualIncome,
                                         EarnestMoney = appl.EarnestMoney,
                                         ApplicationFee = appl.ApplicationFee,
                                         FormFeeSGST = appl.FormSGST,
                                         FormFeeCGST = appl.FormCGST,
                                         ProcessingCharge = appl.ProcessingCharge,
                                         ProcessingCGST = appl.ProcessingCGST,
                                         ProcessingSGST = appl.ProcessingSGST,
                                         TotalAmount = appl.TotalAmount,
                                         PanNumber = appl.pan,
                                         RefundBankId = appl.RefundBankId,
                                         RefundBank = "",
                                         BranchName = appl.BranchName,
                                         RefundInfaverof = appl.RefundInfaverof,
                                         RefundAccountNo = appl.RefundaccountNo,
                                         IFSCCode = appl.IFSCCode,
                                         AadharNumber = appl.AadharNumber,
                                         Sector = appl.Sector,
                                         PropertyNo = appl.PropertyNo,
                                         ApplicationFormType = appl.FormCategory.Trim(),
                                         ExpansionType = appl.FormSubCategory.Trim(),
                                         ExistingProperty = appl.ExistingPropertyNo,
                                         AllottmentDate = appl.AllotmentDate,
                                         LetterCode = appl.RentingLetterNo,
                                         LetterDate = appl.RentingDate,
                                         DispatchDate = appl.DispatchDate,
                                         AreaRangeId = appl.area,
                                         AreaRange = dbContext.FloorMsts.Where(f => f.floorId.ToString() == appl.area).Select(f => f.floorName).FirstOrDefault(),
                                         PropertyTypeId = appl.PropertyTypeID,
                                         PropertyType = dbContext.PropertyTypeMsts.FirstOrDefault(p => p.departmentId == appl.departmentId && p.propertyTypeId == appl.PropertyTypeID && p.IsActive == true).propertyTypeName,
                                         ChecklistHtml = appl.Documentfilename,
                                         FormType = appl.Online_offline == "Y" ? "Online" : "Offline",

                                         EPFRegistrationNo = appl.RentingPropertyNo,
                                         ESIRegistrationNo = appl.ExistingPropertyNo,

                                         //DirectorModel = new onlineDirectorViewModel
                                         //{
                                         //    DirectorType = appl.gender == "Company" ? dbContext.Common_Config.Where(m => m.Id == appl.occupationId).FirstOrDefault().Name : string.Empty
                                         //},
                                         //ProposedModel = new ProposedCompanyViewModel
                                         //{
                                         //    ProposedProject = appl.projectname,
                                         //    ImplementationTime = appl.projecttimeempl,
                                         //    TotalCost = appl.projectcost
                                         //},
                                         ////PreviousFormNo = appl.Comment != null ? appl.Comment : string.Empty,
                                         SchemeProposedFirm = new SchemeProposedFirmViewModel
                                         {
                                             DirectorType = appl.gender == "Company" ? dbContext.Common_Config.Where(m => m.Id == appl.occupationId).FirstOrDefault().Name : string.Empty,
                                             ProposedProject = appl.projectname,
                                             ImplementationTime = appl.projecttimeempl,
                                             TotalCost = appl.projectcost
                                         },
                                         PreviousFormNo = appl.PreviousFormNo != null ? appl.PreviousFormNo : string.Empty,
                                         AppType = dbContext.NICsingalwindowSystems.Where(x => x.onlineapplicationId == id).FirstOrDefault() != null ? OSStringConstant.NIC : null
                                     }).FirstOrDefault();
                    }

                    var banklist = (from scheme in dbContext.SchemeBankTrans
                               where scheme.IsActive == true && scheme.schemeId == applicant.SchemeId
                               select new OSDropdownViewModel
                               {
                                   Id = scheme.BankMst.bankId,
                                   Text = scheme.BankMst.bankName
                               }).ToList();

                    if (banklist != null)
                    {
                        applicant.SchemeBankList = banklist;
                    }

                    if (applicant.Gender.ToLower() == OSStringConstant.Company.ToLower())
                    {
                        applicant.SigningAuthorityId = applicant.CategoryId;
                        if (applicant.CategoryId != null && applicant.CategoryId > 0)
                        {
                            applicant.SignatoryStatus = dbContext.Common_Config.FirstOrDefault(x => x.Id == applicant.CategoryId).Name;
                        }
                    }

                    applicant.FormFeeGST = applicant.FormFeeCGST + applicant.FormFeeSGST;
                    applicant.FormFeeWithGST = applicant.ApplicationFee + applicant.FormFeeGST;
                    applicant.ProcessingChargeGST = applicant.ProcessingSGST + applicant.ProcessingCGST;
                    applicant.ProcessingChargeWithGST = applicant.ProcessingCharge + applicant.ProcessingChargeGST;

                    //var doctype = applicant.ChecklistHtml.Split(',');
                    if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + id + "/Pictures/")))
                    {
                        var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + id + "/Pictures/");
                        string[] files = Directory.GetFiles(filepath);
                        if (files != null)
                        {
                            if (files.Count() > 0)
                            {
                                if (files.Count() == 2)
                                {
                                    if (files[0] != null)
                                    {
                                        var uplod = files[0].Split('\\');
                                        applicant.SignImage = "/UploadFiles/" + id + "/Pictures/" + uplod[uplod.Length - 1];
                                    }
                                    if (files[1] != null)
                                    {
                                        var uplod = files[1].Split('\\');
                                        applicant.UserImage = "/UploadFiles/" + id + "/Pictures/" + uplod[uplod.Length - 1];
                                    }
                                }
                            }
                        }
                    }

                    //Scheme form payment check;
                    var _PaymentList = new List<SchemePaymentViewModel>();
                    var formpay = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(t => t.ServiceRefId == applicant.ApplicationFormId && (t.ServiceType == OSConstant.SchemeFormPayment || t.ServiceType == OSConstant.SchemeBrochurePayment) && t.TranStatus == 1 && t.status == 1);
                    applicant.IsApplicationFeePaid = formpay != null ? true : false;
                    if (formpay != null)
                    {
                        var _FormPayment = GetOnlineSchemePaymentDetailById(id, formpay.AutoID);
                        _PaymentList.Add(_FormPayment);
                    }

                    var OEMPayment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(t => t.ServiceRefId == applicant.ApplicationFormId && t.ServiceType == OSConstant.ReservationMoneyOnlinePayment && t.TranStatus == 1 && t.status == 1);
                    if (OEMPayment != null)
                    {
                        applicant.IsReservationFeePaid = true;
                        var resmoney = GetOnlineSchemePaymentDetailById(id, OEMPayment.AutoID);
                        _PaymentList.Add(resmoney);
                    }
                   
                    var challan = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(t => t.ServiceRefId == applicant.ApplicationFormId && t.ServiceType == OSConstant.ReservationMoneyChallanPayment);
                    applicant.IsChallanGenerated = challan != null ? true : false;
                    if (challan != null)
                    {
                        applicant.PaymentMode = (challan.status == 1 && challan.TranStatus == 1) ? "Offline_Validated" : (!string.IsNullOrEmpty(challan.TrKey) ? "Offline_Updated" : "Offline");
                        applicant.IsReservationFeePaid = (challan.status == 1 && challan.TranStatus == 1) ? true : false;

                        var _EarnestMoneyPayment = GetOnlineSchemePaymentDetailById(id, challan.AutoID);
                        _PaymentList.Add(_EarnestMoneyPayment);
                        applicant.SchemePayment = _EarnestMoneyPayment;
                    }
                    
                    applicant.IsPaidThroughSWP = false;
                    if (applicant.AppType == OSStringConstant.NIC)
                    {
                        //check single window portal
                        var singleWindowPortal = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(p => p.ServiceRefId == detail.onlineapplicationId && p.ServiceType == OSConstant.NICSchemeFormPayment);
                        if (singleWindowPortal != null)
                        {
                            applicant.IsPaidThroughSWP = true;
                            if (singleWindowPortal.TranStatus == 1 && singleWindowPortal.status == 1)
                            {
                                applicant.IsApplicationFeePaid = true;
                                applicant.ServiceAppPayStatus = "Paid";
                            }
                            applicant.SchemePayment = GetOnlineSchemePaymentDetailById(id, singleWindowPortal.AutoID);
                        }
                    }

                    var flag = IsDocumentForSchemeFormUploaded(id);
                    if (flag == true)
                    {
                        applicant.IsDocumentUploaded = true;
                        //applicant.DocumentsTable = dbContext.Sp_NewSchemereturn(applicant.ApplicationFormId, applicant.SchemeId).FirstOrDefault();
                        applicant.DocumentsTable = dbContext.ssp_SchemeFormUploadedDocument(applicant.ApplicationFormId, applicant.SchemeId).FirstOrDefault();
                    }
                    //else applicant.IsDocumentUploaded = false;

                    if (applicant.SchemeType == SchemeConstant.Transport || applicant.SchemeType == SchemeConstant.OpenEnded)
                    {
                        var PreChallan = IsPreviousChallanUploadedForSchemeForm(id);
                        if (PreChallan == true)
                        {
                            applicant.IsPreviousChallanUploaded = true;
                        }
                    }
                }
                return applicant;
            }
        }

        private SchemePaymentViewModel GetOnlineSchemePaymentDetailById(int? id, int? AutoId)
        {
            using (var dbContext = new OSDbContext())
            {
                var detail = (from transaction in dbContext.OnlineApplicationDetails_trans
                              where transaction.ServiceRefId == id && transaction.AutoID == AutoId
                              select new SchemePaymentViewModel
                              {
                                  Id = id,
                                  ApplicationFormId = transaction.ServiceRefId,
                                  TransactionKey = transaction.TrKey,
                                  TransactionId = transaction.txnid,
                                  Amount = transaction.Amount,
                                  ProductInfo = transaction.productinfo,
                                  Mode = transaction.mode,
                                  TransactionStatusId = transaction.TranStatus,
                                  Discount = transaction.discount,
                                  StatusId = transaction.status,
                                  EntryDate = transaction.EntryDate,
                                  Status = transaction.TranStatus == OSConstant.Success ? "Success" : "Failure",
                                  PaymentSource = transaction.payment_source,
                                  BankReferenceNo = transaction.bank_ref_num,
                                  BankCode = transaction.bankcode,
                                  Error = transaction.error,
                                  ErrorMessage = transaction.error_Message,
                                  NameOnCard = transaction.name_on_card,
                                  CardNumber = transaction.cardnum,
                                  CardHash = transaction.cardhash,
                                  IssuingBank = transaction.issuing_bank,
                                  CardType = transaction.card_type,
                                  Mihpayid = transaction.mihpayid,
                                  modifiydate = transaction.modifiydate,
                                  BankName = transaction.ServiceType == 3 ? dbContext.BankMsts.Where(m => m.bankId.ToString() == transaction.GetwayName && m.IsActive == true).FirstOrDefault().bankName : (transaction.ServiceType == 5 ? "Single Window Portal" : string.Empty)
                              }).FirstOrDefault();
                return detail;
            }
        }

        public bool IsDocumentForSchemeFormUploaded(int? formId)
        {
            var flag = false;
            if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formId + "/Documents/")))
            {
                var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formId + "/Documents/");
                string[] files = Directory.GetFiles(filepath);
                if (files != null)
                {
                    if (files.Count() > 0) flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }

        public bool IsPreviousChallanUploadedForSchemeForm(int? formId)
        {
            var flag = false;
            if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formId + "/PreviousChallan/")))
            {
                var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formId + "/PreviousChallan/");
                string[] files = Directory.GetFiles(filepath);
                if (files != null)
                {
                    if (files.Count() > 0) flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }

        public DataSourceResult GetUploadedDocumentsAsDataSource(DataSourceRequest request, SchemeFormViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                List<SchemeDocumentViewModel> docs = new List<SchemeDocumentViewModel>();
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(f => f.onlineapplicationId == model.ApplicationFormId);
                var documents = dbContext.OnlineCheckListMasters.ToList();
                if (form != null)
                {
                    docs = (from doc in dbContext.OnlineCheckListMasters
                            where doc.IsActive == true && doc.DepartmentId == model.DepartmentId //&& doc.SchemeId == form.schemeId
                            select new SchemeDocumentViewModel
                            {
                                Id = doc.CheckListId,
                                DocumentId = doc.CheckListId,
                                DocumentName = doc.CheckListName,
                                ParentDocumentType = doc.MainList
                            }).ToList();
                    if (form.ScrutinyStatusId == 2)
                    {
                        var scrutinydocs = (from doc in dbContext.OnlineCheckListMasters
                                            where doc.IsActive == true && doc.CheckListType == OSStringConstant.SCRUTINY //&& doc.SchemeId == form.schemeId // configSchemeId
                                            select new SchemeDocumentViewModel
                                            {
                                                Id = doc.CheckListId,
                                                DocumentId = doc.CheckListId,
                                                DocumentName = doc.CheckListName,
                                                ParentDocumentType = doc.MainList
                                            }).ToList();
                        docs.AddRange(scrutinydocs);
                    }
                    if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.ApplicationFormId + "/Documents/")))
                    {
                        var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.onlineapplicationId + "/Documents/");
                        string[] files = Directory.GetFiles(filepath);
                        if (files != null)
                        {

                            for (int x = 0; x < files.Length; x++)
                            {
                                var ufile = files[x].Split('\\');
                                var filename = ufile[ufile.Length - 1];
                                int start = (filename.LastIndexOf('-') + 1);
                                int end = filename.LastIndexOf('.');
                                int dif = end - start;
                                int nod = Convert.ToInt32(filename.Substring(start, dif));
                                docs.Where(m => m.Id == nod).FirstOrDefault().PathName = "/UploadFiles/" + model.ApplicationFormId + "/Documents/" + filename;
                                docs.Where(m => m.Id == nod).FirstOrDefault().UploadedDocument = filename;
                            }
                        }
                    }
                    docs = docs.Where(m => !string.IsNullOrEmpty(m.PathName)).ToList();
                    int counter = 1;
                    docs.ForEach(x => x.Sno = counter++);
                    return docs.ToDataSourceResult(request);
                }
                return null;
            }
        }

        public SchemePaymentViewModel SaveOnlineSchemePaymentTransaction(SchemeFormViewModel form)
        {
            using (var dbContext = new OSDbContext())
            {
                string transid = string.Empty;
                //string trxkey = ApplicationHelper.GenerateTransactionId();
                string trxkey = OnlineSchemeHelper.GeneratePaymentTransactionId();
                var transactionId = dbContext.OnlineApplicationDetails_trans.Where(f => f.ServiceRefId == form.ApplicationFormId).OrderByDescending(x => x.AutoID).Select(x => x.txnid).FirstOrDefault();
                if (transactionId == null)
                {
                    int _Institutional = Convert.ToInt32(ConfigurationManager.AppSettings["InstitutionalDepartmentId"]);
                    
                    OnlineApplicationDetails_trans trans = new OnlineApplicationDetails_trans
                    {
                        ServiceRefId = form.ApplicationFormId,
                        TrKey = trxkey,
                        txnid = form.ApplicationFormId.ToString() + "-1",
                        Amount = form.PayType == "FormPayment" ? form.FormFeeWithGST : form.TotalAmount,
                        productinfo = "Online",
                        mode = "Online",
                        ServiceType = form.DepartmentId != _Institutional ? OSConstant.SchemeFormPayment : OSConstant.SchemeBrochurePayment,
                        GetwayName = "IND",
                        status = 0,
                        EntryDate = DateTime.Now
                    };
                    dbContext.OnlineApplicationDetails_trans.Add(trans);
                    dbContext.SaveChanges();
                    transid = trans.txnid;
                }
                else
                {
                    var temp = transactionId.Split('-').Last();
                    int newTxId = Convert.ToInt32(temp) + 1;
                    OnlineApplicationDetails_trans trans = new OnlineApplicationDetails_trans
                    {
                        ServiceRefId = form.ApplicationFormId,
                        TrKey = trxkey,
                        txnid = form.ApplicationFormId.ToString() + "-" + newTxId,
                        Amount = form.PayType == "FormPayment" ? form.FormFeeWithGST : form.TotalAmount,
                        productinfo = "Online",
                        mode = "Online",
                        ServiceType = OSConstant.SchemeFormPayment,
                        GetwayName = "IND",
                        status = 0,
                        EntryDate = DateTime.Now
                    };

                    dbContext.OnlineApplicationDetails_trans.Add(trans);
                    dbContext.SaveChanges();
                    transid = trans.txnid;
                }

                var model = (from trans in dbContext.OnlineApplicationDetails_trans
                             join appl in dbContext.OnlineApplicationDetails on trans.ServiceRefId equals appl.onlineapplicationId
                             where trans.txnid == transid
                             select new SchemePaymentViewModel
                             {
                                 ApplicationFormId = trans.ServiceRefId,
                                 TransactionKey = trans.TrKey,
                                 TransactionId = trans.txnid,
                                 Amount = trans.Amount,
                                 ProductInfo = trans.productinfo,
                                 Mode = trans.mode,
                                 TransactionStatusId = trans.TranStatus,
                                 Discount = trans.discount,
                                 StatusId = trans.status,
                                 EntryDate = trans.EntryDate,
                                 FirstName = form.FirstName,
                                 Email = form.Email,
                                 PhoneNumber = form.MobileNumber
                             }).FirstOrDefault();
                return model;

            }
        }

        public SchemePaymentViewModel GetOnlineSchemePaymentTransactionDetailById(SchemePaymentViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                if (!string.IsNullOrEmpty(model.TransactionId))
                {
                    var PaymentModel = (from transaction in dbContext.OnlineApplicationDetails_trans
                                        join details in dbContext.OnlineApplicationDetails on transaction.ServiceRefId equals details.onlineapplicationId
                                        where transaction.txnid == model.TransactionId
                                        select new SchemePaymentViewModel
                                        {
                                            ApplicationFormId = transaction.ServiceRefId,
                                            TransactionKey = transaction.TrKey,
                                            TransactionId = transaction.txnid,
                                            Amount = transaction.Amount,
                                            ProductInfo = transaction.productinfo,
                                            Mode = transaction.mode,
                                            TransactionStatusId = transaction.TranStatus,
                                            Discount = transaction.discount,
                                            StatusId = transaction.status,
                                            EntryDate = transaction.EntryDate,
                                            Applicant = details.gender == OSStringConstant.Company ? details.CompanyName : details.firstName + " " + details.middleName + "" + details.lastName,
                                            FirstName = details.gender == OSStringConstant.Company ? details.CompanyName : details.firstName + " " + details.middleName + "" + details.lastName,
                                            Email = details.email,
                                            Mobile = details.mobileNumberP2,
                                            PhoneNumber = details.phoneNumberP2,
                                            Status = transaction.TranStatus == OSConstant.Success ? "Success" : "Failure",
                                            PaymentSource = transaction.payment_source,
                                            BankReferenceNo = transaction.bank_ref_num,
                                            BankCode = transaction.bankcode,
                                            BankName = transaction.bankcode != null ? dbContext.BankMsts.FirstOrDefault(b => b.bankId.ToString() == transaction.bankcode).bankName : string.Empty,
                                            AccountNo = "Online",
                                            Error = transaction.error,
                                            ErrorMessage = transaction.error_Message,
                                            NameOnCard = transaction.name_on_card,
                                            CardNumber = transaction.cardnum,
                                            CardHash = transaction.cardhash,
                                            IssuingBank = transaction.issuing_bank,
                                            CardType = transaction.card_type,
                                            Mihpayid = transaction.mihpayid
                                        }).FirstOrDefault();
                    if (PaymentModel.ApplicationFormId != null && PaymentModel.ApplicationFormId > 0)
                    {
                        var _SchemeForm = GetOnlineSchemeApplicationFormById(PaymentModel.ApplicationFormId);
                        if (_SchemeForm != null)
                        {
                            PaymentModel.SchemeForm = _SchemeForm;
                        }
                    }
                    return PaymentModel;
                }
                else
                {
                    var _PaymentModel = (from transaction in dbContext.OnlineApplicationDetails_trans
                                                 join details in dbContext.OnlineApplicationDetails on transaction.ServiceRefId equals details.onlineapplicationId
                                                 where transaction.ServiceType == 3 && transaction.status == 0 && details.isActive == true && details.onlineapplicationId == model.ApplicationFormId
                                                 select new SchemePaymentViewModel
                                                 {
                                                     ApplicationFormId = transaction.ServiceRefId,
                                                     TransactionKey = transaction.TrKey,
                                                     TransactionId = transaction.txnid,
                                                     Amount = transaction.Amount,
                                                     ProductInfo = transaction.productinfo,
                                                     Mode = transaction.mode,
                                                     TransactionStatusId = transaction.TranStatus,
                                                     Discount = transaction.discount,
                                                     StatusId = transaction.status,
                                                     EntryDate = transaction.EntryDate,
                                                     Applicant = details.gender == OSStringConstant.Company ? details.CompanyName : details.firstName + " " + details.middleName + "" + details.lastName,
                                                     FirstName = details.gender == OSStringConstant.Company ? details.CompanyName : details.firstName + " " + details.middleName + "" + details.lastName,
                                                     Email = details.email,
                                                     PhoneNumber = details.mobileNumberP1,
                                                     Status = transaction.TranStatus == OSConstant.Success ? "Success" : "Failure",
                                                     PaymentSource = transaction.payment_source,
                                                     BankReferenceNo = transaction.bank_ref_num,
                                                     BankCode = transaction.bankcode,
                                                     Error = transaction.error,
                                                     ErrorMessage = transaction.error_Message,
                                                     NameOnCard = transaction.name_on_card,
                                                     CardNumber = transaction.cardnum,
                                                     CardHash = transaction.cardhash,
                                                     IssuingBank = transaction.issuing_bank,
                                                     CardType = transaction.card_type,
                                                     Mihpayid = transaction.mihpayid,
                                                     modifiydate = transaction.modifiydate,
                                                     BankName = dbContext.BankMsts.Where(m => m.bankId.ToString() == transaction.GetwayName && m.IsActive == true).FirstOrDefault().bankName,
                                                     SchemeForm = new SchemeFormViewModel
                                                     {
                                                         ApplicationFee = details.ApplicationFee,
                                                         ProcessingCharge = details.ProcessingCharge,
                                                         FormFeeGST = details.ApplicationFee + details.FormCGST + details.FormSGST,
                                                         ProcessingChargeGST = details.ProcessingCharge + details.ProcessingSGST + details.ProcessingCGST,
                                                         FormFeeSGST = details.FormSGST,
                                                         FormFeeCGST = details.FormCGST,
                                                         ProcessingSGST = details.ProcessingSGST,
                                                         ProcessingCGST = details.ProcessingCGST,
                                                         TotalAmount = details.ApplicationFee + details.ProcessingCharge + details.EarnestMoney,
                                                         EarnestMoney = details.EarnestMoney,
                                                         TotalAmountGST = (details.ProcessingCharge + details.ProcessingSGST + details.ProcessingCGST) + (details.ApplicationFee + details.FormCGST + details.FormSGST) + details.EarnestMoney,
                                                     }
                                                 }).FirstOrDefault();

                    string DocType = "Challan";
                    string filePath = string.Empty;
                    if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.ApplicationFormId + "/" + DocType + "/")))
                    {
                        var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.ApplicationFormId + "/" + DocType + "/");
                        string[] files = Directory.GetFiles(filepath);
                        if (files != null)
                        {
                            if (files.Count() > 0)
                            { 
                                string[] uFile = files[0].Split('\\'); 
                                string fileName = uFile[(uFile).Length - 1];
                                filePath = "/UploadFiles/" + model.ApplicationFormId + "/" + DocType + "/" + fileName; 
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(filePath)) { _PaymentModel.docPath = filePath; }
                    return _PaymentModel;
                }
            }
        }

        public SchemeFormViewModel SaveOpenEndedSchemeFormDetail(SchemeFormViewModel model, System.Web.HttpPostedFileBase userImage, System.Web.HttpPostedFileBase signatureImage)
        {
            string encryptedId = string.Empty;
            int nicmsg = 0;
            using (var dbContext = new OSDbContext())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        OnlineApplicationDetail application = new OnlineApplicationDetail();
                        model.Id = model.Id == null ? 0 : model.Id;
                        var detail = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == model.Id);
                        if (detail == null)
                        {
                            if (model.ApplicantType == "Individual")
                            {
                                application.firstName = model.FirstName;
                                application.middleName = model.MiddleName;
                                application.lastName = model.LastName;
                                application.fatherHusbandName = model.FatherName;
                                application.motherName = model.MotherName;
                                application.gender = model.Gender;
                                application.religionId = model.ReligionId;
                                application.occupationId = model.OccupationId;
                                application.quotaId = OSConstant.General;
                                application.marritalStatus = model.MaritalStatus;
                                application.dateOfBirth = model.DOB;
                            }
                            else
                            {
                                application.CompanyName = model.Applicant;
                                application.signingAuthority = model.SigningAuthority;
                                application.registeredOffice = model.PermanentAddress;
                                application.CompanyType = model.CompanyTypeId;
                                application.gender = string.IsNullOrEmpty(model.ApplicantType) ? OSStringConstant.Company : model.ApplicantType;
                                application.fatherHusbandName = model.ApplicantMaster;
                                application.quotaId = model.SigningAuthorityId;
                            }
                            application.CompanyType = model.CompanyTypeId;
                            application.schemeId = model.SchemeId;
                            application.departmentId = model.DepartmentId;
                            application.permanentAdd = model.PermanentAddress;
                            application.correspondanceAdd = model.CorrespondingAddress;
                            application.area = model.AreaRangeId;
                            application.mobileNumberP2 = model.MobileNumber;
                            application.phoneNumberP2 = model.PhoneNumber;
                            application.faxNumberP2 = model.FaxNumber;
                            application.email = model.Email;
                            application.annualIncome = model.AnnualIncome;
                            application.ApplicationFee = model.ApplicationFee;
                            application.FormCGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.FormSGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.ProcessingCharge = model.ProcessingCharge;
                            application.ProcessingCGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.ProcessingSGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                            application.EarnestMoney = model.EarnestMoney;
                            application.TotalAmount = model.TotalAmount;
                            application.pan = model.PanNumber;
                            application.AadharNumber = model.AadharNumber;
                            application.GSTNO = model.ApplicantGSTNumber;
                            application.ApplicationDate = DateTime.Now;
                            application.PropertyTypeID = model.PropertyTypeId;
                            application.RefundBankId = model.RefundBankId;
                            application.RefundInfaverof = model.RefundInfaverof;
                            application.RefundaccountNo = model.RefundAccountNo;
                            application.BranchName = model.BranchName;
                            application.IFSCCode = model.IFSCCode;

                            application.Sector = model.Sector;
                            application.PropertyNo = model.PropertyNo;
                            application.ExistingPropertyNo = model.ExistingProperty;
                            application.AllotmentDate = model.AllottmentDate;
                            application.DispatchDate = model.DispatchDate;

                            application.FormCategory = model.ApplicationFormType;
                            
                            application.createdBy = "Online";
                            application.createdDate = DateTime.Now;
                            application.isActive = true;
                            application.IsSubmited = false;
                            application.Online_offline = model.FormType == "Online" ? "Y" : "N";

                            //string _password = OnlineSchemeHelper.GenerateSchemeFormPassword();
                            string _password = "NDA" + DateTime.Now.Year.ToString();
                            _password = _password.MD5HashPassword();
                            application.Userpassword = _password;

                            dbContext.OnlineApplicationDetails.Add(application);
                            dbContext.SaveChanges();

                            model.Id = application.onlineapplicationId;
                            model.ApplicationFormId = application.onlineapplicationId;
                            
                            if (!string.IsNullOrEmpty(model.NICControlId))
                            {
                                Table xmlTbl = new Table();
                                xmlTbl.OnlineApplicationId = application.onlineapplicationId;
                                xmlTbl.departmentId = application.departmentId.Value;
                                xmlTbl.SchemeId = application.schemeId.Value;
                                xmlTbl.ServiceID = model.NICServiceId;
                                xmlTbl.Control_ID = model.NICControlId;
                                xmlTbl.ApplicationID = model.NICApplicationId;
                                xmlTbl.Unit_Id = model.NICUnitId;
                                xmlTbl.ProcessIndustryID = application.onlineapplicationId.ToString();
                                NewDataSet nds = new NewDataSet();
                                nds.Table = xmlTbl;

                                NICsingalwindowSystem NICdata = new NICsingalwindowSystem();
                                NICdata = MapNICSingalWindowSystemTable(nds);
                                NICdata.Fee_Amount = model.FormFeeWithGST;
                                NICdata.Status_Code = Convert.ToInt32(NICStatusCode.FEE_PENDING);
                                NICdata.Fee_Status = NICTextStatus.FEE_PENDING;
                                dbContext.NICsingalwindowSystems.Add(NICdata);
                                dbContext.SaveChanges();
                                model.FormStatusId = OSStatusId.Success;
                                nicmsg = 1;

                                model.NICApplicationId = Convert.ToString(application.onlineapplicationId);
                                model.NICProcessIndustryId = Convert.ToString(application.onlineapplicationId);
                            }

                            SaveImagesForOnlineSchemeApplication(model,userImage, signatureImage);//save applicant photo and signature

                            string message = string.Format(OSMessages.OnlineApplicationSubmitted, model.ApplicationFormId);
                            if (!string.IsNullOrEmpty(model.Email)) OnlineSchemeHelper.SendEmail(model.Email, "Scheme Form Submission", message);
                            if (!string.IsNullOrEmpty(model.MobileNumber)) OnlineSchemeHelper.SendSMS(model.MobileNumber, message);

                            if (nicmsg == 0)
                            {
                                string _loginMessage = string.Format(OSMessages.PIS_Registration_Activation, model.ApplicationFormId, _password);
                                if (!string.IsNullOrEmpty(model.MobileNumber)) OnlineSchemeHelper.SendSMS(model.MobileNumber, _loginMessage);
                                if (!string.IsNullOrEmpty(model.Email)) OnlineSchemeHelper.SendEmail(model.Email, "OnlineForm", _loginMessage);
                            }
                            model.FormStatusId = OSStatusId.Success;
                            model.ReturnTypeId = OSReturnTypeId.Saved;
                        }
                        else
                        {
                            int flag = UpdateOpenEndSchemeFormDetail(model, userImage, signatureImage);
                            model.FormStatusId = OSStatusId.Success;
                            model.ReturnTypeId = OSReturnTypeId.Saved;
                        }
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        //return string.Empty;
                        scope.Dispose();
                    }
                }
            }
            return model;
        }

        private int UpdateOpenEndSchemeFormDetail(SchemeFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            var flag = OSReturnTypeId.None;
            using (var dbContext = new OSDbContext())
            {
                //OnlineApplicationDetail application = new OnlineApplicationDetail();
                var application = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == model.Id);
                if (application != null)
                {
                    if (model.ApplicantType == "Individual")
                    {
                        application.firstName = model.FirstName;
                        application.middleName = model.MiddleName;
                        application.lastName = model.LastName;
                        application.fatherHusbandName = model.FatherName;
                        application.motherName = model.MotherName;
                        application.gender = model.Gender;
                        application.religionId = model.ReligionId;
                        application.occupationId = model.OccupationId;
                        application.quotaId = OSConstant.General;
                        application.marritalStatus = model.MaritalStatus;
                        application.dateOfBirth = model.DOB;
                    }
                    else
                    {
                        application.CompanyName = model.Applicant;
                        application.signingAuthority = model.SigningAuthority;
                        application.registeredOffice = model.PermanentAddress;
                        application.CompanyType = model.CompanyTypeId;
                        application.gender = OSStringConstant.Company;
                        application.fatherHusbandName = model.ApplicantMaster;
                        application.quotaId = model.SigningAuthorityId;
                    }
                    application.CompanyType = model.CompanyTypeId;
                    application.schemeId = model.SchemeId;
                    application.departmentId = model.DepartmentId;
                    application.permanentAdd = model.PermanentAddress;
                    application.correspondanceAdd = model.CorrespondingAddress;
                    application.area = model.AreaRangeId;
                    application.mobileNumberP2 = model.MobileNumber;
                    application.phoneNumberP2 = model.PhoneNumber;
                    application.faxNumberP2 = model.FaxNumber;
                    application.email = model.Email;
                    application.annualIncome = model.AnnualIncome;
                    application.ApplicationFee = model.ApplicationFee;
                    application.FormCGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                    application.FormSGST = Math.Round((model.FormFeeGST.Value / 2), 2, MidpointRounding.ToEven);
                    application.ProcessingCharge = model.ProcessingCharge;
                    application.ProcessingCGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                    application.ProcessingSGST = Math.Round((model.ProcessingChargeGST.Value / 2), 2, MidpointRounding.ToEven);
                    application.EarnestMoney = model.EarnestMoney;
                    application.TotalAmount = model.TotalAmount;
                    application.pan = model.PanNumber;
                    application.AadharNumber = model.AadharNumber;
                    application.GSTNO = model.ApplicantGSTNumber;
                    application.ApplicationDate = DateTime.Now;

                    application.RefundBankId = model.RefundBankId;
                    application.RefundInfaverof = model.RefundInfaverof;
                    application.RefundaccountNo = model.RefundAccountNo;
                    application.BranchName = model.BranchName;
                    application.IFSCCode = model.IFSCCode;

                    application.PropertyTypeID = model.PropertyTypeId;
                    application.Sector = model.Sector;
                    application.PropertyNo = model.PropertyNo;
                    application.ExistingPropertyNo = model.ExistingProperty;
                    application.AllotmentDate = model.AllottmentDate;
                    application.DispatchDate = model.DispatchDate;

                    application.FormCategory = model.ApplicationFormType;
                    
                    application.createdBy = "Online";
                    application.createdDate = DateTime.Now;
                    application.isActive = true;
                    application.IsSubmited = false;

                    application.projectname = model.SchemeProposedFirm.ProposedProject;
                    application.projectcost = model.SchemeProposedFirm.TotalCost.ToString();
                    application.projecttimeempl = model.SchemeProposedFirm.ImplementationTime;
                    application.Online_offline = model.FormType == "Online" ? "Y" : "N";
                    
                    dbContext.SaveChanges();

                    SaveImagesForOnlineSchemeApplication(model, userImage, signatureImage);

                    flag = OSReturnTypeId.Updated;
                }
                else
                {
                    flag = OSReturnTypeId.NotExist;
                }
            }
            return flag;
        }

        public SchemeFormViewModel SaveDocumentsForOpenEndScheme(SchemeFormViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            SaveDocumentsForOnlineSchemeApplication(model, files);
            return model;
        }

        public SchemeFormViewModel GetOpenEndedSchemeFormDataById(SchemeFormViewModel form)
        {
            using (var dbContext = new OSDbContext())
            {
                //OnlineFormViewModel applicant = new OnlineFormViewModel();
                var detail = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == form.ApplicationFormId && c.isActive == true);
                if (detail != null)
                {
                    form.Id = detail.onlineapplicationId;
                    form.ApplicationFormId = detail.onlineapplicationId;
                    form.UserPassword = "Noida"; //user password for login existing application
                    form.SchemeId = detail.schemeId;
                    form.SchemeName = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId == form.SchemeId).schemeName;
                    form.SchemeType = dbContext.SchemeMsts.FirstOrDefault(x => x.schemeId == detail.schemeId).SchemeTypeMst.SchemeTypeDesc; //OnlineSchemeType.OpenEnded; //"Open End Scheme";
                    form.SchemeTypeId = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId == form.SchemeId).schemeTypeId;
                    form.DepartmentId = detail.departmentId;
                    form.Department = dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == detail.departmentId).departmentName;
                    form.Applicant = detail.gender == "Company" ? detail.CompanyName : detail.firstName + " " + (string.IsNullOrEmpty(detail.middleName) ? string.Empty : detail.middleName + " ") + detail.lastName;
                    form.ApplicantType = detail.gender == "Company" ? "Company" : "Individual";
                    form.SigningAuthority = detail.signingAuthority;
                    form.CompanyTypeId = detail.CompanyType;
                    form.CompanyType = dbContext.Common_Config.Where(c => c.Id == detail.CompanyType).Select(c => c.Name).FirstOrDefault();
                    form.FirstName = detail.firstName;
                    form.MiddleName = detail.middleName;
                    form.LastName = detail.lastName;
                    form.Gender = detail.gender;
                    form.MaritalStatus = detail.marritalStatus;
                    form.DOB = detail.dateOfBirth;
                    form.FatherName = (!string.IsNullOrEmpty(detail.fatherHusbandName)) ? detail.fatherHusbandName : "NA";
                    form.ApplicantMaster = (!string.IsNullOrEmpty(detail.fatherHusbandName)) ? detail.fatherHusbandName : "NA";
                    form.MotherName = detail.motherName;
                    form.OccupationId = detail.occupationId;
                    form.Occupation = detail.occupationId != null ? dbContext.OccupationMsts.Where(c => c.occupationId == detail.occupationId).Select(s => s.occupation).FirstOrDefault() : string.Empty;
                    form.ReligionId = detail.religionId;
                    //form.Religion = detail.religionId != null ? dbContext.ReligionMsts.Where(r => r.religionId == detail.religionId).Select(r => r.religion).FirstOrDefault() : null;
                    form.CategoryId = detail.quotaId;
                    form.CategoryName = dbContext.QuotaMsts.Where(c => c.quotaId == detail.quotaId).Select(q => q.quotaName).FirstOrDefault();
                    form.MobileNumber = detail.mobileNumberP2;
                    form.PhoneNumber = detail.phoneNumberP2;
                    form.FaxNumber = detail.faxNumberP2;
                    form.ApplicantGSTNumber = detail.GSTNO;
                    form.PermanentAddress = detail.permanentAdd;
                    form.CorrespondingAddress = detail.correspondanceAdd;
                    form.ApplicationFee = detail.ApplicationFee;
                    form.FormFeeCGST = detail.FormCGST;
                    form.FormFeeSGST = detail.FormSGST;
                    form.ProcessingCharge = detail.ProcessingCharge;
                    form.ProcessingCGST = detail.ProcessingCGST;
                    form.ProcessingSGST = detail.ProcessingSGST;
                    form.TotalAmount = detail.TotalAmount;
                    form.Email = detail.email;
                    form.AnnualIncome = detail.annualIncome;
                    form.EarnestMoney = detail.EarnestMoney;
                    form.PanNumber = detail.pan;
                    form.RefundBankId = detail.RefundBankId;
                    form.RefundBank = detail.RefundBankId != null ? dbContext.BankMsts.FirstOrDefault(b => b.bankId == detail.RefundBankId).bankName : string.Empty;
                    form.BranchName = detail.BranchName;
                    form.RefundAccountNo = detail.RefundaccountNo;
                    form.IFSCCode = detail.IFSCCode;
                    form.RefundInfaverof = detail.RefundInfaverof;
                    form.AadharNumber = detail.AadharNumber;
                    form.Sector = detail.Sector;
                    form.PropertyNo = detail.PropertyNo;
                    form.ApplicationFormType = string.IsNullOrEmpty(detail.FormCategory) ? string.Empty : detail.FormCategory.Trim();
                    form.ExpansionType = string.IsNullOrEmpty(detail.FormSubCategory) ? string.Empty : detail.FormSubCategory.Trim();
                    form.ExistingProperty = detail.ExistingPropertyNo;
                    form.AllottmentDate = detail.AllotmentDate;
                    form.LetterCode = detail.RentingLetterNo;
                    form.LetterDate = detail.RentingDate;
                    form.DispatchDate = detail.DispatchDate;
                    form.AreaRangeId = detail.area;
                    form.AreaRange = string.IsNullOrEmpty(detail.area) ? null : dbContext.FloorMsts.Where(f => f.floorId.ToString() == detail.area).Select(f => f.floorName).FirstOrDefault();
                    form.PropertyTypeId = detail.PropertyTypeID;
                    form.PropertyType = detail.PropertyTypeID != null ? dbContext.PropertyTypeMsts.FirstOrDefault(p => p.departmentId == detail.departmentId && p.propertyTypeId == detail.PropertyTypeID && p.IsActive == true).propertyTypeName : null;
                    form.ChecklistHtml = detail.Documentfilename;
                    form.FormType = detail.Online_offline == "Y" ? "Online" : "Offline";
                    //form.DirectorModel = new onlineDirectorViewModel
                    //{
                    //    DirectorType = (detail.gender == "Company" && detail.occupationId != null) ? dbContext.Common_Config.Where(m => m.Id == detail.occupationId).FirstOrDefault().Name : string.Empty
                    //};
                    //form.ProposedModel = new ProposedCompanyViewModel
                    //{
                    //    ProposedProject = detail.projectname,
                    //    ImplementationTime = detail.projecttimeempl,
                    //    TotalCost = detail.projectcost
                    //};
                    form.SchemeProposedFirm = new SchemeProposedFirmViewModel
                    {
                        DirectorType = (detail.gender == "Company" && detail.occupationId != null) ? dbContext.Common_Config.Where(m => m.Id == detail.occupationId).FirstOrDefault().Name : string.Empty,
                        ProposedProject = detail.projectname,
                        ImplementationTime = detail.projecttimeempl,
                        TotalCost = detail.projectcost
                    };
                    form.PreviousFormNo = detail.PreviousFormNo != null ? detail.PreviousFormNo : string.Empty;
                    //form.AppType = dbContext.NICsingalwindowSystems.Where(x => x.onlineapplicationId == form.ApplicationFormId).FirstOrDefault() != null ? Constants.AppType : null;

                    if (form.NICControlId != null && form.NICUnitId != null && form.NICServiceId != null)
                    {
                        var nic = dbContext.NICsingalwindowSystems.FirstOrDefault(n => n.onlineapplicationId == form.ApplicationFormId && n.Control_ID == form.NICControlId && n.Unit_Id == form.NICUnitId && n.ServiceID == form.NICServiceId);
                        if (nic != null)
                        {
                            //form.NICSingleWindowModel = nic;
                            form.NICSchemeForm = GetNICSchemeFormDetail(form);
                            form.AppType = OSStringConstant.NIC;
                            form.IsFromNIC = true;
                        }
                        else
                        {
                            form.AppType = OSStringConstant.Authority;
                        }
                    }
                    else
                    {
                        form.IsFromNIC = true;
                        form.AppType = OSStringConstant.Authority;
                    }

                    if (form.Gender.ToLower() == OSStringConstant.Company.ToLower())
                    {
                        form.SigningAuthorityId = form.CategoryId;
                        if (form.CategoryId != null && form.CategoryId > 0)
                        {
                            form.SignatoryStatus = dbContext.Common_Config.FirstOrDefault(x => x.Id == form.CategoryId).Name;
                        }
                    }

                    form.FormFeeGST = form.FormFeeCGST + form.FormFeeSGST;
                    form.FormFeeWithGST = form.ApplicationFee + form.FormFeeGST;
                    form.ProcessingChargeGST = form.ProcessingSGST + form.ProcessingCGST;
                    form.ProcessingChargeWithGST = form.ProcessingCharge + form.ProcessingChargeGST;

                    //var doctype = applicant.ChecklistHtml.Split(',');
                    if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.ApplicationFormId + "/Pictures/")))
                    {
                        var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.ApplicationFormId + "/Pictures/");
                        string[] files = Directory.GetFiles(filepath);
                        if (files != null)
                        {
                            if (files.Count() > 0)
                            {
                                if (files.Count() == 2)
                                {
                                    if (files[0] != null)
                                    {
                                        var uplod = files[0].Split('\\');
                                        form.SignImage = "/UploadFiles/" + form.ApplicationFormId + "/Pictures/" + uplod[uplod.Length - 1];
                                    }
                                    if (files[1] != null)
                                    {
                                        var uplod = files[1].Split('\\');
                                        form.UserImage = "/UploadFiles/" + form.ApplicationFormId + "/Pictures/" + uplod[uplod.Length - 1];
                                    }
                                }
                            }
                        }
                    }
                    form.IsApplicationFeePaid = false;
                    var payment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(p => p.ServiceRefId == detail.onlineapplicationId && p.status == 1 && p.ServiceType == 1);
                    if (payment != null)
                    {
                        form.IsApplicationFeePaid = true;
                        form.PaymentMode = "Online";
                        form.SchemePayment = GetOnlineSchemePaymentDetailById(form.ApplicationFormId, payment.AutoID);
                    }
                    else
                    {
                        var Offpayment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(p => p.ServiceRefId == detail.onlineapplicationId && p.ServiceType == 3);
                        if (Offpayment != null)
                        {
                            form.PaymentMode = "Offline";
                            form.IsChallanGenerated = true;
                            if (!string.IsNullOrEmpty(Offpayment.TrKey))
                            {
                                form.PaymentMode = "Offline_Updated";
                                if (Offpayment.status == 1 || Offpayment.TranStatus == 1)
                                {
                                    form.IsApplicationFeePaid = true;
                                    form.IsChallanGenerated = false;
                                    form.PaymentMode = "Offline_Validated";
                                }
                            }
                            form.SchemePayment = GetOnlineSchemePaymentDetailById(form.ApplicationFormId, Offpayment.AutoID);
                        }
                    }

                    form.IsPaidThroughSWP = false;
                    if (form.AppType == OSStringConstant.NIC)
                    {
                        //check single window portal
                        var singleWindowPortal = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(p => p.ServiceRefId == detail.onlineapplicationId && p.ServiceType == OSConstant.NICSchemeFormPayment);
                        if (singleWindowPortal != null)
                        {
                            form.IsPaidThroughSWP = true;
                            if (singleWindowPortal.TranStatus == 1 && singleWindowPortal.status == 1)
                            {
                                form.IsApplicationFeePaid = true;
                                form.ServiceAppPayStatus = "Paid";
                            }
                            form.SchemePayment = GetOnlineSchemePaymentDetailById(form.ApplicationFormId, singleWindowPortal.AutoID);
                        }
                    }

                    var flag = IsDocumentForSchemeFormUploaded(form.ApplicationFormId); //IsDocumentUploaded(form.ApplicationFormId);
                    if (flag == true)
                    {
                        form.IsDocumentUploaded = true;
                        //form.DocumentsTable = dbContext.Sp_NewSchemereturn(form.ApplicationFormId, form.SchemeId).FirstOrDefault();
                        form.DocumentsTable = dbContext.ssp_SchemeFormUploadedDocument(form.ApplicationFormId, form.SchemeId).FirstOrDefault();
                    }
                    //else applicant.IsDocumentUploaded = false;

                    if (form.SchemeType == SchemeConstant.Transport || form.SchemeType == SchemeConstant.OpenEnded)
                    {
                        var PreChallan = IsPreviousChallanUploadedForSchemeForm(form.ApplicationFormId); //IsPreviousChallanUploaded(form.ApplicationFormId);
                        if (PreChallan == true)
                        {
                            form.IsPreviousChallanUploaded = true;
                        }
                    }
                }
                else
                {
                    form.FlagId = OSReturnTypeId.NotExist;
                }
                return form;
            }
        }

        private NICSchemeFormViewModel GetNICSchemeFormDetail(SchemeFormViewModel form)
        {
            using (var dbContext = new OSDbContext())
            {
                NICSchemeFormViewModel nicSchemeForm = new NICSchemeFormViewModel();
                var nic = dbContext.NICsingalwindowSystems.FirstOrDefault(n => n.onlineapplicationId == form.ApplicationFormId && n.Control_ID == form.NICControlId && n.Unit_Id == form.NICUnitId && n.ServiceID == form.NICServiceId);
                if (nic != null)
                {
                    nicSchemeForm.ApplicationId = nic.onlineapplicationId;
                    nicSchemeForm.SchemeId = form.SchemeId;
                    nicSchemeForm.DepartmentId = form.DepartmentId;
                    nicSchemeForm.ControlId = nic.Control_ID;
                    nicSchemeForm.UnitId = nic.Unit_Id;
                    nicSchemeForm.ServiceId = nic.ServiceID;
                    nicSchemeForm.ProcessIndustryId = nic.ProcessIndustryID;
                    nicSchemeForm.CompanyName = nic.Company_Name;
                    nicSchemeForm.IndustryDistrict = nic.Industry_District;
                    nicSchemeForm.IndustryDistrictId = nic.Industry_District_Id;
                    nicSchemeForm.Industry_Address = nic.Industry_Address;
                    nicSchemeForm.PinCode = nic.Pin_Code;
                    nicSchemeForm.OccupierName = nic.Occupier_Name;
                    nicSchemeForm.OccupierEmailId = nic.Occupier_Email_ID;
                    nicSchemeForm.OccupierMobileNo = nic.Occupier_Mobile_No;
                    nicSchemeForm.OccupierDOB = nic.Occupier_DOB;
                    nicSchemeForm.OccupierGender = nic.Occupier_Gender;
                    nicSchemeForm.OccupierAddress = nic.Occupier_Address;
                    nicSchemeForm.OccupierDistrictId = nic.Occupier_District_ID;
                    nicSchemeForm.OccupierDistrictName = nic.Occupier_District_Name;
                    nicSchemeForm.OccupierPinCode = nic.Occupier_Pin_Code;
                    nicSchemeForm.NatureOfActivity = nic.Nature_of_Activity;
                    nicSchemeForm.InstalledCapacity = nic.Installed_Capacity;
                    nicSchemeForm.Employees = nic.Employees;
                    nicSchemeForm.NatureOfOperation = nic.Nature_of_Operation;
                    nicSchemeForm.ProjectCost = nic.publicdecimalProject_Cost;
                    nicSchemeForm.OrganizationTypeId = nic.Organization_Type_ID;
                    nicSchemeForm.OrganizationType = nic.Organization_Type;
                    nicSchemeForm.IndustryTypeId = nic.Industry_Type_ID;
                    nicSchemeForm.IndustryTypeName = nic.Industry_Type_Name;
                    nicSchemeForm.ExpectedConstructionDate = nic.Expected_date_construction;
                    nicSchemeForm.ProjectStatus = nic.Project_Status;
                    nicSchemeForm.IndustryColor = nic.Industry_Color;
                    nicSchemeForm.ExpectedProductionDate = nic.Expected_date_production;
                    nicSchemeForm.UnitCategory = nic.Unit_Category;
                    nicSchemeForm.ItemsManufactured = nic.Items_Manufactured;
                    nicSchemeForm.AnnualTurnover = nic.Annual_Turnover;
                    nicSchemeForm.FeeAmount = nic.Fee_Amount;
                    nicSchemeForm.FeeStatus = nic.Fee_Status;
                    nicSchemeForm.StatusCode = nic.Status_Code;
                    nicSchemeForm.PaymentThrough = nic.Payment_Through;
                    nicSchemeForm.PaymentDescription = nic.Payment_Description;
                }
                return nicSchemeForm;
            }
        }

        public SchemeFormViewModel UpdateOpenEndSchemeFormPaymentStatus(SchemeFormViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                if (model.NICApplicationId != null && model.NICControlId != null && model.NICUnitId != null && model.NICServiceId != null)
                {
                    var exService = dbContext.NICsingalwindowSystems.FirstOrDefault(c => c.onlineapplicationId.ToString() == model.NICApplicationId && c.Control_ID == model.NICControlId && c.Unit_Id == model.NICUnitId);
                    if (exService != null)
                    {
                        exService.Status_Code = Convert.ToInt32(NICStatusCode.FEE_PAID);
                        exService.Fee_Status = NICTextStatus.FEE_PAID;
                        dbContext.SaveChanges();
                        model.NICFeeStatus = NICTextStatus.FEE_PAID;
                        model.NICFeeStatusId = NICStatusCode.FEE_PAID;
                    }
                }
                else
                {
                    var exPayment = dbContext.OnlineApplicationDetails_trans.Where(c => c.ServiceRefId == model.ApplicationFormId).OrderByDescending(o => o.AutoID).FirstOrDefault();
                    model.IsApplicationFeePaid = (exPayment != null && exPayment.status == 1) ? true : false;
                }
                return model;
            }
        }

        public SchemeFormViewModel SaveProjectAndRefundDetailForOpenEndScheme(SchemeFormViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(f => f.onlineapplicationId == model.ApplicationFormId);
                if (form != null)
                {
                    form.RefundBankId = model.RefundBankId;
                    form.BranchName = model.BranchName;
                    form.RefundInfaverof = model.RefundInfaverof;
                    form.RefundaccountNo = model.RefundAccountNo;
                    form.IFSCCode = model.IFSCCode;
                    form.projectname = model.SchemeProposedFirm.ProposedProject;
                    form.projectcost = model.SchemeProposedFirm.TotalCost.ToString();
                    form.projecttimeempl = model.SchemeProposedFirm.ImplementationTime;
                    dbContext.SaveChanges();
                           
                    model.IsDirectorDetailsSaved = true;
                }
                else
                {
                    model.IsDirectorDetailsSaved = false;
                }
                return model;
            }
        }

        public SchemeProposedFirmViewModel SaveProposedFirmDetailForOpenEndScheme(SchemeProposedFirmViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                if (model.ActionType == "Delete")
                {
                    var director = dbContext.online_Director_Master.FirstOrDefault(d => d.Director_Id == model.DirectorId);
                    director.Is_Active = 0;
                    dbContext.SaveChanges();
                    model.ActionTypeId = OSReturnTypeId.Removed;
                }
                else
                {
                    online_Director_Master master = new online_Director_Master();
                    master.onlineapplicationId = model.ApplicationFormId;
                    master.Director_Name = model.DirectorName;
                    master.Director_Share = model.DirectorShare;
                    master.Type = model.DirectorTypeId;
                    master.pan_no = model.PAN;
                    master.Is_Active = 1;
                    master.Created_By = model.ApplicationFormId;
                    dbContext.online_Director_Master.Add(master);
                    dbContext.SaveChanges();
                    model.DirectorStatusId = 1;
                    model.ActionTypeId = OSReturnTypeId.Saved;
                }

                return model;
            }
        }

        public DataSourceResult GetProposedFirmDetailsForOpenEndScheme(DataSourceRequest request, SchemeProposedFirmViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var directorlist = dbContext.online_Director_Master.Where(d => d.onlineapplicationId == model.ApplicationFormId).ToList();
                if (directorlist != null && directorlist.Count > 0)
                {
                    var Dirlist = (from directors in dbContext.online_Director_Master
                                   where directors.onlineapplicationId == model.ApplicationFormId && directors.Is_Active == 1
                                   select new SchemeProposedFirmViewModel
                                   {
                                       Id = directors.Director_Id,
                                       DirectorId = directors.Director_Id,
                                       ApplicationFormId = directors.onlineapplicationId,
                                       DirectorName = directors.Director_Name,
                                       DirectorShare = directors.Director_Share,
                                       PAN = directors.pan_no,
                                       DirectorTypeId = directors.Type,
                                       DirectorStatusId = directors.Is_Active,
                                       IsActive = directors.Is_Active == 1 ? true : false,
                                       DirectorType = dbContext.Common_Config.FirstOrDefault(d => d.Id == directors.Type && d.Category.ToLower() == "director" && d.Is_Active == 1).Name
                                   }).ToList();
                    int count = 1;
                    Dirlist.ForEach(c => c.Id = count++);

                    return Dirlist.ToDataSourceResult(request);
                }
                else
                {
                    return null;
                }
            }
        }


        public SchemeFormViewModel GetOnlineSchemeBasicDetail(SchemeFormViewModel modal)
        {
            using (var dbContext = new OSDbContext())
            {
                var data = (from scheme in dbContext.SchemeMsts
                            where scheme.schemeId == modal.SchemeId && scheme.IsActive == true
                            select new SchemeFormViewModel
                            {
                                SchemeId = scheme.schemeId,
                                SchemeName = scheme.schemeName,
                                SchemeStartDate = scheme.startDate,
                                SchemeEndDate = scheme.endDate,
                                DepartmentId = modal.DepartmentId,
                                Department = modal.DepartmentId != null ? dbContext.DepartmentMsts.FirstOrDefault(d=>d.departmentId==modal.DepartmentId).departmentName : string.Empty,
                                ApplicationFee = scheme.FormFee,
                                FormFeeCGST = scheme.FormCGST == null ? 0 : scheme.FormCGST,
                                FormFeeSGST = scheme.FormSGST == null ? 0 : scheme.FormSGST,
                                ProcessingCharge = scheme.ProcessingFee == null ? 0 : scheme.ProcessingFee,
                                ProcessingCGST = scheme.ProcessingCGST == null ? 0 : scheme.ProcessingCGST,
                                ProcessingSGST = scheme.ProcessingSGST == null ? 0 : scheme.ProcessingSGST
                            }).FirstOrDefault();
                data.FormFeeGST = data.FormFeeCGST + data.FormFeeSGST;
                data.ProcessingChargeGST = data.ProcessingCGST + data.ProcessingSGST;
                return data;
            }
        }

        public SchemeFormViewModel ValidateOnlineSchemeFormByType(SchemeFormViewModel modal)
        {
            using (var dbContext = new OSDbContext())
            {
                if (!string.IsNullOrEmpty(modal.ActionType) && modal.ActionType.ToLower() == "schemelogin")
                {
                    var _password = modal.UserPassword.MD5HashPassword();
                    var form = dbContext.OnlineApplicationDetails.FirstOrDefault(a=>a.onlineapplicationId == modal.ApplicationFormId && a.Userpassword == _password);
                    if (form != null) modal.ReturnTypeId = OSReturnTypeId.Success;
                    else modal.ReturnTypeId = OSReturnTypeId.Failed;
                }
                if (!string.IsNullOrEmpty(modal.ActionType) && modal.ActionType.ToLower() == "forgotpassword")
                {
                    var form = dbContext.OnlineApplicationDetails.FirstOrDefault(a => a.onlineapplicationId == modal.ApplicationFormId && a.departmentId == modal.DepartmentId);
                    if (form != null)
                    {
                        string _password = OnlineSchemeHelper.GenerateSchemeFormPassword();
                        _password = _password.MD5HashPassword();
                        form.Userpassword = _password;
                        dbContext.SaveChanges();

                        string _loginMessage = string.Format(OSMessages.PIS_Registration_Activation, modal.ApplicationFormId, _password);
                        if (!string.IsNullOrEmpty(form.mobileNumberP2)) OnlineSchemeHelper.SendSMS(form.mobileNumberP2, _loginMessage);
                        if (!string.IsNullOrEmpty(form.email)) OnlineSchemeHelper.SendEmail(form.email, "OnlineForm", _loginMessage);

                        modal.ReturnTypeId = OSReturnTypeId.Success;
                    }
                }
                return modal;
            }
        }

        public SchemeFormViewModel ActionForOnlineSchemeApplicationForm(SchemeFormViewModel modal)
        {
            throw new NotImplementedException();
        }


        public SchemePaymentViewModel SaveOnlineSchemePaymentTransaction(System.Web.Mvc.FormCollection form)
        {
            using (var dbContext = new OSDbContext())
            {
                var TransactionModel = new SchemePaymentViewModel();
                if (HttpContext.Current.Session["OnlineBankId"] != null)
                {
                    var bankId = (int)HttpContext.Current.Session["OnlineBankId"];
                    ////var bob = UpdateBankOfBarodaOnlinePayment(form);
                    //HttpContext.Current.Session["OnlineBankId"] = null;
                    //return bob;
                    return TransactionModel;
                }
                else
                {
                    //string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";
                    string HashSequence = ConfigurationManager.AppSettings["hashSequence"];
                    //var ServiceDetailModel = new ServiceRequestViewModel();

                    if (form != null && form["status"].ToString() == "success")
                    {
                        var id = form["udf1"]; // TransactionId
                        var rid = form["udf2"]; // RegistrationId
                        var requestId = form["udf3"]; // OnlineRequestId
                        var challanId = form["udf4"]; // ChallanId
                        var serviceRequestId = !string.IsNullOrEmpty(requestId) ? Convert.ToInt32(requestId) : 0;//requestId
                        var paidAmount = Convert.ToDecimal(form["amount"]);

                        //var serviceRequest = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == serviceRequestId);
                        //var processingCharge = dbContext.CitizenService_Master.FirstOrDefault(c => c.Deptt_Id == serviceRequest.DepartmentId && c.service_id == serviceRequest.ServiceId && c.Status == 1).Amount;

                        string[] paramArr = HashSequence.Split('|');
                        Array.Reverse(paramArr);
                        string paramSequence = ConfigurationManager.AppSettings["HDFC_SALT"] + "|" + form["status"].ToString();
                        foreach (string param in paramArr)
                        {
                            paramSequence += "|";
                            paramSequence = paramSequence + (form[param] != null ? form[param] : "");
                        }
                        
                        string hashSequence = OSPaymentGateway.GenerateSHA512HashCode(paramSequence).ToLower();
                        if (hashSequence == form["hash"])
                        {
                            var transaction = dbContext.OnlineApplicationDetails_trans.Where(m => m.txnid == id).FirstOrDefault();
                            if (transaction != null)
                            {
                                //transaction.TranStatus = (form["status"].ToString() == "success" && !string.IsNullOrEmpty(form["bank_ref_num"])) ? NAStatusId.Success : NAStatusId.Failed;
                                transaction.TranStatus = form["status"].ToString() == "success" ? OSStatusId.Success : OSStatusId.Failed;
                                transaction.status = form["status"].ToString() == "success" ? 1 : 0;
                                transaction.error = form["error"];
                                transaction.error_Message = form["error_Message"];
                                transaction.TrKey = form["txnid"].ToString();
                                transaction.mode = form["mode"].ToString();
                                transaction.mihpayid = form["mihpayid"];
                                transaction.productinfo = form["productinfo"];
                                transaction.EntryDate = DateTime.Now;
                                transaction.payment_source = form["payment_source"];
                                transaction.PG_Type = form["PG_Type"];
                                transaction.bank_ref_num = form["bank_ref_num"];
                                transaction.bankcode = form["bankcode"];
                                transaction.name_on_card = form["name_on_card"];
                                transaction.cardnum = form["cardnum"];
                                //transaction.cardhash = form["cardhash"];
                                transaction.issuing_bank = form["issuing_bank"];
                                transaction.card_type = form["card_type"];
                                transaction.udf1 = form["udf1"];
                                transaction.udf2 = form["udf2"];
                                transaction.udf3 = form["udf3"];
                                transaction.udf4 = form["udf4"];
                                transaction.ChallanId = form["udf4"];
                                dbContext.SaveChanges();

                                TransactionModel = GetOnlineSchemePaymentTransactionDetailById(new SchemePaymentViewModel { TransactionId = id });
                                
                                if (form["status"].ToString() == "success")
                                {
                                    var _mobileNo = form["phone"];
                                    var _email = form["email"];
                                   
                                    TransactionModel.ReturnTypeId = OSReturnTypeId.Success; //success
                                    TransactionModel.Status = form["status"].ToString();
                                    string message = string.Format(OSMessages.OnlineProcessingFee, form["productinfo"], transaction.ServiceRefId, transaction.Amount);
                                    if (!string.IsNullOrEmpty(_mobileNo)) OnlineSchemeHelper.SendSMS(_mobileNo, message);
                                    if (!string.IsNullOrEmpty(_email)) OnlineSchemeHelper.SendEmail(_email, "Online Payment", message);
                                }
                                else
                                {
                                    TransactionModel.ReturnTypeId = OSReturnTypeId.Failed;
                                    TransactionModel.Status = form["status"].ToString();
                                }

                            }
                            else
                            {
                                TransactionModel.ReturnTypeId = OSReturnTypeId.Mismatch; //mismatch
                            }
                        }
                        else
                        {
                            TransactionModel.ReturnTypeId = OSReturnTypeId.Mismatch; //mismatch
                        }
                    }
                    else
                    {
                        TransactionModel.ReturnTypeId = OSReturnTypeId.Failed; // failed
                    }
                }

                //ServiceDetailModel.OnlinePaymentModel = TransactionModel;
                return TransactionModel;
            }
        }


        public DataSourceResult GetChecklistDocumentsAsDataSource(DataSourceRequest request, SchemeFormViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                List<SchemeDocumentViewModel> docs = new List<SchemeDocumentViewModel>();
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(f => f.onlineapplicationId == model.ApplicationFormId);
                //var documents = dbContext.OnlineCheckListMasters.ToList();
                if (form != null)
                {
                    docs = (from doc in dbContext.OnlineCheckListMasters
                            where doc.IsActive == true && doc.DepartmentId == model.DepartmentId && doc.SchemeTypeId == model.SchemeTypeId
                            && (model.ChecklistType == null || doc.CheckListType == model.ChecklistType)
                            select new SchemeDocumentViewModel
                            {
                                Id = doc.CheckListId,
                                DocumentId = doc.CheckListId,
                                DocumentName = doc.CheckListName,
                                ParentDocumentType = doc.MainList
                            }).ToList();
                    if (form.ScrutinyStatusId == 2)
                    {
                        var scrutinydocs = (from doc in dbContext.OnlineCheckListMasters
                                            where doc.IsActive == true && doc.CheckListType == OSStringConstant.SCRUTINY //&& doc.SchemeId == form.schemeId // configSchemeId
                                            select new SchemeDocumentViewModel
                                            {
                                                Id = doc.CheckListId,
                                                DocumentId = doc.CheckListId,
                                                DocumentName = doc.CheckListName,
                                                ParentDocumentType = doc.MainList
                                            }).ToList();
                        docs.AddRange(scrutinydocs);
                    }
                    if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.ApplicationFormId + "/Documents/")))
                    {
                        var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.onlineapplicationId + "/Documents/");
                        string[] files = Directory.GetFiles(filepath);
                        if (files != null)
                        {

                            for (int x = 0; x < files.Length; x++)
                            {
                                var ufile = files[x].Split('\\');
                                var filename = ufile[ufile.Length - 1];
                                int start = (filename.LastIndexOf('-') + 1);
                                int end = filename.LastIndexOf('.');
                                int dif = end - start;
                                int nod = Convert.ToInt32(filename.Substring(start, dif));
                                docs.Where(m => m.Id == nod).FirstOrDefault().PathName = "/UploadFiles/" + model.ApplicationFormId + "/Documents/" + filename;
                                docs.Where(m => m.Id == nod).FirstOrDefault().UploadedDocument = filename;
                            }
                        }
                    }
                   // docs = docs.Where(m => !string.IsNullOrEmpty(m.PathName)).ToList();
                    int counter = 1;
                    docs.ForEach(x => x.Sno = counter++);
                    return docs.ToDataSourceResult(request);
                }
                return null;
            }
        }

        public int RemoveDocumentFromApplicationForm(string formNo, string filename)
        {
            throw new NotImplementedException();
        }

        public DataSourceResult GetProposedFirmDirectorsDetailAsDataSource(DataSourceRequest request, SchemeFormViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var directorList = (from directors in dbContext.online_Director_Master
                                    where directors.onlineapplicationId == model.ApplicationFormId && directors.Is_Active == 1
                                    select new SchemeProposedFirmViewModel
                                    {
                                        Id = directors.Director_Id,
                                        DirectorId = directors.Director_Id,
                                        ApplicationFormId = directors.onlineapplicationId,
                                        DirectorName = directors.Director_Name,
                                        DirectorShare = directors.Director_Share,
                                        PAN = directors.pan_no,
                                        DirectorTypeId = directors.Type,
                                        DirectorType = dbContext.Common_Config.FirstOrDefault(d => d.Id == directors.Type && d.Category.ToLower() == OSConfigCategory.Director.ToLower() && d.Is_Active == 1).Name
                                    }).ToList();
                int count = 1;
                foreach (var director in directorList)
                {
                    director.Id = count;
                    count++;
                }
                return directorList.ToDataSourceResult(request);
            }
        }

        public SchemeProposedFirmViewModel SaveProposedFirmDirectorsDetail(SchemeProposedFirmViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                if (model.ActionType.ToLower() == "saveproject")
                {
                    var form = dbContext.OnlineApplicationDetails.FirstOrDefault(o => o.onlineapplicationId == model.ApplicationFormId);
                    if (form != null)
                    {
                        form.projectname = model.ProposedProject;
                        form.projectcost = model.TotalCost;
                        form.projecttimeempl = model.ImplementationTime;
                        dbContext.SaveChanges();
                        model.ReturnTypeId = OSReturnTypeId.Saved;
                    }
                }
                if (model.ActionType.ToLower() == "save")
                {
                    online_Director_Master director = new online_Director_Master();
                    director.onlineapplicationId = model.ApplicationFormId;
                    director.Director_Name = model.DirectorName;
                    director.Director_Share = model.DirectorShare;
                    director.Type = model.DirectorTypeId;
                    director.pan_no = model.PAN;
                    director.Is_Active = 1;
                    director.Created_By = 0;

                    dbContext.online_Director_Master.Add(director);
                    dbContext.SaveChanges();
                    model.ReturnTypeId = OSReturnTypeId.Saved;
                }
                if (model.ActionType.ToLower() == "remove")
                {
                    var director = dbContext.online_Director_Master.FirstOrDefault(x => x.onlineapplicationId == model.ApplicationFormId && x.Director_Id == model.DirectorId);
                    if (director != null)
                    {
                        dbContext.online_Director_Master.Remove(director);
                        dbContext.SaveChanges();
                        model.ReturnTypeId = OSReturnTypeId.Removed;
                    }
                }
                return model;
            }
        }

        public SchemeFormViewModel MigrateOnlineSchemeFormDataForAllotment(SchemeFormViewModel form)
        {
            throw new NotImplementedException();
        }

        public DataSourceResult GetApplicationFormIdListAsDataSource(DataSourceRequest request, SchemeFormViewModel model)
        {
            throw new NotImplementedException();
        }

        public SchemeFormViewModel ValidatePropertyForAllotment(SchemeFormViewModel model)
        {
            throw new NotImplementedException();
        }

        public SchemeFormViewModel SaveSchemeFormProcess(SchemeFormViewModel model)
        {
            throw new NotImplementedException();
        }

        public DataSourceResult GetOnlineApplicationProcessAsDataSource(DataSourceRequest request, SchemeFormViewModel model)
        {
            throw new NotImplementedException();
        }

        public SchemeFormViewModel SaveOnlineApplicationProcessStatus(SchemeFormViewModel model)
        {
            throw new NotImplementedException();
        }

        private SchemeChallanViewModel SaveSchemeFormPaymentChallanTransaction(SchemeFormViewModel form)
        {
            using (var dbContext = new OSDbContext())
            {
                var onlineForm = dbContext.OnlineApplicationDetails.Where(m => m.onlineapplicationId == form.ApplicationFormId && m.isActive == true).FirstOrDefault();
                if (onlineForm != null)
                {
                    var transaction = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(f => f.ServiceRefId == form.ApplicationFormId && f.ServiceType != 1);
                    if (transaction == null)
                    {
                        OnlineApplicationDetails_trans trans = new OnlineApplicationDetails_trans
                        {
                            ServiceRefId = form.ApplicationFormId,
                            txnid = form.ApplicationFormId.ToString() + "T01",
                            Amount = form.TotalAmount,
                            productinfo = "RTGS/NEFT",
                            mode = "RTGS/NEFT",
                            ServiceType = OSConstant.ReservationMoneyChallanPayment,
                            GetwayName = "67", //form.ChallanBankId.ToString(),
                            udf5 = form.BankIFSCCode,
                            virtualaccountno = form.VirtualAccPrefix + form.ApplicationFormId.ToString() + "T01",
                            status = 0,
                            EntryDate = DateTime.Now
                        };
                        dbContext.OnlineApplicationDetails_trans.Add(trans);
                        dbContext.SaveChanges();
                    }
                }

                var model = (from appl in dbContext.OnlineApplicationDetails
                             join trans in dbContext.OnlineApplicationDetails_trans on appl.onlineapplicationId equals trans.ServiceRefId
                             where appl.onlineapplicationId == form.ApplicationFormId && trans.ServiceType == 3
                             select new SchemeChallanViewModel
                             {
                                 ApplicationFormId = appl.onlineapplicationId,
                                 Applicant = appl.gender != OSStringConstant.Company ? appl.firstName + " " + (!string.IsNullOrEmpty(appl.middleName) ? appl.middleName + " " + appl.lastName : appl.lastName) : appl.CompanyName,
                                 Email = appl.email,
                                 MobileNo = appl.mobileNumberP2,
                                 CorrespondAddress = appl.correspondanceAdd,
                                 SchemeFormFee = appl.ApplicationFee,
                                 ProcessingFee = appl.ProcessingCharge,
                                 FormFeeWithGST = appl.ApplicationFee + appl.FormCGST + appl.FormSGST,
                                 ProcessingFeeWithGST = appl.ProcessingCharge + appl.ProcessingSGST + appl.ProcessingCGST,
                                 FormFeeSGST = appl.FormSGST,
                                 FormFeeCGST = appl.FormCGST,
                                 ProcessingSGST = appl.ProcessingSGST,
                                 ProcessingCGST = appl.ProcessingCGST,
                                 TotalAmount = appl.ApplicationFee + appl.ProcessingCharge + appl.EarnestMoney,
                                 EarnestMoney = appl.EarnestMoney,
                                 TotalAmountWithGST = (appl.ProcessingCharge + appl.ProcessingSGST + appl.ProcessingCGST) + (appl.ApplicationFee + appl.FormCGST + appl.FormSGST) + appl.EarnestMoney,
                                 BankName = dbContext.BankMsts.Where(m => m.bankId.ToString() == trans.GetwayName && m.IsActive == true).FirstOrDefault().bankName + " Branch : " + dbContext.SchemeBankTrans.Where(m => m.bankId.ToString() == trans.GetwayName && m.schemeId == appl.schemeId && m.IsActive == true).FirstOrDefault().BranchMst.branchName,
                                 AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == appl.area && m.IsActive == true).FirstOrDefault().floorName,
                                 SchemeName = dbContext.SchemeMsts.Where(m => m.schemeId == appl.schemeId && m.IsActive == true && m.completed == true && m.Status != OSConstant.SchemeClosed).FirstOrDefault().schemeName,
                                 Department = dbContext.DepartmentMsts.Where(m => m.departmentId == appl.departmentId && m.IsActive == true).FirstOrDefault().departmentName,
                                 //In offline case BankId place in gateway name , Branch Id Place in Bank code and Account Place in Card Num Field in OnlineApplicationDetails_trans table 23 aug 2017 According to vishal Shukla Sir
                                 AccountNo = trans.cardnum,
                                 BranchName = dbContext.BankMsts.Where(m => m.bankId.ToString() == trans.GetwayName && m.IsActive == true).FirstOrDefault().bankName + " " + dbContext.BranchMsts.Where(m => m.branchId.ToString() == trans.bankcode && m.IsActive == true).FirstOrDefault().branchName,
                                 //In Case of applicant GST No saved in fax no 30 Aug 2017
                                 GSTNO = appl.GSTNO,
                                 IFSCCode = trans.udf5,
                                 AccountPrefix = trans.virtualaccountno,
                                 FormModel = new SchemeFormViewModel
                                 {
                                     ApplicationFormId = appl.onlineapplicationId,
                                     Applicant = appl.gender != OSStringConstant.Company ? appl.firstName + " " + (!string.IsNullOrEmpty(appl.middleName) ? appl.middleName + " " + appl.lastName : appl.lastName) : appl.CompanyName,
                                     Email = appl.email,
                                     MobileNumber = appl.mobileNumberP2,
                                     CorrespondingAddress = appl.correspondanceAdd,
                                     ApplicationFee = appl.ApplicationFee,
                                     ProcessingCharge = appl.ProcessingCharge,
                                     FormFeeGST = appl.ApplicationFee + appl.FormCGST + appl.FormSGST,
                                     ProcessingChargeGST = appl.ProcessingCharge + appl.ProcessingSGST + appl.ProcessingCGST,
                                     FormFeeSGST = appl.FormSGST,
                                     FormFeeCGST = appl.FormCGST,
                                     ProcessingSGST = appl.ProcessingSGST,
                                     ProcessingCGST = appl.ProcessingCGST,
                                     TotalAmount = appl.ApplicationFee + appl.ProcessingCharge + appl.EarnestMoney,
                                     EarnestMoney = appl.EarnestMoney,
                                     TotalAmountGST = (appl.ProcessingCharge + appl.ProcessingSGST + appl.ProcessingCGST) + (appl.ApplicationFee + appl.FormCGST + appl.FormSGST) + appl.EarnestMoney,
                                     ChallanBank = dbContext.BankMsts.Where(m => m.bankId.ToString() == trans.GetwayName && m.IsActive == true).FirstOrDefault().bankName + " Branch : " + dbContext.SchemeBankTrans.Where(m => m.bankId.ToString() == trans.GetwayName && m.schemeId == appl.schemeId && m.IsActive == true).FirstOrDefault().BranchMst.branchName,
                                     AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == appl.area && m.IsActive == true).FirstOrDefault().floorName,
                                     SchemeName = dbContext.SchemeMsts.Where(m => m.schemeId == appl.schemeId && m.IsActive == true && m.completed == true && m.Status != OSConstant.SchemeClosed).FirstOrDefault().schemeName,
                                     Department = dbContext.DepartmentMsts.Where(m => m.departmentId == appl.departmentId && m.IsActive == true).FirstOrDefault().departmentName,
                                     //In offline case BankId place in gateway name , Branch Id Place in Bank code and Account Place in Card Num Field in OnlineApplicationDetails_trans table 23 aug 2017 According to vishal Shukla Sir
                                     ChallanAccountNo = trans.cardnum,
                                     ChallanBranchName = dbContext.BankMsts.Where(m => m.bankId.ToString() == trans.GetwayName && m.IsActive == true).FirstOrDefault().bankName + " " + dbContext.BranchMsts.Where(m => m.branchId.ToString() == trans.bankcode && m.IsActive == true).FirstOrDefault().branchName,
                                     //In Case of applicant GST No saved in fax no 30 Aug 2017
                                     ApplicantGSTNumber = appl.GSTNO,
                                     BankIFSCCode = trans.udf5,
                                     VirtualAccPrefix = trans.virtualaccountno,
                                     ValidTillDate = dbContext.SchemeMsts.Where(m => m.schemeId == appl.schemeId && m.IsActive == true && m.completed == true && m.Status != OSConstant.SchemeClosed).FirstOrDefault().endDate
                                 },
                                 PaymentModel = new SchemePaymentViewModel
                                 {
                                     ApplicationFormId = trans.ServiceRefId,
                                     TransactionKey = trans.TrKey,
                                     TransactionId = trans.txnid,
                                     Amount = trans.Amount,
                                     ProductInfo = trans.productinfo,
                                     Mode = trans.mode,
                                     TransactionStatusId = trans.TranStatus,
                                     Discount = trans.discount,
                                     StatusId = trans.status,
                                     EntryDate = trans.EntryDate,
                                 }
                             }).FirstOrDefault();
                return model;
            }
        }

        public SchemeFormViewModel SaveAndGetOnlineSchemeFormCallan(SchemeFormViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                string _ApiTokenUrl = ConfigurationManager.AppSettings["CHALLAN_API_TOKEN"];
                string _ApiChallanUrl = ConfigurationManager.AppSettings["CHALLAN_API_CREATE"];
                string _ApiUsername = ConfigurationManager.AppSettings["CHALLAN_API_USERNAME"];
                string _ApiPassword = ConfigurationManager.AppSettings["CHALLAN_API_PASSWORD"];
                string _ApiGrantType = ConfigurationManager.AppSettings["CHALLAN_API_GRANT_TYPE"];

                var form = GetOnlineSchemeApplicationFormById(model.ApplicationFormId);
                //var formchallan = SaveSchemeFormPaymentChallanTransaction(model);
                if (form != null)
                {
                    var extrans = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(t => t.ServiceRefId == form.ApplicationFormId && t.ServiceType == 3);
                    if (extrans == null)
                    {
                        OnlineApplicationDetails_trans trans = new OnlineApplicationDetails_trans
                        {
                            ServiceRefId = form.ApplicationFormId,
                            txnid = form.ApplicationFormId.ToString() + "T01",
                            Amount = form.TotalAmount,
                            productinfo = "RTGS/NEFT",
                            mode = "RTGS/NEFT",
                            ServiceType = OSConstant.ReservationMoneyChallanPayment,
                            GetwayName = "67", //form.ChallanBankId.ToString(),
                            //udf5 = form.BankIFSCCode,
                            //virtualaccountno = form.VirtualAccPrefix + form.ApplicationFormId.ToString() + "T01",
                            status = 0,
                            EntryDate = DateTime.Now
                        };
                        dbContext.OnlineApplicationDetails_trans.Add(trans);
                        dbContext.SaveChanges();
                    }
                    
                    var challan = new SchemeFormChallanApiModel();
                    challan.Allottee = form.Applicant;
                    challan.Department_Id = form.DepartmentId;
                    challan.Address = form.CorrespondingAddress;
                    challan.BankCode = "HDFC18C"; //form.BankId.ToString();
                    challan.Mobile_No = form.MobileNumber;
                    challan.ChallanTypeId = 6;
                    challan.PaymentModeId = string.IsNullOrEmpty(model.PaymentTypeId) ? 1 : Convert.ToInt32(model.PaymentTypeId); //1;
                    challan.TotalAmount = form.ProcessingCharge + form.ProcessingCGST + form.ProcessingSGST + form.EarnestMoney;
                    challan.Sector_Name = "";
                    challan.BlockName = "";
                    challan.Plot_No = "";
                    challan.Rid = ""; //form.ApplicationFormId.ToString();
                    challan.Email = form.Email;
                    challan.GST_No = form.ApplicantGSTNumber;
                    challan.PAN = form.PanNumber;
                    challan.FormNumber = form.ApplicationFormId.ToString();

                    var chargeList = new List<SchemeFormChargesApiModel>();
                    chargeList.Add(new SchemeFormChargesApiModel { HeadId = 0, SubheadId = 3, Amount = form.ProcessingCharge });
                    chargeList.Add(new SchemeFormChargesApiModel { HeadId = 148, SubheadId = 289, Amount = form.ProcessingCGST });
                    chargeList.Add(new SchemeFormChargesApiModel { HeadId = 148, SubheadId = 290, Amount = form.ProcessingSGST });
                    chargeList.Add(new SchemeFormChargesApiModel { HeadId = 133, SubheadId = 185, Amount = form.EarnestMoney });
                   
                    challan.ChallanChargesVM = chargeList;

                    form.FormApiChallan = challan;
                }
                return form;
            }
        }

        public SchemeFormViewModel SaveGeneratedSchemeFormChallan(SchemeFormViewModel form, HttpPostedFileBase files)
        {
            int flag = 0;
            string bFlag = string.Empty;
            using (var dbContext = new OSDbContext())
            {
                var data = (from transaction in dbContext.OnlineApplicationDetails_trans
                            join details in dbContext.OnlineApplicationDetails on transaction.ServiceRefId equals details.onlineapplicationId
                            where transaction.ServiceType == 3 && transaction.status == 0 && details.isActive == true && details.onlineapplicationId == form.ApplicationFormId
                            select transaction).FirstOrDefault();
                if (data != null)
                {
                    if (data.txnid == form.SchemePayment.TransactionId)
                    {
                        var trans = dbContext.OnlineApplicationDetails_trans.Where(m => m.AutoID == data.AutoID && m.status == 0).FirstOrDefault();
                        if (trans != null)
                        {
                            //SaveChallanDocumentsForApplicationForm(objOnlineFormViewModel, files);
                            if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.ApplicationFormId)))
                            {
                                if (files != null && files.ContentLength > 0)
                                {
                                    string challan = "Challan";
                                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" +form.ApplicationFormId + "/" + challan)))
                                    {
                                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.ApplicationFormId + "/" + challan));
                                    }
                                    string extension = Path.GetExtension(files.FileName);
                                    var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.ApplicationFormId + "/" + challan + "/" + form.ApplicationFormId + "-" + form.SchemePayment.TransactionId + extension);
                                    files.SaveAs(fileSavePath);
                                    flag = OSReturnTypeId.Success;
                                }
                            }
                            trans.TrKey = form.PayType == "RTGS" ? form.SchemePayment.Udf1 : form.SchemePayment.BankReferenceNo;
                            trans.modifiydate = DateTime.Now;//objOnlineFormViewModel.PaymentModel.EntryDate;
                            dbContext.SaveChanges();
                            form.IsPaidChallanUploaded = true;
                            form.Message = "Updated Successfully";
                            bFlag = "Updated Successfully";
                        }
                    }
                    else
                    {
                        form.IsPaidChallanUploaded = false;
                        form.Message = "Challan is not of this form.";
                        bFlag = "2";
                    }
                }
                else {
                    form.IsPaidChallanUploaded = false;
                    form.Message = "Scheme Form paid challan already updated."; 
                    bFlag = "challan Details (RTGS/NEFT) already updated."; 
                }

            }
            //return bFlag;
            return form;
        }


        public SchemeChallanViewModel GenerateSchemeFormChallanByTemplate(SchemeFormViewModel form)
        {
            using (var dbContext = new OSDbContext())
            {
                var onlineForm = dbContext.OnlineApplicationDetails.Where(m => m.onlineapplicationId == form.ApplicationFormId && m.isActive == true).FirstOrDefault();
                if (onlineForm != null)
                {
                    var _serviceTypeId = onlineForm.departmentId == 1 ? 6 : 1;
                    var transaction = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(f => f.ServiceRefId == form.ApplicationFormId && f.ServiceType != _serviceTypeId);
                    if (transaction == null)
                    {
                        var _schemeBank = dbContext.SchemeBankTrans.FirstOrDefault(b => b.bankId == form.BankId);
                        OnlineApplicationDetails_trans trans = new OnlineApplicationDetails_trans
                        {
                            ServiceRefId = form.ApplicationFormId,
                            txnid = form.ApplicationFormId.ToString() + "T01",
                            Amount = form.TotalAmount,
                            productinfo = "RTGS/NEFT",
                            mode = "RTGS/NEFT",
                            ServiceType = OSConstant.ReservationMoneyChallanPayment,
                            GetwayName = form.BankId != null ? form.BankId.ToString() : "67", //form.ChallanBankId.ToString(),
                            udf5 = _schemeBank.IFSCCode, //form.BankIFSCCode,
                            virtualaccountno = _schemeBank.virtualAccountprefix + form.ApplicationFormId.ToString() + "T01",
                            status = 0,
                            EntryDate = DateTime.Now
                        };
                        dbContext.OnlineApplicationDetails_trans.Add(trans);
                        dbContext.SaveChanges();
                    }
                }

                var model = (from appl in dbContext.OnlineApplicationDetails
                             join trans in dbContext.OnlineApplicationDetails_trans on appl.onlineapplicationId equals trans.ServiceRefId
                             where appl.onlineapplicationId == form.ApplicationFormId && trans.ServiceType == 3
                             select new SchemeChallanViewModel
                             {
                                 ApplicationFormId = appl.onlineapplicationId,
                                 Applicant = appl.gender != OSStringConstant.Company ? appl.firstName + " " + (!string.IsNullOrEmpty(appl.middleName) ? appl.middleName + " " + appl.lastName : appl.lastName) : appl.CompanyName,
                                 Email = appl.email,
                                 MobileNo = appl.mobileNumberP2,
                                 CorrespondAddress = appl.correspondanceAdd,
                                 SchemeFormFee = appl.ApplicationFee,
                                 ProcessingFee = appl.ProcessingCharge,
                                 FormFeeWithGST = appl.ApplicationFee + appl.FormCGST + appl.FormSGST,
                                 ProcessingFeeWithGST = appl.ProcessingCharge + appl.ProcessingSGST + appl.ProcessingCGST,
                                 FormFeeSGST = appl.FormSGST,
                                 FormFeeCGST = appl.FormCGST,
                                 ProcessingSGST = appl.ProcessingSGST,
                                 ProcessingCGST = appl.ProcessingCGST,
                                 TotalAmount = appl.ApplicationFee + appl.ProcessingCharge + appl.EarnestMoney,
                                 EarnestMoney = appl.EarnestMoney,
                                 TotalAmountWithGST = (appl.ProcessingCharge + appl.ProcessingSGST + appl.ProcessingCGST) + (appl.ApplicationFee + appl.FormCGST + appl.FormSGST) + appl.EarnestMoney,
                                 BankName = dbContext.BankMsts.Where(m => m.bankId.ToString() == trans.GetwayName && m.IsActive == true).FirstOrDefault().bankName + " Branch : " + dbContext.SchemeBankTrans.Where(m => m.bankId.ToString() == trans.GetwayName && m.schemeId == appl.schemeId && m.IsActive == true).FirstOrDefault().BranchMst.branchName,
                                 AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == appl.area && m.IsActive == true).FirstOrDefault().floorName,
                                 SchemeName = dbContext.SchemeMsts.Where(m => m.schemeId == appl.schemeId && m.IsActive == true && m.completed == true && m.Status != OSConstant.SchemeClosed).FirstOrDefault().schemeName,
                                 Department = dbContext.DepartmentMsts.Where(m => m.departmentId == appl.departmentId && m.IsActive == true).FirstOrDefault().departmentName,
                                 //In offline case BankId place in gateway name , Branch Id Place in Bank code and Account Place in Card Num Field in OnlineApplicationDetails_trans table 23 aug 2017 According to vishal Shukla Sir
                                 AccountNo = trans.cardnum,
                                 BranchName = dbContext.BankMsts.Where(m => m.bankId.ToString() == trans.GetwayName && m.IsActive == true).FirstOrDefault().bankName + " " + dbContext.BranchMsts.Where(m => m.branchId.ToString() == trans.bankcode && m.IsActive == true).FirstOrDefault().branchName,
                                 //In Case of applicant GST No saved in fax no 30 Aug 2017
                                 GSTNO = appl.GSTNO,
                                 IFSCCode = trans.udf5,
                                 AccountPrefix = trans.virtualaccountno,
                                 TransactionId = trans.txnid,
                                 FormModel = new SchemeFormViewModel
                                 {
                                     ApplicationFormId = appl.onlineapplicationId,
                                     Applicant = appl.gender != OSStringConstant.Company ? appl.firstName + " " + (!string.IsNullOrEmpty(appl.middleName) ? appl.middleName + " " + appl.lastName : appl.lastName) : appl.CompanyName,
                                     Email = appl.email,
                                     MobileNumber = appl.mobileNumberP2,
                                     CorrespondingAddress = appl.correspondanceAdd,
                                     ApplicationFee = appl.ApplicationFee,
                                     ProcessingCharge = appl.ProcessingCharge,
                                     FormFeeGST = appl.ApplicationFee + appl.FormCGST + appl.FormSGST,
                                     ProcessingChargeGST = appl.ProcessingCharge + appl.ProcessingSGST + appl.ProcessingCGST,
                                     FormFeeSGST = appl.FormSGST,
                                     FormFeeCGST = appl.FormCGST,
                                     ProcessingSGST = appl.ProcessingSGST,
                                     ProcessingCGST = appl.ProcessingCGST,
                                     TotalAmount = appl.ApplicationFee + appl.ProcessingCharge + appl.EarnestMoney,
                                     EarnestMoney = appl.EarnestMoney,
                                     TotalAmountGST = (appl.ProcessingCharge + appl.ProcessingSGST + appl.ProcessingCGST) + (appl.ApplicationFee + appl.FormCGST + appl.FormSGST) + appl.EarnestMoney,
                                     ChallanBank = dbContext.BankMsts.Where(m => m.bankId.ToString() == trans.GetwayName && m.IsActive == true).FirstOrDefault().bankName + " Branch : " + dbContext.SchemeBankTrans.Where(m => m.bankId.ToString() == trans.GetwayName && m.schemeId == appl.schemeId && m.IsActive == true).FirstOrDefault().BranchMst.branchName,
                                     AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == appl.area && m.IsActive == true).FirstOrDefault().floorName,
                                     SchemeName = dbContext.SchemeMsts.Where(m => m.schemeId == appl.schemeId && m.IsActive == true && m.completed == true && m.Status != OSConstant.SchemeClosed).FirstOrDefault().schemeName,
                                     Department = dbContext.DepartmentMsts.Where(m => m.departmentId == appl.departmentId && m.IsActive == true).FirstOrDefault().departmentName,
                                     //In offline case BankId place in gateway name , Branch Id Place in Bank code and Account Place in Card Num Field in OnlineApplicationDetails_trans table 23 aug 2017 According to vishal Shukla Sir
                                     ChallanAccountNo = trans.cardnum,
                                     ChallanBranchName = dbContext.BankMsts.Where(m => m.bankId.ToString() == trans.GetwayName && m.IsActive == true).FirstOrDefault().bankName + " " + dbContext.BranchMsts.Where(m => m.branchId.ToString() == trans.bankcode && m.IsActive == true).FirstOrDefault().branchName,
                                     ApplicantGSTNumber = appl.GSTNO,
                                     BankIFSCCode = trans.udf5,
                                     VirtualAccPrefix = trans.virtualaccountno,
                                     ValidTillDate = dbContext.SchemeMsts.Where(m => m.schemeId == appl.schemeId && m.IsActive == true && m.completed == true && m.Status != OSConstant.SchemeClosed).FirstOrDefault().endDate
                                 },
                                 PaymentModel = new SchemePaymentViewModel
                                 {
                                     ApplicationFormId = trans.ServiceRefId,
                                     TransactionKey = trans.TrKey,
                                     TransactionId = trans.txnid,
                                     Amount = trans.Amount,
                                     ProductInfo = trans.productinfo,
                                     Mode = trans.mode,
                                     TransactionStatusId = trans.TranStatus,
                                     Discount = trans.discount,
                                     StatusId = trans.status,
                                     EntryDate = trans.EntryDate,
                                 }
                             }).FirstOrDefault();
                return model;
            }
        }


        public SchemeFormViewModel GetSchemeFormFeeAndChargesByArea(SchemeFormViewModel model)
        {
            using (var dbContext = new OSDbContext())
            {
                var data = (from scheme in dbContext.SchemeMsts
                            where scheme.schemeId == model.SchemeId && scheme.IsActive == true
                            select new SchemeFormViewModel
                            {
                                SchemeId = scheme.schemeId,
                                SchemeName = scheme.schemeName,
                                SchemeStartDate = scheme.startDate,
                                SchemeEndDate = scheme.endDate,
                                DepartmentId = model.DepartmentId,
                                Department = model.DepartmentId != null ? dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == model.DepartmentId).departmentName : string.Empty,
                                ApplicationFee = scheme.FormFee,
                                FormFeeCGST = scheme.FormCGST == null ? 0 : scheme.FormCGST,
                                FormFeeSGST = scheme.FormSGST == null ? 0 : scheme.FormSGST,
                                ProcessingCharge = scheme.ProcessingFee == null ? 0 : scheme.ProcessingFee,
                                ProcessingCGST = scheme.ProcessingCGST == null ? 0 : scheme.ProcessingCGST,
                                ProcessingSGST = scheme.ProcessingSGST == null ? 0 : scheme.ProcessingSGST
                            }).FirstOrDefault();
                data.FormFeeGST = data.FormFeeCGST + data.FormFeeSGST;
                data.ProcessingChargeGST = data.ProcessingCGST + data.ProcessingSGST;

                var schemecost = dbContext.SchemeCostTrans.FirstOrDefault(c => c.schemeId == model.SchemeId && c.departmentId == model.DepartmentId && c.floorId.ToString() == model.AreaRangeId);
                if (schemecost != null)
                {
                    data.EarnestMoney = schemecost.earnestMoney;
                }
                return data;
            }
        }


        public SchemeFormViewModel SaveSchemeFormPaidChallanDetail(SchemeFormViewModel form, HttpPostedFileBase files)
        {
            using (var dbContext = new OSDbContext())
            {
                var data = (from detail in dbContext.OnlineApplicationDetails
                           join transn in dbContext.OnlineApplicationDetails_trans on detail.onlineapplicationId equals transn.ServiceRefId
                           where detail.onlineapplicationId == form.SchemePayment.ApplicationFormId && transn.ServiceType == 3 && transn.status == 0 && detail.isActive == true
                           select transn).FirstOrDefault();
                //var data = (from transaction in dbContext.OnlineApplicationDetails_trans
                //            join details in dbContext.OnlineApplicationDetails on transaction.ServiceRefId equals details.onlineapplicationId
                //            where transaction.ServiceType == 3 && transaction.status == 0 && details.isActive == true && details.onlineapplicationId == form.SchemePayment.ApplicationFormId
                //            select transaction).FirstOrDefault();
                if (data != null)
                {
                    if (data.txnid == form.SchemePayment.TransactionId)
                    {
                        var trans = dbContext.OnlineApplicationDetails_trans.Where(m => m.AutoID == data.AutoID && m.status == 0).FirstOrDefault();
                        if (trans != null)
                        {
                            if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.SchemePayment.ApplicationFormId)))
                            {
                                if (files != null && files.ContentLength > 0)
                                {
                                    string challan = "Challan";
                                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.SchemePayment.ApplicationFormId + "/" + challan)))
                                    {
                                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.SchemePayment.ApplicationFormId + "/" + challan));
                                    }
                                    string extension = Path.GetExtension(files.FileName);
                                    var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + form.SchemePayment.ApplicationFormId + "/" + challan + "/" + form.SchemePayment.ApplicationFormId + "-" + form.SchemePayment.TransactionId + extension);
                                    files.SaveAs(fileSavePath);
                                }
                            }
                            //trans.TrKey = form.PayType == "RTGS" ? form.SchemePayment.Udf1 : form.SchemePayment.BankReferenceNo;
                            trans.TrKey = form.SchemePayment.BankReferenceNo;
                            trans.modifiydate = DateTime.Now;
                            dbContext.SaveChanges();
                            form.ReturnTypeId = OSReturnTypeId.Saved;
                            form.Message = "Challan Updated Successfully";
                        }
                    }
                    else
                    {
                        form.ReturnTypeId = OSReturnTypeId.Mismatch;
                        form.Message = "Transaction Id mismatch";
                    }
                }
                else { 
                    form.ReturnTypeId = OSReturnTypeId.Updated;
                    form.Message = "Challan already updated.";
                }

            }
            return form;
        }
    }
}
