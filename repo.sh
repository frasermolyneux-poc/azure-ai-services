cd terraform

terraform init --backend-config=backends/scratch.backend.hcl

terraform plan -var-file=tfvars/scratch.tfvars

terraform apply -var-file=tfvars/scratch.tfvars --auto-approve

terraform destroy -var-file=tfvars/scratch.tfvars --auto-approve
