
resource "aws_s3_bucket" "aggregate-bucket" {
  bucket = "ncsc-dmarc-aggregate-reports"
  acl    = "private"
  policy = <<EOF
{
    "Version": "2008-10-17",
    "Statement": [{
        "Sid": "GiveSESPermissionToWritAggregate",
        "Effect": "Allow",
        "Principal": {
          "Service": "ses.amazonaws.com"
        },
        "Action": "s3:PutObject",
        "Resource": "arn:aws:s3:::ncsc-dmarc-aggregate-reports/*",
        "Condition": {
          "StringEquals": {
            "aws:Referer": "${var.aws-account-id}"
          }
        }
    }]
}
EOF
  tags {
    Name = "Aggregate report emails delivered by SES"
  }
}

resource "aws_s3_bucket" "forensic-bucket" {
  bucket = "ncsc-dmarc-forensic-reports"
  acl    = "private"
  policy =  <<EOF
{
    "Version": "2008-10-17",
    "Statement": [{
        "Sid": "GiveSESPermissionToWriteForensic",
        "Effect": "Allow",
        "Principal": {
          "Service": "ses.amazonaws.com"
        },
        "Action": "s3:PutObject",
        "Resource": "arn:aws:s3:::ncsc-dmarc-forensic-reports/*",
        "Condition": {
          "StringEquals": {
            "aws:Referer": "${var.aws-account-id}"
          }
        }
    }]
}
EOF
  tags {
    Name = "Forensic report emails delivered by SES"
  }
}

resource "aws_s3_bucket" "admin-bucket" {
  bucket = "ncsc-dmarc-admin-emails"
  acl    = "private"
  policy =  <<EOF
{
    "Version": "2008-10-17",
    "Statement": [{
        "Sid": "GiveSESPermissionToWriteAdmin",
        "Effect": "Allow",
        "Principal": {
          "Service": "ses.amazonaws.com"
        },
        "Action": "s3:PutObject",
        "Resource": "arn:aws:s3:::ncsc-dmarc-admin-emails/*",
        "Condition": {
          "StringEquals": {
            "aws:Referer": "${var.aws-account-id}"
          }
        }
    }]
}
EOF
  tags {
    Name = "Admin emails delivered by SES - domain validation"
  }
}






