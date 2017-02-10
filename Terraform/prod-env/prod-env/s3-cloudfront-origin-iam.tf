resource "aws_cloudfront_origin_access_identity" "origin_access_identity" {
  comment = "TF-Origin identity for cloudfront"
}

data "aws_iam_policy_document" "origin_s3_policy" {
  statement {
    actions   = ["s3:GetObject"]
    resources = ["arn:aws:s3:::ncsc-dmarc-${var.env-name}-app/*"]

    principals {
      type        = "AWS"
      identifiers = ["${aws_cloudfront_origin_access_identity.origin_access_identity.iam_arn}"]
    }
  }

  statement {
    actions   = ["s3:ListBucket"]
    resources = ["arn:aws:s3:::ncsc-dmarc-${var.env-name}-app"]

    principals {
      type        = "AWS"
      identifiers = ["${aws_cloudfront_origin_access_identity.origin_access_identity.iam_arn}"]
    }
  }
}

