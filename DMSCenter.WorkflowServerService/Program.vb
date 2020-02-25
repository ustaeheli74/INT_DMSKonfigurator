Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.ServiceProcess
Imports System.Text

Namespace DMSCenter.WorkflowServerService
    Friend NotInheritable Class Program
        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        Private Sub New()
        End Sub
        Shared Sub Main()
            Dim ServicesToRun() As ServiceBase
            ServicesToRun = New ServiceBase() {New DMSCenterWorkflowServer()}
            ServiceBase.Run(ServicesToRun)
        End Sub
    End Class
End Namespace
