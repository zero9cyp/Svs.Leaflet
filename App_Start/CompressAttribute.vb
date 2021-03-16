Imports System
Imports System.Collections.Generic
Imports System.IO.Compression
Imports System.Linq
Imports System.Net.Http.Headers
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Web
Imports System.Web.Http.Controllers
Imports System.Web.Mvc

Public Class CompressAttribute
    Inherits ActionFilterAttribute


    Public Overrides Sub OnResultExecuting(ByVal filterContext As ResultExecutingContext)
        Dim request As HttpRequestBase = filterContext.HttpContext.Request
        Dim acceptEncoding As String = request.Headers("Accept-Encoding")

        If String.IsNullOrEmpty(acceptEncoding) Then
            Return
        End If

        acceptEncoding = acceptEncoding.ToLowerInvariant()
        Dim response As HttpResponseBase = filterContext.HttpContext.Response

        If acceptEncoding.Contains("gzip") Then
            response.AppendHeader("Content-encoding", "gzip")
            response.Filter = New GZipStream(response.Filter, CompressionMode.Compress)
        ElseIf acceptEncoding.Contains("deflate") Then
            response.AppendHeader("Content-encoding", "deflate")
            response.Filter = New DeflateStream(response.Filter, CompressionMode.Compress)
        End If
    End Sub

End Class
