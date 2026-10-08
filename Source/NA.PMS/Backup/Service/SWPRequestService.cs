using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.NICService.Resource;
//using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NA.PMS.NICService.Context;

namespace NA.PMS.NICServices
{
    public class SWPRequestService : ISWPRequestService
    {
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        private List<int?> DepartmentList = new List<int?>();
        public SWPRequestService()
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

        public SWPServicesViewModel SaveServiceRequestDetail(SWPServicesViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var flag = SWPReturnTypeId.None;
            switch (model.ServiceModel.ServiceId)
            {
                case SWPService.Transfer:
                    flag = SaveTransferRequestService(model, files);
                    break;
                case SWPService.Rent:
                    flag = SaveRentRequestService(model, files);
                    break;
                case SWPService.CIC:
                    flag = SaveCICRequestService(model, files);
                    break;
                case SWPService.Mortgage:
                    flag = SaveMortgageRequestService(model, files);
                    break;
                case SWPService.Extension:
                    flag = SaveExtensionRequestDetails(model, files);
                    break;
                case SWPService.GPA:
                    flag = SaveGPARequestService(model, files);
                    break;
                case SWPService.Mutation:
                    flag = SaveMutationRequestService(model, files);
                    break;
                default:
                    flag = SaveOtherRequestService(model, files);
                    break;
            }
            model.ServiceModel.RequestId = flag;

            //update NIC table
            if (model.ServiceModel.RequestId > 0)
            {
                var _data = new SWPPostViewModel();
                _data.TxtUnitID = model.SWPPostModel.TxtUnitID;
                _data.TxtControlID = model.SWPPostModel.TxtControlID;
                _data.TxtProcessIndustryID = model.SWPPostModel.TxtProcessIndustryID;
                _data.TxtServiceID = model.SWPPostModel.TxtServiceID;
                _data.TxtApplicationID = Convert.ToString(model.ServiceModel.RequestId);
                SaveNiveshMitraUnit(_data);
            }

            model.Status = flag;
            return model;
        }

        private int SaveOtherRequestService(SWPServicesViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int requestNo = 0;
            if (model != null)
            {
                requestNo = SaveCustomerServiceRequestDetail(model);
                if (requestNo > 0)
                {
                    UploadRequiredFiles(model, files, requestNo);
                }
            }
            return requestNo;
        }

        private void UploadRequiredFiles(SWPServicesViewModel model, IEnumerable<HttpPostedFileBase> files, int requestNo)
        {
            if (files != null)
            {
                if (files.ToList().Count > 0)
                {
                    FTPHandler.UploadFiles(files, model.ServiceModel.RegistrationId.ToString(), requestNo);
                }
            }
        }

        private int SaveCustomerServiceRequestDetail(SWPServicesViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (model.Id != null && model.Id > 0)
                {
                    var exService = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == model.Id);
                    if (exService != null)
                    {
                        exService.Registration_No = model.ServiceModel.RegistrationId.ToString();
                        exService.Property_No = !string.IsNullOrEmpty(model.ServiceModel.PropertyNo) ? model.ServiceModel.PropertyNo : model.ServiceModel.Sector + "/" + model.ServiceModel.Block + "-" + model.ServiceModel.PlotNo;
                        exService.ServiceId = model.ServiceModel.ServiceId;
                        exService.ServiceType = SWPConstant.NIC_NiveshMitra;
                        exService.DepartmentId = model.ServiceModel.DepartmentId;
                        exService.SubDepartment = model.ServiceModel.SubDepartment;
                        exService.ApplicantName = model.ServiceModel.Applicant;
                        exService.MobileNumber = model.ServiceModel.MobileNo;
                        exService.Email = model.ServiceModel.Email;
                        exService.ApplicantAddress = model.ServiceModel.ApplicantAddress;
                        exService.Description = model.ServiceModel.Description;
                        exService.IsActive = true;
                        exService.RequestorName = model.ServiceModel.Requestor ?? model.ServiceModel.Applicant;
                        exService.RequestorAddress = model.ServiceModel.RequestorAddress ?? model.ServiceModel.ApplicantAddress;
                        exService.Modified_Date = DateTime.Now;
                        exService.Modified_By = userInfo.UserID;
                        dbContext.SaveChanges();

                        return exService.Id;
                    }
                    else return (int)model.Id;
                }
                else
                {
                    Customer_ServiceRequest request = new Customer_ServiceRequest
                    {
                        Registration_No = model.ServiceModel.RegistrationId.ToString(),
                        Property_No = model.ServiceModel.Sector + "/" + model.ServiceModel.Block + "-" + model.ServiceModel.PlotNo,
                        ServiceId = model.ServiceModel.ServiceId,
                        ServiceType = SWPConstant.NIC_NiveshMitra,
                        DepartmentId = model.ServiceModel.DepartmentId,
                        SubDepartment = model.ServiceModel.SubDepartment,
                        ApplicantName = model.ServiceModel.Applicant,
                        MobileNumber = model.ServiceModel.MobileNo,
                        Email = model.ServiceModel.Email,
                        ApplicantAddress = model.ServiceModel.ApplicantAddress,
                        Description = model.ServiceModel.Description,
                        IsActive = true,
                        Request_Status = SWPStatusId.Initiated,
                        RequestorName = model.ServiceModel.Requestor ?? model.ServiceModel.Applicant,
                        RequestorAddress = model.ServiceModel.RequestorAddress ?? model.ServiceModel.ApplicantAddress,
                        Created_Date = DateTime.Now,
                        Created_By = userInfo.UserID,
                        RequestThrough = SWPConstant.NIC_NiveshMitra
                    };

                    // in case if service has fee
                    var _CitizenMaster = dbContext.CitizenService_Master.Where(m => m.Status == 1 && m.service_id == model.ServiceModel.ServiceId && m.Deptt_Id == model.ServiceModel.DepartmentId).FirstOrDefault();
                    if (_CitizenMaster != null)
                    {
                        if (_CitizenMaster.Amount > 0)
                        {
                            request.PaymentStatus = 0;// not paid
                        }
                    }

                    dbContext.Customer_ServiceRequest.Add(request);
                    dbContext.SaveChanges();

                    model.Id = request.Id;
                    model.RegistrationId = model.ServiceModel.RegistrationId;
                    model.ServiceModel.Id = request.Id;
                    model.ServiceModel.PropertyNo = request.Property_No;
                    model.ServiceModel.PropertyNo = request.Property_No;
                    model.ServiceModel.CreatedDate = DateTime.Now;

                    string message = string.Format(SWPMessage.OnlineServiceRequest, request.Id);
                    SWPApplication.SendSMS(request.MobileNumber, message);
                    if (!string.IsNullOrEmpty(request.Email)) SWPApplication.SendEmail(request.Email, "Online Request", message);

                    return request.Id;
                }
            }
        }

        private int SaveMutationRequestService(SWPServicesViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int requestNo = 0;
            if (model != null)
            {
                requestNo = SaveCustomerServiceRequestDetail(model);
                if (requestNo > 0)
                {
                    UploadRequiredFiles(model, files, requestNo);
                }
            }
            return requestNo;
        }

        private int SaveGPARequestService(SWPServicesViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int requestNo = 0;
            if (model != null)
            {
                requestNo = SaveCustomerServiceRequestDetail(model);
                if (requestNo > 0)
                {
                    UploadRequiredFiles(model, files, requestNo);
                }
            }
            return requestNo;
        }

        private int SaveExtensionRequestDetails(SWPServicesViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int requestNo = 0;
            if (model != null)
            {
                requestNo = SaveCustomerServiceRequestDetail(model);
                if (requestNo > 0)
                {
                    SaveExtensionRequest(model, requestNo);

                    UploadRequiredFiles(model, files, requestNo);
                }
            }
            return requestNo;
        }

        private void SaveExtensionRequest(SWPServicesViewModel model, int requestNo)
        {
            var flag = SWPReturnTypeId.None;
            using (var dbContext = new PIMSEntitiesContext())
            {
                var exExtension = dbContext.OnlineExtensionDetails.FirstOrDefault(e => e.RequestNo == requestNo);
                if (exExtension != null)
                {
                    exExtension.Rid = model.ServiceModel.RegistrationId;
                    exExtension.RequestNo = requestNo;
                    exExtension.ExtensionDueDate = model.ExtensionModel.ExtensionDueDate;
                    exExtension.ExtensionGivenDate = model.ExtensionModel.ExtensionGivenDate;
                    exExtension.IsActive = true;
                    exExtension.ModifiedDate = DateTime.Now;
                    exExtension.ModifiedBy = userInfo.UserID;
                    exExtension.Comment = model.ServiceModel.Description;
                    dbContext.SaveChanges();
                    flag = SWPReturnTypeId.Success;
                }
                else
                {
                    model.Id = requestNo;
                    OnlineExtensionDetail extension = new OnlineExtensionDetail
                    {
                        Rid = model.ServiceModel.RegistrationId,
                        RequestNo = requestNo,
                        ExtensionDueDate = model.ExtensionModel.ExtensionDueDate,
                        ExtensionGivenDate = model.ExtensionModel.ExtensionGivenDate,
                        Status = SWPStatusId.Initiated,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        CreatedBy = userInfo.UserID,
                        Comment = model.ServiceModel.Description
                    };

                    dbContext.OnlineExtensionDetails.Add(extension);
                    dbContext.SaveChanges();
                    flag = SWPReturnTypeId.Success;
                }
                //return flag;
            }
        }

        private int SaveMortgageRequestService(SWPServicesViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int requestNo = 0;
            if (model != null)
            {
                requestNo = SaveCustomerServiceRequestDetail(model);
                if (requestNo > 0)
                {
                    SaveMortgageRequest(model, requestNo);
                    UploadRequiredFiles(model, files, requestNo);
                }
            }
            return requestNo;
        }

        private int SaveMortgageRequest(SWPServicesViewModel model, int requestNo)
        {
            var flag = SWPReturnTypeId.None;
            using (var dbContext = new PIMSEntitiesContext())
            {
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
                    exMortgage.ModifiedDate = DateTime.Now;
                    exMortgage.Modifiedby = userInfo.UserID;
                    dbContext.SaveChanges();
                    flag = SWPReturnTypeId.Success;
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
                    mortgage.StatusId = SWPStatusId.Initiated;

                    dbContext.OnlineMortgageDetails.Add(mortgage);
                    dbContext.SaveChanges();
                    flag = SWPReturnTypeId.Success;
                }
            }
            return flag;
        }

        private int SaveCICRequestService(SWPServicesViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int requestNo = 0;
            if (model != null)
            {
                requestNo = SaveCustomerServiceRequestDetail(model);
                if (requestNo > 0)
                {
                    UploadRequiredFiles(model, files, requestNo);
                }
            }
            return requestNo;
        }

        private int SaveRentRequestService(SWPServicesViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int requestNo = 0;
            if (model != null)
            {
                requestNo = SaveCustomerServiceRequestDetail(model);
                if (requestNo > 0)
                {
                    SaveRentRequest(model, requestNo);

                    UploadRequiredFiles(model, files, requestNo);
                }
            }
            return requestNo;
        }

        private int SaveRentRequest(SWPServicesViewModel model, int requestNo)
        {
            var flag = SWPReturnTypeId.None;
            using (var dbContext = new PIMSEntitiesContext())
            {
                var exRent = dbContext.OnlineRentPermissionDetails.FirstOrDefault(r => r.RequestNo == requestNo);
                if (exRent != null)
                {
                    exRent.Rid = model.ServiceModel.RegistrationId;
                    exRent.TenantName = model.RentModel.TenantName;
                    exRent.TenantProject = model.RentModel.TenantProject;
                    exRent.RentDurationYears = model.RentModel.RentDuration;
                    exRent.RentingDate = model.RentModel.RentingDate;
                    exRent.IsActive = true;
                    exRent.Comment = exRent.Comment + "</br>" + model.ServiceModel.Description;
                    exRent.CommentDate = DateTime.Now;
                    exRent.ModifiedDate = DateTime.Now;
                    exRent.Modifiedby = userInfo.UserID;
                    dbContext.SaveChanges();
                    flag = SWPReturnTypeId.Success;
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
                    rent.StatusId = SWPStatusId.Initiated;
                    rent.Comment = model.ServiceModel.Description;
                    rent.CommentDate = DateTime.Now;
                    rent.CreatedBy = userInfo.UserID;
                    dbContext.OnlineRentPermissionDetails.Add(rent);
                    dbContext.SaveChanges();
                    flag = SWPReturnTypeId.Success;
                }

                return flag;
            }
        }

        private int SaveTransferRequestService(SWPServicesViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int requestNo = 0;
            if (model != null)
            {
                requestNo = SaveCustomerServiceRequestDetail(model);
                if (requestNo > 0)
                {
                    // Transfer Request
                    SaveTransferRequest(model, requestNo);

                    if (requestNo > 0)
                    {
                        UploadRequiredFiles(model, files, requestNo);
                    }
                }
            }
            return requestNo;
        }

        private int SaveTransferRequest(SWPServicesViewModel model, int requestNo)
        {
            var flag = SWPReturnTypeId.None;
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (model.Id != null && model.Id > 0)
                {
                    var exTransfer = dbContext.OnlineTransferMutations.FirstOrDefault(t => t.RequestNo == requestNo);
                    if (exTransfer != null)
                    {
                        exTransfer.Rid = model.ServiceModel.RegistrationId;
                        exTransfer.Type = "T"; // Constants.Transfer;
                        exTransfer.Transfer_Type = model.TransferModel.TransferTypeId;
                        exTransfer.Transfer_Sub_Type = model.TransferModel.TransferSubTypeId;

                        if (model.TransferModel.TypeOfTransferee == SWPConstant.Company)
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

                        flag = SWPReturnTypeId.Success;
                    }
                    else
                    {
                        model.Id = requestNo;
                        var transfer = new OnlineTransferMutation();
                        transfer.Rid = model.ServiceModel.RegistrationId;
                        transfer.RequestNo = requestNo;
                        transfer.Type = "T"; // Constants.Transfer;
                        transfer.Transfer_Type = model.TransferModel.TransferTypeId;
                        transfer.Transfer_Sub_Type = model.TransferModel.TransferSubTypeId;
                        transfer.Created_Date = DateTime.Now;
                        if (model.TransferModel.TypeOfTransferee == SWPConstant.Company)
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
                        transfer.Applicant_Name = model.ServiceModel.Applicant;
                        transfer.Correspondance_Add = model.ServiceModel.ApplicantAddress;
                        transfer.Created_By = userInfo.UserID;
                        transfer.Created_Date = DateTime.Now;
                        dbContext.OnlineTransferMutations.Add(transfer);
                        dbContext.SaveChanges();

                        flag = SWPReturnTypeId.Success;
                    }
                }
            }
            return flag;
        }


        public SWPApplicantViewModel GetApplicantDetailsByRegistrationId(int? registrationId)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var applicant = (from alotment in dbContext.AllotmentMasters
                                 join department in dbContext.DepartmentMsts on alotment.departmentId equals department.departmentId
                                 join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                                 join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                                 where alotment.rid == registrationId && alotment.isActive == 1
                                 select new SWPApplicantViewModel
                                 {
                                     RegistrationId = alotment.rid,
                                     DepartmentId = alotment.departmentId,
                                     Department = department.departmentName,
                                     SectorId = property.sectorId,
                                     Sector = property.SectorMst.sectorName,
                                     BlockId = property.blockId,
                                     Block = property.BlockMst.blockName,
                                     PlotNo = property.propertyNo,
                                     Applicant = aplicant.tGender == SWPConstant.Company ? (!string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.T_Company_Name : aplicant.tFirstName) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? aplicant.tLastName : aplicant.tMiddleName + " " + aplicant.tLastName),
                                     ApplicantType = aplicant.tGender,
                                     Mobile = aplicant.tMobileNumber,
                                     Email = aplicant.tEmail,
                                     PermanentAddress = aplicant.tPermanentAdd,
                                     CorrespondAddress = aplicant.tCorrespondanceAdd,
                                     IsAllotted = true
                                 }).FirstOrDefault();
                var kya = dbContext.KYADetails.Where(k => k.RId == registrationId).OrderByDescending(o=>o.Id).FirstOrDefault();
                applicant.IsKYADone = (kya != null && kya.StatusId == SWPStatusId.Approved) ? true : false;
                return applicant;
            }
        }

        public string GetFileUploadHtmlForService(int? departmentId, int? serviceId)
        {
            using (var context = new PIMSEntitiesContext())
            {
                var checklists = (from checklist in context.ServiceCheckList_Master
                                  where checklist.dept_id == departmentId && checklist.service_id == serviceId
                                  select new SWPCheckListViewModel
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
                                    + "<div class='col-md-3 col-lbl-vl'><small>only pdf, jpg & jpeg file</small></div>"
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

        public SWPApplicantViewModel ValidateRegistrationId(int? registrationId)
        {
            var _ApplicantVM = new SWPApplicantViewModel();
            using (var dbContext = new PIMSEntitiesContext())
            {
                var _Allotment = dbContext.AllotmentMasters.Where(m => m.rid == registrationId && m.isActive == 1).FirstOrDefault();
                if (_Allotment != null)
                {
                    _ApplicantVM = GetApplicantDetailsByRegistrationId(registrationId);
                }
                else
                {
                    _ApplicantVM.IsAllotted = false;
                    _ApplicantVM.RegistrationId = registrationId;
                }
            }
            return _ApplicantVM;
        }

        public bool SaveNiveshMitraUnit(SWPPostViewModel model)
        {
            var flag = false;
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (!string.IsNullOrEmpty(model.TxtControlID) && !string.IsNullOrEmpty(model.TxtUnitID))
                {
                    var _nsMaster = dbContext.NiveshMitraEntr_Master.FirstOrDefault(c => c.UnitID == model.TxtUnitID && c.ControlID == model.TxtControlID && c.ServiceID == model.TxtServiceID);
                    if (_nsMaster == null)
                    {
                        var _ApplicantVM = new SWPApplicantViewModel();
                        if (!string.IsNullOrEmpty(model.TxtProcessIndustryID))
                        {
                            int rid = Convert.ToInt32(model.TxtProcessIndustryID);
                            _ApplicantVM = GetApplicantDetailsByRegistrationId(rid);
                        }

                        if (_ApplicantVM != null)
                        {
                            var niveshMitramaster = new NiveshMitraEntr_Master();
                            niveshMitramaster.ControlID = model.TxtControlID;
                            niveshMitramaster.UnitID = model.TxtUnitID;
                            niveshMitramaster.ServiceID = model.TxtServiceID;
                            niveshMitramaster.ProcessIndustryID = model.TxtProcessIndustryID;
                            niveshMitramaster.ApplicationID = model.TxtApplicationID;
                            niveshMitramaster.Sector = _ApplicantVM.Sector;
                            niveshMitramaster.Block = _ApplicantVM.Block;
                            niveshMitramaster.PlotNo = _ApplicantVM.PlotNo;
                            niveshMitramaster.MobileNo = _ApplicantVM.Mobile;
                            niveshMitramaster.Status = true;
                            niveshMitramaster.CreatedBy = userInfo.UserID;
                            niveshMitramaster.CreatedDate = DateTime.Now;
                            dbContext.NiveshMitraEntr_Master.Add(niveshMitramaster);
                            dbContext.SaveChanges();
                            flag = true;
                        }
                    }
                    else
                    {
                        _nsMaster.ApplicationID = model.TxtApplicationID;
                        _nsMaster.ModifiedBy = userInfo.UserID;
                        _nsMaster.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                    }
                }
            }
            return flag;
        }

        public SWPServicesViewModel GetNiveshMitraServicesByRegistrationId(SWPPostViewModel apimodel)
        {
            var _ServiceRequestVM = new SWPServicesViewModel();
            _ServiceRequestVM.TxtControlID = apimodel.TxtControlID;
            _ServiceRequestVM.TxtUnitID = apimodel.TxtUnitID;
            _ServiceRequestVM.TxtServiceID = apimodel.TxtServiceID;
            _ServiceRequestVM.TxtProcessIndustryID = apimodel.TxtProcessIndustryID;
            _ServiceRequestVM.TxtApplicationID = apimodel.TxtApplicationID;
            using (var dbContext = new PIMSEntitiesContext())
            {
                int _registrationId = !string.IsNullOrEmpty(apimodel.TxtProcessIndustryID) ? Convert.ToInt32(apimodel.TxtProcessIndustryID) : 0;
                var _ApplicantVM = new SWPApplicantViewModel();
                _ApplicantVM = GetApplicantDetailsByRegistrationId(_registrationId);
                _ServiceRequestVM.ApplicantModel = _ApplicantVM;

                var _rid = Convert.ToString(_registrationId);
                var nic = dbContext.NiveshMitraEntr_Master.Where(nmservice => nmservice.ControlID == apimodel.TxtControlID && nmservice.UnitID == apimodel.TxtUnitID && nmservice.ProcessIndustryID == _rid && nmservice.ServiceID == apimodel.TxtServiceID).FirstOrDefault();
                if (nic != null)
                {
                    var _nic = new SWPPostViewModel
                    {
                        TxtControlID = nic.ControlID,
                        TxtApplicationID = nic.ApplicationID,
                        TxtUnitID = nic.UnitID,
                        TxtServiceID = nic.ServiceID,
                        TxtProcessIndustryID = nic.ProcessIndustryID
                    };
                    _ServiceRequestVM.SWPPostModel = _nic;
                    _ServiceRequestVM.IsServiceExist = !string.IsNullOrEmpty(_nic.TxtApplicationID) ? true : false;
                }
                else
                {
                    _ServiceRequestVM.SWPPostModel = apimodel;
                    _ServiceRequestVM.IsServiceExist = false;
                }

                var _ServiceVM = GetDetailsForNICRequestServiceByRegistrationId(_registrationId);
                if (_ServiceVM != null)
                {
                    _ServiceVM.ServiceId = GetServiceByUnitServiceID(_ServiceRequestVM.SWPPostModel.TxtServiceID, _ApplicantVM.DepartmentId);
                    _ServiceRequestVM.ServiceModel = _ServiceVM;
                }
               
                if (_ServiceRequestVM.SWPPostModel.TxtServiceID == SWPConstant.NICNoDuesCertificate || _ServiceRequestVM.SWPPostModel.TxtServiceID == SWPConstant.NICDuesCalculation)
                {
                    if (_ServiceRequestVM.ServiceModel.DepartmentId == SWPDepartment.Institutional) { _ServiceRequestVM.ServiceModel.SubDepartmentId = 2; }
                    if (_ServiceRequestVM.ServiceModel.DepartmentId == SWPDepartment.Industrial) { _ServiceRequestVM.ServiceModel.SubDepartmentId = 8; }
                }
            }
            return _ServiceRequestVM;
        }

        private SWPServiceViewModel GetDetailsForNICRequestServiceByRegistrationId(int? registrationId)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var serviceVM = (from alotment in dbContext.AllotmentMasters
                                 join department in dbContext.DepartmentMsts on alotment.departmentId equals department.departmentId
                                 join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
                                 join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
                                 where alotment.rid == registrationId && alotment.isActive == 1
                                 select new SWPServiceViewModel
                                 {
                                     RegistrationId = alotment.rid,
                                     RegistrationNo = alotment.rid.ToString(),
                                     DepartmentId = alotment.departmentId,
                                     Department = department.departmentName,
                                     SectorId = property.sectorId,
                                     Sector = property.SectorMst.sectorName,
                                     BlockId = property.blockId,
                                     Block = property.BlockMst.blockName,
                                     PlotNo = property.propertyNo,
                                     Applicant = aplicant.tGender == SWPConstant.Company ? (!string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.T_Company_Name : aplicant.tFirstName) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? aplicant.tLastName : aplicant.tMiddleName + " " + aplicant.tLastName),
                                     ApplicantType = aplicant.tGender,
                                     MobileNo = aplicant.tMobileNumber,
                                     Email = aplicant.tEmail,
                                     ApplicantAddress = aplicant.tCorrespondanceAdd
                                 }).FirstOrDefault();
                return serviceVM;
            }
        }

        private int? GetServiceByUnitServiceID(string serviceId, int? departmentId)
        {
            int? _serviceId = 0;
            using (var dbContext = new PIMSEntitiesContext())
            {
                var _serviceDetails = (from nm_services in dbContext.Services
                                       join nm_serviceTrans in dbContext.ServiceTrans on nm_services.Id.ToString() equals nm_serviceTrans.ServiceId
                                       join customerservice in dbContext.CitizenService_Master on nm_serviceTrans.CitizenServiceId equals customerservice.Id
                                       where nm_services.Status == 1 && customerservice.Status == 1
                                       && nm_services.ServiceCode == serviceId && customerservice.Deptt_Id == departmentId
                                       select customerservice).FirstOrDefault();

                if (_serviceDetails != null)
                {
                    _serviceId = _serviceDetails.service_id;
                }
            }
            return _serviceId;
        }

        public SWPServicesViewModel GetNiveshMitraServicesByRequestId(int? RequestId)
        {
            var _ServiceRequestVM = new SWPServicesViewModel();
            using (var dbContext = new PIMSEntitiesContext())
            {
                int _requestId = RequestId > 0 ? Convert.ToInt32(RequestId) : 0;
                var _ApplicantVM = new SWPApplicantViewModel();
                _ApplicantVM = GetApplicantDetailsByRegistrationId(_requestId);

                var _rid = Convert.ToString(_requestId);

                _ServiceRequestVM = (from nmservice in dbContext.NiveshMitraEntr_Master
                                     where nmservice.ProcessIndustryID == _rid
                                     select new SWPServicesViewModel
                                     {
                                         RegistrationId = _ApplicantVM.RegistrationId,
                                         ServiceModel = new SWPServiceViewModel
                                         {
                                             RegistrationId = _ApplicantVM.RegistrationId,
                                             DepartmentId = _ApplicantVM.DepartmentId,
                                         },
                                         SWPPostModel = new SWPPostViewModel
                                         {
                                             TxtControlID = nmservice.ControlID,
                                             TxtApplicationID = nmservice.ApplicationID,
                                             TxtUnitID = nmservice.UnitID,
                                             TxtServiceID = nmservice.ServiceID,
                                             TxtProcessIndustryID = nmservice.ProcessIndustryID
                                         }
                                     }).FirstOrDefault();

                if (!string.IsNullOrEmpty(_ServiceRequestVM.SWPPostModel.TxtServiceID))
                {
                    _ServiceRequestVM.ServiceModel.ServiceId = GetServiceByUnitServiceID(_ServiceRequestVM.SWPPostModel.TxtServiceID, _ServiceRequestVM.ServiceModel.DepartmentId);

                    // in case of NDC sub dept. is account
                    if (_ServiceRequestVM.SWPPostModel.TxtServiceID == SWPConstant.NICNoDuesCertificate)
                    {
                        if (_ServiceRequestVM.ServiceModel.DepartmentId == SWPDepartment.Institutional) { _ServiceRequestVM.ServiceModel.SubDepartmentId = 2; }
                        if (_ServiceRequestVM.ServiceModel.DepartmentId == SWPDepartment.Industrial) { _ServiceRequestVM.ServiceModel.SubDepartmentId = 8; }
                    }
                }
            }
            return _ServiceRequestVM;
        }

        public SWPApiServiceViewModel GetNiveshMitraServicesDetails(SWPPostViewModel model)
        {
            var _apiService = new SWPApiServiceViewModel();
            using (var dbContext = new PIMSEntitiesContext())
            {
                var exService = dbContext.NiveshMitraEntr_Master.Where(e => e.ControlID == model.TxtControlID && e.UnitID == model.TxtUnitID).ToList();
                if (exService != null && exService.Count > 0)
                {
                    _apiService = (from nmservice in dbContext.NiveshMitraEntr_Master
                                   where nmservice.ControlID == model.TxtControlID && nmservice.UnitID == model.TxtUnitID
                                   && nmservice.Status == true
                                   select new SWPApiServiceViewModel
                                   {
                                       NICApplicationId = nmservice.ApplicationID,
                                       NICControlId = nmservice.ControlID,
                                       NICUnitId = nmservice.UnitID,
                                       NICServiceId = model.TxtServiceID, //nmservice.ServiceID,
                                       NICProcessIndustryId = nmservice.ProcessIndustryID,
                                       Sector = nmservice.Sector,
                                       Block = nmservice.Block,
                                       PlotNo = nmservice.PlotNo,
                                       MobileNo = nmservice.MobileNo,
                                       Status = nmservice.Status
                                   }).FirstOrDefault();
                    _apiService.IsServiceExist = true;
                }
                else
                {
                    _apiService.NICControlId = model.TxtControlID;
                    _apiService.NICUnitId = model.TxtUnitID;
                    _apiService.NICServiceId = model.TxtServiceID; //nmservice.ServiceID,
                    _apiService.NICProcessIndustryId = model.TxtProcessIndustryID;
                    _apiService.IsServiceExist = false;
                }
                return _apiService;
                //var nicservice = (from nmservice in dbContext.NiveshMitraEntr_Master
                //                  where nmservice.ControlID == model.TxtControlID && nmservice.UnitID == model.TxtUnitID && nmservice.ServiceID == model.TxtServiceID
                //                  && nmservice.Status == true
                //                  select new SWPApiServiceViewModel
                //                  {
                //                      NICApplicationId = nmservice.ApplicationID,
                //                      NICControlId = nmservice.ControlID,
                //                      NICUnitId = nmservice.UnitID,
                //                      NICServiceId = nmservice.ServiceID,
                //                      NICProcessIndustryId = nmservice.ProcessIndustryID,
                //                      Sector = nmservice.Sector,
                //                      Block = nmservice.Block,
                //                      PlotNo = nmservice.PlotNo,
                //                      MobileNo = nmservice.MobileNo,
                //                      Status = nmservice.Status
                //                  }).LastOrDefault();
                //nicservice.IsServiceExist = !string.IsNullOrEmpty(nicservice.NICProcessIndustryId) ? true : false;
                //return nicservice;
            }
        }

        public SWPServicesViewModel GetServiceRequestDetailById(int? id)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var model = new SWPServicesViewModel();

                var service = (from csr in dbContext.Customer_ServiceRequest
                               where csr.Id == id
                               select new SWPServiceViewModel
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
                                   SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SWPDepartment.SubDepartment.Property : csr.SubDepartment,
                                   SubDepartmentId = string.IsNullOrEmpty(csr.SubDepartment) ? 1 : dbContext.SubDepartmentMsts.FirstOrDefault(x => x.departmentId == csr.DepartmentId && x.SubdepartmentName == csr.SubDepartment && x.IsActive == true).SubdepartmentId,
                                   Description = csr.Description,
                                   RegistrationType = csr.ServiceType == SWPConstant.SDService ? SWPConstant.PradhikaranDiwas : (string.IsNullOrEmpty(csr.Registration_No) ? SWPConstant.UnRegistered : SWPConstant.Registered),
                                   ServiceStatusId = csr.Request_Status,
                                   RequestStatus = dbContext.StatusMasters.FirstOrDefault(m => m.Id == csr.Request_Status && m.IsActive == true).Status,
                                   Comment = csr.Comment,
                                   RequestComment = csr.Comment,
                                   ApproverId = csr.ApproverId,
                                   ValidatorId = csr.ValidatorId,
                                   ValidationDate = csr.ValidatedDate,
                                   ApprovalDate = csr.ApprovalDate,
                                   CreatedDate = csr.Created_Date,
                                   Approver = csr.ApproverId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == csr.ApproverId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == csr.ApproverId).LastName) : string.Empty,
                                   Validator = csr.ValidatorId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == csr.ValidatorId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == csr.ValidatorId).LastName) : string.Empty,
                                   StatusId = csr.Request_Status,
                                   PaymentStatus = csr.PaymentStatus,
                                   DispatchedDocument = csr.DispatchDocumentName,
                                   ModifiedDate = csr.Modified_Date
                               }).FirstOrDefault();
                if (service != null)
                {
                    service.RegistrationId = Convert.ToInt32(service.RegistrationNo);
                    if (service.ApprovalDate != null)
                    {
                        if (service.StatusId == SWPStatusId.Completed || service.StatusId == SWPStatusId.Approved)
                        {
                            service.PendencyLevel = "Completed By - " + service.Approver;
                        }
                        else if (service.StatusId == SWPStatusId.Rejected)
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
                        if (service.StatusId == SWPStatusId.Forwarded)
                        {
                            service.PendencyLevel = "Forwarded To - " + service.Approver;
                        }
                        else if (service.StatusId == SWPStatusId.Pending)
                        {
                            service.PendencyLevel = "Put on Pending By - " + service.Approver;
                        }
                        else if (service.StatusId == SWPStatusId.Objection)
                        {
                            service.PendencyLevel = "Put on Objection By - " + service.Approver;
                        }
                        else if (service.StatusId == SWPStatusId.Initiated)
                        {
                            service.PendencyLevel = "Service Request is in Progress.";
                        }
                        else if (service.StatusId == SWPStatusId.Withdrawn)
                        {
                            service.PendencyLevel = "Withdrawn by - Allottee";
                        }
                        else if (service.StatusId == SWPStatusId.Resubmitted)
                        {
                            service.PendencyLevel = "Resubmitted By - Allottee";
                        }
                        else if (service.StatusId == SWPStatusId.Appointment)
                        {
                            service.PendencyLevel = "Put on Appointment By - " + service.Approver;
                        }
                        else
                        {
                            service.PendencyLevel = null;
                        }
                    }
                }

                service.StatusMessage = GetRequestStatusMessage(service.StatusId, service.PaymentStatus);

                if (!string.IsNullOrEmpty(service.RegistrationNo)) service.RegistrationId = Convert.ToInt32(service.RegistrationNo);


                model.Id = service.Id;
                model.RegistrationId = !string.IsNullOrEmpty(service.RegistrationNo) ? Convert.ToInt32(service.RegistrationNo) : 0;
                model.OnlineRequestId = service.Id;
                model.ServiceModel = service;

                // get detail by service id
                switch (service.ServiceId)
                {
                    case SWPService.Transfer:
                        model = GetTransferRequestServiceDetail(model);
                        break;
                    case SWPService.Rent:
                        model = GetRentRequestServiceDetail(model);
                        break;
                    case SWPService.Mortgage:
                        model = GetMortgageRequestServiceDetail(model);
                        break;
                    case SWPService.Extension:
                        model = GetExtensionRequestDetails(model);
                        break;
                    default:
                        //model = model;
                        break;
                }
                return model;
            }
        }

        private string GetRequestStatusMessage(int? StatusId, int? PaymentStatus)
        {
            var msg = string.Empty;
            if (StatusId == SWPStatusId.Initiated)
            {
                msg = "Your application has been submitted and send to department.";
                if (PaymentStatus == 0)
                {
                    msg = "Your application has been submitted and payment is pending";
                }
                else if (PaymentStatus == 1)
                {
                    msg = "Your application has been submitted and payment is paid.";
                }
            }
            else if (StatusId == SWPStatusId.Objection)
            {
                msg = "Your application has an objection.Please submit the required documents.";
            }
            else if (StatusId == SWPStatusId.Completed)
            {
                msg = "Your application has been completed.";
            }
            else if (StatusId == SWPStatusId.Pending)
            {
                msg = "Your application has been pending.";
            }
            else if (StatusId == SWPStatusId.Rejected)
            {
                msg = "Your application has been rejected.";
            }
            else
            {
                msg = "Your application has been in process.";
            }
            return msg;
        }

        private SWPServicesViewModel GetExtensionRequestDetails(SWPServicesViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var data = dbContext.OnlineExtensionDetails.Where(m => m.RequestNo == model.Id).FirstOrDefault();
                if (data != null)
                {
                    SWPExtensionViewModel extension = new SWPExtensionViewModel
                    {
                        ExtensionDueDate = data.ExtensionDueDate,
                        ExtensionGivenDate = data.ExtensionGivenDate
                    };
                    model.ExtensionModel = extension;
                    //return model;
                }
                else
                {
                    model.ExtensionModel = new SWPExtensionViewModel();
                }
                return model;
            }
        }

        private SWPServicesViewModel GetMortgageRequestServiceDetail(SWPServicesViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var data = dbContext.OnlineMortgageDetails.Where(m => m.RequestNo == model.Id).FirstOrDefault();
                if (data != null)
                {
                    SWPMortgageViewModel mortgage = new SWPMortgageViewModel
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
                    model.MortgageModel = new SWPMortgageViewModel();
                }

                return model;
            }
        }

        private SWPServicesViewModel GetRentRequestServiceDetail(SWPServicesViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var rents = dbContext.OnlineRentPermissionDetails.Where(r => r.RequestNo == model.Id).FirstOrDefault();
                if (rents != null)
                {
                    SWPRentingViewModel rent = new SWPRentingViewModel
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
                    model.RentModel = new SWPRentingViewModel();
                }
                return model;
            }
        }

        private SWPServicesViewModel GetTransferRequestServiceDetail(SWPServicesViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var transfer = dbContext.OnlineTransferMutations.Where(m => m.RequestNo == model.Id && m.Type == "T").FirstOrDefault();
                if (transfer != null)
                {
                    SWPTransferViewModel tmodel = new SWPTransferViewModel
                    {
                        TransferTypeId = transfer.Transfer_Type.Value,
                        TransferType = dbContext.Transfer_Type.Where(t => t.Id == transfer.Transfer_Type).Select(t => t.type).FirstOrDefault(),
                        TransferSubTypeId = transfer.Transfer_Sub_Type.Value,
                        TransferSubType = dbContext.Transfer_Type.Where(t => t.Id == transfer.Transfer_Sub_Type).Select(t => t.type).FirstOrDefault(),
                        Gender = (transfer.T_Gender == SWPConstant.Company) ? SWPConstant.Company : (transfer.T_Gender == SWPConstant.Male ? SWPConstant.Male : (transfer.T_Gender == SWPConstant.Female ? SWPConstant.Female : string.Empty)),
                        ApplicantType = transfer.T_Gender,
                        FirstName = transfer.T_First_Name,
                        MiddleName = transfer.T_Middle_Name,
                        LastName = transfer.T_Last_Name,
                        Applicant = transfer.T_Gender == SWPConstant.Company ? transfer.T_Company_Name : transfer.T_First_Name + " " + (string.IsNullOrEmpty(transfer.T_Middle_Name) ? string.Empty : transfer.T_Middle_Name + " ") + transfer.T_Last_Name,
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
                        TypeOfTransferee = (transfer.T_Gender == SWPConstant.Male || transfer.T_Gender == SWPConstant.Female) ? SWPConstant.Individual : SWPConstant.Company
                    };

                    model.TransferModel = tmodel;
                }
                else
                {
                    model.TransferModel = new SWPTransferViewModel();
                }
                return model;
            }
        }


        public SWPServicesViewModel ReSubmitRequest(SWPServicesViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            var _ServiceRequestVM = new SWPServicesViewModel();
            using (var dbContext = new PIMSEntitiesContext())
            {
                var _service = dbContext.Customer_ServiceRequest.Where(m => m.Id == model.OnlineRequestId).FirstOrDefault();
                if (_service != null)
                {
                    _service.Request_Status = SWPStatusId.Resubmitted;
                    _service.Modified_Date = DateTime.Now;
                    _service.Modified_By = Convert.ToInt32(_service.Registration_No);
                    //_service.ValidatorId = null;
                    dbContext.SaveChanges();
                    int _rid = Convert.ToInt32(_service.Registration_No);

                    _ServiceRequestVM = GetNiveshMitraServicesByReqId(model.OnlineRequestId);

                    //string message = string.Format(NAMessages.OnlineServiceRequest, _service.Id);
                    //ApplicationHelper.SendSMS(_service.MobileNumber, message);
                    //if (!string.IsNullOrEmpty(_service.Email)) ApplicationHelper.SendEmail(_service.Email, "Online Request", message);

                    if (_service.Id > 0)
                    {
                        model.RegistrationId = _rid;
                        model.ServiceModel = new SWPServiceViewModel
                        {
                            RegistrationId = _rid
                        };
                        UploadRequiredFiles(model, files, _service.Id);
                    }
                }
            }
            return _ServiceRequestVM;
        }

        public DataSourceResult GetServiceRequestUploadedDocumentsById(DataSourceRequest request, SWPServiceViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                List<SWPDocumentViewModel> documentList = new List<SWPDocumentViewModel>();
                FTPHandler ftpHandler = new FTPHandler();
                var service = dbContext.Customer_ServiceRequest.FirstOrDefault(f => f.Id == model.RequestId);
                if (model.RegistrationId == null)
                {
                    if (service != null)
                    {
                        documentList = ftpHandler.GetServiceRequestDocuments_NIC((int)model.RequestId);
                        return documentList.ToDataSourceResult(request);
                    }
                    else return null;
                }
                else
                {
                    documentList = ftpHandler.GetServiceRequestUploadedDocumentsById_NIC((int)model.RegistrationId, (int)model.RequestId);
                    return documentList.ToDataSourceResult(request);
                }
            }
        }

        public SWPApiServiceViewModel GetNiveshMitraServicesDetailsByApplicationId(int applicationId)
        {
            var _ServiceRequestVM = new SWPApiServiceViewModel();
            var _appId = Convert.ToString(applicationId);
            using (var dbContext = new PIMSEntitiesContext())
            {
                _ServiceRequestVM = (from nmservice in dbContext.NiveshMitraEntr_Master
                                     where nmservice.ApplicationID == _appId
                                     && nmservice.Status == true
                                     select new SWPApiServiceViewModel
                                     {
                                         NICApplicationId = nmservice.ApplicationID,
                                         NICControlId = nmservice.ControlID,
                                         NICUnitId = nmservice.UnitID,
                                         NICServiceId = nmservice.ServiceID,
                                         NICProcessIndustryId = nmservice.ProcessIndustryID,
                                         Sector = nmservice.Sector,
                                         Block = nmservice.Block,
                                         PlotNo = nmservice.PlotNo,
                                         MobileNo = nmservice.MobileNo,
                                         Status = nmservice.Status
                                     }).FirstOrDefault();

            }
            return _ServiceRequestVM;
        }

        public int UpdateRequestPaymentStatus(SWPServicesViewModel model)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (model.Id != null && model.Id > 0)
                {
                    var exService = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == model.Id);
                    if (exService != null)
                    {
                        exService.PaymentStatus = 1;
                        exService.Modified_Date = DateTime.Now;
                        exService.Modified_By = userInfo.UserID;
                        dbContext.SaveChanges();
                        return exService.Id;
                    }
                }
            }
            return 0;
        }

        public SWPServicesViewModel GetNiveshMitraServicesByReqId(int? RequestId)
        {
            var _ServiceRequestVM = new SWPServicesViewModel();
            using (var dbContext = new PIMSEntitiesContext())
            {
                int _requestId = RequestId > 0 ? Convert.ToInt32(RequestId) : 0;

                var _ApplicantVM = new SWPApplicantViewModel();
                var _rid = Convert.ToString(_requestId);

                _ServiceRequestVM = (from nmservice in dbContext.NiveshMitraEntr_Master
                                     where nmservice.ApplicationID == _rid
                                     select new SWPServicesViewModel
                                     {
                                         SWPPostModel = new SWPPostViewModel
                                         {
                                             TxtControlID = nmservice.ControlID,
                                             TxtApplicationID = nmservice.ApplicationID,
                                             TxtUnitID = nmservice.UnitID,
                                             TxtServiceID = nmservice.ServiceID,
                                             TxtProcessIndustryID = nmservice.ProcessIndustryID
                                         }
                                     }).FirstOrDefault();
            }
            return _ServiceRequestVM;
        }

        public SWPApiServiceViewModel GetServiceStatusByCustomerRequestStatusId(int RequestStatusId)
        {
            var _ServiceStatus = new SWPApiServiceViewModel();
            using (var dbContext = new PIMSEntitiesContext())
            {
                _ServiceStatus = (from nm_service in dbContext.ServiceStatus
                                  join nm_status in dbContext.ServiceStatusTrans on nm_service.Id equals nm_status.StatusID
                                  join customerstatus in dbContext.StatusMasters on nm_status.CitizenStatusID equals customerstatus.Id
                                  where nm_service.Status == 1 && customerstatus.IsActive == true && customerstatus.Id == RequestStatusId
                                  select new SWPApiServiceViewModel
                                  {
                                      StatusCode = nm_service.StatusCode,
                                      StatusName = nm_service.StatusName
                                  }).FirstOrDefault();
            }
            return _ServiceStatus;
        }

        public SWPClientRequestViewModel GetCitizenServiceDetails(int? ServiceId, int? DeptId)
        {
            var _details = new SWPClientRequestViewModel();
            using (var dbContext = new PIMSEntitiesContext())
            {
                _details = (from service in dbContext.CitizenService_Master
                            where service.Status == 1 && service.service_id == ServiceId && service.Deptt_Id == DeptId
                            select new SWPClientRequestViewModel
                            {
                                ServiceId = service.service_id,
                                ServiceName = service.ServiceName,
                                DepartmentId = service.Deptt_Id > 0 ? (int)service.Deptt_Id : 0,
                                ServiceFee = service.Amount != null && service.Amount > 0 ? service.Amount : 0
                            }).FirstOrDefault();
            }
            return _details;
        }

        public Kendo.Mvc.UI.DataSourceResult GetNiveshMitraServiceRequests(Kendo.Mvc.UI.DataSourceRequest request)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var _niveshmitraService = (from niveshmitra in dbContext.NiveshMitraEntr_Master
                                           join nm_services in dbContext.Services on niveshmitra.ServiceID equals nm_services.ServiceCode
                                           where niveshmitra.Status == true && niveshmitra.ApplicationID != null
                                           select new SWPApiServiceViewModel
                                           {
                                               Id = niveshmitra.ID,
                                               NICControlId = niveshmitra.ControlID,
                                               NICUnitId = niveshmitra.UnitID,
                                               NICServiceId = niveshmitra.ServiceID,
                                               NICProcessIndustryId = niveshmitra.ProcessIndustryID,
                                               NICApplicationId = niveshmitra.ApplicationID,
                                               CreatedBy = niveshmitra.CreatedBy,
                                               CreatedDate = niveshmitra.CreatedDate,
                                               Status = niveshmitra.Status,
                                               ServiceName = nm_services.ServiceName
                                           });
                return _niveshmitraService.ToDataSourceResult(request);
            }
        }

        public Kendo.Mvc.UI.DataSourceResult GetNiveshMitraServices(Kendo.Mvc.UI.DataSourceRequest request, SWPApiServiceViewModel apimodel)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var _niveshmitraService = (from nm_services in dbContext.Services
                                           where (apimodel.NICServiceCode == null || nm_services.ServiceCode == apimodel.NICServiceCode) //&& nm_services.Status == 1
                                           select new SWPApiServiceViewModel
                                           {
                                               Id = nm_services.Id,
                                               NICServiceCode = nm_services.ServiceCode,
                                               ServiceName = nm_services.ServiceName,
                                               StatusId = nm_services.Status,
                                               CreatedBy = nm_services.CreatedBy,
                                               CreatedDate = nm_services.CreatedDate,
                                           });
                return _niveshmitraService.ToDataSourceResult(request);
            }
        }

        public Kendo.Mvc.UI.DataSourceResult GetNiveshMitraServiceStatus(Kendo.Mvc.UI.DataSourceRequest request, SWPApiServiceViewModel apimodel)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var _niveshmitraServiceStatus = (from nm_service in dbContext.ServiceStatus
                                                 join nm_status in dbContext.ServiceStatusTrans on nm_service.Id equals nm_status.StatusID
                                                 join customerstatus in dbContext.StatusMasters on nm_status.CitizenStatusID equals customerstatus.Id
                                                 where customerstatus.IsActive == true //&&  nm_service.Status == 1 
                                                 && (apimodel.CitizenStatusId == null || nm_status.CitizenStatusID == apimodel.CitizenStatusId)
                                                 select new SWPApiServiceViewModel
                                                 {
                                                     Id = nm_service.Id,
                                                     StatusCode = nm_service.StatusCode,
                                                     StatusName = nm_service.StatusName,
                                                     StatusId = nm_service.Status,
                                                     CreatedBy = nm_service.CreatedBy,
                                                     CreatedDate = nm_service.CreatedDate,
                                                     CitizenStatusId = nm_status.CitizenStatusID,
                                                     CitizenStatusName = customerstatus.Status
                                                 });
                return _niveshmitraServiceStatus.ToDataSourceResult(request);
            }
        }

        public Kendo.Mvc.UI.DataSourceResult GetCitizenServiceDetailsbyServiceId(Kendo.Mvc.UI.DataSourceRequest request)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var _niveshmitraService = (from nm_services in dbContext.Services
                                           join nm_serviceTrans in dbContext.ServiceTrans on nm_services.Id.ToString() equals nm_serviceTrans.ServiceId
                                           join customerservice in dbContext.CitizenService_Master on nm_serviceTrans.CitizenServiceId equals customerservice.Id
                                           join dept in dbContext.DepartmentMsts on customerservice.Deptt_Id equals dept.departmentId
                                           where nm_services.Status == 1 && customerservice.Status == 1 && dept.IsActive == true
                                           select new SWPApiServiceViewModel
                                           {
                                               NICServiceCode = nm_services.ServiceCode,
                                               ServiceName = nm_services.ServiceName,
                                               StatusId = nm_services.Status,
                                               CreatedBy = nm_services.CreatedBy,
                                               CreatedDate = nm_services.CreatedDate,
                                               CitizenServiceId = nm_serviceTrans.CitizenServiceId,
                                               DepartmentId = customerservice.Deptt_Id,
                                               ServiceId = customerservice.service_id,
                                               Amount = customerservice.Amount,
                                               CitizenServiceName = customerservice.ServiceName,
                                               Timeline = customerservice.Timeline,
                                               CitizenStatusId = customerservice.Status,
                                               Department = dept.departmentName
                                           });
                return _niveshmitraService.ToDataSourceResult(request);
            }
        }

        public Kendo.Mvc.UI.DataSourceResult GetCustomerServiceRequestList_NIC(Kendo.Mvc.UI.DataSourceRequest request)
        {
            IQueryable<SWPServiceViewModel> list;
            List<string> SubDepartmentList = new List<string>();
            if (userInfo.RoleMaster.RoleInDepartment == SWPDepartment.Role.Accountant || userInfo.RoleMaster.RoleInDepartment == SWPDepartment.Role.HODAccounts)
            {
                SubDepartmentList.Add("Account");
            }
            else
            {
                if (userInfo.RoleMaster.RoleInDepartment == SWPDepartment.Role.Assistant || userInfo.RoleMaster.RoleInDepartment == SWPDepartment.Role.Property)
                {
                    SubDepartmentList.Add("Property");
                }
                else
                {
                    SubDepartmentList.Add("Account");
                    SubDepartmentList.Add("Property");
                }
            }

            int userid = userInfo.UserID;
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (userInfo.RoleMaster.RoleInDepartment == SWPDepartment.Role.OSD)
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && SubDepartmentList.Contains(csr.SubDepartment) && csm.Status == 1 && csr.RequestThrough == SWPConstant.NIC_NiveshMitra && csr.ServiceId != 12 && csr.ServiceId != 13
                            select new SWPServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No ?? string.Empty,
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
                                IsCancelled = csr.Request_Status == SWPStatusId.Cancelled ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SWPDepartment.SubDepartment.Property : csr.SubDepartment,
                                PaymentStatus = csr.PaymentStatus,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                IsUploaded = csr.IsUploadedLetter,
                                DispatchedDocument = csr.DispatchDocumentName,
                                NICUnitId = dbContext.NiveshMitraEntr_Master.FirstOrDefault(m => m.ApplicationID == csr.Id.ToString()).UnitID
                            });
                }
                else if (userInfo.RoleMaster.RoleInDepartment == SWPDepartment.Role.HODAccounts)
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && SubDepartmentList.Contains(csr.SubDepartment) && csm.Status == 1 && csr.RequestThrough == SWPConstant.NIC_NiveshMitra && (csr.ServiceId == 12 || csr.ServiceId == 13)
                            select new SWPServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No ?? string.Empty,
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
                                IsCancelled = csr.Request_Status == SWPStatusId.Cancelled ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SWPDepartment.SubDepartment.Property : csr.SubDepartment,
                                PaymentStatus = csr.PaymentStatus,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                IsUploaded = csr.IsUploadedLetter,
                                DispatchedDocument = csr.DispatchDocumentName,
                                NICUnitId = dbContext.NiveshMitraEntr_Master.FirstOrDefault(m => m.ApplicationID == csr.Id.ToString()).UnitID
                            });
                }
                else if (userInfo.RoleMaster.RoleInDepartment == null || userInfo.RoleMaster.RoleInDepartment == SWPDepartment.Role.Admin)
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && SubDepartmentList.Contains(csr.SubDepartment) && csm.Status == 1 && csr.RequestThrough == SWPConstant.NIC_NiveshMitra
                            select new SWPServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No ?? string.Empty,
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
                                IsCancelled = csr.Request_Status == SWPStatusId.Cancelled ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SWPDepartment.SubDepartment.Property : csr.SubDepartment,
                                PaymentStatus = csr.PaymentStatus,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                IsUploaded = csr.IsUploadedLetter,
                                DispatchedDocument = csr.DispatchDocumentName,
                                NICUnitId = dbContext.NiveshMitraEntr_Master.FirstOrDefault(m => m.ApplicationID == csr.Id.ToString()).UnitID
                            });
                }
                else
                {
                    list = (from csr in dbContext.Customer_ServiceRequest
                            join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
                            join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
                            where DepartmentList.Contains(csr.DepartmentId) && csm.Status == 1 && csr.RequestThrough == SWPConstant.NIC_NiveshMitra && csr.ApproverId == userid
                            select new SWPServiceViewModel
                            {
                                Id = csr.Id,
                                RequestId = csr.Id,
                                RegistrationNo = csr.Registration_No,
                                DepartmentId = csr.DepartmentId,
                                Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
                                PropertyNo = csr.Property_No ?? string.Empty,
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
                                IsCancelled = csr.Request_Status == SWPStatusId.Cancelled ? true : false,
                                Comment = csr.Comment,
                                MobileNo = csr.MobileNumber,
                                Applicant = csr.ApplicantName,
                                Description = csr.Description,
                                Requestor = csr.RequestorName,
                                RequestorAddress = csr.RequestorAddress,
                                SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SWPDepartment.SubDepartment.Property : csr.SubDepartment,
                                PaymentStatus = csr.PaymentStatus,
                                Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
                                IsUploaded = csr.IsUploadedLetter,
                                DispatchedDocument = csr.DispatchDocumentName,
                                NICUnitId = dbContext.NiveshMitraEntr_Master.FirstOrDefault(m => m.ApplicationID == csr.Id.ToString()).UnitID
                            });
                }
                return list.ToDataSourceResult(request);
            }
        }

        public int UpdateCustomerServiceRequestStatus(SWPServiceViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int flag = SWPReturnTypeId.None;

            using (var dbContext = new PIMSEntitiesContext())
            {
                var service = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == model.Id);
                var status = dbContext.Customer_ServiceStatusTrans.Where(c => c.RequestRefId == model.Id).OrderByDescending(c => c.Id).FirstOrDefault();
                if (service.Request_Status == SWPStatusId.Completed)
                {
                    flag = SWPReturnTypeId.Updated;
                }
                else if (service.Request_Status == SWPStatusId.Forwarded || service.Request_Status == SWPStatusId.Pending || service.Request_Status == SWPStatusId.Objection || service.Request_Status == SWPStatusId.Resubmitted)
                {
                    if (model.StatusId == SWPStatusId.Forwarded && service.Request_Status == SWPStatusId.Forwarded)
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        service.ValidatorId = userInfo.UserID;
                        service.ValidatedDate = DateTime.Now;
                        service.ApproverId = model.ApproverId;
                        dbContext.SaveChanges();

                        if (status != null)
                        {
                            var service_Status = new Customer_ServiceStatusTrans
                            {
                                RequestRefId = service.Id,
                                ValidatorId = userInfo.UserID,
                                ValidatedDate = DateTime.Now,
                                ApproverId = model.ApproverId,
                                CreatedDate = DateTime.Now,
                                CreatedBy = userInfo.UserID,
                                StatusId = model.StatusId,
                                Remarks = model.Comment + "by \n" + userInfo.FirstName + " " + userInfo.LastName + "Date:" + DateTime.Now + "."
                            };
                            dbContext.Customer_ServiceStatusTrans.Add(service_Status);
                            dbContext.SaveChanges();
                        }
                        flag = SWPReturnTypeId.Validated;
                    }
                    else if (model.StatusId == SWPStatusId.Pending && service.Request_Status == SWPStatusId.Pending)
                    {
                        flag = SWPReturnTypeId.Exist;
                    }
                    else if (model.StatusId == SWPStatusId.Pending && service.Request_Status == SWPStatusId.Forwarded)
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        flag = SWPReturnTypeId.Pending;
                        dbContext.SaveChanges();
                    }
                    else if (model.StatusId == SWPStatusId.Objection && service.Request_Status == SWPStatusId.Objection)
                    {
                        flag = SWPReturnTypeId.Exist;
                    }
                    else if (model.StatusId == SWPStatusId.Objection && service.Request_Status == SWPStatusId.Forwarded)
                    {
                        service.Request_Status = model.StatusId;
                        service.ObjectionStatus = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + "-" + userInfo.FirstName + " " + userInfo.LastName + " " + " Status: Objection " + " " + " Date:" + DateTime.Now + ".";
                        //service.ValidatorId = userInfo.UserID;
                        //service.ValidatedDate = DateTime.Now;
                        //service.ApproverId = model.ApproverId;
                        dbContext.SaveChanges();
                        if (status != null)
                        {
                            status.StatusId = model.StatusId;
                            status.ModifiedBy = userInfo.UserID;
                            status.ModifiedDate = DateTime.Now;
                            status.Remarks = status.Remarks + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                            dbContext.SaveChanges();
                        }
                        flag = SWPReturnTypeId.Validated;
                    }
                    else if (model.StatusId == SWPStatusId.Forwarded && service.Request_Status == SWPStatusId.Resubmitted)
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
                        service.ValidatorId = userInfo.UserID;
                        service.ValidatedDate = DateTime.Now;
                        service.ApproverId = model.ApproverId;
                        dbContext.SaveChanges();

                        if (status != null)
                        {
                            var service_Status = new Customer_ServiceStatusTrans
                            {
                                RequestRefId = service.Id,
                                ValidatorId = userInfo.UserID,
                                ValidatedDate = DateTime.Now,
                                ApproverId = model.ApproverId,
                                CreatedDate = DateTime.Now,
                                CreatedBy = userInfo.UserID,
                                StatusId = model.StatusId,
                                Remarks = model.Comment + "by \n" + userInfo.FirstName + " " + userInfo.LastName + "Date:" + DateTime.Now + "."
                            };
                            dbContext.Customer_ServiceStatusTrans.Add(service_Status);
                            dbContext.SaveChanges();
                        }
                        flag = SWPReturnTypeId.Validated;
                    }
                    else
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.MiddleName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
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
                        if (model.StatusId == SWPStatusId.Cancelled || model.StatusId == SWPStatusId.Rejected)
                        {
                            service.IsActive = false;
                            dbContext.SaveChanges();
                            flag = SWPReturnTypeId.Rejected;
                        }
                        if (model.StatusId == SWPStatusId.Approved || model.StatusId == SWPStatusId.Completed)
                        {
                            if (string.IsNullOrEmpty(service.Registration_No) && service.ServiceId != SWPService.Query)
                            {
                                flag = SWPReturnTypeId.NotRegistered;
                            }
                            else
                            {
                                dbContext.SaveChanges();

                                if (service.Request_Status == SWPStatusId.Completed)
                                {
                                    if (files != null && files.Count() > 0)
                                    {
                                        FTPHandler.UploadFiles_NIC(files, model.Id.ToString());
                                        if (!string.IsNullOrEmpty(files.FirstOrDefault().FileName))
                                        {
                                            var _fileName = "http://doc.mynoida.in/UploadDocuments/NiveshMitraUpload/" + model.RequestId + "/" + model.Id.ToString() + ".pdf";
                                            service.DispatchDocumentName = _fileName;
                                            dbContext.SaveChanges();
                                        }
                                    }
                                    flag = SWPReturnTypeId.Completed;
                                }
                            }
                        }

                        else flag = SWPReturnTypeId.Cancelled;
                        string reqName = dbContext.StatusMasters.Where(m => m.Id == service.Request_Status).FirstOrDefault().Status;

                        string message = string.Empty;
                        message = string.Format(SWPMessage.SDServiceReqStatusChange, model.Id, reqName);
                        if (!string.IsNullOrEmpty(service.MobileNumber)) { SWPApplication.SendSMS(service.MobileNumber, message); }
                        if (!string.IsNullOrEmpty(service.Email)) { SWPApplication.SendEmail(service.Email, "Online Request", message); }
                    }
                }
                else if (service.Request_Status == SWPStatusId.Initiated)//for forward 
                {
                    if (model.StatusId == SWPStatusId.Forwarded)
                    {
                        service.Request_Status = model.StatusId;
                        service.Comment = model.Comment + "-" + userInfo.FirstName + " " + userInfo.LastName;
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
                        flag = SWPReturnTypeId.Validated;
                    }
                    else flag = SWPReturnTypeId.Initiated;
                }
            }
            return flag;
        }

        public int UploadGeneratedLetterByserviceId(SWPServiceViewModel model, IEnumerable<HttpPostedFileBase> documentfiles)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var flag = SWPReturnTypeId.None;
                var service = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == model.RequestId);
                if (service.Request_Status == SWPStatusId.Completed)
                {
                    if (documentfiles != null && documentfiles.Count() > 0)
                    {
                        FTPHandler.UploadFiles_NIC(documentfiles, model.RequestId.ToString());
                        if (!string.IsNullOrEmpty(documentfiles.FirstOrDefault().FileName))
                        {
                            service.IsUploadedLetter = true;
                            var _fileName = "http://doc.mynoida.in/UploadDocuments/NiveshMitraUpload/" + model.RequestId + "/" + model.RequestId.ToString() + ".pdf";
                            service.DispatchDocumentName = _fileName;
                            service.DispatchDate = DateTime.Now;
                            dbContext.SaveChanges();
                            flag = SWPReturnTypeId.Saved;
                        }
                    }
                }
                return flag;
            }
        }

        public List<SWPDropdownViewModel> GetServiceListByDepartmentForNIC(int departmentId)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                var lst = (from service in dbContext.CitizenService_Master
                           join nic_service_trans in dbContext.ServiceTrans on service.Id equals nic_service_trans.CitizenServiceId
                           where service.Deptt_Id == departmentId && service.Status == 1
                           select new SWPDropdownViewModel
                           {
                               Id = service.service_id.Value,
                               ServiceId = service.service_id,
                               Text = service.ServiceName
                           }).ToList();
                return lst;
            }
        }

        public SWPServicesViewModel SaveNiveshMitraServiceUnit(SWPPostViewModel apimodel)
        {
            var service = new SWPServicesViewModel();
            service.IsServiceExist = false;
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (!string.IsNullOrEmpty(apimodel.TxtControlID) && !string.IsNullOrEmpty(apimodel.TxtUnitID))
                {
                    var _nsMaster = dbContext.NiveshMitraEntr_Master.FirstOrDefault(c => c.UnitID == apimodel.TxtUnitID && c.ControlID == apimodel.TxtControlID && c.ServiceID == apimodel.TxtServiceID);
                    if (_nsMaster == null)
                    {
                        var _ApplicantVM = new SWPApplicantViewModel();
                        if (!string.IsNullOrEmpty(apimodel.TxtProcessIndustryID))
                        {
                            int rid = Convert.ToInt32(apimodel.TxtProcessIndustryID);
                            _ApplicantVM = GetApplicantDetailsByRegistrationId(rid);
                        }

                        if (_ApplicantVM != null)
                        {
                            var niveshMitramaster = new NiveshMitraEntr_Master();
                            niveshMitramaster.ControlID = apimodel.TxtControlID;
                            niveshMitramaster.UnitID = apimodel.TxtUnitID;
                            niveshMitramaster.ServiceID = apimodel.TxtServiceID;
                            niveshMitramaster.ProcessIndustryID = apimodel.TxtProcessIndustryID;
                            niveshMitramaster.Sector = _ApplicantVM.Sector;
                            niveshMitramaster.Block = _ApplicantVM.Block;
                            niveshMitramaster.PlotNo = _ApplicantVM.PlotNo;
                            niveshMitramaster.MobileNo = _ApplicantVM.Mobile;
                            niveshMitramaster.Status = true;
                            niveshMitramaster.CreatedBy = userInfo.UserID;
                            niveshMitramaster.CreatedDate = DateTime.Now;
                            dbContext.NiveshMitraEntr_Master.Add(niveshMitramaster);
                            dbContext.SaveChanges();

                            service.IsServiceExist = true;
                        }
                    }
                    else
                    {
                        _nsMaster.ApplicationID = apimodel.TxtApplicationID;
                        _nsMaster.ModifiedBy = userInfo.UserID;
                        _nsMaster.ModifiedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        service.IsServiceExist = true;
                    }
                }
            }
            return service;
        }




        public SWPApiServiceViewModel SaveSWPServices(SWPApiServiceViewModel apimodel)
        {
            throw new NotImplementedException();
        }

        public SWPApiServiceViewModel ActivateSWPServiceStatus(SWPApiServiceViewModel apimodel)
        {
            using (var dbContext = new PIMSEntitiesContext())
            {
                if (apimodel.ActionType == "Status")
                {
                    var nicstatus = dbContext.ServiceStatus.FirstOrDefault(s => s.Id == apimodel.Id);
                    if (nicstatus != null)
                    {
                        nicstatus.Status = nicstatus.Status == 0 ? 1 : 0;
                        dbContext.SaveChanges();
                        apimodel.ReturnTypeId = SWPReturnTypeId.Updated;
                    }
                    else
                    {
                        apimodel.ReturnTypeId = SWPReturnTypeId.NotExist;
                    }
                }
                if (apimodel.ActionType == "Service")
                {
                    var nicservice = dbContext.Services.FirstOrDefault(s => s.Id == apimodel.Id);
                    if (nicservice != null)
                    {
                        nicservice.Status = nicservice.Status == 0 ? 1 : 0;
                        dbContext.SaveChanges();
                        apimodel.ReturnTypeId = SWPReturnTypeId.Updated;
                    }
                    else
                    {
                        apimodel.ReturnTypeId = SWPReturnTypeId.NotExist;
                    }
                }

                return apimodel;
            }
        }
    }
}
