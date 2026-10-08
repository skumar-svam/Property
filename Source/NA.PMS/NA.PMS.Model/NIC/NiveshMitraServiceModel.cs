using System;
using System.IO;

namespace NA.PMS.Model
{
    public class NiveshMitraServiceModel
    {
    }

    public class WBasicDetailsModel_NMS
    {
        public string TxtControlID { get; set; }
        public string TxtUnitID { get; set; }
        public string TxtServiceID { get; set; }
        public string TxtProcessIndustryID { get; set; }
        public string TxtApplicationID { get; set; }
    }

    public class NewDataSet_NMS
    {
        public Table Table { get; set; }
    }

    public class Table_NMS
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

    public class Deserial_NMS
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

    public class WreturnCusidStatusModel_NMS
    {
        public string ControlID { get; set; }
        public string UnitID { get; set; }
        public string ServiceID { get; set; }
        public string ProcessIndustryID { get; set; }
        public string ApplicationID { get; set; }
        public string Status_Code { get; set; }
        public string Remarks { get; set; }
        public string Fee_Amount { get; set; }
        public string Fee_Status { get; set; }
        public string Transaction_ID { get; set; }
        public string Transaction_Date { get; set; }
        public string Transaction_Date_Time { get; set; }
        public string NOC_Certificate_Number { get; set; }
        public string NOC_URL { get; set; }
        public string ISNOC_URL_ActiveYesNO { get; set; }
        public string passsalt { get; set; }

        public string ISLandPurchasedYesNO { get; set; }
        public string PrintApplicationURL { get; set; }
    }
}
