output "lambda-arn" {
  value = "${aws_lambda_function.scheduled-lambda-function.arn}"
}

output "sns-arn" {
  value = "${aws_sns_topic.lambdaprocessor-output.arn}"
}
