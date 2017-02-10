variable "reporting-domains" {
    type = "string" 
}
variable "report-1st-receiving-domain" {
    type = "string"
}
variable "report-2nd-receiving-domain" {
    type = "string"
}
variable "report-staging-receiving-domain" {
    type = "string"
}
variable "env-name" {
    type = "string"
    default = "TF"
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
