using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Model
{
   public class ServiceRequestModel
    {
        public int Id { get; set; }
        public Nullable<int> Rid { get; set; }
        public Nullable<int> ServiceRequestId { get; set; }
        public Nullable<int> RegistrationId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> SubDepartmentId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> Modified_By { get; set; }
        public Nullable<int> Request_Status { get; set; }
        public Nullable<int> Created_By { get; set; }
        public Nullable<int> SchemeId { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<int> BlockId { get; set; }
        public Nullable<int> StatusId { get; set; }

        public string Registration_No { get; set; }
        public string RegistrationType { get; set; }
        public string SchemeName { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string Property_No { get; set; }
        public string PropertyNo { get; set; }  
        public string DepartmentName { get; set; }       
        public string SubDepartment { get; set; }      
        public string ServiceName { get; set; }       
        public string Status { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantMaster { get; set; }
        public string ApplicantAddress { get; set; }
        public string Requestor { get; set; }
        public string RequestorAddress { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string RequestStatus { get; set; }
        public string AddComment { get; set; }
        //get & set user role
        public string UserRoleType { get; set; }
        public string ActionType { get; set; }

        public Nullable<DateTime> Created_Date { get; set; }
        public Nullable<DateTime> Modified_Date { get; set; }

        public HttpPostedFileBase Document { get; set; }

        public string EncryptedId { get { return ServiceRequestId != null ? CommonHelper.Encode(ServiceRequestId.ToString()) : null; } }
    }

   public class ServiceReportModel
   {
       public int Id { get; set; }
       public string RegistrationNo { get; set; }
       public string ApplicantName { get; set; }
       public string Sector { get; set; }
       public string Block { get; set; }
       public string PropertyNo { get; set; }
       public string DepartmentName { get; set; }
       public int DepartmentId { get; set; }
       public int ServiceId { get; set; }
       public string ServiceName { get; set; }
       public string Description { get; set; }
       public DateTime CreatedDate { get; set; }
       public string Email { get; set; }
       public string MobileNo { get; set; }
       public bool RequestEntered { get; set; }
       public string Status { get; set; }
       public string SubDepartment { get; set; }
       public bool IsServiceHasDoc { get; set; }

        public string UploadedDocumentName { get; set; }
        public string DispatchDocumentName { get; set; }

        public string EncodedParameter
        {
            get { return CommonHelper.Encode(Id.ToString()); }
            set { CommonHelper.Encode(Id.ToString()); }
        }
   }
}
