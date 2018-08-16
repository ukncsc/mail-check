resource "aws_s3_bucket" "aggregate-bucket" {
  bucket = "${var.aggregate-report-bucket}"
  count  = "${var.create-buckets}"
  acl    = "private"

  versioning {
    enabled = true
  }

  tags {
    Name = "Aggregate report emails"
  }
}




resource "aws_s3_bucket_policy" "replication-policy" {
  bucket = "${aws_s3_bucket.aggregate-bucket.id}"
  policy =<<POLICY
{
  "Version": "2012-10-17",
  "Id": "S3ReplicationPolicy",
  "Statement": [
    {
    "Sid": "ReadVersioningAllow",
            "Effect": "Allow",
            "Principal": {
                "AWS": "arn:aws:iam::${var.s3-replication-source-aws-account-id != "" ? var.s3-replication-source-aws-account-id : var.aws-account-id }:root"
            },
            "Action": "s3:GetBucketVersioning",
            "Resource": "${aws_s3_bucket.aggregate-bucket.arn}"
        },
    {
      "Sid": "ReplicationAllow",
      "Effect": "Allow",
      "Principal": {
            "AWS": "arn:aws:iam::${var.s3-replication-source-aws-account-id != "" ? var.s3-replication-source-aws-account-id : var.aws-account-id }:root"
      },
      "Action": [
                "s3:ReplicateObject",
                "s3:ReplicateDelete",
                "s3:ReplicateTags",
                "s3:ObjectOwnerOverrideToBucketOwner"
            ],
      "Resource": "${aws_s3_bucket.aggregate-bucket.arn}/*"
    },
    {
      "Sid": "ReadAllow",
      "Effect": "Allow",
      "Principal": {
            "AWS": "arn:aws:iam::${var.aws-account-id}:root"
      },
      "Action": [
                "s3:GetObject"
            ],
      "Resource": "${aws_s3_bucket.aggregate-bucket.arn}/*"
    } 
  ]
}
POLICY
}

