using NA.PMS.Model;
using NA.PMS.Service;
using NA.PMS.Service.BusinessRuleEngine;
using NA.PMS.Service.Property;
using NA.PMS.Service.Reports;
using NA.PMS.Service.TemplateParser;
using NA.PMS.Web.Models.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NA.PMS.Web.Controllers
{
    public class DocParserController : WebBaseController
    {
        private ITemplateParserService _templateParserService;
        private IPropertyRegistrationService _propertyRegistrationService;
        private IPossessionService _possessionService;
        private IAllotmentService _allotmentService;
        private IPaymentEngine _paymentEngine;
        private ICompletionService _completionService;
        private ICitizenRequestsService _citizenService;
        private IReportService _reportservice;
        public DocParserController(ITemplateParserService templateParserService, IPropertyRegistrationService propertyRegistrationService, IPossessionService possessionService, IAllotmentService allotmentService, IPaymentEngine paymentEngine, ICompletionService completionService, ICitizenRequestsService citizenService, IReportService reportservice)
        {
            _templateParserService = templateParserService;
            _propertyRegistrationService = propertyRegistrationService;
            _possessionService = possessionService;
            _allotmentService = allotmentService;
            _paymentEngine = paymentEngine;
            _completionService = completionService;
            _citizenService = citizenService;
            _reportservice = reportservice;
        }

        //public string Index()
        //{
        //    //ITemplateParserService _templateParserService = new TemplateParserService();
        //    string parsedHTML = _templateParserService.GetParsedHTML(GetModelData(), "AllotmentDoc.cshtml");
        //    //need to convert parsed html to pdf, also here we can save this file for tracking if needed

        //    return parsedHTML;
        //}

        //Prints Mutation Letter
        public ActionResult PrintMutationLetter(int id)
        {
            string parsedHTML = string.Empty;
            var mutationData = _propertyRegistrationService.GetMutationDetailsByRId(id);
            if (mutationData != null)
                parsedHTML = _templateParserService.GetParsedHTML(mutationData, "MutationLetter.cshtml");
            return Json(parsedHTML, JsonRequestBehavior.AllowGet);
        }

        // Method for Print Possession Order Letter
        public ActionResult PrintPossessionOrder(int id)
        {
            string parsedHtml = _templateParserService.GetParsedHTML(_possessionService.GetPossessionDetailsByRID(id), "PossessionOrder.cshtml");
            //return parsedHtml;
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }

        // Method for Print Possession Order Letter
        public ActionResult PrintBulkPossessionOrder(List<int> rIds)
        {
            string parsedHtml = String.Empty;
            var listModels = _possessionService.GetModelToPrintPossessionOrder(rIds);
            foreach (var model in listModels)
            {
                parsedHtml += _templateParserService.GetParsedHTML(model, "PossessionOrder.cshtml");
                parsedHtml += "<div style='page-break-after: always;' ></div>";
            }
            //return parsedHtml;
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }

        //Prints Allotment Challan
        public ActionResult PrintAllotmentChallan(int propId, int schemeId, int deptId)
        {
            string parsedHtml = String.Empty;
            var challanModel = _allotmentService.PrintViewAllotment(propId, schemeId, deptId);
            if (challanModel != null)
                parsedHtml = _templateParserService.GetParsedHTML(challanModel, "ChallanPrint.cshtml");
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }

        //Prints Allotment Challans in bulk
        public ActionResult PrintBulkAllotmentChallans(List<int> propIds, int schemeId, int deptId)
        {
            string parsedHtml = String.Empty;
            var allotmentDetails = _allotmentService.GetAllotmentDetailsForBulkPrint(propIds, schemeId, deptId);
            foreach (var model in allotmentDetails)
            {
                parsedHtml += _templateParserService.GetParsedHTML(model, "ChallanPrint.cshtml");
                //parsedHtml += _templateParserService.GetParsedHTML(model, "AllotmentLetter.cshtml");
                parsedHtml += "<div style='page-break-after: always;' ></div>";
            }
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }

        //Pring Allotment Letters in bulk
        public ActionResult PrintBulkAllotmentLetter(List<int> rIds)
        {
            string parsedHtml = String.Empty;
            var allotmentDetails = _allotmentService.GetAllotmentDetailsForBulkLetterPrint(rIds);
            foreach (var model in allotmentDetails)
            {
                parsedHtml += _templateParserService.GetParsedHTML(model, "AllotmentLetter.cshtml");
                parsedHtml += "<div style='page-break-after: always;' ></div>";
            }
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }

        //Method for printing Lease Rent Challan
        public ActionResult PrintLeaseDeedChallan(int rId, int deptId, int type, int propId, int schemeId)
        {
            string parsedHtml = String.Empty;
            var challanModel = _allotmentService.PrintViewAllotment(propId, schemeId, deptId);
            //if (challanModel != null)
            //parsedHtml = _templateParserService.GetParsedHTML(challanModel, "ChallanPrint.cshtml");
            var challan = _paymentEngine.GetLeaseRentValue(rId, deptId, type);
            if (challanModel != null && challan != null)
            {
                challanModel.LeaseRentAmount = challan.LeaseRentAmount;
                challanModel.Dues = challan.Dues;
                challanModel.TotalDue = challan.TotalDue;
                //challanModel.TotalDue = Convert.ToInt32(challanModel.TotalDue);
                challanModel.TotalDueInWords = NumbersToWords(Convert.ToInt32(challanModel.TotalDue));
                parsedHtml = _templateParserService.GetParsedHTML(challanModel, "LeaseDeedChallan.cshtml");
            }
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }

        // Method for Print Rent Permission Request Challan
        public ActionResult PrintRentPermissionRequestChallan(int rid, int requestNo)
        {
            string parsedHTML = string.Empty;
            var requestModel = _propertyRegistrationService.GetChallanDetailsForRentPermission(rid, requestNo);
            //var requestModel = _propertyRegistrationService.GetRentRequestByRequestNumber(requestNo);
            if (requestModel != null)
            {
                parsedHTML = _templateParserService.GetParsedHTML(requestModel, "ChallanTemplate.cshtml");
            }
            return Json(parsedHTML, JsonRequestBehavior.AllowGet);
        }

        // Method for Print Rent Permission Letter
        public ActionResult PrintRentPermissionLetter(int requestNo, int rid)
        {
            //string parsedHtml = _templateParserService.GetParsedHTML(_propertyRegistrationService.GetRentRequestByRequestNumber(id), "RentPermissionLetter.cshtml");
            string parsedHtml = _templateParserService.GetParsedHTML(_propertyRegistrationService.GetModelToPrintRentPermissionLetter(requestNo, rid), "RentPermissionLetter.cshtml");
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }

        // Method for Print Rent Permission Order Letter in bulk
        public ActionResult PrintBulkRentPermissionLetter(List<int> requestNoList, List<int> ridList)
        {
            string parsedHtml = String.Empty;
            //var listModels = _possessionService.GetModelToPrintPossessionOrder(rIds);
            var listModels = _propertyRegistrationService.GetModelListToPrintRentPermissionLetter(requestNoList, ridList);
            foreach (var model in listModels)
            {
                parsedHtml += _templateParserService.GetParsedHTML(model, "RentPermissionLetter.cshtml");
                parsedHtml += "<div style='page-break-after: always;' ></div>";
            }
            //return parsedHtml;
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintBulkRentPermissionChallan(List<int> requestNoList, List<int> ridList)
        {
            string parsedHtml = String.Empty;
            //var listModels = _propertyRegistrationService.GetModelListToPrintRentPermissionLetter(requestNoList, ridList);
            var listModels = _propertyRegistrationService.GetModelListToPrintRentPermissionChallan(requestNoList, ridList);
            foreach (var model in listModels)
            {
                parsedHtml += _templateParserService.GetParsedHTML(model, "ChallanTemplate.cshtml");
                parsedHtml += "<div style='page-break-after: always;' ></div>";
            }
            //return parsedHtml;
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintDrawSlipForAuthority(int schemeId, int departmentId, List<string> nameList)
        {
            string parsedHTML = string.Empty;
            var requestModel = _propertyRegistrationService.GetApplicationFormDetailsForDrawSlip(schemeId, departmentId, nameList);
            if (requestModel.Count != 0)
            {
                string parsedHtmlLeft = string.Empty;
                string parsedHtmlRight = string.Empty;
                parsedHtmlLeft += "<div style='float: left;width:48%' >";
                parsedHtmlRight += "<div style='float: right;width:48%' >";
                int count = 1;
                foreach (var model in requestModel)
                {
                    if (count % 2 == 0)
                    {
                        parsedHtmlRight += _templateParserService.GetParsedHTML(model, "DrawSlipTemplate.cshtml") + "<br>";
                        count++;
                    }
                    else
                    {
                        parsedHtmlLeft += _templateParserService.GetParsedHTML(model, "DrawSlipTemplate.cshtml") + "<br>";
                        count++;
                    }

                    //parsedHTML += "<div style='page-break-after: always;' ></div>";
                }
                parsedHtmlLeft += "</div>"; parsedHtmlRight += "</div>";
                //return Json(parsedHTML, JsonRequestBehavior.AllowGet);
                return Json(parsedHtmlLeft + parsedHtmlRight, JsonRequestBehavior.AllowGet);
            }
            else
            {
                parsedHTML = requestModel.Count.ToString();
                return Json(parsedHTML, JsonRequestBehavior.AllowGet);
            }
        }

        // Method for Print Functional Charge  Challan
        public ActionResult PrintFunctionalChargeChallan(int rid, int requestNo)
        {
            string parsedHTML = string.Empty;
            var requestModel = _propertyRegistrationService.GetChallanDetailsForFunctionalPermission(rid, requestNo);
            //var requestModel = _propertyRegistrationService.GetRentRequestByRequestNumber(requestNo);
            if (requestModel != null)
            {
                parsedHTML = _templateParserService.GetParsedHTML(requestModel, "ChallanTemplate.cshtml");
            }
            return Json(parsedHTML, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintBulkFunctionalChargeChallan(List<int> requestNoList, List<int> ridList)
        {
            string parsedHtml = String.Empty;
            //var listModels = _propertyRegistrationService.GetModelListToPrintRentPermissionLetter(requestNoList, ridList);
            var listModels = _propertyRegistrationService.GetModelListToPrintFunctionalChallan(requestNoList, ridList);
            foreach (var model in listModels)
            {
                parsedHtml += _templateParserService.GetParsedHTML(model, "ChallanTemplate.cshtml");
                parsedHtml += "<div style='page-break-after: always;' ></div>";
            }
            //return parsedHtml;
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }
        //private Commericial GetModelData()
        //{
        //    Commericial obj = new Commericial();
        //    obj.AllotmentMoney = "12222";
        //    obj.AreaOfPlot = "344";
        //    obj.BalanceAmount = "3333";
        //    obj.DateOfAllotment = System.DateTime.UtcNow.ToString();
        //    obj.EarnestMoney = "566";
        //    obj.FirstName = "P";
        //    obj.LastName = "Singh";
        //    obj.RateofInterest = "45";
        //    return obj;
        //}

        //Private function which converts numbers to words
        private static string NumbersToWords(int inputNumber)
        {
            int inputNo = inputNumber;

            if (inputNo == 0)
                return "Zero";

            int[] numbers = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (inputNo < 0)
            {
                sb.Append("Minus ");
                inputNo = -inputNo;
            }

            string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ",
            "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
            string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ",
            "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
            string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ",
            "Seventy ","Eighty ", "Ninety "};
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };

            numbers[0] = inputNo % 1000; // units
            numbers[1] = inputNo / 1000;
            numbers[2] = inputNo / 100000;
            numbers[1] = numbers[1] - 100 * numbers[2]; // thousands
            numbers[3] = inputNo / 10000000; // crores
            numbers[2] = numbers[2] - 100 * numbers[3]; // lakhs

            for (int i = 3; i > 0; i--)
            {
                if (numbers[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (numbers[i] == 0) continue;
                u = numbers[i] % 10; // ones
                t = numbers[i] / 10;
                h = numbers[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }

        //Prints Completion Challan
        public ActionResult PrintCompletionChallan(int propId, int schemeId, int deptId, int rId)
        {
            string parsedHtml = String.Empty;
            var challanModel = _allotmentService.PrintViewAllotment(propId, schemeId, deptId, rId);
            if (challanModel != null)
                parsedHtml = _templateParserService.GetParsedHTML(challanModel, "ChallanPrint.cshtml");
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }

        // Method for Print Completion request Letter
        public ActionResult PrintCompletionRequest(int rId)
        {
            string parsedHtml = String.Empty;
            var completionRequestModel = _completionService.GetModelToPrintCompletionRequest(rId);
            parsedHtml = _templateParserService.GetParsedHTML(completionRequestModel, "CompletionRequestLetter.cshtml");
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintBulkExtensionChallan(List<int> requestNoList, List<int> ridList)
        {
            string parsedHtml = String.Empty;
            var listModels = _propertyRegistrationService.GetModelListToPrintExtensionChallan(requestNoList, ridList);
            foreach (var model in listModels)
            {
                parsedHtml += _templateParserService.GetParsedHTML(model, "ChallanTemplate.cshtml");
                parsedHtml += "<div style='page-break-after: always;' ></div>";
            }
            //return parsedHtml;
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PrintCICChallan(int ridList, int directorID)
        {
            string parsedHtml = String.Empty;
            var listModels = _propertyRegistrationService.GetModelListToPrintCICChallan(ridList, directorID);
            foreach (var model in listModels)
            {
                parsedHtml += _templateParserService.GetParsedHTML(model, "ChallanTemplate.cshtml");
                parsedHtml += "<div style='page-break-after: always;' ></div>";
            }
            //return parsedHtml;
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintCICFirmnProductChallan(int ridList, int directorID)
        {
            string parsedHtml = String.Empty;
            var listModels = _propertyRegistrationService.GetModelListToPrintCICFirmnProductChallan(ridList, directorID);
            foreach (var model in listModels)
            {
                parsedHtml += _templateParserService.GetParsedHTML(model, "ChallanTemplate.cshtml");
                parsedHtml += "<div style='page-break-after: always;' ></div>";
            }
            //return parsedHtml;
            return Json(parsedHtml, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateChallan(int rId, int bankId, int branchId, string ddlAccountNumber, int? DepttId)
        {
            string parsedHTML = string.Empty;
            ChallanModel requestModel = _propertyRegistrationService.GenerateChallan(rId, bankId, branchId, ddlAccountNumber, DepttId);
            if (requestModel != null)
            {

                parsedHTML = _templateParserService.GetParsedHTML(requestModel, "PaymentChallanTemplate.cshtml");
                string challan = _propertyRegistrationService.SaveGeneratedChallanByRid(rId, bankId, branchId, ddlAccountNumber, parsedHTML);
            }
            return Json(parsedHTML, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GeneratePaymentChallan(int rId, int bankId, int branchId, string DdlAccountNumber, int? deptId)
        {
            string challan = string.Empty;
            ChallanModel requestModel = _propertyRegistrationService.GeneratePaymentChallan(rId, bankId, branchId, DdlAccountNumber, deptId);
            if (requestModel != null)
            {
                challan = _templateParserService.GetParsedHTML(requestModel, "PaymentChallanTemplate.cshtml");
                //bool flag = _propertyRegistrationService.SaveGeneratedChallan(rId, bankId, branchId, DdlAccountNumber, DepttId, challan);
                bool flag = _propertyRegistrationService.SaveGeneratedChallan(requestModel.ChallanId, challan);
            }
            return Json(challan, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateChallanByServices(int rId, int bankId, int branchId, string DdlAccountNumber, int? deptId, int? serviceId)
        {
            string challan = string.Empty;
            //ChallanModel requestModel = _propertyRegistrationService.GeneratePaymentChallan(rId, bankId, branchId, DdlAccountNumber, deptId);
            ChallanModel requestModel = _citizenService.GenerateChallanByServices(rId, bankId, branchId, DdlAccountNumber, deptId, serviceId);
            if (requestModel != null)
            {
                challan = _templateParserService.GetParsedHTML(requestModel, "ServicePaymentChallanTemplate.cshtml");
                //bool flag = _propertyRegistrationService.SaveGeneratedChallan(requestModel.ChallanId, challan);
                bool flag = _propertyRegistrationService.SaveGeneratedChallanForServiceRequest(requestModel.ChallanId, challan);
            }
            return Json(challan, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintServiceReport(int deptId, DateTime startDate, DateTime enddate)
        {
            string parsedHTML = string.Empty;
            var servicereportdepartmentwise = new ServiceReportDepartmentWiseVM();
            var servicereportdepartmentLst = new List<ServiceReportDepartmentWiseVM>();
            servicereportdepartmentwise.DepartmentId = deptId;
            servicereportdepartmentwise.StartDate = startDate;
            servicereportdepartmentwise.EndDate = enddate;

            var requestModel = _reportservice.GetServiceReportDepartmentWise(servicereportdepartmentwise);
            if (requestModel != null)
            {
                servicereportdepartmentwise.DepartmentName = requestModel.FirstOrDefault().DepartmentName.ToUpper();
                servicereportdepartmentwise.ServiceList = new List<ServiceReportDepartmentWiseVM>();
                servicereportdepartmentwise.ServiceList.AddRange(requestModel);
                parsedHTML = _templateParserService.GetParsedHTML(servicereportdepartmentwise, "ServiceReportTemplate.cshtml");
            }
            return Json(parsedHTML, JsonRequestBehavior.AllowGet);
        }
    }
}