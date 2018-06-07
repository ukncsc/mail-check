variable "vpc-id" {
  description = "Id of the VPC in which to create this service"
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

variable "default-task-count" {
  description = "Number of AWS instances used for all branches except prod and stage"
}

variable "prod-stage-task-count" {
  description = "Number of AWS instances used in prod and stage"
}

variable "container-memory" {
  description = "MB of memory to assign to the container"
}

variable "health-check-grace-period" {
  default     = "300"
  description = "Time after instance comes into service before checking health"
}

variable "registry-url" {
  default     = ""
  description = "Docker private registry URL"
}

variable "env-name" {
  description = "environment name"
}

variable "aws-region" {}

variable "service-name" {}

variable "server-name" {
  description = "DNS name - defaults to env-service"
  default     = ""
}

variable "aws-account-id" {}

variable "cache-node-type" {
  description = "redis cluster created if not null"
  default     = ""
}

variable "cache-node-failover" {
  description = "create a failover cluster, must be bigger than t2 or will fail"
  default     = "false"
}

variable "cloudwatch-alerts-sns-arn" {
  description = "SNS topic destination for cloudwatch alerts"
}

variable "processor-only" {
  type        = "string"
  description = "Does not create load balancer or autoscaling group"
  default     = "false"
}

variable "docker-environment" {
  type        = "list"
  description = "list of environment variables in format Name=Value"
  default     = []
}

variable "docker-environment-count" {
  type        = "string"
  description = "Number of variables to process (can't be computed)"
  default     = "0"
}

variable "docker-port-mapping" {
  type        = "list"
  description = "list of port mappings in addition to the load balancer port eg 80:80 where instance:container"
  default     = []
}

variable "docker-port-mapping-count" {
  type        = "string"
  description = "Number of port mappings to process (can't be computed)"
  default     = "0"
}

variable "port" {
  type        = "string"
  description = "TCP port used for ECS"
  default     = "80"
}

variable "dynamic-port" {
  type        = "string"
  description = "if true a dynamic port is mapped and communicated to the load balancer"
  default     = "true"
}

variable "allow-inter-instance-traffic" {
  type        = "string"
  description = "create a security group rule to allow instances to communicate on docker-port-mapping ports"
  default     = "false"
}

variable "input-queue-subscriptions" {
  type        = "string"
  description = "comma separated list of input queue subscriptions"
  default     = ""
}

variable "input-queue-subscription-count" {
  type        = "string"
  description = " number of SQS queue subscriptions"
  default     = "0"
}

variable "raw-queue-subscription" {
  type        = "string"
  description = "If true, won't add to the original SNS message"
  default     = "false"
}

variable "cluster-id" {
  type        = "string"
  description = "The cluster id of the ECS cluster on which to run this microservice task"
  default     = ""
}

variable "ecs-service-role-arn" {
  type        = "string"
  description = "ARN of the cluster role that can add to the load balancer"
  default     = ""
}

variable "target-group-arn" {
  type        = "string"
  description = "ARN of the load balancer target group"
  default     = ""
}

variable "command" {
  type        = "string"
  description = "Command parameter"
  default     = ""
}

variable "parameter" {
  type        = "string"
  description = "Command 2nd parameter"
  default     = ""
}

variable "volumelist" {
  type        = "list"
  description = "list of maps of volumes"

  default = []
}

variable "volumelist-count" {
  type        = "string"
  description = "Number of maps in volumelist"
  default     = "0"
}
