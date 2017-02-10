resource "aws_sqs_queue" "forensic-report-queue1" {
  name = "${var.env-name}-forensic"
  delay_seconds = 0
//  fifo_queue = true
//  content_based_deduplication = true
  max_message_size = 2048
// 14 day maximum
  message_retention_seconds = 1209600
// Lambda to end as soon as possible no long queuing
  receive_wait_time_seconds = 0
  redrive_policy = "{\"deadLetterTargetArn\":\"${aws_sqs_queue.forensic-report-queue2.arn}\",\"maxReceiveCount\":2}"
  policy = <<POLICY
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": "*",
      "Action": "sqs:SendMessage",
      "Resource": "arn:aws:sqs:*:*:${var.env-name}-forensic",
      "Condition": {
        "ArnEquals":  { "aws:SourceArn": "arn:aws:s3:::${var.forensic-report-bucket}" }
      }
    }
  ]
}
POLICY
}


resource "aws_sqs_queue" "forensic-report-queue2" {
  name = "${var.env-name}-forensic-large"
  delay_seconds = 0
// fifo_queue = true 
//  content_based_deduplication = true
  max_message_size = 2048
// 14 day maximum
  message_retention_seconds = 1209600
// Lambda to end as soon as possible no long queuing
  receive_wait_time_seconds = 0
  redrive_policy = "{\"deadLetterTargetArn\":\"${aws_sqs_queue.forensic-report-deadletter.arn}\",\"maxReceiveCount\":3}"

}

resource "aws_sqs_queue" "forensic-report-deadletter" {
  name = "${var.env-name}-forensic-dead"
  delay_seconds = 0 
//  fifo_queue = true
//  content_based_deduplication = true
  max_message_size = 2048
// 14 day maximum
  message_retention_seconds = 1209600
// Lambda to end as soon as possible no long queuing
  receive_wait_time_seconds = 0

}

