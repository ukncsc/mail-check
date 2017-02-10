
resource "aws_cloudfront_distribution" "dmarc_distribution" {
  origin {
    domain_name = "${aws_s3_bucket.app-bucket.bucket}.s3.amazonaws.com"
    origin_id   = "app-bucket"

    s3_origin_config {
      origin_access_identity = "${aws_cloudfront_origin_access_identity.origin_access_identity.cloudfront_access_identity_path}"
    }
  }
  origin {
    domain_name = "${aws_api_gateway_rest_api.aggregate-report-api.id}.execute-api.${var.aws-region}.amazonaws.com"
    origin_id   = "api-gateway"
    custom_origin_config {
      https_port = "443"
      http_port  = "80"
      origin_protocol_policy = "https-only"
      origin_ssl_protocols = ["TLSv1.2"]
    }

  }

  enabled             = true
  is_ipv6_enabled     = true
  web_acl_id	      = "${aws_waf_web_acl.waf_acl.id}"
  comment             = "Static DMARC application"
  default_root_object = "index.html"

  logging_config {
    include_cookies = true
    bucket          = "ncsc-dmarc-${var.env-name}-cloudfront-logs.s3.amazonaws.com"
    prefix          = ""
}

  aliases = ["${var.web-url}"]

  default_cache_behavior {
    allowed_methods  = ["DELETE", "GET", "HEAD", "OPTIONS", "PATCH", "POST", "PUT"]
    cached_methods   = ["GET", "HEAD"]
    target_origin_id = "app-bucket"

    forwarded_values {
      query_string = false

      cookies {
        forward = "none"
      }
    }

    viewer_protocol_policy = "redirect-to-https"
    min_ttl                = 0
    default_ttl            = 600
    max_ttl                = 600
  }

cache_behavior {
    path_pattern = "api/*"
    allowed_methods  = ["DELETE", "GET", "HEAD", "OPTIONS", "PATCH", "POST", "PUT"]
    cached_methods   = ["GET", "HEAD"]
    target_origin_id = "api-gateway"

    forwarded_values {
      query_string = true
      headers = [ "Accept", "Authorization" ]

      cookies {
        forward = "all"
      }
    }

    viewer_protocol_policy = "redirect-to-https"
    min_ttl                = 0
    default_ttl            = 0
    max_ttl                = 0
  }
  price_class = "PriceClass_100"

  restrictions {
    geo_restriction {
#      restriction_type = "whitelist"
#      locations        = ["GB","US"]    
	restriction_type = "none"
    }
  }

  tags {
    Environment = "${var.env-name}"
  }
  custom_error_response {
	error_code = "500"
	error_caching_min_ttl = "10"
  }
  custom_error_response {
	error_code = "502"
	error_caching_min_ttl = "10"
  }
  custom_error_response {
	error_code = "504"
	error_caching_min_ttl = "10"
  }
  custom_error_response {
	error_code = "404"
	error_caching_min_ttl = "10"
  }
  custom_error_response {
	error_code = "403"
	error_caching_min_ttl = "10"
  }
  viewer_certificate {
    acm_certificate_arn = "${var.acm-certificate}"
    ssl_support_method = "sni-only" 
    minimum_protocol_version = "TLSv1" // has to be this value with ACM certs
  }
}
