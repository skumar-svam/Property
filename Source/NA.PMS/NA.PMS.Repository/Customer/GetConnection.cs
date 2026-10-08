using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using NoidaAuthority.PMS.Common;
using StackExchange.Profiling;
using NA.PMS.Common.Helpers;

namespace NA.PMS.Repository
{
    public class GetConnection
    {
        public static DbConnection GetOpenConnection()
        {
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["PISSqlConnection"].ConnectionString);
            if (SettingsHelper.IsMiniProfilerEnabled)
            {
                return new StackExchange.Profiling.Data.ProfiledDbConnection(conn, MiniProfiler.Current);
            }
            else
            {
                return conn;
            }
        }
    }
}
