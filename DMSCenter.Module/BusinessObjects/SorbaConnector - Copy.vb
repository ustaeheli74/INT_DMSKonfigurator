Imports System.Threading.Tasks
Imports DevExpress.Data.Filtering
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports RestSharp

Namespace DMSCenter
    Public Class SorbaConnector




        Public Function TransferFiles(ByVal obj As Domain, ByVal strType As String) As Boolean

           TransferFilesSend(obj,"P","Projekte") 
           TransferFilesSend(obj,"A","Adressen") 
           TransferFilesSend(obj,"I","Inventar") 
           TransferFilesSend(obj,"PERS","Personal") 


        End Function


         Public Function TransferFilesSend(ByVal obj As Domain, ByVal strType As String,byval strBaseFolderName As String) As Boolean
            Dim strSorbaWebservice As String = obj.FK_Kunde.SorbaWebservice
            Dim strDomain As String = obj.Name
            Dim strConString As String = strSorbaWebservice & "/webapi/" & strDomain & "/?sFunction=DMS_SENDFILE"
            Dim strDMS As String = obj.FK_Kunde.DMSAdresse
            Dim strDMSUser As String = obj.FK_Kunde.DMSUserTeamfolder
            Dim strDMSPasswort As String = obj.FK_Kunde.DMSPasswortTeamfolder
            Dim DMSClient As New DMSConnector(strDMSUser, strDMSPasswort, strDMS)
            Dim filter As CriteriaOperator = CriteriaOperator.Parse("[Type] = '" & strType & "'")
            obj.DatenObjekts.Filter = filter

            Dim dCollection As XPCollection(Of DatenObjekt) = obj.DatenObjekts
                    
            'Basisfolder erstellen
            DMSClient.CreateFolder("Sorba", "/sorbateamfolder")
            DMSClient.CreateFolder(obj.Name, "/sorbateamfolder/Sorba")
            DMSClient.CreateFolder(strBaseFolderName, "/sorbateamfolder/Sorba/" & obj.Name)

            Dim strBaseFolder = "/sorbateamfolder/Sorba/" & obj.Name & "/" & strBaseFolderName
           

            For Each oDO As DatenObjekt In dCollection

                DMSClient.CreateFolder(oDO.DValue, strBaseFolder)
                Dim strDetailfolder As String = strBaseFolder & "/" & oDO.DValue
                CreateFolders(oDO.FilesnFoldersCollection,oDO.DValue,DMSClient,strDetailfolder,oDO.DValue)

                Dim fCollection As XPCollection(Of FilesnFolders) = oDO.FilesnFoldersCollection

                 CreateFiles(obj, fCollection, strConString, DMSClient, strDetailfolder,strType)

            Next



        End Function




        Public Function CreateFiles(ByVal xpk As Domain, ByVal xpc As XPCollection(Of FilesnFolders), ByVal strConString As String, ByVal dmsc As DMSConnector, ByVal strBase As String,byval strTypeFilter As String) As Task(Of Boolean)
            xpc.Filter = Nothing
            Dim filter As CriteriaOperator = CriteriaOperator.Parse("IsFolder == False")

            Dim strDMS As String = xpk.FK_Kunde.DMSAdresse
            Dim strDMSUser As String = xpk.FK_Kunde.DMSUserTeamfolder
            Dim strDMSPasswort As String = xpk.FK_Kunde.DMSPasswortTeamfolder


            xpc.Filter = filter
            For Each oFF As FilesnFolders In xpc


                Dim client As RestClient = New RestClient(strConString & "&sfp=" & oFF.Path)
                Dim request = New RestRequest(Method.[GET])
                request.AddHeader("cache-control", "no-cache")
                request.AddHeader("Authorization", "Basic YWRtaW46ZWxp")

                Dim response As IRestResponse = client.Execute(request)


                If response.IsSuccessful = True Then

                    Dim objDevice As DMSFile = JsonConvert.DeserializeObject(Of DMSFile)(response.Content)
                    If IsNothing(objDevice) = False Then
                        If objDevice.File.Length > 0 Then

                            Dim strOriginalPath As String = oFF.Path
                            Dim strTargetPath As String = strBase & "/" & After(oFF.Path, oFF.FK_DatenObjekt.Path).Replace("\", "/").Replace(oFF.Name,"")



                      Dim uploadSuccess =  dmsc.UploadFileAsync(strTargetPath, oFF.Name, objDevice.File).Result

                        End If


                    End If
                End If



            Next





        End Function

        Public Sub CreateFolders(ByRef xpc As XPCollection(Of FilesnFolders), ByVal strFilter As String, ByRef dmsc As DMSConnector, ByVal strPath As String, ByVal strParent As String)
            xpc.Filter = CriteriaOperator.Parse("IsFolder == True AND [Path] like '%" & LCase(strFilter) & "%' AND [Parent] == '" & UCase(strParent) & "'")
            For Each oFF As FilesnFolders In xpc
                dmsc.CreateFolder(oFF.Name, strPath)
                CreateFolders(xpc, strFilter & "\" & oFF.Name, dmsc, strPath & "/" & oFF.Name, oFF.Name)
            Next

            xpc.Filter = Nothing

        End Sub


        Public Function ImportProjects(ByVal obj As Domain) As Boolean
           ImportData(obj,"DMS_PROJEKTE")
           ImportData(obj,"DMS_ADRESSEN")
           ImportData(obj,"DMS_PERSONAL")
           ImportData(obj,"DMS_PROJEKTE")
           ImportData(obj,"DMS_INVENTAR")
         
        End Function
        Public Function ImportData(ByVal obj As Domain,byval strFunction As String) As Boolean
            Dim strSorbaWebservice As String = obj.FK_Kunde.SorbaWebservice
            Dim strDomain As String = obj.Name

            Dim strConString As String = strSorbaWebservice & "/webapi/" & strDomain & "/?sFunction=" & strFunction
            Dim strOid As String = obj.Oid


            If strSorbaWebservice.Length > 0 And strDomain.Length > 0 Then

                Dim client As RestClient = New RestClient(strConString)
                Dim request = New RestRequest(Method.[GET])
                request.AddHeader("cache-control", "no-cache")
                request.AddHeader("Authorization", "Basic YWRtaW46ZWxp")
                Dim response As IRestResponse = client.Execute(request)

                If response.IsSuccessful = True Then

                    Dim objDevice As List(Of Project) = JsonConvert.DeserializeObject(Of List(Of Project))(response.Content)
                    If IsNothing(objDevice) = False
                        For Each objp As Project In objDevice
                            Dim dpobjas As New DatenObjekt(obj.Session)

                            dpobjas.DValue = objp.DValue
                            dpobjas.ID = objp.ID
                            dpobjas.Path = objp.DPfad
                            dpobjas.ActualDate = DateTime.Now
                            dpobjas.FK_Domain = obj
                            dpobjas.Type = objp.Art
                            dpobjas.Save


                            Dim objFolder As List(Of SorbaFile) = JsonConvert.DeserializeObject(Of List(Of SorbaFile))(objp.DFolders)

                            If IsNothing(objFolder) = False Then

                            For Each objf As SorbaFile In objFolder
                                Dim dfileobj As New FilesnFolders(obj.Session)
                                dfileobj.ActualDate = DateTime.Now
                                dfileobj.FK_DatenObjekt = dpobjas
                                dfileobj.IsFolder = objf.IsDirectory
                                dfileobj.Name = objf.Name
                                dfileobj.Path = objf.Path
                                dfileobj.Parent = objf.Parent
                                dfileobj.Status = 0
                                dfileobj.Save
                            Next
                            End if




                        Next

                        obj.Session.CommitTransaction

                    End If


                    Return True

                Else

                    Return False

                End If

            End If

        End Function



        Function Before(value As String, a As String) As String
            ' Get index of argument and return substring up to that point.
            Dim posA As Integer = value.IndexOf(a)
            If posA = -1 Then
                Return ""
            End If
            Return value.Substring(0, posA)
        End Function

        Function After(value As String, a As String) As String
            ' Get index of argument and return substring after its position.
            Dim posA As Integer = value.LastIndexOf(a)
            If posA = -1 Then
                Return ""
            End If
            Dim adjustedPosA As Integer = posA + a.Length
            If adjustedPosA >= value.Length Then
                Return ""
            End If
            Return value.Substring(adjustedPosA)
        End Function
        Function Between(value As String, a As String,
                        b As String) As String
            ' Get positions for both string arguments.
            Dim posA As Integer = value.IndexOf(a)
            Dim posB As Integer = value.LastIndexOf(b)
            If posA = -1 Then
                Return ""
            End If
            If posB = -1 Then
                Return ""
            End If

            Dim adjustedPosA As Integer = posA + a.Length
            If adjustedPosA >= posB Then
                Return ""
            End If

            ' Get the substring between the two positions.
            Return value.Substring(adjustedPosA, posB - adjustedPosA)
        End Function


    End Class

    Public Class DMSFile
        Public Property File As String
        Public Property Filesize As String

    End Class

    Public Class Projects
        Public Property Projects As List(Of Project)
    End Class
    Public Class Project
        Public Property Art As String
        Public Property ID As String
        Public Property DValue As String
        Public Property DPfad As String
        Public Property DFolders As String
    End Class



    Public Class SorbaFile
        Public Property Name As String
        Public Property Path As String

        Public Property Parent As String

        Public Property IsDirectory As Boolean

    End Class

End Namespace
