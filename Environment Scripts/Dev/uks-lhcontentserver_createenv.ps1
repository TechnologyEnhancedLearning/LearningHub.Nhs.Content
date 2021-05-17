### Used to set up the machines, env must exist (use _createenv_ script first)

Login-AzAccount
Get-AzSubscription
select-AzSubscription -Subscription "eLfH DevTest"

# before running this script, delete all resources, excluding the vnet and public IP address
# if the ne-elfh-esrproxy-rg and network resources do not exist. Please run elfproxypoc_createnetwork, before continuing with this script. 

$resourcegroup = "UKS-ELFH-ESRPROXY-RG"
$region = "UK South"
$ssname = "uks-LHContentServer-ss"
$vnetname = "UKS-LHCONTENTSERVER-VNET"
$pipname = "UKS-LHCONTENTSERVER-PIP"
$AdminUsername = "windowsadmin";
$AdminPassword = "P@ssword" + $resourcegroup;

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
  -Name "LHContentServer-FPool" `
  -PublicIpAddress $publicIP
$backendPool = New-AzLoadBalancerBackendAddressPoolConfig -Name "LHContentServer-BPool"

write-host "****** Create a Network Address Translation (NAT) pool"
# A rule for Remote Desktop Protocol (RDP) traffic is created on TCP port 3389
$inboundNATPool = New-AzLoadBalancerInboundNatPoolConfig `
  -Name "LHContentServer-RDPRule" `
  -FrontendIpConfigurationId $frontendIP.Id `
  -Protocol TCP `
  -FrontendPortRangeStart 50001 `
  -FrontendPortRangeEnd 50010 `
  -BackendPort 3389

write-host "****** Create the load balancer"
$lb = New-AzLoadBalancer `
  -ResourceGroupName $resourcegroup `
  -Name "LHContentServer-LB" `
  -Location $region `
  -Sku Standard `
  -FrontendIpConfiguration $frontendIP `
  -BackendAddressPool $backendPool `
  -InboundNatPool $inboundNATPool

write-host "****** Create a load balancer health probe for TCP port 80"
Add-AzLoadBalancerProbeConfig -Name "LHContentServer-probe" `
  -LoadBalancer $lb `
  -Protocol TCP `
  -Port 80 `
  -IntervalInSeconds 15 `
  -ProbeCount 2

write-host "****** Create a load balancer rule to distribute traffic on port TCP 80"
# The health probe from the previous step is used to make sure that traffic is
# only directed to healthy VM instances
Add-AzLoadBalancerRuleConfig `
  -Name "LHContentServer-LBRule" `
  -LoadBalancer $lb `
  -FrontendIpConfiguration $lb.FrontendIpConfigurations[0] `
  -BackendAddressPool $lb.BackendAddressPools[0] `
  -Protocol TCP `
  -FrontendPort 80 `
  -BackendPort 80 `
  -Probe (Get-AzLoadBalancerProbeConfig -Name "LHContentServer-probe" -LoadBalancer $lb)

write-host "****** Update the load balancer configuration"
Set-AzLoadBalancer -LoadBalancer $lb

write-host "****** Create IP address configurations"
$ipConfig = New-AzVmssIpConfig `
  -Name "LHContentServer-IPConfig" `
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
$publicSettings = @{
    "fileUris" = (,"https://ukselfhproxyfs.blob.core.windows.net/elfhartifacts/LHContentServer/uks-lhcontentserver_cse_v1.0.ps1");
    "commandToExecute" = "powershell -ExecutionPolicy Unrestricted -File uks-lhcontentserver_cse_v1.0.ps1"
}

write-host "***** Run Custom Script Extension to install IIS and configure basic website"
Add-AzVmssExtension -VirtualMachineScaleSet $vmss `
  -Name "customScript" `
  -Publisher "Microsoft.Compute" `
  -Type "CustomScriptExtension" `
  -TypeHandlerVersion 1.9 `
  -Setting $publicSettings

write-host "***** Update the scale set and apply the Custom Script Extension to the VM instances"
Update-AzVmss `
  -ResourceGroupName $resourcegroup `
  -Name $ssname `
  -VirtualMachineScaleSet $vmss


write-host "***** Update the scale set and apply the Custom Script Extension to the VM instances"
Update-AzVmss `
-ResourceGroupName $resourcegroup `
-Name $ssname `
-VirtualMachineScaleSet $vmss
