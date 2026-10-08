using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class ReportModel : PropertyModel
    {
        public string PropertyBank { get; set; }
    }

    public class PropertyInfoModel
    {
        public int Id { get; set; }
        public Nullable<int> Rid { get; set; }
        public Nullable<int> KYAStatusId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> RequestId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> LetterId { get; set; }
        public Nullable<int> RequestRefId { get; set; }

        public string DepartmentName { get; set; }
        public string RequestStatus { get; set; }
        public string ServiceName { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo { get { return Sector + "/" + Block + "-" + PlotNo; } }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ApplicantName { get; set; }
        public string CorresspondentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string IndividualOrCompany { get; set; }
        public string FatherOrHusbandName { get; set; }
        public string AutorizedSignatory { get; set; }
        public string KYAStatus { get; set; }
        
        public Nullable<DateTime> LetterDate { get; set; }

        public Nullable<bool> IsKYAUpdated { get; set; }
        public Nullable<bool> IsKYAApproved { get; set; }
    }

    public class UserWiseRequest
    {
        public Nullable<int> DepartmentId { get; set; }
        // public string DepartmentName { get; set; }
        public Nullable<int> RequestId { get; set; }
        //public string RequestStatus { get; set; }
        public Nullable<int> ServiceId { get; set; }
        // public string ServiceName { get; set; }
        public int TotalRequest { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Matrix { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class PendencyReport
    {
        public int Request_No { get; set; }
        public Nullable<int> rid { get; set; }
        public Nullable<int> departmentid { get; set; }
        public string departmentName { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string propertyNo { get; set; }
        public Nullable<int> Requested_By { get; set; }
        public string requestedname { get; set; }
        public Nullable<System.DateTime> Requested_Date { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> Approved_By { get; set; }
        public string ApprovedName { get; set; }
        public Nullable<System.DateTime> Approved_Date { get; set; }
        public string PagePath { get; set; }
        public string requestType { get; set; }
        public string strStatus { get; set; }
        private string encodedParam;
        public string EncodedParameter
        {
            get { return encodedParam = CommonHelper.Encode(Request_No.ToString()); }
            set { encodedParam = CommonHelper.Encode(Request_No.ToString()); }
        }
    }

    public class ServiceTypeList
    {
        public int TotalCount { get; set; }
        public string ServiceType { get; set; }
    }

    public class VacantPropertyViewModel
    {
        public int? DepartmentId { get; set; }
        public int? SectorId { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public string DepartmentName { get; set; }
        public Decimal? TotalArea { get; set; }
        public string PlotProperty { get; set; }
    }

    public class SMSLogViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> SenderId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }


        public string Sector { get; set; }
        public string Block { get; set; }
        public string Department { get; set; }
        public string PlotNo { get; set; }
        public string PropertyNo
        {
            get
            {
                return Sector + "/" + Block + "-" + PlotNo;
            }
        }
        public string MobileNo { get; set; }
        public string SMSType { get; set; }
        public string Heading { get; set; }
        public string StatusMsg { get; set; }
        public string Message { get; set; }
        public string Sender { get; set; }
        public string Applicant { get; set; }

        public Nullable<DateTime> SentDate { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }

        public Nullable<bool> Status { get; set; }
    }
}
