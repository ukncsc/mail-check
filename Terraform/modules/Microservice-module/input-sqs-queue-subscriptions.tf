resource "aws_sns_topic_subscription" "sns-subscription" {
  count                = "${var.input-queue-subscription-count}"
  topic_arn            = "${element(split(",",var.input-queue-subscriptions),count.index)}"
  protocol             = "sqs"
  endpoint             = "${aws_sqs_queue.microservice-input-queue.arn}"
  raw_message_delivery = "${var.raw-queue-subscription}"
}
