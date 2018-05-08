resource "aws_sns_topic" "cloudwatch-alerts" {
  name = "TF-${var.env-name}-cloudwatch-alerts"
}
