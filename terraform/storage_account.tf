resource "azurerm_storage_account" "app_data" {
  name = local.storage_account_name

  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location

  account_tier             = "Standard"
  account_replication_type = "ZRS"

  tags = var.tags

  allow_nested_items_to_be_public = true
}

resource "azurerm_storage_container" "images" {
  name                  = "images"
  storage_account_name  = azurerm_storage_account.app_data.name
  container_access_type = "blob"
}

resource "azurerm_role_assignment" "deploy_principal_app_data_sbdc" {
  count                = var.environment == "scratch" ? 1 : 0
  scope                = azurerm_storage_account.app_data.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = var.development_principal_id
}
