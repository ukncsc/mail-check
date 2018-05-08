variable "vpc-id" {
  description = "Id of the VPC in which to create this service"
}

variable "parameter-name" {
  description = "passed as a parameter to the server"
  default     = ""
}

variable "ami" {
  description = "id of ami if you want to override the latest Amazon ECS image"
  default     = ""
}

variable "ssh-key-name" {
  description = "SSH key name in your AWS account for AWS instances."
}

variable "availability-zones" {
  description = "Comma separated list of EC2 availability zones to launch instances, must be within region"
}

variable "subnet-ids" {
  description = "Comma separated list of subnet ids, must match availability zones"
}

variable "connection-string" {
  description = "database connection string"
  default     = ""
}

variable "parameter-store-access" {
  description = "attach IAM instance polices to allow access to parameter store"
  default     = "false"
}

variable "eip-allocation" {
  description = "attach IAM instance policy to allow eip allocation"
  default     = "false"
}

variable "sqs-access" {
  description = "attache IAM instance policy to allow SQS"
  default     = "false"
}

variable "sns-access" {
  description = "attache IAM instance policy to allow SNS"
  default     = "false"
}

variable "db-access" {
  description = "add rule to allow access to RDS"
  default     = "false"
}

variable "db-port" {
  description = "port for db access"
  default     = "3306"
}

variable "rds-sg-id" {
  description = "RDS security group id, must be defined if db-access is true"
  default     = ""
}

variable "subnet-cidr" {
  description = "Comma separated list of subnets in CIDR notation"
}

variable "elb-ingress-subnets" {
  description = "Comma separated list of subnets"
  default     = ""
}

variable "elb-ingress-subnets2" {
  description = "Comma separated list of subnets if there are too many for a security group"
  default     = ""
}

variable "egress-internet-custom-ports" {
  description = "custom outbound ports to the internet, comma separated"
  default     = ""
}

variable "egress-internet-custom-ports-count" {
  description = "number of custom outbound ports to the internet, comma separated"
  default     = "0"
}

variable "egress-custom-port1" {
  description = "custom outbound port to the internet"
  default     = ""
}

variable "egress-custom-destination1" {
  description = "custom outbound destination"
  default     = "0.0.0.0/0"
}

variable "egress-custom-port2" {
  description = "custom outbound port"
  default     = ""
}

variable "egress-custom-destination2" {
  description = "custom outbound destination"
  default     = "0.0.0.0/0"
}

variable "egress-custom-port3" {
  description = "custom outbound port"
  default     = ""
}

variable "egress-custom-destination3" {
  description = "custom outbound destination"
  default     = "0.0.0.0/0"
}

variable "egress-custom-port4" {
  description = "custom outbound port"
  default     = ""
}

variable "egress-custom-destination4" {
  description = "custom outbound destination"
  default     = "0.0.0.0/0"
}

variable "egress-custom-port5" {
  description = "custom outbound port"
  default     = ""
}

variable "egress-custom-destination5" {
  description = "custom outbound destination"
  default     = "0.0.0.0/0"
}

variable "default-instance-type" {
  description = "Name of the AWS instance type used for all branches except prod and stage"
}

variable "default-instance-count" {
  description = "Number of AWS instances used for all branches except prod and stage"
}

variable "prod-stage-instance-type" {
  description = "Name of the AWS instance type used in prod and stage"
}

variable "prod-stage-instance-count" {
  description = "Number of AWS instances used in prod and stage"
}

variable "prod-stage-scaling-multiplier" {
  description = "Max autoscaling capacity is prod-stage-instance-count multiplied by this number"
  default     = "2"
}

variable "container-memory" {
  description = "MB of memory to assign to the container"
}

variable "min-size" {
  default     = "1"
  description = "Minimum number of instances to run in the group"
}

variable "max-size" {
  default     = "6"
  description = "Maximum number of instances to run in the group"
}

variable "desired-capacity" {
  default     = "2"
  description = "Desired number of instances to run in the group"
}

variable "health-check-grace-period" {
  default     = "300"
  description = "Time after instance comes into service before checking health"
}

variable "env-name" {
  description = "environment name"
}

variable "aws-region" {}

variable "cluster-name" {}

variable "aws-account-id" {}

variable "cloudwatch-alerts-sns-arn" {
  description = "SNS topic destination for cloudwatch alerts"
}

variable "admin-subnets" {
  description = "source subnets for SSH sessions"
  default     = ""
}

variable "cluster-ingress-ports" {
  type        = "string"
  description = "comma separated list of ports"
  default     = ""
}

variable "cluster-ingress-ports-count" {
  type        = "string"
  description = "Number of port mappings to process (can't be computed)"
  default     = "0"
}

variable "allow-inter-instance-traffic" {
  type        = "string"
  description = "create a security group rule to allow instances to communicate on docker-ingress-ports ports"
  default     = "false"
}

variable "lb-sg-id" {
  type        = "list"
  description = "List of security group ids"
  default     = []
}

variable "lb-sg-count" {
  type        = "string"
  description = "Number of items in the list of security group ids"
  default     = "0"
}

variable "lb-arn" {
  type        = "string"
  description = "arn of load balancer"
  default     = ""
}

variable "lb-arn-count" {
  type        = "string"
  description = "1 if there is a load balancer"
  default     = "0"
}

variable "launch-runcmd" {
  type        = "string"
  description = "commands to run when a new instance is launched by autoscaling"
  default     = ""
}

variable "launch-packages" {
  type        = "string"
  description = "packages to install"
  default     = ""
}

variable "prod-environments" {
  type        = "list"
  description = "list of environments to get the production infrastructure scaling"
  default     = ["prod", "staging", "stage"]
}

variable "custom-policy-arn" {
  type        = "string"
  description = "Allow a custom policy to be attached to the cluster instances"
  default     = ""
}
