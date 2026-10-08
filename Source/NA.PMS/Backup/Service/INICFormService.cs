using Kendo.Mvc.UI;
using NA.PMS.NICService.Context;
//using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.NICServices
{
    public interface INICFormService
    {
        SWPFormViewModel GetSchemeBasicInfoData(SWPFormViewModel naonlineform);

        SWPFormViewModel GetOpenEndedSchemeFormDataById(SWPFormViewModel nicdetail);

        SWPFormViewModel SaveOpenEndedSchemeFormDetail(SWPFormViewModel model, System.Web.HttpPostedFileBase userImage, System.Web.HttpPostedFileBase signatureImage);

        SWPFormViewModel SaveProposedProjectAndRefundDetail(SWPFormViewModel model);

        SWPFormViewModel SaveUploadedDocument(SWPFormViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files, System.Web.HttpPostedFileBase userImage, System.Web.HttpPostedFileBase signatureImage);

        int RemoveDocumentFromApplicationForm(string formNo, string filename);

        SWPDirectorViewModel SaveDirectorDetailsForOpenScheme(SWPDirectorViewModel model);

        DataSourceResult GetFloorAreaListAsDataSource(DataSourceRequest request, SWPDropdownViewModel model);

        DataSourceResult GetUploadedDocumentsByFormId(DataSourceRequest request, int? formId);

        DataSourceResult GetDirectorDetailsAsDataSourceByFormId(DataSourceRequest request, int? id);

        IEnumerable<SWPDropdownViewModel> GetFloorAreaRangeList(SWPDropdownViewModel model);

        SWPFormViewModel GetApplicationFormFeeAndProcessingCharge(SWPFormViewModel model);

        SWPFormViewModel ValidatePANForApplicationForm(SWPFormViewModel model);

        int GetProcessingAndReservationMoneyPaymentStatus(string formNo);

        SWPFormViewModel GetBankAccountDetails(SWPFormViewModel model);

        SWPChallanViewModel SaveOfflinePaymentTransactionForChallan(SWPFormViewModel model);

        SWPFormViewModel SaveOnlineSchemePaymentStatus(SWPFormViewModel model, HttpPostedFileBase challan);

        DataSourceResult GetOnlineSchemeListAsDataSource(DataSourceRequest request, SWPDropdownViewModel model);

        DataSourceResult GetDepartmentBySchemeAsDataSource(DataSourceRequest request, SWPDropdownViewModel model);

        DataSourceResult GetOnlineSchemeApplicationAsDataSource(DataSourceRequest request, SWPFormViewModel modal);

        DataSourceResult GetOnlineSchemeFormPaymentAsDataSource(DataSourceRequest request, SWPFormViewModel modal);

        SWPPaymentViewModel GetSchemeFormPaymentTransaction(SWPPaymentViewModel model);

        SWPPaymentViewModel ValidateSchemeFormChallan(SWPPaymentViewModel model);

        SWPFormViewModel GetOnlineSchemeFormDataById(SWPFormViewModel model);

        SWPFormViewModel SaveOnlineSchemeFormStatus(SWPFormViewModel model);

        SWPFormViewModel SaveOnlineSchemeFormProcessRequest(SWPFormViewModel model);

        SWPFormViewModel SaveAllotmentDetailByOnlineSchemeFormId(SWPFormViewModel model);

        DataSourceResult GetDropDownListByTypeAsDataSource(DataSourceRequest request, SWPDropdownViewModel model);

        SWPPaymentViewModel SaveOnlinePaymentTransaction(SWPPaymentViewModel model);

        SWPPaymentViewModel SaveOnlinePaymentTransactionReturn(System.Web.Mvc.FormCollection form);

        DataSourceResult GetSchemeFormChallanAsDataSource(DataSourceRequest request, SWPFormViewModel modal);

        SWPEpassViewModel SaveEpassFormDetail(SWPEpassViewModel model, HttpPostedFileBase userImage, HttpPostedFileBase idfile, HttpPostedFileBase rcfile, HttpPostedFileBase dlfile);

        SWPEpassViewModel GetEpassFormDetailById(SWPEpassViewModel model);
        DataSourceResult GetAllEpassList(DataSourceRequest request, SWPEpassViewModel model);

        int ValidateEpass(SWPEpassViewModel model);

        SWPEpassViewModel EpassDetails(int? Id);

        SWPFormViewModel GetSchemeInformationForOnlineApplication(SWPFormViewModel SWPFormViewModel);

        SWPFormViewModel GetInitialDataForScheme(SWPFormViewModel SWPFormViewModel);

        SWPFormViewModel GetInitialDataForScheme();

        List<SWPDropdownViewModel> GetAreaRangeByDepartment(int schemeId, int departmentId);

        SWPFormViewModel GetApplicationFeeAndCharges(int? schemeId, int? departmentId, int? propertyTypeId, int? areaTypeId);

        int ValidatePANnumber(string pan, int? areaId, int? schemeId);

        NICsingalwindowSystem GetNICSingleWindowTableData(SWPFormViewModel model);

        DataSourceResult GetUploadedDocumentsAfterScrutiny(DataSourceRequest request, int? formId, int? checklistStartId, int? checklistEndId);
    }
}
 