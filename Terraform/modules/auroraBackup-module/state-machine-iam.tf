# Create IAM Roles for state machine invocation by scheduler

resource "aws_iam_role" "scheduler-invoke-sfn" {
  name = "TF-${var.env-name}-aurora-snapshot-sfn"

  assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": "sts:AssumeRole",
      "Principal": {
        "Service": "events.amazonaws.com"
      },
      "Effect": "Allow",
      "Sid": ""
    }
  ]
}
EOF
}

# Define the policy to start state machine execution from scheduler

data "aws_iam_policy_document" "state-invocation-policy" {
  statement {
    actions   = ["states:StartExecution"]
    resources = ["*"]
  }
}

resource "aws_iam_policy" "state-invocation-policy" {
  name        = "TF-${var.env-name}-aurora-snapshot-state-from-scheduler"
  path        = "/"
  description = "Invoke state machine for aurora snapshots"
  policy      = "${data.aws_iam_policy_document.state-invocation-policy.json}"
}

resource "aws_iam_role_policy_attachment" "state-invocation-policy" {
  role       = "${aws_iam_role.scheduler-invoke-sfn.name}"
  policy_arn = "${aws_iam_policy.state-invocation-policy.arn}"
}

# Create IAM Roles for state machine to call lambda

resource "aws_iam_role" "sfn-invoke-lambda" {
  name = "TF-${var.env-name}-aurora-snapshot-sfn-lambda"

  assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": "sts:AssumeRole",
      "Principal": {
        "Service": "states.amazonaws.com"
      },
      "Effect": "Allow",
      "Sid": ""
    }
  ]
}
EOF
}

# Define the policy start state machine execution

data "aws_iam_policy_document" "sfn-invoke-lambda" {
  statement {
    actions = ["lambda:InvokeFunction"]

    resources = ["${concat(aws_lambda_function.lambdaTakeSnapshotsAurora.*.arn, aws_lambda_function.lambdaShareSnapshotsAurora.*.arn,aws_lambda_function.lambdaCopySnapshotsAurora.*.arn,aws_lambda_function.lambdaDeleteOldSnapshotsAurora.*.arn,aws_lambda_function.lambdaDeleteOldSnapshotsDestAurora.*.arn)}"]
  }
}

resource "aws_iam_policy" "sfn-invoke-lambda" {
  name        = "TF-${var.env-name}-aurora-snapshot-lambda-from-state"
  path        = "/"
  description = "Invoke state machine for aurora snapshots"
  policy      = "${data.aws_iam_policy_document.sfn-invoke-lambda.json}"
}

resource "aws_iam_role_policy_attachment" "sfn-invoke-lambda" {
  role       = "${aws_iam_role.sfn-invoke-lambda.name}"
  policy_arn = "${aws_iam_policy.sfn-invoke-lambda.arn}"
}
