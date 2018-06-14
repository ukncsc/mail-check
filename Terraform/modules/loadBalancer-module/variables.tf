variable "vpc-id" {
  description = "Id of the VPC in which to create this service"
}

variable "lb-subnet-ids" {
  description = "Comma separated list of subnet ids, must match availability zones"
}

variable "instance-subnet-cidr" {
  description = "Comma separated list of subnets in CIDR notation"
}

variable "lb-ingress-subnets1" {
  description = "Comma separated list of subnets"
  default     = ""
}

variable "lb-ingress-subnets2" {
  description = "Comma separated list of subnets if there are too many for a security group"
  default     = ""
}

variable "env-name" {
  description = "environment name"
}

variable "aws-region" {}

variable "health-check-grace-period" {
  default = ""
  type    = "string"
}

variable "lb-internal" {
  description = "create an internal rather than internet facing load balancer by default"
  default     = "true"
}

variable "aws-account-id" {}

variable "route53-zone-id" {}

variable "lb-target-count" {
  description = "number of load balancer target groups / routes"
  default     = "1"
  type        = "string"
}

variable "lb-target-paths" {
  description = "comma separated list of paths"
  type        = "string"
}

variable "balancer-name" {
  description = "name of the load balancer"
  type        = "string"
}

variable "balancer-fqdn" {
  description = "FQDN of load balancer and certificate blank for internal"
  type        = "string"
  default     = ""
}

variable "certificate-name" {
  description = "If defined, use this certificate which must exist in ACM, otherwise look for a wildcard cert on the internal domain"
  type        = "string"
  default     = ""
}

variable "admin-subnets" {
  description = "source subnets for test sessions"
}

variable "cloudwatch-alerts-sns-arn" {
  description = "SNS topic destination for cloudwatch alerts"
}

variable "lb-type" {
  description = "load balancer type network or application"
  default     = "application"
}

variable "healthcheck-path" {
  description = "path to request for load balancer health check"
  default     = "/healthcheck"
}

variable "healthcheck-protocol" {
  description = "protocol to use for health check"
  default     = "HTTP"
}

variable "http-listener" {
  description = "if true listen on port 80"
  default     = "false"
}

variable "lb-target-path-prefix" {
  description = "prefix to add before all path rules"
  default     = ""
}
