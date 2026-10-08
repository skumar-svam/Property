using Kendo.Mvc.UI;
using NA.PMS.MTC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NA.PMS.Web.Areas.Reports.Controllers
{
    public class DashboardController : Controller
    {
        //private var _dashboardRepository
        // GET: MasterDashboard/Dashboard
        public ActionResult Index()
        {
            List<SelectListItem> deptList = new List<SelectListItem>();
            //List<SelectListItem> typelist = new List<SelectListItem>();
            List<SelectListItem> requesttype = new List<SelectListItem>();
            deptList.Add(new SelectListItem
            {
                Value = "51",
                Text = "All Department"
            });

            requesttype.Add(new SelectListItem
            {
                Value = "null",
                Text = "All"
            });
            requesttype.Add(new SelectListItem
            {
                Value = "Online",
                Text = "Online"
            });
            requesttype.Add(new SelectListItem
            {
                Value = "JSK",
                Text = "JSK"
            });
            //requesttype.Add(new SelectListItem
            //{
            //    Value = "NIC Nivesh Mitra",
            //    Text = "Nivesh Mitra"
            //});



            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var startDate1 = startDate.ToString("dd-MM-yyyy");
            var enddate = now.ToString("dd-MM-yyyy");
            ViewBag.StartDate = startDate1;
            ViewBag.EndDate = enddate;
            ViewBag.DeptList = deptList;
            ViewBag.RequestType = requesttype;
            return View();
        }

        [HttpPost]
        public JsonResult GetName(int DepartmentID, string StartDate, string EndDate)
        {
            string name = "Gaurav Sharma";
            return Json(name, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDashboardReport(string StartDate, string EndDate, string RequestThrough)
        {
            int DepartmentID = 51;
            MasterDashboardRepository masterDashboardRepository = new MasterDashboardRepository();
            Chart _chart = new Chart();
            List<MasterDashboardModel> lst = new List<MasterDashboardModel>();
            try
            {
                lst = masterDashboardRepository.GetAllReportsDeptWise(DepartmentID, StartDate, EndDate, RequestThrough);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetServiceTimelineReportsAsDataSource([DataSourceRequest]DataSourceRequest request, MasterDashboardModel model)
        {
            MasterDashboardRepository masterDashboardRepository = new MasterDashboardRepository();
            var list = masterDashboardRepository.GetServiceTimelineReportsAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// For Nivesh Mitra in saparate link
        /// </summary>
        /// <returns></returns>
        public ActionResult NiveshMitra()
        {
            List<SelectListItem> deptList = new List<SelectListItem>();
            //List<SelectListItem> typelist = new List<SelectListItem>();
            List<SelectListItem> requesttype = new List<SelectListItem>();
            deptList.Add(new SelectListItem
            {
                Value = "51",
                Text = "All Department"
            });

            //requesttype.Add(new SelectListItem
            //{
            //    Value = "null",
            //    Text = "All"
            //});
            //requesttype.Add(new SelectListItem
            //{
            //    Value = "Online",
            //    Text = "Online"
            //});
            //requesttype.Add(new SelectListItem
            //{
            //    Value = "JSK",
            //    Text = "JSK"
            //});
            requesttype.Add(new SelectListItem
            {
                Value = "NIC Nivesh Mitra",
                Text = "Nivesh Mitra"
            });



            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var startDate1 = startDate.ToString("dd-MM-yyyy");
            var enddate = now.ToString("dd-MM-yyyy");
            ViewBag.StartDate = startDate1;
            ViewBag.EndDate = enddate;
            ViewBag.DeptList = deptList;
            ViewBag.RequestType = requesttype;
            return View();
        }

        #region Account Service Request
        public ActionResult AccountServiceRequest()
        {
            List<SelectListItem> requesttype = new List<SelectListItem>();
            List<SelectListItem> deptList = new List<SelectListItem>();
            deptList.Add(new SelectListItem
            {
                Value = "51",
                Text = "All Department"
            });

            requesttype.Add(new SelectListItem
            {
                Value = "null",
                Text = "All"
            });
            requesttype.Add(new SelectListItem
            {
                Value = "Online",
                Text = "Online"
            });
            requesttype.Add(new SelectListItem
            {
                Value = "JSK",
                Text = "JSK"
            });
            requesttype.Add(new SelectListItem
            {
                Value = "NIC Nivesh Mitra",
                Text = "Nivesh Mitra"
            });

            ViewBag.DeptList = deptList;
            ViewBag.RequestType = requesttype;
            return View();
        }

        public JsonResult GetAccountTypeReport(string StartDate, string EndDate, string RequestThrough)
        {
            int DepartmentID = 51;
            MasterDashboardRepository masterDashboardRepository = new MasterDashboardRepository();
            List<MasterDashboardModel> lst = new List<MasterDashboardModel>();
            try
            {
                lst = masterDashboardRepository.GetAccountTypeReport(DepartmentID, StartDate, EndDate, RequestThrough);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceTimelineAccountReportsAsDataSource([DataSourceRequest]DataSourceRequest request, MasterDashboardModel model)
        {
            MasterDashboardRepository masterDashboardRepository = new MasterDashboardRepository();
            var list = masterDashboardRepository.GetServiceTimelineAccountReportsAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Dues Dashboard
        public ActionResult DuesDashboard()
        {
            return View();
        }
        public JsonResult all_dept_dues_chart()
        {
            MasterDashboardRepository masterDashboardRepository = new MasterDashboardRepository();
            var sp_result = masterDashboardRepository.rsp_defaulter_total_alldept().ToList();
            //var sp_result_install = db.rsp_dues_count_total_alldept(2).ToList();
            //var sp_result_lease_install = db.rsp_dues_count_total_alldept(3).ToList();



            var dept_name = sp_result.Select(b => b.departmentName).Distinct().ToArray();

            var install_data = sp_result.Where(a => a.DuesType.Equals("Installment")).Select(b => b.DuesCount).ToArray();
            var lease_data = sp_result.Where(a => a.DuesType.Equals("lease")).Select(b => b.DuesCount).ToArray();
            var install_lease_data = sp_result.Where(a => a.DuesType.Equals("Installment and lease ")).Select(b => b.DuesCount).ToArray();


            //var query = sp_result.Where(c => c.DuesType.Equals("installment")).GroupBy(a => a.departmentName).AsEnumerable().Select(b => new { dues_count = b.Count() }).ToArray();

            //DateTime lastDate = DateTime.Now.AddDays(-1);
            ////var query = db.sales.Where(c => c.invoice_date.Value.Equals(DateTime.Now)).GroupBy(a => a.comp_ID).AsEnumerable().Select(b => new { COMP_ID = (b.Key), sales_count = b.Count() }).ToList();
            //var query1 = db.sales.Where(c => c.invoice_date.Value.Equals(lastDate)).GroupBy(a => a.comp_ID).AsEnumerable().Select(b => new { COMPID = (b.Key), sales_count_last = b.Count() }).ToList();

            chartdues _chart = new chartdues();

            _chart.labels = dept_name;
            _chart.datasets = new List<datasetsdues>();
            List<datasetsdues> _dataSet = new List<datasetsdues>();
            _dataSet.Add(new datasetsdues()
            {
                label = "Lease & Installment ",
                data = install_lease_data,
                borderColor = new string[] { "#4e73df" },
                backgroundColor = new string[] { "#4e73df", "#4e73df", "#4e73df", "#4e73df", "#4e73df", "#4e73df" },
                //fill = "false",
                borderWidth = "3",
                pointRadius = 5,
                pointHoverRadius = 15

            });
            _dataSet.Add(new datasetsdues()
            {
                label = "Lease",
                data = lease_data,
                borderColor = new string[] { "#bc5090" },
                backgroundColor = new string[] { "#bc5090", "#bc5090", "#bc5090", "#bc5090", "#bc5090", "#bc5090" },
                //borderColor = new string[] { "rgba(75,192,192,0.4)" },
                //backgroundColor = new string[] { "rgba(75,192,192,0.4)" },
                //fill = "false",
                borderWidth = "3",
                pointRadius = 5,
                pointHoverRadius = 15

            });
            _dataSet.Add(new datasetsdues()
            {
                label = "Installment",
                data = install_data,
                borderColor = new string[] { "#ffa600" },
                backgroundColor = new string[] { "#ffa600", "#ffa600", "#ffa600", "#ffa600", "#ffa600", "#ffa600" },
                //fill = "false",
                borderWidth = "3",
                pointRadius = 5,
                pointHoverRadius = 15
            });
            _chart.datasets = _dataSet;
            return Json(_chart, JsonRequestBehavior.AllowGet);
        }
        public JsonResult all_dept_dues_total_chart()
        {
            MasterDashboardRepository masterDashboardRepository = new MasterDashboardRepository();
            var sp_result = masterDashboardRepository.rsp_defaulter_duestotal_alldept().ToList();

            //var dept_name = sp_result.Select(b => b.departmentName).Distinct().ToArray();
            var dues_type = sp_result.Select(b => b.DuesType).Distinct().ToArray();

            var industrial_data = sp_result.Where(a => a.departmentName.Equals("Industrial")).Select(b => b.duestotal).ToArray();

            var commercial_data = sp_result.Where(a => a.departmentName.Equals("Commercial")).Select(b => b.duestotal).ToArray();
            var housing_data = sp_result.Where(a => a.departmentName.Equals("Housing")).Select(b => b.duestotal).ToArray();
            var institutional_data = sp_result.Where(a => a.departmentName.Equals("Institutional")).Select(b => b.duestotal).ToArray();

            var residential_data = sp_result.Where(a => a.departmentName.Equals("Residential")).Select(b => b.duestotal).ToArray();
            var abadi_data = sp_result.Where(a => a.departmentName.Equals("5 % Abadi")).Select(b => b.duestotal).ToArray();

            //var lease_data = sp_result.Where(a => a.DuesType.Equals("lease")).Select(b => b.DuesCount).ToArray();
            //var install_lease_data = sp_result.Where(a => a.DuesType.Equals("Installment and lease ")).Select(b toa b.DuesCount).ToArray();

            chart _achart = new chart();

            _achart.labels = dues_type;
            _achart.datasets = new List<datasetss>();
            List<datasetss> _dataSet = new List<datasetss>();
            _dataSet.Add(new datasetss()
            {
                label = "Industrial",
                data = industrial_data,
                borderColor = new string[] { "#4e73df" },
                backgroundColor = new string[] { "#4e73df", "#4e73df", "#4e73df", "#4e73df", "#4e73df", "#4e73df" },

                //fill = "false",
                borderWidth = "3",
                pointRadius = 5,
                pointHoverRadius = 15


            });
            _dataSet.Add(new datasetss()
            {
                label = "Commercial",
                data = commercial_data,
                borderColor = new string[] { "#ffa600" },
                backgroundColor = new string[] { "#ffa600", "#ffa600", "#ffa600", "#ffa600", "#ffa600", "#ffa600" },
                //borderColor = new string[] { "rgba(75,192,192,0.4)" },
                //backgroundColor = new string[] { "rgba(75,192,192,0.4)" },

                //fill = "false",
                borderWidth = "3",
                pointRadius = 5,
                pointHoverRadius = 15


            });
            _dataSet.Add(new datasetss()
            {
                label = "Housing",
                data = housing_data,
                borderColor = new string[] { "#7a5195" },
                backgroundColor = new string[] { "#7a5195", "#7a5195", "#7a5195", "#7a5195", "#7a5195", "#7a5195" },

                //fill = "false",
                borderWidth = "3",
                pointRadius = 5,
                pointHoverRadius = 15




            });

            _dataSet.Add(new datasetss()
            {
                label = "Institutional",
                data = institutional_data,
                borderColor = new string[] { "#bc5090" },
                backgroundColor = new string[] { "#bc5090", "#bc5090", "#bc5090", "#bc5090", "#bc5090", "#bc5090" },

                //fill = "false",
                borderWidth = "3",
                pointRadius = 5,
                pointHoverRadius = 15


            });
            _dataSet.Add(new datasetss()
            {
                label = "Residential",
                data = residential_data,
                borderColor = new string[] { "#ef5675" },
                backgroundColor = new string[] { "#ef5675", "#ef5675", "#ef5675", "#ef5675", "#ef5675", "#ef5675" },
                //borderColor = new string[] { "rgba(75,192,192,0.4)" },
                //backgroundColor = new string[] { "rgba(75,192,192,0.4)" },

                //fill = "false",
                borderWidth = "3",
                pointRadius = 5,
                pointHoverRadius = 15


            });
            _dataSet.Add(new datasetss()
            {
                label = "5 % abadi",
                data = abadi_data,
                borderColor = new string[] { "#ff764a" },
                backgroundColor = new string[] { "#ff764a", "#ff764a", "#ff764a", "#ff764a", "#ff764a", "#ff764a" },
                //fill = "false",
                borderWidth = "3",
                pointRadius = 5,
                pointHoverRadius = 15

            });

            _achart.datasets = _dataSet;
            return Json(_achart, JsonRequestBehavior.AllowGet);
        }
        public JsonResult deptwise_defaulter_chart_install()
        {
            MasterDashboardRepository masterDashboardRepository = new MasterDashboardRepository();
            var sp_result = masterDashboardRepository.rsp_defaulter_duestotal_alldept().ToList();

            var dept_name = sp_result.Select(b => b.departmentName).Distinct().ToArray();
            //var dues_type = sp_result.Where(a => a.departmentName.Equals("Industrial")).Select( b =>  b.DuesType).Distinct().ToArray();

            //var install_lease_data = sp_result.Where(a => a.DuesType.Equals("Installment and lease ")).Select(b => b.duestotal).ToArray();
            ////var query = db.purchases.Where(c => c.invoice_date.Value.Year.Equals(DateTime.Now.Year)).GroupBy(a => new { a.comp_ID }).AsEnumerable().Select(b => new { COMP_ID = (b.Key.comp_ID), totamt = b.Sum(x => Convert.ToInt32(x.total_amount)), pur_count = b.Count()  }).ToList();
            //var lease_data = sp_result.Where(a => a.DuesType.Equals("lease")).Select(b => b.duestotal).ToArray();
            var installment_data = sp_result.Where(a => a.DuesType.Equals("Installment")).Select(b => b.duestotal).ToArray();

            //var lease_data = sp_result.Where(a => a.DuesType.Equals("lease")).Select(b => b.DuesCount).ToArray();
            //var install_lease_data = sp_result.Where(a => a.DuesType.Equals("Installment and lease ")).Select(b toa b.DuesCount).ToArray();

            chart _achart = new chart();

            _achart.labels = dept_name;
            _achart.datasets = new List<datasetss>();
            List<datasetss> _dataSet = new List<datasetss>();
            _dataSet.Add(new datasetss()
            {
                label = "Install - Dues total",
                data = installment_data,
                borderColor = new string[] { "#ffa600" },
                backgroundColor = new string[] { "#ffa600" },

                fill = "origin",
                borderWidth = "3",
                pointRadius = 5,
                pointHoverRadius = 15


            });
            //_dataSet.Add(new datasetss()
            //{
            //    label = "b",
            //    data = lease_data,
            //    borderColor = new string[] { "rgba(75,192,192,0.4)" },
            //    backgroundColor = new string[] { "rgba(75,192,192,0.4)" },
            //    //borderColor = new string[] { "rgba(75,192,192,0.4)" },
            //    //backgroundColor = new string[] { "rgba(75,192,192,0.4)" },

            //    fill = "origin",
            //    borderWidth = "3",
            //    pointRadius = 5,
            //    pointHoverRadius = 15


            //});
            //_dataSet.Add(new datasetss()
            //{
            //    label = "c",
            //    data = install_lease_data,
            //    borderColor = new string[] { "yellow" },
            //    backgroundColor = new string[] { "yellow" },

            //    fill = "origin",
            //    borderWidth = "3",
            //    pointRadius = 5,
            //    pointHoverRadius = 15




            //});


            _achart.datasets = _dataSet;
            return Json(_achart, JsonRequestBehavior.AllowGet);
        }
        public JsonResult deptwise_defaulter_chart_lease()
        {
            MasterDashboardRepository masterDashboardRepository = new MasterDashboardRepository();
            var sp_result = masterDashboardRepository.rsp_defaulter_duestotal_alldept().ToList();

            var dept_name = sp_result.Select(b => b.departmentName).Distinct().ToArray();

            var lease_data = sp_result.Where(a => a.DuesType.Equals("lease")).Select(b => b.duestotal).ToArray();


            chart _achart = new chart();

            _achart.labels = dept_name;
            _achart.datasets = new List<datasetss>();
            List<datasetss> _dataSet = new List<datasetss>();

            _dataSet.Add(new datasetss()
            {
                label = "Lease - Dues total",
                data = lease_data,
                borderColor = new string[] { "#4e73df" },
                backgroundColor = new string[] { "#4e73df" },
                //borderColor = new string[] { "rgba(75,192,192,0.4)" },
                //backgroundColor = new string[] { "rgba(75,192,192,0.4)" },

                fill = "origin",
                borderWidth = "3",
                pointRadius = 5,
                pointHoverRadius = 15


            });



            _achart.datasets = _dataSet;
            return Json(_achart, JsonRequestBehavior.AllowGet);
        }
        public JsonResult deptwise_defaulter_chart_install_lease()
        {
            MasterDashboardRepository masterDashboardRepository = new MasterDashboardRepository();
            var sp_result = masterDashboardRepository.rsp_defaulter_duestotal_alldept().ToList();

            var dept_name = sp_result.Select(b => b.departmentName).Distinct().ToArray();

            var install_lease_data = sp_result.Where(a => a.DuesType.Equals("Installment and lease ")).Select(b => b.duestotal).ToArray();

            chart _achart = new chart();

            _achart.labels = dept_name;
            _achart.datasets = new List<datasetss>();
            List<datasetss> _dataSet = new List<datasetss>();

            _dataSet.Add(new datasetss()
            {
                label = "Instalment and lease - Dues total",
                data = install_lease_data,
                borderColor = new string[] { "#ef5675" },
                backgroundColor = new string[] { "#ef5675", "#ef5675", "#ef5675", "#ef5675", "#ef5675", "#ef5675" },

                //fill = "false",
                borderWidth = "3",
                pointRadius = 5,
                pointHoverRadius = 15




            });


            _achart.datasets = _dataSet;
            return Json(_achart, JsonRequestBehavior.AllowGet);
        }
        #endregion


        public ActionResult EmployeePendency()
        {
            List<SelectListItem> deptList = new List<SelectListItem>();
            //List<SelectListItem> typelist = new List<SelectListItem>();
            List<SelectListItem> requesttype = new List<SelectListItem>();
            deptList.Add(new SelectListItem { Value = "51", Text = "All Department" });

            requesttype.Add(new SelectListItem { Value = "null", Text = "All" });
            requesttype.Add(new SelectListItem { Value = "Online", Text = "Online" });
            requesttype.Add(new SelectListItem { Value = "JSK", Text = "JSK" });
            requesttype.Add(new SelectListItem { Value = "NIC Nivesh Mitra", Text = "Nivesh Mitra" });

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var startDate1 = startDate.ToString("dd-MM-yyyy");
            var enddate = now.ToString("dd-MM-yyyy");
            ViewBag.StartDate = startDate1;
            ViewBag.EndDate = enddate;
            ViewBag.DeptList = deptList;
            ViewBag.RequestType = requesttype;
            return View();
        }

        public JsonResult GetServicesByEmployeeDashboardReport(string StartDate, string EndDate, string RequestThrough)
        {
            int DepartmentID = 51;
            MasterDashboardRepository masterDashboardRepository = new MasterDashboardRepository();
            Chart _chart = new Chart();
            List<MasterDashboardModel> lst = new List<MasterDashboardModel>();
            try
            {
                //lst = masterDashboardRepository.GetAllReportsDeptWise(DepartmentID, StartDate, EndDate, RequestThrough);
                lst = masterDashboardRepository.GetServicesByEmployeeDashboardReport(DepartmentID, StartDate, EndDate, RequestThrough);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServicesByEmployeeTimelineReportsAsDataSource([DataSourceRequest]DataSourceRequest request, MasterDashboardModel model)
        {
            MasterDashboardRepository masterDashboardRepository = new MasterDashboardRepository();
            var list = masterDashboardRepository.GetServicesByEmployeeTimelineReportsAsDataSource(request, model);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDashboardChartReportByEmployee(string StartDate, string EndDate, string RequestThrough)
        {
            int DepartmentID = 51;
            MasterDashboardRepository masterDashboardRepository = new MasterDashboardRepository();
            Chart _chart = new Chart();
            List<MasterDashboardModel> lst = new List<MasterDashboardModel>();
            try
            {
                lst = masterDashboardRepository.GetDashboardChartReportByEmployee(DepartmentID, StartDate, EndDate, RequestThrough);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
    }
}
