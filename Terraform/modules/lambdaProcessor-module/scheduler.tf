resource "aws_cloudwatch_event_rule" "scheduler" {
  name                = "${aws_lambda_function.scheduled-lambda-function.function_name}-${var.scheduler-interval}-${var.scheduler-interval == "1" ? "min" : "mins"}"
  description         = "Fires every ${var.scheduler-interval} minutes"
  schedule_expression = "rate(${var.scheduler-interval} ${var.scheduler-interval == "1" ? "minute" : "minutes"})"
}

resource "aws_cloudwatch_event_target" "run_lambda_from_scheduler" {
  rule      = "${aws_cloudwatch_event_rule.scheduler.name}"
  target_id = "${aws_lambda_function.scheduled-lambda-function.function_name}"
  arn       = "${aws_lambda_function.scheduled-lambda-function.arn}"
}

resource "aws_lambda_permission" "allow_cloudwatch_to_call_lambda" {
  statement_id  = "AllowExecutionFromCloudWatch"
  action        = "lambda:InvokeFunction"
  function_name = "${aws_lambda_function.scheduled-lambda-function.function_name}"
  principal     = "events.amazonaws.com"
  source_arn    = "${aws_cloudwatch_event_rule.scheduler.arn}"
}
