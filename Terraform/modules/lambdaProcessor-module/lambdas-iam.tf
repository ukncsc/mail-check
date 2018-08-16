# Create IAM Roles for aggregate and forensic report processors

resource "aws_iam_role" "lambda-processor" {
  name = "${var.lambda-function-name}"

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
  role       = "${aws_iam_role.lambda-processor.name}"
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaVPCAccessExecutionRole"
}

# Define the policy to access database credentials in parameter store
data "aws_iam_policy_document" "lambda-parameter-store-policy" {
  statement {
    actions   = ["ssm:DescribeParameters"]
    resources = ["*"]
  }

  statement {
    actions   = ["ssm:GetParameter"]
    resources = ["arn:aws:ssm:${var.aws-region}:${var.aws-account-id}:parameter/${var.env-name}*"]
  }
}

resource "aws_iam_policy" "lambda-parameter-store-policy" {
  name        = "${var.lambda-function-name}-parameterstore"
  path        = "/"
  description = "Lambda to SSM parameter store"
  policy      = "${data.aws_iam_policy_document.lambda-parameter-store-policy.json}"
}

resource "aws_iam_role_policy_attachment" "lambda-parameter-store" {
  role       = "${aws_iam_role.lambda-processor.name}"
  policy_arn = "${aws_iam_policy.lambda-parameter-store-policy.arn}"
}

# Define the sns policies to allow processor to publish to queues

data "aws_iam_policy_document" "sns-policy" {
  statement {
    actions = ["sns:Publish"]

    resources = [ "${concat(list(aws_sns_topic.lambdaprocessor-output.arn),var.sns-arns)}" ]
  }
}

resource "aws_iam_policy" "sns-policy" {
  name        = "${var.lambda-function-name}-sns"
  path        = "/"
  description = "Lambda to publish to SNS topics"
  policy      = "${data.aws_iam_policy_document.sns-policy.json}"
}

resource "aws_iam_role_policy_attachment" "attach-sns" {
  role       = "${aws_iam_role.lambda-processor.name}"
  policy_arn = "${aws_iam_policy.sns-policy.arn}"
}

# Define the sqs policies to allow processor to access queues.

data "aws_iam_policy_document" "sqs-policy" {
  statement {
    actions = ["sqs:ReceiveMessage",
      "sqs:DeleteMessage",
    ]

    resources = ["${split(",",var.sqs-queue-arns)}"]
  }
}

resource "aws_iam_policy" "sqs-policy" {
  count       = "${var.sqs-queue-count == "0" ? 0 : 1 }"
  name        = "${var.lambda-function-name}-sqs"
  path        = "/"
  description = "Lambda to access SQS queues"
  policy      = "${data.aws_iam_policy_document.sqs-policy.json}"
}

resource "aws_iam_role_policy_attachment" "attach-sqs" {
  count      = "${var.sqs-queue-count == "0" ? 0 : 1 }"
  role       = "${aws_iam_role.lambda-processor.name}"
  policy_arn = "${aws_iam_policy.sqs-policy.arn}"
}

# Define the s3 policies for processors

data "aws_iam_policy_document" "s3-policy" {
  statement {
    actions = ["s3:GetObject"]

    resources = ["${split(",",var.s3-bucket-arns)}"]
  }
}

resource "aws_iam_policy" "s3-policy" {
  count       = "${var.s3-bucket-arns == "" ? 0 : 1 }"
  name        = "${var.lambda-function-name}-s3"
  path        = "/"
  description = "Lambda to access s3 buckets"
  policy      = "${data.aws_iam_policy_document.s3-policy.json}"
}

resource "aws_iam_role_policy_attachment" "attach-s3" {
  count      = "${var.s3-bucket-arns == "" ? 0 : 1 }"
  role       = "${aws_iam_role.lambda-processor.name}"
  policy_arn = "${aws_iam_policy.s3-policy.arn}"
}
