using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class CitizenRequestsModel
    {
        public int Id { get; set; }
        public string RId { get; set; }
        public int RefNo { get; set; }
        public DateTime? ReqDate { get; set; }
        public int? SLA { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }
        public string PropertyNo { get; set; } //This is Plot No.       
        public int? ChallanId { get; set; }
        public decimal? DuesAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string Department { get; set; }
        public string PropNo { get { return SectorName + "/" + BlockName + "-" + PropertyNo; } }
        public string Status { get; set; }
        public string EncryptedRId { get { return (RId == null || RId == "") ? null : CommonHelper.Encode(RId); } }
        public int RegistrationId { get { return (RId == null || RId == "") ? 0 : Convert.ToInt32(RId); } }
        public string EncryptedRefNo { get { return RefNo <= 0 ? null : CommonHelper.Encode(RefNo.ToString()); } }
        public string ApplicantName { get; set; }
        public string Description { get; set; }
        public string SubDepartment { get; set; }
        public string MobileNumber { get; set; }
    }

    public class RequestDetails
    {
        public string RId { get; set; }
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string ReqStatus { get; set; }
        public int StatusId { get; set; }
        public string Description { get; set; }

        [RegularExpression(@"^[0-9.]*$", ErrorMessage = "Service must be numeric.")]
        public decimal? ServiceFee { get; set; }

        [RegularExpression(@"^[0-9.]*$", ErrorMessage = "Dues must be numeric.")]
        public decimal? DuesAmnt { get; set; }
        public string AllotteeName { get; set; }
        public string Address { get; set; }
        public string RequestorName { get; set; }
        public string Comment { get; set; }
        public string OldComment { get; set; }
        public string MobileNo { get; set; }
        public string PaymentStatus { get; set; }
        //Following two fields added on request by Vishal Shukla
        public string DispatchNo { get; set; }
        public DateTime? DispatchDate { get; set; }
    }
}
