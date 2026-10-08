
namespace NA.PMS.Model
{
    public class NiveshMitraMasterVM
    {
        public int ID { get; set; }
        public string ControlID { get; set; }
        public string UnitID { get; set; }
        public string ProcessIndustryID { get; set; }
        public string ServiceID { get; set; }
        public string ApplicationID { get; set; }
        public string Sector { get; set; }
        public string Block { get; set; }
        public string PlotNo { get; set; }
        public string MobileNo { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public System.DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public System.DateTime? ModifiedDate { get; set; }

        public string ServiceName { get; set; }
    }
}
