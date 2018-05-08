variable "env-name" {
  type = "string"
}

variable "ssh-key-name" {
  type = "string"
}

variable "aws-account-id" {
  type = "string"
}

variable "aws-region" {
  type = "string"
}

variable "aws-region-name" {
  type = "string"
}

variable "aws-secondary-region" {
  description = "For services not supported in the primary region"
  type        = "string"
}

variable "aws-secondary-region-name" {
  description = "For services not supported in the primary region"
  type        = "string"
}

variable "zone-count" {
  type = "string"
}

variable "zone-names" {
  type = "map"
}

variable "zone-subnets" {
  type = "map"
}

variable "frontend-zone-names" {
  type = "map"
}

variable "frontend-zone-subnets" {
  type = "map"
}

variable "public-zone-count" {
  type = "string"
}

variable "natgw-count" {
  type = "string"
}

variable "public-zone-names" {
  type = "map"
}

variable "public-zone-subnets" {
  type = "map"
}

variable "vpc-cidr-block" {
  type = "string"
}

variable "db-master-size" {
  type = "string"
}

variable "db-replica-size" {
  type = "string"
}

variable "db-replica-count" {
  type = "string"
}

variable "db-snapshot-to-restore" {
  type    = "string"
  default = ""
}

variable "db-username" {
  type = "string"
}

variable "db-password" {
  type = "string"
}

variable "build-vpc" {
  type = "string"
}

variable "build-route-table" {
  type = "string"
}

variable "bastion-ami" {
  type = "string"
}

variable "bastion-type" {
  type = "string"
}

variable "web-url" {
  type = "string"
}

variable "email-domain" {
  type = "string"
}

variable "cors-origin" {
  type = "string"
}

variable "parent-zone" {
  type = "string"
}

variable "aggregate-report-bucket" {
  type = "string"
}

variable "forensic-report-bucket" {
  type = "string"
}

variable "create-buckets" {
  type    = "string"
  default = "0"
}

variable "db-users" {
  type = "map"

  default = {
    dnsrecordevaluator = "dnseval"
    aggregateparser    = "aggproc"
    domaininformation  = "dnsproc"
    forensicparser     = "forproc"
    statsapi           = "statapi"
    statusapi          = "domstat"
    adminapi           = "admin"
    metricsapi         = "metrics"
    securitytester     = "sectest"
    securityevaluator  = "seceval"
  }
}

variable "db-name" {
  type = "string"
}

variable "auth-OIDC-client-id" {
  type = "string"
}

variable "auth-OIDC-client-secret" {
  type = "string"
}

variable "auth-OIDC-provider-metadata" {
  type    = "string"
  default = ""
}

variable "default-instance-type" {
  type    = "string"
  default = "t2.micro"
}

variable "default-container-memory" {
  type    = "string"
  default = "300"
}

variable "disable-firewall" {
  type    = "string"
  default = "false"
}

variable "allow-external" {
  type    = "string"
  default = "false"
}

variable "dotnet-container-githash" {
  type = "string"
}

variable "frontend-container-githash" {
  type = "string"
}

variable "backup-account" {
  type    = "string"
  default = ""
}

variable "frontend-OIDCCryptoPassphrase" {
  type = "string"
}
