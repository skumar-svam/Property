
namespace NA.PMS.Model.NIC
{
    public class ServiceStatusVM
    {
        public int Id { get; set; }
        public string StatusName { get; set; }
        public string StatusCode { get; set; }
        public int? Status { get; set; }
        public System.DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public System.DateTime? ModifiedDate { get; set; }

        public int? CitizenStatusID { get; set; }
        public string CitizenStatusName { get; set; }
    }
}
