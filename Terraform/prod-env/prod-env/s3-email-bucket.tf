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

resource "aws_s3_bucket" "forensic-bucket" {
  bucket = "${var.forensic-report-bucket}"
  count  = "${var.create-buckets}"
  acl    = "private"

  versioning {
    enabled = true
  }

  tags {
    Name = "Forensic report emails"
  }
}
