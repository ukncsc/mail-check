output "sns-arn" {
  value = "${aws_sns_topic.microservice-output.arn}"
}

output "sqs-arn" {
  value = ["${aws_sqs_queue.microservice-input-queue.*.arn}"]
}
