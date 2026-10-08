using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model
{
    public class ApplicationModel
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
    }

    public class AuditViewModel
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> AuditId { get; set; }
        
        public string TableName { get; set; }
        public string FormName { get; set; }
        public string KeyField { get; set; }
        public string KeyValue { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ActionName { get; set; }
        public string ActionType { get; set; }        
        public string ActivityType { get; set; }

        public Nullable<DateTime> ActionDate { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
    }

    public class AuditActionModel
    {
        public int AuditID { get; set; }
        public string ActionType { get; set; }
        public string TableName { get; set; }
        public string FormName { get; set; }
        public string PrimaryKeyField { get; set; }
        public string PrimaryKeyValue { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string UserName { get; set; }

        public string ActivityType
        {
            get
            {
                if (ActionType == "D")
                {
                    return "Delete";
                }
                if (ActionType == "I")
                {
                    return "Insert";
                }
                if (ActionType == "U")
                {
                    return "Update";
                }
                else
                {
                    return "";
                }
            }
        }
    }

    public class AuditTrailModel
    {
        public string ModuleName { get; set; }
        public List<AuditActionModel> Audit { get; set; }
    }

    public class PropertyTransactionAudit
    {
        public int Id { get; set; }
        public Nullable<int> Rid { get; set; }
        public string PropertyNumber { get; set; }
        public string ServiceName { get; set; }
        public Nullable<int> UserIdentity { get; set; }
        public Nullable<System.DateTime> ServiceDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string UserName { get; set; }
    }
}
