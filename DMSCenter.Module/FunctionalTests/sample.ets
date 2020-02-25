;#DropDB DMSCenterEasyTest

#Application DMSCenter.Win
#Application DMSCenter.Web

*FillForm
 User Name = Admin
*Action Log In

#IfDef DMSCenter.Win

*Action About...
 
*Action OK

#EndIf

#IfDef DMSCenter.Web
*Action Navigation(My Details)
#EndIf
