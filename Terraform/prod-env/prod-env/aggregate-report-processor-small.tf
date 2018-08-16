module "aggregate-report-processor-small" {
  source                        = "../../modules/lambdaProcessor-module"
  env-name                      = "${var.env-name}"
  aws-region                    = "${var.aws-region}"
  aws-account-id                = "${var.aws-account-id}"
  lambda-filename               = "AggregateReportParser.zip"
  source-code-hash              = "${base64sha256(file("AggregateReportParser.zip"))}"
  lambda-function-name          = "TF-${var.env-name}-aggregate-report-parser-small"
  handler                       = "Dmarc.AggregateReport.Parser.Lambda::Dmarc.AggregateReport.Parser.Lambda.AggregateReportProcessor::HandleScheduledEvent"
  connection-string             = "Server = ${aws_rds_cluster.rds-cluster.endpoint}; Port = 3306; Database = dmarc; Uid = ${var.env-name}_${lookup(var.db-users,"aggregateparser")};Connection Timeout=5;"
  subnet-ids                    = "${join(",", aws_subnet.dmarc-env-subnet.*.id)}"
  security-group-ids            = "${aws_security_group.lambda-parser.id}"
  lambda-memory                 = "128"
  lambda-timeout                = "300"
  scheduler-interval            = "1"
  RemainingTimeThresholdSeconds = "30"
  QueueUrl                      = "${aws_sqs_queue.aggregate-report-queue1.id}"
  sqs-queue-count               = 1
  sqs-queue-arns                = "${aws_sqs_queue.aggregate-report-queue1.arn}"
  s3-bucket-arns                = "arn:aws:s3:::${var.aggregate-report-bucket}/*"
  environment = {DkimSelectorsTopicArn = "${aws_sns_topic.selector-seen-in-aggregate-report.arn}"}
  MaxS3ObjectSizeKilobytes      = "100"
  sns-arns = [ "${aws_sns_topic.selector-seen-in-aggregate-report.arn}" ]

}
