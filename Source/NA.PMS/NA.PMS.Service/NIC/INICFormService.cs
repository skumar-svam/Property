using Kendo.Mvc.UI;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Service
{
    public interface INICFormService
    {
        OnlineFormViewModel GetSchemeBasicInfoData(OnlineFormViewModel naonlineform);

        OnlineFormViewModel GetOpenEndedSchemeFormDataById(OnlineFormViewModel nicdetail);

        OnlineFormViewModel SaveOpenEndedSchemeFormDetail(OnlineFormViewModel model, System.Web.HttpPostedFileBase userImage, System.Web.HttpPostedFileBase signatureImage);

        OnlineFormViewModel SaveProposedProjectAndRefundDetail(OnlineFormViewModel model);

        OnlineFormViewModel SaveUploadedDocument(OnlineFormViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files, System.Web.HttpPostedFileBase userImage, System.Web.HttpPostedFileBase signatureImage);

        int RemoveDocumentFromApplicationForm(string formNo, string filename);

        onlineDirectorViewModel SaveDirectorDetailsForOpenScheme(onlineDirectorViewModel model);

        DataSourceResult GetFloorAreaListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        DataSourceResult GetUploadedDocumentsByFormId(DataSourceRequest request, int? formId);

        DataSourceResult GetDirectorDetailsAsDataSourceByFormId(DataSourceRequest request, int? id);

        IEnumerable<DropdownViewModel> GetFloorAreaRangeList(DropdownViewModel model);

        OnlineFormViewModel GetApplicationFormFeeAndProcessingCharge(OnlineFormViewModel model);

        OnlineFormViewModel ValidatePANForApplicationForm(OnlineFormViewModel model);

        int GetProcessingAndReservationMoneyPaymentStatus(string formNo);

        OnlineFormViewModel GetBankAccountDetails(OnlineFormViewModel model);

        OnlineChallanViewModel SaveOfflinePaymentTransactionForChallan(OnlineFormViewModel model);

        OnlineFormViewModel SaveOnlineSchemePaymentStatus(OnlineFormViewModel model, HttpPostedFileBase challan);

        DataSourceResult GetOnlineSchemeListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        DataSourceResult GetDepartmentBySchemeAsDataSource(DataSourceRequest request, DropdownViewModel model);

        DataSourceResult GetOnlineSchemeApplicationAsDataSource(DataSourceRequest request, OnlineFormViewModel modal);

        DataSourceResult GetOnlineSchemeFormPaymentAsDataSource(DataSourceRequest request, OnlineFormViewModel modal);

        OnlinePaymentViewModel GetSchemeFormPaymentTransaction(OnlinePaymentViewModel model);

        OnlinePaymentViewModel ValidateSchemeFormChallan(OnlinePaymentViewModel model);

        OnlineFormViewModel GetOnlineSchemeFormDataById(OnlineFormViewModel model);

        OnlineFormViewModel SaveOnlineSchemeFormStatus(OnlineFormViewModel model);

        OnlineFormViewModel SaveOnlineSchemeFormProcessRequest(OnlineFormViewModel model);

        OnlineFormViewModel SaveAllotmentDetailByOnlineSchemeFormId(OnlineFormViewModel model);

        DataSourceResult GetDropDownListByTypeAsDataSource(DataSourceRequest request, DropdownViewModel model);

        OnlinePaymentViewModel SaveOnlinePaymentTransaction(OnlinePaymentViewModel model);

        OnlinePaymentViewModel SaveOnlinePaymentTransactionReturn(System.Web.Mvc.FormCollection form);

        DataSourceResult GetSchemeFormChallanAsDataSource(DataSourceRequest request, OnlineFormViewModel modal);

        EpassViewModel SaveEpassFormDetail(EpassViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase idfile, HttpPostedFileBase rcfile, HttpPostedFileBase dlfile);

        EpassViewModel GetEpassFormDetailById(EpassViewModel model);
        DataSourceResult GetAllEpassList(DataSourceRequest request, EpassViewModel model);

        int ValidateEpass(EpassViewModel model);

        EpassViewModel EpassDetails(int? Id);
    }
}
 