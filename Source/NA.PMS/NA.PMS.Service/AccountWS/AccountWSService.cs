using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NA.PMS.Repository.AccountWS;
using NA.PMS.Model;

namespace NA.PMS.Service.AccountWS
{
    public class AccountWSService : IAccountWSService
    {
        IAccountWSRepository _accountWSRepository;
        public AccountWSService()
        {
            _accountWSRepository = new AccountWSRepository();
        }
        public ReceiptDetails GetReceiptDetail(DateTime entryDate)
        {
            return _accountWSRepository.GetReceiptDetail(entryDate);
        }

        public OutResult InsertReceiptMasterData(List<RECEIPT_MASTER> lstMaster)
        {
            return _accountWSRepository.InsertReceiptMasterData(lstMaster);
        }

        public OutResult InsertReceiptTransData(List<RECEIPT_TRANS> lstReceiptTrans)
        {
            return _accountWSRepository.InsertReceiptTransData(lstReceiptTrans);
        }

        public List<PropertyDocument> GetDocumentDetails(int rid)
        {
            return _accountWSRepository.GetDocumentDetails(rid);
        }

        public OutResultModel SaveReceiptData(ReceiptModel receiptmodel)
        {
            return _accountWSRepository.SaveReceiptData(receiptmodel);
        }
    }
}
