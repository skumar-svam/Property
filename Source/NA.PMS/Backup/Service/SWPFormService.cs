using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
//using NA.PMS.Model;
using NA.PMS.NICService.Resource;
//using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using com.fss.plugin.bob;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using NA.PMS.NICService.Context;

namespace NA.PMS.NICServices
{
    public class SWPFormService : ISWPFormService
    {
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public SWPFormService()
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

        public SWPFormViewModel GetSchemeBasicInfoData(SWPFormViewModel form)
        {
            int schemeId = 0;
            int departmentId = 0; 

            using (var dbContext = new PIMSEntitiesContext())
            {
                int? formId = (form.ApplicationFormId != null && form.ApplicationFormId > 0) ? form.ApplicationFormId : 0;
                var onlineform = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == formId);
                if (onlineform != null)
                {
                    schemeId = onlineform.schemeId.Value;
                    departmentId = onlineform.departmentId.Value;
                }
                else
                {
                    departmentId = ConfigurationManager.AppSettings["DepartmentId"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["DepartmentId"]) : 0;

                    if (form.SchemeType == SWPSchemeType.IndustrialPlots) { schemeId = ConfigurationManager.AppSettings["SchemeId"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["SchemeId"]) : 0; }
                    else if (form.SchemeType == SWPSchemeType.Transport) { schemeId = ConfigurationManager.AppSettings["TransportScheme"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["TransportScheme"]) : 0; }
                    else if (form.SchemeType == SWPSchemeType.OpenEnded) { schemeId = ConfigurationManager.AppSettings["OpenScheme"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["OpenScheme"]) : 0; }
                    else if (form.SchemeType == SWPSchemeType.IndustrialScheme) { schemeId = ConfigurationManager.AppSettings["IndustrialSchemeId"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["IndustrialSchemeId"]) : 0; }

                }
                var scheme = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId == schemeId && s.IsActive == true);
                if (scheme != null)
                {
                    form.SchemeId = scheme.schemeId;
                    form.SchemeName = scheme.schemeName;
                    form.SchemeEndDate = scheme.endDate;
                    form.ApplicationFee = scheme.FormFee;
                    form.ProcessingCharge = scheme.ProcessingFee;
                    form.FormFeeGST = scheme.FormCGST + scheme.FormSGST;
                    form.ProcessingChargeGST = scheme.ProcessingCGST + scheme.ProcessingSGST;
                    form.SchemeType = dbContext.SchemeTypeMsts.Where(x => x.schemeTypeId == scheme.schemeTypeId).Select(x => x.SchemeTypeDesc).FirstOrDefault();
                }
                var department = dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == departmentId);
                if (department != null)
                {
                    form.DepartmentId = department.departmentId;
                    form.Department = department.departmentName;
                }
            }
            return form;
        }

        public SWPFormViewModel GetOpenEndedSchemeFormDataById(SWPFormViewModel form)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                SWPFormViewModel applicant = new SWPFormViewModel();
                var detail = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == form.ApplicationFormId && c.isActive == true);
                if (detail != null)
                {
                    form.Id = detail.onlineapplicationId;
                    form.ApplicationFormId = detail.onlineapplicationId;
                    form.UserPassword = "Noida"; //user password for login existing application
                    form.SchemeId = detail.schemeId;
                    form.SchemeName = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId == form.SchemeId).schemeName;
                    form.SchemeEndDate = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId == form.SchemeId).endDate;
                    form.SchemeType = dbContext.SchemeMsts.FirstOrDefault(c => c.schemeId == detail.schemeId).SchemeTypeMst.SchemeTypeDesc;
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
                    form.Religion = detail.religionId != null ? dbContext.ReligionMsts.Where(r => r.religionId == detail.religionId).Select(r => r.religion).FirstOrDefault() : null;
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

                    if (form.Gender.ToLower() == SWPConstant.Company.ToLower())
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

                    form.DirectorModel = new SWPDirectorViewModel
                    {
                        DirectorType = (detail.gender == "Company" && detail.occupationId != null) ? dbContext.Common_Config.Where(m => m.Id == detail.occupationId).FirstOrDefault().Name : string.Empty
                    };
                    form.ProposedModel = new SWPCompanyViewModel
                    {
                        ProposedProject = detail.projectname,
                        ImplementationTime = detail.projecttimeempl,
                        TotalCost = detail.projectcost
                    };
                    form.IsProposedProjectSaved = (!string.IsNullOrEmpty(detail.projectname) && !string.IsNullOrEmpty(detail.projecttimeempl) && !string.IsNullOrEmpty(detail.projectcost)) ? true : false;

                    form.PreviousFormNo = detail.PreviousFormNo != null ? detail.PreviousFormNo : string.Empty;
                    form.PaidThroughSWP = false;
                    form.IsApplicationFeePaid = false;
                    //check nic data
                    var nic = dbContext.NICsingalwindowSystems.FirstOrDefault(n => n.onlineapplicationId == form.ApplicationFormId);
                    if (nic != null)
                    {
                        form.NICControlId = nic.Control_ID;
                        form.NICUnitId = nic.Unit_Id;
                        form.NICServiceId = nic.ServiceID;
                        form.NICProcessIndustryId = nic.ProcessIndustryID;
                        form.IsFromNIC = true;
                        form.AppType = SWPConstant.NIC;
                        form.PaidThroughSWP = true;
                        form.IsApplicationFeePaid = nic.Status_Code == 11 ? true : false;
                        form.ServiceAppPayStatus = nic.Fee_Status;

                        form.NICSingleWindowModel = nic;
                    }
                    else
                    {
                        form.IsFromNIC = false;
                        form.AppType = SWPConstant.Authority;
                    }
                    //check payment
                    var payment = dbContext.OnlineApplicationDetails_trans.Where(p => p.ServiceRefId == detail.onlineapplicationId).OrderByDescending(o => o.AutoID).FirstOrDefault();
                    if (payment != null)
                    {
                        //form.IsApplicationFeePaid = form.IsApplicationFeePaid == true ? true : ((payment.status == 1 && payment.ServiceType == 1) ? true : false);
                        form.IsRegistrationFeePaid = (payment.ServiceType == 3 && payment.status == 1) ? true : ((payment.status == 1 && payment.ServiceType == 1) ? true : false);
                        form.PaymentMode = (payment.ServiceType == 3) ? (!string.IsNullOrEmpty(payment.TrKey) ? ((payment.status == 1 || payment.TranStatus == 1) ? "Offline_Validated" : "Offline_Updated") : "Offline") : "Online";
                        form.IsChallanGenerated = payment.ServiceType == 3 ? true : false;

                        form.PaymentModel = GetOnlinePaymentDetailById(form.ApplicationFormId, payment.AutoID);
                    }

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

                    var flag = IsDocumentUploaded(form.ApplicationFormId);
                    if (flag == true)
                    {
                        form.IsDocumentUploaded = true;
                        form.DocumentsTable = dbContext.Sp_NewSchemereturn(form.ApplicationFormId, form.SchemeId).FirstOrDefault();
                    }
                    //else applicant.IsDocumentUploaded = false;

                    if (form.SchemeType == SWPSchemeType.Transport || form.SchemeType == SWPSchemeType.OpenEnded || form.SchemeType == SWPSchemeType.IndustrialScheme)
                    {
                        var PreChallan = IsPreviousChallanUploaded(form.ApplicationFormId);
                        if (PreChallan == true)
                        {
                            form.IsPreviousChallanUploaded = true;
                        }
                    }
                }
                else
                {
                    form.FlagId = SWPReturnTypeId.NotExist;
                }
                return form;
            }
        }

        private SWPPaymentViewModel GetOnlinePaymentDetailById(int? id, int? AutoId)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var detail = (from transaction in dbContext.OnlineApplicationDetails_trans
                              where transaction.ServiceRefId == id && transaction.AutoID == AutoId
                              select new SWPPaymentViewModel
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

                                  Status = (transaction.TranStatus != null ? (transaction.TranStatus == SWPConstant.Success ? "Success" : "Failure") : "Validation Pending"),
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

        private bool IsDocumentUploaded(int? formId)
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

        private bool IsPreviousChallanUploaded(int? formId)
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

        public SWPFormViewModel SaveOpenEndedSchemeFormDetail(SWPFormViewModel model, System.Web.HttpPostedFileBase userImage, System.Web.HttpPostedFileBase signatureImage)
        {
            string encryptedId = string.Empty;
            int nicmsg = 0;
            using (var dbContext = new PIMSEntitiesContext())
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
                                //application.quotaId = model.CategoryId;
                                application.quotaId = SWPConstant.General;
                                application.marritalStatus = model.MaritalStatus;
                                application.dateOfBirth = model.DOB;
                            }
                            else
                            {
                                application.CompanyName = model.Applicant;
                                application.signingAuthority = model.SigningAuthority;
                                application.registeredOffice = model.PermanentAddress;
                                application.CompanyType = model.CompanyTypeId;
                                application.gender = string.IsNullOrEmpty(model.ApplicantType) ? SWPConstant.Company : model.ApplicantType;
                                application.fatherHusbandName = model.ApplicantMaster;
                                application.quotaId = model.SigningAuthorityId;
                            }
                            application.CompanyType = model.CompanyTypeId;
                            application.schemeId = model.SchemeId;
                            application.departmentId = model.DepartmentId;
                            application.permanentAdd = model.PermanentAddress;
                            application.correspondanceAdd = model.CorrespondingAddress;
                            application.area = model.AreaRangeId;
                            //application.Phase = Convert.ToInt32(model.AreaRange);
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

                            //application.RefundBankId = model.RefundBankId;
                            //application.RefundInfaverof = model.RefundInfaverof;
                            //application.RefundaccountNo = model.RefundAccountNo;

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
                            application.isActive = true;
                            application.IsSubmited = false;

                            //application.projectname = model.ProposedModel.ProposedProject;
                            //application.projectcost = model.ProposedModel.TotalCost.ToString();
                            //application.projecttimeempl = model.ProposedModel.ImplementationTime;
                            application.Online_offline = model.FormType == "Online" ? "Y" : "N";

                            string PassWord = string.Empty;
                            if (model.SchemeType == SWPSchemeType.OpenEnded || model.SchemeType == SWPSchemeType.Transport)
                            {
                                PassWord = SWPApplication.GeneratePasswordForScheme();
                                application.Userpassword = PassWord.ToMD5HashForPasswordPIS();
                            }

                            dbContext.OnlineApplicationDetails.Add(application);
                            dbContext.SaveChanges();
                            //added on 1-nov-2019
                            model.ApplicationFormId = application.onlineapplicationId;

                            if (!string.IsNullOrEmpty(model.NICControlId))
                            {
                                //Table xmlTbl = new Table();
                                SWPTable xmlTbl = new SWPTable();
                                xmlTbl.OnlineApplicationId = application.onlineapplicationId;
                                xmlTbl.departmentId = application.departmentId.Value;
                                xmlTbl.SchemeId = application.schemeId.Value;
                                xmlTbl.ServiceID = model.NICServiceId;
                                xmlTbl.Control_ID = model.NICControlId;
                                xmlTbl.ApplicationID = model.NICApplicationId;
                                xmlTbl.Unit_Id = model.NICUnitId;
                                xmlTbl.ProcessIndustryID = application.onlineapplicationId.ToString();
                                //NewDataSet nds = new NewDataSet();
                                //nds.Table = xmlTbl;
                                SWPTableViewModel nds = new SWPTableViewModel();
                                nds.Table = xmlTbl;

                                NICsingalwindowSystem NICdata = new NICsingalwindowSystem();
                                NICdata = MapNICsingalwindowSystemTable(nds);//mapping nic model
                                NICdata.Fee_Amount = model.FormFeeWithGST;
                                NICdata.Status_Code = Convert.ToInt32(SWPStatus.FEE_PENDING);
                                NICdata.Fee_Status = SWPStatus.Wording.FEE_PENDING;
                                dbContext.NICsingalwindowSystems.Add(NICdata);
                                dbContext.SaveChanges();
                                model.FormStatusId = SWPStatusId.Success;
                                nicmsg = 1;
                            }

                            int id = application.onlineapplicationId;
                            model.Id = id;
                            model.ApplicationFormId = id;
                            model.NICApplicationId = Convert.ToString(id);
                            model.NICProcessIndustryId = Convert.ToString(id);
                            //save documents photo and signature
                            SaveDocumentsForApplicationForm(model, null, userImage, signatureImage);

                            //UpdateDirectorDetailsForOpenSchemeForm(id);

                            string message = string.Format(SWPMessage.OnlineApplicationSubmitted, id);
                            if (!string.IsNullOrEmpty(model.Email)) SWPApplication.SendEmail(model.Email, "Registration Form", message);
                            if (!string.IsNullOrEmpty(model.MobileNumber)) SWPApplication.SendSMS(model.MobileNumber, message);

                            if (model.SchemeType == SWPSchemeType.OpenEnded || model.SchemeType == SWPSchemeType.Transport)
                            {
                                if (nicmsg == 0)
                                {
                                    string mobileMessage = string.Format(SWPMessage.OnlineUserRegistration, id, PassWord);
                                    string emailMessage = string.Format(SWPMessage.OnlineUserRegistration, id, PassWord);
                                    if (model.MobileNumber != null && model.MobileNumber != "") SWPApplication.SendSMS(model.MobileNumber, mobileMessage);
                                    if (model.Email != null && model.Email != "") SWPApplication.SendEmail(model.Email, "OnlineForm", emailMessage);
                                }
                            }
                            encryptedId = SWPEncryption.Encode(model.Id.ToString());
                        }
                        else
                        {
                            int flag = UpdateOpenSchemeForm(model, userImage, signatureImage);
                            model.FormStatusId = SWPStatusId.Success;
                            encryptedId = SWPEncryption.Encode(model.Id.ToString());
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

        private NICsingalwindowSystem MapNICsingalwindowSystemTable(SWPTableViewModel apidata)
        {
            NICsingalwindowSystem nicForm = new NICsingalwindowSystem();
            nicForm.onlineapplicationId = apidata.Table.OnlineApplicationId;
            nicForm.schemeId = apidata.Table.SchemeId;
            nicForm.Departmentid = apidata.Table.departmentId;
            nicForm.Control_ID = apidata.Table.Control_ID;
            nicForm.Unit_Id = apidata.Table.Unit_Id;
            nicForm.ServiceID = apidata.Table.ServiceID;
            nicForm.ProcessIndustryID = Convert.ToString(apidata.Table.OnlineApplicationId);
            //below lines were commented
            nicForm.Company_Name = apidata.Table.Company_Name;
            nicForm.Industry_District = apidata.Table.Industry_District;
            nicForm.Industry_District_Id = apidata.Table.Industry_District_Id;
            nicForm.Industry_Address = apidata.Table.Industry_Address;
            nicForm.Pin_Code = apidata.Table.Pin_Code;
            nicForm.Occupier_Name = apidata.Table.Occupier_Name;

            nicForm.Occupier_Email_ID = apidata.Table.Occupier_Email_ID;
            nicForm.Occupier_Mobile_No = apidata.Table.Occupier_Mobile_No;
            nicForm.Occupier_DOB = apidata.Table.Occupier_DOB;
            nicForm.Occupier_Gender = apidata.Table.Occupier_Gender;
            //below lines were commented
            nicForm.Occupier_Address = apidata.Table.Occupier_Address;
            nicForm.Occupier_District_ID = apidata.Table.Occupier_District_ID;
            nicForm.Occupier_District_Name = apidata.Table.Occupier_District_Name;
            nicForm.Occupier_Pin_Code = apidata.Table.Occupier_Pin_Code;
            nicForm.Nature_of_Activity = apidata.Table.Nature_of_Activity;
            nicForm.Installed_Capacity = apidata.Table.Installed_Capacity;
            nicForm.Employees = apidata.Table.Employees;
            nicForm.Nature_of_Operation = apidata.Table.Nature_of_Operation;
            nicForm.publicdecimalProject_Cost = Convert.ToString(apidata.Table.Project_Cost);
            nicForm.Organization_Type_ID = apidata.Table.Organization_Type_ID;
            nicForm.Organization_Type = apidata.Table.Organization_Type;
            nicForm.Industry_Type_ID = apidata.Table.Industry_Type_ID;
            nicForm.Industry_Type_Name = apidata.Table.Industry_Type_Name;
            nicForm.Expected_date_construction = apidata.Table.Expected_date_construction;
            nicForm.Project_Status = apidata.Table.Project_Status;
            nicForm.Industry_Color = apidata.Table.Industry_Color;
            nicForm.Expected_date_production = apidata.Table.Expected_date_production;
            nicForm.Unit_Category = apidata.Table.Unit_Category;
            nicForm.Items_Manufactured = apidata.Table.Items_Manufactured;

            nicForm.Annual_Turnover = Convert.ToString(apidata.Table.Annual_Turnover);
            return nicForm;
        }

        private int SaveDocumentsForApplicationForm(SWPFormViewModel model, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
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
                            using (var dbContext = new PIMSEntitiesContext())
                            {
                                OnlineCheckLisTran onlineDocument = new OnlineCheckLisTran();
                                onlineDocument.CheckListId = Convert.ToInt32(docsIds[count]);
                                onlineDocument.onlineapplicationId = (int)model.Id;
                                onlineDocument.FileNAme = (model.Id + "-" + docsIds[count] + extension).ToString();
                                onlineDocument.CreatedDate = DateTime.Now.Date;
                                onlineDocument.CreatedBy = 0;
                                onlineDocument.isActive = true;
                                dbContext.OnlineCheckLisTrans.Add(onlineDocument);
                                dbContext.SaveChanges();
                            }
                            file.SaveAs(fileSavePath);
                            count++;
                            flag = SWPReturnTypeId.Success;
                        }
                    }
                    //30-11-2017 shatrughna
                    string message = string.Format(SWPMessage.UploadDocumentSuccess, model.Id);
                    //string message = string.Empty;
                    if (!string.IsNullOrEmpty(model.MobileNumber)) { SWPApplication.SendSMS(model.MobileNumber, message); }
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
                flag = SWPReturnTypeId.Success;
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
                flag = SWPReturnTypeId.Success;
            }

            return flag;
        }

        private int UpdateOpenSchemeForm(SWPFormViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase signatureImage)
        {
            var flag = SWPReturnTypeId.None;
            using (var dbContext = new PIMSEntitiesContext())
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
                        //application.quotaId = model.CategoryId;
                        application.quotaId = SWPConstant.General;
                        application.marritalStatus = model.MaritalStatus;
                        application.dateOfBirth = model.DOB;
                    }
                    else
                    {
                        application.CompanyName = model.Applicant;
                        application.signingAuthority = model.SigningAuthority;
                        application.registeredOffice = model.PermanentAddress;
                        application.CompanyType = model.CompanyTypeId;
                        application.gender = string.IsNullOrEmpty(model.ApplicantType) ? "Company" : model.ApplicantType;
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
                    application.isActive = true;
                    application.IsSubmited = false;

                    application.projectname = model.ProposedModel.ProposedProject;
                    application.projectcost = model.ProposedModel.TotalCost.ToString();
                    application.projecttimeempl = model.ProposedModel.ImplementationTime;
                    application.Online_offline = model.FormType == "Online" ? "Y" : "N";
                    if (SWPSchemeType.Transport == model.SchemeType)
                    {
                        application.PropertyNo = model.PropertyNo;
                    }

                    dbContext.SaveChanges();

                    SaveDocumentsForApplicationForm(model, null, userImage, signatureImage);

                    //UpdateDirectorDetailsForOpenSchemeForm(model.ApplicationFormId);

                    flag = SWPReturnTypeId.Updated;
                }
                else
                {
                    flag = SWPReturnTypeId.NotExist;
                }
            }
            return flag;
        }

        public SWPFormViewModel SaveProposedProjectAndRefundDetail(SWPFormViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(f => f.onlineapplicationId == model.ApplicationFormId);
                if (form != null)
                {
                    form.RefundBankId = model.RefundBankId;
                    form.BranchName = model.BranchName;
                    form.RefundInfaverof = model.RefundInfaverof;
                    form.RefundaccountNo = model.RefundAccountNo;
                    form.IFSCCode = model.IFSCCode;
                    form.projectname = model.ProposedModel.ProposedProject;
                    form.projectcost = model.ProposedModel.TotalCost.ToString();
                    form.projecttimeempl = model.ProposedModel.ImplementationTime;
                    dbContext.SaveChanges();
                    //flag = UpdateDirectorDetailsForOpenSchemeForm(model.ApplicationFormId);        
                    model.IsDirectorDetailsSaved = true;
                }
                else
                {
                    model.IsDirectorDetailsSaved = false;
                }
                return model;
            }
        }

        public SWPFormViewModel SaveUploadedDocument(SWPFormViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files, System.Web.HttpPostedFileBase userImage, System.Web.HttpPostedFileBase signatureImage)
        {
            if (model.ApplicationFormId != null && model.ApplicationFormId > 0)
            {
                model.Id = model.ApplicationFormId;
                if (model.Id != null && model.Id > 0)
                {
                    int formId = model.Id.Value;
                    SaveDocumentsForApplicationForm(model, files, userImage, signatureImage);
                    //return formId;
                    return model;
                }
            }
            //return ReturnType.Failure;
            model.ReturnTypeId = SWPReturnTypeId.Failure;
            return model;
        }

        public int RemoveDocumentFromApplicationForm(string formNo, string filename)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                int flag = 0;
                if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formNo + "/Documents/")))
                {
                    var filepath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formNo + "/Documents/");
                    string[] files = Directory.GetFiles(filepath);
                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            var flst = file.Split('\\');
                            var nfileName = flst[flst.Length - 1];
                            var wFileName = nfileName.Split('.');
                            int CheckListId = Convert.ToInt32((wFileName[0].Split('-'))[1]);
                            if (flst[flst.Length - 1] == filename)
                            {
                                OnlineCheckLisTran onlineDocument = new OnlineCheckLisTran();
                                onlineDocument = dbContext.OnlineCheckLisTrans.Where(m => m.onlineapplicationId.ToString() == formNo && m.CheckListId == CheckListId).FirstOrDefault();
                                if (onlineDocument != null)
                                {
                                    dbContext.OnlineCheckLisTrans.Remove(onlineDocument);
                                    dbContext.SaveChanges();
                                }
                                File.Delete(file);
                                flag = SWPReturnTypeId.Success;
                            }
                        }
                    }
                }
                return flag;
            }
        }

        public SWPDirectorViewModel SaveDirectorDetailsForOpenScheme(SWPDirectorViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (model.ActionType == "Delete")
                {
                    var director = dbContext.online_Director_Master.FirstOrDefault(d => d.Director_Id == model.DirectorId);
                    director.Is_Active = 0;
                    dbContext.SaveChanges();
                    model.ActionTypeId = SWPReturnTypeId.Removed;
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
                    model.IsDirectorActive = 1;
                    model.ActionTypeId = SWPReturnTypeId.Saved;
                }

                return model;
            }
        }

        public DataSourceResult GetFloorAreaListAsDataSource(DataSourceRequest request, SWPDropdownViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var schemetype = dbContext.SchemeMsts.Where(s => s.schemeId == model.SchemeId).Select(x => x.SchemeTypeMst.SchemeTypeDesc).FirstOrDefault();
                var list = (from scheme in dbContext.SchemeCostTrans
                            where scheme.schemeId == model.SchemeId && scheme.departmentId == model.DepartmentId && scheme.IsActive == true
                            //&& scheme.FloorMst.modifiedBy == schemetype
                            select new SWPDropdownViewModel
                            {
                                Id = scheme.FloorMst.floorId,
                                Text = scheme.FloorMst.floorName
                            }).Distinct();
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetUploadedDocumentsByFormId(DataSourceRequest request, int? formId)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                List<SWPDocumentViewModel> docs = new List<SWPDocumentViewModel>();
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(f => f.onlineapplicationId == formId);
                //var documents = dbContext.OnlineCheckListMasters.ToList();
                if (form != null)
                {
                    docs = (from doc in dbContext.OnlineCheckListMasters
                            //join styp in dbContext.SchemeTypeMsts on doc.SchemeTypeId equals styp.schemeTypeId
                            where doc.IsActive == true && doc.SchemeId == form.schemeId //doc.CheckListType == "IP" //configSchemeId
                            select new SWPDocumentViewModel
                            {
                                Id = doc.CheckListId,
                                DocumentId = doc.CheckListId,
                                DocumentName = doc.CheckListName,
                                ParentDocumentType = doc.MainList
                            }).ToList();
                    if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + formId + "/Documents/")))
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
                                docs.Where(m => m.Id == nod).FirstOrDefault().DocumentPath = "/UploadFiles/" + formId + "/Documents/" + filename;
                                docs.Where(m => m.Id == nod).FirstOrDefault().UploadedDocument = filename;
                                //docs.ElementAt(nod - 1).PathName = "/UploadFiles/" + formId + "/Documents/" + filename;
                                // docs.ElementAt(nod - 1).UploadedDocument = filename;
                            }

                        }
                    }
                    int counter = 1;
                    docs.ForEach(x => x.SerialNo = counter++);
                }
                return docs.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetDirectorDetailsAsDataSourceByFormId(DataSourceRequest request, int? id)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var directorlist = dbContext.online_Director_Master.Where(d => d.onlineapplicationId == id).ToList();
                if (directorlist != null && directorlist.Count > 0)
                {
                    var Dirlist = (from directors in dbContext.online_Director_Master
                                   where directors.onlineapplicationId == id && directors.Is_Active == 1
                                   select new SWPDirectorViewModel
                                   {
                                       Id = directors.Director_Id,
                                       DirectorId = directors.Director_Id,
                                       ApplicationFormId = directors.onlineapplicationId,
                                       DirectorName = directors.Director_Name,
                                       DirectorShare = directors.Director_Share,
                                       PAN = directors.pan_no,
                                       DirectorTypeId = directors.Type,
                                       status = directors.Is_Active == 1 ? true : false,
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

        public IEnumerable<SWPDropdownViewModel> GetFloorAreaRangeList(SWPDropdownViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var schemetype = dbContext.SchemeMsts.Where(s => s.schemeId == model.SchemeId).Select(x => x.SchemeTypeMst.SchemeTypeDesc).FirstOrDefault();
                var lst = (from scheme in dbContext.SchemeCostTrans
                           where scheme.schemeId == model.SchemeId && scheme.departmentId == model.DepartmentId && scheme.IsActive == true
                           && scheme.FloorMst.modifiedBy == schemetype
                           select new SWPDropdownViewModel
                           {
                               Id = scheme.FloorMst.floorId,
                               Text = scheme.FloorMst.floorName
                           }).ToList();
                return lst;
            }
        }

        public SWPFormViewModel GetApplicationFormFeeAndProcessingCharge(SWPFormViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var scheme = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId == model.SchemeId);
                var schemecost = dbContext.SchemeCostTrans.FirstOrDefault(c => c.schemeId == model.SchemeId && c.departmentId == model.DepartmentId && c.floorId == model.AreaRangeTypeId);
                var online = dbContext.OnlineAreawithRegisandProcfees.FirstOrDefault(r => r.schemeId == model.SchemeId);
                if (schemecost != null)
                {
                    model.ApplicationFee = scheme.FormFee;
                    model.FormFeeGST = scheme.FormCGST + scheme.FormSGST;
                    model.ProcessingCharge = scheme.ProcessingFee;
                    model.ProcessingChargeGST = scheme.ProcessingCGST + scheme.ProcessingSGST;
                    model.EarnestMoney = schemecost.earnestMoney;
                }
                if (online != null)
                {
                    model.ApplicationFee = online.ApplicationFee;
                    model.ProcessingCharge = online.ProcessingCharge;
                }
                return model;
            }
        }

        public SWPFormViewModel ValidateFormInputFieldByType(SWPFormViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (model.ActionType == "PAN")
                {
                    if (model.ApplicationFormId == null)
                    {
                        var form = dbContext.OnlineApplicationDetails.FirstOrDefault(o => o.isActive == true && o.pan.ToUpper() == model.PanNumber.ToUpper() && o.area == model.AreaRangeId.ToString() && o.schemeId == model.SchemeId);
                        model.ReturnTypeId = form == null ? SWPReturnTypeId.NotExist : SWPReturnTypeId.Exist;
                    }
                    else if (model.ApplicationFormId != null && model.ApplicationFormId > 0)
                    {
                        var form = dbContext.OnlineApplicationDetails.FirstOrDefault(a => a.onlineapplicationId == model.ApplicationFormId);
                        if (form != null && form.pan != model.PanNumber)
                        {
                            model.ReturnTypeId = SWPReturnTypeId.Mismatch;
                        }
                        else
                        {
                            model.ReturnTypeId = SWPReturnTypeId.Success;
                        }
                    }
                }
                
                return model;
            }
        }

        public int GetProcessingAndReservationMoneyPaymentStatus(string formNo)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                int flag = 0;
                int formId = Convert.ToInt32(formNo);
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(f => f.onlineapplicationId == formId);
                var payment = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(o => o.ServiceRefId == formId && o.status == 1 && o.ServiceType == 1);
                var payment_off = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(o => o.ServiceRefId == formId && o.ServiceType == 3);

                if (form != null)
                {
                    if (payment == null && payment_off == null)
                    {
                        flag = SWPReturnTypeId.None;
                    }
                    else flag = SWPReturnTypeId.Exist;
                }
                else flag = SWPReturnTypeId.NotExist;

                return flag;
            }
        }

        public SWPFormViewModel GetBankAccountDetails(SWPFormViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var accounts = (from prop in dbContext.SchemeBankTrans
                                where prop.IsActive == true && prop.schemeId == model.SchemeId && prop.bankId == model.BankId
                                select new SWPFormViewModel
                                {
                                    BranchId = prop.BranchMst.branchId,
                                    BranchName = prop.BranchMst.branchName,
                                    AccountNo = prop.accountnumber,
                                    IFSCCode = prop.IFSCCode,
                                    BankIFSCCode = prop.IFSCCode,
                                    VirtualAccountPrefix = prop.virtualAccountprefix
                                }).FirstOrDefault();
                return accounts;
            }
        }

        public SWPChallanViewModel SaveOfflinePaymentTransactionForChallan(SWPFormViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var onlineApplication = dbContext.OnlineApplicationDetails.Where(m => m.onlineapplicationId == model.ApplicationFormId && m.isActive == true).FirstOrDefault();
                if (onlineApplication != null)
                {
                    var transaction = dbContext.OnlineApplicationDetails_trans.FirstOrDefault(f => f.ServiceRefId == model.ApplicationFormId && f.ServiceType != 1);
                    if (transaction == null)
                    {
                        OnlineApplicationDetails_trans trans = new OnlineApplicationDetails_trans
                        {
                            ServiceRefId = model.ApplicationFormId,
                            txnid = model.ApplicationFormId.ToString() + "T01",
                            Amount = onlineApplication.ProcessingCharge + onlineApplication.EarnestMoney + (2 * onlineApplication.ProcessingCGST), //onlineApplication.TotalAmount,
                            productinfo = "RTGS/NEFT",
                            mode = "RTGS/NEFT",
                            ServiceType = SWPConstant.offlineApplicationPayment,
                            GetwayName = model.ChallanBankId.ToString(),
                            udf5 = model.BankIFSCCode,
                            virtualaccountno = model.VirtualAccPrefix + model.ApplicationFormId.ToString() + "T01",
                            status = 0,
                            EntryDate = DateTime.Now
                        };
                        dbContext.OnlineApplicationDetails_trans.Add(trans);
                        dbContext.SaveChanges();
                    }
                }

                var cmodel = (from appl in dbContext.OnlineApplicationDetails
                              join scme in dbContext.SchemeMsts on appl.schemeId equals scme.schemeId
                              join trans in dbContext.OnlineApplicationDetails_trans on appl.onlineapplicationId equals trans.ServiceRefId
                              where appl.onlineapplicationId == model.ApplicationFormId && trans.ServiceType == 3
                              && scme.IsActive == true && scme.completed == true
                              select new SWPChallanViewModel
                              {
                                  FormModel = new SWPFormViewModel
                                  {
                                      ApplicationFormId = appl.onlineapplicationId,
                                      Applicant = appl.gender != SWPConstant.genderCompany ? appl.firstName + " " + (!string.IsNullOrEmpty(appl.middleName) ? appl.middleName + " " + appl.lastName : appl.lastName) : appl.CompanyName,
                                      Email = appl.email,
                                      MobileNumber = appl.mobileNumberP2,
                                      CorrespondingAddress = appl.correspondanceAdd,
                                      ApplicationFee = 0, //appl.ApplicationFee,
                                      ProcessingCharge = appl.ProcessingCharge,
                                      FormFeeGST = 0, // appl.ApplicationFee + appl.FormCGST + appl.FormSGST,
                                      ProcessingChargeGST = appl.ProcessingCharge + appl.ProcessingSGST + appl.ProcessingCGST,
                                      FormFeeSGST = 0,// appl.FormSGST,
                                      FormFeeCGST = 0,// appl.FormCGST,
                                      ProcessingSGST = appl.ProcessingSGST,
                                      ProcessingCGST = appl.ProcessingCGST,
                                      TotalAmount = (appl.ProcessingCharge + appl.ProcessingSGST + appl.ProcessingCGST) + appl.EarnestMoney, // application fee paid via nic or nivesh mitra 
                                      EarnestMoney = appl.EarnestMoney,
                                      TotalAmountGST = (appl.ProcessingCharge + appl.ProcessingSGST + appl.ProcessingCGST) + appl.EarnestMoney, // application fee paid via nic or nivesh mitra 
                                      ChallanBank = dbContext.BankMsts.Where(m => m.bankId.ToString() == trans.GetwayName && m.IsActive == true).FirstOrDefault().bankName + " Branch : " + dbContext.SchemeBankTrans.Where(m => m.bankId.ToString() == trans.GetwayName && m.schemeId == appl.schemeId && m.IsActive == true).FirstOrDefault().BranchMst.branchName,
                                      AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == appl.area && m.IsActive == true).FirstOrDefault().floorName,
                                      SchemeName = dbContext.SchemeMsts.Where(m => m.schemeId == appl.schemeId && m.IsActive == true && m.completed == true && m.Status != SWPConstant.SchemeClosed).FirstOrDefault().schemeName,
                                      Department = dbContext.DepartmentMsts.Where(m => m.departmentId == appl.departmentId && m.IsActive == true).FirstOrDefault().departmentName,
                                      //In offline case BankId place in gateway name , Branch Id Place in Bank code and Account Place in Card Num Field in OnlineApplicationDetails_trans table 23 aug 2017 According to vishal Shukla Sir
                                      ChallanAccountNo = trans.cardnum,
                                      ChallanBranchName = dbContext.BankMsts.Where(m => m.bankId.ToString() == trans.GetwayName && m.IsActive == true).FirstOrDefault().bankName + " " + dbContext.BranchMsts.Where(m => m.branchId.ToString() == trans.bankcode && m.IsActive == true).FirstOrDefault().branchName,
                                      //In Case of applicant GST No saved in fax no 30 Aug 2017
                                      ApplicantGSTNumber = appl.GSTNO,
                                      BankIFSCCode = trans.udf5,
                                      VirtualAccPrefix = trans.virtualaccountno,
                                      ValidTillDate = scme.endDate
                                  },
                                  PaymentModel = new SWPPaymentViewModel
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
                return cmodel;
            }
        }

        public SWPFormViewModel SaveOnlineSchemePaymentStatus(SWPFormViewModel model, System.Web.HttpPostedFileBase challan)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (model.ActionType == "UpdateChallan")
                {
                    var data = (from transaction in dbContext.OnlineApplicationDetails_trans
                                join details in dbContext.OnlineApplicationDetails on transaction.ServiceRefId equals details.onlineapplicationId
                                where transaction.ServiceType == 3 && transaction.status == 0 && details.isActive == true && details.onlineapplicationId == model.PaymentModel.ApplicationFormId
                                select transaction).FirstOrDefault();
                    if (data != null)
                    {
                        if (data.txnid == model.PaymentModel.TransactionId)
                        {
                            var trans = dbContext.OnlineApplicationDetails_trans.Where(m => m.AutoID == data.AutoID && m.status == 0).FirstOrDefault();
                            if (trans != null)
                            {
                                //SaveChallanDocumentsForApplicationForm(objOnlineFormViewModel, files);

                                if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.PaymentModel.ApplicationFormId)))
                                {
                                    if (challan != null && challan.ContentLength > 0)
                                    {
                                        string directory = "Challan";
                                        if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.PaymentModel.ApplicationFormId + "/" + directory)))
                                        {
                                            Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.PaymentModel.ApplicationFormId + "/" + directory));
                                        }
                                        string extension = Path.GetExtension(challan.FileName);
                                        var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.PaymentModel.ApplicationFormId + "/" + directory + "/" + model.PaymentModel.ApplicationFormId + "-" + model.PaymentModel.TransactionId + extension);
                                        challan.SaveAs(fileSavePath);

                                        model.IsPaidChallanUploaded = true;
                                    }
                                }

                                trans.TrKey = model.PayType == "RTGS" ? model.PaymentModel.Udf1 : model.PaymentModel.BankReferenceNo;
                                trans.modifiydate = DateTime.Now;//objOnlineFormViewModel.PaymentModel.EntryDate;
                                trans.TranStatus = 1;
                                dbContext.SaveChanges();
                                model.ReturnTypeId = SWPReturnTypeId.Success;
                                model.Message = "Challan Uploaded Successfully";
                            }
                        }
                        else
                        {
                            model.ReturnTypeId = SWPReturnTypeId.Mismatch;
                            model.ErrorMessage = "TransactionId mismatch.";
                            model.Message = "TransactionId mismatch.";
                        }
                    }
                    else
                    {
                        model.ReturnTypeId = SWPReturnTypeId.NotExist;
                        model.ErrorMessage = "Payment record is not available.";
                        model.Message = "Payment record is not available.";
                    }
                }
                else
                {
                    if (model.NICApplicationId != null && model.NICControlId != null && model.NICUnitId != null && model.NICServiceId != null)
                    {
                        var exService = dbContext.NICsingalwindowSystems.FirstOrDefault(c => c.onlineapplicationId.ToString() == model.NICApplicationId && c.Control_ID == model.NICControlId && c.Unit_Id == model.NICUnitId);
                        if (exService != null)
                        {
                            exService.Status_Code = Convert.ToInt32(SWPStatus.FEE_PAID);
                            exService.Fee_Status = SWPStatus.Wording.FEE_PAID;
                            dbContext.SaveChanges();
                            model.NICFeeStatus = SWPStatus.Wording.FEE_PAID;
                            model.NICFeeStatusId = SWPStatus.FEE_PAID;
                        }
                    }
                    else
                    {
                        var exPayment = dbContext.OnlineApplicationDetails_trans.Where(c => c.ServiceRefId == model.ApplicationFormId).OrderByDescending(o => o.AutoID).FirstOrDefault();
                        model.IsApplicationFeePaid = (exPayment != null && exPayment.status == 1) ? true : false;
                    }
                }
                return model;
            }
        }

        public Kendo.Mvc.UI.DataSourceResult GetOnlineSchemeListAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, SWPDropdownViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var list = (from scheme in dbContext.SchemeMsts
                            where scheme.Status == SWPConstant.SchemeOpen && scheme.IsActive == true
                            && scheme.SchemeTypeMst.modifiedBy == SWPConstant.SchemeOnline
                            select new SWPDropdownViewModel
                            {
                                Id = scheme.schemeId,
                                Text = scheme.schemeName
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public Kendo.Mvc.UI.DataSourceResult GetDepartmentBySchemeAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, SWPDropdownViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var departments = (from department in dbContext.DepartmentMsts
                                   join depttrans in dbContext.SchemeDepartmentTrans on department.departmentId equals depttrans.departmentId
                                   where department.IsActive == true && depttrans.IsActive == true
                                  && DepartmentList.Contains(department.departmentId)
                                  && (model.SchemeId == null || depttrans.schemeId == model.SchemeId)
                                   select new SWPDropdownViewModel
                                   {
                                       Id = department.departmentId,
                                       Text = department.departmentName,
                                       SchemeId = depttrans.schemeId != null ? (int)depttrans.schemeId : 0,
                                       Value = string.Empty
                                   });
                return departments.ToDataSourceResult(request);
            }
        }

        public Kendo.Mvc.UI.DataSourceResult GetOnlineSchemeApplicationAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, SWPFormViewModel modal)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                string userRole = userInfo.RoleMaster.RoleType;

                if (modal.SchemeId == null)
                {
                    modal.SchemeId = Convert.ToInt32(ConfigurationManager.AppSettings["CurrentSchemeId"]);
                }
                if (userRole == SWPConstant.Admin || userInfo.UserID == 1222)
                {
                    //int MoveToOSD = Convert.ToInt32(strOnlineApplicationProcess.MoveToOSD);
                    //int Scrutiny = Convert.ToInt32(strOnlineApplicationProcess.Scrutiny);
                    //int Draw = Convert.ToInt32(strOnlineApplicationProcess.Draw);
                    //int ApprovalCEO = Convert.ToInt32(strOnlineApplicationProcess.ApprovalCEO);
                    var Online_trans = (from oam in dbContext.OnlineApplicationDetails
                                        //from pmt in dbContext.OnlineApplicationDetails_trans.Where(p=>p.ServiceRefId==oam.onlineapplicationId).OrderByDescending(o=>o.AutoID).DefaultIfEmpty()
                                        from nic in dbContext.NICsingalwindowSystems.Where(n => n.onlineapplicationId == oam.onlineapplicationId).DefaultIfEmpty()
                                        from sch in dbContext.SchemeMsts.Where(s => s.schemeId == oam.schemeId).DefaultIfEmpty()
                                        from deptt in dbContext.DepartmentMsts.Where(d => d.departmentId == oam.departmentId).DefaultIfEmpty()
                                        where DepartmentList.Contains(oam.departmentId) && nic.Status_Code == 11
                                        && (modal.Id == null || oam.onlineapplicationId == modal.Id)
                                        && (modal.DepartmentId == null || oam.departmentId == modal.DepartmentId)
                                        && (modal.SchemeId == null || oam.schemeId == modal.SchemeId)
                                        select new SWPFormViewModel
                                        {
                                            Id = oam.onlineapplicationId,
                                            ApplicationFormId = oam.onlineapplicationId,
                                            SchemeName = sch.schemeName,
                                            Department = deptt.departmentName,
                                            NICControlId = nic.Control_ID,
                                            NICUnitId = nic.Unit_Id,
                                            NICServiceId = nic.ServiceID,
                                            ServiceAppPayStatus = nic.Fee_Status,
                                            IsApplicationFeePaid = nic.Status_Code == 11 ? true : false,
                                            Applicant = string.IsNullOrEmpty(oam.CompanyName) ? oam.firstName + " " + (!string.IsNullOrEmpty(oam.middleName) ? oam.middleName + " " + oam.lastName : oam.lastName) : oam.CompanyName,
                                            FirstName = string.IsNullOrEmpty(oam.CompanyName) ? oam.firstName + " " + (!string.IsNullOrEmpty(oam.middleName) ? oam.middleName + " " + oam.lastName : oam.lastName) : oam.CompanyName,
                                            Gender = oam.gender,
                                            DOB = oam.dateOfBirth,
                                            FormStatus = oam.IsSubmited == null ? SWPStatus.Form.Rejected : (oam.IsSubmited == true ? SWPStatus.Form.Accepted : SWPStatus.Form.InProgress),
                                            IsDeleted = oam.isActive == null ? "" : (oam.isActive == true ? SWPStatus.Form.Accepted : SWPStatus.Form.Rejected),
                                            IsFormRejected = oam.isActive == false ? true : false,
                                            TotalAmount = oam.TotalAmount,
                                            SubmitDate = oam.createdDate,
                                            AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == oam.area && m.IsActive == true).FirstOrDefault().floorName,
                                            ApplicantType = oam.gender == "Company" ? "Company" : "Individual",
                                            AmountPaidDate = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().EntryDate,
                                            ChallanBank = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.SchemeBankTrans.FirstOrDefault(m => m.schemeId == oam.schemeId && m.IsActive == true && m.bankId.ToString() == (dbContext.OnlineApplicationDetails_trans.FirstOrDefault(i => i.ServiceRefId == oam.onlineapplicationId && i.ServiceType == 3).GetwayName)).BankMst.bankName) : "--",
                                            //PayType = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? "Online" : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? "Offline (RTGS/NEFT)" : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 4 ? "Previous Challan" : "--")),
                                            PayType = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? "Online" : "Challan",
                                            //ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().TrKey) ? "Updated" : "Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "Paid" : "Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 4 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "PC Paid" : "PC Not Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 5 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().status == 1 ? "SWP Paid" : "SWP Not Paid") : "--"))),
                                            ChallanStatus = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.AutoID).FirstOrDefault().ServiceType == 3 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.AutoID).FirstOrDefault().status == 0 ? ((!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.AutoID).FirstOrDefault().TrKey) ? "Updated" : "Generated")) : "Paid") : (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.AutoID).FirstOrDefault().ServiceType == 1 ? (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.AutoID).FirstOrDefault().status == 1 ? "Online Paid" : "Not Paid") : "---"),

                                            AccountNo = dbContext.OnlineApplicationDetails_trans.Where(a => a.ServiceRefId == oam.onlineapplicationId).OrderByDescending(o => o.AutoID).FirstOrDefault().virtualaccountno,
                                            ApplicationFormType = !string.IsNullOrEmpty(oam.FormCategory) ? (oam.FormCategory) : "General",
                                            ExpansionType = !string.IsNullOrEmpty(oam.FormCategory) ? ((oam.FormCategory == SWPConstant.ApplicationFormTypeEx) ? oam.FormSubCategory : "--") : "--",
                                            ProcessType = oam.OnlineApplicationProcessDetails.FirstOrDefault() != null ? (oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == SWPConstant.MoveToOSD) != null ? "Move To OSD" : oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == SWPConstant.Scrutiny) != null ? "Scrunity" : oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == SWPConstant.ApprovalCEO) != null ? "Approval for CEO" : oam.OnlineApplicationProcessDetails.FirstOrDefault(m => m.ApplicationStatus == SWPConstant.Draw) != null ? "Draw" : string.Empty) : string.Empty,
                                            IsRegistrationFeePaid = dbContext.OnlineApplicationDetails_trans.Where(t => t.ServiceRefId == oam.onlineapplicationId).OrderByDescending(d => d.AutoID).FirstOrDefault().TranStatus == 1 ? true : false,
                                            IsPaidChallanUploaded = (!string.IsNullOrEmpty(dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.AutoID).FirstOrDefault().TrKey) && (dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == oam.onlineapplicationId).OrderByDescending(m => m.EntryDate).FirstOrDefault().ServiceType == 3)) ? true : false,
                                            IsPropertyAllotted = dbContext.ApplicationDetails.FirstOrDefault(a => a.formNo == oam.onlineapplicationId.ToString()) != null ? true : false,
                                            IsDrawSucceeded = true
                                        });

                    if (modal.PayType == "2")
                    {
                        Online_trans = Online_trans.Where(m => m.IsRegistrationFeePaid == true);
                    }
                    if (modal.PayType == SWPStatus.Payment.Paid)
                    {
                        Online_trans = Online_trans.Where(m => m.IsRegistrationFeePaid == true);
                    }
                    if (modal.PayType == "3")
                    {
                        Online_trans = Online_trans.Where(m => m.ChallanStatus == "Generated" || m.ChallanStatus == "Not Paid");
                    }
                    if (modal.PayType == "4")
                    {
                        Online_trans = Online_trans.Where(m => m.PayType == "Online");
                    }
                    if (modal.PayType == "5")
                    {
                        Online_trans = Online_trans.Where(m => m.ChallanStatus == "Online Paid");
                    }
                    if (modal.PayType == "6")
                    {
                        Online_trans = Online_trans.Where(m => m.PayType == "Challan");
                    }
                    if (modal.PayType == "7")
                    {
                        Online_trans = Online_trans.Where(m => m.ChallanStatus == "Paid" && m.PayType == "Challan");
                    }
                    if (modal.PayType == "8")
                    {
                        Online_trans = Online_trans.Where(m => m.ChallanStatus == "Generated");
                    }
                    if (modal.PayType == "9")
                    {
                        Online_trans = Online_trans.Where(m => m.ChallanStatus == "Updated");
                    }
                    return Online_trans.ToDataSourceResult(request);
                }
                else
                {
                    int? area = (userInfo.OptionalId == null || userInfo.OptionalId == 0) ? 0 : userInfo.OptionalId;
                    var Online = (from oam in dbContext.OnlineApplicationDetails
                                  join oad_t in dbContext.OnlineApplicationDetails_trans on oam.onlineapplicationId equals oad_t.ServiceRefId
                                  join sch in dbContext.SchemeMsts on oam.schemeId equals sch.schemeId
                                  join deptt in dbContext.DepartmentMsts on oam.departmentId equals deptt.departmentId
                                  from floor in dbContext.FloorMsts.Where(f => f.floorId.ToString() == oam.area).DefaultIfEmpty()
                                  where ((oam.isActive == true && oad_t.status == 1) || oad_t.ServiceType == 4)
                                  && DepartmentList.Contains(oam.departmentId)
                                  && (modal.Id == null || oam.onlineapplicationId == modal.Id)
                                  && (modal.DepartmentId == null || oam.departmentId == modal.DepartmentId)
                                  && (modal.SchemeId == null || oam.schemeId == modal.SchemeId)
                                  && (area == 0 || oam.area == area.ToString())
                                  select new SWPFormViewModel
                                  {
                                      Id = oam.onlineapplicationId,
                                      ApplicationFormId = oam.onlineapplicationId,
                                      SchemeName = sch.schemeName,
                                      Department = deptt.departmentName,
                                      FirstName = string.IsNullOrEmpty(oam.CompanyName) ? oam.firstName + " " + (!string.IsNullOrEmpty(oam.middleName) ? oam.middleName + " " + oam.lastName : oam.lastName) : oam.CompanyName,
                                      Gender = oam.gender,
                                      DOB = oam.dateOfBirth,
                                      FormStatus = oam.IsSubmited == null ? SWPStatus.Form.Rejected : (oam.IsSubmited == true ? SWPStatus.Form.Accepted : SWPStatus.Form.InProgress),
                                      AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == oam.area && m.IsActive == true).FirstOrDefault().floorName,
                                      TotalAmount = (decimal)oam.TotalAmount,//Amount Paid
                                      SubmitDate = oam.createdDate,
                                      AmountPaidDate = oad_t.EntryDate,
                                      ApplicantType = oam.gender == "Company" ? "Company" : "Individual",
                                      PayType = oad_t.ServiceType == 1 ? "Online" : (oad_t.ServiceType == 3 ? "Offline (RTGS/NEFT)" : "--"),
                                      ChallanStatus = oad_t.ServiceType == 3 ? (oad_t.status == 0 ? ((!string.IsNullOrEmpty(oad_t.TrKey) ? "Challan Updated" : "Challan Generated")) : "Paid") : (oad_t.ServiceType == 1 ? (oad_t.status == 1 ? "Paid" : "Not Paid") : "--"),
                                      ApplicationFormType = !string.IsNullOrEmpty(oam.FormCategory) ? (oam.FormCategory) : "--",
                                      ExpansionType = !string.IsNullOrEmpty(oam.FormCategory) ? ((oam.FormCategory == SWPConstant.ApplicationFormTypeEx) ? oam.FormSubCategory : "--") : "--"
                                  });
                    return Online.ToDataSourceResult(request);
                }
            }
        }

        public Kendo.Mvc.UI.DataSourceResult GetOnlineSchemeFormPaymentAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, SWPFormViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var Online = (from payment in dbContext.OnlineApplicationDetails_trans
                              where payment.ServiceRefId == model.ApplicationFormId
                              select new SWPPaymentViewModel
                              {
                                  Id = payment.ServiceRefId,
                                  ApplicationFormId = payment.ServiceRefId,
                                  //IsApplicationFeePaid = payment.TranStatus == 1 ? true : false,
                                  Amount = (decimal)payment.Amount,
                                  TransactionKey = payment.TrKey,
                                  TransactionId = payment.txnid,
                                  PaymentTypeId = payment.ServiceType,
                                  PaymentType = payment.ServiceType == 1 ? "Online" : (payment.ServiceType == 3 ? "Challan" : "Other"),
                                  TransactionStatus = payment.TranStatus == 1 ? "Paid" : "Not Paid",
                                  TransactionStatusId = payment.TranStatus,
                                  EntryDate = payment.EntryDate,
                                  StatusId = payment.status,
                                  Status = payment.status == 1 ? "Paid" : "Not Paid",
                                  VirtualAccountNo = payment.virtualaccountno,
                                  ProductInfo = payment.productinfo,
                                  GatewayName = payment.GetwayName == "67" ? "HDFC" : "PAYU"
                              });
                return Online.ToDataSourceResult(request);
            }
        }

        public SWPPaymentViewModel GetSchemeFormPaymentTransaction(SWPPaymentViewModel model)
        {
            SWPPaymentViewModel payment = new SWPPaymentViewModel();
            using (var dbContext = new PIMSEntitiesContext())
            {
                payment = (from transaction in dbContext.OnlineApplicationDetails_trans
                           join details in dbContext.OnlineApplicationDetails on transaction.ServiceRefId equals details.onlineapplicationId
                           where transaction.ServiceType == 3 && transaction.status == 0 && details.isActive == true && details.onlineapplicationId == model.ApplicationFormId
                           select new SWPPaymentViewModel
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
                               Applicant = details.gender == SWPConstant.genderCompany ? details.CompanyName : details.firstName + " " + details.middleName + "" + details.lastName,
                               FirstName = details.gender == SWPConstant.genderCompany ? details.CompanyName : details.firstName + " " + details.middleName + "" + details.lastName,
                               Email = details.email,
                               PhoneNumber = details.mobileNumberP1,
                               Status = transaction.TranStatus == SWPConstant.Success ? "Success" : "Failure",
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
                               FormModel = new SWPFormViewModel
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
                //challan uploaded file path
                string directory = ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.ApplicationFormId + "/" + "Challan" + "/";
                if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(directory)))
                {
                    var filepath = System.Web.HttpContext.Current.Server.MapPath(directory);
                    string[] files = Directory.GetFiles(filepath);
                    if (files != null)
                    {
                        if (files.Count() > 0)
                        {
                            string[] uFile = files[0].Split('\\');
                            string fileName = uFile[(uFile).Length - 1];
                            payment.docPath = "/UploadFiles/" + model.ApplicationFormId + "/" + "Challan" + "/" + fileName;
                        }
                    }
                }
            }
            return payment;
        }

        public SWPPaymentViewModel ValidateSchemeFormChallan(SWPPaymentViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                model.ReturnTypeId = SWPReturnTypeId.None;
                if (model.ActionType == "ValidateAll")
                {
                    //var formlist = (from oaf in dbContext.OnlineApplicationDetails
                    //                from pmt in dbContext.OnlineApplicationDetails_trans.Where(x => x.ServiceRefId == oaf.onlineapplicationId).OrderByDescending(o => o.AutoID).DefaultIfEmpty()
                    //                join nic in dbContext.NICsingalwindowSystems on oaf.onlineapplicationId equals nic.onlineapplicationId
                    //                where oaf.schemeId == model.Id && nic.Status_Code == 11 && pmt.ServiceType == 3 && pmt.TrKey != null && pmt.TranStatus == 1 //&& pmt.status == 1
                    //                select new OnlineFormViewModel
                    //                {
                    //                    ApplicationFormId = oaf.onlineapplicationId
                    //                }).ToList();
                    //var formlist = (from oaf in dbContext.OnlineApplicationDetails
                    //                where oaf.schemeId == 165165 && oaf.paymentTypeId == "3"
                    //                select new OnlineFormViewModel
                    //                {
                    //                    ApplicationFormId = oaf.onlineapplicationId
                    //                }).ToList();

                    List<int> formlist = dbContext.Database.SqlQuery<int>("select onlineapplicationId from SchemeIndSmsSend").ToList();

                    foreach (var item in formlist)
                    {
                        //var SchemeForm = dbContext.OnlineApplicationDetails.Where(us => us.onlineapplicationId == item).FirstOrDefault();
                        //var mobileNo = dbContext.Database.SqlQuery<string>("select MobileNo from SchemeIndSmsSend where onlineapplicationid="+item).FirstOrDefault();
                        var mobileNo = dbContext.OnlineApplicationDetails.FirstOrDefault(us => us.onlineapplicationId == item).mobileNumberP2;
                        var emailId = dbContext.Database.SqlQuery<string>("select email from SchemeIndSmsSend where onlineapplicationid=" + item).FirstOrDefault();
                        var status = dbContext.Database.SqlQuery<string>("select ChallanStatus from SchemeIndSmsSend where onlineapplicationid=" + item).FirstOrDefault();
                        var name = dbContext.Database.SqlQuery<string>("select MobileNo from SchemeIndSmsSend where onlineapplicationid=" + item).FirstOrDefault();
                        string ebody = string.Empty;
                        string smsbody1 = string.Empty;
                        string smsbody2 = string.Empty;
                        string message = string.Empty;

                        //smsbody1 = string.Format(SWPMessage.IPSchemePaymentInfoU1, SchemeForm.onlineapplicationId, "NOIDA/IP/2019-20/03");
                        //smsbody2 = string.Format(SWPMessage.IPSchemePaymentInfoU2, "industry@noidaauthorityonline.com", "06-03-2020");

                        smsbody1 = string.Format(SWPMessage.SMS_StatusAfterPaymentValidation, "NOIDA/IP/2019-20/03");

                        //ebody = string.Format(SWPMessage.PaymentMismatchEmail, "06-03-2020", "industry@noidaauthorityonline.com", "06-03-2020 1800 hrs");
                        //ebody = string.Format(SWPMessage.Email_StatusAfterPaymentValidation, "NOIDA/IP/2019-20/03",);
                        //message = "Duplicate Form Payment";
                        ebody = string.Format(SWPMessage.Email_StatusAfterPaymentValidation, "NOIDA/IP/2019-20/03", status, "industry@noidaauthorityonline.com");
                        message = "Payment Status after validation";

                        if (!string.IsNullOrEmpty(ebody))
                        {
                            if (!string.IsNullOrEmpty(mobileNo)) { SWPApplication.SendSMS(mobileNo, smsbody1); }
                            //if (!string.IsNullOrEmpty(SchemeForm.mobileNumberP2)) { ApplicationHelper.SendSMS(SchemeForm.mobileNumberP2, smsbody2); }

                            if (!string.IsNullOrEmpty(emailId)) { SWPApplication.SendEmail(emailId, message, ebody); }
                        }

                        //var Payment = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == SchemeForm.onlineapplicationId && m.ServiceType == 3).FirstOrDefault();
                        //if (Payment != null)
                        //{
                        //    Payment.status = 1;
                        //    dbContext.SaveChanges();
                        //    body = string.Format(SWPMessage.PaymentMismatchEmail, "06-03-2020", "industry@noidaauthorityonline.com", "06-03-2020 1800 hrs");
                        //    message = "Online Scheme Form Payment Validation";

                        //    if (!string.IsNullOrEmpty(body))
                        //    {
                        //        //if (!string.IsNullOrEmpty(SchemeForm.email)) { ApplicationHelper.SendEmail(SchemeForm.email, message, body); }
                        //        //if (!string.IsNullOrEmpty(SchemeForm.mobileNumberP2)) { ApplicationHelper.SendSMS(SchemeForm.mobileNumberP2, body); }

                        //        if (!string.IsNullOrEmpty(SchemeForm.email)) { ApplicationHelper.SendEmail(SchemeForm.email, message, body); }
                        //        if (!string.IsNullOrEmpty(SchemeForm.mobileNumberP2)) { ApplicationHelper.SendSMS(SchemeForm.mobileNumberP2, body); }
                        //    }
                        //}


                        var audit = new Audit();
                        audit.TableName = "OnlineApplicationDetail";
                        audit.Type = "S";
                        audit.FormName = item.ToString();
                        //audit.FormName = "ManageEmail";
                        audit.PrimaryKeyField = "Payment Validation Info";
                        audit.PrimaryKeyValue = smsbody1;
                        audit.FieldName = mobileNo;
                        audit.OldValue = ebody;
                        audit.NewValue = smsbody1;
                        audit.UpdateDate = DateTime.Now;
                        audit.UserName = userInfo.UserID.ToString();
                        dbContext.Audits.Add(audit);

                        //form.IsSubmited = true;

                        dbContext.SaveChanges();

                    }
                    model.ReturnTypeId = SWPReturnTypeId.Updated;
                }
                else
                {
                    var SchemeForm = dbContext.OnlineApplicationDetails.Where(us => us.onlineapplicationId == model.ApplicationFormId && us.isActive == true).FirstOrDefault();
                    if (SchemeForm != null)
                    {
                        //check service type 
                        int ServiceType = 0;
                        if (model.PaymentTypeId == SWPConstant.offlinePrevoiusChallanApplicationPayment)
                        {
                            ServiceType = SWPConstant.offlinePrevoiusChallanApplicationPayment;
                        }

                        else { ServiceType = SWPConstant.offlineApplicationPayment; }
                        var Details_trans = dbContext.OnlineApplicationDetails_trans.Where(m => m.ServiceRefId == SchemeForm.onlineapplicationId && m.ServiceType == ServiceType && m.status == 0).FirstOrDefault();
                        if (Details_trans != null)
                        {
                            Details_trans.status = 1;
                            Details_trans.TranStatus = 1;
                            dbContext.SaveChanges();
                            model.ReturnTypeId = SWPReturnTypeId.Updated;

                            string body = string.Empty;
                            string message = string.Empty;
                            if (model.PaymentTypeId == SWPConstant.offlineApplicationPayment || model.PaymentTypeId == SWPConstant.offlinePrevoiusChallanApplicationPayment)
                            {
                                body = string.Format(SWPMessage.OnlineApplicationChallanStatus, SchemeForm.onlineapplicationId);
                                message = "Online Application Challan Validated";
                            }

                            if (!string.IsNullOrEmpty(body))
                            {
                                if (!string.IsNullOrEmpty(SchemeForm.email)) { SWPApplication.SendEmail(SchemeForm.email, message, body); }
                                if (!string.IsNullOrEmpty(SchemeForm.mobileNumberP2)) { SWPApplication.SendSMS(SchemeForm.mobileNumberP2, body); }
                            }
                        }
                    }
                }

                return model;
            }
        }

        public SWPFormViewModel GetOnlineSchemeFormDataById(SWPFormViewModel form)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                SWPFormViewModel applicant = new SWPFormViewModel();
                var detail = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == form.ApplicationFormId && c.isActive == true);
                if (detail != null)
                {
                    form.Id = detail.onlineapplicationId;
                    form.ApplicationFormId = detail.onlineapplicationId;
                    form.UserPassword = "Noida"; //user password for login existing application
                    form.SchemeId = detail.schemeId;
                    form.SchemeName = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId == form.SchemeId).schemeName;
                    form.SchemeEndDate = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId == form.SchemeId).endDate;
                    form.SchemeType = dbContext.SchemeMsts.FirstOrDefault(c => c.schemeId == detail.schemeId).SchemeTypeMst.SchemeTypeDesc;
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
                    form.Religion = detail.religionId != null ? dbContext.ReligionMsts.Where(r => r.religionId == detail.religionId).Select(r => r.religion).FirstOrDefault() : null;
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

                    if (form.Gender.ToLower() == SWPConstant.Company.ToLower())
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

                    form.DirectorModel = new SWPDirectorViewModel
                    {
                        DirectorType = (detail.gender == "Company" && detail.occupationId != null) ? dbContext.Common_Config.Where(m => m.Id == detail.occupationId).FirstOrDefault().Name : string.Empty
                    };
                    form.ProposedModel = new SWPCompanyViewModel
                    {
                        ProposedProject = detail.projectname,
                        ImplementationTime = detail.projecttimeempl,
                        TotalCost = detail.projectcost
                    };
                    form.IsProposedProjectSaved = (!string.IsNullOrEmpty(detail.projectname) && !string.IsNullOrEmpty(detail.projecttimeempl) && !string.IsNullOrEmpty(detail.projectcost)) ? true : false;

                    form.PreviousFormNo = detail.PreviousFormNo != null ? detail.PreviousFormNo : string.Empty;
                    form.PaidThroughSWP = false;
                    form.IsApplicationFeePaid = false;

                    var nic = dbContext.NICsingalwindowSystems.FirstOrDefault(n => n.onlineapplicationId == form.ApplicationFormId);
                    if (nic != null)
                    {
                        form.NICControlId = nic.Control_ID;
                        form.NICUnitId = nic.Unit_Id;
                        form.NICServiceId = nic.ServiceID;
                        form.IsFromNIC = true;
                        form.AppType = SWPConstant.NIC;
                        form.PaidThroughSWP = true;
                        form.IsApplicationFeePaid = nic.Status_Code == 11 ? true : false;
                        form.ServiceAppPayStatus = nic.Fee_Status;

                        form.NICSingleWindowModel = nic;
                    }
                    else
                    {
                        form.IsFromNIC = false;
                        form.AppType = SWPConstant.Authority;
                    }

                    var payment = dbContext.OnlineApplicationDetails_trans.Where(p => p.ServiceRefId == detail.onlineapplicationId).OrderByDescending(o => o.AutoID).FirstOrDefault();
                    if (payment != null)
                    {
                        form.IsApplicationFeePaid = form.IsApplicationFeePaid == true ? true : ((payment.status == 1 && payment.ServiceType == 1) ? true : false);
                        form.IsRegistrationFeePaid = (payment.ServiceType == 3 && payment.status == 1) ? true : ((payment.status == 1 && payment.ServiceType == 1) ? true : false);
                        form.PaymentMode = (payment.ServiceType == 3) ? (!string.IsNullOrEmpty(payment.TrKey) ? ((payment.status == 1 || payment.TranStatus == 1) ? "Offline_Validated" : "Offline_Updated") : "Offline") : "Online";
                        form.IsChallanGenerated = payment.ServiceType == 3 ? true : false;

                        form.PaymentModel = GetOnlinePaymentDetailById(form.ApplicationFormId, payment.AutoID);
                    }

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

                    var flag = IsDocumentUploaded(form.ApplicationFormId);
                    if (flag == true)
                    {
                        form.IsDocumentUploaded = true;
                        form.DocumentsTable = dbContext.Sp_NewSchemereturn(form.ApplicationFormId, form.SchemeId).FirstOrDefault();
                    }
                    //else applicant.IsDocumentUploaded = false;

                    if (form.SchemeType == SWPSchemeType.Transport || form.SchemeType == SWPSchemeType.OpenEnded || form.SchemeType == SWPSchemeType.IndustrialScheme)
                    {
                        var PreChallan = IsPreviousChallanUploaded(form.ApplicationFormId);
                        if (PreChallan == true)
                        {
                            form.IsPreviousChallanUploaded = true;
                        }
                    }
                }
                else
                {
                    form.FlagId = SWPReturnTypeId.NotExist;
                }
                return form;
            }
        }

        public SWPFormViewModel SaveOnlineSchemeFormStatus(SWPFormViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                model.ReturnTypeId = SWPReturnTypeId.None;
                var Form = dbContext.OnlineApplicationDetails.Where(us => us.onlineapplicationId == model.ApplicationFormId).FirstOrDefault();
                if (Form != null && model.ActionType == "Reject") // Reject Application Form
                {
                    Boolean IsRejected = false;
                    Form.isActive = Form.isActive != true ? true : false;
                    IsRejected = (Boolean)Form.isActive;
                    dbContext.SaveChanges();

                    model.ReturnTypeId = SWPReturnTypeId.Rejected;
                    string body = string.Empty;
                    if (IsRejected)
                    {
                        body = string.Format(SWPMessage.AppRejected, Form.onlineapplicationId);
                    }
                    else { body = string.Format(SWPMessage.OfflineAppReqSuccess, Form.onlineapplicationId); }
                    if (!string.IsNullOrEmpty(body))
                    {
                        if (!string.IsNullOrEmpty(Form.email)) { SWPApplication.SendEmail(Form.email, "Online Application Request Status", body); }
                        if (!string.IsNullOrEmpty(Form.mobileNumberP2)) { SWPApplication.SendSMS(Form.mobileNumberP2, body); }
                    }
                }

                return model;
            }
        }

        public SWPFormViewModel SaveOnlineSchemeFormProcessRequest(SWPFormViewModel model)
        {
            SWPResultMessage resultMessage = new SWPResultMessage();

            var flag = SWPReturnTypeId.None;
            string message = "<div class='row'><div class='col-md-12'>";
            using (var dbContext = new PIMSEntitiesContext())
            {
                for (int i = 0; i < model.ApplicationIdList.Length; i++)
                {
                    SWPResultMessage processMessage = new SWPResultMessage();
                    int key = 0;
                    if (model.ApplicationIdList[i] > 0)
                    {
                        //OnlineApplicationProcessDetail Process = new OnlineApplicationProcessDetail();
                        int Id = model.ApplicationIdList[i];
                        int EnumStatus = (int)Enum.Parse(typeof(SWPEnum.FormProcess), model.ProcessType);

                        int ApproverUserId = 0;
                        if (!string.IsNullOrEmpty(model.User)) { ApproverUserId = Convert.ToInt32(model.User); }

                        var ProcessDetail = dbContext.OnlineApplicationProcessDetails.FirstOrDefault(m => m.OnlineApplicationId == Id);
                        if (ProcessDetail == null)
                        {
                            int onAppId = (from onlineapp in dbContext.OnlineApplicationDetails
                                           join trans in dbContext.OnlineApplicationDetails_trans on onlineapp.onlineapplicationId equals trans.ServiceRefId
                                           where trans.status == 1 && trans.TranStatus == 1 && onlineapp.onlineapplicationId == Id
                                           select onlineapp.onlineapplicationId).FirstOrDefault();
                            if (onAppId > 0)
                            {
                                OnlineApplicationProcessDetail ApplicationProcess = new OnlineApplicationProcessDetail();
                                ApplicationProcess.OnlineApplicationId = onAppId;
                                ApplicationProcess.ApplicationStatus = EnumStatus;
                                ApplicationProcess.ProcessStatus = SWPConstant.InProgress;
                                ApplicationProcess.SubmittedBy = userInfo.UserID;
                                ApplicationProcess.SubmitDate = DateTime.Now;
                                ApplicationProcess.Approver = ApproverUserId;
                                ApplicationProcess.CreatedBy = userInfo.UserID;
                                ApplicationProcess.CreatedDate = DateTime.Now;
                                dbContext.OnlineApplicationProcessDetails.Add(ApplicationProcess);
                                dbContext.SaveChanges();
                                flag = 201;
                                key = onAppId;
                                message = message + "<label>" + onAppId + " saved.</label>";
                            }
                            else { flag = SWPReturnTypeId.NotExist; message = message + "<label>" + onAppId + " not saved.application id not exist.</label>"; }
                        }
                        else
                        {
                            if (ProcessDetail.ProcessStatus == SWPConstant.Approved)
                            {
                                if (EnumStatus > ProcessDetail.ApplicationStatus)
                                {
                                    ProcessDetail.ApplicationStatus = EnumStatus;
                                    ProcessDetail.ProcessStatus = SWPConstant.InProgress;
                                    ProcessDetail.Approver = ApproverUserId;
                                    ProcessDetail.SubmittedBy = userInfo.UserID;
                                    ProcessDetail.SubmitDate = DateTime.Now;
                                    ProcessDetail.ModifiedBy = userInfo.UserID;
                                    ProcessDetail.ModifiedDate = DateTime.Now;
                                    dbContext.SaveChanges();
                                    flag = 201;
                                    key = Id;
                                    message = message + "<label>" + Id + " updated.</label>";
                                }
                                else
                                {
                                    flag = SWPReturnTypeId.NotExist;
                                    message = message + "<label>" + Id + " not updated.wrong process selected.</label>";
                                }
                            }
                            else
                            {
                                flag = SWPReturnTypeId.NotExist;
                                message = message + "<label>" + Id + " not updated.previous process not approved.</label>";
                            }
                        }
                    }
                    processMessage.ReturnType = flag;
                    processMessage.PrimaryKey = key;
                    resultMessage.Message = resultMessage.Message + message;
                    resultMessage.ResultTypeList.Add(processMessage);
                }
                model.ReturnTypeId = SWPReturnTypeId.Saved;
                model.Message = message + "</div></div>";
                model.ResultMessage = resultMessage;
            }
            return model;
        }

        public SWPFormViewModel SaveAllotmentDetailByOnlineSchemeFormId(SWPFormViewModel model)
        {
            model.ReturnTypeId = SWPReturnTypeId.None;
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (model.ApplicationFormId > 0)
                {
                    var data = GetOnlineSchemeFormDataById(model);
                    if (data != null)
                    {
                        string FormNo = Convert.ToString(data.ApplicationFormId);
                        var ExApplication = dbContext.ApplicationDetails.FirstOrDefault(m => m.formNo == FormNo);
                        if (ExApplication == null)
                        {
                            ApplicationDetail Form = new ApplicationDetail();
                            if (data.ApplicantType.ToLower() == SWPConstant.Company.ToLower())
                            {
                                Form = new ApplicationDetail
                                {
                                    firstName = data.Applicant,
                                    signingAuthority = data.SigningAuthority,
                                    registeredOffice = data.PermanentAddress,
                                    gender = data.Gender,
                                    fatherHusbandName = data.ApplicantMaster,
                                    quotaId = data.SigningAuthorityId,
                                    tFirstName = data.Applicant,
                                    T_Company_Name = data.Applicant,
                                    tAnnualIncome = data.AnnualIncome,
                                };
                            }
                            else
                            {
                                Form = new ApplicationDetail
                                {
                                    firstName = data.FirstName,
                                    middleName = data.MiddleName,
                                    lastName = data.LastName,
                                    fatherHusbandName = data.FatherName,
                                    motherName = data.MotherName,
                                    gender = data.Gender,
                                    religionId = data.ReligionId,
                                    occupationId = data.OccupationId,
                                    quotaId = SWPConstant.General,
                                    marritalStatus = data.MaritalStatus,
                                    dateOfBirth = data.DOB,
                                    tFirstName = data.FirstName,
                                    tMiddleName = data.MiddleName,
                                    tLastName = data.LastName
                                };
                            }

                            Form.pan = data.PanNumber;
                            Form.dateOfBirth = data.DOB;
                            Form.mobileNumberP2 = data.MobileNumber;
                            Form.dateOfBirth = data.DOB;
                            Form.permanentAdd = data.PermanentAddress;
                            Form.correspondanceAdd = data.CorrespondingAddress;
                            Form.email = data.Email;

                            Form.tFatherHusbandName = data.FatherName;
                            Form.tMotherName = data.MotherName;
                            Form.tPan = data.PanNumber;
                            Form.tEmail = data.Email;
                            Form.tMobileNumber = data.MobileNumber;
                            Form.tDateOfBirth = data.DOB;

                            Form.gender = data.Gender;
                            Form.tCorrespondanceAdd = data.CorrespondingAddress;
                            Form.tPermanentAdd = data.PermanentAddress;

                            Form.schemeId = data.SchemeId;
                            Form.departmentId = data.DepartmentId;
                            Form.formNo = data.FormNo;

                            Form.createdBy = userInfo.UserID.ToString();
                            Form.createdDate = DateTime.Now;

                            dbContext.ApplicationDetails.Add(Form);
                            dbContext.SaveChanges();

                            data.Id = Form.applicationId;
                            int pflag = SavePaymentDetailByOnlineSchemeFormId(data);
                            if (pflag == SWPReturnTypeId.Paid) model.ReturnTypeId = SWPReturnTypeId.Saved;
                            else model.ReturnTypeId = SWPReturnTypeId.NotPaid;
                            //flag = ReturnType.Updated;
                        }
                        else
                        {
                            model.ReturnTypeId = SWPReturnTypeId.Exist;
                        }
                    }
                }
            }
            return model;
        }

        private int SavePaymentDetailByOnlineSchemeFormId(SWPFormViewModel model)
        {
            var flag = SWPReturnTypeId.NotPaid;
            using (var dbContext = new PIMSEntitiesContext())
            {
                var existingRecord = dbContext.ApplicationPaymentDetails.Where(x => x.applicationId == model.Id).FirstOrDefault();
                if (existingRecord == null)
                {
                    var payment = new ApplicationPaymentDetail();
                    payment.applicationId = model.Id;
                    payment.paymentMode = model.PaymentModel.Mode;
                    payment.formNo = model.Id.ToString();
                    payment.amountDeposited = model.PaymentModel.Amount;
                    payment.utn = model.PaymentModel.TransactionKey;
                    payment.paymentDate = model.PaymentModel.modifiydate == null ? model.PaymentModel.EntryDate : model.PaymentModel.modifiydate;
                    payment.challanIssueDate = model.PaymentModel.modifiydate != null ? model.PaymentModel.EntryDate : null;
                    //payment.challanNo = model.PaymentModel.modifiydate != null ? model.PaymentModel.TransactionId : null;
                    payment.createdBy = userInfo.UserID.ToString();
                    payment.createdDate = DateTime.Now;
                    dbContext.ApplicationPaymentDetails.Add(payment);
                    dbContext.SaveChanges();
                    flag = SWPReturnTypeId.Paid;
                }
            }
            return flag;
        }

        public Kendo.Mvc.UI.DataSourceResult GetDropDownListByTypeAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, SWPDropdownViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (model.FilterType == "Scheme")
                {
                    var list = (from scheme in dbContext.SchemeMsts
                                where scheme.Status == SWPConstant.SchemeOpen && scheme.IsActive == true
                                && scheme.SchemeTypeMst.modifiedBy == SWPConstant.SchemeOnline
                                select new SWPDropdownViewModel
                                {
                                    Id = scheme.schemeId,
                                    Text = scheme.schemeName
                                });
                    return list.ToDataSourceResult(request);
                }
                else if (model.FilterType == "Department")
                {
                    var department = (from deptt in dbContext.DepartmentMsts
                                      join dtrans in dbContext.SchemeDepartmentTrans on deptt.departmentId equals dtrans.departmentId
                                      where deptt.IsActive == true && dtrans.IsActive == true
                                      && DepartmentList.Contains(deptt.departmentId)
                                      && (model.SchemeId == null || dtrans.schemeId == model.SchemeId)
                                      select new SWPDropdownViewModel
                                      {
                                          Id = deptt.departmentId,
                                          Text = deptt.departmentName,
                                          SchemeId = dtrans.schemeId != null ? (int)dtrans.schemeId : 0,
                                          Value = string.Empty
                                      });
                    return department.ToDataSourceResult(request);
                }
                else if (model.FilterType == "FormType")
                {
                    var list = (from config in dbContext.Common_Config
                                where config.Is_Active == 1 && config.Category == "FormType"
                                select new SWPDropdownViewModel
                                {
                                    Id = config.Id,
                                    Text = config.Name
                                });
                    return list.ToDataSourceResult(request);
                }
                else if (model.FilterType == "SubFormType")
                {
                    var list = (from config in dbContext.Common_Config
                                where config.Is_Active == 1 && config.Category == model.ActionType
                                select new SWPDropdownViewModel
                                {
                                    Id = config.Id,
                                    Text = config.Name,
                                    FilterType = model.ActionType
                                });
                    return list.ToDataSourceResult(request);
                }
                else if (model.FilterType == "ApplicantType")
                {
                    var list = (from config in dbContext.Common_Config
                                where config.Is_Active == 1 && config.Category == "ApplicantType"
                                select new SWPDropdownViewModel
                                {
                                    Id = config.Id,
                                    Text = config.Name
                                }).ToList();
                    return list.ToDataSourceResult(request);
                }
                else if (model.FilterType == "CompanyType")
                {
                    var lst = (from config in dbContext.Common_Config
                               where config.Is_Active == 1 && config.Category.ToLower() == model.ActionType.ToLower()
                               select new SWPDropdownViewModel
                               {
                                   Id = config.Id,
                                   Text = config.Name,
                                   ActionType = model.ActionType
                               });
                    request.Filters.RemoveAt(0);
                    return lst.ToDataSourceResult(request);
                }
                else if (model.FilterType == "AreaRange")
                {
                    var floorList = (from floor in dbContext.FloorMsts
                                     where floor.departmentId == model.DepartmentId && floor.modifiedBy == model.ActionType
                                     select new SWPDropdownViewModel
                                     {
                                         Id = floor.floorId,
                                         Text = floor.floorName,
                                         ActionType = model.ActionType
                                     });
                    return floorList.ToDataSourceResult(request);
                }
                else if (model.FilterType == "DirectorType")
                {
                    var lst = (from cfg in dbContext.Common_Config
                               where cfg.Is_Active == 1 && cfg.Category.ToLower().Equals(SWPCategoryType.Director.ToLower())
                               select new SWPDropdownViewModel
                               {
                                   Id = cfg.Id,
                                   Text = cfg.Name
                               });
                    return lst.ToDataSourceResult(request);
                }
                else if (model.FilterType == "Category")
                {
                    var lst = (from prop in dbContext.QuotaMsts
                               where prop.IsActive == true
                               select new SWPDropdownViewModel
                               {
                                   Id = prop.quotaId,
                                   Text = prop.quotaName,
                               });
                    return lst.ToDataSourceResult(request);
                }
                else if (model.FilterType == "Occupation")
                {
                    var lst = (from f in dbContext.OccupationMsts
                               select new SWPDropdownViewModel
                               {
                                   Id = f.occupationId,
                                   Text = f.occupation
                               });
                    return lst.ToDataSourceResult(request);
                }
                else if (model.FilterType == "Gender")
                {
                    List<SWPDropdownViewModel> genderList = new List<SWPDropdownViewModel>();
                    genderList.Add(new SWPDropdownViewModel { Text = SWPConstant.Male, Value = SWPConstant.Male });
                    genderList.Add(new SWPDropdownViewModel { Text = SWPConstant.Female, Value = SWPConstant.Female });
                    return genderList.ToDataSourceResult(request);
                }
                else if (model.FilterType == "MaritalStatus")
                {
                    List<SWPDropdownViewModel> MaritalList = new List<SWPDropdownViewModel>();
                    MaritalList.Add(new SWPDropdownViewModel { Text = "Married", Value = "Married" });
                    MaritalList.Add(new SWPDropdownViewModel { Text = "Unmarried", Value = "Unmarried" });
                    return MaritalList.ToDataSourceResult(request);
                }
                else if (model.FilterType == "ApplicationFormId")
                {
                    var list = (from f in dbContext.OnlineApplicationDetails
                                join t in dbContext.OnlineApplicationDetails_trans on f.onlineapplicationId equals t.ServiceRefId
                                join n in dbContext.NICsingalwindowSystems on f.onlineapplicationId equals n.onlineapplicationId
                                where f.schemeId == model.SchemeId && t.TranStatus ==1 && t.status ==1 && n.Status_Code == 11
                               select new SWPDropdownViewModel
                               {
                                   Id = f.onlineapplicationId,
                                   Text = f.onlineapplicationId.ToString()
                               });
                    return list.ToDataSourceResult(request);
                }
                else
                {
                    return null;
                }
            }
        }

        public SWPPaymentViewModel SaveOnlinePaymentTransaction(SWPPaymentViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                string transid = string.Empty;
                string trxkey = SWPApplication.GenerateTransactionId();
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(c => c.onlineapplicationId == model.ApplicationFormId);
                var transactionId = dbContext.OnlineApplicationDetails_trans.Where(f => f.ServiceRefId == model.ApplicationFormId).OrderByDescending(x => x.AutoID).Select(x => x.txnid).FirstOrDefault();
                if (transactionId == null)
                {
                    int brochureDepartmentId = Convert.ToInt32(ConfigurationManager.AppSettings["InstitutionalDepartmentId"]);
                    OnlineApplicationDetails_trans trans = new OnlineApplicationDetails_trans
                    {
                        ServiceRefId = form.onlineapplicationId,
                        TrKey = trxkey,
                        txnid = form.onlineapplicationId.ToString() + "-1",
                        Amount = form.EarnestMoney + form.ProcessingCharge + (2 * form.ProcessingCGST),
                        productinfo = "Reservation Money",
                        mode = "Online",
                        ServiceType = SWPConstant.OnlineApplicationPayment,
                        GetwayName = model.BankId == 2 ? "HDFC" : "PAYU",
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
                        ServiceRefId = form.onlineapplicationId,
                        TrKey = trxkey,
                        txnid = form.onlineapplicationId.ToString() + "-" + newTxId,
                        Amount = form.EarnestMoney + form.ProcessingCharge + (2 * form.ProcessingCGST),
                        productinfo = "Reservation Money",
                        mode = "Online",
                        ServiceType = SWPConstant.OnlineApplicationPayment,
                        GetwayName = model.BankId == 2 ? "HDFC" : "PAYU",
                        status = 0,
                        EntryDate = DateTime.Now
                    };

                    dbContext.OnlineApplicationDetails_trans.Add(trans);
                    dbContext.SaveChanges();
                    transid = trans.txnid;
                }

                var data = (from trans in dbContext.OnlineApplicationDetails_trans
                            join appl in dbContext.OnlineApplicationDetails on trans.ServiceRefId equals appl.onlineapplicationId
                            where trans.txnid == transid
                            select new SWPPaymentViewModel
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
                                FirstName = form.gender == "Company" ? form.CompanyName : (form.firstName + " " + (!string.IsNullOrEmpty(form.middleName) ? (form.middleName + " " + form.lastName) : form.lastName)),
                                Email = form.email,
                                PhoneNumber = form.mobileNumberP2,
                                MobileNo = form.mobileNumberP2,
                                Applicant = form.gender == "Company" ? form.CompanyName : (form.firstName + " " + (!string.IsNullOrEmpty(form.middleName) ? (form.middleName + " " + form.lastName) : form.lastName)),
                                AddressI = form.gender == "Company" ? form.registeredOffice : form.permanentAdd,
                                AddressII = form.correspondanceAdd
                            }).FirstOrDefault();
                return data;
            }
        }

        public SWPPaymentViewModel SaveOnlinePaymentTransactionReturn(System.Web.Mvc.FormCollection form)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var TransactionModel = new SWPPaymentViewModel();
                if (HttpContext.Current.Session["OnlineBankId"] != null)
                {
                    var bankId = (int)HttpContext.Current.Session["OnlineBankId"];
                    var bob = SaveBankOfBarodaOnlinePaymentReturn(form);
                    HttpContext.Current.Session["OnlineBankId"] = null;
                    return null; //bob;
                }
                else
                {
                    //string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";
                    string HashSequence = ConfigurationManager.AppSettings["hashSequence"];

                    if (form != null && form["status"].ToString() == "success")
                    {
                        var id = form["udf1"]; // TransactionId
                        var rid = form["udf2"]; // RegistrationId
                        var requestId = form["udf3"]; // OnlineRequestId
                        var challanId = form["udf4"]; // ChallanId
                        var serviceRequestId = !string.IsNullOrEmpty(requestId) ? Convert.ToInt32(requestId) : 0;//requestId
                        var paidAmount = Convert.ToDecimal(form["amount"]);

                        string[] paramArr = HashSequence.Split('|');
                        Array.Reverse(paramArr);
                        string paramSequence = ConfigurationManager.AppSettings["HDFC_SALT"] + "|" + form["status"].ToString();
                        foreach (string param in paramArr)
                        {
                            paramSequence += "|";
                            paramSequence = paramSequence + (form[param] != null ? form[param] : "");
                        }

                        string hashSequence = SWPPaymentGateway.GenerateSHA512HashCode(paramSequence).ToLower();
                        if (hashSequence == form["hash"])
                        {
                            var transaction = dbContext.OnlineApplicationDetails_trans.Where(m => m.txnid == id).FirstOrDefault();
                            if (transaction != null)
                            {
                                //transaction.TranStatus = (form["status"].ToString() == "success" && !string.IsNullOrEmpty(form["bank_ref_num"])) ? NAStatusId.Success : NAStatusId.Failed;
                                transaction.TranStatus = form["status"].ToString() == "success" ? SWPStatusId.Success : SWPStatusId.Failed;
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

                                TransactionModel = GetOnlinePaymentDetailById(new SWPPaymentViewModel { TransactionId = id });

                                if (form["status"].ToString() == "success")
                                {
                                    var SchemeForm = dbContext.OnlineApplicationDetails.FirstOrDefault(f => f.onlineapplicationId == transaction.ServiceRefId);
                                    TransactionModel.ReturnTypeId = SWPReturnTypeId.Success; //success
                                    TransactionModel.Status = form["status"].ToString();
                                    string message = string.Format(SWPMessage.OnlineProcessingFee, form["productinfo"], transaction.ServiceRefId, transaction.Amount);
                                    if (!string.IsNullOrEmpty(SchemeForm.mobileNumberP2)) SWPApplication.SendSMS(SchemeForm.mobileNumberP2, message);
                                    if (!string.IsNullOrEmpty(SchemeForm.email)) SWPApplication.SendEmail(SchemeForm.email, "Online Payment", message);
                                }
                                else
                                {
                                    TransactionModel.ReturnTypeId = SWPReturnTypeId.Failed;
                                    TransactionModel.Status = form["status"].ToString();
                                }

                            }
                            else
                            {
                                TransactionModel.ReturnTypeId = SWPReturnTypeId.Mismatch; //mismatch
                            }
                        }
                        else
                        {
                            TransactionModel.ReturnTypeId = SWPReturnTypeId.Mismatch; //mismatch
                        }
                    }
                    else
                    {
                        TransactionModel.ReturnTypeId = SWPReturnTypeId.Failed; // failed
                    }
                }
                return TransactionModel;
            }
        }

        private SWPPaymentViewModel GetOnlinePaymentDetailById(SWPPaymentViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var data = (from trans in dbContext.OnlineApplicationDetails_trans
                            join appl in dbContext.OnlineApplicationDetails on trans.ServiceRefId equals appl.onlineapplicationId
                            //join challan in dbContext.Challan_Master on trans.ChallanId equals challan.Id.ToString()
                            where trans.txnid == model.TransactionId
                            select new SWPPaymentViewModel
                            {
                                Id = trans.AutoID,
                                ApplicationFormId = appl.onlineapplicationId,
                                ServiceRefId = trans.ServiceRefId,
                                TransactionId = trans.txnid,
                                TransactionKey = trans.TrKey,
                                Amount = trans.Amount,
                                PaymentTypeId = trans.ServiceType,
                                GatewayName = trans.GetwayName,
                                ProductInfo = trans.productinfo,
                                Udf1 = trans.udf1,
                                Udf2 = trans.udf2,
                                Udf3 = trans.udf3,
                                Udf4 = trans.udf4,
                                Udf5 = trans.udf5,
                                Mihpayid = trans.mihpayid,
                                Mode = trans.mode,
                                TransactionStatusId = trans.TranStatus == 1 ? 1 : 0,
                                TransactionStatus = trans.TranStatus == 1 ? "Success" : "Failed",
                                StatusId = trans.status,
                                //Status = trans.status != null ? dbContext.StatusMasters.FirstOrDefault(f => f.Id == trans.status).Status : "NA",
                                Status = (trans.TranStatus != null ? (trans.TranStatus == SWPConstant.Success ? "Success" : "Failure") : "Validation Pending"),
                                EntryDate = trans.EntryDate,
                                PaymentSource = trans.payment_source,
                                PG_Type = trans.PG_Type,
                                BankReferenceNo = trans.bank_ref_num,
                                BankCode = trans.bankcode,
                                NameOnCard = trans.name_on_card,
                                CardNumber = trans.cardnum,
                                IssuingBank = trans.issuing_bank,
                                RegistrationId = trans.RegistrationId,
                                OnlineRequestId = trans.OnlineRequestId,

                                VirtualAccountNo = trans.virtualaccountno,
                                AccountNo = trans.virtualaccountno,
                                DepartmentId = appl.departmentId,
                                Department = dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == appl.departmentId).departmentName,
                                Applicant = appl.gender == "Company" ? appl.CompanyName : (appl.firstName + " " + (!string.IsNullOrEmpty(appl.middleName) ? (appl.middleName + " " + appl.lastName) : appl.lastName)),
                                MobileNo = appl.mobileNumberP2,
                                Email = appl.email,

                                BankName = trans.GetwayName == "HDFC" ? "HDFC BANK" : (trans.GetwayName == "PAYU" ? "INDUSIND BANK" : "OTHER")

                            }).FirstOrDefault();
                return data;
            }
        }

        private SWPPaymentViewModel SaveBankOfBarodaOnlinePaymentReturn(System.Web.Mvc.FormCollection form)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                //var ServiceDetailModel = new ServiceRequestViewModel();
                var TransactionModel = new SWPPaymentViewModel();

                string resourcePath = ConfigurationManager.AppSettings["BOB_RESOURCE_PATH"];
                string recieptURL = ConfigurationManager.AppSettings["BOB_RETURN_URL"];
                string errorURL = ConfigurationManager.AppSettings["BOB_RETURN_URL"];
                string aliasName = ConfigurationManager.AppSettings["BOB_ALIAS_NAME"];

                iPayPipe pipe = new iPayPipe();
                pipe.setAlias(aliasName);
                pipe.setResourcePath(resourcePath); //pipe.getResourcePath();
                pipe.setKeystorePath(resourcePath);

                if (form != null)
                {
                    if (!String.IsNullOrEmpty(form["trandata"]))
                    {
                        int a = pipe.parseEncryptedRequest(form["trandata"].ToString());
                        var challanId = pipe.getUdf19(); // ChallanId
                        var rid = pipe.getUdf20(); // RegistrationId
                        var transid = pipe.getUdf21(); //pipe.getTransId(); // TransactionId
                        var paidAmount = Convert.ToDecimal(pipe.getAmt());
                        if (a == 0)
                        {
                            string errorText = !String.IsNullOrEmpty(HttpContext.Current.Request.Form["ErrorText"]) ? HttpContext.Current.Request.Form["ErrorText"] : !String.IsNullOrEmpty(HttpContext.Current.Request.QueryString.Get("ErrorText")) ? HttpContext.Current.Request.QueryString.Get("ErrorText") : "";
                            var result = pipe.getResult();
                            //var id = pipe.getUdf1();
                            var transaction = dbContext.OnlineApplicationDetails_trans.Where(m => m.txnid == transid).FirstOrDefault();
                            if (transaction != null)
                            {
                                //transaction.TranStatus = (form["status"].ToString() == "success" && !string.IsNullOrEmpty(form["bank_ref_num"])) ? NAStatusId.Success : NAStatusId.Failed;
                                transaction.TranStatus = a == 0 ? SWPStatusId.Success : SWPStatusId.Failed;
                                transaction.status = a == 0 ? 1 : 0;
                                transaction.error = errorText;// form["error"];
                                transaction.error_Message = errorText; // form["error_Message"];
                                //transaction.TrKey = pipe.getTransId();  //form["txnid"].ToString();
                                transaction.mode = "Online"; //pipe.getPmntmode(); //form["mode"].ToString();
                                transaction.mihpayid = pipe.getPaymentId(); //form["mihpayid"];
                                //transaction.productinfo = form["productinfo"];
                                transaction.EntryDate = DateTime.Now;
                                transaction.payment_source = pipe.getType(); //form["payment_source"];
                                transaction.PG_Type = "BOB"; //pipe.getPmntmode();  //form["PG_Type"];
                                transaction.bank_ref_num = pipe.getRef();  //form["bank_ref_num"];
                                transaction.bankcode = pipe.getRef(); //form["bankcode"];
                                transaction.name_on_card = pipe.getMember(); //form["name_on_card"];
                                transaction.cardnum = pipe.getCard(); //form["cardnum"];
                                //transaction.cardhash = form["cardhash"];
                                transaction.issuing_bank = "Bank Of Baroda"; //form["issuing_bank"];
                                transaction.card_type = pipe.getType(); //form["card_type"];
                                transaction.udf1 = pipe.getTransId();
                                transaction.udf2 = pipe.getTrackId();
                                transaction.ChallanId = pipe.getUdf19();
                                dbContext.SaveChanges();

                                //var request = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == transaction.ServiceRefId);
                                TransactionModel = GetOnlinePaymentDetailById(new SWPPaymentViewModel { TransactionId = transid });
                                if (a == 0)
                                {
                                    //var challan = dbContext.Challan_Master.FirstOrDefault(c => c.Id.ToString() == challanId);
                                    var challan = dbContext.Challan_Master.FirstOrDefault(c => c.Id.ToString() == TransactionModel.ChallanId.ToString());
                                    if (challan != null) challan.Is_Verified = true;
                                    dbContext.SaveChanges();
                                    TransactionModel.ReturnTypeId = SWPReturnTypeId.Success; //success
                                    TransactionModel.Status = "success";
                                    string message = string.Format(SWPMessage.OnlineProcessingFee, "Online Payment", transaction.ServiceRefId, transaction.Amount);
                                    if (!string.IsNullOrEmpty(TransactionModel.MobileNo)) SWPApplication.SendSMS(TransactionModel.MobileNo, message);
                                    if (!string.IsNullOrEmpty(TransactionModel.Email)) SWPApplication.SendEmail(TransactionModel.Email, "Online Request", message);
                                    //form = null;
                                }
                                else
                                {
                                    TransactionModel.ReturnTypeId = SWPReturnTypeId.Failed; //failed
                                    TransactionModel.Status = "failed";
                                }
                            }
                            else
                            {
                                TransactionModel.ReturnTypeId = SWPReturnTypeId.Mismatch; //mismatch
                            }
                        }
                    }
                    else
                    {
                        TransactionModel.ReturnTypeId = SWPReturnTypeId.Failed; // failed
                    }
                }
                else
                {
                    TransactionModel.ReturnTypeId = SWPReturnTypeId.Failed; // failed
                }

                //ServiceDetailModel.OnlinePaymentModel = TransactionModel;
                return TransactionModel;
            }
            //return null;
        }

        public Kendo.Mvc.UI.DataSourceResult GetSchemeFormChallanAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, SWPFormViewModel modal)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var formlist = dbContext.Database.SqlQuery<int>("select ServiceRefId from RePrint_Challan").ToList();
                //var formlist = dbContext.Database.SqlQuery<ReprintChallanModel>("select * from RePrint_Challan").ToList();
                var list = (from oam in dbContext.OnlineApplicationDetails
                            //join form in formlist on oam.onlineapplicationId equals form.ServiceRefId
                            from nic in dbContext.NICsingalwindowSystems.Where(n => n.onlineapplicationId == oam.onlineapplicationId).DefaultIfEmpty()
                            from sch in dbContext.SchemeMsts.Where(s => s.schemeId == oam.schemeId).DefaultIfEmpty()
                            from deptt in dbContext.DepartmentMsts.Where(d => d.departmentId == oam.departmentId).DefaultIfEmpty()
                            where nic.Status_Code == 11 && formlist.Contains(oam.onlineapplicationId)
                            && (modal.Id == null || oam.onlineapplicationId == modal.Id)
                            && (modal.DepartmentId == null || oam.departmentId == modal.DepartmentId)
                            && (modal.SchemeId == null || oam.schemeId == modal.SchemeId)
                            select new SWPFormViewModel
                            {
                                Id = oam.onlineapplicationId,
                                ApplicationFormId = oam.onlineapplicationId,
                                SchemeName = sch.schemeName,
                                Department = deptt.departmentName,
                                NICControlId = nic.Control_ID,
                                NICUnitId = nic.Unit_Id,
                                NICServiceId = nic.ServiceID,
                                ServiceAppPayStatus = nic.Fee_Status,
                                IsApplicationFeePaid = nic.Status_Code == 11 ? true : false,
                                Applicant = string.IsNullOrEmpty(oam.CompanyName) ? oam.firstName + " " + (!string.IsNullOrEmpty(oam.middleName) ? oam.middleName + " " + oam.lastName : oam.lastName) : oam.CompanyName,
                                FirstName = string.IsNullOrEmpty(oam.CompanyName) ? oam.firstName + " " + (!string.IsNullOrEmpty(oam.middleName) ? oam.middleName + " " + oam.lastName : oam.lastName) : oam.CompanyName,
                                Gender = oam.gender,
                                DOB = oam.dateOfBirth,
                                FormStatus = oam.IsSubmited == null ? SWPStatus.Form.Rejected : (oam.IsSubmited == true ? SWPStatus.Form.Accepted : SWPStatus.Form.InProgress),
                                IsDeleted = oam.isActive == null ? "" : (oam.isActive == true ? SWPStatus.Form.Accepted : SWPStatus.Form.Rejected),
                                IsFormRejected = oam.isActive == false ? true : false,
                                TotalAmount = oam.TotalAmount,
                                SubmitDate = oam.createdDate,
                                AreaRange = dbContext.FloorMsts.Where(m => m.floorId.ToString() == oam.area && m.IsActive == true).FirstOrDefault().floorName,
                                ApplicantType = oam.gender == "Company" ? "Company" : "Individual",
                                AmountPaidDate = dbContext.OnlineApplicationDetails_trans.Where(x => x.ServiceRefId == oam.onlineapplicationId).OrderByDescending(o => o.AutoID).FirstOrDefault().EntryDate,
                                ChallanBank = "HDFC", //dbContext.SchemeBankTrans.FirstOrDefault(m => m.schemeId == oam.schemeId && m.IsActive == true && m.bankId.ToString()==pmt.GetwayName).BankMst.bankName, 
                                PayType = dbContext.OnlineApplicationDetails_trans.Where(x => x.ServiceRefId == oam.onlineapplicationId).OrderByDescending(o => o.AutoID).FirstOrDefault().ServiceType == 3 ? "Challan" : "Other",
                                BranchName = "Noida 18 Branch",
                                IFSCCode = dbContext.OnlineApplicationDetails_trans.Where(x => x.ServiceRefId == oam.onlineapplicationId).OrderByDescending(o => o.AutoID).FirstOrDefault().udf5,
                                AccountNo = dbContext.OnlineApplicationDetails_trans.Where(x => x.ServiceRefId == oam.onlineapplicationId).OrderByDescending(o => o.AutoID).FirstOrDefault().virtualaccountno,
                                ApplicationFormType = !string.IsNullOrEmpty(oam.FormCategory) ? (oam.FormCategory) : "General",
                                ExpansionType = !string.IsNullOrEmpty(oam.FormCategory) ? ((oam.FormCategory == SWPConstant.ApplicationFormTypeEx) ? oam.FormSubCategory : "--") : "--",
                                TransactionId = dbContext.OnlineApplicationDetails_trans.Where(x => x.ServiceRefId == oam.onlineapplicationId).OrderByDescending(o => o.AutoID).FirstOrDefault().txnid
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public SWPEpassViewModel SaveEpassFormDetail(SWPEpassViewModel model, System.Web.HttpPostedFileBase userImage, System.Web.HttpPostedFileBase idfile, System.Web.HttpPostedFileBase rcfile, System.Web.HttpPostedFileBase dlfile)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                SWPEpassViewModel epass = new SWPEpassViewModel();
                epass.ActionTypeId = 1;
                //epass.EpassNo = "Covid-19-2020" + DateTime.Now.Month + DateTime.Now.Day + "-" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                epass.EpassNo = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
                epass.EpassType = model.EpassType == null ? "V" : model.EpassType;
                epass.Applicant = model.Applicant;
                epass.MobileNo = model.MobileNo;
                epass.Email = model.Email;
                epass.State = model.State;
                epass.City = model.City;
                epass.LocalPlace = model.LocalPlace;
                epass.PIN = model.PIN;
                epass.StartDate = model.StartDate;
                epass.EndDate = model.EndDate;
                epass.FromAddress = model.FromAddress;
                epass.ToAddress = model.ToAddress;
                epass.VehicleType = model.VehicleType;
                epass.VehicleNo = model.VehicleNo;
                epass.RCNo = model.RCNo;
                epass.Purpose = model.Purpose;
                epass.EPDI = model.EPDI;
                epass.EPDII = model.IsAuthorityEmployee.ToString();
                epass.EPDIII = model.EPDIII;
                epass.EPDIV = model.EPDIV;
                epass.EPDV = model.EPDV;
                epass.IsActive = true;
                epass.StatusId = 0;
                epass.EntryDate = DateTime.Now;

                epass.PhotoPath = userImage != null ? "/UploadFiles/" + "Epass/" + epass.EpassNo + "/ApplicantImage.jpg" : null;
                epass.UserIdPath = idfile != null ? "/UploadFiles/" + "Epass/" + epass.EpassNo + "/ApplicantId.jpg" : null;
                epass.RCPath = rcfile != null ? "/UploadFiles/" + "Epass/" + epass.EpassNo + "/VehicleRC.jpg" : null;
                epass.LicencePath = dlfile != null ? "/UploadFiles/" + "Epass/" + epass.EpassNo + "/DrivingLicence.jpg" : null;

                var sqlConnectionString = ConfigurationManager.ConnectionStrings["PIMSSqlConnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                {

                    using (SqlCommand command = new SqlCommand("USP_SaveEpassDetail", connection))
                    {
                       // DataTable dt = MapEpassWithDataTable(epass);
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        //SqlParameter param = new SqlParameter("@epassTVP", SqlDbType.Structured)
                        //{
                        //    TypeName = "EpassMasterTableType",
                        //    Value = dt
                        //};
                        //command.Parameters.Add(param);
                        command.Parameters.Add("@ActionTypeId", SqlDbType.Int).Value = epass.ActionTypeId;
                        command.Parameters.Add("@Id", SqlDbType.VarChar).Value = 0;
                        command.Parameters.Add("@EpassNo", SqlDbType.VarChar).Value = epass.EpassNo;
                        command.Parameters.Add("@EpassType", SqlDbType.VarChar).Value = epass.EpassType;
                        command.Parameters.Add("@Applicant", SqlDbType.VarChar).Value = epass.Applicant;
                        command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = epass.MobileNo;
                        command.Parameters.Add("@Email", SqlDbType.VarChar).Value = epass.Email;
                        command.Parameters.Add("@City", SqlDbType.VarChar).Value = epass.City;
                        command.Parameters.Add("@State", SqlDbType.VarChar).Value = epass.State;
                        command.Parameters.Add("@LocalPlace", SqlDbType.VarChar).Value = epass.LocalPlace;
                        command.Parameters.Add("@PIN", SqlDbType.VarChar).Value = epass.PIN;
                        command.Parameters.Add("@FromAddress", SqlDbType.VarChar).Value = epass.FromAddress;
                        command.Parameters.Add("@ToAddress", SqlDbType.VarChar).Value = epass.ToAddress;
                        command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = epass.StartDate;
                        command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = epass.EndDate;
                        command.Parameters.Add("@Purpose", SqlDbType.VarChar).Value = epass.Purpose;
                        command.Parameters.Add("@VehicleType", SqlDbType.VarChar).Value = epass.VehicleType;
                        command.Parameters.Add("@VehicleNo", SqlDbType.VarChar).Value = epass.VehicleNo;
                        command.Parameters.Add("@RCNo", SqlDbType.VarChar).Value = epass.RCNo;
                        command.Parameters.Add("@EntryDate", SqlDbType.DateTime).Value = epass.EntryDate;
                        command.Parameters.Add("@PhotoPath", SqlDbType.VarChar).Value = epass.PhotoPath;
                        command.Parameters.Add("@UserIdPath", SqlDbType.VarChar).Value = epass.UserIdPath;
                        command.Parameters.Add("@RCPath", SqlDbType.VarChar).Value = epass.RCPath;
                        command.Parameters.Add("@LicencePath", SqlDbType.VarChar).Value = epass.LicencePath;
                        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = epass.IsActive;
                        command.Parameters.Add("@StatusId", SqlDbType.Int).Value = epass.StatusId;
                        command.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = epass.IsApproved;
                        command.Parameters.Add("@ApprovalDate", SqlDbType.DateTime).Value = epass.ApprovalDate;
                        command.Parameters.Add("@Approver", SqlDbType.VarChar).Value = epass.Approver;
                        command.Parameters.Add("@Comment", SqlDbType.VarChar).Value = epass.Comment;
                        command.Parameters.Add("@EPDI", SqlDbType.VarChar).Value = epass.EPDI;
                        command.Parameters.Add("@EPDII", SqlDbType.VarChar).Value = epass.EPDII;
                        command.Parameters.Add("@EPDIII", SqlDbType.VarChar).Value = epass.EPDIII;
                        command.Parameters.Add("@EPDIV", SqlDbType.VarChar).Value = epass.EPDIV;
                        command.Parameters.Add("@EPDV", SqlDbType.VarChar).Value = epass.EPDV;
                        command.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = epass.ModifiedDate;

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                //dbContext.Database.SqlQuery<int>("EXEC USP_SaveEpassDetail @epass", epass);

                //dbContext.EpassMasters.Add(epass);
                //dbContext.SaveChanges();

                //int id = (int)epass.Id;
                model.Id = epass.Id;
                //model.EpassNo = dbContext.EpassMasters.FirstOrDefault(c => c.Id == id).EpassNo;
                model.EpassNo = epass.EpassNo;
                model.StatusId = SWPReturnTypeId.Saved;
                SaveEpassDocuments(model, userImage, idfile, rcfile, dlfile);

                string message = string.Format(SWPMessage.OnlineApplicationSubmitted, model.EpassNo);
                if (!string.IsNullOrEmpty(model.Email)) SWPApplication.SendEmail(model.Email, "Registration Form", message);
                if (!string.IsNullOrEmpty(model.MobileNo)) SWPApplication.SendSMS(model.MobileNo, message);

                return model;
            }
        }

        private DataTable MapEpassWithDataTable(SWPEpassViewModel model)
        {
            DataTable dt = new DataTable();
            DataRow dtrow = dt.NewRow();
            dt.Columns.Add(new DataColumn { ColumnName = "Id", DataType = typeof(int), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "EpassNo", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "EpassType", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "Applicant", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "MobileNo", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "Email", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "State", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "City", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "LocalPlace", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "PIN", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "StartDate", DataType = typeof(DateTime), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "EndDate", DataType = typeof(DateTime), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "FromAddress", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "ToAddress", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "VehicleType", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "VehicleNo", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "RCNo", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "Purpose", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "EPDI", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "EPDII", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "EPDIII", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "EPDIV", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "EPDV", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "IsActive", DataType = typeof(bool), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "StatusId", DataType = typeof(int), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "EntryDate", DataType = typeof(DateTime), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "IsApproved", DataType = typeof(bool), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "ApprovalDate", DataType = typeof(DateTime), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "Approver", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "Comment", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "ModifiedDate", DataType = typeof(DateTime), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "PhotoPath", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "UserIdPath", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "RCPath", DataType = typeof(string), AllowDBNull = true });
            dt.Columns.Add(new DataColumn { ColumnName = "LicencePath", DataType = typeof(string), AllowDBNull = true });
            //dt.Columns.Add(new DataColumn { ColumnName = "Approver", DataType = typeof(string), AllowDBNull = true });
            
            dtrow["Id"] = 0;
            dtrow["EpassNo"]= model.EpassNo;
            dtrow["EpassType"]=model.EpassType;
            dtrow["Applicant"]=model.Applicant;
            dtrow["MobileNo"]=model.MobileNo;
            dtrow["Email"]=model.Email;
            dtrow["State"]=model.State;
            dtrow["City"]=model.City;
            dtrow["LocalPlace"]=model.LocalPlace;
            dtrow["PIN"]=model.PIN;
            //dtrow["StartDate"]=model.StartDate;
            //dtrow["EndDate"]=model.EndDate;
            dtrow["FromAddress"]=model.FromAddress;
            //dtrow["ToAddress"]=model.ToAddress;
            dtrow["VehicleType"]=model.VehicleType;
            dtrow["VehicleNo"]=model.VehicleNo;
            dtrow["RCNo"]=model.RCNo;
            dtrow["Purpose"]=model.Purpose;
            dtrow["EPDI"]=model.EPDI;
            dtrow["EPDII"]=model.EPDII;
            dtrow["EPDIII"]=model.EPDIII;
            dtrow["EPDIV"]=model.EPDIV;
            dtrow["EPDV"]=model.EPDV;
            dtrow["IsActive"]=model.IsActive;
            dtrow["StatusId"]=model.StatusId;
            dtrow["EntryDate"]=model.EntryDate;
            //dtrow["IsApproved"]=model.IsApproved;
            //dtrow["ApprovalDate"]=model.ApprovalDate;
            //dtrow["Approver"]=model.Approver;
            dtrow["Comment"]=model.Comment;
            //dtrow["ModifiedDate"]=model.ModifiedDate;
            dtrow["PhotoPath"]=model.PhotoPath;
            dtrow["UserIdPath"]=model.UserIdPath;
            dtrow["RCPath"]=model.RCPath;
            dtrow["LicencePath"]=model.LicencePath;
            dt.Rows.Add(dtrow);
            return dt;

            //DataTable dt = new DataTable();
            //dt.Columns.Add("Id", typeof(int));
            //dt.Columns.Add("EpassNo", typeof(string));
            //dt.Columns.Add("EpassType", typeof(string));
            //dt.Columns.Add("Applicant", typeof(string));
            //dt.Columns.Add("MobileNo", typeof(string));
            //dt.Columns.Add("Email", typeof(string));
            //dt.Columns.Add("State", typeof(string));
            //dt.Columns.Add("City", typeof(string));
            //dt.Columns.Add("LocalPlace", typeof(string));
            //dt.Columns.Add("PIN", typeof(string));
            //dt.Columns.Add("StartDate", typeof(DateTime));
            //dt.Columns.Add("EndDate", typeof(DateTime));
            //dt.Columns.Add("FromAddress", typeof(string));
            //dt.Columns.Add("ToAddress", typeof(string));
            //dt.Columns.Add("VehicleType", typeof(string));
            //dt.Columns.Add("VehicleNo", typeof(string));
            //dt.Columns.Add("RCNo", typeof(string));
            //dt.Columns.Add("Purpose", typeof(string));
            //dt.Columns.Add("EPDI", typeof(string));
            //dt.Columns.Add("EPDII", typeof(string));
            //dt.Columns.Add("EPDIII", typeof(string));
            //dt.Columns.Add("EPDIV", typeof(string));
            //dt.Columns.Add("EPDV", typeof(string));
            //dt.Columns.Add("IsActive", typeof(bool));
            //dt.Columns.Add("StatusId", typeof(int));
            //dt.Columns.Add("EntryDate", typeof(DateTime));
            //dt.Columns.Add("IsApproved", typeof(bool));
            //dt.Columns.Add("ApprovalDate", typeof(DateTime));
            //dt.Columns.Add("Approver", typeof(string));
            //dt.Columns.Add("Comment", typeof(string));
            //dt.Columns.Add("ModifiedDate", typeof(DateTime));

            //dt.Columns.Add("PhotoPath", typeof(string));
            //dt.Columns.Add("UserIdPath", typeof(string));
            //dt.Columns.Add("RCPath", typeof(string));
            //dt.Columns.Add("LicencePath", typeof(string));
            //return dt;
        }

        private void SaveEpassDocuments(SWPEpassViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase idfile, HttpPostedFileBase rcfile, HttpPostedFileBase dlfile)
        {
            model.Id = model.Id;
            var docpath = ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + "Epass";

            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(docpath)))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(docpath));
            }

            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(docpath + "/" + model.EpassNo)))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(docpath + "/" + model.EpassNo));
            }
            var dirpath = System.Web.HttpContext.Current.Server.MapPath(docpath + "/" + model.EpassNo);

            if (userImage != null && userImage.ContentLength > 0)
            {
                FileInfo[] imagefile = new DirectoryInfo(dirpath).GetFiles("*" + "ApplicantImage" + "*.*");
                if (imagefile != null && imagefile.Count() > 0)
                {
                    imagefile[0].Delete();
                }
                string imagepath = dirpath + "\\" + "ApplicantImage" + ".jpg";
                string extension = Path.GetExtension(userImage.FileName);
                var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + "Epass" + "/" + model.EpassNo + "/" + "ApplicantImage" + extension);
                userImage.SaveAs(fileSavePath);
            }
            if (idfile != null && idfile.ContentLength > 0)
            {
                FileInfo[] ifile = new DirectoryInfo(dirpath).GetFiles("*" + "ApplicantId" + "*.*");
                if (ifile != null && ifile.Count() > 0)
                {
                    ifile[0].Delete();
                }
                string idpath = dirpath + "\\" + "ApplicantId" + ".jpg";
                string extension = Path.GetExtension(idfile.FileName);
                var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + "Epass" + "/" + model.EpassNo + "/" + "ApplicantId" + extension);
                idfile.SaveAs(fileSavePath);
            }
            if (rcfile != null && rcfile.ContentLength > 0)
            {
                FileInfo[] rfile = new DirectoryInfo(dirpath).GetFiles("*" + "VehicleRC" + "*.*");
                if (rfile != null && rfile.Count() > 0)
                {
                    rfile[0].Delete();
                }
                string rcpath = dirpath + "\\" + "VehicleRC" + ".jpg";
                string extension = Path.GetExtension(rcfile.FileName);
                var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + "Epass" + "/" + model.EpassNo + "/" + "VehicleRC" + extension);
                rcfile.SaveAs(fileSavePath);
            }
            if (dlfile != null && dlfile.ContentLength > 0)
            {
                FileInfo[] dfile = new DirectoryInfo(dirpath).GetFiles("*" + "DrivingLicence" + "*.*");
                if (dfile != null && dfile.Count() > 0)
                {
                    dfile[0].Delete();
                }
                string dlpath = dirpath + "\\" + "DrivingLicence" + ".jpg";
                string extension = Path.GetExtension(dlfile.FileName);
                var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + "Epass" + "/" + model.EpassNo + "/" + "DrivingLicence" + extension);
                dlfile.SaveAs(fileSavePath);
            }
        }

        public SWPEpassViewModel GetEpassFormDetailById(SWPEpassViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                //var data = (from epass in dbContext.EpassMasters
                //            where epass.EpassNo == model.EpassNo //&& epass.StatusId==1
                //            select new SWPEpassViewModel
                //            {
                //                Id = epass.Id,
                //                EpassNo = epass.EpassNo,
                //                EpassType = epass.EpassType,
                //                Applicant = epass.Applicant,
                //                MobileNo = epass.MobileNo,
                //                Email = epass.Email,
                //                State = epass.State,
                //                City = epass.City,
                //                LocalPlace = epass.LocalPlace,
                //                PIN = epass.PIN,
                //                StartDate = epass.StartDate,
                //                EndDate = epass.EndDate,
                //                FromAddress = epass.FromAddress,
                //                ToAddress = epass.ToAddress,
                //                Purpose = epass.Purpose,
                //                VehicleType = epass.VehicleType,
                //                VehicleNo = epass.VehicleNo,
                //                RCNo = epass.RCNo,
                //                EntryDate = epass.EntryDate,
                //                PhotoPath = epass.PhotoPath,
                //                UserIdPath = epass.UserIdPath,
                //                RCPath = epass.RCPath,
                //                LicencePath = epass.LicencePath,
                //                IsActive = epass.IsActive,
                //                StatusId = epass.StatusId,
                //                IsApproved = epass.IsApproved,
                //                ApprovalDate = epass.ApprovalDate,
                //                Approver = epass.Approver,
                //                Comment = epass.Comment,
                //                EPDI = epass.EPDI,
                //                EPDII = epass.EPDII,
                //                EPDIII = epass.EPDIII,
                //                EPDIV = epass.EPDIV,
                //                EPDV = epass.EPDV,
                //                IsAuthorityEmployee = epass.EPDII == "True" ? true : false,
                //                ModifiedDate = epass.ModifiedDate
                //            }).FirstOrDefault();

                var data = new SWPEpassViewModel();
                try
                {
                    data = dbContext.Database.SqlQuery<SWPEpassViewModel>("select * from EpassMaster where EpassNo=" + model.EpassNo).FirstOrDefault();
                    
                    if (data != null)
                    {
                        data.IsAuthorityEmployee = data.EPDII == "True" ? true : false;
                        data.ValidTillDate = data.ApprovalDate != null ? data.ApprovalDate.Value.AddDays(7) : DateTime.Now.AddDays(7);

                        if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + "Epass" + "/" + model.EpassNo)))
                        {
                            var dirpath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + "Epass" + "/" + model.EpassNo);
                            //var fileList = new DirectoryInfo(dirpath).GetFileSystemInfos(fileExtn);
                            FileInfo[] imagefile = new DirectoryInfo(dirpath).GetFiles("*" + "ApplicantImage" + "*.*");
                            if (imagefile != null)
                            {
                                data.PhotoPath = "/UploadFiles/" + "Epass/" + model.EpassNo + "/ApplicantImage.jpg";
                                data.ApplicantImage = "/UploadFiles/" + "Epass/" + model.EpassNo + "/ApplicantImage.jpg";
                            }

                            FileInfo[] idfile = new DirectoryInfo(dirpath).GetFiles("*" + "ApplicantId" + "*.*");
                            if (idfile != null)
                            {
                                data.UserIdPath = "/UploadFiles/" + "Epass/" + model.EpassNo + "/ApplicantId.jpg";
                            }

                            FileInfo[] rcfile = new DirectoryInfo(dirpath).GetFiles("*" + "VehicleRC" + "*.*");
                            if (rcfile != null)
                            {
                                data.RCPath = "/UploadFiles/" + "Epass/" + model.EpassNo + "/VehicleRC.jpg";
                            }

                            FileInfo[] dlfile = new DirectoryInfo(dirpath).GetFiles("*" + "DrivingLicence" + "*.*");
                            if (dlfile != null)
                            {
                                data.LicencePath = "/UploadFiles/" + "Epass/" + model.EpassNo + "/DrivingLicence.jpg";
                            }
                        }
                    }
                    else
                    {
                        data = new SWPEpassViewModel { StatusId = SWPReturnTypeId.NotExist };
                    }
                }
                catch (Exception e)
                {
                    data = new SWPEpassViewModel { StatusId = SWPReturnTypeId.NotExist };
                }

                
                return data;
            }
        }

        public Kendo.Mvc.UI.DataSourceResult GetAllEpassList(Kendo.Mvc.UI.DataSourceRequest request, SWPEpassViewModel model)
        {
            string host = HttpContext.Current.Request.Url.Authority;
            string protocol = HttpContext.Current.Request.Url.Scheme;

            using (var dbContext = new PIMSEntitiesContext())
            {
                var data = dbContext.Database.SqlQuery<SWPEpassViewModel>("select * from EpassMaster");


                //var data = (from pass in dbContext.EpassMasters
                //            where (model.Id == null || pass.Id == model.Id)
                //            && (model.StartDate == null || DbFunctions.TruncateTime(pass.EntryDate) >= DbFunctions.TruncateTime(model.StartDate))
                //            && (model.EndDate == null || DbFunctions.TruncateTime(pass.EntryDate) <= DbFunctions.TruncateTime(model.EndDate))
                //            //&& pass.IsActive==true
                //            select new SWPEpassViewModel
                //            {
                //                Id = pass.Id,
                //                EpassNo = pass.EpassNo,
                //                EpassType = pass.EpassType,
                //                Applicant = pass.Applicant,
                //                MobileNo = pass.MobileNo,
                //                Email = pass.Email,
                //                City = pass.City,
                //                State = pass.State,
                //                LocalPlace = pass.LocalPlace,
                //                PIN = pass.PIN,
                //                FromAddress = pass.FromAddress,
                //                ToAddress = pass.ToAddress,
                //                StartDate = pass.StartDate,
                //                EndDate = pass.EndDate,
                //                Purpose = pass.Purpose,
                //                VehicleNo = pass.VehicleNo,
                //                VehicleType = pass.VehicleType,
                //                RCNo = pass.RCNo,
                //                EntryDate = pass.EntryDate,
                //                PhotoPath = pass.PhotoPath,
                //                //LicencePath = protocol + "//" + host + "/" + pass.LicencePath.Trim(), //string.Format("{0}://{1}/{2}", protocol, host, pass.LicencePath.Trim()),
                //                RCPath = pass.RCPath,
                //                UserIdPath = pass.UserIdPath,
                //                //Status = pass.StatusId != null ? dbContext.StatusMasters.FirstOrDefault(s => s.Id == pass.StatusId).Status : string.Empty,
                //                IsActive = pass.IsActive,
                //                IsApproved = pass.IsApproved,
                //                StatusId = pass.StatusId,
                //                Approver = pass.Approver,
                //                ApprovalDate = pass.ApprovalDate,
                //                Comment = pass.Comment,
                //                ModifiedDate = pass.ModifiedDate,
                //                EPDI = pass.EPDI,
                //                EPDII = pass.EPDII,
                //                EPDIII = pass.EPDIII,
                //                EPDIV = pass.EPDIV,
                //                EPDV = pass.EPDV
                //            });

                return data.ToDataSourceResult(request);
            }
        }

        public int ValidateEpass(SWPEpassViewModel model)
        {
            var flag = SWPReturnTypeId.None;
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (model.Id != null)
                {
                    var epass = dbContext.Database.SqlQuery<SWPEpassViewModel>("select * from EpassMaster where Id=" + model.Id).FirstOrDefault();
                    if (epass != null)
                    {
                        var sqlConnectionString = ConfigurationManager.ConnectionStrings["PIMSSqlConnection"].ConnectionString;
                        using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                        {
                            using (SqlCommand command = new SqlCommand("USP_SaveEpassDetail", connection))
                            {
                                // for update epass 
                                command.CommandType = System.Data.CommandType.StoredProcedure;

                                command.Parameters.Add("@ActionTypeId", SqlDbType.Int).Value = 2;
                                command.Parameters.Add("@Id", SqlDbType.VarChar).Value = model.Id; 
                                command.Parameters.Add("@EpassNo", SqlDbType.VarChar).Value = epass.EpassNo;
                                command.Parameters.Add("@EpassType", SqlDbType.VarChar).Value = epass.EpassType;
                                command.Parameters.Add("@Applicant", SqlDbType.VarChar).Value = epass.Applicant;
                                command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = epass.MobileNo;
                                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = epass.Email;
                                command.Parameters.Add("@City", SqlDbType.VarChar).Value = epass.City;
                                command.Parameters.Add("@State", SqlDbType.VarChar).Value = epass.State;
                                command.Parameters.Add("@LocalPlace", SqlDbType.VarChar).Value = epass.LocalPlace;
                                command.Parameters.Add("@PIN", SqlDbType.VarChar).Value = epass.PIN;
                                command.Parameters.Add("@FromAddress", SqlDbType.VarChar).Value = epass.FromAddress;
                                command.Parameters.Add("@ToAddress", SqlDbType.VarChar).Value = epass.ToAddress;
                                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = DateTime.Now; //epass.StartDate;
                                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = DateTime.Now.AddDays(7); //epass.EndDate;
                                command.Parameters.Add("@Purpose", SqlDbType.VarChar).Value = epass.Purpose;
                                command.Parameters.Add("@VehicleType", SqlDbType.VarChar).Value = epass.VehicleType;
                                command.Parameters.Add("@VehicleNo", SqlDbType.VarChar).Value = epass.VehicleNo;
                                command.Parameters.Add("@RCNo", SqlDbType.VarChar).Value = epass.RCNo;
                                command.Parameters.Add("@EntryDate", SqlDbType.DateTime).Value = epass.EntryDate;
                                command.Parameters.Add("@PhotoPath", SqlDbType.VarChar).Value = epass.PhotoPath;
                                command.Parameters.Add("@UserIdPath", SqlDbType.VarChar).Value = epass.UserIdPath;
                                command.Parameters.Add("@RCPath", SqlDbType.VarChar).Value = epass.RCPath;
                                command.Parameters.Add("@LicencePath", SqlDbType.VarChar).Value = epass.LicencePath;
                                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = epass.IsActive;
                                command.Parameters.Add("@StatusId", SqlDbType.Int).Value = model.StatusId; //epass.StatusId;
                                command.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = true; //epass.IsApproved;
                                command.Parameters.Add("@ApprovalDate", SqlDbType.DateTime).Value = DateTime.Now; //epass.ApprovalDate;
                                command.Parameters.Add("@Approver", SqlDbType.VarChar).Value = userInfo.FirstName + " " + userInfo.LastName; //epass.Approver;
                                command.Parameters.Add("@Comment", SqlDbType.VarChar).Value = model.Comment; //epass.Comment;
                                command.Parameters.Add("@EPDI", SqlDbType.VarChar).Value = epass.EPDI;
                                command.Parameters.Add("@EPDII", SqlDbType.VarChar).Value = epass.EPDII;
                                command.Parameters.Add("@EPDIII", SqlDbType.VarChar).Value = epass.EPDIII;
                                command.Parameters.Add("@EPDIV", SqlDbType.VarChar).Value = epass.EPDIV;
                                command.Parameters.Add("@EPDV", SqlDbType.VarChar).Value = epass.EPDV;
                                command.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now; //epass.ModifiedDate;

                                connection.Open();
                                command.ExecuteNonQuery();

                                flag = model.StatusId == 1 ? SWPReturnTypeId.Approved : SWPReturnTypeId.Rejected;

                                var Covidstatus = model.StatusId == 1 ? "Approved" : "Rejected";
                                string message = string.Format(SWPMessage.CovidPassStatus, epass.EpassNo, Covidstatus);
                                if (!string.IsNullOrEmpty(epass.MobileNo)) SWPApplication.SendSMS(epass.MobileNo, message);
                                if (!string.IsNullOrEmpty(epass.Email)) SWPApplication.SendEmail(epass.Email, "Online Covid 19 Pass", message);
                            }
                        }
                    }
                    //var data = dbContext.EpassMasters.Where(m => m.Id == model.Id).FirstOrDefault();
                    //if (data != null)
                    //{
                    //    data.StatusId = model.StatusId;
                    //    data.IsApproved = true;
                    //    data.Comment = model.Comment;
                    //    data.Approver = userInfo.FirstName + " " + userInfo.LastName;
                    //    data.ApprovalDate = DateTime.Now;
                    //    data.ModifiedDate = DateTime.Now;
                    //    if (model.StatusId == 1)
                    //    {
                    //        data.StartDate = DateTime.Now;
                    //        data.EndDate = DateTime.Now.AddDays(7);
                    //    }

                    //    dbContext.SaveChanges();
                    //    flag = model.StatusId == 1 ? SWPReturnTypeId.Approved : SWPReturnTypeId.Rejected;

                    //    var Covidstatus = model.StatusId == 1 ? "Approved" : "Rejected";
                    //    string message = string.Format(SWPMessage.CovidPassStatus, data.EpassNo, Covidstatus);
                    //    if (!string.IsNullOrEmpty(data.MobileNo)) SWPApplication.SendSMS(data.MobileNo, message);
                    //    if (!string.IsNullOrEmpty(data.Email)) SWPApplication.SendEmail(data.Email, "Online Covid 19 Pass", message);
                    //}
                }
            }
            return flag;
        }

        public SWPEpassViewModel EpassDetails(int? Id)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                SWPEpassViewModel model = new SWPEpassViewModel();
                var details = dbContext.Database.SqlQuery<SWPEpassViewModel>("select * from EpassMaster where Id=" + Id).FirstOrDefault();
                //var details = dbContext.EpassMasters.FirstOrDefault(r => r.Id == Id);
                if (details != null)
                {
                    model.Id = details.Id;
                    model.EpassNo = details.EpassNo;
                    model.EpassType = details.EpassType == "I" ? "Individual" : "Vehicle";
                    model.Applicant = details.Applicant;
                    model.MobileNo = details.MobileNo;
                    model.Email = details.Email;
                    model.City = details.City;
                    model.State = details.State;
                    model.LocalPlace = details.LocalPlace;
                    model.PIN = details.PIN;
                    model.FromAddress = details.FromAddress;
                    model.ToAddress = details.ToAddress;
                    model.StartDate = details.StartDate;
                    model.EndDate = details.EndDate;
                    model.Purpose = details.Purpose;
                    model.VehicleNo = details.VehicleNo;
                    model.VehicleType = details.VehicleType;
                    model.RCNo = details.RCNo;
                    model.EntryDate = details.EntryDate;
                    model.PhotoPath = details.PhotoPath != null ? GetAbsoluteUrl(details.PhotoPath) : string.Empty;
                    model.LicencePath = details.LicencePath != null ? GetAbsoluteUrl(details.LicencePath) : string.Empty;
                    model.RCPath = details.RCPath != null ? GetAbsoluteUrl(details.RCPath) : string.Empty;
                    model.UserIdPath = details.UserIdPath != null ? GetAbsoluteUrl(details.UserIdPath) : string.Empty;
                    //model.Status = details.StatusId != null ? dbContext.StatusMasters.FirstOrDefault(s => s.Id == details.StatusId).Status : string.Empty;
                    model.IsActive = details.IsActive;
                    model.IsApproved = details.IsApproved;
                    model.StatusId = details.StatusId;
                    model.Approver = details.Approver;
                    model.ApprovalDate = details.ApprovalDate;
                    model.Comment = details.Comment;
                    model.ModifiedDate = details.ModifiedDate;
                    model.EPDI = details.EPDI;
                    model.EPDII = details.EPDII;
                    model.EPDIII = details.EPDIII;
                    model.EPDIV = details.EPDIV;
                    model.EPDV = details.EPDV;
                }
                return model;
            }
        }

        private string GetAbsoluteUrl(string relativePath)
        {
            try
            {
                //Get host and port by using Authority
                //Get host only by DnsSafeHost
                string host = HttpContext.Current.Request.Url.Authority;
                string protocol = HttpContext.Current.Request.Url.Scheme;
                return string.Format("{0}://{1}/{2}", protocol, host, relativePath.Trim());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SWPFormViewModel GetSchemeInformationForOnlineApplication(SWPFormViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (model.DepartmentId == SWPDepartment.Industrial)
                {
                    string schemeId = ConfigurationManager.AppSettings["IndustrialSchemeId"];
                    string departmentId = ConfigurationManager.AppSettings["IndustryDepartmentId"];
                    //OnlineFormViewModel model = new OnlineFormViewModel();
                    var scheme = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId.ToString() == schemeId && s.IsActive == true);
                    if (scheme != null)
                    {
                        model.SchemeId = scheme.schemeId;
                        model.SchemeName = scheme.schemeName;
                        model.SchemeEndDate = scheme.endDate;
                        model.ApplicationFee = scheme.FormFee;
                        model.ProcessingCharge = scheme.ProcessingFee;
                        model.FormFeeGST = scheme.FormCGST + scheme.FormSGST;
                        model.ProcessingChargeGST = scheme.ProcessingCGST + scheme.ProcessingSGST;
                        model.SchemeType = dbContext.SchemeTypeMsts.Where(x => x.schemeTypeId == scheme.schemeTypeId).Select(x => x.SchemeType).FirstOrDefault();
                    }
                    var department = dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId.ToString() == departmentId);
                    if (department != null)
                    {
                        model.DepartmentId = department.departmentId;
                        model.Department = department.departmentName;
                    }
                }
                return model;
            }
        }

        public SWPFormViewModel GetInitialDataForScheme(SWPFormViewModel formModel)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                SWPFormViewModel model = new SWPFormViewModel();
                var scheme = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId == formModel.SchemeId && s.IsActive == true);
                if (scheme != null)
                {
                    model.SchemeId = scheme.schemeId;
                    model.SchemeName = scheme.schemeName;
                    model.SchemeEndDate = scheme.endDate;
                    model.SchemeType = dbContext.SchemeTypeMsts.Where(x => x.schemeTypeId == scheme.schemeTypeId).Select(x => x.SchemeTypeDesc).FirstOrDefault();
                    model.ApplicationFormType = "General";
                }
                var department = dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == formModel.DepartmentId);
                if (department != null)
                {
                    model.DepartmentId = department.departmentId;
                    model.Department = department.departmentName;
                }
                return model;
            }
        }

        public SWPFormViewModel GetInitialDataForScheme()
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                string schemeId = ConfigurationManager.AppSettings["SchemeId"];
                string departmentId = ConfigurationManager.AppSettings["DepartmentId"];
                SWPFormViewModel model = new SWPFormViewModel();
                var scheme = dbContext.SchemeMsts.FirstOrDefault(s => s.schemeId.ToString() == schemeId && s.IsActive == true);
                if (scheme != null)
                {
                    model.SchemeId = scheme.schemeId;
                    model.SchemeName = scheme.schemeName;
                    model.SchemeEndDate = scheme.endDate;
                    model.SchemeType = dbContext.SchemeTypeMsts.Where(x => x.schemeTypeId == scheme.schemeTypeId).Select(x => x.SchemeTypeDesc).FirstOrDefault();
                }
                var department = dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId.ToString() == departmentId);
                if (department != null)
                {
                    model.DepartmentId = department.departmentId;
                    model.Department = department.departmentName;
                }
                return model;
            }
        }

        public List<SWPDropdownViewModel> GetAreaRangeByDepartment(int schemeId, int departmentId)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var schemetype = dbContext.SchemeMsts.Where(s => s.schemeId == schemeId).Select(x => x.SchemeTypeMst.SchemeTypeDesc).FirstOrDefault();
                var lst = (from scheme in dbContext.SchemeCostTrans
                           where scheme.schemeId == schemeId && scheme.departmentId == departmentId && scheme.IsActive == true
                           && scheme.FloorMst.modifiedBy == schemetype
                           select new SWPDropdownViewModel
                           {
                               Id = scheme.FloorMst.floorId,
                               Text = scheme.FloorMst.floorName
                           }).ToList();
                return lst;
            }
        }

        public SWPFormViewModel GetApplicationFeeAndCharges(int? schemeId, int? departmentId, int? propertyTypeId, int? areaTypeId)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                //var schemecost = dbContext.SchemeCostTrans.FirstOrDefault(c => c.schemeId == schemeId && c.departmentId == departmentId && c.propertyTypeId == propertyTypeId && c.floorId == areaTypeId);
                var schemecost = dbContext.SchemeCostTrans.FirstOrDefault(c => c.schemeId == schemeId && c.departmentId == departmentId && c.floorId == areaTypeId);
                var online = dbContext.OnlineAreawithRegisandProcfees.FirstOrDefault(r => r.schemeId == schemeId);
                SWPFormViewModel model = new SWPFormViewModel();
                if (schemecost != null)
                {
                    model.EarnestMoney = schemecost.earnestMoney;
                }
                if (online != null)
                {
                    model.ApplicationFee = online.ApplicationFee;
                    model.ProcessingCharge = online.ProcessingCharge;
                }

                return model;
            }
        }

        public int ValidatePANnumber(string pan, int? areaId, int? schemeId)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var form = dbContext.OnlineApplicationDetails.FirstOrDefault(o => o.isActive == true && o.pan.ToUpper() == pan.ToUpper() && o.area == areaId.ToString() && o.schemeId == schemeId);
                if (form == null) return SWPReturnTypeId.NotExist;
                else return SWPReturnTypeId.Exist;
            }
        }

        public SWPApiViewModel GetNICSingleWindowTableData(SWPFormViewModel model)
        {
            //NICsingalwindowSystem detail = new NICsingalwindowSystem();
            //if (model.ApplicationFormId != null)
            //{
            //    using (var dbContext = new PIMSEntitiesContext())
            //    {
            //        detail = dbContext.NICsingalwindowSystems.FirstOrDefault(c => c.onlineapplicationId == model.ApplicationFormId);
            //        if (detail != null)
            //        {
            //            return detail;
            //        }
            //        else { return detail; }
            //    }
            //}
            //return detail;
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (model.ApplicationFormId != null)
                {
                    var data = (from nic in dbContext.NICsingalwindowSystems
                                select new SWPApiViewModel
                                {
                                    onlineapplicationId = nic.onlineapplicationId,
                                    schemeId = nic.schemeId,
                                    Departmentid = nic.Departmentid,
                                    Control_ID = nic.Control_ID,
                                    Unit_Id = nic.Unit_Id,
                                    ServiceID = nic.ServiceID,
                                    ProcessIndustryID = nic.ProcessIndustryID,
                                    Company_Name = nic.Company_Name,
                                    Industry_District = nic.Industry_District,
                                    Industry_District_Id = nic.Industry_District_Id,
                                    Industry_Address = nic.Industry_Address,
                                    Pin_Code = nic.Pin_Code,
                                    Occupier_Name = nic.Occupier_Name,
                                    Occupier_Email_ID = nic.Occupier_Email_ID,
                                    Occupier_Mobile_No = nic.Occupier_Mobile_No,
                                    Occupier_DOB = nic.Occupier_DOB,
                                    Occupier_Gender = nic.Occupier_Gender,
                                    Occupier_Address = nic.Occupier_Address,
                                    Occupier_District_ID = nic.Occupier_District_ID,
                                    Occupier_District_Name = nic.Occupier_District_Name,
                                    Occupier_Pin_Code = nic.Occupier_Pin_Code,
                                    Nature_of_Activity = nic.Nature_of_Activity,
                                    Installed_Capacity = nic.Installed_Capacity,
                                    Employees = nic.Employees,
                                    Nature_of_Operation = nic.Nature_of_Operation,
                                    publicdecimalProject_Cost = nic.publicdecimalProject_Cost,
                                    Organization_Type_ID = nic.Organization_Type_ID,
                                    Organization_Type = nic.Organization_Type,
                                    Industry_Type_ID = nic.Industry_Type_ID,
                                    Industry_Type_Name = nic.Industry_Type_Name,
                                    Expected_date_construction = nic.Expected_date_construction,
                                    Project_Status = nic.Project_Status,
                                    Industry_Color = nic.Industry_Color,
                                    Expected_date_production = nic.Expected_date_production,
                                    Unit_Category = nic.Unit_Category,
                                    Items_Manufactured = nic.Items_Manufactured,
                                    Annual_Turnover = nic.Annual_Turnover,
                                    Fee_Amount = nic.Fee_Amount,
                                    Fee_Status = nic.Fee_Status,
                                    Status_Code = nic.Status_Code,
                                    Payment_Through = nic.Payment_Through,
                                    Payment_Description = nic.Payment_Description
                                }).FirstOrDefault();
                    return data;
                }
                else return new SWPApiViewModel { };
            }
        }


        public SWPFormViewModel SaveAllottedPropertyDocument(SWPFormViewModel model, HttpPostedFileBase allotmentLetter, HttpPostedFileBase authorityLetter)
        {
            model.Id = model.ApplicationFormId;

            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id)))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id));
            }

            if (allotmentLetter != null && allotmentLetter.ContentLength > 0)
            {
                string folderName = "Alotment";
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + folderName)))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + folderName));
                }
                else
                {
                    var dirpath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + folderName);
                    string[] fils = Directory.GetFiles(dirpath);
                    if (fils != null && fils.Count() > 0)
                    {
                        foreach (var fl in fils)
                        {
                            string path1 = dirpath + "\\" + model.Id + "-Allotment" + ".pdf";
                            if (fl == path1) { File.Delete(fl); }
                        }
                    }
                }
                string extension = Path.GetExtension(allotmentLetter.FileName);
                var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + folderName + "/" + model.Id + "-Allotment" + extension);
                allotmentLetter.SaveAs(fileSavePath);
                model.IsAllotmentLetterSaved = true;
            }
            if (authorityLetter != null && authorityLetter.ContentLength > 0)
            {
                string folderName = "Allotment";
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + folderName)))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + folderName));
                }
                else
                {
                    string fileExtn = "*Authority";
                    var dirpath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + folderName);
                    var fileList = new DirectoryInfo(dirpath).GetFileSystemInfos(fileExtn);
                    if (fileList != null && fileList.Count() > 0)
                    {
                        foreach (var fl in fileList)
                        {
                            fl.Delete();
                        }
                    }
                }
                string extension = Path.GetExtension(authorityLetter.FileName);
                var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id + "/" + folderName + "/" + model.Id + "-Authority" + extension);
                authorityLetter.SaveAs(fileSavePath);
                model.IsAuthorityLetterSaved = true;
            }

            if (model.IsAllotmentLetterSaved == true)
            {
                using(var dbContext = new PIMSEntitiesContext()){
                    var nmform = dbContext.NICsingalwindowSystems.Where(n => n.onlineapplicationId == model.ApplicationFormId).FirstOrDefault();
                    nmform.Status_Code = 6;
                    
                    dbContext.SaveChanges();
                }
            }

            return model;
        }
    }
}
