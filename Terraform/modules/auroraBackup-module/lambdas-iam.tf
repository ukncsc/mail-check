# Create IAM Roles for lamdba functions
resource "aws_iam_role" "lambda-role" {
  name = "TF-${var.env-name}-aurora-snapshot-lambda"

  assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": "sts:AssumeRole",
      "Principal": {
        "Service": "lambda.amazonaws.com"
      },
      
      "Effect": "Allow",
      "Sid": ""
    }
  ]
}
EOF
}

# Attach VPCAccess execution policy to role

resource "aws_iam_role_policy_attachment" "lambda-vpc-exec" {
  role       = "${aws_iam_role.lambda-role.name}"
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaVPCAccessExecutionRole"
}

# Define the policy to access databases
data "aws_iam_policy_document" "lambda-rds-policy" {
  statement {
    actions = ["rds:CreateDBClusterSnapshot",
      "rds:DeleteDBClusterSnapshot",
      "rds:DescribeDBClusters",
      "rds:DescribeDBClusterSnapshots",
      "rds:CreateDBClusterSnapshot",
      "rds:DeleteDBClusterSnapshot",
      "rds:ModifyDBClusterSnapshotAttribute",
      "rds:DescribeDBClusterSnapshotAttributes",
      "rds:CopyDBClusterSnapshot",
      "rds:ListTagsForResource",
    ]

    resources = ["*"]
  }
}

resource "aws_iam_policy" "lambda-rds-policy" {
  name        = "TF-${var.env-name}-aurora-snapshot-rds"
  path        = "/"
  description = "Lambda create snapshots"
  policy      = "${data.aws_iam_policy_document.lambda-rds-policy.json}"
}

resource "aws_iam_role_policy_attachment" "lambda-rds-policy" {
  role       = "${aws_iam_role.lambda-role.name}"
  policy_arn = "${aws_iam_policy.lambda-rds-policy.arn}"
}

# Define the log policies

data "aws_iam_policy_document" "log-policy" {
  statement {
    actions = ["logs:CreateLogGroup",
      "logs:CreateLogStream",
      "logs:PutLogEvents",
    ]

    resources = ["arn:aws:logs:*:*:*"]
  }
}

resource "aws_iam_policy" "log-policy" {
  name        = "TF-${var.env-name}-aurora-snapshot-log"
  path        = "/"
  description = "Lambda to create cloudwatch logs"
  policy      = "${data.aws_iam_policy_document.log-policy.json}"
}

resource "aws_iam_role_policy_attachment" "attach-log" {
  role       = "${aws_iam_role.lambda-role.name}"
  policy_arn = "${aws_iam_policy.log-policy.arn}"
}

# Define the kms policies

data "aws_iam_policy_document" "kms-policy" {
  statement {
    actions = ["kms:Encrypt",
      "kms:Decrypt",
      "kms:ReEncrypt*",
      "kms:GenerateDataKey*",
      "kms:DescribeKey",
      "kms:CreateGrant",
      "kms:ListGrants",
      "kms:RevokeGrant",
    ]

    resources = ["*"]
  }
}

resource "aws_iam_policy" "kms-policy" {
  name        = "TF-${var.env-name}-aurora-snapshot-kms"
  path        = "/"
  description = "Lambda to use kms keys"
  policy      = "${data.aws_iam_policy_document.kms-policy.json}"
}

resource "aws_iam_role_policy_attachment" "attach-kms" {
  role       = "${aws_iam_role.lambda-role.name}"
  policy_arn = "${aws_iam_policy.kms-policy.arn}"
}
