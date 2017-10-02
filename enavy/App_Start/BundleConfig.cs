using System.Web;
using System.Web.Optimization;

namespace enavy
{
    public class BundleConfig
    {
        // Pour plus d’informations sur le Bundling, accédez à l’adresse http://go.microsoft.com/fwlink/?LinkId=254725 (en anglais)
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Utilisez la version de développement de Modernizr pour développer et apprendre. Puis, lorsque vous êtes
            // prêt pour la production, utilisez l’outil de génération sur http://modernizr.com pour sélectionner uniquement les tests dont vous avez besoin.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/materialstyl").Include("~/Content/assets/css/materialize.css",
                "~/Content/assets/css/style.css", "~/Content/assets/css/layouts/style-horizontal.css",
                "~/Content/assets/css/custom/ui-notification.css", "~/Content/assets/css/custom/angular-moment-picker.css"));
            bundles.Add(new ScriptBundle("~/bundles/materialjs").Include(
                        "~/Content/assets/js/plugins/jquery-1.11.2.min.js",
                        "~/Content/assets/js/materialize.js",
                        "~/Content/assets/js/plugins/angular.js",
                        "~/Content/assets/js/plugins/angular-materialize.js",
                        "~/Content/assets/js/angular-animate.js",
                        "~/Content/assets/js/plugins/perfect-scrollbar/perfect-scrollbar.js",
                        "~/Content/assets/js/angular-moment-picker.js",
                        "~/Content/assets/js/ui-notification.js"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}