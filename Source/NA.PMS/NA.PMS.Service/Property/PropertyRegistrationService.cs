using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Model;
using NA.PMS.Model.CommonModel;
using NA.PMS.Service.BusinessRuleEngine;
using NA.PMS.Repository;

namespace NA.PMS.Service.Property
{
    public class PropertyRegistrationService : IPropertyRegistrationService
    {
        IPropertyRegistrationRepository _propertyRegistrationRepository;
        IAllotmentEngine _allotmentEngineService;
        IPaymentEngine _paymentEngineService;

        public PropertyRegistrationService(IPaymentEngine paymentEngine, IAllotmentEngine allotmentEngine, IPropertyRegistrationRepository propertyRegistrationRepository)
        {
            _propertyRegistrationRepository = propertyRegistrationRepository;
            _allotmentEngineService = allotmentEngine;
            _paymentEngineService = paymentEngine;
        }

        public DataSourceResult GetPossessionData(DataSourceRequest req)
        {
            return _propertyRegistrationRepository.GetPossessionData(req);
        }

        // Get CheckList Type
        public IEnumerable<CommonListModel> GetCheckListType()
        {
            return _propertyRegistrationRepository.GetCheckListType();
        }

        // Get all RIDs 
        public IEnumerable<CommonListModel> GetAllRIDsForLeaseDeed()
        {
            return _propertyRegistrationRepository.GetAllRIDsForLeaseDeed();
        }

        // Getting Property details by RID for Lease Deed
        public CheckList GetCheckListDetailsByRId(int rId)
        {
            return _propertyRegistrationRepository.GetCheckListDetailsByRId(rId);
        }

        // Get all lease deed properties
        public List<LeaseDeedProperty> GetAllLeaseDeadProperties()
        {
            return _propertyRegistrationRepository.GetAllLeaseDeadProperties();
        }

        // Get lease deed property details by RID
        public LeaseDeedProperty GetLeaseDeadPropertyDetailsByRId(int rId)
        {
            return _propertyRegistrationRepository.GetLeaseDeadPropertyDetailsByRId(rId);
        }

        // Add/Update lease deed property details by RID ---- Add Lease deed Execution date
        public LeaseDeedProperty AddLeaseDeadPropertyDetailsByRId(LeaseDeedProperty leaseDeadProperty)
        {
            return _propertyRegistrationRepository.AddLeaseDeadPropertyDetailsByRId(leaseDeadProperty);
        }

        public DataSourceResult GetPropertyRentList(DataSourceRequest request)
        {
            return _propertyRegistrationRepository.GetPropertyRentList(request);
        }

        public DataSourceResult GetPropertyRentListByRid(DataSourceRequest request, int Rid)
        {
            return _propertyRegistrationRepository.GetPropertyRentListByRid(request, Rid);
        }

        public DetailsByRId GetDetailsByRID(int rId)
        {
            DetailsByRId details = _propertyRegistrationRepository.GetDetailsByRID(rId);
            if (details != null)
            {
                details.BalanceDues = _paymentEngineService.GetBalanceDueTillDate(rId, details.DepttID);
                details.PreviousDues = _paymentEngineService.CalculatePreviousDues(rId);
                details.LeaseDeedDueDate = _allotmentEngineService.GetLeaseDeedDueDate(rId);
                details.ChecklistDuedate = _allotmentEngineService.GetChecklistDueDate(rId);
            }
            return details;
        }

        public List<ChecklistDocuments> GetChcklstDocuments(DataSourceRequest request, int propTypeId, int chkLstType)
        {
            return _propertyRegistrationRepository.GetChcklstDocuments(request, propTypeId, chkLstType);
        }

        public bool SaveChecklistDetails(int rId, int chkType, DateTime chkDate, string viewName)
        {
            return _propertyRegistrationRepository.SaveChecklistDetails(rId, chkType, chkDate, viewName);
        }

        public DataSourceResult GetLeaseDeedData(DataSourceRequest req)
        {
            return _propertyRegistrationRepository.GetLeaseDeedData(req);
        }

        public LeaseDeedProperty GetStampDetailsByLeaseExcutionDate(int rId, DateTime LDExecDate)
        {
            //return _propertyRegistrationRepository.GetStampDetailsByLeaseExcutionDate(LDExecDate);
            return _paymentEngineService.CalculateStampDutyAmount(rId, LDExecDate);
        }

        public bool SaveLeaseDeedDetails(int rId, DateTime LDExecDate, string SA, string w1N, string w1A, string w1M, string w2N, string w2A, string w2M, string w3N, string w3A, string w3M, string w4N, string w4A, string w4M, DateTime LDDueDate, string regType, decimal SDAmt, decimal DSDAmt, decimal prevDues, decimal balDueTD, string viewName, int type, decimal docCharges)
        {
            return _propertyRegistrationRepository.SaveLeaseDeedDetails(rId, LDExecDate, SA, w1N, w1A, w1M, w2N, w2A, w2M, w3N, w3A, w3M, w4N, w4A, w4M, LDDueDate, regType, SDAmt, DSDAmt, prevDues, balDueTD, viewName, type, docCharges);
        }

        public LeaseDeedProperty GetLeaseDeedDetailsByRId(int id)
        {
            LeaseDeedProperty details = _propertyRegistrationRepository.GetLeaseDeedDetailsByRId(id);
            if (details != null)
            {
                details.BalanceDues = _paymentEngineService.GetBalanceDueTillDate(details.RId, details.DepttId.Value);
                details.PreviousDues = _paymentEngineService.CalculatePreviousDues(details.RId);
            }
            return details;
            //return _propertyRegistrationRepository.GetLeaseDeedDetailsByRId(id);
        }

        public DataSourceResult GetMutationData(DataSourceRequest req)
        {
            return _propertyRegistrationRepository.GetMutationData(req);
        }

        public DataSourceResult GetMutationDataByApproverId(DataSourceRequest req)
        {
            return _propertyRegistrationRepository.GetMutationDataByApproverId(req);
        }

        public bool SaveMutationDetails(DateTime mutDate, DateTime transDeedDate, string bahiNo, string bahiZildNo, string bahiPageNo, string SINo, int userVal, int ReqNo, int rId, DateTime transDate, string transType, int? ReqRefNo)
        {
            return _propertyRegistrationRepository.SaveMutationDetails(mutDate, transDeedDate, bahiNo, bahiZildNo, bahiPageNo, SINo, userVal, ReqNo, rId, transDate, transType, ReqRefNo);
        }

        public bool UpdateMutationDetails(DateTime mutDate, DateTime transDeedDate, string bahiNo, string bahiZildNo, string bahiPageNo, string SINo, int userVal, int ReqNo, int rId)
        {
            return _propertyRegistrationRepository.UpdateMutationDetails(mutDate, transDeedDate, bahiNo, bahiZildNo, bahiPageNo, SINo, userVal, ReqNo, rId);
        }

        public DataSourceResult GetRIDsForMutation(DataSourceRequest Req)
        {
            return _propertyRegistrationRepository.GetRIDsForMutation(Req);
        }

        public DataSourceResult GetRIDsForTransfer(DataSourceRequest Req)
        {
            return _propertyRegistrationRepository.GetRIDsForTransfer(Req);
        }

        public DataSourceResult GetRIDsForTransfer(DataSourceRequest Req, int Rid)
        {
            return _propertyRegistrationRepository.GetRIDsForTransfer(Req, Rid);
        }

        public DetailsByRId GetTransferDetailsByRID(int rId)
        {
            return _propertyRegistrationRepository.GetTransferDetailsByRID(rId);
        }

        public MutationModel GetMutationDetailsByRId(int reqNo)
        {
            return _propertyRegistrationRepository.GetMutationDetailsByRId(reqNo);
        }

        public bool CancelMutationRequest(int reqNo)
        {
            return _propertyRegistrationRepository.CancelMutationRequest(reqNo);
        }

        public bool IsChecklistGenerated(int rId)
        {
            return _allotmentEngineService.IsCheckListGenerated(rId);
        }

        public bool SaveApprovalStatus(string comments, int intStatus, int ReqNo)
        {
            return _propertyRegistrationRepository.SaveApprovalStatus(comments, intStatus, ReqNo);
        }

        public DateTime? GetChklstDateByChklstType(int rId, int type)
        {
            return _propertyRegistrationRepository.GetChklstDateByChklstType(rId, type);
        }

        public DataSourceResult GetTransferData(DataSourceRequest req)
        {
            return _propertyRegistrationRepository.GetTransferData(req);
        }

        public DataSourceResult GetTransferData(DataSourceRequest req, int? Rid)
        {
            return _propertyRegistrationRepository.GetTransferData(req, Rid);
        }

        public DataSourceResult GetTransferHistory(DataSourceRequest req)
        {
            return _propertyRegistrationRepository.GetTransferHistory(req);
        }

        public TransferModel GetOriginalDetailsForTransferByRID(int rId)
        {
            //return _propertyRegistrationRepository.GetOriginalDetailsForTransferByRID(rId);
            TransferModel details = _propertyRegistrationRepository.GetOriginalDetailsForTransferByRID(rId);
            if (details != null)
            {
                details.BalanceDues = _paymentEngineService.GetBalanceDueTillDate(rId, details.DepttId.Value);
            }
            return details;
        }

        public TransferModel GetOriginalDetailsForTransferOnlyByRID(int rId, int reqNo)
        {
            TransferModel details = _propertyRegistrationRepository.GetOriginalDetailsForTransferOnlyByRID(rId, reqNo);
            if (details != null)
            {
                details.BalanceDues = _paymentEngineService.GetBalanceDueTillDate(rId, details.DepttId.Value);
            }
            return details;
        }

        public DataSourceResult GetPropFunctional(DataSourceRequest sourceReq)
        {
            return _propertyRegistrationRepository.GetPropFunctional(sourceReq);
        }
        public FunctionalModel GetFunctionalDetailsByReqNo(int reqNo)
        {
            return _propertyRegistrationRepository.GetFunctionalDetailsByReqNo(reqNo);
        }
        public DataSourceResult GetRIDsForFunctional(DataSourceRequest Req, int Rid)
        {
            return _propertyRegistrationRepository.GetRIDsForFunctional(Req, Rid);
        }
        public int SaveFunctionalDetails(int rId, DateTime? FunctionalDate, DateTime? FunctionalDueDate, bool MeterSealing, bool Affidavit, bool RegistrationCertificate, bool NDCAccount, string user, string PropertyNumber, decimal? FunctionalCharge, int ReqRefNo, DateTime? completionDate, DateTime? affidavitDate)
        {
            return _propertyRegistrationRepository.SaveFunctionalDetails(rId, FunctionalDate, FunctionalDueDate, MeterSealing, Affidavit, RegistrationCertificate, NDCAccount, user, PropertyNumber, FunctionalCharge, ReqRefNo, completionDate, affidavitDate);
        }
        public bool CancelFunctionRequest(int reqNo)
        {
            return _propertyRegistrationRepository.CancelFunctionRequest(reqNo);
        }
        public int ReSubmitFunctionalDetails(int rId, DateTime? FunctionalDate, DateTime? FunctionalDueDate, bool MeterSealing, bool Affidavit, bool RegistrationCertificate, bool NDCAccount, string user, string PropertyNumber, decimal? FunctionalCharge)
        {
            return _propertyRegistrationRepository.ReSubmitFunctionalDetails(rId, FunctionalDate, FunctionalDueDate, MeterSealing, Affidavit, RegistrationCertificate, NDCAccount, user, PropertyNumber, FunctionalCharge);
        }
        public DataSourceResult GetFunctionalDataByApproverId(DataSourceRequest req)
        {
            return _propertyRegistrationRepository.GetFunctionalDataByApproverId(req);
        }
        public bool SaveFunctionalApprovalStatus(string comments, int intStatus, int ReqNo)
        {
            return _propertyRegistrationRepository.SaveFunctionalApprovalStatus(comments, intStatus, ReqNo);
        }
        public FunctionalModel GetFUnctionalTransferDetailsByRID(int rId)
        {
            return _propertyRegistrationRepository.GetFUnctionalTransferDetailsByRID(rId);
        }

        //To Get All Mortgage record to fill grid.
        public DataSourceResult GetAllMortgage(DataSourceRequest req)
        {
            return _propertyRegistrationRepository.GetAllMortgage(req);
        }

        public DataSourceResult GetAllMortgageByRid(DataSourceRequest req, int Rid)
        {
            return _propertyRegistrationRepository.GetAllMortgageByRid(req, Rid);
        }
        // To Fill Mortgagetype drop down.
        public List<DDList> GetMortgageType()
        {
            return _propertyRegistrationRepository.GetMortgageType();
        }
        // To Fill Mortgage PrevLoan drop down.
        public List<DDList> GetMortgagePrevLoan()
        {
            return _propertyRegistrationRepository.GetMortgagePrevLoan();
        }

        //To Fill Mortgage Prev Loan Drop Down
        public MortgageModel GetPropertyDetailByRid(int rID)
        {
            return _propertyRegistrationRepository.GetPropertyDetailByRid(rID);
        }

        // To Save Mortgage Date
        public bool AddMortgage(DateTime mortgageDate, string bankName, string mortgageType, short previousLoanNoc, string branchAddress, decimal processingFee, decimal sanctionedAmount, string user, int rid, DateTime validUpto, int? reqRefNo)
        {
            return _propertyRegistrationRepository.AddMortgage(mortgageDate, bankName, mortgageType, previousLoanNoc, branchAddress, processingFee, sanctionedAmount, user, rid, validUpto, reqRefNo);
        }

        // To Get Details of All Mortgage by Approver ID.
        public DataSourceResult GetAllMortgageByApprover(DataSourceRequest req)
        {
            return _propertyRegistrationRepository.GetAllMortgageByApprover(req);
        }

        // Get Mortgage by request ID.
        public MortgageModel GetMortgageByRequestID(int requestId)
        {
            return _propertyRegistrationRepository.GetMortgageByRequestID(requestId);
        }

        // To Save comment of approver.
        public bool SaveCommentByRequestID(int requestNo, string Comment, bool acceptReject)
        {
            return _propertyRegistrationRepository.SaveCommentByRequestID(requestNo, Comment, acceptReject);
        }
        // To Update MOrtgage.        
        public bool UpdateMortgage(int requestID, DateTime mortgageDate, string bankName, string mortgageType, short previousLoanNoc, string branchAddress, decimal processingFee, decimal sanctionedAmount, string user, int rid)
        {
            return _propertyRegistrationRepository.UpdateMortgage(requestID, mortgageDate, bankName, mortgageType, previousLoanNoc, branchAddress, processingFee, sanctionedAmount, user, rid);
        }

        // To cancel Mortgage Request
        public bool CancelMortgageRequest(int requestID)
        {
            return _propertyRegistrationRepository.CancelMortgageRequest(requestID);
        }

        // To Invalida Mortgate

        public bool InValidMortgageRequestID(int requestNo, string Comment)
        {
            return _propertyRegistrationRepository.InValidMortgageRequestID(requestNo, Comment);
        }
        //To Generate Mortgage letter
        public string GenerateMortgageLetter(int rid)
        {
            return _propertyRegistrationRepository.GenerateMortgageLetter(rid);
        }
        //To Generate Completion letter
        public string GenerateCompletionLetter(int rid)
        {
            return _propertyRegistrationRepository.GenerateCompletionLetter(rid);
        }

        public string GenerateDemandLetters(int rId)
        {
            return _propertyRegistrationRepository.GenerateDemandLetters(rId);
        }

        public DataSourceResult GetRIDsForRentPermission(DataSourceRequest Req, int Rid)
        {
            return _propertyRegistrationRepository.GetRIDsForRentPermission(Req, Rid);
        }


        public RentPermissionModel GetAllotteDetailsForRentPermission(int rid)
        {
            return _propertyRegistrationRepository.GetAllotteDetailsForRentPermission(rid);
        }


        public bool SaveRentPermissionRequest(RentPermissionModel model)
        {
            return _propertyRegistrationRepository.SaveRentPermissionRequest(model);
        }


        public List<DDList> GetPropertyTypeForRentPermission()
        {
            return _propertyRegistrationRepository.GetPropertyTypeForRentPermission();
        }


        public List<DDList> GetRentDurationForRentPermission()
        {
            return _propertyRegistrationRepository.GetRentDurationForRentPermission();
        }


        public List<DDList> GetApproverForRentPermission(int rid)
        {
            return _propertyRegistrationRepository.GetApproverForRentPermission(rid);
        }


        public DataSourceResult GetRentRequestsList(DataSourceRequest request)
        {
            return _propertyRegistrationRepository.GetRentRequestsList(request);
        }


        public RentPermissionModel GetRentRequestByRequestNumber(int requestNo)
        {
            return _propertyRegistrationRepository.GetRentRequestByRequestNumber(requestNo);
        }


        public bool UpdateStatusOfRentRequest(int rid, int requestNo, string comment, string requestStatus, string viewName)
        {
            return _propertyRegistrationRepository.UpdateStatusOfRentRequest(rid, requestNo, comment, requestStatus, viewName);
        }


        public ChallanModel GetChallanDetailsForRentPermission(int rid, int requestNo)
        {
            return _propertyRegistrationRepository.GetChallanDetailsForRentPermission(rid, requestNo);
        }

        public RentPermissionModel GetModelToPrintRentPermissionLetter(int requestNo, int rid)
        {
            return _propertyRegistrationRepository.GetModelToPrintRentPermissionLetter(requestNo, rid);
        }

        public List<RentPermissionModel> GetModelListToPrintRentPermissionLetter(List<int> requestNoList, List<int> ridList)
        {
            return _propertyRegistrationRepository.GetModelListToPrintRentPermissionLetter(requestNoList, ridList);
        }

        public List<DDList> GetTransferTypes()
        {
            return _propertyRegistrationRepository.GetTransferTypes();
        }

        public List<DDList> GetTransferSubTypes(int TransferType)
        {
            return _propertyRegistrationRepository.GetTransferSubTypes(TransferType);
        }

        public bool SaveTransferData(int rId, int propId, string applicantName, string applicantGender, string applicantRelativeName, string applicantPropType, decimal Area, string floor, int userVal, int transType, int transSubType, DateTime transDate, decimal transChargePerSqMrt, decimal totTransCharge, string transfereeGender, string transfereeCompanyName, string transfereeFrstName, string trasfereeSignAuth, string transfereeMiddleName, string transfereeLstName, string transfereeCompanyRegOfc, string TransfereeRelName, string TransfereeMotherName, string TransfereeMobile, string TransfereeEmail, string TransfereeCorrAdd, string TransfereePerAdd, string TransfereePAN, int? TransfereeOccupation, int? reqNo, string compName, string signAuth, int? OnlineReqRefNo, string transferorCorrAdd, string transferorPerAdd, decimal? CurrentPropertyRate, decimal? LocationCharge, decimal? TotalPropertyCost, decimal? AnnualLeaseRent)
        {
            return _propertyRegistrationRepository.SaveTransferData(rId, propId, applicantName, applicantGender, applicantRelativeName, applicantPropType, Area, floor, userVal, transType, transSubType, transDate, transChargePerSqMrt, totTransCharge, transfereeGender, transfereeCompanyName, transfereeFrstName, trasfereeSignAuth, transfereeMiddleName, transfereeLstName, transfereeCompanyRegOfc, TransfereeRelName, TransfereeMotherName, TransfereeMobile, TransfereeEmail, TransfereeCorrAdd, TransfereePerAdd, TransfereePAN, TransfereeOccupation, reqNo, compName, signAuth, OnlineReqRefNo, transferorCorrAdd, transferorPerAdd, CurrentPropertyRate, LocationCharge, TotalPropertyCost, AnnualLeaseRent);
        }
        public ChallanModel GetChallanDetailsForFunctionalPermission(int rid, int requestNo)
        {
            return _propertyRegistrationRepository.GetChallanDetailsForFunctionalPermission(rid, requestNo);
        }

        public DataSourceResult GetTransferData_Approver(DataSourceRequest req)
        {
            return _propertyRegistrationRepository.GetTransferData_Approver(req);
        }

        public TransferModel GetTransfereeDetailByReqID(int Id)
        {
            //return _propertyRegistrationRepository.GetTransfereeDetailByReqID(Id);
            TransferModel details = _propertyRegistrationRepository.GetTransfereeDetailByReqID(Id);
            if (details != null)
            {
                details.BalanceDues = _paymentEngineService.GetBalanceDueTillDate(details.RId.Value, details.DepttId.Value);
            }
            return details;
        }

        public bool SaveTransferApprovalStatus(string comments, int intStatus, int ReqNo)
        {
            return _propertyRegistrationRepository.SaveTransferApprovalStatus(comments, intStatus, ReqNo);
        }

        // To cancel Transfer Request
        public bool CancelTransferRequest(int requestID)
        {
            return _propertyRegistrationRepository.CancelTransferRequest(requestID);
        }

        public DataSourceResult GetBuildingPlan(DataSourceRequest request)
        {
            return _propertyRegistrationRepository.GetBuildingPlan(request);
        }

        public BuildingPlanModel GetBuildingPlanDetailsByRId(int rId)
        {
            return _propertyRegistrationRepository.GetBuildingPlanDetailsByRId(rId);
        }
        public DataSourceResult GetBuildingDetail(DataSourceRequest request, int rId)
        {
            return _propertyRegistrationRepository.GetBuildingDetail(request, rId);
        }
        public DataSourceResult GetChargesDemanded(DataSourceRequest request, int rId)
        {
            return _propertyRegistrationRepository.GetChargesDemanded(request, rId);
        }
        public DataSourceResult GetNOCDetail(DataSourceRequest request, int rId)
        {
            return _propertyRegistrationRepository.GetNOCDetail(request, rId);
        }
        public BuildingPlanModel GetBuildingPlanByRId(int rId)
        {
            return _propertyRegistrationRepository.GetBuildingPlanByRId(rId);
        }
        public int SaveBuildingPlan1(int rId, string BuildingPlanFileNo, int? PropertyUse, DateTime? DateOfSubmission, DateTime? DateOfSanction, decimal? PurchasableFar, DateTime? SanctionPlanValidity, string MapReleased, string MapRevised, decimal? PloatArea, decimal? CoveredArea, double? FloorAreasRation, int NumberOfStories, string ArchitectRegNo, string NameOfArchitect)
        {
            return _propertyRegistrationRepository.SaveBuildingPlan1(rId, BuildingPlanFileNo, PropertyUse, DateOfSubmission, DateOfSanction, PurchasableFar, SanctionPlanValidity, MapReleased, MapRevised, PloatArea, CoveredArea, FloorAreasRation, NumberOfStories, ArchitectRegNo, NameOfArchitect);
        }
        public List<DDList> GetChargesType()
        {
            return _propertyRegistrationRepository.GetChargesType();
        }
        public List<DDList> GetNOCType()
        {
            return _propertyRegistrationRepository.GetNOCType();
        }
        public int SaveBuildingDetails(int rId, string NameOfTower, decimal? CoveredAreaMultiFloors, decimal? BuildingHeight, decimal? CoveredAreaGroundFloor, int? NumberOfStories2)
        {
            return _propertyRegistrationRepository.SaveBuildingDetails(rId, NameOfTower, CoveredAreaMultiFloors, BuildingHeight, CoveredAreaGroundFloor, NumberOfStories2);
        }
        public int SaveBuildingCharge(int rId, int Charges, decimal? FeeAmount)
        {
            return _propertyRegistrationRepository.SaveBuildingCharge(rId, Charges, FeeAmount);
        }
        public int SaveNocType(int rId, int NocType, string Submitted)
        {
            return _propertyRegistrationRepository.SaveNocType(rId, NocType, Submitted);
        }

        public bool RemoveBuildingDetails(int RequestNo, int rId)
        {
            return _propertyRegistrationRepository.RemoveBuildingDetails(RequestNo, rId);
        }

        public bool RemoveBPCharges(int RequestNo, int rId)
        {
            return _propertyRegistrationRepository.RemoveBPCharges(RequestNo, rId);
        }
        public bool RemoveNocType(int RequestNo, int rId)
        {
            return _propertyRegistrationRepository.RemoveNocType(RequestNo, rId);
        }
        public int SubmitBuildingPlan(int rId)
        {
            return _propertyRegistrationRepository.SubmitBuildingPlan(rId);
        }

        public int UpdateBuildingPlan1(int rId, string BuildingPlanFileNo, int? PropertyUse, DateTime? DateOfSubmission, DateTime? DateOfSanction, decimal? PurchasableFar, DateTime? SanctionPlanValidity, string MapReleased, string MapRevised, decimal? PloatArea, decimal? CoveredArea, double? FloorAreasRation, int NumberOfStories, string ArchitectRegNo, string NameOfArchitect)
        {
            return _propertyRegistrationRepository.UpdateBuildingPlan1(rId, BuildingPlanFileNo, PropertyUse, DateOfSubmission, DateOfSanction, PurchasableFar, SanctionPlanValidity, MapReleased, MapRevised, PloatArea, CoveredArea, FloorAreasRation, NumberOfStories, ArchitectRegNo, NameOfArchitect);
        }
        public List<DDList> GetPropertyUse()
        {
            return _propertyRegistrationRepository.GetPropertyUse();
        }


        public List<DynamicDataModel> GetAllottedPropertyRidList()
        {
            return _propertyRegistrationRepository.GetAllottedPropertyRidList();
        }

        public AllottedPropertyPaymentModel GetAllottedPropertyDetail(int rid)
        {
            return _propertyRegistrationRepository.GetAllottedPropertyDetail(rid);
        }

        public List<DynamicDataModel> GetPropertyPaymentType()
        {
            return _propertyRegistrationRepository.GetPropertyPaymentType();
        }

        public List<DynamicDataModel> GetPropertySubPaymentType(int receiptId)
        {
            return _propertyRegistrationRepository.GetPropertySubPaymentType(receiptId);
        }

        public bool AddAllottedPropertyPayment(AllottedPropertyPaymentModel model)
        {
            return _propertyRegistrationRepository.AddAllottedPropertyPayment(model);
        }


        public List<AllottedPropertyPaymentModel> GetPropertyPaymentDetails()
        {
            return _propertyRegistrationRepository.GetPropertyPaymentDetails();
        }


        public List<ChallanModel> GetModelListToPrintRentPermissionChallan(List<int> requestNoList, List<int> ridList)
        {
            return _propertyRegistrationRepository.GetModelListToPrintRentPermissionChallan(requestNoList, ridList);
        }

        public DataSourceResult GetAllGPAData(DataSourceRequest req)
        {
            return _propertyRegistrationRepository.GetAllGPAData(req);
        }

        public bool ActivateDeactivateToggle(int id, bool isActive)
        {
            return _propertyRegistrationRepository.ActivateDeactivateToggle(id, isActive);
        }
        public bool SaveGPANominee(GPAModel GPA)
        {
            return _propertyRegistrationRepository.SaveGPANominee(GPA);
        }
        public List<DDLStringList> GetGPAType(string company)
        {
            return _propertyRegistrationRepository.GetGPAType(company);
        }
        public DataSourceResult GetNomineeData(int rid, DataSourceRequest req)
        {
            return _propertyRegistrationRepository.GetNomineeData(rid, req);
        }
        public bool AddNominee(int rid, string nomName, string relation, DateTime nomDate)
        {
            return _propertyRegistrationRepository.AddNominee(rid, nomName, relation, nomDate);
        }
        public bool RemoveNominee(int id)
        {
            return _propertyRegistrationRepository.RemoveNominee(id);
        }
        public GPAModel GetGPADetailsByRId(int rId)
        {
            return _propertyRegistrationRepository.GetGPADetailsByRId(rId);
        }
        public string GenerateFunctionalCertificater(int rid)
        {
            return _propertyRegistrationRepository.GenerateFunctionalCertificater(rid);
        }
        public List<ChallanModel> GetModelListToPrintFunctionalChallan(List<int> requestNoList, List<int> ridList)
        {
            return _propertyRegistrationRepository.GetModelListToPrintFunctionalChallan(requestNoList, ridList);
        }
        public List<InterviewDetailsModel> GetSchemeList()
        {
            return _propertyRegistrationRepository.GetSchemeList();
        }
        public List<InterviewDetailsModel> FilterDepartmentOnScheme(int schemeId)
        {
            return _propertyRegistrationRepository.FilterDepartmentOnScheme(schemeId);
        }
        //public  InterviewDetailsModel GetApplicantDetails(string formno, int schemeID, int departmentID)
        // {
        //     return _propertyRegistrationRepository.GetApplicantDetails(formno, schemeID, departmentID);
        // }
        public InterviewDetailsModel GetApplicantDetailsForInterview(string formno, int schemeID, int departmentID)
        {
            return _propertyRegistrationRepository.GetApplicantDetailsForInterview(formno, schemeID, departmentID);
        }
        public int SaveInterviewForm(int ApplicationId, string InterviewDetails, DateTime InterviewDate, int SchemeId, int DepartmentId, string FormNo)
        {
            return _propertyRegistrationRepository.SaveInterviewForm(ApplicationId, InterviewDetails, InterviewDate, SchemeId, DepartmentId, FormNo);
        }
        public InterviewDetailsModel GetInterviewDetailsById(int id)
        {
            return _propertyRegistrationRepository.GetInterviewDetailsById(id);
        }
        public DataSourceResult GetInterviewDetails(DataSourceRequest sourceReq)
        {
            return _propertyRegistrationRepository.GetInterviewDetails(sourceReq);
        }
        public int UpdateInterviewDetails(int ApplicationId, string InterviewDetails, DateTime InterviewDate, int Id, int SchemeId)
        {
            return _propertyRegistrationRepository.UpdateInterviewDetails(ApplicationId, InterviewDetails, InterviewDate, Id, SchemeId);
        }
        public bool RemoveInterviewDetails(int rId)
        {
            return _propertyRegistrationRepository.RemoveInterviewDetails(rId);
        }
        public List<DDList> GetRIDsForNoting()
        {
            return _propertyRegistrationRepository.GetRIDsForNoting();
        }
        public List<DDList> GetDeptmentForNoting()
        {
            return _propertyRegistrationRepository.GetDeptmentForNoting();
        }
        public int AddNotingFile(int Rid, string FileName, string DepartmentName, int? deptId)
        {
            return _propertyRegistrationRepository.AddNotingFile(Rid, FileName, DepartmentName, deptId);
        }
        public NotingDetailsModel GetNotingDeptmentByRId(int rid)
        {
            return _propertyRegistrationRepository.GetNotingDeptmentByRId(rid);
        }
        public DataSourceResult GetNotingDetails(DataSourceRequest sourceReq, int? Rid, int? DepartmentId, string FileName)
        {
            return _propertyRegistrationRepository.GetNotingDetails(sourceReq, Rid, DepartmentId, FileName);
        }
        public NotingDetailsModel ViewNotingDetails(int reqNo)
        {
            return _propertyRegistrationRepository.ViewNotingDetails(reqNo);
        }

        public DataSourceResult GetDemandLetterDataByFilter(DataSourceRequest req, int? deptt, int? sector, int? block, int type)
        {
            return _propertyRegistrationRepository.GetDemandLetterDataByFilter(req, deptt, sector, block, type);
        }

        public List<DrawSlipModel> GetApplicationFormDetailsForDrawSlip(int schemeId, int departmentId, List<string> nameList)
        {
            return _propertyRegistrationRepository.GetApplicationFormDetailsForDrawSlip(schemeId, departmentId, nameList);
        }
        public string AddNotingsDetails(int Rid, string AddNotingDetails)
        {
            return _propertyRegistrationRepository.AddNotingsDetails(Rid, AddNotingDetails);
        }

        public int SubmitForMoveNoting(int Rid, string user)
        {
            return _propertyRegistrationRepository.SubmitForMoveNoting(Rid, user);
        }


        public List<DDList> GetDemandLetterTypes()
        {
            return _propertyRegistrationRepository.GetDemandLetterTypes();
        }

        public List<DDList> GetAllAccountHead()
        {
            return _propertyRegistrationRepository.GetAllAccountHead();
        }
        public List<DDList> GetAccountSubHead(int AccountHeadId)
        {
            return _propertyRegistrationRepository.GetAccountSubHead(AccountHeadId);
        }

        public int SaveCreateChallan(int rId, int AccountHeadId, int AccountSubHeadId, decimal? Amount)
        {
            return _propertyRegistrationRepository.SaveCreateChallan(rId, AccountHeadId, AccountSubHeadId, Amount);
        }
        public bool RemoveChallanChargeDetail(int ChallanTransID, int rId)
        {
            return _propertyRegistrationRepository.RemoveChallanChargeDetail(ChallanTransID, rId);
        }
        public DataSourceResult GetAccountChargeDetails(DataSourceRequest request, int rId)
        {
            return _propertyRegistrationRepository.GetAccountChargeDetails(request, rId);
        }
        public string SaveGenerateChallan(int rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId)
        {
            return _propertyRegistrationRepository.SaveGenerateChallan(rId, bankId, branchId, DdlAccountNumber, DepttId);
        }
        public DataSourceResult GetManageAccountDetails(DataSourceRequest request)
        {
            return _propertyRegistrationRepository.GetManageAccountDetails(request);
        }
        public string GetAccountNumber(int bankId, int branchId)
        {
            return _propertyRegistrationRepository.GetAccountNumber(bankId, branchId);
        }
        public List<DDList> GetRIDsForManageAccountDetails()
        {
            return _propertyRegistrationRepository.GetRIDsForManageAccountDetails();
        }
        public DataSourceResult GetRIDsForManageAccountDetailsByDataSource(DataSourceRequest req)
        {
            return _propertyRegistrationRepository.GetRIDsForManageAccountDetailsByDataSource(req);
        }
        public bool DeActivate(string ChallanNo, bool status, string viewName)
        {
            return _propertyRegistrationRepository.DeActivate(ChallanNo, status, viewName);
        }
        public string ChallanGenerate(string ChallanId, int rid)
        {
            return _propertyRegistrationRepository.ChallanGenerate(ChallanId, rid);
        }

        public List<DynamicDataModel> GetBankNamesForPayment()
        {
            return _propertyRegistrationRepository.GetBankNamesForPayment();
        }

        public DataSourceResult GetRIDsForLeaseDeed(DataSourceRequest Request)
        {
            return _propertyRegistrationRepository.GetRIDsForLeaseDeed(Request);
        }

        public List<DDList> GetRIDsForSubleaseDeed()
        {
            return _propertyRegistrationRepository.GetRIDsForSubleaseDeed();
        }

        public List<ChallanModel> GetModelListToPrintExtensionChallan(List<int> requestNoList, List<int> ridList)
        {
            return _propertyRegistrationRepository.GetModelListToPrintExtensionChallan(requestNoList, ridList);
        }


        public string GenerateRentPermissionLetter(int requestNo, int rid)
        {
            return _propertyRegistrationRepository.GenerateRentPermissionLetter(requestNo, rid);
        }

        public List<ChallanModel> GetModelListToPrintCICChallan(int ridList, int directorID)
        {
            return _propertyRegistrationRepository.GetModelListToPrintCICChallan(ridList, directorID);
        }

        public List<ChallanModel> GetModelListToPrintCICFirmnProductChallan(int ridList, int directorID)
        {
            return _propertyRegistrationRepository.GetModelListToPrintCICFirmnProductChallan(ridList, directorID);
        }


        public FunctionalModel GetFunctionalDetailByRid(int rid)
        {
            return _propertyRegistrationRepository.GetFunctionalDetailByRid(rid);
        }


        public List<DDList> GetRidToSearchNoting()
        {
            return _propertyRegistrationRepository.GetRidToSearchNoting();
        }


        public ChallanModel GenerateChallan(int rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId)
        {
            return _propertyRegistrationRepository.GenerateChallan(rId, bankId, branchId, DdlAccountNumber, DepttId);
        }


        public bool SaveGeneratedChallan(int rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId, string parsedHTML)
        {
            return _propertyRegistrationRepository.SaveGeneratedChallan(rId, bankId, branchId, DdlAccountNumber, DepttId, parsedHTML);
        }
        public bool SaveGeneratedChallan(string challanId, string challan)
        {
            return _propertyRegistrationRepository.SaveGeneratedChallan(challanId, challan);
        }

        public List<BankAccountManagementModel> GetGeneratedChallanDetails(int rid)
        {
            return _propertyRegistrationRepository.GetGeneratedChallanDetails(rid);
        }


        public ChallanModel GeneratePaymentChallan(int rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId)
        {
            return _propertyRegistrationRepository.GeneratePaymentChallan(rId, bankId, branchId, DdlAccountNumber, DepttId);
        }


        public bool RemoveChallanChargeDetail(int rid, string headName, string subHeadName, decimal amount)
        {
            return _propertyRegistrationRepository.RemoveChallanChargeDetail(rid, headName, subHeadName, amount);
        }

        public string SaveGeneratedChallanByRid(int rid, int bankId, int branchId, string accountNumber, string challan)
        {
            return _propertyRegistrationRepository.SaveGeneratedChallanByRid(rid, bankId, branchId, accountNumber, challan);
        }


        public int UpdateNotingForAllottedProperty(int Rid, string user, string notingDetails)
        {
            return _propertyRegistrationRepository.UpdateNotingForAllottedProperty(Rid, user, notingDetails);
        }


        public bool SaveGeneratedChallanForServiceRequest(string challanId, string challan)
        {
            return _propertyRegistrationRepository.SaveGeneratedChallanForServiceRequest(challanId, challan);
        }


        public DataSourceResult GetRIDsForFunctional_Read(DataSourceRequest Req, int? rid)
        {
            return _propertyRegistrationRepository.GetRIDsForFunctional_Read(Req, rid);
        }

        public DataSourceResult GetAllottedPropertyRidList(DataSourceRequest Req)
        {
            return _propertyRegistrationRepository.GetAllottedPropertyRidList(Req);
        }

        public string GetMortgagePreviousLoanDetails(int? requestNo, int? Rid)
        {
            return _propertyRegistrationRepository.GetMortgagePreviousLoanDetails(requestNo, Rid);
        }


        public DataSourceResult GetRentPermissionByRid(DataSourceRequest request, int Rid)
        {
            return _propertyRegistrationRepository.GetRentPermissionByRid(request, Rid);
        }


        public int RemoveRentRequestDetailsById(RentPermissionModel model)
        {
            return _propertyRegistrationRepository.RemoveRentRequestDetailsById(model);
        }
    }
}
