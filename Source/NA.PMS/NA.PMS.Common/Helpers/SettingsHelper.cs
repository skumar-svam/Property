using System;
using System.Configuration;

namespace NA.PMS.Common.Helpers
{
    public static class SettingsHelper
    {

        public static bool IsMiniProfilerEnabled
        {
            get
            {
                string result = ConfigurationManager.AppSettings["IsMiniProfilerEnabled"];
                return Convert.ToBoolean(result);
            }
        }
    }
}