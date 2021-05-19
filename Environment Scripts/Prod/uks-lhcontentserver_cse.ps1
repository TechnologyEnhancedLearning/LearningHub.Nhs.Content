#!/bin/bash
###  Used by each VM in the scaleset to set up its internal Windows/IIS env and configure installs

#$Share = 'ukselfhproxyfs'
#$Folder = 'elfh-content-share'
#$User = "Azure\$Share"
##$Key = 'j5IVEi8ayZUjYoEOWQHTbKqAXr84jEjcvFUgCElV3QA7qlwem0YRCkEc7RdfmJec4xOlT14blR18y/gI5lBv2w=='
#$Key = 'qON7X1Sjo1V5itEU8l+vR51aNBsnNJl1PtD2vS2qN7hk9T8kyHE3Tv9k1rIOi33w3BwBvxbgIHQJBxeL1OeZnQ=='
#$PWord = ConvertTo-SecureString -String "$Key" -AsPlainText -Force
#$Credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $User, $PWord

$Share = 'ukselfhdevlhcontentstore'
$Folder = 'resources-dev'
$User = "Azure\$Share"
$Key = 'swyZwYQOA6EWDG0A33juC5nqUUIFh+QVWXHxTZZeVv1oyiRnDCnx8m4UHtwuodnSHiPxEhw5j+KMT7BJ+ba6iw=='
$PWord = ConvertTo-SecureString -String "$Key" -AsPlainText -Force
$Credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $User, $PWord

# Add IIS Server
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer
Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpRedirect
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationDevelopment

Enable-WindowsOptionalFeature -online -FeatureName NetFx4Extended-ASPNET45
Enable-WindowsOptionalFeature -Online -FeatureName IIS-NetFxExtensibility45

Enable-WindowsOptionalFeature -Online -FeatureName IIS-HealthAndDiagnostics
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpLogging
Enable-WindowsOptionalFeature -Online -FeatureName IIS-LoggingLibraries
Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestMonitor
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpTracing
Enable-WindowsOptionalFeature -Online -FeatureName IIS-Security
Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestFiltering
Enable-WindowsOptionalFeature -Online -FeatureName IIS-Performance
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerManagementTools
Enable-WindowsOptionalFeature -Online -FeatureName IIS-IIS6ManagementCompatibility
Enable-WindowsOptionalFeature -Online -FeatureName IIS-Metabase
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ManagementConsole
Enable-WindowsOptionalFeature -Online -FeatureName IIS-BasicAuthentication
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WindowsAuthentication
Enable-WindowsOptionalFeature -Online -FeatureName IIS-StaticContent
Enable-WindowsOptionalFeature -Online -FeatureName IIS-DefaultDocument
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebSockets
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationInit
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ISAPIExtensions
Enable-WindowsOptionalFeature -Online -FeatureName IIS-ISAPIFilter
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpCompressionStatic

Enable-WindowsOptionalFeature -Online -FeatureName IIS-ASPNET45


# add urlrewriter - not in standard components
#choco install urlrewrite -y


Import-Module webadministration
Set-Content -Path 'C:\inetpub\wwwroot\Default.htm' -Value "Host: $($env:computername)"
Test-NetConnection -ComputerName "$Share.file.core.windows.net" -Port 445

# Save the password so the drive will persist on reboot
Invoke-Expression -Command "cmdkey /add:$Share.file.core.windows.net /user:$User /pass:$Key"

# Mount the drive
New-PSDrive -Name Z -PSProvider FileSystem -Root "\\$Share.file.core.windows.net\$Folder" -Persist -Credential $Credential

# Copy the ESR SCORM adapter & create WebApplication
#Invoke-WebRequest -Uri "https://$Share.blob.core.windows.net/elfhartifacts/JSAdapter12_aspnet.zip" -outfile 'C:\inetpub\wwwroot\JSAdapter12_aspnet.zip'
#New-WebVirtualDirectory -Site "Default Web Site" -Name JSAapter12_aspnet -physicalPath C:\inetpub\wwwroot\JSAdapter12_aspnet 
#Expand-Archive -LiteralPath C:\inetpub\wwwroot\JSAdapter12_aspnet.zip -DestinationPath C:\inetpub\wwwroot\JSAdapter12_aspnet
#ConvertTo-WebApplication "IIS:\Sites\Default Web Site\JSAdapter12_aspnet"
Invoke-WebRequest -Uri "https://$Share.blob.core.windows.net/elfhartifacts/LHContentServer/LHContentServer.zip" -outfile 'C:\inetpub\wwwroot\LHContentServer.zip'
New-WebVirtualDirectory -Site "Default Web Site" -Name LHContentServer -physicalPath C:\inetpub\wwwroot\LHContentServer 
Expand-Archive -LiteralPath C:\inetpub\wwwroot\LHContentServer.zip -DestinationPath C:\inetpub\wwwroot\LHContentServer
ConvertTo-WebApplication "IIS:\Sites\Default Web Site\LHContentServer"

# Create Virtual Directory
$physicalPath = "\\$Share.file.core.windows.net\$Folder"
$virtualDirectoryPath = "IIS:\Sites\Default Web Site\content"
New-Item $virtualDirectoryPath -type VirtualDirectory -physicalPath $physicalPath
Set-ItemProperty $virtualDirectoryPath -Name username -Value "$User"
Set-ItemProperty $virtualDirectoryPath -Name password -Value "$Key"
New-WebVirtualDirectory -Site "Default Web Site" -Name content -PhysicalPath $physicalPath -Force

# Create new local user with same name and password as Azure File Share 
$pwd = ConvertTo-SecureString -AsPlainText "$Key" -Force
New-LocalUser -PasswordNeverExpires -Name "$Share" -Password $pwd 
Add-LocalGroupMember -Group IIS_IUSRS -Member "$Share"

# Create new App Pool & set identity to new local user
$app_pool_name = "esrproxyapppool"
New-WebAppPool -Name $app_pool_name
Set-ItemProperty IIS:\AppPools\$app_pool_name -name processModel.identityType -Value SpecificUser 
Set-ItemProperty IIS:\AppPools\$app_pool_name -name processModel.userName -Value "$Share"
Set-ItemProperty IIS:\AppPools\$app_pool_name -name processModel.password -Value "$Key"
Set-ItemProperty 'IIS:\Sites\Default Web Site' applicationPool $app_pool_name

# Change anonymous authentication of content site to App Pool
set-webconfigurationproperty /system.webServer/security/authentication/anonymousAuthentication -name userName -value "" -location "Default Web Site/content"

# try to change the vm image to server core
# try to select the DS1 image
# try to change vm to managed vm
