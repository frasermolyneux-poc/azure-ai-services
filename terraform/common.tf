data "azurerm_client_config" "current" {}

data "azuread_client_config" "current" {}

// This resource provides a unique id for the environment per environment instance (e.g. per creation of a new state file)
// The value of this should be appended to all names that are required to be globally unique to prevent naming conflicts.
resource "random_id" "environment_id" {
  byte_length = 4
}

// This resource will change every 30 days and can be used for secret rotation
resource "time_rotating" "thirty_days" {
  rotation_days = 30
}

// This resource will be destroyed and recreated every execution and can be used to manage resource locks
resource "random_id" "lock" {
  keepers = {
    id = "${timestamp()}"
  }
  byte_length = 8
}
