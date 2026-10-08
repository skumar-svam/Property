using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class ReceiptDetails
    {
        public List<RECEIPT_MASTER> ReciptDetailMaster { get; set; }
        public List<RECEIPT_TRANS> ReceiptAmountTrans { get; set; }
    }

    public class RECEIPT_MASTER
    {
        public long RECEIPT_ID { get; set; }
        public string PROPERTY_NUMBER { get; set; }
        public string PROP_REG_ID { get; set; }
        public string PROP_ID { get; set; }
        public string SECTOR { get; set; }
        public string BLOCK { get; set; }
        public string ALLOTE_NAME { get; set; }
        public string DEPOSETER_NAME { get; set; }
        public string ADDRESS { get; set; }
        public Nullable<int> DEPT_ID { get; set; }
        public string BANK_ID { get; set; }
        public string CHALLAN_ID { get; set; }
        public Nullable<System.DateTime> DEPOSIT_DATE { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }
        public Nullable<int> STATUS { get; set; }
        public string USERID { get; set; }
        public Nullable<System.DateTime> ENTRY_DATE { get; set; }
        public Nullable<System.DateTime> MODIFY_DATE { get; set; }
        public string RID_NO { get; set; }
        public Nullable<System.DateTime> P_FROM_DATE { get; set; }
        public Nullable<System.DateTime> P_TO_DATE { get; set; }
        public string CONS_NO { get; set; }
        public string FLAG_EDIT { get; set; }

    }

    public class RECEIPT_TRANS
    {
        public int ID { get; set; }
        public Nullable<long> RECEIPT_ID { get; set; }
        public Nullable<int> DEPT_CODE { get; set; }
        public Nullable<int> RECEIPT_HEAD_ID { get; set; }
        public Nullable<int> RECEIPT_SUBHEAD_ID { get; set; }
        public string CHALLAN_ID { get; set; }
        public Nullable<System.DateTime> DEPOSIT_DATE { get; set; }
        public Nullable<decimal> AMOUNT_PAID { get; set; }
        public Nullable<int> STATUS { get; set; }
        public string USERID { get; set; }
        public Nullable<System.DateTime> ENTRY_DATE { get; set; }

    }

    public class OutResult
    {
        public string NewlyAdded_Receipt { get; set; }
        public string Modified_Receipt { get; set; }
        public string ErrorMessage { get; set; }

    }
    public class SyncOutPut
    {
        public OutResult ReceiptMaster { get; set; }
        public OutResult ReceiptTrans { get; set; }
    }
}
