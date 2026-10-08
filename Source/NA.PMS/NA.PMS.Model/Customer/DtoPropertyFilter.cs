using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class DtoPropertyFilter
    {
        public int PropertyId { get; set; }
        public string PropertyNo { get; set; }
        public string Block { get; set; }
        public string Sector { get; set; }
        public string PropertyTypeId { get; set; }
        public int AreaId { get; set; }
        public int AllotmentStatusId { get; set; }
        public int Year { get; set; }
        public string AlloteName { get; set; }
        public int RegistrationId { get; set; }
        public string SearchType { get; set; }
        public string PlotNumber { get; set; }
        public string MonYear { get; set; }
        public int LegalTypeID { get; set; }
        public int PaymentStatus { get; set; } // 1 : Paid ,2:Due 3:Defaulted
        public int ComplaintStatusId { get; set; }
        public string SelectedQuarter { get; set; }
        public int PaymentScheduleId { get; set; }
        public string Username { get; set; }
    }
}
