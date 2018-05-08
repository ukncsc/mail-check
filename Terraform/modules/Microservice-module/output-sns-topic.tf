resource "aws_sns_topic" "microservice-output" {
  name = "TF-${var.env-name}-${var.service-name}-output"
}
