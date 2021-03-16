Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.Mvc

Public Class ProhibiteBrowserAttribute
    Inherits ActionFilterAttribute
    Public Sub New()
    End Sub

    Public Overrides Sub OnActionExecuting(ByVal filterContext As ActionExecutingContext)
        Dim userAgent As String = filterContext.HttpContext.Request.Headers("User-Agent")

        If Not String.IsNullOrEmpty(userAgent) Then
            Dim isIEBrowser As Regex = New Regex("([MS]?IE) (\d+)\.(\d+)", RegexOptions.IgnoreCase)
            Dim isTridentEngine As Regex = New Regex("Trident(.*)rv.(\d+)\.(\d+)", RegexOptions.IgnoreCase)
            Dim isEdgeBrowser As Regex = New Regex("(Edge)/(\d+)(?:\.(\d+))?", RegexOptions.IgnoreCase)

            If isIEBrowser.IsMatch(userAgent) OrElse isTridentEngine.IsMatch(userAgent) OrElse isEdgeBrowser.IsMatch(userAgent) Then
                filterContext.Result = New HttpStatusCodeResult(HttpStatusCode.NotFound)
            End If
        End If

        MyBase.OnActionExecuting(filterContext)
    End Sub

End Class
