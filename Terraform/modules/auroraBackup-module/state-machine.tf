resource "aws_sfn_state_machine" "stateMachineTakeSnapshotsAurora" {
  count    = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  name     = "TF-${var.env-name}-stateMachineTakeSnapshotsAurora"
  role_arn = "${aws_iam_role.sfn-invoke-lambda.arn}"

  definition = <<EOF
{
    "Comment": "Triggers snapshot backup for Aurora clusters",
    "StartAt": "TakeSnapshots",
    "States": {
        "TakeSnapshots": {
            "Type": "Task",
            "Resource": "${aws_lambda_function.lambdaTakeSnapshotsAurora.arn}",
            "Retry": [
                {
                    "ErrorEquals": [ "SnapshotToolException" ],
                    "IntervalSeconds": 300,
                    "MaxAttempts": 20,
                    "BackoffRate": 1
                },
                {
                    "ErrorEquals": [ "States.ALL" ],
                    "IntervalSeconds": 30,
                    "MaxAttempts": 20,
                    "BackoffRate": 1
                } ] ,
                "End": true 
        }
    }
}
EOF
}

resource "aws_sfn_state_machine" "statemachineShareSnapshotsAurora" {
  count    = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  name     = "TF-${var.env-name}-statemachineShareSnapshotsAurora"
  role_arn = "${aws_iam_role.sfn-invoke-lambda.arn}"

  definition = <<EOF
{
    "Comment": "Shares snapshots with DEST_ACCOUNT",
    "StartAt": "ShareSnapshots",
    "States": {
        "ShareSnapshots": {
            "Type": "Task",
            "Resource": "${aws_lambda_function.lambdaShareSnapshotsAurora.arn}",
            "Retry": [
                {
                    "ErrorEquals": [ "SnapshotToolException" ],
                    "IntervalSeconds": 300,
                    "MaxAttempts": 3,
                    "BackoffRate": 1
                },
                {
                    "ErrorEquals": [ "States.ALL" ],
                    "IntervalSeconds": 30,
                    "MaxAttempts": 10,
                    "BackoffRate": 1
                } ],
                "End": true 
        }
    }
}
EOF
}

resource "aws_sfn_state_machine" "statemachineDeleteOldSnapshotsAurora" {
  count    = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  name     = "TF-${var.env-name}-statemachineDeleteOldSnapshotsAurora"
  role_arn = "${aws_iam_role.sfn-invoke-lambda.arn}"

  definition = <<EOF
{
    "Comment": "DeleteOld management for Aurora snapshots",
    "StartAt": "DeleteOld",
    "States": {
        "DeleteOld": {
            "Type": "Task",
            "Resource": "${aws_lambda_function.lambdaDeleteOldSnapshotsAurora.arn}",
            "Retry": [
                {
                    "ErrorEquals": [ "SnapshotToolException" ],
                    "IntervalSeconds": 300,
                    "MaxAttempts": 7,
                    "BackoffRate": 1
                },
                {
                    "ErrorEquals": [ "States.ALL" ],
                    "IntervalSeconds": 30,
                    "MaxAttempts": 10,
                    "BackoffRate": 1
                } ],
                "End": true 
        }
    }
}
EOF
}

resource "aws_sfn_state_machine" "statemachineCopySnapshotsDestAurora" {
  count    = "${var.source-or-dest == "DEST" ? 1 :0}"
  name     = "TF-${var.env-name}-statemachineCopySnapshotsDestAurora"
  role_arn = "${aws_iam_role.sfn-invoke-lambda.arn}"

  definition = <<EOF
{
    "Comment": "Copies snapshots locally and then to DEST_REGION",
    "StartAt": "CopySnapshots",
    "States": {
        "CopySnapshots": {
            "Type": "Task",
            "Resource": "${aws_lambda_function.lambdaCopySnapshotsAurora.arn}",
            "Retry": [
                {
                    "ErrorEquals": [ "SnapshotToolException" ],
                    "IntervalSeconds": 300,
                    "MaxAttempts": 5,
                    "BackoffRate": 1
                },
                {
                    "ErrorEquals": [ "States.ALL" ],
                    "IntervalSeconds": 30,
                    "MaxAttempts": 20,
                    "BackoffRate": 1
                } ],
                "End": true 
        }
    }
}
EOF
}

resource "aws_sfn_state_machine" "statemachineDeleteOldSnapshotsDestAurora" {
  count    = "${var.source-or-dest == "DEST" ? 1 :0}"
  name     = "TF-${var.env-name}-statemachineDeleteOldSnapshotsDestAurora"
  role_arn = "${aws_iam_role.sfn-invoke-lambda.arn}"

  definition = <<EOF
{
    "Comment": "DeleteOld management for Aurora snapshots in destination region",
    "StartAt": "DeleteOld",
    "States": {
        "DeleteOld": {
            "Type": "Task",
            "Resource": "${aws_lambda_function.lambdaDeleteOldSnapshotsDestAurora.arn}",
            "Retry": [
                {
                    "ErrorEquals": [ "SnapshotToolException" ],
                    "IntervalSeconds": 600,
                    "MaxAttempts": 3,
                    "BackoffRate": 1
                },
                {
                    "ErrorEquals": [ "States.ALL" ],
                    "IntervalSeconds": 30,
                    "MaxAttempts": 10,
                    "BackoffRate": 1
                } ],
                "End": true 
        }
    }
}
EOF
}
