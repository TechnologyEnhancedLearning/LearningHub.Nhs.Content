### Used to set up the initial environment if torn down completely
### ResourceGroup must be created first manually.
### Safetynet:
throw 'Never run this script twice - IP address must be static and not overwritten, check before commenting out this line.'
###

Login-AzAccount
Get-AzSubscription
select-AzSubscription -Subscription "eLfH DevTest"

$DeploymentEnvironment= "UAT"

$resourcegroup = "UKS-LEARNINGHUB-CONTENTSERVER-$DeploymentEnvironment-RG"
$region = "UK South"
$vnetname = "UKS-LHCONTENTSERVER-$DeploymentEnvironment-VNET"
$snetname = "UKS-LHCONTENTSERVER-$DeploymentEnvironment-SNET"
$pipname = "UKS-LHCONTENTSERVER-$DeploymentEnvironment-PIP"
$nsgname = "UKS-LHCONTENTSERVER-$DeploymentEnvironment-NSG"

#write-host "****** Creating Resource Group"
#New-AzResourceGroup -Name $resourcegroup -Location $region -Force

write-host "****** Create a virtual network subnet"
$subnet = New-AzVirtualNetworkSubnetConfig `
  -Name "$snetname" `
  -AddressPrefix 10.0.0.0/24

write-host "******  Create a virtual network"
$vnet = New-AzVirtualNetwork `
    -ResourceGroupName $resourcegroup `
    -Name "$vnetname" `
    -Location $region `
    -AddressPrefix 10.0.0.0/16 `
    -Subnet $subnet  

write-host "******  Create a public IP address"
$publicIP = New-AzPublicIpAddress `
  -ResourceGroupName $resourcegroup `
  -Location $region `
  -AllocationMethod Static `
  -sku Standard `
  -Name $pipname

write-host "***** Create a rule to allow traffic over port 80"
$nsgFrontendRule80 = New-AzNetworkSecurityRuleConfig `
-Name awdelfferule `
-Protocol Tcp `
-Direction Inbound `
-Priority 200 `
-SourceAddressPrefix * `
-SourcePortRange * `
-DestinationAddressPrefix * `
-DestinationPortRange 80 `
-Access Allow

write-host "***** Create a network security group and associate it with the rule"
$nsgFrontend = New-AzNetworkSecurityGroup `
-ResourceGroupName  $resourcegroup `
-Location $region `
-Name $nsgname `
-SecurityRules $nsgFrontendRule80

write-host "***** Get vnet variable"
$ssvnet = Get-AzVirtualNetwork `
-ResourceGroupName  $resourcegroup `
-Name "$vnetname"

$frontendSubnet = $ssvnet.Subnets[0]

write-host "***** Set NSG on subnet"
Set-AzVirtualNetworkSubnetConfig `
-VirtualNetwork $ssvnet `
-Name "$snetname" `
-AddressPrefix $frontendSubnet.AddressPrefix `
-NetworkSecurityGroup $nsgFrontend

Set-AzVirtualNetwork -VirtualNetwork $ssvnet