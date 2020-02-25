Imports System.IO
Imports System.Linq
Imports System.Net
Imports ApiHelper
Imports ApiHelper.Client
Imports ApiHelper.IWrapper
Imports ApiHelper.Model
Imports ApiHelper.Wrapper
Imports System.Threading.Tasks


Namespace DMSCenter

    Public Class DMSConnector

        Private Shared Property _cookiesContainer As List(Of Cookie)
        Private Shared Property _cookiesContainerAdmin As List(Of Cookie)
        Protected Shared _apiClient As IApiClient
        Protected Shared _apiClientAdmin As IApiClient
        Protected Shared _userApi As IUserApi
        Protected Shared _adminApi As IAdminApi

        Private Shared Property DMSAdress As String

        Public Sub New(ByVal strUser As String, ByVal strPasswort As String, ByVal strDMS As String, ByVal strAdminUser As String, ByVal strAdminPasswort As String)
            DMSAdress = strDMS
            LoginCheck(strUser, strPasswort, strDMS, strAdminUser, strAdminPasswort)

            _apiClient = New ApiClient(strDMS, _cookiesContainer)
            _apiClientAdmin = New ApiClient(strDMS, _cookiesContainerAdmin)
            _userApi = New UserApi(_apiClient)
            _adminApi = New AdminApi(_apiClientAdmin)


        End Sub

        Private Sub LoginCheck(ByVal strUser As String, ByVal strPasswort As String, ByVal struserUrl As String, ByVal strAdminUser As String, ByVal strAdminPasswort As String)

            Dim subapiClient = New ApiClient(struserUrl, _cookiesContainer)
            Dim user = New User() With {
               .isAdmin = False,
               .userid = strUser,
               .password = strPasswort}

            Dim response = New LoginClient(subapiClient).Login(user, Function(respCookie)

                                                                         If respCookie IsNot Nothing AndAlso respCookie.ToList().Count > 0 Then
                                                                             _cookiesContainer = respCookie.ToList()

                                                                             ' Dim cookieContainer = TryCast(_cookiesContainer, List(Of Cookie))
                                                                             _cookiesContainer = TryCast(_cookiesContainer, List(Of Cookie))
                                                                             '   _apiClient.cookieContainer = _cookiesContainer


                                                                         End If
                                                                     End Function).Result



            Dim subapiClient1 = New ApiClient(struserUrl, _cookiesContainerAdmin)
            Dim user1 = New User() With {
               .isAdmin = True,
               .userid = strAdminUser,
               .password = strAdminPasswort}

            Dim response1 = New LoginClient(subapiClient1).Login(user1, Function(respCookie)

                                                                            If respCookie IsNot Nothing AndAlso respCookie.ToList().Count > 0 Then
                                                                                _cookiesContainerAdmin = respCookie.ToList()

                                                                                ' Dim cookieContainer = TryCast(_cookiesContainer, List(Of Cookie))
                                                                                _cookiesContainerAdmin = TryCast(_cookiesContainerAdmin, List(Of Cookie))
                                                                                '   _apiClient.cookieContainer = _cookiesContainer


                                                                            End If
                                                                        End Function).Result


        End Sub




        Public Function CreateFolder(ByVal strFolder As String, ByVal strFolderPath As String) As Boolean

            If strFolder <> "DQ" And strFolder <> "RQ" And strFolder <> "PQ" Then
                'apiClient = New ApiClient(DMSAdress, cookiesContainer)
                'userApi = New UserApi(apiClient)
                Dim folder As New CreateFolderPostModel
                folder.name = strFolder
                folder.path = strFolderPath
                Dim folderResponse = _userApi.CreateFolder(folder).Result
            End If

            Return True
        End Function


        Public Function RenameFolderArchiv(ByVal strFolder As String, ByVal strFolderNew As String) As Boolean
            Try

                Dim folder As New RenameOrMoveModel
                folder.fromname = strFolder

                folder.toname = strFolderNew


                Dim aramfolderResponse = _userApi.RenameOrMove(folder).Result


            Catch ex As Exception

            End Try

            Return True
        End Function
        Public Function RenameFolder(ByVal strFolder As String, ByVal strFolderNew As String, ByVal strFolderPath As String) As Boolean
            Try
                '   Dim strFolder1 As String = "\\DMSFiler\DMS$\Transfer\368\Sorba\368" & strFolderPath & "\" & strFolder

                Dim folder As New RenameFileModel
                folder.name = strFolder
                folder.path = strFolderPath
                folder.newname = strFolderNew.TrimEnd(".")
                Dim aramfolderResponse = _userApi.RenameFile(folder).Result



                '  FileIO.FileSystem.RenameDirectory(strFolder1, strFolderNew)


            Catch ex As Exception

            End Try

            Return True
        End Function

        Public Function CreateFolder(ByVal strFolder As String, ByVal strFolderNew As String, ByVal strFolderPath As String) As Boolean
            Try
                '   Dim strFolder1 As String = "\\DMSFiler\DMS$\Transfer\368\Sorba\368" & strFolderPath & "\" & strFolder

                Dim folder As New RenameFileModel
                folder.name = strFolder
                folder.path = strFolderPath
                folder.newname = strFolderNew.TrimEnd(".")
                '  Dim aramfolderResponse = _userApi.RenameFile(folder).Result

                Dim folder1 As New CreateFolderPostModel
                folder1.path = strFolderPath
                folder1.name = strFolderNew.TrimEnd(".")

                Dim aramfolderResponse = _userApi.CreateFolder(folder1).Result

                '  FileIO.FileSystem.RenameDirectory(strFolder1, strFolderNew)


            Catch ex As Exception

            End Try

            Return True
        End Function

        Public Function GetGroupId(ByVal strGroup As String) As String

            Dim group As New GetGroupByNamePostModel
            group.groupname = strGroup

            Dim groupResponse As GetGroupByNameResponseModel = _adminApi.GetGroupByByName(group).Result


            If groupResponse.Data.UserGroups.Count > 0 Then

                Return groupResponse.Data.UserGroups(0).Groupid
            Else
                Return ""
            End If
        End Function

        Public Function GetGroupMembers(ByVal strGroup As String) As List(Of GetMembersForGroupMember)

            Dim group As New GetMembersForGroupPostModel
            group.groupid = strGroup

            Dim groupResponse As GetMembersForGroupResponseModel = _adminApi.GetMembersForGroup(group).Result
            Return groupResponse.Data.GetMembersForGroupMember

        End Function

        Public Function GetFolders(ByVal strPath As String) As List(Of Entry)


            Dim model As New FileListModel
            model.limit = 1000000
            model.path = strPath
            model.start = 0


            Dim ListResponse As FileListResponseModel = _userApi.GetFileList(model).Result

            Return ListResponse.Data.Entries

        End Function

        Public Function AddGroupMembers(ByVal strGroup As String, ByVal strUserId As String) As Boolean

            Dim group As New AddMemberToGroupPostModel
            group.groupid = strGroup
            group.userid = strUserId

            Dim groupResponse = _adminApi.AddMemberToGroup(group).Result
            Return True

        End Function

        Public Function DeleteGroupMembers(ByVal strGroup As String, ByVal strUserId As String) As Boolean

            Dim group As New AddMemberToGroupPostModel
            group.groupid = strGroup
            group.userid = strUserId

            Dim groupResponse = _adminApi.DeleteMemberFromGroup(group).Result
            Return True

        End Function

        Public Function DeleteGroup(ByVal strGroup As String) As Boolean

            Dim group As New DeleteGroupPostModel
            group.groupid = strGroup


            Dim groupResponse = _adminApi.DeleteGroup(group).Result
            Return True

        End Function

        Public Function CreateGroup(ByVal strGroup As String) As String

            Dim group As New AddGroupPostModel
            group.groupname = strGroup
            Dim groupResponse As AddGroupPostResponseModel = _adminApi.AddGroup(group).Result

            If groupResponse.Data.UserGroups.Count > 0 Then

                Return groupResponse.Data.UserGroups(0).Groupid
            Else
                Return ""
            End If
        End Function



        Public Function CreateUser(ByVal strUserName As String, ByVal strUserPassword As String, ByVal strEmail As String) As AddUserPostResponseModel

            Dim usr As New AddUserPostModel
            usr.email = strEmail
            usr.password = strUserPassword
            usr.username = strUserName
            usr.authType = 0

            Dim usrResponse As AddUserPostResponseModel = _adminApi.AddUser(usr).Result

            Return usrResponse
        End Function

        Public Function UpdateUser(ByVal strUserName As String, ByVal strUEmail As String, ByVal bAktivieren As Boolean) As UpdateUserResponseModel

            Dim usr As New UpdateUserPostModel
            usr.profile = strUserName
            usr.email = strUEmail
            usr.adminstatus = 0
            usr.disablemyfilessync = 0
            usr.disablenetworksync = 0
            usr.requirepasswordchange = 0



            If bAktivieren = True Then
                usr.status = "1"
            Else
                usr.status = "2"
            End If

            Dim usrResponse As UpdateUserResponseModel = _adminApi.UpdateUser(usr).Result

            Return usrResponse
        End Function


        Public Function ChangeUserPW(ByVal strUserName As String, ByVal strUserPassword As String) As SetUserPasswordResponseModel

            Dim usr As New SetUserPasswordPostModel
            usr.password = strUserPassword
            usr.profile = strUserName

            Dim usrResponse As SetUserPasswordResponseModel = _adminApi.SetUserPassword(usr).Result

            Return usrResponse
        End Function


        Public Function GetGroups() As List(Of Group)
            Dim group As New AddGroupPostModel
            Dim groupResponse As GetGroupsResponseModel = _adminApi.GetGroups().Result

            Return groupResponse.Data.UserGroups
        End Function

        Public Function SetFolderGroup(ByVal strFolderPath As String, ByVal strGroup As String, ByVal strPerm As String, ByVal strFlag As String, Optional ByVal bdontInherit As Boolean = False) As Boolean
            Dim group As New AclEntryModel
            group.path = strFolderPath
            group.perm = strPerm '"RWDS"
            group.flag = strFlag '"allow"
            group.type = "group"
            group.value = strGroup

            Dim groupResponse = _userApi.AddAclEntry(group).Result
            If bdontInherit = True Then
                Dim BaseResponse = _userApi.SetAclInheritance(strFolderPath, "0")
            End If
            Return True
        End Function

        Public Async Function UploadFileAsync(ByVal strPath As String, ByVal strName As String, ByVal strFile As String) As Task(Of Boolean)
            Dim bytes = Convert.FromBase64String(strFile)
            Dim contents = New MemoryStream(bytes)

            Dim upl As New UploadFileModel
            upl.appname = "explorer"
            upl.path = strPath
            upl.complete = 1
            upl.offset = 0
            upl.filename = strName
            Dim response = Await _userApi.UploadFileStreamAsync(upl, contents)

            Return True
        End Function

    End Class



End Namespace