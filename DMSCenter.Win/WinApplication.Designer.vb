Imports Microsoft.VisualBasic
Imports System

Partial Public Class DMSCenterWindowsFormsApplication
	''' <summary> 
	''' Required designer variable.
	''' </summary>
	Private components As System.ComponentModel.IContainer = Nothing

	''' <summary> 
	''' Clean up any resources being used.
	''' </summary>
	''' <param name="disposing">true if managed resources should be disposed otherwise, false.</param>
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
		Me.module1 = New DevExpress.ExpressApp.SystemModule.SystemModule()
        Me.module2 = New DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule()
		Me.module3 = New Global.DMSCenter.Module.DMSCenterModule()
		Me.module4 = New Global.DMSCenter.Module.Win.DMSCenterWindowsFormsModule()
        Me.securityModule1 = New DevExpress.ExpressApp.Security.SecurityModule()
        Me.securityStrategyComplex1 = New DevExpress.ExpressApp.Security.SecurityStrategyComplex()
        Me.securityStrategyComplex1.SupportNavigationPermissionsForTypes = False
        Me.auditTrailModule = New DevExpress.ExpressApp.AuditTrail.AuditTrailModule()
        Me.objectsModule = New DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule()
        Me.chartModule = New DevExpress.ExpressApp.Chart.ChartModule()
        Me.chartWindowsFormsModule = New DevExpress.ExpressApp.Chart.Win.ChartWindowsFormsModule()
        Me.cloneObjectModule = New DevExpress.ExpressApp.CloneObject.CloneObjectModule()
        Me.conditionalAppearanceModule = New DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule()
        Me.fileAttachmentsWindowsFormsModule = New DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule()
        Me.officeWindowsFormsModule = New DevExpress.ExpressApp.Office.Win.OfficeWindowsFormsModule()
        Me.reportsModuleV2 = New DevExpress.ExpressApp.ReportsV2.ReportsModuleV2()
        Me.reportsWindowsFormsModuleV2 = New DevExpress.ExpressApp.ReportsV2.Win.ReportsWindowsFormsModuleV2()
        Me.validationModule = New DevExpress.ExpressApp.Validation.ValidationModule()
        Me.validationWindowsFormsModule = New DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule()
        Me.viewVariantsModule = New DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule()
        Me.workflowModule = New DevExpress.ExpressApp.Workflow.WorkflowModule()
        Me.workflowWindowsFormsModule = New DevExpress.ExpressApp.Workflow.Win.WorkflowWindowsFormsModule()
        Me.authenticationStandard1 = New DevExpress.ExpressApp.Security.AuthenticationStandard()
		CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        ' 
        ' securityStrategyComplex1
        ' 
        Me.securityStrategyComplex1.Authentication = Me.authenticationStandard1
        Me.securityStrategyComplex1.RoleType = GetType(DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyRole)
        Me.securityStrategyComplex1.UserType = GetType(DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyUser)
        ' 
        ' securityModule1
        ' 
        Me.securityModule1.UserType = GetType(DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyUser)
        ' 
        ' authenticationStandard1
        ' 
        Me.authenticationStandard1.LogonParametersType = GetType(DevExpress.ExpressApp.Security.AuthenticationStandardLogonParameters)
        '
        ' auditTrailModule
        '
        Me.auditTrailModule.AuditDataItemPersistentType = GetType(DevExpress.Persistent.BaseImpl.AuditDataItemPersistent)
        '
        ' reportsModuleV2
        '
        Me.reportsModuleV2.EnableInplaceReports = True
        Me.reportsModuleV2.ReportDataType = GetType(DevExpress.Persistent.BaseImpl.ReportDataV2)
        Me.reportsModuleV2.ShowAdditionalNavigation = False
        Me.reportsModuleV2.ReportStoreMode = DevExpress.ExpressApp.ReportsV2.ReportStoreModes.XML
        '
        ' workflowModule
        '
        Me.workflowModule.RunningWorkflowInstanceInfoType = GetType(DevExpress.ExpressApp.Workflow.Xpo.XpoRunningWorkflowInstanceInfo)
        Me.workflowModule.StartWorkflowRequestType = GetType(DevExpress.ExpressApp.Workflow.Xpo.XpoStartWorkflowRequest)
        Me.workflowModule.UserActivityVersionType = GetType(DevExpress.ExpressApp.Workflow.Versioning.XpoUserActivityVersion)
        Me.workflowModule.WorkflowControlCommandRequestType = GetType(DevExpress.ExpressApp.Workflow.Xpo.XpoWorkflowInstanceControlCommandRequest)
        Me.workflowModule.WorkflowDefinitionType = GetType(DevExpress.ExpressApp.Workflow.Xpo.XpoWorkflowDefinition)
        Me.workflowModule.WorkflowInstanceKeyType = GetType(DevExpress.Workflow.Xpo.XpoInstanceKey)
        Me.workflowModule.WorkflowInstanceType = GetType(DevExpress.Workflow.Xpo.XpoWorkflowInstance)
        ' 
		' DMSCenterWindowsFormsApplication
		' 
        Me.ApplicationName = "DMSCenter"
        Me.CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema
        Me.Modules.Add(Me.module1)
		Me.Modules.Add(Me.module2)
		Me.Modules.Add(Me.module3)
		Me.Modules.Add(Me.module4)
        Me.Modules.Add(Me.securityModule1)
        Me.Security = Me.securityStrategyComplex1
        Me.Modules.Add(Me.auditTrailModule)
        Me.Modules.Add(Me.objectsModule)
        Me.Modules.Add(Me.chartModule)
        Me.Modules.Add(Me.chartWindowsFormsModule)
        Me.Modules.Add(Me.cloneObjectModule)
        Me.Modules.Add(Me.conditionalAppearanceModule)
        Me.Modules.Add(Me.fileAttachmentsWindowsFormsModule)
        Me.Modules.Add(Me.officeWindowsFormsModule)
        Me.Modules.Add(Me.reportsModuleV2)
        Me.Modules.Add(Me.reportsWindowsFormsModuleV2)
        Me.Modules.Add(Me.validationModule)
        Me.Modules.Add(Me.validationWindowsFormsModule)
        Me.Modules.Add(Me.viewVariantsModule)
        Me.Modules.Add(Me.workflowModule)
        Me.Modules.Add(Me.workflowWindowsFormsModule)
        Me.UseOldTemplates = False

		CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

	End Sub

#End Region

	Private module1 As DevExpress.ExpressApp.SystemModule.SystemModule
    Private module2 As DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule
	Private module3 As Global.DMSCenter.Module.DMSCenterModule
    Private module4 As Global.DMSCenter.Module.Win.DMSCenterWindowsFormsModule
    Private securityModule1 As DevExpress.ExpressApp.Security.SecurityModule
    Private securityStrategyComplex1 As DevExpress.ExpressApp.Security.SecurityStrategyComplex
    Private authenticationStandard1 As DevExpress.ExpressApp.Security.AuthenticationStandard
    Private auditTrailModule As DevExpress.ExpressApp.AuditTrail.AuditTrailModule
    Private objectsModule As DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule
    Private chartModule As DevExpress.ExpressApp.Chart.ChartModule
    Private chartWindowsFormsModule As DevExpress.ExpressApp.Chart.Win.ChartWindowsFormsModule
    Private cloneObjectModule As DevExpress.ExpressApp.CloneObject.CloneObjectModule
    Private conditionalAppearanceModule As DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule
    Private fileAttachmentsWindowsFormsModule As DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule
    Private officeWindowsFormsModule As DevExpress.ExpressApp.Office.Win.OfficeWindowsFormsModule
    Private reportsModuleV2 As DevExpress.ExpressApp.ReportsV2.ReportsModuleV2
    Private reportsWindowsFormsModuleV2 As DevExpress.ExpressApp.ReportsV2.Win.ReportsWindowsFormsModuleV2
    Private validationModule As DevExpress.ExpressApp.Validation.ValidationModule
    Private validationWindowsFormsModule As DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule
    Private viewVariantsModule As DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule
    Private workflowModule As DevExpress.ExpressApp.Workflow.WorkflowModule
    Private workflowWindowsFormsModule As DevExpress.ExpressApp.Workflow.Win.WorkflowWindowsFormsModule
End Class
