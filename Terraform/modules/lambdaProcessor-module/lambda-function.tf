resource "aws_lambda_function" "scheduled-lambda-function" {
  filename         = "${var.lambda-filename}"
  function_name    = "${var.lambda-function-name}"
  role             = "${aws_iam_role.lambda-processor.arn}"
  runtime          = "dotnetcore2.1"
  handler          = "${var.handler}"
  source_code_hash = "${var.source-code-hash}"
  memory_size      = "${var.lambda-memory}"
  timeout          = "${var.lambda-timeout}"

  environment {
    variables = "${merge(map("ConnectionString", "${var.connection-string}",
      "RemainingTimeThresholdSeconds" , "${var.RemainingTimeThresholdSeconds}",
      "QueueUrl"                      , "${var.QueueUrl}",
      "TimeoutSqsSeconds"             , "${var.TimeoutSqsSeconds}",
      "TimeoutS3Seconds"              , "${var.TimeoutS3Seconds}",
      "MaxS3ObjectSizeKilobytes"      , "${var.MaxS3ObjectSizeKilobytes}",
      "RefreshIntervalSeconds"        , "${var.RefreshIntervalSeconds}",
      "FailureRefreshIntervalSeconds" , "${var.FailureRefreshIntervalSeconds}",
      "DnsRecordLimit"                , "${var.DnsRecordLimit}",
      "SnsTopicArn"                   , "${aws_sns_topic.lambdaprocessor-output.arn}"
    ),var.environment)}"
  }

  vpc_config {
    subnet_ids         = ["${split(",", var.subnet-ids)}"]
    security_group_ids = ["${split(",", var.security-group-ids)}"]
  }
}
