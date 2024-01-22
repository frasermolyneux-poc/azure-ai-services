locals {
  resource_group_name         = "rg-${var.workload}-${var.environment}-${var.location}"
  cognitive_account_name      = "cog-${var.workload}-${var.environment}-${var.location}-${random_id.environment_id.hex}"
  search_service_account_name = "search-${var.workload}-${var.environment}-${var.location}-${random_id.environment_id.hex}"

  storage_account_name = "sa${var.location}${random_id.environment_id.hex}"

  webapp_app_registration_name = "Azure AI Services Web App - ${var.environment}"
}
