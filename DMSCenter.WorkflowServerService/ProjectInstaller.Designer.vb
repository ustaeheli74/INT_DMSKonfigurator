Imports Microsoft.VisualBasic
Imports System
Namespace DMSCenter.WorkflowServerService
    Partial Public Class ProjectInstaller
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary> 
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#Region "Component Designer generated code"

        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.serviceProcessInstaller = New System.ServiceProcess.ServiceProcessInstaller()
            Me.serviceInstaller = New System.ServiceProcess.ServiceInstaller()
            ' 
            ' serviceProcessInstaller
            ' 
            Me.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem
            Me.serviceProcessInstaller.Password = Nothing
            Me.serviceProcessInstaller.Username = Nothing
            ' 
            ' serviceInstaller
            ' 
            Me.serviceInstaller.Description = "eXpressApp Framework Workflow Server"
            Me.serviceInstaller.DisplayName = "DMSCenterWorkflowServer"
            Me.serviceInstaller.ServiceName = "DMSCenterWorkflowServer"
            Me.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic
            ' 
            ' ProjectInstaller
            ' 
            Me.Installers.AddRange(New System.Configuration.Install.Installer() {Me.serviceProcessInstaller, Me.serviceInstaller})

        End Sub

#End Region

        Private serviceProcessInstaller As System.ServiceProcess.ServiceProcessInstaller
        Private serviceInstaller As System.ServiceProcess.ServiceInstaller
    End Class
End Namespace
