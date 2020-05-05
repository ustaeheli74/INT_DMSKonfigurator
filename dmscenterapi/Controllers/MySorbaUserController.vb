Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Security.Cryptography
Imports System.Web.Http
Imports System.Web.Script.Serialization
Imports DevExpress.Data.Filtering
Imports DevExpress.Xpo
Imports DMSCenter.Module.DMSCenter


Imports Mindscape.Raygun4Net.Messages
Imports Mindscape.Raygun4Net.WebApi

Namespace Controllers
    Public Class MySorbaUserController
        Inherits ApiController

     Dim Public Shared statusdict As New Dictionary(Of String, Integer)

        <HttpGet, Route("status")>
        Public Function Status() As String
            Return "DMSService is running..."
        End Function

       



        <HttpPost, Route("deactivatedmsuser")>
        Public Function UserDelete(ByVal req As MySorbaUser) As standardResponse
            Try

              
                If req IsNot Nothing Then
                    If req.Name.Length > 0 And req.KdNr.Length > 0 And req.Password.Length > 0 Then

                        If IsValidEmail(req.Name) Then



              
                            If req.Password.Length > 7 Then



                                Dim session As Session = XpoHelper.GetNewSession()

                                Dim objKunde As Kunde = session.FindObject(Of Kunde)(New BinaryOperator("AdrNr", req.KdNr))
                                If IsNothing(objKunde) = False Then

                                    Dim sConn As New SorbaConnector
                                    Dim strReturn As String = sConn.ActDeactUser(objKunde, req.Name, "", False)

                                    Dim strStatusCode As String = Between(strReturn, "<result>", "</result>")
                                    Dim strMessage As String = Between(strReturn, "<message>", "</message>")


                            


                                    If strStatusCode = "1" Then
                                        Return New standardResponse() With {.IsSuccess = True, .Result = strMessage}
                                    Else
                                        Return New standardResponse() With {.IsSuccess = False, .Result = strMessage}
                                    End If

                                Else
                                    Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Kunde im DMS nicht vorhanden")}
                                End If

                            Else
                                Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Passwort ist zu kurz")}
                            End If

                        Else
                            Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Falsches Emailformat")}
                        End If

                    Else
                        Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Fehlende Daten")}
                    End If
                Else
                    Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Fehlende Daten / Model")}
                End If

            Catch ex As Exception
                Dim rg As New RaygunWebApiClient()
                rg.SendInBackground(ex)
                Throw New HttpResponseException(HttpStatusCode.InternalServerError)
            End Try
        End Function



        <HttpPost, Route("dmsuser")>
        Public Function UserCreate(ByVal req As MySorbaUser) As standardResponse
            Try
               


                If req IsNot Nothing Then
                    If req.Name.Length > 0 And req.KdNr.Length > 0 And req.Password.Length > 0 Then

                        If IsValidEmail(req.Name) Then

                                              If req.Password.Length > 7 Then

                                Dim session As Session = XpoHelper.GetNewSession()

                                Dim objKunde As Kunde = session.FindObject(Of Kunde)(New BinaryOperator("AdrNr", req.KdNr))
                                If IsNothing(objKunde) = False Then

                                    Dim sConn As New SorbaConnector
                                    Dim strReturn As String = sConn.CreateDMSUser(objKunde, req.Name, req.Password)

                                    Dim strStatusCode As String = Between(strReturn, "<result>", "</result>")
                                    Dim strMessage As String = Between(strReturn, "<message>", "</message>")

                                    If strStatusCode = "1" Then
                                        Return New standardResponse() With {.IsSuccess = True, .Result = strMessage}
                                    Else

                                        If strMessage = "Username already exists and is not available" Then
                                            Dim strReturn1 As String = sConn.ActDeactUser(objKunde, req.Name,req.Password, True)
                                            Dim strStatusCode1 As String = Between(strReturn1, "<result>", "</result>")
                                            Dim strMessage1 As String = Between(strReturn1, "<message>", "</message>")
                                            If strStatusCode1 = "1" Then
                                                Return New standardResponse() With {.IsSuccess = True, .Result = "Benutzer wurde wieder aktiviert"}
                                            Else
                                                Return New standardResponse() With {.IsSuccess = False, .Result = strMessage1}
                                            End If
                                        Else
                                            Return New standardResponse() With {.IsSuccess = False, .Result = strMessage}
                                        End If


                                    End If

                                Else
                                    Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Kunde im DMS nicht vorhanden")}
                                End If

                            Else
                                Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Passwort ist zu kurz")}
                            End If

                        Else
                            Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Falsches Emailformat")}
                        End If

                    Else
                        Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Fehlende Daten")}
                    End If
                Else
                    Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Fehlende Daten / Model")}
                End If

            Catch ex As Exception
                Dim rg As New RaygunWebApiClient()
                rg.SendInBackground(ex)
                Throw New HttpResponseException(HttpStatusCode.InternalServerError)
            End Try
        End Function



        <Route("autosyncdomain/{kdnr}/{domain}"), HttpGet>
        Public Function AutoSyncDomain(ByVal kdnr As String, ByVal domain As String) As standardResponse
            Try
                If kdnr.Length > 0 Then


                    Dim session As Session = XpoHelper.GetNewSession()

                    Dim objKunde As Kunde = session.FindObject(Of Kunde)(New BinaryOperator("AdrNr", kdnr))

                    If IsNothing(objKunde) = False Then

                        Dim bFound As Boolean = False
                        Dim sConn As New SorbaConnector
                        For Each dobj As Domain In objKunde.Domains
                            If LCase(dobj.Name) = LCase(domain) Then
                                sConn.SorbaAutorisierung(dobj)
                                bFound = True
                            End If

                        Next
                        If bFound = True Then
                            Return New standardResponse() With {.IsSuccess = True, .Result = "Sync erfolgt"}
                        Else
                            Return New standardResponse() With {.IsSuccess = False, .Result = "Domäne nicht gefunden"}
                        End If

                    Else
                        Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Kunde im DMS nicht vorhanden")}

                    End If

                Else
                    Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Kunde im DMS nicht vorhanden")}
                End If


            Catch ex As Exception
                Dim rg As New RaygunWebApiClient()
                rg.SendInBackground(ex)
                Throw New HttpResponseException(HttpStatusCode.InternalServerError)
            End Try
        End Function


        <Route("transferdomain/{kdnr}/{domain}"), HttpGet>
        Public Function TransferDomain(ByVal kdnr As String, ByVal domain As String) As standardResponse
            '   Try
            If kdnr.Length > 0 Then
                Dim session As Session = XpoHelper.GetNewSession()
                Dim objKunde As Kunde = session.FindObject(Of Kunde)(New BinaryOperator("AdrNr", kdnr))
                If IsNothing(objKunde) = False Then
                    Dim bFound As Boolean = False
                    Dim sConn As New SorbaConnector
                    For Each dobj As Domain In objKunde.Domains
                        If LCase(dobj.Name) = LCase(domain) Then
                            sConn.TransferFiles(dobj)
                            bFound = True
                        End If
                    Next
                    If bFound = True Then
                        Return New standardResponse() With {.IsSuccess = True, .Result = "DatenTransfer erfolgt"}
                    Else
                        Return New standardResponse() With {.IsSuccess = False, .Result = "Domäne nicht gefunden"}
                    End If

                Else
                    Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Kunde im DMS nicht vorhanden")}

                End If

            Else
                Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Kunde im DMS nicht vorhanden")}
            End If


            '    Catch ex As Exception
            '       Dim rg As New RaygunWebApiClient()
            '       rg.SendInBackground(ex)
            '       Throw New HttpResponseException(HttpStatusCode.InternalServerError)
            '  End Try
        End Function

        <Route("importdomain/{kdnr}/{domain}"), HttpGet>
        Public Function ImportDomain(ByVal kdnr As String, ByVal domain As String) As standardResponse
            Try
                If kdnr.Length > 0 Then


                    Dim session As Session = XpoHelper.GetNewSession()

                    Dim objKunde As Kunde = session.FindObject(Of Kunde)(New BinaryOperator("AdrNr", kdnr))

                    If IsNothing(objKunde) = False Then

                        Dim bFound As Boolean = False
                        Dim sConn As New SorbaConnector
                        For Each dobj As Domain In objKunde.Domains
                            If LCase(dobj.Name) = LCase(domain) Then
                                sConn.ImportProjects(dobj)
                                bFound = True
                            End If

                        Next
                        If bFound = True Then
                            Return New standardResponse() With {.IsSuccess = True, .Result = "StrukturImport erfolgt"}
                        Else
                            Return New standardResponse() With {.IsSuccess = False, .Result = "Domäne nicht gefunden"}
                        End If

                    Else
                        Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Kunde im DMS nicht vorhanden")}

                    End If

                Else
                    Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Kunde im DMS nicht vorhanden")}
                End If


            Catch ex As Exception
                Dim rg As New RaygunWebApiClient()
                rg.SendInBackground(ex)
                Throw New HttpResponseException(HttpStatusCode.InternalServerError)
            End Try
        End Function

        <Route("autosync/{kdnr}"), HttpGet>
        Public Function AutoSync(ByVal kdnr As String) As standardResponse
            Try

                If kdnr.Length > 0 Then
                    Dim session As Session = XpoHelper.GetNewSession()
                    Dim objKunde As Kunde = session.FindObject(Of Kunde)(New BinaryOperator("AdrNr", kdnr))
                    If IsNothing(objKunde) = False Then
                        Dim sConn As New SorbaConnector
                        For Each dobj As Domain In objKunde.Domains
                  

                       sconn.SorbaAutorisierung(dobj)

                        Next
                       
                        Return New standardResponse() With {.IsSuccess = True, .Result = "Sync ist erfolgt"}
                    End If
                Else
                    Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Kunde im DMS nicht vorhanden")}
                End If
            Catch ex As Exception
                  Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Es ist ein Fehler aufgetreten")}
                Dim rg As New RaygunWebApiClient()
                rg.SendInBackground(ex)

                Throw New HttpResponseException(HttpStatusCode.InternalServerError)

            Finally
                
               
            End Try
        End Function


         <Route("addstatus/{gd}"), HttpGet>
        Public Function AddStatus(byval gd As String) As standardResponse
            Try

                If gd.Length > 0 Then

                statusdict.Add(gd,1)
                    End If
                 Catch ex As Exception
                 Return New standardResponse() With {.IsSuccess = True, .Result = String.Format("alles io")}
                Dim rg As New RaygunWebApiClient()
                rg.SendInBackground(ex)

                Throw New HttpResponseException(HttpStatusCode.InternalServerError)

            Finally
                
               
            End Try
        End Function


          <Route("syncstatus/{gd}"), HttpGet>
        Public Function SyncStatus(byval gd As String) As standardResponse
            Try

                If gd.Length > 0 Then

                   If (statusdict.ContainsKey(gd)) Then

                         Return New standardResponse() With {.IsSuccess = True, .Result = statusdict(gd).ToString}

                        Else
                         Return New standardResponse() With {.IsSuccess = False, .Result = "0"}
                   End If
                 

                    End If
                 Catch ex As Exception
                 Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Es ist ein Fehler aufgetreten")}
                Dim rg As New RaygunWebApiClient()
                rg.SendInBackground(ex)

                Throw New HttpResponseException(HttpStatusCode.InternalServerError)

            Finally
                
               
            End Try
        End Function

        <Route("autosynctrans/{kdnr}/{gd}"), HttpGet>
        Public Function AutoSyncTrans(ByVal kdnr As String,byval gd As String) As standardResponse
            Try

                If kdnr.Length > 0 and gd.Length > 0 Then

                    statusdict.Add(gd,1)

                    Dim session As Session = XpoHelper.GetNewSession()
                    Dim objKunde As Kunde = session.FindObject(Of Kunde)(New BinaryOperator("AdrNr", kdnr))
                    If IsNothing(objKunde) = False Then
                        Dim sConn As New SorbaConnector
                        For Each dobj As Domain In objKunde.Domains
                  

                       sconn.SorbaAutorisierung(dobj)

                        Next

                        statusdict(gd) = 2
                        Return New standardResponse() With {.IsSuccess = True, .Result = "Sync ist erfolgt"}
                    End If
                Else
                    Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Kunde im DMS nicht vorhanden")}
                End If
            Catch ex As Exception
                 statusdict.Remove(gd) 
                 Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Es ist ein Fehler aufgetreten")}
                Dim rg As New RaygunWebApiClient()
                rg.SendInBackground(ex)

                Throw New HttpResponseException(HttpStatusCode.InternalServerError)

            Finally
                
               
            End Try
        End Function
        <HttpPost, Route("dmsuserpwchange")>
        Public Function PWChange(ByVal req As MySorbaUser) As standardResponse
            Try

               

                If req IsNot Nothing Then
                    If req.Name.Length > 0 And req.KdNr.Length > 0 And req.Password.Length > 0 Then

                        If IsValidEmail(req.Name) Then

                            If req.Password.Length > 7 Then

                                Dim session As Session = XpoHelper.GetNewSession()

                                Dim objKunde As Kunde = session.FindObject(Of Kunde)(New BinaryOperator("AdrNr", req.KdNr))
                                If IsNothing(objKunde) = False Then

                                    Dim sConn As New SorbaConnector
                                    Dim strReturn As String = sConn.ChangePWDMSUser(objKunde, req.Name, req.Password)

                                    Dim strStatusCode As String = Between(strReturn, "<result>", "</result>")
                                    Dim strMessage As String = Between(strReturn, "<message>", "</message>")

                                    If strStatusCode = "1" Then
                                        Return New standardResponse() With {.IsSuccess = True, .Result = strMessage}
                                    Else
                                        Return New standardResponse() With {.IsSuccess = False, .Result = strMessage}
                                    End If

                                Else
                                    Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Kunde im DMS nicht vorhanden")}
                                End If

                            Else
                                Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Passwort ist zu kurz")}
                            End If

                        Else
                            Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Falsches Emailformat")}
                        End If

                    Else
                        Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Fehlende Daten")}
                    End If
                Else
                    Return New standardResponse() With {.IsSuccess = False, .Result = String.Format("Fehlende Daten / Model")}
                End If

            Catch ex As Exception
                Dim rg As New RaygunWebApiClient()
                rg.SendInBackground(ex)
                Throw New HttpResponseException(HttpStatusCode.InternalServerError)
            End Try
        End Function
        Public Shared Function IsValidEmail(email As String) As Boolean

            If String.IsNullOrWhiteSpace(email) Then Return False

            ' Use IdnMapping class to convert Unicode domain names.
            Try
                'Examines the domain part of the email and normalizes it.
                Dim DomainMapper =
                    Function(match As Match) As String

                        'Use IdnMapping class to convert Unicode domain names.
                        Dim idn = New IdnMapping

                        'Pull out and process domain name (throws ArgumentException on invalid)
                        Dim domainName As String = idn.GetAscii(match.Groups(2).Value)

                        Return match.Groups(1).Value & domainName

                    End Function

                'Normalize the domain
                email = Regex.Replace(email, "(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200))

            Catch e As RegexMatchTimeoutException
                Return False

            Catch e As ArgumentException
                Return False

            End Try

            Try
                Return Regex.IsMatch(email,
                                     "^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                     "(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                                     RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))

            Catch e As RegexMatchTimeoutException
                Return False

            End Try

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
End Namespace