using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc.UI;


namespace NA.PMS.Repository
{
    public interface IMastersRepositiory
    {
        // Scheme Refunds
        DataSourceResult GetSchemeRefundDetails(DataSourceRequest request);
        Refund GetRefundDetailById(int id);
        List<SchemeModel> GetAllSchemesForRefund();
        List<MediaModel> GetMediaTypesForRefund();
        List<DepartmentRefund> GetDepartmentForRefund();
        List<DepartmentRefund> GetDepartmentOnSchemeForRefund(int schemeId);
        List<DeductionApplyOnForRefund> GetDeductionApplyOnForRefund();
        List<UnitTypeForRefund> GetUnitTypeForRefund();
        bool SaveRefundForScheme(Refund refund, string loginUser);
        bool RemoveRefundForScheme(int refundId);

        // Scheme Property
        List<PropertyModel> GetPropertyBankDetailAdd(int? schemeId, int? deptId, int? propertyTypeId, int? sectorId, int? blockId, int? floorId);
       List<PropertyModel> GetPropertyBankDetail(int? schemeId, int? deptId, int? propertyTypeId, int? sectorId, int? blockId, int? floorId);
       List<PropertyModel> GetDetachedPropertyDetail(int? schemeId, int? deptId, int? propertyTypeId, int? sectorId, int? blockId, int? floorId);
        bool AttachProperties(List<int> rIds, string refIds, int schemeId, string registry);
        bool DetachProperties(List<int> refIds);
        List<PropertyModel> GetPropertyDetail();
        DataSourceResult GetPropertyDetail_Read(DataSourceRequest request);
        bool RemovePropertyDetail(int propertyId);
        List<PropertyModel> GetSchemes();
        List<PropertyModel> GetPropertyType();
        List<PropertyModel> GetLocations();
        //DataSourceResult GetAllScheme(DataSourceRequest sourceReq);
        List<PropertyModel> GetDepartment(int schemeId);
        List<PropertyModel> GetPropertyTypeBySchemeAndDeptId(int schemeId, int departmentId);
        List<PropertyModel> GetSectorBySchemeAndDeptId(int schemeId, int departmentId, int propTypeId);
        List<PropertyModel> GetBlockBySchemeAndDeptId(int schemeId, int departmentId, int propTypeId, int sectorId);
        List<PropertyModel> GetFloorBySchemeAndDeptId(int schemeId, int departmentId, int propTypeId, int sectorId, int blockId);
        bool SavePropertyDetail(PropertyModel propertyDetail);
        PropertyModel GetLandRate(int schemeId, int departmentId, int floorId, int blockId, int sectorId, int propertyTypeId);

        // Scheme Notifications
        List<NotificationsModel> GetAllNotifications(DataSourceRequest request);
        DataSourceResult GetAllNotifications_Read(DataSourceRequest request);
        NotificationsModel ViewSchemeNotification(int notificationId);
        List<SchemeModel> GetAllSchemesForNotifications(DataSourceRequest request);
        List<MediaModel> GetMediaTypeForNotification();
        List<PaymentModel> GetPaymentModeForNotifications();
        int SaveNotificationForScheme(NotificationsModel notification, string user);
        SubleaseModel CheckRIDValidity(int parentPropRId);
        bool RemoveNotification(int notificationID);
        List<NotificationsModel> GetOldNotifications(int schemeId, int notiId);
        List<DDList> GetDepartmentsForSubLease();
        bool CheckDuplicateProperty(int secotrId, int blockId, string plotNo, int refId);
        bool SaveSubLease(int parentPropRId, int dept, string subLeasePropNo, decimal areaRate, decimal propArea, decimal propCost, int areaRangeId);
        bool CompareNotiStartAndEndDate(DateTime notiStartDate, DateTime notiEndDate, int schemeId);
        DataSourceResult GetSubLeaseData(DataSourceRequest request, int rid);
        int AddCircleRate(int departmentId, int sector, decimal rate, DateTime startDate, int blockId);
        DataSourceResult GetCircleRate(DataSourceRequest request);
        bool RemoveCircleRate(int refId);
        bool RemoveSubLease(int Id);

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

        int UpdatePropertyCostDetail(PropertyDetailViewModel model);

        SchemePropertyModel GetPropertyCostDetailsById(int? propertyId);

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
