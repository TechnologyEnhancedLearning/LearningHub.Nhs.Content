#!/bin/bash
###  Used by each VM in the scaleset to set up its internal Windows/IIS env and configure installs
# Add IIS Server

$log_file =  "$env:SystemDrive\inetpub\wwwroot\log.txt"

$LHContentServerDownloadFolder =  "$env:SystemDrive\LearningHub"

# Learning Hub Content Server artifacts location
$LHContentServerZippedFileUrl = 'https://ukselfhdevlhcontentstore.blob.core.windows.net/contentserverartifacts/dev/LearningHub.Nhs.Content.zip'
$LHContentServerZippedFile = 'LearningHub.Nhs.Content.zip'
$LHContentServerZippedFileLocation = "$LHContentServerDownloadFolder\$LHContentServerZippedFile"

$LHContentServerZippedFileExtactLocation = "$env:SystemDrive\inetpub\wwwroot"

# .NET Core Web Hosting Bundle installer location
$DotnetCore2WebHostingBundleInstallerUrl = 'https://ukselfhdevlhcontentstore.blob.core.windows.net/contentserverartifacts/dotnet-hosting-2.2.2-win.exe'
$DotnetCore2WebHostingBundleInstallerFile = 'dotnet-hosting-2.2.2-win.exe'
$DotnetCore2WebHostingBundleInstallerFileLocation = "$LHContentServerDownloadFolder\$DotnetCore2WebHostingBundleInstallerFile"
    
# .NET Core 5 Web Hosting Bundle installer location
$DotnetCore5WebHostingBundleInstallerUrl = 'https://ukselfhdevlhcontentstore.blob.core.windows.net/contentserverartifacts/dotnet-hosting-5.0.6-win.exe'
$DotnetCore5WebHostingBundleInstallerFile = 'dotnet-hosting-5.0.6-win.exe'
$DotnetCore5WebHostingBundleInstallerFileLocation = "$LHContentServerDownloadFolder\$DotnetCore5WebHostingBundleInstallerFile"

#File share network mapped drive
$Share = 'ukselfhdevlhcontentstore'
$Folder = 'resourcesdev'

$User = "Azure\ukselfhdevlhcontentstore"
$Key = 'swyZwYQOA6EWDG0A33juC5nqUUIFh+QVWXHxTZZeVv1oyiRnDCnx8m4UHtwuodnSHiPxEhw5j+KMT7BJ+ba6iw=='


function Write-Log {
    param(
    [parameter(Mandatory=$true)]
    [string]$Text,
    [parameter(Mandatory=$true)]
    [ValidateSet("WARNING","ERROR","INFO")]
    [string]$Type
    )

    [string]$logMessage = [System.String]::Format("[$(Get-Date)] -"),$Type, $Text
    Add-Content -Path $log_file -Value $logMessage
}

function Finish-Execution {    
    $log_file_new_destination =  "$env:SystemDrive\inetpub\wwwroot\JSAdapter12_aspnet\log.txt"    
    Write-Log -Text "Script execution finished" -Type INFO  

    #If (Test-Path -Path $log_file) {
        #Move-Item -Path $log_file -Destination $log_file_new_destination
    #}
}

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

Import-Module webadministration

Write-Log -Text "IIS Configured" -Type INFO

Write-Log -Text "Preparing to Download and configure scorm content server and adapter" -Type INFO

# Create a temp folder to download artifacts
if( ![System.IO.Directory]::Exists( $LHContentServerDownloadFolder ))
{   
   md $LHContentServerDownloadFolder
   Write-Log -Text "Created Temp download folder" -Type INFO   
}

# Download .NET Core Web Hosting Bundle installer
try{
    Invoke-WebRequest -Uri $DotnetCore2WebHostingBundleInstallerUrl -OutFile $DotnetCore2WebHostingBundleInstallerFileLocation    
	Write-Log -Text "Downloaded .NET Core Hosting Bundle" -Type INFO
}
catch{
	Write-Log -Text "Failed to Download.NET Core Hosting Bundle" -Type ERROR
    Finish-Execution
	throw 'Failed to Download.NET Core Hosting Bundle'
}
  
# Install .NET Core hosting bundle
    Start-Process $LHContentServerDownloadFolder\dotnet-hosting-2.2.2-win.exe -ArgumentList '/install /quiet /norestart'        
    Write-Log -Text "Completed Installing .NET Core hosting bundle" -Type INFO    

# Create a temp folder to download artifacts
if( ![System.IO.Directory]::Exists( $LHContentServerDownloadFolder ))
{   
   md $LHContentServerDownloadFolder
   Write-Log -Text "Created Temp download folder" -Type INFO   
}

# Download .NET Core 5 Web Hosting Bundle installer
    Invoke-WebRequest -Uri $DotnetCore5WebHostingBundleInstallerUrl -OutFile $DotnetCore5WebHostingBundleInstallerFileLocation    
    Write-Log -Text "Downloaded .NET Core Hosting Bundle" -Type INFO

# Install .NET Core hosting bundle
Start-Process $LHContentServerDownloadFolder\dotnet-hosting-5.0.6-win.exe -ArgumentList '/install /quiet /norestart'

Enable-WindowsOptionalFeature -Online -FeatureName IIS-HostableWebCore
Enable-WindowsOptionalFeature -Online -FeatureName Web-WHC

Write-Log -Text "Installed .NET Core 5 Hosting Bundle" -Type INFO
	
# Download Content Server zipped artifact
    Invoke-WebRequest -Uri $LHContentServerZippedFileUrl -OutFile $LHContentServerZippedFileLocation
    Write-Log -Text "Completed downloading Content Server zipped artifact" -Type INFO    

# Extract Content Server zipped artifact
    Expand-Archive -LiteralPath $LHContentServerZippedFileLocation -DestinationPath $LHContentServerZippedFileExtactLocation
	Write-Log -Text "Completed Extract Content Server zipped artifact" -Type INFO    
    
# Convert SCORM Adapter to Web Application
    ConvertTo-WebApplication "IIS:\Sites\Default Web Site\JSAdapter12_aspnet"
 
## Restrat IIS
   iisreset
   Write-Log -Text "IIS Restarted" -Type INFO    
## Restrat IIS

Write-Log -Text "Finished configuring scorm content server and adapter under default website" -Type INFO


Write-Log -Text "Preparing to Mount Network drive" -Type INFO

$PWord = ConvertTo-SecureString -String "$Key" -AsPlainText -Force
$Credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $User, $PWord

$Root = "\\$Share.file.core.windows.net\$Folder"
$ContentPath = "$env:SystemDrive\inetpub\wwwroot\Default.htm"

Set-Content -Path  $ContentPath -Value "Host: $($env:computername)"

Test-NetConnection -ComputerName "$Share.file.core.windows.net" -Port 445

# Save the password so the drive will persist on reboot
Invoke-Expression -Command "cmdkey /add:$Share.file.core.windows.net /user:$User /pass:$Key"

# Mount the drive
New-PSDrive -Name Z -PSProvider FileSystem -Root $Root -Persist -Credential $Credential -Scope Global

Write-Log -Text "Mounted network drive" -Type INFO

If (Test-Path -Path Z:) {
	Write-Log -Text "Mounted network drive Z: is accessible" -Type INFO	
}else{
	Write-Log -Text "Mounted network drive Z: is not accessible" -Type ERROR
    Finish-Execution
    throw "Unable to mount network drive  Z:"
}

Write-Log -Text "Mapping Network drive as virtual directory and creating apppools" -Type INFO
    
# Create Virtual Directory
$physicalPath = "\\$Share.file.core.windows.net\$Folder"
$virtualDirectoryPath = "IIS:\Sites\Default Web Site\content"
New-Item $virtualDirectoryPath -type VirtualDirectory -physicalPath $physicalPath
Set-ItemProperty $virtualDirectoryPath -Name username -Value "$User"
Set-ItemProperty $virtualDirectoryPath -Name password -Value "$Key"
New-WebVirtualDirectory -Site "Default Web Site" -Name content -PhysicalPath $physicalPath -Force

Write-Log -Text "content virtual directory created mapping to network drive" -Type INFO	

# Create new local user with same name and password as Azure File Share 
try{
$pwd = ConvertTo-SecureString -AsPlainText "$Key" -Force
New-LocalUser -PasswordNeverExpires -Name "$Share" -Password $pwd 
Add-LocalGroupMember -Group IIS_IUSRS -Member "$Share"
Write-Log -Text "created local user" -Type INFO	
}
catch{
    Write-Log -Text "failed creating local user" -Type ERROR	
}

# Create new App Pool & set identity to new local user
try{
$app_pool_name = "contentserverapppool"
New-WebAppPool -Name $app_pool_name
Set-ItemProperty IIS:\AppPools\$app_pool_name -name processModel.identityType -Value SpecificUser 
Set-ItemProperty IIS:\AppPools\$app_pool_name -name processModel.userName -Value "$Share"
Set-ItemProperty IIS:\AppPools\$app_pool_name -name processModel.password -Value "$Key"
Set-ItemProperty 'IIS:\Sites\Default Web Site' applicationPool $app_pool_name
Write-Log -Text "created apppool under local user" -Type INFO	
}
catch{
    Write-Log -Text "failed creating app pool under local user" -Type ERROR	
}
try{
# Change anonymous authentication of content site to App Pool
set-webconfigurationproperty /system.webServer/security/authentication/anonymousAuthentication -name userName -value "" -location "Default Web Site/content"
Write-Log -Text "changed anonymous authentication of content site to App Pool" -Type INFO
}
catch{
    Write-Log -Text "failed setting webconfigurationproperty anonymousAuthentication" -Type ERROR	
}
## Restrat IIS
   iisreset
## Restrat IIS
Write-Log -Text "VM Configuration Complete" -Type INFO

Finish-Execution