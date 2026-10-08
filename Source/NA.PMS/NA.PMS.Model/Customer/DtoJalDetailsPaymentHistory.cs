using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class DtoJalDetailsPaymentHistory
    {
        public string Consumer { get; set; }
        public string FlatNo { get; set; }
        public string Block { get; set; }
        public string Sector { get; set; }
        public DateTime? BLFrom { get; set; }
        public DateTime? BLTo { get; set; }
        public DateTime? DueDate { get; set; }
        public double? BillAmt { get; set; }
        public double? Surcharge { get; set; }
        public double? PaidAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public double? PLDIPROPERTY_ID { get; set; }
        public int? RegistrationId { get; set; }
        public string ConsumerNumber { get; set; }
        public string PropertyNumber { get; set; }
    }
}
