resource "azurerm_resource_group" "ContentServerResourceGroup" {
  name     = var.ResourceGroupName
  location = var.ResourceGroupLocation
}

resource "azurerm_service_plan" "ContentServerAppServicePlan" {
  name                = var.AppServicePlanName
  location            = azurerm_resource_group.ContentServerResourceGroup.location
  resource_group_name = azurerm_resource_group.ContentServerResourceGroup.name

  os_type = "Windows"
  sku_name = "S1"
}

resource "azurerm_windows_web_app" "ContentServerAppService" {
  name                = var.AppServiceName
  location            = azurerm_resource_group.ContentServerResourceGroup.location
  resource_group_name = azurerm_resource_group.ContentServerResourceGroup.name
  service_plan_id     = azurerm_service_plan.ContentServerAppServicePlan.id

  site_config {
    virtual_application {
      virtual_path  = "/"
      physical_path = "site\\wwwroot"
      preload       = false
    }

    virtual_application {
      virtual_path  = "/JSAdapter12_aspnet"
      physical_path = "site\\wwwroot\\JSAdapter12_aspnet"
      preload       = false
    }

    application_stack {
      dotnet_version = "v10.0"
    }
    
    use_32_bit_worker = false # Set to false for 64-bit
  }
}
