using System.ComponentModel;
namespace NA.PMS.Common
{
    public enum Role
    {
        Admin = 1,
        Member
    }

    public enum RoleType
    {
        [Description("SA")]
        SuperAdmin = 1,
        [Description("A")]
        Admin = 2,
        [Description("U")]
        User = 3
    }

    public enum Frequency
    {
        [Description("Monthly")]
        Monthly = 12,
        [Description("Quarterly")]
        Quarterly = 4,
        [Description("Half Yearly")]
        HalfYearly = 2,
        [Description("Yearly")]
        Yearly = 1
    }

    public enum QuotUnit
    {
        [Description("FIX")]
        FIX = 1,
        [Description("PER")]
        PERCENTAGE = 2,
    }

    public enum RebateUnit
    {
        [Description("PER")]
        PERCENTAGE = 1,
        [Description("INR")]
        INR = 2,
    }

    public enum Unit
    {
        [Description("Indian Rupees")]
        INR,
        [Description("Percent")]
        P
    }

    public enum DeductionApplyOn
    {
        [Description("Land Rate")]
        LandRate,
        [Description("Civil Cost")]
        CivilCost,
        [Description("Total Cost")]
        TotalCost
    }
    public enum Gender
    {
        [Description("Male")]
        Male = 1,
        [Description("Female")]
        Female = 2,
        [Description("Company")]
        Company = 3
    }

    public enum MaritialStatus
    {
        [Description("Married")]
        married,
        [Description("UnMarried")]
        unmarried
    }
    public enum PropertyBank
    {
        [Description("0")]
        Attached,
        [Description("1")]
        Detached
    }
    public enum Registry
    {
        [Description("Freehold")]
        Freehold,
        [Description("Leasehold")]
        Leasehold
    }

    public enum SelectionType
    {
        [Description("Draw")]
        DrawBased = 1,
        [Description("Open Ended")]
        OpenEnded = 2,
        [Description("Tender")]
        Tender = 3,
        [Description("E-Auction")]
        EAuction = 4
    }

    //Resused and added new statuses in this same Enum
    public enum AllotmentStatus
    {
        NotSubmitted,
        InProgress,
        Approved,
        RefundInitiated,
        Rejected,
        Pending,
        Cancelled
    }

    public enum Departmentenum
    {
        Institutional = 1,
        Commercial = 2,
        Residential = 3,
        Industrial = 4,
        Housing = 5,
        GroupHousing = 6
    }

    public enum CheckListType
    {
        LeaseDeed = 1,
        TransferDeed = 2,
        Functional = 3
    }

    public enum TransferType
    {
        T,
        M
    }

    public enum ProcessType
    {
        Allotment = 1,
        AllotmentMoney = 2,
        Checklist = 3,
        LeaseDeed = 4,
        PossessionOrder = 5,
        Possession = 6,
        BuildingPlan = 7,
        PartCompletion = 8,
        Completion = 9,
        Functional = 10
    }
    public enum ScreenMenuKey
    {
        ManageScheme = 6,
        ManageProperty = 7,
        ManageNotifications = 8,
        ManageRefund = 9,
        ManageCircleRate = 19,
        ManageApplicants = 11,
        ManageAllotment = 12,
        DraftAllotteeList = 14,
        Refunds = 15,
        ManageRequests = 16,
        AllotteeList = 17,
        UnsuccessfullApplicantList = 18
    }

    public enum CompletionType
    {
        [Description("Partial")]
        Partial = 1,
        [Description("Full")]
        Full = 2,
    }

    public enum RIdType
    {
        completion,
        buildingPlan
    }

    public enum BoolStatus
    {
        Yes,
        No
    }

    public enum MortgageType
    {
        [Description("Collateral")]
        Collateral = 1,
        [Description("Normal")]
        Normal = 2
    }

    public enum MortgagePrevLoan
    {
        [Description("Yes")]
        Yes = 1,
        [Description("No")]
        No = 2
    }

    public enum GPAType
    {
        GPA,
        Nominee
    }

    public enum PossessionType
    {
        Order,
        Entry
    }

    public enum FunctionalStatus
    {
        Functional,
        NonFunctional
    }
    public enum PossessionStatus
    {
        Ordered,
        Released,
        Due
    }
    public enum AreaChange
    {
        Change,
        NoChange,
        Increase,
        Decrease
    }
    public enum BuildingCompletion
    {
        Completed,
        PartialCompleted,
        NotCompleted
    }
    public enum LetterTemplate
    {
        AllotmentLetter = 1,
        TransferLetter = 2,
        TransferLetterWithPowerOfAttorney = 3,
        MutationLetter = 4,
        MortgageLetter = 5,
        Functionalletter = 6,
        CIC = 7,
        CICSHolderChange = 8,
        CICDirectorNameChange = 9,
        CICProductNameChange = 10,
        ExtinctionLetter = 11,
        PossessionOrder = 12,
        PossessionLetter = 18,
        PaymentSchedule = 13,
        DemandLetter = 14,
        Challan = 15,
        CompletionLetter = 16,
        Amalgamation = 17,
        Deamalgamation = 18,
        RentPermissionLetter = 14,
        ExtensionLetter = 17,
        ApplicationFormat = 19
    }

    public enum DDLSectorBlock
    {
        [Description("Sector")]
        Sector = 1,
        [Description("Block")]
        Block = 2
    }

    public enum EnumStatusType
    {
        No = 0,
        Yes = 1
    }
}