Imports Owin
Imports Microsoft.Owin

<Assembly: OwinStartupAttribute(GetType(Startup))>

Partial Public Class Startup
    Public Sub Configuration(app As IAppBuilder)
        ConfigureAuth(app) 'This is Throwing exeption i 'll see it later
    End Sub
End Class
