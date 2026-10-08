using Kendo.Mvc.UI;
using NA.PMS.Model;
using NA.PMS.Model.NIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Repository
{
    public interface INICFormRepository
    {
        Model.OnlineFormViewModel GetSchemeBasicInfoData(Model.OnlineFormViewModel form);

        Model.OnlineFormViewModel GetOpenEndedSchemeFormDataById(Model.OnlineFormViewModel model);

        Model.OnlineFormViewModel SaveOpenEndedSchemeFormDetail(Model.OnlineFormViewModel model, System.Web.HttpPostedFileBase userImage, System.Web.HttpPostedFileBase signatureImage);

        Model.OnlineFormViewModel SaveProposedProjectAndRefundDetail(Model.OnlineFormViewModel model);

        Model.OnlineFormViewModel SaveUploadedDocument(Model.OnlineFormViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files, System.Web.HttpPostedFileBase userImage, System.Web.HttpPostedFileBase signatureImage);

        int RemoveDocumentFromApplicationForm(string formNo, string filename);

        Model.onlineDirectorViewModel SaveDirectorDetailsForOpenScheme(Model.onlineDirectorViewModel model);

        Kendo.Mvc.UI.DataSourceResult GetFloorAreaListAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, Model.DropdownViewModel model);

        Kendo.Mvc.UI.DataSourceResult GetUploadedDocumentsByFormId(Kendo.Mvc.UI.DataSourceRequest request, int? formId);

        Kendo.Mvc.UI.DataSourceResult GetDirectorDetailsAsDataSourceByFormId(Kendo.Mvc.UI.DataSourceRequest request, int? id);

        IEnumerable<Model.DropdownViewModel> GetFloorAreaRangeList(Model.DropdownViewModel model);

        Model.OnlineFormViewModel GetApplicationFormFeeAndProcessingCharge(Model.OnlineFormViewModel model);

        Model.OnlineFormViewModel ValidatePANForApplicationForm(Model.OnlineFormViewModel model);

        int GetProcessingAndReservationMoneyPaymentStatus(string formNo);

        Model.OnlineFormViewModel GetBankAccountDetails(Model.OnlineFormViewModel model);

        Model.OnlineChallanViewModel SaveOfflinePaymentTransactionForChallan(Model.OnlineFormViewModel model);

        Model.OnlineFormViewModel SaveOnlineSchemePaymentStatus(Model.OnlineFormViewModel model, HttpPostedFileBase challan);

        Kendo.Mvc.UI.DataSourceResult GetOnlineSchemeListAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, Model.DropdownViewModel model);

        Kendo.Mvc.UI.DataSourceResult GetDepartmentBySchemeAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, DropdownViewModel model);

        Kendo.Mvc.UI.DataSourceResult GetOnlineSchemeApplicationAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, Model.OnlineFormViewModel model);

        Kendo.Mvc.UI.DataSourceResult GetOnlineSchemeFormPaymentAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, OnlineFormViewModel model);

        OnlinePaymentViewModel GetSchemeFormPaymentTransaction(OnlinePaymentViewModel model);

        OnlinePaymentViewModel ValidateSchemeFormChallan(OnlinePaymentViewModel model);

        OnlineFormViewModel GetOnlineSchemeFormDataById(OnlineFormViewModel model);

        OnlineFormViewModel SaveOnlineSchemeFormStatus(OnlineFormViewModel model);

        OnlineFormViewModel SaveOnlineSchemeFormProcessRequest(OnlineFormViewModel model);

        OnlineFormViewModel SaveAllotmentDetailByOnlineSchemeFormId(OnlineFormViewModel model);

        Kendo.Mvc.UI.DataSourceResult GetDropDownListByTypeAsDataSource(Kendo.Mvc.UI.DataSourceRequest request, DropdownViewModel model);

        OnlinePaymentViewModel SaveOnlinePaymentTransaction(OnlinePaymentViewModel model);

        OnlinePaymentViewModel SaveOnlinePaymentTransactionReturn(System.Web.Mvc.FormCollection form);

        DataSourceResult GetAllEpassList(DataSourceRequest request, EpassViewModel model);

        DataSourceResult GetSchemeFormChallanAsDataSource(DataSourceRequest request, OnlineFormViewModel model);

        EpassViewModel SaveEpassFormDetail(EpassViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase idfile, HttpPostedFileBase rcfile, HttpPostedFileBase dlfile);

        EpassViewModel GetEpassFormDetailById(EpassViewModel model);

        int ValidateEpass(EpassViewModel model);

        EpassViewModel EpassDetails(int? Id);
    }
}
