using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Service
{
    public class NICFormService : INICFormService
    {
        INICFormRepository _nicFormRepository;
        public NICFormService(INICFormRepository nicFormRepository)
        {
            _nicFormRepository = nicFormRepository;
        }

        public Model.OnlineFormViewModel GetSchemeBasicInfoData(Model.OnlineFormViewModel form)
        {
            return _nicFormRepository.GetSchemeBasicInfoData(form);
        }

        public Model.OnlineFormViewModel GetOpenEndedSchemeFormDataById(Model.OnlineFormViewModel model)
        {
            return _nicFormRepository.GetOpenEndedSchemeFormDataById(model);
        }

        public Model.OnlineFormViewModel SaveOpenEndedSchemeFormDetail(Model.OnlineFormViewModel model, System.Web.HttpPostedFileBase userImage, System.Web.HttpPostedFileBase signatureImage)
        {
            return _nicFormRepository.SaveOpenEndedSchemeFormDetail(model,userImage,signatureImage);
        }

        public Model.OnlineFormViewModel SaveProposedProjectAndRefundDetail(Model.OnlineFormViewModel model)
        {
            return _nicFormRepository.SaveProposedProjectAndRefundDetail(model);
        }

        public Model.OnlineFormViewModel SaveUploadedDocument(Model.OnlineFormViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files, System.Web.HttpPostedFileBase userImage, System.Web.HttpPostedFileBase signatureImage)
        {
            return _nicFormRepository.SaveUploadedDocument(model, files, userImage, signatureImage);
        }

        public int RemoveDocumentFromApplicationForm(string formNo, string filename)
        {
            return _nicFormRepository.RemoveDocumentFromApplicationForm(formNo, filename);
        }

        public Model.onlineDirectorViewModel SaveDirectorDetailsForOpenScheme(Model.onlineDirectorViewModel model)
        {
            return _nicFormRepository.SaveDirectorDetailsForOpenScheme(model);
        }

        public Kendo.Mvc.UI.DataSourceResult GetFloorAreaListAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, Model.DropdownViewModel model)
        {
            return _nicFormRepository.GetFloorAreaListAsDataSource(request, model);
        }

        public Kendo.Mvc.UI.DataSourceResult GetUploadedDocumentsByFormId(Kendo.Mvc.UI.DataSourceRequest request, int? formId)
        {
            return _nicFormRepository.GetUploadedDocumentsByFormId(request, formId);
        }

        public Kendo.Mvc.UI.DataSourceResult GetDirectorDetailsAsDataSourceByFormId(Kendo.Mvc.UI.DataSourceRequest request, int? id)
        {
            return _nicFormRepository.GetDirectorDetailsAsDataSourceByFormId(request, id);
        }


        public IEnumerable<Model.DropdownViewModel> GetFloorAreaRangeList(Model.DropdownViewModel model)
        {
            return _nicFormRepository.GetFloorAreaRangeList(model);
        }

        public Model.OnlineFormViewModel GetApplicationFormFeeAndProcessingCharge(Model.OnlineFormViewModel model)
        {
            return _nicFormRepository.GetApplicationFormFeeAndProcessingCharge(model);
        }

        public Model.OnlineFormViewModel ValidatePANForApplicationForm(Model.OnlineFormViewModel model)
        {
            return _nicFormRepository.ValidatePANForApplicationForm(model);
        }


        public int GetProcessingAndReservationMoneyPaymentStatus(string formNo)
        {
            return _nicFormRepository.GetProcessingAndReservationMoneyPaymentStatus(formNo);
        }


        public Model.OnlineFormViewModel GetBankAccountDetails(Model.OnlineFormViewModel model)
        {
            return _nicFormRepository.GetBankAccountDetails(model);
        }


        public Model.OnlineChallanViewModel SaveOfflinePaymentTransactionForChallan(Model.OnlineFormViewModel model)
        {
            return _nicFormRepository.SaveOfflinePaymentTransactionForChallan(model);
        }


        public Model.OnlineFormViewModel SaveOnlineSchemePaymentStatus(Model.OnlineFormViewModel model, HttpPostedFileBase challan)
        {
            return _nicFormRepository.SaveOnlineSchemePaymentStatus(model, challan);
        }


        public Kendo.Mvc.UI.DataSourceResult GetOnlineSchemeListAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, Model.DropdownViewModel model)
        {
            return _nicFormRepository.GetOnlineSchemeListAsDataSource(request, model);
        }

        public Kendo.Mvc.UI.DataSourceResult GetDepartmentBySchemeAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, DropdownViewModel model)
        {
            return _nicFormRepository.GetDepartmentBySchemeAsDataSource(request, model);
        }

        public Kendo.Mvc.UI.DataSourceResult GetOnlineSchemeApplicationAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, Model.OnlineFormViewModel model)
        {
            return _nicFormRepository.GetOnlineSchemeApplicationAsDataSource(request, model);
        }


        public Kendo.Mvc.UI.DataSourceResult GetOnlineSchemeFormPaymentAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, OnlineFormViewModel model)
        {
            return _nicFormRepository.GetOnlineSchemeFormPaymentAsDataSource(request, model);
        }


        public OnlinePaymentViewModel GetSchemeFormPaymentTransaction(OnlinePaymentViewModel model)
        {
            return _nicFormRepository.GetSchemeFormPaymentTransaction(model);
        }


        public OnlinePaymentViewModel ValidateSchemeFormChallan(OnlinePaymentViewModel model)
        {
            return _nicFormRepository.ValidateSchemeFormChallan(model);
        }


        public OnlineFormViewModel GetOnlineSchemeFormDataById(OnlineFormViewModel model)
        {
            return _nicFormRepository.GetOnlineSchemeFormDataById(model);
        }


        public OnlineFormViewModel SaveOnlineSchemeFormStatus(OnlineFormViewModel model)
        {
            return _nicFormRepository.SaveOnlineSchemeFormStatus(model);
        }


        public OnlineFormViewModel SaveOnlineSchemeFormProcessRequest(OnlineFormViewModel model)
        {
            return _nicFormRepository.SaveOnlineSchemeFormProcessRequest(model);
        }


        public OnlineFormViewModel SaveAllotmentDetailByOnlineSchemeFormId(OnlineFormViewModel model)
        {
            return _nicFormRepository.SaveAllotmentDetailByOnlineSchemeFormId(model);
        }


        public Kendo.Mvc.UI.DataSourceResult GetDropDownListByTypeAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, DropdownViewModel model)
        {
            return _nicFormRepository.GetDropDownListByTypeAsDataSource(request, model);
        }


        public OnlinePaymentViewModel SaveOnlinePaymentTransaction(OnlinePaymentViewModel model)
        {
            return _nicFormRepository.SaveOnlinePaymentTransaction(model);
        }


        public OnlinePaymentViewModel SaveOnlinePaymentTransactionReturn(System.Web.Mvc.FormCollection form)
        {
            return _nicFormRepository.SaveOnlinePaymentTransactionReturn(form);
        }


        public Kendo.Mvc.UI.DataSourceResult GetSchemeFormChallanAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, OnlineFormViewModel model)
        {
            return _nicFormRepository.GetSchemeFormChallanAsDataSource(request, model);
        }


        public EpassViewModel SaveEpassFormDetail(EpassViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase idfile, HttpPostedFileBase rcfile, HttpPostedFileBase dlfile)
        {
            return _nicFormRepository.SaveEpassFormDetail(model, userImage, idfile, rcfile, dlfile);
        }


        public EpassViewModel GetEpassFormDetailById(EpassViewModel model)
        {
            return _nicFormRepository.GetEpassFormDetailById(model);
        }

        public DataSourceResult GetAllEpassList(DataSourceRequest request, EpassViewModel model)
        {
            return _nicFormRepository.GetAllEpassList(request, model);
        }

        public int ValidateEpass(EpassViewModel model)
        {
            return _nicFormRepository.ValidateEpass(model);
        }


        public EpassViewModel EpassDetails(int? Id)
        {
            return _nicFormRepository.EpassDetails(Id);
        }
    }
}
