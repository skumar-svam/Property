using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class KYAViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> ApplicantTypeId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> ActionId { get; set; }
        public Nullable<int> ValidatorId { get; set; }
        public Nullable<int> ApproverId { get; set; }
        public Nullable<int> OptionalId { get; set; }

        public Nullable<int> TotalKYA { get; set; }
        public Nullable<int> ApprovedKYA { get; set; }
        public Nullable<int> PendingKYA { get; set; }
        public Nullable<int> RejectedKYA { get; set; }
        public Nullable<int> ForwardKYA { get; set; }
        public Nullable<int> AverageKYA { get; set; }
        public Nullable<int> CancelAfterTransferKYA { get; set; }
        public Nullable<int> Count { get; set; }
        public Nullable<int> DateMar { get; set; }

        public Nullable<decimal> AverageKYAForm { get; set; }

        public string KYAuid { get; set; }
        public string Department { get; set; }
        public string FormNo { get; set; }
        public string SchemeName { get; set; }
        public string AllotteeType { get; set; }
        public string AllotteeName { get;set; }
        public string Applicant { get; set; }
        public string ApplicantMaster { get; set; }
        public string ApplicantType { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }
        public string CountryCode { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string CorrespondAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string CommunicationAddress { get; set; }
        public string HouseNo { get; set; }
        public string StreetName { get; set; }
        public string AreaLocality { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string GSTNo { get; set; }
        public string PAN { get; set; }
        public string AadharNo { get; set; }
        public string ROC { get; set; }
        public string Status { get; set; }
        public string Validator { get; set; }
        public string Approver { get; set; }
        public string Remarks { get; set; }
        public string OptionalAction { get; set; }
        public string ActionType { get; set; }
        public string FilterType { get; set; }
        public string KYAReferenceCode { get; set; }

        public string OrganisationName { get; set; }
        public string AuthorizedSignatory { get; set; }
        public string OrganisationMobileNo { get; set; }
        public string SignatoryMobileNo { get; set; }
        public string SignatoryEmail { get; set; }
        public string OrganisationEmail { get; set; }

        public string AllotteeIdFileType { get; set; }
        public string AllotteeIdFilePath { get; set; }
        public string PlotOwnershipFileType { get; set; }
        public string PlotOwnershipFilePath { get; set; }
        public string OtherFileType { get; set; }
        public string OtherFilePath { get; set; }
        public string DocumentType { get; set; }
        public string DocumentPath { get; set; }

        public Nullable<DateTime> SubmitDate { get; set; }
        public Nullable<DateTime> ValidationDate { get; set; }
        public Nullable<DateTime> ApprovalDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<DateTime> StatusDate { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsInitiated { get; set; }
        public Nullable<bool> IsRejected { get; set; }
        public Nullable<bool> IsPending { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<bool> IsForwarded { get; set; }
        public Nullable<bool> IsDeclarationChecked { get; set; }
    }
}
