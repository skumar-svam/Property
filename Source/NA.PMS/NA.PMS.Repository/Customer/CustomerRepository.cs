using Dapper;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Model;
using NA.PMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Dal.CustomerContext;
using NA.PMS.Model.Entities;
using NoidaAuthority.PMS.Common;
using System.Web;
using System.IO;
using System.Configuration;

namespace NA.PMS.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        public CustomerRepository()
        {

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
            using (var dbContext = new NoidaPMSEntities())
            {
                int requestNo = SaveCustomerServiceRequestDetail(model);
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
                mutation.Created_By = model.RegistrationId; // userInfo.UserID;
                mutation.Comment = model.ServiceModel.Description;
                dbContext.OnlineTransferMutations.Add(mutation);
                dbContext.SaveChanges();
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

        private int SaveGPARequestService(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int requestNo = SaveCustomerServiceRequestDetail(model);
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
                gpa.Created_By = model.RegistrationId; // userInfo.UserID;
                gpa.Created_Date = DateTime.Now;

                dbContext.OnlineGPAs.Add(gpa);
                dbContext.SaveChanges();

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

        private int SaveExtensionRequestDetails(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int requestNo = SaveCustomerServiceRequestDetail(model);
                model.Id = requestNo;
                OnlineExtensionDetail extension = new OnlineExtensionDetail();
                extension.Rid = model.ServiceModel.RegistrationId;
                extension.RequestNo = requestNo;
                extension.ExtensionDueDate = model.ExtensionModel.ExtensionDueDate;
                extension.ExtensionGivenDate = model.ExtensionModel.ExtensionGivenDate;
                extension.Status = NAStatusId.Initiated;
                extension.IsActive = true;
                extension.CreatedDate = DateTime.Now;
                extension.CreatedBy = model.RegistrationId; // userInfo.UserID;
                extension.Comment = model.ServiceModel.Description;

                dbContext.OnlineExtensionDetails.Add(extension);
                dbContext.SaveChanges();
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

        private int SaveMortgageRequestService(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                int requestNo = SaveCustomerServiceRequestDetail(model);
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
                mortgage.CreatedBy = model.RegistrationId; // userInfo.UserID;
                mortgage.Comment = model.ServiceModel.Description;
                mortgage.CommentDate = DateTime.Now;
                mortgage.StatusId = NAStatusId.Initiated;

                dbContext.OnlineMortgageDetails.Add(mortgage);
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
                            director.CreatedBy = model.RegistrationId; // userInfo.UserID;
                            firmMaster.Add(director);
                        }
                        dbContext.OnlineFirmDirectorMasters.AddRange(firmMaster);
                        //dbContext.OnlineFirmDirectorMasters.Add(dataResult);                    
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
                        flag = ReturnType.Failed;
                    }
                }
                if (model.CICModel.ChangeTypeId == NAService.ChangeInFirmName)
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
                    firm.CreatedBy = model.RegistrationId; // userInfo.UserID;
                    dbContext.OnlineFirmRequestMasters.Add(firm);
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
                if (model.CICModel.ChangeTypeId == NAService.ChangeInProduct)
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
                    firm.CreatedBy = model.RegistrationId; // userInfo.UserID;
                    firm.ChangeType = NAService.ChangeInProduct;

                    dbContext.OnlineFirmRequestMasters.Add(firm);
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
                return flag;
            }
        }

        private int SaveRentRequestService(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int requestNo = SaveCustomerServiceRequestDetail(model);
                //model.ServiceRequestId = requestNo;
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
                rent.CreatedBy = model.RegistrationId; // userInfo.UserID;
                dbContext.OnlineRentPermissionDetails.Add(rent);
                dbContext.SaveChanges();

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

        private int SaveTransferRequestService(ServiceRequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                int requestNo = SaveCustomerServiceRequestDetail(model);
                //model.ServiceRequestId = requestNo;
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
                transfer.Created_By = model.RegistrationId; // userInfo.UserID;
                transfer.Created_Date = DateTime.Now;
                dbContext.OnlineTransferMutations.Add(transfer);
                dbContext.SaveChanges();

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

        private int SaveCustomerServiceRequestDetail(ServiceRequestViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                Customer_ServiceRequest request = new Customer_ServiceRequest();
                request.Registration_No = model.RegistrationId.ToString();
                request.Property_No = model.ServiceModel.Sector + "/" + model.ServiceModel.Block + "-" + model.ServiceModel.PlotNo;
                request.ServiceId = model.ServiceModel.ServiceId;
                //request.ServiceType = model.ServiceModel.RegistrationType == Constants.PradhikaranDiwas ? Constants.SDService : Constants.CustomerService;
                request.ServiceType = Constants.CustomerService;
                request.DepartmentId = model.ServiceModel.DepartmentId;
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
                request.Created_By = model.RegistrationId; // userInfo.UserID;
                dbContext.Customer_ServiceRequest.Add(request);
                dbContext.SaveChanges();

                //string message = string.Format(NAMessages.OnlineServiceRequest, request.Id);
                //ApplicationHelper.SendSMS(request.MobileNumber, message);
                //if (!string.IsNullOrEmpty(request.Email)) ApplicationHelper.SendEmail(request.Email, "Online Request", message);

                //int requestNo = (from req in dbContext.Customer_ServiceRequest where req.Registration_No == model.RegistrationId.ToString() && req.Description == model.Description && req.Request_Status == StatusType.Initiated && req.IsActive == true select req.Id).FirstOrDefault();
                model.ServiceModel.Id = request.Id;
                model.ServiceModel.RequestId = request.Id;
                model.ServiceModel.CreatedDate = DateTime.Now;
                return request.Id;
            }
        }


        public ServiceRequestViewModel GetServiceRequestDetailById(int? id)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                ServiceRequestViewModel model = new ServiceRequestViewModel();

                var service = (from csr in dbContext.Customer_ServiceRequest
                               where csr.Id == id //&& csr.IsActive == true
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
                                   Comment = csr.Comment
                               }).FirstOrDefault();

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
                model.RegistrationId = service.RegistrationNo != null ? Convert.ToInt32(service.RegistrationNo) : 0;
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
                        FirstName = transfer.T_First_Name,
                        MiddleName = transfer.T_Middle_Name,
                        LastName = transfer.T_Middle_Name,
                        Applicant = transfer.T_First_Name + " " + transfer.T_Middle_Name + " " + transfer.T_Last_Name,
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
                    return model;
                }
                else
                {
                    return model;
                }
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
                    return model;
                }
                if (firms != null && firms.ChangeType == NAService.ChangeInFirmName)
                {
                    cic.OldFirmName = firms.OldFirmName;
                    cic.NewFirmName = firms.NewFirmName;

                    model.CICModel = cic;
                    return model;
                }
                if (firms != null && firms.ChangeType == NAService.ChangeInProduct)
                {
                    cic.OldFirmProduct = firms.OldFirmProduct;
                    cic.NewFirmProduct = firms.NewFirmProduct;
                    model.CICModel = cic;
                    return model;
                }
                else
                {
                    return model;
                }
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
                    return model;
                }
                else
                {
                    return model;
                }
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
                    return model;
                }
                else
                {
                    return model;
                }
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
                    return model;
                }
                else
                {
                    return model;
                }
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
                    return model;
                }
                else
                {
                    return model;
                }
            }
        }


        public DataSourceResult GetPaymentReceiptScheduleListById(DataSourceRequest request, PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //string Rid = Convert.ToString(rid);
                //var data = (from payreceipt in dbContext.ViewReceiptMasters
                //            join receipthead in dbContext.RECIEPT_HEAD on payreceipt.RECEIPT_HEAD_ID equals receipthead.RECIEPT_CODE
                //            join receiptsubhead in dbContext.RECEIPT_SUB_HEAD on payreceipt.RECEIPT_SUBHEAD_ID equals receiptsubhead.RECEIPT_SUBHEAD_ID
                //            where payreceipt.RID_NO.Equals(Rid)
                //            select new PaymentViewModel
                //            {
                //                Id = 1,
                //                ReceiptId = payreceipt.RECEIPT_ID,
                //                ReceiptHeadName = receipthead.RECIEPT_HEAD_NAME,
                //                ReceiptSubHeadName = receiptsubhead.RECEIPT_SUB_HEAD1,
                //                ChallanId = payreceipt.CHALLAN_ID,
                //                DepositDate = payreceipt.DEPOSIT_DATE,
                //                PaidAmount = payreceipt.AMOUNT_PAID,
                //                RegistrationId = rid,
                //                RegistrationNo = rid.ToString()
                //            });
                var data = (from receipt in dbContext.RECEIPT_DETAIL_MASTER
                            join recptTrans in dbContext.RECEIPT_AMOUNT_TRANS on receipt.RECEIPT_ID equals recptTrans.RECEIPT_ID
                            join head in dbContext.RECIEPT_HEAD on recptTrans.RECEIPT_HEAD_ID equals head.RECIEPT_CODE
                            join subhead in dbContext.RECEIPT_SUB_HEAD on recptTrans.RECEIPT_SUBHEAD_ID equals subhead.RECEIPT_SUBHEAD_ID
                            where receipt.RID_NO == model.RegistrationId.ToString() && (model.ReceiptId == null || receipt.RECEIPT_ID == model.ReceiptId)
                            select new PaymentViewModel
                            {
                                Id = 1,
                                ReceiptId = receipt.RECEIPT_ID,
                                ReceiptHeadName = head.RECIEPT_HEAD_NAME,
                                ReceiptSubHeadName = subhead.RECEIPT_SUB_HEAD1,
                                ChallanId = receipt.CHALLAN_ID,
                                DepositDate = receipt.DEPOSIT_DATE,
                                EntryDate = recptTrans.ENTRY_DATE,
                                PaidAmount = recptTrans.AMOUNT_PAID,
                                RegistrationId = model.RegistrationId,
                                RegistrationNo = receipt.RID_NO,
                                DepositorName = receipt.DEPOSETER_NAME
                            });
                return data.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetPaymentScheduleDataListById(DataSourceRequest request, int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from payschtans in dbContext.PaymentScheduleTrans
                            where payschtans.Rid == rid && payschtans.IsActive == true
                            select new PaymentViewModel
                            {
                                Id = payschtans.Id,
                                RegistrationId = payschtans.Rid,
                                RegistrationNo = payschtans.Rid.ToString(),
                                InstallmentNo = payschtans.InstallmentNo,
                                InstallmentDueDate = payschtans.InstallmentDueDate,
                                InstallmentAmount = payschtans.InstallmentAmount,
                                PrincipalAmount = payschtans.BalanceAmount,
                                InstallmentInterest = payschtans.InterestAmount,
                                BalanceAmount = payschtans.InstallmentAmount + payschtans.InterestAmount
                            });
                return data.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetPaymentRescheduledListById(DataSourceRequest request, int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from pay in dbContext.PaymentScheduleMasters
                            join payTrans in dbContext.PaymentScheduleTrans on pay.ScheduleId equals payTrans.ScheduleId
                            where pay.Rid == rid && pay.ScheduleType == Constants.reschedule && pay.IsActive == true && payTrans.IsActive == true
                            select new PaymentViewModel
                            {
                                Id = payTrans.Id,
                                RegistrationId = payTrans.Rid,
                                RegistrationNo = payTrans.Rid.ToString(),
                                InstallmentNo = payTrans.InstallmentNo,
                                InstallmentDueDate = payTrans.InstallmentDueDate,
                                InstallmentAmount = payTrans.InstallmentAmount,
                                PrincipalAmount = payTrans.BalanceAmount,
                                InstallmentInterest = payTrans.InterestAmount,
                                BalanceAmount = payTrans.InstallmentAmount + payTrans.InterestAmount
                            });
                return data.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetPaymentLedgerDataListById(DataSourceRequest request, int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var paymentLedger = dbContext.SpAccountLedger(rid).ToList();
                List<PaymentViewModel> lstPay = new List<PaymentViewModel>();
                if (paymentLedger != null)
                {
                    foreach (var item in paymentLedger)
                    {
                        var lstPayLed = new PaymentViewModel
                        {
                            Id = item.Id,
                            RegistrationId = item.Rid,
                            RegistrationNo = item.Rid.ToString(),
                            ReceiptSubHeadName = item.RECEIPT_SUB_HEAD,
                            EntryDate = item.Entry_Date,
                            DebitAmount = item.Debit_Amount,
                            CreditAmount = item.Credit_Amount,
                            BalanceAmount = item.Balance_Amount
                        };
                        lstPay.Add(lstPayLed);
                    }
                }
                //var payment = new PaymentViewModel
                //{
                //    Id = 1,
                //    RegistrationId = 10000013,
                //    RegistrationNo = "10000013",
                //    ReceiptSubHeadName = "Premium",
                //    EntryDate = DateTime.Now,
                //    DebitAmount = 50000,
                //    CreditAmount = 500000,
                //    BalanceAmount = 450000
                //};
                //lstPay.Add(payment);
                return lstPay.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetServiceHistoryDataListById(DataSourceRequest request, ServiceViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from requests in dbContext.Customer_ServiceRequest
                            join service in dbContext.CitizenService_Master on new { x1 = requests.DepartmentId, x2 = requests.ServiceId } equals new { x1 = service.Deptt_Id, x2 = service.service_id }
                            where requests.Registration_No == model.RegistrationId.ToString() && service.Status == Constants.Active
                            && (model.Id == null || requests.Id == model.Id)
                            select new ServiceViewModel
                            {
                                Id = requests.Id,
                                RequestId = requests.Id,
                                RegistrationId = model.RegistrationId,
                                RegistrationNo = requests.Registration_No,
                                ServiceId = requests.ServiceId,
                                ServiceName = service.ServiceName,
                                DepartmentId = requests.DepartmentId,
                                Department = requests.DepartmentId == null ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d=>d.departmentId==requests.DepartmentId).departmentName,
                                SubDepartment = (requests.SubDepartment == "A" || requests.SubDepartment=="Account") ? "Account" : "Property",
                                Requestor = requests.RequestorName,
                                RequestorAddress = requests.RequestorAddress,
                                Description = requests.Description,
                                Comment = requests.Comment,
                                CreatedDate = requests.Created_Date,
                                StatusId = requests.Request_Status,
                                Status = dbContext.StatusMasters.Where(s => s.Id == requests.Request_Status).Select(s => s.Status).FirstOrDefault()
                            });
                return data.ToDataSourceResult(request);
            }
        }

        //save registered customer
        public int RegisterCustomerDetails(NACustomer customer, IEnumerable<HttpPostedFileBase> files)
        {
            var flag = ReturnType.None;
            CustomerMst ctxCustomer = new CustomerMst();
            try
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    if (files != null && files.Count() > 0)
                    {
                        var fileList = files.ToList();
                        customer.CustomerIdFileName = fileList[0] != null ? fileList[0].FileName : string.Empty;
                        customer.AuthorityLetter = fileList[1] != null ? fileList[1].FileName : string.Empty;
                    }
                    
                    //var existingCustomer = dbContext.Users.FirstOrDefault(us => us.PropertyId == customer.RegistrationId);
                    var existingCustomer = dbContext.CustomerMsts.FirstOrDefault(us => us.UserName == customer.RegistrationId.ToString());
                    if (existingCustomer != null)
                    {
                        //creating password
                        //var newPassword = "password123";
                        //existingCustomer.Pasword = newPassword.ToMD5HashForPasswordPIS();

                        existingCustomer.RoleId = 2;
                        existingCustomer.DepartmentId = customer.DepartmentId;
                        existingCustomer.Sector = customer.Sector;
                        existingCustomer.Block = customer.Block;
                        existingCustomer.PlotNo = customer.PlotNo;
                        existingCustomer.ModifiedDate = DateTime.Now;
                        existingCustomer.MobileNo = customer.MobileNo;
                        existingCustomer.Email = customer.Email;
                        existingCustomer.PropertyId = customer.PropertyId.ToString();
                        existingCustomer.FirstName = customer.CustomerName;                        
                        existingCustomer.IdFileName = customer.CustomerIdFileName;
                        existingCustomer.IdFileType = customer.CustomerIdFiletype;
                        existingCustomer.PropertyFileName = customer.AuthorityLetter;
                        existingCustomer.PropertyFileType = customer.AuthorityLetterType;
                        existingCustomer.SecurityQuestion = customer.SecurityQuestion;
                        existingCustomer.SecurityAnswer = customer.SecurityAnswer;
                        existingCustomer.IsActive = true;
                        existingCustomer.StatusId = NAStatusId.Pending;
                        existingCustomer.IsFirstTimeActivated = false;
                    }
                    else
                    {
                        //ctxCustomer.UserId = Guid.NewGuid();
                        ctxCustomer.UserName = customer.RegistrationId.ToString();
                        ctxCustomer.FirstName = customer.CustomerName;
                        ctxCustomer.DepartmentId = customer.DepartmentId;
                        ctxCustomer.PropertyId = customer.PropertyId.ToString();
                        ctxCustomer.RoleId = 2;
                        ctxCustomer.Sector = customer.Sector;
                        ctxCustomer.Block = customer.Block;
                        ctxCustomer.PlotNo = customer.PlotNo;
                        ctxCustomer.MobileNo = customer.MobileNo;
                        ctxCustomer.Email = customer.Email;
                        ctxCustomer.CreatedDate = DateTime.Now;
                        ctxCustomer.CreatedBy = customer.UserName;
                        ctxCustomer.IdFileName = customer.CustomerIdFileName;
                        ctxCustomer.IdFileType = customer.CustomerIdFiletype;
                        ctxCustomer.PropertyFileName = customer.AuthorityLetter;
                        ctxCustomer.PropertyFileType = customer.AuthorityLetterType;
                        ctxCustomer.SecurityQuestion = customer.SecurityQuestion;
                        ctxCustomer.SecurityAnswer = customer.SecurityAnswer;
                        ctxCustomer.IsActive = false;                       
                        ctxCustomer.StatusId = NAStatusId.Initiated;
                        ctxCustomer.IsFirstTimeActivated = false;
                        //creating password
                        var newPassword = "password123";
                        ctxCustomer.Password = newPassword.ToMD5HashForPasswordPIS();

                        dbContext.CustomerMsts.Add(ctxCustomer);

                        var msg1 = NAMessages.PIS_registration_1;
                        if (customer.Email != null)
                        {
                            EmailHelper emailHelper = new EmailHelper();
                            emailHelper.Send(customer.Email, "Registration Completed", msg1 + "Regards, http://mynoida.in");
                        }
                        //Send SMS
                        ApplicationHelper.SendSMS(customer.MobileNo, msg1);
                        //Sending mail to From Address on new Registration, as asked by Vishal Shukla to do so
                        var emailAdd = System.Configuration.ConfigurationManager.AppSettings["SmtpFromAddress"];
                        var msg2 = string.Format(NAMessages.PIS_registration_2, ctxCustomer.PropertyId);
                        if (!string.IsNullOrEmpty(emailAdd))
                        {
                            EmailHelper emailHelper = new EmailHelper();
                            emailHelper.Send(emailAdd, "Registration Completed", msg2 + "<br><br>Regards, http://mynoida.in");
                        }
                    }
                    dbContext.SaveChanges();
                    var dflag = SaveCustomerDocument(customer, files);
                    flag = ReturnType.Saved;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return flag;
        }

        private int SaveCustomerDocument(NACustomer model, IEnumerable<HttpPostedFileBase> files)
        {
            int flag = ReturnType.None;
            if (files != null && files.Count() > 0)
            {
                var directoryPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["FilePath"]).ToString() + model.RegistrationId;
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
                
                foreach (var fyl in files)
                {
                    if (fyl != null)
                    {
                        fyl.SaveAs(HttpContext.Current.Server.MapPath((ConfigurationManager.AppSettings["FilePath"]) + model.RegistrationId).ToString() + "/" + fyl.FileName);
                    }
                }
                flag = ReturnType.Saved;
            }
            return flag;
        }

        public PropertyDetail GetPropertyDetails(DtoPropertyFilter objPropertyFilter, int inumber)
        {
            try
            {
                using (System.Data.IDbConnection connection = GetConnection.GetOpenConnection())
                {
                    return connection.Query<PropertyDetail>("sp_GetPropertyDetailsByRegistrationId", new { RegistrationId = objPropertyFilter.RegistrationId, OperationType = inumber }, commandType: System.Data.CommandType.StoredProcedure).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public DataSourceResult GetTransferHistoryDataListById(DataSourceRequest request, int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from transfer in dbContext.Succ_Mut_Trans
                            where transfer.Rid == rid
                            select new TransferViewModel
                            {
                                Id = transfer.Request_No,
                                RequestNo = transfer.Request_No,
                                RegistrationId = transfer.Rid,
                                Applicant = transfer.Applicant_Name,
                                ApplicantAddress = transfer.Correspondance_Add,
                                Transferee = transfer.T_Gender == "Company" ? transfer.T_Company_Name : transfer.T_First_Name + " " + transfer.T_Middle_Name + " " + transfer.T_Last_Name,
                                TransfereeAddress = transfer.T_Correspondence_Add,
                                TransferType = transfer.Type == "M" ? "Mutation" : "Transfer",
                                TransferDate = transfer.Transfer_Date,
                                PropertyNo = null,
                                TransferStatus = dbContext.StatusMasters.Where(s => s.Id == transfer.Status).Select(s => s.Status).FirstOrDefault()
                            });
                return data.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetMortgageHistoryDataListById(DataSourceRequest request, int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from mortgage in dbContext.MortgageDetails
                            join aplicant in dbContext.ApplicationDetails on mortgage.RID equals aplicant.registrationId
                            where mortgage.RID == rid
                            select new MortgageViewModel
                            {
                                Id = mortgage.RequestNo,
                                RegistrationId = mortgage.RID,
                                MortgageDate = mortgage.MortgageDate,
                                MortgageType = mortgage.MortgageType == "2" ? "Collateral" : "Normal",
                                BankName = mortgage.BankName,
                                BranchAddress = mortgage.BranchAddress,
                                MortgageStatus = mortgage.StatusMaster.Status,
                                CreatedDate = mortgage.CreatedDate,
                                ApproveDate = mortgage.ApproveDate,
                                ValidUpto = mortgage.ValidUpto,
                                ProcessingFee = mortgage.ProcessingFee,
                                SanctionedAmount = mortgage.SanctionedAmount,
                                Applicant = aplicant.tGender == Constants.Company ? aplicant.T_Company_Name : aplicant.tFirstName + " " + aplicant.tMiddleName + " " + aplicant.tLastName
                            });
                return data.ToDataSourceResult(request);
            }
        }

        public DataSourceResult GetExtensionHistoryDataListById(DataSourceRequest request, int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from extension in dbContext.Extension_Details
                            where extension.Rid == rid
                            select new ExtensionViewModel
                            {
                                Id = extension.Id,
                                RegistrationId = extension.Rid,
                                CompletionDueDate = extension.Completion_DueDate,
                                ExtensionDueDate = extension.Extension_Due_Date,
                                ExtensionGivenDate = extension.Extension_Given_Date,
                                ExtensionCharge = extension.Extension_Charge,
                                ApprovedDate = extension.Approved_Date,
                                Comment = null,
                                ExtensionStatus = dbContext.StatusMasters.Where(s => s.Id == extension.Status).Select(s => s.Status).FirstOrDefault()
                            });
                return data.ToDataSourceResult(request);
            }
        }

        public IEnumerable<DtoLegalHistory> GetLegalHistoryByRegistrationId(DtoPropertyFilter objPropertyFilter)
        {
            try
            {
                using (System.Data.IDbConnection connection = GetConnection.GetOpenConnection())
                {
                    return connection.Query<DtoLegalHistory>("sp_GetLegalHistoryByRegistrationId", new { RegistrationId = objPropertyFilter.RegistrationId }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public IEnumerable<DtoJalDetailsPaymentHistory> GetJalDetailsPaymentHistoryByRegistrationId(DtoPropertyFilter objPropertyFilter)
        {
            try
            {
                using (System.Data.IDbConnection connection = GetConnection.GetOpenConnection())
                {
                    return connection.Query<DtoJalDetailsPaymentHistory>("sp_GetJalDetailsHistoryByRegistrationId", new { RegistrationId = objPropertyFilter.RegistrationId }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                }

            }
            catch (Exception ex)
            {
                return null;
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
                                    + "<div class='col-md-3 col-lbl'><label>Serial No</label></div>"
                                    + "<div class='col-md-6 col-lbl'><label>Required File</label></div>"
                                    + "<div class='col-md-3 col-lbl-vl'><label>Select File</label></div>"
                                + "</div>";

                    int docCounter = 1;
                    foreach (var item in checklists)
                    {
                        divMain = divMain + "<div class='row  row-border row-file'> "
                                               + "<div class='col-md-3 col-lbl'><label>" + docCounter + "</label></div>"
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


        public PropertyDetailViewModel GetPropertyDetailByRegistrationId(int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from alotment in dbContext.AllotmentMasters
                            join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where alotment.rid == rid
                            select new PropertyDetailViewModel
                            {
                                Id = alotment.rid,
                                RegistrationId = aplicant.registrationId,
                                SchemeId = alotment.schemeId,
                                SchemeName = alotment.SchemeMst.schemeName,
                                DepartmentId = alotment.departmentId,
                                Department = alotment.DepartmentMst.departmentName,
                                SectorId = property.sectorId,
                                Sector = property.SectorMst.sectorName,
                                BlockId = property.blockId,
                                Block = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                Applicant = aplicant.tGender == Constants.Company ? aplicant.T_Company_Name : aplicant.tFirstName + " " + aplicant.tMiddleName + " " + aplicant.tLastName,
                                ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                ApplicantAddress = aplicant.tCorrespondanceAdd,
                                MobileNo = aplicant.tMobileNumber,
                                Email = aplicant.tEmail
                            }).FirstOrDefault();
                return data;
            }
        }


        public ServiceRequestViewModel GetPropertyDetailForServiceRequestById(int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                ServiceRequestViewModel model = new ServiceRequestViewModel();
                var data = (from alotment in dbContext.AllotmentMasters
                            join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                            join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                            where alotment.rid == rid
                            select new ServiceViewModel
                            {
                                Id = alotment.rid,
                                RegistrationId = aplicant.registrationId,
                                SchemeId = alotment.schemeId,
                                SchemeName = alotment.SchemeMst.schemeName,
                                DepartmentId = alotment.departmentId,
                                Department = alotment.DepartmentMst.departmentName,
                                SectorId = property.sectorId,
                                Sector = property.SectorMst.sectorName,
                                BlockId = property.blockId,
                                Block = property.BlockMst.blockName,
                                PlotNo = property.propertyNo,
                                PropertyNo = property.SectorMst.sectorName + "/" + property.BlockMst.blockName + "-" + property.propertyNo,
                                Applicant = aplicant.tGender == Constants.Company ? aplicant.T_Company_Name : aplicant.tFirstName + " " + aplicant.tMiddleName + " " + aplicant.tLastName,
                                ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                ApplicantAddress = aplicant.tCorrespondanceAdd,
                                MobileNo = aplicant.tMobileNumber,
                                Email = aplicant.tEmail
                            }).FirstOrDefault();

                model.RegistrationId = rid;
                model.ServiceModel = data;
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

        public int SaveDirectorOrShareholders(string directorName, decimal? share, string shareType)
        {
            var rflag = ReturnType.None;
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
                    rflag = ReturnType.Updated;
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
                    rflag = ReturnType.Saved;
                }
                return rflag;
            }
            catch (Exception e)
            {
                return rflag = ReturnType.Failed;
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
                    directors.RemoveAt(id);
                    flag = ReturnType.Success;
                }
                else
                    flag = ReturnType.NotExist;

                return flag;
            }
            catch (Exception e)
            {
                return flag = ReturnType.Failed;
            }
        }

        #region Login Repo Methods
        /// <summary>
        /// all details of logged user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserViewModel GetLoginUserDetails(string userName)
        {          
            using (var dbContext = new NoidaPMSEntities())
            {
                var data = (from user in dbContext.CustomerMsts
                            where user.UserName == userName
                            select new UserViewModel
                            {
                                //UserId = user.UserId,
                                Id = user.Id,
                                RegistrationId = user.RegistrationId,
                                PropertyId = user.PropertyId,
                                UserName = user.UserName,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Applicant = user.FirstName + (string.IsNullOrEmpty(user.LastName) ? string.Empty : " " + user.LastName),
                                Sector = user.Sector,
                                Block = string.IsNullOrEmpty(user.Block) ? "NA" : user.Block,
                                PlotNo = user.PlotNo,
                                RoleId = user.RoleId,
                                RoleName = user.RoleId == null ? string.Empty : dbContext.UmRoleMasters.FirstOrDefault(r => r.RoleId == user.RoleId).RoleName,
                                MobileNo = user.MobileNo,
                                Email = user.Email,
                                DepartmentId = user.DepartmentId,
                                Department = (user.DepartmentId == null || user.DepartmentId <= 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(r => r.departmentId == user.DepartmentId).departmentName,
                                CustomerIdFileType = user.IdFileType,
                                CustomerIdFileName = user.IdFileName,
                                AuthorityLetterType = user.PropertyFileType,
                                AuthorityLetter = user.PropertyFileName,
                                StatusId = user.StatusId,
                                IsActive = (user.IsActive == null || user.IsActive == false) ? false : true,
                                IsLocked = (user.IsLocked == null || user.IsLocked == false) ? false : true,
                                IsRejected = (user.StatusId == NAStatusId.Rejected) ? false : true,
                                IsFirstTimeActivated = (user.IsFirstTimeActivated == null || user.IsFirstTimeActivated == false) ? false : true,
                                Remarks = user.Remarks,
                                SecurityQuestion = user.SecurityQuestion,
                                SecurityAnswer = user.SecurityAnswer,
                            }).FirstOrDefault();
                return data;
            }
        }

        /// <summary>
        /// user is locked after trying 5 times 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>

        public bool LockUser(string userName)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.CustomerMsts.FirstOrDefault(c => c.UserName == userName);
                user.IsLocked = false;
                dbContext.SaveChanges();
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// validate user name and password at login time
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>

        public bool ValidateUser(string userName, string password)
        {
            var flag = false;
            var encryptedPassword = password.ToMD5HashForPasswordPIS();
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.CustomerMsts.FirstOrDefault(c => c.UserName == userName && c.Password == encryptedPassword);
                flag = user != null ? true : false;
            }
            return flag;
        }

        /// <summary>
        /// change password for logged user verify email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool ChangePassword(string email, string newPassword)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.CustomerMsts.FirstOrDefault(c => c.UserName == email);
                user.Password = newPassword.ToMD5HashForPasswordPIS();
                user.ModifiedDate = DateTime.Now;
                //user.ModifiedBy = "1";// userInfo.UserID.ToString();
                dbContext.SaveChanges();
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// change password for logged user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>

        public bool ChangePassword(string userName, string email, string newPassword)
        {
            var flag = false;
            using (var dbContext = new NoidaPMSEntities())
            {
                var user = dbContext.CustomerMsts.FirstOrDefault(c => c.UserName == userName && c.Email == email);
                user.Password = newPassword.ToMD5HashForPasswordPIS();
                user.ModifiedDate = DateTime.Now;
                //user.ModifiedBy = "1";// userInfo.UserID.ToString();
                dbContext.SaveChanges();
                if (user.MobileNo != null)
                {
                    var msg = string.Format(NAMessages.PasswordChange, newPassword);
                    ApplicationHelper.SendSMS(msg, newPassword);
                    //SendSMS(user.MobileNo, msg);
                }
                if (user.Email != null)
                {
                    var body = "Dear User,<br><br>You have successfully changed your password. Your new password is " + newPassword + ". </br></br>Regards, </br>http://mynoida.in";
                    //EmailHelper emailHelper = new EmailHelper();
                    //emailHelper.Send(user.Email, "You have successfully changed your password.", body);
                    ApplicationHelper.SendEmail(user.Email, "You have successfully changed your password.", body);
                }
                flag = true;
            }
            
            return flag;
        }

        public int GetRoleIDForUser(string username)
        {
            int roleID = 0;
            if (string.IsNullOrEmpty(username))
            {
                return 0;
            }
            try
            {

                using (var dbContext = new CustomerContext())
                {
                    User user = null;

                    var role = (from users in dbContext.Users
                                join selRole in dbContext.Roles on users.RoleId equals selRole.RoleId
                                where users.UserName == username
                                select selRole.RoleId).FirstOrDefault();

                    roleID = role;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return roleID;
        }

        public NA.PMS.Model.Entities.Role GetRoleForUser(string userName)
        {
            return GetRoleById(GetRoleIDForUser(userName));
        }
        public NA.PMS.Model.Entities.Role GetRoleById(int id)
        {
            var result = new NA.PMS.Model.Entities.Role();
            try
            {

                using (var dbContext = new CustomerContext())
                {

                    var role = (from NA.PMS.Model.Entities.Role rl in dbContext.Roles.ToList()
                                where rl.RoleId == id
                                select new NA.PMS.Model.Entities.Role
                                {
                                    RoleName = rl.RoleName,
                                    RoleId = rl.RoleId,
                                    CreatedBy = rl.CreatedBy,
                                    CreatedOn = rl.CreatedOn,
                                    ModifiedBy = rl.ModifiedBy,
                                    ModifiedOn = rl.ModifiedOn,
                                }).FirstOrDefault();

                    result = role;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public IList<DtoList> LookupPropertyType()
        {
            using (System.Data.IDbConnection connection = GetConnection.GetOpenConnection())
            {
                return
                    connection.Query<DtoList>("dbo.sp_GetLookupPropertyType", new { },
                        commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
        }
        #endregion

        public UserViewModel GetCustomerDetailById(int id)
        {
            using (var dbContext = new CustomerContext())
            {
                var data = (from user in dbContext.Users
                            where user.UserName == id.ToString()
                            select new UserViewModel
                            {
                                UserId = user.UserId,
                                UserName = user.UserName,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Applicant = user.FirstName + (string.IsNullOrEmpty(user.LastName) ? string.Empty : " " + user.LastName),
                                Sector = user.Sector,
                                Block = user.Block,
                                PlotNo = user.Property,
                                RoleId = user.RoleId,
                                RoleName = user.RoleId == null ? string.Empty : dbContext.Roles.FirstOrDefault(r => r.RoleId == user.RoleId).RoleName,
                                PropertyId = user.PropertyId,
                                MobileNo = user.MobileNo,
                                Email = user.UserEmail,
                                DepartmentId = user.DeptId,
                                Department = (user.DeptId == null || user.DeptId <= 0) ? string.Empty : dbContext.LookupDepartments.FirstOrDefault(r => r.Id == user.DeptId).NAME,
                                CustomerIdFileType = user.CustomerIdFileType,
                                AuthorityLetterType = user.CustomerLetterType,
                                Status = user.Status,
                                IsActive = user.Status,
                                IsLocked = user.IsLockedOut,
                                IsRejected = user.IsRejected,
                                IsFirstTimeActivated = user.IsFirstTimeActivated,
                                Remarks = user.Remarks,
                                SecurityQuestion = user.Question,
                                SecurityAnswer = user.Answer,
                            }).FirstOrDefault();
                return data;
            }
        }


        public DataSourceResult GetJalPaymentDataList(DataSourceRequest request, JalViewModel model)
        {
            return null;
        }

        public DataSourceResult GetLitigationDataList(DataSourceRequest request, LitigationViewModel model)
        {
            return null;
        }


        public CustomerDetailViewModel GetCustomerDetails(int rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                CustomerDetailViewModel model = new CustomerDetailViewModel();
                var customer = (from alotment in dbContext.AllotmentMasters
                                join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                                where alotment.rid == rid
                                select new CustomerViewModel
                                {
                                    Id = alotment.rid,
                                    RegistrationId = alotment.rid,
                                    PropertyId = alotment.propertyId,
                                    DepartmentId = alotment.departmentId,
                                    Department = alotment.DepartmentMst.departmentName,
                                    Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName,
                                    ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                    MotherName = aplicant.tMotherName,
                                    ApplicantType = aplicant.tGender,
                                    CorrespondAddress = aplicant.tCorrespondanceAdd,
                                    PermanentAddress = aplicant.tPermanentAdd,
                                    MobileNo = aplicant.tMobileNumber,
                                    Email = aplicant.tEmail,
                                    PanNo = aplicant.tPan,
                                    DateOfBirth = aplicant.tDateOfBirth,
                                    MaritalStatus = aplicant.tMarritalStatus,
                                    Religion = aplicant.religionId == null ? string.Empty : dbContext.ReligionMsts.FirstOrDefault(r => r.religionId == aplicant.religionId).religion,
                                    Category = aplicant.quotaId == null ? string.Empty : aplicant.QuotaMst.quotaName,
                                    Occupation = aplicant.tOccupationId == null ? string.Empty : dbContext.OccupationMsts.FirstOrDefault(o => o.occupationId == aplicant.tOccupationId).occupation,
                                    AnnualIncome = aplicant.tAnnualIncome
                                }).FirstOrDefault();
                model.CustomerModel = customer;
                model.PropertyModel = GetCustomerPropertyDetail(customer.RegistrationId);
                return model;
            }
        }

        private PropertyViewModel GetCustomerPropertyDetail(int? rid)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var details = (from alotment in dbContext.AllotmentMasters
                               join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                               where alotment.rid == rid
                               select new PropertyViewModel
                               {
                                   RegistrationId = alotment.rid,
                                   PropertyId = alotment.propertyId,
                                   SchemeId = alotment.schemeId,
                                   SchemeName = alotment.SchemeMst.schemeName,
                                   DepartmentId = alotment.departmentId,
                                   Department = alotment.DepartmentMst.departmentName,
                                   SectorId = property.sectorId,
                                   SectorName = property.SectorMst.sectorName,
                                   BlockId = property.blockId,
                                   BlockName = property.blockId == null ? "NA" : property.BlockMst.blockName,
                                   PlotNo = property.propertyNo,
                                   FloorAreaId = property.floorId,
                                   FloorArea = property.FloorMst.floorName,
                                   PropertyTypeId = property.propertyTypeId,
                                   PropertyType = property.PropertyTypeMst.propertyTypeName,
                                   LandRate = property.landRatePerSqmt,
                                   ActualArea = property.actualArea,
                                   CoveredArea = property.coveredArea,
                                   TotalArea = property.totalArea,
                                   PropertyCost = property.propertyCost,
                                   TotalPropertyCost = property.totalPropertyCost,
                                   CivilCost = property.civilCost,
                                   AllotmentMoney = property.allotmentMoney,
                                   Registry = property.Registry,
                                   AllotmentDate = alotment.allotmentDate
                               }).FirstOrDefault();
                var possession = dbContext.PossessionDetails.FirstOrDefault(p => p.Rid == details.RegistrationId);
                if (possession != null) details.PossessionDate = possession.PossessionDate;
                var registry = dbContext.RegistryDetails.FirstOrDefault(r => r.Rid == details.RegistrationId);
                if (registry != null) details.RegistryDate = registry.RegistryDoneDate;
                var fuctional = dbContext.FunctionalDetails.FirstOrDefault(f => f.Rid == details.RegistrationId && f.IsActive == true);
                if (fuctional != null) details.FunctionalDate = fuctional.FunctionalDate;
                var completion = dbContext.Completion_Details.FirstOrDefault(c => c.Rid == details.RegistrationId);
                if (completion != null) details.CompletionDate = completion.Completion_Execution_date;
                return details;
            }
        }


        public DataSourceResult GetScannedDocumentListById(DataSourceRequest request, DocumentViewModel model)
        {
            FtpHandler ftp = new FtpHandler();
            string path = ftp.GetDocumentPath(model.RegistrationId, string.Empty, true);//System.Configuration.ConfigurationManager.AppSettings["RootPath"] + "\\" + rid;
            //List<string> oldFiles = ftp.DirSearch(path);
            //foreach (string file in oldFiles)
            //{
            //    string str = file;
            //    if (str.Contains(" "))
            //    {
            //        str = str.Replace(" ", "");
            //        ftp.RenameFiles(file, str, path);
            //    }
            //}
            List<string> allFiles = ftp.DirSearch(path);
            List<DocumentViewModel> documentList = new List<DocumentViewModel>();
            int count = 1;
            foreach (string file in allFiles)
            {
                string str = file;
                string str1 = string.Empty;
                if (file.Contains(" "))
                {
                    str = file.Replace(" ", "");
                }
                if ((str.Split('-')).Length > 1)
                {
                    str1 = str.Substring(0, str.Length - 4);
                    str1 = str1.Substring(9, str1.Length - 9);
                }
                DocumentViewModel document = new DocumentViewModel();
                document.DocumentPath = ftp.GetDocumentPath(model.RegistrationId, file, false);
                document.DocumentName = !(string.IsNullOrEmpty(str1)) ? (!(string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[str1])) ? System.Configuration.ConfigurationManager.AppSettings[str1] : "Other Documents") : "Other Documents";
                document.RegistrationId = model.RegistrationId;
                document.SerialNo = count;
                count++;
                documentList.Add(document);
            }
            return documentList.ToDataSourceResult(request);
        }

        public DataSourceResult GetGeneratedDocumentListById(DataSourceRequest request, DocumentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var list = (from letter in dbContext.Letter_History
                            join template in dbContext.TemplateMasters on new { x = letter.Department_Id, y = letter.Template_Id } equals new { x = template.departmentId, y = template.templateId }
                            where (letter.Template_Html != null && letter.Template_Html != "")
                            && letter.Rid == model.RegistrationId
                            select new DocumentViewModel
                            {
                                Id = letter.Id,
                                RegistrationId = letter.Rid,
                                DepartmentId = letter.Department_Id,
                                Department = letter.DepartmentMst.departmentName,
                                DocumentContent = letter.Template_Html,
                                Template = template.templateName,
                                DocumentName = template.templateName,
                                Barcode = letter.Barcode_Val
                            });
                return list.ToDataSourceResult(request);
            }
        }


        public DataSourceResult GetRegistrationIdListAsDataSource(DataSourceRequest request)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var ridlist = (from alotment in dbContext.AllotmentMasters
                               where alotment.isActive == 1
                               select new DropdownViewModel
                               {
                                   Id = alotment.rid,
                                   Text = alotment.rid.ToString()
                               });
                return ridlist.ToDataSourceResult(request);
            }
        }


        public CustomerViewModel ValidateCustomerRegistration(CustomerViewModel model)
        {
            using (var dbContext = new CustomerContext())
            {
                var exuser = dbContext.Users.FirstOrDefault(u => u.UserName == model.RegistrationId.ToString());
                if (exuser == null)
                {
                    var pimsContext = new NoidaPMSEntities();
                    var data = pimsContext.AllotmentMasters.FirstOrDefault(c => c.rid == model.RegistrationId && c.isActive == Constants.Active);
                    if (data != null)
                    {
                        var customer = (from alotment in pimsContext.AllotmentMasters
                                        join aplicant in pimsContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                                        join property in pimsContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                                        where alotment.rid == model.RegistrationId
                                        select new CustomerViewModel
                                        {
                                            RegistrationId = alotment.rid,
                                            DepartmentId = alotment.departmentId,
                                            Department = alotment.DepartmentMst.departmentName,
                                            PropertyId = alotment.propertyId,
                                            Applicant = aplicant.tGender==Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : (aplicant.tFirstName+" "+(string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName),
                                            MobileNo = aplicant.tMobileNumber,
                                            Email = aplicant.tEmail,
                                            SectorId = property.sectorId,
                                            Sector = property.SectorMst.sectorName,
                                            BlockId = property.blockId,
                                            Block = (property.blockId == null || property.blockId==0)? "NA" : property.BlockMst.blockName,
                                            PlotNo = property.propertyNo
                                        }).FirstOrDefault();
                        return customer;
                    }
                    else
                    {
                        model.IsExist = false;
                        return model;
                    }
                }
                else
                {
                    model.IsActive = false;
                    return model;
                }
            }

        }


        public KYAViewModel GetPropertyDetailForKYA(KYAViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                if (model.BlockId == null)
                {
                    model.BlockId = Constants.NABlockId;
                }
                //var exKYA = dbContext.KYADetails.Where(k => k.SectorId == model.SectorId && k.BlockId == model.BlockId && k.PlotNo == model.PlotNo && (k.StatusId == NAStatusId.Approved || k.StatusId == NAStatusId.Initiated)).FirstOrDefault();
                var exKYA = dbContext.KYADetails.Where(k => k.SectorId == model.SectorId && k.BlockId == model.BlockId && k.PlotNo == model.PlotNo && k.IsActive == true).FirstOrDefault();
                if (exKYA == null)
                {
                    var data = (from alotment in dbContext.AllotmentMasters
                                join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                                join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                                where property.sectorId == model.SectorId
                                && (model.BlockId == null || property.blockId == model.BlockId)
                                && property.propertyNo == model.PlotNo && property.IsActive==true && alotment.isActive==1
                                select new KYAViewModel
                                {
                                    Id = alotment.rid,
                                    RegistrationId = alotment.rid,
                                    SchemeId = alotment.schemeId,
                                    SchemeName = alotment.SchemeMst.schemeName,
                                    DepartmentId = alotment.departmentId,
                                    Department = alotment.DepartmentMst.departmentName,
                                    SectorId = property.sectorId,
                                    Sector = property.SectorMst.sectorName,
                                    BlockId = property.blockId,
                                    Block = property.BlockMst.blockName,
                                    PlotNo = property.propertyNo,
                                    AllotteeType = aplicant.tGender,
                                    Applicant = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : (aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName),
                                    AllotteeName = aplicant.tGender == Constants.Company ? (string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.tFirstName : aplicant.T_Company_Name) : (aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? string.Empty : aplicant.tMiddleName + " ") + aplicant.tLastName),
                                    ApplicantMaster = aplicant.tGender == Constants.Company ? aplicant.tSigningAuthority : aplicant.tFatherHusbandName,
                                    CorrespondAddress = aplicant.tCorrespondanceAdd,
                                    MobileNo = aplicant.tMobileNumber,
                                    Email = aplicant.tEmail
                                }).FirstOrDefault();
                    if (data != null)
                    {
                        return data;
                    }
                    else {
                        model.ActionId = ReturnType.NotExist;
                        return model;
                    }
                }
                else
                {
                    model.ActionId = ReturnType.Exist;
                    return model;
                }
            }
        }

        public int SavePropertyDetailForKYA(KYAViewModel model, IEnumerable<HttpPostedFileBase> files)
        {            
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                var exDetail = dbContext.KYADetails.Where(c => c.SectorId == model.SectorId && c.BlockId == (model.BlockId==null?Constants.NABlockId:model.BlockId) && c.PlotNo == model.PlotNo).FirstOrDefault();
                if (exDetail == null)
                {
                    var detail = new KYADetail();
                    detail.RId = model.RegistrationId;                    
                    detail.DepartmentId = model.DepartmentId;
                    detail.AllotteeName = model.AllotteeName;
                    detail.AllotteeType = model.AllotteeType;
                    detail.SectorId = model.SectorId;
                    detail.BlockId = model.BlockId==null ? Constants.NABlockId :model.BlockId;
                    detail.PlotNo = model.PlotNo;
                    detail.FatherOrHusbandName = model.AllotteeType == Constants.Company ? string.Empty : model.AuthorizedSignatory;
                    detail.AuthorizedSignatory = model.AllotteeType == Constants.Company ? model.AuthorizedSignatory : string.Empty;
                    detail.CountryCode = model.CountryCode;
                    detail.MobileNo = model.MobileNo;
                    detail.Email = model.Email;
                    detail.PhoneNo = model.PhoneNo;
                    detail.GSTNo = model.GSTNo;
                    detail.PAN = model.PAN;
                    detail.AadharNo = model.AadharNo;
                    detail.ROC = model.ROC;
                    detail.CorrespondAddress = model.CorrespondAddress;
                    detail.IdFileType = model.AllotteeIdFileType;
                    detail.PlotOwnershipFileType = model.PlotOwnershipFileType;
                    detail.SubmitDate = DateTime.Now;
                    detail.CreatedDate = DateTime.Now;
                    detail.StatusId = NAStatusId.Initiated;
                    detail.IsActive = true;
                    dbContext.KYADetails.Add(detail);
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;

                    if (files != null && files.Count() > 0)
                    {
                        //var lfiles = files.ToList();
                        //for (int i = 0; i < files.Count(); i++)
                        //{
                        //    FtpHandler.UploadFileByName(lfiles[i], model.AllotteeIdFileType, model.RegistrationId.ToString(), Constants.KYA);
                        //}
                        FtpHandler.UploadKYAFiles(files, model.RegistrationId.ToString(), Constants.KYA);
                        if (model.AllotteeIdFileType != null)
                        {
                            FtpHandler.UploadFileByName(files.First(), model.AllotteeIdFileType, model.RegistrationId.ToString(), Constants.KYA);
                        }
                    }
                }
                else
                {
                    flag = ReturnType.Exist;
                }                
            }
            return flag;
        }


        public int SavePropertyDetailForKYAII(KYAViewModel model, HttpPostedFileBase idfile, HttpPostedFileBase letterfile, HttpPostedFileBase otherfile)
        {           
            int flag = ReturnType.None;
            using (var dbContext = new NoidaPMSEntities())
            {
                //var exDetail = dbContext.KYADetails.Where(c => c.SectorId == model.SectorId && c.BlockId == (model.BlockId == null ? Constants.NABlockId : model.BlockId) && c.PlotNo == model.PlotNo && c.IsActive==true).FirstOrDefault();
                var exDetail = dbContext.KYADetails.Where(c => c.RId==model.RegistrationId && c.IsActive == true).FirstOrDefault();
                if (exDetail == null)
                {
                    var detail = new KYADetail();
                    detail.RId = model.RegistrationId;
                    detail.DepartmentId = model.DepartmentId;
                    detail.AllotteeName = model.AllotteeName;
                    detail.AllotteeType = model.AllotteeType;
                    detail.SectorId = model.SectorId;
                    detail.BlockId = model.BlockId == null ? Constants.NABlockId : model.BlockId;
                    detail.PlotNo = model.PlotNo;
                    detail.FatherOrHusbandName = model.AllotteeType == Constants.Company ? string.Empty : model.AuthorizedSignatory;
                    detail.AuthorizedSignatory = model.AllotteeType == Constants.Company ? model.AuthorizedSignatory : string.Empty;
                    detail.CountryCode = model.CountryCode;
                    detail.MobileNo = model.MobileNo;
                    detail.SignatoryMobileNo = model.SignatoryMobileNo;
                    detail.Email = model.Email;
                    detail.SignatoryEmail = model.SignatoryEmail;
                    detail.PhoneNo = model.PhoneNo;
                    detail.GSTNo = model.GSTNo;
                    detail.PAN = model.PAN;
                    detail.AadharNo = model.AadharNo;
                    detail.ROC = model.ROC;
                    detail.CorrespondAddress = model.CorrespondAddress;
                    detail.AddressLine1 = model.AddressLine1;
                    detail.AddressLine2 = model.AddressLine2;
                    detail.AreaLocality = model.AreaLocality;
                    detail.City = model.City;
                    detail.State = model.State;
                    detail.PinCode = model.PinCode;
                    detail.IdFileType = model.AllotteeIdFileType;
                    detail.PlotOwnershipFileType = model.PlotOwnershipFileType;
                    detail.SubmitDate = DateTime.Now;
                    detail.CreatedDate = DateTime.Now;
                    detail.StatusId = NAStatusId.Initiated;
                    detail.IsActive = true;
                    //detail.KYAReferenceCode = DateTime.Now.Ticks.ToString() + model.RegistrationId.ToString();
                    //detail.KYAReferenceCode = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString()  + model.RegistrationId.ToString();
                    dbContext.KYADetails.Add(detail);
                    dbContext.SaveChanges();

                    detail.KYAReferenceCode = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString()+ DateTime.Now.Day.ToString()+model.RegistrationId.ToString();
                    dbContext.SaveChanges();
                    flag = ReturnType.Saved;
                    model.Id = detail.Id;

                    //int dflag = SaveKYADocument(model, idfile, letterfile, otherfile);
                    int dflag = SaveKYADocumentII(model, idfile, letterfile, otherfile);
                    if (flag == ReturnType.Saved && dflag == ReturnType.Success)
                    {
                        string message = string.Format(NAMessages.KYASubmitted, detail.KYAReferenceCode);
                        if (detail.AllotteeType == "Company")
                        {
                            if (!string.IsNullOrEmpty(detail.SignatoryEmail)) { ApplicationHelper.SendEmail(detail.SignatoryEmail, "KYA Application Form Status", message); }
                            if (!string.IsNullOrEmpty(detail.SignatoryMobileNo)) { ApplicationHelper.SendSMS(detail.SignatoryMobileNo, message); }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(detail.Email)) { ApplicationHelper.SendEmail(detail.Email, "KYA Application Form Status", message); }
                            if (!string.IsNullOrEmpty(detail.MobileNo)) { ApplicationHelper.SendSMS(detail.MobileNo, message); }
                        }
                    }
                }
                else
                {
                    flag = ReturnType.Exist;
                }
            }
            return flag;
        }

        private int SaveKYADocument(KYAViewModel model, HttpPostedFileBase idfile, HttpPostedFileBase letterfile, HttpPostedFileBase otherfile)
        {
            int flag = ReturnType.None; bool bflag = false;
            //if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id)))
            //{
            //    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDetailsPath"] + "/" + model.Id));
            //}

            if (idfile != null)
            {
                string extension = Path.GetExtension(idfile.FileName);
                //string filename = model.AllotteeIdFileType + extension;
                string filename = "idfile" + extension;
                bflag = FtpHandler.UploadFileByName(idfile, filename, model.RegistrationId.ToString(), Constants.KYA);
                flag = bflag == true ? ReturnType.Success : ReturnType.Failed;
            }

            if (letterfile != null)
            {
                string extension = Path.GetExtension(letterfile.FileName);
                //string filename = model.PlotOwnershipFileType + extension;
                string filename = "letterfile" + extension;
                bflag = FtpHandler.UploadFileByName(letterfile, filename, model.RegistrationId.ToString(), Constants.KYA);
                flag = bflag == true ? ReturnType.Success : ReturnType.Failed;
            }
            if (otherfile != null)
            {
                string extension = Path.GetExtension(otherfile.FileName);
                //string filename = model.OtherFileType + extension;
                string filename = "otherfile" + extension;
                bflag = FtpHandler.UploadFileByName(otherfile, filename, model.RegistrationId.ToString(), Constants.KYA);
                flag = bflag == true ? ReturnType.Success : ReturnType.Failed;
            }

            return flag;
        }

        private int SaveKYADocumentII(KYAViewModel model, HttpPostedFileBase idfile, HttpPostedFileBase letterfile, HttpPostedFileBase otherfile)
        {
            int flag = ReturnType.None; bool bflag = false;

            if (idfile != null)
            {
                string extension = Path.GetExtension(idfile.FileName);
                //string filename = model.AllotteeIdFileType + extension;
                string filename = "idfile" + extension;
                bflag = FtpHandler.UploadFileByName(idfile, filename, model.RegistrationId.ToString(), Constants.KYA,model.Id.ToString());
                flag = bflag == true ? ReturnType.Success : ReturnType.Failed;
            }

            if (letterfile != null)
            {
                string extension = Path.GetExtension(letterfile.FileName);
                //string filename = model.PlotOwnershipFileType + extension;
                string filename = "letterfile" + extension;
                bflag = FtpHandler.UploadFileByName(letterfile, filename, model.RegistrationId.ToString(), Constants.KYA, model.Id.ToString());
                flag = bflag == true ? ReturnType.Success : ReturnType.Failed;
            }
            if (otherfile != null)
            {
                string extension = Path.GetExtension(otherfile.FileName);
                //string filename = model.OtherFileType + extension;
                string filename = "otherfile" + extension;
                bflag = FtpHandler.UploadFileByName(otherfile, filename, model.RegistrationId.ToString(), Constants.KYA, model.Id.ToString());
                flag = bflag == true ? ReturnType.Success : ReturnType.Failed;
            }

            return flag;
        }
    }
}
