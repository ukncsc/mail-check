resource "aws_s3_bucket" "app-bucket" {
  bucket = "ncsc-dmarc-${var.env-name}-app"
  acl    = "private"
  policy = "${data.aws_iam_policy_document.origin_s3_policy.json}"
  tags {
    Name = "HTML code and Javascript application"
  }
}


resource "aws_s3_bucket" "cloudfront-log-bucket" {
  bucket = "ncsc-dmarc-${var.env-name}-cloudfront-logs"
  acl    = "private"
#  policy = "${data.aws_iam_policy_document.origin_s3_policy.json}"
  tags {
    Name = "Logs"
  }
}
