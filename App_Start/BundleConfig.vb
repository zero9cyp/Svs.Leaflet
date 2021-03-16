Imports System.Web.Optimization

Public Module BundleConfig
    ' For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
    Public Sub RegisterBundles(ByVal bundles As BundleCollection)

        bundles.Add(New ScriptBundle("~/bundles/jquery").Include(
                    "~/Scripts/jquery-{version}.js"))

        bundles.Add(New ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Scripts/jquery.validate*"))

        ' Use the development version of Modernizr to develop with and learn from. Then, when you're
        ' ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
        bundles.Add(New ScriptBundle("~/bundles/modernizr").Include(
                    "~/Scripts/modernizr-*"))

        bundles.Add(New ScriptBundle("~/bundles/bootstrap").Include(
                  "~/Scripts/bootstrap.js"))

        bundles.Add(New StyleBundle("~/Content/css").Include(
                  "~/Content/bootstrap.css",
                  "~/Content/site.css"))

        'Add by Svs To meet Cac's bundle
        bundles.Add(New StyleBundle("~/content/css").Include("~/content/leaflet.css", "~/content/bootstrap.css", "~/content/Control.FullScreen.css", "~/content/L.Control.Layers.Tree.css", "~/content/L.Control.SlideMenu.css", "~/content/leaflet.Coordinates-{version}.css", "~/content/leaflet.fusesearch.css", "~/content/leaflet.PolylineMeasure.css", "~/content/flags-all.css", "~/content/font-awesome.css", "~/content/RobotoCondensed.css", "~/content/site.css"))
        bundles.Add(New ScriptBundle("~/scripts/js").Include("~/scripts/jquery-{version}.js", "~/scripts/leaflet.js", "~/scripts/umd/popper.js", "~/scripts/bootstrap.js", "~/scripts/fuse.min.js", "~/scripts/bundle.js", "~/scripts/Control.FullScreen.js", "~/scripts/L.Control.Layers.Tree.js", "~/scripts/L.Control.SlideMenu.js", "~/scripts/L.Graticule.js", "~/scripts/leaflet.Coordinates-{version}.js", "~/scripts/leaflet.fusesearch.js", "~/scripts/leaflet.PolylineMeasure.js", "~/scripts/leaflet.rotatedmarker.js", "~/scripts/leaflet.latlng-graticule.js", "~/scripts/milsymbol.js", "~/scripts/moment.js"))
        bundles.Add(New ScriptBundle("~/scripts/validation").Include("~/scripts/jquery.validate*"))


    End Sub
End Module

