using NA.PMS.Common.Helpers;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Dal.DBConnection
{
    public class NADBConnection
    {
        public static DbConnection GetPISConnection()
        {
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PISSqlConnection"].ConnectionString);
            if (SettingsHelper.IsMiniProfilerEnabled)
            {
                return new StackExchange.Profiling.Data.ProfiledDbConnection(connection, MiniProfiler.Current);
            }
            else
            {
                return connection;
            }
        }

        public static DbConnection GetPIMSConnection()
        {
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PIMSSqlConnection"].ConnectionString);
            if (SettingsHelper.IsMiniProfilerEnabled)
            {
                return new StackExchange.Profiling.Data.ProfiledDbConnection(connection, MiniProfiler.Current);
            }
            else
            {
                return connection;
            }
        }
    }
}
