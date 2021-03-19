Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc
Imports System.Web.Routing

Public Module RouteConfig
    Public Sub RegisterRoutes(ByVal routes As RouteCollection)
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")

        routes.MapRoute(
            name:="Default",
            url:="{controller}/{action}/{id}",
            defaults:=New With {.controller = "Home", .action = "Index", .id = UrlParameter.Optional}
        )

        'Add by Svs to meet Cac's
        routes.MapRoute(name:="FilteredTracks",
       url:="TracksController/{action}/{id}",
       defaults:=New With {Key .controller = "TracksController", Key .action = "GetFilteredTracks", Key .id = UrlParameter.[Optional]
          })

    End Sub
End Module