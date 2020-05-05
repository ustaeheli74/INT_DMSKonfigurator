Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Text
Imports System.Threading.Tasks
Imports ApiHelper.Model
Imports DevExpress.Data.Filtering
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports NLog
Imports RestSharp
Imports Serilog

Namespace DMSCenter
    Public Class SorbaConnector

        Public _SorbaCredentials As String = ""
        Public Function SorbaAutorisierung(ByRef obj As Domain) As Boolean
            SyncAuthorization(obj)
            Return True
        End Function




        Public Function CreateDMSUser(ByVal obj As Kunde, ByVal strUEmail As String, ByVal strUPassword As String) As String

            Dim strDMS As String = obj.DMSAdresse
            Dim strDMSUser As String = obj.DMSUserTeamfolder
            Dim strDMSPasswort As String = obj.DMSPasswortTeamfolder
            Dim strDMSAdminUser As String = obj.DMSUser
            Dim strDMSAdminPasswort As String = obj.DMSPassword

            Dim DMSClient As New DMSConnector(strDMSUser, strDMSPasswort, strDMS, strDMSAdminUser, strDMSAdminPasswort)
            Dim resp As AddUserPostResponseModel = DMSClient.CreateUser(strUEmail.Replace("@", "").Replace(".", ""), strUPassword, strUEmail)


            Dim strResultat As String = resp.ResponseResult

            Return strResultat

        End Function

        Public Function ActDeactUser(ByVal obj As Kunde, ByVal strUEmail As String, ByVal strPasswort As String, ByVal bAktivieren As Boolean) As String

            Dim strDMS As String = obj.DMSAdresse
            Dim strDMSUser As String = obj.DMSUserTeamfolder
            Dim strDMSPasswort As String = obj.DMSPasswortTeamfolder
            Dim strDMSAdminUser As String = obj.DMSUser
            Dim strDMSAdminPasswort As String = obj.DMSPassword

            Dim DMSClient As New DMSConnector(strDMSUser, strDMSPasswort, strDMS, strDMSAdminUser, strDMSAdminPasswort)
            Dim resp As UpdateUserResponseModel = DMSClient.UpdateUser(strUEmail.Replace("@", "").Replace(".", ""), strUEmail, bAktivieren)
            If bAktivieren = True Then
                Dim resp1 As SetUserPasswordResponseModel = DMSClient.ChangeUserPW(strUEmail.Replace("@", "").Replace(".", ""), strPasswort)
            End If

            Dim strResultat As String = resp.ResponseResult



            Return strResultat

        End Function

        Public Function GetCredentials(ByVal strKdNr As String, ByVal strDomain As String) As String



            If _SorbaCredentials.Length < 1 Then

                Dim dt As DataTable = getSQLCredentials(strKdNr, strDomain)
                If IsNothing(dt) = False Then
                    If dt.Rows.Count > 0 Then
                        Dim credentials As String = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(dt.Rows(0)("Username").ToString & ":" & dt.Rows(0)("Password").ToString))
                        _SorbaCredentials = credentials
                    End If
                End If
            End If
            '
            If _SorbaCredentials.Length < 1 Then
                _SorbaCredentials = "YWRtaW46ZWxp"
            End If

            Return _SorbaCredentials

        End Function



        Public Function getSQLCredentials(ByVal strKdNr As String, strDomain As String) As DataTable
            Using m_con9 As SqlConnection = New SqlConnection("Data Source=SQLSERVER;Initial Catalog=SINFO2009;Persist Security Info=True;User ID=bezugspersonen;Password=12@echoosterhase")
                Try
                    Using m_DA As New SqlDataAdapter()
                        Dim m_DT As New DataTable()
                        If m_con9.State = ConnectionState.Closed Then
                            m_con9.Open()
                        End If
                        m_DT.Clear()
                        Using m_CMD As SqlCommand = New SqlCommand(String.Format("Select * from fnGetMySCompanyAdmins({0},'{1}')", strKdNr, strDomain), m_con9)
                            m_DA.SelectCommand = m_CMD
                        End Using
                        m_DA.Fill(m_DT)
                        Return m_DT
                    End Using
                Catch ex As Exception

                    Return Nothing
                End Try
            End Using
        End Function


        Public Function ChangePWDMSUser(ByVal obj As Kunde, ByVal strUEmail As String, ByVal strUPassword As String) As String

            Dim strDMS As String = obj.DMSAdresse
            Dim strDMSUser As String = obj.DMSUserTeamfolder
            Dim strDMSPasswort As String = obj.DMSPasswortTeamfolder
            Dim strDMSAdminUser As String = obj.DMSUser
            Dim strDMSAdminPasswort As String = obj.DMSPassword

            Dim DMSClient As New DMSConnector(strDMSUser, strDMSPasswort, strDMS, strDMSAdminUser, strDMSAdminPasswort)
            Dim resp As SetUserPasswordResponseModel = DMSClient.ChangeUserPW(strUEmail.Replace("@", "").Replace(".", ""), strUPassword)

            Dim strResultat As String = resp.ResponseResult

            Return strResultat

        End Function

        Public Function SyncAuthorization(ByRef obj As Domain)

            'Benötige Gruppen erstellen
            Dim strDMS As String = obj.FK_Kunde.DMSAdresse
            Dim strDMSUser As String = obj.FK_Kunde.DMSUserTeamfolder
            Dim strDMSPasswort As String = obj.FK_Kunde.DMSPasswortTeamfolder
            Dim strDMSAdminUser As String = obj.FK_Kunde.DMSUser
            Dim strDMSAdminPasswort As String = obj.FK_Kunde.DMSPassword

            Dim DMSClient As New DMSConnector(strDMSUser, strDMSPasswort, strDMS, strDMSAdminUser, strDMSAdminPasswort)
           
            ImportSorbaUser(obj)
            SetGroupMembershipProjekte(obj, DMSClient)
            SetGroupMembership(obj, DMSClient, "M_AQ", "Adressen")
            SetGroupMembership(obj, DMSClient, "M_MQ", "Inventar")
            SetGroupMembership(obj, DMSClient, "M_FIBU", "Fibu")
            SetGroupMembershipPersonal(obj, DMSClient)

        End Function

        Public Function ArchiveProjects(ByVal obj As Domain, ByVal DMSCLient As DMSConnector)
            ImportData(obj, "DMS_PROJEKTE", "P", "&sOption=true")
            Dim strBaseFolder = "/sorbateamfolder/Sorba/" & obj.Name & "/Projekte/"
            Dim strBaseFolderArchiv = "/sorbateamfolder/SorbaArchiv/" & obj.Name & "/Projekte/"
            Dim listEntries As List(Of Entry) = DMSCLient.GetFolders(strBaseFolder)

            For Each item As Entry In listEntries
                'item.Name

                Try

                    Dim filter As CriteriaOperator = CriteriaOperator.Parse("[Type] = 'P' AND [DValue] ='" & item.Name & "'")
                    obj.DatenObjekts.Filter = filter
                    Dim dCollection As XPCollection(Of DatenObjekt) = obj.DatenObjekts
                    If dCollection.Count = 0 Then
                        DMSCLient.RenameFolderArchiv(strBaseFolder & item.Name, strBaseFolderArchiv & item.Name)
                    Else
                        Dim strtest As String = ""

                    End If

                Catch ex As Exception

                End Try



            Next






        End Function

        Public Sub SetGroupMembership(ByVal obj As Domain, ByVal DMSCLient As DMSConnector, ByVal strFilter As String, ByVal strModul As String)
            Dim dCollection As XPCollection(Of SorbaUsers) = obj.SorbaUsersCollection

            dCollection.Filter = Nothing
            Dim filter As CriteriaOperator = CriteriaOperator.Parse("MYSUSER <> '' AND " & strFilter & " == True")
            dCollection.Filter = filter

            Dim strGroupID As String

            strGroupID = DMSCLient.GetGroupId(RemoveDiacritics(obj.Name) & "_" & strModul)

            If strGroupID.Length > 0 Then
                Dim listNames As List(Of GetMembersForGroupMember) = DMSCLient.GetGroupMembers(strGroupID)

                For Each lobj In listNames
                    DMSCLient.DeleteGroupMembers(strGroupID, lobj.Name)
                Next

                DMSCLient.AddGroupMembers(strGroupID, "teamfolder")
                For Each dcObj As SorbaUsers In dCollection
                    DMSCLient.AddGroupMembers(strGroupID, dcObj.MYSUSER.Replace("@", "").Replace(".", ""))
                Next


            End If

            dCollection.Filter = Nothing

        End Sub

        Public Sub SetGroupMembershipProjekte(ByVal obj As Domain, ByVal DMSCLient As DMSConnector)
            Dim dCollection As XPCollection(Of SorbaUsers) = obj.SorbaUsersCollection

            'Alle Gruppen mit Eigenfilter löschen (EigenProjekte_usernam)
            Dim gList As List(Of Group) = DMSCLient.GetGroups()

            For Each gObj As Group In gList
                If gObj.Groupname.Contains("EigenProjekte_" & RemoveDiacritics(obj.Name)) Then
                    DMSCLient.DeleteGroup(gObj.Groupid)
                End If
            Next


            dCollection.Filter = Nothing
           ' Dim filter As CriteriaOperator = CriteriaOperator.Parse("MYSUSER <> '' AND M_PROJ == True ")
            Dim filter As CriteriaOperator = CriteriaOperator.Parse("MYSUSER <> '' AND M_PROJ == True AND  MY_PRJ == False AND PRJ_ACCESS == False")
            dCollection.Filter = filter

            Dim strGroupID As String

            strGroupID = DMSCLient.GetGroupId(RemoveDiacritics(obj.Name) & "_Projekte")

            If strGroupID.Length > 0 Then
                Dim listNames As List(Of GetMembersForGroupMember) = DMSCLient.GetGroupMembers(strGroupID)

                For Each lobj In listNames
                    DMSCLient.DeleteGroupMembers(strGroupID, lobj.Name)
                Next
                DMSCLient.AddGroupMembers(strGroupID, "teamfolder")
                For Each dcObj As SorbaUsers In dCollection

                    Dim strUser As String = dcObj.MYSUSER.Replace("@", "").Replace(".", "")

                    DMSCLient.AddGroupMembers(strGroupID, strUser)
                Next


            End If

            dCollection.Filter = Nothing


            'Alle Benutzer mit EigeneProjekte-Filter
              filter = CriteriaOperator.Parse(" MYSUSER <> '' AND M_PROJ == True AND ( MY_PRJ == True OR PRJ_ACCESS == True)")
              dCollection.Filter = filter

            Dim strGroupIDFilter As String = DMSCLient.GetGroupId(RemoveDiacritics(obj.Name) & "_Projekte_Filter")

            Dim listNames1 As List(Of GetMembersForGroupMember) = DMSCLient.GetGroupMembers(strGroupIDFilter)

            For Each lobj1 In listNames1
                 DMSCLient.DeleteGroupMembers(strGroupIDFilter, lobj1.Name)
            Next





            If dCollection.Count > 0 Then
             ' ImportData(obj, "DMS_PROJEKTE", "P", "")
              ImportData(obj, "DMS_PROJEKTE", "P", "&sOption=1")

                Dim projCollection As XPCollection(Of DatenObjekt) = obj.DatenObjekts
                  strGroupId = DMSClient.GetGroupId(RemoveDiacritics(obj.Name) & "_Projekte_Filter")
        
            'Für alle Projekte die Gruppe Projekte_Filter hinzufügen
           ' For Each oDO As DatenObjekt In projCollection
           '     If oDO.ID <> "" Then
           '     Dim strBaseFolderProj = "/sorbateamfolder/Sorba/" & obj.Name & "/Projekte/" & oDO.DValue
           '            SetEigenFilter(strGroupId, oDO.DValue, strBaseFolderProj, DMSClient)
           '      End If
           '    Next

                For Each pObj In dCollection
                    Dim strMySuser As String = pObj.MYSUSER.Replace("@", "").Replace(".", "")
                    'Benutzer der Gruppe Projekte_Filter hinzufügen
                    Dim strGroupIdS As String = DMSCLient.CreateGroup("EigenProjekte_" & RemoveDiacritics(obj.Name) & "_" & strMySuser)
                    If strGroupIdS.Length > 0 Then
                        DMSCLient.AddGroupMembers(strGroupIdS, strMySuser)
                        'Gruppe "Projekte_Filter"
                          DMSCLient.AddGroupMembers(strGroupId, strMySuser)
                       

                        If pObj.MY_PRJ = True Then
                            'Filter auf die Projektetabelle und Feld PROJ_FILTER
                            projCollection.Filter = Nothing
                            Dim filtereigen1 As CriteriaOperator = CriteriaOperator.Parse("Type = 'P' AND DFilter like '%" & strMySuser & "#%'")
                            projCollection.Filter = filtereigen1
                            For Each pdObj As DatenObjekt In projCollection

                                'EigeneGruppe auf das entsprechende Projekt hinzufügen
                                Dim strBaseFolder = "/sorbateamfolder/Sorba/" & obj.Name & "/Projekte/" & pdObj.DValue

                                DMSCLient.SetFolderGroup(strBaseFolder, strGroupIdS, "RWDS", "allow")

                            Next

                            projCollection.Filter = Nothing

                        End If

                        If pObj.PRJ_ACCESS = True then
                        'Filter auf die Projektetabelle und Feld PROJ_FILTER
                        projCollection.Filter = Nothing
                        Dim filtereigen2 As CriteriaOperator = CriteriaOperator.Parse("Type = 'P' AND DFilter like '%" & strMySuser & "@%'")
                        projCollection.Filter = filtereigen2
                        For Each pdObj As DatenObjekt In projCollection

                            'EigeneGruppe auf das entsprechende Projekt hinzufügen
                            Dim strBaseFolder = "/sorbateamfolder/Sorba/" & obj.Name & "/Projekte/" & pdObj.DValue

                            DMSCLient.SetFolderGroup(strBaseFolder, strGroupIdS, "RWDS", "allow")

                        Next

                        projCollection.Filter = Nothing

                      End If





                    End If

                Next

            End If



            dCollection.Filter = Nothing



        End Sub

        Public Sub SetGroupMembershipPersonal(ByVal obj As Domain, ByVal DMSCLient As DMSConnector)
            Dim dCollection As XPCollection(Of SorbaUsers) = obj.SorbaUsersCollection

            dCollection.Filter = Nothing
            Dim filter As CriteriaOperator = CriteriaOperator.Parse("MYSUSER <> '' AND PERSONAL = '1'")
            dCollection.Filter = filter

            Dim strGroupID As String

            strGroupID = DMSCLient.GetGroupId(RemoveDiacritics(obj.Name) & "_Personal_Read")

            If strGroupID.Length > 0 Then
                Dim listNames As List(Of GetMembersForGroupMember) = DMSCLient.GetGroupMembers(strGroupID)

                For Each lobj In listNames
                    DMSCLient.DeleteGroupMembers(strGroupID, lobj.Name)
                Next
                DMSCLient.AddGroupMembers(strGroupID, "teamfolder")
                For Each dcObj As SorbaUsers In dCollection
                    DMSCLient.AddGroupMembers(strGroupID, dcObj.MYSUSER.Replace("@", "").Replace(".", ""))
                Next


            End If


            dCollection.Filter = Nothing
            filter = CriteriaOperator.Parse("MYSUSER <> '' AND PERSONAL = '2'")
            dCollection.Filter = filter


            strGroupID = DMSCLient.GetGroupId(RemoveDiacritics(obj.Name) & "_Personal")

            If strGroupID.Length > 0 Then
                Dim listNames As List(Of GetMembersForGroupMember) = DMSCLient.GetGroupMembers(strGroupID)

                For Each lobj In listNames
                    DMSCLient.DeleteGroupMembers(strGroupID, lobj.Name)
                Next

                For Each dcObj As SorbaUsers In dCollection
                    DMSCLient.AddGroupMembers(strGroupID, dcObj.MYSUSER.Replace("@", "").Replace(".", ""))
                Next


            End If

            dCollection.Filter = Nothing

        End Sub

        Public Function TransferFiles(ByVal obj As Domain) As Boolean

            Dim strDMS As String = obj.FK_Kunde.DMSAdresse
            Dim strDMSUser As String = obj.FK_Kunde.DMSUserTeamfolder
            Dim strDMSPasswort As String = obj.FK_Kunde.DMSPasswortTeamfolder
            Dim strDMSAdminUser As String = obj.FK_Kunde.DMSUser
            Dim strDMSAdminPasswort As String = obj.FK_Kunde.DMSPassword

            Dim DMSClient As New DMSConnector(strDMSUser, strDMSPasswort, strDMS, strDMSAdminUser, strDMSAdminPasswort)
            Dim strBase As String = "Sorba"

            TransferFilesSend(obj, "A", "Adressen")
            TransferFilesSend(obj, "P", "Projekte")
            TransferFilesSend(obj, "I", "Inventar")
            TransferFilesSend(obj, "M", "Material")
            TransferFilesSend(obj, "PERS", "Personal")
            TransferFilesSend(obj, "F", "Fibu")

             ArchiveProjects(obj,DMSClient)


        End Function

        Public Function TransferFilesSend(ByVal obj As Domain, ByVal strType As String, ByVal strBaseFolderName As String) As Boolean
            Dim strSorbaWebservice As String = obj.FK_Kunde.SorbaWebservice
            Dim strDomain As String = obj.Name
            Dim strDMS As String = obj.FK_Kunde.DMSAdresse
            Dim strDMSUser As String = obj.FK_Kunde.DMSUserTeamfolder
            Dim strDMSPasswort As String = obj.FK_Kunde.DMSPasswortTeamfolder
            Dim strDMSAdminUser As String = obj.FK_Kunde.DMSUser
            Dim strDMSAdminPasswort As String = obj.FK_Kunde.DMSPassword
            Dim DMSClient As New DMSConnector(strDMSUser, strDMSPasswort, strDMS, strDMSAdminUser, strDMSAdminPasswort)
            Dim filter As CriteriaOperator = CriteriaOperator.Parse("[Type] = '" & strType & "'")
            Dim strGroupId As String = ""
            obj.DatenObjekts.Filter = filter

            Dim dCollection As XPCollection(Of DatenObjekt) = obj.DatenObjekts

            ' Dim strBaseFolder = "\" & strBaseFolderName
            Dim strBaseFolder = "/sorbateamfolder/Sorba/" & obj.Name & "/" & strBaseFolderName

            If strType = "P" Then
                strGroupId = DMSClient.GetGroupId(RemoveDiacritics(obj.Name) & "_Projekte_Filter")
            End If

            For Each oDO As DatenObjekt In dCollection
                If oDO.ID <> "" Then

                    DMSClient.RenameFolder(oDO.ID, oDO.DValue, strBaseFolder)
                      If strType = "P" Then
                          SetEigenFilter(strGroupId, oDO.DValue, strBaseFolder, DMSClient)
                     End If

                End If

            Next




        End Function

        Public Sub SetEigenFilter(ByVal strGroupIDFilter As String, ByVal strFolder As String, ByVal strBaseFolder As String, ByVal DMSClient As DMSConnector)
            DMSClient.SetFolderGroup(strBaseFolder & "/" & strFolder, strGroupIDFilter, "RWDSM", "deny")
        End Sub


        Public Sub CreateFolders(ByRef xpc As XPCollection(Of FilesnFolders), ByVal strFilter As String, ByRef dmsc As DMSConnector, ByVal strPath As String, ByVal strParent As String)
            xpc.Filter = CriteriaOperator.Parse("IsFolder == True AND [Path] like '%" & LCase(strFilter.Replace("'", "''")) & "%' AND [Parent] == '" & UCase(strParent) & "'")
            For Each oFF As FilesnFolders In xpc

                Try


                    dmsc.CreateFolder(oFF.Name, strPath)
                    CreateFolders(xpc, strFilter & "\" & oFF.Name, dmsc, strPath & "/" & oFF.Name, oFF.Name)

                Catch ex As Exception

                End Try
            Next

            xpc.Filter = Nothing

        End Sub

        Public Sub RenameFolders(ByRef xpc As XPCollection(Of FilesnFolders), ByVal strFilter As String, ByRef dmsc As DMSConnector, ByVal strPath As String, ByVal strParent As String)
            xpc.Filter = CriteriaOperator.Parse("IsFolder == True AND [Path] like '%" & LCase(strFilter.Replace("'", "''")) & "%' AND [Parent] == '" & UCase(strParent) & "'")
            For Each oFF As FilesnFolders In xpc

                Try


                    dmsc.CreateFolder(oFF.Name, strPath)
                    CreateFolders(xpc, strFilter & "\" & oFF.Name, dmsc, strPath & "/" & oFF.Name, oFF.Name)

                Catch ex As Exception

                End Try
            Next

            xpc.Filter = Nothing

        End Sub


        Public Function ImportProjects(ByVal obj As Domain) As Boolean
            BaseConfiguration(obj)
        End Function

        Public Shared Function RemoveDiacritics(ByVal s As String) As String
            Dim normalizedString As String = s.Normalize(NormalizationForm.FormD)
            Dim stringBuilder As StringBuilder = New StringBuilder()

            For i As Integer = 0 To normalizedString.Length - 1
                Dim c As Char = normalizedString(i)
                If CharUnicodeInfo.GetUnicodeCategory(c) <> UnicodeCategory.NonSpacingMark Then stringBuilder.Append(c)
            Next

            Return RemoveSpecialCharacters(stringBuilder.ToString()).ToString
        End Function

        Public Shared Function RemoveSpecialCharacters(ByVal str As String) As String
            Dim sb As StringBuilder = New StringBuilder()

            For Each c As Char In str

                If (c >= "0"c AndAlso c <= "9"c) OrElse (c >= "A"c AndAlso c <= "Z"c) OrElse (c >= "a"c AndAlso c <= "z"c) OrElse c = "."c OrElse c = "_"c OrElse c = " "c Then
                    sb.Append(c)
                End If
            Next

            Return sb.ToString()
        End Function

        Public Function BaseConfiguration(ByVal obj As Domain)

            'Benötige Gruppen erstellen
            Dim strDMS As String = obj.FK_Kunde.DMSAdresse
            Dim strDMSUser As String = obj.FK_Kunde.DMSUserTeamfolder
            Dim strDMSPasswort As String = obj.FK_Kunde.DMSPasswortTeamfolder
            Dim strDMSAdminUser As String = obj.FK_Kunde.DMSUser
            Dim strDMSAdminPasswort As String = obj.FK_Kunde.DMSPassword

            Dim DMSClient As New DMSConnector(strDMSUser, strDMSPasswort, strDMS, strDMSAdminUser, strDMSAdminPasswort)
            Dim strDomainS As String = RemoveDiacritics(obj.Name)
            Dim strBase As String = "Sorba"
            DMSClient.CreateFolder(strBase, "/sorbateamfolder")
            DMSClient.CreateFolder(obj.Name, "/sorbateamfolder/" & strBase)
            DMSClient.CreateFolder("Adressen", "/sorbateamfolder/" & strBase & "/" & obj.Name)
            DMSClient.CreateFolder("Projekte", "/sorbateamfolder/" & strBase & "/" & obj.Name)
            DMSClient.CreateFolder("Material", "/sorbateamfolder/" & strBase & "/" & obj.Name)
            DMSClient.CreateFolder("Inventar", "/sorbateamfolder/" & strBase & "/" & obj.Name)
            DMSClient.CreateFolder("Personal", "/sorbateamfolder/" & strBase & "/" & obj.Name)
            DMSClient.CreateFolder("Fibu", "/sorbateamfolder/" & strBase & "/" & obj.Name)

            Dim strBase1 As String = "SorbaArchiv"
            DMSClient.CreateFolder(strBase1, "/sorbateamfolder")
            DMSClient.CreateFolder(obj.Name, "/sorbateamfolder/" & strBase1)
            DMSClient.CreateFolder("Projekte", "/sorbateamfolder/" & strBase1 & "/" & obj.Name)



            Dim strGroupID As String

            strGroupID = DMSClient.GetGroupId(strDomainS & "_Adressen")
            If strGroupID.Length < 1 Then
                strGroupID = DMSClient.CreateGroup(strDomainS & "_Adressen")
            End If

            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Adressen", strGroupID, "RWDSM", "allow")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Projekte", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Inventar", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Material", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Personal", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Fibu", strGroupID, "RWDSM", "deny")


            strGroupID = DMSClient.GetGroupId(strDomainS & "_Projekte")
            If strGroupID.Length < 1 Then
                strGroupID = DMSClient.CreateGroup(strDomainS & "_Projekte")
            End If

            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Projekte", strGroupID, "RWDSM", "allow")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase1 & "/" & obj.Name & "/Projekte", strGroupID, "RWDSM", "allow")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Adressen", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Inventar", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Material", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Personal", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Fibu", strGroupID, "RWDSM", "deny")


            strGroupID = DMSClient.GetGroupId(strDomainS & "_Projekte_Filter")
            If strGroupID.Length < 1 Then
                strGroupID = DMSClient.CreateGroup(strDomainS & "_Projekte_Filter")
            End If

            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Projekte", strGroupID, "R", "allow")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase1 & "/" & obj.Name & "/Projekte", strGroupID, "R", "allow")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Adressen", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Inventar", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Material", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Personal", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Fibu", strGroupID, "RWDSM", "deny")


            strGroupID = DMSClient.GetGroupId(strDomainS & "_Inventar")
            If strGroupID.Length < 1 Then
                strGroupID = DMSClient.CreateGroup(strDomainS & "_Inventar")
            End If

            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Inventar", strGroupID, "RWDSM", "allow")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Material", strGroupID, "RWDSM", "allow")

            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Adressen", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Projekte", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Personal", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Fibu", strGroupID, "RWDSM", "deny")

            strGroupID = DMSClient.GetGroupId(strDomainS & "_Personal")
            If strGroupID.Length < 1 Then
                strGroupID = DMSClient.CreateGroup(strDomainS & "_Personal")
            End If

            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Personal", strGroupID, "RWDSM", "allow")

            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Adressen", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Projekte", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Inventar", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Fibu", strGroupID, "RWDSM", "deny")


            strGroupID = DMSClient.GetGroupId(strDomainS & "_Personal_Read")
            If strGroupID.Length < 1 Then
                strGroupID = DMSClient.CreateGroup(strDomainS & "_Personal_Read")
            End If


            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Personal", strGroupID, "R", "allow", True)
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Adressen", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Projekte", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Inventar", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Material", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Fibu", strGroupID, "RWDSM", "deny")


            strGroupID = DMSClient.GetGroupId(strDomainS & "_Fibu")
            If strGroupID.Length < 1 Then
                strGroupID = DMSClient.CreateGroup(strDomainS & "_Fibu")
            End If

            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Fibu", strGroupID, "RWDSM", "allow")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Adressen", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Projekte", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Inventar", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Material", strGroupID, "RWDSM", "deny")
            DMSClient.SetFolderGroup("/sorbateamfolder/" & strBase & "/" & obj.Name & "/Personal", strGroupID, "RWDSM", "deny")



        End Function




        Public Function ImportSorbaUser(ByRef obj As Domain) As Boolean
            Dim strSorbaWebservice As String = obj.FK_Kunde.SorbaWebservice
            Dim strDomain As String = obj.Name
            Dim strKdNr As String = obj.FK_Kunde.AdrNr

            Dim strConString As String = strSorbaWebservice & "/webapi/" & strDomain & "/?sFunction=DMS_USERS"
            Dim strOid As String = obj.Oid


            If strSorbaWebservice.Length > 0 And strDomain.Length > 0 Then

                Dim client As RestClient = New RestClient(strConString)
                Dim request = New RestRequest(Method.[GET])
                request.AddHeader("cache-control", "no-cache")
                request.AddHeader("Authorization", "Basic " + GetCredentials(strKdNr, strDomain))
                Dim response As IRestResponse = client.Execute(request)

                If response.IsSuccessful = True Then

                    Dim suColl As XPCollection(Of SorbaUsers) = obj.SorbaUsersCollection

                    'obj.Session.BeginTransaction

                    'obj.Session.Delete(suColl)
                    obj.Session.Delete(suColl)

                    'obj.Session.CommitTransaction


                    Dim objDevice As List(Of ImportSorbaUsers) = JsonConvert.DeserializeObject(Of List(Of ImportSorbaUsers))(response.Content)
                    If IsNothing(objDevice) = False Then


                        '    obj.Session.BeginTransaction
                        For Each objp As ImportSorbaUsers In objDevice
                            Dim suObj As New SorbaUsers(obj.Session)
                            suObj.FK_Domain = obj
                            suObj.GROUP = objp.GROUP
                            suObj.GR_ROLE = objp.GR_ROLE
                            suObj.MYSUSER = objp.MYSUSER
                            suObj.MY_PRJ = objp.MY_PRJ
                            suObj.M_AQ = objp.M_AQ
                            suObj.M_MQ = objp.M_MQ
                            suObj.M_PROJ = objp.M_PROJ
                            suObj.PERSONAL = objp.PERSONAL
                            suObj.PRJOWNER = objp.PRJOWNER
                            suObj.SUCH_NAME = objp.SUCH_NAME
                            suObj.USERGROUP = objp.USERGROUP
                            suObj.USERNAME = objp.USERNAME
                            suObj.M_FIBU = objp.M_FIBU

                            If IsNothing(objp.PRJ_ACCESS) = False Then
                                suObj.PRJ_ACCESS = objp.PRJ_ACCESS
                            End If
                            suColl.Add(suObj)





                        Next

                        obj.Session.Save(suColl)

                    End If


                    Return True

                Else

                    Return False

                End If

            End If

        End Function

        Public Function ImportData(ByRef obj As Domain, ByVal strFunction As String, ByVal strFilter As String, Optional ByVal strZusatzquery As String = "") As Boolean




            Dim strSorbaWebservice As String = obj.FK_Kunde.SorbaWebservice
            Dim strDomain As String = obj.Name
            Dim strKDNr As String = obj.FK_Kunde.AdrNr


            Dim strConString As String = strSorbaWebservice & "/webapi/" & strDomain & "/?sFunction=" & strFunction & strZusatzquery
            Dim strOid As String = obj.Oid


            If strSorbaWebservice.Length > 0 And strDomain.Length > 0 Then

                If strFilter.Length > 0 Then
                    '    obj.Session.BeginTransaction
                    Dim suColl As XPCollection(Of DatenObjekt) = obj.DatenObjekts
                    Dim filter As CriteriaOperator = CriteriaOperator.Parse("[Type]='" & strFilter & "'")
                    suColl.Filter = filter
                    obj.Session.Delete(suColl)
                    '   obj.Session.CommitTransaction
                End If

                Dim strCredentials As String = GetCredentials(strKDNr, strDomain)

                Dim client As RestClient = New RestClient(strConString)
                client.Timeout = -1
                Dim request = New RestRequest(Method.GET)

                request.AddHeader("Authorization", "Basic " + strCredentials)

                Dim response As IRestResponse = client.Execute(request)

                If response.IsSuccessful = True Then

                    Dim objDevice As List(Of Project) = JsonConvert.DeserializeObject(Of List(Of Project))(response.Content)
                    If IsNothing(objDevice) = False Then
                        '  obj.Session.BeginTransaction
                        For Each objp As Project In objDevice
                            Dim dpobjas As New DatenObjekt(obj.Session)

                            dpobjas.DValue = objp.DValue
                            dpobjas.ID = objp.ID
                            dpobjas.Path = objp.DPfad
                            dpobjas.ActualDate = DateTime.Now
                            dpobjas.FK_Domain = obj
                            dpobjas.Type = objp.Art
                            dpobjas.DFilter = objp.DFilter

                            dpobjas.Save()




                        Next
                        '   obj.Session.CommitTransaction



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
        Public Property DFilter As String

    End Class

    Public Class SorbaFile
        Public Property Name As String
        Public Property Path As String

        Public Property Parent As String

        Public Property IsDirectory As Boolean

    End Class

    Public Class ImportSorbaUsers
        Public Property USERNAME As String
        Public Property SUCH_NAME As String
        Public Property MYSUSER As String
        Public Property USERGROUP As String
        Public Property GROUP As Boolean
        Public Property GR_ROLE As Boolean
        Public Property M_MQ As Boolean
        Public Property PERSONAL As String
        Public Property MY_PRJ As Boolean
        Public Property PRJOWNER As Boolean
        Public Property M_PROJ As Boolean
        Public Property M_AQ As Boolean
        Public Property M_FIBU As Boolean
        Public Property PRJ_ACCESS As Boolean
    End Class

End Namespace
