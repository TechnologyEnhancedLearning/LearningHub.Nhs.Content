provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "ContentServerResourceGroup" {
  name     = "UKS-LearningHub-ContentServer-DEV-RG"
  location = "UK South"
}

resource "azurerm_app_service_plan" "ContentServerAppServicePlan" {
  name                = "learninghub-contnet-dev-plan"
  location            = azurerm_resource_group.ContentServerResourceGroup.location
  resource_group_name = azurerm_resource_group.ContentServerResourceGroup.name
  kind                = "Windows"
  reserved            = true
  sku {
    tier = "Standard"
    size = "S1"
  }
}

resource "azurerm_app_service" "ContentServerAppService" {
  name                = "learninghub-content-dev"
  location            = azurerm_resource_group.ContentServerResourceGroup.location
  resource_group_name = azurerm_resource_group.ContentServerResourceGroup.name
  app_service_plan_id = azurerm_app_service_plan.ContentServerAppServicePlan.id
}
