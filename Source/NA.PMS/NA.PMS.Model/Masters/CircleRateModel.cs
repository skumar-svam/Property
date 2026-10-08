using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NA.PMS.Model
{

 public   class CircleRateModel
    {
        [DisplayName("Department")]
        [Required]
        public Nullable<int> departmentId { get; set; }
        public string departmentName { get; set; }
        [DisplayName("Sector")]
        public int sector { get; set; }
        [DisplayName("Rate (per sq.mtr)")]
        public Nullable<decimal> rate { get; set; }
        [DisplayName("Start Date")]
        public Nullable<System.DateTime> StartDate { get; set; }
        [DisplayName("End Date")]
         public Nullable<System.DateTime> EndDate { get; set; }
        public int refid { get; set; }
        public string sectorName { get; set; }
        public string blockName { get; set; }
       public int blockId { get; set; }
    }
}
