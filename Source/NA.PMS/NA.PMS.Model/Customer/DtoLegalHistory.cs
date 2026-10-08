using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class DtoLegalHistory
    {
        public int PropertyId { get; set; }
        public string PropertyNo { get; set; }
        public string Prpty_Sec { get; set; }
        public string Prpty_Block { get; set; }
        public double? Rid_no { get; set; }
        public string CASENO { get; set; }
        public double? YEAR { get; set; }
        public string COURT { get; set; }
        public string PARTIES { get; set; }
        public string DEPTT { get; set; }
        public string DESCRIPTION { get; set; }
        public string ADVOCATEDETAILS { get; set; }
        public string STAY { get; set; }
        public string filedFlag { get; set; }
        public string DATE { get; set; }
        public string NARRATIVEYES_NO { get; set; }
        public int? RegistrationId { get; set; }
    }
}
