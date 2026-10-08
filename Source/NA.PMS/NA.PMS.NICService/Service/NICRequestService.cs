using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
//using NA.PMS.Common;
//using NA.PMS.Model;
//using NA.PMS.Model.NIC;
//using NA.PMS.Repository;
//using NA.PMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;

namespace NA.PMS.NICServices
{
    public class NICRequestService //: INICRequestService
    {
        //private CurrentUserDetail userInfo = new CurrentUserDetail();
        //private List<int?> DepartmentList = new List<int?>();
        //public NICRequestService()
        //{
        //    if (HttpContext.Current != null)
        //    {
        //        if (HttpContext.Current.Session != null)
        //        {
        //            if (HttpContext.Current.Session["CurrentUser"] != null)
        //            {
        //                userInfo = HttpContext.Current.Session["CurrentUser"] as CurrentUserDetail;
        //                using (var dbContext = new Model.NoidaPMSEntities())
        //                {
        //                    DepartmentList = dbContext.UmUserDepartmentTrans.Where(u => u.UserRefId == userInfo.UserID && u.Status == true).Select(d => d.DepartmentId).ToList();
        //                }
        //            }
        //        }
        //    }
        //}

        //#region Nivesh Mitra Services Controller Methods

        //public ServiceRequestVM ReSubmitRequest(ServiceRequestVM model, IEnumerable<HttpPostedFileBase> files)
        //{
        //    var _ServiceRequestVM = new ServiceRequestVM();
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {
        //        var _service = dbContext.Customer_ServiceRequest.Where(m => m.Id == model.OnlineRequestId).FirstOrDefault();
        //        if (_service != null)
        //        {
        //            _service.Request_Status = NAStatusId.Resubmitted;
        //            _service.Modified_Date = DateTime.Now;
        //            _service.Modified_By = Convert.ToInt32(_service.Registration_No);
        //            //_service.ValidatorId = null;
        //            dbContext.SaveChanges();
        //            int _rid = Convert.ToInt32(_service.Registration_No);

        //            _ServiceRequestVM = GetNiveshMitraServicesByReqId(model.OnlineRequestId);

        //            //string message = string.Format(NAMessages.OnlineServiceRequest, _service.Id);
        //            //ApplicationHelper.SendSMS(_service.MobileNumber, message);
        //            //if (!string.IsNullOrEmpty(_service.Email)) ApplicationHelper.SendEmail(_service.Email, "Online Request", message);

        //            if (_service.Id > 0)
        //            {
        //                model.RegistrationId = _rid;
        //                model.ServiceModel = new ServiceVM
        //                {
        //                    RegistrationId = _rid
        //                };
        //                UploadFiles(model, files, _service.Id);
        //            }
        //        }
        //    }
        //    return _ServiceRequestVM;
        //}

        //public ServiceRequestVM SaveServiceRequestDetail(ServiceRequestVM model, IEnumerable<HttpPostedFileBase> files)
        //{
        //    var flag = ReturnType.None;
        //    switch (model.ServiceModel.ServiceId)
        //    {
        //        case NAService.Transfer:
        //            flag = SaveTransferRequestService(model, files);
        //            break;
        //        case NAService.Rent:
        //            flag = SaveRentRequestService(model, files);
        //            break;
        //        case NAService.CIC:
        //            flag = SaveCICRequestService(model, files);
        //            break;
        //        case NAService.Mortgage:
        //            flag = SaveMortgageRequestService(model, files);
        //            break;
        //        case NAService.Extension:
        //            flag = SaveExtensionRequestDetails(model, files);
        //            break;
        //        case NAService.GPA:
        //            flag = SaveGPARequestService(model, files);
        //            break;
        //        case NAService.Mutation:
        //            flag = SaveMutationRequestService(model, files);
        //            break;
        //        default:
        //            flag = SaveOtherRequestService(model, files);
        //            break;
        //    }
        //    model.ServiceModel.RequestId = flag;

        //    //update NIC table
        //    if (model.ServiceModel.RequestId > 0)
        //    {
        //        var _data = new WBasicDetailsModel_NMS();
        //        _data.TxtUnitID = model.NICModel.TxtUnitID;
        //        _data.TxtControlID = model.NICModel.TxtControlID;
        //        _data.TxtProcessIndustryID = model.NICModel.TxtProcessIndustryID;
        //        _data.TxtServiceID = model.NICModel.TxtServiceID;
        //        _data.TxtApplicationID = Convert.ToString(model.ServiceModel.RequestId);
        //        SaveNiveshMitraUnit(_data);
        //    }

        //    model.Status = flag;
        //    return model;
        //}

        //public ApplicantVM GetApplicantDetailsByRegistrationId(int? registrationId)
        //{
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {
        //        var applicant = (from alotment in dbContext.AllotmentMasters
        //                         join aplicant in dbContext.ApplicationDetails on alotment.rid equals aplicant.registrationId
        //                         join property in dbContext.SchemePropTrans on alotment.propertyId equals property.propertyId
        //                         where alotment.rid == registrationId && alotment.isActive == 1
        //                         select new ApplicantVM
        //                         {
        //                             RegistrationId = alotment.rid,
        //                             DepartmentId = alotment.departmentId,
        //                             Department = alotment.DepartmentMst.departmentName,
        //                             SectorId = property.sectorId,
        //                             Sector = property.SectorMst.sectorName,
        //                             BlockId = property.blockId,
        //                             Block = property.BlockMst.blockName,
        //                             PlotNo = property.propertyNo,
        //                             Applicant = aplicant.tGender == Constants.Company ? (!string.IsNullOrEmpty(aplicant.T_Company_Name) ? aplicant.T_Company_Name : aplicant.tFirstName) : aplicant.tFirstName + " " + (string.IsNullOrEmpty(aplicant.tMiddleName) ? aplicant.tLastName : aplicant.tMiddleName + " " + aplicant.tLastName),
        //                             Mobile = aplicant.tMobileNumber,
        //                             Email = aplicant.tEmail,
        //                             PermanentAddress = aplicant.tPermanentAdd,
        //                             CorrespondAddress = aplicant.tCorrespondanceAdd
        //                         }).FirstOrDefault();
        //        return applicant;
        //    }
        //}

        //public NiveshMitraMasterVM GetNiveshMitraServicesDetails(WBasicDetailsModel_NMS _WBasicDetailsModel_NMS)
        //{
        //    var _ServiceRequestVM = new NiveshMitraMasterVM();
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {
        //        _ServiceRequestVM = (from nmservice in dbContext.NiveshMitraEntr_Master
        //                             where nmservice.ControlID == _WBasicDetailsModel_NMS.TxtControlID && nmservice.UnitID == _WBasicDetailsModel_NMS.TxtUnitID && nmservice.ServiceID == _WBasicDetailsModel_NMS.TxtServiceID
        //                             && nmservice.Status == true
        //                             select new NiveshMitraMasterVM
        //                             {
        //                                 ApplicationID = nmservice.ApplicationID,
        //                                 ControlID = nmservice.ControlID,
        //                                 UnitID = nmservice.UnitID,
        //                                 ServiceID = nmservice.ServiceID,
        //                                 ProcessIndustryID = nmservice.ProcessIndustryID,
        //                                 Sector = nmservice.Sector,
        //                                 Block = nmservice.Block,
        //                                 PlotNo = nmservice.PlotNo,
        //                                 MobileNo = nmservice.MobileNo,
        //                                 Status = nmservice.Status
        //                             }).FirstOrDefault();

        //    }
        //    return _ServiceRequestVM;
        //}

        //public NiveshMitraMasterVM GetNiveshMitraServicesDetailsByApplicationId(int applicationId)
        //{
        //    var _ServiceRequestVM = new NiveshMitraMasterVM();
        //    var _appId = Convert.ToString(applicationId);
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {
        //        _ServiceRequestVM = (from nmservice in dbContext.NiveshMitraEntr_Master
        //                             where nmservice.ApplicationID == _appId
        //                             && nmservice.Status == true
        //                             select new NiveshMitraMasterVM
        //                             {
        //                                 ApplicationID = nmservice.ApplicationID,
        //                                 ControlID = nmservice.ControlID,
        //                                 UnitID = nmservice.UnitID,
        //                                 ServiceID = nmservice.ServiceID,
        //                                 ProcessIndustryID = nmservice.ProcessIndustryID,
        //                                 Sector = nmservice.Sector,
        //                                 Block = nmservice.Block,
        //                                 PlotNo = nmservice.PlotNo,
        //                                 MobileNo = nmservice.MobileNo,
        //                                 Status = nmservice.Status
        //                             }).FirstOrDefault();

        //    }
        //    return _ServiceRequestVM;
        //}

        //public ServiceRequestVM GetNiveshMitraServicesByRegistrationId(WBasicDetailsModel_NMS apimodel)
        //{
        //    var _ServiceRequestVM = new ServiceRequestVM();
        //    _ServiceRequestVM.TxtControlID = apimodel.TxtControlID;
        //    _ServiceRequestVM.TxtUnitID = apimodel.TxtUnitID;
        //    _ServiceRequestVM.TxtServiceID = apimodel.TxtServiceID;
        //    _ServiceRequestVM.TxtProcessIndustryID = apimodel.TxtProcessIndustryID;
        //    _ServiceRequestVM.TxtApplicationID = apimodel.TxtApplicationID;
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {
        //        int _registrationId = !string.IsNullOrEmpty(apimodel.TxtProcessIndustryID) ? Convert.ToInt32(apimodel.TxtProcessIndustryID) : 0;
        //        var _ApplicantVM = new ApplicantVM();
        //        _ApplicantVM = GetApplicantDetailsByRegistrationId(_registrationId);

        //        var _rid = Convert.ToString(_registrationId);

        //        _ServiceRequestVM = (from nmservice in dbContext.NiveshMitraEntr_Master
        //                             where nmservice.ControlID == apimodel.TxtControlID && nmservice.UnitID == apimodel.TxtUnitID
        //                             && nmservice.ProcessIndustryID == _rid && nmservice.ServiceID == apimodel.TxtServiceID
        //                             select new ServiceRequestVM
        //                             {
        //                                 RegistrationId = _ApplicantVM.RegistrationId,
        //                                 ServiceModel = new ServiceVM
        //                                 {
        //                                     RegistrationId = _ApplicantVM.RegistrationId,
        //                                     DepartmentId = _ApplicantVM.DepartmentId,
        //                                 },
        //                                 NICModel = new WBasicDetailsModel_NMS
        //                                 {
        //                                     TxtControlID = nmservice.ControlID,
        //                                     TxtApplicationID = nmservice.ApplicationID,
        //                                     TxtUnitID = nmservice.UnitID,
        //                                     TxtServiceID = nmservice.ServiceID,
        //                                     TxtProcessIndustryID = nmservice.ProcessIndustryID
        //                                 }
        //                             }).FirstOrDefault();

        //        if (!string.IsNullOrEmpty(_ServiceRequestVM.NICModel.TxtServiceID))
        //        {
        //            _ServiceRequestVM.ServiceModel.ServiceId = GetServiceByUnitServiceID(_ServiceRequestVM.NICModel.TxtServiceID, _ServiceRequestVM.ServiceModel.DepartmentId);
        //            // in case of NDC & calculation of Dues sub dept. is account 
        //            if (_ServiceRequestVM.NICModel.TxtServiceID == ServiceIds_NIC.NoDuesCertificate || _ServiceRequestVM.NICModel.TxtServiceID == ServiceIds_NIC.CalculationOfDues)
        //            {
        //                if (_ServiceRequestVM.ServiceModel.DepartmentId == NADepartment.Institutional) { _ServiceRequestVM.ServiceModel.SubDepartmentId = 2; }
        //                if (_ServiceRequestVM.ServiceModel.DepartmentId == NADepartment.Industrial) { _ServiceRequestVM.ServiceModel.SubDepartmentId = 8; }
        //            }
        //        }
        //    }
        //    return _ServiceRequestVM;
        //}

        //public ServiceRequestVM GetNiveshMitraServicesByRequestId(int? RequestId)
        //{
        //    var _ServiceRequestVM = new ServiceRequestVM();
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {
        //        int _requestId = RequestId > 0 ? Convert.ToInt32(RequestId) : 0;
        //        var _ApplicantVM = new ApplicantVM();
        //        _ApplicantVM = GetApplicantDetailsByRegistrationId(_requestId);

        //        var _rid = Convert.ToString(_requestId);

        //        _ServiceRequestVM = (from nmservice in dbContext.NiveshMitraEntr_Master
        //                             where nmservice.ProcessIndustryID == _rid
        //                             select new ServiceRequestVM
        //                             {
        //                                 RegistrationId = _ApplicantVM.RegistrationId,
        //                                 ServiceModel = new ServiceVM
        //                                 {
        //                                     RegistrationId = _ApplicantVM.RegistrationId,
        //                                     DepartmentId = _ApplicantVM.DepartmentId,
        //                                 },
        //                                 NICModel = new WBasicDetailsModel_NMS
        //                                 {
        //                                     TxtControlID = nmservice.ControlID,
        //                                     TxtApplicationID = nmservice.ApplicationID,
        //                                     TxtUnitID = nmservice.UnitID,
        //                                     TxtServiceID = nmservice.ServiceID,
        //                                     TxtProcessIndustryID = nmservice.ProcessIndustryID
        //                                 }
        //                             }).FirstOrDefault();

        //        if (!string.IsNullOrEmpty(_ServiceRequestVM.NICModel.TxtServiceID))
        //        {
        //            _ServiceRequestVM.ServiceModel.ServiceId = GetServiceByUnitServiceID(_ServiceRequestVM.NICModel.TxtServiceID, _ServiceRequestVM.ServiceModel.DepartmentId);

        //            // in case of NDC sub dept. is account
        //            if (_ServiceRequestVM.NICModel.TxtServiceID == ServiceIds_NIC.NoDuesCertificate)
        //            {
        //                if (_ServiceRequestVM.ServiceModel.DepartmentId == NADepartment.Institutional) { _ServiceRequestVM.ServiceModel.SubDepartmentId = 2; }
        //                if (_ServiceRequestVM.ServiceModel.DepartmentId == NADepartment.Industrial) { _ServiceRequestVM.ServiceModel.SubDepartmentId = 8; }
        //            }
        //        }
        //    }
        //    return _ServiceRequestVM;
        //}

        //public ServiceRequestVM GetNiveshMitraServicesByReqId(int? RequestId)
        //{
        //    var _ServiceRequestVM = new ServiceRequestVM();
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {
        //        int _requestId = RequestId > 0 ? Convert.ToInt32(RequestId) : 0;

        //        var _ApplicantVM = new ApplicantVM();
        //        var _rid = Convert.ToString(_requestId);

        //        _ServiceRequestVM = (from nmservice in dbContext.NiveshMitraEntr_Master
        //                             where nmservice.ApplicationID == _rid
        //                             select new ServiceRequestVM
        //                             {
        //                                 NICModel = new WBasicDetailsModel_NMS
        //                                 {
        //                                     TxtControlID = nmservice.ControlID,
        //                                     TxtApplicationID = nmservice.ApplicationID,
        //                                     TxtUnitID = nmservice.UnitID,
        //                                     TxtServiceID = nmservice.ServiceID,
        //                                     TxtProcessIndustryID = nmservice.ProcessIndustryID
        //                                 }
        //                             }).FirstOrDefault();
        //    }
        //    return _ServiceRequestVM;
        //}

        ///// <summary>
        ///// Validate registration Id [Check RID Exist and KYA is done]
        ///// </summary>
        ///// <param name="registrationId"></param>
        ///// <returns>ApplicantVM</returns>
        //public ApplicantVM ValidateRegistrationId(int? registrationId)
        //{
        //    var _ApplicantVM = new ApplicantVM();
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {
        //        var IsRidExists = dbContext.AllotmentMasters.Where(m => m.rid == registrationId && m.isActive==1).FirstOrDefault();
        //        if (IsRidExists != null)
        //        {
        //            var _IsKYADone = dbContext.KYADetails.Where(m => m.RId == registrationId && m.IsActive == true && m.StatusId == NAStatusId.Approved).FirstOrDefault();
        //            if (_IsKYADone != null)
        //            {
        //                _ApplicantVM = GetApplicantDetailsByRegistrationId(registrationId);
        //                _ApplicantVM.IsKYADone = true;
        //            }
        //            else
        //            {
        //                _ApplicantVM.RegistrationId = registrationId;
        //                _ApplicantVM.IsKYADone = false;
        //            }
        //        }
        //        else
        //        {
        //            _ApplicantVM.RegistrationId = 0;
        //        }
        //    }
        //    return _ApplicantVM;
        //}

        //public bool SaveNiveshMitraUnit(WBasicDetailsModel_NMS model)
        //{
        //    var flag = false;
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {
        //        if (!string.IsNullOrEmpty(model.TxtControlID) && !string.IsNullOrEmpty(model.TxtUnitID))
        //        {
        //            var _nsMaster = dbContext.NiveshMitraEntr_Master.FirstOrDefault(c => c.UnitID == model.TxtUnitID && c.ControlID == model.TxtControlID && c.ServiceID == model.TxtServiceID);
        //            if (_nsMaster == null)
        //            {
        //                var _ApplicantVM = new ApplicantVM();
        //                if (!string.IsNullOrEmpty(model.TxtProcessIndustryID))
        //                {
        //                    int rid = Convert.ToInt32(model.TxtProcessIndustryID);
        //                    _ApplicantVM = GetApplicantDetailsByRegistrationId(rid);
        //                }

        //                if (_ApplicantVM != null)
        //                {
        //                    var niveshMitramaster = new NiveshMitraEntr_Master();
        //                    niveshMitramaster.ControlID = model.TxtControlID;
        //                    niveshMitramaster.UnitID = model.TxtUnitID;
        //                    niveshMitramaster.ServiceID = model.TxtServiceID;
        //                    niveshMitramaster.ProcessIndustryID = model.TxtProcessIndustryID;
        //                    niveshMitramaster.Sector = _ApplicantVM.Sector;
        //                    niveshMitramaster.Block = _ApplicantVM.Block;
        //                    niveshMitramaster.PlotNo = _ApplicantVM.PlotNo;
        //                    niveshMitramaster.MobileNo = _ApplicantVM.Mobile;
        //                    niveshMitramaster.Status = true;
        //                    niveshMitramaster.CreatedBy = userInfo.UserID;
        //                    niveshMitramaster.CreatedDate = DateTime.Now;
        //                    dbContext.NiveshMitraEntr_Master.Add(niveshMitramaster);
        //                    dbContext.SaveChanges();
        //                    flag = true;
        //                }
        //            }
        //            else
        //            {
        //                _nsMaster.ApplicationID = model.TxtApplicationID;
        //                _nsMaster.ModifiedBy = userInfo.UserID;
        //                _nsMaster.ModifiedDate = DateTime.Now;
        //                dbContext.SaveChanges();
        //            }
        //        }
        //    }
        //    return flag;

        //}

        //#region Service Request Documents

        //public string GetFileUploadHtmlForService(int? departmentId, int? serviceId)
        //{
        //    using (var context = new NoidaPMSEntities())
        //    {
        //        var checklists = (from checklist in context.ServiceCheckList_Master
        //                          where checklist.dept_id == departmentId && checklist.service_id == serviceId
        //                          select new ServiceCheckListVM
        //                          {
        //                              Id = checklist.ChkId,
        //                              DepartmentId = checklist.dept_id,
        //                              Department = context.DepartmentMsts.Where(d => d.departmentId == departmentId).Select(d => d.departmentName).FirstOrDefault(),
        //                              ServiceId = checklist.service_id,
        //                              ChecklistRefNo = checklist.Checklist_Ref,
        //                              ChecklistName = checklist.ChkName,
        //                              Status = checklist.Status
        //                          }).ToList();
        //        string divMain = "";
        //        if (checklists.Count != 0)
        //        {
        //            divMain = "<div class='row  file-header'> "
        //                            + "<div class='col-md-3 col-lbl-sn'><label>Serial No</label></div>"
        //                            + "<div class='col-md-6 col-lbl'><label>Required File</label></div>"
        //                            + "<div class='col-md-3 col-lbl-vl'><small>only pdf, jpg & jpeg file</small></div>"
        //                        + "</div>";

        //            int docCounter = 1;
        //            foreach (var item in checklists)
        //            {
        //                divMain = divMain + "<div class='row  row-border row-file'> "
        //                                       + "<div class='col-md-3 col-lbl-sn'><label>" + docCounter + "</label></div>"
        //                                       + "<div class='col-md-6 col-lbl'><label>" + item.ChecklistName + "</label></div>"
        //                                       + "<div class='col-md-3 col-lbl-vl'><input type='file' class='single' name='files' /></div>"
        //                                  + "</div>";
        //                docCounter++;
        //            }
        //            return divMain;
        //        }
        //        else return null;
        //        //return divMain;
        //    }
        //}

        //public DataSourceResult GetServiceRequestUploadedDocumentsById(DataSourceRequest request, ServiceVM model)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        List<DocumentVM> documentList = new List<DocumentVM>();
        //        FtpHandler ftpHandler = new FtpHandler();
        //        var service = dbContext.Customer_ServiceRequest.FirstOrDefault(f => f.Id == model.RequestId);
        //        if (model.RegistrationId == null)
        //        {
        //            if (service != null)
        //            {
        //                documentList = ftpHandler.GetServiceRequestDocuments_NIC((int)model.RequestId);
        //                return documentList.ToDataSourceResult(request);
        //            }
        //            else return null;
        //        }
        //        else
        //        {
        //            documentList = ftpHandler.GetServiceRequestUploadedDocumentsById_NIC((int)model.RegistrationId, (int)model.RequestId);
        //            return documentList.ToDataSourceResult(request);
        //        }
        //    }
        //}

        //#endregion

        //#region [Private Methods]

        //private int? GetServiceByUnitServiceID(string ServiceID, int? DeptID)
        //{
        //    int? _serviceId = 0;
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {

        //        var _serviceDetails = (from nm_services in dbContext.Services
        //                               join nm_serviceTrans in dbContext.ServiceTrans on nm_services.Id.ToString() equals nm_serviceTrans.ServiceId
        //                               join customerservice in dbContext.CitizenService_Master on nm_serviceTrans.CitizenServiceId equals customerservice.Id
        //                               where nm_services.Status == 1 && customerservice.Status == 1
        //                               && nm_services.ServiceCode == ServiceID && customerservice.Deptt_Id == DeptID
        //                               select customerservice).FirstOrDefault();

        //        if (_serviceDetails != null)
        //        {
        //            _serviceId = _serviceDetails.service_id;
        //        }
        //    }
        //    return _serviceId;
        //}

        //public CitizenServiceRequest GetCitizenServiceDetails(int? ServiceId, int? DeptId)
        //{
        //    var _details = new CitizenServiceRequest();
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {
        //        _details = (from service in dbContext.CitizenService_Master
        //                    where service.Status == 1 && service.service_id == ServiceId && service.Deptt_Id == DeptId
        //                    select new CitizenServiceRequest
        //                    {
        //                        ServiceId = service.service_id,
        //                        ServiceName = service.ServiceName,
        //                        DepartmentId = service.Deptt_Id > 0 ? (int)service.Deptt_Id : 0,
        //                        ServiceFee = service.Amount != null && service.Amount > 0 ? service.Amount : 0
        //                    }).FirstOrDefault();
        //    }
        //    return _details;
        //}

        //private int SaveCustomerServiceRequestDetail(ServiceRequestVM model)
        //{
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {
        //        if (model.Id != null && model.Id > 0)
        //        {
        //            var exService = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == model.Id);
        //            if (exService != null)
        //            {
        //                exService.Registration_No = model.ServiceModel.RegistrationId.ToString();
        //                exService.Property_No = !string.IsNullOrEmpty(model.ServiceModel.PropertyNo) ? model.ServiceModel.PropertyNo : model.ServiceModel.Sector + "/" + model.ServiceModel.Block + "-" + model.ServiceModel.PlotNo;
        //                exService.ServiceId = model.ServiceModel.ServiceId;
        //                exService.ServiceType = Constants.NIC_NiveshMitra;
        //                exService.DepartmentId = model.ServiceModel.DepartmentId;
        //                exService.SubDepartment = model.ServiceModel.SubDepartment;
        //                exService.ApplicantName = model.ServiceModel.Applicant;
        //                exService.MobileNumber = model.ServiceModel.MobileNo;
        //                exService.Email = model.ServiceModel.Email;
        //                exService.ApplicantAddress = model.ServiceModel.ApplicantAddress;
        //                exService.Description = model.ServiceModel.Description;
        //                exService.IsActive = true;
        //                exService.RequestorName = model.ServiceModel.Requestor ?? model.ServiceModel.Applicant;
        //                exService.RequestorAddress = model.ServiceModel.RequestorAddress ?? model.ServiceModel.ApplicantAddress;
        //                exService.Modified_Date = DateTime.Now;
        //                exService.Modified_By = userInfo.UserID;
        //                dbContext.SaveChanges();

        //                return exService.Id;
        //            }
        //            else return (int)model.Id;
        //        }
        //        else
        //        {
        //            Customer_ServiceRequest request = new Customer_ServiceRequest
        //            {
        //                Registration_No = model.ServiceModel.RegistrationId.ToString(),
        //                Property_No = model.ServiceModel.Sector + "/" + model.ServiceModel.Block + "-" + model.ServiceModel.PlotNo,
        //                ServiceId = model.ServiceModel.ServiceId,
        //                ServiceType = Constants.NIC_NiveshMitra,
        //                DepartmentId = model.ServiceModel.DepartmentId,
        //                SubDepartment = model.ServiceModel.SubDepartment,
        //                ApplicantName = model.ServiceModel.Applicant,
        //                MobileNumber = model.ServiceModel.MobileNo,
        //                Email = model.ServiceModel.Email,
        //                ApplicantAddress = model.ServiceModel.ApplicantAddress,
        //                Description = model.ServiceModel.Description,
        //                IsActive = true,
        //                Request_Status = NAStatusId.Initiated,
        //                RequestorName = model.ServiceModel.Requestor ?? model.ServiceModel.Applicant,
        //                RequestorAddress = model.ServiceModel.RequestorAddress ?? model.ServiceModel.ApplicantAddress,
        //                Created_Date = DateTime.Now,
        //                Created_By = userInfo.UserID,
        //                RequestThrough = Constants.NIC_NiveshMitra
        //            };

        //            // in case if service has fee
        //            var _CitizenMaster = dbContext.CitizenService_Master.Where(m => m.Status == 1 && m.service_id == model.ServiceModel.ServiceId && m.Deptt_Id == model.ServiceModel.DepartmentId).FirstOrDefault();
        //            if (_CitizenMaster != null)
        //            {
        //                if (_CitizenMaster.Amount > 0)
        //                {
        //                    request.PaymentStatus = 0;// not paid
        //                }
        //            }

        //            dbContext.Customer_ServiceRequest.Add(request);
        //            dbContext.SaveChanges();

        //            model.Id = request.Id;
        //            model.RegistrationId = model.ServiceModel.RegistrationId;
        //            model.ServiceModel.Id = request.Id;
        //            model.ServiceModel.PropertyNo = request.Property_No;
        //            model.ServiceModel.PropertyNo = request.Property_No;
        //            model.ServiceModel.CreatedDate = DateTime.Now;

        //            string message = string.Format(NAMessages.OnlineServiceRequest, request.Id);
        //            ApplicationHelper.SendSMS(request.MobileNumber, message);
        //            if (!string.IsNullOrEmpty(request.Email)) ApplicationHelper.SendEmail(request.Email, "Online Request", message);

        //            return request.Id;
        //        }
        //    }
        //}

        //public int UpdateRequestPaymentStatus(ServiceRequestVM model)
        //{
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {
        //        if (model.Id != null && model.Id > 0)
        //        {
        //            var exService = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == model.Id);
        //            if (exService != null)
        //            {
        //                exService.PaymentStatus = 1;
        //                exService.Modified_Date = DateTime.Now;
        //                exService.Modified_By = userInfo.UserID;
        //                dbContext.SaveChanges();
        //                return exService.Id;
        //            }
        //        }
        //    }
        //    return 0;
        //}

        //private bool UploadFiles(ServiceRequestVM model, IEnumerable<HttpPostedFileBase> files, int requestNo)
        //{
        //    if (files != null)
        //    {
        //        if (files.ToList().Count > 0)
        //        {
        //            return FtpHandler.UploadFiles(files, model.ServiceModel.RegistrationId.ToString(), requestNo);
        //        }
        //    }
        //    return false;
        //}

        //private int SaveMortgageRequestService(ServiceRequestVM model, IEnumerable<HttpPostedFileBase> files)
        //{
        //    int requestNo = 0;
        //    if (model != null)
        //    {
        //        requestNo = SaveCustomerServiceRequestDetail(model);
        //        if (requestNo > 0)
        //        {
        //            SaveMortgageRequest(model, requestNo);
        //            UploadFiles(model, files, requestNo);
        //        }
        //    }
        //    return requestNo;
        //}

        //private int SaveOtherRequestService(ServiceRequestVM model, IEnumerable<HttpPostedFileBase> files)
        //{
        //    int requestNo = 0;
        //    if (model != null)
        //    {
        //        requestNo = SaveCustomerServiceRequestDetail(model);
        //        if (requestNo > 0)
        //        {
        //            UploadFiles(model, files, requestNo);
        //        }
        //    }
        //    return requestNo;
        //}

        //private int SaveMutationRequestService(ServiceRequestVM model, IEnumerable<HttpPostedFileBase> files)
        //{
        //    int requestNo = 0;
        //    if (model != null)
        //    {
        //        requestNo = SaveCustomerServiceRequestDetail(model);
        //        if (requestNo > 0)
        //        {
        //            UploadFiles(model, files, requestNo);
        //        }
        //    }
        //    return requestNo;
        //}

        //private int SaveGPARequestService(ServiceRequestVM model, IEnumerable<HttpPostedFileBase> files)
        //{
        //    int requestNo = 0;
        //    if (model != null)
        //    {
        //        requestNo = SaveCustomerServiceRequestDetail(model);
        //        if (requestNo > 0)
        //        {
        //            UploadFiles(model, files, requestNo);
        //        }
        //    }
        //    return requestNo;
        //}

        //private int SaveExtensionRequestDetails(ServiceRequestVM model, IEnumerable<HttpPostedFileBase> files)
        //{
        //    int requestNo = 0;
        //    if (model != null)
        //    {
        //        requestNo = SaveCustomerServiceRequestDetail(model);
        //        if (requestNo > 0)
        //        {
        //            SaveExtensionRequest(model, requestNo);

        //            UploadFiles(model, files, requestNo);
        //        }
        //    }
        //    return requestNo;
        //}

        //private int SaveCICRequestService(ServiceRequestVM model, IEnumerable<HttpPostedFileBase> files)
        //{
        //    int requestNo = 0;
        //    if (model != null)
        //    {
        //        requestNo = SaveCustomerServiceRequestDetail(model);
        //        if (requestNo > 0)
        //        {
        //            UploadFiles(model, files, requestNo);
        //        }
        //    }
        //    return requestNo;
        //}

        //private int SaveRentRequestService(ServiceRequestVM model, IEnumerable<HttpPostedFileBase> files)
        //{
        //    int requestNo = 0;
        //    if (model != null)
        //    {
        //        requestNo = SaveCustomerServiceRequestDetail(model);
        //        if (requestNo > 0)
        //        {
        //            SaveRentRequest(model, requestNo);

        //            UploadFiles(model, files, requestNo);
        //        }
        //    }
        //    return requestNo;
        //}

        //private int SaveTransferRequestService(ServiceRequestVM model, IEnumerable<HttpPostedFileBase> files)
        //{
        //    int requestNo = 0;
        //    if (model != null)
        //    {
        //        requestNo = SaveCustomerServiceRequestDetail(model);
        //        if (requestNo > 0)
        //        {
        //            // Transfer Request
        //            SaveTransferRequest(model, requestNo);

        //            if (requestNo > 0)
        //            {
        //                UploadFiles(model, files, requestNo);
        //            }
        //        }
        //    }
        //    return requestNo;
        //}

        //private int SaveTransferRequest(ServiceRequestVM model, int requestNo)
        //{
        //    var flag = ReturnType.None;
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {
        //        if (model.Id != null && model.Id > 0)
        //        {
        //            var exTransfer = dbContext.OnlineTransferMutations.FirstOrDefault(t => t.RequestNo == requestNo);
        //            if (exTransfer != null)
        //            {
        //                exTransfer.Rid = model.ServiceModel.RegistrationId;
        //                exTransfer.Type = "T"; // Constants.Transfer;
        //                exTransfer.Transfer_Type = model.TransferModel.TransferTypeId;
        //                exTransfer.Transfer_Sub_Type = model.TransferModel.TransferSubTypeId;

        //                if (model.TransferModel.TypeOfTransferee == Constants.Company)
        //                {
        //                    exTransfer.T_Gender = "Company";
        //                    exTransfer.T_Company_Name = model.TransferModel.CompanyName;
        //                    exTransfer.T_Signing_Authority = model.TransferModel.SigningAuthority;
        //                    exTransfer.T_Registered_Office = model.TransferModel.RegisteredOffice;
        //                }
        //                else
        //                {
        //                    exTransfer.T_Gender = model.TransferModel.Gender;
        //                    exTransfer.T_First_Name = model.TransferModel.FirstName;
        //                    exTransfer.T_Middle_Name = model.TransferModel.MiddleName;
        //                    exTransfer.T_Last_Name = model.TransferModel.LastName;
        //                    exTransfer.T_Father_Husband_Name = model.TransferModel.ApplicantMaster;
        //                    exTransfer.T_Mother_Name = model.TransferModel.MotherName;
        //                }
        //                exTransfer.T_Email = model.TransferModel.Email;
        //                exTransfer.T_Mobile = model.TransferModel.Mobile;
        //                exTransfer.T_Correspondence_Add = model.TransferModel.CorrespondenceAdd;
        //                exTransfer.T_Permanent_Add = model.TransferModel.PermanentAdd;
        //                exTransfer.T_Occupation_Id = model.TransferModel.OccupationId;
        //                exTransfer.T_Pan = model.TransferModel.PAN;
        //                exTransfer.Applicant_Name = model.ServiceModel.Applicant;
        //                exTransfer.Correspondance_Add = model.ServiceModel.ApplicantAddress;
        //                exTransfer.Modified_By = userInfo.UserID;
        //                exTransfer.Modified_Date = DateTime.Now;
        //                dbContext.SaveChanges();

        //                flag = ReturnType.Success;
        //            }
        //            else
        //            {
        //                model.Id = requestNo;
        //                var transfer = new OnlineTransferMutation();
        //                transfer.Rid = model.ServiceModel.RegistrationId;
        //                transfer.RequestNo = requestNo;
        //                transfer.Type = "T"; // Constants.Transfer;
        //                transfer.Transfer_Type = model.TransferModel.TransferTypeId;
        //                transfer.Transfer_Sub_Type = model.TransferModel.TransferSubTypeId;
        //                transfer.Created_Date = DateTime.Now;
        //                if (model.TransferModel.TypeOfTransferee == Constants.Company)
        //                {
        //                    transfer.T_Gender = "Company";
        //                    transfer.T_Signing_Authority = model.TransferModel.SigningAuthority;
        //                    transfer.T_Registered_Office = model.TransferModel.RegisteredOffice;
        //                }
        //                else
        //                {
        //                    transfer.T_Gender = model.TransferModel.Gender;
        //                    transfer.T_First_Name = model.TransferModel.FirstName;
        //                    transfer.T_Middle_Name = model.TransferModel.MiddleName;
        //                    transfer.T_Last_Name = model.TransferModel.LastName;
        //                    transfer.T_Father_Husband_Name = model.TransferModel.ApplicantMaster;
        //                    transfer.T_Mother_Name = model.TransferModel.MotherName;
        //                }
        //                transfer.T_Email = model.TransferModel.Email;
        //                transfer.T_Mobile = model.TransferModel.Mobile;
        //                transfer.T_Correspondence_Add = model.TransferModel.CorrespondenceAdd;
        //                transfer.T_Permanent_Add = model.TransferModel.PermanentAdd;
        //                transfer.T_Occupation_Id = model.TransferModel.OccupationId;
        //                transfer.T_Pan = model.TransferModel.PAN;
        //                transfer.Applicant_Name = model.ServiceModel.Applicant;
        //                transfer.Correspondance_Add = model.ServiceModel.ApplicantAddress;
        //                transfer.Created_By = userInfo.UserID;
        //                transfer.Created_Date = DateTime.Now;
        //                dbContext.OnlineTransferMutations.Add(transfer);
        //                dbContext.SaveChanges();

        //                flag = ReturnType.Success;
        //            }
        //        }
        //    }
        //    return flag;
        //}

        //private int SaveRentRequest(ServiceRequestVM model, int requestNo)
        //{
        //    var flag = ReturnType.None;
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var exRent = dbContext.OnlineRentPermissionDetails.FirstOrDefault(r => r.RequestNo == requestNo);
        //        if (exRent != null)
        //        {
        //            exRent.Rid = model.ServiceModel.RegistrationId;
        //            exRent.TenantName = model.RentModel.TenantName;
        //            exRent.TenantProject = model.RentModel.TenantProject;
        //            exRent.RentDurationYears = model.RentModel.RentDuration;
        //            exRent.RentingDate = model.RentModel.RentingDate;
        //            exRent.IsActive = true;
        //            exRent.Comment = exRent.Comment + "</br>" + model.ServiceModel.Description;
        //            exRent.CommentDate = DateTime.Now;
        //            exRent.ModifiedDate = DateTime.Now;
        //            exRent.Modifiedby = userInfo.UserID;
        //            dbContext.SaveChanges();
        //            flag = ReturnType.Success;
        //        }
        //        else
        //        {
        //            model.Id = requestNo;
        //            var rent = new OnlineRentPermissionDetail();
        //            rent.Rid = model.ServiceModel.RegistrationId;
        //            rent.RequestNo = requestNo;
        //            rent.TenantName = model.RentModel.TenantName;
        //            rent.TenantProject = model.RentModel.TenantProject;
        //            rent.RentDurationYears = model.RentModel.RentDuration;
        //            rent.RentingDate = model.RentModel.RentingDate;
        //            rent.IsActive = true;
        //            rent.CreatedDate = DateTime.Now;
        //            rent.StatusId = NAStatusId.Initiated;
        //            rent.Comment = model.ServiceModel.Description;
        //            rent.CommentDate = DateTime.Now;
        //            rent.CreatedBy = userInfo.UserID;
        //            dbContext.OnlineRentPermissionDetails.Add(rent);
        //            dbContext.SaveChanges();
        //            flag = ReturnType.Success;
        //        }

        //        return flag;
        //    }
        //}

        //private int SaveMortgageRequest(ServiceRequestVM model, int requestNo)
        //{
        //    var flag = ReturnType.None;
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var exMortgage = dbContext.OnlineMortgageDetails.FirstOrDefault(m => m.RequestNo == requestNo);
        //        if (exMortgage != null)
        //        {
        //            exMortgage.RID = model.ServiceModel.RegistrationId;
        //            exMortgage.RequestNo = requestNo;
        //            exMortgage.BankName = model.MortgageModel.BankName;
        //            exMortgage.BranchAddress = model.MortgageModel.BranchAddress;
        //            exMortgage.SanctionedAmount = model.MortgageModel.SanctionedAmount;
        //            exMortgage.MortgageType = model.MortgageModel.MortgageType;
        //            exMortgage.PreviousLoanNoc = model.MortgageModel.PreviousLoanNoc;
        //            exMortgage.IsActive = true;
        //            exMortgage.Comment = model.ServiceModel.Description;
        //            exMortgage.CommentDate = DateTime.Now;
        //            exMortgage.ModifiedDate = DateTime.Now;
        //            exMortgage.Modifiedby = userInfo.UserID;
        //            dbContext.SaveChanges();
        //            flag = ReturnType.Success;
        //        }
        //        else
        //        {
        //            model.Id = requestNo;
        //            var mortgage = new OnlineMortgageDetail();
        //            mortgage.RID = model.ServiceModel.RegistrationId;
        //            mortgage.RequestNo = requestNo;
        //            mortgage.BankName = model.MortgageModel.BankName;
        //            mortgage.BranchAddress = model.MortgageModel.BranchAddress;
        //            mortgage.SanctionedAmount = model.MortgageModel.SanctionedAmount;
        //            mortgage.MortgageType = model.MortgageModel.MortgageType;
        //            mortgage.PreviousLoanNoc = model.MortgageModel.PreviousLoanNoc;
        //            mortgage.IsActive = true;
        //            mortgage.CreatedDate = DateTime.Now;
        //            mortgage.CreatedBy = userInfo.UserID;
        //            mortgage.Comment = model.ServiceModel.Description;
        //            mortgage.CommentDate = DateTime.Now;
        //            mortgage.StatusId = NAStatusId.Initiated;

        //            dbContext.OnlineMortgageDetails.Add(mortgage);
        //            dbContext.SaveChanges();
        //            flag = ReturnType.Success;
        //        }
        //    }
        //    return flag;
        //}

        //private int SaveExtensionRequest(ServiceRequestVM model, int requestNo)
        //{
        //    var flag = ReturnType.None;
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var exExtension = dbContext.OnlineExtensionDetails.FirstOrDefault(e => e.RequestNo == requestNo);
        //        if (exExtension != null)
        //        {
        //            exExtension.Rid = model.ServiceModel.RegistrationId;
        //            exExtension.RequestNo = requestNo;
        //            exExtension.ExtensionDueDate = model.ExtensionModel.ExtensionDueDate;
        //            exExtension.ExtensionGivenDate = model.ExtensionModel.ExtensionGivenDate;
        //            exExtension.IsActive = true;
        //            exExtension.ModifiedDate = DateTime.Now;
        //            exExtension.ModifiedBy = userInfo.UserID;
        //            exExtension.Comment = model.ServiceModel.Description;
        //            dbContext.SaveChanges();
        //            flag = ReturnType.Success;
        //        }
        //        else
        //        {
        //            model.Id = requestNo;
        //            OnlineExtensionDetail extension = new OnlineExtensionDetail
        //            {
        //                Rid = model.ServiceModel.RegistrationId,
        //                RequestNo = requestNo,
        //                ExtensionDueDate = model.ExtensionModel.ExtensionDueDate,
        //                ExtensionGivenDate = model.ExtensionModel.ExtensionGivenDate,
        //                Status = NAStatusId.Initiated,
        //                IsActive = true,
        //                CreatedDate = DateTime.Now,
        //                CreatedBy = userInfo.UserID,
        //                Comment = model.ServiceModel.Description
        //            };

        //            dbContext.OnlineExtensionDetails.Add(extension);
        //            dbContext.SaveChanges();
        //            flag = ReturnType.Success;
        //        }
        //        return flag;
        //    }
        //}

        //#endregion

        //#region

        //public ServiceRequestVM GetServiceRequestDetailById(int? id)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var model = new ServiceRequestVM();

        //        var service = (from csr in dbContext.Customer_ServiceRequest
        //                       where csr.Id == id
        //                       select new ServiceVM
        //                       {
        //                           Id = csr.Id,
        //                           RequestId = csr.Id,
        //                           RegistrationNo = csr.Registration_No,
        //                           Applicant = csr.ApplicantName,
        //                           ApplicantAddress = csr.ApplicantAddress,
        //                           Requestor = csr.RequestorName,
        //                           RequestorAddress = csr.RequestorAddress,
        //                           PropertyNo = csr.Property_No,
        //                           MobileNo = csr.MobileNumber,
        //                           Email = csr.Email,
        //                           ServiceId = csr.ServiceId,
        //                           ServiceName = dbContext.CitizenService_Master.FirstOrDefault(x => x.Deptt_Id == csr.DepartmentId && x.service_id == csr.ServiceId && x.Status == 1).ServiceName,
        //                           DepartmentId = csr.DepartmentId,
        //                           Department = dbContext.DepartmentMsts.FirstOrDefault(x => x.departmentId == csr.DepartmentId && x.IsActive == true).departmentName,
        //                           SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
        //                           SubDepartmentId = string.IsNullOrEmpty(csr.SubDepartment) ? 1 : dbContext.SubDepartmentMsts.FirstOrDefault(x => x.departmentId == csr.DepartmentId && x.SubdepartmentName == csr.SubDepartment && x.IsActive == true).SubdepartmentId,
        //                           Description = csr.Description,
        //                           RegistrationType = csr.ServiceType == Constants.SDService ? Constants.PradhikaranDiwas : (string.IsNullOrEmpty(csr.Registration_No) ? Constants.UnRegistered : Constants.Registered),
        //                           ServiceStatusId = csr.Request_Status,
        //                           RequestStatus = dbContext.StatusMasters.FirstOrDefault(m => m.Id == csr.Request_Status && m.IsActive == true).Status,
        //                           Comment = csr.Comment,
        //                           RequestComment=csr.Comment,
        //                           ApproverId = csr.ApproverId,
        //                           ValidatorId = csr.ValidatorId,
        //                           ValidationDate = csr.ValidatedDate,
        //                           ApprovalDate = csr.ApprovalDate,
        //                           CreatedDate = csr.Created_Date,
        //                           Approver = csr.ApproverId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == csr.ApproverId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == csr.ApproverId).LastName) : string.Empty,
        //                           Validator = csr.ValidatorId != null ? (dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == csr.ValidatorId).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(u => u.UserRefId == csr.ValidatorId).LastName) : string.Empty,
        //                           StatusId = csr.Request_Status,
        //                           PaymentStatus = csr.PaymentStatus,
        //                           DispatchedDocument = csr.DispatchDocumentName,
        //                           ModifiedDate=csr.Modified_Date
        //                       }).FirstOrDefault();
        //        if (service != null)
        //        {
        //            if (service.ApprovalDate != null)
        //            {
        //                if (service.StatusId == NAStatusId.Completed || service.StatusId == NAStatusId.Approved)
        //                {
        //                    service.PendencyLevel = "Completed By - " + service.Approver;
        //                }
        //                else if (service.StatusId == NAStatusId.Rejected)
        //                {
        //                    service.PendencyLevel = "Rejected By " + service.Approver;
        //                }
        //                else
        //                {
        //                    service.PendencyLevel = "Cancelled By - " + service.Approver;
        //                }
        //            }
        //            else
        //            {
        //                if (service.StatusId == NAStatusId.Forwarded)
        //                {
        //                    service.PendencyLevel = "Forwarded To - " + service.Approver;
        //                }
        //                else if (service.StatusId == NAStatusId.Pending)
        //                {
        //                    service.PendencyLevel = "Put on Pending By - " + service.Approver;
        //                }
        //                else if (service.StatusId == NAStatusId.Objection)
        //                {
        //                    service.PendencyLevel = "Put on Objection By - " + service.Approver;
        //                }
        //                else if (service.StatusId == NAStatusId.Initiated)
        //                {
        //                    service.PendencyLevel = "Service Request is in Progress.";
        //                }
        //                else if (service.StatusId == NAStatusId.Withdrawn)
        //                {
        //                    service.PendencyLevel = "Withdrawn by - Allottee";
        //                }
        //                else if (service.StatusId == NAStatusId.Resubmitted)
        //                {
        //                    service.PendencyLevel = "Resubmitted By - Allottee";
        //                }
        //                else if (service.StatusId == NAStatusId.Appointment)
        //                {
        //                    service.PendencyLevel = "Put on Appointment By - " + service.Approver;
        //                }
        //                else
        //                {
        //                    service.PendencyLevel = null;
        //                }
        //            }
        //        }

        //        service.StatusMessage = GetRequestStatusMsg(service.StatusId, service.PaymentStatus);

        //        if (!string.IsNullOrEmpty(service.RegistrationNo)) service.RegistrationId = Convert.ToInt32(service.RegistrationNo);


        //        model.Id = service.Id;
        //        model.RegistrationId = !string.IsNullOrEmpty(service.RegistrationNo) ? Convert.ToInt32(service.RegistrationNo) : 0;
        //        model.OnlineRequestId = service.Id;
        //        model.ServiceModel = service;

        //        // get detail by service id
        //        switch (service.ServiceId)
        //        {
        //            case NAService.Transfer:
        //                model = GetTransferRequestServiceDetail(model);
        //                break;
        //            case NAService.Rent:
        //                model = GetRentRequestServiceDetail(model);
        //                break;
        //            case NAService.Mortgage:
        //                model = GetMortgageRequestServiceDetail(model);
        //                break;
        //            case NAService.Extension:
        //                model = GetExtensionRequestDetails(model);
        //                break;
        //            default:
        //                //model = model;
        //                break;
        //        }
        //        return model;
        //    }
        //}

        //private ServiceRequestVM GetMortgageRequestServiceDetail(ServiceRequestVM model)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var data = dbContext.OnlineMortgageDetails.Where(m => m.RequestNo == model.Id).FirstOrDefault();
        //        if (data != null)
        //        {
        //            MortgageVM mortgage = new MortgageVM
        //            {
        //                SanctionedAmount = data.SanctionedAmount,
        //                BankName = data.BankName,
        //                BranchAddress = data.BranchAddress,
        //                MortgageType = data.MortgageType,
        //                PreviousLoanNoc = data.PreviousLoanNoc
        //            };
        //            model.MortgageModel = mortgage;
        //            //return model;
        //        }
        //        else
        //        {
        //            model.MortgageModel = new MortgageVM();
        //        }

        //        return model;
        //    }
        //}


        //private string GetRequestStatusMsg(int? StatusId, int? PaymentStatus)
        //{
        //    var msg = string.Empty;
        //    if (StatusId == NAStatusId.Initiated)
        //    {
        //        msg = "Your application has been submitted and send to department.";
        //        if (PaymentStatus == 0)
        //        {
        //            msg = "Your application has been submitted and payment is pending";
        //        }
        //        else if (PaymentStatus == 1)
        //        {
        //            msg = "Your application has been submitted and payment is paid.";
        //        }
        //    }
        //    else if (StatusId == NAStatusId.Objection)
        //    {
        //        msg = "Your application has an objection.Please submit the required documents.";
        //    }
        //    else if (StatusId == NAStatusId.Completed)
        //    {
        //        msg = "Your application has been completed.";
        //    }
        //    else if (StatusId == NAStatusId.Pending)
        //    {
        //        msg = "Your application has been pending.";
        //    }
        //    else if (StatusId == NAStatusId.Rejected)
        //    {
        //        msg = "Your application has been rejected.";
        //    }
        //    else
        //    {
        //        msg = "Your application has been in process.";
        //    }
        //    return msg;
        //}

        //private ServiceRequestVM GetTransferRequestServiceDetail(ServiceRequestVM model)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var transfer = dbContext.OnlineTransferMutations.Where(m => m.RequestNo == model.Id && m.Type == "T").FirstOrDefault();
        //        if (transfer != null)
        //        {
        //            TransferVM tmodel = new TransferVM
        //            {
        //                TransferTypeId = transfer.Transfer_Type.Value,
        //                TransferType = dbContext.Transfer_Type.Where(t => t.Id == transfer.Transfer_Type).Select(t => t.type).FirstOrDefault(),
        //                TransferSubTypeId = transfer.Transfer_Sub_Type.Value,
        //                TransferSubType = dbContext.Transfer_Type.Where(t => t.Id == transfer.Transfer_Sub_Type).Select(t => t.type).FirstOrDefault(),
        //                Gender = (transfer.T_Gender == Constants.Company) ? Constants.Company : (transfer.T_Gender == Constants.Male ? Constants.Male : (transfer.T_Gender == Constants.Female ? Constants.Female : string.Empty)),
        //                ApplicantType = transfer.T_Gender,
        //                FirstName = transfer.T_First_Name,
        //                MiddleName = transfer.T_Middle_Name,
        //                LastName = transfer.T_Last_Name,
        //                Applicant = transfer.T_Gender == Constants.Company ? transfer.T_Company_Name : transfer.T_First_Name + " " + (string.IsNullOrEmpty(transfer.T_Middle_Name) ? string.Empty : transfer.T_Middle_Name + " ") + transfer.T_Last_Name,
        //                CompanyName = transfer.T_Company_Name,
        //                SigningAuthority = transfer.T_Signing_Authority,
        //                RegisteredOffice = transfer.T_Registered_Office,
        //                ApplicantMaster = transfer.T_Father_Husband_Name,
        //                MotherName = transfer.T_Mother_Name,
        //                PermanentAdd = transfer.T_Permanent_Add,
        //                CorrespondenceAdd = transfer.T_Correspondence_Add,
        //                PAN = transfer.T_Pan,
        //                OccupationId = transfer.T_Occupation_Id,
        //                Occupation = dbContext.OccupationMsts.Where(m => m.occupationId == transfer.T_Occupation_Id).Select(o => o.occupation).FirstOrDefault(),
        //                Mobile = transfer.T_Mobile,
        //                Email = transfer.T_Email,
        //                TypeOfTransferee = (transfer.T_Gender == Constants.Male || transfer.T_Gender == Constants.Female) ? Constants.Individual : Constants.Company
        //            };

        //            model.TransferModel = tmodel;
        //        }
        //        else
        //        {
        //            model.TransferModel = new TransferVM();
        //        }
        //        return model;
        //    }
        //}

        //private ServiceRequestVM GetRentRequestServiceDetail(ServiceRequestVM model)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var rents = dbContext.OnlineRentPermissionDetails.Where(r => r.RequestNo == model.Id).FirstOrDefault();
        //        if (rents != null)
        //        {
        //            RentingVM rent = new RentingVM
        //            {
        //                TenantName = rents.TenantName,
        //                TenantProject = rents.TenantProject,
        //                RentDuration = rents.RentDurationYears,
        //                RentingDate = rents.RentingDate
        //            };
        //            model.RentModel = rent;
        //            //return model;
        //        }
        //        else
        //        {
        //            model.RentModel = new RentingVM();
        //        }
        //        return model;
        //    }
        //}

        //private ServiceRequestVM GetExtensionRequestDetails(ServiceRequestVM model)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var data = dbContext.OnlineExtensionDetails.Where(m => m.RequestNo == model.Id).FirstOrDefault();
        //        if (data != null)
        //        {
        //            ExtensionVM extension = new ExtensionVM
        //            {
        //                ExtensionDueDate = data.ExtensionDueDate,
        //                ExtensionGivenDate = data.ExtensionGivenDate
        //            };
        //            model.ExtensionModel = extension;
        //            //return model;
        //        }
        //        else
        //        {
        //            model.ExtensionModel = new ExtensionVM();
        //        }
        //        return model;
        //    }
        //}

        ///// <summary>
        ///// Get nivesh status code mappped with the customer request status
        ///// </summary>
        ///// <param name="RequestStatusId"></param>
        ///// <returns>Nivesh Mitra StatusCode</returns>
        //public ServiceStatusVM GetServiceStatusByCustomerRequestStatusId(int RequestStatusId)
        //{
        //    var _ServiceStatus = new ServiceStatusVM();
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {
        //        _ServiceStatus = (from nm_service in dbContext.ServiceStatus
        //                          join nm_status in dbContext.ServiceStatusTrans on nm_service.Id equals nm_status.StatusID
        //                          join customerstatus in dbContext.StatusMasters on nm_status.CitizenStatusID equals customerstatus.Id
        //                          where nm_service.Status == 1 && customerstatus.IsActive == true && customerstatus.Id == RequestStatusId
        //                          select new ServiceStatusVM
        //                          {
        //                              StatusCode = nm_service.StatusCode,
        //                              StatusName = nm_service.StatusName
        //                          }).FirstOrDefault();
        //    }
        //    return _ServiceStatus;

        //}

        //#endregion

        //#endregion

        //#region  Manage Nivesh Mitra Controller Methods

        //public DataSourceResult GetNiveshMitraServiceRequests(DataSourceRequest request)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var _niveshmitraService = (from niveshmitra in dbContext.NiveshMitraEntr_Master
        //                                   join nm_services in dbContext.Services on niveshmitra.ServiceID equals nm_services.ServiceCode
        //                                   where niveshmitra.Status == true && niveshmitra.ApplicationID != null
        //                                   select new NiveshMitraMasterVM
        //                                   {
        //                                       ID = niveshmitra.ID,
        //                                       ControlID = niveshmitra.ControlID,
        //                                       UnitID = niveshmitra.UnitID,
        //                                       ServiceID = niveshmitra.ServiceID,
        //                                       ProcessIndustryID = niveshmitra.ProcessIndustryID,
        //                                       ApplicationID = niveshmitra.ApplicationID,
        //                                       CreatedBy = niveshmitra.CreatedBy,
        //                                       CreatedDate = niveshmitra.CreatedDate,
        //                                       Status = niveshmitra.Status,
        //                                       ServiceName = nm_services.ServiceName
        //                                   });
        //        return _niveshmitraService.ToDataSourceResult(request);
        //    }
        //}

        //public DataSourceResult GetNiveshMitraServices(DataSourceRequest request)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var _niveshmitraService = (from nm_services in dbContext.Services
        //                                   where nm_services.Status == 1
        //                                   select new NMServicesVM
        //                                   {
        //                                       Id = nm_services.Id,
        //                                       ServiceCode = nm_services.ServiceCode,
        //                                       ServiceName = nm_services.ServiceName,
        //                                       Status = nm_services.Status,
        //                                       CreatedBy = nm_services.CreatedBy,
        //                                       CreatedDate = nm_services.CreatedDate,
        //                                   });
        //        return _niveshmitraService.ToDataSourceResult(request);
        //    }
        //}

        //public DataSourceResult GetCitizenServiceDetailsbyServiceId(DataSourceRequest request)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var _niveshmitraService = (from nm_services in dbContext.Services
        //                                   join nm_serviceTrans in dbContext.ServiceTrans on nm_services.Id.ToString() equals nm_serviceTrans.ServiceId
        //                                   join customerservice in dbContext.CitizenService_Master on nm_serviceTrans.CitizenServiceId equals customerservice.Id
        //                                   join dept in dbContext.DepartmentMsts on customerservice.Deptt_Id equals dept.departmentId
        //                                   where nm_services.Status == 1 && customerservice.Status == 1 && dept.IsActive == true
        //                                   select new NMServicesVM
        //                                   {
        //                                       ServiceCode = nm_services.ServiceCode,
        //                                       ServiceName = nm_services.ServiceName,
        //                                       Status = nm_services.Status,
        //                                       CreatedBy = nm_services.CreatedBy,
        //                                       CreatedDate = nm_services.CreatedDate,
        //                                       CitizenServiceId = nm_serviceTrans.CitizenServiceId,
        //                                       DeptId = customerservice.Deptt_Id,
        //                                       ServiceId = customerservice.service_id,
        //                                       Amount = customerservice.Amount,
        //                                       CitizenServiceName = customerservice.ServiceName,
        //                                       Timeline = customerservice.Timeline,
        //                                       CitizenServiceStatus = customerservice.Status,
        //                                       DepartmentName = dept.departmentName
        //                                   });
        //        return _niveshmitraService.ToDataSourceResult(request);
        //    }
        //}


        //public DataSourceResult GetNiveshMitraServiceStatus(DataSourceRequest request)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var _niveshmitraServiceStatus = (from nm_service in dbContext.ServiceStatus
        //                                         join nm_status in dbContext.ServiceStatusTrans on nm_service.Id equals nm_status.StatusID
        //                                         join customerstatus in dbContext.StatusMasters on nm_status.CitizenStatusID equals customerstatus.Id
        //                                         where nm_service.Status == 1 && customerstatus.IsActive == true
        //                                         select new ServiceStatusVM
        //                                         {
        //                                             Id = nm_service.Id,
        //                                             StatusCode = nm_service.StatusCode,
        //                                             StatusName = nm_service.StatusName,
        //                                             Status = nm_service.Status,
        //                                             CreatedBy = nm_service.CreatedBy,
        //                                             CreatedDate = nm_service.CreatedDate,
        //                                             CitizenStatusID = nm_status.CitizenStatusID,
        //                                             CitizenStatusName = customerstatus.Status
        //                                         });
        //        return _niveshmitraServiceStatus.ToDataSourceResult(request);
        //    }
        //}

        //public DataSourceResult GetCustomerServiceRequestList_NIC(DataSourceRequest request)
        //{
        //    IQueryable<ServiceViewModel> list;
        //    List<string> SubDepartmentList = new List<string>();
        //    if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Accountant || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.HODAccounts)
        //    {
        //        SubDepartmentList.Add("Account");
        //    }
        //    else
        //    {
        //        if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Assistant || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Property)
        //        {
        //            SubDepartmentList.Add("Property");
        //        }
        //        else
        //        {
        //            SubDepartmentList.Add("Account");
        //            SubDepartmentList.Add("Property");
        //        }
        //    }

        //    int userid = userInfo.UserID;
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.OSD)
        //        {
        //            list = (from csr in dbContext.Customer_ServiceRequest
        //                        join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
        //                        join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
        //                        where DepartmentList.Contains(csr.DepartmentId) && SubDepartmentList.Contains(csr.SubDepartment) && csm.Status == 1 && csr.RequestThrough == Constants.NIC_NiveshMitra && csr.ServiceId != 12 && csr.ServiceId != 13
        //                        select new ServiceViewModel
        //                        {
        //                            Id = csr.Id,
        //                            RequestId = csr.Id,
        //                            RegistrationNo = csr.Registration_No,
        //                            DepartmentId = csr.DepartmentId,
        //                            Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
        //                            PropertyNo = csr.Property_No ?? string.Empty,
        //                            RequestDate = csr.Created_Date,
        //                            CreatedDate = csr.Created_Date,
        //                            Timeline = csm.Timeline,
        //                            ServiceId = csr.ServiceId,
        //                            ServiceName = csm.ServiceName,
        //                            ServiceType = csr.ServiceType,
        //                            DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
        //                            Amount = csr.DuesAmount,
        //                            StatusId = sts.Id,
        //                            Status = sts.Status,
        //                            IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
        //                            Comment = csr.Comment,
        //                            MobileNo = csr.MobileNumber,
        //                            Applicant = csr.ApplicantName,
        //                            Description = csr.Description,
        //                            Requestor = csr.RequestorName,
        //                            RequestorAddress = csr.RequestorAddress,
        //                            SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
        //                            PaymentStatus = csr.PaymentStatus,
        //                            Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
        //                            IsUploaded = csr.IsUploadedLetter,
        //                            DispatchedDocument = csr.DispatchDocumentName,
        //                            UnitId = dbContext.NiveshMitraEntr_Master.FirstOrDefault(m => m.ApplicationID == csr.Id.ToString()).UnitID
        //                        });
        //        }
        //        else if (userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.HODAccounts)
        //        {
        //            list = (from csr in dbContext.Customer_ServiceRequest
        //                        join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
        //                        join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
        //                        where DepartmentList.Contains(csr.DepartmentId) && SubDepartmentList.Contains(csr.SubDepartment) && csm.Status == 1 && csr.RequestThrough == Constants.NIC_NiveshMitra && (csr.ServiceId == 12 || csr.ServiceId == 13)
        //                        select new ServiceViewModel
        //                        {
        //                            Id = csr.Id,
        //                            RequestId = csr.Id,
        //                            RegistrationNo = csr.Registration_No,
        //                            DepartmentId = csr.DepartmentId,
        //                            Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
        //                            PropertyNo = csr.Property_No ?? string.Empty,
        //                            RequestDate = csr.Created_Date,
        //                            CreatedDate = csr.Created_Date,
        //                            Timeline = csm.Timeline,
        //                            ServiceId = csr.ServiceId,
        //                            ServiceName = csm.ServiceName,
        //                            ServiceType = csr.ServiceType,
        //                            DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
        //                            Amount = csr.DuesAmount,
        //                            StatusId = sts.Id,
        //                            Status = sts.Status,
        //                            IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
        //                            Comment = csr.Comment,
        //                            MobileNo = csr.MobileNumber,
        //                            Applicant = csr.ApplicantName,
        //                            Description = csr.Description,
        //                            Requestor = csr.RequestorName,
        //                            RequestorAddress = csr.RequestorAddress,
        //                            SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
        //                            PaymentStatus = csr.PaymentStatus,
        //                            Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
        //                            IsUploaded = csr.IsUploadedLetter,
        //                            DispatchedDocument = csr.DispatchDocumentName,
        //                            UnitId = dbContext.NiveshMitraEntr_Master.FirstOrDefault(m => m.ApplicationID == csr.Id.ToString()).UnitID
        //                        });
        //        }
        //        else if (userInfo.RoleMaster.RoleInDepartment == null || userInfo.RoleMaster.RoleInDepartment == RoleInDepartment.Admin)
        //        {
        //            list = (from csr in dbContext.Customer_ServiceRequest
        //                        join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
        //                        join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
        //                    where DepartmentList.Contains(csr.DepartmentId) && SubDepartmentList.Contains(csr.SubDepartment) && csm.Status == 1 && csr.RequestThrough == Constants.NIC_NiveshMitra
        //                        select new ServiceViewModel
        //                        {
        //                            Id = csr.Id,
        //                            RequestId = csr.Id,
        //                            RegistrationNo = csr.Registration_No,
        //                            DepartmentId = csr.DepartmentId,
        //                            Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
        //                            PropertyNo = csr.Property_No ?? string.Empty,
        //                            RequestDate = csr.Created_Date,
        //                            CreatedDate = csr.Created_Date,
        //                            Timeline = csm.Timeline,
        //                            ServiceId = csr.ServiceId,
        //                            ServiceName = csm.ServiceName,
        //                            ServiceType = csr.ServiceType,
        //                            DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
        //                            Amount = csr.DuesAmount,
        //                            StatusId = sts.Id,
        //                            Status = sts.Status,
        //                            IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
        //                            Comment = csr.Comment,
        //                            MobileNo = csr.MobileNumber,
        //                            Applicant = csr.ApplicantName,
        //                            Description = csr.Description,
        //                            Requestor = csr.RequestorName,
        //                            RequestorAddress = csr.RequestorAddress,
        //                            SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
        //                            PaymentStatus = csr.PaymentStatus,
        //                            Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
        //                            IsUploaded = csr.IsUploadedLetter,
        //                            DispatchedDocument = csr.DispatchDocumentName,
        //                            UnitId = dbContext.NiveshMitraEntr_Master.FirstOrDefault(m => m.ApplicationID == csr.Id.ToString()).UnitID
        //                        });
        //        }
        //        else
        //        {
        //            list = (from csr in dbContext.Customer_ServiceRequest
        //                        join csm in dbContext.CitizenService_Master on new { y = csr.DepartmentId, x = csr.ServiceId } equals new { y = csm.Deptt_Id, x = csm.service_id }
        //                        join sts in dbContext.StatusMasters on csr.Request_Status equals sts.Id
        //                    where DepartmentList.Contains(csr.DepartmentId) && csm.Status == 1 && csr.RequestThrough == Constants.NIC_NiveshMitra && csr.ApproverId == userid
        //                        select new ServiceViewModel
        //                        {
        //                            Id = csr.Id,
        //                            RequestId = csr.Id,
        //                            RegistrationNo = csr.Registration_No,
        //                            DepartmentId = csr.DepartmentId,
        //                            Department = (csr.DepartmentId == null || csr.DepartmentId == 0) ? string.Empty : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == csr.DepartmentId).departmentName,
        //                            PropertyNo = csr.Property_No ?? string.Empty,
        //                            RequestDate = csr.Created_Date,
        //                            CreatedDate = csr.Created_Date,
        //                            Timeline = csm.Timeline,
        //                            ServiceId = csr.ServiceId,
        //                            ServiceName = csm.ServiceName,
        //                            ServiceType = csr.ServiceType,
        //                            DuesAmount = csr.DuesAmount == null ? 0 : csr.DuesAmount,
        //                            Amount = csr.DuesAmount,
        //                            StatusId = sts.Id,
        //                            Status = sts.Status,
        //                            IsCancelled = csr.Request_Status == NAStatusId.Cancelled ? true : false,
        //                            Comment = csr.Comment,
        //                            MobileNo = csr.MobileNumber,
        //                            Applicant = csr.ApplicantName,
        //                            Description = csr.Description,
        //                            Requestor = csr.RequestorName,
        //                            RequestorAddress = csr.RequestorAddress,
        //                            SubDepartment = string.IsNullOrEmpty(csr.SubDepartment) ? SubDepartment.Property : csr.SubDepartment,
        //                            PaymentStatus = csr.PaymentStatus,
        //                            Approver = dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).UserName + "-" + dbContext.UmUserMasters.FirstOrDefault(m => m.UserRefId == csr.ApproverId).FirstName,
        //                            IsUploaded = csr.IsUploadedLetter,
        //                            DispatchedDocument = csr.DispatchDocumentName,
        //                            UnitId = dbContext.NiveshMitraEntr_Master.FirstOrDefault(m => m.ApplicationID == csr.Id.ToString()).UnitID
        //                        });
        //        }
        //        return list.ToDataSourceResult(request);
        //    }
        //}


        //public int UpdateCustomerServiceRequestStatus(ServiceViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        //{
        //    int flag = ReturnType.None;

        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var service = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == model.Id);
        //        var status = dbContext.Customer_ServiceStatusTrans.Where(c => c.RequestRefId == model.Id).OrderByDescending(c => c.Id).FirstOrDefault();
        //        if (service.Request_Status == NAStatusId.Completed)
        //        {
        //            flag = ReturnType.Updated;
        //        }
        //        else if (service.Request_Status == NAStatusId.Forwarded || service.Request_Status == NAStatusId.Pending || service.Request_Status == NAStatusId.Objection || service.Request_Status == NAStatusId.Resubmitted)
        //        {
        //            if (model.StatusId == NAStatusId.Forwarded && service.Request_Status == NAStatusId.Forwarded)
        //            {
        //                service.Request_Status = model.StatusId;
        //                service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
        //                service.ValidatorId = userInfo.UserID;
        //                service.ValidatedDate = DateTime.Now;
        //                service.ApproverId = model.ApproverId;
        //                dbContext.SaveChanges();

        //                if (status != null)
        //                {
        //                    var service_Status = new Customer_ServiceStatusTrans
        //                    {
        //                        RequestRefId = service.Id,
        //                        ValidatorId = userInfo.UserID,
        //                        ValidatedDate = DateTime.Now,
        //                        ApproverId = model.ApproverId,
        //                        CreatedDate = DateTime.Now,
        //                        CreatedBy = userInfo.UserID,
        //                        StatusId = model.StatusId,
        //                        Remarks = model.Comment + "by \n" + userInfo.FirstName + " " + userInfo.LastName + "Date:" + DateTime.Now + "."
        //                    };
        //                    dbContext.Customer_ServiceStatusTrans.Add(service_Status);
        //                    dbContext.SaveChanges();
        //                }
        //                flag = ReturnType.Validated;
        //            }
        //            else if (model.StatusId == NAStatusId.Pending && service.Request_Status == NAStatusId.Pending)
        //            {
        //                flag = ReturnType.Exist;
        //            }
        //            else if (model.StatusId == NAStatusId.Pending && service.Request_Status == NAStatusId.Forwarded)
        //            {
        //                service.Request_Status = model.StatusId;
        //                service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
        //                flag = ReturnType.Pending;
        //                dbContext.SaveChanges();
        //            }
        //            else if (model.StatusId == NAStatusId.Objection && service.Request_Status == NAStatusId.Objection)
        //            {
        //                flag = ReturnType.Exist;
        //            }
        //            else if (model.StatusId == NAStatusId.Objection && service.Request_Status == NAStatusId.Forwarded)
        //            {
        //                service.Request_Status = model.StatusId;
        //                service.ObjectionStatus = model.StatusId;
        //                service.Comment = service.Comment + "\n" + model.Comment + "-" + userInfo.FirstName + " " + userInfo.LastName + " " + " Status: Objection " + " " + " Date:" + DateTime.Now + ".";
        //                //service.ValidatorId = userInfo.UserID;
        //                //service.ValidatedDate = DateTime.Now;
        //                //service.ApproverId = model.ApproverId;
        //                dbContext.SaveChanges();
        //                if (status != null)
        //                {
        //                    status.StatusId = model.StatusId;
        //                    status.ModifiedBy = userInfo.UserID;
        //                    status.ModifiedDate = DateTime.Now;
        //                    status.Remarks = status.Remarks + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
        //                    dbContext.SaveChanges();
        //                }
        //                flag = ReturnType.Validated;
        //            }
        //            else if (model.StatusId == NAStatusId.Forwarded && service.Request_Status == NAStatusId.Resubmitted)
        //            {
        //                service.Request_Status = model.StatusId;
        //                service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
        //                service.ValidatorId = userInfo.UserID;
        //                service.ValidatedDate = DateTime.Now;
        //                service.ApproverId = model.ApproverId;
        //                dbContext.SaveChanges();

        //                if (status != null)
        //                {
        //                    var service_Status = new Customer_ServiceStatusTrans
        //                    {
        //                        RequestRefId = service.Id,
        //                        ValidatorId = userInfo.UserID,
        //                        ValidatedDate = DateTime.Now,
        //                        ApproverId = model.ApproverId,
        //                        CreatedDate = DateTime.Now,
        //                        CreatedBy = userInfo.UserID,
        //                        StatusId = model.StatusId,
        //                        Remarks = model.Comment + "by \n" + userInfo.FirstName + " " + userInfo.LastName + "Date:" + DateTime.Now + "."
        //                    };
        //                    dbContext.Customer_ServiceStatusTrans.Add(service_Status);
        //                    dbContext.SaveChanges();
        //                }
        //                flag = ReturnType.Validated;
        //            }
        //            else
        //            {
        //                service.Request_Status = model.StatusId;
        //                service.Comment = service.Comment + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.MiddleName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
        //                service.ApproverId = userInfo.UserID;
        //                service.ApprovalDate = DateTime.Now;

        //                if (status != null)
        //                {
        //                    status.StatusId = model.StatusId;
        //                    status.ModifiedBy = userInfo.UserID;
        //                    status.ModifiedDate = DateTime.Now;
        //                    status.Remarks = status.Remarks + "\n" + model.Comment + " by \n" + userInfo.FirstName + " " + userInfo.LastName + " Date:" + DateTime.Now + ".";
        //                    dbContext.SaveChanges();
        //                }
        //                if (model.StatusId == NAStatusId.Cancelled || model.StatusId == NAStatusId.Rejected)
        //                {
        //                    service.IsActive = false;
        //                    dbContext.SaveChanges();
        //                    flag = ReturnType.Rejected;
        //                }
        //                if (model.StatusId == NAStatusId.Approved || model.StatusId == NAStatusId.Completed)
        //                {
        //                    if (string.IsNullOrEmpty(service.Registration_No) && service.ServiceId != NAService.Query)
        //                    {
        //                        flag = ReturnType.NotRegistered;
        //                    }
        //                    else
        //                    {
        //                        dbContext.SaveChanges();

        //                        if (service.Request_Status == NAStatusId.Completed)
        //                        {
        //                            if (files != null && files.Count() > 0)
        //                            {
        //                                FtpHandler.UploadFiles_NIC(files, model.Id.ToString());
        //                                if (!string.IsNullOrEmpty(files.FirstOrDefault().FileName))
        //                                {
        //                                    var _fileName = "http://doc.mynoida.in/UploadDocuments/NiveshMitraUpload/" + model.RequestId + "/" + model.Id.ToString() + ".pdf";
        //                                    service.DispatchDocumentName = _fileName;
        //                                    dbContext.SaveChanges();
        //                                }
        //                            }
        //                            flag = ReturnType.Completed;
        //                        }
        //                    }
        //                }

        //                else flag = ReturnType.Cancelled;
        //                string reqName = dbContext.StatusMasters.Where(m => m.Id == service.Request_Status).FirstOrDefault().Status;

        //                string message = string.Empty;
        //                message = string.Format(NAMessages.SDServiceReqStatusChange, model.Id, reqName);
        //                if (!string.IsNullOrEmpty(service.MobileNumber)) { ApplicationHelper.SendSMS(service.MobileNumber, message); }
        //                if (!string.IsNullOrEmpty(service.Email)) { ApplicationHelper.SendEmail(service.Email, "Online Request", message); }
        //            }
        //        }
        //        else if (service.Request_Status == NAStatusId.Initiated)//for forward 
        //        {
        //            if (model.StatusId == NAStatusId.Forwarded)
        //            {
        //                service.Request_Status = model.StatusId;
        //                service.Comment = model.Comment + "-" + userInfo.FirstName + " " + userInfo.LastName;
        //                service.ValidatorId = userInfo.UserID;
        //                service.ValidatedDate = DateTime.Now;
        //                service.ApproverId = model.ApproverId;
        //                dbContext.SaveChanges();
        //                if (status == null)
        //                {
        //                    var service_Status = new Customer_ServiceStatusTrans();
        //                    service_Status.RequestRefId = service.Id;
        //                    service_Status.ValidatorId = userInfo.UserID;
        //                    service_Status.ValidatedDate = DateTime.Now;
        //                    service_Status.ApproverId = model.ApproverId;
        //                    service_Status.CreatedDate = DateTime.Now;
        //                    service_Status.CreatedBy = userInfo.UserID;
        //                    service_Status.StatusId = model.StatusId;
        //                    service_Status.Remarks = model.Comment + "by \n" + userInfo.FirstName + " " + userInfo.LastName + "Date:" + DateTime.Now + ".";
        //                    dbContext.Customer_ServiceStatusTrans.Add(service_Status);
        //                    dbContext.SaveChanges();
        //                }
        //                flag = ReturnType.Validated;
        //            }
        //            else flag = ReturnType.Initiated;
        //        }
        //    }
        //    return flag;
        //}

        //#endregion

        //#region

        //public int UploadGeneratedLetterByserviceId(ServiceViewModel model, IEnumerable<HttpPostedFileBase> documentfiles)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var flag = ReturnType.None;
        //        var service = dbContext.Customer_ServiceRequest.FirstOrDefault(c => c.Id == model.RequestId);
        //        if (service.Request_Status == NAStatusId.Completed)
        //        {
        //            if (documentfiles != null && documentfiles.Count() > 0)
        //            {
        //                FtpHandler.UploadFiles_NIC(documentfiles, model.RequestId.ToString());
        //                if (!string.IsNullOrEmpty(documentfiles.FirstOrDefault().FileName))
        //                {
        //                    service.IsUploadedLetter = true;
        //                    var _fileName = "http://doc.mynoida.in/UploadDocuments/NiveshMitraUpload/" + model.RequestId + "/" + model.RequestId.ToString() + ".pdf";
        //                    service.DispatchDocumentName = _fileName;
        //                    service.DispatchDate = DateTime.Now;
        //                    dbContext.SaveChanges();
        //                    flag = ReturnType.Saved;
        //                }
        //            }
        //        }
        //        return flag;
        //    }
        //}

        //#endregion

        //#region Department NIC

        //public List<DropdownViewModel> GetServiceListByDepartmentForNIC(int departmentId)
        //{
        //    using (var dbContext = new NoidaPMSEntities())
        //    {
        //        var lst = (from service in dbContext.CitizenService_Master
        //                   join nic_service_trans in dbContext.ServiceTrans on service.Id equals nic_service_trans.CitizenServiceId
        //                   where service.Deptt_Id == departmentId && service.Status == 1
        //                   select new DropdownViewModel
        //                   {
        //                       Id = service.service_id.Value,
        //                       ServiceId = service.service_id,
        //                       Text = service.ServiceName
        //                   }).ToList();
        //        return lst;
        //    }
        //}

        //#endregion



        //public ServiceRequestVM SaveNiveshMitraServiceUnit(WBasicDetailsModel_NMS apimodel)
        //{
        //    var service = new ServiceRequestVM();
        //    service.IsServiceExist = false;
        //    using (var dbContext = new Model.NoidaPMSEntities())
        //    {
        //        if (!string.IsNullOrEmpty(apimodel.TxtControlID) && !string.IsNullOrEmpty(apimodel.TxtUnitID))
        //        {
        //            var _nsMaster = dbContext.NiveshMitraEntr_Master.FirstOrDefault(c => c.UnitID == apimodel.TxtUnitID && c.ControlID == apimodel.TxtControlID && c.ServiceID == apimodel.TxtServiceID);
        //            if (_nsMaster == null)
        //            {
        //                var _ApplicantVM = new ApplicantVM();
        //                if (!string.IsNullOrEmpty(apimodel.TxtProcessIndustryID))
        //                {
        //                    int rid = Convert.ToInt32(apimodel.TxtProcessIndustryID);
        //                    _ApplicantVM = GetApplicantDetailsByRegistrationId(rid);
        //                }

        //                if (_ApplicantVM != null)
        //                {
        //                    var niveshMitramaster = new NiveshMitraEntr_Master();
        //                    niveshMitramaster.ControlID = apimodel.TxtControlID;
        //                    niveshMitramaster.UnitID = apimodel.TxtUnitID;
        //                    niveshMitramaster.ServiceID = apimodel.TxtServiceID;
        //                    niveshMitramaster.ProcessIndustryID = apimodel.TxtProcessIndustryID;
        //                    niveshMitramaster.Sector = _ApplicantVM.Sector;
        //                    niveshMitramaster.Block = _ApplicantVM.Block;
        //                    niveshMitramaster.PlotNo = _ApplicantVM.PlotNo;
        //                    niveshMitramaster.MobileNo = _ApplicantVM.Mobile;
        //                    niveshMitramaster.Status = true;
        //                    niveshMitramaster.CreatedBy = userInfo.UserID;
        //                    niveshMitramaster.CreatedDate = DateTime.Now;
        //                    dbContext.NiveshMitraEntr_Master.Add(niveshMitramaster);
        //                    dbContext.SaveChanges();

        //                    service.IsServiceExist = true;
        //                }
        //            }
        //            else
        //            {
        //                _nsMaster.ApplicationID = apimodel.TxtApplicationID;
        //                _nsMaster.ModifiedBy = userInfo.UserID;
        //                _nsMaster.ModifiedDate = DateTime.Now;
        //                dbContext.SaveChanges();
        //                service.IsServiceExist = true;
        //            }
        //        }
        //    }
        //    return service;
        //}
    }
}
