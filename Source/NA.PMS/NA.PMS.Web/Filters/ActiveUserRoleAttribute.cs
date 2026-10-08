using System.Web.Mvc;
using NA.PMS.Common.Logger;
using NA.PMS.Common.Extension;
using System;
using System.Web;
using System.Web.Routing;
using NA.PMS.Service;
using NA.PMS.Web.Models;
using NA.PMS.Model;

namespace NA.PMS.Web.Filters
{
    public class ActiveUserRoleAttribute : ActionFilterAttribute
    {
        ILoginService service = new LoginService();
       
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    CurrentUserDetail usr = new CurrentUserDetail();
        //    usr = HttpContext.Current.Session["CurrentUser"] as CurrentUserDetail;
        //    //var usr = (CurrentUserDetail)filterContext.HttpContext.Session["CurrentUser"];
        //    if (usr != null)
        //    {
        //        ILoginService service = new LoginService();
        //        bool flag = service.IsUserisActive(usr.UserID);
        //        if (filterContext.IsChildAction)
        //        {
        //            return;
        //        } 
        //        if (flag)
        //        {
        //            return;
        //        }                            
        //        else
        //        {
        //            //HttpContext.Current.Session.Clear();
        //            filterContext.HttpContext.Session.Clear();
        //        }
        //    }
        //    else
        //    {
                
        //        //new RedirectResult("/Account/Login");
        //    }
        //    base.OnActionExecuting(filterContext);

        //}

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //filterContext.HttpContext.Session["TT"] = HttpContext.Current.Session["CurrentUser"];
            if (filterContext.HttpContext.Session != null)
            {
                if (filterContext.HttpContext.Session["CurrentUser"] != null)
                {
                    var usr = filterContext.HttpContext.Session["CurrentUser"] as CurrentUserDetail;
                    bool flag = service.IsUserisActive(usr.UserID);
                    if (flag)
                    {
                        return;
                    }
                    else
                    {
                        filterContext.HttpContext.Session["CurrentUser"] = null;
                    }

                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new NotImplementedException();

        }
    }

    public class UrlEncodingAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var param1 = filterContext.HttpContext.Request.Params.Get("id");
            var param2 = Convert.ToInt32(filterContext.HttpContext.Request.Params.Get("id"));
            //int ID = Convert.ToInt32(CommonHelper.Decode(id));
            //if (param2 == (int)param2)
            //{
            //    return;
            //}
            //else 
            //{
            //    return Convert.ToInt32(CommonHelper.Decode(param1));
            //}
        }
    }

    public class WordDocumentAttribute : ActionFilterAttribute
    {
        public string DefaultFilename { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var result = filterContext.Result as ViewResult;

            if (result != null)
                result.MasterName = "~/Views/Shared/_PrintChallan.cshtml";

            filterContext.Controller.ViewBag.WordDocumentMode = true;

            base.OnActionExecuted(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var filename = filterContext.Controller.ViewBag.WordDocumentFilename;
            filename = filename ?? DefaultFilename ?? "Document";

            filterContext.HttpContext.Response.AppendHeader("Content-Disposition", string.Format("filename={0}.doc", filename));
            filterContext.HttpContext.Response.ContentType = "application/msword";

            base.OnResultExecuted(filterContext);
        }
    }
}