Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Web.Hosting
Imports System.Web.Http
Imports Microsoft.AspNet.WebApi.Extensions.Compression.Server.Attributes
Imports Svs.Leaflet.MbtilesProvider

Namespace Controllers
    <Compression(Enabled:=False)>
    Public Class TmsController
        Inherits ApiController

        ' GET: Tms
        'Commended by Svs
        '  Function Index() As ActionResult
        ' Return View()
        ' End Function

        'Add by Svs
        Private Shared ReadOnly _log As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

        <Route("api/map/{name}/{level}/{col}/{row}")>
        Public Function [Get](ByVal name As String, ByVal level As Integer, ByVal col As Integer, ByVal row As Integer) As HttpResponseMessage
            Dim response As HttpResponseMessage = New HttpResponseMessage()
            Dim fullPath As String = String.Empty

            Select Case name
                Case "osm"
                    fullPath = HostingEnvironment.MapPath("~/cyprus.mbtiles")
                Case "enc"
                    fullPath = HostingEnvironment.MapPath("~/App_Data/GeoIndex_el.mbtiles")
                Case Else
                    fullPath = HostingEnvironment.MapPath("~/App_Data/cyprus.mbtiles")
            End Select

            Try
                Dim eTag As String = String.Format("""{0:X2}-{1:X8}-{2:X8}""", level, col, row)
                Dim requestedETag As EntityTagHeaderValue = Me.Request.Headers.IfNoneMatch.FirstOrDefault()

                If requestedETag IsNot Nothing AndAlso eTag.Equals(requestedETag.Tag, StringComparison.InvariantCultureIgnoreCase) Then
                    Return New HttpResponseMessage(HttpStatusCode.NotModified)
                End If

                Dim connectionString = String.Format("Data Source={0}", fullPath)
                Dim mbTileProvider = New MbtilesProvider(connectionString)
                Dim tileStream As MemoryStream = mbTileProvider.GetTile(level, col, row)
                response.Content = New StreamContent(tileStream)
                response.Content.Headers.ContentType = New MediaTypeHeaderValue("image/png")
                response.Headers.ETag = New EntityTagHeaderValue(eTag)
                response.Headers.CacheControl = New CacheControlHeaderValue() With {.[Private] = True}
            Catch ex As Exception
                _log.Fatal("Get()", ex)
            End Try

            Return response
        End Function
    End Class
End Namespace