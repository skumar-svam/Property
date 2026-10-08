using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace NA.PMS.MTC
{
    //public class MasterDashboardRepository
    //{

    //}
    public class MasterDashboardRepository
    {
        #region Getting Connection
        /// <summary>
        /// Global variables
        /// </summary>
        private SqlConnection con;

        /// <summary>
        /// Getting Connection
        /// </summary>
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["PIMSSqlConnection"].ToString();
            con = new SqlConnection(constr);
        }
        #endregion

        public List<MasterDashboardModel> GetAllReportsDeptWise(int deptid, string StartDate, string EndDate, string RequestThrough)
        {
            NoidaPMSEntities context = new NoidaPMSEntities();
            List<MasterDashboardModel> lst = new List<MasterDashboardModel>();
            int type = 51;

            try
            {
                if (deptid == 51)
                {
                    if (RequestThrough == "null")
                    {
                        //string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_all_dept " + "null" + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "null"; 
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + "null" + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {

                        //string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_all_dept " + "null" + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "null"; 
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + "null" + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "'" + RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                }
                else
                {
                    if (RequestThrough == "null")
                    {
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + deptid + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + " null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + deptid + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "'" + RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return lst;
        }

        public DataSourceResult GetServiceTimelineReportsAsDataSource(DataSourceRequest request, MasterDashboardModel model)
        {
            NoidaPMSEntities context = new NoidaPMSEntities();
            List<MasterDashboardModel> lst = new List<MasterDashboardModel>();
            int type = 2;
            try
            {
                if (model.ActionType == "DepartmentWise")
                {
                    if (model.RequestThrough == "null")
                    {
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + model.DepartmentId + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + type + "," + "null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + model.DepartmentId + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + type + "," + "'" + model.RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                }
                else if (model.ActionType == "ServiceDetails")
                {
                    if (model.RequestThrough == "null")
                    {
                        string SqlQuery = "EXEC rsp_ServiceDetailsByServiceId_property " + model.ServiceId + "," + model.DepartmentId + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + model.ActionTypeId + "," + "null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {
                        string SqlQuery = "EXEC rsp_ServiceDetailsByServiceId_property " + model.ServiceId + "," + model.DepartmentId + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + model.ActionTypeId + "," + "'" + model.RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                }
                else if (model.ActionType == "All")
                {
                    if (model.RequestThrough == "null")
                    {
                        //type = 51;
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + "null" + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + 51 + "," + "null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {
                        //type = 51;
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + "null" + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + 51 + "," + "'" + model.RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();

                    }

                }
            }
            catch (Exception ex)
            {

            }
            return lst.ToDataSourceResult(request);
        }

        #region get service request For Accounts
        public List<MasterDashboardModel> GetAccountTypeReport(int deptid, string StartDate, string EndDate, string RequestThrough)
        {
            NoidaPMSEntities context = new NoidaPMSEntities();
            List<MasterDashboardModel> lst = new List<MasterDashboardModel>();
            try
            {
                int type;
                if (deptid == 51)
                {
                    if (RequestThrough == "null")
                    {
                        type = 51;
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_Account " + "null" + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {
                        type = 51;
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_Account " + "null" + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "'" + RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                }
                else
                {
                    if (RequestThrough == "null")
                    {
                        type = 2;
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_Account " + deptid + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {
                        type = 2;
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_Account " + deptid + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "'" + RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return lst;
        }
        public DataSourceResult GetServiceTimelineAccountReportsAsDataSource(DataSourceRequest request, MasterDashboardModel model)
        {
            NoidaPMSEntities context = new NoidaPMSEntities();
            List<MasterDashboardModel> lst = new List<MasterDashboardModel>();
            int type = 2;
            try
            {
                if (model.ActionType == "DepartmentWise")
                {
                    if (model.RequestThrough == "null")
                    {
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_Account " + model.DepartmentId + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + type + "," + "null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_Account " + model.DepartmentId + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + type + "," + "'" + model.RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                }
                else if (model.ActionType == "ServiceDetails")
                {
                    if (model.RequestThrough == "null")
                    {
                        string SqlQuery = "EXEC rsp_ServiceDetailsByServiceId_accounts " + model.ServiceId + "," + model.DepartmentId + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + model.ActionTypeId + "," + "null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {
                        string SqlQuery = "EXEC rsp_ServiceDetailsByServiceId_accounts " + model.ServiceId + "," + model.DepartmentId + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + model.ActionTypeId + "," + "'" + model.RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                }
                else if (model.ActionType == "All")
                {
                    if (model.RequestThrough == "null")
                    {
                        //type = 51;
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_Account " + "null" + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + 51 + "," + "null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {
                        //type = 51;
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_Account " + "null" + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + 51 + "," + "'" + model.RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return lst.ToDataSourceResult(request);
        }

        #endregion

        #region Dues Dashboard

        public List<rsp_defaulter_total_alldept_Result> rsp_defaulter_total_alldept()
        {
            connection();
            List<rsp_defaulter_total_alldept_Result> DefaulterList = new List<rsp_defaulter_total_alldept_Result>();

            SqlCommand com = new SqlCommand("rsp_defaulter_total_alldept", con);
            com.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();

            con.Open();
            da.Fill(dt);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                DefaulterList.Add(new rsp_defaulter_total_alldept_Result
                {
                    departmentName = Convert.ToString(dr["departmentName"]),
                    DuesType = Convert.ToString(dr["DuesType"]),
                    DuesCount = Convert.ToInt32(dr["DuesCount"])
                });
            }
            return DefaulterList;
        }

        public List<rsp_defaulter_duestotal_alldept_Result> rsp_defaulter_duestotal_alldept()
        {
            connection();
            List<rsp_defaulter_duestotal_alldept_Result> defaulter_dues_total_alldept_Result = new List<rsp_defaulter_duestotal_alldept_Result>();

            SqlCommand com = new SqlCommand("rsp_defaulter_dues_total_alldept", con);
            com.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();

            con.Open();
            da.Fill(dt);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                defaulter_dues_total_alldept_Result.Add(new rsp_defaulter_duestotal_alldept_Result
                {
                    departmentName = Convert.ToString(dr["departmentName"]),
                    DuesType = Convert.ToString(dr["DuesType"]),
                    duestotal = Convert.ToDecimal(dr["duestotal"])
                });
            }
            return defaulter_dues_total_alldept_Result;
        }

        #endregion


        public List<MasterDashboardModel> GetServicesByEmployeeDashboardReport(int deptid, string StartDate, string EndDate, string RequestThrough)
        {
            NoidaPMSEntities context = new NoidaPMSEntities();
            List<MasterDashboardModel> lst = new List<MasterDashboardModel>();
            int type = 41;

            try
            {
                if (deptid == 51)
                {
                    if (RequestThrough == "null")
                    {
                        //string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + "null" + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "null";
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + "null" + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {

                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + "null" + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "'" + RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                }
                else
                {
                    if (RequestThrough == "null")
                    {
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + deptid + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + " null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + deptid + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "'" + RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return lst;
        }

        public DataSourceResult GetServicesByEmployeeTimelineReportsAsDataSource(DataSourceRequest request, MasterDashboardModel model)
        {
            NoidaPMSEntities context = new NoidaPMSEntities();
            List<MasterDashboardModel> lst = new List<MasterDashboardModel>();
            int type = 42;
            try
            {
                if (model.ActionType == "DepartmentWise")
                {
                    if (model.RequestThrough == "null")
                    {
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + model.DepartmentId + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + type + "," + "null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + model.DepartmentId + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + type + "," + "'" + model.RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                }
                else if (model.ActionType == "ServiceDetails")
                {
                    if (model.RequestThrough == "null")
                    {
                        //string SqlQuery = "EXEC rsp_ServiceDetailsByServiceId_property " + model.ServiceId + "," + model.DepartmentId + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + model.ActionTypeId + "," + "null";
                        string SqlQuery = "EXEC ssp_PropertyServiceExecutionByEmployee " + model.ServiceId + "," + model.DepartmentId + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + model.ActionTypeId + "," + "null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {
                        string SqlQuery = "EXEC rsp_ServiceDetailsByServiceId_property " + model.ServiceId + "," + model.DepartmentId + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + model.ActionTypeId + "," + "'" + model.RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                }
                else if (model.ActionType == "All")
                {
                    if (model.RequestThrough == "null")
                    {
                        //type = 51;
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + "null" + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + 51 + "," + "null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {
                        //type = 51;
                        string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_deptwise_status_merged " + "null" + "," + "'" + model.StartDate + "'" + "," + "'" + model.EndDate + "'" + "," + 51 + "," + "'" + model.RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();

                    }

                }
            }
            catch (Exception ex)
            {

            }
            return lst.ToDataSourceResult(request);
        }

        public List<MasterDashboardModel> GetDashboardChartReportByEmployee(int deptid, string StartDate, string EndDate, string RequestThrough)
        {
            NoidaPMSEntities context = new NoidaPMSEntities();
            List<MasterDashboardModel> lst = new List<MasterDashboardModel>();
            int type = 0;

            try
            {
                if (deptid == 51)
                {
                    if (RequestThrough == "null")
                    {
                        //string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_all_dept " + "null" + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "null"; 
                        string SqlQuery = "EXEC ssp_ServiceRequestDashboardByEmployeeWise " + "null" + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {

                        //string SqlQuery = "EXEC rsp_service_dashboard_request_count_timeline_all_dept " + "null" + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "null"; 
                        string SqlQuery = "EXEC ssp_ServiceRequestDashboardByEmployeeWise " + "null" + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "'" + RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                }
                else
                {
                    if (RequestThrough == "null")
                    {
                        string SqlQuery = "EXEC ssp_ServiceRequestDashboardByEmployeeWise " + deptid + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + " null";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                    else
                    {
                        string SqlQuery = "EXEC ssp_ServiceRequestDashboardByEmployeeWise " + deptid + "," + "'" + StartDate + "'" + "," + "'" + EndDate + "'" + "," + type + "," + "'" + RequestThrough + "'";
                        lst = context.Database.SqlQuery<MasterDashboardModel>(SqlQuery).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return lst;
        }
    }
}
