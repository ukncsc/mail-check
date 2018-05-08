resource "aws_sqs_queue" "microservice-input-queue" {
  name          = "TF-${var.env-name}-${var.service-name}-input"
  delay_seconds = 0

  // 14 day maximum
  message_retention_seconds = 1209600
  receive_wait_time_seconds = 0
  redrive_policy            = "{\"deadLetterTargetArn\":\"${aws_sqs_queue.microservice-input-deadletter.arn}\",\"maxReceiveCount\":3}"
}

data "aws_iam_policy_document" "allow_sendmessage-sns" {
  statement {
    actions = ["SQS:SendMessage"]

    principals {
      type        = "AWS"
      identifiers = ["*"]
    }

    resources = ["${aws_sqs_queue.microservice-input-queue.arn}"]

    condition {
      test     = "ArnEquals"
      variable = "aws:SourceArn"

      values = [
        "${split(",",var.input-queue-subscriptions)}",
      ]
    }
  }
}

resource "aws_sqs_queue_policy" "input-queue-policy" {
  count     = "${var.input-queue-subscription-count > 0 ? 1 :0}"
  queue_url = "${aws_sqs_queue.microservice-input-queue.id}"
  policy    = "${data.aws_iam_policy_document.allow_sendmessage-sns.json}"
}

resource "aws_sqs_queue" "microservice-input-deadletter" {
  name          = "TF-${var.env-name}-${var.service-name}-input-DeadLetter"
  delay_seconds = 0

  // 14 day maximum
  message_retention_seconds = 1209600

  // Lambda to end as soon as possible no long queuing
  receive_wait_time_seconds = 0
}

resource "aws_cloudwatch_metric_alarm" "queue-length" {
  alarm_name          = "TF-${var.env-name}-${var.service-name}-input"
  comparison_operator = "GreaterThanOrEqualToThreshold"
  evaluation_periods  = "2"
  metric_name         = "ApproximateNumberOfMessagesVisible"
  namespace           = "AWS/SQS"
  period              = "120"
  statistic           = "Average"
  threshold           = "100"

  dimensions {
    QueueName = "${aws_sqs_queue.microservice-input-queue.name}"
  }

  alarm_description = "SQS queue length"

  alarm_actions = ["${var.cloudwatch-alerts-sns-arn}"]
}

resource "aws_cloudwatch_metric_alarm" "deadletter" {
  alarm_name          = "TF-${var.env-name}-${var.service-name}-input-DeadLetter"
  comparison_operator = "GreaterThanOrEqualToThreshold"
  evaluation_periods  = "2"
  metric_name         = "NumberOfMessagesSent"
  namespace           = "AWS/SQS"
  period              = "120"
  statistic           = "Average"
  threshold           = "1"

  dimensions {
    QueueName = "${aws_sqs_queue.microservice-input-deadletter.name}"
  }

  alarm_description = "Messages delivered to deadletter queue"

  alarm_actions = ["${var.cloudwatch-alerts-sns-arn}"]
}
