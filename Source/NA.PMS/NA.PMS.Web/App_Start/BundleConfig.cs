using System.Web.Optimization;
namespace NA.PMS.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));



            // The Kendo js bundle
            bundles.Add(new ScriptBundle("~/bundles/KendoJS").Include(
                    "~/Scripts/KendoJS/2016.2/kendo.web.min.js",
                    "~/Scripts/KendoJS/2016.2/kendo.all.min.js",   
                    "~/Scripts/KendoJS/2016.2/jszip.min.js",
                    "~/Scripts/KendoJS/2016.2/kendo.excel.min.js",
                    "~/Scripts/KendoJS/2016.2/kendo.pdf.min.js"                  
                    ));

            // The jQuery bundle
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                            "~/Scripts/jquery-1.10.2.min.js",
                             "~/Scripts/jquery.validate.min.js",
                            "~/Scripts/jquery.validate.unobtrusive.min.js"));

            //,  "~/Scripts/kendo.aspnetmvc.min.js"

            // The others js bundle
            bundles.Add(new ScriptBundle("~/bundles/others").Include(
                            "~/Scripts/bootstrap.min.js",
                            "~/Scripts/alertify.min.js",
                            "~/Scripts/tooltip.js",
                            "~/Scripts/common-validation.js"
                             ));
            // The others js bundle
            //bundles.Add(new ScriptBundle("~/bundles/highcharts").Include(
            //                "~/Scripts/HighCharts/highcharts.js",
            //                "~/Scripts/HighCharts/highcharts-3d.js",
            //                "~/Scripts/HighCharts/exporting.js",
            //                  "~/Scripts/HighCharts/drilldown.js"
            //                 ));

            // The others CSS bundle
            bundles.Add(new StyleBundle("~/Content/others").Include(
                      "~/Content/CSS/bootstrap.min.css",                      
                      "~/Content/alertifycss/alertify.css",
                      "~/Content/alertifycss/themes/default.css",
                      "~/Content/CSS/font-awesome.min.css",
                      "~/Content/CSS/PMSTemplate.css",
                       "~/Content/CSS/Site.css"
                      ));

            bundles.Add(new StyleBundle("~/bundles/KendoCSS").Include(
                    "~/Content/KendoCSS/2016.2/kendo.default.min.css",
                    "~/Content/KendoCSS/2016.2/kendo.common.min.css",
                    "~/Content/KendoCSS/2016.2/kendo.default.mobile.min.css"
                    // "~/Content/KendoCSS/2016.2/kendo.material.min.css"
                    ));
            // Clear all items from the default ignore list to allow minified CSS and JavaScript files to be included in debug mode
            bundles.IgnoreList.Clear();
            // Add back the default ignore list rules sans the ones which affect minified files and debug mode
            bundles.IgnoreList.Ignore("*.intellisense.js");
            bundles.IgnoreList.Ignore("*-vsdoc.js");
            bundles.IgnoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);

            bundles.IgnoreList.Clear();
        }
    }
}