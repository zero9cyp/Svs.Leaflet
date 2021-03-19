Imports System
Imports System.Linq
Imports System.Web.Mvc
Imports Nss.Models
Public Class HomeController
    Inherits System.Web.Mvc.Controller


    Private Shared ReadOnly _log As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    Public Function Index() As ActionResult
        Using db As NssDbEntities = New NssDbEntities()
            Try
                Dim dataSources As String() = db.vwCurrentSituations.AsNoTracking().Where(Function(m) m.DataSource.Equals("ACSS-TRACKS")).[Select](Function(m) m.Category).Distinct().ToArray()
                ViewBag.dataSources = dataSources
            Catch ex As Exception
                _log.Fatal("GetRealTimeTracks()", ex)
            End Try
        End Using

        Return View()
    End Function

    Public Function About() As ActionResult
        ViewBag.Message = "Your application description page."
        Return View()
    End Function

    Public Function Contact() As ActionResult
        ViewBag.Message = "Your contact page."
        Return View()
    End Function

    Public Function TestView1() As ActionResult
        Return View()
    End Function

End Class
