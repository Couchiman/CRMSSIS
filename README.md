# CRMSSIS
CRM SSIS Integration Components

This are a basic Connection Manager and Source Adapter. 
Destination Adapter is still pending task. SQL Server 2016 & 2017 problems with Connection Manager not be recognized on Pipeline Validation. Any help appreciated.



Need SQL Server Client SDK Installed. You can found it on SQL Server installation DVD or ISO.

Add SDK Server SDK libraries referenced on the project, Verify added references propertities to be: 

"Copy Local = false" 
"Embed Interop Types= false"

Restore Nuget packages Dynamics CRM 365 version 9.007

Register in GAC - you could open "Developer Command Prompt for Visual Studio xx":

ex:
gacutil.exe -iF "YOURPATH\CRMSSIS.CRMConnectionManager.dll"

CRMSSIS.CRMConnectionManager.dll
CRMSSIS.CRMSourceAdapter.dll
Microsoft.Xrm.Sdk.dll
Microsoft.Crm.Sdk.Proxy.dll
Microsoft.Xrm.Tooling.Connector.dll
Microsoft.Xrm.Tooling.Connector.dll
Microsoft.Xrm.Sdk.Deployment.dll
Microsoft.IdentityModel.Clients.ActiveDirectory.dll
Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll

Use SQL Server Directory version

140 for SQL Server 2017 --> Still not working. NEED HELP!
130 for SQL Server 2016 --> Still not working. NEED HELP!
120 for SQL Server 2014
110 for SQL Server 2012

Fast deploy example:

gacutil.exe -u CRMSSIS.CRMConnectionManager
gacutil.exe -iF "C:\CRMSSIS\CRMConnectionManager\bin\Debug\CRMSSIS.CRMConnectionManager.dll"
copy "C:\CRMSSIS\CRMConnectionManager\bin\Debug\*.dll" "C:\Program Files (x86)\Microsoft SQL Server\140\DTS\Connections" /y
copy "C:\CRMSSIS\CRMConnectionManager\bin\Debug\*.dll" "C:\Program Files\Microsoft SQL Server\140\DTS\Connections" /y


gacutil.exe -u CRMSSIS.CRMSourceAdapter
gacutil.exe -iF "C:\CRMSSIS\CRMSourceAdapter\bin\Debug\CRMSSIS.CRMSourceAdapter.dll"
copy "C:\CRMSSIS\CRMSourceAdapter\bin\Debug\*.dll" "C:\Program Files (x86)\Microsoft SQL Server\140\DTS\PipelineComponents" /y
copy "C:\CRMSSIS\CRMSourceAdapter\bin\Debug\*.dll" "C:\Program Files\Microsoft SQL Server\140\DTS\PipelineComponents" /y
