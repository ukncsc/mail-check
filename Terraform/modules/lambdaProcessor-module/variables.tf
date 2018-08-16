variable "scheduler-interval" {}
variable "env-name" {}
variable "subnet-ids" {}
variable "security-group-ids" {}
variable "aws-region" {}
variable "aws-account-id" {}
variable "lambda-filename" {}
variable "lambda-function-name" {}
variable "connection-string" {}
variable "handler" {}
variable "source-code-hash" {}

variable "lambda-memory" {
  type    = "string"
  default = "128"
}

variable "lambda-timeout" {
  type    = "string"
  default = "30"
}

variable "TimeoutSqsSeconds" {
  type    = "string"
  default = "10"
}

variable "TimeoutS3Seconds" {
  type    = "string"
  default = "10"
}

variable "MaxS3ObjectSizeKilobytes" {
  type    = "string"
  default = "20000"
}

variable "QueueUrl" {
  type    = "string"
  default = ""
}

variable "RemainingTimeThresholdSeconds" {
  type    = "string"
  default = "10"
}

variable "RefreshIntervalSeconds" {
  type    = "string"
  default = ""
}

variable "FailureRefreshIntervalSeconds" {
  type    = "string"
  default = ""
}

variable "DnsRecordLimit" {
  type    = "string"
  default = ""
}

variable "sqs-queue-arns" {
  type    = "string"
  default = ""
}

variable "sqs-queue-count" {
  type    = "string"
  default = "0"
}

variable "s3-bucket-arns" {
  type    = "string"
  default = ""
}

variable "environment" {
  type = "map"
  default = {}
}

variable "sns-arns" {
  type = "list"
  default = []
}