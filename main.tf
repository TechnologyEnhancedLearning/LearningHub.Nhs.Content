resource "azurerm_resource_group" "ContentServerResourceGroup" {
  name     = var.ResourceGroupName
  location = var.ResourceGroupLocation
}

resource "azurerm_app_service_plan" "ContentServerAppServicePlan" {
  name                = var.AppServicePlanName
  location            = azurerm_resource_group.ContentServerResourceGroup.location
  resource_group_name = azurerm_resource_group.ContentServerResourceGroup.name
  kind                = "app"
  reserved            = false
  sku {
    tier = var.AppServicePlanSkuTier
    size = var.AppServicePlanSkuSize
  }
}

resource "azurerm_app_service" "ContentServerAppService" {
  name                = var.AppServiceName
  location            = azurerm_resource_group.ContentServerResourceGroup.location
  resource_group_name = azurerm_resource_group.ContentServerResourceGroup.name
  app_service_plan_id = azurerm_app_service_plan.ContentServerAppServicePlan.id
}
