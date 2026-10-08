using Kendo.Mvc.UI;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Model.CommonModel;

namespace NA.PMS.Service.Property
{
    public interface IPropertyRegistrationService
    {
        DataSourceResult GetPossessionData(DataSourceRequest req);

        // For getting CheckList Type
        IEnumerable<CommonListModel> GetCheckListType();

        // Get all RIDs 
        IEnumerable<CommonListModel> GetAllRIDsForLeaseDeed();

        // Getting Property details by RID
        CheckList GetCheckListDetailsByRId(int rId);

        // Get All Lease Deed Properties
        List<LeaseDeedProperty> GetAllLeaseDeadProperties();

        // Get lease deed property details by RID
        LeaseDeedProperty GetLeaseDeadPropertyDetailsByRId(int rId);

        // Add/Update lease deed property details by RID ---- Add Lease deed Execution date
        LeaseDeedProperty AddLeaseDeadPropertyDetailsByRId(LeaseDeedProperty leaseDeadProperty);
        DataSourceResult GetLeaseDeedData(DataSourceRequest req);
        DataSourceResult GetPropertyRentList(DataSourceRequest request);
        DataSourceResult GetPropertyRentListByRid(DataSourceRequest request, int Rid);
        DetailsByRId GetDetailsByRID(int rId);
        LeaseDeedProperty GetStampDetailsByLeaseExcutionDate(int rId, DateTime LDExecDate);
        List<ChecklistDocuments> GetChcklstDocuments(DataSourceRequest request, int propTypeId, int chkLstType);
        bool SaveChecklistDetails(int rId, int chkType, DateTime chkDate, string viewName);

        //added new parameters according to requirement -- (, decimal? LeaseRent, int RevisedAfterYear, decimal? RevisedRate)
        bool SaveLeaseDeedDetails(int rId, DateTime LDExecDate, string SA, string w1N, string w1A, string w1M, string w2N, string w2A, string w2M, string w3N, string w3A, string w3M, string w4N, string w4A, string w4M, DateTime LDDueDate, string regType, decimal SDAmt, decimal DSDAmt, decimal prevDues, decimal balDueTD, string viewName, int type, decimal docCharges);
        LeaseDeedProperty GetLeaseDeedDetailsByRId(int id);
        DataSourceResult GetMutationData(DataSourceRequest req);
        DataSourceResult GetMutationDataByApproverId(DataSourceRequest req);
        bool SaveMutationDetails(DateTime mutDate, DateTime transDeedDate, string bahiNo, string bahiZildNo, string bahiPageNo, string SINo, int userVal, int ReqNo, int rId, DateTime transDate, string transType, int? ReqRefNo);
        bool UpdateMutationDetails(DateTime mutDate, DateTime transDeedDate, string bahiNo, string bahiZildNo, string bahiPageNo, string SINo, int userVal, int ReqNo, int rId);
        DataSourceResult GetRIDsForMutation(DataSourceRequest Req);
        DataSourceResult GetRIDsForTransfer(DataSourceRequest Req);
        DataSourceResult GetRIDsForTransfer(DataSourceRequest Req, int Rid);
        DetailsByRId GetTransferDetailsByRID(int rId);
        MutationModel GetMutationDetailsByRId(int reqNo);
        bool CancelMutationRequest(int reqNo);
        bool IsChecklistGenerated(int rId);
        bool SaveApprovalStatus(string comments, int intStatus, int ReqNo);
        DateTime? GetChklstDateByChklstType(int rId, int type);
        DataSourceResult GetTransferData(DataSourceRequest req);
        DataSourceResult GetTransferData(DataSourceRequest req, int? Rid);
        DataSourceResult GetTransferHistory(DataSourceRequest req);
        TransferModel GetOriginalDetailsForTransferByRID(int rId);
        TransferModel GetOriginalDetailsForTransferOnlyByRID(int rId, int reqNo);
        DataSourceResult GetTransferData_Approver(DataSourceRequest req);
        DataSourceResult GetPropFunctional(DataSourceRequest sourceReq);
        FunctionalModel GetFunctionalDetailsByReqNo(int reqNo);
        DataSourceResult GetRIDsForFunctional(DataSourceRequest Req, int Rid);
        int SaveFunctionalDetails(int rId, DateTime? FunctionalDate, DateTime? FunctionalDueDate, bool MeterSealing, bool Affidavit, bool RegistrationCertificate, bool NDCAccount, string user, string PropertyNumber, decimal? FunctionalCharge, int ReqRefNo, DateTime? CompletionDate, DateTime? AffidavitDate);
        bool CancelFunctionRequest(int reqNo);
        int ReSubmitFunctionalDetails(int rId, DateTime? FunctionalDate, DateTime? FunctionalDueDate, bool MeterSealing, bool Affidavit, bool RegistrationCertificate, bool NDCAccount, string user, string PropertyNumber, decimal? FunctionalCharge);
        DataSourceResult GetFunctionalDataByApproverId(DataSourceRequest req);
        bool SaveFunctionalApprovalStatus(string comments, int intStatus, int ReqNo);
        FunctionalModel GetFUnctionalTransferDetailsByRID(int rId);

        // To Get Details of All Mortgage.
        DataSourceResult GetAllMortgage(DataSourceRequest req);
        DataSourceResult GetAllMortgageByRid(DataSourceRequest req, int Rid);
        //To Fill Mortgage Type Drop Down.
        List<DDList> GetMortgageType();
        //To Fill Mortgage Prev Loan Drop Down.
        List<DDList> GetMortgagePrevLoan();
        //To Get Property Details based on registation id or RID.
        MortgageModel GetPropertyDetailByRid(int rID);
        bool AddMortgage(DateTime mortgageDate, string bankName, string mortgageType, short previousLoanNoc, string branchAddress, decimal processingFee, decimal sanctionedAmount, string user, int rid, DateTime validUpto, int? reqRefNo);
        // To Get Details of All Mortgage by Approver ID.
        DataSourceResult GetAllMortgageByApprover(DataSourceRequest req);
        // Get Mortgage by request ID.
        MortgageModel GetMortgageByRequestID(int requestId);
        // To Save comment of approver.
        bool SaveCommentByRequestID(int requestNo, string Comment, bool acceptReject);
        //To Update Mortgage
        bool UpdateMortgage(int requestID, DateTime mortgageDate, string bankName, string mortgageType, short previousLoanNoc, string branchAddress, decimal processingFee, decimal sanctionedAmount, string user, int rid);
        // To cancel Mortgage Request
        bool CancelMortgageRequest(int requestID);
        // To Invalida Mortgate
        bool InValidMortgageRequestID(int requestNo, string Comment);
        //To Generate Mortgage letter
        string GenerateMortgageLetter(int rid);
        //To Generate Completion letter
        string GenerateCompletionLetter(int rid);
        string GenerateDemandLetters(int rId);
        DataSourceResult GetDemandLetterDataByFilter(DataSourceRequest req, int? deptt, int? sector, int? block, int type);

        DataSourceResult GetRIDsForRentPermission(DataSourceRequest Req, int Rid);

        RentPermissionModel GetAllotteDetailsForRentPermission(int rid);

        bool SaveRentPermissionRequest(RentPermissionModel model);

        List<DDList> GetPropertyTypeForRentPermission();

        List<DDList> GetRentDurationForRentPermission();

        List<DDList> GetApproverForRentPermission(int rid);

        DataSourceResult GetRentRequestsList(DataSourceRequest request);

        RentPermissionModel GetRentRequestByRequestNumber(int requestNo);

        bool UpdateStatusOfRentRequest(int rid, int requestNo, string comment, string requestStatus, string viewName);

        ChallanModel GetChallanDetailsForRentPermission(int rid, int requestNo);

        RentPermissionModel GetModelToPrintRentPermissionLetter(int requestNo, int rid);

        List<RentPermissionModel> GetModelListToPrintRentPermissionLetter(List<int> requestNoList, List<int> ridList);
        List<DDList> GetTransferTypes();
        List<DDList> GetTransferSubTypes(int TransferType);

        ChallanModel GetChallanDetailsForFunctionalPermission(int rid, int requestNo);
        string GenerateFunctionalCertificater(int rid);

        bool SaveTransferData(int rId, int propId, string applicantName, string applicantGender, string applicantRelativeName, string applicantPropType, decimal Area, string floor, int userVal, int transType, int transSubType, DateTime transDate, decimal transChargePerSqMrt, decimal totTransCharge, string transfereeGender, string transfereeCompanyName, string transfereeFrstName, string trasfereeSignAuth, string transfereeMiddleName, string transfereeLstName, string transfereeCompanyRegOfc, string TransfereeRelName, string TransfereeMotherName, string TransfereeMobile, string TransfereeEmail, string TransfereeCorrAdd, string TransfereePerAdd, string TransfereePAN, int? TransfereeOccupation, int? reqNo, string compName, string signAuth, int? OnlineReqRefNo, string transferorCorrAdd, string transferorPerAdd, decimal? CurrentPropertyRate, decimal? LocationCharge, decimal? TotalPropertyCost, decimal? AnnualLeaseRent);

        TransferModel GetTransfereeDetailByReqID(int Id);
        // To cancel Transfer Request
        bool CancelTransferRequest(int requestID);
        bool SaveTransferApprovalStatus(string comments, int intStatus, int ReqNo);
        List<DDList> GetDemandLetterTypes();
        DataSourceResult GetBuildingPlan(DataSourceRequest request);
        BuildingPlanModel GetBuildingPlanDetailsByRId(int rId);
        DataSourceResult GetBuildingDetail(DataSourceRequest request, int rId);
        DataSourceResult GetChargesDemanded(DataSourceRequest request, int rId);
        DataSourceResult GetNOCDetail(DataSourceRequest request, int rId);

        BuildingPlanModel GetBuildingPlanByRId(int rid);

        int SaveBuildingPlan1(int rId, string BuildingPlanFileNo, int? PropertyUse, DateTime? DateOfSubmission, DateTime? DateOfSanction, decimal? PurchasableFar, DateTime? SanctionPlanValidity, string MapReleased, string MapRevised, decimal? PloatArea, decimal? CoveredArea, double? FloorAreasRation, int NumberOfStories, string ArchitectRegNo, string NameOfArchitect);

        List<DDList> GetChargesType();
        List<DDList> GetNOCType();
        int SaveBuildingDetails(int rId, string NameOfTower, decimal? CoveredAreaMultiFloors, decimal? BuildingHeight, decimal? CoveredAreaGroundFloor, int? NumberOfStories2);
        int SaveBuildingCharge(int rId, int Charges, decimal? FeeAmount);
        int SaveNocType(int rId, int NocType, string Submitted);

        bool RemoveBuildingDetails(int RequestNo, int rId);
        bool RemoveBPCharges(int RequestNo, int rId);
        bool RemoveNocType(int RequestNo, int rId);

        int SubmitBuildingPlan(int rId);
        List<DDList> GetPropertyUse();

        //To update buliding plan
        int UpdateBuildingPlan1(int rId, string BuildingPlanFileNo, int? PropertyUse, DateTime? DateOfSubmission, DateTime? DateOfSanction, decimal? PurchasableFar, DateTime? SanctionPlanValidity, string MapReleased, string MapRevised, decimal? PloatArea, decimal? CoveredArea, double? FloorAreasRation, int NumberOfStories, string ArchitectRegNo, string NameOfArchitect);

        List<DynamicDataModel> GetAllottedPropertyRidList();

        AllottedPropertyPaymentModel GetAllottedPropertyDetail(int rid);

        List<DynamicDataModel> GetPropertyPaymentType();

        List<DynamicDataModel> GetPropertySubPaymentType(int receiptId);

        bool AddAllottedPropertyPayment(AllottedPropertyPaymentModel model);

        List<AllottedPropertyPaymentModel> GetPropertyPaymentDetails();

        List<ChallanModel> GetModelListToPrintRentPermissionChallan(List<int> requestNoList, List<int> ridList);

        List<ChallanModel> GetModelListToPrintFunctionalChallan(List<int> requestNoList, List<int> ridList);

        List<InterviewDetailsModel> GetSchemeList();
        List<InterviewDetailsModel> FilterDepartmentOnScheme(int schemeId);
        InterviewDetailsModel GetApplicantDetailsForInterview(string formno, int schemeID, int departmentID);
        int SaveInterviewForm(int ApplicationId, string InterviewDetails, DateTime InterviewDate, int SchemeId, int DepartmentId, string FormNo);

        int UpdateInterviewDetails(int ApplicationId, string InterviewDetails, DateTime InterviewDate, int Id, int SchemeId);
        bool RemoveInterviewDetails(int rId);
        DataSourceResult GetInterviewDetails(DataSourceRequest sourceReq);

        DataSourceResult GetAllGPAData(DataSourceRequest req);
        bool ActivateDeactivateToggle(int id, bool isActive);
        bool SaveGPANominee(GPAModel GPA);
        List<DDLStringList> GetGPAType(string company);
        DataSourceResult GetNomineeData(int rid, DataSourceRequest req);
        bool AddNominee(int rid, string nomName, string relation, DateTime nomDate);
        bool RemoveNominee(int id);
        GPAModel GetGPADetailsByRId(int rId);
        InterviewDetailsModel GetInterviewDetailsById(int id);

        List<DDList> GetRIDsForNoting();
        List<DDList> GetDeptmentForNoting();

        int AddNotingFile(int Rid, string FileName, string DepartmentName, int? deptId);
        NotingDetailsModel GetNotingDeptmentByRId(int rid);
        DataSourceResult GetNotingDetails(DataSourceRequest sourceReq, int? Rid, int? DepartmentId, string FileName);
        NotingDetailsModel ViewNotingDetails(int reqNo);
        string AddNotingsDetails(int Rid, string AddNotingDetails);

        int SubmitForMoveNoting(int Rid, string user);


        List<DrawSlipModel> GetApplicationFormDetailsForDrawSlip(int schemeId, int departmentId, List<string> nameList);


        List<DDList> GetAllAccountHead();
        List<DDList> GetAccountSubHead(int AccountHeadId);
        int SaveCreateChallan(int rId, int AccountHeadId, int AccountSubHeadId, decimal? Amount);
        bool RemoveChallanChargeDetail(int ChallanTransID, int rId);
        bool RemoveChallanChargeDetail(int rid, string headName, string subHeadName, decimal amount);
        DataSourceResult GetAccountChargeDetails(DataSourceRequest request, int rId);

        string SaveGenerateChallan(int rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId);
        DataSourceResult GetManageAccountDetails(DataSourceRequest request);
        string GetAccountNumber(int bankId, int branchId);
        List<DDList> GetRIDsForManageAccountDetails();
        DataSourceResult GetRIDsForManageAccountDetailsByDataSource(DataSourceRequest Req);
        bool DeActivate(string ChallanNo, bool status, string viewName);
        string ChallanGenerate(string ChallanId, int rid);

        //get banks name for payment
        List<DynamicDataModel> GetBankNamesForPayment();
        DataSourceResult GetRIDsForLeaseDeed(DataSourceRequest Request);
        List<DDList> GetRIDsForSubleaseDeed();

        List<ChallanModel> GetModelListToPrintExtensionChallan(List<int> requestNoList, List<int> ridList);

        string GenerateRentPermissionLetter(int requestNo, int rid);

        List<ChallanModel> GetModelListToPrintCICChallan(int ridList, int directorID);

        List<ChallanModel> GetModelListToPrintCICFirmnProductChallan(int ridList, int directorID);

        FunctionalModel GetFunctionalDetailByRid(int rid);

        List<DDList> GetRidToSearchNoting();

        ChallanModel GenerateChallan(int rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId);

        bool SaveGeneratedChallan(int rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId, string parsedHTML);
        bool SaveGeneratedChallan(string challanId, string challan);

        List<BankAccountManagementModel> GetGeneratedChallanDetails(int rid);

        ChallanModel GeneratePaymentChallan(int rId, int bankId, int branchId, string DdlAccountNumber, int? DepttId);

        string SaveGeneratedChallanByRid(int rid, int bankId, int branchId, string accountNumber, string challan);


        int UpdateNotingForAllottedProperty(int Rid, string user, string notingDetails);

        bool SaveGeneratedChallanForServiceRequest(string challanId, string challan);

        DataSourceResult GetRIDsForFunctional_Read(DataSourceRequest Req, int? rid);
        DataSourceResult GetAllottedPropertyRidList(DataSourceRequest Req);
        string GetMortgagePreviousLoanDetails(int? requestNo, int? Rid);

        DataSourceResult GetRentPermissionByRid(DataSourceRequest request, int Rid);

        int RemoveRentRequestDetailsById(RentPermissionModel model);
    }
}
