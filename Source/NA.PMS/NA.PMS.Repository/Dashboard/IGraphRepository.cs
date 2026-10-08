using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Repository
{
    public interface IGraphRepository
    {
        IEnumerable<UserViewModel> GetUsers();

        Boolean SendPassword(string email, string mobileNo, string UserId);

        Boolean LockUnLockCustomer(string email);

        Boolean ResetPassword(string emailID);

        Boolean DeactivateUser(string email);

        Boolean RejectCustomer(string email, string mobileNo, string remarks);
    }
}
