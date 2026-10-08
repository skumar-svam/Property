using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NA.PMS.Model;
using NA.PMS.Service;
using NA.PMS.Service.BusinessRuleEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NA.PMS.Common;

namespace NA.PMS.Web.Areas.Property.Controllers
{
    public class ManagePaymentController : Controller
    {
        private IGeneralService _generalService;
        private IAllotmentService _allotmentService;
        private IPaymentEngine _paymentEngine;

        public ManagePaymentController(IGeneralService generalService, IPaymentEngine paymentEngine, IAllotmentService allotmentService)
        {
            _generalService = generalService;
            _paymentEngine = paymentEngine;
            _allotmentService = allotmentService;
        }

        // GET: Property/ManagePayment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PaymentList()
        {
            return View();
        }

        public ActionResult Payment(string id)
        {
            if (id != null)
            {
                var ridNo = CommonHelper.Decode(id);
            }
            return View();
        }

        public ActionResult PaymentDetail()
        {
            return View();
        }

        public ActionResult Installment()
        {
            return View();
        }

        public JsonResult GetPartialOrFullPaymentDetails([DataSourceRequest]DataSourceRequest request, string departmentId, string isPremiumPaid, string isLeaseRentPaid)
        {
            DataSourceResult detail = _paymentEngine.GetPartialOrFullPaymentDetails(request, departmentId, isPremiumPaid, isLeaseRentPaid);
            return Json(detail, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetPropertyPaymentDetails_Read([DataSourceRequest]DataSourceRequest request)
        {
            var paymentDetails = _paymentEngine.GetPropertyPaymentDetails_Read(request);
            //var data = rentList.ToDataSourceResult(request);
            return Json(paymentDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SavePaymentByRegistrationId(PaymentViewModel model)
        {
            PaymentViewModel data = _paymentEngine.SavePaymentByRegistrationId(model);
            return RedirectToAction("Index");
        }

        public ActionResult GetYesNoStatus()
        {
            var deptList = _generalService.GetYesNoStatus();
            return Json(deptList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDepartmentList()
        {
            var deptList = _generalService.GetAllDepartments();
            return Json(deptList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegistrationIdByDepartment(int? departmentId)
        {
            var ridList = _generalService.GetRegistrationIdByDepartment(departmentId);
            return Json(ridList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertyPaymentType()
        {
            List<DynamicDataModel> ptlist = _paymentEngine.GetPropertyPaymentType();
            return Json(ptlist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPropertySubPaymentType(int receiptId)
        {
            List<DynamicDataModel> pstlist = _paymentEngine.GetPropertySubPaymentType(receiptId);
            return Json(pstlist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBankNamesForPayment([DataSourceRequest] DataSourceRequest request)
        {
            List<DynamicDataModel> banks = _generalService.GetBankNamesForPayment();
            var data = banks.ToDataSourceResult(request);
            return Json(banks, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SavePropertyPayment(PaymentViewModel model)
        {
            PaymentViewModel data = _paymentEngine.SavePropertyPayment(model);
            TempData["MesgAdd"] = data;
            return RedirectToAction("Index");
        }

        #region Payment Schedule
        public ActionResult CreateSchedule()
        {
            PaymentScheduleModel model = new PaymentScheduleModel();
            AlloteeBasicInfo basicInfo = new AlloteeBasicInfo();
            model.AlloteeBasicInfo = basicInfo;
            return View(model);
        }

        public ActionResult GetPaymentSchedule([DataSourceRequest] DataSourceRequest request, int rid)
        {
            var dataresult = _paymentEngine.GetPaymentSchedule(request, rid);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddPaymentSchedule(PaymentScheduleModel model)
        {
            if (model.RegistrationId != null && model.RegistrationId > 0)
            {
                var data = _paymentEngine.SavePaymentSchedule(model);
                if (data == true) return Json(ReturnType.Success, JsonRequestBehavior.AllowGet);
                else return Json(ReturnType.Failed, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(ReturnType.None, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentSchedule_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PaymentScheduleModel> paySch)
        {
            try
            {
                if (paySch != null)
                {
                    foreach (var edit in paySch)
                    {
                        Boolean flag;
                        flag = _paymentEngine.UpdatePaymentSchedule(edit);
                        if (flag == true)
                        { flag = true; }
                        else
                        { ModelState.AddModelError("PaymentSchedule_Update", "Record Not Saved"); }
                    }
                }
                return Json(new[] { paySch }.ToDataSourceResult(request, ModelState));
            }
            catch (Exception ex)
            {
                return RedirectToAction("error", "home");
            }
        }

        public JsonResult GetPayScheduleInformation(int Rid)
        {
            var data = _paymentEngine.GetPayScheduleInfo(Rid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemovePayScheduleInformation(int Rid, int ScheduleId)
        {
            var data = _paymentEngine.RemovePayScheduleInfo(Rid, ScheduleId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult GetRegistrationIdListForPayment([DataSourceRequest] DataSourceRequest request)
        {
            var list = _paymentEngine.GetRegistrationIdListForPayment(request);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentModeList()
        {
            var list = _paymentEngine.GetPaymentModeList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveInstallmentPaymentSchedule(PaymentScheduleModel model)
        {
            if (model.RegistrationId != null && model.RegistrationId != 0)
            {
                var data = _paymentEngine.SaveInstallmentPaymentSchedule(model);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(ReturnType.None,JsonRequestBehavior.AllowGet);
            }            
        }

        public ActionResult GetInstallmentPaymentList([DataSourceRequest] DataSourceRequest request, int rid)
        {
            var dataresult = _paymentEngine.GetInstallmentPaymentListByRegistrationId(request, rid);
            return Json(dataresult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentFrequencyList()
        {
            var list = _paymentEngine.GetPaymentFrequencyList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateInstallmentPayment([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PaymentScheduleModel> paySch)
        {
            try
            {
                if (paySch != null)
                {
                    foreach (var edit in paySch)
                    {
                        Boolean flag;
                        flag = _paymentEngine.UpdatePaymentSchedule(edit);
                        if (flag == true)
                        { flag = true; }
                        else
                        { ModelState.AddModelError("PaymentSchedule_Update", "Record Not Saved"); }
                    }
                }
                return Json(new[] { paySch }.ToDataSourceResult(request, ModelState));
            }
            catch (Exception ex)
            {
                return RedirectToAction("error", "home");
            }
        }
    }
}

