### Used to set up the machines, env must exist (use uks-lhcontentserver_createnetwork first)

Login-AzAccount
Get-AzSubscription
select-AzSubscription -Subscription "eLfH Prod"

# before running this script, delete all resources, excluding the vnet and public IP address
# if the network resources do not exist. Please run uks-lhcontentserver_createnetwork, before continuing with this script. 

$DeploymentEnvironment= "prod"
$ArtifactLocation = "https://learninghubprodstor.blob.core.windows.net/contentserverartifacts/"

$CustomScriptFileName = "$DeploymentEnvironment/uks-lhcontentserver_cse.ps1"

$DeploymentEnvironment= "PROD"

$resourcegroup = "UKS-LEARNINGHUB-CONTENTSERVER-$DeploymentEnvironment-RG"
$region = "UK South"
$ssname = "UKS-LHCONTENTSERVER-$DeploymentEnvironment-SS"
$vnetname = "UKS-LHCONTENTSERVER-$DeploymentEnvironment-VNET"
$pipname = "UKS-LHCONTENTSERVER-$DeploymentEnvironment-PIP"

$AdminUsername = "windowsadmin";
$AdminPassword = "Pr0dCOnSvR@dM1N";

write-host "****** Get vnet ********"
$vnet = Get-AzVirtualNetwork `
  -ResourceGroupName $resourcegroup `
  -Name "$vnetname" 

write-host "****** Get public IP address ******"
$publicIP = Get-AzPublicIpAddress `
  -Name $pipname `
  -ResourceGroupName $resourcegroup

write-host "****** Create a frontend and backend IP pool"
$frontendIP = New-AzLoadBalancerFrontendIpConfig `
  -Name "UKS-LHCONTENTSERVER-$DeploymentEnvironment-FPool" `
  -PublicIpAddress $publicIP
$backendPool = New-AzLoadBalancerBackendAddressPoolConfig -Name "UKS-LHCONTENTSERVER-$DeploymentEnvironment-BPOOL"

write-host "****** Create a Network Address Translation (NAT) pool"
# A rule for Remote Desktop Protocol (RDP) traffic is created on TCP port 3389
$inboundNATPool = New-AzLoadBalancerInboundNatPoolConfig `
  -Name "UKS-LHCONTENTSERVER-$DeploymentEnvironment-RDPRULE" `
  -FrontendIpConfigurationId $frontendIP.Id `
  -Protocol TCP `
  -FrontendPortRangeStart 50001 `
  -FrontendPortRangeEnd 50010 `
  -BackendPort 3389

write-host "****** Create the load balancer"
$lb = New-AzLoadBalancer `
  -ResourceGroupName $resourcegroup `
  -Name "UKS-LHCONTENTSERVER-$DeploymentEnvironment-LB" `
  -Location $region `
  -Sku Standard `
  -FrontendIpConfiguration $frontendIP `
  -BackendAddressPool $backendPool `
  -InboundNatPool $inboundNATPool

write-host "****** Create a load balancer health probe for TCP port 80"
Add-AzLoadBalancerProbeConfig -Name "UKS-LHCONTENTSERVER-$DeploymentEnvironment-PROBE" `
  -LoadBalancer $lb `
  -Protocol TCP `
  -Port 80 `
  -IntervalInSeconds 15 `
  -ProbeCount 2

write-host "****** Create a load balancer rule to distribute traffic on port TCP 80"
# The health probe from the previous step is used to make sure that traffic is
# only directed to healthy VM instances
Add-AzLoadBalancerRuleConfig `
  -Name "UKS-LHCONTENTSERVER-$DeploymentEnvironment-LBRule" `
  -LoadBalancer $lb `
  -FrontendIpConfiguration $lb.FrontendIpConfigurations[0] `
  -BackendAddressPool $lb.BackendAddressPools[0] `
  -Protocol TCP `
  -FrontendPort 80 `
  -BackendPort 80 `
  -Probe (Get-AzLoadBalancerProbeConfig -Name "UKS-LHCONTENTSERVER-$DeploymentEnvironment-PROBE" -LoadBalancer $lb)

write-host "****** Update the load balancer configuration"
Set-AzLoadBalancer -LoadBalancer $lb

write-host "****** Create IP address configurations"
$ipConfig = New-AzVmssIpConfig `
  -Name "UKS-LHCONTENTSERVER-$DeploymentEnvironment-IPCONFIG" `
  -LoadBalancerBackendAddressPoolsId $lb.BackendAddressPools[0].Id `
  -LoadBalancerInboundNatPoolsId $inboundNATPool.Id `
  -SubnetId $vnet.Subnets[0].Id

write-host "****** Create a config object"
# The VMSS config object stores the core information for creating a scale set
$vmssConfig = New-AzVmssConfig `
    -Location $region `
    -SkuCapacity 2 `
    -SkuName "Standard_DS1_V2" `
    -UpgradePolicyMode "Automatic"

write-host "****** Reference a virtual machine image from the gallery"
Set-AzVmssStorageProfile $vmssConfig `
  -OsDiskCreateOption "FromImage" `
  -ImageReferencePublisher "MicrosoftWindowsServer" `
  -ImageReferenceOffer "WindowsServer" `
  -ImageReferenceSku "2019-Datacenter" `
  -ImageReferenceVersion "latest"

write-host "****** Set up information for authenticating with the virtual machine"
Set-AzVmssOsProfile $vmssConfig `
  -AdminUsername $AdminUsername `
  -AdminPassword $AdminPassword `
  -ComputerNamePrefix "LHConSrVM"

write-host "****** # Attach the virtual network to the config object"
Add-AzVmssNetworkInterfaceConfiguration `
  -VirtualMachineScaleSet $vmssConfig `
  -Name "network-config" `
  -Primary $true `
  -IPConfiguration $ipConfig

write-host "****** Create the scale set with the config object (this step might take a few minutes)"
$vmss = New-AzVmss `
  -ResourceGroupName $resourcegroup `
  -Name $ssname `
  -VirtualMachineScaleSet $vmssConfig
  
write-host "****** Define the script for your Custom Script Extension to run"

$customConfig = @{
    "fileUris" = (,"$ArtifactLocation$CustomScriptFileName");
    "commandToExecute" = "powershell -ExecutionPolicy Unrestricted -File $CustomScriptFileName"
}

write-host "***** Run Custom Script Extension to install IIS and configure basic website"

# Add the Custom Script Extension to install IIS and configure basic website
$vmss = Add-AzVmssExtension `
  -VirtualMachineScaleSet $vmss `
  -Name "customScript" `
  -Publisher "Microsoft.Compute" `
  -Type "CustomScriptExtension" `
  -TypeHandlerVersion 1.9 `
  -Setting $customConfig

Write-Output "Started to update vm scale set"

write-host "***** Update the scale set and apply the Custom Script Extension to the VM instances"
Update-AzVmss `
  -ResourceGroupName $resourcegroup `
  -Name $ssname `
  -VirtualMachineScaleSet $vmss
