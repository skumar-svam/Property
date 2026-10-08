using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NA.PMS.Model.CommonModel
{
    public class CommonListModel
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }

    public class Audittable
    {
        public int AuditID { get; set; }
        public string Type { get; set; }
        public string TableName { get; set; }
        public string PrimaryKeyField { get; set; }
        public string PrimaryKeyValue { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UserName { get; set; }                
    }

    public class AuditDelta
    {
        public string FieldName { get; set; }
        public string ValueBefore { get; set; }
        public string ValueAfter { get; set; }
    }

    public class AutoSchedule
    {
        public int? DurationInDays { get; set; }
    }

    public class RoleMenuKeyModel
    {
        public Nullable<bool> EditMenuVal { get; set; }
        public Nullable<bool> AddMenuVal { get; set; }
        public Nullable<bool> DeleteMenuVal { get; set; }
        public Nullable<bool> ReadOnlyMenu { get; set; }
    }

}
