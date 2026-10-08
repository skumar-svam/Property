using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model.Login
{
    public class UserTypeAndIdVM
    {
        public string UserRoleType { get; set; }
        public int UserID { get; set; }
        public bool IsUserExist { get; set; }
    }
}
