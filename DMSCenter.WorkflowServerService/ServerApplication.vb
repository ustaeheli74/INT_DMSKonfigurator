Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports DevExpress.ExpressApp.Security
Imports DevExpress.Data.Filtering
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Xpo

Namespace DMSCenter.WorkflowServerService
    Public Class ServerApplication
        Inherits XafApplication
        Protected Overrides Sub CreateDefaultObjectSpaceProvider(args As DevExpress.ExpressApp.CreateCustomObjectSpaceProviderEventArgs)
            args.ObjectSpaceProvider = New XPObjectSpaceProvider(args.ConnectionString, args.Connection, True)
        End Sub
        Protected Overrides Function CreateLayoutManagerCore(ByVal simple As Boolean) As DevExpress.ExpressApp.Layout.LayoutManager
            Return Nothing
        End Function
    End Class
End Namespace