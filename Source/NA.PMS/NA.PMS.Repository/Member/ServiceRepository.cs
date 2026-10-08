using NA.PMS.Common;
using NA.PMS.Common.Extension;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Model;
using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Kendo.Mvc;
using NA.PMS.Dal.CustomerContext;
using NA.PMS.Model.Entities;
using NA.PMS.Dal.DBConnection;
using Dapper;
using NA.PMS.Common.TemplateParser;
using NA.PMS.Model.NIC;
using System.Data.Entity.Core.Objects;
using System.Configuration;
using System.IO;

namespace NA.PMS.Repository
{
    public class ServiceRepository : IServiceRepository
    {
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public ServiceRepository()
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


        public ServiceRequestViewModel SaveServiceRequestDetail(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var flag = ReturnType.None;
            switch (model.ServiceModel.ServiceId)
            {
                case NAService.Transfer:
                    flag = SaveTransferRequestService(model, files);
                    break;
                case NAService.Rent:
                    flag = SaveRentRequestService(model, files);
                    break;
                case NAService.CIC:
                    flag = SaveCICRequestService(model, files);
                    break;
                case NAService.Mortgage:
                    flag = SaveMortgageRequestService(model, files);
                    break;
                case NAService.Extension:
                    flag = SaveExtensionRequestDetails(model, files);
                    break;
                case NAService.GPA:
                    flag = SaveGPARequestService(model, files);
                    break;
                case NAService.Mutation:
                    flag = SaveMutationRequestService(model, files);
                    break;
                default:
                    flag = SaveOtherRequestService(model, files);
                    break;
            }
            return model;
        }

        private int SaveOtherRequestService(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int requestNo = SaveCustomerServiceRequestDetail(model);
                model.Id = requestNo;
                if (files != null)
                {
                    if (files.ToList().Count > 0)
                    {
                        FtpHandler.UploadFiles(files, model.ServiceModel.RegistrationId.ToString(), requestNo);
                    }
                }

                return ReturnType.Success;
            }
        }

        private int SaveMutationRequestService(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                int requestNo = SaveCustomerServiceRequestDetail(model);
                var exMutation = dbContext.OnlineTransferMutations.FirstOrDefault(m => m.RequestNo == requestNo);
                if (exMutation != null)
                {
                    exMutation.Rid = model.ServiceModel.RegistrationId;
                    exMutation.RequestNo = requestNo;
                    exMutation.Applicant_Name = model.ServiceModel.Applicant;
                    exMutation.Correspondance_Add = model.ServiceModel.ApplicantAddress;
                    exMutation.TransferDeed_Date = model.MutationModel.TransferdeedDate;
                    exMutation.Bahi_No = model.MutationModel.BahiNo;
                    exMutation.Bahi_Zild_No = model.MutationModel.BahiZildNo;
                    exMutation.Bahi_Page_No = model.MutationModel.BahiPageNo;
                    exMutation.Bahi_Series_No = model.MutationModel.BahiSeriesNo;
                    exMutation.Type = "M"; // Constants.Mutation;
                    exMutation.Modified_Date = DateTime.Now;
                    exMutation.Modified_By = userInfo.UserID;
                    exMutation.Comment = model.ServiceModel.Description;
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else
                {
                    model.Id = requestNo;

                    var mutation = new OnlineTransferMutation();
                    mutation.Rid = model.ServiceModel.RegistrationId;
                    mutation.RequestNo = requestNo;
                    mutation.Applicant_Name = model.ServiceModel.Applicant;
                    mutation.Correspondance_Add = model.ServiceModel.ApplicantAddress;
                    mutation.TransferDeed_Date = model.MutationModel.TransferdeedDate;
                    mutation.Bahi_No = model.MutationModel.BahiNo;
                    mutation.Bahi_Zild_No = model.MutationModel.BahiZildNo;
                    mutation.Bahi_Page_No = model.MutationModel.BahiPageNo;
                    mutation.Bahi_Series_No = model.MutationModel.BahiSeriesNo;
                    mutation.Type = "M"; // Constants.Mutation;
                    mutation.Created_Date = DateTime.Now;
                    mutation.Created_By = userInfo.UserID;
                    mutation.Comment = model.ServiceModel.Description;
                    dbContext.OnlineTransferMutations.Add(mutation);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }

                if (files != null)
                {
                    if (files.ToList().Count > 0)
                    {
                        FtpHandler.UploadFiles(files, model.ServiceModel.RegistrationId.ToString(), requestNo);
                    }
                }
                return flag;
            }
        }

        private int SaveGPARequestService(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                int requestNo = SaveCustomerServiceRequestDetail(model);
                var exGPA = dbContext.OnlineGPAs.FirstOrDefault(g => g.RequestNo == requestNo);
                if (exGPA != null)
                {
                    exGPA.Rid = model.ServiceModel.RegistrationId;
                    exGPA.GPA_Holder_Name = model.GPAModel.GPAHolderName;
                    exGPA.GPA_Holder_Address = model.GPAModel.GPAHolderAdd;
                    exGPA.Effcetd_From = model.GPAModel.EffectiveFrom;
                    exGPA.Effected_To = model.GPAModel.EffectiveTo;
                    exGPA.Relation = model.GPAModel.RelationName;
                    exGPA.Application_Date = model.GPAModel.ApplicationDate;
                    exGPA.Modified_By = userInfo.UserID;
                    exGPA.Modified_date = DateTime.Now;
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else
                {
                    model.Id = requestNo;
                    OnlineGPA gpa = new OnlineGPA();
                    gpa.RequestNo = requestNo;
                    gpa.Rid = model.ServiceModel.RegistrationId;
                    gpa.GPA_Holder_Name = model.GPAModel.GPAHolderName;
                    gpa.GPA_Holder_Address = model.GPAModel.GPAHolderAdd;
                    gpa.Effcetd_From = model.GPAModel.EffectiveFrom;
                    gpa.Effected_To = model.GPAModel.EffectiveTo;
                    gpa.Relation = model.GPAModel.RelationName;
                    gpa.Application_Date = model.GPAModel.ApplicationDate;
                    gpa.Created_By = userInfo.UserID;
                    gpa.Created_Date = DateTime.Now;

                    dbContext.OnlineGPAs.Add(gpa);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }

                if (files != null)
                {
                    if (files.ToList().Count > 0)
                    {
                        FtpHandler.UploadFiles(files, model.ServiceModel.RegistrationId.ToString(), requestNo);
                    }
                }
                return flag;
            }
        }

        private int SaveExtensionRequestDetails(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                int requestNo = SaveCustomerServiceRequestDetail(model);
                var exExtension = dbContext.OnlineExtensionDetails.FirstOrDefault(e => e.RequestNo == requestNo);
                if (exExtension != null)
                {
                    exExtension.Rid = model.ServiceModel.RegistrationId;
                    exExtension.RequestNo = requestNo;
                    exExtension.ExtensionDueDate = model.ExtensionModel.ExtensionDueDate;
                    exExtension.ExtensionGivenDate = model.ExtensionModel.ExtensionGivenDate;
                    //exExtension.Status = NAStatusId.Initiated;
                    exExtension.IsActive = true;
                    exExtension.ModifiedDate = DateTime.Now;
                    exExtension.ModifiedBy = userInfo.UserID;
                    exExtension.Comment = model.ServiceModel.Description;
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else
                {
                    model.Id = requestNo;
                    OnlineExtensionDetail extension = new OnlineExtensionDetail();
                    extension.Rid = model.ServiceModel.RegistrationId;
                    extension.RequestNo = requestNo;
                    extension.ExtensionDueDate = model.ExtensionModel.ExtensionDueDate;
                    extension.ExtensionGivenDate = model.ExtensionModel.ExtensionGivenDate;
                    extension.Status = NAStatusId.Initiated;
                    extension.IsActive = true;
                    extension.CreatedDate = DateTime.Now;
                    extension.CreatedBy = userInfo.UserID;
                    extension.Comment = model.ServiceModel.Description;

                    dbContext.OnlineExtensionDetails.Add(extension);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }

                if (files != null)
                {
                    if (files.ToList().Count > 0)
                    {
                        FtpHandler.UploadFiles(files, model.ServiceModel.RegistrationId.ToString(), requestNo);
                    }
                }
                return flag;
            }
        }

        private int SaveMortgageRequestService(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                int requestNo = SaveCustomerServiceRequestDetail(model);
                var exMortgage = dbContext.OnlineMortgageDetails.FirstOrDefault(m => m.RequestNo == requestNo);
                if (exMortgage != null)
                {
                    exMortgage.RID = model.ServiceModel.RegistrationId;
                    exMortgage.RequestNo = requestNo;
                    exMortgage.BankName = model.MortgageModel.BankName;
                    exMortgage.BranchAddress = model.MortgageModel.BranchAddress;
                    exMortgage.SanctionedAmount = model.MortgageModel.SanctionedAmount;
                    exMortgage.MortgageType = model.MortgageModel.MortgageType;
                    exMortgage.PreviousLoanNoc = model.MortgageModel.PreviousLoanNoc;
                    exMortgage.IsActive = true;
                    exMortgage.Comment = model.ServiceModel.Description;
                    exMortgage.CommentDate = DateTime.Now;
                    //exMortgage.StatusId = NAStatusId.Initiated;
                    exMortgage.ModifiedDate = DateTime.Now;
                    exMortgage.Modifiedby = userInfo.UserID;
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else
                {
                    model.Id = requestNo;
                    var mortgage = new OnlineMortgageDetail();
                    mortgage.RID = model.ServiceModel.RegistrationId;
                    mortgage.RequestNo = requestNo;
                    mortgage.BankName = model.MortgageModel.BankName;
                    mortgage.BranchAddress = model.MortgageModel.BranchAddress;
                    mortgage.SanctionedAmount = model.MortgageModel.SanctionedAmount;
                    mortgage.MortgageType = model.MortgageModel.MortgageType;
                    mortgage.PreviousLoanNoc = model.MortgageModel.PreviousLoanNoc;
                    mortgage.IsActive = true;
                    mortgage.CreatedDate = DateTime.Now;
                    mortgage.CreatedBy = userInfo.UserID;
                    mortgage.Comment = model.ServiceModel.Description;
                    mortgage.CommentDate = DateTime.Now;
                    mortgage.StatusId = NAStatusId.Initiated;

                    dbContext.OnlineMortgageDetails.Add(mortgage);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }

                if (files != null)
                {
                    if (files.ToList().Count > 0)
                    {
                        FtpHandler.UploadFiles(files, model.ServiceModel.RegistrationId.ToString(), requestNo);
                    }
                }

            }
            return flag;
        }

        private int SaveCICRequestService(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                int requestNo = SaveCustomerServiceRequestDetail(model);
                model.Id = requestNo;
                if (model.CICModel.ChangeTypeId == NAService.ChangeInDirector)
                {
                    var masterList = (List<DirectorShareholderModel>)HttpContext.Current.Session["TempDirectors"];
                    if (masterList != null)
                    {
                        var exDirectors = dbContext.OnlineFirmDirectorMasters.Where(f => f.RequestNo == requestNo).ToList();
                        if (exDirectors != null)
                        {
                            exDirectors.RemoveAll(r => r.RequestNo == requestNo);
                        }
                        List<OnlineFirmDirectorMaster> firmMaster = new List<OnlineFirmDirectorMaster>();
                        foreach (var master in masterList)
                        {
                            OnlineFirmDirectorMaster director = new OnlineFirmDirectorMaster();
                            director.RequestNo = requestNo;
                            director.Rid = model.ServiceModel.RegistrationId;
                            director.DirectorName = master.ShareholderName;
                            director.DirectorShare = master.ShareValue;
                            director.Type = master.ShareType;
                            director.IsActive = 1;
                            director.RequestDate = DateTime.Now;
                            director.CreatedDate = DateTime.Now;
                            director.CreatedBy = userInfo.UserID;
                            firmMaster.Add(director);
                        }
                        dbContext.OnlineFirmDirectorMasters.AddRange(firmMaster);
                        dbContext.SaveChanges();
                        if (files != null)
                        {
                            if (files.ToList().Count > 0)
                            {
                                FtpHandler.UploadFiles(files, model.ServiceModel.RegistrationId.ToString(), requestNo);
                            }
                        }
                        flag = ReturnType.Success;
                    }
                    else
                    {
                        flag = ReturnType.NotExist;
                    }
                }
                if (model.CICModel.ChangeTypeId == NAService.ChangeInFirmName)
                {
                    var exFirm = dbContext.OnlineFirmRequestMasters.FirstOrDefault(o => o.RequestNo == requestNo);
                    if (exFirm != null)
                    {
                        exFirm.Rid = model.ServiceModel.RegistrationId;
                        exFirm.OldFirmName = model.CICModel.OldFirmName;
                        exFirm.NewFirmName = model.CICModel.NewFirmName;
                        exFirm.NewFirmStatus = model.CICModel.NewFirmStatusId;
                        //exFirm.Status = NAStatusId.Initiated;
                        exFirm.IsActive = 1;
                        exFirm.RequestDate = DateTime.Now;
                        exFirm.ChangeType = NAService.ChangeInFirmName;
                        exFirm.ModifiedDate = DateTime.Now;
                        exFirm.ModifiedBy = userInfo.UserID;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                    else
                    {
                        OnlineFirmRequestMaster firm = new OnlineFirmRequestMaster();
                        firm.RequestNo = requestNo;
                        firm.Rid = model.ServiceModel.RegistrationId;
                        //firm.OldFirmName = model.CICmodel.OldFirmName;
                        firm.NewFirmName = model.CICModel.NewFirmName;
                        //firm.OldFirmStatus = model.CICmodel.OldFirmStatus;
                        firm.NewFirmStatus = model.CICModel.NewFirmStatusId;
                        firm.Status = NAStatusId.Initiated;
                        firm.IsActive = 1;
                        firm.RequestDate = DateTime.Now;
                        firm.CreatedDate = DateTime.Now;
                        firm.ChangeType = NAService.ChangeInFirmName;
                        firm.CreatedBy = userInfo.UserID;
                        dbContext.OnlineFirmRequestMasters.Add(firm);
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }

                    if (files != null)
                    {
                        if (files.ToList().Count > 0)
                        {
                            FtpHandler.UploadFiles(files, model.ServiceModel.RegistrationId.ToString(), requestNo);
                        }
                    }
                    //flag = ReturnType.Success;
                }
                if (model.CICModel.ChangeTypeId == NAService.ChangeInProduct)
                {
                    var exProduct = dbContext.OnlineFirmRequestMasters.FirstOrDefault(e => e.RequestNo == requestNo);
                    if (exProduct != null)
                    {
                        exProduct.Rid = model.ServiceModel.RegistrationId;
                        exProduct.OldFirmProduct = model.CICModel.OldFirmProduct;
                        exProduct.NewFirmProduct = model.CICModel.NewFirmProduct;
                        exProduct.Status = NAStatusId.Initiated;
                        exProduct.IsActive = 1;
                        exProduct.RequestDate = DateTime.Now;
                        exProduct.ModifiedDate = DateTime.Now;
                        exProduct.ModifiedBy = userInfo.UserID;
                        exProduct.ChangeType = NAService.ChangeInProduct;
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }
                    else
                    {
                        OnlineFirmRequestMaster firm = new OnlineFirmRequestMaster();
                        firm.RequestNo = requestNo;
                        firm.Rid = model.ServiceModel.RegistrationId;
                        //firm.OldFirmProduct = model.CICmodel.OldFirmProduct;
                        firm.NewFirmProduct = model.CICModel.NewFirmProduct;
                        firm.Status = NAStatusId.Initiated;
                        firm.IsActive = 1;
                        firm.RequestDate = DateTime.Now;
                        firm.CreatedDate = DateTime.Now;
                        firm.CreatedBy = userInfo.UserID;
                        firm.ChangeType = NAService.ChangeInProduct;

                        dbContext.OnlineFirmRequestMasters.Add(firm);
                        dbContext.SaveChanges();
                        flag = ReturnType.Success;
                    }

                    if (files != null)
                    {
                        if (files.ToList().Count > 0)
                        {
                            FtpHandler.UploadFiles(files, model.ServiceModel.RegistrationId.ToString(), requestNo);
                        }
                    }
                    //flag = ReturnType.Success;
                }
                return flag;
            }
        }

        private int SaveRentRequestService(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                int requestNo = SaveCustomerServiceRequestDetail(model);
                var exRent = dbContext.OnlineRentPermissionDetails.FirstOrDefault(r => r.RequestNo == requestNo);
                if (exRent != null)
                {
                    exRent.Rid = model.ServiceModel.RegistrationId;
                    exRent.TenantName = model.RentModel.TenantName;
                    exRent.TenantProject = model.RentModel.TenantProject;
                    exRent.RentDurationYears = model.RentModel.RentDuration;
                    exRent.RentingDate = model.RentModel.RentingDate;
                    exRent.IsActive = true;
                    //exRent.StatusId = NAStatusId.Initiated;
                    exRent.Comment = exRent.Comment + "</br>" + model.ServiceModel.Description;
                    exRent.CommentDate = DateTime.Now;
                    exRent.ModifiedDate = DateTime.Now;
                    exRent.Modifiedby = userInfo.UserID;
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }
                else
                {
                    model.Id = requestNo;
                    var rent = new OnlineRentPermissionDetail();
                    rent.Rid = model.ServiceModel.RegistrationId;
                    rent.RequestNo = requestNo;
                    rent.TenantName = model.RentModel.TenantName;
                    rent.TenantProject = model.RentModel.TenantProject;
                    rent.RentDurationYears = model.RentModel.RentDuration;
                    rent.RentingDate = model.RentModel.RentingDate;
                    rent.IsActive = true;
                    rent.CreatedDate = DateTime.Now;
                    rent.StatusId = NAStatusId.Initiated;
                    rent.Comment = model.ServiceModel.Description;
                    rent.CommentDate = DateTime.Now;
                    rent.CreatedBy = userInfo.UserID;
                    dbContext.OnlineRentPermissionDetails.Add(rent);
                    dbContext.SaveChanges();
                    flag = ReturnType.Success;
                }

                if (files != null)
                {
                    if (files.ToList().Count > 0)
                    {
                        FtpHandler.UploadFiles(files, model.ServiceModel.RegistrationId.ToString(), requestNo);
                    }
                }
                //var flag = SaveDocumentsForServiceRequest(model, files);

                return flag;
            }
        }

        private int SaveTransferRequestService(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                int requestNo = SaveCustomerServiceRequestDetail(model);
                //var exTransfer = dbContext.Succ_Mut_Trans.FirstOrDefault(t => t.Request_No == requestNo);
                var exTransfer = dbContext.OnlineTransferMutations.FirstOrDefault(t => t.RequestNo == requestNo);
                if (exTransfer != null)
                {
                    exTransfer.Rid = model.ServiceModel.RegistrationId;
                    exTransfer.Type = "T"; // Constants.Transfer;
                    exTransfer.Transfer_Type = model.TransferModel.TransferTypeId;
                    exTransfer.Transfer_Sub_Type = model.TransferModel.TransferSubTypeId;
                    //exTransfer.Created_Date = DateTime.Now;

                    if (model.TransferModel.TypeOfTransferee == Constants.Company)
                    {
                        exTransfer.T_Gender = "Company";
                        exTransfer.T_Company_Name = model.TransferModel.CompanyName;
                        exTransfer.T_Signing_Authority = model.TransferModel.SigningAuthority;
                        exTransfer.T_Registered_Office = model.TransferModel.RegisteredOffice;
                    }
                    else
                    {
                        exTransfer.T_Gender = model.TransferModel.Gender;
                        exTransfer.T_First_Name = model.TransferModel.FirstName;
                        exTransfer.T_Middle_Name = model.TransferModel.MiddleName;
                        exTransfer.T_Last_Name = model.TransferModel.LastName;
                        exTransfer.T_Father_Husband_Name = model.TransferModel.ApplicantMaster;
                        exTransfer.T_Mother_Name = model.TransferModel.MotherName;
                    }
                    exTransfer.T_Email = model.TransferModel.Email;
                    exTransfer.T_Mobile = model.TransferModel.Mobile;
                    exTransfer.T_Correspondence_Add = model.TransferModel.CorrespondenceAdd;
                    exTransfer.T_Permanent_Add = model.TransferModel.PermanentAdd;
                    exTransfer.T_Occupation_Id = model.TransferModel.OccupationId;
                    exTransfer.T_Pan = model.TransferModel.PAN;
                    exTransfer.Applicant_Name = model.ServiceModel.Applicant;
                    exTransfer.Correspondance_Add = model.ServiceModel.ApplicantAddress;
                    exTransfer.Modified_By = userInfo.UserID;
                    exTransfer.Modified_Date = DateTime.Now;
                    dbContext.SaveChanges();

                    flag = ReturnType.Success;
                }
                else
                {
                    model.Id = requestNo;
                    var transfer = new OnlineTransferMutation();
                    transfer.Rid = model.ServiceModel.RegistrationId;
                    transfer.RequestNo = requestNo;
                    //transfer.Transfer_Date = transDate;
                    transfer.Type = "T"; // Constants.Transfer;
                    transfer.Transfer_Type = model.TransferModel.TransferTypeId;
                    transfer.Transfer_Sub_Type = model.TransferModel.TransferSubTypeId;
                    transfer.Created_Date = DateTime.Now;

                    if (model.TransferModel.TypeOfTransferee == Constants.Company)
                    {
                        transfer.T_Gender = "Company";
                        transfer.T_Signing_Authority = model.TransferModel.SigningAuthority;
                        transfer.T_Registered_Office = model.TransferModel.RegisteredOffice;
                    }
                    else
                    {
                        transfer.T_Gender = model.TransferModel.Gender;
                        transfer.T_First_Name = model.TransferModel.FirstName;
                        transfer.T_Middle_Name = model.TransferModel.MiddleName;
                        transfer.T_Last_Name = model.TransferModel.LastName;
                        transfer.T_Father_Husband_Name = model.TransferModel.ApplicantMaster;
                        transfer.T_Mother_Name = model.TransferModel.MotherName;
                    }
                    transfer.T_Email = model.TransferModel.Email;
                    transfer.T_Mobile = model.TransferModel.Mobile;
                    transfer.T_Correspondence_Add = model.TransferModel.CorrespondenceAdd;
                    transfer.T_Permanent_Add = model.TransferModel.PermanentAdd;
                    transfer.T_Occupation_Id = model.TransferModel.OccupationId;
                    transfer.T_Pan = model.TransferModel.PAN;
                    //transfer.RequestNo = model.ServiceRequestId;
                    transfer.Applicant_Name = model.ServiceModel.Applicant;
                    transfer.Correspondance_Add = model.ServiceModel.ApplicantAddress;
                    transfer.Created_By = userInfo.UserID;
                    transfer.Created_Date = DateTime.Now;
                    dbContext.OnlineTransferMutations.Add(transfer);
                    dbContext.SaveChanges();

                    flag = ReturnType.Success;
                }

                if (files != null)
                {
                    if (files.ToList().Count > 0)
                    {

                        FtpHandler.UploadFiles(files, model.ServiceModel.RegistrationId.ToString(), requestNo);
                    }
                }

                return flag;
            }
        }

        private int SaveCustomerServiceRequestDetail(ServiceRequestViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.Id != null && model.Id > 0)
                {
                    var exService = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == model.Id);
                    if (exService != null)
                    {
                        exService.Registration_No = model.ServiceModel.RegistrationId.ToString();
                        exService.Property_No = !string.IsNullOrEmpty(model.ServiceModel.PropertyNo) ? model.ServiceModel.PropertyNo : model.ServiceModel.Sector + "/" + model.ServiceModel.Block + "-" + model.ServiceModel.PlotNo;
                        exService.ServiceId = model.ServiceModel.ServiceId;
                        exService.ServiceType = model.ServiceModel.RegistrationType == Constants.PradhikaranDiwas ? Constants.SDService : Constants.JSKService;
                        exService.DepartmentId = model.ServiceModel.DepartmentId;
                        exService.SectorName = model.ServiceModel.Sector;
                        exService.BlockName = model.ServiceModel.Block;
                        exService.PlotNo = model.ServiceModel.PlotNo;
                        exService.SubDepartment = model.ServiceModel.SubDepartment;
                        exService.ApplicantName = model.ServiceModel.Applicant;
                        exService.MobileNumber = model.ServiceModel.MobileNo;
                        exService.Email = model.ServiceModel.Email;
                        exService.ApplicantAddress = model.ServiceModel.ApplicantAddress;
                        exService.Description = model.ServiceModel.Description;
                        exService.IsActive = true;
                        //exService.Request_Status = NAStatusId.Initiated;
                        exService.RequestorName = model.ServiceModel.Requestor != null ? model.ServiceModel.Requestor : model.ServiceModel.Applicant;
                        exService.RequestorAddress = model.ServiceModel.RequestorAddress != null ? model.ServiceModel.RequestorAddress : model.ServiceModel.ApplicantAddress;
                        exService.Modified_Date = DateTime.Now;
                        exService.Modified_By = userInfo.UserID;
                        dbContext.SaveChanges();

                        return exService.Id;
                    }
                    else return (int)model.Id;
                }
                else
                {
                    Customer_ServiceRequest request = new Customer_ServiceRequest();
                    request.Registration_No = model.ServiceModel.RegistrationId.ToString();
                    request.Property_No = model.ServiceModel.Sector + "/" + model.ServiceModel.Block + "-" + model.ServiceModel.PlotNo;
                    request.ServiceId = model.ServiceModel.ServiceId;
                    request.ServiceType = model.ServiceModel.RegistrationType == Constants.PradhikaranDiwas ? Constants.SDService : Constants.JSKService;
                    request.DepartmentId = model.ServiceModel.DepartmentId;
                    request.SectorName = model.ServiceModel.Sector;
                    request.BlockName = model.ServiceModel.Block;
                    request.PlotNo = model.ServiceModel.PlotNo;
                    request.SubDepartment = model.ServiceModel.SubDepartment;
                    request.ApplicantName = model.ServiceModel.Applicant;
                    request.MobileNumber = model.ServiceModel.MobileNo;
                    request.Email = model.ServiceModel.Email;
                    request.ApplicantAddress = model.ServiceModel.ApplicantAddress;
                    request.Description = model.ServiceModel.Description;
                    request.IsActive = true;
                    request.Request_Status = NAStatusId.Initiated;
                    request.RequestorName = model.ServiceModel.Requestor != null ? model.ServiceModel.Requestor : model.ServiceModel.Applicant;
                    request.RequestorAddress = model.ServiceModel.RequestorAddress != null ? model.ServiceModel.RequestorAddress : model.ServiceModel.ApplicantAddress;
                    request.Created_Date = DateTime.Now;
                    request.Created_By = userInfo.UserID;
                    request.RequestThrough = userInfo.UserProfile == Constants.JSK ? Constants.JSK : userInfo.UserID.ToString();
                    dbContext.Customer_ServiceRequest.Add(request);
                    dbContext.SaveChanges();

                    model.Id = request.Id;
                    model.RegistrationId = model.ServiceModel.RegistrationId;
                    model.ServiceModel.Id = request.Id;
                    model.ServiceModel.PropertyNo = request.Property_No;
                    model.ServiceModel.PropertyNo = request.Property_No;
                    model.ServiceModel.CreatedDate = DateTime.Now;

                    string message = string.Format(NAMessages.OnlineServiceRequest, request.Id);
                    ApplicationHelper.SendSMS(request.MobileNumber, message);
                    if (!string.IsNullOrEmpty(request.Email)) ApplicationHelper.SendEmail(request.Email, "Online Request", message);

                    //int requestNo = (from req in dbContext.Customer_ServiceRequest where req.Registration_No == model.RegistrationId.ToString() && req.Description == model.Description && req.Request_Status == StatusType.Initiated && req.IsActive == true select req.Id).FirstOrDefault();
                    return request.Id;
                }
            }
        }

        private int SaveDocumentsForServiceRequest(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var flag = ReturnType.None;
            if (files != null)
            {
                if (files.ToList().Count > 0)
                {
                    FtpHandler.UploadFiles(files, model.ServiceModel.RegistrationId.ToString(), (int)model.Id);
                }
            }
            return flag;
        }


        public ServiceRequestViewModel GetServiceRequestDetailById(int? id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                ServiceRequestViewModel model = new ServiceRequestViewModel();

                var service = (from csr in dbContext.Customer_ServiceRequest
                               where csr.Id == id
                               select new ServiceViewModel
                               {
                                   Id = csr.Id,
                                   RequestId = csr.Id,
                                   RegistrationNo = csr.Registration_No,
                                   Applicant = csr.ApplicantName,
                                   ApplicantAddress = csr.ApplicantAddress,
                                   Requestor = csr.RequestorName,
                                   RequestorAddress = csr.RequestorAddress,
                                   PropertyNo = csr.Property_No,
                                   MobileNo = csr.MobileNumber,
                                   Email = csr.Email,
                                   ServiceId = csr.ServiceId,
                                   ServiceName = dbContext.CitizenService_Master.FirstOrDefault(x => x.Deptt_Id == csr.DepartmentId && x.service_id == csr.ServiceId && x.Status == 1).ServiceName,
                                   DepartmentId = csr.DepartmentId,
                                   Department = dbContext.DepartmentMsts.FirstOrDefault(x => x.departmentId == csr.DepartmentId && x.IsActive == true).departmentName,
                                   SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                   SubDepartmentId = string.IsNullOrEmpty(csr.SubDepartment) ? 1 : dbContext.SubDepartmentMsts.FirstOrDefault(x => x.departmentId == csr.DepartmentId && x.SubdepartmentName == csr.SubDepartment && x.IsActive == true).SubdepartmentId,
                                   Description = csr.Description,
                                   RegistrationType = csr.ServiceType == Constants.SDService ? Constants.PradhikaranDiwas : (string.IsNullOrEmpty(csr.Registration_No) ? Constants.UnRegistered : Constants.Registered),
                                   ServiceStatusId = csr.Request_Status,
                                   RequestStatus = dbContext.StatusMasters.FirstOrDefault(m => m.Id == csr.Request_Status && m.IsActive == true).Status,
                                   Comment = string.Empty,
                                   RequestComment=csr.Comment,
                                   ApproverId = csr.ApproverId,
                                   ValidatorId = csr.ValidatorId,
                                   ValidationDate = csr.ValidatedDate,
                                   ApprovalDate = csr.ApprovalDate,
                                   CreatedDate = csr.Created_Date,
                                   Approver = csr.ApproverId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == csr.ApproverId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == csr.ApproverId).LastName) : string.Empty,
                                   Validator = csr.ValidatorId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == csr.ValidatorId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == csr.ValidatorId).LastName) : string.Empty,
                                   StatusId = csr.Request_Status,
                                   PaymentStatus = csr.PaymentStatus,
                                   IsUploaded = csr.IsUploadedLetter,
                                   RequestThrough = csr.RequestThrough,
                                   DispatchedDocument = csr.DispatchDocumentName,
                                   UploadedDocument = csr.UploadedDocumentName,
                                   ModifiedDate = csr.Modified_Date,
                                   ObjectionStatus = csr.ObjectionStatus
                               }).FirstOrDefault();
                if (service != null)
                {
                    if (service.ApprovalDate != null)
                    {
                        if (service.StatusId == NAStatusId.Completed || service.StatusId == NAStatusId.Approved)
                        {
                            service.PendencyLevel = "Completed By - " + service.Approver;
                        }
                        else if (service.StatusId == NAStatusId.Rejected)
                        {
                            service.PendencyLevel = "Rejected By " + service.Approver;
                        }
                        else
                        {
                            service.PendencyLevel = "Cancelled By - " + service.Approver;
                        }
                    }
                    else
                    {
                        if (service.StatusId == NAStatusId.Forwarded)
                        {
                            service.PendencyLevel = "Forwarded To - " + service.Approver;
                        }
                        else if (service.StatusId == NAStatusId.Pending)
                        {
                            service.PendencyLevel = "Put on Pending By - " + service.Approver;
                        }
                        else if (service.StatusId == NAStatusId.Objection)
                        {
                            service.PendencyLevel = "Put on Objection By - " + service.Approver;
                        }
                        else if (service.StatusId == NAStatusId.Initiated)
                        {
                            service.PendencyLevel = "Service Request is in Progress.";
                        }
                        else if (service.StatusId == NAStatusId.Withdrawn)
                        {
                            service.PendencyLevel = "Withdrawn by - Allottee";
                        }
                        else if (service.StatusId == NAStatusId.Resubmitted)
                        {
                            service.PendencyLevel = "Resubmitted By - Allottee";
                        }
                        else if (service.StatusId == NAStatusId.Appointment)
                        {
                            service.PendencyLevel = "Put on Appointment By - " + service.Approver;
                        }
                        else
                        {
                            service.PendencyLevel = null;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(service.RegistrationNo)) service.RegistrationId = Convert.ToInt32(service.RegistrationNo);

                //if (!string.IsNullOrEmpty(service.Property_No))
                //{
                //    var property = service.Property_No.Split('/');
                //    if (property != null)
                //    {
                //        service.Sector = property[0];
                //        var blck = property[1].Split('-');
                //        if (blck != null)
                //        {
                //            service.Block = blck[0];
                //            service.PlotNo = blck[1];
                //        }
                //    }
                //}
                model.Id = service.Id;
                model.RegistrationId = !string.IsNullOrEmpty(service.RegistrationNo) ? Convert.ToInt32(service.RegistrationNo) : 0;
                model.OnlineRequestId = service.Id;
                model.ServiceModel = service;

                // get detail by service id
                switch (service.ServiceId)
                {
                    case NAService.Transfer:
                        model = GetTransferRequestServiceDetail(model);
                        break;
                    case NAService.Rent:
                        model = GetRentRequestServiceDetail(model);
                        break;
                    case NAService.CIC:
                        model = GetCICRequestServiceDetail(model);
                        break;
                    case NAService.Mortgage:
                        model = GetMortgageRequestServiceDetail(model);
                        break;
                    case NAService.Extension:
                        model = GetExtensionRequestDetails(model);
                        break;
                    case NAService.GPA:
                        model = GetGPARequestServiceDetail(model);
                        break;
                    case NAService.Mutation:
                        model = GetMutationRequestServiceDetail(model);
                        break;
                    default:
                        //model = model;
                        break;
                }
                return model;
            }
        }

        private ServiceRequestViewModel GetTransferRequestServiceDetail(ServiceRequestViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var transfer = dbContext.OnlineTransferMutations.Where(m => m.RequestNo == model.Id && m.Type == "T").FirstOrDefault();
                if (transfer != null)
                {
                    TransferViewModel tmodel = new TransferViewModel
                    {
                        TransferTypeId = transfer.Transfer_Type.Value,
                        TransferType = dbContext.Transfer_Type.Where(t => t.Id == transfer.Transfer_Type).Select(t => t.type).FirstOrDefault(),
                        TransferSubTypeId = transfer.Transfer_Sub_Type.Value,
                        TransferSubType = dbContext.Transfer_Type.Where(t => t.Id == transfer.Transfer_Sub_Type).Select(t => t.type).FirstOrDefault(),
                        Gender = (transfer.T_Gender == Constants.Male || transfer.T_Gender == Constants.Female) ? Constants.Individual : Constants.Company,
                        ApplicantType = transfer.T_Gender,
                        FirstName = transfer.T_First_Name,
                        MiddleName = transfer.T_Middle_Name,
                        LastName = transfer.T_Middle_Name,
                        Applicant = transfer.T_Gender == Constants.Company ? transfer.T_Company_Name : transfer.T_First_Name + " " + (string.IsNullOrEmpty(transfer.T_Middle_Name) ? string.Empty : transfer.T_Middle_Name + " ") + transfer.T_Last_Name,
                        CompanyName = transfer.T_Company_Name,
                        SigningAuthority = transfer.T_Signing_Authority,
                        RegisteredOffice = transfer.T_Registered_Office,
                        ApplicantMaster = transfer.T_Father_Husband_Name,
                        MotherName = transfer.T_Mother_Name,
                        PermanentAdd = transfer.T_Permanent_Add,
                        CorrespondenceAdd = transfer.T_Correspondence_Add,
                        PAN = transfer.T_Pan,
                        OccupationId = transfer.T_Occupation_Id,
                        Occupation = dbContext.OccupationMsts.Where(m => m.occupationId == transfer.T_Occupation_Id).Select(o => o.occupation).FirstOrDefault(),
                        Mobile = transfer.T_Mobile,
                        Email = transfer.T_Email,
                        TypeOfTransferee = (transfer.T_Gender == Constants.Male || transfer.T_Gender == Constants.Female) ? Constants.Individual : Constants.Company
                    };

                    model.TransferModel = tmodel;
                }
                else
                {
                    model.TransferModel = new TransferViewModel();
                }
                return model;
            }
        }

        private ServiceRequestViewModel GetRentRequestServiceDetail(ServiceRequestViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var rents = dbContext.OnlineRentPermissionDetails.Where(r => r.RequestNo == model.Id).FirstOrDefault();
                if (rents != null)
                {
                    RentingViewModel rent = new RentingViewModel
                    {
                        TenantName = rents.TenantName,
                        TenantProject = rents.TenantProject,
                        RentDuration = rents.RentDurationYears,
                        RentingDate = rents.RentingDate
                    };
                    model.RentModel = rent;
                    //return model;
                }
                else
                {
                    model.RentModel = new RentingViewModel();
                }
                return model;
            }
        }

        private ServiceRequestViewModel GetCICRequestServiceDetail(ServiceRequestViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var directors = dbContext.OnlineFirmDirectorMasters.Where(m => m.RequestNo == model.Id && m.IsActive == 1).ToList();
                var firms = dbContext.OnlineFirmRequestMasters.Where(m => m.RequestNo == model.Id && m.IsActive == 1).FirstOrDefault();
                CICViewModel cic = new CICViewModel();
                if (directors != null)
                {
                    List<DirectorShareholderModel> dlist = new List<DirectorShareholderModel>();
                    foreach (var drtor in directors)
                    {
                        dlist.Add(new DirectorShareholderModel
                        {
                            ShareType = drtor.Type,
                            ShareholderName = drtor.DirectorName,
                            ShareValue = drtor.DirectorShare
                        });
                    }
                    //model.CICModel.DirectorsModel = dlist;
                    cic.ChangeTypeId = NAService.ChangeInDirector;
                    model.CICModel = cic;
                    return model;
                }
                if (firms != null && firms.ChangeType == NAService.ChangeInFirmName)
                {
                    cic.OldFirmName = firms.OldFirmName;
                    cic.NewFirmName = firms.NewFirmName;
                    cic.ChangeTypeId = NAService.ChangeInFirmName;
                    model.CICModel = cic;
                    //return model;
                }
                if (firms != null && firms.ChangeType == NAService.ChangeInProduct)
                {
                    cic.OldFirmProduct = firms.OldFirmProduct;
                    cic.NewFirmProduct = firms.NewFirmProduct;
                    cic.ChangeTypeId = NAService.ChangeInProduct;
                    model.CICModel = cic;
                    //return model;
                }
                else
                {
                    model.CICModel = cic;
                }
                return model;
            }
        }

        private ServiceRequestViewModel GetMortgageRequestServiceDetail(ServiceRequestViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.OnlineMortgageDetails.Where(m => m.RequestNo == model.Id).FirstOrDefault();
                if (data != null)
                {
                    MortgageViewModel mortgage = new MortgageViewModel
                    {
                        SanctionedAmount = data.SanctionedAmount,
                        BankName = data.BankName,
                        BranchAddress = data.BranchAddress,
                        MortgageType = data.MortgageType,
                        PreviousLoanNoc = data.PreviousLoanNoc
                    };
                    model.MortgageModel = mortgage;
                    //return model;
                }
                else
                {
                    model.MortgageModel = new MortgageViewModel();
                }

                return model;
            }
        }

        private ServiceRequestViewModel GetExtensionRequestDetails(ServiceRequestViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.OnlineExtensionDetails.Where(m => m.RequestNo == model.Id).FirstOrDefault();
                if (data != null)
                {
                    ExtensionViewModel extension = new ExtensionViewModel
                    {
                        ExtensionDueDate = data.ExtensionDueDate,
                        ExtensionGivenDate = data.ExtensionGivenDate
                    };
                    model.ExtensionModel = extension;
                    //return model;
                }
                else
                {
                    model.ExtensionModel = new ExtensionViewModel();
                }
                return model;
            }
        }

        private ServiceRequestViewModel GetGPARequestServiceDetail(ServiceRequestViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.OnlineGPAs.Where(m => m.RequestNo == model.Id).FirstOrDefault();
                if (data != null)
                {
                    GPAViewModel gpa = new GPAViewModel
                    {
                        Id = data.GpaSid,
                        GPAId = data.GpaSid,
                        EffectiveFrom = data.Effcetd_From,
                        EffectiveTo = data.Effected_To,
                        RelationName = data.Relation,
                        ApplicationDate = data.Application_Date,
                        GPAHolderName = data.GPA_Holder_Name,
                        GPAHolderAdd = data.GPA_Holder_Address,
                        IsGPARegistered = data.Registered,
                        GPARegisteredNo = data.Registration_No,
                        IsGPAActive = data.Is_Active
                    };
                    model.GPAModel = gpa;
                    //return model;
                }
                else
                {
                    model.GPAModel = new GPAViewModel();
                }
                return model;
            }
        }

        private ServiceRequestViewModel GetMutationRequestServiceDetail(ServiceRequestViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = dbContext.OnlineTransferMutations.Where(m => m.RequestNo == model.Id).FirstOrDefault();
                if (data != null && data.Type == Constants.Mutation)
                {
                    TransferViewModel mutation = new TransferViewModel
                    {
                        TransferdeedDate = data.TransferDeed_Date,
                        BahiNo = data.Bahi_No,
                        BahiZildNo = data.Bahi_Zild_No,
                        BahiPageNo = data.Bahi_Page_No,
                        BahiSeriesNo = data.Bahi_Series_No
                    };
                    model.MutationModel = mutation;
                    //return model;
                }
                else
                {
                    model.MutationModel = new TransferViewModel();
                }

                return model;
            }
        }



        public DataSourceResult GetServiceRequestReport(DataSourceRequest request, int? departmentId, DateTime? fromDate, DateTime? toDate)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var lst = (from csr in dbContext.View_Service_report
                           where csr.ServiceType == "P"
                          && (departmentId == null || departmentId == 0 || csr.DepartmentId == departmentId)
                                && (fromDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(fromDate))
                                && (toDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(toDate))
                           select new ServiceReportModel
                           {
                               Id = csr.requestNo,
                               RegistrationNo = csr.Registration_No,
                               CreatedDate = csr.Created_Date.Value,
                               Description = csr.Description,
                               ApplicantName = csr.PRDVREGIST_APPLICANT_NAME,
                               Sector = csr.SECTOR,
                               Block = csr.BLOCK,
                               PropertyNo = csr.PLDIPROPERTY_NO,
                               ServiceName = csr.ServiceName,
                               Email = csr.Email,
                               MobileNo = csr.MobileNumber,
                               //DepartmentName = csr.DepartmentId != 1 ? (csr.DepartmentId != 2 ? (csr.DepartmentId != 3 ? (csr.DepartmentId != 4 ? (csr.DepartmentId != 5 ? "Group Housing" : "Housing") : "Industrial") : "Residential") : "Commercial") : "Institutional",
                               DepartmentId = csr.DepartmentId.Value,
                               DepartmentName = dbContext.DepartmentMsts.FirstOrDefault(x => x.departmentId == csr.DepartmentId.Value).departmentName,
                               Status = csr.RequestName,
                               SubDepartment = csr.SubDepartment,
                               UploadedDocumentName = !(string.IsNullOrEmpty(csr.UploadedDocumentName)) ? csr.UploadedDocumentName : string.Empty,
                               DispatchDocumentName = !(string.IsNullOrEmpty(csr.DispatchDocumentName)) ? csr.DispatchDocumentName : string.Empty
                           }).Distinct();
                var data = lst.ToDataSourceResult(request);
                return data;
            }

            //using (var dbContext = new NoidaPMSEntities())
            //{
            //    var list = (from serv in dbContext.Customer_ServiceRequest
            //                join csm in dbContext.CitizenService_Master on new { x = serv.DepartmentId, y = serv.ServiceId } equals new { x = csm.Deptt_Id, y = csm.service_id }
            //                where serv.ServiceType == Constants.SDService && csm.Status == 1
            //                && (departmentId == null || serv.DepartmentId == departmentId)
            //                && (fromDate == null || DbFunctions.TruncateTime(serv.Created_Date) >= DbFunctions.TruncateTime(fromDate))
            //                && (toDate == null || DbFunctions.TruncateTime(serv.Created_Date) <= DbFunctions.TruncateTime(toDate))
            //                select new ServiceViewModel
            //                {
            //                    Id = serv.Id,
            //                    RequestId = serv.Id,
            //                    RegistrationNo = serv.Registration_No,
            //                    DepartmentId = serv.DepartmentId,
            //                    Department = (serv.DepartmentId == null || serv.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == serv.DepartmentId).departmentName,
            //                    PropertyNo = serv.Property_No != null ? serv.Property_No : "",
            //                    RequestDate = serv.Created_Date,
            //                    CreatedDate = serv.Created_Date,
            //                    Timeline = csm.Timeline,
            //                    ServiceId = serv.ServiceId,
            //                    ServiceName = csm.ServiceName,
            //                    ServiceType = serv.ServiceType,
            //                    DuesAmount = serv.DuesAmount == null ? 0 : serv.DuesAmount,
            //                    Amount = serv.DuesAmount,
            //                    Status = (serv.Request_Status != null || serv.Request_Status != 0) ? dbContext.StatusMasters.FirstOrDefault(x => x.Id == serv.Request_Status).Status : string.Empty,
            //                    Comment = serv.Comment,
            //                    Email = serv.Email,
            //                    MobileNo = serv.MobileNumber,
            //                    Applicant = serv.ApplicantName,
            //                    Description = serv.Description,
            //                    Requestor = serv.RequestorName,
            //                    RequestorAddress = serv.RequestorAddress,
            //                    SubDepartment = string.IsNullOrEmpty(serv.SubDepartment) ? SubDepartment.Property : serv.SubDepartment,
            //                    UploadedDocument = !(string.IsNullOrEmpty(serv.UploadedDocumentName)) ? serv.UploadedDocumentName : string.Empty,
            //                    DispatchedDocument = !(string.IsNullOrEmpty(serv.DispatchDocumentName)) ? serv.DispatchDocumentName : string.Empty
            //                });
            //    var data = list.ToDataSourceResult(request);
            //    return data;
            //}
        }

        public DataSourceResult GetServicerequestUploadedDocuments(DataSourceRequest request, int? RequestId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<DocumentViewModel> documentList = new List<DocumentViewModel>();
                var service = dbContext.Customer_ServiceRequest.FirstOrDefault(f => f.Id == RequestId);
                if (service != null)
                {
                    FtpHandler ftpHandler = new FtpHandler();
                    documentList = ftpHandler.GetServiceRequestDocuments((int)RequestId);
                    return documentList.ToDataSourceResult(request);
                }
                return null;
            }
        }

        public ServiceRequestViewModel UploadServiceRequestDocuments(ServiceRequestViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            FtpHandler.UploadFiles(files, model.ServiceModel.RequestId.ToString());
            if (!string.IsNullOrEmpty(files.FirstOrDefault().FileName))
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    var ServiceRequest = dbContext.Customer_ServiceRequest.Where(m => m.Id == model.ServiceModel.RequestId && m.ServiceType == "P" && m.IsActive == true).FirstOrDefault();
                    if (ServiceRequest != null)
                    {
                        ServiceRequest.UploadedDocumentName = files.FirstOrDefault().FileName;
                        dbContext.SaveChanges();
                    }
                }
            }
            return model;
        }

        public bool UpdateStatus(ServiceRequestViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            bool flag = false;
            if (files != null)
            {
                if (files.Count() > 0)
                {
                    UploadServiceRequestDocuments(model, files);
                    if (!string.IsNullOrEmpty(files.FirstOrDefault().FileName))
                    {
                        using (var dbContext = new NoidaPMSEntities())
                        {
                            var ServiceRequest = dbContext.Customer_ServiceRequest.Where(m => m.Id == model.ServiceModel.RequestId && m.ServiceType == "P" && m.IsActive == true).FirstOrDefault();
                            if (ServiceRequest != null)
                            {
                                ServiceRequest.DispatchDocumentName = files.FirstOrDefault().FileName;
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }

            using (var dbContext = new NoidaPMSEntities())
            {
                string sComment = string.Empty;
                var username = userInfo.UserID.ToString() + "-" + userInfo.FirstName + " " + userInfo.MiddleName + " " + userInfo.LastName + " Dated:- " + DateTime.Now.ToString("dd-MMM-yyyy");
                username = username.Trim();
                var ServiceRequest = dbContext.Customer_ServiceRequest.Where(m => m.Id == model.ServiceModel.RequestId && m.IsActive == true).FirstOrDefault();
                if (ServiceRequest != null)
                {
                    if (ServiceRequest.Request_Status == 5)//if status is In progress
                    {
                        if (model.ServiceModel.ServiceStatusId != ServiceRequest.Request_Status)
                        {
                            ServiceRequest.Request_Status = model.ServiceModel.ServiceStatusId;
                            string reqName = dbContext.StatusMasters.Where(m => m.Id == model.ServiceModel.ServiceStatusId).FirstOrDefault().Status;
                            string message = string.Format(NAMessages.SDServiceReqStatusChange, model.ServiceModel.RequestId, reqName);
                            if (!string.IsNullOrEmpty(ServiceRequest.MobileNumber)) { ApplicationHelper.SendSMS(ServiceRequest.MobileNumber, message); }
                            if (!string.IsNullOrEmpty(ServiceRequest.Email)) { ApplicationHelper.SendEmail(ServiceRequest.Email, "Online Request", message); }
                        }
                        if (!string.IsNullOrEmpty(model.ServiceModel.Comment))
                        {
                            string sAddComment = "Comment:" + model.ServiceModel.Comment + " " + username;
                            if (string.IsNullOrEmpty(ServiceRequest.Comment)) { sComment = sAddComment; }
                            else { sComment = (sAddComment + "#n#" + ServiceRequest.Comment).Replace("#n#", System.Environment.NewLine); }
                            ServiceRequest.Comment = sComment;
                        }
                        dbContext.SaveChanges();
                        flag = true;
                    }
                }
            }
            return flag;
        }

        public bool UpdateServiceReq(ServiceRequestModel ObjServiceReq)
        {
            bool flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var ServiceRequestDetails = dbContext.Customer_ServiceRequest.FirstOrDefault(m => m.Id == ObjServiceReq.ServiceRequestId && m.ServiceType == "P" && m.IsActive == true);
                if (ServiceRequestDetails != null)
                {
                    ServiceRequestDetails.Registration_No = ObjServiceReq.RegistrationId.ToString();
                    ServiceRequestDetails.Property_No = ObjServiceReq.Sector + "/" + ObjServiceReq.Block + "-" + ObjServiceReq.PlotNo;
                    ServiceRequestDetails.ApplicantName = ObjServiceReq.ApplicantName;
                    ServiceRequestDetails.ApplicantAddress = ObjServiceReq.ApplicantAddress;
                    ServiceRequestDetails.ServiceId = ObjServiceReq.ServiceId;
                    ServiceRequestDetails.DepartmentId = ObjServiceReq.DepartmentId;
                    ServiceRequestDetails.MobileNumber = ObjServiceReq.Mobile;
                    ServiceRequestDetails.Email = ObjServiceReq.Email;
                    ServiceRequestDetails.SubDepartment = ObjServiceReq.SubDepartmentId.ToString();
                    ServiceRequestDetails.Description = ObjServiceReq.Description;
                    ServiceRequestDetails.Modified_By = userInfo.UserID;
                    ServiceRequestDetails.Modified_Date = DateTime.Now;
                    dbContext.SaveChanges();

                    string message = string.Format(NAMessages.SDServiceReqUpdate, ServiceRequestDetails.Id);
                    ApplicationHelper.SendSMS(ServiceRequestDetails.MobileNumber, message);
                    if (!string.IsNullOrEmpty(ServiceRequestDetails.Email)) ApplicationHelper.SendEmail(ServiceRequestDetails.Email, "Online Request Updated", message);

                    flag = true;
                }
            }
            return flag;
        }


        public ApplicantViewModel GetApplicantDetailsByRegistrationId(int? registrationId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var applicant = (from alotment in dbContext.AllotmentMasters
                                 join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                                 join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                                 where alotment.rid == registrationId && alotment.isActive == 1
                                 select new ApplicantViewModel
                                 {
                                     RegistrationId = alotment.rid,
                                     DepartmentId = alotment.departmentId,
                                     Department = alotment.DepartmentMst.departmentName,
                                     SectorId = property.sectorId,
                                     Sector = property.SectorMst.sectorName,
                                     BlockId = property.blockId,
                                     Block = property.BlockMst.blockName,
                                     PlotNo = property.propertyNo,
                                     Applicant = aplicant.tGender == Constants.Company ? aplicant.T_Company_Name : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                     Mobile = aplicant.tMobileNumber,
                                     Email = aplicant.tEmail,
                                     PermanentAddress = aplicant.tPermanentAdd,
                                     CorrespondAddress = aplicant.tCorrespondanceAdd
                                 }).FirstOrDefault();
                return applicant;
            }
        }

        public List<DirectorShareholderModel> SaveDirectorOrShareholders(string directorName, decimal? share, string shareType)
        {
            try
            {
                var directors = (List<DirectorShareholderModel>)HttpContext.Current.Session["TempDirectors"];
                if (directors == null)
                {
                    directors = new List<DirectorShareholderModel>();
                    directors.Add(new DirectorShareholderModel
                    {
                        Id = 1,
                        ShareType = Convert.ToInt32(shareType),
                        ShareholderName = directorName,
                        ShareValue = share
                    });
                    HttpContext.Current.Session["TempDirectors"] = directors;
                }
                else
                {
                    var flag = directors.Count;
                    directors.Add(new DirectorShareholderModel
                    {
                        Id = flag + 1,
                        ShareType = Convert.ToInt32(shareType),
                        ShareholderName = directorName,
                        ShareValue = share
                    });
                    HttpContext.Current.Session["TempDirectors"] = directors;
                }
                return directors;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<ServiceCheckListModel> GetChecklistOptionsForFileUpload(int? departmentId, int? serviceId)
        {
            using (var context = new NoidaPMSEntities())
            {
                var checklists = (from checklist in context.ServiceCheckList_Master
                                  where checklist.dept_id == departmentId && checklist.service_id == serviceId
                                  select new ServiceCheckListModel
                                  {
                                      Id = checklist.ChkId,
                                      DepartmentId = checklist.dept_id,
                                      Department = context.DepartmentMsts.Where(d => d.departmentId == departmentId).Select(d => d.departmentName).FirstOrDefault(),
                                      ServiceId = checklist.service_id,
                                      ChecklistRefNo = checklist.Checklist_Ref,
                                      ChecklistName = checklist.ChkName,
                                      Status = checklist.Status
                                  }).ToList();
                return checklists;
            }
        }


        public ServiceRequestModel SaveServiceRequestForSamadhanDiwas(ServiceRequestModel model, IEnumerable<HttpPostedFileBase> files)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                Customer_ServiceRequest service = new Customer_ServiceRequest();
                service.Registration_No = model.RegistrationId.ToString();
                service.Property_No = model.Sector + "/" + model.Block + "-" + model.PlotNo;
                service.ApplicantName = model.ApplicantName;
                service.ApplicantAddress = model.ApplicantAddress;
                service.RequestorName = model.ApplicantName;
                service.RequestorAddress = model.ApplicantAddress;
                //service.RequestorName = model.Requestor;
                //service.RequestorAddress = model.RequestorAddress;
                service.ServiceId = model.ServiceId;
                service.DepartmentId = model.DepartmentId;
                service.MobileNumber = model.Mobile;
                service.Email = model.Email;
                if ((model.RegistrationType == Constants.Registered) || (model.RegistrationType == Constants.UnRegistered)) service.ServiceType = Constants.JSKService;
                else service.ServiceType = Constants.SDService;
                //service.SubDepartment = model.SubDepartmentId.ToString();
                service.SubDepartment = model.SubDepartment;
                service.Description = model.Description;
                service.Request_Status = Constants.InProgress;
                service.Created_By = userInfo.UserID;
                service.Created_Date = DateTime.Now;
                service.IsActive = true;

                dbContext.Customer_ServiceRequest.Add(service);
                dbContext.SaveChanges();

                model.Id = service.Id;
                model.Created_Date = service.Created_Date;
                model.Property_No = service.Property_No;

                //string message = string.Format(NAMessages.OnlineServiceRequest, service.Id);
                //ApplicationHelper.SendSMS(service.MobileNumber, message);
                //if (!string.IsNullOrEmpty(service.Email)) ApplicationHelper.SendEmail(service.Email, "Online Request", message);

                ////SaveFilesByRequestId(files, service.Id);

                //FtpHandler.UploadFiles(files, service.Id.ToString());
            }
            return model;
        }


        public ServiceRequestViewModel GetServiceRequestDetailForCustomer(int requestId, string mobile)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                ServiceRequestViewModel model = new ServiceRequestViewModel();

                var service = (from csr in dbContext.Customer_ServiceRequest
                               where csr.Id == requestId
                               select new ServiceViewModel
                               {
                                   Id = csr.Id,
                                   RequestId = csr.Id,
                                   RegistrationNo = csr.Registration_No,
                                   Applicant = csr.ApplicantName,
                                   ApplicantAddress = csr.ApplicantAddress,
                                   Requestor = csr.RequestorName,
                                   RequestorAddress = csr.RequestorAddress,
                                   PropertyNo = csr.Property_No,
                                   MobileNo = csr.MobileNumber,
                                   Email = csr.Email,
                                   ServiceId = csr.ServiceId,
                                   ServiceName = dbContext.CitizenService_Master.FirstOrDefault(x => x.Deptt_Id == csr.DepartmentId && x.service_id == csr.ServiceId && x.Status == 1).ServiceName,
                                   DepartmentId = csr.DepartmentId,
                                   Department = dbContext.DepartmentMsts.FirstOrDefault(x => x.departmentId == csr.DepartmentId && x.IsActive == true).departmentName,
                                   SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                   SubDepartmentId = string.IsNullOrEmpty(csr.SubDepartment) ? 1 : dbContext.SubDepartmentMsts.FirstOrDefault(x => x.departmentId == csr.DepartmentId && x.SubdepartmentName == csr.SubDepartment && x.IsActive == true).SubdepartmentId,
                                   Description = csr.Description,
                                   RegistrationType = csr.ServiceType == Constants.SDService ? "AuthorityDay" : "Registered",
                                   ServiceStatusId = csr.Request_Status,
                                   RequestStatus = dbContext.StatusMasters.FirstOrDefault(m => m.Id == csr.Request_Status && m.IsActive == true).Status,
                                   Comment = csr.Comment
                               }).FirstOrDefault();
                //service.SubDepartmentId = service.SubDepartment == "P" ? 1 : Convert.ToInt32(service.SubDepartment);
                //service.SubDepartmentId = string.IsNullOrEmpty(service.SubDepartment) ? 1 : dbContext.SubDepartmentMsts.FirstOrDefault(x => x.SubdepartmentName == service.SubDepartment).SubdepartmentId;
                //if (service.SubDepartmentId != null)
                //{
                //    service.SubDepartment = dbContext.SubDepartmentMsts.FirstOrDefault(s => s.departmentId == service.DepartmentId && s.SubdepartmentId == service.SubDepartmentId && s.IsActive == true).SubdepartmentName;
                //}
                if (!string.IsNullOrEmpty(service.RegistrationNo)) service.RegistrationId = Convert.ToInt32(service.RegistrationNo);
                if (!string.IsNullOrEmpty(service.PropertyNo))
                {
                    var property = service.PropertyNo.Split('/');
                    if (property != null)
                    {
                        service.Sector = property[0];
                        var blck = property[1].Split('-');
                        if (blck != null)
                        {
                            service.Block = blck[0];
                            service.PlotNo = blck[1];
                        }
                    }
                }
                model.ServiceModel = service;

                return model;
            }
        }


        public DataSourceResult GetDirectorShareholderDataList(DataSourceRequest request)
        {
            try
            {
                var directors = (List<DirectorShareholderModel>)HttpContext.Current.Session["TempDirectors"];
                if (directors != null)
                {
                    return directors.ToDataSourceResult(request);
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public int RemoveDirectorShareholderFromList(int id)
        {
            int flag = ReturnType.None;
            try
            {
                var directors = (List<DirectorShareholderModel>)HttpContext.Current.Session["TempDirectors"];
                if (directors != null)
                {
                    directors.RemoveAt(id - 1);
                    flag = ReturnType.Success;
                }
                else
                    flag = ReturnType.NotExist;

                return flag;
            }
            catch (Exception e)
            {
                return flag;
            }
        }


        public ServiceRequestViewModel SaveServiceRequestForSamadhanDiwas(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                Customer_ServiceRequest service = new Customer_ServiceRequest();
                service.Registration_No = model.ServiceModel.RegistrationId.ToString();
                service.Property_No = model.ServiceModel.Sector + "/" + (model.ServiceModel.Block == null ? "NA" : model.ServiceModel.Block) + "-" + model.ServiceModel.PlotNo;
                service.ApplicantName = model.ServiceModel.Applicant;
                service.ApplicantAddress = model.ServiceModel.ApplicantAddress;
                service.RequestorName = model.ServiceModel.Applicant;
                service.RequestorAddress = model.ServiceModel.ApplicantAddress;
                //service.RequestorName = model.ServiceModel.Requestor;
                //service.RequestorAddress = model.ServiceModel.RequestorAddress;
                service.ServiceId = model.ServiceModel.ServiceId;
                service.DepartmentId = model.ServiceModel.DepartmentId;
                service.MobileNumber = model.ServiceModel.MobileNo;
                service.Email = model.ServiceModel.Email;
                service.ServiceType = Constants.SDService;
                service.SubDepartment = model.ServiceModel.SubDepartment;
                //service.SubDepartment = model.ServiceModel.SubDepartmentId.ToString();
                service.Description = model.ServiceModel.Description;
                service.Request_Status = Constants.InProgress;
                service.Created_By = userInfo.UserID;
                service.Created_Date = DateTime.Now;
                service.IsActive = true;
                //service.UploadedDocumentName = null;
                dbContext.Customer_ServiceRequest.Add(service);
                dbContext.SaveChanges();

                model.ServiceModel.Id = service.Id;
                model.ServiceModel.PropertyNo = service.Property_No;
                model.ServiceModel.CreatedDate = service.Created_Date;

                string message = string.Format(NAMessages.OnlineServiceRequest, service.Id);
                ApplicationHelper.SendSMS(service.MobileNumber, message);
                if (!string.IsNullOrEmpty(service.Email)) ApplicationHelper.SendEmail(service.Email, "Online Request", message);

                //SaveFilesByRequestId(files, service.Id);

                FtpHandler.UploadFiles(files, service.Id.ToString());
            }
            return model;
        }


        public ServiceRequestViewModel GetServiceRequestDetailOfSamadhanDiwasById(int? id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                ServiceRequestViewModel model = new ServiceRequestViewModel();

                var service = (from csr in dbContext.Customer_ServiceRequest
                               where csr.Id == id && csr.IsActive == true
                               select new ServiceViewModel
                               {
                                   Id = csr.Id,
                                   RequestId = csr.Id,
                                   RegistrationNo = csr.Registration_No,
                                   Applicant = csr.ApplicantName,
                                   ApplicantAddress = csr.ApplicantAddress,
                                   Requestor = csr.RequestorName,
                                   RequestorAddress = csr.RequestorAddress,
                                   PropertyNo = csr.Property_No,
                                   MobileNo = csr.MobileNumber,
                                   Email = csr.Email,
                                   ServiceId = csr.ServiceId,
                                   ServiceName = dbContext.CitizenService_Master.FirstOrDefault(x => x.Deptt_Id == csr.DepartmentId && x.service_id == csr.ServiceId && x.Status == 1).ServiceName,
                                   DepartmentId = csr.DepartmentId,
                                   Department = dbContext.DepartmentMsts.FirstOrDefault(x => x.departmentId == csr.DepartmentId && x.IsActive == true).departmentName,
                                   SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                   SubDepartmentId = string.IsNullOrEmpty(csr.SubDepartment) ? 1 : dbContext.SubDepartmentMsts.FirstOrDefault(x => x.SubdepartmentName == csr.SubDepartment).SubdepartmentId,
                                   Description = csr.Description,
                                   RegistrationType = csr.ServiceType == Constants.SDService ? Constants.PradhikaranDiwas : Constants.Registered,
                                   ServiceStatusId = csr.Request_Status,
                                   RequestStatus = dbContext.StatusMasters.FirstOrDefault(m => m.Id == csr.Request_Status && m.IsActive == true).Status,
                                   Comment = csr.Comment
                               }).FirstOrDefault();
                //if (!string.IsNullOrEmpty(service.SubDepartment))
                //{
                //    service.SubDepartmentId = service.SubDepartment == "P" ? 1 : Convert.ToInt32(service.SubDepartment);
                //}
                //if (service.SubDepartmentId != null && service.SubDepartmentId > 0)
                //{
                //    service.SubDepartment = dbContext.SubDepartmentMsts.FirstOrDefault(s => s.departmentId == service.DepartmentId && s.SubdepartmentId == service.SubDepartmentId && s.IsActive == true).SubdepartmentName;
                //}
                if (!string.IsNullOrEmpty(service.RegistrationNo)) service.RegistrationId = Convert.ToInt32(service.RegistrationNo);
                if (!string.IsNullOrEmpty(service.PropertyNo))
                {
                    var property = service.PropertyNo.Split('/');
                    if (property != null)
                    {
                        service.Sector = property[0];
                        var blck = property[1].Split('-');
                        if (blck != null)
                        {
                            service.Block = blck[0];
                            service.PlotNo = blck[1];
                        }
                    }
                }
                model.ServiceModel = service;

                return model;
            }
        }


        public string GetFileUploadHtmlForService(int? departmentId, int? serviceId)
        {
            using (var context = new NoidaPMSEntities())
            {
                var checklists = (from checklist in context.ServiceCheckList_Master
                                  where checklist.dept_id == departmentId && checklist.service_id == serviceId
                                  select new ServiceCheckListModel
                                  {
                                      Id = checklist.ChkId,
                                      DepartmentId = checklist.dept_id,
                                      Department = context.DepartmentMsts.Where(d => d.departmentId == departmentId).Select(d => d.departmentName).FirstOrDefault(),
                                      ServiceId = checklist.service_id,
                                      ChecklistRefNo = checklist.Checklist_Ref,
                                      ChecklistName = checklist.ChkName,
                                      Status = checklist.Status
                                  }).ToList();
                string divMain = "";
                if (checklists.Count != 0)
                {
                    divMain = "<div class='row  file-header'> "
                                    + "<div class='col-md-3 col-lbl-sn'><label>Serial No</label></div>"
                                    + "<div class='col-md-6 col-lbl'><label>Required File</label></div>"
                                    + "<div class='col-md-3 col-lbl-vl'><label>Select File</label></div>"
                                + "</div>";

                    int docCounter = 1;
                    foreach (var item in checklists)
                    {
                        divMain = divMain + "<div class='row  row-border row-file'> "
                                               + "<div class='col-md-3 col-lbl-sn'><label>" + docCounter + "</label></div>"
                                               + "<div class='col-md-6 col-lbl'><label>" + item.ChecklistName + "</label></div>"
                                               + "<div class='col-md-3 col-lbl-vl'><input type='file' class='single' name='files' /></div>"
                                          + "</div>";
                        docCounter++;
                    }
                    return divMain;
                }
                else return null;
                //return divMain;
            }
        }


        public DataSourceResult GetCustomerServiceRequestList(DataSourceRequest request, ServiceViewModel model)
        {
            //For sorting Kendo DataSourceResult
            if (request.Sorts.Count == 0)
            {
                request.Sorts.Add(new SortDescriptor("Id", System.ComponentModel.ListSortDirection.Descending));
            }
            int userid = userInfo.UserID;
            // added for accountants to see only accounts related service
            //string subdepartment = userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Accountant ? "Account" : "Property";
            List<string> SubDepartmentList = new List<string>();
            List<int?> DepartmentListForBothAccess = new List<int?>() { 3,7,5};
            
            List<int?> ServiceIdList = new List<int?>();// { 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30};
           
            if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Accountant || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.HODAccounts)
            {
                SubDepartmentList.Add("Account");
            }
            else
            {
                if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Assistant || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Property)
                {
                    SubDepartmentList.Add("Property");
                }
                else
                {
                    SubDepartmentList.Add("Account");
                    SubDepartmentList.Add("Property");
                }
            }
            
            IQueryable<ServiceViewModel> list;
            using (var dbContext = new NoidaPMSEntities())
            {
                ServiceIdList = dbContext.CitizenService_Master.Where(u => u.Status == 1).Select(d => d.service_id).Distinct().ToList();
                if (HttpContext.Current.Session["ServiceStatusId"] != null) model.ServiceStatusId = (int)HttpContext.Current.Session["ServiceStatusId"];
                if (HttpContext.Current.Session["ServiceName"] != null)
                {
                    string service = (string)HttpContext.Current.Session["ServiceName"];
                    int? serviceId = dbContext.Database.SqlQuery<int>("select distinct service_id from CitizenService_Master where servicename like '" + service + "%'").FirstOrDefault();
                    model.ServiceId = serviceId;
                }
                //if (HttpContext.Current.Session["ServiceStatusId"] != null) model.ServiceStatusId = (int)HttpContext.Current.Session["ServiceStatusId"];
                //if (HttpContext.Current.Session["ServiceId"] != null) model.ServiceStatusId = (int)HttpContext.Current.Session["ServiceId"];
                if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.OSD)
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && csm.Status == 1 && SubDepartmentList.Contains(csr.SubDepartment)  && csr.RequestThrough != "NIC Nivesh Mitra"
                            && (!DepartmentListForBothAccess.Contains(csr.DepartmentId) ? csr.ServiceId != 12 && csr.ServiceId != 13 : ServiceIdList.Contains(csr.ServiceId))
                                //&& DepartmentListForBothAccess.Contains(csr.DepartmentId)&& csr.ServiceId != 12 && csr.ServiceId != 13 // && csr.IsActive == true
                                && (model.Id == null || csr.Id == model.Id)
                                && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                                && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                                && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                                && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                                && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            select new ServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                                RequestDate = csr.Created_Date,
                                CreatedDate = csr.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = csr.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = csr.ServiceType,
                                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                                Amount = csr.DuesAmount,
                                StatusId = sts.Id,
                                Status = sts.Status,
                                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                                IsForwarded = csr.Request_Status == NAStatusId.Forwarded ? true : false,
                                IsCompleted = csr.Request_Status == NAStatusId.Completed ? true : false,
                                IsInitiated = csr.Request_Status == NAStatusId.Initiated ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                ApproverId = csr.ApproverId,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                Sector = csr.SectorName,
                                Block = csr.BlockName,
                                PlotNo = csr.PlotNo,
                                IsUploaded = csr.IsUploadedLetter
                            });
                }
                else if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.HODAccounts)
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && csm.Status == 1 && SubDepartmentList.Contains(csr.SubDepartment) && (csr.ServiceId == 12 || csr.ServiceId == 13) && csr.RequestThrough != "NIC Nivesh Mitra" // && csr.IsActive == true
                                && (model.Id == null || csr.Id == model.Id)
                                && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                                && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                                && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                                && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                                && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            select new ServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                                RequestDate = csr.Created_Date,
                                CreatedDate = csr.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = csr.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = csr.ServiceType,
                                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                                Amount = csr.DuesAmount,
                                StatusId = sts.Id,
                                Status = sts.Status,
                                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                                IsForwarded = csr.Request_Status == NAStatusId.Forwarded ? true : false,
                                IsCompleted = csr.Request_Status == NAStatusId.Completed ? true : false,
                                IsInitiated = csr.Request_Status == NAStatusId.Initiated ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                ApproverId = csr.ApproverId,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                Sector = csr.SectorName,
                                Block = csr.BlockName,
                                PlotNo = csr.PlotNo,
                                IsUploaded = csr.IsUploadedLetter
                            });
                }
                else if (userInfo.RoleMaster.RoleInDepartment == null || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Admin)
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && csm.Status == 1 && SubDepartmentList.Contains(csr.SubDepartment) && csr.RequestThrough != "NIC Nivesh Mitra" // && csr.IsActive == true
                                && (model.Id == null || csr.Id == model.Id)
                                && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                                && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                                && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                                && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                                && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            select new ServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                                RequestDate = csr.Created_Date,
                                CreatedDate = csr.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = csr.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = csr.ServiceType,
                                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                                Amount = csr.DuesAmount,
                                StatusId = sts.Id,
                                Status = sts.Status,
                                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                                IsForwarded = csr.Request_Status == NAStatusId.Forwarded ? true : false,
                                IsCompleted = csr.Request_Status == NAStatusId.Completed ? true : false,
                                IsInitiated = csr.Request_Status == NAStatusId.Initiated ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                ApproverId = csr.ApproverId,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                Sector = csr.SectorName,
                                Block = csr.BlockName,
                                PlotNo = csr.PlotNo,
                                IsUploaded = csr.IsUploadedLetter
                            });
                }
                else
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && csm.Status == 1 && csr.ApproverId == userid && csr.RequestThrough != "NIC Nivesh Mitra"//&& SubDepartmentList.Contains(csr.SubDepartment)  && csr.IsActive == true
                            && (model.Id == null || csr.Id == model.Id)
                            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                            && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            select new ServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                                RequestDate = csr.Created_Date,
                                CreatedDate = csr.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = csr.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = csr.ServiceType,
                                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                                Amount = csr.DuesAmount,
                                StatusId = sts.Id,
                                Status = sts.Status,
                                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                                IsForwarded = csr.Request_Status == NAStatusId.Forwarded ? true : false,
                                IsCompleted = csr.Request_Status == NAStatusId.Completed ? true : false,
                                IsInitiated = csr.Request_Status == NAStatusId.Initiated ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                ApproverId = csr.ApproverId,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                Sector = csr.SectorName,
                                Block = csr.BlockName,
                                PlotNo = csr.PlotNo,
                                IsUploaded = csr.IsUploadedLetter
                            });
                }
                //if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Assistant || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.AccountAssistant)
                //{
                //    list = (from csr in dbContext.Customer_ServiceRequest
                //            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                //            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                //            where DepartmentList.Contains(csr.DepartmentId) && csm.Status == 1 && csr.ApproverId == userid && csr.IsActive == true && SubDepartmentList.Contains(csr.SubDepartment)
                //            && (model.Id == null || csr.Id == model.Id)
                //            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                //            && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                //            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                //            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                //            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                //            select new ServiceViewModel
                //            {
                //                Id = csr.Id,
                //                RequestId = csr.Id,
                //                RegistrationNo = csr.Registration_No,
                //                DepartmentId = csr.DepartmentId,
                //                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                //                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                //                RequestDate = csr.Created_Date,
                //                CreatedDate = csr.Created_Date,
                //                Timeline = csm.Timeline,
                //                ServiceId = csr.ServiceId,
                //                ServiceName = csm.ServiceName,
                //                ServiceType = csr.ServiceType,
                //                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                //                Amount = csr.DuesAmount,
                //                StatusId = sts.Id,
                //                Status = sts.Status,
                //                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                //                IsForwarded = csr.Request_Status == NAStatusId.Forwarded ? true : false,
                //                IsCompleted = csr.Request_Status == NAStatusId.Completed ? true : false,
                //                IsInitiated = csr.Request_Status == NAStatusId.Initiated ? true : false,
                //                Comment = csr.Comment,
                //                MobileNo = csr.MobileNumber,
                //                Applicant = csr.ApplicantName,
                //                Description = csr.Description,
                //                Requestor = csr.RequestorName,
                //                RequestorAddress = csr.RequestorAddress,
                //                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                //                ApproverId = csr.ApproverId,
                //                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName
                //            });
                //}
                //else
                //{
                //    list = (from csr in dbContext.Customer_ServiceRequest
                //                join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                //                join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                //            where DepartmentList.Contains(csr.DepartmentId) && csm.Status == 1 && csr.IsActive == true && SubDepartmentList.Contains(csr.SubDepartment)
                //                && (model.Id == null || csr.Id == model.Id)
                //                && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                //                && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                //                && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                //                && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                //                && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                //                select new ServiceViewModel
                //                {
                //                    Id = csr.Id,
                //                    RequestId = csr.Id,
                //                    RegistrationNo = csr.Registration_No,
                //                    DepartmentId = csr.DepartmentId,
                //                    Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                //                    PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                //                    RequestDate = csr.Created_Date,
                //                    CreatedDate = csr.Created_Date,
                //                    Timeline = csm.Timeline,
                //                    ServiceId = csr.ServiceId,
                //                    ServiceName = csm.ServiceName,
                //                    ServiceType = csr.ServiceType,
                //                    DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                //                    Amount = csr.DuesAmount,
                //                    StatusId = sts.Id,
                //                    Status = sts.Status,
                //                    IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                //                    IsForwarded = csr.Request_Status == NAStatusId.Forwarded ? true : false,
                //                    IsCompleted = csr.Request_Status == NAStatusId.Completed ? true : false,
                //                    IsInitiated = csr.Request_Status == NAStatusId.Initiated ? true : false,
                //                    Comment = csr.Comment,
                //                    MobileNo = csr.MobileNumber,
                //                    Applicant = csr.ApplicantName,
                //                    Description = csr.Description,
                //                    Requestor = csr.RequestorName,
                //                    RequestorAddress = csr.RequestorAddress,
                //                    SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                //                    ApproverId = csr.ApproverId,
                //                    Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName
                //                });
                //}
                HttpContext.Current.Session["ServiceStatusId"] = null;
                HttpContext.Current.Session["ServiceName"] = null;
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetPradhikaranDiwasRequestList(DataSourceRequest request, ServiceViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from serv in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { x = serv.DepartmentId, y = serv.ServiceId } equals new { x = csm.Deptt_Id, y = csm.service_id }
                            where serv.ServiceType == Constants.SDService && csm.Status == 1
                            && (model.Id == null || serv.Id == model.Id)
                            && (model.ServiceId == null || serv.ServiceId == model.ServiceId)
                            && (model.StatusId == null || serv.Request_Status == model.StatusId)
                            && (model.DepartmentId == null || serv.DepartmentId == model.DepartmentId)
                            && (model.StartDate == null || DbFunctions.TruncateTime(serv.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(serv.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            select new ServiceViewModel
                            {
                                Id = serv.Id,
                                RequestId = serv.Id,
                                RegistrationNo = serv.Registration_No,
                                DepartmentId = serv.DepartmentId,
                                Department = (serv.DepartmentId == null || serv.DepartmentId == 0) ? "" : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == serv.DepartmentId).departmentName,
                                PropertyNo = serv.Property_No != null ? serv.Property_No : "",
                                RequestDate = serv.Created_Date,
                                CreatedDate = serv.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = serv.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = serv.ServiceType,
                                DuesAmount = serv.DuesAmount == null ? 0 : serv.DuesAmount,
                                Amount = serv.DuesAmount,
                                Status = (serv.Request_Status != null || serv.Request_Status != 0) ? dbContext.StatusMasters.FirstOrDefault(x => x.Id == serv.Request_Status).Status : string.Empty,
                                Comment = serv.Comment,
                                Email = serv.Email,
                                MobileNo = serv.MobileNumber,
                                Applicant = serv.ApplicantName,
                                Description = serv.Description,
                                Requestor = serv.RequestorName,
                                RequestorAddress = serv.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(serv.SubDepartment) ? SubDepartment.Property : serv.SubDepartment,
                                UploadedDocument = !(string.IsNullOrEmpty(serv.UploadedDocumentName)) ? serv.UploadedDocumentName : string.Empty,
                                DispatchedDocument = !(string.IsNullOrEmpty(serv.DispatchDocumentName)) ? serv.DispatchDocumentName : string.Empty
                            });
                var data = list.ToDataSourceResult(request);
                return data;
            }
        }


        public int UpdateCustomerServiceRequestStatusOld(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            //int flag = ReturnType.None;
            //using (var dbContext = new NoidaPMSEntities())
            //{
            //    var user = dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == userInfo.UserID);

            //    var service = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == model.Id);
            //    service.Request_Status = model.StatusId;
            //    service.Comment = service.Comment + "\n" + model.Comment + " by \n" + user.FirstName + " " + user.MiddleName + " " + user.LastName + " Date:" + DateTime.Now + ".";
            //    service.Modified_By = userInfo.UserID;
            //    service.Modified_Date = DateTime.Now;
            //    dbContext.SaveChanges();
            //    flag = ReturnType.Updated;
            //}
            //return flag;

            int flag = ReturnType.None;

            using (var dbContext = new NoidaPMSEntities())
            {
                var service = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == model.Id);
                if (service.Request_Status == NAStatusId.Completed)
                {
                    flag = ReturnType.Updated;
                }
                else if (service.Request_Status == NAStatusId.Forwarded || service.Request_Status == NAStatusId.Pending || service.Request_Status == NAStatusId.Objection || service.Request_Status == NAStatusId.Resubmitted)
                {
                    if (model.StatusId == NAStatusId.Forwarded && service.Request_Status == NAStatusId.Forwarded)
                    {
                        service.Request_Status = model.StatusId;
                        //service.Comment = model.Comment + "-" + userInfo.FirstName + " " + userInfo.LastName;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        service.ValidatorId = userInfo.UserID;
                        service.ValidatedDate = DateTime.Now;
                        service.ApproverId = model.ApproverId;
                        dbContext.SaveChanges();
                        flag = ReturnType.Validated;
                        //flag = ReturnType.Forwarded;
                    }
                    else if (model.StatusId == NAStatusId.Pending && service.Request_Status == NAStatusId.Pending)
                    {
                        flag = ReturnType.Exist;
                    }
                    else if (model.StatusId == NAStatusId.Pending && service.Request_Status == NAStatusId.Forwarded)
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        dbContext.SaveChanges();
                        flag = ReturnType.Pending;
                    }
                    else if (model.StatusId == NAStatusId.Objection && service.Request_Status == NAStatusId.Objection)
                    {
                        flag = ReturnType.Exist;
                    }
                    else if (model.StatusId == NAStatusId.Objection && service.Request_Status == NAStatusId.Forwarded)
                    {
                        service.Request_Status = model.StatusId;
                        service.ObjectionStatus = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        dbContext.SaveChanges();

                        //string reqName = dbContext.StatusMasters.Where(m => m.Id == service.Request_Status).FirstOrDefault().Status;
                        string message = string.Empty;
                        message = string.Format(NAMessages.SDServiceReqStatusChange, model.Id, " held on pending with objection");
                        if (!string.IsNullOrEmpty(service.MobileNumber)) { ApplicationHelper.SendSMS(service.MobileNumber, message); }
                        if (!string.IsNullOrEmpty(service.Email)) { ApplicationHelper.SendEmail(service.Email, "Online Request", message); }
                        flag = ReturnType.Pending;
                    }
                    else if (model.StatusId == NAStatusId.Resubmitted && service.Request_Status == NAStatusId.Forwarded)
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        service.ValidatorId = userInfo.UserID;
                        service.ValidatedDate = DateTime.Now;
                        service.ApproverId = model.ApproverId;
                        dbContext.SaveChanges();
                        flag = ReturnType.Validated;
                    }
                    else
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        service.ApproverId = userInfo.UserID;
                        service.ApprovalDate = DateTime.Now;
                        if (model.StatusId == NAStatusId.Cancelled)
                        {
                            service.IsActive = false;
                            dbContext.SaveChanges();
                        }
                        if (model.StatusId == NAStatusId.Approved || model.StatusId == NAStatusId.Completed)
                        {
                            if (string.IsNullOrEmpty(service.Registration_No) && service.ServiceId != NAService.Query)
                            {
                                flag = ReturnType.NotRegistered;
                            }
                            else
                            {
                                dbContext.SaveChanges();
                                if (service.Request_Status == NAStatusId.Completed)
                                {
                                    if (files != null && files.Count() > 0)
                                    {
                                        FtpHandler.UploadFiles(files, model.Id.ToString());
                                        if (!string.IsNullOrEmpty(files.FirstOrDefault().FileName))
                                        {
                                            service.DispatchDocumentName = files.FirstOrDefault().FileName;
                                            dbContext.SaveChanges();
                                        }
                                    }
                                    flag = ReturnType.Completed;
                                }
                                else flag = ReturnType.Cancelled;
                                string reqName = dbContext.StatusMasters.Where(m => m.Id == service.Request_Status).FirstOrDefault().Status;

                                string message = string.Empty;
                                message = string.Format(NAMessages.SDServiceReqStatusChange, model.Id, reqName);
                                if (!string.IsNullOrEmpty(service.MobileNumber)) { ApplicationHelper.SendSMS(service.MobileNumber, message); }
                                if (!string.IsNullOrEmpty(service.Email)) { ApplicationHelper.SendEmail(service.Email, "Online Request", message); }
                            }
                        }

                    }
                }
                else if (service.Request_Status == NAStatusId.Initiated)//for forward 
                {
                    if (model.StatusId == NAStatusId.Forwarded)
                    {
                        service.Request_Status = model.StatusId;
                        //service.Comment = model.Comment + "-" + userInfo.FirstName + " " + userInfo.LastName;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        service.ValidatorId = userInfo.UserID;
                        service.ValidatedDate = DateTime.Now;
                        service.ApproverId = model.ApproverId;
                        dbContext.SaveChanges();
                        flag = ReturnType.Validated;
                    }
                    else flag = ReturnType.Initiated;
                }
                else if (model.StatusId == NAStatusId.Objection && service.ObjectionStatus == NAStatusId.Objection)
                {
                    flag = ReturnType.NotAvailable;
                }

            }
            return flag;
        }


        public DataSourceResult GetGeneratedChallanList(DataSourceRequest request, ChallanViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                model.BankId = userInfo.OptionalId != null ? userInfo.OptionalId : (model.BankId != null ? model.BankId : null);

                //model.IsActive = model.StatusId == 3 ? false : true;
                if (model.StatusId == 3) model.IsActive = false;
                if (model.StatusId == 1) model.IsVerified = true;
                if (model.StatusId == 2) model.IsVerified = false;

                var list = (from challan in dbContext.Challan_Master
                            join bank in dbContext.BankMsts on challan.Bank_Id equals bank.bankId
                            where (model.Id == null || challan.Id == model.Id) && (DepartmentList.Contains(challan.Department_Id))
                            && (model.DepartmentId == null || challan.Department_Id == model.DepartmentId)
                            && (model.StartDate == null || DbFunctions.TruncateTime(challan.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(challan.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            && (model.BankId == null || challan.Bank_Id == model.BankId)
                            && (model.IsVerified == null || challan.Is_Verified == model.IsVerified)
                            && (model.IsActive == null || challan.Is_Active == model.IsActive)
                            select new ChallanViewModel
                            {
                                Id = challan.Id,
                                ChallanId = challan.Challan_Id,
                                RegistrationId = challan.Rid,
                                DepartmentId = challan.Department_Id,
                                Department = challan.Department_Id != null ? dbContext.DepartmentMsts.FirstOrDefault(c => c.departmentId == challan.Department_Id).departmentName : string.Empty,
                                SectorId = challan.Sector_Id,
                                Sector = challan.Sector_Id != null ? dbContext.SectorMsts.FirstOrDefault(s => s.sectorId == challan.Sector_Id).sectorName : string.Empty,
                                BlockId = challan.Block_Id,
                                Block = challan.Block_Id != null ? dbContext.BlockMsts.FirstOrDefault(b => b.blockId == challan.Block_Id).blockName : string.Empty,
                                PlotNo = challan.Plot_No,
                                Applicant = (challan.Allottee == null && challan.Rid != null) ? dbContext.ApplicationDetails.FirstOrDefault(a => a.registrationId == challan.Rid).tFirstName : challan.Allottee,
                                CorrespondAddress = challan.Address,
                                //Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : (aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName),
                                MobileNo = challan.Mobile_No,
                                PropertyNo = (challan.Sector_Id != null ? dbContext.SectorMsts.FirstOrDefault(s => s.sectorId == challan.Sector_Id).sectorName : string.Empty) + "/" + (challan.Block_Id != null ? dbContext.BlockMsts.FirstOrDefault(b => b.blockId == challan.Block_Id).blockName : string.Empty) + "-" + challan.Plot_No,
                                //PropertyNo = property.SectorMst.sectorName + "/" + property.blockId == null ? Constants.NA : property.BlockMst.blockName + "-" + property.propertyNo,
                                GeneratedDate = challan.Generate_Date,
                                CreateDate = challan.Created_Date,
                                ChallanContent = challan.Content,
                                IsActive = challan.Is_Active,
                                IsCancelled = challan.Is_Active == true ? false : true,
                                IsVerified = challan.Is_Verified,
                                ChallanStatus = challan.Is_Active == false ? "Canceled" : (challan.Is_Verified == true ? "Verified" : "Not Verified"),
                                RequestId = challan.ServiceRequestNo,
                                ServiceRequestId = challan.ServiceRequestNo,
                                IsBankUserId = userInfo.OptionalId != null ? true : false,
                                BankId = challan.Bank_Id,
                                BankName = challan.Bank_Id == null ? string.Empty : dbContext.BankMsts.FirstOrDefault(b => b.bankId == challan.Bank_Id).bankName,
                                TotalAmount=challan.TotalAmount
                            });
                return list.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetGeneratedChallanAmountListById(DataSourceRequest request, int? challanId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from challan in dbContext.Challan_Master
                            join challanTrans in dbContext.Challan_Trans on challan.Id equals challanTrans.Challan_Master_Id
                            //join alotment in dbContext.AllotmentMasters on challan.Rid equals alotment.rid
                            //join aplicant in dbContext.ApplicationDetails on challan.Rid equals aplicant.registrationId
                            //join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            join bank in dbContext.BankMsts on challan.Bank_Id equals bank.bankId
                            where challan.Id == challanId
                            select new ChallanViewModel
                            {
                                Id = challan.Id,
                                ChallanId = challan.Challan_Id,
                                RegistrationId = challan.Rid,
                                RequestId = challan.ServiceRequestNo,
                                Applicant = challan.Allottee,
                                MobileNo = challan.Mobile_No,
                                //PropertyNo = property.SectorMst.sectorName + "/" + property.blockId == null ? "NA" : property.BlockMst.blockName + "-" + property.propertyNo,
                                GeneratedDate = challan.Generate_Date,
                                CreateDate = challan.Created_Date,
                                ChallanContent = challan.Content,
                                Amount = challanTrans.Amount,
                                AccountHead = challanTrans.Head_Id == null ? string.Empty : dbContext.RECIEPT_HEAD.FirstOrDefault(x => x.RECIEPT_CODE == challanTrans.Head_Id && x.STATUS == 1).RECIEPT_HEAD_NAME,
                                AccountSubHead = challanTrans.Subhead_Id == null ? string.Empty : dbContext.RECEIPT_SUB_HEAD.FirstOrDefault(x => x.RECEIPT_CODE == challanTrans.Head_Id && x.RECEIPT_SUBHEAD_ID == challanTrans.Subhead_Id && x.STATUS == 1).RECEIPT_SUB_HEAD1
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetGeneratedLetterList(DataSourceRequest request, LetterViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var list = (from letter in dbContext.Letter_History
                //            join alotment in dbContext.AllotmentMasters on letter.Rid equals alotment.rid
                //            join aplicant in dbContext.ApplicationDetails on letter.Rid equals aplicant.registrationId
                //            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                //            join template in dbContext.TemplateMasters on new { x = alotment.departmentId, y = letter.Template_Id } equals new { x = template.departmentId, y = template.templateId }
                //            join user in dbContext.UmUserMasters on letter.Created_By equals user.UserRefId
                //            where (model.Id == null || letter.Id == model.Id)
                //               && (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                //               && (model.TemplateId == null || template.templateId == model.TemplateId)
                //               && (model.StartDate == null || DbFunctions.TruncateTime(letter.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                //               && (model.EndDate == null || DbFunctions.TruncateTime(letter.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                //            select new LetterViewModel
                //            {
                //                Id = letter.Id,
                //                RegistrationId = letter.Rid,
                //                UserId = letter.User_Id,
                //                DepartmentId = letter.Department_Id,
                //                Department = alotment.DepartmentMst.departmentName,
                //                Applicant = aplicant.tGender == Constants.Company ? aplicant.T_Company_Name : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + "-") + aplicant.tLastName,
                //                ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                //                ApplicantType = aplicant.tGender,
                //                CreatedDate = letter.Created_Date,
                //                GeneratedDate = letter.Generate_Date,
                //                SectorId = property.sectorId,
                //                Sector = property.SectorMst.sectorName,
                //                BlockId = property.blockId,
                //                Block = property.BlockMst.blockName,
                //                PlotNo = property.propertyNo,
                //                TemplateId = letter.Template_Id,
                //                BarcodeValue = letter.Barcode_Val,
                //                LetterContent = letter.Template_Html,
                //                Template = template.templateName,
                //                CreatedBy = user.FirstName + " " + (string.IsNullOrEmpty(user.MiddleName) ? string.Empty : user.MiddleName + " ") + user.LastName
                //            });
                //return list.ToDataSourceResult(request);

                var letterlist = (from letter in dbContext.Letter_History
                                  group letter by new { letter.Rid } into lettergrp
                                  select lettergrp.FirstOrDefault().Rid);

                var list = (from alotment in dbContext.AllotmentMasters
                            join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where letterlist.Contains(alotment.rid)
                            && (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                            select new LetterViewModel
                            {
                                Id = alotment.rid,
                                RegistrationId = alotment.rid,
                                //UserId = letter.User_Id,
                                DepartmentId = alotment.departmentId,
                                Department = alotment.DepartmentMst.departmentName,
                                Applicant = aplicant.tGender == Constants.Company ? aplicant.T_Company_Name : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + "-") + aplicant.tLastName,
                                ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                ApplicantType = aplicant.tGender,
                                AllotmentDate = alotment.allotmentDate,
                                //GeneratedDate = letter.Generate_Date,
                                SectorId = property.sectorId,
                                Sector = property.SectorMst.sectorName,
                                BlockId = property.blockId,
                                Block = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                MobileNo = aplicant.tMobileNumber,
                                //TemplateId = letter.Template_Id,
                                //BarcodeValue = letter.Barcode_Val,
                                //LetterContent = letter.Template_Html,
                                //Template = template.templateName,
                                //CreatedBy = user.FirstName + " " + (string.IsNullOrEmpty(user.MiddleName) ? string.Empty : user.MiddleName + " ") + user.LastName
                            });
                // var glist = list.GroupBy(x => x.RegistrationId);
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetGeneratedLetterListById(DataSourceRequest request, int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from letter in dbContext.Letter_History
                            join alotment in dbContext.AllotmentMasters on letter.Rid equals alotment.rid
                            join aplicant in dbContext.ApplicationDetails on letter.Rid equals aplicant.registrationId
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            join template in dbContext.TemplateMasters on new { x = alotment.departmentId, y = letter.Template_Id } equals new { x = template.departmentId, y = template.templateId }
                            join user in dbContext.UmUserMasters on letter.Created_By equals user.UserRefId
                            where letter.Rid == rid
                            select new LetterViewModel
                            {
                                Id = letter.Id,
                                RegistrationId = letter.Rid,
                                UserId = letter.User_Id,
                                DepartmentId = letter.Department_Id,
                                Department = letter.DepartmentMst.departmentName,
                                Applicant = aplicant.tGender == Constants.Company ? aplicant.T_Company_Name : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + "-") + aplicant.tLastName,
                                ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                ApplicantType = aplicant.tGender,
                                CreatedDate = letter.Created_Date,
                                GeneratedDate = letter.Generate_Date,
                                SectorId = property.sectorId,
                                Sector = property.SectorMst.sectorName,
                                BlockId = property.blockId,
                                Block = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                TemplateId = letter.Template_Id,
                                BarcodeValue = letter.Barcode_Val,
                                LetterContent = letter.Template_Html,
                                Template = template.templateName,
                                CreatedBy = user.FirstName + " " + (string.IsNullOrEmpty(user.MiddleName) ? string.Empty : user.MiddleName + " ") + user.LastName
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetNotingFileDetailList(DataSourceRequest request, NotingViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from master in dbContext.Noting_File_Master
                            join dep in dbContext.DepartmentMsts on master.Department_Id equals dep.departmentId
                            where master.Is_Active == true
                                  && (model.RegistrationId == null || master.Rid == model.RegistrationId)
                                  && (model.DepartmentId == null || master.Department_Id == model.DepartmentId)
                                  && (model.FileName == null || master.File_Number == model.FileName)
                                  && DepartmentList.Contains(master.Department_Id)
                            select new NotingViewModel
                            {
                                Id = master.Id,
                                NotingFileId = master.Id,
                                DepartmentId = dep.departmentId,
                                Department = dep.departmentName,
                                RegistrationId = master.Rid,
                                FileName = master.File_Number,
                                NotingFileNo = master.File_Number,
                                CreatedBy = (from uname in dbContext.UmUserMasters where uname.UserRefId == master.Created_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                Receiver = (from uname in dbContext.UmUserMasters where uname.UserRefId == master.Receive_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                CreatedDate = master.Created_Date,
                                ReceivedDate = master.Receive_Date
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetNotingFileDetailListById(DataSourceRequest request, NotingViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from master in dbContext.Noting_File_Master
                            join trans in dbContext.Noting_File_Trans on master.Id equals trans.Noting_File_Id
                            join dep in dbContext.DepartmentMsts on master.Department_Id equals dep.departmentId
                            where master.Is_Active == true && trans.Noting_File_Id == model.NotingFileId
                            select new NotingViewModel
                            {
                                Id = master.Id,
                                NotingFileId = master.Id,
                                DepartmentId = dep.departmentId,
                                Department = dep.departmentName,
                                RegistrationId = master.Rid,
                                FileName = master.File_Number,
                                NotingFileNo = master.File_Number,
                                NotingDetail = trans.Noting_Details,
                                CreatedBy = (from uname in dbContext.UmUserMasters where uname.UserRefId == master.Created_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                Receiver = (from uname in dbContext.UmUserMasters where uname.UserRefId == master.Receive_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                CreatedDate = master.Created_Date,
                                ReceivedDate = master.Receive_Date
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public int SaveNotingFileContent(NotingViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var ExNoting = dbContext.Noting_File_Master.FirstOrDefault(cond => cond.Rid == model.RegistrationId && cond.Is_Active == true);
                if (ExNoting == null)
                {
                    Noting_File_Master noting = new Noting_File_Master();
                    noting.File_Number = model.NotingFileNo;
                    noting.Rid = model.RegistrationId;
                    noting.Department_Id = model.DepartmentId;
                    noting.Is_Active = true;
                    noting.Created_By = userInfo.UserID;
                    noting.Created_Date = DateTime.Now;
                    noting.Receive_By = userInfo.UserID;
                    noting.Receive_Date = DateTime.Now;
                    dbContext.Noting_File_Master.Add(noting);
                    dbContext.SaveChanges();

                    model.Id = noting.Id;

                    Noting_File_Trans fileTrans = new Noting_File_Trans();
                    fileTrans.Noting_File_Id = model.Id;
                    fileTrans.Noting_Details = model.NotingDetail;
                    fileTrans.Note_By = userInfo.UserID;
                    fileTrans.Noting_Date = DateTime.Now;
                    fileTrans.Created_By = userInfo.UserID;
                    fileTrans.Created_Date = DateTime.Now;
                    fileTrans.Is_Active = true;
                    dbContext.Noting_File_Trans.Add(fileTrans);
                    dbContext.SaveChanges();

                    flag = ReturnType.Saved;
                }
                else
                {
                    var ExFile = dbContext.Noting_File_Trans.FirstOrDefault(x => x.Noting_File_Id == ExNoting.Id);
                    ExFile.Noting_Details = ExFile + "<br/>" + model.NotingDetail;
                    ExFile.Note_By = userInfo.UserID;
                    ExFile.Noting_Date = DateTime.Now;
                    ExFile.Modified_By = userInfo.UserID;
                    ExFile.Modified_Date = DateTime.Now;
                    ExFile.Is_Active = true;
                    dbContext.SaveChanges();

                    flag = ReturnType.Updated;
                }
                return flag;
            }
        }


        public NotingViewModel GetNotingFileDetailById(NotingViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from master in dbContext.Noting_File_Master
                            join trans in dbContext.Noting_File_Trans on master.Id equals trans.Noting_File_Id
                            join dept in dbContext.DepartmentMsts on master.Department_Id equals dept.departmentId
                            where master.Rid == model.RegistrationId && master.Is_Active == true
                            select new NotingViewModel
                            {
                                Id = master.Id,
                                NotingFileId = master.Id,
                                DepartmentId = dept.departmentId,
                                Department = dept.departmentName,
                                RegistrationId = master.Rid,
                                FileName = master.File_Number,
                                NotingFileNo = master.File_Number,
                                NotingDetail = trans.Noting_Details,
                                CreatedBy = (from uname in dbContext.UmUserMasters where uname.UserRefId == master.Created_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                Receiver = (from uname in dbContext.UmUserMasters where uname.UserRefId == master.Receive_By && uname.IsActive == true select uname.FirstName + " " + uname.MiddleName + " " + uname.LastName).FirstOrDefault(),
                                CreatedDate = master.Created_Date,
                                ReceivedDate = master.Receive_Date
                            }).FirstOrDefault();
                return data;
            }
        }


        public DataSourceResult GetServiceRequestListForJanSuvidhaKendra(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var today = DateTime.Now;
                var list = (from serv in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = serv.DepartmentId, x = serv.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            //join st in dbContext.StatusMasters on serv.Request_Status equals st.Id
                            where DepartmentList.Contains(serv.DepartmentId) && csm.Status == 1
                            && (DbFunctions.TruncateTime(serv.Created_Date) == DbFunctions.TruncateTime(today)) && serv.RequestThrough == Constants.JSK
                            select new ServiceViewModel
                            {
                                Id = serv.Id,
                                RequestId = serv.Id,
                                RegistrationNo = serv.Registration_No,
                                DepartmentId = serv.DepartmentId,
                                Department = (serv.DepartmentId == null || serv.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == serv.DepartmentId).departmentName,
                                PropertyNo = serv.Property_No != null ? serv.Property_No : "",
                                RequestDate = serv.Created_Date,
                                CreatedDate = serv.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = serv.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = serv.ServiceType,
                                DuesAmount = serv.DuesAmount == null ? 0 : serv.DuesAmount,
                                Amount = serv.DuesAmount,
                                //Status = st.Status,
                                Comment = serv.Comment,
                                MobileNo = serv.MobileNumber,
                                Applicant = serv.ApplicantName,
                                Description = serv.Description,
                                Requestor = serv.RequestorName,
                                RequestorAddress = serv.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(serv.SubDepartment) ? SubDepartment.Property : serv.SubDepartment
                            });

                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetPropertyListForJanSuvidhaKendra(DataSourceRequest request, PropertyViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from alotment in dbContext.AllotmentMasters
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                            where alotment.isActive == 1
                            && (model.DepartmentId == null || alotment.departmentId == model.DepartmentId)
                            && (model.SectorId == null || property.sectorId == model.SectorId)
                            && (model.BlockId == null || property.blockId == model.BlockId)
                            && (model.PlotNo == null || property.propertyNo == model.PlotNo)
                            && (model.RegistrationId == null || alotment.rid == model.RegistrationId)
                            select new PropertyViewModel
                            {
                                Id = alotment.rid,
                                RegistrationId = alotment.rid,
                                PropertyId = alotment.propertyId,
                                SchemeId = alotment.schemeId,
                                SchemeName = alotment.SchemeMst.schemeName,
                                SectorId = property.sectorId,
                                SectorName = property.SectorMst.sectorName,
                                BlockId = property.blockId,
                                BlockName = property.blockId == null ? "NA" : property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                PropertyNo = property.SectorMst.sectorName + "/" + (property.blockId == null ? string.Empty : property.BlockMst.blockName + " - ") + property.propertyNo,
                                ApplicantType = aplicant.tGender,
                                Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.lastName,
                                ApplicantAddress = aplicant.tCorrespondanceAdd,
                                ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                AllotmentDate = alotment.allotmentDate,
                                Department = alotment.DepartmentMst.departmentName,
                                Status = alotment.isStatus,
                                MobileNo = aplicant.tMobileNumber,
                                Email = aplicant.tEmail
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetBankAccountDetailList(DataSourceRequest request, BankAccountViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.FilterType == "Bank")
                {
                    var data = (from bank in dbContext.BankMsts
                                where (model.BankId == null || bank.bankId == model.BankId)
                                select new BankAccountViewModel
                                {
                                    Id = bank.bankId,
                                    BankId = bank.bankId,
                                    BankName = bank.bankName,
                                    IsBankActive = bank.IsActive,
                                    BankStatus = bank.IsActive == true ? "Open" : "Closed"
                                });
                    return data.ToDataSourceResult(request);
                }
                else if (model.FilterType == "Branch")
                {
                    var data = (from bank in dbContext.BankMsts
                                join branch in dbContext.BranchMsts on bank.bankId equals branch.bankId
                                where (model.BankId == null || bank.bankId == model.BankId)
                                select new BankAccountViewModel
                                {
                                    Id = bank.bankId,
                                    BankId = bank.bankId,
                                    BranchId = branch.branchId,
                                    BankName = bank.bankName,
                                    BranchAddress = branch.branchName,
                                    IFSCCode = branch.IFSCcode,
                                    AccountNo = branch.accountNumber,
                                    IsBankActive = bank.IsActive,
                                    IsBranchActive = branch.IsActive,
                                    BankStatus = bank.IsActive == true ? "Open" : "Closed",
                                    BranchStatus = branch.IsActive == true ? "Open" : "Closed"
                                });
                    return data.ToDataSourceResult(request);
                }
                else
                {
                    return null;
                }
            }
        }

        public DataSourceResult GetAuthorityServicesDetailList(DataSourceRequest request, ServiceViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from service in dbContext.CitizenService_Master
                            join department in dbContext.DepartmentMsts on service.Deptt_Id equals department.departmentId
                            where (model.DepartmentId == null || service.Deptt_Id == model.DepartmentId)
                            select new ServiceViewModel
                            {
                                Id = service.Id,
                                ServiceId = service.service_id,
                                DepartmentId = service.Deptt_Id,
                                Department = department.departmentName,
                                ServiceName = service.ServiceName,
                                StatusId = service.Status,
                                Status = service.Status == 1 ? "Active" : "InActive",
                                IsActive = service.Status == 1 ? true : false
                            });
                return data.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetPaymentReceiptHeadAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from receipt in dbContext.RECIEPT_HEAD
                            where (model.ReceiptId == null || receipt.RECIEPT_CODE == model.ReceiptId)
                            select new PaymentViewModel
                            {
                                Id = receipt.RECIEPT_CODE,
                                ReceiptCode = receipt.RECIEPT_CODE,
                                ReceiptId = receipt.RECIEPT_CODE,
                                ReceiptHeadName = receipt.RECIEPT_HEAD_NAME,
                                ReceiptHeadId = receipt.HEAD_CODE,
                                StatusId = receipt.STATUS,
                                HeadStatus = receipt.STATUS == 1 ? "Active" : "InActive",
                                IsActive = receipt.STATUS == 1 ? true : false
                            });
                return data.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetPaymentReceiptSubHeadAsDataSource(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from receipt in dbContext.RECEIPT_SUB_HEAD
                            where receipt.RECEIPT_CODE == model.ReceiptId
                            select new PaymentViewModel
                            {
                                Id = receipt.RECEIPT_SUBHEAD_ID,
                                ReceiptSubHeadId = receipt.RECEIPT_SUBHEAD_ID,
                                ReceiptCode = receipt.RECEIPT_CODE,
                                //ReceiptId = receipt.RECEIPT_CODE,
                                ReceiptSubHeadName = receipt.RECEIPT_SUB_HEAD1,
                                StatusId = receipt.STATUS,
                                SubHeadStatus = receipt.STATUS == 1 ? "Active" : "InActive",
                                IsActive = receipt.STATUS == 1 ? true : false,
                                RegistrationNo = null
                            });
                return data.ToDataSourceResult(request);
            }
        }


        public int ActivateBanckAccountStatus(BankAccountViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var flag = ReturnType.None;
                if (model.FilterType == "Bank")
                {
                    var bank = dbContext.BankMsts.FirstOrDefault(b => b.bankId == model.BankId);
                    bank.IsActive = bank.IsActive == true ? false : true;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                else if (model.FilterType == "Branch")
                {
                    var branch = dbContext.BranchMsts.FirstOrDefault(f => f.bankId == model.BankId && f.branchId == model.BranchId);
                    branch.IsActive = branch.IsActive == true ? false : true;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                return flag;
            }
        }

        public int ActivatePaymentReceiptStatus(PaymentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "Head")
                {
                    var head = dbContext.RECIEPT_HEAD.FirstOrDefault(r => r.RECIEPT_CODE == model.ReceiptId);
                    head.STATUS = head.STATUS == 0 ? 1 : 0;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                if (model.ActionType == "SubHead")
                {
                    var head = dbContext.RECEIPT_SUB_HEAD.FirstOrDefault(r => r.RECEIPT_SUBHEAD_ID == model.ReceiptSubHeadId);
                    head.STATUS = head.STATUS == 0 ? 1 : 0;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                if (model.ActionType == "RemoveHead")
                {
                    var head = dbContext.RECIEPT_HEAD.FirstOrDefault(r => r.RECIEPT_CODE == model.ReceiptHeadId);
                    dbContext.RECIEPT_HEAD.Remove(head);
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                if (model.ActionType == "RemoveSubHead")
                {
                    var subhead = dbContext.RECEIPT_SUB_HEAD.FirstOrDefault(r => r.RECEIPT_SUBHEAD_ID == model.ReceiptSubHeadId);
                    dbContext.RECEIPT_SUB_HEAD.Remove(subhead);
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                return flag;
            }
        }

        public int ActivateAuthorityServiceStatus(ServiceViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                //var service = dbContext.CitizenService_Master.FirstOrDefault(c => c.Deptt_Id == model.DepartmentId && c.service_id == model.ServiceId);
                var service = dbContext.CitizenService_Master.FirstOrDefault(c => c.Id == model.Id);
                service.Status = service.Status == 0 ? 1 : 0;
                dbContext.SaveChanges();
                flag = ReturnType.Updated;
                return flag;
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
                    service.ServiceName = model.ServiceName;
                    service.service_id = model.ServiceId;
                    service.Status = Constants.Active;
                    dbContext.CitizenService_Master.Add(service);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                else if (model.ActionType == "UpdateService")
                {
                    var service = dbContext.CitizenService_Master.FirstOrDefault(r => r.Id == model.Id);
                    service.ServiceName = model.ServiceName;
                    service.service_id = (model.ServiceId == null) ? service.service_id : model.ServiceId;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                else if (model.ActionType == "DeleteService")
                {
                    var service = dbContext.CitizenService_Master.FirstOrDefault(r => r.Id == model.Id);
                    dbContext.CitizenService_Master.Remove(service);
                    dbContext.SaveChanges();
                    flag = ReturnType.Removed;
                }
            }
            return flag;
        }


        public int SavePaymentReceiptHeadDetail(PaymentViewModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.ActionType == "AddReceiptHead")
                {
                    var receipt = new RECIEPT_HEAD();
                    receipt.RECIEPT_HEAD_NAME = model.ReceiptHeadName.ToUpper();
                    receipt.STATUS = Constants.Active;
                    receipt.HEAD_CODE = 4;
                    receipt.USERID = userInfo.UserID.ToString();
                    receipt.ENTRY_DATE = DateTime.Now;
                    dbContext.RECIEPT_HEAD.Add(receipt);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                else if (model.ActionType == "UpdateReceiptHead")
                {
                    var head = dbContext.RECIEPT_HEAD.FirstOrDefault(r => r.RECIEPT_CODE == model.ReceiptHeadId);
                    head.RECIEPT_HEAD_NAME = model.ReceiptHeadName.ToUpper();
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                else if (model.ActionType == "AddReceiptSubHead")
                {
                    var subhead = new RECEIPT_SUB_HEAD();
                    subhead.RECEIPT_CODE = model.ReceiptHeadId;
                    subhead.RECEIPT_SUB_HEAD1 = model.ReceiptSubHeadName.ToUpper();
                    subhead.STATUS = Constants.Active;
                    subhead.USERID = userInfo.UserID.ToString();
                    subhead.ENTRY_DATE = DateTime.Now;
                    dbContext.RECEIPT_SUB_HEAD.Add(subhead);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                }
                else if (model.ActionType == "UpdateReceiptSubHead")
                {
                    var subhead = dbContext.RECEIPT_SUB_HEAD.FirstOrDefault(r => r.RECEIPT_SUBHEAD_ID == model.ReceiptSubHeadId);
                    subhead.RECEIPT_SUB_HEAD1 = model.ReceiptSubHeadName.ToUpper();
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
                return flag;
            }
        }



        public DataSourceResult GetKYASubmittedFormList(DataSourceRequest request, KYAViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (HttpContext.Current.Session["KYADepartmentId"] != null) model.DepartmentId = (int)HttpContext.Current.Session["KYADepartmentId"];
                if (HttpContext.Current.Session["KYAStatus"] != null)
                {
                    string service = (string)HttpContext.Current.Session["KYAStatus"];
                    int? statusId = dbContext.Database.SqlQuery<int>("select distinct Id from StatusMaster where status like '%" + service + "%'").FirstOrDefault();
                    model.ActionId = statusId;
                }

                var data = (from kya in dbContext.KYADetails
                            where (model.Id == null || kya.Id == model.Id)
                            && (model.DepartmentId == null || kya.DepartmentId == model.DepartmentId)
                            && (model.ActionId == null || kya.StatusId == model.ActionId)
                            && (model.StartDate == null || DbFunctions.TruncateTime(kya.CreatedDate) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(kya.CreatedDate) <= DbFunctions.TruncateTime(model.EndDate))
                            && DepartmentList.Contains(kya.DepartmentId)
                            //&& kya.IsActive==true
                            select new KYAViewModel
                            {
                                Id = kya.Id,
                                KYAuid = kya.KYAuid,
                                RegistrationId = kya.RId,
                                DepartmentId = kya.DepartmentId,
                                SectorId = kya.SectorId,
                                BlockId = kya.BlockId,
                                PlotNo = kya.PlotNo,
                                Department = kya.DepartmentId != null ? dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == kya.DepartmentId).departmentName : string.Empty,
                                Sector = kya.SectorId != null ? dbContext.SectorMsts.FirstOrDefault(s => s.sectorId == kya.SectorId).sectorName : Constants.NA,
                                Block = kya.BlockId != null ? dbContext.BlockMsts.FirstOrDefault(b => b.blockId == kya.BlockId).blockName : Constants.NA,
                                AllotteeName = kya.AllotteeName,
                                AllotteeType = kya.AllotteeType,
                                CorrespondAddress = kya.CorrespondAddress,
                                AuthorizedSignatory = kya.AllotteeType == Constants.Company ? kya.AuthorizedSignatory : kya.FatherOrHusbandName,
                                CountryCode = kya.CountryCode,
                                MobileNo = kya.MobileNo,
                                SignatoryMobileNo = kya.SignatoryMobileNo,
                                Email = kya.Email,
                                SignatoryEmail = kya.SignatoryEmail,
                                PhoneNo = kya.PhoneNo,
                                GSTNo = kya.GSTNo,
                                PAN = kya.PAN,
                                AadharNo = kya.AadharNo,
                                ROC = kya.ROC,
                                AddressLine1 = kya.AddressLine1,
                                AddressLine2 = kya.AddressLine2,
                                AreaLocality = kya.AreaLocality,
                                City = kya.City,
                                State = kya.State,
                                PinCode = kya.PinCode,
                                KYAReferenceCode = kya.KYAReferenceCode,
                                AllotteeIdFileType = kya.IdFileType,
                                PlotOwnershipFileType = kya.PlotOwnershipFileType,
                                OtherFileType = kya.OtherFileType,
                                IsActive = kya.IsActive,
                                StatusId = kya.StatusId,
                                Status = kya.StatusId != null ? dbContext.StatusMasters.FirstOrDefault(s => s.Id == kya.StatusId).Status : string.Empty,
                                SubmitDate = kya.SubmitDate,
                                ApprovalDate = kya.ApprovalDate,
                                CreatedDate = kya.CreatedDate,
                                ValidatorId = kya.ValidatorId,
                                Validator = kya.ValidatorId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ValidatorId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ValidatorId).LastName) : string.Empty,
                                ApproverId = kya.ApproverId,
                                Approver = kya.ApproverId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ApproverId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ApproverId).LastName) : string.Empty,
                                Remarks = kya.Remarks,
                                OptionalAction = kya.OptionalAction,
                                ActionId = kya.ActionId,
                                IsInitiated = kya.StatusId == NAStatusId.Initiated ? true : false,
                                IsForwarded = kya.StatusId == NAStatusId.Forwarded ? true : false,
                                IsApproved = kya.StatusId == NAStatusId.Approved ? true : false
                            });
                return data.ToDataSourceResult(request);
            }
            //throw new NotImplementedException();
        }


        public int ValidateKYAForm(KYAViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var exForm = dbContext.KYADetails.Where(r => r.Id == model.Id).FirstOrDefault();
                if (exForm.StatusId == NAStatusId.Approved)
                {
                    flag = ReturnType.Updated;
                }
                else if (exForm.StatusId == NAStatusId.Forwarded)
                {
                    if (model.StatusId == NAStatusId.Forwarded)
                    {
                        flag = ReturnType.Forwarded;
                    }
                    else
                    {
                        exForm.KYAuid = exForm.RId.ToString() + "-" + model.Id;
                        exForm.StatusId = model.StatusId;
                        exForm.OptionalAction = model.OptionalAction;
                        exForm.Remarks = exForm.Remarks + " \n" + model.Remarks + "-" + userInfo.FirstName + " " + userInfo.LastName;
                        //exForm.ValidatorId = userInfo.UserID; // model.StatusId == NAStatusId.Forwarded ? userInfo.UserID.AsNullable() : null;
                        exForm.ApproverId = userInfo.UserID;
                        exForm.ApprovalDate = DateTime.Now;
                        if (model.StatusId == NAStatusId.Rejected)
                        {
                            exForm.IsActive = false;
                        }
                        dbContext.SaveChanges();
                        if (exForm.StatusId == NAStatusId.Approved) flag = ReturnType.Approved;
                        else flag = ReturnType.Rejected;
                        string message = string.Empty;
                        if (exForm.StatusId == NAStatusId.Approved)
                        {
                            message = string.Format(NAMessages.KYAApproved, exForm.KYAReferenceCode, exForm.KYAuid);
                        }
                        if (exForm.StatusId == NAStatusId.Rejected)
                        {
                            message = string.Format(NAMessages.KYARejected, exForm.KYAReferenceCode);
                        }
                        if (exForm.AllotteeType == Constants.Company)
                        {
                            //if (!string.IsNullOrEmpty(exForm.SignatoryEmail)) { ApplicationHelper.SendEmail(exForm.SignatoryEmail, "KYA Status", message); }
                            if (!string.IsNullOrEmpty(exForm.SignatoryMobileNo)) { ApplicationHelper.SendSMS(exForm.SignatoryMobileNo, message); }
                        }
                        else
                        {
                            //if (!string.IsNullOrEmpty(exForm.Email)) { ApplicationHelper.SendEmail(exForm.Email, "KYA Status", message); }
                            if (!string.IsNullOrEmpty(exForm.MobileNo)) { ApplicationHelper.SendSMS(exForm.MobileNo, message); }
                        }

                        if (exForm.StatusId == NAStatusId.Approved)
                        {
                            var customer = dbContext.CustomerMsts.FirstOrDefault(c => c.UserName == exForm.RId.ToString());
                            var applicant = dbContext.ApplicationDetails.FirstOrDefault(c => c.registrationId == exForm.RId);
                            if (applicant != null)
                            {
                                if (applicant.tGender == Constants.Company)
                                {
                                    applicant.tMobileNumber = exForm.SignatoryMobileNo;
                                    applicant.tEmail = exForm.SignatoryEmail;
                                }
                                else
                                {
                                    applicant.tMobileNumber = exForm.MobileNo;
                                    applicant.tEmail = exForm.Email;
                                }
                                dbContext.SaveChanges();
                            }
                            if (customer == null)
                            {
                                SaveCustomerDetail(exForm);
                                using (var pisContext = new CustomerContext())
                                {
                                    var exuser = pisContext.Users.FirstOrDefault(u => u.UserName == exForm.RId.ToString());
                                    if (exuser == null)
                                    {
                                        User user = new User();
                                        user.UserId = Guid.NewGuid();
                                        user.PropertyId = exForm.RId.ToString();
                                        user.FirstName = dbContext.ApplicationDetails.FirstOrDefault(a => a.registrationId == exForm.RId).tFirstName;
                                        user.LastName = dbContext.ApplicationDetails.FirstOrDefault(a => a.registrationId == exForm.RId).tLastName;
                                        user.UserName = exForm.RId.ToString();
                                        var newPassword = ApplicationHelper.CreatePassword();
                                        user.Pasword = newPassword.ToMD5HashForPassword();
                                        user.DeptId = exForm.DepartmentId;
                                        user.RoleId = 2;// NAConstant.CustomerRoleId;
                                        user.Sector = exForm.SectorId != null ? dbContext.SectorMsts.FirstOrDefault(s => s.sectorId == exForm.SectorId).sectorName : null;
                                        user.Block = exForm.BlockId != null ? dbContext.BlockMsts.FirstOrDefault(b => b.blockId == exForm.BlockId).blockName : null;
                                        user.Property = exForm.PlotNo;
                                        if (exForm.AllotteeType == Constants.Company)
                                        {
                                            user.UserEmail = exForm.SignatoryEmail;
                                            user.MobileNo = exForm.SignatoryMobileNo;
                                        }
                                        else
                                        {
                                            user.UserEmail = exForm.Email;
                                            user.MobileNo = exForm.MobileNo;
                                        }
                                        //user.Question = model.SecurityQuestion;
                                        //user.Answer = model.SecurityAnswer;
                                        //user.CustomerIdFileName = fileI != null ? model.CustomerIdName : string.Empty; // model.CustomerIdName;
                                        user.CustomerIdFileType = exForm.IdFileType;
                                        //user.CustomerLetterFileName = fileII != null ? model.NALetterName : string.Empty; // model.NALetterName;
                                        user.CustomerLetterType = exForm.PlotOwnershipFileType;
                                        user.CreatedOn = DateTime.Now;
                                        //user.CreatedBy = user.UserName;
                                        user.Status = true;
                                        user.IsLockedOut = false;
                                        user.Remarks = "";
                                        user.IsFirstTimeActivated = false;
                                        pisContext.Users.Add(user);
                                        pisContext.SaveChanges();

                                        flag = ReturnType.Saved;
                                    }
                                }
                            }
                            else
                            {
                                //if (customer.IsActive == false)
                                //{
                                var newPassword = ApplicationHelper.CreatePassword();
                                customer.Password = newPassword.ToMD5HashForPassword();
                                if (exForm.AllotteeType == Constants.Company)
                                {
                                    customer.MobileNo = exForm.SignatoryMobileNo;
                                    customer.Email = exForm.SignatoryEmail;
                                }
                                else
                                {
                                    customer.MobileNo = exForm.MobileNo;
                                    customer.Email = exForm.Email;
                                }
                                customer.FirstName = dbContext.ApplicationDetails.FirstOrDefault(a => a.registrationId == exForm.RId).tFirstName + " " + dbContext.ApplicationDetails.FirstOrDefault(a => a.registrationId == exForm.RId).tMiddleName;
                                customer.LastName = dbContext.ApplicationDetails.FirstOrDefault(a => a.registrationId == exForm.RId).tLastName;
                                customer.IsActive = true;
                                customer.StatusId = 1;
                                if (customer.IsLocked == true)
                                {
                                    customer.IsFirstTimeActivated = false;
                                }
                                if (customer.IsFirstTimeActivated == false || customer.IsFirstTimeActivated == null)
                                {
                                    customer.IsFirstTimeActivated = true;
                                }
                                dbContext.SaveChanges();
                                flag = ReturnType.Saved;

                                var msg = string.Empty; //"Dear User, You are successfully registered with mynoida.in. Your user name is " + exForm.RId + " and password is " + newPassword + " Regards, http://mynoida.in";
                                if (customer.MobileNo != null)
                                {
                                    msg = string.Format(NAMessages.PIS_Registration_Activation, customer.UserName, newPassword);
                                    if (!string.IsNullOrEmpty(customer.MobileNo)) { ApplicationHelper.SendSMS(customer.MobileNo, msg); }
                                }
                                //}
                            }
                            flag = ReturnType.Approved;
                        }
                        else
                        {
                            flag = ReturnType.Rejected;
                        }
                    }
                }
                else if (exForm.StatusId == NAStatusId.Initiated)//for forward 
                {
                    if (model.StatusId == NAStatusId.Forwarded)
                    {
                        exForm.StatusId = model.StatusId;
                        exForm.OptionalAction = model.OptionalAction;
                        exForm.Remarks = model.Remarks + "-" + userInfo.FirstName + " " + userInfo.LastName;
                        exForm.ValidatorId = userInfo.UserID; // model.StatusId == NAStatusId.Forwarded ? userInfo.UserID.AsNullable() : null;
                        exForm.ApproverId = model.ApproverId;// userInfo.UserID;
                        exForm.ValidationDate = DateTime.Now;
                        dbContext.SaveChanges();
                        flag = ReturnType.Validated;
                    }
                    else flag = ReturnType.Initiated;
                }
            }
            return flag;
        }

        private int SaveCustomerDetail(KYADetail exForm)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                CustomerMst user = new CustomerMst();
                //user.UserId = Guid.NewGuid();
                user.PropertyId = exForm.RId.ToString();
                user.RegistrationId = exForm.RId;
                user.FirstName = dbContext.ApplicationDetails.FirstOrDefault(a => a.registrationId == exForm.RId).tFirstName + " " + dbContext.ApplicationDetails.FirstOrDefault(a => a.registrationId == exForm.RId).tMiddleName;
                user.LastName = dbContext.ApplicationDetails.FirstOrDefault(a => a.registrationId == exForm.RId).tLastName;
                user.UserName = exForm.RId.ToString();
                var newPassword = ApplicationHelper.CreatePassword();
                user.Password = newPassword.ToMD5HashForPassword();
                user.DepartmentId = exForm.DepartmentId;
                user.RoleId = 2;// NAConstant.CustomerRoleId;
                user.Sector = exForm.SectorId != null ? dbContext.SectorMsts.FirstOrDefault(s => s.sectorId == exForm.SectorId).sectorName : null;
                user.Block = exForm.BlockId != null ? dbContext.BlockMsts.FirstOrDefault(b => b.blockId == exForm.BlockId).blockName : null;
                user.PlotNo = exForm.PlotNo;
                if (exForm.AllotteeType == Constants.Company)
                {
                    user.Email = exForm.SignatoryEmail;
                    user.MobileNo = exForm.SignatoryMobileNo;
                }
                else
                {
                    user.Email = exForm.Email;
                    user.MobileNo = exForm.MobileNo;
                }
                //user.Question = model.SecurityQuestion;
                //user.Answer = model.SecurityAnswer;
                //user.CustomerIdFileName = fileI != null ? model.CustomerIdName : string.Empty; // model.CustomerIdName;
                user.IdFileType = exForm.IdFileType;
                //user.CustomerLetterFileName = fileII != null ? model.NALetterName : string.Empty; // model.NALetterName;
                user.PropertyFileType = exForm.PlotOwnershipFileType;
                user.CreatedDate = DateTime.Now;
                user.CreatedBy = "KYA";
                user.StatusId = NAStatusId.Approved;
                user.IsActive = true;
                user.IsLocked = false;
                user.Remarks = "";
                user.IsFirstTimeActivated = true;
                dbContext.CustomerMsts.Add(user);
                dbContext.SaveChanges();

                var msg = string.Empty; //"Dear User, You are successfully registered with mynoida.in. Your user name is " + exForm.RId + " and password is " + newPassword + " Regards, http://mynoida.in";
                if (user.MobileNo != null)
                {
                    msg = string.Format(NAMessages.PIS_Registration_Activation, exForm.RId, newPassword);
                    if (!string.IsNullOrEmpty(user.MobileNo)) { ApplicationHelper.SendSMS(user.MobileNo, msg); }
                }
                flag = ReturnType.Saved;
            }
            return flag;
        }


        public KYAViewModel GetKYADetailsById(KYAViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var details = dbContext.KYADetails.FirstOrDefault(r => r.Id == model.Id);
                if (details != null)
                {
                    model.Id = details.Id;
                    model.RegistrationId = details.RId;
                    model.DepartmentId = details.DepartmentId;
                    model.Department = details.DepartmentId != null ? dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == details.DepartmentId).departmentName : string.Empty;
                    model.AllotteeName = details.AllotteeName;
                    model.AllotteeType = details.AllotteeType;
                    model.KYAuid = details.KYAuid;
                    model.KYAReferenceCode = details.KYAReferenceCode;
                    model.SectorId = details.SectorId;
                    model.BlockId = details.BlockId;
                    model.Sector = details.SectorId != null ? dbContext.SectorMsts.FirstOrDefault(s => s.sectorId == details.SectorId).sectorName : Constants.NA;
                    model.Block = details.BlockId != null ? dbContext.BlockMsts.FirstOrDefault(b => b.blockId == details.BlockId).blockName : Constants.NA;
                    model.PlotNo = details.PlotNo;
                    model.MobileNo = details.MobileNo;
                    model.Email = details.Email;
                    model.GSTNo = details.GSTNo;
                    model.PAN = details.PAN;
                    model.AadharNo = details.AadharNo;
                    model.ROC = details.ROC;
                    model.SignatoryMobileNo = details.SignatoryMobileNo;
                    model.SignatoryEmail = details.SignatoryEmail;
                    model.AuthorizedSignatory = details.AuthorizedSignatory;
                    model.StatusId = details.StatusId;
                    model.Status = details.StatusId != null ? dbContext.StatusMasters.FirstOrDefault(s => s.Id == details.StatusId).Status : string.Empty;
                    model.IsForwarded = details.StatusId == NAStatusId.Forwarded ? true : false;
                    model.IsApproved = details.StatusId == NAStatusId.Approved ? true : false;
                    model.CorrespondAddress = details.CorrespondAddress;
                    model.ActionId = details.ActionId;
                    model.OptionalAction = details.OptionalAction;
                    model.Remarks = details.Remarks;
                    model.SubmitDate = details.SubmitDate;
                    model.ValidationDate = details.ValidationDate;
                    model.ApprovalDate = details.ApprovalDate;
                    model.CreatedDate = details.CreatedDate;
                    model.ValidatorId = details.ValidatorId;
                    model.Validator = details.ValidatorId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == details.ValidatorId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == details.ValidatorId).LastName) : string.Empty;
                    model.ApproverId = details.ApproverId;
                    model.Approver = details.ApproverId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == details.ApproverId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == details.ApproverId).LastName) : string.Empty;
                    model.CommunicationAddress = (string.IsNullOrEmpty(details.AddressLine1) ? string.Empty : details.AddressLine1 + ",")
                        + (string.IsNullOrEmpty(details.AddressLine2) ? string.Empty : details.AddressLine2 + ",")
                        + (string.IsNullOrEmpty(details.AreaLocality) ? string.Empty : details.AreaLocality + ",")
                        + (string.IsNullOrEmpty(details.City) ? string.Empty : details.City + ",")
                        + (string.IsNullOrEmpty(details.State) ? string.Empty : details.State + ",")
                        + (string.IsNullOrEmpty(details.PinCode) ? string.Empty : "Pincode-" + details.PinCode);
                    //model.DocumentPath= GetDocumentListByRegistrationId(model.RegistrationId).DocumentPath;
                }
                return model;
            }
        }


        public DataSourceResult GetDocumentListByRegistrationId(DataSourceRequest request, int? rid, int? id)
        {
            FtpHandler ftp = new FtpHandler();
            bool IsOldFormat = false;
            string path = FtpHandler.GetDocumentPathForKYA(rid.ToString(), Constants.KYA, id.ToString(), string.Empty, true, IsOldFormat);
            //string path = FtpHandler.GetDocumentPathForKYA(rid.ToString(), Constants.KYA, string.Empty, true);//System.Configuration.ConfigurationManager.AppSettings["RootPath"] + "\\" + rid;
            List<string> fileList = ftp.DirSearch(path);
            List<DocumentViewModel> documentList = new List<DocumentViewModel>();
            int count = 1;
            if (fileList.Count == 0)
            {
                path = FtpHandler.GetDocumentPathForKYA(rid.ToString(), Constants.KYA, id.ToString(), string.Empty, true, true);
                fileList = ftp.DirSearch(path);
                IsOldFormat = fileList.Count > 0 ? true : false;
            }
            if (fileList != null && fileList.Count > 0)
            {
                foreach (string file in fileList)
                {
                    DocumentViewModel document = new DocumentViewModel();
                    //string filename = file;
                    //string str1 = string.Empty;
                    //if (filename.Contains(" "))
                    //{
                    //    filename = filename.Replace(" ", "");
                    //}
                    //if ((filename.Split('-')).Length > 1)
                    //{
                    //    str1 = filename.Substring(0, filename.Length - 4);
                    //    str1 = str1.Substring(9, str1.Length - 9);
                    //}
                    //document.DocumentPath = FtpHandler.GetDocumentPathForKYA(rid.ToString(), Constants.KYA, file, false);
                    document.DocumentPath = FtpHandler.GetDocumentPathForKYA(rid.ToString(), Constants.KYA, id.ToString(), file, false, IsOldFormat);
                    int index = file.LastIndexOf(".");
                    document.DocumentName = index == -1 ? file : file.Substring(0, index).ToUpper();
                    var newString = document.DocumentName.Replace("FILE", "");
                    document.DocumentName = newString + " " + "FILE";
                    //document.DocumentName = file.Substring(0, file.Length - 4).ToUpper();
                    //document.DocumentName = !(string.IsNullOrEmpty(str1)) ? (!(string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[str1])) ? System.Configuration.ConfigurationManager.AppSettings[str1] : "Other Documents") : "Other Documents";
                    document.RegistrationId = rid;
                    document.SerialNo = count;
                    documentList.Add(document);
                    count++;
                }
                return documentList.ToDataSourceResult(request);
            }
            else
            {
                return null;
            }
        }


        public DataSourceResult GetKYAFormListForValidation(DataSourceRequest request, KYAViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from kya in dbContext.KYADetails
                            where kya.ApproverId == userInfo.UserID
                            && (model.Id == null || kya.Id == model.Id)
                            && (model.DepartmentId == null || kya.DepartmentId == model.DepartmentId)
                            && (model.ActionId == null || kya.StatusId == model.ActionId)
                            && (model.StartDate == null || DbFunctions.TruncateTime(kya.CreatedDate) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(kya.CreatedDate) <= DbFunctions.TruncateTime(model.EndDate))
                            && DepartmentList.Contains(kya.DepartmentId)
                            select new KYAViewModel
                            {
                                Id = kya.Id,
                                KYAuid = kya.KYAuid,
                                RegistrationId = kya.RId,
                                DepartmentId = kya.DepartmentId,
                                SectorId = kya.SectorId,
                                BlockId = kya.BlockId,
                                PlotNo = kya.PlotNo,
                                Department = kya.DepartmentId != null ? dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == kya.DepartmentId).departmentName : string.Empty,
                                Sector = kya.SectorId != null ? dbContext.SectorMsts.FirstOrDefault(s => s.sectorId == kya.SectorId).sectorName : Constants.NA,
                                Block = kya.BlockId != null ? dbContext.BlockMsts.FirstOrDefault(b => b.blockId == kya.BlockId).blockName : Constants.NA,
                                AllotteeName = kya.AllotteeName,
                                AllotteeType = kya.AllotteeType,
                                CorrespondAddress = kya.CorrespondAddress,
                                AuthorizedSignatory = kya.AllotteeType == Constants.Company ? kya.AuthorizedSignatory : kya.FatherOrHusbandName,
                                CountryCode = kya.CountryCode,
                                MobileNo = kya.MobileNo,
                                SignatoryMobileNo = kya.SignatoryMobileNo,
                                Email = kya.Email,
                                SignatoryEmail = kya.SignatoryEmail,
                                PhoneNo = kya.PhoneNo,
                                GSTNo = kya.GSTNo,
                                PAN = kya.PAN,
                                AadharNo = kya.AadharNo,
                                ROC = kya.ROC,
                                AddressLine1 = kya.AddressLine1,
                                AddressLine2 = kya.AddressLine2,
                                AreaLocality = kya.AreaLocality,
                                City = kya.City,
                                State = kya.State,
                                PinCode = kya.PinCode,
                                KYAReferenceCode = kya.KYAReferenceCode,
                                AllotteeIdFileType = kya.IdFileType,
                                PlotOwnershipFileType = kya.PlotOwnershipFileType,
                                OtherFileType = kya.OtherFileType,
                                IsActive = kya.IsActive,
                                StatusId = kya.StatusId,
                                Status = kya.StatusId != null ? dbContext.StatusMasters.FirstOrDefault(s => s.Id == kya.StatusId).Status : string.Empty,
                                SubmitDate = kya.SubmitDate,
                                ApprovalDate = kya.ApprovalDate,
                                CreatedDate = kya.CreatedDate,
                                ValidatorId = kya.ValidatorId,
                                Validator = kya.ValidatorId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ValidatorId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ValidatorId).LastName) : string.Empty,
                                ApproverId = kya.ApproverId,
                                Approver = kya.ApproverId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ApproverId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == kya.ApproverId).LastName) : string.Empty,
                                Remarks = kya.Remarks,
                                OptionalAction = kya.OptionalAction,
                                ActionId = kya.ActionId,
                                IsInitiated = kya.StatusId == NAStatusId.Initiated ? true : false,
                                IsForwarded = kya.StatusId == NAStatusId.Forwarded ? true : false,
                                IsApproved = kya.StatusId == NAStatusId.Approved ? true : false
                            });
                return data.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetKYADetailStatusList(DataSourceRequest request, KYAViewModel model)
        {
            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                //var list = connection.Query<KYAViewModel>("SP_KYAReport", commandType: System.Data.CommandType.StoredProcedure).ToList();
                var list = connection.Query<KYAViewModel>("SP_KYADashboardGraph", new { DepartmentId = model.DepartmentId, StartDate = model.StartDate, EndDate = model.EndDate, FilterType = model.FilterType }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                return list.ToDataSourceResult(request);
            }
        }

        public decimal GetAverageApprovedForm()
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                KYAViewModel model = new KYAViewModel();
                model.TotalKYA = dbContext.KYADetails.Where(k => k.DepartmentId != null && k.StatusId != 15).Count();
                model.ApprovedKYA = dbContext.KYADetails.Where(k => k.DepartmentId != null && k.StatusId == 1).Count();
                model.AverageKYAForm = Math.Round(((decimal)model.ApprovedKYA / (decimal)model.TotalKYA) * 100, 2);
                return (decimal)model.AverageKYAForm;
            }
        }

        public List<KYAViewModel> GetKYAFormListCount(KYAViewModel model)
        {
            List<KYAViewModel> list = new List<KYAViewModel>();
            using (System.Data.IDbConnection connection = NADBConnection.GetPIMSConnection())
            {
                list = connection.Query<KYAViewModel>("SP_KYADashboardGraph", new { DepartmentId = model.DepartmentId, StartDate = model.StartDate, EndDate = model.EndDate, FilterType = model.FilterType }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                //return list.ToDataSourceResult(request);
            }


            //if (model.StatusDate != null)
            //{
            //    var FromDate = Convert.ToDateTime(model.StatusDate.Value.ToString("MM/dd/yyyy") + " 00:00");
            //    var ToDate = Convert.ToDateTime(model.StatusDate.Value.ToString("MM/dd/yyyy") + " 23:59");
            //    using (var dbContext = new NoidaPMSEntities())
            //    {
            //        for (int i = 1; i < 8; i++)
            //        {
            //            KYAViewModel kya = new KYAViewModel();
            //            kya.DepartmentId = i;
            //            kya.Department = dbContext.DepartmentMsts.FirstOrDefault(m => m.departmentId == i).departmentName;
            //            kya.Count = dbContext.KYADetails.Where(k => k.DepartmentId == i && k.SubmitDate >= FromDate && k.SubmitDate <= ToDate).ToList().Count;
            //            list.Add(kya);
            //        }
            //    }
            //}
            //else
            //{
            //    using (var dbContext = new NoidaPMSEntities())
            //    {
            //        for (int i = 1; i < 8; i++)
            //        {
            //            KYAViewModel kya = new KYAViewModel();
            //            kya.DepartmentId = i;
            //            kya.Department = dbContext.DepartmentMsts.FirstOrDefault(m => m.departmentId == i).departmentName;
            //            kya.Count = dbContext.KYADetails.Where(k => k.DepartmentId == i).ToList().Count;
            //            list.Add(kya);
            //        }
            //    }
            //}
            return list;
        }


        public PaymentViewModel GetDetailsForNDC(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from alotment in dbContext.AllotmentMasters
                            join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where alotment.rid == model.RegistrationId
                            select new PaymentViewModel
                            {
                                RegistrationId = alotment.rid,
                                DepartmentId = alotment.departmentId,
                                DepartmentName = alotment.DepartmentMst.departmentName,
                                SectorId = property.sectorId,
                                SectorName = property.SectorMst.sectorName,
                                BlockId = property.blockId,
                                BlockName = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                //PropertyNo = property.SectorMst.sectorName+"/"+property.BlockMst.blockName+"-"+property.propertyNo,
                                Applicant = aplicant.tGender == "Company" ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : (aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName),
                                ApplicantAddress = aplicant.tCorrespondanceAdd
                            }).FirstOrDefault();
                return data;
            }
        }


        public DataSourceResult GetNDCGeneratedListAsDataSource(DataSourceRequest request, NDCVeiwModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (HttpContext.Current.Session["DNDCDepartmentId"] != null) model.DepartmentId = (int)HttpContext.Current.Session["DNDCDepartmentId"];
                List<NDCVeiwModel> idList = new List<NDCVeiwModel>();
                idList = (from ndcmst in dbContext.NDCDetailMsts
                          select new NDCVeiwModel
                          {
                              Id = dbContext.NDCDetailMsts.Where(m => m.RegistrationId == ndcmst.RegistrationId).OrderByDescending(c => c.Id).FirstOrDefault().Id
                          }).ToList();
                List<int> intList = idList.Select(k => (int)k.Id).ToList();

                var data = (from ndc in dbContext.NDCDetailMsts
                            where intList.Contains(ndc.Id)
                            && (model.DepartmentId == null || ndc.DepartmentId == model.DepartmentId)
                            && (model.StatusId == null || ndc.StatusId == model.StatusId)
                            //&& ndc.CreatedBy == userInfo.UserID
                            select new NDCVeiwModel
                            {
                                Id = ndc.Id,
                                RegistrationId = ndc.RegistrationId.ToString(),
                                RegistrationNo = ndc.RegistrationId,
                                Applicant = ndc.Applicant,
                                Sector = ndc.Sector,
                                Block = ndc.Block,
                                PlotNo = ndc.PlotNo,
                                //Department = ndc.DepartmentId == 1 ? DepartmentInHindi.Institutional : (ndc.DepartmentId == 2 ? DepartmentInHindi.Commercial : (ndc.DepartmentId == 3 ? DepartmentInHindi.Residential : (ndc.DepartmentId == 4 ? DepartmentInHindi.Industry : (ndc.DepartmentId == 5 ? DepartmentInHindi.Housing : (ndc.DepartmentId == 6 ? DepartmentInHindi.GroupHousing : DepartmentInHindi.Residential))))),
                                Department = ndc.Department,
                                InstallmentDateInWord = ndc.InstallmentDateInWord,
                                InterestDateInWord = ndc.InterestDateInWord,
                                LeaseRentDateInWord = ndc.LeaseRentDateInWord,
                                IsTotalInstallmentPaid = ndc.IsTotalInstallmentPaid,
                                IsOneTimeLeasePaid = ndc.IsOneTimeLeasePaid,
                                NDCDate = ndc.NDCDate,
                                LetterNo = ndc.LetterNo,
                                LetterDateInWord = ndc.LetterDateInWord,
                                ChallanId = ndc.ChallanDetail,
                                ChalanAmount = ndc.ChallanAmount,
                                StatusId = ndc.StatusId,
                                Status = ndc.StatusId != null ? dbContext.StatusMasters.FirstOrDefault(s => s.Id == ndc.StatusId).Status : string.Empty,
                                TemplateContent = ndc.NDCTemplate,
                                CreatedDate = ndc.CreatedDate,
                                IsActive = ndc.IsActive,
                                Remarks = ndc.Remarks,
                                IsLetterIssued = ndc.IsLetterIssued,
                                ActionType = ndc.LetterType
                            });
                HttpContext.Current.Session["DNDCDepartmentId"] = null;
                return data.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetOnlineCustomerServiceRequestAsDataSource(DataSourceRequest request, ServiceViewModel model)
        {
            int userid = userInfo.UserID;
            IQueryable<ServiceViewModel> list;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (HttpContext.Current.Session["ServiceStatusId"] != null) model.ServiceStatusId = (int)HttpContext.Current.Session["ServiceStatusId"];
                if (HttpContext.Current.Session["ServiceName"] != null)
                {
                    string service = (string)HttpContext.Current.Session["ServiceName"];
                    int? serviceId = dbContext.Database.SqlQuery<int>("select distinct service_id from CitizenService_Master where servicename like '" + service + "%'").FirstOrDefault();
                    model.ServiceId = serviceId;
                }
                List<string> SubDepartmentList = new List<string>();
                List<int?> DepartmentListForBothAccess = new List<int?>() { 3, 7, 5 };
                List<int?> ServiceIdList = new List<int?>();
                ServiceIdList = dbContext.CitizenService_Master.Where(u => u.Status == 1).Select(d => d.service_id).Distinct().ToList();
                if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Accountant || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.HODAccounts)
                {
                    SubDepartmentList.Add("Account");
                }
                else
                {
                    if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Property || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Assistant)
                    {
                        SubDepartmentList.Add("Property");
                    }
                    else
                    {
                        SubDepartmentList.Add("Account");
                        SubDepartmentList.Add("Property");
                    }
                }
                if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.OSD)
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && SubDepartmentList.Contains(csr.SubDepartment) && csm.Status == 1 && csr.RequestThrough == Constants.Online //&& csr.ServiceId != 12 && csr.ServiceId != 13//csr.Created_By == 0 && csr.ServiceType != "P" && csr.IsActive == true
                            && (!DepartmentListForBothAccess.Contains(csr.DepartmentId) ? csr.ServiceId != 12 && csr.ServiceId != 13 : ServiceIdList.Contains(csr.ServiceId))
                            && (model.Id == null || csr.Id == model.Id)
                            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                            && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            select new ServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                                RequestDate = csr.Created_Date,
                                CreatedDate = csr.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = csr.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = csr.ServiceType,
                                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                                Amount = csr.DuesAmount,
                                StatusId = sts.Id,
                                Status = sts.Status,
                                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                ApproverId = csr.ApproverId,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                Sector = csr.SectorName,
                                Block = csr.BlockName,
                                PlotNo = csr.PlotNo,
                                IsUploaded = csr.IsUploadedLetter
                            });
                }
                else if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.HODAccounts)
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && csm.Status == 1 && csr.RequestThrough == Constants.Online && (csr.ServiceId == 12 || csr.ServiceId == 13 || csr.ApproverId == userInfo.UserID) //csr.Created_By == 0 && csr.ServiceType != "P" && csr.IsActive == true
                            && (model.Id == null || csr.Id == model.Id)
                            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                            && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            select new ServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                                RequestDate = csr.Created_Date,
                                CreatedDate = csr.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = csr.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = csr.ServiceType,
                                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                                Amount = csr.DuesAmount,
                                StatusId = sts.Id,
                                Status = sts.Status,
                                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                ApproverId = csr.ApproverId,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                Sector = csr.SectorName,
                                Block = csr.BlockName,
                                PlotNo = csr.PlotNo,
                                IsUploaded = csr.IsUploadedLetter
                            });
                }
                else if (userInfo.RoleMaster.RoleInDepartment == null || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Admin)
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && SubDepartmentList.Contains(csr.SubDepartment) && csm.Status == 1 && csr.RequestThrough == Constants.Online //csr.Created_By == 0 && csr.ServiceType != "P" && csr.IsActive == true
                            && (model.Id == null || csr.Id == model.Id)
                            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                            && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            select new ServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                                RequestDate = csr.Created_Date,
                                CreatedDate = csr.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = csr.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = csr.ServiceType,
                                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                                Amount = csr.DuesAmount,
                                StatusId = sts.Id,
                                Status = sts.Status,
                                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                ApproverId = csr.ApproverId,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                Sector = csr.SectorName,
                                Block = csr.BlockName,
                                PlotNo = csr.PlotNo,
                                IsUploaded = csr.IsUploadedLetter
                            });
                }
                else
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && csm.Status == 1 && csr.RequestThrough == Constants.Online && csr.ApproverId == userid//csr.Created_By == 0 && csr.ServiceType != "P" && SubDepartmentList.Contains(csr.SubDepartment) && csr.IsActive == true
                            && (model.Id == null || csr.Id == model.Id)
                            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                            && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            select new ServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                                RequestDate = csr.Created_Date,
                                CreatedDate = csr.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = csr.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = csr.ServiceType,
                                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                                Amount = csr.DuesAmount,
                                StatusId = sts.Id,
                                Status = sts.Status,
                                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                ApproverId = csr.ApproverId,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                Sector = csr.SectorName,
                                Block = csr.BlockName,
                                PlotNo = csr.PlotNo,
                                IsUploaded = csr.IsUploadedLetter
                            });
                }
                //if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Assistant || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Accountant)
                //{
                //    list = (from csr in dbContext.Customer_ServiceRequest
                //            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                //            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                //            where DepartmentList.Contains(csr.DepartmentId) && SubDepartmentList.Contains(csr.SubDepartment) && csm.Status == 1 && csr.RequestThrough == Constants.Online && csr.ApproverId == userid && csr.IsActive == true//csr.Created_By == 0 && csr.ServiceType != "P"
                //            && (model.Id == null || csr.Id == model.Id)
                //            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                //            && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                //            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                //            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                //            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                //            select new ServiceViewModel
                //            {
                //                Id = csr.Id,
                //                RequestId = csr.Id,
                //                RegistrationNo = csr.Registration_No,
                //                DepartmentId = csr.DepartmentId,
                //                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                //                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                //                RequestDate = csr.Created_Date,
                //                CreatedDate = csr.Created_Date,
                //                Timeline = csm.Timeline,
                //                ServiceId = csr.ServiceId,
                //                ServiceName = csm.ServiceName,
                //                ServiceType = csr.ServiceType,
                //                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                //                Amount = csr.DuesAmount,
                //                StatusId = sts.Id,
                //                Status = sts.Status,
                //                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                //                Comment = csr.Comment,
                //                MobileNo = csr.MobileNumber,
                //                Applicant = csr.ApplicantName,
                //                Description = csr.Description,
                //                Requestor = csr.RequestorName,
                //                RequestorAddress = csr.RequestorAddress,
                //                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                //                ApproverId = csr.ApproverId,
                //                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName
                //            });
                //}
                //else
                //{
                //    list = (from csr in dbContext.Customer_ServiceRequest
                //            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                //            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                //            where DepartmentList.Contains(csr.DepartmentId) && SubDepartmentList.Contains(csr.SubDepartment) && csm.Status == 1 && csr.RequestThrough == Constants.Online && csr.IsActive == true //csr.Created_By == 0 && csr.ServiceType != "P"
                //            && (model.Id == null || csr.Id == model.Id)
                //            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                //            && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                //            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                //            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                //            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                //            select new ServiceViewModel
                //            {
                //                Id = csr.Id,
                //                RequestId = csr.Id,
                //                RegistrationNo = csr.Registration_No,
                //                DepartmentId = csr.DepartmentId,
                //                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                //                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                //                RequestDate = csr.Created_Date,
                //                CreatedDate = csr.Created_Date,
                //                Timeline = csm.Timeline,
                //                ServiceId = csr.ServiceId,
                //                ServiceName = csm.ServiceName,
                //                ServiceType = csr.ServiceType,
                //                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                //                Amount = csr.DuesAmount,
                //                StatusId = sts.Id,
                //                Status = sts.Status,
                //                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                //                Comment = csr.Comment,
                //                MobileNo = csr.MobileNumber,
                //                Applicant = csr.ApplicantName,
                //                Description = csr.Description,
                //                Requestor = csr.RequestorName,
                //                RequestorAddress = csr.RequestorAddress,
                //                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                //                ApproverId = csr.ApproverId,
                //                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName
                //            });
                //}

                HttpContext.Current.Session["ServiceStatusId"] = null;
                HttpContext.Current.Session["ServiceName"] = null;
                return list.ToDataSourceResult(request);
            }
        }


        public int UpdateRegistrationIdToServiceRequestById(ServiceViewModel model)
        {
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var service = dbContext.Customer_ServiceRequest.FirstOrDefault(r => r.Id == model.Id);
                service.Registration_No = model.RegistrationId.ToString();
                dbContext.SaveChanges();
                flag = ReturnType.Updated;
            }
            return flag;
        }


        public DataSourceResult GetCustomerServiceRequestByJSKAsDataSource(DataSourceRequest request, ServiceViewModel model)
        {
            IQueryable<ServiceViewModel> list;
            using (var dbContext = new NoidaPMSEntities())
            {
                if (HttpContext.Current.Session["ServiceStatusId"] != null) model.ServiceStatusId = (int)HttpContext.Current.Session["ServiceStatusId"];
                if (HttpContext.Current.Session["ServiceName"] != null)
                {
                    string service = (string)HttpContext.Current.Session["ServiceName"];
                    int? serviceId = dbContext.Database.SqlQuery<int>("select distinct service_id from CitizenService_Master where servicename like '" + service + "%'").FirstOrDefault();
                    model.ServiceId = serviceId;
                }
                List<int?> DepartmentListForBothAccess = new List<int?>() { 3, 7, 5 };
                List<int?> ServiceIdList = new List<int?>();
                ServiceIdList = dbContext.CitizenService_Master.Where(u => u.Status == 1).Select(d => d.service_id).Distinct().ToList();
                List<string> SubDepartmentList = new List<string>();
                if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Accountant || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.HODAccounts)
                {
                    SubDepartmentList.Add("Account");
                }
                else
                {
                    if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Property || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Assistant)
                    {
                        SubDepartmentList.Add("Property");
                    }
                    else
                    {
                        SubDepartmentList.Add("Account");
                        SubDepartmentList.Add("Property");
                    }
                }
                //if (HttpContext.Current.Session["ServiceStatusId"] != null) model.ServiceStatusId = (int)HttpContext.Current.Session["ServiceStatusId"];
                //if (HttpContext.Current.Session["ServiceId"] != null) model.ServiceStatusId = (int)HttpContext.Current.Session["ServiceId"];
                if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.OSD)
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && SubDepartmentList.Contains(csr.SubDepartment) && csm.Status == 1 && csr.RequestThrough == Constants.JSK //&& csr.ServiceId != 12 && csr.ServiceId != 13//&& csr.IsActive == true && csr.Created_By == 0 && csr.ServiceType != "P"
                            && (!DepartmentListForBothAccess.Contains(csr.DepartmentId) ? csr.ServiceId != 12 && csr.ServiceId != 13 : ServiceIdList.Contains(csr.ServiceId))
                            && (model.Id == null || csr.Id == model.Id)
                            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                            && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            select new ServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                                RequestDate = csr.Created_Date,
                                CreatedDate = csr.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = csr.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = csr.ServiceType,
                                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                                Amount = csr.DuesAmount,
                                StatusId = sts.Id,
                                Status = sts.Status,
                                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                ApproverId = csr.ApproverId,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                Sector = csr.SectorName,
                                Block = csr.BlockName,
                                PlotNo = csr.PlotNo,
                                IsUploaded = csr.IsUploadedLetter
                            });
                }
                else if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.HODAccounts)
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && SubDepartmentList.Contains(csr.SubDepartment) && csm.Status == 1 && csr.RequestThrough == Constants.JSK && (csr.ServiceId == 12 || csr.ServiceId == 13) //&& csr.IsActive == true && csr.Created_By == 0 && csr.ServiceType != "P"
                            && (model.Id == null || csr.Id == model.Id)
                            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                            && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            select new ServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                                RequestDate = csr.Created_Date,
                                CreatedDate = csr.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = csr.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = csr.ServiceType,
                                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                                Amount = csr.DuesAmount,
                                StatusId = sts.Id,
                                Status = sts.Status,
                                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                ApproverId = csr.ApproverId,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                Sector = csr.SectorName,
                                Block = csr.BlockName,
                                PlotNo = csr.PlotNo,
                                IsUploaded = csr.IsUploadedLetter
                            });
                }
                else if (userInfo.RoleMaster.RoleInDepartment == null || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Admin)
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && SubDepartmentList.Contains(csr.SubDepartment) && csm.Status == 1 && csr.RequestThrough == Constants.JSK //csr.Created_By == 0 && csr.ServiceType != "P" && csr.IsActive == true
                            && (model.Id == null || csr.Id == model.Id)
                            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                            && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            select new ServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                                RequestDate = csr.Created_Date,
                                CreatedDate = csr.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = csr.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = csr.ServiceType,
                                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                                Amount = csr.DuesAmount,
                                StatusId = sts.Id,
                                Status = sts.Status,
                                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                ApproverId = csr.ApproverId,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                Sector = csr.SectorName,
                                Block = csr.BlockName,
                                PlotNo = csr.PlotNo,
                                IsUploaded = csr.IsUploadedLetter
                            });
                }
                else
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && csm.Status == 1 && csr.RequestThrough == Constants.JSK && csr.ApproverId == userInfo.UserID //csr.Created_By == 0 && csr.ServiceType != "P" && SubDepartmentList.Contains(csr.SubDepartment) && csr.IsActive == true
                            && (model.Id == null || csr.Id == model.Id)
                            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                            && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            select new ServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                                RequestDate = csr.Created_Date,
                                CreatedDate = csr.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = csr.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = csr.ServiceType,
                                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                                Amount = csr.DuesAmount,
                                StatusId = sts.Id,
                                Status = sts.Status,
                                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                ApproverId = csr.ApproverId,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                Sector = csr.SectorName,
                                Block = csr.BlockName,
                                PlotNo = csr.PlotNo,
                                IsUploaded = csr.IsUploadedLetter
                            });
                }
                //if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Assistant || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Accountant)
                //{
                //    list = (from csr in dbContext.Customer_ServiceRequest
                //            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                //            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                //            where DepartmentList.Contains(csr.DepartmentId) && SubDepartmentList.Contains(csr.SubDepartment) && csm.Status == 1 && csr.RequestThrough == Constants.JSK && csr.ApproverId == userInfo.UserID && csr.IsActive == true
                //            && (model.Id == null || csr.Id == model.Id)
                //            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                //            && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                //            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                //            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                //            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                //            select new ServiceViewModel
                //            {
                //                Id = csr.Id,
                //                RequestId = csr.Id,
                //                RegistrationNo = csr.Registration_No,
                //                DepartmentId = csr.DepartmentId,
                //                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                //                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                //                RequestDate = csr.Created_Date,
                //                CreatedDate = csr.Created_Date,
                //                Timeline = csm.Timeline,
                //                ServiceId = csr.ServiceId,
                //                ServiceName = csm.ServiceName,
                //                ServiceType = csr.ServiceType,
                //                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                //                Amount = csr.DuesAmount,
                //                StatusId = sts.Id,
                //                Status = sts.Status,
                //                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                //                Comment = csr.Comment,
                //                MobileNo = csr.MobileNumber,
                //                Applicant = csr.ApplicantName,
                //                Description = csr.Description,
                //                Requestor = csr.RequestorName,
                //                RequestorAddress = csr.RequestorAddress,
                //                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                //                ApproverId = csr.ApproverId,
                //                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName
                //            });
                //}
                //else
                //{
                //    list = (from csr in dbContext.Customer_ServiceRequest
                //            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                //            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                //            where DepartmentList.Contains(csr.DepartmentId) && SubDepartmentList.Contains(csr.SubDepartment) && csm.Status == 1 && csr.RequestThrough == Constants.JSK && csr.IsActive == true //&& csr.ServiceType != "P"
                //            && (model.Id == null || csr.Id == model.Id)
                //            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                //            && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                //            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                //            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                //            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                //            select new ServiceViewModel
                //            {
                //                Id = csr.Id,
                //                RequestId = csr.Id,
                //                RegistrationNo = csr.Registration_No,
                //                DepartmentId = csr.DepartmentId,
                //                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                //                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                //                RequestDate = csr.Created_Date,
                //                CreatedDate = csr.Created_Date,
                //                Timeline = csm.Timeline,
                //                ServiceId = csr.ServiceId,
                //                ServiceName = csm.ServiceName,
                //                ServiceType = csr.ServiceType,
                //                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                //                Amount = csr.DuesAmount,
                //                StatusId = sts.Id,
                //                Status = sts.Status,
                //                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                //                Comment = csr.Comment,
                //                MobileNo = csr.MobileNumber,
                //                Applicant = csr.ApplicantName,
                //                Description = csr.Description,
                //                Requestor = csr.RequestorName,
                //                RequestorAddress = csr.RequestorAddress,
                //                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                //                ApproverId = csr.ApproverId,
                //                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName
                //            });
                //}

                HttpContext.Current.Session["ServiceStatusId"] = null;
                HttpContext.Current.Session["ServiceName"] = null;
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetServiceRequestUploadedDocumentsById(DataSourceRequest request, ServiceViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<DocumentViewModel> documentList = new List<DocumentViewModel>();
                FtpHandler ftpHandler = new FtpHandler();
                var service = dbContext.Customer_ServiceRequest.FirstOrDefault(f => f.Id == model.RequestId);
                if (model.RegistrationId == null)
                {
                    if (service != null)
                    {
                        documentList = ftpHandler.GetServiceRequestDocuments((int)model.RequestId);
                        return documentList.ToDataSourceResult(request);
                    }
                    else return null;
                }
                else
                {
                    documentList = ftpHandler.GetServiceRequestUploadedDocumentsById((int)model.RegistrationId, (int)model.RequestId);
                    return documentList.ToDataSourceResult(request);
                }
            }
        }


        public int UpdateRemarksForNDCLetter(NDCVeiwModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var ndcDetail = dbContext.NDCDetailMsts.FirstOrDefault(m => m.RegistrationId.ToString() == model.RegistrationId && m.Id == model.Id);
                if (ndcDetail != null)
                {
                    ndcDetail.IsLetterIssued = true;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }


        public string GetNDCLetter(NDCVeiwModel model)
        {
            string ndcletter = string.Empty;
            using (var dbContext = new NoidaPMSEntities())
            {
                var ndcDetail = dbContext.NDCDetailMsts.FirstOrDefault(m => m.Id == model.Id);
                if (ndcDetail != null)
                {
                    //model.TemplateContent = ndcDetail.NDCTemplate;
                    //model.Department = ndcDetail.DepartmentId == 1 ? DepartmentInHindi.Institutional : (ndcDetail.DepartmentId == 2 ? DepartmentInHindi.Commercial : (ndcDetail.DepartmentId == 3 ? DepartmentInHindi.Residential : (ndcDetail.DepartmentId == 4 ? DepartmentInHindi.Industry : (ndcDetail.DepartmentId == 5 ? DepartmentInHindi.Housing : (ndcDetail.DepartmentId == 6 ? DepartmentInHindi.GroupHousing : DepartmentInHindi.Residential)))));
                    //model.TemplateContent.Replace("????????", model.Department);

                    //var alloteeDetail = dbContext.ApplicationDetails.FirstOrDefault(m => m.registrationId.ToString() == model.RegistrationId);
                    model.RegistrationId = ndcDetail.RegistrationId.ToString();
                    model.DepartmentId = ndcDetail.DepartmentId;
                    model.Department = ndcDetail.Department;
                    model.Applicant = ndcDetail.Applicant;
                    model.Sector = ndcDetail.Sector;
                    model.Block = ndcDetail.Block;
                    model.PlotNo = ndcDetail.PlotNo;
                    model.InstallmentPaidAmount = ndcDetail.InstallmentDuesAmount;
                    model.InstallmentDateInWord = ndcDetail.InstallmentDateInWord;
                    model.InstallmentInWord = ndcDetail.InstallmentStatus;
                    model.InstallmentPaidUpto = ndcDetail.InstallmentPaidUptoDate;
                    model.InstallmentInterestPaidAmount = ndcDetail.InterestDuesAmount;
                    model.InterestDateInWord = ndcDetail.InterestDateInWord;
                    model.LeaseRentPaidAmount = ndcDetail.LeaseRentAmount; //== null ? 0 : (double)ndcDetail.LeaseRentAmount;
                    model.LeaseRentDateInWord = ndcDetail.LeaseRentDateInWord;
                    model.LeaseRentInterestPaidUpto = ndcDetail.LeaseRentPaidUptoDate;
                    model.LeaseRentInWord = ndcDetail.LeaseRentStatus;
                    model.IsOneTimeLeasePaid = ndcDetail.IsOneTimeLeasePaid;
                    model.IsTotalInstallmentPaid = ndcDetail.IsTotalInstallmentPaid;
                    model.NDCDate = ndcDetail.NDCDate;
                    model.LetterNo = ndcDetail.LetterNo;
                    model.LetterDateInWord = ndcDetail.LetterDateInWord;
                    model.ChalanAmount = ndcDetail.ChallanAmount;
                    model.BankName = ndcDetail.BankName;
                    model.Remarks = ndcDetail.Remarks;
                    model.TemplateContent = ndcDetail.NDCTemplate;
                    model.CreatedDate = ndcDetail.CreatedDate;
                    model.PropertyNo = ndcDetail.Sector + "/" + ndcDetail.Block + "-" + ndcDetail.PlotNo;
                    model.Address = ndcDetail.ApplicantAddress;
                    model.ChallanId = ndcDetail.ChallanDetail;
                    model.ActionType = ndcDetail.LetterType;
                    model.IsLetterIssued = ndcDetail.IsLetterIssued;

                    if (ndcDetail.DepartmentId != 2)
                    {
                        model.Department = ndcDetail.DepartmentId == 1 ? DepartmentInHindi.Institutional : (ndcDetail.DepartmentId == 2 ? DepartmentInHindi.Commercial : (ndcDetail.DepartmentId == 3 ? DepartmentInHindi.Residential : (ndcDetail.DepartmentId == 4 ? DepartmentInHindi.Industry : (ndcDetail.DepartmentId == 5 ? DepartmentInHindi.Housing : (ndcDetail.DepartmentId == 6 ? DepartmentInHindi.GroupHousing : DepartmentInHindi.Residential)))));
                        ndcletter = RazorParser.ParseTemplate(model, "NDCIndustryTemplate.cshtml");
                    }
                    else
                    {
                        ndcletter = RazorParser.ParseTemplate(model, "NDCCommercialTemplateEng.cshtml");
                    }
                    if (ndcDetail.NDCTemplate == null)
                    {
                        ndcDetail.NDCTemplate = ndcletter;
                        dbContext.SaveChanges();
                    }
                }
            }
            return ndcletter;
        }


        public NDCVeiwModel GetNDCDetailsById(int? Id)
        {
            NDCVeiwModel model = new NDCVeiwModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                var ndcDetail = dbContext.NDCDetailMsts.FirstOrDefault(m => m.Id == Id);
                //var alloteeDetail=dbContext.ApplicationDetails.FirstOrDefault(m=>m.registrationId.ToString()==model.RegistrationId);
                model.RegistrationId = ndcDetail.RegistrationId.ToString();
                model.DepartmentId = ndcDetail.DepartmentId;
                model.Department = ndcDetail.Department;
                model.Applicant = ndcDetail.Applicant;
                model.Sector = ndcDetail.Sector;
                model.Block = ndcDetail.Block;
                model.PlotNo = ndcDetail.PlotNo;
                model.InstallmentPaidAmount = ndcDetail.InstallmentDuesAmount;
                model.InstallmentDateInWord = ndcDetail.InstallmentDateInWord;
                model.InstallmentInWord = ndcDetail.InstallmentStatus;
                model.InstallmentPaidUpto = ndcDetail.InstallmentPaidUptoDate;
                model.InstallmentInterestPaidAmount = ndcDetail.InterestDuesAmount;
                model.InterestDateInWord = ndcDetail.InterestDateInWord;
                model.LeaseRentPaidAmount = ndcDetail.LeaseRentAmount; //== null ? 0 : (double)ndcDetail.LeaseRentAmount;
                model.LeaseRentDateInWord = ndcDetail.LeaseRentDateInWord;
                model.LeaseRentInterestPaidUpto = ndcDetail.LeaseRentPaidUptoDate;
                model.LeaseRentInWord = ndcDetail.LeaseRentStatus;
                model.IsOneTimeLeasePaid = ndcDetail.IsOneTimeLeasePaid;
                model.IsTotalInstallmentPaid = ndcDetail.IsTotalInstallmentPaid;
                model.NDCDate = ndcDetail.NDCDate;
                model.LetterNo = ndcDetail.LetterNo;
                model.LetterDateInWord = ndcDetail.LetterDateInWord;
                model.ChalanAmount = ndcDetail.ChallanAmount;
                model.BankName = ndcDetail.BankName;
                if (ndcDetail.BankName != null)
                {
                    model.BankId = dbContext.BankMsts.FirstOrDefault(m => m.bankName == ndcDetail.BankName && m.IsActive == true).bankId;
                }
                model.Remarks = ndcDetail.Remarks;
                model.TemplateContent = ndcDetail.NDCTemplate;
                model.CreatedDate = ndcDetail.CreatedDate;
                model.PropertyNo = ndcDetail.Sector + "/" + ndcDetail.Block + "-" + ndcDetail.PlotNo;
                model.Address = ndcDetail.ApplicantAddress;
                model.ChallanId = ndcDetail.ChallanDetail;
                model.ActionType = ndcDetail.LetterType;
                model.IsLetterIssued = ndcDetail.IsLetterIssued;
                model.OnlineReqNo = ndcDetail.ServiceRequestId;
                model.ApproverId = ndcDetail.ApproverId;
                model.RequesterId = ndcDetail.ValidatorId;
                model.RequestedDate = ndcDetail.ValidatedDate;
                model.ApprovalDate = ndcDetail.ApprovalDate;
                model.Comment = ndcDetail.Comment;
                model.StatusId = ndcDetail.StatusId;
            }
            return model;
        }


        public int UpdateNDCLetterDetails(NDCVeiwModel model)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var todaysNDC = dbContext.NDCDetailMsts.FirstOrDefault(m => m.Id == model.Id);
                if (todaysNDC != null)
                {
                    todaysNDC.RegistrationId = Convert.ToInt32(model.RegistrationId);
                    todaysNDC.DepartmentId = model.DepartmentId;
                    todaysNDC.Department = model.ActionType != "Hindi" ? model.Department : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == model.DepartmentId).departmentName;
                    todaysNDC.Sector = model.Sector;
                    todaysNDC.Block = model.Block;
                    todaysNDC.PlotNo = model.PlotNo;
                    todaysNDC.Applicant = model.Applicant;
                    todaysNDC.InstallmentDuesAmount = model.InstallmentPaidAmount;
                    todaysNDC.InstallmentDateInWord = model.InstallmentDateInWord;
                    todaysNDC.InstallmentStatus = model.InstallmentInWord;
                    todaysNDC.InterestDuesAmount = model.InstallmentInterestPaidAmount;
                    todaysNDC.InterestDateInWord = model.InterestDateInWord;
                    todaysNDC.InterestDueStatus = model.InterestInWord;
                    todaysNDC.InstallmentPaidUptoDate = model.InstallmentPaidUpto;
                    todaysNDC.LeaseRentAmount = model.LeaseRentPaidAmount;
                    todaysNDC.LeaseRentDateInWord = model.LeaseRentDateInWord;
                    todaysNDC.LeaseRentStatus = model.LeaseRentInWord;
                    todaysNDC.LeaseRentPaidUptoDate = model.LeaseRentPaidUpto;
                    todaysNDC.ChallanDetail = model.ChallanId;
                    todaysNDC.ChallanAmount = model.ChalanAmount;
                    todaysNDC.LetterNo = model.LetterNo;
                    todaysNDC.BankName = model.BankName;
                    todaysNDC.IsTotalInstallmentPaid = model.IsTotalInstallmentPaid;
                    todaysNDC.IsOneTimeLeasePaid = model.IsOneTimeLeasePaid;
                    todaysNDC.NDCDate = model.NDCDate;
                    todaysNDC.Remarks = model.Remarks;
                    todaysNDC.LetterType = model.ActionType;
                    todaysNDC.ApplicantAddress = model.Address;
                    dbContext.SaveChanges();
                    flag = ReturnType.Updated;
                }
            }
            return flag;
        }

        #region NIC Nivesh Mitra Requests

        public DataSourceResult GetCustomerServiceRequestList_NIC(DataSourceRequest request, ServiceVM model)
        {
            //For sorting Kendo DataSourceResult
            if (request.Sorts.Count == 0)
            {
                request.Sorts.Add(new SortDescriptor("Id", System.ComponentModel.ListSortDirection.Descending));
            }
            int userid = userInfo.UserID;
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && csm.Status == 1 && csr.RequestThrough == Constants.NIC_NiveshMitra
                            && (model.Id == null || csr.Id == model.Id)
                            && (model.ServiceId == null || csr.ServiceId == model.ServiceId)
                            && (model.ServiceStatusId == null || csr.Request_Status == model.ServiceStatusId)
                            && (model.DepartmentId == null || csr.DepartmentId == model.DepartmentId)
                            && (model.StartDate == null || DbFunctions.TruncateTime(csr.Created_Date) >= DbFunctions.TruncateTime(model.StartDate))
                            && (model.EndDate == null || DbFunctions.TruncateTime(csr.Created_Date) <= DbFunctions.TruncateTime(model.EndDate))
                            select new ServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                                RequestDate = csr.Created_Date,
                                CreatedDate = csr.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = csr.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = csr.ServiceType,
                                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                                Amount = csr.DuesAmount,
                                StatusId = sts.Id,
                                Status = sts.Status,
                                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                PaymentStatus = csr.PaymentStatus
                            });

                return list.ToDataSourceResult(request);
            }
        }

        public int UpdateCustomerServiceRequestStatus_NIC(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            int flag = ReturnType.None;

            using (var dbContext = new NoidaPMSEntities())
            {
                var service = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == model.Id);
                if (service.Request_Status == NAStatusId.Completed)
                {
                    flag = ReturnType.Updated;
                }
                else if (service.Request_Status == NAStatusId.Forwarded || service.Request_Status == NAStatusId.Pending)
                {
                    if (model.StatusId == NAStatusId.Forwarded)
                    {
                        flag = ReturnType.Forwarded;
                    }
                    else if (model.StatusId == NAStatusId.Pending && service.Request_Status == NAStatusId.Pending)
                    {
                        flag = ReturnType.Exist;
                    }
                    else if (model.StatusId == NAStatusId.Pending && service.Request_Status == NAStatusId.Forwarded)
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        flag = ReturnType.Pending;
                        dbContext.SaveChanges();
                    }
                    else if (model.StatusId == NAStatusId.Objection)
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = model.Comment + "-" + userInfo.FirstName + " " + userInfo.LastName + " " + " Status: Objection " + " " + " Date:" + DateTime.Now + ".";
                        service.ValidatorId = userInfo.UserID;
                        service.ValidatedDate = DateTime.Now;
                        service.ApproverId = model.ApproverId;
                        dbContext.SaveChanges();
                        flag = ReturnType.Validated;
                    }
                    else
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.MiddleName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        service.ApproverId = userInfo.UserID;
                        service.ApprovalDate = DateTime.Now;
                        if (model.StatusId == NAStatusId.Cancelled)
                        {
                            service.IsActive = false;
                        }
                        dbContext.SaveChanges();

                        if (service.Request_Status == NAStatusId.Completed)
                        {
                            if (files != null && files.Count() > 0)
                            {
                                FtpHandler.UploadFiles_NIC(files, model.Id.ToString());
                                if (!string.IsNullOrEmpty(files.FirstOrDefault().FileName))
                                {
                                    var _fileName = "http://doc.mynoida.in/UploadDocuments/NiveshMitraUpload/" + model.RequestId + "/" + model.Id.ToString() + ".pdf";
                                    service.DispatchDocumentName = _fileName;
                                    dbContext.SaveChanges();
                                }
                            }
                            flag = ReturnType.Completed;
                        }
                        else flag = ReturnType.Cancelled;
                        string reqName = dbContext.StatusMasters.Where(m => m.Id == service.Request_Status).FirstOrDefault().Status;

                        string message = string.Empty;
                        message = string.Format(NAMessages.SDServiceReqStatusChange, model.Id, reqName);
                        if (!string.IsNullOrEmpty(service.MobileNumber)) { ApplicationHelper.SendSMS(service.MobileNumber, message); }
                        if (!string.IsNullOrEmpty(service.Email)) { ApplicationHelper.SendEmail(service.Email, "Online Request", message); }
                    }
                }
                else if (service.Request_Status == NAStatusId.Initiated)//for forward 
                {
                    if (model.StatusId == NAStatusId.Forwarded)
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = model.Comment + "-" + userInfo.FirstName + " " + userInfo.LastName;
                        service.ValidatorId = userInfo.UserID;
                        service.ValidatedDate = DateTime.Now;
                        service.ApproverId = model.ApproverId;
                        dbContext.SaveChanges();
                        flag = ReturnType.Validated;
                    }
                    else flag = ReturnType.Initiated;
                }
            }
            return flag;
        }

        public ServiceRequestViewModel GetServiceRequestDetailForCustomer_NIC(int requestId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                ServiceRequestViewModel model = new ServiceRequestViewModel();

                var service = (from csr in dbContext.Customer_ServiceRequest
                               where csr.Id == requestId
                               select new ServiceViewModel
                               {
                                   Id = csr.Id,
                                   RequestId = csr.Id,
                                   RegistrationNo = csr.Registration_No,
                                   Applicant = csr.ApplicantName,
                                   ApplicantAddress = csr.ApplicantAddress,
                                   Requestor = csr.RequestorName,
                                   RequestorAddress = csr.RequestorAddress,
                                   PropertyNo = csr.Property_No,
                                   MobileNo = csr.MobileNumber,
                                   Email = csr.Email,
                                   ServiceId = csr.ServiceId,
                                   ServiceName = dbContext.CitizenService_Master.FirstOrDefault(x => x.Deptt_Id == csr.DepartmentId && x.service_id == csr.ServiceId && x.Status == 1).ServiceName,
                                   DepartmentId = csr.DepartmentId,
                                   Department = dbContext.DepartmentMsts.FirstOrDefault(x => x.departmentId == csr.DepartmentId && x.IsActive == true).departmentName,
                                   SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                   SubDepartmentId = string.IsNullOrEmpty(csr.SubDepartment) ? 1 : dbContext.SubDepartmentMsts.FirstOrDefault(x => x.departmentId == csr.DepartmentId && x.SubdepartmentName == csr.SubDepartment && x.IsActive == true).SubdepartmentId,
                                   Description = csr.Description,
                                   RegistrationType = csr.ServiceType == Constants.SDService ? "AuthorityDay" : "Registered",
                                   ServiceStatusId = csr.Request_Status,
                                   RequestStatus = dbContext.StatusMasters.FirstOrDefault(m => m.Id == csr.Request_Status && m.IsActive == true).Status,
                                   Comment = csr.Comment,
                                   DispatchedDocument = csr.DispatchDocumentName
                               }).FirstOrDefault();

                if (!string.IsNullOrEmpty(service.RegistrationNo)) service.RegistrationId = Convert.ToInt32(service.RegistrationNo);
                if (!string.IsNullOrEmpty(service.PropertyNo))
                {
                    var property = service.PropertyNo.Split('/');
                    if (property != null)
                    {
                        service.Sector = property[0];
                        var blck = property[1].Split('-');
                        if (blck != null)
                        {
                            service.Block = blck[0];
                            service.PlotNo = blck[1];
                        }
                    }
                }
                model.ServiceModel = service;

                return model;
            }
        }

        #endregion

        public DataSourceResult GetNDCListForApprovalAsDataSource(DataSourceRequest request, NDCVeiwModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<NDCVeiwModel> idList = new List<NDCVeiwModel>();
                idList = (from ndcmst in dbContext.NDCDetailMsts
                          select new NDCVeiwModel
                          {
                              Id = dbContext.NDCDetailMsts.Where(m => m.RegistrationId == ndcmst.RegistrationId).OrderByDescending(c => c.Id).FirstOrDefault().Id
                          }).ToList();
                List<int> intList = idList.Select(k => (int)k.Id).ToList();

                var data = (from ndc in dbContext.NDCDetailMsts
                            where intList.Contains(ndc.Id)
                            && ndc.ApproverId == userInfo.UserID
                            //&& (model.DepartmentId == null || ndc.DepartmentId == model.DepartmentId)
                            //&& (model.StatusId == null || ndc.StatusId == model.StatusId)
                            select new NDCVeiwModel
                            {
                                Id = ndc.Id,
                                RegistrationId = ndc.RegistrationId.ToString(),
                                RegistrationNo = ndc.RegistrationId,
                                Applicant = ndc.Applicant,
                                Sector = ndc.Sector,
                                Block = ndc.Block,
                                PlotNo = ndc.PlotNo,
                                PropertyNo = ndc.Sector + "/" + ndc.Block + "-" + ndc.PlotNo,
                                Department = ndc.Department,
                                NDCDate = ndc.NDCDate,
                                LetterNo = ndc.LetterNo,
                                StatusId = ndc.StatusId,
                                Status = ndc.StatusId != null ? dbContext.StatusMasters.FirstOrDefault(s => s.Id == ndc.StatusId).Status : string.Empty,
                                //TemplateContent = ndc.NDCTemplate,
                                CreatedDate = ndc.CreatedDate,
                                IsActive = ndc.IsActive,
                                Remarks = ndc.Remarks,
                                IsLetterIssued = ndc.IsLetterIssued,
                                ActionType = ndc.LetterType,
                                OnlineReqNo = ndc.ServiceRequestId,
                                RequesterId = ndc.ApproverId,
                                Requester = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == ndc.ValidatorId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == ndc.ValidatorId).LastName,
                                RequestedDate = ndc.ValidatedDate,
                                ApprovalDate = ndc.ApprovalDate,
                                ApproverId = ndc.ApproverId,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == ndc.ApproverId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == ndc.ApproverId).LastName
                            });
                return data.ToDataSourceResult(request);
            }
        }


        public int ForwardServiceRequestInBulkFormat(ServiceViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                foreach (var id in model.RequestIdList)
                {
                    var service = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == id);
                    service.Request_Status = model.StatusId;
                    service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                    service.ValidatorId = userInfo.UserID;
                    service.ValidatedDate = DateTime.Now;
                    service.ApproverId = model.ApproverId;
                    dbContext.SaveChanges();
                    flag = ReturnType.Forwarded;
                }
                return flag;
            }
        }


        public int UploadGeneratedLetterByserviceId(ServiceViewModel model, IEnumerable<HttpPostedFileBase> documentfiles)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var flag = ReturnType.None;
                var service = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == model.RequestId);
                if (service.Request_Status == NAStatusId.Completed)
                {
                    if (documentfiles != null && documentfiles.Count() > 0)
                    {
                        FtpHandler.UploadFiles(documentfiles, model.RegistrationId.ToString(), model.RequestId.ToString() + Constants.ReqComplete);
                        if (!string.IsNullOrEmpty(documentfiles.FirstOrDefault().FileName))
                        {
                            service.IsUploadedLetter = true;
                            var _fileName = "http://doc.mynoida.in/UploadDocuments/" + model.RegistrationId + "/" + model.RequestId + Constants.ReqComplete + "/" + documentfiles.FirstOrDefault().FileName;
                            service.DispatchDate = DateTime.Now;
                            service.UploadedDocumentName = _fileName;//documentfiles.FirstOrDefault().FileName;
                            dbContext.SaveChanges();
                            flag = ReturnType.Saved;
                        }
                    }
                }
                return flag;
            }
        }

        public int UpdateCustomerServiceRequestStatus(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            int flag = ReturnType.None;

            using (var dbContext = new NoidaPMSEntities())
            {
                var service = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == model.Id);
                var status = dbContext.Customer_ServiceStatusTrans.Where(c => c.RequestRefId == model.Id).OrderByDescending(c => c.Id).FirstOrDefault();
                if (service.Request_Status == NAStatusId.Completed)
                {
                    flag = ReturnType.Updated;
                }
                else if (service.Request_Status == NAStatusId.Forwarded || service.Request_Status == NAStatusId.Pending || service.Request_Status == NAStatusId.Objection || service.Request_Status == NAStatusId.Resubmitted || service.Request_Status == NAStatusId.Appointment)
                {
                    if (model.StatusId == NAStatusId.Forwarded && service.Request_Status == NAStatusId.Forwarded)
                    {
                        service.Request_Status = model.StatusId;
                        //service.Comment = model.Comment + "-" + userInfo.FirstName + " " + userInfo.LastName;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        service.ValidatorId = userInfo.UserID;
                        service.ValidatedDate = DateTime.Now;
                        service.ApproverId = model.ApproverId;
                        dbContext.SaveChanges();

                        if (status != null)
                        {
                            var service_Status = new Customer_ServiceStatusTrans();
                            service_Status.RequestRefId = service.Id;
                            service_Status.ValidatorId = userInfo.UserID;
                            service_Status.ValidatedDate = DateTime.Now;
                            service_Status.ApproverId = model.ApproverId;
                            service_Status.CreatedDate = DateTime.Now;
                            service_Status.CreatedBy = userInfo.UserID;
                            service_Status.StatusId = model.StatusId;
                            service_Status.Remarks = model.Comment + "by \n" + userInfo.FirstName + " " + userInfo.LastName + "Date:" + DateTime.Now + ".";
                            dbContext.Customer_ServiceStatusTrans.Add(service_Status);
                            dbContext.SaveChanges();
                        }
                        flag = ReturnType.Validated;
                        //flag = ReturnType.Forwarded;
                    }
                    else if (model.StatusId == NAStatusId.Pending && service.Request_Status == NAStatusId.Pending)
                    {
                        flag = ReturnType.Exist;
                    }
                    else if (model.StatusId == NAStatusId.Pending && service.Request_Status == NAStatusId.Forwarded)
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        dbContext.SaveChanges();
                        flag = ReturnType.Pending;
                    }
                    else if (model.StatusId == NAStatusId.Objection && service.Request_Status == NAStatusId.Objection)
                    {
                        flag = ReturnType.Exist;
                    }
                    else if (model.StatusId == NAStatusId.Objection && (service.Request_Status == NAStatusId.Forwarded||service.Request_Status == NAStatusId.Appointment))
                    {
                        if (service.ObjectionStatus == null)
                        {
                            service.Request_Status = model.StatusId;
                            service.ObjectionStatus = model.StatusId;
                            service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                            dbContext.SaveChanges();
                            if (files != null && files.Count() > 0)
                            {
                                FtpHandler.UploadFiles(files, service.Registration_No, model.Id.ToString() + Constants.ReqObjection);
                                if (!string.IsNullOrEmpty(files.FirstOrDefault().FileName))
                                {
                                    service.DocumentStatus = model.StatusId;
                                    dbContext.SaveChanges();
                                }
                            }
                            //string reqName = dbContext.StatusMasters.Where(m => m.Id == service.Request_Status).FirstOrDefault().Status;
                            if (model.IsDocumentRequired == true)
                            {
                                UpdateCustomersRequiredDocument(model);
                            }
                            if (model.IsChargesRequired == true)
                            {
                                UpdateCustomersServiceChargeRequired(model);
                            }
                            string message = string.Empty;
                            message = string.Format(NAMessages.SDServiceReqStatusChange, model.Id, " held on pending with objection");
                            if (!string.IsNullOrEmpty(service.MobileNumber)) { ApplicationHelper.SendSMS(service.MobileNumber, message); }
                            //if (!string.IsNullOrEmpty(service.MobileNumber)) { ApplicationHelper.SendSMS(service.MobileNumber, message); }
                            if (!string.IsNullOrEmpty(service.Email)) { ApplicationHelper.SendEmail(service.Email, "Online Request", message); }

                            if (status != null)
                            {
                                status.StatusId = model.StatusId;
                                status.ModifiedBy = userInfo.UserID;
                                status.ModifiedDate = DateTime.Now;
                                status.Remarks = status.Remarks + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                                dbContext.SaveChanges();
                            }
                            flag = ReturnType.Pending;
                        }
                        else
                        {
                            flag = ReturnType.NotAvailable;
                        }
                    }
                    else if (model.StatusId == NAStatusId.Forwarded && service.Request_Status == NAStatusId.Resubmitted)
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        service.ValidatorId = userInfo.UserID;
                        service.ValidatedDate = DateTime.Now;
                        service.ApproverId = model.ApproverId;
                        dbContext.SaveChanges();

                        if (status != null)
                        {
                            var service_Status = new Customer_ServiceStatusTrans();
                            service_Status.RequestRefId = service.Id;
                            service_Status.ValidatorId = userInfo.UserID;
                            service_Status.ValidatedDate = DateTime.Now;
                            service_Status.ApproverId = model.ApproverId;
                            service_Status.CreatedDate = DateTime.Now;
                            service_Status.CreatedBy = userInfo.UserID;
                            service_Status.StatusId = model.StatusId;
                            service_Status.Remarks = model.Comment + "by \n" + userInfo.FirstName + " " + userInfo.LastName + "Date:" + DateTime.Now + ".";
                            dbContext.Customer_ServiceStatusTrans.Add(service_Status);
                            dbContext.SaveChanges();
                        }
                        flag = ReturnType.Validated;
                    }
                    else if (model.StatusId == NAStatusId.Appointment && service.Request_Status == NAStatusId.Forwarded)
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        dbContext.SaveChanges();

                        string message = string.Empty;
                        message = string.Format(NAMessages.SDServiceReqStatusChange, model.Id, " held in Appointment State.");
                        if (!string.IsNullOrEmpty(service.MobileNumber)) { ApplicationHelper.SendSMS(service.MobileNumber, message); }
                        if (!string.IsNullOrEmpty(service.Email)) { ApplicationHelper.SendEmail(service.Email, "Online Request", message); }
                        flag = ReturnType.Appointment;

                        if (status != null)
                        {
                            status.StatusId = model.StatusId;
                            status.ModifiedBy = userInfo.UserID;
                            status.ModifiedDate = DateTime.Now;
                            status.Remarks = status.Remarks + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                            dbContext.SaveChanges();
                        }
                    }
                    else if (model.StatusId == NAStatusId.Forwarded && service.Request_Status == NAStatusId.Appointment)
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        service.ValidatorId = userInfo.UserID;
                        service.ValidatedDate = DateTime.Now;
                        service.ApproverId = model.ApproverId;
                        dbContext.SaveChanges();

                        var service_Status = new Customer_ServiceStatusTrans();
                        service_Status.RequestRefId = service.Id;
                        service_Status.ValidatorId = userInfo.UserID;
                        service_Status.ValidatedDate = DateTime.Now;
                        service_Status.ApproverId = model.ApproverId;
                        service_Status.CreatedDate = DateTime.Now;
                        service_Status.CreatedBy = userInfo.UserID;
                        service_Status.StatusId = model.StatusId;
                        service_Status.Remarks = model.Comment + "by \n" + userInfo.FirstName + " " + userInfo.LastName + "Date:" + DateTime.Now + ".";
                        dbContext.Customer_ServiceStatusTrans.Add(service_Status);
                        dbContext.SaveChanges();

                        flag = ReturnType.Validated;
                    }
                    else
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        service.ApproverId = userInfo.UserID;
                        service.ApprovalDate = DateTime.Now;

                        if (status != null)
                        {
                            status.StatusId = model.StatusId;
                            status.ModifiedBy = userInfo.UserID;
                            status.ModifiedDate = DateTime.Now;
                            status.Remarks = status.Remarks + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                            dbContext.SaveChanges();
                        }
                        if (model.StatusId == NAStatusId.Cancelled || model.StatusId == NAStatusId.Rejected)
                        {
                            service.IsActive = false;
                            dbContext.SaveChanges();
                            flag = ReturnType.Rejected;
                        }
                        if (model.StatusId == NAStatusId.Approved || model.StatusId == NAStatusId.Completed)
                        {
                            if (string.IsNullOrEmpty(service.Registration_No) && service.ServiceId != NAService.Query)
                            {
                                flag = ReturnType.NotRegistered;
                            }
                            else
                            {
                                dbContext.SaveChanges();
                                if (service.Request_Status == NAStatusId.Completed)
                                {
                                    //if (files != null && files.Count() > 0)
                                    //{
                                    //    FtpHandler.UploadFiles(files, model.Id.ToString());
                                    //    if (!string.IsNullOrEmpty(files.FirstOrDefault().FileName))
                                    //    {
                                    //        service.DispatchDocumentName = files.FirstOrDefault().FileName;
                                    //        dbContext.SaveChanges();
                                    //    }
                                    //}
                                    flag = ReturnType.Completed;
                                }
                                else flag = ReturnType.Cancelled;
                                string reqName = dbContext.StatusMasters.Where(m => m.Id == service.Request_Status).FirstOrDefault().Status;

                                string message = string.Empty;
                                message = string.Format(NAMessages.SDServiceReqStatusChange, model.Id, reqName);
                                if (!string.IsNullOrEmpty(service.MobileNumber)) { ApplicationHelper.SendSMS(service.MobileNumber, message); }
                                if (!string.IsNullOrEmpty(service.Email)) { ApplicationHelper.SendEmail(service.Email, "Online Request", message); }
                            }
                        }

                    }
                }
                else if (service.Request_Status == NAStatusId.Initiated)//for forward 
                {
                    if (model.StatusId == NAStatusId.Forwarded)
                    {
                        service.Request_Status = model.StatusId;
                        //service.Comment = model.Comment + "-" + userInfo.FirstName + " " + userInfo.LastName;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        service.ValidatorId = userInfo.UserID;
                        service.ValidatedDate = DateTime.Now;
                        service.ApproverId = model.ApproverId;
                        dbContext.SaveChanges();
                        if (status == null)
                        {
                            var service_Status = new Customer_ServiceStatusTrans();
                            service_Status.RequestRefId = service.Id;
                            service_Status.ValidatorId = userInfo.UserID;
                            service_Status.ValidatedDate = DateTime.Now;
                            service_Status.ApproverId = model.ApproverId;
                            service_Status.CreatedDate = DateTime.Now;
                            service_Status.CreatedBy = userInfo.UserID;
                            service_Status.StatusId = model.StatusId;
                            service_Status.Remarks = model.Comment + "by \n" + userInfo.FirstName + " " + userInfo.LastName + "Date:" + DateTime.Now + ".";
                            dbContext.Customer_ServiceStatusTrans.Add(service_Status);
                            dbContext.SaveChanges();
                        }
                        flag = ReturnType.Validated;
                    }
                    else flag = ReturnType.Initiated;
                }
                else if (model.StatusId == NAStatusId.Objection && service.ObjectionStatus == NAStatusId.Objection)
                {
                    flag = ReturnType.NotAvailable;
                }

            }
            return flag;
        }

        private void UpdateCustomersRequiredDocument(ServiceViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<DocumentViewModel> tempdata = (List<DocumentViewModel>)HttpContext.Current.Session["TempDocumentModel"];
                if (tempdata != null && model.IsDocumentRequired == true && tempdata.FirstOrDefault().RegistrationId == model.RegistrationId)
                {
                    var docs = dbContext.Customer_ServiceRequestDocument.Where(c => c.RegistrationId == model.RegistrationId && c.ServiceRequestRefId == model.RequestId).ToList();
                    if (docs != null) dbContext.Customer_ServiceRequestDocument.RemoveRange(docs);

                    List<Customer_ServiceRequestDocument> doclist = new List<Customer_ServiceRequestDocument>();
                    foreach (var data in tempdata)
                    {
                        Customer_ServiceRequestDocument document = new Customer_ServiceRequestDocument();
                        document.ServiceRequestRefId = model.RequestId;
                        document.RegistrationId = model.RegistrationId;
                        document.DepartmentId = model.DepartmentId;
                        document.DocumentRefId = data.DocumentId;
                        document.DocumentName = data.DocumentName;
                        document.RequestorId = userInfo.UserID;
                        document.UploadStatusId = 2;//1-by customer,2-department, 3-resubmitted by customer
                        document.CreatedBy = userInfo.UserID;
                        document.CreatedDate = DateTime.Now;
                        document.StatusId = NAStatusId.Active;
                        document.IsActive = true;
                        doclist.Add(document);
                    }
                    dbContext.Customer_ServiceRequestDocument.AddRange(doclist);
                    dbContext.SaveChanges();
                    HttpContext.Current.Session["TempDocumentModel"] = null;
                }
            }
        }

        private void UpdateCustomersServiceChargeRequired(ServiceViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<ChallanViewModel> tempDataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempChallanModel"];
                if (tempDataList != null && tempDataList.FirstOrDefault().RegistrationId == model.RegistrationId && model.IsChargesRequired == true)
                {
                    var charges = dbContext.Customer_ServiceRequestCharges.Where(c => c.RegistrationId == model.RegistrationId && c.ServiceRequestRefId == model.RequestId).ToList();
                    if(charges != null){
                        dbContext.Customer_ServiceRequestCharges.RemoveRange(charges);
                    }
                    List<Customer_ServiceRequestCharges> chargelist = new List<Customer_ServiceRequestCharges>();
                    foreach (var data in tempDataList)
                    {
                        Customer_ServiceRequestCharges serviceCharge = new Customer_ServiceRequestCharges();
                        serviceCharge.ServiceRequestRefId = model.RequestId;
                        serviceCharge.RegistrationId = model.RegistrationId;
                        serviceCharge.DepartmentId = model.DepartmentId;
                        serviceCharge.AccountHeadId = data.AccountHeadId;
                        serviceCharge.AccountSubHeadId = data.AccountSubHeadId;
                        serviceCharge.Amount = data.Amount;
                        serviceCharge.StatusId = NAStatusId.NotPaid;
                        serviceCharge.IsActive = true;
                        serviceCharge.RequestorId = userInfo.UserID;
                        serviceCharge.CreatedBy = userInfo.UserID;
                        serviceCharge.CreatedDate = DateTime.Now;
                        chargelist.Add(serviceCharge);
                    }
                    dbContext.Customer_ServiceRequestCharges.AddRange(chargelist);
                    dbContext.SaveChanges();
                    HttpContext.Current.Session["TempChallanModel"] = null;
                }
            }
        }

        public string GetUploadedDocumentByServiceId(int Id, int Rid, string ActionType)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var file = string.Empty;
                FtpHandler ftpHandler = new FtpHandler();
                var service = dbContext.Customer_ServiceRequest.FirstOrDefault(f => f.Id == Id);
                if (service != null)
                {
                    if (service.Registration_No != null)
                    {
                        if (ActionType == "Complete")
                        {
                            file = ftpHandler.GetUploadedDocumentByServiceId(Rid, Id);
                            //file = service.UploadedDocumentName;
                        }
                        else if (ActionType == "NIC")
                        {
                            //file = ftpHandler.GetUploadedDocumentByServiceId_NIC(Id,service.DispatchDocumentName);
                            if (service.DispatchDocumentName != null)
                            {
                                file = service.DispatchDocumentName;
                            }
                        }
                        else
                        {
                            file = null;
                        }
                    }
                }
                return file;
            }
        }


        public DataSourceResult GetCustomerServiceRequestListByRid(DataSourceRequest request, int? rId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && csm.Status == 1 && csr.Registration_No == rId.ToString()
                            select new ServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No != null ? csr.Property_No : string.Empty,
                                RequestDate = csr.Created_Date,
                                CreatedDate = csr.Created_Date,
                                Timeline = csm.Timeline,
                                ServiceId = csr.ServiceId,
                                ServiceName = csm.ServiceName,
                                ServiceType = csr.ServiceType,
                                DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
                                Amount = csr.DuesAmount,
                                StatusId = sts.Id,
                                Status = sts.Status,
                                IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
                                IsForwarded = csr.Request_Status == NAStatusId.Forwarded ? true : false,
                                IsCompleted = csr.Request_Status == NAStatusId.Completed ? true : false,
                                IsInitiated = csr.Request_Status == NAStatusId.Initiated ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
                                ApproverId = csr.ApproverId,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                Sector = csr.SectorName,
                                Block = csr.BlockName,
                                PlotNo = csr.PlotNo,
                                IsUploaded = csr.IsUploadedLetter,
                                UploadedDocument = csr.UploadedDocumentName,
                                DispatchedDocument = csr.DispatchDocumentName,
                                RequestThrough = csr.RequestThrough
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public int SaveExportedDocument(string contentType, string base64, string fileName)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int flag = ReturnType.None;
                if (!string.IsNullOrEmpty(contentType) && !string.IsNullOrEmpty(base64) && !string.IsNullOrEmpty(fileName))
                {
                    string[] namearr = fileName.Split('-');
                    int rid = Convert.ToInt32(namearr[0]);
                    int letterId = Convert.ToInt32(namearr[1]);
                    int requestId = Convert.ToInt32(namearr[2]);
                    int departmentId = dbContext.AllotmentMasters.FirstOrDefault(c => c.rid == rid && c.isActive.Value == 1).departmentId.Value;
                    var fileContents = Convert.FromBase64String(base64);//<add key="UploadFilePath" value="~\UploadFiles\" />
                    //string path = ConfigurationManager.AppSettings["LocalUploadFilePath"];//<add key="LocalUploadFilePath" value="D:/TFSProjects/NoidaAuthority/Source/NA.PMS/NA.PMS.Web/UploadFiles/" />

                    ////var fileSavePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + rid + extension);
                    ////if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["LocalUploadFilePath"] + rid)))
                    ////{
                    ////    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["LocalUploadFilePath"] + rid));
                    ////}

                    //if (!Directory.Exists(ConfigurationManager.AppSettings["LocalUploadFilePath"] + rid))
                    //{
                    //    Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalUploadFilePath"] + rid);
                    //}

                    ////System.IO.File.WriteAllBytes(path + rid + "/" + fileName, fileContents);
                    FtpHandler.UploadBinaryFiles(rid.ToString(), requestId.ToString(), fileName, fileContents, "Exported");
                    UpdateExportedDocumentById(contentType,base64,fileName,null,null);
                    flag = ReturnType.Saved;
                }
                return flag;
            }
        }

        private void UpdateExportedDocumentById(string contentType, string base64, string fileName, string path, string flag)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                string[] namearr = fileName.Split('-');
                int rid = Convert.ToInt32(namearr[0]);
                int letterId = Convert.ToInt32(namearr[1]);
                int requestId = Convert.ToInt32(namearr[2]);
                int departmentId = dbContext.AllotmentMasters.FirstOrDefault(c => c.rid == rid && c.isActive.Value == 1).departmentId.Value;

                var doc = new Customer_ServiceRequestDocument();
                doc.RegistrationId = rid;
                doc.DepartmentId = departmentId;
                doc.TemplateId = letterId;
                doc.ServiceRequestRefId = requestId;
                doc.DocumentName = fileName;
                doc.DocumentPath = path;
                doc.IsActive = true;
                doc.StatusId = 1;
                doc.CreatedBy = userInfo.UserID;
                doc.CreatedDate = DateTime.Now;
                dbContext.Customer_ServiceRequestDocument.Add(doc);
                dbContext.SaveChanges();
            }
        }


        public int SaveDocumentTypesInSession(DocumentViewModel model)
        {
            int flag = ReturnType.None;
            List<DocumentViewModel> dataList = (List<DocumentViewModel>)HttpContext.Current.Session["TempDocumentModel"];
            if (model.ActionType == "Add")
            {
                if (dataList == null || dataList.Count == 0)
                {
                    List<DocumentViewModel> modelList = new List<DocumentViewModel>();
                    modelList.Add(model);
                    HttpContext.Current.Session["TempDocumentModel"] = modelList;
                    flag = ReturnType.Saved;
                }
                else
                {
                    int count = 0;
                    foreach (var data in dataList)
                    {
                        if (data.RegistrationId != model.RegistrationId)
                        {
                            dataList = new List<DocumentViewModel>();
                            List<DocumentViewModel> modelList = new List<DocumentViewModel>();
                            modelList.Add(model);
                            HttpContext.Current.Session["TempDocumentModel"] = modelList;
                        }
                        else
                        {
                            count++;
                        }
                    }
                    dataList.Add(model);
                    HttpContext.Current.Session["TempDocumentModel"] = dataList;
                    return flag = ReturnType.Saved;
                }
            }
            else if (model.ActionType == "Remove")
            {
                if (dataList != null || dataList.Count > 0)
                {
                    var rmodel = dataList.Where(c => c.Id == model.Id && c.SerialNo == model.SerialNo).FirstOrDefault();
                    dataList.Remove(rmodel);
                    flag = ReturnType.Removed;
                }
            }
            return flag;
        }

        public DataSourceResult GetSavedDocumentTypeListByIdAsDataSource(DataSourceRequest request, DocumentViewModel model)
        {
            List<DocumentViewModel> tempdata = (List<DocumentViewModel>)HttpContext.Current.Session["TempDocumentModel"];
            if (tempdata != null && tempdata.Count > 0)
            {
                for (int id = 0; id < tempdata.Count; id++) tempdata[id].SerialNo = id + 1;
            }
            return tempdata != null ? tempdata.ToDataSourceResult(request) : null;
        }


        public DataSourceResult GetDigitalSingedLetterHistory(DataSourceRequest request, int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {

                var letterDetails = (from service in dbContext.Customer_ServiceRequest
                                     join csm in dbContext.CitizenService_Master on new { y = service.DepartmentId, x = service.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                                     join temp in dbContext.TemplateMasters on new { y = csm.Deptt_Id, x = csm.TemplateId } equals new { y = temp.departmentId, x = temp.templateId }
                                     where service.Registration_No == rid.ToString() && service.IsUploadedLetter==true && csm.Status==1 && temp.status==1
                                     select new ServiceViewModel
                                     {
                                         Id = service.Id,
                                         DispatchDate = service.DispatchDate,
                                         ServiceName = csm.ServiceName,
                                         //UploadedDocument = service.UploadedDocumentName != "" ? service.UploadedDocumentName : service.DispatchDocumentName,
                                         StatusId=service.Request_Status,
                                         IsUploaded=service.IsUploadedLetter,
                                         RequestThrough=service.RequestThrough,
                                         UploadedDocument=service.UploadedDocumentName,
                                         DispatchedDocument=service.DispatchDocumentName,
                                         TemplateName = temp.templateName
                                     }).ToList();
                int i = 1;
                foreach (var items in letterDetails)
                {
                    items.SerialNo = i;
                    i = i + 1;
                    if (items.RequestThrough == Constants.NIC_NiveshMitra)
                    {
                        items.UploadedDocument = items.DispatchedDocument;
                    }
                    else
                    {
                        items.UploadedDocument = items.UploadedDocument;
                        //FtpHandler ftpHandler = new FtpHandler();
                        //items.UploadedDocument = ftpHandler.GetUploadedDocumentByServiceId((int)rid, (int)items.Id);
                    }
                }
                return letterDetails.ToDataSourceResult(request);
            }
        }
    }
}

