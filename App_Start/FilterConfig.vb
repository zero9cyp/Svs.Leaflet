Imports System.Web
Imports System.Web.Mvc

Public Module FilterConfig
    Public Sub RegisterGlobalFilters(ByVal filters As GlobalFilterCollection)
        filters.Add(New HandleErrorAttribute())
        'Add by Svs to meet cac's
        filters.Add(New ProhibiteBrowserAttribute())
    End Sub
End Module