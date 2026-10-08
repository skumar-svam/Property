using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Model
{
    public class EpassViewModel
    {
        public Nullable<int> Id { get; set; }
        public string EpassNo { get; set; }
        public string EpassType { get; set; }
        public string Applicant { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string LocalPlace { get; set; }
        public string PIN { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string Purpose { get; set; }
        public string VehicleType { get; set; }
        public string VehicleNo { get; set; }
        public string RCNo { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public string PhotoPath { get; set; }
        public string UserIdPath { get; set; }
        public string RCPath { get; set; }
        public string LicencePath { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<DateTime> ApprovalDate { get; set; }
        public string Approver { get; set; }
        public string Comment { get; set; }
        public string EPDI { get; set; }
        public string EPDII { get; set; }
        public string EPDIII { get; set; }
        public string EPDIV { get; set; }
        public string EPDV { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsAuthorityEmployee { get; set; }
        public string ApplicantImage { get; set; }
        public Nullable<bool> IsDeclarationChecked { get; set; }
        public string Status { get; set; }
        public Nullable<DateTime> ValidTillDate { get; set; }
    }

}
