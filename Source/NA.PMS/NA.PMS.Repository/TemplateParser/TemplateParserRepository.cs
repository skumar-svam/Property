using NA.PMS.Common;
using NA.PMS.Common.TemplateParser;
using NA.PMS.Model;
using NA.PMS.Web.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NA.PMS.Repository.TemplateParser
{
    public class TemplateParserRepository : ITemplateParserRepository
    {
        private CurrentUserDetail userInfo = new CurrentUserDetail();
        public TemplateParserRepository()
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

        public string GetParsedHTML<T>(T model, string templateName)
        {
            return RazorParser.ParseTemplate(model, templateName);
        }

        public string GetGeneratedChallanByRegistrationId(ChallanViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var challanIdPK = 0;
                List<ChallanViewModel> tempDataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempChallanModel"];
                if (tempDataList != null && tempDataList.FirstOrDefault().RegistrationId == model.RegistrationId)
                {
                    decimal? total = 0;
                    Challan_Master challanMaster = new Challan_Master();
                    challanMaster.Rid = model.RegistrationId;
                    challanMaster.Department_Id = model.DepartmentId;
                    challanMaster.Sector_Id = model.ActionType == Constants.Registered ? (string.IsNullOrEmpty(model.Sector) ? Constants.NASectorId : dbContext.SectorMsts.FirstOrDefault(s => s.sectorName == model.Sector.Trim()).sectorId) : model.SectorId;
                    challanMaster.Block_Id = model.ActionType == Constants.Registered ? (string.IsNullOrEmpty(model.Block) ? Constants.NABlockId : dbContext.BlockMsts.FirstOrDefault(s => s.blockName == model.Block.Trim()).blockId) : model.BlockId;
                    challanMaster.Plot_No = model.PlotNo;
                    challanMaster.Allottee = model.Applicant;
                    challanMaster.Address = model.CorrespondAddress;
                    challanMaster.Mobile_No = model.MobileNo;
                    challanMaster.Email = model.Email;
                    challanMaster.Bank_Id = model.BankId;
                    challanMaster.Branch_Id = model.BranchId;
                    challanMaster.Account_Number = model.AccountNo;
                    challanMaster.Created_Date = DateTime.Now;
                    challanMaster.Generate_Date = DateTime.Now;
                    challanMaster.Created_By = userInfo.UserID;
                    challanMaster.Is_Active = true;
                    challanMaster.Is_Verified = false;
                    //challanMaster.ServiceRequestNo = model.ServiceRequestId;
                    challanMaster.PAN = (model.ReferenceType == "PAN" && model.ReferenceType != null) ? model.ReferenceNo : null;
                    challanMaster.GST_No = (model.ReferenceType == "GST" && model.ReferenceType != null) ? model.ReferenceNo : null;
                    dbContext.Challan_Master.Add(challanMaster);
                    dbContext.SaveChanges();
                    //var challanIdPK = dbContext.Challan_Master.Max(m => m.Id);
                    var challanId = challanMaster.Id;
                    model.Id = challanMaster.Id;
                    model.ChallanId = challanMaster.Challan_Id;
                    foreach (var data in tempDataList)
                    {
                        Challan_Trans trans = new Challan_Trans();
                        trans.Rid = model.RegistrationId;
                        trans.Challan_Master_Id = model.Id;
                        //trans.Challan_Master_Id = Convert.ToInt32(model.ChallanId);
                        trans.Head_Id = data.AccountHeadId;
                        trans.Subhead_Id = data.AccountSubHeadId;
                        trans.Amount = data.Amount;
                        trans.Is_Active = true;
                        trans.Created_By = userInfo.UserID;
                        trans.Created_Date = DateTime.Now;
                        dbContext.Challan_Trans.Add(trans);
                        dbContext.SaveChanges();
                        total = total + data.Amount;
                    }
                    model.TotalAmount = total;
                    challanMaster.TotalAmount = model.TotalAmount;
                    dbContext.SaveChanges();
                    string challanTemplate = string.Empty;
                    string bankCopy = string.Empty;
                    string allotteeCopy = string.Empty;
                    string authorityCopy = string.Empty;
                    //var header = GetBankChallanHeaderTemplate(challan);
                    var challanDetail = GetChallanChargesTable(model);
                    var applicantDetail = GetApplicantTable(model);
                    string content = string.Empty;

                    content = "<table style='width:100%;border-top:1px solid black;border-bottom:1px solid black;'><tr><td style='width:50%;border-right: 1px solid black;'> " + challanDetail + "</td><td style='width:50%;'>" + applicantDetail + " </td></tr></table>";
                    var footer = GetChallanFooterTemplate(model);

                    bankCopy = GetChallanHeaderTemplate(model, HeaderName.BankCopy) + content + footer;
                    authorityCopy = GetChallanHeaderTemplate(model, HeaderName.AuthorityCopy) + content + footer;
                    allotteeCopy = GetChallanHeaderTemplate(model, HeaderName.AllotteeCopy) + content + footer;

                    challanTemplate = bankCopy + authorityCopy + allotteeCopy;
                    challanMaster.Content = challanTemplate;
                    dbContext.SaveChanges();
                    return challanTemplate;
                }
                else
                {
                    return Constants.Mismatch;// null;
                }
            }
        }

        public string GetGeneratedChallanByPropertyNo(ChallanViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var challanIdPK = 0;
                List<ChallanViewModel> tempDataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempChallanModelII"];
                if (tempDataList != null)
                {
                    decimal? total = 0;
                    Challan_Master challanMaster = new Challan_Master();
                    challanMaster.Rid = model.RegistrationId;
                    challanMaster.Department_Id = model.DepartmentId;
                    challanMaster.Sector_Id = model.SectorId;
                    challanMaster.Block_Id = model.BlockId;
                    challanMaster.Plot_No = model.PlotNo;
                    challanMaster.Allottee = model.Applicant;
                    challanMaster.Address = model.CorrespondAddress;
                    challanMaster.Mobile_No = model.MobileNo;
                    challanMaster.Email = model.Email;
                    challanMaster.Bank_Id = model.BankId;
                    challanMaster.Branch_Id = model.BranchId;
                    challanMaster.Account_Number = model.AccountNo;
                    challanMaster.Created_Date = DateTime.Now;
                    challanMaster.Generate_Date = DateTime.Now;
                    challanMaster.Created_By = userInfo.UserID;
                    challanMaster.Is_Active = true;
                    challanMaster.Is_Verified = false;
                    challanMaster.PAN = (model.ReferenceType == "PAN" && model.ReferenceType != null) ? model.ReferenceNo : null;
                    challanMaster.GST_No = (model.ReferenceType == "GST" && model.ReferenceType != null) ? model.ReferenceNo : null;
                    dbContext.Challan_Master.Add(challanMaster);
                    dbContext.SaveChanges();
                    //var challanIdPK = dbContext.Challan_Master.Max(m => m.Id);
                    var challanId = challanMaster.Id;
                    model.Id = challanMaster.Id;
                    model.ChallanId = challanMaster.Challan_Id;
                    foreach (var data in tempDataList)
                    {
                        Challan_Trans trans = new Challan_Trans();
                        trans.Rid = model.RegistrationId;
                        trans.Challan_Master_Id = model.Id;
                        //trans.Challan_Master_Id = Convert.ToInt32(model.ChallanId);
                        trans.Head_Id = data.AccountHeadId;
                        trans.Subhead_Id = data.AccountSubHeadId;
                        trans.Amount = data.Amount;
                        trans.Is_Active = true;
                        trans.Created_By = userInfo.UserID;
                        trans.Created_Date = DateTime.Now;
                        dbContext.Challan_Trans.Add(trans);
                        dbContext.SaveChanges();
                        total = total + data.Amount;
                    }
                    model.TotalAmount = total;
                    challanMaster.TotalAmount = model.TotalAmount;
                    dbContext.SaveChanges();
                    string challanTemplate = string.Empty;
                    string bankCopy = string.Empty;
                    string allotteeCopy = string.Empty;
                    string authorityCopy = string.Empty;
                    //var header = GetBankChallanHeaderTemplate(challan);
                    var challanDetail = GetChallanChargesTableII(model);
                    var applicantDetail = GetApplicantTableII(model);
                    string content = string.Empty;

                    content = "<table style='width:100%;border-top:1px solid black;border-bottom:1px solid black;'><tr><td style='width:50%;border-right: 1px solid black;'> " + challanDetail + "</td><td style='width:50%;'>" + applicantDetail + " </td></tr></table>";
                    var footer = GetChallanFooterTemplate(model);

                    bankCopy = GetChallanHeaderTemplateII(model, HeaderName.BankCopy) + content + footer;
                    authorityCopy = GetChallanHeaderTemplateII(model, HeaderName.AuthorityCopy) + content + footer;
                    allotteeCopy = GetChallanHeaderTemplateII(model, HeaderName.AllotteeCopy) + content + footer;

                    challanTemplate = bankCopy + authorityCopy + allotteeCopy;
                    challanMaster.Content = challanTemplate;
                    dbContext.SaveChanges();
                    return challanTemplate;
                }
                else
                {
                    return Constants.Mismatch;// null;
                }
            }
        }

        public ChallanViewModel GetGeneratedChallanByChallanId(ChallanViewModel model)
        {
            if (model != null)
            {
                using (var dbContext = new NoidaPMSEntities())
                {
                    var challan = dbContext.Challan_Master.FirstOrDefault(m => m.Challan_Id == model.ChallanId);
                    model.Id = challan.Id;
                    model.ChallanId = challan.Challan_Id;
                    model.RegistrationId = challan.Rid;
                    model.ChallanContent = challan.Content;
                    model.IsVerified = challan.Is_Verified;
                }
            }
            return model;
        }

        private string GetChallanChargesTable(ChallanViewModel model)
        {
            List<ChallanViewModel> tempDataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempChallanModel"];
            string content = string.Empty;
            int count = 1;
            decimal? total = 0;
            content = "<Table style='width:100%;'><tr style=''><td class='tr-no'>SI No</td><td class='td-head'>Account Head</td><td class='td-amount' style='text-align:right;'>Amount</td>";
            foreach (var data in tempDataList)
            {
                content = content + "<tr  style=''><td class='i-no'>" + count + "</td><td class='td-head'>" + data.AccountSubHead + "</td><td class='td-amount' style='text-align:right;'>" + data.Amount + "</td></tr>";
                count++;
                total = total + data.Amount;
            }
            content = content + "<tr  style=''><td class='i-no'></td><td class='td-head' style='text-align:right;'>Total: </td><td class='td-amount' style='text-align:right;'>" + total + "</td></tr>";
            content = content + "</Table>";
            HttpContext.Current.Session["TempChallanModel"] = null;
            return content;
        }

        private string GetChallanChargesTableII(ChallanViewModel model)
        {
            List<ChallanViewModel> tempDataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempChallanModelII"];
            string content = string.Empty;
            int count = 1;
            decimal? total = 0;
            content = "<Table style='width:100%;'><tr style=''><td class='tr-no'>SI No</td><td class='td-head'>Account Head</td><td class='td-amount' style='text-align:right;'>Amount</td>";
            foreach (var data in tempDataList)
            {
                content = content + "<tr  style=''><td class='i-no'>" + count + "</td><td class='td-head'>" + data.AccountSubHead + "</td><td class='td-amount' style='text-align:right;'>" + data.Amount + "</td></tr>";
                count++;
                total = total + data.Amount;
            }
            content = content + "<tr  style=''><td class='i-no'></td><td class='td-head' style='text-align:right;'>Total: </td><td class='td-amount' style='text-align:right;'>" + total + "</td></tr>";
            content = content + "</Table>";
            HttpContext.Current.Session["TempChallanModelII"] = null;
            return content;
        }

        private string GetApplicantTable(ChallanViewModel model)
        {
            List<ChallanViewModel> tempDataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempChallanModel"];
            string content = string.Empty;
            string gst = model.ReferenceType == "GST" ? model.ReferenceNo : string.Empty;
            string pan = model.ReferenceType == "PAN" ? model.ReferenceNo : string.Empty;
            content = "<Table style='width:100%;'>";
            content = content + "<tr  style=''><td colspan='2'>Property Location: <b> Sector-" + model.Sector + ", Block-" + model.Block + ", Plot/Flat No-" + model.PlotNo + " </b></td></tr>";
            content = content + "<tr  style=''><td colspan='2'>Applicant: <b>" + model.Applicant + "</b></td></tr>";
            content = content + "<tr  style=''><td colspan='2'>GST No.:<b>" + gst + "</b> </td></tr>";
            content = content + "<tr  style=''><td colspan='2'>PAN No.:<b>" + pan + "</b> </td></tr>";
            content = content + "<tr  style=''><td colspan='2'>E-mail: <b>" + model.Email + "</b></td></tr>";
            content = content + "<tr  style=''><td colspan='2'>Mobile No: <b>" + model.MobileNo + "</b></td></tr>";
            content = content + "<tr  style=''><td colspan='2'>Address: <b>" + model.CorrespondAddress + "</b></td></tr>";
            content = content + "</Table>";
            HttpContext.Current.Session["TempChallanModel"] = null;
            return content;
        }

        private string GetApplicantTableII(ChallanViewModel model)
        {
            string content = string.Empty;
            string gst = model.ReferenceType == "GST" ? model.ReferenceNo : string.Empty;
            string pan = model.ReferenceType == "PAN" ? model.ReferenceNo : string.Empty;
            content = "<Table style='width:100%;'>";
            content = content + "<tr  style=''><td colspan='2'>Property Location: <b> Sector-" + model.Sector + ", Block-" + model.Block + ", Plot/Flat No-" + model.PlotNo + " </b></td></tr>";
            content = content + "<tr  style=''><td colspan='2'>Applicant: <b>" + model.Applicant + "</b></td></tr>";
            content = content + "<tr  style=''><td colspan='2'>GST No.:<b>" + gst + "</b> </td></tr>";
            content = content + "<tr  style=''><td colspan='2'>PAN No.:<b>" + pan + "</b> </td></tr>";
            content = content + "<tr  style=''><td colspan='2'>E-mail: <b>" + model.Email + "</b></td></tr>";
            content = content + "<tr  style=''><td colspan='2'>Mobile No: <b>" + model.MobileNo + "</b></td></tr>";
            content = content + "<tr  style=''><td colspan='2'>Address: <b>" + model.CorrespondAddress + "</b></td></tr>";
            content = content + "</Table>";
            return content;
        }

        //New One

        private string GetChallanHeaderTemplate(ChallanViewModel model, string HeaderName)
        {
            var today = DateTime.Now.ToString("dd/MMM/yyyy");
            string content = "<div id='authorityChallan' style='page-break-before:auto;overflow:auto'><Table style='width:100%;'><tr><td style='width:25%;'><b>" + model.BankName + "</b></td><td colspan='2'><center>Date:<b>" + today + "</b></center></td><td style='text-align:right;width:25%;'><b>" + HeaderName + "</b></td></tr>" +
            "<tr><td colspan='4'><center><h4 class='h4-noida'><b>NEW OKHLA INDUSTRIAL DEVELOPMENT AUTHORITY</b></h4></center></td></tr>" +
                //"<tr><td colspan='4'><center><h4 class='h4-dept' style='padding-top:0px;padding-bottom:0px;'><b>"+model.Department+"</b></h4></center></td></tr>" +
            "<tr><td colspan='4'><center><h4 style='padding-top:0px;'><b>GST No.: " + Constants.NoidaAuthorityGSTNo + "</b></h4></center></td></tr>" +
            "<tr><td colspan='2'>Registration Id: <b>" + model.RegistrationId + "</b></td><td colspan='2' style='text-align:right;'>Challan No.:<b>" + model.ChallanId + "</b></td></tr>" +
            "<tr><td colspan='2'>Account No.: <b>" + model.AccountNo + "</b></td><td colspan='2'  style='text-align:right;'>Property Type: <b>" + model.Department + "</b></td></tr>" +
            "<tr><td colspan='2'>IFSC Code: <b>" + model.IFSCCode + "</b></td></tr></Table>";
            return content;
        }

        private string GetChallanHeaderTemplateII(ChallanViewModel model, string HeaderName)
        {
            var today = DateTime.Now.ToString("dd/MMM/yyyy");
            string content = "<div id='authorityChallan' style='page-break-before:auto;overflow:auto'><Table style='width:100%;'><tr><td style='width:25%;'><b>" + model.BankName + "</b></td><td colspan='2'><center>Date:<b>" + today + "</b></center></td><td style='text-align:right;width:25%;'><b>" + HeaderName + "</b></td></tr>" +
            "<tr><td colspan='4'><center><h4 class='h4-noida'><b>NEW OKHLA INDUSTRIAL DEVELOPMENT AUTHORITY</b></h4></center></td></tr>" +
                //"<tr><td colspan='4'><center><h4 class='h4-dept' style='padding-top:0px;padding-bottom:0px;'><b>" + model.Department + "</b></h4></center></td></tr>" +
            "<tr><td colspan='4'><center><h4 style='padding-top:0px;'><b>GST No.: " + Constants.NoidaAuthorityGSTNo + "</b></h4></center></td></tr>" +
            "<tr><td colspan='2'>Registration Id: <b>" + model.RegistrationId + "</b></td><td colspan='2' style='text-align:right;'>Challan No.:<b>" + model.ChallanId + "</b></td></tr>" +
            "<tr><td colspan='2'>Account No.: <b>" + model.AccountNo + "</b></td><td colspan='2'  style='text-align:right;'>Property Type: <b>" + model.Department + "</b></td></tr>" +
            "<tr><td colspan='2'>IFSC Code: <b>" + model.IFSCCode + "</b></td></tr></Table>";
            return content;
        }

        private string GetChallanFooterTemplate(ChallanViewModel model)
        {
            string content = "<Table><tr style='padding-top:15px;'><td colspan='3'><p>Please find enclosed herewith Draft/Pay order No./Cash____________  Dated____________ for Rs.____________Drawn On" +
                        "____________  against above mentioned account head the payment of property Allotted / Lease / Sublease /Rent or any charges to me by NOIDA Authority." +
                    "</p></td></tr>" +
                    "<tr><td colspan='3' style='height:6px;'></td></tr>" +
                    "<tr><td><b>Authorised Signatory</b></td><td></td><td style='text-align:right;'><b>Depositor Signature</b></td></tr>" +

                    "<tr><td colspan='3'><table style='width:100%;border:1px solid;'><tr><td colspan=9 ><center style='border-bottom:1px solid black;'><b> Details of Notes </b></center></td></tr>" +
                    "<tr class='tr-notetype' style='border-bottom:1px solid black;padding-bottom:2px'><td style='border-right:1px solid black;'><center>2000*</center></td><td style='border-right:1px solid black;'><center>500*</center></td><td style='border-right:1px solid black;'><center>200*</center></td><td style='border-right:1px solid black;'><center>100*</center></td><td style='border-right:1px solid black;'><center>50*</center></td><td  style='border-right:1px solid black;'><center>20*</center></td><td style='border-right:1px solid black;'><center>10*</center></td><td style='border-right:1px solid black;'><center>5*</center></td><td>Total</td></tr>" +
                    "<tr style='padding-top:2px;padding-bottom:2px'><td style='border-right:1px solid black;'>&nbsp;</td><td style='border-right:1px solid black;'>&nbsp;</td><td style='border-right:1px solid black;'>&nbsp;</td><td style='border-right:1px solid black;'>&nbsp;</td><td style='border-right:1px solid black;'>&nbsp;</td><td style='border-right:1px solid black;'>&nbsp;</td><td style='border-right:1px solid black;'>&nbsp;</td><td style='border-right:1px solid black;'>&nbsp;</td><td>&nbsp;</td></tr></table></td></tr></Table></div>" +
                    "<tr><p style='font-size:11px;'>Note: </p>" +
                        "<p style='font-size:11px;'>(1)Payment alone will not accrue any right to allottee.</p>" +
                        "<p style='font-size:11px;'>(2)Notwithstanding any request of the allotee the payment, made by the allotee shall be first adjusted towards the interest due, if any, and the balance shall be adjusted towards the annual leaserent and the installment respectively.</p></tr>" +
                //"<p style='font-size:11px;'>(3)IFSC Code: "+model.IFSCCode+"</p>" +
                        "<p style='font-size:11px;'>(3)<b>Allottee will pay GST by Reverse Charge Mechanism against Property. Authority's GST No: 09AAALN0120A1ZV</b></p></tr><hr style='display:block; margin-left:auto;margin-right:auto;margin-top:0;margin-bottom:0;border-style=inset;border-width;1px;page-break-after:always;'><br/>";

            return content;
        }

        //private string GetBankChallanHeaderTemplate(ChallanViewModel model)
        //{
        //    var today = DateTime.Now.ToString("dd/MMM/yyyy");
        //    string content = "<Table style='width:100%;'><tr><td>Bank Name: <b>" + model.BankName + "</b></td><td>Date:<b>" + today + "</b></td><td style='text-align:right;'><b>Bank Copy</b></td></tr>" +
        //    "<tr><td colspan='3'><center><h4><b>NEW OKHLA INDUSTRIAL DEVELOPMENT AUTHORITY</b></h4></center></td></tr>" +
        //    "<tr><td colspan='2'>Registration Id: <b>" + model.RegistrationId + "</b></td><td style='text-align:right;'>Challan No.:<b>" + model.ChallanId + "</b></td></tr>" +
        //    "<tr><td colspan='2'>Property No: <b>" + model.PropertyNo + "</b></td><td style='text-align:right;'>Bank: <b>" + model.BankName + "</b></td></tr>" +
        //    "<tr><td colspan='2'>Applicant: <b>" + model.Applicant + "</b></td><td style='text-align:right;'>Account No: <b>" + model.AccountNo + "</b></td></tr>" +
        //    "<tr><td colspan='2'>Address: <b>" + model.CorrespondAddress + "</b></td><td style='text-align:right;'>Mobile No: <b>" + model.MobileNo + "</b></td></tr></Table>";

        //    return content;
        //}

        //private string GetAuthorityChallanHeaderTemplate(ChallanViewModel model)
        //{
        //    var today = DateTime.Now.ToString("dd/MMM/yyyy");
        //    string content = "<Table style='width:100%;'><tr><td>Bank Name: <b>" + model.BankName + "</b></td><td>Date:<b>" + today + "</b></td><td style='text-align:right;'><b>Authority Copy</b></td></tr>" +
        //    "<tr><td colspan='3'><center><h4><b>NEW OKHLA INDUSTRIAL DEVELOPMENT AUTHORITY</b></h4></center></td></tr>" +
        //    "<tr><td colspan='2'>Registration Id: <b>" + model.RegistrationId + "</b></td><td style='text-align:right;'>Challan No.:<b>" + model.ChallanId + "</b></td></tr>" +
        //    "<tr><td colspan='2'>Property No: <b>" + model.PropertyNo + "</b></td><td style='text-align:right;'>Bank: <b>" + model.BankName + "</b></td></tr>" +
        //    "<tr><td colspan='2'>Applicant: <b>" + model.Applicant + "</b></td><td style='text-align:right;'>Account No: <b>" + model.AccountNo + "</b></td></tr>" +
        //    "<tr><td colspan='2'>Address: <b>" + model.CorrespondAddress + "</b></td><td style='text-align:right;'>Mobile No: <b>" + model.MobileNo + "</b></td></tr></Table>";

        //    return content;
        //}

        //private string GetAllotteeChallanHeaderTemplate(ChallanViewModel model)
        //{
        //    var today = DateTime.Now.ToString("dd/MMM/yyyy");
        //    string content = "<Table style='width:100%;'><tr><td>Bank Name: <b>" + model.BankName + "</b></td><td>Date:<b>" + today + "</b></td><td style='text-align:right;'><b>Allottee Copy</b></td></tr>" +
        //    "<tr><td colspan='3'><center><h4><b>NEW OKHLA INDUSTRIAL DEVELOPMENT AUTHORITY</b></h4></center></td></tr>" +
        //    "<tr><td colspan='2'>Registration Id: <b>" + model.RegistrationId + "</b></td><td style='text-align:right;'>Challan No.:<b>" + model.ChallanId + "</b></td></tr>" +
        //    "<tr><td colspan='2'>Property No: <b>" + model.PropertyNo + "</b></td><td style='text-align:right;'>Bank: <b>" + model.BankName + "</b></td></tr>" +
        //    "<tr><td colspan='2'>Applicant: <b>" + model.Applicant + "</b></td><td style='text-align:right;'>Account No: <b>" + model.AccountNo + "</b></td></tr>" +
        //    "<tr><td colspan='2'>Address: <b>" + model.CorrespondAddress + "</b></td><td style='text-align:right;'>Mobile No: <b>" + model.MobileNo + "</b></td></tr></Table>";

        //    return content;
        //}

        public PaymentViewModel CreateCustomDemandNote(PaymentViewModel model, string templateName)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                string template = RazorParser.ParseTemplate(model, templateName);
                model.DemandNoteContent = template;
                var demand = new DemandNoteDetail();
                demand.RegistrationId = model.RegistrationId;
                demand.NotificationId = model.NotificationId.ToString();
                demand.PropertyId = Convert.ToInt32(model.PropertyId);
                demand.DepartmentId = model.DepartmentId;
                demand.LeaseRentDues = model.LeaseRentDues;
                demand.LeaseRentDuesInterest = model.LeaseRentInterest;
                demand.InstallmentDues = model.InstallmentAmount;
                demand.InstallmentDuesInterest = model.InstallmentInterest;
                //demand.OtherCharges = model.OtherCharges;
                //demand.TotalDuesAmount = model.TotalDuesAmount;
                demand.TotalDuesAmount = model.TotalDuesAmount;
                demand.DemandNoteTemplate = template;
                demand.IsActive = true;
                demand.StatusId = NAStatusId.Initiated;
                demand.CreatedBy = userInfo.UserID;
                demand.CreatedDate = DateTime.Now;
                demand.DemandNoteTypeId = model.DemandNoteTypeId;
                dbContext.DemandNoteDetails.Add(demand);
                dbContext.SaveChanges();
                return model;
            }
        }

        //public PaymentViewModel CreateCustomDemandNote(PaymentViewModel model, string templateName)
        //{
        //    var demand = RazorParser.ParseTemplate(model, templateName);
        //    var datetime = DateTime.Now;
        //    var notes = "<p>You are informed that last date for dues against above mentioned property is &nbsp" + DateTime.Now.ToString("dd/MM/yyyy") + ". " +
        //                     "Kindly pay your dues amount by any bank generated challan in noida.</p>";
        //    model.DemandNoteContent = !string.IsNullOrEmpty(model.DemandNoteContent) ? model.DemandNoteContent : notes;
        //    var template = "<table class='tbl-demand-note' style='width:100%;'>"+
        //        "<tr>"+
        //            "<td colspan='3' style='text-align:center;'>"+
        //                "<h2>New Okhla Industrial Development Authority</h2>"+
        //                "<h3>Administrative Block, Sector - 6, NOIDA</h3>"+
        //                "<h4>"+model.Department+" Accounts Department</h4>"+
        //            "</td>"+
        //        "</tr>"+
        //        "<tr>"+
        //            "<td colspan='3' style='text-align:right;'>"+
        //                "<b>Notification No:"+ model.NotificationId +"</b> <br />"+
        //                "<b>Date:" + DateTime.Now.ToString("dd/MM/yyyy") + "</b>" +
        //            "</td>"+
        //        "</tr>"+
        //        "<tr>"+
        //            "<td colspan='3'>To,</td>"+
        //        "</tr>"+
        //        "<tr>"+
        //            "<td colspan='3'><b>"+model.Applicant+"</b><br /><b>"+ model.CorresspondentAddress+"</b></td>"+
        //        "</tr>"+
        //        "<tr>"+
        //            "<td colspan='3'>Subject:&nbsp;<b>"+model.ActionType+"</b></td>"+
        //        "</tr>"+       
        //        "<tr>"+
        //            "<td colspan='3'>Sir/Madam,</td>"+
        //        "</tr>"+
        //        "<tr>"+
        //            "<td colspan='3'><b>"+ model.DemandNoteContent +"</b></td>"+
        //        "</tr>"+
        //        "<tr>"+
        //            "<td> 1 &nbsp; Installment Amount: </td>"+ 
        //            "<td colspan='2'>"+ model.InstallmentAmount+"</td>"+
        //        "</tr>"+
        //        "<tr>"+
        //            "<td> 2 &nbsp; Installment Interest Amount: </td>"+
        //            "<td colspan='2'>"+ model.InstallmentInterest+"</td>"+
        //        "</tr>"+
        //        "<tr>"+
        //            "<td> 3 &nbsp; Leaserent Amount: </td>"+
        //            "<td colspan='2'>"+ model.LeaseRentAmount+"</td>"+
        //        "</tr>"+
        //        "<tr>"+
        //            "<td> 4 &nbsp; Leaserent Interest Amount: </td>"+
        //            "<td colspan='2'>"+ model.LeaseRentInterest +"</td>"+
        //        "</tr>"+
        //        "<tr>"+
        //            "<td> 5 &nbsp; Total Leaserent Amount: </td>"+
        //            "<td colspan='2'>"+ model.TotalLeaseRent+"</td>"+
        //        "</tr>"+
        //        "<tr>" +
        //            "<td> 6 &nbsp; One Time Leaserent Amount: </td>" +
        //            "<td colspan='2'>" + model.OneTimeLeaseRentAmount + "</td>" +
        //        "</tr>" +
        //        "<tr>"+
        //            "<td> &nbsp; &nbsp; <b>Total</b>: </td>"+
        //            "<td colspan='2'><b>"+ model.TotalDuesAmount + "<b></td>"+
        //        "</tr>"+
        //        "<tr>"+
        //            "<td colspan='3'><b>Amount in words:"+model.AmountInWords+"</b></td>"+
        //        "</tr>"+       
        //        "<tr>"+
        //            "<td colspan='3'><br /><br /><br />"+
        //                "<b>Your faithfully</b><br />"+
        //                "<b>Accounts Manager</b> <br />"+
        //                "<b>Noida Authority</b>" + 
        //            "</td>"+
        //        "</tr>"+
        //    "</table>";

        //    model.DemandNoteContent = template;
        //    return model;
        //}


        public string GetParsedAndSavedDemandNote(PaymentViewModel model, string templateName)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                string demandNoteTemplate = RazorParser.ParseTemplate(model, templateName);

                var demand = new DemandNoteDetail();
                demand.RegistrationId = model.RegistrationId;
                demand.NotificationId = model.NotificationId.ToString();
                demand.PropertyId = Convert.ToInt32(model.PropertyId);
                demand.DepartmentId = model.DepartmentId;
                demand.LeaseRentDues = model.LeaseRentDues;
                demand.LeaseRentDuesInterest = model.LeaseRentInterest;
                demand.InstallmentDues = model.InstallmentAmount;
                demand.InstallmentDuesInterest = model.InstallmentInterest;
                demand.OtherCharges = model.OtherCharges;
                demand.TotalDuesAmount = model.TotalDuesAmount;
                demand.DemandNoteTemplate = demandNoteTemplate;
                demand.IsActive = true;
                demand.StatusId = NAStatusId.Initiated;
                demand.CreatedBy = userInfo.UserID;
                demand.CreatedDate = DateTime.Now;
                demand.DemandNoteTypeId = model.DemandNoteTypeId;
                dbContext.DemandNoteDetails.Add(demand);
                dbContext.SaveChanges();
                return demandNoteTemplate;
            }
            //return RazorParser.ParseTemplate(model, templateName);
        }


        public string GenerateNDCLetter(NDCVeiwModel model, string templateName)
        {
            string ndcletter = string.Empty;
            if (model.ActionType == "Hindi")
            {
                model.Department = model.DepartmentId == 1 ? DepartmentInHindi.Institutional : (model.DepartmentId == 2 ? DepartmentInHindi.Commercial : (model.DepartmentId == 3 ? DepartmentInHindi.Residential : (model.DepartmentId == 4 ? DepartmentInHindi.Industry : (model.DepartmentId == 5 ? DepartmentInHindi.Housing : (model.DepartmentId == 6 ? DepartmentInHindi.GroupHousing : DepartmentInHindi.Residential)))));
                ndcletter = RazorParser.ParseTemplate(model, templateName);
            }
            else
            {
                ndcletter = RazorParser.ParseTemplate(model, "NDCCommercialTemplateEng.cshtml");
            }

            using (var dbContext = new NoidaPMSEntities())
            {
                //var todaysNDC = dbContext.NDCDetailMsts.Where(n => n.RegistrationId.ToString() == model.RegistrationId && n.CreatedDate.Value.Date == DateTime.Now.Date).FirstOrDefault();
                var todaysNDC = (from ndcdetail in dbContext.NDCDetailMsts
                                 where ndcdetail.RegistrationId.ToString() == model.RegistrationId
                                 && DbFunctions.TruncateTime(ndcdetail.CreatedDate) == DbFunctions.TruncateTime(DateTime.Now)
                                 && ndcdetail.IsActive == true
                                 select ndcdetail).FirstOrDefault();
                if (todaysNDC != null)
                {
                    todaysNDC.DepartmentId = model.DepartmentId;
                    todaysNDC.Department = model.ActionType != "Hindi" ? model.Department : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == model.DepartmentId).departmentName;
                    todaysNDC.Sector = model.Sector;
                    todaysNDC.Block = model.Block;
                    todaysNDC.PlotNo = model.PlotNo;
                    todaysNDC.Applicant = model.Applicant;
                    todaysNDC.InstallmentDuesAmount = model.InstallmentPaidAmount;
                    todaysNDC.InstallmentDateInWord = model.InstallmentDateInWord;
                    todaysNDC.InstallmentStatus = model.InstallmentInWord;
                    todaysNDC.InterestDuesAmount = model.InstallmentInterestPaidAmount;
                    todaysNDC.InterestDateInWord = model.InterestDateInWord;
                    todaysNDC.InterestDueStatus = model.InterestInWord;
                    todaysNDC.InstallmentPaidUptoDate = model.InstallmentPaidUpto;
                    todaysNDC.LeaseRentAmount = model.LeaseRentPaidAmount;
                    todaysNDC.LeaseRentDateInWord = model.LeaseRentDateInWord;
                    todaysNDC.LeaseRentStatus = model.LeaseRentInWord;
                    todaysNDC.LeaseRentPaidUptoDate = model.LeaseRentPaidUpto;
                    todaysNDC.ChallanDetail = model.ChallanId;
                    todaysNDC.ChallanAmount = model.ChalanAmount;
                    todaysNDC.LetterNo = model.LetterNo;
                    todaysNDC.BankName = model.BankName;
                    todaysNDC.IsTotalInstallmentPaid = model.IsTotalInstallmentPaid;
                    todaysNDC.IsOneTimeLeasePaid = model.IsOneTimeLeasePaid;
                    todaysNDC.NDCDate = model.NDCDate;
                    todaysNDC.NDCTemplate = ndcletter;
                    todaysNDC.Remarks = model.Remarks;
                    //todaysNDC.LetterType = model.ActionType;
                    //todaysNDC.Address = model.Address;
                    todaysNDC.IsActive = true;
                    todaysNDC.StatusId = NAStatusId.Approved;
                    dbContext.SaveChanges();
                }
                else
                {
                    var exNDC = dbContext.NDCDetailMsts.Where(n => n.RegistrationId.ToString() == model.RegistrationId).ToList();
                    if (exNDC != null)
                    {
                        exNDC.ForEach(f => f.IsActive = false);
                    }
                    NDCDetailMst ndc = new NDCDetailMst();
                    ndc.RegistrationId = Convert.ToInt32(model.RegistrationId);
                    ndc.DepartmentId = model.DepartmentId;
                    ndc.Department = model.ActionType != "Hindi" ? model.Department : dbContext.DepartmentMsts.FirstOrDefault(d => d.departmentId == model.DepartmentId).departmentName;
                    ndc.Sector = model.Sector;
                    ndc.Block = model.Block;
                    ndc.PlotNo = model.PlotNo;
                    ndc.Applicant = model.Applicant;
                    ndc.InstallmentDuesAmount = model.InstallmentPaidAmount;
                    ndc.InstallmentDateInWord = model.InstallmentDateInWord;
                    ndc.InstallmentStatus = model.InstallmentInWord;
                    ndc.InterestDuesAmount = model.InstallmentInterestPaidAmount;
                    ndc.InterestDateInWord = model.InterestDateInWord;
                    ndc.InterestDueStatus = model.InterestInWord;
                    ndc.InstallmentPaidUptoDate = model.InstallmentPaidUpto;
                    ndc.LeaseRentAmount = model.LeaseRentPaidAmount;
                    ndc.LeaseRentDateInWord = model.LeaseRentDateInWord;
                    ndc.LeaseRentStatus = model.LeaseRentInWord;
                    ndc.LeaseRentPaidUptoDate = model.LeaseRentPaidUpto;
                    ndc.ChallanDetail = model.ChallanId;
                    ndc.ChallanAmount = model.ChalanAmount;
                    ndc.LetterNo = model.LetterNo;
                    ndc.BankName = model.BankName;
                    ndc.IsTotalInstallmentPaid = model.IsTotalInstallmentPaid;
                    ndc.IsOneTimeLeasePaid = model.IsOneTimeLeasePaid;
                    ndc.NDCDate = model.NDCDate;
                    ndc.NDCTemplate = ndcletter;
                    ndc.Remarks = model.Remarks;
                    ndc.LetterType = model.ActionType;
                    ndc.ApplicantAddress = model.Address;
                    ndc.IsActive = true;
                    ndc.StatusId = NAStatusId.Approved;
                    ndc.ServiceRequestId = model.OnlineReqNo;
                    ndc.CreatedBy = userInfo.UserID;
                    ndc.CreatedDate = DateTime.Now;
                    dbContext.NDCDetailMsts.Add(ndc);
                    dbContext.SaveChanges();
                }
            }

            return ndcletter;
        }

        #region Ledger Group By ReceiptHead
        private string GetHtmlLedger(List<PaymentViewModel> paymentList, PaymentViewModel model)
        {
            string htmlStr = "";
            string mainTable = "<table cellpadding='10' cellspacing='10' style=' border:1px solid black; padding-top:10px;width:100%'>";
            string mainHeading = "<tr><th colspan=8 style='text-align:center;'>NEW OKHLA INDUSTRIAL DEVELOPMENT AUTHORITY</th></tr><tr><th colspan=8 style='text-align:center;'>Main Administrative Building Sector - VI</th></tr><tr><th colspan=8 style='text-align:center;'>NOIDA - 201301</th></tr><tr><th colspan=4 style='text-align:left;'>Registration Id: " + model.RegistrationNo + "</th><th colspan=4 style='text-align:right;'>Property No.: " + model.PropertyNo + "</th></tr>";
            string mainTableTh = "<tr style='padding:10px;border-bottom:1px solid black;border-top:1px solid black;'><th  style='text-align:center;'>Deposit Head</th><th   style='text-align:center;'>Receipt No</th><th   style='text-align:center;'>Challan No</th><th  style='text-align:center;'>Deposit Date</th><th style='text-align:center;'>Depositer</th><th style='text-align:center;'>Bank</th><th style='text-align:center;'>Sub Head</th><th style='text-align:center;'>Amount Paid</th></tr>";

            var reciptHeadIdChk = 0;
            string trDataReceipt = string.Empty;
            htmlStr = htmlStr + mainTable + mainHeading + mainTableTh;
            foreach (var item in paymentList)
            {
                string trDataHeader = string.Empty;
                if (reciptHeadIdChk != item.ReceiptHeadId)
                {
                    var lstReceipt = GetDetailReceiptByReceiptHead(model, item.ReceiptHeadId);
                    trDataReceipt = GetHtmlReceiptData(lstReceipt);
                    trDataHeader = string.Format("<tr ><td align=top style='text-align:center;'>{0}</td><td colspan=7>{1}</td></tr>", item.ReceiptHeadName, "&nbsp;");
                }

                // string trData = string.Format("<tr><td>{0}</td><td>{1}</td></tr>", Convert.ToString(item.ReceiptId), Convert.ToString(item.ChallanId));
                htmlStr = htmlStr + trDataHeader + trDataReceipt;
            }
            htmlStr = htmlStr + "</table>";
            return htmlStr;
        }
        private string GetHtmlReceiptData(List<PaymentViewModel> paymentList)
        {
            decimal? total = 0;
            string Totaltr = string.Empty;
            string mainTable = string.Empty;
            foreach (var item in paymentList)
            {
                mainTable = mainTable + string.Format("<tr><td></td><td style='text-align:center;'>{0}</td><td  style='text-align:center;'>{1}</td><td  style='text-align:center;'>{2}</td><td  style='text-align:center;'>{3}</td><td  style='text-align:center;'>{4}</td><td style='text-align:center;'>{5}</td><td style='text-align:center;'>{6}</td></tr>", item.ReceiptId == null ? string.Empty : Convert.ToString(item.ReceiptId), item.ChallanId, item.DepositDate.Value.ToShortDateString(), item.DepositorName, item.BankName, item.ReceiptSubHeadName, item.Amount);
                total = total + item.Amount;
            }
            Totaltr = Totaltr + string.Format("<tr style='border:1px solid black;'><th colspan=8 style='text-align:right;'>Total: " + total + " </th></tr>");
            return mainTable + Totaltr;//+"</table>";
        }

        public string GetGeneratedLedgerByRId(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var paymentList = (from pay in dbContext.ViewReceiptMasters
                                   where model.RegistrationNo == null ? (pay.PROPERTY_NUMBER == model.PropertyNo) : (pay.RID_NO == model.RegistrationNo)
                                   select new PaymentViewModel
                                   {
                                       ReceiptHeadId = pay.RECEIPT_HEAD_ID,
                                       ReceiptHeadName = pay.RECIEPT_HEAD_NAME
                                   }).Distinct().ToList();
                //return GetHtmlLedger(paymentList, model.RegistrationNo==null?model.PropertyNo:model.RegistrationNo);
                return GetHtmlLedger(paymentList, model);
            }
        }

        public List<PaymentViewModel> GetDetailReceiptByReceiptHead(PaymentViewModel model, int? recHeadId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var receiptDetail = (from pay in dbContext.ViewReceiptMasters
                                     //where (pay.RID_NO == rId || pay.PROPERTY_NUMBER==rId) && pay.RECEIPT_HEAD_ID == recHeadId
                                     where model.RegistrationNo == null ? (pay.PROPERTY_NUMBER == model.PropertyNo) : (pay.RID_NO == model.RegistrationNo) && pay.RECEIPT_HEAD_ID == recHeadId
                                     select new PaymentViewModel
                                     {
                                         ReceiptId = pay.RECEIPT_ID,
                                         Applicant = pay.ALLOTE_NAME,
                                         Amount = pay.AMOUNT_PAID,
                                         DepositDate = pay.DEPOSIT_DATE,
                                         ChallanId = pay.CHALLAN_ID,
                                         ReceiptSubHeadName = pay.RECEIPT_SUB_HEAD,
                                         BankName = pay.BANK_ID,
                                         //BankName = dbContext.UmUserMasters.FirstOrDefault(m => m.UserName == pay.BANK_ID).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(m => m.UserName == pay.BANK_ID).LastName,
                                         DepositorName = pay.DEPOSETER_NAME
                                     }).ToList();
                return receiptDetail;
            }
        }
        #endregion

        #region Ledger Group By ReceiptId
        public string GetGeneratedLedgerByRIdNew(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var paymentList = (from pay in dbContext.ViewReceiptMasters
                                   where model.RegistrationNo == null ? (pay.PROPERTY_NUMBER == model.PropertyNo) : (pay.RID_NO == model.RegistrationNo)
                                   select new PaymentViewModel
                                   {
                                       ReceiptId = pay.RECEIPT_ID
                                   }).Distinct().ToList();
                //return GetHtmlLedger(paymentList, model.RegistrationNo==null?model.PropertyNo:model.RegistrationNo);
                return GetHtmlLedgerNew(paymentList, model);
            }
        }
        private string GetHtmlLedgerNew(List<PaymentViewModel> paymentList, PaymentViewModel model)
        {
            var personalDetails = new PaymentViewModel();
            string htmlStr = "";
            string mainTable = "<table cellpadding='10' cellspacing='10' style=' border:1px solid black; padding-top:10px;width:100%'>";
            string mainHeading = string.Empty;
            if (model.RegistrationNo != null)
            {
                //personalDetails = GetPersonalDetailsByRid(model);
                personalDetails = GetPersonalDetails(model);
                mainHeading = "<tr><th colspan=8 style='text-align:center;'>NEW OKHLA INDUSTRIAL DEVELOPMENT AUTHORITY</th></tr><tr><th colspan=8 style='text-align:center;'>Main Administrative Building Sector - VI</th></tr><tr><th colspan=8 style='text-align:center;'>NOIDA - 201301</th></tr><tr><th colspan=4 style='text-align:left;'>Allottee: " + personalDetails.Applicant + "</th><th colspan=4 style='text-align:right;'>Registraion Id: " + model.RegistrationNo + "</th></tr><tr><th style='text-align:left;'>Property No: " + personalDetails.PropertyNumber + "</th></tr>";
            }
            else
            {
                personalDetails = GetPersonalDetails(model);
                mainHeading = "<tr><th colspan=8 style='text-align:center;'>NEW OKHLA INDUSTRIAL DEVELOPMENT AUTHORITY</th></tr><tr><th colspan=8 style='text-align:center;'>Main Administrative Building Sector - VI</th></tr><tr><th colspan=8 style='text-align:center;'>NOIDA - 201301</th></tr><tr><th colspan=4 style='text-align:left;'>Allottee: " + personalDetails.Applicant + "</th><th colspan=4 style='text-align:right;'>Property No.: " + model.PropertyNo + "</th></tr>";
            }
            string mainTableTh = "<tr style='padding:10px;border-bottom:1px solid black;border-top:1px solid black;'><th  style='text-align:center;'>Receipt No</th><th   style='text-align:center;'>Deposit Head</th><th   style='text-align:center;'>Challan No</th><th  style='text-align:center;'>Deposit Date</th><th style='text-align:center;'>Depositer</th><th style='text-align:center;'>Bank</th><th style='text-align:center;'>Sub Head</th><th style='text-align:center;'>Amount Paid</th></tr>";

            var reciptIdChk = 0;
            string trDataReceipt = string.Empty;
            htmlStr = htmlStr + mainTable + mainHeading + mainTableTh;
            foreach (var item in paymentList)
            {
                string trDataHeader = string.Empty;
                if (reciptIdChk != item.ReceiptId)
                {
                    var lstReceipt = GetDetailReceiptByReceiptHeadNew(model, item.ReceiptId);
                    trDataReceipt = GetHtmlReceiptDataNew(lstReceipt);
                    trDataHeader = string.Format("<tr ><td align=top style='text-align:center;'>{0}</td><td colspan=7>{1}</td></tr>", item.ReceiptId, "&nbsp;");
                }

                // string trData = string.Format("<tr><td>{0}</td><td>{1}</td></tr>", Convert.ToString(item.ReceiptId), Convert.ToString(item.ChallanId));
                htmlStr = htmlStr + trDataHeader + trDataReceipt;
            }
            htmlStr = htmlStr + "</table>";
            return htmlStr;
        }
        public List<PaymentViewModel> GetDetailReceiptByReceiptHeadNew(PaymentViewModel model, long? receiptId)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var receiptDetail = (from pay in dbContext.ViewReceiptMasters
                                     where model.RegistrationNo == null ? (pay.PROPERTY_NUMBER == model.PropertyNo) : (pay.RID_NO == model.RegistrationNo) && pay.RECEIPT_ID == receiptId
                                     select new PaymentViewModel
                                     {
                                         ReceiptId = pay.RECEIPT_ID,
                                         Applicant = pay.ALLOTE_NAME,
                                         Amount = pay.AMOUNT_PAID,
                                         DepositDate = pay.DEPOSIT_DATE,
                                         ChallanId = pay.CHALLAN_ID,
                                         ReceiptSubHeadName = pay.RECEIPT_SUB_HEAD,
                                         BankName = pay.BANK_ID,
                                         //BankName = dbContext.UmUserMasters.FirstOrDefault(m => m.UserName == pay.BANK_ID).FirstName + " " + dbContext.UmUserMasters.FirstOrDefault(m => m.UserName == pay.BANK_ID).LastName,
                                         DepositorName = pay.DEPOSETER_NAME,
                                         ReceiptHeadName = pay.RECIEPT_HEAD_NAME
                                     }).ToList();
                return receiptDetail;
            }
        }
        private string GetHtmlReceiptDataNew(List<PaymentViewModel> paymentList)
        {
            decimal? total = 0;
            string Totaltr = string.Empty;
            string mainTable = string.Empty;
            foreach (var item in paymentList)
            {
                mainTable = mainTable + string.Format("<tr><td></td><td style='text-align:center;'>{0}</td><td  style='text-align:center;'>{1}</td><td  style='text-align:center;'>{2}</td><td  style='text-align:center;'>{3}</td><td  style='text-align:center;'>{4}</td><td style='text-align:center;'>{5}</td><td style='text-align:center;'>{6}</td></tr>", item.ReceiptHeadName, item.ChallanId, item.DepositDate.Value.ToShortDateString(), item.DepositorName, item.BankName, item.ReceiptSubHeadName, item.Amount);
                total = total + item.Amount;
            }
            Totaltr = Totaltr + string.Format("<tr style='border:1px solid black;'><th colspan=8 style='text-align:right;'>Total: " + total + " </th></tr>");
            return mainTable + Totaltr;//+"</table>";
        }
        private PaymentViewModel GetPersonalDetails(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var personalDetails = (from pay in dbContext.ViewReceiptMasters
                                       where model.RegistrationNo == null ? (pay.PROPERTY_NUMBER == model.PropertyNo) : (pay.RID_NO == model.RegistrationNo)
                                       select new PaymentViewModel
                                       {
                                           Applicant = pay.ALLOTE_NAME,
                                           SectorName = pay.SECTOR,
                                           BlockName = pay.BLOCK,
                                           PlotNo = pay.PROP_ID,
                                           PropertyNumber = pay.PROPERTY_NUMBER
                                       });
                model.Applicant = personalDetails.FirstOrDefault().Applicant;
                model.SectorName = personalDetails.FirstOrDefault().SectorName;
                model.BlockName = personalDetails.FirstOrDefault().BlockName;
                model.PlotNo = personalDetails.FirstOrDefault().PlotNo;
                model.PropertyNumber = personalDetails.FirstOrDefault().PropertyNumber;
            }
            return model;
        }
        private PaymentViewModel GetPersonalDetailsByRid(PaymentViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                var personalDetails = (from allotment in dbContext.AllotmentMasters
                                       join application in dbContext.ApplicationDetails on allotment.rid equals application.registrationId
                                       join propertyTrans in dbContext.SchemePropTrans on allotment.propertyId equals propertyTrans.propertyId
                                       where allotment.rid.ToString() == model.RegistrationNo
                                       select new PaymentViewModel
                                       {
                                           Applicant = application.tFirstName + " " + application.tMiddleName + " " + application.tLastName,
                                           SectorName = propertyTrans.SectorMst.sectorName,
                                           BlockName = propertyTrans.BlockMst.blockName,
                                           PlotNo = propertyTrans.propertyNo,
                                           CorresspondentAddress = application.correspondanceAdd
                                       }).FirstOrDefault();
                return personalDetails;
            }
        }
        #endregion


        public ChallanViewModel GenerateChallanForPaymentById(ChallanViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                //var challanIdPK = 0;
                List<ChallanViewModel> tempDataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempChallanModel"];
                if (tempDataList != null && tempDataList.FirstOrDefault().RegistrationId == model.RegistrationId)
                {
                    decimal? total = 0;
                    Challan_Master challanMaster = new Challan_Master();
                    challanMaster.Rid = model.RegistrationId;
                    challanMaster.Department_Id = model.DepartmentId;
                    challanMaster.Sector_Id = model.ActionType == Constants.Registered ? (string.IsNullOrEmpty(model.Sector) ? Constants.NASectorId : dbContext.SectorMsts.FirstOrDefault(s => s.sectorName == model.Sector.Trim()).sectorId) : model.SectorId;
                    challanMaster.Block_Id = model.ActionType == Constants.Registered ? (string.IsNullOrEmpty(model.Block) ? Constants.NABlockId : dbContext.BlockMsts.FirstOrDefault(s => s.blockName == model.Block.Trim()).blockId) : model.BlockId;
                    challanMaster.Plot_No = model.PlotNo;
                    challanMaster.Allottee = model.Applicant;
                    challanMaster.Address = model.CorrespondAddress;
                    challanMaster.Mobile_No = model.MobileNo;
                    challanMaster.Email = model.Email;
                    challanMaster.Bank_Id = model.BankId;
                    challanMaster.Branch_Id = model.BranchId;
                    challanMaster.Account_Number = model.AccountNo;
                    challanMaster.Created_Date = DateTime.Now;
                    challanMaster.Generate_Date = DateTime.Now;
                    challanMaster.Created_By = userInfo.UserID;
                    challanMaster.Is_Active = true;
                    challanMaster.Is_Verified = false;
                    //challanMaster.ServiceRequestNo = model.ServiceRequestId;
                    //challanMaster.PAN = (model.FilterType == "PAN" && model.FilterType != null) ? model.ReferenceNo : null;
                    //challanMaster.GST_No = (model.FilterType == "GST" && model.FilterType != null) ? model.ReferenceNo : null;
                    challanMaster.PAN = (model.ReferenceType == "PAN" && model.FilterType != null) ? model.ReferenceNo : null;
                    challanMaster.GST_No = (model.ReferenceType == "GST" && model.FilterType != null) ? model.ReferenceNo : null;
                    dbContext.Challan_Master.Add(challanMaster);
                    dbContext.SaveChanges();
                    //var challanIdPK = dbContext.Challan_Master.Max(m => m.Id);
                    var challanId = challanMaster.Id;
                    model.Id = challanMaster.Id;
                    model.ChallanId = challanMaster.Challan_Id;
                    foreach (var data in tempDataList)
                    {
                        Challan_Trans trans = new Challan_Trans();
                        trans.Rid = model.RegistrationId;
                        trans.Challan_Master_Id = model.Id;
                        //trans.Challan_Master_Id = Convert.ToInt32(model.ChallanId);
                        trans.Head_Id = data.AccountHeadId;
                        trans.Subhead_Id = data.AccountSubHeadId;
                        trans.Amount = data.Amount;
                        trans.Is_Active = true;
                        trans.Created_By = userInfo.UserID;
                        trans.Created_Date = DateTime.Now;
                        dbContext.Challan_Trans.Add(trans);
                        dbContext.SaveChanges();
                        total = total + data.Amount;
                    }
                    model.TotalAmount = total;
                    challanMaster.TotalAmount = model.TotalAmount;
                    dbContext.SaveChanges();
                    string challanTemplate = string.Empty;
                    string bankCopy = string.Empty;
                    string allotteeCopy = string.Empty;
                    string authorityCopy = string.Empty;
                    //var header = GetBankChallanHeaderTemplate(challan);
                    var challanDetail = GetChallanChargesTable(model);
                    var applicantDetail = GetApplicantTable(model);
                    string content = string.Empty;

                    content = "<table style='width:100%;border-top:1px solid black;border-bottom:1px solid black;'><tr><td style='width:50%;border-right: 1px solid black;'> " + challanDetail + "</td><td style='width:50%;'>" + applicantDetail + " </td></tr></table>";
                    var footer = GetChallanFooterTemplate(model);

                    bankCopy = GetChallanHeaderTemplate(model, HeaderName.BankCopy) + content + footer;
                    authorityCopy = GetChallanHeaderTemplate(model, HeaderName.AuthorityCopy) + content + footer;
                    allotteeCopy = GetChallanHeaderTemplate(model, HeaderName.AllotteeCopy) + content + footer;

                    challanTemplate = bankCopy + authorityCopy + allotteeCopy;
                    challanMaster.Content = challanTemplate;
                    dbContext.SaveChanges();
                    model.HtmlContent = challanTemplate;
                    return model;
                }
                else
                {
                    model.ReturnType = Constants.Mismatch;
                    return model;
                }
            }
        }

        #region Employee Challan
        public string GetGeneratedChallanForEmployee(ChallanViewModel model)
        {
            using (var dbContext = new NoidaPMSEntities())
            {
                List<ChallanViewModel> tempDataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempEmpChallan"];//TempChallanModelII
                if (tempDataList != null)
                {
                    decimal? total = 0;
                    Challan_Master challanMaster = new Challan_Master();
                    challanMaster.Department_Id = model.DepartmentId;
                    challanMaster.Mobile_No = model.MobileNo;
                    challanMaster.ReceivedFrom = model.ReceivedBank;
                    challanMaster.ReceivedFromOrg = model.ReceivedOrg;
                    challanMaster.ChequeNumber = model.ChequeNumber;
                    challanMaster.ChequeDate = model.ChequeDate;
                    challanMaster.Bank_Id = model.BankId;
                    challanMaster.Branch_Id = model.BranchId;
                    challanMaster.Account_Number = model.AccountNo;
                    challanMaster.Created_Date = DateTime.Now;
                    challanMaster.Generate_Date = DateTime.Now;
                    challanMaster.Created_By = userInfo.UserID;
                    challanMaster.Is_Active = true;
                    challanMaster.Is_Verified = false;
                    dbContext.Challan_Master.Add(challanMaster);
                    dbContext.SaveChanges();
                    //var challanIdPK = dbContext.Challan_Master.Max(m => m.Id);
                    var challanId = challanMaster.Id;
                    model.Id = challanMaster.Id;
                    model.ChallanId = challanMaster.Challan_Id;
                    foreach (var data in tempDataList)
                    {
                        Challan_Trans trans = new Challan_Trans();
                        trans.Challan_Master_Id = model.Id;
                        trans.Head_Id = data.AccountHeadId;
                        trans.Subhead_Id = data.AccountSubHeadId;
                        trans.Amount = data.Amount;
                        trans.EmployeeCode = data.EmployeeCode;
                        trans.Is_Active = true;
                        trans.Created_By = userInfo.UserID;
                        trans.Created_Date = DateTime.Now;
                        dbContext.Challan_Trans.Add(trans);
                        dbContext.SaveChanges();

                        total = total + data.Amount;
                    }
                    model.TotalAmount = total;
                    challanMaster.TotalAmount = model.TotalAmount;
                    dbContext.SaveChanges();
                    string challanTemplate = string.Empty;
                    string bankCopy = string.Empty;
                    string employeeCopy = string.Empty;
                    string authorityCopy = string.Empty;
                    var challanDetail = GetChallanChargesTableForEmployee(model);//GetChallanChargesTableForEmployee(model);
                    var empDetail = GetEmployeeTable(model);
                    string content = string.Empty;

                    content = "<table style='width:100%;border-top:1px solid black;border-bottom:1px solid black;'><tr><td style='width:100%;border-right: 1px solid black;'> " + challanDetail + "</td></tr></table>"; //<td style='width:50%;'>" + empDetail + " </td>
                    var footer = GetChallanFooterTemplateForEmployee(model);

                    bankCopy = GetChallanHeaderTemplateForEmployee(model, HeaderName.BankCopy) + content + footer;
                    authorityCopy = GetChallanHeaderTemplateForEmployee(model, HeaderName.AuthorityCopy) + content + footer;
                    employeeCopy = GetChallanHeaderTemplateForEmployee(model, HeaderName.EmployeeCopy) + content + footer;

                    challanTemplate = bankCopy + authorityCopy + employeeCopy;
                    challanMaster.Content = challanTemplate;
                    dbContext.SaveChanges();
                    return challanTemplate;
                }
                else
                {
                    return Constants.Mismatch;// null;
                }
            }
        }

        private string GetChallanChargesTableForEmployeeTest(ChallanViewModel model) // For Testing
        {
            List<ChallanViewModel> tempDataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempEmpChallan"];
            string content = string.Empty;
            int count = 1;
            decimal? total = 0;
            content = "<Table style='width:100%;'><tr style=''><td class='tr-no'>SI No</td><td class='td-EmpId'>Employee Code</td><td class='td-Emp'>Employee Name</td><td class='td-head'>Account Head</td><td class='td-amount' style='text-align:right;'>Amount</td>";
            foreach (var data in tempDataList)
            {
                content = content + "<tr  style=''><td class='i-no'>" + count + "</td><td class='td-EmpId'>" + data.EmployeeCode + "</td><td class='td-Emp'>" + data.EmployeeName + "</td><td class='td-head'>" + data.AccountHead + "</td><td class='td-amount' style='text-align:right;'>" + data.Amount + "</td></tr>";
                count++;
                total = total + data.Amount;
            }
            content = content + "<tr  style=''><td class='i-no'></td><td class='i-no'></td><td class='i-no'></td><td class='td-head' style='text-align:right;'>Total: </td><td class='td-amount' style='text-align:right;'>" + total + "</td></tr>";
            content = content + "</Table>";
            //model.TotalAmount = total;
            HttpContext.Current.Session["TempEmpChallan"] = null;
            return content;
        }

        private string GetChallanChargesTableForEmployee(ChallanViewModel model) 
        {
            List<ChallanViewModel> tempDataList = (List<ChallanViewModel>)HttpContext.Current.Session["TempEmpChallan"];
            string content = string.Empty;
            int count = 1;
            decimal? total = 0;
            var emplist = tempDataList.OrderBy(c => c.EmployeeCode).ToList();
            var tempEmpid = 0;
            content = @"<Table style='width:100%;'><tr style='font-weight:800'>
                                       <td class='td-Emp'>Employee Name</td>
                                       <td class='td-no'>SI No</td>
                                       <td class='td-head'>Account Head</td>
                                       <td class='td-amount' style='text-align:right;'>Amount</td></tr>";
            foreach (var data in emplist)
            {

                if (tempEmpid != (int)data.EmployeeCode)
                {
                    /// here change your html
                    tempEmpid = (int)data.EmployeeCode;
                    content = content + "<tr  style=''><td class='td-Emp'>" + data.EmployeeName + "</td><td colspan=3>&nbsp;</td></tr>";
                    count = 1;
                }
                content = content + "<tr  style=''><td class='i-no'></td><td class='i-no'>" + count + "</td><td class='td-head'>" + data.AccountSubHead + "</td><td class='td-amount' style='text-align:right;'>" + data.Amount + "</td></tr>";
                count++;
                total = total + data.Amount;
            }
            content = content + "<tr  style=''><td class='i-no'></td><td class='i-no'></td><td class='td-head' style='text-align:right;'>Total: </td><td class='td-amount' style='text-align:right;'>" + total + "</td></tr>";
            content = content + "</Table>";
            //model.TotalAmount = total;
            HttpContext.Current.Session["TempEmpChallan"] = null;
            return content;
        }

        private string GetEmployeeTable(ChallanViewModel model)
        {
            string content = string.Empty;
            content = "<Table style='width:100%;'>";
            //content = content + "<tr  style=''><td colspan='2'>Property Location: <b> Sector-" + model.Sector + ", Block-" + model.Block + ", Plot/Flat No-" + model.PlotNo + " </b></td></tr>";
            //content = content + "<tr  style=''><td colspan='2'>Applicant: <b>" + model.Applicant + "</b></td></tr>";
            //content = content + "<tr  style=''><td colspan='2'>E-mail: <b>" + model.Email + "</b></td></tr>";
            //content = content + "<tr  style=''><td colspan='2'>Mobile No: <b>" + model.MobileNo + "</b></td></tr>";
            //content = content + "<tr  style=''><td colspan='2'>Address: <b>" + model.CorrespondAddress + "</b></td></tr>";
            content = content + "</Table>";
            return content;
        }

        private string GetChallanHeaderTemplateForEmployee(ChallanViewModel model, string HeaderName)
        {
            var today = DateTime.Now.ToString("dd/MMM/yyyy");
            string content = "<div id='authorityChallan' style='page-break-before:auto;overflow:auto'><Table style='width:100%;'><tr><td style='width:25%;'><b>" + model.BankName + "</b></td><td colspan='2'><center>Date:<b>" + today + "</b></center></td><td style='text-align:right;width:25%;'><b>" + HeaderName + "</b></td></tr>" +
            "<tr><td colspan='4'><center><h4 class='h4-noida'><b>NEW OKHLA INDUSTRIAL DEVELOPMENT AUTHORITY</b></h4></center></td></tr>" +
                //"<tr><td colspan='4'><center><h4 class='h4-dept' style='padding-top:0px;padding-bottom:0px;'><b>" + model.Department + "</b></h4></center></td></tr>" +
            "<tr><td colspan='4'><center><h4 style='padding-top:0px;'><b>GST No.: " + Constants.NoidaAuthorityGSTNo + "</b></h4></center></td></tr>" +
            "<tr><td colspan='2'>Account No.: <b>" + model.AccountNo + "</b></td><td colspan='2' style='text-align:right;'>Challan No.:<b>" + model.ChallanId + "</b></td></tr>" +
            "<tr><td colspan='2'>IFSC Code: <b>" + model.IFSCCode + "</b></td><td colspan='2'  style='text-align:right;'>Department: <b>" + model.Department + "</b></td></tr>" +
            "<tr><td colspan='2'></td><td colspan='2'  style='text-align:right;'>Phone No: <b>" + model.MobileNo + "</b></td></tr>" +
            "</Table>";
            return content;
        }

        private string GetChallanFooterTemplateForEmployee(ChallanViewModel model)
        {
            string content = "<Table><tr style='padding-top:15px;'><td colspan='3'><p>Please find enclosed herewith Draft/Pay order No./Cash <span style='font-weight:800'>" + model.ChequeNumber + "</span> Dated <span style='font-weight:800'>" + model.ChequeDate.Value.ToShortDateString() + "</span> for Rs. <span style='font-weight:800'>" + model.DepositedAmount + "</span> Drawn On <span style='font-weight:800'>" +
                        model.ReceivedBank + "</span>  against above mentioned account head the payment of property Allotted / Lease / Sublease /Rent or any charges to me by NOIDA Authority. This Cheque is received from <span style='font-weight:800'>" + model.ReceivedOrg + "</span>." + 
                    "</p></td></tr>" +
                    "<tr><td colspan='3' style='height:6px;'></td></tr>" +
                    "<tr><td><b>Authorised Signatory</b></td><td></td><td style='text-align:right;'><b>Depositor Signature</b></td></tr>" +

                    //"<tr><td colspan='3'><table style='width:100%;border:1px solid;'><tr><td colspan=9 ><center style='border-bottom:1px solid black;'><b> Details of Notes </b></center></td></tr>" +
                //"<tr class='tr-notetype' style='border-bottom:1px solid black;padding-bottom:2px'><td style='border-right:1px solid black;'><center>2000*</center></td><td style='border-right:1px solid black;'><center>500*</center></td><td style='border-right:1px solid black;'><center>200*</center></td><td style='border-right:1px solid black;'><center>100*</center></td><td style='border-right:1px solid black;'><center>50*</center></td><td  style='border-right:1px solid black;'><center>20*</center></td><td style='border-right:1px solid black;'><center>10*</center></td><td style='border-right:1px solid black;'><center>5*</center></td><td>Total</td></tr>" +
                //"<tr style='padding-top:2px;padding-bottom:2px'><td style='border-right:1px solid black;'>&nbsp;</td><td style='border-right:1px solid black;'>&nbsp;</td><td style='border-right:1px solid black;'>&nbsp;</td><td style='border-right:1px solid black;'>&nbsp;</td><td style='border-right:1px solid black;'>&nbsp;</td><td style='border-right:1px solid black;'>&nbsp;</td><td style='border-right:1px solid black;'>&nbsp;</td><td style='border-right:1px solid black;'>&nbsp;</td><td>&nbsp;</td></tr></table></td></tr>"+
                    "</Table></div>" +
                    "<tr><p style='font-size:11px;'>Note: </p>" +
                        "<p style='font-size:11px;'>(1)Payment alone will not accrue any right to allottee.</p>" +
                        "<p style='font-size:11px;'>(2)Notwithstanding any request of the allotee the payment, made by the allotee shall be first adjusted towards the interest due, if any, and the balance shall be adjusted towards the annual leaserent and the installment respectively.</p></tr>" +
                //"<p style='font-size:11px;'>(3)IFSC Code: "+model.IFSCCode+"</p>" +
                        "<p style='font-size:11px;'>(3)<b>Allottee will pay GST by Reverse Charge Mechanism against Property. Authority's GST No: 09AAALN0120A1ZV</b></p></tr><hr style='display:block; margin-left:auto;margin-right:auto;margin-top:0;margin-bottom:0;border-style=inset;border-width;1px;page-break-after:always;'><br/>";

            return content;
        }
        #endregion
    }
    public static class HeaderName
    {
        public const string BankCopy = "Bank Copy";
        public const string AuthorityCopy = "Authority Copy";
        public const string AllotteeCopy = "Allottee Copy";
        public const string EmployeeCopy = "Employee Copy";
    }
}
