using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
   public class SectorBlock
    {
        public int MasterTypeId { get; set; }
        public string MasterTypeValue { get; set; }
        public string createdBy { get; set; }
    }

   public class MasterTypeForSectorBlock
   {
       public int MasterId { get; set; }
       public string MasterValue { get; set; }
       public string MasterName { get; set; }
   }
}
