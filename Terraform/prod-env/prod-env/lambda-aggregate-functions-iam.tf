resource "aws_iam_role" "lambda-aggregate-reports" {
    name = "TF-${var.env-name}-lambda-aggregate-reports"
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

resource "aws_iam_role_policy_attachment" "lambda-report-attach" {
    role = "${aws_iam_role.lambda-aggregate-reports.name}"
    policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaVPCAccessExecutionRole"
}

resource "aws_iam_role" "lambda-report-processor" {
    name = "TF-${var.env-name}-lambda-report-processor"
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

data "aws_iam_policy_document" "sqs-policy" {
    statement {
        actions =  [ "sqs:ReceiveMessage",
		     "sqs:DeleteMessage" ]
	resources = [
		"${aws_sqs_queue.aggregate-report-queue1.arn}" ,
		"${aws_sqs_queue.aggregate-report-queue2.arn}" ]
		}
}

data "aws_iam_policy_document" "s3-aggregate-policy" {
    statement {
        actions =  [ "s3:GetObject" ]
	resources = [
		"arn:aws:s3:::${var.aggregate-report-bucket}/*" ]
		}
}

resource "aws_iam_policy" "s3-aggregate-policy" {
    name = "TF-${var.env-name}-s3-aggregate-policy"
    path = "/"
    description = "Lambda to access s3 aggregate report bucket"
    policy = "${data.aws_iam_policy_document.s3-aggregate-policy.json}"
}

resource "aws_iam_policy" "sqs-iam-policy" {
    name = "TF-${var.env-name}-sqs-policy"
    path = "/"
    description = "Lambda to access SQS queue"
    policy = "${data.aws_iam_policy_document.sqs-policy.json}"
}

resource "aws_iam_role_policy_attachment" "lambda-processor-attach-vpc" {
    role = "${aws_iam_role.lambda-report-processor.name}"
    policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaVPCAccessExecutionRole"
}

resource "aws_iam_role_policy_attachment" "lambda-processor-attach-sqs" {
    role = "${aws_iam_role.lambda-report-processor.name}"
    policy_arn = "${aws_iam_policy.sqs-iam-policy.arn}"
}

resource "aws_iam_role_policy_attachment" "lambda-processor-attach-s3-aggregate" {
    role = "${aws_iam_role.lambda-report-processor.name}"
    policy_arn = "${aws_iam_policy.s3-aggregate-policy.arn}"
}

