Imports Microsoft.VisualBasic
Imports System
Imports System.Configuration
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Diagnostics
Imports System.Linq
Imports System.ServiceProcess
Imports System.Text
Imports System.ServiceModel
Imports DevExpress.ExpressApp.Workflow.Server
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Security
Imports DevExpress.Data.Filtering
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.Persistent.Base
Imports DevExpress.ExpressApp.Workflow
Imports DevExpress.Workflow
Imports DevExpress.ExpressApp.MiddleTier

Namespace DMSCenter.WorkflowServerService
    Partial Public Class DMSCenterWorkflowServer
        Inherits System.ServiceProcess.ServiceBase
        Private server As WorkflowServer

        Protected Overrides Sub OnStart(ByVal args() As String)
            If (server Is Nothing) Then
                Dim _application As ServerApplication = New ServerApplication()
                _application.ApplicationName = "DMSCenter"

                _application.CheckCompatibilityType = CheckCompatibilityType.DatabaseSchema
				_application.Modules.Add(New WorkflowModule())
				_application.Modules.Add(New Global.DMSCenter.Module.DMSCenterModule())
				_application.Modules.Add(New Global.DMSCenter.Module.Win.DMSCenterWindowsFormsModule())
				_application.Modules.Add(New Global.DMSCenter.Module.Web.DMSCenterAspNetModule())
                If (Not ConfigurationManager.ConnectionStrings.Item("ConnectionString") Is Nothing) Then
                    _application.ConnectionString = ConfigurationManager.ConnectionStrings.Item("ConnectionString").ConnectionString
                End If

                _application.Setup()
                _application.Logon()

                WorkflowCreationKnownTypesProvider.AddKnownType(GetType(DevExpress.Xpo.Helpers.IdList))
                server = New WorkflowServer("http://localhost:46232", New BasicHttpBinding(), _application.ObjectSpaceProvider, _application.ObjectSpaceProvider)

                server.StartWorkflowListenerService.DelayPeriod = TimeSpan.FromSeconds(15)
                server.StartWorkflowByRequestService.DelayPeriod = TimeSpan.FromSeconds(15)
                server.RefreshWorkflowDefinitionsService.DelayPeriod = TimeSpan.FromMinutes(15)

                AddHandler server.CustomizeHost, Function(sender, e) CustomizeHostHandler(sender, e)
                AddHandler server.CustomHandleException, Function(sender, e) CustomHandleExceptionHandler(sender, e)
            End If

            server.Start()
        End Sub
        Protected Overrides Sub OnStop()
            server.Stop()
        End Sub
        Public Sub New()
            InitializeComponent()
        End Sub

        Private Function CustomizeHostHandler(ByVal sender As Object, ByVal e As CustomizeHostEventArgs) As Boolean
            e.WorkflowInstanceStoreBehavior.WorkflowInstanceStore.RunnableInstancesDetectionPeriod = TimeSpan.FromSeconds(15)
            Return True
        End Function

        Private Function CustomHandleExceptionHandler(ByVal sender As Object, ByVal e As CustomHandleServiceExceptionEventArgs) As Boolean
            Tracing.Tracer.LogError(e.Exception)
            e.Handled = False
            Return True
        End Function
    End Class
End Namespace
