using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class PropertyCancellationModel
    {
        public int ReqNo { get; set; }
        public int? RId { get; set; }
        public string DepttName { get; set; }
        public int DepttId { get; set; }
        public string AssignedTo { get; set; }
        public DateTime? ReqDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Status { get; set; }
        public int? TypeOfCancel { get; set; }
        public string ReasonOfCancel { get; set; }
        public int? RefundYesNo { get; set; }
        public int? RefundType { get; set; }
        public decimal? RefundAmt { get; set; }
        public int User { get; set; }
        public string From { get; set; }
        public string ApproverComments { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string Comment { get; set; }
        public string RestorationReason { get; set; }
        public decimal? RestorationCharges { get; set; }
        public DateTime? CancellationDate { get; set; }
    }
}
