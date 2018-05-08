resource "aws_sns_topic" "lambdaprocessor-output" {
  name = "TF-${var.env-name}-${var.lambda-function-name}-output"
}
