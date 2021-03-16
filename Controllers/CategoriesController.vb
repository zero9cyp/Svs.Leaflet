Imports System.Web.Mvc
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

Namespace Controllers
    Public Class CategoriesController
        Inherits Controller

        ' GET: Categories
        Function Index() As ActionResult
            Return View()
        End Function

        'Add By Svs
        Public Function [Get]() As PartialViewResult
            Dim arr As String() = {"small", "medium", "large"}
            Return PartialView("_TracksCategories", arr)
        End Function

    End Class
End Namespace