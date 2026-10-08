using NA.PMS.Model;
using NA.PMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using OfficeOpenXml;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Web;
using System.Globalization;
using System.Text.RegularExpressions;
using NA.PMS.Web.Models;
using NoidaAuthority.PMS.Common;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using System.Data.Entity.Core.Objects;
using System.Web.Mvc;
using NA.PMS.Model.Entities;
using System.Data.Entity;

namespace NA.PMS.Repository
{
    public class AllotmentRepository : IAllotmentRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public AllotmentRepository()
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

        #region Manage Application Form Process

        // Get Archived Applications
        public List<ApplicationFormModel> GetArchivedApplications()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID//userInfo.UserID
                                      select dept.DepartmentId).ToList();
                var applications = new List<ApplicationFormModel>();
                applications = (from forms in dbContext.ApplicationDetails
                                join depts in dbContext.DepartmentMsts on forms.departmentId equals depts.departmentId
                                join scheme in dbContext.SchemeMsts on forms.schemeId equals scheme.schemeId
                                where scheme.IsActive == true && scheme.completed == true && loginUserDeptt.Contains(forms.departmentId) && scheme.Status == Constants.SchemeClosed
                                //&& forms.isAllotted != "0" //Bug#92 - Resolved
                                select new ApplicationFormModel
                                {
                                    ApplicationId = forms.applicationId,
                                    SchemeName = scheme.schemeName,
                                    SchemeId = scheme.schemeId,
                                    DepartmentId = depts.departmentId,
                                    DepartmentName = depts.departmentName,
                                    FormNo = forms.formNo,
                                    FirstName = forms.firstName,
                                    MobileNumber = forms.mobileNumberP2,
                                    IsAllotted = forms.isAllotted,
                                    Status = forms.isAllotted == "1" ? Constants.Allotted : "",
                                    SchemeStatus = scheme.Status
                                }).OrderByDescending(x => x.ApplicationId).ToList();
                return applications;
            }
        }

        /// <summary>
        /// Get all Application Forms
        /// </summary>
        /// <returns></returns>
        /// //List<ApplicationFormModel>
        public DataSourceResult GetAllApplications([DataSourceRequest] DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID//userInfo.UserID
                                      select dept.DepartmentId).ToList();
                //  var applications = new List<ApplicationFormModel>();
                var applications = (from forms in dbContext.ApplicationDetails
                                    join depts in dbContext.DepartmentMsts on forms.departmentId equals depts.departmentId
                                    join scheme in dbContext.SchemeMsts on forms.schemeId equals scheme.schemeId
                                    join paymentDetails in dbContext.ApplicationPaymentDetails on forms.applicationId equals paymentDetails.applicationId
                                    where scheme.IsActive == true && scheme.completed == true && loginUserDeptt.Contains(forms.departmentId) && scheme.Status != Constants.SchemeClosed
                                    //&& forms.isAllotted != "0" //Bug#92 - Resolved
                                    select new ApplicationFormModel
                                    {
                                        RID = forms.registrationId != null ? forms.registrationId.ToString() : string.Empty,
                                        ApplicationId = forms.applicationId,
                                        SchemeName = scheme.schemeName,
                                        SchemeId = scheme.schemeId,
                                        DepartmentId = depts.departmentId,
                                        DepartmentName = depts.departmentName,
                                        FormNo = forms.formNo,
                                        FirstName = forms.firstName + " " + forms.middleName + " " + forms.lastName,
                                        MobileNumber = forms.mobileNumberP2,
                                        IsAllotted = forms.isAllotted,
                                        Status = forms.isAllotted == "1" ? Constants.Allotted : "",
                                        SchemeStatus = scheme.Status,
                                        BankId = paymentDetails.bankId
                                    });//.OrderByDescending(x => x.ApplicationId)
                return applications.ToDataSourceResult(request);
            }
        }

        public ApplicationFormModel GetApplicationFormDetailById(int applicationId)
        {

            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID//userInfo.UserID
                                      select dept.DepartmentId).ToList();
                var applications = new ApplicationFormModel();
                applications = (from forms in dbContext.ApplicationDetails
                                join depts in dbContext.DepartmentMsts on forms.departmentId equals depts.departmentId
                                join scheme in dbContext.SchemeMsts on forms.schemeId equals scheme.schemeId
                                //join religion in dbContext.ReligionMsts on forms.religionId equals religion.religionId
                                //join category in dbContext.QuotaMsts on forms.quotaId equals category.quotaId
                                //join occupation in dbContext.OccupationMsts on forms.occupationId equals occupation.occupationId
                                join paymentDetails in dbContext.ApplicationPaymentDetails on forms.applicationId equals paymentDetails.applicationId
                                //join banks in dbContext.BankMsts on paymentDetails.bankId equals banks.bankId
                                //join branches in dbContext.BranchMsts on paymentDetails.branchId equals branches.branchId
                                //where scheme.IsActive == true && scheme.completed == true && loginUserDeptt.Contains(forms.departmentId)
                                where forms.applicationId == applicationId
                                select new ApplicationFormModel
                                {
                                    ApplicationId = forms.applicationId,
                                    SchemeName = scheme.schemeName,
                                    SchemeId = scheme.schemeId,
                                    DepartmentId = depts.departmentId,
                                    DepartmentName = depts.departmentName,
                                    FormNo = forms.formNo,
                                    //FirstName = forms.gender.ToLower() == Constants.Company ? forms.lastName : forms.firstName,
                                    FirstName = forms.firstName,
                                    MiddleName = forms.middleName,
                                    LastName = forms.lastName,
                                    GenderName = forms.gender,
                                    //GenderId = forms.gender.ToLower() == Constants.Male ? 1 : forms.gender.ToLower() == Constants.Female ? 2 : 3,
                                    FatherName = forms.fatherHusbandName,
                                    DOB = forms.dateOfBirth,
                                    //MaritialStatusId = forms.marritalStatus.ToLower() == Constants.Married ? 1 : 2,
                                    MaritialStatusName = forms.marritalStatus,
                                    CorrespondingAddress = forms.correspondanceAdd,
                                    PermanentAddress = forms.permanentAdd,
                                    PhoneNumberP1 = forms.phoneNumberP1,
                                    PhoneNumber = forms.phoneNumberP2,
                                    MobileNumberP1 = forms.mobileNumberP1,
                                    MobileNumber = forms.mobileNumberP2,
                                    OccupationId = forms.occupationId,
                                    //OccupationName = occupation.occupation,
                                    OccupationName = (from opt in dbContext.OccupationMsts where opt.occupationId == forms.occupationId select opt.occupation).FirstOrDefault(),
                                    //CategoryName = category.quotaName,
                                    CategoryName = (from cat in dbContext.QuotaMsts where cat.quotaId == forms.quotaId select cat.quotaName).FirstOrDefault(),
                                    FaxNumberP1 = forms.faxNumberP1,
                                    FaxNumber = forms.faxNumberP2,
                                    ReligionId = forms.religionId,
                                    //ReligionName = religion.religion,
                                    ReligionName = (from rel in dbContext.ReligionMsts where forms.religionId == rel.religionId select rel.religion).FirstOrDefault(),
                                    CategoryId = forms.quotaId,
                                    PanNumber = forms.pan,
                                    AnnualIncome = forms.annualIncome,

                                    AmountDepositedId = paymentDetails.amountDeposited,
                                    //Amount = paymentDetails.paymentMode.ToLower() == Constants.DD ? paymentDetails.amountDeposited : null,
                                    //AmountRTGS = paymentDetails.paymentMode.ToLower() == Constants.RTGS ? paymentDetails.amountDeposited : null,
                                    //BankId = paymentDetails.paymentMode.ToLower() == Constants.DD ? paymentDetails.bankId : null,
                                    //BankIdRTGS = paymentDetails.paymentMode.ToLower() == Constants.RTGS ? paymentDetails.bankId : null,
                                    BankId = paymentDetails.bankId,
                                    BranchId = paymentDetails.branchId,
                                    DemandDraftNumber = paymentDetails.ddNo,
                                    IssueDate = paymentDetails.ddIssueDate,
                                    //IssueDate = paymentDetails.paymentMode.ToLower() == Constants.DD ? paymentDetails.ddIssueDate : null,
                                    //IssueDateRTGS = paymentDetails.paymentMode.ToLower() == Constants.RTGS ? paymentDetails.ddIssueDate : null,
                                    IssueBank = paymentDetails.ddIssueBank,
                                    PaymentType = paymentDetails.paymentMode,
                                    SigningAuthority = forms.signingAuthority,
                                    RegisteredOffice = forms.registeredOffice,
                                    UTRNumber = paymentDetails.utn,
                                    MotherName = forms.motherName,
                                    Email = forms.email,
                                    BankName = (from bank in dbContext.BankMsts where bank.bankId == paymentDetails.bankId select bank.bankName).FirstOrDefault(),
                                    BranchName = (from branch in dbContext.BranchMsts where branch.branchId == paymentDetails.branchId select branch.branchName).FirstOrDefault(),
                                    //BankName = paymentDetails.paymentMode.ToLower() == Constants.DD ? banks.bankName : null,
                                    //BranchName = paymentDetails.paymentMode.ToLower() == Constants.DD ? branches.branchName : null,
                                    IsAllotted = forms.isAllotted,
                                    Status = forms.isAllotted == "1" ? Constants.Allotted : "",
                                    SchemeStatus = scheme.Status
                                }).FirstOrDefault();
                return applications;
            }
        }

        /// <summary>
        /// Add and Edit Application Form
        /// </summary>
        /// <param name="applicationFormModel"></param>
        /// <param name="applicationFormId"></param>
        /// <returns></returns>

        public bool SaveApplicationForm(ApplicationFormModel formModel)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingForm = dbContext.ApplicationDetails.Where(i => i.applicationId == formModel.ApplicationId).FirstOrDefault();
                if (existingForm != null)
                {
                    if (formModel.GenderName.ToLower().Trim() == Constants.Company.ToLower().Trim())
                    {
                        existingForm.firstName = formModel.LastName;
                        existingForm.tFirstName = formModel.LastName;
                        existingForm.T_Company_Name = formModel.LastName;
                    }
                    else
                    {
                        existingForm.firstName = formModel.FirstName;
                        existingForm.lastName = formModel.LastName;
                        existingForm.tFirstName = formModel.FirstName;
                        existingForm.tLastName = formModel.LastName;
                        existingForm.gender = formModel.GenderName;
                    }

                    existingForm.middleName = formModel.MiddleName;

                    if (formModel.MaritialStatusName != null)
                        existingForm.marritalStatus = formModel.MaritialStatusName.ToLower();
                    existingForm.fatherHusbandName = formModel.FatherName;
                    existingForm.motherName = formModel.MotherName;
                    existingForm.email = formModel.Email;
                    existingForm.signingAuthority = formModel.SigningAuthority;
                    existingForm.registeredOffice = formModel.RegisteredOffice;
                    existingForm.correspondanceAdd = formModel.CorrespondingAddress;
                    existingForm.permanentAdd = formModel.PermanentAddress;
                    existingForm.mobileNumberP1 = formModel.MobileNumberP1;
                    existingForm.mobileNumberP2 = formModel.MobileNumber;
                    existingForm.phoneNumberP1 = formModel.PhoneNumberP1;
                    existingForm.phoneNumberP2 = formModel.PhoneNumber;
                    existingForm.faxNumberP1 = formModel.FaxNumberP1;
                    existingForm.faxNumberP2 = formModel.FaxNumber;
                    existingForm.occupationId = formModel.OccupationId;
                    existingForm.quotaId = formModel.CategoryId;
                    existingForm.religionId = formModel.ReligionId;
                    existingForm.pan = formModel.PanNumber;
                    existingForm.modifiedBy = userInfo.UserID.ToString();
                    existingForm.modifiedDate = DateTime.Now;
                    existingForm.annualIncome = formModel.AnnualIncome;
                    existingForm.dateOfBirth = formModel.DOB;


                    // Entries for transfer Detail
                    existingForm.tMiddleName = formModel.MiddleName;
                    //existingForm.tLastName = formModel.LastName;
                    existingForm.tGender = formModel.GenderName;
                    if (formModel.MaritialStatusName != null)
                        existingForm.tMarritalStatus = formModel.MaritialStatusName.ToLower();
                    existingForm.tFatherHusbandName = formModel.FatherName;
                    existingForm.tMotherName = formModel.MotherName;
                    existingForm.tDateOfBirth = formModel.DOB;
                    existingForm.tSigningAuthority = formModel.SigningAuthority;
                    existingForm.tRegisteredOffice = formModel.RegisteredOffice;
                    existingForm.tCorrespondanceAdd = formModel.CorrespondingAddress;
                    existingForm.tPermanentAdd = formModel.PermanentAddress;
                    existingForm.tMobileNumber = formModel.MobileNumber;
                    existingForm.tPhoneNumber = formModel.PhoneNumber;
                    existingForm.tEmail = formModel.Email;
                    existingForm.tOccupationId = formModel.OccupationId;
                    existingForm.tPan = formModel.PanNumber;
                    existingForm.tAnnualIncome = formModel.AnnualIncome;

                    dbContext.SaveChanges();

                    var existingFormPayment = dbContext.ApplicationPaymentDetails.Where(x => x.applicationId == existingForm.applicationId).FirstOrDefault();
                    existingFormPayment.applicationId = formModel.ApplicationId;
                    existingFormPayment.formNo = formModel.FormNo;
                    existingFormPayment.amountDeposited = formModel.AmountDepositedId;
                    existingFormPayment.bankId = formModel.BankId;
                    if (formModel.DemandDraftNumber == null && formModel.UTRNumber != null)
                    {
                        existingFormPayment.utn = formModel.UTRNumber;
                        existingFormPayment.paymentMode = Constants.RTGS;
                    }
                    else
                    {
                        existingFormPayment.ddNo = formModel.DemandDraftNumber;
                        existingFormPayment.ddIssueBank = formModel.IssueBank;
                        existingFormPayment.branchId = formModel.BranchId;
                        existingFormPayment.paymentMode = Constants.DD;
                    }
                    existingFormPayment.modifiedBy = userInfo.UserID.ToString();
                    existingFormPayment.modifiedDate = DateTime.Now;
                    existingFormPayment.ddIssueDate = formModel.IssueDate;
                    dbContext.SaveChanges();
                }
                else
                {
                    var form = new ApplicationDetail();
                    form.schemeId = formModel.SchemeId;
                    form.departmentId = formModel.DepartmentId;
                    form.formNo = formModel.FormNo;
                    form.gender = formModel.GenderName;
                    if (formModel.GenderName == "Company")
                    {
                        form.firstName = formModel.LastName;
                        form.tFirstName = formModel.LastName;
                        form.T_Company_Name = formModel.LastName;
                    }
                    else
                    {
                        form.firstName = formModel.FirstName;
                        form.lastName = formModel.LastName;
                        form.tFirstName = formModel.FirstName;
                        form.tLastName = formModel.LastName;
                    }
                    form.middleName = formModel.MiddleName;
                    if (formModel.MaritialStatusName != null)
                        form.marritalStatus = formModel.MaritialStatusName.ToLower();
                    form.fatherHusbandName = formModel.FatherName;
                    form.motherName = formModel.MotherName;
                    form.email = formModel.Email;
                    form.signingAuthority = formModel.SigningAuthority;
                    form.registeredOffice = formModel.RegisteredOffice;
                    form.correspondanceAdd = formModel.CorrespondingAddress;
                    form.permanentAdd = formModel.PermanentAddress;
                    form.mobileNumberP1 = formModel.MobileNumberP1;
                    form.mobileNumberP2 = formModel.MobileNumber;
                    form.phoneNumberP1 = formModel.PhoneNumberP1;
                    form.phoneNumberP2 = formModel.PhoneNumber;
                    form.faxNumberP1 = formModel.FaxNumberP1;
                    form.faxNumberP2 = formModel.FaxNumber;
                    form.occupationId = formModel.OccupationId;
                    form.quotaId = formModel.CategoryId;
                    form.religionId = formModel.ReligionId;
                    form.pan = formModel.PanNumber;
                    form.createdBy = userInfo.UserID.ToString();
                    form.createdDate = DateTime.Now;
                    form.annualIncome = formModel.AnnualIncome;
                    form.dateOfBirth = formModel.DOB;

                    // Entries for transfer Detail
                    //form.tFirstName = formModel.FirstName;
                    form.tMiddleName = formModel.MiddleName;
                    //form.tLastName = formModel.LastName;
                    form.tGender = formModel.GenderName;
                    if (formModel.MaritialStatusName != null)
                        form.tMarritalStatus = formModel.MaritialStatusName.ToLower();
                    form.tFatherHusbandName = formModel.FatherName;
                    form.tMotherName = formModel.MotherName;
                    form.tDateOfBirth = formModel.DOB;
                    form.tSigningAuthority = formModel.SigningAuthority;
                    form.tRegisteredOffice = formModel.RegisteredOffice;
                    form.tCorrespondanceAdd = formModel.CorrespondingAddress;
                    form.tPermanentAdd = formModel.PermanentAddress;
                    form.tMobileNumber = formModel.MobileNumber;
                    form.tPhoneNumber = formModel.PhoneNumber;
                    form.tEmail = formModel.Email;
                    form.tOccupationId = formModel.OccupationId;
                    form.tPan = formModel.PanNumber;
                    form.tAnnualIncome = formModel.AnnualIncome;

                    var latInsertedRecord = dbContext.ApplicationDetails.Where(x => x.schemeId == formModel.SchemeId && x.departmentId == formModel.DepartmentId && x.formNo == formModel.FormNo).FirstOrDefault();

                    if (latInsertedRecord == null)
                    {
                        dbContext.ApplicationDetails.Add(form);
                        dbContext.SaveChanges();

                        //int lastInsertedApplicationFormId = dbContext.ApplicationDetails.Max(u => u.applicationId);
                        int lastInsertedApplicationFormId = form.applicationId;
                        var payment = new ApplicationPaymentDetail();
                        payment.applicationId = lastInsertedApplicationFormId;
                        payment.paymentMode = formModel.PaymentType;
                        payment.formNo = formModel.FormNo.ToString();
                        payment.amountDeposited = formModel.AmountDepositedId;
                        payment.bankId = formModel.BankId;
                        if (formModel.DemandDraftNumber == null && formModel.UTRNumber != null)
                        {
                            payment.utn = formModel.UTRNumber;
                            //payment.branchId = Constants.BranchIdOther;
                        }
                        else
                        {
                            payment.ddNo = formModel.DemandDraftNumber;
                            payment.ddIssueBank = formModel.IssueBank;
                            payment.branchId = formModel.BranchId;
                        }
                        payment.ddIssueDate = formModel.IssueDate;
                        payment.createdBy = userInfo.UserID.ToString();
                        payment.createdDate = DateTime.Now;
                        dbContext.ApplicationPaymentDetails.Add(payment);
                        dbContext.SaveChanges();
                        flag = true;
                    }
                }
            }
            return flag;
        }


        public List<DDList> GetRIDs()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from f in dbContext.AllotmentMasters
                           where f.isActive == 1
                           orderby f.createdDate descending
                           select new DDList
                           {
                               id = f.rid,
                               text = f.rid.ToString()
                           }).OrderByDescending(x => x.id).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Getting RID's from AllotmentMaster table for updating Personal Info
        /// </summary>
        /// <returns>List of RIds</returns>
        public List<DDList> GetRIDsByDeptt()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstDeptts = (from u in dbContext.UmUserMasters
                                 join d in dbContext.UmUserDepartmentTrans on u.UserRefId equals d.UserRefId
                                 where u.UserRefId == userInfo.UserID
                                 select d.DepartmentId).ToList();
                var lst = (from f in dbContext.AllotmentMasters
                           where f.isActive == 1 && lstDeptts.Contains(f.departmentId) && f.isStatus.ToLower() == Common.AllotmentStatus.Approved.ToString().ToLower()
                           orderby f.createdDate descending
                           select new DDList
                           {
                               id = f.rid,
                               text = f.rid.ToString()
                           }).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Getting RID's from AllotmentMaster table for updating Personal Info
        /// </summary>
        /// <returns>List of RIds</returns>
        public DataSourceResult GetRIDsByDeptt(DataSourceRequest Req, int Rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstDeptts = (from u in dbContext.UmUserMasters
                                 join d in dbContext.UmUserDepartmentTrans on u.UserRefId equals d.UserRefId
                                 where u.UserRefId == userInfo.UserID
                                 select d.DepartmentId).ToList();
                var lst = (from f in dbContext.AllotmentMasters
                           where f.isActive == 1 && lstDeptts.Contains(f.departmentId) && f.isStatus.ToLower() == Common.AllotmentStatus.Approved.ToString().ToLower()
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

        /// <summary>
        /// Get Personal Information of applicant on the basis of RID
        /// </summary>
        /// <returns></returns>
        public ApplicationFormModel GetPersonalInfo(int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var personalInfo = new ApplicationFormModel();
                personalInfo = (from applicationForms in dbContext.ApplicationDetails
                                join allotmentMaster in dbContext.AllotmentMasters on applicationForms.applicationId equals allotmentMaster.applicationId
                                where allotmentMaster.rid == rId && allotmentMaster.isActive == 1
                                select new ApplicationFormModel
                                {
                                    CorrespondingAddress = applicationForms.correspondanceAdd,
                                    PermanentAddress = applicationForms.permanentAdd,
                                    MobileNumberP1 = applicationForms.mobileNumberP1,
                                    MobileNumber = applicationForms.mobileNumberP2,
                                    PhoneNumberP1 = applicationForms.phoneNumberP1,
                                    PhoneNumber = applicationForms.phoneNumberP2,
                                    FaxNumberP1 = applicationForms.faxNumberP1,
                                    FaxNumber = applicationForms.faxNumberP2,
                                    ApplicationId = applicationForms.applicationId,
                                    Email = applicationForms.email
                                }).FirstOrDefault();
                return personalInfo;
            }
        }

        /// <summary>
        /// Update Personal Info of applicant after Allotment
        /// </summary>
        /// <param name="applicationFormModel"></param>
        /// <returns></returns>
        public bool UpdatePersonalInfo(ApplicationFormModel applicationFormModel)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingApplication = dbContext.ApplicationDetails.Where(i => i.applicationId == applicationFormModel.ApplicationId).FirstOrDefault();
                if (existingApplication != null)
                {
                    existingApplication.email = applicationFormModel.Email;
                    existingApplication.correspondanceAdd = applicationFormModel.CorrespondingAddress;
                    existingApplication.permanentAdd = applicationFormModel.PermanentAddress;
                    existingApplication.mobileNumberP1 = applicationFormModel.MobileNumberP1;
                    existingApplication.mobileNumberP2 = applicationFormModel.MobileNumber;
                    existingApplication.phoneNumberP1 = applicationFormModel.PhoneNumberP1;
                    existingApplication.phoneNumberP2 = applicationFormModel.PhoneNumber;
                    existingApplication.faxNumberP1 = applicationFormModel.FaxNumberP1;
                    existingApplication.faxNumberP2 = applicationFormModel.FaxNumber;
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }
        /// <summary>
        /// Check Form No Duplicacy. Form No should be unique with Scheme and Department wise
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <param name="formNo"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public bool CheckFormNo(int schemeId, int departmentId, string formNo, int applicationId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var applicationForm = new ApplicationDetail();
                if (applicationId == 0)
                {
                    applicationForm = dbContext.ApplicationDetails.Where(cond => cond.formNo.Trim() == formNo.Trim() && cond.schemeId == schemeId && cond.departmentId == departmentId).FirstOrDefault();
                }
                else
                {
                    applicationForm = dbContext.ApplicationDetails.Where(cond => cond.formNo.Trim() == formNo.Trim() && cond.schemeId == schemeId && cond.departmentId == departmentId && cond.applicationId != applicationId).FirstOrDefault();
                }
                if (applicationForm != null)
                    flag = true;
                else
                    flag = false;
            }
            return flag;
        }
        /// <summary>
        /// Check CheckIssue Date. It should be in between Scheme's start date and end date
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public bool CheckIssueDate(int schemeId, DateTime issueDate)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var scheme = new SchemeMst();
                scheme = dbContext.SchemeMsts.Where(cond => cond.schemeId == schemeId).FirstOrDefault();
                if (scheme != null)
                {
                    if ((scheme.startDate <= issueDate && issueDate <= scheme.endDate) && issueDate < DateTime.Now)
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

        public ApplicationFormModel GetSchemeDates(int schemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var result = (from schemes in dbContext.SchemeMsts
                              where schemes.schemeId == schemeId
                              select new ApplicationFormModel
                              {
                                  SchemeStartDate = schemes.startDate,
                                  SchemeEndDate = schemes.endDate,
                                  CurrentDate = DateTime.Now
                              }).FirstOrDefault();

                return result;
            }
        }

        /// <summary>
        /// Getting Earnest Money on the basis of Scheme and Department Id for displaying Amount Deposited in Dropdown
        /// </summary>
        /// <param name="SchemeId"></param>
        /// <param name="DepartmentId"></param>
        /// <returns></returns>
        public List<DDLStringList> GetEarnestMoneyBySchemeAndDeptId(int SchemeId, int DepartmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from schemeCostTrans in dbContext.SchemeCostTrans
                           where schemeCostTrans.schemeId.Value == SchemeId && schemeCostTrans.departmentId.Value == DepartmentId
                           select new DDLStringList
                           {
                               id = schemeCostTrans.earnestMoney.ToString(),
                               text = schemeCostTrans.earnestMoney.ToString(),
                           }).Distinct().ToList();
                return lst;
            }

        }

        // Check Duplicate UTR or DD Number
        public bool CheckUTROrDDNumber(string number, string type, int appId)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var applicationPaymentDetail = new ApplicationPaymentDetail();
                if (type == Constants.DD)
                {
                    applicationPaymentDetail = dbContext.ApplicationPaymentDetails.Where(cond => cond.ddNo.Trim() == number.Trim()).FirstOrDefault();
                }
                else if (type == Constants.RTGS)
                {
                    applicationPaymentDetail = dbContext.ApplicationPaymentDetails.Where(cond => cond.utn.Trim() == number.Trim()).FirstOrDefault();
                }
                if (applicationPaymentDetail != null)
                    flag = true;
                else
                    flag = false;
            }
            return flag;
        }

        #endregion End Manage Application Form Process

        #region Manage Request Process

        #region Allotte List For Approval
        /// <summary>
        /// Get Allottee List for Approve and Reject
        /// </summary>
        /// <returns></returns>
        public List<ManageRequestModel> GetAllRequest()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();
                var requests = new List<ManageRequestModel>();

                requests = (from allotmentMaster in dbContext.AllotmentMasters
                            join users in dbContext.UmUserMasters on allotmentMaster.createdBy equals users.UserRefId.ToString()
                            join depts in dbContext.DepartmentMsts on allotmentMaster.departmentId equals depts.departmentId
                            join schemes in dbContext.SchemeMsts on allotmentMaster.schemeId equals schemes.schemeId
                            where allotmentMaster.isActive == 1 && allotmentMaster.isSubmitted == "1"
                            && allotmentMaster.approverBy == userInfo.UserID.ToString()
                            && loginUserDeptt.Contains(allotmentMaster.departmentId)
                            select new ManageRequestModel
                            {
                                SchemeId = allotmentMaster.schemeId,
                                SchemeName = schemes.schemeName,
                                DepartmentId = allotmentMaster.departmentId,
                                Department = depts.departmentName,
                                Status = allotmentMaster.isStatus,
                                ApprovedBy = dbContext.UmUserMasters.Where(x => x.UserRefId.ToString() == allotmentMaster.approverBy).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault(),//TODO now: ApproverName column's value
                                ApprovedOn = allotmentMaster.approveDate,
                                RequestedReceived = allotmentMaster.submitDate,
                                ReuestedBy = dbContext.UmUserMasters.Where(x => x.UserRefId == allotmentMaster.submittedBy).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault(),//TODO: SubmittedBy field to be added.
                                AllotmentDate = allotmentMaster.allotmentDate,//,
                                //ApplicationId = allotmentMaster.applicationId
                                CreatedDate = allotmentMaster.createdDate

                            }).Distinct().ToList();

                return requests;
            }
        }
        /// <summary>
        /// Getting Aplicants list for approval on the basis of scheme and dept Id
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="depid"></param>
        /// <returns></returns>
        public List<ManageRequestModel> AllotteeListForApproval(int schemeId, int depid, DateTime allotmentDate)
        {
            List<ManageRequestModel> allotteeList = new List<ManageRequestModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                allotteeList = (from objAllotteeList in dbContext.SpAllotteeList(schemeId, depid, allotmentDate)
                                select new ManageRequestModel
                                {
                                    RID = objAllotteeList.rid,
                                    PropertyNo = objAllotteeList.PropertyNumber,
                                    FormNo = objAllotteeList.formNo,
                                    ApplicantName = objAllotteeList.applicantName,
                                    Department = objAllotteeList.Department,
                                    ProperyType = objAllotteeList.PropertyType,
                                    EarnestMoney = objAllotteeList.EarnestMoney,
                                    AllotmentMoney = objAllotteeList.EarnestMoney,
                                    AllotmentDate = objAllotteeList.allotmentDate,
                                    Area = objAllotteeList.totalArea
                                }).Distinct().ToList();

                return allotteeList;
            }
        }
        /// <summary>
        /// Saving Post for Allotte List aplicants and unsuccessfull aplicants, also Saving isAllotted true and false for successful and Unsucessful aplicants 
        /// copying unsuccessful aplicants in UnsuccessfulApplicantListMasters table which have isAllotted column False in Aplication Detail table
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="depid"></param>
        /// <param name="comment"></param>
        /// <param name="status"></param>
        /// <param name="isUnSuccessfullAplicants"></param>
        /// <returns></returns>
        public bool SaveRequetForApprovalRequest(int schemeId, int depid, string comment, string status, bool isUnSuccessfullAplicants, DateTime allotmentDate)
        {
            var msg = "";
            var schemeName = GetMasterDataName(schemeId, 1);
            var deptmentName = GetMasterDataName(depid, 2);
            bool flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (isUnSuccessfullAplicants) //Case when Unsuccessful List comes for approval.
                {
                    var unSuccessfullAplicants = dbContext.UnsuccessfulApplicantListMasters.Where(i => i.schemeId == schemeId && i.departmentId == depid).ToList();
                    if (unSuccessfullAplicants != null)
                    {
                        foreach (var objUnSucessfullAplicants in unSuccessfullAplicants)
                        {
                            objUnSucessfullAplicants.status = status;
                            objUnSucessfullAplicants.comment = comment;
                            objUnSucessfullAplicants.approverName = userInfo.UserID.ToString();
                            objUnSucessfullAplicants.approvedDate = DateTime.Now;
                            objUnSucessfullAplicants.modifiedBy = userInfo.UserID.ToString();
                            objUnSucessfullAplicants.modifiedDate = DateTime.Now;

                            objUnSucessfullAplicants.isSubmitted = false;
                        }
                        dbContext.SaveChanges();
                        // Sending Notification
                        var submitById = unSuccessfullAplicants.Select(x => x.submittedBy).FirstOrDefault();
                        var objUserList = dbContext.UmUserMasters.Where(i => i.UserRefId == submitById).FirstOrDefault();
                        //var objUserList = dbContext.UmUserMasters.Where(i => i.UserRefId == unSuccessfullAplicants.Select(x => x.submittedBy).FirstOrDefault()).FirstOrDefault();
                        if (objUserList != null)
                        {
                            //if (status == AllotmentStatus.Approved.ToString())
                            //{
                            //    msg = "Unsuccessfull aplicant list of scheme  " + schemeName + " for  " + deptmentName + " department has been Approved for Approval.";
                            //    var mobNumber = objUserList.Mobile;
                            //    CommonMethords.SMSSend(mobNumber, msg);
                            //}
                            //else
                            //{
                            //    msg = "Unsuccessfull aplicant list of scheme  " + schemeName + " for  " + deptmentName + " department has been Rejected for Approval.";
                            //    var mobNumber = objUserList.Mobile;
                            //    CommonMethords.SMSSend(mobNumber, msg);
                            //}
                        }
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else //Case when Allottee List comes for approval.
                {
                    var objAllotteeLsts = dbContext.AllotmentMasters.Where(i => i.schemeId == schemeId && i.departmentId == depid && i.allotmentDate == allotmentDate).ToList();
                    // Naresh updated SpAllotteeList--DateTime .Now--Plz chnage 
                    var allotteesFormNo = (from objAllotteeList in dbContext.SpAllotteeList(schemeId, depid, allotmentDate) select objAllotteeList.formNo).ToList();
                    var lstAllottedApplicants = (from objAllotteeList in dbContext.ApplicationDetails
                                                 where objAllotteeList.schemeId == schemeId && objAllotteeList.departmentId == depid && allotteesFormNo.Contains(objAllotteeList.formNo)
                                                 select objAllotteeList).ToList();
                    //var unSuccessfullAplicants = (from appDetail in dbContext.ApplicationDetails
                    //where appDetail.schemeId == schemeId && appDetail.departmentId == depid && !allotteesFormNo.ToList().Contains(appDetail.formNo)
                    //select appDetail).ToList();
                    if (objAllotteeLsts != null)
                    {
                        objAllotteeLsts.Select(ua => { ua.isStatus = status; ua.comment = comment; ua.approveDate = DateTime.Now; ua.modifiedBy = userInfo.UserID.ToString(); ua.approverBy = userInfo.UserID.ToString(); ua.modifiedDate = DateTime.Now; return ua; }).ToList();
                        //objAllotteeLsts.isStatus = status;
                        //objAllotteeLsts.comment = comment;
                        //objAllotteeLsts.allotmentDate = DateTime.Now;
                        //objAllotteeLsts.approverBy = userInfo.UserID.ToString();
                        //objAllotteeLsts.modifiedBy = userInfo.UserID.ToString();
                        //objAllotteeLsts.modifiedDate = DateTime.Now;
                        //var objUserList = dbContext.UmUserMasters.Where(i => i.UserRefId == objAllotteeLsts.FirstOrDefault().submittedBy).FirstOrDefault();
                        var objUserList = (from um in dbContext.UmUserMasters.ToList()
                                           join alloteeLst in objAllotteeLsts on um.UserRefId equals alloteeLst.submittedBy
                                           select um).FirstOrDefault();
                        if (lstAllottedApplicants != null)
                        {
                            if (status.ToLower() == Common.AllotmentStatus.Approved.ToString().ToLower())
                            {
                                lstAllottedApplicants.Select(ua => { ua.isAllotted = "1"; return ua; }).ToList();
                                //if (unSuccessfullAplicants != null)
                                //{
                                //    unSuccessfullAplicants.Select(ua => { ua.isAllotted = "0"; return ua; }).ToList();
                                //    foreach (var unsucessfulAplicant in unSuccessfullAplicants)
                                //    {
                                //        var unsuccessfulApplicant = new UnsuccessfulApplicantListMaster();
                                //        unsuccessfulApplicant.applicationId = unsucessfulAplicant.applicationId;
                                //        unsuccessfulApplicant.schemeId = unsucessfulAplicant.schemeId;
                                //        unsuccessfulApplicant.departmentId = unsucessfulAplicant.departmentId;
                                //        unsuccessfulApplicant.status = Common.AllotmentStatus.NotSubmitted.ToString();
                                //        unsuccessfulApplicant.isActive = true;
                                //        unsuccessfulApplicant.createdBy = objAllotteeLsts.FirstOrDefault().submittedBy.ToString();//TODO: Change accordingly when DB is changed by Ankit -> SubmittedBy column of AllotmentMaster.
                                //        unsuccessfulApplicant.createdDate = DateTime.Now;
                                //        unsuccessfulApplicant.submitDate = DateTime.Now;
                                //        dbContext.UnsuccessfulApplicantListMasters.Add(unsuccessfulApplicant);
                                //    }
                                //}
                                //if (objUserList != null)
                                //{
                                //    msg = "Allottee list of scheme  " + schemeName + " for  " + deptmentName + " department has been Approved for Approval.";
                                //    var mobNumber = objUserList.Mobile;//TODO: Should be mobile no of SubmittedBy
                                //    CommonMethords.SMSSend(mobNumber, msg);
                                //}
                            }
                            else
                            {
                                //if (objUserList != null)
                                //{
                                //    msg = "Allottee list of scheme  " + schemeName + " for  " + deptmentName + " department has been Rejected for Approval.";
                                //    var mobNumber = objUserList.Mobile;//TODO: Should be mobile no of SubmittedBy
                                //    CommonMethords.SMSSend(mobNumber, msg);
                                //}
                            }
                            //else { lstAllottedApplicants.Select(ua => { ua.isAllotted = "0"; return ua; }).ToList(); }
                            //dbContext.SaveChanges();
                        }
                        dbContext.SaveChanges();
                        flag = true;

                    }
                    else
                    {
                        flag = true;
                    }
                }
                return flag;
            }
        }
        /// <summary>
        /// Getting data like Approver Name, Comments, Approve Date for showing in page on the basis of Scheme and Dept Id
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="deptmentId"></param>
        /// <returns></returns>
        public ManageRequestModel GetAllotteeListRequestByID(int schemeId, int deptmentId, DateTime AllotmentDates)
        {
            ManageRequestModel AllotteeLst = new ManageRequestModel();

            using (var dbContext = new NoidaPMSEntities())
            {
                var objAllotteeLsts = dbContext.AllotmentMasters.Where(i => i.schemeId == schemeId && i.departmentId == deptmentId && i.isActive == 1 && i.allotmentDate == AllotmentDates).FirstOrDefault();
                if (objAllotteeLsts != null)
                {
                    AllotteeLst.SchemeId = objAllotteeLsts.schemeId.Value;
                    AllotteeLst.DepartmentId = objAllotteeLsts.departmentId.Value;
                    AllotteeLst.From = dbContext.UmUserMasters.Where(x => x.UserRefId.ToString() == objAllotteeLsts.approverBy).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault();
                    AllotteeLst.ApprovedOn = objAllotteeLsts.approveDate;
                    AllotteeLst.Status = objAllotteeLsts.isStatus != null ? objAllotteeLsts.isStatus : Common.AllotmentStatus.NotSubmitted.ToString();
                    AllotteeLst.Comment = objAllotteeLsts.comment != null ? objAllotteeLsts.comment : string.Empty;

                }
                return AllotteeLst;
            }
        }
        #endregion
        #region Unsucessfull Applicant List For Approval
        /// <summary>
        /// Get Unsuccessfull List for Approve and Reject
        /// </summary>
        /// <returns></returns>
        public List<ManageRequestModel> GetUnsuccessfullApplicants()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();
                var requests = new List<ManageRequestModel>();
                requests = (from allotmentMaster in dbContext.UnsuccessfulApplicantListMasters
                            join depts in dbContext.DepartmentMsts on allotmentMaster.departmentId equals depts.departmentId
                            join schemes in dbContext.SchemeMsts on allotmentMaster.schemeId equals schemes.schemeId
                            where allotmentMaster.isActive == true && loginUserDeptt.Contains(allotmentMaster.departmentId) //&& allotmentMaster.approverName == userInfo.UserID.ToString()//&& allotmentMaster.isSubmitted == true 
                            select new ManageRequestModel
                            {
                                SchemeId = allotmentMaster.schemeId,
                                SchemeName = schemes.schemeName,
                                DepartmentId = allotmentMaster.departmentId,
                                Department = depts.departmentName,
                                Status = allotmentMaster.status,
                                ApprovedBy = dbContext.UmUserMasters.Where(x => x.UserRefId.ToString() == allotmentMaster.approverName).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault(),//TODO now: ApproverName column's value
                                ApprovedOn = allotmentMaster.approvedDate,//TODO:ApprovedOn field to be added.
                                RequestedReceived = allotmentMaster.submitDate,
                                ReuestedBy = dbContext.UmUserMasters.Where(x => x.UserRefId == allotmentMaster.submittedBy).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault()//TODO:SubmittedBy field to be added.
                                //ApplicationId = allotmentMaster.applicationId //TODO now
                            }).Distinct().ToList();
                return requests;
            }
        }
        /// <summary>
        /// Getting data like Approver Name, Comments, Approve Date for showing in page on the basis of Scheme and Dept Id
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="deptmentId"></param>
        /// <returns></returns>
        public ManageRequestModel GetUnsuccessfullAplicantstRequestByID(int schemeId, int deptmentId)
        {
            ManageRequestModel AllotteeLst = new ManageRequestModel();

            using (var dbContext = new NoidaPMSEntities())
            {
                var objAllotteeLsts = dbContext.UnsuccessfulApplicantListMasters.Where(i => i.schemeId == schemeId && i.departmentId == deptmentId && i.isActive == true).FirstOrDefault();
                if (objAllotteeLsts != null)
                {
                    AllotteeLst.SchemeId = objAllotteeLsts.schemeId.Value;
                    AllotteeLst.SchemeName = dbContext.SchemeMsts.Where(y => y.schemeId == schemeId).Select(x => x.schemeName).FirstOrDefault();
                    AllotteeLst.DepartmentId = objAllotteeLsts.departmentId.Value;
                    AllotteeLst.From = dbContext.UmUserMasters.Where(x => x.UserRefId.ToString() == objAllotteeLsts.approverName).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault();
                    AllotteeLst.ApprovedOn = objAllotteeLsts.approvedDate;
                    AllotteeLst.Status = objAllotteeLsts.status != null ? objAllotteeLsts.status : Common.AllotmentStatus.NotSubmitted.ToString();
                    AllotteeLst.Comment = objAllotteeLsts.comment != null ? objAllotteeLsts.comment : string.Empty;

                }
                return AllotteeLst;
            }
        }
        /// <summary>
        /// Getting List of Unsucessfull Applicants for Approve and Reject
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="depid"></param>
        /// <returns></returns>
        public List<ManageRequestModel> GetUnsuccessfullApplicantsForApproval(int schemeId, int depid)
        {
            List<ManageRequestModel> allotteeList = new List<ManageRequestModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                allotteeList = (from uam in dbContext.UnsuccessfulApplicantListMasters
                                join ad in dbContext.ApplicationDetails on uam.applicationId equals ad.applicationId
                                join dep in dbContext.DepartmentMsts on uam.departmentId equals dep.departmentId
                                join apd in dbContext.ApplicationPaymentDetails on uam.applicationId equals apd.applicationId
                                join banks in dbContext.BankMsts on apd.bankId equals banks.bankId
                                where uam.schemeId == schemeId && uam.departmentId == depid
                                select new ManageRequestModel
                                {
                                    FormNo = ad.formNo,
                                    ApplicationID = uam.applicationId.Value,
                                    Department = dep.departmentName,
                                    DepartmentId = uam.departmentId.Value,
                                    ApplicantName = ad.firstName + " " + ad.middleName + " " + ad.lastName,
                                    MobileNo = ad.mobileNumberP2,
                                    AmountDeposited = apd.amountDeposited.Value,
                                    BankName = banks.bankName,
                                    PermanentAddress = ad.permanentAdd
                                }).Distinct().ToList();
                return allotteeList;
            }
        }
        #endregion

        #endregion End Manage Request Process


        //public List<AllotmentModel> GetAllotment()
        public DataSourceResult GetAllotment(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var lstAllotment = (from allotment in dbContext.AllotmentMasters
                                    join scheme in dbContext.SchemeMsts on allotment.schemeId equals scheme.schemeId
                                    join prodetail in dbContext.SchemePropTrans on allotment.propertyId equals prodetail.propertyId
                                    join sector in dbContext.SectorMsts on prodetail.sectorId equals sector.sectorId
                                    join block in dbContext.BlockMsts on prodetail.blockId equals block.blockId
                                    join app in dbContext.ApplicationDetails on allotment.applicationId equals app.applicationId
                                    join deptment in dbContext.DepartmentMsts on allotment.departmentId equals deptment.departmentId
                                    where allotment.isStatus.ToLower() != Constants.IsApproved.ToLower() && loginUserDeptt.Contains(allotment.departmentId) && allotment.isActive == 1
                                    select new AllotmentModel
                                    {
                                        RID = allotment.rid,
                                        SchemeId = scheme.schemeId,
                                        SchemeName = scheme.schemeName,
                                        SectorName = sector.sectorName,
                                        BlockName = block.blockName,
                                        PropertyNo = prodetail.propertyNo,
                                        PropertyNumber = sector.sectorName + "/" + block.blockName + " - " + prodetail.propertyNo,
                                        ApplicantName = app.middleName == null ? app.firstName + " " + app.lastName : app.firstName + " " + app.middleName + " " + app.lastName,
                                        AllotmentDate = allotment.allotmentDate,
                                        DepartmentName = deptment.departmentName,
                                        isStatus = allotment.isStatus,
                                        PropertyId = allotment.propertyId
                                    }
                    );
                return lstAllotment.ToDataSourceResult(req);
            }

        }

        public DataSourceResult GetAllotmentPartial(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var lstAllotment = (from allotment in dbContext.AllotmentMasters
                                    join scheme in dbContext.SchemeMsts on allotment.schemeId equals scheme.schemeId
                                    join prodetail in dbContext.SchemePropTrans on allotment.propertyId equals prodetail.propertyId
                                    join sector in dbContext.SectorMsts on prodetail.sectorId equals sector.sectorId
                                    join block in dbContext.BlockMsts on prodetail.blockId equals block.blockId
                                    join app in dbContext.ApplicationDetails on allotment.applicationId equals app.applicationId
                                    join deptment in dbContext.DepartmentMsts on allotment.departmentId equals deptment.departmentId
                                    where allotment.isStatus.ToLower() == Constants.IsApproved.ToLower() && loginUserDeptt.Contains(allotment.departmentId) && allotment.isActive == 1
                                    select new AllotmentModel
                                    {
                                        RID = allotment.rid,
                                        SchemeName = scheme.schemeName,
                                        PropertyNumber = sector.sectorName + "/" + block.blockName + " - " + prodetail.propertyNo,
                                        ApplicantName = app.middleName == null ? app.firstName + " " + app.lastName : app.firstName + " " + app.middleName + " " + app.lastName,
                                        AllotmentDate = allotment.allotmentDate,
                                        DepartmentName = deptment.departmentName,
                                        isStatus = allotment.isStatus,
                                        PropertyId = allotment.propertyId
                                    }
                    );
                return lstAllotment.ToDataSourceResult(req);
            }
        }

        /// <summary>
        /// upload application form from excel sheet as bulk upload
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <param name="uploadExcel"></param>
        /// <returns></returns>
        public string BulkUploadApplicationForm(int? schemeId, int? departmentId, System.Web.HttpPostedFileBase uploadExcel)
        {
            //int formFlag = 0;
            string formFlag = null;
            var excel = new ExcelPackage(uploadExcel.InputStream);
            //var headerFlag = excel.ValidateExcelHeaderName();
            string headerFlag = excel.ValidateExcelHeaderName();
            if (headerFlag != null)
            {
                return headerFlag;
            }
            //var fieldFlag = excel.ValidateExcelDataField();
            string fieldFlag = excel.ValidateExcelDataForApplicationForm();
            if (fieldFlag != null)
            {
                return formFlag = fieldFlag;
            }
            //if (headerFlag == true && fieldFlag == true)
            //if (headerFlag == true)
            //if (headerFlag == null)
            //{
            using (var dbContext = new NoidaPMSEntities())
            {
                var username = (from usr in dbContext.UmUserMasters where usr.UserRefId == userInfo.UserID select usr.FirstName + usr.MiddleName + usr.LastName).FirstOrDefault();
                var formList = excel.ToApplicationModel();
                if (formList != null)
                {
                    foreach (var frm in formList)
                    {
                        //var fnumber = from app in dbContext.ApplicationDetails where app.formNo == frm.FormNo && app.schemeId == schemeId && app.departmentId == departmentId select app.formNo;
                        var fnumber = dbContext.ApplicationDetails.Where(f => f.formNo == frm.FormNo && f.schemeId == schemeId && f.departmentId == departmentId).FirstOrDefault();
                        if (fnumber != null)
                        {
                            //return formFlag = 2;
                            return formFlag = Constants.Duplicate + " " + Constants.Form + " " + fnumber.formNo;
                        }
                    }
                    foreach (var form in formList)
                    {
                        ApplicationDetail app = new ApplicationDetail();
                        app.schemeId = schemeId;
                        app.departmentId = departmentId;
                        app.formNo = form.FormNo;
                        app.firstName = form.FirstName;
                        app.middleName = form.MiddleName;
                        app.lastName = form.LastName;
                        app.gender = form.Gender.ToLower();
                        app.marritalStatus = form.MarritalStatus;
                        app.fatherHusbandName = form.FatherHusbandName;
                        app.motherName = form.MotherName;
                        app.dateOfBirth = form.DateOfBirth;
                        app.signingAuthority = form.SigningAuthority;
                        app.registeredOffice = form.RegisteredOffice;
                        app.correspondanceAdd = form.CorrespondanceAdd;
                        app.permanentAdd = form.PermanentAdd;
                        app.mobileNumberP1 = form.MobileNumberP1;
                        app.mobileNumberP2 = form.MobileNumberP2;
                        app.phoneNumberP1 = form.PhoneNumberP1;
                        app.phoneNumberP2 = form.PhoneNumberP2;
                        app.faxNumberP1 = form.FaxNumberP1;
                        app.faxNumberP2 = form.FaxNumberP2;
                        app.email = form.Email;
                        app.occupationId = form.OccupationId;
                        app.quotaId = form.QuotaId;
                        app.religionId = form.ReligionId;
                        app.pan = form.PanNumber;
                        app.annualIncome = form.AnnualIncome;
                        app.createdBy = username;
                        app.createdDate = DateTime.Now;

                        dbContext.ApplicationDetails.Add(app);
                        dbContext.SaveChanges();

                        int lastInsertedForm = dbContext.ApplicationDetails.Max(u => u.applicationId);
                        ApplicationPaymentDetail payment = new ApplicationPaymentDetail();
                        payment.applicationId = lastInsertedForm;
                        payment.formNo = form.FormNo;
                        payment.paymentMode = form.PaymentMode.ToUpper();
                        payment.bankId = form.BankId;
                        payment.branchId = form.BranchId;
                        payment.amountDeposited = form.AmountDeposited;
                        payment.ddNo = form.DemandDraftNo;
                        payment.ddIssueBank = form.DemandDraftIssueBank;
                        payment.ddIssueDate = form.DemandDraftIssueDate;
                        payment.utn = form.Utn;
                        //payment.challanNo = form.ChallanNo;
                        //payment.challanIssueDate = form.ChallanIssueDate;
                        //payment.paymentDate = form.PaymentDate;
                        payment.createdBy = username;
                        payment.createdDate = DateTime.Now;

                        dbContext.ApplicationPaymentDetails.Add(payment);
                        dbContext.SaveChanges();
                    }
                    //return formFlag = 3;
                    return formFlag = Constants.Uploaded;
                }
                else
                {
                    //return formFlag=1;
                    return formFlag = Constants.NotNull;
                }
            }//dbcontext
            //}
            //else
            //{
            //    //return formFlag;
            //    return formFlag = "invalid";
            //}
            //return formFlag;
        }

        public string UploadBulkApplicationForm(int? schemeId, int? departmentId, System.Web.HttpPostedFileBase uploadExcel)
        {
            //int formFlag = 0;
            string formFlag = null;
            //var exl = new ExcelPackage(uploadExcel);
            var excel = new ExcelPackage(uploadExcel.InputStream);
            string headerFlag = excel.ValidateExcelHeaderNameForApplicationForm();
            //string headerFlag = excel.ValidateExcelHeaderName();
            if (headerFlag != string.Empty)
            {
                return headerFlag;
            }
            string fieldFlag = excel.ValidateExcelDataField();
            //string fieldFlag = excel.ValidateExcelDataForApplicationForm();
            if (fieldFlag != null)
            {
                return formFlag = fieldFlag;
            }

            using (var dbContext = new NoidaPMSEntities())
            {
                //var username = (from usr in dbContext.UmUserMasters where usr.UserRefId == userInfo.UserID select usr.FirstName + usr.MiddleName + usr.LastName).FirstOrDefault();
                //var formList = excel.ToApplicationModel();
                var formList = excel.ToApplicationFormModel();
                if (formList != null)
                {
                    foreach (var frm in formList)
                    {
                        //var fnumber = from app in dbContext.ApplicationDetails where app.formNo == frm.FormNo && app.schemeId == schemeId && app.departmentId == departmentId select app.formNo;
                        var fnumber = dbContext.ApplicationDetails.Where(f => f.formNo == frm.FormNo && f.schemeId == schemeId && f.departmentId == departmentId).FirstOrDefault();
                        if (fnumber != null)
                        {
                            //return formFlag = 2;
                            return formFlag = Constants.Duplicate + " " + Constants.Form + " " + fnumber.formNo;
                        }
                    }
                    foreach (var form in formList)
                    {
                        ApplicationDetail app = new ApplicationDetail();
                        app.schemeId = schemeId;
                        app.departmentId = departmentId;
                        app.formNo = form.FormNo;
                        app.firstName = form.FirstName;
                        app.middleName = form.MiddleName;
                        app.lastName = form.LastName;
                        app.gender = form.Gender.ToLower();
                        app.marritalStatus = form.MarritalStatus;
                        app.fatherHusbandName = form.FatherHusbandName;
                        app.motherName = form.MotherName;
                        app.dateOfBirth = form.DateOfBirth;
                        app.signingAuthority = form.SigningAuthority;
                        app.registeredOffice = form.RegisteredOffice;
                        app.correspondanceAdd = form.CorrespondanceAdd;
                        app.permanentAdd = form.PermanentAdd;
                        app.mobileNumberP1 = form.MobileNumberP1;
                        app.mobileNumberP2 = form.MobileNumberP2;
                        app.phoneNumberP1 = form.PhoneNumberP1;
                        app.phoneNumberP2 = form.PhoneNumberP2;
                        app.faxNumberP1 = form.FaxNumberP1;
                        app.faxNumberP2 = form.FaxNumberP2;
                        app.email = form.Email;
                        app.occupationId = form.OccupationId;
                        app.quotaId = form.QuotaId;
                        app.religionId = form.ReligionId;
                        app.pan = form.PanNumber;
                        app.annualIncome = form.AnnualIncome;
                        //app.createdBy = username;
                        app.createdBy = userInfo.UserID.ToString();
                        app.createdDate = DateTime.Now;

                        dbContext.ApplicationDetails.Add(app);
                        dbContext.SaveChanges();

                        int lastInsertedForm = dbContext.ApplicationDetails.Max(u => u.applicationId);
                        ApplicationPaymentDetail payment = new ApplicationPaymentDetail();
                        payment.applicationId = lastInsertedForm;
                        payment.formNo = form.FormNo;
                        payment.paymentMode = form.PaymentMode.ToUpper();
                        payment.bankId = form.BankId;
                        payment.branchId = form.BranchId;
                        payment.amountDeposited = form.AmountDeposited;
                        payment.ddNo = form.DemandDraftNo;
                        payment.ddIssueBank = form.DemandDraftIssueBank;
                        payment.ddIssueDate = form.DemandDraftIssueDate;
                        payment.utn = form.Utn;
                        //payment.challanNo = form.ChallanNo;
                        //payment.challanIssueDate = form.ChallanIssueDate;
                        //payment.paymentDate = form.PaymentDate;
                        //payment.createdBy = username;
                        payment.createdBy = userInfo.UserID.ToString();
                        payment.createdDate = DateTime.Now;

                        dbContext.ApplicationPaymentDetails.Add(payment);
                        dbContext.SaveChanges();
                    }
                    //return formFlag = 3;
                    return formFlag = Constants.Uploaded;
                }
                else
                {
                    //return formFlag=1;
                    return formFlag = Constants.NotNull;
                }
            }
        }
        /// <summary>
        /// bulk upload for allotment
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <param name="uploadExcel"></param>
        /// <returns></returns>
        public bool BulkUploadForAllotment(int? schemeId, int? departmentId, HttpPostedFileBase uploadExcel)
        {
            var formFlag = false;
            var excel = new ExcelPackage(uploadExcel.InputStream);
            var headerFlag = excel.ValidateExcelHeaderNameForAllotment();
            var fieldFlag = excel.ValidateExcelDataFieldForAllotment();
            if (headerFlag == true && fieldFlag == true)
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    var formList = excel.ToAllotmentModelList();
                    if (formList != null)
                    {
                        foreach (var form in formList)
                        {
                            var appId = (from application in dbContext.ApplicationDetails where application.formNo == form.FormNo select application.applicationId).FirstOrDefault();

                            AllotmentMaster allotment = new AllotmentMaster();
                            allotment.applicationId = appId;
                            allotment.formNo = form.FormNo;
                            allotment.schemeId = schemeId;
                            allotment.departmentId = departmentId;
                            allotment.propertyId = form.PropertyId.Value;
                            allotment.allotmentDate = form.AllotmentDate;
                            allotment.instalmentStartDate = form.InstalmentStartDate;
                            allotment.createdDate = DateTime.Now;
                            dbContext.AllotmentMasters.Add(allotment);
                            dbContext.SaveChanges();
                        }
                        formFlag = true;
                    }
                }
            }
            return formFlag;
        }

        /// <summary>
        /// download in excel format of application form for bulk upload
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public Stream BulkDownloadApplicationForm(int? schemeId, int? departmentId)
        {
            List<PropertyApplicationForm> formList = new List<PropertyApplicationForm>();

            Stream stream = null;
            using (var package = new ExcelPackage(stream ?? new MemoryStream()))
            {
                package.Workbook.Properties.Company = "Noida Authority";
                package.Workbook.Properties.Title = "EPPlus Application Form";
                package.Workbook.Properties.Comments = "Application Form is uploaded as bulk in excel sheet";
                package.Workbook.Worksheets.Add("First Sheet");
                package.Workbook.Worksheets.Add("Second Sheet");
                var worksheet = package.Workbook.Worksheets[1];
                worksheet.DefaultColWidth = 25;
                worksheet.Cells.Style.WrapText = true;
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                string[] headerName = NewExcelApplicationForm.ApplicationFormHeader;
                for (int i = 1; i <= headerName.Count(); i++)
                {
                    worksheet.Cells[1, i].Value = headerName[i - 1];
                }
                //=Sheet4!$B$2:$B$38


                var worksheet2 = package.Workbook.Worksheets[2];
                worksheet2.Cells[1, 1].Value = "abc";
                worksheet2.Cells[2, 1].Value = "abc";
                worksheet2.Cells[3, 1].Value = "abc";
                //worksheet2.Cells[1, 1].Value=properties;
                //worksheet.Cells[1,1].Value = worksheet2!$A$1:$A$3;
                //worksheet.Cells[1,1].Value= Formula("worksheet2!$A$1:$A$3");
                package.Save();
                return package.Stream;
            }
        }



        public Stream DownloadAllotmentExcelFormat()
        {
            Stream stream = null;
            using (var package = new ExcelPackage(stream ?? new MemoryStream()))
            {
                package.Workbook.Properties.Company = "Noida Authority";
                package.Workbook.Properties.Title = "EPPlus Allotment Form";
                package.Workbook.Properties.Comments = "Allotment Form is uploaded as bulk in excel sheet";
                package.Workbook.Worksheets.Add("First Sheet");
                var worksheet = package.Workbook.Worksheets[1];
                worksheet.DefaultColWidth = 25;
                worksheet.Cells.Style.WrapText = true;
                string[] header = Constants.AllotmentHeader;
                for (int i = 1; i <= header.Count(); i++)
                {
                    worksheet.Cells[1, i].Value = header[i - 1];
                }


                package.Save();
                return package.Stream;
            }
        }

        public bool AddAllotment(AllotmentModel allotmentModel, int userid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var objschmePropTrans = dbContext.SchemePropTrans.FirstOrDefault(id => id.schemeId == allotmentModel.SchemeId && id.departmentId == allotmentModel.DepartmentId && id.propertyId == allotmentModel.PropertyId);
                var dataResult = new AllotmentMaster
                {
                    applicationId = allotmentModel.ApplicationId,
                    schemeId = allotmentModel.SchemeId,
                    departmentId = allotmentModel.DepartmentId,
                    formNo = allotmentModel.FormNo,
                    propertyId = allotmentModel.PropertyId.Value,
                    allotmentDate = allotmentModel.AllotmentDate,
                    instalmentStartDate = allotmentModel.InstalmentStartDate,
                    //isStatus = Status.InProgress,//added on 15 june 2017
                    isActive = 1,
                    createdBy = userid.ToString(),
                    createdDate = DateTime.Now
                };

                if (objschmePropTrans != null)
                {
                    if (allotmentModel.DepartmentId == Convert.ToInt32(Departmentenum.Housing))
                    {
                        var objschmeCostTrans = dbContext.SchemeCostTrans.FirstOrDefault(id => id.schemeId == allotmentModel.SchemeId && id.departmentId == allotmentModel.DepartmentId);
                        if (objschmeCostTrans != null)
                        {
                            objschmePropTrans.totalPropertyCost = objschmeCostTrans.totalPropertyCost;
                        }
                    }
                    else if (objschmePropTrans.totalArea != allotmentModel.TotalArea && allotmentModel.DepartmentId != Convert.ToInt32(Departmentenum.Housing))
                    {
                        var objschmeCostTrans = dbContext.SchemeCostTrans.FirstOrDefault(id => id.schemeId == allotmentModel.SchemeId && id.departmentId == allotmentModel.DepartmentId);
                        var objschmedepttTrans = dbContext.SchemeDepartmentTrans.FirstOrDefault(id => id.schemeId == allotmentModel.SchemeId && id.departmentId == allotmentModel.DepartmentId);
                        if (objschmeCostTrans != null)
                        {
                            objschmePropTrans.totalArea = allotmentModel.TotalArea;
                            objschmePropTrans.totalPropertyCost = allotmentModel.TotalPropertyCost;
                            //objschmePropTrans.totalPropertyCost = (allotmentModel.TotalArea) * objschmeCostTrans.landRatePerSqmt;
                        }
                        if (objschmedepttTrans != null)
                        {
                            objschmePropTrans.allotmentMoney = allotmentModel.AllotmentMoney;
                            //objschmePropTrans.allotmentMoney = (((((allotmentModel.TotalArea) * (objschmeCostTrans.landRatePerSqmt)) * Convert.ToDecimal(objschmedepttTrans.allotmentMoneyPercent) / 100) - objschmeCostTrans.earnestMoney));
                        }
                        //************Caluculation of allotment money remains.****************
                    }
                }
                dbContext.AllotmentMasters.Add(dataResult);
                var i = dbContext.AllotmentMasters.FirstOrDefault(x => x.propertyId == allotmentModel.PropertyId && x.isActive == 1);
                if (i == null)
                {
                    dbContext.SaveChanges();
                    if (dataResult.rid > 0)
                    {
                        var objresult = dbContext.ApplicationDetails.FirstOrDefault(id => id.applicationId == allotmentModel.ApplicationId);
                        if (objresult != null)
                        {
                            objresult.registrationId = dataResult.rid;
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            return true;
        }

        public List<DDLStringList> GetAllForms(int SchemeId, int DepartmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var allotedApplications = (from appl in dbContext.AllotmentMasters where appl.schemeId == SchemeId && appl.departmentId == DepartmentId && appl.isActive == 1 select appl.applicationId).ToList();
                var lst = (from appdetails in dbContext.ApplicationDetails
                           where appdetails.schemeId.Value == SchemeId && appdetails.departmentId.Value == DepartmentId
                           select new DDLStringList
                           {
                               id = appdetails.formNo,
                               text = appdetails.formNo,
                               RecordExistsIn = appdetails.registrationId != null ? true : false
                           }).ToList();
                return lst;
            }
        }
        /// <summary>
        /// To get the details of Applicant details on the basis of formno, scheme and department.
        /// </summary>
        /// <param name="formno"></param>
        /// <param name="SchemeId"></param>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        public ApplicationFormModel GetApplicantDetails(int rID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from appdet in dbContext.ApplicationDetails
                           join schemeCost in dbContext.SchemeCostTrans on new { appdet.schemeId, appdet.departmentId } equals new { schemeCost.schemeId, schemeCost.departmentId }
                           where appdet.registrationId == rID
                           select new ApplicationFormModel
                            {
                                FormNo = appdet.formNo,
                                FirstName = appdet.tFirstName + " " + appdet.tMiddleName + " " + appdet.tLastName,
                                GenderName = appdet.tGender,
                                FatherName = appdet.tFatherHusbandName,
                                MobileNumber = appdet.tMobileNumber,
                                CorrespondingAddress = appdet.tCorrespondanceAdd,
                                earnestMoney = schemeCost.earnestMoney.Value,
                                ApplicationId = appdet.applicationId,
                                AnnualIncome = appdet.tAnnualIncome,
                                PanNumber = appdet.tPan,
                                OccupationName = (from occu in dbContext.OccupationMsts where occu.occupationId == appdet.occupationId select occu.occupation).FirstOrDefault(),
                                ReligionName = (from reli in dbContext.ReligionMsts where reli.religionId == appdet.religionId select reli.religion).FirstOrDefault(),
                                CategoryName = (from quot in dbContext.QuotaMsts where quot.quotaId == appdet.quotaId select quot.quotaName).FirstOrDefault(),
                                RecordExistsIn = appdet.registrationId != null ? true : false,
                                SigningAuthority = appdet.tSigningAuthority,
                                Email = appdet.tEmail,
                                PermanentAddress = appdet.tPermanentAdd
                                //DepartmentId = appdet.departmentId.Value,
                                //SchemeId = appdet.schemeId.Value,
                                //PropID = spt.propertyId.Value
                            }).FirstOrDefault();
                return lst;
            }
        }

        /// <summary>
        /// To get the details of Applicant details on the basis of formno, scheme and department.
        /// </summary>
        /// <param name="formno"></param>
        /// <param name="SchemeId"></param>
        /// <param name="departmentID"></param>
        /// Created by vishal shukla 11-Jul-2016
        /// <returns></returns>
        //public ApplicationFormModel GetApplicantDetails(string formno, int SchemeId, int departmentID)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {

        //        var lst = (from appdet in dbContext.ApplicationDetails
        //                   join schemeCost in dbContext.SchemeCostTrans on appdet.schemeId equals schemeCost.schemeId
        //                   join quot in dbContext.QuotaMsts on appdet.quotaId equals quot.quotaId
        //                   join occu in dbContext.OccupationMsts on appdet.occupationId equals occu.occupationId
        //                   join reli in dbContext.ReligionMsts on appdet.religionId equals reli.religionId
        //                   where appdet.formNo == formno.ToString() && appdet.schemeId == SchemeId && appdet.departmentId == departmentID
        //                   select new ApplicationFormModel
        //                   {
        //                       FormNo = appdet.formNo,
        //                       FirstName = appdet.tFirstName + " " + appdet.middleName + " " + appdet.tLastName,
        //                       GenderName = appdet.tGender,
        //                       FatherName = appdet.tFatherHusbandName,
        //                       MobileNumber = appdet.tMobileNumber,
        //                       CorrespondingAddress = appdet.tCorrespondanceAdd,
        //                       earnestMoney = schemeCost.earnestMoney.Value,
        //                       ApplicationId = appdet.applicationId,
        //                       AnnualIncome = appdet.tAnnualIncome,
        //                       PanNumber = appdet.tPan,
        //                       OccupationName = occu.occupation,
        //                       ReligionName = reli.religion,
        //                       CategoryName = quot.quotaName,
        //                       RecordExistsIn = (from allot in dbContext.AllotmentMasters where allot.formNo == formno && allot.schemeId == SchemeId && allot.departmentId == departmentID && allot.isActive == 1 select true).FirstOrDefault()
        //                       // RecordExistsIn = appdet.registrationId != null ? true : false
        //                   }).FirstOrDefault();
        //        return lst;
        //    }
        //}

        public ApplicationFormModel GetApplicantDetails(string formno, int SchemeId, int departmentID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var application = dbContext.ApplicationDetails.Where(a => a.formNo == formno && a.schemeId == SchemeId && a.departmentId == departmentID).FirstOrDefault();

                var applicant = (from appl in dbContext.ApplicationDetails
                                 join scot in dbContext.SchemeCostTrans on new { x1 = appl.schemeId, x2 = appl.departmentId } equals new { x1 = scot.schemeId, x2 = scot.departmentId }
                                 where appl.formNo == formno && appl.schemeId == SchemeId && appl.departmentId == departmentID
                                 select new ApplicationFormModel
                                 {
                                     ApplicationId = appl.applicationId,
                                     FormNo = appl.formNo,
                                     FirstName = appl.firstName,
                                     MiddleName = appl.middleName,
                                     LastName = appl.lastName,
                                     GenderName = appl.gender,
                                     FatherName = appl.fatherHusbandName,
                                     MotherName = appl.motherName,
                                     PhoneNumber = appl.phoneNumberP2,
                                     MobileNumber = appl.mobileNumberP2,
                                     Email = appl.email,
                                     PanNumber = appl.pan,
                                     CorrespondingAddress = appl.correspondanceAdd,
                                     PermanentAddress = appl.permanentAdd,
                                     SigningAuthority = appl.signingAuthority,
                                     RegisteredOffice = appl.registeredOffice,
                                     AnnualIncome = appl.annualIncome,
                                     OccupationName = appl.OccupationMst.occupation,
                                     ReligionName = appl.ReligionMst.religion,
                                     CategoryName = appl.QuotaMst.quotaName,
                                     earnestMoney = scot.earnestMoney,
                                     RecordExistsIn = appl.registrationId != null ? true : false
                                     //RecordExistsIn = (from allot in dbContext.AllotmentMasters where allot.formNo == formno && allot.schemeId == SchemeId && allot.departmentId == departmentID && allot.isActive == 1 select true).FirstOrDefault()
                                 }).FirstOrDefault();
                return applicant;
            }
        }


        public CICModel GetApplicantInfo(string formno, int SchemeId, int departmentID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var applicant = (from appl in dbContext.ApplicationDetails
                                 join scot in dbContext.SchemeCostTrans on new { x1 = appl.schemeId, x2 = appl.departmentId } equals new { x1 = scot.schemeId, x2 = scot.departmentId }
                                 where appl.formNo == formno && appl.schemeId == SchemeId && appl.departmentId == departmentID
                                 select new CICModel
                                 {
                                     applicationId = appl.applicationId,
                                     ApplicantName = appl.tFirstName + " " + appl.tMiddleName + " " + appl.tLastName,
                                     Gender = appl.gender,
                                     FatherName = appl.fatherHusbandName,
                                     MobileNumber = appl.tMobileNumber,
                                     Email = appl.email,
                                     IsAllotted = (from allot in dbContext.AllotmentMasters where allot.schemeId == SchemeId && allot.departmentId == departmentID && allot.formNo == formno && allot.isStatus == Constants.IsApproved && allot.isActive == 1 select "true").FirstOrDefault()
                                 }).FirstOrDefault();

                var firmMaster = dbContext.Firm_Master.Where(m => m.Application_Id == applicant.applicationId && m.Is_Active == 1).FirstOrDefault();
                if (firmMaster != null)
                {
                    applicant.NewFirmName = firmMaster.New_Firm_Name;
                    applicant.NewFirmProduct = firmMaster.New_Firm_Product;
                    applicant.NewFirmStatus = firmMaster.New_Firm_Status;
                    applicant.Rid = firmMaster.Rid;
                }
                return applicant;
            }
        }

        #region "Manage Allotee List "
        public DataSourceResult GetAlloteeLists(DataSourceRequest sourceReq)
        {
            int userid = userInfo.UserID;
            //List<AllotteeListModel> allotteeList = new List<AllotteeListModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userid
                                      select dept.DepartmentId).ToList();

                var allotteeList = (from schemeMsts in dbContext.SchemeMsts
                                    join allotmentmst in dbContext.AllotmentMasters on schemeMsts.schemeId equals allotmentmst.schemeId
                                    join departmentMsts in dbContext.DepartmentMsts on allotmentmst.departmentId equals departmentMsts.departmentId
                                    where allotmentmst.isActive == 1 && loginUserDeptt.Contains(allotmentmst.departmentId)
                                    select new AllotteeListModel
                                     {
                                         SchemeName = schemeMsts.schemeName,
                                         SchemeId = schemeMsts.schemeId,
                                         DepartmentName = departmentMsts.departmentName,
                                         DepartmentId = allotmentmst.departmentId.Value,
                                         AllotmentDate = allotmentmst.allotmentDate,
                                         Status = allotmentmst.isStatus != null ? allotmentmst.isStatus : "Not Submitted",
                                         ApproverByName = allotmentmst.approverBy != null ? dbContext.UmUserMasters.Where(p => p.UserRefId.ToString() == allotmentmst.approverBy).Select(p => p.MiddleName == null ? p.FirstName + " " + p.LastName : p.FirstName + " " + p.MiddleName + " " + p.LastName).FirstOrDefault() : string.Empty,
                                         //ApplicationId = allotmentmst.applicationId.Value,
                                         SubmittedDate = allotmentmst.submitDate
                                     }
                                ).Distinct();
                return allotteeList.ToDataSourceResult(sourceReq);

                //var list = (from allotment in dbContext.AllotmentMasters
                //            where allotment.isActive == 1 && DepartmentList.Contains(allotment.departmentId) && allotment.ApplicationDetail.isAllotted == null
                //            group allotment by allotment.schemeId into alotment
                //            join scheme in dbContext.SchemeMsts on alotment.FirstOrDefault().schemeId equals scheme.schemeId
                //            join department in dbContext.DepartmentMsts on alotment.FirstOrDefault().departmentId equals department.departmentId
                //            select new AllotteeListModel
                //            {
                //                SchemeName = scheme.schemeName,
                //                SchemeId = scheme.schemeId,
                //                DepartmentName = department.departmentName,
                //                DepartmentId = department.departmentId,
                //                AllotmentDate = alotment.OrderByDescending(x=>x.rid).FirstOrDefault().allotmentDate,
                //                Status = alotment.OrderByDescending(x => x.rid).FirstOrDefault().isStatus != null ? alotment.OrderByDescending(x => x.rid).FirstOrDefault().isStatus : "Not Submitted",
                //                ApproverByName = alotment.OrderByDescending(x => x.rid).FirstOrDefault().approverBy != null ? dbContext.UmUserMasters.Where(p => p.UserRefId.ToString() == alotment.OrderByDescending(x => x.rid).FirstOrDefault().approverBy).Select(p => p.MiddleName == null ? p.FirstName + " " + p.LastName : p.FirstName + " " + p.MiddleName + " " + p.LastName).FirstOrDefault() : string.Empty,
                //                SubmittedDate = alotment.OrderByDescending(x => x.rid).FirstOrDefault().submitDate
                //            }).Distinct();
                //return list.ToDataSourceResult(sourceReq);
            }
        }
        public List<AllotteeListModel> AllotteeListView(int schemeId, int depid, DateTime allotmentDate)
        {

            List<AllotteeListModel> allotteeList = new List<AllotteeListModel>();
            using (var dbContext = new NoidaPMSEntities())
            {
                allotteeList = (from objAllotteeList in dbContext.SpAllotteeList(schemeId, depid, allotmentDate)
                                select new AllotteeListModel
                                {
                                    RID = objAllotteeList.rid,
                                    PropertyNo = objAllotteeList.PropertyNumber,
                                    FormNo = objAllotteeList.formNo,
                                    ApplicantName = objAllotteeList.applicantName,
                                    DepartmentName = objAllotteeList.Department,
                                    ProperyType = objAllotteeList.PropertyType,
                                    EarnestMoney = objAllotteeList.EarnestMoney,
                                    AllotmentMoney = objAllotteeList.allotmentMoney,
                                    AllotmentDate = objAllotteeList.allotmentDate,
                                    Area = objAllotteeList.totalArea,
                                    PropertyId = objAllotteeList.propertyId,
                                    ApplicationId = objAllotteeList.applicationId,
                                    // PropRefId = objAllotteeList.RefId
                                    PropRefId = objAllotteeList.refId

                                }).ToList();

                return allotteeList;
            }
        }
        public AllotteeListModel GetAllotteeListByID(int schemeId, int deptmentId, DateTime AllotmentDates)
        {
            AllotteeListModel AllotteeLst = new AllotteeListModel();

            using (var dbContext = new NoidaPMSEntities())
            {
                var objAllotteeLsts = dbContext.AllotmentMasters.Where(i => i.schemeId == schemeId && i.departmentId == deptmentId && i.isActive == 1 && i.allotmentDate == AllotmentDates).FirstOrDefault();
                if (objAllotteeLsts != null)
                {

                    AllotteeLst.SchemeId = objAllotteeLsts.schemeId.Value;
                    AllotteeLst.DepartmentId = objAllotteeLsts.departmentId.Value;
                    AllotteeLst.From = objAllotteeLsts.approverBy != null ? dbContext.UmUserMasters.Where(p => p.UserRefId.ToString() == objAllotteeLsts.approverBy).Select(p => p.FirstName + " " + p.MiddleName + " " + p.LastName).FirstOrDefault() : string.Empty;
                    AllotteeLst.SubmittedDate = objAllotteeLsts.submitDate;
                    AllotteeLst.Status = objAllotteeLsts.isStatus != null ? objAllotteeLsts.isStatus : "Not Submitted";
                    AllotteeLst.Comment = objAllotteeLsts.comment != null ? objAllotteeLsts.comment : string.Empty;
                    AllotteeLst.AllotmentDate = objAllotteeLsts.allotmentDate;
                    //AllotteeLst.ApplicationId = objAllotteeLsts.applicationId.Value;



                }
                return AllotteeLst;
            }
        }
        public bool SaveRequetForApproval(int schemeId, int depid, string user, DateTime allotmentDate)
        {
            bool flag = false;
            var userid = userInfo.UserID;
            using (var dbContext = new NoidaPMSEntities())
            {
                var objAllotteeLsts = dbContext.AllotmentMasters.Where(i => i.schemeId == schemeId && i.departmentId == depid && i.isActive == 1 && i.allotmentDate == allotmentDate).ToList();
                int usrid = Convert.ToInt32(user);
                if (objAllotteeLsts.Count > 0)
                {
                    var objUserList = dbContext.UmUserMasters.Where(i => i.UserRefId == usrid).FirstOrDefault();
                    objAllotteeLsts.Select(ua =>
                    {
                        ua.isStatus = Common.AllotmentStatus.InProgress.ToString();
                        ua.submitDate = DateTime.Now;
                        ua.submittedBy = userid;
                        //ua.approverBy = objUserList.UserRefId.ToString();
                        ua.approverBy = user;
                        ua.isSubmitted = "1";
                        return ua;
                    }).ToList();
                    dbContext.SaveChanges();
                    flag = true;
                    var schemeName = GetMasterDataName(schemeId, 1);
                    var deptmentName = GetMasterDataName(depid, 2);
                    //Dear User, Allottee list of scheme  ~ department has been submitted. Regards, http://mynoida.in
                    //var msg = "Dear User, Allottee list of scheme  " + schemeName + " deptment has been submitted. Regards, http://mynoida.in";
                    var msg = string.Format(NAMessages.AllotteeListSuccess, schemeName);
                    var mobNumber = objUserList.Mobile;
                    //Email
                    var body = "Dear User, Allottee list of scheme  " + schemeName + " deptment has been submitted. Regards, http://mynoida.in";
                    EmailHelper emailHelper = new EmailHelper();
                    emailHelper.Send(objUserList.Email, "Request submitted", body);
                    //End Email
                    //CommonMethords.SMSSend(mobNumber, msg);
                    ApplicationHelper.SendSMS(mobNumber, msg);
                }
                else
                {
                    flag = true;
                }
                return flag;
            }
        }
        /// <summary>
        /// Get master data name base on type and master data id
        /// </summary>


        private string GetMasterDataName(int id, int type)
        {
            string name = string.Empty;


            using (var dbContext = new NoidaPMSEntities())
            {

                if (type == 1)
                {
                    var objSchememst = dbContext.SchemeMsts.Where(i => i.schemeId == id && i.IsActive == true).FirstOrDefault();

                    if (objSchememst != null)
                    {
                        name = objSchememst.schemeName;
                    }

                }
                if (type == 2)
                {
                    var objSchememst = dbContext.DepartmentMsts.Where(dep => dep.departmentId == id && dep.IsActive == true).FirstOrDefault();

                    if (objSchememst != null)
                    {
                        name = objSchememst.departmentName;
                    }

                }
                return name;
            }

        }

        //     /// <summary>
        //     /// Send SMS to user who forgot password
        //     /// </summary>
        //     /// <param name="mobileNo"></param>
        //     /// <param name="msg"></param>

        //     private void SMSSend(string mobileNo, string msg)
        //     {
        //         WebClient client = new WebClient();
        //         string baseurl = ConfigurationManager.AppSettings["SMSsend"].ToString() + ConfigurationManager.AppSettings["SMSUsername"].ToString() + "&password=" + ConfigurationManager.AppSettings["SMSPassword"].ToString() + "&sendername=" + "NETSMS" + "&mobileno=" + mobileNo + "&message=" + msg;
        //         Stream data = client.OpenRead(baseurl);
        //         StreamReader reader = new StreamReader(data);
        //         string s = reader.ReadToEnd();
        //         data.Close();
        //         reader.Close();
        //}

        public string GetBulkAllotmnetLetterPrint(List<int> rIds)
        {
            string strLettter = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (rIds.Count > 0)
                {
                    foreach (var rid in rIds)
                    {

                        var Objdeptt = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == rid && m.isActive == 1);
                        if (Objdeptt != null)
                        {
                            strLettter += CommonMethords.GenerateLetter(Objdeptt.rid, Constants.BulkAllotmentLetterTemplateID, Objdeptt.departmentId.Value, userInfo.UserID);
                            strLettter += "<div style='page-break-after: always;' ></div>";
                        }
                    }

                }

            }
            return strLettter;
        }
        public string GetBulkAllotmnetPaymentLetterPrint(List<int> rIds)
        {
            string strLettter = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (rIds.Count > 0)
                {
                    foreach (var rid in rIds)
                    {

                        var Objdeptt = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == rid && m.isActive == 1);
                        if (Objdeptt != null)
                        {
                            strLettter += CommonMethords.GenerateLetter(Objdeptt.rid, Constants.BulkAllotmentPaymentScheduleTemplateID, Objdeptt.departmentId.Value, userInfo.UserID);
                            strLettter += "<div style='page-break-after: always;' ></div>";
                        }
                    }

                }

            }
            return strLettter;
        }
        public List<DDList> GetAssineTo()
        {
            int userid = userInfo.UserID;
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userid
                                      select dept.DepartmentId).ToList();
                var userList = (from userView in dbContext.UmUserMasters
                                join dept in dbContext.UmUserDepartmentTrans on userView.UserRefId equals dept.UserRefId
                                where userView.IsActive == true && loginUserDeptt.Contains(dept.DepartmentId) && dept.UserRefId != userid && userView.UserRefId != 1001
                                select new DDList
                              {
                                  id = userView.UserRefId,
                                  text = userView.UserName + "\t" + dbContext.UmUserMasters.Where(u => u.UserRefId == userView.UserRefId).Select(f => f.FirstName).FirstOrDefault()
                              }).Distinct().ToList();
                return userList;

                //var lst = (from f in dbContext.UmUserMasters
                //           join dept in dbContext.UmUserDepartmentTrans on f.UserRefId equals dept.UserRefId
                //           where f.IsActive == true && loginUserDeptt.Contains(dept.DepartmentId) && dept.UserRefId != userid
                //           select new DDList
                //           {
                //               id = f.UserRefId,
                //               text = f.UserName
                //           }).Distinct().ToList();
                //return lst;
            }
        }

        /// <summary>
        /// To Get All Property in Sector/Block-Property number.
        /// </summary>
        /// <param name="SchemeId"></param>
        /// <param name="DepartmentId"></param>
        /// <returns></returns>
        public List<DDList> GetAllProperty(int SchemeId, int DepartmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.SchemePropTrans
                           join sec in dbContext.SectorMsts on prop.sectorId.Value equals sec.sectorId
                           join block in dbContext.BlockMsts on prop.blockId.Value equals block.blockId
                           where prop.IsActive == true && prop.schemeId == SchemeId && prop.departmentId == DepartmentId
                           select new DDList
                           {
                               //id = prop.propertyId.Value,
                               id = prop.propertyId.Value == null ? 0 : prop.propertyId.Value,
                               text = sec.sectorName + "/" + block.blockName + "-" + prop.propertyNo,
                               //RecordExistsIn = (from allot in dbContext.AllotmentMasters where allot.schemeId == SchemeId && allot.departmentId == DepartmentId && allot.propertyId == prop.propertyId && allot.isActive == 1 select true).FirstOrDefault()
                           }).ToList();

                foreach (var item in lst)
                {
                    item.RecordExistsIn = (from allot in dbContext.AllotmentMasters where allot.schemeId == SchemeId && allot.departmentId == DepartmentId && allot.propertyId == item.id && ((allot.isActive == 1 || allot.isStatus == Constants.IsApproved)) select true).FirstOrDefault();
                }
                return lst;
            }

        }

        /// <summary>
        /// To Get All Property in Sector/Block-Property number By DataSource Request.
        /// </summary>
        /// /// <param name="DataSourceRequest"></param>
        /// <param name="SchemeId"></param>
        /// <param name="DepartmentId"></param>
        /// <returns></returns>
        public DataSourceResult GetAllProperty(DataSourceRequest Req, int? SchemeId, int? DepartmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from prop in dbContext.SchemePropTrans
                           where
                             prop.IsActive == true &&
                             (SchemeId == null || prop.schemeId == SchemeId) &&
                             (DepartmentId == null || prop.departmentId == DepartmentId) &&
                             !
                               (from AllotmentMaster in dbContext.AllotmentMasters
                                where
                                  (SchemeId == null || AllotmentMaster.schemeId == SchemeId) &&
                                  (DepartmentId == null || AllotmentMaster.departmentId == DepartmentId) &&
                                  AllotmentMaster.isActive == 1
                                select new
                                {
                                    AllotmentMaster.propertyId
                                }).Contains(new { propertyId = (System.Int32)prop.propertyId })
                           select new RidList
                           {
                               id = prop.propertyId.Value == null ? 0 : prop.propertyId.Value,
                               text = prop.SectorMst.sectorName + "/" + prop.BlockMst.blockName + "-" + prop.propertyNo,
                               DepartmentId = prop.departmentId != null ? (int)prop.departmentId : 0
                           });

                return lst.ToDataSourceResult(Req);
            }

        }
        /// <summary>
        /// To get the details of property on the basis of propertyid, scheme and department.
        /// </summary>
        /// <param name="rID"></param>       
        /// <returns></returns>
        public SchemePropertyTransDetail GetPropertyDetails(int rID)
        {
            var lst = new SchemePropertyTransDetail();
            using (var dbContext = new NoidaPMSEntities())
            {
                var allomentMaster = dbContext.AllotmentMasters.FirstOrDefault(id => id.rid == rID);
                if (allomentMaster != null)
                {
                    lst = (from proptrans in dbContext.SchemePropTrans
                           join sch in dbContext.SchemeMsts on proptrans.schemeId equals sch.schemeId
                           join appDetails in dbContext.ApplicationDetails on allomentMaster.rid equals appDetails.registrationId
                           join deptt in dbContext.DepartmentMsts on proptrans.departmentId equals deptt.departmentId
                           join sec in dbContext.SectorMsts on proptrans.sectorId equals sec.sectorId
                           join block in dbContext.BlockMsts on proptrans.blockId equals block.blockId
                           join protype in dbContext.PropertyTypeMsts on proptrans.propertyTypeId.Value equals protype.propertyTypeId
                           //join schemeCost in dbContext.SchemeCostTrans on proptrans.schemeId equals schemeCost.schemeId
                           join schemeDeptt in dbContext.SchemeDepartmentTrans on proptrans.schemeId equals schemeDeptt.schemeId
                           join floor in dbContext.FloorMsts on proptrans.floorId equals floor.floorId
                           join appPay in dbContext.ApplicationPaymentDetails on appDetails.applicationId equals appPay.applicationId
                           where proptrans.propertyId == allomentMaster.propertyId && proptrans.schemeId == allomentMaster.schemeId && proptrans.departmentId == allomentMaster.departmentId && proptrans.IsActive == true
                           select new SchemePropertyTransDetail
                           {
                               SchemeName = sch.schemeName,
                               SchemeTypeId = sch.schemeTypeId,
                               Gender = appDetails.gender,
                               PropertyTypeName = protype.propertyTypeName,
                               BlockName = block.blockName,
                               FloorName = floor.floorName,
                               PropertyCost = proptrans.propertyCost,
                               TotalPropertyCost = proptrans.totalPropertyCost,
                               CoveredArea = proptrans.coveredArea,
                               //Since Location charges and location type could be null
                               LocationCharges = (from locch in dbContext.PropertyLocationChargesTrans where locch.propertyId == proptrans.propertyId && locch.IsActive == true select locch.charges).FirstOrDefault(),
                               LocationType = (from locch in dbContext.PropertyLocationChargesTrans join loc in dbContext.LocationMsts on locch.locationId equals loc.locationId where locch.propertyId == proptrans.propertyId && locch.IsActive == true select loc.locationName).FirstOrDefault(),
                               DepartmentName = deptt.departmentName,
                               SectorName = sec.sectorName,
                               PropertyNo = sec.sectorName + "/" + block.blockName + "-" + proptrans.propertyNo,
                               TotalArea = proptrans.totalArea,
                               ActualArea = proptrans.actualArea,
                               CivilCost = proptrans.civilCost,
                               LandRate = proptrans.landRatePerSqmt,
                               AllotmentDate = allomentMaster.allotmentDate,
                               InstalmentStartDate = allomentMaster.instalmentStartDate,
                               Frequency = schemeDeptt.frequency,
                               PropertyId = proptrans.propertyId,
                               SchemeId = proptrans.schemeId,
                               DepartmentId = proptrans.departmentId,
                               TotalnoofInstallment = (from deptt1 in dbContext.SchemeDepartmentTrans where deptt1.schemeId == proptrans.schemeId && deptt1.departmentId == proptrans.departmentId && deptt1.IsActive.Value == true select deptt1.noOfInstallments).FirstOrDefault(),
                               ApplicantName = appDetails.firstName + " " + appDetails.middleName + " " + appDetails.lastName,
                               EarnestMoney = appPay.amountDeposited
                           }).FirstOrDefault();
                }
                return lst;
            }

        }

        /// <summary>
        /// To get the details of property on the basis of propertyid, scheme and department.
        /// </summary>
        /// <param name="propertyId"></param>
        /// <param name="schemeID"></param>
        /// <param name="departmentId"></param>
        /// Created by vishal shukla dataed 11-jul-2016
        /// <returns></returns>
        public SchemePropertyTransDetail GetPropertyDetails(int propertyId, int schemeID, int departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from proptrans in dbContext.SchemePropTrans
                           join sche in dbContext.SchemeMsts on proptrans.schemeId equals sche.schemeId
                           join sec in dbContext.SectorMsts on proptrans.sectorId equals sec.sectorId
                           join block in dbContext.BlockMsts on proptrans.blockId equals block.blockId
                           join protype in dbContext.PropertyTypeMsts on proptrans.propertyTypeId equals protype.propertyTypeId
                           //join schemeCost in dbContext.SchemeCostTrans on proptrans.schemeId equals schemeCost.schemeId
                           join schemeDeptt in dbContext.SchemeDepartmentTrans on new { proptrans.schemeId, proptrans.departmentId } equals new { schemeDeptt.schemeId, schemeDeptt.departmentId }
                           join floor in dbContext.FloorMsts on proptrans.floorId equals floor.floorId
                           where proptrans.propertyId == propertyId && proptrans.schemeId == schemeID && proptrans.departmentId == departmentId && proptrans.IsActive == true
                           select new SchemePropertyTransDetail
                           {
                               PropertyId = proptrans.propertyId,
                               BlockName = block.blockName,
                               SectorName = sec.sectorName,
                               PropertyTypeName = protype.propertyTypeName,
                               TotalArea = proptrans.totalArea,
                               AllotmentMoney = proptrans.allotmentMoney,
                               Frequency = schemeDeptt.frequency,
                               RecordExistsIn = (from allot in dbContext.AllotmentMasters where allot.propertyId == propertyId && allot.isActive == 1 select true).FirstOrDefault(),
                               TotalPropertyCost = proptrans.totalPropertyCost,
                               CivilCost = proptrans.civilCost,
                               TotalnoofInstallment = schemeDeptt.noOfInstallments,
                               EndDate = sche.endDate,
                               FloorName = floor.floorName,
                               Registry = proptrans.Registry,
                               ParentPropertyId = proptrans.ParentPropertyId != null && proptrans.ParentPropertyId > 0 ? (int)proptrans.ParentPropertyId : 0
                           }).FirstOrDefault();
                return lst;
            }

        }
        #endregion

        /// <summary>
        /// Used for reading Unsuccessful List in Kendo Grid
        /// </summary>
        /// <param name="request">Kendo Grid's internal parameter</param>
        /// <param name="scID">Scheme ID</param>
        /// <param name="depID">Department ID</param>
        /// <returns></returns>
        public DataSourceResult GetUnsuccessfulApplicants(DataSourceRequest request, int scID, int depID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var unsuccessfulApplicants = (from uam in dbContext.UnsuccessfulApplicantListMasters
                                              join ad in dbContext.ApplicationDetails on uam.applicationId equals ad.applicationId
                                              join dep in dbContext.DepartmentMsts on uam.departmentId equals dep.departmentId
                                              join apd in dbContext.ApplicationPaymentDetails on uam.applicationId equals apd.applicationId
                                              where uam.schemeId == scID && uam.departmentId == depID
                                              select new UnsuccessfulApplicant
                                              {
                                                  FormNo = ad.formNo,
                                                  ApplicationID = uam.applicationId.Value,
                                                  DepttName = dep.departmentName,
                                                  DepttId = uam.departmentId.Value,
                                                  ApplicantName = ad.firstName + " " + ad.middleName + " " + ad.lastName,
                                                  MobileNo = ad.mobileNumberP2,
                                                  AmountDeposited = apd.amountDeposited.Value
                                              });
                return unsuccessfulApplicants.ToDataSourceResult(request);
            }
        }

        /// <summary>
        /// Used for reading ManageRefund page's grid.
        /// </summary>
        /// <param name="req">Kendo Grid's internal parameter</param>
        /// <returns></returns>
        public DataSourceResult GetRefundDetails(DataSourceRequest req, int loginUser)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == loginUser
                                      select dept.DepartmentId).ToList();
                var allSchemes = (from uam in dbContext.UnsuccessfulApplicantListMasters
                                  join sm in dbContext.SchemeMsts on uam.schemeId equals sm.schemeId
                                  join dm in dbContext.DepartmentMsts on uam.departmentId equals dm.departmentId
                                  //where sm.IsActive == true && sm.completed == true && dm.IsActive == true && 
                                  where uam.isActive == true && loginUserDeptt.Contains(uam.departmentId)
                                  select new RefundModel
                                  {
                                      SchemeId = uam.schemeId.Value,
                                      SchemeName = sm.schemeName,
                                      DepttId = uam.departmentId.Value,
                                      DepttName = dm.departmentName,
                                      RefundInitiationDate = uam.RefundInitiateDate == null ? null : uam.RefundInitiateDate,
                                      SubmissionDate = uam.submitDate == null ? null : uam.submitDate,
                                      RefundStatusText = uam.status
                                  }).Distinct();
                return allSchemes.ToDataSourceResult(req);
            }
        }

        /// <summary>
        /// Used for getting Unsuccessful List details, such as status -> Approved, Rejected, Refund initiated etc.
        /// </summary>
        /// <param name="scID">Scheme ID</param>
        /// <param name="depID">Department ID</param>
        /// <returns></returns>
        public UnsuccessfulApplicant GetUnsuccessfulLstDetails(int scID, int depID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var det = (from ual in dbContext.UnsuccessfulApplicantListMasters
                           join um in dbContext.UmUserMasters on ual.createdBy equals um.UserRefId.ToString()
                           where ual.schemeId == scID && ual.departmentId == depID
                           select new UnsuccessfulApplicant
                           {
                               CurrentStatus = ual.status,
                               Comment = ual.comment,
                               SubmittedDate = ual.submitDate,
                               From = um.FirstName + " " + um.MiddleName + " " + um.LastName
                           }).FirstOrDefault();
                return det;
            }
        }

        /// <summary>
        /// Used for Submitting for Approval or Initiating Refund
        /// </summary>
        /// <param name="scID">Scheme ID</param>
        /// <param name="depID">Department ID</param>
        /// <param name="action">Action = 1 => Submitting for Approval; Action = 2 => Initiating Refund</param>
        /// <returns>Flag: True -> Data updated; False -> Data not updated</returns>
        public bool UnsuccessfulListAction(int scID, int depID, int action, int loginUserID, string userVal)
        {
            int userEmpId = 0;
            if (!string.IsNullOrEmpty(userVal))
            {
                userEmpId = Convert.ToInt32(userVal);
            }
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (action == 1)
                {
                    var lst = (from ual in dbContext.UnsuccessfulApplicantListMasters
                               where ual.schemeId == scID && ual.departmentId == depID
                               select ual).ToList();
                    if (lst.Count > 0)
                    {
                        lst.Select(ua => { ua.status = Common.AllotmentStatus.InProgress.ToString(); ua.submitDate = DateTime.Now; ua.modifiedDate = DateTime.Now; ua.modifiedBy = loginUserID.ToString(); ua.approverName = userVal; ua.isSubmitted = true; ua.submittedBy = userInfo.UserID; return ua; }).ToList();
                        dbContext.SaveChanges();
                        flag = true;
                        //Notifications
                        var approver = new UmUserMaster();
                        if (userEmpId != 0)
                        {
                            approver = (from us in dbContext.UmUserMasters where us.UserRefId == userEmpId select us).FirstOrDefault();
                        }
                        if (approver != null)
                        {
                            var schemeName = GetMasterDataName(scID, 1);
                            var deptmentName = GetMasterDataName(depID, 2);
                            //Email
                            //Dear User, Unsuccessful List of Scheme ~ Department has been submitted for Approval. Regards, http://mynoida.in
                            var body = "Dear User, Unsuccessful List of Scheme " + schemeName + " has been submitted for Approval. Regards, http://mynoida.in";
                            EmailHelper emailHelper = new EmailHelper();
                            emailHelper.Send(approver.Email, "New Request Submitted", body);
                            //SMS
                            //var msg = "Dear User, Unsuccessful List of Scheme " + schemeName + " Department has been submitted for Approval. Regards, http://mynoida.in";
                            var msg = string.Format(NAMessages.UnsuccessfulListSuccess, schemeName);
                            //CommonMethords.SMSSend(approver.Mobile, msg);
                            ApplicationHelper.SendSMS(approver.Mobile, msg);
                        }
                    }
                }
                else
                {
                    var lst = (from ual in dbContext.UnsuccessfulApplicantListMasters
                               where ual.schemeId == scID && ual.departmentId == depID
                               select ual).ToList();
                    if (lst.Count > 0)
                    {
                        lst.Select(ua => { ua.status = Common.AllotmentStatus.RefundInitiated.ToString(); ua.RefundInitiateDate = DateTime.Now; ua.modifiedDate = DateTime.Now; ua.modifiedBy = loginUserID.ToString(); return ua; }).ToList();
                        dbContext.SaveChanges();
                        flag = true;
                        //TODO: Email to Accounts for Refund Initiation
                    }
                }
            }
            return flag;
        }

        public AllotmentModel GetAllotmentById(int Id)
        {
            using (var dbcontext = new NoidaPMSEntities())
            {
                var dataResult = (from rat in dbcontext.AllotmentMasters
                                  join dep in dbcontext.DepartmentMsts on rat.departmentId equals dep.departmentId
                                  join spt in dbcontext.SchemePropTrans on rat.propertyId equals spt.propertyId
                                  join sec in dbcontext.SectorMsts on spt.sectorId equals sec.sectorId
                                  join blo in dbcontext.BlockMsts on spt.blockId equals blo.blockId
                                  //join 
                                  where (rat.rid == Id)
                                  select new AllotmentModel
                                  {
                                      RID = rat.rid,
                                      ApplicationId = rat.applicationId.Value,
                                      SchemeId = rat.schemeId,
                                      DepartmentId = rat.departmentId,
                                      FormNo = rat.formNo,
                                      PropertyId = rat.propertyId,
                                      AllotmentDate = rat.allotmentDate,
                                      InstalmentStartDate = rat.instalmentStartDate,
                                      isStatus = rat.isStatus,
                                      HdnPropertyID = rat.propertyId,
                                      HdnFormNo = rat.formNo,
                                      DepartmentName = dep.departmentName,
                                      PropertyNumber = sec.sectorName + "/" + blo.blockName + " - " + spt.propertyNo
                                  }
                    ).FirstOrDefault();
                PropertyAllotmentModel pmodel = new PropertyAllotmentModel();
                pmodel.DepartmentId = dataResult.DepartmentId;
                pmodel.SchemeId = dataResult.SchemeId;
                dataResult.PropertyAllotment = pmodel;
                return dataResult;
            }
        }

        public bool UpdateAllotment(AllotmentModel allotmentModel, int userId)
        {
            bool result = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var objresult = dbContext.AllotmentMasters.FirstOrDefault(id => id.rid == allotmentModel.RID);
                var objschmePropTrans = dbContext.SchemePropTrans.FirstOrDefault(id => id.schemeId == allotmentModel.SchemeId && id.departmentId == allotmentModel.DepartmentId && id.propertyId == allotmentModel.PropertyId);
                if (objresult != null)
                {
                    objresult.applicationId = allotmentModel.ApplicationId;
                    objresult.schemeId = allotmentModel.SchemeId;
                    objresult.departmentId = allotmentModel.DepartmentId;
                    objresult.formNo = allotmentModel.FormNo;
                    objresult.propertyId = allotmentModel.PropertyId.Value;
                    objresult.allotmentDate = allotmentModel.AllotmentDate;
                    objresult.instalmentStartDate = allotmentModel.InstalmentStartDate;
                    objresult.modifiedBy = userId.ToString();
                    objresult.modifiedDate = DateTime.Now;
                    if (objschmePropTrans != null)
                    {
                        if (allotmentModel.DepartmentId == Convert.ToInt32(Departmentenum.Housing))
                        {
                            var objschmeCostTrans = dbContext.SchemeCostTrans.FirstOrDefault(id => id.schemeId == allotmentModel.SchemeId && id.departmentId == allotmentModel.DepartmentId);
                            if (objschmeCostTrans != null)
                            {
                                objschmePropTrans.totalPropertyCost = objschmeCostTrans.totalPropertyCost;
                            }
                        }
                        else if (objschmePropTrans.totalArea != allotmentModel.TotalArea && allotmentModel.DepartmentId != Convert.ToInt32(Departmentenum.Housing))
                        {
                            var objschmeCostTrans = dbContext.SchemeCostTrans.FirstOrDefault(id => id.schemeId == allotmentModel.SchemeId && id.departmentId == allotmentModel.DepartmentId);
                            var objschmedepttTrans = dbContext.SchemeDepartmentTrans.FirstOrDefault(id => id.schemeId == allotmentModel.SchemeId && id.departmentId == allotmentModel.DepartmentId);
                            if (objschmeCostTrans != null)
                            {
                                objschmePropTrans.totalArea = allotmentModel.TotalArea;
                                objschmePropTrans.totalPropertyCost = allotmentModel.TotalPropertyCost;
                                //objschmePropTrans.totalPropertyCost = (allotmentModel.TotalArea) * objschmeCostTrans.landRatePerSqmt;
                            }
                            if (objschmedepttTrans != null)
                            {
                                objschmePropTrans.allotmentMoney = allotmentModel.AllotmentMoney;
                                // objschmePropTrans.allotmentMoney = (((((allotmentModel.TotalArea) * (objschmeCostTrans.landRatePerSqmt)) * Convert.ToDecimal(objschmedepttTrans.allotmentMoneyPercent) / 100) - objschmeCostTrans.earnestMoney));
                            }
                        }
                    }
                    //var objappresult = (from appDet in dbContext.ApplicationDetails where appDet.applicationId == allotmentModel.ApplicationId select appDet).FirstOrDefault();
                    //var objappresult = dbContext.ApplicationDetails.Where(id => id.applicationId == allotmentModel.ApplicationId).FirstOrDefault();
                    var objappresult = dbContext.ApplicationDetails.FirstOrDefault(id => id.applicationId.ToString() == allotmentModel.ApplicationId.ToString());
                    var objoldappresult = dbContext.ApplicationDetails.FirstOrDefault(id => id.registrationId.ToString() == allotmentModel.RID.ToString());
                    if (objappresult != null)
                    {
                        if (objoldappresult != null)
                        {
                            objoldappresult.registrationId = null;
                        }
                        objappresult.registrationId = allotmentModel.RID;
                    }
                    dbContext.SaveChanges();
                    result = true;
                }
            }
            return result;
        }

        public ApplicationPayment GetApplicantPaymentDetails(string formno, int applicationId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from appdet in dbContext.ApplicationPaymentDetails
                           join bank in dbContext.BankMsts on appdet.bankId equals bank.bankId
                           join branch in dbContext.BranchMsts on appdet.branchId equals branch.branchId
                           where appdet.formNo.Trim() == formno.Trim() && appdet.applicationId == applicationId
                           select new ApplicationPayment
                           {
                               AmountDeposited = appdet.amountDeposited,
                               BankName = bank.bankName,
                               BranchName = branch.branchName,
                               DdNo = appdet.ddNo,
                               PaymentMode = appdet.paymentMode,
                               DdIssueBank = appdet.ddIssueBank,
                               DdIssueDate = appdet.ddIssueDate
                           }).FirstOrDefault();
                return lst;
            }
        }

        public ChallanModel PrintViewAllotment(int propertyId, int schemeID, int departmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from proptrans in dbContext.SchemePropTrans
                           join sec in dbContext.SectorMsts on proptrans.sectorId equals sec.sectorId
                           join block in dbContext.BlockMsts on proptrans.blockId equals block.blockId
                           join protype in dbContext.PropertyTypeMsts on proptrans.propertyTypeId equals protype.propertyTypeId
                           join schemeCost in dbContext.SchemeCostTrans on proptrans.schemeId equals schemeCost.schemeId
                           join schemeDeptt in dbContext.SchemeDepartmentTrans on proptrans.schemeId equals schemeDeptt.schemeId
                           join allotment in dbContext.AllotmentMasters on proptrans.propertyId equals allotment.propertyId
                           where proptrans.propertyId == propertyId && proptrans.schemeId == schemeID && proptrans.departmentId == departmentId && proptrans.IsActive == true
                           select new ChallanModel
                           {
                               BlockName = block.blockName,
                               SectorName = sec.sectorName,
                               PropertyTypeName = protype.propertyTypeName,
                               FormNo = allotment.formNo,
                               AllotmentMoney = proptrans.allotmentMoney - schemeCost.earnestMoney,
                               RID = allotment.rid

                           }).FirstOrDefault();

                ApplicationFormModel appdetails = new ApplicationFormModel();
                if (lst != null)
                {
                    var appresult = dbContext.ApplicationDetails.FirstOrDefault(id => id.schemeId == schemeID && id.departmentId == departmentId && id.formNo == lst.FormNo);
                    if (appresult != null)
                    {
                        appdetails.FirstName = appresult.firstName + " " + appresult.middleName + "" + appresult.lastName;
                        appdetails.CorrespondingAddress = appresult.correspondanceAdd;
                        appdetails.Email = appresult.email;
                        appdetails.MobileNumber = appresult.mobileNumberP1;
                    }
                    lst.ApplicationForm = appdetails;
                }
                return lst;
            }

        }

        public List<ChallanModel> GetAllotmentDetailsForBulkPrint(List<int> propIds, int schemeId, int deptId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from proptrans in dbContext.SchemePropTrans
                           join sec in dbContext.SectorMsts on proptrans.sectorId equals sec.sectorId
                           join block in dbContext.BlockMsts on proptrans.blockId equals block.blockId
                           join protype in dbContext.PropertyTypeMsts on proptrans.propertyTypeId equals protype.propertyTypeId
                           join schemeCost in dbContext.SchemeCostTrans on proptrans.schemeId equals schemeCost.schemeId
                           join schemeDeptt in dbContext.SchemeDepartmentTrans on proptrans.schemeId equals schemeDeptt.schemeId
                           join allotment in dbContext.AllotmentMasters on proptrans.propertyId equals allotment.propertyId
                           //join appDet in dbContext.ApplicationDetails on allotment.formNo equals appDet.formNo
                           where propIds.Contains(allotment.rid) && proptrans.schemeId == schemeId && proptrans.departmentId == deptId && proptrans.IsActive == true
                           select new ChallanModel
                           {
                               BlockName = block.blockName,
                               SectorName = sec.sectorName,
                               PropertyTypeName = protype.propertyTypeName,
                               FormNo = allotment.formNo,
                               AllotmentMoney = proptrans.allotmentMoney,
                               RID = allotment.rid,
                               ApplicationForm = (from appDet in dbContext.ApplicationDetails
                                                  where appDet.schemeId == schemeId && appDet.departmentId == deptId && appDet.formNo == allotment.formNo
                                                  select new ApplicationFormModel
                                                  {
                                                      FirstName = appDet.firstName + " " + appDet.middleName + "" + appDet.lastName,
                                                      CorrespondingAddress = appDet.correspondanceAdd,
                                                      Email = appDet.email,
                                                      MobileNumber = appDet.mobileNumberP1
                                                  }).FirstOrDefault()
                           }).Distinct().ToList();
                return lst;
            }
        }

        public List<AllotmentLetterModel> GetAllotmentDetailsForBulkLetterPrint(List<int> rIds)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var lst = (from proptrans in dbContext.SchemePropTrans
                //           join sec in dbContext.SectorMsts on proptrans.sectorId equals sec.sectorId
                //           join block in dbContext.BlockMsts on proptrans.blockId equals block.blockId
                //           join protype in dbContext.PropertyTypeMsts on proptrans.propertyTypeId equals protype.propertyTypeId
                //           join schemeCost in dbContext.SchemeCostTrans on proptrans.schemeId equals schemeCost.schemeId
                //           join schemeDeptt in dbContext.SchemeDepartmentTrans on proptrans.schemeId equals schemeDeptt.schemeId
                //           join allotment in dbContext.AllotmentMasters on proptrans.propertyId equals allotment.propertyId
                //           //join appDet in dbContext.ApplicationDetails on allotment.formNo equals appDet.formNo
                //           where propIds.Contains(proptrans.propertyId.Value) && proptrans.schemeId == schemeId && proptrans.departmentId == deptId && proptrans.IsActive == true
                //           select new ChallanModel
                //           {
                //               BlockName = block.blockName,
                //               SectorName = sec.sectorName,
                //               PropertyTypeName = protype.propertyTypeName,
                //               FormNo = allotment.formNo,
                //               AllotmentMoney = proptrans.allotmentMoney,
                //               RID = allotment.rid,
                //               ApplicationForm = (from appDet in dbContext.ApplicationDetails
                //                                  where appDet.schemeId == schemeId && appDet.departmentId == deptId && appDet.formNo == allotment.formNo
                //                                  select new ApplicationFormModel
                //                                  {
                //                                      FirstName = appDet.firstName + " " + appDet.middleName + "" + appDet.lastName,
                //                                      CorrespondingAddress = appDet.correspondanceAdd,
                //                                      Email = appDet.email,
                //                                      MobileNumber = appDet.mobileNumberP1
                //                                  }).FirstOrDefault()
                //           }).ToList();

                var lst = (from allMas in dbContext.AllotmentMasters
                           join appDet in dbContext.ApplicationDetails on allMas.rid equals appDet.registrationId
                           //join paysch in dbContext.PaymentScheduleMasters on allMas.rid equals paysch.Rid
                           join spt in dbContext.SchemePropTrans on allMas.propertyId equals spt.propertyId
                           join depttMas in dbContext.DepartmentMsts on spt.departmentId equals depttMas.departmentId
                           join bloMas in dbContext.BlockMsts on spt.blockId equals bloMas.blockId
                           join sec in dbContext.SectorMsts on spt.sectorId equals sec.sectorId
                           join scheme in dbContext.SchemeMsts on spt.schemeId equals scheme.schemeId
                           join propMas in dbContext.PropertyTypeMsts on spt.propertyTypeId equals propMas.propertyTypeId
                           //join sdt in dbContext.SchemeDepartmentTrans on allMas.propertyId equals sdt.schemeId
                           where rIds.Contains(allMas.rid)
                           select new AllotmentLetterModel
                           {
                               ApplicantName = appDet.firstName + " " + appDet.middleName + " " + appDet.lastName,
                               ApplicantAddress = appDet.permanentAdd,
                               DepttName = depttMas.departmentName,
                               RId = allMas.rid,
                               PropType = propMas.propertyTypeName,
                               PropNo = sec.sectorName + "/" + bloMas.blockName + " - " + spt.propertyNo,
                               SchemeName = scheme.schemeName,
                               //ApplicationDate = appDet.app
                               Sector = sec.sectorName,
                               Block = bloMas.blockName,
                               PropRate = spt.propertyCost,
                               AllotedArea = spt.totalArea,
                               //EarnestMoney = spt.
                               AllotmentDate = allMas.allotmentDate,
                               //InterestRate = sdt.normalInt
                           }).ToList();
                return lst;
            }
        }
        public int BulkPaymentSchedule(List<int> rIds)
        {
            int status = 0;
            if (rIds.Count > 0)
            {
                foreach (var rid in rIds)
                {
                    var param = new SqlParameter
                    {
                        ParameterName = "rid",
                        Value = rid
                    };
                    using (var dbContext = new NoidaPMSEntities())
                    {
                        status = dbContext.Database.SqlQuery<int>("exec Sp_NewPaymentSchedule @rid", param).FirstOrDefault();
                        //return status;
                    }
                }

            }
            return status;


        }

        public ChallanModel PrintViewAllotment(int propertyId, int schemeID, int departmentId, int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from proptrans in dbContext.SchemePropTrans
                           join sec in dbContext.SectorMsts on proptrans.sectorId equals sec.sectorId
                           join block in dbContext.BlockMsts on proptrans.blockId equals block.blockId
                           join protype in dbContext.PropertyTypeMsts on proptrans.propertyTypeId equals protype.propertyTypeId
                           join schemeCost in dbContext.SchemeCostTrans on proptrans.schemeId equals schemeCost.schemeId
                           join schemeDeptt in dbContext.SchemeDepartmentTrans on proptrans.schemeId equals schemeDeptt.schemeId
                           join allotment in dbContext.AllotmentMasters on proptrans.propertyId equals allotment.propertyId
                           join completionDetail in dbContext.Completion_Details on allotment.rid equals completionDetail.Rid
                           where proptrans.propertyId == propertyId && proptrans.schemeId == schemeID && proptrans.departmentId == departmentId && proptrans.IsActive == true
                           && completionDetail.Is_Active == true
                           select new ChallanModel
                           {
                               BlockName = block.blockName,
                               SectorName = sec.sectorName,
                               PropertyTypeName = protype.propertyTypeName,
                               FormNo = allotment.formNo,
                               AllotmentMoney = proptrans.allotmentMoney - schemeCost.earnestMoney,
                               RID = allotment.rid,
                               CompletionCharges = completionDetail.Completion_Charge

                           }).FirstOrDefault();

                ApplicationFormModel appdetails = new ApplicationFormModel();
                if (lst != null)
                {
                    var appresult = dbContext.ApplicationDetails.FirstOrDefault(id => id.schemeId == schemeID && id.departmentId == departmentId && id.formNo == lst.FormNo);
                    if (appresult != null)
                    {
                        appdetails.FirstName = appresult.firstName + " " + appresult.middleName + "" + appresult.lastName;
                        appdetails.CorrespondingAddress = appresult.correspondanceAdd;
                        appdetails.Email = appresult.email;
                        appdetails.MobileNumber = appresult.mobileNumberP1;
                    }
                    lst.ApplicationForm = appdetails;
                }
                return lst;
            }

        }


        //To Get All form Id for compnay.
        public List<DDLStringList> GetAllCompanyForms(int SchemeId, int DepartmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from appdetails in dbContext.ApplicationDetails
                           where appdetails.schemeId.Value == SchemeId && appdetails.departmentId.Value == DepartmentId && appdetails.departmentId.Value != Constants.DepartmentIdForHousing
                           select new DDLStringList
                           {
                               id = appdetails.formNo,
                               text = appdetails.formNo
                           }).ToList();
                return lst;
            }
        }


        //To fill Director Grid on Add Company => under Manage Application.
        public DataSourceResult GetDirectorDetailsByAppID(DataSourceRequest req, int applicationID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int i = 0;
                var lstDirectorDetails = (from rd in dbContext.Firm_Director_Master
                                          join ty in dbContext.Common_Config on rd.Type equals ty.Id
                                          where rd.Is_Active == 1 && rd.Application_Id == applicationID
                                          select new CICModel
                                          {
                                              Director_Id = rd.Director_Id,
                                              Director_Name = rd.Director_Name,
                                              Director_Share = rd.Director_Share,
                                              TypeName = ty.Name,
                                              Type = rd.Type,
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

        //To save Directors for Add company
        public bool SaveDiretors(decimal directorShare, string directorName, int type, int applicationID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var dirDetails = new Firm_Director_Master
                {
                    Director_Name = directorName,
                    Director_Share = directorShare,
                    Type = type,
                    Is_Active = 1,
                    Application_Id = applicationID,
                    Created_By = userInfo.UserID,
                    Created_Date = DateTime.Now
                };
                dbContext.Firm_Director_Master.Add(dirDetails);
                dbContext.SaveChanges();
                return true;
            }
        }

        // To Add Company Details
        public bool AddCompanyDetails(string data, string NewFirmName, string NewFirmProduct, int NewFirmStatus, int appId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var _firmDetails = dbContext.Firm_Master.Where(f => f.Application_Id == appId && f.Is_Active == 1).FirstOrDefault();
                var _dirDetails = dbContext.Firm_Director_Master.Where(d => d.Application_Id == appId && d.Is_Active == 1);
                bool flag = false;

                //if already exist any firm or director details then inactive those records and re-create them.
                if (_firmDetails != null)
                    _firmDetails.Is_Active = 0;

                if (_dirDetails != null)
                {
                    foreach (var item in _dirDetails)
                    {
                        item.Is_Active = 0;
                    }
                }

                var firmDetails = new Firm_Master
                {
                    New_Firm_Name = NewFirmName,
                    Old_Firm_Name = NewFirmName,
                    New_Firm_Product = NewFirmProduct,
                    Old_Firm_Product = NewFirmProduct,
                    New_Firm_Status = NewFirmStatus,
                    Old_Firm_Status = NewFirmStatus,
                    Is_Active = 1,
                    Created_By = userInfo.UserID,
                    Created_Date = DateTime.Now,
                    Application_Id = appId
                };
                dbContext.Firm_Master.Add(firmDetails);
                dbContext.SaveChanges();
                if (firmDetails.Id > 0)
                {
                    dynamic deserilyzData = JsonConvert.DeserializeObject(data);
                    foreach (var item in deserilyzData)
                    {
                        var dirDetails = new Firm_Director_Master();
                        dirDetails.Director_Name = Convert.ToString(item["Director_Name"]);
                        dirDetails.Director_Share = Convert.ToDecimal(item["Director_Share"]);
                        dirDetails.Type = Convert.ToInt32(item["Type"]);
                        dirDetails.Is_Active = 1;
                        dirDetails.Application_Id = appId;
                        dirDetails.Created_By = userInfo.UserID;
                        dirDetails.Created_Date = DateTime.Now;
                        dbContext.Firm_Director_Master.Add(dirDetails);
                    }
                    dbContext.SaveChanges();
                    flag = true;
                }
                return flag;
            }
        }

        //To Cancel Allotment request
        public bool CancelAllotment(int rid)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var existingAllotment = dbContext.AllotmentMasters.Where(i => i.rid == rid).FirstOrDefault();
                var objresult = dbContext.ApplicationDetails.FirstOrDefault(id => id.registrationId == rid);
                if (existingAllotment != null)
                {
                    existingAllotment.isActive = 0;
                    existingAllotment.modifiedBy = userInfo.UserID.ToString();
                    existingAllotment.modifiedDate = DateTime.Now;
                    if (objresult != null)
                    {
                        objresult.registrationId = null;
                    }
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }

        //To generate letter
        public string Generateletter(int rid)
        {
            string strLettter = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                var Objdeptt = dbContext.AllotmentMasters.FirstOrDefault(m => m.rid == rid);
                if (Objdeptt != null)
                {
                    ObjectParameter commaString = new ObjectParameter("CommaString", typeof(string));
                    dbContext.Sp_LatterPrintTemp(Objdeptt.rid.ToString(), Constants.BulkAllotmentLetterTemplateID, Objdeptt.departmentId.Value, userInfo.UserID.ToString(), null, commaString);
                    strLettter = commaString.Value.ToString();
                }
            }
            return strLettter;
        }

        //Added on 23 April 2018
        //To view Letter
        public string ViewletterTemplate(LetterHistory _LetterHistory)
        {
            string strLettter = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                var LetterTemplate = (from letterhistory in dbContext.Letter_History
                                      where //letterhistory.Rid == _LetterHistory.Rid &&
                                      letterhistory.Id == _LetterHistory.Id
                                      select new LetterHistory
                                      {
                                          Template_Html = letterhistory.Template_Html
                                      }).FirstOrDefault();
                strLettter = !string.IsNullOrEmpty(LetterTemplate.Template_Html.ToString()) ? LetterTemplate.Template_Html.ToString() : string.Empty;
            }
            return strLettter;
        }

        public List<SchemeAllotmentModel> SchemeListForAllotment()
        {
            int userid = userInfo.UserID;
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userid
                                      select dept.DepartmentId).ToList();

                List<SchemeAllotmentModel> schemeList = new List<SchemeAllotmentModel>();
                schemeList = (from schemes in dbContext.SchemeMsts
                              join dpt in dbContext.SchemeDepartmentTrans on schemes.schemeId equals dpt.schemeId
                              where schemes.IsActive == true && schemes.completed == true && loginUserDeptt.Contains(dpt.departmentId) && schemes.Status != Constants.SchemeClosed
                              select new SchemeAllotmentModel
                              {
                                  schemeId = schemes.schemeId,
                                  schemeName = schemes.schemeName
                              }).Distinct().OrderByDescending(x => x.schemeId).ToList();
                return schemeList;
            }

        }

        public List<DDLStringList> GetAllFormsForAllotment(int SchemeId, int DepartmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {

                //var activeAllotment = (from allot in dbContext.AllotmentMasters
                //                       where allot.schemeId == SchemeId && allot.departmentId == DepartmentId && allot.isActive == 1 || (allot.isActive == 0 && allot.isStatus == Constants.IsApproved)
                //                       select allot.applicationId.Value).ToList();

                var activeAllotment = (from allot in dbContext.AllotmentMasters
                                       join appl in dbContext.ApplicationDetails on allot.rid equals appl.registrationId
                                       where allot.isActive == 1 && appl.isAllotted == "1"
                                       select allot.applicationId.Value).ToList();

                var activeApplicationID = (from appln in dbContext.ApplicationDetails
                                           where appln.schemeId.Value == SchemeId && appln.departmentId.Value == DepartmentId
                                           select appln.applicationId).ToList();



                var lst = (from appdetails in dbContext.ApplicationDetails
                           where appdetails.schemeId.Value == SchemeId && appdetails.departmentId.Value == DepartmentId && (!activeAllotment.Contains(appdetails.applicationId))
                           select new DDLStringList
                           {
                               id = appdetails.formNo,
                               text = appdetails.formNo,
                               RecordExistsIn = appdetails.registrationId != null ? true : false
                           }).ToList();

                return lst;
            }
        }

        public DataSourceResult GetAllFormsForAllotment(DataSourceRequest Req, int? SchemeId, int? DepartmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {

                var lst = (from ApplicationDetails in dbContext.ApplicationDetails
                           where
                            (SchemeId == null || ApplicationDetails.schemeId == SchemeId) &&
                            (DepartmentId == null || ApplicationDetails.departmentId == DepartmentId) &&
                             !
                               (from AllotmentMaster in dbContext.AllotmentMasters
                                where
                                  (SchemeId == null || AllotmentMaster.schemeId == SchemeId) &&
                                  (DepartmentId == null || AllotmentMaster.departmentId == DepartmentId) &&
                                  AllotmentMaster.isActive == 1 || (AllotmentMaster.isActive == 0 && AllotmentMaster.isStatus == Constants.IsApproved)
                                  && AllotmentMaster.applicationId != null
                                select new
                                {
                                    AllotmentMaster.applicationId
                                }).Contains(new { applicationId = (System.Int32?)ApplicationDetails.applicationId })
                           select new FormAllotmentList
                           {
                               id = ApplicationDetails.formNo,
                               text = ApplicationDetails.formNo,
                               RecordExistsIn = ApplicationDetails.registrationId != null ? true : false,
                               DepartmentId = ApplicationDetails.departmentId != null ? (int)ApplicationDetails.departmentId : 0
                           }).AsQueryable();
                //Req.Filters.RemoveAt(0);
                return lst.ToDataSourceResult(Req);
            }
        }

        //To Check Allotment Date
        public bool CheckAllotmentDate(int schemeId, int departmentId, DateTime allotmentDate)
        {
            var bflag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                //var result = from allot in dbContext.AllotmentMasters where allot.isActive == 1 && allot.isStatus == Constants.IsApproved && allot.schemeId == schemeId && allot.departmentId == departmentId && allot.allotmentDate == allotmentDate select true;
                var result = dbContext.AllotmentMasters.Where(allot => allot.isActive == 1 && allot.isStatus == Constants.IsApproved && allot.schemeId == schemeId && allot.departmentId == departmentId && allot.allotmentDate == allotmentDate).FirstOrDefault();
                if (result != null)
                    bflag = true;
            }
            return bflag;
        }

        public List<DDLStringList> GetAllFormsForView(int SchemeId, int DepartmentId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {

                var lst = (from appdetails in dbContext.ApplicationDetails
                           where appdetails.schemeId.Value == SchemeId && appdetails.departmentId.Value == DepartmentId
                           select new DDLStringList
                           {
                               id = appdetails.formNo,
                               text = appdetails.formNo,
                               RecordExistsIn = appdetails.registrationId != null ? true : false
                           }).ToList();

                return lst;
            }
        }

        public OnlineApplicationFormModel OnlineApplicationRequest(OnlineApplicationFormModel applicationFormModel)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var obj = new OnlineApplicationFormModel();
                if (applicationFormModel != null)
                {
                    var lastFormnu = 1;//(from app in dbContext.OnlineApplicationDetails.OrderByDescending(m => m.onlineapplicationId) select app.formNo).FirstOrDefault();
                    var nextFormnu = Convert.ToString(Convert.ToInt32(lastFormnu) + 1);
                    var form = new OnlineApplicationDetail()
                    {
                        schemeId = applicationFormModel.SchemeId,
                        departmentId = applicationFormModel.DepartmentId,
                        //gender = applicationFormModel.GenderName,
                        gender = applicationFormModel.IsCompany == true ? Constants.genderCompany : applicationFormModel.GenderName,
                        // formNo = nextFormnu,
                        //company
                        CompanyName = applicationFormModel.CompanyName,
                        CompanyType = applicationFormModel.CompanySelType,
                        firstName = applicationFormModel.FirstName,
                        middleName = applicationFormModel.MiddleName,
                        lastName = applicationFormModel.LastName,
                        fatherHusbandName = applicationFormModel.FatherName,
                        motherName = applicationFormModel.MotherName,
                        email = applicationFormModel.Email,
                        signingAuthority = applicationFormModel.SigningAuthority,
                        registeredOffice = applicationFormModel.RegisteredOffice,
                        correspondanceAdd = applicationFormModel.CorrespondingAddress,
                        permanentAdd = applicationFormModel.PermanentAddress,
                        mobileNumberP1 = applicationFormModel.MobileNumber,
                        mobileNumberP2 = applicationFormModel.MobileNumberP1,
                        phoneNumberP1 = applicationFormModel.PhoneNumber,
                        phoneNumberP2 = applicationFormModel.PhoneNumberP1,
                        faxNumberP1 = applicationFormModel.FaxNumber,
                        faxNumberP2 = applicationFormModel.FaxNumberP1,
                        occupationId = applicationFormModel.OccupationId,
                        quotaId = applicationFormModel.CategoryId,
                        religionId = applicationFormModel.ReligionId,
                        pan = applicationFormModel.PanNumber,
                        createdBy = userInfo.UserID.ToString(),
                        createdDate = DateTime.Now,
                        annualIncome = applicationFormModel.AnnualIncome,
                        dateOfBirth = applicationFormModel.DOB,
                        ApplicationFee = applicationFormModel.ApplicationFee,
                        EarnestMoney = applicationFormModel.earnestMoney,
                        ApplicationDate = DateTime.Now,
                        marritalStatus = applicationFormModel.MaritialStatusName,
                        RefundBankId = applicationFormModel.RefundBankId,
                        RefundInfaverof = applicationFormModel.RefundInfaverof,
                        RefundaccountNo = applicationFormModel.RefundaccountNo,
                        area = applicationFormModel.Area,
                        ProcessingCharge = applicationFormModel.ProcessingFee,
                        TotalAmount = applicationFormModel.TotalAmount,
                        isActive = true
                    };
                    dbContext.OnlineApplicationDetails.Add(form);

                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors_loopVariable in dbEx.EntityValidationErrors)
                        {
                            var validationErrors = validationErrors_loopVariable;
                            foreach (var validationError_loopVariable in validationErrors.ValidationErrors)
                            {
                                var validationError = validationError_loopVariable;
                                string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }
                    obj = applicationFormModel;
                    obj.ApplicationId = form.onlineapplicationId;
                    obj.flag = true;
                }
                else
                {
                    obj.flag = false;
                }
                return obj;
            }
        }

        public List<ChecklistDocuments> GetApplicationChcklstDocuments(DataSourceRequest request, int applicationId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var allDocs = new List<ChecklistDocuments>();
                int i = 0;
                var schemeID = (from olAppDet in dbContext.OnlineApplicationDetails where olAppDet.onlineapplicationId == applicationId select olAppDet.schemeId).FirstOrDefault();
                var docs = (from doc in dbContext.OnlineCheckListMasters
                            where doc.IsActive == true && doc.SchemeId == schemeID
                            select new ChecklistDocuments
                            {
                                Id = doc.CheckListId,
                                DocName = doc.CheckListName
                            }).ToList();
                if (docs.Count > 0)
                {
                    allDocs = (from d in docs select new ChecklistDocuments { Id = d.Id, DocName = d.DocName, SNo = ++i }).ToList();
                }
                //For reading Update screen load
                var checklist = (from chk in dbContext.OnlineCheckLisTrans where chk.onlineapplicationId == applicationId && chk.isActive == true select chk.CheckListId).ToList();
                if (checklist != null && checklist.Any())
                {
                    foreach (var doc in allDocs)
                    {
                        if (checklist.Contains(doc.Id))
                        {
                            doc.IsSelected = true;
                        }
                    }
                }
                return allDocs;
            }
        }

        public bool SaveFileDetailsToDB(UploadDetails details)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var olAppdetails = (from oad in dbContext.OnlineApplicationDetails where oad.onlineapplicationId == details.ApplicationID select oad).FirstOrDefault();
                if (olAppdetails != null)
                {
                    olAppdetails.Photographfilename = (!string.IsNullOrEmpty(details.filePhoto)) ? details.filePhoto : olAppdetails.Photographfilename;
                    olAppdetails.Signaturefilename = (!string.IsNullOrEmpty(details.fileSign)) ? details.fileSign : olAppdetails.Signaturefilename;
                    olAppdetails.Documentfilename = (!string.IsNullOrEmpty(details.fileDoc)) ? details.fileDoc : olAppdetails.Documentfilename;
                    olAppdetails.modifiedDate = DateTime.Now;
                }
                var existingChcklist = (from oct in dbContext.OnlineCheckLisTrans where oct.onlineapplicationId == details.ApplicationID select oct).ToList();
                if (existingChcklist != null && existingChcklist.Any())
                {
                    foreach (var item in existingChcklist)
                    {
                        dbContext.OnlineCheckLisTrans.Remove(item);
                    }
                }
                var newChcklist = details.IDLst.Split(',').ToList();
                foreach (var it in newChcklist)
                {
                    var newChk = new OnlineCheckLisTran();
                    newChk.onlineapplicationId = details.ApplicationID;
                    newChk.CheckListId = Convert.ToInt32(it);
                    newChk.isActive = true;
                    newChk.CreatedDate = DateTime.Now;
                    dbContext.OnlineCheckLisTrans.Add(newChk);
                }
                dbContext.SaveChanges();
                //on mobile
                //var msg = "Dear User, Your application no " + olAppdetails.onlineapplicationId + " submitted successfully. Kindly check the document list on mynoida.in Regards, http://mynoida.in";
                var msg = string.Format(NAMessages.OfflineAppReqSuccess, olAppdetails.onlineapplicationId);
                var mobNumber = olAppdetails.mobileNumberP1;
                //CommonMethords.SMSSend(mobNumber, msg);
                ApplicationHelper.SendSMS(mobNumber, msg);
                //Email
                if (olAppdetails.email != null)
                {
                    var body = "Dear User, Your application no " + olAppdetails.onlineapplicationId + " submitted successfully. Kindly check the document list on mynoida.in Regards, http://mynoida.in";
                    EmailHelper emailHelper = new EmailHelper();
                    emailHelper.Send(olAppdetails.email, "Request submitted", body);
                }

                flag = true;
            }
            return flag;
        }

        public UploadDetails GetFileDetailsFromDB(int applicationId)
        {
            var details = new UploadDetails();
            if (applicationId != 0)
                using (var dbContext = new NoidaPMSEntities())
                {
                    var applicationDetails = (from oad in dbContext.OnlineApplicationDetails where oad.onlineapplicationId == applicationId select oad).FirstOrDefault();
                    if (applicationDetails != null)
                    {
                        details.fileDoc = applicationDetails.Documentfilename;
                        details.filePhoto = applicationDetails.Photographfilename;
                        details.fileSign = applicationDetails.Signaturefilename;
                    }
                    var checklist = (from chk in dbContext.OnlineCheckLisTrans where chk.onlineapplicationId == applicationId && chk.isActive == true select chk.CheckListId).ToList();
                    if (checklist != null && checklist.Any())
                    {
                        details.Checklist = checklist;
                    }
                }
            return details;
        }

        public OnlineApplicationFormModel ViewOnlineDetails(int onlineappId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var path = ConfigurationManager.AppSettings["ApplicationDetailsPath"].ToString();
                var lst = (from applicationFormModel in dbContext.OnlineApplicationDetails
                           join schemeMst in dbContext.SchemeMsts on applicationFormModel.schemeId equals schemeMst.schemeId
                           join department in dbContext.DepartmentMsts on applicationFormModel.departmentId equals department.departmentId
                           join floor in dbContext.FloorMsts on applicationFormModel.area equals floor.floorId.ToString()
                           where applicationFormModel.onlineapplicationId == onlineappId
                           select new OnlineApplicationFormModel
                           {
                               SchemeId = applicationFormModel.schemeId.Value,
                               SchemeName = schemeMst.schemeName,
                               DepartmentId = applicationFormModel.departmentId.Value,
                               Department = department.departmentName,
                               GenderName = applicationFormModel.gender,
                               IsCompany = applicationFormModel.gender == Constants.genderCompany ? true : false,
                               // FormNo = applicationFormModel.formNo,
                               MaritialStatusName = applicationFormModel.marritalStatus,
                               CategoryId = applicationFormModel.quotaId,
                               ApplicationId = applicationFormModel.onlineapplicationId,
                               CompanyName = applicationFormModel.CompanyName,
                               CompanySelType = applicationFormModel.CompanyType,
                               FirstName = applicationFormModel.firstName,
                               MiddleName = applicationFormModel.middleName,
                               LastName = applicationFormModel.lastName,
                               FatherName = applicationFormModel.fatherHusbandName,
                               MotherName = applicationFormModel.motherName,
                               Email = applicationFormModel.email,
                               SigningAuthority = applicationFormModel.signingAuthority,
                               RegisteredOffice = applicationFormModel.registeredOffice,
                               CorrespondingAddress = applicationFormModel.correspondanceAdd,
                               PermanentAddress = applicationFormModel.permanentAdd,
                               MobileNumber = applicationFormModel.mobileNumberP1,
                               MobileNumberP1 = applicationFormModel.mobileNumberP2,
                               PhoneNumber = applicationFormModel.phoneNumberP1,
                               PhoneNumberP1 = applicationFormModel.phoneNumberP2,
                               FaxNumber = applicationFormModel.faxNumberP1,
                               FaxNumberP1 = applicationFormModel.faxNumberP2,
                               OccupationId = applicationFormModel.occupationId,
                               ReligionId = applicationFormModel.religionId,
                               PanNumber = applicationFormModel.pan,
                               AnnualIncome = applicationFormModel.annualIncome,
                               DOB = applicationFormModel.dateOfBirth,
                               ApplicationFee = applicationFormModel.ApplicationFee,
                               earnestMoney = applicationFormModel.EarnestMoney,
                               ApplicationDate = applicationFormModel.ApplicationDate,
                               TotalAmount = applicationFormModel.TotalAmount,
                               ApplicantName = applicationFormModel.firstName + " " + applicationFormModel.middleName + " " + applicationFormModel.lastName,
                               RefundBankId = applicationFormModel.RefundBankId,
                               RefundInfaverof = applicationFormModel.RefundInfaverof,
                               RefundaccountNo = applicationFormModel.RefundaccountNo,
                               Area = applicationFormModel.area,
                               AreaRange = floor.floorName,
                               ProcessingFee = applicationFormModel.ProcessingCharge,
                               FileDetails = new UploadDetails
                               {
                                   //filePhoto = applicationFormModel.Photographfilename,
                                   filePhoto = (path + "/" + onlineappId + "/" + applicationFormModel.Photographfilename),
                                   fileSign = (path + "/" + onlineappId + "/" + applicationFormModel.Signaturefilename),
                                   fileDoc = (path + "/" + onlineappId + "/" + applicationFormModel.Documentfilename)
                               },
                               Status = applicationFormModel.StatusCode.ToString()
                           }).FirstOrDefault();
                return lst;
            }
        }

        //Edit Online application Basic info
        public OnlineApplicationFormModel EditOnlineDetails(OnlineApplicationFormModel applicationFormModel)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var objresult = dbContext.OnlineApplicationDetails.FirstOrDefault(id => id.onlineapplicationId == applicationFormModel.ApplicationId);
                if (objresult != null)
                {
                    objresult.schemeId = applicationFormModel.SchemeId;
                    objresult.departmentId = applicationFormModel.DepartmentId;
                    objresult.gender = applicationFormModel.IsCompany == true ? Constants.genderCompany : applicationFormModel.GenderName;
                    objresult.CompanyName = applicationFormModel.CompanyName;
                    objresult.CompanyType = applicationFormModel.CompanySelType;
                    //IsCompany = applicationFormModel.GenderName == Constants.genderCompany ? true : false,
                    objresult.firstName = applicationFormModel.FirstName;
                    objresult.middleName = applicationFormModel.MiddleName;
                    objresult.lastName = applicationFormModel.LastName;
                    objresult.fatherHusbandName = applicationFormModel.FatherName;
                    objresult.motherName = applicationFormModel.MotherName;
                    objresult.email = applicationFormModel.Email;
                    objresult.signingAuthority = applicationFormModel.SigningAuthority;
                    objresult.registeredOffice = applicationFormModel.RegisteredOffice;
                    objresult.correspondanceAdd = applicationFormModel.CorrespondingAddress;
                    objresult.permanentAdd = applicationFormModel.PermanentAddress;
                    objresult.mobileNumberP1 = applicationFormModel.MobileNumber;
                    objresult.mobileNumberP2 = applicationFormModel.MobileNumberP1;
                    objresult.phoneNumberP1 = applicationFormModel.PhoneNumber;
                    objresult.phoneNumberP2 = applicationFormModel.PhoneNumberP1;
                    objresult.faxNumberP1 = applicationFormModel.FaxNumber;
                    objresult.faxNumberP2 = applicationFormModel.FaxNumberP1;
                    objresult.occupationId = applicationFormModel.OccupationId;
                    objresult.quotaId = applicationFormModel.CategoryId;
                    objresult.religionId = applicationFormModel.ReligionId;
                    objresult.pan = applicationFormModel.PanNumber;
                    objresult.ApplicationFee = applicationFormModel.ApplicationFee;
                    objresult.EarnestMoney = applicationFormModel.earnestMoney;
                    objresult.annualIncome = applicationFormModel.AnnualIncome;
                    objresult.dateOfBirth = applicationFormModel.DOB;
                    objresult.ApplicationDate = applicationFormModel.ApplicationDate;
                    objresult.marritalStatus = applicationFormModel.MaritialStatusName;
                    objresult.modifiedBy = userInfo.UserID.ToString();
                    objresult.modifiedDate = DateTime.Now;
                    objresult.RefundBankId = applicationFormModel.RefundBankId;
                    objresult.RefundInfaverof = applicationFormModel.RefundInfaverof;
                    objresult.RefundaccountNo = applicationFormModel.RefundaccountNo;
                    objresult.area = applicationFormModel.Area;
                    objresult.ProcessingCharge = applicationFormModel.ProcessingFee;
                    objresult.TotalAmount = applicationFormModel.TotalAmount;
                }
                dbContext.SaveChanges();
                OnlineApplicationFormModel onlineApplicationFormModel = ViewOnlineDetails(applicationFormModel.ApplicationId.Value);
                return onlineApplicationFormModel;
            }
        }
        //For online scheme selection
        public List<SchemeAllotmentModel> GetSchemeListForAllotment()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<SchemeAllotmentModel> schemeList = new List<SchemeAllotmentModel>();
                schemeList = (from schemes in dbContext.SchemeMsts
                              join dpt in dbContext.OnlineAreawithRegisandProcfees on schemes.schemeId equals dpt.schemeId
                              where schemes.IsActive == true && dpt.isActive == true && schemes.completed == true && schemes.Status != Constants.SchemeClosed
                              select new SchemeAllotmentModel
                              {
                                  schemeId = schemes.schemeId,
                                  schemeName = schemes.schemeName
                              }).Distinct().OrderByDescending(x => x.schemeId).ToList();
                return schemeList;
            }
        }
        //For online department selection
        public List<DepartmentAllotmentModel> FilterDepartmentOnScheme(int SchemeId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<DepartmentAllotmentModel> departmentList = new List<DepartmentAllotmentModel>();
                departmentList = (from dept in dbContext.DepartmentMsts
                                  join sdt in dbContext.SchemeDepartmentTrans on dept.departmentId equals sdt.departmentId
                                  where sdt.schemeId == SchemeId
                                  select new DepartmentAllotmentModel
                                  {
                                      DepartmentId = dept.departmentId,
                                      DepartmentName = dept.departmentName
                                  }).ToList();
                return departmentList;
            }
        }

        public List<DDList> GetAllBanksforOnline()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from f in dbContext.BankMsts
                           where f.IsActive == true
                           select new DDList
                           {
                               id = f.bankId,
                               text = f.bankName.ToString()
                           }).ToList();
                return lst;
            }
        }

        public OnlineApplicationFormModel GetAreaDetails(int id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from f in dbContext.OnlineAreawithRegisandProcfees
                           where f.isActive == true && f.schemeId == id
                           select new OnlineApplicationFormModel
                           {
                               ProcessingFee = f.ProcessingCharge,
                               ApplicationFee = f.ApplicationFee
                           }).FirstOrDefault();
                return lst;
            }
        }

        public OnlineApplicationFormModel GetEarneshMoney(int id, int deptt, int floor)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from f in dbContext.SchemeCostTrans
                           where f.IsActive == true && f.schemeId == id && f.departmentId == deptt && f.floorId == floor
                           select new OnlineApplicationFormModel
                           {
                               earnestMoney = f.earnestMoney
                           }).FirstOrDefault();
                return lst;
            }
        }

        public List<DDList> GetFloors(int schemeId, int depttID)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from f in dbContext.FloorMsts
                           join sdt in dbContext.SchemeCostTrans on f.floorId equals sdt.floorId
                           where sdt.IsActive == true && sdt.schemeId == schemeId && sdt.departmentId == depttID
                           select new DDList
                           {
                               id = f.floorId,
                               text = f.floorName
                           }).ToList();
                return lst;
            }
        }

        public OnlineApplicationDetailsTrans SaveTrasOnlineDetails(int applicationId)
        {
            var form = new OnlineApplicationDetails_trans();
            var lst = new OnlineApplicationDetailsTrans();
            using (var dbContext = new NoidaPMSEntities())
            {

                var resultObj = dbContext.OnlineApplicationDetails.Where(model => model.onlineapplicationId == applicationId).FirstOrDefault();
                //var existingLatestReq = (from oadt in dbContext.OnlineApplicationDetails_trans where oadt.onlineapplicationId == applicationId select oadt.txnid).OrderByDescending(x => x.)
                var existingLatestReq = dbContext.OnlineApplicationDetails_trans.Where(oadt => oadt.ServiceRefId == applicationId).OrderByDescending(x => x.AutoID).Select(x => x.txnid).FirstOrDefault();
                if (resultObj != null)
                {

                    if (existingLatestReq != null)
                    {
                        var temp = existingLatestReq.Split('-').Last();
                        int newTxId = Convert.ToInt32(temp) + 1;
                        form = new OnlineApplicationDetails_trans()
                        {
                            ServiceRefId = resultObj.onlineapplicationId,
                            txnid = resultObj.onlineapplicationId.ToString() + "-" + newTxId.ToString(),
                            Amount = resultObj.TotalAmount,
                            productinfo = "Online",
                            mode = "",
                            ServiceType = Constants.OnlineApplicationPayment,
                            GetwayName = "IND",
                            status = 0,
                            EntryDate = DateTime.Now
                        };
                    }
                    else
                    {
                        form = new OnlineApplicationDetails_trans()
                        {
                            ServiceRefId = resultObj.onlineapplicationId,
                            txnid = resultObj.onlineapplicationId.ToString() + "-1",
                            Amount = resultObj.TotalAmount,
                            productinfo = "Online",
                            mode = "Online",
                            ServiceType = Constants.OnlineApplicationPayment,
                            GetwayName = "IND",
                            status = 0,
                            EntryDate = DateTime.Now
                        };
                    }
                    dbContext.OnlineApplicationDetails_trans.Add(form);
                    dbContext.SaveChanges();

                    lst = (from applicationFormModel in dbContext.OnlineApplicationDetails_trans
                           where applicationFormModel.txnid == form.txnid
                           select new OnlineApplicationDetailsTrans
                           {
                               OnlineApplicationId = applicationFormModel.ServiceRefId,
                               TrKey = applicationFormModel.TrKey,
                               Txnid = applicationFormModel.txnid,
                               Amount = applicationFormModel.Amount,
                               Productinfo = applicationFormModel.productinfo,
                               Mode = applicationFormModel.mode,
                               TranStatus = applicationFormModel.TranStatus,
                               Discount = applicationFormModel.discount,
                               Status = applicationFormModel.status,
                               EntryDate = applicationFormModel.EntryDate,
                               FirstName = resultObj.firstName,
                               Email = resultObj.email,
                               PhoneNumber = resultObj.mobileNumberP1
                           }).FirstOrDefault();
                }
                return lst;
            }
        }

        public OnlineApplicationDetailsTrans GetTrasactionDetails(string valtxnId, string paymenttype)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = new OnlineApplicationDetailsTrans();
                if (paymenttype == Constants.strOther)
                {
                    var rid = Convert.ToInt32(valtxnId.Split('-')[1]);

                    var alloteeBasicDetails = dbContext.ApplicationDetails.Where(model => model.registrationId == rid).FirstOrDefault();
                    var customerName = alloteeBasicDetails.tFirstName + " " + alloteeBasicDetails.tMiddleName + "" + alloteeBasicDetails.tLastName;
                    var cutomerEmail = alloteeBasicDetails.tEmail;
                    var customerPhone = alloteeBasicDetails.mobileNumberP2;

                    lst = (from applicationFormModel in dbContext.OnlineApplicationDetails_trans
                           where applicationFormModel.txnid == valtxnId
                           select new OnlineApplicationDetailsTrans
                           {
                               OnlineApplicationId = applicationFormModel.ServiceRefId,
                               TrKey = applicationFormModel.TrKey,
                               Txnid = applicationFormModel.txnid,
                               Amount = applicationFormModel.Amount,
                               Productinfo = applicationFormModel.productinfo,
                               Mode = applicationFormModel.mode,
                               TranStatus = applicationFormModel.TranStatus,
                               Discount = applicationFormModel.discount,
                               Status = applicationFormModel.status,
                               EntryDate = applicationFormModel.EntryDate,
                               FirstName = customerName,
                               Email = cutomerEmail,
                               PhoneNumber = customerPhone,
                               StatusName = applicationFormModel.TranStatus == Constants.Success ? "Success" : "Failure",
                               payment_source = applicationFormModel.payment_source,
                               bank_ref_num = applicationFormModel.bank_ref_num,
                               bankcode = applicationFormModel.bankcode,
                               error = applicationFormModel.error,
                               error_Message = applicationFormModel.error_Message,
                               name_on_card = applicationFormModel.name_on_card,
                               cardnum = applicationFormModel.cardnum,
                               cardhash = applicationFormModel.cardhash,
                               issuing_bank = applicationFormModel.issuing_bank,
                               card_type = applicationFormModel.card_type,
                               Mihpayid = applicationFormModel.mihpayid
                           }).FirstOrDefault();
                }
                else
                {
                    lst = (from applicationFormModel in dbContext.OnlineApplicationDetails_trans
                           join details in dbContext.OnlineApplicationDetails on applicationFormModel.ServiceRefId equals details.onlineapplicationId
                           where applicationFormModel.txnid == valtxnId
                           select new OnlineApplicationDetailsTrans
                           {
                               OnlineApplicationId = applicationFormModel.ServiceRefId,
                               TrKey = applicationFormModel.TrKey,
                               Txnid = applicationFormModel.txnid,
                               Amount = applicationFormModel.Amount,
                               Productinfo = applicationFormModel.productinfo,
                               Mode = applicationFormModel.mode,
                               TranStatus = applicationFormModel.TranStatus,
                               Discount = applicationFormModel.discount,
                               Status = applicationFormModel.status,
                               EntryDate = applicationFormModel.EntryDate,
                               FirstName = details.gender == Constants.genderCompany ? details.CompanyName : details.firstName + " " + details.middleName + "" + details.lastName,
                               Email = details.email,
                               PhoneNumber = details.mobileNumberP1,
                               StatusName = applicationFormModel.TranStatus == Constants.Success ? "Success" : "Failure",
                               payment_source = applicationFormModel.payment_source,
                               bank_ref_num = applicationFormModel.bank_ref_num,
                               bankcode = applicationFormModel.bankcode,
                               error = applicationFormModel.error,
                               error_Message = applicationFormModel.error_Message,
                               name_on_card = applicationFormModel.name_on_card,
                               cardnum = applicationFormModel.cardnum,
                               cardhash = applicationFormModel.cardhash,
                               issuing_bank = applicationFormModel.issuing_bank,
                               card_type = applicationFormModel.card_type,
                               Mihpayid = applicationFormModel.mihpayid
                           }).FirstOrDefault();
                }


                return lst;
            }
        }

        public OnlineApplicationDetailsTrans UpdateTrasactionDetails(FormCollection form)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var id = form["udf1"];
                var paymenttype = form["udf2"];

                var onlineApplicationDetailsTrans = new OnlineApplicationDetailsTrans();
                var objresult = dbContext.OnlineApplicationDetails_trans.Where(m => m.txnid == id).FirstOrDefault();
                var flag = false;
                if (objresult.TranStatus != 1 && objresult.status != 1)
                {
                    if (paymenttype == Constants.strOther)
                    {
                        if (objresult != null)
                        {
                            if (form["status"].ToString() == "success")
                            {
                                objresult.TranStatus = Constants.Success;
                            }
                            else
                            {
                                objresult.TranStatus = Constants.Faliure;
                                objresult.error = form["error"];
                                objresult.error_Message = form["error_Message"];
                            }
                            objresult.TrKey = form["txnid"].ToString();
                            objresult.status = 1;
                            objresult.mode = form["mode"].ToString();
                            objresult.mihpayid = form["mihpayid"];
                            objresult.productinfo = form["productinfo"];
                            objresult.EntryDate = DateTime.Now;
                            objresult.payment_source = form["payment_source"];
                            objresult.PG_Type = form["PG_Type"];
                            objresult.bank_ref_num = form["bank_ref_num"];
                            objresult.bankcode = form["bankcode"];
                            objresult.name_on_card = form["name_on_card"];
                            objresult.cardnum = form["cardnum"];
                            //objresult.cardhash = form["cardhash"];
                            objresult.issuing_bank = form["issuing_bank"];
                            objresult.card_type = form["card_type"];
                            dbContext.SaveChanges();
                            onlineApplicationDetailsTrans = GetTrasactionDetails(id, paymenttype);
                        }
                    }
                    else
                    {
                        if (objresult != null)
                        {
                            if (form["status"].ToString() == "success")
                            {
                                objresult.TranStatus = Constants.Success;
                            }
                            else
                            {
                                objresult.TranStatus = Constants.Faliure;
                                objresult.error = form["error"];
                                objresult.error_Message = form["error_Message"];
                            }
                            objresult.TrKey = form["txnid"].ToString();
                            objresult.status = 1;
                            objresult.mode = form["mode"].ToString();
                            objresult.mihpayid = form["mihpayid"];
                            objresult.productinfo = form["productinfo"];
                            objresult.EntryDate = DateTime.Now;
                            objresult.payment_source = form["payment_source"];
                            objresult.PG_Type = form["PG_Type"];
                            objresult.bank_ref_num = form["bank_ref_num"];
                            objresult.bankcode = form["bankcode"];
                            objresult.name_on_card = form["name_on_card"];
                            objresult.cardnum = form["cardnum"];
                            //objresult.cardhash = form["cardhash"];
                            objresult.issuing_bank = form["issuing_bank"];
                            objresult.card_type = form["card_type"];
                            dbContext.SaveChanges();
                            onlineApplicationDetailsTrans = GetTrasactionDetails(id, paymenttype);
                        }
                    }
                }
                else
                {
                    onlineApplicationDetailsTrans.Status = 4;
                }
                return onlineApplicationDetailsTrans;
            }
        }

        public DataSourceResult GetOnlineApplications(DataSourceRequest req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstonlineApplicationDetails = (from oam in dbContext.OnlineApplicationDetails
                                                   join sch in dbContext.SchemeMsts on oam.schemeId equals sch.schemeId
                                                   join deptt in dbContext.DepartmentMsts on oam.departmentId equals deptt.departmentId
                                                   where oam.isActive == true
                                                   select new OnlineApplicationFormModel
                                                   {
                                                       SchemeName = sch.schemeName,
                                                       Department = deptt.departmentName,
                                                       ApplicationId = oam.onlineapplicationId,
                                                       FirstName = oam.CompanyName == null ? oam.firstName + " " + oam.middleName + " " + oam.lastName : oam.CompanyName,
                                                       GenderName = oam.gender,
                                                       DOB = oam.dateOfBirth,
                                                       Status = oam.StatusCode == null ? "" : (oam.StatusCode == 1 ? Status.Accepted : Status.Rejected)
                                                   });
                return lstonlineApplicationDetails.ToDataSourceResult(req);
            }
        }

        public bool SaveCommentByApplicationID(int requestNo, string Comment, bool acceptReject)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var flag = false;
                var result = dbContext.OnlineApplicationDetails.FirstOrDefault(m => m.onlineapplicationId == requestNo);
                if (result != null)
                {
                    result.Comment = Comment;
                    result.CommentDate = DateTime.Now;
                    result.ApproveDate = DateTime.Now;
                    result.StatusCode = acceptReject == true ? Constants.Approved : Constants.RejectedProp;
                    dbContext.SaveChanges();

                    flag = true;
                }
                return true;
            }
        }

        public bool SaveCommentByAppID(int requestNo, string Comment, bool acceptReject, string MobNo, string EmailId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var flag = false;
                var result = dbContext.OnlineApplicationDetails.FirstOrDefault(m => m.onlineapplicationId == requestNo);
                if (result != null)
                {
                    result.Comment = Comment;
                    result.CommentDate = DateTime.Now;
                    result.ApproveDate = DateTime.Now;
                    result.StatusCode = acceptReject == true ? Constants.Approved : Constants.RejectedProp;
                    dbContext.SaveChanges();
                    if (acceptReject == true)
                    {
                        var form = new ApplicationDetail();
                        form.schemeId = result.schemeId;
                        form.departmentId = result.departmentId;
                        form.formNo = result.onlineapplicationId.ToString();
                        form.gender = result.gender;
                        if (result.gender.ToLower() == "company")
                        {
                            form.firstName = result.CompanyName;
                            form.tFirstName = result.CompanyName;
                            form.T_Company_Name = result.CompanyName;
                        }
                        else
                        {
                            form.firstName = result.firstName;
                            form.lastName = result.lastName;
                            form.tFirstName = result.firstName;
                            form.tLastName = result.lastName;
                        }
                        form.middleName = result.middleName;
                        if (result.marritalStatus != null)
                            form.marritalStatus = result.marritalStatus.ToLower();
                        form.fatherHusbandName = result.fatherHusbandName;
                        form.motherName = result.motherName;
                        form.email = result.email;
                        form.signingAuthority = result.signingAuthority;
                        form.registeredOffice = result.registeredOffice;
                        form.correspondanceAdd = result.correspondanceAdd;
                        form.permanentAdd = result.permanentAdd;
                        form.mobileNumberP1 = result.mobileNumberP1;
                        form.mobileNumberP2 = result.mobileNumberP1;
                        form.phoneNumberP1 = result.phoneNumberP1;
                        form.phoneNumberP2 = result.phoneNumberP2;
                        form.faxNumberP1 = result.faxNumberP1;
                        form.faxNumberP2 = result.faxNumberP2;
                        form.occupationId = result.occupationId;
                        form.quotaId = result.quotaId;
                        form.religionId = result.religionId;
                        form.pan = result.pan;
                        form.createdBy = userInfo.UserID.ToString();
                        form.createdDate = DateTime.Now;
                        form.annualIncome = result.annualIncome;
                        form.dateOfBirth = result.dateOfBirth;

                        // Entries for transfer Detail
                        //form.tFirstName = formModel.FirstName;
                        form.tMiddleName = result.middleName;
                        //form.tLastName = formModel.LastName;
                        form.tGender = result.gender;
                        if (result.marritalStatus != null)
                            form.tMarritalStatus = result.marritalStatus.ToLower();
                        form.tFatherHusbandName = result.fatherHusbandName;
                        form.tMotherName = result.motherName;
                        form.tDateOfBirth = result.dateOfBirth;
                        form.tSigningAuthority = result.signingAuthority;
                        form.tRegisteredOffice = result.registeredOffice;
                        form.tCorrespondanceAdd = result.correspondanceAdd;
                        form.tPermanentAdd = result.permanentAdd;
                        form.tMobileNumber = result.mobileNumberP2;
                        form.tPhoneNumber = result.phoneNumberP2;
                        form.tEmail = result.email;
                        form.tOccupationId = result.occupationId;
                        form.tPan = result.pan;
                        form.tAnnualIncome = result.annualIncome;

                        //var latInsertedRecord = dbContext.ApplicationDetails.Where(x => x.schemeId == result.schemeId && x.departmentId == result.departmentId && x.formNo == formModel.FormNo).FirstOrDefault();

                        //if (latInsertedRecord == null)
                        //{
                        dbContext.ApplicationDetails.Add(form);
                        dbContext.SaveChanges();

                        //int lastInsertedApplicationFormId = dbContext.ApplicationDetails.Max(u => u.applicationId);
                        int lastInsertedApplicationFormId = form.applicationId;
                        var payment = new ApplicationPaymentDetail();
                        payment.applicationId = lastInsertedApplicationFormId;
                        payment.paymentMode = Constants.RTGS;
                        payment.formNo = result.onlineapplicationId.ToString();
                        var paymentTrans = dbContext.OnlineApplicationDetails_trans.Where(x => x.ServiceRefId == result.onlineapplicationId && x.status == 1).FirstOrDefault();
                        if (paymentTrans != null)
                        {
                            payment.ddIssueDate = paymentTrans.EntryDate;
                            payment.amountDeposited = paymentTrans.Amount;
                            payment.bankId = 4;
                            payment.utn = paymentTrans.txnid;
                        }
                        payment.createdBy = userInfo.UserID.ToString();
                        payment.createdDate = DateTime.Now;
                        dbContext.ApplicationPaymentDetails.Add(payment);
                        dbContext.SaveChanges();
                    }
                    string mailBody = string.Empty;
                    if (EmailId != null)
                    {
                        if (acceptReject == true)
                        {
                            mailBody = "Dear User, </br></br> Your application for " + result.onlineapplicationId + " is being accepted. Kindly connect with NOIDA.</br></br>Regards,<br> http://mynoida.in";
                        }
                        else
                        {
                            mailBody = "Dear User, </br></br> Your application number of NOIDA for " + result.onlineapplicationId + " is rejected. Kindly check the status online at mynoida.in.</br></br>Regards,<br> http://mynoida.in";
                        }
                        EmailHelper emailHelper = new EmailHelper();
                        emailHelper.Send(EmailId, "Online Application", mailBody);
                    }
                    string smsBody = string.Empty;
                    if (MobNo != null)
                    {
                        if (acceptReject == true)
                        {
                            //smsBody = "Dear User, Your application for " + result.onlineapplicationId + " has been accepted. Kindly connect with NOIDA Authority. Regards, http://mynoida.in";
                            smsBody = string.Format(NAMessages.AppAccepted, result.onlineapplicationId);
                        }
                        else
                        {
                            //smsBody = "Dear User, Your application number of NOIDA for " + result.onlineapplicationId + " has been rejected. Kindly check the status online at mynoida.in. Regards, http://mynoida.in";
                            smsBody = string.Format(NAMessages.AppRejected, result.onlineapplicationId);
                        }
                        //CommonMethords.SMSSend(MobNo, smsBody);
                        ApplicationHelper.SendSMS(MobNo, smsBody);
                    }
                    flag = true;
                }
                return true;
            }
        }

        public ApplicatandDetailsModel GetDetails(int applid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lstapplicatandDetailsModel = (from allotment in dbContext.OnlineApplicationDetails
                                                  where allotment.onlineapplicationId == applid && allotment.isActive == true
                                                  select new ApplicatandDetailsModel
                                                  {
                                                      Appld = applid,
                                                      MobNu = allotment.mobileNumberP1,
                                                      EmailId = allotment.email
                                                  }
                        ).FirstOrDefault();
                return lstapplicatandDetailsModel;
            }
        }

        public OnlineApplicationDetailsTrans SaveOtherPaymentTras(int id)
        {
            var form = new OnlineApplicationDetails_trans();
            var lst = new OnlineApplicationDetailsTrans();
            using (var dbContext = new NoidaPMSEntities())
            {
                var resultObj = dbContext.Customer_ServiceRequest.Where(model => model.Id == id).FirstOrDefault();
                if (resultObj != null)
                {
                    var existingLatestReq = dbContext.OnlineApplicationDetails_trans.Where(oadt => oadt.ServiceRefId == id).OrderByDescending(x => x.AutoID).Select(x => x.txnid).FirstOrDefault();
                    if (existingLatestReq != null)
                    {
                        var temp = existingLatestReq.Split('-').Last();
                        int newTxId = Convert.ToInt32(temp) + 1;

                        form = new OnlineApplicationDetails_trans()
                        {
                            ServiceRefId = resultObj.Id,
                            txnid = resultObj.Id.ToString() + "-" + resultObj.Registration_No + "-" + newTxId.ToString(),
                            Amount = resultObj.ServiceFee + resultObj.DuesAmount,
                            productinfo = "Others",
                            ServiceType = Constants.OnlineOtherPayments,
                            mode = "",
                            GetwayName = "IND",
                            status = 0,
                            EntryDate = DateTime.Now
                        };
                    }
                    else
                    {
                        form = new OnlineApplicationDetails_trans()
                       {
                           ServiceRefId = resultObj.Id,
                           txnid = resultObj.Id.ToString() + "-" + resultObj.Registration_No + "-1",
                           Amount = resultObj.ServiceFee + resultObj.DuesAmount,
                           productinfo = "Others",
                           ServiceType = Constants.OnlineOtherPayments,
                           mode = "",
                           GetwayName = "IND",
                           status = 0,
                           EntryDate = DateTime.Now
                       };
                    }
                    dbContext.OnlineApplicationDetails_trans.Add(form);
                    dbContext.SaveChanges();
                    var rid = Convert.ToInt32(resultObj.Registration_No);
                    var alloteeBasicDetails = dbContext.ApplicationDetails.Where(model => model.registrationId == rid).FirstOrDefault();

                    if (alloteeBasicDetails != null)
                    {
                        var customerName = alloteeBasicDetails.tFirstName + " " + alloteeBasicDetails.tMiddleName + "" + alloteeBasicDetails.tLastName;
                        var cutomerEmail = alloteeBasicDetails.tEmail;
                        var customerPhone = alloteeBasicDetails.mobileNumberP2;
                        lst = (from applicationFormModel in dbContext.OnlineApplicationDetails_trans
                               where applicationFormModel.txnid == form.txnid
                               select new OnlineApplicationDetailsTrans
                               {
                                   OnlineApplicationId = applicationFormModel.ServiceRefId,
                                   TrKey = applicationFormModel.TrKey,
                                   Txnid = applicationFormModel.txnid,
                                   Amount = applicationFormModel.Amount,
                                   Productinfo = applicationFormModel.productinfo,
                                   Mode = applicationFormModel.mode,
                                   TranStatus = applicationFormModel.TranStatus,
                                   Discount = applicationFormModel.discount,
                                   Status = applicationFormModel.status,
                                   EntryDate = applicationFormModel.EntryDate,
                                   FirstName = customerName,
                                   Email = cutomerEmail,
                                   PhoneNumber = customerPhone
                               }).FirstOrDefault();
                    }
                }
                return lst;
            }
        }

        #region Update Application Address

        /// <summary>
        /// To pick RIDs
        /// </summary>
        /// <returns>List of RIDs</returns>
        public DataSourceResult GetRIDsForApplication(DataSourceRequest Req)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var loginUserDeptt = (from userMst in dbContext.UmUserMasters
                                      join dept in dbContext.UmUserDepartmentTrans on userMst.UserRefId equals dept.UserRefId
                                      where userMst.UserRefId == userInfo.UserID
                                      select dept.DepartmentId).ToList();

                var lstRId = (from ad in dbContext.ApplicationDetails
                              //join am in dbContext.AllotmentMasters on ad.Rid equals am.rid
                              where loginUserDeptt.Contains(ad.departmentId) && ad.registrationId != null
                              select new DDList
                              {
                                  id = ad.registrationId != null ? ad.registrationId.Value : 0,
                                  text = ad.registrationId != null ? ad.registrationId.ToString() : string.Empty
                              });

                return lstRId.ToDataSourceResult(Req);
            }
        }

        /// <summary>
        /// Get Application Details of applicant on the basis of RID
        /// </summary>
        /// <returns></returns>
        public UpdateAddress GetApplicationDetailsById(int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var ApplicationDetails = new UpdateAddress();
                if (rId > 0)
                {
                    ApplicationDetails = (from applicationForms in dbContext.ApplicationDetails
                                          where applicationForms.registrationId == rId
                                          select new UpdateAddress
                                          {
                                              TCorrespondenceAdd = applicationForms.tCorrespondanceAdd,
                                              TPermanentAdd = applicationForms.tPermanentAdd,
                                              Name = applicationForms.firstName + " " + (!string.IsNullOrEmpty(applicationForms.middleName) ? applicationForms.middleName : "") + " " + applicationForms.lastName,
                                              ApplicationId = applicationForms.applicationId
                                          }).FirstOrDefault();
                }
                return ApplicationDetails;
            }
        }

        /// <summary>
        /// Update Address
        /// </summary>
        /// <param name="UpdateAddress"></param>
        /// <returns></returns>
        public bool UpdateAddress(UpdateAddress objUpdateAddress)
        {
            var flag = false;
            if (!string.IsNullOrEmpty(objUpdateAddress.RID))
            {
                int Rid = Convert.ToInt32(objUpdateAddress.RID);
                using (var dbContext = new NoidaPMSEntities())
                {
                    var existingApplication = dbContext.ApplicationDetails.Where(i => i.registrationId == Rid).FirstOrDefault();
                    if (existingApplication != null)
                    {
                        existingApplication.tCorrespondanceAdd = objUpdateAddress.TCorrespondenceAdd;
                        existingApplication.tPermanentAdd = objUpdateAddress.TPermanentAdd;
                        dbContext.SaveChanges();
                        //audit trail
                        //GeneralRepository.CreateAuditTrail(Constants.Update, "ApplicationDetails", "UpdateAddress", Rid, objUpdateAddress, objUpdateAddress, userInfo.UserID.ToString());
                        flag = true;
                    }
                }
            }
            return flag;
        }
        #endregion

        #region Update Details
        public UpdateDetails GetFormDetailsById(int rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var FormDetails = new UpdateDetails();
                if (rId > 0)
                {
                    var LeaseDeedDate = (from regd in dbContext.RegistryDetails
                                         where regd.Rid == rId
                                         select new UpdateDetails
                                         {
                                             LeaseDeedDate = regd.RegistryDoneDate
                                         }).FirstOrDefault();

                    var AllotteName = (from AppDet in dbContext.ApplicationDetails
                                       where AppDet.registrationId == rId
                                       select new UpdateDetails
                                       {
                                           FirstName = AppDet.tFirstName,
                                           MiddleName = AppDet.tMiddleName,
                                           LastName = AppDet.tLastName,
                                       }).FirstOrDefault();

                    var AllotmentMst = (from allotment in dbContext.AllotmentMasters
                                        join prodetail in dbContext.SchemePropTrans on allotment.propertyId equals prodetail.propertyId
                                        where allotment.rid == rId
                                        select prodetail).FirstOrDefault();

                    var PossessionDetails = (from possDet in dbContext.PossessionDetails
                                             where possDet.Rid == rId
                                             select new UpdateDetails
                                             {
                                                 PossessionDate = possDet.PossessionDate
                                             }).FirstOrDefault();

                    if (AllotteName != null)
                    {
                        FormDetails.FirstName = AllotteName.FirstName;
                        FormDetails.MiddleName = AllotteName.MiddleName;
                        FormDetails.LastName = AllotteName.LastName;
                    }
                    if (LeaseDeedDate != null) { FormDetails.LeaseDeedDate = LeaseDeedDate.LeaseDeedDate; }

                    if (AllotmentMst != null)
                    {
                        FormDetails.Area = AllotmentMst.totalArea;
                        FormDetails.LandRate = AllotmentMst.landRatePerSqmt;
                        FormDetails.PropertyCost = AllotmentMst.propertyCost;
                    }

                    if (PossessionDetails != null) { FormDetails.PossessionDate = PossessionDetails.PossessionDate; }
                }
                return FormDetails;
            }
        }

        public bool UpdateFormDetails(UpdateDetails ObjUpdateDetails)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                int Rid = Convert.ToInt32(ObjUpdateDetails.RID);
                if (ObjUpdateDetails.LeaseDeedDate != null)
                {
                    var LeaseDeedDate = dbContext.RegistryDetails.Where(m => m.Rid == Rid).FirstOrDefault();
                    if (LeaseDeedDate != null) { if (LeaseDeedDate.RegistryDoneDate != ObjUpdateDetails.LeaseDeedDate) { LeaseDeedDate.RegistryDoneDate = ObjUpdateDetails.LeaseDeedDate; } }
                }

                if (!string.IsNullOrEmpty(ObjUpdateDetails.FirstName) || !string.IsNullOrEmpty(ObjUpdateDetails.MiddleName) || !string.IsNullOrEmpty(ObjUpdateDetails.LastName))
                {
                    var AllotteeName = dbContext.ApplicationDetails.Where(m => m.registrationId == Rid).FirstOrDefault();
                    if (AllotteeName != null)
                    {
                        if (!string.IsNullOrEmpty(ObjUpdateDetails.FirstName)) { if (AllotteeName.tFirstName != ObjUpdateDetails.FirstName) { AllotteeName.tFirstName = ObjUpdateDetails.FirstName; } }
                        if (!string.IsNullOrEmpty(ObjUpdateDetails.MiddleName)) { if (AllotteeName.tMiddleName != ObjUpdateDetails.MiddleName) { AllotteeName.tMiddleName = ObjUpdateDetails.MiddleName; } }
                        if (!string.IsNullOrEmpty(ObjUpdateDetails.LastName)) { if (AllotteeName.tLastName != ObjUpdateDetails.LastName) { AllotteeName.tLastName = ObjUpdateDetails.LastName; } }
                    }
                }

                if (ObjUpdateDetails.Area != null || ObjUpdateDetails.LandRate != null)
                {
                    var AllotmentMst = (from allotment in dbContext.AllotmentMasters
                                        join prodetail in dbContext.SchemePropTrans on allotment.propertyId equals prodetail.propertyId
                                        where allotment.rid == Rid
                                        select prodetail).FirstOrDefault();

                    if (ObjUpdateDetails.Area != null && AllotmentMst != null && ObjUpdateDetails.Area > 0)
                    {
                        if (AllotmentMst.totalArea != ObjUpdateDetails.Area)
                        {
                            AllotmentMst.coveredArea = ObjUpdateDetails.Area;
                            AllotmentMst.actualArea = ObjUpdateDetails.Area;
                            AllotmentMst.totalArea = ObjUpdateDetails.Area;

                            decimal? LandRate = AllotmentMst.landRatePerSqmt;
                            if (LandRate > 0)
                            {
                                decimal? PropertyCost = (LandRate * ObjUpdateDetails.Area);
                                AllotmentMst.propertyCost = PropertyCost;
                                AllotmentMst.totalPropertyCost = PropertyCost;
                            }
                        }
                    }

                    if (ObjUpdateDetails.LandRate != null && AllotmentMst != null && ObjUpdateDetails.LandRate > 0)
                    {
                        if (AllotmentMst.landRatePerSqmt != ObjUpdateDetails.LandRate)
                        {
                            AllotmentMst.landRatePerSqmt = ObjUpdateDetails.LandRate;

                            decimal? Area = AllotmentMst.totalArea;
                            if (Area > 0)
                            {
                                decimal? nPropertyCost = Area * ObjUpdateDetails.LandRate;
                                AllotmentMst.propertyCost = nPropertyCost;
                                AllotmentMst.totalPropertyCost = nPropertyCost;
                            }
                        }
                    }
                }

                if (ObjUpdateDetails.PossessionDate != null)
                {
                    var PossessionDate = dbContext.PossessionDetails.Where(m => m.Rid == Rid).FirstOrDefault();
                    if (PossessionDate != null) { if (PossessionDate.PossessionDate != ObjUpdateDetails.PossessionDate) { PossessionDate.PossessionDate = ObjUpdateDetails.PossessionDate; } }
                }

                dbContext.SaveChanges();
            }
            return flag;
        }

        #endregion

        #region NIc Service
        public List<ApplicationDetail> GetDetailsOfAllotedProprety(int schemeId, int depid, DateTime allotmentDate)
        {
            List<ApplicationDetail> objApplicationDetail = new List<ApplicationDetail>();
            using (var dbContext = new NoidaPMSEntities())
            {
                var objAllotteeLsts = dbContext.AllotmentMasters.Where(i => i.schemeId == schemeId && i.departmentId == depid && i.allotmentDate == allotmentDate).ToList();
                var allotteesFormNo = (from objAllotteeList in dbContext.SpAllotteeList(schemeId, depid, allotmentDate) select objAllotteeList.formNo).ToList();
                objApplicationDetail = (from objAllotteeList in dbContext.ApplicationDetails
                                        where objAllotteeList.schemeId == schemeId && objAllotteeList.departmentId == depid && allotteesFormNo.Contains(objAllotteeList.formNo)
                                        select objAllotteeList).ToList();
            }
            return objApplicationDetail;
        }
        #endregion


        public int PropertyAllotmentForOnlineApplicationForm(OnlineFormViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var onlineForm = dbContext.OnlineApplicationDetails.FirstOrDefault(o => o.onlineapplicationId == model.ApplicationFormId);
                var property = dbContext.SchemePropTrans.FirstOrDefault(id => id.propertyId == model.PropertyId);
                if (onlineForm != null && property != null)
                {
                    int applicationId = SaveOnlineApplicationForm(model.ApplicationFormId);
                    if (applicationId != ReturnType.None)
                    {
                        var applicant = dbContext.ApplicationDetails.FirstOrDefault(a => a.applicationId == applicationId);
                        var installmentStartDate = model.AllottmentDate.Value.AddMonths(6);
                        var exAllotment = dbContext.AllotmentMasters.FirstOrDefault(a => a.propertyId == model.PropertyId && a.isActive == NAStatusId.Active);
                        if (exAllotment == null)
                        {
                            var allotment = new AllotmentMaster
                            {
                                applicationId = applicant.applicationId,
                                schemeId = applicant.schemeId,
                                departmentId = applicant.departmentId,
                                formNo = applicant.formNo,
                                propertyId = property.propertyId.Value,
                                allotmentDate = model.AllottmentDate,
                                instalmentStartDate = installmentStartDate,
                                isStatus = Status.Approved,
                                comment = Status.Approved,
                                approverBy = userInfo.UserID.ToString(), //Constants.SvamAdmin.ToString(),
                                isSubmitted = NAStatusId.Submitted.ToString(),
                                isActive = NAStatusId.Active,
                                createdBy = userInfo.UserID.ToString(),
                                submittedBy = userInfo.UserID,
                                createdDate = DateTime.Now,
                                submitDate = DateTime.Now,
                                approveDate = DateTime.Now
                            };
                            dbContext.AllotmentMasters.Add(allotment);
                            dbContext.SaveChanges();

                            applicant.registrationId = allotment.rid;
                            applicant.isAllotted = NAStatusId.Allotted.ToString();
                            dbContext.SaveChanges();

                            flag = ReturnType.Allotted;
                        }
                        else { flag = ReturnType.NotAllotted; }
                    }
                }
            }
            return flag;
        }

        private int SaveOnlineApplicationForm(int? applicationId)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var onlineForm = dbContext.OnlineApplicationDetails.FirstOrDefault(o => o.onlineapplicationId == applicationId);
                var existingForm = dbContext.ApplicationDetails.Where(x => x.schemeId == onlineForm.schemeId && x.departmentId == onlineForm.departmentId && x.formNo == onlineForm.onlineapplicationId.ToString()).FirstOrDefault();

                if (onlineForm != null && existingForm == null)
                {
                    var form = new ApplicationDetail();
                    form.schemeId = onlineForm.schemeId;
                    form.departmentId = onlineForm.departmentId;
                    form.formNo = onlineForm.onlineapplicationId.ToString();
                    form.gender = onlineForm.gender;
                    form.tGender = onlineForm.gender;
                    if (onlineForm.gender.ToLower() == Constants.Company.ToLower())
                    {
                        form.firstName = onlineForm.CompanyName;
                        form.tFirstName = onlineForm.CompanyName;
                        form.T_Company_Name = onlineForm.CompanyName;
                        form.registeredOffice = onlineForm.registeredOffice;
                        form.signingAuthority = onlineForm.signingAuthority;
                        form.tRegisteredOffice = onlineForm.signingAuthority;
                        form.tSigningAuthority = onlineForm.signingAuthority;
                    }
                    else
                    {
                        form.firstName = onlineForm.firstName;
                        form.middleName = onlineForm.middleName;
                        form.lastName = onlineForm.lastName;
                        form.tFirstName = onlineForm.firstName;
                        form.tMiddleName = onlineForm.middleName;
                        form.tLastName = onlineForm.lastName;
                    }

                    form.fatherHusbandName = onlineForm.fatherHusbandName;
                    form.tFatherHusbandName = onlineForm.fatherHusbandName;
                    form.motherName = onlineForm.motherName;
                    form.tMotherName = onlineForm.motherName;
                    form.email = onlineForm.email;
                    form.tEmail = onlineForm.email;
                    form.correspondanceAdd = onlineForm.correspondanceAdd;
                    form.tCorrespondanceAdd = onlineForm.correspondanceAdd;
                    form.permanentAdd = onlineForm.permanentAdd;
                    form.tPermanentAdd = onlineForm.permanentAdd;
                    form.mobileNumberP2 = onlineForm.mobileNumberP2;
                    form.tMobileNumber = onlineForm.mobileNumberP2;
                    form.phoneNumberP2 = onlineForm.phoneNumberP2;
                    form.tPhoneNumber = onlineForm.phoneNumberP2;
                    form.faxNumberP1 = onlineForm.faxNumberP1;
                    form.faxNumberP2 = onlineForm.faxNumberP2;

                    form.pan = onlineForm.pan;
                    form.tPan = onlineForm.pan;
                    form.annualIncome = onlineForm.annualIncome;
                    form.tAnnualIncome = onlineForm.annualIncome;
                    form.dateOfBirth = onlineForm.dateOfBirth;
                    form.tDateOfBirth = onlineForm.dateOfBirth;
                    form.createdBy = userInfo.UserID.ToString();
                    form.createdDate = DateTime.Now;

                    dbContext.ApplicationDetails.Add(form);
                    dbContext.SaveChanges();

                    var onlinePayment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(c => c.ServiceRefId == onlineForm.onlineapplicationId && c.TranStatus == 1 && c.status == 1);
                    var exPayment = dbContext.ApplicationPaymentDetails.FirstOrDefault(x => x.applicationId == form.applicationId);
                    if (onlinePayment == null)
                    {
                        onlinePayment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(c => c.ServiceRefId == form.applicationId);
                    }

                    if (onlinePayment != null && exPayment == null)
                    {
                        SaveOnlinePaymentDetail((int)applicationId);
                    }
                    flag = form.applicationId;
                }
                else
                {
                    var onlinePayment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(c => c.ServiceRefId == onlineForm.onlineapplicationId && c.TranStatus == 1 && c.status == 1);
                    var exPayment = dbContext.ApplicationPaymentDetails.FirstOrDefault(x => x.applicationId == x.applicationId);
                    if (onlinePayment == null)
                    {
                        onlinePayment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(c => c.ServiceRefId == existingForm.applicationId);
                    }

                    if (onlinePayment != null && exPayment == null)
                    {
                        SaveOnlinePaymentDetail((int)applicationId);
                    }
                    flag = existingForm.applicationId;
                }
            }
            return flag;
        }

        private int SaveOnlinePaymentDetail(int applicationId)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var onlinePayment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(c => c.ServiceRefId == applicationId && c.TranStatus == 1 && c.status == 1);
                var exPayment = dbContext.ApplicationPaymentDetails.FirstOrDefault(x => x.applicationId == x.applicationId);
                if (onlinePayment == null)
                {
                    onlinePayment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(c => c.ServiceRefId == applicationId);
                }

                if (onlinePayment != null && exPayment == null)
                {
                    var payment = new ApplicationPaymentDetail();
                    payment.applicationId = applicationId;
                    payment.formNo = applicationId.ToString();
                    payment.paymentMode = onlinePayment.mode;
                    payment.amountDeposited = onlinePayment.Amount;
                    payment.utn = onlinePayment.TrKey;
                    payment.paymentDate = onlinePayment.modifiydate == null ? onlinePayment.EntryDate : onlinePayment.modifiydate;
                    payment.challanIssueDate = onlinePayment.modifiydate != null ? onlinePayment.EntryDate : null;
                    payment.createdBy = userInfo.UserID.ToString();
                    payment.createdDate = DateTime.Now;
                    dbContext.ApplicationPaymentDetails.Add(payment);
                    dbContext.SaveChanges();
                }
                flag = applicationId;
            }
            return flag;
        }


        public DataSourceResult GetApplicationFormAsDataSourceByApplicationId(DataSourceRequest request, int? applicationId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from applicant in dbContext.ApplicationDetails
                            join payment in dbContext.ApplicationPaymentDetails on applicant.applicationId equals payment.applicationId
                            where applicant.applicationId == applicationId && applicant.isAllotted == null
                            select new FormViewModel
                            {
                                Id = applicant.applicationId,
                                SchemeId = applicant.schemeId,
                                SchemeName = applicant.SchemeMst.schemeName,
                                DepartmentId = applicant.departmentId,
                                Department = applicant.DepartmentMst.departmentName,
                                FormNo = applicant.formNo,
                                ApplicationId = applicant.applicationId,
                                FirstName = applicant.firstName,
                                MiddleName = applicant.middleName,
                                LastName = applicant.lastName,
                                Applicant = applicant.gender == Constants.Company ? applicant.firstName : applicant.firstName + " " + (!string.IsNullOrEmpty(applicant.middleName) ? (applicant.middleName + " ") : "") + applicant.lastName,
                                ApplicantMaster = applicant.gender == Constants.Company ? applicant.signingAuthority : applicant.fatherHusbandName,
                                PermanentAddress = applicant.permanentAdd,
                                AnnualIncome = applicant.annualIncome,
                                PAN = applicant.pan,
                                MobileNo = applicant.mobileNumberP2,
                                CorrespondAddress = applicant.correspondanceAdd,
                                Email = applicant.email,
                                Gender = applicant.gender,
                                ApplicantType = applicant.gender,
                                TotalAmount = payment.amountDeposited,
                            });
                return data.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetPropertyDetailAsDataSourceByPropertyId(DataSourceRequest request, int? propertyId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from property in dbContext.SchemePropTrans
                            where property.IsActive == true && property.propertyId == propertyId
                            select new PropertyViewModel
                            {
                                Id = property.refId,
                                PropertyId = property.propertyId,
                                SchemeId = property.schemeId,
                                SchemeName = property.SchemeMst.schemeName,
                                DepartmentId = property.departmentId,
                                Department = property.DepartmentMst.departmentName,
                                PropertyTypeId = property.propertyTypeId,
                                PropertyType = property.PropertyTypeMst.propertyTypeName,
                                FloorId = property.floorId,
                                FloorArea = property.FloorMst.floorName,
                                PropertyNo = property.SectorMst.sectorName + "/" + (property.blockId == null ? "" : property.BlockMst.blockName + "-") + property.propertyNo,
                                TotalArea = property.totalArea,
                                CoveredArea = property.coveredArea,
                                ActualArea = property.actualArea,
                                PropertyCost = property.propertyCost,
                                CivilCost = property.civilCost,
                                TotalPropertyCost = property.totalPropertyCost,
                                LandRate = property.landRatePerSqmt
                            });

                return data.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetAllottedPropertyFormListById(DataSourceRequest request, AllotmentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from alotment in dbContext.AllotmentMasters
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            join aplicant in dbContext.ApplicationDetails on alotment.applicationId equals aplicant.applicationId
                            where (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                            && (model.SchemeId == null || alotment.schemeId == model.SchemeId)
                            select new AllotmentViewModel
                            {
                                Id = aplicant.applicationId,
                                RegistrationId = alotment.rid,
                                ApplicationId = aplicant.applicationId,
                                PropertyId = alotment.propertyId,
                                SchemeName = alotment.SchemeMst.schemeName,
                                Department = alotment.DepartmentMst.departmentName,
                                Applicant = aplicant.gender == Constants.Company ? aplicant.firstName : aplicant.firstName + " " + (!string.IsNullOrEmpty(aplicant.middleName) ? (aplicant.middleName + " ") : "") + aplicant.lastName,
                                Gender = aplicant.gender,
                                ApplicantType = aplicant.gender,
                                SubmissionDate = alotment.submitDate,
                                PAN = aplicant.pan,
                                AllotmentDate = alotment.allotmentDate,
                                AllotmentStatus = aplicant.isAllotted == "1" ? "Allotted" : (aplicant.isAllotted == null ? "Not Approved" : (aplicant.isAllotted == "3" ? "Cancelled" : "Not Allotted"))
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public int ValidatePropertyAndApplicationForm(FormViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == ActionType.Property)
                {
                    var property = dbContext.AllotmentMasters.FirstOrDefault(c => c.propertyId == model.PropertyId);
                    if (property != null) flag = ReturnType.Exist;
                    else flag = ReturnType.NotExist;
                }
                if (model.ActionType == ActionType.Application)
                {
                    var application = dbContext.ApplicationDetails.FirstOrDefault(a => a.formNo == model.ApplicationId.ToString() && a.isAllotted == NAStatusId.Allotted.ToString());
                    if (application != null) flag = ReturnType.Exist;
                    else flag = ReturnType.NotExist;
                }
            }
            return flag;
        }

        public int PropertyAllotmentForApplicationForm(FormViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var applicant = dbContext.ApplicationDetails.FirstOrDefault(o => o.applicationId == model.ApplicationId);
                var property = dbContext.SchemePropTrans.FirstOrDefault(id => id.propertyId == model.PropertyId);
                if (applicant != null && property != null)
                {
                    var exAllotment = dbContext.AllotmentMasters.FirstOrDefault(a => a.propertyId == model.PropertyId && a.isActive == NAStatusId.Active);
                    if (exAllotment == null)
                    {
                        var allotment = new AllotmentMaster
                        {
                            applicationId = applicant.applicationId,
                            schemeId = applicant.schemeId,
                            departmentId = applicant.departmentId,
                            formNo = applicant.formNo,
                            propertyId = property.propertyId.Value,
                            allotmentDate = model.AllotmentDate,
                            instalmentStartDate = model.InstallmentStartDate != null ? model.InstallmentStartDate : model.AllotmentDate.Value.AddMonths(6),
                            approverBy = model.ApproverId.ToString(),
                            isSubmitted = NAStatusId.Submitted.ToString(),
                            isActive = NAStatusId.Active,
                            createdBy = userInfo.UserID.ToString(),
                            submittedBy = userInfo.UserID,
                            createdDate = DateTime.Now,
                            submitDate = DateTime.Now
                        };
                        if (model.ActionType == "PreviousAllotment")
                        {
                            allotment.isStatus = Status.Approved;
                            allotment.comment = Status.Approved;
                            allotment.approveDate = DateTime.Now;
                        }
                        dbContext.AllotmentMasters.Add(allotment);
                        dbContext.SaveChanges();

                        applicant.registrationId = allotment.rid;
                        if (model.ActionType == "PreviousAllotment") applicant.isAllotted = NAStatusId.Allotted.ToString();
                        dbContext.SaveChanges();

                        flag = ReturnType.Allotted;
                    }
                    else { flag = ReturnType.NotAllotted; }
                }
            }
            return flag;
        }


        public DataSourceResult GetAllottedPropertyFormListForApproval(DataSourceRequest request, AllotmentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from alotment in dbContext.AllotmentMasters
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            join aplicant in dbContext.ApplicationDetails on alotment.applicationId equals aplicant.applicationId
                            where (model.RegistrationId == null || alotment.rid == model.RegistrationId)
                            && (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                            && (model.SchemeId == null || alotment.schemeId == model.SchemeId)
                            && (model.ApplicationId == null || alotment.applicationId == model.ApplicationId)
                            && (model.PropertyId == null || alotment.propertyId == model.PropertyId)
                            && DepartmentList.Contains(alotment.departmentId)
                            select new AllotmentViewModel
                            {
                                Id = aplicant.applicationId,
                                RegistrationId = alotment.rid,
                                ApplicationId = aplicant.applicationId,
                                FormNo = aplicant.formNo,
                                PropertyId = alotment.propertyId,
                                SchemeName = alotment.SchemeMst.schemeName,
                                Department = alotment.DepartmentMst.departmentName,
                                Applicant = aplicant.gender == Constants.Company ? aplicant.firstName : aplicant.firstName + " " + (!string.IsNullOrEmpty(aplicant.middleName) ? (aplicant.middleName + " ") : "") + aplicant.lastName,
                                Gender = aplicant.gender,
                                CorresspondAddress = aplicant.tCorrespondanceAdd,
                                PropertyNo = property.SectorMst.sectorName + "/" + (property.blockId == null ? string.Empty : property.BlockMst.blockName + "-") + property.propertyNo,
                                ApplicantType = aplicant.gender,
                                SubmissionDate = alotment.submitDate,
                                PAN = aplicant.pan,
                                AllotmentDate = alotment.allotmentDate,
                                Approver = (from user in dbContext.UmUserMasters where user.UserRefId.ToString() == alotment.approverBy select (user.FirstName + " " + (string.IsNullOrEmpty(user.MiddleName) ? string.Empty : user.MiddleName + " ") + user.LastName)).FirstOrDefault(),
                                UserId = userInfo.UserID,
                                Status = string.IsNullOrEmpty(aplicant.isAllotted) ? "Not Allotted" : dbContext.StatusMasters.FirstOrDefault(x => x.Id.ToString() == aplicant.isAllotted && x.IsActive == true).Status,
                                ApplicantStatus = aplicant.isAllotted,
                                AllotmentStatus = aplicant.isAllotted == "1" ? "Allotted" : "Not Allotted",
                                IsAllotted = (aplicant.isAllotted != "1" && alotment.approverBy == userInfo.UserID.ToString()) ? true : false
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public int UpdateAllottedPropertyStatus(AllotmentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var alotment = dbContext.AllotmentMasters.FirstOrDefault(a => a.rid == model.RegistrationId);
                if (alotment != null)
                {
                    var status = dbContext.StatusMasters.FirstOrDefault(s => s.Id == model.StatusId && s.IsActive == true);
                    alotment.isStatus = status.Status;
                    alotment.isActive = Constants.Active;
                    alotment.comment = model.Comment;
                    alotment.approveDate = DateTime.Now;

                    var applicant = dbContext.ApplicationDetails.FirstOrDefault(a => a.registrationId == model.RegistrationId);
                    applicant.isAllotted = model.StatusId.ToString();

                    dbContext.SaveChanges();
                    flag = ReturnType.Allotted;
                }
                else flag = ReturnType.NotAllotted;
            }
            return flag;
        }


        public DataSourceResult GetApplicationFormList(DataSourceRequest request, FormViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from forms in dbContext.ApplicationDetails
                            from payment in dbContext.ApplicationPaymentDetails.Where(a => a.applicationId == forms.applicationId).DefaultIfEmpty()
                            where DepartmentList.Contains(forms.departmentId)
                            && (model.ApplicationId == null || forms.applicationId == model.ApplicationId)
                            && (model.SchemeId == null || forms.schemeId == model.SchemeId)
                            && (model.DepartmentId == null || forms.departmentId == model.DepartmentId)
                            select new FormViewModel
                            {
                                Id = forms.applicationId,
                                ApplicationId = forms.applicationId,
                                FormId = forms.applicationId,
                                RegistrationId = forms.registrationId,
                                ExRegistrationId = forms.OldRegistrationId,
                                SchemeId = forms.schemeId,
                                SchemeName = forms.SchemeMst.schemeName,
                                DepartmentId = forms.departmentId,
                                Department = forms.DepartmentMst.departmentName,
                                FormNo = forms.formNo,
                                FirstName = forms.tFirstName,
                                MiddleName = forms.tMiddleName,
                                LastName = forms.tLastName,
                                FirstApplicant = forms.firstName + " " + (string.IsNullOrEmpty(forms.middleName) ? string.Empty : forms.middleName + " ") + forms.lastName,
                                Applicant = forms.tFirstName + " " + (string.IsNullOrEmpty(forms.tMiddleName) ? string.Empty : forms.tMiddleName + " ") + forms.tLastName,
                                ApplicantType = forms.gender,
                                ApplicantMaster = forms.gender == Constants.Company ? forms.signingAuthority : forms.fatherHusbandName,
                                CorrespondAddress = forms.tCorrespondanceAdd,
                                FirstApplicantAdd = forms.correspondanceAdd,
                                PermanentAddress = forms.tPermanentAdd,
                                MobileNo = forms.tMobileNumber,
                                IsFormAllotted = forms.isAllotted == "1" ? true : false,
                                FormStatus = forms.isAllotted == "1" ? Constants.Allotted : "Not Allotted",
                                PropertyId = forms.isAllotted == "1" ? dbContext.AllotmentMasters.FirstOrDefault(r => r.rid == forms.registrationId).propertyId : 0,
                                BankId = payment.bankId,
                                BankName = payment.BankMst.bankName,
                                PaymentType = payment.paymentMode,
                                DepositedAmount = payment.amountDeposited,
                                DDNo = payment.ddNo,
                                UTR = payment.utn,
                                CreatedDate = forms.createdDate
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public PropertyViewModel GetApplicantDetailsToTransferProperty(PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var details = (from alotment in dbContext.AllotmentMasters
                               join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                               join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                               where alotment.rid == model.RegistrationId
                               select new PropertyViewModel
                               {
                                   RegistrationId = alotment.rid,
                                   SchemeName = property.SchemeMst.schemeName,
                                   SchemeId = property.schemeId,
                                   PropertyId = alotment.propertyId,
                                   DepartmentId = alotment.departmentId,
                                   Department = alotment.DepartmentMst.departmentName,
                                   SectorId = property.sectorId,
                                   SectorName = property.SectorMst.sectorName,
                                   BlockId = property.blockId,
                                   BlockName = property.BlockMst.blockName,
                                   PlotNo = property.propertyNo,
                                   PropertyNo = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo,
                                   ApplicantType = aplicant.tGender,
                                   Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                   ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                   ApplicantAddress = aplicant.tCorrespondanceAdd,
                                   PropertyType = property.PropertyTypeMst.propertyTypeName,
                                   TotalArea = property.totalArea,
                                   FloorArea = property.FloorMst.floorName,
                                   MortgageStatus = ((from mort in dbContext.MortgageDetails where (mort.PreviousLoanNoc != MortPrevLoan.invalid || (mort.StatusId == StatusOption.Approved || mort.StatusId == StatusOption.InProgress)) && (mort.RID == model.RegistrationId && mort.IsActive == true) orderby mort.RequestNo descending select mort).FirstOrDefault() == null) ? Constants.no : Constants.yes,
                                   FunctionalStatus = ((from func in dbContext.FunctionalDetails where func.StatusId == StatusOption.Approved && func.Rid == model.RegistrationId && func.IsActive == true orderby func.RequestNo descending select func).FirstOrDefault() == null) ? Constants.no : Constants.yes,
                                   OneTimeLeaseStatus = (from rdm in dbContext.RECEIPT_DETAIL_MASTER join rat in dbContext.RECEIPT_AMOUNT_TRANS on rdm.RECEIPT_ID equals rat.RECEIPT_ID where rdm.RID_NO == model.RegistrationId.ToString() && rat.RECEIPT_HEAD_ID == Constants.OTLRReceiptHead && rat.RECEIPT_SUBHEAD_ID == Constants.OTLRReceiptSubHead select rdm).FirstOrDefault() == null ? Constants.no : Constants.yes,
                                   IsLeaseDeedExecuted = dbContext.RegistryDetails.FirstOrDefault(r => r.Rid == model.RegistrationId) != null ? true : false,
                                   AllotmentDate = alotment.allotmentDate,
                                   RegistryDate = (from reg in dbContext.RegistryDetails where reg.IsActive == true && reg.Rid == model.RegistrationId select reg.RegistryDoneDate).FirstOrDefault(),//reg.RegistryDoneDate,
                                   PossessionDate = (from pos in dbContext.PossessionDetails where pos.Rid == model.RegistrationId && pos.IsActive == true select pos.PossessionDate).FirstOrDefault(), //Subquery because this function is being used in many places to pick data and its not necessary that possession has been given for all RIDs. //pos.PossessionDate,
                               }).FirstOrDefault();

                if (details != null)
                {
                    var GPA = (from gpa in dbContext.GPAs where gpa.Rid == model.RegistrationId && DbFunctions.TruncateTime(gpa.Effcetd_From) <= DbFunctions.TruncateTime(DateTime.Now) && gpa.Is_Active == true select gpa).FirstOrDefault();
                    if (GPA != null)
                    {
                        details.IsPropertyHasGPA = true;
                        details.GPAHolderName = GPA.GPA_Holder_Name;
                        details.GPAHolderAddress = GPA.GPA_Holder_Address;
                        details.GPAEffectiveFromDate = GPA.Effcetd_From;
                        details.GPAEffectiveTillDate = GPA.Effected_To;
                        details.IsGPAEffective = GPA.Effected_To.Value.Date <= DateTime.Now.Date ? false : true;
                    }
                    details.IsCancelledOrSurrendered = dbContext.Property_Cancellation_Details.Where(r => r.Type == Constants.CancellationId || r.Type == Constants.SurrenderId && r.Rid == model.RegistrationId && r.Is_Active == true).OrderByDescending(o => o.Id).FirstOrDefault() == null ? false : true;
                }
                return details;
            }
        }


        public int SavePropertyTransferDetail(TransferViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var ExTransfer = dbContext.Succ_Mut_Trans.FirstOrDefault(t => t.Request_No == model.Id && t.Status == NAStatusId.Initiated);
                if (ExTransfer != null)
                {
                    ExTransfer.Applicant_Name = model.Applicant;
                    ExTransfer.Applicant_Signing_Authority = model.ApplicantMaster;
                    ExTransfer.ApplicantFather_Name = model.ApplicantMaster;
                    ExTransfer.Transfer_Date = model.TransferDate;
                    ExTransfer.Applicant_Gender = model.ApplicantType;
                    ExTransfer.Type = TransferType.T.ToString();
                    ExTransfer.Transfer_Type = model.TransferTypeId;
                    ExTransfer.Transfer_Sub_Type = model.TransferSubTypeId;
                    ExTransfer.Correspondance_Add = model.CorrespondenceAdd;
                    ExTransfer.Permanent_Add = model.PermanentAdd;
                    ExTransfer.Transfer_Charge = model.TransferRate;
                    ExTransfer.Total_Transfer_Charge = model.TotalTransferCharge;
                    ExTransfer.Is_Active = true;
                    ExTransfer.Requested_By = userInfo.UserID;
                    ExTransfer.Modified_By = userInfo.UserID;
                    ExTransfer.Modified_Date = DateTime.Now;

                    ExTransfer.T_Gender = model.TransfereeType;
                    ExTransfer.T_First_Name = model.TransfereeType == Constants.Company ? null : model.Transferee;
                    ExTransfer.T_Father_Husband_Name = model.TransfereeType == Constants.Company ? null : model.TransfereeMaster;
                    ExTransfer.T_Mother_Name = model.TransfereeType == Constants.Company ? null : model.TransfereeMother;
                    ExTransfer.T_Occupation_Id = model.TransfereeType == Constants.Company ? null : model.OccupationId;
                    ExTransfer.T_Company_Name = model.TransfereeType == Constants.Company ? model.Transferee : null;
                    ExTransfer.T_Signing_Authority = model.TransfereeType == Constants.Company ? model.TransfereeMaster : null;
                    ExTransfer.T_Registered_Office = model.TransfereeType == Constants.Company ? model.TransfereeAddressII : null;
                    ExTransfer.T_Email = model.Email;
                    ExTransfer.T_Mobile = model.MobileNo;
                    ExTransfer.T_Correspondence_Add = model.TransfereeAddress;
                    ExTransfer.T_Permanent_Add = model.TransfereeAddressII;
                    ExTransfer.T_Registered_Office = model.TransfereeAddress;
                    ExTransfer.T_Pan = model.PAN;
                    ExTransfer.OnlineRequestNo = model.OnlineRequestNo;
                    ExTransfer.Approved_By = model.ApproverId;
                    dbContext.SaveChanges();
                }
                else
                {
                    var transfer = new Succ_Mut_Trans();
                    transfer.Rid = model.RegistrationId;
                    transfer.Property_Id = model.PropertyId;
                    transfer.Applicant_Name = model.Applicant;
                    transfer.Applicant_Signing_Authority = model.ApplicantMaster;
                    transfer.ApplicantFather_Name = model.ApplicantMaster;
                    transfer.Transfer_Date = model.TransferDate;
                    transfer.Applicant_Gender = model.ApplicantType;
                    transfer.Type = TransferType.T.ToString();
                    transfer.Transfer_Type = model.TransferTypeId;
                    transfer.Transfer_Sub_Type = model.TransferSubTypeId;
                    transfer.Correspondance_Add = model.CorrespondenceAdd;
                    transfer.Permanent_Add = model.PermanentAdd;
                    transfer.Transfer_Charge = model.TransferRate;
                    transfer.Total_Transfer_Charge = model.TotalTransferCharge;
                    transfer.Is_Active = true;
                    transfer.Status = NAStatusId.Initiated;
                    transfer.Requested_By = userInfo.UserID;
                    transfer.Requested_Date = DateTime.Now;
                    transfer.Created_By = userInfo.UserID;
                    transfer.Created_Date = DateTime.Now;

                    transfer.T_Gender = model.TransfereeType;
                    transfer.T_First_Name = model.TransfereeType == Constants.Company ? null : model.Transferee;
                    //transfer.T_Middle_Name = transfereeMiddleName;
                    //transfer.T_Last_Name = transfereeLstName;
                    transfer.T_Father_Husband_Name = model.TransfereeType == Constants.Company ? null : model.TransfereeMaster;
                    transfer.T_Mother_Name = model.TransfereeType == Constants.Company ? null : model.TransfereeMother;
                    transfer.T_Occupation_Id = model.TransfereeType == Constants.Company ? null : model.OccupationId;
                    transfer.T_Company_Name = model.TransfereeType == Constants.Company ? model.Transferee : null;
                    transfer.T_Signing_Authority = model.TransfereeType == Constants.Company ? model.TransfereeMaster : null;
                    transfer.T_Registered_Office = model.TransfereeType == Constants.Company ? model.TransfereeAddressII : null;
                    transfer.T_Email = model.Email;
                    transfer.T_Mobile = model.MobileNo;
                    transfer.T_Correspondence_Add = model.TransfereeAddress;
                    transfer.T_Permanent_Add = model.TransfereeAddressII;
                    transfer.T_Registered_Office = model.TransfereeAddress;
                    transfer.T_Pan = model.PAN;
                    transfer.OnlineRequestNo = model.OnlineRequestNo;
                    transfer.Approved_By = model.ApproverId;
                    dbContext.Succ_Mut_Trans.Add(transfer);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
            }
            return flag;
        }

        public TransferViewModel GetTransferedPropertyDetailById(TransferViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var details = (from transfer in dbContext.Succ_Mut_Trans
                               join alotment in dbContext.AllotmentMasters on transfer.Rid equals alotment.rid
                               join aplicant in dbContext.ApplicationDetails on transfer.Rid equals aplicant.registrationId
                               join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                               join statsmst in dbContext.StatusMasters on transfer.Status equals statsmst.Id
                               join approver in dbContext.UmUserMasters on transfer.Approved_By equals approver.UserRefId
                               where transfer.Request_No == model.RequestNo
                               select new TransferViewModel
                               {
                                   RequestNo = transfer.Request_No,
                                   RegistrationId = transfer.Rid,
                                   PropertyId = transfer.Property_Id,
                                   DepartmentId = property.departmentId,
                                   Department = property.DepartmentMst.departmentName,
                                   SectorId = property.sectorId,
                                   Sector = property.SectorMst.sectorName,
                                   BlockId = property.blockId,
                                   Block = property.BlockMst.blockName,
                                   PlotNo = property.propertyNo,
                                   PropertyNo = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo,
                                   ApplicantType = transfer.Applicant_Gender,
                                   Applicant = transfer.Applicant_Name,
                                   ApplicantMaster = transfer.Applicant_Gender == Constants.Company ? transfer.Applicant_Signing_Authority : transfer.ApplicantFather_Name,
                                   ApplicantAddress = transfer.Correspondance_Add,
                                   ApplicantAddressII = transfer.Applicant_Gender == Constants.Company ? transfer.Applicant_Registered_Office : transfer.Permanent_Add,

                                   GPAHolderName = transfer.GPA_Holder_Name,
                                   GPAHolderAddress = transfer.GPA_Holder_Address,
                                   GPAEffectiveFrom = transfer.GPA_Effective_From,
                                   GPAEffectiveTo = transfer.GPA_Effective_To,

                                   TransfereeType = transfer.T_Gender,
                                   Transferee = transfer.T_Gender == Constants.Company ? transfer.T_Company_Name : transfer.T_First_Name,
                                   TransfereeMaster = transfer.T_Gender == Constants.Company ? transfer.T_Signing_Authority : transfer.T_Father_Husband_Name,
                                   TransfereeMother = transfer.T_Mother_Name,
                                   TransfereeAddress = transfer.T_Correspondence_Add,
                                   TransfereeAddressII = transfer.T_Gender == Constants.Company ? transfer.T_Registered_Office : transfer.T_Permanent_Add,
                                   Email = transfer.T_Email,
                                   MobileNo = transfer.T_Mobile,
                                   PAN = transfer.T_Pan,
                                   OccupationId = transfer.T_Occupation_Id,
                                   TransferTypeId = transfer.Transfer_Type,
                                   TransferSubTypeId = transfer.Transfer_Sub_Type,
                                   
                                   TransferDate = transfer.Transfer_Date,
                                   TransferdeedDate = transfer.TransferDeed_Date,
                                   TransferRate = transfer.Transfer_Charge,
                                   TotalTransferCharge = transfer.Total_Transfer_Charge,
                                   TransferStatus = statsmst.Status,
                                   Comment = transfer.Comment,
                                   ApprovedDate = transfer.Approved_Date,
                                   Approver = (from us in dbContext.UmUserMasters join tr in dbContext.Succ_Mut_Trans on us.UserRefId equals tr.Approved_By where tr.Request_No == model.RequestNo select us.FirstName + " " + us.MiddleName + " " + us.LastName).FirstOrDefault(),
                                   
                                   //TypeOfTransferee = (transfer.T_Gender == Constants.genderCompany) ? 1 : 2, //1 for comapny and 2 for individual
                                   OnlineRequestNo = transfer.OnlineRequestNo != null ? (int)transfer.OnlineRequestNo : 0
                               }).FirstOrDefault();
                return details;
            }
        }


        public DataSourceResult GetAllotmentRequestByUserIdAsDataSource(DataSourceRequest request, AllotmentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from alotment in dbContext.AllotmentMasters
                            join users in dbContext.UmUserMasters on alotment.createdBy equals users.UserRefId.ToString()
                            join depts in dbContext.DepartmentMsts on alotment.departmentId equals depts.departmentId
                            join schemes in dbContext.SchemeMsts on alotment.schemeId equals schemes.schemeId
                            where alotment.isActive == 1 && alotment.isSubmitted == "1" && alotment.isStatus != "Approved"
                            && alotment.approverBy == userInfo.UserID.ToString()
                            && DepartmentList.Contains(alotment.departmentId)
                            select new AllotmentViewModel
                            {
                                RegistrationId = alotment.rid,
                                SchemeId = alotment.schemeId,
                                SchemeName = schemes.schemeName,
                                DepartmentId = alotment.departmentId,
                                Department = depts.departmentName,
                                Status = alotment.isStatus,
                                ApproverId = userInfo.UserID,
                                Approver = users.FirstName + " " +(string.IsNullOrEmpty(users.MiddleName) ? string.Empty : users.MiddleName + " ") + users.LastName,
                                //Approver = dbContext.UmUserMasters.Where(x => x.UserRefId.ToString() == alotment.approverBy).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault(),//TODO now: ApproverName column's value
                                ApprovalDate = alotment.approveDate,
                                SubmissionDate = alotment.submitDate,
                                RequestorId = alotment.submittedBy,
                                Requestor = users.FirstName + " " + (string.IsNullOrEmpty(users.MiddleName) ? string.Empty : users.MiddleName + " ")+ users.LastName,
                                //Requestor = dbContext.UmUserMasters.Where(x => x.UserRefId == alotment.submittedBy).Select(y => y.FirstName + " " + y.MiddleName + " " + y.LastName).FirstOrDefault(),//TODO: SubmittedBy field to be added.
                                AllotmentDate = alotment.allotmentDate,//,
                                //ApplicationId = allotmentMaster.applicationId
                                CreatedDate = alotment.createdDate,
                                IsApproved = alotment.isStatus == "Approved" ? true : false
                            });

                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetAllottedPropertyListByYearAsDataSource(DataSourceRequest request, AllotmentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from alotment in dbContext.AllotmentMasters
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            join aplicant in dbContext.ApplicationDetails on alotment.applicationId equals aplicant.applicationId
                            where (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                            && (model.SectorId == null || property.sectorId == model.SectorId)
                            && (model.BlockId == null || property.blockId == model.BlockId)
                            && (model.Year == null || alotment.allotmentDate.Value.Year == model.Year)
                            select new AllotmentViewModel
                            {
                                Id = aplicant.applicationId,
                                RegistrationId = alotment.rid,
                                ApplicationId = aplicant.applicationId,
                                PropertyId = alotment.propertyId,
                                SchemeName = alotment.SchemeMst.schemeName,
                                Department = alotment.DepartmentMst.departmentName,
                                SectorId = property.sectorId,
                                SectorName = property.SectorMst.sectorName,
                                BlockId = property.blockId,
                                BlockName = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                Applicant = aplicant.gender == Constants.Company ? aplicant.firstName : aplicant.firstName + " " + (!string.IsNullOrEmpty(aplicant.middleName) ? (aplicant.middleName + " ") : "") + aplicant.lastName,
                                Gender = aplicant.gender,
                                ApplicantType = aplicant.gender,
                                SubmissionDate = alotment.submitDate,
                                PAN = aplicant.pan,
                                AllotmentDate = alotment.allotmentDate,
                                AllotmentStatus = aplicant.isAllotted == "1" ? "Allotted" : (aplicant.isAllotted == null ? "Not Approved" : (aplicant.isAllotted == "3" ? "Cancelled" : "Not Allotted"))
                            });
                return list.ToDataSourceResult(request);
            }
        }
    }
}

