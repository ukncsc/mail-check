resource "aws_s3_bucket_notification" "aggregate_bucket_notification" {
  // provider = "aws.secondary"

  bucket = "${var.aggregate-report-bucket}"

  queue {
    queue_arn = "${aws_sqs_queue.aggregate-report-queue1.arn}"
    events    = ["s3:ObjectCreated:*"]
  }
}
