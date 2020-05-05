Imports System.Web.Http

Imports Owin

Public Class WebApiApplication
    Inherits System.Web.HttpApplication

    


    Protected Sub Application_Start()
        Http.GlobalConfiguration.Configure(AddressOf WebApiConfig.Register)
         
    End Sub

    Protected Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
     
    End Sub



End Class
