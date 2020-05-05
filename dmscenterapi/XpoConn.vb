
Imports System
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports DevExpress.Xpo.Metadata
Imports System.Configuration
Imports System.Threading

Public NotInheritable Class XpoHelper
    Public Shared Function GetNewSession() As Session
        Return New Session(DataLayer)
    End Function

    Public Shared Function GetNewUnitOfWork() As UnitOfWork
        Return New UnitOfWork(DataLayer)
    End Function

    Private Shared lockObject As New Object()

    Private Shared fDataLayer As IDataLayer
    Private Shared ReadOnly Property DataLayer() As IDataLayer
        Get
            If fDataLayer Is Nothing Then
                SyncLock lockObject
                    If Thread.VolatileRead(fDataLayer) Is Nothing Then
                        Thread.VolatileWrite(fDataLayer, GetDataLayer())
                    End If
                End SyncLock
            End If
            Return fDataLayer
        End Get
    End Property

    Private Shared Function GetDataLayer() As IDataLayer
        XpoDefault.Session = Nothing

        Dim conn As String = ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
        conn = XpoDefault.GetConnectionPoolString(conn)
        Dim dict As XPDictionary = New ReflectionDictionary()
        Dim store As IDataStore = XpoDefault.GetConnectionProvider(conn, AutoCreateOption.SchemaAlreadyExists)
        dict.GetDataStoreSchema(System.Reflection.Assembly.GetExecutingAssembly())
        Dim dl As IDataLayer = New ThreadSafeDataLayer(dict, store)
        Return dl
    End Function
End Class

