resource "aws_s3_bucket" "lblogs" {
  bucket = "tf-ncsc-dmarc-${var.env-name}-${var.balancer-name}-accesslogs"
  acl    = "private"

  policy = <<EOF
{
"Version": "2012-10-17",
"Statement": [
{
"Effect": "Allow",
   "Principal": {
        "AWS": [
          "652711504416"
        ]
      },
"Action": "s3:PutObject",
"Resource": "arn:aws:s3:::tf-ncsc-dmarc-${var.env-name}-${var.balancer-name}-accesslogs/*"
}
]
}
EOF

  tags {
    Name = "Load balancer logs"
  }
}
