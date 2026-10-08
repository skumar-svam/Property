using NA.PMS.Model;
using NA.PMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Web.Controllers;

using NA.PMS.Web.Models;
using MvcSiteMapProvider;
using NA.PMS.Common;
//using PdfSharp;
//using PdfSharp.Drawing;
//using PdfSharp.Pdf;
//using MigraDoc.DocumentObjectModel;
//using MigraDoc;
//using MigraDoc.Rendering;
//using MigraDoc.DocumentObjectModel.Tables;
//using System.Data;
//using PdfSharp.Pdf.IO;
//using System.IO;
//using System.Diagnostics;
//using TheArtOfDev.HtmlRenderer.PdfSharp;
namespace NA.PMS.Web.Areas.Master.Controllers
{
    public class CircleRateController : WebBaseController
    {
        IMastersService _MastersService;
        IGeneralService _generalService;
       //static int menuKey = Int32.Parse(SiteMaps.Current.CurrentNode.Key);
        static int menuKey = (int)Common.ScreenMenuKey.ManageCircleRate;
        // GET: Master/CircleRate
        public CircleRateController(IMastersService mastersService, IGeneralService generalService)
        {
            _MastersService = mastersService;
            _generalService = generalService;
        }
        /// <summary>
        ///Manage 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult Manage()
        {
            if (menuKey != 0)
            {
                var setPrmision = CommonMethords.SetRolePrmision(menuKey);
                //var loginUser = (CurrentUserDetail)Session["CurrentUser"];
                if (setPrmision != null)
                {
                    //foreach (var Role in loginUser.MenuMaster)
                    //{
                    //    if (Role != null && Role.MenuId == menuKey)
                    //    {
                    ViewBag.EditMenuVal = setPrmision.EditMenuVal;
                    ViewBag.AddMenuVal = setPrmision.AddMenuVal;
                    ViewBag.DeleteMenuVal = setPrmision.DeleteMenuVal;
                    ViewBag.ReadOnlyMenu = setPrmision.ReadOnlyMenu;

                    //        Constants.URL = Request.Url;
                    //    }
                    //}


                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Account", new { area = "" });
                }
            }
            else
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
        }
        /// <summary>
        ///Generate pdf base on give id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Save pdf file in given loa</returns>
        //public string GeneratePdf(int id)
        //{
        //    CircleRateModel model = new CircleRateModel();
        //    dynamic crFormPath = GetCRFormPath(id);
        //    dynamic savePath = Server.MapPath(crFormPath);
        //    //Create Directory
        //    Directory.CreateDirectory(Path.GetDirectoryName(savePath));

        //        model.departmentName = "IT (Software Deptment)";
        //        model.sectorName = "Noida sec 127";

        //         //string html = RenderViewToString(this.ControllerContext, "~/Views/PdfGenerate.cshtml", model);
        //        string html = RenderViewToString(this.ControllerContext, "~/Views/PdfTemplate/AllotmentDoc.cshtml", model);
        //        PdfDocument pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
        //        pdf.Save(savePath);
        //        Process.Start(savePath);
          
        //    return "true";
        //}
        /// <summary>
        ///Convert view to string 
        /// </summary>
        /// <param name="Controler , view path , model"></param>
        /// <returns></returns>
        //public static String RenderViewToString(ControllerContext context, String viewPath, object model = null)
        //{
        //    context.Controller.ViewData.Model = model;
        //    bool partial = false;
        //     ViewEngineResult viewResult = null;
        //  //  StringWriter sw = new StringWriter();
        //    using (var sw = new StringWriter())
        //    {
        //        if (partial)
        //            viewResult = ViewEngines.Engines.FindPartialView(context, viewPath);
        //        else
        //           viewResult = ViewEngines.Engines.FindView(context, viewPath, null);

        //        if (viewResult == null)
        //            throw new FileNotFoundException("View cannot be found.");
        //       // dynamic viewResult = ViewEngines.Engines.FindView(context, viewPath, null);
        //        dynamic viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
        //        viewResult.View.Render(viewContext, sw);
        //        viewResult.ViewEngine.ReleaseView(context, viewResult.View);
        //        return sw.GetStringBuilder().ToString();
        //    }
        //}
    


       
        /// <summary>
        ///To get all depatment
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public JsonResult GetAllDepartments()
        {
            var lst = _generalService.GetAllDepartments();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///To get all Prop Types
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public JsonResult GetPropTypes()
        {
            var lst = _generalService.GetSectorsByPropType();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        // Add Circle Rate 
        /// </summary>
        /// <param name="departmentId,sector,rate,startDate"></param>
        /// <returns></returns>
        public JsonResult btnAddCircleRate(int departmentId, int sector, decimal rate, DateTime startDate, int blockId)
        {
           int flag = 0;
           flag = _MastersService.AddCircleRate(departmentId, sector, rate, startDate, blockId);
           return Json(flag, JsonRequestBehavior.AllowGet);
        }
       
        /// <summary>
        ///Remove Circle Rate record base on refId
        /// </summary>
        /// <param name="refId"></param>
        /// <returns></returns>
        public JsonResult RemoveCircleRate(int refId)
        {
            var data = _MastersService.RemoveCircleRate (refId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
     
        /// <summary>
        ///To get all Circle Rate
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult GetCircleRate([DataSourceRequest] DataSourceRequest request)
        {
           var data = _MastersService.GetCircleRate(request);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

  
                //public string CreateCheckRequestFormPdf(int id)
                //{
                //    PdfDocument outputDocument = new PdfDocument();
                //   // CheckRequestModel checkrequestModel = objICheckRequestService.GetCheckReuqestPdfModel(id);
                //    dynamic success = false;
                //    dynamic crFormPath = GetCRFormPath(id);
                //    dynamic savePath = Server.MapPath(crFormPath);
                //    //Create Directory
                //    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                //        Document documnet = new Document();
                //    Section section = documnet.AddSection();

                //    AddPdfTitle(section, "Check Request Form", 22);

                //    Table tableHeader = section.AddTable();

                //    Column column = tableHeader.AddColumn(Unit.FromCentimeter(4));
                //    Column column1 = tableHeader.AddColumn(Unit.FromCentimeter(3));
                //    Column column2 = tableHeader.AddColumn(Unit.FromCentimeter(5));
                //    Column column3 = tableHeader.AddColumn(Unit.FromCentimeter(5));

                //    // CreateTable(tableHeader, 22)
                //    Row row11 = tableHeader.AddRow();
                //    row11.Cells[0].AddParagraph("Approved Amount : ");
                //    row11.Cells[0].Format.Alignment = ParagraphAlignment.Right;
                //    //row11.Cells(1).AddParagraph(GetFormattedDecimal(checkrequestModel.ApprovedAmount));
                //    row11.Cells[1].AddParagraph("122");
                //    row11.Cells[2].AddParagraph("Case Value :");
                //    row11.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                //    //row11.Cells(3).AddParagraph(GetFormattedDecimal(checkrequestModel.CaseValue));
                //     row11.Cells[3].AddParagraph("1111.33");
                //    Row row2 = tableHeader.AddRow();
                //    row2.Cells[0].AddParagraph("Law Firm : ");
                //    row2.Cells[0].Format.Alignment = ParagraphAlignment.Right;
                //    //row2.Cells(1).AddParagraph(checkrequestModel.LawFirm);
                //    row2.Cells[1].AddParagraph("LawFirm");
                //    row2.Cells[2].AddParagraph("Total Approved Amount :");
                //    row2.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                //    //row2.Cells(3).AddParagraph(GetFormattedDecimal(checkrequestModel.TotalApprovedAmount));
                //    row2.Cells[3].AddParagraph("10000002");

                //    Row row3 = tableHeader.AddRow();
                //    row3.Cells[0].AddParagraph("Funding Entity :");
                //    row3.Cells[0].Format.Alignment = ParagraphAlignment.Right;
                //    //row3.Cells(1).AddParagraph(checkrequestModel.Company);
                //    row3.Cells[1].AddParagraph("North Shore Technology");
                //    row3.Cells[2].AddParagraph("Attorney :");
                //    row3.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                //    //row3.Cells(3).AddParagraph(checkrequestModel.Attorney);
                //    row3.Cells[3].AddParagraph("3222.33");

                //    Row row4 = tableHeader.AddRow();
                //    row4.Cells[0].AddParagraph("Sales Person : ");
                //    row4.Cells[0].Format.Alignment = ParagraphAlignment.Right;
                //    row4.Cells[1].AddParagraph("Anuj Kumar");
                //    //row4.Cells(1).AddParagraph(checkrequestModel.SalesPerson);
                //    row4.Cells[2].AddParagraph("Sales Person Type :");
                //    row4.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                //    //row4.Cells(3).AddParagraph(checkrequestModel.SalesPersonType);
                //    row4.Cells[3].AddParagraph("Sumit Kumar");

                //    Row row5 = tableHeader.AddRow();
                //    row5.Cells[0].AddParagraph("Sales Source :");
                //    row5.Cells[0].Format.Alignment = ParagraphAlignment.Right;
                //    row5.Cells[1].AddParagraph("Google");
                //    //row5.Cells(1).AddParagraph(checkrequestModel.SalesSource);
                //    row5.Cells[2].AddParagraph("Tag :");
                //    row5.Cells[2].Format.Alignment = ParagraphAlignment.Right;
                //    //row5.Cells(3).AddParagraph(checkrequestModel.URUag);
                //      row5.Cells[3].AddParagraph("Tag module");

                //    Row row6 = tableHeader.AddRow();
                //    row6.Cells[0].AddParagraph("Plaintiff :");
                //    row6.Cells[0].Format.Alignment = ParagraphAlignment.Right;
                //    //row6.Cells(1).AddParagraph(checkrequestModel.Plantiff);
                //    row6.Cells[1].AddParagraph("Plaintiff Name");

                //    Paragraph addionalfieldRow5 = section.AddParagraph();
                //    addionalfieldRow5.AddText( " Approval Notes : " + "checkrequestModel.ApprovalNotes" );



                //  //  //Add Normal Check Table
                //  //  //if (checkrequestModel.NormalCheckList.Any()) {
                //    AddPdfTitle(section, "Normal Checks", 15);
                //    Table table = section.AddTable();
                //    string[] headerList = {
                //            "Name",
                //            "Amount",
                //            "Payable",
                //            "Check Date",
                //            "Cut"
                //       };
                //     CreateTableHeader(headerList, table);
                //  //      int i = 0;
                //  //      //foreach (void check_loopVariable in checkrequestModel.NormalCheckList) {
                //  //      //    check = check_loopVariable;
                //  //      //    Row row1 = table.AddRow();
                //  //      //    row1.Cells(0).AddParagraph(check.Name);
                //  //      //    row1.Cells(1).AddParagraph(GetFormattedDecimal(check.Amount));
                //  //      //    row1.Cells(2).AddParagraph(check.PayableTo);
                //  //      //    row1.Cells(3).AddParagraph(GetFormattedDate(check.CheckDate));
                //  //      //    row1.Cells(4).AddParagraph(check.IsCut ? "Yes" : "No");
                //  //      //    //row1.HeadingFormat = True
                //  //      //    row1.Format.Alignment = ParagraphAlignment.Left;
                //  //      //}
                //  //      Paragraph addionalRow = section.AddParagraph();
                //  //      addionalRow.AddText(Constants.vbLf);
                //  //  }

                //  //  //  Add Additonal Check Table
                //  //  //if (checkrequestModel.AdditonalCheckList.Any()) {
                //     AddPdfTitle(section, "Additional Checks", 15);
                //     Table table1 = section.AddTable();
                //     string[] headerList2 = {
                //            "Type",
                //            "Amount",
                //            "Payable",
                //            "Check Date",
                //            "Cut"
                //        };
                //     CreateTableHeader(headerList2, table1);
                //  //      //foreach (void check_loopVariable in checkrequestModel.AdditonalCheckList) {
                //  //      //    check = check_loopVariable;
                //  //      //    Row row1 = table.AddRow();
                //  //      //    row1.Cells(0).AddParagraph(check.OperatingCheckType);
                //  //      //    row1.Cells(1).AddParagraph(GetFormattedDecimal(check.Amount));
                //  //      //    row1.Cells(2).AddParagraph(check.PayableTo);
                //  //      //    row1.Cells(3).AddParagraph(GetFormattedDate(check.CheckDate));
                //  //      //    row1.Cells(4).AddParagraph(check.IsCut ? "Yes" : "No");
                //  //      //    //row1.HeadingFormat = True
                //  //      //    row1.Format.Alignment = ParagraphAlignment.Left;
                //  //      //}
                //  //      Paragraph addionalRow = section.AddParagraph();
                //  //      addionalRow.AddText(Constants.vbLf);
                //  ////  }

                //  //  //Add Obligor TABLE
                //  ////  if (checkrequestModel.ObligorList.Any()) {
                //     AddPdfTitle(section, "Obligors", 15);
                //     Table table2 = section.AddTable();
                //     string[] headerList3 = {
                //            "Primary",
                //            "Obligor",
                //            "Insurance Type",
                //            "Min Limit",
                //            "Max Limit"
                //        };
                //        CreateTableHeader(headerList3, table2);
                //  //      //foreach (void check_loopVariable in checkrequestModel.ObligorList) {
                //  //      //    check = check_loopVariable;
                //  //      //    Row ObligorRow = table.AddRow();
                //  //      //    ObligorRow.Cells(0).AddParagraph(check.IsPrimary ? "Yes" : "No");
                //  //      //    ObligorRow.Cells(1).AddParagraph(check.InsuranceCarrier);
                //  //      //    ObligorRow.Cells(2).AddParagraph(check.InsuranceType);
                //  //      //    ObligorRow.Cells(3).AddParagraph(check.MinLimit.ToString("C"));
                //  //      //    ObligorRow.Cells(4).AddParagraph(check.MaxLimit.ToString("C"));
                //  //      //}
                //  //      Paragraph addionalRow = section.AddParagraph();
                //  //      addionalRow.AddText(Constants.vbLf);
                //  // // }

                //  //  //Add Rate Spec table
                //  //  //if (checkrequestModel.RateSpecList.Any()) {
                //     AddPdfTitle(section, "Rate Specs", 15);
                //     Table table3 = section.AddTable();
                //     string[] headerList4 = {
                //            "Type",
                //            "Rate %",
                //            "Min Term",
                //            "Thereafter",
                //            "Term Length"
                //        };
                //     CreateTableHeader(headerList4, table3);

                //  //      //foreach (void rateSpec_loopVariable in checkrequestModel.RateSpecList) {
                //  //      //    rateSpec = rateSpec_loopVariable;
                //  //      //    Row ObligorRow = table.AddRow();
                //  //      //    ObligorRow.Cells(0).AddParagraph(rateSpec.TypeName);
                //  //      //    ObligorRow.Cells(1).AddParagraph(rateSpec.Rate.ToString());
                //  //      //    ObligorRow.Cells(2).AddParagraph(rateSpec.MinTerm.ToString());
                //  //      //    ObligorRow.Cells(3).AddParagraph(rateSpec.ThereAfter.ToString());
                //  //      //    ObligorRow.Cells(4).AddParagraph(rateSpec.TermLength.ToString());

                //  //      //}
                //  // // }


                //    PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
                //    renderer.Document = documnet;
                //    renderer.RenderDocument();

                //    renderer.PdfDocument.Save(savePath);
                //    //'..and start a viewer.
                //    Process.Start(savePath);
                //    return "success";

                //}
                public static string GetCRFormPath(int id)
                {
                    dynamic tempPath = string.Empty;
                    tempPath = "~\\Documents\\CheckRequestForm_" + id + ".pdf";
                    return tempPath;
                }
                //private void CreateTableHeader(string[] headerList, Table table)
                //{
                //    Column column = default(Column);
                //    int i = 0;
                //    table.Style = "Table";
                //    table.Borders.Width = 0.25;
                //    table.Borders.Left.Width = 0.5;
                //    table.Borders.Right.Width = 0.5;
                //    table.Rows.LeftIndent = 0;
                //    foreach (var header_loopVariable in headerList)
                //    {
                //         column = table.AddColumn(Unit.FromCentimeter(3));
                //        column.Format.Alignment = ParagraphAlignment.Center;
                //    }
                //    Row row = table.AddRow();
                //    foreach (var header in headerList)
                //    {
                //         row.Cells[i].AddParagraph(header);
                //        i = i + 1;
                //    }

                //}
                //private void CreateTable(Table table, int count)
                //{
                //    int i = 0;
                //    Column column = default(Column);
                //    for (i = 0; i <= count; i++)
                //    {
                //        column = table.AddColumn(Unit.FromCentimeter(4));
                //        column.Format.Alignment = ParagraphAlignment.Center;
                //    }

                //}
                //private void AddPdfTitle(Section document, string title, int fontSize)
                //{
                //    Paragraph para = document.AddParagraph();
                //    para.Format.Alignment = ParagraphAlignment.Center;
                //    para.Format.Font.Name = "Arial";
                //    para.Format.Font.Size = fontSize;
                //    para.AddFormattedText(title);
                //}


    }
}