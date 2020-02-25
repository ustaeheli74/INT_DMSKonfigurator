Imports Microsoft.VisualBasic
Imports System


Partial Public Class DMSCenterAspNetModule
	''' <summary> 
	''' Required designer variable.
	''' </summary>
	Private components As System.ComponentModel.IContainer = Nothing

	''' <summary> 
	''' Clean up any resources being used.
	''' </summary>
	''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		If disposing AndAlso (Not components Is Nothing) Then
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
		'
		'DMSCenterAspNetModule
		'
        me.RequiredModuleTypes.Add(GetType(DMSCenterModule))
        Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule))
		Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.Chart.Web.ChartAspNetModule))
		Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule))
		Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.HtmlPropertyEditor.Web.HtmlPropertyEditorAspNetModule))
		Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.ReportsV2.Web.ReportsAspNetModuleV2))
		Me.RequiredModuleTypes.Add(GetType(DevExpress.ExpressApp.Validation.Web.ValidationAspNetModule))

	End Sub

#End Region
End Class