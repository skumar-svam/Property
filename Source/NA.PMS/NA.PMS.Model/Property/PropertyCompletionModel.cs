using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NA.PMS.Model
{
    public class PropertyCompletionModel
    {
        public int ReqNo { get; set; }
        public int? RId { get; set; }
        public int DepttId { get; set; }
        public string DepttName { get; set; }
        public string PropNo { get; set; }
        public string PropertyNo { get; set; }
        public int? SchemeId { get; set; }
        public string SchemeName { get; set; }
        public int? PropertyId { get; set; }
        public string ApplicantName { get; set; }
        public string Gender { get; set; }
        public string FatherOrHusbandName { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string PropType { get; set; }
        public int PropTypeId { get; set; }
        public decimal? Area { get; set; }
        public string Floor { get; set; }
        public int FloorId { get; set; }
        public DateTime? LeaseDeedDate { get; set; }
        public DateTime? PossessionDate { get; set; }
        public string BuildingPlanApproved { get; set; }
        public DateTime? CompletionDueDate { get; set; }
        public DateTime? CompletionExecutionDate { get; set; }
        public string CompletionTimeFrom { get; set; }
        public string CompletionType { get; set; }
        public Decimal? CompletionCharges { get; set; }
        public Decimal? PartCompletionPercentage { get; set; }
        public Decimal? PartCompletionArea { get; set; }
        public int ExtensionCharge { get; set; }
        public string Extension { get; set; }
        public string ViewName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? BuildingPlanSanctionDate { get; set; }
        public DateTime? CurrentDateCompletion { get; set; }
        public DateTime? CompletionExecutionMinDate { get; set; }
      
        public string PropertyNumber { get; set; }
        public string PropertyType { get; set; }
        //public decimal Area { get; set; }
        public string FloorAreaRange { get; set; }
        //public string Gender { get; set; }
        public DateTime LeaseDeedDueDate { get; set; }
        public DateTime LeaseDeedExecutionDate { get; set; }
        public bool Print { get; set; }
        public string PossessionBy { get; set; }
        public bool IsSelected { get; set; }
        public string AreaChangeType { get; set; }
        public string SectorName { get; set; }
        public string BlockName { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Applicant
        {
            get
            {
                return FirstName + " " + MiddleName + " " + LastName;
            }
        }

        public int CurrentYear
        {
            get { return DateTime.Now.Year; }

        }
        public string CurrentDate
        {
            get { return DateTime.Now.Date.ToShortDateString(); }

        }

        public virtual AllotmentMaster AllotmentMaster { get; set; }
        public virtual StatusMaster StatusMaster { get; set; }
    }
}
