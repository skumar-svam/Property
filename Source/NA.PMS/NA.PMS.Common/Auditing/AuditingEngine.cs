using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KellermanSoftware.CompareNetObjects;
using NA.PMS.Model.CommonModel;
using System.Web.Mvc;

namespace NA.PMS.Common
{
    public static class AuditingEngine
    {
        /// <summary>
        /// To Create History/Audit log.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableName"></param>
        /// <param name="viewName"></param>
        /// <param name="pkey"></param>
        /// <param name="oldObject"></param>
        /// <param name="newObject"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static ComparisonResult CreateAuditMaster(string type, string tableName, string viewName, int pkey, Object oldObject, Object newObject, string userID)
        {
            CompareLogic compObjects = new CompareLogic();
            compObjects.Config.MaxDifferences = 200;
            ComparisonResult compResult = compObjects.Compare(oldObject, newObject);
            return compResult;
        }
    }
}
