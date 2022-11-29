#!/bin/bash
###  This script is run only once during the initial setup process, use uks-lhcontentserver_cse_upgrade to push latest version of url rewriter
# Add IIS Server

$log_file =  "$env:SystemDrive\inetpub\wwwroot\log.txt"

$LHContentServerDownloadFolder =  "$env:SystemDrive\LearningHub"

$DeploymentEnvironment= "prod"

# Learning Hub Content Server artifacts location
$LHContentServerZippedFileUrl = "https://learninghubprodstor.blob.core.windows.net/contentserverartifacts/$DeploymentEnvironment/LearningHub.Nhs.Content.zip"
$LHContentServerZippedFile = 'LearningHub.Nhs.Content.zip'
$LHContentServerZippedFileLocation = "$LHContentServerDownloadFolder\$LHContentServerZippedFile"

$LHContentServerZippedFileExtactLocation = "$env:SystemDrive\inetpub\wwwroot"

# .NET Core Web Hosting Bundle installer location    
$Dotnet6WebHostingBundleInstallerUrl = "https://learninghubprodstor.blob.core.windows.net/contentserverartifacts/dotnet-hosting-6.0.10-win.exe"
$Dotnet6WebHostingBundleInstallerFile = "dotnet-hosting-6.0.10-win.exe"
$Dotnet6WebHostingBundleInstallerFileLocation = "$LHContentServerDownloadFolder\$Dotnet6WebHostingBundleInstallerFile"

#File share network mapped drive
$Share = 'learninghubprodstor'
$Folder = "resources$DeploymentEnvironment"

$User = "Azure\learninghubprodstor"
$Key = "Q2KWUV1oiETMkJ4V0eAxvES6Zih272BmvVW/wQsuHRsO0xnx33YjMbeZ0iQlm+8tejueeSYpSeGrhKTrPqT8Dg=="

$MountedDrive = "Z"

$ScriptExecutionContext = "Start"

function Write-Log {
    param(
    [parameter(Mandatory=$true)]
    [string]$Text,
    [parameter(Mandatory=$true)]
    [ValidateSet("WARNING","ERROR","INFO")]
    [string]$Type
    )

    [string]$logMessage = $(Get-Date).ToString('dd-MM-yyyy HH:mm:ss'), ":", $Type, $Text
    Add-Content -Path $log_file -Value $logMessage    
}

function Finish-Execution { 
    $log_file_new_path = "$env:SystemDrive\inetpub\wwwroot\JSAdapter12_aspnet"
    $log_file_new_location =  "$log_file_new_path\log.txt"    
    Write-Log -Text "Script execution finished" -Type INFO

    If (Test-Path -Path $log_file_new_path) {
        Move-Item -Path $log_file -Destination $log_file_new_location -Force      
    }
}


try{

    #Check if the initial setup has been complete and only needs new version of url rewiter/scorm adapter to be deployed
    If (Test-Path -Path "${MountedDrive}:") {    
        Write-Log -Text "Started Upgrading Content server with latest version" -Type INFO
        Write-Log -Text "Preparing to Download and configure scorm content server and adapter" -Type INFO

        # Create a temp folder to download artifacts
        if( ![System.IO.Directory]::Exists( $LHContentServerDownloadFolder ))
        {   
           md $LHContentServerDownloadFolder
           Write-Log -Text "Created Temp download folder" -Type INFO   
        }

        # Download Content Server zipped artifact
            Invoke-WebRequest -Uri $LHContentServerZippedFileUrl -OutFile $LHContentServerZippedFileLocation
            Write-Log -Text "Completed downloading Content Server zipped artifact" -Type INFO    
    
        ## Stop IIS
           iisreset /stop
           Write-Log -Text "IIS Stopped" -Type INFO      
        ## Stop IIS

        # Extract Content Server zipped artifact
            Expand-Archive -LiteralPath $LHContentServerZippedFileLocation -DestinationPath $LHContentServerZippedFileExtactLocation -Force
	        Write-Log -Text "Completed Extract Content Server zipped artifact" -Type INFO    

        ## Restrat IIS
           iisreset /start
           Write-Log -Text "IIS Restarted" -Type INFO    
        ## Restrat IIS

        Write-Log -Text "VM Configuration Complete" -Type INFO
        Write-Log -Text "Finished Upgrading Content server with latest version" -Type INFO
        Finish-Execution    
    }
    else{
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
        Invoke-WebRequest -Uri $Dotnet6WebHostingBundleInstallerUrl -OutFile $Dotnet6WebHostingBundleInstallerFileLocation    
	    Write-Log -Text "Downloaded .NET 6 Hosting Bundle " -Type INFO
          
        # Install .NET Core hosting bundle
            Start-Process $LHContentServerDownloadFolder\dotnet-hosting-6.0.10-win.exe -ArgumentList '/install /quiet /norestart'        
            Write-Log -Text "Completed Installing .NET 6 hosting bundle" -Type INFO  

        # Create a temp folder to download artifacts
        if( ![System.IO.Directory]::Exists( $LHContentServerDownloadFolder ))
        {   
           md $LHContentServerDownloadFolder
           Write-Log -Text "Created Temp download folder" -Type INFO   
        }

        Enable-WindowsOptionalFeature -Online -FeatureName IIS-HostableWebCore
	
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
        
        Write-Log -Text "Testing the netconnection for fileshare" -Type INFO        
        Test-NetConnection -ComputerName "$Share.file.core.windows.net" -Port 445
        Write-Log -Text "Finished Testing the netconnection for fileshare" -Type INFO
           
        # Save the password so the drive will persist on reboot
        Invoke-Expression -Command "cmdkey /add:$Share.file.core.windows.net /user:$User /pass:$Key"
        # Mount the drive
        Write-Log -Text "Running command New-PSDrive to mount the drive" -Type INFO
        New-PSDrive -Name $MountedDrive -PSProvider FileSystem -Root $Root -Persist -Credential $Credential -Scope Global
        Write-Log -Text "Finished Running command New-PSDrive to mount the drive" -Type INFO
        
        If (Test-Path -Path "${MountedDrive}:") {
	        Write-Log -Text "Mounted network drive Z: is accessible" -Type INFO	
        }else{
	        Write-Log -Text "Mounted network drive Z: is not accessible : $Error[0]" -Type ERROR
            Finish-Execution
            throw "Unable to mount network drive  Z:"
        }
        
        Write-Log -Text "Creating Local User and apppool" -Type INFO
  
        # Create new local user with same name and password as Azure File Share         
        $pwd = ConvertTo-SecureString -AsPlainText "$Key" -Force
        New-LocalUser -PasswordNeverExpires -Name "$Share" -Password $pwd 
        Add-LocalGroupMember -Group IIS_IUSRS -Member "$Share"
        Write-Log -Text "created local user" -Type INFO	
        
        # Create new App Pool & set identity to new local user
        $app_pool_name = "contentserverapppool"
        New-WebAppPool -Name $app_pool_name
        Set-ItemProperty IIS:\AppPools\$app_pool_name -name processModel.identityType -Value SpecificUser 
        Set-ItemProperty IIS:\AppPools\$app_pool_name -name processModel.userName -Value "$Share"
        Set-ItemProperty IIS:\AppPools\$app_pool_name -name processModel.password -Value "$Key"
        Set-ItemProperty 'IIS:\Sites\Default Web Site' applicationPool $app_pool_name
        Write-Log -Text "created apppool under local user" -Type INFO	
        
        ## Restrat IIS
           iisreset
        ## Restrat IIS
        Write-Log -Text "VM Configuration Complete" -Type INFO

        Finish-Execution
    }   
}
 catch{
    Write-Log -Text "$Error[0]" -Type ERROR	
    Finish-Execution
    }