Imports System
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Web
Imports System.Web.Hosting
Imports System.Web.Mvc
Imports Newtonsoft.Json
Imports Nss.Models

Namespace Controllers
    Public Class LayersController
        Inherits Controller

        ' GET: Layers

        ' Commend by Svs
        'Function Index() As ActionResult
        '    Return View()
        'End Function

        'Add by Svs
        Private Shared ReadOnly _log As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

        ''GET: GeoLayer
        <HttpGet>
        <Compress>
        Public Function Index() As ActionResult
            Dim geojsons As IEnumerable(Of String) = System.IO.Directory.EnumerateFiles(HostingEnvironment.MapPath("~/App_Data/GeoJson"), "*.geojson")
            Dim result As List(Of Layer) = New List(Of Layer)()

            Using db As NssDbEntities = New NssDbEntities()

                Try
                    'Add the Layers to DB I don't have the DB ready Yet
                    result = db.Layers.AsNoTracking().Where(Function(m) m.visible = True).OrderBy(Function(m) m.groupname).ThenBy(Function(m) m.sortorder).ToList()
                Catch ex As Exception
                    _log.Fatal("Index()", ex)
                End Try
            End Using

            Return Content(JsonConvert.SerializeObject(result, Formatting.None), "application/json", Encoding.UTF8)
        End Function

        <HttpGet>
        <Compress>
        Public Function Fetch(ByVal id As String) As ActionResult
            Dim virtualPath As String = String.Format("~/App_Data/GeoJson/{0}.geojson", id)
            Dim fullPath As String = HostingEnvironment.MapPath(virtualPath)

            If System.IO.File.Exists(fullPath) Then
                Dim fi As FileInfo = New FileInfo(fullPath)
                Dim eTag As String = fi.LastWriteTimeUtc.Ticks.ToString("X")
                Dim requestedETag As String = Request.Headers("If-None-Match")

                If eTag.Equals(requestedETag, StringComparison.InvariantCultureIgnoreCase) Then
                    Return New HttpStatusCodeResult(304, "Not Modified")
                End If

                Dim geojson As String = System.IO.File.ReadAllText(fullPath)
                Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate)
                Response.Cache.SetETag(eTag)
                Return Content(geojson, "application/json", Encoding.UTF8)
            End If

            Return HttpNotFound("File not found.")
        End Function

    End Class
End Namespace