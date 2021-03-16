Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Http.Extensions.Compression.Core.Compressors
Imports System.Web.Http
Imports Microsoft.AspNet.WebApi.Extensions.Compression.Server
Public Module WebApiConfig
    Public Sub Register(ByVal config As HttpConfiguration)
        ' Web API configuration and services

        ' Web API routes
        config.MapHttpAttributeRoutes()
        'Add by Svs to meet Cac's
        config.MessageHandlers.Insert(0, New ServerCompressionHandler(New GZipCompressor(), New DeflateCompressor()))
        '
        config.Routes.MapHttpRoute(
            name:="DefaultApi",
            routeTemplate:="api/{controller}/{id}",
            defaults:=New With {.id = RouteParameter.Optional}
        )
    End Sub
End Module
