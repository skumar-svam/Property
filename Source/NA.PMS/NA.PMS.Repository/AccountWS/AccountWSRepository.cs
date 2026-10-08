using NA.PMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NA.PMS.Web.Models;
using System.Web;
using NA.PMS.Model.Entities;

namespace NA.PMS.Repository.AccountWS
{
    public class AccountWSRepository : IAccountWSRepository
    {
        // Getting UserID from Session
        private CurrentUserDetail userInfo = new CurrentUserDetail();

        public AccountWSRepository()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["CurrentUser"] != null)
                    {
                        userInfo = HttpContext.Current.Session["CurrentUser"] as CurrentUserDetail;
                    }
                }
            }
        }

        public ReceiptDetails GetReceiptDetail(DateTime entryDate)
        {
            ReceiptDetails result = new ReceiptDetails();

            using (var dbContext = new NoidaPMSEntities())
            {

                result.ReciptDetailMaster = (from r in dbContext.RECEIPT_DETAIL_MASTER
                                             where r.STATUS == 1 && r.ENTRY_DATE >= entryDate || r.MODIFY_DATE >= entryDate
                                             select new RECEIPT_MASTER
                                             {
                                                 RECEIPT_ID = r.RECEIPT_ID,
                                                 PROPERTY_NUMBER = r.PROPERTY_NUMBER,
                                                 PROP_REG_ID = r.PROP_REG_ID,
                                                 PROP_ID = r.PROP_ID,
                                                 SECTOR = r.SECTOR,
                                                 BLOCK = r.BLOCK,
                                                 ALLOTE_NAME = r.ALLOTE_NAME,
                                                 DEPOSETER_NAME = r.DEPOSETER_NAME,
                                                 ADDRESS = r.ADDRESS,
                                                 DEPT_ID = r.DEPT_ID,
                                                 BANK_ID = r.BANK_ID,
                                                 CHALLAN_ID = r.CHALLAN_ID,
                                                 DEPOSIT_DATE = r.DEPOSIT_DATE,
                                                 AMOUNT = r.AMOUNT,
                                                 STATUS = r.STATUS,
                                                 USERID = r.USERID,
                                                 ENTRY_DATE = r.ENTRY_DATE,
                                                 MODIFY_DATE = r.MODIFY_DATE,
                                                 RID_NO = r.RID_NO,
                                                 P_FROM_DATE = r.P_FROM_DATE,
                                                 P_TO_DATE = r.P_TO_DATE,
                                                 CONS_NO = r.CONS_NO,
                                                 FLAG_EDIT = r.FLAG_EDIT
                                             }).ToList();


                if (result.ReciptDetailMaster != null)
                {
                    List<long?> lst = result.ReciptDetailMaster.Select(l => (long?)l.RECEIPT_ID).ToList();

                    result.ReceiptAmountTrans = (from r in dbContext.RECEIPT_AMOUNT_TRANS
                                                 where lst.Contains(r.RECEIPT_ID)
                                                 select new RECEIPT_TRANS
                                                    {
                                                        ID = r.id,
                                                        RECEIPT_ID = r.RECEIPT_ID,
                                                        DEPT_CODE = r.DEPT_CODE,
                                                        RECEIPT_HEAD_ID = r.RECEIPT_HEAD_ID,
                                                        RECEIPT_SUBHEAD_ID = r.RECEIPT_SUBHEAD_ID,
                                                        CHALLAN_ID = r.CHALLAN_ID,
                                                        DEPOSIT_DATE = r.DEPOSIT_DATE,
                                                        AMOUNT_PAID = r.AMOUNT_PAID,
                                                        STATUS = r.STATUS,
                                                        USERID = r.USERID,
                                                        ENTRY_DATE = r.ENTRY_DATE
                                                    }).ToList();
                }

            }
            return result;
        }

        public OutResult InsertReceiptMasterData(List<RECEIPT_MASTER> lstMaster)
        {
            OutResult outResult = new OutResult();
            using (var dbContext = new NoidaPMSEntities())
            {
                foreach (var row in lstMaster)
                {
                    try
                    {
                        RECEIPT_DETAIL_MASTER dbRow = dbContext.RECEIPT_DETAIL_MASTER.FirstOrDefault(r => r.RECEIPT_ID == row.RECEIPT_ID);
                        if (dbRow != null)
                        {
                            SetReceiptMasterData(row, dbRow);
                            dbContext.SaveChanges();
                            outResult.Modified_Receipt = outResult.Modified_Receipt + "||" + dbRow.RECEIPT_ID.ToString();
                        }
                        else
                        {
                            RECEIPT_DETAIL_MASTER newRow = new RECEIPT_DETAIL_MASTER();

                            SetReceiptMasterData(row, newRow);
                            dbContext.RECEIPT_DETAIL_MASTER.Add(newRow);
                            dbContext.SaveChanges();

                            outResult.NewlyAdded_Receipt = outResult.NewlyAdded_Receipt + "||" + newRow.RECEIPT_ID.ToString();

                        }
                    }
                    catch (Exception ex)
                    {
                        outResult.ErrorMessage = outResult.ErrorMessage + "||" + row.RECEIPT_ID + ": " + ex.Message;
                    }
                }

            }
            return outResult;
        }

        public OutResult InsertReceiptTransData(List<RECEIPT_TRANS> lstReceiptTrans)
        {
            OutResult outResult = new OutResult();
            using (var dbContext = new NoidaPMSEntities())
            {
                foreach (var row in lstReceiptTrans)
                {
                    try
                    {
                        RECEIPT_AMOUNT_TRANS dbRow = dbContext.RECEIPT_AMOUNT_TRANS.FirstOrDefault(r => r.RECEIPT_ID == row.RECEIPT_ID && r.RECEIPT_HEAD_ID == row.RECEIPT_HEAD_ID && r.RECEIPT_SUBHEAD_ID == row.RECEIPT_SUBHEAD_ID);
                        if (dbRow != null)
                        {
                            SetReceiptTransData(row, dbRow);
                            dbContext.SaveChanges();
                            outResult.Modified_Receipt = outResult.Modified_Receipt + "||" + dbRow.RECEIPT_ID.ToString() + "," + dbRow.RECEIPT_HEAD_ID.ToString() + "," + dbRow.RECEIPT_SUBHEAD_ID.ToString();
                        }
                        else
                        {
                            RECEIPT_AMOUNT_TRANS newRow = new RECEIPT_AMOUNT_TRANS();
                            SetReceiptTransData(row, newRow);
                            dbContext.RECEIPT_AMOUNT_TRANS.Add(newRow);
                            dbContext.SaveChanges();

                            outResult.NewlyAdded_Receipt = outResult.NewlyAdded_Receipt + "||" + newRow.RECEIPT_ID.ToString() + "," + newRow.RECEIPT_HEAD_ID.ToString() + "," + newRow.RECEIPT_SUBHEAD_ID.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        outResult.ErrorMessage = outResult.ErrorMessage + "||" + row.RECEIPT_ID.ToString() + "," + row.RECEIPT_HEAD_ID.ToString() + "," + row.RECEIPT_SUBHEAD_ID.ToString() + ": " + ex.Message;
                    }
                }
            }
            return outResult;
        }

        private void SetReceiptTransData(RECEIPT_TRANS dataModal, RECEIPT_AMOUNT_TRANS dbData)
        {
            dbData.AMOUNT_PAID = dataModal.AMOUNT_PAID.Value;
            dbData.CHALLAN_ID = dataModal.CHALLAN_ID;
            dbData.DEPOSIT_DATE = dataModal.DEPOSIT_DATE.Value;
            dbData.DEPT_CODE = dataModal.DEPT_CODE;
            dbData.ENTRY_DATE = dataModal.ENTRY_DATE;
            dbData.id = dataModal.ID;
            dbData.RECEIPT_ID = dataModal.RECEIPT_ID;
            dbData.RECEIPT_HEAD_ID = dataModal.RECEIPT_HEAD_ID;
            dbData.RECEIPT_SUBHEAD_ID = dataModal.RECEIPT_SUBHEAD_ID;
            dbData.STATUS = dataModal.STATUS;
            dbData.USERID = dataModal.USERID;
        }

        private void SetReceiptMasterData(RECEIPT_MASTER dataModal, RECEIPT_DETAIL_MASTER dbData)
        {
            dbData.ADDRESS = dataModal.ADDRESS;
            dbData.ALLOTE_NAME = dataModal.ALLOTE_NAME;
            dbData.AMOUNT = dataModal.AMOUNT;
            dbData.BANK_ID = dataModal.BANK_ID;
            dbData.BLOCK = dataModal.BLOCK;
            dbData.CHALLAN_ID = dataModal.CHALLAN_ID;
            dbData.CONS_NO = dataModal.CONS_NO;
            dbData.DEPOSETER_NAME = dataModal.DEPOSETER_NAME;
            dbData.DEPOSIT_DATE = dataModal.DEPOSIT_DATE;
            dbData.DEPT_ID = dataModal.DEPT_ID;
            dbData.ENTRY_DATE = dataModal.ENTRY_DATE;
            dbData.FLAG_EDIT = dataModal.FLAG_EDIT;
            dbData.MODIFY_DATE = dataModal.MODIFY_DATE;
            dbData.P_FROM_DATE = dataModal.P_FROM_DATE;
            dbData.P_TO_DATE = dataModal.P_TO_DATE;
            dbData.PROP_ID = dataModal.PROP_ID;
            dbData.PROP_REG_ID = dataModal.PROP_REG_ID;
            dbData.PROPERTY_NUMBER = dataModal.PROPERTY_NUMBER;
            // dbRow.RECEIPT_AMOUNT_TRANS = row.RECEIPT_AMOUNT_TRANS;
            dbData.RECEIPT_ID = dataModal.RECEIPT_ID;
            dbData.RID_NO = dataModal.RID_NO;
            dbData.SECTOR = dataModal.SECTOR;
            dbData.STATUS = dataModal.STATUS;
            dbData.USERID = dataModal.USERID;
        }

        public List<PropertyDocument> GetDocumentDetails(int rid)
        {
            //string path = System.Configuration.ConfigurationManager.AppSettings["DocsPath"] + "\\" + rid;
            List<PropertyDocument> lstfiles = new List<PropertyDocument>();
            //DirectoryInfo dinfo = new DirectoryInfo(path);
            //FileInfo[] Files = dinfo.GetFiles();
            //foreach (FileInfo file in Files)
            //{
            //    PropertyDocument propertyDocument = new PropertyDocument();
            //    propertyDocument.DocumentName = file.Name;
            //    propertyDocument.DocumentPath = file.FullName;
            //    lstfiles.Add(propertyDocument);
            //}
            return lstfiles;
        }

        #region Method For Web Service (Reciept Entry)

        public OutResultModel SaveReceiptData(ReceiptModel receiptmodel)
        {
            OutResultModel outResult = new OutResultModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                try
                {
                    RECEIPT_DETAIL_MASTER dbRow = dbContext.RECEIPT_DETAIL_MASTER.FirstOrDefault(r => r.RECEIPT_ID == receiptmodel.ReciptDetailMaster.RECEIPT_ID);
                    if (dbRow == null)
                    {
                        if (receiptmodel.ReciptDetailMaster.RECEIPT_ID > 0)
                        {
                            RECEIPT_DETAIL_MASTER new_receipt_detail_master = new RECEIPT_DETAIL_MASTER();
                            MapReceiptMasterData(receiptmodel.ReciptDetailMaster, new_receipt_detail_master);
                            dbContext.RECEIPT_DETAIL_MASTER.Add(new_receipt_detail_master);
                            dbContext.SaveChanges();
                            receiptmodel.ReciptDetailMaster.RECEIPTTRANSLIST.ForEach(m => m.RECEIPT_ID = receiptmodel.ReciptDetailMaster.RECEIPT_ID);
                            SaveReceiptTransData(receiptmodel.ReciptDetailMaster.RECEIPTTRANSLIST);
                            

                            outResult.NewlyAdded_Receipt = new_receipt_detail_master.RECEIPT_ID.ToString();
                        }
                        else { outResult.NewlyAdded_Receipt = "Receipt ID should not be blank."; }
                    }
                }
                catch (Exception ex)
                {
                    outResult.ErrorMessage = receiptmodel.ReciptDetailMaster.RECEIPT_ID + ": " + ex.Message;
                }
            }
            return outResult;
        }

        public OutResultModel SaveReceiptTransData(List<ReceiptAmountTransModel> lstReceiptTrans)
        {
            OutResultModel outResult = new OutResultModel();
            using (var dbContext = new NoidaPMSEntities())
            {
                foreach (var rowTrans in lstReceiptTrans)
                {
                    try
                    {
                        RECEIPT_AMOUNT_TRANS dbRow = dbContext.RECEIPT_AMOUNT_TRANS.FirstOrDefault(r => r.RECEIPT_ID == rowTrans.RECEIPT_ID && r.RECEIPT_HEAD_ID == rowTrans.RECEIPT_HEAD_ID && r.RECEIPT_SUBHEAD_ID == rowTrans.RECEIPT_SUBHEAD_ID);
                        if (dbRow == null)
                        {
                            RECEIPT_AMOUNT_TRANS new_receipt_amount_trans = new RECEIPT_AMOUNT_TRANS();
                            MapReceiptTransData(rowTrans, new_receipt_amount_trans);
                            dbContext.RECEIPT_AMOUNT_TRANS.Add(new_receipt_amount_trans);
                            dbContext.SaveChanges();

                            outResult.NewlyAdded_Receipt = new_receipt_amount_trans.RECEIPT_ID.ToString() + "," + new_receipt_amount_trans.RECEIPT_HEAD_ID.ToString() + "," + new_receipt_amount_trans.RECEIPT_SUBHEAD_ID.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        outResult.ErrorMessage = rowTrans.RECEIPT_ID.ToString() + "," + rowTrans.RECEIPT_HEAD_ID.ToString() + "," + rowTrans.RECEIPT_SUBHEAD_ID.ToString() + ": " + ex.Message;
                    }
                }
            }
            return outResult;
        }

        private void MapReceiptMasterData(ReceiptDetailMasterModel dataModal, RECEIPT_DETAIL_MASTER dbData)
        {
            dbData.ADDRESS = dataModal.ADDRESS;
            dbData.ALLOTE_NAME = dataModal.ALLOTE_NAME;
            dbData.AMOUNT = dataModal.AMOUNT;
            dbData.BANK_ID = dataModal.BANK_ID;
            dbData.BLOCK = dataModal.BLOCK;
            dbData.CHALLAN_ID = dataModal.CHALLAN_ID;
            dbData.CONS_NO = dataModal.CONS_NO;
            dbData.DEPOSETER_NAME = dataModal.DEPOSETER_NAME;
            dbData.DEPOSIT_DATE = dataModal.DEPOSIT_DATE;
            dbData.DEPT_ID = dataModal.DEPT_ID;
            dbData.ENTRY_DATE = dataModal.ENTRY_DATE;
            dbData.FLAG_EDIT = dataModal.FLAG_EDIT;
            dbData.MODIFY_DATE = dataModal.MODIFY_DATE;
            dbData.P_FROM_DATE = dataModal.P_FROM_DATE;
            dbData.P_TO_DATE = dataModal.P_TO_DATE;
            dbData.PROP_ID = dataModal.PROP_ID;
            dbData.PROP_REG_ID = dataModal.PROP_REG_ID;
            dbData.PROPERTY_NUMBER = dataModal.PROPERTY_NUMBER;
            // dbRow.RECEIPT_AMOUNT_TRANS = row.RECEIPT_AMOUNT_TRANS;
            dbData.RECEIPT_ID = dataModal.RECEIPT_ID;
            dbData.RID_NO = dataModal.RID_NO;
            dbData.SECTOR = dataModal.SECTOR;
            dbData.STATUS = dataModal.STATUS;
            dbData.USERID = dataModal.USERID;
        }

        private void MapReceiptTransData(ReceiptAmountTransModel dataModal, RECEIPT_AMOUNT_TRANS dbData)
        {
            dbData.AMOUNT_PAID = dataModal.AMOUNT_PAID.Value;
            dbData.CHALLAN_ID = dataModal.CHALLAN_ID;
            dbData.DEPOSIT_DATE = dataModal.DEPOSIT_DATE.Value;
            dbData.DEPT_CODE = dataModal.DEPT_CODE;
            dbData.ENTRY_DATE = dataModal.ENTRY_DATE;
            dbData.id = dataModal.ID;
            dbData.RECEIPT_ID = dataModal.RECEIPT_ID;
            dbData.RECEIPT_HEAD_ID = dataModal.RECEIPT_HEAD_ID;
            dbData.RECEIPT_SUBHEAD_ID = dataModal.RECEIPT_SUBHEAD_ID;
            dbData.STATUS = dataModal.STATUS;
            dbData.USERID = dataModal.USERID;
        }

        #endregion
    }
}
