using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.MTC
{
    //public class MTCMasterDashboardViewModel
    //{
    //}
    public class MasterDashboardModel
    {
        public int TotalReceived { get; set; }
        public int WithDrawn { get; set; }
        public int CompletedWithinTime { get; set; }
        public int CompletedBeyondTime { get; set; }
        public int PendingWithTime { get; set; }
        public int PendingMoreThanWeek { get; set; }
        public int PendingMoreThanFortNight { get; set; }
        public int PendingMoreThanOneMonth { get; set; }
        public int TotalReject { get; set; }
        public int TotalObjection { get; set; }
        public int Total { get; set; }
        public int AverageProcessingTime { get; set; }
        public int AverageDelaytime { get; set; }
        public string Department { get; set; }
        public int DepartmentId { get; set; }
        public string EndDate { get; set; }
        public string StartDate { get; set; }
        public string ActionType { get; set; }
        public string ServiceName { get; set; }
        public int ServiceId { get; set; }
        public int RequestId { get; set; }
        public string RegistrationNo { get; set; }
        public string PropertyNo { get; set; }
        public string Applicant { get; set; }
        public DateTime? RequestDate { get; set; }
        public string Status { get; set; }
        public string Approver { get; set; }
        public DateTime? ValidationDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int ActionTypeId { get; set; }
        public int TotalCompleted { get; set; }
        public int TotalPending { get; set; }
        public string RequestThrough { get; set; }
    }
    public class Chart
    {
        public string[] labels { get; set; }
        public List<Datasets> datasets { get; set; }
    }
    public class Datasets
    {
        public string label { get; set; }
        public string[] backgroundcolor { get; set; }
        public string[] bordercolor { get; set; }
        public string borderwidth { get; set; }
        public int?[] data { get; set; }
    }

    #region Dues Dashboard
    public class chartdues
    {
        public string[] labels { get; set; }
        public List<datasetsdues> datasets { get; set; }

    }
    public class datasetsdues
    {
        public string label { get; set; }
        public string[] backgroundColor { get; set; }
        public string[] borderColor { get; set; }
        public string borderWidth { get; set; }
        public int?[] data { get; set; }

        public string fill { get; set; }
        public int pointRadius { get; set; }
        public int pointHoverRadius { get; set; }
    }
    public class chart
    {
        public string[] labels { get; set; }
        public List<datasetss> datasets { get; set; }

    }
    public class datasetss
    {
        public string label { get; set; }
        public string[] backgroundColor { get; set; }
        public string[] borderColor { get; set; }
        public string borderWidth { get; set; }
        public decimal?[] data { get; set; }

        public string fill { get; set; }
        public int pointRadius { get; set; }
        public int pointHoverRadius { get; set; }
    }
    public class rsp_defaulter_total_alldept_Result
    {
        public string departmentName { get; set; }
        public string DuesType { get; set; }
        public Nullable<int> DuesCount { get; set; }
    }
    public class rsp_defaulter_dues_total_alldept_Result
    {
        public int departmentId { get; set; }
        public string departmentName { get; set; }
        public string DuesType { get; set; }
        public Nullable<decimal> DuesCount { get; set; }
    }
    public class rsp_defaulter_dues_total_alldept1_Result
    {
        public int departmentId { get; set; }
        public string departmentName { get; set; }
        public string DuesType { get; set; }
        public Nullable<decimal> duestotal { get; set; }
    }
    public class rsp_defaulter_duestotal_alldept_Result
    {
        public int departmentId { get; set; }
        public string departmentName { get; set; }
        public string DuesType { get; set; }
        public Nullable<decimal> duestotal { get; set; }
    }
    #endregion
}
