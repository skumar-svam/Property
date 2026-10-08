using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class CICModel
    {
        public int Id { get; set; }

        public int Director_Id { get; set; }

        public Nullable<int> Rid { get; set; }

        [DisplayName("Director Name")]
        public string Director_Name { get; set; }

        [DisplayName("Type")]
        public Nullable<int> Type { get; set; }

        public string TypeName { get; set; }

        [DisplayName("Director Share")]
        public Nullable<decimal> Director_Share { get; set; }

        [DisplayName("CIC Charge")]
        public Nullable<decimal> CIC_Charge { get; set; }

        public Nullable<int> Change_Type { get; set; }

        public string ChangeTypeName { get; set; }

        [DisplayName("Old Firm Name")]
        public string OldFirmName { get; set; }

        [DisplayName("New Firm Name")]
        public string NewFirmName { get; set; }

        [DisplayName("Old Firm Status")]
        public Nullable<int> OldFirmStatus { get; set; }

        public string OldFirmStatusName { get; set; }

        [DisplayName("New Firm Status")]
        public Nullable<int> NewFirmStatus { get; set; }

        [DisplayName("Old Firm Product")]
        public string OldFirmProduct { get; set; }

        [DisplayName("New Firm Product")]
        public string NewFirmProduct { get; set; }

        public string DepartmentName { get; set; }

        public string PropNo { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public string PropertyNo { get; set; }

        public int SNo { get; set; }

        public string User { get; set; }

        public Nullable<System.DateTime> Request_Date { get; set; }

        public Nullable<System.DateTime> Approved_Date { get; set; }

        public Nullable<int> Is_Active { get; set; }

        public Nullable<int> Created_By { get; set; }

        public Nullable<System.DateTime> Created_Date { get; set; }

        public Nullable<int> Modified_By { get; set; }

        public Nullable<System.DateTime> Modified_Date { get; set; }

        public string AssignTo { get; set; }

        public int? Status { get; set; }

        public string StatusName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string ApplicantName { get; set; }

        public string FatherName { get; set; }

        public string PropertyNumber { get; set; }

        public string PropertyTypeName { get; set; }

        public string Gender { get; set; }

        public string Comment { get; set; }

        public int SchemeId { get; set; }

        public int DepartmentId { get; set; }

        public int FormId { get; set; }

        public int applicationId { get; set; }

        public string MobileNumber { get; set; }

        public string Email { get; set; }

        public string IsFirmExists { get; set; }

        public string IsAllotted { get; set; }
        [DisplayName("Request Reference No.")]
        public int ReqRefNo { get; set; }

        // Added on 6 Sept. 2018 for [ share type - value or percentage ]
        public string Share_Type { get; set; }
    }
}
