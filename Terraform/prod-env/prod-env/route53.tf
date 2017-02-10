data "aws_route53_zone" "dmarc-zone" {
  name = "${var.parent-zone}"
}

resource "aws_route53_record" "app" {
  zone_id = "${data.aws_route53_zone.dmarc-zone.zone_id}"
  name = "${var.web-url}"
  type = "CNAME"
  ttl = "600"
  records = ["${aws_cloudfront_distribution.dmarc_distribution.domain_name}"]
}

