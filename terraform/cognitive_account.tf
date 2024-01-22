resource "azurerm_cognitive_account" "example" {
  name = local.cognitive_account_name

  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  kind = "CognitiveServices" // Kind=CogntiveServices will create a multi-service account

  sku_name = "S0"

  tags = var.tags
}
