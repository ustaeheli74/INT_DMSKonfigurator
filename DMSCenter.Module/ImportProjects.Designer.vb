Partial Class ImportProjects

    <System.Diagnostics.DebuggerNonUserCode()> _
    Public Sub New(ByVal Container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support
        Container.Add(Me)

    End Sub

    'Component overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.ImportProjectAction = New DevExpress.ExpressApp.Actions.SimpleAction(Me.components)
        Me.TransferData = New DevExpress.ExpressApp.Actions.SimpleAction(Me.components)
        Me.SimpleAction1 = New DevExpress.ExpressApp.Actions.SimpleAction(Me.components)
        '
        'ImportProjectAction
        '
        Me.ImportProjectAction.Caption = "Import Projekte"
        Me.ImportProjectAction.Category = "View"
        Me.ImportProjectAction.ConfirmationMessage = Nothing
        Me.ImportProjectAction.Id = "c5bf2e8b-a549-4508-ab8f-0d626487aeb2"
        Me.ImportProjectAction.TargetObjectType = GetType(DMSCenter.Domain)
        Me.ImportProjectAction.ToolTip = Nothing
        '
        'TransferData
        '
        Me.TransferData.Caption = "Daten transferieren"
        Me.TransferData.Category = "View"
        Me.TransferData.ConfirmationMessage = Nothing
        Me.TransferData.Id = "866a6127-f601-44b9-af7e-f948ec3ec609"
        Me.TransferData.TargetObjectType = GetType(DMSCenter.Domain)
        Me.TransferData.ToolTip = Nothing
        '
        'SimpleAction1
        '
        Me.SimpleAction1.Caption = "Benutzer autorisieren"
        Me.SimpleAction1.Category = "View"
        Me.SimpleAction1.ConfirmationMessage = Nothing
        Me.SimpleAction1.Id = "866a6127-f601-44b9-af7e-f948ec3e1609"
        Me.SimpleAction1.TargetObjectType = GetType(DMSCenter.Domain)
        Me.SimpleAction1.ToolTip = Nothing
        '
        'ImportProjects
        '
        Me.Actions.Add(Me.ImportProjectAction)
        Me.Actions.Add(Me.TransferData)
        Me.Actions.Add(Me.SimpleAction1)
        Me.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView

    End Sub

    Friend WithEvents ImportProjectAction As DevExpress.ExpressApp.Actions.SimpleAction
    Friend WithEvents TransferData As DevExpress.ExpressApp.Actions.SimpleAction
    Friend WithEvents SimpleAction1 As DevExpress.ExpressApp.Actions.SimpleAction
End Class
