variable "env-name" {
  description = "environment name, added to all objects, keep it short"
  type        = "string"
}

variable "ssh-key-name" {
  description = "Default SSH key to apply to EC2 instances"
  type        = "string"
}

variable "aws-account-id" {
  description = "AWS Account number"
  type        = "string"
}

variable "aws-region" {
  description = "AWS region in which to deploy the environment"
  type        = "string"
}

variable "aws-region-name" {
  description = "friendly name for AWS region for descriptions"
  type        = "string"
}

variable "aws-secondary-region" {
  description = "For the Common environment using cross environment services such as SES"
  type        = "string"
}

variable "aws-secondary-region-name" {
  description = "friendly name for secondary AWS region for descriptions"
  type        = "string"
}

variable "zone-count" {
  description = "number of AZs in which to deploy the solution"
  type        = "string"
}

variable "zone-names" {
  description = "backend AZ names"
  type        = "map"
}

variable "zone-subnets" {
  description = "backend AZ subnets"
  type        = "map"
}

variable "frontend-zone-names" {
  description = "frontend AZ names"
  type        = "map"
}

variable "frontend-zone-subnets" {
  description = "frontend AZ subnets"
  type        = "map"
}

variable "public-zone-count" {
  description = "Number of publc zones"
  type        = "string"
}

variable "natgw-count" {
  description = "number of NAT gateways to create, could be less than AZs"
  type        = "string"
}

variable "public-zone-names" {
  description = "public internet facing AZ names"
  type        = "map"
}

variable "public-zone-subnets" {
  description = "public internet facing AZ subnets"
  type        = "map"
}

variable "vpc-cidr-block" {
  description = "CIDR block for entire VPC"
  type        = "string"
}

variable "db-master-size" {
  description = "Master database instance size"
  type        = "string"
}

variable "db-replica-size" {
  description = "Replica database instance size"
  type        = "string"
}

variable "db-replica-count" {
  description = "Number of replicas"
  type        = "string"
}

variable "db-snapshot-to-restore" {
  description = "Name of database snapshot, blank for default database"
  type        = "string"
  default     = ""
}

variable "db-kms-key-id" {
  description = "KMS key to encrypt database at rest"
  type        = "string"
  default     = ""
}

variable "db-name" {
  description = "database name"
  type        = "string"
}

variable "db-username" {
  description = "database admin username"
  type        = "string"
}

variable "db-password" {
  description = "database admin password"
  type        = "string"
}

variable "build-vpc" {
  description = "VPC id to create VPC peering and routing"
  type        = "string"
}

variable "build-route-table" {
  description = "build VPC routing table in which to create a route"
  type        = "string"
}

variable "web-url" {
  description = "Base URL for the web app"
  type        = "string"
}

variable "email-domain" {
  type = "string"
}

variable "cors-origin" {
  description = "CORS origin for API calls"
  type        = "string"
}

variable "parent-zone" {
  type = "string"
}

variable "aggregate-report-bucket" {
  description = "Name of the S3 bucket to receive aggregate reports from SES"
  type        = "string"
}

variable "create-buckets" {
  description = "Manage the S3 bucket with this terraform (1)"
  type        = "string"
  default     = "0"
}

variable "db-users" {
  description = "map of database users to a short name that fits within the MySQL username constraints"
  type        = "map"

  default = {
    dnsrecordevaluator = "dnseval"
    aggregateparser    = "aggproc"
    domaininformation  = "dnsproc"
    statsapi           = "statapi"
    statusapi          = "domstat"
    adminapi           = "admin"
    metricsapi         = "metrics"
    securitytester     = "sectest"
    securityevaluator  = "seceval"
    quicksight         = "quickst"
    reports            = "reports"
  }
}

variable "auth-OIDC-client-id" {
  description = "OIDC client id passed to the apache module mod_auth_oid"
  type        = "string"
}

variable "auth-OIDC-client-secret" {
  description = "OIDC client secret passed to the apache module mod_auth_oid"
  type        = "string"
}

variable "auth-OIDC-provider-metadata" {
  description = "OIDC metadata URI endpoint passed to the apache module mod_auth_oid to receive configuration for that provider"
  type        = "string"
  default     = ""
}

variable "frontend-OIDCCryptoPassphrase" {
  description = "Encryption key used to store session information in the frontend cache"
  type        = "string"
}

variable "default-instance-type" {
  description = "If not overridden (most are), all instances will be of this type"
  type        = "string"
  default     = "t2.micro"
}

variable "default-container-memory" {
  description = "Amount of RAM to assign containers if not overriden"
  type        = "string"
  default     = "300"
}

variable "disable-firewall" {
  description = "If true the frontend web server will be open to the internet"
  type        = "string"
  default     = "false"
}

variable "allow-external" {
  description = "If true allows IP addresses classed as external access to the frontend web server"
  type        = "string"
  default     = "false"
}

variable "dotnet-container-githash" {
  description = "Passed in by the pipeline to automatically upgrade API containers to the latest build by modify the task definition in the infrastructure"
  type        = "string"
}

variable "frontend-container-githash" {
  description = "Passed in by the pipeline to automatically upgrade frontend containers to the latest build by modify the task definition in the infrastructure"

  type = "string"
}

variable "backup-account" {
  description = "Account used to store database snapshots for DR purposes"
  type        = "string"
  default     = ""
}

variable "KmsKeySource" {
  description = "KMS key used to store database snapshots for DR purposes"

  type    = "string"
  default = ""
}

variable "role-to-assume" {
  description = "Role for terraform to assume with permissions in the correct account"
  type        = "string"
  default     = ""
}

variable "allowed-account-ids" {
  description = "List of account ids that terraform is allowd to modify"
  type        = "string"
  default     = ""
}

variable "build-account-id" {
  description = "AWS account id containing the build environment for VPC peering"
  type        = "string"
  default     = ""
}

variable "build-vpc-cidr-block" {
  description = "CIDR block of the build VPC to create a route"
  type        = "string"
  default     = ""
}

variable "ecr-aws-account-id" {
  description = "AWS account id for docker container registries (ECR)"
  type        = "string"
}

variable "db-microservice-certificate-evaluator-snapshot-to-restore" {
  description = "DB snapshot to restore into the certificate evaluator microservice"
  type        = "string"
  default     = ""
}

variable "db-microservice-reverse-dns-snapshot-to-restore" {
  description = "DB snapshot to restore into the reverse dns microservice"
  type        = "string"
  default     = ""
}

variable "db-microservice-dkim-snapshot-to-restore" {
  description = "DB snapshot to restore into the dkim microservice"
  type        = "string"
  default     = ""
}

variable "db-microservice-spf-snapshot-to-restore" {
  description = "DB snapshot to restore into the spf microservice"
  type        = "string"
  default     = ""
}

variable "s3-replication-source-aws-account-id" {
  description = "If the source S3 bucket for reports is in a different account this will set up the bucket policy"
  type        = "string"
  default     = ""
}
