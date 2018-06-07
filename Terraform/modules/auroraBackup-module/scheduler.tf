# Scheduled tasks for source account

resource "aws_cloudwatch_event_rule" "stateMachineTakeSnapshotsAurora" {
  count               = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  name                = "TF-${var.env-name}-stateMachineTakeSnapshotsAurora"
  description         = "Triggers the BackupAurora state machine"
  schedule_expression = "cron(${var.BackupSchedule})"
}

resource "aws_cloudwatch_event_target" "stateMachineTakeSnapshotsAurora" {
  count    = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  rule     = "${aws_cloudwatch_event_rule.stateMachineTakeSnapshotsAurora.name}"
  arn      = "${aws_sfn_state_machine.stateMachineTakeSnapshotsAurora.id}"
  role_arn = "${aws_iam_role.scheduler-invoke-sfn.arn}"
}

resource "aws_cloudwatch_event_rule" "statemachineShareSnapshotsAurora" {
  count               = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  name                = "TF-${var.env-name}-statemachineShareSnapshotsAurora"
  description         = "Triggers the BackupAurora state machine"
  schedule_expression = "cron(/10 * * * ? *)"
}

resource "aws_cloudwatch_event_target" "statemachineShareSnapshotsAurora" {
  count    = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  rule     = "${aws_cloudwatch_event_rule.statemachineShareSnapshotsAurora.name}"
  arn      = "${aws_sfn_state_machine.statemachineShareSnapshotsAurora.id}"
  role_arn = "${aws_iam_role.scheduler-invoke-sfn.arn}"
}

resource "aws_cloudwatch_event_rule" "statemachineDeleteOldSnapshotsAurora" {
  count               = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  name                = "TF-${var.env-name}-statemachineDeleteOldSnapshotsAurora"
  description         = "Triggers the BackupAurora state machine"
  schedule_expression = "cron(/10 * * * ? *)"
}

resource "aws_cloudwatch_event_target" "statemachineDeleteOldSnapshotsAurora" {
  count    = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  rule     = "${aws_cloudwatch_event_rule.statemachineDeleteOldSnapshotsAurora.name}"
  arn      = "${aws_sfn_state_machine.statemachineDeleteOldSnapshotsAurora.id}"
  role_arn = "${aws_iam_role.scheduler-invoke-sfn.arn}"
}

# Scheduled tasks for destination acccount

resource "aws_cloudwatch_event_rule" "statemachineCopySnapshotsDestAurora" {
  count               = "${var.source-or-dest == "DEST" ? 1 :0}"
  name                = "TF-${var.env-name}-statemachineCopySnapshotsDestAurora"
  description         = "Triggers the BackupAurora state machine"
  schedule_expression = "cron(/30 * * * ? *)"
}

resource "aws_cloudwatch_event_target" "statemachineCopySnapshotsDestAurora" {
  count    = "${var.source-or-dest == "DEST" ? 1 :0}"
  rule     = "${aws_cloudwatch_event_rule.statemachineCopySnapshotsDestAurora.name}"
  arn      = "${aws_sfn_state_machine.statemachineCopySnapshotsDestAurora.id}"
  role_arn = "${aws_iam_role.scheduler-invoke-sfn.arn}"
}

resource "aws_cloudwatch_event_rule" "statemachineDeleteOldSnapshotsDestAurora" {
  count               = "${var.source-or-dest == "DEST" ? 1 :0}"
  name                = "TF-${var.env-name}-statemachineDeleteOldSnapshotsDestAurora"
  description         = "Triggers the BackupAurora state machine"
  schedule_expression = "cron(0 /1 * * ? *)"
}

resource "aws_cloudwatch_event_target" "statemachineDeleteOldSnapshotsDestAurora" {
  count    = "${var.source-or-dest == "DEST" ? 1 :0}"
  rule     = "${aws_cloudwatch_event_rule.statemachineDeleteOldSnapshotsDestAurora.name}"
  arn      = "${aws_sfn_state_machine.statemachineDeleteOldSnapshotsDestAurora.id}"
  role_arn = "${aws_iam_role.scheduler-invoke-sfn.arn}"
}
