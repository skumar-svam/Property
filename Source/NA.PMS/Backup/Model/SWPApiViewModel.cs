using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.NICServices
{
    public class SWPApiViewModel
    {
        public int onlineapplicationId { get; set; }
        public Nullable<int> schemeId { get; set; }
        public Nullable<int> Departmentid { get; set; }
        public string Control_ID { get; set; }
        public string Unit_Id { get; set; }
        public string ServiceID { get; set; }
        public string ProcessIndustryID { get; set; }
        public string Company_Name { get; set; }
        public string Industry_District { get; set; }
        public string Industry_District_Id { get; set; }
        public string Industry_Address { get; set; }
        public string Pin_Code { get; set; }
        public string Occupier_Name { get; set; }
        public string Occupier_Email_ID { get; set; }
        public string Occupier_Mobile_No { get; set; }
        public string Occupier_DOB { get; set; }
        public string Occupier_Gender { get; set; }
        public string Occupier_Address { get; set; }
        public string Occupier_District_ID { get; set; }
        public string Occupier_District_Name { get; set; }
        public string Occupier_Pin_Code { get; set; }
        public string Nature_of_Activity { get; set; }
        public string Installed_Capacity { get; set; }
        public string Employees { get; set; }
        public string Nature_of_Operation { get; set; }
        public string publicdecimalProject_Cost { get; set; }
        public string Organization_Type_ID { get; set; }
        public string Organization_Type { get; set; }
        public string Industry_Type_ID { get; set; }
        public string Industry_Type_Name { get; set; }
        public string Expected_date_construction { get; set; }
        public string Project_Status { get; set; }
        public string Industry_Color { get; set; }
        public string Expected_date_production { get; set; }
        public string Unit_Category { get; set; }
        public string Items_Manufactured { get; set; }
        public string Annual_Turnover { get; set; }
        public Nullable<decimal> Fee_Amount { get; set; }
        public string Fee_Status { get; set; }
        public Nullable<int> Status_Code { get; set; }
        public Nullable<int> Payment_Through { get; set; }
        public string Payment_Description { get; set; }
    }

    public class SWPTableViewModel
    {
        public SWPTable Table { get; set; }
    }

    public class NewDataSet
    {
        public SWPTable Table { get; set; }
    }
    
    public class SWPPostViewModel
    {
        public string TxtControlID { get; set; }
        public string TxtUnitID { get; set; }
        public string TxtServiceID { get; set; }
        public string TxtProcessIndustryID { get; set; }
        public string TxtApplicationID { get; set; }
        public string SchemeType { get; set; }
    }

    public class Table
    {
        public string Control_ID { get; set; }
        public string Unit_Id { get; set; }
        public string Company_Name { get; set; }
        public string Industry_District { get; set; }
        public string Industry_District_Id { get; set; }
        public string Industry_Address { get; set; }
        public string Pin_Code { get; set; }
        public string Occupier_Name { get; set; }
        public string Occupier_Email_ID { get; set; }
        public string Occupier_Mobile_No { get; set; }
        public string Occupier_DOB { get; set; }
        public string Occupier_PAN { get; set; }
        public string Occupier_Gender { get; set; }
        public string Occupier_Address { get; set; }
        public string Occupier_District_ID { get; set; }
        public string Occupier_District_Name { get; set; }
        public string Occupier_Pin_Code { get; set; }
        public string Nature_of_Activity { get; set; }
        public string Installed_Capacity { get; set; }
        public string Employees { get; set; }
        public string Nature_of_Operation { get; set; }
        public decimal Project_Cost { get; set; }
        public string Organization_Type_ID { get; set; }
        public string Organization_Type { get; set; }
        public string Industry_Type_ID { get; set; }
        public string Industry_Type_Name { get; set; }
        public string Expected_date_construction { get; set; }
        public string Project_Status { get; set; }
        public string Industry_Color { get; set; }
        public string Expected_date_production { get; set; }
        public string Unit_Category { get; set; }
        public string Items_Manufactured { get; set; }
        public decimal Annual_Turnover { get; set; }

        public string ServiceID { get; set; }

        public int OnlineApplicationId { get; set; }
        public int departmentId { get; set; }
        public int SchemeId { get; set; }
        public string ProcessIndustryID { get; set; }

        public string Fee_Amount { get; set; }
        public string Fee_Status { get; set; }
        public string Status_Code { get; set; }
        public Nullable<int> Payment_Through { get; set; }
        public string Payment_Description { get; set; }

        //Payment validation
        public string ControlID { get; set; }
        public string UnitID { get; set; }
        public string ApplicationID { get; set; }
        public string Remarks { get; set; }
        public string Transaction_ID { get; set; }
        public string Transaction_Date { get; set; }
        public string Transaction_Date_Time { get; set; }
        public string NOC_Certificate_Number { get; set; }
        public string NOC_URL { get; set; }
        public string ISNOC_URL_ActiveYesNO { get; set; }
        public string passsalt { get; set; }
        public string Service_ID { get; set; }
    }

    public class SWPTable
    {
        public string Control_ID { get; set; }
        public string Unit_Id { get; set; }
        public string Company_Name { get; set; }
        public string Industry_District { get; set; }
        public string Industry_District_Id { get; set; }
        public string Industry_Address { get; set; }
        public string Pin_Code { get; set; }
        public string Occupier_Name { get; set; }
        public string Occupier_Email_ID { get; set; }
        public string Occupier_Mobile_No { get; set; }
        public string Occupier_DOB { get; set; }
        public string Occupier_PAN { get; set; }
        public string Occupier_Gender { get; set; }
        public string Occupier_Address { get; set; }
        public string Occupier_District_ID { get; set; }
        public string Occupier_District_Name { get; set; }
        public string Occupier_Pin_Code { get; set; }
        public string Nature_of_Activity { get; set; }
        public string Installed_Capacity { get; set; }
        public string Employees { get; set; }
        public string Nature_of_Operation { get; set; }
        public decimal Project_Cost { get; set; }
        public string Organization_Type_ID { get; set; }
        public string Organization_Type { get; set; }
        public string Industry_Type_ID { get; set; }
        public string Industry_Type_Name { get; set; }
        public string Expected_date_construction { get; set; }
        public string Project_Status { get; set; }
        public string Industry_Color { get; set; }
        public string Expected_date_production { get; set; }
        public string Unit_Category { get; set; }
        public string Items_Manufactured { get; set; }
        public decimal Annual_Turnover { get; set; }

        public string ServiceID { get; set; }

        public int OnlineApplicationId { get; set; }
        public int departmentId { get; set; }
        public int SchemeId { get; set; }
        public string ProcessIndustryID { get; set; }

        public string Fee_Amount { get; set; }
        public string Fee_Status { get; set; }
        public string Status_Code { get; set; }
        public Nullable<int> Payment_Through { get; set; }
        public string Payment_Description { get; set; }

        //Payment validation
        public string ControlID { get; set; }
        public string UnitID { get; set; }
        public string ApplicationID { get; set; }
        public string Remarks { get; set; }
        public string Transaction_ID { get; set; }
        public string Transaction_Date { get; set; }
        public string Transaction_Date_Time { get; set; }
        public string NOC_Certificate_Number { get; set; }
        public string NOC_URL { get; set; }
        public string ISNOC_URL_ActiveYesNO { get; set; }
        public string passsalt { get; set; }
        public string Service_ID { get; set; }
    }

    public class SWPStatusViewModel
    {
        public string ControlId { get; set; }
        public string UnitId { get; set; }
        public string ServiceId { get; set; }
        public string ProcessIndustryId { get; set; }
        public string ApplicationId { get; set; }
        public string StatusCode { get; set; }
        public string Remarks { get; set; }
        public string FeeAmount { get; set; }
        public string FeeStatus { get; set; }
        public string TransactionId { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionDateTime { get; set; }
        public string NOCCertificateNo { get; set; }
        public string NOCUrl { get; set; }
        public string NOCUrlActiveStatus { get; set; }//yes or no
        public string NICPassSalt { get; set; }

        public string LandPurchasedStatus { get; set; }//yes or no
        public string PrintApplicationUrl { get; set; }

        public string Status { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class SWPXmlDeserializer
    {
        public T Deserialize<T>(string input) where T : class
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }
    }

}
