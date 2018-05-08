# Create IAM Roles for Aurora to export to s3 bucket

resource "aws_iam_role" "rds-s3-export" {
  name = "TF-db-export-${var.env-name}"

  assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": "sts:AssumeRole",
      "Principal": {
        "Service": "rds.amazonaws.com"
      },
      "Effect": "Allow",
      "Sid": ""
    }
  ]
}
EOF
}


# Define the s3 policies aurora export

data "aws_iam_policy_document" "s3-export-policy" {
  statement {
    actions = ["s3:PutObject"]
    resources = ["${aws_s3_bucket.db-export-bucket.arn}"]
  }
}

resource "aws_iam_policy" "s3-export-policy" {
  name        = "TF-db-export-${var.env-name}-s3"
  path        = "/"
  description = "Aurora export to s3 bucket"
  policy      = "${data.aws_iam_policy_document.s3-export-policy.json}"
}

resource "aws_iam_role_policy_attachment" "attach-s3" {
  role       = "${aws_iam_role.rds-s3-export.name}"
  policy_arn = "${aws_iam_policy.s3-export-policy.arn}"
}
