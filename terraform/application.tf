resource "azuread_application" "webapp" {
  display_name = local.webapp_app_registration_name
  description  = "Azure AI Services Web App"

  owners = [data.azuread_client_config.current.object_id]

  sign_in_audience = "AzureADMyOrg"

  web {
    redirect_uris = [
      "http://localhost:48745/signin-oidc",
      "http://localhost:5161/signin-oidc",
      "https://localhost:44338/signin-oidc",
      "https://localhost:7045/signin-oidc",
    ]

    implicit_grant {
      access_token_issuance_enabled = false
      id_token_issuance_enabled     = true
    }
  }
}
