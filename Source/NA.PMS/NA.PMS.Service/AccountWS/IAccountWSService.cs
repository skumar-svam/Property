using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Service.AccountWS
{
    public interface IAccountWSService
    {
        ReceiptDetails GetReceiptDetail(DateTime entryDate);
        OutResult InsertReceiptMasterData(List<RECEIPT_MASTER> lstMaster);
        OutResult InsertReceiptTransData(List<RECEIPT_TRANS> lstReceiptTrans);
        List<PropertyDocument> GetDocumentDetails(int rid);

        OutResultModel SaveReceiptData(ReceiptModel receiptmodel);
    }
}
