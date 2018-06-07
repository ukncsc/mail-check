output "sns-topicBackupsFailed" {
  value = "${element(concat(aws_sns_topic.topicBackupsFailed.*.arn,list("")),0)}"
}

output "sns-topicShareFailed" {
  value = "${element(concat(aws_sns_topic.topicShareFailed.*.arn,list("")),0)}"
}

output "sns-topicDeleteOldFailed" {
  value = "${element(concat(aws_sns_topic.topicDeleteOldFailed.*.arn,list("")),0)}"
}

output "sns-topicCopyFailedDest" {
  value = "${element(concat(aws_sns_topic.topicCopyFailedDest.*.arn,list("")),0)}"
}

output "sns-topicDeleteOldDestFailed" {
  value = "${element(concat(aws_sns_topic.topicDeleteOldDestFailed.*.arn,list("")),0)}"
}
