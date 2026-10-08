using NA.PMS.Model;
using NA.PMS.Service.AccountWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Web.Http.Description;

namespace NA.PMS.Web.Controllers
{
    public class PropertyApiController : ApiController
    {

        private UserViewModel[] users = new UserViewModel[]
        {
            new UserViewModel { Id = 1, UserName = "Haleemah Redfern", Email = "email1@mail.com", MobileNo = "01111111", RoleId = 1},
            new UserViewModel { Id = 2, UserName = "Aya Bostock", Email = "email2@mail.com", MobileNo = "01111111", RoleId = 1},
            new UserViewModel { Id = 3, UserName = "Sohail Perez", Email = "email3@mail.com", MobileNo = "01111111", RoleId = 1},
            new UserViewModel { Id = 4, UserName = "Merryn Peck", Email = "email4@mail.com", MobileNo = "01111111", RoleId = 2},
            new UserViewModel { Id = 5, UserName = "Cairon Reynolds", Email = "email5@mail.com", MobileNo = "01111111", RoleId = 3}
        };

        // GET: api/Users
        [ResponseType(typeof(IEnumerable<UserViewModel>))]
        public IEnumerable<UserViewModel> Get(UserViewModel model)
        {
            return users;
        }

       // [HttpGet]
       // [AllowAnonymous]
       // [Route("api/GetPendingSyncReceiptDetail/{entryDate:datetime:regex(\\d{4}-\\d{2}-\\d{2})}")]//"yyyy-mm-dd" format
       // public ReceiptDetails GetPendingSyncReceiptDetail(DateTime entryDate)
       // {
       //     ReceiptDetails result = null;

       //     IAccountWSService _accountWSService = new AccountWSService();
       //     result = _accountWSService.GetReceiptDetail(entryDate);
       //     return result;
       // }

       // [HttpGet]
       // [AllowAnonymous]
       //// [Authorize]
       // [Route("api/SyncReceiptDB/{entryDate:datetime:regex(\\d{4}-\\d{2}-\\d{2})}")]//"yyyy-mm-dd" format
       // public SyncOutPut SyncReceiptDB(DateTime entryDate)
       // {
       //     SyncOutPut outResult = new SyncOutPut();
       //     IAccountWSService _accountWSService = new AccountWSService();
       //     ReceiptDetails result = _accountWSService.GetReceiptDetail(entryDate);

       //     if (result != null)
       //     {
       //         if (result.ReciptDetailMaster != null)
       //         {
       //             outResult.ReceiptMaster = _accountWSService.InsertReceiptMasterData(result.ReciptDetailMaster);

       //         }
       //         if (result.ReceiptAmountTrans != null)
       //         {
       //             outResult.ReceiptTrans = _accountWSService.InsertReceiptTransData(result.ReceiptAmountTrans);
       //         }
       //     }
       //     return outResult;
       // }

    }


}
