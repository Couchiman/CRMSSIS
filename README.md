# CRMSSIS
CRM SSIS Integration Component. 

Need SQL Server Client SDK Installed. You can found it on SQL Server installation DVD or ISO.

Add SDK Server SDK libraries referenced on the project.
Download Nuget packages Dynamics CRM 365 version 9.007

Register in GAC - you could open "Developer Command Prompt for Visual Studio xx":

ex:
gacutil.exe -iF "YOURPATH\CRMSSIS.CRMConnectionManager.dll"

CRMSSIS.CRMConnectionManager.dll
CRMSSIS.CRMSourceAdapter.dll"
Microsoft.Xrm.Sdk.dll
Microsoft.Crm.Sdk.Proxy.dll
Microsoft.Xrm.Tooling.Connector.dll
Microsoft.Xrm.Tooling.Connector.dll
Microsoft.Xrm.Sdk.Deployment.dll
Microsoft.IdentityModel.Clients.ActiveDirectory.dll
Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll

Copy in SQL Server Directory version * 140 is for SQL Server 2017 

copy "C:\CRMSSIS\CRMConnectionManager\bin\Debug\CRMSSIS.CRMConnectionManager.dll" "C:\Program Files (x86)\Microsoft SQL Server\140\DTS\Connections" /y
copy "C:\CRMSSIS\CRMConnectionManager\bin\Debug\CRMSSIS.CRMConnectionManager.dll" "C:\Program Files\Microsoft SQL Server\140\DTS\Connections" /y

copy "C:\CRMSSIS\CRMSourceAdapter\bin\Debug\CRMSSIS.CRMSourceAdapter.dll" "C:\Program Files (x86)\Microsoft SQL Server\140\DTS\PipelineComponents" /y
copy "C:\CRMSSIS\CRMSourceAdapter\bin\Debug\CRMSSIS.CRMSourceAdapter.dll" "C:\Program Files\Microsoft SQL Server\140\DTS\PipelineComponents" /y
