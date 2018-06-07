resource "aws_cloudwatch_metric_alarm" "alarmcwBackupsFailed" {
  count               = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  alarm_name          = "TF-${var.env-name}-alarmcwBackupsFailed"
  comparison_operator = "GreaterThanOrEqualToThreshold"
  evaluation_periods  = "1"
  metric_name         = "ExecutionsFailed"
  namespace           = "AWS/States"
  period              = "300"
  statistic           = "Sum"
  threshold           = "1.0"

  dimensions {
    StateMachineArn = "${aws_sfn_state_machine.stateMachineTakeSnapshotsAurora.id}"
  }

  alarm_actions = ["${aws_sns_topic.topicBackupsFailed.arn}"]
}

resource "aws_cloudwatch_metric_alarm" "alarmcwShareFailed" {
  count               = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  alarm_name          = "TF-${var.env-name}-alarmcwShareFailed"
  comparison_operator = "GreaterThanOrEqualToThreshold"
  evaluation_periods  = "2"
  metric_name         = "ExecutionsFailed"
  namespace           = "AWS/States"
  period              = "3600"
  statistic           = "Sum"
  threshold           = "6.0"

  dimensions {
    StateMachineArn = "${aws_sfn_state_machine.statemachineShareSnapshotsAurora.id}"
  }

  alarm_actions = ["${aws_sns_topic.topicShareFailed.arn}"]
}

resource "aws_cloudwatch_metric_alarm" "alarmcwDeleteOldFailed" {
  count               = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  alarm_name          = "TF-${var.env-name}-alarmcwDeleteOldFailed"
  comparison_operator = "GreaterThanOrEqualToThreshold"
  evaluation_periods  = "2"
  metric_name         = "ExecutionsFailed"
  namespace           = "AWS/States"
  period              = "3600"
  statistic           = "Sum"
  threshold           = "2.0"

  dimensions {
    StateMachineArn = "${aws_sfn_state_machine.statemachineDeleteOldSnapshotsAurora.id}"
  }

  alarm_actions = ["${aws_sns_topic.topicDeleteOldFailed.arn}"]
}

resource "aws_cloudwatch_metric_alarm" "alarmcwDeleteOldDestFailed" {
  count               = "${var.source-or-dest == "DEST" ? 1 :0}"
  alarm_name          = "TF-${var.env-name}-alarmcwDeleteOldDestFailed"
  comparison_operator = "GreaterThanOrEqualToThreshold"
  evaluation_periods  = "2"
  metric_name         = "ExecutionsFailed"
  namespace           = "AWS/States"
  period              = "3600"
  statistic           = "Sum"
  threshold           = "2.0"

  dimensions {
    StateMachineArn = "${aws_sfn_state_machine.statemachineDeleteOldSnapshotsDestAurora.id}"
  }

  alarm_actions = ["${aws_sns_topic.topicDeleteOldDestFailed.arn}"]
}
