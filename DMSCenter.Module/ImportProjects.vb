Imports Microsoft.VisualBasic
Imports System
Imports System.Linq
Imports System.Text
Imports DevExpress.ExpressApp
Imports DevExpress.Data.Filtering
Imports System.Collections.Generic
Imports DevExpress.Persistent.Base
Imports DevExpress.ExpressApp.Utils
Imports DevExpress.ExpressApp.Layout
Imports DevExpress.ExpressApp.Actions
Imports DevExpress.ExpressApp.Editors
Imports DevExpress.ExpressApp.Templates
Imports DevExpress.Persistent.Validation
Imports DevExpress.ExpressApp.SystemModule
Imports DevExpress.ExpressApp.Model.NodeGenerators
Imports DMSCenter.Module.DMSCenter

' For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
Partial Public Class ImportProjects
    Inherits ViewController
    Public Sub New()
        InitializeComponent()
        ' Target required Views (via the TargetXXX properties) and create their Actions.
    End Sub
    Protected Overrides Sub OnActivated()
        MyBase.OnActivated()
        ' Perform various tasks depending on the target View.
    End Sub
    Protected Overrides Sub OnViewControlsCreated()
        MyBase.OnViewControlsCreated()
        ' Access and customize the target View control.
    End Sub
    Protected Overrides Sub OnDeactivated()
        ' Unsubscribe from previously subscribed events and release other references and resources.
        MyBase.OnDeactivated()
    End Sub

    Private Sub ImportProjectAction_Execute(sender As Object, e As SimpleActionExecuteEventArgs) Handles ImportProjectAction.Execute
        Dim objProj As New SorbaConnector
        Dim objTrack As Domain = CType(View.CurrentObject, Domain)
        If IsNothing(objTrack) = False Then
            objProj.ImportProjects(objTrack)
        End If

    End Sub

    Private Sub TransferData_Execute(sender As Object, e As SimpleActionExecuteEventArgs) Handles TransferData.Execute
        Dim objProj As New SorbaConnector
        Dim objTrack As Domain = CType(View.CurrentObject, Domain)
        If IsNothing(objTrack) = False Then
            objProj.TransferFiles(objTrack)
        End If
    End Sub




    Private Sub SimpleAction1_Execute(sender As Object, e As SimpleActionExecuteEventArgs) Handles SimpleAction1.Execute
        Dim objProj As New SorbaConnector
        Dim objTrack As Domain = CType(View.CurrentObject, Domain)
        If IsNothing(objTrack) = False Then

            objProj.SorbaAutorisierung(objTrack)
        End If
    End Sub
End Class
