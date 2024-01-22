variable "workload" {
  default     = "poc"
  description = "The short name of the workload, used in naming of resources"
}

variable "environment" {
  default     = "dev"
  description = "The environment instance for the workload deployment, used in naming resources"
}

variable "location" {
  default     = "uksouth"
  description = "The location that the environment instance will be deployed to, used in naming resources"
}

variable "subscription_id" {
  description = "The subscription id that the environment instance will be deployed to"
}

variable "tags" {
  default     = {}
  description = "The tags to apply to all resources where possible"
}

variable "development_principal_id" {
  default     = "f4518b1d-4bbe-48de-be7f-cad566b20936"
  description = "The principal id of the development user"
}
