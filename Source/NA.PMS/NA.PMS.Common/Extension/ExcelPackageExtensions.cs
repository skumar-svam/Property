using NA.PMS.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NA.PMS.Common
{
    public static class ExcelPackageExtensions
    {
        // return in DataTable format from excel data
        public static DataTable ToDataTable(this ExcelPackage package)
        {
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            DataTable table = new DataTable();
            foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
            {
                table.Columns.Add(firstRowCell.Text);
            }
            for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
            {
                var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                var newRow = table.NewRow();
                foreach (var cell in row)
                {
                    newRow[cell.Start.Column - 1] = cell.Text;
                }
                table.Rows.Add(newRow);
            }
            return table;
        }

        //convert uploaded excel data in list type of PropertyApplicationForm model
        public static List<PropertyApplicationForm> ToApplicationModel(this ExcelPackage package)
        {
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            var len = workSheet.Dimension.End.Row;
            var lenColumn = workSheet.Dimension.End.Column;
            List<PropertyApplicationForm> modelList = new List<PropertyApplicationForm>();
            for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
            {
                PropertyApplicationForm model = new PropertyApplicationForm();
                if (workSheet.Cells[rowNumber, 5].Text.ToLower() == Constants.Company)
                {
                    model.FormNo = workSheet.Cells[rowNumber, 1].Text;
                    model.FirstName = workSheet.Cells[rowNumber, 2].Text;                   
                    model.Gender = workSheet.Cells[rowNumber, 5].Text;                  
                    model.SigningAuthority = workSheet.Cells[rowNumber, 10].Text;
                    model.RegisteredOffice = workSheet.Cells[rowNumber, 11].Text;
                    model.CorrespondanceAdd = workSheet.Cells[rowNumber, 12].Text;
                    model.PermanentAdd = workSheet.Cells[rowNumber, 13].Text;
                    model.MobileNumberP2 = workSheet.Cells[rowNumber, 14].Text;
                    model.PhoneNumberP2 = workSheet.Cells[rowNumber, 15].Text;
                    model.FaxNumberP2 = workSheet.Cells[rowNumber, 16].Text;
                    model.Email = workSheet.Cells[rowNumber, 17].Text;
                }
                else
                {
                    model.FormNo = workSheet.Cells[rowNumber, 1].Text;
                    model.FirstName = workSheet.Cells[rowNumber, 2].Text;
                    model.MiddleName = workSheet.Cells[rowNumber, 3].Text;
                    model.LastName = workSheet.Cells[rowNumber, 4].Text;
                    model.Gender = workSheet.Cells[rowNumber, 5].Text;
                    model.MarritalStatus = workSheet.Cells[rowNumber, 6].Text;
                    model.FatherHusbandName = workSheet.Cells[rowNumber, 7].Text;
                    model.MotherName = workSheet.Cells[rowNumber, 8].Text;
                    model.DateOfBirth = DateTime.Parse(workSheet.Cells[rowNumber, 9].Text);
                    model.CorrespondanceAdd = workSheet.Cells[rowNumber, 12].Text;
                    model.PermanentAdd = workSheet.Cells[rowNumber, 13].Text;
                    model.MobileNumberP2 = workSheet.Cells[rowNumber, 14].Text;
                    model.PhoneNumberP2 = workSheet.Cells[rowNumber, 15].Text;
                    model.Email = workSheet.Cells[rowNumber, 17].Text;
                    model.OccupationId = int.Parse(workSheet.Cells[rowNumber, 18].Text);
                    model.QuotaId = int.Parse(workSheet.Cells[rowNumber, 19].Text);
                    model.ReligionId = int.Parse(workSheet.Cells[rowNumber, 20].Text);                    
                }
                model.PanNumber = workSheet.Cells[rowNumber, 21].Text;
                model.AnnualIncome = decimal.Parse(workSheet.Cells[rowNumber, 22].Text);
                model.PaymentMode = workSheet.Cells[rowNumber, 23].Text.ToUpper();
                model.BankId = int.Parse(workSheet.Cells[rowNumber, 24].Text);
                model.BranchId = int.Parse(workSheet.Cells[rowNumber, 25].Text);
                model.AmountDeposited = decimal.Parse(workSheet.Cells[rowNumber, 26].Text);
                model.DemandDraftNo = workSheet.Cells[rowNumber, 27].Text;
                model.Utn = workSheet.Cells[rowNumber, 28].Text;
                model.DemandDraftIssueBank = workSheet.Cells[rowNumber, 29].Text;
                model.DemandDraftIssueDate = DateTime.Parse(workSheet.Cells[rowNumber, 30].Text);
                
                modelList.Add(model);
            }
            return modelList;
        }

        //validate excel data header name with PropertyApplicationFormEnum for its model
        public static string ValidateExcelHeaderName(this ExcelPackage package)
        {
            string flag = null;
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            var excelColumnCount = workSheet.Dimension.End.Column;
            string[] excelColumns = new string[excelColumnCount];

            for (int i = 1; i <= excelColumnCount; i++)
            {
                excelColumns[i - 1] = workSheet.Cells[1, i].Text;
            }
            string[] headerNames = Enum.GetNames(typeof(PropertyApplicationFormEnum));
            for (int count = 0; count < excelColumnCount; count++)
            {
                if (headerNames[count].ToLower() == excelColumns[count].ToLower())
                {
                    flag = null;
                }
                else
                {
                    return flag = excelColumns[count].ToString();
                } 
            }
            return flag;
        }

        public static string ValidateExcelHeaderNameForApplicationForm(this ExcelPackage package)
        {
            string message = string.Empty;
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            var excelColumnCount = workSheet.Dimension.End.Column;
            string[] excelColumns = new string[excelColumnCount];

            for (int i = 1; i <= excelColumnCount; i++)
            {
                excelColumns[i - 1] = workSheet.Cells[1, i].Text.Trim();
            }
            //string[] enumNames = Enum.GetNames(typeof(PropertyApplicationFormEnum));
            string[] headerName = NewExcelApplicationForm.ApplicationFormHeader;
            for (int x = 0; x < excelColumnCount; x++)
            {
                if (headerName[x].ToLower().Trim() == excelColumns[x].ToLower().Trim())
                {
                    //return message;
                }
                else
                {
                    message = "Error in header name: "+ excelColumns[x];
                }
            }
            return message;
        }

        //validate excel data header name with PropertyApplicationFormEnum for its model
        public static bool ValidateExcelHeaderNameForAllotment(this ExcelPackage package)
        {
            var flag = false;
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            var excelColumnCount = workSheet.Dimension.End.Column;
            string[] excelColumns = new string[excelColumnCount];
            string[] header = Constants.AllotmentHeader;
            for (int i = 1; i <= excelColumnCount; i++)
            {
                excelColumns[i - 1] = workSheet.Cells[1, i].Text;
            }
            //string[] enumNames = Enum.GetNames(typeof(AllotmentEnum));
            for (int x = 0; x < excelColumnCount; x++)
            {
                //if (enumNames[x].ToLower() == excelColumns[x].ToLower())
                if (header[x].ToLower().Trim() == excelColumns[x].ToLower().Trim())
                {
                    flag = true;
                }
                else
                {
                    return flag = false;
                }
            }
            return flag;
        }
        //validate excel data for allotment

        public static bool ValidateExcelDataFieldForAllotment(this ExcelPackage package)
        {
            var flag = false;
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            var excelColumnCount = workSheet.Dimension.End.Column;
            var excelRowCount = workSheet.Dimension.End.Row;
            for (int i = 1; i <= excelRowCount; i++)
            {
                for (int j = 1; j <= excelColumnCount; j++)
                {
                    if (workSheet.Cells[i, j].Text != null)
                    {
                        flag = true;
                    }
                }
            }
            DateTime Test = DateTime.Now;
            int n = 0;
            Regex exp = new Regex("^[a-zA-Z0-9]*$");
            Regex nameExp = new Regex("^[a-zA-Z0-9\\-\\s]+$");
            for (int i = 2; i <= excelRowCount; i++)
            {
                if (!int.TryParse(workSheet.Cells[i, 1].Text, out n))
                {
                    flag = false;
                    return flag;
                }
                if (!int.TryParse(workSheet.Cells[i, 2].Text, out n))
                {
                    flag = false;
                    return flag;
                }
                if (!DateTime.TryParse(workSheet.Cells[i, 3].Text, out Test))
                {
                    flag = false;
                    return flag;
                }
                if (!DateTime.TryParse(workSheet.Cells[i, 4].Text, out Test))
                {
                    flag = false;
                    return flag;
                }
                
            }
            return flag;
        }

        public static List<AllotmentModel> ToAllotmentModelList(this ExcelPackage package)
        {
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            var len = workSheet.Dimension.End.Row;
            var lenColumn = workSheet.Dimension.End.Column;
            List<AllotmentModel> modelList = new List<AllotmentModel>();
            for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
            {
                AllotmentModel model = new AllotmentModel();
                model.FormNo = (workSheet.Cells[rowNumber, 1].Text);
                model.PropertyId = int.Parse(workSheet.Cells[rowNumber, 2].Text);
                model.AllotmentDate = DateTime.Parse(workSheet.Cells[rowNumber, 3].Text);
                model.InstalmentStartDate = DateTime.Parse(workSheet.Cells[rowNumber, 4].Text);
                modelList.Add(model);
            }
            return modelList;
        }

        //validate excel field data during upload for PropertyApplicationForm
        public static string ValidateExcelDataField(this ExcelPackage package)
        {
            string flag = null;
            DateTime Test = DateTime.Now;
            int n = 0;
            decimal decnum = 1.05M;
            Regex exp = new Regex("^[a-zA-Z0-9]*$");
            Regex nameExp = new Regex("^[a-zA-Z0-9\\-\\.\\s]+$");
            Regex emailExp = new Regex("^([a-zA-Z0-9_\\-\\.]+)@(([a-zA-Z0-9\\-]+\\.)+)([a-zA-Z0-9]{2,4})$");
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            var excelColumnCount = workSheet.Dimension.End.Column;
            var excelRowCount = workSheet.Dimension.End.Row;
            
            //string[] headerNames = Enum.GetNames(typeof(PropertyApplicationFormEnum));
            string[] headerNames = NewExcelApplicationForm.ApplicationFormHeader;
            for (int i = 2; i <= excelRowCount; i++)
            {
                string message = null; int blankRowCounter = 0;
                
                if (!workSheet.Cells[i, 1].Text.Trim().All(char.IsLetterOrDigit) || workSheet.Cells[i, 1].Text.Trim() == "")
                {   //form no
                    message = message + " " + headerNames[1 - 1] + ": " + workSheet.Cells[i, 1].Text.Trim(); blankRowCounter++;
                }
                if (!(nameExp.IsMatch(workSheet.Cells[i, 2].Text.Trim()) && workSheet.Cells[i, 2].Text.Trim() != "" && workSheet.Cells[i, 2].Text.Length <= 99))
                {   //first name
                    message = message + " " + headerNames[2 - 1] + ": " + workSheet.Cells[i, 2].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 5].Text.Trim().ToLower() == Constants.Company.ToLower() || workSheet.Cells[i, 5].Text.Trim().ToLower() == Constants.Male.ToLower() || workSheet.Cells[i, 5].Text.Trim().ToLower() == Constants.Female.ToLower()))
                {   //gender name
                    message = message + " " + headerNames[5 - 1] + ": " + workSheet.Cells[i, 5].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 12].Text.Trim() != "" && workSheet.Cells[i, 12].Text.Length <= 499))
                {   //corresponding address
                    message = message + " " + headerNames[12 - 1] + ": " + workSheet.Cells[i, 12].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 13].Text.Trim() != "" && workSheet.Cells[i, 13].Text.Length <= 499)) 
                {   //permanent address
                    message = message + " " + headerNames[13 - 1] + ": " + workSheet.Cells[i, 13].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 14].Text.All(char.IsLetterOrDigit) && workSheet.Cells[i, 14].Text.Trim() != "" && workSheet.Cells[i, 14].Text.Length <= 15))
                {   //mobile number
                    message = message + " " + headerNames[14 - 1] + ": " + workSheet.Cells[i, 14].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 15].Text.All(char.IsLetterOrDigit) && workSheet.Cells[i, 15].Text.Length <= 15))
                {   //phone number
                    message = message + " " + headerNames[15 - 1] + ": " + workSheet.Cells[i, 15].Text.Trim(); blankRowCounter++;
                }
                if (!(emailExp.IsMatch(workSheet.Cells[i, 17].Text) && workSheet.Cells[i, 17].Text.Trim() != "" && workSheet.Cells[i, 17].Text.Length <= 199))
                {   //Email
                    message = message + " " + headerNames[17 - 1] + ": " + workSheet.Cells[i, 17].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 21].Text.All(char.IsLetterOrDigit) && workSheet.Cells[i, 21].Text.Trim() != "" && workSheet.Cells[i, 21].Text.Length <= 10))
                {   //pan number
                    message = message + " " + headerNames[21 - 1] + ": " + workSheet.Cells[i, 21].Text.Trim(); blankRowCounter++;
                }
                if (!(Decimal.TryParse(workSheet.Cells[i, 22].Text, out decnum) && workSheet.Cells[i, 22].Text.Length <= 15))
                {   //annual income
                    message = message + " " + headerNames[22 - 1] + ": " + workSheet.Cells[i, 22].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 23].Text.Trim().ToLower() == Constants.DD.ToLower() || workSheet.Cells[i, 23].Text.Trim().ToLower() == Constants.RTGS.ToLower()))
                {   //payment mode
                    message = message + " " + headerNames[23 - 1] + ": " + workSheet.Cells[i, 23].Text.Trim(); blankRowCounter++;
                }
                if (workSheet.Cells[i, 5].Text.Trim().ToLower() == Constants.Company.ToLower())
                {
                    if (!(nameExp.IsMatch(workSheet.Cells[i, 10].Text) && workSheet.Cells[i, 10].Text != "" && workSheet.Cells[i, 10].Text.Length <= 149))
                    {   //signing authority
                        message = message + " " + headerNames[10 - 1] + ": " + workSheet.Cells[i, 10].Text.Trim(); blankRowCounter++;
                    }
                    if (!(workSheet.Cells[i, 11].Text != "" && workSheet.Cells[i, 11].Text.Length <= 499))
                    {   //registered office
                        message = message + " " + headerNames[11 - 1] + ": " + workSheet.Cells[i, 11].Text.Trim(); blankRowCounter++;
                    }

                    if (!(workSheet.Cells[i, 16].Text.All(char.IsLetterOrDigit) && workSheet.Cells[i, 16].Text.Length <= 15))
                    {   //fax number
                        message = message + " " + headerNames[16 - 1] + ": " + workSheet.Cells[i, 16].Text.Trim(); blankRowCounter++;
                    }
                }
                if (workSheet.Cells[i, 5].Text.Trim().ToLower() == Constants.Male.ToLower() || workSheet.Cells[i, 5].Text.Trim().ToLower() == Constants.Female.ToLower())
                {
                    if (!(workSheet.Cells[i, 3].Text.Trim().All(char.IsLetterOrDigit) && workSheet.Cells[i, 3].Text.Length <= 99))
                    {   //middle name
                        message = message + " " + headerNames[3 - 1] + ": " + workSheet.Cells[i, 3].Text.Trim(); blankRowCounter++;
                    }
                    if (!(workSheet.Cells[i, 4].Text.Trim().All(char.IsLetterOrDigit) && workSheet.Cells[i, 4].Text.Trim() != "" && workSheet.Cells[i, 4].Text.Length <= 99))
                    {   //last name
                        message = message + " " + headerNames[4 - 1] + ": " + workSheet.Cells[i, 4].Text.Trim(); blankRowCounter++;
                    }
                    if (!(workSheet.Cells[i, 5].Text.Trim().All(char.IsLetter) && workSheet.Cells[i, 5].Text.Length <= 7))
                    {   //gender name
                        message = message + " " + headerNames[5 - 1] + ": " + workSheet.Cells[i, 5].Text.Trim(); blankRowCounter++;
                    }
                    if (!(workSheet.Cells[i, 6].Text.Trim() == "married" || workSheet.Cells[i, 6].Text.Trim() == "unmarried"))
                    {   //marrital status
                        message = message + " " + headerNames[6 - 1] + ": " + workSheet.Cells[i, 6].Text.Trim(); blankRowCounter++;
                    }
                    if (!(nameExp.IsMatch(workSheet.Cells[i, 7].Text.Trim()) && workSheet.Cells[i, 7].Text.Trim() != "" && workSheet.Cells[i, 7].Text.Length <= 99))
                    {   //father/husband name
                        message = message + " " + headerNames[7 - 1] + ": " + workSheet.Cells[i, 7].Text.Trim(); blankRowCounter++;
                    }
                    if (!(nameExp.IsMatch(workSheet.Cells[i, 8].Text.Trim()) && workSheet.Cells[i, 8].Text.Trim() != "" && workSheet.Cells[i, 8].Text.Length <= 99))
                    {   //mother name
                        message = message + " " + headerNames[8 - 1] + ": " + workSheet.Cells[i, 8].Text.Trim(); blankRowCounter++;
                    }
                    if (!DateTime.TryParse(workSheet.Cells[i, 9].Text.Trim(), out Test))
                    {   //date of birth
                        message = message + " " + headerNames[9 - 1] + ": " + workSheet.Cells[i, 9].Text.Trim(); blankRowCounter++;
                    }
                    if (DateTime.TryParse(workSheet.Cells[i, 9].Text.Trim(), out Test))
                    {   // test for 18 years old 
                        if (Test.AddYears(18) > DateTime.Now)
                        {
                            message = message + " " + headerNames[9 - 1] + ": " + workSheet.Cells[i, 9].Text.Trim(); blankRowCounter++;
                        }
                    }

                    if (!nameExp.IsMatch(workSheet.Cells[i, 18].Text.Trim()) || workSheet.Cells[i, 18].Text.Trim() == "")
                    {   //occupation
                        message = message + " " + headerNames[18 - 1] + ": " + workSheet.Cells[i, 18].Text.Trim(); blankRowCounter++;
                    }
                    if (!workSheet.Cells[i, 19].Text.Trim().All(char.IsLetterOrDigit) || workSheet.Cells[i, 19].Text.Trim() == "")
                    {   //quota gen/obc/sc/st
                        message = message + " " + headerNames[19 - 1] + ": " + workSheet.Cells[i, 19].Text.Trim(); blankRowCounter++;
                    }
                    if (!workSheet.Cells[i, 20].Text.Trim().All(char.IsLetterOrDigit) || workSheet.Cells[i, 20].Text.Trim() == "")
                    {   //religion
                        message = message + " " + headerNames[20 - 1] + ": " + workSheet.Cells[i, 20].Text.Trim(); blankRowCounter++;
                    }
                }
                if (workSheet.Cells[i, 23].Text.Trim() != "")
                {
                    if (workSheet.Cells[i, 23].Text.Trim().ToLower() == Constants.DD.ToLower())
                    {
                        if (!(workSheet.Cells[i, 24].Text.Trim().All(char.IsLetterOrDigit) && workSheet.Cells[i, 24].Text.Trim() != "" && workSheet.Cells[i, 24].Text.Length <= 6))
                        {   //DD number
                            message = message + " " + headerNames[24 - 1] + ": " + workSheet.Cells[i, 24].Text.Trim(); blankRowCounter++;
                        }
                    }
                    if (workSheet.Cells[i, 23].Text.Trim().ToLower() == Constants.RTGS.ToLower())
                    {
                        if (!(workSheet.Cells[i, 25].Text.Trim().All(char.IsLetterOrDigit) && workSheet.Cells[i, 25].Text.Trim() != "" && workSheet.Cells[i, 25].Text.Length <= 50))
                        {   //utn number
                            message = message + " " + headerNames[25 - 1] + ": " + workSheet.Cells[i, 25].Text.Trim(); blankRowCounter++;
                        }
                    }
                }
                //if ((nameExp.IsMatch(workSheet.Cells[i, 26].Text) && workSheet.Cells[i, 26].Text.Trim() != "" && workSheet.Cells[i, 26].Text.Length <= 99))
                //{   //bank name for test
                //    var one = workSheet.Cells[i, 26].Text.Trim();
                //    var two = workSheet.Cells[i, 26].Value;
                //}

                if (!(nameExp.IsMatch(workSheet.Cells[i, 26].Text) && workSheet.Cells[i, 26].Text.Trim() != "" && workSheet.Cells[i, 26].Text.Length <= 99))
                {   //bank name
                    message = message + " " + headerNames[26 - 1] + ": " + workSheet.Cells[i, 26].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 27].Text.Trim() != "" && workSheet.Cells[i, 27].Text.Length <= 10))
                {   //bank id
                    message = message + " " + headerNames[27 - 1] + ": " + workSheet.Cells[i, 27].Text.Trim(); blankRowCounter++;
                }
                if (!Decimal.TryParse(workSheet.Cells[i, 28].Text, out decnum) || (workSheet.Cells[i, 28].Text.Trim().Length >= 18))
                {   //deposit amount
                    message = message + " " + headerNames[28 - 1] + ": " + workSheet.Cells[i, 28].Text.Trim(); blankRowCounter++;
                }

                if (!DateTime.TryParse(workSheet.Cells[i, 29].Text, out Test))
                {   // dd/rtgs issue date
                    message = message + " " + headerNames[29 - 1] + ": " + workSheet.Cells[i, 29].Text.Trim(); blankRowCounter++;
                }
                if (blankRowCounter == 28)
                {
                    return flag;
                }
                if (message != null)
                {
                    return "Row No: " + i + ":-" + message;
                }
            }

            return flag;
        }

        public static List<PropertyApplicationForm> ToApplicationFormModel(this ExcelPackage package)
        {
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            var len = workSheet.Dimension.End.Row;
            var lenColumn = workSheet.Dimension.End.Column;
            List<PropertyApplicationForm> modelList = new List<PropertyApplicationForm>();
            for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
            {
                PropertyApplicationForm model = new PropertyApplicationForm();
                if (workSheet.Cells[rowNumber, 5].Text.ToLower() == Constants.Company.ToLower())
                {
                    model.FormNo = workSheet.Cells[rowNumber, 1].Text;
                    model.FirstName = workSheet.Cells[rowNumber, 2].Text;
                    model.Gender = workSheet.Cells[rowNumber, 5].Text;
                    model.SigningAuthority = workSheet.Cells[rowNumber, 10].Text;
                    model.RegisteredOffice = workSheet.Cells[rowNumber, 11].Text;
                    model.CorrespondanceAdd = workSheet.Cells[rowNumber, 12].Text;
                    model.PermanentAdd = workSheet.Cells[rowNumber, 13].Text;
                    model.MobileNumberP2 = workSheet.Cells[rowNumber, 14].Text;
                    model.PhoneNumberP2 = workSheet.Cells[rowNumber, 15].Text;
                    model.FaxNumberP2 = workSheet.Cells[rowNumber, 16].Text;
                    model.Email = workSheet.Cells[rowNumber, 17].Text;
                }
                else
                {
                    model.FormNo = workSheet.Cells[rowNumber, 1].Text;
                    model.FirstName = workSheet.Cells[rowNumber, 2].Text;
                    model.MiddleName = workSheet.Cells[rowNumber, 3].Text;
                    model.LastName = workSheet.Cells[rowNumber, 4].Text;
                    model.Gender = workSheet.Cells[rowNumber, 5].Text;
                    model.MarritalStatus = workSheet.Cells[rowNumber, 6].Text;
                    model.FatherHusbandName = workSheet.Cells[rowNumber, 7].Text;
                    model.MotherName = workSheet.Cells[rowNumber, 8].Text;
                    model.DateOfBirth = DateTime.Parse(workSheet.Cells[rowNumber, 9].Text);
                    model.CorrespondanceAdd = workSheet.Cells[rowNumber, 12].Text;
                    model.PermanentAdd = workSheet.Cells[rowNumber, 13].Text;
                    model.MobileNumberP2 = workSheet.Cells[rowNumber, 14].Text;
                    model.PhoneNumberP2 = workSheet.Cells[rowNumber, 15].Text;
                    model.Email = workSheet.Cells[rowNumber, 17].Text;
                    model.OccupationId = AssignOccupation(workSheet.Cells[rowNumber, 18].Text.Trim().ToLower());
                    model.QuotaId = AssignCategory(workSheet.Cells[rowNumber, 19].Text.Trim().ToUpper());
                    model.ReligionId = AssignReligion(workSheet.Cells[rowNumber, 20].Text.Trim().ToLower());
                }
                model.PanNumber = workSheet.Cells[rowNumber, 21].Text;
                model.AnnualIncome = decimal.Parse(workSheet.Cells[rowNumber, 22].Text);
                model.PaymentMode = workSheet.Cells[rowNumber, 23].Text.ToUpper();
                model.DemandDraftNo = workSheet.Cells[rowNumber, 24].Text;
                model.Utn = workSheet.Cells[rowNumber, 25].Text;
                //model.DemandDraftIssueBank = workSheet.Cells[rowNumber, 26].Text;
                model.BankId = Int32.Parse(workSheet.Cells[rowNumber, 27].Text);
                model.AmountDeposited = decimal.Parse(workSheet.Cells[rowNumber, 28].Text);
                model.DemandDraftIssueDate = DateTime.Parse(workSheet.Cells[rowNumber, 29].Text);

                modelList.Add(model);
            }
            return modelList;
        }
        

        public static string ValidateExcelDataForApplicationForm(this ExcelPackage package)
        {
            string flag = null;
            DateTime Test = DateTime.Now;
            int n = 0;
            decimal decnum = 1.05M;
            Regex exp = new Regex("^[a-zA-Z0-9]*$");
            Regex nameExp = new Regex("^[a-zA-Z0-9\\-\\s]+$");
            Regex emailExp = new Regex("^([a-zA-Z0-9_\\-\\.]+)@(([a-zA-Z0-9\\-]+\\.)+)([a-zA-Z0-9]{2,4})$");
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            var excelColumnCount = workSheet.Dimension.End.Column;
            var excelRowCount = workSheet.Dimension.End.Row;
            string[] headerNames = Enum.GetNames(typeof(PropertyApplicationFormEnum));
            for (int i = 2; i <= excelRowCount; i++)
            {
                string message = null;
                //message = "Row No: " + i + ":-";
                if (!workSheet.Cells[i, 1].Text.Trim().All(char.IsLetterOrDigit) || workSheet.Cells[i, 1].Text.Trim() == "")
                {
                    //return headerNames[1-1]+" "+workSheet.Cells[i, 1].Text.Trim();
                    message = message+ " " + headerNames[1 - 1] + " " + workSheet.Cells[i, 1].Text.Trim();
                }
                if (!(nameExp.IsMatch(workSheet.Cells[i, 2].Text.Trim()) && workSheet.Cells[i, 2].Text.Trim() != "" && workSheet.Cells[i, 2].Text.Length <= 99))
                {
                    //return headerNames[2 - 1] + " " + workSheet.Cells[i, 2].Text.Trim();
                    message = message + " " + headerNames[2 - 1] + " " + workSheet.Cells[i, 2].Text.Trim();
                }
                if (!(workSheet.Cells[i, 5].Text.Trim().ToLower() == Constants.Company || workSheet.Cells[i, 5].Text.Trim().ToLower() == Constants.Male || workSheet.Cells[i,5].Text.Trim().ToLower()==Constants.Female))
                {
                    //return headerNames[5 - 1] + " " + workSheet.Cells[i, 5].Text.Trim();
                    message = message + " " + headerNames[5 - 1] + " " + workSheet.Cells[i, 5].Text.Trim();
                }
                if (!(nameExp.IsMatch(workSheet.Cells[i, 12].Text) && workSheet.Cells[i, 12].Text.Length <= 499))
                {
                    //return headerNames[12 - 1] + " " + workSheet.Cells[i, 12].Text.Trim();
                    message = message + " " + headerNames[12 - 1] + " " + workSheet.Cells[i, 12].Text.Trim();
                }
                if (!(nameExp.IsMatch(workSheet.Cells[i, 13].Text) && workSheet.Cells[i, 13].Text.Length <= 499))
                {
                    //return headerNames[13 - 1] + " " + workSheet.Cells[i, 13].Text.Trim();
                    message = message + " " + headerNames[13 - 1] + " " + workSheet.Cells[i, 13].Text.Trim();
                }
                if (!(workSheet.Cells[i, 14].Text.All(char.IsLetterOrDigit) && workSheet.Cells[i, 14].Text.Trim() != "" && workSheet.Cells[i, 14].Text.Length <= 15))
                {
                    //return headerNames[14 - 1] + " " + workSheet.Cells[i, 14].Text.Trim();
                    message = message + " " + headerNames[14 - 1] + " " + workSheet.Cells[i, 14].Text.Trim();
                }
                if (!(workSheet.Cells[i, 15].Text.All(char.IsLetterOrDigit) && workSheet.Cells[i, 15].Text.Length <= 15))
                {
                    //return headerNames[15 - 1] + " " + workSheet.Cells[i, 15].Text.Trim();
                    message = message + " " + headerNames[15 - 1] + " " + workSheet.Cells[i, 15].Text.Trim();
                }
                if (!(emailExp.IsMatch(workSheet.Cells[i, 17].Text) && workSheet.Cells[i, 17].Text.Length <= 199))
                {
                    //return headerNames[17 - 1] + " " + workSheet.Cells[i, 17].Text.Trim();
                    message = message + " " + headerNames[17 - 1] + " " + workSheet.Cells[i, 17].Text.Trim();
                }
                if (!(workSheet.Cells[i, 21].Text.All(char.IsLetterOrDigit) && workSheet.Cells[i, 21].Text.Trim() != "" && workSheet.Cells[i, 21].Text.Length <= 10))
                {
                    //return headerNames[21 - 1] + " " + workSheet.Cells[i, 21].Text.Trim();
                    message = message + " " + headerNames[21 - 1] + " " + workSheet.Cells[i, 21].Text.Trim();
                }
                if (!(Decimal.TryParse(workSheet.Cells[i, 22].Text, out decnum) && workSheet.Cells[i, 22].Text.Length <= 15))
                {
                    //return headerNames[22 - 1] + " " + workSheet.Cells[i, 22].Text.Trim();
                    message = message + " " + headerNames[22 - 1] + " " + workSheet.Cells[i, 22].Text.Trim();
                }
                if (!(workSheet.Cells[i, 23].Text.Trim().ToLower() == Constants.DD.ToLower() || workSheet.Cells[i, 23].Text.Trim().ToLower() == Constants.RTGS.ToLower()))
                {
                    //return headerNames[23 - 1] + " " + workSheet.Cells[i, 23].Text.Trim();
                    message = message + " " + headerNames[23 - 1] + " " + workSheet.Cells[i, 23].Text.Trim();
                }
                if (workSheet.Cells[i, 5].Text.Trim().ToLower() == Constants.Company)
                {
                    if (!(nameExp.IsMatch(workSheet.Cells[i, 10].Text) && workSheet.Cells[i, 10].Text.Length <= 149))
                    {
                        //return headerNames[10 - 1] + " " + workSheet.Cells[i, 10].Text.Trim();
                        message = message + " " + headerNames[10 - 1] + " " + workSheet.Cells[i, 10].Text.Trim();
                    }
                    if (!(nameExp.IsMatch(workSheet.Cells[i, 11].Text) && workSheet.Cells[i, 11].Text.Length <= 499))
                    {
                        //return headerNames[11 - 1] + " " + workSheet.Cells[i, 11].Text.Trim();
                        message = message + " " + headerNames[11 - 1] + " " + workSheet.Cells[i, 11].Text.Trim();
                    }

                    if (!(workSheet.Cells[i, 16].Text.All(char.IsLetterOrDigit) && workSheet.Cells[i, 16].Text.Length <= 15))
                    {
                        //return headerNames[16 - 1] + " " + workSheet.Cells[i, 16].Text.Trim();
                        message = message + " " + headerNames[16 - 1] + " " + workSheet.Cells[i, 16].Text.Trim();
                    }

                    //flag = true;
                }
                if (workSheet.Cells[i, 5].Text.Trim().ToLower() == Constants.Male || workSheet.Cells[i, 5].Text.Trim().ToLower() == Constants.Female)
                {
                    if (!(workSheet.Cells[i, 3].Text.Trim().All(char.IsLetterOrDigit) && workSheet.Cells[i, 3].Text.Length <= 99))
                    {
                        //return headerNames[3 - 1] + " " + workSheet.Cells[i, 3].Text.Trim();
                        message = message + " " + headerNames[3 - 1] + " " + workSheet.Cells[i, 3].Text.Trim();
                    }
                    if (!(workSheet.Cells[i, 4].Text.Trim().All(char.IsLetterOrDigit) && workSheet.Cells[i, 4].Text.Trim() != "" && workSheet.Cells[i, 4].Text.Length <= 99))
                    {
                        //return headerNames[4 - 1] + " " + workSheet.Cells[i, 4].Text.Trim();
                        message = message + " " + headerNames[4 - 1] + " " + workSheet.Cells[i, 4].Text.Trim();
                    }
                    if (!(workSheet.Cells[i, 5].Text.Trim().All(char.IsLetter) && workSheet.Cells[i, 5].Text.Length <= 7))
                    {
                        //return headerNames[5 - 1] + " " + workSheet.Cells[i, 5].Text.Trim();
                        message = message + " " + headerNames[5 - 1] + " " + workSheet.Cells[i, 5].Text.Trim();
                    }
                    if (!(workSheet.Cells[i, 6].Text.Trim().All(char.IsLetterOrDigit) && workSheet.Cells[i, 6].Text.Length <= 9))
                    {
                        //return headerNames[6 - 1] + " " + workSheet.Cells[i, 6].Text.Trim();
                        message = message + " " + headerNames[6 - 1] + " " + workSheet.Cells[i, 6].Text.Trim();
                    }
                    if (!(nameExp.IsMatch(workSheet.Cells[i, 7].Text.Trim()) && workSheet.Cells[i, 7].Text.Length <= 99))
                    {
                        //return headerNames[7 - 1] + " " + workSheet.Cells[i, 7].Text.Trim();
                        message = message + " " + headerNames[7 - 1] + " " + workSheet.Cells[i, 7].Text.Trim();
                    }
                    if (!(nameExp.IsMatch(workSheet.Cells[i, 8].Text.Trim()) && workSheet.Cells[i, 8].Text.Length <= 99))
                    {
                        //return headerNames[8 - 1] + " " + workSheet.Cells[i, 8].Text.Trim();
                        message = message + " " + headerNames[8 - 1] + " " + workSheet.Cells[i, 8].Text.Trim();
                    }
                    if (!DateTime.TryParse(workSheet.Cells[i, 9].Text.Trim(), out Test))
                    {
                        message = message + " " + headerNames[9 - 1] + " " + workSheet.Cells[i, 9].Text.Trim();
                    }
                    if (DateTime.TryParse(workSheet.Cells[i, 9].Text.Trim(), out Test))
                    {
                        if (Test.AddYears(18) > DateTime.Now)
                        {
                            message = message + " " + headerNames[9 - 1] + " " + workSheet.Cells[i, 9].Text.Trim();
                            //return headerNames[9 - 1] + " " + workSheet.Cells[i, 9].Text.Trim(); 
                        }                                            
                    }
                    
                    if (!int.TryParse(workSheet.Cells[i, 18].Text, out n))
                    {
                        //return headerNames[18 - 1] + " " + workSheet.Cells[i, 18].Text.Trim();
                        message = message + " " + headerNames[18 - 1] + " " + workSheet.Cells[i, 18].Text.Trim();
                    }
                    if (!int.TryParse(workSheet.Cells[i, 19].Text, out n))
                    {
                        //return headerNames[19 - 1] + " " + workSheet.Cells[i, 19].Text.Trim();
                        message = message + " " + headerNames[19 - 1] + " " + workSheet.Cells[i, 19].Text.Trim();
                    }
                    if (!int.TryParse(workSheet.Cells[i, 20].Text, out n))
                    {
                        //return headerNames[20 - 1] + " " + workSheet.Cells[i, 20].Text.Trim();
                        message = message + " " + headerNames[20 - 1] + " " + workSheet.Cells[i, 20].Text.Trim();
                    }

                    //flag = true;
                }
                if (workSheet.Cells[i, 23].Text.Trim() != "")
                {
                    if (workSheet.Cells[i, 23].Text.Trim().ToLower() == Constants.DD.ToLower())
                    {
                        if (!(workSheet.Cells[i, 27].Text.Trim().All(char.IsLetterOrDigit) && workSheet.Cells[i, 27].Text.Length <= 6)) //DD number
                        {
                            //return headerNames[27 - 1] + " " + workSheet.Cells[i, 27].Text.Trim();
                            message = message + " " + headerNames[27 - 1] + " " + workSheet.Cells[i, 27].Text.Trim();
                        }
                    }
                    if (workSheet.Cells[i, 23].Text.Trim().ToLower() == Constants.RTGS.ToLower())
                    {
                        if (!(workSheet.Cells[i, 28].Text.Trim().All(char.IsLetterOrDigit) && workSheet.Cells[i, 28].Text.Length <= 50)) //UTN number
                        {
                            //return headerNames[28 - 1] + " " + workSheet.Cells[i, 28].Text.Trim();
                            message = message + " " + headerNames[28 - 1] + " " + workSheet.Cells[i, 28].Text.Trim();
                        }
                    }
                    //flag = true;
                }
                //else
                //{
                //    return flag = false;
                //}

                if (!int.TryParse(workSheet.Cells[i, 24].Text, out n))
                {
                    //return headerNames[24 - 1] + " " + workSheet.Cells[i, 24].Text.Trim();
                    message = message + " " + headerNames[24 - 1] + " " + workSheet.Cells[i, 24].Text.Trim();
                }
                if (!int.TryParse(workSheet.Cells[i, 25].Text, out n))
                {
                    //return headerNames[25 - 1] + " " + workSheet.Cells[i, 25].Text.Trim();
                    message = message + " " + headerNames[25 - 1] + " " + workSheet.Cells[i, 25].Text.Trim();
                }
                if (!Decimal.TryParse(workSheet.Cells[i, 26].Text, out decnum))
                {
                    //return headerNames[26 - 1] + " " + workSheet.Cells[i, 26].Text.Trim();
                    message = message + " " + headerNames[26 - 1] + " " + workSheet.Cells[i, 26].Text.Trim();
                }
                if (!(workSheet.Cells[i, 29].Text.All(char.IsLetterOrDigit) && workSheet.Cells[i, 29].Text.Trim() != "" && workSheet.Cells[i, 29].Text.Length <= 99))
                {
                    //return headerNames[29 - 1] + " " + workSheet.Cells[i, 29].Text.Trim();
                    message = message + " " + headerNames[29 - 1] + " " + workSheet.Cells[i, 29].Text.Trim();
                }
                if (!DateTime.TryParse(workSheet.Cells[i, 30].Text, out Test))
                {
                    //return headerNames[30 - 1] + " " + workSheet.Cells[i, 30].Text.Trim();
                    message = message + " " + headerNames[30 - 1] + " " + workSheet.Cells[i, 30].Text.Trim();
                }

                if (message != null)
                {
                    return "Row No: "+ i +":-"+message;
                }
            }

            return flag;
        }

        public static string ValidateHeaderNameForIndividualForm(this ExcelPackage package)
        {
            string message = string.Empty;
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            var excelColumnCount = workSheet.Dimension.End.Column;
            string[] excelColumns = new string[excelColumnCount];

            for (int i = 1; i <= excelColumnCount; i++)
            {
                excelColumns[i - 1] = workSheet.Cells[1, i].Text.Trim();
            }
            
            string[] headerName = IndividualForm.FormHeader;
            for (int x = 0; x < excelColumnCount; x++)
            {
                if (headerName[x].ToLower().Trim() == excelColumns[x].ToLower().Trim())
                {
                    //return message;
                }
                else
                {
                    message = "Error in header name: " + excelColumns[x];
                }
            }
            return message;
        }

        public static string ValidateExcelDataForIndividualForm(this ExcelPackage package)
        {
            string flag = null;
            DateTime Test = DateTime.Now;
            int n = 0;
            decimal decnum = 1.05M;
            Regex exp = new Regex("^[a-zA-Z0-9]*$");
            Regex nameExp = new Regex("^[a-zA-Z0-9\\-\\.\\s]+$");
            Regex emailExp = new Regex("^([a-zA-Z0-9_\\-\\.]+)@(([a-zA-Z0-9\\-]+\\.)+)([a-zA-Z0-9]{2,4})$");
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            var excelColumnCount = workSheet.Dimension.End.Column;
            var excelRowCount = workSheet.Dimension.End.Row;

            string[] headerNames = IndividualForm.FormHeader;
            for (int i = 2; i <= excelRowCount; i++)
            {
                string message = null; int blankRowCounter = 0;

                if (!workSheet.Cells[i, 1].Text.Trim().All(char.IsLetterOrDigit) || workSheet.Cells[i, 1].Text.Trim() == "")
                {   //form no
                    message = message + " " + headerNames[1 - 1] + ": " + workSheet.Cells[i, 1].Text.Trim(); blankRowCounter++;
                }
                if (!(nameExp.IsMatch(workSheet.Cells[i, 2].Text.Trim()) && workSheet.Cells[i, 2].Text.Trim() != "" && workSheet.Cells[i, 2].Text.Length <= 99))
                {   //first name
                    message = message + " " + headerNames[2 - 1] + ": " + workSheet.Cells[i, 2].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 3].Text.Trim().All(char.IsLetterOrDigit) && workSheet.Cells[i, 3].Text.Length <= 99))
                {   //middle name
                    message = message + " " + headerNames[3 - 1] + ": " + workSheet.Cells[i, 3].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 4].Text.Trim().All(char.IsLetterOrDigit) && workSheet.Cells[i, 4].Text.Trim() != "" && workSheet.Cells[i, 4].Text.Length <= 99))
                {   //last name
                    message = message + " " + headerNames[4 - 1] + ": " + workSheet.Cells[i, 4].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 5].Text.Trim().ToLower() == Constants.Male.ToLower() || workSheet.Cells[i, 5].Text.Trim().ToLower() == Constants.Female.ToLower()))
                {   //gender name
                    message = message + " " + headerNames[5 - 1] + ": " + workSheet.Cells[i, 5].Text.Trim(); blankRowCounter++;
                }               
                //if (!(workSheet.Cells[i, 5].Text.Trim().All(char.IsLetter) && workSheet.Cells[i, 5].Text.Length <= 7))
                //{   //gender name
                //    message = message + " " + headerNames[5 - 1] + ": " + workSheet.Cells[i, 5].Text.Trim(); blankRowCounter++;
                //}
                if (!(workSheet.Cells[i, 6].Text.Trim() == MaritalStatus.Married.ToLower() || workSheet.Cells[i, 6].Text.Trim() == MaritalStatus.Unmarried.ToLower() || workSheet.Cells[i, 6].Text.Trim() == MaritalStatus.Yes.ToLower() || workSheet.Cells[i, 6].Text.Trim() == MaritalStatus.No.ToLower()))
                {   //marrital status
                    message = message + " " + headerNames[6 - 1] + ": " + workSheet.Cells[i, 6].Text.Trim(); blankRowCounter++;
                }
                if (!(nameExp.IsMatch(workSheet.Cells[i, 7].Text.Trim()) && workSheet.Cells[i, 7].Text.Trim() != "" && workSheet.Cells[i, 7].Text.Length <= 99))
                {   //father/husband name
                    message = message + " " + headerNames[7 - 1] + ": " + workSheet.Cells[i, 7].Text.Trim(); blankRowCounter++;
                }
                if (!(nameExp.IsMatch(workSheet.Cells[i, 8].Text.Trim()) && workSheet.Cells[i, 8].Text.Trim() != "" && workSheet.Cells[i, 8].Text.Length <= 99))
                {   //mother name
                    message = message + " " + headerNames[8 - 1] + ": " + workSheet.Cells[i, 8].Text.Trim(); blankRowCounter++;
                }
                if (!DateTime.TryParse(workSheet.Cells[i, 9].Text.Trim(), out Test))
                {   //date of birth
                    message = message + " " + headerNames[9 - 1] + ": " + workSheet.Cells[i, 9].Text.Trim(); blankRowCounter++;
                }
                if (DateTime.TryParse(workSheet.Cells[i, 9].Text.Trim(), out Test))
                {   // test for 18 years old 
                    if (Test.AddYears(18) > DateTime.Now)
                    {
                        message = message + " " + headerNames[9 - 1] + ": " + workSheet.Cells[i, 9].Text.Trim(); blankRowCounter++;
                    }
                }
                if (!(workSheet.Cells[i, 10].Text.Trim() != "" && workSheet.Cells[i, 10].Text.Length <= 499))
                {   //corresponding address
                    message = message + " " + headerNames[10 - 1] + ": " + workSheet.Cells[i, 10].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 11].Text.Trim() != "" && workSheet.Cells[i, 11].Text.Length <= 499))
                {   //permanent address
                    message = message + " " + headerNames[11 - 1] + ": " + workSheet.Cells[i, 11].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 12].Text.All(char.IsLetterOrDigit) && workSheet.Cells[i, 12].Text.Trim() != "" && workSheet.Cells[i, 12].Text.Length <= 15))
                {   //mobile number
                    message = message + " " + headerNames[12 - 1] + ": " + workSheet.Cells[i, 12].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 13].Text.All(char.IsLetterOrDigit) && workSheet.Cells[i, 13].Text.Length <= 15))
                {   //phone number
                    message = message + " " + headerNames[13 - 1] + ": " + workSheet.Cells[i, 13].Text.Trim(); blankRowCounter++;
                }
                if (!(emailExp.IsMatch(workSheet.Cells[i, 14].Text) && workSheet.Cells[i, 14].Text.Trim() != "" && workSheet.Cells[i, 14].Text.Length <= 199))
                {   //Email
                    message = message + " " + headerNames[14 - 1] + ": " + workSheet.Cells[i, 14].Text.Trim(); blankRowCounter++;
                }
                if (!nameExp.IsMatch(workSheet.Cells[i, 15].Text.Trim()) || workSheet.Cells[i, 15].Text.Trim() == "")
                {   //occupation
                    message = message + " " + headerNames[15 - 1] + ": " + workSheet.Cells[i, 15].Text.Trim(); blankRowCounter++;
                }
                if (!workSheet.Cells[i, 16].Text.Trim().All(char.IsLetterOrDigit) || workSheet.Cells[i, 16].Text.Trim() == "")
                {   //quota gen/obc/sc/st
                    message = message + " " + headerNames[16 - 1] + ": " + workSheet.Cells[i, 16].Text.Trim(); blankRowCounter++;
                }
                if (!workSheet.Cells[i, 17].Text.Trim().All(char.IsLetterOrDigit) || workSheet.Cells[i, 17].Text.Trim() == "")
                {   //religion
                    message = message + " " + headerNames[17 - 1] + ": " + workSheet.Cells[i, 17].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 18].Text.All(char.IsLetterOrDigit) && workSheet.Cells[i, 18].Text.Trim() != "" && workSheet.Cells[i, 18].Text.Length <= 10))
                {   //pan number
                    message = message + " " + headerNames[18 - 1] + ": " + workSheet.Cells[i, 18].Text.Trim(); blankRowCounter++;
                }
                if (!(Decimal.TryParse(workSheet.Cells[i, 19].Text, out decnum) && workSheet.Cells[i, 19].Text.Length <= 15))
                {   //annual income
                    message = message + " " + headerNames[19 - 1] + ": " + workSheet.Cells[i, 19].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 20].Text.Trim().ToLower() == Constants.DD.ToLower() || workSheet.Cells[i, 20].Text.Trim().ToLower() == Constants.RTGS.ToLower()))
                {   //payment mode
                    message = message + " " + headerNames[20 - 1] + ": " + workSheet.Cells[i, 20].Text.Trim(); blankRowCounter++;
                }
                if (workSheet.Cells[i, 20].Text.Trim() != "")
                {
                    if (workSheet.Cells[i, 20].Text.Trim().ToLower() == Constants.DD.ToLower())
                    {
                        if (!(workSheet.Cells[i, 21].Text.Trim().All(char.IsLetterOrDigit) && workSheet.Cells[i, 21].Text.Trim() != "" && workSheet.Cells[i, 21].Text.Length <= 6))
                        {   //DD number
                            message = message + " " + headerNames[21 - 1] + ": " + workSheet.Cells[i, 21].Text.Trim(); blankRowCounter++;
                        }
                    }
                    if (workSheet.Cells[i, 20].Text.Trim().ToLower() == Constants.RTGS.ToLower())
                    {
                        if (!(workSheet.Cells[i, 22].Text.Trim().All(char.IsLetterOrDigit) && workSheet.Cells[i, 22].Text.Trim() != "" && workSheet.Cells[i, 22].Text.Length <= 50))
                        {   //utn number
                            message = message + " " + headerNames[22 - 1] + ": " + workSheet.Cells[i, 22].Text.Trim(); blankRowCounter++;
                        }
                    }
                }
                if (!(nameExp.IsMatch(workSheet.Cells[i, 23].Text) && workSheet.Cells[i, 23].Text.Trim() != "" && workSheet.Cells[i, 23].Text.Length <= 99))
                {   //bank name
                    message = message + " " + headerNames[23 - 1] + ": " + workSheet.Cells[i, 23].Text.Trim(); blankRowCounter++;
                }
                if (!(workSheet.Cells[i, 24].Text.Trim() != "" && workSheet.Cells[i, 24].Text.Length <= 10))
                {   //bank id
                    message = message + " " + headerNames[24 - 1] + ": " + workSheet.Cells[i, 24].Text.Trim(); blankRowCounter++;
                }
                if (!Decimal.TryParse(workSheet.Cells[i, 25].Text, out decnum) || (workSheet.Cells[i, 25].Text.Trim().Length >= 18))
                {   //deposit amount
                    message = message + " " + headerNames[25 - 1] + ": " + workSheet.Cells[i, 25].Text.Trim(); blankRowCounter++;
                }

                if (!DateTime.TryParse(workSheet.Cells[i, 26].Text, out Test))
                {   // dd/rtgs issue date
                    message = message + " " + headerNames[26 - 1] + ": " + workSheet.Cells[i, 26].Text.Trim(); blankRowCounter++;
                }
                
                if (message != null)
                {
                    return "Row No: " + i + ":-" + message;
                }
            }

            return flag;
        }

        public static List<PropertyApplicationForm> ToIndividualFormModel(this ExcelPackage package)
        {
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            var len = workSheet.Dimension.End.Row;
            var lenColumn = workSheet.Dimension.End.Column;
            List<PropertyApplicationForm> modelList = new List<PropertyApplicationForm>();
            for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
            {
                PropertyApplicationForm model = new PropertyApplicationForm();

                model.FormNo = workSheet.Cells[rowNumber, 1].Text;
                model.FirstName = workSheet.Cells[rowNumber, 2].Text;
                model.MiddleName = workSheet.Cells[rowNumber, 3].Text;
                model.LastName = workSheet.Cells[rowNumber, 4].Text;
                model.Gender = workSheet.Cells[rowNumber, 5].Text;
                model.MarritalStatus = AssignMaritalStatus(workSheet.Cells[rowNumber, 6].Text);
                model.FatherHusbandName = workSheet.Cells[rowNumber, 7].Text;
                model.MotherName = workSheet.Cells[rowNumber, 8].Text;
                model.DateOfBirth = DateTime.Parse(workSheet.Cells[rowNumber, 9].Text);
                model.CorrespondanceAdd = workSheet.Cells[rowNumber, 10].Text;
                model.PermanentAdd = workSheet.Cells[rowNumber, 11].Text;
                model.MobileNumberP2 = workSheet.Cells[rowNumber, 12].Text;
                model.PhoneNumberP2 = workSheet.Cells[rowNumber, 13].Text;
                model.Email = workSheet.Cells[rowNumber, 14].Text;
                model.OccupationId = AssignOccupation(workSheet.Cells[rowNumber, 15].Text.Trim().ToLower());
                model.QuotaId = AssignCategory(workSheet.Cells[rowNumber, 16].Text.Trim().ToUpper());
                model.ReligionId = AssignReligion(workSheet.Cells[rowNumber, 17].Text.Trim().ToLower());
                model.PanNumber = workSheet.Cells[rowNumber, 18].Text;
                model.AnnualIncome = decimal.Parse(workSheet.Cells[rowNumber, 19].Text);
                model.PaymentMode = workSheet.Cells[rowNumber, 20].Text.ToUpper();
                model.DemandDraftNo = workSheet.Cells[rowNumber, 21].Text;
                model.Utn = workSheet.Cells[rowNumber, 22].Text;
                //model.DemandDraftIssueBank = workSheet.Cells[rowNumber, 23].Text;
                model.BankId = Int32.Parse(workSheet.Cells[rowNumber, 24].Text);
                model.AmountDeposited = decimal.Parse(workSheet.Cells[rowNumber, 25].Text);
                model.DemandDraftIssueDate = DateTime.Parse(workSheet.Cells[rowNumber, 26].Text);

                modelList.Add(model);
            }
            return modelList;
        }

        public static int AssignReligion(string relgion)
        {
            switch (relgion)
            {
                case "hindu" : return 1;
                case "muslim": return 2;
                case "sikh": return 3;
                case "christian": return 4;
                case "budhism": return 5;
                case "jain": return 6;
                case "others": return 7;
                default: return 7;
            }
        }
        public static int AssignCategory(string category)
        {
            switch (category)
            {
                case "ST": return 1;
                case "OBC": return 2;
                case "EBC": return 3;
                case "SC": return 4;
                case "GENERAL": return 5;
                default: return 5;
            }
        }
        public static int AssignOccupation(string occupation)
        {
            switch (occupation)
            {
                case "self-employed": return 1;
                case "pvt. employee": return 2;
                case "govt. employee": return 3;
                case "farmer": return 4;
                case "pensioner": return 5;
                case "others": return 6;
                default: return 6;
            }
        }
        public static string AssignMaritalStatus(string status)
        {
            switch (status)
            {
                case "married": return MaritalStatus.Married;
                case "Married": return MaritalStatus.Married;
                case "Unmarried": return MaritalStatus.Unmarried;
                case "unmarried": return MaritalStatus.Unmarried;
                case "Yes": return MaritalStatus.Married;
                case "yes": return MaritalStatus.Married;
                case "No": return MaritalStatus.Unmarried;
                case "no": return MaritalStatus.Unmarried;
                default: return MaritalStatus.Unmarried;
            }
        }
    }

    

    public enum PropertyApplicationFormEnum
    {
        FormNo=1,
        FirstName = 2,
        MiddleName=3,
        LastName=4,
        Gender=5,
        MarritalStatus=6,
        FatherHusbandName=7,
        MotherName=8,
        DateOfBirth=9,
        SigningAuthority=10,
        RegisteredOffice=11,
        CorrespondanceAdd=12,
        PermanentAdd=13,
        MobileNumber=14,
        PhoneNumber=15,
        FaxNumber=16,
        Email=17,
        OccupationId=18,
        QuotaId=19,
        ReligionId=20,
        PanNumber=21,
        AnnualIncome=22,
        
        PaymentMode=23,
        BankId=24,
        BranchId=25,
        AmountDeposited=26,
        DDNo=27,
        UTN=28,
        IssueBank=29,
        IssueDate=30
    }

    public enum AllotmentEnum
    {
        FormNo,
        PropertyId,
        AllotmentDate,
        InstallmentStartDate
    }

}
