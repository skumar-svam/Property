using NA.PMS.Model;
using NA.PMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Service
{
    public class GraphService : IGraphService
    {
        private IGraphRepository _graphRepository;
        public GraphService(IGraphRepository graphRepository)
        {
            _graphRepository = graphRepository;
        }

        public IEnumerable<UserViewModel> GetUsers()
        {
            return _graphRepository.GetUsers();
        }

        public Boolean SendPassword(string email, string mobileNo, string UserId)
        {
            return _graphRepository.SendPassword(email, mobileNo, UserId);
        }

        public Boolean LockUnLockCustomer(string email)
        {
            return _graphRepository.LockUnLockCustomer(email);
        }

        public Boolean ResetPassword(string emailID)
        {
            return _graphRepository.ResetPassword(emailID);
        }

        public Boolean DeactivateUser(string email)
        {
            return _graphRepository.DeactivateUser(email);
        }

        public Boolean RejectCustomer(string email, string mobileNo, string remarks)
        {
            return _graphRepository.RejectCustomer(email, mobileNo, remarks);
        }

    }
}
