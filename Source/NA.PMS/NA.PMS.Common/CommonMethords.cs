using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Model;
using NA.PMS.Common;
using System.Web;
using NA.PMS.Web.Models;
using NA.PMS.Model.CommonModel;
using System.Data.SqlClient;
using System.Data;
namespace NA.PMS.Common
{
    public static class CommonMethords
    {
        /// <summary>
        /// Send SMS to user who forgot password
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <param name="msg"></param>

        public static void SMSSend(string mobileNo, string msg)
        {
            WebClient client = new WebClient();
            string baseurl = ConfigurationManager.AppSettings["SMSsend"].ToString() + ConfigurationManager.AppSettings["SMSUsername"].ToString() + "&password=" + ConfigurationManager.AppSettings["SMSPassword"].ToString() + "&sendername=" + ConfigurationManager.AppSettings["SMSSenderID"].ToString() + "&mobileno=" + mobileNo + "&message=" + msg;
            Stream data = client.OpenRead(baseurl);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            data.Close();
            reader.Close();
        }
        public static RoleMenuKeyModel SetRolePrmision(int menuKey)
        {
            RoleMenuKeyModel objRoleMenuKey = new RoleMenuKeyModel();
            if (menuKey != 0)
            {
                var loginUser = (CurrentUserDetail)HttpContext.Current.Session["CurrentUser"];
                if (loginUser != null)
                {
                    foreach (var Role in loginUser.MenuMaster)
                    {
                        if (Role != null && Role.MenuId == menuKey)
                        {
                            objRoleMenuKey.EditMenuVal = Role.IsUpdate;
                            objRoleMenuKey.AddMenuVal = Role.IsWrite;
                            objRoleMenuKey.DeleteMenuVal = Role.Isdelete;
                            objRoleMenuKey.ReadOnlyMenu = Role.IsRead;
                        }
                    }
                }
                //    else
                //    {
                //        RedirectToAction("Login", "Account", new { area = "" });
                //    }
                //}
                //else
                //{
                //    RedirectToAction("Login", "Account", new { area = "" });
                //}
            }
            return objRoleMenuKey;
            
        }
        public static string GenerateLetter(int rid, int templateId, int departmentId,int UserID)
        {
            var strLettter = 0;

            string constring = ConfigurationManager.ConnectionStrings["PIMSSqlConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("Sp_LatterPrintTemp", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@registrationId", rid.ToString());
                    cmd.Parameters.Add("@templateId", templateId);
                    cmd.Parameters.Add("@departmentId", departmentId);
                    cmd.Parameters.Add("@userId", UserID.ToString());
                    cmd.Parameters.Add("@CommaString", SqlDbType.VarChar, 8000);
                    cmd.Parameters["@CommaString"].Direction = ParameterDirection.Output;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    return cmd.Parameters["@CommaString"].Value.ToString();
                }
            }
        }

    }
}