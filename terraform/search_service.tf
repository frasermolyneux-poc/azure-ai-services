resource "azurerm_search_service" "search" {
  name = local.search_service_account_name

  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location

  sku = "standard"

  local_authentication_enabled = false
}

resource "azurerm_role_assignment" "deploy_principal_search_ssc" {
  count                = var.environment == "scratch" ? 1 : 0
  scope                = azurerm_search_service.search.id
  role_definition_name = "Search Service Contributor"
  principal_id         = var.development_principal_id
}

resource "azurerm_role_assignment" "deploy_principal_search_sidc" {
  count                = var.environment == "scratch" ? 1 : 0
  scope                = azurerm_search_service.search.id
  role_definition_name = "Search Index Data Contributor"
  principal_id         = var.development_principal_id
}
