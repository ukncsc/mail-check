resource "aws_s3_bucket" "db-export-bucket" {
  bucket = "ncsc-dmarc-db-export-${var.env-name}"
  acl    = "private"

  versioning {
    enabled = true
  }
policy = <<EOF
{
    "Version": "2008-10-17",
    "Statement": [
        {
            "Sid": "GiveRDSPermissionToWriteExports",
            "Effect": "Allow",
            "Principal": {
                "AWS": "${aws_iam_role.rds-s3-export.arn}"
            },
            "Action": [
                "s3:PutObject",
                "s3:ListBucket"
            ],
            "Resource": [
                "arn:aws:s3:::ncsc-dmarc-db-export-${var.env-name}/*",
                "arn:aws:s3:::ncsc-dmarc-db-export-${var.env-name}"
            ]
        }
    ]
}
EOF

  tags {
    Name = "Database exports"
  }
}