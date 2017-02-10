variable "env-name" {
  type    = "string"
}
variable "ssh-key-name" {
  type    = "string"
}
variable "aws-account-id" {
  type    = "string"
}
variable "aws-region" {
  type    = "string"
}
variable "aws-region-name" {
  type    = "string"
}
variable "zone-count" {
  type    = "string"
}
variable "zone-names" {
  type    = "map"
}
variable "zone-subnets" {
  type    = "map"
}
variable "public-zone-count" {
  type    = "string"
}
variable "public-zone-names" {
  type    = "map"
}
variable "public-zone-subnets" {
  type    = "map"
}

variable "vpc-cidr-block" {
  type    = "string"
}
variable "db-size" {
  type    = "string"
}
variable "db-username" {
  type    = "string"
}
variable "db-password" {
  type    = "string"
}
variable "db-count" {
  type    = "string"
}
variable "build-vpc" {
  type    = "string"
}
variable "build-route-table" {
  type    = "string"
}
variable "bastion-ami" {
 type  	  = "string"
}
variable "bastion-type" {
 type     = "string"
}
variable "web-url" {
 type     = "string"
}
variable "acm-certificate" {
 type     = "string"
}
variable "email-domain" {
  type    = "string"
}
variable "cors-origin" {
  type    = "string"
}
variable "waf-acl" {
  type    = "string"
}
variable "parent-zone" {
  type    = "string"
}
variable "aggregate-report-bucket" {
  type    = "string"
}
variable "forensic-report-bucket" {
  type    = "string"
}

variable "db-api-uname" {
  type    = "string"
}
variable "db-api-pwd" {
  type    = "string"
}
variable "db-processor-uname" {
  type    = "string"
}
variable "db-processor-pwd" {
  type    = "string"
}
variable "db-name" {
  type    = "string"
}
variable "api-key" {
  type    = "string"
}
