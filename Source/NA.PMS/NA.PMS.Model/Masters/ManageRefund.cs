using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace NA.PMS.Model
{
    public class ManageRefund
    {
        public int RefundId { get; set; }
        public string SchemeId { get; set; }
        public string SchemeName { get; set; }
        public string RefundDescription { get; set; }
        public string Created { get; set; }
        public string LastModified { get; set; }
    }

    public class AllottedPropertyPaymentModel
    {
        public int RID { get; set; }

        public string RID_NO { get; set; }

        [Required(ErrorMessage="Receipt id is required")]
        [Range(100000000000, 999999999999, ErrorMessage = "Receipt id should be in 12 digits")]
        public long RECEIPT_ID { get; set; }

        public int RECIEPT_CODE { get; set; }
        public string RECIEPT_HEAD_NAME { get; set; }
        public int RECEIPT_SUBHEAD_ID { get; set; }
        public string RECEIPT_SUB_HEAD { get; set; }
        public string PROPERTY_NUMBER { get; set; }
        public string PROPERTY_REGESTRY_ID { get; set; }
        public string PROPERTY_ID { get; set; }
        public int SECTOR_ID { get; set; }
        public string SECTOR { get; set; }
        public int BLOCK_ID { get; set; }
        public string BLOCK { get; set; }
        public string FIRST_NAME { get; set; }
        public string MIDDLE_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string ALLOTEE_NAME
        {
            get
            {
                return FIRST_NAME + " " + MIDDLE_NAME + " " + LAST_NAME;
            }
        }

        [Required(ErrorMessage = "Depositor name is required")]
        public string DEPOSITOR_NAME { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string ADDRESS { get; set; }

        public Nullable<int> DEPARTMENT_ID { get; set; }
        public string DEPARTMENT_NAME { get; set; }

        [Required(ErrorMessage = "Bank id is required")]
        public string BANK_ID { get; set; }

        [Required(ErrorMessage = "Challan id is required")]
        public string CHALLAN_ID { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public Nullable<decimal> AMOUNT { get; set; }

        public Nullable<int> STATUS { get; set; }

        [Required(ErrorMessage = "Deposit date is required")]
        public Nullable<System.DateTime> DEPOSIT_DATE { get; set; }

        [Required(ErrorMessage = "Entry date is required")]
        public Nullable<System.DateTime> ENTRY_DATE { get; set; }

        public Nullable<System.DateTime> MODIFY_DATE { get; set; }
        
        public Nullable<System.DateTime> P_FROM_DATE { get; set; }
        public Nullable<System.DateTime> P_TO_DATE { get; set; }
        public string CONS_NO { get; set; }
        public string FLAG_EDIT { get; set; }

        public int HEAD_CODE { get; set; }
        public string HEAD_NAME { get; set; }
        public Nullable<decimal> BUDGET_AMOUNT { get; set; }

        public int SUB_HEAD_CODE { get; set; }
        public string SUB_HEAD_NAME { get; set; }

        public string USERID { get; set; }
    }

}