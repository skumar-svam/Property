using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc.UI;


namespace NA.PMS.Service
{
    public interface IMastersService
    {
        DataSourceResult GetSchemeRefundDetails(DataSourceRequest request);
        Refund GetRefundDetailById(int id);
        Boolean SaveRefundForScheme(Refund refund, string loginUser);
        List<SchemeModel> GetAllSchemesForRefund();
        List<MediaModel> GetMediaTypesForRefund();
        List<DepartmentRefund> GetDepartmentForRefund();
        List<DepartmentRefund> GetDepartmentOnSchemeForRefund(int schemeId);
        List<DeductionApplyOnForRefund> GetDeductionApplyOnForRefund();
        List<UnitTypeForRefund> GetUnitTypeForRefund();
        bool RemoveRefundForScheme(int refundId);
        bool AttachProperties(List<int> rIds, string refIds, int schemeId, string registry);
        bool DetachProperties(List<int> refIds);
        List<PropertyModel> GetPropertyBankDetailAdd(int? schemeId, int? deptId, int? propertyTypeId, int? sectorId, int? blockId, int? floorId);
        List<PropertyModel> GetPropertyBankDetail(int? schemeId, int? deptId, int? propertyTypeId, int? sectorId, int? blockId, int? floorId);
        List<PropertyModel> GetDetachedPropertyDetail(int? schemeId, int? deptId, int? propertyTypeId, int? sectorId, int? blockId, int? floorId);
        List<PropertyModel> GetPropertyDetail();
        DataSourceResult GetPropertyDetail_Read(DataSourceRequest request);
        bool RemovePropertyDetail(int propertyId);
        List<PropertyModel> GetSchemes();
        List<PropertyModel> GetPropertyType();
        List<PropertyModel> GetLocations();
        List<PropertyModel> GetDepartment(int schemeId);
        List<DDList> GetDepartmentsForSubLease();
        //DataSourceResult GetAllScheme(DataSourceRequest sourceReq);
        bool SavePropertyDetail(PropertyModel propertyDetail);
        PropertyModel GetLandRate(int schemeId, int departmentId, int floorId, int blockId, int sectorId, int propertyTypeId);
        List<PropertyModel> GetPropertyTypeBySchemeAndDeptId(int schemeId, int departmentId);
        List<PropertyModel> GetSectorBySchemeAndDeptId(int schemeId, int departmentId, int propTypeId);
        List<PropertyModel> GetBlockBySchemeAndDeptId(int schemeId, int departmentId, int propTypeId, int sectorId);
        List<PropertyModel> GetFloorBySchemeAndDeptId(int schemeId, int departmentId, int propTypeId, int sectorId, int blockId);
        SubleaseModel CheckRIDValidity(int parentPropRId);
        List<NotificationsModel> GetAllNotifications(DataSourceRequest request);
        DataSourceResult GetAllNotifications_Read(DataSourceRequest request);
        //NotificationsModel EditSchemeNotification(int notificationId);
        bool SaveSubLease(int parentPropRId, int dept, string subLeasePropNo, decimal areaRate, decimal propArea, decimal propCost, int areaRangeId);
        NotificationsModel ViewSchemeNotification(int notificationId);
        List<SchemeModel> GetAllSchemesForNotifications(DataSourceRequest request);
        DataSourceResult GetSubLeaseData(DataSourceRequest request, int rid);
        List<MediaModel> GetMediaTypeForNotification();
        bool RemoveSubLease(int Id);
        List<PaymentModel> GetPaymentModeForNotifications();

        int SaveNotificationForScheme(NotificationsModel notification, string user);

        bool RemoveNotification(int notificationID);

        List<NotificationsModel> GetOldNotifications(int schemeId, int notiId);

        bool CheckDuplicateProperty(int secotrId, int blockId, string plotNo, int refId);

        bool CompareNotiStartAndEndDate(DateTime notiStartDate, DateTime notiEndDate, int schemeId);

        int AddCircleRate(int departmentId, int sector, decimal rate, DateTime startDate, int blockId);

        DataSourceResult GetCircleRate(DataSourceRequest request);

        bool RemoveCircleRate(int refId);

        NotificationsModel GetSchemeDateForNotification(int schemeId);

        bool SaveSchemeNotification(NotificationsModel notification);

        bool CheckNotificationPublishDate(int schemeId, DateTime notificationStart, DateTime notificationEnd, DateTime publishDate);

        DataSourceResult GetSubLeasePropertyList(DataSourceRequest request);

        SubLeaseViewModel GetSubleasePropertyDetail(string id);

        bool AddSectorBlock(SectorBlock objSectorBlock);
        List<MasterTypeForSectorBlock> GetDDLSectorBlock();

        SubLeaseViewModel GetParentPropertyDetailById(int rid);

        DataSourceResult GetSubLeasedProperty(DataSourceRequest request, int rid);

        int SaveSubLeaseProperty(SubLeaseViewModel model);

        DataSourceResult GetRegistrationIdListForSubLease(DataSourceRequest request);

        int SubLeasePlotActivation(int Id);
        List<DropdownViewModel> GetProjectsByParentRid(int ParentPropertyId);
        int SavePropertyProject(ProjectViewModel model);
        DataSourceResult GetProjectsByRidForGrid(DataSourceRequest Req, int Rid);
        DataSourceResult GetRIDsForProject(DataSourceRequest Request);
        int UpdateProjectStatus(ProjectModel ProjectModel);

        PropertyModel GetPropertyDetailById(int refId);

        DataSourceResult GetMasterSearchParameterAsDataSource(DataSourceRequest request, PropertyViewModel model);

        DataSourceResult GetMasterPropertyList(DataSourceRequest request, PropertyViewModel model);

        DataSourceResult GetPropertyRateListAsDataSource(DataSourceRequest request, PropertyViewModel model);

        int SavePropertyRateDetail(PropertyViewModel model);

        int RemovePropertyRateDetailById(PropertyViewModel model);

        DataSourceResult GetPropertyTypeList(DataSourceRequest request, PropertyViewModel model);

        int SavePropertyTypeByDepartment(PropertyViewModel model);

        int ChangeStatusOfPropertyType(PropertyViewModel model);

        SchemePropertyModel GetPropertyDetailsById(int? propertyId);

        int UpdatePropertyDetail(PropertyDetailViewModel model);

        SchemePropertyModel GetPropertyCostDetailsById(int? propertyId);

        int UpdatePropertyCostDetail(PropertyDetailViewModel model);

        DataSourceResult GetMasterPropertyCostList(DataSourceRequest request, PropertyViewModel model);

        int SaveAllottedPropertyDetail(PropertyViewModel model);

        DataSourceResult GetPropertyDetailAsDataSource(DataSourceRequest request, PropertyViewModel model);

        PropertyViewModel GetPropertyCostDetailById(PropertyViewModel model);

        int SavePropertyDocuments(DocumentViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files);

        int SaveDocumentsInTempSession(DocumentViewModel model, System.Web.HttpPostedFileBase tempFile);

        DataSourceResult GetDocumentListFromTempSessionAsDataSource(DataSourceRequest request, DocumentViewModel model);

        DataSourceResult GetDocumentTypeListAsDataSource(DataSourceRequest request, DropdownViewModel model);

        int RemoveDocumentByIdFromTempSession(DocumentViewModel model);

        DataSourceResult GetExistingDocumentListAsDataSource(DataSourceRequest request, DocumentViewModel model);
    }
}
