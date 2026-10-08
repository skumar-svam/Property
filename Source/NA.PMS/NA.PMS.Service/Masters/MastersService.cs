using NA.PMS.Model;
using NA.PMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kendo.Mvc.UI;


namespace NA.PMS.Service
{
    public class MastersService : IMastersService
    {
        IMastersRepositiory _mastersRepository;

        public MastersService()
        {
            _mastersRepository = new MastersRepositiory();
        }

        #region RefundScheme by Shatrughna

        public DataSourceResult GetSchemeRefundDetails(DataSourceRequest request)
        {
            return _mastersRepository.GetSchemeRefundDetails(request);
        }

        public List<SchemeModel> GetAllSchemesForRefund()
        {
            return _mastersRepository.GetAllSchemesForRefund();
        }

        public List<MediaModel> GetMediaTypesForRefund()
        {
            return _mastersRepository.GetMediaTypesForRefund();
        }

        public List<DeductionApplyOnForRefund> GetDeductionApplyOnForRefund()
        {
            return _mastersRepository.GetDeductionApplyOnForRefund();
        }

        public List<UnitTypeForRefund> GetUnitTypeForRefund()
        {
            return _mastersRepository.GetUnitTypeForRefund();
        }

        public List<DepartmentRefund> GetDepartmentForRefund()
        {
            return _mastersRepository.GetDepartmentForRefund();
        }

        public List<DepartmentRefund> GetDepartmentOnSchemeForRefund(int schemeId)
        {
            return _mastersRepository.GetDepartmentOnSchemeForRefund(schemeId);
        }

        public Refund GetRefundDetailById(int id)
        {
            return _mastersRepository.GetRefundDetailById(id);
        }

        public bool SaveRefundForScheme(Refund refund, string loginUser)
        {
            return _mastersRepository.SaveRefundForScheme(refund, loginUser);
        }

        public bool RemoveRefundForScheme(int refundId)
        {
            return _mastersRepository.RemoveRefundForScheme(refundId);
        }

        #endregion
        public List<PropertyModel> GetPropertyBankDetailAdd(int? schemeId, int? deptId, int? propertyTypeId, int? sectorId, int? blockId, int? floorId)
        {
            return _mastersRepository.GetPropertyBankDetailAdd(schemeId, deptId, propertyTypeId, sectorId, blockId, floorId);
        }
        public List<PropertyModel> GetPropertyBankDetail(int? schemeId, int? deptId, int? propertyTypeId, int? sectorId, int? blockId, int? floorId)
        {
            return _mastersRepository.GetPropertyBankDetail(schemeId, deptId, propertyTypeId, sectorId, blockId, floorId);
        }
        public List<PropertyModel> GetDetachedPropertyDetail(int? schemeId, int? deptId, int? propertyTypeId, int? sectorId, int? blockId, int? floorId)
        {
            return _mastersRepository.GetDetachedPropertyDetail(schemeId, deptId, propertyTypeId, sectorId, blockId, floorId);
        }
        public bool AttachProperties(List<int> rIds, string refIds, int schemeId, string registry)
        {
            return _mastersRepository.AttachProperties(rIds, refIds, schemeId, registry);
        }
        public bool DetachProperties(List<int> refIds)
        {
            return _mastersRepository.DetachProperties(refIds);
        }
        public List<PropertyModel> GetPropertyDetail()
        {
            return _mastersRepository.GetPropertyDetail();
        }
        public DataSourceResult GetPropertyDetail_Read(DataSourceRequest request)
        {
            return _mastersRepository.GetPropertyDetail_Read(request);
        }
        public bool RemovePropertyDetail(int propertyId)
        {
            return _mastersRepository.RemovePropertyDetail(propertyId);
        }
        public List<PropertyModel> GetSchemes()
        {
            return _mastersRepository.GetSchemes();
        }
        public List<PropertyModel> GetPropertyType()
        {
            return _mastersRepository.GetPropertyType();
        }
        public List<PropertyModel> GetLocations()
        {
            return _mastersRepository.GetLocations();
        }
        //public DataSourceResult GetAllScheme(DataSourceRequest sourceReq)
        //{
        //    return _mastersRepository.GetAllScheme(sourceReq);
        //}
        public List<PropertyModel> GetDepartment(int schemeId)
        {
            return _mastersRepository.GetDepartment(schemeId);
        }
        public bool SavePropertyDetail(PropertyModel propertyDetail)
        {
            return _mastersRepository.SavePropertyDetail(propertyDetail);
        }
        public PropertyModel GetLandRate(int schemeId, int departmentId, int floorId, int blockId, int sectorId, int propertyTypeId)
        {
            return _mastersRepository.GetLandRate(schemeId, departmentId, floorId, blockId, sectorId, propertyTypeId);
        }
        public List<PropertyModel> GetPropertyTypeBySchemeAndDeptId(int schemeId, int departmentId)
        {
            return _mastersRepository.GetPropertyTypeBySchemeAndDeptId(schemeId, departmentId);
        }
        public List<PropertyModel> GetSectorBySchemeAndDeptId(int schemeId, int departmentId, int propTypeId)
        {
            return _mastersRepository.GetSectorBySchemeAndDeptId(schemeId, departmentId, propTypeId);
        }
        public List<PropertyModel> GetFloorBySchemeAndDeptId(int schemeId, int departmentId, int propTypeId, int sectorId, int blockId)
        {
            return _mastersRepository.GetFloorBySchemeAndDeptId(schemeId, departmentId, propTypeId, sectorId, blockId);
        }
        public List<PropertyModel> GetBlockBySchemeAndDeptId(int schemeId, int departmentId, int propTypeId, int sectorId)
        {
            return _mastersRepository.GetBlockBySchemeAndDeptId(schemeId, departmentId, propTypeId, sectorId);
        }
        public List<NotificationsModel> GetAllNotifications(DataSourceRequest request)
        {
            return _mastersRepository.GetAllNotifications(request);
        }
        public DataSourceResult GetAllNotifications_Read(DataSourceRequest request)
        {
            return _mastersRepository.GetAllNotifications_Read(request);
        }
        public NotificationsModel ViewSchemeNotification(int notificationId)
        {
            return _mastersRepository.ViewSchemeNotification(notificationId);
        }

        public List<SchemeModel> GetAllSchemesForNotifications(DataSourceRequest request)
        {
            return _mastersRepository.GetAllSchemesForNotifications(request);
        }

        public List<MediaModel> GetMediaTypeForNotification()
        {
            return _mastersRepository.GetMediaTypeForNotification();
        }

        public List<PaymentModel> GetPaymentModeForNotifications()
        {
            return _mastersRepository.GetPaymentModeForNotifications();
        }

        public int SaveNotificationForScheme(NotificationsModel notification, string user)
        {
            return _mastersRepository.SaveNotificationForScheme(notification, user);
        }

        public bool RemoveNotification(int notificationID)
        {
            return _mastersRepository.RemoveNotification(notificationID);
        }

        public List<NotificationsModel> GetOldNotifications(int schemeId, int notiId)
        {
            return _mastersRepository.GetOldNotifications(schemeId, notiId);
        }

        public bool CheckDuplicateProperty(int secotrId, int blockId, string plotNo, int refId)
        {
            return _mastersRepository.CheckDuplicateProperty(secotrId, blockId, plotNo, refId);
        }

        public bool CompareNotiStartAndEndDate(DateTime notiStartDate, DateTime notiEndDate, int schemeId)
        {
            return _mastersRepository.CompareNotiStartAndEndDate(notiStartDate, notiEndDate, schemeId);
        }

        public int AddCircleRate(int departmentId, int sector, decimal rate, DateTime startDate, int blockId)
        {
            return _mastersRepository.AddCircleRate(departmentId, sector, rate, startDate, blockId);
        }

        public DataSourceResult GetCircleRate(DataSourceRequest request)
        {
            return _mastersRepository.GetCircleRate(request);
        }
        public bool RemoveCircleRate(int refId)
        {
            return _mastersRepository.RemoveCircleRate(refId);
        }

        public SubleaseModel CheckRIDValidity(int parentPropRId)
        {
            return _mastersRepository.CheckRIDValidity(parentPropRId);
        }

        public NotificationsModel GetSchemeDateForNotification(int schemeId)
        {
            return _mastersRepository.GetSchemeDateForNotification(schemeId);
        }

        public List<DDList> GetDepartmentsForSubLease()
        {
            return _mastersRepository.GetDepartmentsForSubLease();
        }

        public bool SaveSchemeNotification(NotificationsModel notification)
        {
            return _mastersRepository.SaveSchemeNotification(notification);
        }

        public bool SaveSubLease(int parentPropRId, int dept, string subLeasePropNo, decimal areaRate, decimal propArea, decimal propCost, int areaRangeId)
        {
            return _mastersRepository.SaveSubLease(parentPropRId, dept, subLeasePropNo, areaRate, propArea, propCost, areaRangeId);
        }

        public bool CheckNotificationPublishDate(int schemeId, DateTime notificationStart, DateTime notificationEnd, DateTime publishDate)
        {
            return _mastersRepository.CheckNotificationPublishDate(schemeId, notificationStart, notificationEnd, publishDate);
        }

        public DataSourceResult GetSubLeaseData(DataSourceRequest request, int rid)
        {
            return _mastersRepository.GetSubLeaseData(request, rid);
        }

        public bool RemoveSubLease(int Id)
        {
            return _mastersRepository.RemoveSubLease(Id);
        }


        public DataSourceResult GetSubLeasePropertyList(DataSourceRequest request)
        {
            return _mastersRepository.GetSubLeasePropertyList(request);
        }


        public SubLeaseViewModel GetSubleasePropertyDetail(string id)
        {
            return _mastersRepository.GetSubleasePropertyDetail(id);
        }

        public bool AddSectorBlock(SectorBlock objSectorBlock)
        {
            return _mastersRepository.AddSectorBlock(objSectorBlock);
        }

        public List<MasterTypeForSectorBlock> GetDDLSectorBlock()
        {
            return _mastersRepository.GetDDLSectorBlock();
        }


        public SubLeaseViewModel GetParentPropertyDetailById(int rid)
        {
            return _mastersRepository.GetParentPropertyDetailById(rid);
        }


        public DataSourceResult GetSubLeasedProperty(DataSourceRequest request, int rid)
        {
            return _mastersRepository.GetSubLeasedProperty(request, rid);
        }


        public int SaveSubLeaseProperty(SubLeaseViewModel model)
        {
            return _mastersRepository.SaveSubLeaseProperty(model);
        }


        public DataSourceResult GetRegistrationIdListForSubLease(DataSourceRequest request)
        {
            return _mastersRepository.GetRegistrationIdListForSubLease(request);
        }


        public int SubLeasePlotActivation(int Id)
        {
            return _mastersRepository.SubLeasePlotActivation(Id);
        }

        public List<DropdownViewModel> GetProjectsByParentRid(int ParentPropertyId)
        {
            return _mastersRepository.GetProjectsByParentRid(ParentPropertyId);
        }

        public int SavePropertyProject(ProjectViewModel model)
        {
            return _mastersRepository.SavePropertyProject(model);
        }

        public DataSourceResult GetProjectsByRidForGrid(DataSourceRequest Req, int Rid)
        {
            return _mastersRepository.GetProjectsByRidForGrid(Req, Rid);
        }

        public DataSourceResult GetRIDsForProject(DataSourceRequest Request)
        {
            return _mastersRepository.GetRIDsForProject(Request);
        }

        public int UpdateProjectStatus(ProjectModel ProjectModel)
        {
            return _mastersRepository.UpdateProjectStatus(ProjectModel);
        }


        public PropertyModel GetPropertyDetailById(int refId)
        {
            return _mastersRepository.GetPropertyDetailById(refId);
        }


        public DataSourceResult GetMasterSearchParameterAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            return _mastersRepository.GetMasterSearchParameterAsDataSource(request, model);
        }

        public DataSourceResult GetMasterPropertyList(DataSourceRequest request, PropertyViewModel model)
        {
            return _mastersRepository.GetMasterPropertyList(request, model);
        }


        public DataSourceResult GetPropertyRateListAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            return _mastersRepository.GetPropertyRateListAsDataSource(request, model);
        }

        public int SavePropertyRateDetail(PropertyViewModel model)
        {
            return _mastersRepository.SavePropertyRateDetail(model);
        }

        public int RemovePropertyRateDetailById(PropertyViewModel model)
        {
            return _mastersRepository.RemovePropertyRateDetailById(model);
        }

        public DataSourceResult GetPropertyTypeList(DataSourceRequest request, PropertyViewModel model)
        {
            return _mastersRepository.GetPropertyTypeList(request, model);
        }


        public int SavePropertyTypeByDepartment(PropertyViewModel model)
        {
            return _mastersRepository.SavePropertyTypeByDepartment(model);
        }

        public int ChangeStatusOfPropertyType(PropertyViewModel model)
        {
            return _mastersRepository.ChangeStatusOfPropertyType(model);
        }


        public SchemePropertyModel GetPropertyDetailsById(int? propertyId)
        {
            return _mastersRepository.GetPropertyDetailsById(propertyId);
        }


        public int UpdatePropertyDetail(PropertyDetailViewModel model)
        {
            return _mastersRepository.UpdatePropertyDetail(model);
        }


        public SchemePropertyModel GetPropertyCostDetailsById(int? propertyId)
        {
            return _mastersRepository.GetPropertyCostDetailsById(propertyId);
        }

        public int UpdatePropertyCostDetail(PropertyDetailViewModel model)
        {
            return _mastersRepository.UpdatePropertyCostDetail(model);
        }

        public DataSourceResult GetMasterPropertyCostList(DataSourceRequest request, PropertyViewModel model)
        {
            return _mastersRepository.GetMasterPropertyCostList(request,model);
        }


        public int SaveAllottedPropertyDetail(PropertyViewModel model)
        {
            return _mastersRepository.SaveAllottedPropertyDetail(model);
        }

        public DataSourceResult GetPropertyDetailAsDataSource(DataSourceRequest request, PropertyViewModel model)
        {
            return _mastersRepository.GetPropertyDetailAsDataSource(request, model);
        }


        public PropertyViewModel GetPropertyCostDetailById(PropertyViewModel model)
        {
            return _mastersRepository.GetPropertyCostDetailById(model);
        }


        public int SavePropertyDocuments(DocumentViewModel model, IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            return _mastersRepository.SavePropertyDocuments(model,files);
        }

        public int SaveDocumentsInTempSession(DocumentViewModel model, System.Web.HttpPostedFileBase tempFile)
        {
            return _mastersRepository.SaveDocumentsInTempSession(model, tempFile);
        }

        public DataSourceResult GetDocumentListFromTempSessionAsDataSource(DataSourceRequest request, DocumentViewModel model)
        {
            return _mastersRepository.GetDocumentListFromTempSessionAsDataSource(request, model);
        }

        public DataSourceResult GetDocumentTypeListAsDataSource(DataSourceRequest request, DropdownViewModel model)
        {
            return _mastersRepository.GetDocumentTypeListAsDataSource(request, model);
        }


        public int RemoveDocumentByIdFromTempSession(DocumentViewModel model)
        {
            return _mastersRepository.RemoveDocumentByIdFromTempSession(model);
        }


        public DataSourceResult GetExistingDocumentListAsDataSource(DataSourceRequest request, DocumentViewModel model)
        {
            return _mastersRepository.GetExistingDocumentListAsDataSource(request, model);
        }
    }
}
